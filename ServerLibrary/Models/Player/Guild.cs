using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.SystemModels;
using Microsoft.Scripting.Hosting;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 行会模块
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        #region Guild
        /// <summary>
        /// 创建行会
        /// </summary>
        /// <param name="p"></param>
        public void GuildCreate(C.GuildCreate p)
        {
            Enqueue(new S.GuildCreate { ObserverPacket = false });

            if (Character.Account.GuildMember != null) return;  //已经有行会的跳出

            if (p.Members < 0 || p.Members > 60) return;  //行会成员小于0或者大于60 跳出
            if (p.Storage < 0 || p.Storage > 100) return;  //行会仓库小于0或者大于100 跳出

            long cost = p.Members * Config.GuildMemberCost + p.Storage * Config.GuildStorageCost;

            if (p.UseGold)
            {
                cost += Config.GuildCreationCost;
            }
            else
            {
                bool result = false;
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.UmaKingHorn) continue;

                    result = true;
                    break;
                }

                if (!result)
                {
                    Connection.ReceiveChat("Guild.GuildNeedHorn".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            if (cost > Gold)
            {
                Connection.ReceiveChat("Guild.GuildNeedGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (Config.CanUseChineseGuildName)
            {
                if (!Globals.GuildNameRegex.IsMatch(p.Name))
                {
                    Connection.ReceiveChat("Guild.GuildBadName".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }
            else
            {
                if (!Globals.EnGuildNameRegex.IsMatch(p.Name))
                {
                    Connection.ReceiveChat("Guild.GuildBadName".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            GuildInfo info = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, p.Name, StringComparison.OrdinalIgnoreCase) == 0);

            if (info != null)
            {
                Connection.ReceiveChat("Guild.GuildNameTaken".Lang(Connection.Language), MessageType.System);
                return;
            }

            info = SEnvir.GuildInfoList.CreateNewObject();

            info.GuildName = p.Name;
            info.MemberLimit = 30 + p.Members;   //默认创建行会成员30人
            info.StorageSize = 10 + p.Storage;   //默认创建行会仓库10格
            //info.GuildFunds = Globals.GuildCreationCost;
            info.GuildLevel = 1;

            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = info;
            memberInfo.Rank = "会长".Lang(Connection.Language);
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = GuildPermission.Leader;

            if (!p.UseGold)
            {
                for (int i = 0; i < Inventory.Length; i++)
                {
                    UserItem item = Inventory[i];
                    if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.UmaKingHorn) continue;

                    if (item.Count > 1)
                    {
                        item.Count -= 1;

                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                        break;
                    }

                    RemoveItem(item);
                    Inventory[i] = null;
                    item.Delete();

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });
                    break;
                }
            }

            Gold -= cost;
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "行会系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = (int)cost,
                ExtraInfo = $"创建行会"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            SendGuildInfo();
        }
        /// <summary>
        /// 行会编辑公告
        /// </summary>
        /// <param name="p"></param>
        public void GuildEditNotice(C.GuildEditNotice p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.EditNotice) != GuildPermission.EditNotice)
            {
                Connection.ReceiveChat("Guild.GuildNoticePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.Notice.Length > Globals.MaxGuildNoticeLength) return;


            Character.Account.GuildMember.Guild.GuildNotice = p.Notice;

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildNoticeChanged { Notice = p.Notice, ObserverPacket = false });
            }
        }
        /// <summary>
        /// 行会编辑成员
        /// </summary>
        /// <param name="p"></param>
        public void GuildEditMember(C.GuildEditMember p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildEditMemberPermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.Rank.Length > Globals.MaxCharacterNameLength)
            {
                Connection.ReceiveChat("Guild.GuildMemberLength".Lang(Connection.Language), MessageType.System);
                return;
            }


            if (p.Index > 0)
            {
                GuildMemberInfo info = Character.Account.GuildMember.Guild.Members.FirstOrDefault(x => x.Index == p.Index);

                if (info == null)
                {
                    Connection.ReceiveChat("Guild.GuildMemberNotFound".Lang(Connection.Language), MessageType.System);
                    return;
                }
                if (p.Permission == GuildPermission.Leader && (info.Permission != p.Permission) && Character.Account.GuildMember.Guild.Members.Where(x => x.Permission == p.Permission).ToList().Count() > 2)
                {
                    Connection.ReceiveChat("行会最多只能三个会长".Lang(Connection.Language), MessageType.System);
                    return;
                }

                if (info != Character.Account.GuildMember)
                    info.Permission = p.Permission;
                info.Rank = p.Rank;

                S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

                update.Members.Add(info.ToClientInfo());

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    member.Account.Connection?.Player?.Enqueue(update);

                info.Account.Connection?.Player?.Broadcast(new S.GuildChanged { ObjectID = info.Account.Connection.Player.ObjectID, GuildName = info.Guild.GuildName, GuildRank = info.Rank });
            }
            else
            {
                Character.Account.GuildMember.Guild.DefaultRank = p.Rank;
                Character.Account.GuildMember.Guild.DefaultPermission = p.Permission;

                S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    member.Account.Connection?.Player?.Enqueue(update);
            }
        }
        /// <summary>
        /// 行会删除成员
        /// </summary>
        /// <param name="p"></param>
        public void GuildKickMember(C.GuildKickMember p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildKickPermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            GuildMemberInfo info = Character.Account.GuildMember.Guild.Members.FirstOrDefault(x => x.Index == p.Index);

            if (info == null)
            {
                Connection.ReceiveChat("Guild.GuildMemberNotFound".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info == Character.Account.GuildMember)
            {
                Connection.ReceiveChat("Guild.GuildKickSelf".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (SEnvir.ConquestWars.Count > 0)   //攻城期间
            {
                Connection.ReceiveChat("攻城期间，无法删除成员。".Lang(Connection.Language), MessageType.System);
                return;
            }

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, info });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnKickGuild", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            GuildInfo guild = info.Guild;
            PlayerObject player = info.Account.Connection?.Player;
            string memberName = info.Account.LastCharacter.CharacterName;

            info.Account.GuildTime = SEnvir.Now.AddHours(Config.ExitGuild);  //行会时间改为小时，服务端设置为24小时;   

            info.Guild = null;
            info.Account = null;
            info.Delete();

            if (player != null)
            {
                player.Connection.ReceiveChat("Guild.GuildKicked".Lang(player.Connection.Language, Name), MessageType.System);
                player.Enqueue(new S.GuildInfo { ObserverPacket = false });
                player.Broadcast(new S.GuildChanged { ObjectID = player.ObjectID });
                player.RemoveAllObjects();
                player.ApplyGuildBuff();
            }

            foreach (GuildMemberInfo member in guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat("Guild.GuildMemberKicked".Lang(member.Account.Connection.Language, memberName, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(new S.GuildKick { Index = info.Index, ObserverPacket = false });
                member.Account.Connection.Player.RemoveAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

            foreach (GuildAllianceInfo allianceInfo in guild.Alliances)
            {
                foreach (GuildMemberInfo member in allianceInfo.Guild1 == guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
                {
                    if (member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildAllyOffline
                    {
                        Index = allianceInfo.Index,
                        ObserverPacket = false
                    });
                }
            }
        }
        /// <summary>
        /// 行会旗帜
        /// </summary>
        /// <param name="p"></param>
        public void GuildFlag(C.GuildFlag p)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }
            var guild = Character.Account.GuildMember.Guild;
            guild.GuildFlag = p.Flag;
            guild.FlagColor = p.Color;
            foreach (var war in ConquestWar.wars)
            {
                if (war.Key == guild.Castle && war.Value is ConquestWarFlag flagwar && flagwar.Flag != null)
                {
                    flagwar.Flag.SetImage(LibraryFile.Flag, guild.GuildFlag, guild.FlagColor);
                    break;
                }
            }

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        /// <summary>
        /// 行会税率
        /// </summary>
        /// <param name="p"></param>
        public void GuildTax(C.GuildTax p)
        {
            Enqueue(new S.GuildTax { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.Tax < 0 || p.Tax > 100) return;

            Character.Account.GuildMember.Guild.GuildTax = p.Tax / 100M;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        /// <summary>
        /// 行会增加成员
        /// </summary>
        /// <param name="p"></param>
        public void GuildIncreaseMember(C.GuildIncreaseMember p)
        {
            Enqueue(new S.GuildIncreaseMember { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            GuildInfo guild = Character.Account.GuildMember.Guild;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (guild.MemberLimit >= Config.GuildMemberHardLimit)
            {
                Connection.ReceiveChat("Guild.GuildMemberLimit".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (guild.GuildFunds < Config.GuildMemberCost)
            {
                Connection.ReceiveChat("Guild.GuildMemberCost".Lang(Connection.Language), MessageType.System);
                return;
            }

            guild.GuildFunds -= Config.GuildMemberCost;
            guild.DailyGrowth -= Config.GuildMemberCost;

            Character.Account.GuildMember.Guild.MemberLimit++;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        /// <summary>
        /// 行会增加仓库容量
        /// </summary>
        /// <param name="p"></param>
        public void GuildIncreaseStorage(C.GuildIncreaseStorage p)
        {
            Enqueue(new S.GuildIncreaseStorage { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            GuildInfo guild = Character.Account.GuildMember.Guild;
            if (guild.StorageSize >= 100)
            {
                Connection.ReceiveChat("Guild.GuildStorageLimit".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (guild.GuildFunds < Config.GuildStorageCost)
            {
                Connection.ReceiveChat("Guild.GuildStorageCost".Lang(Connection.Language), MessageType.System);
                return;
            }

            guild.GuildFunds -= Config.GuildStorageCost;
            guild.DailyGrowth -= Config.GuildStorageCost;
            Character.Account.GuildMember.Guild.StorageSize++;

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);
        }
        /// <summary>
        /// 行会邀请成员加入
        /// </summary>
        /// <param name="p"></param>
        public void GuildInviteMember(C.GuildInviteMember p)
        {
            Enqueue(new S.GuildInviteMember { ObserverPacket = false });

            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember)
            {
                Connection.ReceiveChat("Guild.GuildInvitePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            PlayerObject player = SEnvir.GetPlayerByCharacter(p.Name);

            if (player == null)
            {
                Connection.ReceiveChat("System.CannotFindPlayer".Lang(Connection.Language, p.Name), MessageType.System);
                return;
            }

            if (player.Character.Account.GuildMember != null)
            {
                Connection.ReceiveChat("Guild.GuildInviteGuild".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (player.GuildInvitation != null)
            {
                Connection.ReceiveChat("Guild.GuildInviteInvited".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (SEnvir.IsBlocking(Character.Account, player.Character.Account))
            {
                Connection.ReceiveChat("Guild.GuildInviteNotAllowed".Lang(Connection.Language, player.Name), MessageType.System);
                return;
            }
            if (!player.Character.Account.AllowGuild)
            {
                Connection.ReceiveChat("Guild.GuildInviteNotAllowed".Lang(Connection.Language, player.Name), MessageType.System);
                player.Connection.ReceiveChat("Guild.GuildInvitedNotAllowed".Lang(player.Connection.Language, Character.CharacterName, Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                return;
            }

            if (Character.Account.GuildMember.Guild.Members.Count >= Character.Account.GuildMember.Guild.MemberLimit)
            {
                Connection.ReceiveChat("Guild.GuildInviteRoom".Lang(Connection.Language, player.Name), MessageType.System);
                return;
            }

            player.GuildInvitation = this;
            player.Enqueue(new S.GuildInvite { Name = Name, GuildName = Character.Account.GuildMember.Guild.GuildName, ObserverPacket = false });
        }
        /// <summary>
        /// 申请加入行会
        /// </summary>
        /// <param name="guildIndex"></param>
        public void ApplyJoinGuild(int guildIndex)
        {
            if (Character.Account.GuildMember != null) return;

            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Index == guildIndex);
            if (guild == null)
            {
                Connection.ReceiveChat("申请的行会不存在".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!guild.AllowApply)
            {
                Connection.ReceiveChat("目标行会目前不接受申请加入".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (guild.PendingApplications.Contains(Character.Index))
            {
                Connection.ReceiveChat("请不要重复提交申请".Lang(Connection.Language), MessageType.System);
                return;
            }

            guild.PendingApplications.Add(Character.Index);
            Connection.ReceiveChat("入会申请已提交".Lang(Connection.Language), MessageType.System);

            foreach (GuildMemberInfo member in guild.Members)
            {
                if ((member.Permission & GuildPermission.AddMember) == GuildPermission.AddMember)
                {
                    member.Account?.Connection?.ReceiveChat("收到新的入会申请".Lang(Connection.Language), MessageType.Guild);
                }
            }
        }
        public void ProcessWithdrawal(int playerIndex, bool accept)
        {
            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            CharacterInfo playerInfo = SEnvir.GetCharacter(playerIndex);
            if (playerInfo == null)
            {
                Connection.ReceiveChat("玩家不存在".Lang(Connection.Language), MessageType.System);
                return;
            }
            long amount;
            if (!Character.Account.GuildMember.Guild.PendingWithdrawal.TryGetValue(playerIndex, out amount))
            {
                Connection.ReceiveChat("申请不存在".Lang(Connection.Language), MessageType.System);
                return;
            }
            if (accept)//同意
            {
                if (Character.Account.GuildMember.Guild.GameGoldTotal < amount)//行会赞助币不足
                {
                    Connection.ReceiveChat("行会赞助币不足，无法提现。".Lang(Connection.Language), MessageType.System);
                    return;
                }
                if (playerInfo.Account.GuildMember.GameGoldTotal < amount) //提交者的赞助币不足
                {

                    Connection.ReceiveChat("申请人提取赞助币超出数量，无法提现。".Lang(Connection.Language), MessageType.System);
                    return;
                }
                Character.Account.GuildMember.Guild.PendingWithdrawal.Remove(playerIndex);
                List<string> applicants = new List<string>();
                foreach (var characterIndex in Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.Keys)
                {
                    CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                    if (applicant == null) continue;
                    applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{Character?.Account?.GuildMember?.Guild?.PendingWithdrawal[characterIndex]},{applicant.LastLogin.ToString("MM/dd HH:mm")}");
                }

                Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                playerInfo.Account.GuildMember.GameGoldTotal -= amount;//更新提取玩家的
                if (playerInfo.Account.Connection?.Player != null)
                    playerInfo.Account.Connection?.Enqueue(new S.UpdateGuildGameTotal { Amount = playerInfo.Account.GuildMember.GameGoldTotal });
                Character.Account.GuildMember.Guild.GameGoldTotal -= amount;//更新行会赞助币总额
                Enqueue(new S.UpdateGuildGameTotal { Amount = Character.Account.GuildMember.Guild.GameGoldTotal });
                //发送赞助币的邮件
                //给赞助币卖家
                // 邮件交易
                var mail = SEnvir.MailInfoList.CreateNewObject();
                mail.Account = playerInfo.Account;

                //long tax = (long)(cost * Config.NewAuctionTax);//税率

                mail.Subject = "成功提现".Lang(Connection.Language);
                mail.Sender = "行会赞助币提现".Lang(Connection.Language);

                var gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                gold.Count = Convert.ToInt32(amount * 0.93); // * 0.95
                gold.Mail = mail;
                gold.Slot = 0;
                mail.Message = $"会长已经通过你的提现申请".Lang(Connection.Language) + "\n\n" +
                               string.Format("成功提现".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(gold.Count) / 100);

                mail.HasItem = true;
                if (playerInfo.Account.Connection?.Player != null)
                {
                    playerInfo.Account.Connection.Enqueue(new S.MailNew
                    {
                        Mail = mail.ToClientInfo(),
                        ObserverPacket = false,
                    });
                    applicants.Clear();
                    playerInfo.Account.Connection.Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                }
            }
            else
            {
                Character.Account.GuildMember.Guild.PendingWithdrawal.Remove(playerIndex);
                List<string> applicants = new List<string>();
                foreach (var characterIndex in Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.Keys)
                {
                    CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                    if (applicant == null) continue;
                    applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{Character?.Account?.GuildMember?.Guild?.PendingWithdrawal[characterIndex]},{applicant.LastLogin.ToString("MM/dd HH:mm")}");

                }

                Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                //发送被拒绝的邮件
                var mail = SEnvir.MailInfoList.CreateNewObject();
                mail.Account = playerInfo.Account;
                mail.Subject = "行会赞助币提现被拒绝".Lang(Connection.Language);
                mail.Sender = "行会赞助币提现".Lang(Connection.Language);
                mail.Message = $"你的赞助币提现申请已经被会长拒绝".Lang(Connection.Language) + "\n\n";
                mail.HasItem = true;
                if (playerInfo.Account.Connection?.Player != null)
                {
                    playerInfo.Account.Connection.Enqueue(new S.MailNew
                    {
                        Mail = mail.ToClientInfo(),
                        ObserverPacket = false,
                    });
                    applicants.Clear();
                    playerInfo.Account.Connection.Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                }
            }
        }


        /// <summary>
        /// 接收玩家申请入会
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="accept"></param>
        public void ProcessApplication(int playerIndex, bool accept)
        {
            if ((Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember)
            {
                Connection.ReceiveChat("Guild.GuildManagePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            CharacterInfo playerInfo = SEnvir.GetCharacter(playerIndex);
            if (playerInfo == null)
            {
                Connection.ReceiveChat("玩家不存在".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!Character.Account.GuildMember.Guild.PendingApplications.Contains(playerIndex))
            {
                Connection.ReceiveChat("申请不存在".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!accept)
            {
                Character.Account.GuildMember.Guild.PendingApplications.Remove(playerIndex);
                playerInfo.Account?.Connection?.ReceiveChat($"PlayerObject.RefusalGuildApplication".Lang(Connection.Language, Character.Account.GuildMember.Guild.GuildName), MessageType.Guild);
            }
            else
            {
                if (playerInfo.Account.GuildMember != null)
                {
                    Connection.ReceiveChat($"PlayerObject.ThereAreGuild".Lang(Connection.Language, playerInfo.CharacterName), MessageType.System);
                    return;
                }

                foreach (GuildInfo guild in SEnvir.GuildInfoList.Binding)
                {
                    guild.PendingApplications.Remove(playerIndex);
                }

                GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();
                memberInfo.Account = playerInfo.Account;
                memberInfo.Guild = Character.Account.GuildMember.Guild;
                memberInfo.Rank = Character.Account.GuildMember.Guild.DefaultRank;
                memberInfo.JoinDate = SEnvir.Now;
                memberInfo.Permission = Character.Account.GuildMember.Guild.DefaultPermission;

                PlayerObject playerObj = playerInfo.Account.Connection?.Player;
                if (playerObj != null)
                {
                    playerObj.SendGuildInfo();
                    playerObj.Connection.ReceiveChat("Guild.GuildJoinWelcome".Lang(Connection.Language, memberInfo.Guild.GuildName), MessageType.System);

                    Broadcast(new S.GuildChanged { ObjectID = playerObj.ObjectID, GuildName = memberInfo.Guild.GuildName, GuildRank = memberInfo.Rank });
                    playerObj.AddAllObjects();

                    //playerObj.ApplyCastleBuff();
                    playerObj.ApplyGuildBuff();

                }

                S.GuildUpdate update = memberInfo.Guild.GetUpdatePacket();
                update.Members.Add(memberInfo.ToClientInfo());
                foreach (GuildMemberInfo member in memberInfo.Guild.Members)
                {
                    if (member == memberInfo || member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.ReceiveChat($"PlayerObject.GuildJoinWelcome".Lang(Connection.Language, playerInfo.CharacterName), MessageType.System);
                    member.Account.Connection.Player.Enqueue(update);

                    member.Account.Connection.Player.AddAllObjects();
                    member.Account.Connection.Player.ApplyGuildBuff();
                }
                foreach (GuildAllianceInfo allianceInfo in memberInfo.Guild.Alliances)
                {
                    foreach (GuildMemberInfo member in allianceInfo.Guild1 == memberInfo.Guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildAllyOnline
                        {
                            Index = allianceInfo.Index,
                            ObserverPacket = false
                        });
                    }
                }

                //python 触发
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this, playerInfo, });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnAcceptGuild", args);
                        //trig_player(this, "OnPlayerAttack", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
                {
                    string msg = "PlayerEvent Syntax error : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "PlayerEvent SystemExit : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }

            }
        }
        /// <summary>
        /// 行会战
        /// </summary>
        /// <param name="guildName"></param>
        public void GuildWar(string guildName)
        {
            S.GuildWar result = new S.GuildWar { ObserverPacket = false };
            Enqueue(result);

            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("Guild.GuildNoGuild".Lang(Connection.Language), MessageType.System);
                return;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.StartWar) != GuildPermission.StartWar)
            {
                Connection.ReceiveChat("Guild.GuildWarPermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

            if (guild == null)
            {
                Connection.ReceiveChat("Guild.GuildNotFoundGuild".Lang(Connection.Language, guildName), MessageType.System);
                return;
            }

            if (guild.Index == 1)
            {
                Connection.ReceiveChat("你不能和新手行会进行行会战。".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (guild == Character.Account.GuildMember.Guild)
            {
                Connection.ReceiveChat("Guild.GuildWarOwnGuild".Lang(Connection.Language), MessageType.System);
                result.Success = true;
                return;
            }

            if (SEnvir.GuildWarInfoList.Binding.Any(x => (x.Guild1 == guild && x.Guild2 == Character.Account.GuildMember.Guild) ||
                                                         (x.Guild2 == guild && x.Guild1 == Character.Account.GuildMember.Guild)))
            {
                Connection.ReceiveChat("Guild.GuildAlreadyWar".Lang(Connection.Language, guild.GuildName), MessageType.System);
                return;
            }

            if (Config.GuildWarCost > Character.Account.GuildMember.Guild.GuildFunds)
            {
                Connection.ReceiveChat("Guild.GuildWarCost".Lang(Connection.Language), MessageType.System);
                return;
            }

            result.Success = true;

            Character.Account.GuildMember.Guild.GuildFunds -= Config.GuildWarCost;
            Character.Account.GuildMember.Guild.DailyGrowth -= Config.GuildWarCost;

            GuildWarInfo warInfo = SEnvir.GuildWarInfoList.CreateNewObject();

            warInfo.Guild1 = Character.Account.GuildMember.Guild;
            warInfo.Guild2 = guild;
            warInfo.Duration = TimeSpan.FromHours(2);  //行会站时间设置 2小时

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -Config.GuildWarCost, ObserverPacket = false });
                member.Account.Connection?.Player?.Enqueue(new S.GuildWarStarted { GuildName = guild.GuildName, Duration = warInfo.Duration });
            }

            foreach (GuildMemberInfo member in guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildWarStarted { GuildName = Character.Account.GuildMember.Guild.GuildName, Duration = warInfo.Duration });
            }
        }
        /// <summary>
        /// 请求行会联盟
        /// </summary>
        /// <param name="guildName"></param>
        public void RequestGuildAlliance(string guildName)
        {
            S.GuildAlliance result = new S.GuildAlliance { ObserverPacket = false };
            Enqueue(result);

            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

            if (!CanGuildAlliance(Character.Account.GuildMember.Guild, guild, guildName)) return;

            result.Success = true;

            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.GuildAllianceTreaty);
            if (info == null)
            {
                SEnvir.Log(string.Format("[数据库没有联盟道具]"));
                return;
            }
            ItemCheck check = new ItemCheck(info, 1, UserItemFlags.Expirable | UserItemFlags.Worthless, TimeSpan.FromSeconds(3600));
            if (CanGainItems(false, check))
            {
                UserItem item = SEnvir.CreateFreshItem(check);
                item.AddStat(Stat.Guild1, Character.Account.GuildMember.Guild.Index, StatSource.None);
                item.AddStat(Stat.Guild2, guild.Index, StatSource.None);
                item.StatsChanged();
                GainItem(item);
            };

            Connection.ReceiveChat("Guild.GuildAllianceTreatyCreated".Lang(Connection.Language, guild.GuildName), MessageType.System);
        }
        /// <summary>
        /// 接收行会联盟
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AcceptGuildAlliance(UserItem item)
        {
            if (item.Stats[Stat.Guild1] <= 0 || item.Stats[Stat.Guild2] <= 0)
            {
                SEnvir.Log("接受行会联盟: 没有行会属性", true);
                return true;
            }

            GuildInfo guild1 = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Index == item.Stats[Stat.Guild1]);
            GuildInfo guild2 = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Index == item.Stats[Stat.Guild2]);

            if (!CanGuildAlliance(guild2, guild1)) return false;

            if (Character.Account.GuildMember.Guild != guild2)
            {
                Connection.ReceiveChat("Guild.WrongAllianceGuild".Lang(Connection.Language), MessageType.System);
                return false;
            }

            GuildAllianceInfo allianceInfo = SEnvir.GuildAllianceInfoList.CreateNewObject();

            allianceInfo.Guild1 = Character.Account.GuildMember.Guild;
            allianceInfo.Guild2 = guild1;

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(new S.GuildAllianceStarted { AllianceInfo = allianceInfo.ToClientInfo(allianceInfo.Guild1) });

            foreach (GuildMemberInfo member in guild1.Members)
                member.Account.Connection?.Player?.Enqueue(new S.GuildAllianceStarted { AllianceInfo = allianceInfo.ToClientInfo(allianceInfo.Guild2) });

            return true;
        }
        /// <summary>
        /// 结束行会联盟
        /// </summary>
        /// <param name="guildName"></param>
        public void EndGuildAlliance(string guildName)
        {
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("Guild.GuildNoGuild".Lang(Connection.Language), MessageType.System);
                return;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.Alliance) != GuildPermission.Alliance)
            {
                Connection.ReceiveChat("Guild.GuildAlliancePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

            if (guild == null)
            {
                Connection.ReceiveChat("Guild.GuildNotFoundGuild".Lang(Connection.Language, guildName), MessageType.System);
                return;
            }

            if (guild == Character.Account.GuildMember.Guild)
            {
                Connection.ReceiveChat("Guild.GuildAllianceOwnGuild".Lang(Connection.Language), MessageType.System);
                return;
            }

            GuildAllianceInfo allianceInfo = SEnvir.GuildAllianceInfoList.Binding.FirstOrDefault(x => (x.Guild1 == guild && x.Guild2 == Character.Account.GuildMember.Guild) || (x.Guild2 == guild && x.Guild1 == Character.Account.GuildMember.Guild));

            if (allianceInfo == null)
            {
                Connection.ReceiveChat("Guild.GuildNoAlliance".Lang(Connection.Language, guild.GuildName), MessageType.System);
                return;
            }

            allianceInfo.Delete();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(new S.GuildAllianceEnded { GuildName = guild.GuildName });

            foreach (GuildMemberInfo member in guild.Members)
                member.Account.Connection?.Player?.Enqueue(new S.GuildAllianceEnded { GuildName = Character.Account.GuildMember.Guild.GuildName });
        }
        /// <summary>
        /// 允许行会联盟
        /// </summary>
        /// <param name="senderguild"></param>
        /// <param name="recieveguild"></param>
        /// <param name="tryname"></param>
        /// <returns></returns>
        public bool CanGuildAlliance(GuildInfo senderguild, GuildInfo recieveguild, string tryname = "")
        {
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("Guild.GuildNoGuild".Lang(Connection.Language), MessageType.System);
                return false;
            }

            if (senderguild == null || recieveguild == null)
            {
                Connection.ReceiveChat("Guild.GuildNotFoundGuild".Lang(Connection.Language, tryname), MessageType.System);
                return false;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.Alliance) != GuildPermission.Alliance)
            {
                Connection.ReceiveChat("Guild.GuildAlliancePermission".Lang(Connection.Language), MessageType.System);
                return false;
            }

            if (recieveguild == Character.Account.GuildMember.Guild)
            {
                Connection.ReceiveChat("Guild.GuildAllianceOwnGuild".Lang(Connection.Language), MessageType.System);
                return false;
            }

            if (SEnvir.GuildAllianceInfoList.Binding.Any(x => (x.Guild1 == recieveguild && x.Guild2 == senderguild) || (x.Guild2 == recieveguild && x.Guild1 == senderguild)))
            {
                Connection.ReceiveChat("Guild.GuildAlreadyAlliance".Lang(Connection.Language, recieveguild.GuildName), MessageType.System);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 行会请求申请攻城
        /// </summary>
        /// <param name="index"></param>
        public void GuildConquest(int index)
        {
            if (Character.Account.GuildMember == null)   //没有行会不能申请
            {
                Connection.ReceiveChat("Guild.GuildNoGuild".Lang(Connection.Language), MessageType.System);
                return;
            }

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)  //不是行会老大不能申请
            {
                Connection.ReceiveChat("Guild.GuildWarPermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (Character.Account.GuildMember.Guild.Castle != null)        //已经是沙巴克了不能申请
            {
                Connection.ReceiveChat("Guild.GuildConquestCastle".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (SEnvir.UserConquestList.Any(p => p.Guild == Character.Account.GuildMember.Guild && SEnvir.Now.AddDays(Config.WarsTime).Date == p.WarDate.Date))
            //if (Character.Account.GuildMember.Guild.Conquest != null)    //已经申请了攻城不能在重复申请
            {
                Connection.ReceiveChat("Guild.GuildConquestExists".Lang(Connection.Language), MessageType.System);
                return;
            }

            CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == index);  //赋值城堡信息列表序号

            if (castle == null)   //城堡信息列表为空 跳过
            {
                Connection.ReceiveChat("Guild.GuildConquestBadCastle".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (SEnvir.ConquestWars.Count > 0)   //攻城期间不能申请
            {
                Connection.ReceiveChat("Guild.GuildConquestProgress".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (castle.Item != null)    //攻城申请道具判断不为空
            {
                if (GetItemCount(castle.Item) == 0)  //没有申请攻城的道具不能申请
                {
                    Connection.ReceiveChat("Guild.GuildConquestNeedItem".Lang(Connection.Language, castle.Item.ItemName, castle.Name), MessageType.System);
                    return;
                }

                TakeItem(castle.Item, 1);  //扣除攻城申请的必须道具
            }

            DateTime now = SEnvir.Now;  //现在的时间
            DateTime date = new DateTime(now.Ticks - now.TimeOfDay.Ticks + TimeSpan.TicksPerDay * Config.WarsTime);

            if (now.TimeOfDay.Ticks >= castle.StartTime.Ticks)
                date = date.AddTicks(TimeSpan.TicksPerDay);

            UserConquest conquest = SEnvir.UserConquestList.CreateNewObject();
            conquest.Guild = Character.Account.GuildMember.Guild;
            conquest.Castle = castle;
            conquest.WarDate = date;

            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle); //赋值城堡主信息

            if (ownerGuild != null)
            {
                foreach (GuildMemberInfo member in ownerGuild.Members)  //给当前的城堡成员发送有人申请攻城的信息
                {
                    if (member.Account.Connection?.Player == null) continue; //脱机

                    member.Account.Connection.ReceiveChat("Guild.GuildConquestSuccess".Lang(member.Account.Connection.Language), MessageType.System);
                    member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = (date + castle.StartTime) - SEnvir.Now, ObserverPacket = false });
                }
            }

            //将战争日期发送给申请攻城的公会
            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue; //脱机

                member.Account.Connection.ReceiveChat("Guild.GuildConquestDate".Lang(member.Account.Connection.Language, castle.Name), MessageType.System);
                member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = (date + castle.StartTime) - SEnvir.Now, ObserverPacket = false });
            }
        }
        /// <summary>
        /// 加入行会
        /// </summary>
        public void GuildJoin()
        {
            if (GuildInvitation != null && GuildInvitation.Node == null) GuildInvitation = null;

            if (GuildInvitation == null) return;

            if (Character.Account.GuildMember != null)
            {
                Connection.ReceiveChat("Guild.GuildJoinGuild".Lang(Connection.Language), MessageType.System);
                return;
            }
            if (Character.Account.GuildTime > SEnvir.Now)
            {
                Connection.ReceiveChat("Guild.GuildJoinTime".Lang(Connection.Language, (Character.Account.GuildTime - SEnvir.Now).Lang(Connection.Language, true)), MessageType.System);
                return;
            }
            if (GuildInvitation.Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("Guild.GuildJoinGuild".Lang(Connection.Language, GuildInvitation.Name), MessageType.System);
                return;
            }

            if ((GuildInvitation.Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember)
            {
                Connection.ReceiveChat("Guild.GuildJoinPermission".Lang(Connection.Language, GuildInvitation.Name), MessageType.System);
                return;
            }

            if (GuildInvitation.Character.Account.GuildMember.Guild.Members.Count >= GuildInvitation.Character.Account.GuildMember.Guild.MemberLimit)
            {
                Connection.ReceiveChat("Guild.GuildJoinNoRoom".Lang(Connection.Language, GuildInvitation.Name), MessageType.System);
                return;
            }


            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = GuildInvitation.Character.Account.GuildMember.Guild;
            memberInfo.Rank = GuildInvitation.Character.Account.GuildMember.Guild.DefaultRank;
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = GuildInvitation.Character.Account.GuildMember.Guild.DefaultPermission;


            SendGuildInfo();
            Connection.ReceiveChat("Guild.GuildJoinWelcome".Lang(Connection.Language, memberInfo.Guild.GuildName), MessageType.System);

            Broadcast(new S.GuildChanged { ObjectID = ObjectID, GuildName = memberInfo.Guild.GuildName, GuildRank = memberInfo.Rank });
            AddAllObjects();

            S.GuildUpdate update = memberInfo.Guild.GetUpdatePacket();

            update.Members.Add(memberInfo.ToClientInfo());

            foreach (GuildMemberInfo member in memberInfo.Guild.Members)
            {
                if (member == memberInfo || member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat("Guild.GuildMemberJoined".Lang(member.Account.Connection.Language, GuildInvitation.Name, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(update);

                member.Account.Connection.Player.AddAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

            foreach (GuildAllianceInfo allianceInfo in memberInfo.Guild.Alliances)
            {
                foreach (GuildMemberInfo member in allianceInfo.Guild1 == memberInfo.Guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
                {
                    if (member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildAllyOnline
                    {
                        Index = allianceInfo.Index,
                        ObserverPacket = false
                    });
                }
            }

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnJoinGuild", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            //ApplyCastleBuff();
            ApplyGuildBuff();
        }
        /// <summary>
        /// 退出行会
        /// </summary>
        public void GuildLeave()
        {
            if (Character.Account.GuildMember == null) return;

            //正在攻城的城堡
            var wars = SEnvir.ConquestWars.Where(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == Character.Account.GuildMember.Guild.GuildName)).ToList();
            if (wars.Count > 0)
            {
                Connection.ReceiveChat("攻城中无法退出行会。".Lang(Connection.Language), MessageType.System);
                return;  //判断再攻城中无法退出行会
            }

            GuildMemberInfo info = Character.Account.GuildMember;

            if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) == GuildPermission.Leader && info.Guild.Members.Count > 1 && info.Guild.Members.FirstOrDefault(x => x.Index != info.Index && (x.Permission & GuildPermission.Leader) == GuildPermission.Leader) == null)
            {
                Connection.ReceiveChat("Guild.GuildLeaveFailed".Lang(Connection.Language), MessageType.System);
                return;
            }

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnLeaveGuild", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            GuildInfo guild = info.Guild;
            int index = info.Index;

            info.Guild = null;
            info.Account = null;
            info.Delete();
            Character.DayActiveCount = 0;
            Character.TotalActiveCount = 0;
            Character.DayExpAdd = 0;
            Character.DayDonations = 0;
            if (!guild.StarterGuild)      //退出行会冷却时间
                Character.Account.GuildTime = SEnvir.Now.AddHours(Config.ExitGuild);  //退出行会时间改为小时，服务端设置为24小时

            Connection.ReceiveChat("Guild.GuildLeave".Lang(Connection.Language), MessageType.System);
            Enqueue(new S.GuildInfo { ObserverPacket = false });

            Broadcast(new S.GuildChanged { ObjectID = ObjectID });
            RemoveAllObjects();

            foreach (GuildMemberInfo member in guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.Player.Enqueue(new S.GuildKick { Index = index, ObserverPacket = false });
                member.Account.Connection.ReceiveChat("Guild.GuildMemberLeave".Lang(member.Account.Connection.Language, Name), MessageType.System);
                member.Account.Connection.Player.RemoveAllObjects();
                member.Account.Connection.Player.ApplyGuildBuff();
            }

            foreach (GuildAllianceInfo allianceInfo in guild.Alliances)
            {
                foreach (GuildMemberInfo member in allianceInfo.Guild1 == guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
                {
                    if (member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildAllyOffline
                    {
                        Index = allianceInfo.Index,
                        ObserverPacket = false
                    });
                }
            }

            //ApplyCastleBuff();
            ApplyGuildBuff();
        }
        /// <summary>
        /// 是否战争同盟行会
        /// </summary>
        /// <returns></returns>

        public bool IsWarAlliances()
        {
            if (Character.Account.GuildMember == null) return false;
            foreach (GuildAllianceInfo allianceInfo in Character.Account.GuildMember.Guild.Alliances)
            {
                GuildInfo guild = Character.Account.GuildMember.Guild == allianceInfo.Guild1 ? allianceInfo.Guild2 : allianceInfo.Guild1;
                //正在攻城的城堡
                var wars = SEnvir.ConquestWars.Where(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == guild.GuildName)).ToList();
                if (wars.Count == 0) continue;

            }
            return false;
        }
        /// <summary>
        /// 是否为战争参与者
        /// </summary>
        /// <returns></returns>
        public bool IsWarPartake()
        {
            if (Character.Account.GuildMember == null) return false;

            //正在攻城的城堡
            var wars = SEnvir.ConquestWars.Where(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == Character.Account.GuildMember.Guild.GuildName)).ToList();
            if (wars.Count == 0) return false;

            return true;
        }

        /// <summary>
        /// 玩家是否为进攻方
        /// </summary>
        /// <returns></returns>
        public bool IsWarAttacker()
        {
            if (Character.Account.GuildMember == null) return false;

            //正在攻城的城堡
            var wars = SEnvir.ConquestWars.Where(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == Character.Account.GuildMember.Guild.GuildName)).ToList();
            if (wars.Count == 0) return false;

            //如果玩家所在行会的城堡 是被争夺的那个 那么他是守方 反之是攻方
            return !(wars.Exists(x => x.info.Name == Character.Account.GuildMember.Guild?.Castle?.Name));
        }
        /// <summary>
        /// 在战争中
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool AtWar(PlayerObject player)
        {
            foreach (ConquestWar conquest in SEnvir.ConquestWars)
            {
                if (conquest.Map != CurrentMap) continue;

                return !InGuild(player);
            }

            if (player.Character.Account.GuildMember == null) return false;
            if (Character.Account.GuildMember == null) return false;

            foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
            {
                if (warInfo.Guild1 == Character.Account.GuildMember.Guild && warInfo.Guild2 == player.Character.Account.GuildMember.Guild) return true;
                if (warInfo.Guild2 == Character.Account.GuildMember.Guild && warInfo.Guild1 == player.Character.Account.GuildMember.Guild) return true;
            }

            return false;
        }
        /// <summary>
        /// 发送行会信息
        /// </summary>
        public void SendGuildInfo()
        {
            if (Character.Account.GuildMember == null || Character.Account.GuildMember.Guild == null) return;

            S.GuildInfo result = new S.GuildInfo
            {
                Guild = Character.Account.GuildMember.Guild.ToClientInfo(),
                ObserverPacket = false,
            };
            result.Guild.OwnDailyActiveCount = Character.DayActiveCount;
            result.Guild.OwnTotalActiveCount = Character.TotalActiveCount;

            result.Guild.UserIndex = Character.Account.GuildMember.Index;

            foreach (GuildAllianceInfo allianceInfo in Character.Account.GuildMember.Guild.Alliances)
                result.Guild.Alliances.Add(allianceInfo.ToClientInfo(Character.Account.GuildMember.Guild));

            Enqueue(result);

            foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
            {
                if (warInfo.Guild1 == Character.Account.GuildMember.Guild)
                    Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild2.GuildName, Duration = warInfo.Duration });

                if (warInfo.Guild2 == Character.Account.GuildMember.Guild)
                    Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild1.GuildName, Duration = warInfo.Duration });
            }

            //将战争日期发送到公会
            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)
            {
                UserConquest conquest = SEnvir.UserConquestList.Binding.FirstOrDefault(x => x.Castle == castle && (x.Guild == Character.Account.GuildMember.Guild || x.Castle == Character.Account.GuildMember.Guild.Castle));

                TimeSpan warTime = TimeSpan.MinValue;
                if (conquest != null)
                    warTime = (conquest.WarDate + conquest.Castle.StartTime) - SEnvir.Now;

                Enqueue(new S.GuildConquestDate { Index = castle.Index, WarTime = warTime, ObserverPacket = false });
            }
        }
        /// <summary>
        /// 加入新手行会
        /// </summary>
        public void JoinStarterGuild()
        {
            if (Character.Account.GuildMember != null) return;

            GuildMemberInfo memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();

            memberInfo.Account = Character.Account;
            memberInfo.Guild = SEnvir.StarterGuild;
            memberInfo.Rank = SEnvir.StarterGuild.DefaultRank;
            memberInfo.JoinDate = SEnvir.Now;
            memberInfo.Permission = SEnvir.StarterGuild.DefaultPermission;

            SendGuildInfo();

            Connection.ReceiveChat("Guild.GuildJoinWelcome".Lang(Connection.Language, memberInfo.Guild.GuildName), MessageType.System);

            Broadcast(new S.GuildChanged { ObjectID = ObjectID, GuildName = memberInfo.Guild.GuildName, GuildRank = memberInfo.Rank });
            AddAllObjects();

            S.GuildUpdate update = memberInfo.Guild.GetUpdatePacket();

            update.Members.Add(memberInfo.ToClientInfo());

            foreach (GuildMemberInfo member in memberInfo.Guild.Members)
            {
                if (member == memberInfo || member.Account.Connection?.Player == null) continue;

                member.Account.Connection.ReceiveChat("Guild.GuildMemberJoined".Lang(member.Account.Connection.Language, SEnvir.StarterGuild, Name), MessageType.System);
                member.Account.Connection.Player.Enqueue(update);

                member.Account.Connection.Player.AddAllObjects();
            }

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnJoinGuild", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            //ApplyCastleBuff(); //使用沙巴克BUFF
            ApplyGuildBuff();  //使用行会BUFF
        }


        public void GuildlLevelUp()
        {
            if (Character.Account.GuildMember == null) return;
            var guild = Character.Account.GuildMember;
            if ((guild.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("不是行会会长，无法升级。", MessageType.System);
                return;
            }
            var explist = SEnvir.GuildLevelExpList.Binding.FirstOrDefault(x => x.Level == guild.Guild.GuildLevel);
            if (explist == null) return;
            if (explist.GuildFunds > guild.Guild.GuildFunds)
            {
                Connection.ReceiveChat("行会资金不足，无法升级。", MessageType.System);
                return; //行会资金不足
            }
            if (explist.ActivCount > guild.Guild.ActivCount)
            {
                Connection.ReceiveChat("行会活跃度不足，无法升级。", MessageType.System);
                return; //行会总贡献度不够
            }
            guild.Guild.GuildFunds -= explist.GuildFunds;
            guild.Guild.ActivCount -= explist.ActivCount;
            //升级
            guild.Guild.GuildLevel++;
            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);

        }
        /// <summary>
        /// 编辑行会公告
        /// </summary>
        /// <param name="content"></param>
        public void GuildEditVaultNotice(string content)
        {
            if (Character.Account.GuildMember == null) return;

            if ((Character.Account.GuildMember.Permission & GuildPermission.EditNotice) != GuildPermission.EditNotice)
            {
                Connection.ReceiveChat("Guild.GuildNoticePermission".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (content.Length > Globals.MaxGuildNoticeLength) return;


            Character.Account.GuildMember.Guild.GuildVaultNotice = content;

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
            {
                member.Account.Connection?.Player?.Enqueue(new S.GuildNoticeChanged { Notice = content, ObserverPacket = false });
            }
        }
        /// <summary>
        /// 捐献和提取行会赞助币
        /// </summary>
        /// <param name="p"></param>
        public void UpdateGuildGameGoldTotal(C.GuildGameGold p)
        {
            var amount = p.Amount * 100;//乘上货币比率
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("无法捐赠：你没有行会", MessageType.System);
                return;
            }
            if (amount < 0)
            {
                Connection.ReceiveChat("无法捐赠：不能捐赠负数", MessageType.System);
                return;
            }
            if (p.Type == 0)//捐赠
            {
                if (Character.Account.GameGold < amount)
                {
                    Connection.ReceiveChat("无法捐赠：你的赞助币不足", MessageType.System);
                    return;
                }
                ChangeGameGold(-(int)amount, "行会捐赠");
                Character.Account.GuildMember.GameGoldTotal += amount;//更新自己总捐赠赞助币
                Character.Account.GuildMember.Guild.GameGoldTotal += amount;  //更新行会赞助币
                //会长发送 行会总赞助币 会员发送自己捐赠的赞助币
                if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)//判断是否为会长
                {
                    //不是会长
                    Enqueue(new S.UpdateGuildGameTotal
                    {
                        Amount = Character.Account.GuildMember.GameGoldTotal
                    });
                    //如果会长在线而更新会长
                    GuildMemberInfo member = Character.Account.GuildMember.Guild.Members.FirstOrDefault(x => (x.Permission & GuildPermission.Leader) == GuildPermission.Leader);

                    member?.Account.Connection?.Player?.Enqueue(new S.UpdateGuildGameTotal
                    {
                        Amount = Character.Account.GuildMember.Guild.GameGoldTotal
                    });

                }
                else
                {
                    //会长
                    Enqueue(new S.UpdateGuildGameTotal
                    {
                        Amount = Character.Account.GuildMember.Guild.GameGoldTotal
                    });
                }
            }
            else if (p.Type == 1)//提取
            {
                //会长从行会赞助币中提取 会员提取自己曾经捐赠赞助币
                if ((Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)
                {
                    if (amount > Character.Account.GuildMember.GameGoldTotal)
                    {
                        Connection.ReceiveChat("无法提取：你的行会捐赠赞助币不足", MessageType.System);
                        return;
                    }
                    //保存到数据库中  在线通知自己
                    //Character.Account.GuildMember.GameGoldTotal -= amount;//更新自己总捐赠赞助币
                    //Character.Account.GuildMember.Guild.GameGoldTotal -= amount;  //更新行会赞助币
                    //ChangeGameGold((int)amount, "行会非会长提取");
                    if (Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.ContainsKey(Character.Index) ?? false)
                        Character.Account.GuildMember.Guild.PendingWithdrawal[Character.Index] = amount;
                    else
                        Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.Add(Character.Index, amount);

                    int characterIndex = Character.Index;
                    List<string> applicants = new List<string>();
                    CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                    if (applicant != null)
                        applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{amount},{applicant.LastLogin.ToString("MM/dd HH:mm")}");
                    Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                }
                //else
                //{
                //    if (amount > Character.Account.GuildMember.Guild.GameGoldTotal)
                //    {
                //        Connection.ReceiveChat("无法提取：行会捐赠赞助币不足", MessageType.System);
                //        return;
                //    }

                //    Character.Account.GuildMember.GameGoldTotal -= amount;//更新自己总捐赠赞助币
                //    Character.Account.GuildMember.Guild.GameGoldTotal -= amount;  //更新行会赞助币
                //    ChangeGameGold((int)amount, "行会赞助币会长提取");
                //    //会长
                //    Enqueue(new S.UpdateGuildGameTotal
                //    {
                //        Amount = Character.Account.GuildMember.Guild.GameGoldTotal
                //    });

                //}
            }
        }
        /// <summary>
        /// 玩家捐赠金币
        /// </summary>
        /// <param name="amount">数额</param>
        public void DonateGoldToGuild(long amount)
        {
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("无法捐赠：你没有行会", MessageType.System);
                return;
            }

            if (amount < 0)
            {
                Connection.ReceiveChat("无法捐赠：不能捐赠负数", MessageType.System);
                return;
            }

            if (Gold < amount)
            {
                Connection.ReceiveChat("无法捐赠：你的金币不足", MessageType.System);
                return;
            }

            GuildMemberInfo info = Character.Account.GuildMember;

            if (info.Guild.GuildFunds >= Config.GuildMaxFund)
            {
                Connection.ReceiveChat("无法捐赠：行会金库已达上限", MessageType.System);
                return;
            }

            ChangeGold(-amount);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "行会系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = amount,
                ExtraInfo = $"行会捐赠系统扣除捐赠金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            info.Guild.GuildFunds += amount;
            info.Guild.DailyGrowth += amount;

            info.Guild.DailyContribution += amount;
            info.Guild.TotalContribution += amount;
            int perAmount = Config.PersonalGoldRatio;//金币兑换活跃度
            int maxCount = Config.ActivationCeiling; //活跃度上限
            long t = 0;
            if (DayDonations / perAmount <= maxCount && (DayDonations + (int)amount) / perAmount > DayDonations / perAmount && DayActiveCount < maxCount)
            {
                t = (DayDonations + (int)amount) / perAmount - DayDonations / perAmount;
                if (DayActiveCount + t > maxCount) t = maxCount - DayActiveCount;//保证只加10点每天
                TotalActiveCount += t;
                DayActiveCount += t;
                info.Guild.ActivCount += t;
                info.Guild.DailyActivCount += t;
                DayDonations += amount;
                Enqueue(new S.GuildActiveCountChange
                {
                    DailyActiveCount = DayActiveCount,
                    TotalActiveCount = TotalActiveCount
                });
            }
            else
                DayDonations += amount;

            info.DailyContribution += amount;
            info.TotalContribution += amount;

            foreach (GuildMemberInfo member in info.Guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.Enqueue(new S.GuildMemberContribution
                {
                    Index = info.Index,
                    Contribution = amount,
                    ActiveCount = t,
                    IsVoluntary = true,
                    ObserverPacket = false
                });
            }

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);

            if (SEnvir.GuildFundChangeInfoList.Binding.Count >= Config.GuildFundChangeRecordLimit)
            {
                SEnvir.GuildFundChangeInfoList.Binding.First().Delete();
            }

            GuildFundChangeInfo fundChangeInfo = SEnvir.GuildFundChangeInfoList.CreateNewObject();
            fundChangeInfo.Guild = info.Guild;
            fundChangeInfo.Amount = amount;
            fundChangeInfo.CharacterName = Character.CharacterName;
            fundChangeInfo.OperationTime = SEnvir.Now;

            Connection.ReceiveChat($"捐赠成功！你捐献了{amount}金币", MessageType.System);
        }
        /// <summary>
        /// 会长取出金币
        /// </summary>
        /// <param name="amount">数额</param>
        public void WithdrawGoldFromGuild(long amount)
        {
            if (Character.Account.GuildMember == null)
            {
                Connection.ReceiveChat("无法取款：你没有行会", MessageType.System);
                return;
            }

            GuildMemberInfo info = Character.Account.GuildMember;

            if (amount < 0)
            {
                Connection.ReceiveChat("无法取款：不能取出负数", MessageType.System);
                return;
            }

            if (info.Guild.GuildFunds < amount)
            {
                Connection.ReceiveChat("无法取款：行会资金不足", MessageType.System);
                return;
            }

            if ((info.Permission & GuildPermission.Leader) != GuildPermission.Leader)
            {
                Connection.ReceiveChat("Guild.GuildWarPermission".Lang(Connection.Language), MessageType.System);
                return;
            }


            info.Guild.GuildFunds -= amount;
            info.Guild.DailyGrowth -= amount;

            info.Guild.DailyContribution -= amount;
            info.Guild.TotalContribution -= amount;

            info.DailyContribution -= amount;
            info.TotalContribution -= amount;

            ChangeGold(amount);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "行会系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Add,
                Source = CurrencySource.ItemAdd,
                Amount = amount,
                ExtraInfo = $"行会会长提取行会资金获得金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            foreach (GuildMemberInfo member in info.Guild.Members)
            {
                if (member.Account.Connection?.Player == null) continue;

                member.Account.Connection.Enqueue(new S.GuildMemberContribution
                {
                    Index = info.Index,
                    Contribution = -amount,
                    IsVoluntary = true,
                    ObserverPacket = false
                });
            }

            S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

            foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                member.Account.Connection?.Player?.Enqueue(update);

            if (SEnvir.GuildFundChangeInfoList.Binding.Count >= Config.GuildFundChangeRecordLimit)
            {
                SEnvir.GuildFundChangeInfoList.Binding.First().Delete();
            }

            GuildFundChangeInfo fundChangeInfo = SEnvir.GuildFundChangeInfoList.CreateNewObject();
            fundChangeInfo.Guild = info.Guild;
            fundChangeInfo.Amount = -amount;
            fundChangeInfo.CharacterName = Character.CharacterName;
            fundChangeInfo.OperationTime = SEnvir.Now;
        }

        #endregion
    }
}
