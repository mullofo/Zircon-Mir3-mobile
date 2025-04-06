using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 宠物信息
    /// </summary>
    public class CompanionInfo : DBObject
    {
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo MonsterInfo
        {
            get { return _MonsterInfo; }
            set
            {
                if (_MonsterInfo == value) return;

                var oldValue = _MonsterInfo;
                _MonsterInfo = value;

                OnChanged(oldValue, value, "MonsterInfo");
            }
        }
        private MonsterInfo _MonsterInfo;
        /// <summary>
        /// 金币价格
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
        /// 元宝价格
        /// </summary>
        public int GameGoldPrice
        {
            get { return _GameGoldPrice; }
            set
            {
                if (_GameGoldPrice == value) return;

                var oldValue = _GameGoldPrice;
                _GameGoldPrice = value;

                OnChanged(oldValue, value, "GameGoldPrice");
            }
        }
        private int _GameGoldPrice;
        /// <summary>
        /// 是否直接购买
        /// </summary>
        public bool Available
        {
            get { return _Available; }
            set
            {
                if (_Available == value) return;

                var oldValue = _Available;
                _Available = value;

                OnChanged(oldValue, value, "Available");
            }
        }
        private bool _Available;
        /// <summary>
        /// 是否带捡取过滤
        /// </summary>
        public bool Sorting
        {
            get { return _Sorting; }
            set
            {
                if (_Sorting == value) return;

                var oldValue = _Sorting;
                _Sorting = value;

                OnChanged(oldValue, value, "Sorting");
            }
        }
        private bool _Sorting;
    }
}
