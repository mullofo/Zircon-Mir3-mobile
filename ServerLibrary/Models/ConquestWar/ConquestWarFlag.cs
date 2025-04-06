using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 攻城类型夺旗玩法
    /// </summary>
    public sealed class ConquestWarFlag : ConquestWar
    {
        /// <summary>
        /// 自定义NPC 旗帜
        /// </summary>
        public CustomNPC Flag { get; private set; }
        /// <summary>
        /// 当前玩家类型
        /// </summary>
        PlayerObject Current;
        /// <summary>
        /// 开始夺旗的时间
        /// </summary>
        DateTime starTime;
        /// <summary>
        /// 占领时间
        /// </summary>
        DateTime? OccupationTime = null;

        /// <summary>
        /// 初始化城堡信息
        /// </summary>
        /// <param name="info">城堡信息</param>
        protected override void OnInit(CastleInfo info)
        {
            base.OnInit(info);

            SetCurrentGuildFlag();  //设置旗帜
        }
        /// <summary>
        /// 开始攻城战
        /// </summary>
        protected override void OnStartWar()
        {
            base.OnStartWar();

            SetCurrentGuildFlag();  //设置旗帜
        }
        /// <summary>
        /// 设置当前帮会旗帜
        /// </summary>
        void SetCurrentGuildFlag()
        {
            NPCObject oldFlag = SEnvir.GetMap(info.Map).NPCs?.FirstOrDefault(x => x.NPCInfo.Index == info.InfoID);

            if (IsWaring)
            {
                if (oldFlag != null) oldFlag.Visible = false;

                //攻城期间旗帜
                if (Flag == null)
                {
                    Flag = new CustomNPC("中立旗帜", LibraryFile.NPC, 207);  //调用的旗帜
                    Flag.Spawn(info.Map, new Point(oldFlag.CurrentLocation.X, oldFlag.CurrentLocation.Y));  //旗帜刷新指定的地图，指定的坐标
                    Flag.IsNew = true;
                    Flag.OnCall += OnClickFlag;  //单击旗帜时                  
                }
                OccupationTime = SEnvir.Now;
                Flag.Visible = true;  //显示旗帜               
            }
            else
            {
                //非攻城时间旗帜
                OccupationTime = null;
                if (Flag != null) Flag.Visible = false;
                if (oldFlag != null) oldFlag.Visible = true;
            }

            foreach (var guild in SEnvir.GuildInfoList.Binding)  //遍历行会信息列表
            {
                if (guild.Castle == info)
                {
                    if (IsWaring)
                    {
                        Flag.NPCName = $"[{guild.GuildName}]的旗帜";   //设置旗帜匹配的行会名
                        Flag?.SetImage(LibraryFile.Flag, guild.GuildFlag, guild.FlagColor);   //对应旗帜的效果和颜色
                        Flag.IsNew = false;
                    }
                    else
                    {
                        //非攻城时间显示对应的行会旗帜和效果颜色
                        oldFlag?.Broadcast(new S.UpdateNPCLook { ObjectID = oldFlag.ObjectID, NPCName = $"[{guild.GuildName}]的旗帜", Library = LibraryFile.Flag, ImageIndex = guild.GuildFlag, OverlayColor = guild.FlagColor });
                    }
                    return;
                }
            }

            if (IsWaring)
            {
                Flag?.SetImage(LibraryFile.NPC, 207, Color.White);
            }
            else
            {
                oldFlag?.Broadcast(new S.UpdateNPCLook { ObjectID = oldFlag.ObjectID, Library = LibraryFile.NPC, ImageIndex = 207, OverlayColor = Color.White });
            }
        }
        /// <summary>
        /// 旗帜保护时间信息提示计数
        /// </summary>
        public int _showTip = 1;
        /// <summary>
        /// 当前攻城时间
        /// </summary>
        DateTime _currentConquest = SEnvir.Now;
        /// <summary>
        /// 设置旗帜3分钟延迟
        /// </summary>
        void SendConquest()
        {
            //如果攻城结束就跳出
            if (!IsWaring) return;

            if (
                //占领时间等空 且 系统时间 小于等于 开始时间增加3.6分                 占领时间有值不为空 且 系统时间 小于等于 占领时间的值增加3.6分
                (OccupationTime == null && SEnvir.Now <= StartTime.AddMinutes(3.2) || OccupationTime.HasValue && SEnvir.Now <= OccupationTime.Value.AddMinutes(3.2))//开始或夺旗
                                                                                                                                                                    //系统时间 减去 当前攻城时间 的值 总秒数 大于等于18        信息提示计数小于11 且 （占领时间等空 且 系统时间 大于等于 开始时间增加3.6分）    占领时间有值不为空 且 系统时间 大于等于 占领时间的值增加3.6分
                && ((SEnvir.Now - _currentConquest).TotalSeconds >= 18 || _showTip < 11 && (OccupationTime == null && SEnvir.Now >= StartTime.AddMinutes(3.6) || OccupationTime.HasValue && SEnvir.Now >= OccupationTime.Value.AddMinutes(3.2)))//时间差
                )
            {
                showConquest(_showTip);
                _currentConquest = SEnvir.Now;
                _showTip++;
            }
        }
        /// <summary>
        /// 设置旗帜延迟通知
        /// </summary>
        void showConquest(int showTip)
        {
            switch (showTip)
            {
                case 1:
                    foreach (SConnection con in SEnvir.Connections)
                    {
                        con.ReceiveChat("Conquest.ConquestMsg1".Lang(con.Language, info.Name), MessageType.Shout);
                        con.ReceiveChat("Conquest.ConquestMsg2".Lang(con.Language), MessageType.Shout);
                    }
                    break;
                case 2:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg3".Lang(con.Language), MessageType.Shout);
                    break;
                case 3:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg4".Lang(con.Language), MessageType.Shout);
                    break;
                case 4:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg5".Lang(con.Language), MessageType.Shout);
                    break;
                case 5:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg6".Lang(con.Language), MessageType.Shout);
                    break;
                case 6:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg7".Lang(con.Language), MessageType.Shout);
                    break;
                case 7:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg8".Lang(con.Language), MessageType.Shout);
                    break;
                case 8:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg9".Lang(con.Language), MessageType.Shout);
                    break;
                case 9:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg10".Lang(con.Language), MessageType.Shout);
                    break;
                case 10:
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestMsg11".Lang(con.Language), MessageType.Shout);
                    break;
                case 11:
                    foreach (SConnection con in SEnvir.Connections)
                    {
                        con.ReceiveChat("Conquest.ConquestMsg12".Lang(con.Language), MessageType.Shout);
                        con.ReceiveChat("Conquest.ConquestMsg13".Lang(con.Language), MessageType.Shout);
                    }
                    _showTip = 1;
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// 单击旗帜标识时
        /// </summary>
        /// <param name="player"></param>
        private void OnClickFlag(PlayerObject player)
        {
            if (!IsWaring) return;  //不在攻城时间
            if (Current != null) return;  //当前角色为空
            if (player == null) return;   //玩家为空
            if (!ValidPlayer(player)) return;   //不是能抢旗帜的玩家

            starTime = SEnvir.Now;  //开始时间等系统时间
            Current = player;   //判断是否能抢旗的玩家
            Flag.NPCName = "中立旗帜(争夺中)";
            Flag.Current = player;
            Flag.SetImage(LibraryFile.NPC, 207, Color.White);

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat($"勇士 {player.Name} 开始夺旗! 坚持{Config.FlagCaptureTime}秒占领城池!", MessageType.RollNotice);

            Flag.Broadcast(new S.ConquestWarFlagFightStarted
            {
                MapIndex = info.Map.Index,
                StartTime = starTime,
                EndTime = starTime + TimeSpan.FromSeconds(Config.FlagCaptureTime),  //结束时间等于开始时间加夺旗时间
                DelayTime = TimeSpan.FromSeconds(Config.FlagCaptureTime)
            });
        }
        /// <summary>
        /// 是否能抢旗帜的玩家判断
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        bool ValidPlayer(PlayerObject player)
        {
            if (player.Connection == null) return false;  //玩家等空
            if (player.Dead) return false;  //玩家死亡
            if (player.Character.Account.GuildMember == null)  //玩家的行会等空
            {
                player.Connection.ReceiveChat("你没有行会，不能夺旗", MessageType.System);
                return false;
            }
            if (player.Character.Account.GuildMember.Guild.Castle != null) return false;   //城堡信息不为空

            if (Participants.Count > 0 && !Participants.Contains(player.Character.Account.GuildMember.Guild)) //参与的行会
            {
                player.Connection.ReceiveChat("你的行会没有参与攻城，不能夺旗", MessageType.System);
                return false;
            }
            if ((player.Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader)  //不是行会老大
            {
                player.Connection.ReceiveChat("你不是行会老大，不能夺旗", MessageType.System);
                return false;
            }
            if (Functions.Distance(player.CurrentLocation, Flag.CurrentLocation) != 1) return false; //不是在旗帜范围1格内

            if (Config.ConquestFlagDelay)
            {
                if (IsWaring && (OccupationTime.HasValue && (SEnvir.Now - OccupationTime.Value).TotalMinutes <= 3 || OccupationTime == null && (SEnvir.Now - StartTime).TotalMinutes <= 3))
                {
                    player.Connection.ReceiveChat("旗帜准备中，不能夺旗", MessageType.System);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 夺旗处理过程中
        /// </summary>
        public override void OnProcess()
        {
            try
            {
                //夺旗以后延迟30分钟，没人抢直接结束攻城
                if (IsWaring && Flag != null && Flag.Visible && OccupationTime.HasValue && (SEnvir.Now - OccupationTime.Value).TotalMinutes >= Config.VictoryDelay)
                {
                    EndWar();
                    return;
                }

                base.OnProcess();

                if (Config.ConquestFlagDelay)
                {
                    SendConquest();
                }

                if (IsWaring && Current != null)  //攻城中 且 角色不为空
                {
                    if (!ValidPlayer(Current))  //当前角色不能抢旗帜
                    {
                        //Current.Connection?.ReceiveChat("夺旗失败!", MessageType.System);
                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat($"夺旗失败!夺旗失败!!夺旗失败!!!", MessageType.System);
                        SetCurrentGuildFlag();
                        Current = null;
                        Flag.Broadcast(new S.ConquestWarFlagFightEnd());
                        return;
                    }

                    if (SEnvir.Now > starTime + TimeSpan.FromSeconds(Config.FlagCaptureTime))  //当前时间大于 开始夺旗时间加夺旗过程时间
                    {
                        ClearOwner();   //清除原来的所有者
                        Current.Character.Account.GuildMember.Guild.Castle = info;  //设置行会信息

                        OccupationTime = SEnvir.Now;

                        SetCurrentGuildFlag();   //设置旗帜

                        if (Config.ConquestFlagDelay)
                        {
                            _showTip = 1;
                        }

                        if (Config.FlagSuccessTeleport)
                        {
                            PingPlayers();  //夺旗成功把其他行会玩家传送走
                        }

                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat("Conquest.ConquestCapture".Lang(con.Language, Current.Character.Account.GuildMember.Guild.GuildName, info.Name), MessageType.System);
                        SEnvir.Broadcast(new S.GuildCastleInfo { Index = info.Index, Owner = Current.Character.Account.GuildMember.Guild.GuildName });
                        Current = null;
                        return;
                    }
                }
            }
            catch
            {
                SEnvir.Log($"ConquestWarFlag.OnProcess夺旗处理过程发生错误。");
            }
        }
        /// <summary>
        /// 结束攻城
        /// </summary>
        protected override void OnEndWar()
        {
            base.OnEndWar();
            SetCurrentGuildFlag();   //设置旗帜
        }
    }
}
