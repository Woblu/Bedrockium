using System.Diagnostics;
using System.IO;

namespace AmethystLauncher.Services;

public class InstanceSandboxService : IInstanceSandboxService
{
    private static readonly string MojangPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        @"Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang");

    public async Task SwitchInstanceAsync(string targetInstanceDirectory, CancellationToken cancellationToken = default)
    {
        var mojangDir = new DirectoryInfo(MojangPath);
        if (mojangDir.Exists)
        {
            var isReparsePoint = (mojangDir.Attributes & FileAttributes.ReparsePoint) != 0;
            if (isReparsePoint)
                Directory.Delete(MojangPath);
            else
                Directory.Move(MojangPath, MojangPath + ".backup");
        }

        Directory.CreateDirectory(targetInstanceDirectory);

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c mklink /J \"{MojangPath}\" \"{targetInstanceDirectory}\"",
            CreateNoWindow = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi);
        if (process == null)
            throw new InvalidOperationException("Failed to start cmd.exe for junction creation.");
        await process.WaitForExitAsync(cancellationToken);
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Junction creation failed with exit code {process.ExitCode}.");
    }
}
