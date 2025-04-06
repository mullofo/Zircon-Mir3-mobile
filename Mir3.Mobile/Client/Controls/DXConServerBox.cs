using Library;
using MonoGame.Extended;
using System.Drawing;

namespace Client.Controls
{
    /// <summary>
    /// 游戏登录提示
    /// </summary>
    public class DXConServerBox : DXImageControl
    {
        public DXLabel Label;

        /// <summary>
        /// 游戏登录提示
        /// </summary>
        /// <param name="message">信息</param>
        public DXConServerBox(string message)
        {
            LibraryFile = LibraryFile.UI1;  //UI面板主图的索引位置
            Index = 1248;  //UI主图序号
            PassThrough = true;  //穿透开启
            Movable = false;
            Modal = true;

            Label = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Text = message,
                DrawFormat = (TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter)
            };
            Label.Size = new Size(325, DXLabel.GetSize(message, Label.Font, Label.Outline, new Size(4096, 4096)).Height);
            Label.Location = new Point((Size.Width - Label.Size.Width) / 2, (Size.Height - Label.Size.Height) / 2);

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
            }
        }
        #endregion
    }
}
