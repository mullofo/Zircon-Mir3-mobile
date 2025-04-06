using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.DBModels
{
    /// <summary>
    /// 行会信息
    /// </summary>
    [UserObject]
    public sealed class GuildInfo : DBObject
    {
        /// <summary>
        /// 行会名称
        /// </summary>
        public string GuildName
        {
            get { return _GuildName; }
            set
            {
                if (_GuildName == value) return;

                var oldValue = _GuildName;
                _GuildName = value;

                OnChanged(oldValue, value, "GuildName");
            }
        }
        private string _GuildName;
        /// <summary>
        /// 行会旗帜
        /// </summary>
        public int GuildFlag
        {
            get { return _GuildFlag; }
            set
            {
                if (_GuildFlag == value) return;

                var oldValue = _GuildFlag;
                _GuildFlag = value;

                OnChanged(oldValue, value, "GuildFlag");
            }
        }
        private int _GuildFlag = 1;
        /// <summary>
        /// 行会旗帜的颜色
        /// </summary>
        public Color FlagColor
        {
            get { return _FlagColor; }
            set
            {
                if (_FlagColor == value) return;

                var oldValue = _FlagColor;
                _FlagColor = value;

                OnChanged(oldValue, value, "FlagColor");
            }
        }
        private Color _FlagColor = Color.Red;
        /// <summary>
        /// 行会成员列表
        /// </summary>
        public int MemberLimit
        {
            get { return _MemberLimit; }
            set
            {
                if (_MemberLimit == value) return;

                var oldValue = _MemberLimit;
                _MemberLimit = value;

                OnChanged(oldValue, value, "MemberLimit");
            }
        }
        private int _MemberLimit;
        /// <summary>
        /// 行会仓库容量
        /// </summary>
        public int StorageSize
        {
            get { return _StorageSize; }
            set
            {
                if (_StorageSize == value) return;

                var oldValue = _StorageSize;
                _StorageSize = value;

                OnChanged(oldValue, value, "StorageSize");
            }
        }
        private int _StorageSize;
        /// <summary>
        /// 行会资金
        /// </summary>
        public long GuildFunds
        {
            get { return _GuildFunds; }
            set
            {
                if (_GuildFunds == value) return;

                var oldValue = _GuildFunds;
                _GuildFunds = value;

                OnChanged(oldValue, value, "GuildFunds");
            }
        }
        private long _GuildFunds;
        /// <summary>
        /// 行会等级
        /// </summary>
        public int GuildLevel
        {
            get { return _GuildLevel; }
            set
            {
                if (_GuildLevel == value) return;

                var oldValue = _GuildLevel;
                _GuildLevel = value;

                OnChanged(oldValue, value, "GuildLevel");
            }
        }
        private int _GuildLevel;


        public long GuildExp
        {
            get { return _GuildExp; }
            set
            {
                if (_GuildExp == value) return;
                var oldValue = _GuildExp;
                _GuildExp = value;
                OnChanged(oldValue, value, "GuildExp");
            }
        }
        private long _GuildExp;
        /// <summary>
        /// 行会公告
        /// </summary>
        public string GuildNotice
        {
            get { return _GuildNotice; }
            set
            {
                if (_GuildNotice == value) return;

                var oldValue = _GuildNotice;
                _GuildNotice = value;

                OnChanged(oldValue, value, "GuildNotice");
            }
        }
        private string _GuildNotice;
        /// <summary>
        /// 行会税率
        /// </summary>
        public decimal GuildTax
        {
            get { return _GuildTax; }
            set
            {
                if (_GuildTax == value) return;

                var oldValue = _GuildTax;
                _GuildTax = value;

                OnChanged(oldValue, value, "GuildTax");
            }
        }
        private decimal _GuildTax;
        /// <summary>
        /// 行会总活跃度
        /// </summary>
        public long ActivCount
        {
            get { return _ActivCount; }
            set
            {
                if (_ActivCount == value) return;

                var oldValue = _ActivCount;
                _ActivCount = value;

                OnChanged(oldValue, value, "ActivCount");
            }
        }
        private long _ActivCount;

        /// <summary>
        /// 行会总活跃度
        /// </summary>
        public long DailyActivCount
        {
            get { return _DailyActivCount; }
            set
            {
                if (_DailyActivCount == value) return;

                var oldValue = _DailyActivCount;
                _DailyActivCount = value;

                OnChanged(oldValue, value, "DailyActivCount");
            }
        }
        private long _DailyActivCount;
        /// <summary>
        /// 行会总捐款额
        /// </summary>
        public long TotalContribution
        {
            get { return _TotalContribution; }
            set
            {
                if (_TotalContribution == value) return;

                var oldValue = _TotalContribution;
                _TotalContribution = value;

                OnChanged(oldValue, value, "TotalContribution");
            }
        }
        private long _TotalContribution;

        /// <summary>
        /// 行会捐献赞助币总数
        /// </summary>
        public long GameGoldTotal
        {
            get { return _GameGoldTotal; }
            set
            {
                if (_GameGoldTotal == value) return;
                var oldValue = _GameGoldTotal;
                _GameGoldTotal = value;
                OnChanged(oldValue, value, "GameGoldTotal");
            }

        }
        private long _GameGoldTotal;
        /// <summary>
        /// 行会每日捐款额
        /// </summary>
        public long DailyContribution
        {
            get { return _DailyContribution; }
            set
            {
                if (_DailyContribution == value) return;

                var oldValue = _DailyContribution;
                _DailyContribution = value;

                OnChanged(oldValue, value, "DailyContribution");
            }
        }
        private long _DailyContribution;
        /// <summary>
        /// 行会每日增长
        /// </summary>
        public long DailyGrowth
        {
            get { return _DailyGrowth; }
            set
            {
                if (_DailyGrowth == value) return;

                var oldValue = _DailyGrowth;
                _DailyGrowth = value;

                OnChanged(oldValue, value, "DailyGrowth");
            }
        }
        private long _DailyGrowth;
        /// <summary>
        /// 成员级别
        /// </summary>
        public string DefaultRank
        {
            get { return _DefaultRank; }
            set
            {
                if (_DefaultRank == value) return;

                var oldValue = _DefaultRank;
                _DefaultRank = value;

                OnChanged(oldValue, value, "DefaultRank");
            }
        }
        private string _DefaultRank;
        /// <summary>
        /// 行会默认权限
        /// </summary>
        public GuildPermission DefaultPermission
        {
            get { return _DefaultPermission; }
            set
            {
                if (_DefaultPermission == value) return;

                var oldValue = _DefaultPermission;
                _DefaultPermission = value;

                OnChanged(oldValue, value, "DefaultPermission");
            }
        }
        private GuildPermission _DefaultPermission;
        /// <summary>
        /// 新手行会
        /// </summary>
        public bool StarterGuild
        {
            get { return _StarterGuild; }
            set
            {
                if (_StarterGuild == value) return;

                var oldValue = _StarterGuild;
                _StarterGuild = value;

                OnChanged(oldValue, value, "StarterGuild");
            }
        }
        private bool _StarterGuild;

        /// <summary>
        /// 允许申请进入
        /// </summary>
        public bool AllowApply
        {
            get { return _AllowApply; }
            set
            {
                if (_AllowApply == value) return;

                var oldValue = _AllowApply;
                _AllowApply = value;

                OnChanged(oldValue, value, "AllowApply");
            }
        }
        private bool _AllowApply;

        /// <summary>
        /// 行会金库公告
        /// </summary>
        public string GuildVaultNotice
        {
            get { return _GuildVaultNotice; }
            set
            {
                if (_GuildVaultNotice == value) return;

                var oldValue = _GuildVaultNotice;
                _GuildVaultNotice = value;

                OnChanged(oldValue, value, "GuildVaultNotice");
            }
        }
        private string _GuildVaultNotice;

        //申请列表
        [IgnoreProperty]
        public List<int> PendingApplications { get; set; }

        //申请提取赞助币列表
        [IgnoreProperty]
        public Dictionary<int, long> PendingWithdrawal { get; set; }

        /// <summary>
        /// 行会战争
        /// </summary>
        [Association("Conquest", true)]
        public UserConquest Conquest
        {
            get { return _Conquest; }
            set
            {
                if (_Conquest == value) return;

                var oldValue = _Conquest;
                _Conquest = value;

                OnChanged(oldValue, value, "Conquest");
            }
        }
        private UserConquest _Conquest;
        /// <summary>
        /// 城堡信息
        /// </summary>
        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                var oldValue = _Castle;
                _Castle = value;

                OnChanged(oldValue, value, "Castle");
            }
        }
        private CastleInfo _Castle;
        /// <summary>
        /// 行会仓库容量
        /// </summary>
        public UserItem[] Storage = new UserItem[1000];
        /// <summary>
        /// 行会成员
        /// </summary>
        [Association("Members", true)]
        public DBBindingList<GuildMemberInfo> Members { get; set; }
        /// <summary>
        /// 行会道具
        /// </summary>
        [Association("Items", true)]
        public DBBindingList<UserItem> Items { get; set; }
        /// <summary>
        /// 捐献记录
        /// </summary>
        [Association("Donations", true)]
        public DBBindingList<GuildFundChangeInfo> Donations { get; set; }

