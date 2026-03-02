namespace AmethystLauncher.Services;

public interface IExtractionService
{
    Task ExtractArchiveAsync(string archivePath, string destinationDirectory, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
