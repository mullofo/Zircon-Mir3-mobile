using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using Server.Models.Monsters;
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 角色道具物品
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        private DateTime delaytime;
        /// <summary>
        /// 定点传送道具的信息
        /// </summary>
        public List<FixedPointInfo> FPoints = new List<FixedPointInfo>();
        /// <summary>
        /// 传奇宝箱道具信息
        /// </summary>
        public List<ItemInfo> TreaItems = new List<ItemInfo>();
        /// <summary>
        /// 传奇宝箱数量
        /// </summary>
        public int TreaCount;
        /// <summary>
        /// 是否为新传奇宝箱
        /// </summary>
        public bool TreaNew;
        /// <summary>
        /// 传奇宝箱更改计数
        /// </summary>
        public int TreaChangeCount;
        /// <summary>
        /// 传奇宝箱单事件锁定计数
        /// </summary>
        public int TreaSelCount;
        /// <summary>
        /// 传奇宝箱打开次数最大值
        /// </summary>
        public bool[] TreaBoxOn = new bool[15];

        /// <summary>
        /// 火炬照明处理过程
        /// </summary>
        public void ProcessTorch()
        {
            if (SEnvir.Now <= TorchTime || InSafeZone) return;

            TorchTime = SEnvir.Now.AddSeconds(10);

            DamageItem(GridType.Equipment, (int)EquipmentSlot.Torch, Config.TorchRate);

            UserItem torch = Equipment[(int)EquipmentSlot.Torch];
            if (torch == null || torch.CurrentDurability != 0 || torch.Info.Durability <= 0) return;

            RemoveItem(torch);
            Equipment[(int)EquipmentSlot.Torch] = null;
            torch.Delete();

            RefreshWeight();

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Torch },
                Success = true,
            });
        }

        private bool CheckIllusionItem(GridType gridType)
        {

            bool refresh = false;
            UserItem[] array;
            switch (gridType)
            {
                case GridType.Inventory:  //背包
                    array = Inventory;
                    break;
                case GridType.Equipment:  //人物装备栏
                    array = Equipment;
                    break;
                case GridType.Storage:   //仓库
                    array = Storage;
                    break;
                case GridType.GuildStorage:   //行会仓库
                    if (Character.Account.GuildMember == null) return false;
                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return false;

                    array = Character.Account.GuildMember.Guild.Storage;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return false;

                    array = Companion.Inventory;
                    break;
                default:
                    return false;
            }
            TimeSpan ticks = SEnvir.Now - ItemTime;
            for (int i = 0; i < array.Length; i++)
            {
                UserItem item = array[i];
                if (item == null) continue;
                //如果是武器栏判断是否为幻化物品
                if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                {
                    TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                    item.IllusionExpireTime -= remainTicks;
                    if (item.IllusionExpireTime <= TimeSpan.Zero)
                    {
                        item.Stats[Stat.Illusion] = 0;//去除幻化
                        SendShapeUpdate();
                        S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                        {
                            GridType = gridType,
                            Slot = (int)i,
                            NewStats = new Stats(item.Stats)
                        };
                        result.FullItemStats = item.ToClientInfo().FullItemStats;
                        Enqueue(result);
                    }
                }
                //===============================================
                //以下为判断限时装备是否到期
                if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                if (Config.InSafeZoneItemExpire)
                {
                    TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                    item.ExpireTime -= remainTicks;
                }
                else
                    item.ExpireTime -= ticks;
                //System.Diagnostics.Debug.WriteLine(SEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                if (item.ExpireTime > TimeSpan.Zero) continue;

                Connection.ReceiveChat("Items.Expired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Items.Expired".Lang(con.Language, item.Info.ItemName), MessageType.System);
                //=================
                //到期则删除装备
                RemoveItem(item);
                array[i] = null;
                item.Delete();

                SendShapeUpdate();

                refresh = true;

                Enqueue(new S.ItemChanged
                {
                    Link = new CellLinkInfo { GridType = gridType, Slot = i },
                    Success = true,
                });
            }
            return refresh;
        }
        /// <summary>
        /// 道具过期处理过程
        /// </summary>
        public void ProcessItemExpire()
        {
            try
            {
                if (ItemTime.AddSeconds(1) > SEnvir.Now) return;

                TimeSpan ticks = SEnvir.Now - ItemTime;
                ItemTime = SEnvir.Now;

                if (!Config.InSafeZoneItemExpire && InSafeZone) return;  //如果安全区不计时   且  是在安全区

                bool refresh = false;

                for (int i = 0; i < Equipment.Length; i++)
                {
                    UserItem item = Equipment[i];
                    if (item == null) continue;
                    //如果是武器栏判断是否为幻化物品
                    //if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                    //{
                    //    TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                    //    item.IllusionExpireTime -= remainTicks;
                    //    if (item.IllusionExpireTime <= TimeSpan.Zero)
                    //    {
                    //        item.Stats[Stat.Illusion] = 0;//去除幻化
                    //        SendShapeUpdate();
                    //        S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                    //        {
                    //            GridType = GridType.Equipment,
                    //            Slot = (int)EquipmentSlot.Weapon,
                    //            NewStats = new Stats(Equipment[(int)EquipmentSlot.Weapon].Stats)
                    //        };
                    //        result.FullItemStats = Equipment[(int)EquipmentSlot.Weapon].ToClientInfo().FullItemStats;
                    //        Enqueue(result);
                    //    }
                    //}
                    if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                    {
                        TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                        item.IllusionExpireTime -= remainTicks;
                        if (item.IllusionExpireTime <= TimeSpan.Zero)
                        {
                            item.Stats[Stat.Illusion] = 0;//去除幻化
                            SendShapeUpdate();
                            S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                            {
                                GridType = GridType.Equipment,
                                Slot = (int)i,
                                NewStats = new Stats(item.Stats)
                            };
                            result.FullItemStats = item.ToClientInfo().FullItemStats;
                            Enqueue(result);
                        }
                    }

                    if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                    if (Config.InSafeZoneItemExpire)
                    {
                        TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                        item.ExpireTime -= remainTicks;
                    }
                    else
                        item.ExpireTime -= ticks;
                    //System.Diagnostics.Debug.WriteLine(SEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    if (item.ExpireTime > TimeSpan.Zero) continue;

                    Connection.ReceiveChat("Items.Expired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);
                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Items.Expired".Lang(con.Language, item.Info.ItemName), MessageType.System);

                    RemoveItem(item);
                    Equipment[i] = null;
                    item.Delete();

                    SendShapeUpdate();

                    refresh = true;

                    Enqueue(new S.ItemChanged
                    {
                        Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = i },
                        Success = true,
                    });
                }

                for (int i = 0; i < Inventory.Length; i++)
                {
                    UserItem item = Inventory[i];
                    if (item == null) continue;
                    //如果是武器栏判断是否为幻化物品
                    if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                    {
                        TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                        item.IllusionExpireTime -= remainTicks;
                        if (item.IllusionExpireTime <= TimeSpan.Zero)
                        {
                            item.Stats[Stat.Illusion] = 0;//去除幻化
                            SendShapeUpdate();
                            S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                            {
                                GridType = GridType.Inventory,
                                Slot = (int)i,
                                NewStats = new Stats(item.Stats)
                            };
                            result.FullItemStats = item.ToClientInfo().FullItemStats;
                            Enqueue(result);
                        }
                    }
                    if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                    if (Config.InSafeZoneItemExpire)
                    {
                        TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                        item.ExpireTime -= remainTicks;
                    }
                    else
                        item.ExpireTime -= ticks;
                    //System.Diagnostics.Debug.WriteLine(SEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    if (item.ExpireTime > TimeSpan.Zero) continue;

                    Connection.ReceiveChat("Items.Expired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);
                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Items.Expired".Lang(con.Language, item.Info.ItemName), MessageType.System);

                    RemoveItem(item);
                    Inventory[i] = null;
                    item.Delete();

                    refresh = true;

                    Enqueue(new S.ItemChanged
                    {
                        Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i },
                        Success = true,
                    });
                }
                if (CheckIllusionItem(GridType.Storage))
                {
                    refresh = true;
                }

                if (CheckIllusionItem(GridType.GuildStorage))
                {
                    refresh = true;
                }
                if (Companion != null)
                {
                    for (int i = 0; i < Companion.Inventory.Length; i++)
                    {
                        UserItem item = Companion.Inventory[i];
                        if (item == null) continue;
                        //如果是武器栏判断是否为幻化物品
                        if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                        {
                            TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                            item.IllusionExpireTime -= remainTicks;
                            if (item.IllusionExpireTime <= TimeSpan.Zero)
                            {
                                item.Stats[Stat.Illusion] = 0;//去除幻化
                                SendShapeUpdate();
                                S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                                {
                                    GridType = GridType.CompanionInventory,
                                    Slot = (int)i,
                                    NewStats = new Stats(item.Stats)
                                };
                                result.FullItemStats = item.ToClientInfo().FullItemStats;
                                Enqueue(result);
                            }
                        }
                        if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        if (Config.InSafeZoneItemExpire)
                        {
                            TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                            item.ExpireTime -= remainTicks;
                        }
                        else
                            item.ExpireTime -= ticks;
                        //System.Diagnostics.Debug.WriteLine(SEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        if (item.ExpireTime > TimeSpan.Zero) continue;

                        Connection.ReceiveChat("Items.Expired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Items.Expired".Lang(con.Language, item.Info.ItemName), MessageType.System);

                        RemoveItem(item);
                        Companion.Inventory[i] = null;
                        item.Delete();

                        refresh = true;

                        Enqueue(new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i },
                            Success = true,
                        });
                    }
                    for (int i = 0; i < Companion.Equipment.Length; i++)
                    {
                        UserItem item = Companion.Equipment[i];
                        if (item == null) continue;
                        //如果是武器栏判断是否为幻化物品
                        if (item.Stats[Stat.Illusion] > 0)//物品是幻化物品
                        {
                            TimeSpan remainTicks = item.IllusionExpireTime - (item.IllusionExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                            item.IllusionExpireTime -= remainTicks;
                            if (item.IllusionExpireTime <= TimeSpan.Zero)
                            {
                                item.Stats[Stat.Illusion] = 0;//去除幻化
                                SendShapeUpdate();
                                S.ItemStatsRefreshed result = new S.ItemStatsRefreshed
                                {
                                    GridType = GridType.CompanionEquipment,
                                    Slot = (int)i,
                                    NewStats = new Stats(item.Stats)
                                };
                                result.FullItemStats = item.ToClientInfo().FullItemStats;
                                Enqueue(result);
                            }
                        }
                        if ((item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) continue;

                        if (Config.InSafeZoneItemExpire)
                        {
                            TimeSpan remainTicks = item.ExpireTime - (item.ExpireDateTime - SEnvir.Now); //根据过期日期换算出tick
                            item.ExpireTime -= remainTicks;
                        }
                        else
                            item.ExpireTime -= ticks;
                        //System.Diagnostics.Debug.WriteLine(SEnvir.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + item.ExpireTime + "  " + item.ExpireDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        if (item.ExpireTime > TimeSpan.Zero) continue;

                        Connection.ReceiveChat("Items.Expired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Items.Expired".Lang(con.Language, item.Info.ItemName), MessageType.System);

                        RemoveItem(item);
                        Companion.Equipment[i] = null;
                        item.Delete();

                        refresh = true;

                        Enqueue(new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = GridType.CompanionEquipment, Slot = i },
                            Success = true,
                        });
                    }


                }

                if (refresh)
                    RefreshStats();
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
        /// <summary>
        /// 使用道具复活
        /// </summary>
        public override void ItemRevive()
        {
            base.ItemRevive();

            Character.ItemReviveTime = ItemReviveTime;

            UpdateReviveTimers(Connection);

            //删除复活道具
            var flower = Equipment[(int)EquipmentSlot.Flower];
            //判断是否有ItemReviveTime属性
            if (flower != null && flower.Info.Stats[Stat.ItemReviveTime] == 1)
            {
                //可以删除
                RemoveEquippedItem((int)EquipmentSlot.Flower);
            }

            var torch = Equipment[(int)EquipmentSlot.Torch];
            //判断是否有ItemReviveTime属性
            if (torch != null && torch.Info.Stats[Stat.ItemReviveTime] == 1)
            {
                //可以删除
                RemoveEquippedItem((int)EquipmentSlot.Torch);
            }
        }
        /// <summary>
        /// 复活时间更新
        /// </summary>
        /// <param name="con"></param>
        public void UpdateReviveTimers(SConnection con)
        {
            con.Enqueue(new S.ReviveTimers
            {
                ItemReviveTime = ItemReviveTime > SEnvir.Now ? ItemReviveTime - SEnvir.Now : TimeSpan.Zero,
                ReincarnationPillTime = Character.ReincarnationPillTime > SEnvir.Now ? Character.ReincarnationPillTime - SEnvir.Now : TimeSpan.Zero,
            });
        }

        #region Items

        /// <summary>
        /// 道具部分解析链接
        /// </summary>
        /// <param name="links">链接</param>
        /// <param name="minCount">最小计数</param>
        /// <param name="maxCount">最大计数</param>
        /// <returns></returns>
        public bool ParseLinks(List<CellLinkInfo> links, int minCount, int maxCount)
        {
            if (links == null || links.Count < minCount || links.Count > maxCount) return false;

            List<CellLinkInfo> tempLinks = new List<CellLinkInfo>();

            foreach (CellLinkInfo link in links)
            {
                if (link == null || link.Count <= 0) return false;

                CellLinkInfo tempLink = tempLinks.FirstOrDefault(x => x.GridType == link.GridType && x.Slot == link.Slot);

                if (tempLink == null)
                {
                    tempLinks.Add(link);
                    continue;
                }

                tempLink.Count += link.Count;
            }

            links.Clear();    //链接清除
            links.AddRange(tempLinks);      //添加链接范围（临时链接）

            return true;
        }
        /// <summary>
        /// 解析链接
        /// </summary>
        /// <param name="link">单元链接信息</param>
        /// <returns></returns>
        public bool ParseLinks(CellLinkInfo link)
        {
            return link != null && link.Count > 0;
        }
        /// <summary>
        /// 删除身上的一个装备
        /// </summary>
        /// <param name="itemPosition"></param>
        public void RemoveEquippedItem(int itemPosition)
        {
            if (itemPosition >= Enum.GetNames(typeof(EquipmentSlot)).Length)
                return;

            UserItem item = Equipment[itemPosition];
            if (item == null)
                return;

            RemoveItem(item);
            Equipment[itemPosition] = null;
            item.Delete();
            RefreshStats();
            SendShapeUpdate();
            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = itemPosition, Count = 0 }, Success = true });
        }

        /// <summary>
        /// 强行给玩家穿上某个装备
        /// </summary>
        /// <param name="newItem">角色道具</param>
        /// <param name="itemPosition">道具装备位置</param>
        public void PutOnEquip(UserItem newItem, int itemPosition)
        {
            int fromIndex = -1;
            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem temp = Inventory[i];
                if (temp == null) continue;
                if (temp.Index == newItem.Index)
                {
                    fromIndex = i;
                    break;
                }
            }
            if (fromIndex == -1)
                return;

            Inventory[fromIndex] = Equipment[itemPosition];

            if (Inventory[fromIndex] != null)
                Inventory[fromIndex].Slot = fromIndex;

            Equipment[itemPosition] = newItem;
            newItem.Slot = Globals.EquipmentOffSet + itemPosition;
            Enqueue(new S.ItemMove { FromGrid = GridType.Inventory, FromSlot = fromIndex, ToGrid = GridType.Equipment, ToSlot = itemPosition, MergeItem = false, Success = true });

            /*
            S.ItemLock result = new S.ItemLock
            {
                Grid = GridType.Equipment,
                Slot = itemPosition,
                Locked = true,
            };
            Enqueue(result); */

            RefreshStats();
        }
        /// <summary>
        /// 新建一个装备并给人物装备上
        /// </summary>
        /// <param name="ItemName">道具名字</param>
        /// <param name="itemPosition">道具装备位置</param>
        public void CreateThenPutOnItem(string ItemName, int itemPosition)
        {
            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
            if (info == null) return;
            ItemCheck check = new ItemCheck(info, 1, UserItemFlags.None, TimeSpan.Zero);
            if (!CanGainItems(false, check)) return;

            UserItem newItem = SEnvir.CreateFreshItem(ItemName);
            //记录物品来源
            SEnvir.RecordTrackingInfo(newItem, CurrentMap?.Info?.Description, ObjectType.None, "脚本系统".Lang(Connection.Language), Character?.CharacterName);
            GainItem(newItem);

            PutOnEquip(newItem, itemPosition);
        }
        /// <summary>
        /// 删除道具
        /// </summary>
        /// <param name="item">道具名字</param>
        public void RemoveItem(UserItem item)
        {
            /*  foreach (BeltLink link in Character.BeltLinks)
              {
                  if (link.LinkItemIndex != item) continue;
                  link.LinkSlot = -1;
              }*/

            item.Slot = -1;                //道具位置-1
            item.Character = null;         //道具事物等0
            item.Account = null;           //道具账户等0
            item.Mail = null;              //道具邮件等0
            item.Auction = null;           //道具寄售等0
            item.Companion = null;         //道具宠物等0
            item.Guild = null;             //道具行会仓库等0

            /*item.GuildInfo = null;       //道具公会信息等0
            item.SaleInfo = null;*/        //道具销售信息等0

            //   item.Flags &= ~UserItemFlags.Locked;   //道具标记   锁定的道具标记
        }
        /// <summary>
        /// 改变武器元素
        /// </summary>
        /// <param name="IntParameter1">元素</param>
        public void ChangeElement(Element IntParameter1)
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
            result.NewStats[Stat.WeaponElement] = (int)IntParameter1 - weapon.Stats[Stat.WeaponElement];

            weapon.AddStat(Stat.WeaponElement, (int)IntParameter1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
            weapon.StatsChanged();
            RefreshStats();
            //刷新完整属性列表
            result.FullItemStats = weapon.ToClientInfo().FullItemStats;
            Enqueue(result);
        }
        /// <summary>
        /// 给道具
        /// </summary>
        /// <param name="ItemName">道具名</param>
        /// <param name="count">道具数量</param>
        /// <param name="flag">是否给极品</param>
        /// <returns></returns>
        public bool GiveItem(string ItemName, int count, bool flag = true)
        {
            switch (ItemName)
            {
                case "金币":
                    ChangeGold(count);
                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "脚本系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Undefined,
                        Source = CurrencySource.ItemAdd,
                        Amount = count,
                        ExtraInfo = $"脚本系统金币变化"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                    return true;
                case "元宝":
                    ChangeGameGold(count, "未知", CurrencySource.ItemAdd, "GiveItem()调用");
                    return true;
                case "赏金":
                    ChangeHuntGold(count);
                    return true;
                case "声望":
                    ChangePrestige(count);
                    return true;
                case "荣誉":
                    ChangeContribute(count);
                    return true;
                default:
                    ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
                    if (info == null) return false;
                    UserItemFlags flags = UserItemFlags.None;
                    if (!info.CanDrop) flags |= UserItemFlags.Bound;
                    TimeSpan dur = TimeSpan.FromSeconds(info.Duration);
                    if (dur != TimeSpan.Zero) flags |= UserItemFlags.Expirable;
                    ItemCheck check = new ItemCheck(info, count, flags, dur);
                    if (!CanGainItems(false, check)) return false;

                    while (check.Count > 0)
                    {
                        UserItem newItem = null;

                        if (flag)
                            newItem = SEnvir.CreateFreshItem(check);  //默认不给极品
                        else
                            newItem = SEnvir.CreateDropItem(check);   //加 False 给极品

                        //记录物品来源
                        SEnvir.RecordTrackingInfo(newItem, CurrentMap?.Info?.Description, ObjectType.NPC, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

                        GainItem(newItem);
                    }
                    return true;
            }
        }
        /// <summary>
        /// 给道具 名字 绑定 数量 使用时间
        /// </summary>
        /// <param name="goods">商品</param>
        /// <returns></returns>
        public bool GiveItemsByStat(IronPython.Runtime.List goods)
        {
            if (goods == null) return false;
            List<ItemCheck> checks = new List<ItemCheck>();
            for (int i = 0; i < goods.Count; i++)
            {
                IronPython.Runtime.PythonDictionary good = (PythonDictionary)goods[i];
                if (good == null) return false;
                string ItemName = "";
                bool bound = true;
                bool worthless = true;
                int count = 1;
                TimeSpan duration = TimeSpan.Zero;
                foreach (var k in good.Keys)
                {
                    object obcount;
                    if (k.ToString().ToUpper() == "NAME")       //名字
                    {
                        if (!good.TryGetValue(k, out obcount))
                            return false;
                        if (obcount == null) return false;
                        ItemName = (string)obcount;
                    }
                    if (k.ToString().ToUpper() == "BOUND")       //绑定
                    {
                        if (!good.TryGetValue(k, out obcount))
                            return false;
                        if (obcount == null) return false;
                        bound = (bool)obcount;
                    }
                    if (k.ToString().ToUpper() == "WORTHLESS")       //绑定
                    {
                        if (!good.TryGetValue(k, out obcount))
                            return false;
                        if (obcount == null) return false;
                        worthless = (bool)obcount;
                    }
                    if (k.ToString().ToUpper() == "COUNT")       //数量
                    {
                        if (!good.TryGetValue(k, out obcount))
                            return false;
                        if (obcount == null) return false;
                        count = (int)obcount;
                    }
                    if (k.ToString().ToUpper() == "EXPIRE")       //时效
                    {
                        if (!good.TryGetValue(k, out obcount))
                            return false;
                        if (obcount == null) return false;
                        duration = TimeSpan.FromSeconds((int)obcount);
                    }
                }
                UserItemFlags flags = UserItemFlags.None;
                if (bound) flags |= UserItemFlags.Bound;
                if (worthless) flags |= UserItemFlags.Worthless;
                if (duration != TimeSpan.Zero) flags |= UserItemFlags.Expirable;
                ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
                if (info == null)
                {
                    SEnvir.Log($"[道具错误] 数据库不存在 {ItemName} ");
                    return false;
                }
                ItemCheck check = new ItemCheck(info, count, flags, duration);
                checks.Add(check);
            }
            if (checks.Count == 0) return false;
            if (!CanGainItems(false, checks.ToArray()))
            {
                return false;
            }
            List<UserItem> userItems = new List<UserItem>();
            foreach (ItemCheck check in checks)
            {
                while (check.Count > 0)
                {
                    UserItem newItem = SEnvir.CreateFreshItem(check);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(newItem, CurrentMap?.Info?.Description, ObjectType.NPC, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

                    userItems.Add(newItem);
                }
            }
            GainItem(userItems.ToArray());
            return true;
        }
        /// <summary>
        /// 给道具
        /// </summary>
        /// <param name="good"></param>
        /// <returns></returns>
        public bool GiveItemByStat(IronPython.Runtime.PythonDictionary good)
        {
            //List<ItemCheck> checks = new List<ItemCheck>();
            if (good == null) return false;
            string ItemName = "";
            bool bound = true;
            int count = 1;
            TimeSpan duration = TimeSpan.Zero;
            foreach (var k in good.Keys)
            {
                object obcount;
                if (k.ToString().ToUpper() == "NAME")
                {
                    if (!good.TryGetValue(k, out obcount))
                        return false;
                    if (obcount == null) return false;
                    ItemName = (string)obcount;
                }
                if (k.ToString().ToUpper() == "BOUND")
                {
                    if (!good.TryGetValue(k, out obcount))
                        return false;
                    if (obcount == null) return false;
                    bound = (bool)obcount;
                }
                if (k.ToString().ToUpper() == "COUNT")
                {
                    if (!good.TryGetValue(k, out obcount))
                        return false;
                    if (obcount == null) return false;
                    count = (int)obcount;
                }
                if (k.ToString().ToUpper() == "EXPIRE")
                {
                    if (!good.TryGetValue(k, out obcount))
                        return false;
                    if (obcount == null) return false;
                    duration = TimeSpan.FromSeconds((int)obcount);
                }
            }
            UserItemFlags flags = UserItemFlags.None;
            if (bound) flags |= UserItemFlags.Bound;
            if (duration != TimeSpan.Zero) flags |= UserItemFlags.Expirable;
            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
            if (info == null) return false;
            ItemCheck check = new ItemCheck(info, count, flags, duration);
            if (!CanGainItems(false, check)) return false;

            while (check.Count > 0)
            {
                UserItem newItem = SEnvir.CreateFreshItem(check);
                //记录物品来源
                SEnvir.RecordTrackingInfo(newItem, CurrentMap?.Info?.Description, ObjectType.NPC, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

                GainItem(newItem);
            }
            return true;
        }
        /// <summary>
        /// 给道具碎片
        /// </summary>
        /// <param name="ItemName">道具名</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public bool GiveItemParts(string ItemName, int count)
        {
            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
            if (info.PartCount < 1) return false;
            UserItem item = SEnvir.CreateDropItem(SEnvir.ItemPartInfo);

            //记录物品来源
            SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

            item.AddStat(Stat.ItemIndex, info.Index, StatSource.Added);
            item.Count = count;
            item.StatsChanged();

            Enqueue(new S.ItemsGained { Items = new List<ClientUserItem>() { item.ToClientInfo() } });

            bool handled = false;
            foreach (UserItem oldItem in PatchGrid)
            {
                if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;

                if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                if (!oldItem.Stats.Compare(item.Stats)) continue;
                int i = Array.IndexOf(PatchGrid, oldItem);
                if (oldItem.Count + item.Count <= item.Info.StackSize)
                {
                    oldItem.Count += item.Count;
                    item.IsTemporary = true;
                    item.Delete();
                    handled = true;
                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i, Count = oldItem.Count }, Success = true });
                    break;
                }

                item.Count -= item.Info.StackSize - oldItem.Count;
                oldItem.Count = item.Info.StackSize;
                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i, Count = oldItem.Count }, Success = true });

            }
            if (!handled)
            {
                for (int i = 0; i < Character.PatchGridSize; i++)
                {
                    if (PatchGrid[i] != null) continue;
                    PatchGrid[i] = item;
                    item.Slot = i + Globals.PatchOffSet;
                    item.Character = Character;
                    item.IsTemporary = false;
                    handled = true;
                    Enqueue(new S.ItemCellRefresh { Item = item.ToClientInfo(), Grid = GridType.PatchGrid });
                    break;
                }
            }
            //碎片包满了
            if (!handled)
            {
                foreach (UserItem oldItem in Inventory)
                {
                    if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;

                    if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                    if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                    if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                    if (!oldItem.Stats.Compare(item.Stats)) continue;
                    int i = Array.IndexOf(Inventory, oldItem);
                    if (oldItem.Count + item.Count <= item.Info.StackSize)
                    {
                        oldItem.Count += item.Count;
                        item.IsTemporary = true;
                        item.Delete();
                        handled = true;
                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = oldItem.Count }, Success = true });
                        break;
                    }

                    item.Count -= item.Info.StackSize - oldItem.Count;
                    oldItem.Count = item.Info.StackSize;
                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = oldItem.Count }, Success = true });
                }

                if (!handled)
                {
                    for (int i = 0; i < Inventory.Length; i++)
                    {
                        if (Inventory[i] != null) continue;

                        Inventory[i] = item;
                        item.Slot = i;
                        item.Character = Character;
                        item.IsTemporary = false;
                        handled = true;
                        Enqueue(new S.ItemCellRefresh { Item = item.ToClientInfo(), Grid = GridType.Inventory });
                        break;
                    }
                }
            }

            RefreshWeight();
            return handled;
        }
        /// <summary>
        /// 取走碎片
        /// </summary>
        /// <param name="ItemName">道具名</param>
        /// <param name="count">数量</param>
        public void TakeItemParts(string ItemName, long count)
        {
            for (int i = 0; i < PatchGrid.Length; i++)  //碎片包裹
            {
                UserItem item = PatchGrid[i];
                if (item == null) continue;
                ItemInfo info = SEnvir.ItemInfoList.Binding.First(x => x.Index == item.Stats[Stat.ItemIndex]);
                if (info.ItemName != ItemName) continue;

                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item); //移除项目（道具）
                PatchGrid[i] = null; //碎片包裹[i] 为空
                item.Delete();  //道具 删除

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i }, Success = true });

                if (count == 0) return;
            }
        }
        /// <summary>
        /// 给道具
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        public bool GiveItems(IronPython.Runtime.PythonDictionary goods)
        {
            List<ItemCheck> checks = new List<ItemCheck>();
            if (goods == null) return false;
            foreach (var k in goods.Keys)
            {
                object obcount;
                if (!goods.TryGetValue(k, out obcount))
                    return false;
                if (obcount == null) return false;
                int count = (int)obcount;
                if (count < 1)
                    continue;
                string ItemName = k.ToString();
                ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
                if (info == null) return false;

                ItemCheck check = new ItemCheck(info, count, UserItemFlags.None, TimeSpan.Zero);
                checks.Add(check);
            }
            if (checks.Count == 0) return false;
            if (!CanGainItems(false, checks.ToArray()))
            {
                return false;
            }
            List<UserItem> userItems = new List<UserItem>();
            foreach (ItemCheck check in checks)
            {
                while (check.Count > 0)
                {
                    UserItem newItem = SEnvir.CreateFreshItem(check);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(newItem, CurrentMap?.Info?.Description, ObjectType.NPC, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

                    userItems.Add(newItem);
                }
            }
            GainItem(userItems.ToArray());
            return true;
        }
        /// <summary>
        /// 可以获得的道具
        /// </summary>
        /// <param name="checkWeight"></param>
        /// <param name="checks"></param>
        /// <returns></returns>
        public bool CanGainItems(bool checkWeight, params ItemCheck[] checks)
        {
            int index = 0;
            int patchindex = 0;

            int inventtoryemptycount = 0;
            //开始索引
            for (int i = index; i < Inventory.Length; i++)
            {
                index++;
                UserItem item = Inventory[i];
                if (item == null)
                {
                    inventtoryemptycount++;
                }
            }
            index = 0;
            foreach (ItemCheck check in checks)
            {
                if ((check.Flags & UserItemFlags.QuestItem) == UserItemFlags.QuestItem) continue;

                long count = check.Count;

                if (check.Info.Effect == ItemEffect.Gold)
                {
                    long gold = Gold;

                    gold += count;

                    continue;
                }
                if (check.Info.Effect == ItemEffect.GameGold) continue;   //如果检查信息效果是道具效果 元宝 那么继续
                if (check.Info.Effect == ItemEffect.Experience) continue;  //如果检查信息效果是道具效果 经验 那么继续
                if (check.Info.Effect == ItemEffect.Prestige) continue;   //如果检查信息效果是道具效果 声望 那么继续
                if (check.Info.Effect == ItemEffect.Contribute) continue;  //如果检查信息效果是道具效果 贡献 那么继续

                if (checkWeight)
                {
                    switch (check.Info.ItemType)
                    {
                        case ItemType.Amulet:
                        case ItemType.Poison:
                            if (BagWeight + check.Info?.Weight > Stats[Stat.BagWeight]) return false;
                            break;
                        default:
                            if (BagWeight + check.Info?.Weight * count > Stats[Stat.BagWeight]) return false;
                            break;
                    }
                }

                if (check.Info.StackSize > 1 && (check.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable)
                {
                    if (check.Info.ItemType == ItemType.ItemPart)
                    {
                        foreach (UserItem oldItemPart in PatchGrid)
                        {
                            if (oldItemPart == null) continue;
                            if (oldItemPart.Info != check.Info || oldItemPart.Count >= check.Info.StackSize) continue;

                            if ((oldItemPart.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                            if ((oldItemPart.Flags & UserItemFlags.Bound) != (check.Flags & UserItemFlags.Bound)) continue;
                            if ((oldItemPart.Flags & UserItemFlags.Worthless) != (check.Flags & UserItemFlags.Worthless)) continue;
                            if ((oldItemPart.Flags & UserItemFlags.NonRefinable) != (check.Flags & UserItemFlags.NonRefinable)) continue;
                            if (!oldItemPart.Stats.Compare(check.Stats)) continue;
                            count -= check.Info.StackSize - oldItemPart.Count;

                            if (count <= 0) break;

                        }
                    }
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
                if (check.Info.ItemType == ItemType.ItemPart)
                {
                    for (int i = patchindex; i < Character.PatchGridSize; i++)
                    {
                        patchindex++;
                        UserItem item = PatchGrid[i];
                        if (item == null)
                        {
                            count -= check.Info.StackSize;

                            if (count <= 0) break;
                        }
                    }
                }
                //开始索引
                for (int i = index; i < Inventory.Length; i++)
                {
                    index++;
                    UserItem item = Inventory[i];
                    if (item == null)
                    {
                        count -= check.Info.StackSize;
                        inventtoryemptycount--;
                        if (count <= 0) break;
                    }
                }
                if (inventtoryemptycount < 0) return false;
                if (count > 0) return false;
            }

            return true;
        }
        /// <summary>
        /// 人物捡取获得道具
        /// </summary>
        /// <param name="items"></param>
        public void GainItem(params UserItem[] items)
        {
            HashSet<UserQuest> changedQuests = new HashSet<UserQuest>();

            List<ClientUserItem> clientItems = items.Where(x => x.Info != null && x.Info.Effect != ItemEffect.Experience)
                .Select(x => x.ToClientInfo()).ToList();

            Enqueue(new S.ItemsGained { Items = clientItems });

            foreach (UserItem item in items)
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

                if (item.Info.Effect == ItemEffect.Gold)    //金币
                {
                    Gold += item.Count;
                    GoldChanged();
                    item.IsTemporary = true;

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.ItemAdd,
                        Amount = (int)item.Count,
                        ExtraInfo = $"人物获得 道具名: {item.Info.ItemName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);

                    item.Delete();
                    continue;
                }
                if (item.Info.Effect == ItemEffect.GameGold)   //元宝
                {
                    Character.Account.GameGold += (int)item.Count;
                    GameGoldChanged();
                    item.IsTemporary = true;

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.ItemAdd,
                        Amount = (int)item.Count,
                        ExtraInfo = $"人物获得 道具名: {item.Info.ItemName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);

                    item.Delete();
                    continue;
                }
                if (item.Info.Effect == ItemEffect.Prestige)  //声望
                {
                    Character.Prestige += (int)item.Count;
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }
                if (item.Info.Effect == ItemEffect.Contribute)   //贡献
                {
                    Character.Contribute += (int)item.Count;
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }
                if (item.Info.Effect == ItemEffect.Experience)   //经验
                {
                    GainExperience(item.Count, false);
                    item.IsTemporary = true;
                    item.Delete();
                    continue;
                }

                item.OwnerlessType = OwnerlessItemType.None;
                bool handled = false;
                ClientUserItem updatedItem;
                if (item.Info.StackSize > 1 && (item.Flags & UserItemFlags.Expirable) != UserItemFlags.Expirable) //可以堆叠 尝试进行堆叠
                {
                    if (item.Info.ItemType == ItemType.ItemPart) //碎片
                    {
                        foreach (UserItem oldItem in PatchGrid) //在碎片包尝试堆叠
                        {
                            if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;
                            if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                            if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                            if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                            if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                            if (!oldItem.Stats.Compare(item.Stats)) continue;

                            if (oldItem.Count + item.Count <= item.Info.StackSize) //堆上去 不超上限
                            {
                                oldItem.Count += item.Count;
                                item.IsTemporary = true;
                                item.Delete();
                                item.Slot = oldItem.Slot;
                                handled = true;

                                //更新物品的slot
                                updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                                if (updatedItem != null)
                                {
                                    updatedItem.Slot = oldItem.Slot;
                                }
                                //旧物品堆叠发生变化 需要刷新
                                Enqueue(new S.ItemCellRefresh { Item = oldItem.ToClientInfo(), Grid = GridType.PatchGrid });

                                break;
                            }

                            //只能堆一部分上去，堆满
                            item.Count -= item.Info.StackSize - oldItem.Count;
                            oldItem.Count = item.Info.StackSize;

                            //新物品的count发生变化 更新
                            updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                            if (updatedItem != null)
                            {
                                updatedItem.Count = item.Count;
                            }

                            //旧物品堆叠发生变化 需要刷新
                            Enqueue(new S.ItemCellRefresh { Item = oldItem.ToClientInfo(), Grid = GridType.PatchGrid });

                            //新物品只给了一部分 剩下的寻找下一个里可以堆的物品
                        }
                    }

                    //碎片包没找到可以堆叠的 开始找人物包
                    foreach (UserItem oldItem in Inventory)
                    {
                        if (oldItem == null || oldItem.Info != item.Info || oldItem.Count >= oldItem.Info.StackSize) continue;
                        if ((oldItem.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                        if ((oldItem.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                        if ((oldItem.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                        if ((oldItem.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;
                        if (!oldItem.Stats.Compare(item.Stats)) continue;

                        if (oldItem.Count + item.Count <= item.Info.StackSize) //堆上去 不超上限
                        {
                            oldItem.Count += item.Count;
                            item.IsTemporary = true;
                            item.Delete();
                            handled = true;

                            //更新物品的slot
                            updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                            if (updatedItem != null)
                            {
                                updatedItem.Slot = oldItem.Slot;
                            }

                            //旧物品堆叠发生变化 需要刷新
                            Enqueue(new S.ItemCellRefresh { Item = oldItem.ToClientInfo(), Grid = GridType.Inventory });

                            break;
                        }

                        //只能堆一部分上去，堆满
                        item.Count -= item.Info.StackSize - oldItem.Count;
                        oldItem.Count = item.Info.StackSize;

                        //新物品的count发生变化 更新
                        updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                        if (updatedItem != null)
                        {
                            updatedItem.Count = item.Count;
                        }

                        //旧物品堆叠发生变化 需要刷新
                        Enqueue(new S.ItemCellRefresh { Item = oldItem.ToClientInfo(), Grid = GridType.Inventory });

                        //新物品只给了一部分 剩下的寻找下一个里可以堆的物品
                    }
                }

                if (handled) continue; //当前物品交付完毕 处理下一个

                //当前物品没交付完毕 找新空位
                if (item.Info.ItemType == ItemType.ItemPart)
                {
                    handled = false;
                    for (int i = 0; i < Character.PatchGridSize; i++)
                    {
                        if (PatchGrid[i] != null) continue;
                        PatchGrid[i] = item;
                        item.Slot = i + Globals.PatchOffSet;

                        //算好的slot存一下发给客户端
                        updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                        if (updatedItem != null)
                        {
                            updatedItem.Slot = i + Globals.PatchOffSet;
                        }

                        item.Character = Character;
                        item.IsTemporary = false;
                        item.Slot = item.Slot;
                        handled = true;
                        break;
                    }
                    if (handled) continue; //交付完毕 找新空位
                }

                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] != null) continue;

                    Inventory[i] = item;
                    item.Slot = i;
                    //算好的slot存一下发给客户端
                    updatedItem = clientItems.FirstOrDefault(x => x.Index == item.Index);
                    if (updatedItem != null)
                    {
                        updatedItem.Slot = i;
                    }

                    item.Character = Character;
                    item.IsTemporary = false;
                    break;
                }
            }

            foreach (UserQuest quest in changedQuests)
                Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });


            RefreshWeight();

            //更新任务进度
            UpdateItemOnlyQuestTasks();

            #region 人物获得物品事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerGainItem(EventTypes.PlayerGainItem,
                    new PlayerGainItemEventArgs { items = items.ToList() }));
            #endregion

        }
        /// <summary>
        /// 从道具检查中获得道具
        /// </summary>
        /// <param name="ItemName"></param>
        /// <param name="value"></param>
        public void GainItemFromItemCheck(string ItemName, int value)
        {
            ItemInfo itemInfo = SEnvir.GetItemInfo(ItemName);
            if (itemInfo == null) return;
            while (value > 0)
            {
                int count = Math.Min(value, itemInfo.StackSize);

                if (!CanGainItems(false, new ItemCheck(itemInfo, count, UserItemFlags.None, TimeSpan.Zero))) break;

                UserItem userItem = SEnvir.UserItemList.CreateNewObject();
                userItem.Info = itemInfo;

                userItem.Count = count;
                userItem.Flags = UserItemFlags.None;

                value -= count;

                GainItem(userItem);
            }
        }
        /// <summary>
        /// 创建普通道具
        /// </summary>
        /// <param name="ItemName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public UserItem CreatePlainItem(string ItemName, long count = 1)
        {
            ItemInfo itemInfo = SEnvir.GetItemInfo(ItemName);
            if (itemInfo == null) return null;

            if (!CanGainItems(false, new ItemCheck(itemInfo, count, UserItemFlags.None, TimeSpan.Zero))) return null;

            UserItem userItem = SEnvir.UserItemList.CreateNewObject();
            userItem.Info = itemInfo;

            userItem.Count = count;

            userItem.Flags = UserItemFlags.None;

            return userItem;
        }
        /// <summary>
        /// 道具使用
        /// </summary>
        /// <param name="link">单元格链接</param>
        public void ItemUse(CellLinkInfo link)
        {
            if (!ParseLinks(link)) return;

            UserItem[] fromArray;
            switch (link.GridType)
            {
                case GridType.Inventory:   //背包
                    fromArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    fromArray = PatchGrid;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                case GridType.CompanionEquipment:  //宠物装备
                    if (Companion == null) return;

                    fromArray = Companion.Equipment;
                    break;
                default:
                    return;
            }

            if (link.Slot < 0 || link.Slot >= fromArray.Length) return;

            UserItem item = fromArray[link.Slot];

            if (item == null) return;

            if (SEnvir.Now < AutoPotionTime && item.Info.Effect != ItemEffect.ElixirOfPurification)
            {
                if (DelayItemUse != null)
                    Enqueue(new S.ItemChanged
                    {
                        Link = DelayItemUse
                    });

                DelayItemUse = link;
                return;
            }

            S.ItemChanged result = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = link.GridType, Slot = link.Slot }
            };
            Enqueue(result);

            if (Buffs.Any(x => x.Type == BuffType.DragonRepulse)) return;   //如果是狂涛泉涌，无法吃药

            if (!CanUseItem(item)) return;

            //str = 地图信息里的道具限制信息
            string str = CurrentMap.Info.NoUseItem;
            //获取里边的分号判断
            string[] arr = str.Split(',');
            //循环判断里边的数值
            foreach (string s in arr)
            {
                //值不为空  且 设置为负值 跳出
                if (!string.IsNullOrEmpty(s) && Convert.ToInt32(s) < 0)
                {
                    Connection.ReceiveChat("当前地图禁止使用".Lang(Connection.Language), MessageType.System);
                    return;
                }
                //值不为空 且 道具ID等里边的设置数值 跳出
                if (!string.IsNullOrEmpty(s) && item.Info.Index == Convert.ToInt32(s))
                {
                    Connection.ReceiveChat("当前地图禁止使用".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            if (Dead && item.Info.Effect != ItemEffect.PillOfReincarnation) return;  //如果死亡 或者 道具类型不是回生丸 那么跳过

            int useCount = 1;

            UserMagic magic;  //用户魔法
            BuffInfo buff;    //BUFF信息
            UserItem gainItem = null;  //用户道具 增益道具 为 零

            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, item, });
                    Nullable<bool> canuse = SEnvir.ExecutePyWithTimer(trig_play, this, "OnUseItem", args);
                    //Nullable<bool> canuse = trig_play(this, "OnUseItem", args);
                    if (canuse != null)
                    {
                        if (!canuse.Value)
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

            switch (item.Info.ItemType)
            {
                case ItemType.Consumable:  //项目类型 消耗品
                    if (SEnvir.Now < UseItemTime && item.Info.Effect != ItemEffect.ElixirOfPurification) return;  // || Horse != HorseType.None

                    bool work;  //判断 BUFF开启
                    bool hasSpace; //判断有BUFF空位
                    UserItem weapon, armour, necklace, bracelet, ring, shoes, helmet;  //判断用户道具 武器 衣服 项链 手镯 戒指
                    ItemInfo extractorInfo; //判断 道具信息 提取器信息
                    switch (item.Info.Shape)  //道具定义 消耗品类
                    {
                        case 0: //药水
                            if ((Poison & PoisonType.ElectricShock) == PoisonType.ElectricShock)
                            {
                                Connection.ReceiveChat("电击状态下，无法使用药水。".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("I电击状态下，无法使用药水。".Lang(con.Language), MessageType.System);
                                return;
                            }

                            int health = item.Info.Stats[Stat.Health];
                            int mana = item.Info.Stats[Stat.Mana];

                            if (Magics.TryGetValue(MagicType.PotionMastery, out magic) && Level >= magic.Info.NeedLevel1)
                            {
                                health += health * magic.GetPower() / 100;
                                mana += mana * magic.GetPower() / 100;

                                if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                                    LevelMagic(magic);
                            }

                            if (Magics.TryGetValue(MagicType.AdvancedPotionMastery, out magic) && Level >= magic.Info.NeedLevel1)
                            {
                                health += health * magic.GetPower() / 100;
                                mana += mana * magic.GetPower() / 100;

                                if (CurrentHP < Stats[Stat.Health] || CurrentMP < Stats[Stat.Mana])
                                    LevelMagic(magic);
                            }

                            if (SEnvir.Now < delaytime.AddSeconds(3))
                            {
                                health = health * 150 / 100;
                                mana = mana * 150 / 100;
                            }
                            else if (SEnvir.Random.Next(100) < Stats[Stat.PotionLuck])
                            {
                                delaytime = SEnvir.Now;
                                health = health * 150 / 100;
                                mana = mana * 150 / 100;
                            }

                            if (Stats[Stat.Synerg] > 0)
                            {
                                health += health * Stats[Stat.Synerg] / 100;
                                mana += mana * Stats[Stat.Synerg] / 100;
                            }

                            ChangeHP(health);
                            ChangeMP(mana);

                            if (item.Info.Stats[Stat.Experience] > 0) GainExperience(item.Info.Stats[Stat.Experience], false);
                            break;
                        case 1:  //BUFF类
                            if (!ItemBuffAdd(item.Info)) return;
                            break;
                        case 2: //回城卷
                            if (!CurrentMap.Info.AllowTT)
                            {
                                Connection.ReceiveChat("Items.CannotTownTeleport".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Items.CannotTownTeleport".Lang(con.Language), MessageType.System);
                                return;
                            }

                            if (Character.BindPoint.StartPointX == 0 || Character.BindPoint.StartPointY == 0)
                            {
                                if (!Teleport(SEnvir.Maps[Character.BindPoint.BindRegion.Map], Character.BindPoint.ValidBindPoints[SEnvir.Random.Next(Character.BindPoint.ValidBindPoints.Count)]))
                                    return;
                            }
                            else
                            {
                                if (!Teleport(SEnvir.Maps[Character.BindPoint.BindRegion.Map], Character.BindPoint.StartPointX, Character.BindPoint.StartPointY))
                                    return;
                            }
                            break;
                        case 3: //随机传送卷

                            if (!CurrentMap.Info.AllowRT)
                            {
                                Connection.ReceiveChat("Items.CannotRandomTeleport".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Items.CannotRandomTeleport".Lang(con.Language), MessageType.System);
                                return;
                            }

                            if (!Teleport(CurrentMap, CurrentMap.GetRandomLocation()))
                                return;
                            break;
                        case 4: //祝福油
                            if (!UseOilOfBenediction()) return;
                            RefreshStats();
                            break;
                        case 5: //武器强化油
                            if (!UseOilOfConservation()) return;
                            RefreshStats();
                            break;
                        case 6: //战神油
                            work = SpecialRepair(EquipmentSlot.Weapon);

                            work = SpecialRepair(EquipmentSlot.Shield) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 7: //亡灵之药水
                            if (Character.SpentPoints == 0) return;
                            if (Experience < 1000000) return;  //经验少于100W直接跳出

                            Character.SpentPoints = 0;
                            Character.HermitStats.Clear();

                            decimal loss = Math.Min(Experience, 1000000);

                            if (loss != 0)
                            {
                                Experience -= loss;
                                Enqueue(new S.GainedExperience { Amount = -loss, WeapEx = 0M, BonusEx = 0M, });
                            }

                            Broadcast(new S.ObjectUseItem { ObjectID = ObjectID });
                            RefreshStats();
                            break;
                        case 8: //回生神水
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);
                            if (buff == null) return;

                            buff.Stats[Stat.PKPoint] = Math.Max(0, buff.Stats[Stat.PKPoint] + item.Info.Stats[Stat.PKPoint]);

                            if (buff.Stats[Stat.PKPoint] == 0)
                                BuffRemove(buff);
                            else
                            {
                                Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                                RefreshStats();
                            }

                            break;
                        case 9: //藏罪证据

                            TimeSpan duration = TimeSpan.FromSeconds(item.Info.Stats[Stat.Duration]);

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.Redemption);
                            if (buff != null)
                                duration += buff.RemainingTime;

                            Stats stats = new Stats(item.Info.Stats) { [Stat.Duration] = 0 };

                            BuffAdd(BuffType.Redemption, duration, stats, false, false, TimeSpan.Zero);

                            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PvPCurse);

                            if (buff != null)
                            {
                                buff.RemainingTime = TimeSpan.FromTicks(buff.RemainingTime.Ticks / 2);
                                Enqueue(new S.BuffTime { Index = buff.Index, Time = buff.RemainingTime });
                            }

                            break;
                        case 10: //诅咒之药水
                            if (Character.SpentPoints == 0) return;

                            Character.SpentPoints = 0;
                            Character.HermitStats.Clear();

                            Broadcast(new S.ObjectUseItem { ObjectID = ObjectID });
                            RefreshStats();
                            break;
                        case 11: //超级冰泉圣水  修复类

                            work = SpecialRepair(EquipmentSlot.Weapon);
                            work = SpecialRepair(EquipmentSlot.Shield) || work;

                            work = SpecialRepair(EquipmentSlot.Helmet) || work;
                            work = SpecialRepair(EquipmentSlot.Armour) || work;
                            work = SpecialRepair(EquipmentSlot.Necklace) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletL) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletR) || work;
                            work = SpecialRepair(EquipmentSlot.RingL) || work;
                            work = SpecialRepair(EquipmentSlot.RingR) || work;
                            work = SpecialRepair(EquipmentSlot.Shoes) || work;

                            work = SpecialRepair(EquipmentSlot.Emblem) || work;
                            work = SpecialRepair(EquipmentSlot.Fashion) || work;
                            work = SpecialRepair(EquipmentSlot.HorseArmour) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 12: //首饰修复油  修复类
                            work = SpecialRepair(EquipmentSlot.Necklace);
                            work = SpecialRepair(EquipmentSlot.BraceletL) || work;
                            work = SpecialRepair(EquipmentSlot.BraceletR) || work;
                            work = SpecialRepair(EquipmentSlot.RingL) || work;
                            work = SpecialRepair(EquipmentSlot.RingR) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 13: //服饰修复油  修复类

                            work = SpecialRepair(EquipmentSlot.Helmet);
                            work = SpecialRepair(EquipmentSlot.Armour) || work;
                            work = SpecialRepair(EquipmentSlot.Shoes) || work;

                            if (!work) return;
                            RefreshStats();
                            break;
                        case 14: //紧急解毒药  
                            work = false;

                            for (int i = PoisonList.Count - 1; i >= 0; i--)
                            {
                                Poison pois = PoisonList[i];

                                switch (pois.Type)
                                {
                                    case PoisonType.Green:
                                    case PoisonType.Red:
                                    case PoisonType.Slow:
                                    case PoisonType.Paralysis:
                                    case PoisonType.StunnedStrike:
                                    case PoisonType.HellFire:
                                    case PoisonType.Silenced:
                                        work = true;
                                        PoisonList.Remove(pois);
                                        break;
                                    case PoisonType.Abyss:
                                        work = true;
                                        PoisonList.Remove(pois);
                                        break;
                                    default:
                                        continue;
                                }
                            }

                            if (!work)
                            {
                                if (SEnvir.Now.AddSeconds(3) > UseItemTime)
                                    UseItemTime = UseItemTime.AddMilliseconds(item.Info.Durability);

                                AutoPotionCheckTime = UseItemTime.AddMilliseconds(500);
                                return;
                            }

                            break;
                        case 15: //回生丸
                            if (!Dead || SEnvir.Now < Character.ReincarnationPillTime) return;

                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            Dead = false;
                            SetHP(Stats[Stat.Health]);
                            SetMP(Stats[Stat.Mana]);

                            Character.ReincarnationPillTime = SEnvir.Now.AddSeconds(item.Info.Stats[Stat.ItemReviveTime]);

                            UpdateReviveTimers(Connection);
                            Broadcast(new S.ObjectRevive { ObjectID = ObjectID, Location = CurrentLocation, Effect = true });
                            break;
                        case 16: //宠物意识药水
                            if (Companion == null) return;

                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            switch (item.Info.RequiredAmount)
                            {
                                case 3:  //3级
                                    if (Companion.UserCompanion.Level3 == null) return;

                                    if (!CompanionLevelLock3)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 3), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 3), MessageType.System);
                                        return;
                                    }

                                    Stats current = new Stats(Companion.UserCompanion.Level3);

                                    while (current.Compare(Companion.UserCompanion.Level3))
                                    {
                                        Companion.UserCompanion.Level3 = null;

                                        Companion.CheckSkills();
                                    }

                                    break;
                                case 5:  //5级
                                    if (Companion.UserCompanion.Level5 == null) return;

                                    if (!CompanionLevelLock5)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 5), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 5), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level5);

                                    while (current.Compare(Companion.UserCompanion.Level5))
                                    {
                                        Companion.UserCompanion.Level5 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 7:  //7级
                                    if (Companion.UserCompanion.Level7 == null) return;

                                    if (!CompanionLevelLock7)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 7), MessageType.System);
                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 7), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level7);

                                    while (current.Compare(Companion.UserCompanion.Level7))
                                    {
                                        Companion.UserCompanion.Level7 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 10:   //10级
                                    if (Companion.UserCompanion.Level10 == null) return;

                                    if (!CompanionLevelLock10)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 10), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 10), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level10);

                                    while (current.Compare(Companion.UserCompanion.Level10))
                                    {
                                        Companion.UserCompanion.Level10 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 11:   //11级
                                    if (Companion.UserCompanion.Level11 == null) return;

                                    if (!CompanionLevelLock11)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 11), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 11), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level11);

                                    while (current.Compare(Companion.UserCompanion.Level11))
                                    {
                                        Companion.UserCompanion.Level11 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 13:   //13级
                                    if (Companion.UserCompanion.Level13 == null) return;

                                    if (!CompanionLevelLock13)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 13), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 13), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level13);

                                    while (current.Compare(Companion.UserCompanion.Level13))
                                    {
                                        Companion.UserCompanion.Level13 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                case 15:   //15级
                                    if (Companion.UserCompanion.Level15 == null) return;

                                    if (!CompanionLevelLock15)
                                    {
                                        Connection.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(Connection.Language, item.Info.ItemName, 15), MessageType.System);

                                        foreach (SConnection con in Connection.Observers)
                                            con.ReceiveChat("Items.ConnotResetCompanionSkill".Lang(con.Language, item.Info.ItemName, 15), MessageType.System);
                                        return;
                                    }

                                    current = new Stats(Companion.UserCompanion.Level15);

                                    while (current.Compare(Companion.UserCompanion.Level15))
                                    {
                                        Companion.UserCompanion.Level15 = null;

                                        Companion.CheckSkills();
                                    }
                                    break;
                                default:
                                    return;
                            }
                            break;
                        case 17:  //仓库空间扩展

                            int size = Character.Account.StorageSize + 10;

                            if (size >= 210)//Storage.Length)
                            {
                                Connection.ReceiveChat("Items.StorageLimit".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Items.StorageLimit".Lang(con.Language), MessageType.System);
                                return;
                            }

                            Character.Account.StorageSize = size;
                            Enqueue(new S.StorageSize { Size = Character.Account.StorageSize });
                            break;
                        case 18:   //口哨
                            if (item.Info.Stats[Stat.MapSummoning] > 0 && CurrentMap.HasSafeZone)  //再安全区无法使用
                            {
                                Connection.ReceiveChat($" [{item.Info.ItemName}] 不能在有安全区的地图使用。", MessageType.System);
                                return;
                            }

                            if (item.Info.Stats[Stat.Experience] > 0) GainExperience(item.Info.Stats[Stat.Experience], false);  //经验加对应设置的值

                            IncreasePKPoints(item.Info.Stats[Stat.PKPoint]);  //增加对应的PK值

                            //一定几率给足球服
                            //if (item.Info.Stats[Stat.FootballArmourAction] > 0 && SEnvir.Random.Next(item.Info.Stats[Stat.FootballArmourAction]) == 0)
                            //{
                            //    hasSpace = false;

                            //    foreach (UserItem slot in Inventory)
                            //    {
                            //        if (slot != null) continue;

                            //        hasSpace = true;
                            //        break;
                            //    }

                            //    if (!hasSpace)
                            //    {
                            //        Connection.ReceiveChat("你的包裹空间不足", MessageType.System);
                            //        return;
                            //    }

                            //    //Give armour
                            //    ItemInfo armourInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.FootballArmour && CanStartWith(x));

                            //    if (armourInfo != null)
                            //    {
                            //        gainItem = SEnvir.CreateDropItem(armourInfo, 2);

                            //        //记录物品来源
                            //        SEnvir.RecordTrackingInfo(gainItem, CurrentMap?.Info?.Description, ObjectType.None, "口哨".Lang(Connection.Language), Character?.CharacterName);

                            //        gainItem.CurrentDurability = gainItem.MaxDurability;
                            //    }
                            //}

                            if (item.Info.Stats[Stat.MapSummoning] > 0)
                            {
                                MonsterInfo boss;
                                while (true)
                                {
                                    boss = SEnvir.BossList[SEnvir.Random.Next(SEnvir.BossList.Count)];

                                    if (boss.Level >= 150) continue;    //超过150级的不召唤

                                    if (boss.CallMapSummoning == false) continue;

                                    break;
                                }

                                MonsterObject mob = MonsterObject.GetMonster(boss);

                                if (mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 2)))  //刷怪再当前地图的随机范围内
                                {
                                    if (SEnvir.Random.Next(item.Info.Stats[Stat.MapSummoning]) == 0)   //按设置的几率里，将地图的怪物全部传送到附近
                                    {
                                        for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                                        {
                                            mob = CurrentMap.Objects[i] as MonsterObject;

                                            if (mob == null) continue;   //怪物对象为空的 忽略

                                            if (mob.PetOwner != null) continue;  //是宠物的 忽略

                                            if (mob is Guard) continue;   //属于卫士的 忽略

                                            if (mob.Dead || mob.MoveDelay == 0 || !mob.CanMove) continue;  //死亡 移动速度0  不能移动的 忽略

                                            if (mob.Target != null) continue;  //攻击目标为空的 忽略

                                            if (mob.Level >= 300) continue;  //怪物等级大于300级的 忽略

                                            if (boss.CallMapSummoning == false) continue;

                                            mob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 30));
                                        }

                                        string text = $"[{item.Info.ItemName}]已在{CurrentMap.Info.Description}使用";

                                        foreach (SConnection con in SEnvir.Connections)
                                        {
                                            switch (con.Stage)
                                            {
                                                case GameStage.Game:
                                                case GameStage.Observer:
                                                    con.ReceiveChat(text, MessageType.System);
                                                    break;
                                                default: continue;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 19:  //武器属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备武器
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)  //判断手上是否有武器
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)  //判断武器是否有附魔石
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)  //判断该武器是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{weapon.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)  //判断武器是否达到最高等级
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            if (weapon.AddedStats.Count == 0)  //判断武器是否有附加属性
                            {
                                Connection.ReceiveChat("你的武器没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.StatExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, Count = 0 }, Success = true });
                            RemoveItem(weapon);
                            Equipment[(int)EquipmentSlot.Weapon] = null;
                            weapon.Delete();
                            RefreshStats();
                            SendShapeUpdate();

                            break;
                        case 20:  //武器属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{weapon.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            weapon.Flags &= ~UserItemFlags.Refinable;

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = weapon.AddedStats[i];
                                stat.Delete();
                            }

                            weapon.StatsChanged();

                            //Give armour
                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 21:  //精炼属性提取1
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            bool hasRefine = false;

                            foreach (UserItemStat stat in weapon.AddedStats)
                            {
                                if (stat.StatSource != StatSource.Refine) continue;

                                hasRefine = true;
                                break;
                            }

                            if (!hasRefine)
                            {
                                Connection.ReceiveChat("你的武器没有任何提炼属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //Give armour
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.RefineExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                if (weapon.AddedStats[i].StatSource != StatSource.Refine) continue;

                                weapon.AddedStats[i].Item = gainItem;
                            }

                            gainItem.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });

                            RefreshStats();

                            break;
                        case 22: //精炼属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            weapon.Flags &= ~UserItemFlags.Refinable;

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = weapon.AddedStats[i];
                                if (stat.StatSource != StatSource.Refine) continue;
                                stat.Delete();
                            }

                            weapon.StatsChanged();

                            //Give armour
                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            weapon.StatsChanged();
                            weapon.ResetCoolDown = SEnvir.Now.AddDays(Config.ResetCoolDown);  //武器重置冷却时间

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 23://时间补给
                            Character.Account.AutoTime += item.Info.Durability;
                            AutoTime = SEnvir.Now.AddSeconds(Character.Account.AutoTime);
                            Enqueue(new S.AutoTimeChanged { AutoTime = Character.Account.AutoTime });
                            RefreshStats();
                            break;
                        case 24://传奇宝箱(5)  
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            TreaCount = 5;
                            TreaChangeCount = 0;
                            TreaNew = true;
                            TreaSelCount = 0;
                            S.TreasureChest packet = new S.TreasureChest { Cost = Config.Reset, Count = TreaCount, Items = new List<ClientUserItem>() };
                            TreaItems.Clear();
                            for (int i = 0; i < TreaBoxOn.Length; i++)
                            {
                                TreaBoxOn[i] = true;
                                int index = SEnvir.Random.Next(SEnvir.TreaItemList.Count);
                                UserItem itemtem = SEnvir.CreateDropItem(SEnvir.TreaItemList[index]);

                                //记录物品来源
                                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "传奇宝箱".Lang(Connection.Language), Character?.CharacterName);

                                itemtem.IsTemporary = true;
                                packet.Items.Add(itemtem.ToClientInfo());
                                TreaItems.Add(SEnvir.TreaItemList[index]);
                            }
                            Enqueue(packet);
                            RefreshStats();
                            break;
                        case 25://传奇宝箱(10)  
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            TreaCount = 10;
                            TreaChangeCount = 0;
                            TreaNew = true;
                            TreaSelCount = 0;
                            packet = new S.TreasureChest { Cost = Config.Reset, Count = TreaCount, Items = new List<ClientUserItem>() };
                            TreaItems.Clear();
                            for (int i = 0; i < TreaBoxOn.Length; i++)
                            {
                                TreaBoxOn[i] = true;
                                int index = SEnvir.Random.Next(SEnvir.TreaItemList.Count);
                                UserItem itemtem = SEnvir.CreateDropItem(SEnvir.TreaItemList[index]);

                                //记录物品来源
                                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "传奇宝箱".Lang(Connection.Language), Character?.CharacterName);

                                itemtem.IsTemporary = true;
                                packet.Items.Add(itemtem.ToClientInfo());
                                TreaItems.Add(SEnvir.TreaItemList[index]);
                            }
                            Enqueue(packet);
                            RefreshStats();
                            break;
                        case 26:  //碎片包裹空间扩展

                            int patchsize = Character.PatchGridSize + 6;

                            if (patchsize >= PatchGrid.Length)
                            {
                                Connection.ReceiveChat("Items.PatchGridLimit".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Items.PatchGridLimit".Lang(con.Language), MessageType.System);
                                return;
                            }

                            Character.PatchGridSize = patchsize;
                            Enqueue(new S.PatchGridSize { Size = Character.PatchGridSize });
                            break;
                        case 1001:  //行会联盟条约
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            if (!AcceptGuildAlliance(item)) return;
                            break;
                        case 1002:
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            if (Character.FixedPointTCount < Config.MaxFixedPointCount)
                            {
                                Character.FixedPointTCount++;
                                Enqueue(new S.sc_FixedPointAdd { count = Character.FixedPointTCount });
                            }
                            break;
                        case 1003:
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            if (Pets.Count > Config.CallPetCount)  //如果宠物数量大于4只 跳开
                            {
                                Connection.ReceiveChat("超过召唤数量，无法在召唤战斗宠物", MessageType.System);
                                return;
                            }

                            if (item.Info.Effect == ItemEffect.CallPet)
                            {
                                var mons = SEnvir.MonsterInfoList.Binding.Where(x => x.Level <= Config.CallPetLevel && x.CallFightPet).ToList();  //怪物信息列表

                                var monInfo = mons[SEnvir.Random.Next(mons.Count)];

                                if (monInfo == null) return;//如果怪物信息为空 跳开

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

                                Connection.ReceiveChat((string.Format("召唤1只{0}。", monInfo.MonsterName)), MessageType.System);
                            }
                            break;
                        case 1011:  //衣服属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备衣服
                            armour = Equipment[(int)EquipmentSlot.Armour];

                            if (armour == null)  //判断身上是否有衣服
                            {
                                Connection.ReceiveChat("你身上没有装备衣服。", MessageType.System);
                                return;
                            }

                            if (armour.Stats[Stat.UsedGemSlot] > 0)  //判断衣服是否有附魔石
                            {
                                Connection.ReceiveChat("你的衣服镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (armour.Info.Effect == ItemEffect.SpiritBlade)  //判断该衣服是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{armour.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (armour.Level != 1)  //判断衣服是否1级
                            {
                                Connection.ReceiveChat("你的衣服已经升级无法提取。", MessageType.System);
                                return;
                            }

                            if (armour.AddedStats.Count == 0)  //判断衣服是否有附加属性
                            {
                                Connection.ReceiveChat("你的衣服没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.ArmourExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = armour.AddedStats.Count - 1; i >= 0; i--)
                                armour.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            armour.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Armour, Count = 0 }, Success = true });
                            RemoveItem(armour);
                            Equipment[(int)EquipmentSlot.Armour] = null;
                            armour.Delete();
                            RefreshStats();
                            SendShapeUpdate();

                            break;
                        case 1012:  //衣服属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            armour = Equipment[(int)EquipmentSlot.Armour];

                            if (armour == null)
                            {
                                Connection.ReceiveChat("你身上没有装备衣服。", MessageType.System);
                                return;
                            }

                            if (armour.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的衣服镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (armour.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{armour.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (armour.Level != 1)
                            {
                                Connection.ReceiveChat("你的衣服不是1级，无法使用。", MessageType.System);
                                return;
                            }


                            armour.Flags &= ~UserItemFlags.Refinable;

                            for (int i = armour.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = armour.AddedStats[i];
                                stat.Delete();
                            }

                            armour.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                armour.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            armour.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Armour, GridType = GridType.Equipment, NewStats = new Stats(armour.Stats), FullItemStats = armour.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1013:  //项链属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备项链
                            necklace = Equipment[(int)EquipmentSlot.Necklace];

                            if (necklace == null)  //判断身上是否有项链
                            {
                                Connection.ReceiveChat("你身上没有装备项链。", MessageType.System);
                                return;
                            }

                            if (necklace.Stats[Stat.UsedGemSlot] > 0)  //判断项链是否有附魔石
                            {
                                Connection.ReceiveChat("你的项链镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (necklace.Info.Effect == ItemEffect.SpiritBlade)  //判断该武器是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{necklace.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (necklace.Level != 1)  //判断项链是否1级
                            {
                                Connection.ReceiveChat("你的项链已经升级无法提取。", MessageType.System);
                                return;
                            }

                            if (necklace.AddedStats.Count == 0)  //判断项链是否有附加属性
                            {
                                Connection.ReceiveChat("你的项链没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.NecklaceExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = necklace.AddedStats.Count - 1; i >= 0; i--)
                                necklace.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            necklace.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Necklace, Count = 0 }, Success = true });
                            RemoveItem(necklace);
                            Equipment[(int)EquipmentSlot.Necklace] = null;
                            necklace.Delete();
                            RefreshStats();

                            break;
                        case 1014:  //属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            necklace = Equipment[(int)EquipmentSlot.Necklace];

                            if (necklace == null)
                            {
                                Connection.ReceiveChat("你身上没有装备项链。", MessageType.System);
                                return;
                            }

                            if (necklace.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的项链镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (necklace.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{necklace.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (necklace.Level != 1)
                            {
                                Connection.ReceiveChat("你的项链不是1级，无法使用。", MessageType.System);
                                return;
                            }

                            necklace.Flags &= ~UserItemFlags.Refinable;

                            for (int i = necklace.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = necklace.AddedStats[i];
                                stat.Delete();
                            }

                            necklace.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                necklace.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            necklace.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Necklace, GridType = GridType.Equipment, NewStats = new Stats(necklace.Stats), FullItemStats = necklace.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1015:  //手镯属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备右手镯
                            bracelet = Equipment[(int)EquipmentSlot.BraceletR];

                            if (bracelet == null)  //判断身上是否有右手镯
                            {
                                Connection.ReceiveChat("你身上没有装备右手镯。", MessageType.System);
                                return;
                            }

                            if (bracelet.Stats[Stat.UsedGemSlot] > 0)  //判断手镯是否有附魔石
                            {
                                Connection.ReceiveChat("你的手镯镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (bracelet.Info.Effect == ItemEffect.SpiritBlade)  //判断该手镯是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{bracelet.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (bracelet.Level != 1)  //判断手镯是否1级
                            {
                                Connection.ReceiveChat("你的手镯已经升级无法提取。", MessageType.System);
                                return;
                            }

                            if (bracelet.AddedStats.Count == 0)  //判断手镯是否有附加属性
                            {
                                Connection.ReceiveChat("你的手镯没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.BraceletRExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = bracelet.AddedStats.Count - 1; i >= 0; i--)
                                bracelet.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            bracelet.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.BraceletR, Count = 0 }, Success = true });
                            RemoveItem(bracelet);
                            Equipment[(int)EquipmentSlot.BraceletR] = null;
                            bracelet.Delete();
                            RefreshStats();

                            break;
                        case 1016:  //手镯属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            bracelet = Equipment[(int)EquipmentSlot.BraceletR];

                            if (bracelet == null)
                            {
                                Connection.ReceiveChat("你神上没有装备右手镯。", MessageType.System);
                                return;
                            }

                            if (bracelet.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的手镯镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (bracelet.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{bracelet.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (bracelet.Level != 1)
                            {
                                Connection.ReceiveChat("你的手镯已升级无法提取。", MessageType.System);
                                return;
                            }

                            bracelet.Flags &= ~UserItemFlags.Refinable;

                            for (int i = bracelet.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = bracelet.AddedStats[i];
                                stat.Delete();
                            }

                            bracelet.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                bracelet.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            bracelet.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.BraceletR, GridType = GridType.Equipment, NewStats = new Stats(bracelet.Stats), FullItemStats = bracelet.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1017:  //戒指属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备右戒指
                            ring = Equipment[(int)EquipmentSlot.RingR];

                            if (ring == null)  //判断身上是否有右边的戒指
                            {
                                Connection.ReceiveChat("你身上没有装备右戒指。", MessageType.System);
                                return;
                            }

                            if (ring.Stats[Stat.UsedGemSlot] > 0)  //判断戒指是否有附魔石
                            {
                                Connection.ReceiveChat("你的戒指镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (ring.Info.Effect == ItemEffect.SpiritBlade)  //判断该武器是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{ring.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (ring.Level != 1)  //判断戒指是否1等级
                            {
                                Connection.ReceiveChat("你的戒指已升级无法提取。", MessageType.System);
                                return;
                            }

                            if (ring.AddedStats.Count == 0)  //判断戒指是否有附加属性
                            {
                                Connection.ReceiveChat("你的戒指没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.RingRExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = ring.AddedStats.Count - 1; i >= 0; i--)
                                ring.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            ring.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.RingR, Count = 0 }, Success = true });
                            RemoveItem(ring);
                            Equipment[(int)EquipmentSlot.RingR] = null;
                            ring.Delete();
                            RefreshStats();

                            break;
                        case 1018:  //戒指属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            ring = Equipment[(int)EquipmentSlot.RingR];

                            if (ring == null)
                            {
                                Connection.ReceiveChat("你身上没有装备右戒指。", MessageType.System);
                                return;
                            }

                            if (ring.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的戒指镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (ring.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{ring.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (ring.Level != 1)
                            {
                                Connection.ReceiveChat("你的戒指已升级无法提取。", MessageType.System);
                                return;
                            }

                            ring.Flags &= ~UserItemFlags.Refinable;

                            for (int i = ring.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = ring.AddedStats[i];
                                stat.Delete();
                            }

                            ring.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                ring.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            ring.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.RingR, GridType = GridType.Equipment, NewStats = new Stats(ring.Stats), FullItemStats = ring.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1019:  //鞋子属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备鞋子
                            shoes = Equipment[(int)EquipmentSlot.Shoes];

                            if (shoes == null)  //判断身上是否有鞋子
                            {
                                Connection.ReceiveChat("你身上没有装备鞋子。", MessageType.System);
                                return;
                            }

                            if (shoes.Stats[Stat.UsedGemSlot] > 0)  //判断鞋子是否有附魔石
                            {
                                Connection.ReceiveChat("你的鞋子镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (shoes.Info.Effect == ItemEffect.SpiritBlade)  //判断该鞋子是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{shoes.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (shoes.Level != 1)  //判断戒指是否1等级
                            {
                                Connection.ReceiveChat("你的鞋子已升级无法提取。", MessageType.System);
                                return;
                            }

                            if (shoes.AddedStats.Count == 0)  //判断戒指是否有附加属性
                            {
                                Connection.ReceiveChat("你的鞋子没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.ShoesExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = shoes.AddedStats.Count - 1; i >= 0; i--)
                                shoes.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            shoes.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Shoes, Count = 0 }, Success = true });
                            RemoveItem(shoes);
                            Equipment[(int)EquipmentSlot.Shoes] = null;
                            shoes.Delete();
                            RefreshStats();

                            break;
                        case 1020:  //鞋子属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            shoes = Equipment[(int)EquipmentSlot.Shoes];

                            if (shoes == null)
                            {
                                Connection.ReceiveChat("你身上没有装备鞋子。", MessageType.System);
                                return;
                            }

                            if (shoes.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的鞋子镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (shoes.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{shoes.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (shoes.Level != 1)
                            {
                                Connection.ReceiveChat("你的鞋子已升级无法提取。", MessageType.System);
                                return;
                            }

                            shoes.Flags &= ~UserItemFlags.Refinable;

                            for (int i = shoes.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = shoes.AddedStats[i];
                                stat.Delete();
                            }

                            shoes.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                shoes.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            shoes.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Shoes, GridType = GridType.Equipment, NewStats = new Stats(shoes.Stats), FullItemStats = shoes.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1021:  //头盔属性提取1
                            //如果道具有使用时间  或  骑马状态  跳出
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备头盔
                            helmet = Equipment[(int)EquipmentSlot.Helmet];

                            if (helmet == null)  //判断身上是否有头盔
                            {
                                Connection.ReceiveChat("你身上没有装备头盔。", MessageType.System);
                                return;
                            }

                            if (helmet.Stats[Stat.UsedGemSlot] > 0)  //判断头盔是否有附魔石
                            {
                                Connection.ReceiveChat("你的头盔镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("提取功能已锁定，请键入 @属性提取 并重试。", MessageType.System);
                                return;
                            }

                            if (helmet.Info.Effect == ItemEffect.SpiritBlade)  //判断该武器是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{helmet.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (helmet.Level != 1)  //判断戒指是否1等级
                            {
                                Connection.ReceiveChat("你的头盔已升级无法提取。", MessageType.System);
                                return;
                            }

                            if (helmet.AddedStats.Count == 0)  //判断戒指是否有附加属性
                            {
                                Connection.ReceiveChat("你的头盔没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.HelmetExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = helmet.AddedStats.Count - 1; i >= 0; i--)
                                helmet.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            helmet.StatsChanged();

                            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Helmet, Count = 0 }, Success = true });
                            RemoveItem(helmet);
                            Equipment[(int)EquipmentSlot.Helmet] = null;
                            helmet.Delete();
                            RefreshStats();
                            SendShapeUpdate();

                            break;
                        case 1022:  //头盔属性提取2
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            helmet = Equipment[(int)EquipmentSlot.Helmet];

                            if (helmet == null)
                            {
                                Connection.ReceiveChat("你身上没有装备头盔。", MessageType.System);
                                return;
                            }

                            if (helmet.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的头盔镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (helmet.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{helmet.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (helmet.Level != 1)
                            {
                                Connection.ReceiveChat("你的头盔已升级无法提取。", MessageType.System);
                                return;
                            }

                            helmet.Flags &= ~UserItemFlags.Refinable;

                            for (int i = helmet.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = helmet.AddedStats[i];
                                stat.Delete();
                            }

                            helmet.StatsChanged();

                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                helmet.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            helmet.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Helmet, GridType = GridType.Equipment, NewStats = new Stats(helmet.Stats), FullItemStats = helmet.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1023:
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;
                            //定义为身上装备武器
                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)  //判断手上是否有武器
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)  //判断武器是否有附魔石
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)  //判断是否允许属性提取
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)  //判断该武器是否属于不能提取的属性
                            {
                                Connection.ReceiveChat($"你不能提取{weapon.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)  //判断武器是否达到最高等级
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            if (weapon.AddedStats.Count == 0)  //判断武器是否有附加属性
                            {
                                Connection.ReceiveChat("你的武器没有任何附加属性。", MessageType.System);
                                return;
                            }

                            hasSpace = false;

                            foreach (UserItem slot in Inventory)  //判断包裹是否有空格
                            {
                                if (slot != null) continue;

                                hasSpace = true;
                                break;
                            }

                            if (!hasSpace)
                            {
                                Connection.ReceiveChat("你的背包没有空位。", MessageType.System);
                                return;
                            }

                            //提取道具的判断
                            extractorInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.StatExcellentExtractor);

                            if (extractorInfo == null) return;

                            gainItem = SEnvir.CreateFreshItem(extractorInfo);

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddedStats[i].Item = gainItem;

                            gainItem.StatsChanged();
                            weapon.StatsChanged();

                            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });

                            RefreshStats();

                            break;
                        case 1024:
                            if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                            weapon = Equipment[(int)EquipmentSlot.Weapon];

                            if (weapon == null)
                            {
                                Connection.ReceiveChat("你手上没有装备武器。", MessageType.System);
                                return;
                            }

                            if (weapon.Stats[Stat.UsedGemSlot] > 0)
                            {
                                Connection.ReceiveChat("你的武器镶嵌有附魔石，请先拆除。", MessageType.System);
                                return;
                            }

                            if (!ExtractorLock)
                            {
                                Connection.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("属性提取已锁定，请键入 @属性提取 并重试", MessageType.System);
                                return;
                            }

                            if (weapon.Info.Effect == ItemEffect.SpiritBlade)
                            {
                                Connection.ReceiveChat($"你不能用于{weapon.Info.ItemName}。", MessageType.System);
                                return;
                            }

                            if (weapon.Level != 17)
                            {
                                Connection.ReceiveChat("你的武器不是最高等级。", MessageType.System);
                                return;
                            }

                            weapon.Flags &= ~UserItemFlags.Refinable;

                            for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                            {
                                UserItemStat stat = weapon.AddedStats[i];
                                stat.Delete();
                            }

                            weapon.StatsChanged();

                            //Give armour
                            for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                                weapon.AddStat(item.AddedStats[i].Stat, item.AddedStats[i].Amount, item.AddedStats[i].StatSource);

                            item.StatsChanged();
                            weapon.StatsChanged();
                            weapon.ResetCoolDown = SEnvir.Now.AddDays(Config.ResetCoolDown);  //武器重置冷却时间

                            Enqueue(new S.ItemStatsRefreshed { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });
                            RefreshStats();
                            break;
                        case 1025: //净化水  
                            for (int i = PoisonList.Count - 1; i >= 0; i--)
                            {
                                Poison pois = PoisonList[i];

                                switch (pois.Type)
                                {
                                    case PoisonType.HellFire:
                                    case PoisonType.StunnedStrike:
                                    case PoisonType.ElectricShock:
                                        work = true;
                                        PoisonList.Remove(pois);
                                        break;
                                    default:
                                        continue;
                                }
                            }

                            BuffRemove(BuffType.PierceBuff);
                            BuffRemove(BuffType.BurnBuff);
                            break;

                        default:

                            break;
                    }

                    if (item.Info.Effect == ItemEffect.QuestStarter) //道具开启任务
                    {
                        QuestInfo quest = Globals.QuestInfoList.Binding.FirstOrDefault(x => x?.StartItem?.Index == item.Info.Index);
                        if (quest != null)   //任务不等空
                        {
                            QuestAccept(quest.Index);
                        }
                    }

                    if (item.Info.Effect != ItemEffect.ElixirOfPurification || UseItemTime < SEnvir.Now)
                        UseItemTime = SEnvir.Now.AddMilliseconds(item.Info.Durability - (item.Info.Durability * Stats[Stat.Marvellously] / 100));
                    else
                        UseItemTime = UseItemTime.AddMilliseconds(item.Info.Durability - (item.Info.Durability * Stats[Stat.Marvellously] / 100));

                    AutoPotionCheckTime = UseItemTime.AddMilliseconds(500);
                    break;
                case ItemType.CompanionFood:  //道具类型 宠物粮食
                    if (Companion == null) return;

                    if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                    if (Companion.UserCompanion.Hunger >= Companion.LevelInfo.MaxHunger) return;

                    Companion.UserCompanion.Hunger = Math.Min(Companion.LevelInfo.MaxHunger, Companion.UserCompanion.Hunger + item.Info.Stats[Stat.CompanionHunger]);

                    if (Buffs.All(x => x.Type != BuffType.Companion))
                        CompanionApplyBuff();

                    Companion.RefreshStats();

                    Enqueue(new S.CompanionUpdate
                    {
                        Level = Companion.UserCompanion.Level,
                        Experience = Companion.UserCompanion.Experience,
                        Hunger = Companion.UserCompanion.Hunger,
                    });
                    break;
                case ItemType.Book:  //道具类型 技能书

                    if (SEnvir.Now < UseItemTime) return;  // || Horse != HorseType.None

                    if (SEnvir.Random.Next(100) >= item.CurrentDurability)
                    {
                        Connection.ReceiveChat("Items.LearnBookFailed".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Items.LearnBookFailed".Lang(con.Language), MessageType.System);

                        break;
                    }

                    MagicInfo info = SEnvir.MagicInfoList.Binding.First(x => x.Index == item.Info.Shape);

                    if (Magics.TryGetValue(info.Magic, out magic))
                    {
                        if (magic.Level == Config.MaxMagicLv)
                        {
                            Connection.ReceiveChat("你已学习该技能", MessageType.System);
                            return;
                        }

                        int rate = (magic.Level - 2) * Config.SkillExpDrop;

                        magic.Experience++;

                        if (magic.Experience >= rate || (magic.Level == 3 && SEnvir.Random.Next(rate) == 0))
                        {
                            magic.Level++;
                            magic.Experience = 0;

                            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });

                            Connection.ReceiveChat("Items.LearnBook4Success".Lang(Connection.Language, magic.Info.Name, magic.Level), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Items.LearnBook4Success".Lang(con.Language, magic.Info.Name, magic.Level), MessageType.System);

                            RefreshStats();
                        }
                        else
                        {
                            Connection.ReceiveChat("Items.LearnBook4Failed".Lang(Connection.Language, magic.Level + 1), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Items.LearnBook4Failed".Lang(con.Language, magic.Level + 1), MessageType.System);

                            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });
                        }
                    }
                    else
                    {
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

                    break;
                case ItemType.ItemPart:  //道具类型 道具碎片
                    if (SEnvir.Now < UseItemTime || Horse != HorseType.None) return;

                    ItemInfo partInfo = SEnvir.ItemInfoList.Binding.First(x => x.Index == item.Stats[Stat.ItemIndex]);

                    if (partInfo.PartCount < 1 || partInfo.PartCount > item.Count) return;

                    if (!CanGainItems(false, new ItemCheck(partInfo, 1, UserItemFlags.None, TimeSpan.Zero))) return;

                    useCount = partInfo.PartCount;

                    gainItem = SEnvir.CreateDropItem(partInfo, 2);

                    //记录物品来源
                    SEnvir.RecordTrackingInfo(gainItem, CurrentMap?.Info?.Description, ObjectType.None, "碎片合成".Lang(Connection.Language), Character?.CharacterName);

                    break;
                default:
                    return;
            }

            result.Success = true;

            BuffRemove(BuffType.Cloak);

            if (!Config.MYWZHY)   //妙影无踪 设置 吃药
            {
                BuffRemove(BuffType.Transparency);
            }

            if (item.Count > useCount)
            {
                item.Count -= useCount;
                result.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                fromArray[link.Slot] = null;
                item.Delete();

                result.Link.Count = 0;
            }

            if (gainItem != null)
                GainItem(gainItem);

            Companion?.RefreshWeight();
            RefreshWeight();
        }
        /// <summary>
        /// 处理定点传送道具
        /// </summary>
        /// <param name="Info">客户端定点传送道具信息</param>
        /// <returns></returns>
        public bool ProcessNewFixedPoint(ClientFixedPointInfo Info)
        {
            if (null == Info) return false;
            if (!CurrentMap.Info.AllowFPM)   //地图限制无法传送  那么无法记忆坐标
            {
                Connection.ReceiveChat("Items.CannotSignThere".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Items.CannotSignThere".Lang(con.Language), MessageType.System);

                return false;
            }

            if (FPoints.Count >= FixedPointTCount)
            {
                Connection.ReceiveChat("当前可标记的坐标数量已达上限,请扩充", MessageType.System);
                return false;
            }

            //已经存在的坐标不再次记录
            foreach (var info in FPoints)
            {
                if (info.ToClientInfo().Uind == Info.Uind)
                {
                    Connection.ReceiveChat("已经存在相同的标记", MessageType.System);
                    return false;
                }
            }

            FixedPointInfo data = SEnvir.FixedPointInfoList.CreateNewObject();
            data.Character = Character;
            data.Name = Info.Name;
            data.NameColour = Info.NameColour;
            data.QuestIndex = Info.Uind.QuestIndex;
            data.Pos = Info.Uind.Pos;
            FPoints.Add(data);
            return true;
        }
        /// <summary>
        /// 记忆坐标传送
        /// </summary>
        /// <param name="p">可以传送</param>
        public void FixedPointMove(C.cs_FixedPointMove p)
        {
            if (Dead) return;

            if (null == p || null == p.Uind) return;

            if (!CurrentMap.Info.AllowFPM)   //限制地图无法使用坐标传送
            {
                Connection.ReceiveChat("Items.CannotTeleportByFP".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Items.CannotTeleportByFP".Lang(con.Language), MessageType.System);

                return;
            }
            string ItemsName = "坐标传送符";
            foreach (KeyValuePair<MapInfo, Map> pair in SEnvir.Maps)
            {
                if (pair.Key.Index == p.Uind.QuestIndex)
                {
                    if (!pair.Key.AllowFPM)  //注解:后期有部分地图漏限制，才加了这个限制
                    {
                        Connection.ReceiveChat("Items.CannotTeleportByFP".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Items.CannotTeleportByFP".Lang(con.Language), MessageType.System);

                        return;
                    }

                    foreach (var item in Inventory)
                    {
                        if (null == item || null == item.Info) continue;
                        if (item.Info.Effect == ItemEffect.DeliveryTally && item.Count >= 1)
                        {
                            item.Count--;
                            S.ItemChanged result = new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = item.Slot, Count = item.Count },
                                Success = true,
                            };
                            Enqueue(result);
                            if (item.Count == 0)
                            {
                                Inventory[item.Slot] = null;
                                RemoveItem(item);
                                item.Delete();
                            }
                            Teleport(pair.Value, p.Uind.Pos);
                            return;
                        }
                    }
                    Connection.ReceiveChat("Items.ItemsNotEnough".Lang(Connection.Language, ItemsName), MessageType.System);
                    break;
                }
            }
            return;
        }
        /// <summary>
        /// 记忆坐标记录
        /// </summary>
        /// <param name="p"></param>
        public void FixedPointSet(C.cs_FixedPoint p)
        {
            if (null == p || null == p.Info || null == p.Info.Uind) return;

            bool isSucceed = true;
            if (p.Opt == 2)
            {
                foreach (var info in FPoints)
                {
                    if (info.ToClientInfo().Uind == p.Info.Uind)
                    {
                        info.Delete();
                        FPoints.Remove(FPoints[FPoints.IndexOf(info)]);
                        break;
                    }
                }
            }
            else if (p.Opt == 1)
            {
                //修改 消耗检测(暂时没有要求消耗任何东西)
                foreach (var info in FPoints)
                {
                    if (info.ToClientInfo().Uind == p.Info.Uind)
                    {
                        FPoints[FPoints.IndexOf(info)].Name = p.Info.Name;
                        FPoints[FPoints.IndexOf(info)].NameColour = p.Info.NameColour;
                        break;
                    }
                }
            }
            else if (p.Opt == 0)
            {
                isSucceed = ProcessNewFixedPoint(p.Info);
            }
            if (isSucceed)
            {
                Enqueue(new S.sc_FixedPoint { Opt = p.Opt, Info = p.Info });
            }
        }

        public bool CanStartWith(ItemInfo info)  //开始
        {
            switch (Gender)
            {
                case MirGender.Male:  //性别 男
                    if ((info.RequiredGender & RequiredGender.Male) != RequiredGender.Male)
                        return false;
                    break;
                case MirGender.Female:  //性别 女
                    if ((info.RequiredGender & RequiredGender.Female) != RequiredGender.Female)
                        return false;
                    break;
            }

            switch (Class)
            {
                case MirClass.Warrior:  //战士
                    if ((info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                        return false;
                    break;
                case MirClass.Wizard:  //法师
                    if ((info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                        return false;
                    break;
                case MirClass.Taoist:  //道士
                    if ((info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                        return false;
                    break;
                case MirClass.Assassin:  //刺客
                    if ((info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                        return false;
                    break;
            }

            return true;
        }
        /// <summary>
        /// 可以使用道具
        /// </summary>
        /// <param name="item">道具名</param>
        /// <returns></returns>
        public bool CanUseItem(UserItem item)
        {
            switch (Gender)
            {
                case MirGender.Male:
                    if ((item.Info.RequiredGender & RequiredGender.Male) != RequiredGender.Male)
                        return false;
                    break;
                case MirGender.Female:
                    if ((item.Info.RequiredGender & RequiredGender.Female) != RequiredGender.Female)
                        return false;
                    break;
            }

            switch (Class)
            {
                case MirClass.Warrior:
                    if ((item.Info.RequiredClass & RequiredClass.Warrior) != RequiredClass.Warrior)
                        return false;
                    break;
                case MirClass.Wizard:
                    if ((item.Info.RequiredClass & RequiredClass.Wizard) != RequiredClass.Wizard)
                        return false;
                    break;
                case MirClass.Taoist:
                    if ((item.Info.RequiredClass & RequiredClass.Taoist) != RequiredClass.Taoist)
                        return false;
                    break;
                case MirClass.Assassin:
                    if ((item.Info.RequiredClass & RequiredClass.Assassin) != RequiredClass.Assassin)
                        return false;
                    break;
            }

            switch (item.Info.RequiredType) //所需项目信息类型
            {
                case RequiredType.Level:   //要求类型 等级
                    if (Level < item.Info.RequiredAmount && Stats[Stat.Rebirth] == 0) return false;
                    break;
                case RequiredType.MaxLevel://要求类型 最高等级
                    if (Level > item.Info.RequiredAmount || Stats[Stat.Rebirth] > 0) return false;
                    break;
                case RequiredType.CompanionLevel://要求类型 宠物等级
                    if (Companion == null) return false;

                    if (Companion.UserCompanion.Level < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxCompanionLevel: //要求类型 最高宠物等级
                    if (Companion == null) return false;

                    if (Companion.UserCompanion.Level > item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.AC: //要求类型 物理防御
                    if (Stats[Stat.MaxAC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MR: //要求类型 魔法防御
                    if (Stats[Stat.MaxMR] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.DC: //要求类型 破坏
                    if (Stats[Stat.MaxDC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MC: //要求类型 自然
                    if (Stats[Stat.MaxMC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.SC: //要求类型 灵魂
                    if (Stats[Stat.MaxSC] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Health: //要求类型 血值
                    if (Stats[Stat.Health] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Mana:  //要求类型 魔法值
                    if (Stats[Stat.Mana] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Accuracy:  //要求类型 准确
                    if (Stats[Stat.Accuracy] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.Agility: //要求类型 敏捷
                    if (Stats[Stat.Agility] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.RebirthLevel: //要求类型 转生等级
                    if (Stats[Stat.Rebirth] < item.Info.RequiredAmount) return false;
                    break;
                case RequiredType.MaxRebirthLevel: //要求类型 最大转生等级
                    if (Stats[Stat.Rebirth] > item.Info.RequiredAmount) return false;
                    break;
            }

            switch (item.Info.ItemType) //项目信息项目类型
            {
                case ItemType.Book:  //道具类型 书
                    MagicInfo magic = SEnvir.MagicInfoList.Binding.FirstOrDefault(x => x.Index == item.Info.Shape);
                    if (magic == null) return false;
                    if (Magics.ContainsKey(magic.Magic) && (Magics[magic.Magic].Level < 3 || (item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable)) return false;
                    return true;
                case ItemType.Consumable: //道具类型 消耗品
                    switch (item.Info.Shape)  //道具信息 Shape
                    {
                        case 1: //道具 Buffs
                            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == item.Info.Index);

                            if (buff != null && buff.RemainingTime == TimeSpan.MaxValue) return false;
                            break;
                        case 1002:  //道具 记忆传送扩展
                            if (Character.FixedPointTCount >= Config.MaxFixedPointCount)
                            {
                                Connection.ReceiveChat("坐标记忆栏可扩充数量已达上限", MessageType.System);
                                return false;
                            }
                            break;
                    }

                    if (item.Info.Effect == ItemEffect.QuestStarter) //开启任务
                    {
                        if (Character.RepeatableQuestCount >= Config.RepeatableQuestLimit)
                        {
                            Connection.ReceiveChat("本日剩余任务次数不足", MessageType.System);
                            return false;
                        }
                        QuestInfo quest = Globals.QuestInfoList.Binding.FirstOrDefault(x => x?.StartItem?.Index == item.Info.Index);
                        if (quest == null)
                        {
                            Connection.ReceiveChat($"数据库中不存在对应任务: {item.Info.Index}", MessageType.System);
                            return false;
                        }
                        if (!QuestCanAccept(quest))
                        {
                            Connection.ReceiveChat("你现在无法接受此任务", MessageType.System);
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }
        /// <summary>
        /// 道具移动位置
        /// </summary>
        /// <param name="p"></param>
        public void ItemMove(C.ItemMove p)
        {
            ItemMove(p.FromGrid, p.FromSlot, p.ToGrid, p.ToSlot, p.MergeItem);
        }
        /// <summary>
        /// 道具移动位置
        /// </summary>
        /// <param name="fromGrid">从哪类格子</param>
        /// <param name="fromSlot">从哪个格子</param>
        /// <param name="toGrid">到哪类格子</param>
        /// <param name="toSlot">到哪个格子</param>
        /// <param name="mergeItem">是否合并道具</param>
        public void ItemMove(GridType fromGrid, int fromSlot, GridType toGrid, int toSlot, bool mergeItem = false)
        {
            S.ItemMove result = new S.ItemMove
            {
                FromGrid = fromGrid,  //从格子里
                FromSlot = fromSlot,  //从道具槽里
                ToGrid = toGrid,      //到格子里
                ToSlot = toSlot,      //到道具槽里
                MergeItem = mergeItem, //合并道具

                ObserverPacket = toGrid != GridType.GuildStorage && fromGrid != GridType.GuildStorage,
            };

            Enqueue(result);  //队列

            if (Dead || (fromGrid == toGrid && fromSlot == toSlot)) return;

            UserItem[] fromArray, toArray;

            switch (fromGrid)  //从下面这些道具格子里移动
            {
                case GridType.Inventory:   //背包
                    fromArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    fromArray = PatchGrid;

                    if (fromSlot >= Character.PatchGridSize) return;
                    break;
                case GridType.Equipment:   //人物装备格子

                    fromArray = Equipment;
                    break;
                case GridType.Storage:   //仓库
                    /*if (!InSafeZone && !Character.Account.TempAdmin)   //只能在安全区使用
                    {
                        Connection.ReceiveChat("".Lang(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }*/

                    fromArray = Storage;

                    if (fromSlot >= Character.Account.StorageSize) return;
                    break;
                case GridType.GuildStorage:   //行会仓库
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    if (!InSafeZone && toGrid != GridType.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    fromArray = Character.Account.GuildMember.Guild.Storage;

                    if (fromSlot >= Character.Account.GuildMember.Guild.StorageSize) return;
                    break;
                case GridType.CompanionInventory:  //宠物背包
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    if (fromSlot >= Companion.Stats[Stat.CompanionInventory]) return;
                    break;
                case GridType.CompanionEquipment:  //宠物装备栏
                    if (Companion == null) return;

                    fromArray = Companion.Equipment;
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    fromArray = FishingEquipment;
                    break;
                default:
                    return;
            }

            if (fromSlot < 0 || fromSlot >= fromArray.Length) return;

            UserItem fromItem = fromArray[fromSlot];

            if (fromItem == null) return;
            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;  //如果是结婚戒指无法移动
            if (fromItem == Equipment[(int)EquipmentSlot.FameTitle]) return;  //如果是声望称号无法移动

            switch (toGrid)  //移动道具到下面这些地方
            {
                case GridType.Inventory:   //背包
                    toArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    toArray = PatchGrid;

                    if (toSlot >= Character.PatchGridSize) return;
                    break;
                case GridType.Equipment:   //人物装备栏
                    toArray = Equipment;
                    break;
                case GridType.Storage:    //仓库
                    /*if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("".Lang(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);
                        return;
                    }*/

                    toArray = Storage;

                    if (toSlot >= Character.Account.StorageSize) return;
                    if (!fromItem.Info.CanStore) return;   //道具是绑定的，不能存仓
                    break;
                case GridType.GuildStorage:  //行会仓库
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    if (!InSafeZone && fromGrid != GridType.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    toArray = Character.Account.GuildMember.Guild.Storage;

                    if (toSlot >= Character.Account.GuildMember.Guild.StorageSize) return;
                    break;
                case GridType.CompanionInventory:  //宠物背包
                    if (Companion == null) return;

                    toArray = Companion.Inventory;

                    if (toSlot >= Companion.Stats[Stat.CompanionInventory]) return;
                    break;
                case GridType.CompanionEquipment:  //宠物装备栏
                    if (Companion == null) return;

                    toArray = Companion.Equipment;
                    break;
                case GridType.FishingEquipment:  //钓鱼装备格子
                    toArray = FishingEquipment;
                    break;
                default:
                    return;
            }

            if (toSlot < 0 || toSlot >= toArray.Length) return;

            UserItem toItem = toArray[toSlot];

            if (toItem != null && (toItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (fromGrid == GridType.Storage && toGrid == GridType.Inventory || fromGrid == GridType.Storage && toGrid == GridType.Equipment)   // 从仓库 到包裹
            {
                if (toItem != null && !toItem.Info.CanStore) return;   //如果道具不等空 且 无法存仓库的道具 跳出
            }

            if (fromGrid == GridType.Equipment)
            {
                //可以删除道具
                if (toGrid == GridType.Equipment) return;

                if (!mergeItem && toItem != null) return;
            }

            if (fromGrid == GridType.CompanionEquipment)
            {
                //可以删除道具
                if (toGrid == GridType.CompanionEquipment) return;

                if (!mergeItem && toItem != null) return;

                if (toGrid == GridType.CompanionInventory)
                {
                    int space = fromItem.Stats[Stat.CompanionInventory] + fromItem.Info.Stats[Stat.CompanionInventory];

                    if (toSlot >= Companion.Stats[Stat.CompanionInventory] - space) return;
                }
            }

            if (fromGrid == GridType.FishingEquipment)    //钓鱼装备格子
            {
                //可以删除道具
                if (toGrid == GridType.FishingEquipment) return;

                if (!mergeItem && toItem != null) return;
            }

            if (toGrid == GridType.Equipment)
            {
                if (!CanWearItem(fromItem, (EquipmentSlot)toSlot)) return;
            }

            if (toGrid == GridType.PatchGrid)
            {
                if (fromItem.Info.ItemType != ItemType.ItemPart) return;
            }

            if (toGrid == GridType.CompanionEquipment)
            {
                if (!Companion.CanWearItem(fromItem, (CompanionSlot)toSlot)) return;

                if (fromGrid == GridType.CompanionInventory && toItem != null)
                {
                    int space = fromItem.Stats[Stat.CompanionInventory] + fromItem.Info.Stats[Stat.CompanionInventory]
                                - toItem.Stats[Stat.CompanionInventory] - toItem.Info.Stats[Stat.CompanionInventory];

                    if (toSlot >= Companion.Stats[Stat.CompanionInventory] + space) return;
                }
            }

            if (toGrid == GridType.CompanionInventory && fromGrid != GridType.CompanionInventory)
            {
                int weight = 0;

                switch (fromItem.Info.ItemType)  //从项目的信息 道具类型
                {
                    case ItemType.Poison:  //毒药
                    case ItemType.Amulet:  //护身符
                        if (mergeItem) break;
                        weight = fromItem.Weight;

                        if (toItem != null)
                            weight -= toItem.Weight;

                        break;
                    default:
                        if (mergeItem)  //合并项目
                        {
                            if (toItem != null && toItem.Count < toItem.Info.StackSize)
                                weight = (int)(Math.Min(fromItem.Count, toItem.Info.StackSize - toItem.Count) * fromItem.Info.Weight);
                        }
                        else
                        {
                            weight = fromItem.Weight;

                            if (toItem != null)
                                weight -= toItem.Weight;
                        }
                        break;
                }

                if (Companion.BagWeight + weight > Companion.Stats[Stat.CompanionBagWeight])
                {
                    Connection.ReceiveChat("Items.CompanionNoRoom".Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Items.CompanionNoRoom".Lang(con.Language), MessageType.System);
                    return;
                }
            }
            if (fromGrid == GridType.CompanionInventory && toGrid != GridType.CompanionInventory && toItem != null && !mergeItem)
            {
                int weight = toItem.Weight;

                weight -= fromItem.Weight;

                if (Companion.BagWeight + weight > Companion.Stats[Stat.CompanionBagWeight])
                {
                    Connection.ReceiveChat("Items.CompanionNoRoom".Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Items.CompanionNoRoom".Lang(con.Language), MessageType.System);
                    return;
                }
            }

            Packet guildpacket;
            //合并道具
            if (mergeItem)
            {
                if (toItem == null || toItem.Info != fromItem.Info || toItem.Count >= toItem.Info.StackSize || toItem.ExpireTime != fromItem.ExpireTime) return;

                if ((toItem.Flags & UserItemFlags.Bound) != (fromItem.Flags & UserItemFlags.Bound)) return;
                if ((toItem.Flags & UserItemFlags.Worthless) != (fromItem.Flags & UserItemFlags.Worthless)) return;
                if ((toItem.Flags & UserItemFlags.Expirable) != (fromItem.Flags & UserItemFlags.Expirable)) return;
                if ((toItem.Flags & UserItemFlags.NonRefinable) != (fromItem.Flags & UserItemFlags.NonRefinable)) return;
                if (!toItem.Stats.Compare(fromItem.Stats)) return;

                long fromCount, toCount;
                if (toItem.Count + fromItem.Count <= toItem.Info.StackSize)
                {
                    toItem.Count += fromItem.Count;

                    fromArray[fromSlot] = null;
                    fromItem.Delete();

                    toCount = toItem.Count;
                    fromCount = 0;
                }
                else
                {
                    fromItem.Count -= fromItem.Info.StackSize - toItem.Count;
                    toItem.Count = toItem.Info.StackSize;

                    toCount = toItem.Count;
                    fromCount = fromItem.Count;
                }

                result.Success = true;
                RefreshWeight();
                Companion?.RefreshWeight();

                if (toGrid == GridType.GuildStorage || fromGrid == GridType.GuildStorage)
                {
                    if (toGrid == GridType.GuildStorage)
                    {
                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = toGrid, Slot = toSlot, Count = toCount, },
                            Success = true,

                            ObserverPacket = false
                        };

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(guildpacket);

                        }
                    }
                    else
                    {
                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = toGrid, Slot = toSlot, Count = toCount },
                                Success = true,
                            });
                        }
                    }

                    if (fromGrid == GridType.GuildStorage)
                    {
                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = fromGrid, Slot = fromSlot, Count = fromCount, },
                            Success = true,

                            ObserverPacket = false
                        };

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(guildpacket);

                        }
                    }
                    else
                    {
                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = fromGrid, Slot = fromSlot, Count = fromCount },
                                Success = true,
                            });
                        }
                    }
                }
                return;
            }

            if (toGrid == GridType.GuildStorage)
            {
                if (toItem != null && fromGrid != GridType.GuildStorage) //This should force us to me merging stacks OR empty item?
                    return;

                if (!fromItem.Info.CanTrade || (fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;
            }

            if (fromGrid == GridType.GuildStorage)
            {
                if (toItem != null && toGrid != GridType.GuildStorage) //This should force us to me merging stacks OR empty item?
                    return;
            }

            fromArray[fromSlot] = toItem;  //从数组  到道具
            toArray[toSlot] = fromItem;   //到数组  道具
            bool sendShape = false, sendCompanionShape = false;

            switch (fromGrid)  //从网格
            {
                case GridType.Inventory:  //背包
                    if (toItem == null) break;

                    toItem.Slot = fromSlot;
                    toItem.Character = Character;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    if (toItem == null) break;

                    toItem.Slot = fromSlot;
                    toItem.Character = Character;
                    break;
                case GridType.Equipment:  //人物装备栏
                    sendShape = true;
                    if (toItem == null) break;
                    throw new Exception("移动道具错误"); //引发新异常 
                case GridType.CompanionInventory: //宠物背包
                    if (toItem == null) break;

                    toItem.Slot = fromSlot;
                    toItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.CompanionEquipment: //宠物装备栏
                    sendCompanionShape = true;
                    if (toItem == null) break;
                    throw new Exception("移动道具错误");
                case GridType.Storage:  //仓库
                    if (toItem == null) break;

                    toItem.Slot = fromSlot;
                    toItem.Account = Character.Account;
                    break;
                case GridType.GuildStorage:  //行会仓库
                    if (toGrid == GridType.GuildStorage)
                    {
                        //行会仓库 -> 行会仓库向其他玩家发送更新
                        guildpacket = new S.ItemMove
                        {
                            FromGrid = fromGrid,
                            FromSlot = fromSlot,
                            ToGrid = toGrid,
                            ToSlot = toSlot,
                            MergeItem = mergeItem,
                            Success = true,
                            ObserverPacket = false,
                        };
                    }
                    else
                    {
                        //发送给我的观察者 我从公会商店得到了物品和位置
                        foreach (SConnection con in Connection.Observers)
                        {
                            con.Enqueue(new S.GuildGetItem
                            {
                                Grid = toGrid,
                                Slot = toSlot,
                                Item = fromItem.ToClientInfo(), //目标为空，因此不合并
                            });
                        }

                        guildpacket = new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = fromGrid, Slot = fromSlot, },
                            Success = true,

                            ObserverPacket = false
                        };
                    }

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject player = member.Account.Connection?.Player;

                        if (player == null || player == this) continue;

                        //发送删除命令
                        player.Enqueue(guildpacket);
                    }

                    if (toItem == null) break; //只能是GS -> GS

                    toItem.Slot = fromSlot;
                    break;
            }

            switch (toGrid) //去网格
            {
                case GridType.Inventory:  //包裹
                    fromItem.Slot = toSlot;
                    fromItem.Character = Character;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    fromItem.Slot = toSlot + Globals.PatchOffSet;
                    fromItem.Character = Character;
                    break;
                case GridType.FishingEquipment:
                    fromItem.Slot = toSlot + Globals.FishingOffSet;
                    fromItem.Character = Character;
                    break;
                case GridType.Equipment:  //人物装备栏
                    sendShape = true;
                    fromItem.Slot = toSlot + Globals.EquipmentOffSet;
                    fromItem.Character = Character;
                    break;
                case GridType.CompanionInventory: //宠物背包
                    fromItem.Slot = toSlot;
                    fromItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.CompanionEquipment: //宠物装备栏
                    sendCompanionShape = true;
                    fromItem.Slot = toSlot + Globals.EquipmentOffSet;
                    fromItem.Companion = Companion.UserCompanion;
                    break;
                case GridType.Storage: //仓库
                    fromItem.Slot = toSlot;
                    fromItem.Account = Character.Account;
                    break;
                case GridType.GuildStorage:  //行会仓库
                    fromItem.Slot = toSlot;
                    fromItem.Guild = Character.Account.GuildMember.Guild;

                    if (fromGrid == GridType.GuildStorage) break; //已处理

                    //必须从玩家移动到行会存储，更新观察者包
                    foreach (SConnection con in Connection.Observers)
                    {
                        con.Enqueue(new S.ItemChanged
                        {
                            Link = new CellLinkInfo { GridType = fromGrid, Slot = fromSlot }, //始终数量 = 0;
                            Success = true,
                        });
                    }

                    guildpacket = new S.GuildNewItem
                    {
                        Slot = toSlot,
                        Item = fromItem.ToClientInfo(),
                        ObserverPacket = false
                    };

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject player = member.Account.Connection?.Player;

                        if (player == null || player == this) continue;

                        player.Enqueue(guildpacket);
                    }

                    break;
            }

            result.Success = true;

            RefreshStats(); //刷新属性

            if (sendShape) SendShapeUpdate();
            if (sendCompanionShape) Companion?.SendShapeUpdate();

            if (toGrid == GridType.CompanionEquipment || toGrid == GridType.CompanionInventory || fromGrid == GridType.CompanionEquipment || fromGrid == GridType.CompanionInventory)
            {
                Companion.SearchTime = DateTime.MinValue;
                Companion.RefreshStats();
            }

            //物品移动位置 更新任务进度
            UpdateItemOnlyQuestTasks();

            #region 物品移动事件

            //队列一个事件, 不要忘记添加listener
            //SEnvir.EventManager.QueueEvent(
            //    new PlayerItemMove(EventTypes.ItemMove,
            //        new PlayerItemMoveEventArgs
            //        {
            //            FromGridType = fromGrid,
            //            ToGridType = toGrid,
            //            Item = fromItem.Info,
            //        }));


            //python 触发

            //脱装备
            // 分为从装备栏拿掉，和被新装备挤掉2种情况
            if (fromGrid == GridType.Equipment || (toItem != null && toGrid == GridType.Equipment))
            {
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args;
                        if (toItem != null && toGrid == GridType.Equipment)
                        {
                            args = PythonOps.MakeTuple(new object[] { this, toItem, toSlot });
                        }
                        else
                        {
                            args = PythonOps.MakeTuple(new object[] { this, fromItem, fromSlot });
                        }

                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnTakeOffItem", args);
                        //trig_player(this, "OnTakeOffItem", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
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
            }

            //穿装备
            if (toGrid == GridType.Equipment)
            {
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this, fromItem, toSlot });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnPutOnItem", args);
                        //trig_player(this, "OnPutOnItem", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
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
            }


            #endregion
        }
        /// <summary>
        /// 获取道具数量
        /// </summary>
        /// <param name="itemName">道具名称</param>
        /// <returns></returns>
        public long GetItemCount(string itemName)
        {
            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == itemName);
            return info != null ? GetItemCount(info) : 0;
        }
        /// <summary>
        /// 获取道具碎片数量
        /// </summary>
        /// <param name="ItemName">道具名称</param>
        /// <returns></returns>
        public long GetItemPartsCount(string ItemName)
        {
            foreach (UserItem item in PatchGrid)
            {
                if (item == null) continue;
                ItemInfo info = SEnvir.ItemInfoList.Binding.First(x => x.Index == item.Stats[Stat.ItemIndex]);
                if (info.ItemName == ItemName)
                    return item.Count;
            }
            return 0;
        }
        /// <summary>
        /// 获取道具数量
        /// </summary>
        /// <param name="info">道具信息</param>
        /// <returns></returns>
        public long GetItemCount(ItemInfo info)
        {
            long count = 0;
            foreach (UserItem item in Inventory)
            {
                if (item == null || item.Info != info) continue;

                count += item.Count;
            }

            if (Companion != null)
            {
                foreach (UserItem item in Companion.Inventory)
                {
                    if (item == null || item.Info != info) continue;

                    count += item.Count;
                }
            }
            return count;
        }
        /// <summary>
        /// 获取背包中的道具数量
        /// </summary>
        /// <param name="info">道具信息</param>
        /// <returns></returns>
        public int GetItemCountFromInventory(ItemInfo info)
        {
            if (info == null) return 0;

            switch (info.Effect)
            {
                case ItemEffect.Gold:
                    return (int)Gold;
                case ItemEffect.GameGold:
                    return GameGold;
                default:
                    return (int)Inventory.Where(x => x != null && x.Info.Index == info.Index).Sum(x => x.Count);
            }
        }
        /// <summary>
        /// 穿戴道具
        /// </summary>
        /// <param name="ItemName">道具名</param>
        /// <returns></returns>
        public bool WearingItem(string ItemName)
        {
            foreach (UserItem item in Equipment)
            {
                if (item?.Info?.ItemName == ItemName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 穿戴道具
        /// </summary>
        /// <param name="info">道具信息</param>
        /// <returns></returns>
        public bool WearingItem(ItemInfo info)
        {
            foreach (UserItem item in Equipment)
            {
                if (item?.Info?.Index == info.Index)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取玩家身上某装备的数目
        /// </summary>
        /// <param name="ItemInfo">装备info</param>
        /// <returns></returns>
        public int GetItemCountFromEquipment(ItemInfo info)
        {
            if (info?.ItemName == null) return 0;
            return Equipment.Count(x => x?.Info != null && x.Info.ItemName == info.ItemName);
        }

        /// <summary>
        /// 获取玩家身上某装备的数目
        /// </summary>
        /// <param name="itemName">装备名字</param>
        /// <returns></returns>
        public int GetItemCountFromEquipment(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) return 0;
            return Equipment.Count(x => x?.Info != null && x.Info.ItemName == itemName);
        }
        /// <summary>
        /// 强行脱掉玩家的某个装备
        /// </summary>
        /// <param name="item">要脱掉的装备</param>
        /// <returns></returns>
        public void TakeOffEquipment(UserItem item)
        {
            if (item == null) return;

            // 先找到应该放在哪个空位
            int inventorySlot = -1;
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null)
                {
                    inventorySlot = i;
                    break;
                }
            }

            if (inventorySlot < 0)
            {
                SEnvir.Log($"无法脱下装备。玩家名：{Character.CharacterName}, 道具名：{item.Info.ItemName}, 道具编号：{item.Index}");
                return;
            }

            // 这里借用道具移动的函数
            // 这里需要转换slot
            // 因为是从装备格子转换，所以直接减去offset
            ItemMove(GridType.Equipment, item.Slot - Globals.EquipmentOffSet, GridType.Inventory, inventorySlot, false);

        }
        /// <summary>
        /// 按包裹固定位置取走道具
        /// </summary>
        /// <param name="i">位置值</param>
        /// <param name="count">数量</param>
        public void TakeItem(int i, long count)
        {
            UserItem item = Inventory[i];

            if (item == null) return;

            if (item.Count > count)
            {
                item.Count -= count;

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                return;
            }

            count -= item.Count;

            RemoveItem(item); //移除项目（道具）
            Inventory[i] = null; //背包值[i] 为空
            item.Delete();  //道具 删除
            Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });

            if (count == 0) return;

        }
        /// <summary>
        /// 按道具名取走道具
        /// </summary>
        /// <param name="item">角色道具</param>
        /// <param name="count">数量</param>
        public void TakeItem(UserItem item, long count)
        {
            if (item == null) return;
            for (int i = 0; i < Inventory.Length; i++)   //背包
            {
                UserItem itemtemp = Inventory[i];
                if (itemtemp == null || itemtemp != item) continue;
                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item); //移除项目（道具）
                Inventory[i] = null; //背包值[i] 为空
                item.Delete();  //道具 删除
                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });

                if (count == 0) return;
            }

        }
        /// <summary>
        /// 取走道具
        /// </summary>
        /// <param name="ItemName">道具名</param>
        /// <param name="count">数量</param>
        public void TakeItem(string ItemName, long count)
        {
            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
            if (info != null)
                TakeItem(info, count);
        }
        /// <summary>
        /// 取走道具
        /// </summary>
        /// <param name="info">道具信息</param>
        /// <param name="count">数量</param>
        /// todo 是否进入回购池子的选项？
        public void TakeItem(ItemInfo info, long count)
        {
            if (info.Effect == ItemEffect.Gold)
            {
                if (Gold >= count)
                {
                    ChangeGold(-count);
                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "脚本系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.ItemAdd,
                        Amount = count,
                        ExtraInfo = $"脚本系统扣除金币"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
                return;
            }

            if (info.Effect == ItemEffect.GameGold)
            {
                if (GameGold >= count)
                    ChangeGameGold((int)-count, "未知", CurrencySource.ItemDeduct, "TakeItem()调用");
                return;
            }

            for (int i = 0; i < Inventory.Length; i++)   //背包
            {
                UserItem item = Inventory[i];

                //SystemModel的比较均应比较Index 而不是Object自己
                if (item == null || item.Info.Index != info.Index) continue;

                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item); //移除项目（道具）
                Inventory[i] = null; //背包值[i] 为空
                item.Delete();  //道具 删除

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i }, Success = true });

                if (count == 0) return;
            }

            for (int i = 0; i < PatchGrid.Length; i++)  //碎片包裹
            {
                UserItem item = PatchGrid[i];

                if (item == null || item.Info != info) continue;

                if (item.Count > count)
                {
                    item.Count -= count;

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i, Count = item.Count }, Success = true });
                    return;
                }

                count -= item.Count;

                RemoveItem(item); //移除项目（道具）
                PatchGrid[i] = null; //碎片包裹[i] 为空
                item.Delete();  //道具 删除

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.PatchGrid, Slot = i }, Success = true });

                if (count == 0) return;
            }

            //有些角色没有宠物
            if (Companion?.Inventory != null)
            {
                for (int i = 0; i < Companion.Inventory.Length; i++)  //宠物包裹
                {
                    UserItem item = Companion.Inventory[i];

                    if (item == null || item.Info != info) continue;

                    if (item.Count > count)
                    {
                        item.Count -= count;

                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i, Count = item.Count }, Success = true });
                        return;
                    }

                    count -= item.Count;

                    RemoveItem(item);//移除项目（道具）
                    Companion.Inventory[i] = null;//宠物包裹[i] 为空
                    item.Delete();//道具 删除

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i }, Success = true });

                    if (count == 0) return;
                }
            }

            throw new Exception(string.Format("无法从{2}获得{0}x{1}。", info.ItemName, count, Name));
        }
        /// <summary>
        /// 道具锁定
        /// </summary>
        /// <param name="p"锁定信息></param>
        public void ItemLock(C.ItemLock p)
        {
            UserItem[] itemArray;

            switch (p.GridType)
            {
                case GridType.Inventory:  //背包
                    itemArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    itemArray = PatchGrid;
                    break;
                case GridType.Equipment:  //人物装备栏
                    itemArray = Equipment;
                    break;
                case GridType.Storage:  //仓库
                    itemArray = Storage;
                    break;
                case GridType.CompanionInventory:  //宠物背包
                    if (Companion == null) return;

                    itemArray = Companion.Inventory;
                    break;
                case GridType.CompanionEquipment:  //宠物装备栏
                    if (Companion == null) return;

                    itemArray = Companion.Equipment;
                    break;
                case GridType.FishingEquipment:   //钓鱼装备栏
                    itemArray = FishingEquipment;
                    break;
                default:
                    return;
            }

            if (p.SlotIndex < 0 || p.SlotIndex >= itemArray.Length) return;

            UserItem fromItem = itemArray[p.SlotIndex];

            if (fromItem == null) return;

            if (p.Locked)
                fromItem.Flags |= UserItemFlags.Locked;
            else
                fromItem.Flags &= ~UserItemFlags.Locked;

            S.ItemLock result = new S.ItemLock
            {
                Grid = p.GridType,
                Slot = p.SlotIndex,
                Locked = p.Locked,
            };
            Enqueue(result);
        }
        /// <summary>
        /// 道具分割拆解拆分
        /// </summary>
        /// <param name="p">道具拆分</param>
        public void ItemSplit(C.ItemSplit p)
        {
            S.ItemSplit result = new S.ItemSplit
            {
                Grid = p.Grid,
                Slot = p.Slot,
                Count = p.Count,
                ObserverPacket = p.Grid != GridType.GuildStorage,
            };

            if (Dead || p.Count <= 0) return;

            UserItem[] array;
            int offset = 0;
            switch (p.Grid)
            {
                case GridType.Inventory:  //背包
                    array = Inventory;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    array = PatchGrid;
                    offset = Globals.PatchOffSet;
                    break;
                case GridType.Storage:  //仓库
                    array = Storage;
                    break;
                case GridType.GuildStorage:  //行会仓库
                    if (Character.Account.GuildMember == null) return;

                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return;

                    array = Character.Account.GuildMember.Guild.Storage;
                    break;
                case GridType.CompanionInventory: //宠物背包
                    if (Companion == null) return;

                    array = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Slot < 0 || p.Slot >= array.Length) return;

            UserItem item = array[p.Slot];

            if (item == null || item.Count <= p.Count || item.Info.StackSize < p.Count) return;

            int length = array.Length;
            if (p.Grid == GridType.CompanionInventory)  //宠物背包
                length = Math.Min(array.Length, Companion.Stats[Stat.CompanionInventory]);

            if (p.Grid == GridType.Storage)   //仓库
                length = Math.Min(array.Length, Character.Account.StorageSize);

            if (p.Grid == GridType.PatchGrid) //碎片包裹
                length = Math.Min(array.Length, Character.PatchGridSize);

            for (int i = 0; i < length; i++)
            {
                if (array[i] != null) continue;

                if (p.Grid == GridType.GuildStorage && i >= Character.Account.GuildMember.Guild.StorageSize) break;

                result.Success = true;
                result.NewSlot = i;

                item.Count -= p.Count;

                UserItem newItem = SEnvir.CreateFreshItem(item);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, newItem);

                newItem.Count = p.Count;

                array[i] = newItem;
                newItem.Slot = i + offset;

                switch (p.Grid)
                {
                    case GridType.Inventory: //背包
                        newItem.Character = Character;
                        break;
                    case GridType.PatchGrid:  //碎片包裹
                        newItem.Character = Character;
                        break;
                    case GridType.Storage: //仓库
                        newItem.Account = Character.Account;
                        break;
                    case GridType.CompanionInventory: //宠物背包
                        newItem.Companion = Companion.UserCompanion;
                        break;
                    case GridType.GuildStorage: //行会仓库
                        newItem.Guild = Character.Account.GuildMember.Guild;

                        foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        {
                            PlayerObject player = member.Account.Connection?.Player;

                            if (player == null || player == this) continue;

                            player.Enqueue(new S.ItemChanged
                            {
                                Link = new CellLinkInfo { GridType = p.Grid, Slot = p.Slot, Count = item.Count },
                                Success = true,

                                ObserverPacket = false
                            });

                            player.Enqueue(new S.GuildNewItem
                            {
                                Slot = newItem.Slot,
                                Item = newItem.ToClientInfo(),

                                ObserverPacket = false
                            });
                        }
                        break;
                }
                Enqueue(result);
                return;
            }
        }
        /// <summary>
        /// 刷新仓库
        /// </summary>
        /// <param name="p"></param>
        public void StorageItemRefresh(C.StorageItemRefresh p)
        {
            var isPart = false;
            UserItem[] fromArray = isPart ? PatchGrid : Storage;

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
            S.StorageItemRefresh result = new S.StorageItemRefresh
            {
                Items = newList
            };

            Enqueue(result);
        }
        /// <summary>
        /// 刷新宠物包裹
        /// </summary>
        /// <param name="p"></param>
        public void CompanionGridItemRefresh(C.CompanionGridRefresh p)
        {
            var isPart = false;
            UserItem[] fromArray = isPart ? PatchGrid : Companion.Inventory;

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
            S.CompanionGridRefresh result = new S.CompanionGridRefresh
            {
                Items = newList
            };

            Enqueue(result);
        }
        /// <summary>
        /// 金币改变
        /// </summary>
        public void GoldChanged()
        {
            Enqueue(new S.GoldChanged { Gold = Gold });
        }
        /// <summary>
        /// 变更玩家金币
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        public void ChangeGold(long amount)
        {
            Gold += amount;
            GoldChanged();
        }
        /// <summary>
        /// 元宝变化
        /// </summary>
        public void GameGoldChanged()
        {
            Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold });
        }
        /// <summary>
        /// 记录输出函数啊
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        /// <param name="type">日志</param>
        /// <param name="component">功能输出</param>
        /// <param name="source">来源</param>
        /// <param name="extraInfo">额外信息</param>
        public void ToLog(int amount, CurrencyType type = CurrencyType.GameGold, string component = "未知",
            CurrencySource source = CurrencySource.Undefined,
            string extraInfo = "调用ChangeGameGold()的函数未提供详细信息")
        {
            // 此处构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry(
                logLevel: LogLevel.Info,
                component: component,
                time: SEnvir.Now,
                character: this.Character,
                currency: type,
                action: amount < 0 ? CurrencyAction.Deduct : CurrencyAction.Add,
                source: source,
                amount: amount,
                extraInfo: extraInfo);
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
        }
        /// <summary>
        /// 变更玩家元宝
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        /// <param name="logEntry">日志</param>
        /// <param name="component">哪个功能调用的</param>
        /// <param name="source">来源</param>
        /// <param name="extraInfo">额外信息</param>
        public void ChangeGameGold(int amount, string component = "未知",
            CurrencySource source = CurrencySource.Undefined,
            string extraInfo = "调用ChangeGameGold()的函数未提供详细信息")
        {
            // 此处构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry(
                logLevel: LogLevel.Info,
                component: component,
                time: SEnvir.Now,
                character: this.Character,
                currency: CurrencyType.GameGold,
                action: amount < 0 ? CurrencyAction.Deduct : CurrencyAction.Add,
                source: source,
                amount: amount,
                extraInfo: extraInfo);
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            Character.Account.GameGold += amount;
            GameGoldChanged();
        }
        /// <summary>
        /// 声望变化
        /// </summary>
        public void PrestigeChanged()
        {
            Enqueue(new S.PrestigeChanged { Prestige = Character.Prestige });
        }
        /// <summary>
        /// 变更玩家声望
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        public void ChangePrestige(int amount)
        {
            Prestige += amount;
            PrestigeChanged();
        }
        /// <summary>
        /// 贡献变化
        /// </summary>
        public void ContributeChanged()
        {
            Enqueue(new S.ContributeChanged { Contribute = Character.Contribute });
        }
        /// <summary>
        /// 变更玩家贡献
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        public void ChangeContribute(int amount)
        {
            Contribute += amount;
            ContributeChanged();
        }
        /// <summary>
        /// 赏金变化
        /// </summary>
        public void HuntGoldChanged()
        {
            Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });
        }
        /// <summary>
        /// 变更玩家赏金
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        public void ChangeHuntGold(int amount)
        {
            Character.Account.HuntGold += amount;
            HuntGoldChanged();
        }
        /// <summary>
        /// 经验值变化
        /// </summary>
        /// <param name="amount"></param>
        public void ExperienceChanged(decimal amount)
        {
            if (Level >= Config.MaxLevel && Config.MaxLevelLimit)
            {
                Enqueue(new S.GainedExperience { Amount = 0, WeapEx = 0M, BonusEx = 0M, });
            }
            else
            {
                Enqueue(new S.GainedExperience { Amount = amount, WeapEx = 0M, BonusEx = 0M, });
            }
        }
        /// <summary>
        /// 变更玩家经验值
        /// </summary>
        /// <param name="amount">变化值(加或者减)</param>
        public void ChangeExperience(decimal amount)
        {
            Experience += amount;
            ExperienceChanged(amount);
        }
        /// <summary>
        /// 道具掉落
        /// </summary>
        /// <param name="p">道具掉落</param>
        public void ItemDrop(C.ItemDrop p)
        {
            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link
            };
            Enqueue(result);

            if (Dead && !Config.DeadLoseItem || !ParseLinks(p.Link))  // 死亡     分析链接
                return;

            UserItem[] fromArray;

            switch (p.Link.GridType)
            {
                case GridType.Inventory:  //背包
                    fromArray = Inventory;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    fromArray = PatchGrid;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Link.Slot < 0 || p.Link.Slot >= fromArray.Length) return;

            UserItem fromItem = fromArray[p.Link.Slot];

            if (fromItem == null || p.Link.Count > fromItem.Count || !fromItem.Info.CanDrop || (fromItem.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;

            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (InSafeZone)
            {
                Connection.ReceiveChat("MapObject.InSafeZoneDiscardItem".Lang(Connection.Language), MessageType.Hint);
                return;
            }

            Cell cell = GetDropLocation(1, null);  //获取物品放置的位置

            if (cell == null) return;

            result.Success = true;

            UserItem dropItem;

            if (p.Link.Count == fromItem.Count)
            {
                dropItem = fromItem;
                RemoveItem(fromItem);
                fromArray[p.Link.Slot] = null;

                result.Link.Count = 0;
            }
            else
            {
                UserItem newItem = SEnvir.CreateFreshItem(fromItem);
                //记录物品来源
                SEnvir.RecordTrackingInfo(fromItem, newItem);
                dropItem = newItem;

                dropItem.Count = p.Link.Count;
                fromItem.Count -= p.Link.Count;

                result.Link.Count = fromItem.Count;
            }

            RefreshWeight();
            Companion?.RefreshWeight();
            dropItem.IsTemporary = true;

            ItemObject ob = new ItemObject
            {
                Item = dropItem,
            };

            if ((fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)
                ob.Account = Character.Account;

            ob.Spawn(CurrentMap.Info, cell.Location);

            //丢弃物品 更新任务进度
            UpdateItemOnlyQuestTasks();

            #region 玩家主动丢弃物品

            //python 触发
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, CurrentMap.Info, dropItem });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnDropItem", args);
                    //trig_player(ob, "OnDropItem", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
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

            #endregion
        }
        /// <summary>
        /// 道具删除
        /// </summary>
        /// <param name="p">道具删除</param>
        public void ItemDelete(C.ItemDelete p)
        {
            if (Dead || !ParseLinks(p.Link))
                return;
            if (p.Link.GridType != GridType.Inventory)
            {
                return;
            }
            var fromArray = Inventory;
            if (p.Link.Slot < 0 || p.Link.Slot >= fromArray.Length) return;

            var fromItem = fromArray[p.Link.Slot];

            //锁定物品，非绑定或不可扔出物品，不可删除
            if (fromItem == null || fromItem.Flags.HasFlag(UserItemFlags.Locked)  //|| p.Link.Count > 1  可叠加物品
                || !(!(fromItem.Info.CanDrop && fromItem.Info.CanDeathDrop) || fromItem.Flags.HasFlag(UserItemFlags.Worthless)))
            {
                Connection.ReceiveChat($"物品 {fromItem.Info.ItemName} 不可删除。", MessageType.System);
                return;
            }

            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            p.Link.Count = 0;
            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link,
                Success = true
            };

            Enqueue(result);

            RemoveItem(fromItem);
            fromArray[p.Link.Slot] = null;
            RefreshWeight();
            Companion?.RefreshWeight();
        }
        /// <summary>
        /// 金币掉落
        /// </summary>
        /// <param name="p">金币掉落</param>
        public void GoldDrop(C.GoldDrop p)
        {
            if (Dead || p.Amount <= 0 || p.Amount > Gold) return;

            Cell cell = GetDropLocation(Config.DropDistance, null);

            if (cell == null) return;

            Gold -= p.Amount;
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "道具系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemDeduct,
                Amount = p.Amount,
                ExtraInfo = $"人物掉落金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            UserItem dropItem = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
            dropItem.Count = p.Amount;
            dropItem.IsTemporary = true;

            ItemObject ob = new ItemObject
            {
                Item = dropItem,
            };

            ob.Spawn(CurrentMap.Info, cell.Location);
        }
        /// <summary>
        /// 物品快捷栏变动
        /// </summary>
        /// <param name="p">物品快捷栏变化</param>
        public void BeltLinkChanged(C.BeltLinkChanged p)
        {
            if (p.Slot < 0 || p.Slot >= Globals.MaxBeltCount) return;
            if (p.LinkIndex > 0 && p.LinkItemIndex > 0) return;
            if (p.Slot >= Inventory.Length) return;

            ItemInfo info = null;
            UserItem item = null;

            if (p.LinkIndex > 0)
            {
                info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == p.LinkIndex);
                if (info != null && info.ItemType == ItemType.ItemPart) return;
            }
            else if (p.Slot > 0)
            {
                item = Inventory.FirstOrDefault(x => x?.Index == p.LinkItemIndex);
                if (item != null && item.Info.ItemType == ItemType.ItemPart) return;
            }

            foreach (CharacterBeltLink link in Character.BeltLinks)
            {
                if (link.Slot != p.Slot && (link.LinkInfoIndex != -1 || link.LinkItemIndex != -1)) continue;

                link.Slot = p.Slot;
                link.LinkInfoIndex = info?.Index ?? -1;
                link.LinkItemIndex = item?.Index ?? -1;
                return;
            }

            if (info == null && item == null) return;

            CharacterBeltLink bLink = SEnvir.BeltLinkList.CreateNewObject();

            bLink.Character = Character;
            bLink.Slot = p.Slot;
            bLink.LinkInfoIndex = p.LinkIndex;
            bLink.LinkItemIndex = p.LinkItemIndex;
        }
        /// <summary>
        /// 自动喝药栏变化
        /// </summary>
        /// <param name="p">自动喝药栏变化</param>
        public void AutoPotionLinkChanged(C.AutoPotionLinkChanged p)
        {
            if (p.Slot < 0 || p.Slot >= Globals.MaxAutoPotionCount) return;
            if (p.Slot >= Inventory.Length) return;

            ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == p.LinkIndex);

            foreach (AutoPotionLink link in Character.AutoPotionLinks)
            {
                if (link.Slot != p.Slot) continue;

                link.Slot = p.Slot;
                link.LinkInfoIndex = info?.Index ?? -1;
                link.Health = p.Health;
                link.Mana = p.Mana;
                link.Enabled = p.Enabled;
                //新喝药
                AutoPotions.Sort((x1, x2) => x1.Health.CompareTo(x2.Health));
                AutoPotions.Sort((x1, x2) => x1.Mana.CompareTo(x2.Mana));
                return;
            }

            AutoPotionLink aLink = SEnvir.AutoPotionLinkList.CreateNewObject();

            aLink.Character = Character;
            aLink.Slot = p.Slot;
            aLink.LinkInfoIndex = info?.Index ?? -1;
            aLink.Health = p.Health;
            aLink.Mana = p.Mana;
            aLink.Enabled = p.Enabled;

            AutoPotions.Add(aLink);

            //旧喝药
            //AutoPotions.Sort((x1, x2) => x2.Slot.CompareTo(x1.Slot));
            //新喝药
            AutoPotions.Sort((x1, x2) => x1.Health.CompareTo(x2.Health));
            AutoPotions.Sort((x1, x2) => x2.Mana.CompareTo(x1.Mana));
        }
        /// <summary>
        /// 宠物道具捡取排序
        /// </summary>
        /// <param name="p">按类型</param>
        public void ComSortingConfChanged(C.ComSortingConfChanged p)
        {
            if (Companion == null || Companion.UserCompanion == null || !Companion.bSort) return;
            foreach (UserSorting sort in Companion.UserCompanion.Sortings)
            {
                if (sort.SetType != p.Slot) continue;
                sort.SetType = p.Slot;
                sort.Enabled = p.Enabled;
                Companion.Sortings[(int)p.Slot] = p.Enabled;
                return;
            }
            UserSorting alink = SEnvir.SortingList.CreateNewObject();
            alink.Companion = Companion.UserCompanion;
            alink.SetType = p.Slot;
            alink.Enabled = p.Enabled;
            Companion.Sortings[(int)p.Slot] = p.Enabled;
        }
        /// <summary>
        /// 宠物道具捡取排序
        /// </summary>
        /// <param name="p">按品质</param>
        public void ComSortingConf1Changed(C.ComSortingConf1Changed p)
        {
            if (Companion == null || Companion.UserCompanion == null || !Companion.bSort) return;
            foreach (UserSortingLev sort in Companion.UserCompanion.SortingsLev)
            {
                if (sort.SetType != p.Slot) continue;
                sort.SetType = p.Slot;
                sort.Enabled = p.Enabled;
                Companion.SortingLevs[(int)p.Slot] = p.Enabled;
                return;
            }
            UserSortingLev alink = SEnvir.SortingLevList.CreateNewObject();
            alink.Companion = Companion.UserCompanion;
            alink.SetType = p.Slot;
            alink.Enabled = p.Enabled;
            Companion.SortingLevs[(int)p.Slot] = p.Enabled;
        }
        /// <summary>
        /// 自动战斗状态改变
        /// </summary>
        /// <param name="p">挂机变化</param>
        public void AutoFightConfChanged(C.AutoFightConfChanged p)
        {
            if (p.Slot < AutoSetConf.SetRunCheckBox || p.Slot >= AutoSetConf.SetMaxConf) return;
            if (p.Slot == AutoSetConf.SetMagicskillsBox || p.Slot == AutoSetConf.SetMagicskills1Box || p.Slot == AutoSetConf.SetSingleHookSkillsBox)
            {
                UserMagic magic;
                if (!Magics.TryGetValue(p.MagicIndex, out magic))
                {
                    return;
                }
            }
            if (p.Slot == AutoSetConf.SetAutoOnHookBox)
            {
                if (p.Enabled) AutoTime = SEnvir.Now.AddSeconds(Character.Account.AutoTime);
                setConfArr[(int)p.Slot] = p.Enabled;
                return;
            }
            foreach (AutoFightConfig link in Character.AutoFightLinks)
            {
                if (link.Slot != p.Slot) continue;

                link.Slot = p.Slot;
                link.MagicIndex = p.MagicIndex;
                link.TimeCount = p.TimeCount;
                link.Enabled = p.Enabled;
                setConfArr[(int)p.Slot] = p.Enabled;
                return;
            }
            AutoFightConfig aLink = SEnvir.AutoFightConfList.CreateNewObject();
            aLink.Character = Character;
            aLink.Slot = p.Slot;
            aLink.MagicIndex = p.MagicIndex;
            aLink.Enabled = p.Enabled;
            aLink.TimeCount = p.TimeCount;
            AutoFights.Add(aLink);
            setConfArr[(int)p.Slot] = p.Enabled;
        }
        /// <summary>
        /// 捡取物品
        /// </summary>
        public void PickUp()
        {
            if (Dead) return;   //死亡跳出

            int range = Stats[Stat.PickUpRadius];    //取整  范围=状态[状态.捡取范围]

            for (int d = 0; d <= range; d++)  //遍历 （取整 d=0; d小于或者等于 范围; d++）
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y]; //直接进入我们已经检查过了

                        if (cell?.Objects == null) continue;

                        foreach (MapObject cellObject in cell.Objects)
                        {
                            if (cellObject.Race != ObjectType.Item) continue;

                            ItemObject item = (ItemObject)cellObject;

                            if (item.PickUpItem(this)) return;
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 捡取道具
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="itemIdx">道具ID</param>
        public void PickUp(int x, int y, int itemIdx)
        {
            if (Dead) return;     //死亡跳出

            // 不设条件的拾取附近所有物品
            if (x == 0 && y == 0 && itemIdx == 0)
            {
                PickUp();
                return;
            }

            if (x < 0)
                return;
            if (x >= CurrentMap.Width)
                return;

            int distance = Functions.Distance(new Point(x, y), CurrentLocation);

            // 不在拾取范围
            if (distance > Stats[Stat.PickUpRadius])
                return;

            Cell cell = CurrentMap.Cells[x, y]; //直接进入我们已经检查过了

            if (cell?.Objects == null)
                return;

            foreach (MapObject cellObject in cell.Objects)
            {
                if (cellObject.Race != ObjectType.Item)
                    continue;

                ItemObject item = (ItemObject)cellObject;

                if (itemIdx != -1)
                {
                    if (item?.Item?.Info?.Index == itemIdx)
                    {
                        item.PickUpItem(this);
                        return;
                    }
                }
                else
                {
                    item.PickUpItem(this);
                    return;
                }
            }
        }
        /// <summary>
        /// 可穿戴道具
        /// </summary>
        /// <param name="item">道具</param>
        /// <param name="slot">格子</param>
        /// <returns></returns>
        public bool CanWearItem(UserItem item, EquipmentSlot slot)
        {
            if (!Functions.CorrectSlot(item.Info.ItemType, slot) || !CanUseItem(item))
                return false;

            switch (item.Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Torch:
                case ItemType.Shield:
                    if (HandWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > Stats[Stat.HandWeight]) return false;
                    break;
                default:
                    if (WearWeight - (Equipment[(int)slot]?.Info.Weight ?? 0) + item.Weight > Stats[Stat.WearWeight]) return false;
                    break;

            }
            return true;
        }
        public bool ReflushDurability(GridType grid, int slot, int rate = 1)
        {
            UserItem item;  //角色道具
            switch (grid)   //格子类型
            {
                case GridType.Inventory:
                    item = Inventory[slot];
                    break;
                case GridType.Equipment:
                    item = Equipment[slot];
                    break;
                case GridType.FishingEquipment:
                    item = FishingEquipment[slot];
                    break;
                default:
                    return false;
            }
            //如果 道具为空  道具持久为0 道具当前持久为0 跳出
            if (item == null || item.Info.Durability == 0) return false;
            //如果 道具是结婚戒指 跳出
            //if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
            //道具持久消耗 每次攻击的耗损
            item.CurrentDurability = Math.Max(0, item.CurrentDurability - rate);
            Enqueue(new S.ItemDurability         //发包更新道具持久
            {
                GridType = grid,
                Slot = slot,
                CurrentDurability = item.CurrentDurability,
            });

            if (item.CurrentDurability == 0)   //如果道具的持久为0
            {
                SendShapeUpdate();    //发送图像更新
                RefreshStats();       //发送属性更新
                return true;
            }
            return false;

        }
        /// <summary>
        /// 道具磨损
        /// </summary>
        /// <param name="grid">格子类型</param>
        /// <param name="slot"></param>
        /// <param name="rate"></param>
        /// <param name="delayStats"></param>
        /// <returns></returns>
        public bool DamageItem(GridType grid, int slot, int rate = 1, bool delayStats = false)
        {
            UserItem item;  //角色道具
            switch (grid)   //格子类型
            {
                case GridType.Inventory:
                    item = Inventory[slot];
                    break;
                case GridType.Equipment:
                    item = Equipment[slot];
                    break;
                case GridType.FishingEquipment:
                    item = FishingEquipment[slot];
                    break;
                default:
                    return false;
            }
            //如果 道具为空  道具持久为0 道具当前持久为0 跳出
            if (item == null || item.Info.Durability == 0 || item.CurrentDurability == 0) return false;
            //如果 道具是结婚戒指 跳出
            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;

            switch (item.Info.ItemType)  //判断道具类型
            {
                case ItemType.Nothing:     //其他
                case ItemType.Consumable:  //消耗品
                case ItemType.Poison:      //毒药
                case ItemType.Amulet:      //护身符
                case ItemType.Scroll:      //卷轴
                case ItemType.Orb:         //宝珠
                case ItemType.Gem:         //附魔石
                case ItemType.Rune:        //符文
                    return false;
                case ItemType.Weapon:  //武器
                    //如果武器的强度随机值>0 不减持久  强度越高消耗持久越少
                    if (SEnvir.Random.Next(Math.Max(0, Stats[Stat.Strength])) > 0) return false;
                    break;
                default:
                    //其他类别 随机值3数等0 且 道具强度随机值>0 不减持久
                    if (SEnvir.Random.Next(Config.DurabilityRate) == 0 && SEnvir.Random.Next(Math.Max(0, Stats[Stat.Strength])) > 0) return false;
                    break;
            }

            //道具持久消耗 每次攻击的耗损
            item.CurrentDurability = Math.Max(0, item.CurrentDurability - rate);

            Enqueue(new S.ItemDurability         //发包更新道具持久
            {
                GridType = grid,
                Slot = slot,
                CurrentDurability = item.CurrentDurability,
            });

            if (item.CurrentDurability == 0)   //如果道具的持久为0
            {
                SendShapeUpdate();    //发送图像更新
                RefreshStats();       //发送属性更新
                return true;
            }
            return false;
        }
        /// <summary>
        /// 消耗自动药剂
        /// </summary>
        /// <param name="rate"></param>
        public void DamageMedicament(int rate = 1)
        {
            DamageItem(GridType.Equipment, (int)EquipmentSlot.Medicament, rate);

            UserItem medicament = Equipment[(int)EquipmentSlot.Medicament];

            if (medicament == null || medicament.CurrentDurability != 0 || medicament.Info.Durability <= 0) return;

            RemoveItem(medicament);
            Equipment[(int)EquipmentSlot.Medicament] = null;
            medicament.Delete();

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Medicament },
                Success = true,
            });
        }
        /// <summary>
        /// 消耗属性石
        /// </summary>
        /// <param name="rate"></param>
        public void DamageDarkStone(int rate = 1)
        {
            DamageItem(GridType.Equipment, (int)EquipmentSlot.Amulet, rate);

            UserItem stone = Equipment[(int)EquipmentSlot.Amulet];

            if (stone == null || stone.CurrentDurability != 0 || stone.Info.Durability <= 0) return;

            RemoveItem(stone);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            stone.Delete();

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet },
                Success = true,
            });
        }
        /// <summary>
        /// 消耗毒药
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool UsePoison(int count, out int shape)
        {
            shape = 0;

            UserItem poison = Equipment[(int)EquipmentSlot.Poison];

            if (poison == null || poison.Info.ItemType != ItemType.Poison || poison.Count < count) return false;

            shape = poison.Info.Shape;

            poison.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Poison, Count = poison.Count },
                Success = true
            });

            if (poison.Count != 0) return true;

            RemoveItem(poison);
            Equipment[(int)EquipmentSlot.Poison] = null;
            poison.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }
        /// <summary>
        /// 消耗护身符
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool UseAmulet(int count, int shape)
        {
            UserItem amulet = Equipment[(int)EquipmentSlot.Amulet];

            if (amulet == null || amulet.Info.ItemType != ItemType.Amulet || amulet.Count < count || amulet.Info.Shape != shape) return false;

            amulet.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet, Count = amulet.Count },
                Success = true
            });

            if (amulet.Count != 0) return true;

            RemoveItem(amulet);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            amulet.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }

        /// <summary>
        /// 使用护身符
        /// </summary>
        /// <param name="count"></param>
        /// <param name="shape"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        public bool UseAmulet(int count, int shape, out Stats stats)
        {
            stats = null;

            UserItem amulet = Equipment[(int)EquipmentSlot.Amulet];

            if (amulet == null || amulet.Info.ItemType != ItemType.Amulet || amulet.Count < count || amulet.Info.Shape != shape) return false;

            amulet.Count -= count;

            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Amulet, Count = amulet.Count },
                Success = true
            });

            stats = new Stats(amulet.Info.Stats);

            if (amulet.Count != 0) return true;

            RemoveItem(amulet);
            Equipment[(int)EquipmentSlot.Amulet] = null;
            amulet.Delete();

            RefreshStats();
            RefreshWeight();

            return true;
        }

        // 刷新显示，value为变化值而不是绝对值, 例如
        // ItemStatsChangeRefresh(int(EquipmentSlot.Weapon), Stat.Luck, -1, StatSource.Refine)
        // 效果为使武器幸运-1
        public void ItemStatsChangeRefresh(EquipmentSlot itemPosition, Stat statType, int value, StatSource source, bool isAdd = true)
        {
            //加属性
            UserItem item = Equipment[(int)itemPosition];
            item.AddStat(statType, value, source);
            item.StatsChanged();
            // 构建封包 加入队列
            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)itemPosition, NewStats = new Stats() };
            //result.FullItemStats = item.ToClientInfo().FullItemStats;
            result.NewStats[statType] = value;
            result.FullItemStats = item.ToClientInfo().FullItemStats;
            if (isAdd)
                Stats[statType] += value;
            else
                Stats[statType] = value;
            Enqueue(result);

            // 刷新实时属性显示
            RefreshStats();
        }

        //获取指定道具的格子类型
        public GridType GetItemGridType(int itemIndex)
        {
            if (Equipment.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.Equipment;
            }
            if (Inventory.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.Inventory;
            }
            if (PatchGrid.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.PatchGrid;
            }
            if (Storage.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.Storage;
            }
            if (FishingEquipment.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.FishingEquipment;
            }

            if (Companion?.Equipment != null && Companion.Equipment.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.CompanionEquipment;
            }
            if (Companion?.Inventory != null && Companion.Inventory.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.CompanionInventory;
            }

            if (Character.Account?.GuildMember?.Guild?.Storage != null && Character.Account.GuildMember.Guild.Storage.Any(item => item != null && item.Index == itemIndex))
            {
                return GridType.GuildStorage;
            }

            return GridType.None;
        }

        //获取指定道具的格子类型
        public GridType GetItemGridType(UserItem item)
        {
            return item == null ? GridType.None : GetItemGridType(item.Index);
        }
        //完全刷新某道具
        public void ItemCompleteRefresh(int userItemIndex)
        {
            UserItem item = Character.Items.FirstOrDefault(x => x.Index == userItemIndex);
            if (item == null)
            {
                SEnvir.Log("无法刷新物品: 找不到指定物品,请检查传入的Index");
                return;
            }

            GridType itemGrid = GetItemGridType(userItemIndex);
            if (itemGrid == GridType.None)
            {
                SEnvir.Log("无法刷新物品: 找不到对应的格子");
                return;
            }

            int slot = item.Slot;
            if (item.Slot >= Globals.FishingOffSet)
            {
                slot -= Globals.FishingOffSet;
            }
            else if (item.Slot >= Globals.PatchOffSet)
            {
                slot -= Globals.PatchOffSet;
            }
            else if (item.Slot >= Globals.EquipmentOffSet)
            {
                slot -= Globals.EquipmentOffSet;
            }

            item.StatsChanged();

            Enqueue(new S.ItemStatsRefreshed
            {
                GridType = itemGrid,
                Slot = slot,
                NewStats = new Stats(item.Stats),
                FullItemStats = item.ToClientInfo().FullItemStats
            });

            Enqueue(new S.ItemExperience
            {
                Target = new CellLinkInfo { GridType = itemGrid, Slot = slot },
                Experience = item.Experience,
                Level = item.Level,
                Flags = item.Flags
            });

            RefreshStats();
            RefreshWeight();
        }
        /// <summary>
        /// 使用祝福油
        /// </summary>
        /// <returns></returns>
        public bool UseOilOfBenediction()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];   //判断武器装备格子是否有武器

            if (weapon == null) return false;  //如果武器为空 跳过

            int luck = 0;  //定义 幸运值0

            foreach (UserItemStat stat in weapon.AddedStats)  //遍历 武器的属性增加
            {
                if (stat.Stat != Stat.Luck) continue;   //如果武器的属性没有幸运 继续
                if (stat.StatSource != StatSource.Enhancement) continue;  //如果武器不等增加 继续

                luck += stat.Amount;  //幸运 += 属性数值
            }

            if (weapon.Stats[Stat.Luck] >= Config.MaxLuck)
            {
                Connection.ReceiveChat($"达到幸运最大值无法使用".Lang(Connection.Language), MessageType.System);
                return false;  //如果幸运大于或等于最高幸运值 跳出
            }

            //发包更新武器属性值变化
            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
            Enqueue(result);

            if (luck > Config.MaxCurse && SEnvir.Random.Next(Config.CurseRate) == 0)  //如果幸运大于最大诅咒值 与 诅咒几率等0
            {
                weapon.AddStat(Stat.Luck, -1, StatSource.Enhancement);  //武器增加幸运值-1
                weapon.StatsChanged();                                  //武器属性值变化
                result.NewStats[Stat.Luck]--;                           //结果 新属性信息[幸运]--

                Stats[Stat.Luck]--;                                     //属性值运行--
            }
            else if (luck <= 0 || SEnvir.Random.Next(luck * Config.LuckRate) == 0)  //如果幸运小于等0 或 随机值幸运*设置几率=0 
            {
                weapon.AddStat(Stat.Luck, 1, StatSource.Enhancement);  //武器增加幸运1
                weapon.StatsChanged();                                 //武器属性值变化  
                result.NewStats[Stat.Luck]++;                          //结果 新属性信息[幸运]++

                Stats[Stat.Luck] = Math.Min(Config.MaxLucky, Stats[Stat.Luck] + 1);   //幸运值=最小值(设置的最大值，幸运+1)
            }
            //刷新完整属性列表
            result.FullItemStats = weapon.ToClientInfo().FullItemStats;
            return true;
        }
        /// <summary>
        /// 使用武器强化油
        /// </summary>
        /// <returns></returns>
        public bool UseOilOfConservation()
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            int strength = 0;

            if (weapon == null) return false;

            foreach (UserItemStat stat in weapon.AddedStats)
            {
                if (stat.Stat != Stat.Strength) continue;
                if (stat.StatSource != StatSource.Enhancement) continue;

                strength += stat.Amount;
            }

            if (strength >= Config.MaxStrength) return false;

            S.ItemStatsChanged result = new S.ItemStatsChanged
            {
                GridType = GridType.Equipment,
                Slot = (int)EquipmentSlot.Weapon,
                NewStats = new Stats(),
                FullItemStats = weapon.ToClientInfo().FullItemStats
            };
            Enqueue(result);

            if (strength > 0 && SEnvir.Random.Next(Config.StrengthLossRate) == 0)
            {
                weapon.AddStat(Stat.Strength, -1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Strength]--;
            }
            else if (strength <= 0 || SEnvir.Random.Next(strength * Config.StrengthAddRate) == 0)
            {
                weapon.AddStat(Stat.Strength, 1, StatSource.Enhancement);
                weapon.StatsChanged();
                result.NewStats[Stat.Strength]++;
            }
            //刷新完整属性列表
            result.FullItemStats = weapon.ToClientInfo().FullItemStats;
            return true;
        }
        /// <summary>
        /// 特殊修理
        /// </summary>
        /// <param name="slot">装备格子</param>
        /// <returns></returns>
        public bool SpecialRepair(EquipmentSlot slot)
        {
            UserItem item = Equipment[(int)slot];

            if (item == null) return false;

            if (item.CurrentDurability >= item.MaxDurability || !item.Info.CanRepair) return false;

            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;

            item.CurrentDurability = item.MaxDurability;

            Enqueue(new S.NPCRepair { Links = new List<CellLinkInfo> { new CellLinkInfo { GridType = GridType.Equipment, Slot = (int)slot, Count = 1 } }, Special = true, Success = true, SpecialRepairDelay = TimeSpan.Zero });

            return true;
        }
        /// <summary>
        /// 是否显示头盔
        /// </summary>
        /// <param name="value"></param>
        public void HelmetToggle(bool value)
        {
            if (Character.HideHelmet == value) return;

            Character.HideHelmet = value;
            SendShapeUpdate();
            Enqueue(new S.HelmetToggle { HideHelmet = Character.HideHelmet });
        }
        /// <summary>
        /// 是否显示盾牌
        /// </summary>
        /// <param name="value"></param>
        public void ShieldToggle(bool value)
        {
            if (Character.HideShield == value) return;

            Character.HideShield = value;
            SendShapeUpdate();
            Enqueue(new S.ShieldToggle { HideShield = Character.HideShield });
        }
        /// <summary>
        /// 是否显示时装
        /// </summary>
        /// <param name="value"></param>
        public void FashionToggle(bool value)
        {
            if (Character.HideFashion == value) return;

            Character.HideFashion = value;
            SendShapeUpdate();
            Enqueue(new S.FashionToggle { HideFashion = Character.HideFashion });
        }
        /// <summary>
        /// 传奇宝箱变化
        /// </summary>
        /// <param name="p"></param>
        public void ChangeTrea(C.TreasureChange p)
        {
            if (TreaNew)
            {
                TreaChangeCount++;
                int cost = Config.Reset * TreaChangeCount;
                if (Character.Account.GameGold < cost)
                {
                    Connection.ReceiveChat($"你的赞助币不足，无法重置传奇宝箱。", MessageType.System);
                    return;
                }
                Character.Account.GameGold -= cost;
                Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold });

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "传奇宝箱",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.GameGold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.ItemAdd,
                    Amount = cost,
                    ExtraInfo = $""
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                cost = Config.Reset * (TreaChangeCount + 1);
                S.TreasureChest packet = new S.TreasureChest { Cost = cost, Count = TreaCount, Items = new List<ClientUserItem>() };
                TreaItems.Clear();
                for (int i = 0; i < TreaBoxOn.Length; i++)
                {
                    TreaBoxOn[i] = true;
                    int index = SEnvir.Random.Next(SEnvir.TreaItemList.Count);
                    UserItem itemtem = SEnvir.CreateDropItem(SEnvir.TreaItemList[index]);

                    //记录物品来源
                    SEnvir.RecordTrackingInfo(itemtem, CurrentMap?.Info?.Description, ObjectType.None, "传奇宝箱".Lang(Connection.Language), Character?.CharacterName);

                    itemtem.IsTemporary = true;
                    packet.Items.Add(itemtem.ToClientInfo());
                    TreaItems.Add(SEnvir.TreaItemList[index]);

                }
                Enqueue(packet);
            }
        }
        /// <summary>
        /// 传奇宝箱开出奖品
        /// </summary>
        /// <param name="p"></param>
        public void SelectTrea(C.TreasureSelect p)
        {
            S.TreasureSel packet = new S.TreasureSel { Slot = p.Slot };
            if (TreaCount > 0 && p.Slot >= 0 && p.Slot < TreaBoxOn.Length && TreaBoxOn[p.Slot])
            {
                int cost = TreaSelCount * Config.LuckDraw;
                if (Character.Account.GameGold < cost)
                {
                    Connection.ReceiveChat($"你的赞助币不足，无法继续开启传奇宝箱。", MessageType.System);
                    return;
                }
                Character.Account.GameGold -= cost;
                Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold });

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "传奇宝箱",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.GameGold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.ItemAdd,
                    Amount = cost,
                    ExtraInfo = $""
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                TreaSelCount++;
                packet.Cost = TreaSelCount * Config.LuckDraw;
                TreaNew = false;
                ItemInfo info = TreaItems[SEnvir.Random.Next(TreaItems.Count)];

                UserItem item = SEnvir.CreateFreshItem(info);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.NPC, "宝箱".Lang(Connection.Language), Character?.CharacterName);

                try
                {
                    TreaItems.Remove(info);
                    TreaBoxOn[p.Slot] = false;
                    TreaCount--;
                    packet.Count = TreaCount;
                    packet.Item = item.ToClientInfo();
                    Enqueue(packet);
                    if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
                    {
                        MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                        mail.Account = Character.Account;
                        mail.Subject = "传奇宝箱奖品";
                        mail.Message = string.Format("你获得了奖品'{0}{1}'，请领取你的奖励。", item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count);
                        mail.Sender = "传奇宝箱";
                        item.Mail = mail;
                        item.Slot = 0;
                        mail.HasItem = true;

                        Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });
                    }
                    else
                    {
                        GainItem(item);
                    }
                }
                catch
                {

                }
                //if (item.Info == null) return;
            }
        }
        /// <summary>
        /// 获取背包空余格子数目
        /// </summary>
        /// <param name="gridType">格子类型</param>
        /// <returns></returns>
        public int RemainingSlots(GridType gridType)
        {
            UserItem[] fromArray;
            switch (gridType)
            {
                case GridType.Inventory:   //背包
                    fromArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    fromArray = PatchGrid;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return 0;
                    fromArray = Companion.Inventory;
                    break;
                case GridType.CompanionEquipment:  //宠物装备
                    if (Companion == null) return 0;
                    fromArray = Companion.Equipment;
                    break;
                default:
                    return 0;
            }
            return fromArray.Count(x => x == null);
        }

        #endregion

        // 设置道具自定义前缀和前缀颜色
        public void SetItemCustomPrefix(UserItem item, string prefix, PythonTuple color = null)
        {
            if (color == null || color.Count != 3)
            {
                SetItemCustomPrefix(item, prefix, Color.Yellow);
            }
            else
            {
                SetItemCustomPrefix(item, prefix, Color.FromArgb((int)color[0], (int)color[1], (int)color[2]));
            }
        }
        public void SetItemCustomPrefix(UserItem item, string prefix, Color color)
        {
            if (item == null) return;

            item.CustomPrefixText = prefix;
            item.CustomPrefixColor = color;

            Enqueue(new S.ItemCellRefresh { Item = item.ToClientInfo(), Grid = GetItemGridType(item) });
        }
    }
}
