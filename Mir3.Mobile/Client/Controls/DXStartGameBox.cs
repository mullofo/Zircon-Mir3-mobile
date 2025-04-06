using Library;
using MonoGame.Extended;
using System.Drawing;

namespace Client.Controls
{
    /// <summary>
    /// 启动游戏公告
    /// </summary>
    public class DXStartGameBox : DXImageControl
    {
        public DXLabel Label;
        public DxMirButton Button;

        /// <summary>
        /// 启动游戏公告
        /// </summary>
        /// <param name="message">信息</param>
        public DXStartGameBox(string message)
        {
            LibraryFile = LibraryFile.UI1;  //UI面板主图的索引位置
            Index = 1250;  //UI主图序号
            PassThrough = true;  //穿透开启
            Movable = false;
            Modal = true;

            Label = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Text = message,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            Label.Size = new Size(340, DXLabel.GetSize(message, Label.Font, Label.Outline, new Size(4096, 4096)).Height);
            Label.Location = new Point((Size.Width - Label.Size.Width) / 2, (Size.Height - Label.Size.Height) / 2);

            Button = new DxMirButton
            {
                MirButtonType = MirButtonType.TowStatu2,
                LibraryFile = LibraryFile.UI1,
                Index = 1251,
                Parent = this,
                Location = new Point(131, 428),
            };
            Button.MouseClick += (s, e) =>
            {
                Visible = false;
            };

            BringToFront();
        }
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

                if (Button != null)
                {
                    if (!Button.IsDisposed)
                        Button.Dispose();
                    Button = null;
                }
            }
        }
        #endregion
    }
}
