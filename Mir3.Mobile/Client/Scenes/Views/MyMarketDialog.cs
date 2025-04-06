using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class MyMarketDialog : DXWindow
    {
        #region Properites

        #region Consign
        public DXImageControl ConsignBackGround;

        public DXComboBox ConsignPriceLabelBox;

        public DXButton Close1Button, CancelButton, NextButton, PreviousButton, ClearButton;

        public DXVScrollBar ConsignScrollBar;

        public DXLabel PageValue;

        public MarketConsignRow[] ConsignRows;
        #endregion

        public int Cost => 0;

        public List<ClientMarketPlaceInfo> ConsignItems = new List<ClientMarketPlaceInfo>();

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

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        public MyMarketDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;

            Size = new Size(405, 373);
            Location = ClientArea.Location;

            ConsignBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1891,
                IsControl = true,
                PassThrough = true,
                Location = new Point(0, 0)
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

            NextButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1896,
                Parent = this,
                Location = new Point(132, 301),
                Hint = "下一页".Lang(),
            };
            NextButton.MouseClick += NextButton_MouseClick;

            PreviousButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1894,
                Parent = this,
                Location = new Point(62, 302),
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
                Location = new Point(85, 10),
            };

            ClearButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1898,
                Parent = this,
                Location = new Point(202, 303),
                Hint = "刷新".Lang(),
            };
            ClearButton.MouseClick += (o, e) =>
            {
                RefreshConsignList();
            };
            #region 委托

            ConsignRows = new MarketConsignRow[10];
            for (int i = 0; i < ConsignRows.Length; i++)
            {
                int index = i;
                ConsignRows[index] = new MarketConsignRow
                {
                    Parent = this,
                    Location = new Point(7, 77 + index * 19),
                };
                ConsignRows[index].MouseClick += (o, e) =>
                {
                    ClientMarketPlaceInfo info = ConsignRows[index].MarketInfo;

                    if (info == null) return;

                    StringBuilder message = new StringBuilder();

                    ItemInfo displayInfo = info.Item.Info;

                    if (info.Item.Info.Effect == ItemEffect.ItemPart)
                        displayInfo = Globals.ItemInfoList.Binding.First(x => x.Index == info.Item.AddedStats[Stat.ItemIndex]);

                    message.Append($"取消寄售".Lang() + $": {displayInfo.Lang(p => p.ItemName)}");

                    if (info.Item.Info.Effect == ItemEffect.ItemPart)
                        message.Append(" - [" + "碎片".Lang() + "]");

                    new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
                    {
                        CEnvir.Enqueue(new C.MarketPlaceCancelConsign { Index = info.Index, Count = info.Item.Count });
                    });
                };
                ConsignRows[index].MouseEnter += (o, e) =>
                {
                    GameScene.Game.MouseItem = ConsignRows[index].MarketInfo?.Item;
                };
                ConsignRows[index].MouseLeave += (o, e) =>
                {
                    GameScene.Game.MouseItem = null;
                };
            }

            ConsignScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(453, 75),
                Size = new Size(14, 200),
                VisibleSize = ConsignRows.Length,
                Change = 3,
            };
            ConsignScrollBar.ValueChanged += ConsignScrollBar_ValueChanged;

            #endregion
        }

        /// <summary>
        /// 寄售滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConsignScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshConsignList();
        }
        /// <summary>
        /// 刷新寄售列表
        /// </summary>
        public void RefreshConsignList()
        {
            ConsignScrollBar.MaxValue = ConsignItems.Count;

            for (int i = 0; i < ConsignRows.Length; i++)
            {
                if (i + ConsignScrollBar.Value >= ConsignItems.Count)
                {
                    ConsignRows[i].MarketInfo = null;
                    ConsignRows[i].Visible = false;
                    continue;
                }

                if (ConsignItems[i + ConsignScrollBar.Value].Loading) continue;

                ConsignRows[i].MarketInfo = ConsignItems[i + ConsignScrollBar.Value];
            }
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

        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList(int page = 1)
        {
            if (ConsignItems == null && page > TotalPage) return;

            var Value = (page - 1) * 10;

            for (int i = 0; i < ConsignRows.Length; i++)
            {
                if (i + Value >= ConsignItems.Count)
                {
                    ConsignRows[i].MarketInfo = null;
                    ConsignRows[i].Loading = false;
                    ConsignRows[i].Visible = false;
                    continue;
                }

                if (ConsignItems[i + Value] == null)
                {
                    ConsignRows[i].Loading = true;
                    ConsignRows[i].Visible = true;
                    continue;
                }

                ConsignRows[i].Loading = false;
                ConsignRows[i].MarketInfo = ConsignItems[i + Value];
            }
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

                ConsignItems = null;
            }
        }
        #endregion
    }


    public sealed class MarketConsignRow : MarketPlaceRow
    {
        protected override void LastValue()
        {
            SellerLabel.ForeColour = Color.White;
            SellerLabel.Text = MarketInfo.Item == null ? "" : MarketInfo.CreatTime.ToString("yyyy年MM月dd日");
            SellerLabel.Location = new Point(225, -2);
            SellerLabel.Size = new Size(140, 20);
            PriceLabel.ForeColour = Color.White;
            PriceLabel.Location = new Point(120, -2);
            PriceLabel.Size = new Size(135, 20);
            if (MarketInfo.PriceType == CurrencyType.GameGold)
            {
                PriceLabel.Text += " 赞助币".Lang();
            }
            else
            {
                PriceLabel.Text += " 金币".Lang();
            }
        }
    }
}
