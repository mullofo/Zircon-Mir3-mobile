using MirDB;

namespace Library.SystemModels
{
    /// <summary>
    /// 完整的物品属性
    /// </summary>
    public sealed class FullItemStat : DBObject
    {
        /// <summary>
        /// 源名称
        /// </summary>
        [Association("AddedStats")]
        public string SourceName
        {
            get { return _SourceName; }
            set
            {
                if (_SourceName == value) return;

                var oldValue = _SourceName;
                _SourceName = value;

                OnChanged(oldValue, value, "SourceName");
            }
        }
        private string _SourceName;
        /// <summary>
        /// 属性
        /// </summary>
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
        /// <summary>
        /// 属性数值
        /// </summary>
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
        /// <summary>
        /// 属性源
        /// </summary>
        public StatSource StatSource
        {
            get { return _StatSource; }
            set
            {
                if (_StatSource == value) return;

                var oldValue = _StatSource;
                _StatSource = value;

                OnChanged(oldValue, value, "StatSource");
            }
        }
        private StatSource _StatSource;
    }
}
