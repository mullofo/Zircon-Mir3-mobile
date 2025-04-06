using MirDB;
using System.ComponentModel;

namespace Library.SystemModels
{
    /// <summary>
    /// 任务信息
    /// </summary>
    public sealed class QuestInfo : DBObject
    {
        /// <summary>
        /// 任务名字
        /// </summary>
        public string QuestName
        {
            get { return _QuestName; }
            set
            {
                if (_QuestName == value) return;

                var oldValue = _QuestName;
                _QuestName = value;

                OnChanged(oldValue, value, "QuestName");
            }
        }
        private string _QuestName;
        /// <summary>
        /// 任务类型
        /// </summary>
        public QuestType QuestType
        {
            get { return _QuestType; }
            set
            {
                if (_QuestType == value) return;

                var oldValue = _QuestType;
                _QuestType = value;

                OnChanged(oldValue, value, "QuestType");
            }
        }
        private QuestType _QuestType;
        /// <summary>
        /// 接任务对话内容
        /// </summary>
        public string AcceptText
        {
            get { return _AcceptText; }
            set
            {
                if (_AcceptText == value) return;

                var oldValue = _AcceptText;
                _AcceptText = value;

                OnChanged(oldValue, value, "AcceptText");
            }
        }
        private string _AcceptText;
        /// <summary>
        /// 进行中的任务内容
        /// </summary>
        public string ProgressText
        {
            get { return _ProgressText; }
            set
            {
                if (_ProgressText == value) return;

                var oldValue = _ProgressText;
                _ProgressText = value;

                OnChanged(oldValue, value, "ProgressText");
            }
        }
        private string _ProgressText;
        /// <summary>
        /// 完成的任务内容
        /// </summary>
        public string CompletedText
        {
            get { return _CompletedText; }
            set
            {
                if (_CompletedText == value) return;

                var oldValue = _CompletedText;
                _CompletedText = value;

                OnChanged(oldValue, value, "CompletedText");
            }
        }
        private string _CompletedText;
        /// <summary>
        /// 记录在完成任务中的内容
        /// </summary>
        public string ArchiveText
        {
            get { return _ArchiveText; }
            set
            {
                if (_ArchiveText == value) return;

                var oldValue = _ArchiveText;
                _ArchiveText = value;

                OnChanged(oldValue, value, "ArchiveText");
            }
        }
        private string _ArchiveText;

        /// <summary>
        /// 任务需求
        /// </summary>
        [Association("Requirements", true)]
        public DBBindingList<QuestRequirement> Requirements { get; set; }
        /// <summary>
        /// 任务开始NPC
        /// </summary>
        [Association("StartQuests")]
        public NPCInfo StartNPC
        {
            get { return _StartNPC; }
            set
            {
                if (_StartNPC == value) return;

                var oldValue = _StartNPC;
                _StartNPC = value;

                OnChanged(oldValue, value, "StartNPC");
            }
        }
        private NPCInfo _StartNPC;
        /// <summary>
        /// 任务结束NPC
        /// </summary>
        [Association("FinishQuests")]
        public NPCInfo FinishNPC
        {
            get { return _FinishNPC; }
            set
            {
                if (_FinishNPC == value) return;

                var oldValue = _FinishNPC;
                _FinishNPC = value;

                OnChanged(oldValue, value, "FinishNPC");
            }
        }
        private NPCInfo _FinishNPC;
        /// <summary>
        /// 任务使用道具获得
        /// </summary>
        public ItemInfo StartItem
        {
            get { return _StartItem; }
            set
            {
                if (_StartItem == value) return;

                var oldValue = _StartItem;
                _StartItem = value;

                OnChanged(oldValue, value, "StartItem");
            }
        }
        private ItemInfo _StartItem;


        //杨伟
        //private string _StartItemName;
        /// <summary>
        /// 开始任务的道具名字
        /// </summary>
        //public string StartItemName
        //{
        //get { return _StartItemName; }
        //set
        //{
        //if (_StartItemName == value) return;

        //var oldValue = _StartItemName;
        //_StartItemName = value;

