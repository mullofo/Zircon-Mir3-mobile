using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 拍卖行
    /// </summary>
    public sealed class AuctionsDialog : DXWindow
    {
        #region Properites

        #region Search
        public DXImageControl SearchBackGround;
        public DXControl IconControl;
        public DXTextBox ItemNameBox;
        public DXComboBox ItemTypeBox;
        public DXButton BuyButton, SearchButton, ClearButton, Close1Button, ConsignButton, AuctionBidButton;
        public DXLabel BiddingPriceLabel, ItemNameLabelLabel, FixedPriceLabel, SellerLabelLabel;
        public DXVScrollBar SearchScrollBar;

        public AuctionsRow[] SearchRows;
        public List<ClientNewAuction> ItemResults;
        public List<ClientNewAuction> SearchResults;
        public CurrencyType DlgType;
        #endregion

        #region SelectedRow


        /// <summary>
        /// 选定行
        /// </summary>
        public AuctionsRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                AuctionsRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private AuctionsRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(AuctionsRow oValue, AuctionsRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            //if (nValue?.NewAuctionInfo == null || nValue.NewAuctionInfo.Item == null)
            //{
            //    //GameScene.Game.AuctionsBox.SellerLabelLabel.Text = "";
            //}
            //else
            //{
            //    //GameScene.Game.AuctionsBox.SellerLabelLabel.Text = nValue.NewAuctionInfo.OwnName;

            //    //CEnvir.Enqueue(new C.MarketPlaceHistory { Index = nValue.MarketInfo.Item.Info.Index, PartIndex = nValue.MarketInfo.Item.AddedStats[Stat.ItemIndex], Display = 1 });
            //}

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (SearchRows == null) return;
            ItemNameBox.TextBox.Text = "";
            //if (SearchResults == null)
            if (nValue)
                CEnvir.Enqueue(new C.AutionsFlash() { });
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 拍卖行
        /// </summary>
        public AuctionsDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(505, 440);
            Location = ClientArea.Location;
            SearchResults = new List<ClientNewAuction>();
            ItemResults = new List<ClientNewAuction>();
            SearchBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1873,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            IconControl = new DXControl
            {
                Parent = this,
                Size = new Size(108, 25),
                BackColour = Color.FromArgb(8, 8, 8),   //道具背景颜色
                Border = false,
                DrawTexture = true,
                IsControl = false,
                IsVisible = true,
                Location = new Point(365, 10),
            };

            Close1Button = new DXButton
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 7, Size.Height - 46);
            Close1Button.MouseClick += (o, e) => Visible = false;



            #region 拍卖行

            DXLabel label = new DXLabel
            {
                Parent = this,
                Location = new Point(220, 16),
                Text = "拍卖行".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };

            ItemNameLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "物品名称".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(37, 55),
            };

            BiddingPriceLabel = new DXLabel
            {
                Parent = this,
                Text = "起拍价（竞价）".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(140, 55),
            };

            FixedPriceLabel = new DXLabel
            {
                Parent = this,
                Text = "一口价".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(282, 55),
            };

            SellerLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "出售人".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(395, 55),
            };

            ItemNameBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(134, 19),
                Location = new Point(20, 379),
                Border = false,
            };
            ItemNameBox.TextBox.KeyPress += TextBox_KeyPress;

            ItemTypeBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(210, 370),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198,
                Visible = false,
            };

            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"全部".Lang() },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.Medicament; i++)
            {
                new DXListBoxItem
                {
                    Parent = ItemTypeBox.ListBox,
                    Label = { Text = i.Lang() },
                    Item = i
                };
            }
            ItemTypeBox.ListBox.SelectItem(null);

            SearchButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(162, 372),
                Parent = this,
                Hint = "以物品名称检索".Lang(),
                Opacity = 0F,
            };
            SearchButton.MouseEnter += (o, e) =>
            {
                SearchButton.Opacity = 1F;
            };
            SearchButton.MouseLeave += (o, e) =>
            {
                SearchButton.Opacity = 0F;
            };
            SearchButton.MouseClick += (o, e) => Search();

            ClearButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1898,
                Parent = this,
                Location = new Point(222, 370),
                Hint = "刷新".Lang(),
            };
            ClearButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.AutionsFlash() { });
            };

            SearchRows = new AuctionsRow[13];

            SearchScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(475, 48),
                Size = new Size(25, 287),
                VisibleSize = SearchRows.Length,
                Change = 3,
            };
            SearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;

            //为滚动条自定义皮肤 -1为不设置
            SearchScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            for (int i = 0; i < SearchRows.Length; i++)
            {
                int index = i;
                SearchRows[index] = new AuctionsRow
                {
                    Parent = this,
                    Location = new Point(15, 83 + i * 19),
                };
                SearchRows[index].MouseClick += (o, e) => { SelectedRow = SearchRows[index]; };
                SearchRows[index].MouseWheel += SearchScrollBar.DoMouseWheel;
                SearchRows[index].MouseEnter += (o, e) => { GameScene.Game.MouseItem = SearchRows[index].NewAuctionInfo.Item; };
                SearchRows[index].MouseLeave += (o, e) => { GameScene.Game.MouseItem = null; };
            }

            ConsignButton = new DXButton
            {
                Parent = this,
                Index = 1904,
                LibraryFile = LibraryFile.UI1,
                Hint = "登记物品",
                Location = new Point(305, 368),
            };
            ConsignButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Inventory.All(x => x == null || x.Info.ItemName != "拍卖入场卷"))   //如果 玩家背包库存为空 或者 道具不是拍卖入场劵
                {
                    GameScene.Game.ReceiveChat("你需要在商城购买一张拍卖入场卷，才能上架道具到拍卖行".Lang(), MessageType.System);   //提示并跳过
                    return;
                }
                else
                {
                    GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;
                    GameScene.Game.AuctionsConsignBox.Visible = !GameScene.Game.AuctionsConsignBox.Visible;
                }
            };

            AuctionBidButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1227,
                Location = new Point(355, 368),
                Hint = "竞拍出价".Lang(),
            };
            AuctionBidButton.MouseClick += AuctionBidButton_MouseClick;

            BuyButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1906,
                Location = new Point(405, 368),
                Hint = "一口价购买".Lang(),
            };
            BuyButton.MouseClick += BuyButton_MouseClick;

            #endregion

        }

        #region Methods



        /// <summary>
        /// 鼠标点击竞拍出价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuctionBidButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedRow?.NewAuctionInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = SelectedRow.NewAuctionInfo.Item.Info;

            if (SelectedRow.NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedRow.NewAuctionInfo.Item.AddedStats[Stat.ItemIndex]);

            message.Append($"竞拍出价".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

            if (SelectedRow.NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                message.Append(" - [" + "碎片".Lang() + "]");

            message.Append("\n");
            long newPrice = SelectedRow.NewAuctionInfo.LastPrice == 0 ? SelectedRow.NewAuctionInfo.Price : SelectedRow.NewAuctionInfo.LastPrice + SelectedRow.NewAuctionInfo.PriceAdd;

            message.Append($"当前最低竞价".Lang() + $": {(Convert.ToDecimal(newPrice) / 100).ToString("###0.00")}" + " 赞助币".Lang());

            new DXInputWindow((str) =>
            {
                double price = 0;
                bool isDigit = double.TryParse(str, out price);
                if (!isDigit) return isDigit;
                if (price < 0 || price < newPrice / 100)
                    return false;
                return true;
            },
                (str) =>
                {
                    double price = 0;
                    if (!double.TryParse(str, out price)) return;
                    if (price > 0 && price >= newPrice / 100)
                    {
                        CEnvir.Enqueue(new C.AutinsBuy { Index = SelectedRow.NewAuctionInfo.Index, BuyPrice = (int)price * 100 });
                        CEnvir.Enqueue(new C.AutionsFlash() { });
                    }
                }, message.ToString()
            );
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            //SearchResults = null;
            SearchResults.Clear();
            SearchScrollBar.MaxValue = 0;

            foreach (AuctionsRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            SearchScrollBar.MaxValue = 0;

            foreach (var row in SearchRows)
                row.Visible = true;

            // ItemType filter = (ItemType?)ItemTypeBox.SelectedItem ?? 0;
            //bool useFilter = ItemTypeBox.SelectedItem != null;

            foreach (var info in ItemResults)
            {

                if (!string.IsNullOrEmpty(ItemNameBox.TextBox.Text) && info.Item.Info.ItemName.IndexOf(ItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;
                SearchResults.Add(info);
            }

            RefreshList();
            //CEnvir.Enqueue(new C.AutionsFlash
            //{
            //  Name = ItemNameBox.TextBox.Text,
            //});
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList()
        {
            if (SearchResults == null) return;

            SearchScrollBar.MaxValue = SearchResults.Count;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                if (i + SearchScrollBar.Value >= SearchResults.Count)
                {
                    SearchRows[i].NewAuctionInfo = null;
                    SearchRows[i].Loading = false;
                    SearchRows[i].Visible = false;
                    continue;
                }

                if (SearchResults[i + SearchScrollBar.Value] == null)
                {
                    SearchRows[i].Loading = true;
                    SearchRows[i].Visible = true;
                    //SearchResults[i + SearchScrollBar.Value] = new ClientNewAuction { Loading = true };
                    //CEnvir.Enqueue(new C.AutionsFlashIndex { Index = i + SearchScrollBar.Value });
                    continue;
                }

                if (SearchResults[i + SearchScrollBar.Value].Loading) continue;

                SearchRows[i].Loading = false;
                SearchRows[i].NewAuctionInfo = SearchResults[i + SearchScrollBar.Value];
            }
        }

        /// <summary>
        /// 鼠标点击购买按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {

            if (SelectedRow?.NewAuctionInfo?.Item == null) return;

            if (SelectedRow.NewAuctionInfo.BuyItNowPrice == 0) return;

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = SelectedRow.NewAuctionInfo.Item.Info;

            if (SelectedRow.NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedRow.NewAuctionInfo.Item.AddedStats[Stat.ItemIndex]);

            message.Append($"购买物品".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

            if (SelectedRow.NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                message.Append(" - [" + "碎片".Lang() + "]");

            message.Append("\n\n");

            message.Append($"一口价购买".Lang() + $": {(Convert.ToDecimal(SelectedRow.NewAuctionInfo.BuyItNowPrice) / 100).ToString("###0.00")}" + " 赞助币".Lang());

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new C.AutinsBuy { Index = SelectedRow.NewAuctionInfo.Index, BuyPrice = SelectedRow.NewAuctionInfo.BuyItNowPrice });
                CEnvir.Enqueue(new C.AutionsFlash() { });
            });
        }

        /// <summary>
        /// 搜索滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        /// <summary>
        /// 文本输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (SearchButton.Enabled)
                Search();
        }

        /// <summary>
        /// 文本输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                #region Search

                if (SearchBackGround != null)
                {
                    if (!SearchBackGround.IsDisposed)
                        SearchBackGround.Dispose();

                    SearchBackGround = null;
                }

                if (IconControl != null)
                {
                    if (!IconControl.IsDisposed)
                        IconControl.Dispose();

                    IconControl = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }

                if (ConsignButton != null)
                {
                    if (!ConsignButton.IsDisposed)
                        ConsignButton.Dispose();

                    ConsignButton = null;
                }

                if (ItemNameBox != null)
                {
                    if (!ItemNameBox.IsDisposed)
                        ItemNameBox.Dispose();

                    ItemNameBox = null;
                }

                if (ItemTypeBox != null)
                {
                    if (!ItemTypeBox.IsDisposed)
                        ItemTypeBox.Dispose();

                    ItemTypeBox = null;
                }

                if (ClearButton != null)
                {
                    if (!ClearButton.IsDisposed)
                        ClearButton.Dispose();

                    ClearButton = null;
                }

                if (BuyButton != null)
                {
                    if (!BuyButton.IsDisposed)
                        BuyButton.Dispose();

                    BuyButton = null;
                }

                if (SearchButton != null)
                {
                    if (!SearchButton.IsDisposed)
                        SearchButton.Dispose();

                    SearchButton = null;
                }

                if (SearchScrollBar != null)
                {
                    if (!SearchScrollBar.IsDisposed)
                        SearchScrollBar.Dispose();

                    SearchScrollBar = null;
                }

                if (SellerLabelLabel != null)
                {
                    if (!SellerLabelLabel.IsDisposed)
                        SellerLabelLabel.Dispose();

                    SellerLabelLabel = null;
                }

                if (SellerLabelLabel != null)
                {
                    if (!SellerLabelLabel.IsDisposed)
                        SellerLabelLabel.Dispose();

                    SellerLabelLabel = null;
                }

                if (FixedPriceLabel != null)
                {
                    if (!FixedPriceLabel.IsDisposed)
                        FixedPriceLabel.Dispose();

                    FixedPriceLabel = null;
                }

                if (ItemNameLabelLabel != null)
                {
                    if (!ItemNameLabelLabel.IsDisposed)
                        ItemNameLabelLabel.Dispose();

                    ItemNameLabelLabel = null;
                }
                if (BiddingPriceLabel != null)
                {
                    if (!BiddingPriceLabel.IsDisposed)
                        BiddingPriceLabel.Dispose();

                    BiddingPriceLabel = null;
                }

                if (SearchRows != null)
                {
                    for (int i = 0; i < SearchRows.Length; i++)
                    {
                        if (SearchRows[i] != null)
                        {
                            if (!SearchRows[i].IsDisposed)
                                SearchRows[i].Dispose();

                            SearchRows[i] = null;
                        }
                    }
                    SearchRows = null;
                }

                SearchResults = null;
                #endregion

                _SelectedRow = null;
                SelectedRowChanged = null;
            }
        }

        #endregion
    }

    /// <summary>
    /// 拍卖内容条
    /// </summary>
    public sealed class AuctionsRow : DXControl
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
            BackColour = Selected ? Color.FromArgb(33, 117, 156) : Color.Empty;

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MarketInfo
        /// <summary>
        /// 拍卖行信息
        /// </summary>
        public ClientNewAuction NewAuctionInfo
        {
            get => _NewAuctionInfo;
            set
            {
                ClientNewAuction oldValue = _NewAuctionInfo;
                _NewAuctionInfo = value;

                OnNewAuctionInfoChanged(oldValue, value);
            }
        }
        private ClientNewAuction _NewAuctionInfo;
        public event EventHandler<EventArgs> NewAuctionInfoChanged;
        public void OnNewAuctionInfoChanged(ClientNewAuction oValue, ClientNewAuction nValue)
        {
            Visible = NewAuctionInfo != null;

            if (NewAuctionInfo == null) return;

            ItemInfo displayInfo = NewAuctionInfo.Item?.Info;

            if (NewAuctionInfo.Item != null && NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == NewAuctionInfo.Item.AddedStats[Stat.ItemIndex]);

            string name = displayInfo?.Lang(p => p.ItemName) ?? "商品已售出".Lang();

            if (NewAuctionInfo.Item != null && NewAuctionInfo.Item.Info.Effect == ItemEffect.ItemPart)
                name += " - [" + "碎片".Lang() + "]";

            NameLabel.Text = name;

            if (NewAuctionInfo.Item != null && NewAuctionInfo.Item.AddedStats.Count > 0)
                NameLabel.ForeColour = Color.FromArgb(148, 255, 206);
            else
                NameLabel.ForeColour = Color.White;

            BiddingPriceLabel.Text = NewAuctionInfo.Item == null ? "" : (Convert.ToDecimal(NewAuctionInfo.Price) / 100).ToString("###0") + " (" + (Convert.ToDecimal(NewAuctionInfo.PriceAdd) / 100).ToString("###0") + ")";

            FixedPriceLabel.Text = NewAuctionInfo.Item == null ? "" : (Convert.ToDecimal(NewAuctionInfo.BuyItNowPrice) / 100).ToString("###0");

            SellerLabel.Text = NewAuctionInfo.Item == null ? "" : NewAuctionInfo.OwnName;
            NewAuctionInfo.Item.LastName = NewAuctionInfo.LastName;
            NewAuctionInfo.Item.LastPrice = NewAuctionInfo.LastPrice;
            NewAuctionInfo.Item.Flags |= UserItemFlags.AutionItem;

            NewAuctionInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Loading
        /// <summary>
        /// 加载
        /// </summary>
        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading == value) return;

                bool oldValue = _Loading;
                _Loading = value;

                OnLoadingChanged(oldValue, value);
            }
        }
        private bool _Loading;
        public event EventHandler<EventArgs> LoadingChanged;
        public void OnLoadingChanged(bool oValue, bool nValue)
        {
            BiddingPriceLabel.Visible = !Loading;
            SellerLabel.Visible = !Loading;

            if (Loading)
            {
                NewAuctionInfo = null;
                NameLabel.Text = "加载中".Lang();
            }
            else
                NameLabel.Text = "";

            LoadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel NameLabel, BiddingPriceLabel, FixedPriceLabel, SellerLabel;

        #endregion

        /// <summary>
        /// 拍卖内容条
        /// </summary>
        public AuctionsRow()
        {
            Size = new Size(450, 19);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.Empty;

            Visible = false;

            NameLabel = new DXLabel   //道具名字
            {
                Parent = this,
                Location = new Point(5, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(100, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            BiddingPriceLabel = new DXLabel  //竞价
            {
                Parent = this,
                Location = new Point(115, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(123, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            FixedPriceLabel = new DXLabel  //一口价
            {
                Parent = this,
                Location = new Point(235, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(115, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            SellerLabel = new DXLabel   //出售人
            {
                Parent = this,
                Location = new Point(355, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(100, 20),
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
                _Selected = false;
                SelectedChanged = null;

                _NewAuctionInfo = null;
                NewAuctionInfoChanged = null;

                _Loading = false;
                LoadingChanged = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (BiddingPriceLabel != null)
                {
                    if (!BiddingPriceLabel.IsDisposed)
                        BiddingPriceLabel.Dispose();

                    BiddingPriceLabel = null;
                }

                if (FixedPriceLabel != null)
                {
                    if (!FixedPriceLabel.IsDisposed)
                        FixedPriceLabel.Dispose();

                    FixedPriceLabel = null;
                }

                if (SellerLabel != null)
                {
                    if (!SellerLabel.IsDisposed)
                        SellerLabel.Dispose();

                    SellerLabel = null;
                }
            }
        }
        #endregion
    }

    public sealed class AuctionsConsignDialog : DXWindow
    {
        #region Properites

        #region Consign
        public DXImageControl ConsignBackGround;

        public DXTextBox StartingPriceBox, BiddingPriceBox, FixedPriceBox, ConsignCostBox;
        public DXControl ConsignPanel, ConsignConfirmPanel;
        public DXButton Close1Button, ConsignButton, CancelButton;
        public DXItemGrid ConsignGrid;
        public DXLabel ConsignPriceLabel;
        public DXVScrollBar ConsignScrollBar;

        public MarketPlaceRow[] ConsignRows;
        #endregion

        #region Price
        /// <summary>
        /// 价格
        /// </summary>
        public long Price
        {
            get => _Price;
            set
            {
                if (_Price == value) return;

                long oldValue = _Price;
                _Price = value;

                OnPriceChanged(oldValue, value);
            }
        }
        private long _Price;
        public event EventHandler<EventArgs> PriceChanged;
        public void OnPriceChanged(long oValue, long nValue)
        {
            PriceChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 一口价
        /// </summary>
        public long BuyItNowPrice
        {
            get => _BuyItNowPrice;
            set
            {
                if (_BuyItNowPrice == value) return;

                long oldValue = _BuyItNowPrice;
                _BuyItNowPrice = value;

                OnBuyItNowPriceChanged(oldValue, value);
            }
        }
        private long _BuyItNowPrice;
        public event EventHandler<EventArgs> BuyItNowPriceChanged;
        public void OnBuyItNowPriceChanged(long oValue, long nValue)
        {
            BuyItNowPriceChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 增价
        /// </summary>
        public long PerAddPrice
        {
            get => _PerAddPrice;
            set
            {
                if (_PerAddPrice == value) return;

                long oldValue = _PerAddPrice;
                _PerAddPrice = value;

                OnPerAddPriceChanged(oldValue, value);
            }
        }
        private long _PerAddPrice;
        public event EventHandler<EventArgs> PerAddPriceChanged;
        public void OnPerAddPriceChanged(long oValue, long nValue)
        {
            PerAddPriceChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public int Cost => 0;

        public List<ClientMarketPlaceInfo> ConsignItems = new List<ClientMarketPlaceInfo>();

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!Visible)
            {
                ConsignGrid.ClearLinks();
                return;
            }
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public AuctionsConsignDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(245, 298);
            Location = ClientArea.Location;

            ConsignBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1874,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            Close1Button = new DXButton
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 8, Size.Height - 46);
            Close1Button.MouseClick += (o, e) => Visible = false;

            #region 拍卖委托

            DXLabel label = new DXLabel
            {
                Parent = this,
                Location = new Point(90, 16),
                Text = "拍卖品登记".Lang(),
                ForeColour = Color.White,
            };

            ConsignGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Location = new Point(34, 226),
                Parent = this,
                Border = false,
                Linked = true,
                GridType = GridType.Consign,
            };
            //ConsignGrid.Grid[0].LinkChanged += (o, e) =>
            //{
            //    if (ConsignGrid.Grid[0].Item == null)
            //    {
            //        ConsignGrid.Grid[0].LinkedCount = 0;
            //    }
            //    else
            //    {
            //        CEnvir.Enqueue(new C.MarketPlaceHistory { Index = ConsignGrid.Grid[0].Item.Info.Index, PartIndex = ConsignGrid.Grid[0].Item.AddedStats[Stat.ItemIndex], Display = 2 });
            //    }
            //};

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(35, 85),
                Text = "起拍价:".Lang(),
                ForeColour = Color.White,
            };
            StartingPriceBox = new DXTextBox
            {
                Location = new Point(85, 85),
                Size = new Size(130, 18),
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(82, 46, 24),
                BackColour = Color.Empty,
            };
            StartingPriceBox.TextBox.TextChanged += (o, e) =>
            {
                long price;
                long.TryParse(StartingPriceBox.TextBox.Text, out price);

                Price = price;
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(25, 115),
                Text = "竞拍加价:".Lang(),
                ForeColour = Color.White,
            };
            BiddingPriceBox = new DXTextBox
            {
                Location = new Point(85, 115),
                Size = new Size(130, 18),
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(82, 46, 24),
                BackColour = Color.Empty,
            };
            BiddingPriceBox.TextBox.TextChanged += (o, e) =>
            {
                long price;
                long.TryParse(BiddingPriceBox.TextBox.Text, out price);

                PerAddPrice = price;
            };

            label = new DXLabel
            {
                Parent = this,
                Location = new Point(35, 145),
                Text = "一口价:".Lang(),
                ForeColour = Color.White,
            };
            FixedPriceBox = new DXTextBox
            {
                Location = new Point(85, 145),
                Size = new Size(130, 18),
                Parent = this,
                Border = true,
                BorderColour = Color.FromArgb(82, 46, 24),
                BackColour = Color.Empty,
            };
            FixedPriceBox.TextBox.TextChanged += (o, e) =>
            {
                long price;
                long.TryParse(FixedPriceBox.TextBox.Text, out price);

                BuyItNowPrice = price;
            };

            ConsignButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1904,
                Parent = this,
                Location = new Point(105, 226),
                Hint = "登记".Lang(),
            };
            ConsignButton.MouseClick += ConsignButton_MouseClick;

            CancelButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1908,
                Parent = this,
                Location = new Point(168, 226),
                Hint = "取消".Lang(),
            };
            CancelButton.MouseClick += CancelButton_MouseClick;

            #endregion

        }

        /// <summary>
        /// 鼠标点击取消上架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell cell = ConsignGrid.Grid[0];

            if (cell.Item == null) return;

            cell.Link = null;

            StartingPriceBox.TextBox.Text = null;
            BiddingPriceBox.TextBox.Text = null;
            FixedPriceBox.TextBox.Text = null;
        }
        /// <summary>
        /// 鼠标点击寄售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsignButton_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell cell = ConsignGrid.Grid[0];

            if (cell.Item == null)
            {
                GameScene.Game.ReceiveChat("错误".Lang() + ":" + "没有选择任何物品".Lang(), MessageType.System);
                return;
            }

            if (Price <= 0)
            {
                GameScene.Game.ReceiveChat("错误".Lang() + ":" + "无效的价格".Lang(), MessageType.System);
                return;
            }

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = cell.Item.Info;

            if (cell.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == cell.Item.AddedStats[Stat.ItemIndex]);

            message.Append($"物品".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

            if (cell.Item.Info.Effect == ItemEffect.ItemPart)
                message.Append(" - [" + "碎片".Lang() + "]");

            //if (cell.LinkedCount > 1)
            //message.Append($" x{cell.LinkedCount:###0}");

            message.Append("\n\n");

            message.Append($"赞助币".Lang() + $": {Price:###0}");

            //if (cell.LinkedCount > 1)
            //message.Append(" (" + "每个".Lang() + ")");

            message.Append("\n\n");

            message.Append($"寄售费用".Lang() + $": {Cost:###0}");

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new C.AutionsAdd
                {
                    Link = new CellLinkInfo { GridType = cell.Link.GridType, Count = cell.LinkedCount, Slot = cell.Link.Slot },
                    Price = Price * 100,      //起拍价格
                    BuyItNowPrice = BuyItNowPrice * 100,   //一口价格
                    PerAddPrice = PerAddPrice * 100,     //加价的幅度               
                });

                cell.Link.Locked = true;
                cell.Link = null;
                FixedPriceBox.TextBox.Text = "";
            });
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ConsignBackGround != null)
                {
                    if (!ConsignBackGround.IsDisposed)
                        ConsignBackGround.Dispose();

                    ConsignBackGround = null;
                }

                #region Consign
                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }

                if (FixedPriceBox != null)
                {
                    if (!FixedPriceBox.IsDisposed)
                        FixedPriceBox.Dispose();

                    FixedPriceBox = null;
                }

                if (ConsignCostBox != null)
                {
                    if (!ConsignCostBox.IsDisposed)
                        ConsignCostBox.Dispose();

                    ConsignCostBox = null;
                }

                if (BiddingPriceBox != null)
                {
                    if (!BiddingPriceBox.IsDisposed)
                        BiddingPriceBox.Dispose();

                    BiddingPriceBox = null;
                }

                if (StartingPriceBox != null)
                {
                    if (!StartingPriceBox.IsDisposed)
                        StartingPriceBox.Dispose();

                    StartingPriceBox = null;
                }

                if (ConsignPanel != null)
                {
                    if (!ConsignPanel.IsDisposed)
                        ConsignPanel.Dispose();

                    ConsignPanel = null;
                }

                if (ConsignConfirmPanel != null)
                {
                    if (!ConsignConfirmPanel.IsDisposed)
                        ConsignConfirmPanel.Dispose();

                    ConsignConfirmPanel = null;
                }

                if (ConsignButton != null)
                {
                    if (!ConsignButton.IsDisposed)
                        ConsignButton.Dispose();

                    ConsignButton = null;
                }

                if (ConsignGrid != null)
                {
                    if (!ConsignGrid.IsDisposed)
                        ConsignGrid.Dispose();

                    ConsignGrid = null;
                }

                if (ConsignPriceLabel != null)
                {
                    if (!ConsignPriceLabel.IsDisposed)
                        ConsignPriceLabel.Dispose();

                    ConsignPriceLabel = null;
                }

                if (ConsignScrollBar != null)
                {
                    if (!ConsignScrollBar.IsDisposed)
                        ConsignScrollBar.Dispose();

                    ConsignScrollBar = null;
                }

                if (ConsignRows != null)
                {
                    for (int i = 0; i < ConsignRows.Length; i++)
                    {
                        if (ConsignRows[i] != null)
                        {
                            if (!ConsignRows[i].IsDisposed)
                                ConsignRows[i].Dispose();

                            ConsignRows[i] = null;
                        }
                    }
                    ConsignRows = null;
                }
                #endregion

                _Price = 0;
                PriceChanged = null;

                ConsignItems = null;
            }
        }
        #endregion
    }
}