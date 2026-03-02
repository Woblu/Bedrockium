using Wpf.Ui.Abstractions;

namespace AmethystLauncher.Services;

public class ApplicationPageService(IServiceProvider serviceProvider) : INavigationViewPageProvider
{
    public object? GetPage(Type pageType) =>
        serviceProvider.GetService(pageType);
}
