using System.Diagnostics;
using System.Media;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace IvaoAuto;

internal sealed class WatchdogContext : ApplicationContext
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

    private readonly NotifyIcon tray;
    private readonly System.Windows.Forms.Timer timer;
    private readonly ToolStripMenuItem monitoringItem;
    private readonly AppSettings settings = AppSettings.Load();
    private readonly string logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "IvaoAuto", "watchdog.log");

    private bool monitoring = true;
    private bool busy;
    private DateTimeOffset? offlineSince;
    private CancellationTokenSource shutdown = new();

    public WatchdogContext()
    {
        monitoringItem = new ToolStripMenuItem("Monitoring: On", null, (_, _) => ToggleMonitoring());
        var menu = new ContextMenuStrip();
        menu.Items.Add(monitoringItem);
        menu.Items.Add("Reconnect Now", null, async (_, _) => await ReconnectAsync(force: true));
        menu.Items.Add("Settings...", null, (_, _) => OpenSettings());
        menu.Items.Add("Open Log", null, (_, _) => OpenLog());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => Exit());

        tray = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "IVAO Auto Reconnect",
            ContextMenuStrip = menu,
            Visible = true
        };
        tray.DoubleClick += (_, _) => ToggleMonitoring();

        timer = new System.Windows.Forms.Timer { Interval = (int)PollInterval.TotalMilliseconds };
        timer.Tick += async (_, _) => await CheckAsync();
        timer.Start();

        Log("Started. Monitoring enabled.");
        Notify("IVAO Auto Reconnect", "Monitoring aktif.", ToolTipIcon.Info);
    }

    private async Task CheckAsync()
    {
        if (!monitoring || busy) return;

        try
        {
            using var automation = new UIA3Automation();
            var window = FindAltitudeWindow(automation, settings.AltitudeProcessName);
            if (window is null)
            {
                offlineSince = null;
                SetTray("IVAO: Altitude tidak ditemukan");
                return;
            }

            if (HasNamedElement(window, "ONLINE"))
            {
                offlineSince = null;
                SetTray("IVAO: Online");
                return;
            }

            if (!HasNamedElement(window, "OFFLINE"))
            {
                offlineSince = null;
                SetTray("IVAO: Status tidak dikenali");
                return;
            }

            offlineSince ??= DateTimeOffset.Now;
            SetTray("IVAO: Offline");
            if (DateTimeOffset.Now - offlineSince >= TimeSpan.FromMinutes((double)settings.ReconnectDelayMinutes))
                await ReconnectAsync(force: false);
        }
        catch (Exception ex)
        {
            Log($"Check error: {ex.Message}");
        }
    }

    private async Task ReconnectAsync(bool force)
    {
        if (busy) return;
        if (!force && !IsProcessRunning(settings.MsfsProcessName))
        {
            Log("Reconnect skipped: MSFS 2024 tidak berjalan.");
            offlineSince = null;
            return;
        }

        busy = true;
        timer.Stop();
        try
        {
            for (var attempt = 0; attempt < RetryDelays.Length; attempt++)
            {
                await Task.Delay(RetryDelays[attempt], shutdown.Token);
                Log($"Reconnect attempt {attempt + 1}/{RetryDelays.Length}.");

                using var automation = new UIA3Automation();
                var window = FindAltitudeWindow(automation, settings.AltitudeProcessName);
                if (window is null)
                {
                    Fail("Altitude tidak ditemukan.");
                    return;
                }
                if (HasNamedElement(window, "ONLINE"))
                {
                    Success();
                    return;
                }

                var offline = FindNamedElement(window, "OFFLINE");
                if (offline is null || !offline.IsEnabled)
                {
                    Log("OFFLINE button tidak ditemukan atau disabled.");
                    continue;
                }
                Invoke(offline);

                var connect = await WaitForElementAsync(automation, "CONNECT", TimeSpan.FromSeconds(10));
                if (connect is null || !connect.IsEnabled)
                {
                    Log("CONNECT button tidak ditemukan atau disabled.");
                    continue;
                }
                Invoke(connect);

                if (await WaitUntilOnlineAsync(automation, ConnectTimeout))
                {
                    Success();
                    return;
                }
                Log("Reconnect timeout.");
            }

            Fail("Reconnect gagal setelah 3 percobaan.");
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Fail($"Reconnect error: {ex.Message}");
        }
        finally
        {
            busy = false;
            offlineSince = null;
            if (!shutdown.IsCancellationRequested) timer.Start();
        }
    }

    private static Window? FindAltitudeWindow(UIA3Automation automation, string processName)
    {
        var processIds = Process.GetProcessesByName(AppSettings.NormalizeProcessName(processName))
            .Select(p => p.Id).ToHashSet();
        return automation.GetDesktop().FindAllChildren(cf => cf.ByControlType(ControlType.Window))
            .Select(x => x.AsWindow())
            .FirstOrDefault(x => processIds.Contains(x.Properties.ProcessId.Value)
                && x.Title.StartsWith("IVAO Pilot Client: Altitude", StringComparison.OrdinalIgnoreCase));
    }

    private static AutomationElement? FindNamedElement(AutomationElement root, string name) =>
        root.FindAllDescendants(cf => cf.ByName(name))
            .FirstOrDefault(x => x.ControlType == ControlType.Button || x.ControlType == ControlType.Custom);

    private static bool HasNamedElement(AutomationElement root, string name) => FindNamedElement(root, name) is not null;

    private static void Invoke(AutomationElement element)
    {
        if (element.Patterns.Invoke.IsSupported) element.Patterns.Invoke.Pattern.Invoke();
        else element.Click();
    }

    private static async Task<AutomationElement?> WaitForElementAsync(UIA3Automation automation, string name, TimeSpan timeout)
    {
        var deadline = DateTimeOffset.Now + timeout;
        while (DateTimeOffset.Now < deadline)
        {
            var element = automation.GetDesktop().FindFirstDescendant(cf => cf.ByName(name));
            if (element is not null) return element;
            await Task.Delay(500);
        }
        return null;
    }

    private async Task<bool> WaitUntilOnlineAsync(UIA3Automation automation, TimeSpan timeout)
    {
        var deadline = DateTimeOffset.Now + timeout;
        while (DateTimeOffset.Now < deadline)
        {
            var window = FindAltitudeWindow(automation, settings.AltitudeProcessName);
            if (window is not null && HasNamedElement(window, "ONLINE")) return true;
            await Task.Delay(1000);
        }
        return false;
    }

    private static bool IsProcessRunning(string processName) =>
        Process.GetProcessesByName(AppSettings.NormalizeProcessName(processName)).Length > 0;

    private void OpenSettings()
    {
        using var form = new SettingsForm(settings);
        if (form.ShowDialog() == DialogResult.OK && form.ApplyTo(settings))
        {
            offlineSince = null;
            Log($"Settings saved: MSFS={settings.MsfsProcessName}, Altitude={settings.AltitudeProcessName}, delay={settings.ReconnectDelayMinutes} minute(s).");
            Notify("Settings saved", "Pengaturan watchdog diperbarui.", ToolTipIcon.Info);
        }
    }

    private void ToggleMonitoring()
    {
        monitoring = !monitoring;
        monitoringItem.Text = $"Monitoring: {(monitoring ? "On" : "Off")}";
        offlineSince = null;
        Log($"Monitoring {(monitoring ? "enabled" : "paused")}.");
        SetTray(monitoring ? "IVAO: Monitoring aktif" : "IVAO: Monitoring pause");
    }

    private void Success()
    {
        Log("Reconnect successful.");
        Notify("IVAO tersambung", "Reconnect berhasil.", ToolTipIcon.Info);
    }

    private void Fail(string message)
    {
        Log(message);
        Notify("IVAO reconnect gagal", message, ToolTipIcon.Error);
        SystemSounds.Exclamation.Play();
    }

    private void OpenLog()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        if (!File.Exists(logPath)) File.WriteAllText(logPath, "");
        Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
    }

    private void Log(string message)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        File.AppendAllText(logPath, $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz} {message}{Environment.NewLine}");
    }

    private void Notify(string title, string text, ToolTipIcon icon)
    {
        tray.BalloonTipTitle = title;
        tray.BalloonTipText = text;
        tray.BalloonTipIcon = icon;
        tray.ShowBalloonTip(5000);
    }

    private void SetTray(string text) => tray.Text = text.Length <= 63 ? text : text[..63];

    private void Exit()
    {
        shutdown.Cancel();
        timer.Stop();
        tray.Visible = false;
        tray.Dispose();
        timer.Dispose();
        shutdown.Dispose();
        ExitThread();
    }
}
