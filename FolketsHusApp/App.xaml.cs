using FolketsHusApp.ViewModel;

namespace FolketsHusApp {
    public partial class App : Application {
        public App(LoginViewModel vm) {
            InitializeComponent();

            MainPage = new Pages.LoginPage(vm);

        }
    }
}
