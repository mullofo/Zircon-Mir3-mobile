using Client.Envir;
using Client.UserModels;
using Library;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 消息框控件
    /// </summary>
    public sealed class DXMessageBox : DXWindow
    {
        #region Properties
        public DXLabel Label { get; private set; }
        public DXButton OKButton, CancelButton, NoButton, YesButton;
        public DXMessageBoxButtons Buttons;

        public DXTextBox HiddenBox;

        public DXImageControl Background;

        /// <summary>
        /// 尺寸大小改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (Parent != null)
                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);

            if (Label != null)
                Label.Location = ClientArea.Location;

            if (OKButton != null)
                OKButton.Location = new Point(OKButton.Location.X, Size.Height - 44);
            if (CancelButton != null)
                CancelButton.Location = new Point(CancelButton.Location.X, Size.Height - 44);
            if (NoButton != null)
                NoButton.Location = new Point(NoButton.Location.X, Size.Height - 44);
            if (YesButton != null)
                YesButton.Location = new Point(YesButton.Location.X, Size.Height - 44);

        }
        /// <summary>
        /// 面板改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (Parent == null) return;

            Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
        }
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
            {
                HiddenBox = DXTextBox.ActiveTextBox;
                DXTextBox.ActiveTextBox = null;
            }
            else if (HiddenBox != null)
                DXTextBox.ActiveTextBox = HiddenBox;

        }

        public override WindowType Type => WindowType.MessageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 消息框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="buttons"></param>
        public DXMessageBox(string message, string caption = "", DXMessageBoxButtons buttons = DXMessageBoxButtons.OK)
        {
            Buttons = buttons;
            Modal = true;

            HasFooter = false;
            HasTitle = false;
            HasTopBorder = false;
            HasTransparentEdges = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Parent = ActiveScene;
            MessageBoxList.Add(this);

            Background = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1240,
            };

            Label = new DXLabel
            {
                AutoSize = false,
                Location = new Point(0, 0),
                Parent = Background,
                Text = message,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            Label.Size = new Size(365, DXLabel.GetSize(message, Label.Font, Label.Outline, new Size(4096, 4096)).Height);
            SetClientSize(Background.Size);

            Label.Location = new Point(0, 120);

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);


            switch (Buttons)
            {
                case DXMessageBoxButtons.OK:
                    OKButton = new DXButton
                    {
                        Location = new Point((Size.Width - 80) / 2 - 5, Size.Height - 105),
                        Size = new Size(80, DefaultHeight),
                        Parent = Background,
                        LibraryFile = LibraryFile.UI1,
                        Index = 1243,
                    };
                    OKButton.MouseClick += (o, e) => Dispose();
                    break;
                case DXMessageBoxButtons.YesNo:
                    YesButton = new DXButton
                    {
                        Location = new Point(80, 190),
                        Size = new Size(80, DefaultHeight),
                        Parent = Background,
                        LibraryFile = LibraryFile.UI1,
                        Index = 1241,
                    };
                    YesButton.MouseClick += (o, e) => Dispose();
                    NoButton = new DXButton
                    {
                        Location = new Point(210, 190),
                        Size = new Size(80, DefaultHeight),
                        Parent = Background,
                        LibraryFile = LibraryFile.UI1,
                        Index = 1245,
                    };
                    NoButton.MouseClick += (o, e) => Dispose();
                    break;
                case DXMessageBoxButtons.Cancel:
                    CancelButton = new DXButton
                    {
                        Location = new Point((Size.Width - 80) / 2 - 5, Size.Height - 105),
                        Size = new Size(80, DefaultHeight),
                        Parent = Background,
                        LibraryFile = LibraryFile.UI1,
                        Index = 1241,
                    };
                    CancelButton.MouseClick += (o, e) => Dispose();
                    break;
            }
            BringToFront();
        }

        #region Methods
        /// <summary>
        /// 消息框显示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static DXMessageBox Show(string message, string caption, DialogAction action = DialogAction.None)
        {
            DXMessageBox box = new DXMessageBox(message, caption);

            switch (action)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Close:
                    box.OKButton.MouseClick += (o, e) => CEnvir.Target.Close();
                    box.CloseButton.MouseClick += (o, e) => CEnvir.Target.Close();
                    break;
                case DialogAction.ReturnToLogin:
                    box.OKButton.MouseClick += (o, e) => CEnvir.ReturnToLogin();
                    //box.CloseButton.MouseClick += (o, e) => CEnvir.ReturnToLogin();
                    break;
            }

            return box;
        }

        /// <summary>
        /// 按键动作
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)  //ESC按键事件
            {
                switch (Buttons)
                {
                    case DXMessageBoxButtons.OK:
                        if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick();
                        break;
                    case DXMessageBoxButtons.YesNo:
                        if (NoButton != null && !NoButton.IsDisposed) NoButton.InvokeMouseClick();
                        break;
                }
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Enter)   //回车键事件
            {
                switch (Buttons)
                {
                    case DXMessageBoxButtons.OK:
                        if (OKButton != null && !OKButton.IsDisposed) OKButton.InvokeMouseClick();
                        break;
                    case DXMessageBoxButtons.YesNo:
                        if (YesButton != null && !YesButton.IsDisposed) YesButton.InvokeMouseClick();
                        break;

                }
                e.Handled = true;
            }
        }
        /// <summary>
        /// 分辨率改变
        /// </summary>
        public override void ResolutionChanged()
        {
            base.ResolutionChanged();

            if (Parent != null)
                Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }

                if (OKButton != null)
                {
                    if (!OKButton.IsDisposed)
                        OKButton.Dispose();
                    OKButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();
                    CancelButton = null;
                }

                if (NoButton != null)
                {
                    if (!NoButton.IsDisposed)
                        NoButton.Dispose();
                    NoButton = null;
                }

                if (YesButton != null)
                {
                    if (!YesButton.IsDisposed)
                        YesButton.Dispose();
                    YesButton = null;
                }

                if (HiddenBox != null)
                {
                    DXTextBox.ActiveTextBox = HiddenBox;
                    HiddenBox = null;
                }

                Buttons = DXMessageBoxButtons.None;
                MessageBoxList.Remove(this);
            }
        }
        #endregion
    }

    /// <summary>
    /// 消息框按钮
    /// </summary>
    public enum DXMessageBoxButtons
    {
        /// <summary>
        /// 消息框按钮 无
        /// </summary>
        None,
        /// <summary>
        /// 消息框按钮 OK
        /// </summary>
        OK,
        /// <summary>
        /// 消息框按钮 YES NO
        /// </summary>
        YesNo,
        /// <summary>
        /// 消息框按钮 取消
        /// </summary>
        Cancel
    }

    /// <summary>
    /// 对话动作类型
    /// </summary>
    public enum DialogAction
    {
        /// <summary>
        /// 对话动作类型 无
        /// </summary>
        None,
        /// <summary>
        /// 对话动作类型 关闭
        /// </summary>
        Close,
        /// <summary>
        /// 对话动作类型 返回登录
        /// </summary>
        ReturnToLogin
    }
}
