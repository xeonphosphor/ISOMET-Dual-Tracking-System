using Microsoft.Maui.Devices.Sensors;
using System;
using System.Configuration;
using System.Data.OleDb;
using Microsoft.Data.Sqlite;

namespace ISOMET_Dual_Tracking_System
{
    public partial class Inventory : ContentPage
    {
        SQLAccess database = new SQLAccess();

        static string dptLocation = Preferences.Get("Location", string.Empty);

        public Inventory()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            ScannerBtn.IsEnabled = false; // Disable button to prevent duplicate entries

            database.scannedCode = entry.Text;

            await database.scannerMethod(entry.Text);

            entry.Text = "";

            ScannerBtn.IsEnabled = true; // Re-enable button after execution of await method is completed
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            string oldText = e.OldTextValue;
            string newText = e.NewTextValue;
            string currentText = entry.Text;
        }

        private void OnEntryCompleted(object sender, EventArgs e)
        {
            //database.scannedCode = entry.Text;

           // database.scannerMethod(entry.Text);
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            locationLbl.Text = dptLocation;
            entry.Focus();
        }
    }
}
