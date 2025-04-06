using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserItemStat : DBObject  //角色道具属性状态
    {
        [Association("AddedStats")]
        public UserItem Item     //道具
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

        public string SourceName   //道具来源
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

        public Stat Stat       //属性状态
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

        public int Amount    //数量
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

        public StatSource StatSource   //属性状态统计
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

        protected override internal void OnDeleted()   //删除时
        {
            Item = null;

            base.OnDeleted();
        }

    }
}
