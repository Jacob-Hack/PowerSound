namespace PowerSound;

internal sealed class PowerSoundSettings
{
    public string ConnectedSoundPath { get; set; } = "";
    public string DisconnectedSoundPath { get; set; } = "";
    public bool StartWithWindows { get; set; }

    public bool UseDefaultConnectedSound => string.IsNullOrWhiteSpace(ConnectedSoundPath);
    public bool UseDefaultDisconnectedSound => string.IsNullOrWhiteSpace(DisconnectedSoundPath);
}
