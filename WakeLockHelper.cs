using Android.OS;
using Android.Content;

public class WakeLockHelper
{
    private PowerManager.WakeLock _wakeLock;

    public void AcquireWakeLock(Context context)
    {
        PowerManager powerManager = (PowerManager)context.GetSystemService(Context.PowerService);
        _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "MyApp::WakeLockTag");
        _wakeLock.Acquire();
    }

    public void ReleaseWakeLock()
    {
        _wakeLock?.Release();
    }
}
