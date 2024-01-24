using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class BioRosenQuickPage : ContentPage {
    public BioRosenQuickPage(BioRosenQuickViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}