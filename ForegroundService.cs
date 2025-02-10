using Android.App;
using Android.Content;
using Android.OS;

[Service]
public class ScreenStateService : Service
{
    private WakeLockHelper _wakeLockHelper;
    private ScreenOffBroadcastReceiver _screenOffReceiver;
    private UserPresentBroadcastReceiver _userPresentReceiver;

    public override void OnCreate()
    {
        base.OnCreate();
        _wakeLockHelper = new WakeLockHelper();
        _wakeLockHelper.AcquireWakeLock(this);

        // Register the screen off receiver
        _screenOffReceiver = new ScreenOffBroadcastReceiver();
        IntentFilter screenOffFilter = new IntentFilter(Intent.ActionScreenOff);
        RegisterReceiver(_screenOffReceiver, screenOffFilter);

        // Register the user present receiver
        _userPresentReceiver = new UserPresentBroadcastReceiver();
        IntentFilter userPresentFilter = new IntentFilter(Intent.ActionUserPresent);
        RegisterReceiver(_userPresentReceiver, userPresentFilter);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _wakeLockHelper.ReleaseWakeLock();
        UnregisterReceiver(_screenOffReceiver);
        UnregisterReceiver(_userPresentReceiver);
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }
}