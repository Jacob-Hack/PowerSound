using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PowerSound;

internal sealed class UpdatePromptForm : Form
{
    public bool InstallUpdate { get; private set; }

    public UpdatePromptForm(UpdateInfo updateInfo)
    {
        Text = "PowerSound update available";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(520, 420);
        Size = new Size(640, 520);
        Font = SystemFonts.MessageBoxFont;
        Icon = AppIcons.CreateApplicationIcon();
        AccessibleName = "PowerSound update available";
        AccessibleDescription = "Review the available update and choose whether to install it.";

        BuildUi(updateInfo);
    }

    private void BuildUi(UpdateInfo updateInfo)
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        layout.Controls.Add(new Label
        {
            Text = $"PowerSound {updateInfo.VersionText} is available.",
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            AccessibleName = $"PowerSound {updateInfo.VersionText} is available"
        }, 0, 0);

        var notesTextBox = new TextBox
        {
            Text = string.IsNullOrWhiteSpace(updateInfo.ReleaseNotes)
                ? "No release notes were provided for this update."
                : FormatReleaseNotes(updateInfo.ReleaseNotes),
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            AccessibleName = "Update release notes",
            AccessibleDescription = "Release notes for the available PowerSound update."
        };
        layout.Controls.Add(notesTextBox, 0, 1);

        layout.Controls.Add(new Label
        {
            Text = "Choose Install update to download and run the latest installer.",
            AutoSize = true
        }, 0, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill,
            AutoSize = true
        };

        var installButton = new Button
        {
            Text = "&Install update",
            AutoSize = true,
            AccessibleName = "Install update"
        };
        installButton.Click += (_, _) =>
        {
            InstallUpdate = true;
            DialogResult = DialogResult.OK;
            Close();
        };

        var laterButton = new Button
        {
            Text = "&Later",
            AutoSize = true,
            DialogResult = DialogResult.Cancel,
            AccessibleName = "Later"
        };
        laterButton.Click += (_, _) => Close();

        var releasePageButton = new Button
        {
            Text = "Open release &page",
            AutoSize = true,
            AccessibleName = "Open release page"
        };
        releasePageButton.Click += (_, _) => UpdateService.OpenReleasePage(updateInfo);

        buttonPanel.Controls.Add(installButton);
        buttonPanel.Controls.Add(laterButton);
        buttonPanel.Controls.Add(releasePageButton);
        layout.Controls.Add(buttonPanel, 0, 3);

        AcceptButton = installButton;
        CancelButton = laterButton;
        Controls.Add(layout);
    }

    private static string FormatReleaseNotes(string markdown)
    {
        var text = ExtractWhatsNew(markdown.Replace("\r\n", "\n").Replace('\r', '\n')).Trim();

        text = Regex.Replace(text, @"`([^`\r\n]+)`", "$1");
        text = Regex.Replace(text, @"\*\*([^*]+)\*\*", "$1");
        text = Regex.Replace(text, @"__([^_]+)__", "$1");
        text = Regex.Replace(text, @"\*([^*\r\n]+)\*", "$1");
        text = Regex.Replace(text, @"_([^_\r\n]+)_", "$1");
        text = Regex.Replace(text, @"^\s{0,3}#{1,6}\s*", "", RegexOptions.Multiline);
        text = Regex.Replace(text, @"^\s*[-*+]\s+", "- ", RegexOptions.Multiline);
        text = Regex.Replace(text, @"\n{3,}", "\n\n");

        return text.Replace("\n", Environment.NewLine);
    }

    private static string ExtractWhatsNew(string markdown)
    {
        var match = Regex.Match(
            markdown,
            @"(?ims)^\s*##\s+What's New\s*(?:\n|$)(?<body>.*?)(?=^\s*##\s+\S|\z)");

        if (!match.Success)
        {
            return markdown;
        }

        var body = match.Groups["body"].Value.Trim();
        return string.IsNullOrWhiteSpace(body) ? markdown : body;
    }
}
