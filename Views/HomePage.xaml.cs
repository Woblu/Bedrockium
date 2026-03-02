using System.Windows.Controls;
using AmethystLauncher.ViewModels;

namespace AmethystLauncher.Views;

public partial class HomePage : Page
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += (s, e) => ViewModel.LoadVersionsCommand.Execute(null);
    }

    public HomeViewModel ViewModel { get; }
}
