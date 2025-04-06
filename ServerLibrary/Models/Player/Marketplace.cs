using Library;
using Library.SystemModels;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject // 寄售
    {
        #region MarketPlace
        //商城
        public void MarketPlaceConsign(C.MarketPlaceConsign p)  //寄售
        {
            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link,
            };
            Enqueue(result);

            if (!ParseLinks(p.Link)) return;

            if (p.Message.Length > 150) return;

            UserItem[] array;
            switch (p.Link.GridType)
            {
                case GridType.Inventory: //背包
                    array = Inventory;
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    break;
                case GridType.Storage:  //仓库
                    array = Storage;
                    break;
                case GridType.PatchGrid:  //碎片包裹
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignSafeZone".Lang(Connection.Language), MessageType.System);//判断如果不是在安全区，那么提示语
                        return;
                    }
                    array = PatchGrid;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;

                    array = Companion.Inventory;
                    if (!InSafeZone && !Character.Account.TempAdmin)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignSafeZone".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    break;
                default:
                    return;
            }

            if (p.Link.Slot < 0 || p.Link.Slot >= array.Length) return;
            UserItem item = array[p.Link.Slot];

            if (item == null || p.Link.Count > item.Count) return; //trying to sell more than owned.

            if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;
            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
            if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;


            if (p.Price <= 5000 && p.PriceType == CurrencyType.Gold)
            {
                Connection.ReceiveChat("售价不能低于5000金币，无法上架。".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.Price <= 0 && p.PriceType == CurrencyType.GameGold) return;

            //寄售上架费用
            int cost = 5000; //(int) Math.Min(int.MaxValue, p.Price*Globals.MarketPlaceTax*p.Link.Count + Globals.MarketPlaceFee);

            if (Character.Account.Auctions.Count >= Character.Account.HightestLevel() * 3 + Character.Account.StorageSize - Globals.StorageSize)  //仓库物品达到最大寄售数量
            {
                Connection.ReceiveChat("MarketPlace.ConsignLimit".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat("MarketPlace.ConsignGuildFundsGuild".Lang(Connection.Language), MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMarket) != GuildPermission.FundsMarket)
                {
                    Connection.ReceiveChat("MarketPlace.ConsignGuildFundsPermission".Lang(Connection.Language), MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat("MarketPlace.ConsignGuildFundsCost".Lang(Connection.Language), MessageType.System);
                    return;
                }

                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat("MarketPlace.ConsignGuildFundsUsed".Lang(Connection.Language, Name, cost, item.Info.ItemName, result.Link.Count, p.Price), MessageType.System);
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat("MarketPlace.ConsignCost".Lang(Connection.Language), MessageType.System);
                    return;
                }

                Gold -= cost;
                GoldChanged();

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "寄售系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = cost,
                    ExtraInfo = $"扣除寄售上架费用"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            UserItem auctionItem;

            if (p.Link.Count == item.Count)
            {
                auctionItem = item;
                RemoveItem(item);
                array[p.Link.Slot] = null;

                result.Link.Count = 0;
            }
            else
            {
                auctionItem = SEnvir.CreateFreshItem(item);

                //记录物品来源
                SEnvir.RecordTrackingInfo(item, auctionItem);

                auctionItem.Count = p.Link.Count;
                item.Count -= p.Link.Count;

                result.Link.Count = item.Count;
            }

            RefreshWeight();
            Companion?.RefreshWeight();

            AuctionInfo auction = SEnvir.AuctionInfoList.CreateNewObject();

            auction.Account = Character.Account;

            auction.Price = p.Price;
            auction.PriceType = p.PriceType;  //价格类型
            auction.Item = auctionItem;
            auction.Character = Character;
            auction.Message = p.Message;

            result.Success = true;

            Enqueue(new S.MarketPlaceConsign { Consignments = new List<ClientMarketPlaceInfo> { auction.ToClientInfo(Character.Account) }, ObserverPacket = false });
            Connection.ReceiveChat("MarketPlace.ConsignComplete".Lang(Connection.Language), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("MarketPlace.ConsignComplete".Lang(con.Language), MessageType.System);
        }
        public void MarketPlaceCancelConsign(C.MarketPlaceCancelConsign p)  //取消寄售
        {
            if (p.Count <= 0) return;

            AuctionInfo info = Character.Account.Auctions?.FirstOrDefault(x => x.Index == p.Index);

            if (info == null)
            {
                Connection.ReceiveChat("MarketPlace.ConsignInfoNull".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info.Item == null)
            {
                Connection.ReceiveChat("MarketPlace.ConsignAlreadySold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info.Item.Count < p.Count)
            {
                Connection.ReceiveChat("MarketPlace.ConsignNotEnough".Lang(Connection.Language), MessageType.System);
                return;
            }

            UserItem item = info.Item;

            if (info.Item.Count > p.Count)
            {
                info.Item.Count -= p.Count;

                UserItem newItem = SEnvir.CreateFreshItem(info.Item);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, newItem);
                item = newItem;

                item.Count = p.Count;
            }
            else
                info.Item = null;

            if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = Character.Account;
                mail.Subject = "取消寄售".Lang(Connection.Language);
                mail.Message = string.Format("PlayerObject.CancelMarketPlace".Lang(Connection.Language, item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count));
                mail.Sender = "寄售".Lang(Connection.Language);
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

            if (info.Item == null)
                info.Delete();

            Enqueue(new S.MarketPlaceConsignChanged { Index = info.Index, Count = info.Item?.Count ?? 0, ObserverPacket = false, });
        }

        public void MarketPlaceBuy(C.MarketPlaceBuy p) //寄售购买
        {
            if (p.Count <= 0) return;

            S.MarketPlaceBuy result = new S.MarketPlaceBuy
            {
                ObserverPacket = false,
            };

            Enqueue(result);

            AuctionInfo info = Connection.MPSearchResults.FirstOrDefault(x => x.Index == p.Index);
            if (info == null) return;

            p.Count = info.Item.Count;

            if (info.Item == null)
            {
                Connection.ReceiveChat("MarketPlace.ConsignAlreadySold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info.Account == Character.Account && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat("MarketPlace.ConsignBuyOwnItem".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info.Item.Count < p.Count)
            {
                Connection.ReceiveChat("MarketPlace.ConsignNotEnough".Lang(Connection.Language), MessageType.System);
                return;
            }


            long cost = p.Count;

            cost = info.Price;
            //  如果需要元宝购买
            if (info.PriceType == CurrencyType.GameGold)
            {
                if (cost > Character.Account.GameGold)
                {
                    Connection.ReceiveChat("MarketPlace.StoreCost".Lang(Connection.Language), MessageType.System);
                    return;
                }

                Character.Account.GameGold -= (int)cost;
                Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold, ObserverPacket = false });

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "寄售系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.GameGold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.MarketplaceDeduct,
                    Amount = cost,
                    ExtraInfo = $"道具Index: {info.Item.Index}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }
            else
            {
                if (p.GuildFunds)
                {
                    if (Character.Account.GuildMember == null)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignBuyGuildFundsGuild".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMarket) != GuildPermission.FundsMarket)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignBuyGuildFundsPermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignBuyGuildFundsCost".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    Character.Account.GuildMember.Guild.GuildFunds -= cost;
                    Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                        member.Account.Connection?.ReceiveChat("MarketPlace.ConsignBuyGuildFundsUsed".Lang(member.Account.Connection.Language, Name, cost, info.Item.Info.ItemName, p.Count, info.Price), MessageType.System);
                    }
                }
                else
                {
                    if (cost > Gold)
                    {
                        Connection.ReceiveChat("MarketPlace.ConsignBuyCost".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    Gold -= cost;
                    GoldChanged();

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "寄售系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.MarketplaceDeduct,
                        Amount = cost,
                        ExtraInfo = $"道具名: {info.Item.Index}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
            }

            UserItem item = info.Item;

            if (info.Item.Count > p.Count)
            {
                info.Item.Count -= p.Count;

                UserItem newItem = SEnvir.CreateFreshItem(info.Item);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, newItem);
                item = newItem;

                item.Count = p.Count;
            }
            else
                info.Item = null;

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            mail.Account = info.Account;

            long tax = (long)(cost * Config.MarketPlaceTax);  //税率 

            mail.Subject = "寄售清单".Lang(Connection.Language);
            mail.Sender = "寄售".Lang(Connection.Language);

            ItemInfo itemInfo = item.Info;
            int partIndex = item.Stats[Stat.ItemIndex];

            string itemName;

            if (item.Info.Effect == ItemEffect.ItemPart)
                itemName = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex).ItemName + " - [" + "碎片".Lang(Connection.Language) + "]";
            else
                itemName = item.Info.ItemName;
            string pricename;
            if (info.PriceType == CurrencyType.GameGold)
            {
                UserItem gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                gold.Count = (long)(cost); //去掉了寄售税率
                pricename = "赞助币".Lang(Connection.Language);
                gold.Mail = mail;
                gold.Slot = 0;
                mail.Message = $"你成功出售一件商品".Lang(Connection.Language) + "\n\n" +
                          string.Format("买家".Lang(Connection.Language) + ": {0}\n", Name) +
                          string.Format("商品".Lang(Connection.Language) + ": {0} x{1}\n", itemName, p.Count) +
                          string.Format("{1}: {0:#,##0} " + "单个".Lang(Connection.Language) + "\n", info.Price / 100, pricename) +
                          string.Format("小计".Lang(Connection.Language) + ": {0:#,##0}\n\n", cost / 100) +
                          string.Format("合计".Lang(Connection.Language) + ": {0:#,##0}", cost / 100);
            }
            else
            {
                UserItem gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                gold.Count = (long)(cost - tax);
                pricename = "金币".Lang(Connection.Language);
                gold.Mail = mail;
                gold.Slot = 0;
                mail.Message = $"你成功出售一件商品".Lang(Connection.Language) + "\n\n" +
                          string.Format("买家".Lang(Connection.Language) + ": {0}\n", Name) +
                          string.Format("商品".Lang(Connection.Language) + ": {0} x{1}\n", itemName, p.Count) +
                          string.Format("{1}: {0:#,##0} " + "单个".Lang(Connection.Language) + "\n", info.Price, pricename) +
                          string.Format("小计".Lang(Connection.Language) + ": {0:#,##0}\n\n", cost) +
                          string.Format("税收".Lang(Connection.Language) + ": {0:#,##0} ({1:p0})\n\n", tax, Config.MarketPlaceTax) +
                          string.Format("合计".Lang(Connection.Language) + ": {0:#,##0}", cost - tax);
            }

            mail.HasItem = true;

            if (info.Account.Connection?.Player != null)
                info.Account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });


            item.Flags |= UserItemFlags.None;

            if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = Character.Account;

                mail.Subject = "购买商品".Lang(Connection.Language);
                mail.Sender = "寄售".Lang(Connection.Language);
                mail.Message = string.Format("PlayerObject.BuyGoods".Lang(Connection.Language, itemName, item.Count == 1 ? "" : "x" + item.Count));

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

            result.Index = info.Index;
            result.Count = info.Item?.Count ?? 0;
            result.Success = true;

            AuctionHistoryInfo history = SEnvir.AuctionHistoryInfoList.Binding.FirstOrDefault(x => x.Info == itemInfo.Index && x.PartIndex == partIndex) ?? SEnvir.AuctionHistoryInfoList.CreateNewObject();

            history.Info = itemInfo.Index;
            history.PartIndex = partIndex;
            history.SaleCount += p.Count;
            if (info.PriceType == CurrencyType.GameGold)
            {
                history.LastGameGoldPrice = info.Price;
                for (int i = history.GameGoldAverage.Length - 2; i >= 0; i--)
                    history.GameGoldAverage[i + 1] = history.GameGoldAverage[i];

                history.GameGoldAverage[0] = info.Price; //Only care about the price per transaction
            }
            else
            {
                history.LastPrice = info.Price;
                for (int i = history.Average.Length - 2; i >= 0; i--)
                    history.Average[i + 1] = history.Average[i];

                history.Average[0] = info.Price; //Only care about the price per transaction
            }


            if (info.Account.Connection?.Player != null)
                info.Account.Connection.Enqueue(new S.MarketPlaceConsignChanged { Index = info.Index, Count = info.Item?.Count ?? 0, ObserverPacket = false, });

            if (info.Item == null)
                info.Delete();
        }

        public void MarketPlaceStoreBuy(C.MarketPlaceStoreBuy p)  //元宝商城购买
        {
            if (p.Count <= 0) return;
            p.UseHuntGold = false;

            S.MarketPlaceStoreBuy result = new S.MarketPlaceStoreBuy
            {
                ObserverPacket = false,
            };

            Enqueue(result);

            StoreInfo info = SEnvir.StoreInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);

            if (info?.Item == null) return;

            if (!info.Available)
            {
                Connection.ReceiveChat("MarketPlace.StoreNotAvailable".Lang(Connection.Language), MessageType.System);
                return;
            }

            //p.Count = Math.Min(p.Count, info.Item.StackSize);

            long cost = p.Count;

            // 如果 角色用赏金购买 且 赏金价格为0      或    角色不用赏金购买 且 元宝价格为0    直接跳出
            if (p.UseHuntGold && info.HuntGoldPrice == 0 || !p.UseHuntGold && info.Price == 0) return;
            // 取对应价格
            int price = p.UseHuntGold ? info.HuntGoldPrice : info.Price;

            cost *= price;

            UserItemFlags flags = UserItemFlags.Worthless;  //flags 定义不能出售
            TimeSpan duration = TimeSpan.FromSeconds(info.Duration);   //duration 定义商城设置道具使用期限

            if (!Config.HuntGoldPrice)
            {
                if (p.UseHuntGold)   //如果 赏金购买  那么获得道具绑定
                    flags |= UserItemFlags.None;
            }
            else
            {
                if (p.UseHuntGold || Character.Account.HightestLevel() < Config.MarketPlaceStoreBuyLevelBound)   //如果 赏金购买 或者 最高级别小于40级  那么获得道具绑定
                    flags |= UserItemFlags.None;
            }

            if (duration != TimeSpan.Zero)  //如果时间使用期限不等0
                flags |= UserItemFlags.Expirable;   //标签定义为时间限制

            flags |= UserItemFlags.None;

            ItemCheck check = new ItemCheck(info.Item, p.Count, flags, duration);

            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat("MarketPlace.StoreNeedSpace".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!Config.TestServer)
            {
                if (p.UseHuntGold)
                {
                    if (cost > Character.Account.HuntGold)
                    {
                        Connection.ReceiveChat("MarketPlace.StoreCost".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    Character.Account.HuntGold -= (int)cost;
                    Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });
                }
                else
                {
                    if (cost > Character.Account.GameGold)
                    {
                        Connection.ReceiveChat("MarketPlace.StoreCost".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    Character.Account.GameGold -= (int)cost;
                    Enqueue(new S.GameGoldChanged { GameGold = Character.Account.GameGold, ObserverPacket = false });

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "元宝商城",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.StoreDeduct,
                        Amount = cost,
                        ExtraInfo = $"道具Index: {info.Item.Index}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
            }
            var count = p.Count;

            long temcount = 0;
            do
            {
                temcount = Math.Min(count, info.Item.StackSize);
                ItemCheck check1 = new ItemCheck(info.Item, temcount, flags, duration);
                UserItem item = SEnvir.CreateFreshItem(check1);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, "商城".Lang(Connection.Language), ObjectType.NPC, "商城购买".Lang(Connection.Language), Character?.CharacterName);
                GainItem(item);
                count -= temcount;
            } while (count != 0);
            GameStoreSale sale = SEnvir.GameStoreSaleList.CreateNewObject();

            sale.Item = info.Item;
            sale.Account = Character.Account;
            sale.Count = p.Count;
            sale.Price = price;
            sale.HuntGold = p.UseHuntGold;
            // 奖金池玩法
            if (Config.EnableRewardPoolMarketBuy && !p.UseHuntGold)
            {
                // 消费金额计入奖金池
                SEnvir.RewardPoolAddBalance(Character, (int)cost / 100, "商城消费");
            }
        }

        public void MarketPlaceCancelSuperior()  //取消寄售
        {
            for (int i = SEnvir.AuctionInfoList.Count - 1; i >= 0; i--)
            {
                AuctionInfo info = SEnvir.AuctionInfoList[i];

                if (info.Item == null) continue;

                //if ((info.Item.Info.ItemType != ItemType.ItemPart) &&
                //(info.Item.Info.RequiredType != RequiredType.Level || info.Item.Info.RequiredAmount < 40 || info.Item.Info.RequiredAmount > 56)) continue;

                UserItem item = info.Item;

                info.Item = null;

                MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                mail.Account = info.Account;
                mail.Subject = "已取消寄售".Lang(Connection.Language);
                mail.Message = "你的购买被取消，因为商品取消寄售".Lang(Connection.Language);
                mail.Sender = "系统信息".Lang(Connection.Language);
                item.Mail = mail;
                item.Slot = 0;
                mail.HasItem = true;

                info.Account.Connection?.Player?.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });

                if (info.Item == null)
                    info.Delete();
            }
        }
        #endregion

        #region 清理过期寄售
        static DateTime _lastDate = DateTime.Now.Date.AddDays(-1);

        /// <summary>
        /// 清理寄售
        /// </summary>
        public static void ClearOverMarket(int days = -7)
        {
            if (_lastDate == DateTime.Now.Date)
            {
                return;
            }
            Func<AuctionInfo, bool> where = (p) =>
             (p.PriceType == CurrencyType.Gold || p.PriceType == CurrencyType.GameGold)
             && p.CreatTime.Date <= DateTime.Now.Date.AddDays(days);

            if (SEnvir.AuctionInfoList.Any(where))
            {
                var list = SEnvir.AuctionInfoList.Where(where).ToArray();
                for (var i = 0; i < list.Length; i++)
                {
                    var info = list[i];
                    if (info.Item == null) continue;

                    //生成新邮件
                    UserItem item = info.Item;
                    MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                    mail.Account = info.Character.Account;
                    mail.Subject = "取消寄售".Lang(Language.SimplifiedChinese);
                    mail.Message = string.Format("PlayerObject.CancelMarketPlace".Lang(Language.SimplifiedChinese, item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count));
                    mail.Sender = "寄售".Lang(Language.SimplifiedChinese);
                    item.Mail = mail;
                    item.Slot = 0;
                    mail.HasItem = true;
                    //通知在线用户
                    for (var j = 0; j < SEnvir.Connections.Count; j++)
                    {
                        if (SEnvir.Connections[j].Account == mail.Account)
                        {
                            mail.Subject = "取消寄售".Lang(SEnvir.Connections[j].Language);
                            mail.Message = string.Format("PlayerObject.CancelMarketPlace".Lang(SEnvir.Connections[j].Language, item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count));
                            mail.Sender = "寄售".Lang(SEnvir.Connections[j].Language);
                            SEnvir.Connections[j].Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });
                            break;
                        }
                    }
                    info.Delete();
                }
            }
            _lastDate = DateTime.Now.Date;
        }
        #endregion       
    }
}
