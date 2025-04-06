using Library;
using MirDB;
using Server.Envir;
using System;
using System.Linq;

namespace Server.DBModels
{
    /// <summary>
    /// 行会成员信息
    /// </summary>
    [UserObject]
    public sealed class GuildMemberInfo : DBObject
    {
        /// <summary>
        /// 行会信息
        /// </summary>
        [Association("Members")]
        public GuildInfo Guild
        {
            get { return _Guild; }
            set
            {
                if (_Guild == value) return;

                var oldValue = _Guild;
                _Guild = value;

                OnChanged(oldValue, value, "Guild");
            }
        }
        private GuildInfo _Guild;
        /// <summary>
        /// 行会称号
        /// </summary>
        public string Rank
        {
            get { return _Rank; }
            set
            {
                if (_Rank == value) return;

                var oldValue = _Rank;
                _Rank = value;

                OnChanged(oldValue, value, "Rank");
            }
        }
        private string _Rank;
        /// <summary>
        /// 账号信息
        /// </summary>
        [Association("Member")]
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
        /// 加入日期
        /// </summary>
        public DateTime JoinDate
        {
            get { return _JoinDate; }
            set
            {
                if (_JoinDate == value) return;

                var oldValue = _JoinDate;
                _JoinDate = value;

                OnChanged(oldValue, value, "JoinDate");
            }
        }
        private DateTime _JoinDate;
        /// <summary>
        /// 捐款金额
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

        /// <summary>
        /// 活跃度
        /// </summary>
        public long ActiveCount
        {
            get { return _ActiveCount; }
            set
            {
                if (_ActiveCount == value) return;

                var oldValue = _ActiveCount;
                _ActiveCount = value;

                OnChanged(oldValue, value, "ActiveCount");
            }
        }
        private long _ActiveCount;
        /// <summary>
        /// 每天捐款额
        /// </summary>
        public long DailyContribution
        {
            get { return _DailyContribution; }
            set
            {
                if (_DailyContribution == value) return;

                var oldValue = _DailyContribution;
                _DailyContribution = value;

                OnChanged(oldValue, value, "DailyContribution");
            }
        }
        private long _DailyContribution;

        /// <summary>
        /// 个人捐献赞助币总数
        /// </summary>
        public long GameGoldTotal
        {
            get { return _GameGoldTotal; }
            set
            {
                if (_GameGoldTotal == value) return;
                var oldValue = _GameGoldTotal;
                _GameGoldTotal = value;
                OnChanged(oldValue, value, "GameGoldTotal");
            }

        }

        private long _GameGoldTotal;
        /// <summary>
        /// 允许使用的行会管理权限
        /// </summary>
        public GuildPermission Permission
        {
            get { return _Permission; }
            set
            {
                if (_Permission == value) return;

                var oldValue = _Permission;
                _Permission = value;

                OnChanged(oldValue, value, "Permission");
            }
        }
        private GuildPermission _Permission;
        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Account = null;
            Guild = null;

            base.OnDeleted();
        }

#if !ServerTool
        /// <summary>
        /// 更新到客户端信息
        /// </summary>
        /// <returns></returns>
        public ClientGuildMemberInfo ToClientInfo()
        {
            ClientGuildMemberInfo info = new ClientGuildMemberInfo
            {
                Index = Index,
                PlayerIndex = Account.LastCharacter.Index,
                Name = Account.LastCharacter.CharacterName,
                Rank = Rank,
                DailyContribution = DailyContribution,
                TotalContribution = TotalContribution,
                Permission = Permission,
            };

            if (Account.Connection?.Player != null)
            {
                info.Online = TimeSpan.MinValue;
                info.ObjectID = Account.Connection.Player.ObjectID;
            }
            else
            {
                if (Account.Characters.Count > 0)
                    info.Online = SEnvir.Now - Account.Characters.Max(x => x.LastLogin);
            }

            return info;
        }
#endif
    }
}
