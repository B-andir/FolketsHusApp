using FolketsHusApp.ViewModel;
using System.Diagnostics;

namespace FolketsHusApp.Pages;

public partial class BioRosenFilmPage : ContentPage {

    BioRosenFilmViewModel viewModel;

    public BioRosenFilmPage(BioRosenFilmViewModel vm) {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e) {
        Debug.WriteLine("Refresh Content");

        if (viewModel.RefreshCommand.CanExecute(null))
            viewModel.RefreshCommand.Execute(null);
    }
}