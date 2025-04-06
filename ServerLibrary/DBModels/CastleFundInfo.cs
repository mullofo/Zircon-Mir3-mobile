using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public class CastleFundInfo : DBObject
    {
        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                var oldValue = _Castle;
                _Castle = value;

                OnChanged(oldValue, value, "Castle");
            }
        }
        private CastleInfo _Castle;

        public long TotalTax
        {
            get { return _TotalTax; }
            set
            {
                if (_TotalTax == value) return;

                long oldValue = _TotalTax;
                _TotalTax = value;

                OnChanged(oldValue, value, "TotalTax");
            }
        }
        private long _TotalTax;

        public long TotalDeposit
        {
            get { return _TotalDeposit; }
            set
            {
                if (_TotalDeposit == value) return;

                long oldValue = _TotalDeposit;
                _TotalDeposit = value;

                OnChanged(oldValue, value, "TotalDeposit");
            }
        }
        private long _TotalDeposit;

        [IgnoreProperty]
        public long TotalFund => TotalTax + TotalDeposit;

        protected override internal void OnCreated()
        {
            base.OnCreated();
            Castle = null;
            TotalTax = 0;
            TotalDeposit = 0;
        }

        protected override internal void OnDeleted()
        {
            Castle = null;
            TotalTax = 0;
            TotalDeposit = 0;
            base.OnDeleted();
        }
    }
}