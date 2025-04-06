using System;
using Client.Envir;
using CocoaUiBinding;
using Foundation;
using Mir3.Mobile;
using UIKit;
using C = Library.Network.ClientPackets;

namespace Client.Controllers
{
    [Register("ForgetController")]
    public class ForgetController : BaseController
    {
        IInput Account { get; set; }
        IInput Key { get; set; }
        IInput Pwd { get; set; }
        IInput PwdConfig { get; set; }

        public static bool IsMail { get; set; }
        public ForgetController(IUI ui):base(ui)
        {

        }

        public override IView InitView()
        {
            var view = IView.NamedView(IsMail ? "forget" : "forget-key");
            if (IsMail)
            {
                Account = GetInput(view, "email", Done, true);
            }
            else
            {
                Key = GetInput(view, "key", Next);
                Pwd = GetInput(view, "u_pwd", Next);
                PwdConfig = GetInput(view, "u_pwd_confirm", Done, true);
            }

            var creat = view.GetViewById("ok") as IImage;
            var cancel = view.GetViewById("cancel") as IImage;
            creat.AddEvent(IEventType.Click, (e, s) => ChangeEvent());

            cancel.AddEvent(IEventType.Click, (e, s) => {
                _ui.Push(new LoginController(_ui));
            });

            return view;
        }
        private bool Done(UITextField textField)
        {
            if (ChangeEvent())
            {
                if(IsMail)
                    Account.TextField.EndEditing(false);
                else
                    PwdConfig.TextField.EndEditing(false);
            }
            return true;
        }

        private bool Next(UITextField textField)
        {
            if (Key.TextField.IsFirstResponder)
            {
                Pwd.TextField.BecomeFirstResponder();
            }
            else if (Pwd.TextField.IsFirstResponder)
            {
                PwdConfig.TextField.BecomeFirstResponder();
            }
            return true;
        }

        private bool ChangeEvent()
        {
            if (IsMail)
            {
                C.RequestPasswordReset packet = new C.RequestPasswordReset
                {
                    EMailAddress = Account.Value,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            else
            {
                if (PwdConfig.Value != Pwd.Value)
                {
                    _ui.ShowMsg("新密码不一致！");
                    Pwd.Value = "";
                    PwdConfig.Value = "";
                    return false;
                }

                C.ResetPassword packet = new C.ResetPassword
                {
                    ResetKey = Key.Value,
                    NewPassword = Pwd.Value,
                    CheckSum = CEnvir.C,
                };

                CEnvir.Enqueue(packet);
            }
            return true;
        }
    }
}

