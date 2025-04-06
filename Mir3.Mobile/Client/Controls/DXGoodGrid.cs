using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 道具方格
    /// </summary>
    public sealed class DXGoodGrid : DXControl
    {
        public Action CellDbClick;

        DXGoodCell _selectedGoodCell;
        /// <summary>
        /// 选择单元格
        /// </summary>
        public DXGoodCell SelectedGoodCell
        {
            get => _selectedGoodCell;
            set
            {
                if (_selectedGoodCell == value) return;
                if (_selectedGoodCell != null)
                {
                    _selectedGoodCell.BackGround.Visible = false;
                }
                _selectedGoodCell = value;
                if (value != null)
                {
                    value.BackGround.Visible = true;
                }

            }
        }

        #region Properies

        #region GridSize

        /*
         *   StartSpace.Width      GridSpace.Width
         *             .Height  +-+--------+-+----------------+
         *                     _|  ________   ________    
         *                      | |        | |        |
         *                      | |        | |        |
         *                     _| |________| |________|
         * GridSpace.Height    _|  ________
         *                      | |        |
         *                      | |        |
         *                      | |________|
         *                      |
         * 
         */
        /// <summary>
        /// 每个格子间距
        /// </summary>
        public Size GridSpace;
        /// <summary>
        /// 格子与边框间距
        /// </summary>
        public Size StartSpace;
        /// <summary>
        /// 方格数量
        /// </summary>
        public Size GridSize
        {
            get => _GridSize;
            set
            {
                if (_GridSize == value) return;

                Size oldValue = _GridSize;
                _GridSize = value;

                OnGridSizeChanged(oldValue, value);
            }
        }
        private Size _GridSize;
        public event EventHandler<EventArgs> GridSizeChanged;
        public void OnGridSizeChanged(Size oValue, Size nValue)
        {

            Size = new Size((StartSpace.Width * 2) + ((DXItemCell.CellWidth - 1 + GridSpace.Width) * GridSize.Width) - GridSpace.Width + 1, (StartSpace.Height * 2) + ((DXItemCell.CellHeight - 1 + GridSpace.Height) * Math.Min(GridSize.Height, VisibleHeight)) - GridSpace.Height + 1);
            CreateGrid();

            GridSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ItemGrid

        /// <summary>
        /// 客户端用户道具项目 道具方格
        /// </summary>
        public ClientNPCGood[] ItemGrid
        {
            get => _ItemGrid;
            set
            {
                if (_ItemGrid == value) return;

                ClientNPCGood[] oldValue = _ItemGrid;
                _ItemGrid = value;

                OnItemGridChanged(oldValue, value);
            }
        }
        private ClientNPCGood[] _ItemGrid;
        public event EventHandler<EventArgs> ItemGridChanged;
        public void OnItemGridChanged(ClientNPCGood[] oValue, ClientNPCGood[] nValue)
        {
            ItemGridChanged?.Invoke(this, EventArgs.Empty);
            SelectedGoodCell = null;
            for (var i = 0; i < Consts.BackItemCount; i++)
            {
                if (nValue.Length > i)
                {
                    Grid[i].Good = nValue[i];
                }
                else
                {
                    Grid[i].Good = null;
                }

            }
        }
        #endregion


        #region ScrollValue

        /// <summary>
        /// 滚动值
        /// </summary>
        public int ScrollValue
        {
            get => _ScrollValue;
            set
            {
                if (_ScrollValue == value) return;

                int oldValue = _ScrollValue;
                _ScrollValue = value;

                OnScrollValueChanged(oldValue, value);
            }
        }
        private int _ScrollValue;
        public event EventHandler<EventArgs> ScrollValueChanged;
        public void OnScrollValueChanged(int oValue, int nValue)
        {
            UpdateGridDisplay();
            ScrollValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region VisibleHeight

        /// <summary>
        /// 可见高度
        /// </summary>
        public int VisibleHeight
        {
            get => _VisibleHeight;
            set
            {
                if (_VisibleHeight == value) return;

                int oldValue = _VisibleHeight;
                _VisibleHeight = value;

                OnVisibleHeightChanged(oldValue, value);
            }
        }
        private int _VisibleHeight;
        public event EventHandler<EventArgs> VisibleHeightChanged;
        public void OnVisibleHeightChanged(int oValue, int nValue)
        {
            Size = new Size((StartSpace.Width * 2) + ((DXItemCell.CellWidth - 1 + GridSpace.Width) * GridSize.Width - GridSpace.Width) + 1, (StartSpace.Height * 2) + ((DXItemCell.CellHeight - 1 + GridSpace.Height) * Math.Min(GridSize.Height, VisibleHeight)) - GridSpace.Height + 1);

            UpdateGridDisplay();
            VisibleHeightChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ImagePanel
        public DXImageControl ImagePanel;
        #endregion

        public DXGoodCell[] Grid;            //道具单元格


        public override void OnOpacityChanged(float oValue, float nValue)      //不透明度改变时
        {
            base.OnOpacityChanged(oValue, nValue);

            if (Grid == null) return;

            foreach (DXGoodCell cell in Grid)
                cell.Opacity = Opacity;
        }

        #endregion

        /// <summary>
        /// 道具方格
        /// </summary>
        public DXGoodGrid()
        {
            StartSpace = new Size(0, 0);
            GridSpace = new Size(0, 0);
            DrawTexture = true;                                               //纹理绘制
            //BackColour = Color.FromArgb(24, 12, 12);                        //背景色
            Border = true;                                                    //边界
            BorderColour = Color.FromArgb(99, 83, 50);                        //边框颜色
            Size = new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);     //尺寸=新的尺寸（道具单元格宽度，道具单元格高度）//默认尺寸就一个格子大小
            VisibleHeight = 1000;                                             //可见高度
            ImagePanel = new DXImageControl
            {
                Visible = false,
                Size = Size,
                Parent = this
            };

        }

        #region Methods

        /// <summary>
        /// 创建方格
        /// </summary>
        private void CreateGrid()
        {
            if (Grid != null)
                foreach (DXGoodCell cell in Grid)
                    cell.Dispose();

            Grid = new DXGoodCell[GridSize.Width * GridSize.Height];

            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    Grid[y * GridSize.Width + x] = new DXGoodCell(this)
                    {
                        Parent = this,
                        Location = new Point(x * (DXItemCell.CellWidth - 1 + GridSpace.Width) + StartSpace.Width, y * (DXItemCell.CellHeight - 1 + GridSpace.Height) + StartSpace.Height),
                        Slot = y * GridSize.Width + x
                    };
                }

            UpdateGridDisplay();
        }

        /// <summary>
        /// 更新方格显示
        /// </summary>
        public void UpdateGridDisplay()
        {
            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    DXGoodCell cell = Grid[y * GridSize.Width + x];

                    if (y < ScrollValue || y >= ScrollValue + VisibleHeight)
                    {
                        cell.Visible = false;
                        continue;
                    }
                    cell.Visible = true;

                    cell.Location = new Point(x * (DXItemCell.CellWidth - 1 + GridSpace.Width) + StartSpace.Width, (y - ScrollValue) * (DXItemCell.CellHeight - 1 + GridSpace.Height) + StartSpace.Height);
                }
        }

        /// <summary>
        /// 清理纹理
        /// </summary>
        protected override void OnClearTexture()
        {
            base.OnClearTexture();

            if (!Border || BorderInformation == null) return;

            //画整个大框线条
            DXManager.Line.Draw(BorderInformation, BorderColour);

            //画每个格子边框
            for (int y = 0; y < Math.Min(GridSize.Height, VisibleHeight); y++)
            {
                int gridSpaceHeight = y == 0 ? 0 : GridSpace.Height;
                for (int x = 0; x < GridSize.Width; x++)
                {
                    int gridSpaceWidth = x == 0 ? 0 : GridSpace.Width;
                    DXManager.Line.Draw(new[]
                    {
                        new Vector2(StartSpace.Width + (DXItemCell.CellWidth - 1) * x + gridSpaceWidth * x, StartSpace.Height + (DXItemCell.CellHeight - 1) * y + gridSpaceHeight * y),
                        new Vector2(StartSpace.Width + (DXItemCell.CellWidth - 1) * (x + 1) + gridSpaceWidth * x, StartSpace.Height + (DXItemCell.CellHeight - 1) * y + gridSpaceHeight * y),
                        new Vector2(StartSpace.Width + (DXItemCell.CellWidth - 1) * (x + 1) + gridSpaceWidth * x, StartSpace.Height + (DXItemCell.CellHeight - 1) * (y + 1) + gridSpaceHeight * y),
                        new Vector2(StartSpace.Width + (DXItemCell.CellWidth - 1) * x + gridSpaceWidth * x, StartSpace.Height + (DXItemCell.CellHeight - 1) * (y + 1) + gridSpaceHeight * y),
                        new Vector2(StartSpace.Width + (DXItemCell.CellWidth - 1) * x + gridSpaceWidth * x, StartSpace.Height + (DXItemCell.CellHeight - 1) * y + gridSpaceHeight * y),
                    }, BorderColour);
                }
            }
        }

        /// <summary>
        /// 更新边框信息
        /// </summary>
        protected internal override void UpdateBorderInformation()
        {
            BorderInformation = null;
            if (!Border || Size.Width == 0 || Size.Height == 0) return;
            //整个大框线条
            BorderInformation = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(Size.Width - 1, 0),
                new Vector2(Size.Width - 1, Size.Height - 1),
                new Vector2(0, Size.Height - 1),
                new Vector2(0, 0)
            };
        }

        /// <summary>
        /// 绘制边框
        /// </summary>
        protected override void DrawBorder() { }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _GridSize = Size.Empty;
                _ItemGrid = null;
                _ScrollValue = 0;
                _VisibleHeight = 0;

                GridSizeChanged = null;
                ItemGridChanged = null;
                ScrollValueChanged = null;
                VisibleHeightChanged = null;

                for (int i = 0; i < Grid.Length; i++)
                {
                    if (Grid[i] == null) continue;

                    if (!Grid[i].IsDisposed)
                        Grid[i].Dispose();
                    Grid[i] = null;
                }
                Grid = null;
            }
        }
        #endregion
    }

    public sealed class DXGoodCell : DXControl
    {



        public DXLabel Price;

        public DXItemCell ItemCell;

        public DXControl BackGround;
        #region Slot

        /// <summary>
        /// 槽 口
        /// </summary>
        public int Slot
        {
            get => _Slot;
            set
            {
                if (_Slot == value) return;

                int oldValue = _Slot;
                _Slot = value;

                OnSlotChanged(oldValue, value);
            }
        }
        private int _Slot;
        public event EventHandler<EventArgs> SlotChanged;
        public void OnSlotChanged(int oValue, int nValue)
        {
            SlotChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion Slot

        #region Good
        /// <summary>
        /// 商品
        /// </summary>
        public ClientNPCGood Good
        {
            get => _Good;
            set
            {
                if (_Good == value) return;

                ClientNPCGood oldValue = _Good;
                _Good = value;

                OnGoodChanged(oldValue, value);
            }
        }
        private ClientNPCGood _Good;
        public event EventHandler<EventArgs> GoodChanged;
        public void OnGoodChanged(ClientNPCGood oValue, ClientNPCGood nValue)
        {
            if (nValue?.UserItem != null)
            {
                ItemCell.Item = new ClientUserItem(nValue.UserItem, 1) { Flags = UserItemFlags.None };
                ItemCell.Tag = nValue.UserItem.Index;
                Price.Text = nValue.Cost.ToString();
                Price.Location = new Point(Size.Width - Price.Size.Width, Size.Height - Price.Size.Height);
            }
            else
            {
                ItemCell.Item = null;
                ItemCell.Tag = null;
                Price.Text = string.Empty;
                BackGround.Visible = false;
            }

            GoodChanged?.Invoke(this, EventArgs.Empty);
        }

        // 判断是否是回购的物品
        public bool IsBuybackCell => Good?.UserItem != null;
        #endregion

        DXGoodGrid _grid;
        public DXGoodCell(DXGoodGrid grid)
        {
            _grid = grid;
            Size = new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, 0), //(Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                //FixedBorderColour = true,
                ShowCountLabel = false,
                BorderColour = Color.FromArgb(198, 166, 99),
                Border = false,
            };
            ItemCell.MouseClick += ItemCell_MouseClick;
            ItemCell.MouseDoubleClick += ItemCell_MouseDoubleClick;
            Price = new DXLabel
            {
                ForeColour = Color.White,
                IsControl = false,
                Parent = this,
                PassThrough = true,
            };
            BackGround = new DXControl
            {
                Size = ItemCell.Size,
                Parent = ItemCell,
                Border = true,
                BorderSize = 1,
                BorderColour = Color.LightSkyBlue,
                IsControl = false,
                PassThrough = true,
                DrawTexture = true,
                Opacity = 0.3f,
                BackColour = Color.FromArgb(20, 48, 108, 56),
                Visible = false
            };
        }

        private void ItemCell_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ItemCell.Tag != null)
            {
                _grid.SelectedGoodCell = this;
                _grid.CellDbClick?.Invoke();
            }

        }

        private void ItemCell_MouseClick(object sender, MouseEventArgs e)
        {
            if (ItemCell.Tag != null)
            {
                _grid.SelectedGoodCell = this;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Price.TryDispose();
                ItemCell.TryDispose();
            }
        }
    }
}
