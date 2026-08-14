namespace PowerSound;

internal sealed record UpdateInfo(
    string VersionText,
    Version Version,
    Uri ReleasePageUri,
    Uri InstallerUri,
    string ReleaseNotes);
