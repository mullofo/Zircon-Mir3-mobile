using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 金币寄售
    /// </summary>
    public sealed class MarketSearchDialog : DXWindow
    {
        #region Properites

        #region Search
        public DXImageControl SearchBackGround;
        public DXControl IconControl;
        public DXAnimatedControl IconAnimation;
        public DXTextBox ItemNameBox, NameBox;
        public DXComboBox ItemTypeBox;
        public DXButton ItemTypeSearchButton;
        public DXButton BuyButton, SearchButton, NameSearchButton, ClearButton, Close1Button, ConsignButton, NextButton, PreviousButton, CancelButton;
        public DXLabel MessageLabel, ItemNameLabelLabel, PriceLabelLabel, SellerLabelLabel, PageValue;
        public DXVScrollBar SearchScrollBar;

        public MarketPlaceRow[] SearchRows;
        public ClientMarketPlaceInfo[] SearchResults;
        public DXItemCell ItemCell;
        public CurrencyType DlgType;
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

        #region SelectedRow

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);
            if (nValue)
            {
                Search();
                PriceLabelLabel.Text = DlgType == CurrencyType.Gold ? " 金币" : "赞助币";
            }
        }
        /// <summary>
        /// 选定行
        /// </summary>
        public MarketPlaceRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                MarketPlaceRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private MarketPlaceRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(MarketPlaceRow oValue, MarketPlaceRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            if (nValue?.MarketInfo == null || nValue.MarketInfo.Item == null || nValue.MarketInfo.PriceType != DlgType)
            {
                GameScene.Game.MarketSearchBox.SellerLabelLabel.Text = "";
                MessageLabel.Text = "";
                ItemCell.Visible = false;
            }
            else
            {
                GameScene.Game.MarketSearchBox.SellerLabelLabel.Text = nValue.MarketInfo.Seller;

                //PriceLabelLabel.Text = nValue.MarketInfo.PriceType.Lang();
                PriceLabelLabel.ForeColour = Color.White;

                MessageLabel.Text = nValue.MarketInfo.Message;

                ItemCell.Visible = true;
                ItemCell.Item = nValue.MarketInfo.Item;
                ItemCell.RefreshItem();

                CEnvir.Enqueue(new C.MarketPlaceHistory { Index = nValue.MarketInfo.Item.Info.Index, PartIndex = nValue.MarketInfo.Item.AddedStats[Stat.ItemIndex], Display = 1 });
            }

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (SearchRows == null) return;

            if (SearchResults == null)
                Search();
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 金币寄售
        /// </summary>
        public MarketSearchDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(720, 364);
            Location = ClientArea.Location;

            SearchBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1091,
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

            IconAnimation = new DXAnimatedControl
            {
                Parent = IconControl,
                BaseIndex = 1920,
                LibraryFile = LibraryFile.UI1,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(2),
                FrameCount = 10,
                Loop = true,
                Location = new Point(25, 2),
            };

            Close1Button = new DXButton
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 8, Size.Height - 45);
            Close1Button.MouseClick += (o, e) => Visible = false;

            ConsignButton = new DXButton
            {
                Parent = this,
                Index = 1904,
                LibraryFile = LibraryFile.UI1,
                Hint = "登记物品",
                Location = new Point(513, 292),
            };
            ConsignButton.MouseClick += (o, e) =>
            {
                if (DlgType == CurrencyType.Gold)
                {
                    GameScene.Game.MarketConsignBox.Visible = !GameScene.Game.MarketConsignBox.Visible;
                }
                else if (DlgType == CurrencyType.GameGold)
                {
                    GameScene.Game.GameGoldMarketConsignBox.Visible = !GameScene.Game.GameGoldMarketConsignBox.Visible;
                }
                if (GameScene.Game.InventoryBox.Visible == true)
                {
                    GameScene.Game.InventoryBox.Visible = false;
                    GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;
                }
                else
                {
                    GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;
                }
            };

            #region 寄售

            DXLabel label = new DXLabel
            {
                Parent = this,
                Location = new Point(27, 16),
                Text = "[购买物品]".Lang(),
                ForeColour = Color.White,
            };

            ItemNameLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "物品名称".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(60, 55),
            };

            PriceLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "金币".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(226, 55),
            };

            SellerLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "出售人".Lang(),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(370, 55),
            };

            ItemNameBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(134, 19),
                Location = new Point(269, 289),
                Border = false,
            };
            ItemNameBox.TextBox.KeyPress += TextBox_KeyPress;

            ItemTypeBox = new DXComboBox
            {
                Parent = this,
                Location = new Point(22, 319),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 140,
                SelectedLabel = { BackColour = Color.Black },
                //Visible = false,
            };

            new DXListBoxItem
            {
                Parent = ItemTypeBox.ListBox,
                Label = { Text = $"全部".Lang() },
                Item = null
            };
            //只取道具数据库有的类型
            List<ItemType> itemType = Globals.ItemInfoList.Binding.Select(x => x.ItemType).Distinct().ToList();
            foreach (ItemType type in itemType)
            {
                new DXListBoxItem
                {
                    Parent = ItemTypeBox.ListBox,
                    Label = { Text = type.Lang() },
                    Item = type
                };
            }
            ItemTypeBox.ListBox.SelectItem(null);

            ItemTypeSearchButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(ItemTypeBox.Location.X + ItemTypeBox.Size.Width + 20, ItemTypeBox.Location.Y - 5),
                Parent = this,
                Hint = "物品类型检索".Lang(),
            };
            ItemTypeSearchButton.MouseClick += (o, e) =>
            {
                if (ItemTypeBox.SelectedItem == null)
                {
                    ItemNameBox.TextBox.Text = "";
                    Search();
                }
                else
                    Search((ItemType)ItemTypeBox.SelectedItem);
            };

            SearchButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(412, 282),
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

            NameBox = new DXTextBox
            {
                Parent = this,
                Size = new Size(134, 19),
                Location = new Point(269, 319),
                Border = false,
            };
            NameBox.TextBox.KeyPress += NameTextBox_KeyPress;

            NameSearchButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1900,
                Location = new Point(412, 312),
                Parent = this,
                Hint = "以物品持有人检索".Lang(),
                Opacity = 0F,
            };
            NameSearchButton.MouseEnter += (o, e) =>
            {
                NameSearchButton.Opacity = 1F;
            };
            NameSearchButton.MouseLeave += (o, e) =>
            {
                NameSearchButton.Opacity = 0F;
            };
            NameSearchButton.MouseClick += (o, e) => NameSearch();

            ClearButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1898,
                Parent = this,
                Location = new Point(122, 283),
                Hint = "刷新".Lang(),
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemNameBox.TextBox.Text = "";
                ItemTypeBox.ListBox.SelectItem(null);
                Search();
            };

            SearchRows = new MarketPlaceRow[5];

            SearchScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(475, 55),
                Size = new Size(14, 295),
                VisibleSize = SearchRows.Length,
                Change = 3,
                Visible = false,
            };
            //SearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                var row = new MarketPlaceRow
                {
                    Parent = this,
                    Location = new Point(15, 88 + i * 34),
                };
                row.MouseClick += (o, e) => { SelectedRow = row; };
                //row.FreeDrag += SearchScrollBar_DoFreeDrage;
                row.MouseEnter += (o, e) => { GameScene.Game.MouseItem = row.MarketInfo?.Item; };
                row.MouseLeave += (o, e) => { GameScene.Game.MouseItem = null; };
                SearchRows[i] = row;
            }

            NextButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1896,
                Parent = this,
                Location = new Point(72, 281),
                Hint = "下一页".Lang(),
            };
            NextButton.MouseClick += NextButton_MouseClick;

            PreviousButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1894,
                Parent = this,
                Location = new Point(22, 282),
                Hint = "上一页".Lang(),
            };
            PreviousButton.MouseClick += PreviousButton_MouseClick;

            PageValue = new DXLabel
            {
                Parent = this,
                IsControl = false,
                AutoSize = false,
                Size = new Size(240, 25),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Location = new Point(120, 10),
            };

            MessageLabel = new DXLabel
            {
                Location = new Point(493, 56),
                Parent = this,
                Size = new Size(218, 110),
                AutoSize = false,
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                ForeColour = Color.White,
            };

            BuyButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1906,
                Location = new Point(563, 292),
                Hint = "购买物品".Lang(),
            };
            BuyButton.MouseClick += BuyButton_MouseClick;

            SellerLabelLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                AutoSize = false,
                Size = new Size(140, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(490, 20),
            };

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point(577, 188),
                FixedBorder = true,
                Border = true,
                BackColour = Color.FromArgb(33, 33, 33),
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                Visible = false,
            };

            CancelButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1908,
                Location = new Point(643, 292),
                Hint = "登记取消".Lang(),
            };
            CancelButton.MouseClick += CancelButton_MouseClick;

            #endregion

        }

        #region Methods

        /// <summary>
        /// 鼠标点击取消寄售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedRow?.MarketInfo?.Item == null) return;

            ClientMarketPlaceInfo info = SelectedRow?.MarketInfo;

            if (info == null) return;

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = SelectedRow.MarketInfo.Item.Info;

            if (SelectedRow.MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedRow.MarketInfo.Item.AddedStats[Stat.ItemIndex]);

            message.Append($"取消寄售".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

            if (SelectedRow.MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                message.Append(" - [" + "碎片".Lang() + "]");

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new C.MarketPlaceCancelConsign { Index = info.Index, Count = info.Item.Count });
                Search();
            });
        }

        /// <summary>
        /// 分类搜索
        /// </summary>
        public void Search(ItemType itemType)
        {
            SearchResults = null;

            SearchScrollBar.MaxValue = 0;

            foreach (MarketPlaceRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            CEnvir.Enqueue(new C.MarketPlaceSearch
            {
                //Name = ItemNameBox.TextBox.Text,
                PriceType = DlgType,
                ItemTypeFilter = true,
                ItemType = itemType,
            });
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void Search()
        {
            SearchResults = null;

            SearchScrollBar.MaxValue = 0;

            foreach (MarketPlaceRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            CEnvir.Enqueue(new C.MarketPlaceSearch
            {
                Name = ItemNameBox.TextBox.Text,
                PriceType = DlgType,
            });
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void NameSearch()
        {
            SearchResults = null;

            SearchScrollBar.MaxValue = 0;

            foreach (MarketPlaceRow row in SearchRows)
            {
                row.Loading = true;
                row.Visible = true;
            }

            CEnvir.Enqueue(new C.MarketPlaceSearch
            {
                SellName = NameBox.TextBox.Text,
                PriceType = DlgType,
            });
        }

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList(int page = 1)
        {
            if (SearchResults == null && page > TotalPage) return;

            //SearchScrollBar.MaxValue = SearchResults.Length;
            //SearchScrollBar.MaxValue = SearchResults.Count()-(page-1)*13>13?13:SearchResults.Count() - (page - 1) * 13;
            var Value = (page - 1) * 5;

            for (int i = 0; i < SearchRows.Length; i++)
            {
                //SearchRows[i].MarketInfo = null;
                //SearchRows[i].Loading = false;
                //SearchRows[i].Visible = false;
                if (i + Value >= SearchResults.Length)
                {
                    SearchRows[i].MarketInfo = null;
                    SearchRows[i].Loading = false;
                    SearchRows[i].Visible = false;
                    continue;
                }

                if (SearchResults[i + Value] == null)
                {
                    SearchRows[i].Loading = true;
                    SearchRows[i].Visible = true;
                    SearchResults[i + Value] = new ClientMarketPlaceInfo { Loading = true };
                    CEnvir.Enqueue(new C.MarketPlaceSearchIndex { Index = i + Value });
                    continue;
                }

                if (SearchResults[i + Value].Loading) continue;

                SearchRows[i].Loading = false;
                SearchRows[i].MarketInfo = SearchResults[i + Value];
            }
        }

        /// <summary>
        /// 鼠标点击购买按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedRow?.MarketInfo?.Item == null) return;

            StringBuilder message = new StringBuilder();

            ItemInfo displayInfo = SelectedRow.MarketInfo.Item.Info;

            if (SelectedRow.MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == SelectedRow.MarketInfo.Item.AddedStats[Stat.ItemIndex]);

            message.Append($"购买物品".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

            if (SelectedRow.MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                message.Append(" - [" + "碎片".Lang() + "]");

            message.Append("\n\n");

            if (DlgType == CurrencyType.Gold)
            {
                message.Append($"总价".Lang() + $": {SelectedRow.MarketInfo.Price.ToString("###0")}" + " 金币".Lang());
            }
            else if (DlgType == CurrencyType.GameGold)
            {
                message.Append($"总价".Lang() + $": {(Convert.ToDecimal(SelectedRow.MarketInfo.Price) / 100).ToString("###0.00")}" + " 赞助币".Lang());
            }

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                BuyButton.Enabled = false;

                CEnvir.Enqueue(new C.MarketPlaceBuy { Index = SelectedRow.MarketInfo.Index, Count = 1 });
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

            if (NameSearchButton.Enabled)
                NameSearch();
        }

        private void NextButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage + 1 > TotalPage) return;
            CurrentPage += 1;
            RefreshList(CurrentPage);
        }

        private void PreviousButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentPage - 1 < 1) return;
            CurrentPage -= 1;
            RefreshList(CurrentPage);
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

                if (IconAnimation != null)
                {
                    if (!IconAnimation.IsDisposed)
                        IconAnimation.Dispose();

                    IconAnimation = null;
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

                if (NameBox != null)
                {
                    if (!NameBox.IsDisposed)
                        NameBox.Dispose();

                    NameBox = null;
                }

                if (ItemTypeBox != null)
                {
                    if (!ItemTypeBox.IsDisposed)
                        ItemTypeBox.Dispose();

                    ItemTypeBox = null;
                }

                if (ItemTypeSearchButton != null)
                {
                    if (!ItemTypeSearchButton.IsDisposed)
                        ItemTypeSearchButton.Dispose();

                    ItemTypeSearchButton = null;
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

                if (NameSearchButton != null)
                {
                    if (!NameSearchButton.IsDisposed)
                        NameSearchButton.Dispose();

                    NameSearchButton = null;
                }

                if (NextButton != null)
                {
                    if (!NextButton.IsDisposed)
                        NextButton.Dispose();

                    NextButton = null;
                }

                if (PreviousButton != null)
                {
                    if (!PreviousButton.IsDisposed)
                        PreviousButton.Dispose();

                    PreviousButton = null;
                }

                if (PageValue != null)
                {
                    if (!PageValue.IsDisposed)
                        PageValue.Dispose();

                    PageValue = null;
                }

                if (MessageLabel != null)
                {
                    if (!MessageLabel.IsDisposed)
                        MessageLabel.Dispose();

                    MessageLabel = null;
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

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (SellerLabelLabel != null)
                {
                    if (!SellerLabelLabel.IsDisposed)
                        SellerLabelLabel.Dispose();

                    SellerLabelLabel = null;
                }

                if (PriceLabelLabel != null)
                {
                    if (!PriceLabelLabel.IsDisposed)
                        PriceLabelLabel.Dispose();

                    PriceLabelLabel = null;
                }

                if (ItemNameLabelLabel != null)
                {
                    if (!ItemNameLabelLabel.IsDisposed)
                        ItemNameLabelLabel.Dispose();

                    ItemNameLabelLabel = null;
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
    /// 寄售内容条
    /// </summary>
    public class MarketPlaceRow : DXControl
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

            GameScene.Game.MarketSearchBox.ItemCell.BorderColour = Selected ? Color.FromArgb(90, 90, 90) : Color.FromArgb(90, 90, 90);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MarketInfo
        /// <summary>
        /// 寄售信息
        /// </summary>
        public ClientMarketPlaceInfo MarketInfo
        {
            get => _MarketInfo;
            set
            {
                ClientMarketPlaceInfo oldValue = _MarketInfo;
                _MarketInfo = value;

                OnMarketInfoChanged(oldValue, value);
            }
        }
        private ClientMarketPlaceInfo _MarketInfo;
        public event EventHandler<EventArgs> MarketInfoChanged;
        public void OnMarketInfoChanged(ClientMarketPlaceInfo oValue, ClientMarketPlaceInfo nValue)
        {
            Visible = MarketInfo != null;

            if (MarketInfo == null) return;

            ItemInfo displayInfo = MarketInfo.Item?.Info;

            if (MarketInfo.Item != null && MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == MarketInfo.Item.AddedStats[Stat.ItemIndex]);

            string name = displayInfo?.Lang(p => p.ItemName) ?? "商品已售出".Lang();

            if (MarketInfo.Item != null && MarketInfo.Item.Info.Effect == ItemEffect.ItemPart)
                name += " - [" + "碎片".Lang() + "]";

            NameLabel.Text = name;

            if (MarketInfo.Item != null && MarketInfo.Item.AddedStats.Count > 0)
                NameLabel.ForeColour = Color.FromArgb(148, 255, 206);
            else
                NameLabel.ForeColour = Color.White;

            if (GameScene.Game.MarketSearchBox.DlgType == CurrencyType.Gold)
            {
                PriceLabel.Text = MarketInfo.Item == null ? "" : (Convert.ToDecimal(MarketInfo.Price)).ToString("#,##0");
                if (MarketInfo.Price > 10000000)
                {
                    PriceLabel.ForeColour = Color.Red;
                }
                else if (MarketInfo.Price > 1000000)
                {
                    PriceLabel.ForeColour = Color.Orange;
                }
                else if (MarketInfo.Price > 100000)
                {
                    PriceLabel.ForeColour = Color.DeepSkyBlue;
                }
                else
                {
                    PriceLabel.ForeColour = Color.White;
                }
            }
            else
                PriceLabel.Text = MarketInfo.Item == null ? "" : (Convert.ToDecimal(MarketInfo.Price) / 100).ToString("###0");

            SellerLabel.Text = MarketInfo.Item == null ? "" : MarketInfo.Seller;

            SellerLabel.ForeColour = MarketInfo.IsOwner ? Color.Yellow : Color.White;

            LastValue();

            if (GameScene.Game.MarketSearchBox.SelectedRow == this)
                GameScene.Game.MarketSearchBox.SelectedRow = null;

            MarketInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void LastValue()
        {
            SellerLabel.Text = MarketInfo.Item == null ? "" : MarketInfo.Seller;

            SellerLabel.ForeColour = MarketInfo.IsOwner ? Color.Yellow : Color.White;
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
            PriceLabel.Visible = !Loading;
            SellerLabel.Visible = !Loading;

            if (Loading)
            {
                MarketInfo = null;
                NameLabel.Text = "加载中".Lang();
            }
            else
                NameLabel.Text = "";

            if (GameScene.Game.MarketSearchBox?.SelectedRow == this)
                GameScene.Game.MarketSearchBox.SelectedRow = null;

            LoadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel NameLabel, PriceLabel, SellerLabel;

        #endregion

        /// <summary>
        /// 寄售内容条
        /// </summary>
        public MarketPlaceRow()
        {

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.Empty;

            Visible = false;

            var font = new Font(Config.FontName, CEnvir.FontSize(11F));
            Size = new Size(450, TextRenderer.MeasureText(null, "字", font).Height);

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(0, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(160, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = font,
            };

            PriceLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(162, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(145, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
                Font = font,
            };

            SellerLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(310, -2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(130, Size.Height),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = font,
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

                _MarketInfo = null;
                MarketInfoChanged = null;

                _Loading = false;
                LoadingChanged = null;

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

}