using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AmethystLauncher.Services;

public class DownloadService(HttpClient httpClient) : IDownloadService
{
    public async Task DownloadFileAsync(string url, string destinationPath, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        var fileInfo = new FileInfo(destinationPath);
        var existingLength = fileInfo.Exists ? fileInfo.Length : 0L;

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (existingLength > 0)
            request.Headers.Range = new RangeHeaderValue(existingLength, null);

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = response.Content;
        var totalBytes = content.Headers.ContentLength ?? 0L;
        if (response.StatusCode == System.Net.HttpStatusCode.PartialContent && content.Headers.ContentRange?.Length.HasValue == true)
            totalBytes = content.Headers.ContentRange.Length.Value;

        await using var sourceStream = await content.ReadAsStreamAsync(cancellationToken);
        var fileMode = existingLength > 0 ? FileMode.Append : FileMode.Create;
        await using var destStream = new FileStream(destinationPath, fileMode, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true);

        var buffer = new byte[81920];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await destStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalBytesRead += bytesRead;
            if (totalBytes > 0 && progress != null)
            {
                var current = existingLength + totalBytesRead;
                progress.Report((double)current / totalBytes);
            }
        }
    }
}
