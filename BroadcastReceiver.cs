using Android.App;
using Android.Content;
using Android.OS;
using ISOMET_Dual_Tracking_System;

[BroadcastReceiver]
public class ScreenOffBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            // Send a broadcast to stop the lock task
            Intent stopLockTaskIntent = new Intent("STOP_LOCK_TASK");
            context.SendBroadcast(stopLockTaskIntent);
        }
    }
}
