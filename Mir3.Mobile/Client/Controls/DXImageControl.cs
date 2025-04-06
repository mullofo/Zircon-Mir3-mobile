using Client.Envir;
using Library;
using System;
using System.Drawing;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 图像控制控件
    /// </summary>
    public class DXImageControl : DXControl
    {
        #region Properties

        #region Blend
        /// <summary>
        /// 混合
        /// </summary>
        public bool Blend
        {
            get => _Blend;
            set
            {
                if (_Blend == value) return;

                bool oldValue = _Blend;
                _Blend = value;

                OnBlendChanged(oldValue, value);
            }
        }
        private bool _Blend;
        public event EventHandler<EventArgs> BlendChanged;
        public virtual void OnBlendChanged(bool oValue, bool nValue)
        {
            BlendChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DrawImage
        /// <summary>
        /// 绘制图片
        /// </summary>
        public bool DrawImage
        {
            get => _DrawImage;
            set
            {
                if (_DrawImage == value) return;

                bool oldValue = _DrawImage;
                _DrawImage = value;

                OnDrawImageChanged(oldValue, value);
            }
        }
        private bool _DrawImage;
        public event EventHandler<EventArgs> DrawImageChanged;
        public virtual void OnDrawImageChanged(bool oValue, bool nValue)
        {
            DrawImageChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region FixedSize
        /// <summary>
        /// 自定义尺寸
        /// </summary>
        public bool FixedSize
        {
            get => _FixedSize;
            set
            {
                if (_FixedSize == value) return;

                bool oldValue = _FixedSize;
                _FixedSize = value;

                OnFixedSizeChanged(oldValue, value);
            }
        }
        private bool _FixedSize;
        public event EventHandler<EventArgs> FixedSizeChanged;
        public virtual void OnFixedSizeChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            FixedSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ImageOpacity
        /// <summary>
        /// 图片透明度
        /// </summary>
        public float ImageOpacity
        {
            get => _ImageOpacity;
            set
            {
                if (_ImageOpacity == value) return;

                float oldValue = _ImageOpacity;
                _ImageOpacity = value;

                OnImageOpacityChanged(oldValue, value);
            }
        }
        private float _ImageOpacity;
        public event EventHandler<EventArgs> ImageOpacityChanged;
        public virtual void OnImageOpacityChanged(float oValue, float nValue)
        {
            ImageOpacityChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Index
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value) return;

                int oldValue = _Index;
                _Index = value;

                OnIndexChanged(oldValue, value);
            }
        }
        private int _Index;
        public event EventHandler<EventArgs> IndexChanged;
        public virtual void OnIndexChanged(int oValue, int nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            IndexChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region LibraryFile
        public MirLibrary Library;
        /// <summary>
        /// 库路径
        /// </summary>
        public LibraryFile LibraryFile
        {
            get => _LibraryFile;
            set
            {
                if (_LibraryFile == value) return;

                LibraryFile oldValue = _LibraryFile;
                _LibraryFile = value;

                OnLibraryFileChanged(oldValue, value);
            }
        }
        private LibraryFile _LibraryFile;
        public event EventHandler<EventArgs> LibraryFileChanged;
        public virtual void OnLibraryFileChanged(LibraryFile oValue, LibraryFile nValue)
        {
            CEnvir.LibraryList.TryGetValue(LibraryFile, out Library);

            TextureValid = false;
            UpdateDisplayArea();

            LibraryFileChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region PixelDetect
        /// <summary>
        /// 像素检测
        /// </summary>
        public bool PixelDetect
        {
            get => _PixelDetect;
            set
            {
                if (_PixelDetect == value) return;

                bool oldValue = _PixelDetect;
                _PixelDetect = value;

                OnPixelDetectChanged(oldValue, value);
            }
        }
        private bool _PixelDetect;
        public event EventHandler<EventArgs> PixelDetectChanged;
        public virtual void OnPixelDetectChanged(bool oValue, bool nValue)
        {
            PixelDetectChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region UseOffSet
        /// <summary>
        /// 使用偏移量
        /// </summary>
        public bool UseOffSet
        {
            get => _UseOffSet;
            set
            {
                if (_UseOffSet == value) return;

                bool oldValue = _UseOffSet;
                _UseOffSet = value;

                OnUseOffSetChanged(oldValue, value);
            }
        }
        private bool _UseOffSet;
        public event EventHandler<EventArgs> UseOffSetChanged;
        public virtual void OnUseOffSetChanged(bool oValue, bool nValue)
        {
            UpdateDisplayArea();
            UseOffSetChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region TilingMode
        /// <summary>
        /// 平铺模式
        /// </summary>
        public TilingMode TilingMode
        {
            get => _TilingMode;

            set
            {
                if (_TilingMode == value) return;

                _TilingMode = value;
                TextureValid = false;
                if (value != TilingMode.None)
                {
                    BeforeDraw -= DXImageControl_BeforeDraw;
                    BeforeDraw += DXImageControl_BeforeDraw;
                }
            }
        }

        private TilingMode _TilingMode = TilingMode.None;
        /// <summary>
        /// 图像控制 绘图前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DXImageControl_BeforeDraw(object sender, EventArgs e)
        {

            if (TilingMode == TilingMode.Horizontally || TilingMode == TilingMode.All)
            {
                //DXManager.Device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);//横向平铺
            }
            if (TilingMode == TilingMode.Vertically || TilingMode == TilingMode.All)
            {
                //DXManager.Device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);//纵向平铺
            }
        }
        #endregion

        /// <summary>
        /// 大小
        /// </summary>
        public override Size Size
        {
            get
            {
                if (Library != null && Index >= 0 && !FixedSize)
                    //return Functions.ScaleSize(Library.GetSize(Index), ZoomRate);
                    return Library.GetSize(Index);

                return base.Size;
            }
            set => base.Size = value;
        }

        /// <summary>
        /// 覆盖
        /// </summary>
        public bool Overlay;
        /// <summary>
        /// 覆盖颜色
        /// </summary>
        public Color OverlayColor;

        /// <summary>
        /// 灰化效果
        /// </summary>
        public bool GrayScale { get; set; }

        #region Zoom
        /// <summary>
        /// 是否缩放
        /// </summary>
        public bool Zoom
        {
            get => _Zoom;
            set
            {
                if (_Zoom == value) return;

                bool oldValue = _Zoom;
                _Zoom = value;

                OnZoomChanged(oldValue, value);
            }
        }
        private bool _Zoom;
        public event EventHandler<EventArgs> ZoomChanged;
        public virtual void OnZoomChanged(bool oValue, bool nValue)
        {
            TextureValid = false;
            UpdateDisplayArea();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ZoomSize
        /// <summary>
        /// 缩放尺寸
        /// </summary>
        /// 
        public Size ZoomSize
        {
            get => _ZoomSize;

            set
            {
                if (_ZoomSize == value) return;

                _ZoomSize = value;
                TextureValid = false;
            }
        }
        private Size _ZoomSize;
        #endregion

        #region Scale
        /// <summary>
        /// 缩放比例
        /// </summary>
        public float Scale
        {
            get
            {
                var scale = 1F;
                if (Library != null && Index >= 0)
                {
                    var size = Library.GetSize(Index);

                    if (ZoomSize.Width < size.Width)
                    {
                        scale = (float)ZoomSize.Width / size.Width;
                    }
                    if (ZoomSize.Height < size.Height)
                    {
                        var hscale = (float)ZoomSize.Height / size.Height;
                        scale = hscale < scale ? hscale : scale;
                    }
                }
                return scale;
            }
        }
        #endregion

        #region ScalingSize
        /// <summary>
        /// 缩放后真实尺寸
        /// </summary>
        public Size ScalingSize
        {
            get
            {
                var imageSize = Library.GetSize(Index);
                var size = new Size(Convert.ToInt32(imageSize.Width * Scale), Convert.ToInt32(imageSize.Height * Scale));
                return size;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// 图像控制控件
        /// </summary>
        public DXImageControl()
        {
            DrawImage = true;
            Index = -1;
            ImageOpacity = 1F;
            PixelDetect = true;
        }

        #region Methods
        /// <summary>
        /// 绘制控件
        /// </summary>
        protected override void DrawControl()
        {
            base.DrawControl();

            if (!DrawImage) return;

            DrawMirTexture();

        }
        /// <summary>
        /// 绘制MIR纹理
        /// </summary>
        protected virtual void DrawMirTexture()
        {
            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;
            bool oldGrayScale = DXManager.GrayScale;

            MirImage image = Library.CreateImage(Index, ImageType.Image);

            if (image?.Image == null) return;

            if (GrayScale)
            {
                DXManager.SetGrayscale(true);
            }

            if (Blend)
                DXManager.SetBlend(true, ImageOpacity);
            else
                DXManager.SetOpacity(ImageOpacity);


            PresentMirImage(image.Image, Parent, DisplayArea, IsEnabled ? ForeColour : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            if (Overlay)
            {
                Library.CreateImage(Index, ImageType.Overlay);
                PresentMirImage(image.Overlay, Parent, DisplayArea, IsEnabled ? OverlayColor : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            }

            if (Blend)
                DXManager.SetBlend(oldBlend, oldRate);
            else
                DXManager.SetOpacity(1F);

            if (GrayScale)
            {
                DXManager.SetGrayscale(oldGrayScale);
            }

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 更新显示区域
        /// </summary>
        protected internal override void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, Size);

            if (UseOffSet && Library != null)
                //area.Offset(Functions.ScalePoint(Library.GetOffSet(Index), ZoomRate));
                area.Offset(Library.GetOffSet(Index));

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            DisplayArea = area;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Blend = false;
                _DrawImage = false;
                _FixedSize = false;
                _ImageOpacity = 0F;
                _Index = -1;
                Library = null;
                _LibraryFile = LibraryFile.None;
                _PixelDetect = false;
                _UseOffSet = false;

                BlendChanged = null;
                DrawImageChanged = null;
                FixedSizeChanged = null;
                ImageOpacityChanged = null;
                IndexChanged = null;
                LibraryFileChanged = null;
                PixelDetectChanged = null;
                UseOffSetChanged = null;
            }
        }
        #endregion
    }

    /// <summary>
    /// 平铺模式类型
    /// </summary>
    public enum TilingMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 平铺模式类型横向
        /// </summary>
        Horizontally,
        /// <summary>
        /// 平铺模式类型纵向
        /// </summary>
        Vertically,
        /// <summary>
        /// 平铺模式类型全部
        /// </summary>
        All
    }
}
