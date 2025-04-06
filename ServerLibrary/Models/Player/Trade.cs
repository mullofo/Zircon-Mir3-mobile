using Library;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using Server.Utils.Logging;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 交易
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 交易
        /// </summary>
        public PlayerObject TradePartner;
        /// <summary>
        /// 交易请求
        /// </summary>
        public PlayerObject TradePartnerRequest;
        public Dictionary<UserItem, CellLinkInfo> TradeItems = new Dictionary<UserItem, CellLinkInfo>();
        public bool TradeConfirmed;
        public long TradeGold;

        #region Trade

        public void TradeClose()  //交易失败
        {
            if (TradePartner == null) return;

            Enqueue(new S.TradeClose());

            if (TradePartner?.Node != null)
                TradePartner.Enqueue(new S.TradeClose());

            TradePartner.TradePartner = null;
            TradePartner.TradeItems.Clear();
            TradePartner.TradeGold = 0;
            TradePartner.TradeConfirmed = false;

            TradePartner = null;
            TradeItems.Clear();
            TradeGold = 0;
            TradeConfirmed = false;
        }

        public void TradeRequest(int chrIndex)  //交易要求
        {
            if (TradePartner != null)   //交易对象
            {
                Connection.ReceiveChat("Trade.TradeAlreadyTrading".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeAlreadyTrading".Lang(con.Language), MessageType.System);
                return;
            }
            if (TradePartnerRequest != null)  //交易发送请求
            {
                Connection.ReceiveChat("Trade.TradeAlreadyHaveRequest".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeAlreadyHaveRequest".Lang(con.Language), MessageType.System);
                return;
            }

            PlayerObject player = null;
            //手游验证是否在范围内
            if (ClientPlatform == Platform.Mobile)
            {
                player = SEnvir.GetCharacter(chrIndex).Player;
                if (player == null || Functions.Distance(CurrentLocation, player.CurrentLocation) >= 8)  //如果玩家 为空  或者 玩家超过施法范围
                {
                    Connection.ReceiveChat("不在交易范围内！！！", MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("不在交易范围内！！！", MessageType.System);
                    return;
                }
            }
            //桌面验证是否面对面
            else
            {
                Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction));

                if (cell?.Objects == null) return;

                foreach (MapObject ob in cell.Objects)
                {
                    if (ob.Race != ObjectType.Player) continue;
                    player = (PlayerObject)ob;
                    break;
                }

                if (player == null || player.Direction != Functions.ShiftDirection(Direction, 4))  //如果玩家 为空  或者 玩家不是面对面交易
                {
                    Connection.ReceiveChat("Trade.TradeNeedFace".Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Trade.TradeNeedFace".Lang(con.Language), MessageType.System);
                    return;
                }
            }

            if (SEnvir.IsBlocking(Character.Account, player.Character.Account))  //玩家拒绝交易
            {
                Connection.ReceiveChat("Trade.TradeTargetNotAllowed".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeTargetNotAllowed".Lang(con.Language, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.TradePartner != null)
            {
                Connection.ReceiveChat("Trade.TradeTargetAlreadyTrading".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeTargetAlreadyTrading".Lang(con.Language, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.TradePartnerRequest != null)
            {
                Connection.ReceiveChat("Trade.TradeTargetAlreadyHaveRequest".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeTargetAlreadyHaveRequest".Lang(con.Language, player.Character.CharacterName), MessageType.System);
                return;
            }


            if (!player.Character.Account.AllowTrade)
            {
                Connection.ReceiveChat("Trade.TradeTargetNotAllowed".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat("Trade.TradeNotAllowed".Lang(player.Connection.Language, Character.CharacterName), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeTargetNotAllowed".Lang(con.Language, player.Character.CharacterName), MessageType.System);

                foreach (SConnection con in player.Connection.Observers)
                    con.ReceiveChat("Trade.TradeNotAllowed".Lang(con.Language, Character.CharacterName), MessageType.System);
                return;
            }

            if (player.Dead || Dead)
            {
                Connection.ReceiveChat("Trade.TradeTargetDead".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeTargetDead".Lang(con.Language), MessageType.System);
                return;
            }


            player.TradePartnerRequest = this;
            player.Enqueue(new S.TradeRequest { Name = Name, ObserverPacket = false });

            Connection.ReceiveChat("Trade.TradeRequested".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("Trade.TradeRequested".Lang(con.Language, player.Character.CharacterName), MessageType.System);
        }
        public void TradeAccept()  //交易接受
        {
            if (TradePartnerRequest?.Node == null || TradePartnerRequest.TradePartner != null || TradePartnerRequest.Dead) return;

            //如果发起交易方是手游 验证范围
            if (TradePartnerRequest.ClientPlatform == Platform.Mobile)
            {
                if (Functions.Distance(CurrentLocation, TradePartnerRequest.CurrentLocation) >= Globals.MagicRange) return;
            }
            //其他交易必须面对面
            else
            {
                if (Functions.Distance(CurrentLocation, TradePartnerRequest.CurrentLocation) != 1 || TradePartnerRequest.Direction != Functions.ShiftDirection(Direction, 4)) return;
            }

            TradePartner = TradePartnerRequest;
            TradePartnerRequest.TradePartner = this;

            TradePartner.Enqueue(new S.TradeOpen { Name = Name });
            Enqueue(new S.TradeOpen { Name = TradePartner.Name });
        }

        public void TradeAddItem(CellLinkInfo cell) //交易增加道具
        {
            S.TradeAddItem result = new S.TradeAddItem
            {
                Cell = cell,
            };

            Enqueue(result);

            if (!ParseLinks(cell) || TradePartner == null || TradeItems.Count >= 15) return;

            UserItem[] fromArray;

            switch (cell.GridType)
            {
                case GridType.Inventory:   //背包
                    fromArray = Inventory;
                    break;
                case GridType.PatchGrid:   //碎片包裹
                    fromArray = PatchGrid;
                    break;
                case GridType.Equipment:  //人物装备栏
                    fromArray = Equipment;
                    break;
                case GridType.CompanionInventory: //宠物背包
                    if (Companion == null) return;

                    fromArray = Companion.Inventory;
                    break;
                case GridType.Storage: //仓库
                    /*if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("".Lang(Connection.Language.StorageSafeZone, MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat(con.Language.StorageSafeZone, MessageType.System);

                        return;
                    }*/

                    fromArray = Storage;
                    break;
                /*case GridType.GuildStorage:
                    if (Character.GuildMemberInfo == null) return;

                    if (!Character.GuildMemberInfo.Permissions.HasFlag(GuildPermissions.GetItem))
                    {
                        ReceiveChat("You do no have the permissions to take from the guild storage", ChatType.System);
                        return;
                    }

                    if (!CurrentCell.IsSafeZone)
                    {
                        ReceiveChat("You cannot use guild storage unless you are in a safe zone", ChatType.Hint);
                        return;
                    }

                    fromArray = Character.GuildMemberInfo.GuildInfo.StorageArray;
                    break;*/
                case GridType.FishingEquipment:   //钓鱼装备栏
                    fromArray = FishingEquipment;
                    break;
                default:
                    return;
            }

            if (cell.Slot < 0 || cell.Slot >= fromArray.Length) return;

            UserItem fromItem = fromArray[cell.Slot];

            if (fromItem == null || cell.Count > fromItem.Count || (!TradePartner.Character.Account.Admin && !Character.Account.Admin && ((fromItem.Flags & UserItemFlags.Bound) == UserItemFlags.Bound || !fromItem.Info.CanTrade))) return;
            if ((fromItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (TradeItems.ContainsKey(fromItem)) return;

            //All is Well
            result.Success = true;
            TradeItems[fromItem] = cell;
            S.TradeItemAdded packet = new S.TradeItemAdded
            {
                Item = fromItem.ToClientInfo()
            };
            packet.Item.Count = cell.Count;
            TradePartner.Enqueue(packet);
        }
        public void TradeAddGold(long gold) //交易增加金币
        {
            S.TradeAddGold p = new S.TradeAddGold
            {
                Gold = TradeGold,
            };
            Enqueue(p);

            if (TradePartner == null || TradeGold >= gold) return;

            if (gold <= 0 || gold > Gold) return;

            TradeGold = gold;
            p.Gold = TradeGold;

            //All is Well
            S.TradeGoldAdded packet = new S.TradeGoldAdded
            {
                Gold = TradeGold,
            };

            TradePartner.Enqueue(packet);
        }

        public void TradeConfirm()  //交易确认
        {
            if (TradePartner == null) return;

            TradeConfirmed = true;

            if (!TradePartner.TradeConfirmed)
            {
                Connection.ReceiveChat("Trade.TradeWaiting".Lang(Connection.Language), MessageType.System);
                TradePartner.Connection.ReceiveChat("Trade.TradePartnerWaiting".Lang(TradePartner.Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeWaiting".Lang(con.Language), MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat("Trade.TradePartnerWaiting".Lang(con.Language), MessageType.System);

                return;
            }

            long gold = Gold;
            gold += TradePartner.TradeGold - TradeGold;

            if (gold < 0)
            {
                Connection.ReceiveChat("Trade.TradeNoGold".Lang(Connection.Language), MessageType.System);
                TradePartner.Connection.ReceiveChat("Trade.TradePartnerNoGold".Lang(TradePartner.Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeNoGold".Lang(con.Language), MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat("Trade.TradePartnerNoGold".Lang(con.Language), MessageType.System);
                TradeClose();
                return;
            }


            gold = TradePartner.Gold;
            gold += TradeGold - TradePartner.TradeGold;

            if (gold < 0)
            {
                Connection.ReceiveChat("Trade.TradePartnerNoGold".Lang(Connection.Language), MessageType.System);
                TradePartner.Connection.ReceiveChat("Trade.TradeNoGold".Lang(TradePartner.Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradePartnerNoGold".Lang(con.Language), MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat("Trade.TradeNoGold".Lang(con.Language), MessageType.System);

                TradeClose();
                return;
            }

            List<ItemCheck> checks = new List<ItemCheck>();

            int inventtoryemptycount = 0;
            //开始索引
            for (int i = 0; i < TradePartner.Inventory.Length; i++)
            {
                UserItem item = TradePartner.Inventory[i];
                if (item == null)
                {
                    inventtoryemptycount++;
                }
            }

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
            {
                UserItem[] fromArray;
                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:  //背包
                        fromArray = Inventory;
                        if (TradeItems.Count > inventtoryemptycount)
                        {
                            Connection.ReceiveChat("Trade.TradeWaiting".Lang(Connection.Language), MessageType.System);
                            TradePartner.Connection.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(TradePartner.Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Trade.TradeWaiting".Lang(con.Language), MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(con.Language), MessageType.System);

                            TradePartner.TradeConfirmed = false;
                            TradePartner.Enqueue(new S.TradeUnlock());
                            return;
                        }
                        break;
                    case GridType.PatchGrid:   //碎片包裹
                        fromArray = PatchGrid;
                        break;
                    case GridType.Equipment:  //人物装备栏
                        fromArray = Equipment;
                        break;
                    case GridType.Storage:   //仓库
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        if (Companion == null)
                        {
                            Connection.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(Connection.Language), MessageType.System);
                            TradePartner.Connection.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(TradePartner.Connection.Language, Name), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(con.Language), MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(con.Language, Name), MessageType.System);

                            TradeClose();
                            return;
                        }
                        fromArray = Companion.Inventory;
                        break;
                    case GridType.FishingEquipment:   //钓鱼装备栏
                        fromArray = FishingEquipment;
                        break;
                    default:
                        //MAJOR LOGIC FAILURE 
                        return;
                }

                if (fromArray[pair.Value.Slot] != pair.Key || pair.Key.Count < pair.Value.Count)
                {
                    Connection.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(Connection.Language), MessageType.System);
                    TradePartner.Connection.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(TradePartner.Connection.Language, Name), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(con.Language), MessageType.System);

                    foreach (SConnection con in TradePartner.Connection.Observers)
                        con.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(con.Language, Name), MessageType.System);

                    TradeClose();
                    return;
                }

                UserItem item = fromArray[pair.Value.Slot];

                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                bool handled = false;

                foreach (ItemCheck check in checks)
                {
                    if (check.Info != item.Info) continue;
                    if ((check.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((check.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                    if ((check.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                    if ((check.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;


                    check.Count += pair.Value.Count;
                    handled = true;
                    break;
                }

                if (handled) continue;

                checks.Add(new ItemCheck(item, pair.Value.Count, item.Flags, item.ExpireTime));
            }

            if (!TradePartner.CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat("Trade.TradeWaiting".Lang(Connection.Language), MessageType.System);
                TradePartner.Connection.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(TradePartner.Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeWaiting".Lang(con.Language), MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(con.Language), MessageType.System);

                TradePartner.TradeConfirmed = false;
                TradePartner.Enqueue(new S.TradeUnlock());
                return;
            }

            checks.Clear();

            inventtoryemptycount = 0;
            //开始索引
            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];
                if (item == null)
                {
                    inventtoryemptycount++;
                }
            }
            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
            {
                UserItem[] fromArray;
                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:        //背包
                        fromArray = TradePartner.Inventory;
                        if (TradePartner.TradeItems.Count > inventtoryemptycount)
                        {
                            Connection.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(Connection.Language), MessageType.System);
                            TradePartner.Connection.ReceiveChat("Trade.TradeWaiting".Lang(TradePartner.Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(con.Language), MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat("Trade.TradeWaiting".Lang(con.Language), MessageType.System);

                            TradeConfirmed = false;
                            Enqueue(new S.TradeUnlock());
                            return;
                        }
                        break;
                    case GridType.PatchGrid:         //碎片包裹
                        fromArray = TradePartner.PatchGrid;
                        break;
                    case GridType.Equipment:        //人物装备栏
                        fromArray = TradePartner.Equipment;
                        break;
                    case GridType.Storage:        //仓库
                        fromArray = TradePartner.Storage;
                        break;
                    case GridType.CompanionInventory:     //宠物包裹
                        if (TradePartner.Companion == null)
                        {
                            Connection.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(Connection.Language, TradePartner.Name), MessageType.System);
                            TradePartner.Connection.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(TradePartner.Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(con.Language, TradePartner.Name), MessageType.System);

                            foreach (SConnection con in TradePartner.Connection.Observers)
                                con.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(con.Language), MessageType.System);
                            TradeClose();
                            return;
                        }
                        fromArray = TradePartner.Companion.Inventory;
                        break;
                    case GridType.FishingEquipment:   //钓鱼装备栏
                        fromArray = TradePartner.FishingEquipment;
                        break;
                    default:
                        //MAJOR LOGIC FAILURE 
                        return;
                }

                if (fromArray[pair.Value.Slot] != pair.Key || pair.Key.Count < pair.Value.Count)
                {
                    Connection.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(Connection.Language, TradePartner.Name), MessageType.System);
                    TradePartner.Connection.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(TradePartner.Connection.Language), MessageType.System);


                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Trade.TradeFailedPartnerItemsChanged".Lang(con.Language, TradePartner.Name), MessageType.System);

                    foreach (SConnection con in TradePartner.Connection.Observers)
                        con.ReceiveChat("Trade.TradeFailedItemsChanged".Lang(con.Language), MessageType.System);

                    TradeClose();
                    return;
                }

                UserItem item = fromArray[pair.Value.Slot];
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                bool handled = false;

                foreach (ItemCheck check in checks)
                {
                    if (check.Info != item.Info) continue;
                    if ((check.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((item.Flags & UserItemFlags.Expirable) == UserItemFlags.Expirable) continue;
                    if ((check.Flags & UserItemFlags.Bound) != (item.Flags & UserItemFlags.Bound)) continue;
                    if ((check.Flags & UserItemFlags.Worthless) != (item.Flags & UserItemFlags.Worthless)) continue;
                    if ((check.Flags & UserItemFlags.NonRefinable) != (item.Flags & UserItemFlags.NonRefinable)) continue;

                    check.Count += pair.Value.Count;
                    handled = true;
                    break;
                }

                if (handled) continue;

                checks.Add(new ItemCheck(item, pair.Value.Count, item.Flags, item.ExpireTime));
            }

            if (!CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(Connection.Language), MessageType.System);
                TradePartner.Connection.ReceiveChat("Trade.TradeWaiting".Lang(TradePartner.Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Trade.TradeNotEnoughSpace".Lang(con.Language), MessageType.System);

                foreach (SConnection con in TradePartner.Connection.Observers)
                    con.ReceiveChat("Trade.TradeWaiting".Lang(con.Language), MessageType.System);

                TradeConfirmed = false;
                Enqueue(new S.TradeUnlock());
                return;
            }

            Enqueue(new S.ItemsChanged { Links = TradeItems.Values.ToList(), Success = true });

            //交易成功，双方都可以接受没有问题的商品，因此发送出去
            UserItem tempItem;

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
            {

                if (pair.Key.Count > pair.Value.Count)
                {
                    pair.Key.Count -= pair.Value.Count;

                    tempItem = SEnvir.CreateFreshItem(pair.Key);

                    //记录物品来源
                    SEnvir.RecordTrackingInfo(pair.Key, tempItem);

                    tempItem.Count = pair.Value.Count;
                    TradePartner.GainItem(tempItem);
                    continue;
                }

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry2 = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "交易",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.None,
                    Action = CurrencyAction.Undefined,
                    Source = CurrencySource.TradeAdd,
                    Amount = pair.Value.Count,
                    ExtraInfo = $"{TradePartner.Character}交易给{Character}: {pair.Key.Info.ItemName}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry2);

                UserItem[] fromArray;

                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:   //背包
                        fromArray = Inventory;
                        break;
                    case GridType.PatchGrid:   //碎片包裹
                        fromArray = PatchGrid;
                        break;
                    case GridType.Equipment:  //人物装备栏
                        fromArray = Equipment;
                        break;
                    case GridType.Storage:    //仓库
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        fromArray = Companion.Inventory;
                        break;
                    case GridType.FishingEquipment:   //钓鱼装备栏
                        fromArray = FishingEquipment;
                        break;
                    default:
                        return;
                }

                fromArray[pair.Value.Slot] = null;
                RemoveItem(pair.Key);
                TradePartner.GainItem(pair.Key);
            }
            TradePartner.Enqueue(new S.ItemsChanged { Links = TradePartner.TradeItems.Values.ToList(), Success = true });

            foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
            {
                if (pair.Key.Count > pair.Value.Count)
                {
                    pair.Key.Count -= pair.Value.Count;

                    tempItem = SEnvir.CreateFreshItem(pair.Key);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(pair.Key, tempItem);

                    tempItem.Count = pair.Value.Count;
                    GainItem(tempItem);
                    continue;
                }

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry3 = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "交易",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.None,
                    Action = CurrencyAction.Undefined,
                    Source = CurrencySource.TradeAdd,
                    Amount = pair.Value.Count,
                    ExtraInfo = $"{Character}获得{TradePartner.Character}给的: {pair.Key.Info.ItemName}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry3);

                UserItem[] fromArray;

                switch (pair.Value.GridType)
                {
                    case GridType.Inventory:        //背包
                        fromArray = TradePartner.Inventory;
                        break;
                    case GridType.PatchGrid:        //碎片包裹
                        fromArray = TradePartner.PatchGrid;
                        break;
                    case GridType.Equipment:        //人物装备栏
                        fromArray = TradePartner.Equipment;
                        break;
                    case GridType.Storage:          //仓库
                        fromArray = TradePartner.Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        fromArray = TradePartner.Companion.Inventory;
                        break;
                    case GridType.FishingEquipment:   //钓鱼装备栏
                        fromArray = FishingEquipment;
                        break;
                    default:
                        return;
                }

                fromArray[pair.Value.Slot] = null;
                TradePartner.RemoveItem(pair.Key);
                GainItem(pair.Key);
            }

            RefreshStats();
            SendShapeUpdate();
            TradePartner.RefreshStats();
            TradePartner.SendShapeUpdate();

            Gold += TradePartner.TradeGold - TradeGold;
            GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "交易系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Undefined,
                Source = CurrencySource.ItemAdd,
                Amount = TradePartner.TradeGold - TradeGold,
                ExtraInfo = $"{TradePartner.Character}交易给{Character}"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            TradePartner.Gold += TradeGold - TradePartner.TradeGold;
            TradePartner.GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry1 = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "交易系统",
                Time = SEnvir.Now,
                Character = TradePartner.Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Undefined,
                Source = CurrencySource.ItemAdd,
                Amount = TradeGold - TradePartner.TradeGold,
                ExtraInfo = $"{Character}获得{TradePartner.Character}交易"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry1);

            Connection.ReceiveChat("Trade.TradeComplete".Lang(Connection.Language), MessageType.System);
            TradePartner.Connection.ReceiveChat("Trade.TradeComplete".Lang(TradePartner.Connection.Language), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("Trade.TradeComplete".Lang(con.Language), MessageType.System);

            foreach (SConnection con in TradePartner.Connection.Observers)
                con.ReceiveChat("Trade.TradeComplete".Lang(con.Language), MessageType.System);

            TradeClose();
        }

        #endregion
    }
}
