using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 金币交易行
    /// </summary>
    public sealed class GoldTradingBusinessDialog : DXWindow
    {
        #region Properties

        public DXImageControl GoldTradingBusinessBackGround, OrderBackGround;
        public DXButton Close1Button, Transaction, Order, TransactionRefresh, OrderRefresh, RechargeButton, Purchase, SellOut, OrderToBuyButton, OrderAndSellButton, RevokeButton;
        public DXControl PurchasePanel, SellOutPanel, GoldPanel, BusinessPanel, OrderPanel;
        public DXLabel GoldLabel, GameGoldLabel, GoldBox, GameGoldBox;
        public DXDecimalNumberBox PurchaseUnitPrice, PurchaseTotalPrice, SellOutUnitPrice, SellOutTotalPrice;
        public DXNumberBox PurchaseGoldCount, SellOutGoldCount;
        public DXVScrollBar OrderScrollBar;

        public BusinessRow[] BusineSellRows;
        public BusinessRow[] BusineBuyRows;
        public OrderRow[] OrderRow;

        List<ClientGoldMarketMyOrderInfo> MyOrders;
        /// <summary>
        /// 选定行
        /// </summary>
        public OrderRow SelectedRow
        {
            get => _SelectedRow;
            set
            {
                if (_SelectedRow == value) return;

                OrderRow oldValue = _SelectedRow;
                _SelectedRow = value;

                OnSelectedRowChanged(oldValue, value);
            }
        }
        private OrderRow _SelectedRow;
        public event EventHandler<EventArgs> SelectedRowChanged;
        public void OnSelectedRowChanged(OrderRow oValue, OrderRow nValue)
        {
            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;

            SelectedRowChanged?.Invoke(this, EventArgs.Empty);
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;  //自定义大小
        public override bool AutomaticVisibility => false;  //自动可见性
        #endregion

        /// <summary>
        /// 金币交易行
        /// </summary>
        public GoldTradingBusinessDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;

            Size = new Size(495, 415);
            Location = ClientArea.Location;

            GoldTradingBusinessBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 141,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                Visible = true,
            };

            OrderBackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 140,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                Visible = false,
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(454, 370),
                Hint = "关闭",
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            DXLabel dXLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(210, 15),
                Text = "金币交易行".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
            };

            #region 底部按钮
            Transaction = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 145,
                Label = { Text = "金币交易", Location = new Point(0, 2), },
                Location = new Point(20, 365),
            };
            Transaction.MouseClick += (o, e) =>
            {
                GoldTradingBusinessBackGround.Visible = true;
                OrderBackGround.Visible = false;
                CEnvir.Enqueue(new C.GoldMarketFlash() { });
            };

            Order = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 145,
                Label = { Text = "我的订单", Location = new Point(0, 2), },
                Location = new Point(110, 365),
            };
            Order.MouseClick += (o, e) =>
            {
                GoldTradingBusinessBackGround.Visible = false;
                OrderBackGround.Visible = true;
                CEnvir.Enqueue(new C.GoldMarketGetOwnOrder() { });
            };
            #endregion

            TransactionRefresh = new DXButton               //交易刷新
            {
                Parent = GoldTradingBusinessBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 145,
                Label = { Text = "刷新", Location = new Point(0, 2), },
                Location = new Point(350, 365),
            };
            TransactionRefresh.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GoldMarketFlash() { });
            };


            OrderRefresh = new DXButton                    //订单刷新
            {
                Parent = OrderBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 145,
                Label = { Text = "刷新", Location = new Point(0, 2), },
                Location = new Point(350, 365),
            };
            OrderRefresh.MouseClick += (o, e) => { CEnvir.Enqueue(new C.GoldMarketGetOwnOrder() { }); };

            Purchase = new DXButton
            {
                Parent = GoldTradingBusinessBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 148,
                Label = { Text = "买入", Location = new Point(0, 2), ForeColour = Color.FromArgb(198, 166, 99), },
                Location = new Point(28, 47),
            };
            Purchase.MouseClick += (o, e) =>
            {
                Purchase.Label.ForeColour = Color.FromArgb(198, 166, 99);
                SellOut.Label.ForeColour = Color.FromArgb(123, 123, 131);
                PurchasePanel.Visible = true;
                SellOutPanel.Visible = false;
            };

            SellOut = new DXButton
            {
                Parent = GoldTradingBusinessBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 148,
                Label = { Text = "卖出", Location = new Point(0, 2), ForeColour = Color.FromArgb(123, 123, 131), },
                Location = new Point(97, 47),
            };
            SellOut.MouseClick += (o, e) =>
            {
                Purchase.Label.ForeColour = Color.FromArgb(123, 123, 131);
                SellOut.Label.ForeColour = Color.FromArgb(198, 166, 99);
                PurchasePanel.Visible = false;
                SellOutPanel.Visible = true;
            };

            #region 买入
            PurchasePanel = new DXControl
            {
                Location = new Point(21, 83),
                Parent = GoldTradingBusinessBackGround,
                Size = new Size(142, 147),
                Border = true,
                BorderColour = Color.FromArgb(94, 66, 44),
                Visible = true,
            };

            DXLabel dXLabel1 = new DXLabel
            {
                Parent = PurchasePanel,
                Text = "单价".Lang(),
                Location = new Point(1, 10),
            };

            PurchaseUnitPrice = new DXDecimalNumberBox
            {
                Parent = PurchasePanel,
                MinValue = 1 / 100,
                MaxValue = 100000000,
                Location = new Point(29, 9),
                ValueTextBox = { ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
            };
            PurchaseUnitPrice.ValueTextBox.ValueChanged += PurchaseUnitPrice_ValueChanged;

            DXImageControl dXImage = new DXImageControl
            {
                Parent = PurchasePanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(119, 9),
                Hint = "赞助币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = PurchasePanel,
                Text = "数量".Lang(),
                Location = new Point(1, 40),
            };

            PurchaseGoldCount = new DXNumberBox
            {
                Parent = PurchasePanel,
                MinValue = 0,
                MaxValue = 10000,
                Location = new Point(16, 39),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(65, 18), Editable = true, ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
            };

            PurchaseGoldCount.ValueTextBox.ValueChanged += PurchaseUnitPrice_ValueChanged;


            dXLabel1 = new DXLabel
            {
                Parent = PurchasePanel,
                Text = "万".Lang(),
                Location = new Point(102, 40),
            };

            dXImage = new DXImageControl
            {
                Parent = PurchasePanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 150,
                Location = new Point(119, 39),
                Hint = "金币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = PurchasePanel,
                Text = "总价".Lang(),
                Location = new Point(1, 70),
            };

            PurchaseTotalPrice = new DXDecimalNumberBox
            {
                Parent = PurchasePanel,
                MinValue = 0 / 100,
                MaxValue = 10000,
                Location = new Point(16, 69),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(80, 18), ReadOnly = true, Editable = false, ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
            };

            dXImage = new DXImageControl
            {
                Parent = PurchasePanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(119, 69),
                Hint = "赞助币".Lang(),
            };

            OrderToBuyButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter1,
                Index = 146,
                Location = new Point(20, 100),
                Parent = PurchasePanel,
                Label = { Text = "下单买入".Lang(), Location = new Point(0, 2), },
            };
            OrderToBuyButton.MouseClick += (o, e) =>
            {
                //todo 交易条件是否满足

                CEnvir.Enqueue(new C.GoldMarketTrade() { GameGold = Convert.ToInt64(PurchaseUnitPrice.Value * 100), Gold = PurchaseGoldCount.Value, TradeType = TradeType.Buy });
            };

            #endregion

            #region 卖出
            SellOutPanel = new DXControl
            {
                Location = new Point(21, 83),
                Parent = GoldTradingBusinessBackGround,
                Size = new Size(142, 147),
                Border = true,
                BorderColour = Color.FromArgb(94, 66, 44),
                Visible = false,
            };

            dXLabel1 = new DXLabel
            {
                Parent = SellOutPanel,
                Text = "单价".Lang(),
                Location = new Point(1, 10),
            };

            SellOutUnitPrice = new DXDecimalNumberBox
            {
                Parent = SellOutPanel,
                MinValue = 1 / 100,
                MaxValue = 100000000,
                Location = new Point(29, 9),
                ValueTextBox = { ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
            };
            SellOutUnitPrice.ValueTextBox.ValueChanged += SellOutUnitPrice_ValueChanged;

            dXImage = new DXImageControl
            {
                Parent = SellOutPanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(119, 9),
                Hint = "赞助币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = SellOutPanel,
                Text = "数量".Lang(),
                Location = new Point(1, 40),
            };

            SellOutGoldCount = new DXNumberBox
            {
                Parent = SellOutPanel,
                MinValue = 0,
                MaxValue = 10000,
                Location = new Point(16, 39),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(65, 18), Editable = true, ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
            };
            SellOutGoldCount.ValueTextBox.ValueChanged += SellOutUnitPrice_ValueChanged;

            dXLabel1 = new DXLabel
            {
                Parent = SellOutPanel,
                Text = "万".Lang(),
                Location = new Point(102, 40),
            };

            dXImage = new DXImageControl
            {
                Parent = SellOutPanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 150,
                Location = new Point(119, 39),
                Hint = "金币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = SellOutPanel,
                Text = "总价".Lang(),
                Location = new Point(1, 70),
            };

            SellOutTotalPrice = new DXDecimalNumberBox
            {
                Parent = SellOutPanel,
                MinValue = 0 / 100,
                MaxValue = 10000,
                Location = new Point(16, 69),
                Size = new Size(125, 20),
                ValueTextBox = { Size = new Size(80, 18), ReadOnly = true, Editable = false, ForeColour = Color.White, BorderColour = Color.FromArgb(94, 66, 44), },
                UpButton = { Visible = false, },
                DownButton = { Visible = false, },
            };

            dXImage = new DXImageControl
            {
                Parent = SellOutPanel,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(119, 69),
                Hint = "赞助币".Lang(),
            };

            OrderAndSellButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter1,
                Index = 146,
                Location = new Point(20, 100),
                Parent = SellOutPanel,
                Label = { Text = "下单卖出".Lang(), Location = new Point(0, 2), },
            };
            OrderAndSellButton.MouseClick += (o, e) =>
            {
                //todo 交易条件是否满足

                CEnvir.Enqueue(new C.GoldMarketTrade() { GameGold = Convert.ToInt64(SellOutUnitPrice.Value * 100), Gold = SellOutGoldCount.Value, TradeType = TradeType.Sell });
            };
            #endregion

            #region 充值面板
            GoldPanel = new DXControl
            {
                Location = new Point(21, 231),
                Parent = GoldTradingBusinessBackGround,
                Size = new Size(142, 117),
                Border = true,
                BorderColour = Color.FromArgb(94, 66, 44),
                Visible = true,
            };

            GoldLabel = new DXLabel
            {
                Parent = GoldPanel,
                Text = "金币".Lang(),
                Location = new Point(6, 15),
            };

            GoldBox = new DXLabel
            {
                Parent = GoldPanel,
                Location = new Point(43, 13),
                AutoSize = false,
                Size = new Size(90, 20),
                Border = true,
                BorderColour = Color.FromArgb(94, 66, 44),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
            };

            GameGoldLabel = new DXLabel
            {
                Parent = GoldPanel,
                Text = "赞助币".Lang(),
                Location = new Point(0, 45),
            };

            GameGoldBox = new DXLabel
            {
                Parent = GoldPanel,
                Location = new Point(43, 43),
                AutoSize = false,
                Size = new Size(90, 20),
                Border = true,
                BorderColour = Color.FromArgb(94, 66, 44),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
            };

            RechargeButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter1,
                Index = 146,
                Location = new Point(20, 73),
                Parent = GoldPanel,
                Label = { Text = "赞助币充值".Lang(), Location = new Point(0, 2), },
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
                        System.Diagnostics.Process.Start(CEnvir.BuyAddress);
                        return;
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(CEnvir.BuyAddress + MapObject.User.Name);
                        return;
                    }
                });
            };
            #endregion

            #region 买卖内容面板
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "单价（".Lang(),
                Location = new Point(220, 52),
            };

            dXImage = new DXImageControl
            {
                Parent = GoldTradingBusinessBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(260, 51),
                Hint = "赞助币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "/万".Lang(),
                Location = new Point(280, 52),
            };

            dXImage = new DXImageControl
            {
                Parent = GoldTradingBusinessBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 150,
                Location = new Point(300, 51),
                Hint = "金币".Lang(),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "）".Lang(),
                Location = new Point(317, 52),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "数量".Lang(),
                Location = new Point(375, 52),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "卖出五".Lang(),
                Location = new Point(173, 82),
                ForeColour = Color.FromArgb(171, 83, 70),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "卖出四".Lang(),
                Location = new Point(173, 101),
                ForeColour = Color.FromArgb(171, 83, 70),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "卖出三".Lang(),
                Location = new Point(173, 120),
                ForeColour = Color.FromArgb(171, 83, 70),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "卖出二".Lang(),
                Location = new Point(173, 139),
                ForeColour = Color.FromArgb(171, 83, 70),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "卖出一".Lang(),
                Location = new Point(173, 158),
                ForeColour = Color.FromArgb(171, 83, 70),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "买入一".Lang(),
                Location = new Point(173, 177),
                ForeColour = Color.FromArgb(116, 154, 105),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "买入二".Lang(),
                Location = new Point(173, 196),
                ForeColour = Color.FromArgb(116, 154, 105),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "买入三".Lang(),
                Location = new Point(173, 215),
                ForeColour = Color.FromArgb(116, 154, 105),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "买入四".Lang(),
                Location = new Point(173, 234),
                ForeColour = Color.FromArgb(116, 154, 105),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "买入五".Lang(),
                Location = new Point(173, 253),
                ForeColour = Color.FromArgb(116, 154, 105),
            };

            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "一、显示买卖金额各5组，价格匹配自动成交".Lang(),
                Location = new Point(173, 290),
                ForeColour = Color.FromArgb(146, 146, 146),
            };
            dXLabel1 = new DXLabel
            {
                Parent = GoldTradingBusinessBackGround,
                Text = "二、48小时后未成交，自动取消。".Lang(),
                Location = new Point(173, 315),
                ForeColour = Color.FromArgb(146, 146, 146),
            };
            //dXLabel1 = new DXLabel
            //{
            //    Parent = GoldTradingBusinessBackGround,
            //    Text = "三、成交后收卖方5%交易税（每笔最低扣1赞助币）".Lang(),
            //    Location = new Point(173, 324),
            //    ForeColour = Color.FromArgb(146, 146, 146),
            //};

            BusinessPanel = new DXControl
            {
                Location = new Point(216, 80),
                Parent = GoldTradingBusinessBackGround,
                Size = new Size(260, 190),
                Border = false,
                BorderColour = Color.Red,
            };

            BusineSellRows = new BusinessRow[5];

            for (int i = 0; i < BusineSellRows.Length; i++)
            {
                BusineSellRows[i] = new BusinessRow
                {
                    Parent = BusinessPanel,
                    Location = new Point(0, 77 - i * 19),
                };
            }

            BusineBuyRows = new BusinessRow[5];

            for (int i = 0; i < BusineBuyRows.Length; i++)
            {
                BusineBuyRows[i] = new BusinessRow
                {
                    Parent = BusinessPanel,
                    Location = new Point(0, 97 + i * 19),
                };
            }

            #endregion

            #region 订单面板
            dXLabel1 = new DXLabel
            {
                Parent = OrderBackGround,
                Text = "日期     买/卖       单价         数量         状态      剩余时间".Lang(),
                Location = new Point(35, 50),
            };

            OrderPanel = new DXControl
            {
                Location = new Point(18, 70),
                Parent = OrderBackGround,
                Size = new Size(433, 280),
                Border = false,
                BorderColour = Color.Red,
            };

            RevokeButton = new DXButton
            {
                Parent = OrderBackGround,
                LibraryFile = LibraryFile.GameInter1,
                Index = 145,
                Label = { Text = "取消订单", Location = new Point(0, 2), },
                Location = new Point(275, 365),
            };
            RevokeButton.MouseClick += (o, e) =>
            {
                if (SelectedRow != null && SelectedRow.MyOrder != null && SelectedRow.MyOrder.TradeState == StockOrderType.Normal)
                    CEnvir.Enqueue(new C.GoldMarketCannel() { Index = SelectedRow.MyOrder.Index });

            };

            OrderRow = new OrderRow[14];
            OrderScrollBar = new DXVScrollBar   //查询栏滚动条
            {
                Parent = OrderBackGround,
                Location = new Point(460, 45),
                Size = new Size(20, 295),
                VisibleSize = OrderRow.Length,
                Change = 3,
            };
            OrderScrollBar.ValueChanged += (o, e) => ScrollBar_ValueChanged(); //滚动值变化
            OrderScrollBar.MouseWheel += OrderScrollBar.DoMouseWheel;

            //为滚动条自定义皮肤 - 1为不设置
            OrderScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);

            for (int i = 0; i < OrderRow.Length; i++)
            {
                int index = i;
                OrderRow[index] = new OrderRow
                {
                    Parent = OrderPanel,
                    Location = new Point(0, 0 + i * 20),
                };
                OrderRow[index].MouseWheel += OrderScrollBar.DoMouseWheel;
                OrderRow[index].MouseClick += (o, e) =>
                {
                    SelectedRow = OrderRow[index];
                };
            }

            #endregion
        }

        /// <summary>
        /// 买入金额改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PurchaseUnitPrice_ValueChanged(object sender, EventArgs e)
        {
            if (PurchaseUnitPrice.ValueTextBox.Value * 100 * PurchaseGoldCount.ValueTextBox.Value > GameScene.Game.User.GameGold)
            {
                PurchaseGoldCount.ValueTextBox.Value = Convert.ToInt64(Math.Truncate(Convert.ToDecimal(GameScene.Game.User.GameGold) / (PurchaseUnitPrice.ValueTextBox.Value * 100)));
                PurchaseTotalPrice.ValueTextBox.Value = PurchaseUnitPrice.ValueTextBox.Value * PurchaseGoldCount.ValueTextBox.Value;
                return;
            }

            PurchaseTotalPrice.ValueTextBox.Value = PurchaseUnitPrice.ValueTextBox.Value * PurchaseGoldCount.ValueTextBox.Value;
        }

        /// <summary>
        /// 卖出金额改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SellOutUnitPrice_ValueChanged(object sender, EventArgs e)
        {
            if (SellOutTotalPrice.ValueTextBox.Value > GameScene.Game.User.Gold / 10000)
            {
                SellOutTotalPrice.ValueTextBox.Value = Convert.ToDecimal(GameScene.Game.User.Gold / 10000) * SellOutUnitPrice.ValueTextBox.Value;
                return;
            }

            SellOutTotalPrice.ValueTextBox.Value = SellOutUnitPrice.ValueTextBox.Value * SellOutGoldCount.ValueTextBox.Value;
        }

        private void ScrollBar_ValueChanged()
        {
            if (MyOrders == null) return;

            OrderScrollBar.MaxValue = MyOrders.Count;

            for (int i = 0; i < OrderRow.Length; i++)
            {
                OrderRow[i].MyOrder = i + OrderScrollBar.Value >= MyOrders.Count ? null : MyOrders[i + OrderScrollBar.Value];
            }
        }

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (nValue == true)
                CEnvir.Enqueue(new C.GoldMarketFlash() { });
        }
        public void TradeReFlash(S.GoldMarketList p)
        {
            for (int i = 0; i < BusineSellRows.Length; ++i)
            {
                if (i < p.SellList.Count)
                {
                    BusineSellRows[i].GameGoldLabel.Text = (Convert.ToDecimal(p.SellList[i].GoldPrice) / 100).ToString("###0.00");
                    BusineSellRows[i].GoldLabel.Text = p.SellList[i].Count.ToString();
                }
                else
                {
                    BusineSellRows[i].GameGoldLabel.Text = "";
                    BusineSellRows[i].GoldLabel.Text = "";
                }
            }
            for (int i = 0; i < BusineBuyRows.Length; ++i)
            {
                if (i < p.BuyList.Count)
                {
                    BusineBuyRows[i].GameGoldLabel.Text = (Convert.ToDecimal(p.BuyList[i].GoldPrice) / 100).ToString("###0.00");
                    BusineBuyRows[i].GoldLabel.Text = p.BuyList[i].Count.ToString();
                }
                else
                {
                    BusineBuyRows[i].GameGoldLabel.Text = "";
                    BusineBuyRows[i].GoldLabel.Text = "";
                }

            }
        }
        public void OrderFlash(S.GoldMarketMyOrderList p)
        {
            MyOrders = p.MyOrder;
            ScrollBar_ValueChanged();
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (GoldTradingBusinessBackGround != null)
                {
                    if (!GoldTradingBusinessBackGround.IsDisposed)
                        GoldTradingBusinessBackGround.Dispose();

                    GoldTradingBusinessBackGround = null;
                }

                if (OrderBackGround != null)
                {
                    if (!OrderBackGround.IsDisposed)
                        OrderBackGround.Dispose();

                    OrderBackGround = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }

                if (RevokeButton != null)
                {
                    if (!RevokeButton.IsDisposed)
                        RevokeButton.Dispose();

                    RevokeButton = null;
                }

                if (Transaction != null)
                {
                    if (!Transaction.IsDisposed)
                        Transaction.Dispose();

                    Transaction = null;
                }

                if (Order != null)
                {
                    if (!Order.IsDisposed)
                        Order.Dispose();

                    Order = null;
                }

                if (TransactionRefresh != null)
                {
                    if (!TransactionRefresh.IsDisposed)
                        TransactionRefresh.Dispose();

                    TransactionRefresh = null;
                }

                if (OrderRefresh != null)
                {
                    if (!OrderRefresh.IsDisposed)
                        OrderRefresh.Dispose();

                    OrderRefresh = null;
                }

                if (Purchase != null)
                {
                    if (!Purchase.IsDisposed)
                        Purchase.Dispose();

                    Purchase = null;
                }

                if (SellOut != null)
                {
                    if (!SellOut.IsDisposed)
                        SellOut.Dispose();

                    SellOut = null;
                }

                if (PurchasePanel != null)
                {
                    if (!PurchasePanel.IsDisposed)
                        PurchasePanel.Dispose();

                    PurchasePanel = null;
                }

                if (PurchaseUnitPrice != null)
                {
                    if (!PurchaseUnitPrice.IsDisposed)
                        PurchaseUnitPrice.Dispose();

                    PurchaseUnitPrice = null;
                }

                if (PurchaseGoldCount != null)
                {
                    if (!PurchaseGoldCount.IsDisposed)
                        PurchaseGoldCount.Dispose();

                    PurchaseGoldCount = null;
                }

                if (PurchaseTotalPrice != null)
                {
                    if (!PurchaseTotalPrice.IsDisposed)
                        PurchaseTotalPrice.Dispose();

                    PurchaseTotalPrice = null;
                }

                if (SellOutPanel != null)
                {
                    if (!SellOutPanel.IsDisposed)
                        SellOutPanel.Dispose();

                    SellOutPanel = null;
                }

                if (SellOutUnitPrice != null)
                {
                    if (!SellOutUnitPrice.IsDisposed)
                        SellOutUnitPrice.Dispose();

                    SellOutUnitPrice = null;
                }

                if (SellOutGoldCount != null)
                {
                    if (!SellOutGoldCount.IsDisposed)
                        SellOutGoldCount.Dispose();

                    SellOutGoldCount = null;
                }

                if (SellOutTotalPrice != null)
                {
                    if (!SellOutTotalPrice.IsDisposed)
                        SellOutTotalPrice.Dispose();

                    SellOutTotalPrice = null;
                }

                if (OrderAndSellButton != null)
                {
                    if (!OrderAndSellButton.IsDisposed)
                        OrderAndSellButton.Dispose();

                    OrderAndSellButton = null;
                }

                if (BusinessPanel != null)
                {
                    if (!BusinessPanel.IsDisposed)
                        BusinessPanel.Dispose();

                    BusinessPanel = null;
                }

                if (OrderPanel != null)
                {
                    if (!OrderPanel.IsDisposed)
                        OrderPanel.Dispose();

                    OrderPanel = null;
                }

                if (OrderScrollBar != null)
                {
                    if (!OrderScrollBar.IsDisposed)
                        OrderScrollBar.Dispose();

                    OrderScrollBar = null;
                }

                if (GoldPanel != null)
                {
                    if (!GoldPanel.IsDisposed)
                        GoldPanel.Dispose();

                    GoldPanel = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (GoldBox != null)
                {
                    if (!GoldBox.IsDisposed)
                        GoldBox.Dispose();

                    GoldBox = null;
                }

                if (GameGoldLabel != null)
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (GameGoldBox != null)
                {
                    if (!GameGoldBox.IsDisposed)
                        GameGoldBox.Dispose();

                    GameGoldBox = null;
                }

                if (RechargeButton != null)
                {
                    if (!RechargeButton.IsDisposed)
                        RechargeButton.Dispose();

                    RechargeButton = null;
                }
            }
        }
        #endregion
    }

    #region 买卖内容条
    /// <summary>
    /// 买卖内容条
    /// </summary>
    public sealed class BusinessRow : DXControl
    {
        #region Properties
        public DXLabel GameGoldLabel, GoldLabel, Company;
        public DXImageControl GameGoldImage, GoldImage;

        #endregion

        public BusinessRow()
        {
            Size = new Size(255, 19);

            GameGoldLabel = new DXLabel
            {
                Parent = this,
                Text = "0",
                Location = new Point(5, 2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(70, 16),
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            GameGoldImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(80, 1),
                Hint = "赞助币".Lang(),
            };

            GoldLabel = new DXLabel
            {
                Parent = this,
                Text = "0",
                Location = new Point(115, 2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(70, 16),
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            Company = new DXLabel
            {
                Parent = this,
                Text = "万".Lang(),
                Location = new Point(187, 2),
                ForeColour = Color.White,
            };

            GoldImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 150,
                Location = new Point(203, 1),
                Hint = "金币".Lang(),
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (GameGoldLabel != null)
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (GameGoldImage != null)
                {
                    if (!GameGoldImage.IsDisposed)
                        GameGoldImage.Dispose();

                    GameGoldImage = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (Company != null)
                {
                    if (!Company.IsDisposed)
                        Company.Dispose();

                    Company = null;
                }

                if (GoldImage != null)
                {
                    if (!GoldImage.IsDisposed)
                        GoldImage.Dispose();

                    GoldImage = null;
                }
            }
        }
        #endregion
    }
    #endregion

    #region 订单内容条
    /// <summary>
    /// 订单内容条
    /// </summary>
    public sealed class OrderRow : DXControl
    {
        #region Properties
        public DXLabel DateLabel, TradingStatus, GameGoldLabel, GoldLabel, Company, TransactionStatus, RemainingTime;
        public DXImageControl GameGoldImage, GoldImage;
        private ClientGoldMarketMyOrderInfo _myOrder;

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

        public ClientGoldMarketMyOrderInfo MyOrder
        {
            get => _myOrder;
            set
            {
                if (value == _myOrder) return;
                _myOrder = value;
                //TODO 修改UI对应的显示
                if (value != null)
                {
                    Visible = true;
                    DateLabel.Text = value.Date.ToShortDateString();
                    TradingStatus.Text = value.TradeType == TradeType.Sell ? "卖出" : "买入";
                    GameGoldLabel.Text = (Convert.ToDecimal(value.GoldPrice) / 100).ToString("####0.00");
                    GoldLabel.Text = value.Count.ToString();
                    if (value.TradeState == StockOrderType.Completed)
                        TransactionStatus.Text = "成交";
                    else if (value.TradeState == StockOrderType.Cannel)
                        TransactionStatus.Text = "主动撤单";
                    else if (value.CompletedCount != 0)
                        TransactionStatus.Text = "部分成交";
                    else
                        TransactionStatus.Text = "未成交";
                    RemainingTime.Text = "";
                    Hint = $"订单号:{value.Index}\n挂单时间:{value.Date.ToShortDateString()}\n" +
                        $"挂单总量:{value.Count.ToString()}\n成交量:{value.CompletedCount.ToString()}";
                }
                else
                {
                    DateLabel.Text = "";
                    TradingStatus.Text = "";
                    GameGoldLabel.Text = "";
                    GoldLabel.Text = "";
                    TransactionStatus.Text = "";
                    RemainingTime.Text = "";
                }
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

        #endregion

        public OrderRow()
        {
            Size = new Size(430, 20);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.Empty;

            Visible = false;

            DateLabel = new DXLabel
            {
                Parent = this,
                Text = "1900/0/0",
                Location = new Point(5, 2),
                ForeColour = Color.White,
                IsControl = false,
            };

            TradingStatus = new DXLabel
            {
                Parent = this,
                Text = "买卖",
                Location = new Point(75, 2),
                IsControl = false,
            };

            GameGoldLabel = new DXLabel
            {
                Parent = this,
                Text = "0",
                Location = new Point(120, 2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            GameGoldImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 151,
                Location = new Point(175, 1),
                IsControl = false,
            };

            GoldLabel = new DXLabel
            {
                Parent = this,
                Text = "0",
                Location = new Point(205, 2),
                IsControl = false,
                AutoSize = false,
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                ForeColour = Color.White,
            };

            Company = new DXLabel
            {
                Parent = this,
                Text = "万".Lang(),
                Location = new Point(254, 2),
                ForeColour = Color.White,
                IsControl = false,
            };

            GoldImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter1,
                Index = 150,
                Location = new Point(268, 1),
                IsControl = false,
            };

            TransactionStatus = new DXLabel
            {
                Parent = this,
                Text = "成交".Lang(),
                Location = new Point(300, 2),
                ForeColour = Color.White,
                IsControl = false,
            };

            RemainingTime = new DXLabel
            {
                Parent = this,
                Text = "1900/0/0",
                Location = new Point(360, 2),
                ForeColour = Color.White,
                IsControl = false,
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (DateLabel != null)
                {
                    if (!DateLabel.IsDisposed)
                        DateLabel.Dispose();

                    DateLabel = null;
                }

                if (TradingStatus != null)
                {
                    if (!TradingStatus.IsDisposed)
                        TradingStatus.Dispose();

                    TradingStatus = null;
                }

                if (GameGoldLabel != null)
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (GameGoldImage != null)
                {
                    if (!GameGoldImage.IsDisposed)
                        GameGoldImage.Dispose();

                    GameGoldImage = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (Company != null)
                {
                    if (!Company.IsDisposed)
                        Company.Dispose();

                    Company = null;
                }

                if (GoldImage != null)
                {
                    if (!GoldImage.IsDisposed)
                        GoldImage.Dispose();

                    GoldImage = null;
                }

                if (TransactionStatus != null)
                {
                    if (!TransactionStatus.IsDisposed)
                        TransactionStatus.Dispose();

                    TransactionStatus = null;
                }

                if (RemainingTime != null)
                {
                    if (!RemainingTime.IsDisposed)
                        RemainingTime.Dispose();

                    RemainingTime = null;
                }
            }
        }
        #endregion
    }
    #endregion
}
