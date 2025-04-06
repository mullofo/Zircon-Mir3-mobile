using Client.Envir;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Client.Controls
{
    /// <summary>
    /// 列表视图控件
    /// </summary>
    public class DXListView : DXControl
    {
        public event EventHandler<EventArgs> ItemMouseEnter, ItemMouseLeave;
        public event EventHandler<MouseEventArgs> ItemMouseClick, ItemMouseDbClick, ItemMouseRButton;
        public int ItemHeight;
        public int Vspac, Hspac;//行列间距
        public Color ItemForeColour, ItemBorderColour, ItemSelectedBorderColour, ItemBackColour, ItemSelectedBackColour;
        public Color HeaderBorderColour, HeaderSelectedBorderColour, HeaderBackColour, HeaderSelectedBackColour;
        public bool ItemBorder, HeaderBorder, SelectedBorder, HasHeader, HasVScrollBar;
        public DXVScrollBar VScrollBar;
        public int VScrollValue;
        public DXControl Items;
        public DXControl Headers;
        private int _First, _Last;
        private DXControl _HeightLight;
        /// <summary>
        /// 高光
        /// </summary>
        public DXControl HeightLight
        {
            get => _HeightLight;
            set
            {
                if (_HeightLight == value) return;
                DXControl old = _HeightLight;
                _HeightLight = value;
                if (old?.Controls != null)
                {
                    foreach (DXControl c in old.Controls)
                    {
                        c.BackColour = ItemBackColour;
                        c.BorderColour = ItemBorderColour;
                    }
                    old.BorderColour = ItemBorderColour;
                    old.Border = false;
                }
                foreach (DXControl c in _HeightLight.Controls)
                {
                    c.BackColour = ItemSelectedBackColour;
                    c.BorderColour = ItemSelectedBorderColour;
                }
                _HeightLight.BorderColour = ItemSelectedBorderColour;
                _HeightLight.Border = true;
            }
        }
        /// <summary>
        /// 行数量
        /// </summary>
        public uint ItemCount
        {
            get => (uint)Items.Controls.Count;
        }
        /// <summary>
        /// 列计数
        /// </summary>
        public uint ColumnCount
        {
            get => (uint)Headers.Controls.Count;
        }

        /*
         * +-Headers-+-------------------+
         * | Header  |      Header       |
         * +---------+-------------------+
         * __Items________________________
         * | Item    |      subitem1     |
         * +---------+-------------------+
         * | Item    |      subitem1     |
         * +---------+-------------------+
         */

        /// <summary>
        /// 列表视图控件
        /// </summary>
        public DXListView()
        {
            ItemHeight = 25;
            HasHeader = true;
            HasVScrollBar = true;
            Hspac = 1;//3;
            Vspac = 0;//1;// 3;
            HeightLight = null;
            _First = 0;
            _Last = 0;
            ItemForeColour = Color.FromArgb(198, 166, 99);
            ItemBorderColour = Color.FromArgb(0x45, 0x38, 0x20);//Color.FromArgb(0x23, 0x1c, 0x0f);
            ItemBackColour = Color.Empty;//Color.FromArgb(0x1f, 0x19, 0x0c);
            ItemSelectedBorderColour = Color.FromArgb(0xA0, 0x7D, 0x16);//Color.FromArgb(0x45, 0x38, 0x20);
            ItemSelectedBackColour = Color.FromArgb(0x1f, 0x19, 0x0c);
            ItemBorder = true;

            HeaderBorderColour = Color.FromArgb(0xA0, 0x7D, 0x16);
            HeaderBackColour = Color.FromArgb(0x1f, 0x19, 0x0c);
            HeaderSelectedBorderColour = Color.Yellow;//Color.FromArgb(0x45, 0x38, 0x20);
            HeaderSelectedBackColour = Color.FromArgb(0x59, 0x44, 0x0C);
            HeaderBorder = true;

            SelectedBorder = false;
            Headers = new DXControl
            {
                Parent = this,
                Size = new Size(ItemHeight, ItemHeight),
                //Border = true,
                //BorderColour = HeaderBorderColour,
            };
            Items = new DXControl
            {
                Parent = this,
                Size = new Size(ItemHeight, ItemHeight),
                // Border = true,
                // BorderColour = HeaderBorderColour,
            };

            VScrollValue = 0;
            VScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(20, ItemHeight),
                Visible = true,
                Value = 0,
                MaxValue = 1,
                MinValue = 0,
                BorderColour = HeaderBorderColour,
            };
            VScrollBar.ValueChanged += (o, e) =>
            {
                UpdateItems();
            };

            MouseWheel += VScrollBar.DoMouseWheel;
            Headers.MouseWheel += VScrollBar.DoMouseWheel;
            Headers.MouseUp += OnItemMouseUp;
            Items.MouseWheel += VScrollBar.DoMouseWheel;
            Items.MouseUp += OnItemMouseUp;
        }

        /// <summary>
        /// 尺寸变化大小
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);
            UpdateViewRect();
        }
        /// <summary>
        /// 更新视图矩形
        /// </summary>
        public void UpdateViewRect()
        {
            Headers.Location = new Point(5, 5);
            Headers.Size = new Size(Size.Width - VScrollBar.Size.Width - 5 * 2, Headers.Size.Height);

            Items.Location = new Point(5, Headers.Location.Y + Headers.Size.Height + 3);
            Items.Size = new Size(Headers.Size.Width, Size.Height - Headers.Size.Height - 5 * 2 - 3);
            UpdateScrollBar();
            UpdateItems();
        }
        /// <summary>
        /// 绘制格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        protected void DrawGrid(int x, int y, int cx, int cy)
        {
            Vector2[] GridInformation = new[]
            {
                new Vector2(x, y),
                new Vector2(cx , y),
                new Vector2(cx, cy),
                new Vector2(x, cy),
                new Vector2(x, y)
            };
            {
                if (DXManager.Line.Width != 1)
                    DXManager.Line.Width = 1;

                Surface old = DXManager.CurrentSurface;
                DXManager.SetSurface(DXManager.ScratchSurface);

                DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);

                DXManager.Line.Draw(GridInformation, ItemBorderColour);

                DXManager.SetSurface(old);

                PresentMirImage(DXManager.ScratchTexture, Items.Parent, Rectangle.Inflate(Items.DisplayArea, 1, 1), Color.White, Items, uiOffsetX: UI_Offset_X);
            }
        }
        /// <summary>
        /// 绘制子控件
        /// </summary>
        protected override void DrawChildControls()
        {
            //foreach (DXControl control in Controls)
            //{
            //    control.Draw();
            //}
            foreach (DXControl line in Items.Controls)
            {
                line.Draw();
                //if (!ItemBorder) DrawGrid(Items.Location.X,line.Location.Y, Headers.Location.X+ Headers.Size.Width-2, line.Size.Height);
                //foreach (DXControl item in line.Controls)
                //{
                //    item.Draw();
                //}
            }

            if (HasHeader)
            {
                foreach (DXControl column in Headers.Controls)
                {
                    column.Draw();
                    //if (!ItemBorder) DrawGrid(column.Location.X,0, column.Location.X+column.Size.Width,Items.Size.Height);
                }
            }

            if (HeightLight != null)
            {
                HeightLight.Draw();
            }

            if (HasVScrollBar)
                VScrollBar.Draw();
        }
        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            if (ItemCount == 0 || ColumnCount == 0)
            {
                VScrollBar.Visible = false;
                VScrollBar.Value = 0;
                VScrollBar.MaxValue = 1;
                return;
            }

            VScrollBar.Location = new Point(Headers.Location.X + Headers.Size.Width, Headers.Location.Y + 1);
            VScrollBar.VisibleSize = Items.Size.Height;
            VScrollBar.Size = new Size(VScrollBar.Size.Width, Headers.Size.Height + Items.Size.Height + 3);
            VScrollBar.Visible = true;
            VScrollBar.Change = Items.Controls[0].Size.Height + Vspac;
            int Mode = VScrollBar.VisibleSize % VScrollBar.Change;
            if (Mode > 0) VScrollBar.VisibleSize -= Mode;
        }
        /// <summary>
        /// 更新行
        /// </summary>
        public void UpdateItems()
        {
            int X = 1;
            int Y = 0;
            int ScrollValue = VScrollBar.Value;
            int _Scrolled = -ScrollValue;
            if (ItemCount > 0)
            {
                _First = ScrollValue / VScrollBar.Change;
            }
            else
            {
                VScrollBar.Value = 0;
                HeightLight = null;
                _First = 0;
                _Last = 0;
                return;
            }

            for (int i = 0; i < _First; i++)
            {
                Items.Controls[i].Visible = false;
            }

            for (int i = _First; Y < Size.Height && i < ItemCount; i++)
            {
                DXControl Item = Items.Controls[i];
                Item.Location = new Point(0, Y);
                Item.Size = new Size(Items.Size.Width, Item.Size.Height);
                Y += Item.Size.Height + Vspac;
                Item.Visible = true;
                _Last = i;
            }
            for (int i = _Last + 1; i < ItemCount; i++)
            {
                Items.Controls[i].Visible = false;
            }

            VScrollBar.MaxValue = (int)(ItemCount * VScrollBar.Change);

            for (uint i = 0; i < ColumnCount; i++)
            {
                DXControl Head = Headers.Controls[(int)i];
                Head.Location = new Point(X, 1);
                Head.Size = new Size(Head.Size.Width, Headers.Size.Height - 2);
                for (int n = _First; n <= _Last && n < ItemCount; n++)
                {
                    DXControl Item = Items.Controls[n];
                    if (i < Item.Controls.Count)
                    {
                        DXControl sub = Item.Controls[(int)i];
                        sub.Location = new Point(Head.Location.X, 1);
                        sub.Size = new Size(Head.Size.Width, Item.Size.Height);
                    }
                }
                X += Head.Size.Width + Hspac;
            }
        }
        /// <summary>
        /// 行上鼠标悬停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnItemMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ItemMouseRButton?.Invoke(sender, e);
        }
        /// <summary>
        /// 行上鼠标进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnItemMouseEnter(object sender, EventArgs e)
        {
            DXControl control = sender as DXControl;
            if (null == control) return;

            if ((HeightLight == control.Parent))//高亮
            {
                if (SelectedBorder) control.BorderColour = ItemSelectedBorderColour;
                else control.BackColour = ItemSelectedBackColour;
            }
            else
            {
                if (SelectedBorder) control.BorderColour = ItemSelectedBorderColour;
                else control.BackColour = ItemSelectedBackColour;
            }
            ItemMouseEnter?.Invoke(sender, e);
        }
        /// <summary>
        /// 行上鼠标离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnItemMouseLeave(object sender, EventArgs e)
        {
            DXControl control = sender as DXControl;
            if (null == control) return;
            if (HeightLight == control.Parent)//高亮
            {
                if (SelectedBorder) control.BorderColour = ItemSelectedBorderColour;
                else control.BackColour = ItemSelectedBackColour;

            }
            else
            {
                if (SelectedBorder) control.BorderColour = ItemBorderColour;
                else control.BackColour = ItemBackColour;
            }
            ItemMouseLeave?.Invoke(sender, e);
        }
        /// <summary>
        /// 行上鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnItemMouseClick(object sender, MouseEventArgs e)
        {
            DXControl control = sender as DXControl;
            if (null == control) return;

            HeightLight = control.Parent;

            ItemMouseClick?.Invoke(sender, e);
        }
        /// <summary>
        /// 行上鼠标双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnItemMouseDbClick(object sender, MouseEventArgs e)
        {
            DXControl control = sender as DXControl;
            if (null == control) return;
            ItemMouseDbClick?.Invoke(sender, e);
        }

        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="col"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public uint InsertColumn(uint col, DXControl control)
        {
            control.Border = HeaderBorder;
            control.BorderColour = HeaderBorderColour;
            control.BackColour = HeaderBackColour;
            Headers.Size = new Size(Headers.Size.Width, control.Size.Height);
            UpdateViewRect();
            if (col >= ColumnCount)//直接放末尾
            {
                col = ColumnCount;
                control.Parent = Headers;
            }
            else
            {
                //插入前面需要调整位置
                control.Parent = Headers;
                Headers.Controls.Remove(control);
                Headers.Controls.Insert((int)col, control);
            }

            control.MouseClick += (o, e) =>
            {
                //这个排序有问题
                //Items.Sort(delegate (DXControl left, DXControl right)
                //{
                //    Point pos = (Point)control.Tag;
                //    if (left.Controls.Count < right.Controls.Count) return - 1;
                //    if (left.Controls.Count == right.Controls.Count) return 0;
                //    if()
                //    if (left.Controls[pos.X].Text != null && right.Controls[pos.X].Text != null)
                //    {
                //        int rt = left.Controls[pos.X].Text.CompareTo(right.Controls[pos.X].Text);
                //        switch(rt)
                //        {
                //            case 1:
                //            case -1:
                //                {
                //                    //交换location
                //                    for (int i = 0; i < left.Count; i++)
                //                    {
                //                        pos = right[i].Location;
                //                        right[i].Location = left[i].Location;
                //                        left[i].Location = pos;
                //                    }
                //                }
                //                break;
                //            case 0:
                //                break;
                //        }
                //        return rt;
                //    }
                //    return 0;
                //});
            };
            control.MouseEnter += (o, e) =>
            {
                control.BackColour = HeaderSelectedBackColour;
                //control.BorderColour = HeaderSelectedBorderColour;
            };
            control.MouseLeave += (o, e) =>
            {
                control.BackColour = HeaderBackColour;
                //control.BorderColour = HeaderBorderColour;
            };
            control.MouseUp += OnItemMouseUp;
            //更新滚动条
            UpdateScrollBar();

            //更新项目位置
            UpdateItems();
            return col;
        }
        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="col"></param>
        public void DeleteColumn(uint col)
        {
            if (col < ColumnCount)
            {
                Headers.Controls.RemoveAt((int)col);
                foreach (DXControl item in Items.Controls)
                {
                    item.Controls.RemoveAt((int)col);
                }
                UpdateScrollBar();
                UpdateItems();
            }
        }
        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public uint InsertItem(uint nItem, DXControl control)
        {
            //没有头 插入一个临时的
            if (ColumnCount == 0)
            {
                InsertColumn(0, new DXControl
                {
                    Text = "unnamed",
                });
            }

            //插入一个行
            DXControl Item = new DXControl
            {
                Size = control.Size,
            };
            Item.MouseClick += (o, e) => HeightLight = o as DXControl;
            Item.MouseWheel += VScrollBar.DoMouseWheel;
            if (nItem >= ItemCount)
            {
                nItem = ItemCount;
                Item.Parent = Items;
            }
            else
            {
                Item.Parent = Items;
                Items.Controls.Remove(Item);
                Items.Controls.Insert((int)nItem, Item);
            }

            //把子项插进去
            control.Parent = Item;
            control.ForeColour = ItemForeColour;
            control.Border = ItemBorder;
            control.BorderColour = ItemBorderColour;
            control.BackColour = ItemBackColour;
            control.MouseUp += OnItemMouseUp;
            control.MouseEnter += OnItemMouseEnter;
            control.MouseLeave += OnItemMouseLeave;
            control.MouseClick += OnItemMouseClick;
            control.MouseDoubleClick += OnItemMouseDbClick;

            //把后面的空余补上
            for (uint i = 1; i < ColumnCount; i++)
            {
                DXLabel Lab = new DXLabel
                {
                    Parent = control.Parent,
                    Text = i.ToString(),
                    Border = control.Border,
                    BorderColour = control.BorderColour,
                    AutoSize = false,
                };
                Lab.MouseWheel += VScrollBar.DoMouseWheel;
            }

            //更新滚动条
            control.MouseWheel += VScrollBar.DoMouseWheel;
            UpdateScrollBar();

            //UpdateItems();
            return nItem;
        }
        /// <summary>
        /// 设置行
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="subItem"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public DXControl SetItem(uint nItem, uint subItem, DXControl control)
        {
            if (nItem < ItemCount && subItem < ColumnCount)
            {
                DXControl Item = Items.Controls[(int)nItem];
                DXControl sub = Item.Controls[(int)subItem];

                //Item.Controls.RemoveAt((int)subItem);
                sub.Parent = null;
                control.Parent = Item;
                Item.Controls.Remove(control);
                Item.Controls.Insert((int)subItem, control);

                control.ForeColour = ItemForeColour;
                control.Border = ItemBorder;
                control.BorderColour = ItemBorderColour;
                control.BackColour = ItemBackColour;
                control.MouseUp += OnItemMouseUp;
                control.MouseWheel += VScrollBar.DoMouseWheel;
                control.MouseEnter += OnItemMouseEnter;
                control.MouseLeave += OnItemMouseLeave;
                control.MouseClick += OnItemMouseClick;
                control.MouseDoubleClick += OnItemMouseDbClick;

                sub.Dispose();
            }
            //UpdateItems();
            return control;
        }
        /// <summary>
        /// 获取行
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="nSubItem"></param>
        /// <returns></returns>
        public DXControl GetItem(uint nItem, uint nSubItem)
        {
            if (nItem < ItemCount && nSubItem < ColumnCount)
            {
                DXControl Item = Items.Controls[(int)nItem];
                DXControl sub = Item.Controls[(int)nSubItem];
                return sub;
            }
            return null;
        }
        /// <summary>
        /// 插入行
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public uint InsertItem(uint nItem, string text)
        {
            return InsertItem(nItem, new DXLabel
            {
                Text = text,
                AutoSize = false,
                Size = new Size(0, ItemHeight),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.Left,
            });
        }
        /// <summary>
        /// 设置行
        /// </summary>
        /// <param name="nItem"></param>
        /// <param name="nSubItem"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public DXControl SetItem(uint nItem, uint nSubItem, string text)
        {
            return SetItem(nItem, nSubItem, new DXLabel
            {
                Text = text,
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.Left,
            });
        }
        /// <summary>
        /// 插入列
        /// </summary>
        /// <param name="nColumn"></param>
        /// <param name="text"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="hint"></param>
        /// <returns></returns>
        public uint InsertColumn(uint nColumn, string text, int Width, int Height, string hint = null)
        {
            return InsertColumn(nColumn, new DXLabel
            {
                Text = text,
                AutoSize = false,
                Size = new Size(Width, Height),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.Left,
                Hint = hint,
            });
        }
        /// <summary>
        /// 按名称排序
        /// </summary>
        /// <param name="name"></param>
        public void SortByName(string name)
        {
            for (int i = 0; i < ItemCount; i++)
            {
                DXControl control = Items.Controls[i];
                if (control.Controls.Count == 0) continue;
                if (control.Controls[0].Text.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Items.Controls.RemoveAt(i);
                    Items.Controls.Insert(0, control);
                }
            }
            VScrollBar.Value = 0;
            UpdateItems();
            UpdateScrollBar();
        }
        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="nItem"></param>
        public void DeleteItem(uint nItem)
        {
            if (nItem < ItemCount)
            {
                //删除一行
                DXControl item = Items.Controls[(int)nItem];
                Items.Controls.RemoveAt((int)nItem);
                UpdateScrollBar();
                UpdateItems();
                item.Dispose();
            }
        }
        /// <summary>
        /// 移除所有
        /// </summary>
        public void RemoveAll()
        {
            //修复不能删除所有行
            uint count = ItemCount;
            for (uint i = 0; i < count; i++)
            {
                //删除一行
                DXControl item = Items.Controls[0];
                Items.Controls.RemoveAt(0);
                item.Dispose();
            }
            //清除已选行
            _HeightLight = null;

            UpdateScrollBar();
            UpdateItems();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (Items != null && !Items.IsDisposed)
            {
                Items.Dispose();
            }
        }
    }
}
