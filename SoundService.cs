using System.Media;

namespace PowerSound;

internal sealed class SoundService
{
    private readonly PowerSoundSettings settings;

    public SoundService(PowerSoundSettings settings)
    {
        this.settings = settings;
    }

    public void PlayConnectedSound() =>
        Play(settings.ConnectedSoundPath, DefaultSounds.Connected);

    public void PlayDisconnectedSound() =>
        Play(settings.DisconnectedSoundPath, DefaultSounds.Disconnected);

    private static void Play(string customPath, byte[] defaultSound)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(customPath) && File.Exists(customPath))
            {
                using var player = new SoundPlayer(customPath);
                player.PlaySync();
                return;
            }

            using var stream = new MemoryStream(defaultSound);
            using var defaultPlayer = new SoundPlayer(stream);
            defaultPlayer.PlaySync();
        }
        catch
        {
            SystemSounds.Beep.Play();
        }
    }
}