#if !ServerTool
        /// <summary>
        /// 到客户端的行会信息
        /// </summary>
        /// <returns></returns>
        public ClientGuildInfo ToClientInfo()
        {
            return new ClientGuildInfo
            {
                GuildName = GuildName,
                Flag = GuildFlag,
                FlagColor = FlagColor,
                ActiveCount = ActivCount,
                DailyActiveCount = DailyActivCount,
                GuildExp = GuildExp,
                DailyGrowth = DailyGrowth,
                GuildFunds = GuildFunds,
                TotalContribution = TotalContribution,
                DailyContribution = DailyContribution,


                MemberLimit = MemberLimit,
                StorageLimit = StorageSize,

                Notice = GuildNotice,

                DefaultPermission = DefaultPermission,
                DefaultRank = DefaultRank,

                AllowApply = AllowApply,

                Tax = (int)(GuildTax * 100),

                Members = Members.Select(x => x.ToClientInfo()).ToList(),

                Storage = Items.Select(x => x.ToClientInfo()).ToList(),

                Alliances = new List<ClientGuildAllianceInfo>(),

                VaultNotice = GuildVaultNotice,

                MaxFund = Config.GuildMaxFund,

                FundRanks = GetDonationRanks(),

                FundChanges = Donations.Select(x => x.ToClientInfo()).OrderByDescending(y => y.OperationTime).Take(20).ToList(),

                GuildLevel = GuildLevel,
            };
        }

        /// <summary>
        /// 行会联盟
        /// </summary>
        [IgnoreProperty]
        public List<GuildAllianceInfo> Alliances
        {
            get { return SEnvir.GuildAllianceInfoList.Binding.Where(x => x.Guild1 == this || (x.Guild2 == this)).ToList(); }
        }
