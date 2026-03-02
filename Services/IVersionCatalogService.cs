using AmethystLauncher.Models;

namespace AmethystLauncher.Services;

public interface IVersionCatalogService
{
    Task<IEnumerable<VersionEntry>> GetAvailableVersionsAsync(CancellationToken cancellationToken = default);
}
