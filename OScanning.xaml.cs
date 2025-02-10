namespace ISOMET_Dual_Tracking_System;

using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;

public partial class OScanning : ContentPage
{
    SQLAccess database = new SQLAccess();

    public OScanning()
	{
		InitializeComponent();
		Loaded += new EventHandler(Page_Loaded);
	}

	async void OnButtonClicked(object sender, EventArgs e)
	{
		osBtn.IsEnabled = false;

		database.scannedCode = identifierField.Text;

		await database.oscanningMethod(waveField.Text, laserField1.Text, laserField2.Text, slField.Text);

		identifierField.Text = "";
		waveField.Text = "";
		laserField1.Text = "";
		laserField2.Text = "";
		slField.Text = "";

		osBtn.IsEnabled = true;
	}

    private void Page_Loaded(object sender, EventArgs e)
    {
        identifierField.Focus();
    }
}