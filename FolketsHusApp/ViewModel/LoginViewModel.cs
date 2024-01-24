using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Pages;

namespace FolketsHusApp.ViewModel;

public partial class LoginViewModel : ObservableObject {

    IConnectivity connectivity;
    private readonly INavigationService navigationService;

    public LoginViewModel(IConnectivity connectivity, INavigationService navigationService) {
        this.connectivity = connectivity;
        this.navigationService = navigationService;
    }

    [ObservableProperty]
    string? enteredPassword;

    private const string password = "test";

    [RelayCommand]
    async Task Submit() {
        if (string.IsNullOrWhiteSpace(EnteredPassword))
            return;

        if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            await Shell.Current.DisplayAlert("Error", "No Internet Access. Try again later", "OK");
            return;
        }

        if (EnteredPassword != password)
            return;

        await navigationService.GoToAsync($"//{nameof(HomePage)}");
    }

}
