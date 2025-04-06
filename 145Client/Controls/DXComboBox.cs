using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 组合框控件
    /// </summary>
    public sealed class DXComboBox : DXControl
    {
        #region Properties
        public static List<DXComboBox> ComboBoxes = new List<DXComboBox>();

        #region NormalHeight
        /// <summary>
        /// 正常高度
        /// </summary>
        public int NormalHeight
        {
            get => _NormalHeight;
            set
            {
                if (_NormalHeight == value) return;

                int oldValue = _NormalHeight;
                _NormalHeight = value;

                OnNormalHeightChanged(oldValue, value);
            }
        }
        private int _NormalHeight;
        public event EventHandler<EventArgs> NormalHeightChanged;
        public void OnNormalHeightChanged(int oValue, int nValue)
        {
            NormalHeightChanged?.Invoke(this, EventArgs.Empty);

            if (!Showing)
                Size = new Size(Size.Width, NormalHeight);
        }
        #endregion

        #region DropDownHeight
        /// <summary>
        /// 下降高度
        /// </summary>
        public int DropDownHeight
        {
            get => _DropDownHeight;
            set
            {
                if (_DropDownHeight == value || value < NormalHeight) return;

                int oldValue = _DropDownHeight;
                _DropDownHeight = value;

                OnDropDownHeightChanged(oldValue, value);
            }
        }
        private int _DropDownHeight;
        public event EventHandler<EventArgs> DropDownHeightChanged;
        public void OnDropDownHeightChanged(int oValue, int nValue)
        {
            DropDownHeightChanged?.Invoke(this, EventArgs.Empty);

            if (Showing)
                Size = new Size(Size.Width, DropDownHeight);
        }
        #endregion

        #region Showing
        /// <summary>
        /// 显示
        /// </summary>
        public bool Showing
        {
            get => _Showing;
            set
            {
                if (_Showing == value) return;

                bool oldValue = _Showing;
                _Showing = value;

                OnShowingChanged(oldValue, value);
            }
        }
        private bool _Showing;
        public event EventHandler<EventArgs> ShowingChanged;
        public void OnShowingChanged(bool oValue, bool nValue)
        {
            Size = new Size(Size.Width, Showing ? Math.Min(ListBox.ScrollBar.MaxValue + NormalHeight + 2, DropDownHeight) : NormalHeight);

            if (ListBox != null)
                ListBox.Visible = Showing;

            ShowingChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region SelectedItem
        //选择编辑项目
        public object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (_SelectedItem == value) return;

                object oldValue = _SelectedItem;
                _SelectedItem = value;

                OnSelectedItemChanged(oldValue, value);
            }
        }
        private object _SelectedItem;
        public event EventHandler<EventArgs> SelectedItemChanged;
        public void OnSelectedItemChanged(object oValue, object nValue)
        {
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public const int DefaultNormalHeight = 16;   //默认正常高度

        public DXButton DownArrow;
        public DXLabel SelectedLabel;
        public DXListBox ListBox;
        /// <summary>
        /// 尺寸改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (DownArrow == null || SelectedLabel == null || ListBox == null) return;

            SelectedLabel.Size = new Size(Size.Width - 3 - DownArrow.Size.Height, NormalHeight);

            DownArrow.Location = new Point(Size.Width - DownArrow.Size.Width, (NormalHeight - DownArrow.Size.Height) / 2);

            ListBox.Location = new Point(DisplayArea.Location.X + SelectedLabel.Location.X, DisplayArea.Location.Y + NormalHeight + 2);
            ListBox.BringToFront();

            if (Showing)
                ListBox.Size = new Size(Size.Width, Size.Height - NormalHeight - 2);
        }

        #endregion

        /// <summary>
        /// 组合框控件
        /// </summary>
        public DXComboBox()
        {
            Sort = true;
            NormalHeight = DefaultNormalHeight;
            DropDownHeight = 123;
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            DownArrow = new DXButton  //下箭头
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 795,
                Parent = this,
            };
            DownArrow.MouseClick += DownArrow_MouseClick;

            SelectedLabel = new DXLabel  //选定的标签
            {
                Location = new Point(0, -1),
                AutoSize = false,
                Parent = this,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            ListBox = new DXListBox  //列表框
            {
                Parent = ActiveScene,
                BackColour = Color.Black,
                Sort = true,
                ScrollBar = { Change = 15 }
            };
            ListBox.selectedItemChanged += (o, e) =>
            {
                SelectedItem = ListBox.SelectedItem?.Item;
                SelectedLabel.Text = ListBox.SelectedItem?.Label.Text ?? string.Empty;
                Showing = false;
                SelectedLabel.ForeColour = Color.FromArgb(198, 166, 99);  //选定标签前景色
            };

            ComboBoxes.Add(this);
        }

        #region Methods
        /// <summary>
        /// 下箭头鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownArrow_MouseClick(object sender, MouseEventArgs e)
        {
            Showing = !Showing;
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ComboBoxes.Remove(this);

                _NormalHeight = 0;
                _DropDownHeight = 0;
                _Showing = false;
                _SelectedItem = null;

                NormalHeightChanged = null;
                DropDownHeightChanged = null;
                ShowingChanged = null;
                SelectedItemChanged = null;

                if (DownArrow != null)
                {
                    if (!DownArrow.IsDisposed)
                        DownArrow.Dispose();
                    DownArrow = null;
                }

                if (SelectedLabel != null)
                {
                    if (!SelectedLabel.IsDisposed)
                        SelectedLabel.Dispose();
                    SelectedLabel = null;
                }

                if (ListBox != null)
                {
                    if (!ListBox.IsDisposed)
                        ListBox.Dispose();
                    ListBox = null;
                }
            }
        }
        #endregion
    }
}
