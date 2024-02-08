using FolketsHusApp.Pages;
using FolketsHusApp.ViewModel;
using Microsoft.Extensions.Logging;

namespace FolketsHusApp {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services
                .AddSingleton<IConnectivity>(Connectivity.Current)
                .AddSingleton<INavigationService, NavigationService>()
                .AddSingleton<IAPIService, APIService>()

                .AddTransient<AppShell>()

                .AddTransient<LoginPage>()
                .AddTransient<LoginViewModel>()

                .AddTransient<HomePage>()
                .AddTransient<HomeViewModel>()

                .AddTransient<BioRosenQuickPage>()
                .AddTransient<BioRosenQuickViewModel>()

                .AddTransient<BioRosenFilmPage>()
                .AddTransient<BioRosenFilmViewModel>()

                .AddTransient<BioRosenLivePage>()
                .AddTransient<BioRosenLiveViewModel>()

                .AddTransient<BioRosenKontrastPage>()
                .AddTransient<BioRosenKontrastViewModel>()

                .AddTransient<FilmDetailPage>()
                .AddTransient<FilmDetailViewModel>()

                .AddTransient<UnesPage>()
                .AddTransient<UnesViewModel>()

                .AddTransient<EvenemangPage>()
                .AddTransient<EvenemangViewModel>()

                .AddTransient<LogoutSubPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}
