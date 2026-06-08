namespace PowerSound;

internal sealed class BatteryAlertSettings
{
    public bool Enabled { get; set; }
    public bool PlaySound { get; set; } = true;
    public bool ShowNotification { get; set; } = true;
    public string SoundPath { get; set; } = "";
    public int ThresholdPercent { get; set; }

    public BatteryAlertSettings Copy() => new()
    {
        Enabled = Enabled,
        PlaySound = PlaySound,
        ShowNotification = ShowNotification,
        SoundPath = SoundPath,
        ThresholdPercent = ThresholdPercent
    };

    public void CopyFrom(BatteryAlertSettings source)
    {
        Enabled = source.Enabled;
        PlaySound = source.PlaySound;
        ShowNotification = source.ShowNotification;
        SoundPath = source.SoundPath;
        ThresholdPercent = source.ThresholdPercent;
    }

    public static BatteryAlertSettings CreateDefault(BatteryAlertKind kind) => kind switch
    {
        BatteryAlertKind.Low => new BatteryAlertSettings { Enabled = true, ThresholdPercent = 20 },
        BatteryAlertKind.Critical => new BatteryAlertSettings { Enabled = true, ThresholdPercent = 10 },
        BatteryAlertKind.Emergency => new BatteryAlertSettings { Enabled = true, ThresholdPercent = 5 },
        BatteryAlertKind.FullyCharged => new BatteryAlertSettings { Enabled = false, ThresholdPercent = 100 },
        _ => new BatteryAlertSettings()
    };
}
