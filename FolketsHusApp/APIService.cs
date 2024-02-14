
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace FolketsHusApp;

internal class ProtectedRequest {

    public ProtectedRequest() { }

    public ProtectedRequest(string accessToken, object data) {
        AccessToken = accessToken;
        Data = data;
    }

    [JsonProperty("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonProperty("data")]
    public object? Data { get; set; }
}

internal class AccessTokenRequest {
    public AccessTokenRequest(string refreshToken) {
        RefreshToken = refreshToken;
    }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }

}

public class APIService : IAPIService {

    // IMPORTANT! Change "http" to "https" when deployed and SSL Certificate is created.
    // Change Url to webdomain when ready for production.
    //const string url = "http://146.190.16.245:80/api";  // Public IP
    const string url = "http://10.0.2.2:5100/api";  // Localhost

    private INavigationService navigationService;
    private IConnectivity connectivity;

    public APIService(INavigationService navigationService, IConnectivity connectivity) {
        this.navigationService = navigationService;
        this.connectivity = connectivity;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<HttpResponseMessage> sendPost(string route, StringContent body) {
        var httpClient = new HttpClient();
        if (connectivity.NetworkAccess == NetworkAccess.Internet) {
            return httpClient.PostAsync(route, body).Result;
        } else {
            return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

    private bool GetNewAccessToken() {
        var body = new AccessTokenRequest(PreferencesStore.Get<string>("refreshToken"));
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        string apiUrl = url + "/refreshAccessToken";

        HttpResponseMessage responseMessage = sendPost(apiUrl, content).Result;

        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK) {
            string responseContent = responseMessage.Content.ReadAsStringAsync().Result;
            JObject responseJson = JObject.Parse(responseContent);

            PreferencesStore.Set("accessToken", responseJson.GetValue("accessToken"));
            PreferencesStore.Set("refreshToken", responseJson.GetValue("refreshToken"));

            return true;
        } else {
            navigationService.GoToAsync("//login");

            return false;
        }
    }


    public Response post(string route, object body, bool shouldRedirect = true) {

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var apiUrl = url + route;

        HttpResponseMessage responseMessage = sendPost(apiUrl, content).Result;
        string responseContent = responseMessage.Content.ReadAsStringAsync().Result;

        if ((responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized || responseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest) && shouldRedirect) {
            return postProtected(route, body, 0);
        } else {
            return new Response(responseMessage, responseContent);
        }
    }

    public Response postProtected(string route, object body) {
        return postProtected(route, body, 0);
    }

    private Response postProtected(string route, object body, int iterations = 0) {

        Debug.WriteLine(string.Format("Sending post request to protedted route {0}, iteration {1}", route, iterations));

        var bodyProtected = new ProtectedRequest(PreferencesStore.Get<string>("accessToken"), body);

        var content = new StringContent(JsonConvert.SerializeObject(bodyProtected), Encoding.UTF8, "application/json");
        var apiUrl = url + route;

        HttpResponseMessage responseMessage = sendPost(apiUrl, content).Result;
        string responseContent = responseMessage.Content.ReadAsStringAsync().Result;

        if (iterations <= 0 && (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)) {
            // Access Token was denied. Try to get a new token, then retry to call the API
            bool result = GetNewAccessToken();

            if (result) {
                int i = iterations + 1;
                return postProtected(route, body, i);
            } else {
                return new Response();
            }

        } else if (iterations >= 1 && responseMessage.StatusCode != System.Net.HttpStatusCode.OK) {
            navigationService.GoToAsync("//login");

            return new Response();
        } else {
            return new Response(responseMessage, responseContent);
        }


    }

    public async Task<bool> preloadService() {

        await Task.Delay(10);

        return true;
    }
}
