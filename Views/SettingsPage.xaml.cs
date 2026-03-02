using System.Windows.Controls;
using AmethystLauncher.ViewModels;

namespace AmethystLauncher.Views;

public partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        ViewModel = viewModel;
        Loaded += (_, _) => ViewModel.LoadSettingsCommand.Execute(null);
    }
}