        //OnChanged(oldValue, value, "IntParameter1");
        //}
        //}

        /// <summary>
        /// 任务奖励
        /// </summary>
        [Association("Rewards", true)]
        public DBBindingList<QuestReward> Rewards { get; set; }
        /// <summary>
        /// 任务需求
        /// </summary>
        [Association("Tasks", true)]
        public DBBindingList<QuestTask> Tasks { get; set; }

        /// <summary>
        /// 开始任务NPC名字
        /// </summary>
        //public string StartNPCName
        //{
        //    get { return _StartNPCName; }
        //    set
        //    {
        //        if (_StartNPCName == value) return;

        //        var oldValue = _StartNPCName;
        //        _StartNPCName = value;

        //        //OnChanged(oldValue, value, "IntParameter1");
        //    }
        //}
        //private string _StartNPCName;
        /// <summary>
        /// 结束任务NPC名字
        /// </summary>
        //public string FinishNPCName
        //{
        //    get { return _FinishNPCName; }
        //    set
        //    {
        //        if (_FinishNPCName == value) return;

        //        var oldValue = _FinishNPCName;
        //        _FinishNPCName = value;

        //        //OnChanged(oldValue, value, "IntParameter1");
        //    }
        //}
        //private string _FinishNPCName;
        /// <summary>
        /// 任务类型名字
        /// </summary>
        //public string QuestTypeName
        //{
        //    get { return _QuestTypeName; }
        //    set
        //    {
        //        if (_QuestTypeName == value) return;

        //        var oldValue = _QuestTypeName;
        //        _QuestTypeName = value;

        //        //OnChanged(oldValue, value, "IntParameter1");
        //    }
        //}
        //private string _QuestTypeName;
        /// <summary>
        /// 复制ID索引
        /// </summary>
        //public int IndexCopy
        //{
        //    get { return _IndexCopy; }
        //    set
        //    {
        //        if (_IndexCopy == value) return;

        //        var oldValue = _IndexCopy;
        //        _IndexCopy = value;

        //        //OnChanged(oldValue, value, "IntParameter1");
        //    }
        //}
        //private int _IndexCopy;
        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            QuestRequirement requirement = Requirements.AddNew();

