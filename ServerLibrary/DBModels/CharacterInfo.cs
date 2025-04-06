using Library;
using Library.SystemModels;
using MirDB;
using System;
using System.Collections.Generic;
using System.Drawing;
#if !ServerTool
using Server.Models;
#endif

namespace Server.DBModels
{
    [UserObject]
    /// <summary>
    /// 角色信息
    /// </summary>
    public sealed class CharacterInfo : DBObject
    {
        /// <summary>
        /// 账号信息
        /// </summary>
        [Association("Characters")]
        public AccountInfo Account
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;

        /// <summary>
        /// 角色名字
        /// </summary>
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
        /// <summary>
        /// 职业
        /// </summary>
        public MirClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private MirClass _Class;
        /// <summary>
        /// 性别
        /// </summary>
        public MirGender Gender
        {
            get { return _Gender; }
            set
            {
                if (_Gender == value) return;

                var oldValue = _Gender;
                _Gender = value;

                OnChanged(oldValue, value, "Gender");
            }
        }
        private MirGender _Gender;
        /// <summary>
        /// 等级
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;
        /// <summary>
        /// 碎片包裹格子可使用大小
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
        /// 记忆传送计数
        /// </summary>
        public int FixedPointTCount
        {
            get { return _FixedPointTCount; }
            set
            {
                if (_FixedPointTCount == value) return;

                var oldValue = _FixedPointTCount;
                _FixedPointTCount = value;

                OnChanged(oldValue, value, "FixedPointTCount");
            }
        }
        private int _FixedPointTCount;
        /// <summary>
        /// 头发类型
        /// </summary>
        public int HairType
        {
            get { return _HairType; }
            set
            {
                if (_HairType == value) return;

                var oldValue = _HairType;
                _HairType = value;

                OnChanged(oldValue, value, "HairType");
            }
        }
        private int _HairType;
        /// <summary>
        /// 头发颜色
        /// </summary>
        public Color HairColour
        {
            get { return _HairColour; }
            set
            {
                if (_HairColour == value) return;

                var oldValue = _HairColour;
                _HairColour = value;

                OnChanged(oldValue, value, "HairColour");
            }
        }
        private Color _HairColour;
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour
        {
            get { return _ArmourColour; }
            set
            {
                if (_ArmourColour == value) return;

                var oldValue = _ArmourColour;
                _ArmourColour = value;

                OnChanged(oldValue, value, "ArmourColour");
            }
        }
        private Color _ArmourColour;
        /// <summary>
        /// 最后登录时间
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
        /// 删除角色记录
        /// </summary>
        public bool Deleted
        {
            get { return _Deleted; }
            set
            {
                if (_Deleted == value) return;

                var oldValue = _Deleted;
                _Deleted = value;

                OnChanged(oldValue, value, "Deleted");
            }
        }
        private bool _Deleted;
        /// <summary>
        /// 角色禁止上架寄售
        /// </summary>
        public bool ProhibitListing
        {
            get { return _ProhibitListing; }
            set
            {
                if (_ProhibitListing == value) return;

                var oldValue = _ProhibitListing;
                _ProhibitListing = value;

                OnChanged(oldValue, value, "ProhibitListing");
            }
        }
        private bool _ProhibitListing;
        /// <summary>
        /// 创建角色的日期
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
        /// 创建角色的IP
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
        /// 角色当前位置
        /// </summary>
        public Point CurrentLocation
        {
            get { return _CurrentLocation; }
            set
            {
                if (_CurrentLocation == value) return;

                var oldValue = _CurrentLocation;
                _CurrentLocation = value;

                OnChanged(oldValue, value, "CurrentLocation");
            }
        }
        private Point _CurrentLocation;
        /// <summary>
        /// 角色当前地图
        /// </summary>
        public MapInfo CurrentMap
        {
            get { return _CurrentMap; }
            set
            {
                if (_CurrentMap == value) return;

                var oldValue = _CurrentMap;
                _CurrentMap = value;

                OnChanged(oldValue, value, "CurrentMap");
            }
        }
        private MapInfo _CurrentMap;

