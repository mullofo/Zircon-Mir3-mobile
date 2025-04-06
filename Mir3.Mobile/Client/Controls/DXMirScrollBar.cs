using Library;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using Point = System.Drawing.Point;
//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 自定滚动条控件
    /// </summary>
    public sealed class DXMirScrollBar : DXControl
    {
        #region Properties

        #region Value
        /// <summary>
        /// 值
        /// </summary>
        public int Value
        {
            get => _Value;
            set
            {
                if (_Value == value) return;

                int oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private int _Value;
        public event EventHandler<EventArgs> ValueChanged;
        public void OnValueChanged(int oValue, int nValue)
        {

            if (Value != Math.Max(MinValue, Math.Min(MaxValue - VisibleSize, Value)))
            {
                Value = Math.Max(MinValue, Math.Min(MaxValue - VisibleSize, Value));
                return;
            }

            UpdateScrollBar();

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region MaxValue
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxValue
        {
            get => _MaxValue;
            set
            {
                if (_MaxValue == value) return;

                int oldValue = _MaxValue;
                _MaxValue = value;

                OnMaxValueChanged(oldValue, value);
            }
        }
        private int _MaxValue;
        public event EventHandler<EventArgs> MaxValueChanged;
        public void OnMaxValueChanged(int oValue, int nValue)
        {
            PositionBar.Visible = MaxValue > VisibleSize;

            if (Value + VisibleSize > MaxValue)
                Value = MaxValue - VisibleSize;

            UpdateScrollBar();

            MaxValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region MinValue
        /// <summary>
        /// 最小值
        /// </summary>
        public int MinValue
        {
            get => _MinValue;
            set
            {
                if (_MinValue == value) return;

                int oldValue = _MinValue;
                _MinValue = value;

                OnMinValueChanged(oldValue, value);
            }
        }
        private int _MinValue;
        public event EventHandler<EventArgs> MinValueChanged;
        public void OnMinValueChanged(int oValue, int nValue)
        {
            UpdateScrollBar();

            MinValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region VisibleSize
        /// <summary>
        /// 可见大小
        /// </summary>
        public int VisibleSize
        {
            get => _VisibleSize;
            set
            {
                if (_VisibleSize == value) return;

                int oldValue = _VisibleSize;
                _VisibleSize = value;

                OnVisibleSizeChanged(oldValue, value);
            }
        }
        private int _VisibleSize;
        public event EventHandler<EventArgs> VisibleSizeChanged;
        public void OnVisibleSizeChanged(int oValue, int nValue)
        {
            PositionBar.Visible = MaxValue > VisibleSize;

            UpdateScrollBar();

            VisibleSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion


        private int ScrollHeight => Size.Height - 40;

        public int Change = 10;

        public DXButton UpButton, DownButton, PositionBar;
        /// <summary>
        /// 尺寸改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollHeight < 0)
                return;

            DownButton.Location = new Point(UpButton.Location.X, Size.Height - 13);

            UpdateScrollBar();
        }
        #endregion

        /// <summary>
        /// 自定滚动条
        /// </summary>
        public DXMirScrollBar()
        {
            Border = false;
            Opacity = 0F;

            UpButton = new DXButton  //向上按钮
            {
                Index = 3561,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(0, 2),
                Parent = this,
            };
            UpButton.MouseClick += (o, e) => Value -= Change;
            UpButton.MouseWheel += DoMouseWheel;

            DownButton = new DXButton  //向下按钮
            {
                Index = 3562,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(UpButton.Location.X, 0),
                Parent = this,
            };
            DownButton.MouseClick += (o, e) => Value += Change;
            DownButton.MouseWheel += DoMouseWheel;

            PositionBar = new DXButton  //位置栏
            {
                Index = 3560,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(UpButton.Location.X + 2, UpButton.Size.Height + 4), // | - Space - Button - Space - | - Space - Bar 
                Parent = this,
                Movable = true,
                Sound = SoundIndex.None,
                CanBePressed = false,
            };
            PositionBar.Moving += PositionBar_Moving;
            PositionBar.MouseWheel += DoMouseWheel;
            Size = new Size(16, 0);
        }

        #region Methods
        /// <summary>
        /// 更新滚动条
        /// </summary>
        private void UpdateScrollBar()
        {
            UpButton.Tag = Value > MinValue;
            DownButton.Tag = Value < MaxValue - VisibleSize;
            PositionBar.Tag = MaxValue - MinValue > VisibleSize;

            if (MaxValue - MinValue - VisibleSize != 0)
                PositionBar.Location = new Point(UpButton.Location.X + 2, 14 + (int)(ScrollHeight * (Value / (float)(MaxValue - MinValue - VisibleSize))));
        }

        /// <summary>
        /// 进行鼠标滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoMouseWheel(object sender, MouseEventArgs e)
        {
            //Value -= e.Delta / SystemInformation.MouseWheelScrollDelta * Change;
        }
        /// <summary>
        /// 位置栏移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionBar_Moving(object sender, MouseEventArgs e)
        {
            Value = (int)Math.Round((PositionBar.Location.Y - 16) * (MaxValue - MinValue - VisibleSize) / (float)ScrollHeight);

            if (MaxValue - MinValue - VisibleSize == 0) return;

            PositionBar.Location = new Point(UpButton.Location.X + 2, 14 + (int)(ScrollHeight * (Value / (float)(MaxValue - MinValue - VisibleSize))));
        }
        /// <summary>
        /// 鼠标按下时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Value = (int)Math.Round((e.Location.Y - DisplayArea.Top - 32) * (MaxValue - MinValue - VisibleSize) / (float)ScrollHeight);
        }
        /// <summary>
        /// 鼠标滚轮上
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            DoMouseWheel(this, e);
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Value = 0;
                ValueChanged = null;

                _MaxValue = 0;
                MaxValueChanged = null;

                _MinValue = 0;
                MinValueChanged = null;

                _VisibleSize = 0;
                VisibleSizeChanged = null;

                Change = 0;

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
                }

                if (PositionBar != null)
                {
                    if (!PositionBar.IsDisposed)
                        PositionBar.Dispose();

                    PositionBar = null;
                }
            }
        }
        #endregion
    }
}