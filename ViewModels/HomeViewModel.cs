using System.Collections.ObjectModel;
using System.IO;
using AmethystLauncher.Models;
using AmethystLauncher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AmethystLauncher.ViewModels;

public partial class HomeViewModel(
    IVersionCatalogService versionCatalogService,
    IDownloadService downloadService,
    IExtractionService extractionService,
    IUwpManagerService uwpManagerService,
    IInstanceSandboxService instanceSandboxService,
    ISettingsService settingsService) : ObservableObject
{
    private readonly IVersionCatalogService _versionCatalogService = versionCatalogService;
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IExtractionService _extractionService = extractionService;
    private readonly IUwpManagerService _uwpManagerService = uwpManagerService;
    private readonly IInstanceSandboxService _instanceSandboxService = instanceSandboxService;
    private readonly ISettingsService _settingsService = settingsService;

    public ObservableCollection<VersionEntry> Versions { get; } = [];

    [ObservableProperty]
    private VersionEntry? _selectedVersion;

    [ObservableProperty]
    private double _downloadProgress;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    public async Task LoadVersionsAsync()
    {
        if (Versions.Count > 0) return;
        try
        {
            StatusMessage = "Loading versions...";
            var versions = await _versionCatalogService.GetAvailableVersionsAsync();
            Versions.Clear();
            foreach (var v in versions) Versions.Add(v);
            StatusMessage = Versions.Count > 0 ? "Ready" : "No versions available";
        }
        catch
        {
            StatusMessage = "Failed to load versions";
        }
    }

    [RelayCommand]
    public async Task LaunchAsync()
    {
        if (IsBusy) return;
        var version = SelectedVersion;
        var settings = await _settingsService.GetSettingsAsync();
        if (version is null || string.IsNullOrWhiteSpace(settings.InstancesDirectory))
        {
            StatusMessage = "Select a version and set Instances path in Settings.";
            return;
        }

        IsBusy = true;
        string? tempArchive = null;
        string? extractDir = null;
        try
        {
            tempArchive = Path.Combine(Path.GetTempPath(), $"amethyst_{version.Id}.7z");
            extractDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AmethystLauncher", "Versions", version.Id);

            StatusMessage = "Downloading...";
            var progress = new Progress<double>(p =>
            {
                System.Windows.Application.Current?.Dispatcher.Invoke(() => DownloadProgress = p * 100d);
            });
            await _downloadService.DownloadFileAsync(version.DownloadUrl, tempArchive, progress);

            StatusMessage = "Extracting...";
            Directory.CreateDirectory(extractDir);
            await _extractionService.ExtractArchiveAsync(tempArchive, extractDir);

            StatusMessage = "Setting up instance...";
            await _instanceSandboxService.SwitchInstanceAsync(settings.InstancesDirectory!);

            StatusMessage = "Registering UWP...";
            await _uwpManagerService.UninstallMinecraftUwpAsync();
            var manifestPath = FindAppxManifest(extractDir);
            await _uwpManagerService.RegisterAndLaunchUwpAsync(manifestPath);

            StatusMessage = "Game Running!";
        }
        catch (Exception)
        {
            StatusMessage = "Launch failed. Check Settings and try again.";
        }
        finally
        {
            IsBusy = false;
            DownloadProgress = 0;
            if (tempArchive is not null && File.Exists(tempArchive))
                try { File.Delete(tempArchive); } catch { }
        }
    }

    private static string FindAppxManifest(string directory)
    {
        var path = Path.Combine(directory, "AppxManifest.xml");
        if (File.Exists(path)) return path;
        foreach (var file in Directory.EnumerateFiles(directory, "AppxManifest.xml", SearchOption.AllDirectories))
            return file;
        throw new FileNotFoundException("AppxManifest.xml not found after extraction.", directory);
    }
}
