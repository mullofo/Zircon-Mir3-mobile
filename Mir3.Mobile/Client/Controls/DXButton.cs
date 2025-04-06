using Client.Envir;
using Library;
using MonoGame.Extended;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    public class DXButton : DXImageControl
    {
        #region Properites

        #region HasFocus
        /// <summary>
        /// 有中心点
        /// </summary>
        public bool HasFocus
        {
            get => _HasFocus;
            set
            {
                if (_HasFocus == value) return;

                bool oldValue = _HasFocus;
                _HasFocus = value;

                OnHasFocusChanged(oldValue, value);
            }
        }
        private bool _HasFocus;
        public event EventHandler<EventArgs> HasFocusChanged;
        public virtual void OnHasFocusChanged(bool oValue, bool nValue)
        {
            UpdateDisplayArea();
            HasFocusChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Pressed
        /// <summary>
        /// 按下
        /// </summary>
        public bool Pressed
        {
            get => _Pressed;
            set
            {
                if (_Pressed == value) return;

                bool oldValue = _Pressed;
                _Pressed = value;

                OnPressedChanged(oldValue, value);
            }
        }
        private bool _Pressed;
        public event EventHandler<EventArgs> PressedChanged;
        public virtual void OnPressedChanged(bool oValue, bool nValue)
        {
            UpdateForeColour();

            PressedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region CanBePressed
        /// <summary>
        /// 可以按下
        /// </summary>
        public bool CanBePressed
        {
            get => _CanBePressed;
            set
            {
                if (_CanBePressed == value) return;

                bool oldValue = _CanBePressed;
                _CanBePressed = value;

                OnCanBePressedChanged(oldValue, value);
            }
        }
        private bool _CanBePressed;
        public event EventHandler<EventArgs> CanBePressedChanged;
        public virtual void OnCanBePressedChanged(bool oValue, bool nValue)
        {
            CanBePressedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region RightAligned
        /// <summary>
        /// 右对齐
        /// </summary>
        public bool RightAligned
        {
            get => _RightAligned;
            set
            {
                if (_RightAligned == value) return;

                bool oldValue = _RightAligned;
                _RightAligned = value;

                OnRightAlignedChanged(oldValue, value);
            }
        }
        private bool _RightAligned;
        public event EventHandler<EventArgs> RightAlignedChanged;
        public virtual void OnRightAlignedChanged(bool oValue, bool nValue)
        {
            RightAlignedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ButtonType
        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonType ButtonType
        {
            get => _ButtonType;
            set
            {
                if (_ButtonType == value) return;

                ButtonType oldValue = _ButtonType;
                _ButtonType = value;

                OnButtonTypeChanged(oldValue, value);
            }
        }
        private ButtonType _ButtonType;
        public event EventHandler<EventArgs> ButtonTypeChanged;
        public virtual void OnButtonTypeChanged(ButtonType oValue, ButtonType nValue)
        {
            if (Label == null) return;
            switch (nValue)
            {
                case ButtonType.SmallButton:
                    Label.Location = new Point(0, -1);
                    break;
                default:
                    Label.Location = new Point(0, 0);
                    break;
            }

            ButtonTypeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 按钮标签
        /// </summary>
        public DXLabel Label { get; private set; }

        /// <summary>
        /// 启用已更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsEnabledChanged(bool oValue, bool nValue)
        {
            base.OnIsEnabledChanged(oValue, nValue);

            UpdateForeColour();
            UpdateDisplayArea();
        }
        /// <summary>
        /// 显示区域已更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (Label == null) return;

            Label.Size = DisplayArea.Size;
        }
        #endregion

        /// <summary>
        /// 按钮控件
        /// </summary>
        public DXButton()
        {
            ForeColour = Color.White;
            Sound = SoundIndex.ButtonA;
            CanBePressed = true;
            //ForeColour = new Color4(0.85F, 0.85F, 0.85F, 1F).ToColor();

            Label = new DXLabel()
            {
                Location = new Point(0, -1),
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
                Parent = this,
            };
        }
        #region Methods
        /// <summary>
        /// 更新显示区域
        /// </summary>
        protected internal override void UpdateDisplayArea()
        {
            Rectangle area = new Rectangle(Location, Size);

            if (Parent != null)
                area.Offset(Parent.DisplayArea.Location);

            if (HasFocus && MouseControl == this && !Pressed && IsEnabled && CanBePressed) area.Y++;

            DisplayArea = area;
        }
        /// <summary>
        /// 绘制纹理
        /// </summary>
        protected override void DrawMirTexture()
        {
            Texture texture;

            if (Library == null)
            {
                DXManager.SetOpacity(Opacity);

                Surface oldSurface = DXManager.CurrentSurface;
                DXManager.SetSurface(DXManager.ScratchSurface);
                DXManager.Device.Clear(ClearFlags.Target, 0, 0, 0);

                switch (ButtonType)
                {
                    case ButtonType.Default:
                        DrawDefault();
                        break;
                    case ButtonType.SelectedTab:
                        DrawSelectedTab();
                        break;
                    case ButtonType.DeselectedTab:
                        DrawDeselectedTab();
                        break;
                    case ButtonType.SmallButton:
                        DrawSmallButton();
                        break;
                }

                DXManager.SetSurface(oldSurface);

                texture = DXManager.ScratchTexture;
            }
            else
            {
                MirImage image = Library.CreateImage(Index, ImageType.Image);
                if (image == null) return;
                texture = image.Image;
                image.ExpireTime = CEnvir.Now + Config.CacheDuration;
            }

            bool oldBlend = DXManager.Blending;
            float oldRate = DXManager.BlendRate;

            if (Blend)
                DXManager.SetBlend(true, ImageOpacity);
            else
                DXManager.SetOpacity(Opacity);

            PresentMirImage(texture, Parent, DisplayArea, ForeColour, this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            if (Blend)
                DXManager.SetBlend(oldBlend, oldRate);
            else
                DXManager.SetOpacity(1F);
        }
        /// <summary>
        /// 打开中心点
        /// </summary>
        public override void OnFocus()
        {
            base.OnFocus();

            HasFocus = true;
        }
        /// <summary>
        /// 失去中心点
        /// </summary>
        public override void OnLostFocus()
        {
            base.OnFocus();

            HasFocus = false;
        }
        /// <summary>
        /// 在鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            UpdateForeColour();
            UpdateDisplayArea();
        }
        /// <summary>
        /// 在鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            UpdateForeColour();
            UpdateDisplayArea();
        }
        /// <summary>
        /// 更新前景色
        /// </summary>
        public void UpdateForeColour()
        {
            //if (!IsEnabled)
            //    ForeColour = new Color4(0.2F, 0.2F, 0.2F).ToColor();
            //else
            //{
            //    ForeColour = MouseControl == this || Pressed ? new Color4(0.85F, 0.85F, 0.85F, 1F).ToColor() : new Color4(1F, 1F, 1F, 1F).ToColor();
            //}               
        }
        /// <summary>
        /// 绘制默认按钮
        /// </summary>
        private void DrawDefault()
        {
            Size s = InterfaceLibrary.GetSize(16);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(18);
            InterfaceLibrary.Draw(18, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            InterfaceLibrary.Draw(16, 0, 0, Color.White, false, 1F, ImageType.Image);

            s = InterfaceLibrary.GetSize(17);
            InterfaceLibrary.Draw(17, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);

        }
        /// <summary>
        /// 绘制选择的选项卡按钮
        /// </summary>
        private void DrawSelectedTab()
        {
            Size s = InterfaceLibrary.GetSize(22);
            InterfaceLibrary.Draw(22, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(24);
            InterfaceLibrary.Draw(24, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            s = InterfaceLibrary.GetSize(23);
            InterfaceLibrary.Draw(23, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }
        /// <summary>
        /// 绘制取消的选项卡按钮
        /// </summary>
        private void DrawDeselectedTab()
        {
            Size s = InterfaceLibrary.GetSize(19);
            InterfaceLibrary.Draw(19, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(21);
            InterfaceLibrary.Draw(21, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);

            s = InterfaceLibrary.GetSize(20);
            InterfaceLibrary.Draw(20, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }
        /// <summary>
        /// 绘制小按钮
        /// </summary>
        private void DrawSmallButton()
        {
            Size s = InterfaceLibrary.GetSize(41);
            InterfaceLibrary.Draw(41, 0, 0, Color.White, false, 1F, ImageType.Image);

            int x = s.Width;
            s = InterfaceLibrary.GetSize(43);
            InterfaceLibrary.Draw(43, x, 0, Color.White, new Rectangle(0, 0, Size.Width - x * 2, s.Height), 1f, ImageType.Image);


            s = InterfaceLibrary.GetSize(42);
            InterfaceLibrary.Draw(42, Size.Width - s.Width, 0, Color.White, false, 1F, ImageType.Image);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _HasFocus = false;
                _Pressed = false;
                _CanBePressed = false;
                _RightAligned = false;
                _ButtonType = 0;

                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();

                    Label = null;
                }

                HasFocusChanged = null;
                CanBePressedChanged = null;
                PressedChanged = null;
                RightAlignedChanged = null;
                ButtonTypeChanged = null;
            }
        }
        #endregion
    }
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum ButtonType
    {
        /// <summary>
        /// 按钮类型默认值
        /// </summary>
        Default,
        /// <summary>
        /// 按钮选中选项卡
        /// </summary>
        SelectedTab,
        /// <summary>
        /// 按钮取消选项卡
        /// </summary>
        DeselectedTab,
        /// <summary>
        /// 按钮类型小按钮
        /// </summary>
        SmallButton,
    }
}