        public CharacterState CharacterState
        {
            get { return _CharacterState; }
            set
            {
                if (_CharacterState == value) return;
                var oldValue = _CharacterState;
                _CharacterState = value;
                OnChanged(oldValue, value, "CharacterState");
            }
        }

        private CharacterState _CharacterState;

        /// <summary>
        /// 角色当前坐标
        /// </summary>
        public MirDirection Direction
        {
            get { return _Direction; }
            set
            {
                if (_Direction == value) return;

                var oldValue = _Direction;
                _Direction = value;

                OnChanged(oldValue, value, "Direction");
            }
        }
        private MirDirection _Direction;
        /// <summary>
        /// 绑定回城信息
        /// </summary>
        public SafeZoneInfo BindPoint
        {
            get { return _BindPoint; }
            set
            {
                if (_BindPoint == value) return;

                var oldValue = _BindPoint;
                _BindPoint = value;

                OnChanged(oldValue, value, "BindPoint");
            }
        }
        private SafeZoneInfo _BindPoint;
        /// <summary>
        /// 角色当前HP
        /// </summary>
        public int CurrentHP
        {
            get { return _CurrentHP; }
            set
            {
                if (_CurrentHP == value) return;

                var oldValue = _CurrentHP;
                _CurrentHP = value;

                OnChanged(oldValue, value, "CurrentHP");
            }
        }
        private int _CurrentHP;
        /// <summary>
        /// 角色当前MP
        /// </summary>
        public int CurrentMP
        {
            get { return _CurrentMP; }
            set
            {
                if (_CurrentMP == value) return;

                var oldValue = _CurrentMP;
                _CurrentMP = value;

                OnChanged(oldValue, value, "CurrentMP");
            }
        }
        private int _CurrentMP;

        public long DayExpAdd
        {
            get { return _DayExpAdd; }
            set
            {
                if (_DayExpAdd == value) return;

                var oldValue = _DayExpAdd;
                _DayExpAdd = value;

                OnChanged(oldValue, value, "DayExpAdd");

            }
        }


        private long _DayExpAdd;

        public long DayDonations
        {
            get { return _DayDonations; }
            set
            {
                if (_DayDonations == value) return;

                var oldValue = _DayDonations;
                _DayDonations = value;

                OnChanged(oldValue, value, "DayDonations");

            }
        }
        private long _DayDonations;
        public long DayActiveCount
        {
            get { return _DayActiveCount; }
            set
            {
                if (_DayActiveCount == value) return;

                var oldValue = _DayActiveCount;
                _DayActiveCount = value;

                OnChanged(oldValue, value, "DayActiveCount");

            }
        }
        private long _DayActiveCount;

