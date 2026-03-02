using System.IO;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace AmethystLauncher.Services;

public class ExtractionService : IExtractionService
{
    public async Task ExtractArchiveAsync(string archivePath, string destinationDirectory, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(destinationDirectory);

        await Task.Run(() =>
        {
            using var archive = ArchiveFactory.OpenArchive(archivePath);
            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
            var total = entries.Count;
            IProgress<SharpCompress.Common.ProgressReport>? sharpProgress = null;
            if (progress != null && total > 0)
            {
                var current = 0;
                sharpProgress = new Progress<SharpCompress.Common.ProgressReport>(_ =>
                {
                    current++;
                    progress.Report((double)current / total);
                });
            }
            archive.WriteToDirectory(destinationDirectory, sharpProgress);
        }, cancellationToken);

        if (Directory.Exists(destinationDirectory) && Directory.EnumerateFileSystemEntries(destinationDirectory).Any())
            File.Delete(archivePath);
    }
}
