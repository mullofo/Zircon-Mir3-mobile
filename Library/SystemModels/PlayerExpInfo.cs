using MirDB;

namespace Library.SystemModels
{

    public sealed class PlayerExpInfo : DBObject
    {
        [Association("PlayerExp")]
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;

        public decimal Exp
        {
            get { return _Exp; }
            set
            {
                if (_Exp == value) return;

                var oldValue = _Exp;
                _Exp = value;

                OnChanged(oldValue, value, "Exp");
            }
        }
        private decimal _Exp;

    }
}

