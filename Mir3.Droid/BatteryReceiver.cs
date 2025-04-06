using Android.Content;
using Client.Envir;

namespace Mir3.Droid
{
    internal class BatteryReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if ("android.intent.action.BATTERY_CHANGED".Equals(intent.Action))
            {
                CEnvir.BatteryLevel = intent.GetIntExtra("level", 0);
                CEnvir.BatteryScale = intent.GetIntExtra("scale", 0);
            }
        }
    }
}
