using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Views.DirectX;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Blend = SharpDX.Direct3D9.Blend;
using Color = System.Drawing.Color;
using Matrix = SharpDX.Matrix;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Server.Views
{
    public partial class MapViewer : DevExpress.XtraBars.Ribbon.RibbonForm      //地图编辑器
    {
        public static MapViewer CurrentViewer;
        public DXManager Manager;
        public MapControl Map;

        public DateTime AnimationTime;

        #region MapRegion

        public MapRegion MapRegion
        {
            get { return _MapRegion; }
            set
            {
                if (_MapRegion == value) return;

                MapRegion oldValue = _MapRegion;
                _MapRegion = value;

                OnMapRegionChanged(oldValue, value);
            }
        }
        private MapRegion _MapRegion;
        public event EventHandler<EventArgs> MapRegionChanged;
        public virtual void OnMapRegionChanged(MapRegion oValue, MapRegion nValue)
        {
            Map.Selection.Clear();
            Map.TextureValid = false;

            if (MapRegion == null)
            {
                Map.Width = 0;
                Map.Height = 0;
                Map.Cells = null;
                UpdateScrollBars();
                return;
            }

            if (oValue == null || MapRegion.Map != oValue.Map)
            {
                Map.Load(MapRegion.Map.FileName);
                //修复首次打开地图无法显示滚动条
                UpdateScrollBars();
            }


            Map.Selection = MapRegion.GetPoints(Map.Width);

            MapRegionChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion





        public MapViewer()
        {
            InitializeComponent();

            CurrentViewer = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (CurrentViewer == this)
                CurrentViewer = null;

            Manager.Dispose();
            Manager = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Manager = new DXManager(DXPanel);
            Manager.Create();
            Map = new MapControl(Manager)
            {
                Size = DXPanel.ClientSize,
            };

            DXPanel.MouseWheel += DXPanel_MouseWheel;

            //UpdateScrollBars();
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (Manager == null) return;

            Manager.ResetDevice();
            Map.Size = DXPanel.ClientSize;


            UpdateScrollBars();
        }

        public void Process()
        {
            UpdateEnvironment();
            RenderEnvironment();
        }

        private void UpdateEnvironment()
        {
            if (SEnvir.Now > AnimationTime && Map != null)
            {
                AnimationTime = SEnvir.Now.AddMilliseconds(100);
                Map.Animation++;
            }

        }
        private void RenderEnvironment()
        {
            try
            {
                if (Manager.DeviceLost)
                {
                    Manager.AttemptReset();
                    return;
                }

                Manager.Device.Clear(ClearFlags.Target, Color.Black.ToRawColorBGRA(), 1, 0);
                Manager.Device.BeginScene();
                Manager.Sprite.Begin(SpriteFlags.AlphaBlend);
                Manager.TextSprite.Begin(SpriteFlags.AlphaBlend);
                Manager.SetSurface(Manager.MainSurface);

                Map.Draw();

                Manager.Sprite.End();
                Manager.TextSprite.End();
                Manager.Device.EndScene();
                Manager.Device.Present();

            }
            catch (SharpDXException)
            {
                Manager.DeviceLost = true;
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());

                Manager.AttemptRecovery();
            }
        }


        public void UpdateScrollBars()
        {
            if (Map.Width == 0 || Map.Height == 0)
            {
                MapVScroll.Enabled = false;
                MapHScroll.Enabled = false;
                return;
            }

            MapVScroll.Enabled = true;
            MapHScroll.Enabled = true;

            int wCount = (int)(DXPanel.ClientSize.Width / (Map.CellWidth));
            int hCount = (int)(DXPanel.ClientSize.Height / (Map.CellHeight));


            MapVScroll.Maximum = Math.Max(0, Map.Height - hCount + 20);
            MapHScroll.Maximum = Math.Max(0, Map.Width - wCount + 20);

            if (MapVScroll.Value >= MapVScroll.Maximum)
                MapVScroll.Value = MapVScroll.Maximum - 1;

            if (MapHScroll.Value >= MapHScroll.Maximum)
                MapHScroll.Value = MapHScroll.Maximum - 1;
        }

        private void MapVScroll_ValueChanged(object sender, EventArgs e)
        {
            Map.StartY = MapVScroll.Value;
        }
        private void MapHScroll_ValueChanged(object sender, EventArgs e)
        {
            Map.StartX = MapHScroll.Value;
        }

        private void ZoomResetButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom = 1;
            UpdateScrollBars();
        }

        private void ZoomInButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom *= 2F;
            if (Map.Zoom > 4F)
                Map.Zoom = 4F;

            UpdateScrollBars();
        }

        private void ZoomOutButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.Zoom /= 2;
            if (Map.Zoom < 0.01F)
                Map.Zoom = 0.01F;

            UpdateScrollBars();
        }

        private void AttributesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Map.DrawAttributes = !Map.DrawAttributes;
        }

        private void DXPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            Map.Radius = Math.Max(0, Map.Radius - e.Delta / SystemInformation.MouseWheelScrollDelta);
        }
        private void DXPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Map.MouseDown(e);
        }

        private void DXPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Map.MouseMove(e);
        }

        private void DXPanel_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void DXPanel_MouseEnter(object sender, EventArgs e)
        {
            Map.MouseEnter();
        }

        private void DXPanel_MouseLeave(object sender, EventArgs e)
        {
            Map.MouseLeave();
        }

        private void SelectionButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //钓鱼区域与选择区域只能选其一
            if (Map.FishingDrawSelection)
            {
                Map.FishingDrawSelection = false;
                Map.DrawSelection = !Map.DrawSelection;
            }
            else
                Map.DrawSelection = !Map.DrawSelection;
        }

        private void FishingSelectionButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //钓鱼区域与选择区域只能选其一
            if (Map.DrawSelection)
            {
                Map.DrawSelection = false;
                Map.FishingDrawSelection = !Map.FishingDrawSelection;
            }
            else
                Map.FishingDrawSelection = !Map.FishingDrawSelection;
        }

        private void SaveButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            if (MapRegion == null) return;

            BitArray bitRegion = null;
            Point[] pointRegion = null;

            if (Map.Selection.Count * 8 * 8 > Map.Width * Map.Height)
            {
                bitRegion = new BitArray(Map.Width * Map.Height);

                foreach (Point point in Map.Selection)
                    bitRegion[point.Y * Map.Width + point.X] = true;
            }
            else
            {
                pointRegion = Map.Selection.ToArray();
            }

            MapRegion.BitRegion = bitRegion;
            MapRegion.PointRegion = pointRegion;

            MapRegion.Size = Map.Selection.Count;
        }

        private void CancelButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MapRegion == null) return;

            Map.Selection = MapRegion.GetPoints(Map.Width);

            Map.TextureValid = false;
        }
    }


}


namespace Server.Views.DirectX
{
    public class DXManager : IDisposable
    {
        public Graphics Graphics;

        public readonly Control Target;

        public Dictionary<LibraryFile, MirLibrary> LibraryList = new Dictionary<LibraryFile, MirLibrary>();

        public PresentParameters Parameters { get; private set; }
        public Device Device { get; private set; }
        public Sprite Sprite { get; private set; }
        public Sprite TextSprite { get; private set; }
        public Line Line { get; private set; }

        public Surface CurrentSurface { get; private set; }
        public Surface MainSurface { get; private set; }

        public float Opacity { get; private set; } = 1F;

        public bool Blending { get; private set; }
        public float BlendRate { get; private set; } = 1F;

        public bool DeviceLost { get; set; }

        public List<MirImage> TextureList { get; } = new List<MirImage>();

        public static List<DXSound> SoundList { get; } = new List<DXSound>();

        public Texture AttributeTexture;

        public MapControl Map;

        public DXManager(Control target)
        {
            Target = target;

            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);


            foreach (KeyValuePair<LibraryFile, string> pair in Libraries.LibraryList)
            {
                if (!File.Exists(Path.Combine(Config.ClientPath, pair.Value))) continue;

                LibraryList[pair.Key] = new MirLibrary(Path.Combine(Config.ClientPath, pair.Value), this);
            }

        }

        public void Create()
        {
            Parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                PresentationInterval = PresentInterval.Default,
                BackBufferWidth = Target.ClientSize.Width,
                BackBufferHeight = Target.ClientSize.Height,
                PresentFlags = PresentFlags.LockableBackBuffer,
            };

            Direct3D direct3D = new Direct3D();

            Device = new Device(direct3D, direct3D.Adapters.First().Adapter, DeviceType.Hardware, Target.Handle, CreateFlags.HardwareVertexProcessing, Parameters);

            LoadTextures();

