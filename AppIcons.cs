using System.Drawing;

namespace PowerSound;

internal static class AppIcons
{
    public static Icon CreateApplicationIcon()
    {
        using var stream = typeof(AppIcons).Assembly.GetManifestResourceStream("PowerSound.Assets.PowerSound.ico")
            ?? throw new InvalidOperationException("Embedded PowerSound icon resource was not found.");
        return new Icon(stream);
    }
}
