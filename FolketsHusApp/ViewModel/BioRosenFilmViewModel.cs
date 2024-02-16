using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using FolketsHusApp.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

public class DeleteElementParams {
    [JsonProperty("dataType")]
    private string DataType = "DeleteElement";

    [JsonProperty("elementType")]
    public string ElementType { get; set; } = "";

    [JsonProperty("_id")]
    public string Id { get; set; } = "";
}

public partial class BioRosenFilmViewModel : ObservableObject {

    private IAPIService api;

    [ObservableProperty]
    public ObservableCollection<FilmObject> filmsObjects;

    [ObservableProperty]
    public bool isSwipeViewEnabled, noObjects;

    public BioRosenFilmViewModel(IAPIService api) {
        this.api = api;

        IsSwipeViewEnabled = true;
        noObjects = true;
        FilmsObjects = new ObservableCollection<FilmObject>();

        TrySetObjects();
    }

    void TrySetObjects() {

        FilmsObjects.Clear();

#pragma warning disable CS8604 // Possible null reference argument.
        try {
            Debug.WriteLine("Setting Objects");

            var filmCollection = PreferencesStore.GetJToken("film");

            if (filmCollection == null) {
                Debug.WriteLine("Film Collection is null");
            }

            int filmCount = filmCollection.Count();

            Debug.WriteLine($"Film count: {filmCount}");


            for (int i = 0; i < filmCount; i++) {

                FilmObject film = new FilmObject(filmCollection[i]);

                FilmsObjects.Add(film);

            }

#pragma warning restore CS8604 // Possible null reference argument.

        } catch {
            Debug.WriteLine("Failed to set objects");
        }

        if (FilmsObjects.Count > 0) {
            NoObjects = false;
        } else {
            NoObjects = true;
        }
    }

    [RelayCommand]
    void Refresh() {
        Debug.WriteLine("Try to refresh");
        TrySetObjects();
    }

    [RelayCommand]
    async void Delete(FilmObject obj) {
        Debug.WriteLine(obj.FilmName);

        DeleteElementParams deleteElementParams = new DeleteElementParams {
            ElementType = "film",
            Id = obj._id ?? ""
        };

        Response response = api.postProtected("/sendData", deleteElementParams);

        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            await Application.Current.MainPage.DisplayAlert("Error", response.responseString, "OK");
        } else {

            JObject responseJson = JObject.Parse(response.responseString);

            foreach (JProperty property in responseJson.Properties()) {
                Debug.WriteLine(string.Format("Writing {0} data to memory", property.Name));
                PreferencesStore.Delete(property.Name);
                PreferencesStore.Set(property.Name, responseJson.GetValue(property.Name));
            }

            Refresh();
        }

    }

    [RelayCommand]
    async Task Tap(FilmObject obj) {

        FilmObject filmObject = obj as FilmObject;

        var navigationParameter = new Dictionary<string, object> {
            { "FilmObject", filmObject }
        };

        await Shell.Current.GoToAsync(nameof(FilmDetailPage), navigationParameter);
    }

}
