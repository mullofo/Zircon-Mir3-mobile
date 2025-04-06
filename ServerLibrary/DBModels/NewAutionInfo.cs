using Library;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    public sealed class NewAutionInfo : DBObject
    {
        [Association("NewAuctions")]
        public AccountInfo Account   //账号
        {
            get { return _Account; }
            set
            {
                if (_Account == value) return;

                var oldValue = _Account;
                _Account = value;

                OnChanged(oldValue, value, "Account");
            }
        }
        private AccountInfo _Account;
        [Association("NewAuctions")]
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

        [Association("NewAuctions")]
        public UserItem Item   //道具
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
        private UserItem _Item;

        public long Price   //价格
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
        private long _Price;
        public long LastPrice   //最后出价
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
        private long _LastPrice;

        //[Association("NewAuctions")]
        public CharacterInfo LastCharacter
        {
            get { return _LastCharacter; }
            set
            {
                if (_LastCharacter == value) return;

                var oldValue = _LastCharacter;
                _LastCharacter = value;

                OnChanged(oldValue, value, "LastCharacter");
            }
        }
        private CharacterInfo _LastCharacter;
        public long BuyItNowPrice   //最后出价
        {
            get { return _BuyItNowPrice; }
            set
            {
                if (_BuyItNowPrice == value) return;

                var oldValue = _BuyItNowPrice;
                _BuyItNowPrice = value;

                OnChanged(oldValue, value, "BuyItNowPrice");
            }
        }
        private long _BuyItNowPrice;
        public long PriceAdd   //最后出价
        {
            get { return _PriceAdd; }
            set
            {
                if (_PriceAdd == value) return;

                var oldValue = _PriceAdd;
                _PriceAdd = value;

                OnChanged(oldValue, value, "PriceAdd");
            }
        }
        private long _PriceAdd;

        public CurrencyType PriceType   //价格类型
        {
            get { return _PriceType; }
            set
            {
                if (_PriceType == value) return;

                var oldValue = _PriceType;
                _PriceType = value;

                OnChanged(oldValue, value, "PriceType");
            }
        }

        private CurrencyType _PriceType;
        public bool Closed
        {
            get { return _Closed; }
            set
            {
                if (_Closed == value) return;
                var oldValue = _Closed;
                _Closed = value;
                OnChanged(oldValue, value, "Closed");
            }
        }
        private bool _Closed;

        protected override internal void OnCreated()   //创建时
        {
            base.OnCreated();
            Date = SEnvir.Now;
        }
#if !ServerTool
        public ClientNewAuction ToClientInfo()    //更新到客户端信息
        {
            return new ClientNewAuction
            {
                Index = Index,
                Price = Price,
                PriceAdd = PriceAdd,
                BuyItNowPrice = BuyItNowPrice,
                LastPrice = LastPrice,
                Item = Item.ToClientInfo(),
                OwnName = Character.ToString(),
                LastName = LastCharacter?.ToString() ?? "",
                Closed = Closed,
            };
        }
#endif
    }

}
