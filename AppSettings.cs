using System.Text.Json;

namespace IvaoAuto;

internal sealed class AppSettings
{
    private static readonly string DirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IvaoAuto");
    private static readonly string FilePath = Path.Combine(DirectoryPath, "settings.json");

    public string MsfsProcessName { get; set; } = "FlightSimulator2024.exe";
    public string AltitudeProcessName { get; set; } = "PilotUI.exe";
    public decimal ReconnectDelayMinutes { get; set; } = 0.25m;

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
        }
        catch { }
        return new AppSettings();
    }

    public void Save()
    {
        Directory.CreateDirectory(DirectoryPath);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static string NormalizeProcessName(string value) => Path.GetFileNameWithoutExtension(value.Trim());
}
