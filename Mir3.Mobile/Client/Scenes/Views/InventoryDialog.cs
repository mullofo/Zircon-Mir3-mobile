using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 角色背包功能
    /// </summary>
    public sealed class InventoryDialog : DXWindow
    {
        #region Properties

        #region InventoryType 

        /// <summary>
        /// 背包类型
        /// </summary>
        public InventoryType InventoryType
        {
            get
            {
                return _InventoryType;
            }
            set
            {
                if (_InventoryType == value) return;

                InventoryType oldValue = _InventoryType;
                _InventoryType = value;

                OnInventoryTypeChanged(oldValue, value);
            }
        }
        public void OnInventoryTypeChanged(InventoryType oValue, InventoryType nValue)
        {
            RefreshGrid.Index = nValue == InventoryType.Repair ? 1227 : 1964;   //如果是修理状态 刷新图标变修理图标
            if (DXItemCell.SelectedInventoryCell != null) DXItemCell.SelectedInventoryCell = null;
            ChangePriceDes();
        }
        InventoryType _InventoryType;

        /// <summary>
        /// 判断价格
        /// </summary>
        public void ChangePriceDes()
        {
            var type = InventoryType;
            var cell = DXItemCell.SelectedInventoryCell;

            if (type == InventoryType.Repair)
            {
                RefreshGrid.Hint = "修理".Lang();
                PriceLabel.Visible = true;
                GameGoldLabel.Visible = false;
                if (cell != null && cell.Item.Info.CanRepair)
                {
                    var sum = cell.Item.RepairCost(false);//SpecialCheckBox.Checked
                    PriceLabel.ForeColour = sum > MapObject.User.Gold ? Color.Red : Color.FromArgb(255, 150, 79);
                    PriceLabel.Text = sum.ToString("#,##0");
                }
                else
                {
                    PriceLabel.Text = "";
                }
            }
            else
            {
                RefreshGrid.Hint = "刷新".Lang();
                PriceLabel.Visible = false;
                GameGoldLabel.Visible = true;
                PriceLabel.ForeColour = Color.FromArgb(255, 150, 79);
                PriceLabel.Text = "";
            }

        }
        #endregion

        public DXImageControl InventoryBackGround, PatchBackGround, GoldUnit;   //背包主界面  元宝图片
        public DxMirButton RecoveryButton;
        public DXItemGrid Grid, PatchGrid;          //包裹定义  碎片包定义
        public DXLabel GoldLabel, GridLabel, PatchLabel, WeightLabel, GameGoldLabel, PriceLabel;  //金币标签 包裹标签 碎片包裹标签 重量标签  元宝标签
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (!IsVisible)
            {
                Grid.ClearLinks();
                //InventoryType = InventoryType.Inventory;
                //DXItemCell.SelectedInventoryCell = null;
            }

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override WindowType Type => WindowType.InventoryBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        public DXImageControl Image;      //负重图标
        public DXVScrollBar GridScrollBar, PatchGridScrollBar;    //碎片上下滚动条
        public DXImageControl BagWeightBar, RefreshPatch;  //经验条  碎片包裹整理按钮

        public DXButton GridButton, PatchButton, Close1Button, RefreshGrid;   //关闭按钮 刷新按钮
        public DateTime SortItemTime;  //公共 日期时间 排序道具时间
        public bool nowRefreshGrid = true; //公共 布尔 现在整理包裹 为 true

        #endregion

        /// <summary>
        /// 角色背包面板
        /// </summary>
        public InventoryDialog()
        {
            //TitleLabel.Text = "背包";
            //SetClientSize(new Size(275, 350));       //设置包裹大小
            HasTitle = false;                          //不显示标题
            HasFooter = false;                         //不显示页脚
            HasTopBorder = false;                      //不显示边框
            TitleLabel.Visible = false;                //标题标签不用
            IgnoreMoveBounds = true;
            Opacity = 0F;
            CloseButton.Visible = false;

            Size s;
            s = UI1Library.GetSize(1220);
            Size = s;
            Location = ClientArea.Location;

            InventoryBackGround = new DXImageControl  //包裹容器
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1220,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = true
            };

            PatchBackGround = new DXImageControl   //碎片包裹容器
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1220,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(241, 420),
                Hint = "关闭",
            };
            Close1Button.TouchUp += (o, e) => Visible = false;

            GridButton = new DXButton   //切换到碎片包裹按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1232,
                Location = new Point(223, 11),
                //Hint = "碎片包裹".Lang(),
                Visible = false,
            };
            GridButton.MouseClick += (o, e) =>
            {
                InventoryBackGround.Visible = false;
                PatchBackGround.Visible = true;
                GridButton.Visible = false;
                PatchButton.Visible = true;
            };

            PatchButton = new DXButton   //切换到包裹按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1232,
                Location = new Point(223, 11),
                Hint = "包裹".Lang(),
                Visible = false,
            };
            PatchButton.MouseClick += (o, e) =>
            {
                InventoryBackGround.Visible = true;
                PatchBackGround.Visible = false;
                GridButton.Visible = true;
                PatchButton.Visible = false;
            };

            GridLabel = new DXLabel          //标签
            {
                Parent = InventoryBackGround,
                Location = new Point(15, 8),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.FromArgb(200, 200, 255),
                Text = "[" + "包裹".Lang() + "]",
                Size = new Size(70, 30),
            };

            WeightLabel = new DXLabel          //负重标签
            {
                Parent = InventoryBackGround,
                Location = new Point(100, 17),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Text = "0",
                Size = new Size(ClientArea.Width - 91, 30),
            };

            Grid = new DXItemGrid                 //包裹大小位置等定义
            {
                GridSize = new Size(6, 8),
                Parent = InventoryBackGround,
                Location = new Point(17, 58),
                ItemGrid = GameScene.Game.Inventory,
                GridType = GridType.Inventory,
                Border = false,
            };
            Grid.FreeDrag += DoFreeDrag;
            foreach (DXItemCell cell in Grid.Grid)
                cell.FreeDrag += DoFreeDrag;

            PatchLabel = new DXLabel          //碎片包裹标签
            {
                Parent = PatchBackGround,
                Location = new Point(15, 8),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.FromArgb(200, 200, 255),
                Text = "[" + "碎片包裹".Lang() + "]",
                Size = new Size(70, 30),
            };

            PatchGrid = new DXItemGrid           //碎片包裹
            {
                GridSize = new Size(6, 8),
                Parent = PatchBackGround,
                Location = new Point(17, 58),
                GridType = GridType.PatchGrid,
                ItemGrid = GameScene.Game.PatchGrid,
                VisibleHeight = 8,
                Border = false,
            };
            PatchGrid.GridSizeChanged += PatchGrid_GridSizeChanged;

            PatchGridScrollBar = new DXVScrollBar          //碎片包上下滚动条
            {
                Parent = PatchBackGround,
                Size = new Size(25, 327),
                Location = new Point(ClientArea.Right - 23, 46),
                VisibleSize = 8,                     //可见尺寸为8格
                Change = 1,                          //改变为1格
            };
            PatchGridScrollBar.ValueChanged += PatchGridScrollBar_ValueChanged;   //滚动值变化

            //为滚动条自定义皮肤 -1为不设置
            PatchGridScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            foreach (DXItemCell cell in PatchGrid.Grid)
                cell.MouseWheel += PatchGridScrollBar.DoMouseWheel;

            GoldLabel = new DXLabel            //金币数值标签
            {
                AutoSize = false,
                ForeColour = Color.FromArgb(255, 250, 180),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 55, ClientArea.Bottom - 57),
                Font = new Font("MS Sans Serif", CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "0",
                Size = new Size(ClientArea.Width - 160, 20),
                Sound = SoundIndex.GoldPickUp
            };
            GoldLabel.MouseClick += GoldLabel_MouseClick;   //可以通过鼠标操作金币

            GameGoldLabel = new DXLabel                //元宝数值标签
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 45, ClientArea.Bottom - 37),
                Text = "0",
                Size = new Size(ClientArea.Width - 160, 20),
            };

            //售（修理）价格
            PriceLabel = new DXLabel
            {
                AutoSize = false,
                Font = GoldLabel.Font,
                ForeColour = Color.FromArgb(255, 150, 79),
                DrawFormat = TextFormatFlags.Right,
                Parent = this,
                Location = new Point(ClientArea.Left + 45, ClientArea.Bottom - 37),
                Text = "",
                Size = new Size(100, 20),
                IsControl = false,
                Visible = false
            };

            RefreshGrid = new DXButton         //整理包裹按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1964,
                FixedSize = false,
                Parent = this,
                Location = new Point(200, 398),
                Hint = "刷新".Lang(),
            };
            RefreshGrid.MouseClick += RefreshGrid_MouseClick;

            #region init  参数
            InventoryType = InventoryType.Inventory;
            #endregion
        }

        #region Methods 
        /// <summary>
        /// 鼠标点击整理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (InventoryType == InventoryType.Inventory || InventoryType == InventoryType.Sell)
            {
                RefreshGrid.Enabled = false;
                CEnvir.Enqueue(new C.InventoryRefresh
                {
                    GridType = !PatchButton.Visible ? GridType.Inventory : GridType.PatchGrid
                });
            }
            else if (InventoryType == InventoryType.Repair && DXItemCell.SelectedInventoryCell != null)
            {
                if (GameScene.Game.Observer) return;

                var cell = DXItemCell.SelectedInventoryCell;

                if (!DXItemCell.SelectedInventoryCell.Item.Info.CanRepair)
                {
                    GameScene.Game.ReceiveChat($"Inventory.CanRepair".Lang(cell.Item.Info.Lang(p => p.ItemName)), MessageType.Hint);
                    return;
                }
                if (cell.Item.RepairCost(false) == 0)
                {
                    var box = new DXConfirmWindow("没有必要修理".Lang());
                    return;
                }
                List<CellLinkInfo> links = new List<CellLinkInfo>();

                cell.Locked = true;

                links.Add(new CellLinkInfo { Count = cell.Item.Count, GridType = cell.GridType, Slot = cell.Slot });

                //TODO SpecialCheckBox GuildCheckBox
                CEnvir.Enqueue(new C.NPCRepair { Links = links, Special = false, GuildFunds = false });

                DXItemCell.SelectedInventoryCell = null;
            }
        }


        /// <summary>
        /// 金币拖动处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
                GameScene.Game.GoldPickedUp = !GameScene.Game.GoldPickedUp && MapObject.User.Gold > 0;
        }
        #endregion

        /// <summary>
        /// 刷新碎片包裹
        /// </summary>
        public void RefreshPatchGrid()
        {
            PatchGrid.GridSize = new Size(6, Math.Max(8, (int)Math.Ceiling(GameScene.Game.PatchGridSize / (float)6)));

            PatchGridScrollBar.MaxValue = PatchGrid.GridSize.Height;

            ApplyPatchGridFilter();
        }
        /// <summary>
        /// 碎片包裹过滤
        /// </summary>
        public void ApplyPatchGridFilter()
        {
            foreach (DXItemCell cell in PatchGrid.Grid)
                FilterCell(cell);
        }
        /// <summary>
        /// 过滤单元格
        /// </summary>
        /// <param name="cell"></param>
        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= GameScene.Game.PatchGridSize)
            {
                cell.Enabled = false;
                return;
            }
            cell.Enabled = true;
        }
        /// <summary>
        /// 碎片包裹容量大小变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PatchGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in PatchGrid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);

            foreach (DXItemCell cell in PatchGrid.Grid)
                cell.MouseWheel += PatchGridScrollBar.DoMouseWheel;
        }
        /// <summary>
        /// 碎片包裹滚动条更改值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PatchGridScrollBar_ValueChanged(object sender, EventArgs e)
        {
            PatchGrid.ScrollValue = PatchGridScrollBar.Value;
        }

        /// <summary>
        /// 滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DoFreeDrag(object sender, TouchEventArgs e)
        {
            if (GameScene.Game.SelectedCell != null) return;

            int value = (int)e.Delta.Y;
            Grid.ScrollValue -= value / 10;

            if (Grid.ScrollValue < 0)
                Grid.ScrollValue = 0;

            if (Grid.ScrollValue >= Grid.GridSize.Height)
                Grid.ScrollValue = Grid.GridSize.Height - 1;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)   //排列处理
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)                     //包裹
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (PatchGrid != null)               //碎片包裹
                {
                    if (!PatchGrid.IsDisposed)
                        PatchGrid.Dispose();

                    PatchGrid = null;
                }

                if (GoldLabel != null)               //金币
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (GameGoldLabel != null)          //元宝
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (WeightLabel != null)            //负重
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (BagWeightBar != null)            //负重经验条
                {
                    if (!BagWeightBar.IsDisposed)
                        BagWeightBar.Dispose();

                    BagWeightBar = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (PatchGridScrollBar != null)          //碎片包裹滚动条
                {
                    if (!PatchGridScrollBar.IsDisposed)
                        PatchGridScrollBar.Dispose();

                    PatchGridScrollBar = null;
                }
            }
        }
        #endregion
    }

    public enum InventoryType
    {
        /// <summary>
        /// 背包
        /// </summary>
        Inventory,
        /// <summary>
        /// 普通单个修理
        /// </summary>
        Repair,
        /// <summary>
        /// 特殊修理
        /// </summary>
        SpecialRepair,
        /// <summary>
        /// 购买界面出售
        /// </summary>
        BuySell,
        /// <summary>
        /// 出售
        /// </summary>
        Sell,
        /// <summary>
        /// 一键出售
        /// </summary>
        RootSell,
    }
}