using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PowerSound;

internal static class UpdateService
{
    private static readonly Uri LatestReleaseUri = new("https://api.github.com/repos/Jacob-Hack/PowerSound/releases/latest");

    public static async Task<UpdateInfo?> CheckForUpdateAsync(CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PowerSound", AppVersion.Current.ToString(3)));

        using var response = await client.GetAsync(LatestReleaseUri, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        var root = document.RootElement;
        var tagName = root.GetProperty("tag_name").GetString() ?? string.Empty;
        if (!TryParseVersion(tagName, out var latestVersion) || latestVersion <= AppVersion.Current)
        {
            return null;
        }

        var releasePage = root.GetProperty("html_url").GetString();
        var installerUrl = FindInstallerUrl(root);
        if (string.IsNullOrWhiteSpace(releasePage) || string.IsNullOrWhiteSpace(installerUrl))
        {
            return null;
        }

        return new UpdateInfo(tagName, latestVersion, new Uri(releasePage), new Uri(installerUrl));
    }

    public static async Task<string> DownloadInstallerAsync(UpdateInfo updateInfo, CancellationToken cancellationToken = default)
    {
        var downloadDirectory = Path.Combine(Path.GetTempPath(), "PowerSound", "Updates", updateInfo.VersionText);
        Directory.CreateDirectory(downloadDirectory);

        var installerPath = Path.Combine(downloadDirectory, "PowerSound-Setup.exe");
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PowerSound", AppVersion.Current.ToString(3)));

        await using var input = await client.GetStreamAsync(updateInfo.InstallerUri, cancellationToken).ConfigureAwait(false);
        await using var output = File.Create(installerPath);
        await input.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
        return installerPath;
    }

    public static void LaunchInstaller(string installerPath)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = installerPath,
            UseShellExecute = true
        });
    }

    public static void OpenReleasesPage()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/Jacob-Hack/PowerSound/releases",
            UseShellExecute = true
        });
    }

    private static string? FindInstallerUrl(JsonElement releaseRoot)
    {
        foreach (var asset in releaseRoot.GetProperty("assets").EnumerateArray())
        {
            if (string.Equals(asset.GetProperty("name").GetString(), "PowerSound-Setup.exe", StringComparison.OrdinalIgnoreCase))
            {
                return asset.GetProperty("browser_download_url").GetString();
            }
        }

        return null;
    }

    private static bool TryParseVersion(string value, out Version version)
    {
        var versionText = value.Trim().TrimStart('v', 'V');
        var dashIndex = versionText.IndexOf('-');
        if (dashIndex >= 0)
        {
            versionText = versionText[..dashIndex];
        }

        return Version.TryParse(versionText, out version!);
    }
}
