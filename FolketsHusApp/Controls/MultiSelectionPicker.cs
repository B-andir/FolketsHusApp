namespace FolketsHusApp.Controls;

class MultiSelectionPicker : Entry {

    public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", typeof(string), typeof(MultiSelectionPicker), null);
    public string Title {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(List<string>), typeof(MultiSelectionPicker), null, BindingMode.TwoWay);
    public List<string> ItemsSource {
        get { return (List<string>)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly BindableProperty SelectedIndicesProperty = BindableProperty.Create("SelectedIndices", typeof(List<int>), typeof(MultiSelectionPicker), null, BindingMode.TwoWay, propertyChanged: SelectedIndicesChanged);
    public List<int> SelectedIndices {
        get { return (List<int>)GetValue(SelectedIndicesProperty); }
        set { SetValue(SelectedIndicesProperty, value); }
    }

    private static void SelectedIndicesChanged(BindableObject bindable, object oldValue, object newValue) {
        var ctrl = (MultiSelectionPicker)bindable;
        if (ctrl == null)
            return;

        List<string> selectedItems = new List<string>();
        foreach (int i in ctrl.SelectedIndices) {
            selectedItems.Add(ctrl.ItemsSource[i]);
        }
        ctrl.Text = string.Join(", ", selectedItems);
    }


    public async Task<T> NavigateToModal<T>(BasePage<T> page) {
        var source = new TaskCompletionSource<T>();
        page.PageDisappearing += (result) => {
            var res = (T)Convert.ChangeType(result, typeof(T));
#pragma warning disable CS8604 // Possible null reference argument.
            source.SetResult(res);
#pragma warning restore CS8604 // Possible null reference argument.
        };

        await Navigation.PushModalAsync(new NavigationPage(page), true);

        return await source.Task;
    }

    public MultiSelectionPicker() {
        Focused += async (e, s) => {

            if (s.IsFocused) {
                Unfocus();
                string item = await NavigateToModal<string>(new CheckboxPage(ItemsSource, SelectedIndices));
                if (item == "")
                    return;

                SelectedIndices = item.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                List<string> selectedItems = new List<string>();
                foreach (int i in SelectedIndices) {
                    selectedItems.Add(ItemsSource[i]);
                }

                Text = string.Join(", ", selectedItems);
            }
        };


    }
}

public class BasePage<T> : ContentPage {
    public event Action<T> PageDisappearing;
    protected T _navigationResult;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public BasePage() {

    }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override void OnDisappearing() {
        base.OnDisappearing();

        PageDisappearing?.Invoke(_navigationResult);

        if (PageDisappearing != null) {
            foreach (var @delegate in PageDisappearing.GetInvocationList()) {
                PageDisappearing -= @delegate as Action<T>;
            }
        }

    }
}