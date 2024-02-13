using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

internal class SaveShowParams {
    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("description")]
    public string Description { get; set; } = "";

    [JsonProperty("posterSource")]
    public string PosterSource { get; set; } = "";

    [JsonProperty("trailerURL")]
    public string TrailerURL { get; set; } = "";

    [JsonProperty("ticketURL")]
    public string TicketURL { get; set; } = "";

    [JsonProperty("ageRating")]
    public string AgeRating { get; set; } = "";

    [JsonProperty("genre")]
    public string Genre { get; set; } = "";

    [JsonProperty("runTime")]
    public int RunTime { get; set; }

    [JsonProperty("livePauses")]
    public int LivePauses { get; set; } = 0;

    [JsonProperty("isPremiere")]
    public bool IsPremiere { get; set; }

    [JsonProperty("runDate")]
    public DateTime RunDate { get; set; }

}

public partial class BioRosenQuickViewModel : ObservableObject {

    [ObservableProperty]
    int filmTypePickerIndex = -1, ageRatingIndex = 0, livePausesCount = 0;

    [ObservableProperty]
    string? filmTitle, description, trailerURL, ticketURL, posterSource;

    [ObservableProperty]
    bool isPremiere = false, posterImageVisible = false, loading = false;

    [ObservableProperty]
    TimeSpan? runTime, runDateTime;

    [ObservableProperty]
    DateTime? runDate;

    [ObservableProperty]
    public SelectAgeRatingItem? ageRating;

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


    [RelayCommand]
    async Task SaveShow() {
        Debug.WriteLine("Saving is not yet implemented!");
    }
}
