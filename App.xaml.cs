using System.Windows;

using System;
using AmethystLauncher.Data;
using AmethystLauncher.Services;
using AmethystLauncher.ViewModels;
using AmethystLauncher.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AmethystLauncher;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    private IServiceScope? _mainScope;

    public App()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        Services = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AmethystDbContext>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddHttpClient<IVersionCatalogService, VersionCatalogService>();
        services.AddHttpClient<IDownloadService, DownloadService>();
        services.AddHttpClient<ICurseForgeService, CurseForgeService>(client =>
            client.BaseAddress = new Uri("https://amethyst-proxy.boaz-vega.workers.dev/"));
        services.AddTransient<IModInstallService, ModInstallService>();
        services.AddTransient<IExtractionService, ExtractionService>();
        services.AddTransient<IUwpManagerService, UwpManagerService>();
        services.AddTransient<ILegacyLaunchService, LegacyLaunchService>();
        services.AddTransient<IInstanceSandboxService, InstanceSandboxService>();
        services.AddSingleton<Wpf.Ui.Abstractions.INavigationViewPageProvider, ApplicationPageService>();
        services.AddSingleton<Wpf.Ui.INavigationService, Wpf.Ui.NavigationService>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<ModsViewModel>();
        services.AddTransient<ModsPage>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mainScope = Services.CreateScope();
        var dbContext = _mainScope.ServiceProvider.GetRequiredService<AmethystDbContext>();
        dbContext.Database.EnsureCreatedAsync().GetAwaiter().GetResult();

        var navigationService = _mainScope.ServiceProvider.GetRequiredService<Wpf.Ui.INavigationService>();
        var mainWindow = _mainScope.ServiceProvider.GetRequiredService<MainWindow>();
        var mainViewModel = _mainScope.ServiceProvider.GetRequiredService<MainViewModel>();

        mainWindow.DataContext = mainViewModel;
        mainWindow.Closed += (_, _) => _mainScope?.Dispose();
        mainWindow.Loaded += (_, _) => navigationService.Navigate(typeof(Views.HomePage));
        mainWindow.Show();
    }
}
