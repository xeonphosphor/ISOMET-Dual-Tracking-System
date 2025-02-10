namespace ISOMET_Dual_Tracking_System;

public partial class Reader : ContentPage
{
    SQLAccess database = new SQLAccess();

	int selectedDB;
	bool reportMode;
    bool containsDash;

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

    private void onTextChanged(object sender, EventArgs e) // The way the report data is displayed would create too much data to be readable, therefore limiting to just "last location" solves that.
    {
        string identifierText = identifier.Text;

        if (identifierText.Contains("-"))
        {
            if (selectedDB != 1) // Ensure only Inventory mode is displayed and enabled with a dash
            {
                selectedDB = 1;
                invButton.IsChecked = true;
                trvButton.IsChecked = false;
                osButton.IsChecked = false;
            }

            if (reportMode)
            {
                reportMode = false;

                reportButton.IsChecked = false;
                reportButton.IsEnabled = false;
            }
            else
            {
                reportButton.IsEnabled = false;
            }

            containsDash = true;
            trvButton.IsEnabled = false;
            osButton.IsEnabled = false;
        }
        else
        {
            if (containsDash)
            {
                containsDash = false;
                reportButton.IsEnabled = true;
                trvButton.IsEnabled = true;
                osButton.IsEnabled = true;
            }
        }
    }

    private async void searchBtnClicked(object sender, EventArgs e)
	{
		searchBtn.IsEnabled = false;

		string identifierText = identifier.Text;

		if (identifierText.Contains("-"))
		{
			string[] range = identifierText.Split('-');

			if (range.Length == 2 && int.TryParse(range[0], out int startRange) && int.TryParse(range[1], out int endRange))
			{
                readerEditor.Text = "Last Locations:\n"; ;
                if (selectedDB == 1)
                {
                    string message = await database.repInventoryRange(startRange, endRange);
                    readerEditor.Text += message;
                }
            }
		} 
		else
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
                    await database.readInventory(identifier.Text);
                }
                else if (selectedDB == 2)
                {
                    await database.readTraveler(identifier.Text);
                }
                else if (selectedDB == 3)
                {
                    await database.readOScanning(identifier.Text);
                }
            }
        }

		searchBtn.IsEnabled = true;
	}
}