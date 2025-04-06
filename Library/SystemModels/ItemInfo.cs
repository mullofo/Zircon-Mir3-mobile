using MirDB;
using static MirDB.Association;

namespace Library.SystemModels
{
    /// <summary>
    /// 道具信息
    /// </summary>
    [Lang]
    public class ItemInfo : DBObject
    {
        /// <summary>
        /// 道具名字
        /// </summary>
        [Lang("道具名")]
        [Export]
        public string ItemName
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName == value) return;

                var oldValue = _ItemName;
                _ItemName = value;

                OnChanged(oldValue, value, "ItemName");
            }
        }
        private string _ItemName;
        /// <summary>
        /// 道具类型
        /// </summary>
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
        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType WeaponType
        {
            get { return _WeaponType; }
            set
            {
                if (_WeaponType == value) return;

                var oldValue = _WeaponType;
                _WeaponType = value;

                OnChanged(oldValue, value, "WeaponType");
            }
        }
        private WeaponType _WeaponType;
        /// <summary>
        /// 道具能使用的职业
        /// </summary>
        public RequiredClass RequiredClass
        {
            get { return _RequiredClass; }
            set
            {
                if (_RequiredClass == value) return;

                var oldValue = _RequiredClass;
                _RequiredClass = value;

                OnChanged(oldValue, value, "RequiredClass");
            }
        }
        private RequiredClass _RequiredClass;
        /// <summary>
        /// 道具能使用的性别
        /// </summary>
        public RequiredGender RequiredGender
        {
            get { return _RequiredGender; }
            set
            {
                if (_RequiredGender == value) return;

                var oldValue = _RequiredGender;
                _RequiredGender = value;

                OnChanged(oldValue, value, "RequiredGender");
            }
        }
        private RequiredGender _RequiredGender;
        /// <summary>
        /// 道具所需的类型
        /// </summary>
        public RequiredType RequiredType
        {
            get { return _RequiredType; }
            set
            {
                if (_RequiredType == value) return;

                var oldValue = _RequiredType;
                _RequiredType = value;

                OnChanged(oldValue, value, "RequiredType");
            }
        }
        private RequiredType _RequiredType;
        /// <summary>
        /// 道具所需类型对应数值
        /// </summary>
        public int RequiredAmount
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
        private int _RequiredAmount;
        /// <summary>
        /// 道具特殊定义
        /// </summary>
        public int Shape
        {
            get { return _Shape; }
            set
            {
                if (_Shape == value) return;

                var oldValue = _Shape;
                _Shape = value;

                OnChanged(oldValue, value, "Shape");
            }
        }
        private int _Shape;
        /// <summary>
        /// 道具效果定义
        /// </summary>
        public ItemEffect Effect
        {
            get { return _Effect; }
            set
            {
                if (_Effect == value) return;

                var oldValue = _Effect;
                _Effect = value;

                OnChanged(oldValue, value, "Effect");
            }
        }
        private ItemEffect _Effect;
        /// <summary>
        /// 道具图像
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
        /// 道具持久
        /// </summary>
        public int Durability
        {
            get { return _Durability; }
            set
            {
                if (_Durability == value) return;

                var oldValue = _Durability;
                _Durability = value;

                OnChanged(oldValue, value, "Durability");
            }
        }
        private int _Durability;
        /// <summary>
        /// 道具价格
        /// </summary>
        public int Price
        {
            get { return _Price; }
            set
            {
                if (_Price == value) return;

                var oldValue = _Price;
                _Price = value;

                OnChanged(oldValue, value, "Price");
            }
        }
        private int _Price;
        /// <summary>
        /// 道具重量
        /// </summary>
        public int Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight == value) return;

                var oldValue = _Weight;
                _Weight = value;

                OnChanged(oldValue, value, "Weight");
            }
        }
        private int _Weight;
        /// <summary>
        /// 道具堆叠大小值
        /// </summary>
        public int StackSize
        {
            get { return _StackSize; }
            set
            {
                if (_StackSize == value) return;

                var oldValue = _StackSize;
                _StackSize = value;

                OnChanged(oldValue, value, "StackSize");
            }
        }
        private int _StackSize;
        /// <summary>
        /// 新手第一次进入给予的道具
        /// </summary>
        public bool StartItem
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
        private bool _StartItem;
        /// <summary>
        /// 道具出售的价格比率
        /// </summary>
        public decimal SellRate
        {
            get { return _SellRate; }
            set
            {
                if (_SellRate == value) return;

                var oldValue = _SellRate;
                _SellRate = value;

                OnChanged(oldValue, value, "SellRate");
            }
        }
        private decimal _SellRate;
        /// <summary>
        /// 是否能修理
        /// </summary>
        public bool CanRepair
        {
            get { return _CanRepair; }
            set
            {
                if (_CanRepair == value) return;

                var oldValue = _CanRepair;
                _CanRepair = value;

                OnChanged(oldValue, value, "CanRepair");
            }
        }
        private bool _CanRepair;
        /// <summary>
        /// 是否能出售
        /// </summary>
        public bool CanSell
        {
            get { return _CanSell; }
            set
            {
                if (_CanSell == value) return;

                var oldValue = _CanSell;
                _CanSell = value;

                OnChanged(oldValue, value, "CanSell");
            }
        }
        private bool _CanSell;
        /// <summary>
        /// 是否能存仓库
        /// </summary>
        public bool CanStore
        {
            get { return _CanStore; }
            set
            {
                if (_CanStore == value) return;

                var oldValue = _CanStore;
                _CanStore = value;

                OnChanged(oldValue, value, "CanStore");
            }
        }
        private bool _CanStore;
        /// <summary>
        /// 是否宝箱能开出
        /// </summary>
        public bool CanTreasure
        {
            get { return _CanTreasure; }
            set
            {
                if (_CanTreasure == value) return;

                var oldValue = _CanTreasure;
                _CanTreasure = value;

                OnChanged(oldValue, value, "CanTreasure");
            }
        }
        private bool _CanTreasure;
        /// <summary>
        /// 是否可以寄售
        /// </summary>
        public bool CanTrade
        {
            get { return _CanTrade; }
            set
            {
                if (_CanTrade == value) return;

                var oldValue = _CanTrade;
                _CanTrade = value;

                OnChanged(oldValue, value, "CanTrade");
            }
        }
        private bool _CanTrade;
        /// <summary>
        /// 永不爆出
        /// </summary>
        public bool NoMake
        {
            get { return _NoMake; }
            set
            {
                if (_NoMake == value) return;

                var oldValue = _NoMake;
                _NoMake = value;

                OnChanged(oldValue, value, "NoMake");
            }
        }
        private bool _NoMake;
        /// <summary>
        /// 是否能丢弃
        /// </summary>
        public bool CanDrop
        {
            get { return _CanDrop; }
            set
            {
                if (_CanDrop == value) return;

                var oldValue = _CanDrop;
                _CanDrop = value;

                OnChanged(oldValue, value, "CanDrop");
            }
        }
        private bool _CanDrop;
        /// <summary>
        /// 是否死亡爆出
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
        /// 物品说明
        /// </summary>
        [Lang("道具说明")]
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
        /// 物品分类普通高级稀世
        /// </summary>
        public Rarity Rarity
        {
            get { return _Rarity; }
            set
            {
                if (_Rarity == value) return;

                var oldValue = _Rarity;
                _Rarity = value;

                OnChanged(oldValue, value, "Rarity");
            }
        }
        private Rarity _Rarity;
        /// <summary>
        /// 是否能再自动喝药里使用
        /// </summary>
        public bool CanAutoPot
        {
            get { return _CanAutoPot; }
            set
            {
                if (_CanAutoPot == value) return;

                var oldValue = _CanAutoPot;
                _CanAutoPot = value;

                OnChanged(oldValue, value, "CanAutoPot");
            }
        }
        private bool _CanAutoPot;
        /// <summary>
        /// 道具BUFF对应图标
        /// </summary>
        public int BuffIcon
        {
            get { return _BuffIcon; }
            set
            {
                if (_BuffIcon == value) return;

                var oldValue = _BuffIcon;
                _BuffIcon = value;

                OnChanged(oldValue, value, "BuffIcon");
            }
        }
        private int _BuffIcon;
        /// <summary>
        /// 道具碎片数量
        /// </summary>
        public int PartCount
        {
            get { return _PartCount; }
            set
            {
                if (_PartCount == value) return;

                var oldValue = _PartCount;
                _PartCount = value;

                OnChanged(oldValue, value, "PartCount");
            }
        }
        private int _PartCount;
        /// <summary>
        /// 道具使用时间设置
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

        public Stats Stats = new Stats();

        [Association("ItemStats")]
        public DBBindingList<ItemInfoStat> ItemStats { get; set; }

        [Association("Drops", true)]
        public DBBindingList<DropInfo> Drops { get; set; }

        [Association("ItemDisplayEffects", true)]
        public DBBindingList<ItemDisplayEffect> ItemDisplayEffects { get; set; }

        /*
        [Association("GroupItemInfo")]
        public SetGroupItem SetGroupItem
        {
            get { return _SetGroupItem; }
            set
            {
                if (_SetGroupItem == value) return;

                var oldValue = _SetGroupItem;
                _SetGroupItem = value;

                OnChanged(oldValue, value, "SetGroupItem");
            }
        }
        private SetGroupItem _SetGroupItem;

        */

        [IgnoreProperty]
        public bool ShouldLinkInfo => StackSize > 1 || ItemType == ItemType.Consumable || ItemType == ItemType.Scroll;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            StackSize = 1;

            RequiredGender = RequiredGender.None;
            RequiredClass = RequiredClass.All;

            SellRate = 0.5M;

            CanRepair = true;
            CanSell = true;
            CanStore = true;
            CanTrade = true;
            CanDrop = true;
        }
        /// <summary>
        /// 读取时
        /// </summary>
        protected override internal void OnLoaded()
        {
            base.OnLoaded();
            StatsChanged();
        }
        /// <summary>
        /// 属性状态改变时
        /// </summary>
        public void StatsChanged()
        {
            Stats.Clear();
            foreach (ItemInfoStat stat in ItemStats)
                Stats[stat.Stat] += stat.Amount;
        }
        /// <summary>
        /// 道具名字字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ItemName;
        }
        /// <summary>
        /// 道具类型名字
        /// </summary>
        public string ItemTypeName { get; set; }
        /// <summary>
        /// 道具角色名字
        /// </summary>
        public string RequiredClassName { get; set; }
        /// <summary>
        /// 道具使用性别名字
        /// </summary>
        public string RequiredGenderName { get; set; }
        /// <summary>
        /// 道具所需类型名字
        /// </summary>
        public string RequiredTypeName { get; set; }
        /// <summary>
        /// 道具使用状态名字
        /// </summary>
        public string EffectName { get; set; }
    }
}
