using FolketsHusApp.ViewModel;

namespace FolketsHusApp.Pages;

public partial class UnesPage : ContentPage {
    public UnesPage(UnesViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}