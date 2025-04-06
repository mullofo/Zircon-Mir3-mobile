using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    /// <summary>
    /// 确认窗口 
    /// </summary>
    public sealed class DXConfirmWindow : DXWindow
    {
        #region Properties

        #region HasBootm
        public bool HasBootm
        {
            get => _HasBootm;
            set
            {
                if (_HasBootm == value) return;

                bool oldValue = _HasBootm;
                _HasBootm = value;

                OnHasBootmChanged(oldValue, value);
            }
        }
        private bool _HasBootm;
        public void OnHasBootmChanged(bool oValue, bool nValue)
        {
            ConfirmButton.Visible = nValue;
            CancelButton.Visible = nValue;
            UpdateLocations();
        }
        #endregion

        #region ContentSize
        /// <summary>
        /// 内容大小
        /// </summary>
        public int ContentSize
        {
            get => _ContentSize;
            set
            {
                if (_ContentSize == value) return;

                int oldValue = _ContentSize;
                _ContentSize = value;

                OnContentSizeChanged(oldValue, value);
            }
        }
        private int _ContentSize;
        public void OnContentSizeChanged(int oValue, int nValue)
        {
            UpdateLocations();
        }
        #endregion

        //public override void OnTextChanged(string oValue, string nValue)
        //{
        //    base.OnTextChanged(oValue, nValue);
        //    Content.Size = ContentPanel.Size;
        //    Content.Text = nValue;
        //}

        public DXButton ConfirmButton, CancelButton;
        public DXImageControl TopPanel;
        public DXLabel Content;
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 确认窗口
        /// </summary>
        /// <param name="content"></param>
        /// <param name="buttons"></param>
        /// <param name="ok"></param>
        /// <param name="cancel"></param>
        public DXConfirmWindow(string content, DXMessageBoxButtons buttons, Action ok = null, Action cancel = null)
        {
            Init(content, buttons, ok, cancel);
        }
        public DXConfirmWindow(string content)
        {
            Init(content, DXMessageBoxButtons.OK, null, null);
        }
        public DXConfirmWindow(string content, Action ok)
        {
            Init(content, DXMessageBoxButtons.OK, ok, null);
        }

        #region Methods
        private void Init(string content, DXMessageBoxButtons buttons, Action ok, Action cancel)
        {
            HasFooter = true;

            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0F;

            Parent = ActiveScene;
            MessageBoxList.Add(this);
            Modal = true;

            TopPanel = new DXImageControl   //顶部面板
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1240,
            };

            Content = new DXLabel   //内容
            {
                Text = content,
                Parent = TopPanel,
                DrawFormat = content.IndexOf("\n") != -1 ? TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter : TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                AutoSize = false,
                Location = content.IndexOf("\n") != -1 ? new Point(22, 105) : new Point(22, 125),
            };
            Content.Size = new Size(315, DXLabel.GetSize(content, Content.Font, Content.Outline, new Size(4096, 4096)).Height);
            SetClientSize(TopPanel.Size);

            ConfirmButton = new DXButton   //确认按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1241,
                Location = new Point(50, 190),
                Parent = TopPanel,
                Hint = "确定".Lang(),
            };

            ConfirmButton.MouseClick += (o, e) =>
            {
                ActiveScene.ConfirmWindow = null;
                ok?.Invoke();
                Dispose();
            };

            CancelButton = new DXButton   //取消按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1245,
                Parent = TopPanel,
                Location = new Point(240, 190),
                Hint = "取消".Lang(),
            };
            CancelButton.MouseClick += (s, e) =>
            {
                ActiveScene.ConfirmWindow = null;
                cancel?.Invoke();
                Dispose();
            };
            HasBootm = true;
            switch (buttons)
            {

                case DXMessageBoxButtons.None:
                    HasBootm = false;
                    break;
                case DXMessageBoxButtons.OK:
                    CancelButton.Visible = false;
                    CloseButton.Visible = false;
                    break;
                case DXMessageBoxButtons.YesNo:
                default:
                    break;
                case DXMessageBoxButtons.Cancel:
                    ConfirmButton.Visible = false;
                    CloseButton.Visible = false;
                    break;
            }
            ActiveScene.ConfirmWindow = this;
        }

        public static DXConfirmWindow Show(string message, DialogAction action = DialogAction.None)
        {
            DXConfirmWindow box = new DXConfirmWindow(message);

            switch (action)
            {
                case DialogAction.None:
                    break;
                case DialogAction.Close:
                    new DXConfirmWindow(message, () => CEnvir.Target.Close());
                    break;
                case DialogAction.ReturnToLogin:
                    new DXConfirmWindow(message, () => CEnvir.ReturnToLogin());
                    break;
            }

            return box;
        }

        /// <summary>
        /// OK点击
        /// </summary>
        public void OkPress()
        {
            ConfirmButton.InvokeMouseClick();
        }
        /// <summary>
        /// 更新坐标
        /// </summary>
        private void UpdateLocations()
        {
            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TopPanel != null)
                {
                    if (!TopPanel.IsDisposed)
                        TopPanel.Dispose();
                    TopPanel = null;
                }

                if (Content != null)
                {
                    if (!Content.IsDisposed)
                        Content.Dispose();
                    Content = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();
                    ConfirmButton = null;
                }
                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();
                    CancelButton = null;
                }
            }
        }
        #endregion
    }
}