            requirement.Requirement = QuestRequirementType.HaveNotCompleted;
            requirement.Quest = this;
            requirement.QuestParameter = this;
        }
    }
    /// <summary>
    /// 任务奖励
    /// </summary>
    public sealed class QuestReward : DBObject
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        [Association("Rewards")]
        public QuestInfo Quest
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private QuestInfo _Quest;
        /// <summary>
        /// 任务奖励的道具
        /// </summary>
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
        /// <summary>
        /// 任务奖励的道具数量
        /// </summary>
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
        /// <summary>
        /// 是否可以选择对应的奖励
        /// </summary>
        public bool Choice
        {
            get { return _Choice; }
            set
            {
                if (_Choice == value) return;

                var oldValue = _Choice;
                _Choice = value;

                OnChanged(oldValue, value, "Choice");
            }
        }
        private bool _Choice;
        /// <summary>
        /// 是否随机奖励
        /// </summary>
        public bool Random
        {
            get { return _Random; }
            set
            {
                if (_Random == value) return;

                var oldValue = _Random;
                _Random = value;

                OnChanged(oldValue, value, "Random");
            }
        }
        private bool _Random;
        /// <summary>
        /// 是否绑定道具
        /// </summary>
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
        /// <summary>
        /// 道具是否时效
        /// </summary>
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
        /// <summary>
        /// 奖励对应的职业
        /// </summary>
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
        /// <summary>
        /// 任务信息ID索引
        /// </summary>
        public int QuertInfoIndex
        {
            get { return _QuertInfoIndex; }
            set
            {
                if (_QuertInfoIndex == value) return;

                var oldValue = _QuertInfoIndex;
                _QuertInfoIndex = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _QuertInfoIndex;
        /// <summary>
        /// 道具名字
        /// </summary>
        public string ItemName
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName == value) return;

                var oldValue = _ItemName;
                _ItemName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _ItemName;
        /// <summary>
        /// 职业名字
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set
            {
                if (_ClassName == value) return;

                var oldValue = _ClassName;
                _ClassName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _ClassName;
        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            Amount = 1;

            Class = RequiredClass.All;
        }
    }

    /// <summary>
    /// 任务要求
    /// </summary>
    public sealed class QuestRequirement : DBObject
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        [Association("Requirements")]
        public QuestInfo Quest
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private QuestInfo _Quest;
        /// <summary>
        /// 任务要求的类型
        /// </summary>
        public QuestRequirementType Requirement
        {
            get { return _Requirement; }
            set
            {
                if (_Requirement == value) return;

                var oldValue = _Requirement;
                _Requirement = value;

                OnChanged(oldValue, value, "Requirement");
            }
        }
        private QuestRequirementType _Requirement;
        /// <summary>
        /// 对应任务要求类型的参数
        /// </summary>
        public int IntParameter1
        {
            get { return _IntParameter1; }
            set
            {
                if (_IntParameter1 == value) return;

                var oldValue = _IntParameter1;
                _IntParameter1 = value;

                OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _IntParameter1;
        /// <summary>
        /// 任务参数
        /// </summary>
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
        /// <summary>
        /// 任务要求职业
        /// </summary>
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

        /// <summary>
        /// 任务信息ID索引
        /// </summary>
        public int QuertInfoIndex
        {
            get { return _QuertInfoIndex; }
            set
            {
                if (_QuertInfoIndex == value) return;

                var oldValue = _QuertInfoIndex;
                _QuertInfoIndex = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _QuertInfoIndex;
        /// <summary>
        /// 任务要求名字
        /// </summary>
        public string RequirementName
        {
            get { return _RequirementName; }
            set
            {
                if (_RequirementName == value) return;

                var oldValue = _RequirementName;
                _RequirementName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _RequirementName;

        /// <summary>
        /// 任务职业要求名字
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set
            {
                if (_ClassName == value) return;

                var oldValue = _ClassName;
                _ClassName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _ClassName;

        /// <summary>
        /// 任务要求参数名字
        /// </summary>
        public string QuestParameterName
        {
            get { return _QuestParameterName; }
            set
            {
                if (_QuestParameterName == value) return;

                var oldValue = _QuestParameterName;
                _QuestParameterName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _QuestParameterName;
    }
    /// <summary>
    /// 任务要求
    /// </summary>
    public sealed class QuestTask : DBObject
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        [Association("Tasks")]
        public QuestInfo Quest
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private QuestInfo _Quest;
        /// <summary>
        /// 任务要求类型
        /// </summary>
        public QuestTaskType Task
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;

                OnChanged(oldValue, value, "Task");
            }
        }
        private QuestTaskType _Task;
        /// <summary>
        /// 任务要求道具参数
        /// </summary>
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
        /// <summary>
        /// 任务要求对应的地图
        /// </summary>
        public string MobDescription
        {
            get { return _MobDescription; }
            set
            {
                if (_MobDescription == value) return;

                var oldValue = _MobDescription;
                _MobDescription = value;

                OnChanged(oldValue, value, "MobDescription");
            }
        }
        private string _MobDescription;
        /// <summary>
        /// 任务要求对应的数量
        /// </summary>
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
        /// <summary>
        /// 任务要求的怪物详细信息
        /// </summary>
        [Association("MonsterDetails", true)]
        public DBBindingList<QuestTaskMonsterDetails> MonsterDetails { get; set; }

        /// <summary>
        /// 任务信息ID索引
        /// </summary>
        public int QuertInfoIndex
        {
            get { return _QuertInfoIndex; }
            set
            {
                if (_QuertInfoIndex == value) return;

                var oldValue = _QuertInfoIndex;
                _QuertInfoIndex = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _QuertInfoIndex;

        /// <summary>
        /// 任务要求的名字
        /// </summary>
        public string TaskName
        {
            get { return _TaskName; }
            set
            {
                if (_TaskName == value) return;

                var oldValue = _TaskName;
                _TaskName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _TaskName;
        /// <summary>
        /// 任务要求道具的名字
        /// </summary>
        public string ItemParameterName
        {
            get { return _ItemParameterName; }
            set
            {
                if (_ItemParameterName == value) return;

                var oldValue = _ItemParameterName;
                _ItemParameterName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _ItemParameterName;
        /// <summary>
        /// ID索引
        /// </summary>
        public int IndexCopy
        {
            get { return _IndexCopy; }
            set
            {
                if (_IndexCopy == value) return;

                var oldValue = _IndexCopy;
                _IndexCopy = value;

                // OnChanged(oldValue, value, "Amount");
            }
        }
        private int _IndexCopy;
    }
    /// <summary>
    /// 任务要求杀怪类型
    /// </summary>
    public sealed class QuestTaskMonsterDetails : DBObject
    {
        /// <summary>
        /// 任务要求
        /// </summary>
        [Association("MonsterDetails")]
        public QuestTask Task
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;

                OnChanged(oldValue, value, "Task");
            }
        }
        private QuestTask _Task;
        /// <summary>
        /// 任务要求怪物信息
        /// </summary>
        [Association("QuestDetails")]
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
        /// 任务要求制定的地图
        /// </summary>
        public MapInfo Map
        {
            get { return _Map; }
            set
            {
                if (_Map == value) return;

                var oldValue = _Map;
                _Map = value;

                OnChanged(oldValue, value, "Map");
            }
        }
        private MapInfo _Map;
        /// <summary>
        /// 任务要求对应的参数
        /// </summary>
        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;

                OnChanged(oldValue, value, "Chance");
            }
        }
        private int _Chance;
        /// <summary>
        /// 任务要求对应的数量
        /// </summary>
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

        /// <summary>
        /// 任务要求的爆率
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
        /// 任务要求的ID所有
        /// </summary>
        public int QuerTaskIndex
        {
            get { return _QuerTaskIndex; }
            set
            {
                if (_QuerTaskIndex == value) return;

                var oldValue = _QuerTaskIndex;
                _QuerTaskIndex = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private int _QuerTaskIndex;
        /// <summary>
        /// 任务要求的怪物名字
        /// </summary>
        public string MonsterName
        {
            get { return _MonsterName; }
            set
            {
                if (_MonsterName == value) return;

                var oldValue = _MonsterName;
                _MonsterName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _MonsterName;
        /// <summary>
        /// 任务要求的地图名字
        /// </summary>
        public string MapName
        {
            get { return _MapName; }
            set
            {
                if (_MapName == value) return;

                var oldValue = _MapName;
                _MapName = value;

                //OnChanged(oldValue, value, "IntParameter1");
            }
        }
        private string _MapName;
        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            Chance = 1;
            Amount = 1;
        }
    }

    /// <summary>
    /// 可以接受的任务类型
    /// </summary>
    public enum QuestRequirementType
    {
        /// <summary>
        /// 最低级别
        /// </summary>
        [Description("MinLevel最低级别")]
        MinLevel,
        /// <summary>
        /// 最高级别
        /// </summary>
        [Description("MaxLevel最高级别")]
        MaxLevel,
        /// <summary>
        /// 未接任务
        /// </summary>
        [Description("NotAccepted未接任务")]
        NotAccepted,
        /// <summary>
        /// 已完成任务
        /// </summary>
        [Description("HaveCompleted已完成任务")]
        HaveCompleted,
        /// <summary>
        /// 尚未完成任务
        /// </summary>
        [Description("HaveNotCompleted尚未完成任务")]
        HaveNotCompleted,
        /// <summary>
        /// 要求职业
        /// </summary>
        [Description("Class要求职业")]
        Class,
    }

    /// <summary>
    /// 任务需求类型
    /// </summary>
    public enum QuestTaskType
    {
        /// <summary>
        /// 击杀怪物
        /// </summary>
        [Description("击杀怪物")]
        KillMonster,
        /// <summary>
        /// 收集道具
        /// </summary>
        [Description("收集道具")]
        GainItem,
    }
}
