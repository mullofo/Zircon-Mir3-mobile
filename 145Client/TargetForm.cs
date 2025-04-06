using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.Scenes;
using Library;
using SharpDX.Windows;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace Client
{
    public sealed class TargetForm : RenderForm  //目标窗体 呈现形式
    {
        public bool Resizing { get; private set; }
        public TargetForm() : base(NetworkConfig.ClientName)
        {
            AutoScaleMode = AutoScaleMode.None;//自动缩放模式禁用

            AutoScaleDimensions = new SizeF(96F, 96F);  //自动缩放的尺寸

            ClientSize = new Size(1024, 768);   //客户端显示的大小

            Icon = Properties.Resources.Mir3Game;   //客户端图标

            FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedSingle;  //窗体边框样式

            MaximizeBox = false;  //最大化视窗关闭

            BackColor = Color.Black;
        }

        protected override void OnDeactivate(EventArgs e)  //关闭时
        {
            if (GameScene.Game != null)
                GameScene.Game.MapControl.MapButtons = MouseButtons.None;

            CEnvir.Shift = false;
            CEnvir.Alt = false;
            CEnvir.Ctrl = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)  //鼠标移动时
        {
            if (Config.ClipMouse && Focused)
                Cursor.Clip = RectangleToScreen(ClientRectangle);
            else
                Cursor.Clip = Rectangle.Empty;

            CEnvir.MouseLocation = e.Location;

            try
            {
                DXControl.ActiveScene?.OnMouseMove(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (GameScene.Game != null && e.Button == MouseButtons.Right && (GameScene.Game.SelectedCell != null || GameScene.Game.GoldPickedUp))
            {
                GameScene.Game.SelectedCell = null;
                GameScene.Game.GoldPickedUp = false;
                return;
            }

            try
            {
                DXControl.ActiveScene?.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {

            if (GameScene.Game != null)
                GameScene.Game.MapControl.MapButtons &= ~e.Button;

            try
            {
                DXControl.ActiveScene?.OnMouseUp(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnMouseClick(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e)  //鼠标双击时
        {
            try
            {
                DXControl.ActiveScene?.OnMouseClick(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnMouseWheel(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            CEnvir.Shift = e.Shift;
            CEnvir.Alt = e.Alt;
            CEnvir.Ctrl = e.Control;

            try
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    DXManager.ToggleFullScreen();
                    return;
                }

                DXControl.ActiveScene?.OnKeyDown(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            CEnvir.Shift = e.Shift;
            CEnvir.Alt = e.Alt;
            CEnvir.Ctrl = e.Control;

            if (e.KeyCode == Keys.Pause)
                CreateScreenShot();

            try
            {
                DXControl.ActiveScene?.OnKeyUp(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnKeyPress(e);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        public void ChaneSize(Size size)
        {
            if (ClientSize == size) return;

            ClientSize = size;
        }

        public static void CreateScreenShot()  //屏幕截图
        {
            Bitmap image = CEnvir.Target.GetImage();

            using (Graphics graphics = Graphics.FromImage(image))
            {
                string text = $"日期: {CEnvir.Now.ToShortDateString()}{Environment.NewLine}";
                text += $"时间: {CEnvir.Now.TimeOfDay:hh\\:mm\\:ss}{Environment.NewLine}";
                if (GameScene.Game != null)
                    text += $"玩家: {MapObject.User.Name}{Environment.NewLine}";

                using (Font font = new Font(Config.FontName, CEnvir.FontSize(9F)))
                {
                    graphics.DrawString(text, font, Brushes.Black, 3, 33);
                    graphics.DrawString(text, font, Brushes.Black, 4, 32);
                    graphics.DrawString(text, font, Brushes.Black, 5, 33);
                    graphics.DrawString(text, font, Brushes.Black, 4, 34);
                    graphics.DrawString(text, font, Brushes.White, 4, 33);
                }
            }

            string path = Path.Combine(Application.StartupPath, @"Screenshots\");  //保存路径
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            int count = Directory.GetFiles(path, "*.png").Length;  //保存图片为PNG格式
            string fileName = $"图片 {count}.png";

            image.Save(Path.Combine(path, fileName), ImageFormat.Png);
            image.Dispose();

            if (GameScene.Game != null)
                GameScene.Game.ReceiveChat("屏幕截图： " + fileName, MessageType.System);
        }

        #region ScreenCapture

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr handle);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr handle);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr handle, int width, int height);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr handle, IntPtr handle2);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr handle, int destX, int desty, int width, int height,
                                         IntPtr handle2, int sourX, int sourY, int flag);
        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr handle, IntPtr handle2);
        [DllImport("gdi32.dll")]
        public static extern int DeleteObject(IntPtr handle);

        public Bitmap GetImage()  //获取图像
        {
            Point location = PointToClient(Location);

            location = new Point(-location.X, -location.Y);

            Rectangle r = new Rectangle(location, ClientSize);


            IntPtr sourceDc = GetWindowDC(Handle);
            IntPtr destDc = CreateCompatibleDC(sourceDc);

            IntPtr hBmp = CreateCompatibleBitmap(sourceDc, r.Width, r.Height);
            if (hBmp != IntPtr.Zero)
            {
                IntPtr hOldBmp = SelectObject(destDc, hBmp);
                BitBlt(destDc, 0, 0, r.Width, r.Height, sourceDc, r.X, r.Y, 0xCC0020); //0, 0, 13369376);
                SelectObject(destDc, hOldBmp);
                DeleteDC(destDc);
                ReleaseDC(Handle, sourceDc);

                Bitmap bmp = Image.FromHbitmap(hBmp);

                DeleteObject(hBmp);

                return bmp;
            }

            return null;
        }

        #endregion

        private void InitializeComponent() //初始化组件
        {
            this.SuspendLayout();
            // 
            // 目标形态
            // 
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "TargetForm";
            this.Load += new System.EventHandler(this.TargetForm_Load);
            this.ResumeLayout(false);

        }

        private void TargetForm_Load(object sender, EventArgs e)
        {

        }
    }

}
