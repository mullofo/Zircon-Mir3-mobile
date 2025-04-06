using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 道具属性信息
    /// </summary>
    public sealed class ItemInfoStat : DBObject
    {
        /// <summary>
        /// 道具信息
        /// </summary>
        [Association("ItemStats")]
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
        /// 对应的属性
        /// </summary>
        public Stat Stat
        {
            get { return _Stat; }
            set
            {
                if (_Stat == value) return;

                var oldValue = _Stat;
                _Stat = value;

                OnChanged(oldValue, value, "Stat");
            }
        }
        private Stat _Stat;

        /// <summary>
        /// 属性参数值
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
        /// 是否隐藏属性
        /// </summary>
        public bool ShowHidden
        {
            get { return _ShowHidden; }
            set
            {
                if (_ShowHidden == value) return;

                var oldValue = _ShowHidden;
                _ShowHidden = value;

                OnChanged(oldValue, value, "ShowHidden");
            }
        }
        private bool _ShowHidden;
    }
}
