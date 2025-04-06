using MirDB;

namespace Library.SystemModels
{
    public sealed class GuildLevelExp : DBObject
    {
        /// <summary>
        /// 行会等级
        /// </summary>
        [Association("GuildLevelExp")]
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
        /// <summary>
        /// 行会经验
        /// </summary>
        public int Exp
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
        private int _Exp;


        /// <summary>
        /// 行会升级需要的活跃度
        /// </summary>
        public long ActivCount
        {
            get { return _ActivCount; }
            set
            {
                if (_ActivCount == value) return;

                var oldValue = _ActivCount;
                _ActivCount = value;

                OnChanged(oldValue, value, "ActivCount");
            }
        }
        private long _ActivCount;
        /// <summary>
        /// 行会升级需要的资金资金
        /// </summary>
        public long GuildFunds
        {
            get { return _GuildFunds; }
            set
            {
                if (_GuildFunds == value) return;

                var oldValue = _GuildFunds;
                _GuildFunds = value;

                OnChanged(oldValue, value, "GuildFunds");
            }
        }
        private long _GuildFunds;
        /// <summary>
        /// 行会总捐款额
        /// </summary>
        public long TotalContribution
        {
            get { return _TotalContribution; }
            set
            {
                if (_TotalContribution == value) return;

                var oldValue = _TotalContribution;
                _TotalContribution = value;

                OnChanged(oldValue, value, "TotalContribution");
            }
        }
        private long _TotalContribution;
    }
}