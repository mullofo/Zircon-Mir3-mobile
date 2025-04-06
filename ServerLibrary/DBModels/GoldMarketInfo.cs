using Library;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    public sealed class GoldMarketInfo : DBObject
    {
        /// <summary>
        /// 道具持有角色信息
        /// </summary>
        [Association("GoldMarketInfos")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;
        public DateTime Date   //挂单时间
        {
            get { return _Date; }
            set
            {
                if (_Date == value) return;

                var oldValue = _Date;
                _Date = value;

                OnChanged(oldValue, value, "Date");
            }
        }
        private DateTime _Date;

        public StockOrderType TradeState   //成交状态
        {
            get { return _TradeState; }
            set
            {
                if (_TradeState == value) return;

                var oldValue = _TradeState;
                _TradeState = value;

                OnChanged(oldValue, value, "TradeState");
            }
        }
        private StockOrderType _TradeState;

        public TradeType TradeType //交易类型
        {
            get { return _TradeType; }
            set
            {
                if (_TradeType == value) return;
                var oldValue = _TradeType;
                _TradeType = value;
                OnChanged(oldValue, value, "TradeType");
            }
        }
        private TradeType _TradeType;

        public long GoldCount
        {
            get { return _GoldCount; }
            set
            {
                if (_GoldCount == value) return;
                var oldValue = _GoldCount;
                _GoldCount = value;
                OnChanged(oldValue, value, "GoldCount");
            }
        }
        private long _GoldCount;

        public long GameGoldPrice
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
        private long _GameGoldPrice;
        public long CompletedCount
        {
            get { return _CompletedCount; }
            set
            {
                if (_CompletedCount == value) return;
                var oldValue = _CompletedCount;
                _CompletedCount = value;
                OnChanged(oldValue, value, "CompletedCount");
            }
        }
        private long _CompletedCount;
        protected override internal void OnCreated()   //创建时
        {
            base.OnCreated();
            Date = SEnvir.Now;
        }
        protected override void OnChanged(object oldValue, object newValue, string propertyName)
        {
            base.OnChanged(oldValue, newValue, propertyName);
            if (propertyName == "CompletedCount")
            {
                if (_CompletedCount == GoldCount) TradeState = StockOrderType.Completed;
            }
        }

#if !ServerTool
        public ClientGoldMarketMyOrderInfo ToClientInfo()
        {
            return new ClientGoldMarketMyOrderInfo
            {
                Index = Index,
                GoldPrice = GameGoldPrice,
                Count = GoldCount,
                TradeType = TradeType,
                TradeState = TradeState,
                CompletedCount = CompletedCount,
                Date = Date,
            };
        }
#endif
    }
}
