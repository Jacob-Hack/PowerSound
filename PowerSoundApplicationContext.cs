using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

namespace PowerSound;

internal sealed class PowerSoundApplicationContext : ApplicationContext
{
    private readonly NotifyIcon notifyIcon;
    private readonly PowerSoundSettings settings;
    private readonly SoundService soundService;
    private SettingsForm? settingsForm;

    public PowerSoundApplicationContext()
    {
        settings = SettingsStore.Load();
        soundService = new SoundService(settings);

        notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Shield,
            Text = "PowerSound",
            Visible = true,
            ContextMenuStrip = BuildMenu()
        };
        notifyIcon.DoubleClick += (_, _) => ShowSettings();

        SystemEvents.PowerModeChanged += OnPowerModeChanged;
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
        if (e.Mode != PowerModes.StatusChange)
        {
            return;
        }

        switch (SystemInformation.PowerStatus.PowerLineStatus)
        {
            case PowerLineStatus.Online:
                soundService.PlayConnectedSound();
                break;
            case PowerLineStatus.Offline:
                soundService.PlayDisconnectedSound();
                break;
        }
    }

    private void ShowSettings()
    {
        if (settingsForm is { IsDisposed: false })
        {
            settingsForm.Activate();
            return;
        }

        settingsForm = new SettingsForm(settings, soundService);
        settingsForm.FormClosed += (_, _) =>
        {
            SettingsStore.Save(settings);
            StartupManager.SetStartWithWindows(settings.StartWithWindows);
        };
        settingsForm.Show();
    }

    protected override void ExitThreadCore()
    {
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
        SettingsStore.Save(settings);
        StartupManager.SetStartWithWindows(settings.StartWithWindows);
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
        settingsForm?.Dispose();
        base.ExitThreadCore();
    }
}
