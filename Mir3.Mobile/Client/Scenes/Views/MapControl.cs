using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 地图控件
    /// </summary>
    public sealed class MapControl : DXControl
    {
        #region Properties

        /// <summary>
        /// 自动寻路开关
        /// </summary>
        public bool AutoPath
        {
            get
            {
                return _autoPath;
            }
            set
            {
                if (_autoPath == value) return;
                _autoPath = value;
                if (!_autoPath)
                    CurrentPath = null;
                if (GameScene.Game != null)
                {
                    if (GameScene.Game.BigMapBox != null)
                        GameScene.Game.BigMapBox.UpdatePathToDraw = true;

                    if (GameScene.Game.MiniMapBox != null)
                        GameScene.Game.MiniMapBox.UpdatePathToDraw = true;
                }
                //消息异步会异常，注释掉
                //if (GameScene.Game != null)
                //    GameScene.Game.ReceiveChat(value? "[寻路:开 (停止:ALT+鼠标左键)]" : "[寻路:关]", MessageType.Hint);
            }
        }
        private bool _autoPath;
        public PathFinder PathFinder;
        public List<Node> CurrentPath
        {
            get
            {
                return _currentPath;
            }
            set
            {
                if (_currentPath != value && GameScene.Game != null)
                {
                    if (GameScene.Game.BigMapBox != null)
                        GameScene.Game.BigMapBox.UpdatePathToDraw = true;

                    if (GameScene.Game.MiniMapBox != null)
                        GameScene.Game.MiniMapBox.UpdatePathToDraw = true;

                    _currentPath = value;
                }
            }
        }
        private List<Node> _currentPath;
        public static UserObject User => GameScene.Game.User;
        private DateTime pathfindertime;

        /// <summary>
        /// 钓鱼地块
        /// </summary>
        public Point FishingCellPoint { get; set; }

        #region MapInformation
        /// <summary>
        /// 地图信息
        /// </summary>
        public MapInfo MapInfo
        {
            get => _MapInfo;
            set
            {
                if (_MapInfo == value) return;

                MapInfo oldValue = _MapInfo;
                _MapInfo = value;

                OnMapInfoChanged(oldValue, value);
            }
        }
        private MapInfo _MapInfo;
        public event EventHandler<EventArgs> MapInfoChanged;
        /// <summary>
        /// 地图信息改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public void OnMapInfoChanged(MapInfo oValue, MapInfo nValue)
        {
            TextureValid = false;
            AutoPath = false;
            LoadMap();
            if (oValue != null)  //如果地图信息不为空
            {
                if (nValue == null || nValue.Music != oValue.Music)  //如果地图音乐为空  或者音乐不对
                    DXSoundManager.Stop(oValue.Music);    //停止播放

                if (nValue == null || nValue.MapSound != oValue.MapSound)  //如果地图音乐为空  或者自定义音乐不对
                    DXSoundManager.Stop(oValue.MapSound);    //停止播放
            }

            if (nValue != null)          //如果音乐信息不为空
            {
                if (nValue.Music != SoundIndex.None)
                    DXSoundManager.Play(nValue.Music);    //播放音乐
                else
                    DXSoundManager.Play(nValue.MapSound);  //播放自定义音乐
            }

            LLayer.UpdateLights();
            UpdateWeather();
            PathFinder = new PathFinder(this);
            GameScene.Game?.BigPatchBox?.OnMapInfoChanged();
            MapInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Animation
        /// <summary>
        /// 动画
        /// </summary>
        public int Animation
        {
            get => _Animation;
            set
            {
                if (_Animation == value) return;

                int oldValue = _Animation;
                _Animation = value;

                OnAnimationChanged(oldValue, value);
            }
        }
        private int _Animation;
        public event EventHandler<EventArgs> AnimationChanged;
        public void OnAnimationChanged(int oValue, int nValue)
        {
            AnimationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion      

        #region MouseLocation
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public Point MouseLocation
        {
            get => _MouseLocation;
            set
            {
                if (_MouseLocation == value) return;

                Point oldValue = _MouseLocation;
                _MouseLocation = value;

                OnMouseLocationChanged(oldValue, value);
            }
        }
        private Point _MouseLocation;
        public event EventHandler<EventArgs> MouseLocationChanged;
        public void OnMouseLocationChanged(Point oValue, Point nValue)
        {
            MouseLocationChanged?.Invoke(this, EventArgs.Empty);
            UpdateMapLocation();
        }

        #endregion

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (FLayer != null)
                FLayer.Size = Size;

            if (LLayer != null)
                LLayer.Size = Size;


            OffSetX = Size.Width / 2 / CellWidth;
            OffSetY = Size.Height / 2 / CellHeight;

            //粒子数量，分辨率（粒子数量需要根据分辨率设置一个适合的值，否则分辨率过大，雨点，雪比较稀）
            m_xSnow.SetupSystem(500 * (Config.GameSize.Width / 800), Config.GameSize.Width, Config.GameSize.Height);
            m_xRain.SetupSystem(400 * (Config.GameSize.Width / 800), Config.GameSize.Width, Config.GameSize.Height);
            m_xMist.Init(Config.GameSize.Width, Config.GameSize.Height);
        }

        public MouseButtons MapButtons;
        /// <summary>
        /// 地图坐标
        /// </summary>
        public Point MapLocation;
        /// <summary>
        /// 挖矿
        /// </summary>
        public bool Mining;
        /// <summary>
        /// 挖矿坐标
        /// </summary>
        public Point MiningPoint;
        public MirDirection MiningDirection;

        public bool Harvest;

        public Floor FLayer;
        public Light LLayer;

        public MapCells Cells;
        public int Width, Height;

        public List<MapObject> Objects = new List<MapObject>();
        public List<MirEffect> Effects = new List<MirEffect>();

        public const int CellWidth = 48, CellHeight = 32;

        public int ViewRangeX = 12, ViewRangeY = 24;

        public static int OffSetX;
        public static int OffSetY;

        public MirImage BackgroundImage;           //地图背景最底层
        public float BackgroundScaleX, BackgroundScaleY;
        public Point BackgroundMovingOffset = Point.Empty;

        #endregion

        /// <summary>
        /// 地图控件
        /// </summary>
        public MapControl()
        {
            DrawTexture = true;

            BackColour = Color.Empty;

            FLayer = new Floor { Parent = this, Size = Size };
            LLayer = new Light { Parent = this, Location = new Point(-GameScene.Game.Location.X, -GameScene.Game.Location.Y), Size = Size };

            //鼠标位置初始化屏幕中间
            MouseLocation = ZoomRate == 1F ? new Point((CEnvir.Target.Width / 2), (CEnvir.Target.Height / 2)) : new Point((int)Math.Round(((CEnvir.Target.Width / 2) - UI_Offset_X) / ZoomRate), (int)Math.Round((CEnvir.Target.Height / 2) / ZoomRate));

            #region 粒子相关

            m_xFlyingTail.SetupSystem();
            m_xSmoke.SetupSystem();
            m_xBoom.SetupSystem();
            m_xScatter.SetupSystem();

            //粒子数量，分辨率（粒子数量需要根据分辨率设置一个适合的值，否则分辨率过大，雨点，雪比较稀）
            m_xSnow.SetupSystem(500 * (Config.GameSize.Width / 800), Config.GameSize.Width, Config.GameSize.Height);
            m_xRain.SetupSystem(400 * (Config.GameSize.Width / 800), Config.GameSize.Width, Config.GameSize.Height);
            m_xMist.Init(Config.GameSize.Width, Config.GameSize.Height);

            #endregion
        }

        #region Methods
        /// <summary>
        /// 纹理清理
        /// </summary>
        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (FLayer.TextureValid)
                DXManager.Sprite.Draw(FLayer.ControlTexture, Color.White);

            if (Config.DrawEffects)
            {
                foreach (MirEffect ob in Effects)
                {
                    //绘制地面魔法
                    if (ob.DrawType != DrawType.Floor) continue;

                    ob.Draw();
                }
            }

            DrawObjects();

            if (MapObject.MouseObject != null) // && MapObject.MouseObject != MapObject.TargetObject)
                MapObject.MouseObject.DrawBlend();

            if (Config.DrawEffects)
            {
                foreach (MirEffect ob in Effects)
                {
                    if (ob.DrawType != DrawType.Final) continue;

                    ob.Draw();
                }
            }

            foreach (MapObject ob in Objects)
            {
                ob.DrawPoison();
            }

            if (BigPatchConfig.ShowDamageNumbers)
                foreach (MapObject ob in Objects)
                    ob.DrawDamage();

        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            FLayer.CheckTexture();
            LLayer.CheckTexture();

            OnBeforeDraw();

            DrawControl();

            OnAfterDraw();
        }
        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            PresentMirImage(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            DXManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 绘制物体
        /// </summary>
        private void DrawObjects()
        {
            int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 4), maxX = Math.Min(Width - 1, User.CurrentLocation.X + OffSetX + 4);
            int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 4), maxY = Math.Min(Height - 1, User.CurrentLocation.Y + OffSetY + 25);

            for (int y = minY; y <= maxY; y++)
            {
                int drawY = (y - User.CurrentLocation.Y + OffSetY + 1) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                for (int x = minX; x <= maxX; x++)
                {
                    int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                    Cell cell = Cells[x, y];

                    MirLibrary library;
                    LibraryFile file;

                    if (cell.MiddleFile > 0)
                    {
                        if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.MiddleImage - 1;
                            if (index >= 0)
                            {
                                bool blend = false;
                                if (cell.MiddleAnimationFrame > 0 && cell.MiddleAnimationFrame < 255)
                                {
                                    var animation = cell.MiddleAnimationFrame & 0x0F;
                                    blend = (cell.MiddleAnimationFrame & 0x80) > 0;
                                    var animationTick = cell.MiddleAnimationTick;
                                    index += Animation % (animation + animation * animationTick) / (1 + animationTick);
                                }
                                Size s = library.GetSize(index);
                                //绘制中间层大物体（不是 48*32 或96*64 的地砖），标准地砖在OnClearTexture()已经绘制
                                if (!((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2)))
                                {
                                    if (!blend)
                                        library.Draw(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                                    else
                                        library.DrawBlend(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                                }
                            }
                        }
                    }

                    if (cell.FrontFile != -1)
                    {
                        if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                        {
                            int index = (cell.FrontImage & 0x7FFF) - 1;
                            if (index >= 0)
                            {
                                bool blend = false;
                                if (cell.FrontAnimationFrame > 0 && cell.FrontAnimationFrame < 255)
                                {
                                    var animation = cell.FrontAnimationFrame & 0x0F;
                                    blend = (cell.FrontAnimationFrame & 0x80) > 0;
                                    var animationTick = cell.FrontAnimationTick;
                                    index += Animation % (animation + animation * animationTick) / (1 + animationTick);
                                }
                                Size s = library.GetSize(index);
                                //绘制前景层大物体（不是 48*32 或96*64 的地砖），标准地砖在OnClearTexture()已经绘制
                                if (!((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2)))
                                {
                                    //如果需要混合
                                    if (blend)
                                    {
                                        //新盛大地图
                                        if ((cell.FrontFile > 99) & (cell.FrontFile < 199))
                                            library.DrawBlend(index, drawX, drawY - 3 * CellHeight, Color.White, true, 1F, ImageType.Image);
                                        //传3地图
                                        else if (cell.FrontFile > 199)
                                            library.DrawBlend(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                                        //老地图灯柱 index >= 2723 && index <= 2732
                                        else
                                            library.DrawBlend(index, drawX, drawY - s.Height, Color.White, index >= 2723 && index <= 2732, 1F, ImageType.Image);
                                    }
                                    //不需要混合
                                    else
                                        library.Draw(index, drawX, drawY - s.Height, Color.White, false, 1F, ImageType.Image);
                                }
                            }
                        }
                    }
                }

                for (int x = minX; x <= maxX; x++)
                {
                    Cells[x, y].DrawDeadObjects();
                }

                for (int x = minX; x <= maxX; x++)
                {
                    Cells[x, y].DrawObjects();
                }

                if (Config.DrawEffects)
                {
                    foreach (MirEffect ob in Effects)
                    {
                        if (ob.DrawType != DrawType.Object) continue;

                        //如果魔法地图坐标为空 和 魔法目标不为空 （有攻击目标的魔法）
                        if (ob.MapTarget.IsEmpty && ob.Target != null)
                        {
                            //魔法目标不是自己
                            if (ob.Target.RenderY == y && ob.Target != User)
                                ob.Draw();
                        }
                        //地图坐标等于当前坐标的魔法
                        else if (ob.MapTarget.Y == y)
                            ob.Draw();
                    }
                }
            }

            if (Config.DrawEffects)
            {
                foreach (MirEffect ob in Effects)
                {
                    //绘制坐标为空and目标为自己的魔法(如推手动作，魔法盾等)
                    if (ob.DrawType != DrawType.Object || !ob.MapTarget.IsEmpty || ob.Target != User) continue;

                    ob.Draw();
                }
            }

            //船浪花动画
            if (FLayer.IsBoat)
            {
                int index = 1200;
                int framecount = 10;
                int animationTick = 1;
                index += Animation % (framecount + framecount * animationTick) / (1 + animationTick);

                MirLibrary library;
                if (CEnvir.LibraryList.TryGetValue(LibraryFile.MonMagicEx25, out library))
                {
                    int drawX = (12 - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X - 35;
                    int drawY = (14 - User.CurrentLocation.Y + OffSetY) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y - 12;
                    library.DrawBlend(index, drawX, drawY, Color.White, false, 1F, ImageType.Image);
                }

            }

            #region 粒子相关

            if (Config.EnableParticle)
            {
                if (CEnvir.Now >= ParticleRenderTime)
                {
                    int looptime = CEnvir.FrameTime < 17 ? 17 : (int)CEnvir.FrameTime;
                    ParticleRenderTime = CEnvir.Now.AddMilliseconds(looptime);
                    m_xFlyingTail.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                    m_xSmoke.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                    m_xBoom.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                    if (m_bShowSnow)
                    {
                        m_xSnow.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                    }
                    if (m_bShowRain)
                    {
                        m_xRain.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                    }
                    m_xScatter.UpdateSystem(looptime, new Vector3(0f, 0f, 0f));
                }

                m_xFlyingTail.RenderSystem();
                m_xSmoke.RenderSystem();
                m_xBoom.RenderSystem();
                if (m_bShowMist)
                {
                    m_xMist.ProgressMist();
                }

                if (m_bShowSnow)
                {
                    m_xSnow.RenderSystem();
                }

                if (m_bShowRain)
                {
                    m_xRain.RenderSystem();
                }

                m_xScatter.RenderSystem();
            }

            #endregion

            //if (User.Opacity != 1f) return;
            //绘制前景层本体及外观特效（此绘制在前景层之上，在屋檐或树下呈现本体透明）
            float oldOpacity = MapObject.User.Opacity;
            //如果没隐身 透明度0.65
            if (User.Opacity == 1f)
                MapObject.User.Opacity = 0.65F;
            if (MapObject.User.DrawWingsBehind())
                MapObject.User.DrawWings();
            if (MapObject.User.DrawWeaponEffectBehind())
                MapObject.User.DrawWeaponEffect();
            if (MapObject.User.DrawShieldEffectBehind())
                MapObject.User.DrawShieldEffect();

            MapObject.User.DrawEmblemEffect();
            MapObject.User.DrawBody(oldOpacity != 1F); //隐身画影子

            if (MapObject.User.DrawWingsInfront())
                MapObject.User.DrawWings();
            if (MapObject.User.DrawWeaponEffectInfront())
                MapObject.User.DrawWeaponEffect();
            if (MapObject.User.DrawShieldEffectInfront())
                MapObject.User.DrawShieldEffect();
            MapObject.User.Opacity = oldOpacity;
        }
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            foreach (MapObject ob in Objects)
            {
                //if (ob.Dead) continue;

                switch (ob.Race)
                {
                    case ObjectType.Player:
                        if (!BigPatchConfig.ShowPlayerNames) continue;

                        break;
                    case ObjectType.Item:
                        if (!BigPatchConfig.ChkItemObjShow || ob.CurrentLocation == MapLocation) continue;
                        break;
                    case ObjectType.NPC:
                        if (!BigPatchConfig.ShowPlayerNames) continue;

                        break;
                    case ObjectType.Spell:
                        break;
                    case ObjectType.Monster:
                        if (!BigPatchConfig.ChkMonsterNameTips) continue;
                        break;
                }

                ob.DrawName();
            }

            if (MapObject.MouseObject != null && MapObject.MouseObject.Race != ObjectType.Item)
                MapObject.MouseObject.DrawName();

            foreach (MapObject ob in Objects)
            {
                ob.DrawChat();
                ob.DrawHealth();
            }

            if (MapLocation.X >= 0 && MapLocation.X < Width && MapLocation.Y >= 0 && MapLocation.Y < Height)
            {
                Cell cell = Cells[MapLocation.X, MapLocation.Y];
                int layer = 0;
                if (cell.Objects != null)
                    for (int i = cell.Objects.Count - 1; i >= 0; i--)
                    {
                        ItemObject ob = cell.Objects[i] as ItemObject;

                        ob?.DrawFocus(layer++);
                    }
            }

#if Mobile
            MirLibrary mouse;
            if (Objects != null)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    var ob = Objects[i];

                    if (ob == MapObject.MouseObject || ob == MapObject.TargetObject || ob == MapObject.MagicObject)
                    {
                        if (ob.Race == ObjectType.Monster || ob.Race == ObjectType.Player)
                        {
                            if (ob == MapObject.User) continue;
                            if (CEnvir.LibraryList.TryGetValue(LibraryFile.PhoneUI, out mouse))
                                mouse.Draw(17, ob.DrawX, ob.DrawY, Color.White, true, 0.8F, ImageType.Image, zoomRate: ZoomRate);

                        }

                    }
                }
            }
#endif

            DXManager.Sprite.End();
            DXManager.Device.SetRenderState(DXManager.LLayerBlend);
            DXManager.Sprite.Begin();
            if (ZoomRate == 1F)
                DXManager.Sprite.Draw(LLayer.ControlTexture, Color.White);
            else
                DXManager.Sprite.DrawZoom(LLayer.ControlTexture, Color.White, ZoomRate);
            DXManager.Sprite.End();
            DXManager.Device.SetRenderState(DXManager.AlphaBlend);
            DXManager.Sprite.Begin();


#if Mobile
            //if (CEnvir.LibraryList.TryGetValue(LibraryFile.PhoneUI, out mouse))
            //    mouse.Draw(18, MouseLocation.X, MouseLocation.Y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate);
#endif
        }
        /// <summary>
        /// 更新天气效果
        /// 优先级： 大补贴 > 服务器设置 > 地图本身的设置
        /// </summary>
        public void UpdateWeather()
        {
            m_bShowMist = false;
            m_bShowRain = false;
            m_bShowSnow = false;
            WeatherSetting weather = (WeatherSetting)BigPatchConfig.Weather;

            if (weather == WeatherSetting.None)
            {
                // 大补贴没有进行天气设置
                // 检查服务器是否有设置
                if (!CEnvir.WeatherOverrides.TryGetValue(GameScene.Game.MapControl.MapInfo.Index, out weather))
                {
                    // 服务器也没有进行天气设置
                    // 读取地图默认天气
                    weather = GameScene.Game.MapControl.MapInfo.Weather;
                }
            }

            switch (weather)
            {
                case WeatherSetting.Fog:         //雾             
                    m_bShowMist = true;
                    break;

                //case WeatherSetting.BurningFog:   //燃烧的雾

                //break;

                case WeatherSetting.Snow:         //雪             
                    m_bShowSnow = true;
                    break;

                //case WeatherSetting.Everfall:         //花瓣雨             

                //break;

                case WeatherSetting.Rain:         //雨             
                    m_bShowRain = true;
                    break;
            }
        }

        /// <summary>
        /// 载入地图
        /// </summary>
        private void LoadMap()
        {
            try
            {
                if (!File.Exists(CEnvir.MobileClientPath + Config.MapPath + MapInfo.FileName + ".map")) return;

                DXManager.ForceReworkTextures();
                Cells = new MapCells(CEnvir.MobileClientPath + Config.MapPath + MapInfo.FileName + ".map");
                Width = Cells.Width;
                Height = Cells.Height;

                FLayer.IsBoat = MapInfo.FileName.ToLower() == "juma_s01";
                //创建钓鱼区域
                CreateFishingArea();
                //手动垃圾回收
                GC.Collect(2, GCCollectionMode.Forced);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }

            foreach (MapObject ob in Objects)
                if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                    Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].AddObject(ob);
        }

        /// <summary>
        /// 创建钓鱼区域
        /// </summary>
        private void CreateFishingArea()
        {
            foreach (FishingAreaInfo info in CEnvir.FishingAreaInfoList.Binding)
            {
                if (info.BindRegion == null) continue;
                if (info.BindRegion.Map != MapInfo) continue;

                info.BindRegion.CreatePoints(Width);
                foreach (Point point in info.BindRegion.PointList)
                {
                    //如果可以移动的区域，返回
                    if (!Cells[point.X, point.Y].Flag) continue;
                    Cells[point.X, point.Y].FishingCell = true;
                }
            }
        }

        /// <summary>
        /// 鼠标坐标转换为游戏坐标
        /// </summary>
        /// <param name="pixelPoint"></param>
        /// <returns></returns>
        public Point ConvertMouseToGame(Point pixelPoint)
        {
            return new Point((pixelPoint.X - GameScene.Game.Location.X) / CellWidth - OffSetX + User.CurrentLocation.X,
                                (pixelPoint.Y - GameScene.Game.Location.Y) / CellHeight - OffSetY + User.CurrentLocation.Y);
        }
        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //MouseLocation = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
        }
        /// <summary>
        /// 鼠标按键时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (GameScene.Game.Observer) return;

            MapButtons |= e.Button;

            if (e.Button == MouseButtons.Right)
            {
                if (BigPatchConfig.RightClickDeTarget && MapObject.TargetObject?.Race == ObjectType.Monster)
                    MapObject.TargetObject = null;

                if (GameScene.Game.User.YeManCanRun)
                {
                    GameScene.Game.User.YeManCanRun = false;

                    GameScene.Game.CanRun = true;
                }
            }
            //鼠标中键
            if (e.Button == MouseButtons.Middle)
            {
                if (BigPatchConfig.ChkCallMounts)
                {
                    if (CEnvir.Now < User.NextActionTime || User.ActionQueue.Count > 0) return;
                    if (CEnvir.Now < User.ServerTime) return;
                    if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer)
                    {
                        GameScene.Game.ReceiveChat("战斗中无法骑马".Lang(), MessageType.System);
                        return;
                    }

                    User.ServerTime = CEnvir.Now.AddSeconds(5);
                    CEnvir.Enqueue(new C.Mount());
                }
                else if (BigPatchConfig.ChkCastingMagic)
                {
                    GameScene.Game.UseMagic((SpellKey)BigPatchConfig.MiddleMouse);
                }
            }

            if (e.Button != MouseButtons.Left) return;

            DXItemCell cell = DXItemCell.SelectedCell;
            if (cell != null)
            {
                MapButtons &= ~e.Button;

                if (cell.GridType == GridType.Belt)
                {
                    cell.QuickInfo = null;
                    cell.QuickItem = null;
                    DXItemCell.SelectedCell = null;

                    ClientBeltLink link = GameScene.Game.BeltBox.Links[cell.Slot];
                    CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update serve
                    return;
                }

                if ((cell.Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (cell.GridType != GridType.PatchGrid && cell.GridType != GridType.Inventory && cell.GridType != GridType.CompanionInventory))
                {
                    DXItemCell.SelectedCell = null;
                    return;
                }

                if ((cell.Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (cell.GridType != GridType.PatchGrid && cell.GridType != GridType.Inventory && cell.GridType != GridType.CompanionInventory))
                {
                    DXItemCell.SelectedCell = null;
                    return;
                }

                if (cell.Item.Count > 1)
                {
                    DXItemAmountWindow window = new DXItemAmountWindow("丢弃道具".Lang(), cell.Item);

                    window.ConfirmButton.MouseClick += (o, a) =>  //固定按钮  鼠标单击
                    {
                        if (window.Amount <= 0) return;  //如果数量小于或者等于0 返回

                        CEnvir.Enqueue(new C.ItemDrop
                        {
                            Link = new CellLinkInfo { GridType = cell.GridType, Slot = cell.Slot, Count = window.Amount }
                        });

                        cell.Locked = true;
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.ItemDrop
                    {
                        Link = new CellLinkInfo { GridType = cell.GridType, Slot = cell.Slot, Count = 1 }
                    });

                    cell.Locked = true;
                }

                DXItemCell.SelectedCell = null;
                return;
            }

            if (GameScene.Game.GoldPickedUp)
            {
                MapButtons &= ~e.Button;
                DXItemAmountWindow window = new DXItemAmountWindow("掉落道具".Lang(), new ClientUserItem(Globals.GoldInfo, User.Gold));

                window.ConfirmButton.MouseClick += (o, a) =>
                {
                    if (window.Amount <= 0) return;

                    CEnvir.Enqueue(new C.GoldDrop
                    {
                        Amount = window.Amount
                    });

                };

                GameScene.Game.GoldPickedUp = false;
                return;
            }

            if (CanAttack(MapObject.MouseObject))
            {
                MapObject.TargetObject = MapObject.MouseObject;

                if (MapObject.MouseObject.Race == ObjectType.Monster && ((MonsterObject)MapObject.MouseObject).MonsterInfo.AI >= -1) //Check if AI is guard
                {
                    MapObject.MagicObject = MapObject.TargetObject;
                    GameScene.Game.FocusObject = MapObject.MouseObject;
                }
                return;
            }

            MapObject.TargetObject = null;
            GameScene.Game.FocusObject = null;
            //GameScene.Game.OldTargetObjectID = 0;
        }
        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (e.Button)
            {
                //鼠标左键点击
                case MouseButtons.Left:
                    if (GameScene.Game.Observer) return;
                    //有鼠标干预，关闭自动寻路
                    if (AutoPath && !BigPatchConfig.AndroidPlayer)
                    {
                        GameScene.Game.ReceiveChat("BigMap.PathfindingOff".Lang(), MessageType.Hint);
                        AutoPath = false;
                    }

                    GameScene.Game.AutoRun = false;

                    #region 点击地图特定坐标出现功能

                    if (!CEnvir.ClientControl.CoinPlaceChoiceCheck)
                    {
                        //如果当前地图调用的图片是1 道馆
                        if (MapInfo.FileName.ToLower() == "1")
                        {
                            Point mousePoint = ConvertMouseToGame(e.Location);
                            Point targetPoint1 = new Point(x: 389, y: 133);
                            Point targetPoint2 = new Point(x: 397, y: 139);

                            //指针坐标与目标的坐标 误差距离小于等于1
                            if (Functions.InRange(mousePoint, targetPoint1, 1) || Functions.InRange(mousePoint, targetPoint2, 1))
                            {
                                GameScene.Game.VowBox.Visible = !GameScene.Game.VowBox.Visible;
                                return;
                            }
                        }
                    }
                    else
                    {
                        //如果当前地图调用的图片是15 泰山
                        if (MapInfo.FileName.ToLower() == "15")
                        {
                            Point mousePoint = ConvertMouseToGame(e.Location);
                            Point targetPoint1 = new Point(x: 676, y: 137);
                            Point targetPoint2 = new Point(x: 677, y: 138);
                            Point targetPoint3 = new Point(x: 678, y: 139);
                            Point targetPoint4 = new Point(x: 679, y: 139);

                            //指针坐标与目标的坐标 误差距离小于等于1
                            if (Functions.InRange(mousePoint, targetPoint1, 1) || Functions.InRange(mousePoint, targetPoint2, 1) || Functions.InRange(mousePoint, targetPoint3, 1) || Functions.InRange(mousePoint, targetPoint4, 1))
                            {
                                GameScene.Game.VowBox.Visible = !GameScene.Game.VowBox.Visible;
                                return;
                            }
                        }
                    }
                    #endregion

                    //钓鱼判断
                    if (GameScene.Game.User.HasFishingRod && MapObject.User.HasFishingArmour && MapObject.User.CurrentAction == MirAction.Standing)
                    {
                        Point mousePoint = ConvertMouseToGame(e.Location);
                        if (mousePoint.X >= Width || mousePoint.Y >= Height) return;

                        FishingCellPoint = Functions.Move(User.CurrentLocation, User.Direction, 2);
                        if (FishingCellPoint.X >= Width || FishingCellPoint.Y >= Height) return;

                        Cell fishingCell = Cells[FishingCellPoint.X, FishingCellPoint.Y];
                        if (fishingCell.FishingCell)
                        {
                            //抛竿
                            MirDirection fishingDirection =
                                Functions.DirectionFromPoint(User.CurrentLocation, mousePoint);

                            //MapObject.User.ActionQueue.Add(new ObjectAction(MirAction.FishingCast, fishingDirection, MapObject.User.CurrentLocation));

                            //告知服务端 开始钓鱼
                            CEnvir.Enqueue(new C.FishingCast());
                        }
                        else
                        {
                            if (MapObject.MouseObject == null)
                                GameScene.Game.ReceiveChat("这里不能钓鱼".Lang(), MessageType.Hint);
                        }
                    }

                    if (MapObject.MouseObject == null) return;
                    NPCObject npc = MapObject.MouseObject as NPCObject;
                    if (npc != null)
                    {
                        if (CEnvir.Now <= GameScene.Game.NPCTime) return;

                        GameScene.Game.NPCTime = CEnvir.Now.AddSeconds(1);

                        CEnvir.Enqueue(new C.NPCCall { ObjectID = npc.ObjectID, Key = @"[@MAIN]" });
                    }
                    break;
                //鼠标右键点击
                case MouseButtons.Right:
                    //有鼠标干预，关闭自动寻路
                    if (AutoPath && !BigPatchConfig.AndroidPlayer)
                    {
                        GameScene.Game.ReceiveChat("BigMap.PathfindingOff".Lang(), MessageType.Hint);
                        AutoPath = false;
                    }
                    //鼠标右键点击 自动跑不停结束
                    GameScene.Game.AutoRun = false;
                    //如果角色当前动作是站立 跑动结束
                    if (User.CurrentAction == MirAction.Standing)
                    {
                        GameScene.Game.CanRun = false;
                        GameScene.Game.CanPush = false;
                    }

                    //如果没有按下Ctrl 就结束
                    if (!CEnvir.Ctrl) return;

                    //下面的代码是Ctrl加鼠标右键点击玩家时 发送查看角色界面封包
                    PlayerObject player = MapObject.MouseObject as PlayerObject;
                    //如果玩家为空或者玩家是自己跳出
                    if (player == null || player == MapObject.User) return;
                    //如果地图设置为不显示玩家名字，就无法查看人物界面
                    if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;
                    if (CEnvir.Now <= GameScene.Game.InspectTime && player.ObjectID == GameScene.Game.InspectID) return;

                    GameScene.Game.InspectTime = CEnvir.Now.AddMilliseconds(2500);
                    GameScene.Game.InspectID = player.ObjectID;
                    CEnvir.Enqueue(new C.Inspect { Index = player.CharacterIndex });
                    break;
            }
        }
        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            //绕过过滤强制拾取
            if (GameScene.Game.Observer) return;
            if (User.Dead) return;
            if (MapLocation != User.CurrentLocation) return;
            GameScene.Game.PickUpTime = CEnvir.Now.AddMilliseconds(250);

            Cell cell = Cells[User.CurrentLocation.X, User.CurrentLocation.Y];
            if (cell?.Objects == null) return;

            foreach (MapObject cellObject in cell.Objects)
            {
                if (cellObject.Race != ObjectType.Item) continue;
                ItemObject item = (ItemObject)cellObject;

                int itemIdx = item.Item.Info.Index;
                if (item.Item.Info.Effect == ItemEffect.ItemPart)
                {
                    itemIdx = item.Item.AddedStats[Stat.ItemIndex];
                }
                CEnvir.Enqueue(new C.PickUp
                {
                    ItemIdx = itemIdx,
                    Xpos = User.CurrentLocation.X,
                    Ypos = User.CurrentLocation.Y,
                });
            }
        }
        /// <summary>
        /// 检查游标
        /// </summary>
        public void CheckCursor()
        {
            MapObject deadObject = null, itemObject = null;

            for (int d = 0; d < 4; d++)
            {
                for (int y = MapLocation.Y - d; y <= MapLocation.Y + d; y++)
                {
                    if (y >= Height) continue;
                    if (y < 0) break;
                    for (int x = MapLocation.X - d; x <= MapLocation.X + d; x++)
                    {
                        if (x >= Width) continue;
                        if (x < 0) break;

                        List<MapObject> list = Cells[x, y].Objects;
                        if (list == null) continue;

                        MapObject cellSelect = null;
                        for (int i = list.Count - 1; i >= 0; i--)
                        {
                            MapObject ob = list[i];

                            if (ob == MapObject.User || ob.Race == ObjectType.Spell || ((x != MapLocation.X || y != MapLocation.Y) && !ob.MouseOver(MouseLocation))) continue;

                            if (ob.Dead || (ob.Race == ObjectType.Monster && ((MonsterObject)ob).CompanionObject != null))
                            {
                                if (deadObject == null)
                                    deadObject = ob;
                                continue;
                            }
                            if (ob.Race == ObjectType.Item)
                            {
                                if (itemObject == null)
                                    itemObject = ob;
                                continue;
                            }
                            if (x == MapLocation.X && y == MapLocation.Y && !ob.MouseOver(MouseLocation))
                            {
                                if (cellSelect == null)
                                    cellSelect = ob;
                            }
                            else
                            {
                                MapObject.MouseObject = ob;
                                return;
                            }
                        }

                        if (cellSelect != null)
                        {
                            MapObject.MouseObject = cellSelect;
                            return;
                        }
                    }
                }
            }

            MapObject mouseOb = deadObject ?? itemObject;

            if (mouseOb == null)
            {
                if ((User.CurrentLocation.X == MapLocation.X && User.CurrentLocation.Y == MapLocation.Y) || User.MouseOver(MouseLocation))
                    mouseOb = User;
            }


            MapObject.MouseObject = mouseOb;
        }
        /// <summary>
        /// 过程处理
        /// </summary>
        public void ProcessInput()
        {
            if (GameScene.Game.Observer) return;

            if (User.Dead || (User.Poison & PoisonType.ThousandBlades) == PoisonType.ThousandBlades || (User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (User.Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike || User.Buffs.Any(x => x.Type == BuffType.DragonRepulse || x.Type == BuffType.FrostBite)) return;

            if (User.MagicAction != null)
            {
                if (CEnvir.Now < MapObject.User.NextActionTime || MapObject.User.ActionQueue.Count != 0) return;

                //  if (QueuedMagic.Action == MirAction.Magic && (Spell)QueuedMagic.Extra[1] == Magic.ShoulderDash && !GameScene.Game.MoveFrame) return;

                MapObject.User.AttemptAction(User.MagicAction);
                User.MagicAction = null;
                Mining = false;
            }

            bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);

            if (MapObject.TargetObject != null && !MapObject.TargetObject.Dead && ((MapObject.TargetObject.Race == ObjectType.Monster && string.IsNullOrEmpty(MapObject.TargetObject.PetOwner)) || (CEnvir.Shift || BigPatchConfig.ChkAvertShift)))
            {
                #region 挂机
                //如果挂机
                if (BigPatchConfig.AndroidPlayer)
                {
                    pathfindertime = CEnvir.Now.AddSeconds(2);

                    //如果法师 或者 道士 并且配备了远程挂机魔法
                    if ((User.Class == MirClass.Wizard || User.Class == MirClass.Taoist || MapObject.User.Class == MirClass.Assassin) && BigPatchConfig.AndroidSingleSkill && BigPatchConfig.AndroidSkills != MagicType.None)
                    {
                        //如果自动躲避
                        if (BigPatchConfig.AndroidEluded)
                        {
                            //与目标距离
                            int dis = Functions.Distance(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation);
                            //如果与目标距离小于3
                            if (dis < 3)
                            {
                                //目标方向
                                MirDirection dir = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation);
                                //反方向
                                dir = Functions.ShiftDirection(dir, 4);
                                //如果反方向不能移动
                                if (!CanMove(dir, 1))
                                {
                                    //取个最佳方向
                                    MirDirection bestdir = DirectionBest(dir, 1, MapObject.TargetObject.CurrentLocation);

                                    //如果最佳方向不能移动，就循环周围格子能否移动
                                    if (!CanMove(bestdir, 1))
                                    {
                                        for (int x = -1; x <= 1; x++)
                                        {
                                            for (int y = -1; y <= 1; y++)
                                            {
                                                if (x == 0 && y == 0) continue;

                                                Point emp = new Point(User.CurrentLocation.X + x, User.CurrentLocation.Y + y);
                                                if (EmptyCell(emp))
                                                {
                                                    bestdir = Functions.DirectionFromPoint(User.CurrentLocation, emp);
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    dir = bestdir;
                                }

                                if (CanMove(dir, 1) && !haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                                {
                                    //MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, dir, Functions.Move(MapObject.User.CurrentLocation, dir), 1, MagicType.None));
                                    Run(dir);
                                    return;
                                }
                            }
                        }

                        //释放挂机魔法
                        if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, Globals.MagicRange - 1))
                        {
                            if (CEnvir.Now < MapObject.User.NextMagicTime) return;
                            if (MapObject.User.MagicAction != null) return;

                            if (AutoPath) AutoPath = false;
                            //如果道士打开了自动上毒
                            if (BigPatchConfig.AndroidPoisonDust && MapObject.User.Class == MirClass.Taoist)
                            {
                                bool poison = false;
                                //如果打开了自动换毒，就红绿毒一起放
                                if (BigPatchConfig.AutoPoisonDust && (((MapObject.TargetObject.Poison & PoisonType.Red) != PoisonType.Red) || ((MapObject.TargetObject.Poison & PoisonType.Green) != PoisonType.Green)))
                                    poison = true;
                                //如果人物装配了毒
                                else if (GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.ItemType == ItemType.Poison)
                                {
                                    int shape = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.Shape ?? -1;
                                    PoisonType type = shape == 0 ? PoisonType.Green : PoisonType.Red;
                                    if ((MapObject.TargetObject.Poison & type) != type)
                                        poison = true;
                                }

                                if (poison)
                                {
                                    GameScene.Game.UseMagic(MagicType.PoisonDust);
                                    return;
                                }
                            }
                            GameScene.Game.UseMagic(BigPatchConfig.AndroidSkills);
                            return;
                        }
                        else
                        {
                            if (!AutoPath)
                            {
                                if (!Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, Globals.MagicRange - 3))
                                {
                                    if (!PathFinder.bSearching)
                                    {
                                        PathFinder.bSearching = true;
                                        Task.Run(() =>
                                        {
                                            try
                                            {
                                                InitCurrentPath(MapObject.TargetObject.CurrentLocation.X, MapObject.TargetObject.CurrentLocation.Y);

                                                PathFinder.bSearching = false;
                                            }
                                            catch
                                            {
                                                PathFinder.bSearching = false;
                                            }
                                        });
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //如果目标距离等于1 就关闭寻路
                        if (Functions.Distance(User.CurrentLocation, MapObject.TargetObject.CurrentLocation) <= 1)
                        {
                            if (AutoPath) AutoPath = false;
                        }
                        else
                        {
                            if (!AutoPath)
                            {
                                if (!PathFinder.bSearching)
                                {
                                    PathFinder.bSearching = true;
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            InitCurrentPath(MapObject.TargetObject.CurrentLocation.X, MapObject.TargetObject.CurrentLocation.Y);

                                            PathFinder.bSearching = false;
                                        }
                                        catch
                                        {
                                            PathFinder.bSearching = false;
                                        }
                                    });
                                }
                            }
                        }
                    }

                }
                #endregion

                //普通攻击 如果释放了离魂邪风不能攻击
                if (!haselementalhurricane && Functions.Distance(MapObject.TargetObject.CurrentLocation, MapObject.User.CurrentLocation) == 1 && CEnvir.Now > User.AttackTime && User.Horse == HorseType.None)
                {
                    MapObject.User.AttemptAction(new ObjectAction(
                        MirAction.Attack,
                        Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation),
                        MapObject.User.CurrentLocation,
                        0,               //远程攻击目标ID
                        MagicType.None,
                        Element.None));
                    return;
                }
            }
            else
            {
                #region 挂机自动寻路
                if (BigPatchConfig.AndroidPlayer)//如果挂机且周边没有怪物 则重新自动寻路
                    if (pathfindertime < CEnvir.Now && !GameScene.Game.MapControl.AutoPath)
                    {
                        //GameScene.Game.MapControl.PathFinder = new PathFinder(GameScene.Game.MapControl);
                        //TODO Teleport Ring
                        int x, y;
                        if (BigPatchConfig.AndroidLockRange)  //如果大补帖有限制范围
                        {
                            x = CEnvir.Random.Next((int)(BigPatchConfig.AndroidCoord.X - BigPatchConfig.AndroidCoordRange), (int)(BigPatchConfig.AndroidCoord.X + BigPatchConfig.AndroidCoordRange));
                            y = CEnvir.Random.Next((int)(BigPatchConfig.AndroidCoord.Y - BigPatchConfig.AndroidCoordRange), (int)(BigPatchConfig.AndroidCoord.Y + BigPatchConfig.AndroidCoordRange));

                        }
                        else
                        {
                            //最好限制一个范围，否则随机到太远寻路卡机
                            x = CEnvir.Random.Next(MapObject.User.CurrentLocation.X - 200, MapObject.User.CurrentLocation.X + 200);
                            y = CEnvir.Random.Next(MapObject.User.CurrentLocation.Y - 200, MapObject.User.CurrentLocation.Y + 200);
                        }
                        //防止地图越界
                        if (x <= 0) x = 10;
                        else if (x >= Width) x = Width - 10;
                        if (y <= 0) y = 10;
                        else if (y >= Height) y = Height - 10;
                        if (Cells[x, y].Flag) return;
                        //开始寻路， 启用原子操作，开启异步
                        if (!PathFinder.bSearching)
                        {
                            PathFinder.bSearching = true;
                            Task.Run(() =>
                            {
                                try
                                {
                                    InitCurrentPath(x, y);

                                    PathFinder.bSearching = false;
                                }
                                catch
                                {
                                    PathFinder.bSearching = false;
                                }
                            });
                        }
                    }
                #endregion
            }

            MirDirection direction = MouseDirection(), best;
            if (MapObject.MagicObject != null/* && !MapObject.MagicObject.Dead*/)
                direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.MagicObject.CurrentLocation);

            if (GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip && !haselementalhurricane)
            {
#if Mobile
                ProcessStick();
#endif

                //地图双击触摸跑
                if (TouchRuning)
                {
                    ProcessTouchRuning();
                    return;
                }

                //跑到距离目标8格 开始持续施法
                if (GameScene.Game.AutoAttack && MapObject.MagicObject != null && !MapObject.MagicObject.Dead && Functions.Distance(MapObject.User.CurrentLocation, MapObject.MagicObject.CurrentLocation) > 8)
                {
                    Run(direction);
                    return;
                }

                //自动躲避
                if (GameScene.Game.AutoAttack && !GameScene.Game.MagicBarBox.PhysicalAttack && MapObject.MagicObject != null && !MapObject.MagicObject.Dead && Functions.Distance(MapObject.User.CurrentLocation, MapObject.MagicObject.CurrentLocation) < 3)
                {
                    //目标方向
                    MirDirection dir = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.MagicObject.CurrentLocation);
                    //反方向
                    dir = Functions.ShiftDirection(dir, 4);
                    //如果反方向不能移动
                    if (!CanMove(dir, 1))
                    {
                        //取个最佳方向
                        MirDirection bestdir = DirectionBest(dir, 1, MapObject.MagicObject.CurrentLocation);

                        //如果最佳方向不能移动，就循环周围格子能否移动
                        if (!CanMove(bestdir, 1))
                        {
                            for (int x = -1; x <= 1; x++)
                            {
                                for (int y = -1; y <= 1; y++)
                                {
                                    if (x == 0 && y == 0) continue;

                                    Point emp = new Point(User.CurrentLocation.X + x, User.CurrentLocation.Y + y);
                                    if (EmptyCell(emp))
                                    {
                                        bestdir = Functions.DirectionFromPoint(User.CurrentLocation, emp);
                                        break;
                                    }
                                }
                            }
                        }

                        dir = bestdir;
                    }

                    if (CanMove(dir, 1) && !haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                    {
                        //MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, dir, Functions.Move(MapObject.User.CurrentLocation, dir), 1, MagicType.None));
                        Run(dir);
                        return;
                    }
                }

                //跑不停
                if (GameScene.Game.AutoRun || BigPatchConfig.ChkKeepRunning)
                {
                    Run(direction);
                    return;
                }

                //挖肉
                if (Harvest)
                {
                    if (MapObject.MouseObject != null && MapObject.MouseObject.Dead)
                    {
                        direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.MouseObject.CurrentLocation);
                        if (Functions.Distance(MapObject.User.CurrentLocation, MapObject.MouseObject.CurrentLocation) > 1)
                        {
                            if (Functions.Distance(MapObject.User.CurrentLocation, MapObject.MouseObject.CurrentLocation) == 2)
                                MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
                            else
                                Run(direction);
                        }
                        else
                        {
                            if (User.Horse == HorseType.None)
                                MapObject.User.AttemptAction(new ObjectAction(
                                MirAction.Harvest,
                                direction,
                                MapObject.User.CurrentLocation));
                        }
                        return;
                    }
                    else
                        Harvest = false;
                }
            }

            //鼠标事件
#if Mobile
#else
            if (MouseControl == this)
#endif
            {
                switch (MapButtons)
                {
                    case MouseButtons.Left:   //鼠标左键按键事件
                        Mining = false;
                        Harvest = false;

                        if (MapObject.TargetObject == null && CEnvir.Shift)
                        {
                            if (CEnvir.Now > User.AttackTime && User.Horse == HorseType.None && !haselementalhurricane)
                                MapObject.User.AttemptAction(new ObjectAction(
                                    MirAction.Attack,
                                    direction,
                                    MapObject.User.CurrentLocation,
                                    0, //远程攻击目标ID
                                    MagicType.None,
                                    Element.None));
                            return;
                        }

                        if (CEnvir.Alt)  //按下Alt  挖肉
                        {
                            if (User.Horse == HorseType.None && !haselementalhurricane)
                                MapObject.User.AttemptAction(new ObjectAction(
                                MirAction.Harvest,
                                direction,
                                MapObject.User.CurrentLocation));
                            return;
                        }

                        if (MapLocation == MapObject.User.CurrentLocation)
                        {
                            if (GameScene.Game?.BigPatchBox?.PickupItems() ?? false) return;
                        }

                        if (MapObject.MouseObject != null && MapObject.MouseObject.Race != ObjectType.Item && !MapObject.MouseObject.Dead) break;

                        ClientUserItem weap = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

                        if (!haselementalhurricane && MapInfo.CanMine && weap != null && weap.Info.Effect == ItemEffect.PickAxe)
                        {
                            MiningPoint = Functions.Move(User.CurrentLocation, direction);

                            if (MiningPoint.X >= 0 && MiningPoint.Y >= 0 && MiningPoint.X < Width && MiningPoint.Y < Height && Cells[MiningPoint.X, MiningPoint.Y].Flag)
                            {
                                Mining = true;
                                break;
                            }
                        }

                        if (!CanMove(direction, 1) || haselementalhurricane)  //可以移动 方向
                        {
                            best = MouseDirectionBest(direction, 1);  //最佳鼠标方向

                            if (best == direction)   //方向
                            {
                                if (direction != User.Direction)
                                    if (haselementalhurricane)
                                    {
                                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));  //尝试操作（新的对象操作（站立 当前位置））
                                    }
                                    else
                                    {
                                        Run(direction);  //直接 跑
                                    }
                                return;
                            }

                            direction = best;  //方向=最佳鼠标方向
                        }
                        if (!haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                            MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
                        return;
                    case MouseButtons.Right://鼠标右键按键事件
                        Mining = false;
                        Harvest = false;
                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != MapObject.User && CEnvir.Ctrl) break;

                        if (!GameScene.Game.MoveFrame || (User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) break;

                        if (Functions.InRange(MapLocation, MapObject.User.CurrentLocation, 1) || haselementalhurricane)// 1 为右键点击坐标范围 原始为2
                        {
                            if (direction != User.Direction)
                                MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                            return;
                        }

                        Run(direction);

                        return;
                }
            }

            if (Mining)  //挖矿
            {
                ClientUserItem weap = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

                if (MapInfo.CanMine && weap != null && (weap.CurrentDurability > 0 || weap.Info.Durability == 0) && weap.Info.Effect == ItemEffect.PickAxe &&
                    MiningPoint.X >= 0 && MiningPoint.Y >= 0 && MiningPoint.X < Width && MiningPoint.Y < Height && Cells[MiningPoint.X, MiningPoint.Y].Flag &&
                    Functions.Distance(MiningPoint, MapObject.User.CurrentLocation) == 1 && User.Horse == HorseType.None)
                {
                    if (CEnvir.Now > User.AttackTime)
                        MapObject.User.AttemptAction(new ObjectAction(
                            MirAction.Mining,
                            Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MiningPoint),
                            MapObject.User.CurrentLocation,
                            false));
                }
                else
                {
                    Mining = false;
                }
            }

            if (AutoPath)
            {
                if (CurrentPath == null || CurrentPath.Count == 0)
                {
                    if (CurrentPath?.Count == 0 && !BigPatchConfig.AndroidPlayer)  //挂机不显示消息
                        GameScene.Game.ReceiveChat("BigMap.PathfindingOff".Lang(), MessageType.Hint);
                    AutoPath = false;
                    return;
                }
                if (!GameScene.Game.MoveFrame || (User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip || haselementalhurricane) return;
                Node currentNode = CurrentPath.SingleOrDefault(x => User.CurrentLocation == x.Location);
                if (currentNode != null)
                {
                    while (true)
                    {
                        Node first = CurrentPath.First();
                        CurrentPath.Remove(first);

                        if (first == currentNode)
                            break;
                    }
                }

                if (CurrentPath.Count > 0)
                {

                    MirDirection dir = Functions.DirectionFromPoint(User.CurrentLocation, CurrentPath.First().Location);
                    if (!CanMove(dir, 1))
                    {
                        //如果节点不能移动，重新开始寻路， 启用原子操作，开启异步
                        if (!PathFinder.bSearching)
                        {
                            PathFinder.bSearching = true;
                            Task.Run(() =>
                            {
                                try
                                {
                                    InitCurrentPath(CurrentPath.Last().Location.X, CurrentPath.Last().Location.Y);
                                    PathFinder.bSearching = false;
                                }
                                catch
                                {
                                    PathFinder.bSearching = false;
                                }
                            });
                        }

                        return;
                    }
                    int steps = 1;
                    if (GameScene.Game.CanRun && CEnvir.Now >= User.NextRunTime && User.BagWeight <= User.Stats[Stat.BagWeight] && User.WearWeight <= User.Stats[Stat.WearWeight] && User.HandWeight <= User.Stats[Stat.HandWeight])
                    {
                        steps++;
                        if (User.Horse != HorseType.None)
                            steps++;
                    }
                    Node upcomingStep = null;
                    for (int i = steps; i > 0; i--)
                    {
                        if (!CanMove(dir, i)) continue;
                        upcomingStep = CurrentPath.SingleOrDefault(x => Functions.Move(User.CurrentLocation, dir, i) == x.Location);
                        if (upcomingStep == null) continue;
                        steps = i;
                        break;
                    }
                    if (upcomingStep != null)
                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, dir, Functions.Move(MapObject.User.CurrentLocation, dir, steps), steps, MagicType.None));
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if (User.Horse != HorseType.None) return;
            if ((MapObject.TargetObject.Race == ObjectType.Player || !string.IsNullOrEmpty(MapObject.TargetObject.PetOwner)) && !(BigPatchConfig.ChkAvertShift || CEnvir.Shift)) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, MapObject.User.CurrentLocation, 1)) return;

            direction = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanMove(direction, 1) || haselementalhurricane)
            {
                best = DirectionBest(direction, 1, MapObject.TargetObject.CurrentLocation);

                if (best == direction)
                {
                    if (direction != User.Direction)
                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                    return;
                }
                direction = best;
            }

            //如果没有开启邪风并且画面移动并且未石化，就向服务器发包：尝试动作，移动一格
            if (!haselementalhurricane && GameScene.Game.MoveFrame && (User.Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip)
                if (haselementalhurricane)
                {
                    MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
                }
                else
                {
                    Run(direction);   //直接 跑
                }
        }
        /// <summary>
        /// 初始化当前路径
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        public void InitCurrentPath(int start, int target)
        {
            CurrentPath = PathFinder.FindPath(MapObject.User.CurrentLocation, new Point(start, target));
            if (CurrentPath == null || CurrentPath.Count == 0) return;
            AutoPath = true;
        }
        /// <summary>
        /// 跑
        /// </summary>
        /// <param name="direction"></param>
        public void Run(MirDirection direction)
        {
            int steps = 1;
            if (!GameScene.Game.CanPush && CEnvir.Now >= User.NextRunTime && (((CEnvir.ClientControl.OnRunCheck && BigPatchConfig.ChkAvertVerb) || GameScene.Game.CanRun) && User.BagWeight <= User.Stats[Stat.BagWeight] && User.WearWeight <= User.Stats[Stat.WearWeight] && User.HandWeight <= User.Stats[Stat.HandWeight]))
            {
                steps++;
                if (User.Horse != HorseType.None)
                    steps++;
            }

            for (int i = 1; i <= steps; i++)
            {
                if (CanMove(direction, i)) continue;

                MirDirection best = MouseDirectionBest(direction, 1);

                if (best == direction)
                {
                    if (i == 1)
                    {
                        if (direction != User.Direction)
                            MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                        return;
                    }

                    steps = i - 1;
                }
                else
                {
                    steps = 1;
                }
                direction = best;
                break;
            }

            MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction, steps), steps, MagicType.None));
        }
        /// <summary>
        /// 鼠标最佳方向
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public MirDirection MouseDirectionBest(MirDirection dir, int distance)
        {
            Point loc = Functions.Move(MapObject.User.CurrentLocation, dir, distance);

            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height && !Cells[loc.X, loc.Y].Blocking()) return dir;

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            double ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;

            MirDirection best = (MirDirection)(angle / 45F);

            if (best == dir)
                best = Functions.ShiftDirection(dir, 1);

            MirDirection next = Functions.ShiftDirection(dir, -((int)best - (int)dir));

            if (CanMove(best, distance))
                return best;

            if (CanMove(next, distance))
                return next;

            return dir;
        }
        /// <summary>
        /// 最佳方向
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="distance"></param>
        /// <param name="targetLocation"></param>
        /// <returns></returns>
        public MirDirection DirectionBest(MirDirection dir, int distance, Point targetLocation)
        {
            Point loc = Functions.Move(MapObject.User.CurrentLocation, dir, distance);

            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height && !Cells[loc.X, loc.Y].Blocking()) return dir;

            PointF c = new PointF(MapObject.OffSetX * MapObject.CellWidth + MapObject.CellWidth / 2F, MapObject.OffSetY * MapObject.CellHeight + MapObject.CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF((targetLocation.X - MapObject.User.CurrentLocation.X + MapObject.OffSetX) * MapObject.CellWidth + MapObject.CellWidth / 2F,
                (targetLocation.Y - MapObject.User.CurrentLocation.Y + MapObject.OffSetY) * MapObject.CellHeight + MapObject.CellHeight / 2F);
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            double ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (b.X < c.X) angle = 360 - angle;

            MirDirection best = (MirDirection)(angle / 45F);

            if (best == dir)
                best = Functions.ShiftDirection(dir, 1);

            MirDirection next = Functions.ShiftDirection(dir, -((int)best - (int)dir));

            if (CanMove(best, distance))
                return best;

            return CanMove(next, distance) ? next : dir;
        }
        /// <summary>
        /// 可以移动
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private bool CanMove(MirDirection dir, int distance)
        {
            for (int i = 1; i <= distance; i++)
            {
                Point loc = Functions.Move(User.CurrentLocation, dir, i);

                if (loc.X < 0 || loc.Y < 0 || loc.X >= Width || loc.Y > Height) return false;

                if (Cells[loc.X, loc.Y].Blocking())
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 空单元格
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool EmptyCell(Point loc)
        {
            if (loc.X < 0 || loc.Y < 0 || loc.X >= Width || loc.Y > Height) return false;
            if (Cells[loc.X, loc.Y].Blocking())
                return false;
            return true;
        }
        /// <summary>
        /// 鼠标定向
        /// </summary>
        /// <returns></returns>
        public MirDirection MouseDirection()
        {
            PointF p = new PointF(MouseLocation.X / CellWidth, MouseLocation.Y / CellHeight);

            //If close proximity then co by co ords 
            if (Functions.InRange(new Point(OffSetX, OffSetY), Point.Truncate(p), 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), Point.Truncate(p));

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(MouseLocation.X, MouseLocation.Y);
            float bc = (float)Functions.Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Functions.Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += 22.5F;
            if (angle > 360) angle -= 360;

            return (MirDirection)(angle / 45F);
        }
        /// <summary>
        /// 增加对象
        /// </summary>
        /// <param name="ob"></param>
        public void AddObject(MapObject ob)
        {
            Objects.Add(ob);

            if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].AddObject(ob);
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="ob"></param>
        public void RemoveObject(MapObject ob)
        {
            ob.OnRemoved();
            Objects.Remove(ob);

            if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].RemoveObject(ob);
        }
        public void SortObject(MapObject ob)
        {
            if (ob.CurrentLocation.X < Width && ob.CurrentLocation.Y < Height)
                Cells[ob.CurrentLocation.X, ob.CurrentLocation.Y].Sort();
        }
        /// <summary>
        /// 可以攻击
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public bool CanAttack(MapObject ob)
        {
            if (ob == null || ob == User) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.MonsterInfo.AI < 0) return false;

                    break;
                default:
                    return false;
            }

            return !ob.Dead;
        }
        /// <summary>
        /// 更新地图坐标
        /// </summary>
        public void UpdateMapLocation()
        {
            if (User == null) return;

            MapLocation = new Point((MouseLocation.X - GameScene.Game.Location.X) / CellWidth - OffSetX + User.CurrentLocation.X,
                                    (MouseLocation.Y - GameScene.Game.Location.Y) / CellHeight - OffSetY + User.CurrentLocation.Y);
        }
        /// <summary>
        /// 锁定目标
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public bool HasTarget(Point loc)
        {
            if (loc.X < 0 || loc.Y < 0 || loc.X >= Width || loc.Y > Height) return false;

            Cell cell = Cells[loc.X, loc.Y];

            if (cell.Objects == null) return false;

            foreach (MapObject ob in cell.Objects)
                if (ob.Blocking) return true;

            return false;
        }
        /// <summary>
        /// 可以刺杀
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanEnergyBlast(MirDirection direction)
        {
            return HasTarget(Functions.Move(MapObject.User.CurrentLocation, direction, 2));
        }
        /// <summary>
        /// 可以半月
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanHalfMoon(MirDirection direction)
        {
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, -1)))) return true;
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, 1)))) return true;
            if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, 2)))) return true;

            return false;
        }
        /// <summary>
        /// 可以十方斩
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool CanDestructiveBlow(MirDirection direction)
        {
            for (int i = 1; i < 8; i++)
                if (HasTarget(Functions.Move(MapObject.User.CurrentLocation, Functions.ShiftDirection(direction, i)))) return true;

            return false;
        }

        #endregion

        #region 手游
        public float Stick_X, Stick_Y;
        private bool TouchRuning;
        private Point TouchRuningTargetLocation;

        public void SetStick(float x, float y)
        {
            Stick_X = x;
            Stick_Y = y;
        }

        public void ProcessStick()
        {
            if (GameScene.Game.Observer) return;
            if (Stick_X == 0F && Stick_Y == 0F) return;

            if (Harvest)
                Harvest = false;
            if (Mining)
                Mining = false;
            if (TouchRuning)
                TouchRuning = false;
            if (AutoPath)
                AutoPath = false;

            GameScene.Game.TargetObject = null;

            bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);
            MirDirection direction = StickDirection(Stick_X, Stick_Y);

            if (!CanMove(direction, 1) || haselementalhurricane)
            {
                if (direction != User.Direction)
                    MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                return;
            }

            StickRun(direction);
        }

        public MirDirection StickDirection(float Stick_X, float Stick_Y)
        {
            double x = Math.Sqrt(Stick_X * Stick_X + Stick_Y * Stick_Y);
            double angle = Math.Acos((double)Stick_X / x);

            angle *= 180.0 / Math.PI;

            if (Stick_Y > 0f) angle = 360.0 - angle;
            angle = 450.0 - angle;
            angle += 22.5;
            if (angle > 360.0) angle -= 360.0;

            return (MirDirection)(angle / 45.0);
        }

        public void StickRun(MirDirection direction)
        {
            int steps = 1;
            if (!GameScene.Game.CanPush && CEnvir.Now >= User.NextRunTime && (((CEnvir.ClientControl.OnRunCheck && BigPatchConfig.ChkAvertVerb) || GameScene.Game.CanRun) && User.BagWeight <= User.Stats[Stat.BagWeight] && User.WearWeight <= User.Stats[Stat.WearWeight] && User.HandWeight <= User.Stats[Stat.HandWeight]))
            {
                if (GameScene.Game.StickMode == StickMode.Run)
                    steps++;
                if (User.Horse != HorseType.None)
                    steps++;
            }

            for (int i = 1; i <= steps; i++)
            {
                if (CanMove(direction, i)) continue;

                MirDirection best = StickDirectionBest(direction, 1);

                if (best == direction)
                {
                    if (i == 1)
                    {
                        if (direction != User.Direction)
                            MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                        return;
                    }

                    steps = i - 1;
                }
                else
                {
                    steps = 1;
                }
                direction = best;
                break;
            }

            MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction, steps), steps, MagicType.None));
        }

        public MirDirection StickDirectionBest(MirDirection dir, int distance)
        {
            Point loc = Functions.Move(MapObject.User.CurrentLocation, dir, distance);

            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Width && loc.Y < Height && !Cells[loc.X, loc.Y].Blocking()) return dir;

            MirDirection best = StickDirection(Stick_X, Stick_Y);
            if (best == dir)
                best = Functions.ShiftDirection(dir, 1);

            MirDirection next = Functions.ShiftDirection(dir, -(best - dir));
            if (CanMove(best, distance))
            {
                return best;
            }
            if (CanMove(next, distance))
            {
                return next;
            }
            return dir;
        }

        private void ProcessTouchRuning()
        {
            if (!TouchRuning) return;

            if (MapObject.User.CurrentLocation == TouchRuningTargetLocation)
            {
                TouchRuning = false;
                return;
            }

            MirDirection direction = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, TouchRuningTargetLocation);
            if (!CanMove(direction, 1))
            {
                MirDirection best = DirectionBest(direction, 1, TouchRuningTargetLocation);
                if (best == direction)
                {
                    if (direction != User.Direction)
                    {
                        MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                    }
                    return;
                }
                direction = best;
            }
            if (Functions.Distance(User.CurrentLocation, TouchRuningTargetLocation) == 1)
            {
                MapObject.User.AttemptAction(new ObjectAction(MirAction.Moving, direction, Functions.Move(MapObject.User.CurrentLocation, direction), 1, MagicType.None));
            }
            else
            {
                Run(direction);
            }
        }

        public override void OnFreeDrag(TouchEventArgs e)
        {
            base.OnFreeDrag(e);
            if (GameScene.Game.Observer) return;

            DXItemCell cell = DXItemCell.SelectedCell;
            if (cell != null)
                DXItemCell.SelectedCell = null;
        }
        public override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);
            if (GameScene.Game.Observer) return;

            DXItemCell cell = DXItemCell.SelectedCell;
            if (cell != null)
            {
                if (cell.GridType == GridType.Belt)
                {
                    cell.QuickInfo = null;
                    cell.QuickItem = null;
                    DXItemCell.SelectedCell = null;

                    ClientBeltLink link = GameScene.Game.BeltBox.Links[cell.Slot];
                    CEnvir.Enqueue(new C.BeltLinkChanged { Slot = link.Slot, LinkIndex = link.LinkInfoIndex, LinkItemIndex = link.LinkItemIndex }); //Update serve
                    return;
                }

                if ((cell.Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (cell.GridType != GridType.PatchGrid && cell.GridType != GridType.Inventory && cell.GridType != GridType.CompanionInventory))
                {
                    DXItemCell.SelectedCell = null;
                    return;
                }

                if ((cell.Item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked || (cell.GridType != GridType.PatchGrid && cell.GridType != GridType.Inventory && cell.GridType != GridType.CompanionInventory))
                {
                    DXItemCell.SelectedCell = null;
                    return;
                }

                if (cell.Item.Count > 1)
                {
                    DXItemAmountWindow window = new DXItemAmountWindow("丢弃道具".Lang(), cell.Item);

                    window.ConfirmButton.MouseClick += (o, a) =>  //固定按钮  鼠标单击
                    {
                        if (window.Amount <= 0) return;  //如果数量小于或者等于0 返回

                        CEnvir.Enqueue(new C.ItemDrop
                        {
                            Link = new CellLinkInfo { GridType = cell.GridType, Slot = cell.Slot, Count = window.Amount }
                        });

                        cell.Locked = true;
                    };
                }
                else
                {
                    CEnvir.Enqueue(new C.ItemDrop
                    {
                        Link = new CellLinkInfo { GridType = cell.GridType, Slot = cell.Slot, Count = 1 }
                    });

                    cell.Locked = true;
                }

                DXItemCell.SelectedCell = null;
                return;
            }

            if (GameScene.Game.GoldPickedUp)
            {
                DXItemAmountWindow window = new DXItemAmountWindow("掉落道具".Lang(), new ClientUserItem(Globals.GoldInfo, User.Gold));

                window.ConfirmButton.MouseClick += (o, a) =>
                {
                    if (window.Amount <= 0) return;

                    CEnvir.Enqueue(new C.GoldDrop
                    {
                        Amount = window.Amount
                    });

                };

                GameScene.Game.GoldPickedUp = false;
                return;
            }

            #region 点击地图关闭打开的窗口
            if (GameScene.Game.SelectedCell == null)
            {
                if (GameScene.Game.UserFunctionControl.Visible == true)
                {
                    GameScene.Game.UserFunctionControl.Visible = false;
                }
                if (GameScene.Game.ChatBox.Visible == true)
                {
                    GameScene.Game.ChatBox.Visible = false;
                }
                if (GameScene.Game.CharacterBox.Visible == true)
                {
                    GameScene.Game.CharacterBox.Visible = false;
                }
                if (GameScene.Game.InventoryBox.Visible == true)
                {
                    GameScene.Game.InventoryBox.Visible = false;
                }
                if (GameScene.Game.StorageBox.Visible == true)
                {
                    GameScene.Game.StorageBox.Visible = false;
                }
                if (GameScene.Game.AuctionsBox.Visible == true)
                {
                    GameScene.Game.AuctionsBox.Visible = false;
                }
                if (GameScene.Game.BonusPoolVersionBox.Visible == true)
                {
                    GameScene.Game.BonusPoolVersionBox.Visible = false;
                }
                if (GameScene.Game.NPCBox.Visible == true)
                {
                    GameScene.Game.NPCBox.Visible = false;
                }
                if (GameScene.Game.BigMapBox.Visible == true)
                {
                    GameScene.Game.BigMapBox.Visible = false;
                }
            }
            #endregion

            MapButtons = MouseButtons.None;
            GameScene.Game.FocusObject = null;
        }
        /// <summary>
        /// 触摸单击时 等同OnMouseClick
        /// </summary>
        /// <param name="e"></param>
        public override void OnTap(TouchEventArgs e)
        {
            base.OnTap(e);
            if (GameScene.Game.Observer) return;

            if (Harvest)
                Harvest = false;
            if (Mining)
                Mining = false;
            if (TouchRuning)
                TouchRuning = false;
            if (AutoPath)
                AutoPath = false;

            GameScene.Game.ContinuouslyMagic = false;
            MapButtons = MouseButtons.None;
            MouseLocation = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            CheckCursor();

            #region 点击地图特定坐标出现功能

            if (!CEnvir.ClientControl.CoinPlaceChoiceCheck)
            {
                //如果当前地图调用的图片是1 道馆
                if (MapInfo.FileName.ToLower() == "1")
                {
                    Point mousePoint = ConvertMouseToGame(MouseLocation);
                    Point targetPoint1 = new Point(x: 389, y: 133);
                    Point targetPoint2 = new Point(x: 397, y: 139);

                    //指针坐标与目标的坐标 误差距离小于等于1
                    if (Functions.InRange(mousePoint, targetPoint1, 1) || Functions.InRange(mousePoint, targetPoint2, 1))
                    {
                        GameScene.Game.VowBox.Visible = !GameScene.Game.VowBox.Visible;
                        return;
                    }
                }
            }
            else
            {
                //如果当前地图调用的图片是15 泰山
                if (MapInfo.FileName.ToLower() == "15")
                {
                    Point mousePoint = ConvertMouseToGame(MouseLocation);
                    Point targetPoint1 = new Point(x: 676, y: 137);
                    Point targetPoint2 = new Point(x: 677, y: 138);
                    Point targetPoint3 = new Point(x: 678, y: 139);
                    Point targetPoint4 = new Point(x: 679, y: 139);

                    //指针坐标与目标的坐标 误差距离小于等于1
                    if (Functions.InRange(mousePoint, targetPoint1, 1) || Functions.InRange(mousePoint, targetPoint2, 1) || Functions.InRange(mousePoint, targetPoint3, 1) || Functions.InRange(mousePoint, targetPoint4, 1))
                    {
                        GameScene.Game.VowBox.Visible = !GameScene.Game.VowBox.Visible;
                        return;
                    }
                }
            }
            #endregion

            //钓鱼判断
            if (GameScene.Game.User.HasFishingRod && MapObject.User.HasFishingArmour && MapObject.User.CurrentAction == MirAction.Standing)
            {
                Point mousePoint = ConvertMouseToGame(MouseLocation);
                if (mousePoint.X >= Width || mousePoint.Y >= Height) return;

                FishingCellPoint = Functions.Move(User.CurrentLocation, User.Direction, 2);
                if (FishingCellPoint.X >= Width || FishingCellPoint.Y >= Height) return;

                Cell fishingCell = Cells[FishingCellPoint.X, FishingCellPoint.Y];
                if (fishingCell.FishingCell)
                {
                    //抛竿
                    MirDirection fishingDirection =
                        Functions.DirectionFromPoint(User.CurrentLocation, mousePoint);

                    //MapObject.User.ActionQueue.Add(new ObjectAction(MirAction.FishingCast, fishingDirection, MapObject.User.CurrentLocation));

                    //告知服务端 开始钓鱼
                    CEnvir.Enqueue(new C.FishingCast());
                    return;
                }
                else
                {
                    if (MapObject.MouseObject == null)
                        GameScene.Game.ReceiveChat("这里不能钓鱼".Lang(), MessageType.Hint);
                }
            }

            if (MapObject.MouseObject == null)
            {
                if ((User.Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) return;
                bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);

                ClientUserItem weap = GameScene.Game.Equipment[(int)EquipmentSlot.Weapon];

                //挖矿
                if (!haselementalhurricane && MapInfo.CanMine && weap != null && weap.Info.Effect == ItemEffect.PickAxe)
                {
                    MiningPoint = Functions.Move(User.CurrentLocation, MouseDirection());

                    if (MiningPoint.X >= 0 && MiningPoint.Y >= 0 && MiningPoint.X < Width && MiningPoint.Y < Height && Cells[MiningPoint.X, MiningPoint.Y].Flag)
                    {
                        Mining = true;
                        return;
                    }
                }
            }

            if (MapObject.MouseObject is NPCObject npc)
            {
                if (CEnvir.Now <= GameScene.Game.NPCTime) return;

                GameScene.Game.NPCTime = CEnvir.Now.AddSeconds(1);

                CEnvir.Enqueue(new C.NPCCall { ObjectID = npc.ObjectID, Key = @"[@MAIN]" });
                return;
            }

            if (CanAttack(MapObject.MouseObject))
            {
                MapObject.MagicObject = MapObject.MouseObject;

                //单机屏幕 有对象人物就调整相应方向
                MirDirection direction = Functions.DirectionFromPoint(MapObject.User.CurrentLocation, MapObject.MouseObject.CurrentLocation);
                if (MapObject.User.CurrentAction == MirAction.Standing && direction != User.Direction)
                {
                    MapObject.User.AttemptAction(new ObjectAction(MirAction.Standing, direction, MapObject.User.CurrentLocation));
                }

                if (MapObject.MouseObject.Race == ObjectType.Monster && ((MonsterObject)MapObject.MouseObject).MonsterInfo.AI >= -1) //Check if AI is guard
                {
                    GameScene.Game.FocusObject = MapObject.MouseObject;
                }
            }

            //点击地图优先释放锁定魔法
            if (GameScene.Game.LockMagicType != MagicType.None)
            {
                var ob = GameScene.Game.MagicObject;
                GameScene.Game.MagicObject = null;
                GameScene.Game.UseMagic(GameScene.Game.LockMagicType);
                GameScene.Game.MagicObject = ob;
            }
        }
        /// <summary>
        /// 触摸双击时 等同OnMouseDoubleClick
        /// </summary>
        /// <param name="e"></param>
        public override void OnDoubleTap(TouchEventArgs e)
        {
            base.OnDoubleTap(e);
            if (GameScene.Game.Observer) return;

            if (Harvest)
                Harvest = false;
            if (Mining)
                Mining = false;
            if (TouchRuning)
                TouchRuning = false;
            if (AutoPath)
                AutoPath = false;
            if (GameScene.Game.AutoAttack)   //双击地面 自动打怪中断
                GameScene.Game.AutoAttack = false;
            MapButtons = MouseButtons.None;
            MouseLocation = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
            CheckCursor();

            if (CanAttack(MapObject.MouseObject))
            {
                MapObject.TargetObject = MapObject.MouseObject;

                if (MapObject.MouseObject.Race == ObjectType.Monster && ((MonsterObject)MapObject.MouseObject).MonsterInfo.AI >= -1) //Check if AI is guard
                {
                    MapObject.MagicObject = MapObject.TargetObject;
                    GameScene.Game.FocusObject = MapObject.MouseObject;
                }
                return;
            }

            if (MapObject.MagicObject == null)
            {
                TouchRuning = true;
                TouchRuningTargetLocation = MapLocation;

            }
            MapObject.TargetObject = null;
            GameScene.Game.FocusObject = null;
            MapObject.MagicObject = null;
            CEnvir.Shift = false;
            CEnvir.Alt = false;
        }
        /// <summary>
        /// 触摸移动时 等同OnMouseMove
        /// </summary>
        //public override void OnTouchMoved(TouchEventArgs e)
        //{
        //    base.OnTouchMoved(e);

        //    if (!GameScene.Game.Observer && FocusControl == this)
        //        MouseLocation = ZoomRate == 1F ? e.Location : new Point((int)Math.Round((e.Location.X - UI_Offset_X) / ZoomRate), (int)Math.Round(e.Location.Y / ZoomRate));
        //}
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _MapInfo = null;
                MapInfoChanged = null;

                _Animation = 0;
                AnimationChanged = null;

                MapButtons = 0;
                MapLocation = Point.Empty;
                Mining = false;
                Harvest = false;
                MiningPoint = Point.Empty;
                MiningDirection = 0;

                if (FLayer != null)
                {
                    if (!FLayer.IsDisposed)
                        FLayer.Dispose();

                    FLayer = null;
                }

                if (LLayer != null)
                {
                    if (!LLayer.IsDisposed)
                        LLayer.Dispose();

                    LLayer = null;
                }

                Cells = null;

                Width = 0;
                Height = 0;

                Objects.Clear();
                Objects = null;

                Effects.Clear();
                Effects = null;
                ViewRangeX = 0;
                ViewRangeY = 0;
                OffSetX = 0;
                OffSetY = 0;
            }
        }

        #endregion

        /// <summary>
        /// 地面物品控制
        /// </summary>
        public sealed class Floor : DXControl
        {
            private Point[,] Boat = new Point[10, 10];
            private DateTime MapBackGroundAnim;
            /// <summary>
            /// 是否船地图
            /// </summary>
            public bool IsBoat
            {
                get
                {
                    return _isBoat;
                }
                set
                {
                    _isBoat = value;
                }
            }
            private bool _isBoat;
            /// <summary>
            /// 地面物品控制
            /// </summary>
            public Floor()
            {
                IsControl = false;
                //船动画背景
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        int x = i * 256;
                        int y = j * 256;
                        Boat[i, j] = new Point(x, y);
                    }
                }
            }

            #region Methods
            /// <summary>
            /// 检查纹理
            /// </summary>
            public void CheckTexture()
            {
                if (!TextureValid || (IsBoat && MapBackGroundAnim < CEnvir.Now))
                    CreateTexture();
            }
            /// <summary>
            /// 清理纹理
            /// </summary>
            protected override void OnClearTexture()
            {
                base.OnClearTexture();

                if (GameScene.Game.MapControl.BackgroundImage != null)  //地图背景最底层
                {
                    float pixelspertileX = (GameScene.Game.MapControl.BackgroundImage.Width - Config.GameSize.Width) / GameScene.Game.MapControl.Width;
                    float pixelspertileY = (GameScene.Game.MapControl.BackgroundImage.Height - Config.GameSize.Height) / GameScene.Game.MapControl.Height;
                    int bgX = (int)(User.CurrentLocation.X * pixelspertileX) + GameScene.Game.MapControl.BackgroundMovingOffset.X;
                    int bgY = (int)(User.CurrentLocation.Y * pixelspertileY) + GameScene.Game.MapControl.BackgroundMovingOffset.Y;
                    Rectangle bgdisplay = new Rectangle(bgX, bgY, DisplayArea.Width, DisplayArea.Height);
                    MirLibrary bglibrary;
                    if (CEnvir.LibraryList.TryGetValue(LibraryFile.Background, out bglibrary))
                        bglibrary.Draw(GameScene.Game.MapControl.MapInfo.Background, 0, 0, Color.White, bgdisplay, 1F, ImageType.Image, zoomRate: 1F);
                }

                int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 4), maxX = Math.Min(GameScene.Game.MapControl.Width - 1, User.CurrentLocation.X + OffSetX + 4);
                int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 4), maxY = Math.Min(GameScene.Game.MapControl.Height - 1, User.CurrentLocation.Y + OffSetY + 4);

                for (int y = minY; y <= maxY; y++)
                {
                    //if (y < 0) continue;
                    //if (y >= GameScene.Game.MapControl.Height) break;
                    if (y % 2 != 0) continue;
                    int drawY = (y - User.CurrentLocation.Y + OffSetY) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;
                    for (int x = minX; x <= maxX; x++)
                    {
                        //if (x < 0) continue;
                        //if (x >= GameScene.Game.MapControl.Width) break;
                        if (x % 2 != 0) continue;
                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                        MirLibrary library;
                        LibraryFile file;
                        Cell tile = GameScene.Game.MapControl.Cells[x, y];
                        if (tile.BackImage != -1)
                        {
                            if (!Libraries.KROrder.TryGetValue(tile.BackFile, out file)) continue;
                            if (!CEnvir.LibraryList.TryGetValue(file, out library)) continue;

                            int backimage = (tile.BackImage & 0x1FFFF) - 1;
                            //绘制背景层，既地砖
                            library.Draw(backimage, drawX, drawY, Color.White, false, 1F, ImageType.Image, zoomRate: 1F);
                        }

                    }
                }
                //船动画背景
                if (IsBoat)
                {
                    bool flag = false;
                    for (int x = 0; x < 10; x++)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            if (MapBackGroundAnim < CEnvir.Now)
                            {
                                int drawX = x * 256;
                                int drawY = y * 256;
                                drawX -= GameScene.Game.MapControl.Animation % 256 * 3;
                                drawY -= GameScene.Game.MapControl.Animation % 256 * 3;
                                Boat[x, y] = new Point(drawX, drawY);
                                flag = true;
                            }

                            MirLibrary library;
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library)) continue;

                            library.Draw(561, Boat[x, y].X, Boat[x, y].Y, Color.White, false, 1f, ImageType.Image, zoomRate: 1F);
                        }
                    }
                    if (flag)
                    {
                        MapBackGroundAnim = CEnvir.Now.AddMilliseconds(1.0);
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    int drawY = (y - User.CurrentLocation.Y + OffSetY + 1) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;
                    for (int x = minX; x <= maxX; x++)
                    {
                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;
                        Cell cell = GameScene.Game.MapControl.Cells[x, y];
                        MirLibrary library;
                        LibraryFile file;
                        if (cell.MiddleFile != -1)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                            {
                                int index = cell.MiddleImage - 1;
                                if (index >= 0)
                                {
                                    Size s = library.GetSize(index);
                                    //绘制中间层标准（48 * 32 或96 * 64） 的物体，其它的非标物体由DrawObject()方法绘制
                                    if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                        library.Draw(index, drawX, drawY - CellHeight, Color.White, false, 1F, ImageType.Image, zoomRate: 1F);
                                }

                            }
                        }
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    int drawY = (y - User.CurrentLocation.Y + OffSetY + 1) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;
                    for (int x = minX; x <= maxX; x++)
                    {
                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;
                        Cell cell = GameScene.Game.MapControl.Cells[x, y];
                        MirLibrary library;
                        LibraryFile file;
                        if (cell.FrontFile != -1)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && CEnvir.LibraryList.TryGetValue(file, out library))
                            {
                                int index = (cell.FrontImage & 0x7FFF) - 1;
                                if (index >= 0)
                                {
                                    //有动画帧跳出
                                    if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                                        continue;
                                    Size s = library.GetSize(index);
                                    //绘制前景层标准（48 * 32 或96 * 64） 的物体，其它的非标物体由DrawObject()方法绘制
                                    if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                        library.Draw(index, drawX, drawY - CellHeight, Color.White, false, 1F, ImageType.Image, zoomRate: 1F);
                                }

                            }
                        }

                    }
                }
            }
            /// <summary>
            /// 绘制
            /// </summary>
            public override void Draw() { }
            /// <summary>
            /// 绘制控件
            /// </summary>
            protected override void DrawControl() { }

            #endregion
        }
        /// <summary>
        /// 光线控制
        /// </summary>
        public sealed class Light : DXControl
        {
            struct RGBINFO
            {
                public byte bRed;
                public byte bGreen;
                public byte bBlue;
            }
            private RGBINFO[] m_stRGBInfo = new RGBINFO[10];
            /// <summary>
            /// 光线控制
            /// </summary>
            public Light()
            {
                IsControl = false;
                BackColour = Color.FromArgb(15, 15, 15);
                m_stRGBInfo[0].bRed = 214; m_stRGBInfo[0].bGreen = 198; m_stRGBInfo[0].bBlue = 173;
                m_stRGBInfo[1].bRed = 165; m_stRGBInfo[1].bGreen = 140; m_stRGBInfo[1].bBlue = 90;
                m_stRGBInfo[2].bRed = 115; m_stRGBInfo[2].bGreen = 57; m_stRGBInfo[2].bBlue = 41;
                m_stRGBInfo[3].bRed = 99; m_stRGBInfo[3].bGreen = 198; m_stRGBInfo[3].bBlue = 222;
                m_stRGBInfo[4].bRed = 49; m_stRGBInfo[4].bGreen = 115; m_stRGBInfo[4].bBlue = 41;
                m_stRGBInfo[5].bRed = 255; m_stRGBInfo[5].bGreen = 255; m_stRGBInfo[5].bBlue = 255;
                m_stRGBInfo[6].bRed = 255; m_stRGBInfo[6].bGreen = 255; m_stRGBInfo[6].bBlue = 255;
                m_stRGBInfo[7].bRed = 255; m_stRGBInfo[7].bGreen = 255; m_stRGBInfo[7].bBlue = 255;
                m_stRGBInfo[8].bRed = 255; m_stRGBInfo[8].bGreen = 255; m_stRGBInfo[8].bBlue = 255;
                m_stRGBInfo[9].bRed = 255; m_stRGBInfo[9].bGreen = 255; m_stRGBInfo[9].bBlue = 255;
            }

            #region Methods
            /// <summary>
            /// 检查纹理
            /// </summary>
            public void CheckTexture()
            {
                CreateTexture();
            }
            /// <summary>
            /// 清理纹理
            /// </summary>
            protected override void OnClearTexture()
            {
                base.OnClearTexture();

                //死亡红屏
                if (MapObject.User.Dead)
                {
                    if (BigPatchConfig.ChkDeathRedScreen)
                    {
                        DXManager.Device.Clear(ClearFlags.Target, Color.IndianRed, 0, 0);
                    }
                    return;
                }

                DXManager.SetBlend(true);
                DXManager.Device.SetRenderState(DXManager.LightBlend);

                const float lightScale = 0.02F; //Players/Monsters  //光标
                const float baseSize = 0.1F;

                float fX;
                float fY;

                if ((MapObject.User.Poison & PoisonType.Abyss) == PoisonType.Abyss)
                {
                    DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);

                    float scale = baseSize + 4 * lightScale;

                    fX = (OffSetX + MapObject.User.CurrentLocation.X - User.CurrentLocation.X) * CellWidth + CellWidth / 2;
                    fY = (OffSetY + MapObject.User.CurrentLocation.Y - User.CurrentLocation.Y) * CellHeight;

                    fX -= (DXManager.LightWidth * scale) / 2;
                    fY -= (DXManager.LightHeight * scale) / 2;

                    //fX /= scale;
                    //fY /= scale;

                    //DXManager.Sprite.Transform = Matrix.CreateScale(scale, scale, 1);

                    DXManager.Sprite.DrawZoom(DXManager.LightTexture, new Vector2(fX, fY), Color.White, scale);

                    //DXManager.Sprite.Transform = Matrix.Identity;

                    DXManager.SetBlend(false);

                    MapObject.User.AbyssEffect.Draw();
                    return;
                }

                foreach (MapObject ob in GameScene.Game.MapControl.Objects)
                {
                    if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                    {
                        float scale = baseSize + ob.Light * 2 * lightScale;

                        fX = (OffSetX + ob.CurrentLocation.X - User.CurrentLocation.X) * CellWidth + ob.MovingOffSet.X - User.MovingOffSet.X + CellWidth / 2;
                        fY = (OffSetY + ob.CurrentLocation.Y - User.CurrentLocation.Y) * CellHeight + ob.MovingOffSet.Y - User.MovingOffSet.Y;

                        fX -= (DXManager.LightWidth * scale) / 2;
                        fY -= (DXManager.LightHeight * scale) / 2;

                        //fX /= scale;
                        //fY /= scale;

                        //DXManager.Sprite.Transform = Matrix.CreateScale(scale, scale, 1);

                        DXManager.Sprite.DrawZoom(DXManager.LightTexture, new Vector2(fX, fY), ob.LightColour, scale);

                        //DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }

                foreach (MirEffect ob in GameScene.Game.MapControl.Effects)
                {
                    float frameLight = ob.FrameLight;

                    if (frameLight > 0)
                    {
                        float scale = baseSize + frameLight * 2 * lightScale / 5;

                        fX = ob.DrawX + CellWidth / 2;
                        fY = ob.DrawY + CellHeight / 2;

                        fX -= (DXManager.LightWidth * scale) / 2;
                        fY -= (DXManager.LightHeight * scale) / 2;

                        //fX /= scale;
                        //fY /= scale;

                        //DXManager.Sprite.Transform = Matrix.CreateScale(scale, scale, 1);

                        DXManager.Sprite.DrawZoom(DXManager.LightTexture, new Vector2(fX, fY), ob.FrameLightColour, scale);

                        //DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }

                int minX = Math.Max(0, User.CurrentLocation.X - OffSetX - 15), maxX = Math.Min(GameScene.Game.MapControl.Width - 1, User.CurrentLocation.X + OffSetX + 15);
                int minY = Math.Max(0, User.CurrentLocation.Y - OffSetY - 15), maxY = Math.Min(GameScene.Game.MapControl.Height - 1, User.CurrentLocation.Y + OffSetY + 15);

                for (int y = minY; y <= maxY; y++)
                {
                    if (y < 0) continue;
                    if (y >= GameScene.Game.MapControl.Height) break;

                    int drawY = (y - User.CurrentLocation.Y + OffSetY) * CellHeight - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x < 0) continue;
                        if (x >= GameScene.Game.MapControl.Width) break;

                        int drawX = (x - User.CurrentLocation.X + OffSetX) * CellWidth - User.MovingOffSet.X - User.ShakeScreenOffset.X;

                        Cell tile = GameScene.Game.MapControl.Cells[x, y];

                        if (tile.Light == 0 && (tile.Light & 0x07) != 1) continue;
                        int LightColor = (tile.Light & 0x3ff0) >> 4;
                        int LightSize = (tile.Light & 0xc000) >> 14;
                        if (LightSize >= 4 || LightColor >= 10) continue;

                        float scale = baseSize + (tile.Light & 0x0f) * 2 * 30 * lightScale;

                        fX = drawX + CellWidth / 2;
                        fY = drawY + CellHeight / 2;

                        fX -= DXManager.LightWidth * scale / 2;
                        fY -= DXManager.LightHeight * scale / 2;

                        //fX /= scale;
                        //fY /= scale;

                        //DXManager.Sprite.Transform = Matrix.CreateScale(scale, scale, 1);

                        DXManager.Sprite.DrawZoom(DXManager.LightTexture, new Vector2(fX, fY), Color.FromArgb(m_stRGBInfo[LightColor].bRed, m_stRGBInfo[LightColor].bGreen, m_stRGBInfo[LightColor].bBlue), scale);

                        //DXManager.Sprite.Transform = Matrix.Identity;
                    }
                }
                DXManager.SetBlend(false);
            }
            /// <summary>
            /// 全亮
            /// </summary>
            private void AllLights()
            {
                BackColour = Color.White;  //背景色白色
                Visible = true;
            }
            /// <summary>
            /// 更新光线
            /// </summary>
            public void UpdateLights()
            {
                if (CEnvir.ClientControl.OnBrightBox && BigPatchConfig.ChkAvertBright)  //免蜡设置
                {
                    AllLights();  //打开所有光，地图全亮
                    return;
                }
                switch (GameScene.Game.MapControl.MapInfo.Light)  //切换 （服务端地图设置里的光效设置）
                {
                    case LightSetting.Default:  //默认设置
                        byte shading = (byte)(255 * GameScene.Game.DayTime);   //对应系统设置的游戏时间
                        BackColour = Color.FromArgb(shading, shading, shading);
                        Visible = true;
                        break;
                    case LightSetting.Night:   //夜晚设置
                        BackColour = Color.FromArgb(15, 15, 15);   //对应背景色 黑色
                        Visible = true;
                        break;
                    case LightSetting.Light:   //白天设置
                        Visible = MapObject.User != null && (MapObject.User.Poison & PoisonType.Abyss) != PoisonType.Abyss;
                        break;
                    case LightSetting.FullBright:   //地图全亮设置
                        BackColour = Color.White;  //背景色白色
                        Visible = true;
                        break;
                }
            }
            /// <summary>
            /// 绘制控件
            /// </summary>
            protected override void DrawControl() { }
            /// <summary>
            /// 绘制
            /// </summary>
            public override void Draw() { }

            #endregion
        }

        #region 粒子相关
        public CFlyingTail m_xFlyingTail = new CFlyingTail();
        public CSmoke m_xSmoke = new CSmoke();
        public CBoom m_xBoom = new CBoom();
        public CMist m_xMist = new CMist();
        public CSnow m_xSnow = new CSnow();
        public CScatter m_xScatter = new CScatter();
        public CRain m_xRain = new CRain();

        public bool m_bShowSnow;
        public bool m_bShowMist;
        public bool m_bShowRain;

        public DateTime ParticleRenderTime;
        #endregion
    }

    public sealed class MapCells
    {
        private int Version = 5;  //5是韩版传奇3
        private int HeaderOffset = 28;
        private int xor;

        private byte[] Data;
        private int cellRawDataSize = 14;

        public string FileName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }


        private Cell[,] Cells;
        public BitArray BlockFlag;

        public Cell this[int x, int y]
        {
            get
            {
                return GetCell(x, y);
            }
            set
            {
                Cells[x, y] = value;
            }
        }

        public MapCells(string fileName)
        {
            FileName = fileName;

            Data = File.ReadAllBytes(FileName);
            Load();
        }

        private void Load()
        {
            //(0-99) c#自定义地图格式 title:
            if (Data[2] == 0x43 && Data[3] == 0x23)
            {
                LoadMapType100();
                Version = 100;
                return;
            }
            //(200-299) 韩版传奇3 title:
            if (Data[0] == 0)
            {
                LoadMapType5();
                Version = 5;
                return;
            }
            //(300-399) 盛大传奇3 title: (C) SNDA, MIR3.
            if (Data[0] == 0x0F && Data[5] == 0x53 && Data[14] == 0x33)
            {
                LoadMapType6();
                Version = 6;
                return;
            }
            //(400-499) 应该是盛大传奇3第二种格式，未知？无参考
            if (Data[0] == 0x0F && Data[5] == 0x4D && Data[14] == 0x33)
            {
                LoadMapType8();
                Version = 8;
                return;
            }
            //(0-99) wemades antihack map (laby maps) title start with: Mir2 AntiHack
            if (Data[0] == 0x15 && Data[4] == 0x32 && Data[6] == 0x41 && Data[19] == 0x31)
            {
                LoadMapType4();
                Version = 4;
                return;
            }
            //(0-99) wemades 2010 map format i guess title starts with: Map 2010 Ver 1.0
            if (Data[0] == 0x10 && Data[2] == 0x61 && Data[7] == 0x31 && Data[14] == 0x31)
            {
                LoadMapType1();
                Version = 1;
                return;
            }
            //(100-199) shanda's 2012 format and one of shandas(wemades) older formats share same header info, only difference is the filesize
            if ((Data[4] == 0x0F || (Data[4] == 0x03)) && Data[18] == 0x0D && Data[19] == 0x0A)
            {
                int w = Data[0] + (Data[1] << 8);
                int h = Data[2] + (Data[3] << 8);
                if (Data.Length > 52 + (w * h * 14))
                {
                    LoadMapType3();
                    Version = 3;
                }
                else
                {
                    LoadMapType2();
                    Version = 2;
                }
                return;
            }
            //(0-99) 3/4 heroes map format (myth/lifcos i guess)
            if (Data[0] == 0x0D && Data[1] == 0x4C && Data[7] == 0x20 && Data[11] == 0x6D)
            {
                LoadMapType7();
                Version = 7;
                return;
            }
            //if it's none of the above load the default old school format
            LoadMapType0();
            Version = 0;
        }

        /// <summary>
        /// 加载地图类型0
        /// </summary>
        private void LoadMapType0()
        {
            Width = BitConverter.ToInt16(Data, 0);
            Height = BitConverter.ToInt16(Data, 2);
            HeaderOffset = 52;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 12;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            int BackImage = 0, FrontImage = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    BackImage = BitConverter.ToInt16(Data, offset);
                    FrontImage = BitConverter.ToInt16(Data, offset + 4);
                    bool flag = ((BackImage & 0x8000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型1
        /// </summary>
        private void LoadMapType1()
        {
            int w = BitConverter.ToInt16(Data, 21);
            xor = BitConverter.ToInt16(Data, 23);
            int h = BitConverter.ToInt16(Data, 25);
            Width = (w ^ xor);
            Height = (h ^ xor);
            HeaderOffset = 54;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 15;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = (int)(BitConverter.ToInt32(Data, offset) ^ 0xAA38AA38);
                    int FrontImage = (short)(BitConverter.ToInt16(Data, offset + 6) ^ xor);
                    bool flag = ((BackImage & 0x20000000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型2
        /// </summary>
        private void LoadMapType2()
        {
            Width = BitConverter.ToInt16(Data, 0);
            Height = BitConverter.ToInt16(Data, 2);
            HeaderOffset = 52;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 14;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = BitConverter.ToInt16(Data, offset);
                    int FrontImage = BitConverter.ToInt16(Data, offset + 4);
                    bool flag = ((BackImage & 0x8000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型3
        /// </summary>
        private void LoadMapType3()
        {
            Width = BitConverter.ToInt16(Data, 0);
            Height = BitConverter.ToInt16(Data, 2);
            HeaderOffset = 52;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 36;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = BitConverter.ToInt16(Data, offset);
                    int FrontImage = BitConverter.ToInt16(Data, offset + 4);
                    bool flag = ((BackImage & 0x8000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型4
        /// </summary>
        private void LoadMapType4()
        {
            int w = BitConverter.ToInt16(Data, 31);
            xor = BitConverter.ToInt16(Data, 33);
            int h = BitConverter.ToInt16(Data, 35);
            Width = (w ^ xor);
            Height = (h ^ xor);
            HeaderOffset = 64;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 12;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = BitConverter.ToInt16(Data, offset);
                    int FrontImage = BitConverter.ToInt16(Data, offset + 4);
                    bool flag = ((BackImage & 0x8000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型5
        /// </summary>
        private void LoadMapType5()
        {
            Width = (int)(BitConverter.ToInt16(Data, 22));
            Height = (int)(BitConverter.ToInt16(Data, 24));
            //ignoring eventfile and fogcolor for now (seems unused in maps i checked)
            HeaderOffset = 28;
            //initiate all cells
            Cells = new Cell[Width, Height];
            cellRawDataSize = 14;
            BlockFlag = new BitArray(Width * Height);
            int offset = 28 + 3 * (Width / 2 + Width % 2) * (Height / 2);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool flag = ((Data[offset] & 0x01) != 1) || ((Data[offset] & 0x02) != 2);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型6
        /// </summary>
        private void LoadMapType6()
        {
            Width = BitConverter.ToInt16(Data, 16);
            Height = BitConverter.ToInt16(Data, 18);
            HeaderOffset = 40;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 20;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool flag = ((Data[offset] & 0x01) != 1) || ((Data[offset] & 0x02) != 2);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型7
        /// </summary>
        private void LoadMapType7()
        {
            Width = BitConverter.ToInt16(Data, 21);
            Height = BitConverter.ToInt16(Data, 25);
            HeaderOffset = 54;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 15;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = BitConverter.ToInt32(Data, offset);
                    int FrontImage = BitConverter.ToInt16(Data, offset + 6);
                    bool flag = ((BackImage & 0x8000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型8
        /// </summary>
        private void LoadMapType8()
        {
            Width = BitConverter.ToInt16(Data, 16);
            Height = BitConverter.ToInt16(Data, 18);
            HeaderOffset = 40;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 20;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool flag = ((Data[offset] & 0x01) != 1) || ((Data[offset] & 0x02) != 2);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;
                }
            }
        }
        /// <summary>
        /// 加载地图类型100
        /// </summary>
        private void LoadMapType100()
        {
            if (Data[0] != 1 || Data[1] != 0) return;//only support version 1 atm
            Width = BitConverter.ToInt16(Data, 4);
            Height = BitConverter.ToInt16(Data, 6);
            HeaderOffset = 8;
            Cells = new Cell[Width, Height];
            cellRawDataSize = 26;
            BlockFlag = new BitArray(Width * Height);
            int offset = HeaderOffset;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int BackImage = BitConverter.ToInt32(Data, offset + 2);
                    int FrontImage = BitConverter.ToInt16(Data, offset + 12);
                    bool flag = ((BackImage & 0x20000000) != 0 || (FrontImage & 0x8000) != 0);
                    BlockFlag.Set(x + y * Width, flag);
                    offset += cellRawDataSize;

                }
            }
        }

        public Cell GetCell(int x, int y)
        {
            if (Cells[x, y] != null) return Cells[x, y];

            int offset = 0;
            byte flag = 0;
            switch (Version)
            {
                case 1:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell
                    {
                        BackFile = 0,
                        BackImage = (int)(BitConverter.ToInt32(Data, offset) ^ 0xAA38AA38),
                        MiddleFile = 1,
                        MiddleImage = (ushort)(BitConverter.ToInt16(Data, offset += 4) ^ xor),
                        FrontImage = (ushort)(BitConverter.ToInt16(Data, offset += 2) ^ xor),
                        DoorIndex = (byte)(Data[offset += 2] & 0x7F),
                        DoorOffset = Data[++offset],
                        FrontAnimationFrame = Data[++offset],
                        FrontAnimationTick = Data[++offset],
                        FrontFile = (short)(Data[++offset] != 255 ? Data[offset] + 2 : -1),
                        Light = Data[++offset],
                        Unknown = Data[++offset]
                    };
                    offset++;
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 2:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    Cells[x, y].BackImage = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].DoorIndex = (byte)(Data[offset++] & 0x7F);
                    Cells[x, y].DoorOffset = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationTick = Data[offset++];
                    Cells[x, y].FrontFile = (short)(Data[offset++] + 120);
                    Cells[x, y].Light = Data[offset++];
                    Cells[x, y].BackFile = (short)(Data[offset++] + 100);
                    Cells[x, y].MiddleFile = (short)(Data[offset++] + 110);
                    if ((Cells[x, y].BackImage & 0x8000) != 0)
                        Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 3:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    Cells[x, y].BackImage = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].DoorIndex = (byte)(Data[offset++] & 0x7F);
                    Cells[x, y].DoorOffset = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationTick = Data[offset++];
                    Cells[x, y].FrontFile = (short)(Data[offset++] + 120);
                    Cells[x, y].Light = Data[offset++];
                    Cells[x, y].BackFile = (short)(Data[offset++] + 100);
                    Cells[x, y].MiddleFile = (short)(Data[offset++] + 110);
                    Cells[x, y].TileAnimationImage = BitConverter.ToInt16(Data, offset);
                    offset += 7;//2bytes from tileanimframe, 2 bytes always blank?, 2bytes potentialy 'backtiles index', 1byte fileindex for the backtiles?
                    Cells[x, y].TileAnimationFrames = Data[offset++];
                    Cells[x, y].TileAnimationOffset = BitConverter.ToInt16(Data, offset);
                    offset += 14;//tons of light, blending, .. related options i hope
                    if ((Cells[x, y].BackImage & 0x8000) != 0)
                        Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);

                    break;
                case 4:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    Cells[x, y].BackFile = 0;
                    Cells[x, y].MiddleFile = 1;
                    Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Data, offset) ^ xor);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)(BitConverter.ToInt16(Data, offset) ^ xor);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)(BitConverter.ToInt16(Data, offset) ^ xor);
                    offset += 2;
                    Cells[x, y].DoorIndex = (byte)(Data[offset++] & 0x7F);
                    Cells[x, y].DoorOffset = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationTick = Data[offset++];
                    Cells[x, y].FrontFile = (short)(Data[offset++] + 2);
                    Cells[x, y].Light = Data[offset++];
                    if ((Cells[x, y].BackImage & 0x8000) != 0)
                        Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 5:

                    offset = HeaderOffset + ((x / 2) * (Height / 2) * 3) + ((y / 2) * 3);
                    Cells[x, y] = new Cell();
                    Cells[x, y].BackFile = (short)((Data[offset] != 255) ? (Data[offset] + 200) : (-1));
                    Cells[x, y].BackImage = BitConverter.ToUInt16(Data, offset + 1) + 1;

                    offset = HeaderOffset + 3 * (Width / 2 + Width % 2) * (Height / 2) + (x * Height * cellRawDataSize) + (y * cellRawDataSize);
                    flag = Data[offset++];
                    Cells[x, y].MiddleAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = (byte)((Data[offset] != 255) ? Data[offset] : 0);
                    Cells[x, y].FrontAnimationFrame &= 0x8F;
                    offset++;
                    Cells[x, y].MiddleAnimationTick = 0;
                    Cells[x, y].FrontAnimationTick = 0;
                    Cells[x, y].FrontFile = (short)((Data[offset] != 255) ? (Data[offset] + 200) : (-1));
                    offset++;
                    Cells[x, y].MiddleFile = (short)((Data[offset] != 255) ? (Data[offset] + 200) : (-1));
                    offset++;
                    Cells[x, y].MiddleImage = (ushort)(BitConverter.ToUInt16(Data, offset) + 1);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)(BitConverter.ToUInt16(Data, offset) + 1);
                    if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                        Cells[x, y].FrontFile = -1;
                    offset += 2;
                    offset += 3;//mir3 maps dont have doors so dont bother reading the info
                    Cells[x, y].Light = Data[offset];
                    offset += 2;
                    if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                    if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (ushort)((UInt16)Cells[x, y].FrontImage | 0x8000);
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    //if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                    //{
                    //    Cells[x, y].FishingCell = true;
                    //}
                    //else
                    //{
                    //    Cells[x, y].Light *= 2;//expand general mir3 lighting as default range is small. Might break new colour lights.
                    //}

                    break;
                case 6:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    flag = Data[offset++];
                    Cells[x, y].BackFile = (short)((Data[offset] != 255) ? (Data[offset] + 300) : (-1));
                    offset++;
                    Cells[x, y].MiddleFile = (short)((Data[offset] != 255) ? (Data[offset] + 300) : (-1));
                    offset++;
                    Cells[x, y].FrontFile = (short)((Data[offset] != 255) ? (Data[offset] + 300) : (-1));
                    offset++;
                    Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                        Cells[x, y].FrontFile = -1;
                    Cells[x, y].MiddleAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset] == 255 ? (byte)0 : Data[offset];
                    if (Cells[x, y].FrontAnimationFrame > 0x0F)//assuming shanda used same value not sure
                        Cells[x, y].FrontAnimationFrame = (byte)(/*0x80 ^*/Cells[x, y].FrontAnimationFrame & 0x0F);
                    offset++;
                    Cells[x, y].MiddleAnimationTick = 1;
                    Cells[x, y].FrontAnimationTick = 1;
                    Cells[x, y].Light = (byte)(Data[offset] & 0x0F);
                    Cells[x, y].Light *= 4;//far wants all light on mir3 maps to be maxed :p
                    offset += 8;
                    if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                    if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (ushort)((UInt16)Cells[x, y].FrontImage | 0x8000);
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 7:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell
                    {
                        BackFile = 0,
                        BackImage = BitConverter.ToInt32(Data, offset),
                        MiddleFile = 1,
                        MiddleImage = (ushort)BitConverter.ToInt16(Data, offset += 4),
                        FrontImage = (ushort)BitConverter.ToInt16(Data, offset += 2),
                        DoorIndex = (byte)(Data[offset += 2] & 0x7F),
                        DoorOffset = Data[++offset],
                        FrontAnimationFrame = Data[++offset],
                        FrontAnimationTick = Data[++offset],
                        FrontFile = (short)(Data[++offset] + 2),
                        Light = Data[++offset],
                        Unknown = Data[++offset]
                    };
                    if ((Cells[x, y].BackImage & 0x8000) != 0)
                        Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                    offset++;
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 8:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    flag = Data[offset++];
                    Cells[x, y].BackFile = (short)((Data[offset] != 255) ? (Data[offset] + 400) : (-1));
                    offset++;
                    Cells[x, y].MiddleFile = (short)((Data[offset] != 255) ? (Data[offset] + 400) : (-1));
                    offset++;
                    Cells[x, y].FrontFile = (short)((Data[offset] != 255) ? (Data[offset] + 400) : (-1));
                    offset++;
                    Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)(BitConverter.ToInt16(Data, offset) + 1);
                    offset += 2;
                    if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                        Cells[x, y].FrontFile = -1;
                    Cells[x, y].MiddleAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset] == 255 ? (byte)0 : Data[offset];
                    if (Cells[x, y].FrontAnimationFrame > 0x0F)//assuming shanda used same value not sure
                        Cells[x, y].FrontAnimationFrame = (byte)(/*0x80 ^*/ Cells[x, y].FrontAnimationFrame & 0x0F);
                    offset++;
                    Cells[x, y].MiddleAnimationTick = 1;
                    Cells[x, y].FrontAnimationTick = 1;
                    Cells[x, y].Light = (byte)(Data[offset] & 0x0F);
                    Cells[x, y].Light *= 4;//far wants all light on mir3 maps to be maxed :p
                    offset += 8;
                    if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                    if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (ushort)((UInt16)Cells[x, y].FrontImage | 0x8000);
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                case 100:
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    Cells[x, y].BackFile = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].BackImage = BitConverter.ToInt32(Data, offset);
                    offset += 4;
                    Cells[x, y].MiddleFile = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].FrontFile = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].DoorIndex = (byte)(Data[offset++] & 0x7F);
                    Cells[x, y].DoorOffset = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationTick = Data[offset++];
                    Cells[x, y].MiddleAnimationFrame = Data[offset++];
                    Cells[x, y].MiddleAnimationTick = Data[offset++];
                    Cells[x, y].TileAnimationImage = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].TileAnimationOffset = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].TileAnimationFrames = Data[offset++];
                    Cells[x, y].Light = Data[offset++];
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
                default: // version 0
                    offset = HeaderOffset + (x * Height * cellRawDataSize) + (y * cellRawDataSize);

                    Cells[x, y] = new Cell();
                    Cells[x, y].BackFile = 0;
                    Cells[x, y].MiddleFile = 1;
                    Cells[x, y].BackImage = BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].MiddleImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].FrontImage = (ushort)BitConverter.ToInt16(Data, offset);
                    offset += 2;
                    Cells[x, y].DoorIndex = (byte)(Data[offset++] & 0x7F);
                    Cells[x, y].DoorOffset = Data[offset++];
                    Cells[x, y].FrontAnimationFrame = Data[offset++];
                    Cells[x, y].FrontAnimationTick = Data[offset++];
                    Cells[x, y].FrontFile = (short)(Data[offset++] + 2);
                    Cells[x, y].Light = Data[offset++];
                    if ((Cells[x, y].BackImage & 0x8000) != 0)
                        Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                    if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        Cells[x, y].FishingCell = true;
                    Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    break;
            }

            return Cells[x, y];
        }

        public bool EmptyCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y > Height) return false;
            if (Cells[x, y] != null)
            {
                if (Cells[x, y].Blocking())
                    return false;
            }
            if (BlockFlag.Get(x + y * Width))
                return false;
            return true;
        }

        //public int GetFlagInt(int x, int y)
        //{
        //    return BlockFlag.Get(x + y * Width) ? 1 : 0;
        //}
    }

    public sealed class Cell
    {
        public short BackFile;
        public int BackImage;
        public bool Flag;
        public short MiddleFile;
        public ushort MiddleImage;
        public byte MiddleAnimationFrame;
        public byte MiddleAnimationTick;
        public short FrontFile;
        public ushort FrontImage;
        public byte FrontAnimationFrame;
        public byte FrontAnimationTick;
        public byte DoorIndex;
        public byte DoorOffset;
        public short TileAnimationImage;
        public short TileAnimationOffset;
        public byte TileAnimationFrames;
        public byte Light;
        public bool FishingCell;
        public byte Unknown;

        public List<MapObject> Objects;

        /// <summary>
        /// 阻拦
        /// </summary>
        /// <returns></returns>
        public bool Blocking()
        {
            if (Objects != null)
            {
                for (int i = Objects.Count - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(Objects[i].PetOwner) && (Objects[i].PetOwner == MapObject.User.Name)) continue;
                    if (Objects[i].Blocking) return true;
                }
            }

            return Flag;
        }
        /// <summary>
        /// 增加对象
        /// </summary>
        /// <param name="ob"></param>
        public void AddObject(MapObject ob)
        {
            if (Objects == null)
                Objects = new List<MapObject>();

            Objects.Insert(0, ob);
            Sort();

            ob.CurrentCell = this;
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="ob"></param>
        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            if (Objects.Count == 0)
                Objects = null;
            else
                Sort();

            ob.CurrentCell = null;
        }

        public void DrawObjects()
        {
            if (Objects == null) return;

            for (int i = 0; i < Objects.Count; i++)
            {
                var ob = Objects[i];

#if Mobile
                //if (ob == MapObject.MouseObject || ob == MapObject.TargetObject || ob == MapObject.MagicObject)
                //{
                //    if (ob.Race == ObjectType.Monster || ob.Race == ObjectType.Player)
                //    {
                //        if (ob == MapObject.User) continue;
                //        MirLibrary mouse;
                //        if (CEnvir.LibraryList.TryGetValue(LibraryFile.PhoneUI, out mouse))
                //            mouse.Draw(19, ob.DrawX, ob.DrawY, Color.White, true, 1F, ImageType.Image);

                //    }

                //}
#endif

                if (!Objects[i].Dead)
                {
                    //如果是自己，只绘制本体，不绘制特效，以免下面重复绘制（此绘制在前景层之下）, 隐身后这里不绘制下面绘制
                    if (ob.ObjectID == MapObject.User.ObjectID && ob.Opacity == 1F)
                        ob.DrawBody(true);
                    else if (ob.ObjectID != MapObject.User.ObjectID)
                        ob.Draw();
                }

            }
        }

        public void DrawDeadObjects()
        {
            if (Objects == null) return;

            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].Dead)
                {
                    var ob = Objects[i];
                    //如果是自己，只绘制本体，不绘制特效，以免下面重复绘制（此绘制在前景层之下）, 隐身后这里不绘制下面绘制
                    if (ob.ObjectID == MapObject.User.ObjectID && ob.Opacity == 1F)
                        ob.DrawBody(true);
                    else if (ob.ObjectID != MapObject.User.ObjectID)
                        ob.Draw();
                }
            }
        }

        public void Sort()
        {
            Objects.Sort(delegate (MapObject ob1, MapObject ob2)
            {
                if (ob1.Race == ObjectType.Item && ob2.Race != ObjectType.Item)
                {
                    return -1;
                }
                if (ob2.Race == ObjectType.Item && ob1.Race != ObjectType.Item)
                {
                    return 1;
                }
                if (ob1.Race == ObjectType.Spell && ob2.Race != ObjectType.Spell)
                {
                    return -1;
                }
                if (ob2.Race == ObjectType.Spell && ob1.Race != ObjectType.Spell)
                {
                    return 1;
                }
                int value = ob2.Dead.CompareTo(ob1.Dead);
                return (value != 0) ? value : ob1.ObjectID.CompareTo(ob2.ObjectID);
            });
        }
    }

}
