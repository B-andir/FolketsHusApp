using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class BioRosenKontrastPage : ContentPage {
    public BioRosenKontrastPage(BioRosenKontrastViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}