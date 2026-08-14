using System.Drawing;
using System.Windows.Forms;

namespace PowerSound;

internal sealed class SettingsForm : Form
{
    private readonly PowerSoundSettings settings;
    private readonly PowerSoundSettings editSettings;
    private readonly SoundService previewSoundService;
    private readonly TextBox connectedPathTextBox = new();
    private readonly TextBox disconnectedPathTextBox = new();
    private readonly CheckBox startWithWindowsCheckBox = new();
    private readonly Dictionary<BatteryAlertKind, AlertControls> alertControls = [];

    public SettingsForm(PowerSoundSettings settings)
    {
        this.settings = settings;
        editSettings = settings.Copy();
        previewSoundService = new SoundService(editSettings);

        Text = "PowerSound Settings";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(760, 560);
        Size = new Size(840, 640);
        Font = SystemFonts.MessageBoxFont;
        AutoScaleMode = AutoScaleMode.Font;
        AccessibleName = "PowerSound settings";
        AccessibleDescription = "Choose power sounds, battery alerts, notifications, and startup behavior.";
        Icon = AppIcons.CreateApplicationIcon();

        BuildUi();
        LoadValues();
    }

    private void BuildUi()
    {
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 2
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            AccessibleName = "Settings sections"
        };
        tabs.TabPages.Add(BuildGeneralTab());
        tabs.TabPages.Add(BuildBatteryAlertsTab());
        tabs.TabPages.Add(BuildAboutTab());
        mainLayout.Controls.Add(tabs, 0, 0);

