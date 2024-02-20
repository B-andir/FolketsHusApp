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

    private void age_rating_picker_Loaded(object sender, EventArgs e) {
        age_rating_picker.SelectedIndex = viewModel.AgeRatingIndex;
        age_rating_picker.SelectedItem = viewModel.AgeRating;
    }
}