using System.Collections.ObjectModel;
using System.IO;
using AmethystLauncher.Models;
using AmethystLauncher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AmethystLauncher.ViewModels;

public partial class ModsViewModel(
    ICurseForgeService curseForgeService,
    IDownloadService downloadService,
    IModInstallService modInstallService,
    ISettingsService settingsService) : ObservableObject
{
    private readonly ICurseForgeService _curseForgeService = curseForgeService;
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IModInstallService _modInstallService = modInstallService;
    private readonly ISettingsService _settingsService = settingsService;

    public ObservableCollection<CurseForgeMod> TrendingMods { get; } = [];

    [ObservableProperty]
    private bool _isLoading;

    [RelayCommand]
    public async Task LoadModsAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        TrendingMods.Clear();
        try
        {
            var mods = await _curseForgeService.GetTrendingModsAsync();
            foreach (var mod in mods) TrendingMods.Add(mod);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task InstallModAsync(CurseForgeMod mod)
    {
        var settings = await _settingsService.GetSettingsAsync();
        if (string.IsNullOrWhiteSpace(settings.InstancesDirectory)) return;

        string tempFile = Path.Combine(Path.GetTempPath(), $"{mod.Id}.mcaddon");
        try
        {
            await _downloadService.DownloadFileAsync(mod.DownloadUrl, tempFile);
            await _modInstallService.InstallModAsync(tempFile, settings.InstancesDirectory);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
