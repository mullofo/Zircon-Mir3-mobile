using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 游戏进程控制角色类
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            // if (LastHitter != null && LastHitter.Node == null) LastHitter = null;
            if (GroupInvitation != null && GroupInvitation.Node == null) GroupInvitation = null;
            if (GuildInvitation != null && GuildInvitation.Node == null) GuildInvitation = null;
            if (MarriageInvitation != null && MarriageInvitation.Node == null) MarriageInvitation = null;

            if (CombatTime != SentCombatTime)
            {
                SentCombatTime = CombatTime;
                Enqueue(new S.CombatTime());
            }

            ProcessRegen();

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>();

            foreach (MonsterObject ob in TaggedMonsters)
            {
                if (SEnvir.Now < ob.EXPOwnerTime) continue;
                clearList.Add(ob);
            }

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;

            if (CanFlamingSword && SEnvir.Now >= FlamingSwordTime)
            {
                CanFlamingSword = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.FlamingSword, CanUse = CanFlamingSword });

                Connection.ReceiveChat("Skills.ChargeExpire".Lang(Connection.Language, Magics[MagicType.FlamingSword].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.ChargeExpire".Lang(con.Language, Magics[MagicType.FlamingSword].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
            }
            if (CanDragonRise && SEnvir.Now >= DragonRiseTime)
            {
                CanDragonRise = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.DragonRise, CanUse = CanDragonRise });

                Connection.ReceiveChat("Skills.ChargeExpire".Lang(Connection.Language, Magics[MagicType.DragonRise].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.ChargeExpire".Lang(con.Language, Magics[MagicType.DragonRise].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
            }
            if (CanBladeStorm && SEnvir.Now >= BladeStormTime)
            {
                CanBladeStorm = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.BladeStorm, CanUse = CanBladeStorm });

                Connection.ReceiveChat("Skills.ChargeExpire".Lang(Connection.Language, Magics[MagicType.BladeStorm].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.ChargeExpire".Lang(con.Language, Magics[MagicType.BladeStorm].Info.Lang(Connection.Language, p => p.Name)), MessageType.Hint);
            }
            if (CanMaelstromBlade && SEnvir.Now >= MaelstromBladeTime)
            {
                CanMaelstromBlade = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.MaelstromBlade, CanUse = CanMaelstromBlade });

                Connection.ReceiveChat("Skills.ChargeExpire".Lang(Connection.Language, Magics[MagicType.MaelstromBlade].Info.Lang(Connection.Language, p => p.Name)), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.ChargeExpire".Lang(con.Language, Magics[MagicType.MaelstromBlade].Info.Lang(Connection.Language, p => p.Name)), MessageType.System);
            }

            if (Config.AutoReviveDelay != TimeSpan.Zero || (Config.AutoReviveDelay == TimeSpan.Zero && CurrentMap.Info.CanDeadTownRevive == true))
            {
                if (Dead && SEnvir.Now >= RevivalTime || Dead && CurrentMap.Info.CanDeadTownRevive == true)
                    TownRevive();
            }

            ProcessTorch();

            ProcessAutoPotion();

            ProcessItemExpire();

            ProcessSkill();
            ProcessDetectionDay();

            ProcessPetsFollow();

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnProcess", args);
                    //trig_player(this, "OnProcess", args);
                }

            }
            catch (SyntaxErrorException e)
            {
                string msg = "DelayCall Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "DelayCall SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "DelayCall Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            //检查制造情况
            if (SEnvir.Now >= CraftTime)
            {
                CraftTime = SEnvir.Now.AddMilliseconds(100);
                CheckCrafting();
            }

            //更新成就信息
            if (SEnvir.Now >= SEnvir.EventPacketTime)
            {
                SEnvir.EventPacketTime = SEnvir.Now + SEnvir.EventPacketInterval;
                SendAchievementUpdates();
            }

            //更新钓鱼信息
            if (SEnvir.Now >= FishingStartTime && IsFishing && SEnvir.Now > NextFishingCheckTime)
            {
                CheckFishing();
            }
            //检查背号情况
            if (Config.EnableFollowing)
            {
                ProcessFollowing();
            }
            //检查NPC的距离
            if (NPC != null && Functions.InRange(NPC.CurrentLocation, CurrentLocation, Config.MaxViewRange) && !(Functions.InRange(NPC.CurrentLocation, CurrentLocation, Config.MaxNPCViewRange)))
            {
                NPC = null;
                NPCPage = null;
                Enqueue(new S.NPCClose());
            }
        }
        /// <summary>
        /// 检测日期变化
        /// </summary>
        public void ProcessDetectionDay()
        {
            if (Character.Account.FlashTime.Date != SEnvir.Now.Date)//如果在线并隔天
            {
                /*
                try
                {
                    OnDayChange?.Invoke(this);
                }
                catch (Exception ex)
                {
                    SEnvir.Log(ex.ToString());
                }
                */
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        //var argss = new Tuple<object>(this);
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnDayChange", args);
                        //trig_player(this, "OnDayChange", args);
                        if (Character.Account.FlashTime.Month != SEnvir.Now.Month)//隔月了
                        {
                            trig_player(this, "OnMonthChange", args);
                        }
                        if (Character.Account.FlashTime.DayOfWeek == DayOfWeek.Monday)//隔周了
                        {
                            trig_player(this, "OnWeekChange", args);
                        }
                    }
                }
                catch (SyntaxErrorException e)
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

                if (Character.Account.AutoTime < Config.AutoTime * 3600)     //挂机时间
                    Character.Account.AutoTime = Config.AutoTime * 3600;
                AutoTime = SEnvir.Now.AddSeconds(Character.Account.AutoTime);
                Character.Account.FlashTime = SEnvir.Now;
                Enqueue(new S.AutoTimeChanged { AutoTime = Character.Account.AutoTime });
            }
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            if (!SetBindPoint())
            {
                SEnvir.Log($"[生成角色失败]角色ID: {Character.Index}, 角色名字: {Character.CharacterName}, 重置初始出生点失败。");
                Enqueue(new S.StartGame { Result = StartGameResult.UnableToSpawn });
                Connection = null;
                Character = null;
                return;
            }

            if (!Spawn(Character.CurrentMap, CurrentLocation) && !Spawn(Character.BindPoint.BindRegion))
            {
                SEnvir.Log($"[生成角色失败]角色ID: {Character.Index}, 角色名字: {Character.CharacterName}");
                Enqueue(new S.StartGame { Result = StartGameResult.UnableToSpawn });
                Connection = null;
                Character = null;
                return;
            }

            SendClientNameChanged();  //显示客户端名字变化

            SendItemSourceDisplaySetting();   //客户端显示道具信息

            FriendListRefresh(true);   // 提示好友上线 并更新目标好友列表

            if (Config.EnableRewardPool)
            {
                SendRewardPoolUpdate(); // 发送奖金池信息
            }

            if (Config.EnableRedPacket)
            {
                SendRecentRedPacketsUpdate();
                SendRewardPoolRanks();
            }

        }
        /// <summary>
        /// 用户登录的时候调用客户端名字
        /// </summary>
        public void SendClientNameChanged()
        {
            Enqueue(new S.ClientNameChanged
            {
                ClientName = Config.ClientName,
                PhysicalResistanceSwitch = Config.PhysicalResistanceSwitch,
                PenetraliumKeyA = Config.PenetraliumKeyA,
                PenetraliumKeyB = Config.PenetraliumKeyB,
                PenetraliumKeyC = Config.PenetraliumKeyC,

                AllowTeleportMagicNearFlag = Config.AllowTeleportMagicNearFlag,
                TeleportMagicRadiusRange = Config.WarFlagRangeLimit,
            });
        }
        /// <summary>
        /// 是否允许客户端显示物品来源
        /// </summary>
        public void SendItemSourceDisplaySetting()
        {
            Enqueue(new S.ShowItemSource
            {
                DisplayItemSource = Config.ShowItemSource,
                DisplayGMSource = Config.ShowGMItemSource,
            });
        }

        /// <summary>
        /// 发送图像更新
        /// </summary>
        public void SendShapeUpdate()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];
            UserItem armour = Equipment[(int)EquipmentSlot.Armour];

            S.PlayerUpdate p = new S.PlayerUpdate
            {
                ObjectID = ObjectID,

                //Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info.Shape ?? -1, //武器
                Weapon = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Weapon]), //武器幻化

                WeaponImage = Equipment[(int)EquipmentSlot.Weapon]?.Info.Image ?? 0,   //武器图像
                WeaponIndex = Equipment[(int)EquipmentSlot.Weapon]?.Info.Index ?? 0,   //武器index

                Shield = Character.HideShield ? 0 : Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? -1,  //盾牌

                //Armour = Equipment[(int)EquipmentSlot.Armour]?.Info.Shape ?? 0,  //衣服
                Armour = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Armour], 0), //衣服幻化

                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty, //衣服颜色
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info.Image ?? 0,   //衣服图像
                ArmourIndex = Equipment[(int)EquipmentSlot.Armour]?.Info.Index ?? 0,   //衣服index

                Helmet = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,  //头盔
                Fashion = Character.HideFashion ? -1 : Equipment[(int)EquipmentSlot.Fashion]?.Info.Shape ?? -1, //时装
                FashionImage = Equipment[(int)EquipmentSlot.Fashion]?.Info.Image ?? 0,   //时装图像
                Emblem = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? -1,   //徽章效果

                HorseArmour = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,

                Light = Stats[Stat.Light],
            };

            Broadcast(p);
        }
        /// <summary>
        /// 发送更改更新
        /// </summary>
        public void SendChangeUpdate()
        {
            S.PlayerChangeUpdate p = new S.PlayerChangeUpdate
            {
                ObjectID = ObjectID,

                Name = Name,

                Gender = Gender,
                HairType = HairType,
                HairColour = HairColour,
                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty,
            };

            Broadcast(p);
        }
        /// <summary>
        /// 移出玩家
        /// </summary>
        public override void OnDespawned()
        {
            base.OnDespawned();
            SEnvir.Players.Remove(this);
        }

        /// <summary>
        /// 清除
        /// </summary>
        public override void CleanUp()
        {
            base.CleanUp();

            NPC = null;
            NPCPage = null;

            Pets?.Clear();

            VisibleObjects?.Clear();

            VisibleDataObjects?.Clear();

            TaggedMonsters?.Clear();

            NearByObjects?.Clear();

            Inventory = null;
            PatchGrid = null;
            Equipment = null;
            Storage = null;

            Companion = null;

            LastHitter = null;

            GroupInvitation = null;

            GuildInvitation = null;

            MarriageInvitation = null;

            TradePartner = null;

            TradePartnerRequest = null;

            TradeItems?.Clear();

            Magics?.Clear();

            AutoPotions?.Clear();
            AutoFights?.Clear();
        }
        /// <summary>
        /// 停用
        /// </summary>
        public override void DeActivate()
        {
            return;
        }
        /// <summary>
        /// 停止游戏
        /// </summary>
        public void StopGame()
        {
            //Item Links
            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                    SEnvir.ExecutePyWithTimer(trig_play, this, "OnStopGame", args);
                    //trig_play(this, "OnStopGame", args);
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

            Character.LastLogin = SEnvir.Now;
            Character.TodayOnlineMinutes += (int)(SessionDurationInMinutes);

            if (Character.Account.GuildMember != null)
            {
                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member == Character.Account.GuildMember || member?.Account?.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildMemberOffline { Index = Character.Account.GuildMember.Index, ObserverPacket = false });
                }

                foreach (GuildAllianceInfo allianceInfo in Character.Account.GuildMember.Guild.Alliances)
                {
                    foreach (GuildMemberInfo member in allianceInfo.Guild1 == Character.Account.GuildMember.Guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
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

            TradeClose();

            BuffRemove(BuffType.DragonRepulse);
            BuffRemove(BuffType.Developer);
            BuffRemove(BuffType.Ranking);
            BuffRemove(BuffType.Castle);
            BuffRemove(BuffType.Veteran);
            BuffRemove(BuffType.ElementalHurricane);
            BuffRemove(BuffType.SuperiorMagicShield);

            if (GroupMembers != null) GroupLeave();

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>(TaggedMonsters);

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;

            TaggedMonsters.Clear();

            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();
            SpellList.Clear();

            for (int i = Pets.Count - 1; i >= 0; i--)
                Pets[i].Despawn();
            Pets.Clear();
            List<CharacterInfo> characterInfos = new List<CharacterInfo>();
            for (int i = Connection.Observers.Count - 1; i >= 0; i--)
            {
                var sc = Connection.Observers[i];
                if (sc.Account.TempAdmin)
                {
                    characterInfos.Clear();
                    bool flag = false;
                    foreach (CharacterInfo info in SEnvir.Rankings)
                    {
                        if (info.Deleted) continue;
                        if (info.Player == null || info.Player == this) continue;  //如果不在线 或者 角色等空 忽略
                        if (info.Account.TempAdmin) continue;  //如果是管理员 忽略
                        characterInfos.Add(info);
                    }
                    if (characterInfos.Count > 0)
                    {
                        CharacterInfo info = characterInfos[SEnvir.Random.Next(Math.Min(characterInfos.Count, Config.ObservedCount))];
                        sc.Observed.Observers.Remove(sc);
                        sc.Observed = null;
                        info.Player.SetUpObserver(sc);
                        flag = true;
                    }

                    if (flag) continue;
                }

                Connection.Observers[i].EndObservation();
            }
            Connection.Observers.Clear();

            #region 清理绑定的py函数

            SEnvir.ScriptList.RemoveAll(x => x.Player == this);

            #endregion

            CompanionDespawn();

            if (Character.Partner?.Player != null)
                Character.Partner.Player.Enqueue(new S.MarriageOnlineChanged());

            Despawn();

            Connection.Player = null;
            Character.Player = null;

            FriendListRefresh(false);  //提示好友离线 并更新目标好友列表

            Connection = null;
            Character = null;
        }
    }
}
