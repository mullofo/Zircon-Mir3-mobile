using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 地图对象
    /// </summary>
    public abstract class MapObject
    {
        /// <summary>
        /// 对象ID值
        /// </summary>
        public uint ObjectID { get; }
        /// <summary>
        /// 对象类型种类
        /// </summary>
        public abstract ObjectType Race { get; }

        /// <summary>
        /// 匿名函数 阻塞 如果死亡 
        /// </summary>
        public virtual bool Blocking => !Dead;
        /// <summary>
        /// 名字
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public virtual int Level { get; set; }
        /// <summary>
        /// 当前单元格
        /// </summary>
        public Cell CurrentCell
        {
            get { return _CurrentCell; }
            set
            {
                if (_CurrentCell == value) return;

                var oldValue = _CurrentCell;
                _CurrentCell = value;

                LocationChanged(oldValue, value);
            }
        }
        private Cell _CurrentCell;
        /// <summary>
        /// 当前地图
        /// </summary>
        public Map CurrentMap
        {
            get { return _CurrentMap; }
            set
            {
                if (_CurrentMap == value) return;

                var oldValue = _CurrentMap;
                _CurrentMap = value;

                MapChanged(oldValue, value);
            }
        }
        private Map _CurrentMap;

        public List<KeyValuePair<string, string>> NPCVar = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// 当前坐标
        /// </summary>
        public virtual Point CurrentLocation { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public virtual MirDirection Direction { get; set; }
        /// <summary>
        /// 显示暴击
        /// </summary>
        public bool DisplayCrit;
        /// <summary>
        /// 显示MISS
        /// </summary>
        public bool DisplayMiss;
        /// <summary>
        /// 显示抵抗
        /// </summary>
        public bool DisplayResist;
        /// <summary>
        /// 显示挡住
        /// </summary>
        public bool DisplayBlock;
        /// <summary>
        /// 显示致命一击
        /// </summary>
        public bool DisplayFatalAttack;
        /// <summary>
        /// 显示会心一击
        /// </summary>
        public bool DisplayCriticalHit;
        /// <summary>
        /// 显示暴捶效果
        /// </summary>
        public bool DisplayDamageAdd;
        /// <summary>
        /// 显示爆毒效果
        /// </summary>
        public bool DisplayGreenPosionPro;
        /// <summary>
        /// 显示抽蓝效果
        /// </summary>
        public bool DisplaySmokingMP;
        /// <summary>
        /// 显示HP
        /// </summary>
        public int DisplayHP;
        /// <summary>
        /// 显示MP
        /// </summary>
        public int DisplayMP;
        /// <summary>
        /// 当前HP
        /// </summary>
        public virtual int CurrentHP { get; set; }
        /// <summary>
        /// 当前MP
        /// </summary>
        public virtual int CurrentMP { get; set; }
        /// <summary>
        /// 刷怪
        /// </summary>
        public bool Spawned;
        /// <summary>
        /// 死亡
        /// </summary>
        public bool Dead;
        /// <summary>
        /// 反隐身几率
        /// </summary>
        public bool CoolEye;
        /// <summary>
        /// 激活
        /// </summary>
        public bool Activated;
        /// <summary>
        /// 在安全区
        /// </summary>
        public bool InSafeZone;
        /// <summary>
        /// 冻伤免疫时间
        /// </summary>
        public DateTime FrostBiteImmunity;
        /// <summary>
        /// 行为时间
        /// </summary>
        public DateTime ActionTime;
        /// <summary>
        /// 移动时间
        /// </summary>
        public DateTime MoveTime;
        /// <summary>
        /// 跑动时间
        /// </summary>
        public DateTime RuningTime;
        /// <summary>
        /// 移动距离
        /// </summary>
        public int RuningSetps;
        /// <summary>
        /// 复活时间
        /// </summary>
        public DateTime RegenTime;
        /// <summary>
        /// 攻击时间
        /// </summary>
        public DateTime AttackTime;

        public DateTime SpeedDelay;
        /// <summary>
        /// 施法时间
        /// </summary>
        public DateTime MagicTime;
        /// <summary>
        /// 单元时间
        /// </summary>
        public DateTime CellTime;
        /// <summary>
        /// 打击时间
        /// </summary>
        public DateTime StruckTime;
        /// <summary>
        /// BUFF时间
        /// </summary>
        public DateTime BuffTime;
        /// <summary>
        /// 被诱惑时间
        /// </summary>
        public DateTime ShockTime;
        /// <summary>
        /// 显示HPMP时间
        /// </summary>
        public DateTime DisplayHPMPTime;
        /// <summary>
        /// 道具复活时间
        /// </summary>
        public DateTime ItemReviveTime;
        /// <summary>
        /// 延迟动作列表
        /// </summary>
        public List<DelayedAction> ActionList;
        /// <summary>
        /// 可以移动
        /// </summary>
        public virtual bool CanMove => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= MoveTime && SEnvir.Now > ShockTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.StunnedStrike) != PoisonType.StunnedStrike && (Poison & PoisonType.ThousandBlades) != PoisonType.ThousandBlades && (Poison & PoisonType.WraithGrip) != PoisonType.WraithGrip && isBuffCanMove;
        /// <summary>
        /// 可以攻击
        /// </summary>
        public virtual bool CanAttack => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= AttackTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.StunnedStrike) != PoisonType.StunnedStrike && (Poison & PoisonType.ThousandBlades) != PoisonType.ThousandBlades && _buffs.All(x => x.Type != BuffType.DragonRepulse && x.Type != BuffType.FrostBite);
        /// <summary>
        /// 可以投射
        /// </summary>
        public virtual bool CanCast => !Dead && SEnvir.Now >= ActionTime && SEnvir.Now >= MagicTime && (Poison & PoisonType.Paralysis) != PoisonType.Paralysis && (Poison & PoisonType.StunnedStrike) != PoisonType.StunnedStrike && (Poison & PoisonType.ThousandBlades) != PoisonType.ThousandBlades && (Poison & PoisonType.Silenced) != PoisonType.Silenced && _buffs.All(x => x.Type != BuffType.DragonRepulse && x.Type != BuffType.FrostBite);
        /// <summary>
        /// BUFF信息
        /// </summary>
        public List<BuffInfo> Buffs { get => _buffs; set => _buffs = value; }
        /// <summary>
        /// 施法对象列表
        /// </summary>
        public List<SpellObject> SpellList = new List<SpellObject>();
        /// <summary>
        /// 地图对象 节点
        /// </summary>
        public LinkedListNode<MapObject> Node;
        /// <summary>
        /// 是BUFF可以移动
        /// </summary>
        private bool isBuffCanMove = true;
        /// <summary>
        /// BUFF列表
        /// </summary>
        private List<BuffInfo> _buffs;
        /// <summary>
        /// 属性状态
        /// </summary>
        public Stats Stats;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visible;
        /// <summary>
        /// 对文档或选定文本进行排序
        /// </summary>
        public bool PreventSpellCheck;
        /// <summary>
        /// 吸血
        /// </summary>
        public decimal LifeSteal;
        /// <summary>
        /// 吸蓝
        /// </summary>
        public decimal WitchcraftSteal;
        /// <summary>
        /// 回血回蓝延迟时间
        /// </summary>
        public TimeSpan RegenDelay;
        /// <summary>
        /// 名字颜色
        /// </summary>
        public Color NameColour;
        /// <summary>
        /// 附近的玩家
        /// </summary>
        public List<PlayerObject> NearByPlayers;
        /// <summary>
        /// 被玩家看到
        /// </summary>
        public List<PlayerObject> SeenByPlayers;
        /// <summary>
        /// 玩家看到的数据
        /// </summary>
        public List<PlayerObject> DataSeenByPlayers;
        /// <summary>
        /// 毒类型
        /// </summary>
        public PoisonType Poison;
        /// <summary>
        /// 毒类型列表
        /// </summary>
        public List<Poison> PoisonList;
        /// <summary>
        /// 组队成员列表
        /// </summary>
        public List<PlayerObject> GroupMembers;
        /// <summary>
        /// 地图对象
        /// </summary>
        protected MapObject()
        {
            ObjectID = SEnvir.ObjectID;

            NameColour = Color.White;
            Visible = true;
            RegenDelay = TimeSpan.FromSeconds(10);
            BuffTime = SEnvir.Now;

            NearByPlayers = new List<PlayerObject>();
            SeenByPlayers = new List<PlayerObject>();

            ActionList = new List<DelayedAction>();
            _buffs = new List<BuffInfo>();
            PoisonList = new List<Poison>();
            DataSeenByPlayers = new List<PlayerObject>();
        }
        /// <summary>
        /// 启动进程
        /// </summary>
        public void StartProcess()
        {
            DeActivate();

            //杂项
            for (int i = ActionList.Count - 1; i >= 0; i--)
            {
                if (SEnvir.Now < ActionList[i].Time) continue;

                DelayedAction ac = ActionList[i];
                ActionList.RemoveAt(i);
                try
                {
                    ProcessAction(ac);
                }
                catch (Exception ex)
                {
                    SEnvir.Log($"过程操作 {ac.ToString()} 未知异常: 名字：{Name}  动作：{ac.Type.ToString()}  朝向：{ac.Data[0].ToString()}");
                    SEnvir.Log(ex.Message);
                    SEnvir.Log(ex.StackTrace);
                }
            }

            ProcessGroupBuff();
            ProcessBuff();
            ProcessPoison();
            Process();

            ProcessHPMP();

            Color oldColour = NameColour;
            ProcessNameColour();

            if (oldColour != NameColour)
                Broadcast(new S.ObjectNameColour { ObjectID = ObjectID, Colour = NameColour });

        }
        /// <summary>
        /// 过程
        /// </summary>
        public virtual void Process()
        {

        }
        /// <summary>
        /// 处理HP MP过程
        /// </summary>
        public virtual void ProcessHPMP()
        {
            if (SEnvir.Now < DisplayHPMPTime) return;

            DisplayHPMPTime = SEnvir.Now.AddMilliseconds(50);

            bool changed = false;
            if (DisplayHP != CurrentHP || DisplayBlock || DisplayCrit || DisplayMiss || DisplayFatalAttack || DisplayCriticalHit || DisplayDamageAdd || DisplayGreenPosionPro || DisplaySmokingMP)
            {
                int change = CurrentHP - DisplayHP;

                Broadcast(new S.HealthChanged { ObjectID = ObjectID, Change = change, Critical = DisplayCrit, Miss = DisplayMiss, Block = DisplayBlock, FatalAttack = DisplayFatalAttack, CriticalHit = DisplayCriticalHit, DamageAdd = DisplayDamageAdd, GreenPosionPro = DisplayGreenPosionPro, SmokingMP = DisplaySmokingMP });

                DisplayHP = CurrentHP;

                DisplayMiss = false;
                DisplayBlock = false;
                DisplayCrit = false;
                DisplayFatalAttack = false;
                DisplayCriticalHit = false;
                DisplayDamageAdd = false;
                DisplayGreenPosionPro = false;
                DisplaySmokingMP = false;

                changed = true;
            }

            if (DisplayMP != CurrentMP)
            {
                int change = CurrentMP - DisplayMP;
                Broadcast(new S.ManaChanged { ObjectID = ObjectID, Change = change });
                DisplayMP = CurrentMP;

                changed = true;
            }

            if (!changed) return;

            S.DataObjectHealthMana p = new S.DataObjectHealthMana { ObjectID = ObjectID, Health = DisplayHP, Mana = DisplayMP, Dead = Dead };

            foreach (PlayerObject player in DataSeenByPlayers)
            {
                player.Enqueue(p);

                //如果GM模式无敌
                if (player.GameMaster)
                {
                    player.CurrentHP = player.Stats[Stat.Health];
                    player.CurrentMP = player.Stats[Stat.Mana];
                }
            }
        }
        /// <summary>
        /// 处理中毒状态
        /// </summary>
        public virtual void ProcessPoison()
        {
            PoisonType current = PoisonType.None;


            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                Poison poison = PoisonList[i];
                if (poison.Owner?.Node == null || poison.Owner.Dead || poison.Owner.CurrentMap != CurrentMap)// || !Functions.InRange(poison.Owner.CurrentLocation, CurrentLocation, Config.MaxViewRange))
                {
                    PoisonList.Remove(poison);
                    continue;
                }

                if (poison.Owner.Race == ObjectType.Player && Race == ObjectType.Player)
                {
                    PlayerObject poj = poison.Owner as PlayerObject;
                    if (poj.AttackMode == AttackMode.Peace)
                    {
                        PoisonList.Remove(poison);
                        continue;
                    }
                }

                current |= poison.Type;

                if (SEnvir.Now < poison.TickTime) continue;
                if (poison.TickCount-- <= 0) PoisonList.RemoveAt(i);
                poison.TickTime = SEnvir.Now + poison.TickFrequency;

                //bool infection = false;
                int damage = 0;
                MonsterObject mob;
                switch (poison.Type)
                {
                    case PoisonType.Green:
                        damage += poison.Value;
                        if (poison.ExtraValue != 0 && SEnvir.Now < poison.ExtraTickTime)
                        {
                            damage *= poison.ExtraValue;
                        }
                        break;
                    case PoisonType.ThousandBlades:
                        damage += poison.Value;
                        break;
                    case PoisonType.WraithGrip:
                        ChangeMP(-poison.Value);

                        if (poison.Extra != null)
                            poison.Owner.ChangeMP(poison.Value * (((UserMagic)poison.Extra).Level + 1));
                        break;
                    case PoisonType.HellFire:
                        damage += poison.Value;
                        break;
                    case PoisonType.ElectricShock:
                        damage += poison.Value;
                        break;
                    case PoisonType.Infection:  //传染
                        if (Race == ObjectType.Player)
                            damage += 1 + Stats[Stat.Health] / 100;
                        else
                        {
                            damage += poison.Value;

                            for (int x = 0; x < poison.Owner.Stats[Stat.Rebirth]; x++)
                                damage = (int)(damage * 1.5F);
                        }

                        //infection = true;

                        if (Race == ObjectType.Monster && poison.Owner.Race == ObjectType.Player)
                        {
                            mob = (MonsterObject)this;

                            if (mob.EXPOwner == null)
                                mob.EXPOwner = (PlayerObject)poison.Owner;
                        }

                        if (poison.TickCount <= 0) break;

                        foreach (MapObject ob in poison.Owner.GetTargets(CurrentMap, CurrentLocation, 1))
                        {
                            //if (ob.Race != ObjectType.Monster) continue;


                            if (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.IsBoss) continue;

                            if (ob.PoisonList.Any(x => x.Type == PoisonType.Infection)) continue;

                            ob.ApplyPoison(new Poison
                            {
                                Value = poison.Value,
                                Owner = poison.Owner,
                                TickCount = poison.TickCount,
                                TickFrequency = poison.TickFrequency,
                                Type = poison.Type,
                                TickTime = poison.TickTime
                            });
                        }
                        break;
                }

                if (Stats[Stat.Invincibility] > 0)
                    damage = 0;

                if (damage > 0)
                {
                    if (Config.PoisonDead)
                    {
                        //if (Race == ObjectType.Monster && ((MonsterObject)this).MonsterInfo.IsBoss && !Config.PoisoningBossCheck)
                        //damage = 0;
                        //else
                        if (Race == ObjectType.Monster)
                            damage = Math.Min(CurrentHP - 1, damage);
                    }
                    else
                    {
                        damage = Math.Min(CurrentHP - 1, damage);
                    }

                    if (damage > 0)
                    {
                        #region 战争统计

                        UserConquestStats conquest;

                        switch (Race)
                        {
                            case ObjectType.Player:
                                conquest = SEnvir.GetConquestStats((PlayerObject)this);

                                if (conquest != null)
                                {
                                    switch (poison.Owner.Race)
                                    {
                                        case ObjectType.Player:
                                            conquest.PvPDamageTaken += damage;

                                            conquest = SEnvir.GetConquestStats((PlayerObject)poison.Owner);

                                            if (conquest != null)
                                                conquest.PvPDamageDealt += damage;
                                            break;
                                        case ObjectType.Monster:
                                            mob = (MonsterObject)poison.Owner;

                                            if (mob is CastleLord)
                                                conquest.BossDamageTaken += damage;
                                            else if (mob.PetOwner != null)
                                            {
                                                conquest.PvPDamageTaken += damage;

                                                conquest = SEnvir.GetConquestStats(mob.PetOwner);

                                                if (conquest != null)
                                                    conquest.PvPDamageDealt += damage;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case ObjectType.Monster:
                                mob = (MonsterObject)this;

                                if (mob is CastleLord)
                                {
                                    switch (poison.Owner.Race)
                                    {
                                        case ObjectType.Player:
                                            conquest = SEnvir.GetConquestStats((PlayerObject)poison.Owner);

                                            if (conquest != null)
                                                conquest.BossDamageDealt += damage;
                                            break;
                                        case ObjectType.Monster:

                                            mob = (MonsterObject)poison.Owner;
                                            if (mob.PetOwner != null)
                                            {
                                                conquest = SEnvir.GetConquestStats(mob.PetOwner);

                                                if (conquest != null)
                                                    conquest.BossDamageDealt += damage;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                        #endregion

                        ChangeHP(-damage);
                    }
                    if (Dead)
                    {
                        if (CurrentMap.Info.Fight == FightSetting.Safe || CurrentMap.Info.Fight == FightSetting.Fight || InSafeZone)
                            break;
                        PlayerObject murder = null, victim = null;
                        if (poison == null) break;
                        if (Race == ObjectType.Player)//如果被杀死的是人物
                        {
                            murder = poison.Owner as PlayerObject;
                            victim = this as PlayerObject;

                        }
                        else if (Race == ObjectType.Monster && (this as MonsterObject).PetOwner != null)//如果被杀死的是宠物
                        {
                            murder = poison.Owner as PlayerObject;
                            victim = (this as MonsterObject).PetOwner as PlayerObject;
                        }
                        else break;
                        if (murder == null) break;
                        BuffInfo buff;
                        int rate;
                        TimeSpan time;
                        if (victim.AtWar(murder))  //如果是在（战争中的攻击者）  || CurrentMap.Info.Fight == FightSetting.War
                        {
                            foreach (GuildMemberInfo member in victim.Character.Account.GuildMember.Guild.Members)  //行会成员
                            {
                                if (member.Account.Connection == null) continue;

                                //member.Account.Connection.ReceiveChat("Guild.GuildWarDeath".Lang(member.Account.Connection.Language, Name, victim.Character.Account.GuildMember.Guild.GuildName, murder.Name, murder.Character.Account.GuildMember.Guild.GuildName), MessageType.System);

                                //foreach (SConnection con in SEnvir.Connections)
                                //con.ReceiveChat("Guild.GuildWarDeath".Lang(con.Language, Name, victim.Character.Account.GuildMember.Guild.GuildName, murder.Name, murder.Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                            }
                            foreach (GuildMemberInfo member in murder.Character.Account.GuildMember.Guild.Members)  //攻击方行会成员
                            {
                                if (member.Account.Connection == null) continue;

                                //member.Account.Connection.ReceiveChat("Guild.GuildWarDeath".Lang(member.Account.Connection.Language, Name, victim.Character.Account.GuildMember.Guild.GuildName, murder.Name, murder.Character.Account.GuildMember.Guild.GuildName), MessageType.System); 
                            }
                            foreach (SConnection con in SEnvir.Connections)
                                con.ReceiveChat("Guild.GuildWarDeath".Lang(con.Language, Name, victim.Character.Account.GuildMember.Guild.GuildName, murder.Name, murder.Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                        }
                        else
                        {
                            if (Stats[Stat.PKPoint] < Config.RedPoint && Stats[Stat.Brown] == 0)  //如果 PK值小于200 或者 不是灰名
                            {
                                victim.Connection.ReceiveChat("System.MurderedBy".Lang(victim.Connection.Language, murder.Name), MessageType.System); //出提示被杀死了
                                foreach (SConnection con in victim.Connection.Observers)
                                    con.ReceiveChat("System.MurderedBy".Lang(con.Language, murder.Name), MessageType.System);

                                //PvP击杀对方
                                if (murder.Stats[Stat.PKPoint] >= Config.RedPoint && SEnvir.Random.Next(Config.PvPCurseRate) == 0)  //如果PK值大于200 或者 诅咒随机值等0
                                {
                                    if (Config.PVPLuckCheck)
                                    {
                                        int luck = Config.MaxCurse;  //定义 幸运值等最大诅咒值
                                        //判断攻击者武器装备格子没有武器    判断攻击者武器的幸运等于最大诅咒值
                                        if (murder.Equipment[(int)EquipmentSlot.Weapon] != null && (murder.Equipment[(int)EquipmentSlot.Weapon].Stats[Stat.Luck] != luck))
                                        {
                                            //发包更新攻击者武器属性值变化
                                            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                                            murder.Enqueue(result);

                                            murder.Equipment[(int)EquipmentSlot.Weapon].AddStat(Stat.Luck, -1, StatSource.Enhancement);  //攻击者武器增加幸运值-1
                                            murder.Equipment[(int)EquipmentSlot.Weapon].StatsChanged();                                  //攻击者武器属性值变化
                                            result.NewStats[Stat.Luck]--;                                    //结果 新属性信息[幸运]--

                                            murder.Stats[Stat.Luck]--;                                     //攻击者属性值运行--

                                            //刷新完整属性列表
                                            result.FullItemStats = murder.Equipment[(int)EquipmentSlot.Weapon].ToClientInfo().FullItemStats;
                                        }
                                    }
                                    else
                                    {
                                        //增加诅咒BUFF -1点 一个小时
                                        rate = -1;
                                        time = Config.PvPCurseDuration;
                                        buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PvPCurse);

                                        if (buff != null)
                                        {
                                            rate += buff.Stats[Stat.Luck];
                                            time += buff.RemainingTime;
                                        }

                                        murder.BuffAdd(BuffType.PvPCurse, time, new Stats { [Stat.Luck] = rate }, false, false, TimeSpan.Zero);
                                    }

                                    murder.Connection.ReceiveChat("System.Curse".Lang(murder.Connection.Language, Name), MessageType.System);  //出提示厄运伴随着你
                                    foreach (SConnection con in murder.Connection.Observers)
                                        con.ReceiveChat("System.Murdered".Lang(con.Language, Name), MessageType.System);    //出提示你杀死了谁
                                }
                                else
                                {
                                    if (SEnvir.Random.Next(5) == 0)
                                    {
                                        int luck = Config.MaxCurse;  //定义 幸运值等最大诅咒值
                                        //判断攻击者武器装备格子没有武器       判断攻击者武器的幸运等于最大诅咒值
                                        if (murder.Equipment[(int)EquipmentSlot.Weapon] != null && (murder.Equipment[(int)EquipmentSlot.Weapon].Stats[Stat.Luck] != luck))
                                        {
                                            //发包更新攻击者武器属性值变化
                                            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                                            murder.Enqueue(result);

                                            murder.Equipment[(int)EquipmentSlot.Weapon].AddStat(Stat.Luck, -1, StatSource.Enhancement);  //攻击者武器增加幸运值-1
                                            murder.Equipment[(int)EquipmentSlot.Weapon].StatsChanged();                                  //攻击者武器属性值变化
                                            result.NewStats[Stat.Luck]--;                                    //结果 新属性信息[幸运]--

                                            murder.Stats[Stat.Luck]--;                                     //攻击者属性值运行--

                                            //刷新完整属性列表
                                            result.FullItemStats = murder.Equipment[(int)EquipmentSlot.Weapon].ToClientInfo().FullItemStats;
                                        }
                                    }
                                    murder.Connection.ReceiveChat("System.Murdered".Lang(murder.Connection.Language, Name), MessageType.System);  //出提示你杀死了谁
                                    foreach (SConnection con in murder.Connection.Observers)
                                        con.ReceiveChat("System.Murdered".Lang(con.Language, Name), MessageType.System);
                                }

                                murder.IncreasePKPoints(Config.PKPointRate);
                            }
                        }
                        break;
                    };

                    RegenTime = SEnvir.Now + RegenDelay;
                    ShockTime = DateTime.MinValue;
                }
            }

            if (current == Poison) return;

            Poison = current;
            Broadcast(new S.ObjectPoison { ObjectID = ObjectID, Poison = Poison });
        }
        /// <summary>
        /// 名字颜色处理过程
        /// </summary>
        public virtual void ProcessNameColour()
        {

        }
        /// <summary>
        /// 处理组队BUFF
        /// </summary>
        public virtual void ProcessGroupBuff()
        {

            var player = this as PlayerObject;

            if (player == null) return;

            var check = true;
            if (GroupMembers == null) check = false;  //如果没有组队跳过
            if (Config.AutoTrivial && player.setConfArr[(int)AutoSetConf.SetAutoOnHookBox])
                check = false;

            if (!check)
            {
                if (_buffs.Any(p => p.Type == BuffType.Group))
                {
                    BuffRemove(BuffType.Group);
                }
                return;
            }

            if (!_buffs.Any(p => p.Type == BuffType.Group))
            {
                player.ApplyGroupBuff();
            }
        }

        /// <summary>
        /// BUFF处理过程
        /// </summary>
        public virtual void ProcessBuff()
        {
            try
            {
                if (_buffs == null || _buffs.Count < 1) return;

                TimeSpan ticks = SEnvir.Now - BuffTime;

                BuffTime = SEnvir.Now;
                List<BuffInfo> expiredBuffs = new List<BuffInfo>();

                foreach (BuffInfo buff in _buffs)
                {
                    int amount;
                    PlayerObject player;

                    UserMagic magic;

                    if (buff.Pause) continue;

                    switch (buff.Type)
                    {
                        case BuffType.Companion:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            player = (PlayerObject)this;

                            if (!player.InSafeZone || player.Companion.UserCompanion.Level < 15)
                                player.Companion.UserCompanion.Hunger--;

                            if (player.Companion.LevelInfo.MaxExperience > 0)
                            {
                                int highest = player.Character.Account.Companions.Max(x => x.Level);

                                if (highest <= player.Companion.UserCompanion.Level)
                                    highest = 1;

                                player.Companion.UserCompanion.Experience += highest + Stats[Stat.CompanionRate];

                                if (player.Companion.UserCompanion.Experience >= player.Companion.LevelInfo.MaxExperience)
                                {
                                    player.Companion.UserCompanion.Experience = 0;
                                    player.Companion.UserCompanion.Level++;
                                    player.Companion.CheckSkills();
                                    player.Companion.RefreshStats();
                                }
                            }

                            player.Companion.AutoFeed();

                            player.Enqueue(new S.CompanionUpdate
                            {
                                Level = player.Companion.UserCompanion.Level,
                                Experience = player.Companion.UserCompanion.Experience,
                                Hunger = player.Companion.UserCompanion.Hunger,
                            });

                            if (player.Companion.UserCompanion.Hunger <= 0)
                                expiredBuffs.Add(buff);
                            break;
                        case BuffType.Heal:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            amount = Math.Min(buff.Stats[Stat.Healing], buff.Stats[Stat.HealingCap]);

                            ChangeHP(amount);
                            buff.Stats[Stat.Healing] -= amount;

                            if (CurrentHP < Stats[Stat.Health] && buff.Stats[Stat.Healing] > 0)
                            {
                                if (Race == ObjectType.Player)
                                    ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                            }
                            else
                                expiredBuffs.Add(buff);

                            break;
                        case BuffType.Cloak:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            amount = buff.Stats[Stat.CloakDamage];

                            if (amount >= CurrentHP)
                            {
                                expiredBuffs.Add(buff);
                                break;
                            }

                            ChangeHP(-amount);
                            break;
                        case BuffType.DarkConversion:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            amount = buff.Stats[Stat.DarkConversion];

                            if (amount > CurrentMP)
                            {
                                expiredBuffs.Add(buff);
                                break;
                            }

                            ChangeMP(-amount);
                            ChangeHP(amount * 2);
                            if (Race != ObjectType.Player) break;

                            player = (PlayerObject)this;

                            if (!player.Magics.TryGetValue(MagicType.DarkConversion, out magic)) break;
                            player.LevelMagic(magic);
                            break;
                        case BuffType.PKPoint:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickFrequency = Config.PKPointTickRate;

                            buff.TickTime += buff.TickFrequency;

                            buff.Stats[Stat.PKPoint]--;

                            RefreshStats();

                            if (buff.Stats[Stat.PKPoint] <= 0)
                                expiredBuffs.Add(buff);
                            else
                            {
                                if (Race == ObjectType.Player)
                                    ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                            }
                            break;
                        case BuffType.HuntGold:
                            buff.TickTime -= ticks;

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            player = this as PlayerObject;

                            if (player != null)
                            {
                                if (SEnvir.ConquestWars.Any(war => war.Map == CurrentMap))
                                {
                                    player.Enqueue(new S.HuntGoldChanged { HuntGold = ++player.Character.Account.HuntGold });
                                    continue;
                                }
                            }

                            if (buff.Stats[Stat.AvailableHuntGold] >= buff.Stats[Stat.AvailableHuntGoldCap]) continue;

                            buff.Stats[Stat.AvailableHuntGold]++;

                            if (Race == ObjectType.Player)
                                ((PlayerObject)this).Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                            break;
                        case BuffType.DragonRepulse:
                            buff.TickTime -= ticks;

                            if (buff.RemainingTime != TimeSpan.MaxValue)
                            {
                                buff.RemainingTime -= ticks;

                                if (buff.RemainingTime <= TimeSpan.Zero)
                                    expiredBuffs.Add(buff);
                            }

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 5);

                            switch (Race)
                            {
                                case ObjectType.Player:
                                    player = (PlayerObject)this;

                                    if (!player.Magics.TryGetValue(MagicType.DragonRepulse, out magic)) break;
                                    //player.LevelMagic(magic);

                                    for (int c = cells.Count - 1; c >= 0; c--)
                                    {
                                        Cell cell = cells[c];
                                        if (cell.Objects == null) continue;

                                        if (Functions.Distance4Directions(CurrentLocation, cell.Location) > 5) continue;

                                        for (int o = cell.Objects.Count - 1; o >= 0; o--)
                                        {
                                            MapObject ob = cell.Objects[o];

                                            if (!CanAttackTarget(ob)) continue;


                                            ActionList.Add(new DelayedAction(
                                                SEnvir.Now.AddMilliseconds(SEnvir.Random.Next(200) + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 20),
                                                ActionType.DelayMagic,
                                                new List<UserMagic> { magic },
                                                ob));
                                        }
                                    }
                                    break;
                                case ObjectType.Monster:
                                    for (int c = cells.Count - 1; c >= 0; c--)
                                    {
                                        Cell cell = cells[c];
                                        if (cell.Objects == null) continue;

                                        if (Functions.Distance4Directions(CurrentLocation, cell.Location) > 5) continue;

                                        for (int o = cell.Objects.Count - 1; o >= 0; o--)
                                        {
                                            MapObject ob = cell.Objects[o];

                                            if (!CanAttackTarget(ob)) continue;

                                            ActionList.Add(new DelayedAction(
                                                SEnvir.Now.AddMilliseconds(400),
                                                ActionType.DelayMagic,
                                                MagicType.DragonRepulse,
                                                ob));
                                        }
                                    }
                                    break;
                            }
                            break;
                        case BuffType.FrostBite:
                            buff.RemainingTime -= ticks;

                            if (buff.RemainingTime > TimeSpan.Zero) continue;

                            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.FrostBiteEnd });

                            switch (Race)
                            {
                                case ObjectType.Player:
                                    player = (PlayerObject)this;

                                    if (!player.Magics.TryGetValue(MagicType.FrostBite, out magic)) break;
                                    //player.LevelMagic(magic);

                                    foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, 3))
                                    {
                                        if (!CanAttackTarget(ob)) continue;

                                        if (ob.Race != ObjectType.Monster) continue;

                                        MonsterObject mob = (MonsterObject)ob;

                                        if (mob.MonsterInfo.IsBoss) continue;

                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(SEnvir.Random.Next(500)),
                                            ActionType.DelayedMagicDamage,
                                            new List<UserMagic> { magic },
                                            ob,
                                            true,
                                            buff.Stats,
                                            0));
                                    }
                                    break;
                            }

                            FrostBiteImmunity = SEnvir.Now.AddSeconds(1);

                            expiredBuffs.Add(buff);
                            break;
                        case BuffType.ElementalHurricane:
                            buff.TickTime -= ticks;

                            if (buff.RemainingTime != TimeSpan.MaxValue)
                            {
                                buff.RemainingTime -= ticks;

                                if (buff.RemainingTime <= TimeSpan.Zero)
                                    expiredBuffs.Add(buff);
                            }

                            if (buff.TickTime > TimeSpan.Zero) continue;

                            buff.TickTime += buff.TickFrequency;

                            switch (Race)
                            {
                                case ObjectType.Player:
                                    {
                                        player = (PlayerObject)this;

                                        if (!player.Magics.TryGetValue(MagicType.ElementalHurricane, out magic)) break;
                                        int cost = magic.Cost;

                                        if (cost > CurrentMP)
                                        {
                                            expiredBuffs.Add(buff);
                                        }
                                        ChangeMP(-cost);

                                        for (int i = 1; i <= 8; i++)
                                        {
                                            Point location = Functions.Move(CurrentLocation, Direction, i);
                                            Cell cell = CurrentMap.GetCell(location);

                                            ActionList.Add(new DelayedAction(
                                                SEnvir.Now,
                                                ActionType.DelayMagic,
                                                new List<UserMagic> { magic },
                                                cell,
                                                true));


                                            switch (Direction)
                                            {
                                                case MirDirection.Up:
                                                case MirDirection.Right:
                                                case MirDirection.Down:
                                                case MirDirection.Left:
                                                    ActionList.Add(new DelayedAction(
                                                        SEnvir.Now,
                                                        ActionType.DelayMagic,
                                                        new List<UserMagic> { magic },
                                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -2))),
                                                        false));
                                                    ActionList.Add(new DelayedAction(
                                                        SEnvir.Now,
                                                        ActionType.DelayMagic,
                                                        new List<UserMagic> { magic },
                                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 2))),
                                                        false));
                                                    break;
                                                case MirDirection.UpRight:
                                                case MirDirection.DownRight:
                                                case MirDirection.DownLeft:
                                                case MirDirection.UpLeft:
                                                    ActionList.Add(new DelayedAction(
                                                        SEnvir.Now,
                                                        ActionType.DelayMagic,
                                                        new List<UserMagic> { magic },
                                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, 1))),
                                                        false));
                                                    ActionList.Add(new DelayedAction(
                                                        SEnvir.Now,
                                                        ActionType.DelayMagic,
                                                        new List<UserMagic> { magic },
                                                        CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(Direction, -1))),
                                                        false));
                                                    break;
                                            }
                                        }
                                        break;
                                    }
                            }
                            break;
                        default:
                            // 其他任何类型的buff
                            // 不清楚为何不计算TickTime 只看RemainingTime
                            if (buff.RemainingTime == TimeSpan.MaxValue) continue;

                            buff.RemainingTime -= ticks;

                            if (buff.RemainingTime > TimeSpan.Zero) continue;

                            expiredBuffs.Add(buff);
                            break;
                    }
                }

                //for (int i = expiredBuffs.Count - 1; i >= 0; i++)
                //BuffRemove(expiredBuffs[i]);
                foreach (BuffInfo buff in expiredBuffs)
                    BuffRemove(buff);
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
        /// <summary>
        /// 处理行为动作过程
        /// </summary>
        /// <param name="action"></param>
        public virtual void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.BroadCastPacket:
                    Broadcast((Packet)action.Data[0]);
                    break;
            }
        }
        /// <summary>
        /// 刷怪 地图区域
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool Spawn(MapRegion region)
        {
            if (region == null) return false;

            Map map = SEnvir.GetMap(region.Map);

            if (map == null) return false;

            if (region.PointList.Count == 0) return false;

            for (int i = 0; i < 20; i++)
                if (Spawn(region.Map, region.PointList[SEnvir.Random.Next(region.PointList.Count)])) return true;

            //SEnvir.Log($"刷新怪物失败, 未能在20次内找到可用的格子");
            return false;
        }

        public bool Spawn(int MapID, int x, int y, int radius)
        {
            Map map = SEnvir.GetMap(MapID);
            if (map == null) return false;

            int adjustedRadius = radius;
            Point point = new Point(x + SEnvir.Random.Next(-radius, radius), y + SEnvir.Random.Next(-radius, radius));

            for (int i = 0; i < 20; i++)
            {
                if (Spawn(map.Info, point)) return true;
                adjustedRadius = (int)(adjustedRadius / 1.2);
                point = new Point(x + SEnvir.Random.Next(-radius, radius), y + SEnvir.Random.Next(-radius, radius));
            }

            for (int i = 0; i < 20; i++)
                if (Spawn(map.Info, map.GetRandomLocation())) return true;

            SEnvir.Log($"刷新怪物失败, 未能在20次内找到可用的格子");
            return false;
        }

        /// <summary>
        /// 刷怪 地图信息 地图坐标
        /// </summary>
        /// <param name="info"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool Spawn(MapInfo info, Point location)
        {
            if (Node != null)
                throw new InvalidOperationException("节点不为空，对象已生成显示");

            if (info == null) return false;

            Map map;

            if (!SEnvir.Maps.TryGetValue(info, out map)) return false;

            if (Race == ObjectType.Player && info.MinimumLevel > Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;
            if (Race == ObjectType.Player && info.MaximumLevel > 0 && info.MaximumLevel < Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;

            Cell cell = map.GetCell(location);

            if (cell == null) return false;

            CurrentCell = cell;

            Spawned = true;
            Node = SEnvir.Objects.AddLast(this);

            OnSpawned();

            return true;
        }
        /// <summary>
        /// 地图改变时
        /// </summary>
        /// <param name="oMap"></param>
        /// <param name="nMap"></param>
        protected void MapChanged(Map oMap, Map nMap)
        {
            oMap?.RemoveObject(this);
            nMap?.AddObject(this);

            OnMapChanged();
        }
        /// <summary>
        /// 坐标改变时
        /// </summary>
        /// <param name="oCell"></param>
        /// <param name="nCell"></param>
        public void LocationChanged(Cell oCell, Cell nCell)
        {
            oCell?.RemoveObject(this);
            nCell?.AddObject(this);

            OnLocationChanged();
        }
        /// <summary>
        /// 位置改变时
        /// </summary>
        protected virtual void OnLocationChanged()
        {
            CellTime = SEnvir.Now.AddMilliseconds(300);

            BuffRemove(BuffType.PoisonousCloud);

            if (CurrentCell != null)
            {
                if (!PreventSpellCheck) CheckSpellObjects();

                S.DataObjectLocation p = new S.DataObjectLocation { ObjectID = ObjectID, MapIndex = CurrentCell.Map.Info.Index, CurrentLocation = CurrentLocation };

                foreach (PlayerObject player in DataSeenByPlayers)
                    player.Enqueue(p);
            }
        }
        /// <summary>
        /// 检查施法对象
        /// </summary>
        public virtual void CheckSpellObjects()
        {
            Cell cell = CurrentCell;

            for (int i = CurrentCell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = CurrentCell.Objects[i];
                if (Dead) break;
                if (ob.Race != ObjectType.Spell) continue;

                ((SpellObject)ob).ProcessSpell(this);

                if (cell != CurrentCell) break;
            }
        }
        /// <summary>
        /// 地图变化
        /// </summary>
        protected virtual void OnMapChanged() { }
        /// <summary>
        /// 刷怪变化
        /// </summary>
        protected virtual void OnSpawned() { }
        /// <summary>
        /// 附近传送
        /// </summary>
        /// <param name="minDistance"></param>
        /// <param name="maxDistance"></param>
        public void TeleportNearby(int minDistance, int maxDistance)
        {
            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, minDistance, maxDistance);

            if (cells.Count == 0) return;

            Teleport(CurrentMap, cells[SEnvir.Random.Next(cells.Count)].Location);
        }
        /// <summary>
        /// 传送 地图区域
        /// </summary>
        /// <param name="region"></param>
        /// <param name="leaveEffect"></param>
        /// <returns></returns>
        public bool Teleport(MapRegion region, bool leaveEffect = true)
        {
            Map map = SEnvir.GetMap(region.Map);

            Point point = region.PointList.Count > 0 ? region.PointList[SEnvir.Random.Next(region.PointList.Count)] : map.GetRandomLocation();

            return Teleport(map, point, leaveEffect);
        }
        /// <summary>
        /// 传送 地图指定坐标
        /// </summary>
        /// <param name="map"></param>
        /// <param name="location"></param>
        /// <param name="leaveEffect"></param>
        /// <returns></returns>
        public virtual bool Teleport(Map map, Point location, bool leaveEffect = true)
        {
            if (Race == ObjectType.Player && map.Info.MinimumLevel > Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;
            if (Race == ObjectType.Player && map.Info.MaximumLevel > 0 && map.Info.MaximumLevel < Level && !((PlayerObject)this).Character.Account.TempAdmin) return false;

            Cell cell = map?.GetCell(location);

            if (cell == null || cell.Movements != null) return false;

            if (Race == ObjectType.Player)
            {
                ((PlayerObject)this).FishingInterrupted();
            }

            if (leaveEffect)
                Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.TeleportOut });

            CurrentCell = cell.GetMovement(this);
            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.TeleportIn });

            return true;
        }
        /// <summary>
        /// 添加所有对象
        /// </summary>
        public virtual void AddAllObjects()
        {
            foreach (PlayerObject ob in CurrentMap.Players)
            {
                if (CanBeSeenBy(ob))  //可见
                    ob.AddObject(this);

                if (IsNearBy(ob))  //就在附近
                    ob.AddNearBy(this);
            }

            foreach (PlayerObject ob in SEnvir.Players)
            {
                if (CanDataBeSeenBy(ob))
                    ob.AddDataObject(this);
            }
        }
        /// <summary>
        /// 删除所有对象
        /// </summary>
        public virtual void RemoveAllObjects()
        {
            for (int i = SeenByPlayers.Count - 1; i >= 0; i--)
            {
                if (CanBeSeenBy(SeenByPlayers[i])) continue;

                SeenByPlayers[i].RemoveObject(this);
            }

            for (int i = DataSeenByPlayers.Count - 1; i >= 0; i--)
            {
                if (CanDataBeSeenBy(DataSeenByPlayers[i])) continue;

                DataSeenByPlayers[i].RemoveDataObject(this);
            }

            for (int i = NearByPlayers.Count - 1; i >= 0; i--)
            {
                if (IsNearBy(NearByPlayers[i])) continue;

                NearByPlayers[i].RemoveNearBy(this);
            }
        }
        /// <summary>
        /// 激活
        /// </summary>
        public virtual void Activate()
        {
            if (Activated) return;

            if (NearByPlayers.Count == 0) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        /// <summary>
        /// 取消激活
        /// </summary>
        public virtual void DeActivate()
        {
            if (!Activated) return;

            if (NearByPlayers.Count > 0 && ActionList.Count == 0) return;

            Activated = false;
            SEnvir.ActiveObjects.Remove(this);
        }
        /// <summary>
        /// 数据可以被看到
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool CanDataBeSeenBy(PlayerObject ob)
        {
            if (ob == this) return true;

            if (CurrentMap == null || ob.CurrentMap == null) return false;

            switch (Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)this;
                    if (player.Observer) return false;

                    if (InGroup(ob)) return true;

                    if (player.InGuild(ob)) return true;

                    if (player.Character?.Partner?.Player == ob) return true;

                    if (ob.CurrentMap == CurrentMap && ob.Stats[Stat.PlayerTracker] > 0) return true;

                    break;
                case ObjectType.Monster:

                    MonsterObject mob = (MonsterObject)this;

                    if (ob.CurrentMap == CurrentMap && mob.MonsterInfo.IsBoss && ob.Stats[Stat.BossTracker] > 0) return true;
                    break;
            }

            return CanBeSeenBy(ob);
        }
        /// <summary>
        /// 可以被看到
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool CanBeSeenBy(PlayerObject ob)
        {
            if (ob == null) return false;
            if (ob == this) return true;

            if (CurrentMap != ob.CurrentMap)
                return false;

            if (!Functions.InRange(CurrentLocation, ob.CurrentLocation, Config.MaxViewRange))
                return false;

            if ((CurrentMap.Info.Index == 247 || CurrentMap.Info.Index == 248 || CurrentMap.Info.Index == 249 || CurrentMap.Info.Index == 250 || CurrentMap.Info.Index == 251) && (!Functions.InRange(CurrentLocation, ob.CurrentLocation, 16)))
                return false;

            if ((CurrentMap.Info.Index == 371 || CurrentMap.Info.Index == 372 || CurrentMap.Info.Index == 373 || CurrentMap.Info.Index == 374 || CurrentMap.Info.Index == 565 || CurrentMap.Info.Index == 566 || CurrentMap.Info.Index == 567 || CurrentMap.Info.Index == 579) && (!Functions.InRange(CurrentLocation, ob.CurrentLocation, 12)))
                return false;

            if (Race == ObjectType.Player && ((PlayerObject)this).Observer) return false;

            if (ob.Character.Account.TempAdmin)  //临时管理员
                return true;

            if (_buffs.Any(x => x.Type == BuffType.Cloak || x.Type == BuffType.Transparency))
            {
                if (InGroup(ob))
                    return true;

                if (Race == ObjectType.Player)
                {
                    PlayerObject player = (PlayerObject)this;

                    if (player.Observer) return false;

                    if (player.InGuild(ob)) return true;

                    if (player.Character?.Partner?.Player == ob) return true;
                }

                //if (ob.Level < Level || !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.CloakRange))
                return false;
            }

            return true;
        }
        /// <summary>
        /// 在附近的
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool IsNearBy(PlayerObject ob)
        {
            if (ob == this) return true;

            return CurrentMap == ob.CurrentMap && Functions.InRange(CurrentLocation, ob.CurrentLocation, Config.MaxViewRange);
        }
        /// <summary>
        /// 移除对象
        /// </summary>
        public void Despawn()
        {
            if (Node == null)
            {
                SEnvir.Log($"删除MapObject时出错 类型: {EnumHelp.GetDescription(this.Race)}, 名称: {this.Name}");
                throw new InvalidOperationException("节点为空，对象已取消显示");
            }

            CurrentMap = null;
            CurrentCell = null;

            RemoveAllObjects();  //删除所有对象

            //清除列表

            Node.List.Remove(Node);
            Node = null;

            if (Activated)  //激活
            {
                Activated = false;
                SEnvir.ActiveObjects.Remove(this);
            }

            OnDespawned();  //已生成

            CleanUp(); //清理
        }
        /// <summary>
        /// 安全刷新
        /// </summary>
        public void SafeDespawn()
        {
            CurrentMap = null;
            CurrentCell = null;

            RemoveAllObjects();

            if (Node != null)
            {
                Node.List.Remove(Node);
                Node = null;
            }

            OnSafeDespawn();

            CleanUp();
        }
        /// <summary>
        /// 清理
        /// </summary>
        public virtual void CleanUp()
        {
            ActionList?.Clear();

            SpellList?.Clear();

            _buffs?.Clear();

            NearByPlayers?.Clear();

            SeenByPlayers?.Clear();

            DataSeenByPlayers?.Clear();

            PoisonList?.Clear();

            GroupMembers?.Clear();

            isBuffCanMove = true;
        }
        /// <summary>
        /// 已经生成
        /// </summary>
        public virtual void OnDespawned()
        {
            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
        }
        /// <summary>
        /// 安全刷新
        /// </summary>
        public virtual void OnSafeDespawn()
        {
            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
        }
        /// <summary>
        /// 刷新属性状态
        /// </summary>
        public virtual void RefreshStats() { }
        /// <summary>
        /// 获得物品放置位置（距离数，玩家）
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual Cell GetDropLocation(int distance, PlayerObject player)
        {
            if (CurrentMap == null || CurrentCell == null) return null;  //如果 单前地图空 单前单元格空 返回空

            Cell bestCell = null;  //单元 最佳单元 空
            int layers = 0;   //层 为 0

            //如果 D=0  D小于或者等于距离数  D++ 
            for (int d = 1; d <= distance; d++)    //修改D=1，丢弃物品时道具出现在左上角
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y];

                        if (cell == null || cell.Movements != null) continue;

                        bool blocking = false;

                        int count = 0;

                        if (cell.Objects != null)
                            foreach (MapObject ob in cell.Objects)
                            {
                                if (ob.Blocking)
                                {
                                    blocking = true;
                                    break;
                                }

                                if (ob.Race != ObjectType.Item) continue;

                                if (player != null && !ob.CanBeSeenBy(player)) continue;

                                count++;
                            }

                        if (blocking) continue;

                        if (count == 0) return cell;

                        if (bestCell != null && count >= layers) continue;

                        bestCell = cell;
                        layers = count;
                    }
                }
            }

            if (bestCell == null || layers >= Config.DropLayers) return null;  //最佳的单元格为空 并且 层大于或者等于层设置  返回空

            return bestCell;
        }
        /// <summary>
        /// 设置HP数值
        /// </summary>
        /// <param name="amount"></param>
        public void SetHP(int amount)
        {
            if (Dead) return;   //如果 死亡 返回

            CurrentHP = Math.Min(amount, Stats[Stat.Health]);  //当前HP = 数值最小值（数值，状态生命值）

            if (CurrentHP <= 0 && !Dead)    //如果 当前HP小于或等于0  并且 没有死亡（比方复活了）
            {
                if (_buffs.Any(x => x.Type == BuffType.CelestialLight))      //如果BUFF是阴阳法环
                {
                    CelestialLightActivate();   //阴阳法环激活
                    return;                     //返回
                }
                if (Stats[Stat.ItemReviveTime] > 0 && SEnvir.Now >= ItemReviveTime)   //如果 复活冷却大于0  并且 系统时间大于或等于 复活冷却时间
                {
                    ItemRevive();   //物品复活
                    return;         //返回
                }

                Die();  //死亡
            }
        }
        /// <summary>
        /// HP数值改变
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeHP(int amount)
        {
            if (Dead) return;

            if (amount < 0 && Stats[Stat.ProtectionRing] > 0)
            {
                if (CurrentMP >= Math.Abs(amount))
                {
                    ChangeMP(amount);
                    return;
                }

                if (CurrentMP > 0)
                {
                    amount += CurrentMP; //数量为负，所以-100+15剩余法力=85点伤害
                    SetMP(0);
                }
            }

            if (CurrentHP + amount > Stats[Stat.Health])
                amount = Stats[Stat.Health] - CurrentHP;

            CurrentHP += amount;

            if (CurrentHP <= 0 && !Dead)
            {
                if (_buffs.Any(x => x.Type == BuffType.CelestialLight))
                {
                    CelestialLightActivate();
                    return;
                }
                if (Stats[Stat.ItemReviveTime] > 0 && SEnvir.Now >= ItemReviveTime)
                {
                    ItemRevive();
                    return;
                }

                Die();
            }
        }
        /// <summary>
        /// 设置MP
        /// </summary>
        /// <param name="amount"></param>
        public void SetMP(int amount)
        {
            CurrentMP = Math.Min(amount, Stats[Stat.Mana]);
        }
        /// <summary>
        /// MP数值改变时
        /// </summary>
        /// <param name="amount"></param>
        public void ChangeMP(int amount)
        {
            if (CurrentMP + amount > Stats[Stat.Mana])
                amount = Stats[Stat.Mana] - CurrentMP;

            CurrentMP += amount;
        }
        /// <summary>
        /// 阴阳法环激活
        /// </summary>
        public virtual void CelestialLightActivate()
        {
            CurrentHP = Stats[Stat.Health] * (Stats[Stat.CelestialLight] + Stats[Stat.Rehydrations]) / 100;  //当前HP= 生命值* 阴阳法环设置值/100
            BuffRemove(BuffType.CelestialLight);  //删除 阴阳法环BUFF
        }
        /// <summary>
        /// 道具复活
        /// </summary>
        public virtual void ItemRevive()
        {
            CurrentHP = Stats[Stat.Health];   //当前HP=生命值
            CurrentMP = Stats[Stat.Mana];     //当前MP=魔法值
            ItemReviveTime = SEnvir.Now.AddSeconds(Stats[Stat.ItemReviveTime]);  //道具复活时间= 复活冷却时间 秒
        }
        /// <summary>
        /// 清理地图对象
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual int Purify(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead) return 0;

            int result = 0;

            List<BuffInfo> buffs = new List<BuffInfo>();

            if (CanHelpTarget(ob))
            {
                result += ob.PoisonList.Count;

                for (int i = ob.PoisonList.Count - 1; i >= 0; i--)
                {
                    if (ob.PoisonList[i].Type == PoisonType.Infection) continue;

                    ob.PoisonList.RemoveAt(i);
                }

                foreach (BuffInfo buff in ob._buffs)
                {
                    switch (buff.Type)
                    {
                        case BuffType.Heal:
                        case BuffType.Invisibility:
                        case BuffType.MagicResistance:
                        case BuffType.Resilience:
                        case BuffType.MagicShield:
                        case BuffType.SuperiorMagicShield:
                        case BuffType.ElementalSuperiority:
                        case BuffType.BloodLust:
                        case BuffType.Defiance:
                        case BuffType.Might:
                        case BuffType.ReflectDamage:
                        case BuffType.JudgementOfHeaven:
                        case BuffType.StrengthOfFaith:
                        case BuffType.CelestialLight:
                        case BuffType.Transparency:
                        case BuffType.Renounce:
                        case BuffType.Cloak:
                        case BuffType.GhostWalk:
                        case BuffType.LifeSteal:
                        case BuffType.DarkConversion:
                        case BuffType.Evasion:
                        case BuffType.RagingWind:
                        case BuffType.Invincibility:
                        case BuffType.Concentration:
                        case BuffType.MagicWeakness:
                            buffs.Add(buff);
                            result++;
                            break;
                        default:
                            continue;
                    }
                }
            }

            if (CanAttackTarget(ob) && (ob.Level <= Level || SEnvir.Random.Next(100) < 42))
            {
                result += ob.PoisonList.Count;

                for (int i = ob.PoisonList.Count - 1; i >= 0; i--)
                {
                    if (ob.PoisonList[i].Type == PoisonType.Infection) continue;

                    ob.PoisonList.RemoveAt(i);
                }

                foreach (BuffInfo buff in ob._buffs)
                {
                    switch (buff.Type)
                    {
                        case BuffType.Heal:
                        case BuffType.Invisibility:
                        case BuffType.MagicResistance:
                        case BuffType.Resilience:
                        case BuffType.MagicShield:
                        case BuffType.SuperiorMagicShield:
                        case BuffType.ElementalSuperiority:
                        case BuffType.BloodLust:
                        case BuffType.Defiance:
                        case BuffType.Might:
                        case BuffType.ReflectDamage:
                        case BuffType.JudgementOfHeaven:
                        case BuffType.StrengthOfFaith:
                        case BuffType.CelestialLight:
                        case BuffType.Transparency:
                        case BuffType.Renounce:
                        case BuffType.Cloak:
                        case BuffType.GhostWalk:
                        case BuffType.LifeSteal:
                        case BuffType.DarkConversion:
                        case BuffType.Evasion:
                        case BuffType.RagingWind:
                        case BuffType.Invincibility:
                        case BuffType.Concentration:
                            buffs.Add(buff);
                            result++;
                            break;
                        default:
                            continue;
                    }
                }
            }

            for (int i = buffs.Count - 1; i >= 0; i--)
                ob.BuffRemove(buffs[i]);

            return result;
        }

        /// <summary>
        /// BUFF信息  BUFF增加
        /// </summary>
        /// <param name="type">BUFF类型</param>
        /// <param name="remainingTicks">剩余时间</param>
        /// <param name="stats">增加的属性</param>
        /// <param name="visible">是否可见</param>
        /// <param name="pause">是否暂停</param>
        /// <param name="tickRate">时间频率</param>
        /// <param name="fromCustomBuff">来自自定义BUFF</param>
        /// <returns></returns>
        public virtual BuffInfo BuffAdd(BuffType type, TimeSpan remainingTicks, Stats stats, bool visible, bool pause, TimeSpan tickRate, int fromCustomBuff = 0)
        {
            // 玩家可能拥有多个自定义类型的buff
            // 玩家可能拥有多个泰山buff
            // 玩家可能拥有多个活动buff
            if (type != BuffType.CustomBuff && type != BuffType.TarzanBuff && type != BuffType.EventBuff)
            {
                BuffRemove(type);
            }

            BuffInfo info;

            _buffs.Add(info = SEnvir.BuffInfoList.CreateNewObject());

            info.Type = type;
            info.Visible = visible;

            info.RemainingTime = remainingTicks;
            info.TickFrequency = tickRate;
            info.Pause = pause;
            info.Stats = stats;
            info.FromCustomBuff = fromCustomBuff;

            isBuffCanMove = _buffs.All(x => x.Type != BuffType.DragonRepulse && x.Type != BuffType.FrostBite);

            if (info.Stats != null && info.Stats.Count > 0)
                RefreshStats();

            switch (type)
            {
                case BuffType.PoisonousCloud:
                case BuffType.Observable:
                case BuffType.TheNewBeginning:
                case BuffType.Server:
                case BuffType.MapEffect:
                case BuffType.Castle:
                case BuffType.Guild:
                case BuffType.Group:
                case BuffType.Veteran:
                    info.IsTemporary = true;
                    break;
                case BuffType.RagingWind:
                case BuffType.MagicWeakness:
                case BuffType.PierceBuff:
                case BuffType.BurnBuff:
                    RefreshStats();
                    break;
                case BuffType.Invisibility:
                    //Much faster than checking nearby cells?
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        if (mob.Target == this && !mob.CoolEye)
                        {
                            mob.Target = null;
                            mob.SearchTime = SEnvir.Now;
                        }
                    }
                    break;
                case BuffType.Cloak:
                case BuffType.Transparency:
                    //Much faster than checking nearby cells?
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        if (mob.Target == this)
                        {
                            mob.Target = null;
                            mob.SearchTime = SEnvir.Now;
                        }
                    }

                    RemoveAllObjects();
                    AddAllObjects();

                    if (Race == ObjectType.Player)
                    {
                        ((PlayerObject)this).Companion?.RemoveAllObjects();
                        ((PlayerObject)this).Companion?.AddAllObjects();
                    }

                    break;
                case BuffType.FishingMaster:
                    foreach (MapObject mapOb in CurrentMap.Objects)
                    {
                        if (mapOb.Race != ObjectType.Monster) continue;

                        MonsterObject mob = (MonsterObject)mapOb;

                        if (mob.Target == this)
                        {
                            mob.Target = null;
                            mob.SearchTime = SEnvir.Now;
                        }
                    }
                    break;
            }

            if (!info.Visible) return info;

            Broadcast(new S.ObjectBuffAdd { ObjectID = ObjectID, Type = type, Index = fromCustomBuff });

            return info;
        }
        /// <summary>
        /// BUFF删除时
        /// </summary>
        /// <param name="info"></param>
        public virtual void BuffRemove(BuffInfo info)
        {
            try
            {
                if (info.Visible)
                    Broadcast(new S.ObjectBuffRemove { ObjectID = ObjectID, Type = info.Type, Index = info.FromCustomBuff });

                _buffs.Remove(info);

                isBuffCanMove = _buffs.All(x => x.Type != BuffType.DragonRepulse && x.Type != BuffType.FrostBite);

                if (info.Stats != null && info.Stats.Count > 0)
                    RefreshStats();

                info.Delete();
                switch (info.Type)
                {
                    case BuffType.Cloak:
                    case BuffType.Transparency:
                        BuffRemove(BuffType.GhostWalk);
                        RemoveAllObjects();
                        AddAllObjects();

                        if (Race == ObjectType.Player)
                        {
                            ((PlayerObject)this).Companion?.RemoveAllObjects();
                            ((PlayerObject)this).Companion?.AddAllObjects();
                        }
                        break;
                    case BuffType.RagingWind:
                        RefreshStats();
                        break;
                    case BuffType.MagicWeakness:
                        RefreshStats();
                        if (Race == ObjectType.Player)
                            ((PlayerObject)this).Connection.ReceiveChat($"你的魔御恢复正常", MessageType.System);
                        break;
                    case BuffType.PierceBuff:
                        RefreshStats();
                        if (Race == ObjectType.Player)
                            ((PlayerObject)this).Connection.ReceiveChat($"你的强元素恢复正常", MessageType.System);
                        break;
                    case BuffType.BurnBuff:
                        RefreshStats();
                        if (Race == ObjectType.Player)
                            ((PlayerObject)this).Connection.ReceiveChat($"你的攻击恢复正常", MessageType.System);
                        break;
                }

                switch (info.Type)
                {
                    case BuffType.Cloak:
                    case BuffType.Transparency:
                    case BuffType.Invisibility:
                    case BuffType.FishingMaster:
                        //Much faster than checking nearby cells?
                        foreach (MapObject mapOb in CurrentMap.Objects)
                        {
                            if (mapOb.Race != ObjectType.Monster) continue;

                            MonsterObject mob = (MonsterObject)mapOb;

                            mob.SearchTime = DateTime.MinValue;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
        /// <summary>
        /// BUFF移除
        /// </summary>
        /// <param name="type"></param>
        public void BuffRemove(BuffType type)
        {
            BuffInfo info = _buffs.FirstOrDefault(x => x.Type == type);

            if (info != null)
            {
                BuffRemove(info);
                if (Race == ObjectType.Player && info.Type == BuffType.Transparency)
                {
                    PlayerObject player = this as PlayerObject;
                    player.TraOverTime = SEnvir.Now;
                }
            }
        }

        /// <summary>
        /// 是否拥有对应类型的BUFF
        /// </summary>
        /// <param name="type">BUFF序号</param>
        /// <returns></returns>
        public bool HasBuff(BuffType type)
        {
            return Buffs.Any(x => x.Type == type);
        }

        /// <summary>
        /// 是否拥有对应序号的BUFF
        /// </summary>
        /// <param name="index">BUFF序号</param>
        /// <returns></returns>
        public bool HasCustomBuff(int index)
        {
            return Buffs.Any(x => x.FromCustomBuff == index);
        }
        /// <summary>
        /// 移除玩家自定义BUFF
        /// </summary>
        /// <param name="index">BUFF序号</param>
        public void CustomBuffRemove(int index)
        {
            if (index == 0)
            {
                // 因为FromCustomBuff默认值是0 如果移除的话会移除非自定义buff
                return;
            }

            BuffInfo convertedBuffInfo = Buffs.FirstOrDefault(x => x.FromCustomBuff == index);
            if (convertedBuffInfo != null)
            {
                BuffRemove(convertedBuffInfo);
            }
        }
        /// <summary>
        /// 给玩家增加自定义BUFF
        /// </summary>
        /// <param name="index">BUFF序号</param>
        /// <returns></returns>
        public BuffInfo CustomBuffAdd(int index)
        {
            CustomBuffInfo buffInfo = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == index);
            if (buffInfo == null)
                return null;

            //先移除
            CustomBuffRemove(index);
            //读取属性加成信息
            Stats stats = new Stats();
            foreach (CustomBuffInfoStat customBuffInfoStat in buffInfo.BuffStats)
            {
                stats[customBuffInfoStat.Stat] = customBuffInfoStat.Amount;
            }
            //如果剩余时间是00:00:00 则视为永久buff
            bool isPermanentBuff = buffInfo.Duration < TimeSpan.FromSeconds(1);
            //加buff
            BuffInfo convertedBuff = BuffAdd(buffInfo.BuffType, isPermanentBuff ? TimeSpan.MaxValue : buffInfo.Duration, stats, true, InSafeZone && buffInfo.PauseInSafeZone,
                isPermanentBuff ? TimeSpan.Zero : TimeSpan.FromSeconds(1), index);
            return convertedBuff;
        }


        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="power">攻击值</param>
        /// <param name="elemnet">元素</param>
        /// <param name="canReflect">反弹伤害</param>
        /// <param name="ignoreShield">无视盾</param>
        /// <param name="canCrit">抗暴击</param>
        /// <param name="canStruck">抗推动</param>
        /// <returns></returns>
        public virtual int Attacked(MapObject attacker, int power, Element elemnet, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true) { return 0; }
        /// <summary>
        /// 获取目标
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="location">坐标</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public List<MapObject> GetTargets(Map map, Point location, int radius)
        {
            List<MapObject> targets = new List<MapObject>();

            for (int y = location.Y - radius; y <= location.Y + radius; y++)
            {
                if (y < 0) continue;
                if (y >= map.Height) break;

                for (int x = location.X - radius; x <= location.X + radius; x++)
                {
                    if (x < 0) continue;
                    if (x >= map.Width) break;

                    Cell cell = map.Cells[x, y];

                    if (cell?.Objects == null) continue;

                    foreach (MapObject ob in cell.Objects)
                    {
                        if (!CanAttackTarget(ob)) continue;

                        targets.Add(ob);
                    }
                }
            }
            return targets;
        }
        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <param name="location"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public List<MapObject> GetAllObjects(Point location, int radius)
        {
            List<MapObject> obs = new List<MapObject>();

            for (int y = location.Y - radius; y <= location.Y + radius; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = location.X - radius; x <= location.X + radius; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;

                    Cell cell = CurrentMap.Cells[x, y];

                    if (cell?.Objects == null) continue;

                    obs.AddRange(cell.Objects);
                }
            }
            return obs;
        }
        /// <summary>
        /// 可以帮助的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool CanHelpTarget(MapObject ob)
        {
            return false;
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool CanAttackTarget(MapObject ob)
        {
            return false;
        }
        public virtual bool CanFlyTarget(MapObject ob)
        {
            return false;
        }
        /// <summary>
        /// 躲避
        /// </summary>
        public virtual void Dodged()
        {
            DisplayMiss = true;
        }
        /// <summary>
        /// 格挡
        /// </summary>
        public virtual void Blocked()
        {
            DisplayBlock = true;
        }
        /// <summary>
        /// 暴击
        /// </summary>
        public virtual void Critical()
        {
            DisplayCrit = true;
        }
        /// <summary>
        /// 致命一击
        /// </summary>
        public virtual void FatalAttack()
        {
            DisplayFatalAttack = true;
        }
        /// <summary>
        /// 会心一击
        /// </summary>
        public virtual void CriticalHit()
        {
            DisplayCriticalHit = true;
        }
        /// <summary>
        /// 暴捶
        /// </summary>
        public virtual void DamageAdd()
        {
            DisplayDamageAdd = true;
        }
        /// <summary>
        /// 爆毒
        /// </summary>
        public virtual void GreenPosionPro()
        {
            DisplayGreenPosionPro = true;
        }
        /// <summary>
        /// 抽蓝
        /// </summary>
        public virtual void SmokingMP()
        {
            DisplaySmokingMP = true;
        }
        /// <summary>
        /// 施毒
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool ApplyPoison(Poison p)
        {
            //如果死亡 毒结束
            if (Dead) return false;
            //如果随机值小于毒性抵抗 毒结束
            if (SEnvir.Random.Next(100) < Stats[Stat.PoisonResistance])
            {
                if (Race != ObjectType.Player) return false;  //如果不是玩家类 有加毒性抵抗就不中毒

                PlayerObject player = (PlayerObject)this;  //如果是玩家，直接抵抗成功出个提示

                if (player.Race == ObjectType.Player)
                    player.Connection.ReceiveChat("成功抵抗本次毒素攻击", MessageType.System);

                return false;
            }
            //遍历毒列表
            foreach (Poison poison in PoisonList)
            {
                //毒的类型不对 继续下一步
                if (poison.Type != p.Type) continue;
                //毒的值大于值 毒结束
                if (poison.Value > p.Value) return false;
                //删除毒
                PoisonList.Remove(poison);
                break;
            }
            //增加毒
            PoisonList.Add(p);

            return true;
        }
        /// <summary>
        /// 死亡
        /// </summary>
        public virtual void Die()
        {
            Dead = true;  //死亡为真

            BuffRemove(BuffType.Heal);   //删除治愈术BUFF
            BuffRemove(BuffType.DragonRepulse);  //删除狂涛涌泉BUFF
            BuffRemove(BuffType.ElementalHurricane);  //删除离魂邪风BUFF

            PoisonList.Clear(); //毒药列表 清除

            //播送 新的物体死亡 对象ID 方向 当前位置
            Broadcast(new S.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }
        /// <summary>
        /// 获取信息数据包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public abstract Packet GetInfoPacket(PlayerObject ob);
        /// <summary>
        /// 获取数据包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public abstract Packet GetDataPacket(PlayerObject ob);
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="p"></param>
        public void Broadcast(Packet p)
        {
            foreach (PlayerObject player in SeenByPlayers)  //玩家看到中的玩家对象
                player.Enqueue(p);   //玩家队列
        }
        /// <summary>
        /// 动作推
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public virtual int Pushed(MirDirection direction, int distance)
        {
            int count = 0;
            if (Dead) return count;
            if (_buffs.Any(x => x.Type == BuffType.Endurance || x.Type == BuffType.DragonRepulse || x.Type == BuffType.ElementalHurricane)) return count;

            PreventSpellCheck = true;
            for (int i = 0; i < distance; i++)
            {
                Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction));

                if (cell == null || cell.IsBlocking(this, false)) break;
                if (Race == ObjectType.Monster && cell.Movements != null) break;

                Direction = Functions.ShiftDirection(direction, 4);

                ActionTime += TimeSpan.FromMilliseconds(Config.GlobalsTurnTime);

                //CurrentCell = cell.GetMovement(this); //Repel same direction across map ?
                CurrentCell = cell;
                RemoveAllObjects();
                AddAllObjects();
                Broadcast(new S.ObjectPushed { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                count++;
            }
            PreventSpellCheck = false;

            /*    if (count > 0 && checkSpells)
                    CheckSpellObjects();*/
            return count;
        }
        /// <summary>
        /// 在组里
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public bool InGroup(MapObject ob)
        {
            //ob可以为空
            return ob?.GroupMembers != null && ob.GroupMembers == GroupMembers;
        }

        /// <summary>
        /// 获取物理攻击值
        /// </summary>
        /// <returns></returns>
        public int GetDC()
        {
            int min = Stats[Stat.MinDC];
            int max = Stats[Stat.MaxDC];
            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 获取魔法攻击值
        /// </summary>
        /// <returns></returns>
        public int GetMC()
        {
            int min = Stats[Stat.MinMC];
            int max = Stats[Stat.MaxMC] * Config.WizardMCRate / 100;
            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 获取灵魂攻击值
        /// </summary>
        /// <returns></returns>
        public int GetSC()
        {
            int min = Stats[Stat.MinSC];
            int max = Stats[Stat.MaxSC] * Config.TaoistSCRate / 100;
            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 获取全系列魔法攻击值
        /// </summary>
        /// <returns></returns>
        public int GetSP()
        {
            int min = Math.Min(Stats[Stat.MinMC], Stats[Stat.MinSC]);
            int max = Math.Min(Stats[Stat.MaxMC], Stats[Stat.MaxSC]);
            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 获取防御值
        /// </summary>
        /// <returns></returns>
        public int GetAC()
        {
            int min = Stats[Stat.MinAC];
            int max = Stats[Stat.MaxAC];

            if (min < 0) min = 0;
            if (min >= max) return max;  //如果最小防大于等于最大防 按最大防值计算

            //随机 （最小防， 最大防+1）
            int randomAC = SEnvir.Random.Next(min, max + 1);

            return randomAC;
        }
        /// <summary>
        /// 获取魔御值
        /// </summary>
        /// <returns></returns>
        public int GetMR()
        {
            int min = Stats[Stat.MinMR];
            int max = Stats[Stat.MaxMR];

            if (min < 0) min = 0;
            if (min >= max) return max;  //如果最小魔防大于等于最大魔防 按最大魔防值计算

            //随机 （最小魔防， 最大魔防+1）
            int randomMR = SEnvir.Random.Next(min, max + 1);

            return randomMR;
        }
    }
    /// <summary>
    /// 毒
    /// </summary>
    public class Poison
    {
        /// <summary>
        /// 地图对象 施毒者
        /// </summary>
        public MapObject Owner;
        /// <summary>
        /// 毒类型
        /// </summary>
        public PoisonType Type;
        /// <summary>
        /// 伤害值
        /// 冰冻的话，代表时间*500毫秒
        /// </summary>
        public int Value;
        /// <summary>
        /// 频率
        /// </summary>
        public TimeSpan TickFrequency;
        /// <summary>
        /// 频率计数
        /// </summary>
        public int TickCount;
        /// <summary>
        /// 频率时间
        /// </summary>
        public DateTime TickTime;
        /// <summary>
        /// 额外的时间
        /// </summary>
        public DateTime ExtraTickTime;
        /// <summary>
        /// 额外的数值
        /// </summary>
        public int ExtraValue;
        /// <summary>
        /// 频率
        /// </summary>
        public TimeSpan ExtraTickFrequency;

        /// <summary>
        /// 额外的
        /// </summary>
        public object Extra;
    }

}
