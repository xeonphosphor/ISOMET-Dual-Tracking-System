using Microsoft.Maui.Storage;

namespace ISOMET_Dual_Tracking_System;

public partial class Configuration : ContentPage
{
	ConfigClass config = new ConfigClass();

    bool root;

	public Configuration()
	{
		InitializeComponent();
		this.Loaded += new EventHandler(OnLoaded);
	}

	private void changeConfig()
	{
        config.serverName = netServer.Text;
        //config.networkName = netUserName.Text;
        //config.networkPassword = netPassword.Text;
		config.database1 = directory.Text;
        config.database2 = directory2.Text;
        config.database3 = directory3.Text;
        config.name = userName.Text;
        config.location = department.Text;

        config.updateConfig();
    }

    private void displayConfigChanges()
    {
        netServer.Text = Preferences.Get("Server", string.Empty);
        //netUserName.Text = Preferences.Get("NetworkName", string.Empty);
        //netPassword.Text = Preferences.Get("NetworkPassword", string.Empty);
        directory.Text = Preferences.Get("Database", string.Empty);
        directory2.Text = Preferences.Get("Database2", string.Empty);
        directory3.Text = Preferences.Get("Database3", string.Empty);
        userName.Text = Preferences.Get("Name", string.Empty);
        department.Text = Preferences.Get("Location", string.Empty);
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        displayConfigChanges();
    }

    private async void OnButtonClicked(object sender, EventArgs e)
	{
		changeConfig();
        displayConfigChanges();
        await DisplayAlert("Configuration", "Successfully applied configuration changes, please restart application.", "OK");
    }
}