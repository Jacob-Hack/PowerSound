using System.Windows.Forms;

namespace PowerSound;

internal sealed class BatteryAlertEvent
{
    public BatteryAlertEvent(BatteryAlertKind kind, int batteryPercent)
    {
        Kind = kind;
        BatteryPercent = batteryPercent;
    }

    public BatteryAlertKind Kind { get; }
    public int BatteryPercent { get; }

    public string NotificationTitle => Kind switch
    {
        BatteryAlertKind.Low => "Battery low",
        BatteryAlertKind.Critical => "Battery critical",
        BatteryAlertKind.Emergency => "Battery emergency",
        BatteryAlertKind.FullyCharged => "Battery fully charged",
        _ => "PowerSound"
    };

    public string NotificationText => Kind switch
    {
        BatteryAlertKind.Low => $"Battery low: {BatteryPercent}% remaining.",
        BatteryAlertKind.Critical => $"Battery critical: {BatteryPercent}% remaining.",
        BatteryAlertKind.Emergency => $"Battery emergency: {BatteryPercent}% remaining. Connect power immediately.",
        BatteryAlertKind.FullyCharged => "Battery fully charged.",
        _ => ""
    };

    public ToolTipIcon NotificationIcon => Kind switch
    {
        BatteryAlertKind.Emergency => ToolTipIcon.Error,
        BatteryAlertKind.Critical => ToolTipIcon.Warning,
        BatteryAlertKind.Low => ToolTipIcon.Warning,
        BatteryAlertKind.FullyCharged => ToolTipIcon.Info,
        _ => ToolTipIcon.None
    };
}
