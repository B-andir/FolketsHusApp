using Newtonsoft.Json.Linq;
using System.Globalization;

namespace FolketsHusApp.Models;

public class SelectAgeRatingItem {
    public string AgeRatingString { get; set; }
    public string AgeRatingValue { get; set; }

    public SelectAgeRatingItem(string s, string v) {
        AgeRatingString = s;
        AgeRatingValue = v;
    }

    public SelectAgeRatingItem(string s) {
        if (s == "" || s == " ") {
            AgeRatingString = "Ingen Åldersgräns";
            AgeRatingValue = "Ingen Åldersgräns";
        } else {
            AgeRatingString = s;
            AgeRatingValue = s;
        }
    }

    public bool Equals(SelectAgeRatingItem item) {
        return this.AgeRatingValue == item.AgeRatingValue && this.AgeRatingString == item.AgeRatingString;
    }
}

public class FilmObject {

    private string? _id;
    public string FilmName { get; set; }
    public string PosterSource { get; set; }
    public TimeSpan RunTime { get; set; }
    public string RuntimeHours { get; set; }
    public string RuntimeMinutes { get; set; }
    public string Description { get; set; }
    public DateTime RunDate { get; set; }
    public TimeSpan RunDateTime { get; set; }
    public string RunDateString { get; set; }
    public string TrailerURL { get; set; }
    public string TicketURL { get; set; }
    public string AgeRating { get; set; }
    public string Genre { get; set; }
    public bool IsPremiere { get; set; }


    public FilmObject(JToken filmToken) {
        _id = filmToken.Value<string>("_id");

        FilmName = filmToken.Value<string>("name") ?? "NaN";
        PosterSource = filmToken.Value<string>("posterURL") ?? "NaN";

        int runtime = filmToken.Value<int>("runtime");
        int hours = runtime / 60;
        int minutes = (runtime - hours * 60);

        RunTime = new TimeSpan(hours, minutes, 0);

        RuntimeHours = hours.ToString();
        RuntimeMinutes = minutes.ToString();

        Description = filmToken.Value<string>("description") ?? "NaN";

#pragma warning disable CS8604 // Possible null reference argument.
        RunDate = DateTime.Parse(filmToken.Value<string>("date"));
        RunDateString = RunDate.ToString("ddd dd/MM HH:mm", CultureInfo.GetCultureInfo("sv-SE"));
#pragma warning restore CS8604 // Possible null reference argument.

        RunDateTime = RunDate.TimeOfDay;

        TrailerURL = filmToken.Value<string>("trailerURL") ?? "NaN";
        TicketURL = filmToken.Value<string>("ticketURL") ?? "NaN";

        AgeRating = filmToken.Value<string>("ageRating") ?? "NaN";

        Genre = filmToken.Value<string>("genre") ?? "NaN";
        IsPremiere = filmToken.Value<bool>("premiere") || false;
    }

}
