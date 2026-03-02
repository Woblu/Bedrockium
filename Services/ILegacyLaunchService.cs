namespace AmethystLauncher.Services;

public interface ILegacyLaunchService
{
    Task LaunchLegacyAsync(string executablePath, string arguments, CancellationToken cancellationToken = default);
}
