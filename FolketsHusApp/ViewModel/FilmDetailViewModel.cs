using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using System.ComponentModel;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

[QueryProperty(nameof(FilmObject), "FilmObject")]
public partial class FilmDetailViewModel : ObservableObject, IQueryAttributable, INotifyPropertyChanged {

    public FilmObject? filmObject { get; private set; }

    [ObservableProperty]
    public string? filmTitle, oGFilmTitle, posterSource, newPosterSource, runtimeHours, runtimeMinutes, description, trailerURL, ticketURL, genre;

    [ObservableProperty]
    public SelectAgeRatingItem? ageRating;

    [ObservableProperty]
    public int ageRatingIndex = 0;

    [ObservableProperty]
    public DateTime runDate;

    [ObservableProperty]
    public TimeSpan runTime, runDateTime;

    [ObservableProperty]
    public bool isPremiere;

    [ObservableProperty]
    public List<SelectAgeRatingItem> ageRatingItems = new() {
        new SelectAgeRatingItem("Ingen Åldersgräns", ""),
        new SelectAgeRatingItem("7 år", "7 år"),
        new SelectAgeRatingItem("11 år", "11 år"),
        new SelectAgeRatingItem("15 år", "15 år")
    };

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        filmObject = query["FilmObject"] as FilmObject;
        OnPropertyChanged("FilmObject");

        if (filmObject != null) {
            FilmTitle = filmObject.FilmName;
            OGFilmTitle = FilmTitle;
            PosterSource = filmObject.PosterSource;
            NewPosterSource = "";
            RuntimeHours = filmObject.RuntimeHours;
            RuntimeMinutes = filmObject.RuntimeMinutes;
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

            Genre = filmObject.Genre;
            RunDate = filmObject.RunDate;

            Debug.WriteLine(filmObject.RunDateTime);

            RunDateTime = filmObject.RunDateTime;

            IsPremiere = filmObject.IsPremiere;
        }

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

    [RelayCommand]
    async Task Delete() {
        bool result = await Shell.Current.DisplayAlert("Delete Film", "Are you sure you want to delete this film? This action cannot be undone.", "Yes, Delete It", "Cancel");

        if (result) {
            Debug.WriteLine("DELETING");
        } else {
            Debug.WriteLine("Deletion canceled");
        }
    }

    [RelayCommand]
    void Save() {

    }
}
