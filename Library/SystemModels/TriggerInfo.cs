using MirDB;
using System;
using System.ComponentModel;

namespace Library.SystemModels
{
    public sealed class TriggerInfo : DBObject //触发信息
    {
        public string TriggerName
        {
            get { return _TriggerName; }
            set
            {
                if (_TriggerName == value) return;

                var oldValue = _TriggerName;
                _TriggerName = value;

                OnChanged(oldValue, value, "TriggerName");
            }
        }
        private string _TriggerName;

        public string TriggerDescription
        {
            get { return _TriggerDescription; }
            set
            {
                if (_TriggerDescription == value) return;

                var oldValue = _TriggerDescription;
                _TriggerDescription = value;

                OnChanged(oldValue, value, "TriggerDescription");
            }
        }
        private string _TriggerDescription;

        public int Probability
        {
            get { return _Probability; }
            set
            {
                if (_Probability == value) return;

                var oldValue = _Probability;
                _Probability = value;

                OnChanged(oldValue, value, "Probability");
            }
        }
        private int _Probability;

        [Association("TriggerConditions", false)]
        public DBBindingList<TriggerCondition> TriggerConditions { get; set; }

        [Association("TriggerEffects", false)]
        public DBBindingList<TriggerEffect> TriggerEffects { get; set; }



        protected override internal void OnCreated()
        {
            base.OnCreated();

            TriggerCondition condition = TriggerConditions.AddNew();

            condition.ConditionType = TriggerConditionType.None;
            condition.Trigger = this;

            TriggerEffect effect = TriggerEffects.AddNew();
            effect.Trigger = this;
        }

    }

    public sealed class TriggerCondition : DBObject //触发条件
    {
        [Association("TriggerConditions")]
        public TriggerInfo Trigger
        {
            get { return _Trigger; }
            set
            {
                if (_Trigger == value) return;

                var oldValue = _Trigger;
                _Trigger = value;

                OnChanged(oldValue, value, "Trigger");
            }
        }
        private TriggerInfo _Trigger;

        //要求类型
        public TriggerConditionType ConditionType
        {
            get { return _ConditionType; }
            set
            {
                if (_ConditionType == value) return;

                var oldValue = _ConditionType;
                _ConditionType = value;

                OnChanged(oldValue, value, "ConditionType");
            }
        }
        private TriggerConditionType _ConditionType;

        //时间要求(可以是null)
        public TimeSpan TimeParameter
        {
            get { return _TimeParameter; }
            set
            {
                if (_TimeParameter == value) return;

                var oldValue = _TimeParameter;
                _TimeParameter = value;

                OnChanged(oldValue, value, "TimeParameter");
            }
        }
        private TimeSpan _TimeParameter;

        //属性类要求(可以是null)
        public Stat StatParameter
        {
            get { return _StatParameter; }
            set
            {
                if (_StatParameter == value) return;

                var oldValue = _StatParameter;
                _StatParameter = value;

                OnChanged(oldValue, value, "StatParameter");
            }
        }
        private Stat _StatParameter;

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

        protected override internal void OnCreated()
        {
            _ItemParameter = null;
            _MonsterParameter = null;
            _MapParameter = null;
            _MagicParameter = null;
        }
    }

    public sealed class TriggerEffect : DBObject //触发效果
    {
        [Association("TriggerEffects")]
        public TriggerInfo Trigger
        {
            get { return _Trigger; }
            set
            {
                if (_Trigger == value) return;

                var oldValue = _Trigger;
                _Trigger = value;

                OnChanged(oldValue, value, "Trigger");
            }
        }
        private TriggerInfo _Trigger;

        //效果类型
        public TriggerEffectType EffectType
        {
            get { return _EffectType; }
            set
            {
                if (_EffectType == value) return;

                var oldValue = _EffectType;
                _EffectType = value;

                OnChanged(oldValue, value, "EffectType");
            }
        }
        private TriggerEffectType _EffectType;

        //效果持续时间(可以是null)
        //通过新buff实现
        public TimeSpan Duration
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
        private TimeSpan _Duration;

