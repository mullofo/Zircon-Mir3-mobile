using MirDB;
using System;

namespace Server.DBModels
{

    [UserObject]
    public sealed class GuildWarInfo : DBObject    //行会战争信息
    {
        public GuildInfo Guild1    //行会1
        {
            get { return _Guild1; }
            set
            {
                if (_Guild1 == value) return;

                var oldValue = _Guild1;
                _Guild1 = value;

                OnChanged(oldValue, value, "Guild1");
            }
        }
        private GuildInfo _Guild1;

        public GuildInfo Guild2   //行会2
        {
            get { return _Guild2; }
            set
            {
                if (_Guild2 == value) return;

                var oldValue = _Guild2;
                _Guild2 = value;

                OnChanged(oldValue, value, "Guild2");
            }
        }
        private GuildInfo _Guild2;

        public TimeSpan Duration   //行会站持续时间
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
        private TimeSpan _Duration;
    }
}
