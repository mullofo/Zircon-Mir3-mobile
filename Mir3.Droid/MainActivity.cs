using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Client.Controls;
using Client.Envir;
using Com.Aigestudio.Wheelpicker;
using Microsoft.Xna.Framework;
using Mir3.Mobile;
using System;
using Uri = Android.Net.Uri;

namespace Mir3.Droid
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = false,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public partial class MainActivity : AndroidGameActivity, IUI
    {
        public static MainActivity Main { get; set; }
        public Game1 _game;
        private static View _view;
        private static EditText editText;
        //private static WebView webView;
        private static RelativeLayout.LayoutParams layoutParams;
        private static InputMethodManager imm;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //注册未处理异常事件
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            Main = this;
            RequestWindowFeature(WindowFeatures.NoTitle);
            HideNavBars();
            FullScreen();
            Game1.Native = new DrodNative();
            _game = new Game1();

            InitLayout();

            imm = (InputMethodManager)GetSystemService("input_method");
            Window!.SetSoftInputMode(SoftInput.StateAlwaysHidden | SoftInput.AdjustResize);
            IntentFilter filter = new IntentFilter("android.intent.action.BATTERY_CHANGED");
            BatteryReceiver receiver = new BatteryReceiver();
            RegisterReceiver(receiver, filter);
            SetUpInputField();

            _game.Window.OrientationChanged += (o, e) =>
            {
                var orientation = _game.Window.CurrentOrientation;
                if (orientation == DisplayOrientation.LandscapeLeft || orientation == DisplayOrientation.LandscapeRight)
                {
                    ScreenOffset = (orientation == DisplayOrientation.LandscapeLeft) ?
                       new Microsoft.Xna.Framework.Point(40, 0) :
                       new Microsoft.Xna.Framework.Point(0, 40);

                    Client.Scenes.GameScene.Game?.ChangeLandscape();
                }
            };
            _game.Run();
        }

        void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            UnhandledExceptionHandler(e.Exception, e);
        }

        /// <summary>
        /// 处理未处理异常
        /// </summary>
        /// <param name="e"></param>
        private void UnhandledExceptionHandler(Exception ex, RaiseThrowableEventArgs e)
        {
            //处理程序（记录 异常、设备信息、时间等重要信息）
            //**************
            CEnvir.SaveError(ex.ToString());

            e.Handled = true;
        }

        public void BeginInvoke(Action action)
        {
            RunOnUiThread(action);
        }

        #region UI

        FrameLayout _overLayout;
        ViewPager _pager;
        WheelPicker _picker;
        LinearLayout _pickLayout;
        public Action<int> PickerEvent;
        private void InitLayout()
        {
            SetContentView(Resource.Layout.main);
            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _overLayout = FindViewById<FrameLayout>(Resource.Id.overLayout);
            var layout = FindViewById<FrameLayout>(Resource.Id.game);

            var adapter = new MyPagerAdapter(this);
            adapter.Close = () => ShowHook = false;
            _pager.Adapter = adapter;
            _view = _game.Services.GetService(typeof(View)) as View;
            _view.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            layout.AddView(_view);


            _picker = FindViewById<WheelPicker>(Resource.Id.myWheelPicker);
            _picker.SetOnItemSelectedListener(new WheelClick());
        }

        public Point ScreenOffset { get; set; } = Point.Zero;

        public System.Collections.IList PickDatas
        {
            get
            {
                return _picker.Data;
            }
            set
            {
                if (_picker.Data == value) return;

                _picker.Data = value;
                ShowPick = true;
            }
        }

        private void InitWheelPicker(params string[] items)
        {

            _picker.Data = items;
            _picker.SetAtmospheric(true);
            _picker.Curved = true;
            _picker.SetCurtain(true);
        }

        private void over_lick(object sender, EventArgs e)
        {
            _pager!.Visibility = ViewStates.Gone;
        }
        private void AddView(View view)
        {
            _overLayout.Visibility = ViewStates.Visible;
            _overLayout.RemoveAllViews();
            _overLayout.AddView(view);
        }

        PopupWindow _currentWindow;

        public void HidePopWindow()
        {
            _currentWindow?.Dismiss();
        }
        private PopupWindow GetWindow(int layoutId)
        {
            var window = new PopupWindow(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            window.ContentView = LayoutInflater.Inflate(layoutId, null);
            window.IsLaidOutInScreen = false;
            window.Focusable = true;
            //loginWindow.SetBackgroundDrawable(ContextCompat.GetDrawable(this, Resource.Drawable.background));
            window.Width = ViewGroup.LayoutParams.MatchParent;
            window.Height = ViewGroup.LayoutParams.MatchParent;
            window.SoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize;
            return window;
        }

        bool _showLayout = true;
        public bool ShowLayout
        {
            get { return _showLayout; }
            set
            {
                if (value == _showLayout) return;
                _showLayout = value;
                _overLayout.Visibility = _showLayout ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        bool _showHook = false;
        public bool ShowHook
        {
            get { return _showHook; }
            set
            {
                if (value == _showHook) return;
                _showHook = value;
                _pager!.Visibility = _showHook ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        bool _showPick = false;
        public bool ShowPick
        {
            get { return _showPick; }
            set
            {
                if (value == _showPick) return;
                _showPick = value;
                _picker!.Visibility = _showPick ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        #endregion

        public long GetVersionCode()
        {
            var manager = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.Activities);
            var code = manager.LongVersionCode;
            return code;
        }

        public string GetVersionName()
        {
            var manager = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.Activities);
            var name = manager.VersionName;
            return name;
        }

        private void SetUpInputField()
        {
            editText = new EditText(Game.Activity);
            editText.SetSingleLine();
            editText.SetTextSize(ComplexUnitType.Sp, 16f);
            editText.SetPadding(5, 5, 5, 5);
            editText.SetTextColor(Android.Graphics.Color.Black);
            editText.SetBackgroundColor(Android.Graphics.Color.Argb(150, 255, 255, 255));
            //editText.SetOutlineAmbientShadowColor(Android.Graphics.Color.Red);
            //editText.SetOutlineSpotShadowColor(Android.Graphics.Color.Blue);
            editText.ImeOptions = (ImeAction)301989894;
            editText.Visibility = ViewStates.Invisible;
            editText.TextChanged += EditText_TextChanged;
            //editText.InputType = InputTypes.ClassText | InputTypes.TextVariationPassword;

            layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.TopMargin = 60;
            layoutParams.LeftMargin = 60;
            layoutParams.RightMargin = 60;
            AddContentView(editText, layoutParams);
        }

        static DateTime? _lastTime;
        public void Exit()
        {
            if (!_lastTime.HasValue || (DateTime.Now - _lastTime.Value) > new TimeSpan(0, 0, 2))
            {
                Toast.MakeText(ApplicationContext, "再按一次退出程序", ToastLength.Short).Show();
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
            Toast.MakeText(ApplicationContext, msg, ToastLength.Long).Show();
        }

        public static void ShowInputField(string defaultText)
        {
            if (editText.Visibility == ViewStates.Invisible)
                editText.Visibility = ViewStates.Visible;

            editText.Text = defaultText;
            if (defaultText != null)
            {
                editText.SetSelection(defaultText.Length);
            }
            editText.RequestLayout();
            editText.RequestFocus();
            editText.EditorAction -= EditText_EditorAction;
            editText.EditorAction += EditText_EditorAction;

            if (imm.IsActive)
                imm.ShowSoftInput(editText, ShowFlags.Implicit);
        }

        private static void EditText_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                if (DXTextBox.ActiveTextBox != null)
                {
                    DXTextBox.ActiveTextBox.TextBox.Text = editText.Text;
                    DXTextBox.ActiveTextBox.TextBox.OnEnter();
                    DXTextBox.ActiveTextBox = null;
                    DXControl.FocusControl = null;
                    HideInputField();
                }
            }
        }

        public static void HideInputField()
        {
            if (editText.Visibility == ViewStates.Visible)
            {
                editText.Visibility = ViewStates.Invisible;
                imm.HideSoftInputFromWindow(_view.WindowToken, HideSoftInputFlags.NotAlways);

                HideNavBars();
            }
        }

        private void EditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DXTextBox.ActiveTextBox != null)
            {
                DXTextBox.ActiveTextBox.TextBox.Text = e.Text!.ToString();
            }
        }

        public void StartWebView(string url)
        {
            Uri uri = Uri.Parse(url);
            Intent intent = new Intent("android.intent.action.VIEW", uri);
            StartActivity(intent);
        }

        public static void ResetView(int x, int y)
        {
            ResolutionScalingCreate(_view, new Android.Graphics.Point(x, y));
        }
        static View ResolutionScalingCreate(View view, Android.Graphics.Point sourceResolution, Android.Graphics.Point destinationResolution)
        {
            view.ScaleX = destinationResolution.X / (float)sourceResolution.X;
            view.ScaleY = destinationResolution.Y / (float)sourceResolution.Y;
            view.ScaleX = view.ScaleY;
            view.PivotX = 0f;
            view.PivotY = 0f;
            view.SetX((destinationResolution.X - destinationResolution.Y / (float)sourceResolution.Y * sourceResolution.X) / 2f);
            return view;
        }

        static View ResolutionScalingCreate(View view, Android.Graphics.Point sourceResolution)
        {
            Android.Graphics.Point point = new Android.Graphics.Point();
            (view.Context as Activity).WindowManager!.DefaultDisplay!.GetRealSize(point);
            int x = point.X;
            int y = point.Y;
            return ResolutionScalingCreate(view, sourceResolution, new Android.Graphics.Point(x, y));
        }

        protected override void OnResume()
        {
            base.OnResume();
            HideNavBars();
        }

        private static void HideNavBars()
        {
            Main.Window!.DecorView!.SystemUiVisibility = (StatusBarVisibility)5894;
            //WindowCompat.SetDecorFitsSystemWindows(_activity.Window, false);

            //_activity.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            //_activity.Window!.DecorView!.SystemUiVisibility = StatusBarVisibility.Hidden;
        }

        private void FullScreen()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                Window!.Attributes!.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
                Window!.AddFlags(WindowManagerFlags.Fullscreen);
                Window!.AddFlags(WindowManagerFlags.LayoutInOverscan);

                // for covering the full screen in android..
                Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

                // clear FLAG_TRANSLUCENT_STATUS flag:
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                // add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

                //
                Window!.AddFlags(WindowManagerFlags.KeepScreenOn);
            }
        }


        public override void Finish()
        {
            _game.Unload();
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironment_UnhandledExceptionRaiser;
            //完全退出
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            Java.Lang.JavaSystem.Exit(0);
            //base.Finish();
        }

        public bool IsAllScreen => true;
    }
}
