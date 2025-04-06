using Client.Controls;
using Client.Envir;
using Client.UserModels;
using System.Drawing;
using System.Timers;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 消息提示文本框
    /// </summary>
    public class MsgTipTextBox : DXWindow
    {
        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;

        #region Properties
        DXLabel MainLabel, MidLabel, LastLabel;
        Timer _timer;
        #endregion

        /// <summary>
        /// 消息提示文本框
        /// </summary>
        public MsgTipTextBox()
        {
            Size = new Size(300, 100);

            Opacity = 0F;

            PassThrough = true;  //穿透开启

            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            CloseButton.Visible = false;

            AllowResize = false;
            CanResizeHeight = false;
            Border = false;
            Movable = false;

            var width = Size.Width - 15;


            MainLabel = new DXLabel { Parent = this };

            var font = new Font(Config.FontName, 20, FontStyle.Regular);

            MainLabel.Location = new Point(1, 1);
            //MainLabel.Size = new Size(width / 3, Size.Height);
            MainLabel.Font = font;
            MainLabel.BackColour = Color.Empty;

            MidLabel = new DXLabel { Parent = this };
            MidLabel.Location = new Point((width / 3) + 15, 8);
            //MidLabel.Size = new Size(width / 3, Size.Height);
            MidLabel.Font = new Font(Config.FontName, 14, FontStyle.Regular); ;
            MidLabel.BackColour = Color.Empty;
            MidLabel.ForeColour = Color.FromArgb(0, 250, 250, 250);


            LastLabel = new DXLabel { Parent = this };
            LastLabel.Location = new Point((width / 2) + 12, 1);
            //LastLabel.Size = new Size(width / 3, Size.Height);
            LastLabel.Font = font;
            LastLabel.BackColour = Color.Empty;
            LastLabel.ForeColour = Color.FromArgb(0, 87, 79, 254);

            _timer = new Timer();
            //设置时间间隔（毫秒为单位）
            _timer.Interval = 3000;
            _timer.Enabled = true;
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Visible = false;
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        /// <param name="msg3"></param>
        /// <param name="isSafe"></param>
        public void ShowTip(string msg1, string msg2, string msg3, bool isSafe = false)
        {
            if (this.Visible)
            {
                _timer.Stop();
            }
            _timer.Start();
            this.Visible = true;
            MainLabel.Text = msg1;
            MainLabel.ForeColour = isSafe ? Color.FromArgb(0, 10, 165, 100) : Color.FromArgb(0, 35, 75, 120);
            MidLabel.Text = msg2;
            LastLabel.Text = msg3;
        }
    }
}