using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AmethystLauncher.Models;

namespace AmethystLauncher.Services;

public class CurseForgeService(HttpClient httpClient) : ICurseForgeService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IEnumerable<CurseForgeMod>> GetTrendingModsAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("v1/mods/search?gameId=432", cancellationToken);
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<List<CurseForgeMod>>(JsonOptions, cancellationToken);
        return list ?? [];
    }
}
