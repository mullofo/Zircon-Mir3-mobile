using MirDB;
using System;
using System.ComponentModel;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// NPC信息
    /// </summary>
    [Lang]
    public sealed class NPCInfo : DBObject
    {
        /// <summary>
        /// NPC脚本路径
        /// </summary>
        public string NPCFile
        {
            get { return _NPCFile; }
            set
            {
                if (_NPCFile == value) return;

                var oldValue = _NPCFile;
                _NPCFile = value;

                OnChanged(oldValue, value, "NPCFile");
            }
        }
        private string _NPCFile;
        /// <summary>
        /// NPC指定的区域
        /// </summary>
        public MapRegion Region
        {
            get { return _Region; }
            set
            {
                if (_Region == value) return;

                var oldValue = _Region;
                _Region = value;

                OnChanged(oldValue, value, "Region");
            }
        }
        private MapRegion _Region;
        /// <summary>
        /// NPC名字
        /// </summary>
        [Lang("NPC名")]
        [Export]
        public string NPCName
        {
            get { return _NPCName; }
            set
            {
                if (_NPCName == value) return;

                var oldValue = _NPCName;
                _NPCName = value;

                OnChanged(oldValue, value, "NPCName");
            }
        }
        private string _NPCName;
        /// <summary>
        /// NPC图像
        /// </summary>
        public int Image
        {
            get { return _Image; }
            set
            {
                if (_Image == value) return;

                var oldValue = _Image;
                _Image = value;

                OnChanged(oldValue, value, "Image");
            }
        }
        private int _Image;
        /// <summary>
        /// NPC隐藏显示
        /// </summary>
        public bool Display
        {
            get { return _Display; }
            set
            {
                if (_Display == value) return;

                var oldValue = _Display;
                _Display = value;

                OnChanged(oldValue, value, "Display");
            }
        }
        private bool _Display;
        /// <summary>
        /// 攻城是否移除NPC
        /// </summary>
        public bool WarDisplay
        {
            get { return _WarDisplay; }
            set
            {
                if (_WarDisplay == value) return;

                var oldValue = _WarDisplay;
                _WarDisplay = value;

                OnChanged(oldValue, value, "WarDisplay");
            }
        }
        private bool _WarDisplay;
        /// <summary>
        /// NPC进入页面(目前弃用)
        /// </summary>
        /*public NPCPage EntryPage
        {
            get { return _EntryPage; }
            set
            {
                if (_EntryPage == value) return;

                var oldValue = _EntryPage;
                _EntryPage = value;

                OnChanged(oldValue, value, "EntryPage");
            }
        }
        private NPCPage _EntryPage;
        /// <summary>
        /// NPC脚本接口(后续火鸟脚本链接)
        /// </summary>
        public string Script
        {
            get { return _Script; }
            set
            {
                if (_Script == value) return;
                var oldValue = _Script;
                _Script = value;

                OnChanged(oldValue, value, "Script");
            }
        }
        private string _Script;*/

        /// <summary>
        /// NPC的设定区域名字
        /// </summary>
        [IgnoreProperty]
        public string RegionName => Region?.ServerDescription ?? string.Empty;
        /// <summary>
        /// 开始任务
        /// </summary>
        [Association("StartQuests")]
        public DBBindingList<QuestInfo> StartQuests { get; set; }
        /// <summary>
        /// 结束任务
        /// </summary>
        [Association("FinishQuests")]
        public DBBindingList<QuestInfo> FinishQuests { get; set; }
        /// <summary>
        /// 当前任务图标
        /// </summary>
        public QuestIcon CurrentIcon;
    }
    /// <summary>
    /// 原NPC内容设置页（目前已经停用）
    /// </summary>
    //public sealed class NPCPage : DBObject
    //{
    //    public string Description
    //    {
    //        get { return _Description; }
    //        set
    //        {
    //            if (_Description == value) return;

    //            var oldValue = _Description;
    //            _Description = value;

    //            OnChanged(oldValue, value, "Description");
    //        }
    //    }
    //    private string _Description;

    //    public NPCDialogType DialogType
    //    {
    //        get { return _DialogType; }
    //        set
    //        {
    //            if (_DialogType == value) return;

    //            var oldValue = _DialogType;
    //            _DialogType = value;

    //            OnChanged(oldValue, value, "DialogType");
    //        }
    //    }
    //    private NPCDialogType _DialogType;

    //    public string Say
    //    {
    //        get { return _Say; }
    //        set
    //        {
    //            if (_Say == value) return;

    //            var oldValue = _Say;
    //            _Say = value;

    //            OnChanged(oldValue, value, "Say");
    //        }
    //    }
    //    private string _Say;

    //    public NPCPage SuccessPage
    //    {
    //        get { return _SuccessPage; }
    //        set
    //        {
    //            if (_SuccessPage == value) return;

    //            var oldValue = _SuccessPage;
    //            _SuccessPage = value;

    //            OnChanged(oldValue, value, "SuccessPage");
    //        }
    //    }
    //    private NPCPage _SuccessPage;

    //    public string Arguments
    //    {
    //        get { return _Arguments; }
    //        set
    //        {
    //            if (_Arguments == value) return;

    //            var oldValue = _Arguments;
    //            _Arguments = value;

    //            OnChanged(oldValue, value, "Arguments");
    //        }
    //    }
    //    private string _Arguments;

    //    [Association("Checks", true)]
    //    public DBBindingList<NPCCheck> Checks { get; set; }

    //    [Association("Actions", true)]
    //    public DBBindingList<NPCAction> Actions { get; set; }

    //    [Association("Buttons", true)]
    //    public DBBindingList<NPCButton> Buttons { get; set; }

    //    [Association("Goods", true)]
    //    public DBBindingList<NPCGood> Goods { get; set; }

    //    [Association("Types", true)]
    //    public DBBindingList<NPCType> Types { get; set; }
    //}

    public sealed class NPCGood : DBObject
    {
        //[Association("Goods")]
        //public NPCPage Page
        //{
        //    get { return _Page; }
        //    set
        //    {
        //        if (_Page == value) return;

        //        var oldValue = _Page;
        //        _Page = value;

        //        OnChanged(oldValue, value, "Page");
        //    }
        //}
        //private NPCPage _Page;


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

        public decimal Rate
        {
            get { return _Rate; }
            set
            {
                if (_Rate == value) return;

                var oldValue = _Rate;
                _Rate = value;

                OnChanged(oldValue, value, "Rate");
            }
        }
        private decimal _Rate;

        protected override internal void OnCreated()
        {
            base.OnCreated();

            Rate = 1M;
        }

        [IgnoreProperty]
        public int Cost => (int)Math.Round(Item.Price * Rate);
    }

    public sealed class NPCType : DBObject
    {
        //[Association("Types")]
        //public NPCPage Page
        //{
        //    get { return _Page; }
        //    set
        //    {
        //        if (_Page == value) return;

        //        var oldValue = _Page;
        //        _Page = value;

        //        OnChanged(oldValue, value, "Page");
        //    }
        //}
        //private NPCPage _Page;

        public ItemType ItemType
        {
            get { return _ItemType; }
            set
            {
                if (_ItemType == value) return;

                var oldValue = _ItemType;
                _ItemType = value;

                OnChanged(oldValue, value, "ItemType");
            }
        }
        private ItemType _ItemType;
    }

    public sealed class NPCCheck : DBObject
    {
        //[Association("Checks")]
        //public NPCPage Page
        //{
        //    get { return _Page; }
        //    set
        //    {
        //        if (_Page == value) return;

        //        var oldValue = _Page;
        //        _Page = value;

        //        OnChanged(oldValue, value, "Page");
        //    }
        //}
        //private NPCPage _Page;

        public NPCCheckType CheckType
        {
            get { return _CheckType; }
            set
            {
                if (_CheckType == value) return;

                var oldValue = _CheckType;
                _CheckType = value;

                OnChanged(oldValue, value, "CheckType");
            }
        }
        private NPCCheckType _CheckType;

        public Operator Operator
        {
            get { return _Operator; }
            set
            {
                if (_Operator == value) return;

                var oldValue = _Operator;
                _Operator = value;

                OnChanged(oldValue, value, "Operator");
            }
        }
        private Operator _Operator;

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

        public int IntParameter2
        {
            get { return _IntParameter2; }
            set
            {
                if (_IntParameter2 == value) return;

                var oldValue = _IntParameter2;
                _IntParameter2 = value;

                OnChanged(oldValue, value, "IntParameter2");
            }
        }
        private int _IntParameter2;

        public ItemInfo ItemParameter1
        {
            get { return _ItemParameter1; }
            set
            {
                if (_ItemParameter1 == value) return;

                var oldValue = _ItemParameter1;
                _ItemParameter1 = value;

                OnChanged(oldValue, value, "ItemParameter1");
            }
        }
        private ItemInfo _ItemParameter1;


        public Stat StatParameter1
        {
            get { return _StatParameter1; }
            set
            {
                if (_StatParameter1 == value) return;

                var oldValue = _StatParameter1;
                _StatParameter1 = value;

                OnChanged(oldValue, value, "StatParameter1");
            }
        }
        private Stat _StatParameter1;

        //public NPCPage FailPage
        //{
        //    get { return _FailPage; }
        //    set
        //    {
        //        if (_FailPage == value) return;

        //        var oldValue = _FailPage;
        //        _FailPage = value;

        //        OnChanged(oldValue, value, "FailPage");
        //    }
        //}
        //private NPCPage _FailPage;
    }

    public sealed class NPCAction : DBObject
    {
        //[Association("Actions")]
        //public NPCPage Page
        //{
        //    get { return _Page; }
        //    set
        //    {
        //        if (_Page == value) return;

        //        var oldValue = _Page;
        //        _Page = value;

        //        OnChanged(oldValue, value, "Page");
        //    }
        //}
        //private NPCPage _Page;

        public NPCActionType ActionType
        {
            get { return _ActionType; }
            set
            {
                if (_ActionType == value) return;

                var oldValue = _ActionType;
                _ActionType = value;

                OnChanged(oldValue, value, "ActionType");
            }
        }
        private NPCActionType _ActionType;

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

        public int IntParameter2
        {
            get { return _IntParameter2; }
            set
            {
                if (_IntParameter2 == value) return;

                var oldValue = _IntParameter2;
                _IntParameter2 = value;

                OnChanged(oldValue, value, "IntParameter2");
            }
        }
        private int _IntParameter2;

        public ItemInfo ItemParameter1
        {
            get { return _ItemParameter1; }
            set
            {
                if (_ItemParameter1 == value) return;

                var oldValue = _ItemParameter1;
                _ItemParameter1 = value;

                OnChanged(oldValue, value, "ItemParameter1");
            }
        }
        private ItemInfo _ItemParameter1;

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

        public Stat StatParameter1
        {
            get { return _StatParameter1; }
            set
            {
                if (_StatParameter1 == value) return;

                var oldValue = _StatParameter1;
                _StatParameter1 = value;

                OnChanged(oldValue, value, "StatParameter1");
            }
        }
        private Stat _StatParameter1;
    }

    public sealed class NPCButton : DBObject
    {
        //[Association("Buttons")]
        //public NPCPage Page
        //{
        //    get { return _Page; }
        //    set
        //    {
        //        if (_Page == value) return;

        //        var oldValue = _Page;
        //        _Page = value;

        //        OnChanged(oldValue, value, "Page");
        //    }
        //}
        //private NPCPage _Page;

        public int ButtonID
        {
            get { return _ButtonID; }
            set
            {
                if (_ButtonID == value) return;

                var oldValue = _ButtonID;
                _ButtonID = value;

                OnChanged(oldValue, value, "ButtonID");
            }
        }
        private int _ButtonID;

        //public NPCPage DestinationPage
        //{
        //    get { return _DestinationPage; }
        //    set
        //    {
        //        if (_DestinationPage == value) return;

        //        var oldValue = _DestinationPage;
        //        _DestinationPage = value;

        //        OnChanged(oldValue, value, "DestinationPage");
        //    }
        //}
        //private NPCPage _DestinationPage;



    }

    /// <summary>
    /// NPC判断类型
    /// </summary>
    public enum NPCCheckType
    {
        /// <summary>
        /// NPC检查类型 等级
        /// </summary>
        [Description("Level 判断等级")]
        Level,
        /// <summary>
        /// NPC检查类型 职业
        /// </summary>
        [Description("Class 判断职业")]
        Class,
        /// <summary>
        /// NPC检查类型 性别
        /// </summary>
        [Description("Gender 判断性别")]
        Gender,
        /// <summary>
        /// NPC检查类型 金币
        /// </summary>
        [Description("Gold 判断金币")]
        Gold,
        /// <summary>
        /// NPC检查类型 持有道具
        /// </summary>
        [Description("HasItem 判断持有的道具")]
        HasItem,
        /// <summary>
        /// NPC检查类型 PK值
        /// </summary>
        [Description("PKPoints 判断PK值")]
        PKPoints,
        /// <summary>
        /// NPC检查类型 持有武器
        /// </summary>
        [Description("HasWeapon 判断手上武器")]
        HasWeapon,
        /// <summary>
        /// NPC检查类型 武器等级
        /// </summary>
        [Description("WeaponLevel 判断武器等级")]
        WeaponLevel,
        /// <summary>
        /// NPC检查类型 武器元素
        /// </summary>
        [Description("WeaponElement 判断武器元素")]
        WeaponElement,
        /// <summary>
        /// NPC检查类型 武器是否精炼
        /// </summary>
        [Description("WeaponCanRefine 判断武器是否精炼")]
        WeaponCanRefine,
        /// <summary>
        /// NPC检查类型 坐骑
        /// </summary>
        [Description("Horse 判断坐骑")]
        Horse,
        /// <summary>
        /// NPC检查类型 结婚
        /// </summary>
        [Description("Marriage 判断是否结婚")]
        Marriage,
        /// <summary>
        /// NPC检查类型 结婚戒指
        /// </summary>
        [Description("WeddingRing 判断结婚戒指")]
        WeddingRing,
        /// <summary>
        /// NPC检查类型 可获得的道具
        /// </summary>
        [Description("CanGainItem 判断可获得的道具")]
        CanGainItem,
        /// <summary>
        /// NPC检查类型 可重置武器
        /// </summary>
        [Description("CanResetWeapon 判断是否可重置武器")]
        CanResetWeapon,
        /// <summary>
        /// NPC检查类型 随机值
        /// </summary>
        [Description("Random 判断随机值")]
        Random,
        /// <summary>
        /// NPC检查类型 武器增加属性状态
        /// </summary>
        [Description("WeaponAddedStats 判断武器增加属性状态")]
        WeaponAddedStats
    }
    /// <summary>
    /// 数值对比参数
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// 操作 等于
        /// </summary>
        [Description("Equal 等于")]
        Equal,
        /// <summary>
        /// 操作 不等于
        /// </summary>
        [Description("NotEqual 不等于")]
        NotEqual,
        /// <summary>
        /// 操作 少于
        /// </summary>
        [Description("LessThan 少于")]
        LessThan,
        /// <summary>
        /// 操作 小于或等于
        /// </summary>
        [Description("LessThanOrEqual 小于或者等于")]
        LessThanOrEqual,
        /// <summary>
        /// 操作 大于
        /// </summary>
        [Description("GreaterThan 大于")]
        GreaterThan,
        /// <summary>
        /// 操作 大于或等于
        /// </summary>
        [Description("GreaterThanOrEqual 大于或者等于")]
        GreaterThanOrEqual,
    }
    /// <summary>
    /// NPC执行动作类型
    /// </summary>
    public enum NPCActionType
    {
        /// <summary>
        /// NPC动作类型 传送
        /// </summary>
        [Description("Teleport 传送")]
        Teleport,
        /// <summary>
        /// NPC动作类型 给金币
        /// </summary>
        [Description("GiveGold 给金币")]
        GiveGold,
        /// <summary>
        /// NPC动作类型 扣金币
        /// </summary>
        [Description("TakeGold 扣金币")]
        TakeGold,
        /// <summary>
        /// NPC动作类型 给道具
        /// </summary>
        [Description("GiveItem 给道具")]
        GiveItem,
        /// <summary>
        /// NPC动作类型 扣道具
        /// </summary>
        [Description("TakeItem 扣道具")]
        TakeItem,
        /// <summary>
        /// NPC动作类型 改变元素
        /// </summary>
        [Description("ChangeElement 改变元素")]
        ChangeElement,
        /// <summary>
        /// NPC动作类型 改变坐骑
        /// </summary>
        [Description("ChangeHorse 改变坐骑")]
        ChangeHorse,
        /// <summary>
        /// NPC动作类型 说话
        /// </summary>
        [Description("Message 说话")]
        Message,
        /// <summary>
        /// NPC动作类型 结婚
        /// </summary>
        [Description("Marriage 结婚")]
        Marriage,
        /// <summary>
        /// NPC动作类型 离婚
        /// </summary>
        [Description("Divorce 离婚")]
        Divorce,
        /// <summary>
        /// NPC动作类型 移除婚戒
        /// </summary>
        [Description("RemoveWeddingRing 移除婚戒")]
        RemoveWeddingRing,
        /// <summary>
        /// NPC动作类型 重置武器
        /// </summary>
        [Description("ResetWeapon 重置武器")]
        ResetWeapon,
        /// <summary>
        /// NPC动作类型 给予道具经验
        /// </summary>
        [Description("GiveItemExperience 给道具加经验")]
        GiveItemExperience,
        /// <summary>
        /// NPC动作类型 特殊精炼
        /// </summary>
        [Description("SpecialRefine 特殊精炼")]
        SpecialRefine,
        /// <summary>
        /// NPC动作类型 转生
        /// </summary>
        [Description("Rebirth 转生")]
        Rebirth,
    }
}
