using System;
using CocoaUiBinding;
using Foundation;
using Mir3.Mobile;
using UIKit;

namespace Client.Controllers
{
	public class BaseController : ITable
    {
        protected IUI _ui;
        UITextField _currentTextField;

        public BaseController(IUI ui)
        {
            _ui = ui;
            ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext | UIModalPresentationStyle.FullScreen;
            this.View!.BackgroundColor = UIColor.White.ColorWithAlpha(0);
        }

        protected bool SetCurrentTextField(UITextField textField)
        {
            _currentTextField = textField;
            return true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RegisterKeybordEvent();
            var view = InitView();
            if (view == null) return;
            view.AddEvent(IEventType.Click, (s, e) => {
                _currentTextField?.EndEditing(false);
            });
            this.AddIViewRow(view);
            this.Reload();
        }

        private void RegisterKeybordEvent()
        {
           
            UIKeyboard.Notifications.ObserveWillShow((s,e) => {
                if (_currentTextField == null) return;
                var rect = _currentTextField.ConvertRectToView(_currentTextField.Frame, View.Window);
                var keyborad = (NSValue)e.Notification.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey);
                var keyboradValue = keyborad.CGRectValue;
                //获取键盘相对于this.view的frame ，传window和传null是一样的
                var keyboradRect = View.ConvertRectFromView(keyboradValue, View.Window);
                var keyboardTop = keyboradRect.Y;
                //获取键盘弹出动画时间值
                var animationDurationValue = e.AnimationDuration;
                //如果键盘盖住了输入框
                if (keyboardTop < rect.Y)
                {
                    //计算需要往上移动的偏移量（输入框底部离键盘顶部为10的间距）
                    var offset = keyboardTop - rect.Y - _currentTextField.Frame.Height - 10;
                    UIView.AnimateAsync(animationDurationValue, () => {
                        View.Frame = new CoreGraphics.CGRect(View.Frame.X, offset, View.Frame.Width, View.Frame.Height);
                    });
                }
            });

            UIKeyboard.Notifications.ObserveWillHide((s, e) => {
                var animationDurationValue = e.AnimationDuration;
                if (View.Frame.Y < 0)
                {
                    UIView.AnimateAsync(animationDurationValue, () => {
                        View.Frame = new CoreGraphics.CGRect(View.Frame.X, 0, View.Frame.Width, View.Frame.Height);
                    });
                }

            });
        }

        public virtual IView InitView()
        {
            return null;
        }
        protected IInput GetInput(IView view, string id, UITextFieldCondition condition = null, bool isDone = false)
        {
            var input = view.GetViewById(id) as IInput;
            input.TextField.ShouldBeginEditing = SetCurrentTextField;
            input.TextField.ReturnKeyType = isDone ? UIReturnKeyType.Done : UIReturnKeyType.Next;
            if (condition != null)
                input.TextField.ShouldReturn = condition;
            return input;
        }
    }
}

