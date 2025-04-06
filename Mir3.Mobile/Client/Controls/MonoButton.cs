using Library;
using MonoGame.Extended;
using System;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    public class MonoButton : MonoImageControl
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
        #endregion

        /// <summary>
        /// 按钮控件
        /// </summary>
        public MonoButton()
        {
            ForeColour = Color.White;
            Sound = SoundIndex.ButtonA;
            CanBePressed = true;
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
            if (!IsEnabled)
                ForeColour = new Color4(0.2F, 0.2F, 0.2F).ToColor();
            else
            {
                ForeColour = MouseControl == this || Pressed ? new Color4(1F, 1F, 1F, 1F).ToColor() : new Color4(0.85F, 0.85F, 0.85F, 1F).ToColor();
            }
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

                HasFocusChanged = null;
                CanBePressedChanged = null;
                PressedChanged = null;
            }
        }
        #endregion
    }
}
