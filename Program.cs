using System.Windows.Forms;

namespace PowerSound;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new PowerSoundApplicationContext());
    }
}
