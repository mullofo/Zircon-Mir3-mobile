using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 商城信息
    /// </summary>
    public sealed class StoreInfo : DBObject
    {
        /// <summary>
        /// 商城道具信息
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
        /// 元宝购买价格
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
        /// 赏金购买价格
        /// </summary>
        public int HuntGoldPrice
        {
            get => _HuntGoldPrice;
            set
            {
                if (_HuntGoldPrice == value) return;

                int oldValue = _HuntGoldPrice;
                _HuntGoldPrice = value;

                OnChanged(oldValue, value, "HuntGoldPrice");
            }
        }
        private int _HuntGoldPrice;
        /// <summary>
        /// 商城道具搜索索引分类
        /// </summary>
        public string Filter
        {
            get { return _Filter; }
            set
            {
                if (_Filter == value) return;

                var oldValue = _Filter;
                _Filter = value;

                OnChanged(oldValue, value, "Filter");
            }
        }
        private string _Filter;
        /// <summary>
        /// 是否可以购买开关
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
        /// 使用时效限制
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
        /// 推荐位
        /// </summary>
        public int Recommend
        {
            get { return _Recommend; }
            set
            {
                if (_Recommend == value) return;

                var oldValue = _Recommend;
                _Recommend = value;

                OnChanged(oldValue, value, "Recommend");
            }
        }
        private int _Recommend;
    }

}
