using System;
using Client.Controls;
using CocoaUiBinding;
using Foundation;
using GameController;
using Mir3.Mobile;
using UIKit;

namespace Client.Controllers
{
    public class InputController : BaseController
    {
        IInput _input;
        string _txt;
        public InputController(IUI ui,string txt) : base(ui)
        {
            _txt = txt;
        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _input.Value = _txt ?? "";
            _input.TextField.BecomeFirstResponder();
        }
        public override IView InitView()
        {
            var view = IView.NamedView("input");

            _input = view.GetViewById("edit") as IInput;
            _input.TextField.ReturnKeyType = UIKit.UIReturnKeyType.Done;

            _input.AddEvent(IEventType.Change, (e, s) =>
            {
                if (DXTextBox.ActiveTextBox != null)
                {
                    DXTextBox.ActiveTextBox.TextBox.Text = _input.Value;
                }
            });
            _input.AddEvent(IEventType.Return, (e, s) =>
            {
                if (DXTextBox.ActiveTextBox != null)
                {
                    DXTextBox.ActiveTextBox.TextBox.Text = _input.Value;
                    DXTextBox.ActiveTextBox.TextBox.OnEnter();
                    DXTextBox.ActiveTextBox = null;
                    DXControl.FocusControl = null;
                }
                _input.TextField.EndEditing(false);
                _ui.HidePopWindow();
            });

            return view;
        }

    }
}

