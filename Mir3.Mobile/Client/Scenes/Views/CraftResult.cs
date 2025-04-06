using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using System;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 制作进度界面
    /// </summary>
    public sealed class CraftResult : DXWindow
    {
        #region Properties

        private DXImageControl CraftResultBackGround;
        private DXItemCell TargetItemCell;
        private DXButton CompleateButton, CancelButton;
        private DXLabel ProgressText, RemainingTimeText;

        public override WindowType Type => WindowType.CraftBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 制作进度界面
        /// </summary>
        public CraftResult()
        {
            Size = new Size(291, 196);

            CraftResultBackGround = new DXImageControl  //制作进度面板
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3770,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            TargetItemCell = new DXItemCell  //目标道具单元格
            {
                Parent = this,
                Location = new Point(30, 60),
                //FixedBorder = true,
                //Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                //FixedBorderColour = true,
                Visible = true,
            };

            CompleateButton = new DXButton  //制作完成按钮
            {
                Index = 3797,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(105, 155),
                Parent = this,
                Visible = true,
            };
            CompleateButton.MouseClick += (o, e) => Visible = false;

            CancelButton = new DXButton
            {
                Index = 3792,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(105, 155),
                Parent = this,
                Hint = "取消制作不会返还已扣除的材料".Lang(),
                Visible = false,
            };
            CancelButton.MouseClick += (sender, args) =>
            {
                CEnvir.Enqueue(new C.CraftCancel());
            };

            ProgressText = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(90, 70),
                Text = "当前没有正在制作的物品".Lang(),
                Visible = true,
            };

            RemainingTimeText = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.LightYellow,
                Outline = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "00:00:00",
                Location = new Point(120, 130)
            };

            RemainingTimeText.BeforeDraw += (sender, args) =>
            {
                if (GameScene.Game.User == null) return;
                CraftItemInfo target = GameScene.Game.User.CraftingItem;
                if (target == null) return;

                DateTime finishDateTime = GameScene.Game.User.CraftFinishTime; //完成时间
                DateTime startDateTime = finishDateTime.Subtract(new TimeSpan(0, 0, target.TimeCost)); //开始制造的时间

                TimeSpan remainingTime = finishDateTime.Subtract(DateTime.Now) + TimeSpan.FromSeconds(1); //剩余时间
                if (DateTime.Compare(DateTime.Now, finishDateTime) < 0)
                {
                    RemainingTimeText.Text = remainingTime.ToString(@"hh\:mm\:ss");
                    ProgressText.Text = "正在制作提交的物品".Lang();
                    CompleateButton.Visible = false;
                    CancelButton.Visible = true;
                }
                else
                {
                    RemainingTimeText.Text = "00:00:00";
                    ProgressText.Text = "当前没有正在制作的物品".Lang();
                    CompleateButton.Visible = true;
                    CancelButton.Visible = false;
                }
                RemainingTimeText.Location = new Point(CraftResultBackGround.DisplayArea.Width / 2 - RemainingTimeText.DisplayArea.Width / 2 - 4, 130);

                //进度条调整宽度
                double elapsedSeconds = (DateTime.Now - startDateTime).TotalSeconds;
                if (elapsedSeconds < 0) return;
                elapsedSeconds = Math.Max(0, elapsedSeconds);
                double percent = Math.Min(1, elapsedSeconds / Math.Max(1, target.TimeCost));

                //画进度条
                MirImage image = CraftResultBackGround.Library.CreateImage(percent >= 0.999 ? 3780 : 3784, ImageType.Image);
                if (image == null) return;
                PresentMirImage(image.Image, this, new Rectangle(CraftResultBackGround.DisplayArea.X + 25, CraftResultBackGround.DisplayArea.Y + 115, (int)(image.Width * percent), image.Height),
                    Color.White, RemainingTimeText, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            };
        }
        /// <summary>
        /// 更新制作目标
        /// </summary>
        /// <param name="item"></param>
        public void UpdateCraftTarget(CraftItemInfo item)
        {
            if (item == null) return;
            TargetItemCell.Item = new ClientUserItem(item.Item, item.TargetAmount);
            TargetItemCell.RefreshItem();
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CraftResultBackGround != null)
                {
                    if (!CraftResultBackGround.IsDisposed)
                        CraftResultBackGround.Dispose();

                    CraftResultBackGround = null;
                }
                if (TargetItemCell != null)
                {
                    if (!TargetItemCell.IsDisposed)
                        TargetItemCell.Dispose();

                    TargetItemCell = null;
                }
                if (CompleateButton != null)
                {
                    if (!CompleateButton.IsDisposed)
                        CompleateButton.Dispose();

                    CompleateButton = null;
                }
                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }
                if (ProgressText != null)
                {
                    if (!ProgressText.IsDisposed)
                        ProgressText.Dispose();

                    ProgressText = null;
                }
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
