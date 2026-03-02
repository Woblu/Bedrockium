using Wpf.Ui.Controls;

namespace AmethystLauncher.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindow(Wpf.Ui.INavigationService navigationService)
    {
        InitializeComponent();
        navigationService.SetNavigationControl(RootNavigation);
    }
}

