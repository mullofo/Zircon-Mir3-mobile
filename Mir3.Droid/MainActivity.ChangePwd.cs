using Android.Views;
using Android.Widget;
using Client.Envir;
using System;
using C = Library.Network.ClientPackets;
namespace Mir3.Droid
{
    public partial class MainActivity
    {

        View _changePwdView = null;


        private void ShowChangePwd()
        {
            _currentWindow = GetWindow(Resource.Layout.change_pwd);
            _changePwdView = _currentWindow.ContentView;

            var view = View.Inflate(this, Resource.Layout.change_pwd, null);
            var ok = _changePwdView.FindViewById<ImageButton>(Resource.Id.u_ok_btn);
            ok.Click -= U_OK_Click;
            ok.Click += U_OK_Click;

            var cancel = _changePwdView.FindViewById<ImageButton>(Resource.Id.u_cancel_btn);
            cancel.Click -= U_Cancel_Click;
            cancel.Click += U_Cancel_Click;
            _currentWindow.ShowAtLocation(_overLayout, GravityFlags.CenterVertical, 0, 0);
        }

        private void U_OK_Click(object sender, EventArgs e)
        {

            var account = _changePwdView.FindViewById<EditText>(Resource.Id.u_acc);
            var oldPwd = _changePwdView.FindViewById<EditText>(Resource.Id.o_pwd);
            var pwd = _changePwdView.FindViewById<EditText>(Resource.Id.u_pwd);
            var pwdConfig = _changePwdView.FindViewById<EditText>(Resource.Id.u_pwd_confirm);
            if (pwdConfig.Text != pwd.Text)
            {
                ShowMsg("新密码不一致！");
                pwd.Text = "";
                pwdConfig.Text = "";
                return;
            }
            C.ChangePassword packet = new C.ChangePassword
            {
                EMailAddress = account.Text,
                CurrentPassword = oldPwd.Text,
                NewPassword = pwd.Text,
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
        }

        private void U_Cancel_Click(object sender, EventArgs e)
        {
            _currentWindow.Dismiss();
        }
    }
}

