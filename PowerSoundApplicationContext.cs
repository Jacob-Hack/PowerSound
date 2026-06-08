using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace PowerSound;

internal sealed class PowerSoundApplicationContext : ApplicationContext
{
    private readonly NotifyIcon notifyIcon;
    private readonly PowerSoundSettings settings;
    private readonly SoundService soundService;
    private readonly BatteryAlertEvaluator batteryAlertEvaluator;
    private readonly System.Windows.Forms.Timer batteryMonitorTimer;
    private SettingsForm? settingsForm;
    private PowerLineStatus lastPowerLineStatus;

    public PowerSoundApplicationContext()
    {
        settings = SettingsStore.Load();
        soundService = new SoundService(settings);
        lastPowerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
        batteryAlertEvaluator = new BatteryAlertEvaluator(settings, lastPowerLineStatus);

        notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Shield,
            Text = "PowerSound",
            Visible = true,
            ContextMenuStrip = BuildMenu()
        };
        notifyIcon.DoubleClick += (_, _) => ShowSettings();

        SystemEvents.PowerModeChanged += OnPowerModeChanged;

        batteryMonitorTimer = new System.Windows.Forms.Timer
        {
            Interval = 60_000
        };
        batteryMonitorTimer.Tick += (_, _) => EvaluateBatteryAlerts();
        batteryMonitorTimer.Start();
        EvaluateBatteryAlerts();
    }

    private ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open PowerSound settings", null, (_, _) => ShowSettings());
        menu.Items.Add("Test AC connected sound", null, (_, _) => soundService.PlayConnectedSound());
        menu.Items.Add("Test AC disconnected sound", null, (_, _) => soundService.PlayDisconnectedSound());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitThread());
        return menu;
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode == PowerModes.Resume)
        {
            EvaluateBatteryAlerts();
            return;
        }

        if (e.Mode != PowerModes.StatusChange)
        {
            return;
        }

        var currentPowerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
        if (currentPowerLineStatus == lastPowerLineStatus)
        {
            return;
        }

        lastPowerLineStatus = currentPowerLineStatus;

        switch (currentPowerLineStatus)
        {
            case PowerLineStatus.Online:
                soundService.PlayConnectedSound();
                break;
            case PowerLineStatus.Offline:
                soundService.PlayDisconnectedSound();
                break;
        }

        EvaluateBatteryAlerts();
    }

    private void EvaluateBatteryAlerts()
    {
        var powerStatus = SystemInformation.PowerStatus;
        var batteryPercent = (int)Math.Round(powerStatus.BatteryLifePercent * 100);
        var alertEvent = batteryAlertEvaluator.Evaluate(batteryPercent, powerStatus.PowerLineStatus);
        if (alertEvent is null)
        {
            return;
        }

        var alertSettings = settings.GetBatteryAlert(alertEvent.Kind);
        if (alertSettings.PlaySound)
        {
            soundService.PlayBatteryAlertSound(alertEvent.Kind);
        }

        if (alertSettings.ShowNotification)
        {
            notifyIcon.ShowBalloonTip(
                10_000,
                alertEvent.NotificationTitle,
                alertEvent.NotificationText,
                alertEvent.NotificationIcon);
        }
    }

    private void ShowSettings()
    {
        if (settingsForm is { IsDisposed: false })
        {
            settingsForm.Activate();
            return;
        }

        settingsForm = new SettingsForm(settings);
        settingsForm.FormClosed += (_, _) => settingsForm = null;
        settingsForm.Show();
    }

    protected override void ExitThreadCore()
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        batteryMonitorTimer.Stop();
        batteryMonitorTimer.Dispose();
        SettingsStore.Save(settings);
        StartupManager.SetStartWithWindows(settings.StartWithWindows);
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
        settingsForm?.Dispose();
        base.ExitThreadCore();
    }
}
