using Library;
using Library.Network;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //组队
    {
        #region Group
        public void GroupSwitch(bool allowGroup)  //组队开关
        {
            if (Character.Account.AllowGroup == allowGroup) return;

            Character.Account.AllowGroup = allowGroup;

            Enqueue(new S.GroupSwitch { Allow = Character.Account.AllowGroup });

            if (GroupMembers != null)
                GroupLeave();
        }

        public void GroupRemove(string name)  //退出组队
        {
            if (GroupMembers == null)
            {
                Connection.ReceiveChat("Group.GroupNoGroup".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupNoGroup".Lang(con.Language), MessageType.System);
                return;
            }

            if (GroupMembers[0] != this)
            {
                Connection.ReceiveChat("Group.GroupNotLeader".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupNotLeader".Lang(con.Language), MessageType.System);
                return;
            }

            foreach (PlayerObject member in GroupMembers)
            {
                if (string.Compare(member.Name, name, StringComparison.OrdinalIgnoreCase) != 0) continue;

                member.GroupLeave();
                return;
            }

            Connection.ReceiveChat("Group.GroupMemberNotFound".Lang(Connection.Language, name), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("Group.GroupMemberNotFound".Lang(con.Language, name), MessageType.System);

        }
        public void GroupInvite(string name)  //组队邀请
        {
            if (GroupMembers != null && GroupMembers[0] != this)
            {
                Connection.ReceiveChat("Group.GroupNotLeader".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupNotLeader".Lang(con.Language), MessageType.System);
                return;
            }

            //修复组队输入同账号角色可直接组队的BUG
            var charter = SEnvir.GetCharacter(name);
            if (charter == null) return;
            PlayerObject player = charter.Player;// GetPlayerByCharacter(name);

            if (player == null)
            {
                Connection.ReceiveChat("System.CannotFindPlayer".Lang(Connection.Language, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("System.CannotFindPlayer".Lang(con.Language, name), MessageType.System);
                return;
            }

            if (player.GroupMembers != null)
            {
                Connection.ReceiveChat("Group.GroupAlreadyGrouped".Lang(Connection.Language, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupAlreadyGrouped".Lang(con.Language, name), MessageType.System);
                return;
            }

            if (player.GroupInvitation != null)
            {
                Connection.ReceiveChat("Group.GroupAlreadyInvited".Lang(Connection.Language, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupAlreadyInvited".Lang(con.Language, name), MessageType.System);
                return;
            }

            if (!player.Character.Account.AllowGroup || SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat("Group.GroupInviteNotAllowed".Lang(Connection.Language, name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupInviteNotAllowed".Lang(con.Language, name), MessageType.System);
                return;
            }

            if (player == this)
            {
                Connection.ReceiveChat("Group.GroupSelf".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupSelf".Lang(con.Language), MessageType.System);
                return;
            }

            player.GroupInvitation = this;
            player.Enqueue(new S.GroupInvite { Name = Name, ObserverPacket = false });
        }

        public void GroupJoin()  //加入队伍
        {
            if (GroupInvitation != null && GroupInvitation.Node == null) GroupInvitation = null;

            if (GroupInvitation == null || GroupMembers != null) return;


            if (GroupInvitation.GroupMembers == null)
            {
                GroupInvitation.GroupSwitch(true);
                GroupInvitation.GroupMembers = new List<PlayerObject> { GroupInvitation };
                GroupInvitation.Enqueue(new S.GroupMember { ObjectID = GroupInvitation.ObjectID, Name = GroupInvitation.Name }); //<-- 设置组长?
            }
            else if (GroupInvitation.GroupMembers[0] != GroupInvitation)
            {
                Connection.ReceiveChat("Group.GroupAlreadyGrouped".Lang(Connection.Language, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupAlreadyGrouped".Lang(con.Language, GroupInvitation.Name), MessageType.System);
                return;
            }
            else if (GroupInvitation.GroupMembers.Count >= Globals.GroupLimit)
            {
                Connection.ReceiveChat("Group.GroupMemberLimit".Lang(Connection.Language, GroupInvitation.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Group.GroupMemberLimit".Lang(con.Language, GroupInvitation.Name), MessageType.System);
                return;
            }

            GroupMembers = GroupInvitation.GroupMembers;
            GroupMembers.Add(this);

            foreach (PlayerObject ob in GroupMembers)
            {
                if (ob == this) continue;

                ob.Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
                Enqueue(new S.GroupMember { ObjectID = ob.ObjectID, Name = ob.Name });

                ob.AddAllObjects();
                ob.ApplyGroupBuff();
                ob.RefreshStats();
                if (Config.GroupOrGuild)
                {
                    ob.ApplyGuildBuff();
                }
            }

            AddAllObjects();
            ApplyGroupBuff();
            if (Config.GroupOrGuild)
            {
                ApplyGuildBuff();
            }

            RefreshStats();
            Enqueue(new S.GroupMember { ObjectID = ObjectID, Name = Name });
        }
        public void GroupLeave()  //离开队伍
        {
            Packet p = new S.GroupRemove { ObjectID = ObjectID };

            GroupMembers.Remove(this);
            List<PlayerObject> oldGroup = GroupMembers;
            GroupMembers = null;

            foreach (PlayerObject ob in oldGroup)
            {
                ob.Enqueue(p);
                ob.RemoveAllObjects();
                ob.RefreshStats();
                ob.ApplyGroupBuff();
                if (Config.GroupOrGuild)
                {
                    ob.ApplyGuildBuff();
                }
            }

            if (oldGroup.Count == 1) oldGroup[0].GroupLeave();

            GroupMembers = null;
            Enqueue(p);
            RemoveAllObjects();
            RefreshStats();
            ApplyGroupBuff();
            if (Config.GroupOrGuild)
            {
                ApplyGuildBuff();
            }
        }
        #endregion
    }
}
