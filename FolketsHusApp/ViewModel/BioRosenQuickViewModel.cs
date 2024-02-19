using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

internal class SaveShowParams {
    [JsonProperty("dataType")]
    private string DataType = "BioRosenShow";

    [JsonProperty("showType")]
    public string ShowType { get; set; } = "NaN";

    [JsonProperty("title")]
    public string Title { get; set; } = "Nan";

    [JsonProperty("description")]
    public string Description { get; set; } = "Nan";

    [JsonProperty("posterCloudinaryId")]
    public string PosterCloudinaryId { get; set; } = " ";

    [JsonProperty("posterURL")]
    public string PosterURL { get; set; } = " ";

    [JsonProperty("trailerURL")]
    public string TrailerURL { get; set; } = " ";

    [JsonProperty("ticketURL")]
    public string TicketURL { get; set; } = " ";

    [JsonProperty("ageRating")]
    public string AgeRating { get; set; } = " ";

    [JsonProperty("genre")]
    public string Genre { get; set; } = " ";

    [JsonProperty("runTime")]
    public int RunTime { get; set; } = 0;

    [JsonProperty("livePauses")]
    public int LivePauses { get; set; } = 0;

    [JsonProperty("isPremiere")]
    public bool IsPremiere { get; set; } = false;

    [JsonProperty("runDate")]
    public DateTime RunDate { get; set; } = new DateTime();

}

public partial class BioRosenQuickViewModel : ObservableObject {

    IConnectivity connectivity;
    IAPIService api;


    [ObservableProperty]
    int filmTypePickerIndex = -1, ageRatingIndex, livePausesCount = 0;

    [ObservableProperty]
    string? filmTitle, description, trailerURL, ticketURL, posterSource;

    [ObservableProperty]
    bool isPremiere = false, posterImageVisible = false, loading = false;

    [ObservableProperty]
    TimeSpan runTime = new DateTime(1, 1, 1, 1, 30, 0).TimeOfDay, runDateTime = new DateTime(1, 1, 1, 18, 0, 0).TimeOfDay;

    [ObservableProperty]
    DateTime runDate = DateTime.Now;

    [ObservableProperty]
    public List<SelectAgeRatingItem> ageRatingItems = new() {
        new SelectAgeRatingItem("Ingen Åldersgräns", ""),
        new SelectAgeRatingItem("7 år", "7 år"),
        new SelectAgeRatingItem("11 år", "11 år"),
        new SelectAgeRatingItem("15 år", "15 år")
    };

    [ObservableProperty]
    public SelectAgeRatingItem? ageRating;

    [ObservableProperty]
    public List<string> genresItems = new List<string>() { "Action", "Adventure", "Animation", "Comedy", "Crime", "Documentary", "Drama", "Family", "Fantasy", "Horror", "Musical", "Mystery", "Romance", "Science Fiction", "Thriller", "War", "Western" };

    [ObservableProperty]
    public List<int> genresIndices = new List<int>();

    public string GenresString = "";

    public BioRosenQuickViewModel(IConnectivity connectivity, IAPIService api) {
        this.connectivity = connectivity;
        this.api = api;
    }


    [RelayCommand]
    async Task ImageSelect() {
        if (MediaPicker.Default.IsCaptureSupported) {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null) {
                PosterImageVisible = true;
                PosterSource = photo.FullPath;
            }
        }
    }


