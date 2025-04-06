using Library;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Utils.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject
    {
        #region Market
        public void GoldMarketTrade(C.GoldMarketTrade p)
        {
            if (p.Gold <= 0 || p.GameGold <= 0) return;
            //Todo 验证数量
            if (p.TradeType == TradeType.Sell)
            {
                if (Gold < p.Gold * 10000 || p.Gold < 10)
                {
                    Connection.ReceiveChat("每单最低10万金币，你的金币不足，无法挂单", MessageType.System);
                    return;
                }
            }
            else if (p.TradeType == TradeType.Buy)
            {
                if (GameGold < p.GameGold * p.Gold)
                {
                    Connection.ReceiveChat("你的赞助币不足，无法挂单", MessageType.System);
                    return;
                }
            }
            else
            {
                //todo 外挂无疑
                return;
            }
            //ChangeGold(-100);
            //List<GoldMarketInfo> goldMarketInfos = SEnvir.GoldMarketInfoList.Binding as List<GoldMarketInfo>;
            List<GoldMarketInfo> temlist = null;
            GoldMarketInfo goldMarketInfo = SEnvir.GoldMarketInfoList.CreateNewObject();
            goldMarketInfo.Character = Character;
            goldMarketInfo.GameGoldPrice = Convert.ToInt32(p.GameGold);
            goldMarketInfo.TradeType = p.TradeType;
            goldMarketInfo.GoldCount = p.Gold;
            goldMarketInfo.TradeState = StockOrderType.Normal;
            goldMarketInfo.CompletedCount = 0;
            MailInfo mail = null;
            UserItem gold = null;
            if (p.TradeType == TradeType.Buy)
            {

                temlist = SEnvir.GoldMarketInfoList.Binding.Where(x => x.GameGoldPrice <= p.GameGold && x.TradeState == StockOrderType.Normal && x.TradeType == TradeType.Sell).OrderBy(x => x.GameGoldPrice).ThenBy(x => x.Date).ToList();

                if (temlist != null)
                {
                    //temlist.Sort((x1, x2) =>
                    //{
                    //if (x1.GameGoldPrice != x2.GameGoldPrice) return x1.GameGoldPrice > x2.GameGoldPrice ? 1 : -1;
                    //return x1.Date > x2.Date ? 1 : -1;
                    //});
                    for (int i = 0; i < temlist.Count; ++i)
                    {

                        //PlayerObject player = null;
                        if (temlist[i].GoldCount - temlist[i].CompletedCount <= goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)
                        {
                            goldMarketInfo.CompletedCount += temlist[i].GoldCount - temlist[i].CompletedCount;
                            //ChangeGold((temlist[i].GoldCount - temlist[i].CompletedCount) * 10000);
                            ChangeGameGold(Convert.ToInt32(-((temlist[i].GoldCount - temlist[i].CompletedCount)) * temlist[i].GameGoldPrice), "金币商城");

                            //给金币给买家
                            // 邮件交易
                            mail = SEnvir.MailInfoList.CreateNewObject();
                            mail.Account = Character.Account;

                            //long tax = (long)(cost * Config.NewAuctionTax);//税率

                            mail.Subject = "成功购得金币".Lang(Connection.Language);
                            mail.Sender = "金币交易行".Lang(Connection.Language);


                            gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                            gold.Count = (temlist[i].GoldCount - temlist[i].CompletedCount) * 10000;
                            gold.Mail = mail;
                            gold.Slot = 0;
                            mail.Message = $"你成功购得金币".Lang(Connection.Language) + "\n\n" +
                                           string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                           string.Format("成交数量".Lang(Connection.Language) + ": {0}万\n", (temlist[i].GoldCount - temlist[i].CompletedCount)) +
                                           string.Format("花费".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32(-((temlist[i].GoldCount - temlist[i].CompletedCount)) * temlist[i].GameGoldPrice)) / 100);

                            mail.HasItem = true;
                            if (Character.Account.Connection?.Player != null)
                                Character.Account.Connection.Enqueue(new S.MailNew
                                {
                                    Mail = mail.ToClientInfo(),
                                    ObserverPacket = false,
                                });

                            //给赞助币给卖家
                            mail = SEnvir.MailInfoList.CreateNewObject();
                            mail.Account = temlist[i].Character.Account;

                            //long tax = (long)(cost * Config.NewAuctionTax);//税率

                            mail.Subject = "成功卖出金币".Lang(Connection.Language);
                            mail.Sender = "金币交易行".Lang(Connection.Language);

                            gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                            gold.Count = Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice); //*0.95
                            gold.Mail = mail;
                            gold.Slot = 0;
                            mail.Message = $"你成功卖出金币".Lang(Connection.Language) + "\n\n" +
                                           string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                           string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (temlist[i].GoldCount - temlist[i].CompletedCount)) +
                                           string.Format("成交获得".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice)) / 100); //*0.95

                            mail.HasItem = true;
                            if (temlist[i].Character.Account.Connection?.Player != null)
                                temlist[i].Character.Account.Connection.Enqueue(new S.MailNew
                                {
                                    Mail = mail.ToClientInfo(),
                                    ObserverPacket = false,
                                });
                            /*
                            player = SEnvir.GetPlayerByCharacter(temlist[i].Character.CharacterName);
                            if (player == null)
                            {
                                temlist[i].Character.Account.GameGold += Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice * 0.95);
                            }
                            else
                            {
                                player.ChangeGameGold(Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice * 0.95));
                            }
                            */
                            //TODO 日志记录元宝变化

                            temlist[i].CompletedCount = temlist[i].GoldCount;
                            temlist[i].TradeState = StockOrderType.Completed;

                            continue;
                        }
                        temlist[i].CompletedCount += goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount;
                        //ChangeGold((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000);
                        ChangeGameGold(Convert.ToInt32(-((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)) * temlist[i].GameGoldPrice), "金币商城");
                        //给金币给买家
                        // 邮件交易
                        mail = SEnvir.MailInfoList.CreateNewObject();
                        mail.Account = Character.Account;

                        //long tax = (long)(cost * Config.NewAuctionTax);//税率

                        mail.Subject = "成功购得金币".Lang(Connection.Language);
                        mail.Sender = "金币交易行".Lang(Connection.Language);


                        gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                        gold.Count = (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000;
                        gold.Mail = mail;
                        gold.Slot = 0;
                        mail.Message = $"你成功购得金币".Lang(Connection.Language) + "\n\n" +
                                       string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                       string.Format("成交数量".Lang(Connection.Language) + ": {0} 万 \n", goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) +
                                       string.Format("花费".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32(((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)) * temlist[i].GameGoldPrice)) / 100);

                        mail.HasItem = true;
                        if (Character.Account.Connection?.Player != null)
                            Character.Account.Connection.Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });

                        //给赞助币给卖家
                        mail = SEnvir.MailInfoList.CreateNewObject();
                        mail.Account = temlist[i].Character.Account;

                        //long tax = (long)(cost * Config.NewAuctionTax);//税率

                        mail.Subject = "成功卖出金币".Lang(Connection.Language);
                        mail.Sender = "金币交易行".Lang(Connection.Language);

                        gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                        gold.Count = Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice);  //*0.95
                        gold.Mail = mail;
                        gold.Slot = 0;
                        mail.Message = $"你成功卖出金币".Lang(Connection.Language) + "\n\n" +
                                       string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                       string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)) +
                                       string.Format("成交获得".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice)) / 100);  //*0.95

                        mail.HasItem = true;
                        if (temlist[i].Character.Account.Connection?.Player != null)
                            temlist[i].Character.Account.Connection.Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });
                        /*
                        player = SEnvir.GetPlayerByCharacter(temlist[i].Character.CharacterName);
                        if (player == null)
                        {
                            temlist[i].Character.Account.GameGold += Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice * 0.95);
                        }
                        else
                        {
                            player.ChangeGameGold(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice * 0.95));
                        }*/
                        //TODO 日志记录元宝变化
                        goldMarketInfo.CompletedCount = goldMarketInfo.GoldCount;

                        break;
                    }
                }
                if (goldMarketInfo.TradeState != StockOrderType.Completed)
                    ChangeGameGold(Convert.ToInt32(-(goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * goldMarketInfo.GameGoldPrice), "金币商城");
            }
            else if (p.TradeType == TradeType.Sell)
            {
                ChangeGold(-(goldMarketInfo.GoldCount) * 10000);
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "金币交易行系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = goldMarketInfo.GoldCount * 10000,
                    ExtraInfo = $"金币交易行预扣交易的金币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
                temlist = SEnvir.GoldMarketInfoList.Binding.Where(x => x.GameGoldPrice >= p.GameGold && x.TradeState == StockOrderType.Normal && x.TradeType == TradeType.Buy).OrderByDescending(x => x.GameGoldPrice).ThenBy(x => x.Date).ToList();

                if (temlist != null)
                {
                    //temlist.Sort((x1, x2) =>
                    //{
                    //if (x1.GameGoldPrice != x2.GameGoldPrice) return x1.GameGoldPrice < x2.GameGoldPrice ? 1 : -1;
                    //return x1.Date > x2.Date ? 1 : -1;
                    //});
                    for (int i = 0; i < temlist.Count; ++i)
                    {
                        //PlayerObject player = null;
                        if (temlist[i].GoldCount - temlist[i].CompletedCount <= goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)
                        {
                            goldMarketInfo.CompletedCount += temlist[i].GoldCount - temlist[i].CompletedCount;

                            //ChangeGameGold(Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice * 0.95));
                            //Todo 完成交易部分 换成邮件
                            //给赞助币卖家
                            // 邮件交易
                            mail = SEnvir.MailInfoList.CreateNewObject();
                            mail.Account = Character.Account;

                            //long tax = (long)(cost * Config.NewAuctionTax);//税率

                            mail.Subject = "成功卖出金币".Lang(Connection.Language);
                            mail.Sender = "金币交易行".Lang(Connection.Language);


                            gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                            gold.Count = Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice);  // * 0.95
                            gold.Mail = mail;
                            gold.Slot = 0;
                            mail.Message = $"你成功卖出金币".Lang(Connection.Language) + "\n\n" +
                                           string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                           string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (temlist[i].GoldCount - temlist[i].CompletedCount)) +
                                           string.Format("成交获得".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice)) / 100);  //* 0.95

                            mail.HasItem = true;
                            if (Character.Account.Connection?.Player != null)
                                Character.Account.Connection.Enqueue(new S.MailNew
                                {
                                    Mail = mail.ToClientInfo(),
                                    ObserverPacket = false,
                                });

                            //给金币给买家
                            mail = SEnvir.MailInfoList.CreateNewObject();
                            mail.Account = temlist[i].Character.Account;

                            //long tax = (long)(cost * Config.NewAuctionTax);//税率

                            mail.Subject = "成功购得金币".Lang(Connection.Language);
                            mail.Sender = "金币交易行".Lang(Connection.Language);

                            gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                            gold.Count = Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * 10000);
                            gold.Mail = mail;
                            gold.Slot = 0;
                            mail.Message = $"你成功购得金币".Lang(Connection.Language) + "\n\n" +
                                           string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                           string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (temlist[i].GoldCount - temlist[i].CompletedCount)) +
                                           string.Format("花费".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((temlist[i].GoldCount - temlist[i].CompletedCount) * temlist[i].GameGoldPrice)) / 100);

                            mail.HasItem = true;
                            if (temlist[i].Character.Account.Connection?.Player != null)
                                temlist[i].Character.Account.Connection.Enqueue(new S.MailNew
                                {
                                    Mail = mail.ToClientInfo(),
                                    ObserverPacket = false,
                                });
                            /*
                             // todo temlist[i].Character 角色在线 及时刷新 如果不在线 保存到角色数据库里 ？
                             player = SEnvir.GetPlayerByCharacter(temlist[i].Character.CharacterName);
                             if (player == null)
                             {
                                 temlist[i].Character.Account.Gold += (temlist[i].GoldCount - temlist[i].CompletedCount) * 10000;
                             }
                             else
                             {
                                 player.ChangeGold((temlist[i].GoldCount - temlist[i].CompletedCount) * 10000);
                             }
                            */
                            temlist[i].CompletedCount = temlist[i].GoldCount;
                            temlist[i].TradeState = StockOrderType.Completed;
                            //TODO 日志记录元宝变化

                            continue;
                        }
                        temlist[i].CompletedCount += goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount;
                        //ChangeGameGold(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice * 0.95));
                        //给赞助币卖家
                        // 邮件交易
                        mail = SEnvir.MailInfoList.CreateNewObject();
                        mail.Account = Character.Account;

                        //long tax = (long)(cost * Config.NewAuctionTax);//税率

                        mail.Subject = "成功卖出金币".Lang(Connection.Language);
                        mail.Sender = "金币交易行".Lang(Connection.Language);


                        gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                        gold.Count = Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice); // * 0.95
                        gold.Mail = mail;
                        gold.Slot = 0;
                        mail.Message = $"你成功卖出金币".Lang(Connection.Language) + "\n\n" +
                                       string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                       string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)) +
                                       string.Format("成交获得".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice)) / 100); // * 0.95

                        mail.HasItem = true;
                        if (Character.Account.Connection?.Player != null)
                            Character.Account.Connection.Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });

                        //给金币给买家
                        mail = SEnvir.MailInfoList.CreateNewObject();
                        mail.Account = temlist[i].Character.Account;

                        //long tax = (long)(cost * Config.NewAuctionTax);//税率

                        mail.Subject = "成功购得金币".Lang(Connection.Language);
                        mail.Sender = "金币交易行".Lang(Connection.Language);

                        gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                        gold.Count = Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000);
                        gold.Mail = mail;
                        gold.Slot = 0;
                        mail.Message = $"你成功购得金币".Lang(Connection.Language) + "\n\n" +
                                       string.Format("成交价格".Lang(Connection.Language) + ": {0:###0.00}\n", Convert.ToDecimal(temlist[i].GameGoldPrice) / 100) +
                                       string.Format("成交数量".Lang(Connection.Language) + ": {0} 万\n", (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount)) +
                                       string.Format("花费".Lang(Connection.Language) + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * temlist[i].GameGoldPrice)) / 100);

                        mail.HasItem = true;
                        if (temlist[i].Character.Account.Connection?.Player != null)
                            temlist[i].Character.Account.Connection.Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });
                        /*
                        player = SEnvir.GetPlayerByCharacter(temlist[i].Character.CharacterName);
                        if (player == null)
                        {
                            temlist[i].Character.Account.Gold += (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000;
                        }
                        else
                        {
                            player.ChangeGold((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000);
                        }*/
                        //TODO 日志记录元宝变化

                        goldMarketInfo.CompletedCount = goldMarketInfo.GoldCount;

                        break;
                    }
                }
            }
            GoldMarketFlash();
        }
        public void GoldMarketCannel(C.GoldMarketCannel p)
        {
            GoldMarketInfo goldMarketInfo = Character.GoldMarketInfos.FirstOrDefault(x => x.Index == p.Index);
            if (goldMarketInfo == null)
            {
                //todo 订单不存在
                return;
            }
            if (goldMarketInfo.TradeState == StockOrderType.Cannel || goldMarketInfo.TradeState == StockOrderType.Completed)
            {
                return;
            }
            goldMarketInfo.TradeState = StockOrderType.Cannel;
            if (goldMarketInfo.TradeType == TradeType.Buy)
            {
                ChangeGameGold(Convert.ToInt32((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * goldMarketInfo.GameGoldPrice), "金币商城");
            }
            else if (goldMarketInfo.TradeType == TradeType.Sell)
            {
                ChangeGold((goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000);
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "金币交易行系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.ItemAdd,
                    Amount = (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000,
                    ExtraInfo = $"金币交易行取消订单退回交易未成交金币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }
            GoldMarketGetOwnOrder();

        }
        public void GoldMarketFlash()
        {
            //List<GoldMarketInfo> goldMarketInfos = SEnvir.GoldMarketInfoList.Binding as List<GoldMarketInfo>;
            var temlist = SEnvir.GoldMarketInfoList.Binding.Where(x => x.TradeState == StockOrderType.Normal && x.TradeType == TradeType.Sell).OrderBy(x => x.GameGoldPrice).ToList();
            //temlist?.Sort((x1, x2) =>
            //{
            //return x1.GameGoldPrice > x2.GameGoldPrice ? 1 : -1;
            //});
            long temprice = 0;
            var result = new S.GoldMarketList { BuyList = new List<ClientGoldMarketInfo>(), SellList = new List<ClientGoldMarketInfo>() };
            int count = 0;
            for (int i = 0; i < temlist.Count; ++i)
            {

                if (temlist[i].GameGoldPrice == 0) continue;
                if (temlist[i].GameGoldPrice != temprice)
                {
                    if (count > 5) break;
                    temprice = temlist[i].GameGoldPrice;
                    var tem = new ClientGoldMarketInfo { GoldPrice = temlist[i].GameGoldPrice, Count = temlist[i].GoldCount - temlist[i].CompletedCount };
                    result.SellList.Add(tem);
                    count++;
                    continue;
                }
                result.SellList[count - 1].Count += temlist[i].GoldCount - temlist[i].CompletedCount;
            }

            var temlist1 = SEnvir.GoldMarketInfoList.Binding.Where(x => x.TradeState == StockOrderType.Normal && x.TradeType == TradeType.Buy).OrderByDescending(x => x.GameGoldPrice).ToList();
            //temlist1?.Sort((x1, x2) =>
            //{
            //return x1.GameGoldPrice < x2.GameGoldPrice ? 1 : -1;
            //});
            count = 0;
            temprice = 0;
            for (int i = 0; i < temlist1.Count; ++i)
            {

                if (temlist1[i].GameGoldPrice == 0) continue;
                if (temlist1[i].GameGoldPrice != temprice)
                {
                    if (count > 5) break;
                    temprice = temlist1[i].GameGoldPrice;
                    var tem = new ClientGoldMarketInfo { GoldPrice = temlist1[i].GameGoldPrice, Count = temlist1[i].GoldCount - temlist1[i].CompletedCount };
                    result.BuyList.Add(tem);
                    count++;
                    continue;
                }
                result.BuyList[count - 1].Count += temlist1[i].GoldCount - temlist1[i].CompletedCount;
            }
            Enqueue(result);

        }
        public void GoldMarketGetOwnOrder()
        {
            Enqueue(new S.GoldMarketMyOrderList { MyOrder = Character.GoldMarketInfos.Where(x => x.Date.AddDays(7) > SEnvir.Now || x.TradeState != StockOrderType.Completed && x.TradeState != StockOrderType.Cannel).OrderByDescending(x => x.Index).Select(x => x.ToClientInfo()).ToList() });//如何只取不超过100行
        }
        #endregion
    }
}
