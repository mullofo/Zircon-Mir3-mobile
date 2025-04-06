using Android.Views;
using Android.Widget;
using Client.Envir;
using System;
using C = Library.Network.ClientPackets;
namespace Mir3.Droid
{
    public partial class MainActivity
    {
        #region prop
        public string CAccount
        {
            get
            {
                if (_newAccount == null)
                {
                    return "";
                }
                var acc = _newAccount.FindViewById<EditText>(Resource.Id.c_acc);
                return acc.Text;
            }
            set
            {
                if (_newAccount == null)
                {
                    return;
                }
                var acc = _newAccount.FindViewById<EditText>(Resource.Id.c_acc);
                acc.Text = value;
            }
        }

        public string CPwd
        {
            get
            {
                if (_newAccount == null)
                {
                    return "";
                }
                var acc = _newAccount.FindViewById<EditText>(Resource.Id.c_pwd);
                return acc.Text;
            }
            set
            {

                if (_newAccount == null)
                {
                    return;
                }
                var acc = _newAccount.FindViewById<EditText>(Resource.Id.c_pwd);
                acc.Text = value;
            }
        }
        #endregion

        View _newAccount = null;

        private void ShowCreat()
        {
            _currentWindow = GetWindow(Resource.Layout.new_account);
            _newAccount = _currentWindow.ContentView;
            var create = _newAccount.FindViewById<ImageButton>(Resource.Id.creat_btn);
            create.Click -= Create_Click;
            create.Click += Create_Click;
            var cancel = _newAccount.FindViewById<ImageButton>(Resource.Id.cancel_btn);
            cancel.Click -= Cancel_Click;
            cancel.Click += Cancel_Click;
            _currentWindow.ShowAtLocation(_overLayout, GravityFlags.CenterVertical, 0, 0);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _currentWindow.Dismiss();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            if (_newAccount == null) return;
            //var birthday = _newAccount.FindViewById<EditText>(Resource.Id.c_birthday);

            var inviteCode = _newAccount.FindViewById<EditText>(Resource.Id.c_invite_code);

            //var realName = _newAccount.FindViewById<EditText>(Resource.Id.c_real_name);


            //if(!DateTime.TryParse(birthday.Text, out DateTime birthDate))
            //{
            //    birthDate = new DateTime(1970, 1, 1);
            //}


            C.NewAccount packet = new C.NewAccount
            {
                EMailAddress = CAccount,
                Password = CPwd,
                InviteCode = inviteCode.Text,
                //RealName = realName.Text,
                //BirthDate = birthDate,
                Referral = "",
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
        }



    }
}

