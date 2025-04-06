using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Library;
using System;
using System.Drawing;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 钓鱼功能
    /// </summary>
    public sealed class FishingDialog : DXImageControl
    {
        public DXButton CloseButton;       //关闭按钮
        public DXItemCell[] FishingGrid;   //钓鱼道具格子


        /// <summary>
        /// 钓鱼功能界面
        /// </summary>
        public FishingDialog()
        {
            Index = 4600;
            LibraryFile = LibraryFile.GameInter;
            Movable = true;     //可移动为true
            Sort = true;        //排序为true
            IgnoreMoveBounds = true;

            CloseButton = new DXButton           //关闭按钮
            {
                Index = 113,
                Location = new Point(190, 0),
                LibraryFile = LibraryFile.Interface,
                Parent = this,
            };
            CloseButton.MouseClick += (o, e) => Hide();  //关闭按钮 鼠标点击 += 隐藏

            FishingGrid = new DXItemCell[Globals.FishingEquipmentSize];   //钓鱼道具格子 = 新的 道具格子
            DXItemCell cell;
            FishingGrid[(int)FishingSlot.Hook] = cell = new DXItemCell   //鱼钩
            {
                Slot = (int)FishingSlot.Hook,
                ItemGrid = GameScene.Game.FishingEquipment,
                GridType = GridType.FishingEquipment,
                Parent = this,
                Location = new Point(13, 170),
                Hint = "鱼钩".Lang(),
            };
            FishingGrid[(int)FishingSlot.Float] = cell = new DXItemCell  //鱼漂
            {
                Slot = (int)FishingSlot.Float,
                ItemGrid = GameScene.Game.FishingEquipment,
                GridType = GridType.FishingEquipment,
                Parent = this,
                Location = new Point(13, 210),
                Hint = "鱼漂".Lang(),
            };

            FishingGrid[(int)FishingSlot.Bait] = cell = new DXItemCell   //鱼饵
            {
                Slot = (int)FishingSlot.Bait,
                ItemGrid = GameScene.Game.FishingEquipment,
                GridType = GridType.FishingEquipment,
                Parent = this,
                Location = new Point(53, 210),
                Hint = "鱼饵".Lang(),
            };

            FishingGrid[(int)FishingSlot.Finder] = cell = new DXItemCell   //探鱼器
            {
                Slot = (int)FishingSlot.Finder,
                ItemGrid = GameScene.Game.FishingEquipment,
                GridType = GridType.FishingEquipment,
                Parent = this,
                Location = new Point(93, 210),
                Hint = "探鱼器".Lang(),
            };

            FishingGrid[(int)FishingSlot.Reel] = cell = new DXItemCell   //摇轮
            {
                Slot = (int)FishingSlot.Reel,
                ItemGrid = GameScene.Game.FishingEquipment,
                GridType = GridType.FishingEquipment,
                Parent = this,
                Location = new Point(133, 210),
                Hint = "摇轮".Lang(),
            };
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (Visible) return;

            if (!GameScene.Game.User.HasFishingRod)  //判断是否装备钓鱼杆  Shape值对应 125  126
            {
                DXConfirmWindow.Show("你没有装备钓鱼竿".Lang());
                return;
            }
            Visible = true;
        }

        public void ProcessFishingStarted(S.FishingStarted p)
        {
            GameScene.Game.FishingStatusBox.Visible = true;
            //进度条
            GameScene.Game.FishingStatusBox.ChangePercent(p.FindingChance);
            GameScene.Game.User.FishingStartTime = p.StartTime;
            GameScene.Game.User.FishingEndTime = p.EndTime;
            GameScene.Game.User.FishingPerfectTime = p.PerfectTime;
            GameScene.Game.User.IsFishing = true;
            GameScene.Game.FishingStatusBox.blink.Visible = false;
            GameScene.Game.FishingStatusBox.ShowRipple();
        }

        public void ProcessFishingEnded()
        {
            GameScene.Game.FishingStatusBox.Visible = false;
            GameScene.Game.User.ResetFishing();
            //if (GameScene.Game.User.ActionQueue.Count > 0 && GameScene.Game.User.ActionQueue[0].Action == MirAction.FishingWait)
            //    GameScene.Game.User.ActionQueue[0] = new ObjectAction(MirAction.FishingReel, GameScene.Game.User.Direction, GameScene.Game.User.CurrentLocation, MagicType.None);
            //else GameScene.Game.User.ActionQueue.Add(new ObjectAction(MirAction.FishingReel, GameScene.Game.User.Direction, GameScene.Game.User.CurrentLocation, MagicType.None));
            GameScene.Game.ReceiveChat("钓鱼结束".Lang(), MessageType.Hint);
            GameScene.Game.FishingStatusBox.blink.Visible = false;
            GameScene.Game.FishingStatusBox.HideRipple();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            if (CloseButton != null)
            {
                if (!CloseButton.IsDisposed)
                {
                    CloseButton.Dispose();
                }
                CloseButton = null;
            }

            if (FishingGrid != null)
            {
                for (int i = 0; i < FishingGrid.Length; i++)
                {
                    if (FishingGrid[i] != null)
                    {
                        if (!FishingGrid[i].IsDisposed)
                            FishingGrid[i].Dispose();

                        FishingGrid[i] = null;
                    }
                }
                FishingGrid = null;
            }
        }
    }

    /// <summary>
    /// 角色头顶上的图标
    /// </summary>
    public sealed class FishingStatusIcon : DXControl
    {
        private DXImageControl iconbase, iconRing1, iconRing2;
        DXControl back, back2;
        public DXAnimatedControl blink;
        public MirEffect ripple, cork;

        Point _iconPoint = new Point(44, 43);
        public FishingStatusIcon()
        {
            Size = new Size(44 * 3, 43 * 3);

            MouseClick += (sender, args) => CEnvir.Enqueue(new C.FishingReel());

            iconbase = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4500,
                Parent = this,
                Location = _iconPoint
            };
            //iconbase.MouseClick += (sender, args) => CEnvir.Enqueue(new C.FishingReel());

            back = new DXControl
            {
                Parent = this,
                Size = new Size(44, 43),
                Location = _iconPoint,
                PassThrough = true
            };

            back2 = new DXControl
            {
                Parent = this,
                Size = new Size(44, 43),
                Location = _iconPoint,
                Visible = false,
                PassThrough = true
            };

            iconRing1 = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4501,
                Parent = back,
                PassThrough = true
            };

            iconRing2 = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4501,
                Parent = back2,
                PassThrough = true
            };

            blink = new DXAnimatedControl
            {
                Parent = this,
                Loop = true,
                LibraryFile = LibraryFile.GameInter,
                BaseIndex = 4510,
                FrameCount = 3,
                AnimationDelay = TimeSpan.FromMilliseconds(500),
                Visible = false,
                PassThrough = false
            };
            blink.Location = new Point(50, 50);
            blink.MouseClick += (sender, args) => CEnvir.Enqueue(new C.FishingReel());
        }

        public void ShowRipple()
        {
            ripple = new MirEffect(1420, 6, TimeSpan.FromMilliseconds(200), LibraryFile.MagicEx5, 50, 60, Globals.NoneColour)
            {
                MapTarget = GameScene.Game.MapControl.FishingCellPoint,
                Blend = true,
                Loop = true,
                DrawType = DrawType.Final,
                AdditionalOffSet = new Point(0, 0) //这里可以微调水波的位置
            };
            ripple.Process();
            cork = new MirEffect(1430, 6, TimeSpan.FromMilliseconds(200), LibraryFile.MagicEx5, 50, 60, Globals.NoneColour)
            {
                MapTarget = GameScene.Game.MapControl.FishingCellPoint,
                Blend = true,
                Loop = true,
                DrawType = DrawType.Final,
                AdditionalOffSet = new Point(0, 0) //这里可以微调鱼漂的位置
            };
            cork.Process();
        }

        public void HideRipple()
        {
            ripple?.Remove();
            cork?.Remove();
            ripple = null;
            cork = null;
        }

        public void ChangePercent(int per)
        {
            var offset = per <= 50 ? 42 * (per * 2 / 100f) : 42 * ((per - 50) * 2 / 100f);
            if (offset < 8)
            {
                offset = 8;
            }
            if (per <= 50)
            {
                back2.Visible = false;
                iconRing1.Location = new Point(-22, (int)(42 - offset));
                back.Location = new Point(_iconPoint.X - iconRing1.Location.X, _iconPoint.Y - iconRing1.Location.Y);

            }
            else if (per <= 100)
            {
                iconRing1.Location = new Point(-22, 0);
                back.Location = new Point(_iconPoint.X - iconRing1.Location.X, _iconPoint.Y - iconRing1.Location.Y);

                back2.Visible = true;
                iconRing2.Location = new Point(22, -(int)(42 - offset));
                back2.Location = new Point(_iconPoint.X - iconRing2.Location.X, _iconPoint.Y - iconRing2.Location.Y);
            }
            else
            {
                back2.Visible = false;
                back.Location = Location;
                iconRing1.Location = new Point(0, 0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                return;
            }
            if (iconbase != null)
            {
                if (!iconbase.IsDisposed)
                {
                    iconbase.Dispose();
                }
                iconbase = null;
            }
            if (iconRing1 != null)
            {
                if (!iconRing1.IsDisposed)
                {
                    iconRing1.Dispose();
                }
                iconRing1 = null;
            }
            if (iconRing2 != null)
            {
                if (!iconRing2.IsDisposed)
                {
                    iconRing2.Dispose();
                }
                iconRing2 = null;
            }
            if (back != null)
            {
                if (!back.IsDisposed)
                {
                    back.Dispose();
                }
                back = null;
            }
            if (back2 != null)
            {
                if (!back2.IsDisposed)
                {
                    back2.Dispose();
                }
                back2 = null;
            }
            if (blink != null)
            {
                if (!blink.IsDisposed)
                {
                    blink.Dispose();
                }
                blink = null;
            }
        }
    }
}