        public long TotalActiveCount
        {
            get { return _TotalActiveCount; }
            set
            {
                if (_TotalActiveCount == value) return;

                var oldValue = _TotalActiveCount;
                _TotalActiveCount = value;

                OnChanged(oldValue, value, "TotalActiveCount");

            }
        }
        private long _TotalActiveCount;
        /// <summary>
        /// 角色经验值
        /// </summary>
        public decimal Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private decimal _Experience;
        /// <summary>
        /// 角色声望值
        /// </summary>
        public int Prestige
        {
            get { return _Prestige; }
            set
            {
                if (_Prestige == value) return;

                var oldValue = _Prestige;
                _Prestige = value;

                OnChanged(oldValue, value, "Prestige");
            }
        }
        private int _Prestige;
        /// <summary>
        /// 角色贡献值
        /// </summary>
        public int Contribute
        {
            get { return _Contribute; }
            set
            {
                if (_Contribute == value) return;

                var oldValue = _Contribute;
                _Contribute = value;

                OnChanged(oldValue, value, "Contribute");
            }
        }
        private int _Contribute;
        /// <summary>
        /// 刺杀开关
        /// </summary>
        public bool CanThrusting
        {
            get { return _canThrusting; }
            set
            {
                if (_canThrusting == value) return;

                var oldValue = _canThrusting;
                _canThrusting = value;

                OnChanged(oldValue, value, "CanThrusting");
            }
        }
        private bool _canThrusting;
        /// <summary>
        /// 半月开关
        /// </summary>
        public bool CanHalfMoon
        {
            get { return _CanHalfMoon; }
            set
            {
                if (_CanHalfMoon == value) return;

                var oldValue = _CanHalfMoon;
                _CanHalfMoon = value;

                OnChanged(oldValue, value, "CanHalfMoon");
            }
        }
        private bool _CanHalfMoon;
        /// <summary>
        /// 十方斩开关
        /// </summary>
        public bool CanDestructiveSurge
        {
            get { return _canDestructiveSurge; }
            set
            {
                if (_canDestructiveSurge == value) return;

                var oldValue = _canDestructiveSurge;
                _canDestructiveSurge = value;

                OnChanged(oldValue, value, "CanDestructiveSurge");
            }
        }
        private bool _canDestructiveSurge;
        /// <summary>
        /// 新月炎龙爆开关
        /// </summary>
        public bool CanFlameSplash
        {
            get { return _CanFlameSplash; }
            set
            {
                if (_CanFlameSplash == value) return;

                var oldValue = _CanFlameSplash;
                _CanFlameSplash = value;

                OnChanged(oldValue, value, "CanFlameSplash");
            }
        }
        private bool _CanFlameSplash;
        /// <summary>
        /// 最新属性状态
        /// </summary>
        public Stats LastStats
        {
            get { return _LastStats; }
            set
            {
                if (_LastStats == value) return;

                var oldValue = _LastStats;
                _LastStats = value;

                OnChanged(oldValue, value, "LastStats");
            }
        }
        private Stats _LastStats;
        /// <summary>
        /// 额外属性加点
        /// </summary>
        public Stats HermitStats
        {
            get { return _HermitStats; }
            set
            {
                if (_HermitStats == value) return;

                var oldValue = _HermitStats;
                _HermitStats = value;

                OnChanged(oldValue, value, "HermitStats");
            }
        }
        private Stats _HermitStats;
        /// <summary>
        /// 已经使用的额外属性点
        /// </summary>
        public int SpentPoints
        {
            get { return _SpentPoints; }
            set
            {
                if (_SpentPoints == value) return;

                var oldValue = _SpentPoints;
                _SpentPoints = value;

                OnChanged(oldValue, value, "SpentPoints");
            }
        }
        private int _SpentPoints;
        /// <summary>
        /// 攻击模式切换
        /// </summary>
        public AttackMode AttackMode
        {
            get { return _AttackMode; }
            set
            {
                if (_AttackMode == value) return;

                var oldValue = _AttackMode;
                _AttackMode = value;

                OnChanged(oldValue, value, "AttackMode");
            }
        }
        private AttackMode _AttackMode;
        /// <summary>
        /// 宠物模式切换
        /// </summary>
        public PetMode PetMode
        {
            get { return _PetMode; }
            set
            {
                if (_PetMode == value) return;

                var oldValue = _PetMode;
                _PetMode = value;

                OnChanged(oldValue, value, "PetMode");
            }
        }
        private PetMode _PetMode;
        /// <summary>
        /// 观察者开关
        /// </summary>
        public bool Observable
        {
            get { return _Observable; }
            set
            {
                if (_Observable == value) return;

                var oldValue = _Observable;
                _Observable = value;

                OnChanged(oldValue, value, "Observable");
            }
        }
        private bool _Observable;
        /// <summary>
        /// 物品复活恢复时间
        /// </summary>
        public DateTime ItemReviveTime
        {
            get { return _ItemReviveTime; }
            set
            {
                if (_ItemReviveTime == value) return;

                var oldValue = _ItemReviveTime;
                _ItemReviveTime = value;

                OnChanged(oldValue, value, "ItemReviveTime");
            }
        }
        private DateTime _ItemReviveTime;
        /// <summary>
        /// 重生丸恢复时间
        /// </summary>
        public DateTime ReincarnationPillTime
        {
            get { return _ReincarnationPillTime; }
            set
            {
                if (_ReincarnationPillTime == value) return;

                var oldValue = _ReincarnationPillTime;
                _ReincarnationPillTime = value;

                OnChanged(oldValue, value, "ReincarnationPillTime");
            }
        }
        private DateTime _ReincarnationPillTime;
        /// <summary>
        /// 结婚戒指传送时间
        /// </summary>
        public DateTime MarriageTeleportTime
        {
            get { return _MarriageTeleportTime; }
            set
            {
                if (_MarriageTeleportTime == value) return;

                var oldValue = _MarriageTeleportTime;
                _MarriageTeleportTime = value;

                OnChanged(oldValue, value, "MarriageTeleportTime");
            }
        }
        private DateTime _MarriageTeleportTime;
        /// <summary>
        /// 行会传送时间
        /// </summary>
        public DateTime GroupRecallTime
        {
            get { return _GroupRecallTime; }
            set
            {
                if (_GroupRecallTime == value) return;

                var oldValue = _GroupRecallTime;
                _GroupRecallTime = value;

                OnChanged(oldValue, value, "GroupRecallTime");
            }
        }
        private DateTime _GroupRecallTime;
        /// <summary>
        /// 隐藏头盔
        /// </summary>
        public bool HideHelmet
        {
            get { return _HideHelmet; }
            set
            {
                if (_HideHelmet == value) return;

                var oldValue = _HideHelmet;
                _HideHelmet = value;

                OnChanged(oldValue, value, "HideHelmet");
            }
        }
        private bool _HideHelmet;
        /// <summary>
        /// 隐藏盾牌
        /// </summary>
        public bool HideShield
        {
            get { return _HideShield; }
            set
            {
                if (_HideShield == value) return;

                var oldValue = _HideShield;
                _HideShield = value;

                OnChanged(oldValue, value, "HideShield");
            }
        }
        private bool _HideShield;
        /// <summary>
        /// 隐藏时装
        /// </summary>
        public bool HideFashion
        {
            get { return _HideFashion; }
            set
            {
                if (_HideFashion == value) return;

                var oldValue = _HideFashion;
                _HideFashion = value;

                OnChanged(oldValue, value, "HideFashion");
            }
        }
        private bool _HideFashion;
        /// <summary>
        /// 死亡爆出物品开关
        /// </summary>
        public bool CanDeathDrop
        {
            get { return _CanDeathDrop; }
            set
            {
                if (_CanDeathDrop == value) return;

                var oldValue = _CanDeathDrop;
                _CanDeathDrop = value;

                OnChanged(oldValue, value, "CanDeathDrop");
            }
        }
        private bool _CanDeathDrop;
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
        /// 转生
        /// </summary>
        public int Rebirth
        {
            get => _Rebirth;
            set
            {
                if (_Rebirth == value) return;

                int oldValue = _Rebirth;
                _Rebirth = value;

                OnChanged(oldValue, value, "Rebirth");
            }
        }
        private int _Rebirth;
        /// <summary>
        /// 制作等级
        /// </summary>
        public int CraftLevel
        {
            get => _CraftLevel;
            set
            {
                if (_CraftLevel == value) return;

                int oldValue = _CraftLevel;
                _CraftLevel = value;

                OnChanged(oldValue, value, "CraftLevel");
            }
        }
        private int _CraftLevel;
        /// <summary>
        /// 制作经验
        /// </summary>
        public int CraftExp
        {
            get => _CraftExp;
            set
            {
                if (_CraftExp == value) return;

                int oldValue = _CraftExp;
                _CraftExp = value;

                OnChanged(oldValue, value, "CraftExp");
            }
        }
        private int _CraftExp;
        /// <summary>
        /// 可重复任务完成的次数 每日0点清零
        /// </summary>
        public int RepeatableQuestCount
        {
            get => _repeatableQuestCount;
            set
            {
                if (_repeatableQuestCount == value) return;

                int oldValue = _repeatableQuestCount;
                _repeatableQuestCount = value;

                OnChanged(oldValue, value, "RepeatableQuestCount");
            }
        }
        private int _repeatableQuestCount;
        /// <summary>
        /// 每日任务完成的次数 每日0点清零
        /// </summary>
        public int DailyQuestCount
        {
            get => _DailyQuestCount;
            set
            {
                if (_DailyQuestCount == value) return;

                int oldValue = _DailyQuestCount;
                _DailyQuestCount = value;

                OnChanged(oldValue, value, "DailyQuestCount");
            }
        }
        private int _DailyQuestCount;
        /// <summary>
        /// 制作物品花费时间
        /// </summary>
        public DateTime CraftFinishTime
        {
            get { return _CraftFinishTime; }
            set
            {
                if (_CraftFinishTime == value) return;

                var oldValue = _CraftFinishTime;
                _CraftFinishTime = value;

                OnChanged(oldValue, value, "CraftFinishTime");
            }
        }
        private DateTime _CraftFinishTime;
        /// <summary>
        /// 工艺信息
        /// </summary>
        public CraftItemInfo BookmarkedCraftItemInfo
        {
            get { return _BookmarkedCraftItemInfo; }
            set
            {
                if (_BookmarkedCraftItemInfo == value) return;

                var oldValue = _BookmarkedCraftItemInfo;
                _BookmarkedCraftItemInfo = value;

                OnChanged(oldValue, value, "BookmarkedCraftItemInfo");
            }
        }
        private CraftItemInfo _BookmarkedCraftItemInfo;
        /// <summary>
        /// 制作道具
        /// </summary>
        public CraftItemInfo CraftingItem
        {
            get { return _CraftingItem; }
            set
            {
                if (_CraftingItem == value) return;

                var oldValue = _CraftingItem;
                _CraftingItem = value;

                OnChanged(oldValue, value, "CraftingItem");
            }
        }
        private CraftItemInfo _CraftingItem;
        /// <summary>
        /// 成就名称
        /// </summary>
        public string AchievementTitle
        {
            get { return _AchievementTitle; }
            set
            {
                if (_AchievementTitle == value) return;

                var oldValue = _AchievementTitle;
                _AchievementTitle = value;

                OnChanged(oldValue, value, "AchievementTitle");
            }
        }
        private string _AchievementTitle;
        /// <summary>
        /// 下一个死亡掉落改变
        /// </summary>
        public DateTime NextDeathDropChange
        {
            get { return _NextDeathDropChange; }
            set
            {
                if (_NextDeathDropChange == value) return;

                var oldValue = _NextDeathDropChange;
                _NextDeathDropChange = value;

                OnChanged(oldValue, value, "NextDeathDropChange");
            }
        }
        private DateTime _NextDeathDropChange;

