using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using Server.Models.Monsters;
using Server.Scripts.Npc;
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
    /// NPC对象
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// NPC对象
        /// </summary>
        public NPCObject NPC;
        /// <summary>
        /// 客户端NPC页面
        /// </summary>
        public INPCPage NPCPage;

        #region txt脚本
        public Dictionary<NPCSegment, bool> NPCSuccess = new Dictionary<NPCSegment, bool>();
        public List<string> NPCSpeech = new List<string>();
        public int NPCScriptID;
        public string NPCInputStr;

        public void NPCCall(uint objectID, string key)
        {
            if (Dead) return;
            key = key.ToUpper();

            foreach (NPCObject ob in CurrentMap.NPCs)
            {
                if (ob.ObjectID != objectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Config.MaxNPCViewRange)) return;
                if (NPC?.ObjectID != objectID)
                {
                    if (ob.ScriptID == 0)
                    {
                        var npcScript = NPCScript.Get(ob.NPCInfo.NPCFile);
                        ob.ScriptID = npcScript.ScriptID;
                    }
                    ob.CurrentScriptID = ob.ScriptID;
                    NPC = ob;
                }
                //脚本变化
                if (NPCScriptID != 0 && ob.CurrentScriptID != NPCScriptID)
                {
                    ob.CurrentScriptID = NPCScriptID;
                    NPC.Dead = true;
                }
                var scriptID = NPCScriptID;
                if (key == NPCScript.MainKey)
                {
                    scriptID = ob.ScriptID;
                }

                var script = NPCScript.Get(scriptID);
                script.Call(this, objectID, key);

                break;
            }
            CallNPCNextPage();
        }
        public void CallNPCNextPage()
        {
            //process any new npc calls immediately
            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Type != ActionType.NPC || ActionList[i].Time < SEnvir.Now) continue;
                var action = ActionList[i];

                ActionList.RemoveAt(i);

                CompleteNPC(action.Data);
            }
        }
        private void CompleteNPC(IList<object> data)
        {
            uint npcid = (uint)data[0];
            string page = (string)data[1];
            int scriptIndex = NPCScriptID;

            if (data.Count == 3)
            {
                scriptIndex = (int)data[2];
            }

            else if (data.Count == 4)
            {
                Map map = (Map)data[2];
                Point coords = (Point)data[3];

                Teleport(map, coords);
            }

            NPC.Dead = true;

            if (page.Length > 0)
            {
                var script = NPCScript.Get(scriptIndex);
                script.Call(this, npcid, page.ToUpper());
            }
        }

        #endregion

        public List<AsyncPyCall> AsyncPyCallList = new List<AsyncPyCall>();

        #region NPC调用内容
        /// <summary>
        /// NPC确认框
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="option"></param>
        public void NPCConfirmationBox(string msg, int option)
        {
            Enqueue(new S.ShowConfirmationBox { Msg = msg, MenuOption = option });
        }
        /// <summary>
        /// NPC文本输入框
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="option"></param>
        public void NPCTextInputBox(string msg, int option)
        {
            Enqueue(new S.ShowInputBox { Msg = msg, MenuOption = option });

        }

        #region D键菜单

        /// <summary>
        /// D键菜单
        /// </summary>
        public void DKeyCall()
        {
            //if (Dead) return;
            try
            {
                dynamic trig_dkey;
                trig_dkey = SEnvir.PythonEvent["PlayerEvent_trig_player"];

                PythonTuple args = PythonOps.MakeTuple(new object[] { this, });

                dynamic response = SEnvir.ExecutePyWithTimer(trig_dkey, this, "OnDKey", args);

                //IronPython.Runtime.List DKeyMenu = trig_dkey(this, "OnDKey", args);

                List<DKeyPage> labels = new List<DKeyPage>();

                if (response is IronPython.Runtime.List)
                {
                    IronPython.Runtime.List DKeyMenu = response;
                    //处理py返回的元素
                    //py返回值是一个list of dicts

                    foreach (PythonDictionary d in DKeyMenu)
                    {
                        PythonTuple color = (PythonTuple)d["TextColor"];
                        labels.Add(new DKeyPage
                        {
                            text = d["Text"].ToString(),
                            labelLocationX = int.Parse(d["LocationX"].ToString()),
                            labelLocationY = int.Parse(d["LocationY"].ToString()),
                            targetNPCIndex = uint.Parse(d["NPCIndex"].ToString()),
                            textColor = Color.FromArgb(int.Parse(color[0].ToString()), int.Parse(color[1].ToString()), int.Parse(color[2].ToString()))
                        });
                    }
                    Enqueue(new S.DKey { Options = labels });
                }


                else if (response is int)
                {
                    int npcIndex = response;
                    if (npcIndex > 0)
                    {
                        NPCCall((uint)npcIndex, true);
                    }
                }

            }
            catch (SyntaxErrorException e)
            {
                string msg = "D键菜单语法错误 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "D键菜单延迟系统推出 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "D键菜单加载插件出现延迟调用错误 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }
        }

        #endregion


        /// <summary>
        /// NPC单元
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="isDKey"></param>
        public void NPCCall(uint objectID, bool isDKey = false)
        {
            if (Dead) return;

            NPC = null;
            NPCPage = null;

            if (isDKey)
            {
                //TODO 应该写个单独的函数来找NPCObject
                int NPCIndex = (int)objectID;
                NPCInfo targetNPCInfo = SEnvir.GetNpcInfo(NPCIndex);
                Map NPCMap = SEnvir.GetMap(targetNPCInfo.Region.Map);
                NPCObject ob = NPCMap.NPCs.Find(x => x.NPCInfo.Index == NPCIndex);
                ob.NPCCall(this);
                return;
            }

            foreach (NPCObject ob in CurrentMap.NPCs)
            {
                if (ob.ObjectID != objectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Config.MaxViewRange))
                {
                    Connection.ReceiveChat(String.Format("距离{0}太远了", ob.Name), MessageType.System);
                    return;
                }

                //ob.NPCCall(this, ob.NPCInfo?.EntryPage);
                ob.NPCCall(this);
                return;
            }
        }
        /// <summary>
        /// NPC按钮
        /// </summary>
        /// <param name="ButtonID"></param>
        /// <param name="links"></param>
        /// <param name="userInput"></param>
        public void NPCButton(int ButtonID, List<CellLinkInfo> links, string userInput = "")
        {
            if (Dead || NPC == null || NPCPage == null) return;
            NPC.NPCCall(this, ButtonID, links, userInput);
        }

        #region NPC买

        public void NPCBuyBack(C.NPCBuyBack p)
        {
            if (Dead || NPC == null || NPCPage == null || p.PageIndex <= 0) return;


            var list = GetBackGoods(out int total, NPCPage.Types, p.PageIndex);
            for (var i = 0; i < list.Count; i++)
            {
                if (!NPCPage.Goods.Any(x => x.Index == list[i].Index))
                {
                    NPCPage.Goods.Add(list[i]);
                }
            }

            if (null != list)
                Enqueue(new S.NPCBuyBack { BackGoods = list, TotalPage = total, PageIndex = p.PageIndex });
        }
        public void NPCBuyBackSeach(C.NPCBuyBackSeach p)
        {
            if (p.Index <= 0) return;
            var list = SEnvir.AllOwnerlessItemList.Where(x => x.Info != null && x.Info.Index == p.Index && x.OwnerLessTime.AddDays(2) > SEnvir.Now);
            var result = list.Skip((p.PageIndex - 1) * Consts.BackItemCount).Take(Consts.BackItemCount).Select(
              x => new ClientNPCGood
              {
                  UserItem = x.ToClientInfo(),
                  ItemName = x.Info.ItemName,
                  Index = x.Index,
                  Rate = GetBackRate(x)
              });
            if (list.Count() == 0)//商品已卖完
            {
                Connection.ReceiveChat("NPCBack.Goods.Over".Lang(Connection.Language), MessageType.System);
                Connection.Enqueue(new S.NPCBuyBackSeach { BackGoods = new List<ClientNPCGood>(), PageIndex = p.PageIndex, TotalPage = 0 });
                return;
            }
            var count = list.Count();
            int total = count % Consts.BackItemCount == 0 ? count / Consts.BackItemCount : count / Consts.BackItemCount + 1;
            Connection.Enqueue(new S.NPCBuyBackSeach { BackGoods = result.ToList(), PageIndex = p.PageIndex, TotalPage = total });
        }

        /// <summary>
        /// 计算回购价格比例
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private decimal GetBackRate(UserItem item)
        {
            //return Math.Round(item.CurrentDurability * 1m / item.MaxDurability, 2) + item.AddedStats.Count * 0.2m; //极品个数*0.2+物品持久比例;

            //TODO 叠加物品 代码要重写
            return item.Price(1) * 2m / item.Info.Price;
        }
        /// <summary>
        /// 获取回购分类列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public List<ClientNPCGood> GetBackGoods(out int total, List<ItemType> typelist, int pageIndex = 1)//TODO 使用分页
        {
            var list = SEnvir.AllOwnerlessItemList;
            if (list?.Count == 0)
            {
                total = 0;
                return new List<ClientNPCGood>();
            }
            var ids = list.Where(x => x.Info != null && (typelist.Exists(item => item == x.Info.ItemType)) && x.OwnerLessTime.AddDays(2) > SEnvir.Now).Select(x => x.Info.Index).Distinct().ToArray();
            var result = SEnvir.ItemInfoList.Binding.Where(x => ids.Contains(x.Index))
                .Skip((pageIndex - 1) * Consts.BackCount).Take(Consts.BackCount).Select(
                x => new ClientNPCGood
                {
                    ItemName = x.ItemName,
                    Rate = 1m,
                    Index = x.Index,
                }).ToList();
            var count = SEnvir.ItemInfoList.Binding.Where(x => ids.Contains(x.Index)).Count();
            total = count % Consts.BackCount == 0 ? count / Consts.BackCount : count / Consts.BackCount + 1;
            return result;
        }

        /// <summary>
        /// NPC买
        /// </summary>
        /// <param name="p"></param>
        public void NPCBuy(C.NPCBuy p)
        {
            if (Dead || NPC == null || NPCPage == null || p.Amount <= 0) return;

            decimal taxRate = 0;
            CastleFundInfo fundInfo = null;

            if (NPCPage is ClientNPCPage page)
            {
                fundInfo = SEnvir.CastleFundInfoList.Binding.FirstOrDefault(x => x.Castle?.Name == page.CastleName);
                taxRate = page.TaxRate;
            }

            ClientNPCGood target = null;
            ItemInfo info = null;
            UserItem userItem = null;
            if (p.IsBuyback)//回购
            {
                userItem = SEnvir.AllOwnerlessItemList.FirstOrDefault(x => x.Index == p.Index);
                info = userItem?.Info;

                target = NPCPage.Goods.FirstOrDefault(x => x.Index == info?.Index);
            }
            else
            {
                target = NPCPage.Goods.FirstOrDefault(x => x.Item != null && x.Index == p.Index);
                info = target?.Item;
            }

            if (target == null || info == null)
            {
                //Connection.ReceiveChat("MarketPlace.Selled".Lang(Connection.Language), MessageType.System);
                //SEnvir.Log($"购物出错, NPC = {{NPC.NPCInfo.NPCName}}, 玩家 = {{this.Character.CharacterName}}");
                return;
            }

            if (p.Amount > info.StackSize) return;

            //非沙巴克成员购买物品时一直开着窗口可以折扣购买药
            if (Character?.Account?.GuildMember?.Guild?.Castle == null) target.Rate = target.RealRate;

            // 折后价格
            long cost = (long)(target.Rate * info.Price * p.Amount);
            if (p.IsBuyback)
            {
                cost = (long)(GetBackRate(userItem) * info.Price * p.Amount);
            }
            int itemCost = Int32.MaxValue;

            if (target.Currency != Globals.Currency)
            {
                itemCost = (int)(target.CurrencyCost * p.Amount);

                if (GetItemCount(target.Currency) < itemCost)
                {
                    Connection.ReceiveChat("MarketPlace.ConsignBuyCost".Lang(Connection.Language), MessageType.System);
                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("MarketPlace.ConsignBuyCost".Lang(con.Language), MessageType.System);
                    return;
                }
            }
            else
            {
                if (p.GuildFunds)
                {
                    if (Character.Account.GuildMember == null)
                    {
                        Connection.ReceiveChat("NPC.NPCFundsGuild".Lang(Connection.Language), MessageType.System);
                        return;
                    }
                    if ((Character.Account.GuildMember.Permission & GuildPermission.FundsMerchant) != GuildPermission.FundsMerchant)
                    {
                        Connection.ReceiveChat("NPC.NPCFundsPermission".Lang(Connection.Language), MessageType.System);
                        return;
                    }

                    if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                    {
                        Connection.ReceiveChat("NPC.NPCFundsCost".Lang(Connection.Language, Character.Account.GuildMember.Guild.GuildFunds - cost), MessageType.System);
                        return;
                    }
                }
                else
                {
                    if (cost > Gold)
                    {
                        Connection.ReceiveChat("NPC.NPCCost".Lang(Connection.Language, Gold - cost), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("NPC.NPCCost".Lang(con.Language, Gold - cost), MessageType.System);
                        return;
                    }
                }
            }

            UserItemFlags flags = UserItemFlags.None;    //商店出售物品购买到锁定.Locked

            switch (info.ItemType)  //商品项目类型
            {
                case ItemType.Weapon:   //武器
                case ItemType.Armour:   //衣服
                case ItemType.Helmet:   //头盔
                case ItemType.Necklace: //项链
                case ItemType.Bracelet: //手镯
                case ItemType.Ring:     //戒指
                case ItemType.Shoes:    //鞋
                    if (Config.ShopNonRefinable)
                    {
                        flags |= UserItemFlags.NonRefinable;
                    }
                    else
                    {
                        if (info.Effect == ItemEffect.PickAxe)  //如果是挖矿工具，就不能精炼
                        {
                            flags |= UserItemFlags.NonRefinable;
                        }
                        else
                        {
                            flags |= UserItemFlags.None;
                        }
                    }
                    break;
                case ItemType.Book:     //书
                    flags |= UserItemFlags.NonRefinable;
                    break;
            }

            ItemCheck check = new ItemCheck(info, p.Amount, flags, TimeSpan.Zero);

            if (!CanGainItems(true, check))
            {
                Connection.ReceiveChat("NPC.NPCNoRoom".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCNoRoom".Lang(con.Language), MessageType.System);
                return;
            }

            UserItem item = null;
            if (p.IsBuyback)
            {
                item = SEnvir.AllOwnerlessItemList.FirstOrDefault(x => x.Index == p.Index);
            }
            else
            {
                item = SEnvir.CreateFreshItem(check);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, NPC?.NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPC?.Name, Character?.CharacterName);
            }

            if (item == null)
            {
                // Cannot buy, item has been sold already.
                Connection.ReceiveChat("无法购买, 物品已售出", MessageType.System);
                return;
            }

            if (target.Currency != Globals.Currency)
            {
                TakeItem(target.Currency, itemCost);
            }
            else
            {
                if (p.GuildFunds)
                {
                    Character.Account.GuildMember.Guild.GuildFunds -= cost;
                    Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                    {
                        member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                        member.Account.Connection?.ReceiveChat("NPC.NPCFundsBuy".Lang(member.Account.Connection.Language, Name, cost, item.Info.ItemName, item.Count), MessageType.System);
                    }
                }
                else
                {
                    Gold -= cost;
                    GoldChanged();

                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "NPC购买系统",
                        Time = SEnvir.Now,
                        Character = Character,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Deduct,
                        Source = CurrencySource.ItemAdd,
                        Amount = cost,
                        ExtraInfo = $"道具名: {item.Info.ItemName}"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
                // 增加城堡税收
                if (fundInfo != null)
                {
                    fundInfo.TotalTax += (long)(cost * taxRate);
                }
            }

            GainItem(item);
            if (p.IsBuyback)
            {
                SEnvir.AllOwnerlessItemList.Remove(item);
                NPCBuyBackSeach(new C.NPCBuyBackSeach { Index = info.Index, PageIndex = p.PageIndex });
            }
        }

        #endregion

        #region NPC卖

        /// <summary>
        /// NPC卖
        /// </summary>
        /// <param name="links"></param>
        public void NPCSell(List<CellLinkInfo> links)
        {
            S.ItemsChanged p = new S.ItemsChanged { Links = links };
            Enqueue(p);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.BuySell) return;   //添加类型对应卖 NPCDialogType.Sell

            if (!ParseLinks(p.Links, 0, 100)) return;

            long gold = 0;
            long count = 0;

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:    //背包
                        fromArray = Inventory;
                        break;
                    case GridType.PatchGrid:    //碎片包裹
                        fromArray = PatchGrid;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || !item.Info.CanSell || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
                if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return;

                count += link.Count;
                gold += item.Price(link.Count);
            }

            if (gold < 0)
            {
                Connection.ReceiveChat("NPC.NPCSellWorthless".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCSellWorthless".Lang(con.Language), MessageType.System);
                return;
            }

            foreach (CellLinkInfo link in links)  //链接中的单元格链接信息链接
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
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

                UserItem item = fromArray[link.Slot];

                if (item.Count == link.Count)
                {
                    // 回购设置
                    if (Config.AllowBuyback && item.CanBuyback)
                    {
                        item.MarkOwnerless(OwnerlessItemType.UserSold);
                        fromArray[link.Slot] = null;
                        SEnvir.AllOwnerlessItemList.Add(item);
                    }
                    else
                    {
                        RemoveItem(item);
                        fromArray[link.Slot] = null;
                        item.Delete();
                    }
                }
                else
                    item.Count -= link.Count;
            }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            Connection.ReceiveChat("NPC.NPCSellResult".Lang(Connection.Language, count, gold), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("NPC.NPCSellResult".Lang(con.Language, count, gold), MessageType.System);

            p.Success = true;
            Gold += gold;

            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC出售系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Add,
                Source = CurrencySource.ItemDeduct,
                Amount = gold,
                ExtraInfo = $"NPC出售道具获得金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
        }

        #endregion

        #region NPC一键出售

        /// <summary>
        /// NPC一键出售
        /// </summary>
        /// <param name="links"></param>
        public void NPCRootSell(List<CellLinkInfo> links)
        {
            S.ItemsChanged p = new S.ItemsChanged { Links = links };
            Enqueue(p);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.RootSell) return;

            if (!ParseLinks(p.Links, 0, 100)) return;

            long gold = 0;
            long count = 0;

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || !item.Info.CanSell || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
                if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return;

                count += link.Count;
                gold += item.Price(link.Count);
            }

            if (gold < 0)
            {
                Connection.ReceiveChat("NPC.NPCSellWorthless".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCSellWorthless".Lang(con.Language), MessageType.System);
                return;
            }

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = fromArray[link.Slot];

                if (item.Count == link.Count)
                {
                    // 回购设置
                    if (Config.AllowBuyback && item.CanBuyback)
                    {
                        item.MarkOwnerless(OwnerlessItemType.UserSold);
                        fromArray[link.Slot] = null;
                        SEnvir.AllOwnerlessItemList.Add(item);
                    }
                    else
                    {
                        RemoveItem(item);
                        fromArray[link.Slot] = null;
                        item.Delete();
                    }
                }
                else
                    item.Count -= link.Count;
            }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            Connection.ReceiveChat("NPC.NPCSellResult".Lang(Connection.Language, count, gold), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("NPC.NPCSellResult".Lang(con.Language, count, gold), MessageType.System);

            p.Success = true;
            Gold += gold;

            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC一键出售系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Add,
                Source = CurrencySource.ItemDeduct,
                Amount = gold,
                ExtraInfo = $"NPC一键出售道具获得金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
        }

        #endregion

        #region NPC分解装备获得碎片

        /// <summary>
        /// NPC分解装备获得碎片
        /// </summary>
        /// <param name="links"></param>
        public void NPCFragment(List<CellLinkInfo> links)
        {
            S.ItemsChanged p = new S.ItemsChanged { Links = links };
            Enqueue(p);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.ItemFragment) return;

            if (!ParseLinks(p.Links, 0, 100)) return;

            long cost = 0;
            int fragmentCount = 0;
            int fragment2Count = 0;
            int itemCount = 0;

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return; //No harm in checking
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
                if (!item.CanFragment()) return;

                cost += item.FragmentCost();
                itemCount++;

                if (item.Info.Rarity == Rarity.Common)
                    fragmentCount += item.FragmentCount();
                else
                    fragment2Count += item.FragmentCount();
            }

            if (cost > Gold)
            {
                Connection.ReceiveChat("NPC.FragmentCost".Lang(Connection.Language, Gold - cost), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.FragmentCost".Lang(con.Language, Gold - cost), MessageType.System);
                return;
            }

            List<ItemCheck> checks = new List<ItemCheck>();

            if (fragmentCount > 0)
                checks.Add(new ItemCheck(SEnvir.FragmentInfo, fragmentCount, UserItemFlags.None, TimeSpan.Zero));

            if (fragment2Count > 0)
                checks.Add(new ItemCheck(SEnvir.Fragment2Info, fragment2Count, UserItemFlags.None, TimeSpan.Zero));

            if (!CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat("NPC.FragmentSpace".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.FragmentSpace".Lang(con.Language), MessageType.System);
                return;
            }

            foreach (CellLinkInfo link in links)
            {
                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:
                        fromArray = Inventory;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = fromArray[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (ItemCheck check in checks)
                while (check.Count > 0)
                {
                    UserItem newItem = SEnvir.CreateFreshItem(check);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(newItem, NPC?.NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPC?.Name, Character?.CharacterName);
                    GainItem(newItem);
                }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            Connection.ReceiveChat("NPC.FragmentResult".Lang(Connection.Language, itemCount, cost), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("NPC.FragmentResult".Lang(con.Language, itemCount, cost), MessageType.System);

            p.Success = true;
            Gold -= cost;

            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC分解装备系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = cost,
                ExtraInfo = $"NPC分解装备扣除手续费"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
        }

        #endregion

        #region NPC首饰等级升级

        /// <summary>
        /// NPC首饰熔炼升级
        /// </summary>
        /// <param name="p"></param>
        public void NPCAccessoryLevelUp(C.NPCAccessoryLevelUp p)
        {
            Enqueue(new S.NPCAccessoryLevelUp { Target = p.Target, Links = p.Links });
            //  死亡      NPC等空         NPC页等空             NPC类型不是首饰熔炼升级    跳出
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryRefineLevel) return;
            //  道具解析链接   最小值0   最大值100       不是对象信息   跳出
            if (!ParseLinks(p.Links, 0, 100) || !ParseLinks(p.Target)) return;
            // 角色道具 目标数组 等空
            UserItem[] targetArray = null;
            //对象的格子类型
            switch (p.Target.GridType)
            {
                case GridType.Inventory:  //背包
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:  //人物装备栏
                    targetArray = Equipment;
                    break;
                case GridType.Storage:   //仓库
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;  //如果宠物等空跳出

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Target.Slot < 0 || p.Target.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Target.Slot];

            if (targetItem == null || p.Target.Count > targetItem.Count) return; //目标道具为空  或者 目标道具计数大于 目标道具的计数值 跳出
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return; //道具标签是不可精炼的 跳出
            //目标道具 信息 道具类型 首饰
            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:     //戒指
                case ItemType.Bracelet: //手镯
                case ItemType.Necklace: //项链
                    break;
                default: return;
            }
            //目标道具 等级 大于等于 设置的 道具经验值 等级计数  跳出
            if (targetItem.Level >= Globals.GameAccessoryEXPInfoList.Count) return;
            //定义一个 道具改变 判断开关
            bool changed = false;

            S.ItemsChanged result = new S.ItemsChanged { Links = new List<CellLinkInfo>(), Success = true };
            Enqueue(result);
            long goldtemp = Gold;
            foreach (CellLinkInfo link in p.Links)
            {
                if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) break;

                UserItem[] fromArray = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:  //背包
                        fromArray = Inventory;
                        break;
                    case GridType.Storage:  //仓库
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        if (Companion == null) continue;

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        continue;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) continue;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) continue;
                if (item.Info != targetItem.Info) continue;
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound && (targetItem.Flags & UserItemFlags.Bound) != UserItemFlags.Bound) continue;

                long cost = Globals.AccessoryLevelCost * link.Count;

                if (Gold < cost)
                {
                    Connection.ReceiveChat("NPC.AccessoryLevelCost".Lang(Connection.Language), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.AccessoryLevelCost".Lang(con.Language), MessageType.System);
                    continue;
                }

                result.Links.Add(link);

                //给道具增加经验
                if (targetItem.Info.Rarity != Rarity.Common || targetItem.Level == 1)
                    targetItem.Experience += link.Count * 5;
                else
                    targetItem.Experience += link.Count;

                if (item.Level > 1 && targetItem.Info.Rarity == Rarity.Common)
                    targetItem.Experience -= 4;

                while (item.Level > 1)
                {
                    targetItem.Experience += Globals.GameAccessoryEXPInfoList[item.Level - 1].Exp;
                    item.Level--;
                }

                targetItem.Experience += item.Experience;

                Gold -= cost;

                if (targetItem.Experience >= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp)
                {
                    targetItem.Experience -= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp;
                    targetItem.Level++;

                    targetItem.Flags |= UserItemFlags.Refinable;
                }

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;

                changed = true;
            }

            if (changed)
            {
                if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable)
                {
                    Connection.ReceiveChat("NPC.AccessoryLeveled".Lang(Connection.Language, targetItem.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.AccessoryLeveled".Lang(con.Language, targetItem.Info.ItemName), MessageType.System);
                }

                Companion?.RefreshWeight();
                RefreshWeight();
                GoldChanged();
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "NPC首饰熔炼系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = goldtemp - Gold,
                    ExtraInfo = $"NPC首饰熔炼扣除金币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                Enqueue(new S.ItemExperience { Target = p.Target, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });

                //首饰升级事件
                //队列一个事件, 不要忘记添加listener
                SEnvir.EventManager.QueueEvent(
                    new PlayerAccessoryRefineLevel(EventTypes.PlayerAccessoryRefineLevel,
                        new PlayerAccessoryRefineLevelEventArgs()));
            }
        }

        #endregion

        #region NPC首饰升级

        /// <summary>
        /// NPC首饰升级增加属性
        /// </summary>
        /// <param name="p"></param>
        public void NPCAccessoryUpgrade(C.NPCAccessoryUpgrade p)
        {
            Enqueue(new S.ItemChanged { Link = p.Target }); //解锁道具

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryRefineUpgrade) return;

            if (!ParseLinks(p.Target)) return;

            UserItem[] targetArray = null;

            switch (p.Target.GridType)
            {
                case GridType.Inventory:  //背包
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:  //人物装备栏
                    targetArray = Equipment;
                    break;
                case GridType.Storage:   //仓库
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (p.Target.Slot < 0 || p.Target.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Target.Slot];

            if (targetItem == null || p.Target.Count > targetItem.Count) return; //Already Leveled.
            if ((targetItem.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return;
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:  //戒指
                case ItemType.Bracelet:  //手镯
                case ItemType.Necklace:  //项链
                    break;
                default: return;
            }

            S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = p.Target.GridType, Slot = p.Target.Slot, NewStats = new Stats() };
            Enqueue(result);

            switch (p.RefineType)
            {
                case RefineType.DC:  //破坏
                    targetItem.AddStat(Stat.MaxDC, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxDC] = 1;
                    break;
                case RefineType.SpellPower:  //全系列魔法
                    if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                    {
                        targetItem.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxMC] = 1;

                        targetItem.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxSC] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                    {
                        targetItem.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxMC] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                    {
                        targetItem.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                        result.NewStats[Stat.MaxSC] = 1;
                    }
                    break;
                case RefineType.Health:  //血值
                    targetItem.AddStat(Stat.Health, 10, StatSource.Refine);
                    result.NewStats[Stat.Health] = 10;
                    break;
                case RefineType.Mana:    //魔法值
                    targetItem.AddStat(Stat.Mana, 10, StatSource.Refine);
                    result.NewStats[Stat.Mana] = 10;
                    break;
                case RefineType.DCPercent:  //破坏百分比
                    targetItem.AddStat(Stat.DCPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.DCPercent] = 1;
                    break;
                case RefineType.SPPercent:  //全系列魔法百分比
                    if (targetItem.Info.Stats[Stat.MinMC] == 0 && targetItem.Info.Stats[Stat.MaxMC] == 0 && targetItem.Info.Stats[Stat.MinSC] == 0 && targetItem.Info.Stats[Stat.MaxSC] == 0)
                    {
                        targetItem.AddStat(Stat.MCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.MCPercent] = 1;

                        targetItem.AddStat(Stat.SCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.SCPercent] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinMC] > 0 || targetItem.Info.Stats[Stat.MaxMC] > 0)
                    {
                        targetItem.AddStat(Stat.MCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.MCPercent] = 1;
                    }

                    if (targetItem.Info.Stats[Stat.MinSC] > 0 || targetItem.Info.Stats[Stat.MaxSC] > 0)
                    {
                        targetItem.AddStat(Stat.SCPercent, 1, StatSource.Refine);
                        result.NewStats[Stat.SCPercent] = 1;
                    }
                    break;
                case RefineType.HealthPercent:  //血值百分比
                    targetItem.AddStat(Stat.HealthPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.HealthPercent] = 1;
                    break;
                case RefineType.ManaPercent:   //魔法值百分比
                    targetItem.AddStat(Stat.ManaPercent, 1, StatSource.Refine);
                    result.NewStats[Stat.ManaPercent] = 1;
                    break;
                case RefineType.Fire:  //火
                    targetItem.AddStat(Stat.FireAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.FireAttack] = 1;
                    break;
                case RefineType.Ice:   //冰
                    targetItem.AddStat(Stat.IceAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.IceAttack] = 1;
                    break;
                case RefineType.Lightning:  //雷
                    targetItem.AddStat(Stat.LightningAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.LightningAttack] = 1;
                    break;
                case RefineType.Wind:  //风
                    targetItem.AddStat(Stat.WindAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.WindAttack] = 1;
                    break;
                case RefineType.Holy:  //神圣
                    targetItem.AddStat(Stat.HolyAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.HolyAttack] = 1;
                    break;
                case RefineType.Dark:  //暗黑
                    targetItem.AddStat(Stat.DarkAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.DarkAttack] = 1;
                    break;
                case RefineType.Phantom:  //体质
                    targetItem.AddStat(Stat.PhantomAttack, 1, StatSource.Refine);
                    result.NewStats[Stat.PhantomAttack] = 1;
                    break;
                case RefineType.AC:  //物理防御
                    targetItem.AddStat(Stat.MinAC, 1, StatSource.Refine);
                    result.NewStats[Stat.MinAC] = 1;
                    targetItem.AddStat(Stat.MaxAC, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxAC] = 1;
                    break;
                case RefineType.MR:  //魔法防御
                    targetItem.AddStat(Stat.MinMR, 1, StatSource.Refine);
                    result.NewStats[Stat.MinMR] = 1;
                    targetItem.AddStat(Stat.MaxMR, 1, StatSource.Refine);
                    result.NewStats[Stat.MaxMR] = 1;
                    break;
                case RefineType.Accuracy:  //准确
                    targetItem.AddStat(Stat.Accuracy, 1, StatSource.Refine);
                    result.NewStats[Stat.Accuracy] = 1;
                    break;
                case RefineType.Agility:  //敏捷
                    targetItem.AddStat(Stat.Agility, 1, StatSource.Refine);
                    result.NewStats[Stat.Agility] = 1;
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "开始精炼，附加精炼类型".Lang(Connection.Language);
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            //刷新完整属性列表
            result.FullItemStats = targetItem.ToClientInfo().FullItemStats;
            targetItem.Flags &= ~UserItemFlags.Refinable;
            targetItem.StatsChanged();

            RefreshStats();

            if (targetItem.Experience >= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp)
            {
                targetItem.Experience -= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp;
                targetItem.Level++;

                targetItem.Flags |= UserItemFlags.Refinable;
            }

            Enqueue(new S.ItemExperience { Target = p.Target, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });
        }

        #endregion

        #region NPC首饰重置

        /// <summary>
        /// NPC首饰重置
        /// </summary>
        /// <param name="p"></param>
        public void NPCAccessoryReset(C.NPCAccessoryReset p)
        {
            Enqueue(new S.ItemChanged { Link = p.Cell }); //解锁道具

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryReset) return;

            if (!ParseLinks(p.Cell)) return;

            UserItem[] targetArray = null;

            switch (p.Cell.GridType)
            {
                case GridType.Inventory:   //背包
                    targetArray = Inventory;
                    break;
                case GridType.Equipment:   //人物装备栏
                    targetArray = Equipment;
                    break;
                case GridType.Storage:    //仓库
                    targetArray = Storage;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return;

                    targetArray = Companion.Inventory;
                    break;
                default:
                    return;
            }

            if (Globals.AccessoryResetCost > Gold)
            {
                Connection.ReceiveChat("NPC.NPCRefinementGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (p.Cell.Slot < 0 || p.Cell.Slot >= targetArray.Length) return;
            UserItem targetItem = targetArray[p.Cell.Slot];

            if (targetItem == null || p.Cell.Count > targetItem.Count) return; //已经调整
            if ((targetItem.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            switch (targetItem.Level)
            {
                case 1:
                    return;
                case 2:
                    if ((targetItem.Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable) return; //不退还
                    break;
                default:
                    break;
            }

            switch (targetItem.Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return;
            }

            S.ItemStatsRefreshed result = new S.ItemStatsRefreshed { GridType = p.Cell.GridType, Slot = p.Cell.Slot };
            Enqueue(result);

            for (int i = targetItem.AddedStats.Count - 1; i >= 0; i--)
            {
                if (targetItem.AddedStats[i].StatSource != StatSource.Refine) continue;

                targetItem.AddedStats[i].Delete();
            }

            targetItem.StatsChanged();

            result.NewStats = new Stats(targetItem.Stats);
            //刷新完整属性列表
            result.FullItemStats = targetItem.ToClientInfo().FullItemStats;

            RefreshStats();

            Gold -= Globals.AccessoryResetCost;
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC首饰重置系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = Globals.AccessoryResetCost,
                ExtraInfo = $"NPC首饰重置费用扣除金币"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            while (targetItem.Level > 1)
            {
                targetItem.Experience += Globals.GameAccessoryEXPInfoList[targetItem.Level - 1].Exp;
                targetItem.Level--;
            }

            if (targetItem.Experience >= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp)
            {
                targetItem.Experience -= Globals.GameAccessoryEXPInfoList[targetItem.Level].Exp;
                targetItem.Level++;

                targetItem.Flags |= UserItemFlags.Refinable;
            }

            Enqueue(new S.ItemExperience { Target = p.Cell, Experience = targetItem.Experience, Level = targetItem.Level, Flags = targetItem.Flags });
        }

        #endregion

        #region NPC修理

        /// <summary>
        /// NPC修理
        /// </summary>
        /// <param name="p"></param>
        public void NPCRepair(C.NPCRepair p)
        {
            S.NPCRepair result = new S.NPCRepair { Links = p.Links, Special = p.Special, SpecialRepairDelay = Config.SpecialRepairDelay };
            Enqueue(result);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.Repair) return;

            if (!ParseLinks(result.Links, 0, 100)) return;

            long cost = 0;
            int count = 0;

            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array;
                switch (link.GridType)
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
                        if (Character.Account.GuildMember == null) return;
                        if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return;

                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || !item.Info.CanRepair || item.Info.Durability == 0) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                switch (item.Info.ItemType)
                {
                    case ItemType.Weapon:   //武器
                    case ItemType.Armour:   //衣服
                    case ItemType.Helmet:   //头盔
                    case ItemType.Necklace: //项链
                    case ItemType.Bracelet: //手镯
                    case ItemType.Ring:     //戒指
                    case ItemType.Shoes:    //鞋子
                    case ItemType.Shield:   //盾牌
                    case ItemType.Emblem:   //勋章
                    case ItemType.Fashion:  //时装
                        break;
                    default:
                        Connection.ReceiveChat("NPC.RepairFail".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("NPC.RepairFail".Lang(con.Language, item.Info.ItemName), MessageType.System);
                        return;
                }

                if (item.CurrentDurability >= item.MaxDurability)
                {
                    Connection.ReceiveChat("NPC.RepairFailRepaired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.RepairFailRepaired".Lang(con.Language, item.Info.ItemName), MessageType.System);
                    return;
                }
                if (Config.DistinctionRepair)   //如果 修理类型有区分
                {
                    if (NPCPage.Types.All(x => x != item.Info.ItemType))
                    //if (NPCPage.Types.FirstOrDefault(x => x.ItemType == item.Info.ItemType) == null)
                    {
                        Connection.ReceiveChat("NPC.RepairFailLocation".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("NPC.RepairFailLocation".Lang(con.Language, item.Info.ItemName), MessageType.System);
                        return;
                    }
                }
                if (p.Special && SEnvir.Now < item.SpecialRepairCoolDown)  //如果是特修 特修时间未到 出提示
                {
                    Connection.ReceiveChat("NPC.RepairFailCooldown".Lang(Connection.Language, item.Info.ItemName, (item.SpecialRepairCoolDown - SEnvir.Now).Lang(Connection.Language, false)), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.RepairFailCooldown".Lang(con.Language, item.Info.ItemName, (item.SpecialRepairCoolDown - SEnvir.Now).Lang(con.Language, false)), MessageType.System);
                    return;
                }

                count++;
                cost += array[link.Slot].RepairCost(p.Special);
            }

            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat("NPC.NPCRepairGuild".Lang(Connection.Language), MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsRepair) != GuildPermission.FundsRepair)
                {
                    Connection.ReceiveChat("NPC.NPCRepairPermission".Lang(Connection.Language), MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat("NPC.NPCRepairGuildCost".Lang(Connection.Language, Character.Account.GuildMember.Guild.GuildFunds - cost), MessageType.System);
                    return;
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat("NPC.NPCRepairCost".Lang(Connection.Language, Gold - cost), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.NPCRepairCost".Lang(con.Language, Gold - cost), MessageType.System);
                    return;
                }
            }

            bool refresh = false;
            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:  //背包
                        array = Inventory;
                        break;
                    case GridType.Equipment:  //人物装备栏
                        array = Equipment;
                        break;
                    case GridType.Storage:  //仓库
                        array = Storage;
                        break;
                    case GridType.GuildStorage:  //行会仓库
                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        array = Companion.Inventory;
                        break;
                }

                UserItem item = array[link.Slot];

                if (item.CurrentDurability == 0 && link.GridType == GridType.Equipment)
                    refresh = true;

                if (p.Special)
                {
                    item.CurrentDurability = item.MaxDurability;

                    if (item.Info.ItemType != ItemType.Weapon)
                        item.SpecialRepairCoolDown = SEnvir.Now + Config.SpecialRepairDelay;
                }
                else
                {
                    item.MaxDurability = Math.Max(0, item.MaxDurability - (item.MaxDurability - item.CurrentDurability) / Globals.DurabilityLossRate);
                    item.CurrentDurability = item.MaxDurability;
                }
            }

            Connection.ReceiveChat((p.Special ? "NPC.NPCRepairSpecialResult" : "NPC.NPCRepairResult").Lang(Connection.Language, count, cost), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat((p.Special ? "NPC.NPCRepairSpecialResult" : "NPC.NPCRepairResult").Lang(con.Language, count, cost), MessageType.System);

            result.Success = true;

            if (p.GuildFunds)
            {
                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat("NPC.NPCRepairGuildResult".Lang(member.Account.Connection.Language, Name, cost, count), MessageType.System);
                }
            }
            else
            {
                Gold -= cost;
                GoldChanged();
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "NPC修理系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = cost,
                    ExtraInfo = $"NPC修理扣除费用"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            if (refresh)
                RefreshStats();
        }

        #endregion

        #region NPC特修

        /// <summary>
        /// NPC特修
        /// </summary>
        /// <param name="p"></param>
        public void NPCSpecialRepair(C.NPCSpecialRepair p)
        {
            S.NPCRepair result = new S.NPCRepair { Links = p.Links, Special = p.Special, SpecialRepairDelay = Config.SpecialRepairDelay };
            Enqueue(result);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.SpecialRepair) return;

            if (!ParseLinks(result.Links, 0, 100)) return;

            long cost = 0;
            int count = 0;

            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array;
                switch (link.GridType)
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
                        if (Character.Account.GuildMember == null) return;
                        if ((Character.Account.GuildMember.Permission & GuildPermission.Storage) != GuildPermission.Storage) return;

                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || !item.Info.CanRepair || item.Info.Durability == 0) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                switch (item.Info.ItemType)
                {
                    case ItemType.Weapon:   //武器
                    case ItemType.Armour:   //衣服
                    case ItemType.Helmet:   //头盔
                    case ItemType.Necklace: //项链
                    case ItemType.Bracelet: //手镯
                    case ItemType.Ring:     //戒指
                    case ItemType.Shoes:    //鞋子
                    case ItemType.Shield:   //盾牌
                        break;
                    default:
                        Connection.ReceiveChat("NPC.RepairFail".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("NPC.RepairFail".Lang(con.Language, item.Info.ItemName), MessageType.System);
                        return;
                }

                if (item.CurrentDurability >= item.MaxDurability)
                {
                    Connection.ReceiveChat("NPC.RepairFailRepaired".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.RepairFailRepaired".Lang(con.Language, item.Info.ItemName), MessageType.System);
                    return;
                }
                if (NPCPage.Types.All(x => x != item.Info.ItemType))
                {
                    Connection.ReceiveChat("NPC.RepairFailLocation".Lang(Connection.Language, item.Info.ItemName), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.RepairFailLocation".Lang(con.Language, item.Info.ItemName), MessageType.System);
                    return;
                }
                if (p.Special && SEnvir.Now < item.SpecialRepairCoolDown)
                {
                    Connection.ReceiveChat("NPC.RepairFailCooldown".Lang(Connection.Language, item.Info.ItemName, (item.SpecialRepairCoolDown - SEnvir.Now).Lang(Connection.Language, false)), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.RepairFailCooldown".Lang(con.Language, item.Info.ItemName, (item.SpecialRepairCoolDown - SEnvir.Now).Lang(con.Language, false)), MessageType.System);
                    return;
                }
                count++;
                cost += array[link.Slot].RepairCost(p.Special);
            }

            if (p.GuildFunds)
            {
                if (Character.Account.GuildMember == null)
                {
                    Connection.ReceiveChat("NPC.NPCRepairGuild".Lang(Connection.Language), MessageType.System);
                    return;
                }
                if ((Character.Account.GuildMember.Permission & GuildPermission.FundsRepair) != GuildPermission.FundsRepair)
                {
                    Connection.ReceiveChat("NPC.NPCRepairPermission".Lang(Connection.Language), MessageType.System);
                    return;
                }

                if (cost > Character.Account.GuildMember.Guild.GuildFunds)
                {
                    Connection.ReceiveChat("NPC.NPCRepairGuildCost".Lang(Connection.Language, Character.Account.GuildMember.Guild.GuildFunds - cost), MessageType.System);
                    return;
                }
            }
            else
            {
                if (cost > Gold)
                {
                    Connection.ReceiveChat("NPC.NPCRepairCost".Lang(Connection.Language, Gold - cost), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.NPCRepairCost".Lang(con.Language, Gold - cost), MessageType.System);
                    return;
                }
            }

            bool refresh = false;
            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] array = null;

                switch (link.GridType)
                {
                    case GridType.Inventory:  //背包
                        array = Inventory;
                        break;
                    case GridType.Equipment:  //人物装备栏
                        array = Equipment;
                        break;
                    case GridType.Storage:  //仓库
                        array = Storage;
                        break;
                    case GridType.GuildStorage:  //行会仓库
                        array = Character.Account.GuildMember.Guild.Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        array = Companion.Inventory;
                        break;
                }

                UserItem item = array[link.Slot];

                if (item.CurrentDurability == 0 && link.GridType == GridType.Equipment)
                    refresh = true;

                if (p.Special)
                {
                    item.CurrentDurability = item.MaxDurability;

                    if (item.Info.ItemType != ItemType.Weapon)
                        item.SpecialRepairCoolDown = SEnvir.Now + Config.SpecialRepairDelay;
                }
                else
                {
                    item.MaxDurability = Math.Max(0, item.MaxDurability - (item.MaxDurability - item.CurrentDurability) / Globals.DurabilityLossRate);
                    item.CurrentDurability = item.MaxDurability;
                }
            }

            Connection.ReceiveChat((p.Special ? "NPC.NPCRepairSpecialResult" : "NPC.NPCRepairResult").Lang(Connection.Language, count, cost), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat((p.Special ? "NPC.NPCRepairSpecialResult" : "NPC.NPCRepairResult").Lang(con.Language, count, cost), MessageType.System);

            result.Success = true;

            if (p.GuildFunds)
            {
                Character.Account.GuildMember.Guild.GuildFunds -= cost;
                Character.Account.GuildMember.Guild.DailyGrowth -= cost;

                foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                {
                    member.Account.Connection?.Player?.Enqueue(new S.GuildFundsChanged { Change = -cost, ObserverPacket = false });
                    member.Account.Connection?.ReceiveChat("NPC.NPCRepairGuildResult".Lang(member.Account.Connection.Language, Name, cost, count), MessageType.System);
                }
            }
            else
            {
                Gold -= cost;
                GoldChanged();
                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "NPC特殊修理系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = cost,
                    ExtraInfo = $"NPC特殊修理扣除费用"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            if (refresh)
                RefreshStats();
        }

        #endregion

        #region NPC精炼石合成

        /// <summary>
        /// NPC精炼石合成
        /// </summary>
        /// <param name="p"></param>
        public void NPCRefinementStone(C.NPCRefinementStone p)
        {
            S.ItemsChanged result = new S.ItemsChanged
            {
                Links = new List<CellLinkInfo>()
            };
            Enqueue(result);

            if (p.IronOres != null) result.Links.AddRange(p.IronOres);
            if (p.SilverOres != null) result.Links.AddRange(p.SilverOres);
            if (p.DiamondOres != null) result.Links.AddRange(p.DiamondOres);
            if (p.GoldOres != null) result.Links.AddRange(p.GoldOres);
            if (p.Crystal != null) result.Links.AddRange(p.Crystal);

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.RefinementStone) return;

            if (SEnvir.RefinementStoneInfo == null)
            {
                return;
            }

            if (!ParseLinks(p.IronOres, 4, 4)) return;
            if (!ParseLinks(p.SilverOres, 4, 4)) return;
            if (!ParseLinks(p.DiamondOres, 4, 4)) return;
            if (!ParseLinks(p.GoldOres, 2, 2)) return;
            if (!ParseLinks(p.Crystal, 1, 1)) return;

            if (p.Gold < 0) return;

            if (p.Gold > Gold)
            {
                Connection.ReceiveChat("NPC.NPCRefinementGold".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementGold".Lang(con.Language), MessageType.System);
                return;
            }

            ItemCheck check = new ItemCheck(SEnvir.RefinementStoneInfo, 1, UserItemFlags.None, TimeSpan.Zero);
            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat("NPC.NPCRefinementStoneFailedRoom".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementStoneFailedRoom".Lang(con.Language), MessageType.System);
                return;
            }

            int ironPurity = 0;
            int silverPurity = 0;
            int diamondPurity = 0;
            int goldPurity = 0;

            foreach (CellLinkInfo link in p.IronOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.IronOre) return;

                ironPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.SilverOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.SilverOre) return;

                silverPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.DiamondOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Diamond) return;

                diamondPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.GoldOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.GoldOre) return;

                goldPurity += item.CurrentDurability;
            }
            foreach (CellLinkInfo link in p.Crystal)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Crystal) return;
            }

            long chance = p.Gold / 25000; // 250k / 10%, 2,500,000 for 100%

            chance += Math.Min(23, ironPurity / 4350); // 需要100 纯度
            chance += Math.Min(23, silverPurity / 3475); // 需要80 纯度
            chance += Math.Min(23, diamondPurity / 2600); //需要60 纯度
            chance += Math.Min(31, goldPurity / 1600); //需要50纯度

            foreach (CellLinkInfo link in p.IronOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.SilverOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.DiamondOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.GoldOres)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }
            foreach (CellLinkInfo link in p.Crystal)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            Gold -= p.Gold;
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC精炼石合成系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = p.Gold,
                ExtraInfo = $"NPC精炼石合成扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            result.Success = true;

            if (SEnvir.Random.Next(100) >= chance)
            {
                Connection.ReceiveChat("NPC.NPCRefinementStoneFailed".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementStoneFailed".Lang(con.Language), MessageType.System);
                return;
            }

            UserItem stone = SEnvir.CreateFreshItem(check);
            //记录物品来源
            SEnvir.RecordTrackingInfo(stone, NPC?.NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPC?.Name, Character?.CharacterName);
            GainItem(stone);
        }

        #endregion

        #region NCP武器精炼

        public int NPCRefine(RefineType refineType, int chance, int maxChance = 100)
        {
            if (Dead || NPC == null) return -1;
            List<CellLinkInfo> ores = new List<CellLinkInfo>();
            List<CellLinkInfo> items = new List<CellLinkInfo>();
            List<CellLinkInfo> specials = new List<CellLinkInfo>();
            S.NPCRefine result = new S.NPCRefine
            {
                RefineQuality = RefineQuality.Quick,
                RefineType = refineType,
                Ores = ores,
                Items = items,
                Specials = specials,
            };
            Enqueue(result);
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null || (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return -2;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return -3;
            RemoveItem(weapon);
            Equipment[(int)EquipmentSlot.Weapon] = null;

            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();

            info.Character = Character;
            info.Weapon = weapon;
            info.Chance = chance;
            info.MaxChance = maxChance;
            info.Quality = RefineQuality.Quick;
            info.Type = refineType;
            info.RetrieveTime = SEnvir.Now + Globals.RefineTimes[RefineQuality.Quick];

            result.Success = true;
            SendShapeUpdate();
            RefreshStats();

            // Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() } });

            //武器锻造(沙巴克升刀)事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerWeaponRefine(EventTypes.PlayerWeaponRefine,
                    new PlayerWeaponRefineEventArgs()));

            return 0;
        }

        /// <summary>
        /// NCP精炼
        /// </summary>
        /// <param name="p"></param>
        public void NPCRefine(C.NPCRefine p)
        {
            S.NPCRefine result = new S.NPCRefine
            {
                RefineQuality = p.RefineQuality,
                RefineType = p.RefineType,
                Ores = p.Ores,
                Items = p.Items,
                Specials = p.Specials,
            };
            Enqueue(result);

            switch (p.RefineQuality)
            {
                case RefineQuality.Rush:
                case RefineQuality.Quick:
                case RefineQuality.Standard:
                case RefineQuality.Careful:
                case RefineQuality.Precise:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "开始精炼，武器精炼时间".Lang(Connection.Language);
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            switch (p.RefineType)
            {
                case RefineType.Durability:
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "开始精炼，武器精炼属性".Lang(Connection.Language);
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.Refine) return;

            if (!ParseLinks(p.Ores, 1, 5))
            {
                Connection.ReceiveChat("NPC.NPCRefinementOres".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementOres".Lang(con.Language), MessageType.System);
                return;
            }
            if (!ParseLinks(p.Items, 1, 5))
            {
                Connection.ReceiveChat("NPC.NPCRefinementItems".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementItems".Lang(con.Language), MessageType.System);
                return;
            }
            if (!ParseLinks(p.Specials, 0, 1)) return;

            int RefineCost = 50000;

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null || (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (Gold < RefineCost)
            {
                Connection.ReceiveChat("NPC.NPCRefinementGold".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefinementGold".Lang(con.Language), MessageType.System);
                return;
            }

            int ore = 0;
            int items = 0;
            int quality = 0;
            int special = 0;
            //检查矿石

            foreach (CellLinkInfo link in p.Ores)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.BlackIronOre) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                ore += item.CurrentDurability;
            }

            foreach (CellLinkInfo link in p.Items)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                switch (item.Info.ItemType)
                {
                    case ItemType.Necklace:
                    case ItemType.Bracelet:
                    case ItemType.Ring:
                        break;
                    default:
                        return;
                }

                items += item.Info.RequiredAmount;

                if (item.Info.Rarity != Rarity.Common)
                    quality++;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 1) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                link.Count = 1;

                special += item.Info.Stats[Stat.MaxRefineChance];
            }

            /*
             * BaseChance  90% - Weapon Level
             * Max Chance  -5% | 0% | +5% | +10% | +20% = (Rush | Quick | Standard | Careful | Precise)  
             * 5 Ore 1% per 2 Dura Max
             * Items 1% per 6 Item Levels, 5% for Quality
             * Base Chance = 60% -Weapon Level  * 5%
             */

            int maxChance = 90 - weapon.Level + special;
            int chance = 60 - weapon.Level * 5;

            switch (p.RefineQuality)  //精炼质量
            {
                case RefineQuality.Rush:
                    maxChance -= 5;
                    break;
                case RefineQuality.Quick:
                    break;
                case RefineQuality.Standard:
                    maxChance += 2;
                    break;
                case RefineQuality.Careful:
                    maxChance += 5;
                    break;
                case RefineQuality.Precise:
                    maxChance += 10;
                    break;
                default:
                    return;
            }

            //Special + Max Chance

            chance += ore / 2000;
            chance += items / 6;
            chance += quality * 25;

            maxChance = Math.Min(100, maxChance);
            chance = Math.Min(maxChance, chance);

            foreach (CellLinkInfo link in p.Ores)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Items)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            RemoveItem(weapon);
            Equipment[(int)EquipmentSlot.Weapon] = null;

            Gold -= RefineCost;
            GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC武器精炼系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = RefineCost,
                ExtraInfo = $"NPC武器精炼扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();

            info.Character = Character;
            info.Weapon = weapon;
            info.Chance = chance;
            info.MaxChance = maxChance;
            info.Quality = p.RefineQuality;
            info.Type = p.RefineType;
            info.RetrieveTime = SEnvir.Now + Globals.RefineTimes[p.RefineQuality];

            result.Success = true;
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() } });

            //武器锻造(沙巴克升刀)事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerWeaponRefine(EventTypes.PlayerWeaponRefine,
                    new PlayerWeaponRefineEventArgs()));
        }

        #endregion

        #region NPC精炼武器取回

        public int PYNPCRefineRetrieve(int index = 0)
        {
            if (Dead || NPC == null) return -1;//无法取
            RefineInfo info = Character.Refines[0];

            if (info == null) return -2;//没有物品

            UserItem weapon = info.Weapon;
            weapon.Chance = info.Chance;
            weapon.RefineType = info.Type;
            weapon.IsRefine = true;

            weapon.Flags &= ~UserItemFlags.Refinable;

            weapon.Flags |= UserItemFlags.None;

            info.Weapon = null;
            info.Character = null;
            info.Delete();

            GainItem(weapon);
            return 0;
        }

        /// <summary>
        /// NPC精炼武器取回
        /// </summary>
        /// <param name="index"></param>
        public void NPCRefineRetrieve(int index)
        {
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.RefineRetrieve) return;

            RefineInfo info = Character.Refines.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (SEnvir.Now < info.RetrieveTime && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat("NPC.NPCRefineNotReady".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefineNotReady".Lang(con.Language), MessageType.System);
                return;
            }

            ItemCheck check = new ItemCheck(info.Weapon, info.Weapon.Count, info.Weapon.Flags, info.Weapon.ExpireTime);

            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat("NPC.NPCRefineNoRoom".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefineNoRoom".Lang(con.Language), MessageType.System);
                return;
            }

            UserItem weapon = info.Weapon;
            weapon.Chance = info.Chance;
            weapon.RefineType = info.Type;
            weapon.IsRefine = true;

            /*
                        if (SEnvir.Random.Next(100) < info.Chance)
                        {
                            switch (info.Type)
                            {
                                case RefineType.Durability:
                                    weapon.MaxDurability += 2000;
                                    break;
                                case RefineType.DC:
                                    weapon.AddStat(Stat.MaxDC, 1, StatSource.Refine);
                                    break;
                                case RefineType.SpellPower:
                                    if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                                    {
                                        weapon.AddStat(Stat.MaxMC, 1, StatSource.Refine);
                                        weapon.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                                    }

                                    if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                                        weapon.AddStat(Stat.MaxMC, 1, StatSource.Refine);

                                    if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                                        weapon.AddStat(Stat.MaxSC, 1, StatSource.Refine);
                                    break;
                                case RefineType.Fire:
                                    weapon.AddStat(Stat.FireAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Ice:
                                    weapon.AddStat(Stat.IceAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 2 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Lightning:
                                    weapon.AddStat(Stat.LightningAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 3 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Wind:
                                    weapon.AddStat(Stat.WindAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 4 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Holy:
                                    weapon.AddStat(Stat.HolyAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 5 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Dark:
                                    weapon.AddStat(Stat.DarkAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 6 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Phantom:
                                    weapon.AddStat(Stat.PhantomAttack, 1, StatSource.Refine);
                                    weapon.AddStat(Stat.WeaponElement, 7 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                    break;
                                case RefineType.Reset:
                                    weapon.Level = 1;
                                    weapon.ResetCoolDown = SEnvir.Now.AddDays(Config.ResetCoolDown);  //武器重置冷却时间

                                    weapon.MergeRefineElements(out Stat element);

                                    for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                    {
                                        UserItemStat stat = weapon.AddedStats[i];
                                        if (stat.StatSource != StatSource.Refine || stat.Stat == Stat.WeaponElement) continue;

                                        int amount = stat.Amount / Config.ResetAddValue;  //武器重置时保留的点数比率

                                        switch (weapon.Info.Rarity)  //按武器的类型走
                                        {
                                            case Rarity.Common:
                                                if (SEnvir.Random.Next(8) == 0)
                                                {
                                                    amount = SEnvir.Random.Next(amount);
                                                }
                                                break;
                                            case Rarity.Superior:
                                                if (SEnvir.Random.Next(5) == 0)
                                                {
                                                    amount = SEnvir.Random.Next(amount);
                                                }
                                                break;
                                            case Rarity.Elite:
                                                if (SEnvir.Random.Next(3) == 0)
                                                {
                                                    amount = SEnvir.Random.Next(amount);
                                                }
                                                break;
                                        }

                                        stat.Delete();
                                        weapon.AddStat(stat.Stat, amount, StatSource.Enhancement);
                                    }

                                    for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                    {
                                        UserItemStat stat = weapon.AddedStats[i];
                                        if (stat.StatSource != StatSource.Enhancement) continue;

                                        switch (stat.Stat)           //武器重置增加的属性
                                        {
                                            case Stat.MaxDC:
                                            case Stat.MaxMC:
                                            case Stat.MaxSC:
                                                stat.Amount = Math.Min(stat.Amount, Config.ResetStatValue);
                                                break;
                                            case Stat.FireAttack:
                                            case Stat.LightningAttack:
                                            case Stat.IceAttack:
                                            case Stat.WindAttack:
                                            case Stat.DarkAttack:
                                            case Stat.HolyAttack:
                                            case Stat.PhantomAttack:
                                                stat.Amount = Math.Min(stat.Amount, Config.ResetElementValue);
                                                break;   
                                            case Stat.EvasionChance:
                                            case Stat.BlockChance:
                                                stat.Amount = Math.Min(stat.Amount, Config.ResetExtraValue);
                                                break;
                                        }
                                    }
                                    break;
                            }
                            weapon.StatsChanged();

                            Connection.ReceiveChat("NPC.NPCRefineSuccess".Lang(Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("NPC.NPCRefineSuccess".Lang(con.Language), MessageType.System);
                        }
                        else
                        {
                            Connection.ReceiveChat("NPC.NPCRefineFailed".Lang(Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("NPC.NPCRefineFailed".Lang(con.Language), MessageType.System);
                        }
            */
            weapon.Flags &= ~UserItemFlags.Refinable;

            weapon.Flags |= UserItemFlags.None;

            Enqueue(new S.NPCRefineRetrieve { Index = info.Index });
            info.Weapon = null;
            info.Character = null;
            info.Delete();

            GainItem(weapon);
        }

        #endregion

        #region NPC武器重置

        /// <summary>
        /// NPC武器重置
        /// </summary>
        public void NPCResetWeapon()
        {
            //赋值 weapon 为装备栏武器道具
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];
            //删除手上的武器
            RemoveItem(weapon);
            //装备武器栏置空
            Equipment[(int)EquipmentSlot.Weapon] = null;
            //发包道具改变
            Enqueue(new S.ItemChanged
            {
                Link = new CellLinkInfo { Slot = (int)EquipmentSlot.Weapon, GridType = GridType.Equipment },
                Success = true   //成功
            });
            //精炼信息赋值创建新的道具对象
            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();
            //道具信息等原角色
            info.Character = Character;
            //道具信息等武器道具
            info.Weapon = weapon;
            //道具精炼几率100%
            info.Chance = 100;
            //道具最大精炼几率100%
            info.MaxChance = 100;
            //道具精炼时间精确
            info.Quality = RefineQuality.Precise;
            //道具精炼类别增加重置
            info.Type = RefineType.Reset;
            //道具赋值重置时间信息
            info.RetrieveTime = SEnvir.Now.AddMinutes(Config.ResetCoolTime);   //武器进行重置取回的等待时间
            //发送图像更新
            SendShapeUpdate();
            //刷新角色属性
            RefreshStats();
            //发包客户端精炼信息
            Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() } });

            //武器冶炼(沙巴克武器重置)事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerWeaponReset(EventTypes.PlayerWeaponReset,
                    new PlayerWeaponResetEventArgs()));
        }

        #endregion

        #region NPC转生

        /// <summary>
        /// NPC转生
        /// </summary>
        public void NPCRebirth()
        {
            Level = Math.Max(Config.RebirthLevel, 1);        //转生以后等级降为1级

            Experience = Experience / Math.Max(Config.RebirthExp, 1);  //转生后经验降低 2倍

            Enqueue(new S.LevelChanged { Level = Level, Experience = Experience });
            Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });

            Character.Rebirth++;

            Character.SpentPoints = 0;
            Character.HermitStats.Clear();

            RefreshStats();
        }

        #endregion

        #region NPC大师精炼

        /// <summary>
        /// NPC大师精炼
        /// </summary>
        /// <param name="p"></param>
        public void NPCMasterRefine(C.NPCMasterRefine p)
        {
            S.NPCMasterRefine result = new S.NPCMasterRefine
            {
                Fragment1s = p.Fragment1s,
                Fragment2s = p.Fragment2s,
                Fragment3s = p.Fragment3s,
                Stones = p.Stones,
                Specials = p.Specials,
            };
            Enqueue(result);

            switch (p.RefineType)
            {
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    Character.Account.Banned = true;
                    Character.Account.BanReason = "开始大师精炼，武器精炼类型".Lang(Connection.Language);
                    Character.Account.ExpiryDate = SEnvir.Now.AddYears(10);
                    return;
            }

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.MasterRefine) return;

            if (!ParseLinks(p.Fragment1s, 1, 1)) return;
            if (!ParseLinks(p.Fragment2s, 1, 1)) return;
            if (!ParseLinks(p.Fragment3s, 1, 1)) return;
            if (!ParseLinks(p.Stones, 1, 1)) return;
            if (!ParseLinks(p.Specials, 0, 1)) return;

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (weapon.Level != 17) return;

            long fragmentCount = 0;
            int special = 0;
            int fragmentRate = 2;
            //检查矿石

            foreach (CellLinkInfo link in p.Fragment1s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment1) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment2s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment2) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment3s)
            {
                if (link.Count < 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment3) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                fragmentCount += link.Count;
            }

            foreach (CellLinkInfo link in p.Stones)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.RefinementStone) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Specials)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 5) return;
                if (item.Count < link.Count) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                special += item.Info.Stats[Stat.MaxRefineChance];
                fragmentRate += item.Info.Stats[Stat.FragmentRate];
            }

            int maxChance = Config.MasterRefineChance + special;
            int statValue = 0;
            bool sucess = false;

            switch (p.RefineType)
            {
                case RefineType.DC:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.Stat != Stat.MaxDC || stat.StatSource != StatSource.Refine) continue;

                        statValue = stat.Amount;
                        break;
                    }

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, Config.MasterRefineChance - statValue * 4 + fragmentCount * fragmentRate);

                    if (sucess)
                    {
                        if (!Config.MasterRefineRandom)
                        {
                            weapon.AddStat(Stat.MaxDC, Config.MasterRefineCount, StatSource.Refine);
                        }
                        else
                        {
                            weapon.AddStat(Stat.MaxDC, SEnvir.Random.Next(1, Config.MasterRefineCount + 1), StatSource.Refine);
                        }
                    }
                    else
                    {
                        if (!Config.MasterRefineRandom)
                        {
                            weapon.AddStat(Stat.MaxDC, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                        }
                        else
                        {
                            weapon.AddStat(Stat.MaxDC, -Math.Min(statValue, SEnvir.Random.Next(1, Config.MasterRefineCount + 1)), StatSource.Refine);
                        }
                    }
                    break;
                case RefineType.SpellPower:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.StatSource != StatSource.Refine) continue;

                        if (stat.Stat != Stat.MaxMC && stat.Stat != Stat.MaxSC) continue;

                        statValue = Math.Max(statValue, stat.Amount);
                    }

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, Config.MasterRefineChance - statValue * 4 + fragmentCount * fragmentRate);

                    if (sucess)
                    {
                        if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxMC, Config.MasterRefineCount, StatSource.Refine);
                                weapon.AddStat(Stat.MaxSC, Config.MasterRefineCount, StatSource.Refine);
                            }
                            else
                            {
                                int a = SEnvir.Random.Next(1, Config.MasterRefineCount + 1);
                                weapon.AddStat(Stat.MaxMC, a, StatSource.Refine);
                                weapon.AddStat(Stat.MaxSC, a, StatSource.Refine);
                            }
                        }

                        if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxMC, Config.MasterRefineCount, StatSource.Refine);
                            }
                            else
                            {
                                weapon.AddStat(Stat.MaxMC, SEnvir.Random.Next(1, Config.MasterRefineCount + 1), StatSource.Refine);
                            }
                        }

                        if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxSC, Config.MasterRefineCount, StatSource.Refine);
                            }
                            else
                            {
                                weapon.AddStat(Stat.MaxSC, SEnvir.Random.Next(1, Config.MasterRefineCount + 1), StatSource.Refine);
                            }
                        }
                    }
                    else
                    {
                        if (weapon.Info.Stats[Stat.MinMC] == 0 && weapon.Info.Stats[Stat.MaxMC] == 0 && weapon.Info.Stats[Stat.MinSC] == 0 && weapon.Info.Stats[Stat.MaxSC] == 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                                weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                            }
                            else
                            {
                                int a = SEnvir.Random.Next(1, Config.MasterRefineCount + 1);
                                weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, a), StatSource.Refine);
                                weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, a), StatSource.Refine);
                            }
                        }

                        if (weapon.Info.Stats[Stat.MinMC] > 0 || weapon.Info.Stats[Stat.MaxMC] > 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                            }
                            else
                            {
                                weapon.AddStat(Stat.MaxMC, -Math.Min(statValue, SEnvir.Random.Next(1, Config.MasterRefineCount + 1)), StatSource.Refine);
                            }
                        }

                        if (weapon.Info.Stats[Stat.MinSC] > 0 || weapon.Info.Stats[Stat.MaxSC] > 0)
                        {
                            if (!Config.MasterRefineRandom)
                            {
                                weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                            }
                            else
                            {
                                weapon.AddStat(Stat.MaxSC, -Math.Min(statValue, SEnvir.Random.Next(1, Config.MasterRefineCount + 1)), StatSource.Refine);
                            }
                        }
                    }
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    statValue = weapon.MergeRefineElements(out Stat element);

                    sucess = SEnvir.Random.Next(100) < Math.Min(maxChance, Config.MasterRefineChance - statValue * 4 + fragmentCount * fragmentRate);

                    if (element == Stat.None)
                        element = Stat.FireAttack; //Could be any

                    if (sucess)
                    {
                        weapon.AddStat(element, Config.MasterRefineCount, StatSource.Refine);
                        switch (p.RefineType)
                        {
                            case RefineType.Fire:
                                weapon.AddStat(Stat.WeaponElement, 1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Ice:
                                weapon.AddStat(Stat.WeaponElement, 2 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Lightning:
                                weapon.AddStat(Stat.WeaponElement, 3 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Wind:
                                weapon.AddStat(Stat.WeaponElement, 4 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Holy:
                                weapon.AddStat(Stat.WeaponElement, 5 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Dark:
                                weapon.AddStat(Stat.WeaponElement, 6 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                            case RefineType.Phantom:
                                weapon.AddStat(Stat.WeaponElement, 7 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                                break;
                        }
                    }
                    else
                        weapon.AddStat(element, -Math.Min(statValue, Config.MasterRefineCount), StatSource.Refine);
                    break;
            }


            foreach (CellLinkInfo link in p.Fragment1s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Fragment2s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Fragment3s)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Stones)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            foreach (CellLinkInfo link in p.Specials)
            {
                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                UserItem item = array[link.Slot];

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    array[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            result.Success = true;

            Connection.ReceiveChat((sucess ? "NPC.NPCRefineSuccess" : "NPC.NPCRefineFailed").Lang(Connection.Language), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat((sucess ? "NPC.NPCRefineSuccess" : "NPC.NPCRefineFailed").Lang(con.Language), MessageType.System);

            weapon.StatsChanged();
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });
        }

        #endregion

        #region NPC特殊精炼

        /// <summary>
        /// NPC特殊精炼
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="amount"></param>
        public void NPCSpecialRefine(Stat stat, int amount)
        {
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if (weapon.Level != 17) return;

            weapon.AddStat(stat, amount, StatSource.Refine);

            Connection.ReceiveChat("NPC.NPCRefineSuccess".Lang(Connection.Language), MessageType.System);

            foreach (SConnection con in Connection.Observers)
                con.ReceiveChat("NPC.NPCRefineSuccess".Lang(con.Language), MessageType.System);

            weapon.StatsChanged();
            SendShapeUpdate();
            RefreshStats();

            Enqueue(new S.ItemStatsRefreshed { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats(weapon.Stats), FullItemStats = weapon.ToClientInfo().FullItemStats });
        }

        #endregion

        #region NPC大师精炼评估

        /// <summary>
        /// NPC大师精炼评估
        /// </summary>
        /// <param name="p"></param>
        public void NPCMasterRefineEvaluate(C.NPCMasterRefineEvaluate p)
        {
            switch (p.RefineType)
            {
                case RefineType.DC:
                case RefineType.SpellPower:
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    break;
                default:
                    return;
            }

            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.MasterRefine) return;

            if (!ParseLinks(p.Fragment1s, 1, 1)) return;
            if (!ParseLinks(p.Fragment2s, 1, 1)) return;
            if (!ParseLinks(p.Fragment3s, 1, 1)) return;
            if (!ParseLinks(p.Stones, 1, 1)) return;
            if (!ParseLinks(p.Specials, 0, 1)) return;

            if (Gold < Globals.MasterRefineEvaluateCost)
            {
                Connection.ReceiveChat("NPC.NPCMasterRefineGold".Lang(Connection.Language, Globals.MasterRefineEvaluateCost), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCMasterRefineGold".Lang(con.Language, Globals.MasterRefineEvaluateCost), MessageType.System);
                return;
            }

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon == null) return;

            if ((weapon.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

            if (weapon.Level != 17) return;

            long fragmentCount = 0;
            int special = 0;
            int fragmentRate = 2;
            //Check Ores

            foreach (CellLinkInfo link in p.Fragment1s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment1) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment2s)
            {
                if (link.Count != 10) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment2) return;
                if (item.Count < 10) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Fragment3s)
            {
                if (link.Count < 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment3) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                fragmentCount += link.Count;
            }
            foreach (CellLinkInfo link in p.Stones)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.RefinementStone) return;
                if (item.Count < link.Count) return;

                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;
            }
            foreach (CellLinkInfo link in p.Specials)
            {
                if (link.Count != 1) return;

                UserItem[] array;
                switch (link.GridType)
                {
                    case GridType.Inventory:
                        array = Inventory;
                        break;
                    case GridType.Storage:
                        array = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;

                        array = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.ItemType != ItemType.RefineSpecial) return;

                if (item.Info.Shape != 5) return;
                if (item.Count < link.Count) return;
                if ((item.Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable) return;

                special += item.Info.Stats[Stat.MaxRefineChance];
                fragmentRate += item.Info.Stats[Stat.FragmentRate];
            }

            int maxChance = 80 + special;
            int statValue = 0;
            bool sucess = false;

            switch (p.RefineType)
            {
                case RefineType.DC:

                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.Stat != Stat.MaxDC || stat.StatSource != StatSource.Refine) continue;

                        statValue = stat.Amount;
                        break;
                    }

                    Connection.ReceiveChat("NPC.NPCMasterRefineChance".Lang(Connection.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.NPCMasterRefineChance".Lang(con.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
                case RefineType.SpellPower:
                    foreach (UserItemStat stat in weapon.AddedStats)
                    {
                        if (stat.StatSource != StatSource.Refine) continue;

                        if (stat.Stat != Stat.MaxMC && stat.Stat != Stat.MaxSC) continue;

                        statValue = Math.Max(statValue, stat.Amount);
                    }
                    Connection.ReceiveChat("NPC.NPCMasterRefineChance".Lang(Connection.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.NPCMasterRefineChance".Lang(con.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    statValue = weapon.MergeRefineElements(out Stat element);
                    weapon.StatsChanged();

                    sucess = SEnvir.Random.Next(100) >= Math.Min(maxChance, 80 - statValue * 4 + fragmentCount * 2);
                    Connection.ReceiveChat("NPC.NPCMasterRefineChance".Lang(Connection.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("NPC.NPCMasterRefineChance".Lang(con.Language, Math.Min(maxChance, Math.Max(80 - statValue * 4 + fragmentCount * fragmentRate, 0))), MessageType.System);
                    break;
            }

            Gold -= Globals.MasterRefineEvaluateCost;
            GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC精炼评估系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = Globals.MasterRefineEvaluateCost,
                ExtraInfo = $"NPC精炼评估系统扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
        }

        #endregion

        #region NPC武器工艺

        /// <summary>
        /// NPC武器工艺
        /// </summary>
        /// <param name="p"></param>
        public void NPCWeaponCraft(C.NPCWeaponCraft p)
        {
            S.NPCWeaponCraft result = new S.NPCWeaponCraft
            {
                Template = p.Template,
                Yellow = p.Yellow,
                Blue = p.Blue,
                Red = p.Red,
                Purple = p.Purple,
                Green = p.Green,
                Grey = p.Grey,
            };
            Enqueue(result);

            int statCount = 0;

            bool isTemplate = false;

            #region Tempate Check
            //临时检查
            if (p.Template == null) return;

            if (p.Template.GridType != GridType.Inventory) return;

            if (p.Template.Slot < 0 || p.Template.Slot >= Inventory.Length) return;

            if (p.Template.Count != 1) return;

            if (Inventory[p.Template.Slot] == null) return;

            if (Inventory[p.Template.Slot].Info.Effect == ItemEffect.WeaponTemplate)
            {
                isTemplate = true;
            }
            else if (Inventory[p.Template.Slot].Info.ItemType != ItemType.Weapon || Inventory[p.Template.Slot].Info.Effect == ItemEffect.SpiritBlade) return;

            #endregion

            long cost = Globals.CraftWeaponPercentCost;        //修复武器工艺丢钱可以制作的BUG

            if (cost > Gold)
            {
                Connection.ReceiveChat("NPC.NPCRefinementGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!isTemplate)
            {
                switch (Inventory[p.Template.Slot].Info.Rarity)
                {
                    case Rarity.Common:
                        cost = Globals.CommonCraftWeaponPercentCost;
                        break;
                    case Rarity.Superior:
                        cost = Globals.SuperiorCraftWeaponPercentCost;
                        break;
                    case Rarity.Elite:
                        cost = Globals.EliteCraftWeaponPercentCost;
                        break;
                }
            }

            #region Yellow Check

            if (p.Yellow != null)
            {
                if (p.Yellow.GridType != GridType.Inventory) return;

                if (p.Yellow.Slot < 0 || p.Yellow.Slot >= Inventory.Length) return;

                if (p.Yellow.Count != 1) return;

                if (Inventory[p.Yellow.Slot] == null || Inventory[p.Yellow.Slot].Info.Effect != ItemEffect.YellowSlot) return;

                statCount += Inventory[p.Yellow.Slot].Info.Shape;
            }

            #endregion

            #region Blue Check

            if (p.Blue != null)
            {
                if (p.Blue.GridType != GridType.Inventory) return;

                if (p.Blue.Slot < 0 || p.Blue.Slot >= Inventory.Length) return;

                if (p.Blue.Count != 1) return;

                if (Inventory[p.Blue.Slot] == null || Inventory[p.Blue.Slot].Info.Effect != ItemEffect.BlueSlot) return;

                statCount += Inventory[p.Blue.Slot].Info.Shape;
            }

            #endregion

            #region Red Check

            if (p.Red != null)
            {
                if (p.Red.GridType != GridType.Inventory) return;

                if (p.Red.Slot < 0 || p.Red.Slot >= Inventory.Length) return;

                if (p.Red.Count != 1) return;

                if (Inventory[p.Red.Slot] == null || Inventory[p.Red.Slot].Info.Effect != ItemEffect.RedSlot) return;

                statCount += Inventory[p.Red.Slot].Info.Shape;
            }

            #endregion

            #region Purple Check

            if (p.Purple != null)
            {
                if (p.Purple.GridType != GridType.Inventory) return;

                if (p.Purple.Slot < 0 || p.Purple.Slot >= Inventory.Length) return;

                if (p.Purple.Count != 1) return;

                if (Inventory[p.Purple.Slot] == null || Inventory[p.Purple.Slot].Info.Effect != ItemEffect.PurpleSlot) return;

                statCount += Inventory[p.Purple.Slot].Info.Shape;
            }

            #endregion

            #region Green Check

            if (p.Green != null)
            {
                if (p.Green.GridType != GridType.Inventory) return;

                if (p.Green.Slot < 0 || p.Green.Slot >= Inventory.Length) return;

                if (p.Green.Count != 1) return;

                if (Inventory[p.Green.Slot] == null || Inventory[p.Green.Slot].Info.Effect != ItemEffect.GreenSlot) return;

                statCount += Inventory[p.Green.Slot].Info.Shape;
            }

            #endregion

            #region Grey Check

            if (p.Grey != null)
            {
                if (p.Grey.GridType != GridType.Inventory) return;

                if (p.Grey.Slot < 0 || p.Grey.Slot >= Inventory.Length) return;

                if (p.Grey.Count != 1) return;

                if (Inventory[p.Grey.Slot] == null || Inventory[p.Grey.Slot].Info.Effect != ItemEffect.GreySlot) return;

                statCount += Inventory[p.Grey.Slot].Info.Shape;
            }

            #endregion

            ItemInfo weap = null;

            if (isTemplate)
            {
                switch (p.Class)
                {
                    case RequiredClass.Warrior:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.WarriorWeapon);
                        break;
                    case RequiredClass.Wizard:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.WizardWeapon);
                        break;
                    case RequiredClass.Taoist:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.TaoistWeapon);
                        break;
                    case RequiredClass.Assassin:
                        weap = SEnvir.ItemInfoList.Binding.First(x => x.Effect == ItemEffect.AssassinWeapon);
                        break;
                    default:
                        return;
                }

                if (!CanGainItems(false, new ItemCheck(weap, 1, UserItemFlags.None, TimeSpan.Zero)))
                {
                    Connection.ReceiveChat("没有足够的包裹空间".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            result.Success = true;

            UserItem item;

            #region Tempate

            if (isTemplate)
            {
                item = Inventory[p.Template.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Template.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }
            #endregion

            #region Yellow

            if (p.Yellow != null)
            {
                item = Inventory[p.Yellow.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Yellow.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Blue

            if (p.Blue != null)
            {
                item = Inventory[p.Blue.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Blue.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Red

            if (p.Red != null)
            {
                item = Inventory[p.Red.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Red.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Purple

            if (p.Purple != null)
            {
                item = Inventory[p.Purple.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Purple.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Green

            if (p.Green != null)
            {
                item = Inventory[p.Green.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Green.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            #region Grey

            if (p.Grey != null)
            {
                item = Inventory[p.Grey.Slot];
                if (item.Count == 1)
                {
                    RemoveItem(item);
                    Inventory[p.Grey.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= 1;
            }

            #endregion

            Gold -= cost;
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC武器工艺系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = cost,
                ExtraInfo = $"NPC武器工艺扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            int total = 0;

            foreach (WeaponCraftStatInfo stat in SEnvir.WeaponCraftStatInfoList.Binding)
            {
                if ((stat.RequiredClass & p.Class) != p.Class) continue;

                total += stat.Weight;
            }

            if (isTemplate)
            {
                item = SEnvir.CreateFreshItem(weap);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, NPC?.NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPC?.Name, Character?.CharacterName);
            }
            else
            {
                item = Inventory[p.Template.Slot];

                RemoveItem(item);
                Inventory[p.Template.Slot] = null;

                item.Level = 1;
                item.Flags &= ~UserItemFlags.Refinable;

                for (int i = item.AddedStats.Count - 1; i >= 0; i--)
                {
                    UserItemStat stat = item.AddedStats[i];
                    if (stat.StatSource == StatSource.Enhancement) continue;

                    stat.Delete();
                }

                item.StatsChanged();
            }

            for (int i = 0; i < statCount; i++)
            {
                int value = SEnvir.Random.Next(total);

                foreach (WeaponCraftStatInfo stat in SEnvir.WeaponCraftStatInfoList.Binding)
                {
                    if ((stat.RequiredClass & p.Class) != p.Class) continue;

                    value -= stat.Weight;

                    if (value >= 0) continue;

                    item.AddStat(stat.Stat, SEnvir.Random.Next(stat.MinValue, stat.MaxValue + 1), StatSource.Added);
                    break;
                }
            }

            item.StatsChanged();

            GainItem(item);
        }

        #endregion

        #region NPC技能书合成

        /// <summary>
        /// NPC技能书合成
        /// </summary>
        /// <param name="p"></param>
        public void NPCBookRefine(C.NPCBookRefine p)
        {
            //如果死亡 NPC为空 NPC页面为空 NPC类型不是技能书合成 跳出
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.BookCombine) return;

            if (!ParseLinks(p.OriginalBook)) return;
            if (!ParseLinks(p.Material, 1, 2)) return; // 后面增加多合一时，这里要改

            if (p.OriginalBook.Count != 1) return;
            if (p.Material.Count < 1) return;

            UserItem[] targetArray = GetUserItemArrayFromLink(p.OriginalBook.GridType);
            if (targetArray == null) return;

            if (p.OriginalBook.Slot < 0 || p.OriginalBook.Slot >= targetArray.Length) return;

            UserItem targetItem = targetArray[p.OriginalBook.Slot];

            if (targetItem == null || p.OriginalBook.Count > targetItem.Count) return;

            if (targetItem.IsTemporary) return;
            if (targetItem.CurrentDurability > 99) return;   //书的页面大于99 跳出
            if (targetItem.Info.ItemType != ItemType.Book) return;  //道具类型不是技能书 跳出

            //计算合成费用
            int cost = targetItem.Info.RequiredAmount * 1000; //技能书要求等级*1000
            foreach (CellLinkInfo link in p.Material)
            {
                UserItem[] fromArray = GetUserItemArrayFromLink(link.GridType);
                if (fromArray == null) continue;

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) continue;
                if (item.Info.ItemType != ItemType.Book) continue;
                if (item.Info.ItemName != targetItem.Info.ItemName) continue;

                cost += item.CurrentDurability * Globals.BookCombineFeePerDurability;
            }

            if (cost > Gold)
            {
                Connection.ReceiveChat("你没有足够的金币来合成这些书".Lang(Connection.Language), MessageType.System);
                return;
            }

            Gold -= Math.Abs(cost);
            GoldChanged();

            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "NPC技能书合成系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = Math.Abs(cost),
                ExtraInfo = $"NPC技能书合成扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            // 暂时再过一遍
            int finalDurability = targetItem.CurrentDurability;

            S.ItemsChanged result = new S.ItemsChanged { Links = new List<CellLinkInfo>(), Success = true };
            Enqueue(result);

            foreach (CellLinkInfo link in p.Material)
            {
                UserItem[] fromArray = GetUserItemArrayFromLink(link.GridType);
                if (fromArray == null) continue;

                if (link.Slot < 0 || link.Slot >= fromArray.Length) continue;
                UserItem item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count || (item.Flags & UserItemFlags.Locked) == UserItemFlags.Locked) continue;
                if (item.Info.ItemType != ItemType.Book) continue;
                if (item.Info.ItemName != targetItem.Info.ItemName) continue;

                finalDurability += item.CurrentDurability;

                result.Links.Add(link);

                if (item.Count == link.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                    item.Delete();
                }
                else
                    item.Count -= link.Count;
            }

            targetItem.CurrentDurability = Math.Min(100, finalDurability);

            Enqueue(new S.ItemDurability
            {
                GridType = p.OriginalBook.GridType,
                Slot = p.OriginalBook.Slot,
                CurrentDurability = targetItem.CurrentDurability,
            });

            Enqueue(new S.ItemChanged { Link = p.OriginalBook, Success = true });
            RefreshWeight();
            RefreshStats();
        }

        #endregion

        #region NPC附魔石合成(未完成)

        /// <summary>
        /// NPC附魔石合成
        /// </summary>
        /// <param name="p"></param>
        public void NPCEnchantmentSynthesis(C.NPCEnchantmentSynthesis p)
        {
            //如果死亡 NPC为空 NPC页面为空 NPC类型不是附魔石合成 跳出
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.EnchantmentSynthesis) return;

            if (!ParseLinks(p.MaterialGrid1, 1, 1)) return;
            if (!ParseLinks(p.MaterialGrid2, 1, 1)) return;
            if (!ParseLinks(p.MaterialGrid3, 1, 1)) return;

            if (p.MaterialGrid1.Count < 1 && p.MaterialGrid2.Count < 1) return;
            if (p.MaterialGrid2.Count < 1 && p.MaterialGrid3.Count < 1) return;
            if (p.MaterialGrid3.Count < 1 && p.MaterialGrid1.Count < 1) return;

        }
        #endregion


        #endregion

        #region helper
        public UserItem[] GetUserItemArrayFromLink(GridType grid)
        {
            UserItem[] itemArray = null; ;
            switch (grid)
            {
                case GridType.Inventory:    //背包
                    itemArray = Inventory;
                    break;
                case GridType.PatchGrid:    //碎片包裹
                    itemArray = PatchGrid;
                    break;
                case GridType.Storage:    //仓库
                    itemArray = Storage;
                    break;
                case GridType.CompanionInventory:  //宠物包裹
                    if (Companion == null) return null;

                    itemArray = Companion.Inventory;
                    break;
                default:
                    return null;
            }
            return itemArray;
        }

        public UserItem GetUserItemFromLink(CellLinkInfo link)
        {
            if (link == null) return null;
            UserItem[] array;
            switch (link.GridType)
            {
                case GridType.Inventory:
                    array = Inventory;
                    break;
                case GridType.Storage:
                    array = Storage;
                    break;
                case GridType.CompanionInventory:
                    if (Companion == null) return null;
                    array = Companion.Inventory;
                    break;
                default:
                    return null;
            }
            if (link.Slot < 0 || link.Slot >= array.Length) return null;
            return array[link.Slot];
        }
        #endregion

        #region 脚本快捷按钮

        /// <summary>
        /// 加载快捷方式
        /// </summary>
        public void LoadShortcutConfig()
        {
            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                    IronPython.Runtime.List IconList = SEnvir.ExecutePyWithTimer(trig_play, this, "OnShortcutDialogClicked", args);
                    //IronPython.Runtime.List IconList = trig_play(this, "OnShortcutDialogClicked", args);

                    List<int> icons = new List<int>();
                    if (IconList != null && IconList.Count > 0)
                    {
                        foreach (PythonTuple iconTuple in IconList)
                        {
                            if (iconTuple == null || iconTuple.Count != 4) continue;
                            icons.Add((int)iconTuple[0]);
                            icons.Add((int)iconTuple[1]);
                            icons.Add((int)iconTuple[2]);
                            icons.Add((int)iconTuple[3]);
                            Enqueue(new S.ShortcutsLoaded { Shortcuts = icons });
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
        }

        #endregion

        #region 新版武器升级

        public void WeaponUpgrade(C.NPCWeaponUpgrade p)
        {
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.WeaponUpgrade) return;
            if (p.refineType != RefineType.DC && p.refineType != RefineType.SpellPower && p.refineType != RefineType.Durability && p.refineType != RefineType.Fire
               && p.refineType != RefineType.Ice && p.refineType != RefineType.Lightning && p.refineType != RefineType.Wind
               && p.refineType != RefineType.Holy && p.refineType != RefineType.Dark && p.refineType != RefineType.Phantom) return;

            if (Character.Refines.Where(x => x.IsNewWeaponUpgrade).ToList().Count > 0)
            {
                Connection.ReceiveChat($"你还有未取回的武器".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!ParseLinks(p.weapon)) return;

            UserItem weapon = GetUserItemFromLink(p.weapon);
            if (weapon == null || weapon.Info.ItemType != ItemType.Weapon) return;

            if (!ParseLinks(p.iron, 0, 5)) return;
            if (!ParseLinks(p.frag1, 0, 1)) return;
            if (!ParseLinks(p.frag2, 0, 1)) return;
            if (!ParseLinks(p.frag3, 0, 1)) return;
            if (!ParseLinks(p.stone, 0, 1)) return;

            int level = 0;
            int maxAddAllowed = Globals.WeaponUpgradeInfoList.Binding.Max(x => x.Increment);
            List<Stat> statsToAdd = new List<Stat>();
            switch (p.refineType)
            {
                case RefineType.DC:
                    level = weapon.GetTotalAddedStat(Stat.MaxDC) + 1;
                    statsToAdd.Add(Stat.MaxDC);
                    break;
                case RefineType.SpellPower:
                    int mc = weapon.GetTotalStat(Stat.MaxMC);
                    int sc = weapon.GetTotalStat(Stat.MaxSC);
                    if (mc == sc)
                    {
                        statsToAdd.Add(Stat.MaxSC);
                        statsToAdd.Add(Stat.MaxMC);
                        level = Math.Max(weapon.GetTotalAddedStat(Stat.MaxSC), weapon.GetTotalAddedStat(Stat.MaxMC)) + 1;
                    }
                    else if (mc > sc)
                    {
                        statsToAdd.Add(Stat.MaxMC);
                        level = weapon.GetTotalAddedStat(Stat.MaxMC) + 1;
                    }
                    else
                    {
                        statsToAdd.Add(Stat.MaxSC);
                        level = weapon.GetTotalAddedStat(Stat.MaxSC) + 1;
                    }
                    break;
                case RefineType.Durability:
                    //持久无限升级
                    level = 0;
                    break;
                case RefineType.Fire:
                case RefineType.Ice:
                case RefineType.Lightning:
                case RefineType.Wind:
                case RefineType.Holy:
                case RefineType.Dark:
                case RefineType.Phantom:
                    ClientUserItem clientWeapon = weapon.ToClientInfo();
                    Stat elementStat = clientWeapon.AddedStats.GetWeaponElement();
                    if (elementStat == Stat.None)
                    {
                        statsToAdd.Add(Functions.ElementToStat(Functions.RefineTypeToElement(p.refineType)));
                    }
                    else
                    {
                        statsToAdd.Add(clientWeapon.AddedStats.GetWeaponElement());
                    }
                    level = clientWeapon.AddedStats.GetWeaponElementValue() + 1;
                    break;
            }

            if (level > maxAddAllowed)
            {
                Connection.ReceiveChat($"此武器已经达到升级上限, 无法再升级了".Lang(Connection.Language), MessageType.System);
                return;
            }

            #region 检查材料

            Dictionary<UserItem, CellLinkInfo> ironList = new Dictionary<UserItem, CellLinkInfo>();
            foreach (CellLinkInfo link in p.iron)
            {
                if (link.Count < 1) return;

                UserItem[] array = GetUserItemArrayFromLink(link.GridType);
                if (array == null) return;
                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.BlackIronOre) return;
                if (item.Count < 1) return;
                ironList.Add(item, link);
            }

            Dictionary<UserItem, CellLinkInfo> frag1List = new Dictionary<UserItem, CellLinkInfo>();
            foreach (CellLinkInfo link in p.frag1)
            {
                if (link.Count < 1) return;

                UserItem[] array = GetUserItemArrayFromLink(link.GridType);
                if (array == null) return;
                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment1) return;
                if (item.Count < 1) return;
                frag1List.Add(item, link);
            }

            Dictionary<UserItem, CellLinkInfo> frag2List = new Dictionary<UserItem, CellLinkInfo>();
            foreach (CellLinkInfo link in p.frag2)
            {
                if (link.Count < 1) return;

                UserItem[] array = GetUserItemArrayFromLink(link.GridType);
                if (array == null) return;
                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment2) return;
                if (item.Count < 1) return;
                frag2List.Add(item, link);
            }

            Dictionary<UserItem, CellLinkInfo> frag3List = new Dictionary<UserItem, CellLinkInfo>();
            foreach (CellLinkInfo link in p.frag3)
            {
                if (link.Count < 1) return;

                UserItem[] array = GetUserItemArrayFromLink(link.GridType);
                if (array == null) return;
                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.Fragment3) return;
                if (item.Count < 1) return;
                frag3List.Add(item, link);
            }

            Dictionary<UserItem, CellLinkInfo> stoneList = new Dictionary<UserItem, CellLinkInfo>();
            foreach (CellLinkInfo link in p.stone)
            {
                if (link.Count < 1) return;

                UserItem[] array = GetUserItemArrayFromLink(link.GridType);
                if (array == null) return;
                if (link.Slot < 0 || link.Slot >= array.Length) return;
                UserItem item = array[link.Slot];

                if (item == null || item.Info.Effect != ItemEffect.RefinementStone) return;
                if (item.Count < 1) return;
                stoneList.Add(item, link);
            }



            #endregion

            int goldRequired, ironRequired, frag1Required, frag2Required, frag3Required, stoneRequired;
            WeaponUpgradeNew upgradeInfo = Globals.WeaponUpgradeInfoList.Binding.FirstOrDefault(x => level == x.Increment);
            if (p.refineType == RefineType.Durability)
            {
                goldRequired = 100000;
                ironRequired = 5;
                frag1Required = 0;
                frag2Required = 0;
                frag3Required = 0;
                stoneRequired = 1;
            }
            else
            {
                if (upgradeInfo == null) return;

                goldRequired = upgradeInfo.SpendGold;
                ironRequired = upgradeInfo.BlackIronOre;
                frag1Required = upgradeInfo.BasicFragment;
                frag2Required = upgradeInfo.AdvanceFragment;
                frag3Required = upgradeInfo.SeniorFragment;
                stoneRequired = upgradeInfo.RefinementStone;
            }

            if (ironRequired > ironList.Count)
            {
                Connection.ReceiveChat($"PlayerObject.ironRequired".Lang(Connection.Language, ironRequired), MessageType.System);
                return;
            }
            if (frag1Required > frag1List.Keys.Sum(x => x.Count))
            {
                Connection.ReceiveChat($"PlayerObject.frag1Required".Lang(Connection.Language, frag1Required), MessageType.System);
                return;
            }
            if (frag2Required > frag2List.Keys.Sum(x => x.Count))
            {
                Connection.ReceiveChat($"PlayerObject.frag2Required".Lang(Connection.Language, frag2Required), MessageType.System);
                return;
            }
            if (frag3Required > frag3List.Keys.Sum(x => x.Count))
            {
                Connection.ReceiveChat($"PlayerObject.frag3Required".Lang(Connection.Language, frag3Required), MessageType.System);
                return;
            }
            if (stoneRequired > stoneList.Keys.Sum(x => x.Count))
            {
                Connection.ReceiveChat($"PlayerObject.stoneRequired".Lang(Connection.Language, stoneRequired), MessageType.System);
                return;
            }
            if (goldRequired > Gold)
            {
                Connection.ReceiveChat($"PlayerObject.goldRequired".Lang(Connection.Language, goldRequired), MessageType.System);
                return;
            }

            //加属性
            bool success = false;
            bool statAdded = false;
            if (p.refineType != RefineType.Durability)
            {
                if (upgradeInfo == null) return;
                if (SEnvir.Random.Next(100) < upgradeInfo.SuccessRate)
                {
                    foreach (Stat stat in statsToAdd)
                    {
                        weapon.AddStat(stat, 1, StatSource.Refine);
                    }

                    statAdded = true;
                    success = true;
                }
                else
                {
                    statAdded = false;
                    success = SEnvir.Random.Next(100) >= upgradeInfo.FailureRate;
                }
            }
            else
            {
                statAdded = true;
                success = true;
                int ironPurity = ironList.Keys.Sum(x => x.CurrentDurability) / 2000;
                if (SEnvir.Random.Next(100) < ironPurity)
                {
                    weapon.MaxDurability += 1000;
                }
                if (SEnvir.Random.Next(100) < ironPurity)
                {
                    weapon.MaxDurability += 1000;
                }
                if (SEnvir.Random.Next(100) < ironPurity)
                {
                    weapon.MaxDurability += 1000;
                }
            }
            weapon.StatsChanged();

            RefineInfo info = SEnvir.RefineInfoList.CreateNewObject();

            info.Character = Character;
            info.Weapon = weapon;
            info.Chance = success ? 100 : 0;
            info.MaxChance = statAdded ? 100 : 0;
            info.Quality = RefineQuality.Standard;
            info.Type = p.refineType;
            info.IsNewWeaponUpgrade = true;
            info.RetrieveTime = SEnvir.Now +
                (upgradeInfo == null ? TimeSpan.Zero : TimeSpan.FromMinutes(upgradeInfo.TimeCost));

            Enqueue(new S.RefineList { List = new List<ClientRefineInfo> { info.ToClientInfo() }, IsNewWeaponUpgrade = true });


            //扣除材料
            RemoveItem(weapon);
            GetUserItemArrayFromLink(p.weapon.GridType)[p.weapon.Slot] = null;
            ChangeGold(-goldRequired);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "新版武器升级系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = goldRequired,
                ExtraInfo = $"新版武器升级扣除费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);
            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in ironList)
            {
                switch (kvp.Value.GridType)
                {
                    case GridType.Inventory:
                        Inventory[kvp.Value.Slot] = null;
                        break;
                    case GridType.Storage:
                        Storage[kvp.Value.Slot] = null;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;
                        Companion.Inventory[kvp.Value.Slot] = null;
                        break;
                    default:
                        SEnvir.Log("道具删除出错");
                        return;
                }
                RemoveItem(kvp.Key);
                kvp.Key.Delete();
            }

            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in frag1List)
            {
                if (frag1Required == kvp.Key.Count)
                {
                    RemoveItem(kvp.Key);
                    switch (kvp.Value.GridType)
                    {
                        case GridType.Inventory:
                            Inventory[kvp.Value.Slot] = null;
                            break;
                        case GridType.Storage:
                            Storage[kvp.Value.Slot] = null;
                            break;
                        case GridType.CompanionInventory:
                            if (Companion == null) return;
                            Companion.Inventory[kvp.Value.Slot] = null;
                            break;
                        default:
                            SEnvir.Log("道具删除出错");
                            return;
                    }
                    kvp.Key.Delete();
                }
                else
                {
                    kvp.Key.Count -= frag1Required;
                }
            }

            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in frag2List)
            {
                if (frag2Required == kvp.Key.Count)
                {
                    RemoveItem(kvp.Key);
                    switch (kvp.Value.GridType)
                    {
                        case GridType.Inventory:
                            Inventory[kvp.Value.Slot] = null;
                            break;
                        case GridType.Storage:
                            Storage[kvp.Value.Slot] = null;
                            break;
                        case GridType.CompanionInventory:
                            if (Companion == null) return;
                            Companion.Inventory[kvp.Value.Slot] = null;
                            break;
                        default:
                            SEnvir.Log("道具删除出错");
                            return;
                    }
                    kvp.Key.Delete();
                }
                else
                {
                    kvp.Key.Count -= frag2Required;
                }
            }

            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in frag3List)
            {
                if (frag3Required == kvp.Key.Count)
                {
                    RemoveItem(kvp.Key);
                    switch (kvp.Value.GridType)
                    {
                        case GridType.Inventory:
                            Inventory[kvp.Value.Slot] = null;
                            break;
                        case GridType.Storage:
                            Storage[kvp.Value.Slot] = null;
                            break;
                        case GridType.CompanionInventory:
                            if (Companion == null) return;
                            Companion.Inventory[kvp.Value.Slot] = null;
                            break;
                        default:
                            SEnvir.Log("道具删除出错");
                            return;
                    }
                    kvp.Key.Delete();
                }
                else
                {
                    kvp.Key.Count -= frag3Required;
                }
            }

            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in stoneList)
            {
                if (stoneRequired == kvp.Key.Count)
                {
                    RemoveItem(kvp.Key);
                    switch (kvp.Value.GridType)
                    {
                        case GridType.Inventory:
                            Inventory[kvp.Value.Slot] = null;
                            break;
                        case GridType.Storage:
                            Storage[kvp.Value.Slot] = null;
                            break;
                        case GridType.CompanionInventory:
                            if (Companion == null) return;
                            Companion.Inventory[kvp.Value.Slot] = null;
                            break;
                        default:
                            SEnvir.Log("道具删除出错");
                            return;
                    }
                    kvp.Key.Delete();
                }
                else
                {
                    kvp.Key.Count -= stoneRequired;
                }
            }

            List<CellLinkInfo> allLinks = new List<CellLinkInfo>(1 + p.iron.Count + p.frag1.Count + p.frag2.Count + p.frag3.Count + p.stone.Count);
            allLinks.Add(p.weapon);
            allLinks.AddRange(p.iron);
            allLinks.AddRange(p.frag1);
            allLinks.AddRange(p.frag2);
            allLinks.AddRange(p.frag3);
            allLinks.AddRange(p.stone);

            S.NPCWeaponUpgrade result = new S.NPCWeaponUpgrade
            {
                items = allLinks
            };
            Enqueue(result);

            SendShapeUpdate();
            RefreshStats();
        }

        #endregion

        #region 新版武器升级取回

        public void WeaponUpgradeRetrieve(int index)
        {
            if (Dead || NPC == null || NPCPage == null) return;

            RefineInfo info = Character.Refines.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (SEnvir.Now < info.RetrieveTime && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat("NPC.NPCRefineNotReady".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefineNotReady".Lang(con.Language), MessageType.System);
                return;
            }

            ItemCheck check = new ItemCheck(info.Weapon, info.Weapon.Count, info.Weapon.Flags, info.Weapon.ExpireTime);

            if (!CanGainItems(false, check))
            {
                Connection.ReceiveChat("NPC.NPCRefineNoRoom".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("NPC.NPCRefineNoRoom".Lang(con.Language), MessageType.System);
                return;
            }

            if (info.Chance != 100)
            {
                //没成功
                if (info.MaxChance != 100)
                {
                    //碎了
                    Connection.ReceiveChat($"PlayerObject.WeaponRefineBroken".Lang(Connection.Language, info?.Weapon?.Info?.ItemName), MessageType.System);
                    info.Weapon = null;
                    info.Character = null;
                    info.Delete();
                    return;
                }
                else
                {
                    //没碎
                    Connection.ReceiveChat($"PlayerObject.WeaponRefineFailure".Lang(Connection.Language, info?.Weapon?.Info?.ItemName), MessageType.System);
                }
            }
            //还给玩家武器
            UserItem weapon = info.Weapon;
            Enqueue(new S.NPCRefineRetrieve { Index = info.Index, IsNewWeaponUpgrade = true });
            info.Weapon = null;
            info.Character = null;
            info.Delete();

            GainItem(weapon);
        }

        #endregion

        #region 新版首饰合成

        public void AccessoryCombine(C.AccessoryCombineRequest p)
        {
            // 如果 死亡  NPC为空 NPC页面为空  NPC的类型不是新版首饰合成 跳出
            if (Dead || NPC == null || NPCPage == null || NPCPage.DialogType != NPCDialogType.AccessoryCombine) return;
            // 不是对应的精炼类型 跳出
            if (p.RefineType != JewelryRefineType.DC && p.RefineType != JewelryRefineType.MC && p.RefineType != JewelryRefineType.SC
                && p.RefineType != JewelryRefineType.FireAttack && p.RefineType != JewelryRefineType.IceAttack
                && p.RefineType != JewelryRefineType.LightningAttack && p.RefineType != JewelryRefineType.WindAttack
                && p.RefineType != JewelryRefineType.HolyAttack && p.RefineType != JewelryRefineType.DarkAttack
                && p.RefineType != JewelryRefineType.PhantomAttack) return;

            if (p.MainItem == null || p.ExtraItem1 == null || p.Corundum == null) return;

            if (Gold < Config.ACGoldRate) return;

            List<KeyValuePair<UserItem, CellLinkInfo>> materials = new List<KeyValuePair<UserItem, CellLinkInfo>>();

            UserItem mainItem = GetUserItemFromLink(p.MainItem);
            if (mainItem == null) return;

            UserItem extraItem1 = GetUserItemFromLink(p.ExtraItem1);
            if (extraItem1.Info.Index != mainItem.Info.Index) return;
            if (extraItem1 == null || extraItem1.Level != mainItem.Level) return;

            materials.Add(new KeyValuePair<UserItem, CellLinkInfo>(extraItem1, p.ExtraItem1));

            UserItem extraItem2 = GetUserItemFromLink(p.ExtraItem2);
            if (mainItem.Info.Rarity == Rarity.Common)
            {
                if (extraItem2 == null)
                    return;
                if (extraItem2.Level != mainItem.Level)
                    return;
                materials.Add(new KeyValuePair<UserItem, CellLinkInfo>(extraItem2, p.ExtraItem2));
            }
            if (extraItem2 != null && extraItem2.Info.Index != mainItem.Info.Index) return;

            UserItem corundum = GetUserItemFromLink(p.Corundum);
            if (corundum == null) return;
            materials.Add(new KeyValuePair<UserItem, CellLinkInfo>(corundum, p.Corundum));

            UserItem crystal = GetUserItemFromLink(p.Crystal);
            if (crystal != null)
            {
                if (crystal.Count < p.Crystal.Count) return;
                materials.Add(new KeyValuePair<UserItem, CellLinkInfo>(crystal, p.Crystal));
            }

            // 判断首饰等级
            int nextLevel = mainItem.Level + 1;
            int baseSuccessRate = 0;
            int addedSuccessRate = (int)(p.Crystal?.Count ?? 0);
            int statAmount = 0;

            switch (mainItem.Info.Rarity)
            {
                case Rarity.Common:
                    baseSuccessRate = Config.CommonItemSuccessRate - (mainItem.Level - 1) * Config.CommonItemReduceRate;
                    if (nextLevel == Config.CommonItemLadder1)
                    {
                        statAmount = Config.CommonItemAdditionalValue1;
                    }
                    else if (nextLevel == Config.CommonItemLadder2)
                    {
                        statAmount = Config.CommonItemAdditionalValue2;
                    }
                    else if (nextLevel == Config.CommonItemLadder3)
                    {
                        statAmount = Config.CommonItemAdditionalValue3;
                    }
                    else
                    {
                        statAmount = Config.CommonItemLevelValue;
                    }
                    break;
                case Rarity.Superior:
                    baseSuccessRate = Config.SuperiorItemSuccessRate - (mainItem.Level - 1) * Config.SuperiorItemReduceRate;
                    if (nextLevel == Config.SuperiorItemLadder1)
                    {
                        statAmount = Config.SuperiorItemAdditionalValue1;
                    }
                    else if (nextLevel == Config.SuperiorItemLadder2)
                    {
                        statAmount = Config.SuperiorItemAdditionalValue2;
                    }
                    else if (nextLevel == Config.SuperiorItemLadder3)
                    {
                        statAmount = Config.SuperiorItemAdditionalValue3;
                    }
                    else
                    {
                        statAmount = Config.SuperiorItemLevelValue;
                    }
                    break;
                case Rarity.Elite:
                    baseSuccessRate = Config.EliteItemSuccessRate - (mainItem.Level - 1) * Config.EliteItemReduceRate;
                    if (nextLevel == Config.EliteItemLadder1)
                    {
                        statAmount = Config.EliteItemAdditionalValue1;
                    }
                    else if (nextLevel == Config.EliteItemLadder2)
                    {
                        statAmount = Config.EliteItemAdditionalValue2;
                    }
                    else if (nextLevel == Config.EliteItemLadder3)
                    {
                        statAmount = Config.EliteItemAdditionalValue3;
                    }
                    else
                    {
                        statAmount = Config.EliteItemLevelValue;
                    }
                    break;
            }

            //扣除材料
            ChangeGold(-Config.ACGoldRate);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "新版首饰合成系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = Config.ACGoldRate,
                ExtraInfo = $"新版首饰合成扣除对应的合成费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            S.ItemsChanged result = new S.ItemsChanged { Links = new List<CellLinkInfo>(), Success = true };
            Enqueue(result);

            foreach (KeyValuePair<UserItem, CellLinkInfo> kvp in materials)
            {
                if (kvp.Key.Count == kvp.Value.Count)
                {
                    RemoveItem(kvp.Key);
                    switch (kvp.Value.GridType)
                    {
                        case GridType.Inventory:
                            Inventory[kvp.Value.Slot] = null;
                            break;
                        case GridType.Storage:
                            Storage[kvp.Value.Slot] = null;
                            break;
                        case GridType.CompanionInventory:
                            if (Companion == null) return;
                            Companion.Inventory[kvp.Value.Slot] = null;
                            break;
                        default:
                            SEnvir.Log("道具删除出错");
                            return;
                    }
                    kvp.Key.Delete();
                }
                else
                {
                    kvp.Key.Count -= kvp.Value.Count;
                }

            }

            result.Links.Add(p.ExtraItem1);
            result.Links.Add(p.Corundum);

            if (extraItem2 != null)
                result.Links.Add(p.ExtraItem2);
            if (crystal != null)
                result.Links.Add(p.Crystal);

            //加属性
            if (SEnvir.Random.Next(100) < baseSuccessRate + addedSuccessRate)
            {
                //成功
                S.ItemStatsChanged mainResult = new S.ItemStatsChanged { GridType = p.MainItem.GridType, Slot = p.MainItem.Slot, NewStats = new Stats() };
                switch (p.RefineType)
                {
                    case JewelryRefineType.DC:
                        mainItem.AddStat(Stat.MaxDC, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.MaxDC] = statAmount;
                        break;
                    case JewelryRefineType.MC:
                        mainItem.AddStat(Stat.MaxMC, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.MaxMC] = statAmount;
                        break;
                    case JewelryRefineType.SC:
                        mainItem.AddStat(Stat.MaxSC, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.MaxSC] = statAmount;
                        break;
                    case JewelryRefineType.FireAttack:
                        mainItem.AddStat(Stat.FireAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.FireAttack] = statAmount;
                        break;
                    case JewelryRefineType.IceAttack:
                        mainItem.AddStat(Stat.IceAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.IceAttack] = statAmount;
                        break;
                    case JewelryRefineType.LightningAttack:
                        mainItem.AddStat(Stat.LightningAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.LightningAttack] = statAmount;
                        break;
                    case JewelryRefineType.WindAttack:
                        mainItem.AddStat(Stat.WindAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.WindAttack] = statAmount;
                        break;
                    case JewelryRefineType.HolyAttack:
                        mainItem.AddStat(Stat.HolyAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.HolyAttack] = statAmount;
                        break;
                    case JewelryRefineType.DarkAttack:
                        mainItem.AddStat(Stat.DarkAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.DarkAttack] = statAmount;
                        break;
                    case JewelryRefineType.PhantomAttack:
                        mainItem.AddStat(Stat.PhantomAttack, statAmount, StatSource.Combine, "新版首饰合成");
                        mainResult.NewStats[Stat.PhantomAttack] = statAmount;
                        break;
                }

                mainItem.Level++;
                mainItem.StatsChanged();

                mainResult.FullItemStats = mainItem.ToClientInfo().FullItemStats;
                Enqueue(mainResult);

                Enqueue(new S.ItemExperience { Target = p.MainItem, Experience = mainItem.Experience, Level = mainItem.Level, Flags = mainItem.Flags });


                // Combine success!
                Connection.ReceiveChat("首饰升级成功".Lang(Connection.Language), MessageType.System);
            }
            else
            {
                //失败

                // Combine failed
                Connection.ReceiveChat("首饰升级失败".Lang(Connection.Language), MessageType.System);
            }

            Enqueue(new S.AccessoryCombineResult());

            SendShapeUpdate();
            RefreshStats();

        }

        #endregion

        public void PyInputBox(string message, string OKScriptName, string cancelScriptName = "", object overriddenParams = null)
        {
            if (!string.IsNullOrEmpty(OKScriptName))
            {
                string id = System.Guid.NewGuid().ToString();
                Enqueue(new S.PyTextBox
                {
                    Message = message,
                    ID = id,
                    ObserverPacket = false,
                });

                AsyncPyCallList.Add(new AsyncPyCall
                {
                    ID = id,
                    OKScriptName = OKScriptName,
                    cancelScriptName = cancelScriptName,
                    OverriddenParams = overriddenParams,
                });
            }
        }
    }

    public class AsyncPyCall
    {
        public string ID { get; set; }
        public string OKScriptName { get; set; }
        public string cancelScriptName { get; set; }
        public object OverriddenParams { get; set; } = null;
    }
}
