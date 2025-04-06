using MirDB;
using System.ComponentModel;

namespace Library.SystemModels
{
    public sealed class AchievementInfo : DBObject
    {
        //成就类别
        public AchievementCategory Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private AchievementCategory _Category;

        //成就名称(称号)
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                var oldValue = _Title;
                _Title = value;

                OnChanged(oldValue, value, "Title");
            }
        }
        private string _Title;

        //成就描述
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;

                var oldValue = _Description;
                _Description = value;

                OnChanged(oldValue, value, "Description");
            }
        }
        private string _Description;

        //是否隐藏成就要求
        public bool IsHidden
        {
            get { return _IsHidden; }
            set
            {
                if (_IsHidden == value) return;

                var oldValue = _IsHidden;
                _IsHidden = value;

                OnChanged(oldValue, value, "IsHidden");
            }
        }
        private bool _IsHidden;

        //职业限制 
        public RequiredClass Class
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
        private RequiredClass _Class;

        //0 白色  1蓝色  2红色
        public int Grade
        {
            get { return _Grade; }
            set
            {
                if (_Grade == value) return;

                var oldValue = _Grade;
                _Grade = value;

                OnChanged(oldValue, value, "Grade");
            }
        }
        private int _Grade;

        [Association("AchievementRequirements", true)]
        public DBBindingList<AchievementRequirement> AchievementRequirements { get; set; } //完成要求

        [Association("AchievementRewards", true)]
        public DBBindingList<AchievementReward> AchievementRewards { get; set; } //完成后的奖励


        protected override internal void OnCreated()
        {
            base.OnCreated();

            Class = RequiredClass.All;

            AchievementRequirement requirement = AchievementRequirements.AddNew();

            requirement.Achievement = this;
            requirement.RequirementType = AchievementRequirementType.None;
            requirement.RequiredAmount = 0;
        }
    }

    public sealed class AchievementRequirement : DBObject
    {
        [Association("AchievementRequirements")]
        public AchievementInfo Achievement
        {
            get { return _Achievement; }
            set
            {
                if (_Achievement == value) return;

                var oldValue = _Achievement;
                _Achievement = value;

                OnChanged(oldValue, value, "Achievement");
            }
        }
        private AchievementInfo _Achievement;

        //要求类型
        public AchievementRequirementType RequirementType
        {
            get { return _RequirementType; }
            set
            {
                if (_RequirementType == value) return;

                var oldValue = _RequirementType;
                _RequirementType = value;

                OnChanged(oldValue, value, "RequirementType");
            }
        }
        private AchievementRequirementType _RequirementType;

        //道具类要求(可以是null)
        public ItemInfo ItemParameter
        {
            get { return _ItemParameter; }
            set
            {
                if (_ItemParameter == value) return;

                var oldValue = _ItemParameter;
                _ItemParameter = value;

                OnChanged(oldValue, value, "ItemParameter");
            }
        }
        private ItemInfo _ItemParameter;

        //杀怪类要求(可以是null)
        public MonsterInfo MonsterParameter
        {
            get { return _MonsterParameter; }
            set
            {
                if (_MonsterParameter == value) return;

                var oldValue = _MonsterParameter;
                _MonsterParameter = value;

                OnChanged(oldValue, value, "MonsterParameter");
            }
        }
        private MonsterInfo _MonsterParameter;

        //地图类要求(可以是null)
        public MapInfo MapParameter
        {
            get { return _MapParameter; }
            set
            {
                if (_MapParameter == value) return;

                var oldValue = _MapParameter;
                _MapParameter = value;

                OnChanged(oldValue, value, "MapParameter");
            }
        }
        private MapInfo _MapParameter;

        //法术类要求(可以是null)
        public MagicInfo MagicParameter
        {
            get { return _MagicParameter; }
            set
            {
                if (_MagicParameter == value) return;

                var oldValue = _MagicParameter;
                _MagicParameter = value;

                OnChanged(oldValue, value, "MagicParameter");
            }
        }
        private MagicInfo _MagicParameter;

        //任务要求(可以是null)
        public QuestInfo QuestParameter
        {
            get { return _QuestParameter; }
            set
            {
                if (_QuestParameter == value) return;

                var oldValue = _QuestParameter;
                _QuestParameter = value;

                OnChanged(oldValue, value, "QuestParameter");
            }
        }
        private QuestInfo _QuestParameter;

        //成就要求(可以是null)
        public AchievementInfo AchievementParameter
        {
            get { return _AchievementParameter; }
            set
            {
                if (_AchievementParameter == value) return;

                var oldValue = _AchievementParameter;
                _AchievementParameter = value;

                OnChanged(oldValue, value, "AchievementParameter");
            }
        }
        private AchievementInfo _AchievementParameter;

        //NPC要求(可以是null)
        public NPCInfo NPCParameter
        {
            get { return _NPCParameter; }
            set
            {
                if (_NPCParameter == value) return;

                var oldValue = _NPCParameter;
                _NPCParameter = value;

                OnChanged(oldValue, value, "NPCParameter");
            }
        }
        private NPCInfo _NPCParameter;

        //要求的数值
        public decimal RequiredAmount
        {
            get { return _RequiredAmount; }
            set
            {
                if (_RequiredAmount == value) return;

                var oldValue = _RequiredAmount;
                _RequiredAmount = value;

                OnChanged(oldValue, value, "RequiredAmount");
            }
        }
        private decimal _RequiredAmount;

        //反转需求
        public bool Reverse
        {
            get { return _Reverse; }
            set
            {
                if (_Reverse == value) return;

                var oldValue = _Reverse;
                _Reverse = value;

                OnChanged(oldValue, value, "Reverse");
            }
        }
        private bool _Reverse;

    }

    public sealed class AchievementReward : DBObject
    {
        [Association("AchievementRewards")]
        public AchievementInfo Achievement
        {
            get { return _Achievement; }
            set
            {
                if (_Achievement == value) return;

                var oldValue = _Achievement;
                _Achievement = value;

                OnChanged(oldValue, value, "Achievement");
            }
        }
        private AchievementInfo _Achievement;

        public ItemInfo Item
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
            }
        }
        private ItemInfo _Item;

        public int Amount
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
        private int _Amount;

        public bool Bound
        {
            get { return _Bound; }
            set
            {
                if (_Bound == value) return;

                var oldValue = _Bound;
                _Bound = value;

                OnChanged(oldValue, value, "Bound");
            }
        }
        private bool _Bound;

        public int Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration == value) return;

                var oldValue = _Duration;
                _Duration = value;

                OnChanged(oldValue, value, "Duration");
            }
        }
        private int _Duration;

        public RequiredClass Class
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
        private RequiredClass _Class;

        protected override internal void OnCreated()
        {
            base.OnCreated();

            Amount = 1;

            Class = RequiredClass.All;
        }
    }

    public enum AchievementCategory
    {
        /// <summary>
        /// 空置
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 普通
        /// </summary>
        [Description("普通")]
        Regular = 1,

        /// <summary>
        /// 生活
        /// </summary>
        [Description("生活")]
        Daily = 2,

        /// <summary>
        /// 任务
        /// </summary>
        [Description("任务")]
        Quest = 3,

        /// <summary>
        /// 杀怪
        /// </summary>
        [Description("杀怪")]
        Hunt = 4,

        /// <summary>
        /// 战斗
        /// </summary>
        [Description("战斗")]
        PK = 5,

        /// <summary>
        /// 地区
        /// </summary>
        [Description("地区")]
        Map = 6,

        /// <summary>
        /// 活动
        /// </summary>
        [Description("活动")]
        Event = 7,


    }

    public enum AchievementRequirementType
    {
        /// <summary>
        /// 空置
        /// </summary>
        [Description("空置")]
        None = 0,

        #region 移动

        /// <summary>
        /// 累计步数
        /// </summary>
        [Description("累计步数")]
        TotalSteps = 1,

        /// <summary>
        /// 步行步数
        /// </summary>
        [Description("步行步数")]
        WalkSteps = 2,

        [Description("跑步步数")]
        RunSteps = 3,

        [Description("骑马步数")]
        RideSteps = 4,

        [Description("定点传送次数")]
        TeleportTimes = 5,

        [Description("随机传送次数")]
        RandomTeleportTimes = 6,

        #endregion

        #region 战斗
        /// <summary>
        /// 平A次数
        /// </summary>
        [Description("平A次数")]
        BasicAttack = 100,

        /// <summary>
        /// 使用技能次数(需要设置MagicParameter)
        /// </summary>
        [Description("使用技能次数")]
        MagicAttack = 101,

        /// <summary>
        /// 使用特定魔法(需要设置MagicParameter)
        /// </summary>
        [Description("使用某技能时-[状态]")]
        UseMagic = 102,

        /// <summary>
        /// 累计造成伤害
        /// </summary>
        [Description("累计造成伤害")]
        TotalDamage = 103,

        /// <summary>
        /// 累计回复血量
        /// </summary>
        [Description("累计回复血量")]
        TotalHPRegen = 104,

        /// <summary>
        /// 累计回复蓝量
        /// </summary>
        [Description("累计回复蓝量")]
        TotalMPRegen = 105,

        /// <summary>
        /// 累计阵亡
        /// </summary>
        [Description("累计阵亡")]
        TotalDeath = 106,

        /// <summary>
        /// 累计吸血
        /// </summary>
        [Description("累计吸血")]
        TotalLifeSteal = 107,

        /// <summary>
        /// 火元素伤害
        /// </summary>
        [Description("火元素伤害")]
        FireDamage = 108,

        /// <summary>
        /// 冰元素伤害
        /// </summary>
        [Description("冰元素伤害")]
        IceDamage = 109,

        /// <summary>
        /// 雷元素伤害
        /// </summary>
        [Description("雷元素伤害")]
        LightingDamage = 110,

        /// <summary>
        /// 风元素伤害
        /// </summary>
        [Description("风元素伤害")]
        WindDamage = 111,

        /// <summary>
        /// 神圣元素伤害
        /// </summary>
        [Description("神圣元素伤害")]
        HolyDamage = 112,

        /// <summary>
        /// 暗黑元素伤害
        /// </summary>
        [Description("暗黑元素伤害")]
        DarkDamage = 113,

        /// <summary>
        /// 幻影元素伤害
        /// </summary>
        [Description("幻影元素伤害")]
        PhantomDamage = 114,

        /// <summary>
        /// 累计对玩家伤害
        /// </summary>
        [Description("累计对玩家伤害")]
        TotalPlayerDamage = 115,

        /// <summary>
        /// 累计对怪造成伤害
        /// </summary>
        [Description("累计对怪造成伤害")]
        TotalMonsterDamage = 116,

        /// <summary>
        /// 累计闪避
        /// </summary>
        [Description("累计闪避")]
        TotalDodge = 117,

        /// <summary>
        /// 累计被人杀死次数
        /// </summary>
        [Description("累计被人杀死次数")]
        TotalKilledByPlayer = 118,

        /// <summary>
        /// 累计被怪杀死次数
        /// </summary>
        [Description("累计被怪杀死次数")]
        TotalKilledByMonster = 118,

        /// <summary>
        /// 累计杀死玩家次数
        /// </summary>
        [Description("累计杀死玩家次数")]
        TotalPlayerKill = 119,

        /// <summary>
        /// 累计杀死战士次数
        /// </summary>
        [Description("累计杀死战士次数")]
        TotalWarriorKill = 120,

        /// <summary>
        /// 累计杀死法师次数
        /// </summary>
        [Description("累计杀死法师次数")]
        TotalWizardKill = 121,

        /// <summary>
        /// 累计杀死道士次数
        /// </summary>
        [Description("累计杀死道士次数")]
        TotalTaoistKill = 122,

        /// <summary>
        /// 累计杀死刺客次数
        /// </summary>
        [Description("累计杀死刺客次数")]
        TotalAssassinKill = 123,
        #endregion

        #region 物品
        /// <summary>
        /// 获得物品次数(需要设置ItemParameter)
        /// </summary>
        [Description("获得物品次数")]
        GainItem = 200,

        /// <summary>
        /// 使用物品次数(需要设置ItemParameter)
        /// </summary>
        [Description("使用物品次数")]
        UseItem = 201,



        #endregion

        #region 杀怪
        /// <summary>
        /// 杀怪次数(需要设置MonsterParameter)
        /// </summary>
        [Description("杀怪次数")]
        KillMonster = 300,

        /// <summary>
        /// 挖肉次数
        /// </summary>
        [Description("挖肉次数")]
        TotalHarvest = 301,

        /// <summary>
        /// 挖肉成功次数
        /// </summary>
        [Description("挖肉成功次数")]
        HarvestSuccess = 302,

        #endregion

        #region 地图

        /// <summary>
        /// 进地图次数(需要设置MapParameter)
        /// </summary>
        [Description("进地图次数")]
        EnterMap = 400,

        /// <summary>
        /// 在某地图(需要设置MapParameter)
        /// </summary>
        [Description("在某地图-[状态]")]
        InMap = 401,

        #endregion

        #region 挖矿
        [Description("挖矿次数")]
        DigTimes = 501,
        #endregion

        #region 任务
        [Description("接任务次数")]
        AcceptTimes = 600,

        [Description("完成任务次数")]
        CompleteTimes = 601,
        #endregion

        #region 装备
        /// <summary>
        /// 穿戴某物(需要设置ItemParameter)
        /// </summary>
        [Description("穿戴着某物时-[状态]")]
        WearingItem = 700,

        /// <summary>
        /// 携带某物(需要设置ItemParameter)
        /// </summary>
        [Description("携带着某物时-[状态]")]
        CarryingItem = 701,


        /// <summary>
        /// 武器锻造(升级)次数
        /// </summary>
        [Description("武器锻造(升级)次数")]
        WeaponRefine = 800,

        /// <summary>
        /// 武器冶炼(重置)次数
        /// </summary>
        [Description("武器冶炼(重置)次数")]
        WeaponReset = 801,

        /// <summary>
        /// 首饰升级次数
        /// </summary>
        [Description("首饰升级次数")]
        AccessoryRefineLevel = 802,

        /// <summary>
        /// 穿戴上某物-[动作] 需要设置ItemParameter)
        /// </summary>
        [Description("穿戴上某物-[动作]")]
        PutOnItem = 803,

        #endregion

        #region 属性
        /// <summary>
        /// 等级小于
        /// </summary>
        [Description("等级小于X时-[状态]")]
        LevelLessThan = 900,

        /// <summary>
        /// 等级大于等于
        /// </summary>
        [Description("等级大于等于X时-[状态]")]
        LevelGreaterOrEqualThan = 901,

        #endregion

        #region 活动
        /// <summary>
        /// 在线分钟数
        /// </summary>
        [Description("在线分钟数")]
        OnlineTime = 1000,

        /// <summary>
        /// 结婚次数
        /// </summary>
        [Description("结婚次数")]
        Marriage = 1001,

        /// <summary>
        /// 离婚次数
        /// </summary>
        [Description("离婚次数")]
        Divorce = 1002,
        #endregion

        #region 排行

        /// <summary>
        /// 排行榜总名次
        /// </summary>
        [Description("排行榜总名次")]
        RankAll = 1100,
        /// <summary>
        /// 排行榜战士名次
        /// </summary>
        [Description("排行榜战士名次")]
        RankWarrior = 1101,
        /// <summary>
        /// 排行榜法师名次
        /// </summary>
        [Description("排行榜法师名次")]
        RankWizard = 1102,
        /// <summary>
        /// 排行榜道士名次
        /// </summary>
        [Description("排行榜道士名次")]
        RankTaoist = 1103,
        /// <summary>
        /// 排行榜刺客名次
        /// </summary>
        [Description("排行榜刺客名次")]
        RankAssassin = 1104

        #endregion

        #region 赞助

        #endregion
    }

}
