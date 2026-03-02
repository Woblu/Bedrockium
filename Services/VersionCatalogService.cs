using System.Net.Http;
using System.Net.Http.Json;
using AmethystLauncher.Models;

namespace AmethystLauncher.Services;

public class VersionCatalogService(HttpClient httpClient) : IVersionCatalogService
{
    private const string CatalogUrl = "https://raw.githubusercontent.com/placeholder/versions.json";

    public async Task<IEnumerable<VersionEntry>> GetAvailableVersionsAsync(CancellationToken cancellationToken = default)
    {
        var entries = await httpClient.GetFromJsonAsync<VersionEntry[]>(CatalogUrl, cancellationToken);
        return entries ?? [];
    }
}
