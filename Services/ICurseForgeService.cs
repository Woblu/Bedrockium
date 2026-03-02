using AmethystLauncher.Models;

namespace AmethystLauncher.Services;

public interface ICurseForgeService
{
    Task<IEnumerable<CurseForgeMod>> GetTrendingModsAsync(CancellationToken cancellationToken = default);
}
