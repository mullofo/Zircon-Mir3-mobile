using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 奖金池动态槽
    /// </summary>
    public sealed class BonusPoolDialog : DXImageControl
    {
        /// <summary>
        /// 数值栏
        /// </summary>
        public DXImageControl BonusPoolValueBar;

        /// <summary>
        /// 数值标签 
        /// </summary>
        public DXLabel BonusPoolValueLabel;

        /// <summary>
        /// 从服务端收到的 当前池子信息
        /// </summary>
        public ClientRewardPoolInfo CurrentPoolInfo
        {
            get => _currentPoolInfo;
            set
            {
                _currentPoolInfo = value;
                // 更新UI
                BonusPoolValueBarOnAfterDraw(this, EventArgs.Empty);
            }
        }
        private ClientRewardPoolInfo _currentPoolInfo = null;

        /// <summary>
        /// 奖金池动态槽
        /// </summary>
        public BonusPoolDialog()
        {
            LibraryFile = LibraryFile.Interface;  //UI面板主图的索引位置
            Index = 231;  //UI主图序号
            PassThrough = true;  //穿透开启

            BonusPoolValueBar = new DXImageControl  //数值栏
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 231,
                Visible = true,
            };
            BonusPoolValueBar.Location = new Point(0, 0);
            BonusPoolValueBar.AfterDraw += BonusPoolValueBarOnAfterDraw;

            BonusPoolValueLabel = new DXLabel   //奖金池总值标签
            {
                Parent = BonusPoolValueBar,
                ForeColour = Color.White,
                Outline = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            //BonusPoolValueLabel.MouseClick += (o, e) => GameScene.Game.BonusPoolVersionBox.Visible = !GameScene.Game.BonusPoolVersionBox.Visible;
            //BonusPoolValueBar.MouseClick += (o, e) => GameScene.Game.BonusPoolVersionBox.Visible = !GameScene.Game.BonusPoolVersionBox.Visible;
        }

        /// <summary>
        /// 更新池子显示
        /// </summary>
        private void BonusPoolValueBarOnAfterDraw(object sender, EventArgs e)
        {
            if (BonusPoolValueBar.Library == null) return;

            if (CurrentPoolInfo == null) return;

            switch (CurrentPoolInfo.CurrentTier)
            {
                case 0:
                    //CEnvir.SaveError("奖金池出错: CurrentPoolInfo.CurrentTier=0");
                    return;
                case 1:
                    // 红
                    BonusPoolValueBar.Index = 232;
                    break;
                case 2:
                    // 黄
                    BonusPoolValueBar.Index = 235;
                    break;
                case 3:
                    // 绿
                    BonusPoolValueBar.Index = 236;
                    break;
                case 4:
                    // 蓝
                    BonusPoolValueBar.Index = 234;
                    break;
                case 5:
                    // 紫
                    BonusPoolValueBar.Index = 233;
                    break;
                default:
                    //CEnvir.SaveError("奖金池出错: CurrentPoolInfo.CurrentTier不在范围内");
                    return;
            }

            // 当前档位的百分比
            float drawPercent;
            // 5级满级了 铺满进度条
            if (CurrentPoolInfo.CurrentTier == 5 || CurrentPoolInfo.CurrentUpperLimit == CurrentPoolInfo.CurrentLowerLimit)
            {
                drawPercent = 1F;
            }
            else
            {
                decimal percent = (CurrentPoolInfo.Balance - CurrentPoolInfo.CurrentLowerLimit) /
                                  (CurrentPoolInfo.CurrentUpperLimit - CurrentPoolInfo.CurrentLowerLimit);
                drawPercent = (float)Math.Min(1, Math.Max(0, percent));
            }
            if (drawPercent == 0) return;

            // 更新文字显示
            BonusPoolValueLabel.Text = $"{CurrentPoolInfo.Balance:0.0000000000}";
            BonusPoolValueLabel.Location = new Point(DisplayArea.Width / 2 - BonusPoolValueLabel.DisplayArea.Width / 2,
                DisplayArea.Height / 2 - BonusPoolValueLabel.DisplayArea.Height / 2 - 1);

            BonusPoolValueLabel.Hint = $"距离下一档: {CurrentPoolInfo.CurrentUpperLimit - CurrentPoolInfo.Balance}";
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BonusPoolValueBar != null)
                {
                    if (!BonusPoolValueBar.IsDisposed)
                        BonusPoolValueBar.Dispose();

                    BonusPoolValueBar = null;
                }

                if (BonusPoolValueLabel != null)
                {
                    if (!BonusPoolValueLabel.IsDisposed)
                        BonusPoolValueLabel.Dispose();

                    BonusPoolValueLabel = null;
                }
            }
        }
        #endregion

    }

    /// <summary>
    /// 奖金池排行版界面
    /// </summary>
    public sealed class BonusPoolVersionDialog : DXImageControl
    {
        public DXButton CloseButton;
        public DXLabel BonusPoolLabel, RedPacketLabel, BonusPoolRankingLabel, RedPacketRankingLabel;

        public DXListBox RedPacketListBox;
        public List<DXListBoxItem> RedPacketListBoxItems = new List<DXListBoxItem>();

        public DXListBox ClaimHistoryListBox;
        public List<DXListBoxItem> ClaimHistoryListBoxItems = new List<DXListBoxItem>();

        public List<ClientRedPacketInfo> ClaimableRedPackets = new List<ClientRedPacketInfo>();
        public List<ClientRedPacketInfo> OtherPackets = new List<ClientRedPacketInfo>();

        public ClientRedPacketInfo SelectedRedpacketInfo => (ClientRedPacketInfo)(RedPacketListBox?.SelectedItem?.Tag);

        /// <summary>
        /// 排行榜姓名label
        /// 第一名 第二名 第三名 我自己
        /// </summary>
        public DXLabel FirstNameLabel, SecondNameLabel, ThirdNameLabel, MyselfNameLabel;
        /// <summary>
        /// 累计获取金额
        /// 第一名 第二名 第三名 我自己
        /// </summary>
        public DXLabel FirstTotalEarnedLabel, SecondTotalEarnedLabel, ThirdTotalEarnedLabel, MyselfTotalEarnedLabel;
        /// <summary>
        /// 累计提现金额排名
        /// 第一名 第二名 第三名 我自己
        /// </summary>
        public DXLabel FirstTotalCashedOutLabel, SecondTotalCashedOutLabel, ThirdTotalCashedOutLabel, MyselfTotalCashedOutLabel;
        public DXLabel MyRank;

        public DXButton ClaimRedpacket;
        public RedpacketFace RedpacketFaceUI;

        public ClientRewardPoolRanks First { get; set; }
        public ClientRewardPoolRanks Second { get; set; }
        public ClientRewardPoolRanks Third { get; set; }
        public ClientRewardPoolRanks Myself { get; set; }

        /// <summary>
        /// 奖金池排行版
        /// </summary>
        public BonusPoolVersionDialog()
        {
            LibraryFile = LibraryFile.Interface;  //UI面板主图的索引位置
            Index = 240;  //奖金池排行版的图片

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            CloseButton.Location = new Point(Size.Width - CloseButton.Size.Width - 13, 2);
            CloseButton.MouseClick += (o, e) => Visible = false;

            BonusPoolLabel = new DXLabel
            {
                Text = "忠诚度",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(185, 14),
            };

            RedPacketLabel = new DXLabel
            {
                Text = "红包",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(485, 14),
            };

            BonusPoolRankingLabel = new DXLabel
            {
                Text = "忠诚度排行版",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(125, 125, 125),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(30, 55),
            };

            #region 排行榜

            FirstNameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(110, 100),
                Text = "第一名"
            };
            FirstTotalEarnedLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 100),
                Text = "累计获取"
            };

            FirstTotalCashedOutLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 125),
                Text = "累计收益"
            };

            SecondNameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(110, 170),
                Text = "第二名"
            };
            SecondTotalEarnedLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 170),
                Text = "累计获取"
            };

            SecondTotalCashedOutLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 190),
                Text = "累计收益"
            };

            ThirdNameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(110, 230),
                Text = "第三名"
            };
            ThirdTotalEarnedLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 230),
                Text = "累计获取"
            };

            ThirdTotalCashedOutLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 250),
                Text = "累计收益"
            };

            MyRank = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.White,
                Size = new Size(50, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Outline = false,
                IsControl = false,
                Location = new Point(55, 305),
                Text = "我自己的排名"
            };

            MyselfNameLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(110, 290),
                Text = "我自己"
            };
            MyselfTotalEarnedLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 290),
                Text = "累计获取"
            };

            MyselfTotalCashedOutLabel = new DXLabel
            {
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                ForeColour = Color.White,
                Outline = false,
                IsControl = false,
                Location = new Point(175, 310),
                Text = "累计收益"
            };

            #endregion

            RedPacketRankingLabel = new DXLabel
            {
                Text = "红包列表",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(125, 125, 125),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(387, 54),
            };

            RedPacketListBox = new DXListBox
            {
                Parent = this,
                Location = new Point(360, 80),
                Size = new Size(110, 260),
                Border = false,
                Opacity = 0f,
            };
            RedPacketListBox.ScrollBar.Visible = true;
            RedPacketListBox.selectedItemChanged += (sender, args) =>
            {
                UpdateRedpacketHistory();

                if (RedpacketFaceUI != null)
                    RedpacketFaceUI.RedpacketInfo = SelectedRedpacketInfo;
            };

            ClaimHistoryListBox = new DXListBox
            {
                Parent = this,
                Location = new Point(490, 260),
                Size = new Size(170, 80),
                Border = false,
                Opacity = 0f,
            };
            ClaimHistoryListBox.ScrollBar.Visible = true;

            RedpacketFaceUI = new RedpacketFace
            {
                Parent = this,
                Location = new Point(550, 128),
                Visible = true,
                //Size = new Size(170, 80),
            };

            GameScene.Game.BonusPoolBox.Parent = this;
            GameScene.Game.BonusPoolBox.Location = new Point(60, 38);
        }

        public void RefreshRanks()
        {
            if (First != null)
            {
                FirstNameLabel.Text = First.Name;
                FirstTotalEarnedLabel.Text = $"累计获得: {First.TotalEarned}";
                FirstTotalCashedOutLabel.Text = $"累计收益: {First.TotalCashedOut}";
            }
            if (Second != null)
            {
                SecondNameLabel.Text = Second.Name;
                SecondTotalEarnedLabel.Text = $"累计获得: {Second.TotalEarned}";
                SecondTotalCashedOutLabel.Text = $"累计收益: {Second.TotalCashedOut}";
            }
            if (Third != null)
            {
                ThirdNameLabel.Text = Third.Name;
                ThirdTotalEarnedLabel.Text = $"累计获得: {Third.TotalEarned}";
                ThirdTotalCashedOutLabel.Text = $"累计收益: {Third.TotalCashedOut}";
            }
            if (Myself != null)
            {
                MyselfNameLabel.Text = Myself.Name;
                MyselfTotalEarnedLabel.Text = $"累计获得: {Myself.TotalEarned}";
                MyselfTotalCashedOutLabel.Text = $"累计收益: {Myself.TotalCashedOut}";
                MyRank.Text = $"{Myself.TotalEarnedRank}";
            }
            // TODO remove this
            RefreshAll();
        }

        public void UpdateRedpacketsList(List<ClientRedPacketInfo> rps)
        {
            if (rps == null) return;
            ClaimableRedPackets.Clear();
            OtherPackets.Clear();

            foreach (var info in rps)
            {
                if (!info.HasExpired && info.RemainingCount > 0)
                {
                    ClaimableRedPackets.Add(info);
                }
                else
                {
                    OtherPackets.Add(info);
                }
            }
            ClaimableRedPackets.Sort((x, y) => y.SendTime.CompareTo(x.SendTime));
            OtherPackets.Sort((x, y) => y.SendTime.CompareTo(x.SendTime));

            RefreshAll();
            UpdateRedpacketHistory();
        }

        public void RefreshAll()
        {
            RedPacketListBox.SelectedItem = null;

            foreach (DXListBoxItem item in RedPacketListBoxItems)
                item.Dispose();

            RedPacketListBoxItems.Clear();

            int counter = 0;
            foreach (var info in ClaimableRedPackets)
            {
                RedPacketListBoxItems.Add(new DXListBoxItem
                {
                    Parent = RedPacketListBox,
                    Label =
                    {
                        Text = $"{info.SenderName} {info.RemainingCount}/{info.TotalCount}"
                    },
                    Item = counter,
                    Tag = info
                });
                counter++;
            }

            foreach (var info in OtherPackets)
            {
                RedPacketListBoxItems.Add(new DXListBoxItem
                {
                    Parent = RedPacketListBox,
                    Label =
                    {
                        Text = $"{info.SenderName} {info.RemainingCount}/{info.TotalCount}"
                    },
                    Item = counter,
                    Tag = info
                });
                counter++;
            }

        }

        public void UpdateSingleRedPacket(ClientRedPacketInfo info)
        {
            if (info == null) return;

            foreach (var item in RedPacketListBoxItems)
            {
                var temp = (ClientRedPacketInfo)item.Tag;
                if (temp != null && temp.Index == info.Index)
                {
                    item.Tag = info;
                    item.Item = RedPacketListBoxItems.Count;
                    item.Label.Text = $"{info.SenderName} {info.RemainingCount}/{info.TotalCount}";
                    UpdateRedpacketHistory();
                    return;
                }
            }

            ClaimableRedPackets.Add(info);
            UpdateRedpacketsList(ClaimableRedPackets.Concat(OtherPackets).ToList());
        }

        public void UpdateRedpacketHistory()
        {
            if (SelectedRedpacketInfo != null)
            {
                // history
                foreach (DXListBoxItem item in ClaimHistoryListBoxItems)
                    item.Dispose();

                ClaimHistoryListBoxItems.Clear();
                int counter = 0;

                foreach (var info in SelectedRedpacketInfo.ClaimInfoList)
                {
                    ClaimHistoryListBoxItems.Add(new DXListBoxItem
                    {
                        Parent = ClaimHistoryListBox,
                        Label =
                        {
                            Text = $"{info.ClaimerName}: {info.Amount}{info.Currency.Lang()}"
                        },
                        Hint = $"{info.ClaimerName} 于 {info.ClaimTime} 领取了 {info.Amount}{info.Currency.Lang()}",
                        Item = counter,
                        Tag = info
                    });
                    counter++;
                }
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

            }
        }
        #endregion
    }


    public sealed class RedpacketFace : DXControl
    {
        // todo 这些暂时没用到 美化的时候有用
        [AutoDispose]
        public DXLabel SenderNameLabel,
            CurrencyLabel,
            FaceValueLabel,
            RemainingValueLabel,
            CountLabel,
            TypeLabel,
            ScopeLabel,
            MessageLabel,
            SendTimeLabel,
            ExpireTimeLabel;

        [AutoDispose]
        public DXButton ClaimButton;

        private ClientRedPacketInfo _redpacketInfo;

        public ClientRedPacketInfo RedpacketInfo
        {
            get => _redpacketInfo;
            set
            {
                _redpacketInfo = value;
                Update();
            }
        }

        public RedpacketFace()
        {
            Size = new Size(60, 60);
            Border = true;

            ClaimButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 241,
                Location = new Point(0, 0),
            };
            ClaimButton.MouseClick += (sender, args) =>
            {
                // 2秒后可以再点
                GameScene.Game.User.NextClaimRedpacketTime = CEnvir.Now + TimeSpan.FromSeconds(2);
                ClaimButton.Enabled = false;
                if (GameScene.Game.BonusPoolVersionBox?.SelectedRedpacketInfo != null)
                {
                    CEnvir.Enqueue(new C.ClaimRedpacket
                    { RedpacketIndex = GameScene.Game.BonusPoolVersionBox.SelectedRedpacketInfo.Index });
                }
            };
        }

        public void Update()
        {
            if (RedpacketInfo == null) return;

            this.Hint = $"发送者: {RedpacketInfo.SenderName}\n" +
                               $"货币类型: {RedpacketInfo.Currency.Lang()}" +
                               $"总额: {RedpacketInfo.FaceValue}\n" +
                               $"剩余金额: {RedpacketInfo.RemainingValue}\n" +
                               $"总个数: {RedpacketInfo.TotalCount}\n" +
                               $"剩余个数: {RedpacketInfo.RemainingCount}\n" +
                               $"红包类型: {Functions.GetEnumDescription(RedpacketInfo.Type)}\n" +
                               $"谁可以领: {Functions.GetEnumDescription(RedpacketInfo.Scope)}\n" +
                               $"信息: {RedpacketInfo.Message}\n" +
                               $"发送时间: {RedpacketInfo.SendTime}\n" +
                               $"过期时间: {RedpacketInfo.ExpireTime}\n";
        }
    }
}
