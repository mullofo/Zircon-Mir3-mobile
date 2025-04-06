using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 攻城
    /// </summary>
    public abstract class ConquestWar
    {
        /// <summary>
        /// 所有类型
        /// </summary>
        static readonly List<Type> allTypes = typeof(ConquestWar).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ConquestWar))).ToList();
        /// <summary>
        /// 战争
        /// </summary>
        public static readonly Dictionary<CastleInfo, ConquestWar> wars = new Dictionary<CastleInfo, ConquestWar>();
        /// <summary>
        /// 是否初始化
        /// </summary>
        static bool IsInit;
        /// <summary>
        /// 初始化
        /// </summary>
        static void Init()
        {
            IsInit = true;   //初始化开启

            //遍历 城堡信息
            foreach (CastleInfo info in SEnvir.CastleInfoList.Binding)
            {
                var name = "ConquestWar" + info.Type;
                var type = allTypes.Find(t => t.Name == name);
                if (type == null) continue;
                var ins = (ConquestWar)Activator.CreateInstance(type);  //激活方法为攻城类型
                ins.OnInit(info);
                wars.Add(info, ins);
            }
        }
        /// <summary>
        /// 初始化过程
        /// </summary>
        public static void Process()
        {
            try
            {
                if (!IsInit) Init();
                foreach (var p in wars)
                {
                    p.Value.OnProcess();
                }
            }
            catch
            {
                SEnvir.Log($"ConquestWar.Process攻城初始化过程发生错误。");
            }
        }
        /// <summary>
        /// 开始攻城
        /// </summary>
        /// <param name="info"></param>
        /// <param name="force"></param>
        public static void StartConquestWar(CastleInfo info, bool force)
        {
            if (wars.TryGetValue(info, out var war)) war.StartWar(force);
        }
        /// <summary>
        /// 结束攻城
        /// </summary>
        /// <param name="info"></param>
        public static void EndConquestWar(CastleInfo info, bool force)
        {
            if (wars.TryGetValue(info, out var war)) war.EndWar(force);
        }
        /// <summary>
        /// 城堡信息
        /// </summary>
        public CastleInfo info { get; private set; }
        /// <summary>
        /// 开始攻城时间
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// 结束攻城时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 参与行会信息
        /// </summary>
        public List<GuildInfo> Participants { get; private set; }
        /// <summary>
        /// 攻城指定地图
        /// </summary>
        public Map Map { get; private set; }
        /// <summary>
        /// 是否在攻城
        /// </summary>
        public bool IsWaring { get; private set; }
        /// <summary>
        /// 是否GM命令结束攻城
        /// </summary>
        public bool GmEndWar { get; private set; }
        /// <summary>
        /// 攻城角色统计
        /// </summary>
        public Dictionary<CharacterInfo, UserConquestStats> Stats = new Dictionary<CharacterInfo, UserConquestStats>();

        /// <summary>
        /// 初始化时城堡信息
        /// </summary>
        /// <param name="info"></param>
        protected virtual void OnInit(CastleInfo info)
        {
            this.info = info;
        }

        /// <summary>
        /// 处理中
        /// </summary>
        public virtual void OnProcess()
        {
            try
            {
                ////检查当天有没有攻城，没有直接return
                var canWar = SEnvir.UserConquestList.Binding.Any(p => p.WarDate.Date == SEnvir.Now.Date);
                if (!canWar)
                {
                    if (!GmEndWar && IsWaring)
                    {
                        EndWar();
                    }
                    return;
                }
                //没有攻城 时间大于等于指定攻城时间 时间小于结束时间 开启攻城
                if (!IsWaring && SEnvir.Now.TimeOfDay >= info.StartTime && SEnvir.Now.TimeOfDay < info.Duration)
                {
                    StartWar();
                }

                if (!GmEndWar)  //如果不是GM命令结束攻城
                {
                    if (IsWaring && (SEnvir.Now.TimeOfDay >= info.Duration))  //攻城中 时间大于等于结束时间 结束攻城
                    {
                        EndWar();
                    }
                }
            }
            catch
            {
                SEnvir.Log($"ConquestWar.OnProcess攻城处理过程发生错误。");
            }
        }
        /// <summary>
        /// 开始攻城
        /// </summary>
        /// <param name="forced"></param>
        public void StartWar(bool forced = false)
        {
            if (IsWaring) return;   //如果是攻城状态 跳出
            IsWaring = true;   //设置攻城状态开启
            StartTime = SEnvir.Now;   //攻城的时间等于现在的时间
            Map = SEnvir.GetMap(info.Map);  //地图等于设置的攻城地图信息
            GmEndWar = forced;   //是否GM命令结束攻城赋值一个变量

            Participants = new List<GuildInfo>();   //赋值参与行会信息的变量

            if (!forced)
            {
                foreach (UserConquest conquest in SEnvir.UserConquestList.Binding)
                {
                    if (conquest.Castle != info) continue;
                    if (conquest.WarDate > SEnvir.Now.Date) continue;
                    Participants.Add(conquest.Guild);
                }

                if (Participants.Count == 0) return;

                foreach (GuildInfo guild in SEnvir.GuildInfoList.Binding)
                {
                    if (guild.Castle != info) continue;
                    Participants.Add(guild);
                }
            }
            else
            {
                //如果是GM强制开启攻城 那么所有行会均可参加
                foreach (GuildInfo guild in SEnvir.GuildInfoList.Binding)
                {
                    Participants.Add(guild);
                }
            }

            Stats.Clear();

            SEnvir.ConquestWars.Add(this);

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat("Conquest.ConquestStarted".Lang(con.Language, info.Name), MessageType.System);

            Map = SEnvir.GetMap(info.Map);

            for (int i = Map.NPCs.Count - 1; i >= 0; i--)
            {
                NPCObject npc = Map.NPCs[i];

                if (npc.NPCInfo?.WarDisplay == true) continue;
                if (npc.NPCInfo?.Image >= 999) continue;

                npc.Visible = false;
                npc.RemoveAllObjects();
            }

            //foreach (GuildInfo guild in Participants)
            //guild.Conquest?.Delete();

            Point flagLocation = Point.Empty;
            if (this is ConquestWarFlag warFlag)
            {
                flagLocation = warFlag.Flag?.CurrentLocation ?? Point.Empty;
            }

            SEnvir.Broadcast(new S.GuildConquestStarted { Index = info.Index, FlagLocation = flagLocation });

            PingPlayers();

            OnStartWar();
        }
        /// <summary>
        /// 开始战争时
        /// </summary>
        protected virtual void OnStartWar() { }
        /// <summary>
        /// 结束战争时
        /// </summary>
        protected virtual void OnEndWar() { }

        /// <summary>
        /// 结束攻城
        /// </summary>
        public void EndWar(bool forced = false)
        {
            try
            {
                if (!IsWaring) return;
                IsWaring = false;
                GmEndWar = forced;

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat("Conquest.ConquestFinished".Lang(con.Language, info.Name), MessageType.System);

                IsWaring = false;

                for (int i = Map.NPCs.Count - 1; i >= 0; i--)
                {
                    NPCObject npc = Map.NPCs[i];
                    //if (!Castle.CastleRegion.PointList.Contains(npc.CurrentLocation)) continue;
                    if (npc.NPCInfo?.WarDisplay == true) continue;
                    if (npc.NPCInfo?.Image >= 999) continue;

                    npc.Visible = true;
                    npc.AddAllObjects();
                }

                if (Config.EndWarTeleport)
                {
                    PingPlayers();  //攻城结束把玩家传送走
                }

                SEnvir.ConquestWars.Remove(this);

                SEnvir.Broadcast(new S.GuildConquestFinished { Index = info.Index });

                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == info);

                if (ownerGuild != null)
                {
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Conquest.ConquestOwner".Lang(con.Language, ownerGuild.GuildName, info.Name), MessageType.System);

                    UserConquest conquest = SEnvir.UserConquestList.Binding.FirstOrDefault(x => x.Castle == info && x.Castle == ownerGuild?.Castle);

                    TimeSpan warTime = TimeSpan.MinValue;

                    if (conquest != null)
                        warTime = (conquest.WarDate + conquest.Castle.StartTime) - SEnvir.Now;

                    foreach (GuildMemberInfo member in ownerGuild.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = info.Index, WarTime = warTime, ObserverPacket = false });
                    }
                }

                foreach (GuildInfo participant in Participants)
                {
                    if (participant == ownerGuild) continue;

                    foreach (GuildMemberInfo member in participant.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildConquestDate { Index = info.Index, WarTime = TimeSpan.MinValue, ObserverPacket = false });
                        //刷新行会buff和城堡buff
                        //member.Account.Connection?.Player.ApplyCastleBuff();
                        member.Account.Connection?.Player.ApplyGuildBuff();
                    }
                }

                OnEndWar();

                //过期申请攻城信息清除

                var list = SEnvir.UserConquestList.Binding.Where(p => p.WarDate.Date <= SEnvir.Now.Date).ToList();
                foreach (var conquest in list)
                {
                    conquest.Delete();
                }

            }
            catch
            {
                SEnvir.Log($"ConquestWar.EndWar攻城结束过程发生错误。");
            }
        }
        /// <summary>
        /// 清除所有者
        /// </summary>
        public void ClearOwner()
        {
            GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == info);
            if (ownerGuild != null)
                ownerGuild.Castle = null;
        }
        /// <summary>
        /// 传送攻城玩家
        /// </summary>
        public void PingPlayers()
        {
            foreach (PlayerObject player in Map.Players)
            {
                //if (!Castle.CastleRegion.PointList.Contains(player.CurrentLocation)) continue;

                if (player.Character.Account.GuildMember?.Guild?.Castle == info) continue;

                player.Teleport(info.AttackSpawnRegion);
            }
        }
        /// <summary>
        /// 获得角色参与攻城的统计信息
        /// </summary>
        /// <param name="character">角色信息</param>
        /// <returns></returns>
        public UserConquestStats GetStat(CharacterInfo character, Language lan)
        {
            UserConquestStats user;

            if (!Stats.TryGetValue(character, out user))
            {
                user = SEnvir.UserConquestStatsList.CreateNewObject();

                user.Character = character;

                user.WarStartDate = StartTime;
                user.CastleName = info.Name;

                user.GuildName = character.Account.GuildMember?.Guild.GuildName ?? "没有行会".Lang(lan);
                user.CharacterName = character.CharacterName;
                user.Class = character.Class;
                user.Level = character.Level;

                Stats[character] = user;
            }

            return user;
        }
    }
}