        /// <summary>
        /// 每日已使用的免费泰山投币次数
        /// </summary>
        public int DailyFreeTossUsed
        {
            get => _DailyFreeTossUsed;
            set
            {
                if (_DailyFreeTossUsed == value) return;

                int oldValue = _DailyFreeTossUsed;
                _DailyFreeTossUsed = value;

                OnChanged(oldValue, value, "DailyFreeTossUsed");
            }
        }
        private int _DailyFreeTossUsed;

        /// <summary>
        /// 当前彩池币的数量
        /// </summary>
        public decimal RewardPoolCoin
        {
            get { return _RewardPoolCoin; }
            set
            {
                if (_RewardPoolCoin == value) return;

                var oldValue = _RewardPoolCoin;
                _RewardPoolCoin = Math.Round(value, 5);

                OnChanged(oldValue, _RewardPoolCoin, "RewardPoolCoin");
            }
        }
        private decimal _RewardPoolCoin;

        /// <summary>
        /// 累计获得的奖池币
        /// </summary>
        public decimal TotalRewardPoolCoinEarned
        {
            get => _TotalRewardPoolCoinEarned;
            set
            {
                if (_TotalRewardPoolCoinEarned == value) return;

                var oldValue = _TotalRewardPoolCoinEarned;
                _TotalRewardPoolCoinEarned = Math.Round(value, 5);

                OnChanged(oldValue, _TotalRewardPoolCoinEarned, "TotalRewardPoolCoinEarned");
            }
        }
        private decimal _TotalRewardPoolCoinEarned;

