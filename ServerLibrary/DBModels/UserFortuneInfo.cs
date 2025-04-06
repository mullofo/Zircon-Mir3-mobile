using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    public class UserFortuneInfo : DBObject   //角色财富信息
    {
        [Association("Fortunes")]
        public AccountInfo Account   //账户
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

        public ItemInfo Item   //道具
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


        public long DropCount   //爆率计数
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

        public decimal DropProgress   //删除爆率进度
        {
            get { return _DropProgress; }
            set
            {
                if (_DropProgress == value) return;

                var oldValue = _DropProgress;
                _DropProgress = value;

                OnChanged(oldValue, value, "DropProgress");
            }
        }
        private decimal _DropProgress;

        public DateTime CheckTime    //检查时间
        {
            get { return _CheckTime; }
            set
            {
                if (_CheckTime == value) return;

                var oldValue = _CheckTime;
                _CheckTime = value;

                OnChanged(oldValue, value, "CheckTime");
            }
        }
        private DateTime _CheckTime;

#if !ServerTool
        public ClientFortuneInfo ToClientInfo()   //更新到客户端信息
        {
            return new ClientFortuneInfo
            {
                ItemIndex = Item.Index,
                CheckTime = SEnvir.Now - CheckTime,
                DropCount = DropCount,
                Progress = DropProgress,
            };
        }
#endif
    }
}
