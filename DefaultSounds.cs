namespace PowerSound;

internal static class DefaultSounds
{
    public static byte[] Connected => CreateTone(660, 120, 0.35);
    public static byte[] Disconnected => CreateTone(330, 180, 0.35);

    private static byte[] CreateTone(double frequency, int durationMilliseconds, double volume)
    {
        const int sampleRate = 44100;
        const short bitsPerSample = 16;
        const short channels = 1;
        var sampleCount = sampleRate * durationMilliseconds / 1000;
        var dataLength = sampleCount * channels * bitsPerSample / 8;

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write("RIFF"u8.ToArray());
        writer.Write(36 + dataLength);
        writer.Write("WAVE"u8.ToArray());
        writer.Write("fmt "u8.ToArray());
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * bitsPerSample / 8);
        writer.Write((short)(channels * bitsPerSample / 8));
        writer.Write(bitsPerSample);
        writer.Write("data"u8.ToArray());
        writer.Write(dataLength);

        for (var i = 0; i < sampleCount; i++)
        {
            var fadeIn = Math.Min(1.0, i / (sampleRate * 0.01));
            var fadeOut = Math.Min(1.0, (sampleCount - i) / (sampleRate * 0.03));
            var envelope = Math.Min(fadeIn, fadeOut);
            var sample = Math.Sin(2 * Math.PI * frequency * i / sampleRate);
            writer.Write((short)(sample * short.MaxValue * volume * envelope));
        }

        return stream.ToArray();
    }
}
