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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 行会功能
    /// </summary>
    public sealed class GuildDialog : DXWindow
    {
        #region Properties

        public DXImageControl GuildGround;  //行会底图
        private DXTabControl GuildTabs;  //行会分页

        #region CreateTab
        //创建选项卡
        private DXTab CreateTab;

        public DXTextBox GuildNameBox;
        public DXNumberTextBox MemberTextBox, StorageTextBox, TotalCostBox;

        public DXCheckBox GoldCheckBox, HornCheckBox;

        #region MemberLimit
        /// <summary>
        /// 行会成员限制
        /// </summary>
        public int MemberLimit
        {
            get => _MemberLimit;
            set
            {
                if (_MemberLimit == value) return;

                int oldValue = _MemberLimit;
                _MemberLimit = value;

                OnMemberLimitChanged(oldValue, value);
            }
        }
        private int _MemberLimit;
        public event EventHandler<EventArgs> MemberLimitChanged;
        public void OnMemberLimitChanged(int oValue, int nValue)
        {
            MemberLimitChanged?.Invoke(this, EventArgs.Empty);  //成员限制更改？调用（ 事件参数.空）

            TotalCostBox.Value = TotalCost;  //总成本框的价格=总成本

            CreateButton.Enabled = CanCreate; //创建按钮已启用 = 可以创建
        }

        #endregion

        #region StorageSize       
        /// <summary>
        /// 行会仓库容量
        /// </summary>
        public int StorageSize
        {
            get => _StorageSize;
            set
            {
                if (_StorageSize == value) return;

                int oldValue = _StorageSize;
                _StorageSize = value;

                OnStorageSizeChanged(oldValue, value);
            }
        }
        private int _StorageSize;
        public event EventHandler<EventArgs> StorageSizeChanged;
        public void OnStorageSizeChanged(int oValue, int nValue)
        {
            StorageSizeChanged?.Invoke(this, EventArgs.Empty);  //仓库大小更改？调用（ 事件参数.空）

            TotalCostBox.Value = TotalCost;

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        #region GuildNameValid
        /// <summary>
        /// 行会名字是否有效
        /// </summary>
        public bool GuildNameValid
        {
            get => _GuildNameValid;
            set
            {
                if (_GuildNameValid == value) return;

                bool oldValue = _GuildNameValid;
                _GuildNameValid = value;

                OnGuildNameValidChanged(oldValue, value);
            }
        }
        private bool _GuildNameValid;
        public event EventHandler<EventArgs> GuildNameValidChanged;
        public void OnGuildNameValidChanged(bool oValue, bool nValue)
        {
            GuildNameValidChanged?.Invoke(this, EventArgs.Empty);

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        #region CreateAttempted
        /// <summary>
        /// 尝试创建行会
        /// </summary>
        public bool CreateAttempted
        {
            get => _CreateAttempted;
            set
            {
                if (_CreateAttempted == value) return;

                bool oldValue = _CreateAttempted;
                _CreateAttempted = value;

                OnCreateAttemptedChanged(oldValue, value);
            }
        }
        private bool _CreateAttempted;
        public event EventHandler<EventArgs> CreateAttemptedChanged;
        public void OnCreateAttemptedChanged(bool oValue, bool nValue)
        {
            CreateAttemptedChanged?.Invoke(this, EventArgs.Empty);

            CreateButton.Enabled = CanCreate;
        }

        #endregion

        public bool CanCreate => !CreateAttempted && GuildNameValid && GameScene.Game != null && TotalCost <= GameScene.Game.User.Gold;
        public int TotalCost => (int)Math.Min(int.MaxValue, (GoldCheckBox.Checked ? CEnvir.ClientControl.GuildCreationCostEdit : 0) + (MemberLimit * CEnvir.ClientControl.GuildMemberCostEdit) + (StorageSize * CEnvir.ClientControl.GuildStorageCostEdit));

        public DXButton CreateButton, StarterGuildButton;

        #endregion

        #region HomeTab 主页

        private DXTab HomeTab;
        public DXLabel MemberLimitLabel, GuildFundLabel, DailyGrowthLabel, TotalContributionLabel, DailyContributionLabel, GuildLevel, GuildLevelExp;

        public DXVScrollBar NoticeScrollBar;
        public DXTextBox NoticeTextBox;
        public DXButton EditNoticeButton, SaveNoticeButton, CancelNoticeButton, GuildLevelButton;

        #endregion

        #region Member Tab 成员

        private DXTab MemberTab;
        public GuildMemberRow[] MemberRows;
        public DXVScrollBar MemberScrollBar;

        #endregion

        #region Storage Tab  行会仓库

        public DXTab StorageTab;               //行会仓库按钮
        public DXTextBox ItemNameTextBox;
        public DXComboBox ItemTypeComboBox;
        public DXItemGrid StorageGrid;          //行会仓库格子
        public DXButton ClearButton;
        public DXVScrollBar StorageScrollBar;   //行会仓库滚动条
        public ClientUserItem[] GuildStorage = new ClientUserItem[1000];   //行会仓库格子数 1000个格子

        #endregion

        #region Manage Tab  行会管理

        private DXTab ManageTab;
        public DXLabel GuildFundLabel1, MemberLimitLabel1, StorageSizeLabel;
        public DXTextBox AddMemberTextBox;
        public DXNumberTextBox MemberTaxBox;
        public DXButton AddMemberButton, EditDefaultMemberButton, SetTaxButton, IncreaseMemberButton, IncreaseStorageButton, StartWarButton, StartAllyButton, PaintFlag;
        public DXNumberTextBox GuildTaxBox;
        public DXControl AddMemberPanel, EditDefaultMemberPanel, TreasuryPanel, UpgradePanel, ApplicationPanel, GuildWarPanel, GuildAllyPanel, NormaWarPanel;
        public DXTextBox GuildWarTextBox, GuildAllyTextBox;
        public GuildFlagPanel FlagPanel;

        public GuildAllianceRow[] AllianceRows;
        public DXVScrollBar AllianceScrollBar;

        public Dictionary<CastleInfo, GuildCastlePanel> CastlePanels = new Dictionary<CastleInfo, GuildCastlePanel>();

        public bool CanUpdateFlag = true;
        public DXCheckBox AllowApplyCheckBox;
        public List<GuildApplicationLine> GuildApplications = new List<GuildApplicationLine>();
        public List<GuildGameGoldWithdrawalLine> GuildGameGoldWithdrawal = new List<GuildGameGoldWithdrawalLine>();
        private DXVScrollBar ScrollBar, GameGoldScrollBar;

        #region GuildWarNameValid
        /// <summary>
        /// 行会站名字是否有效
        /// </summary>
        public bool GuildWarNameValid
        {
            get { return _GuildWarNameValid; }
            set
            {
                if (_GuildWarNameValid == value) return;

                bool oldValue = _GuildWarNameValid;
                _GuildWarNameValid = value;

                OnGuildWarNameValidChanged(oldValue, value);
            }
        }
        private bool _GuildWarNameValid;
        public event EventHandler<EventArgs> GuildWarNameValidChanged;
        public void OnGuildWarNameValidChanged(bool oValue, bool nValue)
        {
            GuildWarNameValidChanged?.Invoke(this, EventArgs.Empty);

            StartWarButton.Enabled = CanWar;
        }

        #endregion

        #region WarTab  行会战争

        private DXTab WarTab;

        #endregion

        #region 行会资金

        private DXTab FundTab;
        public DXControl CurrentFundPanel, FundRankPanel, FundRecordPanel, ExtractPanel;

        public DXLabel FundLabel, FundAmountLabel, UpperLimitLabel, UpperLimitAmountLabel, FundNoticeLabel, FundNoticeContentLabel, FundRankNumLabel, FundRankCharNameLabel, FundRankAmountLabel, ExtractNumLabel;

        public DXLabel GameGoldLabel, GameGoldAmountLabel, FundRankGameGoldAmountLabel, FundRankAgreeLabel;

        public DXTextBox FundNoticeTextBox;
        //public DXVScrollBar FundNoticeScrollBar, FundRankScrollBar, FundChangeScrollBar;
        public DXButton EditFundNoticeButton, SaveFundNoticeButton, CancelFundNoticeButton, DepositButton, WithdrawButton, FundRankAgreeButton, DepositGameGoldButton, WithdrawGameGoldButton;
        public DXListBox FundRanksListBox, FundChangesListBox, ExtractChangesListBox;
        public List<DXListBoxItem> FundRanksBoxItems = new List<DXListBoxItem>();
        public List<DXListBoxItem> FundChangesBoxItems = new List<DXListBoxItem>();
        public List<DXListBoxItem> ExtractChangesBoxItems = new List<DXListBoxItem>();

        public long CurrentFund => GuildInfo.GuildFunds;
        public long MaxFund => GuildInfo.MaxFund;

        public List<ClientGuildFundRankInfo> TopRanks = new List<ClientGuildFundRankInfo>();
        public List<ClientGuildFundChangeInfo> RecentChanges = new List<ClientGuildFundChangeInfo>();

        #endregion

        #region WarAttempted
        /// <summary>
        /// 尝试开启战争
        /// </summary>
        public bool WarAttempted
        {
            get { return _WarAttempted; }
            set
            {
                if (_WarAttempted == value) return;

                bool oldValue = _WarAttempted;
                _WarAttempted = value;

                OnWarAttemptedChanged(oldValue, value);
            }
        }
        private bool _WarAttempted;
        public event EventHandler<EventArgs> WarAttemptedChanged;
        public void OnWarAttemptedChanged(bool oValue, bool nValue)
        {
            WarAttemptedChanged?.Invoke(this, EventArgs.Empty);
            StartWarButton.Enabled = CanWar;
        }

        #endregion

        public bool CanWar => !WarAttempted && GuildWarNameValid;

        #region GuildAllyNameValid
        /// <summary>
        /// 行会联盟名字是否有效
        /// </summary>
        public bool GuildAllyNameValid
        {
            get { return _GuildAllyNameValid; }
            set
            {
                if (_GuildAllyNameValid == value) return;

                bool oldValue = _GuildAllyNameValid;
                _GuildAllyNameValid = value;

                OnGuildAllyNameValidChanged(oldValue, value);
            }
        }
        private bool _GuildAllyNameValid;
        public event EventHandler<EventArgs> GuildAllyNameValidChanged;
        public void OnGuildAllyNameValidChanged(bool oValue, bool nValue)
        {
            GuildAllyNameValidChanged?.Invoke(this, EventArgs.Empty);

            StartAllyButton.Enabled = CanAlly;
        }

        #region AllyAttempted
        /// <summary>
        /// 尝试行会联盟
        /// </summary>
        public bool AllyAttempted
        {
            get { return _AllyAttempted; }
            set
            {
                if (_AllyAttempted == value) return;

                bool oldValue = _AllyAttempted;
                _AllyAttempted = value;

                OnAllyAttemptedChanged(oldValue, value);
            }
        }
        private bool _AllyAttempted;
        public event EventHandler<EventArgs> AllyAttemptedChanged;
        public void OnAllyAttemptedChanged(bool oValue, bool nValue)
        {
            AllyAttemptedChanged?.Invoke(this, EventArgs.Empty);
            StartAllyButton.Enabled = CanAlly;
        }

        #endregion

        public bool CanAlly => !AllyAttempted && GuildAllyNameValid;

        #endregion

        #endregion

        #region GuildInfo
        /// <summary>
        /// 行会信息
        /// </summary>
        public ClientGuildInfo GuildInfo
        {
            get => _GuildInfo;
            set
            {
                if (_GuildInfo == value) return;

                ClientGuildInfo oldValue = _GuildInfo;
                _GuildInfo = value;

                OnGuildInfoChanged(oldValue, value);
            }
        }
        private ClientGuildInfo _GuildInfo;
        public event EventHandler<EventArgs> GuildInfoChanged;
        public void OnGuildInfoChanged(ClientGuildInfo oValue, ClientGuildInfo nValue)
        {
            GuildInfoChanged?.Invoke(this, EventArgs.Empty);

            ClearGuild();

            if (GuildInfo == null)
            {
#if Mobile
#else
                GameScene.Game.UpdateGuildFlag(-1, Color.Empty);
#endif
                CreateTab.TabButton.InvokeMouseClick();
                return;
            }

            for (int i = 0; i < GuildStorage.Length; i++)
                GuildStorage[i] = null;

            HomeTab.TabButton.InvokeMouseClick();

            RefreshStorage();

            RefreshGuildDisplay();
            FlagPanel.UpdateView(GuildInfo);
#if Mobile
#else
            GameScene.Game.UpdateGuildFlag(GuildInfo.Flag, GuildInfo.FlagColor);
#endif

            AllowApplyCheckBox.Checked = GuildInfo.AllowApply;
            AllowApplyCheckBox.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

        }

        #endregion

        public DateTime SabukWarDate;

        public override WindowType Type => WindowType.GuildBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 行会界面
        /// </summary>
        public GuildDialog()
        {
            //TitleLabel.Text = "行会";
            HasTitle = false;  //字幕标题不显示
            HasFooter = false;  //不显示页脚
            HasTopBorder = false; //不显示上边框
            TitleLabel.Visible = false; //不显示标题
            CloseButton.Visible = false; //不显示关闭按钮            
            AllowResize = false; //不允许调整大小
            IgnoreMoveBounds = true;
            Opacity = 0F;

            SetClientSize(new Size(576, 420));

            GuildGround = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1300,
                Parent = this,
                IsControl = true,
                ImageOpacity = 0.85F,
                PassThrough = true,
                Location = new Point(0, 0)
            };

            GuildTabs = new DXTabControl
            {
                Parent = this,
                Size = new Size(568, 420),
                Location = new Point(14, 11),
            };

            CloseButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1221,
                Parent = this,
            };
            CloseButton.TouchUp += (o, e) => Visible = false;

            CreateCreateTab();          //新手页

            CreateHomeTab();            //创建主页

            CreateMemberTab();          //创建成员页

            CreateStorageTab();         //创建行会仓库页

            CreateManageTab();          //创建管理页

            CreateWarTab();             //创建战争页

            CreateFundTab();           //行会金库

            ClearGuild();               //清除行会信息
        }

        #region Methods

        /// <summary>
        /// 清除行会信息
        /// </summary>
        private void ClearGuild()
        {
            CreateTab.TabButton.Visible = GuildInfo == null;

            HomeTab.TabButton.Visible = GuildInfo != null;
            MemberTab.TabButton.Visible = GuildInfo != null;
            StorageTab.TabButton.Visible = GuildInfo != null;
            ManageTab.TabButton.Visible = GuildInfo != null;
            WarTab.TabButton.Visible = GuildInfo != null;
            FundTab.TabButton.Visible = GuildInfo != null;

            GuildTabs.TabsChanged();

            for (int i = 0; i < GuildStorage.Length; i++)
                GuildStorage[i] = null;

            TitleLabel.Text = "行会".Lang();

            NoticeTextBox.TextBox.Text = string.Empty;

            GuildLevel.Text = string.Empty;
            MemberLimitLabel.Text = string.Empty;
            GuildFundLabel.Text = string.Empty;
            DailyGrowthLabel.Text = string.Empty;
            TotalContributionLabel.Text = string.Empty;
            DailyContributionLabel.Text = string.Empty;
            GuildLevelExp.Text = string.Empty;

            GuildLevelButton.Visible = false;

            AddMemberTextBox.TextBox.Text = string.Empty;
            GuildTaxBox.TextBox.Text = string.Empty;
            GuildFundLabel1.Text = string.Empty;
            MemberLimitLabel1.Text = string.Empty;
            StorageSizeLabel.Text = string.Empty;

            MemberScrollBar.MaxValue = 0;

            foreach (GuildMemberRow row in MemberRows)
                row.MemberInfo = null;

            foreach (GuildAllianceRow row in AllianceRows)
                row.AllianceInfo = null;

            StorageGrid.GridSize = new Size(1, 1);

            StorageScrollBar.MaxValue = 0;

            EditNoticeButton.Visible = false;

            ItemNameTextBox.TextBox.Text = string.Empty;

            ItemTypeComboBox.ListBox.SelectItem(null);
        }
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (GuildInfo != null)
                GameScene.Game.GuildBox.RefreshGuildDisplay();
        }
        /// <summary>
        /// 刷新行会显示
        /// </summary>
        public void RefreshGuildDisplay()
        {
            TitleLabel.Text = GuildInfo.GuildName == Globals.StarterGuildName ? GuildInfo.GuildName.Lang() : GuildInfo.GuildName;

            if (!NoticeTextBox.Editable)
                NoticeTextBox.TextBox.Text = GuildInfo.Notice;

            if (!FundNoticeTextBox.Editable)
                FundNoticeTextBox.TextBox.Text = GuildInfo.VaultNotice;

            FundAmountLabel.Text = CurrentFund.ToString();
            UpperLimitAmountLabel.Text = MaxFund.ToString();
            var explist = Globals.GuildLevelExpList.Binding.FirstOrDefault(x => x.Level == GuildInfo.GuildLevel);
            GuildLevel.Text = GuildInfo.GuildLevel.ToString();
            MemberLimitLabel.Text = $"{GuildInfo.Members.Count} / {GuildInfo.MemberLimit}";
            GuildFundLabel.Text = GuildInfo.GuildFunds.ToString("#,##0");
            DailyGrowthLabel.Text = GuildInfo.DailyGrowth.ToString("#,##0");
            TotalContributionLabel.Text = GuildInfo.ActiveCount.ToString("#,##0");
            //DailyContributionLabel.Text = $"{GuildInfo.DailyContribution.ToString("#,##0")} / 10";
            if (explist != null)
                GuildLevelExp.Text = $"资金:{explist.GuildFunds} 活跃度:{explist.ActivCount}";
            else
                GuildLevelExp.Text = "MAX";
            GuildLevelButton.Visible = true;

            //更新
            UpdateFundRanks();
            UpdateFundChanges();

            UpdateMemberRows();
            UpdateAllianceRows();

            PermissionChanged();

            ApplyStorageFilter();

            GuildFundLabel1.Text = GuildInfo.GuildFunds.ToString("#,##0");
            MemberLimitLabel1.Text = GuildInfo.MemberLimit.ToString();
            StorageSizeLabel.Text = GuildInfo.StorageLimit.ToString(); ;

            GuildTaxBox.Value = GuildInfo.Tax;
            FlagPanel.UpdateView(GuildInfo);
        }
        /// <summary>
        /// 刷新行会仓库
        /// </summary>
        private void RefreshStorage()
        {
            StorageGrid.GridSize = new Size(13, Math.Max(9, (int)Math.Ceiling(GuildInfo.StorageLimit / (float)13)));

            StorageScrollBar.MaxValue = StorageGrid.GridSize.Height;

            foreach (DXItemCell cell in StorageGrid.Grid)
                cell.Item = null;

            foreach (ClientUserItem item in GuildInfo.Storage)
            {
                if (item.Slot < 0 || item.Slot >= StorageGrid.Grid.Length) continue;

                StorageGrid.Grid[item.Slot].Item = item;
            }
        }
        /// <summary>
        /// 权限更改时
        /// </summary>
        public void PermissionChanged()
        {
            if (GuildInfo == null)
            {
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled = false;
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Checked = false;

                GameScene.Game.NPCSpecialRepairBox.GuildCheckBox.Checked = false;
                GameScene.Game.NPCSpecialRepairBox.GuildCheckBox.Enabled = false;

                GameScene.Game.NPCSpecialRepairBox.GuildStorageButton.Enabled = false;
                return;
            }

            EditNoticeButton.Visible = (GuildInfo.Permission & GuildPermission.EditNotice) == GuildPermission.EditNotice;

            StorageGrid.ReadOnly = (GuildInfo.Permission & GuildPermission.Storage) != GuildPermission.Storage;

            AddMemberPanel.Enabled = (GuildInfo.Permission & GuildPermission.AddMember) == GuildPermission.AddMember;
            EditDefaultMemberButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            TreasuryPanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            UpgradePanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;
            GuildWarPanel.Enabled = (GuildInfo.Permission & GuildPermission.StartWar) == GuildPermission.StartWar;
            GuildAllyPanel.Enabled = (GuildInfo.Permission & GuildPermission.Alliance) == GuildPermission.Alliance;
            FlagPanel.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            foreach (KeyValuePair<CastleInfo, GuildCastlePanel> pair in CastlePanels)
                pair.Value.RequestButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            //市场，购买维修
            GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsMerchant) == GuildPermission.FundsMerchant;
            GameScene.Game.NPCSpecialRepairBox.GuildCheckBox.Enabled = (GuildInfo.Permission & GuildPermission.FundsRepair) == GuildPermission.FundsRepair;
            GameScene.Game.NPCSpecialRepairBox.GuildStorageButton.Enabled = (GuildInfo.Permission & GuildPermission.Storage) == GuildPermission.Storage;

            //资金
            WithdrawButton.Enabled = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            if (!GameScene.Game.NPCGoodsBox.GuildCheckBox.Enabled)
                GameScene.Game.NPCGoodsBox.GuildCheckBox.Checked = false;

            if (!GameScene.Game.NPCSpecialRepairBox.GuildCheckBox.Enabled)
                GameScene.Game.NPCSpecialRepairBox.GuildCheckBox.Checked = false;
        }

        #region Create Tab
        /// <summary>
        /// 创建页面和新手行会页面
        /// </summary>
        public void CreateCreateTab()
        {
            CreateTab = new DXTab
            {
                TabButton = { Label = { Text = "创建".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };

            DXLabel stepLabel = new DXLabel
            {
                Text = "第一步名称".Lang(),
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, 20)
            };

            DXLabel label = new DXLabel
            {
                Text = "行会名称".Lang(),
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point((CreateTab.Size.Width - label.Size.Width - 5 - 110) / 2, stepLabel.Location.Y + 30);

            GuildNameBox = new DXTextBox
            {
                Size = new Size(110, 18),
                Location = new Point((CreateTab.Size.Width - label.Size.Width - 5 - 110) / 2 + label.Size.Width + 5, label.Location.Y),
                Parent = CreateTab,
            };
            GuildNameBox.TextBox.TextChanged += GuildNameTextBox_TextChanged;

            stepLabel = new DXLabel
            {
                Text = "第二步支付".Lang(),
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, label.Location.Y + 50)
            };

            GoldCheckBox = new DXCheckBox
            {
                Label = { Text = $"CreateGuild.Gold".Lang(CEnvir.ClientControl.GuildCreationCostEdit.ToString("#,##0")), ForeColour = Color.White },
                Parent = CreateTab,
                Checked = true,
                ReadOnly = true,
            };
            GoldCheckBox.Location = new Point((CreateTab.Size.Width - GoldCheckBox.Size.Width) / 2, stepLabel.Location.Y + 30);
            GoldCheckBox.MouseClick += (o, e) => GoldCheckBox.Checked = true;

            HornCheckBox = new DXCheckBox
            {
                Label = { Text = "沃玛号角".Lang(), ForeColour = Color.White },
                Parent = CreateTab,
                ReadOnly = true,
            };
            HornCheckBox.Location = new Point((CreateTab.Size.Width - HornCheckBox.Size.Width) / 2, GoldCheckBox.Location.Y + 20);
            HornCheckBox.MouseClick += (o, e) => HornCheckBox.Checked = true;

            GoldCheckBox.CheckedChanged += (o, e) =>
            {
                TotalCostBox.Value = TotalCost;
                HornCheckBox.Checked = !GoldCheckBox.Checked;
            };
            HornCheckBox.CheckedChanged += (o, e) =>
            {
                TotalCostBox.Value = TotalCost;
                GoldCheckBox.Checked = !HornCheckBox.Checked;
            };

            stepLabel = new DXLabel
            {
                Text = "第三步选项".Lang(),
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, HornCheckBox.Location.Y + 50)
            };

            label = new DXLabel
            {
                Text = "扩展成员".Lang(),
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, stepLabel.Location.Y + 30);

            MemberTextBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MinValue = 0,
                MaxValue = 30,
            };
            MemberTextBox.ValueChanged += MemberTextBox_ValueChanged;

            label = new DXLabel
            {
                Text = "[?]",
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                //IsControl = false,
                Hint = $"CreateGuild.Member".Lang(CEnvir.ClientControl.GuildMemberCostEdit),
            };
            label.Location = new Point(MemberTextBox.Location.X + MemberTextBox.Size.Width, MemberTextBox.Location.Y + (MemberTextBox.Size.Height - label.Size.Height) / 2);

            label = new DXLabel
            {
                Text = "扩展行会仓库".Lang(),
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, MemberTextBox.Location.Y + 20);

            StorageTextBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MinValue = 0,
                MaxValue = 90,
            };
            StorageTextBox.ValueChanged += StorageTextBox_ValueChanged;

            label = new DXLabel
            {
                Text = "[?]",
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                //IsControl = false,
                Hint = $"CreateGuild.Storage".Lang(CEnvir.ClientControl.GuildStorageCostEdit),
            };
            label.Location = new Point(StorageTextBox.Location.X + StorageTextBox.Size.Width, StorageTextBox.Location.Y + (StorageTextBox.Size.Height - label.Size.Height) / 2);

            stepLabel = new DXLabel
            {
                Text = "第四步费用".Lang(),
                Parent = CreateTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(CreateTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, StorageTextBox.Location.Y + 50)
            };

            label = new DXLabel
            {
                Text = "所需金币".Lang(),
                Parent = CreateTab,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = Color.White,
                IsControl = false,
            };
            label.Location = new Point(GuildNameBox.Location.X - label.Size.Width, stepLabel.Location.Y + 30);

            TotalCostBox = new DXNumberTextBox
            {
                Size = new Size(110, 18),
                Location = new Point(GuildNameBox.Location.X, label.Location.Y),
                Parent = CreateTab,
                MaxValue = 2000000000,
                Value = TotalCost,
                ReadOnly = true,
            };
            TotalCostBox.ValueChanged += TotalCostBox_ValueChanged;

            CreateButton = new DXButton
            {
                Parent = CreateTab,
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = "创建行会".Lang() },
                Location = new Point(TotalCostBox.Location.X, TotalCostBox.Location.Y + 30)
            };

            CreateButton.MouseClick += CreateButton_MouseClick;

            StarterGuildButton = new DXButton
            {
                Parent = CreateTab,
                ButtonType = ButtonType.SmallButton,
                Size = new Size(120, SmallButtonHeight),
                Label = { Text = "加入新手行会".Lang() },
                Location = new Point(ClientArea.Left, TotalCostBox.Location.Y + 40)
            };
            StarterGuildButton.MouseClick += StarterGuildButton_MouseClick;
        }
        /// <summary>
        /// 鼠标点击新手行会加入按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StarterGuildButton_MouseClick(object sender, MouseEventArgs e)
        {
            CEnvir.Enqueue(new C.JoinStarterGuild
            {
            });
        }
        /// <summary>
        /// 鼠标点击创建行会按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateButton_MouseClick(object sender, MouseEventArgs e)
        {
            CreateAttempted = true;

            CEnvir.Enqueue(new C.GuildCreate
            {
                Name = GuildNameBox.TextBox.Text,
                UseGold = GoldCheckBox.Checked,
                Members = MemberLimit,
                Storage = StorageSize,
            });
        }
        /// <summary>
        /// 行会名字输入改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuildNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GuildNameValid = Globals.GuildNameRegex.IsMatch(GuildNameBox.TextBox.Text);

            if (string.IsNullOrEmpty(GuildNameBox.TextBox.Text))
                GuildNameBox.BorderColour = Color.FromArgb(198, 166, 99);
            else if (GuildNameValid)
                GuildNameBox.BorderColour = Color.Green;
            else
                GuildNameBox.BorderColour = Color.Red;
        }
        /// <summary>
        /// 行会仓库数值变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageTextBox_ValueChanged(object sender, EventArgs e)
        {
            StorageSize = (int)StorageTextBox.Value;
        }
        /// <summary>
        /// 所需花费输入改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TotalCostBox_ValueChanged(object sender, EventArgs e)
        {
            TotalCostBox.BorderColour = TotalCostBox.Value > GameScene.Game.User.Gold ? Color.Red : Color.FromArgb(198, 166, 99);
        }
        /// <summary>
        /// 成员数量改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberTextBox_ValueChanged(object sender, EventArgs e)
        {
            MemberLimit = (int)MemberTextBox.Value;
        }

        #endregion

        #region Home Tab

        /// <summary>
        /// 创建行会主页
        /// </summary>
        private void CreateHomeTab()
        {
            HomeTab = new DXTab
            {
                TabButton = { Label = { Text = "主页".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };

            new DXLabel
            {
                Text = "公告".Lang(),
                Parent = HomeTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(5, 6),
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            NoticeScrollBar = new DXVScrollBar
            {
                Parent = HomeTab,
                Location = new Point(HomeTab.Size.Width - 25, 25),
                Size = new Size(24, 350),
                VisibleSize = 17,
                Change = 1,
            };
            NoticeScrollBar.ValueChanged += (o, e) => SetLineIndex(NoticeScrollBar.Value);
            //为滚动条自定义皮肤 -1为不设置
            NoticeScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            NoticeTextBox = new DXTextBox
            {
                Parent = HomeTab,
                TextBox = { Multiline = true },
                Location = new Point(5, 25),
                Size = new Size(HomeTab.Size.Width - 37, 275),
                KeepFocus = false,
                Editable = false,
                MaxLength = Globals.MaxGuildNoticeLength,
                Opacity = 0.5F,
            };
            NoticeTextBox.TextBox.TextChanged += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseMove += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseDown += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.MouseUp += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyDown += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyUp += (o, e) => UpdateNoticePosition();
            NoticeTextBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar == (char)1)
                {
                    NoticeTextBox.TextBox.SelectAll();
                    e.Handled = true;
                }

                UpdateNoticePosition();
            };
            //NoticeTextBox.TextBox.MouseWheel += (o, e) => NoticeScrollBar.Value -= e.Delta / SystemInformation.MouseWheelScrollDelta;
            //NoticeTextBox.MouseWheel += (o, e) => NoticeScrollBar.Value -= e.Delta / SystemInformation.MouseWheelScrollDelta;

            EditNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 96, 5),
                Label = { Text = "编辑".Lang() },
                ButtonType = ButtonType.SmallButton
            };
            EditNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = false;
                SaveNoticeButton.Visible = true;
                CancelNoticeButton.Visible = true;
                NoticeTextBox.Editable = true;
                NoticeTextBox.SetFocus();
            };

            SaveNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 161, 5),
                Label = { Text = "保存".Lang() },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            SaveNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = true;
                SaveNoticeButton.Visible = false;
                CancelNoticeButton.Visible = false;
                NoticeTextBox.Editable = false;

                CEnvir.Enqueue(new C.GuildEditNotice { Notice = NoticeTextBox.TextBox.Text });
            };

            CancelNoticeButton = new DXButton
            {
                Parent = HomeTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(HomeTab.Size.Width - 96, 5),
                Label = { Text = "取消".Lang() },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            CancelNoticeButton.MouseClick += (o, e) =>
            {
                EditNoticeButton.Visible = true;
                SaveNoticeButton.Visible = false;
                CancelNoticeButton.Visible = false;
                NoticeTextBox.Editable = false;

                NoticeTextBox.TextBox.Text = GuildInfo.Notice;
            };

            DXControl panel = new DXControl
            {
                Parent = HomeTab,
                Location = new Point(5, NoticeTextBox.Size.Height + 5 + NoticeTextBox.Location.Y),
                Size = new Size(HomeTab.Size.Width - 37, 90),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "行会等级".Lang(),
                Parent = panel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(HomeTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Location = new Point(0, 0),
            };

            GuildLevel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(70, 1),
            };

            DXLabel label = new DXLabel
            {
                Text = "成员".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 30);

            MemberLimitLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = "升级需求".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(260 - label.Size.Width, 3);

            GuildLevelExp = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(260, label.Location.Y),
            };

            GuildLevelButton = new DXButton
            {
                Parent = panel,
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "升级".Lang() },
            };
            GuildLevelButton.Location = new Point(400 + GuildLevelButton.Size.Width, 1);
            GuildLevelButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildUpdate { });
            };

            label = new DXLabel
            {
                Text = "行会资金".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(370 - label.Size.Width, 30);

            GuildFundLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(370, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = "今日增长".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
                Visible = false,
            };
            label.Location = new Point(120 - label.Size.Width, 60);

            DailyGrowthLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
                Visible = false,
            };

            label = new DXLabel
            {
                Text = "总活跃度".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(370 - label.Size.Width, 45);

            TotalContributionLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(370, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = "今日活跃度".Lang(),
                Parent = panel,
                Outline = true,
                IsControl = false,
            };
            label.Location = new Point(120 - label.Size.Width, 45);

            DailyContributionLabel = new DXLabel
            {
                Parent = panel,
                Outline = true,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(120, label.Location.Y),
            };

            label = new DXLabel
            {
                Text = $"备注：行会成员捐赠{CEnvir.ClientControl.PersonalGoldRatio / 10000}万金币或杀怪获{CEnvir.ClientControl.PersonalExpRatio / 10000}万经验，计1点行会活跃度。每天上限{CEnvir.ClientControl.ActivationCeiling}点活跃度。\n行会升级必须由领袖操作，且须消耗行会活跃度和行会资金。",
                Parent = panel,
                Outline = true,
                IsControl = false,
                ForeColour = Color.Red,
            };
            label.Location = new Point(10, 65);
        }
        /// <summary>
        /// 更新公告信息位置
        /// </summary>
        public void UpdateNoticePosition()
        {
            //NoticeScrollBar.MaxValue = NoticeTextBox.TextBox.GetLineFromCharIndex(NoticeTextBox.TextBox.TextLength) + 1;
            NoticeScrollBar.Value = GetCurrentLine();
        }
        /// <summary>
        /// 获取当前信息
        /// </summary>
        /// <returns></returns>
        private int GetCurrentLine()
        {
            //return SendMessage(NoticeTextBox.TextBox.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
            return 0;
        }

        //const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        //const int EM_LINESCROLL = 0x00B6;

        //[DllImport("user32.dll")]
        /// <summary>
        /// 发送信息
        /// </summary>
        //static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        /// <summary>
        /// 设置行索引
        /// </summary>
        /// <param name="lineIndex"></param>
        private void SetLineIndex(int lineIndex)
        {
            int line = GetCurrentLine();
            if (line == lineIndex) return;

            //SendMessage(NoticeTextBox.TextBox.Handle, EM_LINESCROLL, 0, lineIndex - GetCurrentLine());
            NoticeTextBox.DisposeTexture();
        }

        #endregion

        #region Member Tab

        /// <summary>
        /// 行会成员页面
        /// </summary>
        public void CreateMemberTab()
        {
            MemberTab = new DXTab
            {
                TabButton = { Label = { Text = "成员".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };

            new GuildMemberRow
            {
                Location = new Point(5, 10),
                Parent = MemberTab,
                IsHeader = true,
            };

            MemberScrollBar = new DXVScrollBar   //滚动条
            {
                Parent = MemberTab,
                Location = new Point(MemberTab.Size.Width - 25, 16),
                Size = new Size(24, 370),
                VisibleSize = 16,
                Change = 1,
            };
            MemberScrollBar.ValueChanged += MemberScrollBar_ValueChanged;
            //为滚动条自定义皮肤 -1为不设置
            MemberScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            MemberRows = new GuildMemberRow[16];  //成员显示行数
            for (int i = 0; i < MemberRows.Length; i++)
            {
                MemberRows[i] = new GuildMemberRow
                {
                    Parent = MemberTab,
                    Location = new Point(5, 5 + i * 23 + 23),
                    Visible = false
                };

                MemberRows[i].FreeDrag += Member_DoFreeDrag;
            }
            FreeDrag += Member_DoFreeDrag;
        }
        /// <summary>
        /// 滚动条变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateMemberRows();
        }
        public void Member_DoFreeDrag(object sender, TouchEventArgs e)
        {
            int value = (int)e.Delta.Y;
            MemberScrollBar.Value -= value;
        }
        /// <summary>
        /// 更新成员行数
        /// </summary>
        public void UpdateMemberRows()
        {
            if (GuildInfo == null) return;

            MemberScrollBar.MaxValue = GuildInfo.Members.Count;


            for (int i = 0; i < MemberRows.Length; i++)
            {
                MemberRows[i].MemberInfo = i + MemberScrollBar.Value >= GuildInfo.Members.Count ? null : GuildInfo.Members[i + MemberScrollBar.Value];
                //更新申请列表
                GuildApplications.RemoveAll(x => x.PlayerIndex == MemberRows[i].MemberInfo.PlayerIndex);
                ReformatApplicationLines();
            }
        }

        #endregion

        #region Storage Tab

        /// <summary>
        /// 行会仓库页面
        /// </summary>
        public void CreateStorageTab()
        {
            StorageTab = new DXTab
            {
                TabButton = { Label = { Text = "行会仓库".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };

            DXControl filterPanel = new DXControl
            {
                Parent = StorageTab,
                Size = new Size(StorageTab.Size.Width - 38, 26),
                Location = new Point(6, 10),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "名称".Lang(),
            };

            ItemNameTextBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(180, 20),
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
            };
            ItemNameTextBox.TextBox.TextChanged += (o, e) => ApplyStorageFilter();

            label = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(ItemNameTextBox.Location.X + ItemNameTextBox.Size.Width + 10, 5),
                Text = "物品".Lang(),
            };

            ItemTypeComboBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(label.Location.X + label.Size.Width + 5, label.Location.Y),
                Size = new Size(95, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
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
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(ItemTypeComboBox.Location.X + ItemTypeComboBox.Size.Width + 33, label.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "清除".Lang() }
            };
            ClearButton.MouseClick += (o, e) =>
            {
                ItemTypeComboBox.ListBox.SelectItem(null);
                ItemNameTextBox.TextBox.Text = string.Empty;
            };

            StorageGrid = new DXItemGrid             //行会仓库格子
            {
                Parent = StorageTab,
                GridSize = new Size(1, 1),
                Location = new Point(25, 47),
                GridType = GridType.GuildStorage,
                ItemGrid = GuildStorage,
                VisibleHeight = 9,                   //格子高度9个格子位
            };
            StorageGrid.GridSizeChanged += StorageGrid_GridSizeChanged;    //格子改变

            StorageScrollBar = new DXVScrollBar         //行会仓库格子滚动条
            {
                Parent = StorageTab,
                Location = new Point(StorageTab.Size.Width - 20, 43),
                Size = new Size(14, 349),
                VisibleSize = 9,                       //可见尺寸为9格
                Change = 1,                             //改变为1格
            };
            StorageScrollBar.ValueChanged += StorageScrollBar_ValueChanged;  //滚动值变化
            //为滚动条自定义皮肤 -1为不设置
            StorageScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

        }
        /// <summary>
        /// 行会仓库格子容量改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in StorageGrid.Grid)                   //遍历数组 道具单元在行会仓库格子里
                cell.ItemChanged += (o, e1) => FilterCell(cell);
        }
        /// <summary>
        /// 应用行会仓库过滤
        /// </summary>
        public void ApplyStorageFilter()
        {
            foreach (DXItemCell cell in StorageGrid.Grid)         //遍历数组 道具单元在行会仓库格子里
                FilterCell(cell);
        }
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="cell"></param>
        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= GuildInfo?.StorageLimit)         //如果 单元槽>=帮会信息.存储上限
            {
                cell.Enabled = false;         //那么 单元启用为false
                return;
            }

            //如果 单元道具为空 或 项目类型组合.选定项目为空 或 字符串为空(项目名称文本框.文本框.文本)
            if (cell.Item == null && (ItemTypeComboBox.SelectedItem != null || !string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text)))
            {
                cell.Enabled = false;   //那么 单元启用为false
                return;
            }

            //如果 项目类型组合.选定项目为空 或 单元道具为空 或 单元格项信息项类型不等项目类型组合.选定项目
            if (ItemTypeComboBox.SelectedItem != null && cell.Item != null && cell.Item.Info.ItemType != (ItemType)ItemTypeComboBox.SelectedItem)
            {
                cell.Enabled = false;  //那么 单元启用为false
                return;
            }

            //如果 字符串为空(项目名称文本框.文本框.文本) 或 单元道具为空 或 单元格项信息项名称索引(项目名称文本框.文本框.文本,字符串比较) < 0
            if (!string.IsNullOrEmpty(ItemNameTextBox.TextBox.Text) && cell.Item != null && cell.Item.Info.ItemName.IndexOf(ItemNameTextBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0)
            {
                cell.Enabled = false;
                return;
            }
            cell.Enabled = true;   //单元启用
        }

        /// <summary>
        /// 行会仓库滚动条更改值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageScrollBar_ValueChanged(object sender, EventArgs e)
        {
            StorageGrid.ScrollValue = StorageScrollBar.Value;
        }

        #endregion

        #region Manage Tab

        /// <summary>
        /// 行会管理页面
        /// </summary>
        public void CreateManageTab()
        {
            ManageTab = new DXTab
            {
                TabButton = { Label = { Text = "管理".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };
            ManageTab.TabButton.MouseClick += TabButton_MouseClick;

            AddMemberPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 10),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 83),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "邀请成员".Lang(),
                Parent = AddMemberPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(AddMemberPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            DXLabel label = new DXLabel
            {
                Parent = AddMemberPanel,
                Text = "成员名字".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 40);

            AddMemberTextBox = new DXTextBox
            {
                Parent = AddMemberPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(110, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };

            AddMemberButton = new DXButton
            {
                Parent = AddMemberPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "邀请".Lang() },
            };
            AddMemberButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildInviteMember { Name = AddMemberTextBox.TextBox.Text });
                AddMemberButton.Enabled = false;
                AddMemberTextBox.Enabled = false;
            };

            EditDefaultMemberPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(10 + (HomeTab.Size.Width - 11 - 5) / 2, 10),
                Size = new Size((HomeTab.Size.Width - 16) / 2 - 25, 83),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "成员权限".Lang(),
                Parent = EditDefaultMemberPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(AddMemberPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            EditDefaultMemberButton = new DXButton
            {
                Parent = EditDefaultMemberPanel,
                Location = new Point(ClientArea.X + 60, label.Location.Y),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = "设定权限等级".Lang() },
            };
            EditDefaultMemberButton.MouseClick += (o, e) =>
            {
                GameScene.Game.GuildMemberBox.MemberNameLabel.Text = "默认成员".Lang();
                GameScene.Game.GuildMemberBox.RankTextBox.TextBox.Text = GuildInfo.DefaultRank;
                GameScene.Game.GuildMemberBox.Permission = GuildInfo.DefaultPermission;
                GameScene.Game.GuildMemberBox.MemberIndex = 0;

                GameScene.Game.GuildMemberBox.Visible = true;
                GameScene.Game.GuildMemberBox.BringToFront();
            };

            TreasuryPanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 98),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 73),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "财务".Lang(),
                Parent = TreasuryPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(TreasuryPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "行会税收".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            GuildTaxBox = new DXNumberTextBox
            {
                Parent = TreasuryPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(60, 20),
                MaxValue = 100,
                MinValue = 0,
            };
            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "(" + "百分比".Lang() + ")",
                Location = new Point(ClientArea.X + 115, 32),
            };

            SetTaxButton = new DXButton
            {
                Parent = TreasuryPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "更改".Lang() },
            };
            SetTaxButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildTax { Tax = GuildTaxBox.Value });

                GuildTaxBox.Enabled = false;
                SetTaxButton.Enabled = false;
            };

            label = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "行会资金".Lang(),
                Visible = false
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 54);

            GuildFundLabel1 = new DXLabel
            {
                Parent = TreasuryPanel,
                Text = "10,000,000,000",
                Location = new Point(ClientArea.X + 52, label.Location.Y),
                ForeColour = Color.White,
                Visible = false
            };

            UpgradePanel = new DXControl
            {
                Parent = ManageTab,
                Location = new Point(5, 176),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 90),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            new DXLabel
            {
                Text = "升级".Lang(),
                Parent = UpgradePanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(UpgradePanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            label = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "成员".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 35);

            IncreaseMemberButton = new DXButton
            {
                Parent = UpgradePanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = $"升级".Lang() + $" ({CEnvir.ClientControl.GuildMemberCostEdit:#,##0})" },
            };
            IncreaseMemberButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildIncreaseMember());
                IncreaseMemberButton.Enabled = false;
            };

            DXLabel label1 = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "限制".Lang(),
            };
            label1.Location = new Point(ClientArea.X + 205 - label1.Size.Width, label.Location.Y);

            MemberLimitLabel1 = new DXLabel
            {
                Parent = UpgradePanel,
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 205, label.Location.Y)
            };

            label = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "行会仓库".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 60);

            IncreaseStorageButton = new DXButton
            {
                Parent = UpgradePanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(110, SmallButtonHeight),
                Label = { Text = $"升级".Lang() + $" ({CEnvir.ClientControl.GuildStorageCostEdit:#,##0})" },
            };
            IncreaseStorageButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildIncreaseStorage());
                IncreaseStorageButton.Enabled = false;
            };

            label1 = new DXLabel
            {
                Parent = UpgradePanel,
                Text = "限制".Lang(),
            };
            label1.Location = new Point(ClientArea.X + 205 - label1.Size.Width, label.Location.Y);

            StorageSizeLabel = new DXLabel
            {
                Parent = UpgradePanel,
                ForeColour = Color.White,
                Location = new Point(ClientArea.X + 205, label.Location.Y)
            };

            FlagPanel = new GuildFlagPanel
            {
                Parent = ManageTab,
                Location = new Point(10 + (HomeTab.Size.Width - 11 - 5) / 2, 97),
            };

            ApplicationPanel = new DXControl   //入会申请列表框
            {
                Parent = ManageTab,
                Location = new Point(5, 270),
                Size = new Size((HomeTab.Size.Width - 37), 128),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };
            label = new DXLabel
            {
                Parent = ApplicationPanel,
                Text = "申请列表".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(UpgradePanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            AllowApplyCheckBox = new DXCheckBox
            {
                Parent = ApplicationPanel,
                Label = { Text = "接受新申请".Lang() },
                Location = new Point(label.Location.X + label.Size.Width, label.Location.Y + 1),
            };
            AllowApplyCheckBox.CheckedChanged += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildAllowApplyChanged { AllowApply = AllowApplyCheckBox.Checked });
            };

            ScrollBar = new DXVScrollBar       //滚动条
            {
                Parent = ApplicationPanel,
                Size = new Size(14, ApplicationPanel.Size.Height - 3 - 21),
                Location = new Point(ApplicationPanel.Size.Width - 16, 20),
                VisibleSize = 5,
                Change = 1,
            };
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            ApplicationPanel.MouseWheel += ScrollBar.DoMouseWheel;


        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            ReformatApplicationLines();
        }

        private void TabButton_MouseClick(object sender, MouseEventArgs e)  //鼠标点击更新申请列
        {
            //更新申请列表
            if ((GameScene.Game.GuildBox.GuildInfo.Permission & GuildPermission.AddMember) == GuildPermission.AddMember)
            {
                CEnvir.Enqueue(new C.GetGuildApplications());
            }
        }

        public void PopulateApplicationList(List<string> list)  //更新行会申请列表
        {
            foreach (GuildApplicationLine line in GuildApplications)
            {
                line.Dispose();
            }
            GuildApplications.Clear();

            bool canApproveOrReject = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            foreach (string line in list)
            {
                List<string> applicant = line.Split(',').ToList();
                if (applicant.Count != 6) continue;


                GuildApplicationLine newLine = new GuildApplicationLine();
                newLine.Parent = ApplicationPanel;

                newLine.PlayerIndex = int.Parse(applicant[0]);
                newLine.OnlineImage.Index = (bool.Parse(applicant[1])) ? 3625 : 3624;
                newLine.NameLabel.Text = applicant[2];
                newLine.ClassLabel.Text = applicant[3];
                newLine.LevelLabel.Text = applicant[4];
                newLine.LastLoginLabel.Text = applicant[5];

                newLine.ViewButton.Enabled = (bool.Parse(applicant[1]));
                newLine.ApproveButton.Enabled = canApproveOrReject;
                newLine.RejectButton.Enabled = canApproveOrReject;
                GuildApplications.Add(newLine);
            }

            //TODO 排序么
            ScrollBar.MaxValue = GuildApplications.Count;
            ReformatApplicationLines();
        }

        public void ReformatApplicationLines()
        {
            for (int i = 0; i < GuildApplications.Count; i++)
            {
                GuildApplicationLine line = GuildApplications[i];

                if (i >= ScrollBar.Value && i <= ScrollBar.Value + 4)
                {
                    line.Visible = true;
                    line.Location = new Point(5, 20 * (i - ScrollBar.Value + 1) + 5);
                }
                else
                {
                    line.Visible = false;
                }

            }
        }

        #endregion

        #region WarTab

        /// <summary>
        /// 行会战争管理页面
        /// </summary>
        public void CreateWarTab()
        {
            WarTab = new DXTab
            {
                TabButton = { Label = { Text = "战争".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };

            GuildWarPanel = new DXControl
            {
                Parent = WarTab,
                Location = new Point(5, 10),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 83),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            new DXLabel
            {
                Text = "行会战争".Lang(),
                Parent = GuildWarPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(GuildWarPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            DXLabel label = new DXLabel
            {
                Parent = GuildWarPanel,
                Text = "目标行会".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            GuildWarTextBox = new DXTextBox
            {
                Parent = GuildWarPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(110, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };
            GuildWarTextBox.TextBox.TextChanged += GuildWarTextBox_TextChanged;
            GuildWarTextBox.TextBox.KeyPress += GuildWarTextBox_KeyPress;

            StartWarButton = new DXButton
            {
                Parent = GuildWarPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "开战".Lang() },
                Enabled = false,
            };
            StartWarButton.MouseClick += (o, e) => StartWar();

            label = new DXLabel
            {
                Parent = GuildWarPanel,
                Text = $"GuildWar.Gold".Lang(CEnvir.ClientControl.GuildWarCostEdit.ToString("#,##0")),
                Location = new Point(ClientArea.X + 55, label.Location.Y + 25),
            };

            GuildAllyPanel = new DXControl
            {
                Parent = WarTab,
                Location = new Point(5, 98),
                Size = new Size((HomeTab.Size.Width - 11 - 5) / 2, 168),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
                Visible = false,
            };

            new DXLabel
            {
                Text = "联盟".Lang(),
                Parent = GuildAllyPanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(GuildAllyPanel.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            label = new DXLabel
            {
                Parent = GuildAllyPanel,
                Text = "行会".Lang(),
            };
            label.Location = new Point(ClientArea.X + 55 - label.Size.Width, 32);

            GuildAllyTextBox = new DXTextBox
            {
                Parent = GuildAllyPanel,
                Location = new Point(ClientArea.X + 55, label.Location.Y),
                Size = new Size(110, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };
            GuildAllyTextBox.TextBox.TextChanged += GuildAllyTextBox_TextChanged;
            GuildAllyTextBox.TextBox.KeyPress += GuildAllyTextBox_KeyPress;

            StartAllyButton = new DXButton
            {
                Parent = GuildAllyPanel,
                Location = new Point(ClientArea.X + 170, label.Location.Y - 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(60, SmallButtonHeight),
                Label = { Text = "联盟".Lang() },
                Enabled = false,
            };
            StartAllyButton.MouseClick += (o, e) => StartAlly();

            new GuildAllianceRow
            {
                Location = new Point(5, 55),
                Parent = GuildAllyPanel,
                IsHeader = true,
            };

            AllianceScrollBar = new DXVScrollBar
            {
                Parent = GuildAllyPanel,
                Location = new Point(GuildAllyPanel.Size.Width - 20, 78),
                Size = new Size(14, 87),
                VisibleSize = 4,
                Change = 1,
            };
            AllianceScrollBar.ValueChanged += AllianceScrollBar_ValueChanged;

            AllianceRows = new GuildAllianceRow[4];
            for (int i = 0; i < AllianceRows.Length; i++)
            {
                AllianceRows[i] = new GuildAllianceRow
                {
                    Parent = GuildAllyPanel,
                    Location = new Point(5, 55 + i * 23 + 23),
                    Visible = false
                };

                AllianceRows[i].MouseWheel += AllianceScrollBar.DoMouseWheel;
            }
            MouseWheel += AllianceScrollBar.DoMouseWheel;

            int count = 0;
            foreach (CastleInfo castle in CEnvir.CastleInfoList.Binding)
            {
                CastlePanels[castle] = new GuildCastlePanel
                {
                    Parent = WarTab,
                    Castle = castle,
                    Location = new Point(10 + (HomeTab.Size.Width - 11 - 5) / 2, 10 + count * 125),
                    Visible = false,
                };
                count++;
            }

            //NormaWarPanel = new DXControl
            //{
            //    Parent = WarTab,
            //    Location = new Point(10 + (HomeTab.Size.Width - 11 - 5) / 2, 133),
            //    Size = new Size((HomeTab.Size.Width - 11 - 5) / 2 - 25, 133),
            //    Border = true,
            //    BorderColour = Color.FromArgb(198, 166, 99)
            //};

            //new DXLabel
            //{
            //    Text = "诺玛城".Lang(),
            //    Parent = NormaWarPanel,
            //    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            //    ForeColour = Color.FromArgb(198, 166, 99),
            //    Outline = true,
            //    OutlineColour = Color.Black,
            //    IsControl = false,
            //    Size = new Size(AddMemberPanel.Size.Width, 22),
            //    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            //};

            //label = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "当前拥有者".Lang(),
            //};
            //label.Location = new Point(80 - label.Size.Width, 25);

            //DXLabel NormaOwnerLabel = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "无".Lang(),
            //    Location = new Point(80, 25),
            //    ForeColour = Color.White
            //};

            //label = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "攻城时间表".Lang(),
            //};
            //label.Location = new Point(80 - label.Size.Width, 45);


            //DXLabel NormaDateLabel = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "无".Lang(),
            //    Location = new Point(80, 45),
            //    ForeColour = Color.White
            //};

            //DXButton NormaRequestButton = new DXButton
            //{
            //    Parent = NormaWarPanel,
            //    Location = new Point(80, 75),
            //    ButtonType = ButtonType.SmallButton,
            //    Size = new Size(100, SmallButtonHeight),
            //    Label = { Text = "提交".Lang() },
            //    Enabled = false,
            //};

            //label = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "花费".Lang(),
            //};
            //label.Location = new Point(80 - label.Size.Width, 95);

            //DXLabel NormaItemLabel = new DXLabel
            //{
            //    Parent = NormaWarPanel,
            //    Text = "未知".Lang(),
            //    Location = new Point(80, 95),
            //    ForeColour = Color.White
            //};
        }
        /// <summary>
        /// 更新联盟滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllianceScrollBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateAllianceRows();
        }
        /// <summary>
        /// 更新联盟公会行数
        /// </summary>
        public void UpdateAllianceRows()
        {
            if (GuildInfo == null) return;
            GuildInfo.Alliances.Sort((x, y) => y.OnlineCount.CompareTo(x.OnlineCount));

            AllianceScrollBar.MaxValue = GuildInfo.Alliances.Count;

            for (int i = 0; i < AllianceRows.Length; i++)
                AllianceRows[i].AllianceInfo = i + AllianceScrollBar.Value >= GuildInfo.Alliances.Count ? null : GuildInfo.Alliances[i + AllianceScrollBar.Value];
        }
        /// <summary>
        /// 开始行会站
        /// </summary>
        public void StartWar()
        {
            WarAttempted = true;

            C.GuildWar p = new C.GuildWar
            {
                GuildName = GuildWarTextBox.TextBox.Text,
            };

            CEnvir.Enqueue(p);
        }
        /// <summary>
        /// 行会站信息输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuildWarTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (StartWarButton.Enabled)
                StartWar();
        }
        /// <summary>
        /// 行会站信息输入改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuildWarTextBox_TextChanged(object sender, EventArgs e)
        {
            GuildWarNameValid = Globals.GuildNameRegex.IsMatch(GuildWarTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(GuildWarTextBox.TextBox.Text))
                GuildWarTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                GuildWarTextBox.BorderColour = GuildWarNameValid ? Color.Green : Color.Red;
        }
        /// <summary>
        /// 开始联盟
        /// </summary>
        public void StartAlly()
        {
            AllyAttempted = true;

            C.GuildAlliance p = new C.GuildAlliance
            {
                GuildName = GuildAllyTextBox.TextBox.Text,
            };

            CEnvir.Enqueue(p);
        }
        /// <summary>
        /// 行会联盟信息输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuildAllyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (StartAllyButton.Enabled)
                StartAlly();
        }
        /// <summary>
        /// 行会联盟信息输入改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuildAllyTextBox_TextChanged(object sender, EventArgs e)
        {
            GuildAllyNameValid = Globals.GuildNameRegex.IsMatch(GuildAllyTextBox.TextBox.Text);

            if (string.IsNullOrEmpty(GuildAllyTextBox.TextBox.Text))
                GuildAllyTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                GuildAllyTextBox.BorderColour = GuildAllyNameValid ? Color.Green : Color.Red;
        }

        #endregion

        #region 行会资金

        public void CreateFundTab()
        {
            FundTab = new DXTab
            {
                TabButton = { Label = { Text = "金库".Lang() } },
                Parent = GuildTabs,
                Border = false,
                BackColour = Color.Empty,
            };
            FundTab.TabButton.MouseClick += FundTabButton_MouseClick;

            #region 当前资金

            CurrentFundPanel = new DXControl
            {
                Parent = FundTab,
                Location = new Point(5, 10),
                Size = new Size((HomeTab.Size.Width - 45), 20),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            FundLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "行会资金".Lang() + ": ",
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            };
            FundLabel.Location = new Point(ClientArea.X + 65 - FundLabel.Size.Width, 5);

            FundAmountLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "加载中..."
            };
            FundAmountLabel.Location = new Point(FundLabel.DisplayArea.Right - 20, 5);

            UpperLimitLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "资金上限".Lang() + ": ",
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            };
            UpperLimitLabel.Location = new Point(FundAmountLabel.DisplayArea.Right, 5);

            UpperLimitAmountLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "加载中..."
            };
            UpperLimitAmountLabel.Location = new Point(UpperLimitLabel.DisplayArea.Right - 20, 5);

            GameGoldLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "行会赞助币".Lang() + ": ",
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            };
            GameGoldLabel.Location = new Point(UpperLimitAmountLabel.DisplayArea.Right, 5);

            GameGoldAmountLabel = new DXLabel
            {
                Parent = CurrentFundPanel,
                Text = "0",
            };
            GameGoldAmountLabel.Location = new Point(GameGoldLabel.DisplayArea.Right - 20, 5);

            #endregion

            #region 金库公告

            FundNoticeLabel = new DXLabel
            {
                Text = "金库公告".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(5, CurrentFundPanel.DisplayArea.Bottom - 22),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            FundNoticeTextBox = new DXTextBox
            {
                Parent = FundTab,
                TextBox = { Multiline = true },
                Location = new Point(5, CurrentFundPanel.DisplayArea.Bottom - 20 + FundNoticeLabel.Size.Height + 2),
                Size = new Size((FundTab.Size.Width - 45), 80),
                KeepFocus = false,
                Editable = false,
                MaxLength = Globals.MaxGuildNoticeLength,
                Opacity = 0.5F,
            };

            FundNoticeTextBox.TextBox.KeyPress += (o, e) =>
            {
                if (e.KeyChar != (char)1) return;
                FundNoticeTextBox.TextBox.SelectAll();
                e.Handled = true;
            };

            EditFundNoticeButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width - 100, FundNoticeLabel.Location.Y - 4),
                Label = { Text = "编辑".Lang() },
                ButtonType = ButtonType.SmallButton
            };
            EditFundNoticeButton.MouseClick += (o, e) =>
            {
                EditFundNoticeButton.Visible = false;
                SaveFundNoticeButton.Visible = true;
                CancelFundNoticeButton.Visible = true;
                FundNoticeTextBox.Editable = true;
                FundNoticeTextBox.SetFocus();
            };

            SaveFundNoticeButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width - 180, FundNoticeLabel.Location.Y - 4),
                Label = { Text = "保存".Lang() },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            SaveFundNoticeButton.MouseClick += (o, e) =>
            {
                EditFundNoticeButton.Visible = true;
                SaveFundNoticeButton.Visible = false;
                CancelFundNoticeButton.Visible = false;
                FundNoticeTextBox.Editable = false;

                CEnvir.Enqueue(new C.GuildEditVaultNotice { Notice = FundNoticeTextBox.TextBox.Text });
            };

            CancelFundNoticeButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(60, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width - 96, FundNoticeLabel.Location.Y - 4),
                Label = { Text = "取消".Lang() },
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            CancelFundNoticeButton.MouseClick += (o, e) =>
            {
                EditFundNoticeButton.Visible = true;
                SaveFundNoticeButton.Visible = false;
                CancelFundNoticeButton.Visible = false;
                FundNoticeTextBox.Editable = false;


                FundNoticeTextBox.TextBox.Text = GuildInfo.VaultNotice;
            };

            #endregion

            #region 捐赠排名

            FundRankPanel = new DXControl
            {
                Parent = FundTab,
                Location = new Point(5, 160),
                Size = new Size((FundTab.Size.Width - 45), 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            FundRankNumLabel = new DXLabel
            {
                Text = "排名".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(5, FundRankPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            FundRankNumLabel = new DXLabel
            {
                Text = "角色名".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(115, FundRankPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            FundRankAmountLabel = new DXLabel
            {
                Text = "捐赠金额".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(225, FundRankPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            FundRankGameGoldAmountLabel = new DXLabel
            {
                Text = "捐赠赞助币".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(335, FundRankPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Visible = false,
            };

            FundRanksListBox = new DXListBox
            {
                Parent = FundRankPanel,
                Location = new Point(0, 0),
                Size = new Size(FundRankPanel.Size.Width, FundRankPanel.Size.Height),
                Border = true,
                Opacity = 0f,
            };
            FundRanksListBox.ScrollBar.Visible = true;

            //todo
            //TopRanks.Add(new ClientGuildFundRankInfo{Rank = 1, Name = "机构1", TotalAmount = 100032});
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 2, Name = "asfaf机构2", TotalAmount = 100000 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 3, Name = "机构wqwdeasd3", TotalAmount = 99999 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 4, Name = "机构4", TotalAmount = 88888 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 5, Name = "机构fe5", TotalAmount = 77777 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 6, Name = "机构6", TotalAmount = 66666 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 7, Name = "机构7", TotalAmount = 55555 });
            //TopRanks.Add(new ClientGuildFundRankInfo { Rank = 8, Name = "机构8", TotalAmount = 44444 });

            #endregion

            #region 流水记录

            FundRecordPanel = new DXControl
            {
                Parent = FundTab,
                Location = new Point(5, 265),
                Size = new Size(300, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            FundRankNumLabel = new DXLabel
            {
                Text = "金库流水记录".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(5, FundRecordPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            FundChangesListBox = new DXListBox
            {
                Parent = FundRecordPanel,
                Location = new Point(0, 0),
                Size = new Size(FundRecordPanel.Size.Width, FundRecordPanel.Size.Height),
                Border = true,
                Opacity = 0f,
            };
            FundChangesListBox.ScrollBar.Visible = true;

            ExtractPanel = new DXControl
            {
                Parent = FundTab,
                Location = new Point(320, 265),
                Size = new Size(210, 80),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99),
            };

            ExtractNumLabel = new DXLabel
            {
                Text = "提取赞助币申请".Lang(),
                Parent = FundTab,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                Location = new Point(320, ExtractPanel.DisplayArea.Top - 50),
                IsControl = false,
                Size = new Size(FundTab.Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            GameGoldScrollBar = new DXVScrollBar       //滚动条
            {
                Parent = ExtractPanel,
                Size = new Size(14, ExtractPanel.Size.Height - 5),
                Location = new Point(ExtractPanel.Size.Width - 16, 2),
                VisibleSize = 5,
                Change = 1,
            };
            GameGoldScrollBar.ValueChanged += GameGoldScrollBar_ValueChanged;
            ExtractPanel.MouseWheel += GameGoldScrollBar.DoMouseWheel;

            //TODO
            //RecentChanges.Add(new ClientGuildFundChangeInfo{GuildName = "null", Amount = 100, Name = "avas", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(1)});
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = -651465, Name = "动物趣闻多群无", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(2) });
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = 2151243, Name = "avsdf啊实打实as", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(3) });
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = 1231, Name = "的该色调改", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(4) });
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = -656, Name = "avas", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(5) });
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = -999999, Name = "avas", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(6) });
            //RecentChanges.Add(new ClientGuildFundChangeInfo { GuildName = "null", Amount = 0, Name = "avas", OperationTime = CEnvir.Now - TimeSpan.FromMinutes(7) });


            #endregion

            DepositButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width / 8 - 30, FundRankPanel.DisplayArea.Bottom + 90),
                Label = { Text = "存入资金".Lang() },
                ButtonType = ButtonType.SmallButton
            };

            DepositButton.MouseClick += DepositButtonOnMouseClick;

            WithdrawButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width / 4, FundRankPanel.DisplayArea.Bottom + 90),
                Label = { Text = "取出资金".Lang() },
                ButtonType = ButtonType.SmallButton
            };
            WithdrawButton.MouseClick += WithdrawButtonOnMouseClick;

            DepositGameGoldButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width / 2 + 30, FundRankPanel.DisplayArea.Bottom + 90),
                Label = { Text = "存入赞助币".Lang() },
                ButtonType = ButtonType.SmallButton,
            };
            DepositGameGoldButton.MouseClick += DepositGameGoldButtonOnMouseClick;

            WithdrawGameGoldButton = new DXButton
            {
                Parent = FundTab,
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(FundTab.Size.Width - 150, FundRankPanel.DisplayArea.Bottom + 90),
                Label = { Text = "取出赞助币".Lang() },
                ButtonType = ButtonType.SmallButton,
            };
            WithdrawGameGoldButton.MouseClick += WithdrawGameGoldButtonOnMouseClick;
        }

        private void FundTabButton_MouseClick(object sender, MouseEventArgs e)  //鼠标点击更新申请列表
        {
            CEnvir.Enqueue(new C.GetGuileWithDrawal());
        }

        private void FundChangeScrollBarOnValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FundRankScrollBarOnValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WithdrawButtonOnMouseClick(object sender, MouseEventArgs e)
        {
            if ((GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader)
            {
                string message = "取出行会资金";
                new DXInputWindow((str) =>
                    {
                        long amount = 0;
                        bool isDigit = long.TryParse(str, out amount);
                        if (!isDigit) return false;
                        if (amount < 0 || amount > CurrentFund)
                            return false;
                        return true;
                    },
                    (str) =>
                    {
                        long amount = 0;
                        if (!long.TryParse(str, out amount)) return;
                        if (amount > 0 && amount <= CurrentFund)
                        {
                            CEnvir.Enqueue(new C.GuildWithdrawFund { Amount = amount });
                        }
                    }, message.ToString()
                );
            }
            else
            {
                GameScene.Game.ReceiveChat("只有会长有权取出资金", MessageType.System);
            }
        }

        private void WithdrawGameGoldButtonOnMouseClick(object sender, MouseEventArgs e)
        {
            new DXInputWindow((str) =>
            {
                long amount = 0;
                bool isDigit = long.TryParse(str, out amount);
                if (!isDigit) return false;
                if (amount < 0)
                    return false;
                return true;
            }, (str) =>
            {
                long amount = 0;
                if (!long.TryParse(str, out amount)) return;
                if (amount > 0)
                {
                    CEnvir.Enqueue(new C.GuildGameGold { Type = 1, Amount = amount });
                }
            }, "输入取出赞助币的数额".Lang());
        }

        private void DepositButtonOnMouseClick(object sender, MouseEventArgs e)
        {

            new DXInputWindow((str) =>
            {
                long amount = 0;
                bool isDigit = long.TryParse(str, out amount);
                if (!isDigit) return false;
                if (amount < 0 || amount > GameScene.Game.User.Gold)
                    return false;
                return true;
            }, (str) =>
            {
                long amount = 0;
                if (!long.TryParse(str, out amount)) return;
                if (amount > 0 && amount <= GameScene.Game.User.Gold)
                {
                    CEnvir.Enqueue(new C.GuildDonation { Amount = amount });
                }
            }, "输入捐赠金币的数额".Lang());
        }
        private void DepositGameGoldButtonOnMouseClick(object sender, MouseEventArgs e)
        {
            new DXInputWindow((str) =>
            {
                long amount = 0;
                bool isDigit = long.TryParse(str, out amount);
                if (!isDigit) return false;
                if (amount < 0 || amount > GameScene.Game.User.GameGold)
                    return false;
                return true;
            }, (str) =>
            {
                long amount = 0;
                if (!long.TryParse(str, out amount)) return;
                if (amount > 0 && amount <= GameScene.Game.User.GameGold)
                {
                    CEnvir.Enqueue(new C.GuildGameGold { Type = 0, Amount = amount });
                }
            }, "输入捐赠赞助币的数额".Lang());
        }

        private void GameGoldScrollBar_ValueChanged(object sender, EventArgs e)
        {
            ReformatAppGameGoldLines();
        }

        public void ReformatAppGameGoldLines(List<string> list)  //更新赞助币申请列表
        {
            foreach (GuildGameGoldWithdrawalLine line in GuildGameGoldWithdrawal)
            {
                line.Dispose();
            }
            GuildGameGoldWithdrawal.Clear();

            bool canApproveOrReject = (GuildInfo.Permission & GuildPermission.Leader) == GuildPermission.Leader;

            foreach (string line in list)
            {
                List<string> applicant = line.Split(',').ToList();
                if (applicant.Count != 5) continue;

                GuildGameGoldWithdrawalLine newLine = new GuildGameGoldWithdrawalLine();
                newLine.Parent = ExtractPanel;

                newLine.PlayerIndex = int.Parse(applicant[0]);
                newLine.NameLabel.Text = applicant[2];
                int amount = 0;
                int.TryParse(applicant[3], out amount);
                newLine.GameGoldLabel.Text = (amount / 100).ToString();
                newLine.ApproveButton.Enabled = canApproveOrReject;
                newLine.RejectButton.Enabled = canApproveOrReject;
                GuildGameGoldWithdrawal.Add(newLine);
            }

            GameGoldScrollBar.MaxValue = GuildGameGoldWithdrawal.Count;
            ReformatAppGameGoldLines();
        }

        public void ReformatAppGameGoldLines()
        {
            for (int i = 0; i < GuildGameGoldWithdrawal.Count; i++)
            {
                GuildGameGoldWithdrawalLine line = GuildGameGoldWithdrawal[i];

                if (i >= GameGoldScrollBar.Value && i <= GameGoldScrollBar.Value + 4)
                {
                    line.Visible = true;
                    line.Location = new Point(5, 20 * (i - GameGoldScrollBar.Value + 1) - 20);
                }
                else
                {
                    line.Visible = false;
                }
            }
        }

        public void UpdateFundRanks()  //更新捐赠排行信息
        {
            if (TopRanks == null || TopRanks.Count == 0) return;

            foreach (var item in FundRanksBoxItems)
            {
                item.Dispose();
            }
            FundRanksBoxItems.Clear();

            foreach (var info in TopRanks)
            {
                if (info.TotalAmount == 0) continue;
                FundRanksBoxItems.Add(new DXListBoxItem
                {
                    Parent = FundRanksListBox,
                    Label =
                    {
                        Text = $"  {info.Rank,-8}         {info.Name,-16}{info.TotalAmount,-20}"
                    },
                    Tag = info
                });
            }
        }

        public void UpdateFundChanges()   //更新捐献提取信息
        {
            if (RecentChanges == null || RecentChanges.Count == 0) return;


            foreach (var item in FundChangesBoxItems)
            {
                item.Dispose();
            }
            FundChangesBoxItems.Clear();

            foreach (var info in RecentChanges)
            {
                string direction = info.Amount > 0 ? " 捐献了 " : " 提取了 ";
                FundChangesBoxItems.Add(new DXListBoxItem
                {
                    Parent = FundChangesListBox,
                    Label =
                    {
                        Text = $"  「{info.Name}」 {direction} {Math.Abs(info.Amount)} 金币",
                    },
                    Tag = info,
                });
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (GuildTabs != null)
                {
                    if (!GuildTabs.IsDisposed)
                        GuildTabs.Dispose();

                    GuildTabs = null;
                }

                #region Create Tab

                if (CreateTab != null)
                {
                    if (!CreateTab.IsDisposed)
                        CreateTab.Dispose();

                    CreateTab = null;
                }

                if (GuildNameBox != null)
                {
                    if (!GuildNameBox.IsDisposed)
                        GuildNameBox.Dispose();

                    GuildNameBox = null;
                }

                if (MemberTextBox != null)
                {
                    if (!MemberTextBox.IsDisposed)
                        MemberTextBox.Dispose();

                    MemberTextBox = null;
                }

                if (StorageTextBox != null)
                {
                    if (!StorageTextBox.IsDisposed)
                        StorageTextBox.Dispose();

                    StorageTextBox = null;
                }

                if (TotalCostBox != null)
                {
                    if (!TotalCostBox.IsDisposed)
                        TotalCostBox.Dispose();

                    TotalCostBox = null;
                }

                if (GoldCheckBox != null)
                {
                    if (!GoldCheckBox.IsDisposed)
                        GoldCheckBox.Dispose();

                    GoldCheckBox = null;
                }

                if (HornCheckBox != null)
                {
                    if (!HornCheckBox.IsDisposed)
                        HornCheckBox.Dispose();

                    HornCheckBox = null;
                }

                _MemberLimit = 0;
                MemberLimitChanged = null;

                _StorageSize = 0;
                StorageSizeChanged = null;

                _GuildNameValid = false;
                GuildNameValidChanged = null;

                _CreateAttempted = false;
                CreateAttemptedChanged = null;

                if (CreateButton != null)
                {
                    if (!CreateButton.IsDisposed)
                        CreateButton.Dispose();

                    CreateButton = null;
                }

                #endregion

                #region Home Tab

                if (HomeTab != null)
                {
                    if (!HomeTab.IsDisposed)
                        HomeTab.Dispose();

                    HomeTab = null;
                }

                if (MemberLimitLabel != null)
                {
                    if (!MemberLimitLabel.IsDisposed)
                        MemberLimitLabel.Dispose();

                    MemberLimitLabel = null;
                }

                if (GuildFundLabel != null)
                {
                    if (!GuildFundLabel.IsDisposed)
                        GuildFundLabel.Dispose();

                    GuildFundLabel = null;
                }

                if (DailyGrowthLabel != null)
                {
                    if (!DailyGrowthLabel.IsDisposed)
                        DailyGrowthLabel.Dispose();

                    DailyGrowthLabel = null;
                }

                if (TotalContributionLabel != null)
                {
                    if (!TotalContributionLabel.IsDisposed)
                        TotalContributionLabel.Dispose();

                    TotalContributionLabel = null;
                }

                if (DailyContributionLabel != null)
                {
                    if (!DailyContributionLabel.IsDisposed)
                        DailyContributionLabel.Dispose();

                    DailyContributionLabel = null;
                }

                if (GuildLevelButton != null)
                {
                    if (!GuildLevelButton.IsDisposed)
                        GuildLevelButton.Dispose();

                    GuildLevelButton = null;
                }

                if (NoticeScrollBar != null)
                {
                    if (!NoticeScrollBar.IsDisposed)
                        NoticeScrollBar.Dispose();

                    NoticeScrollBar = null;
                }

                if (NoticeTextBox != null)
                {
                    if (!NoticeTextBox.IsDisposed)
                        NoticeTextBox.Dispose();

                    NoticeTextBox = null;
                }

                if (EditNoticeButton != null)
                {
                    if (!EditNoticeButton.IsDisposed)
                        EditNoticeButton.Dispose();

                    EditNoticeButton = null;
                }

                if (SaveNoticeButton != null)
                {
                    if (!SaveNoticeButton.IsDisposed)
                        SaveNoticeButton.Dispose();

                    SaveNoticeButton = null;
                }

                if (CancelNoticeButton != null)
                {
                    if (!CancelNoticeButton.IsDisposed)
                        CancelNoticeButton.Dispose();

                    CancelNoticeButton = null;
                }

                #endregion

                #region Member Tab

                if (MemberTab != null)
                {
                    if (!MemberTab.IsDisposed)
                        MemberTab.Dispose();

                    MemberTab = null;
                }

                if (MemberRows != null)
                {
                    for (int i = 0; i < MemberRows.Length; i++)
                    {
                        if (MemberRows[i] != null)
                        {
                            if (!MemberRows[i].IsDisposed)
                                MemberRows[i].Dispose();

                            MemberRows[i] = null;
                        }
                    }
                    MemberRows = null;
                }

                if (MemberScrollBar != null)
                {
                    if (!MemberScrollBar.IsDisposed)
                        MemberScrollBar.Dispose();

                    MemberScrollBar = null;
                }

                #endregion

                #region Storage Tab

                if (StorageTab != null)
                {
                    if (!StorageTab.IsDisposed)
                        StorageTab.Dispose();

                    StorageTab = null;
                }

                if (ItemNameTextBox != null)
                {
                    if (!ItemNameTextBox.IsDisposed)
                        ItemNameTextBox.Dispose();

                    ItemNameTextBox = null;
                }

                if (ItemTypeComboBox != null)
                {
                    if (!ItemTypeComboBox.IsDisposed)
                        ItemTypeComboBox.Dispose();

                    ItemTypeComboBox = null;
                }

                if (StorageGrid != null)
                {
                    if (!StorageGrid.IsDisposed)
                        StorageGrid.Dispose();

                    StorageGrid = null;
                }

                if (ClearButton != null)
                {
                    if (!ClearButton.IsDisposed)
                        ClearButton.Dispose();

                    ClearButton = null;
                }

                if (StorageScrollBar != null)
                {
                    if (!StorageScrollBar.IsDisposed)
                        StorageScrollBar.Dispose();

                    StorageScrollBar = null;
                }

                if (GuildStorage != null)
                {
                    for (int i = 0; i < GuildStorage.Length; i++)
                    {
                        GuildStorage[i] = null;
                    }

                    StorageTab = null;
                }

                #endregion

                #region Manage Tab

                if (ManageTab != null)
                {
                    if (!ManageTab.IsDisposed)
                        ManageTab.Dispose();

                    ManageTab = null;
                }

                if (GuildFundLabel1 != null)
                {
                    if (!GuildFundLabel1.IsDisposed)
                        GuildFundLabel1.Dispose();

                    GuildFundLabel1 = null;
                }

                if (MemberLimitLabel1 != null)
                {
                    if (!MemberLimitLabel1.IsDisposed)
                        MemberLimitLabel1.Dispose();

                    MemberLimitLabel1 = null;
                }

                if (StorageSizeLabel != null)
                {
                    if (!StorageSizeLabel.IsDisposed)
                        StorageSizeLabel.Dispose();

                    StorageSizeLabel = null;
                }

                if (AddMemberTextBox != null)
                {
                    if (!AddMemberTextBox.IsDisposed)
                        AddMemberTextBox.Dispose();

                    AddMemberTextBox = null;
                }

                if (MemberTaxBox != null)
                {
                    if (!MemberTaxBox.IsDisposed)
                        MemberTaxBox.Dispose();

                    MemberTaxBox = null;
                }

                if (AddMemberButton != null)
                {
                    if (!AddMemberButton.IsDisposed)
                        AddMemberButton.Dispose();

                    AddMemberButton = null;
                }

                if (EditDefaultMemberButton != null)
                {
                    if (!EditDefaultMemberButton.IsDisposed)
                        EditDefaultMemberButton.Dispose();

                    EditDefaultMemberButton = null;
                }

                if (SetTaxButton != null)
                {
                    if (!SetTaxButton.IsDisposed)
                        SetTaxButton.Dispose();

                    SetTaxButton = null;
                }

                if (IncreaseMemberButton != null)
                {
                    if (!IncreaseMemberButton.IsDisposed)
                        IncreaseMemberButton.Dispose();

                    IncreaseMemberButton = null;
                }

                if (IncreaseStorageButton != null)
                {
                    if (!IncreaseStorageButton.IsDisposed)
                        IncreaseStorageButton.Dispose();

                    IncreaseStorageButton = null;
                }

                if (GuildTaxBox != null)
                {
                    if (!GuildTaxBox.IsDisposed)
                        GuildTaxBox.Dispose();

                    GuildTaxBox = null;
                }

                if (AddMemberPanel != null)
                {
                    if (!AddMemberPanel.IsDisposed)
                        AddMemberPanel.Dispose();

                    AddMemberPanel = null;
                }

                if (TreasuryPanel != null)
                {
                    if (!TreasuryPanel.IsDisposed)
                        TreasuryPanel.Dispose();

                    TreasuryPanel = null;
                }

                if (UpgradePanel != null)
                {
                    if (!UpgradePanel.IsDisposed)
                        UpgradePanel.Dispose();

                    UpgradePanel = null;
                }

                if (FlagPanel != null)
                {
                    if (!FlagPanel.IsDisposed)
                        FlagPanel.Dispose();

                    FlagPanel = null;
                }

                if (ApplicationPanel != null)
                {
                    if (!ApplicationPanel.IsDisposed)
                        ApplicationPanel.Dispose();

                    ApplicationPanel = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (AllowApplyCheckBox != null)
                {
                    if (!AllowApplyCheckBox.IsDisposed)
                        AllowApplyCheckBox.Dispose();

                    AllowApplyCheckBox = null;
                }

                if (GuildApplications != null)
                {
                    foreach (var application in GuildApplications)
                    {
                        application.Dispose();
                    }
                    GuildApplications.Clear();
                    GuildApplications = null;
                }

                #endregion

                #region War Tab

                if (WarTab != null)
                {
                    if (!WarTab.IsDisposed)
                        WarTab.Dispose();

                    WarTab = null;
                }

                #endregion

                _GuildInfo = null;
                GuildInfoChanged = null;
            }
        }
        #endregion
    }

    public sealed class GuildFundRankLine : DXControl
    {
        public DXLabel RankLabel, NameLabel, AmountLabel;

        public GuildFundRankLine()
        {
            Size = new Size(470, 20);
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            RankLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(5, 0),
                Size = new Size(100, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(RankLabel.Location.X + RankLabel.Size.Width + 1, 0),
                Size = new Size(30, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            AmountLabel = new DXLabel
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(NameLabel.Location.X + NameLabel.Size.Width + 1, 0),
                Size = new Size(30, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

        }
    }

    /// <summary>
    /// 入会申请行
    /// </summary>
    public sealed class GuildApplicationLine : DXControl
    {
        public DXLabel NameLabel, ClassLabel, LevelLabel, LastLoginLabel;
        public DXButton ViewButton, ApproveButton, RejectButton;
        public DXImageControl OnlineImage;
        public int PlayerIndex = -1;
        public GuildApplicationLine()
        {
            Size = new Size(470, 20);
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            OnlineImage = new DXImageControl  //在线图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3624,
                Location = new Point(8, 4),
                IsControl = false,
            };

            NameLabel = new DXLabel   //名字标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(OnlineImage.Location.X + OnlineImage.Size.Width + 1, 0),
                Size = new Size(100, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            ClassLabel = new DXLabel   //职业标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(NameLabel.Location.X + NameLabel.Size.Width + 1, 0),
                Size = new Size(30, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            LevelLabel = new DXLabel   //等级标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(ClassLabel.Location.X + ClassLabel.Size.Width + 1, 0),
                Size = new Size(30, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            LastLoginLabel = new DXLabel   //最后登录
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(LevelLabel.Location.X + LevelLabel.Size.Width + 1, 0),
                Size = new Size(70, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            ViewButton = new DXButton   //查看
            {
                Parent = this,
                Location = new Point(LastLoginLabel.Location.X + LastLoginLabel.Size.Width + 30, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "查看".Lang() },
                Enabled = false,
                Size = new Size(53, SmallButtonHeight)
            };
            ViewButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.Inspect { Index = PlayerIndex });
            };

            ApproveButton = new DXButton   //批准
            {
                Parent = this,
                Location = new Point(ViewButton.Location.X + ViewButton.Size.Width + 10, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "批准".Lang() },
                Enabled = false,
                Size = new Size(53, SmallButtonHeight)
            };
            ApproveButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildApplyChoice { Approved = true, PlayerIndex = PlayerIndex });
                this.Visible = false;
            };

            RejectButton = new DXButton   //拒绝
            {
                Parent = this,
                Location = new Point(ApproveButton.Location.X + ApproveButton.Size.Width + 10, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "拒绝".Lang() },
                Enabled = false,
                Size = new Size(53, SmallButtonHeight)
            };
            RejectButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildApplyChoice { Approved = false, PlayerIndex = PlayerIndex });
                this.Visible = false;
            };

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (ClassLabel != null)
                {
                    if (!ClassLabel.IsDisposed)
                        ClassLabel.Dispose();

                    ClassLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (LastLoginLabel != null)
                {
                    if (!LastLoginLabel.IsDisposed)
                        LastLoginLabel.Dispose();

                    LastLoginLabel = null;
                }

                if (ViewButton != null)
                {
                    if (!ViewButton.IsDisposed)
                        ViewButton.Dispose();

                    ViewButton = null;
                }

                if (ApproveButton != null)
                {
                    if (!ApproveButton.IsDisposed)
                        ApproveButton.Dispose();

                    ApproveButton = null;
                }

                if (RejectButton != null)
                {
                    if (!RejectButton.IsDisposed)
                        RejectButton.Dispose();

                    RejectButton = null;
                }

                if (OnlineImage != null)
                {
                    if (!OnlineImage.IsDisposed)
                        OnlineImage.Dispose();

                    OnlineImage = null;
                }

                PlayerIndex = -1;
            }
        }
    }



    /// <summary>
    /// 行会成员行
    /// </summary>
    public sealed class GuildMemberRow : DXControl
    {
        #region Properties

        public DXLabel NameLabel, RankLabel, TotalLabel, DailyLabel, OnlineLabel;

        #region IsHeader
        /// <summary>
        /// 显示标题
        /// </summary>
        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            NameLabel.Text = "名字".Lang();
            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            RankLabel.Text = "称号".Lang();
            RankLabel.ForeColour = Color.FromArgb(198, 166, 99);

            TotalLabel.Text = "总贡献值".Lang();
            TotalLabel.ForeColour = Color.FromArgb(198, 166, 99);

            DailyLabel.Text = "今日贡献值".Lang();
            DailyLabel.ForeColour = Color.FromArgb(198, 166, 99);

            OnlineLabel.Text = "在线状态".Lang();
            OnlineLabel.ForeColour = Color.FromArgb(198, 166, 99);

            DrawTexture = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region MemberInfo
        /// <summary>
        /// 行会成员信息
        /// </summary>
        public ClientGuildMemberInfo MemberInfo
        {
            get => _MemberInfo;
            set
            {
                ClientGuildMemberInfo oldValue = _MemberInfo;
                _MemberInfo = value;

                OnMemberInfoChanged(oldValue, value);
            }
        }
        private ClientGuildMemberInfo _MemberInfo;
        public event EventHandler<EventArgs> MemberInfoChanged;
        public void OnMemberInfoChanged(ClientGuildMemberInfo oValue, ClientGuildMemberInfo nValue)
        {
            Visible = MemberInfo != null;

            if (MemberInfo == null) return;

            NameLabel.Text = MemberInfo.Name;
            RankLabel.Text = MemberInfo.Rank == Globals.StarterGuildMember ? MemberInfo.Rank.Lang() : MemberInfo.Rank;
            TotalLabel.Text = MemberInfo.TotalContribution.ToString("#,##0");
            DailyLabel.Text = MemberInfo.DailyContribution.ToString("#,##0");

            OnlineLabel.Text = MemberInfo.Rank;

            MemberInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        /// <summary>
        /// 行会成员行
        /// </summary>
        public GuildMemberRow()
        {
            Size = new Size(488 + 40, 20);

            DrawTexture = true;
            BackColour = /*Selected ? Color.FromArgb(80, 80, 125) :*/ Color.FromArgb(80, 80, 125);

            NameLabel = new DXLabel   //名字标签
            {
                IsControl = false,
                Parent = this,
                Location = new Point(10, 2),
                ForeColour = Color.White,
            };

            RankLabel = new DXLabel   //职位标签
            {
                IsControl = false,
                Parent = this,
                Location = new Point(110, 2),
                ForeColour = Color.White,
            };

            TotalLabel = new DXLabel  //计数标签
            {
                IsControl = false,
                Parent = this,
                Location = new Point(210, 2),
                ForeColour = Color.White,
            };

            DailyLabel = new DXLabel   //日常标签
            {
                IsControl = false,
                Parent = this,
                Location = new Point(310, 2),
                ForeColour = Color.White,
            };

            OnlineLabel = new DXLabel  //在线状态标签
            {
                IsControl = false,
                Parent = this,
                Location = new Point(400, 2),
                ForeColour = Color.White,
            };
        }

        #region Methods
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (MemberInfo == null) return;

            if (MemberInfo.LastOnline == DateTime.MaxValue)
            {
                OnlineLabel.Text = "在线".Lang();
                OnlineLabel.ForeColour = Color.Green;
            }
            else
            {
                OnlineLabel.Text = (CEnvir.Now - MemberInfo.LastOnline).Lang(true);
                OnlineLabel.ForeColour = Color.White;
            }

        }
        /// <summary>
        /// 鼠标点击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (!Enabled || MemberInfo == null || (GameScene.Game.GuildBox.GuildInfo.Permission & GuildPermission.Leader) != GuildPermission.Leader) return;

                    GameScene.Game.GuildMemberBox.MemberNameLabel.Text = MemberInfo.Name;
                    GameScene.Game.GuildMemberBox.RankTextBox.TextBox.Text = MemberInfo.Rank;
                    GameScene.Game.GuildMemberBox.Permission = MemberInfo.Permission;
                    GameScene.Game.GuildMemberBox.MemberIndex = MemberInfo.Index;

                    GameScene.Game.GuildMemberBox.Visible = true;
                    GameScene.Game.GuildMemberBox.BringToFront();
                    break;
                //case MouseButtons.Right:
                //    if (MemberInfo == null || MemberInfo.ObjectID == MapObject.User.ObjectID) return;

                //    GameScene.Game.BigMapBox.Visible = true;
                //    GameScene.Game.BigMapBox.Opacity = 1F;

                //    if (!GameScene.Game.DataDictionary.TryGetValue(MemberInfo.ObjectID, out ClientObjectData data)) return;

                //    GameScene.Game.BigMapBox.SelectedInfo = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Index == data.MapIndex);
                //    break;
                case MouseButtons.Middle:
                    if (MemberInfo == null) return;

                    CEnvir.Enqueue(new C.GroupInvite { Name = MemberInfo.Name });
                    break;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (RankLabel != null)
                {
                    if (!RankLabel.IsDisposed)
                        RankLabel.Dispose();

                    RankLabel = null;
                }

                if (TotalLabel != null)
                {
                    if (!TotalLabel.IsDisposed)
                        TotalLabel.Dispose();

                    TotalLabel = null;
                }

                if (DailyLabel != null)
                {
                    if (!DailyLabel.IsDisposed)
                        DailyLabel.Dispose();

                    DailyLabel = null;
                }

                if (OnlineLabel != null)
                {
                    if (!OnlineLabel.IsDisposed)
                        OnlineLabel.Dispose();

                    OnlineLabel = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 行会联盟行
    /// </summary>
    public sealed class GuildAllianceRow : DXControl
    {
        #region Properties

        public DXLabel NameLabel, OnlineLabel;
        public DXButton EndAllianceButton;

        #region IsHeader
        /// <summary>
        /// 显示标题
        /// </summary>
        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            NameLabel.Text = "行会名".Lang();
            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            OnlineLabel.Text = "在线".Lang();
            OnlineLabel.ForeColour = Color.FromArgb(198, 166, 99);

            EndAllianceButton.Visible = !IsHeader;

            DrawTexture = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region AllianceInfo
        /// <summary>
        /// 联盟行会信息
        /// </summary>
        public ClientGuildAllianceInfo AllianceInfo
        {
            get => _AllianceInfo;
            set
            {
                ClientGuildAllianceInfo oldValue = _AllianceInfo;
                _AllianceInfo = value;

                OnAllianceInfoChanged(oldValue, value);
            }
        }
        private ClientGuildAllianceInfo _AllianceInfo;
        public event EventHandler<EventArgs> AllianceInfoChanged;
        public void OnAllianceInfoChanged(ClientGuildAllianceInfo oValue, ClientGuildAllianceInfo nValue)
        {
            Visible = AllianceInfo != null;

            if (AllianceInfo == null) return;

            NameLabel.Text = AllianceInfo.Name;
            OnlineLabel.Text = AllianceInfo.OnlineCount.ToString();


            AllianceInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        /// <summary>
        /// 行会联盟行
        /// </summary>
        public GuildAllianceRow()
        {
            Size = new Size(223, 20);

            DrawTexture = true;
            BackColour = /*Selected ? Color.FromArgb(80, 80, 125) :*/ Color.FromArgb(25, 20, 0);

            NameLabel = new DXLabel         //名字
            {
                IsControl = false,
                Parent = this,
                Location = new Point(10, 2),
                ForeColour = Color.White,
            };

            OnlineLabel = new DXLabel       //在线状态
            {
                IsControl = false,
                Parent = this,
                Location = new Point(110, 2),
                ForeColour = Color.White,
            };

            EndAllianceButton = new DXButton   //结束联盟按钮
            {
                Parent = this,
                Location = new Point(203, 1),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(20, SmallButtonHeight),
                Label = { Text = "X" },
            };
            EndAllianceButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox($"GuildAlliance.End".Lang(NameLabel.Text), "联盟".Lang(), DXMessageBoxButtons.YesNo);


                box.YesButton.MouseClick += (o1, e1) =>
                {
                    C.EndGuildAlliance p = new C.EndGuildAlliance
                    {
                        GuildName = NameLabel.Text,
                    };

                    CEnvir.Enqueue(p);
                };
            };
        }

        #region Methods
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (AllianceInfo == null) return;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (OnlineLabel != null)
                {
                    if (!OnlineLabel.IsDisposed)
                        OnlineLabel.Dispose();

                    OnlineLabel = null;
                }

                if (EndAllianceButton != null)
                {
                    if (!EndAllianceButton.IsDisposed)
                        EndAllianceButton.Dispose();

                    EndAllianceButton = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 提取赞助币申请行
    /// </summary>
    public sealed class GuildGameGoldWithdrawalLine : DXControl
    {
        public DXLabel NameLabel, GameGoldLabel;
        public DXButton ApproveButton, RejectButton;
        public int PlayerIndex = -1;
        public GuildGameGoldWithdrawalLine()
        {
            Size = new Size(185, 20);
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            NameLabel = new DXLabel   //名字标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(60, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            GameGoldLabel = new DXLabel   //赞助币标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(NameLabel.Location.X + NameLabel.Size.Width + 1, 0),
                Size = new Size(50, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            ApproveButton = new DXButton   //批准
            {
                Parent = this,
                Location = new Point(GameGoldLabel.Location.X + GameGoldLabel.Size.Width + 10, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "批准".Lang() },
                Enabled = false,
                Size = new Size(30, SmallButtonHeight)
            };
            ApproveButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuileWithdrawalApplyChoice { Approved = true, PlayerIndex = PlayerIndex });
                this.Visible = false;
            };

            RejectButton = new DXButton   //拒绝
            {
                Parent = this,
                Location = new Point(ApproveButton.Location.X + ApproveButton.Size.Width + 1, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "拒绝".Lang() },
                Enabled = false,
                Size = new Size(30, SmallButtonHeight)
            };
            RejectButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuileWithdrawalApplyChoice { Approved = false, PlayerIndex = PlayerIndex });
                this.Visible = false;
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (GameGoldLabel != null)
                {
                    if (!GameGoldLabel.IsDisposed)
                        GameGoldLabel.Dispose();

                    GameGoldLabel = null;
                }

                if (ApproveButton != null)
                {
                    if (!ApproveButton.IsDisposed)
                        ApproveButton.Dispose();

                    ApproveButton = null;
                }

                if (RejectButton != null)
                {
                    if (!RejectButton.IsDisposed)
                        RejectButton.Dispose();

                    RejectButton = null;
                }

                PlayerIndex = -1;
            }
        }
    }

    /// <summary>
    /// 行会管理成员功能
    /// </summary>
    public sealed class GuildMemberDialog : DXWindow
    {
        #region Properties

        public DXLabel MemberNameLabel;

        public DXTextBox RankTextBox;

        public DXCheckBox LeaderBox, EditNoticeBox, AddMemberBox, StorageBox, RepairBox, MerchantBox, MarketBox, StartWarBox, AllianceBox;

        public DXButton ConfirmButton, KickButton;

        #region MemberIndex
        /// <summary>
        /// 行会成员索引
        /// </summary>
        public int MemberIndex
        {
            get => _MemberIndex;
            set
            {
                if (_MemberIndex == value) return;

                int oldValue = _MemberIndex;
                _MemberIndex = value;

                OnMemberIndexChanged(oldValue, value);
            }
        }
        private int _MemberIndex;
        public event EventHandler<EventArgs> MemberIndexChanged;
        public void OnMemberIndexChanged(int oValue, int nValue)
        {
            MemberIndexChanged?.Invoke(this, EventArgs.Empty);

            KickButton.Visible = MemberIndex > 0;

            LeaderBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            EditNoticeBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            AddMemberBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            StorageBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            RepairBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            MerchantBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            MarketBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            StartWarBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
            AllianceBox.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;

            KickButton.Enabled = MemberIndex != GameScene.Game.GuildBox.GuildInfo.UserIndex;
        }

        #endregion

        #region Permission

        private bool Updating;
        /// <summary>
        /// 成员权限
        /// </summary>
        public GuildPermission Permission
        {
            get => _Permission;
            set
            {

                GuildPermission oldValue = _Permission;
                _Permission = value;

                OnPermissionChanged(oldValue, value);
            }
        }
        private GuildPermission _Permission;
        public event EventHandler<EventArgs> PermissionChanged;
        public void OnPermissionChanged(GuildPermission oValue, GuildPermission nValue)
        {
            if (Updating) return;

            Updating = true;
            LeaderBox.Checked = (Permission & GuildPermission.Leader) == GuildPermission.Leader;
            EditNoticeBox.Checked = (Permission & GuildPermission.EditNotice) == GuildPermission.EditNotice;
            AddMemberBox.Checked = (Permission & GuildPermission.AddMember) == GuildPermission.AddMember;
            StorageBox.Checked = (Permission & GuildPermission.Storage) == GuildPermission.Storage;
            RepairBox.Checked = (Permission & GuildPermission.FundsRepair) == GuildPermission.FundsRepair;
            MerchantBox.Checked = (Permission & GuildPermission.FundsMerchant) == GuildPermission.FundsMerchant;
            MarketBox.Checked = (Permission & GuildPermission.FundsMarket) == GuildPermission.FundsMarket;
            StartWarBox.Checked = (Permission & GuildPermission.StartWar) == GuildPermission.StartWar;
            AllianceBox.Checked = (Permission & GuildPermission.Alliance) == GuildPermission.Alliance;

            Updating = false;
            PermissionChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        public override WindowType Type => WindowType.GuildMemberBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 管理行会成员面板
        /// </summary>
        public GuildMemberDialog()
        {
            SetClientSize(new Size(200, 160));

            TitleLabel.Text = "管理成员".Lang();

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "成员".Lang(),
            };
            label.Location = new Point(ClientArea.X + 80 - label.Size.Width, ClientArea.Y);

            MemberNameLabel = new DXLabel   //成员名字标签
            {
                Parent = this,
                Location = new Point(ClientArea.X + 80, label.Location.Y),
                ForeColour = Color.White,
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "等级".Lang(),
            };
            label.Location = new Point(ClientArea.X + 80 - label.Size.Width, MemberNameLabel.Location.Y + 20);

            RankTextBox = new DXTextBox
            {
                Parent = this,
                Location = new Point(ClientArea.X + 80, label.Location.Y),
                Size = new Size(120, 20),
                MaxLength = Globals.MaxCharacterNameLength
            };

            LeaderBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "行会领袖".Lang() },
            };
            LeaderBox.CheckedChanged += (o, e) => UpdatePermission();
            LeaderBox.Location = new Point(ClientArea.X + 94 - LeaderBox.Size.Width, RankTextBox.Location.Y + 24);

            EditNoticeBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "编辑公告".Lang() },
            };
            EditNoticeBox.CheckedChanged += (o, e) => UpdatePermission();
            EditNoticeBox.Location = new Point(ClientArea.X + 94 - EditNoticeBox.Size.Width, LeaderBox.Location.Y + 20);

            AddMemberBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "添加成员".Lang() },
            };
            AddMemberBox.CheckedChanged += (o, e) => UpdatePermission();
            AddMemberBox.Location = new Point(ClientArea.X + 94 - AddMemberBox.Size.Width, EditNoticeBox.Location.Y + 20);

            StorageBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "使用仓库".Lang() },
            };
            StorageBox.CheckedChanged += (o, e) => UpdatePermission();
            StorageBox.Location = new Point(ClientArea.X + 94 - StorageBox.Size.Width, AddMemberBox.Location.Y + 20);

            StartWarBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "发起行会战".Lang() },
            };
            StartWarBox.CheckedChanged += (o, e) => UpdatePermission();
            StartWarBox.Location = new Point(ClientArea.X + 94 - StartWarBox.Size.Width, StorageBox.Location.Y + 20);

            RepairBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "修理资金".Lang() },
            };
            RepairBox.CheckedChanged += (o, e) => UpdatePermission();
            RepairBox.Location = new Point(ClientArea.X + 200 - RepairBox.Size.Width, EditNoticeBox.Location.Y);

            MerchantBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "商业资金".Lang() },
            };
            MerchantBox.CheckedChanged += (o, e) => UpdatePermission();
            MerchantBox.Location = new Point(ClientArea.X + 200 - MerchantBox.Size.Width, RepairBox.Location.Y + 20);

            MarketBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "市场资金".Lang() },
            };
            MarketBox.CheckedChanged += (o, e) => UpdatePermission();
            MarketBox.Location = new Point(ClientArea.X + 200 - MarketBox.Size.Width, MerchantBox.Location.Y + 20);

            AllianceBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "联盟".Lang() },
            };
            AllianceBox.CheckedChanged += (o, e) => UpdatePermission();
            AllianceBox.Location = new Point(ClientArea.X + 200 - AllianceBox.Size.Width, StorageBox.Location.Y + 20);


            ConfirmButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X + 120, StorageBox.Location.Y + 40),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(80, SmallButtonHeight),
                Label = { Text = "确认".Lang() },
            };

            ConfirmButton.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GuildEditMember { Index = MemberIndex, Permission = Permission, Rank = RankTextBox.TextBox.Text });

                Visible = false;
            };
            KickButton = new DXButton
            {
                Parent = this,
                Location = new Point(ClientArea.X, StorageBox.Location.Y + 40),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(40, SmallButtonHeight),
                Label = { Text = "驱逐".Lang() },
            };
            KickButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox($"GuildMember.Kick".Lang(MemberNameLabel.Text), "驱逐成员".Lang(), DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.GuildKickMember { Index = MemberIndex });
                    Visible = false;
                };
            };
        }

        #region Methods
        /// <summary>
        /// 更新权限
        /// </summary>
        private void UpdatePermission()
        {
            if (Updating) return;

            GuildPermission permission = GuildPermission.None;

            if (LeaderBox.Checked)
                permission |= GuildPermission.Leader;

            if (EditNoticeBox.Checked)
                permission |= GuildPermission.EditNotice;

            if (AddMemberBox.Checked)
                permission |= GuildPermission.AddMember;

            if (StorageBox.Checked)
                permission |= GuildPermission.Storage;

            if (RepairBox.Checked)
                permission |= GuildPermission.FundsRepair;

            if (MerchantBox.Checked)
                permission |= GuildPermission.FundsMerchant;

            if (MarketBox.Checked)
                permission |= GuildPermission.FundsMarket;

            if (StartWarBox.Checked)
                permission |= GuildPermission.StartWar;

            if (AllianceBox.Checked)
                permission |= GuildPermission.Alliance;

            Permission = permission;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _MemberIndex = 0;
                MemberIndexChanged = null;

                _Permission = 0;
                PermissionChanged = null;

                if (MemberNameLabel != null)
                {
                    if (!MemberNameLabel.IsDisposed)
                        MemberNameLabel.Dispose();

                    MemberNameLabel = null;
                }

                if (RankTextBox != null)
                {
                    if (!RankTextBox.IsDisposed)
                        RankTextBox.Dispose();

                    RankTextBox = null;
                }

                if (LeaderBox != null)
                {
                    if (!LeaderBox.IsDisposed)
                        LeaderBox.Dispose();

                    LeaderBox = null;
                }

                if (EditNoticeBox != null)
                {
                    if (!EditNoticeBox.IsDisposed)
                        EditNoticeBox.Dispose();

                    EditNoticeBox = null;
                }

                if (AddMemberBox != null)
                {
                    if (!AddMemberBox.IsDisposed)
                        AddMemberBox.Dispose();

                    AddMemberBox = null;
                }

                if (StorageBox != null)
                {
                    if (!StorageBox.IsDisposed)
                        StorageBox.Dispose();

                    StorageBox = null;
                }

                if (RepairBox != null)
                {
                    if (!RepairBox.IsDisposed)
                        RepairBox.Dispose();

                    RepairBox = null;
                }

                if (MerchantBox != null)
                {
                    if (!MerchantBox.IsDisposed)
                        MerchantBox.Dispose();

                    MerchantBox = null;
                }

                if (MarketBox != null)
                {
                    if (!MarketBox.IsDisposed)
                        MarketBox.Dispose();

                    MarketBox = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();

                    ConfirmButton = null;
                }

                if (KickButton != null)
                {
                    if (!KickButton.IsDisposed)
                        KickButton.Dispose();

                    KickButton = null;
                }

                if (StartWarBox != null)
                {
                    if (!StartWarBox.IsDisposed)
                        StartWarBox.Dispose();

                    StartWarBox = null;
                }

                if (AllianceBox != null)
                {
                    if (!AllianceBox.IsDisposed)
                        AllianceBox.Dispose();

                    AllianceBox = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 行会攻城战功能
    /// </summary>
    public sealed class GuildCastlePanel : DXControl
    {
        #region Castle
        /// <summary>
        /// 城堡信息
        /// </summary>
        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                CastleInfo oldValue = _Castle;
                _Castle = value;

                OnCastleChanged(oldValue, value);
            }
        }
        private CastleInfo _Castle;
        public event EventHandler<EventArgs> CastleChanged;
        public void OnCastleChanged(CastleInfo oValue, CastleInfo nValue)
        {
            CastleChanged?.Invoke(this, EventArgs.Empty);

            if (Castle == null) return;

            CastleNameLabel.Text = Castle.Lang(p => p.Name);
            ItemLabel.Text = Castle.Item?.Lang(p => p.ItemName) ?? "没有".Lang();
        }

        #endregion

        public DXLabel CastleNameLabel, CastleOwnerLabel, CastleDateLabel, ItemLabel;

        public DXButton RequestButton;

        /// <summary>
        /// 行会攻城战面板
        /// </summary>
        public GuildCastlePanel()
        {
            Size = new Size(250, 118);
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            CastleNameLabel = new DXLabel   //城堡名字标签
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(Size.Width, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            // CastleNameLabel.SizeChanged += (o, e) => CastleNameLabel.Location = new Point((Size.Width - CastleNameLabel.Size.Width) / 2, 0);

            DXLabel label = new DXLabel
            {
                Parent = this,
                Text = "当前拥有者".Lang(),
            };
            label.Location = new Point(80 - label.Size.Width, 25);

            CastleOwnerLabel = new DXLabel
            {
                Parent = this,
                Text = "无".Lang(),
                Location = new Point(80, 25),
                ForeColour = Color.White
            };

            label = new DXLabel
            {
                Parent = this,
                Text = "攻城时间表".Lang(),
            };
            label.Location = new Point(80 - label.Size.Width, 45);

            CastleDateLabel = new DXLabel
            {
                Parent = this,
                Text = "无".Lang(),
                Location = new Point(80, 45),
                ForeColour = Color.White
            };

            RequestButton = new DXButton
            {
                Parent = this,
                Location = new Point(80, 75),
                ButtonType = ButtonType.SmallButton,
                Size = new Size(100, SmallButtonHeight),
                Label = { Text = "提交".Lang() },
                Enabled = false,
            };
            RequestButton.MouseClick += (o, e) => CEnvir.Enqueue(new C.GuildRequestConquest { Index = Castle.Index });

            label = new DXLabel
            {
                Parent = this,
                Text = "花费".Lang(),
            };
            label.Location = new Point(80 - label.Size.Width, 95);

            ItemLabel = new DXLabel
            {
                Parent = this,
                Text = "无".Lang(),
                Location = new Point(80, 95),
                ForeColour = Color.White
            };
        }
        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            string owner = GameScene.Game.CastleOwners[Castle];

            CastleOwnerLabel.Text = string.IsNullOrEmpty(owner) ? "无".Lang() : owner;
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            //如果攻城时间等时间初始最小值 或 为空  那么就显示无
            if (Castle.WarDate.Date == DateTime.MinValue || Castle.WarDate.Date == null)
                CastleDateLabel.Text = "无".Lang();
            else if (Castle.WarDate <= CEnvir.Now)   //如果攻城时间小于等于当前时间   显示进行中
                CastleDateLabel.Text = "进行中".Lang();
            else
                CastleDateLabel.Text = (Castle.WarDate - CEnvir.Now).Lang(true);  //攻城时间显示为  攻城时间减当前时间
        }
    }

    public sealed class GuildFlagPanel : DXControl
    {
        public void UpdateView(ClientGuildInfo Info)
        {
            if (Info == null) return;
            indexBox.Value = Info.Flag + 1;
            flagImage.BaseIndex = Info.Flag * 100;
            colorCrl.BackColour = Info.FlagColor;
        }

        protected override void OnBeforeDraw()
        {
            base.OnBeforeDraw();
            flagImage.OverlayColor = colorCrl.BackColour;
        }

        DXAnimatedControl flagImage;
        DXNumberBox indexBox;
        DXColourControl colorCrl;
        public GuildFlagPanel()
        {
            Size = new Size(250, 169);
            Border = true;
            BorderColour = Color.FromArgb(198, 166, 99);

            flagImage = new DXAnimatedControl
            {
                LibraryFile = LibraryFile.Flag,
                BaseIndex = 0,
                FrameCount = 10,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(1),
                Parent = this,
                Loop = true,
                UseOffSet = true,
                Location = new Point(15, 150),
                Overlay = true,
            };

            var label = new DXLabel
            {
                Parent = this,
                Location = new Point(90, 10),
                Text = "行会旗帜".Lang(),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            };

            var typeLab = new DXLabel
            {
                Parent = this,
                Location = new Point(100, 85),
                Text = "图案".Lang(),
            };

            indexBox = new DXNumberBox
            {
                Change = 1,
                Parent = this,
                Location = new Point(150, 85),
                Size = new Size(80, 20),
                ValueTextBox = { Size = new Size(40, 18) },
                MaxValue = 12,
                MinValue = 1,
                UpButton = { Location = new Point(63, 1) },
            };
            indexBox.ValueTextBox.ValueChanged += (o, e) =>
            {
                flagImage.BaseIndex = ((int)indexBox.Value - 1) * 100;
            };

            var colorLab = new DXLabel
            {
                Parent = this,
                Location = new Point(100, 110),
                Text = "颜色".Lang(),
            };

            colorCrl = new DXColourControl
            {
                Parent = this,
                Location = new Point(170, 110),
            };

            var button = new DXButton
            {
                Parent = this,
                Label = { Text = "保存".Lang() },
                Location = new Point(Size.Width - 100, Size.Height - 35),
                Size = new Size(80, DefaultHeight),
            };

            button.MouseClick += (sender, e) =>
            {
                flagImage.OverlayColor = colorCrl.BackColour;
                CEnvir.Enqueue(new C.GuildFlag { Flag = (int)indexBox.Value - 1, Color = colorCrl.BackColour });
            };
        }

    }
    #endregion
}
