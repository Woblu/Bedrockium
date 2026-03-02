using System.Diagnostics;
using System.IO;

namespace AmethystLauncher.Services;

public class LegacyLaunchService : ILegacyLaunchService
{
    public Task LaunchLegacyAsync(string executablePath, string arguments, CancellationToken cancellationToken = default)
    {
        var workingDir = Path.GetDirectoryName(executablePath);
        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            WorkingDirectory = string.IsNullOrEmpty(workingDir) ? null : workingDir,
            UseShellExecute = false
        };
        Process.Start(startInfo);
        return Task.CompletedTask;
    }
}