#endif
        /// <summary>
        /// 加载时
        /// </summary>
        protected override internal void OnLoaded()
        {
            base.OnLoaded();

#if ServerTool
            foreach (UserItem item in Items)
            {
                if (item.Slot < 0 || item.Slot >= Storage.Length)
                {
                    SEnvir.Log($"[无效道具] 行会: {GuildName}, 放入: {item.Slot}");
                    continue;
                }
            }
#else
            foreach (UserItem item in Items)
            {
                if (item.Slot < 0 || item.Slot >= Storage.Length)
                {
                    SEnvir.Log($"[无效道具] 行会: {GuildName}, 放入: {item.Slot}");
                    continue;
                }

                Storage[item.Slot] = item;
            }
            if (PendingApplications == null)
                PendingApplications = new List<int>();
            if (PendingWithdrawal == null)
                PendingWithdrawal = new Dictionary<int, long>();
#endif

        }

        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            DefaultRank = Globals.StarterGuildMember;
            DefaultPermission = GuildPermission.None;
        }
        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Castle = null;

            base.OnDeleted();
        }

#if !ServerTool
        /// <summary>
        /// 获取更新数据包
        /// </summary>
        /// <returns></returns>
        public S.GuildUpdate GetUpdatePacket()
        {
            return new S.GuildUpdate
            {
                DailyGrowth = DailyGrowth,
                GuildFunds = GuildFunds,
                TotalContribution = TotalContribution,
                DailyContribution = DailyContribution,
                GuildExp = GuildExp,
                ActiveCount = ActivCount,
                DailyActiveCount = DailyActivCount,
                MemberLimit = MemberLimit,
                StorageLimit = StorageSize,

                GuildLevel = GuildLevel,

                Tax = (int)(GuildTax * 100),

                Flag = GuildFlag,
                FlagColor = FlagColor,

                DefaultPermission = DefaultPermission,
                DefaultRank = DefaultRank,

                Members = new List<ClientGuildMemberInfo>(),

                ObserverPacket = false,

                FundRanks = GetDonationRanks(),
            };
        }

        public List<ClientGuildFundRankInfo> GetDonationRanks()
        {
            var top20 = Members.OrderByDescending(x => x.TotalContribution).Take(20).ToList();
            List<ClientGuildFundRankInfo> result = top20.Select((info, i) => new ClientGuildFundRankInfo
            {
                Name = info.Account.LastCharacter.CharacterName,
                Rank = i + 1,
                TotalAmount = info.TotalContribution
            }).ToList();

            return result;
        }
#endif

        /// <summary>
        /// 输出字符串行会名字
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GuildName;
        }
    }

    [UserObject]
    public sealed class GuildFundChangeInfo : DBObject
    {
        [Association("Donations")]
        public GuildInfo Guild
        {
            get { return _Guild; }
            set
            {
                if (_Guild == value) return;

                var oldValue = _Guild;
                _Guild = value;

                OnChanged(oldValue, value, "Guild");
            }
        }
        private GuildInfo _Guild;

        public string CharacterName
        {
            get { return _CharacterName; }
            set
            {
                if (_CharacterName == value) return;

                var oldValue = _CharacterName;
                _CharacterName = value;

                OnChanged(oldValue, value, "CharacterName");
            }
        }
        private string _CharacterName;

        public long Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private long _Amount;

        public DateTime OperationTime
        {
            get { return _operationTime; }
            set
            {
                if (_operationTime == value) return;

                var oldValue = _operationTime;
                _operationTime = value;

                OnChanged(oldValue, value, "DonationTime");
            }
        }
        private DateTime _operationTime;

        public ClientGuildFundChangeInfo ToClientInfo()
        {
            return new ClientGuildFundChangeInfo
            {
                GuildName = Guild.GuildName,
                Name = CharacterName,
                Amount = Amount,
                OperationTime = OperationTime
            };
        }
    }
}
