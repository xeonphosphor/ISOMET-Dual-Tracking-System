namespace ISOMET_Dual_Tracking_System;

public partial class Traveler : ContentPage
{
    SQLAccess database = new SQLAccess();

	bool sliceMode;
    string status;

    public Traveler()
	{
		InitializeComponent();
		Loaded += new EventHandler(Page_Loaded);
	}

	void onIDChanged(object sender, EventArgs e)
	{
		if (identifierField.Text.Length >= 1)
		{
            yesSlice.IsEnabled = true;
            noSlice.IsEnabled = true;
        }
		else
		{
            yesSlice.IsEnabled = false;
            noSlice.IsEnabled = false;
        }
	}

    async void OnButtonClicked(object sender, EventArgs e)
	{
        travelerBtn.IsEnabled = false;

        var selectedItem = picker.SelectedItem;
        var selectedValue = selectedItem?.ToString();

        if (sliceMode)
        {
            var slices = new[] { slice1, slice2, slice3, slice4, slice5, slice6, slice7 };

            for (int i = 0; i < slices.Length; i++)
            {
                database.scannedCode = slices[i].Text;

                slices[i].IsVisible = i < sliceNum.Value;

                status = await database.slicesTravelerMethod(selectedValue, valueField.Text);
            }
            await App.Current.MainPage.DisplayAlert("Traveler", status, "OK");
        }
        else
        {
            database.scannedCode = identifierField.Text;

            database.travelerMethod(selectedValue, valueField.Text);
        }

        travelerBtn.IsEnabled = true;
	}

	void onSliceModeChanged(object sender, EventArgs e)
	{
		if (yesSlice.IsChecked)
		{
            identifierField.IsReadOnly = true;

			sliceMode = true;
			sliceNum.IsEnabled = true;
			sliceNum.Value = 1;
			slice1.IsVisible = true;

			slice1.Text = identifierField.Text + "A";
		}
        else
        {
            identifierField.IsReadOnly = false;

            sliceMode = false;
            sliceNum.Value = 1;
            sliceNum.IsEnabled = false;
        }
    }

	void sliceNumChanged(object sender, EventArgs e)
	{
        var slices = new[] { slice1, slice2, slice3, slice4, slice5, slice6, slice7 };
        var letters = new[] { "A", "B", "C", "D", "E", "F", "G" };

        if (sliceMode)
		{
            for (int i = 0; i < slices.Length; i++)
            {
                slices[i].IsVisible = i < sliceNum.Value;

                if (slices[i].Text != identifierField.Text + letters[i])
                {
                    slices[i].Text = identifierField.Text + letters[i];
                }
            }
        }  
		else
		{
            for (int i = 0; i < slices.Length; i++)
            {
                slices[i].IsVisible = i < sliceNum.Value;
            }

            slice1.IsVisible = false;
        }
    }

    private void Page_Loaded(object sender, EventArgs e)
    {
        identifierField.Focus();
        identifierField.Text = "";

        sliceMode = false;

        slice1.IsVisible = false;
        slice2.IsVisible = false;
        slice3.IsVisible = false;
        slice4.IsVisible = false;
        slice5.IsVisible = false;
        slice6.IsVisible = false;
        slice7.IsVisible = false;

        yesSlice.IsChecked = false;
        noSlice.IsChecked = true;

        yesSlice.IsEnabled = false;
        noSlice.IsEnabled = false;

        sliceNum.IsEnabled = false;
    }
}