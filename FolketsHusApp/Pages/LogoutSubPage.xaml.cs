namespace FolketsHusApp.Pages;

public partial class LogoutSubPage : ContentPage {

    public LogoutSubPage(INavigationService navigationService) {
        InitializeComponent();

        navigationService.GoToAsync("//login");
    }
}