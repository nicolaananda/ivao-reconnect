namespace IvaoAuto;

internal sealed class SettingsForm : Form
{
    private readonly TextBox msfs = new();
    private readonly TextBox altitude = new();
    private readonly NumericUpDown delay = new();

    public SettingsForm(AppSettings settings)
    {
        Text = "IVAO Auto Reconnect Settings";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(430, 190);

        msfs.Text = settings.MsfsProcessName;
        altitude.Text = settings.AltitudeProcessName;
        delay.Minimum = 0.1m;
        delay.Maximum = 60;
        delay.DecimalPlaces = 1;
        delay.Increment = 0.1m;
        delay.Value = Math.Clamp(settings.ReconnectDelayMinutes, delay.Minimum, delay.Maximum);
        delay.Width = 120;

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Padding = new Padding(12),
            Height = 140,
            ColumnCount = 2,
            RowCount = 3
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        grid.Controls.Add(new Label { Text = "MSFS process (.exe)", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        grid.Controls.Add(msfs, 1, 0);
        grid.Controls.Add(new Label { Text = "Altitude process (.exe)", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
        grid.Controls.Add(altitude, 1, 1);
        grid.Controls.Add(new Label { Text = "Reconnect delay (minutes)", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
        grid.Controls.Add(delay, 1, 2);
        msfs.Dock = DockStyle.Fill;
        altitude.Dock = DockStyle.Fill;

        var save = new Button { Text = "Save", DialogResult = DialogResult.OK, Width = 90 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90 };
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 48,
            Padding = new Padding(10),
            FlowDirection = FlowDirection.RightToLeft
        };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        Controls.Add(grid);
        Controls.Add(buttons);
        AcceptButton = save;
        CancelButton = cancel;
    }

    public bool ApplyTo(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(msfs.Text) || string.IsNullOrWhiteSpace(altitude.Text))
        {
            MessageBox.Show("Nama proses MSFS dan Altitude wajib diisi.", "Invalid settings",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        settings.MsfsProcessName = Path.GetFileName(msfs.Text.Trim());
        settings.AltitudeProcessName = Path.GetFileName(altitude.Text.Trim());
        settings.ReconnectDelayMinutes = delay.Value;
        settings.Save();
        return true;
    }
}
