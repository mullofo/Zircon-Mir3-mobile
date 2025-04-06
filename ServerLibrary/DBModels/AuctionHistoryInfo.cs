using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 寄售历史记录
    /// </summary>
    [UserObject]
    public sealed class AuctionHistoryInfo : DBObject
    {
        /// <summary>
        /// 寄售信息
        /// </summary>
        public int Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private int _Info;
        /// <summary>
        /// 寄售销售额
        /// </summary>
        public long SaleCount
        {
            get { return _SaleCount; }
            set
            {
                if (_SaleCount == value) return;

                var oldValue = _SaleCount;
                _SaleCount = value;

                OnChanged(oldValue, value, "SaleCount");
            }
        }
        private long _SaleCount;
        /// <summary>
        /// 寄售最终价格
        /// </summary>
        public int LastPrice
        {
            get { return _LastPrice; }
            set
            {
                if (_LastPrice == value) return;

                var oldValue = _LastPrice;
                _LastPrice = value;

                OnChanged(oldValue, value, "LastPrice");
            }
        }
        private int _LastPrice;
        /// <summary>
        /// 寄售平均价格
        /// </summary>
        public int[] Average
        {
            get { return _Average; }
            set
            {
                if (_Average == value) return;

                var oldValue = _Average;
                _Average = value;

                OnChanged(oldValue, value, "Average");
            }
        }
        private int[] _Average;
        /// <summary>
        /// 寄售元宝平均价格
        /// </summary>
        public int[] GameGoldAverage
        {
            get { return _GameGoldAverage; }
            set
            {
                if (_GameGoldAverage == value) return;

                var oldValue = _GameGoldAverage;
                _GameGoldAverage = value;

                OnChanged(oldValue, value, "GameGoldAverage");
            }
        }
        private int[] _GameGoldAverage;
        /// <summary>
        /// 寄售元宝最终价格
        /// </summary>
        public int LastGameGoldPrice
        {
            get { return _LastGameGoldPrice; }
            set
            {
                if (_LastGameGoldPrice == value) return;

                var oldValue = _LastGameGoldPrice;
                _LastGameGoldPrice = value;

                OnChanged(oldValue, value, "LastGameGoldPrice");
            }
        }
        private int _LastGameGoldPrice;
        /// <summary>
        /// 寄售索引序号
        /// </summary>
        public int PartIndex
        {
            get { return _PartIndex; }
            set
            {
                if (_PartIndex == value) return;

                var oldValue = _PartIndex;
                _PartIndex = value;

                OnChanged(oldValue, value, "PartIndex");
            }
        }
        private int _PartIndex;

        /// <summary>
        /// 创建时
        /// </summary>
        protected override internal void OnCreated()
        {
            base.OnCreated();

            Average = new int[20];
            GameGoldAverage = new int[20];
        }
    }
}
