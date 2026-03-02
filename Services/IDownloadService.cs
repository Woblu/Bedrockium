namespace AmethystLauncher.Services;

public interface IDownloadService
{
    Task DownloadFileAsync(string url, string destinationPath, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
