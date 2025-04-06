using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{

    public partial class PlayerObject : MapObject
    {
        public void AutiionsFlash()
        {

            Connection.AutionSearchResults.Clear();
            Connection.AutionVisibleResults.Clear();
            Connection.AutionSearchResults = SEnvir.NewAutionInfoList.Where(x => !x.Closed).ToList();
            int count = 0;
            foreach (var info in Connection.AutionSearchResults)
            {
                if (count >= 13) break;
                Connection.AutionVisibleResults.Add(info);
                count++;
            }
            Enqueue(new S.NewAuctionFlash { NewAuctionsList = Connection.AutionSearchResults.Where(x => true).Select(x => x.ToClientInfo()).Take(13).ToList() });
        }

        public void AutiionsGetIndex(C.AutionsFlashIndex p)
        {


        }
        public void AutionsAdd(C.AutionsAdd p)
        {
            if (DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("21:59")) > 0) return;
            if ((p.BuyItNowPrice > 0 && p.BuyItNowPrice < p.Price) || p.BuyItNowPrice < 0 || p.Price < 0 || p.PerAddPrice < 100) return;

            UserItem item = null;
            int slot = 0;
            // 寻找入场劵
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.ItemName != "拍卖入场卷") continue;

                item = Inventory[i];
                slot = i;
                break;
            }
            // 找不到入场劵
            if (item == null)
            {
                Connection.ReceiveChat("你没有拍卖入场卷，不能参与拍卖。", MessageType.System);
                return;
            }
            // 把券用掉
            S.ItemChanged changed = new S.ItemChanged
            {
                Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = slot },

                Success = true
            };
            Enqueue(changed);
            if (item.Count > 1)
            {
                item.Count--;
                changed.Link.Count = item.Count;
            }
            else
            {
                RemoveItem(item);
                Inventory[slot] = null;
                item.Delete();
            }
            // 刷新包裹
            RefreshWeight();

            S.ItemChanged result = new S.ItemChanged
            {
                Link = p.Link,
                Success = true
            };
            Enqueue(result);

            if (!ParseLinks(p.Link)) return;

            //if (p.Message.Length > 150) return;

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
            item = array[p.Link.Slot];

            if (item == null || p.Link.Count > item.Count) return; //trying to sell more than owned.

            if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;
            if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
            if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (p.Price / 100 <= 0 || p.BuyItNowPrice / 100 < 0 || p.PerAddPrice / 100 <= 0) return; // 买断少于1

            int cost = 0;//(int) Math.Min(int.MaxValue, p.Price*Globals.MarketPlaceTax*p.Link.Count + Globals.MarketPlaceFee);

            if (Character.Account.Auctions.Count >= Character.Account.HightestLevel() * 3 + Character.Account.StorageSize - Globals.StorageSize)  //仓库物品达到最大寄售数量
            {
                Connection.ReceiveChat("MarketPlace.ConsignLimit".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (cost > Gold)
            {
                Connection.ReceiveChat("MarketPlace.ConsignCost".Lang(Connection.Language), MessageType.System);
                return;
            }
            Gold -= cost;
            GoldChanged();

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
            auctionItem.Flags |= UserItemFlags.AutionItem;
            NewAutionInfo auction = SEnvir.NewAutionInfoList.CreateNewObject();

            auction.Account = Character.Account;

            auction.Price = p.Price;
            auction.PriceType = CurrencyType.GameGold;  //价格类型
            auction.Item = auctionItem;
            auction.Character = Character;
            auction.BuyItNowPrice = p.BuyItNowPrice;
            auction.PriceAdd = p.PerAddPrice;

            //SEnvir.NewAutionInfoTempList.Add(auction);

            //result.Success = true;

            //Enqueue(new S.MarketPlaceConsign { Consignments = new List<ClientMarketPlaceInfo> { auction.ToClientInfo(Character.Account) }, ObserverPacket = false });
            //Connection.ReceiveChat("MarketPlace.ConsignComplete".Lang(Connection.Language), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("商品上架成功".Lang(con.Language), MessageType.System);
            foreach (SConnection con in SEnvir.Connections)
            {
                con.Enqueue(new S.ChatItem { Item = item.ToClientInfo() });
                con.ReceiveChat("玩家 {0} 在拍卖行上架 <{1}/{2}> ，需要的点击拍卖行竞拍。".Lang(con.Language, Character.CharacterName, item.Info.ItemName, item.Index), MessageType.System);
                con.ReceiveChat("玩家 {0} 在拍卖行上架 <{1}/{2}> ，需要的点击拍卖行竞拍。".Lang(con.Language, Character.CharacterName, item.Info.ItemName, item.Index), MessageType.System);
                con.ReceiveChat("玩家 {0} 在拍卖行上架 <{1}/{2}> ，需要的点击拍卖行竞拍。".Lang(con.Language, Character.CharacterName, item.Info.ItemName, item.Index), MessageType.System);
                con.ReceiveChat("玩家 {0} 在拍卖行上架 <{1}/{2}> ，需要的点击拍卖行竞拍。".Lang(con.Language, Character.CharacterName, item.Info.ItemName, item.Index), MessageType.System);
                con.ReceiveChat("玩家 {0} 在拍卖行上架 <{1}/{2}> ，需要的点击拍卖行竞拍。".Lang(con.Language, Character.CharacterName, item.Info.ItemName, item.Index), MessageType.System);
            }

            AutiionsFlash();
        }
        public void AutionsBuy(C.AutinsBuy p)
        {
            if (DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("22:59")) > 0) return;
            var auction = SEnvir.NewAutionInfoList.FirstOrDefault(x => x.Index == p.Index);
            if (auction == null || p.BuyPrice <= 0) return;
            //if(auction.LastCharacter == Character) return;
            if (auction.Account == Character.Account) return;
            if (auction.Item == null) return;
            if (auction.LastPrice + auction.PriceAdd > p.BuyPrice && p.BuyPrice < auction.Price) return;
            if (p.BuyPrice > GameGold)
            {
                //TODO 赞助币不够
                return;
            }
            if (auction.Closed)
            {
                //TODO拍卖已经完成
                return;
            }
            if (auction.LastCharacter != null)
            {
                //退还押金
                var player = SEnvir.GetPlayerByCharacter(auction.LastCharacter.CharacterName);
                if (player != null)
                {
                    player.ChangeGameGold((int)auction.LastPrice, "拍卖失败退还");
                    player.Connection.ReceiveChat("竞价被超过，请查看拍卖行", MessageType.System);
                }
                else
                {
                    auction.LastCharacter.Account.GameGold += (int)auction.LastPrice;
                }
                //TODO 是否邮件告知？
            }
            if (auction.BuyItNowPrice > 0 && auction.BuyItNowPrice <= p.BuyPrice)
            {
                //TODO一口价完成 给物品 扣赞助币
                auction.Closed = true;
                UserItem item = auction.Item;
                auction.Item = null;
                long cost = p.BuyPrice;
                ChangeGameGold(-(int)p.BuyPrice, "拍卖");
                // 邮件交易
                MailInfo mail = SEnvir.MailInfoList.CreateNewObject();
                mail.Account = auction.Account;

                long tax = (long)(cost * Config.NewAuctionTax);//税率

                mail.Subject = "拍卖清单".Lang(Connection.Language);
                mail.Sender = "拍卖".Lang(Connection.Language);

                ItemInfo itemInfo = item.Info;
                int partIndex = item.Stats[Stat.ItemIndex];

                string itemName;

                if (item.Info.Effect == ItemEffect.ItemPart)
                    itemName = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex).ItemName + " - [" + "碎片".Lang(Connection.Language) + "]";
                else
                    itemName = item.Info.ItemName;
                string pricename;

                UserItem gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                gold.Count = (long)cost; // - tax);
                pricename = "赞助币".Lang(Connection.Language);
                gold.Mail = mail;
                gold.Slot = 0;
                mail.Message = $"你成功拍卖出一件商品".Lang(Connection.Language) + "\n\n" +
                               string.Format("买家".Lang(Connection.Language) + ": {0}\n", Name) +
                               string.Format("商品".Lang(Connection.Language) + ": {0} x{1}\n", itemName, 1) +
                               string.Format("{1}: {0:#,##0} " + "单个".Lang(Connection.Language) + "\n", p.BuyPrice / 100, pricename) +
                               string.Format("小计".Lang(Connection.Language) + ": {0:#,##0}\n\n", cost / 100) +
                               //string.Format("税收".Lang(Connection.Language) + ": {0:#,##0} ({1:p0})\n\n", tax / 100, Config.NewAuctionTax) +
                               string.Format("合计".Lang(Connection.Language) + ": {0:#,##0}", cost / 100);

                mail.HasItem = true;
                if (auction.Account.Connection?.Player != null)
                    auction.Account.Connection.Enqueue(new S.MailNew
                    {
                        Mail = mail.ToClientInfo(),
                        ObserverPacket = false,
                    });

                item.Flags |= UserItemFlags.None;
                item.Flags &= ~UserItemFlags.AutionItem;
                if (!InSafeZone || !CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
                {
                    mail = SEnvir.MailInfoList.CreateNewObject();

                    mail.Account = Character.Account;

                    mail.Subject = "拍卖商品".Lang(Connection.Language);
                    mail.Sender = "拍卖".Lang(Connection.Language);
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

                foreach (SConnection con in SEnvir.Connections)
                {
                    con.Enqueue(new S.ChatItem { Item = item.ToClientInfo() });
                    con.ReceiveChat("恭喜 {0} 花费 {1} 直接获得 <{2}/{3}> 。".Lang(con.Language, Character.CharacterName, p.BuyPrice / 100, item.Info.ItemName, item.Index), MessageType.System);
                }
                return;
            }
            auction.LastPrice = p.BuyPrice;
            auction.LastCharacter = Character;
            ChangeGameGold(-(int)(p.BuyPrice), "拍卖暂扣");
            AutiionsFlash();

        }
        public static void NewAuctionProcess()
        {
            //到时间清空拍卖列表 公布拍卖结果
            if (DateTime.Compare(SEnvir.Now, Convert.ToDateTime("22:59")) < 0)
            {
                var auctionList = SEnvir.NewAutionInfoList.Where(x => !x.Closed).ToList();
                foreach (var auction in auctionList)
                {
                    auction.Closed = true;
                    if (auction.Closed || auction.Item == null || auction.LastCharacter == null) continue;
                    //TODO一口价完成 给物品 扣赞助币

                    UserItem item = auction.Item;
                    auction.Item = null;
                    long cost = auction.LastPrice;
                    // 邮件交易
                    MailInfo mail = SEnvir.MailInfoList.CreateNewObject();
                    mail.Account = auction.Account;

                    long tax = (long)(cost * Config.NewAuctionTax);//税率

                    mail.Subject = "拍卖清单";
                    mail.Sender = "拍卖";

                    ItemInfo itemInfo = item.Info;
                    int partIndex = item.Stats[Stat.ItemIndex];

                    string itemName;

                    if (item.Info.Effect == ItemEffect.ItemPart)
                        itemName = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex).ItemName + " - [" + "碎片" + "]";
                    else
                        itemName = item.Info.ItemName;
                    string pricename;
                    var Name = auction.LastCharacter.CharacterName;
                    UserItem gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                    gold.Count = (long)(cost);// - tax);
                    pricename = "赞助币";
                    gold.Mail = mail;
                    gold.Slot = 0;
                    mail.Message = $"你成功拍卖出一件商品\n\n" +
                                   string.Format("买家: {0}\n", Name) +
                                   string.Format("商品: {0} x{1}\n", itemName, 1) +
                                   string.Format("{1}: {0:#,##0} 单个\n", auction.LastPrice / 100, pricename) +
                                   string.Format("小计: {0:#,##0}\n\n", cost / 100) +
                                   //string.Format("税收: {0:#,##0} ({1:p0})\n\n", tax / 100, Config.NewAuctionTax) +
                                   string.Format("合计: {0:#,##0}", cost / 100);

                    mail.HasItem = true;
                    if (auction.Account.Connection?.Player != null)
                        auction.Account.Connection.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });

                    item.Flags |= UserItemFlags.None;
                    item.Flags &= ~UserItemFlags.AutionItem;
                    var player = SEnvir.GetPlayerByCharacter(auction.LastCharacter.CharacterName);

                    if (!(player?.InSafeZone ?? false) || !(player?.CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)) ?? false))
                    {
                        mail = SEnvir.MailInfoList.CreateNewObject();

                        mail.Account = auction.LastCharacter.Account;

                        mail.Subject = "拍卖商品";
                        mail.Sender = "拍卖";
                        mail.Message = string.Format("{}x{}", itemName, item.Count);

                        item.Mail = mail;
                        item.Slot = 0;
                        mail.HasItem = true;

                        player?.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });
                    }
                    else
                    {
                        player.GainItem(item);
                    }

                    foreach (SConnection con in SEnvir.Connections)
                    {
                        con.Enqueue(new S.ChatItem { Item = item.ToClientInfo() });
                        con.ReceiveChat("恭喜 {0} 花费 {1} 竞拍获得 <{2}/{3}> 。".Lang(con.Language, auction.LastCharacter.CharacterName, auction.LastPrice / 100, item.Info.ItemName, item.Index), MessageType.System);
                    }
                }
            }
            SEnvir.NewAutionInfoTempList.Clear();
        }
    }
}


