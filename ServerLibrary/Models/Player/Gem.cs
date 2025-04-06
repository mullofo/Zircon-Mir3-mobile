using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //宝石镶嵌
    {
        public void AttachGemToItem(C.AttachGem p)//宝石镶嵌
        {
            if (Dead || (p.FromGrid == p.ToGrid && p.FromSlot == p.ToSlot))
            {
                return;
            }
            UserItem[] array;
            switch (p.FromGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array = Inventory;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer in Connection.Observers)
                        {
                            observer.ReceiveChat("Items.StorageSafeZone".Lang(observer.Language), MessageType.System);
                        }
                        return;
                    }
                    array = Storage;
                    if (p.FromSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null)
                    {
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if (!InSafeZone && (p.ToGrid != GridType.Storage || p.ToGrid != GridType.PatchGrid))
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    array = Character.Account.GuildMember.Guild.Storage;
                    if (p.FromSlot >= Character.Account.GuildMember.Guild.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array = Companion.Inventory;
                    if (p.FromSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.FromSlot < 0 || p.FromSlot >= array.Length)
            {
                return;
            }
            UserItem gemOrb = array[p.FromSlot];//宝石
            if (gemOrb == null || (gemOrb.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage || (gemOrb.Info.ItemType != ItemType.Gem && gemOrb.Info.ItemType != ItemType.Rune && gemOrb.Info.ItemType != ItemType.Orb))
            {
                return;
            }
            UserItem[] array2;
            switch (p.ToGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array2 = Inventory;
                    break;
                case GridType.Equipment:
                    array2 = Equipment;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer2 in Connection.Observers)
                        {
                            observer2.ReceiveChat("Items.StorageSafeZone".Lang(observer2.Language), MessageType.System);
                        }
                        return;
                    }
                    array2 = Storage;
                    if (p.ToSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null)
                    {
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if (!InSafeZone && p.FromGrid != GridType.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    array2 = Character.Account.GuildMember.Guild.Storage;
                    if (p.ToSlot >= Character.Account.GuildMember.Guild.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array2 = Companion.Inventory;
                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.ToSlot < 0 || p.ToSlot >= array2.Length)
            {
                return;
            }
            UserItem targetItem = array2[p.ToSlot];//目标物品
            if (targetItem == null || (targetItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage || targetItem.Info.Effect == ItemEffect.NoGem)
            {
                Connection.ReceiveChat("该物品不支持镶嵌".Lang(Connection.Language), MessageType.System);
                return;
            }
            if (targetItem.Stats[Stat.UsedGemSlot] >= Config.MaxGemPerItem) //最大镶嵌数目
            {
                Connection.ReceiveChat("PlayerObject.MaxGemPerItem".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
                return;
            }


            if (targetItem.Stats[Stat.EmptyGemSlot] < 1) //插孔数目不够
            {
                Connection.ReceiveChat("PlayerObject.EmptyGemSlot".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
                return;
            }


            bool flag = false;
            ItemType itemType = targetItem.Info.ItemType;
            switch (gemOrb.Info.Shape)   //宝石镶嵌判断
            {
                case 0://万能型宝石
                    flag = (itemType == ItemType.Weapon || itemType == ItemType.Armour || itemType == ItemType.Helmet || itemType == ItemType.Necklace || itemType == ItemType.Bracelet || itemType == ItemType.Ring || itemType == ItemType.Shoes || itemType == ItemType.Belt || itemType == ItemType.Shield || itemType == ItemType.Wing || itemType == ItemType.Fashion);
                    break;
                case 1://只能用于武器
                    flag = (itemType == ItemType.Weapon);
                    break;
                case 2://只能用于衣服
                    flag = (itemType == ItemType.Armour);
                    break;
                case 3://只能用于头盔
                    flag = (itemType == ItemType.Helmet);
                    break;
                case 4://只能用于项链
                    flag = (itemType == ItemType.Necklace);
                    break;
                case 5://只能用于手镯
                    flag = (itemType == ItemType.Bracelet);
                    break;
                case 6://只能用于戒指
                    flag = (itemType == ItemType.Ring);
                    break;
                case 7://只能用于鞋子
                    flag = (itemType == ItemType.Shoes);
                    break;
                case 8://只能用于盾牌
                    flag = (itemType == ItemType.Shield);
                    break;
                case 9://只能用于翅膀
                    flag = (itemType == ItemType.Wing);
                    break;
                case 10://只能用于时装
                    flag = (itemType == ItemType.Fashion);
                    break;
                case 20://只能用于武器 项链 手镯 戒指
                    flag = (itemType == ItemType.Weapon || itemType == ItemType.Necklace || itemType == ItemType.Bracelet || itemType == ItemType.Ring);
                    break;
                case 21://只能用于衣服 盾牌 头盔 鞋子
                    flag = (itemType == ItemType.Armour || itemType == ItemType.Shield || itemType == ItemType.Helmet || itemType == ItemType.Shoes);
                    break;
                default:
                    break;
            }

            bool attachSuccess = false;
            bool itemBroke = false;
            var gemInfo = gemOrb.Info;

            if (flag)
            {
                S.ItemStatsChanged itemStatsChanged = new S.ItemStatsChanged
                {
                    GridType = p.ToGrid,
                    Slot = p.ToSlot,
                    NewStats = new Stats()
                };
                if (SEnvir.Random.Next(100) < gemOrb.Info.Stats[Stat.GemOrbSuccess])//镶嵌成功概率 
                {

                    /*
					foreach (KeyValuePair<Stat, int> value in gemOrb.Info.Stats.Values)
					{
						if (value.Key != Stat.GemOrbBrake && value.Key != Stat.GemOrbSuccess)
						{
							targetItem.AddStat(value.Key, value.Value, StatSource.GemOrb, gemOrb.Info.ItemName);
							itemStatsChanged.NewStats[value.Key] = value.Value;
						}
					}
                    */

                    List<FullItemStat> successfullyAdded = targetItem.AttachGem(gemOrb);
                    foreach (FullItemStat newlyAddedStat in successfullyAdded)
                    {
                        itemStatsChanged.NewStats[newlyAddedStat.Stat] = newlyAddedStat.Amount;
                    }

                    targetItem.AddStat(Stat.UsedGemSlot, 1, StatSource.None);//镶嵌数目+1
                    targetItem.AddStat(Stat.EmptyGemSlot, -1, StatSource.None);//空闲插槽-1
                    itemStatsChanged.NewStats[Stat.UsedGemSlot] = 1;
                    itemStatsChanged.NewStats[Stat.EmptyGemSlot] = -1;
                    itemStatsChanged.FullItemStats = targetItem.ToClientInfo().FullItemStats;

                    targetItem.StatsChanged();

                    Connection.ReceiveChat("PlayerObject.GemOrbSuccess".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);

                    attachSuccess = true;
                }
                else
                {
                    attachSuccess = false;
                    flag = false;
                }
                //itemUpgrade.Success = flag;
                //如果不成功
                if (!flag && SEnvir.Random.Next(100) >= gemOrb.Info.Stats[Stat.GemOrbBrake])
                {
                    flag = true;//没碎
                    itemBroke = false;
                }
                if (flag)
                {
                    Enqueue(itemStatsChanged);
                }
                gemOrb.Count--;
                if (gemOrb.Count < 0)
                {
                    gemOrb.Count = 0L;
                }
                GridType fromGrid = p.FromGrid;
                if (fromGrid == GridType.GuildStorage)
                {
                    Packet p2 = new S.ItemChanged
                    {
                        Link = new CellLinkInfo
                        {
                            GridType = p.FromGrid,
                            Slot = p.FromSlot,
                            Count = gemOrb.Count
                        },
                        Success = true
                    };
                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject playerObject = member.Account.Connection?.Player;
                        if (playerObject != null && playerObject != this)
                        {
                            playerObject.Enqueue(p2);
                        }
                    }
                }
                Enqueue(new S.ItemChanged
                {
                    Link = new CellLinkInfo
                    {
                        GridType = p.FromGrid,
                        Slot = p.FromSlot,
                        Count = gemOrb.Count
                    },
                    Success = true
                });
                if (gemOrb.Count <= 0)
                {
                    array[gemOrb.Slot] = null;
                    RemoveItem(gemOrb);
                    gemOrb.Delete();
                }
                GridType toGrid = p.ToGrid;
                if (toGrid == GridType.GuildStorage)
                {
                    foreach (GuildMemberInfo member2 in Character.Account.GuildMember.Guild.Members)
                    {
                        PlayerObject playerObject2 = member2.Account.Connection?.Player;
                        if (playerObject2 != null && playerObject2 != this)
                        {
                            if (flag)
                            {
                                playerObject2.Enqueue(itemStatsChanged);
                            }
                            else
                            {
                                playerObject2.Enqueue(new S.ItemChanged
                                {
                                    Link = new CellLinkInfo
                                    {
                                        GridType = p.ToGrid,
                                        Slot = p.ToSlot,
                                        Count = 0L
                                    },
                                    Success = true
                                });
                            }
                        }
                    }
                }
                if (!flag)
                {
                    Enqueue(new S.ItemChanged
                    {
                        Link = new CellLinkInfo
                        {
                            GridType = p.ToGrid,
                            Slot = p.ToSlot,
                            Count = 0L
                        },
                        Success = true
                    });
                    Connection.ReceiveChat("PlayerObject.GemOrbBroken".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
                    array2[p.ToSlot] = null;
                    RemoveItem(targetItem);
                    targetItem.Delete();
                    itemBroke = true;
                }
                RefreshStats();
            }
            else
            {
                Connection.ReceiveChat("PlayerObject.GemOrbFailure".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
            }

            // 镶嵌的py事件
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, targetItem, gemInfo, attachSuccess, itemBroke });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnAttachGem", args);
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

        public void AddHole(C.AddHole p) //打孔
        {
            if (Dead || (p.FromGrid == p.ToGrid && p.FromSlot == p.ToSlot))
            {
                return;
            }
            UserItem[] array;
            switch (p.FromGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array = Inventory;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer in Connection.Observers)
                        {
                            observer.ReceiveChat("Items.StorageSafeZone".Lang(observer.Language), MessageType.System);
                        }
                        return;
                    }
                    array = Storage;
                    if (p.FromSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null)
                    {
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if (!InSafeZone && (p.ToGrid != GridType.Storage || p.ToGrid != GridType.PatchGrid))
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    array = Character.Account.GuildMember.Guild.Storage;
                    if (p.FromSlot >= Character.Account.GuildMember.Guild.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array = Companion.Inventory;
                    if (p.FromSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.FromSlot < 0 || p.FromSlot >= array.Length)
            {
                return;
            }

            UserItem drill = array[p.FromSlot];//打孔道具
            if (drill == null || (drill.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage || drill.Info.Effect != ItemEffect.DrillAddHole)
            {
                return;
            }

            UserItem[] array2;
            switch (p.ToGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array2 = Inventory;
                    break;
                case GridType.Equipment:
                    array2 = Equipment;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer2 in Connection.Observers)
                        {
                            observer2.ReceiveChat("Items.StorageSafeZone".Lang(observer2.Language), MessageType.System);
                        }
                        return;
                    }
                    array2 = Storage;
                    if (p.ToSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.GuildStorage:
                    if (Character.Account.GuildMember == null)
                    {
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStoragePermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if (!InSafeZone && p.FromGrid != GridType.Storage)
                    {
                        Connection.ReceiveChat("Items.GuildStorageSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    array2 = Character.Account.GuildMember.Guild.Storage;
                    if (p.ToSlot >= Character.Account.GuildMember.Guild.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array2 = Companion.Inventory;
                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.ToSlot < 0 || p.ToSlot >= array2.Length)
            {
                return;
            }
            UserItem targetItem = array2[p.ToSlot];//目标物品
            if (targetItem == null
                || (targetItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage
                || targetItem.Info.Effect == ItemEffect.NoGem
                || !Globals.AllowedGemItemTypes.Contains(targetItem.Info.ItemType))
            {
                Connection.ReceiveChat("该物品不支持打孔".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (drill.Info.Shape == 0 && targetItem.Info.Rarity != Rarity.Common                //打孔石Shape为1 如果装备不是普通的
                || drill.Info.Shape == 1 && targetItem.Info.Rarity != Rarity.Superior           //打孔石Shape为2 如果装备不是高级的
                || drill.Info.Shape == 2 && targetItem.Info.Rarity != Rarity.Elite              //打孔石Shape为3 如果装备不是稀世的
                || drill.Info.Shape == 3 && targetItem.Info.Rarity == Rarity.Common)            //打孔石Shape为4 如果装备是普通的
            {
                Connection.ReceiveChat("物品类型不匹配".Lang(Connection.Language), MessageType.System);
                return;
            }

            //最大镶嵌数目
            if ((drill.Info.Shape == 0 && targetItem.Info.Rarity == Rarity.Common && targetItem.Stats[Stat.UsedGemSlot] + targetItem.Stats[Stat.EmptyGemSlot] >= 1)
                || (drill.Info.Shape == 1 && targetItem.Info.Rarity == Rarity.Superior && targetItem.Stats[Stat.UsedGemSlot] + targetItem.Stats[Stat.EmptyGemSlot] >= 1)
                || (drill.Info.Shape == 2 && targetItem.Info.Rarity == Rarity.Elite && targetItem.Stats[Stat.UsedGemSlot] + targetItem.Stats[Stat.EmptyGemSlot] >= 1)
                || (drill.Info.Shape == 3 && targetItem.Info.Rarity != Rarity.Common && targetItem.Stats[Stat.UsedGemSlot] + targetItem.Stats[Stat.EmptyGemSlot] >= Config.MaxGemPerItem))
            {
                Connection.ReceiveChat("PlayerObject.SlotMaxGemPerItem".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
                return;
            }

            S.ItemStatsChanged itemStatsChanged = new S.ItemStatsChanged
            {
                GridType = p.ToGrid,
                Slot = p.ToSlot,
                NewStats = new Stats()
            };
            targetItem.AddStat(Stat.EmptyGemSlot, 1, StatSource.None);//插孔数目+1
            itemStatsChanged.NewStats[Stat.EmptyGemSlot] = 1;
            itemStatsChanged.FullItemStats = targetItem.ToClientInfo().FullItemStats;
            targetItem.StatsChanged();

            Enqueue(itemStatsChanged);

            Connection.ReceiveChat("PlayerObject.EmptyGemSlotSuccess".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);


            drill.Count--;
            if (drill.Count < 0)
            {
                drill.Count = 0L;
            }
            GridType fromGrid = p.FromGrid;
            if (fromGrid == GridType.GuildStorage)
            {
                Packet p2 = new S.ItemChanged
                {
                    Link = new CellLinkInfo
                    {
                        GridType = p.FromGrid,
                        Slot = p.FromSlot,
                        Count = drill.Count
                    },
                    Success = true
                };
                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    PlayerObject playerObject = member.Account.Connection?.Player;
                    if (playerObject != null && playerObject != this)
                    {
                        playerObject.Enqueue(p2);
                    }
                }
            }
            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo
                {
                    GridType = p.FromGrid,
                    Slot = p.FromSlot,
                    Count = drill.Count
                },
                Success = true
            });
            if (drill.Count <= 0)
            {
                array[drill.Slot] = null;
                RemoveItem(drill);
                drill.Delete();
            }

        }

        public void RemoveAllGems(C.RemoveGem p) //取下所有宝石
        {
            if (Dead || (p.FromGrid == p.ToGrid && p.FromSlot == p.ToSlot))
            {
                return;
            }
            UserItem[] array;
            switch (p.FromGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array = Inventory;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer in Connection.Observers)
                        {
                            observer.ReceiveChat("Items.StorageSafeZone".Lang(observer.Language), MessageType.System);
                        }
                        return;
                    }
                    array = Storage;
                    if (p.FromSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array = Companion.Inventory;
                    if (p.FromSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.FromSlot < 0 || p.FromSlot >= array.Length)
            {
                return;
            }

            UserItem drill = array[p.FromSlot];//取宝石道具
            if (drill == null || (drill.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage || drill.Info.Effect != ItemEffect.DrillRemoveGem)
            {
                return;
            }

            UserItem[] array2;
            switch (p.ToGrid)
            {
                default:
                    return;
                case GridType.Inventory:
                    array2 = Inventory;
                    break;
                case GridType.Equipment:
                    array2 = Equipment;
                    break;
                case GridType.Storage:
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("Items.StorageSafeZone".Lang(Connection.Language), MessageType.System);
                        foreach (SConnection observer2 in Connection.Observers)
                        {
                            observer2.ReceiveChat("Items.StorageSafeZone".Lang(observer2.Language), MessageType.System);
                        }
                        return;
                    }
                    array2 = Storage;
                    if (p.ToSlot >= Character.Account.StorageSize)
                    {
                        return;
                    }
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null)
                    {
                        return;
                    }
                    array2 = Companion.Inventory;
                    if (p.ToSlot >= Companion.Stats[Stat.CompanionInventory])
                    {
                        return;
                    }
                    break;
            }
            if (p.ToSlot < 0 || p.ToSlot >= array2.Length)
            {
                return;
            }
            UserItem targetItem = array2[p.ToSlot];//目标物品
            if (targetItem.Stats[Stat.UsedGemSlot] <= 0) //最大镶嵌数目
            {
                Connection.ReceiveChat("PlayerObject.UsedGemSlot".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);
                return;
            }
            if ((targetItem.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage || targetItem.Info.Effect == ItemEffect.NoGem || !Globals.AllowedGemItemTypes.Contains(targetItem.Info.ItemType))
            {
                Connection.ReceiveChat("该物品不支持取下宝石", MessageType.System);
                return;
            }

            if (RemainingSlots(GridType.Inventory) < Globals.GemList.Count)
            {
                Connection.ReceiveChat("PlayerObject.UsedInventory".Lang(Connection.Language, Globals.GemList.Count), MessageType.System);
                return;
            }

            // Gem1: Gem1 Stats
            Dictionary<StatSource, List<UserItemStat>> gemDict = new Dictionary<StatSource, List<UserItemStat>>();

            S.ItemStatsChanged itemStatsChanged = new S.ItemStatsChanged
            {
                GridType = p.ToGrid,
                Slot = p.ToSlot,
                NewStats = new Stats()
            };

            foreach (UserItemStat gemStat in targetItem.AddedStats)
            {
                if (!Globals.GemList.Contains(gemStat.StatSource))
                    continue;
                if (!gemDict.ContainsKey(gemStat.StatSource))
                {
                    gemDict.Add(gemStat.StatSource, new List<UserItemStat>());
                }
                gemDict[gemStat.StatSource].Add(gemStat);
            }

            List<UserItem> gems = new List<UserItem>();

            foreach (KeyValuePair<StatSource, List<UserItemStat>> kvp in gemDict)
            {
                string gemName = kvp.Value[0].SourceName;
                if (gemName.Length < 1) continue;

                UserItem newGem = CreatePlainItem(gemName);
                foreach (UserItemStat newStat in kvp.Value)
                {
                    targetItem.RemoveStat(newStat.Stat, newStat.StatSource);
                    newGem.AddStat(newStat.Stat, newStat.Amount, StatSource.None);
                }
                GainItem(newGem);
                gems.Add(newGem);
            }

            targetItem.AddStat(Stat.EmptyGemSlot, gemDict.Count, StatSource.None);
            targetItem.AddStat(Stat.UsedGemSlot, -gemDict.Count, StatSource.None);

            itemStatsChanged.NewStats[Stat.EmptyGemSlot] = gemDict.Count;
            itemStatsChanged.NewStats[Stat.UsedGemSlot] = gemDict.Count;
            itemStatsChanged.FullItemStats = targetItem.ToClientInfo().FullItemStats;
            targetItem.StatsChanged();

            Enqueue(itemStatsChanged);
            //修复客户端绘制已消失属性的bug
            Enqueue(new S.ItemStatsRefreshed
            {
                GridType = p.ToGrid,
                Slot = p.ToSlot,
                NewStats = targetItem.Stats,
                FullItemStats = targetItem.ToClientInfo().FullItemStats,

            });

            Connection.ReceiveChat("PlayerObject.GemSlot".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);


            drill.Count--;
            if (drill.Count < 0)
            {
                drill.Count = 0L;
            }
            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo
                {
                    GridType = p.FromGrid,
                    Slot = p.FromSlot,
                    Count = drill.Count
                },
                Success = true
            });
            if (drill.Count <= 0)
            {
                array[drill.Slot] = null;
                RemoveItem(drill);
                drill.Delete();
            }

            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, targetItem, gems });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnRemoveGems", args);
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

    }
}
