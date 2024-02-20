using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FolketsHusApp.ViewModel;

public partial class FilmDetailViewModel : ObservableObject {

    private IAPIService api;

    public FilmObject filmObject { get; private set; }

    [ObservableProperty]
    public string? filmTitle, oGFilmTitle, posterSource, newPosterSource, description, trailerURL, ticketURL;

    [ObservableProperty]
    public SelectAgeRatingItem? ageRating;

    [ObservableProperty]
    public int ageRatingIndex = 0;

    [ObservableProperty]
    public DateTime runDate;

    [ObservableProperty]
    public TimeSpan runTime, runDateTime;

    [ObservableProperty]
    public bool isPremiere, loading = false;

    [ObservableProperty]
    public List<SelectAgeRatingItem> ageRatingItems = new() {
        new SelectAgeRatingItem("Ingen Åldersgräns", ""),
        new SelectAgeRatingItem("7 år", "7 år"),
        new SelectAgeRatingItem("11 år", "11 år"),
        new SelectAgeRatingItem("15 år", "15 år")
    };

    [ObservableProperty]
    public List<string> genresItems = new List<string>() { "Action", "Adventure", "Animation", "Comedy", "Crime", "Documentary", "Drama", "Family", "Fantasy", "Horror", "Musical", "Mystery", "Romance", "Science Fiction", "Thriller", "War", "Western" };

    [ObservableProperty]
    public List<int> genresIndices = new List<int>();

    public string GenresString = "";

    public FilmDetailViewModel(IAPIService api, FilmObject filmObject) {
        this.filmObject = filmObject;
        this.api = api;

        FilmTitle = filmObject.FilmName;
        OGFilmTitle = FilmTitle;
        PosterSource = filmObject.PosterSource;
        NewPosterSource = "";
        RunTime = filmObject.RunTime;

        string description = filmObject.Description;
        string newDescription = description.Replace("<br>", "\n");

        Description = newDescription;
        TrailerURL = filmObject.TrailerURL;
        TicketURL = filmObject.TicketURL;
        SelectAgeRatingItem _ageRating = new SelectAgeRatingItem(filmObject.AgeRating);

        for (int i = 0; i < AgeRatingItems.Count; i++) {
            if (_ageRating.Equals(AgeRatingItems[i])) {
                AgeRatingIndex = i;
                AgeRating = AgeRatingItems[i];
            }
        }

        string genres = filmObject.Genre;

        List<string> genresList = genres.Split(", ").ToList();

        foreach (string genre in genresList) {
            int index = GenresItems.IndexOf(genre);
            if (index != -1)  // List<T>.IndexOf(x) returns -1 if it couldn't find x in list
                GenresIndices.Add(index);
        }

        RunDate = filmObject.RunDate;

        Debug.WriteLine(filmObject.RunDateTime);

        RunDateTime = filmObject.RunDateTime;

        IsPremiere = filmObject.IsPremiere;
    }

