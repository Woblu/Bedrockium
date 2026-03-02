namespace AmethystLauncher.Services;

public interface IInstanceSandboxService
{
    Task SwitchInstanceAsync(string targetInstanceDirectory, CancellationToken cancellationToken = default);
}
