﻿using MirDB;

namespace Library.SystemModels
{
    public sealed class CustomBuffInfoStat : DBObject
    {
        [Association("BuffStats")]
        public CustomBuffInfo CustomBuff
        {
            get { return _CustomBuff; }
            set
            {
                if (_CustomBuff == value) return;

                var oldValue = _CustomBuff;
                _CustomBuff = value;

                OnChanged(oldValue, value, "CustomBuff");
            }
        }
        private CustomBuffInfo _CustomBuff;
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
    }
}
