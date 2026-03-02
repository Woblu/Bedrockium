namespace AmethystLauncher.Services;

public interface IModInstallService
{
    Task InstallModAsync(string archivePath, string instanceDirectory, CancellationToken cancellationToken = default);
}
