using Client.UserModels;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Controls
{
    /// <summary>
    /// 即时聊天窗口
    /// </summary>
    public sealed class DXMessageBriefBox : DXWindow
    {
        #region Properties
        public DXLabel Label { get; private set; }

        public DXTextBox HiddenBox;

        /// <summary>
        /// 尺寸改变时
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
        }
        /// <summary>
        /// 容器改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (Parent == null) return;

            Location = new Point((Parent.DisplayArea.Width - DisplayArea.Width) / 2, (Parent.DisplayArea.Height - DisplayArea.Height) / 2);
        }

        public override WindowType Type => WindowType.MessageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 即时聊天窗口
        /// </summary>
        public DXMessageBriefBox(string message, string caption, DXMessageBoxButtons buttons = DXMessageBoxButtons.OK)
        {
            Modal = true;
            HasFooter = true;

            TitleLabel.Text = caption;

            Parent = ActiveScene;
            MessageBoxList.Add(this);

            Label = new DXLabel
            {
                AutoSize = false,
                Location = new Point(10, 35),
                Parent = this,
                Text = message,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            Label.Size = new Size(380, DXLabel.GetSize(message, Label.Font, Label.Outline, new Size(4096, 4096)).Height);
            SetClientSize(Label.Size);
            Label.Location = ClientArea.Location;

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);

            BringToFront();
        }

        #region Methods
        /// <summary>
        /// 分辨率改变时
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

                if (HiddenBox != null)
                {
                    DXTextBox.ActiveTextBox = HiddenBox;
                    HiddenBox = null;
                }
            }
        }
        #endregion
    }
}
