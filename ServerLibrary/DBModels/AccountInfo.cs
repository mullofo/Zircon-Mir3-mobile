using Library;
using MirDB;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.DBModels
{
    /// <summary>
    /// 账户信息
    /// </summary>
    [UserObject]
    public sealed class AccountInfo : DBObject
    {
        /// <summary>
        /// 注册的邮件账号
        /// </summary>
        public string EMailAddress
        {
            get { return _EMailAddress; }
            set
            {
                if (_EMailAddress == value) return;

                var oldValue = _EMailAddress;
                _EMailAddress = value;

                OnChanged(oldValue, value, "EMailAddress");
            }
        }
        private string _EMailAddress;
        /// <summary>
        /// 密码
        /// </summary>
        public byte[] Password
        {
            get { return _Password; }
            set
            {
                if (_Password == value) return;

                var oldValue = _Password;
                _Password = value;

                OnChanged(oldValue, value, "Password");
            }
        }
        private byte[] _Password;
        /// <summary>
        /// 注册姓名
        /// </summary>
        public string RealName
        {
            get { return _RealName; }
            set
            {
                if (_RealName == value) return;

                var oldValue = _RealName;
                _RealName = value;

                OnChanged(oldValue, value, "RealName");
            }
        }
        private string _RealName;
        /// <summary>
        /// 注册出生日期
        /// </summary>
        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set
            {
                if (_BirthDate == value) return;

                var oldValue = _BirthDate;
                _BirthDate = value;

                OnChanged(oldValue, value, "BirthDate");
            }
        }
        private DateTime _BirthDate;
        /// <summary>
        /// 注册填的问题
        /// </summary>
        public string Question
        {
            get { return _Question; }
            set
            {
                if (_Question == value) return;

                var oldValue = _Question;
                _Question = value;

                OnChanged(oldValue, value, "Question");
            }
        }
        private string _Question;
        /// <summary>
        /// 注册填的答案
        /// </summary>
        public string Answer
        {
            get { return _Answer; }
            set
            {
                if (_Answer == value) return;

                var oldValue = _Answer;
                _Answer = value;

                OnChanged(oldValue, value, "Answer");
            }
        }
        private string _Answer;

        /// <summary>
        /// 注册填的推荐人
        /// </summary>
        [Association("Referrals")]
        public AccountInfo Referral
        {
            get { return _Referral; }
            set
            {
                if (_Referral == value) return;

                var oldValue = _Referral;
                _Referral = value;

                OnChanged(oldValue, value, "Referral");
            }
        }
        private AccountInfo _Referral;
        /// <summary>
        /// 注册的IP
        /// </summary>
        public string CreationIP
        {
            get { return _CreationIP; }
            set
            {
                if (_CreationIP == value) return;

                var oldValue = _CreationIP;
                _CreationIP = value;

                OnChanged(oldValue, value, "CreationIP");
            }
        }
        private string _CreationIP;
        /// <summary>
        /// 注册的日期
        /// </summary>
        public DateTime CreationDate
        {
            get { return _CreationDate; }
            set
            {
                if (_CreationDate == value) return;

                var oldValue = _CreationDate;
                _CreationDate = value;

                OnChanged(oldValue, value, "CreationDate");
            }
        }
        private DateTime _CreationDate;
        /// <summary>
        /// 最后登录的IP
        /// </summary>
        public string LastIP
        {
            get { return _LastIP; }
            set
            {
                if (_LastIP == value) return;

                var oldValue = _LastIP;
                _LastIP = value;

                OnChanged(oldValue, value, "LastIP");
            }
        }
        private string _LastIP;
        /// <summary>
        /// 最后登录的时间
        /// </summary>
        public DateTime LastLogin
        {
            get { return _LastLogin; }
            set
            {
                if (_LastLogin == value) return;

                var oldValue = _LastLogin;
                _LastLogin = value;

                OnChanged(oldValue, value, "LastLogin");
            }
        }
        private DateTime _LastLogin;
        /// <summary>
        /// 激活KEY
        /// </summary>
        public string ActivationKey
        {
            get { return _ActivationKey; }
            set
            {
                if (_ActivationKey == value) return;

                var oldValue = _ActivationKey;
                _ActivationKey = value;

                OnChanged(oldValue, value, "ActivationKey");
            }
        }
        private string _ActivationKey;
        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActivationTime
        {
            get { return _ActivationTime; }
            set
            {
                if (_ActivationTime == value) return;

                var oldValue = _ActivationTime;
                _ActivationTime = value;

                OnChanged(oldValue, value, "ActivationTime");
            }
        }
        private DateTime _ActivationTime;
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Activated
        {
            get { return _Activated; }
            set
            {
                if (_Activated == value) return;

                var oldValue = _Activated;
                _Activated = value;

                OnChanged(oldValue, value, "Activated");
            }
        }
        private bool _Activated;
        /// <summary>
        /// 重置KEY
        /// </summary>
        public string ResetKey
        {
            get { return _ResetKey; }
            set
            {
                if (_ResetKey == value) return;

                var oldValue = _ResetKey;
                _ResetKey = value;

                OnChanged(oldValue, value, "ResetKey");
            }
        }
        private string _ResetKey;
        /// <summary>
        /// 重置时间
        /// </summary>
        public DateTime ResetTime
        {
            get { return _ResetTime; }
            set
            {
                if (_ResetTime == value) return;

                var oldValue = _ResetTime;
                _ResetTime = value;

                OnChanged(oldValue, value, "ResetTime");
            }
        }
        private DateTime _ResetTime;
        /// <summary>
        /// 密码修改时间
        /// </summary>
        public DateTime PasswordTime
        {
            get { return _PasswordTime; }
            set
            {
                if (_PasswordTime == value) return;

                var oldValue = _PasswordTime;
                _PasswordTime = value;

                OnChanged(oldValue, value, "PasswordTime");
            }
        }
        private DateTime _PasswordTime;
        /// <summary>
        /// 禁止聊天时间
        /// </summary>
        public DateTime ChatBanExpiry
        {
            get { return _ChatBanExpiry; }
            set
            {
                if (_ChatBanExpiry == value) return;

                var oldValue = _ChatBanExpiry;
                _ChatBanExpiry = value;

                OnChanged(oldValue, value, "ChatBanExpiry");
            }
        }
        private DateTime _ChatBanExpiry;
        /// <summary>
        /// 是否被禁止游戏
        /// </summary>
        public bool Banned
        {
            get { return _Banned; }
            set
            {
                if (_Banned == value) return;

                var oldValue = _Banned;
                _Banned = value;

                OnChanged(oldValue, value, "Banned");
            }
        }
        private bool _Banned;
        /// <summary>
        /// 禁止游戏到期时间
        /// </summary>
        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set
            {
                if (_ExpiryDate == value) return;

                var oldValue = _ExpiryDate;
                _ExpiryDate = value;

                OnChanged(oldValue, value, "ExpiryDate");
            }
        }
        private DateTime _ExpiryDate;
        /// <summary>
        /// 禁止游戏的理由
        /// </summary>
        public string BanReason
        {
            get { return _BanReason; }
            set
            {
                if (_BanReason == value) return;

                var oldValue = _BanReason;
                _BanReason = value;

                OnChanged(oldValue, value, "BanReason");
            }
        }
        private string _BanReason;
        /// <summary>
        /// 时间触发
        /// </summary>
        public DateTime FlashTime
        {
            get { return _FlashTime; }
            set
            {
                if (_FlashTime == value) return;

                var oldValue = _FlashTime;
                _FlashTime = value;

                OnChanged(oldValue, value, "FlashTime");
            }
        }
        private DateTime _FlashTime;
        /// <summary>
        /// 挂机时间
        /// </summary>
        public long AutoTime
        {
            get { return _AutoTime; }
            set
            {
                if (_AutoTime == value) return;

                var oldValue = _AutoTime;
                _AutoTime = value;

                OnChanged(oldValue, value, "AutoTime");
            }
        }
        private long _AutoTime;
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold
        {
            get { return _Gold; }
            set
            {
                if (_Gold == value) return;

                var oldValue = _Gold;
                _Gold = value;

                OnChanged(oldValue, value, "Gold");
            }
        }
        private long _Gold;
        /// <summary>
        /// 元宝
        /// </summary>
        public int GameGold
        {
            get { return _GameGold; }
            set
            {
                if (_GameGold == value) return;

                var oldValue = _GameGold;
                _GameGold = value;

                OnChanged(oldValue, value, "GameGold");
            }
        }
        private int _GameGold;
        /// <summary>
        /// 是否组队开关
        /// </summary>
        public bool AllowGroup
        {
            get { return _AllowGroup; }
            set
            {
                if (_AllowGroup == value) return;

                var oldValue = _AllowGroup;
                _AllowGroup = value;

                OnChanged(oldValue, value, "AllowGroup");
            }
        }
        private bool _AllowGroup;
        /// <summary>
        /// 是否开启好友开关
        /// </summary>
        public bool AllowFriend
        {
            get { return _AllowFriend; }
            set
            {
                if (_AllowFriend == value) return;

                var oldValue = _AllowFriend;
                _AllowFriend = value;

                OnChanged(oldValue, value, "AllowFriend");
            }
        }
        private bool _AllowFriend;
        /// <summary>
        /// 是否开启交易开关
        /// </summary>
        public bool AllowTrade
        {
            get { return _AllowTrade; }
            set
            {
                if (_AllowTrade == value) return;

                var oldValue = _AllowTrade;
                _AllowTrade = value;

                OnChanged(oldValue, value, "AllowTrade");
            }
        }
        private bool _AllowTrade;
        /// <summary>
        /// 是否允许回生术开关
        /// </summary>
        public bool AllowResurrectionOrder
        {
            get { return _AllowResurrectionOrder; }
            set
            {
                if (_AllowResurrectionOrder == value) return;

                var oldValue = _AllowResurrectionOrder;
                _AllowResurrectionOrder = value;

                OnChanged(oldValue, value, "AllowResurrectionOrder");
            }
        }
        private bool _AllowResurrectionOrder;
        /// <summary>
        /// 是否加入行会
        /// </summary>
        public bool AllowGuild
        {
            get { return _AllowGuild; }
            set
            {
                if (_AllowGuild == value) return;

                var oldValue = _AllowGuild;
                _AllowGuild = value;

                OnChanged(oldValue, value, "AllowGuild");
            }
        }
        private bool _AllowGuild;
        /// <summary>
        /// 是否允许天地合一传送
        /// </summary>
        public bool AllowGroupRecall
        {
            get { return _AllowGroupRecall; }
            set
            {
                if (_AllowGroupRecall == value) return;

                var oldValue = _AllowGroupRecall;
                _AllowGroupRecall = value;

                OnChanged(oldValue, value, "AllowGroupRecall");
            }
        }
        private bool _AllowGroupRecall;

        /// <summary>
        /// 行会成员
        /// </summary>
        [Association("Member")]
        public GuildMemberInfo GuildMember
        {
            get { return _GuildMember; }
            set
            {
                if (_GuildMember == value) return;

                var oldValue = _GuildMember;
                _GuildMember = value;

                OnChanged(oldValue, value, "GuildMember");
            }
        }
        private GuildMemberInfo _GuildMember;
        /// <summary>
        /// 全局时间
        /// </summary>
        public DateTime GlobalTime
        {
            get { return _GlobalTime; }
            set
            {
                if (_GlobalTime == value) return;

                var oldValue = _GlobalTime;
                _GlobalTime = value;

                OnChanged(oldValue, value, "GlobalTime");
            }
        }
        private DateTime _GlobalTime;
        /// <summary>
        /// 坐骑类型
        /// </summary>
        public HorseType Horse
        {
            get { return _Horse; }
            set
            {
                if (_Horse == value) return;

                var oldValue = _Horse;
                _Horse = value;

                OnChanged(oldValue, value, "Horse");
            }
        }
        private HorseType _Horse;
        /// <summary>
        /// 赏金
        /// </summary>
        public int HuntGold
        {
            get { return _HuntGold; }
            set
            {
                if (_HuntGold == value) return;

                var oldValue = _HuntGold;
                _HuntGold = value;

                OnChanged(oldValue, value, "HuntGold");
            }
        }
        private int _HuntGold;
        /// <summary>
        /// 是否GM
        /// </summary>
        public bool Admin
        {
            get { return _Admin; }
            set
            {
                if (_Admin == value) return;

                var oldValue = _Admin;
                _Admin = value;

                OnChanged(oldValue, value, "Admin");
            }
        }
        private bool _Admin;
        /// <summary>
        /// 仓库的容量大小
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
        /// 碎片包裹的容量大小
        /// </summary>
        public int PatchGridSize
        {
            get { return _PatchGridSize; }
            set
            {
                if (_PatchGridSize == value) return;

                var oldValue = _PatchGridSize;
                _PatchGridSize = value;

                OnChanged(oldValue, value, "PatchGridSize");
            }
        }
        private int _PatchGridSize;
        /// <summary>
        /// 最后登录的IP地址记录
        /// </summary>
        public string LastSum
        {
            get { return _LastSum; }
            set
            {
                if (_LastSum == value) return;

                var oldValue = _LastSum;
                _LastSum = value;

                OnChanged(oldValue, value, "LastSum");
            }
        }
        private string _LastSum;
        /// <summary>
        /// 金币限制
        /// </summary>
        public bool GoldBot
        {
            get { return _GoldBot; }
            set
            {
                if (_GoldBot == value) return;

                var oldValue = _GoldBot;
                _GoldBot = value;

                OnChanged(oldValue, value, "GoldBot");
            }
        }
        private bool _GoldBot;
        /// <summary>
        /// 道具限制
        /// </summary>
        public bool ItemBot
        {
            get { return _ItemBot; }
            set
            {
                if (_ItemBot == value) return;

                var oldValue = _ItemBot;
                _ItemBot = value;

                OnChanged(oldValue, value, "ItemBot");
            }
        }
        private bool _ItemBot;
        /// <summary>
        /// 行会加入退出时间
        /// </summary>
        public DateTime GuildTime
        {
            get { return _GuildTime; }
            set
            {
                if (_GuildTime == value) return;

                var oldValue = _GuildTime;
                _GuildTime = value;

                OnChanged(oldValue, value, "GuildTime");
            }
        }
        private DateTime _GuildTime;
        /// <summary>
        /// 是否观察者
        /// </summary>
        public bool Observer
        {
            get { return _Observer; }
            set
            {
                if (_Observer == value) return;

                var oldValue = _Observer;
                _Observer = value;

                OnChanged(oldValue, value, "Observer");
            }
        }
        private bool _Observer;
        /// <summary>
        /// 是否临时管理员（主密码登录）
        /// </summary>
        public bool TempAdmin;

        //数据库绑定列表<角色道具>
        [Association("Items")]
        public DBBindingList<UserItem> Items { get; set; }
        //数据库绑定列表<角色推荐>
        [Association("Referrals")]
        public DBBindingList<AccountInfo> Referrals { get; set; }
        //数据库绑定列表<角色>
        [Association("Characters")]
        public DBBindingList<CharacterInfo> Characters { get; set; }
        //数据库绑定列表<角色BUFF>
        [Association("Buffs")]
        public DBBindingList<BuffInfo> Buffs { get; set; }
        //数据库绑定列表<角色寄售>
        [Association("Auctions")]
        public DBBindingList<AuctionInfo> Auctions { get; set; }
        //数据库绑定列表<角色邮件>
        [Association("Mail")]
        public DBBindingList<MailInfo> Mail { get; set; }
        //数据库绑定列表<角色爆率>
        [Association("UserDrops")]
        public DBBindingList<UserDrop> UserDrops { get; set; }
        //数据库绑定列表<角色宠物>
        [Association("Companions")]
        public DBBindingList<UserCompanion> Companions { get; set; }
        //数据库绑定列表<角色宠物解锁>
        [Association("CompanionUnlocks")]
        //宠物解锁 {获取 设置}
        public DBBindingList<UserCompanionUnlock> CompanionUnlocks { get; set; }
        //数据库绑定列表<角色黑名单列表>
        [Association("BlockingList")]
        public DBBindingList<BlockInfo> BlockingList { get; set; }
        //数据库绑定列表<角色被列表禁止>
        [Association("BlockedByList")]
        public DBBindingList<BlockInfo> BlockedByList { get; set; }
        //数据库绑定列表<角色充值付款>
        [Association("Payments")]
        public DBBindingList<GameGoldPayment> Payments { get; set; }
        //数据库绑定列表<角色商店销售>
        [Association("StoreSales")]
        public DBBindingList<GameStoreSale> StoreSales { get; set; }
        //数据库绑定列表<角色财富记录>
        [Association("Fortunes")]
        public DBBindingList<UserFortuneInfo> Fortunes { get; set; }

        [Association("CharacterShop")]
        public DBBindingList<CharacterShop> CharacterShop { get; set; }

        [IgnoreProperty]
        public int CharacterCount => Characters.Count;

        /// <summary>
        /// 最后一个角色
        /// </summary>
        public CharacterInfo LastCharacter
        {
            get { return _LastCharacter; }
            set
            {
                if (_LastCharacter == value) return;

                var oldValue = _LastCharacter;
                _LastCharacter = value;

                OnChanged(oldValue, value, "LastCharacter");
            }
        }
        private CharacterInfo _LastCharacter;


        public string InviteCode
        {
            get { return _InviteCode; }
            set
            {
                if (_InviteCode == value) return;
                var oldValue = _InviteCode;
                _InviteCode = value;
                OnChanged(oldValue, value, "InviteCode");

            }
        }
        private string _InviteCode;

