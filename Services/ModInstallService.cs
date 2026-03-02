using System.IO;
using System.IO.Compression;
using System.Text.Json;
using AmethystLauncher.Models;

namespace AmethystLauncher.Services;

public class ModInstallService : IModInstallService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task InstallModAsync(string archivePath, string instanceDirectory, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = File.OpenRead(archivePath);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var manifestEntries = archive.Entries
            .Where(e => e.Name.Equals("manifest.json", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var manifestEntry in manifestEntries)
        {
            await using var manifestStream = manifestEntry.Open();
            var manifest = await JsonSerializer.DeserializeAsync<BedrockManifest>(manifestStream, JsonOptions, cancellationToken);
            if (manifest?.Header == null || manifest.Modules is not { Length: > 0 })
                continue;

            var targetSubDir = manifest.Modules[0].Type == "data" ? "behavior_packs" : "resource_packs";
            var safeName = string.Join("_", manifest.Header.Name.Split(Path.GetInvalidFileNameChars()));
            var extractPath = Path.Combine(instanceDirectory, targetSubDir, safeName);
            var prefix = manifestEntry.FullName[..^manifestEntry.Name.Length];

            foreach (var entry in archive.Entries.Where(e => e.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                var relativePath = entry.FullName[prefix.Length..].TrimStart('/', '\\');
                var destinationPath = Path.Combine(extractPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                entry.ExtractToFile(destinationPath, overwrite: true);
            }
        }
    }
}
