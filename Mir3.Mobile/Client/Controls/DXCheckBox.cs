using Library;
using MonoGame.Extended.Input;
using System;
using System.Drawing;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 复选框控件
    /// </summary>
    public sealed class DXCheckBox : DXControl
    {
        #region Properites

        #region Checked
        /// <summary>
        /// 选中
        /// </summary>
        public bool Checked
        {
            get => _Checked;
            set
            {
                if (_Checked == value) return;

                bool oldValue = _Checked;
                _Checked = value;

                OnCheckedChanged(oldValue, value);
            }
        }
        private bool _Checked;
        public event EventHandler<EventArgs> CheckedChanged;
        public void OnCheckedChanged(bool oValue, bool nValue)
        {
            CheckedChanged?.Invoke(this, EventArgs.Empty);

            Box.Index = Checked ? 1003 : 1004;
        }
        #endregion

        #region ReadOnly
        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == value) return;

                bool oldValue = _ReadOnly;
                _ReadOnly = value;

                OnReadOnlyChanged(oldValue, value);
            }
        }
        private bool _ReadOnly;
        public event EventHandler<EventArgs> ReadOnlyChanged;
        public void OnReadOnlyChanged(bool oValue, bool nValue)
        {
            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 文本标签
        /// </summary>
        public DXLabel Label { get; private set; }
        /// <summary>
        /// 容器
        /// </summary>
        public DXImageControl Box { get; private set; }
        public bool bAlignRight = true;   //右对齐
        public bool AutoSize = true;   //自动大小
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            UpdateControl();
        }

        #endregion

        /// <summary>
        /// 复选框控件
        /// </summary>
        public DXCheckBox()
        {
            Label = new DXLabel
            {
                Parent = this,
                IsControl = false,
                Location = new Point(0, -1),
                //DrawFormat = TextFormatFlags.Top,
            };

            Label.DisplayAreaChanged += (o, e) =>  //显示区域改变
            {
                if (AutoSize)  //自动大小
                {
                    Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
                }
                if (bAlignRight)  //右对齐
                {
                    // xxxxxx口
                    if (AutoSize)
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Box.DisplayArea.Height);
                    }
                    else
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                    }
                    Box.Location = new Point(Label.DisplayArea.Width, (Size.Height - Label.Size.Height) / 2);
                }
                else
                {
                    // 口xxxxxx
                    if (AutoSize)
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Box.DisplayArea.Height);
                    }
                    else
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                    }
                    Label.Location = new Point(Box.DisplayArea.Width, (Size.Height - Label.Size.Height) / 2);
                }
            };

            Box = new DXImageControl
            {
                Location = new Point(Label.Size.Width + 2, 0),
                Index = 1004,
                LibraryFile = LibraryFile.PhoneUI,
                Parent = this,
                IsControl = false,
            };
            Box.DisplayAreaChanged += (o, e) =>
            {
                if (AutoSize)
                {
                    Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
                }
                if (bAlignRight)
                {
                    // xxxxxx口
                    if (AutoSize)
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Box.DisplayArea.Height);
                    }
                    else
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                    }
                    Box.Location = new Point(Label.DisplayArea.Width, (Size.Height - Box.Size.Height) / 2);
                }
                else
                {
                    // 口xxxxxx
                    if (AutoSize)
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Box.DisplayArea.Height);
                    }
                    else
                    {
                        Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                    }
                    Label.Location = new Point(Box.DisplayArea.Width, (Size.Height - Label.Size.Height) / 2);
                }
            };

            if (AutoSize)
            {
                Size = new Size(18, 18);
            }
        }

        #region Methods
        /// <summary>
        /// 更新控件
        /// </summary>
        private void UpdateControl()
        {
            if (Label == null) return;
            /*
            Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
            Label.Location = new Point(0, 0);
            Box.Location = new Point(Label.DisplayArea.Width, 0);
            */
            if (AutoSize)
            {
                Size = new Size(Label.DisplayArea.Width + Box.DisplayArea.Width, Box.DisplayArea.Height);
            }

            if (bAlignRight)
            {
                // xxxxxx口
                if (AutoSize)
                {
                    Label.Size = new Size(Label.Size.Width, Box.DisplayArea.Height);
                }
                else
                {
                    Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                }
                Label.Location = new Point(0, (Size.Height - Label.Size.Height) / 2);
                Box.Location = new Point(Label.DisplayArea.Width, (Size.Height - Box.Size.Height) / 2);
            }
            else
            {
                // 口 xxxxxx
                if (AutoSize)
                {
                    Label.Size = new Size(Label.Size.Width, Box.DisplayArea.Height);
                }
                else
                {
                    Label.Size = new Size(Size.Width - Box.DisplayArea.Width, Size.Height);
                }
                Box.Location = new Point(0, (Size.Height - Box.Size.Height) / 2);
                Label.Location = new Point(Box.DisplayArea.Width, (Size.Height - Label.Size.Height) / 2);
            }
        }
        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            base.OnMouseClick(e);

            if (ReadOnly) return;

            Checked = !Checked;
        }

        public override void OnTouchUp(TouchEventArgs e)
        {
            if (base.IsEnabled)
            {
                base.OnTouchUp(e);
                if (!ReadOnly)
                {
                    Checked = !Checked;
                }
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }

                if (Box != null)
                {
                    if (!Box.IsDisposed)
                        Box.Dispose();
                    Box = null;
                }

                _Checked = false;
                CheckedChanged = null;

                _ReadOnly = false;
                ReadOnlyChanged = null;
            }
        }
        #endregion
    }
}
