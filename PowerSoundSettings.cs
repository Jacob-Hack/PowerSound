namespace PowerSound;

internal sealed class PowerSoundSettings
{
    public string ConnectedSoundPath { get; set; } = "";
    public string DisconnectedSoundPath { get; set; } = "";
    public bool StartWithWindows { get; set; }

    public bool UseDefaultConnectedSound => string.IsNullOrWhiteSpace(ConnectedSoundPath);
    public bool UseDefaultDisconnectedSound => string.IsNullOrWhiteSpace(DisconnectedSoundPath);

    public PowerSoundSettings Copy() => new()
    {
        ConnectedSoundPath = ConnectedSoundPath,
        DisconnectedSoundPath = DisconnectedSoundPath,
        StartWithWindows = StartWithWindows
    };

    public void CopyFrom(PowerSoundSettings source)
    {
        ConnectedSoundPath = source.ConnectedSoundPath;
        DisconnectedSoundPath = source.DisconnectedSoundPath;
        StartWithWindows = source.StartWithWindows;
    }
}
