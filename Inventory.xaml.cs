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

        public Inventory()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            database.scannedCode = entry.Text;

            database.scannerMethod(entry.Text);

            entry.Text = "";
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
            entry.Focus();
        }
    }
}
