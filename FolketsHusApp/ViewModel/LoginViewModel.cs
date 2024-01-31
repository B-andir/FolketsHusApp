using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Xml.Serialization;

namespace FolketsHusApp.ViewModel;

internal class LoginParams {
    /// <summary>
    /// Password used for access to the app
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}

internal class LoginResponse {
    /// <summary>
    /// Response status of request
    /// </summary>
    [JsonProperty("status")]
    [SoapAttribute(AttributeName = "Status")]
    public string Status { get; set; } = "400";
}

public partial class LoginViewModel : ObservableObject {

    IConnectivity connectivity;
    private readonly INavigationService navigationService;
    private IAPIService api;

    public LoginViewModel(IConnectivity connectivity, INavigationService navigationService, IAPIService api) {
        this.connectivity = connectivity;
        this.navigationService = navigationService;
        this.api = api;
    }

    [ObservableProperty]
    string? enteredPassword;

    private const string password = "test";

    [RelayCommand]
    async Task Submit() {
        if (string.IsNullOrWhiteSpace(EnteredPassword))
            return;

        else if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            // NO LONGER WORKS!!! Implement a custom alert lable on the page.
            //await Shell.Current.DisplayAlert("Error", "No Internet Access. Try again later", "OK");
            return;
        } else {

            var loginParams = new LoginParams { Password = EnteredPassword };

            Response response = api.post("/appAuth", loginParams, false);

            JObject responseJson = JObject.Parse(response.responseString);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Debug.WriteLine($"Request called successfully, but failed on the server, Error code: {response.StatusCode}");
                return;
            } else {
                Debug.WriteLine("Request successfull, should redirect");

                PreferencesStore.Set("accessToken", responseJson.GetValue("accessToken"));
                PreferencesStore.Set("refreshToken", responseJson.GetValue("refreshToken"));

                await navigationService.GoToAsync($"//{nameof(Pages.HomePage)}");
            }

        }

    }
}
