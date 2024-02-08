using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

internal class FetchDataParams {
    public FetchDataParams(string targetData) {
        this.targetData = targetData;
    }

    [JsonProperty("targetData")]
    public string targetData { get; set; }
}

public partial class HomeViewModel : ObservableObject {

    private IAPIService api;

    public HomeViewModel(IAPIService api) {
        this.api = api;

        getAPIs();
    }

    /// <summary>
    /// Check to see if required API secrets are in secure storage. If not, get them from webserver and store them
    /// </summary>
    private async void getAPIs() {

        string cloudinaryAPI = await SecureStorage.Default.GetAsync("cloudinaryAPI_cloud_name");

        if (cloudinaryAPI == null) {
            // Retrieve cloudinary API information
            Debug.WriteLine("Retrieve Cloudinary API Secrets");

            FetchDataParams apiParams = new FetchDataParams("cloudinary");
            Response response = api.postProtected("/fetchAPISecrets", apiParams);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                JObject responseJson = JObject.Parse(response.responseString);

                JToken secrets = responseJson.GetValue("data");

                if (secrets != null) {
                    await SecureStorage.Default.SetAsync("cloudinaryAPI_cloud_name", secrets[0].ToString());
                    await SecureStorage.Default.SetAsync("cloudinaryAPI_api_key", secrets[1].ToString());
                    await SecureStorage.Default.SetAsync("cloudinaryAPI_api_secret", secrets[2].ToString());
                }
            }

        } else {
            Debug.WriteLine("No API Secrets to Get");
            return;
        }
    }


    [RelayCommand]
    public void RefreshAllContent() {
        FetchDataParams fetchDataParams = new FetchDataParams("all");
        Response response = api.postProtected("/fetchData", fetchDataParams);


        /* Save retrieved data into the Preferences Store.
         * The data is retrieved with:
         * PreferencesStore.GetJToken("film")[0]["name"];
         * The above code will return the name of the first film in the array.
         */

        if (response.responseString != null && response.responseString != "") {
            JObject responseJson = JObject.Parse(response.responseString);
            JObject bioRosen = (JObject)responseJson.GetValue("BioRosen");
            JObject events = (JObject)responseJson.GetValue("event");

            if (bioRosen != null) {

                foreach (JProperty property in bioRosen.Properties()) {
                    Debug.WriteLine(String.Format("Writing {0} data to memory", property.Name));
                    //PreferencesStore.Delete(property.Name);
                    PreferencesStore.Set(property.Name, bioRosen.GetValue(property.Name));
                }
            } else {
                Debug.WriteLine("Nothing in Bio Rosen to write to memory");
            }

            if (events != null) {

                foreach (JProperty property in events.Properties()) {
                    Debug.WriteLine(String.Format("Writing {0} data to memory", property.Name));
                    PreferencesStore.Delete(property.Name);
                    PreferencesStore.Set(property.Name, events.GetValue(property.Name));
                }
            } else {
                Debug.WriteLine("No Events to write to memory");
            }
        }

        getAPIs();
    }
}
