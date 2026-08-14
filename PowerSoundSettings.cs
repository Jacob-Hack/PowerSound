namespace PowerSound;

internal sealed class PowerSoundSettings
{
    public string ConnectedSoundPath { get; set; } = "";
    public string DisconnectedSoundPath { get; set; } = "";
    public bool StartWithWindows { get; set; }
    public bool CheckForUpdatesOnStartup { get; set; } = true;
    public BatteryAlertSettings LowBatteryAlert { get; set; } = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Low);
    public BatteryAlertSettings CriticalBatteryAlert { get; set; } = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Critical);
    public BatteryAlertSettings EmergencyBatteryAlert { get; set; } = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Emergency);
    public BatteryAlertSettings FullyChargedBatteryAlert { get; set; } = BatteryAlertSettings.CreateDefault(BatteryAlertKind.FullyCharged);

    public bool UseDefaultConnectedSound => string.IsNullOrWhiteSpace(ConnectedSoundPath);
    public bool UseDefaultDisconnectedSound => string.IsNullOrWhiteSpace(DisconnectedSoundPath);

    public static PowerSoundSettings CreateDefault() => new()
    {
        CheckForUpdatesOnStartup = true,
        LowBatteryAlert = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Low),
        CriticalBatteryAlert = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Critical),
        EmergencyBatteryAlert = BatteryAlertSettings.CreateDefault(BatteryAlertKind.Emergency),
        FullyChargedBatteryAlert = BatteryAlertSettings.CreateDefault(BatteryAlertKind.FullyCharged)
    };

    public PowerSoundSettings Copy() => new()
    {
        ConnectedSoundPath = ConnectedSoundPath,
        DisconnectedSoundPath = DisconnectedSoundPath,
        StartWithWindows = StartWithWindows,
        CheckForUpdatesOnStartup = CheckForUpdatesOnStartup,
        LowBatteryAlert = LowBatteryAlert.Copy(),
        CriticalBatteryAlert = CriticalBatteryAlert.Copy(),
        EmergencyBatteryAlert = EmergencyBatteryAlert.Copy(),
        FullyChargedBatteryAlert = FullyChargedBatteryAlert.Copy()
    };

    public void CopyFrom(PowerSoundSettings source)
    {
        ConnectedSoundPath = source.ConnectedSoundPath;
        DisconnectedSoundPath = source.DisconnectedSoundPath;
        StartWithWindows = source.StartWithWindows;
        CheckForUpdatesOnStartup = source.CheckForUpdatesOnStartup;
        LowBatteryAlert.CopyFrom(source.LowBatteryAlert);
        CriticalBatteryAlert.CopyFrom(source.CriticalBatteryAlert);
        EmergencyBatteryAlert.CopyFrom(source.EmergencyBatteryAlert);
        FullyChargedBatteryAlert.CopyFrom(source.FullyChargedBatteryAlert);
    }

    public BatteryAlertSettings GetBatteryAlert(BatteryAlertKind kind) => kind switch
    {
        BatteryAlertKind.Low => LowBatteryAlert,
        BatteryAlertKind.Critical => CriticalBatteryAlert,
        BatteryAlertKind.Emergency => EmergencyBatteryAlert,
        BatteryAlertKind.FullyCharged => FullyChargedBatteryAlert,
        _ => LowBatteryAlert
    };

    public void EnsureDefaults()
    {
        LowBatteryAlert ??= BatteryAlertSettings.CreateDefault(BatteryAlertKind.Low);
        CriticalBatteryAlert ??= BatteryAlertSettings.CreateDefault(BatteryAlertKind.Critical);
        EmergencyBatteryAlert ??= BatteryAlertSettings.CreateDefault(BatteryAlertKind.Emergency);
        FullyChargedBatteryAlert ??= BatteryAlertSettings.CreateDefault(BatteryAlertKind.FullyCharged);

        ClampThreshold(LowBatteryAlert, 20);
        ClampThreshold(CriticalBatteryAlert, 10);
        ClampThreshold(EmergencyBatteryAlert, 5);
        FullyChargedBatteryAlert.ThresholdPercent = 100;
    }

    private static void ClampThreshold(BatteryAlertSettings alertSettings, int defaultThreshold)
    {
        if (alertSettings.ThresholdPercent < 1 || alertSettings.ThresholdPercent > 100)
        {
            alertSettings.ThresholdPercent = defaultThreshold;
        }
    }
}
