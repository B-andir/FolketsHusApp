using FolketsHusApp.ViewModel;
using Microsoft.Maui.Platform;
using System.Diagnostics;
using System.Reflection;

namespace FolketsHusApp.Pages;

public partial class LoginPage : ContentPage {

    ImageSource crossedEye, openedEye;

    public LoginPage(LoginViewModel vm) {
        InitializeComponent();
        BindingContext = vm;

        crossedEye = toggle_password_hidden_button.Source;
        openedEye = helper_image.Source;
    }

    private void username_entry_Completed(object sender, EventArgs e) {
        Debug.WriteLine("Completed");

        password_entry.Focus();
        password_entry.ShowSoftInputAsync(CancellationToken.None);
    }

    private void password_entry_Completed(object sender, EventArgs e) {
        Debug.WriteLine("Completed password");

        submit_button_Clicked(sender, e);


        //password_entry.Unfocus();

        //submit_button_Clicked(sender, e);
        //submit_button.Command.Execute(null);
    }

    private void submit_button_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("Button Clicked");

        if (password_entry.IsSoftInputShowing())
            password_entry.HideSoftInputAsync(CancellationToken.None);

        if (username_entry.IsSoftInputShowing())
            username_entry.HideSoftInputAsync(CancellationToken.None);

        LoginViewModel viewModel = (LoginViewModel)BindingContext;
        if (viewModel.SubmitCommand.CanExecute(null)) {
            Debug.WriteLine("Execute Command");
            viewModel.SubmitCommand.Execute(null);
        } else {
            Debug.WriteLine("Couldn't execute command, sorry");
        }
    }

    private void toggle_password_hidden_button_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("Toggle hide password");

        if (password_entry.IsPassword) {
            password_entry.IsPassword = false;
            toggle_password_hidden_button.Source = openedEye;
        } else {
            password_entry.IsPassword = true;
            toggle_password_hidden_button.Source = crossedEye;
        }
    }
}
