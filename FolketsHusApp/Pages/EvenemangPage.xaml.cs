using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class EvenemangPage : ContentPage {
    public EvenemangPage(EvenemangViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}