using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 爆率信息
    /// </summary>
    public sealed class DropInfo : DBObject
    {
        /// <summary>
        /// 怪物信息
        /// </summary>
        [Association("Drops")]
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
        /// 道具信息
        /// </summary>
        [Association("Drops")]
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
        /// 爆率几率
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
        /// 爆率数值
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
        /// 爆率分组
        /// </summary>
        public int DropGroup
        {
            get { return _DropGroup; }
            set
            {
                if (_DropGroup == value) return;

                var oldValue = _DropGroup;
                _DropGroup = value;

                OnChanged(oldValue, value, "DropGroup");
            }
        }
        private int _DropGroup;
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
        /// 是否只爆碎片
        /// </summary>
        public bool PartOnly
        {
            get { return _PartOnly; }
            set
            {
                if (_PartOnly == value) return;

                var oldValue = _PartOnly;
                _PartOnly = value;

                OnChanged(oldValue, value, "PartOnly");
            }
        }
        private bool _PartOnly;
        /// <summary>
        /// 是否复活节活动
        /// </summary>
        public bool EasterEvent
        {
            get { return _EasterEvent; }
            set
            {
                if (_EasterEvent == value) return;

                var oldValue = _EasterEvent;
                _EasterEvent = value;

                OnChanged(oldValue, value, "EasterEvent");
            }
        }
        private bool _EasterEvent;
        /// <summary>
        /// 输出怪物名字
        /// </summary>
        public string StrMonsterName { get; set; }
        /// <summary>
        /// 输出道具名字
        /// </summary>
        public string StrItemName { get; set; }

        public long AmountTemp { get; set; }

        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            Amount = 1;
        }
    }
}
