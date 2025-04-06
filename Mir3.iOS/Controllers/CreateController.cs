using System;
using Client.Envir;
using CocoaUiBinding;
using Foundation;
using Mir3.Mobile;
using UIKit;
using C = Library.Network.ClientPackets;
namespace Client.Controllers
{
    [Register("CreateController")]
    public class CreateController : BaseController
    {
        public static IInput Account { get; private set; }
        public static IInput Password { get; private set; }

        static IInput Confirm { get; set; }

        static IInput InviteCode { get; set; }



        public CreateController(IUI ui) : base(ui)
        {
        }

        public override IView InitView()
        {
            var view = IView.NamedView("creat");

            Account = view.GetViewById("acc_name") as IInput;

            Password = view.GetViewById("c_pwd") as IInput;

            Confirm = view.GetViewById("c_pwd_confirm") as IInput;

            InviteCode = view.GetViewById("c_invite_code") as IInput;

            Account.TextField.ReturnKeyType = UIReturnKeyType.Next;
            Password.TextField.ReturnKeyType = UIReturnKeyType.Next;
            Confirm.TextField.ReturnKeyType = UIReturnKeyType.Next;
            InviteCode.TextField.ReturnKeyType = UIReturnKeyType.Done;

            Account.TextField.ShouldBeginEditing = SetCurrentTextField;
            Password.TextField.ShouldBeginEditing = SetCurrentTextField;
            Confirm.TextField.ShouldBeginEditing = SetCurrentTextField;
            InviteCode.TextField.ShouldBeginEditing = SetCurrentTextField;

            Account.TextField.ShouldReturn = Next;
            Password.TextField.ShouldReturn = Next;
            Confirm.TextField.ShouldReturn = Next;
            InviteCode.TextField.ShouldReturn = Next;


            var creat = view.GetViewById("ok") as IImage;
            var cancel = view.GetViewById("cancel") as IImage;
            creat.AddEvent(IEventType.Click, (e, s) => CreatEvent());

            cancel.AddEvent(IEventType.Click, (e, s) => {
                _ui.Push(new LoginController(_ui));
            });


            return view;

        }

        private bool Next(UITextField textField)
        {
            if (Account.TextField.IsFirstResponder)
            {
                Password.TextField.BecomeFirstResponder();
            }
            else if (Password.TextField.IsFirstResponder)
            {
                Confirm.TextField.BecomeFirstResponder();
            }
            else if (Confirm.TextField.IsFirstResponder)
            {
                InviteCode.TextField.BecomeFirstResponder();
            }
            else if (InviteCode.TextField.IsFirstResponder)
            {
                if (CreatEvent())
                {
                    InviteCode.TextField.EndEditing(false);
                }
            }
            return true;
        }

        private bool CreatEvent()
        {
            if (string.IsNullOrEmpty(Account.Value)
               || string.IsNullOrEmpty(Password.Value)
               || string.IsNullOrEmpty(Confirm.Value)
               || string.IsNullOrEmpty(InviteCode.Value))
            {
                _ui.ShowMsg("请填写完整提交!");
                return false;
            }

            if (Password.Value != Confirm.Value)
            {
                _ui.ShowMsg("密码不一至!");
                return false;
            }

            C.NewAccount packet = new C.NewAccount
            {
                EMailAddress = Account.Value,
                Password = Password.Value,
                InviteCode = InviteCode.Value,
                //RealName = realName.Text,
                //BirthDate = birthDate,
                Referral = "",
                CheckSum = CEnvir.C,
            };

            CEnvir.Enqueue(packet);
            return true;
        }
    }
}

