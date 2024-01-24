using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class BioRosenLivePage : ContentPage {
    public BioRosenLivePage(BioRosenLiveViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}