using FolketsHusApp.Models;
using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class FilmDetailPage : ContentPage {
    FilmDetailViewModel viewModel;

    public FilmDetailPage(IAPIService api, FilmObject filmObject) {
        InitializeComponent();
        BindingContext = viewModel = new FilmDetailViewModel(api, filmObject);
    }

    private void genre_picker_TextChanged(object sender, TextChangedEventArgs e) {
        viewModel.GenresString = genre_picker.Text;
    }
}