namespace ISOMET_Dual_Tracking_System;

public partial class Reader : ContentPage
{
    SQLAccess database = new SQLAccess();

	int selectedDB;
	bool reportMode;

    public Reader()
	{
		InitializeComponent();
        this.Loaded += new EventHandler(OnLoaded);
    }

	private void OnLoaded(object sender, EventArgs e)
	{
		invButton.IsChecked = true;
		singleButton.IsChecked = true;
		reportMode = false;
		selectedDB = 1;
	}

	private void onSelectionStatusChanged(object sender, EventArgs e)
	{
        readerEditor.Text = "";
        if (invButton.IsChecked)
		{
			selectedDB = 1;
			trvButton.IsChecked = false;
			osButton.IsChecked = false;
		}
		else if (trvButton.IsChecked) {
			selectedDB = 2;
			invButton.IsChecked = false;
			osButton.IsChecked = false;
		}
		else if (osButton.IsChecked)
		{
			selectedDB = 3;
			invButton.IsChecked = false;
			trvButton.IsChecked = false;
		}
	}

	private void onReportModeChanged(object sender, EventArgs e)
	{
		if (singleButton.IsChecked)
		{
			reportMode = false;
			reportButton.IsChecked = false;
		}
		else
		{
			reportMode = true;
			singleButton.IsChecked = false;
		}
	}

    private async void searchBtnClicked(object sender, EventArgs e)
	{
        if (reportMode)
        {
			readerEditor.Text = "Movement History:\n";
            if (selectedDB == 1)
            {
				string message = await database.repInventory(identifier.Text);
				readerEditor.Text += message;
            }
            else if (selectedDB == 2)
            {
                string message = await database.repTraveler(identifier.Text);
                readerEditor.Text += message;
            }
            else if (selectedDB == 3)
            {
                string message = await database.repOS(identifier.Text);
                readerEditor.Text += message;
            }
        }
        else
		{
            readerEditor.Text = "";
            if (selectedDB == 1)
            {
                database.readInventory(identifier.Text);
            }
            else if (selectedDB == 2)
            {
                database.readTraveler(identifier.Text);
            }
            else if (selectedDB == 3)
            {
                database.readOScanning(identifier.Text);
            }
        }
	}
}