using Android.Views;
using Android.Widget;
using Client.Envir;
using System;
using C = Library.Network.ClientPackets;
namespace Mir3.Droid
{
    public partial class MainActivity
    {
        View _changeForgetView = null;
        bool _isMail;
        private void ShowForgetPwd(bool isMail)
        {
            _isMail = isMail;
            _currentWindow = GetWindow(isMail ? Resource.Layout.forget : Resource.Layout.forget_key);
            _changeForgetView = _currentWindow.ContentView;

            var ok = _changeForgetView.FindViewById<ImageButton>(Resource.Id.for_ok_btn);
            ok.Click -= Forget_OK_Click;
            ok.Click += Forget_OK_Click;

            var cancel = _changeForgetView.FindViewById<ImageButton>(Resource.Id.for_cancel_btn);
            cancel.Click -= Forget_Cancel_Click;
            cancel.Click += Forget_Cancel_Click;
            _currentWindow.ShowAtLocation(_overLayout, GravityFlags.CenterVertical, 0, 0);
        }

        private void Forget_OK_Click(object sender, EventArgs e)
        {
            if (_isMail)
            {
                var account = _changeForgetView.FindViewById<EditText>(Resource.Id.for_u);
                C.RequestPasswordReset packet = new C.RequestPasswordReset
                {
                    EMailAddress = account.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            else
            {
                var key = _changeForgetView.FindViewById<EditText>(Resource.Id.for_key);
                var pwd = _changeForgetView.FindViewById<EditText>(Resource.Id.for_pwd);
                var pwdConfig = _changeForgetView.FindViewById<EditText>(Resource.Id.for_pwd_confirm);
                if (pwdConfig.Text != pwd.Text)
                {
                    ShowMsg("新密码不一致！");
                    pwd.Text = "";
                    pwdConfig.Text = "";
                    return;
                }

                C.ResetPassword packet = new C.ResetPassword
                {
                    ResetKey = key.Text,
                    NewPassword = pwd.Text,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }

        }

        private void Forget_Cancel_Click(object sender, EventArgs e)
        {
            _currentWindow.Dismiss();
        }
    }
}

