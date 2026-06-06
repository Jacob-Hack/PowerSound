using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace PowerSound.Installer;

internal static class Program
{
    private const string AppName = "PowerSound";

    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Any(arg => string.Equals(arg, "--uninstall", StringComparison.OrdinalIgnoreCase)))
        {
            Uninstall();
            return;
        }

        Application.Run(new InstallerForm());
    }

    public static string InstallDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", AppName);

    public static string AppPath => Path.Combine(InstallDirectory, "PowerSound.exe");

    public static string InstalledSetupPath => Path.Combine(InstallDirectory, "PowerSound-Setup.exe");

    public static void Install(bool launchAfterInstall)
    {
        Directory.CreateDirectory(InstallDirectory);
        ExtractPayload(AppPath);
        CopyInstaller();
        CreateStartMenuShortcut();
        RegisterUninstaller();

        if (launchAfterInstall)
        {
            Process.Start(new ProcessStartInfo(AppPath) { UseShellExecute = true });
        }
    }

    private static void ExtractPayload(string targetPath)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PowerSound.exe");
        if (stream is null)
        {
            throw new InvalidOperationException("The installer payload is missing. Rebuild the installer after publishing PowerSound.exe.");
        }

        using var output = File.Create(targetPath);
        stream.CopyTo(output);
    }

    private static void CopyInstaller()
    {
        var currentPath = Environment.ProcessPath;
        if (!string.IsNullOrWhiteSpace(currentPath))
        {
            File.Copy(currentPath, InstalledSetupPath, overwrite: true);
        }
    }

    private static void RegisterUninstaller()
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\PowerSound");
        key.SetValue("DisplayName", "PowerSound");
        key.SetValue("DisplayVersion", "0.1.0");
        key.SetValue("Publisher", "Jacob-Hack");
        key.SetValue("InstallLocation", InstallDirectory);
        key.SetValue("DisplayIcon", AppPath);
        key.SetValue("UninstallString", $"\"{InstalledSetupPath}\" --uninstall");
        key.SetValue("QuietUninstallString", $"\"{InstalledSetupPath}\" --uninstall --quiet");
        key.SetValue("NoModify", 1, RegistryValueKind.DWord);
        key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
    }

    private static void CreateStartMenuShortcut()
    {
        var startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        var programs = Path.Combine(startMenu, "Programs");
        Directory.CreateDirectory(programs);
        var shortcutPath = Path.Combine(programs, "PowerSound.lnk");

        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        if (shellType is null)
        {
            return;
        }

        dynamic shell = Activator.CreateInstance(shellType)!;
        dynamic shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = AppPath;
        shortcut.WorkingDirectory = InstallDirectory;
        shortcut.Description = "PowerSound";
        shortcut.Save();
    }

    private static void Uninstall()
    {
        var quiet = Environment.GetCommandLineArgs().Any(arg => string.Equals(arg, "--quiet", StringComparison.OrdinalIgnoreCase));
        if (!quiet)
        {
            var result = MessageBox.Show(
                "Remove PowerSound from this computer?",
                "Uninstall PowerSound",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }
        }

        TryKillRunningApp();
        DeleteStartMenuShortcut();
        DeleteStartupEntry();
        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\PowerSound", throwOnMissingSubKey: false);

        TryDelete(AppPath);
        DeleteInstallerAfterExit();

        if (!quiet)
        {
            MessageBox.Show("PowerSound was removed.", "PowerSound", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private static void TryKillRunningApp()
    {
        foreach (var process in Process.GetProcessesByName("PowerSound"))
        {
            try
            {
                process.Kill();
                process.WaitForExit(3000);
            }
            catch
            {
                // The uninstall can continue even if the app has already exited or cannot be closed.
            }
        }
    }

    private static void DeleteStartMenuShortcut()
    {
        var shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "PowerSound.lnk");
        TryDelete(shortcutPath);
    }

    private static void DeleteStartupEntry()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
        key?.DeleteValue("PowerSound", throwOnMissingValue: false);
    }

    private static void DeleteInstallerAfterExit()
    {
        var command = $"/C timeout /T 2 /NOBREAK > NUL & rmdir /S /Q \"{InstallDirectory}\"";
        Process.Start(new ProcessStartInfo("cmd.exe", command)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        });
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Leave stubborn files behind rather than failing the uninstall UI.
        }
    }
}
