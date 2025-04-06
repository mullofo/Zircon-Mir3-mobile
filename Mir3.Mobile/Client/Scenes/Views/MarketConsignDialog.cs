using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    public sealed class MarketConsignDialog : DXWindow
    {
        #region Properites

        #region Consign
        public DXImageControl ConsignBackGround;

        public DXComboBox ConsignPriceLabelBox;

        public DXTextBox ConsignPriceBox, ConsignCostBox, ConsignMessageBox;
        public DXControl ConsignPanel, ConsignBuyPanel, ConsignConfirmPanel;
        public DXButton Close1Button, ConsignButton, CancelButton;
        public DXItemGrid ConsignGrid;
        public DXLabel ConsignPriceLabel, Explain;
        public DXVScrollBar ConsignScrollBar;

        #endregion

        #region Price
        /// <summary>
        /// 价格
        /// </summary>
        public int Price
        {
            get => _Price;
            set
            {
                if (_Price == value) return;

                int oldValue = _Price;
                _Price = value;

                OnPriceChanged(oldValue, value);
            }
        }
        private int _Price;
        public event EventHandler<EventArgs> PriceChanged;
        public void OnPriceChanged(int oValue, int nValue)
        {
            //ConsignCostBox.TextBox.Text = Cost.ToString("###0");

            PriceChanged?.Invoke(this, EventArgs.Empty);
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

        /// <summary>
        /// 金币寄售上架
        /// </summary>
        public MarketConsignDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(410, 285);
            Location = ClientArea.Location;

            ConsignBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1910,
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
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 10, Size.Height - 45);
            Close1Button.MouseClick += (o, e) => Visible = false;

            #region 委托

            DXLabel label = new DXLabel
            {
                Parent = this,
                Location = new Point(52, 16),
                Text = "[物品登记]".Lang(),
                ForeColour = Color.White,
            };

            DXControl consignPanel = new DXControl
            {
                Parent = this,
                Size = new Size(154, 160),
                Location = new Point(242, 45),
                Border = false,
            };

            Explain = new DXLabel
            {
                Parent = consignPanel,
                Location = new Point(40, 20),
                Text = "要登记的物品".Lang() + "\n" + " 请放在这里".Lang(),
            };

            ConsignGrid = new DXItemGrid
            {
                GridSize = new Size(1, 1),
                Location = new Point(60, 60),
                Parent = consignPanel,
                //Border = false,
                Linked = true,
                GridType = GridType.Consign,
            };

            ConsignGrid.Grid[0].LinkChanged += (o, e) =>
            {
                if (ConsignGrid.Grid[0].Item == null)
                {
                    ConsignGrid.Grid[0].LinkedCount = 0;
                    Explain.Visible = true;
                }
                else
                {
                    Explain.Visible = false;
                    CEnvir.Enqueue(new C.MarketPlaceHistory { Index = ConsignGrid.Grid[0].Item.Info.Index, PartIndex = ConsignGrid.Grid[0].Item.AddedStats[Stat.ItemIndex], Display = 2 });
                }
                //ConsignCostBox.TextBox.Text = Cost.ToString("###0");
            };

            ConsignBuyPanel = new DXControl
            {
                Parent = this,
                Size = new Size(175, 30),
                Location = new Point(10, consignPanel.Location.Y + consignPanel.Size.Height + 5),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Visible = false,
            };

            ConsignPriceLabelBox = new DXComboBox
            {
                Parent = ConsignBuyPanel,
                Location = new Point(5, 7),
                Size = new Size(70, DXComboBox.DefaultNormalHeight),
            };

            new DXListBoxItem
            {
                Parent = ConsignPriceLabelBox.ListBox,
                Label = { Text = CurrencyType.Gold.Lang() },
                Item = CurrencyType.Gold,
            };

            new DXListBoxItem
            {
                Parent = ConsignPriceLabelBox.ListBox,
                Label = { Text = CurrencyType.GameGold.Lang() },
                Item = CurrencyType.GameGold,
            };
            ConsignPriceLabelBox.ListBox.SelectItem(CurrencyType.Gold);

            ConsignPriceBox = new DXTextBox
            {
                Location = new Point(60, 238),
                Size = new Size(80, 18),
                Parent = this,
                Border = false,
                BackColour = Color.Empty,
            };
            ConsignPriceBox.TextBox.TextChanged += (o, e) =>
            {
                int price;
                int.TryParse(ConsignPriceBox.TextBox.Text, out price);

                Price = price;
            };

            ConsignMessageBox = new DXTextBox
            {
                Location = new Point(10, 70),
                Parent = this,
                TextBox = { Multiline = true, AcceptsReturn = true, },
                Size = new Size(223, 115),
                MaxLength = 220,
                Border = false,
                Opacity = 0.5F,
            };

            ConsignButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1904,
                Parent = this,
                Location = new Point(248, 227),
                Hint = "登记".Lang(),
            };
            ConsignButton.MouseClick += ConsignButton_MouseClick;

            CancelButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1908,
                Parent = this,
                Location = new Point(317, 227),
                Hint = "取消".Lang(),
            };
            CancelButton.MouseClick += CancelButton_MouseClick;

            #endregion

        }

        /// <summary>
        /// 鼠标点击取消寄售
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell cell = ConsignGrid.Grid[0];

            if (cell.Item == null) return;

            cell.Link = null;
            ConsignPriceBox.TextBox.Text = null;
            ConsignMessageBox.TextBox.Text = null;
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

            message.Append($"金币".Lang() + $": {Price:###0}");

            //if (cell.LinkedCount > 1)
            //message.Append(" (" + "每个".Lang() + ")");

            message.Append("\n\n");

            message.Append($"寄售费用".Lang() + $": {Cost:###0}");

            new DXConfirmWindow(message.ToString(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new C.MarketPlaceConsign
                {
                    Link = new CellLinkInfo { GridType = cell.Link.GridType, Count = cell.LinkedCount, Slot = cell.Link.Slot },
                    Price = Price,
                    //PriceType = (CurrencyType)ConsignPriceLabelBox.SelectedItem,
                    PriceType = CurrencyType.Gold,
                    Message = ConsignMessageBox.TextBox.Text,
                });

                cell.Link.Locked = true;
                cell.Link = null;
                ConsignPriceBox.TextBox.Text = "";
                GameScene.Game.MarketSearchBox.Search();
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

                if (ConsignPriceBox != null)
                {
                    if (!ConsignPriceBox.IsDisposed)
                        ConsignPriceBox.Dispose();

                    ConsignPriceBox = null;
                }

                if (ConsignCostBox != null)
                {
                    if (!ConsignCostBox.IsDisposed)
                        ConsignCostBox.Dispose();

                    ConsignCostBox = null;
                }

                if (Explain != null)
                {
                    if (!Explain.IsDisposed)
                        Explain.Dispose();

                    Explain = null;
                }

                if (ConsignMessageBox != null)
                {
                    if (!ConsignMessageBox.IsDisposed)
                        ConsignMessageBox.Dispose();

                    ConsignMessageBox = null;
                }

                if (ConsignPanel != null)
                {
                    if (!ConsignPanel.IsDisposed)
                        ConsignPanel.Dispose();

                    ConsignPanel = null;
                }

                if (ConsignBuyPanel != null)
                {
                    if (!ConsignBuyPanel.IsDisposed)
                        ConsignBuyPanel.Dispose();

                    ConsignBuyPanel = null;
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
                #endregion

                _Price = 0;
                PriceChanged = null;

                ConsignItems = null;
            }
        }
        #endregion
    }
}
