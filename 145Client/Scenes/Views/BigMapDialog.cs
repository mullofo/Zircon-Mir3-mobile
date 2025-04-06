using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 大地图功能
    /// </summary>
    public sealed class BigMapDialog : DXWindow
    {
        #region Properties

        #region SelectedInfo
        /// <summary>
        /// 选择信息
        /// </summary>
        public MapInfo SelectedInfo
        {
            get { return _SelectedInfo; }
            set
            {
                if (_SelectedInfo == value) return;

                MapInfo oldValue = _SelectedInfo;
                _SelectedInfo = value;
                OnSelectedInfoChanged(oldValue, value);
            }
        }
        private MapInfo _SelectedInfo;
        public event EventHandler<EventArgs> SelectedInfoChanged;
        public void OnSelectedInfoChanged(MapInfo oValue, MapInfo nValue)  //选定信息更改时
        {
            SelectedInfoChanged?.Invoke(this, EventArgs.Empty);
            ///CurrentNpc.Visible = false;   //当前NPC不显示

            foreach (DXControl control in MapInfoObjects.Values)
                control.Dispose();

            MapInfoObjects.Clear();

            if (SelectedInfo == null) return;
            Image.Index = SelectedInfo.MiniMap;

            //针对地图右边有黑边 可以用这个切掉
            int cutRight = 0; //改成300就是切掉最右边300个像素
            SetClientSize(new Size(Image.Size.Width - cutRight, Image.Size.Height));

            Location = new Point((GameScene.Game.Size.Width - Size.Width) / 2, (GameScene.Game.Size.Height - Size.Height) / 2);
            Opacity = 0F;

            Size size = GetMapSize(SelectedInfo.FileName);
            ScaleX = Image.Size.Width / (float)size.Width;
            ScaleY = Image.Size.Height / (float)size.Height;

            foreach (NPCInfo ob in Globals.NPCInfoList.Binding)
                Update(ob);

            foreach (MovementInfo ob in Globals.MovementInfoList.Binding)
                Update(ob);

            foreach (ClientObjectData ob in GameScene.Game.DataDictionary.Values)
                Update(ob);

            UpdatePathToDraw = true;
        }
        /// <summary>
        /// 设置地图大小
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Size GetMapSize(string fileName)
        {
            if (!File.Exists(Config.MapPath + fileName + ".map")) return Size.Empty;

            using (FileStream stream = File.OpenRead(Config.MapPath + fileName + ".map"))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //stream.Seek(22, SeekOrigin.Begin);
                //return new Size(reader.ReadInt16(), reader.ReadInt16());

                //地图扩展
                byte[] mapBytes = reader.ReadBytes(40);
                //(0-99) c#自定义地图格式 title:
                if (mapBytes[2] == 0x43 && mapBytes[3] == 0x23)
                {
                    int offset = 4;
                    if (mapBytes[0] != 1 || mapBytes[1] != 0) return Size.Empty; ;//only support version 1 atm
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
                //(200-299) 韩版传奇3 title:
                else if (mapBytes[0] == 0)
                {
                    int offset = 20;
                    short Attribute = BitConverter.ToInt16(mapBytes, offset);
                    int Width = (int)(BitConverter.ToInt16(mapBytes, offset += 2));
                    int Height = (int)(BitConverter.ToInt16(mapBytes, offset += 2));
                    return new Size(Width, Height);
                }
                //(300-399) 盛大传奇3 title: (C) SNDA, MIR3.
                else if (mapBytes[0] == 0x0F && mapBytes[5] == 0x53 && mapBytes[14] == 0x33)
                {
                    int offset = 16;
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
                //(400-499) 应该是盛大传奇3第二种格式，未知？无参考
                else if (mapBytes[0] == 0x0F && mapBytes[5] == 0x4D && mapBytes[14] == 0x33)
                {
                    int offset = 16;
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
                //(0-99) wemades antihack map (laby maps) title start with: Mir2 AntiHack
                else if (mapBytes[0] == 0x15 && mapBytes[4] == 0x32 && mapBytes[6] == 0x41 && mapBytes[19] == 0x31)
                {
                    int offset = 31;
                    int w = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int xor = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int h = BitConverter.ToInt16(mapBytes, offset);
                    int Width = (w ^ xor);
                    int Height = (h ^ xor);
                    return new Size(Width, Height);
                }
                //(0-99) wemades 2010 map format i guess title starts with: Map 2010 Ver 1.0
                else if (mapBytes[0] == 0x10 && mapBytes[2] == 0x61 && mapBytes[7] == 0x31 && mapBytes[14] == 0x31)
                {
                    int offset = 21;
                    int w = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int xor = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int h = BitConverter.ToInt16(mapBytes, offset);
                    int Width = (w ^ xor);
                    int Height = (h ^ xor);
                    return new Size(Width, Height);
                }
                //(100-199) shanda's 2012 format and one of shandas(wemades) older formats share same header info, only difference is the filesize
                else if ((mapBytes[4] == 0x0F || (mapBytes[4] == 0x03)) && mapBytes[18] == 0x0D && mapBytes[19] == 0x0A)
                {
                    int offset = 0;
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
                //(0-99) 3/4 heroes map format (myth/lifcos i guess)
                else if (mapBytes[0] == 0x0D && mapBytes[1] == 0x4C && mapBytes[7] == 0x20 && mapBytes[11] == 0x6D)
                {
                    int offset = 21;
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 4;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
                //if it's none of the above load the default old school format
                else
                {
                    int offset = 0;
                    int Width = BitConverter.ToInt16(mapBytes, offset);
                    offset += 2;
                    int Height = BitConverter.ToInt16(mapBytes, offset);
                    return new Size(Width, Height);
                }
            }
        }

        #endregion
        public DXImageControl TopLeft, TopRight, LeftLower, LowerRight;
        public Rectangle Area;   //矩形区域
        public DXImageControl Image;  //图像控制
        public DXControl Panel;  //控制面板

        public static float ScaleX, ScaleY;

        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();

        public bool UpdatePathToDraw
        {
            get;
            set;
        }

        public override void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)  //客户端区域更改
        {
            base.OnClientAreaChanged(oValue, nValue);

            if (Panel == null) return;

            Panel.Location = ClientArea.Location;
            Panel.Size = ClientArea.Size;
            TopLeft.Location = new Point(6, 10);
            TopRight.Location = new Point(Size.Width - TopRight.Size.Width - 6, 10);
            LeftLower.Location = new Point(6, Size.Height - LeftLower.Size.Height - 10);
            LowerRight.Location = new Point(Size.Width - LowerRight.Size.Width - 6, Size.Height - LowerRight.Size.Height - 10);
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)  //打开可见更改
        {
            base.OnIsVisibleChanged(oValue, nValue);

            SelectedInfo = IsVisible ? GameScene.Game.MapControl.MapInfo : null;

        }
        public override void OnOpacityChanged(float oValue, float nValue)  //不透明度改变时
        {
            base.OnOpacityChanged(oValue, nValue);

            foreach (DXControl control in Controls)
                control.Opacity = nValue;

            foreach (DXControl control in MapInfoObjects.Values)
                control.Opacity = nValue;

            if (Image != null)
            {
                Image.Opacity = nValue;
                Image.ImageOpacity = 0.85F;
            }
        }


        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 大地图界面
        /// </summary>
        public BigMapDialog()
        {
            HasTitle = false;  //字幕标题不显示
            HasTopBorder = false; //不显示上边框
            TitleLabel.Visible = false; //不显示标题
            BackColour = Color.Black;  //背景色  黑色
            CloseButton.Visible = false; //不显示关闭按钮
            HasFooter = false;  //不显示页脚
            Opacity = 0F;

            AllowResize = false;  //允许调整大小

            Panel = new DXControl   //面板
            {
                Parent = this,
                Location = Area.Location,
                Size = Area.Size,
            };

            TopLeft = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1486,
                Parent = this,
            };

            TopRight = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1487,
                Parent = this,
            };

            LeftLower = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1488,
                Parent = this,
            };

            LowerRight = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1489,
                Parent = this,
            };

            Image = new DXImageControl   //图像
            {
                Parent = Panel,
                LibraryFile = LibraryFile.MiniMap,   //读取小地图
                ImageOpacity = 1F,
            };

            ClickTick = CEnvir.Now;
            Image.MouseMove += Image_MouseMove;  //显示鼠标指向坐标
            Image.MouseClick += Image_MouseClick;  //地图鼠标单击
        }

        public override void OnVisibleChanged(bool oValue, bool nValue) //无地图 则隐藏边角
        {
            base.OnVisibleChanged(oValue, nValue);
            if (TopLeft != null)
                TopLeft.Visible = Image?.Size.Width > 1 && Image?.Size.Height > 1;
            if (TopRight != null)
                TopRight.Visible = Image?.Size.Width > 1 && Image?.Size.Height > 1;
            if (LeftLower != null)
                LeftLower.Visible = Image?.Size.Width > 1 && Image?.Size.Height > 1;
            if (LowerRight != null)
                LowerRight.Visible = Image?.Size.Width > 1 && Image?.Size.Height > 1;
        }
        /// <summary>
        /// 显示鼠标指向坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (int)((e.Location.X - Image.DisplayArea.X) / ScaleX);
            int y = (int)((e.Location.Y - Image.DisplayArea.Y) / ScaleY);
            Image.Hint = string.Format("{0},{1}", x, y, ScaleX, ScaleY);
        }

        //自动寻路状态
        public DateTime ClickTick;
        /// <summary>
        /// 鼠标单击地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            //修复大地图没有停止寻路时卡顿
            if (GameScene.Game.MapControl.AutoPath)
            {
                GameScene.Game.MapControl.AutoPath = false;
                GameScene.Game.ReceiveChat("BigMap.PathfindingOff".Lang(), MessageType.Hint);
            }

            //if (MapObject.User.Buffs.All(z => z.Type != BuffType.Developer))
            //if (!SelectedInfo.AllowRT || !SelectedInfo.AllowTT || !GameScene.Game.MapControl.MapInfo.AllowRT || !GameScene.Game.MapControl.MapInfo.AllowTT) return;
            int x = (int)((e.Location.X - Image.DisplayArea.X) / ScaleX);
            int y = (int)((e.Location.Y - Image.DisplayArea.Y) / ScaleY);
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                //传送戒指               
                CEnvir.Enqueue(new C.TeleportRing { Location = new Point(x, y), Index = SelectedInfo.Index });
            }
            else if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (BigPatchConfig.AndroidPlayer) return;
                //如果面板显示的不是当前地图，返回
                if (SelectedInfo != GameScene.Game.MapControl.MapInfo) return;

                DateTime _Last = ClickTick;
                ClickTick = CEnvir.Now;
                if (_Last.AddSeconds(1) > ClickTick)
                {
                    GameScene.Game.ReceiveChat("BigMap.PathfindingClick".Lang(), MessageType.System);
                    return;
                }

                //如果目标是不可移动区域
                if (GameScene.Game.MapControl.Cells[x, y].Flag)
                {
                    GameScene.Game.ReceiveChat("BigMap.PathfindingMove".Lang(), MessageType.System);
                    return;
                }

                //开始寻路
                if (!GameScene.Game.MapControl.PathFinder.bSearching)
                {
                    GameScene.Game.MapControl.PathFinder.bSearching = true;
                    Task.Run(() =>
                    {
                        try
                        {
                            GameScene.Game.MapControl.InitCurrentPath(x, y);

                            GameScene.Game.MapControl.PathFinder.bSearching = false;
                        }
                        catch
                        {
                            GameScene.Game.MapControl.PathFinder.bSearching = false;
                        }
                    });

                    GameScene.Game.ReceiveChat("BigMap.PathfindingOn".Lang(), MessageType.Hint);
                }
                else
                {
                    GameScene.Game.ReceiveChat("BigMap.PathfindingNotFinished".Lang(), MessageType.System);
                }
            }
        }

        #region Methods

        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            if (UpdatePathToDraw)
                UpdatePathForDraw();

            OnBeforeDraw();
            DrawControl();
            DrawWindow();
            OnBeforeChildrenDraw();
            DrawChildControls();
            DrawBorder();
            OnAfterDraw();

            if (UpdatePathToDraw)
                UpdatePathToDraw = false;
        }
        /// <summary>
        /// 更新自动寻路线条
        /// </summary>
        public void UpdatePathForDraw()
        {
            if (SelectedInfo == null || MapInfoObjects == null || SelectedInfo.Index != GameScene.Game.MapControl.MapInfo.Index)
            {
                return;
            }
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
                    return;
                }

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
            }
            base.TextureValid = false;
        }
        /// <summary>
        /// 更新NPC信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(NPCInfo ob)
        {
            if (SelectedInfo == null) return;  //如果选择信息等零 返回

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))  //如果 地图信息对象 尝试获取值
            {
                if (ob.Region?.Map != SelectedInfo) return;  //如果 区域地图 不是选择信息 返回

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = Image;         //  来源=图片
                control.Visible = false;        //  不显示
                MapInfoObjects[ob] = control;   //地图信息对象 = 选择
            }
            else if ((QuestIcon)control.Tag == ob.CurrentIcon) return;  //如果 任务图标 等 当前图标  返回

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != SelectedInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Visible = control.GetType() == typeof(DXImageControl);    //显示NPC任务标记
            control.Parent = Image;
            MapInfoObjects[ob] = control;

            Size size = GetMapSize(SelectedInfo.FileName);

            if (ob.Region.PointList == null)
                ob.Region.CreatePoints(size.Width);

            int minX = size.Width, maxX = 0, minY = size.Height, maxY = 0;

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
        /// 更新怪物信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(MovementInfo ob)
        {
            if (ob.SourceRegion == null || ob.SourceRegion.Map != SelectedInfo) return;
            if (ob.DestinationRegion?.Map == null || ob.Icon == MapIcon.None) return;

            Size size = GetMapSize(SelectedInfo.FileName);

            if (ob.SourceRegion.PointList == null)
                ob.SourceRegion.CreatePoints(size.Width);

            int minX = size.Width, maxX = 0, minY = size.Height, maxY = 0;

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
                LibraryFile = LibraryFile.WorldMap,
                Parent = Image,
                Opacity = 1,
                ImageOpacity = 1,
                Hint = ob.DestinationRegion.Map.Description,
            };
            control.OpacityChanged += (o, e) => control.ImageOpacity = control.Opacity;

            switch (ob.Icon)  //入口图片
            {
                case MapIcon.Cave:
                    control.Index = 70;
                    control.ForeColour = Color.Red;
                    control.LibraryFile = LibraryFile.Interface;
                    break;
                case MapIcon.Exit:
                    control.Index = 70;
                    control.ForeColour = Color.Green;
                    control.LibraryFile = LibraryFile.Interface;
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
            control.MouseClick += (o, e) => SelectedInfo = ob.DestinationRegion.Map;
            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        /// <summary>
        /// 更新玩家数据
        /// </summary>
        /// <param name="ob"></param>
        public void Update(ClientObjectData ob)
        {
            if (SelectedInfo == null) return;

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.MapIndex != SelectedInfo.Index) return;
                //if (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common) return;
                if (ob.ItemInfo != null) return;
                if (ob.MonsterInfo != null && ob.Dead) return;


                MapInfoObjects[ob] = control = new DXControl
                {
                    DrawTexture = true,
                    Parent = Image,
                    Opacity = 1,
                };
            }
            else if (ob.MapIndex != SelectedInfo.Index || (ob.MonsterInfo != null && ob.Dead) || (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common))
            {
                control.Dispose();
                MapInfoObjects.Remove(ob);
                return;
            }

            Size size = new Size(3, 3);
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

                    if (control.Controls.Count == 0)
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.Magenta,
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
                    control.DrawTexture = false;
                }
            }
            else if (ob.ItemInfo != null)
            {
                colour = Color.DarkBlue;
            }
            else
            {
                if (MapObject.User.ObjectID == ob.ObjectID)
                {
                    size = new Size(7, 7);

                    if (control.Controls.Count == 0)
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
                    //	control.Controls[0].BackColour = Color.White;
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Observer)
                {
                    control.Visible = false;
                }
                else if (GameScene.Game.GroupBox.Members.Any(x => x.ObjectID == ob.ObjectID))
                {
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Partner != null && GameScene.Game.Partner.ObjectID == ob.ObjectID)
                {
                    colour = Color.DeepPink;
                }
                else if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(x => x.ObjectID == ob.ObjectID))
                {
                    colour = Color.DeepSkyBlue;
                }
            }

            control.Hint = name;
            control.BackColour = colour;
            control.Size = size;
            control.Location = new Point((int)(ScaleX * ob.Location.X) - size.Width / 2, (int)(ScaleY * ob.Location.Y) - size.Height / 2);
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ob"></param>
        public void Remove(object ob)
        {
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SelectedInfo = null;
                SelectedInfoChanged = null;

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

                if (Panel != null)
                {
                    if (!Panel.IsDisposed)
                        Panel.Dispose();

                    Panel = null;
                }
            }
        }

        #endregion
    }
}
