using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 角色爆率
    /// </summary>
    [UserObject]
    public sealed class UserDrop : DBObject
    {
        /// <summary>
        /// 角色账号信息
        /// </summary>
        [Association("UserDrops")]
        public AccountInfo Account
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
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo Item
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
        private ItemInfo _Item;
        /// <summary>
        /// 获得该道具的进度
        /// </summary>
        public decimal Progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress == value) return;

                var oldValue = _Progress;
                _Progress = value;

                OnChanged(oldValue, value, "Progress");
            }
        }
        private decimal _Progress;
        /// <summary>
        /// 获得道具的总计数
        /// </summary>
        public long DropCount
        {
            get { return _DropCount; }
            set
            {
                if (_DropCount == value) return;

                var oldValue = _DropCount;
                _DropCount = value;

                OnChanged(oldValue, value, "DropCount");
            }
        }
        private long _DropCount;

        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Account = null;
            Item = null;

            base.OnDeleted();
        }

    }
}
