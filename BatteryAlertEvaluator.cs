using System.Windows.Forms;

namespace PowerSound;

internal sealed class BatteryAlertEvaluator
{
    private static readonly BatteryAlertKind[] ThresholdAlertPriority =
    [
        BatteryAlertKind.Emergency,
        BatteryAlertKind.Critical,
        BatteryAlertKind.Low
    ];

    private readonly PowerSoundSettings settings;
    private readonly HashSet<BatteryAlertKind> triggeredThresholdAlerts = [];
    private bool fullyChargedTriggered;
    private int? lastBatteryPercent;
    private PowerLineStatus lastPowerLineStatus;

    public BatteryAlertEvaluator(PowerSoundSettings settings, PowerLineStatus initialPowerLineStatus)
    {
        this.settings = settings;
        lastPowerLineStatus = initialPowerLineStatus;
    }

    public BatteryAlertEvent? Evaluate(int batteryPercent, PowerLineStatus powerLineStatus)
    {
        batteryPercent = Math.Clamp(batteryPercent, 0, 100);

        ResetStateForCurrentBatteryLevel(batteryPercent, powerLineStatus);

        var alert = powerLineStatus switch
        {
            PowerLineStatus.Online => EvaluateFullyCharged(batteryPercent),
            PowerLineStatus.Offline => EvaluateThresholdAlerts(batteryPercent),
            _ => null
        };

        lastBatteryPercent = batteryPercent;
        lastPowerLineStatus = powerLineStatus;
        return alert;
    }

    private void ResetStateForCurrentBatteryLevel(int batteryPercent, PowerLineStatus powerLineStatus)
    {
        if (powerLineStatus == PowerLineStatus.Online && lastPowerLineStatus != PowerLineStatus.Online)
        {
            triggeredThresholdAlerts.Clear();
        }

        foreach (var kind in ThresholdAlertPriority)
        {
            var alertSettings = settings.GetBatteryAlert(kind);
            if (batteryPercent > alertSettings.ThresholdPercent)
            {
                triggeredThresholdAlerts.Remove(kind);
            }
        }

        if (batteryPercent < 100)
        {
            fullyChargedTriggered = false;
        }
    }

    private BatteryAlertEvent? EvaluateThresholdAlerts(int batteryPercent)
    {
        foreach (var kind in ThresholdAlertPriority)
        {
            var alertSettings = settings.GetBatteryAlert(kind);
            if (!alertSettings.Enabled || triggeredThresholdAlerts.Contains(kind))
            {
                continue;
            }

            if (batteryPercent <= alertSettings.ThresholdPercent && CrossedDownToThreshold(alertSettings.ThresholdPercent))
            {
                MarkCrossedThresholdsTriggered(batteryPercent);
                return new BatteryAlertEvent(kind, batteryPercent);
            }
        }

        return null;
    }

    private bool CrossedDownToThreshold(int thresholdPercent) =>
        lastBatteryPercent is null || lastBatteryPercent > thresholdPercent;

    private void MarkCrossedThresholdsTriggered(int batteryPercent)
    {
        foreach (var kind in ThresholdAlertPriority)
        {
            if (batteryPercent <= settings.GetBatteryAlert(kind).ThresholdPercent)
            {
                triggeredThresholdAlerts.Add(kind);
            }
        }
    }

    private BatteryAlertEvent? EvaluateFullyCharged(int batteryPercent)
    {
        var alertSettings = settings.FullyChargedBatteryAlert;
        if (!alertSettings.Enabled || fullyChargedTriggered || batteryPercent < 100)
        {
            return null;
        }

        fullyChargedTriggered = true;
        return new BatteryAlertEvent(BatteryAlertKind.FullyCharged, batteryPercent);
    }
}
