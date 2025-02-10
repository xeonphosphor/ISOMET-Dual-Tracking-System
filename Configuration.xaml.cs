using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using Android.Content;
using Android.App;
using Android.OS;
using AndroidX.Core.Content;
using FileProvider = AndroidX.Core.Content.FileProvider;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace ISOMET_Dual_Tracking_System;

public partial class Configuration : ContentPage
{
	ConfigClass config = new ConfigClass();

    bool root;

    static HttpClientHandler handler;
    static HttpClient client;

    public Configuration()
	{
        InitializeComponent();
		this.Loaded += new EventHandler(OnLoaded);

        handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
        client = new HttpClient(handler);
        client.Timeout = TimeSpan.FromSeconds(30);
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

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            string server = Preferences.Get("Server", string.Empty);
            string serverUrl = $"https://{server}/updates";
            string currentVersion = version.Text;

            var response = await client.GetAsync(serverUrl + "/version.txt");
            if (response.IsSuccessStatusCode)
            {
                var latestVersion = await response.Content.ReadAsStringAsync();     
                if (IsNewerVersion(currentVersion, latestVersion))
                {
                    bool confirmation = await DisplayAlert("Update Checker", $"Update found! Install update? New version: {latestVersion}", "Yes", "No");
                    if (confirmation)
                    {
                        var activity = (Activity)Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                        activity.StopLockTask();

                        await DownloadAndUpdateApk(serverUrl + "/com.isomet.tracking.apk");
                    }
                    else
                    {
                        Console.WriteLine("User cancelled update.");
                    }
                }
                else
                {
                    await DisplayAlert("Update Checker", $"Your app is up to date!", "OK");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("Update Checker", $"Request error: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Update Checker", $"An error has occurred: {ex.Message}", "OK");
        }
    }

    private async Task DownloadAndUpdateApk(string apkUrl)
    {
        try
        {
            var apkBytes = await client.GetByteArrayAsync(apkUrl);

            var filePath = Path.Combine(FileSystem.CacheDirectory, "com.isomet.tracking.apk");
            File.WriteAllBytes(filePath, apkBytes);

            var file = new Java.IO.File(filePath);
            var uri = FileProvider.GetUriForFile(Android.App.Application.Context, $"{Android.App.Application.Context.PackageName}.provider", file);

            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "application/vnd.android.package-archive");
            intent.SetFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            Android.App.Application.Context.StartActivity(intent);
        }
        catch (HttpRequestException ex)
        {
            await DisplayAlert("Update Checker", $"Request error: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Update Checker", $"An error has occurred: {ex.Message}", "OK");
        }
    }


    private bool IsNewerVersion(string currentVersion, string latestVersion)
    {
        if (Version.TryParse(currentVersion, out var current) && Version.TryParse(latestVersion, out var latest))
        {
            return latest > current;
        }
        return false;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
        displayConfigChanges();
    }

    private async void OnUpdateButtonClicked(object sender, EventArgs e)
    {
        updateBtn.IsEnabled = false;

        await CheckForUpdatesAsync();

        updateBtn.IsEnabled = true;
    }

    private async void OnButtonClicked(object sender, EventArgs e)
	{
        configBtn.IsEnabled = false;

		changeConfig();
        displayConfigChanges();

        bool restart = await DisplayAlert("Configuration", "Successfully applied configuration changes, please restart application.", "Restart", "Cancel");

        configBtn.IsEnabled = true;

        if (restart)
        {
            var activity = (Activity)Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
            activity.StopLockTask();
            activity.FinishAffinity();
        }
        else
        {
            Console.WriteLine("User declined restart.");
        }
    }
}