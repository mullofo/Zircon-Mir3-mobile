using System;
using System.Linq;
using System.Runtime.InteropServices;
using Client;
using Client.Controllers;
using Client.Envir;
using Foundation;
using Mir3.Mobile;
using UIKit;
using Xamarin.iOS;

namespace Mir3.iOS
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate, IUI
    {
        private static Game1 game;

        public static Program Activity;
        UIViewController _mainController;
        UIViewController _currentController;
        internal static void RunGame()
        {

            Game1.Native = new IosNative();
            game = new Game1();

            //game.Window.Title = "Lengend Of Mir3";
            game.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(Program));
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RegisterNotifier();
            Activity = this;
            RunGame();
            InitView();
        }

        public int Height { get; set; }

        private void InitView()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
            {
                var window = UIApplication.SharedApplication.ConnectedScenes
                    .OfType<UIWindowScene>()
                    .SelectMany(s => s.Windows)
                    .FirstOrDefault(w => w.IsKeyWindow);
                _mainController = window.RootViewController;
            }
            else
            {
                _mainController = UIApplication.SharedApplication.Windows[0].RootViewController;
            }

            Height = (int)_mainController.View.Bounds.Height;
        }
        

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Finish();
        }

        #region 监听
        /// <summary>
        /// 注册网络变化监听、手机旋转监听
        /// </summary>
        private void RegisterNotifier()
        {

            //手机网络监听
            Reachability.Init();
            Reachability.ReachabilityChanged = (flags) =>
            {
                if (!Reachability.IsNetworkAvailable())
                {
                    ShowMsg("无法访问网络！");
                }

            };

            //电量监听
            UIDevice.CurrentDevice.BatteryMonitoringEnabled = true;
            CEnvir.BatteryLevel = (int)(UIDevice.CurrentDevice.BatteryLevel * 100);
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceBatteryLevelDidChangeNotification"), (s) => {
                CEnvir.BatteryLevel = (int)(UIDevice.CurrentDevice.BatteryLevel * 100);
            });

            if (IsAllScreen)
            {
                ScreenOffset = new Microsoft.Xna.Framework.Point(0, 40);
                //旋转监听
                NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), (s) => {
                    var orientation = UIDevice.CurrentDevice.Orientation;
                    if (orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight)
                    {
                        ScreenOffset = (orientation == UIDeviceOrientation.LandscapeLeft) ?
                           new Microsoft.Xna.Framework.Point(40, 0) :
                           new Microsoft.Xna.Framework.Point(0, 40);

                        Client.Scenes.GameScene.Game?.ChangeLandscape();
                    }
                });
            }

        }
        #endregion

        #region ui impl
        public string Pwd
        {
            get
            {
                return Config.RememberedPassword;
            }
            set
            {
                Config.RememberedPassword = value;
                if (!ShowLayout) return;
                LoginController.Password.Value = value;
            }
        }
        public string Account
        {
            get
            {
                return Config.RememberedEMail;
            }
            set
            {
                Config.RememberedEMail = value;
                if (!ShowLayout) return;
                LoginController.Account.Value = value;
            }
        }

        public string CPwd
        {
            get
            {
                return CreateController.Password?.Value;
            }
            set
            {
                if (!(_currentController is CreateController)) return;
                CreateController.Password.Value = value;
            }
        }
        public string CAccount
        {
            get
            {
                return CreateController.Account?.Value;
            }
            set
            {
                if (!(_currentController is CreateController)) return;
                CreateController.Account.Value = value;
            }
        }

        private bool _showLayout = false;
        public bool ShowLayout
        {
            get
            {
                return _showLayout;
            }
            set
            {
                if (value == _showLayout && _currentController is LoginController) return;
                _showLayout = value;
                if(value && _currentController is not LoginController) {
                    Push(new LoginController(this));
                }
                LoginController.Show(value);
            }
        }

        public Microsoft.Xna.Framework.Point ScreenOffset { get; set; }

        public bool IsAllScreen
        {
            get
            {
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                    return true;

                var model = Xamarin.iOS.DeviceHardware.Model;
                var type = Xamarin.iOS.DeviceHardware.ChipType;
             
                //iPhone X 及以上 全面屏
                return type >= iOSChipType.A11Bionic && model.IndexOf("iPhone 8") == -1 || DeviceHardware.IsSimulator;
            }
        }
        public void HidePopWindow()
        {
            CloseDialog(null);
        }
        public void CloseDialog(Action? action = null)
        {
            _currentController?.DismissViewController(false, action);
            _currentController = null;
        }

        static DateTime? _lastTime;
        public void Exit()
        {
            if (!_lastTime.HasValue || (DateTime.Now - _lastTime.Value) > new TimeSpan(0, 0, 2))
            {
                ShowMsg("再按一次退出程序");
                _lastTime = DateTime.Now;
                return;
            }
            else
            {
                Finish();//结束程序
            }
        }

        public void ShowMsg(string msg)
        {
            GlobalToast.Toast.MakeToast(msg).SetPosition(GlobalToast.ToastPosition.Bottom).Show();
        }

        public void Finish()
        {
            game.Dispose();
            Exit(0);
        }

        /// <summary>
        /// 退出应用
        /// </summary>
        /// <param name="status"></param>
        [DllImport("__Internal", EntryPoint = "exit")]
        public static extern void Exit(int status);

        public void Push( CocoaUiBinding.ITable controller)
        {
            if (_currentController == null)
            {
                _currentController = controller;
                _mainController.PresentViewController(_currentController, false, null);
                return;
            }

            if (_currentController.GetType() == controller.GetType())
            {
                return;
            }
            CloseDialog(() => {
                _currentController = controller;
                _mainController.PresentViewController(_currentController, false, null);
            });
        }

        public void BeginInvoke(Action action)
        {
            InvokeOnMainThread(action);
        }
        #endregion

    }
}