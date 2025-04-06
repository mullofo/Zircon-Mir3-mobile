using System;
using Client.Envir;
using CocoaUiBinding;
using Foundation;
using Mir3.Mobile;
using UIKit;
using C = Library.Network.ClientPackets;

namespace Client.Controllers
{
    [Register("ChangeController")]
    public class ChangeController : BaseController
    {
        static IInput Account { get; set; }
        static IInput OldPwd { get; set; }
        static IInput Pwd { get; set; }
        static IInput PwdConfig { get; set; }

        public ChangeController(IUI ui):base(ui)
        {

        }

        public override IView InitView()
        {
            var view = IView.NamedView("change");

            Account = view.GetViewById("acc_name") as IInput;
            OldPwd = view.GetViewById("o_pwd") as IInput;
            Pwd = view.GetViewById("u_pwd") as IInput;
            PwdConfig = view.GetViewById("u_pwd_confirm") as IInput;

            Account.TextField.ReturnKeyType = UIReturnKeyType.Next;
            OldPwd.TextField.ReturnKeyType = UIReturnKeyType.Next;
            Pwd.TextField.ReturnKeyType = UIReturnKeyType.Next;
            PwdConfig.TextField.ReturnKeyType = UIReturnKeyType.Done;


            Account.TextField.ShouldBeginEditing = SetCurrentTextField;
            OldPwd.TextField.ShouldBeginEditing = SetCurrentTextField;
            Pwd.TextField.ShouldBeginEditing = SetCurrentTextField;
            PwdConfig.TextField.ShouldBeginEditing = SetCurrentTextField;

            Account.TextField.ShouldReturn = Next;
            OldPwd.TextField.ShouldReturn = Next;
            Pwd.TextField.ShouldReturn = Next;
            PwdConfig.TextField.ShouldReturn = Next;


            PwdConfig.TextField.ShouldReturn = (s) => {
                if (ChangeEvent())
                {
                    PwdConfig.TextField.EndEditing(false);
                }
                return true;
            };


            var creat = view.GetViewById("ok") as IImage;
            var cancel = view.GetViewById("cancel") as IImage;
            creat.AddEvent(IEventType.Click, (e, s) => ChangeEvent());

            cancel.AddEvent(IEventType.Click, (e, s) => {
                _ui.Push(new LoginController(_ui));
            });

            return view;
        }

        private bool Next(UITextField textField)
        {
            if (Account.TextField.IsFirstResponder)
            {
                OldPwd.TextField.BecomeFirstResponder();
            }
            else if (OldPwd.TextField.IsFirstResponder)
            {
                Pwd.TextField.BecomeFirstResponder();
            }
            else if (Pwd.TextField.IsFirstResponder)
            {
                PwdConfig.TextField.BecomeFirstResponder();
            }
            else if (PwdConfig.TextField.IsFirstResponder)
            {
                ChangeEvent();
            }
            return true;
        }

        private bool ChangeEvent()
        {

            if (PwdConfig.Value != Pwd.Value)
            {
                _ui.ShowMsg("新密码不一致！");
                Pwd.Value = "";
                PwdConfig.Value = "";
                return false;
            }
            C.ChangePassword packet = new C.ChangePassword
            {
                EMailAddress = Account.Value,
                CurrentPassword = OldPwd.Value,
                NewPassword = Pwd.Value,
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
            return true;
        }
    }
}

