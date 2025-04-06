using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //好友
    {
        private void FriendListRefresh(bool isGameStart)
        {
            // 遍历当前角色好友列表
            foreach (FriendInfo friendInfo in Character.Friends)
            {
                PlayerObject target = SEnvir.GetCharacter(friendInfo.Name)?.Player;
                if (target != null && target != this)
                {
                    string stateChatLog = isGameStart ? $"你的好友{Character.CharacterName}已上线" : $"你的好友{Character.CharacterName}已离线";

                    target.Connection.ReceiveChat(stateChatLog, MessageType.System);
                    target.Enqueue(new S.FriendListRefresh { Friend = target.Character.Friends.Select(x => x.ToClientInfo()).ToList() });
                }
            }
        }

        public void FriendSwitch(bool allowFriend)
        {
            if (Character.Account.AllowFriend == allowFriend) return;

            Character.Account.AllowFriend = allowFriend;

            Enqueue(new S.FriendSwitch { Allow = Character.Account.AllowFriend });
        }

        public void FriendInvite(string Name)
        {

            if (Connection.Stage != GameStage.Game) return;

            // 判断自身或者被邀请者是否开启了添加好友的功能
            if (!Character.Account.AllowFriend)
            {
                Connection.ReceiveChat($"你未开启允许添加好友", MessageType.System);
                return;
            }

            if (Name == this.Name || SEnvir.GetPlayerByCharacter(Name).Character.Account == Character.Account)
            {
                Connection.ReceiveChat($"不能添加自己为好友", MessageType.System);
                return;
            }

            if (SEnvir.GetCharacter(Name) == null)
            {
                Connection.ReceiveChat($"无法找到玩家 {Name}", MessageType.System);
                return;
            }

            if (SEnvir.GetPlayerByCharacter(Name) == null)
            {
                Connection.ReceiveChat($"{Name} 不在线", MessageType.System);
                return;
            }

            if (!SEnvir.GetPlayerByCharacter(Name).Character.Account.AllowFriend)
            {
                Connection.ReceiveChat($"{Name} 拒绝添加好友", MessageType.System);
                return;
            }

            CharacterInfo targetCharacterInfo = SEnvir.GetCharacter(Name);
            // 被拉黑的情况 -> 主动拉黑方将不会接收好友邀请
            foreach (BlockInfo blockInfo in Character.Account.BlockedByList)
            {
                if (blockInfo.Account == targetCharacterInfo.Account)
                {
                    Connection.ReceiveChat($"{Name} 拒绝添加好友", MessageType.System);
                    return;
                }
            }

            foreach (BlockInfo blockInfo in Character.Account.BlockingList)
            {
                if (blockInfo.BlockedAccount == targetCharacterInfo.Account)
                {
                    Connection.ReceiveChat($"{Name} 已被拉黑，加好友前需先解除黑名单关系。", MessageType.System);
                    return;
                }
            }

            foreach (FriendInfo targetFriend in Character.Friends)
            {
                if (targetFriend.Name == Name)
                {
                    Connection.ReceiveChat($" {Name} 已经是你的好友了。", MessageType.System);
                    return;
                }
            }

            PlayerObject player = SEnvir.GetPlayerByCharacter(Name);

            if (player.FriendInvitation != null)
            {
                Connection.ReceiveChat(($"你已经向{Name}发起了好友请求，请勿重复发送"), MessageType.System);
                return;
            }
            else
            {
                // 向目标发送好友请求
                player.Enqueue(new S.FriendInvite { Name = Character.CharacterName });
                player.FriendInvitation = this;
            }
        }

        public void FriendAdd(string Name, string LinkID)
        {
            /*
            * 好友相关功能设计
            * 1.好友功能仅对角色绑定
            * 2.数据的存档关联用户账号的数据库资料信息
            * 3.添加黑名单时，检测当前好友列表是否存在同一个人，若存在，则删除好友选项
            */

            // 放到playerObject里实现这个功能

            IList<CharacterInfo> CharacterData = SEnvir.CharacterInfoList?.Binding;
            if (CharacterData != null && CharacterData.Count > 0)
            {
                foreach (FriendInfo targetFriend in Character.Friends)
                {
                    if (targetFriend.Name == Name)
                    {
                        Connection.ReceiveChat($"添加好友失败: {Name} 已经是您的好友了。", MessageType.System);
                        return;
                    }
                }

                // 存储数据
                FriendInfo friendInfo = SEnvir.FriendsList.CreateNewObject();
                friendInfo.Character = Character;
                friendInfo.Name = Name;
                friendInfo.AddDate = SEnvir.Now;
                friendInfo.EMailAddress = SEnvir.GetPlayerByCharacter(Name).Character.Account.EMailAddress;
                friendInfo.LinkID = LinkID;

                Character.Friends.Add(friendInfo);

                if (this != null)
                {
                    // 通知客户端刷新好友列表数据
                    Enqueue(new S.FriendNew
                    {
                        Friend = new ClientFriendInfo
                        {
                            Index = friendInfo.Index,
                            Character = friendInfo.Character.CharacterName,
                            Name = friendInfo.Name,
                            AddDate = friendInfo.AddDate,
                            LinkID = friendInfo.LinkID,
                            Online = true,
                        }
                    });
                }
            }
            else
            {
                // 提供异常处理
                throw new ArgumentException($"添加好友失败:当前角色数据为空，请检查原因。");
            }
        }

        public void FriendDelete(string LinkID, bool isRequester = false)
        {
            IList<CharacterInfo> CharacterData = SEnvir.CharacterInfoList?.Binding;
            if (CharacterData != null && CharacterData.Count > 0)
            {
                FriendInfo target = null;

                foreach (FriendInfo friendInfo in Character.Friends)
                {
                    if (friendInfo.LinkID == LinkID)
                    {
                        target = friendInfo;
                        break;
                    }
                }

                if (target == null)
                {
                    Connection.ReceiveChat("找不到相应好友", MessageType.System);
                    return;
                }

                // 删除数据
                // 删除发起者要删除的目标好友

                Character.Friends.Remove(target);

                ClientFriendInfo friendInfoC = new ClientFriendInfo();
                friendInfoC.Index = target.Index;
                friendInfoC.Character = this.Name;
                friendInfoC.Name = target.Name;
                friendInfoC.AddDate = target.AddDate;
                friendInfoC.LinkID = target.LinkID;
                friendInfoC.Online = SEnvir.GetCharacter(target.Name).Player != null;


                // 通知客户端刷新好友列表数据
                Enqueue(new S.FriendDelete
                {
                    Friend = friendInfoC,
                    isRequester = isRequester,
                });

                CharacterInfo targetPlayerInfo = SEnvir.GetCharacter(target.Name);
                PlayerObject targetPlayer = SEnvir.GetPlayerByCharacter(target.Name);

                // 目标也要删除相应好友

                target = null;

                foreach (FriendInfo friendInfo in targetPlayerInfo.Friends)
                {
                    if (friendInfo.LinkID == LinkID)
                    {
                        target = friendInfo;
                        break;
                    }
                }

                if (target == null)
                {
                    Connection.ReceiveChat("找不到相应好友", MessageType.System);
                    return;
                }

                friendInfoC = new ClientFriendInfo();
                friendInfoC.Index = target.Index;
                friendInfoC.Character = this.Name;
                friendInfoC.Name = target.Name;
                friendInfoC.AddDate = target.AddDate;
                friendInfoC.LinkID = target.LinkID;
                friendInfoC.Online = SEnvir.GetCharacter(target.Name).Player != null;

                targetPlayerInfo.Friends.Remove(target);

                if (targetPlayer != null)
                {
                    // 通知客户端刷新好友列表数据
                    targetPlayer.Enqueue(new S.FriendDelete
                    {
                        Friend = friendInfoC,
                        isRequester = false,
                    });
                }
            }
            else
            {
                // 提供异常处理
                throw new ArgumentException($"删除好友失败:当前角色数据为空，请检查原因。");
            }
        }
    }
}
