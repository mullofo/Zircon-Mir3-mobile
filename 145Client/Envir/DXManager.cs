using Client.Controls;
using Client.Scenes;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Blend = SharpDX.Direct3D9.Blend;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Envir
{
    public static class DXManager    //DX管理器
    {
        /// <summary>
        /// 图样
        /// </summary>
        public static Graphics Graphics { get; private set; }
        /// <summary>
        /// 参数
        /// </summary>
        public static PresentParameters Parameters { get; private set; }
        /// <summary>
        /// 方法
        /// </summary>
        public static Device Device { get; private set; }
        /// <summary>
        /// 精灵
        /// </summary>
        public static Sprite Sprite { get; private set; }
        /// <summary>
        /// 线条
        /// </summary>
        public static Line Line { get; private set; }
        /// <summary>
        /// 当前表面
        /// </summary>
        public static Surface CurrentSurface { get; private set; }
        /// <summary>
        /// 主面
        /// </summary>
        public static Surface MainSurface { get; private set; }
        /// <summary>
        /// 透明度
        /// </summary>
        public static float Opacity { get; private set; } = 1F;
        /// <summary>
        /// 混合
        /// </summary>
        public static bool Blending { get; private set; }
        /// <summary>
        /// 混合率
        /// </summary>
        public static float BlendRate { get; private set; } = 1F;
        public static BlendType BlendType { get; private set; } = BlendType.NORMAL;
        /// <summary>
        /// 渲染失败
        /// </summary>
        public static bool DeviceLost { get; set; }
        /// <summary>
        /// 控制表
        /// </summary>
        public static List<DXControl> ControlList { get; } = new List<DXControl>();
        /// <summary>
        /// 结构列表
        /// </summary>
        public static List<MirImage> TextureList { get; } = new List<MirImage>();
        /// <summary>
        /// 音效列表
        /// </summary>
        public static List<DXSound> SoundList { get; } = new List<DXSound>();

        public static Texture ScratchTexture;
        public static Surface ScratchSurface;

        public static PixelShader GrayScalePixelShader;
        public static bool GrayScale;

        public static byte[] PalleteData;
        private static Texture _ColourPallete;
        /// <summary>
        /// 调色板
        /// </summary>
        public static Texture ColourPallete
        {
            get
            {
                if (_ColourPallete == null || _ColourPallete.IsDisposed)
                {
                    _ColourPallete = null;

                    if (PalleteData != null)
                    {
                        _ColourPallete = new Texture(Device, 200, 149, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                        DataRectangle rect = _ColourPallete.LockRectangle(0, LockFlags.Discard);
                        DataStream data = new DataStream(rect.DataPointer, PalleteData.Length, true, true);
                        data.Write(PalleteData, 0, PalleteData.Length);
                        _ColourPallete.UnlockRectangle(0);
                        data.Dispose();
                    }
                }

                return _ColourPallete;
            }
        }

        //窗口分辨率参数
        public const int LightWidth = 1024;
        public const int LightHeight = 768;

        public static byte[] LightData;
        private static Texture _LightTexture;
        /// <summary>
        /// 光照
        /// </summary>
        public static Texture LightTexture
        {
            get
            {
                if (_LightTexture == null || _LightTexture.IsDisposed)
                    CreateLight();

                return _LightTexture;
            }
        }

        private static Surface _LightSurface;
        /// <summary>
        /// 表面光
        /// </summary>
        public static Surface LightSurface
        {
            get
            {
                if (_LightSurface == null || _LightSurface.IsDisposed)
                    _LightSurface = LightTexture.GetSurfaceLevel(0);

                return _LightSurface;
            }
        }

        public static Texture PoisonTexture;     //污染纹理

        public static bool ForceCleanSync
        {
            get;
            private set;
        }

        static DXManager()               //DX管理
        {
            Graphics = Graphics.FromHwnd(IntPtr.Zero);
            ConfigureGraphics(Graphics);      //配置图形
        }


        public static void Create()  //创建
        {
            Parameters = new PresentParameters  //预设参数
            {
                Windowed = !Config.FullScreen,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate,
                BackBufferWidth = CEnvir.Target.ClientSize.Width,
                BackBufferHeight = CEnvir.Target.ClientSize.Height,
                PresentFlags = PresentFlags.LockableBackBuffer,
            };

            Direct3D direct3D = new Direct3D();

            //Capabilities devCaps = direct3D.GetDeviceCaps(direct3D.Adapters.First().Adapter, DeviceType.Hardware);
            //if (devCaps.DeviceCaps != 0)
            //{
            //    foreach (DisplayMode displayMode in direct3D.Adapters.First().GetDisplayModes(Parameters.BackBufferFormat))
            //    {
            //        if (displayMode.Width < 1024) continue;
            //        if (Globals.ValidResolutions.Any(x => x == new Size(displayMode.Width, displayMode.Height))) continue;
            //        Globals.ValidResolutions.Add(new Size(displayMode.Width, displayMode.Height));
            //    }
            //}
            //else
            //{
            //    Globals.ValidResolutions.Add(new Size(1024, 768));
            //}

            Device = new Device(direct3D, direct3D.Adapters.First().Adapter, DeviceType.Hardware, CEnvir.Target.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve | CreateFlags.Multithreaded, Parameters);

            LoadTextures();
            LoadPixelsShaders();
            Device.DialogBoxMode = true;


            const string path = @".\Data\Pallete.png";    //客户端调色盘的位置

            if (File.Exists(path))                             //如果 文件存在（路径）
            {
                using (Bitmap pallete = new Bitmap(path))      //使用（位图调色板=新位图（路径））  
                {
                    //位图数据=调色板锁定的位置（新矩形（点空调色板的尺寸），图像锁定模式只读，指定格式为每像素 32 位；alpha、红色、绿色和蓝色分量各使用 8 位。）
                    BitmapData data = pallete.LockBits(new Rectangle(Point.Empty, pallete.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    //调色板数据=新字节[调色板宽度*调色板高度*4]
                    PalleteData = new byte[pallete.Width * pallete.Height * 4];
                    //拷贝副本（数据扫描，调色板数据，0，调色板数据长度）
                    Marshal.Copy(data.Scan0, PalleteData, 0, PalleteData.Length);
                    //调色板 解锁 （数据）
                    pallete.UnlockBits(data);
                }
            }

            /*
                        Bitmap text = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(text))
            {
                ConfigureGraphics(g);
                g.Clear(Color.Black);
                string line = "?";
                System.Drawing.Font font = new System.Drawing.Font(Config.FontName, 13F);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 8)))
                {
                    g.DrawString(line, font, brush, 1, 0);
                    g.DrawString(line, font, brush, 0, 1);
                    g.DrawString(line, font, brush, 2, 1);
                    g.DrawString(line, font, brush, 1, 2);
                }

                g.DrawString(line, font, Brushes.White, 1, 1);
            }
            text.Save(@"C:\Zircon Server\Data Works\Game\Q.bmp");*/


        }

        private static void LoadPixelsShaders() //加载灰阶着色文件
        {
            string text = ".\\Data\\Shaders\\grayscale.fx";

            if (File.Exists(text))
            {
                using (var graphicsStream = ShaderBytecode.CompileFromFile(text, "ps_main", "ps_2_0", ShaderFlags.None))
                {
                    GrayScalePixelShader = new PixelShader(Device, graphicsStream);
                }
            }
        }

        private static unsafe void LoadTextures() //加载纹理
        {
            // CleanUp();

            Sprite = new Sprite(Device);

            Line = new Line(Device) { Width = 1F };

            MainSurface = Device.GetBackBuffer(0, 0);
            CurrentSurface = MainSurface;
            Device.SetRenderTarget(0, MainSurface);


            PoisonTexture = new Texture(Device, 6, 6, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

            DataRectangle rect = PoisonTexture.LockRectangle(0, LockFlags.Discard);

            int* data = (int*)rect.DataPointer;

            for (int y = 0; y < 6; y++)
                for (int x = 0; x < 6; x++)
                    data[y * 6 + x] = x == 0 || y == 0 || x == 5 || y == 5 ? -16777216 : -1;

            ScratchTexture = new Texture(Device, Parameters.BackBufferWidth, Parameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ScratchSurface = ScratchTexture.GetSurfaceLevel(0);
        }

        private static void CreateLight()  //创建光效
        {
            Texture light = new Texture(Device, LightWidth, LightHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);


            DataRectangle rect = light.LockRectangle(0, LockFlags.Discard);

            using (Bitmap image = new Bitmap(LightWidth, LightHeight, LightWidth * 4, PixelFormat.Format32bppArgb, rect.DataPointer))
            using (Graphics graphics = Graphics.FromImage(image))
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(0, 0, LightWidth, LightHeight));
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                    brush.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    brush.CenterColor = Color.FromArgb(255, 120, 120, 120);
                    graphics.FillPath(brush, path);
                    graphics.Save();
                }
            }
            light.UnlockRectangle(0);
            //rect.Data.Dispose();

            _LightTexture = light;
        }
        private static void CleanUp()  //清理
        {
            if (Sprite != null)
            {
                if (!Sprite.IsDisposed)
                    Sprite.Dispose();

                Sprite = null;
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

            if (_ColourPallete != null)
            {
                if (!_ColourPallete.IsDisposed)
                    _ColourPallete.Dispose();

                _ColourPallete = null;
            }

            if (ScratchTexture != null)
            {
                if (!ScratchTexture.IsDisposed)
                    ScratchTexture.Dispose();

                ScratchTexture = null;
            }

            if (ScratchSurface != null)
            {
                if (!ScratchSurface.IsDisposed)
                    ScratchSurface.Dispose();

                ScratchSurface = null;
            }

            if (PoisonTexture != null)
            {
                if (!PoisonTexture.IsDisposed)
                    PoisonTexture.Dispose();

                PoisonTexture = null;
            }


            if (_LightTexture != null)
            {
                if (!_LightTexture.IsDisposed)
                    _LightTexture.Dispose();

                _LightTexture = null;
            }


            if (_LightSurface != null)
            {
                if (!_LightSurface.IsDisposed)
                    _LightSurface.Dispose();

                _LightSurface = null;
            }

            if (GrayScalePixelShader != null)
            {
                if (!GrayScalePixelShader.IsDisposed)
                    GrayScalePixelShader.Dispose();

                GrayScalePixelShader = null;
            }

            for (int i = ControlList.Count - 1; i >= 0; i--)
                ControlList[i].DisposeTexture();

            for (int i = TextureList.Count - 1; i >= 0; i--)
                TextureList[i].DisposeTexture();
        }

        public static void ForceReworkTextures(bool forceNow = false)
        {
            if (!forceNow)
            {
                ForceCleanSync = true;
            }
            else
            {
                CleanALLForced();
            }
        }

        public static void CleanALLForced()
        {
            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                ControlList[i].DisposeTexture();
            }
            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                TextureList[i].DisposeTexture();
            }
            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (DXControl.ActiveScene != null && DXControl.ActiveScene is GameScene)
                {
                    GameScene gameScene = (GameScene)DXControl.ActiveScene;
                    if (gameScene != null && gameScene.MapControl != null && gameScene.MapControl.MapInfo != null && gameScene.MapControl.MapInfo.Music != 0 && DXSoundManager.SoundList.TryGetValue(gameScene.MapControl.MapInfo.Music, out var value) && value != null && value.FileName == SoundList[i].FileName)
                    {
                        continue;
                    }
                }
                SoundList[i].DisposeSoundBuffer();
            }
            ForceCleanSync = false;
        }

        public static void MemoryClear()  //内存清除
        {
            for (int i = ControlList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < ControlList[i].ExpireTime) continue;

                ControlList[i].DisposeTexture();
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < TextureList[i].ExpireTime) continue;

                TextureList[i].DisposeTexture();
            }

            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (CEnvir.Now < SoundList[i].ExpireTime) continue;

                SoundList[i].DisposeSoundBuffer();
            }
        }

        public static void Unload()  //处理
        {
            CleanUp();

            if (Device != null)
            {

                if (Device.Direct3D != null)
                {
                    if (!Device.Direct3D.IsDisposed)
                        Device.Direct3D.Dispose();
                }

                if (!Device.IsDisposed)
                    Device.Dispose();

                Device = null;
            }
        }
        /// <summary>
        /// 设置曲面
        /// </summary>
        /// <param name="surface"></param>
        public static void SetSurface(Surface surface)
        {
            if (CurrentSurface == surface) return;

            Sprite.Flush();
            CurrentSurface = surface;
            Device.SetRenderTarget(0, surface);
        }
        /// <summary>
        /// 设置不透明度
        /// </summary>
        /// <param name="opacity"></param>
        public static void SetOpacity(float opacity)
        {
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
        /// <summary>
        /// 设置混合模式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rate"></param>
        public static void SetBlend(bool value, float rate = 1F, BlendType blendType = BlendType.NORMAL)
        {
            if (Blending == value && BlendRate == rate && BlendType == blendType) return;

            Blending = value;
            BlendRate = rate;
            BlendType = blendType;

            //Sprite.Flush();
            Sprite.End();

            if (Blending)
            {
                Sprite.Begin(SpriteFlags.DoNotSaveState);
                Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);

                switch (blendType)
                {
                    case BlendType.INVLIGHT:
                        Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
                        Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);
                        break;
                    case BlendType.COLORFY:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendType.MASK:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        break;
                    case BlendType.EFFECTMASK:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.DestinationAlpha);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendType.HIGHLIGHT:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    #region 粒子相关
                    //相关混合算法来自官方c++版代码
                    case BlendType._BLEND_NORMAL:
                        //if (Device.Material.Diffuse.Alpha < 1.0f || Device.Material.Emissive.Alpha < 1.0f)
                        //{
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

                        //    Device.SetRenderState(RenderState.SourceBlend, Blend.One);
                        //    Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        //}
                        //else
                        //{
                        //    Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        //}
                        break;
                    case BlendType._BLEND_LIGHT:
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendType._BLEND_LIGHTINV:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.One);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);
                        break;
                    case BlendType._BLEND_INVNORMAL:
                        //if (Device.Material.Diffuse.Alpha < 1.0f || Device.Material.Emissive.Alpha < 1.0f)
                        //{
                        //    Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //    Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                        //    Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

                        //    Device.SetRenderState(RenderState.SourceBlend, Blend.One);
                        //    Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        //}
                        //else
                        //{
                        //    Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //    Device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
                        //    Device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
                        //    Device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);

                        //    Device.SetRenderState(RenderState.SourceBlend, Blend.InverseSourceAlpha);
                        //    Device.SetRenderState(RenderState.DestinationBlend, Blend.Zero);
                        //}
                        break;
                    case BlendType._BLEND_INVLIGHT:
                        Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture | TextureArgument.Complement);
                        Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseDestinationColor);
                        break;
                    case BlendType._BLEND_INVLIGHTINV:
                        Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture | TextureArgument.Complement);
                        Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
                        Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseDestinationColor);
                        break;
                    //case 6:
                    //    Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    //    Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                    //    Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);
                    //    break;
                    #endregion
                    default:
                        Device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationColor);
                        Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                }

                Device.SetRenderState(RenderState.BlendFactor, Color.FromArgb((byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate), (byte)(255 * rate)).ToArgb());
            }
            else
            {
                Sprite.Begin(SpriteFlags.AlphaBlend);
            }


            Device.SetRenderTarget(0, CurrentSurface);
        }
        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="colour"></param>
        public static void SetColour(int colour)
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
        /// <summary>
        /// 设置灰阶
        /// </summary>
        /// <param name="value"></param>
        public static void SetGrayscale(bool value)
        {
            GrayScale = value;
            if (value)
            {
                if (Device.PixelShader != GrayScalePixelShader)
                {
                    Sprite.Flush();
                    Device.PixelShader = GrayScalePixelShader;
                }
            }
            else
            {
                if (Device.PixelShader != null)
                {
                    Sprite.Flush();
                    Device.PixelShader = null;
                }
            }
        }

        public static void ResetDevice()  //恢复设置
        {
            CleanUp();

            DeviceLost = true;

            if (CEnvir.Target.ClientSize.Width == 0 || CEnvir.Target.ClientSize.Height == 0) return;

            Parameters = new PresentParameters  //预设参数
            {
                Windowed = !Config.FullScreen,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.X8R8G8B8,
                PresentationInterval = Config.VSync ? PresentInterval.Default : PresentInterval.Immediate,
                BackBufferWidth = CEnvir.Target.ClientSize.Width,
                BackBufferHeight = CEnvir.Target.ClientSize.Height,
                PresentFlags = PresentFlags.LockableBackBuffer,
            };

            Device.Reset(Parameters);
            LoadTextures();
        }
        public static void AttemptReset()  //尝试重置
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result.Code == ResultCode.DeviceLost.Code) return;

                if (result.Code == ResultCode.DeviceNotReset.Code)
                {
                    CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
                    ResetDevice();
                    return;
                }

                if (result.Code != ResultCode.Success.Code) return;

                DeviceLost = false;
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
        public static void AttemptRecovery()  //尝试恢复
        {
            try
            {
                Sprite.End();
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
        /// <summary>
        /// 切换全屏
        /// </summary>
        public static void ToggleFullScreen()
        {
            if (CEnvir.Target == null) return;

            Config.FullScreen = !Config.FullScreen;
            DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

            CEnvir.Target.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            CEnvir.Target.MaximizeBox = false;

            CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
            ResetDevice();
            CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;

            //CEnvir.Target.TopMost = Config.FullScreen;
        }
        /// <summary>
        /// 设定分辨率
        /// </summary>
        /// <param name="size"></param>
        public static void SetResolution(Size size)
        {
            if (CEnvir.Target.ClientSize == size) return;

            Device.Clear(ClearFlags.Target, Color.Black.ToRawColorBGRA(), 0, 0);
            Device.Present();

            CEnvir.Target.ClientSize = size;
            CEnvir.Target.MaximizeBox = false;

            ResetDevice();
        }
        /// <summary>
        /// 配置图形
        /// </summary>
        /// <param name="graphics"></param>
        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            graphics.TextContrast = 0;
        }
    }

    #region 混合状态
    /// <summary>
    /// 混合状态
    /// </summary>
    public enum BlendType
    {
        /// <summary>
        /// 混合状态 没有
        /// </summary>
        NONE = -1,
        NORMAL = 0,
        LIGHT = 1,
        LIGHTINV = 2,
        INVNORMAL = 3,
        INVLIGHT = 4,
        INVLIGHTINV = 5,
        INVCOLOR = 6,
        INVBACKGROUND = 7,
        COLORFY = 8,
        MASK = 9,
        HIGHLIGHT = 10,
        EFFECTMASK = 11,


        _BLEND_NORMAL,
        /// <summary>
        /// 混合 灯光
        /// </summary>
        _BLEND_LIGHT,
        /// <summary>
        /// 混合 轻
        /// </summary>
        _BLEND_LIGHTINV,
        _BLEND_INVNORMAL,
        _BLEND_INVLIGHT,
        /// <summary>
        /// 混合 浅色
        /// </summary>
        _BLEND_INVLIGHTINV
    }
    #endregion
}
