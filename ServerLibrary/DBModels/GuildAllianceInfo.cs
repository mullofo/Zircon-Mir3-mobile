using Library;
using MirDB;

namespace Server.DBModels
{
    [UserObject]
    public sealed class GuildAllianceInfo : DBObject
    {
        public GuildInfo Guild1
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

        public GuildInfo Guild2
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

#if !ServerTool
        public ClientGuildAllianceInfo ToClientInfo(GuildInfo guild)
        {
            int count = 0;
            foreach (GuildMemberInfo member in Guild1 == guild ? Guild2.Members : Guild1.Members)
                if (member.Account.Connection?.Player != null)
                    count++;

            return new ClientGuildAllianceInfo
            {
                Index = Index,
                Name = Guild1 == guild ? Guild2.GuildName : Guild1.GuildName,
                OnlineCount = count,
            };
        }
#endif

        protected override internal void OnDeleted()
        {
            Guild1 = null;
            Guild2 = null;

            base.OnDeleted();
        }
    }
}
