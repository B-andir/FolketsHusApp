using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class HomePage : ContentPage {
    public HomePage(HomeViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}