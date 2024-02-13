using FolketsHusApp.ViewModel;
using System.Diagnostics;

namespace FolketsHusApp.Pages;

public partial class BioRosenQuickPage : ContentPage {
    public BioRosenQuickPage(BioRosenQuickViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }

    private void film_type_first_picker_SelectedIndexChanged(object sender, EventArgs e) {
        start_vertical_stack_layout.IsVisible = false;
        content_grid.IsVisible = true;
    }

    private void film_type_second_picker_SelectedIndexChanged(object sender, EventArgs e) {

        // Reset all possibly changed values

        live_time_and_pauses_container.IsVisible = false;
        normal_film_length_label.IsVisible = true;
        normal_film_length_time_picker.IsVisible = true;
        age_rating_label.IsVisible = true;
        age_rating_picker.IsVisible = true;
        ticket_url_label.IsVisible = true;
        ticket_url_entry.IsVisible = true;
        premiere_label.IsVisible = true;
        premiere_check_box.IsVisible = true;


        if (film_type_second_picker.SelectedIndex == 1) {
            // Live på Bio

            live_time_and_pauses_container.IsVisible = true;
            normal_film_length_label.IsVisible = false;
            normal_film_length_time_picker.IsVisible = false;

            age_rating_label.IsVisible = false;
            age_rating_picker.IsVisible = false;

            premiere_label.IsVisible = false;
            premiere_check_box.IsVisible = false;

        } else if (film_type_second_picker.SelectedIndex == 2) {
            // Bio Kontrast

            ticket_url_label.IsVisible = false;
            ticket_url_entry.IsVisible = false;

        }

    }
}