#if !ServerTool
        /// <summary>
        /// 密码错误计数
        /// </summary>
        public int WrongPasswordCount;

        /// <summary>
        /// 连接
        /// </summary>
        public SConnection Connection;

        /// <summary>
        /// 密匙文本
        /// </summary>
        public string Key;
#endif
        /// <summary>
        /// 创建
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            StorageSize = Globals.StorageSize;  //仓库初始大小

            //PatchGridSize = Globals.PatchGridSize;  //碎片包裹初始大小

#if ServerTool
            DBCollection<BuffInfo> BuffInfoList = SMain.Session.GetCollection<BuffInfo>();
            if (BuffInfoList != null)
            {
                BuffInfo buff = BuffInfoList.CreateNewObject();

                buff.Account = this;
                buff.Type = BuffType.HuntGold;
                buff.TickFrequency = TimeSpan.FromMinutes(Math.Max(1, Config.HuntGoldTime));
                buff.Stats = new Stats { [Stat.AvailableHuntGoldCap] = Config.HuntGoldCap };
                buff.RemainingTime = TimeSpan.MaxValue;
            };
#else
            if (SEnvir.BuffInfoList != null)
            {
                BuffInfo buff = SEnvir.BuffInfoList.CreateNewObject();

                buff.Account = this;
                buff.Type = BuffType.HuntGold;
                buff.TickFrequency = TimeSpan.FromMinutes(Math.Max(1, Config.HuntGoldTime));
                buff.Stats = new Stats { [Stat.AvailableHuntGoldCap] = Config.HuntGoldCap };
                buff.RemainingTime = TimeSpan.MaxValue;
            }
