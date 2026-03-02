using CommunityToolkit.Mvvm.ComponentModel;

namespace AmethystLauncher.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _windowTitle = "Amethyst Launcher - Pre-Alpha";
}

