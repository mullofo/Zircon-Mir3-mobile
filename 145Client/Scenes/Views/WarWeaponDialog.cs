using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 攻城兵器界面
    /// </summary>
    public sealed class WarWeaponDialog : DXWindow
    {
        public DXImageControl WarWeaponBackGround;
        public DXButton Close1Button, AttackButton, WaitButton, MoveButton;
        public DXLabel SurplusAmmunition, TotalAmmunition;
        public WarMapControl Map;
        public DXControl panel;

        public int PaodanCount;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        /// <summary>
        /// 攻城兵器界面
        /// </summary>
        public WarWeaponDialog()
        {
            HasTitle = false;                          //不显示标题
            HasFooter = false;                         //不显示页脚
            HasTopBorder = false;                      //不显示边框
            TitleLabel.Visible = false;                //标题标签不用
            IgnoreMoveBounds = true;
            Border = false;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = UI1Library.GetSize(1590);

            WarWeaponBackGround = new DXImageControl      //背包背景图
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1590,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(154, 239),
                Hint = "关闭",
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            panel = new DXControl  //血条面板
            {
                Size = new Size(164, 7),
                Location = new Point(15, 45),
                Border = false,
                Parent = this,
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };
            panel.AfterDraw += (o, e) =>   //画血条
            {
                if (GameScene.Game.MapControl.Objects != null)
                {
                    if (Map?.PetData != null)
                    {
                        if (CEnvir.LibraryList.TryGetValue(LibraryFile.UI1, out MirLibrary lib))
                        {
                            var percent = Math.Min(1, Math.Max(0, Map.PetData.Health / (float)Map.PetData.MaxHealth));
                            MirImage image = lib.CreateImage(1598, ImageType.Image);
                            if (image != null)
                                PresentTexture(image.Image, this, new Rectangle(panel.DisplayArea.X, panel.DisplayArea.Y, (int)(image.Width * percent), image.Height), Color.White, panel);
                        }
                    }
                }
            };

            AttackButton = new DXButton
            {
                Parent = this,
                Index = 1592,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(17, 229),
                Hint = "攻击",
            };
            AttackButton.MouseClick += (s, e) =>
            {
                CEnvir.Enqueue(new C.WarWeapAttackCoordinates { X = Map.AttackX, Y = Map.AttackY });
            };

            WaitButton = new DXButton
            {
                Parent = this,
                Index = 1594,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(67, 229),
                Hint = "等待",
            };
            WaitButton.MouseClick += (s, e) =>
            {
                CEnvir.Enqueue(new C.WarWeapAttackStop { });
            };

            MoveButton = new DXButton
            {
                Parent = this,
                Index = 1596,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(117, 229),
                Hint = "移动",
            };
            MoveButton.MouseClick += (s, e) =>
            {
                CEnvir.Enqueue(new C.WarWeapMove { });
            };

            SurplusAmmunition = new DXLabel  //剩余弹药数量
            {
                Parent = this,
                Text = "100",
                Location = new Point(15, 18),
                AutoSize = false,
                Size = new Size(80, 18),
                //Border = true,
                //BackColour = Color.Red,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            TotalAmmunition = new DXLabel  //弹药总数
            {
                Parent = this,
                Text = "300",
                Location = new Point(102, 18),
                AutoSize = false,
                Size = new Size(80, 18),
                //Border = true,
                //BackColour = Color.Red,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            Map = new WarMapControl  //绘制地图
            {
                Parent = this,
                Location = new Point(7, 66),
                Size = new Size(180, 147),
                PassThrough = true,
            };
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (GameScene.Game.Observer) return;

            DXItemCell cell = DXItemCell.SelectedCell;
            if (cell != null)
            {
                if (PaodanCount >= 300) return;
                if (cell.Item.Count > 1)
                {
                    new DXInputWindow(cell.Item, (amount) =>
                    {
                        if (amount + PaodanCount > 300) amount = 300 - PaodanCount;
                        if (amount <= 0 || amount > cell.Item.Count) return;

                        CEnvir.Enqueue(new C.Ammunition { Grid = cell.GridType, Slot = cell.Slot, Count = amount });

                    }, cell.Item.Count, "你要装填几个?".Lang());

                    DXItemCell.SelectedCell = null;
                }
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (WarWeaponBackGround != null)
                {
                    if (!WarWeaponBackGround.IsDisposed)
                        WarWeaponBackGround.Dispose();

                    WarWeaponBackGround = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }

                if (panel != null)
                {
                    if (!panel.IsDisposed)
                        panel.Dispose();

                    panel = null;
                }

                if (AttackButton != null)
                {
                    if (!AttackButton.IsDisposed)
                        AttackButton.Dispose();

                    AttackButton = null;
                }

                if (WaitButton != null)
                {
                    if (!WaitButton.IsDisposed)
                        WaitButton.Dispose();

                    WaitButton = null;
                }

                if (MoveButton != null)
                {
                    if (!MoveButton.IsDisposed)
                        MoveButton.Dispose();

                    MoveButton = null;
                }

                if (SurplusAmmunition != null)
                {
                    if (!SurplusAmmunition.IsDisposed)
                        SurplusAmmunition.Dispose();

                    SurplusAmmunition = null;
                }

                if (TotalAmmunition != null)
                {
                    if (!TotalAmmunition.IsDisposed)
                        TotalAmmunition.Dispose();

                    TotalAmmunition = null;
                }

                if (Map != null)
                {
                    if (!Map.IsDisposed)
                        Map.Dispose();

                    Map = null;
                }
            }
        }
        #endregion
    }

    public sealed class WarMapControl : DXControl
    {

        public void UpdatePetLoaction(Point location)
        {
            if (PetData != null)
            {
                PetData.Location = location;
                Update(PetData);
            }
        }
        public ClientObjectData PetData;

        public DXImageControl MapImage, TargetFlag;

        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();
        public static float ScaleX, ScaleY;

        public DXLabel OuterFrame, InnerFrame;

        public DXControl MapImageBackGround;

        public int AttackX, AttackY;

        /// <summary>
        /// 鼠标进入对话框显示图标指针
        /// </summary>
        private bool MouseInDialog => TargetFlag.Visible;

        public WarMapControl()
        {
            MapImage = new DXImageControl  //小地图
            {
                Parent = this,
                LibraryFile = LibraryFile.MiniMap,
                ImageOpacity = 0.85F,
                Movable = false,  //可移动
                IgnoreMoveBounds = true,  //忽略移动边界
                PassThrough = true,
                ForeColour = Color.FromArgb(150, 150, 150)
            };

            MapImageBackGround = new DXControl
            {
                Parent = this,
                PassThrough = true,
                Size = new Size(180, 145),
                Location = new Point(0, 0),
            };

            InnerFrame = new DXLabel //画内框
            {
                Parent = this,
                Border = true,
                BorderColour = Color.Pink,
                Size = new Size(110, 70),
                Location = new Point(35, 38),
            };
            //InnerFrame.MouseEnter += (s, e) =>   //鼠标进入
            //{
            //    TargetFlag.Visible = false;

            //};
            //InnerFrame.MouseLeave += (s, e) =>   //鼠标离开时
            //{
            //    TargetFlag.Visible = true;
            //};

            TargetFlag = new DXImageControl  //目标旗帜
            {
                Parent = MapImageBackGround,
                LibraryFile = LibraryFile.UI1,
                Index = 1599,
                Opacity = 1F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false,
            };

            OuterFrame = new DXLabel //画外框
            {
                Parent = MapImageBackGround,
                Border = true,
                BorderColour = Color.Red,
                Size = new Size(160, 100),
                Location = new Point(10, 23),
            };
            //OuterFrame.MouseEnter += (s, e) =>   //鼠标进入
            //{
            //    TargetFlag.Visible = true;
            //};
            //OuterFrame.MouseLeave += (s, e) =>   //鼠标离开时
            //{
            //    TargetFlag.Visible = false;
            //};
            OuterFrame.MouseClick += (s, e) =>
            {
                TargetFlag.Visible = true;
                TargetFlag.Location = new Point(e.X - DisplayArea.Left - 5, e.Y - DisplayArea.Top - 10);  //光标的位置
                AttackX = (int)((e.Location.X - MapImage.DisplayArea.X) / ScaleX);
                AttackY = (int)((e.Location.Y - MapImage.DisplayArea.Y) / ScaleY);
                //OuterFrame.Hint = string.Format("{0},{1}", x, y);
            };

            //地图变更事件
            GameScene.Game.MapControl.MapInfoChanged += MapControl_MapInfoChanged;
        }
        #region methods

        /// <summary>
        /// 可见更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (null != TargetFlag) TargetFlag.Visible = false;
        }

        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //if (MouseInDialog)
            //{
            //    TargetFlag.Location = new Point(e.X - DisplayArea.Left - 5, e.Y - DisplayArea.Top - 10);  //光标的位置
            //}
        }

        #region 地图模块

        public Point FixLocation(Point location)  //固定位置
        {
            if (location.Y > 0) location.Y = 0;
            if (location.X > 0) location.X = 0;
            if (location.X < -(MapImage.Size.Width - Size.Width)) location.X = -(MapImage.Size.Width - Size.Width);
            if (location.Y < -(MapImage.Size.Height - Size.Height)) location.Y = -(MapImage.Size.Height - Size.Height);
            return location;
        }

        /// <summary>
        /// 地图信息变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MapControl_MapInfoChanged(object sender, EventArgs e)
        {
            foreach (DXControl temp in MapInfoObjects.Values)
                temp.Dispose();

            MapInfoObjects.Clear();

            if (GameScene.Game.MapControl.MapInfo == null) return;

            MapImage.Index = GameScene.Game.MapControl.MapInfo.MiniMap;
            MapImage.Size = new Size(MapImage.Size.Width, MapImage.Size.Height);

            //小地图窗口和地图全景的显示比例
            ScaleX = MapImage.Size.Width / (float)GameScene.Game.MapControl.Width;
            ScaleY = MapImage.Size.Height / (float)GameScene.Game.MapControl.Height;

            //地图背景最底层
            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Background, out MirLibrary library))
            {
                MirImage image = library.CreateImage(GameScene.Game.MapControl.MapInfo.Background, ImageType.Image);
                GameScene.Game.MapControl.BackgroundImage = image;
                if (image != null)
                {
                    GameScene.Game.MapControl.BackgroundScaleX = GameScene.Game.MapControl.Width * MapControl.CellWidth / (float)(image.Width - Config.GameSize.Width);
                    GameScene.Game.MapControl.BackgroundScaleY = GameScene.Game.MapControl.Height * MapControl.CellWidth / (float)(image.Height - Config.GameSize.Height);
                }
            }

            foreach (NPCInfo ob in Globals.NPCInfoList.Binding)
                Update(ob);

            foreach (MovementInfo ob in Globals.MovementInfoList.Binding)
                Update(ob);

            foreach (ClientObjectData ob in GameScene.Game.DataDictionary.Values)
                Update(ob);
        }

        /// <summary>
        /// 更新NPC信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(NPCInfo ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = MapImage;
                control.Opacity = MapImage.Opacity;

                MapInfoObjects[ob] = control;
            }
            else if ((QuestIcon)control.Tag == ob.CurrentIcon) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Parent = MapImage;
            control.Opacity = MapImage.Opacity;
            MapInfoObjects[ob] = control;

            if (ob.Region.PointList == null)
                ob.Region.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

            foreach (Point point in ob.Region.PointList)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int x = (minX + maxX) / 2;
            int y = (minY + maxY) / 2;

            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        /// <summary>
        /// 更新地图链接信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(MovementInfo ob)
        {
            if (ob.SourceRegion == null || ob.SourceRegion.Map != GameScene.Game.MapControl.MapInfo) return;
            if (ob.DestinationRegion?.Map == null || ob.Icon == MapIcon.None) return;

            if (ob.SourceRegion.PointList == null)
                ob.SourceRegion.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

            foreach (Point point in ob.SourceRegion.PointList)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int x = (minX + maxX) / 2;
            int y = (minY + maxY) / 2;

            DXImageControl control;
            MapInfoObjects[ob] = control = new DXImageControl
            {
                LibraryFile = LibraryFile.Interface,
                Parent = MapImage,
                Opacity = MapImage.Opacity,
                ImageOpacity = MapImage.Opacity,
                Hint = ob.DestinationRegion.Map.Description,
            };
            control.OpacityChanged += (o, e) => control.ImageOpacity = control.Opacity;

            switch (ob.Icon)  //地图各种标识图标
            {
                case MapIcon.Cave:
                    control.Index = 70;
                    control.ForeColour = Color.Red;
                    break;
                case MapIcon.Exit:
                    control.Index = 70;
                    control.ForeColour = Color.Green;
                    break;
                case MapIcon.Down:
                    control.Index = 500;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Up:
                    control.Index = 501;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Province:
                    control.Index = 101;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Building:
                    control.Index = 6124;
                    control.LibraryFile = LibraryFile.GameInter;
                    break;
                //各种入口图标
                case MapIcon.Entrance550:
                    control.Index = 550;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance551:
                    control.Index = 551;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance552:
                    control.Index = 552;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance553:
                    control.Index = 553;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance554:
                    control.Index = 554;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance555:
                    control.Index = 555;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance556:
                    control.Index = 556;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance557:
                    control.Index = 557;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance558:
                    control.Index = 558;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance559:
                    control.Index = 559;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance560:
                    control.Index = 560;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance561:
                    control.Index = 561;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance562:
                    control.Index = 562;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance563:
                    control.Index = 563;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                //连接图标
                case MapIcon.Connect100:
                    control.Index = 100;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect102:
                    control.Index = 102;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect103:
                    control.Index = 103;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect104:
                    control.Index = 104;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect120:
                    control.Index = 120;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect121:
                    control.Index = 121;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect122:
                    control.Index = 122;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect123:
                    control.Index = 123;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect140:
                    control.Index = 140;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect141:
                    control.Index = 141;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect142:
                    control.Index = 142;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect143:
                    control.Index = 143;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect160:
                    control.Index = 160;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect161:
                    control.Index = 161;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect162:
                    control.Index = 162;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect300:
                    control.Index = 300;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect301:
                    control.Index = 301;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect302:
                    control.Index = 302;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect510:
                    control.Index = 510;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect511:
                    control.Index = 511;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect570:
                    control.Index = 570;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect571:
                    control.Index = 571;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect572:
                    control.Index = 572;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
            }
            //control.MouseClick += (o, e) => SelectedInfo = ob.DestinationRegion.Map;
            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        /// <summary>?
        /// 更新客户端数据
        /// </summary>
        /// <param name="ob"></param>
        public void Update(ClientObjectData ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;
            //判断是不是攻城车

            if ((ob.MonsterInfo?.Image == MonsterImage.Catapult || ob.MonsterInfo?.Image == MonsterImage.Ballista) && ob.PetOwner == GameScene.Game.User.Name)
            {
                if (PetData != null)
                {
                    Remove(PetData, true);
                }
                PetData = ob;
            }

            DXControl control;
            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index) return;
                //if (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common) return;
                if (ob.ItemInfo != null) return;
                if (ob.MonsterInfo != null && ob.Dead) return;

                MapInfoObjects[ob] = control = new DXControl
                {
                    DrawTexture = true,
                    Parent = MapImage,
                    Opacity = MapImage.Opacity,
                    //MonsterInfo.AI < 0 ? Color.FromArgb(150, 200, 255) : Color.Red,
                };
            }
            else if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index || (ob.MonsterInfo != null && ob.Dead) || (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common))
            {
                control.Dispose();
                MapInfoObjects.Remove(ob);
                return;
            }

            //小地图标记信息
            Size size = new Size(3, 3);
            Color colour = Color.White;
            string name = ob.Name;

            if (ob != PetData && ob.MonsterInfo != null)
            {
                string _temname;
                // 只过滤结尾的数字
                _temname = ob.MonsterInfo.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                name = $"{_temname}";

                if (ob.MonsterInfo.AI < 0)
                {
                    colour = Color.LightBlue;
                }
                else
                {
                    colour = Color.Red;

                    if (GameScene.Game.HasQuest(ob.MonsterInfo, GameScene.Game.MapControl.MapInfo))
                        colour = Color.Orange;
                }

                if (ob.MonsterInfo.IsBoss)
                {
                    size = new Size(8, 8);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.Magenta,//colour,
                            DrawTexture = true,
                            Size = new Size(6, 6)
                        };
                    }
                    else
                        control.Controls[0].BackColour = Color.Magenta;

                    colour = Color.Pink;
                }

                if (!string.IsNullOrEmpty(ob.PetOwner))
                {
                    name += $" ({ob.PetOwner})";
                    control.DrawTexture = true;
                    colour = Color.FromArgb(100, 250, 255);
                }
            }
            else if (ob.ItemInfo != null)
            {
                colour = Color.DarkBlue;
                //物品
            }
            else
            {
                if (MapObject.User.ObjectID == ob.ObjectID)
                {
                    //自己标记
                    size = new Size(5, 5);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.DarkOrange,
                            DrawTexture = true,
                            Size = new Size(3, 3)
                        };
                    }
                    //else
                    //	control.Controls[0].BackColour = Color.DarkOrange;
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Observer)
                {
                    control.Visible = false;
                }
                else if (GameScene.Game.GroupBox.Members.Any(p => p.ObjectID == ob.ObjectID))
                {
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Partner != null && GameScene.Game.Partner.ObjectID == ob.ObjectID)
                {
                    colour = Color.DeepPink;
                }
                else if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(p => p.ObjectID == ob.ObjectID))
                {
                    colour = Color.DeepSkyBlue;
                }
            }

            //小地图标记控件
            control.Hint = name;
            control.BackColour = colour;
            control.Size = size;
            control.Location = new Point((int)(ScaleX * ob.Location.X) - size.Width / 2, (int)(ScaleY * ob.Location.Y) - size.Height / 2);

            if (PetData?.ObjectID != ob.ObjectID) return;

            control = MapInfoObjects[PetData];
            control.DrawTexture = true;
            control.Size = new Size(4, 4);
            control.BackColour = Color.FromArgb(100, 250, 255);

            MapImage.Location = FixLocation(new Point(-control.Location.X + Size.Width / 2, -control.Location.Y + Size.Height / 2));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ob"></param>
        public void Remove(object ob, bool pet = false)
        {
            if (!pet && ob == PetData) return;
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }
        #endregion

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ScaleX = 0;
                ScaleY = 0;

                foreach (KeyValuePair<object, DXControl> pair in MapInfoObjects)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                MapInfoObjects.Clear();
                MapInfoObjects = null;

                if (MapImage != null)
                {
                    if (!MapImage.IsDisposed)
                        MapImage.Dispose();

                    MapImage = null;
                }

                if (MapImageBackGround != null)
                {
                    if (!MapImageBackGround.IsDisposed)
                        MapImageBackGround.Dispose();

                    MapImageBackGround = null;
                }

                if (TargetFlag != null)
                {
                    if (!TargetFlag.IsDisposed)
                        TargetFlag.Dispose();

                    TargetFlag = null;
                }
            }
        }
        #endregion
    }
}
