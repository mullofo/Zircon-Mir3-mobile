using Library.SystemModels;
using MirDB;
using System;

namespace Server.DBModels
{
    /// <summary>
    /// 参与攻城的信息
    /// </summary>
    [UserObject]
    public sealed class UserConquest : DBObject
    {
        /// <summary>
        /// 参与攻城的行会信息
        /// </summary>
        [Association("Conquest")]
        public GuildInfo Guild
        {
            get { return _Guild; }
            set
            {
                if (_Guild == value) return;

                var oldValue = _Guild;
                _Guild = value;

                //OnChanged(oldValue, value, "Guild");
            }
        }
        private GuildInfo _Guild;
        /// <summary>
        /// 攻城的城堡信息
        /// </summary>
        public CastleInfo Castle
        {
            get { return _Castle; }
            set
            {
                if (_Castle == value) return;

                var oldValue = _Castle;
                _Castle = value;

                OnChanged(oldValue, value, "Castle");
            }
        }
        private CastleInfo _Castle;
        /// <summary>
        /// 攻城开启的时间
        /// </summary>
        public DateTime WarDate
        {
            get { return _WarDate; }
            set
            {
                if (_WarDate == value) return;

                var oldValue = _WarDate;
                _WarDate = value;

                OnChanged(oldValue, value, "WarDate");
            }
        }
        private DateTime _WarDate;


        protected override internal void OnDeleted()   //删除时
        {
            Guild = null;
            Castle = null;

            base.OnDeleted();
        }

    }
}
