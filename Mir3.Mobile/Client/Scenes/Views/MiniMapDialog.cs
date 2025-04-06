using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 小地图功能
    /// </summary>
    public sealed class MiniMapDialog : DXWindow
    {
        #region Properties

        public Rectangle Area;
        private DXImageControl Image;
        private DXImageControl LeftUp, RightUp, LeftDown, RightDown;
        /// <summary>
        /// 全景
        /// </summary>
        public DXButton Panorama;
#if Mobile
#else
        /// <summary>
        /// 放大
        /// </summary>
        public DXButton Enlarge;
        /// <summary>
        /// 半透明
        /// </summary>
        public DXButton Translucent;
#endif
        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();

        public static float ScaleX, ScaleY;

        public bool UpdatePathToDraw
        {
            get;
            set;
        }

        public static Size DefSize;
        public float ScaleSize = 1.0f;//放大倍数
        public override void OnOpacityChanged(float oValue, float nValue)
        {
            base.OnOpacityChanged(oValue, nValue);

            foreach (DXControl control in Controls)
                control.Opacity = Opacity;

            foreach (DXControl control in MapInfoObjects.Values)
                control.Opacity = Opacity;

            if (Image != null)
            {
                Image.Opacity = Opacity;
                Image.ImageOpacity = 0.85F;
            }
        }
        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnClientAreaChanged(oValue, nValue);

            Area = ClientArea;  //面积=客户端面积
            Area.Inflate(6, 6);  //面积放大 宽6 高6   这里其实等于小地图里显示的地图缩放大小

            UpdateFrameLocation();

            UpdateMapPosition();

            UpdateBuffBoxLocation();
        }

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            UpdateFrameLocation();

            UpdateBuffBoxLocation();
        }

        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);
            UpdateBuffBoxLocation();
        }
        public void UpdateBuffBoxLocation()
        {
#if Mobile
#else
            if (GameScene.Game?.BuffBox != null)
            {
                if (GameScene.Game?.MiniMapBox?.Visible == false || GameScene.Game?.MapControl?.MapInfo?.MiniMap == 0)
                {
                    GameScene.Game.BuffBox.Location = new Point(GameScene.Game.Size.Width - GameScene.Game.BuffBox.Size.Width, 0);
                }
                else
                {
                    GameScene.Game.BuffBox.Location = new Point(Location.X - GameScene.Game.BuffBox.Size.Width, 0);
                }
            }
#endif
        }
        private void UpdateFrameLocation()
        {
#if Mobile
            Location = new Point(GameScene.Game.Size.Width - Size.Width - 50, 0);
#else
            Location = new Point(GameScene.Game.Size.Width - Size.Width, 0);
#endif

            if (LeftUp == null || LeftDown == null || RightUp == null || RightDown == null) return;

            LeftUp.Location = new Point(0, 0);
            LeftDown.Location = new Point(0, Size.Height - LeftDown.Size.Height);
            RightUp.Location = new Point(Size.Width - RightUp.Size.Width, 0);
            RightDown.Location = new Point(Size.Width - RightDown.Size.Width, Size.Height - RightDown.Size.Height);

#if Mobile
            Panorama.Location = new Point(0, Size.Height - Panorama.Size.Height);
#else
            if (Enlarge == null || Translucent == null || Panorama == null) return;
            Enlarge.Location = new Point(Size.Width - Enlarge.Size.Width, Size.Height - Enlarge.Size.Height);
            Translucent.Location = new Point(Size.Width - Enlarge.Size.Width - Translucent.Size.Width + 1, Size.Height - Enlarge.Size.Height);
            Panorama.Location = new Point(Size.Width - Enlarge.Size.Width - Translucent.Size.Width - Panorama.Size.Width + 1, Size.Height - Enlarge.Size.Height + 1);
#endif
        }
        private void MapNotExsit(bool bNotExsit)
        {
            LeftUp.Visible = !bNotExsit;
            RightUp.Visible = !bNotExsit;
            LeftDown.Visible = !bNotExsit;
            //RightDown.Visible = !bNotExsit;
            Panorama.Visible = !bNotExsit;
#if Mobile
#else
            Enlarge.Visible = !bNotExsit;
            Translucent.Visible = !bNotExsit;
#endif
        }
        public override WindowType Type => WindowType.MiniMapBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        /// <summary>
        /// 小地图界面
        /// </summary>
        public MiniMapDialog()
        {
            HasTitle = false;  //字幕标题不显示
            HasFooter = false;  //不显示页脚
            HasTopBorder = false; //不显示上边框
            TitleLabel.Visible = false; //不显示标题
            CloseButton.Visible = false; //不显示关闭按钮 
            Opacity = 0F;

            AllowResize = false;  //允许调整大小
            Movable = false;

            Size = new Size(150, 150);
            DefSize = Size;
            Image = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MiniMap,
                ImageOpacity = 1.0F,
                Movable = false,  //可移动
                IgnoreMoveBounds = true,  //忽略移动边界
                ForeColour = Color.FromArgb(150, 150, 150),
                ZoomSize = new Size(450, 450),
                Tag = 0,
            };
            Image.MouseClick += Image_MouseClick;  //地图鼠标单击
            Image.MouseMove += Image_MouseMove;  //显示鼠标指向坐标

            LeftUp = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1486,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                Visible = true,
            };
            RightUp = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1487,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                Visible = true,
            };
            LeftDown = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1488,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                Visible = true,
            };
            RightDown = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1489,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                Visible = true,
            };

