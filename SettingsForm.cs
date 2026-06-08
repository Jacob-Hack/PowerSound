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

    public SettingsForm(PowerSoundSettings settings)
    {
        this.settings = settings;
        editSettings = settings.Copy();
        previewSoundService = new SoundService(editSettings);

        Text = "PowerSound Settings";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(560, 360);
        Size = new Size(640, 390);
        Font = SystemFonts.MessageBoxFont;
        AutoScaleMode = AutoScaleMode.Font;
        AccessibleName = "PowerSound settings";
        AccessibleDescription = "Choose power sounds, test them, and set whether PowerSound starts with Windows.";

        BuildUi();
        LoadValues();
    }

    private void BuildUi()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 4,
            RowCount = 6
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

        var saveButton = new Button
        {
            Text = "&Save",
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            DialogResult = DialogResult.OK,
            AccessibleName = "Save settings"
        };
        saveButton.Click += (_, _) => SaveAndClose();

        var cancelButton = new Button
        {
            Text = "Cancel",
            AutoSize = true,
            Anchor = AnchorStyles.Left,
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
        layout.Controls.Add(buttonPanel, 1, 5);
        layout.SetColumnSpan(buttonPanel, 3);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
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
    }

    private void BrowseConnectedSound(object? sender, EventArgs e)
    {
        var path = ChooseSoundFile();
        if (path is not null)
        {
            connectedPathTextBox.Text = path;
        }
    }

    private void BrowseDisconnectedSound(object? sender, EventArgs e)
    {
        var path = ChooseSoundFile();
        if (path is not null)
        {
            disconnectedPathTextBox.Text = path;
        }
    }

    private static string? ChooseSoundFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Choose a sound file",
            Filter = "Wave sound files (*.wav)|*.wav|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
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

    private void SaveAndClose()
    {
        ApplyValues(editSettings);
        settings.CopyFrom(editSettings);
        SettingsStore.Save(settings);
        StartupManager.SetStartWithWindows(settings.StartWithWindows);
        Close();
    }

    private void ApplyValues(PowerSoundSettings target)
    {
        target.ConnectedSoundPath = connectedPathTextBox.Text.Trim();
        target.DisconnectedSoundPath = disconnectedPathTextBox.Text.Trim();
        target.StartWithWindows = startWithWindowsCheckBox.Checked;
    }
}