    [RelayCommand]
    async Task SelectImage() {
        if (MediaPicker.Default.IsCaptureSupported) {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null) {
                // save the file into local storage
                //string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                //using Stream sourceStream = await photo.OpenReadAsync();
                //using FileStream localFileSteam = File.OpenWrite(localFilePath);

                //await sourceStream.CopyToAsync(localFileSteam);

                //sourceStream.Dispose();

                NewPosterSource = photo.FullPath;
            }
        }
    }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
    [RelayCommand]
    async Task Delete() {
        bool result = await Shell.Current.DisplayAlert("Delete Film", "Are you sure you want to delete this film? This action cannot be undone.", "Yes, Delete It", "Cancel");

        if (result) {
            Debug.WriteLine("DELETING");
            Loading = true;
            await Task.Delay(10);

            DeleteElementParams deleteElementParams = new DeleteElementParams {
                ElementType = filmObject.ShowType,
                Id = filmObject._id ?? ""
            };

            Response response = api.postProtected("/sendData", deleteElementParams);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                await Application.Current.MainPage.DisplayAlert("Error", response.responseString, "OK");
                Loading = false;
                return;
            } else {

                JObject responseJson = JObject.Parse(response.responseString);

                foreach (JProperty property in responseJson.Properties()) {
                    Debug.WriteLine(string.Format("Writing {0} data to memory", property.Name));
                    PreferencesStore.Delete(property.Name);
                    PreferencesStore.Set(property.Name, responseJson.GetValue(property.Name));
                }

                Loading = false;
                await Task.Delay(10);

                await Back();
                return;
            }
        } else {
            Debug.WriteLine("Deletion canceled");
        }
    }

    [RelayCommand]
    async Task Save() {

        bool result = await Application.Current.MainPage.DisplayAlert("Warning!", "You are about to overwrite data on the database. This action cannot be undone.\nAre you sure you want to proceed?", "Yes - Proceed", "No - Cancel");

        if (!result)
            return;

        Loading = true;
        await Task.Delay(10);

        DateTime _runDate = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day, RunDateTime.Hours, RunDateTime.Minutes, 0);

        string _description = Description != null ? Description.Replace("\n", "<br>") : filmObject.Description;

        int _runTime = (int)RunTime.TotalMinutes;

        string posterURL = filmObject.PosterSource;
        string posterCloudinaryId = "NaN";

        if (NewPosterSource != null && NewPosterSource != "") {
            Account account = new Account(
                    await SecureStorage.GetAsync("cloudinaryAPI_cloud_name"),
                    await SecureStorage.GetAsync("cloudinaryAPI_api_key"),
                    await SecureStorage.GetAsync("cloudinaryAPI_api_secret"));

            Cloudinary cloudinary = new Cloudinary(account);

            string publicIdName = FilmTitle != null ? FilmTitle.Replace(' ', '_') : filmObject.FilmName.Replace(' ', '_');

            ImageUploadParams uploadParams = new ImageUploadParams {
                File = new FileDescription(NewPosterSource),
                Folder = string.Format("FolketsHusHallstavik/Posters/{0}", filmObject.ShowType),
                PublicId = string.Format("{0}_poster_{1}", publicIdName, Guid.NewGuid()),
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) {
                await Application.Current.MainPage.DisplayAlert("Error", string.Format("There was an issue uploading the image. Error message:\n{0}", uploadResult.Error.Message), "OK");
                Loading = false;
                return;
            } else {
                posterURL = uploadResult.SecureUrl.ToString();
                posterCloudinaryId = uploadResult.PublicId;
            }
        }

        SaveShowParams updateShowParams = new SaveShowParams {
            DataType = "BioRosenUpdate",
            ShowType = filmObject.ShowType,
            _id = filmObject._id ?? "NaN",
            Title = FilmTitle ?? filmObject.FilmName,
            Description = _description,
            PosterCloudinaryId = posterCloudinaryId,
            PosterURL = posterURL,
            TrailerURL = TrailerURL ?? filmObject.TrailerURL,
            TicketURL = TicketURL ?? filmObject.TicketURL,
            AgeRating = AgeRating != null ? AgeRating.AgeRatingValue != "" ? AgeRating.AgeRatingValue : " " : " ",
            Genre = GenresString,
            RunTime = _runTime,
            IsPremiere = IsPremiere,
            RunDate = _runDate
        };

        Response response = api.postProtected("/sendData", updateShowParams);

        if (response.StatusCode != System.Net.HttpStatusCode.OK) {
            await Application.Current.MainPage.DisplayAlert("Server Error", "There was an internal server error when sending the data, please try again later.", "OK");
            Loading = false;
            return;
        } else {

            Debug.WriteLine("Successfully updated show!");

            JObject responseJson = JObject.Parse(response.responseString);

            foreach (JProperty property in responseJson.Properties()) {
                Debug.WriteLine(string.Format("Writing {0} data to memory", property.Name));
                PreferencesStore.Delete(property.Name);
                PreferencesStore.Set(property.Name, responseJson.GetValue(property.Name));
            }

            Loading = false;
            await Back();
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

    [RelayCommand]
    async Task Back() {
        if (Application.Current != null && Application.Current.MainPage != null)
            await Application.Current.MainPage.Navigation.PopModalAsync();

        return;
    }
}
