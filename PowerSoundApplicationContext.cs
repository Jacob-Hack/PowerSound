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
    private readonly System.Windows.Forms.Timer startupUpdateTimer;
    private SettingsForm? settingsForm;
    private bool checkingForUpdates;
    private PowerLineStatus lastPowerLineStatus;

    public PowerSoundApplicationContext()
    {
        settings = SettingsStore.Load();
        soundService = new SoundService(settings);
        lastPowerLineStatus = SystemInformation.PowerStatus.PowerLineStatus;
        batteryAlertEvaluator = new BatteryAlertEvaluator(settings, lastPowerLineStatus);

        notifyIcon = new NotifyIcon
        {
            Icon = AppIcons.CreateApplicationIcon(),
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

        startupUpdateTimer = new System.Windows.Forms.Timer
        {
            Interval = 8_000
        };
        startupUpdateTimer.Tick += async (_, _) =>
        {
            startupUpdateTimer.Stop();
            if (settings.CheckForUpdatesOnStartup)
            {
                await CheckForUpdatesAsync(showUpToDateMessage: false);
            }
        };
        startupUpdateTimer.Start();
    }

    private ContextMenuStrip BuildMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open PowerSound settings", null, (_, _) => ShowSettings());
        menu.Items.Add("Check for updates", null, async (_, _) => await CheckForUpdatesAsync(showUpToDateMessage: true));
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

    private async Task CheckForUpdatesAsync(bool showUpToDateMessage)
    {
        if (checkingForUpdates)
        {
            return;
        }

        checkingForUpdates = true;
        try
        {
            var update = await UpdateService.CheckForUpdateAsync();
            if (update is null)
            {
                if (showUpToDateMessage)
                {
                    MessageBox.Show(
                        "PowerSound is up to date.",
                        "PowerSound",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                return;
            }

            using var prompt = new UpdatePromptForm(update);
            prompt.ShowDialog();
            if (!prompt.InstallUpdate)
            {
                return;
            }

            var installerPath = await UpdateService.DownloadInstallerAsync(update);
            UpdateService.LaunchInstaller(installerPath);
            ExitThread();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"PowerSound could not check for updates.{Environment.NewLine}{ex.Message}",
                "PowerSound",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            checkingForUpdates = false;
        }
    }

    protected override void ExitThreadCore()
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        batteryMonitorTimer.Stop();
        batteryMonitorTimer.Dispose();
        startupUpdateTimer.Stop();
        startupUpdateTimer.Dispose();
        SettingsStore.Save(settings);
        StartupManager.SetStartWithWindows(settings.StartWithWindows);
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
        settingsForm?.Dispose();
        base.ExitThreadCore();
    }
}
