using Library;
using Library.Network;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //玩家对象
    {
        /*
        public ScriptSource NpcSource;
        public ScriptScope Npcscope;
        public Func<object, object> OnDayChange;
        public Func<object, object, object> OnEnter;
        public Func<object, object, object> OnDie;
        */
        public override ObjectType Race => ObjectType.Player;
        /// <summary>
        /// 角色信息
        /// </summary>
        public CharacterInfo Character;
        /// <summary>
        /// 连接
        /// </summary>
        public SConnection Connection;
        /// <summary>
        /// 客户端平台
        /// </summary>
        public Platform ClientPlatform = Platform.Desktop;
        /*
        public void LoadScript()
        {
            try
            {

                    NpcSource = SEnvir.engine.CreateScriptSourceFromFile("./Scripts/PlayerProcess.py" );
                    Npcscope = SEnvir.engine.CreateScope();

                    Npcscope.SetVariable("SEnvir_MapInfoList", SEnvir.MapInfoList);
                    Npcscope.SetVariable("SEnvir_CreateMap", (Func<int, Map>)SEnvir.CreateMap);
                    Npcscope.SetVariable("SEnvir_CloseMap", (Func<MapInfo, int>)SEnvir.CloseMap);
                    NpcSource.Execute(Npcscope);
                    Npcscope.TryGetVariable("OnDayChange", out OnDayChange);
                    Npcscope.TryGetVariable("OnEnter", out OnEnter);
                    Npcscope.TryGetVariable("OnDie", out OnDie);



            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
        */

        //时间
        public DateTime ShoutTime,
            UseItemTime,
            TorchTime,
            CombatTime,
            PvPTime,
            SentCombatTime,
            AutoPotionTime,
            AutoPotionCheckTime,
            ItemTime,
            FlamingSwordTime,
            DragonRiseTime,
            BladeStormTime,
            MaelstromBladeTime,
            RevivalTime,
            TeleportTime,
            AutoTime,
            HPTime,
            CraftTime;

        public bool PacketWaiting;  //数据包等待

        public bool CanPowerAttack, GameMaster, Observer;  //控制攻击  游戏管理员 观察者

        public override bool Blocking => base.Blocking && !Observer;

        public HorseType Horse;      //坐骑类型

        public bool ExtractorLock; //提取锁定

        public override bool CanAttack => base.CanAttack && Horse == HorseType.None;   //可攻击
        public override bool CanCast => base.CanCast && Horse == HorseType.None;  //可精炼


        public MapObject LastHitter;

        public PlayerObject GroupInvitation,
            GuildInvitation, MarriageInvitation, FriendInvitation;

        public Dictionary<MagicType, UserMagic> Magics = new Dictionary<MagicType, UserMagic>();

        //当前技能
        public MagicType CurrentMagic = MagicType.None;


        public bool[] setConfArr = new bool[(int)AutoSetConf.SetMaxConf];
        public CellLinkInfo DelayItemUse;

        public bool CanFlamingSword, CanDragonRise, CanBladeStorm, CanDefiance, CanMight, CanMaelstromBlade;

        public decimal SwiftBladeLifeSteal, FlameSplashLifeSteal, DestructiveSurgeLifeSteal;

        /// <summary>
        /// 本次上线时间 0点重置
        /// </summary>
        public DateTime EnterGameTime
        {
            get
            {
                return _enterGameTime;
            }
            set => _enterGameTime = value;
        }

        private DateTime _enterGameTime = DateTime.MaxValue;
        /// <summary>
        /// 本次上线累计时间
        /// </summary>
        public TimeSpan SessionDuration => SEnvir.Now - EnterGameTime;
        /// <summary>
        /// 本次上线累计分钟数
        /// </summary>
        public double SessionDurationInMinutes => Math.Floor(SessionDuration.TotalMinutes);
        /// <summary>
        /// 本日累计在线时长
        /// </summary>
        public double TodayOnlineInMinutes => Character.TodayOnlineMinutes + SessionDurationInMinutes;
        public PlayerObject(CharacterInfo info, SConnection con)
        {
            Character = info;
            Connection = con;

            DisplayMP = CurrentMP;
            DisplayHP = CurrentHP;

            Character.LastStats = Stats = new Stats();

            foreach (UserItem item in Character.Account.Items)
                Storage[item.Slot] = item;

            //foreach (UserItem item in Character.Account.Items)
            //PatchGrid[item.Slot] = item;

            for (int i = 0; i < Character.Items.Count; i++)
            {
                UserItem item = Character.Items[i];
                if (item.Info == null)
                {
                    Character.Items.RemoveAt(i);
                    i--;
                }
            }

            foreach (UserItem item in Character.Items)
            {

                if (item.Slot >= Globals.FishingOffSet)
                {
                    FishingEquipment[item.Slot - Globals.FishingOffSet] = item;
                    continue;
                }
                if (item.Slot >= Globals.PatchOffSet)
                {
                    PatchGrid[item.Slot - Globals.PatchOffSet] = item;
                    continue;
                }
                if (item.Slot >= Globals.EquipmentOffSet)
                {
                    Equipment[item.Slot - Globals.EquipmentOffSet] = item;
                    continue;
                }

                if (item.Slot < Inventory.Length)
                    Inventory[item.Slot] = item;
                else
                {
                    //RemoveItem(item);
                    //item.IsTemporary = true;
                    //item.Delete();
                    SEnvir.Log(string.Format("物品slot:{0}出错,自动修正", item.Slot));
                    item.Slot += Globals.PatchOffSet;
                    item.Account = null;
                    item.Character = Character;
                }
            }

            ItemReviveTime = info.ItemReviveTime;
            ItemTime = SEnvir.Now;

            foreach (UserMagic magic in Character.Magics)
                Magics[magic.Info.Magic] = magic;
            foreach (UserValue uvalue in Character.Values)
                Values[uvalue.Key] = uvalue;
            Buffs.AddRange(Character.Account.Buffs);
            Buffs.AddRange(Character.Buffs);

            AutoPotions.AddRange(Character.AutoPotionLinks);

            //旧喝药
            //AutoPotions.Sort((x1, x2) => x1.Slot.CompareTo(x2.Slot));
            //新喝药
            AutoPotions.Sort((x1, x2) => x1.Health.CompareTo(x2.Health));
            AutoPotions.Sort((x1, x2) => x1.Mana.CompareTo(x2.Mana));

            AutoFights.AddRange(Character.AutoFightLinks);

            FPoints.AddRange(Character.FPointLinks);  //获取角色传送栏记录

            if (Character.Account.Admin || Character.Account.TempAdmin)
            {
                GameMaster = true;
                Observer = true;
            }

            #region 注册Player相关的事件Listener

            SEnvir.EventManager.RegisterListener(EventTypes.PlayerMove, this.OnPlayerMove);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerHarvest, this.OnPlayerHarvest);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerAttack, this.OnPlayerAttack);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerUseMagic, this.OnPlayerMagic);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerLifeSteal, this.OnPlayerLifeSteal);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerOnline, this.OnPlayerOnline);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerMine, this.OnPlayerMine);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerGainItem, this.OnPlayerGainItem);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerDodge, this.OnPlayerDodge);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerKillMonster, this.OnPlayerKillMonster);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerRankingChange, this.OnPlayerPlayerRankingChange);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerWeaponRefine, this.OnPlayerWeaponRefine);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerWeaponReset, this.OnPlayerWeaponReset);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerAccessoryRefineLevel, this.OnPlayerAccessoryRefineLevel);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerMarry, this.OnPlayerMarriage);
            SEnvir.EventManager.RegisterListener(EventTypes.PlayerDie, this.OnPlayerKilled);
            SEnvir.EventManager.RegisterListener(EventTypes.ItemMove, this.OnPlayerItemMove);
            #endregion
        }

        public void Inspect(int index, SConnection con)  //检查
        {
            if (index == Character.Index) return;

            CharacterInfo target = SEnvir.GetCharacter(index);

            if (target == null) return;

            S.Inspect packet = new S.Inspect
            {
                Name = target.CharacterName,
                Partner = target.Partner?.CharacterName,
                Class = target.Class,
                Gender = target.Gender,
                Stats = target.LastStats,
                HermitStats = target.HermitStats,
                HermitPoints = Math.Max(0, target.Level - 39 - target.SpentPoints),
                Level = target.Level,

                Hair = target.HairType,
                HairColour = target.HairColour,
                Items = new List<ClientUserItem>(),
                ObserverPacket = false,

                HideHelmet = target.HideHelmet,
                HideFashion = target.HideFashion,
                HideShield = target.HideShield,
            };

            if (target.Account.GuildMember != null)
            {
                packet.GuildName = target.Account.GuildMember.Guild.GuildName;
                packet.GuildRank = target.Account.GuildMember.Rank;
                packet.GuildFlag = target.Account.GuildMember.Guild.GuildFlag;
                packet.GuildFlagColor = target.Account.GuildMember.Guild.FlagColor;
            }

            if (target.Player != null)
            {
                packet.WearWeight = target.Player.WearWeight;
                packet.HandWeight = target.Player.HandWeight;
            }


            foreach (UserItem item in target.Items)
            {
                if (item == null || item.Slot < 0 || item.Slot < Globals.EquipmentOffSet || item.Slot >= Globals.PatchOffSet) continue;

                ClientUserItem clientItem = item.ToClientInfo();
                clientItem.Slot -= Globals.EquipmentOffSet;

                packet.Items.Add(clientItem);
            }

            con.Enqueue(packet);
        }

        public void Enqueue(Packet p) => Connection.Enqueue(p);

        public override Packet GetInfoPacket(PlayerObject ob)  //获取信息包
        {
            if (ob == this) return null;
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];   //武器
            UserItem armour = Equipment[(int)EquipmentSlot.Armour];   //衣服

            S.ObjectPlayer p = new S.ObjectPlayer
            {
                Index = Character.Index,

                ObjectID = ObjectID,
                Name = Name,
                AchievementTitle = Character.AchievementTitle,   //成就称号
                GuildName = Character.Account.GuildMember?.Guild.GuildName,
                NameColour = NameColour,
                Location = CurrentLocation,
                Direction = Direction,

                Light = Stats[Stat.Light],
                Dead = Dead,

                Class = Class,
                Gender = Gender,
                HairType = HairType,
                HairColour = HairColour,

                //Weapon = Equipment[(int)EquipmentSlot.Weapon]?.Info.Shape ?? -1, //武器
                Weapon = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Weapon]), //武器幻化

                WeaponImage = Equipment[(int)EquipmentSlot.Weapon]?.Info.Image ?? 0,  //武器图像
                WeaponIndex = Equipment[(int)EquipmentSlot.Weapon]?.Info.Index ?? 0,  //武器index

                Shield = Equipment[(int)EquipmentSlot.Shield]?.Info.Shape ?? -1,   //盾牌

                Emblem = Equipment[(int)EquipmentSlot.Emblem]?.Info.Shape ?? -1,  //徽章效果

                //Armour = Equipment[(int)EquipmentSlot.Armour]?.Info.Shape ?? 0,  //衣服效果
                Armour = GetItemIllusionItemShape(Equipment[(int)EquipmentSlot.Armour], 0), //衣服幻化

                ArmourColour = Equipment[(int)EquipmentSlot.Armour]?.Colour ?? Color.Empty, //衣服颜色
                ArmourImage = Equipment[(int)EquipmentSlot.Armour]?.Info.Image ?? 0,   //衣服图像
                ArmourIndex = Equipment[(int)EquipmentSlot.Armour]?.Info.Index ?? 0,   //衣服index

                Poison = Poison,

                Buffs = Character.Buffs.Where(x => x.Visible).Select(x => x.Type).ToList(),
                CustomIndexs = Character.Buffs.Where(x => x.Type == BuffType.CustomBuff).Select(x => x.FromCustomBuff).ToList(),

                Horse = Horse,
                HorseType = Character.Horse,

                Helmet = Character.HideHelmet ? 0 : Equipment[(int)EquipmentSlot.Helmet]?.Info.Shape ?? 0,   //头盔

                Fashion = Character.HideFashion ? -1 : Equipment[(int)EquipmentSlot.Fashion]?.Info.Shape ?? -1, //时装

                FashionImage = Equipment[(int)EquipmentSlot.Fashion]?.Info.Image ?? 0,   //时装图像

                HorseShape = Equipment[(int)EquipmentSlot.HorseArmour]?.Info.Shape ?? 0,

                CraftLevel = Character.CraftLevel,
                CraftExp = Character.CraftExp,
                CraftFinishTime = Character.CraftFinishTime,
                BookmarkedCraftItemInfo = Character.BookmarkedCraftItemInfo,
            };
            return p;
        }
        public override Packet GetDataPacket(PlayerObject ob)
        {
            return new S.DataObjectPlayer
            {
                ObjectID = ObjectID,

                MapIndex = CurrentMap.Info.Index,
                CurrentLocation = CurrentLocation,

                Name = Name,

                Health = DisplayHP,
                MaxHealth = Stats[Stat.Health],
                Dead = Dead,

                Mana = DisplayMP,
                MaxMana = Stats[Stat.Mana]
            };
        }


        public void InspectPackSack(int index, SConnection con)
        {
            if (index == Character.Index)
            {
                return;
            }
            CharacterInfo character = SEnvir.GetCharacter(index);
            if (character == null)
            {
                return;
            }
            S.InspectPackSack inspect = new S.InspectPackSack
            {
                Name = character.CharacterName,
                Items = new List<ClientUserItem>(),
                ObserverPacket = false,
            };

            foreach (UserItem item in character.Items)
            {
                if (item != null && item.Slot >= 0 && item.Slot < 1000)
                {
                    ClientUserItem clientUserItem = item.ToClientInfo();
                    clientUserItem.Slot = item.Slot;
                    inspect.Items.Add(clientUserItem);
                }
            }
            con.Enqueue(inspect);
        }


        public void InspectMagery(int index, SConnection con)
        {
            if (index == Character.Index)
            {
                return;
            }
            CharacterInfo character = SEnvir.GetCharacter(index);
            if (character == null)
            {
                return;
            }
            List<UserMagic> userMagicList = null;
            userMagicList = SEnvir.UserMagicList.Binding.Where((UserMagic x) => x.Character == character).ToList();

            S.InspectMagery inspect = new S.InspectMagery
            {
                Name = character.CharacterName,
                Items = new List<ClientUserMagic>(),
                ObserverPacket = false,
            };

            foreach (UserMagic magic in userMagicList)
            {
                if (magic != null)
                {
                    ClientUserMagic clientUserItem = magic.ToClientInfo();
                    inspect.Items.Add(clientUserItem);
                }
            }
            con.Enqueue(inspect);
        }

        public void MarketPlaceJiaoseBuy(C.MarketPlaceJiaoseBuy p)
        {
            if (p.Count <= 0 || p.Index <= 0)
            {
                return;
            }

            int index = (int)p.Index;
            if (index == Character.Index)
            {
                return;
            }
            CharacterInfo character = SEnvir.GetCharacter(index);
            if (character == null)
            {
                return;
            }
            if (character.Account == Character.Account)
            {
                Connection.ReceiveChat("同一账户不能购买", MessageType.System);
                return;
            }

            int chrcount = 0;  //INT 计数为0

            foreach (CharacterInfo chr in Character.Account.Characters)   //字符信息 字符输入  账户的角色名
            {
                if (chr.Deleted) continue; //如果（角色名 删除）继续

                if (++chrcount < Globals.MaxCharacterCount) continue;  //如果 (++计数 < 全局变量 最大的角色数 ) 继续

                Connection.ReceiveChat("你的账号已经有两个角色无法购买", MessageType.System);   //提示 角色名超过字符数限制
                return;    //返回
            }
            CharacterShop characterShop = SEnvir.CharacterShopList.Binding.FirstOrDefault(x => x.Character == character && x.Character.CharacterState == CharacterState.Sell && x.IsSell);
            if (characterShop == null)
            {
                Connection.ReceiveChat("角色已经交易或者取回", MessageType.System);
                return;
            }

            long count = characterShop.Price;

            if (count * 100 > Character.Account.GameGold)
            {
                Connection.ReceiveChat("MarketPlace.StoreCost".Lang(Connection.Language), MessageType.System);
                return;
            }

            //扣除购买者元宝
            Character.Account.GameGold -= (int)count * 100;
            GameGoldChanged();

            //增加角色寄售人元宝
            characterShop.Account.GameGold += (int)count * 100;
            characterShop.Account?.Connection?.Enqueue(new S.GameGoldChanged { GameGold = characterShop.Account.GameGold });

            character.Account = Character.Account;
            character.CharacterState = CharacterState.Normal;

            characterShop.BuyDate = SEnvir.Now;
            characterShop.BuyAccount = Character.Account;
            characterShop.IsSell = false;

            S.MarketPlaceJiaoseBuy marketPlaceBuy = new S.MarketPlaceJiaoseBuy
            {
                Success = true,
            };
            Enqueue(marketPlaceBuy);
            Connection.ReceiveChat("购买角色成功！", MessageType.System);

            foreach (SConnection con in SEnvir.Connections)
            {
                con.ReceiveChat("恭喜 {0} 花费 {1} 赞助币购买了角色 <{2}> 。".Lang(con.Language, Character.CharacterName, count, character.CharacterName), MessageType.System);
                con.ReceiveChat("恭喜 {0} 花费 {1} 赞助币购买了角色 <{2}> 。".Lang(con.Language, Character.CharacterName, count, character.CharacterName), MessageType.System);
                con.ReceiveChat("恭喜 {0} 花费 {1} 赞助币购买了角色 <{2}> 。".Lang(con.Language, Character.CharacterName, count, character.CharacterName), MessageType.System);
                con.ReceiveChat("恭喜 {0} 花费 {1} 赞助币购买了角色 <{2}> 。".Lang(con.Language, Character.CharacterName, count, character.CharacterName), MessageType.System);
                con.ReceiveChat("恭喜 {0} 花费 {1} 赞助币购买了角色 <{2}> 。".Lang(con.Language, Character.CharacterName, count, character.CharacterName), MessageType.System);
            }
        }
    }
}