#if Mobile

            Panorama = new DXButton   //隐藏按钮
            {
                LibraryFile = LibraryFile.PhoneUI,
                Index = 81,
                Parent = this,
                Hint = "隐藏".Lang(),
            };
            Panorama.TouchUp += (o, e) =>// GameScene.Game.BigMapBox.Visible = !GameScene.Game.BigMapBox.Visible;
            {
                if (ScaleSize != 1.0f && (int)Image.Tag == 1)
                {
                    ScaleSize = 1.0f;
                    Image.Tag = 2;
                }
                else
                {
                    if (Image.Size.Width >= 250 && Image.Size.Height >= 250 && (int)Image.Tag == 0)
                    {
                        ScaleSize = 1.7f;
                        Image.Tag = 1;
                    }
                    else
                    {
                        if (Size == Panorama.Size)
                            Size = new Size((int)(DefSize.Width * ScaleSize), (int)(DefSize.Height * ScaleSize));
                        else
                            Size = Panorama.Size;
                        Image.Tag = 0;
                        return;
                    }
                }
                Size nSize = new Size((int)(DefSize.Width * ScaleSize), (int)(DefSize.Height * ScaleSize));
                Size = nSize;
            };
#else
            Enlarge = new DXButton       //放大
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1485,
                Parent = this,
                Hint = "缩放".Lang(),
            };
            Enlarge.MouseClick += (o, e) =>
            {
                if (ScaleSize != 1.0f)
                {
                    ScaleSize = 1.0f;
                    Enlarge.Hint = "放大".Lang();
                }
                else
                {
                    if (Image.Size.Width >= 250 && Image.Size.Height >= 250)
                    {
                        ScaleSize = 1.7f;
                        Enlarge.Hint = "缩放".Lang();                     
                    }
                    else
                    {
                        ScaleSize = 1.0f;
                        Enlarge.Hint = "放大".Lang();
                    }
                }
                Size nSize = new Size((int)(DefSize.Width * ScaleSize), (int)(DefSize.Height * ScaleSize));
                Size = nSize;
                Panorama.Visible = true;
            };

            Translucent = new DXButton   //半透明
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1482,
                Parent = this,
                Hint = "透明".Lang(),
            };
            Translucent.MouseClick += (o, e) =>
            {
                if (Image.Opacity == 1.0F)
                {
                    Image.Opacity = 0.7F;
                    Image.ImageOpacity = 0.7F;
                }
                else
                {
                    Image.Opacity = 1.0F;
                    Image.ImageOpacity = 1.0F;
                }
            };

            Panorama = new DXButton   //全景
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1672,
                Parent = this,
                Hint = "大图".Lang(),
            };
            Panorama.MouseClick += (o, e) =>// GameScene.Game.BigMapBox.Visible = !GameScene.Game.BigMapBox.Visible;
            {
                ScaleSize = Image.Size.Height / DefSize.Height;

                Size nSize = new Size(Math.Min(350, Image.Size.Width), Math.Min(350, Image.Size.Height));
                Size = nSize;

                Enlarge.Hint = "缩放".Lang();
                Panorama.Visible = false;
            };