            Device.DialogBoxMode = true;
        }

        private unsafe void LoadTextures()
        {
            Sprite = new Sprite(Device);
            TextSprite = new Sprite(Device);
            Line = new Line(Device) { Width = 1F };

            MainSurface = Device.GetBackBuffer(0, 0);
            CurrentSurface = MainSurface;
            Device.SetRenderTarget(0, MainSurface);

            AttributeTexture = new Texture(Device, 48, 32, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            DataRectangle rect = AttributeTexture.LockRectangle(0, LockFlags.Discard);

            int* data = (int*)rect.DataPointer;

            for (int y = 0; y < 32; y++)
                for (int x = 0; x < 48; x++)
                    data[y * 48 + x] = -1;

        }
        private void CleanUp()
        {
            if (Sprite != null)
            {
                if (!Sprite.IsDisposed)
                    Sprite.Dispose();

                Sprite = null;
            }

            if (TextSprite != null)
            {
                if (!TextSprite.IsDisposed)
                    TextSprite.Dispose();

                TextSprite = null;
            }

            if (Line != null)
            {
                if (!Line.IsDisposed)
                    Line.Dispose();

                Line = null;
            }

            if (CurrentSurface != null)
            {
                if (!CurrentSurface.IsDisposed)
                    CurrentSurface.Dispose();

                CurrentSurface = null;
            }

            if (AttributeTexture != null)
            {
                if (!AttributeTexture.IsDisposed)
                    AttributeTexture.Dispose();

                AttributeTexture = null;
            }


            Map?.DisposeTexture();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();
        }

        public void SetSurface(Surface surface)
        {
            if (CurrentSurface == surface) return;

            Sprite.Flush();
            CurrentSurface = surface;
            Device.SetRenderTarget(0, surface);
        }
        public void SetOpacity(float opacity)
        {
            Device.SetSamplerState(0, SamplerState.MagFilter, 0);

            if (Opacity == opacity)
                return;

            Sprite.Flush();
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);

            if (opacity >= 1 || opacity < 0)
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb(255, 255, 255, 255).ToArgb());
            }
            else
            {
                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseBlendFactor);
                Device.SetRenderState(RenderState.SourceBlendAlpha, Blend.SourceAlpha);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb((byte)(255 * opacity), (byte)(255 * opacity),
                    (byte)(255 * opacity), (byte)(255 * opacity)).ToArgb());
            }

            Opacity = opacity;
            Sprite.Flush();
        }
        public void SetBlend(bool value, float rate = 1F)
        {
            if (value == Blending) return;

            Blending = value;
            BlendRate = 1F;
            Sprite.Flush();

            Sprite.End();
            if (Blending)
            {
                Sprite.Begin(SpriteFlags.DoNotSaveState);
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);

                Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb((byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate)).ToArgb());
            }
            else
            {
                Sprite.Begin(SpriteFlags.AlphaBlend);
            }


            Device.SetRenderTarget(0, CurrentSurface);
        }
        public void SetColour(int colour)
        {
            Sprite.Flush();

            if (colour == 0)
            {
                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
            }
            else
            {

                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.SelectArg1);
                Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Current);
            }

            Sprite.Flush();
        }

        public void ResetDevice()
        {
            CleanUp();

            DeviceLost = true;

            if (Target.ClientSize.Width == 0 || Target.ClientSize.Height == 0) return;

            Parameters = new PresentParameters
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                PresentationInterval = PresentInterval.Default,
                BackBufferWidth = Target.ClientSize.Width,
                BackBufferHeight = Target.ClientSize.Height,
                PresentFlags = PresentFlags.LockableBackBuffer,
            };

            Device.Reset(Parameters);
            LoadTextures();
        }
        public void AttemptReset()
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result.Code == ResultCode.DeviceLost.Code) return;

                if (result.Code == ResultCode.DeviceNotReset.Code)
                {
                    ResetDevice();
                    return;
                }

                if (result.Code != ResultCode.Success.Code) return;

                DeviceLost = false;
            }
            catch (Exception ex)
            {
                SEnvir.SaveError(ex.ToString());
            }
        }
        public void AttemptRecovery()
        {
            try
            {
                Sprite.End();
                TextSprite.End();
            }
            catch
            {
            }

            try
            {
                Device.EndScene();
            }
            catch
            {
            }

            try
            {
                MainSurface = Device.GetBackBuffer(0, 0);
                CurrentSurface = MainSurface;
                Device.SetRenderTarget(0, MainSurface);
            }
            catch
            {
            }
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.TextContrast = 0;
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                if (Sprite != null)
                {
                    if (!Sprite.IsDisposed)
                        Sprite.Dispose();

                    Sprite = null;
                }

                if (TextSprite != null)
                {
                    if (!TextSprite.IsDisposed)
                        TextSprite.Dispose();

                    TextSprite = null;
                }

                if (Line != null)
                {
                    if (!Line.IsDisposed)
                        Line.Dispose();

                    Line = null;
                }

                if (CurrentSurface != null)
                {
                    if (!CurrentSurface.IsDisposed)
                        CurrentSurface.Dispose();

                    CurrentSurface = null;
                }

                if (MainSurface != null)
                {
                    if (!MainSurface.IsDisposed)
                        MainSurface.Dispose();

                    MainSurface = null;
                }

                if (Device != null)
                {
                    if (!Device.IsDisposed)
                        Device.Dispose();

                    Device = null;
                }
                if (AttributeTexture != null)
                {
                    if (!AttributeTexture.IsDisposed)
                        AttributeTexture.Dispose();

                    AttributeTexture = null;
                }

                Map?.DisposeTexture();

                if (Graphics != null)
                {
                    Graphics.Dispose();
                    Graphics = null;
                }

                foreach (KeyValuePair<LibraryFile, MirLibrary> library in LibraryList)
                    library.Value.Dispose();

                Opacity = 0;
                Blending = false;
                BlendRate = 0;
                DeviceLost = false;


            }

        }

        ~DXManager()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

    public sealed class MirLibrary : IDisposable
    {
        public readonly object LoadLocker = new object();

        //版本信息，注意只能20个字节，VersionString 19字节, LibVersion 1字节
        public string VersionString = "BlackDragon Version";
        public const byte LibVersion = 1;

        public string FileName;

        private FileStream _FStream;
        private BinaryReader _BReader;

        public bool Loaded, Loading;

        public MirImage[] Images;

        public readonly DXManager Manager;

        public MirLibrary(string fileName, DXManager manager)
        {
            _FStream = File.OpenRead(fileName);
            _BReader = new BinaryReader(_FStream);

            Manager = manager;
        }
        public void ReadLibrary()
        {
            lock (LoadLocker)
            {
                if (Loading) return;
                Loading = true;
            }

            if (_BReader == null)
            {
                Loaded = true;
                return;
            }

            //读取版本信息
            var currentString = System.Text.Encoding.Default.GetString(_BReader.ReadBytes(19)).TrimEnd('\0');
            var currentVersion = _BReader.ReadByte();
            //兼容Zircon格式
            bool isZirconVersion = currentString != VersionString;
            if (isZirconVersion)
                _BReader.BaseStream.Seek(0, SeekOrigin.Begin);

            using (MemoryStream mstream = new MemoryStream(_BReader.ReadBytes(_BReader.ReadInt32())))
            using (BinaryReader reader = new BinaryReader(mstream))
            {
                if (!isZirconVersion && LibVersion != currentVersion)
                {
                    MessageBox.Show("版本错误, 应为黑龙专用版本: " + LibVersion.ToString() + " 发现版本: " + currentVersion.ToString() + ".", FileName, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }

                Images = new MirImage[reader.ReadInt32()];

                for (int i = 0; i < Images.Length; i++)
                {
                    //判断图片格式
                    byte imageTextureType = reader.ReadByte();
                    if (imageTextureType == 0) continue;

                    Images[i] = new MirImage(reader, Manager, isZirconVersion) { ImageTextureType = imageTextureType, };
                }
            }


            Loaded = true;
        }


        public Size GetSize(int index)
        {
            if (!CheckImage(index)) return Size.Empty;

            return new Size(Images[index].Width, Images[index].Height);
        }
        public Point GetOffSet(int index)
        {
            if (!CheckImage(index)) return Point.Empty;

            return new Point(Images[index].OffSetX, Images[index].OffSetY);
        }
        public MirImage GetImage(int index)
        {
            if (!CheckImage(index)) return null;

            return Images[index];
        }
        public MirImage CreateImage(int index, ImageType type)
        {
            if (!CheckImage(index)) return null;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return null;
            }

            if (texture == null) return null;

            return image;
        }

        private bool CheckImage(int index)
        {
            if (!Loaded) ReadLibrary();

            while (!Loaded)
                Thread.Sleep(1);

            return index >= 0 && index < Images.Length && Images[index] != null;
        }

        public bool VisiblePixel(int index, Point location, bool accurate = true, bool offSet = false)
        {
            if (!CheckImage(index)) return false;

            MirImage image = Images[index];

            if (offSet)
                location = new Point(location.X - image.OffSetX, location.Y - image.OffSetY);

            return image.VisiblePixel(location, accurate);
        }

        public void Draw(int index, float x, float y, Color colour, Rectangle area, float opacity, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = Manager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                Manager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);

                                Manager.SetOpacity(oldOpacity);
                                Manager.Sprite.Transform = Matrix.Identity;
                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                            case 50:
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);

                                Manager.SetOpacity(oldOpacity);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                        }



                        return;
                    }
                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            Manager.SetOpacity(opacity);

            Manager.Sprite.Draw(texture, area, Vector3.Zero, new Vector3(x, y, 0), colour);

            Manager.SetOpacity(oldOpacity);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }
        public void Draw(int index, float x, float y, Color colour, bool useOffSet, float opacity, ImageType type)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            float oldOpacity = Manager.Opacity;
            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    if (!image.ShadowValid) image.CreateShadow(_BReader);
                    texture = image.Shadow;

                    if (useOffSet)
                    {
                        x += image.ShadowOffSetX;
                        y += image.ShadowOffSetY;
                    }


                    if (texture == null)
                    {
                        if (!image.ImageValid) image.CreateImage(_BReader);
                        texture = image.Image;

                        switch (image.ShadowType)
                        {
                            case 177:
                            case 176:
                            case 49:
                                Matrix m = Matrix.Scaling(1F, 0.5f, 0);

                                m.M21 = -0.50F;
                                Manager.Sprite.Transform = m * Matrix.Translation(x + image.Height / 2, y, 0);

                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.None);
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, Vector3.Zero, Color.Black);

                                Manager.SetOpacity(oldOpacity);
                                Manager.Sprite.Transform = Matrix.Identity;
                                Manager.Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                            case 50:
                                if (oldOpacity != 0.5F) Manager.SetOpacity(0.5F);

                                Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), Color.Black);

                                Manager.SetOpacity(oldOpacity);

                                image.ExpireTime = SEnvir.Now.AddMinutes(10);
                                break;
                        }



                        return;
                    }

                    break;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }

            if (texture == null) return;

            Manager.SetOpacity(opacity);

            Manager.Sprite.Draw(texture, Vector3.Zero.ToRawVector3(), new Vector3(x, y, 0).ToRawVector3(), colour);

            Manager.SetOpacity(oldOpacity);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }
        public void DrawBlend(int index, float x, float y, Color colour, bool useOffSet, float rate, ImageType type, byte shadow = 0)
        {
            if (!CheckImage(index)) return;

            MirImage image = Images[index];

            Texture texture;

            switch (type)
            {
                case ImageType.Image:
                    if (!image.ImageValid) image.CreateImage(_BReader);
                    texture = image.Image;
                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                case ImageType.Shadow:
                    return;
                case ImageType.Overlay:
                    if (!image.OverlayValid) image.CreateOverlay(_BReader);
                    texture = image.Overlay;

                    if (useOffSet)
                    {
                        x += image.OffSetX;
                        y += image.OffSetY;
                    }
                    break;
                default:
                    return;
            }
            if (texture == null) return;


            bool oldBlend = Manager.Blending;
            float oldRate = Manager.BlendRate;

            Manager.SetBlend(true, rate);

            Manager.Sprite.Draw(texture, Vector3.Zero, new Vector3(x, y, 0), colour);

            Manager.SetBlend(oldBlend, oldRate);

            image.ExpireTime = SEnvir.Now.AddMinutes(10);
        }


        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                if (Images != null)
                {
                    foreach (MirImage image in Images)
                        image?.Dispose();
                }


                Images = null;


                _FStream?.Dispose();
                _FStream = null;

                _BReader?.Dispose();
                _BReader = null;

                Loading = false;
                Loaded = false;
            }

        }

        ~MirLibrary()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }

    public sealed class MirImage : IDisposable
    {
        public int Position;
        public bool IsZirconVersion;

        public DXManager Manager;

        #region Texture

        public short Width;
        public short Height;
        public short OffSetX;
        public short OffSetY;
        public byte ShadowType;
        public Texture Image;
        public byte ImageTextureType;
        public bool ImageValid { get; private set; }
        public unsafe byte* ImageData;
        public int ImageDataSize;
        #endregion

        #region Shadow
        public short ShadowWidth;
        public short ShadowHeight;

        public short ShadowOffSetX;
        public short ShadowOffSetY;

        public Texture Shadow;
        public byte ShadowTextureType;
        public bool ShadowValid { get; private set; }
        public unsafe byte* ShadowData;
        public int ShadowDataSize;
        #endregion

        #region Overlay
        //public short OverlayWidth;
        //public short OverlayHeight;

        public Texture Overlay;
        public byte OverlayTextureType;
        public bool OverlayValid { get; private set; }
        public unsafe byte* OverlayData;
        public int OverlayDataSize;
        #endregion


        public DateTime ExpireTime;

        public MirImage(BinaryReader reader, DXManager manager, bool isZirconVersion = false)
        {
            IsZirconVersion = isZirconVersion;

            Position = reader.ReadInt32();

            //如果黑龙版
            if (!isZirconVersion)
            {
                ImageDataSize = reader.ReadInt32();
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                OffSetX = reader.ReadInt16();
                OffSetY = reader.ReadInt16();
                ShadowOffSetX = reader.ReadInt16();
                ShadowOffSetY = reader.ReadInt16();
                ShadowType = reader.ReadByte();
                ShadowTextureType = reader.ReadByte();
                OverlayTextureType = reader.ReadByte();

                if (ShadowTextureType > 0)
                {
                    ShadowDataSize = reader.ReadInt32();
                    ShadowWidth = reader.ReadInt16();
                    ShadowHeight = reader.ReadInt16();
                }

                if (OverlayTextureType > 0)
                    OverlayDataSize = reader.ReadInt32();
            }
            //如果zircon版
            else
            {
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                OffSetX = reader.ReadInt16();
                OffSetY = reader.ReadInt16();

                ShadowType = reader.ReadByte();
                ShadowWidth = reader.ReadInt16();
                ShadowHeight = reader.ReadInt16();
                ShadowOffSetX = reader.ReadInt16();
                ShadowOffSetY = reader.ReadInt16();

                //OverlayWidth = reader.ReadInt16();
                //OverlayHeight = reader.ReadInt16();
                int overlayWidth = reader.ReadInt16();
                int overlayHeight = reader.ReadInt16();
                int w = Width + (4 - Width % 4) % 4;
                int h = Height + (4 - Height % 4) % 4;
                ImageDataSize = w * h / 2;
                w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
                h = ShadowHeight + (4 - ShadowHeight % 4) % 4;
                ShadowDataSize = w * h / 2;
                w = overlayWidth + (4 - overlayWidth % 4) % 4;
                h = overlayHeight + (4 - overlayHeight % 4) % 4;
                OverlayDataSize = w * h / 2;

                ImageTextureType = 1;
                ShadowTextureType = 1;
                OverlayTextureType = 1;
            }

            Manager = manager;
        }

        public unsafe bool VisiblePixel(Point p, bool acurrate)
        {
            if (p.X < 0 || p.Y < 0 || !ImageValid || ImageData == null) return false;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (p.X >= w || p.Y >= h)
                return false;

            int x = (p.X - p.X % 4) / 4;
            int y = (p.Y - p.Y % 4) / 4;
            int index = (y * (w / 4) + x) * 8;

            int col0 = ImageData[index + 1] << 8 | ImageData[index], col1 = ImageData[index + 3] << 8 | ImageData[index + 2];

            if (col0 == 0 && col1 == 0) return false;

            if (!acurrate || col1 < col0) return true;

            x = p.X % 4;
            y = p.Y % 4;
            x *= 2;

            return (ImageData[index + 4 + y] & 1 << x) >> x != 1 || (ImageData[index + 4 + y] & 1 << x + 1) >> x + 1 != 1;
        }


        public unsafe void DisposeTexture()
        {
            if (Image != null && !Image.IsDisposed)
                Image.Dispose();

            if (Shadow != null && !Shadow.IsDisposed)
                Shadow.Dispose();

            if (Overlay != null && !Overlay.IsDisposed)
                Overlay.Dispose();

            ImageData = null;
            ShadowData = null;
            OverlayData = null;

            Image = null;
            Shadow = null;
            Overlay = null;

            ImageValid = false;
            ShadowValid = false;
            OverlayValid = false;

            ExpireTime = DateTime.MinValue;

            Manager.TextureList.Remove(this);
        }

        public unsafe void CreateImage(BinaryReader reader)
        {
            if (Position == 0) return;

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = ImageTextureType == 1 ? Format.Dxt1 : ImageTextureType == 3 ? Format.Dxt3 : ImageTextureType == 5 ? Format.Dxt5 : Format.A8R8G8B8;

            Image = new Texture(Manager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Image.LockRectangle(0, LockFlags.Discard);
            ImageData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(ImageDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(ImageDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Image.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            ImageValid = true;
            ExpireTime = SEnvir.Now.AddMinutes(30);
            Manager.TextureList.Add(this);
        }
        public unsafe void CreateShadow(BinaryReader reader)
        {
            if (Position == 0 || ShadowDataSize == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = ShadowWidth + (4 - ShadowWidth % 4) % 4;
            int h = ShadowHeight + (4 - ShadowHeight % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = ShadowTextureType == 1 ? Format.Dxt1 : ShadowTextureType == 3 ? Format.Dxt3 : ShadowTextureType == 5 ? Format.Dxt5 : Format.A8R8G8B8;

            Shadow = new Texture(Manager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Shadow.LockRectangle(0, LockFlags.Discard);
            ShadowData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(ShadowDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(ShadowDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Shadow.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            ShadowValid = true;
        }
        public unsafe void CreateOverlay(BinaryReader reader)
        {
            if (Position == 0 || OverlayDataSize == 0) return;

            if (!ImageValid)
                CreateImage(reader);

            int w = Width + (4 - Width % 4) % 4;
            int h = Height + (4 - Height % 4) % 4;

            if (w == 0 || h == 0) return;

            Format format = OverlayTextureType == 1 ? Format.Dxt1 : OverlayTextureType == 3 ? Format.Dxt3 : OverlayTextureType == 5 ? Format.Dxt5 : Format.A8R8G8B8;

            Overlay = new Texture(Manager.Device, w, h, 1, Usage.None, format, Pool.Managed);
            DataRectangle rect = Overlay.LockRectangle(0, LockFlags.Discard);
            OverlayData = (byte*)rect.DataPointer;

            byte[] buffer = null;
            lock (reader)
            {
                reader.BaseStream.Seek(Position + ImageDataSize + ShadowDataSize, SeekOrigin.Begin);
                if (IsZirconVersion)
                    buffer = reader.ReadBytes(OverlayDataSize);
                else
                    buffer = Decompress(reader.ReadBytes(OverlayDataSize));
            }
            DataStream data = new DataStream(rect.DataPointer, buffer.Length, true, true);
            data.Write(buffer, 0, buffer.Length);

            Overlay.UnlockRectangle(0);
            data.Dispose();

            buffer = null;

            OverlayValid = true;
        }

        private static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (DeflateStream stream = new DeflateStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                Position = 0;

                Width = 0;
                Height = 0;
                OffSetX = 0;
                OffSetY = 0;

                ShadowWidth = 0;
                ShadowHeight = 0;
                ShadowOffSetX = 0;
                ShadowOffSetY = 0;

                //OverlayWidth = 0;
                //OverlayHeight = 0;
            }

        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }
        ~MirImage()
        {
            Dispose(false);
        }

        #endregion

    }

    public enum ImageType
    {
        Image,
        Shadow,
        Overlay,
    }


    public class MapControl : IDisposable
    {
        public DXManager Manager;

        public MapControl(DXManager manager)
        {
            Manager = manager;
            Zoom = 1;
        }

        #region Size

        public Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;

                Size oldValue = _Size;
                _Size = value;

                OnSizeChanged(oldValue, value);
            }
        }
        private Size _Size;
        public event EventHandler<EventArgs> SizeChanged;
        public virtual void OnSizeChanged(Size oValue, Size nValue)
        {
            TextureValid = false;

            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public Cell[,] Cells;
        public int Width, Height;

        #region StartX

        public int StartX
        {
            get { return _StartX; }
            set
            {
                if (_StartX == value) return;

                int oldValue = _StartX;
                _StartX = value;

                OnStartXChanged(oldValue, value);
            }
        }
        private int _StartX;
        public event EventHandler<EventArgs> StartXChanged;
        public virtual void OnStartXChanged(int oValue, int nValue)
        {
            StartXChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region StartY

        public int StartY
        {
            get { return _StartY; }
            set
            {
                if (_StartY == value) return;

                int oldValue = _StartY;
                _StartY = value;

                OnStartYChanged(oldValue, value);
            }
        }
        private int _StartY;
        public event EventHandler<EventArgs> StartYChanged;
        public virtual void OnStartYChanged(int oValue, int nValue)
        {
            StartYChanged?.Invoke(this, EventArgs.Empty);

            TextureValid = false;
        }

        #endregion

        #region DrawAttributes

        public bool DrawAttributes
        {
            get { return _DrawAttributes; }
            set
            {
                if (_DrawAttributes == value) return;

                bool oldValue = _DrawAttributes;
                _DrawAttributes = value;

                OnDrawAttributesChanged(oldValue, value);
            }
        }
        private bool _DrawAttributes;
        public event EventHandler<EventArgs> DrawAttributesChanged;
        public virtual void OnDrawAttributesChanged(bool oValue, bool nValue)
        {
            DrawAttributesChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region DrawSelection

        public bool DrawSelection
        {
            get { return _DrawSelection; }
            set
            {
                if (_DrawSelection == value) return;

                bool oldValue = _DrawSelection;
                _DrawSelection = value;

                OnDrawSelectionChanged(oldValue, value);
            }
        }
        private bool _DrawSelection;
        public event EventHandler<EventArgs> DrawSelectionChanged;
        public virtual void OnDrawSelectionChanged(bool oValue, bool nValue)
        {
            DrawSelectionChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        public HashSet<Point> Selection = new HashSet<Point>();

        public bool FishingDrawSelection;

        //Zoom to handle
        public const int BaseCellWidth = 48;
        public const int BaseCellHeight = 32;

        public float CellWidth => BaseCellWidth * Zoom;
        public float CellHeight => BaseCellHeight * Zoom;


        #region Zoom

        public float Zoom
        {
            get { return _Zoom; }
            set
            {
                if (_Zoom == value) return;

                float oldValue = _Zoom;
                _Zoom = value;

                OnZoomChanged(oldValue, value);
            }
        }
        private float _Zoom;
        public event EventHandler<EventArgs> ZoomChanged;
        public virtual void OnZoomChanged(float oValue, float nValue)
        {
            ZoomChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Animation

        public int Animation
        {
            get { return _Animation; }
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
        public virtual void OnAnimationChanged(int oValue, int nValue)
        {
            AnimationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Border

        public bool Border
        {
            get { return _Border; }
            set
            {
                if (_Border == value) return;

                bool oldValue = _Border;
                _Border = value;

                OnBorderChanged(oldValue, value);
            }
        }
        private bool _Border;
        public event EventHandler<EventArgs> BorderChanged;
        public virtual void OnBorderChanged(bool oValue, bool nValue)
        {
            BorderChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion


        #region MouseLocation

        public Point MouseLocation
        {
            get { return _MouseLocation; }
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
        public virtual void OnMouseLocationChanged(Point oValue, Point nValue)
        {
            MouseLocationChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        #region Radius

        public int Radius
        {
            get { return _Radius; }
            set
            {
                if (_Radius == value) return;

                int oldValue = _Radius;
                _Radius = value;

                OnRadiusChanged(oldValue, value);
            }
        }
        private int _Radius;
        public event EventHandler<EventArgs> RadiusChanged;
        public virtual void OnRadiusChanged(int oValue, int nValue)
        {
            RadiusChanged?.Invoke(this, EventArgs.Empty);
            TextureValid = false;
        }

        #endregion

        private LoadMapType LoadMapType; //地图类型


        #region Texture
        public bool TextureValid { get; set; }
        public Texture ControlTexture { get; set; }
        public Size TextureSize { get; set; }
        public Surface ControlSurface { get; set; }
        public DateTime ExpireTime { get; protected set; }

        protected virtual void CreateTexture()
        {
            if (ControlTexture == null || Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = Size;
                ControlTexture = new Texture(Manager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default); ;
                ControlSurface = ControlTexture.GetSurfaceLevel(0);
                Manager.Map = this;
            }

            Surface previous = Manager.CurrentSurface;
            Manager.SetSurface(ControlSurface);

            Manager.Device.Clear(ClearFlags.Target, Color.Black.ToRawColorBGRA(), 0, 0);

            OnClearTexture();

            Manager.SetSurface(previous);
            TextureValid = true;
        }
        protected virtual void OnClearTexture()
        {
            DrawFloor();

            //DrawObjects();

            //DrawPlacements();


        }
        public virtual void DisposeTexture()
        {
            if (ControlTexture != null)
            {
                if (!ControlTexture.IsDisposed)
                    ControlTexture.Dispose();

                ControlTexture = null;
            }

            if (ControlSurface != null)
            {
                if (!ControlSurface.IsDisposed)
                    ControlSurface.Dispose();

                ControlSurface = null;
            }

            TextureSize = Size.Empty;
            ExpireTime = DateTime.MinValue;
            TextureValid = false;

            if (Manager.Map == this)
                Manager.Map = null;
        }

        #endregion


        public void Draw()
        {
            if (Size.Width <= 0 || Size.Height <= 0) return;

            DrawControl();
        }
        protected virtual void DrawControl()
        {
            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = Manager.Opacity;

            Manager.SetOpacity(1F);

            Manager.Sprite.Draw(ControlTexture, Vector3.Zero, Vector3.Zero, Color.White);

            Manager.SetOpacity(oldOpacity);
        }

        public void DrawFloor() //画地面物品
        {
            int minX = Math.Max(0, StartX - 1);
            int maxX = Math.Min(Width - 1, StartX + (int)Math.Ceiling(Size.Width / CellWidth));

            int minY = Math.Max(0, StartY - 1);
            int maxY = Math.Min(Height - 1, StartY + (int)Math.Ceiling(Size.Height / CellHeight));

            Matrix scale = Matrix.Scaling(Zoom, Zoom, 1);

            if (LoadMapType == LoadMapType.BlackDragonMir3Map)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (y % 2 != 0) continue;

                    float drawY = (y - StartY) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x % 2 != 0) continue;

                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell tile = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (!Libraries.KROrder.TryGetValue(tile.BackFile, out file)) continue;

                        if (!Manager.LibraryList.TryGetValue(file, out library)) continue;

                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);
                        //绘制背景层，既地砖
                        library.Draw(tile.BackImage, 0, 0, Color.White, false, 1F, ImageType.Image);
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.MiddleImage - 1;
                            //有动画帧跳出
                            if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                                continue; //   index += GameScene.Game.MapControl.Animation% cell.MiddleAnimationFrame;

                            Size s = library.GetSize(index);
                            //绘制中间层标准（48 * 32 或96 * 64） 的地砖
                            if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                            {
                                Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);

                                library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            }
                        }
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.FrontImage - 1;
                            //有动画帧跳出
                            if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                                continue; //  index += GameScene.Game.MapControl.Animation% cell.FrontAnimationFrame;

                            Size s = library.GetSize(index);
                            //绘制前景层标准（48 * 32 或96 * 64） 的地砖
                            if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                            {
                                Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);

                                library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            }
                        }
                    }
                }

                maxY = Math.Min(Height - 1, StartY + 20 + (int)Math.Ceiling(Size.Height / CellHeight));
                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.MiddleImage - 1;

                            bool blend = false;
                            if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                            {
                                index += Animation % (cell.MiddleAnimationFrame & 0x0F);  //图片显示错误
                                blend = (cell.MiddleAnimationFrame & 0x20) > 0;
                            }

                            Size s = library.GetSize(index);
                            //绘制中间层大物体（不是 48*32 或96*64 的地砖）
                            if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth * 2 || s.Height != CellHeight * 2))
                            {
                                Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);

                                if (!blend)
                                    library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                else
                                    library.DrawBlend(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            }
                        }


                        if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                        {
                            int index = cell.FrontImage - 1;

                            bool blend = false;
                            if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                            {
                                index += Animation % (cell.FrontAnimationFrame & 0x7F);
                                blend = (cell.MiddleAnimationFrame & 0x80) > 0;
                            }

                            Size s = library.GetSize(index);

                            //绘制前景层大物体（不是 48*32 或96*64 的地砖）
                            if ((s.Width != CellWidth || s.Height != CellHeight) && (s.Width != CellWidth * 2 || s.Height != CellHeight * 2))
                            {
                                Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);

                                if (!blend)
                                    library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                else
                                    library.DrawBlend(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                            }
                        }
                    }
                }

                //Invalid Tile = 59
                //Selected Tile = 58
            }
            else if (LoadMapType == LoadMapType.Others)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (y % 2 != 0) continue;

                    float drawY = (y - StartY) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        if (x % 2 != 0) continue;

                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell tile = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (tile.BackImage != -1)
                        {
                            if (!Libraries.KROrder.TryGetValue(tile.BackFile, out file)) continue;
                            if (!Manager.LibraryList.TryGetValue(file, out library)) continue;

                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);
                            int backimage = (tile.BackImage & 0x1FFFF) - 1;
                            //绘制背景层，既地砖
                            library.Draw(backimage, 0, 0, Color.White, false, 1F, ImageType.Image);
                        }
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;
                        if (cell.MiddleFile != -1)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                            {
                                int index = cell.MiddleImage - 1;
                                if (index >= 0)
                                {
                                    if (cell.MiddleFile > 199) //传3地图
                                    {
                                        //有动画帧跳出
                                        if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                                            continue;

                                        Size s = library.GetSize(index);
                                        //绘制中间层标准（48 * 32 或96 * 64） 的地砖
                                        if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                        {
                                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);
                                            library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                        }
                                    }
                                    else
                                    {
                                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);
                                        library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                    }
                                }

                            }
                        }
                    }
                }

                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (cell.FrontFile != -1)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                            {
                                int index = (cell.FrontImage & 0x7FFF) - 1;
                                if (index >= 0)
                                {
                                    //有动画帧跳出
                                    if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                                        continue;
                                    Size s = library.GetSize(index);
                                    //绘制前景层标准（48 * 32 或96 * 64） 的地砖
                                    if ((s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                    {
                                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - BaseCellHeight, 0), scale);
                                        library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                    }
                                }

                            }
                        }
                    }
                }

                maxY = Math.Min(Height - 1, StartY + 20 + (int)Math.Ceiling(Size.Height / CellHeight));
                for (int y = minY; y <= maxY; y++)
                {
                    float drawY = (y - StartY + 1) * BaseCellHeight;

                    for (int x = minX; x <= maxX; x++)
                    {
                        float drawX = (x - StartX) * BaseCellWidth;

                        Cell cell = Cells[x, y];

                        MirLibrary library;
                        LibraryFile file;

                        if (cell.MiddleFile > 0)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.MiddleFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                            {
                                int index = cell.MiddleImage - 1;
                                if (cell.MiddleFile > 199) //传3地图
                                {
                                    bool blend = false;
                                    if (cell.MiddleAnimationFrame > 1 && cell.MiddleAnimationFrame < 255)
                                    {
                                        index += Animation % (cell.MiddleAnimationFrame & 0x0F);
                                        blend = (cell.MiddleAnimationFrame & 0x20) > 0;
                                    }
                                    Size s = library.GetSize(index);
                                    //绘制中间层大物体（不是 48*32 或96*64 的地砖），标准地砖在OnClearTexture()已经绘制
                                    if (!(s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                    {
                                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);

                                        if (!blend)
                                            library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                        else
                                            library.DrawBlend(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                    }
                                }
                            }
                        }

                        if (cell.FrontFile != -1)
                        {
                            if (Libraries.KROrder.TryGetValue(cell.FrontFile, out file) && file != LibraryFile.Tilesc && Manager.LibraryList.TryGetValue(file, out library))
                            {
                                int index = (cell.FrontImage & 0x7FFF) - 1;
                                if (index >= 0)
                                {
                                    bool blend = false;
                                    if (cell.FrontAnimationFrame > 1 && cell.FrontAnimationFrame < 255)
                                    {
                                        var animation = cell.FrontAnimationFrame & 0x7F;
                                        blend = (cell.FrontAnimationFrame & 0x80) > 0;
                                        var animationTick = cell.FrontAnimationTick;
                                        index += Animation % (animation + animation * animationTick) / (1 + animationTick);
                                    }
                                    Size s = library.GetSize(index);
                                    //绘制前景层大物体（不是 48*32 或96*64 的地砖），标准地砖在OnClearTexture()已经绘制
                                    if (!(s.Width == CellWidth && s.Height == CellHeight) || (s.Width == CellWidth * 2 && s.Height == CellHeight * 2))
                                    {
                                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY - s.Height, 0), scale);
                                        //如果需要混合
                                        if (blend)
                                        {
                                            //新盛大地图
                                            if ((cell.FrontFile > 99) & (cell.FrontFile < 199))
                                                library.DrawBlend(index, 0, 0, Color.White, true, 1F, ImageType.Image);
                                            //传3地图
                                            else if (cell.FrontFile > 199)
                                                library.DrawBlend(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                            //老地图灯柱 index >= 2723 && index <= 2732
                                            else
                                                library.DrawBlend(index, 0, 0, Color.White, index >= 2723 && index <= 2732, 1F, ImageType.Image);
                                        }
                                        //不需要混合
                                        else
                                            library.Draw(index, 0, 0, Color.White, false, 1F, ImageType.Image);
                                    }
                                }

                            }
                        }
                    }
                }

            }

            maxY = Math.Min(Height - 1, StartY + (int)Math.Ceiling(Size.Height / CellHeight));
            Manager.SetOpacity(0.35F);
            for (int y = minY; y <= maxY; y++)
            {
                float drawY = (y - StartY) * BaseCellHeight;

                for (int x = minX; x <= maxX; x++)
                {
                    float drawX = (x - StartX) * BaseCellWidth;

                    Cell tile = Cells[x, y];

                    if (tile.Flag)
                    {
                        if (!DrawAttributes) continue;

                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);

                        //markLibrary.Draw(59, 0, 0, Color.White, false, 1F, ImageType.Image);
                        Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Red);

                        if (FishingDrawSelection) //如果是钓鱼区域，绘制钓鱼图标
                        {
                            if (!Selection.Contains(new Point(x, y))) continue;

                            MirLibrary library;
                            if (!Manager.LibraryList.TryGetValue(LibraryFile.SmTilesc, out library)) continue;
                            Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);
                            library.Draw(116, 0, 0, Color.White, false, 0.8F, ImageType.Image);
                        }
                    }
                    else
                    {
                        if (!DrawSelection) continue;
                        if (!Selection.Contains(new Point(x, y))) continue;

                        Manager.Sprite.Transform = Matrix.Multiply(Matrix.Translation(drawX, drawY, 0), scale);

                        Manager.Sprite.Draw(Manager.AttributeTexture, Vector3.Zero, Vector3.Zero, Color.Yellow);

                        //markLibrary.Draw(58, 0, 0, Color.Lime, false, 1F, ImageType.Image);
                        //If Selected.
                    }
                }
            }
            Manager.Sprite.Flush();

            Manager.SetOpacity(1F);
            if (Border)
            {
                Manager.Line.Draw(new[]
                {
                    new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                    new Vector2((MouseLocation.X - StartX)*CellWidth + CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                    new Vector2((MouseLocation.X - StartX)*CellWidth + CellWidth, (MouseLocation.Y - StartY)*CellHeight + CellHeight),
                    new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight + CellHeight),
                    new Vector2((MouseLocation.X - StartX)*CellWidth, (MouseLocation.Y - StartY)*CellHeight),
                }, Color.Lime);

                if (Radius > 0)
                    Manager.Line.Draw(new[]
                    {
                        new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY - Radius)*CellHeight),
                        new Vector2((MouseLocation.X - StartX + Radius)*CellWidth + CellWidth, (MouseLocation.Y - StartY- Radius)*CellHeight),
                        new Vector2((MouseLocation.X - StartX + Radius)*CellWidth + CellWidth, (MouseLocation.Y - StartY + Radius)*CellHeight + CellHeight),
                        new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY + Radius)*CellHeight + CellHeight),
                        new Vector2((MouseLocation.X - StartX - Radius)*CellWidth, (MouseLocation.Y - StartY - Radius)*CellHeight),
                    }, Color.Lime);

                //增加坐标显示
                var font = new FontDescription() { FaceName = "微软雅园", Weight = FontWeight.Bold };
                var dxFont = new SharpDX.Direct3D9.Font(Manager.Device, font);
                var szText = string.Format("<{0}:{1}>", MouseLocation.X, MouseLocation.Y);
                dxFont.DrawText(Manager.TextSprite, szText, 10, 10, Color.Yellow.ToRawColorBGRA());

                Manager.TextSprite.Flush();
                dxFont.Dispose();
            }

            Manager.Sprite.Transform = Matrix.Identity;
        }

        #region 传奇2和传奇3的地图支持
        public void Load(string fileName)
        {
            try
            {
                if (!File.Exists(Config.MapPath + fileName + ".map")) return;

                byte[] MapBytes = File.ReadAllBytes(Config.MapPath + fileName + ".map");
                using (MemoryStream mStream = new MemoryStream(MapBytes))
                {
                    if (MapBytes[0] == 55)
                    {
                        //黑龙专用地图格式（目前还未开始写，先用原版）
                        LoadBlackDragonMir3MapFormat(mStream);
                        LoadMapType = LoadMapType.BlackDragonMir3Map;
                    }
                    else
                    {
                        //其它地图格式
                        LoadOtherMapFormats(MapBytes);
                        LoadMapType = LoadMapType.Others;
                    }
                }
                /*
                using (BinaryReader reader = new BinaryReader(mStream))
                {
                    mStream.Seek(22, SeekOrigin.Begin);
                    Width = reader.ReadInt16();
                    Height = reader.ReadInt16();

                    mStream.Seek(28, SeekOrigin.Begin);

                    Cells = new Cell[Width, Height];
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            Cells[x, y] = new Cell();

                    for (int x = 0; x < Width / 2; x++)
                        for (int y = 0; y < Height / 2; y++)
                        {
                            Cells[(x * 2), (y * 2)].BackFile = reader.ReadByte();
                            Cells[(x * 2), (y * 2)].BackImage = reader.ReadUInt16();
                        }

                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                        {
                            byte flag = reader.ReadByte();
                            Cells[x, y].MiddleAnimationFrame = reader.ReadByte();

                            byte value = reader.ReadByte();
                            Cells[x, y].FrontAnimationFrame = value == 255 ? 0 : value;
                            Cells[x, y].FrontAnimationFrame &= 0x8F; //Probably a Blend Flag

                            Cells[x, y].FrontFile = reader.ReadByte();
                            Cells[x, y].MiddleFile = reader.ReadByte();

                            Cells[x, y].MiddleImage = reader.ReadUInt16() + 1;
                            Cells[x, y].FrontImage = reader.ReadUInt16() + 1;

                            mStream.Seek(3, SeekOrigin.Current);

                            Cells[x, y].Light = (byte)(reader.ReadByte() & 0x0F) * 2;

                            mStream.Seek(1, SeekOrigin.Current);

                            Cells[x, y].Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);
                        }
                }
                */
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
            TextureValid = false;
        }

        private void LoadBlackDragonMir3MapFormat(MemoryStream mStream)
        {
            using (BinaryReader reader = new BinaryReader(mStream))
            {
                mStream.Seek(22, SeekOrigin.Begin);
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();

                mStream.Seek(28, SeekOrigin.Begin);

                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        Cells[x, y] = new Cell();

                for (int x = 0; x < Width / 2; x++)
                    for (int y = 0; y < Height / 2; y++)
                    {
                        Cells[(x * 2), (y * 2)].BackFile = reader.ReadByte() + 200;
                        Cells[(x * 2), (y * 2)].BackImage = reader.ReadUInt16();
                    }

                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        byte flag = reader.ReadByte();
                        Cells[x, y].MiddleAnimationFrame = reader.ReadByte();

                        byte value = reader.ReadByte();
                        Cells[x, y].FrontAnimationFrame = value == 255 ? 0 : value;
                        Cells[x, y].FrontAnimationFrame &= 0x8F; //可能是混合标记   前动画帧

                        Cells[x, y].FrontFile = reader.ReadByte() + 200; //前面文件
                        Cells[x, y].MiddleFile = reader.ReadByte() + 200;//中间文件

                        Cells[x, y].MiddleImage = reader.ReadUInt16() + 1; //中间图片
                        Cells[x, y].FrontImage = reader.ReadUInt16() + 1;  //前面图片

                        mStream.Seek(3, SeekOrigin.Current);  //查找原点.当前

                        Cells[x, y].Light = (byte)(reader.ReadByte() & 0x0F) * 2;

                        mStream.Seek(1, SeekOrigin.Current);

                        Cells[x, y].Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);
                    }
            }
        }

        private void LoadOtherMapFormats(byte[] mapBytes)
        {
            //(0-99) c#自定义地图格式 title:
            if (mapBytes[2] == 0x43 && mapBytes[3] == 0x23)
            {
                LoadMapType100(mapBytes);
                return;
            }
            //(200-299) 韩版传奇3 title:
            if (mapBytes[0] == 0)
            {
                LoadMapType5(mapBytes);
                return;
            }
            //(300-399) 盛大传奇3 title: (C) SNDA, MIR3.
            if (mapBytes[0] == 0x0F && mapBytes[5] == 0x53 && mapBytes[14] == 0x33)
            {
                LoadMapType6(mapBytes);
                return;
            }
            //(400-499) 应该是盛大传奇3第二种格式，未知？无参考
            if (mapBytes[0] == 0x0F && mapBytes[5] == 0x4D && mapBytes[14] == 0x33)
            {
                LoadMapType8(mapBytes);
                return;
            }
            //(0-99) wemades antihack map (laby maps) title start with: Mir2 AntiHack
            if (mapBytes[0] == 0x15 && mapBytes[4] == 0x32 && mapBytes[6] == 0x41 && mapBytes[19] == 0x31)
            {
                LoadMapType4(mapBytes);
                return;
            }
            //(0-99) wemades 2010 map format i guess title starts with: Map 2010 Ver 1.0
            if (mapBytes[0] == 0x10 && mapBytes[2] == 0x61 && mapBytes[7] == 0x31 && mapBytes[14] == 0x31)
            {
                LoadMapType1(mapBytes);
                return;
            }
            //(100-199) shanda's 2012 format and one of shandas(wemades) older formats share same header info, only difference is the filesize
            if ((mapBytes[4] == 0x0F || (mapBytes[4] == 0x03)) && mapBytes[18] == 0x0D && mapBytes[19] == 0x0A)
            {
                int w = mapBytes[0] + (mapBytes[1] << 8);
                int h = mapBytes[2] + (mapBytes[3] << 8);
                if (mapBytes.Length > 52 + (w * h * 14))
                {
                    LoadMapType3(mapBytes);
                }
                else
                {
                    LoadMapType2(mapBytes);
                }
                return;
            }
            //(0-99) 3/4 heroes map format (myth/lifcos i guess)
            if (mapBytes[0] == 0x0D && mapBytes[1] == 0x4C && mapBytes[7] == 0x20 && mapBytes[11] == 0x6D)
            {
                LoadMapType7(mapBytes);
                return;
            }
            //if it's none of the above load the default old school format
            LoadMapType0(mapBytes);
        }

        private void LoadMapType0(byte[] Bytes)
        {
            try
            {
                int offset = 0;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 52;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        Cells[x, y].BackFile = 0;
                        Cells[x, y].MiddleFile = 1;
                        Cells[x, y].BackImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].MiddleImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].FrontImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].DoorIndex = (byte)(Bytes[offset++] & 0x7F);
                        Cells[x, y].DoorOffset = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationTick = Bytes[offset++];
                        Cells[x, y].FrontFile = (short)(Bytes[offset++] + 2);
                        Cells[x, y].Light = Bytes[offset++];
                        if ((Cells[x, y].BackImage & 0x8000) != 0)
                            Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType1(byte[] Bytes)
        {
            try
            {
                int offset = 21;
                int w = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                int xor = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                int h = BitConverter.ToInt16(Bytes, offset);
                Width = (w ^ xor);
                Height = (h ^ xor);
                offset = 54;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell
                        {
                            BackFile = 0,
                            BackImage = (int)(BitConverter.ToInt32(Bytes, offset) ^ 0xAA38AA38),
                            MiddleFile = 1,
                            MiddleImage = (short)(BitConverter.ToInt16(Bytes, offset += 4) ^ xor),
                            FrontImage = (short)(BitConverter.ToInt16(Bytes, offset += 2) ^ xor),
                            DoorIndex = (byte)(Bytes[offset += 2] & 0x7F),
                            DoorOffset = Bytes[++offset],
                            FrontAnimationFrame = Bytes[++offset],
                            FrontAnimationTick = Bytes[++offset],
                            FrontFile = (short)(Bytes[++offset] != 255 ? Bytes[offset] + 2 : -1),
                            Light = Bytes[++offset],
                            Unknown = Bytes[++offset]
                        };
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        offset++;
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType2(byte[] Bytes)
        {
            try
            {
                int offset = 0;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 52;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        Cells[x, y].BackImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].MiddleImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].FrontImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].DoorIndex = (byte)(Bytes[offset++] & 0x7F);
                        Cells[x, y].DoorOffset = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationTick = Bytes[offset++];
                        Cells[x, y].FrontFile = (short)(Bytes[offset++] + 120);
                        Cells[x, y].Light = Bytes[offset++];
                        Cells[x, y].BackFile = (short)(Bytes[offset++] + 100);
                        Cells[x, y].MiddleFile = (short)(Bytes[offset++] + 110);
                        if ((Cells[x, y].BackImage & 0x8000) != 0)
                            Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType3(byte[] Bytes)
        {
            try
            {
                int offset = 0;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 52;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        Cells[x, y].BackImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].MiddleImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].FrontImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].DoorIndex = (byte)(Bytes[offset++] & 0x7F);
                        Cells[x, y].DoorOffset = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationTick = Bytes[offset++];
                        Cells[x, y].FrontFile = (short)(Bytes[offset++] + 120);
                        Cells[x, y].Light = Bytes[offset++];
                        Cells[x, y].BackFile = (short)(Bytes[offset++] + 100);
                        Cells[x, y].MiddleFile = (short)(Bytes[offset++] + 110);
                        Cells[x, y].TileAnimationImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 7;//2bytes from tileanimframe, 2 bytes always blank?, 2bytes potentialy 'backtiles index', 1byte fileindex for the backtiles?
                        Cells[x, y].TileAnimationFrames = Bytes[offset++];
                        Cells[x, y].TileAnimationOffset = BitConverter.ToInt16(Bytes, offset);
                        offset += 14;//tons of light, blending, .. related options i hope
                        if ((Cells[x, y].BackImage & 0x8000) != 0)
                            Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType4(byte[] Bytes)
        {
            try
            {
                int offset = 31;
                int w = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                int xor = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                int h = BitConverter.ToInt16(Bytes, offset);
                Width = (w ^ xor);
                Height = (h ^ xor);
                offset = 64;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        Cells[x, y].BackFile = 0;
                        Cells[x, y].MiddleFile = 1;
                        Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Bytes, offset) ^ xor);
                        offset += 2;
                        Cells[x, y].MiddleImage = (short)(BitConverter.ToInt16(Bytes, offset) ^ xor);
                        offset += 2;
                        Cells[x, y].FrontImage = (short)(BitConverter.ToInt16(Bytes, offset) ^ xor);
                        offset += 2;
                        Cells[x, y].DoorIndex = (byte)(Bytes[offset++] & 0x7F);
                        Cells[x, y].DoorOffset = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationTick = Bytes[offset++];
                        Cells[x, y].FrontFile = (short)(Bytes[offset++] + 2);
                        Cells[x, y].Light = Bytes[offset++];
                        if ((Cells[x, y].BackImage & 0x8000) != 0)
                            Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType5(byte[] Bytes)
        {
            try
            {
                int offset = 20;
                short Attribute = BitConverter.ToInt16(Bytes, offset);
                Width = (int)(BitConverter.ToInt16(Bytes, offset += 2));
                Height = (int)(BitConverter.ToInt16(Bytes, offset += 2));
                //ignoring eventfile and fogcolor for now (seems unused in maps i checked)
                offset = 28;
                //initiate all cells
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                    }
                }
                //read all back tiles
                for (int x = 0; x < Width / 2; x++)
                {
                    for (int y = 0; y < Height / 2; y++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Cells[(x * 2) + (i % 2), (y * 2) + (i / 2)].BackFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 200) : (-1));
                            Cells[(x * 2) + (i % 2), (y * 2) + (i / 2)].BackImage = BitConverter.ToUInt16(Bytes, offset + 1) + 1;
                        }
                        offset += 3;
                    }
                }
                //read rest of data
                offset = 28 + 3 * (Width / 2 + Width % 2) * (Height / 2);
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        byte flag = Bytes[offset++];
                        Cells[x, y].MiddleAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = ((Bytes[offset] != 255) ? Bytes[offset] : 0);
                        Cells[x, y].FrontAnimationFrame &= 0x8F;
                        offset++;
                        Cells[x, y].MiddleAnimationTick = 0;
                        Cells[x, y].FrontAnimationTick = 0;
                        Cells[x, y].FrontFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 200) : (-1));
                        offset++;
                        Cells[x, y].MiddleFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 200) : (-1));
                        offset++;
                        Cells[x, y].MiddleImage = (ushort)(BitConverter.ToUInt16(Bytes, offset) + 1);
                        offset += 2;
                        Cells[x, y].FrontImage = (ushort)(BitConverter.ToUInt16(Bytes, offset) + 1);
                        if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                            Cells[x, y].FrontFile = -1;
                        offset += 2;
                        offset += 3;//mir3 maps dont have doors so dont bother reading the info
                        Cells[x, y].Light = (byte)(Bytes[offset] & 0x0F);
                        offset += 2;
                        if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                        if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (short)((UInt16)Cells[x, y].FrontImage | 0x8000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                        {
                            Cells[x, y].FishingCell = true;
                        }
                        else
                        {
                            Cells[x, y].Light *= 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType6(byte[] Bytes)
        {
            try
            {
                int offset = 16;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 40;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        byte flag = Bytes[offset++];
                        Cells[x, y].BackFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 300) : (-1));
                        offset++;
                        Cells[x, y].MiddleFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 300) : (-1));
                        offset++;
                        Cells[x, y].FrontFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 300) : (-1));
                        offset++;
                        Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        Cells[x, y].MiddleImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        Cells[x, y].FrontImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                            Cells[x, y].FrontFile = -1;
                        Cells[x, y].MiddleAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset] == 255 ? (byte)0 : Bytes[offset];
                        if (Cells[x, y].FrontAnimationFrame > 0x0F)//assuming shanda used same value not sure
                            Cells[x, y].FrontAnimationFrame = (byte)(/*0x80 ^*/Cells[x, y].FrontAnimationFrame & 0x0F);
                        offset++;
                        Cells[x, y].MiddleAnimationTick = 1;
                        Cells[x, y].FrontAnimationTick = 1;
                        Cells[x, y].Light = (byte)(Bytes[offset] & 0x0F);
                        Cells[x, y].Light *= 4;//far wants all light on mir3 maps to be maxed :p
                        offset += 8;
                        if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                        if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (short)((UInt16)Cells[x, y].FrontImage | 0x8000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType7(byte[] Bytes)
        {
            try
            {
                int offset = 21;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 4;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 54;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell
                        {
                            BackFile = 0,
                            BackImage = BitConverter.ToInt32(Bytes, offset),
                            MiddleFile = 1,
                            MiddleImage = BitConverter.ToInt16(Bytes, offset += 4),
                            FrontImage = BitConverter.ToInt16(Bytes, offset += 2),
                            DoorIndex = (byte)(Bytes[offset += 2] & 0x7F),
                            DoorOffset = Bytes[++offset],
                            FrontAnimationFrame = Bytes[++offset],
                            FrontAnimationTick = Bytes[++offset],
                            FrontFile = (short)(Bytes[++offset] + 2),
                            Light = Bytes[++offset],
                            Unknown = Bytes[++offset]
                        };
                        if ((Cells[x, y].BackImage & 0x8000) != 0)
                            Cells[x, y].BackImage = ((Cells[x, y].BackImage & 0x7FFF) | 0x20000000);
                        offset++;
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        private void LoadMapType8(byte[] Bytes)
        {
            try
            {
                int offset = 16;
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 40;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        byte flag = Bytes[offset++];
                        Cells[x, y].BackFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 400) : (-1));
                        offset++;
                        Cells[x, y].MiddleFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 400) : (-1));
                        offset++;
                        Cells[x, y].FrontFile = (short)((Bytes[offset] != 255) ? (Bytes[offset] + 400) : (-1));
                        offset++;
                        Cells[x, y].BackImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        Cells[x, y].MiddleImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        Cells[x, y].FrontImage = (short)(BitConverter.ToInt16(Bytes, offset) + 1);
                        offset += 2;
                        if (Cells[x, y].FrontImage == 1 && Cells[x, y].FrontFile == 200)
                            Cells[x, y].FrontFile = -1;
                        Cells[x, y].MiddleAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset] == 255 ? (byte)0 : Bytes[offset];
                        if (Cells[x, y].FrontAnimationFrame > 0x0F)//assuming shanda used same value not sure
                            Cells[x, y].FrontAnimationFrame = (byte)(/*0x80 ^*/ Cells[x, y].FrontAnimationFrame & 0x0F);
                        offset++;
                        Cells[x, y].MiddleAnimationTick = 1;
                        Cells[x, y].FrontAnimationTick = 1;
                        Cells[x, y].Light = (byte)(Bytes[offset] & 0x0F);
                        Cells[x, y].Light *= 4;//far wants all light on mir3 maps to be maxed :p
                        offset += 8;
                        if ((flag & 0x01) != 1) Cells[x, y].BackImage |= 0x20000000;
                        if ((flag & 0x02) != 2) Cells[x, y].FrontImage = (short)((UInt16)Cells[x, y].FrontImage | 0x8000);
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }



        private void LoadMapType100(byte[] Bytes)
        {
            try
            {
                int offset = 4;
                if (Bytes[0] != 1 || Bytes[1] != 0) return;//only support version 1 atm
                Width = BitConverter.ToInt16(Bytes, offset);
                offset += 2;
                Height = BitConverter.ToInt16(Bytes, offset);
                offset = 8;
                Cells = new Cell[Width, Height];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Cells[x, y] = new Cell();
                        Cells[x, y].BackFile = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].BackImage = BitConverter.ToInt32(Bytes, offset);
                        offset += 4;
                        Cells[x, y].MiddleFile = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].MiddleImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].FrontFile = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].FrontImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].DoorIndex = (byte)(Bytes[offset++] & 0x7F);
                        Cells[x, y].DoorOffset = Bytes[offset++];
                        Cells[x, y].FrontAnimationFrame = Bytes[offset++];
                        Cells[x, y].FrontAnimationTick = Bytes[offset++];
                        Cells[x, y].MiddleAnimationFrame = Bytes[offset++];
                        Cells[x, y].MiddleAnimationTick = Bytes[offset++];
                        Cells[x, y].TileAnimationImage = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].TileAnimationOffset = BitConverter.ToInt16(Bytes, offset);
                        offset += 2;
                        Cells[x, y].TileAnimationFrames = Bytes[offset++];
                        Cells[x, y].Light = Bytes[offset++];
                        Cells[x, y].Flag = ((Cells[x, y].BackImage & 0x20000000) != 0 || (Cells[x, y].FrontImage & 0x8000) != 0);
                        if (Cells[x, y].Light >= 100 && Cells[x, y].Light <= 119)
                            Cells[x, y].FishingCell = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }

        #endregion

        #region IDisposable Support

        public bool IsDisposed { get; private set; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;

                if (ControlTexture != null)
                {
                    if (!ControlTexture.IsDisposed)
                        ControlTexture.Dispose();

                    ControlTexture = null;
                }

                if (ControlSurface != null)
                {
                    if (!ControlSurface.IsDisposed)
                        ControlSurface.Dispose();

                    ControlSurface = null;
                }

                _Size = Size.Empty;

                TextureValid = false;
                TextureSize = Size.Empty;
                ExpireTime = DateTime.MinValue;

                if (Manager?.Map == this)
                    Manager.Map = null;
            }

        }

        ~MapControl()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void MouseDown(MouseEventArgs e)
        {


            switch (e.Button)
            {
                case MouseButtons.Left:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            //添加选择区域与钓鱼区域判断
                            if (DrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag) continue;
                                Selection.Add(new Point(x, y));
                            }
                            else if (FishingDrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || !Cells[x, y].Flag) continue;
                                Selection.Add(new Point(x, y));
                            }
                        }


                    break;
                case MouseButtons.Right:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            //添加选择区域与钓鱼区域判断
                            if (DrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag) continue;
                                Selection.Remove(new Point(x, y));
                            }
                            else if (FishingDrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || !Cells[x, y].Flag) continue;
                                Selection.Remove(new Point(x, y));
                            }
                        }
                    break;
                case MouseButtons.Middle:
                    if (MouseLocation.X < 0 || MouseLocation.X >= Width || MouseLocation.Y < 0 || MouseLocation.Y >= Height) return;
                    //添加选择区域与钓鱼区域判断
                    if (DrawSelection)
                    { if (Cells[MouseLocation.X, MouseLocation.Y].Flag) return; }
                    else if (FishingDrawSelection)
                    { if (!Cells[MouseLocation.X, MouseLocation.Y].Flag) return; }
                    else
                    { if (Cells[MouseLocation.X, MouseLocation.Y].Flag) return; }

                    HashSet<Point> doneList = new HashSet<Point> { MouseLocation };
                    Queue<Point> todoList = new Queue<Point>();

                    todoList.Enqueue(MouseLocation);

                    if (Selection.Contains(MouseLocation)) //removing
                    {
                        while (todoList.Count > 0)
                        {
                            Point p = todoList.Dequeue();

                            for (int i = 0; i < 8; i++)
                            {
                                Point nPoint = Functions.Move(p, (MirDirection)i);

                                if (nPoint.X < 0 || nPoint.X >= Width || nPoint.Y < 0 || nPoint.Y >= Height) continue;
                                //添加选择区域与钓鱼区域判断
                                if (DrawSelection) if (Cells[nPoint.X, nPoint.Y].Flag) continue;
                                    else if (FishingDrawSelection) if (!Cells[nPoint.X, nPoint.Y].Flag) continue;

                                if (doneList.Contains(nPoint)) continue;

                                if (!Selection.Contains(nPoint)) continue;

                                doneList.Add(nPoint);
                                todoList.Enqueue(nPoint);
                            }

                            Selection.Remove(p);
                        }

                    }
                    else
                    {
                        while (todoList.Count > 0)
                        {
                            Point p = todoList.Dequeue();

                            for (int i = 0; i < 8; i++)
                            {
                                Point nPoint = Functions.Move(p, (MirDirection)i);

                                if (nPoint.X < 0 || nPoint.X >= Width || nPoint.Y < 0 || nPoint.Y >= Height) continue;
                                //添加选择区域与钓鱼区域判断
                                if (DrawSelection) if (Cells[nPoint.X, nPoint.Y].Flag) continue;
                                    else if (FishingDrawSelection) if (!Cells[nPoint.X, nPoint.Y].Flag) continue;

                                if (doneList.Contains(nPoint)) continue;

                                if (Selection.Contains(nPoint)) continue;

                                doneList.Add(nPoint);
                                todoList.Enqueue(nPoint);
                            }

                            Selection.Add(p);
                        }
                    }


                    break;
            }
            TextureValid = false;
        }
        public void MouseMove(MouseEventArgs e)
        {
            MouseLocation = new Point(Math.Min(Width, Math.Max(0, (int)(e.X / CellWidth) + StartX)), Math.Min(Height, Math.Max(0, (int)(e.Y / CellHeight) + StartY)));

            switch (e.Button)
            {
                case MouseButtons.Left:
                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            //添加选择区域与钓鱼区域判断
                            if (DrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag) continue;
                                Selection.Add(new Point(x, y));
                            }
                            else if (FishingDrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || !Cells[x, y].Flag) continue;
                                Selection.Add(new Point(x, y));
                            }
                        }
                    break;
                case MouseButtons.Right:

                    for (int y = MouseLocation.Y - Radius; y <= MouseLocation.Y + Radius; y++)
                        for (int x = MouseLocation.X - Radius; x <= MouseLocation.X + Radius; x++)
                        {
                            //添加选择区域与钓鱼区域判断
                            if (DrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || Cells[x, y].Flag) continue;
                                Selection.Remove(new Point(x, y));
                            }
                            else if (FishingDrawSelection)
                            {
                                if (x < 0 || x >= Width || y < 0 || y >= Height || !Cells[x, y].Flag) continue;
                                Selection.Remove(new Point(x, y));
                            }
                        }
                    break;
            }
        }

        public void MouseEnter()
        {
            Border = true;
        }
        public void MouseLeave()
        {
            Border = false;
        }



        public sealed class Cell
        {
            public int BackFile;
            public int BackImage;

            public int MiddleFile;
            public int MiddleImage;

            public int FrontFile;
            public int FrontImage;

            public int FrontAnimationFrame;
            public int FrontAnimationTick;

            public int MiddleAnimationFrame;
            public int MiddleAnimationTick;

            public int Light;

            public bool Flag;

            public bool FishingCell;

            public byte DoorIndex;

            public byte DoorOffset;

            public short TileAnimationImage;

            public short TileAnimationOffset;

            public byte TileAnimationFrames;

            public byte Unknown;
        }

    }


}