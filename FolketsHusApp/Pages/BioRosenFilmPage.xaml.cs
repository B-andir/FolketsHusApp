using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class BioRosenFilmPage : ContentPage {
    public BioRosenFilmPage(BioRosenFilmViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}