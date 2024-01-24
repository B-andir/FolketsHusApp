using FolketsHusApp.Pages;

namespace FolketsHusApp;

public class NavigationService : INavigationService {

    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task GoToAsync(string route) {
        if (route == "//login") {
            App.Current.MainPage = _serviceProvider.GetRequiredService<LoginPage>();
        } else {
            var shell = App.Current.MainPage as AppShell ?? _serviceProvider.GetRequiredService<AppShell>();
            App.Current.MainPage = shell;
            await shell.GoToAsync(route);
        }
    }
}