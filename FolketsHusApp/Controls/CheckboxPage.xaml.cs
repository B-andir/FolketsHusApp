using System.Diagnostics;

namespace FolketsHusApp.Controls;

public partial class CheckboxPage : BasePage<string> {

    private List<CheckedItem> ItemsSource;
    public CheckboxPage(List<string> itemsSource, List<int> selectedIndices) {
        InitializeComponent();

        List<CheckedItem> list = new List<CheckedItem>();
        for (int i = 0; i < itemsSource.Count; i++) {
            list.Add(new CheckedItem() {
                Text = itemsSource[i],
                IsChecked = false,
                Position = i,
            });
        }
        ItemsSource = list;

        foreach (int i in selectedIndices) {
            list[i].IsChecked = true;
        }

        list_view.ItemsSource = list;
    }

    private void list_view_ItemTapped(object sender, ItemTappedEventArgs e) {
        ((ListView)sender).SelectedItem = null;
    }

    async private void done_button_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("Done Clicked");
        List<CheckedItem> list = (List<CheckedItem>)list_view.ItemsSource;
        _navigationResult = string.Join(",", list.Where(x => x.IsChecked).Select(x => x.Position).ToArray());

        await Navigation.PopModalAsync();

    }

    async private void cancel_button_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("Cancel Clicked");
        _navigationResult = "";

        await Navigation.PopModalAsync();

    }
}

public class CheckedItem {
    public int Position { get; set; }
    public bool IsChecked { get; set; }
    public string? Text { get; set; }
}