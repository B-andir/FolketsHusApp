using FolketsHusApp.Pages;

namespace FolketsHusApp {
    public partial class AppShell : Shell {
        public AppShell() {
            InitializeComponent();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(BioRosenQuickPage), typeof(BioRosenQuickPage));
            Routing.RegisterRoute(nameof(BioRosenFilmPage), typeof(BioRosenFilmPage));
            Routing.RegisterRoute(nameof(BioRosenLivePage), typeof(BioRosenLivePage));
            Routing.RegisterRoute(nameof(BioRosenKontrastPage), typeof(BioRosenKontrastPage));
            Routing.RegisterRoute(nameof(UnesPage), typeof(UnesPage));
            Routing.RegisterRoute(nameof(EvenemangPage), typeof(EvenemangPage));
        }
    }
}
