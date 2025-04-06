using MirDB;

namespace Library.SystemModels
{
    public sealed class CraftItemInfo : DBObject
    {
        [Association("CraftItem")]
        public ItemInfo Item
        {
            get { return _Item; }
            set
            {
                if (_Item == value) return;

                var oldValue = _Item;
                _Item = value;

                OnChanged(oldValue, value, "Item");
                if (value != null)
                {
                    TargetItemType = Item.ItemType;
                }
            }
        }
        private ItemInfo _Item; //目标物品

        public int Blueprint
        {
            get => _Blueprint;
            set
            {
                if (_Blueprint == value) return;

                var oldValue = _Blueprint;
                _Blueprint = value;

                OnChanged(oldValue, value, "Blueprint");
            }
        }
        private int _Blueprint;//合成方案编号

        public ItemType TargetItemType  //制造类型
        {
            get { return _TargetItemType; }
            set
            {
                if (_TargetItemType == value) return;

                var oldValue = _TargetItemType;
                _TargetItemType = value;

                OnChanged(oldValue, value, "TargetItemType");
            }
        }
        private ItemType _TargetItemType;

        public int SortNumber
        {
            get { return _SortNumber; }
            set
            {
                if (_SortNumber == value) return;

                var oldValue = _SortNumber;
                _SortNumber = value;

                OnChanged(oldValue, value, "SortNumber");
            }
        }
        private int _SortNumber; //排序号

        /*public Rarity TargetRarity
        {
            get => _TargetRarity;
            set
            {
                if (_TargetRarity == value) return;

                var oldValue = _TargetRarity;
                _TargetRarity = value;

                OnChanged(oldValue, value, "TargetRarity");
            }
        }
        private Rarity _TargetRarity;*/  //目标品质

        public int TargetAmount
        {
            get { return _TargetAmount; }
            set
            {
                if (_TargetAmount == value) return;

                var oldValue = _TargetAmount;
                _TargetAmount = value;

                OnChanged(oldValue, value, "TargetAmount");
            }
        }
        private int _TargetAmount; //制作数量

        public int SuccessRate
        {
            get { return _SuccessRate; }
            set
            {
                if (_SuccessRate == value) return;

                var oldValue = _SuccessRate;
                _SuccessRate = value;

                OnChanged(oldValue, value, "SuccessRate");
            }
        }
        private int _SuccessRate; //制造成功率

        public int GainExp
        {
            get { return _GainExp; }
            set
            {
                if (_GainExp == value) return;

                var oldValue = _GainExp;
                _GainExp = value;

                OnChanged(oldValue, value, "GainExp");
            }
        }
        private int _GainExp; //获得熟练度

        public int TimeCost
        {
            get { return _TimeCost; }
            set
            {
                if (_TimeCost == value) return;

                var oldValue = _TimeCost;
                _TimeCost = value;

                OnChanged(oldValue, value, "TimeCost");
            }
        }
        private int _TimeCost; //消耗时间

        public int GoldCost
        {
            get { return _GoldCost; }
            set
            {
                if (_GoldCost == value) return;

                var oldValue = _GoldCost;
                _GoldCost = value;

                OnChanged(oldValue, value, "GoldCost");
            }
        }
        private int _GoldCost; //消耗金币

        public int LevelNeeded
        {
            get { return _LevelNeeded; }
            set
            {
                if (_LevelNeeded == value) return;

                var oldValue = _LevelNeeded;
                _LevelNeeded = value;

                OnChanged(oldValue, value, "LevelNeeded");
            }
        }
        private int _LevelNeeded; //打造技能等级要求 （0-10）

        [Association("CraftItem")]
        public ItemInfo Item1
        {
            get { return _Item1; }
            set
            {
                if (_Item1 == value) return;

                var oldValue = _Item1;
                _Item1 = value;

                OnChanged(oldValue, value, "Item1");
            }
        }
        private ItemInfo _Item1; //原料1

        public int Amount1
        {
            get { return _Amount1; }
            set
            {
                if (_Amount1 == value) return;

                var oldValue = _Amount1;
                _Amount1 = value;

                OnChanged(oldValue, value, "Amount1");
            }
        }
        private int _Amount1; //需要的原料数量

        [Association("CraftItem")]
        public ItemInfo Item2
        {
            get { return _Item2; }
            set
            {
                if (_Item2 == value) return;

                var oldValue = _Item2;
                _Item2 = value;

                OnChanged(oldValue, value, "Item2");
            }
        }
        private ItemInfo _Item2; //原料2

        public int Amount2
        {
            get { return _Amount2; }
            set
            {
                if (_Amount2 == value) return;

                var oldValue = _Amount2;
                _Amount2 = value;

                OnChanged(oldValue, value, "Amount2");
            }
        }
        private int _Amount2; //需要的原料数量2

        [Association("CraftItem")]
        public ItemInfo Item3
        {
            get { return _Item3; }
            set
            {
                if (_Item3 == value) return;

                var oldValue = _Item3;
                _Item3 = value;

                OnChanged(oldValue, value, "Item3");
            }
        }
        private ItemInfo _Item3; //原料3

        public int Amount3
        {
            get { return _Amount3; }
            set
            {
                if (_Amount3 == value) return;

                var oldValue = _Amount3;
                _Amount3 = value;

                OnChanged(oldValue, value, "Amount3");
            }
        }
        private int _Amount3; //需要的原料数量3

        [Association("CraftItem")]
        public ItemInfo Item4
        {
            get { return _Item4; }
            set
            {
                if (_Item4 == value) return;

                var oldValue = _Item4;
                _Item4 = value;

                OnChanged(oldValue, value, "Item4");
            }
        }
        private ItemInfo _Item4; //原料4

        public int Amount4
        {
            get { return _Amount4; }
            set
            {
                if (_Amount4 == value) return;

                var oldValue = _Amount4;
                _Amount4 = value;

                OnChanged(oldValue, value, "Amount4");
            }
        }
        private int _Amount4; //需要的原料数量4

        [Association("CraftItem")]
        public ItemInfo Item5
        {
            get { return _Item5; }
            set
            {
                if (_Item5 == value) return;

                var oldValue = _Item5;
                _Item5 = value;

                OnChanged(oldValue, value, "Item5");
            }
        }
        private ItemInfo _Item5; //原料5

        public int Amount5
        {
            get { return _Amount5; }
            set
            {
                if (_Amount5 == value) return;

                var oldValue = _Amount5;
                _Amount5 = value;

                OnChanged(oldValue, value, "Amount5");
            }
        }
        private int _Amount5; //需要的原料数量5

        // 返回所有原料:数量的dict
        /*public IDictionary<ItemInfo, int> GetIngredients()
        {
            Dictionary<ItemInfo, int> res= new Dictionary<ItemInfo, int>()
            {
                {Item1, Amount1 },
                {Item2, Amount2 },
                {Item3, Amount3 },
                {Item4, Amount4 },
                {Item5, Amount5 },
            };
            return res;
        }*/

        protected override internal void OnCreated()
        {
            base.OnCreated();
            TargetAmount = 1;
            Blueprint = 1;
            TimeCost = 0;
            LevelNeeded = 1;
            Amount1 = 0;
            Amount2 = 0;
            Amount3 = 0;
            Amount4 = 0;
            Amount5 = 0;
        }
    }
}

