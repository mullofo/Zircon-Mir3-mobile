using Client.Controls;
using Client.Envir;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    public class CountdownBar : DXImageControl
    {
        public DateTime StartTime, EndTime;
        private DXLabel RemainingTimeText;

        public CountdownBar()
        {
            LibraryFile = LibraryFile.GameInter2;
            Index = 2271; //显示坐标

            RemainingTimeText = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.LightYellow,
                Outline = false,
                Parent = this,
                Visible = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "00:00:00",
            };
            RemainingTimeText.Location = new Point(Location.X + 35, Location.Y);

            RemainingTimeText.BeforeDraw += (sender, args) =>
            {
                if (CEnvir.Now < StartTime || CEnvir.Now > EndTime)
                {
                    Visible = false;
                    return;
                }

                Visible = true;
                TimeSpan remainingTime = EndTime.Subtract(CEnvir.Now) + TimeSpan.FromSeconds(1); //剩余时间
                RemainingTimeText.Text = remainingTime.ToString(@"hh\:mm\:ss");

                //进度条调整宽度
                double elapsedMs = (CEnvir.Now - StartTime).TotalMilliseconds;
                if (elapsedMs < 0) return;
                elapsedMs = Math.Max(0, elapsedMs);
                double percent = Math.Min(1, elapsedMs / Math.Max(1, (EndTime - StartTime).TotalMilliseconds));

                //画进度条
                MirImage image = this.Library.CreateImage(2270, ImageType.Image);
                if (image == null) return;
                PresentTexture(image.Image, this,
                    new Rectangle(this.DisplayArea.X + 4, this.DisplayArea.Y + 2, (int)(image.Width * percent), image.Height),
                    Color.White, RemainingTimeText);
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                StartTime = DateTime.MaxValue;
                EndTime = DateTime.MaxValue;

                if (RemainingTimeText != null)
                {
                    if (!RemainingTimeText.IsDisposed)
                        RemainingTimeText.Dispose();

                    RemainingTimeText = null;
                }
            }
        }
        #endregion
    }
}