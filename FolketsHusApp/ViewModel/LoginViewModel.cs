using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
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

    // IMPORTANT! Change "http" to "https" when deployed and SSL Certificate is created.
    // Change Url to webdomain when ready for production.
    private readonly string apiUrl = "http://10.0.2.2:5100/api/appAuth";

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

        else if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            // NO LONGER WORKS!!! Implement a custom alert lable on the page.
            //await Shell.Current.DisplayAlert("Error", "No Internet Access. Try again later", "OK");
            return;
        } else {

            var httpClient = new HttpClient();
            var loginParams = new LoginParams { Password = EnteredPassword };
            var content = new StringContent(JsonConvert.SerializeObject(loginParams), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                Debug.WriteLine($"Request called successfully, but failed on the server, Error code: {response.StatusCode}");
                return;
            } else {
                Debug.WriteLine("Request successfull, should redirect");
                await navigationService.GoToAsync($"//{nameof(Pages.HomePage)}");
                return;
            }

        }

    }
}
