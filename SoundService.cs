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
        PlayAsync(settings.ConnectedSoundPath, DefaultSounds.Connected);

    public void PlayDisconnectedSound() =>
        PlayAsync(settings.DisconnectedSoundPath, DefaultSounds.Disconnected);

    public void PlayBatteryAlertSound(BatteryAlertKind kind)
    {
        var alertSettings = settings.GetBatteryAlert(kind);
        PlayAsync(alertSettings.SoundPath, GetDefaultSound(kind));
    }

    private static void PlayAsync(string customPath, byte[] defaultSound)
    {
        _ = Task.Run(() => PlaySync(customPath, defaultSound));
    }

    private static void PlaySync(string customPath, byte[] defaultSound)
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

    private static byte[] GetDefaultSound(BatteryAlertKind kind) => kind switch
    {
        BatteryAlertKind.Low => DefaultSounds.BatteryLow,
        BatteryAlertKind.Critical => DefaultSounds.BatteryCritical,
        BatteryAlertKind.Emergency => DefaultSounds.BatteryEmergency,
        BatteryAlertKind.FullyCharged => DefaultSounds.BatteryFullyCharged,
        _ => DefaultSounds.BatteryLow
    };
}
