using Android.Views;
using Android.Widget;
using Client.Envir;
using System;
using C = Library.Network.ClientPackets;

namespace Mir3.Droid
{
    public partial class MainActivity
    {
        #region 属性
        public string Account
        {
            get
            {
                var acc = _overLayout.FindViewById<EditText>(Resource.Id.acc);
                if (acc == null)
                {
                    return "";
                }
                return acc.Text;
            }
            set
            {
                var acc = _overLayout.FindViewById<EditText>(Resource.Id.acc);
                if (acc == null)
                {
                    return;
                }
                Config.RememberedEMail = value;
                acc.Text = value;
            }
        }

        public string Pwd
        {
            get
            {
                var acc = _overLayout.FindViewById<EditText>(Resource.Id.pwd);
                if (acc == null)
                {
                    return "";
                }
                return acc.Text;
            }
            set
            {
                var acc = _overLayout.FindViewById<EditText>(Resource.Id.pwd);
                if (acc == null)
                {
                    return;
                }
                Config.RememberedPassword = value;
                acc.Text = value;
            }
        }
        #endregion

        public void InitLogin()
        {
            var view = View.Inflate(this, Resource.Layout.login, null);
            var newBtn = view.FindViewById<ImageButton>(Resource.Id.newAcount);
            newBtn.Click -= NewBtn_Click;
            newBtn.Click += NewBtn_Click;

            var change = view.FindViewById<ImageButton>(Resource.Id.changePwd);
            change.Click -= Change_Click;
            change.Click += Change_Click;

            var forget = view.FindViewById<ImageButton>(Resource.Id.forgetPwd);
            forget.Click -= Forget_Click;
            forget.Click += Forget_Click;

            var exit = view.FindViewById<ImageButton>(Resource.Id.exit);
            exit.Click -= Exit_Click;
            exit.Click += Exit_Click;


            var loginBtn = view.FindViewById<ImageButton>(Resource.Id.login_btn);
            loginBtn.Click -= LoginBtn_Click;
            loginBtn.Click += LoginBtn_Click;
            AddView(view);

            Account = Config.RememberedEMail;
            Pwd = Config.RememberedPassword;
        }

        private void Forget_Click(object sender, EventArgs e)
        {
            #region action sheet
            var sheet = new BottomActionSheet(this);
            var view = View.Inflate(this, Resource.Layout.sheet_forget, null);
            var email = view.FindViewById<LinearLayout>(Resource.Id.get_code);
            var useCode = view.FindViewById<LinearLayout>(Resource.Id.use_code);
            var cancel = view.FindViewById<LinearLayout>(Resource.Id.cancel);
            email.Touch += (s, e) =>
            {
                sheet.Dismiss();
                ShowForgetPwd(true);
            };
            useCode.Touch += (s, e) =>
            {
                sheet.Dismiss();
                ShowForgetPwd(false);
            };
            cancel.Touch += (s, e) =>
            {
                sheet.Dismiss();
            };
            sheet.SetContentView(view);
            sheet.Show();
            #endregion

            #region 普通窗口
            //_currentWindow = GetWindow(Resource.Layout.sheet_forget);
            //var view = _currentWindow.ContentView;
            //var email = view.FindViewById<LinearLayout>(Resource.Id.get_code);
            //var useCode = view.FindViewById<LinearLayout>(Resource.Id.use_code);
            //var cancel = view.FindViewById<LinearLayout>(Resource.Id.cancel);
            //email.Touch += (s, e) =>
            //{
            //    _currentWindow?.Dismiss();
            //    ShowForgetPwd(true);
            //};
            //useCode.Touch += (s, e) =>
            //{
            //    _currentWindow?.Dismiss();
            //    ShowForgetPwd(false);
            //};
            //cancel.Touch += (s, e) =>
            //{
            //    _currentWindow?.Dismiss();
            //};
            //_currentWindow.ShowAtLocation(_overLayout, GravityFlags.CenterVertical, 0, 0);
            #endregion
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Change_Click(object sender, EventArgs e)
        {
            ShowChangePwd();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
#if !DEBUG
            if (Helpers.VerifyDevice.Verify())
            {
                ShowMsg("禁止模拟器中登录游戏!");
                return;
            }
#endif
            C.Login packet = new C.Login
            {
                EMailAddress = Account,
                Password = Pwd,
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
        }

        private void NewBtn_Click(object sender, EventArgs e)
        {
            ShowCreat();
        }
    }
}

