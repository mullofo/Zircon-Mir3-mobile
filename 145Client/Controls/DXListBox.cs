using Client.Extentions;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 列表框控件
    /// </summary>
    public class DXListBox : DXControl
    {
        #region Properties

        #region SelectedItem
        /// <summary>
        /// 选择编辑项目
        /// </summary>
        public DXListBoxItem SelectedItem
        {
            get => _SelectedItem;
            set
            {
                if (_SelectedItem == value) return;

                DXListBoxItem oldValue = _SelectedItem;
                _SelectedItem = value;

                OnselectedItemChanged(oldValue, value);
            }
        }
        private DXListBoxItem _SelectedItem;
        public event EventHandler<EventArgs> selectedItemChanged;
        public void OnselectedItemChanged(DXListBoxItem oValue, DXListBoxItem nValue)
        {
            if (oValue != null)
                oValue.Selected = false;


            if (nValue != null)
                nValue.Selected = true;


            selectedItemChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXVScrollBar ScrollBar;

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (ScrollBar == null) return;

            foreach (DXControl control in Controls)
                if (control is DXListBoxItem)
                    control.Size = new Size(Size.Width - ScrollBar.Size.Width - 1, control.Size.Height);

            ScrollBar.Size = new Size(14, Size.Height);
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width, 0);

            UpdateScrollBar();
        }
        #endregion

        /// <summary>
        /// 列表框容器
        /// </summary>
        public DXListBox()
        {
            Border = true;
            DrawTexture = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            ScrollBar = new DXVScrollBar
            {
                VisibleSize = Size.Height,
                Size = new Size(14, Size.Height),
                Parent = this,
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;

            ScrollBar.SetSkin(LibraryFile.GameInter, -1, -1, -1, 3560);

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods
        /// <summary>
        /// 鼠标点击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            SelectedItem = null;
        }

        /// <summary>
        /// 滚动条数值变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateItems();
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        public void UpdateItems()
        {
            foreach (DXControl control in Controls)
            {
                DXListBoxItem item = control as DXListBoxItem;

                item?.UpdateLocation();
            }
        }
        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            ScrollBar.VisibleSize = Size.Height;

            int height = 0;

            foreach (DXControl control in Controls)
            {
                if (!(control is DXListBoxItem)) continue;

                height += control.Size.Height;
            }

            ScrollBar.MaxValue = height;
        }
        /// <summary>
        /// 选择项目
        /// </summary>
        /// <param name="ob"></param>
        public void SelectItem(object ob)
        {
            foreach (DXControl control in Controls)
            {
                DXListBoxItem listItem = control as DXListBoxItem;
                if (listItem == null) continue;

                if (ob == null)
                {
                    if (listItem.Item != null) continue;

                    SelectedItem = listItem;
                    break;
                }
                if (!ob.Equals(listItem.Item)) continue;

                SelectedItem = listItem;

                break;
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_SelectedItem != null)
                {
                    if (!_SelectedItem.IsDisposed)
                        _SelectedItem.Dispose();
                    _SelectedItem = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();
                    ScrollBar = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 列表框项目控制
    /// </summary>
    public class DXListBoxItem : DXControl
    {
        #region Properties

        public bool isForeColourUpdate;

        #region BackColour_Select
        /// <summary>
        /// 背景颜色选择
        /// </summary>
        public Color BackColour_Select
        {
            get => _BackColour_Select;
            set
            {
                if (_BackColour_Select == value) return;

                Color oldValue = _BackColour_Select;
                _BackColour_Select = value;
                UpdateColours();
            }
        }
        private Color _BackColour_Select;
        #endregion

        #region BackColour_Select
        /// <summary>
        /// 背景颜色移动
        /// </summary>
        public Color BackColour_Move
        {
            get => _BackColour_Move;
            set
            {
                if (_BackColour_Move == value) return;

                Color oldValue = _BackColour_Move;
                _BackColour_Move = value;
                UpdateColours();
            }
        }
        private Color _BackColour_Move;
        #endregion

        #region Item
        /// <summary>
        /// 项目
        /// </summary>
        public object Item
        {
            get => _Item;
            set
            {
                if (_Item == value) return;

                object oldValue = _Item;
                _Item = value;

                OnItemChanged(oldValue, value);
            }
        }
        private object _Item;
        public event EventHandler<EventArgs> ItemChanged;
        public virtual void OnItemChanged(object oValue, object nValue)
        {
            ItemChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Selected
        /// <summary>
        /// 选择
        /// </summary>
        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public virtual void OnSelectedChanged(bool oValue, bool nValue)
        {
            UpdateColours();

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXLabel Label { get; private set; }
        /// <summary>
        /// 如果面板改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            DXListBox listBox = Parent as DXListBox;
            if (listBox == null) return;

            Size = new Size(Parent.Size.Width - listBox.ScrollBar.Size.Width - 5, Label.Size.Height);

            UpdateLocation();

            listBox.UpdateScrollBar();

            MouseWheel += listBox.ScrollBar.DoMouseWheel;
        }
        #endregion

        /// <summary>
        /// 列表框项目
        /// </summary>
        public DXListBoxItem()
        {
            isForeColourUpdate = true;
            DrawTexture = true;
            BackColour_Select = Color.FromArgb(128, 64, 64);
            BackColour_Move = Color.FromArgb(64, 32, 32);
            Label = new DXLabel
            {
                Parent = this,
                Text = "选择框".Lang(),
                IsControl = false
            };
        }

        #region Methods
        /// <summary>
        /// 鼠标点击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            DXListBox listBox = Parent as DXListBox;
            if (listBox == null) return;

            listBox.SelectedItem = this;
        }
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            UpdateColours();
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            UpdateColours();
        }
        /// <summary>
        /// 更新坐标
        /// </summary>
        public void UpdateLocation()
        {
            DXListBox listBox = Parent as DXListBox;

            if (listBox == null) return;

            int y = -listBox.ScrollBar.Value;

            foreach (DXControl control in Parent.Controls)
            {
                if (!(control is DXListBoxItem)) continue;
                if (control == this) break;
                y += control.Size.Height;
            }

            Location = new Point(0, y);
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        public void UpdateColours()
        {
            if (Selected)
            {
                if (isForeColourUpdate && null != Label) Label.ForeColour = Color.White;
                BackColour = BackColour_Select;
            }
            else if (MouseControl == this)
            {
                if (isForeColourUpdate && null != Label) Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = BackColour_Move;
            }
            else
            {
                if (isForeColourUpdate && null != Label) Label.ForeColour = Color.FromArgb(198, 166, 99);
                BackColour = Color.Empty;
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Item = null;
                _Selected = false;

                ItemChanged = null;
                SelectedChanged = null;

                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();

                    Label = null;
                }
            }
        }
        #endregion
    }
}
