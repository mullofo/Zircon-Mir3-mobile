using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //聊天
    {
        /// <summary>
        /// 禁止私聊开关
        /// </summary>
        public bool BlockWhisper;
        /// <summary>
        /// 允许回生开关
        /// </summary>
        public bool ResurrectionOrder;

        public void Chat(string text, List<ChatItemInfo> linkedItems = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            SEnvir.LogChat($"{Name}: {text}");

            //Item Links
            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, text, });
                    Nullable<bool> notsay = SEnvir.ExecutePyWithTimer(trig_play, this, "OnChat", args);
                    //Nullable<bool> notsay = trig_play(this, "OnChat", args);
                    if (notsay != null)
                    {
                        if (notsay.Value)
                        {
                            return;
                        }
                    }
                }
            }
            catch (SyntaxErrorException e)
            {
                string msg = "Player事件（同步错误） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "Player事件（系统退出） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "Player事件（加载插件时错误）: \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            if (linkedItems != null && linkedItems.Count > 0)
                foreach (var linkedItem in linkedItems)
                {
                    Regex r = new Regex(linkedItem.RegexInternalName, RegexOptions.IgnoreCase);
                    text = r.Replace(text, linkedItem.InternalName, 1);
                }

            string[] parts;

            if (CurrentMap.Info.CanNoChat == true && !Character.Account.TempAdmin) return;  //如果地图属性禁言 无法聊天

            //私聊
            if (text.StartsWith("/"))
            {
                //Private Message
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                SConnection con = SEnvir.GetConnectionByCharacter(parts[0]);

                if (con == null || (con.Stage != GameStage.Observer && con.Stage != GameStage.Game) || SEnvir.IsBlocking(Character.Account, con.Account))
                {
                    Connection.ReceiveChat("System.CannotFindPlayer".Lang(Connection.Language, parts[0]), MessageType.System);
                    return;
                }

                if (!Character.Account.TempAdmin)
                {
                    if (BlockWhisper)
                    {
                        Connection.ReceiveChat("System.BlockingWhisper".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    if (con.Player != null && con.Player.BlockWhisper)
                    {
                        Connection.ReceiveChat("System.PlayerBlockingWhisper".Lang(Connection.Language, parts[0]), MessageType.System);
                        return;
                    }
                }

                ProcessChatItems(Connection, linkedItems);
                Connection.ReceiveChat($"/{text}", MessageType.WhisperOut);  //发给自己

                if (SEnvir.Now < Character.Account.ChatBanExpiry) return;

                ProcessChatItems(con, linkedItems);
                con.ReceiveChat($"{Name}=> {text.Remove(0, parts[0].Length)}", Character.Account.TempAdmin ? MessageType.GMWhisperIn : MessageType.WhisperIn);
            }
            //组队
            else if (text.StartsWith("!!"))
            {
                if (GroupMembers == null) return;

                text = $"-{Name}: {text.Remove(0, 2)}";

                foreach (PlayerObject member in GroupMembers)
                {
                    if (SEnvir.IsBlocking(Character.Account, member.Character.Account)) continue;

                    if (member != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    ProcessChatItems(member.Connection, linkedItems);
                    member.Connection.ReceiveChat(text, MessageType.Group);
                }
            }
            //行会
            else if (text.StartsWith("!~"))
            {
                if (Character.Account.GuildMember == null) return;

                text = $"{Name}: {text.Remove(0, 2)}";

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member.Account.Connection == null) continue;
                    if (member.Account.Connection.Stage != GameStage.Game && member.Account.Connection.Stage != GameStage.Observer) continue;
                    if (SEnvir.IsBlocking(Character.Account, member.Account)) continue;

                    ProcessChatItems(member.Account.Connection, linkedItems);
                    member.Account.Connection.ReceiveChat(text, MessageType.Guild);
                }
            }
            //世界喊话
            else if (text.StartsWith("!@"))
            {
                if (!Character.Account.TempAdmin)
                {
                    if (SEnvir.Now < Character.Account.GlobalTime)
                    {
                        Connection.ReceiveChat("System.GlobalDelay".Lang(Connection.Language, Math.Ceiling((Character.Account.GlobalTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }
                    if (Level < 33 && Stats[Stat.GlobalShout] == 0)
                    {
                        Connection.ReceiveChat("System.GlobalLevel".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    Character.Account.GlobalTime = SEnvir.Now.AddSeconds(30);
                }

                if (Character.Account.TempAdmin)
                {
                    text = string.Format("【" + "系统".Lang(Connection.Language) + "】 {0}", text.Remove(0, 2));
                }
                else
                {
                    if (SEnvir.Now < Character.Account.ChatBanExpiry) return;

                    text = string.Format("(!@){0}: {1}", Name, text.Remove(0, 2));
                }

                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            if (SEnvir.IsBlocking(Character.Account, con.Account)) continue;

                            ProcessChatItems(con, linkedItems);
                            con.ReceiveChat(text, MessageType.Global);
                            break;
                        default: continue;
                    }
                }
            }
            //区域喊话
            else if (text.StartsWith("!"))
            {
                //Shout
                if (!Character.Account.TempAdmin)
                {
                    if (SEnvir.Now < ShoutTime)
                    {
                        Connection.ReceiveChat("System.ShoutDelay".Lang(Connection.Language, Math.Ceiling((ShoutTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }
                    if (Level < 2)
                    {
                        Connection.ReceiveChat("System.ShoutLevel".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                }

                text = string.Format("(!){0}: {1}", Name, text.Remove(0, 1));
                ShoutTime = SEnvir.Now + Config.ShoutDelay;

                foreach (PlayerObject player in CurrentMap.Players)
                {
                    if (player != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    if (!SEnvir.IsBlocking(Character.Account, player.Character.Account))
                    {
                        ProcessChatItems(player.Connection, linkedItems);
                        player.Connection.ReceiveChat(text, MessageType.Shout);
                    }

                    foreach (SConnection observer in player.Connection.Observers)
                    {
                        if (SEnvir.IsBlocking(Character.Account, observer.Account)) continue;
                        ProcessChatItems(observer, linkedItems);
                        observer.ReceiveChat(text, MessageType.Shout);
                    }
                }
            }
            //公告
            else if (text.StartsWith("@!"))
            {
                if (!Character.Account.TempAdmin) return;

                text = string.Format("{0}: {1}", Name, text.Remove(0, 2));

                foreach (SConnection con in SEnvir.Connections)
                {
                    switch (con.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            con.ReceiveChat(text, MessageType.Announcement);
                            break;
                        default: continue;
                    }
                }
            }
            //命令
            else if (text.StartsWith("@"))
            {
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                int level;
                int count;
                int castleName;
                PlayerObject player;
                var command = parts[0].ToUpper();
                var lan = Connection.Language;
                //GM命令

                if (command == "掷骰子".Lang(lan))
                {
                    if (GroupMembers == null) return;   //如果组队为空 跳开

                    if (parts.Length < 2 || !int.TryParse(parts[1], out count) || count < 0)   //随机数
                        count = 6;  //数量最大为6点

                    int result = SEnvir.Random.Next(count) + 1;

                    foreach (PlayerObject member in GroupMembers)
                        member.Connection.ReceiveChat("System.DiceRoll".Lang(member.Connection.Language, Name, result, count), MessageType.Group);
                }
                else if (command == "属性提取".Lang(lan))
                {
                    ExtractorLock = !ExtractorLock;

                    Connection.ReceiveChat(ExtractorLock ? "开启属性提取".Lang(lan) : "锁定属性提取".Lang(lan), MessageType.System);
                }
                else if (command == "宠物技能3".Lang(lan))
                {
                    CompanionLevelLock3 = !CompanionLevelLock3;

                    Connection.ReceiveChat((CompanionLevelLock3 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 3), MessageType.System);
                }
                else if (command == "宠物技能5".Lang(lan))
                {
                    CompanionLevelLock5 = !CompanionLevelLock5;
                    Connection.ReceiveChat((CompanionLevelLock5 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 5), MessageType.System);
                }
                else if (command == "宠物技能7".Lang(lan))
                {
                    CompanionLevelLock7 = !CompanionLevelLock7;
                    Connection.ReceiveChat((CompanionLevelLock7 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 7), MessageType.System);
                }
                else if (command == "宠物技能10".Lang(lan))
                {
                    CompanionLevelLock10 = !CompanionLevelLock10;
                    Connection.ReceiveChat((CompanionLevelLock10 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 10), MessageType.System);
                }
                else if (command == "宠物技能11".Lang(lan))
                {
                    CompanionLevelLock11 = !CompanionLevelLock11;
                    Connection.ReceiveChat((CompanionLevelLock11 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 11), MessageType.System);
                }
                else if (command == "宠物技能13".Lang(lan))
                {
                    CompanionLevelLock13 = !CompanionLevelLock13;
                    Connection.ReceiveChat((CompanionLevelLock13 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 13), MessageType.System);
                }
                else if (command == "宠物技能15".Lang(lan))
                {
                    CompanionLevelLock15 = !CompanionLevelLock15;
                    Connection.ReceiveChat((CompanionLevelLock15 ? "Companion.CompanionSkillEnabled" : "Companion.CompanionSkillDisabled").Lang(Connection.Language, 15), MessageType.System);
                }
                else if (command == "允许交易".Lang(lan))
                {
                    Character.Account.AllowTrade = !Character.Account.AllowTrade;
                    Connection.ReceiveChat((Character.Account.AllowTrade ? "System.TradingEnabled" : "System.TradingDisabled").Lang(Connection.Language), MessageType.System);
                }
                else if (command == "允许好友".Lang(lan))
                {
                    FriendSwitch(!Character.Account.AllowFriend);
                    Connection.ReceiveChat((Character.Account.AllowFriend ? "System.FriendEnabled" : "System.FriendDisabled").Lang(Connection.Language), MessageType.System);
                }
                else if (command == "添加好友".Lang(lan))
                {
                    if (parts.Length != 2) return;

                    player = this;
                    if (player == null) return;
                    FriendInvite(parts[1]);
                }
                else if (command == "拒绝私聊".Lang(lan))
                {
                    BlockWhisper = !BlockWhisper;
                    Connection.ReceiveChat((BlockWhisper ? "System.WhisperDisabled" : "System.WhisperEnabled").Lang(Connection.Language), MessageType.System);
                }
                else if (command == "允许回生术".Lang(lan))
                {
                    if (!Config.ResurrectionOrder) return;
                    Character.Account.AllowResurrectionOrder = !Character.Account.AllowResurrectionOrder;
                    Connection.ReceiveChat((Character.Account.AllowResurrectionOrder ? "System.ResurrectionOrderEnabled" : "System.ResurrectionOrderDisabled").Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat((Character.Account.AllowResurrectionOrder ? "System.ResurrectionOrderEnabled" : "System.ResurrectionOrderDisabled").Lang(con.Language), MessageType.System);
                }
                else if (command == "允许加入行会".Lang(lan))
                {
                    Character.Account.AllowGuild = !Character.Account.AllowGuild;
                    Connection.ReceiveChat((Character.Account.AllowGuild ? "System.GuildInviteEnabled" : "System.GuildInviteDisabled").Lang(Connection.Language), MessageType.System);
                }
                else if (command == "退出行会".Lang(lan))
                {
                    GuildLeave();
                }
                else if (command == "传唤".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //不是管理员跳开
                    if (parts.Length < 2) return;   //小于2个指令输入跳开

                    player = SEnvir.GetPlayerByCharacter(parts[1]);   //等于玩家名

                    player?.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction));  //把玩家传到自己面前
                }
                else if (command == "允许天地合一".Lang(lan))
                {
                    Character.Account.AllowGroupRecall = !Character.Account.AllowGroupRecall;
                    Connection.ReceiveChat((Character.Account.AllowGroupRecall ? "System.GroupRecallEnabled" : "System.GroupRecallDisabled").Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat((Character.Account.AllowGroupRecall ? "System.GroupRecallEnabled" : "System.GroupRecallDisabled").Lang(Connection.Language), MessageType.System);
                }
                else if (command == "天地合一".Lang(lan))
                {
                    if (Stats[Stat.RecallSet] <= 0) return;   //如果天地合一命令 没打开 跳过

                    if (GroupMembers == null)   //没组队
                    {
                        Connection.ReceiveChat("Group.GroupNoGroup".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Group.GroupNoGroup".Lang(con.Language), MessageType.System);
                        return;
                    }

                    if (GroupMembers[0] != this)   //不是队长
                    {
                        Connection.ReceiveChat("Group.GroupNotLeader".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Group.GroupNotLeader".Lang(con.Language), MessageType.System);
                        return;
                    }

                    //地图回城限制  地图传送限制 地图使用技能限制
                    if (!CurrentMap.Info.AllowTT || !CurrentMap.Info.AllowRT || CurrentMap.Info.SkillDelay > 0)
                    {
                        Connection.ReceiveChat("Group.GroupRecallMap".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Group.GroupRecallMap".Lang(con.Language), MessageType.System);
                        return;
                    }

                    if (SEnvir.Now < Character.GroupRecallTime)   //传送冷却时间
                    {
                        Connection.ReceiveChat("Group.GroupRecallDelay".Lang(Connection.Language, (Character.GroupRecallTime - SEnvir.Now).Lang(Connection.Language, true)), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Group.GroupRecallDelay".Lang(con.Language, (Character.GroupRecallTime - SEnvir.Now).Lang(con.Language, true)), MessageType.System);
                        return;
                    }

                    foreach (PlayerObject member in GroupMembers)   //队伍成员定义
                    {
                        if (member.Dead || member == this) continue;   //成员死亡或者成员是自己

                        if (!member.CurrentMap.Info.AllowTT)   //地图回城限制
                        {
                            member.Connection.ReceiveChat("Group.GroupRecallFromMap".Lang(member.Connection.Language), MessageType.System);

                            foreach (SConnection con in member.Connection.Observers)
                                con.ReceiveChat("Group.GroupRecallFromMap".Lang(con.Language), MessageType.System);

                            Connection.ReceiveChat("Group.GroupRecallMemberFromMap".Lang(Connection.Language, member.Name), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Group.GroupRecallMemberFromMap".Lang(con.Language, member.Name), MessageType.System);
                            continue;
                        }

                        if (!member.Character.Account.AllowGroupRecall)   //天地一合是否开启
                        {
                            member.Connection.ReceiveChat("Group.GroupRecallNotAllowed".Lang(member.Connection.Language), MessageType.System);

                            foreach (SConnection con in member.Connection.Observers)
                                con.ReceiveChat("Group.GroupRecallNotAllowed".Lang(con.Language), MessageType.System);

                            Connection.ReceiveChat("Group.GroupRecallMemberNotAllowed".Lang(member.Connection.Language, member.Name), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Group.GroupRecallMemberNotAllowed".Lang(con.Language, member.Name), MessageType.System);
                            continue;
                        }

                        //member.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 18));   //传送队员成员到当前坐标的范围10格
                        member.Teleport(CurrentMap, CurrentLocation);   //传送队员成员到当前坐标
                    }
                    Character.GroupRecallTime = SEnvir.Now.AddMinutes(Config.GroupRecallTime);   //天地合一的冷却时间
                }
                else if (command == "清理药品快捷栏".Lang(lan))
                {
                    for (int i = Character.BeltLinks.Count - 1; i >= 0; i--)   //判断药品快捷栏的物品数量
                        Character.BeltLinks[i].Delete();   //删除所有快捷设置
                }
                else if (command == "隐身".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //不是管理员 跳开
                    Observer = !Observer;   //隐身

                    AddAllObjects();
                    RemoveAllObjects();
                    Connection.ReceiveChat(Observer ? "开启隐身".Lang(lan) : "关闭隐身".Lang(lan), MessageType.System);
                }
                else if (command == "GM".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //不是管理员 跳开
                    GameMaster = !GameMaster;   //不是GM

                    Connection.ReceiveChat(GameMaster ? "管理员模式(开启无敌)".Lang(lan) : "正常模式(关闭无敌)".Lang(lan), MessageType.System);
                }
                else if (command == "金币限制".Lang(lan))
                {   //限制金币爆出  原命令GOLDBOT
                    if (!Character.Account.TempAdmin) return;   //不是管理员 跳开

                    if (parts.Length < 2) return;  //小于2个指令输入 跳开

                    CharacterInfo target = SEnvir.GetCharacter(parts[1]);  //等于玩家名

                    if (target == null) return;   //玩家目标等空 跳开

                    target.Account.GoldBot = !target.Account.GoldBot;   //目标账号的金币限制判断
                    target.Player?.ObservableSwitch(true);   //目标 玩家？.可观察开关（真）
                    Connection.ReceiveChat($"金币爆率惩罚".Lang(lan) + $" [{target.CharacterName}] - [{target.Account.GoldBot}]", MessageType.System);
                }
                else if (command == "爆率限制".Lang(lan))
                {  //限制道具爆出  ITEMBOT
                    if (!Character.Account.TempAdmin) return;   //不是管理员 跳开

                    if (parts.Length < 2) return;   //小于2个指令输入 跳开

                    CharacterInfo target = SEnvir.GetCharacter(parts[1]);  //等于玩家名

                    target = SEnvir.GetCharacter(parts[1]);   //等于玩家名

                    if (target == null) return;   //玩家目标等空 跳开

                    target.Account.ItemBot = !target.Account.ItemBot;   //目标账号的道具限制判断
                    target.Player?.ObservableSwitch(true);   //目标 玩家？.可观察开关（真）
                    Connection.ReceiveChat($"物品爆率惩罚".Lang(lan) + $" [{target.CharacterName}] - [{target.Account.ItemBot}]", MessageType.System);
                }
                else if (command == "调级".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //不是管理员 跳开

                    if (parts.Length < 3)    //判断小于3个指令输入
                    {
                        if (parts.Length < 2) return;   //小于两个指令输入 跳开

                        if (!int.TryParse(parts[1], out castleName) || castleName < 0) return;   //输入1个指令时

                        player = this;   //等于自己
                    }
                    else
                    {
                        if (!int.TryParse(parts[2], out castleName) || castleName < 0) return;   //输入2个指令时

                        player = SEnvir.GetPlayerByCharacter(parts[1]);   //等于玩家名
                    }

                    if (player == null) return;  //如果玩家等空 跳开

                    player.Level = castleName;   //玩家等级等设置的值
                    player.LevelUp();   //玩家升级函数调用
                }
                else if (command == "武器升级".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;    //不是管理员 跳开

                    if (parts.Length < 3)   //判断小于3个指令输入
                    {
                        if (parts.Length < 2) return;   //小于两个指令输入 跳开

                        if (!int.TryParse(parts[1], out castleName) || castleName < 0) return;   //输入1个指令时

                        player = this;   //等于自己
                    }
                    else
                    {
                        if (!int.TryParse(parts[2], out castleName) || castleName < 0) return;   //输入2个指令时

                        player = SEnvir.GetPlayerByCharacter(parts[1]);   //等于玩家名
                    }

                    if (player == null) return;   //如果玩家等空 跳开

                    UserItem weapon = player.Equipment[(int)EquipmentSlot.Weapon];   //判断玩家手上装备的武器

                    castleName = Math.Min(castleName, Globals.GameWeaponEXPInfoList.Count);    //武器等级值

                    if (weapon != null)   //如果武器不等空
                    {
                        weapon.Experience = 0;   //武器经验置0
                        weapon.Level = castleName;    //武器等级等于输入的级别数字

                        if (weapon.Level < Globals.GameWeaponEXPInfoList.Count)   //判断武器是否小于武器设置的等级
                            weapon.Flags |= UserItemFlags.Refinable;    //增加可精炼标签

                        Connection.ReceiveChat($"武器等级".Lang(lan) + $" => {castleName}", MessageType.System);
                        //发送封包，更新玩家的武器
                        Enqueue(new S.ItemExperience { Target = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon }, Experience = weapon.Experience, Level = weapon.Level, Flags = weapon.Flags });
                    }
                }
                else if (command == "飞到".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //不是管理员 跳开
                    if (parts.Length < 2) return;   //小于2个指令输入 跳开

                    player = SEnvir.GetPlayerByCharacter(parts[1]);   //等于玩家名

                    if (player == null) return;   //如果玩家等空 跳开

                    Teleport(player.CurrentMap, player.CurrentLocation);   //传送到玩家当前地图 当前坐标
                }
                else if (command == "学习技能".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //不是管理员 跳开
                    if (parts.Length < 2) return;   //小于2个指令输入 跳开

                    player = SEnvir.GetPlayerByCharacter(parts[1]);   //等于玩家名

                    if (player == null) return;   //如果玩家等空 跳开

                    UserMagic uMagic;  //用户魔法技能
                    foreach (MagicInfo mInfo in SEnvir.MagicInfoList.Binding)   //技能列表绑定
                    {
                        //技能等级 玩家职业 技能属性定义
                        if (mInfo.NeedLevel1 > player.Level || mInfo.Class != player.Class || mInfo.School == MagicSchool.None) continue;

                        if (!player.Magics.TryGetValue(mInfo.Magic, out uMagic))
                        {
                            uMagic = SEnvir.UserMagicList.CreateNewObject();  //创建新对象
                            uMagic.Character = player.Character;  //等目标玩家
                            uMagic.Info = mInfo;   //技能信息
                            player.Magics[mInfo.Magic] = uMagic;   //增加技能

                            //发送封包 新技能信息
                            player.Enqueue(new S.NewMagic { Magic = uMagic.ToClientInfo() });
                        }

                        level = 1;

                        if (player.Level >= mInfo.NeedLevel2)
                            level = 2;

                        if (player.Level >= mInfo.NeedLevel3)
                            level = 3;

                        uMagic.Level = level;  //技能等级设置

                        //发送封包 增加技能和对应的等级
                        player.Enqueue(new S.MagicLeveled { InfoIndex = uMagic.Info.Index, Level = uMagic.Level, Experience = uMagic.Experience });
                    }

                    player.RefreshStats();  //刷新玩家的属性状态

                }
                else if (command == "调整技能".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;
                    if (parts.Length < 3) return;

                    if (parts.Length == 3) player = this; //@levelskill healing 5
                    else player = SEnvir.GetPlayerByCharacter(parts[1]); //@levelskill ryan healing 5

                    if (player == null) return;

                    MagicInfo tinfo = SEnvir.MagicInfoList.Binding.FirstOrDefault(m => m.Name.Replace(" ", "").ToUpper().Equals(parts[2].ToUpper()));
                    if (tinfo == null) return;

                    if (int.TryParse(parts[3], out int tlevel))
                    {
                        player.Magics[tinfo.Magic].Level = tlevel;
                        player.Magics[tinfo.Magic].Experience = 0;

                        player.Enqueue(new S.MagicLeveled { InfoIndex = tinfo.Index, Level = tlevel, Experience = 0 });
                        player.RefreshStats();
                        Connection.ReceiveChat(string.Format("PlayerObject.GMMagicLevel".Lang(Connection.Language, parts[1], parts[2], parts[3])), MessageType.System);
                    }
                }
                else if (command == "清除PK值".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //不是管理员跳开

                    if (parts.Length < 2)   //小于2个指令输入
                    {
                        player = this;  //自己
                    }
                    else
                    {
                        player = SEnvir.GetPlayerByCharacter(parts[1]);  //玩家
                    }

                    BuffInfo buff = player.Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);   //BUFF等PK值判断
                    if (buff != null) player.BuffRemove(buff);   //如果PK的BUFF不为空  删除PKBUFF
                }
                else if (command == "设置宠物".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //不是管理员跳开
                    if (parts.Length < 3) return;   //小于3个指令输入 跳开

                    Stat stat;
                    if (!int.TryParse(parts[1], out level)) return;   //等级
                    if (!Enum.TryParse(parts[2], out stat)) return;   //属性
                    if (!int.TryParse(parts[3], out castleName)) return;   //数值

                    if (Companion == null) return;   //宠物等空 跳开

                    switch (level)
                    {
                        case 3:
                            Companion.UserCompanion.Level3 = new Stats { [stat] = castleName };
                            break;
                        case 5:
                            Companion.UserCompanion.Level5 = new Stats { [stat] = castleName };
                            break;
                        case 7:
                            Companion.UserCompanion.Level7 = new Stats { [stat] = castleName };
                            break;
                        case 10:
                            Companion.UserCompanion.Level10 = new Stats { [stat] = castleName };
                            break;
                        case 11:
                            Companion.UserCompanion.Level11 = new Stats { [stat] = castleName };
                            break;
                        case 13:
                            Companion.UserCompanion.Level13 = new Stats { [stat] = castleName };
                            break;
                        case 15:
                            Companion.UserCompanion.Level15 = new Stats { [stat] = castleName };
                            break;
                    }

                    CompanionRefreshBuff();   //刷新宠物属性

                    //发送封包 修改宠物的级别
                    Enqueue(new S.CompanionSkillUpdate
                    {
                        Level3 = Companion.UserCompanion.Level3,
                        Level5 = Companion.UserCompanion.Level5,
                        Level7 = Companion.UserCompanion.Level7,
                        Level10 = Companion.UserCompanion.Level10,
                        Level11 = Companion.UserCompanion.Level11,
                        Level13 = Companion.UserCompanion.Level13,
                        Level15 = Companion.UserCompanion.Level15
                    });
                }
                else if (command == "刷怪".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //不是管理员 跳开

                    if (parts.Length < 2) return;   //小于2个指令输入 跳开

                    var monsterInfo = SEnvir.GetMonsterInfo(parts[1]);   //怪物信息

                    if (monsterInfo == null) return;   //怪物为空 跳开

                    if (parts.Length < 3 || !int.TryParse(parts[2], out castleName) || castleName == 0)  //输入指令判断
                        castleName = 1;

                    while (castleName > 0)
                    {
                        var monster = MonsterObject.GetMonster(monsterInfo);   //怪物信息

                        monster.Spawn(CurrentMap.Info, Functions.Move(CurrentLocation, Direction));  //生产刷新出怪物  怪物所在位置为当前位置
                        castleName -= 1;
                    }
                }
                else if (command == "召唤宠物".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return; //如果不是管理员 跳开

                    if (Pets.Count > 4) return;  //如果宠物数量大于4只 跳开

                    if (parts.Length < 2) return;  //判断小于3个指令输入

                    var monInfo = SEnvir.GetMonsterInfo(parts[1]);  //怪物信息

                    if (monInfo == null) return;//如果怪物信息为空 跳开

                    count = 1;   //数量1
                                 //byte petlevel = 0;  //等级0

                    if (parts.Length > 2)  //如果字节大于2   数量大于5，那么召唤的宝宝数量为0只
                        if (!int.TryParse(parts[2], out count) || count > 5) count = 1;

                    //if (parts.Length > 3)  //如果字节大于3  数量大于7，那么召唤的宝宝级别为0级
                    //if (!byte.TryParse(parts[3], out petlevel) || petlevel > 7) petlevel = 0;

                    for (int i = 0; i < count; i++)  //如果是0只，生成召唤宠物 最大5只
                    {
                        MonsterObject monster = MonsterObject.GetMonster(monInfo);  //怪物信息
                        if (monster == null) return;   //如果怪物为空 跳开
                        monster.SummonLevel = 0;   //怪物等级
                        monster.PetOwner = this;    //怪物主人= 玩家
                        monster.TameTime = SEnvir.Now.AddHours(3);  //宠物的时间
                        monster.RageTime = DateTime.MinValue;
                        monster.ShockTime = DateTime.MinValue;
                        monster.Spawn(Character.CurrentMap, CurrentLocation);   //再生  角色当前地图 当前坐标
                        Pets.Add(monster);   //宠物增加(怪物名)

                        //发送封包 给对应的角色 增加宠物
                        monster.Broadcast(new S.ObjectPetOwnerChanged { ObjectID = monster.ObjectID, PetOwner = Name });
                    }

                    Connection.ReceiveChat((string.Format("PlayerObject.GMPetOwnerChanged".Lang(Connection.Language, count, monInfo.MonsterName))), MessageType.System);
                }
                else if (command == "制作".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开

                    if (parts.Length < 2) return;  //判断小于2个指令输入

                    if (parts[1].Equals("金币".Lang(lan)) ||
                        parts[1].Equals("元宝".Lang(lan)) ||
                        parts[1].Equals("声望".Lang(lan)) ||
                        parts[1].Equals("贡献".Lang(lan)) ||
                        parts[1].Equals("经验".Lang(lan)))
                    {
                        Connection.ReceiveChat($"此类物品无法制作".Lang(lan), MessageType.System);
                        return;
                    }

                    ItemInfo item = SEnvir.GetItemInfo(parts[1]);  //获取道具信息项（第一部分）

                    if (item == null)   //如果道具等空
                    {
                        Connection.ReceiveChat($"物品不存在".Lang(lan), MessageType.System);
                        return;
                    }

                    if (parts.Length < 3 || !int.TryParse(parts[2], out castleName) || castleName == 0)
                        castleName = 1;

                    while (castleName > 0)
                    {
                        count = Math.Min(castleName, item.StackSize);

                        if (!CanGainItems(false, new ItemCheck(item, count, UserItemFlags.None, TimeSpan.Zero))) break;

                        UserItem userItem = SEnvir.CreateDropItem(item, 0);
                        //记录物品来源
                        SEnvir.RecordTrackingInfo(userItem, CurrentMap?.Info?.Description, ObjectType.None, "GM制造".Lang(Connection.Language), Character?.CharacterName);

                        userItem.Count = count;          //给予的道具数量

                        TimeSpan duration = TimeSpan.FromSeconds(item.Duration);   //duration 定义道具使用期限

                        if (duration != TimeSpan.Zero)  //如果时间使用期限不等0
                        {
                            userItem.Flags = UserItemFlags.Expirable;   //标签定义为时间限制
                            userItem.ExpireTime = duration;
                        }
                        else
                        {
                            userItem.Flags = UserItemFlags.None;   //给予的道具无标签
                        }

                        castleName -= count;

                        GainItem(userItem);  //增加道具
                    }
                }
                else if (command == "制作碎片".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开

                    if (parts.Length < 2) return;   //判断小于2个指令输入

                    ItemInfo item = SEnvir.GetItemInfo(parts[1]);  //获取道具信息项（第一部分）

                    item = SEnvir.GetItemInfo("[" + "碎片".Lang(lan) + "]");   //道具信息为碎片

                    ItemInfo item1 = SEnvir.GetItemInfo(parts[1]);

                    if (item == null || item1 == null)     //判断是否存在道具
                    {
                        Connection.ReceiveChat($"物品不存在".Lang(lan), MessageType.System);
                        return;
                    }

                    if (item1.PartCount <= 0)          //判断该道具的碎片设置
                    {
                        Connection.ReceiveChat($"PlayerObject.GMPartCount".Lang(Connection.Language, item1.ItemName), MessageType.System);
                        return;
                    }

                    if (parts.Length < 3 || !int.TryParse(parts[2], out castleName) || castleName == 0)
                        castleName = 1;

                    while (castleName > 0)
                    {
                        count = Math.Min(castleName, item.StackSize);   //道具堆叠大小

                        if (!CanGainItems(false, new ItemCheck(item, count, UserItemFlags.None, TimeSpan.Zero))) break;

                        UserItem userItem = SEnvir.CreateDropItem(item, 0);
                        //记录物品来源
                        SEnvir.RecordTrackingInfo(userItem, CurrentMap?.Info?.Description, ObjectType.None, "GM制造".Lang(Connection.Language), Character?.CharacterName);

                        userItem.Count = count;
                        userItem.Flags = UserItemFlags.None;

                        userItem.AddStat(Stat.ItemIndex, item1.Index, StatSource.None);
                        userItem.Stats[Stat.ItemIndex] = item1.Index;

                        castleName -= count;

                        GainItem(userItem);  //增加碎片道具
                    }
                }
                else if (command == "内存清理".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //如果不是管理员 跳开

                    DateTime time = Time.Now;

                    GC.Collect(2, GCCollectionMode.Forced);   //强制执行内存回收

                    Connection.ReceiveChat($"[" + "内存回收".Lang(lan) + $"] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
                }
                else if (command == "锁定清理".Lang(lan))
                { //清除所有玩家系统账号锁定
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开

                    SEnvir.IPBlocks.Clear();   //系统账号锁定 移除
                }
                else if (command == "寄售清理".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开

                    DateTime time = Time.Now;

                    time = Time.Now;

                    MarketPlaceCancelSuperior();   //取消寄售

                    Connection.ReceiveChat($"[" + "取消寄售物品" + $"] {(Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond}ms", MessageType.System);
                }
                else if (command == "重置寄售历史记录".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;
                    long c = 0;
                    for (int i = 0; i < SEnvir.AuctionHistoryInfoList.Binding.Count; i++)
                    {
                        AuctionHistoryInfo history = SEnvir.AuctionHistoryInfoList.Binding[i];
                        history.Delete();
                        c++;
                    }
                    Connection.ReceiveChat($"{c} " + "寄售历史记录重置".Lang(lan), MessageType.System);
                }
                else if (command == "清理怪物".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    Map _map = SEnvir.GetMap(Character.CurrentMap);
                    if (_map != null)
                    {
                        for (int i = _map.Objects.Count - 1; i >= 0; i--)
                        {
                            MonsterObject mob = _map.Objects[i] as MonsterObject;

                            if (mob == null) continue;

                            if (mob.PetOwner != null) continue;

                            if (mob.Dead) continue;

                            if (mob.MonsterInfo.AI == -1) continue;  //怪物AI是卫士 忽略

                            if (mob.MonsterInfo.AI == -2) continue;  //怪物AI是宠物 忽略

                            //如果(功能.距离(角色.当前位置, 怪物当前位置)>10）继续
                            //if (Functions.Distance(Character.CurrentLocation, mob.CurrentLocation) > 10) continue;

                            mob.EXPOwner = null;
                            mob.Die();
                            mob.Despawn();
                        }

                    }
                }
                else if (command == "增加元宝".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Account.GameGold += count * 100;
                    character.Account.Connection?.ReceiveChat("Payment.PaymentComplete".Lang(character.Account.Connection.Language, count), MessageType.System);
                    character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "GM命令",
                        Time = SEnvir.Now,
                        Character = character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.GMAdd,
                        Amount = count,
                        ExtraInfo = $"命令: {command}, GM角色名: {this.Character.CharacterName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);

                    if (character.Account.Referral != null)
                    {
                        character.Account.Referral.HuntGold += count / 10;

                        if (character.Account.Referral.Connection != null)
                        {
                            character.Account.Referral.Connection.ReceiveChat("Payment.ReferralPaymentComplete".Lang(character.Account.Referral.Connection.Language, count / 10), MessageType.System, 0);

                            if (character.Account.Referral.Connection.Stage == GameStage.Game)
                                character.Account.Referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.Referral.HuntGold });
                        }
                    }

                    Connection.ReceiveChat(string.Format("[" + "增加赞助币".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);

                    if (Config.EnableRewardPoolTopUp)
                    {
                        // 奖金池
                        SEnvir.RewardPoolAddBalance(character, count, "元宝充值");
                    }
                }
                else if (command == "减少元宝".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Account.GameGold -= count * 100;
                    character.Account.Connection?.ReceiveChat("Payment.PaymentFailed".Lang(character.Account.Connection.Language, count), MessageType.System);
                    character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "GM命令",
                        Time = SEnvir.Now,
                        Character = character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.GMDeduct,
                        Amount = count,
                        ExtraInfo = $"命令: {command}, GM角色名: {this.Character.CharacterName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);

                    if (character.Account.Referral != null)
                    {
                        character.Account.Referral.HuntGold -= count / 10;

                        if (character.Account.Referral.Connection != null)
                        {
                            character.Account.Referral.Connection.ReceiveChat("Payment.ReferralPaymentFailed".Lang(character.Account.Referral.Connection.Language, count / 10), MessageType.System, 0);

                            if (character.Account.Referral.Connection.Stage == GameStage.Game)
                                character.Account.Referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.Referral.HuntGold });
                        }
                    }

                    Connection.ReceiveChat(string.Format("[" + "减少赞助币".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "扣除元宝".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Account.GameGold -= count * 100;
                    character.Account.Connection?.ReceiveChat("Payment.GameGoldLost".Lang(character.Account.Connection.Language, count), MessageType.System);
                    character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                    Connection.ReceiveChat(string.Format("[" + "扣除赞助币".Lang(lan) + $"] {0} " + "数量".Lang(lan) + ": {1}", character.CharacterName, count), MessageType.System);

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "GM命令",
                        Time = SEnvir.Now,
                        Character = character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.GMDeduct,
                        Amount = count * 100,
                        ExtraInfo = $"命令: {command}, GM角色名: {this.Character.CharacterName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
                else if (command == "返还元宝".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Account.GameGold += count * 100;
                    character.Account.Connection?.ReceiveChat("Payment.GameGoldRefund".Lang(character.Account.Connection.Language, count), MessageType.System);
                    character.Player?.Enqueue(new S.GameGoldChanged { GameGold = character.Account.GameGold });

                    Connection.ReceiveChat(string.Format("[" + "返还赞助币".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "GM命令",
                        Time = SEnvir.Now,
                        Character = character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.GMAdd,
                        Amount = count * 100,
                        ExtraInfo = $"命令: {command}, GM角色名: {this.Character.CharacterName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
                else if (command == "返还赏金".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Account.HuntGold += count;
                    character.Account.Connection?.ReceiveChat("Payment.HuntGoldRefund".Lang(character.Account.Connection.Language, count), MessageType.System);
                    character.Player?.Enqueue(new S.HuntGoldChanged { HuntGold = character.Account.HuntGold });

                    Connection.ReceiveChat(string.Format("[" + "返还赏金".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "增加声望".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Prestige += count;
                    character.Player?.Enqueue(new S.PrestigeChanged { Prestige = character.Prestige });
                    Connection.ReceiveChat(string.Format("[" + "增加声望".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "减少声望".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Prestige -= count;
                    character.Player?.Enqueue(new S.PrestigeChanged { Prestige = character.Prestige });
                    Connection.ReceiveChat(string.Format("[" + "减少声望".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "增加贡献".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Contribute += count;
                    character.Player?.Enqueue(new S.ContributeChanged { Contribute = character.Contribute });
                    Connection.ReceiveChat(string.Format("[" + "增加贡献".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "减少贡献".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;

                    if (!int.TryParse(parts[2], out count)) return;

                    character.Contribute -= count;
                    character.Player?.Enqueue(new S.ContributeChanged { Contribute = character.Contribute });
                    Connection.ReceiveChat(string.Format("[" + "减少贡献".Lang(lan) + $"] {0} " + "数量".Lang(lan) + $": {1}", character.CharacterName, count), MessageType.System);
                }
                else if (command == "禁言".Lang(lan))
                {
                    if (!Character.Account.TempAdmin && !Character.Account.Admin) return;   //如果不是管理员 不是GM 跳开 
                    if (parts.Length < 2) return;   //判断小于2个指令输入

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);  //角色名字

                    if (character == null) return;   //角色为空 跳开

                    if (parts.Length < 3 || !int.TryParse(parts[2], out count))
                        count = 1440 * 365 * 10;

                    character.Account.ChatBanExpiry = SEnvir.Now.AddMinutes(count);   //禁止聊天到期时间
                }
                else if (command == "行会禁言".Lang(lan))
                {
                    if (!Character.Account.TempAdmin && !Character.Account.Admin) return;   //如果不是管理员 不是GM 跳开
                    if (parts.Length < 2) return;  //判断小于2个指令输入

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = SEnvir.GetCharacter(parts[1]);  //角色名字

                    if (character == null) return;  //角色为空

                    if (parts.Length < 3 || !int.TryParse(parts[2], out count))
                        count = 1440 * 365 * 10;

                    character.Account.GlobalTime = SEnvir.Now.AddMinutes(count); //禁止行会聊天到期时间
                }
                else if (command == "传送".Lang(lan))
                {
                    if (Dead) return;   //如果 死亡 跳过

                    if (!Character.Account.TempAdmin && Stats[Stat.TeleportRing] <= 0) return;  //如果不是GM 并且 没有传送戒指 跳过

                    if (Config.TeleportIimit)  //传送地图限制
                    {
                        //不是GM并且地图回城限制 或者 不是GM并且地图传送限制 或者 不是GM并且地图使用技能限制
                        if (!Character.Account.TempAdmin && !CurrentMap.Info.AllowTT || !Character.Account.TempAdmin && !CurrentMap.Info.AllowRT || !Character.Account.TempAdmin && CurrentMap.Info.SkillDelay > 0)
                        {
                            Connection.ReceiveChat("无法使用，该地图限制使用命令传送", MessageType.Hint);
                            return;
                        }
                    }

                    if (!Character.Account.TempAdmin && SEnvir.Now < TeleportTime)    //如果不是GM 冷却时间生效
                    {
                        Connection.ReceiveChat("无法使用，冷却时间未到", MessageType.Hint);
                        return;
                    }

                    if (parts.Length < 2) //parts[0] == @飞 
                    {
                        Teleport(CurrentMap, CurrentMap.GetRandomLocation());//传送 当前地图 获取随机位置
                        return;
                    }
                    if (parts.Length == 3 && int.TryParse(parts[1], out int xCord) && int.TryParse(parts[2], out int yCord))   //输出 X Y坐标
                    {
                        if (!Teleport(CurrentMap, new Point(xCord, yCord)))          //X Y坐标为无效点 那么传送失败
                        {
                            Connection.ReceiveChat("传送失败，位置无效", MessageType.Hint);
                        }
                        TeleportTime = SEnvir.Now.AddSeconds(Config.TeleportTime);  //命令延迟时间等秒
                    }
                    else
                    {
                        Connection.ReceiveChat("命令使用无效 @传送 X坐标 Y坐标", MessageType.Hint);
                    }
                }
                else if (command == "飞".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 2) return;   //判断小于2个指令输入

                    //MapInfo info = SEnvir.MapInfoList.Binding.FirstOrDefault(x => string.Compare(x.FileName, parts[1], StringComparison.OrdinalIgnoreCase) == 0);
                    if (!int.TryParse(parts[1], out castleName)) return;
                    MapInfo info = SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == castleName);   //指定地图的序号判断
                    Map map = SEnvir.GetMap(info);   //判断地图信息

                    if (map == null) return;  //如果地图为空 跳开

                    Teleport(map, map.GetRandomLocation());   //传送地图序号  获得随机位置
                }
                else if (command == "查询".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;   //如果不是管理员 跳开

                    if (parts.Length < 2) return;

                    player = SEnvir.GetPlayerByCharacter(parts[1]);

                    if (player == null)
                    {
                        Connection.ReceiveChat(parts[1] + "不在线".Lang(lan) + "。", MessageType.System);
                        return;
                    }
                    if (player.CurrentMap == null) return;

                    Connection.ReceiveChat((string.Format("PlayerObject.GMInquire".Lang(Connection.Language, player.Name, player.CurrentMap.Info.Description, player.CurrentLocation.X, player.CurrentLocation.Y))), MessageType.System);
                }
                else if (command == "增加属性".Lang(lan))
                {
                    if (!Character.Account.TempAdmin)
                        return;
                    if (parts.Length < 4) return;

                    if (!Enum.TryParse(parts[1], out EquipmentSlot tslot)) return;
                    if (!Enum.TryParse(parts[2], out Stat tstat)) return;
                    if (!int.TryParse(parts[3], out int tamount)) return;

                    if (Equipment[(int)tslot] != null)
                    {
                        Equipment[(int)tslot].AddStat(tstat, tamount, StatSource.Added);
                        Equipment[(int)tslot].StatsChanged();

                        SendShapeUpdate();
                        RefreshStats();
                        S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                        {
                            GridType = GridType.Equipment,
                            Slot = (int)tslot,
                            NewStats = new Stats(Equipment[(int)tslot].Stats)
                        };
                        result.FullItemStats = Equipment[(int)tslot].ToClientInfo().FullItemStats;
                        Enqueue(result);
                        Connection.ReceiveChat(string.Format("PlayerObject.GMAddStat".Lang(Connection.Language, Equipment[(int)tslot].Info.ItemName, tstat, tamount)), MessageType.System);
                    }
                }
                else if (command == "开始攻城".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 2) return;   //判断小于2个指令输入

                    if (!int.TryParse(parts[1], out castleName)) return;   //只输入一个指令 跳开

                    //if (!SEnvir.UserConquestList.Binding.Any(p => p.WarDate.Date <= SEnvir.Now.Date))//没有攻城信息
                    //{
                    //    Connection.ReceiveChat("War.NoGuild".Lang(Connection.Language), MessageType.System);
                    //    return;
                    //}

                    CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == castleName);

                    if (castle == null) return;   //如果城堡信息为空 跳开

                    if (SEnvir.ConquestWars.Any(x => x.info == castle)) return;

                    ConquestWar.StartConquestWar(castle, true);
                }
                else if (command == "结束攻城".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 2) return;   //判断小于2个指令输入

                    if (!int.TryParse(parts[1], out castleName)) return;

                    CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == castleName);

                    if (castle == null) return;   //如果城堡信息为空 跳开

                    //过期申请攻城信息清除
                    //var list = SEnvir.UserConquestList.Binding.Where(p => (p.WarDate.Date < SEnvir.Now.Date || p.WarDate.Date == SEnvir.Now.Date && SEnvir.Now.TimeOfDay > p.Castle.Duration) && p.Castle == castle);
                    //foreach (var conquest in list)
                    //{
                    //    conquest.Delete();
                    //}

                    ConquestWar war = SEnvir.ConquestWars.FirstOrDefault(x => x.info == castle);

                    if (war == null) return;  //如果攻城对象列表为空 跳开

                    war.EndTime = DateTime.MinValue;   //结束攻城时间设定

                    ConquestWar.EndConquestWar(castle, true);
                }
                else if (command == "占领城堡".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    if (parts.Length < 3) return;

                    SEnvir.ReassignCastleToGuild(parts[1], parts[2]);
                }
                else if (command == "创建行会".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开

                    if (parts.Length < 2) return;

                    CharacterInfo character = SEnvir.GetCharacter(parts[1]);

                    character = parts.Length < 3 ? Character : SEnvir.GetCharacter(parts[1]);

                    if (character == null) return;   //如果角色为空 跳开

                    if (Character.Account.GuildMember != null) return;   //如果是行会成员 跳开

                    var guildName = parts.Length < 3 ? parts[1] : parts[2];

                    if (!Globals.GuildNameRegex.IsMatch(guildName)) return;   //如果行会名字存在 跳开

                    var guildInfo = SEnvir.GuildInfoList.Binding.FirstOrDefault(
                        x => string.Compare(x.GuildName, guildName, StringComparison.OrdinalIgnoreCase) == 0);

                    if (guildInfo != null) return;   //如果行会信息为空 跳开

                    guildInfo = SEnvir.GuildInfoList.CreateNewObject();   //新建行会信息

                    guildInfo.GuildName = guildName;  //行会名字
                    guildInfo.MemberLimit = 10;       //成员数
                    guildInfo.StorageSize = 10;       //仓库容量数
                    guildInfo.GuildLevel = 1;         //行会等级

                    var memberInfo = SEnvir.GuildMemberInfoList.CreateNewObject();  //新建角色信息

                    memberInfo.Account = character.Account;   //账号
                    memberInfo.Guild = guildInfo;             //行会信息
                    memberInfo.Rank = "行会会长".Lang(lan);             //会长称号
                    memberInfo.JoinDate = SEnvir.Now;         //时间记录
                    memberInfo.Permission = GuildPermission.Leader;   //行会领导者

                    SendGuildInfo();   //发送行会信息
                }
                else if (command == "在线人数".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    Connection.ReceiveChat($"当前真实在线人数".Lang(lan) + $": {SEnvir.Players.Count}", MessageType.System);
                }
                else if (command == "重载脚本".Lang(lan))
                {
                    if (!Character.Account.TempAdmin) return;  //如果不是管理员 跳开
                    SEnvir.LoadTxtSciprts();
                }
            }
            //观察者聊天
            else if (text.StartsWith("#"))
            {
                text = string.Format("(#){0}: {1}", Name, text.Remove(0, 1));

                Connection.ReceiveChat(text, MessageType.ObserverChat);

                foreach (SConnection target in Connection.Observers)
                {
                    if (SEnvir.IsBlocking(Character.Account, target.Account)) continue;

                    target.ReceiveChat(text, MessageType.ObserverChat);
                }
            }
            //一般
            else
            {
                text = string.Format("{0}: {1}", Name, text);
                foreach (PlayerObject player in SeenByPlayers)
                {
                    if (!Functions.InRange(CurrentLocation, player.CurrentLocation, Config.MaxViewRange)) continue;

                    if (player != this && SEnvir.Now < Character.Account.ChatBanExpiry) continue;

                    if (!SEnvir.IsBlocking(Character.Account, player.Character.Account))
                    {
                        ProcessChatItems(player.Connection, linkedItems);
                        player.Connection.ReceiveChat(text, MessageType.Normal, ObjectID);
                    }

                    foreach (SConnection observer in player.Connection.Observers)
                    {
                        if (SEnvir.IsBlocking(Character.Account, observer.Account)) continue;
                        ProcessChatItems(observer, linkedItems);
                        observer.ReceiveChat(text, MessageType.Normal, ObjectID);
                    }
                }
            }
        }

        private void ProcessChatItems(SConnection player, List<ChatItemInfo> linkedItems)
        {
            if (linkedItems == null || linkedItems.Count == 0) return;
            foreach (var linkedItem in linkedItems)
            {
                UserItem[] array;
                //只处理背包，仓库，装备上的道具
                switch (linkedItem.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.Equipment:
                        array = Equipment;
                        break;
                    default:
                        continue;
                }

                UserItem item = null;

                for (int i = 0; i < array.Length; i++)
                {
                    item = array[i];
                    if (item == null || item.Index != linkedItem.Index) continue;
                    break;
                }

                if (item != null)
                {
                    if (player != null) player.Enqueue(new S.ChatItem { Item = item.ToClientInfo() });
                }
            }
        }

        public void ObserverChat(SConnection con, string text) //聊天
        {
            if (string.IsNullOrEmpty(text)) return;

            if (con.Account?.LastCharacter == null)
            {
                con.ReceiveChat("System.ObserverNotLoggedIn".Lang(con.Language), MessageType.System);
                return;
            }
            SEnvir.LogChat($"{con.Account.LastCharacter.CharacterName}: {text}");

            string[] parts;

            if (text.StartsWith("/"))
            {
                //Private Message
                text = text.Remove(0, 1);
                parts = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                SConnection target = SEnvir.GetConnectionByCharacter(parts[0]);

                if (target == null || (target.Stage != GameStage.Observer && target.Stage != GameStage.Game) || SEnvir.IsBlocking(con.Account, target.Account))
                {
                    con.ReceiveChat("System.CannotFindPlayer".Lang(con.Language, parts[0]), MessageType.System);
                    return;
                }

                if (!con.Account.TempAdmin)
                {
                    if (target.Player != null && target.Player.BlockWhisper)
                    {
                        con.ReceiveChat("System.PlayerBlockingWhisper".Lang(con.Language, parts[0]), MessageType.System);
                        return;
                    }
                }

                con.ReceiveChat($"/{text}", MessageType.WhisperOut);

                if (SEnvir.Now < con.Account.LastCharacter.Account.ChatBanExpiry) return;

                target.ReceiveChat($"{con.Account.LastCharacter.CharacterName}=> {text.Remove(0, parts[0].Length)}", Character.Account.TempAdmin ? MessageType.GMWhisperIn : MessageType.WhisperIn);
            }
            else if (text.StartsWith("!~"))
            {
                if (con.Account.GuildMember == null) return;

                text = string.Format("{0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));

                foreach (GuildMemberInfo member in con.Account.GuildMember.Guild.Members)
                {
                    if (member.Account.Connection == null) continue;
                    if (member.Account.Connection.Stage != GameStage.Game && member.Account.Connection.Stage != GameStage.Observer) continue;

                    if (SEnvir.IsBlocking(con.Account, member.Account)) continue;

                    member.Account.Connection.ReceiveChat(text, MessageType.Guild);
                }
            }
            else if (text.StartsWith("!@"))
            {
                if (!con.Account.LastCharacter.Account.TempAdmin)
                {
                    if (SEnvir.Now < con.Account.LastCharacter.Account.GlobalTime)
                    {
                        con.ReceiveChat("System.GlobalDelay".Lang(con.Language, Math.Ceiling((con.Account.GlobalTime - SEnvir.Now).TotalSeconds)), MessageType.System);
                        return;
                    }

                    if (con.Account.LastCharacter.Level < 33 && con.Account.LastCharacter.LastStats[Stat.GlobalShout] == 0)
                    {
                        con.ReceiveChat("System.GlobalLevel".Lang(con.Language), MessageType.System);
                        return;
                    }

                    con.Account.LastCharacter.Account.GlobalTime = SEnvir.Now.AddSeconds(30);
                }

                if (con.Account.LastCharacter.Account.TempAdmin)
                {
                    text = string.Format("【系统】 {0}", text.Remove(0, 2));
                }
                else
                {
                    text = string.Format("(!@){0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));
                }

                foreach (SConnection target in SEnvir.Connections)
                {
                    switch (target.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            if (SEnvir.IsBlocking(con.Account, target.Account)) continue;

                            target.ReceiveChat(text, MessageType.Global);
                            break;
                        default: continue;
                    }
                }
            }
            else if (text.StartsWith("@!"))
            {
                if (!con.Account.LastCharacter.Account.TempAdmin) return;

                text = string.Format("{0}: {1}", con.Account.LastCharacter.CharacterName, text.Remove(0, 2));

                foreach (SConnection target in SEnvir.Connections)
                {
                    switch (target.Stage)
                    {
                        case GameStage.Game:
                        case GameStage.Observer:
                            target.ReceiveChat(text, MessageType.Announcement);
                            break;
                        default: continue;
                    }
                }
            }
            else
            {
                if (SEnvir.IsBlocking(con.Account, Character.Account)) return;

                text = string.Format("(#){0}: {1}", con.Account.LastCharacter.CharacterName, text);

                Connection.ReceiveChat(text, MessageType.ObserverChat);

                foreach (SConnection target in Connection.Observers)
                {
                    if (SEnvir.IsBlocking(con.Account, target.Account)) continue;

                    target.ReceiveChat(text, MessageType.ObserverChat);
                }
            }
        }
    }
}
