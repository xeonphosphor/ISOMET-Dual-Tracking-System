using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Views;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private BroadcastReceiver _stopLockTaskReceiver;
    private BroadcastReceiver _startLockTaskReceiver;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set window flags
        Window.AddFlags(WindowManagerFlags.AllowLockWhileScreenOn);

        // Start the screen state service
        Intent serviceIntent = new Intent(this, typeof(ScreenStateService));
        StartService(serviceIntent);

        // Register the stop lock task receiver
        _stopLockTaskReceiver = new BroadcastReceiver();
        IntentFilter stopFilter = new IntentFilter("STOP_LOCK_TASK");
        RegisterReceiver(_stopLockTaskReceiver, stopFilter);

        // Register the start lock task receiver
        _startLockTaskReceiver = new BroadcastReceiver();
        IntentFilter startFilter = new IntentFilter("START_LOCK_TASK");
        RegisterReceiver(_startLockTaskReceiver, startFilter);

        // Start screen pinning on startup
        StartScreenPinning();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnregisterReceiver(_stopLockTaskReceiver);
        UnregisterReceiver(_startLockTaskReceiver);
    }

    private class BroadcastReceiver : Android.Content.BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (intent.Action == "STOP_LOCK_TASK")
                {
                    // Stop the lock task
                    ((Activity)context).StopLockTask();
                }
                else if (intent.Action == "START_LOCK_TASK")
                {
                    // Start the lock task
                    ((Activity)context).StartLockTask();
                }
            }
        }
    }

    private void StartScreenPinning()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            StartLockTask();
        }
    }

    public override void OnBackPressed()
    {
        // Optionally disable the back button
        // Do nothing
    }

    public void CloseApp()
    {
        FinishAffinity(); // Close all activities and exit the app
    }

    public void RestartApp()
    {
        var intent = new Intent(this, typeof(MainActivity));
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
        StartActivity(intent);
        FinishAffinity(); // Close all activities and exit the app
    }
}
