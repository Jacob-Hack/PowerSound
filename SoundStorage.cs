namespace PowerSound;

internal static class SoundStorage
{
    public static string SoundsDirectory =>
        Path.Combine(SettingsStore.SettingsDirectory, "Sounds");

    public static string CopySoundToAppData(string sourcePath)
    {
        Directory.CreateDirectory(SoundsDirectory);

        var extension = Path.GetExtension(sourcePath);
        if (!string.Equals(extension, ".wav", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("PowerSound supports custom WAV sound files.");
        }

        var baseName = Path.GetFileNameWithoutExtension(sourcePath);
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            baseName = baseName.Replace(invalidChar, '_');
        }

        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = "sound";
        }

        var destinationPath = Path.Combine(SoundsDirectory, baseName + extension);
        var suffix = 1;
        while (File.Exists(destinationPath))
        {
            destinationPath = Path.Combine(SoundsDirectory, $"{baseName}-{suffix}{extension}");
            suffix++;
        }

        File.Copy(sourcePath, destinationPath);
        return destinationPath;
    }
}
