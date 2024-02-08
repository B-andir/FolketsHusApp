using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolketsHusApp.Models;
using FolketsHusApp.Pages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FolketsHusApp.ViewModel;

public partial class BioRosenFilmViewModel : ObservableObject {

    [ObservableProperty]
    public ObservableCollection<FilmObject> filmsObjects;

    [ObservableProperty]
    public bool isSwipeViewEnabled, noObjects;

    public BioRosenFilmViewModel() {
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
    void Delete(FilmObject obj) {
        Debug.WriteLine(obj.FilmName);
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