#endif


            //更新各控件坐标
            UpdateFrameLocation();
            //地图变更事件
            GameScene.Game.MapControl.MapInfoChanged += MapControl_MapInfoChanged;
            //Image.Moving += Image_Moving;   //地图移动
        }

        #region Methods
        public Point FixLocation(Point location)  //固定位置
        {
            if (location.Y > 0) location.Y = 0;
            if (location.X > 0) location.X = 0;
            if (location.X < -(Image.Size.Width - Size.Width)) location.X = -(Image.Size.Width - Size.Width);
            if (location.Y < -(Image.Size.Height - Size.Height)) location.Y = -(Image.Size.Height - Size.Height);
            return location;
        }
        /// <summary>
        /// 图片移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Moving(object sender, MouseEventArgs e)
        {
            int x = Image.Location.X;

            if (x + Image.Size.Width < Size.Width)
                x = Size.Width - Image.Size.Width;

            if (x > 0)
                x = 0;

            int y = Image.Location.Y;

            if (y + Image.Size.Height < Size.Height)
                y = Size.Height - Image.Size.Height;

            if (y > 0)
                y = 0;

            //判读防止小地图移动越界
            Point location = new Point(x, y);
            Image.Location = FixLocation(location);
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

            TitleLabel.Text = GameScene.Game.MapControl.MapInfo.Description;
            Image.Index = GameScene.Game.MapControl.MapInfo.MiniMap;

            if (Image.Size.Width <= 1 || Image.Size.Height <= 1)
            {
                //地图不存在
                MapNotExsit(true);
            }
            else
            {
                MapNotExsit(false);
            }
            ScaleSize = 1F;
            Size = DefSize;

            //小地图窗口和地图全景的显示比例
            ScaleX = Image.Size.Width / (float)GameScene.Game.MapControl.Width;
            ScaleY = Image.Size.Height / (float)GameScene.Game.MapControl.Height;

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

            UpdateBuffBoxLocation();
        }
        /// <summary>
        /// 更新NPC信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(NPCInfo ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;

            if (ob.Display) return;    //不显示隐藏的NPC

            if (ob.Image >= 999) return;  //不显示图库序号大于等于999值的NPC

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = Image;
                control.Opacity = Image.Opacity;

                MapInfoObjects[ob] = control;
            }
            else if ((QuestIcon)control.Tag == ob.CurrentIcon) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Parent = Image;
            control.Opacity = Image.Opacity;
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
                Parent = Image,
                Opacity = Image.Opacity,
                ImageOpacity = Image.Opacity,
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
        /// <summary>
        /// 更新客户端数据
        /// </summary>
        /// <param name="ob"></param>
        public void Update(ClientObjectData ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;
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
                    Parent = Image,
                    Opacity = Image.Opacity,
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
            Size size = new Size(4, 4);
            Color colour = Color.White;
            string name = ob.Name;

            if (ob.MonsterInfo != null)
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
                    //control.DrawTexture = false;
                    if (ob.PetOwner == MapObject.User.Name)
                    {
                        if (ob.MonsterInfo.Image == MonsterImage.Catapult || ob.MonsterInfo.Image == MonsterImage.Ballista)
                        {
                            colour = Color.FromArgb(100, 250, 255);
                        }
                        else
                            colour = Color.Orange;
                    }
                    else
                    {
                        colour = Color.Red;
                    }
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
                    size = new Size(6, 6);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.Lime,
                            DrawTexture = true,
                            Size = new Size(4, 4)
                        };
                    }
                    //else
                    //	control.Controls[0].BackColour = Color.DarkOrange;
                    colour = Color.Black;
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

            if (MapObject.User.ObjectID != ob.ObjectID) return;

            Image.Location = FixLocation(new Point(-control.Location.X + Area.Width / 2, -control.Location.Y + Area.Height / 2));

