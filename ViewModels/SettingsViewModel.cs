using AmethystLauncher.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AmethystLauncher.ViewModels;

public partial class SettingsViewModel(ISettingsService settingsService) : ObservableObject
{
    [ObservableProperty]
    private string _title = "Launcher Settings";

    [ObservableProperty]
    private string _instancesDirectory = string.Empty;

    [ObservableProperty]
    private string _curseForgeApiKey = string.Empty;

    [RelayCommand]
    public async Task LoadSettingsAsync()
    {
        var settings = await settingsService.GetSettingsAsync();
        InstancesDirectory = settings.InstancesDirectory ?? string.Empty;
        CurseForgeApiKey = settings.CurseForgeApiKey ?? string.Empty;
    }

    [RelayCommand]
    public async Task SaveSettingsAsync()
    {
        var settings = await settingsService.GetSettingsAsync();
        settings.InstancesDirectory = InstancesDirectory;
        settings.CurseForgeApiKey = CurseForgeApiKey;
        await settingsService.SaveSettingsAsync(settings);
    }
}