        mainLayout.Controls.Add(BuildButtonPanel(), 0, 1);
        Controls.Add(mainLayout);
    }

    private TabPage BuildGeneralTab()
    {
        var tab = new TabPage("General")
        {
            AccessibleName = "General settings"
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 4,
            RowCount = 4
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

        AddSoundRow(layout, 0, "&AC connected sound", connectedPathTextBox, BrowseConnectedSound, TestConnectedSound);
        AddSoundRow(layout, 1, "AC &disconnected sound", disconnectedPathTextBox, BrowseDisconnectedSound, TestDisconnectedSound);

        startWithWindowsCheckBox.Text = "Start PowerSound with &Windows";
        startWithWindowsCheckBox.AutoSize = true;
        startWithWindowsCheckBox.AccessibleName = "Start PowerSound with Windows";
        startWithWindowsCheckBox.AccessibleDescription = "When checked, PowerSound opens automatically after you sign in.";
        layout.Controls.Add(startWithWindowsCheckBox, 1, 3);
        layout.SetColumnSpan(startWithWindowsCheckBox, 3);

        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildAboutTab()
    {
        var tab = new TabPage("About")
        {
            AccessibleName = "About PowerSound"
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 7
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var nameLabel = new Label
        {
            Text = "PowerSound",
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            AccessibleName = "PowerSound"
        };
        layout.Controls.Add(nameLabel, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = $"Version {AppVersion.Display}",
            AutoSize = true,
            AccessibleName = $"PowerSound version {AppVersion.Display}"
        }, 0, 1);

        layout.Controls.Add(new Label
        {
            Text = "Created by Jacob Hack.",
            AutoSize = true
        }, 0, 2);

        layout.Controls.Add(new Label
        {
            Text = "Bundled default sounds generated using ByteDance Seed Audio 1.0 via fal.ai.",
            AutoSize = true
        }, 0, 3);

        var checkUpdatesButton = new Button
        {
            Text = "Check for &updates",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = "Check for updates",
            AccessibleDescription = "Checks GitHub Releases for a newer PowerSound installer."
        };
        checkUpdatesButton.Click += async (_, _) => await CheckForUpdatesAsync(checkUpdatesButton);
        layout.Controls.Add(checkUpdatesButton, 0, 4);

        var releasesButton = new Button
        {
            Text = "Open &releases page",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = "Open releases page"
        };
        releasesButton.Click += (_, _) => UpdateService.OpenReleasesPage();
        layout.Controls.Add(releasesButton, 0, 5);

        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage BuildBatteryAlertsTab()
    {
        var tab = new TabPage("Battery Alerts")
        {
            AccessibleName = "Battery Alerts settings"
        };

        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(8)
        };

        layout.Controls.Add(BuildAlertGroup(BatteryAlertKind.Low, "Battery Low", true), 0, 0);
        layout.Controls.Add(BuildAlertGroup(BatteryAlertKind.Critical, "Battery Critical", true), 0, 1);
        layout.Controls.Add(BuildAlertGroup(BatteryAlertKind.Emergency, "Battery Emergency", true), 0, 2);
        layout.Controls.Add(BuildAlertGroup(BatteryAlertKind.FullyCharged, "Battery Fully Charged", false), 0, 3);

        scrollPanel.Controls.Add(layout);
        tab.Controls.Add(scrollPanel);
        return tab;
    }

    private GroupBox BuildAlertGroup(BatteryAlertKind kind, string title, bool hasThreshold)
    {
        var controls = new AlertControls();
        alertControls[kind] = controls;

        var group = new GroupBox
        {
            Text = title,
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(12),
            AccessibleName = title
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 4,
            RowCount = 3
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

        controls.EnabledCheckBox.Text = "Enable alert";
        controls.EnabledCheckBox.AutoSize = true;
        controls.EnabledCheckBox.AccessibleName = $"Enable {title} alert";
        layout.Controls.Add(controls.EnabledCheckBox, 0, 0);

        controls.PlaySoundCheckBox.Text = "Play sound";
        controls.PlaySoundCheckBox.AutoSize = true;
        controls.PlaySoundCheckBox.AccessibleName = $"Play sound for {title}";
        layout.Controls.Add(controls.PlaySoundCheckBox, 1, 0);

        controls.ShowNotificationCheckBox.Text = "Show notification";
        controls.ShowNotificationCheckBox.AutoSize = true;
        controls.ShowNotificationCheckBox.AccessibleName = $"Show notification for {title}";
        layout.Controls.Add(controls.ShowNotificationCheckBox, 2, 0);
        layout.SetColumnSpan(controls.ShowNotificationCheckBox, 2);

        if (hasThreshold)
        {
            var thresholdLabel = new Label
            {
                Text = "Threshold &percent",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(thresholdLabel, 0, 1);

            controls.ThresholdNumericUpDown.Minimum = 1;
            controls.ThresholdNumericUpDown.Maximum = 100;
            controls.ThresholdNumericUpDown.Width = 80;
            controls.ThresholdNumericUpDown.AccessibleName = $"{title} threshold percent";
            controls.ThresholdNumericUpDown.AccessibleDescription = "Battery percentage at or below which this alert triggers.";
            layout.Controls.Add(controls.ThresholdNumericUpDown, 1, 1);
            thresholdLabel.Click += (_, _) => controls.ThresholdNumericUpDown.Focus();
        }
        else
        {
            var fullChargeLabel = new Label
            {
                Text = "Triggers at 100% while connected to AC power.",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(fullChargeLabel, 0, 1);
            layout.SetColumnSpan(fullChargeLabel, 4);
        }

        var soundLabel = new Label
        {
            Text = "Sound file",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(soundLabel, 0, 2);

        controls.SoundPathTextBox.Dock = DockStyle.Fill;
        controls.SoundPathTextBox.AccessibleName = $"{title} sound file";
        controls.SoundPathTextBox.AccessibleDescription = "Leave blank to use the built-in default sound. Choose a WAV file for a custom sound.";
        layout.Controls.Add(controls.SoundPathTextBox, 1, 2);
        soundLabel.Click += (_, _) => controls.SoundPathTextBox.Focus();

        var browseButton = new Button
        {
            Text = "&Browse...",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = $"Browse for {title} sound"
        };
        browseButton.Click += (_, _) => BrowseBatteryAlertSound(kind);
        layout.Controls.Add(browseButton, 2, 2);

        var testButton = new Button
        {
            Text = "&Test",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = $"Test {title} sound"
        };
        testButton.Click += (_, _) => TestBatteryAlertSound(kind);
        layout.Controls.Add(testButton, 3, 2);

        group.Controls.Add(layout);
        return group;
    }

    private FlowLayoutPanel BuildButtonPanel()
    {
        var saveButton = new Button
        {
            Text = "&Save",
            AutoSize = true,
            DialogResult = DialogResult.OK,
            AccessibleName = "Save settings"
        };
        saveButton.Click += (_, _) => SaveAndClose();

        var cancelButton = new Button
        {
            Text = "Cancel",
            AutoSize = true,
            DialogResult = DialogResult.Cancel,
            AccessibleName = "Cancel"
        };
        cancelButton.Click += (_, _) => Close();

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill,
            AutoSize = true
        };
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
        return buttonPanel;
    }

    private static void AddSoundRow(
        TableLayoutPanel layout,
        int row,
        string labelText,
        TextBox textBox,
        EventHandler browseHandler,
        EventHandler testHandler)
    {
        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            TextAlign = ContentAlignment.MiddleLeft
        };
        layout.Controls.Add(label, 0, row);

        textBox.Dock = DockStyle.Fill;
        textBox.AccessibleName = labelText.Replace("&", "");
        textBox.AccessibleDescription = "Leave blank to use the built-in default sound. Choose a WAV file for a custom sound.";
        layout.Controls.Add(textBox, 1, row);
        label.Click += (_, _) => textBox.Focus();

        var browseButton = new Button
        {
            Text = "&Browse...",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = $"Browse for {textBox.AccessibleName}"
        };
        browseButton.Click += browseHandler;
        layout.Controls.Add(browseButton, 2, row);

        var testButton = new Button
        {
            Text = "&Test",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            AccessibleName = $"Test {textBox.AccessibleName}"
        };
        testButton.Click += testHandler;
        layout.Controls.Add(testButton, 3, row);
    }

    private void LoadValues()
    {
        connectedPathTextBox.Text = editSettings.ConnectedSoundPath;
        disconnectedPathTextBox.Text = editSettings.DisconnectedSoundPath;
        startWithWindowsCheckBox.Checked = editSettings.StartWithWindows;

        LoadAlertValues(BatteryAlertKind.Low);
        LoadAlertValues(BatteryAlertKind.Critical);
        LoadAlertValues(BatteryAlertKind.Emergency);
        LoadAlertValues(BatteryAlertKind.FullyCharged);
    }

    private void LoadAlertValues(BatteryAlertKind kind)
    {
        var settings = editSettings.GetBatteryAlert(kind);
        var controls = alertControls[kind];

        controls.EnabledCheckBox.Checked = settings.Enabled;
        controls.PlaySoundCheckBox.Checked = settings.PlaySound;
        controls.ShowNotificationCheckBox.Checked = settings.ShowNotification;
        controls.SoundPathTextBox.Text = settings.SoundPath;
        if (kind != BatteryAlertKind.FullyCharged)
        {
            controls.ThresholdNumericUpDown.Value = Math.Clamp(settings.ThresholdPercent, 1, 100);
        }
    }

    private void BrowseConnectedSound(object? sender, EventArgs e)
    {
        var path = ChooseAndCopySoundFile();
        if (path is not null)
        {
            connectedPathTextBox.Text = path;
        }
    }

    private void BrowseDisconnectedSound(object? sender, EventArgs e)
    {
        var path = ChooseAndCopySoundFile();
        if (path is not null)
        {
            disconnectedPathTextBox.Text = path;
        }
    }

    private void BrowseBatteryAlertSound(BatteryAlertKind kind)
    {
        var path = ChooseAndCopySoundFile();
        if (path is not null)
        {
            alertControls[kind].SoundPathTextBox.Text = path;
        }
    }

    private static string? ChooseAndCopySoundFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a sound file",
            Filter = "Wave sound files (*.wav)|*.wav",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return null;
        }

        try
        {
            return SoundStorage.CopySoundToAppData(dialog.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"PowerSound could not copy the sound file.{Environment.NewLine}{ex.Message}",
                "PowerSound",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return null;
        }
    }

    private void TestConnectedSound(object? sender, EventArgs e)
    {
        ApplyValues(editSettings);
        previewSoundService.PlayConnectedSound();
    }

    private void TestDisconnectedSound(object? sender, EventArgs e)
    {
        ApplyValues(editSettings);
        previewSoundService.PlayDisconnectedSound();
    }

    private void TestBatteryAlertSound(BatteryAlertKind kind)
    {
        ApplyValues(editSettings);
        previewSoundService.PlayBatteryAlertSound(kind);
    }

    private void SaveAndClose()
    {
        ApplyValues(editSettings);
        settings.CopyFrom(editSettings);
        SettingsStore.Save(settings);
        StartupManager.SetStartWithWindows(settings.StartWithWindows);
        Close();
    }

    private async Task CheckForUpdatesAsync(Control owner)
    {
        owner.Enabled = false;
        try
        {
            var update = await UpdateService.CheckForUpdateAsync();
            if (update is null)
            {
                MessageBox.Show(
                    this,
                    "PowerSound is up to date.",
                    "PowerSound",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                this,
                $"PowerSound {update.VersionText} is available.{Environment.NewLine}{Environment.NewLine}Download and run the installer now?",
                "PowerSound update available",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (result != DialogResult.Yes)
            {
                return;
            }

            var installerPath = await UpdateService.DownloadInstallerAsync(update);
            UpdateService.LaunchInstaller(installerPath);
            Application.Exit();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"PowerSound could not check for updates.{Environment.NewLine}{ex.Message}",
                "PowerSound",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            if (!owner.IsDisposed)
            {
                owner.Enabled = true;
            }
        }
    }

    private void ApplyValues(PowerSoundSettings target)
    {
        target.ConnectedSoundPath = connectedPathTextBox.Text.Trim();
        target.DisconnectedSoundPath = disconnectedPathTextBox.Text.Trim();
        target.StartWithWindows = startWithWindowsCheckBox.Checked;

        ApplyAlertValues(target, BatteryAlertKind.Low);
        ApplyAlertValues(target, BatteryAlertKind.Critical);
        ApplyAlertValues(target, BatteryAlertKind.Emergency);
        ApplyAlertValues(target, BatteryAlertKind.FullyCharged);
        target.EnsureDefaults();
    }

    private void ApplyAlertValues(PowerSoundSettings target, BatteryAlertKind kind)
    {
        var alertSettings = target.GetBatteryAlert(kind);
        var controls = alertControls[kind];

        alertSettings.Enabled = controls.EnabledCheckBox.Checked;
        alertSettings.PlaySound = controls.PlaySoundCheckBox.Checked;
        alertSettings.ShowNotification = controls.ShowNotificationCheckBox.Checked;
        alertSettings.SoundPath = controls.SoundPathTextBox.Text.Trim();
        alertSettings.ThresholdPercent = kind == BatteryAlertKind.FullyCharged
            ? 100
            : (int)controls.ThresholdNumericUpDown.Value;
    }

    private sealed class AlertControls
    {
        public CheckBox EnabledCheckBox { get; } = new();
        public CheckBox PlaySoundCheckBox { get; } = new();
        public CheckBox ShowNotificationCheckBox { get; } = new();
        public TextBox SoundPathTextBox { get; } = new();
        public NumericUpDown ThresholdNumericUpDown { get; } = new();
    }
}