        /// <summary>
        /// 累计提现的奖池币
        /// </summary>
        public decimal TotalRewardPoolCoinCashedOut
        {
            get => _TotalRewardPoolCoinCashedOut;
            set
            {
                if (_TotalRewardPoolCoinCashedOut == value) return;

                var oldValue = _TotalRewardPoolCoinCashedOut;
                _TotalRewardPoolCoinCashedOut = Math.Round(value, 5);

                OnChanged(oldValue, _TotalRewardPoolCoinCashedOut, "TotalRewardPoolCoinCashedOut");
            }
        }
        private decimal _TotalRewardPoolCoinCashedOut;

        /// <summary>
        /// 今日在线累计时长（分钟）
        /// 理论上不超过1440
        /// </summary>
        public int TodayOnlineMinutes
        {
            get { return _TodayOnlineMinutes; }
            set
            {
                if (_TodayOnlineMinutes == value) return;

                int oldValue = _TodayOnlineMinutes;
                _TodayOnlineMinutes = value;

                OnChanged(oldValue, value, "TodayOnlineMinutes");
            }
        }

        private int _TodayOnlineMinutes;

        /// <summary>
        /// 角色宠物
        /// </summary>
        [Association("Companion")]
        public UserCompanion Companion
        {
            get { return _Companion; }
            set
            {
                if (_Companion == value) return;

                var oldValue = _Companion;
                _Companion = value;

                OnChanged(oldValue, value, "Companion");
            }
        }
        private UserCompanion _Companion;

