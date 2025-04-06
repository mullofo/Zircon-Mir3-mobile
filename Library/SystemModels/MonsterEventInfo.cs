using MirDB;
using System.ComponentModel;

namespace Library.SystemModels
{
    /// <summary>
    /// 怪物事件信息
    /// </summary>
    public sealed class MonsterEventInfo : DBObject
    {
        /// <summary>
        /// 说明
        /// </summary>
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
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue == value) return;

                var oldValue = _MaxValue;
                _MaxValue = value;

                OnChanged(oldValue, value, "MaxValue");
            }
        }
        private int _MaxValue;

        [Association("Targets", true)]
        /// <summary>
        /// 目标
        /// </summary>
        public DBBindingList<MonsterEventTarget> Targets { get; set; }

        [Association("Actions", true)]
        /// <summary>
        /// 操作
        /// </summary>
        public DBBindingList<MonsterEventAction> Actions { get; set; }

        /// <summary>
        /// 当前值
        /// </summary>
        public int CurrentValue;   //服务器变量

    }
    /// <summary>
    /// 怪物事件目标
    /// </summary>
    public sealed class MonsterEventTarget : DBObject
    {
        [Association("Targets")]
        /// <summary>
        /// 怪物事件信息
        /// </summary>
        public MonsterEventInfo MonsterEvent
        {
            get { return _monsterEvent; }
            set
            {
                if (_monsterEvent == value) return;

                var oldValue = _monsterEvent;
                _monsterEvent = value;

                OnChanged(oldValue, value, "MonsterEvent");
            }
        }
        private MonsterEventInfo _monsterEvent;

        [Association("Events")]
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo Monster
        {
            get { return _Monster; }
            set
            {
                if (_Monster == value) return;

                var oldValue = _Monster;
                _Monster = value;

                OnChanged(oldValue, value, "Monster");
            }
        }
        private MonsterInfo _Monster;
        /// <summary>
        /// 爆率限制
        /// </summary>
        public int DropSet
        {
            get { return _DropSet; }
            set
            {
                if (_DropSet == value) return;

                var oldValue = _DropSet;
                _DropSet = value;

                OnChanged(oldValue, value, "DropSet");
            }
        }
        private int _DropSet;
        /// <summary>
        /// 数值
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

        /// <summary>
        /// 结构事件描述
        /// </summary>
        public string StrEventDescription { get; set; }
    }
    /// <summary>
    /// 怪物事件动作
    /// </summary>
    public sealed class MonsterEventAction : DBObject
    {
        [Association("Actions")]
        /// <summary>
        /// 怪物事件信息
        /// </summary>
        public MonsterEventInfo MonsterEvent
        {
            get { return _monsterEvent; }
            set
            {
                if (_monsterEvent == value) return;

                var oldValue = _monsterEvent;
                _monsterEvent = value;

                OnChanged(oldValue, value, "MonsterEvent");
            }
        }
        private MonsterEventInfo _monsterEvent;

        /// <summary>
        /// 触发值
        /// </summary>
        public int TriggerValue
        {
            get { return _TriggerValue; }
            set
            {
                if (_TriggerValue == value) return;

                var oldValue = _TriggerValue;
                _TriggerValue = value;

                OnChanged(oldValue, value, "TriggerValue");
            }
        }
        private int _TriggerValue;

        /// <summary>
        /// 怪物事件动作类型
        /// </summary>
        public MonsterEventActionType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private MonsterEventActionType _Type;
        /// <summary>
        /// 字符串参数1
        /// </summary>
        public string StringParameter1
        {
            get { return _StringParameter1; }
            set
            {
                if (_StringParameter1 == value) return;

                var oldValue = _StringParameter1;
                _StringParameter1 = value;

                OnChanged(oldValue, value, "StringParameter1");
            }
        }
        private string _StringParameter1;
        /// <summary>
        /// 怪物信息参数1
        /// </summary>
        public MonsterInfo MonsterParameter1
        {
            get { return _MonsterParameter1; }
            set
            {
                if (_MonsterParameter1 == value) return;

                var oldValue = _MonsterParameter1;
                _MonsterParameter1 = value;

                OnChanged(oldValue, value, "MonsterParameter1");
            }
        }
        private MonsterInfo _MonsterParameter1;
        /// <summary>
        /// 重新刷怪参数1
        /// </summary>
        public RespawnInfo RespawnParameter1
        {
            get { return _RespawnParameter1; }
            set
            {
                if (_RespawnParameter1 == value) return;

                var oldValue = _RespawnParameter1;
                _RespawnParameter1 = value;

                OnChanged(oldValue, value, "RespawnParameter1");
            }
        }
        private RespawnInfo _RespawnParameter1;

        /// <summary>
        /// 刷怪区域参数1
        /// </summary>
        public MapRegion RegionParameter1
        {
            get { return _RegionParameter1; }
            set
            {
                if (_RegionParameter1 == value) return;

                var oldValue = _RegionParameter1;
                _RegionParameter1 = value;

                OnChanged(oldValue, value, "RegionParameter1");
            }
        }
        private MapRegion _RegionParameter1;
        /// <summary>
        /// 地图参数1
        /// </summary>
        public MapInfo MapParameter1
        {
            get { return _MapParameter1; }
            set
            {
                if (_MapParameter1 == value) return;

                var oldValue = _MapParameter1;
                _MapParameter1 = value;

                OnChanged(oldValue, value, "MapParameter1");
            }
        }
        private MapInfo _MapParameter1;
    }

    /// <summary>
    /// 怪物事件操作类型
    /// </summary>
    public enum MonsterEventActionType
    {
        /// <summary>
        /// 置空
        /// </summary>
        [Description("None 置空")]
        None,
        /// <summary>
        /// 系统事件操作类型
        /// </summary>
        [Description("GlobalMessage 系统事件操作类型")]
        GlobalMessage,
        /// <summary>
        /// 地图事件操作类型
        /// </summary>
        [Description("MapMessage 地图事件操作类型")]
        MapMessage,
        /// <summary>
        /// 玩家事件操作类型
        /// </summary>
        [Description("PlayerMessage 玩家事件操作类型")]
        PlayerMessage,
        /// <summary>
        /// 怪物刷新事件操作类型
        /// </summary>
        [Description("MonsterSpawn 怪物刷新事件操作类型")]
        MonsterSpawn,
        /// <summary>
        /// 玩家杀怪事件操作类型
        /// </summary>
        [Description("MonsterPlayerSpawn 玩家杀怪事件操作类型")]
        MonsterPlayerSpawn,
        /// <summary>
        /// 移动事件操作类型
        /// </summary>
        [Description("MovementSettings 移动事件操作类型")]
        MovementSettings,
        /// <summary>
        /// 玩家传送事件操作类型
        /// </summary>
        [Description("PlayerRecall 玩家传送事件操作类型")]
        PlayerRecall,
        /// <summary>
        /// 玩家逃脱事件操作类型
        /// </summary>
        [Description("PlayerEscape 玩家逃脱事件操作类型")]
        PlayerEscape,
        /// <summary>
        /// 连续击杀怪物事件操作类型
        /// </summary>
        [Description("KillStreak 连续击杀怪物事件操作类型")]
        KillStreak,
        /// <summary>
        /// BOSS复活事件操作类型
        /// </summary>
        [Description("SuperMobRespawn BOSS复活事件操作类型")]
        SuperMobRespawn,
    }
}
