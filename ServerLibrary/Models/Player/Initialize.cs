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
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //初始化人物的相关函数
    {
        /// <summary>
        /// 获取开始信息
        /// </summary>
        /// <returns></returns>
        private StartInformation GetStartInformation()
        {
            List<ClientBeltLink> blinks = new List<ClientBeltLink>();

            foreach (CharacterBeltLink link in Character.BeltLinks)
            {
                if (link.LinkItemIndex > 0 && Inventory.FirstOrDefault(x => x?.Index == link.LinkItemIndex) == null)
                    link.LinkItemIndex = -1;

                blinks.Add(link.ToClientInfo());
            }

            List<ClientAutoPotionLink> alinks = new List<ClientAutoPotionLink>();
            List<ClientAutoFightLink> flinks = new List<ClientAutoFightLink>();

            foreach (AutoPotionLink link in Character.AutoPotionLinks)
                alinks.Add(link.ToClientInfo());
            foreach (AutoFightConfig flink in Character.AutoFightLinks)
            {
                flinks.Add(flink.ToClientInfo());
                setConfArr[(int)flink.Slot] = flink.Enabled;
            }

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];   //武器
            UserItem armour = Equipment[(int)EquipmentSlot.Armour];   //衣服

            CraftLevel = CraftLevel < 1 ? 1 : CraftLevel;  //如果制造等级小于1那么赋值为1级

            // 修复buff倒计时异常
            // 被观察的时候 观察者会调用这个函数获取被观察者的信息
            // 所以如果玩家被观察了 不应对他的属性做出任何修改
            if (Config.OfflineBuffTicking && Connection.Observers.Count == 0)
            {
                ProcessOfflineBuff();
            }

            return new StartInformation  //启动信息
            {
                Index = Character.Index,
                ObjectID = ObjectID,
                Name = Name,
                AchievementTitle = Character.AchievementTitle,    //成就称号
                GuildName = Character.Account.GuildMember?.Guild.GuildName,
                GuildRank = Character.Account.GuildMember?.Rank,
                NameColour = NameColour,
                Level = Level,
                Class = Class,
                Gender = Gender,
                Location = CurrentLocation,
                Direction = Direction,

                MapIndex = CurrentMap.Info.Index,

                Gold = Gold,  //金币
                GameGold = 0,  //元宝
                Prestige = Prestige,  //声望
                Contribute = Contribute,  //贡献

                HairType = HairType,  //头发
                HairColour = HairColour,  //头发颜色
                HorseType = Character.Horse,

                //Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info?.Shape ?? -1,  //武器
                Weapon = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Weapon]),  //武器幻化

                WeaponImage = Equipment[(int)EquipmentSlot.Weapon]?.Info?.Image ?? 0,  //武器图像
                WeaponIndex = Equipment[(int)EquipmentSlot.Weapon]?.Info?.Index ?? 0,   //武器序号

                Shield = Equipment[(int)EquipmentSlot.Shield]?.Info?.Shape ?? -1,   //盾牌效果

                Emblem = Equipment[(int)EquipmentSlot.Emblem]?.Info?.Shape ?? -1,   //徽章效果

                //Armour = Equipment[(int)EquipmentSlot.Armour]?.Info?.Shape ?? 0,  //衣服
                Armour = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Armour], 0),  //衣服幻化

                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty, //衣服颜色
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info?.Image ?? 0,  //衣服图像
                ArmourIndex = Equipment[(int)EquipmentSlot.Armour]?.Info?.Index ?? 0,  //衣服序号

                CraftLevel = CraftLevel, //制造等级
                CraftExp = CraftExp, //制造经验
                CraftFinishTime = CraftFinishTime, //制造物品完成时间
                BookmarkedCraftItemInfoIndex = BookmarkedCraftItemInfo?.Index ?? 0, //收藏的制造物品
                CraftingItemIndex = CraftingItem?.Index ?? 0, //进行中的制造物品

                DailyQuestRemains = Config.DailyQuestLimit - Character.DailyQuestCount,  //每日任务剩余
                RepeatableQuestRemains = Config.RepeatableQuestLimit - Character.RepeatableQuestCount,  //可重复任务剩余

                Experience = Experience,

                DayTime = SEnvir.DayTime,
                AllowGroup = Character.Account.AllowGroup,   //组队开关
                AllowResurrectionOrder = Character.Account.AllowResurrectionOrder,  //回生开关
                AllowFriend = Character.Account.AllowFriend,  //好友开关

                CurrentHP = DisplayHP,
                CurrentMP = DisplayMP,

                AttackMode = AttackMode,
                PetMode = PetMode,

                Items = Character.Items.Select(x => x.ToClientInfo()).ToList(),
                BeltLinks = blinks,
                AutoPotionLinks = alinks,
                AutoFightLinks = flinks,
                Magics = Character.Magics.Select(X => X.ToClientInfo()).ToList(),
                Buffs = Buffs.Select(X => X.ToClientInfo()).ToList(),

                Poison = Poison,

                InSafeZone = InSafeZone,

                Observable = (Character.Account.ItemBot || Character.Account.GoldBot) ? true : Character.Observable,
                HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints),

                Dead = Dead,

                Horse = Horse,

                HelmetShape = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,   //头盔

                FashionShape = Character.HideFashion ? -1 : Equipment[(int)EquipmentSlot.Fashion]?.Info.Shape ?? -1,  //时装

                FashionImage = Equipment[(int)EquipmentSlot.Fashion]?.Info.Image ?? 0,  //时装图像

                HorseShape = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,

                ShieldShape = Character.HideShield ? 0 : Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? 0,   //盾牌

                EmblemShape = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? 0,   //徽章效果

                Quests = Character.Quests.Select(x => x.ToClientInfo()).ToList(),
                Achievements = Character.Achievements.Select(x => x.ToClientInfo()).ToList(),  //成就

                CompanionUnlocks = Character.Account.CompanionUnlocks.Select(x => x.CompanionInfo.Index).ToList(),

                Companions = Character.Account.Companions.Select(x => x.ToClientInfo()).ToList(),

                Companion = Character.Companion?.Index ?? 0,

                StorageSize = Character.Account.StorageSize,

                PatchGridSize = Character.PatchGridSize == 0 ? Character.PatchGridSize = Globals.PatchGridSize : Character.PatchGridSize,  //碎片包裹

                SkillExpDrop = Config.SkillExpDrop,

                FreeTossCount = Config.DailyFreeCoins - Character.DailyFreeTossUsed,

            };
        }
        public void AddBaseStats()  //添加基础信息
        {
            MaxExperience = Level < SEnvir.GamePlayEXPInfoList.Count ? SEnvir.GamePlayEXPInfoList[Level].Exp : 0;
            //MaxExperience = Level < Globals.ExperienceList.Count ? Globals.ExperienceList[Level] : 0;

            BaseStat stat = null;

            //获得最佳匹配
            foreach (BaseStat bStat in SEnvir.BaseStatList.Binding)
            {
                if (bStat.Class != Class) continue;
                if (bStat.Level > Level) continue;
                if (stat != null && bStat.Level < stat.Level) continue;

                stat = bStat;

                if (bStat.Level == Level) break;
            }

            if (stat == null) return;

            Stats[Stat.Health] = stat.Health;
            Stats[Stat.Mana] = stat.Mana;

            Stats[Stat.BagWeight] = stat.BagWeight;
            Stats[Stat.WearWeight] = stat.WearWeight;
            Stats[Stat.HandWeight] = stat.HandWeight;

            Stats[Stat.Accuracy] = stat.Accuracy;

            Stats[Stat.Agility] = stat.Agility;

            Stats[Stat.MinAC] = stat.MinAC;
            Stats[Stat.MaxAC] = stat.MaxAC;

            Stats[Stat.MinMR] = stat.MinMR;
            Stats[Stat.MaxMR] = stat.MaxMR;

            Stats[Stat.MinDC] = stat.MinDC;
            Stats[Stat.MaxDC] = stat.MaxDC;

            Stats[Stat.MinMC] = stat.MinMC;
            Stats[Stat.MaxMC] = stat.MaxMC;

            Stats[Stat.MinSC] = stat.MinSC;
            Stats[Stat.MaxSC] = stat.MaxSC;


            Stats[Stat.PickUpRadius] = 0;  //范围捡取 初始值为0

            Stats[Stat.SkillRate] = 1;
            Stats[Stat.CriticalChance] = 0;

            Stats.Add(Character.HermitStats);

            Stats[Stat.BaseHealth] = Stats[Stat.Health];
            Stats[Stat.BaseMana] = Stats[Stat.Mana];
        }
        public override void ProcessNameColour()  //名字颜色
        {
            NameColour = Color.White;  //名字颜色默认白色

            if (Stats[Stat.Rebirth] > 0)   //转生后的名字颜色
            {
                switch (Stats[Stat.Rebirth])
                {
                    case 1:
                        NameColour = Color.LightPink;
                        break;
                    case 2:
                        NameColour = Color.Pink;
                        break;
                    case 3:
                        NameColour = Color.Plum;
                        break;
                    case 4:
                        NameColour = Color.HotPink;
                        break;
                    case 5:
                        NameColour = Color.DeepPink;
                        break;
                    default:
                        NameColour = Color.DeepPink;
                        break;
                }
            }

            if (Stats[Stat.PKPoint] >= Config.RedPoint)
                NameColour = Globals.RedNameColour;   //PK值超过设置的200 红色
            else if (Stats[Stat.Brown] > 0)
                NameColour = Globals.BrownNameColour;  //PK变名 棕色
            else if (Stats[Stat.PKPoint] >= 50)
                NameColour = Color.Yellow;       //PK值大于或者等于50 黄色
        }
        protected override void OnSpawned()  //生成玩家
        {
            base.OnSpawned();

            SEnvir.Players.Add(this);

            Character.Player = this;
            Connection.Player = this;
            Connection.Stage = GameStage.Game;

            ShoutTime = SEnvir.Now.AddSeconds(10);

            //Broadcast Appearance(?)

            Enqueue(new S.StartGame { Result = StartGameResult.Success, ShortcutEnabled = Config.ShortcutEnabled, StartInformation = GetStartInformation() });
            Character.LastLogin = SEnvir.Now;
            Character.Account.LastCharacter = Character;

            //Send Items
            //Connection.ReceiveChat("15岁及以上可游玩", MessageType.Announcement);
            Connection.ReceiveChat(AttackMode.Lang(Connection.Language), MessageType.Announcement);
            Connection.ReceiveChat("System.AttackModeTips".Lang(Connection.Language), MessageType.Announcement);
            Connection.ReceiveChat("System.Welcome".Lang(Connection.Language), MessageType.Announcement);

            SendGuildInfo();

            if (Level > 0)
            {
                RefreshStats();

                if (CurrentHP <= 0)
                {
                    Dead = true;
                    TownRevive();
                }
            }

            if (Character.Account.GuildMember != null)
            {
                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    if (member == Character.Account.GuildMember || member.Account.Connection?.Player == null) continue;

                    member.Account.Connection.Enqueue(new S.GuildMemberOnline
                    {
                        Index = Character.Account.GuildMember.Index,
                        Name = Name,
                        ObjectID = ObjectID,
                        ObserverPacket = false
                    });
                }

                foreach (GuildAllianceInfo allianceInfo in Character.Account.GuildMember.Guild.Alliances)
                {
                    foreach (GuildMemberInfo member in allianceInfo.Guild1 == Character.Account.GuildMember.Guild ? allianceInfo.Guild2.Members : allianceInfo.Guild1.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildAllyOnline
                        {
                            Index = allianceInfo.Index,
                            ObserverPacket = false
                        });
                    }
                }
            }

            AddAllObjects();

            bool IsNew = false;
            if (Level == 0)
            {
                NewCharacter();
                IsNew = true;
            }

            if (Character.CanThrusting && Magics.ContainsKey(MagicType.Thrusting))
                Enqueue(new S.MagicToggle { Magic = MagicType.Thrusting, CanUse = true });

            if (Character.CanHalfMoon && Magics.ContainsKey(MagicType.HalfMoon))
                Enqueue(new S.MagicToggle { Magic = MagicType.HalfMoon, CanUse = true });

            if (Character.CanDestructiveSurge && Magics.ContainsKey(MagicType.DestructiveSurge))
                Enqueue(new S.MagicToggle { Magic = MagicType.DestructiveSurge, CanUse = true });

            if (Character.CanFlameSplash && Magics.ContainsKey(MagicType.FlameSplash))
                Enqueue(new S.MagicToggle { Magic = MagicType.FlameSplash, CanUse = true });

            List<ClientRefineInfo> refines = new List<ClientRefineInfo>();
            List<ClientRefineInfo> newRefines = new List<ClientRefineInfo>();
            foreach (RefineInfo info in Character.Refines)
            {
                if (info.IsNewWeaponUpgrade)
                    newRefines.Add(info.ToClientInfo());
                else
                    refines.Add(info.ToClientInfo());
            }

            if (refines.Count > 0)
                Enqueue(new S.RefineList { List = refines });
            if (newRefines.Count > 0)
                Enqueue(new S.RefineList { List = newRefines, IsNewWeaponUpgrade = true });

            Enqueue(new S.MarketPlaceConsign { Consignments = Character.Account.Auctions.Select(x => x.ToClientInfo(Character.Account)).ToList(), ObserverPacket = false });

            Enqueue(new S.MailList { Mail = Character.Account.Mail.Select(x => x.ToClientInfo()).ToList() });

            Enqueue(new S.FriendList { Friend = Character.Friends.Select(x => x.ToClientInfo()).ToList() });

            if (Character.Account.Characters.Max(x => x.Level) > Level && Character.Rebirth == 0 && Config.VeteranRate > 0)
                BuffAdd(BuffType.Veteran, TimeSpan.MaxValue, new Stats { [Stat.ExperienceRate] = Config.VeteranRate }, false, false, TimeSpan.Zero);  //回归者经验加成

            if (Character.Rebirth > 0)
            {
                BuffAdd(BuffType.AfterImages, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
            }

            Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold, ObserverPacket = false });  //元宝
            Enqueue(new S.PrestigeChanged { Prestige = Character.Prestige });    //声望
            Enqueue(new S.ContributeChanged { Contribute = Character.Contribute });  //贡献

            Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });  //赏金
            Enqueue(new S.AutoTimeChanged { AutoTime = Character.Account.AutoTime });

            Map map = SEnvir.GetMap(CurrentMap.Info.ReconnectMap);

            if (map != null && !InSafeZone)
                Teleport(map, map.GetRandomLocation());

            UpdateReviveTimers(Connection);

            CompanionSpawn();

            Enqueue(GetMarriageInfo());

            if (Character.Partner?.Player != null)
                Character.Partner.Player.Enqueue(new S.MarriageOnlineChanged { ObjectID = ObjectID });

            ApplyMapBuff();
            ApplyServerBuff();
            //ApplyCastleBuff();
            ApplyGuildBuff();
            ApplyGroupBuff();
            //ApplyObserverBuff();

            ApplyEventBuff();

            if (Config.EnableRewardPool)
            {
                ApplyRewardPoolBuff();
            }

            PauseBuffs();    //生成角色BUFF判断

            if (SEnvir.TopRankings.Contains(Character) && Character.Level > Config.RankingLevel - 1)
                BuffAdd(BuffType.Ranking, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);

            if (Character.Account.Admin || Character.Account.TempAdmin)
                BuffAdd(BuffType.Developer, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);

            Enqueue(new S.HelmetToggle { HideHelmet = Character.HideHelmet });  //显示头盔
            Enqueue(new S.ShieldToggle { HideShield = Character.HideShield });  //显示盾牌
            Enqueue(new S.FashionToggle { HideFashion = Character.HideFashion });  //显示时装

            //将战争日期发送到公会
            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)
            {
                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

                Enqueue(new S.GuildCastleInfo { Index = castle.Index, Owner = ownerGuild?.GuildName ?? String.Empty, ObserverPacket = false });
            }

            foreach (ConquestWar conquest in SEnvir.ConquestWars)
            {
                Point flagLocation = Point.Empty;
                if (conquest is ConquestWarFlag warFlag)
                {
                    flagLocation = warFlag.Flag?.CurrentLocation ?? Point.Empty;
                }

                Enqueue(new S.GuildConquestStarted { Index = conquest.info.Index, FlagLocation = flagLocation });
            }


            Enqueue(new S.FortuneUpdate { Fortunes = Character.Account.Fortunes.Select(x => x.ToClientInfo()).ToList() });

            //更新成就列表
            foreach (AchievementInfo achievement in Globals.AchievementInfoList.Binding)
            {
                if (Character.Achievements.All(x => x.AchievementName.Index != achievement.Index))
                {
                    UserAchievement userAchievement = SEnvir.UserAchievementList.CreateNewObject();
                    userAchievement.AchievementName = achievement;
                    userAchievement.Character = Character;
                    userAchievement.Completed = false;

                    foreach (AchievementRequirement requirement in achievement.AchievementRequirements)
                    {
                        UserAchievementRequirement userAchievementRequirement =
                            SEnvir.UserAchievementRequirementList.CreateNewObject();
                        userAchievementRequirement.Achievement = userAchievement;
                        userAchievementRequirement.Requirement = requirement;
                        userAchievementRequirement.CurrentValue = 0;
                    }
                }
                else
                {
                    //检查requirements
                    UserAchievement changedAchievement =
                        Character.Achievements.FirstOrDefault(y => y.AchievementName.Index == achievement.Index);

                    foreach (AchievementRequirement requirement in achievement.AchievementRequirements)
                    {
                        var temp = changedAchievement.AchievementRequirements.FirstOrDefault(z =>
                            z.Requirement.RequirementType == requirement.RequirementType);
                        if (temp == null || temp.Requirement.RequiredAmount != requirement.RequiredAmount)
                        {
                            changedAchievement.Delete();
                            break;
                        }
                    }
                }
            }

            if (AchievementTitle != null)
            {
                Broadcast(new S.AchievementTitleChanged
                {
                    ObjectID = ObjectID,
                    NewTitle = AchievementTitle
                });
            }

            Enqueue(new S.MapTime
            {
                OnOff = CurrentMap.MapTime > SEnvir.Now,
                MapRemaining = CurrentMap.MapTime - SEnvir.Now,
                ExpiryOnff = CurrentMap.Expiry > SEnvir.Now,
                ExpiryRemaining = CurrentMap.Expiry - SEnvir.Now,
            });

            //泰山buff
            TaishanBuffChanged();

            //发送神舰 赤龙开关门信息
            Enqueue(new S.GateInformation
            {
                NetherworldCloseTime = SEnvir.NetherworldCloseTime,
                LairCloseTime = SEnvir.LairCloseTime,
                NetherworldLocation = SEnvir.NetherworldLocation,
                LairLocation = SEnvir.LairLocation
            });

            //发送NPC外观设置
            foreach (var packet in SEnvir.UpdatedNPCLooks)
            {
                if (packet.CharacterIndex == 0 || this.Character.Index == packet.CharacterIndex)
                {
                    Enqueue(packet);
                }
            }

            // 发送天气设置
            foreach (var kvp in SEnvir.WeatherOverrides)
            {
                Enqueue(new S.ChangeWeather { MapIndex = kvp.Key, Weather = kvp.Value });
            }

            // 记录上线时间
            EnterGameTime = SEnvir.Now;

            //Item Links
            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, IsNew });
                    SEnvir.ExecutePyWithTimer(trig_play, this, "OnStartGame", args);
                    //trig_play(this, "OnStartGame", args);
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
        }
        private void NewCharacter()  //新建角色
        {
            Level = int.Parse(Config.NewLevel);   //默认上线级别

            ////如果账号角色只有1个，才加金币，否则金币为0
            Gold += Character.Account.Characters.Count < 2 ? int.Parse(Config.NewGold) : 0;    //默认上线金币
            if (int.Parse(Config.NewGold) > 0)
            {
                GoldChanged();

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "新建角色",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.ItemAdd,
                    Amount = int.Parse(Config.NewGold),
                    ExtraInfo = $"系统赠送金币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            //如果账号角色只有1个，才加元宝，否则元宝为0
            GameGold += Character.Account.Characters.Count < 2 ? int.Parse(Config.NewGameGold) : 0;  //默认上线元宝

            if ((Character.Account.Characters.Count < 2 ? int.Parse(Config.NewGameGold) : 0) > 0)
            {
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "新建角色",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.GameGold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.ItemAdd,
                    Amount = int.Parse(Config.NewGameGold),
                    ExtraInfo = $"系统赠送赞助币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            Prestige += int.Parse(Config.NewPrestige);  //默认上线声望
            Contribute += int.Parse(Config.NewContribute);  //默认上线贡献

            CraftLevel += 1; //制造等级
            CraftExp += 0; //制造经验
            CraftFinishTime = DateTime.Now; //制造物品完成时间
            BookmarkedCraftItemInfo = null; //收藏的制造物品
            CraftingItem = null;

            LevelUp();

            foreach (ItemInfo info in SEnvir.ItemInfoList.Binding)
            {
                if (!info.StartItem) continue;

                if (!CanStartWith(info)) continue;

                ItemCheck check = new ItemCheck(info, 1, UserItemFlags.Bound | UserItemFlags.Worthless, TimeSpan.Zero);

                if (CanGainItems(false, check))
                {
                    UserItem item = SEnvir.CreateFreshItem(check);

                    //记录物品来源
                    SEnvir.RecordTrackingInfo(item, "新人出生地".Lang(Connection.Language), ObjectType.NPC, "新人上线赠送".Lang(Connection.Language), Character?.CharacterName);

                    if (info.ItemType == ItemType.Armour)
                        item.Colour = Character.ArmourColour;

                    TimeSpan duration = TimeSpan.FromSeconds(info.Duration);   //duration 定义道具使用期限

                    if (duration != TimeSpan.Zero)  //如果时间使用期限不等0
                    {
                        item.Flags = UserItemFlags.Expirable;   //标签定义为时间限制
                        item.ExpireTime = duration;
                    }
                    else
                    {
                        item.Flags = UserItemFlags.None;   //给予的道具无标签
                    }

                    GainItem(item);
                }
            }

            RefreshStats();

            SetHP(Stats[Stat.Health]);
            SetMP(Stats[Stat.Mana]);

            Direction = MirDirection.Down;
        }
        private bool SetBindPoint()  //设置初始出生地点
        {
            if (Character.BindPoint != null && Character.BindPoint.ValidBindPoints.Count > 0)
                return true;

            List<SafeZoneInfo> spawnPoints = new List<SafeZoneInfo>();

            foreach (SafeZoneInfo info in SEnvir.SafeZoneInfoList.Binding)
            {
                if (info.ValidBindPoints.Count == 0) continue;

                switch (Class)
                {
                    case MirClass.Warrior:
                        if ((info.StartClass & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((info.StartClass & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((info.StartClass & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((info.StartClass & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                spawnPoints.Add(info);
            }

            if (spawnPoints.Count > 0)
                Character.BindPoint = spawnPoints[SEnvir.Random.Next(spawnPoints.Count)];

            return Character.BindPoint != null;
        }
        public void TownRevive() //城镇复活
        {
            if (!Dead) return;

            Cell cell = null;

            //攻方 指定复活点复活
            if (!Config.AttackerReviveInDesignatedAreaDuringWar && IsWarAttacker() && CurrentMap.Info.Index == 25)
            {
                var war = SEnvir.ConquestWars.FirstOrDefault(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == Character.Account.GuildMember.Guild.GuildName));
                if (war != null)
                {
                    Point randomPoint = war.info.AttackSpawnRegion.PointList[SEnvir.Random.Next(war.info.AttackSpawnRegion.PointList.Count)];
                    cell = SEnvir.Maps[war.Map.Info].GetCell(randomPoint);
                }
            }
            else if (!Config.AttackerReviveInDesignatedAreaDuringWar && IsWarPartake() && CurrentMap.Info.Index == 25)
            {
                var war = SEnvir.ConquestWars.FirstOrDefault(x => x.IsWaring && x.Participants.Exists(y => y.GuildName == Character.Account.GuildMember.Guild.GuildName));
                if (war != null)
                {
                    Point randomPoint = war.info.DefenderSpawnRegion.PointList[SEnvir.Random.Next(war.info.DefenderSpawnRegion.PointList.Count)];
                    cell = SEnvir.Maps[war.Map.Info].GetCell(randomPoint);
                }
            }
            else
            {
                cell = SEnvir.Maps[Character.BindPoint.BindRegion.Map].GetCell(Character.BindPoint.ValidBindPoints[SEnvir.Random.Next(Character.BindPoint.ValidBindPoints.Count)]);
            }

            if (cell == null)
            {
                SEnvir.Log("寻找复活点出错");
                return;
            }

            CurrentCell = cell.GetMovement(this);

            RemoveAllObjects();

            AddAllObjects();

            Dead = false;
            SetHP(Stats[Stat.Health] * 30 / 100);
            SetMP(Stats[Stat.Mana] * 30 / 100);

            SendShapeUpdate();

            Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = true });
        }

        /// <summary>
        /// 装备幻化Shape
        /// </summary>
        /// <param name="userItem"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public int GetItemIllusionItemShape(UserItem userItem, int DefaultValue = -1)
        {
            int result = DefaultValue;
            if (userItem != null)
            {
                if (userItem.Stats[Stat.Illusion] > 0)
                {
                    ItemInfo Info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == userItem.Stats[Stat.Illusion]);
                    result = Info.Shape;
                }
                else
                {
                    result = userItem.Info.Shape;
                }
            }

            return result;
        }
        /// <summary>
        /// 装备幻化Image
        /// </summary>
        /// <param name="userItem"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public int GetItemIllusionItemImage(UserItem userItem, int DefaultValue = -1)
        {
            int result = DefaultValue;
            if (userItem != null)
            {
                if (userItem.Stats[Stat.Illusion] > 0)
                {
                    ItemInfo Info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == userItem.Stats[Stat.Illusion]);
                    result = Info.Image;
                }
                else
                {
                    result = userItem.Info.Image;
                }
            }

            return result;
        }
    }
}