#pragma warning disable CS8602 // Dereference of a possibly null reference.
    [RelayCommand]
    async Task SaveShow() {
        Loading = true;

        // Required to let the UI Thread update to show the activity indicator
        await Task.Delay(20);

        if (connectivity.NetworkAccess != NetworkAccess.Internet) {
            await Application.Current.MainPage.DisplayAlert("Network Error", "Not connected to the internet, please connect to a network and try again", "OK");
            Loading = false;
            return;
        }

        if (FilmTitle == null || PosterSource == null) {
            await Application.Current.MainPage.DisplayAlert("Error", "Please assign the show title", "OK");
            Loading = false;
            return;
        }

        if (PosterSource == null) {
            await Application.Current.MainPage.DisplayAlert("Error", "Please select a poster image", "OK");
            Loading = false;
            return;
        }

        string showType;

        if (FilmTypePickerIndex == 0)
            showType = "Film";
        else if (FilmTypePickerIndex == 1)
            showType = "Live";
        else if (FilmTypePickerIndex == 2)
            showType = "Kontrast";
        else {
            await Application.Current.MainPage.DisplayAlert("Error", "Please select a poster image", "OK");
            Loading = false;
            return;
        }

        DateTime _runDate = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day, RunDateTime.Hours, RunDateTime.Minutes, 0);

        string _description = Description != null ? Description.Replace("\n", "<br>") : "NaN";

        int _runTime = (int)RunTime.TotalMinutes;

        Account account = new Account(
                await SecureStorage.GetAsync("cloudinaryAPI_cloud_name"),
                await SecureStorage.GetAsync("cloudinaryAPI_api_key"),
                await SecureStorage.GetAsync("cloudinaryAPI_api_secret"));

        Cloudinary cloudinary = new Cloudinary(account);

        ImageUploadParams uploadParams = new ImageUploadParams {
            File = new FileDescription(PosterSource),
            Folder = string.Format("FolketsHusHallstavik/Posters/{0}", showType),
            PublicId = string.Format("{0}_poster_{1}", FilmTitle.Replace(' ', '_'), Guid.NewGuid()),
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null) {
            await Application.Current.MainPage.DisplayAlert("Error", string.Format("There was an issue uploading the image. Error message:\n{0}", uploadResult.Error.Message), "OK");
            Loading = false;
            return;
        }

        string posterURL = uploadResult.SecureUrl.ToString();
        string posterCloudinaryId = uploadResult.PublicId;

        Debug.WriteLine(posterURL);


        SaveShowParams saveShowParams = new SaveShowParams {
            ShowType = showType,
            Title = FilmTitle,
            Description = _description,
            PosterCloudinaryId = posterCloudinaryId,
            PosterURL = posterURL,
            TrailerURL = TrailerURL != null ? TrailerURL : " ",
            TicketURL = TicketURL != null ? TicketURL : " ",
            AgeRating = AgeRating != null || AgeRating.AgeRatingValue == "" ? AgeRating.AgeRatingValue : " ",
            Genre = GenresString,
            RunTime = _runTime,
            IsPremiere = IsPremiere,
            RunDate = _runDate
        };

        Response response = api.postProtected("/sendData", saveShowParams);

        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            await Application.Current.MainPage.DisplayAlert("Server Error", "There was an internal server error when sending the data, please try again later.", "OK");
            Loading = false;
            return;
        } else {

            Debug.WriteLine("Successfully added new film!");
            // Added a new film. /sendData returns the updated database collection for storing locally

            JObject responseJson = JObject.Parse(response.responseString);

            foreach (JProperty property in responseJson.Properties()) {
                Debug.WriteLine(string.Format("Writing {0} data to memory", property.Name));
                PreferencesStore.Delete(property.Name);
                PreferencesStore.Set(property.Name, responseJson.GetValue(property.Name));
            }

            Loading = false;
            await Task.Delay(20);

            bool result = await Application.Current.MainPage.DisplayAlert("Success!", "Successfully saved the show to the server. Do you want to clear all fields to start from scratch with another show?", "Yes", "No");

            if (result) {
                ResetAllValues();
            } else {
                return;
            }
        }

        void ResetAllValues() {
            FilmTypePickerIndex = -1;
            AgeRatingIndex = 0;
            LivePausesCount = 0;
            FilmTitle = Description = TrailerURL = TicketURL = PosterSource = string.Empty;
            IsPremiere = PosterImageVisible = false;
            RunTime = new DateTime(1, 1, 1, 1, 30, 0).TimeOfDay;
            RunDateTime = new DateTime(1, 1, 1, 18, 0, 0).TimeOfDay;
            RunDate = DateTime.Now;

            AgeRating = AgeRatingItems[AgeRatingIndex];
            GenresIndices = new List<int>();
            GenresString = "";

        }


    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}