        //属性类效果(可以是null)
        public Stat StatParameter
        {
            get { return _StatParameter; }
            set
            {
                if (_StatParameter == value) return;

                var oldValue = _StatParameter;
                _StatParameter = value;

                OnChanged(oldValue, value, "StatParameter");
            }
        }
        private Stat _StatParameter;

        //道具类效果(可以是null)
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

        //刷怪类效果(可以是null)
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

        //地图类效果(可以是null)
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

        //法术类效果(可以是null)
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

        //是否全服通知
        public bool GlobalNotification
        {
            get { return _GlobalNotification; }
            set
            {
                if (_GlobalNotification == value) return;

                var oldValue = _GlobalNotification;
                _GlobalNotification = value;

                OnChanged(oldValue, value, "GlobalNotification");
            }
        }
        private bool _GlobalNotification;
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
    }

    public enum TriggerConditionType
    {
        /// <summary>
        /// 空置
        /// </summary>
        [Description("空置")]
        None = 0,

        #region 移动

        [Description("走路-[动作]")]
        Walking = 1,

        [Description("跑步-[动作]")]
        Running = 2,

        [Description("骑马-[动作]")]
        Riding = 3,

        [Description("随机传送-[动作]")]
        RandomTeleport = 4,

        #endregion

        #region 战斗

        [Description("平A-[动作]")]
        BasicAttack = 100,

        /// <summary>
        /// 使用技能(需要设置MagicParameter)
        /// </summary>
        [Description("使用任意技能-[动作]")]
        MagicAttack = 101,

        /// <summary>
        /// 使用特定魔法(需要设置MagicParameter)
        /// </summary>
        [Description("使用特定技能-[状态]")]
        UseMagic = 102,

        #endregion


        #region 物品
        /// <summary>
        /// 使用物品(需要设置ItemParameter)
        /// </summary>
        [Description("使用特定物品-[动作]")]
        UseItem = 200,

        /// <summary>
        /// 丢弃物品(需要设置ItemParameter)
        /// </summary>
        [Description("丢弃特定物品-[动作]")]
        DropItem = 201,

        /// <summary>
        /// 获得物品(需要设置ItemParameter)
        /// </summary>
        [Description("获得物品-[动作]")]
        GainItem = 202,

        #endregion


        #region 杀怪
        //杀boss抓宠
        /// <summary>
        /// 杀死怪物(需要设置MonsterParameter)
        /// </summary>
        [Description("杀死怪物-[动作]")]
        KillMonster = 300,


        #endregion

        #region 地图

        /// <summary>
        /// 进入地图(需要设置MapParameter)
        /// </summary>
        [Description("进入地图-[动作]")]
        EnterMap = 400,

        /// <summary>
        /// 在某地图(需要设置MapParameter)
        /// </summary>
        [Description("在某地图-[状态]")]
        InMap = 401,

        #endregion

        #region 挖矿
        [Description("挖矿-[动作]")]
        Digging = 500,
        #endregion

        #region 装备
        /// <summary>
        /// 穿戴某物(需要设置ItemParameter)
        /// </summary>
        [Description("穿戴着某物-[状态]")]
        WearingItem = 700,

        /// <summary>
        /// 携带某物(需要设置ItemParameter)
        /// </summary>
        [Description("携带着某物-[状态]")]
        CarryingItem = 701,

        #endregion

        #region 属性
        /// <summary>
        /// 等级小于
        /// </summary>
        [Description("等级小于-[状态]")]
        LevelLessThan = 800,

        /// <summary>
        /// 等级大于等于
        /// </summary>
        [Description("等级大于等于-[状态]")]
        LevelGreaterOrEqualThan = 801,

        #endregion

        #region 其他
        [Description("时间要求-[状态]")]
        TimeConstraint = 1000,
        #endregion


        #region 赞助

        #endregion
    }

    public enum TriggerEffectType
    {
        None = 0,

        StatChange = 1,

        GainItem = 2,

        TakeItem = 3,

        SpawnMonster = 4,

        TeleportToMap = 5,

        MagicLevelChange = 6,

        MagicDamageChange = 7,
    }
}
