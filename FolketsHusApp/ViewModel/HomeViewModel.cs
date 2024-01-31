using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    }


    [RelayCommand]
    public void RefreshAllContent() {
        FetchDataParams fetchDataParams = new FetchDataParams("all");
        Response response = api.postProtected("/fetchData", fetchDataParams);

        JObject responseJson = JObject.Parse(response.responseString);

        //Debug.WriteLine(responseJson.GetValue("BioRosen"));

        /* Save retrieved data into the Preferences Store.
         * The data is retrieved with:
         * PreferencesStore.GetJToken("film")[0]["name"];
         * The above code will return the name of the first film in the array.
         */

        if (responseJson != null) {
            JObject bioRosen = (JObject)responseJson.GetValue("BioRosen");
            JObject apis = (JObject)responseJson.GetValue("AppAPIs");

            if (bioRosen != null) {

                foreach (JProperty property in bioRosen.Properties()) {
                    PreferencesStore.Set(property.Name, bioRosen.GetValue(property.Name));
                }

                Debug.WriteLine(PreferencesStore.GetJToken("film")[0]);
            }

            if (apis != null) {

            }
        }


    }
}