#if Mobile
            GameScene.Game.ExpPanel.GridLabel.Text = GameScene.Game.MapControl.MapInfo.Lang(p => p.Description) + string.Format(" {0}:{1}", ob.Location.X, ob.Location.Y);
#else
            if (MapObject.User.ObjectID == ob.ObjectID)
            {
                GameScene.Game.MainPanel.GridLabel.Text = GameScene.Game.MapControl.MapInfo.Lang(p => p.Description) + string.Format(" {0}:{1}", ob.Location.X, ob.Location.Y);
            }
#endif
        }
        /// <summary>
        /// 更新地图坐标
        /// </summary>
        public void UpdateMapPosition()
        {
            if (MapObject.User == null) return;

            ClientObjectData data;

            //获得自己位置的信息
            if (!GameScene.Game.DataDictionary.TryGetValue(MapObject.User.ObjectID, out data)) return;

            //以自己位置为中心移动地图
            DXControl control;
            if (!MapInfoObjects.TryGetValue(data, out control)) return;

            Point location = control.Location;
            Image.Location = FixLocation(new Point(-location.X + Area.Width / 2, -location.Y + Area.Height / 2));

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ob"></param>
        public void Remove(object ob)
        {
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }

        //自动寻路状态
        public DateTime ClickTick;
        /// <summary>
        /// 地图鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            GameScene.Game.BigMapBox.Visible = !GameScene.Game.BigMapBox.Visible;

            //if (BigPatchConfig.AndroidPlayer) return;
            ////修复地图没有停止寻路时卡顿
            //if (GameScene.Game.MapControl.AutoPath)
            //{
            //    GameScene.Game.MapControl.AutoPath = false;
            //    GameScene.Game.ReceiveChat("BigMap.PathfindingOff".Lang(), MessageType.Hint);
            //}

            //Point point = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            //int x = (int)((point.X - Image.DisplayArea.X) / ScaleX);
            //int y = (int)((point.Y - Image.DisplayArea.Y) / ScaleY);

            //if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            //{
            //    //传送戒指               
            //    CEnvir.Enqueue(new C.TeleportRing { Location = new Point(x, y), Index = GameScene.Game.MapControl.MapInfo.Index });
            //}
            //else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
            //    DateTime _Last = ClickTick;
            //    ClickTick = CEnvir.Now;
            //    if (_Last.AddSeconds(1) > ClickTick)
            //    {
            //        GameScene.Game.ReceiveChat("BigMap.PathfindingClick".Lang(), MessageType.System);
            //        return;
            //    }

            //    // 如果目标是不可移动区域
            //    if (GameScene.Game.MapControl.Cells[x, y].Flag)
            //    {
            //        GameScene.Game.ReceiveChat("BigMap.PathfindingMove".Lang(), MessageType.System);
            //        return;
            //    }

            //    //开始寻路， 启用原子操作，开启异步
            //    if (!GameScene.Game.MapControl.PathFinder.bSearching)
            //    {
            //        GameScene.Game.MapControl.PathFinder.bSearching = true;
            //        Task.Run(() =>
            //        {
            //            try
            //            {
            //                GameScene.Game.MapControl.InitCurrentPath(x, y);

            //                GameScene.Game.MapControl.PathFinder.bSearching = false;
            //            }
            //            catch
            //            {
            //                GameScene.Game.MapControl.PathFinder.bSearching = false;
            //            }
            //        });

            //        GameScene.Game.ReceiveChat("BigMap.PathfindingOn".Lang(), MessageType.Hint);
            //    }
            //    else
            //    {
            //        GameScene.Game.ReceiveChat("BigMap.PathfindingNotFinished".Lang(), MessageType.System);
            //    }
            //}
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            if (UpdatePathToDraw)
                UpdatePathForDraw();

            OnBeforeDraw();      //绘制前打开
            DrawControl();       //绘制控件
            OnBeforeChildrenDraw();    //绘制子节点前
            DrawChildControls();       //绘制子控件           
            //DrawWindow();
            DrawBorder();
            OnAfterDraw();             //绘制后打开

            if (UpdatePathToDraw)
                UpdatePathToDraw = false;
        }
        /// <summary>
        /// 更新自动寻路线条
        /// </summary>
        public void UpdatePathForDraw()
        {
            if (MapInfoObjects == null) return;

            lock (MapInfoObjects)
            {
                if (!GameScene.Game.MapControl.AutoPath)
                {
                    for (int i = MapInfoObjects.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<object, DXControl> keyValuePair = MapInfoObjects.ElementAt(i);
                        if (keyValuePair.Key is Node)
                        {
                            keyValuePair.Value?.Dispose();
                            MapInfoObjects.Remove(keyValuePair);
                        }
                    }
                }
                else
                {
                    if (GameScene.Game.MapControl.CurrentPath == null) return;

                    lock (GameScene.Game.MapControl.CurrentPath)
                    {
                        for (int i = MapInfoObjects.Count - 1; i >= 0; i--)
                        {
                            KeyValuePair<object, DXControl> element = MapInfoObjects.ElementAt(i);
                            if (element.Key is Node && GameScene.Game.MapControl.CurrentPath.Any((Node x) => x.Location != element.Value.Location))
                            {
                                element.Value.Dispose();
                                MapInfoObjects.Remove(element);
                            }
                        }
                        for (int i = 0; i < GameScene.Game.MapControl.CurrentPath.Count; i += (int)(12 / ScaleX))
                        {
                            Node ob = GameScene.Game.MapControl.CurrentPath[i];
                            if (ob == null) continue;

                            if (MapInfoObjects.TryGetValue(ob, out var value) && !MapInfoObjects.Any((KeyValuePair<object, DXControl> x) => x.Key is Node && x.Value.Location != ob.Location))
                            {
                                if (ob != null)
                                {
                                    value?.Dispose();
                                    MapInfoObjects.Remove(ob);
                                }
                                continue;
                            }
                            DXControl obj = new DXControl
                            {
                                DrawTexture = true,
                                Parent = Image,
                            };
                            MapInfoObjects[ob] = obj;
                            Size size = new Size(3, 3);
                            obj.BackColour = Color.White;
                            obj.Size = size;
                            obj.Location = new Point((int)(ScaleX * (float)ob.Location.X) - size.Width / 2, (int)(ScaleY * (float)ob.Location.Y) - size.Height / 2);
                        }
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// 显示鼠标指向坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            int x = (int)((point.X - Image.DisplayArea.X) / ScaleX);
            int y = (int)((point.Y - Image.DisplayArea.Y) / ScaleY);
            Image.Hint = string.Format("{0},{1}", x, y, ScaleX, ScaleY);
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Area = Rectangle.Empty;
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


                if (Image != null)
                {
                    if (!Image.IsDisposed)
                        Image.Dispose();

                    Image = null;
                }

            }
        }

        #endregion
    }
}
