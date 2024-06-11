namespace ISOMET_Dual_Tracking_System;

public partial class Traveler : ContentPage
{
    SQLAccess database = new SQLAccess();

    public Traveler()
	{
		InitializeComponent();
		Loaded += new EventHandler(Page_Loaded);
	}

	void OnButtonClicked(object sender, EventArgs e)
	{
		var selectedItem = picker.SelectedItem;
		string selectedValue = selectedItem?.ToString();
		database.scannedCode = identifierField.Text;

		database.travelerMethod(selectedValue, valueField.Text);

		identifierField.Text = "";
		picker.SelectedItem = null;
		valueField.Text = "";
	}

    private void Page_Loaded(object sender, EventArgs e)
    {
        identifierField.Focus();
    }
}