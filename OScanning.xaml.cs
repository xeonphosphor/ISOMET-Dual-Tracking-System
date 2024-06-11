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

	void OnButtonClicked(object sender, EventArgs e)
	{
		database.scannedCode = identifierField.Text;

		database.oscanningMethod(waveField.Text, laserField1.Text, laserField2.Text, slField.Text);

		identifierField.Text = "";
		waveField.Text = "";
		laserField1.Text = "";
		laserField2.Text = "";
		slField.Text = "";
	}

    private void Page_Loaded(object sender, EventArgs e)
    {
        identifierField.Focus();
    }
}