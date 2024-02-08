using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class FilmDetailPage : ContentPage {

    public FilmDetailPage(FilmDetailViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}