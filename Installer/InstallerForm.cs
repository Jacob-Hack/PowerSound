using System.Drawing;
using System.Windows.Forms;

namespace PowerSound.Installer;

internal sealed class InstallerForm : Form
{
    private readonly CheckBox launchCheckBox = new();
    private readonly Button installButton = new();
    private readonly Button cancelButton = new();
    private readonly Label statusLabel = new();

    public InstallerForm()
    {
        Text = "PowerSound Setup";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(520, 260);
        Size = new Size(560, 280);
        Font = SystemFonts.MessageBoxFont;
        AutoScaleMode = AutoScaleMode.Font;
        AccessibleName = "PowerSound setup";
        AccessibleDescription = "Installs PowerSound for the current Windows user.";

        BuildUi();
    }

    private void BuildUi()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            RowCount = 5,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var titleLabel = new Label
        {
            Text = "Install PowerSound",
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true,
            AccessibleName = "Install PowerSound"
        };
        layout.Controls.Add(titleLabel, 0, 0);

        var descriptionLabel = new Label
        {
            Text = $"PowerSound will be installed for your Windows account in:{Environment.NewLine}{Program.InstallDirectory}",
            AutoSize = true,
            MaximumSize = new Size(500, 0)
        };
        layout.Controls.Add(descriptionLabel, 0, 1);

        statusLabel.AutoSize = true;
        statusLabel.AccessibleName = "Setup status";
        layout.Controls.Add(statusLabel, 0, 2);

        launchCheckBox.Text = "Launch PowerSound after installation";
        launchCheckBox.Checked = true;
        launchCheckBox.AutoSize = true;
        launchCheckBox.AccessibleName = "Launch PowerSound after installation";
        layout.Controls.Add(launchCheckBox, 0, 3);

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };

        installButton.Text = "&Install";
        installButton.AutoSize = true;
        installButton.AccessibleName = "Install PowerSound";
        installButton.Click += InstallButton_Click;

        cancelButton.Text = "Cancel";
        cancelButton.AutoSize = true;
        cancelButton.AccessibleName = "Cancel";
        cancelButton.Click += (_, _) => Close();

        buttons.Controls.Add(installButton);
        buttons.Controls.Add(cancelButton);
        layout.Controls.Add(buttons, 0, 4);

        AcceptButton = installButton;
        CancelButton = cancelButton;
        Controls.Add(layout);
    }

    private void InstallButton_Click(object? sender, EventArgs e)
    {
        installButton.Enabled = false;
        cancelButton.Enabled = false;
        statusLabel.Text = "Installing...";

        try
        {
            Program.Install(launchCheckBox.Checked);
            statusLabel.Text = "PowerSound was installed.";
            MessageBox.Show("PowerSound was installed successfully.", "PowerSound Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Installation failed.";
            MessageBox.Show(ex.Message, "PowerSound Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            installButton.Enabled = true;
            cancelButton.Enabled = true;
        }
    }
}
