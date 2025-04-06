using Library.SystemModels;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class UserCompanionUnlock : DBObject  //角色宠物解锁
    {
        [Association("CompanionUnlocks")]
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

        public CompanionInfo CompanionInfo  //宠物信息
        {
            get { return _CompanionInfo; }
            set
            {
                if (_CompanionInfo == value) return;

                var oldValue = _CompanionInfo;
                _CompanionInfo = value;

                OnChanged(oldValue, value, "CompanionInfo");
            }
        }
        private CompanionInfo _CompanionInfo;

        protected override internal void OnDeleted()   //删除时
        {
            Account = null;
            CompanionInfo = null;

            base.OnDeleted();
        }

    }
}
