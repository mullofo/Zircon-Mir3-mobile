using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject // 辅助
    {

        public List<AutoPotionLink> AutoPotions = new List<AutoPotionLink>();
        public List<AutoFightConfig> AutoFights = new List<AutoFightConfig>();
        public DateTime HPMPTime, DHPTime, DMPTime;

        // todo 刷新仓库
        /*public void SortBagItem()//刷新包裹
        {
            int ItemCount = 0;
            SortedDictionary<int, List<UserItem>> ItemSortList = new SortedDictionary<int, List<UserItem>>();

            for (int i = 0; i <= 42; i++) //这个42就是你打算加多少条项目
            {
                ItemSortList[i] = new List<UserItem>();
            }
            for (int i = 0; i < Globals.InventorySize; i++)
            {
                UserItem Item = Inventory[i];
                if (Item == null) continue;
                switch (Item.Info.ItemType)
                {
                    //这些类型大家改成自己的类型 List[0] 里面的数字按顺序加下去就可以了                   
                    case ItemType.Consumable: ItemSortList[0].Add(Item); break;         //消耗品
                    case ItemType.Weapon: ItemSortList[1].Add(Item); break;             //武器
                    case ItemType.Armour: ItemSortList[2].Add(Item); break;             //衣服                    
                    case ItemType.Helmet: ItemSortList[3].Add(Item); break;             //头盔
                    case ItemType.Necklace: ItemSortList[4].Add(Item); break;           //项链
                    case ItemType.Bracelet: ItemSortList[5].Add(Item); break;           //手镯
                    case ItemType.Ring: ItemSortList[6].Add(Item); break;               //戒指
                    case ItemType.Shoes: ItemSortList[7].Add(Item); break;              //鞋子
                    case ItemType.Shield: ItemSortList[8].Add(Item); break;              //盾牌
                    case ItemType.Emblem: ItemSortList[9].Add(Item); break;              //徽章
                    case ItemType.Book: ItemSortList[10].Add(Item); break;               //书籍
                    case ItemType.Poison: ItemSortList[11].Add(Item); break;              //毒药
                    case ItemType.Amulet: ItemSortList[12].Add(Item); break;              //护身符
                    case ItemType.Meat: ItemSortList[13].Add(Item); break;                //肉
                    case ItemType.Ore: ItemSortList[14].Add(Item); break;                 //矿石
                    case ItemType.Scroll: ItemSortList[15].Add(Item); break;             //卷轴
                    case ItemType.DarkStone: ItemSortList[16].Add(Item); break;           //宝石
                    case ItemType.RefineSpecial: ItemSortList[17].Add(Item); break;      //特殊精炼
                    case ItemType.HorseArmour: ItemSortList[18].Add(Item); break;        //马甲
                    case ItemType.Flower: ItemSortList[19].Add(Item); break;              //鲜花
                    case ItemType.CompanionFood: ItemSortList[20].Add(Item); break;       //宠物粮食
                    case ItemType.CompanionBag: ItemSortList[21].Add(Item); break;        //宠物背包
                    case ItemType.CompanionHead: ItemSortList[22].Add(Item); break;       //宠物头盔
                    case ItemType.CompanionBack: ItemSortList[23].Add(Item); break;       //宠物背带
                    case ItemType.System: ItemSortList[24].Add(Item); break;              //系统
                    case ItemType.Nothing: ItemSortList[25].Add(Item); break;            //无
                    case ItemType.Torch: ItemSortList[26].Add(Item); break;             //火把
                    case ItemType.ItemPart: ItemSortList[27].Add(Item); break;            //物品碎片
                    case ItemType.Hook: ItemSortList[28].Add(Item); break;                //鱼钩
                    case ItemType.Float: ItemSortList[29].Add(Item); break;               //鱼漂
                    case ItemType.Bait: ItemSortList[30].Add(Item); break;                //鱼饵
                    case ItemType.Finder: ItemSortList[31].Add(Item); break;              //探鱼器
                    case ItemType.Reel: ItemSortList[32].Add(Item); break;                //摇轮
                    case ItemType.Fish: ItemSortList[33].Add(Item); break;                //鱼
                    case ItemType.Barter: ItemSortList[34].Add(Item); break;                //以物换物标记
                    case ItemType.FameTitle: ItemSortList[35].Add(Item); break;           //声望称号
                    case ItemType.Fashion: ItemSortList[36].Add(Item); break;             //时装
                    case ItemType.Wing: ItemSortList[37].Add(Item); break;             //翅膀
                    case ItemType.Gem: ItemSortList[38].Add(Item); break;             //附魔石
                    case ItemType.Orb: ItemSortList[39].Add(Item); break;             //宝珠
                    case ItemType.Rune: ItemSortList[40].Add(Item); break;             //符文
                    case ItemType.Belt: ItemSortList[41].Add(Item); break;             //腰带
                    case ItemType.Drill: ItemSortList[42].Add(Item); break;             //穿孔材料

                    default: ItemSortList[42].Add(Item); break;
                }
                Inventory[i] = null;
            }
            List<UserItem> TY = new List<UserItem>();
            for (int i = 0; i <= 42; i++) //这个42就是你总共在上面加了多少条项目
            {
                TY = ItemSortList[i];
                foreach (var item in TY)
                {
                    item.Slot = ItemCount;
                    item.Character = Character;
                    Inventory[ItemCount] = item;
                    //Inventory[ItemCount].Slot = ItemCount;
                    ItemCount++;
                }
                ItemSortList[i].Clear();
            }
            TY.Clear();
            TY = null;
            ItemSortList = null;
            Enqueue(new S.InventoryRefresh { Items = Character.Items.Where(x => x.Slot <= Globals.InventorySize).Select(x => x.ToClientInfo()).ToList() });

        }*/
        public void InventoryRefresh(C.InventoryRefresh p)  //刷新包裹
        {
            var isPart = p.GridType == GridType.PatchGrid;
            UserItem[] fromArray = isPart ? PatchGrid : Inventory;

            var list = new List<UserItem>();
            var parts = fromArray.Where(ps => ps != null && ps.Info.ItemType == ItemType.ItemPart).ToArray();
            var items = fromArray.Where(ps => ps != null && ps.Info.ItemType != ItemType.ItemPart).ToArray();
            if (items.Any())
            {
                list.AddRange(OrderItems(items, false));
            }
            if (parts.Any())
            {
                list.AddRange(OrderItems(parts, true));
            }
            //数据库重新排序
            var newList = new List<ClientUserItem>();
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Slot = isPart ? Globals.PatchOffSet + i : i;
                fromArray[i] = list[i];
                newList.Add(list[i].ToClientInfo());
            }
            for (var i = list.Count; i < fromArray.Length; i++)
            {
                if (fromArray[i] != null)
                {
                    fromArray[i] = null;
                }

            }
            var result = new S.InventoryRefresh
            {
                GridType = p.GridType,
                Items = newList,
                Success = true
            };

            Enqueue(result);

            UpdateItemOnlyQuestTasks(); //刷包 更新任务进度
        }

        private List<UserItem> OrderItems(UserItem[] fromArray, bool isPart)
        {
            List<ItemGroup> itemGroups;  //列表道具组
            if (isPart)  //如果是碎片
            {
                itemGroups = fromArray.Where(from => from != null).GroupBy(v => v.Stats[Stat.ItemIndex])
                .Select(v => new ItemGroup { IndexWithFlags = new KeyValuePair<int, UserItemFlags>(v.Key, UserItemFlags.None), Count = v.Sum(m => m.Count) })
                .ToList();
            }
            else
            {
                itemGroups = fromArray.Where(from => from != null).GroupBy(v => new KeyValuePair<int, UserItemFlags>(v.Info.Index, v.Flags))
                .Select(v => new ItemGroup { IndexWithFlags = v.Key, Count = v.Sum(m => m.Count) })
                .ToList();
            }
            List<UserItem> list = new List<UserItem>();
            var itemList = SEnvir.ItemInfoList.Binding;
            for (var i = 0; i < fromArray.Length; i++)
            {
                if (fromArray[i] == null) continue;
                var itemGroup = isPart ? itemGroups.First(v => v.IndexWithFlags.Key == fromArray[i].Stats[Stat.ItemIndex]) : itemGroups.First(v => v.IndexWithFlags.Key == fromArray[i].Info.Index && v.IndexWithFlags.Value == fromArray[i].Flags);
                var size = fromArray[i].Info.StackSize;
                if (itemGroup.Count == 0)
                {
                    //已合并删除
                    if (fromArray[i] != null)
                    {
                        fromArray[i].Delete();
                        fromArray[i] = null;
                    }
                    continue;
                }
                if (size > itemGroup.Count)
                {
                    //最后一次合并
                    fromArray[i].Count = itemGroup.Count;
                    itemGroup.Count = 0;
                }
                else
                {
                    //合并一次
                    fromArray[i].Count = size;
                    itemGroup.Count -= size;
                }
                list.Add(fromArray[i]);
            }
            //排序
            list.Sort(delegate (UserItem x, UserItem y)
            {
                if (isPart)
                {
                    var newX = itemList.First(xx => xx.Index == x.Stats[Stat.ItemIndex]);
                    var newY = itemList.First(xx => xx.Index == y.Stats[Stat.ItemIndex]);
                    return newX.ItemType.CompareTo(newY.ItemType);
                }
                return x.Info.ItemType.CompareTo(y.Info.ItemType);
            });
            list.Sort(delegate (UserItem x, UserItem y)
            {
                if (isPart)
                {
                    var newX = itemList.First(xx => xx.Index == x.Stats[Stat.ItemIndex]);
                    var newY = itemList.First(xx => xx.Index == y.Stats[Stat.ItemIndex]);
                    return newX.Image.CompareTo(newY.Image);
                }
                return x.Info.Image.CompareTo(y.Info.Image);
            });
            return list;
        }

        public void ProcessAutoPotion() //自动喝药
        {
            //如果死亡跳出
            if (Dead) return;
            //定义 角色装备里的自动药剂装备格
            UserItem medicament = Equipment[(int)EquipmentSlot.Medicament];
            //如果 自动药剂为空  或  自动药剂的当前持久为0  跳出
            if (medicament == null || medicament.CurrentDurability == 0) return;

            //定义 获取道具设置里的加血值为最大的取值（要增加的血量值 等于 角色血量值-当前的血量值）
            int addhp = Math.Min(medicament.Info.Stats[Stat.MedicamentsHealth], Stats[Stat.Health] - CurrentHP);
            //定义   获取道具设置里的加蓝值为最大的取值（要增加的蓝值 等于 角色蓝值-当前的蓝值）
            int addmp = Math.Min(medicament.Info.Stats[Stat.MedicamentsMana], Stats[Stat.Mana] - CurrentMP);

            switch (medicament.Info.Shape)
            {
                case 0:  //加血和加蓝
                    if (SEnvir.Now >= HPMPTime)        //循环判断
                    {
                        if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                        {
                            HPMPTime = SEnvir.Now.AddSeconds(Config.MedicamentHPMPTime);   //时间间隔2秒 
                            ChangeHP(addhp);
                            ChangeMP(addmp);
                            DamageMedicament(addhp + addmp);
                        }
                    }
                    break;
                case 1:  //加血
                    if (SEnvir.Now >= DHPTime)        //循环判断
                    {
                        if (CurrentHP < Stats[Stat.Health])
                        {
                            DHPTime = SEnvir.Now.AddSeconds(Config.MedicamentHPTime);   //时间间隔3秒 
                            ChangeHP(addhp);
                            DamageMedicament(addhp);
                        }
                    }
                    break;
                case 2:  //加蓝
                    if (SEnvir.Now >= DMPTime)        //循环判断
                    {
                        if (CurrentMP < Stats[Stat.Mana])
                        {
                            DMPTime = SEnvir.Now.AddSeconds(Config.MedicamentMPTime);   //时间间隔3秒 
                            ChangeMP(addmp);
                            DamageMedicament(addmp);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 增加技能
        /// </summary>
        /// <param name="magicName">技能名字</param>
        public void LearnSkill(string magicName)
        {
            MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Name == magicName);
            if (info == null)
            {
                Connection.ReceiveChat($"PlayerObject.NoMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }

            if (Magics.TryGetValue(info.Magic, out UserMagic magic))
            {
                Connection.ReceiveChat($"PlayerObject.RepeatMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }

            magic = SEnvir.UserMagicList.CreateNewObject();
            magic.Character = Character;
            magic.Info = info;
            Magics[info.Magic] = magic;

            Enqueue(new S.NewMagic { Magic = magic.ToClientInfo() });

            Connection.ReceiveChat("Items.LearnBookSuccess".Lang(Connection.Language, magic.Info.Name), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("Items.LearnBookSuccess".Lang(con.Language, magic.Info.Name), MessageType.System);

            RefreshStats();
        }
        /// <summary>
        /// 调整技能等级
        /// </summary>
        /// <param name="magicName">技能名字</param>
        /// <param name="level">等级</param>
        public void ChangeSkillLevel(string magicName, int level)
        {
            MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Name == magicName);
            if (info == null)
            {
                Connection.ReceiveChat($"PlayerObject.NoMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }

            if (!Magics.TryGetValue(info.Magic, out UserMagic magic))
            {
                Connection.ReceiveChat($"PlayerObject.AdjustmentMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }
            else
            {
                magic.Level = level;
                Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });
                RefreshStats();
            }
        }
        /// <summary>
        /// 删除技能
        /// </summary>
        /// <param name="magicName">技能名字</param>
        public void ForgetSkill(string magicName)
        {
            MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Name == magicName);
            if (info == null)
            {
                Connection.ReceiveChat($"PlayerObject.NoMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }

            if (!Magics.TryGetValue(info.Magic, out UserMagic magic))
            {
                Connection.ReceiveChat($"PlayerObject.DeleteMagic".Lang(Connection.Language, magicName), MessageType.System);
                return;
            }
            else
            {
                Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = -1, Experience = 0 });

                Magics.Remove(magic.Info.Magic);
                Character.Magics.Remove(magic);
                magic.Delete();
                RefreshStats();
            }
        }

        public bool CheckMagic(string magicName)
        {
            if (string.IsNullOrEmpty(magicName)) return false;

            return Character.Magics.Any(x => x.Info.Name == magicName);
        }
    }
}
