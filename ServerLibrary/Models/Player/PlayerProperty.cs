using Library;
using Library.SystemModels;
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
    /// <summary>
    /// 人物属性相关对象
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 角色背包格子容量
        /// </summary>
        public UserItem[] Inventory = new UserItem[Globals.InventorySize];
        /// <summary>
        /// 碎片包裹格子容量
        /// </summary>
        public UserItem[] PatchGrid = new UserItem[1002];
        /// <summary>
        /// 人物装备格子
        /// </summary>
        public UserItem[] Equipment = new UserItem[Globals.EquipmentSize];
        /// <summary>
        /// 仓库格子容量
        /// </summary>
        public UserItem[] Storage = new UserItem[1000];
        /// <summary>
        /// 钓鱼装备格子
        /// </summary>
        public UserItem[] FishingEquipment = new UserItem[Globals.FishingEquipmentSize];
        /// <summary>
        /// 发型
        /// </summary>
        public int HairType
        {
            get { return Character.HairType; }
            set { Character.HairType = value; }
        }
        /// <summary>
        /// 发型颜色
        /// </summary>
        public Color HairColour
        {
            get { return Character.HairColour; }
            set { Character.HairColour = value; }
        }
        /// <summary>
        /// 记忆传送计数
        /// </summary>
        public int FixedPointTCount
        {
            get { return Character.FixedPointTCount; }
            set { Character.FixedPointTCount = value; }
        }
        /// <summary>
        /// 角色名字
        /// </summary>
        public override string Name
        {
            get { return Character.CharacterName; }
            set { Character.CharacterName = value; }
        }
        /// <summary>
        /// 角色佩戴成就称号
        /// </summary>
        public string AchievementTitle
        {
            get { return Character.AchievementTitle; }
            set { Character.AchievementTitle = value; }
        }
        /// <summary>
        /// 角色等级
        /// </summary>
        public override int Level
        {
            get { return Character.Level; }
            set { Character.Level = value; }
        }
        /// <summary>
        /// 角色性别
        /// </summary>
        public MirGender Gender => Character.Gender;
        /// <summary>
        /// 角色职业
        /// </summary>
        public MirClass Class => Character.Class;
        /// <summary>
        /// 当前HP
        /// </summary>
        public override int CurrentHP
        {
            get { return Character.CurrentHP; }
            set { Character.CurrentHP = value; }
        }
        /// <summary>
        /// 当前MP
        /// </summary>
        public override int CurrentMP
        {
            get { return Character.CurrentMP; }
            set { Character.CurrentMP = value; }
        }
        /// <summary>
        /// 金币
        /// </summary>
        public long Gold
        {
            get { return Character.Account.Gold; }
            set { Character.Account.Gold = value; }
        }
        /// <summary>
        /// 元宝
        /// </summary>
        public int GameGold
        {
            get { return Character.Account.GameGold; }
            set { Character.Account.GameGold = value; }
        }
        /// <summary>
        /// 赏金
        /// </summary>
        public int HuntGold
        {
            get { return Character.Account.HuntGold; }
            set { Character.Account.HuntGold = value; }
        }
        /// <summary>
        /// 声望
        /// </summary>
        public int Prestige
        {
            get { return Character.Prestige; }
            set { Character.Prestige = value; }
        }
        /// <summary>
        /// 荣誉
        /// </summary>
        public int Contribute
        {
            get { return Character.Contribute; }
            set { Character.Contribute = value; }
        }

        public long DayExpAdd
        {
            get { return Character.DayExpAdd; }
            set { Character.DayExpAdd = value; }
        }

        public long DayDonations
        {
            get { return Character.DayDonations; }
            set { Character.DayDonations = value; }
        }
        public long DayActiveCount
        {
            get { return Character.DayActiveCount; }
            set { Character.DayActiveCount = value; }
        }

        public long TotalActiveCount
        {
            get { return Character.TotalActiveCount; }
            set { Character.TotalActiveCount = value; }
        }

        public long GuildTotalActiveCount
        {
            get { return Character.Account.GuildMember.Guild.ActivCount; }
            set { Character.Account.GuildMember.Guild.ActivCount = value; }
        }

        public long GuildTotalDailyActiveCount
        {
            get { return Character.Account.GuildMember.Guild.DailyActivCount; }
            set { Character.Account.GuildMember.Guild.DailyActivCount = value; }
        }
        /// <summary>
        /// 经验值
        /// </summary>
        public decimal Experience
        {
            get { return Character.Experience; }
            set { Character.Experience = value; }
        }
        /// <summary>
        /// 背包重量
        /// </summary>
        public int BagWeight;
        /// <summary>
        /// 穿戴负重
        /// </summary>
        public int WearWeight;
        /// <summary>
        /// 手腕负重
        /// </summary>
        public int HandWeight;
        /// <summary>
        /// 最大经验值
        /// </summary>
        public decimal MaxExperience;
        /// <summary>
        /// 制造等级
        /// </summary>
        public int CraftLevel
        {
            get { return Character.CraftLevel; }
            set
            {
                Character.CraftLevel = value;
            }
        }
        /// <summary>
        /// 制造经验
        /// </summary>
        public int CraftExp
        {
            get { return Character.CraftExp; }
            set
            {
                Character.CraftExp = value;
                int newLevel = 1;
                // 匹配制造等级
                foreach (KeyValuePair<int, int> kvp in Globals.CraftExpDict)
                {
                    newLevel = kvp.Key;
                    if (kvp.Value > value)
                    {
                        break;
                    }
                }
                // 最高级
                CraftLevel = Math.Min(Globals.CraftExpDict.Count - 1, newLevel);
                //发包给客户端
                Enqueue(new S.CraftExpChanged { Exp = CraftExp, Level = CraftLevel });
            }
        }
        /// <summary>
        /// 制造完成时间
        /// </summary>
        public DateTime CraftFinishTime
        {
            get { return Character.CraftFinishTime; }
            set { Character.CraftFinishTime = value; }
        }
        /// <summary>
        /// 收藏的制造物品
        /// </summary>
        public CraftItemInfo BookmarkedCraftItemInfo
        {
            get { return Character.BookmarkedCraftItemInfo; }
            set { Character.BookmarkedCraftItemInfo = value; }
        }
        /// <summary>
        /// 正在进行的制造物品
        /// </summary>
        public CraftItemInfo CraftingItem
        {
            get { return Character.CraftingItem; }
            set { Character.CraftingItem = value; }
        }

        //回蓝延迟
        public TimeSpan MPRegenDelay { get; set; }
        public DateTime MPRegenTime { get; set; }

        #region 钓鱼

        public DateTime NextFishingCheckTime { get; set; } = DateTime.MinValue;

        private DateTime _fishingStartTime;
        /// <summary>
        /// 钓鱼开始时间
        /// </summary>
        public DateTime FishingStartTime
        {
            get => _fishingStartTime;
            set
            {
                _fishingStartTime = value;
            }
        }
        /// <summary>
        /// 钓鱼完美时间
        /// </summary>
        public DateTime FishingPerfectTime { get; set; }
        /// <summary>
        /// 钓鱼结束时间
        /// </summary>
        public DateTime FishingFinishTime { get; set; }
        /// <summary>
        /// 钓鱼列表刷新时间
        /// </summary>
        public DateTime FishListShuffleTime { get; set; }
        /// <summary>
        /// 是否找到鱼
        /// </summary>
        public bool FoundFish { get; set; }
        /// <summary>
        /// 钓鱼成功
        /// </summary>
        public bool FishingSuccess { get; set; }
        /// <summary>
        /// 总机会
        /// </summary>
        public int TotalNibbleChance => Math.Min(Stats[Stat.FishNibbleChance] + Globals.FishingBaseNibbleChance, 100);
        /// <summary>
        /// 总查找机会提高
        /// </summary>
        public int TotalFindingFailedAdd => Math.Min(Stats[Stat.FishFindingFailedAdd] + Globals.FishingBaseFindingFailedAdd, 100);
        /// <summary>
        /// 总成功机会
        /// </summary>
        public int TotalFindingChance => Math.Min(Stats[Stat.FishFindingChance] + Globals.FishingBaseFindingChance + (TotalFindingFailedAdd * FishingFindingFailedTimes), 100);
        /// <summary>
        /// 钓鱼失败次数
        /// </summary>
        public int FishingFindingFailedTimes { get; set; }

        /// <summary>
        /// 玩家是否正在钓鱼
        /// </summary>
        private bool _isFishing;
        public bool IsFishing
        {
            get => _isFishing;
            set
            {
                _isFishing = value;
                if (_isFishing)
                {
                    ResumeBuff(BuffType.FishingMaster);
                }
            }
        }
        /// <summary>
        /// 是否可以自动收杆
        /// </summary>
        public bool CanAutoReel => Buffs.Any(x => x.Type == BuffType.FishingMaster && !x.Pause);

        #endregion

        #region Change
        /// <summary>
        /// 性别改变
        /// </summary>
        /// <param name="p"></param>
        public void GenderChange(C.GenderChange p)
        {
            switch (p.Gender)
            {
                case MirGender.Male:  //男
                    if (Gender == MirGender.Male) return;
                    break;
                case MirGender.Female: //女
                    if (Gender == MirGender.Female) return;
                    break;
            }

            if (p.HairType < 0) return;

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255)) return;

            if (Equipment[(int)EquipmentSlot.Armour] != null)  //穿着衣服 跳出
            {
                Connection.ReceiveChat("GenderChange.Undress".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (Character.Partner != null)   //角色配偶不为空 跳出
            {
                Connection.ReceiveChat("GenderChange.AskForDivorce".Lang(Connection.Language), MessageType.System);
                return;
            }

            switch (Class)
            {
                case MirClass.Warrior: //战士
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Wizard: //法师
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Taoist: //道士
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Assassin: //刺客
                    if (p.HairType > 5) return;
                    break;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.GenderChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            Character.Gender = p.Gender;
            Character.HairType = p.HairType;
            Character.HairColour = p.HairColour;

            SendChangeUpdate();
        }
        /// <summary>
        /// 发型改变
        /// </summary>
        /// <param name="p"></param>
        public void HairChange(C.HairChange p)
        {
            if (p.HairType < 0) return;

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255)) return;

            switch (Class)
            {
                case MirClass.Warrior: //战士
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Wizard:  //法师
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Taoist:  //道士
                    if (p.HairType > (Gender == MirGender.Male ? 10 : 11)) return;
                    break;
                case MirClass.Assassin:  //刺客
                    if (p.HairType > 5) return;
                    break;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.HairChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            Character.HairType = p.HairType;
            Character.HairColour = p.HairColour;

            SendChangeUpdate();
        }
        /// <summary>
        /// 衣服染色
        /// </summary>
        /// <param name="colour"></param>
        public void ArmourDye(Color colour)
        {
            if (Equipment[(int)EquipmentSlot.Armour] == null) return;

            switch (Class)
            {
                case MirClass.Warrior:  //战士
                case MirClass.Wizard:   //法师
                case MirClass.Taoist:   //道士
                    if (colour.A != 255) return;
                    break;
                case MirClass.Assassin:
                    if (colour.ToArgb() != 0) return;
                    return;
            }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.ArmourDye) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            Equipment[(int)EquipmentSlot.Armour].Colour = colour;

            SendChangeUpdate();
        }
        /// <summary>
        /// 名字改变
        /// </summary>
        /// <param name="newName"></param>
        public void NameChange(string newName)
        {
            if (Config.CanUseChineseName)
            {
                if (!Globals.CharacterReg.IsMatch(newName))
                {
                    Connection.ReceiveChat("不能使用的名字".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }
            else
            {
                if (!Globals.EnCharacterReg.IsMatch(newName))
                {
                    Connection.ReceiveChat("不能使用的名字".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            if (newName == Name)
            {
                Connection.ReceiveChat($"PlayerObject.NameChange".Lang(Connection.Language, newName), MessageType.System);
                return;
            }

            for (int i = 0; i < SEnvir.CharacterInfoList.Count; i++)
                if (string.Compare(SEnvir.CharacterInfoList[i].CharacterName, newName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (SEnvir.CharacterInfoList[i].Account == Character.Account) continue;

                    Connection.ReceiveChat("该名字已在使用中".Lang(Connection.Language), MessageType.System);
                    return;
                }

            int index = 0;
            UserItem item = null;

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.NameChange) continue;

                if (!CanUseItem(Inventory[i])) continue;

                index = i;
                item = Inventory[i];
                break;
            }

            if (item == null) return;

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = index },
                Success = true
            };
            Enqueue(result);

            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[index] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            SEnvir.Log($"[名字更改] 旧名字: {Name}，新名字: {newName}。", true);

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat($"注意：【{Name}】改名为【{newName}】，请仇人、好友都认准。", MessageType.System);

            ClientFriendInfo friendInfoC = new ClientFriendInfo();
            IList<CharacterInfo> CharacterData = SEnvir.CharacterInfoList?.Binding;
            if (CharacterData != null && CharacterData.Count > 0)
            {
                try
                {
                    for (int i = Character.Friends.Count - 1; i >= 0; i--)
                    {
                        // 删除数据
                        // 删除发起者要删除的目标好友
                        FriendInfo target = null;
                        FriendInfo friendInfo = Character.Friends[i];
                        target = friendInfo;
                        Character.Friends.Remove(target);
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
                            isRequester = true,
                        });

                        CharacterInfo targetPlayerInfo = SEnvir.GetCharacter(target.Name);
                        PlayerObject targetPlayer = SEnvir.GetPlayerByCharacter(target.Name);

                        // 目标也要删除相应好友

                        target = null;

                        foreach (FriendInfo friendInfo1 in targetPlayerInfo.Friends)
                        {
                            if (friendInfo1.LinkID == friendInfo.LinkID)
                            {
                                target = friendInfo1;
                                break;
                            }
                        }

                        if (target == null)
                        {
                            Connection.ReceiveChat("找不到相应好友", MessageType.System);
                            continue;
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
                }
                finally
                {
                    Name = newName;

                    SendChangeUpdate();
                }
            }
            else
            {
                // 提供异常处理
                throw new ArgumentException($"删除好友失败:当前角色数据为空，请检查原因。");
            }
        }
        /// <summary>
        /// 财富检查变化
        /// </summary>
        /// <param name="index"></param>
        public void FortuneCheck(int index)
        {
            if (SEnvir.FortuneCheckerInfo == null) return;

            long count = GetItemCount(SEnvir.FortuneCheckerInfo);

            if (count == 0)
            {
                Connection.ReceiveChat("Movement.NeedItem".Lang(Connection.Language, SEnvir.FortuneCheckerInfo.ItemName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Movement.NeedItem".Lang(con.Language, SEnvir.FortuneCheckerInfo.ItemName), MessageType.System);
                return;
            }

            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == index);

            if (info == null || info.Drops.Count == 0) return;

            if (Config.TestServer && info.Effect != ItemEffect.Gold) return;

            UserFortuneInfo savedFortune = null;

            foreach (UserFortuneInfo fortune in Character.Account.Fortunes)
            {
                if (fortune.Item != info) continue;

                savedFortune = fortune;
                break;
            }

            TakeItem(SEnvir.FortuneCheckerInfo, 1);

            if (savedFortune == null)
            {
                savedFortune = SEnvir.UserFortuneInfoList.CreateNewObject();
                savedFortune.Account = Character.Account;
                savedFortune.Item = info;
            }

            UserDrop drop = Character.Account.UserDrops.FirstOrDefault(x => x.Item == info);

            savedFortune.CheckTime = SEnvir.Now;

            if (drop != null)
            {
                savedFortune.DropCount = drop.DropCount;
                savedFortune.DropProgress = drop.Progress;
            }

            Enqueue(new S.FortuneUpdate { Fortunes = new List<ClientFortuneInfo> { savedFortune.ToClientInfo() } });
        }


        #endregion

        /// <summary>
        /// 分配40级以后额外点数
        /// </summary>
        /// <param name="stat"></param>
        public void AssignHermit(Stat stat)
        {
            if (Level - 39 - Character.SpentPoints <= 0) return;  //如果 等级 -39 - 玩家消耗的额外点数 小于或者等于0 返回
            int str1 = 2;
            int str2 = Config.RandomAssign ? new Random().Next(10, 18) : 10;
            int str3 = Config.RandomAssign ? new Random().Next(15, 27) : 15;
            if (Config.RandomAssign)
            {
                str1 = 1;
                int temp = SEnvir.Random.Next(100);
                if (Character.SpentPoints <= 9)
                {
                    if (temp < 40)
                        str1 = 2;
                }
                else
                {
                    if (temp < 40)
                        str1 = 3;
                    else //if (temp < 40)
                        str1 = 2;
                }
            }

            switch (stat)
            {
                case Stat.MaxDC:  //加攻击
                    if (Config.AssignHermitMinACMR)
                    {
                        Character.HermitStats[Stat.MinDC] += str1;// + Character.SpentPoints / 10;
                    }
                    Character.HermitStats[Stat.MaxDC] += str1;// + Character.SpentPoints / 10;
                    Connection.ReceiveChat("破坏力 {0} 上升了。".Lang(Connection.Language, str1), MessageType.System);
                    break;
                case Stat.MaxMC:  //加自然
                    if (Config.AssignHermitMinACMR)
                    {
                        Character.HermitStats[Stat.MinMC] += str1;// + Character.SpentPoints / 10;
                    }
                    Character.HermitStats[Stat.MaxMC] += str1;// + Character.SpentPoints / 10;
                    Connection.ReceiveChat("自然界魔法 {0} 上升了。".Lang(Connection.Language, str1), MessageType.System);
                    break;
                case Stat.MaxSC:  //加灵魂
                    if (Config.AssignHermitMinACMR)
                    {
                        Character.HermitStats[Stat.MinSC] += str1;// + Character.SpentPoints / 10;
                    }
                    Character.HermitStats[Stat.MaxSC] += str1;// + Character.SpentPoints / 10;
                    Connection.ReceiveChat("灵魂界魔法 {0} 上升了。".Lang(Connection.Language, str1), MessageType.System);
                    break;
                case Stat.MaxAC:  //加防御
                    if (Config.AssignHermitMinACMR)
                    {
                        Character.HermitStats[Stat.MinAC] += str1;// + Character.SpentPoints / 10;
                    }
                    Character.HermitStats[Stat.MaxAC] += str1;// + Character.SpentPoints / 10;
                    Connection.ReceiveChat("防御力 {0} 上升了。".Lang(Connection.Language, str1), MessageType.System);
                    break;
                case Stat.MaxMR:  //加魔防
                    if (Config.AssignHermitMinACMR)
                    {
                        Character.HermitStats[Stat.MinMR] += str1;// + Character.SpentPoints / 10;
                    }
                    Character.HermitStats[Stat.MaxMR] += str1;// + Character.SpentPoints / 10;
                    Connection.ReceiveChat("魔法防御力 {0} 上升了。".Lang(Connection.Language, str1), MessageType.System);
                    break;
                case Stat.Health: //加血
                    Character.HermitStats[stat] += str2 + (Character.SpentPoints / 10) * 10;
                    Connection.ReceiveChat("体力 {0} 上升了。".Lang(Connection.Language, (str2 + (Character.SpentPoints / 10) * 10)), MessageType.System);
                    break;
                case Stat.Mana:  //加蓝
                    Character.HermitStats[stat] += str3 + (Character.SpentPoints / 10) * 15;
                    Connection.ReceiveChat("魔力 {0} 上升了。".Lang(Connection.Language, (str3 + (Character.SpentPoints / 10) * 15)), MessageType.System);
                    break;
                case Stat.WeaponElement:  //随机加攻击元素

                    //if (Character.SpentPoints >= 20) return;

                    int count = str1;// + Character.SpentPoints / 10;

                    List<Stat> Elements = new List<Stat>();

                    if (Config.RandomAssign)
                    {
                        if (Stats[Stat.FireAttack] >= 0) Elements.Add(Stat.FireAttack);
                        if (Stats[Stat.IceAttack] >= 0) Elements.Add(Stat.IceAttack);
                        if (Stats[Stat.LightningAttack] >= 0) Elements.Add(Stat.LightningAttack);
                        if (Stats[Stat.WindAttack] >= 0) Elements.Add(Stat.WindAttack);
                        if (Stats[Stat.HolyAttack] >= 0) Elements.Add(Stat.HolyAttack);
                        if (Stats[Stat.DarkAttack] >= 0) Elements.Add(Stat.DarkAttack);
                        if (Stats[Stat.PhantomAttack] >= 0) Elements.Add(Stat.PhantomAttack);
                    }
                    else
                    {
                        if (Stats[Stat.FireAttack] > 0) Elements.Add(Stat.FireAttack);
                        if (Stats[Stat.IceAttack] > 0) Elements.Add(Stat.IceAttack);
                        if (Stats[Stat.LightningAttack] > 0) Elements.Add(Stat.LightningAttack);
                        if (Stats[Stat.WindAttack] > 0) Elements.Add(Stat.WindAttack);
                        if (Stats[Stat.HolyAttack] > 0) Elements.Add(Stat.HolyAttack);
                        if (Stats[Stat.DarkAttack] > 0) Elements.Add(Stat.DarkAttack);
                        if (Stats[Stat.PhantomAttack] > 0) Elements.Add(Stat.PhantomAttack);
                    }

                    if (Elements.Count == 0)
                        Elements.AddRange(new[]
                        {
                            Stat.FireAttack,
                            Stat.IceAttack,
                            Stat.LightningAttack,
                            Stat.WindAttack,
                            Stat.HolyAttack,
                            Stat.DarkAttack,
                            Stat.PhantomAttack,
                        });

                    for (int i = 0; i < count; i++)
                    {
                        var element = Elements[SEnvir.Random.Next(Elements.Count)];
                        Character.HermitStats[element]++;
                        Connection.ReceiveChat("元素 {0} 属性 1 上升了。".Lang(Connection.Language, element.Lang(Connection.Language)), MessageType.System);
                    }
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "尝试利用额外加点";
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            Character.SpentPoints++;
            Broadcast(new S.ObjectUseItem { ObjectID = ObjectID });
            RefreshStats();
        }

        /// <summary>
        /// 处理回血事件
        /// </summary>

        private void ProcessHPRegen(float rate)
        {
            RegenTime = SEnvir.Now + RegenDelay;

            if (CurrentHP < Stats[Stat.Health])
            {
                int regen = (int)Math.Max(1, Stats[Stat.Health] * rate);

                ChangeHP(regen);
            }
        }

        private void ProcessMPRegen(float rate)
        {
            MPRegenTime = SEnvir.Now + MPRegenDelay;

            if (CurrentMP < Stats[Stat.Mana])
            {
                int regen = (int)Math.Max(1, Stats[Stat.Mana] * rate);

                ChangeMP(regen);
            }
        }

        public void ProcessRegen()
        {
            if (Dead) return;

            float rate = 2; //2%

            if (Class == MirClass.Wizard) rate += 1;
            bool flag = false;
            UserMagic magic = null;
            if (SEnvir.Now > CombatTime.AddSeconds(10) && CurrentHP < Stats[Stat.Health])
            {

                if (Magics.TryGetValue(MagicType.Rejuvenation, out magic) && Level >= magic.Info.NeedLevel1)
                {
                    rate += 0.5F + magic.Level * 0.5F;
                    flag = true;
                }
            }

            rate /= 100F;

            if (SEnvir.Now > RegenTime)
            {
                if (flag && magic != null)
                {
                    LevelMagic(magic);
                }
                ProcessHPRegen(rate);
            }

            if (SEnvir.Now > MPRegenTime)
            {
                ProcessMPRegen(rate);
            }

            if (SEnvir.Now >= HPTime && CurrentMap.Info.HealthRate != 0)        //循环判断
            {
                if (CurrentHP <= Stats[Stat.Health])
                {
                    HPTime = SEnvir.Now.AddSeconds(Config.HPTime);   //时间为5秒                
                    CurrentHP += Stats[Stat.BHealth];           //地图设置加血或者减血
                }
            }
        }
    }
}
