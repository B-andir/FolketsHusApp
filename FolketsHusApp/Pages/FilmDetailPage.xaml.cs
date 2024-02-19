using FolketsHusApp.Models;
using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class FilmDetailPage : ContentPage {

    public FilmDetailPage(FilmObject filmObject) {
        InitializeComponent();
        BindingContext = new FilmDetailViewModel(filmObject);
    }
}