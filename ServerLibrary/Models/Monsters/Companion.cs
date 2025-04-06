using Library;
using Library.Network;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 怪物对象 宠物
    /// </summary>
    public sealed class Companion : MonsterObject
    {
        public override bool Blocking => false;
        /// <summary>
        /// 宠物包裹负重
        /// </summary>
        public int BagWeight;
        /// <summary>
        /// 目标道具
        /// </summary>
        public ItemObject TargetItem;
        /// <summary>
        /// 角色宠物
        /// </summary>
        public UserCompanion UserCompanion;
        /// <summary>
        /// 宠物主人
        /// </summary>
        public PlayerObject CompanionOwner;
        /// <summary>
        /// 宠物等级信息
        /// </summary>
        public CompanionLevelInfo LevelInfo;
        /// <summary>
        /// 包裹
        /// </summary>
        public UserItem[] Inventory;
        /// <summary>
        /// 装备
        /// </summary>
        public UserItem[] Equipment;
        /// <summary>
        /// 捡取分类按道具类别
        /// </summary>
        public bool[] Sortings;
        /// <summary>
        /// 捡取分类按道具品质
        /// </summary>
        public bool[] SortingLevs;
        /// <summary>
        /// 是否带捡取过滤
        /// </summary>
        public bool bSort;
        /// <summary>
        /// 宠物头盔
        /// </summary>
        private int HeadShape;
        /// <summary>
        /// 宠物背饰
        /// </summary>
        private int BackShape;

        /// <summary>
        /// 角色宠物
        /// </summary>
        /// <param name="companion"></param>
        public Companion(UserCompanion companion)
        {
            Visible = false;
            PreventSpellCheck = true;
            UserCompanion = companion;

            MonsterInfo = companion.Info.MonsterInfo;

            Equipment = new UserItem[Globals.CompanionEquipmentSize];

            foreach (UserItem item in companion.Items)
            {
                if (item.Slot < Globals.EquipmentOffSet) continue;

                if (item.Slot - Globals.EquipmentOffSet >= Equipment.Length)
                {
                    SEnvir.Log($"[宠物包裹装备] 位置: {item.Slot}, 角色名: {UserCompanion.Character.CharacterName}, 宠物名: {UserCompanion.Name}");
                    continue;
                }

                if (item.Info.ItemType == ItemType.CompanionHead)
                {
                    HeadShape = item.Info.Shape;
                }
                else if (item.Info.ItemType == ItemType.CompanionBack)
                {
                    BackShape = item.Info.Shape;
                }

                Equipment[item.Slot - Globals.EquipmentOffSet] = item;
            }

            Inventory = new UserItem[Globals.CompanionInventorySize];

            foreach (UserItem item in companion.Items)
            {
                if (item.Slot >= Globals.EquipmentOffSet) continue;

                if (item.Slot >= Inventory.Length)
                {
                    SEnvir.Log($"[宠物包裹清单] 位置: {item.Slot}, 角色名: {UserCompanion.Character.CharacterName}, 宠物名: {UserCompanion.Name}");
                    continue;
                }

                Inventory[item.Slot] = item;
            }
            Sortings = new bool[(int)ItemType.Medicament + 1];  //宠物捡取类别
            for (int i = 0; i < Sortings.Length; i++)
            {
                Sortings[i] = true;
            }
            foreach (UserSorting sort in companion.Sortings)
            {
                if (sort.SetType > ItemType.Medicament) continue;   //宠物捡取类别
                Sortings[(int)sort.SetType] = sort.Enabled;
            }

            SortingLevs = new bool[(int)Rarity.Elite + 1];
            for (int i = 0; i < SortingLevs.Length; i++)
            {
                SortingLevs[i] = true;
            }
            foreach (UserSortingLev sort in companion.SortingsLev)
            {
                if (sort.SetType > Rarity.Elite) continue;
                SortingLevs[(int)sort.SetType] = sort.Enabled;
            }
            bSort = companion.Info.Sorting;
        }

        /// <summary>
        /// AI处理过程
        /// </summary>
        public override void ProcessAI()
        {
            if (!CompanionOwner.VisibleObjects.Contains(this))
                Recall();

            if (TargetItem?.Node == null || TargetItem.CurrentMap != CurrentMap || !Functions.InRange(CurrentLocation, TargetItem.CurrentLocation, ViewRange))
                TargetItem = null;

            ProcessSearch();
            ProcessRoam();
            ProcessTarget();
        }

        /// <summary>
        /// 刷怪属性状态
        /// </summary>
        public override void RefreshStats()
        {
            Stats.Clear();
            Stats.Add(MonsterInfo.Stats);

            LevelInfo = SEnvir.CompanionLevelInfoList.Binding.FirstOrDefault(x => x.Level == UserCompanion.Level && x.MonsterInfo == MonsterInfo);

            if (LevelInfo == null)
            {
                SEnvir.Log($"[宠物信息错误] 宠物等级信息设置错误, 角色名: {UserCompanion.Character.CharacterName}, 宠物名: {UserCompanion.Name}");
                return;
            }

            MoveDelayBase = MonsterInfo.MoveDelay;
            AttackDelayBase = MonsterInfo.AttackDelay;

            foreach (UserItem item in Equipment)
            {
                if (item == null) continue;

                Stats.Add(item.Info.Stats);
                Stats.Add(item.Stats);
            }

            Stats[Stat.CompanionBagWeight] += LevelInfo.InventoryWeight;

            Stats[Stat.CompanionInventory] = Stats[Stat.CompanionInventory] + LevelInfo.InventorySpace > Inventory.Length ? Inventory.Length : Stats[Stat.CompanionInventory] + LevelInfo.InventorySpace;

            RefreshWeight();
        }

        /// <summary>
        /// 刷新包裹负重
        /// </summary>
        public void RefreshWeight()
        {
            BagWeight = 0;

            foreach (UserItem item in Inventory)
            {
                if (item == null) continue;

                BagWeight += item.Weight;

                if (item.Info.ItemType == ItemType.CompanionHead)
                {
                    HeadShape = item.Info.Shape;
                }
                else if (item.Info.ItemType == ItemType.CompanionBack)
                {
                    BackShape = item.Info.Shape;
                }
            }

            CompanionOwner.Enqueue(new S.CompanionWeightUpdate { BagWeight = BagWeight, MaxBagWeight = Stats[Stat.CompanionBagWeight], InventorySize = Stats[Stat.CompanionInventory] });
        }

        /// <summary>
        /// 发送形态更新
        /// </summary>
        public void SendShapeUpdate()
        {
            HeadShape = 0;
            BackShape = 0;

            foreach (UserItem item in Equipment)
            {
                if (item == null) continue;

                if (item.Info.ItemType == ItemType.CompanionHead)
                {
                    HeadShape = item.Info.Shape;
                }
                else if (item.Info.ItemType == ItemType.CompanionBack)
                {
                    BackShape = item.Info.Shape;
                }
            }

            Broadcast(new S.CompanionShapeUpdate { ObjectID = ObjectID, HeadShape = HeadShape, BackShape = BackShape });
        }

        /// <summary>
        /// 召回
        /// </summary>
        public void Recall()
        {
            if (UserCompanion == null) return;  //如果角色宠物为空 跳过

            if (CompanionOwner == null) return;   //如果宠物主人为空 跳过

            Cell cell = CompanionOwner.CurrentMap.GetCell(Functions.Move(CompanionOwner.CurrentLocation, CompanionOwner.Direction, -1));

            if (cell == null || cell.Movements != null)
                cell = CompanionOwner.CurrentCell;

            Teleport(CompanionOwner.CurrentMap, cell.Location);
        }
        /// <summary>
        /// 进程搜索
        /// </summary>
        public override void ProcessSearch()
        {
            if (!CanMove || SEnvir.Now < SearchTime) return;

            int bestDistance = int.MaxValue;  //最佳距离

            List<ItemObject> closest = new List<ItemObject>();  //最近的道具

            foreach (MapObject ob in CompanionOwner.VisibleObjects)
            {
                if (ob.Race != ObjectType.Item) continue;

                int distance = Functions.Distance(ob.CurrentLocation, CurrentLocation);

                if (distance > ViewRange) continue;

                if (distance > bestDistance) continue;


                ItemObject item = (ItemObject)ob;

                if (item.Account != CompanionOwner.Character.Account || !item.MonsterDrop) continue;

                //过滤的不再考虑
                if (CompanionOwner.CompanionPickUpSkips.Contains(item.Item.Info.Index)) continue;

                long amount = 0;

                if (item.Item.Info.Effect == ItemEffect.Gold && item.Account.GuildMember != null && item.Account.GuildMember.Guild.GuildTax > 0)
                    amount = (long)Math.Ceiling(item.Item.Count * item.Account.GuildMember.Guild.GuildTax);

                ItemCheck check = new ItemCheck(item.Item, item.Item.Count - amount, item.Item.Flags, item.Item.ExpireTime);

                if (!CanGainItems(true, check)) continue;

                if (distance != bestDistance) closest.Clear();

                closest.Add(item);
                bestDistance = distance;
            }

            if (closest.Count == 0)
            {
                SearchTime = SEnvir.Now.AddSeconds(10);
                return;
            }

            TargetItem = closest[SEnvir.Random.Next(closest.Count)];

        }
        /// <summary>
        /// 移动到对应坐标
        /// </summary>
        public override void ProcessRoam()
        {
            if (TargetItem != null) return;

            MoveTo(Functions.Move(CompanionOwner.CurrentLocation, CompanionOwner.Direction, -1));
        }
        /// <summary>
        /// 移动到目标道具捡取物品
        /// </summary>
        public override void ProcessTarget()
        {
            if (TargetItem == null) return;

            MoveTo(TargetItem.CurrentLocation);

            if (TargetItem.CurrentLocation != CurrentLocation) return;

            if (CompanionOwner?.CompanionPickUpSkips == null || TargetItem?.Item?.Info == null) return;
            // 过滤的不处理
            if (CompanionOwner.CompanionPickUpSkips.Contains(TargetItem.Item.Info.Index))
            {
                return;
            }
            TargetItem.PickUpItem(this);
        }

        /// <summary>
        /// 激活
        /// </summary>
        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        /// <summary>
        /// 取消激活
        /// </summary>
        public override void DeActivate()
        {
            return;
        }
        /// <summary>
        /// 移除道具
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(UserItem item)
        {
            item.Slot = -1;
            item.Character = null;
            item.Account = null;
            item.Mail = null;
            item.Auction = null;
            item.Companion = null;
            item.Guild = null;


            item.Flags &= ~UserItemFlags.Locked;
        }
        /// <summary>
        /// 穿戴道具
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool CanWearItem(UserItem item, CompanionSlot slot)
        {
            if (!Functions.CorrectSlot(item.Info.ItemType, slot) || !CanUseItem(item.Info))
                return false;

            return true;
        }
        /// <summary>
        /// 使用道具需求
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CanUseItem(ItemInfo info)
        {
            switch (info.RequiredType)
            {
                case RequiredType.CompanionLevel:
                    if (UserCompanion.Level < info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel:
                    if (UserCompanion.Level > info.RequiredAmount) return false;
                    break;
            }

            return true;
        }
        /// <summary>
        /// 宠物自动进食
        /// </summary>
        public void AutoFeed()
        {
            if (UserCompanion.Hunger > 0) return;

            UserItem item = Equipment[(int)CompanionSlot.Food];

            if (item == null || !CanUseItem(item.Info)) return;


            UserCompanion.Hunger = Math.Min(LevelInfo.MaxHunger, item.Info.Stats[Stat.CompanionHunger]);

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.CompanionEquipment, Slot = (int)CompanionSlot.Food },
                Success = true,
            };

            CompanionOwner.Enqueue(result);
            if (item.Count > 1)
            {
                item.Count--;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Equipment[(int)CompanionSlot.Food] = null;
                item.Delete();

                result.Link.Count = 0;
            }
        }
        /// <summary>
        /// 技能变化
        /// </summary>
        public void CheckSkills()
        {
            bool result = false;

            if (UserCompanion.Level >= 3 && (UserCompanion.Level3 == null || UserCompanion.Level3.Count == 0))
            {
                UserCompanion.Level3 = GetSkill(3);
                result = true;
            }

            if (UserCompanion.Level >= 5 && (UserCompanion.Level5 == null || UserCompanion.Level5.Count == 0))
            {
                UserCompanion.Level5 = GetSkill(5);
                result = true;
            }

            if (UserCompanion.Level >= 7 && (UserCompanion.Level7 == null || UserCompanion.Level7.Count == 0))
            {
                UserCompanion.Level7 = GetSkill(7);
                result = true;
            }

            if (UserCompanion.Level >= 10 && (UserCompanion.Level10 == null || UserCompanion.Level10.Count == 0))
            {
                UserCompanion.Level10 = GetSkill(10);
                result = true;
            }

            if (UserCompanion.Level >= 11 && (UserCompanion.Level11 == null || UserCompanion.Level11.Count == 0))
            {
                UserCompanion.Level11 = GetSkill(11);
                result = true;
            }

            if (UserCompanion.Level >= 13 && (UserCompanion.Level13 == null || UserCompanion.Level13.Count == 0))
            {
                UserCompanion.Level13 = GetSkill(13);
                result = true;
            }

            if (UserCompanion.Level >= 15 && (UserCompanion.Level15 == null || UserCompanion.Level15.Count == 0))
            {
                UserCompanion.Level15 = GetSkill(15);
                result = true;
            }

            CompanionOwner.CompanionRefreshBuff();

            if (!result) return;

            CompanionOwner.Enqueue(new S.CompanionSkillUpdate
            {
                Level3 = UserCompanion.Level3,
                Level5 = UserCompanion.Level5,
                Level7 = UserCompanion.Level7,
                Level10 = UserCompanion.Level10,
                Level11 = UserCompanion.Level11,
                Level13 = UserCompanion.Level13,
                Level15 = UserCompanion.Level15
            });

        }
        /// <summary>
        /// 获取技能
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Stats GetSkill(int level)
        {
            int total = 0;

            foreach (CompanionSkillInfo info in SEnvir.CompanionSkillInfoList.Binding)
            {
                if (info.Level != level) continue;

                if (info.MonsterInfo != MonsterInfo) continue;

                total += info.Weight;
            }


            Stats lvStats = new Stats();

            int value = SEnvir.Random.Next(total);

            foreach (CompanionSkillInfo info in SEnvir.CompanionSkillInfoList.Binding)
            {
                if (info.Level != level) continue;

                if (info.MonsterInfo != MonsterInfo) continue;

                value -= info.Weight;

                if (value >= 0) continue;

                lvStats[info.StatType] = SEnvir.Random.Next(info.MaxAmount) + 1;

                break;
            }


            return lvStats;
        }
        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="target"></param>
        public override void MoveTo(Point target)
        {
            if (!CanMove || CurrentLocation == target) return;

            MirDirection direction = Functions.DirectionFromPoint(CurrentLocation, target);

            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

            for (int d = 0; d < 8; d++)
            {
                if (Walk(direction)) return;

                direction = Functions.ShiftDirection(direction, rotation);
            }
        }
        /// <summary>
        /// 走
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public override bool Walk(MirDirection direction)
        {
            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction));
            if (cell == null) return false;

            BuffRemove(BuffType.Invisibility);
            BuffRemove(BuffType.Transparency);

            Direction = direction;

            UpdateMoveTime();

            CurrentCell = cell;//.GetMovement(this);

            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectMove { ObjectID = ObjectID, Direction = direction, Location = CurrentLocation, Distance = 1 });
            return true;
        }
        /// <summary>
        /// 可以获得道具
        /// </summary>
        /// <param name="checkWeight"></param>
        /// <param name="checks"></param>
        /// <returns></returns>
        public bool CanGainItems(bool checkWeight, params ItemCheck[] checks)
        {
            int index = 0;
            foreach (ItemCheck check in checks)
            {
                //if (bSort)
                //{
                //    if (check.Info.ItemType == ItemType.Nothing )
                //    {
                //        if (check.Info.Effect == ItemEffect.Gold)
                //        {
                //            if (!Sortings[(int)check.Info.ItemType]) return false;
                //        }

                //    }
                //    else
                //    {
                //        if (!Sortings[(int)check.Info.ItemType]) return false;
                //        if (!SortingLevs[(int)check.Info.Rarity]) return false;
                //    }
                //}
                if ((check.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                long count = check.Count;

                if (check.Info.Effect == ItemEffect.Experience) continue;
                if (check.Info.Effect == ItemEffect.Gold) continue;

                try
                {
                    if (checkWeight)
                    {
                        switch (check.Info.ItemType)
                        {
                            case ItemType.Amulet:
                            case ItemType.Poison:
                                if (BagWeight + check.Info.Weight > Stats[Stat.CompanionBagWeight]) return false;
                                break;
                            default:
                                if (BagWeight + check.Info.Weight * count > Stats[Stat.CompanionBagWeight]) return false;
                                break;
                        }
                    }

                    if (check.Info.StackSize > 1 && (check.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                    {
                        foreach (UserItem oldItem in Inventory)
                        {
                            if (oldItem == null) continue;

                            if (oldItem.Info != check.Info || oldItem.Count >= check.Info.StackSize) continue;

                            if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                            if ((oldItem.Flags & UserItemFlags.Bound) != (check.Flags & UserItemFlags.Bound)) continue;
                            if ((oldItem.Flags & UserItemFlags.Worthless) != (check.Flags & UserItemFlags.Worthless)) continue;
                            if ((oldItem.Flags & UserItemFlags.NonRefinable) != (check.Flags & UserItemFlags.NonRefinable)) continue;
                            if (!oldItem.Stats.Compare(check.Stats)) continue;

                            count -= check.Info.StackSize - oldItem.Count;

                            if (count <= 0) break;
                        }

                        if (count <= 0) break;
                    }
                }
                catch
                {
                }

                //Start Index
                for (int i = index; i < Stats[Stat.CompanionInventory]; i++)
                {
                    index++;
                    UserItem item = Inventory[i];
                    if (item == null)
                    {
                        count -= check.Info.StackSize;

                        if (count <= 0) break;
                    }
                }

                if (count > 0) return false;
            }

            return true;
        }
        /// <summary>
        /// 获得道具
        /// </summary>
        /// <param name="allItems"></param>
        public void GainItem(params UserItem[] allItems)
        {
            //剥离碎片
            UserItem[] itemsWithoutParts = allItems.Where(x => x.Info.Effect != ItemEffect.ItemPart).ToArray();
            UserItem[] itemParts = allItems.Where(x => x.Info.Effect == ItemEffect.ItemPart).ToArray();
            //碎片视为人物拾取
            CompanionOwner.GainItem(itemParts);

            CompanionOwner.Enqueue(new S.CompanionItemsGained { Items = itemsWithoutParts.Where(x => x.Info.Effect != ItemEffect.Experience).Select(x => x.ToClientInfo()).ToList() });

            HashSet<UserQuest> changedQuests = new HashSet<UserQuest>();

            foreach (UserItem item in itemsWithoutParts)
            {
                if (item.UserTask != null)
                {
                    if (item.UserTask.Completed) continue;

                    item.UserTask.Amount = Math.Min(item.UserTask.Task.Amount, item.UserTask.Amount + item.Count);

                    changedQuests.Add(item.UserTask.Quest);

                    if (item.UserTask.Completed)
                    {
                        for (int i = item.UserTask.Objects.Count - 1; i >= 0; i--)
                            item.UserTask.Objects[i].Despawn();
                    }

                    item.UserTask = null;
                    item.Flags &= ~UserItemFlags.QuestItem;


                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                if (item.Info.Effect == ItemEffect.Gold)
                {
                    CompanionOwner.Gold += item.Count;
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                if (item.Info.Effect == ItemEffect.Experience)
                {
                    CompanionOwner.GainExperience(item.Count, false);
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                bool handled = false;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    foreach (UserItem oldItem in Inventory)
                    {
                        if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;


                        if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!oldItem.Stats.Compare(item.Stats)) continue;

                        if (oldItem.Count + item.Count <= item.Info.StackSize)
                        {
                            oldItem.Count += item.Count;
                            item.IsTemporary = true;
                            item.Delete();
                            handled = true;
                            break;
                        }

                        item.Count -= item.Info.StackSize - oldItem.Count;
                        oldItem.Count = item.Info.StackSize;
                    }
                    if (handled) continue;
                }

                for (int i = 0; i < Stats[Stat.CompanionInventory]; i++)
                {
                    if (Inventory[i] != null) continue;

                    Inventory[i] = item;
                    item.Slot = i;
                    item.Companion = UserCompanion;
                    item.IsTemporary = false;
                    break;
                }
            }

            foreach (UserQuest quest in changedQuests)
                CompanionOwner.Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });


            RefreshStats();
        }
        /// <summary>
        /// 可以被看到
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanBeSeenBy(PlayerObject ob)
        {
            if (ob == CompanionOwner)
                return base.CanBeSeenBy(ob);

            return CompanionOwner != null && CompanionOwner.CanBeSeenBy(ob);
        }

        public override bool CanDataBeSeenBy(PlayerObject ob)
        {
            return false;
        }

        public override Packet GetInfoPacket(PlayerObject ob)   //获取信息包
        {
            return new S.ObjectMonster
            {
                ObjectID = ObjectID,
                MonsterIndex = MonsterInfo.Index,

                Location = CurrentLocation,

                NameColour = NameColour,
                Direction = Direction,

                PetOwner = CompanionOwner.Name,

                Poison = Poison,

                Buffs = Buffs.Where(x => x.Visible).Select(x => x.Type).ToList(),

                CompanionObject = new ClientCompanionObject
                {
                    Name = UserCompanion.Name,
                    HeadShape = HeadShape,
                    BackShape = BackShape,
                }
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)   //获取数据包
        {
            return null;
        }
    }
}
