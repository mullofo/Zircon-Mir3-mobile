using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System.Threading;
using System.Threading.Tasks;

namespace Mir3.Droid
{
    [Activity(
       Label = "@string/app_name",
       MainLauncher = true,
       NoHistory = false,
       Icon = "@drawable/icon",
       AlwaysRetainTaskState = true,
       LaunchMode = LaunchMode.SingleInstance,
       ScreenOrientation = ScreenOrientation.SensorLandscape,
       ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
   )]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.launch);

            //var main = new Intent(this, typeof(MainActivity));
            //StartActivity(main);
            //Finish();
            Task.Run(() =>
            {
                var main = new Intent(this, typeof(MainActivity));
                Thread.Sleep(1000);
                RunOnUiThread(() =>
                {
                    StartActivity(main);
                    Finish();
                });
            });


        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            //禁用返回
            if (keyCode == Keycode.Back)
            {
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}

