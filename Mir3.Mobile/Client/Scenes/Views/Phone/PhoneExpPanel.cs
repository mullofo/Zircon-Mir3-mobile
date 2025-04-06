using Client.Controls;
using Client.Envir;
using Client.Models;
using Library;
using MonoGame.Extended;
using System;
using System.Drawing;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// UI底部主面板
    /// </summary>
    public sealed class PhoneExpPanel : DXControl
    {
        #region Properties

        private DXControl ExpValueControl;
        public DXLabel ExpLabel;
        public DXLabel GridLabel;
        private DXLabel FPSLabel, PINLabel;
        private DateTime FPSTime;

        public DXImageControl BatteryCurrentStatus, NetworkCurrentStatus;
        public DXLabel CurrentTime;
        #endregion

        /// <summary>
        /// 底部UI界面左边框
        /// </summary>
        public PhoneExpPanel()
        {
            DrawTexture = false;
            Size = new Size(CEnvir.GameSize.Width - 1, 25);
            IsControl = false;

            var dXControl = new DXControl
            {
                Parent = this,
                DrawTexture = true,
                Size = new Size(CEnvir.GameSize.Width - 1, 7),
                BackColour = Color.FromArgb(28, 16, 17),
                Border = true,
                BorderColour = Color.FromArgb(191, 134, 61),
                IsControl = false,
            };
            dXControl.Location = new Point(0, Size.Height - dXControl.Size.Height);

            ExpValueControl = new DXControl  //经验值槽
            {
                Parent = dXControl,
                DrawTexture = true,
                BackColour = Color.FromArgb(195, 109, 9),
                Location = new Point(1, 2),
                Size = new Size(1, 1),
                IsControl = false,
            };
            ExpValueControl.BeforeDraw += (o, e) =>  //在经验槽底图前面绘制
            {
                if (MapObject.User.MaxExperience == 0)
                {
                    ExpValueControl.Size = Size.Empty;
                    return;  //如果 最大经验值为零 返回
                }

                //浮动百分比 = 浮动数字最小值（1，最大值（0，玩家经验值/最大经验值））
                float percent = (float)Math.Min(1, Math.Max(0, MapObject.User.Experience / MapObject.User.MaxExperience));

                if (percent == 0)
                {
                    ExpValueControl.Size = new Size(1, 1);
                    return;
                }
                ExpValueControl.Size = new Size((int)Math.Round((Size.Width - 2) * percent), 2);

            };

            GridLabel = new DXLabel               //坐标颜色字体
            {
                Parent = this,
                AutoSize = false,
                //ForeColour = Color.FromArgb(255, 255, 200),
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(150, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
            };
            GridLabel.Location = new Point(30, 0);

            FPSLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Size = new Size(65, 20),
                AutoSize = false,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(200, 0),
                IsControl = false,
            };

            PINLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Size = new Size(65, 20),
                AutoSize = false,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(270, 0),
                IsControl = false,
            };

            ExpLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "000.00%",
                IsControl = false,
            };
            ExpLabel.Location = new Point((Size.Width - ExpLabel.Size.Width) / 2, Size.Height - ExpLabel.Size.Height);

            CurrentTime = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Text = Time.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                IsControl = false,
            };
            CurrentTime.Location = new Point(Size.Width - CurrentTime.Size.Width - 30, 0);

            BatteryCurrentStatus = new DXImageControl
            {
                Parent = this,
                DrawTexture = false,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1151,
                IsControl = false,
            };
            BatteryCurrentStatus.Location = new Point(CurrentTime.Location.X - BatteryCurrentStatus.Size.Width - 20, 1);
            BatteryCurrentStatus.AfterDraw += (o, e) =>
            {
                if (BatteryCurrentStatus.Library == null) return;

                if (CEnvir.BatteryLevel == 0) return;

                float percent = Math.Min(1, Math.Max(0, (float)CEnvir.BatteryLevel / 100));

                if (percent == 0) return;

                Size image = BatteryCurrentStatus.Library.GetSize(1150);

                if (image == Size.Empty) return;

                int offset = (int)(image.Width * (1.0 - percent));
                Rectangle area = new Rectangle(0, 0, image.Width - offset, image.Height);

                //Color color = percent <= 0.1 ? Color.Red : percent <= 0.2 ? Color.Yellow : Color.Green;
                BatteryCurrentStatus.Library.Draw(1150, BatteryCurrentStatus.DisplayArea.X + 2, BatteryCurrentStatus.DisplayArea.Y + 3, Color.White, area, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            };

            NetworkCurrentStatus = new DXImageControl
            {
                Parent = this,
                DrawTexture = false,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1153,
                IsControl = false,
            };
            NetworkCurrentStatus.Location = new Point(BatteryCurrentStatus.Location.X - NetworkCurrentStatus.Size.Width - 20, 0);
            NetworkCurrentStatus.AfterDraw += (o, e) =>
            {
                if (NetworkCurrentStatus.Library == null) return;

                if (CEnvir.Connection?.Ping > 500) return;

                float percent = 0;
                if (CEnvir.Connection?.Ping <= 30)
                    percent = 1f;
                else if (CEnvir.Connection?.Ping <= 50)
                    percent = 0.8f;
                else if (CEnvir.Connection?.Ping <= 100)
                    percent = 0.6f;
                else if (CEnvir.Connection?.Ping <= 200)
                    percent = 0.4f;
                else if (CEnvir.Connection?.Ping <= 500)
                    percent = 0.2f;

                if (percent == 0) return;

                Size image = NetworkCurrentStatus.Library.GetSize(1152);

                if (image == Size.Empty) return;

                int offset = (int)(image.Width * (1.0 - percent));
                Rectangle area = new Rectangle(0, 0, image.Width - offset, image.Height);
                NetworkCurrentStatus.Library.Draw(1152, NetworkCurrentStatus.DisplayArea.X, NetworkCurrentStatus.DisplayArea.Y, Color.White, area, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            };
        }

        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (Time.Now >= FPSTime)
            {
                FPSLabel.Text = $"FPS:" + $"{CEnvir.FPSCount}";
                PINLabel.Text = $"PIN:" + $"{CEnvir.Connection?.Ping}";
                CurrentTime.Text = Text = Time.Now.ToString("yyyy/MM/dd HH:mm:ss");
                FPSTime = Time.Now.AddSeconds(1);
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ExpValueControl != null)
                {
                    if (!ExpValueControl.IsDisposed)
                        ExpValueControl.Dispose();

                    ExpValueControl = null;
                }

                if (ExpLabel != null)
                {
                    if (!ExpLabel.IsDisposed)
                        ExpLabel.Dispose();

                    ExpLabel = null;
                }

                if (GridLabel != null)
                {
                    if (!GridLabel.IsDisposed)
                        GridLabel.Dispose();

                    GridLabel = null;
                }

                if (FPSLabel != null)
                {
                    if (!FPSLabel.IsDisposed)
                        FPSLabel.Dispose();

                    FPSLabel = null;
                }

                if (PINLabel != null)
                {
                    if (!PINLabel.IsDisposed)
                        PINLabel.Dispose();

                    PINLabel = null;
                }

                if (BatteryCurrentStatus != null)
                {
                    if (!BatteryCurrentStatus.IsDisposed)
                        BatteryCurrentStatus.Dispose();

                    BatteryCurrentStatus = null;
                }

                if (NetworkCurrentStatus != null)
                {
                    if (!NetworkCurrentStatus.IsDisposed)
                        NetworkCurrentStatus.Dispose();

                    NetworkCurrentStatus = null;
                }

                if (CurrentTime != null)
                {
                    if (!CurrentTime.IsDisposed)
                        CurrentTime.Dispose();

                    CurrentTime = null;
                }

                FPSTime = DateTime.MinValue;

            }
        }
        #endregion

    }
}
