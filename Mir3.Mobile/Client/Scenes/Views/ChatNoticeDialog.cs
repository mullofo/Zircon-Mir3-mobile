using Client.Controls;
using Client.Envir;
using Library;
using MonoGame.Extended;
using System;
using System.Drawing;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 公告消息通知框
    /// </summary>
    public sealed class ChatNoticeDialog : DXImageControl
    {
        public DXImageControl Layout;
        public DXLabel TextLabel1, TextLabel2;
        public DateTime CurrentTime;
        public int ViewTime;
        public long speed;
        public string str;

        /// <summary>
        /// 公告信息通知框
        /// </summary>
        public ChatNoticeDialog()
        {
            Index = 6911;
            LibraryFile = LibraryFile.GameInter;
            Movable = false;
            Sort = false;
            //Opacity = 0.7F;
            ImageOpacity = 0.3F;
            PassThrough = true;

            TextLabel1 = new DXLabel
            {
                Text = "",
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),             //字体 格式  文字大小
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,   //绘图格式  文本格式标志 垂直中心  水平居中
                Parent = this,
                IsControl = true,                          //能控制
                Location = new Point(15, 6),
                Size = new Size(456, 32),
                ForeColour = Color.Yellow,        //前色
                BorderColour = Color.Black,       //边框颜色               
            };

            TextLabel2 = new DXLabel
            {
                Text = "",
                Font = new Font(Config.FontName, CEnvir.FontSize(15F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = true,
                Location = new Point(15, 6),
                Size = new Size(660, 40),
                ForeColour = Color.Yellow,
                BorderColour = Color.Black,
            };

            Layout = new DXImageControl  //底图
            {
                Index = 327,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(-2, -2),
                Parent = this
            };

            AfterDraw += ChatNotice_AfterDraw;            //显示文字的形式，走马灯
        }
        /// <summary>
        /// 公告信息后绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatNotice_AfterDraw(object sender, EventArgs e)
        {
            if (CurrentTime < CEnvir.Now)           //如果定义时间小于主时间
            {
                Hide();                  //隐藏
            }
            else
            {
                if (CEnvir.Now.Ticks - speed > 80000)
                {
                    speed = CEnvir.Now.Ticks;
                    if (TextLabel1.Text.Length < 100)
                    {
                        TextLabel1.Text += "   " + str;
                        TextLabel2.Text += "   " + str;
                    }

                    TextLabel1.Location = TextLabel2.Location = new Point(TextLabel2.Location.X - 1, 6);
                }
            }
        }
        /// <summary>
        /// 显示公告
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        public void ShowNotice(string text, int type = 0)
        {
            Index = type == 0 ? 6911 : 6913;
            Layout.Index = type == 0 ? 327 : 6912;
            str = TextLabel1.Text = TextLabel2.Text = text;
            TextLabel1.Visible = type == 0;
            TextLabel2.Visible = type == 1;
            TextLabel1.Location = TextLabel2.Location = new Point(15, 6);
            CurrentTime = CEnvir.Now.AddMilliseconds(12000);         //定义当前时间=主时间+查看时间
            speed = CEnvir.Now.Ticks;
            Show();
        }
        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TextLabel1 != null)
                {
                    if (!TextLabel1.IsDisposed)
                        TextLabel1.Dispose();

                    TextLabel1 = null;
                }

                if (TextLabel2 != null)
                {
                    if (!TextLabel2.IsDisposed)
                        TextLabel2.Dispose();

                    TextLabel2 = null;
                }

                if (Layout != null)
                {
                    if (!Layout.IsDisposed)
                        Layout.Dispose();

                    Layout = null;
                }
            }
        }
        #endregion
    }
}