using System.Text.Json;

namespace PowerSound;

internal static class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string SettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerSound");

    public static string SettingsPath => Path.Combine(SettingsDirectory, "settings.json");

    public static PowerSoundSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<PowerSoundSettings>(json) ?? new PowerSoundSettings();
            }
        }
        catch
        {
            // Broken settings should not stop the tray app from opening.
        }

        return new PowerSoundSettings
        {
            StartWithWindows = StartupManager.IsStartWithWindowsEnabled()
        };
    }

    public static void Save(PowerSoundSettings settings)
    {
        Directory.CreateDirectory(SettingsDirectory);
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(SettingsPath, json);
    }
}
