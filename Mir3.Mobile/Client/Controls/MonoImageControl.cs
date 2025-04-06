using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using System;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 图像控制控件
    /// </summary>
    public class MonoImageControl : MonoControl
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

        /// <summary>
        /// 大小
        /// </summary>
        public override Size Size
        {
            get
            {
                if (Library != null && Index >= 0 && !FixedSize)
                    return Library.GetSize(Index);

                return base.Size;
            }
            set => base.Size = value;
        }

        /// <summary>
        /// 灰化效果
        /// </summary>
        public bool GrayScale { get; set; }

        #endregion

        /// <summary>
        /// 图像控制控件
        /// </summary>
        public MonoImageControl()
        {
            DrawImage = true;
            DrawTexture = false;
            Index = -1;
            ImageOpacity = 1F;
            ZoomRate = 1F;
            UI_Offset_X = 0;
        }

        #region Methods
        public override void Draw()
        {
            if (!IsVisible || DisplayArea.Width <= 0 || DisplayArea.Height <= 0) return;

            OnBeforeDraw();
            DrawControl();
            OnBeforeChildrenDraw();
            DrawChildControls();
            //DrawBorder();
            OnAfterDraw();
        }
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

            DXManager.Sprite.Draw(image.Image, Vector2.Zero, new Vector2(DisplayArea.Location.X, DisplayArea.Location.Y), IsEnabled ? ForeColour : System.Drawing.Color.FromArgb(75, 75, 75));

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
                _UseOffSet = false;

                BlendChanged = null;
                DrawImageChanged = null;
                FixedSizeChanged = null;
                ImageOpacityChanged = null;
                IndexChanged = null;
                LibraryFileChanged = null;
                UseOffSetChanged = null;
            }
        }
        #endregion
    }
}
