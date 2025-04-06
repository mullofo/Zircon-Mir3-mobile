using Client.Controls;
using Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Texture = MonoGame.Extended.Texture;

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
        public static PresentationParameters Parameters { get; private set; }
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
        //public static BlendState BlendState { get; private set; } = BlendState.NORMAL;
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

        //public static PixelShader GrayScalePixelShader;
        public static bool GrayScale;

        public static byte[] PalleteData;

        /// <summary>
        /// 调色板
        /// </summary>
        public static Texture ColourPallete;

        //窗口分辨率参数
        public const int LightWidth = 1024;
        public const int LightHeight = 768;

        public static byte[] LightData;
        /// <summary>
        /// 光照
        /// </summary>
        public static Texture LightTexture;

        public static Texture PoisonTexture;     //污染纹理

        public static bool ForceCleanSync
        {
            get;
            private set;
        }

        //------------------------------------------
        //public static Texture ZoomScratchTexture;
        //public static Surface ZoomScratchSurface;
        //public static Texture OpenGameImage;
        //public static Texture ButtonTexture;

        #region Blend混合算法
        public static BlendType BlendType = BlendType.NORMAL;
        public static BlendState BlendState;
        public static BlendState AlphaBlend;
        public static BlendState LightBlend;
        public static BlendState LLayerBlend;
        public static BlendState Normal;
        public static BlendState _BLEND_LIGHT;
        public static BlendState _BLEND_LIGHTINV;
        public static BlendState COLORFY;
        public static BlendState HIGHLIGHT;
        #endregion

        public static Timer _timer;
        static DXManager()               //DX管理
        {
            _timer = new Timer((o) => Helpers.LibraryHelper.Process(), null, 2000, 2000);

            AlphaBlend = CloneBlendState(BlendState.AlphaBlend);
            AlphaBlend.ColorSourceBlend = Blend.SourceAlpha;
            //AlphaBlend.AlphaSourceBlend = Blend.One;
            //AlphaBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;
            //AlphaBlend.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            BlendState = AlphaBlend;

            LightBlend = CloneBlendState(BlendState.AlphaBlend);
            LightBlend.ColorSourceBlend = Blend.SourceAlpha;
            LightBlend.ColorDestinationBlend = Blend.One;

            LLayerBlend = CloneBlendState(BlendState.AlphaBlend);
            LLayerBlend.ColorSourceBlend = Blend.DestinationColor;
            LLayerBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;
            LLayerBlend.AlphaDestinationBlend = Blend.InverseSourceAlpha;

            Normal = CloneBlendState(BlendState.AlphaBlend);
            Normal.ColorSourceBlend = Blend.InverseDestinationColor;
            Normal.ColorDestinationBlend = Blend.One;

            _BLEND_LIGHT = CloneBlendState(BlendState.AlphaBlend);
            _BLEND_LIGHT.ColorDestinationBlend = Blend.One;

            _BLEND_LIGHTINV = CloneBlendState(BlendState.AlphaBlend);
            _BLEND_LIGHTINV.ColorSourceBlend = Blend.One;
            _BLEND_LIGHTINV.ColorDestinationBlend = Blend.InverseSourceColor;

            COLORFY = CloneBlendState(BlendState.AlphaBlend);
            COLORFY.ColorSourceBlend = Blend.SourceAlpha;
            COLORFY.ColorDestinationBlend = Blend.One;

            HIGHLIGHT = CloneBlendState(BlendState.AlphaBlend);
            HIGHLIGHT.ColorSourceBlend = Blend.BlendFactor;
            HIGHLIGHT.ColorDestinationBlend = Blend.One;
        }

        public static BlendState CloneBlendState(BlendState CloneblendState)
        {
            BlendState blendState = new BlendState();
            if (CloneblendState == null)
            {
                return CloneblendState;
            }
            blendState.AlphaBlendFunction = CloneblendState.AlphaBlendFunction;
            blendState.AlphaDestinationBlend = CloneblendState.AlphaDestinationBlend;
            blendState.AlphaSourceBlend = CloneblendState.AlphaSourceBlend;
            blendState.ColorBlendFunction = CloneblendState.ColorBlendFunction;
            blendState.ColorDestinationBlend = CloneblendState.ColorDestinationBlend;
            blendState.ColorSourceBlend = CloneblendState.ColorSourceBlend;
            blendState.ColorWriteChannels = CloneblendState.ColorWriteChannels;
            blendState.BlendFactor = CloneblendState.BlendFactor;
            return blendState;
        }
        public static void Create(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)  //创建
        {
            Parameters = graphicsDevice.PresentationParameters;
            Parameters.IsFullScreen = true;
            Parameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            Device = new Device(graphicsDevice, spriteBatch, graphics);
            LoadTextures();
        }

        //private static void LoadPixelsShaders() //加载灰阶着色文件
        //{
        //    string text = ".\\Data\\Shaders\\grayscale.fx";

        //    if (File.Exists(text))
        //    {
        //        using (var graphicsStream = ShaderBytecode.CompileFromFile(text, "ps_main", "ps_2_0", ShaderFlags.None))
        //        {
        //            GrayScalePixelShader = new PixelShader(Device, graphicsStream);
        //        }
        //    }
        //}

        private static void LoadTextures() //加载纹理
        {
            // CleanUp();

            Sprite = new Sprite(Device);

            Line = new Line(Device) { Width = 1F };

            MainSurface = Device.GetBackBuffer(0, 0);
            CurrentSurface = MainSurface;
            Device.SetRenderTarget(0, MainSurface);


            PoisonTexture = new Texture(Device, 6, 6, 1, Usage.None, SurfaceFormat.Color, Pool.Managed);

            int[] data = new int[36];

            for (int y = 0; y < 6; y++)
                for (int x = 0; x < 6; x++)
                    data[y * 6 + x] = x == 0 || y == 0 || x == 5 || y == 5 ? -16777216 : -1;

            PoisonTexture.SetData(data);

            ScratchTexture = new Texture(Device, Parameters.BackBufferWidth, Parameters.BackBufferHeight, 1, Usage.RenderTarget, SurfaceFormat.Color, Pool.Default);
            ScratchSurface = ScratchTexture.GetSurfaceLevel(0);
            //ZoomScratchTexture = new Texture(Device, Parameters.BackBufferWidth, Parameters.BackBufferHeight, 1, Usage.RenderTarget, SurfaceFormat.Color, Pool.Default);
            //ZoomScratchSurface = ZoomScratchTexture.GetSurfaceLevel(0);
        }

        public static void SetLight(Texture2D texture)
        {
            LightTexture = new Texture(texture);
        }

        //public static void SetOpenImage(Texture2D texture)
        //{
        //    OpenGameImage = new Texture(texture);
        //}

        //public static void SetButtonImage(Texture2D texture)
        //{
        //    ButtonTexture = new Texture(texture);
        //}

        public static void SetColourPallete(Texture2D texture)
        {
            ColourPallete = new Texture(texture);
            PalleteData = new byte[texture.Width * texture.Height * 4];
            texture.GetData(PalleteData);
        }

        private static void CleanUp()  //清理
        {
            if (Sprite != null)
            {
                if (!Sprite.Disposed)
                    Sprite.Dispose();

                Sprite = null;
            }

            if (Line != null)
            {
                if (!Line.Disposed)
                    Line.Dispose();

                Line = null;
            }

            if (CurrentSurface != null)
            {
                if (!CurrentSurface.Disposed)
                    CurrentSurface.Dispose();

                CurrentSurface = null;
            }

            if (ScratchTexture != null)
            {
                if (!ScratchTexture.Disposed)
                    ScratchTexture.Dispose();

                ScratchTexture = null;
            }

            if (ScratchSurface != null)
            {
                if (!ScratchSurface.Disposed)
                    ScratchSurface.Dispose();

                ScratchSurface = null;
            }

            if (PoisonTexture != null)
            {
                if (!PoisonTexture.Disposed)
                    PoisonTexture.Dispose();

                PoisonTexture = null;
            }


            if (LightTexture != null)
            {
                if (!LightTexture.Disposed)
                    LightTexture.Dispose();

                LightTexture = null;
            }

            //if (GrayScalePixelShader != null)
            //{
            //    if (!GrayScalePixelShader.IsDisposed)
            //        GrayScalePixelShader.Dispose();

            //    GrayScalePixelShader = null;
            //}

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
                if (ControlList[i] == null)
                {
                    ControlList.RemoveAt(i);
                    continue;
                }
                ControlList[i].DisposeTexture();
            }
            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (TextureList[i] == null)
                {
                    TextureList.RemoveAt(i);
                    continue;
                }
                TextureList[i].DisposeTexture();
            }
            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (SoundList[i] == null)
                {
                    SoundList.RemoveAt(i);
                    continue;
                }
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
                if (ControlList[i] == null)
                {
                    ControlList.RemoveAt(i);
                    continue;
                }
                if (CEnvir.Now < ControlList[i].ExpireTime) continue;

                ControlList[i].DisposeTexture();
            }

            for (int i = TextureList.Count - 1; i >= 0; i--)
            {
                if (TextureList[i] == null)
                {
                    TextureList.RemoveAt(i);
                    continue;
                }
                if (CEnvir.Now < TextureList[i].ExpireTime) continue;

                TextureList[i].DisposeTexture();
            }

            for (int i = SoundList.Count - 1; i >= 0; i--)
            {
                if (SoundList[i] == null)
                {
                    SoundList.RemoveAt(i);
                    continue;
                }
                if (CEnvir.Now < SoundList[i].ExpireTime) continue;

                SoundList[i].DisposeSoundBuffer();
            }
        }

        public static void Unload()  //处理
        {
            CleanUp();

            if (Device != null)
            {
                if (!Device.Disposed)
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
            if (Opacity != opacity)
            {
                if (opacity > 1f)
                {
                    Opacity = 1f;
                }
                else if (opacity < 0f)
                {
                    Opacity = 0f;
                }
                else
                {
                    Opacity = opacity;
                }
            }
        }
        /// <summary>
        /// 设置混合模式
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rate"></param>
        public static void SetBlend(bool value, float rate = 1f, bool stateDeffered = false, BlendType blendtype = BlendType.NORMAL)
        {
            if (Blending == value && BlendRate == rate && BlendType == blendtype) return;

            Blending = value;
            BlendRate = rate;
            BlendType = blendtype;

            Sprite.End();

            if (Blending)
            {
                switch (blendtype)
                {
                    case BlendType.INVLIGHT:
                        //Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);
                        break;
                    case BlendType.COLORFY:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

                        Device.SetRenderState(COLORFY, rate);
                        break;
                    case BlendType.MASK:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
                        break;
                    case BlendType.EFFECTMASK:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.DestinationAlpha);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.One);
                        break;
                    case BlendType.HIGHLIGHT:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.BlendFactor);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

                        Device.SetRenderState(HIGHLIGHT, rate);
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
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

                        Device.SetRenderState(_BLEND_LIGHT, rate);
                        break;
                    case BlendType._BLEND_LIGHTINV:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.One);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceColor);

                        Device.SetRenderState(_BLEND_LIGHTINV, rate);
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
                        //Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseDestinationColor);
                        break;
                    case BlendType._BLEND_INVLIGHTINV:
                        //Device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //Device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
                        //Device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture | TextureArgument.Complement);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseDestinationColor);
                        break;
                    //case 6:
                    //    Device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    //    Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
                    //    Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);
                    //    break;
                    #endregion
                    default:
                        //Device.SetRenderState(RenderState.SourceBlend, Blend.InverseDestinationColor);
                        //Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

                        Device.SetRenderState(Normal, rate);
                        break;
                }

                if (stateDeffered)
                {
                    Sprite.Begin();
                }
                else
                {
                    Sprite.Begin(SpriteFlags.DoNotSaveState);
                }
                Device.SetRenderState(DXManager.AlphaBlend);
            }
            else
            {
                Sprite.Begin();
            }
        }
        /// <summary>
        /// 设置灰阶
        /// </summary>
        /// <param name="value"></param>
        public static void SetGrayscale(bool value)
        {
            if (value != GrayScale)
            {
                GrayScale = value;
            }
        }

        public static void ResetDevice()  //恢复设置
        {
            CleanUp();

            DeviceLost = true;

            if (CEnvir.Target.Width == 0 || CEnvir.Target.Height == 0) return;

            if (Parameters == null) return;

            Parameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            Device.Reset(Parameters);
            LoadTextures();
        }
        //public static void AttemptReset()  //尝试重置
        //{
        //    try
        //    {
        //        Result result = Device.TestCooperativeLevel();

        //        if (result.Code == ResultCode.DeviceLost.Code) return;

        //        if (result.Code == ResultCode.DeviceNotReset.Code)
        //        {
        //            CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
        //            ResetDevice();
        //            return;
        //        }

        //        if (result.Code != ResultCode.Success.Code) return;

        //        DeviceLost = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        CEnvir.SaveError(ex.ToString());
        //    }
        //}
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
        //public static void ToggleFullScreen()
        //{
        //    if (CEnvir.Target == null) return;

        //    Config.FullScreen = !Config.FullScreen;
        //    DXConfigWindow.ActiveConfig.FullScreenCheckBox.Checked = Config.FullScreen;

        //    CEnvir.Target.FormBorderStyle = (Config.FullScreen || Config.Borderless) ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
        //    CEnvir.Target.MaximizeBox = false;

        //    CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;
        //    ResetDevice();
        //    CEnvir.Target.ClientSize = DXControl.ActiveScene.Size;

        //    //CEnvir.Target.TopMost = Config.FullScreen;
        //}
        /// <summary>
        /// 设定分辨率
        /// </summary>
        /// <param name="size"></param>
        public static void SetResolution(Size size)
        {
            //if (CEnvir.Target == size) return;

            //Device.Clear(ClearFlags.Target, System.Drawing.Color.Black, 0, 0);
            //Device.Present();

            //CEnvir.Target = size;
            //CEnvir.Target.MaximizeBox = false;

            //ResetDevice();
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