        /// <summary>
        /// 角色道具
        /// </summary>
        [Association("Items", true)]
        public DBBindingList<UserItem> Items { get; set; }


        [Association("GoldMarketInfos", true)]
        public DBBindingList<GoldMarketInfo> GoldMarketInfos { get; set; }

        [Association("NewAuctions", true)]
        public DBBindingList<NewAutionInfo> NewAutionInfos { get; set; }
        /// <summary>
        /// 角色药品快捷栏
        /// </summary>
        [Association("BeltLinks", true)]
        public DBBindingList<CharacterBeltLink> BeltLinks { get; set; }
        /// <summary>
        /// 角色自动喝药栏
        /// </summary>
        [Association("AutoPotionLinks", true)]
        public DBBindingList<AutoPotionLink> AutoPotionLinks { get; set; }
        /// <summary>
        /// 角色自动挂机设置
        /// </summary>
        [Association("AutoFightLinks", true)]
        public DBBindingList<AutoFightConfig> AutoFightLinks { get; set; }
        /// <summary>
        /// 角色魔法技能
        /// </summary>
        [Association("Magics", true)]
        public DBBindingList<UserMagic> Magics { get; set; }
        /// <summary>
        /// 角色BUFF信息
        /// </summary>
        [Association("Buffs", true)]
        public DBBindingList<BuffInfo> Buffs { get; set; }
        /// <summary>
        /// 角色精炼信息
        /// </summary>
        [Association("Refines", true)]
        public DBBindingList<RefineInfo> Refines { get; set; }
        /// <summary>
        /// 角色任务列表
        /// </summary>
        [Association("Quests", true)]
        public DBBindingList<UserQuest> Quests { get; set; }
        /// <summary>
        /// 添加好友名单数据
        /// </summary>
        [Association("Friends", true)]
        public DBBindingList<FriendInfo> Friends { get; set; }
        /// <summary>
        /// 角色记忆传送列表信息
        /// </summary>
		[Association("FixedPoints", true)]
        public DBBindingList<FixedPointInfo> FPointLinks { get; set; }
        /// <summary>
        /// 角色绑定技能的列表值
        /// </summary>
        [Association("Values", true)]
        public DBBindingList<UserValue> Values { get; set; }
        /// <summary>
        /// 角色成就信息
        /// </summary>
        [Association("Achievements", true)]
        public DBBindingList<UserAchievement> Achievements { get; set; }
        [Association("RedPocketClaimRecords", true)]
        public DBBindingList<RedPacketClaimInfo> RedPocketClaimRecords { get; set; }
        /// <summary>
        /// 角色配偶信息
        /// </summary>
        [Association("Marriage")]
        public CharacterInfo Partner
        {
            get { return _Partner; }
            set
            {
                if (_Partner == value) return;

                var oldValue = _Partner;
                _Partner = value;

                OnChanged(oldValue, value, "Partner");
            }
        }
        private CharacterInfo _Partner;
        /// <summary>
        /// 角色CPU信息
        /// </summary>
        public string CPUInfo
        {
            get { return _CPUInfo; }
            set
            {
                if (_CPUInfo == value) return;

                var oldValue = _CPUInfo;
                _CPUInfo = value;

                OnChanged(oldValue, value, "CPUInfo");
            }
        }
        private string _CPUInfo;
        /// <summary>
        /// 角色MAC信息
        /// </summary>
        public string MACInfo
        {
            get { return _MACInfo; }
            set
            {
                if (_MACInfo == value) return;

                var oldValue = _MACInfo;
                _MACInfo = value;

                OnChanged(oldValue, value, "MACInfo");
            }
        }
        private string _MACInfo;
        /// <summary>
        /// 角色HDD信息
        /// </summary>
        public string HDDInfo
        {
            get { return _HDDInfo; }
            set
            {
                if (_HDDInfo == value) return;

                var oldValue = _HDDInfo;
                _HDDInfo = value;

                OnChanged(oldValue, value, "HDDInfo");
            }
        }
        private string _HDDInfo;
        /// <summary>
        /// 最大跟随者个数（背号）
        /// </summary>
        public int MaxFollower
        {
            get => _MaxFollower;
            set
            {
                if (_MaxFollower == value) return;

                int oldValue = _MaxFollower;
                _MaxFollower = value;

                OnChanged(oldValue, value, "MaxFollower");
            }
        }
        private int _MaxFollower = 3;
        /// <summary>
        /// 允许跟随（背号）
        /// </summary>
        public bool AllowFollowing
        {
            get => _AllowFollowing;
            set
            {
                if (_AllowFollowing == value) return;

                bool oldValue = _AllowFollowing;
                _AllowFollowing = value;

                OnChanged(oldValue, value, "AllowFollowing");
            }
        }
        private bool _AllowFollowing = false;

        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Account = null;
            Companion = null;
            Partner = null;

