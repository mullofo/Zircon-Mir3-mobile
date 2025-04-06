using UIKit;
using Client.Envir;
using C = Library.Network.ClientPackets;
using Mir3.Mobile;
using Foundation;
using CocoaUiBinding;
using Client.Extentions;

namespace Client.Controllers
{
    [Register("LoginController")]
    public class LoginController : BaseController
    {
        public static IInput Account { get; private set; }
        public static IInput Password { get; private set; }

        static IView _view;

        public LoginController(IUI ui) : base(ui)
        {
        }

        public static void Show(bool show)
        {
            if (_view == null) return;
            if (show)
                _view.Show();
            else
                _view.Hide();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override IView InitView()
        {
            _view = IView.NamedView("login");
            var h = 120;
            var style = $"margin-top:{_ui.Height - h};height:{h};";
            _view.Style.Set(style);

            Account = _view.GetViewById("name") as IInput;
            Password = _view.GetViewById("password") as IInput;
            Account.Value = Config.RememberedEMail;
            Password.Value = Config.RememberedPassword;

            Account.TextField.ReturnKeyType = UIReturnKeyType.Next;
            Account.TextField.ShouldBeginEditing = SetCurrentTextField;
            Password.TextField.ReturnKeyType = UIReturnKeyType.Done;
            Password.TextField.ShouldBeginEditing = SetCurrentTextField;

            Account.TextField.ShouldReturn = (s) => {
                Password.TextField.BecomeFirstResponder();
                return true;
            };

            Password.TextField.ShouldReturn = (s) => {
                if (LoginGame())
                {
                    Password.TextField.EndEditing(false);
                }
                return true;
            };


            var creat = _view.GetViewById("newAcount") as IImage;
            var change = _view.GetViewById("changePwd") as IImage;
            var forget = _view.GetViewById("forgetPwd") as IImage;
            var exit = _view.GetViewById("exit") as IImage;


            creat.AddEvent(IEventType.Click, (e, s) => {
                _ui.Push(new CreateController(_ui));
            });

            change.AddEvent(IEventType.Click, (e, s) => {

                _ui.Push(new ChangeController(_ui));
            });

            forget.AddEvent(IEventType.Click, (e, s) => {

                SelForget();
            });

            exit.AddEvent(IEventType.Click, (e, s) => {

                _ui.Exit();
            });


            var submmit = _view.GetViewById("submit") as IImage;
            submmit.AddEvent(IEventType.Click, (e, s) => LoginGame());

            return _view;
        }

        private bool LoginGame()
        {
#if !DEBUG
            if (Xamarin.iOS.DeviceHardware.IsSimulator)
            {
                _ui.ShowMsg("禁止模拟器中登录游戏!");
                return false;
            }
#endif
            if (!_ui.ShowLayout)
            {
                _ui.ShowMsg("服务连接中...");
                return false;
            }
            if (string.IsNullOrWhiteSpace(Account.Value))
            {
                _ui.ShowMsg("用户必填");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password.Value))
            {
                _ui.ShowMsg("密码必填");
                return false;
            }

            _ui.Account = Account.Value;
            _ui.Pwd = Password.Value;


            C.Login packet = new C.Login
            {
                EMailAddress = Account.Value,
                Password = Password.Value,
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
            return true;
        }


        private void SelForget()
        {
            var sheetView = UIAlertController.Create("重置密码".Lang(), "", UIAlertControllerStyle.ActionSheet);

            sheetView.AddAction(UIAlertAction.Create("申请密钥", UIAlertActionStyle.Default, (alert) => {
                ForgetController.IsMail = true;
                _ui.Push(new ForgetController(_ui));

            }));
            sheetView.AddAction(UIAlertAction.Create("使用密钥", UIAlertActionStyle.Default, (alert) => {
                ForgetController.IsMail = false;
                _ui.Push(new ForgetController(_ui));

            }));
            sheetView.AddAction(UIAlertAction.Create("取消", UIAlertActionStyle.Cancel,null));
            PresentViewController(sheetView, true, null);
        }
    }
}

