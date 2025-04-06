using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework.Input;
using Mir3.Mobile;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 寄售商城功能
    /// </summary>
    public sealed class MarketPlaceDialog : DXWindow
    {
        #region Properites

        public DXImageControl TabControl;

        #region Store
        public DXTextBox StoreItemNameBox, StoreBuyTotalBox;
        public DXNumberBox StoreBuyCountBox, StoreBuyPriceBox, HuntGoldBox;
        public DXComboBox StoreItemTypeBox, StoreSortBox;
        public DXControl StoreBuyPanel, FabSearch, NewSearch, QueryPanel, QueryWindow;
        public DXButton StoreBuyButton, StoreSearchButton;
        public DXCheckBox UseHuntGoldBox;
        public DXVScrollBar StoreScrollBar, QueryScrollBar;
        public DXLabel StoreBuyPriceLabel, GameGoldBox;
        public DXButton NextButton, PreviousButton;
        public DXLabel PageValue;

        public MarketPlaceStoreRow[] StoreRows;
        public MarketRecommendRow[] RecommendRows;
        public List<StoreInfo> StoreSearchResults;
        public List<DXControl> SearchItems = new List<DXControl>();

        const int changeSize = 20;
        #endregion

        /// <summary>
        /// 分类页，当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        private int _currentPage = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage; set
            {
                _currentPage = value;
                PageValue.Text = $@"{value} / {TotalPage}";
            }
        }

        private int _totalPage;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get => _totalPage;
            set
            {
                if (value == _totalPage) return;
                _totalPage = value;
                PageValue.Text = $@"{CurrentPage} / {value}";
            }
        }

        #region SelectedStoreRow
        /// <summary>
        /// 选定商城行
        /// </summary>
        public MarketPlaceStoreRow SelectedStoreRow
        {
            get => _SelectedStoreRow;
            set
            {
                if (_SelectedStoreRow == value) return;

                MarketPlaceStoreRow oldValue = _SelectedStoreRow;
                _SelectedStoreRow = value;

                OnSelectedStoreRowChanged(oldValue, value);
            }
        }
        private MarketPlaceStoreRow _SelectedStoreRow;
        public event EventHandler<EventArgs> SelectedStoreRowChanged;
        public void OnSelectedStoreRowChanged(MarketPlaceStoreRow oValue, MarketPlaceStoreRow nValue)
        {
            if (oValue != null)
            {
                oValue.Selected = false;
            }

            if (nValue != null)
            {
                nValue.Selected = true;
            }

            if (nValue?.StoreInfo == null)
            {
                StoreBuyPanel.Enabled = false;

                StoreBuyCountBox.MinValue = 0;
                StoreBuyCountBox.ValueTextBox.TextBox.Text = "";

                StoreBuyPriceBox.MinValue = 0;
                StoreBuyPriceBox.ValueTextBox.TextBox.Text = "";
            }
            else
            {
                StoreBuyPanel.Enabled = !GameScene.Game.Observer;

                StoreBuyCountBox.MinValue = 1;
                StoreBuyCountBox.MaxValue = nValue.StoreInfo.Item.StackSize;
                StoreBuyCountBox.Value = 1;

                StoreBuyPriceBox.MinValue = nValue.StoreInfo.Price;
                StoreBuyPriceBox.MaxValue = nValue.StoreInfo.Price;
                StoreBuyPriceBox.Value = nValue.StoreInfo.Price;
            }
            SelectedStoreRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DateTime NextSearchTime;

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (TabControl == null) return;

            if (!Visible) return;

            if (StoreSearchResults == null)
                StoreSearch();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public MarketPlaceDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size = new Size(800, 516);
            Location = ClientArea.Location;

            /// <summary>
            /// 商城面板
            /// </summary>
            TabControl = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4801,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
            };

            DXLabel dXLabel = new DXLabel
            {
                Parent = TabControl,
                Location = new Point(385, 12),
                Text = "商城".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
            };

            CloseButton.Parent = TabControl;

            dXLabel = new DXLabel
            {
                Parent = TabControl,
                Location = new Point(77, 359),
                Text = "赞助币".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
            };

            #region 元宝商城

            TabControl.IsVisibleChanged += (o, e) => SelectedStoreRow = null;

            StoreItemNameBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(135, 18),
                Location = new Point(382, 42),
                Border = false,
                BackColour = Color.Empty,
            };
            StoreItemNameBox.TextBox.KeyPress += StoreTextBox_KeyPress;

            StoreItemTypeBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(275, 42),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198,
                Border = false,
            };

            new DXListBoxItem
            {
                Parent = StoreItemTypeBox.ListBox,
                Label = { Text = $"不排列".Lang() },
                Item = null
            };

            HashSet<string> filters = new HashSet<string>();

            StoreItemTypeBox.ListBox.SelectItem(null);

            DXLabel label = new DXLabel
            {
                Parent = this,
                Location = new Point(StoreItemTypeBox.Location.X + StoreItemTypeBox.Size.Width + 10, 5),
                Text = "分类".Lang(),
                Visible = false,
            };

            StoreSortBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
                Visible = false,
            };

            Type storeType = typeof(MarketPlaceStoreSort);

            for (MarketPlaceStoreSort i = MarketPlaceStoreSort.Alphabetical; i <= MarketPlaceStoreSort.Favourite; i++)
            {
                new DXListBoxItem
                {
                    Parent = StoreSortBox.ListBox,
                    Label = { Text = i.Lang() },
                    Item = i
                };
            }

            StoreSortBox.ListBox.SelectItem(MarketPlaceStoreSort.Favourite);

            StoreSearchButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4827,
                Parent = this,
                Location = new Point(524, 36),
            };
            StoreSearchButton.MouseClick += (o, e) => StoreSearch();

            DXButton ClearButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4807,
                Location = new Point(11, 438),
                Parent = this,
                Label = { Text = "刷新商城".Lang() },
            };
            ClearButton.MouseClick += (o, e) =>
            {
                StoreItemNameBox.TextBox.Text = "";
                StoreItemTypeBox.ListBox.SelectItem(null);
                StoreSearch();
            };

            StoreRows = new MarketPlaceStoreRow[10];

            StoreScrollBar = new DXVScrollBar
            {
                Parent = TabControl,
                Location = new Point(760, 47),
                Size = new Size(14, TabControl.Size.Height - 59),
                VisibleSize = StoreRows.Length,
                Change = 3,
                Visible = false,
            };
            StoreScrollBar.ValueChanged += StoreScrollBar_ValueChanged;

            var x = 0;
            var y = -15;

            for (int i = 0; i < StoreRows.Length; i++)
            {
                if ((i + 1) % 2 == 0)
                {
                    x += 202;
                }
                else
                {
                    x = 200;
                    y += 80;
                }
                int index = i;
                StoreRows[index] = new MarketPlaceStoreRow
                {
                    Parent = TabControl,
                    Location = new Point(x, y),
                };
                StoreRows[index].MouseClick += (o, e) =>
                {
                    SelectedStoreRow = StoreRows[index];
                };
                StoreRows[index].MouseWheel += StoreScrollBar.DoMouseWheel;
            }

            RecommendRows = new MarketRecommendRow[5];

            for (int i = 0; i < RecommendRows.Length; i++)
            {
                int index = i;
                RecommendRows[index] = new MarketRecommendRow
                {
                    Parent = TabControl,
                    Location = new Point(622, 87 + i * 88),
                };
                RecommendRows[index].MouseClick += (o, e) =>
                {
                    var str = StoreItemNameBox.TextBox.Text;
                    StoreItemNameBox.TextBox.Text = RecommendRows[index].ItemCell.Item.Info.ItemName;
                    StoreSearch();

                    StoreItemNameBox.TextBox.Text = str;
                };
            }

            DXControl HuntGoldPanel = new DXControl
            {
                Location = new Point(555, 149),
                Parent = TabControl,
                Size = new Size(175, 50),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Visible = false,
            };

            new DXLabel
            {
                Parent = HuntGoldPanel,
                Text = "赏金".Lang(),
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
            };

            label = new DXLabel
            {
                Parent = HuntGoldPanel,
                Text = "数量".Lang(),
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            HuntGoldBox = new DXNumberBox
            {
                Parent = HuntGoldPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = 0,
            };

            GameGoldBox = new DXLabel
            {
                Parent = this,
                Location = new Point(40, 375),
                Size = new Size(120, 20),
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            DXButton RechargeButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4807,
                Location = new Point(11, 408),
                Parent = this,
                Label = { Text = "赞助币充值".Lang() },
                Enabled = !CEnvir.TestServer,
            };
            RechargeButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                new DXConfirmWindow("MarketPlace.Recharge".Lang(), DXMessageBoxButtons.YesNo, () =>
                {
                    if (string.IsNullOrEmpty(CEnvir.BuyAddress)) return;

                    if (CEnvir.ClientControl.RechargeInterfaceCheck)
                    {
                        //System.Diagnostics.Process.Start(CEnvir.BuyAddress);
                        Game1.Native.OpenUrl(CEnvir.BuyAddress);
                        return;
                    }
                    else
                    {
                        //System.Diagnostics.Process.Start(CEnvir.BuyAddress + MapObject.User.Name);
                        Game1.Native.OpenUrl(CEnvir.BuyAddress + MapObject.User.Name);
                        return;
                    }
                });
            };

            DXButton SpareButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4807,
                Location = new Point(11, 467),
                Parent = this,
                Label = { Text = " ".Lang() },
            };

            StoreBuyPanel = new DXControl
            {
                Location = new Point(555, 279),
                Parent = TabControl,
                Size = new Size(175, 150),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Enabled = false,
                Visible = false,
            };

            new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = "购买".Lang(),
                ForeColour = Color.White,
                AutoSize = false,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Size = new Size(175, 15)
            };

            label = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = "数量".Lang(),
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 20);

            StoreBuyCountBox = new DXNumberBox
            {
                Parent = StoreBuyPanel,
                Location = new Point(50, 20),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18) },
                MaxValue = 5000,
                MinValue = 1,
                UpButton = { Location = new Point(108, 1) }
            };
            StoreBuyCountBox.ValueTextBox.ValueChanged += UpdateStoreBuyTotal;

            StoreBuyPriceLabel = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = "赞助币".Lang(),
                ForeColour = Color.White,
            };
            StoreBuyPriceLabel.Location = new Point(50 - StoreBuyPriceLabel.Size.Width, 40);

            StoreBuyPriceBox = new DXNumberBox
            {
                Parent = StoreBuyPanel,
                Location = new Point(50, 40),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(85, 18), ReadOnly = true, Editable = false, ForeColour = Color.FromArgb(198, 166, 99), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
                MaxValue = 200000000,
                MinValue = 0
            };
            StoreBuyPriceBox.ValueTextBox.ValueChanged += UpdateStoreBuyTotal;

            StoreBuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = StoreBuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };

            label = new DXLabel
            {
                Parent = StoreBuyPanel,
                Text = "总价".Lang(),
                ForeColour = Color.White,
            };
            label.Location = new Point(50 - label.Size.Width, 60);

            StoreBuyTotalBox = new DXTextBox
            {
                Location = new Point(69, 61),
                Size = new Size(85, 18),
                Parent = StoreBuyPanel,
                ReadOnly = true,
                Editable = false,
                ForeColour = Color.FromArgb(198, 166, 99),
            };

            UseHuntGoldBox = new DXCheckBox
            {
                Parent = StoreBuyPanel,
                Label = { Text = "使用赏金".Lang() },
                Visible = false,
            };
            UseHuntGoldBox.Location = new Point(158 - UseHuntGoldBox.Size.Width, 101);
            UseHuntGoldBox.CheckedChanged += UpdateStoreBuyTotal;

            StoreBuyButton = new DXButton
            {
                Size = new Size(85, SmallButtonHeight),
                Location = new Point(69, 124),
                Label = { Text = "购买".Lang() },
                ButtonType = ButtonType.SmallButton,
                Parent = StoreBuyPanel,
            };
            StoreBuyButton.MouseClick += StoreBuyButton_MouseClick;

            PreviousButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4840,
                Parent = this,
                Location = new Point(328, 477),
                Hint = "上一页".Lang(),
            };
            PreviousButton.MouseClick += PreviousButton_MouseClick;

            NextButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4845,
                Parent = this,
                Location = new Point(459, 477),
                Hint = "下一页".Lang(),
            };
            NextButton.MouseClick += NextButton_MouseClick;

            PageValue = new DXLabel
            {
                Parent = this,
                IsControl = false,
                AutoSize = false,
                Size = new Size(108, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Location = new Point(350, 475),
            };

            #region 搜索栏
            QueryWindow = new DXControl   //左边查询栏窗体
            {
                Location = new Point(10, 39),
                Parent = TabControl,
                Size = new Size(150, 300),
                Border = false,
                //PassThrough = true,
            };

            QueryPanel = new DXControl   //左边查询栏窗体
            {
                Parent = QueryWindow,
                Size = new Size(150, changeSize * 2),
                Border = false,
                //PassThrough = true,
            };
            FabSearch = new DXControl
            {
                Parent = QueryPanel,
                Size = new Size(148, changeSize),
                //PassThrough = true,
            };
            FabSearch.MouseClick += FabSearch_MouseClick;
            var icon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Parent = FabSearch,
                Index = 4850,
                Location = new Point(5, 5),
                PassThrough = true,
            };
            new DXLabel
            {
                Text = "收藏夹".Lang(),
                Parent = FabSearch,
                Location = new Point(icon.Location.X + 17, icon.Location.Y),
                PassThrough = true,
            };

            NewSearch = new DXControl
            {
                Parent = QueryPanel,
                Size = new Size(148, changeSize),
                Location = new Point(FabSearch.Location.X, FabSearch.Location.Y + changeSize + 1),
                //PassThrough = true,
            };
            NewSearch.MouseClick += NewSearch_MouseClick;
            icon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Parent = NewSearch,
                Index = 4850,
                Location = new Point(5, 5),
                PassThrough = true,
            };
            new DXLabel
            {
                Text = "新物品".Lang(),
                Parent = NewSearch,
                Location = new Point(icon.Location.X + 17, icon.Location.Y),
                PassThrough = true,
            };

            var sX = NewSearch.Location.X;
            var sY = NewSearch.Location.Y;
            foreach (var item in Globals.StoreDic)
            {
                sY += changeSize + 1;
                var sItem = new DXControl
                {
                    Parent = QueryPanel,
                    Size = new Size(148, (item.Value.Length + 1) * changeSize),
                    Location = new Point(sX, sY),
                    Tag = false,
                    //PassThrough = true,
                };
                sItem.MouseClick += SItem_MouseClick;
                var image = new DXImageControl                       //+号
                {
                    LibraryFile = LibraryFile.GameInter,
                    Parent = sItem,
                    Index = 4870,
                    Location = new Point(5, 5),
                    PassThrough = false,
                    Tag = false,
                };
                image.MouseClick += ImageAdd_MouseClick;
                new DXLabel                          //+号后面得文字
                {
                    Text = item.Key.Lang(),
                    Parent = sItem,
                    Location = new Point(icon.Location.X + 17, icon.Location.Y),
                    PassThrough = true,
                    Tag = Text,
                };
                SearchItems.Add(sItem);
                var subY = 0;
                foreach (var sub in item.Value)
                {
                    subY += changeSize + 1;
                    var subItem = new DXControl
                    {
                        Parent = sItem,
                        Size = new Size(148, changeSize),
                        Location = new Point(sX, subY),
                        Visible = false,
                        Tag = sub.Lang(),
                        //PassThrough = true,
                    };
                    subItem.MouseClick += SubItem_MouseClick;
                    new DXLabel
                    {
                        Text = sub.Lang(),
                        Parent = subItem,
                        Location = new Point(27, 5),
                        PassThrough = true,
                    };
                }
            }

            QueryPanel.Size = new Size(QueryPanel.Size.Width, QueryPanel.Size.Height + (Globals.StoreDic.Count + 1) * changeSize);

            QueryScrollBar = new DXVScrollBar   //查询栏滚动条
            {
                Parent = this,
                Location = new Point(167, 45),
                Size = new Size(20, 295),
            };
            QueryScrollBar.VisibleSize = QueryWindow.Size.Height;
            QueryScrollBar.Change = changeSize + 1;
            QueryScrollBar.ValueChanged += QueryScrollBar_ValueChanged;


            //QueryPanel.MouseWheel += QueryScrollBar.DoMouseWheel;
            QueryPanel.SizeChanged += QueryPanel_SizeChanged;
            //为滚动条自定义皮肤 - 1为不设置
            QueryScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);

            #endregion

            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Recommend > 0 && info.Recommend < 6)
                {
                    RecommendRows[info.Recommend - 1].StoreInfo = info;
                    RecommendRows[info.Recommend - 1].Visible = true;
                }

                if (string.IsNullOrEmpty(info.Filter)) continue;

                string[] temp = info.Filter.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in temp)
                    filters.Add(s.Trim());
            }

            foreach (string filter in filters.OrderBy(x => x))
            {
                new DXListBoxItem
                {
                    Parent = StoreItemTypeBox.ListBox,
                    Label = { Text = filter },
                    Item = filter
                };
            }

            #endregion
        }


        private void SItem_MouseClick(object sender, MouseEventArgs e)
        {
            var item = sender as DXControl;
            var label = item.Controls[1] as DXLabel;//注意这里下标使用的是硬编码如果添加控件以后这里要根据实际修改
            var filter = label.Text;
            StoreScrollBar.MaxValue = 0;

            foreach (MarketPlaceStoreRow row in StoreRows)
                row.Visible = true;

            if (StoreSearchResults == null)
                StoreSearchResults = new List<StoreInfo>();
            else
                StoreSearchResults.Clear();
            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Item == null) continue;
                var substrs = info.Filter.Split(',')[0];
                if (substrs == filter)
                    StoreSearchResults.Add(info);
            }

            TotalPage = (StoreSearchResults.Count - 1) / 10 + 1;
            RefreshStoreList();

        }

        private void ImageAdd_MouseClick(object sender, MouseEventArgs e)
        {
            var item = sender as DXImageControl;
            var espand = !Convert.ToBoolean(item.Parent.Tag);
            var y = 0;
            item.Index = espand ? 4871 : 4870;
            foreach (var sub in item.Parent.Controls)
            {
                if (sub.GetType() == typeof(DXControl))
                {
                    sub.Visible = espand;
                    if (espand)
                    {
                        y += changeSize + 1;
                        QueryPanel.Size = new Size(QueryPanel.Size.Width, QueryPanel.Size.Height + changeSize);
                    }
                    else
                    {
                        y -= (changeSize + 1);
                        QueryPanel.Size = new Size(QueryPanel.Size.Width, QueryPanel.Size.Height - changeSize);
                    }
                }
            }
            item.Parent.Tag = espand;
            var find = false;
            foreach (var search in SearchItems)
            {
                if (search == item.Parent)
                {
                    find = true;
                    continue;
                }
                if (find)
                {
                    search.Location = new Point(search.Location.X, search.Location.Y + y);
                }
            }
        }

        private void QueryPanel_SizeChanged(object sender, EventArgs e)
        {
            QueryScrollBar.MaxValue = QueryPanel.Size.Height;
        }

        /// <summary>
        /// 滚动条变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryScrollBar_ValueChanged(object sender, EventArgs e)
        {

            int y = QueryScrollBar.Value % QueryScrollBar.Change != 0 ? QueryScrollBar.Value - QueryScrollBar.Value % QueryScrollBar.Change + QueryScrollBar.Change : QueryScrollBar.Value;

            QueryPanel.Location = new Point(QueryPanel.Location.X, -y);
        }

        private void SubItem_MouseClick(object sender, MouseEventArgs e)
        {
            var item = sender as DXControl;
            var itemper = item.Parent as DXControl;
            var filter1 = itemper.Controls[1].Text;//这里使用硬编码
            var fitler2 = item.Tag as string;
            StoreScrollBar.MaxValue = 0;

            foreach (MarketPlaceStoreRow row in StoreRows)
                row.Visible = true;

            if (StoreSearchResults == null)
                StoreSearchResults = new List<StoreInfo>();
            else
                StoreSearchResults.Clear();
            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Item == null) continue;
                var substrs = info.Filter.Split(',');

                if (substrs[0] == filter1 && substrs[1] == fitler2)
                    StoreSearchResults.Add(info);
            }
            CurrentPage = 1;
            TotalPage = (StoreSearchResults.Count - 1) / 10 + 1;
            RefreshStoreList();

        }

        private void NewSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (StoreSearchResults == null)
                StoreSearchResults = new List<StoreInfo>();
            else
                StoreSearchResults.Clear();


            StoreScrollBar.MaxValue = 0;

            foreach (MarketPlaceStoreRow row in StoreRows)
                row.Visible = true;

            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Item == null) continue;

                if (info.Recommend == 999)
                    StoreSearchResults.Add(info);
            }
            CurrentPage = 1;
            TotalPage = (StoreSearchResults.Count - 1) / 10 + 1;
            RefreshStoreList();
        }

        private void FabSearch_MouseClick(object sender, MouseEventArgs e)
        {
            //TODO 收藏搜索发包
        }

        #region Methods

        /// <summary>
        /// 商城搜索
        /// </summary>
        public void StoreSearch()
        {
            if (StoreSearchResults == null)
                StoreSearchResults = new List<StoreInfo>();
            else
                StoreSearchResults.Clear();

            StoreScrollBar.MaxValue = 0;

            foreach (MarketPlaceStoreRow row in StoreRows)
                row.Visible = true;

            string filter = (string)StoreItemTypeBox.SelectedItem;

            MarketPlaceStoreSort sort = (MarketPlaceStoreSort)StoreSortBox.SelectedItem;

            foreach (StoreInfo info in Globals.StoreInfoList.Binding)
            {
                if (info.Item == null) continue;

                if (filter != null && !info.Filter.Contains(filter)) continue;

                if (!string.IsNullOrEmpty(StoreItemNameBox.TextBox.Text) && info.Item.ItemName.IndexOf(StoreItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;

                StoreSearchResults.Add(info);
            }

            switch (sort)
            {
                case MarketPlaceStoreSort.Alphabetical:
                    StoreSearchResults.Sort((x1, x2) => string.Compare(x1.Item.ItemName, x2.Item.ItemName, StringComparison.Ordinal));
                    break;
                case MarketPlaceStoreSort.HighestPrice:
                    StoreSearchResults.Sort((x1, x2) => x2.Price.CompareTo(x1.Price));
                    break;
                case MarketPlaceStoreSort.LowestPrice:
                    StoreSearchResults.Sort((x1, x2) => x1.Price.CompareTo(x2.Price));
                    break;
                case MarketPlaceStoreSort.Favourite:
                    break;
            }
            TotalPage = (StoreSearchResults.Count - 1) / 10 + 1;
            RefreshStoreList();
        }

        /// <summary>
        /// 刷新商城列表
        /// </summary>
        public void RefreshStoreList(int page = 1)
        {
            CurrentPage = page;
            if (StoreSearchResults == null && page > TotalPage) return;

            var Value = (page - 1) * 10;

            for (int i = 0; i < StoreRows.Length; i++)
            {
                if (i + Value >= StoreSearchResults.Count)
                {
                    StoreRows[i].StoreInfo = null;
                    StoreRows[i].Visible = false;
                    continue;
                }

                StoreRows[i].StoreInfo = StoreSearchResults[i + Value];
            }
        }

        private void NextButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage + 1 > TotalPage) return;
            CurrentPage += 1;
            RefreshStoreList(CurrentPage);
        }

        private void PreviousButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage - 1 < 1) return;
            CurrentPage -= 1;
            RefreshStoreList(CurrentPage);
        }

        /// <summary>
        /// 鼠标点击商城购买按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoreBuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedStoreRow?.StoreInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();

            message.Append($"物品".Lang() + $": {SelectedStoreRow.StoreInfo.Item.Lang(p => p.ItemName)}");

            if (StoreBuyCountBox.Value > 1)
                message.Append($" x{StoreBuyCountBox.Value:###0}");

            message.Append("\n\n");

            message.Append($"价格".Lang() + $": {StoreBuyPriceBox.Value:###0}");

            if (StoreBuyCountBox.Value > 1)
                message.Append(" (" + "每个".Lang() + ")");

            message.Append("\n\n");

            message.Append($"总价".Lang() + $": {StoreBuyTotalBox.TextBox.Text} ({(UseHuntGoldBox.Checked ? "赏金".Lang() : "赞助币".Lang())})");

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                StoreBuyButton.Enabled = false;

                CEnvir.Enqueue(new C.MarketPlaceStoreBuy { Index = SelectedStoreRow.StoreInfo.Index, Count = StoreBuyCountBox.Value, UseHuntGold = UseHuntGoldBox.Checked });
            });
        }

        /// <summary>
        /// 更新商城购买总额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateStoreBuyTotal(object sender, EventArgs e)
        {
            StoreInfo info = SelectedStoreRow?.StoreInfo;

            if (UseHuntGoldBox.Checked)
            {
                if (info != null)
                    StoreBuyPriceBox.Value = info.HuntGoldPrice;

                StoreBuyPriceLabel.Text = "赏金".Lang();
            }
            else
            {
                if (info != null)
                    StoreBuyPriceBox.Value = info.Price;

                StoreBuyPriceLabel.Text = "赞助币".Lang();
            }

            StoreBuyTotalBox.TextBox.Text = (StoreBuyCountBox.Value * StoreBuyPriceBox.Value).ToString("###0");
        }

        /// <summary>
        /// 商城滚动条变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoreScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshStoreList();
        }

        /// <summary>
        /// 商城输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoreTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (StoreSearchButton.Enabled)
                StoreSearch();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                #region Store

                if (StoreItemNameBox != null)
                {
                    if (!StoreItemNameBox.IsDisposed)
                        StoreItemNameBox.Dispose();

                    StoreItemNameBox = null;
                }

                if (StoreBuyTotalBox != null)
                {
                    if (!StoreBuyTotalBox.IsDisposed)
                        StoreBuyTotalBox.Dispose();

                    StoreBuyTotalBox = null;
                }

                if (StoreBuyCountBox != null)
                {
                    if (!StoreBuyCountBox.IsDisposed)
                        StoreBuyCountBox.Dispose();

                    StoreBuyCountBox = null;
                }

                if (StoreBuyPriceBox != null)
                {
                    if (!StoreBuyPriceBox.IsDisposed)
                        StoreBuyPriceBox.Dispose();

                    StoreBuyPriceBox = null;
                }

                if (GameGoldBox != null)
                {
                    if (!GameGoldBox.IsDisposed)
                        GameGoldBox.Dispose();

                    GameGoldBox = null;
                }

                if (HuntGoldBox != null)
                {
                    if (!HuntGoldBox.IsDisposed)
                        HuntGoldBox.Dispose();

                    HuntGoldBox = null;
                }

                if (StoreItemTypeBox != null)
                {
                    if (!StoreItemTypeBox.IsDisposed)
                        StoreItemTypeBox.Dispose();

                    StoreItemTypeBox = null;
                }

                if (StoreSortBox != null)
                {
                    if (!StoreSortBox.IsDisposed)
                        StoreSortBox.Dispose();

                    StoreSortBox = null;
                }

                if (StoreBuyPanel != null)
                {
                    if (!StoreBuyPanel.IsDisposed)
                        StoreBuyPanel.Dispose();

                    StoreBuyPanel = null;
                }

                if (StoreBuyButton != null)
                {
                    if (!StoreBuyButton.IsDisposed)
                        StoreBuyButton.Dispose();

                    StoreBuyButton = null;
                }

                if (StoreSearchButton != null)
                {
                    if (!StoreSearchButton.IsDisposed)
                        StoreSearchButton.Dispose();

                    StoreSearchButton = null;
                }

                if (UseHuntGoldBox != null)
                {
                    if (!UseHuntGoldBox.IsDisposed)
                        UseHuntGoldBox.Dispose();

                    UseHuntGoldBox = null;
                }

                if (StoreScrollBar != null)
                {
                    if (!StoreScrollBar.IsDisposed)
                        StoreScrollBar.Dispose();

                    StoreScrollBar = null;
                }
                QueryWindow.TryDispose();
                QueryPanel.TryDispose();
                if (QueryScrollBar != null)
                {
                    if (!QueryScrollBar.IsDisposed)
                        QueryScrollBar.Dispose();

                    QueryScrollBar = null;
                }

                if (PreviousButton != null)
                {
                    if (!PreviousButton.IsDisposed)
                        PreviousButton.Dispose();

                    PreviousButton = null;
                }

                if (NextButton != null)
                {
                    if (!NextButton.IsDisposed)
                        NextButton.Dispose();

                    NextButton = null;
                }

                if (PageValue != null)
                {
                    if (!PageValue.IsDisposed)
                        PageValue.Dispose();

                    PageValue = null;
                }

                if (StoreRows != null)
                {
                    for (int i = 0; i < StoreRows.Length; i++)
                    {
                        if (StoreRows[i] != null)
                        {
                            if (!StoreRows[i].IsDisposed)
                                StoreRows[i].Dispose();

                            StoreRows[i] = null;
                        }
                    }

                    StoreRows = null;
                }

                if (RecommendRows != null)
                {
                    for (int i = 0; i < RecommendRows.Length; i++)
                    {
                        if (RecommendRows[i] != null)
                        {
                            if (!RecommendRows[i].IsDisposed)
                                RecommendRows[i].Dispose();

                            RecommendRows[i] = null;
                        }
                    }

                    RecommendRows = null;
                }
                StoreSearchResults?.Clear();
                StoreSearchResults = null;

                #endregion

                _SelectedStoreRow = null;
                SelectedStoreRowChanged = null;

                NextSearchTime = DateTime.MinValue;
            }
        }
        #endregion
    }
    /// <summary>
    /// 商城内容条
    /// </summary>
    public sealed class MarketPlaceStoreRow : DXControl
    {
        #region Properties

        #region Selected
        /// <summary>
        /// 选定
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
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(74, 56, 41);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region StoreInfo
        /// <summary>
        /// 商城信息
        /// </summary>
        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;

                StoreInfo oldValue = _StoreInfo;
                _StoreInfo = value;

                OnStoreInfoChanged(oldValue, value);
            }
        }
        private StoreInfo _StoreInfo;
        public event EventHandler<EventArgs> StoreInfoChanged;
        public void OnStoreInfoChanged(StoreInfo oValue, StoreInfo nValue)
        {
            Visible = StoreInfo?.Item != null;
            if (StoreInfo?.Item == null) return;

            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(StoreInfo.Duration);

            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;

            ItemCell.Item = new ClientUserItem(StoreInfo.Item, 1)
            {
                Flags = flags,
                ExpireTime = duration
            };

            ItemCell.RefreshItem();

            NameLabel.Text = StoreInfo.Item.Lang(p => p.ItemName);

            PriceLabel.Text = (Convert.ToDecimal(StoreInfo.Price) / 100).ToString("###0");

            if (!StoreInfo.Available)
                PriceLabel.Text = "(" + "无法使用".Lang() + ")";

            HuntPriceLabel.Visible = StoreInfo.HuntGoldPrice != 0;

            HuntPriceLabelLabel.Visible = StoreInfo.HuntGoldPrice != 0;

            HuntPriceLabel.Text = (StoreInfo.HuntGoldPrice == 0 ? StoreInfo.Price : StoreInfo.HuntGoldPrice).ToString("###0");

            PriceLabel.Visible = StoreInfo.Price != 0;

            PriceLabelLabel.Visible = StoreInfo.Price != 0;

            PriceLabel.Text = (Convert.ToDecimal((StoreInfo.Price == 0 ? StoreInfo.HuntGoldPrice : StoreInfo.Price)) / 100).ToString("###0");

            if (!StoreInfo.Available)
                HuntPriceLabel.Text = "(" + "无法使用".Lang() + ")";

            if (GameScene.Game.MarketPlaceBox.SelectedStoreRow == this)
                GameScene.Game.MarketPlaceBox.SelectedStoreRow = null;

            StoreInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXImageControl BackGound;
        public DXControl BackParent;
        public DXItemCell ItemCell;
        public DXLabel NameLabel, PriceLabel, HuntPriceLabel, PriceLabelLabel, HuntPriceLabelLabel;
        public DXButton FavouriteImage, StoreBuyButton, GiveImage;
        public DXComboBox CountTypeBox;

        #endregion

        /// <summary>
        /// 商城内容条
        /// </summary>
        public MarketPlaceStoreRow()
        {
            Size = new Size(200, 78);

            BackGound = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4872,
                Location = new Point(-1, 2),
                Visible = false,
                Opacity = 0.85F
            };

            BackParent = new DXControl
            {
                Parent = this,
                Size = new Size(200, 80),
                Location = new Point(0, 0),
            };
            BackParent.MouseEnter += (o, e) =>
            {
                BackGound.Visible = true;
            };
            BackParent.MouseLeave += (o, e) =>
            {
                BackGound.Visible = false;
            };

            ItemCell = new DXItemCell
            {
                Parent = BackParent,
                Location = new Point(17, 18),
                FixedBorder = true,
                Border = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };
            ItemCell.MouseEnter += (o, e) =>
            {
                BackGound.Visible = true;
            };
            ItemCell.MouseLeave += (o, e) =>
            {
                BackGound.Visible = false;
            };

            NameLabel = new DXLabel
            {
                Parent = BackParent,
                Location = new Point(70, 10),
                IsControl = false,
                AutoSize = false,
                Size = new Size(120, 16),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            PriceLabel = new DXLabel
            {
                Parent = BackParent,
                Location = new Point(15, 60),
                IsControl = false,
                AutoSize = false,
                Size = new Size(40, 16),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.Orange,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Outline = false,
            };

            PriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "赞助币".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Visible = false,

            };
            PriceLabelLabel.Location = new Point(290 - PriceLabelLabel.Size.Width, 12);

            HuntPriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(420, 12),
                IsControl = false,
                Visible = false,
            };

            HuntPriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "赏金".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Visible = false,

            };
            HuntPriceLabelLabel.Location = new Point(420 - HuntPriceLabelLabel.Size.Width, 12);

            FavouriteImage = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4855,
                Parent = BackParent,
                Location = new Point(155, 52),
                Hint = "收藏".Lang(),
            };
            FavouriteImage.MouseEnter += (o, e) =>
            {
                BackGound.Visible = true;
            };
            FavouriteImage.MouseLeave += (o, e) =>
            {
                BackGound.Visible = false;
            };

            GiveImage = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4830,
                Parent = BackParent,
                Location = new Point(115, 52),
                Hint = "赠送".Lang(),
            };
            GiveImage.MouseEnter += (o, e) =>
            {
                BackGound.Visible = true;
            };
            GiveImage.MouseLeave += (o, e) =>
            {
                BackGound.Visible = false;
            };

            StoreBuyButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4835,
                Parent = BackParent,
                Location = new Point(75, 52),
                Hint = "购买".Lang(),
            };
            StoreBuyButton.MouseEnter += (o, e) =>
            {
                BackGound.Visible = true;
            };
            StoreBuyButton.MouseLeave += (o, e) =>
            {
                BackGound.Visible = false;
            };
            StoreBuyButton.MouseClick += StoreBuyButton_MouseClick;

            CountTypeBox = new DXComboBox
            {
                Parent = BackParent,
                Location = new Point(70, 31),
                Size = new Size(115, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198,
                Border = false,
            };
            for (var i = 1; i <= 9; i++)
            {
                new DXListBoxItem
                {
                    Parent = CountTypeBox.ListBox,
                    Label = { Text = $"{i}个".Lang(), AutoSize = false, Size = new Size(100, 14), DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, },
                    Item = i,
                };
            }
            CountTypeBox.ListBox.SelectItem(1);
        }

        /// <summary>
        /// 鼠标点击商城购买按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoreBuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (StoreInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();

            message.Append($"购买商品".Lang() + $": {StoreInfo.Item.Lang(p => p.ItemName)}");

            if ((int)CountTypeBox.SelectedItem > 1)
                message.Append($" {(int)CountTypeBox.SelectedItem} 个");

            message.Append("\n\n");

            if ((int)CountTypeBox.SelectedItem > 1)
            {
                message.Append($"单价".Lang() + $": {(StoreInfo.Price / 100).ToString("###0")}" + " 赞助币".Lang());

                message.Append("\n\n");
            }

            message.Append($"总价".Lang() + $": {((StoreInfo.Price / 100) * (int)CountTypeBox.SelectedItem).ToString("###0")}" + " 赞助币".Lang());

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                //StoreBuyButton.Enabled = false;

                CEnvir.Enqueue(new C.MarketPlaceStoreBuy { Index = StoreInfo.Index, Count = (int)CountTypeBox.SelectedItem, UseHuntGold = false, });
            });
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _StoreInfo = null;
                StoreInfoChanged = null;

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (PriceLabel != null)
                {
                    if (!PriceLabel.IsDisposed)
                        PriceLabel.Dispose();

                    PriceLabel = null;
                }

                if (PriceLabelLabel != null)
                {
                    if (!PriceLabelLabel.IsDisposed)
                        PriceLabelLabel.Dispose();

                    PriceLabelLabel = null;
                }

                if (HuntPriceLabel != null)
                {
                    if (!HuntPriceLabel.IsDisposed)
                        HuntPriceLabel.Dispose();

                    HuntPriceLabel = null;
                }

                if (HuntPriceLabelLabel != null)
                {
                    if (!HuntPriceLabelLabel.IsDisposed)
                        HuntPriceLabelLabel.Dispose();

                    HuntPriceLabelLabel = null;
                }

                if (FavouriteImage != null)
                {
                    if (!FavouriteImage.IsDisposed)
                        FavouriteImage.Dispose();

                    FavouriteImage = null;
                }

                if (CountTypeBox != null)
                {
                    if (!CountTypeBox.IsDisposed)
                        CountTypeBox.Dispose();

                    CountTypeBox = null;
                }

                if (GiveImage != null)
                {
                    if (!GiveImage.IsDisposed)
                        GiveImage.Dispose();

                    GiveImage = null;
                }

                if (StoreBuyButton != null)
                {
                    if (!StoreBuyButton.IsDisposed)
                        StoreBuyButton.Dispose();

                    StoreBuyButton = null;
                }

                if (BackParent != null)
                {
                    if (!BackParent.IsDisposed)
                        BackParent.Dispose();

                    BackParent = null;
                }

                if (BackGound != null)
                {
                    if (!BackGound.IsDisposed)
                        BackGound.Dispose();

                    BackGound = null;
                }
            }
        }
        #endregion
    }

    public sealed class MarketRecommendRow : DXControl
    {
        #region StoreInfo
        /// <summary>
        /// 商城信息
        /// </summary>
        public StoreInfo StoreInfo
        {
            get => _StoreInfo;
            set
            {
                if (_StoreInfo == value) return;

                StoreInfo oldValue = _StoreInfo;
                _StoreInfo = value;

                OnStoreInfoChanged(oldValue, value);
            }
        }
        private StoreInfo _StoreInfo;
        public event EventHandler<EventArgs> StoreInfoChanged;
        public void OnStoreInfoChanged(StoreInfo oValue, StoreInfo nValue)
        {
            Visible = StoreInfo?.Item != null;
            if (StoreInfo?.Item == null) return;

            UserItemFlags flags = UserItemFlags.Worthless;
            TimeSpan duration = TimeSpan.FromSeconds(StoreInfo.Duration);

            if (duration != TimeSpan.Zero)
                flags |= UserItemFlags.Expirable;

            ItemCell.Item = new ClientUserItem(StoreInfo.Item, 1)
            {
                Flags = flags,
                ExpireTime = duration
            };

            ItemCell.RefreshItem();

            NameLabel.Text = StoreInfo.Item.Lang(p => p.ItemName);

            StoreInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;
        public DXLabel NameLabel;

        public MarketRecommendRow()
        {
            Size = new Size(160, 45);

            DrawTexture = false;
            BackColour = Color.Red;

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(10, 5),
                FixedBorder = true,
                Border = false,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                ShowCountLabel = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(50, 15),
                IsControl = false,
                AutoSize = false,
                Size = new Size(110, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }
            }
        }
        #endregion
    }
}
