using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 仓库功能
    /// </summary>
    public sealed class StorageDialog : DXWindow
    {
        #region Properties
        /// <summary>
        /// 道具格子
        /// </summary>
        public DXItemGrid Grid;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (GameScene.Game.InventoryBox == null) return;

            if (IsVisible)
                GameScene.Game.InventoryBox.Visible = true;

            if (!IsVisible)
                Grid.ClearLinks();
        }

        public override WindowType Type => WindowType.StorageBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public DXLabel StorageTitleLabel;
        public DXImageControl StorageGround;
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXButton ClearButton, Close1Button;
        public DXVScrollBar StorageScrollBar;
        //public ClientUserItem[] GuildStorage = new ClientUserItem[1000];  //没起作用？？？

        /// <summary>
        /// 仓库界面
        /// </summary>
        public StorageDialog()
        {
            //TitleLabel.Text = "仓库";
            HasTitle = false;  //字幕标题不显示
            HasFooter = false;  //不显示页脚
            HasTopBorder = false; //不显示上边框
            TitleLabel.Visible = false; //不显示标题
            CloseButton.Visible = false; //不显示关闭按钮            
            AllowResize = false; //不允许调整大小
            BackColour = Color.FromArgb(36, 13, 5);
            Opacity = 0F;

            SetClientSize(new Size(445, 495));

            StorageGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1531,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            //DXControl filterPanel = new DXControl
            //{
            //    Parent = this,
            //    Size = new Size(ClientArea.Width, 26),
            //    Location = new Point(ClientArea.Location.X, ClientArea.Location.Y),
            //    Border = true,
            //    BorderColour = Color.FromArgb(198, 166, 99)
            //};

            StorageTitleLabel = new DXLabel       //标题标签
            {
                Text = "仓库",
                Parent = StorageGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(204, 15)
            };

            DXLabel label = new DXLabel
            {
                Parent = StorageGround,
                Location = new Point(120, 5 + 450),
                Text = "名称".Lang(),
                Visible = false,
            };

            ItemNameTextBox = new DXTextBox   //道具名字文本框
            {
                Parent = StorageGround,
                Size = new Size(95, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Visible = false,
            };
            ItemNameTextBox.TextBox.TextChanged += (o, e) => ApplyStorageFilter();

            label = new DXLabel
            {
                Parent = StorageGround,
                Location = new Point(ItemNameTextBox.Location.X + ItemNameTextBox.Size.Width + 10, 5 + 450),
                Text = "物品".Lang(),
                Visible = false,
            };

            ItemTypeComboBox = new DXComboBox   //道具类型组合框
            {
                Parent = StorageGround,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198,
                Visible = false,
            };
            ItemTypeComboBox.SelectedItemChanged += (o, e) => ApplyStorageFilter();

            new DXListBoxItem
            {
                Parent = ItemTypeComboBox.ListBox,
                Label = { Text = $"全部".Lang() },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = ItemTypeComboBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            ItemTypeComboBox.ListBox.SelectItem(null);

            ClearButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1535,
                FixedSize = false,
                Location = new Point(48, 446),
                Parent = StorageGround,
                Hint = "整理".Lang(),
            };
            ClearButton.MouseClick += (o, e) =>   //鼠标点击
            {
                ItemTypeComboBox.ListBox.SelectItem(null);
                ItemNameTextBox.TextBox.Text = string.Empty;
                CEnvir.Enqueue(new Library.Network.ClientPackets.StorageItemRefresh { });
            };

            Grid = new DXItemGrid
            {
                Parent = StorageGround,
                GridSize = new Size(1, 1),
                Location = new Point(ClientArea.Location.X + 10, ClientArea.Location.Y + 33),
                GridType = GridType.Storage,  //网格类型
                ItemGrid = CEnvir.Storage,    //道具类型
                Border = false,
                BorderColour = Color.FromArgb(0, 90, 60, 50),
                VisibleHeight = 10,
            };

            Grid.GridSizeChanged += StorageGrid_GridSizeChanged;

            StorageScrollBar = new DXVScrollBar   //滚动条
            {
                Parent = StorageGround,
                Location = new Point(ClientArea.Right - 45, ClientArea.Location.Y + 32),
                Size = new Size(18, 378),
                VisibleSize = 10,
                Change = 1,
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;
            //为滚动条自定义皮肤 -1为不设置
            StorageScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);

            foreach (DXItemCell cell in Grid.Grid)
                cell.MouseWheel += StorageScrollBar.DoMouseWheel;

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(404, 449),
                Hint = "关闭",
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

        }
        /// <summary>
        /// 刷新仓库
        /// </summary>
        public void RefreshStorage()
        {
            Grid.GridSize = new Size(10, Math.Max(10, (int)Math.Ceiling(GameScene.Game.StorageSize / (float)10)));

            StorageScrollBar.MaxValue = Grid.GridSize.Height;

            ApplyStorageFilter();

        }
        /// <summary>
        /// 仓库格子容量改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in Grid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);

            foreach (DXItemCell cell in Grid.Grid)
                cell.MouseWheel += StorageScrollBar.DoMouseWheel;
        }
        /// <summary>
        /// 应用仓库过滤
        /// </summary>
        public void ApplyStorageFilter()
        {
            foreach (DXItemCell cell in Grid.Grid)
                FilterCell(cell);
        }
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="cell"></param>
        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= GameScene.Game.StorageSize)
            {
                cell.Enabled = false;
                return;
            }

            if (cell.Item == null && (ItemTypeComboBox.SelectedItem != null || !string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text)))
            {
                cell.Enabled = false;
                return;
            }

            if (ItemTypeComboBox.SelectedItem != null && cell.Item != null && cell.Item.Info.ItemType != (ItemType)ItemTypeComboBox.SelectedItem)
            {
                cell.Enabled = false;
                return;
            }

            if (!string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text) && cell.Item != null && cell.Item.Info.ItemName.IndexOf(ItemNameTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
            {
                cell.Enabled = false;
                return;
            }

            cell.Enabled = true;
        }
        /// <summary>
        /// 仓库滚动条数值变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Grid.ScrollValue = StorageScrollBar.Value;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (StorageTitleLabel != null)
                {
                    if (!StorageTitleLabel.IsDisposed)
                        StorageTitleLabel.Dispose();

                    StorageTitleLabel = null;
                }

                if (StorageGround != null)
                {
                    if (!StorageGround.IsDisposed)
                        StorageGround.Dispose();

                    StorageGround = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }
            }
        }
        #endregion
    }
}
