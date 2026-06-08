namespace PowerSound;

internal static class DefaultSounds
{
    public static byte[] Connected => LoadEmbeddedSound("ac_connected.wav");
    public static byte[] Disconnected => LoadEmbeddedSound("ac_disconnected.wav");
    public static byte[] BatteryLow => LoadEmbeddedSound("battery_low.wav");
    public static byte[] BatteryCritical => LoadEmbeddedSound("battery_critical.wav");
    public static byte[] BatteryEmergency => LoadEmbeddedSound("battery_emergency.wav");
    public static byte[] BatteryFullyCharged => LoadEmbeddedSound("battery_fully_charged.wav");

    private static byte[] LoadEmbeddedSound(string fileName)
    {
        var resourceName = $"{typeof(DefaultSounds).Namespace}.Assets.Sounds.{fileName}";
        using var stream = typeof(DefaultSounds).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded sound resource was not found: {resourceName}");
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
