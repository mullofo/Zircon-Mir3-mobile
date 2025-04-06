using Library;
using MirDB;

namespace Server.DBModels
{
    /// <summary>
    /// 黑名单信息
    /// </summary>
    [UserObject]
    public sealed class BlockInfo : DBObject
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Association("BlockingList")]
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
        /// 黑名单账号
        /// </summary>
        [Association("BlockedByList")]
        public AccountInfo BlockedAccount
        {
            get { return _BlockedAccount; }
            set
            {
                if (_BlockedAccount == value) return;

                var oldValue = _BlockedAccount;
                _BlockedAccount = value;

                OnChanged(oldValue, value, "BlockedAccount");
            }
        }
        private AccountInfo _BlockedAccount;
        /// <summary>
        /// 黑名单名字
        /// </summary>
        public string BlockedName
        {
            get { return _BlockedName; }
            set
            {
                if (_BlockedName == value) return;

                var oldValue = _BlockedName;
                _BlockedName = value;

                OnChanged(oldValue, value, "BlockedName");
            }
        }
        private string _BlockedName;
        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Account = null;
            BlockedAccount = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientBlockInfo ToClientInfo()
        {
            return new ClientBlockInfo   //用户锁定列表
            {
                Index = Index,
                Name = BlockedName
            };
        }
#endif
    }
}
