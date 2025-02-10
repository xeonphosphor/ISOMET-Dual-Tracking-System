using Android.Content;
using Android.OS;
using ISOMET_Dual_Tracking_System;

[BroadcastReceiver]
public class UserPresentBroadcastReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            // Send a broadcast to start the lock task
            Intent startLockTaskIntent = new Intent("START_LOCK_TASK");
            context.SendBroadcast(startLockTaskIntent);
        }
    }
}
