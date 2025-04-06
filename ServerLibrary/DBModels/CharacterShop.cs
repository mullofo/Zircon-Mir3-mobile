using Library;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    public sealed class CharacterShop : DBObject
    {
        [Association("CharacterShop")]
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

        public CharacterInfo Character   //角色
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

        public bool IsSell
        {
            get { return _IsSell; }
            set
            {
                if (_IsSell == value) return;

                var oldValue = _IsSell;
                _IsSell = value;

                OnChanged(oldValue, value, "IsSell");
            }

        }
        private bool _IsSell;

        [Association("CharacterShop")]
        public AccountInfo BuyAccount   //账号
        {
            get { return _Account; }
            set
            {
                if (_BuyAccount == value) return;

                var oldValue = _BuyAccount;
                _BuyAccount = value;

                OnChanged(oldValue, value, "BuyAccount");
            }
        }
        private AccountInfo _BuyAccount;

        public DateTime BuyDate   //挂单时间
        {
            get { return _BuyDate; }
            set
            {
                if (_BuyDate == value) return;

                var oldValue = _BuyDate;
                _BuyDate = value;

                OnChanged(oldValue, value, "BuyDate");
            }
        }
        private DateTime _BuyDate;

        protected override internal void OnCreated()   //创建时
        {
            base.OnCreated();
            Date = SEnvir.Now;
        }
    }
}
