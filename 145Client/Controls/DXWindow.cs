using Client.Envir;
using Client.UserModels;
using Library;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Controls
{
    /// <summary>
    /// 客户端窗体控件
    /// </summary>
    public abstract class DXWindow : DXControl
    {
        #region Properties

        public static List<DXWindow> Windows = new List<DXWindow>();

        #region HasTopBorder
        /// <summary>
        /// 顶部边框
        /// </summary>
        public bool HasTopBorder
        {
            get => _HasTopBorder;
            set
            {
                if (_HasTopBorder == value) return;

                bool oldValue = _HasTopBorder;
                _HasTopBorder = value;

                OnHasTopBorderChanged(oldValue, value);
            }
        }
        private bool _HasTopBorder;
        public event EventHandler<EventArgs> HasTopBorderChanged;
        public virtual void OnHasTopBorderChanged(bool oValue, bool nValue)
        {
            HasTopBorderChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
        }
        #endregion

        #region HasTitle 
        /// <summary>
        /// 标题
        /// </summary>
        public bool HasTitle
        {
            get => _HasTitle;
            set
            {
                if (_HasTitle == value) return;

                bool oldValue = _HasTitle;
                _HasTitle = value;

                OnHasTitleChanged(oldValue, value);
            }
        }
        private bool _HasTitle;
        public event EventHandler<EventArgs> HasTitleChanged;
        public virtual void OnHasTitleChanged(bool oValue, bool nValue)
        {
            HasTitleChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
            if (TitleLabel == null) return;
            TitleLabel.Visible = HasTitle;
        }
        #endregion

        #region HasFooter
        /// <summary>
        /// 顶部页脚
        /// </summary>
        public bool HasFooter
        {
            get => _HasFooter;
            set
            {
                if (_HasFooter == value) return;

                bool oldValue = _HasFooter;
                _HasFooter = value;

                OnHasFooterChanged(oldValue, value);
            }
        }
        private bool _HasFooter;
        public event EventHandler<EventArgs> HasFooterChanged;
        public virtual void OnHasFooterChanged(bool oValue, bool nValue)
        {
            HasFooterChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
        }
        #endregion

        #region HasTopBorder

        public bool HasTransparentEdges  //是否有透明部分
        {
            get => _hasTransparentEdges;
            set
            {
                if (_hasTransparentEdges == value) return;

                bool oldValue = _hasTransparentEdges;
                _hasTransparentEdges = value;

                OnHasTransparentEdgesChanged(oldValue, value);
            }
        }
        private bool _hasTransparentEdges;
        public event EventHandler<EventArgs> HasTransparentEdgesChanged;
        public virtual void OnHasTransparentEdgesChanged(bool oValue, bool nValue)
        {
            HasTransparentEdgesChanged?.Invoke(this, EventArgs.Empty);

            UpdateClientArea();
        }

        #endregion

        #region ClientArea
        /// <summary>
        /// 客户端区域
        /// </summary>
        public Rectangle ClientArea
        {
            get => _ClientArea;
            set
            {
                if (_ClientArea == value) return;

                Rectangle oldValue = _ClientArea;
                _ClientArea = value;

                OnClientAreaChanged(oldValue, value);
            }
        }
        private Rectangle _ClientArea;
        public event EventHandler<EventArgs> ClientAreaChanged;
        public virtual void OnClientAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            ClientAreaChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 窗口的类型
        /// </summary>
        public abstract WindowType Type { get; }
        /// <summary>
        /// 是否自定义大小
        /// </summary>
        public abstract bool CustomSize { get; }
        /// <summary>
        /// 是否自动可见
        /// </summary>
        public abstract bool AutomaticVisibility { get; }
        /// <summary>
        /// 游戏素材库
        /// </summary>
        protected static MirLibrary GameInterLibrary, UI1Library;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton CloseButton { get; protected set; }
        /// <summary>
        /// 标题标签
        /// </summary>
        public DXLabel TitleLabel { get; protected set; }
        /// <summary>
        /// 窗口纹理
        /// </summary>
        public Texture WindowTexture;
        /// <summary>
        /// 窗口层
        /// </summary>
        public Surface WindowSurface;
        /// <summary>
        /// 窗口是否有效
        /// </summary>
        public bool WindowValid;
        /// <summary>
        /// 尺寸改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            UpdateClientArea();
            UpdateLocations();

            if (Settings != null && IsResizing)
            {
                Settings.Size = nValue;
                Settings.Location = Location;
            }
        }
        /// <summary>
        /// 容器改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            if (Parent == null) return;

            UpdateClientArea();

            UpdateLocations();
        }
        /// <summary>
        /// 位置改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (Settings != null && IsMoving)
                Settings.Location = nValue;
        }
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (IsVisible)
                BringToFront();

            if (Settings != null && AutomaticVisibility)
                Settings.Visible = nValue;
        }
        /// <summary>
        /// 窗口设置
        /// </summary>
        public WindowSetting Settings;
        #endregion

        #region Blend
        /// <summary>
        /// 是否混合
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

        #region LibraryFile
        public MirLibrary Library;
        /// <summary>
        /// 库文件调用
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

        #region DrawImage
        /// <summary>
        /// 绘制图像
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

        /// <summary>
        /// 客户端窗体控件
        /// </summary>
        protected DXWindow()
        {
            Windows.Add(this);

            DrawTexture = true;
            BackColour = Color.FromArgb(16, 8, 8);
            HasTitle = true;
            Movable = true;
            HasTopBorder = true;
            Sort = true;

            CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out GameInterLibrary);
            CEnvir.LibraryList.TryGetValue(LibraryFile.UI1, out UI1Library);

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            CloseButton.MouseClick += (o, e) => Visible = false;

            TitleLabel = new DXLabel       //标题标签
            {
                Text = "Window",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Visible = HasTitle,
                IsControl = false,
            };
            TitleLabel.SizeChanged += (o, e) => TitleLabel.Location = new Point((Size.Width - TitleLabel.Size.Width) / 2, 12);
        }

        #region Methods
        /// <summary>
        /// 分辨率改变时
        /// </summary>
        public override void ResolutionChanged()
        {
            Settings = null;

            base.ResolutionChanged();

            DisposeTexture();
        }
        /// <summary>
        /// 创建纹理
        /// </summary>
        protected override void CreateTexture()
        {
            base.CreateTexture();

            if (WindowTexture == null || DisplayArea.Size != TextureSize)
            {
                WindowTexture = new Texture(DXManager.Device, DXManager.Parameters.BackBufferWidth, DXManager.Parameters.BackBufferHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                WindowSurface = WindowTexture.GetSurfaceLevel(0);
                WindowValid = false;
            }
        }
        /// <summary>
        /// 处理纹理
        /// </summary>
        public override void DisposeTexture()
        {
            base.DisposeTexture();

            if (WindowTexture != null)
            {
                if (!WindowTexture.IsDisposed)
                    WindowTexture.Dispose();

                WindowTexture = null;
            }

            if (WindowSurface != null)
            {
                if (!WindowSurface.IsDisposed)
                    WindowSurface.Dispose();

                WindowSurface = null;
            }
        }

        /// <summary>
        /// 位置改变时
        /// </summary>
        private void UpdateLocations()
        {

            if (CloseButton != null)
                CloseButton.Location = new Point(DisplayArea.Width - CloseButton.Size.Width - 5, 5);

            if (TitleLabel != null)
                TitleLabel.Location = new Point((DisplayArea.Width - TitleLabel.Size.Width) / 2, 12);

        }

        /// <summary>
        /// 更新显示区域
        /// </summary>
        protected internal override void UpdateDisplayArea()
        {
            base.UpdateDisplayArea();

            WindowValid = false;
        }

        /// <summary>
        /// 按下键时
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (CloseButton.Visible != false)
                    {
                        CloseButton.InvokeMouseClick();
                        if (!Config.EscapeCloseAll)
                            e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// 更新客户端区域
        /// </summary>
        public void UpdateClientArea()
        {
            ClientArea = GetClientArea(Size);
        }

        /// <summary>
        /// 设置客户端大小
        /// </summary>
        /// <param name="clientSize"></param>
        public void SetClientSize(Size clientSize)
        {
            Size = GetSize(clientSize);
        }
        /// <summary>
        /// 获取尺寸
        /// </summary>
        /// <param name="clientSize"></param>
        /// <returns></returns>
        public Size GetSize(Size clientSize)
        {
            int w = 3 + 6 + 6 + 3; //Border Padding Padding Border
            int h = 6 + 6; //Padding Padding

            if (!HasTopBorder)
                h += NoFooterSize;
            else if (HasTitle)
                h += HeaderSize;
            else
                h += HeaderBarSize;

            if (!HasFooter)
                h += NoFooterSize;
            else
                h += FooterSize;

            return new Size(clientSize.Width + w, clientSize.Height + h);
        }
        /// <summary>
        /// 矩形 获取客户端指定大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Rectangle GetClientArea(Size size)
        {
            int x = 6 + 3;
            int y = 6;

            if (!HasTopBorder)  //如果 没有上边框
                y += NoFooterSize;
            else if (HasTitle)
                y += HeaderSize;
            else
                y += HeaderBarSize;

            int w = size.Width - x * 2;
            int h = size.Height - y - 6;

            if (!HasFooter)  //如果 没有页脚
                h -= NoFooterSize;
            else
                h -= FooterSize;

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (!IsVisible || Size.Width == 0 || Size.Height == 0) return;

            OnBeforeDraw();
            DrawControl();
            DrawWindow();
            OnBeforeChildrenDraw();
            DrawChildControls();
            DrawBorder();
            OnAfterDraw();
        }
        /// <summary>
        /// 绘制窗体
        /// </summary>
        protected void DrawWindow()
        {
            if (InterfaceLibrary == null) return;

            if (!WindowValid)
            {
                Surface oldSurface = DXManager.CurrentSurface;
                DXManager.SetSurface(WindowSurface);
                DXManager.Device.Clear(ClearFlags.Target, Color.Empty.ToRawColorBGRA(), 0, 0);

                DrawEdges();

                DXManager.SetSurface(oldSurface);
                WindowValid = true;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);
            PresentTexture(WindowTexture, Parent, DisplayArea, ForeColour, this);

            DXManager.SetOpacity(oldOpacity);
        }
        /// <summary>
        /// 绘制边框
        /// </summary>
        private void DrawEdges()
        {
            Size s;

            if (HasTopBorder)
            {
                s = InterfaceLibrary.GetSize(200);
                InterfaceLibrary.Draw(200, 0, 0, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);
            }
            else
            {
                s = InterfaceLibrary.GetSize(202);
                InterfaceLibrary.Draw(202, 0, 0, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);
            }

            int y = s.Height;

            s = InterfaceLibrary.GetSize(201);
            int x = s.Width;
            InterfaceLibrary.Draw(201, 0, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y), 1f, ImageType.Image);
            InterfaceLibrary.Draw(201, Size.Width - s.Width, y, Color.White, new Rectangle(0, 0, s.Width, Size.Height - y), 1F, ImageType.Image);

            if (HasTitle)
            {
                s = InterfaceLibrary.GetSize(203);
                InterfaceLibrary.Draw(203, x, y, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

                y += s.Height;

                InterfaceLibrary.Draw(204, 0, y - 3, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(205);
                InterfaceLibrary.Draw(205, Size.Width - s.Width, y - 3, Color.White, false, 1F, ImageType.Image);
            }

            if (HasTopBorder)
            {
                //2X Corner
                InterfaceLibrary.Draw(211, 0, 0, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(212);
                InterfaceLibrary.Draw(212, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
            }
            else
            {
                //2X Corner
                InterfaceLibrary.Draw(225, 0, 0, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(226);
                InterfaceLibrary.Draw(226, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
            }

            if (!HasFooter)
            {
                s = InterfaceLibrary.GetSize(202);
                InterfaceLibrary.Draw(202, 0, Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                s = InterfaceLibrary.GetSize(208);
                InterfaceLibrary.Draw(208, 0, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(209);
                InterfaceLibrary.Draw(209, Size.Width - s.Width, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);
            }
            else
            {
                s = InterfaceLibrary.GetSize(200);
                InterfaceLibrary.Draw(200, 0, Size.Height - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                y = s.Height;

                s = InterfaceLibrary.GetSize(10);
                InterfaceLibrary.Draw(10, x, Size.Height - s.Height - y, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

                y += s.Height;

                s = InterfaceLibrary.GetSize(202);
                InterfaceLibrary.Draw(202, 0, Size.Height - y - s.Height, Color.White, new Rectangle(0, 0, Size.Width, s.Height), 1f, ImageType.Image);

                y += s.Height;

                s = InterfaceLibrary.GetSize(206);
                InterfaceLibrary.Draw(206, 0, Size.Height - y - s.Height + 3, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(207);
                InterfaceLibrary.Draw(207, Size.Width - s.Width, Size.Height - y - s.Height + 3, Color.White, false, 1F, ImageType.Image);


                s = InterfaceLibrary.GetSize(213);
                InterfaceLibrary.Draw(213, 0, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);

                s = InterfaceLibrary.GetSize(214);
                InterfaceLibrary.Draw(214, Size.Width - s.Width, Size.Height - s.Height, Color.White, false, 1F, ImageType.Image);
            }
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        public void LoadSettings()
        {
            if (Type == WindowType.None || !CEnvir.Loaded) return;

            Settings = CEnvir.WindowSettings.Binding.FirstOrDefault(x => x.Resolution == Config.GameSize && x.Window == Type);

            if (Settings != null)
            {
                ApplySettings();
                return;
            }

            Settings = CEnvir.WindowSettings.CreateNewObject();
            Settings.Resolution = Config.GameSize;
            Settings.Window = Type;
            Settings.Size = Size;
            Settings.Visible = Visible;
            Settings.Location = Location;
        }
        /// <summary>
        /// 应用设置
        /// </summary>
        public virtual void ApplySettings()
        {
            if (Settings == null) return;

            Location = Settings.Location;

            if (AutomaticVisibility)
                Visible = Settings.Visible;

            if (CustomSize)
                Size = Settings.Size;
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
        /// 绘制纹理
        /// </summary>
        protected virtual void DrawMirTexture()
        {
            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            MirImage image = Library.CreateImage(Index, ImageType.Image);

            if (image?.Image == null) return;

            if (Blend)
                DXManager.SetBlend(true, ImageOpacity);
            else
                DXManager.SetOpacity(ImageOpacity);

            PresentTexture(image.Image, Parent, DisplayArea, IsEnabled ? ForeColour : Color.FromArgb(75, 75, 75), this);

            if (Blend)
                DXManager.SetBlend(oldBlend, oldRate);
            else
                DXManager.SetOpacity(1F);

            image.ExpireTime = Time.Now + Config.CacheDuration;
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _HasTopBorder = false;
                _HasTitle = false;
                _HasFooter = false;
                _ClientArea = Rectangle.Empty;

                if (CloseButton != null)
                {
                    if (!CloseButton.IsDisposed)
                        CloseButton.Dispose();
                    CloseButton = null;
                }

                if (TitleLabel != null)
                {
                    if (!TitleLabel.IsDisposed)
                        TitleLabel.Dispose();
                    TitleLabel = null;
                }

                HasTopBorderChanged = null;
                HasTitleChanged = null;
                HasFooterChanged = null;
                ClientAreaChanged = null;

                if (WindowTexture != null)
                {
                    if (!WindowTexture.IsDisposed)
                        WindowTexture.Dispose();

                    WindowTexture = null;
                }

                if (WindowSurface != null)
                {
                    if (!WindowSurface.IsDisposed)
                        WindowSurface.Dispose();

                    WindowSurface = null;
                }

                WindowValid = false;
                Settings = null;
                Windows.Remove(this);
            }
        }
        #endregion
    }
}
