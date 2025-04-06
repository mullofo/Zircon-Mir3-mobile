using Client.Envir;
using Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Drawing;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Texture = MonoGame.Extended.Texture;
//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 道具方格
    /// </summary>
    public sealed class DXItemGrid : DXControl
    {
        #region Properies

        #region GridType

        /// <summary>
        /// 方格类型
        /// </summary>
        public GridType GridType
        {
            get => _GridType;
            set
            {
                if (_GridType == value) return;

                GridType oldValue = _GridType;
                _GridType = value;

                OnGridTypeChanged(oldValue, value);
            }
        }
        private GridType _GridType;
        public event EventHandler<EventArgs> GridTypeChanged;
        public void OnGridTypeChanged(GridType oValue, GridType nValue)  //网格类型更改时
        {
            foreach (DXItemCell cell in Grid)
                cell.GridType = GridType;

            GridTypeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

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
        public ClientUserItem[] ItemGrid
        {
            get => _ItemGrid;
            set
            {
                if (_ItemGrid == value) return;

                ClientUserItem[] oldValue = _ItemGrid;
                _ItemGrid = value;

                OnItemGridChanged(oldValue, value);
            }
        }
        private ClientUserItem[] _ItemGrid;
        public event EventHandler<EventArgs> ItemGridChanged;
        public void OnItemGridChanged(ClientUserItem[] oValue, ClientUserItem[] nValue)
        {
            ItemGridChanged?.Invoke(this, EventArgs.Empty);

            foreach (DXItemCell cell in Grid)
                cell.ItemGrid = ItemGrid;
        }
        #endregion

        #region Linked

        /// <summary>
        /// 道具方格链接
        /// </summary>
        public bool Linked
        {
            get => _Linked;
            set
            {
                if (_Linked == value) return;

                bool oldValue = _Linked;
                _Linked = value;

                OnLinkedChanged(oldValue, value);
            }
        }
        private bool _Linked;
        public event EventHandler<EventArgs> LinkedChanged;
        public void OnLinkedChanged(bool oValue, bool nValue)
        {
            foreach (DXItemCell cell in Grid)
                cell.Linked = Linked;

            LinkedChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region AllowLink

        /// <summary>
        /// 方格允许连接
        /// </summary>
        public bool AllowLink
        {
            get => _AllowLink;
            set
            {
                if (_AllowLink == value) return;

                bool oldValue = _AllowLink;
                _AllowLink = value;

                OnAllowLinkChanged(oldValue, value);
            }
        }
        private bool _AllowLink;
        public event EventHandler<EventArgs> AllowLinkChanged;
        public void OnAllowLinkChanged(bool oValue, bool nValue)
        {
            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.AllowLink = AllowLink;

            AllowLinkChanged?.Invoke(this, EventArgs.Empty);
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
            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.ReadOnly = ReadOnly;

            ReadOnlyChanged?.Invoke(this, EventArgs.Empty);
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

        public DXItemCell[] Grid;            //道具单元格

        public override void OnOpacityChanged(float oValue, float nValue)      //不透明度改变时
        {
            base.OnOpacityChanged(oValue, nValue);

            if (Grid == null) return;

            foreach (DXItemCell cell in Grid)
                cell.Opacity = Opacity;
        }

        #endregion

        /// <summary>
        /// 道具方格
        /// </summary>
        public DXItemGrid()
        {
            StartSpace = new Size(0, 0);
            GridSpace = new Size(0, 0);
            DrawTexture = true;                                               //纹理绘制
            //BackColour = Color.FromArgb(24, 12, 12);                        //背景色
            Border = true;                                                    //边界
            BorderColour = Color.FromArgb(99, 83, 50);                        //边框颜色
            Size = new Size(DXItemCell.CellWidth, DXItemCell.CellHeight);     //尺寸=新的尺寸（道具单元格宽度，道具单元格高度）//默认尺寸就一个格子大小
            AllowLink = true;                                                 //允许链接
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
                foreach (DXItemCell cell in Grid)
                    cell.Dispose();

            Grid = new DXItemCell[GridSize.Width * GridSize.Height];

            for (int y = 0; y < GridSize.Height; y++)
                for (int x = 0; x < GridSize.Width; x++)
                {
                    Grid[y * GridSize.Width + x] = new DXItemCell
                    {
                        Parent = this,
                        Location = new Point(x * (DXItemCell.CellWidth - 1 + GridSpace.Width) + StartSpace.Width, y * (DXItemCell.CellHeight - 1 + GridSpace.Height) + StartSpace.Height),
                        Slot = y * GridSize.Width + x,
                        HostGrid = this,
                        ItemGrid = ItemGrid,
                        GridType = GridType,
                        Linked = Linked,
                        AllowLink = AllowLink,
                        ReadOnly = ReadOnly,
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
                    DXItemCell cell = Grid[y * GridSize.Width + x];

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
        /// 创建纹理
        /// </summary>
        protected override void CreateTexture()
        {
            if (DisplayArea.Size.Width <= 0 || DisplayArea.Size.Height <= 0) return;
            if (ControlTexture == null || DisplayArea.Size != TextureSize)
            {
                DisposeTexture();
                TextureSize = DisplayArea.Size;
                ControlTexture = new Texture(DXManager.Device, TextureSize.Width, TextureSize.Height, 1, Usage.RenderTarget, SurfaceFormat.Color, Pool.Default);
                ControlSurface = ControlTexture.GetSurfaceLevel(0);
                DXManager.ControlList.Add(this);
            }

            Surface previous = DXManager.CurrentSurface;
            DXManager.SetSurface(ControlSurface);

            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0, 0);

            OnClearTexture();

            DXManager.SetSurface(previous);
            TextureValid = true;
            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }
        /// <summary>
        /// 清理纹理
        /// </summary>
        protected override void OnClearTexture()
        {
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
        /// 绘制控件
        /// </summary>
        protected override void DrawControl()
        {
            if (!DrawTexture) return;

            if (!TextureValid)
            {
                CreateTexture();

                if (!TextureValid) return;
            }

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);

            PresentMirImage(ControlTexture, Parent, DisplayArea, IsEnabled ? Color.White : Color.FromArgb(75, 75, 75), this, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

            DXManager.SetOpacity(oldOpacity);

            ExpireTime = CEnvir.Now + Config.CacheDuration;
        }

        /// <summary>
        /// 更新边框信息
        /// </summary>
        //protected internal override void UpdateBorderInformation()
        //{
        //    BorderInformation = null;
        //    if (!Border || Size.Width == 0 || Size.Height == 0) return;
        //    //整个大框线条
        //    BorderInformation = new Vector2[]
        //    {
        //        new Vector2(0, 0),
        //        new Vector2(Size.Width - 1, 0),
        //        new Vector2(Size.Width - 1, Size.Height - 1),
        //        new Vector2(0, Size.Height - 1),
        //        new Vector2(0, 0)
        //    };
        //}

        /// <summary>
        /// 绘制边框
        /// </summary>
        protected override void DrawBorder() { }

        /// <summary>
        /// 清除链接
        /// </summary>
        public void ClearLinks()
        {
            if (Grid != null)
                foreach (DXItemCell cell in Grid)
                {
                    if (cell.Link == null) continue;

                    if (cell.Link.GridType == GridType.TradeUser) continue;

                    cell.Link = null;
                }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _GridType = GridType.None;
                _GridSize = Size.Empty;
                _ItemGrid = null;
                _Linked = false;
                _AllowLink = false;
                _ReadOnly = false;
                _ScrollValue = 0;
                _VisibleHeight = 0;

                GridTypeChanged = null;
                GridSizeChanged = null;
                ItemGridChanged = null;
                LinkedChanged = null;
                AllowLinkChanged = null;
                ReadOnlyChanged = null;
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
}
