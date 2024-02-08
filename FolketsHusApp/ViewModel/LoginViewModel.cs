using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace FolketsHusApp.ViewModel;

internal class LoginParams {
    /// <summary>
    /// Password used for access to the app
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; } = "";
    [JsonProperty("password")]
    public string Password { get; set; } = "";
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

        api.preloadService();

        try {
            RememberUser = PreferencesStore.Get<bool>("LoginRememberUser");
        } catch {
            RememberUser = false;
        }

        if (RememberUser) {
            EnteredUsername = SecureStorage.GetAsync("LoginUsername").Result;
        }
    }

    [ObservableProperty]
    string? enteredPassword, enteredUsername, labelText = "Loading ...";

    [ObservableProperty]
    Color labelColor = new Color(137, 224, 96);

    [ObservableProperty]
    bool labelVisible, enableButton = true, loading = false, rememberUser = false;

    async void SetLabel(string text, Color color) {

        LabelVisible = true;
        LabelText = text;
        LabelColor = color;

        await Task.Delay(10);

        return;
    }

    [RelayCommand]
    async Task Submit() {
        EnableButton = false;
        LabelVisible = false;
        Loading = true;

        // Necessary to allow the UI thread to refresh the screen
        await Task.Delay(10);

        if (string.IsNullOrWhiteSpace(EnteredPassword) || string.IsNullOrEmpty(EnteredUsername)) {
            SetLabel("Please enter both your Username and your Password", new Color(230, 134, 106));

        } else if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            SetLabel("No internet connection! Try again later.", new Color(230, 134, 106));

        } else {

            var loginParams = new LoginParams { Username = EnteredUsername, Password = EnteredPassword };

            Response response = api.post("/authorize", loginParams, false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                Debug.WriteLine("Username and Password missmatch");
                SetLabel("Username or Password was incorrect, please try again", new Color(230, 134, 106));

            } else if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Debug.WriteLine($"Request called successfully, but failed on the server, Error code: {response.StatusCode}");
                SetLabel("Server error! Please try again later.", new Color(230, 134, 106));

            } else {
                Debug.WriteLine("Request successfull, should redirect");

                JObject responseJson = JObject.Parse(response.responseString);

                PreferencesStore.Set("accessToken", responseJson.GetValue("accessToken"));
                PreferencesStore.Set("refreshToken", responseJson.GetValue("refreshToken"));

                LabelVisible = false;

                if (RememberUser) {
                    await SecureStorage.SetAsync("LoginUsername", EnteredUsername);
                }

                PreferencesStore.Set("LoginRememberUser", RememberUser);

                await navigationService.GoToAsync($"//{nameof(Pages.HomePage)}");

            }
        }

        Loading = false;
        EnableButton = true;

    }
}
