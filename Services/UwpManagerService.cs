using System.Diagnostics;
using System.Management.Automation;

namespace AmethystLauncher.Services;

public class UwpManagerService : IUwpManagerService
{
    public async Task UninstallMinecraftUwpAsync(CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            using var ps = PowerShell.Create();
            ps.AddScript("Get-AppxPackage -Name Microsoft.MinecraftUWP | Remove-AppxPackage");
            ps.Invoke();
            if (ps.Streams.Error.Count > 0)
                throw new InvalidOperationException(string.Join("; ", ps.Streams.Error.Select(e => e.ToString())));
        }, cancellationToken);
    }

    public async Task RegisterAndLaunchUwpAsync(string manifestPath, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            using var ps = PowerShell.Create();
            ps.AddCommand("Add-AppxPackage")
                .AddParameter("Register", manifestPath)
                .AddParameter("DisableDevelopmentMode", true);
            ps.Invoke();
            if (ps.Streams.Error.Count > 0)
                throw new InvalidOperationException(string.Join("; ", ps.Streams.Error.Select(e => e.ToString())));
        }, cancellationToken);

        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = "shell:AppsFolder\\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App",
            UseShellExecute = true
        });
    }
}
