using System.Windows.Controls;
using AmethystLauncher.ViewModels;

namespace AmethystLauncher.Views;

public partial class ModsPage : Page
{
    public ModsPage(ModsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += (s, e) =>
        {
            if (ViewModel.TrendingMods.Count == 0) ViewModel.LoadModsCommand.Execute(null);
        };
    }

    public ModsViewModel ViewModel { get; }
}