            base.OnDeleted();
        }
#if !ServerTool
        /// <summary>
        /// 玩家对象
        /// </summary>
        public PlayerObject Player;

        /// <summary>
        /// 角色信息排名对象
        /// </summary>
        public LinkedListNode<CharacterInfo> RankingNode;
        /// <summary>
        /// 选择信息
        /// </summary>
        /// <returns></returns>
        public SelectInfo ToSelectInfo()
        {
            return new SelectInfo
            {
                CharacterIndex = Index,
                CharacterName = CharacterName,
                Class = Class,
                Gender = Gender,
                Level = Level,
                Location = CurrentMap?.Index ?? 0,
                LastLogin = LastLogin,
                CharacterState = (int)CharacterState,
            };
        }
#endif
        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            LastStats = new Stats();
            HermitStats = new Stats();

            Observable = true;
        }

        /*  protected override internal void OnLoaded()
          {
              base.OnLoaded();

              if (LastStats == null)
                  LastStats = new Stats();

              if (HermitStats == null)
                  HermitStats = new Stats();
          }*/

        /// <summary>
        /// 角色名字字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CharacterName;
        }
    }

    /// <summary>
    /// 角色绑定键值
    /// </summary>
    [UserObject]
    public sealed class UserValue : DBObject
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        [Association("Values")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;
        /// <summary>
        /// 角色绑定的键
        /// </summary>
        public int Key
        {
            get { return _Key; }
            set
            {
                if (_Key == value) return;

                var oldValue = _Key;
                _Key = value;

                OnChanged(oldValue, value, "Key");
            }
        }
        private int _Key;
        /// <summary>
        /// 角色绑定的键值
        /// </summary>
        public int Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                var oldValue = _Value;
                _Value = value;

                OnChanged(oldValue, value, "Value");
            }
        }
        private int _Value;

        public object ObjctValue
        {
            get { return _ObjctValue; }
            set
            {
                if (_ObjctValue == value) return;

                var oldValue = _ObjctValue;
                _ObjctValue = value;

                OnChanged(oldValue, value, "ObjctValue");
            }
        }
        private object _ObjctValue;
    }
}