#endif
        }

        protected override internal void OnDeleted()
        {
            this.Banned = true;
            this.BanReason = "账户删除";
            base.OnDeleted();
        }

        /// <summary>
        /// 加载
        /// </summary>
        protected override internal void OnLoaded()
        {
            base.OnLoaded();

            if (StorageSize == 0)
                StorageSize = Globals.StorageSize;  //仓库
            var buff = Buffs.FirstOrDefault(x => x.Type == BuffType.HuntGold);
            if (buff == null) return;
            //赏金 打怪每分钟增加1个，进入安全区将叠加到最多15个
            buff.Stats = new Stats { [Stat.AvailableHuntGoldCap] = Config.HuntGoldCap };
            //增加时间设置，可以自定义时间
            buff.TickFrequency = TimeSpan.FromMinutes(Math.Max(1, Config.HuntGoldTime));
        }

#if !ServerTool
        /// <summary>
        /// 最高等级
        /// </summary>
        /// <returns></returns>
        public int HightestLevel()
        {
            int count = 0;

            foreach (CharacterInfo character in Characters)
                if (character.Level > count)
                    count = character.Level;

            return count;
        }
        /// <summary>
        /// 获取选择信息
        /// </summary>
        /// <returns></returns>
        public List<SelectInfo> GetSelectInfo()
        {
            List<SelectInfo> characters = new List<SelectInfo>();

            foreach (CharacterInfo character in Characters)
            {
                if (character.Deleted) continue;

                characters.Add(character.ToSelectInfo());
            }

            return characters;
        }
#endif
        /// <summary>
        /// 输出到字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return EMailAddress;
        }

    }
}
