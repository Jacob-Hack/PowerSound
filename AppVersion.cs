using System.Reflection;

namespace PowerSound;

internal static class AppVersion
{
    public static Version Current =>
        typeof(AppVersion).Assembly.GetName().Version ?? new Version(0, 0);

    public static string Display =>
        typeof(AppVersion).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? Current.ToString(3);
}
