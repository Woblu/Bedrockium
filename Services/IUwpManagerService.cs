namespace AmethystLauncher.Services;

public interface IUwpManagerService
{
    Task UninstallMinecraftUwpAsync(CancellationToken cancellationToken = default);
    Task RegisterAndLaunchUwpAsync(string manifestPath, CancellationToken cancellationToken = default);
}
