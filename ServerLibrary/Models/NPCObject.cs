using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 自定义NPC
    /// </summary>
    public class CustomNPC : NPCObject
    {
        /// <summary>
        /// 动作 旗帜点击
        /// </summary>
        public event Action<PlayerObject> OnCall;
        /// <summary>
        /// NPC名字
        /// </summary>
        public string NPCName;
        /// <summary>
        /// NPC当前的玩家
        /// </summary>
        public PlayerObject Current;
        /// <summary>
        /// NPC库
        /// </summary>
        public LibraryFile library;
        /// <summary>
        /// NPC图片索引序号
        /// </summary>
        public int ImageIndex;
        /// <summary>
        /// 覆盖颜色
        /// </summary>
        public Color overlayColor;


        public bool IsNew;

        /// <summary>
        /// 自定义NPC
        /// </summary>
        /// <param name="NPCName"></param>
        /// <param name="library"></param>
        /// <param name="imageIndex"></param>
        public CustomNPC(string NPCName, LibraryFile library, int imageIndex)
        {
            this.NPCName = NPCName;
            this.library = library;
            this.ImageIndex = imageIndex;
            this.NPCInfo = new NPCInfo();
            this.NPCInfo.Index = 999999;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            AddAllObjects();
        }
        public void NPCCall(PlayerObject ob)
        {
            OnCall?.Invoke(ob);
        }
        /// <summary>
        /// 设置图像
        /// </summary>
        /// <param name="library"></param>
        /// <param name="image"></param>
        /// <param name="color"></param>
        public void SetImage(LibraryFile library, int image, Color color)
        {
            this.library = library;
            this.ImageIndex = image;
            this.overlayColor = color;
            if (Visible)
            {
                Visible = false;
                RemoveAllObjects();
                Visible = true;
                AddAllObjects();
            }
        }
        /// <summary>
        /// 获取信息包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.CustomNpc
            {
                ObjectID = ObjectID,
                Name = NPCName,
                library = library,
                ImagIndex = ImageIndex,
                CurrentLocation = CurrentLocation,
                Direction = Direction,
                OverlayColor = overlayColor,
            };
        }
    }

    /// <summary>
    /// NPC对象
    /// </summary>
    public class NPCObject : MapObject
    {
        /// <summary>
        /// 对象类型 种类NPC
        /// </summary>
        public override ObjectType Race => ObjectType.NPC;

        /// <summary>
        /// 脚本ID
        /// </summary>
        public int ScriptID { get; set; }
        /// <summary>
        /// 当前脚本ID
        /// </summary>
        public int CurrentScriptID { get; set; }

        /// <summary>
        /// NPC信息
        /// </summary>
        public NPCInfo NPCInfo;
        //public ScriptSource NpcSource;
        // public ScriptScope Npcscope;

        /// <summary>
        /// NPC名字
        /// </summary>
        public override string Name => NPCInfo.NPCName;
        //public dynamic OnClick;

        /// <summary>
        /// 是否显示
        /// </summary>
        public override bool Blocking => Visible;

        /// <summary>
        /// NPC打开
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="control"></param>
        /// <param name="links"></param>
        /// <param name="userInput"></param>
        public void NPCCall(PlayerObject ob, int control = 0, List<CellLinkInfo> links = null, string userInput = "")
        {
            if (this.NPCInfo.Index == 999999)
            {
                //可能是自定义NPC
                CustomNPC customNPC = (CustomNPC)this;
                customNPC.NPCCall(ob);

                //return;
            }
            ob.NPC = this;   //NPC玩家点击
            ob.NPCPage = null;   //NPC页面为空
            S.NPCResponse packet;
            packet = new S.NPCResponse { ObjectID = ObjectID, Index = 0 };
            // 加载外部 python 脚本文件.
            ClientNPCPage page = new ClientNPCPage();
            try
            {
                //脚本返回结果处理
                dynamic trig_npc;
                trig_npc = SEnvir.PythonEvent["NPCEvent_trig_npc"];
                PythonTuple args;
                if (null != links)
                {
                    if (userInput != string.Empty)
                    {
                        args = PythonOps.MakeTuple(new object[] { this, ob, control, links, userInput });
                    }
                    else
                    {
                        args = PythonOps.MakeTuple(new object[] { this, ob, control, links });
                    }
                }
                else
                {
                    if (userInput != string.Empty)
                    {
                        args = PythonOps.MakeTuple(new object[] { this, ob, control, userInput });
                    }
                    else
                    {
                        args = PythonOps.MakeTuple(new object[] { this, ob, control });
                    }
                }

                IronPython.Runtime.PythonDictionary Dic = SEnvir.ExecutePyWithTimer(trig_npc, this, "OnClick", args) as PythonDictionary;
                //IronPython.Runtime.PythonDictionary Dic = trig_npc(this, "OnClick", args) as IronPython.Runtime.PythonDictionary;

                if (Dic != null)
                {
                    object temp;
                    page.DialogType = Dic.TryGetValue("DialogType", out temp) ? (NPCDialogType)temp : NPCDialogType.None;
                    // 以物换物标记
                    page.BarterFlag = (int)(Dic.TryGetValue("Barter", out temp) ? temp : 0);
                    page.Say = Dic.TryGetValue("Say", out temp) ? temp.ToString() : "";

                    // 隶属城堡
                    string castleName = (string)(Dic.TryGetValue("CastleName", out temp) ? temp : string.Empty);
                    // 获取城堡折扣 税率
                    var castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Name == castleName);
                    decimal discountRate = castle?.Discount ?? 1;
                    decimal taxRate = castle?.TaxRate ?? 0;

                    bool canApplyDiscount = !string.IsNullOrEmpty(castleName) && ob.Character.Account.GuildMember?.Guild?.Castle?.Name == castleName;
                    ;
                    // 记录城堡 折扣 税率
                    page.CastleName = castleName;
                    page.DiscountRate = discountRate;
                    page.TaxRate = taxRate;

                    if (string.IsNullOrEmpty(page.Say))
                    {
                        ob.NPC = null;
                        ob.NPCPage = null;
                        ob.Enqueue(new S.NPCClose());
                        return;
                    }

                    List<ItemType> typelist = new List<ItemType>();
                    if (Dic.TryGetValue("Types", out temp))
                    {
                        IronPython.Runtime.List types = temp as IronPython.Runtime.List;
                        if (types != null)
                        {

                            for (int i = 0; i < types.Count; i++)
                            {
                                typelist.Add((ItemType)types[i]);
                            }
                        }
                    }
                    page.Types = typelist;


                    List<ClientNPCGood> goodslist = new List<ClientNPCGood>();
                    if (Dic.TryGetValue("Goods", out temp))
                    {
                        IronPython.Runtime.PythonDictionary goods = temp as IronPython.Runtime.PythonDictionary;
                        if (goods != null)
                        {
                            int i = 0;
                            // 以物换物 不适用税率和折扣
                            if (page.BarterFlag == 1) // 以物换物
                            {
                                foreach (var k in goods.Keys)
                                {
                                    goods.TryGetValue(k, out temp);
                                    IronPython.Runtime.PythonDictionary target = temp as IronPython.Runtime.PythonDictionary;

                                    foreach (var l in target.Keys)
                                    {
                                        // l = '上古钱币'
                                        object amount = 0;
                                        target.TryGetValue(l, out amount);
                                        ClientNPCGood good = new ClientNPCGood();
                                        good.ItemName = k.ToString();
                                        good.Currency = l.ToString();
                                        good.CurrencyCost = (int)(double)amount;
                                        good.Rate = 1;
                                        good.Index = i;
                                        good.Item = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == good.ItemName);
                                        good.Cost = (int)Math.Round(good.Item.Price * good.Rate);
                                        goodslist.Add(good);
                                        i++;
                                    }
                                }
                            }
                            // 金币交易 适用税率和折扣
                            else
                            {
                                foreach (var k in goods.Keys)
                                {
                                    object rate = 0;
                                    goods.TryGetValue(k, out rate);
                                    ClientNPCGood good = new ClientNPCGood();
                                    good.ItemName = k.ToString();
                                    good.Rate = canApplyDiscount ? Convert.ToDecimal(rate) * discountRate : Convert.ToDecimal(rate);
                                    good.RealRate = canApplyDiscount ? Convert.ToDecimal(rate) : Convert.ToDecimal(rate);
                                    good.Index = i;
                                    good.Item = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == good.ItemName);
                                    good.Cost = (int)Math.Round(canApplyDiscount ? good.Item.Price * good.Rate * discountRate : good.Item.Price * good.Rate);
                                    good.RealCost = (int)Math.Round(canApplyDiscount ? good.Item.Price * good.Rate : good.Item.Price * good.Rate);
                                    goodslist.Add(good);
                                    i++;
                                }
                            }
                        }
                    }
                    // 回购
                    else if (Dic.TryGetValue("Buyback", out temp))
                    {
                        //PythonTuple options = temp as PythonTuple;
                        //if (options != null)
                        //{
                        //    decimal rate = Convert.ToDecimal(options[0]);
                        //    int maxItemAmount = (int)options[1];
                        //    int pageIndex = 1;
                        //    // 生成道具列表
                        //    var clientUserItems = SEnvir.AllOwnerlessItemList.Where(x => typelist.Contains(x.Info.ItemType))
                        //        .Select(y => y.ToClientInfo()).Skip((pageIndex - 1) * 10).Take(10).ToList();

                        //    for (int i = 0; i < clientUserItems.Count; i++)
                        //    {
                        //        var item = clientUserItems[i];
                        //        var itemInfo =
                        //            SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Index == item?.InfoIndex);

                        //        if (itemInfo != null)
                        //        {
                        //            ClientNPCGood good = new ClientNPCGood
                        //            {
                        //                ItemName = itemInfo.ItemName,
                        //                Rate = rate,
                        //                Index = i,
                        //                // todo 极品应该更贵一些 这里可以换个方式计算价格
                        //                // 不能出现回购价格比购买来的价格低 否则就可以刷钱
                        //                UserItem = item,
                        //                Cost = (int)(itemInfo.Price * rate)
                        //            };
                        //            goodslist.Add(good);
                        //        }
                        //        else
                        //        {
                        //            SEnvir.Log($"回购物品列表出错：找不到对应的iteminfo");
                        //        }
                        //    }

                        //}
                        var list = ob.GetBackGoods(out int total, typelist);
                        goodslist = list;
                        packet.CurrentPageIndex = 1;
                        packet.TotalPageIndex = total;
                    }
                    page.Goods = goodslist;

                    NPCCustomBG bgtemp = new NPCCustomBG();
                    if (Dic.TryGetValue("bg", out temp))
                    {
                        IronPython.Runtime.PythonDictionary bg = temp as IronPython.Runtime.PythonDictionary;
                        if (null != bg)
                        {
                            foreach (var k in bg.Keys)
                            {
                                bg.TryGetValue(k, out temp);
                                //  bgtemp.GetType().GetProperty(k as string)?.SetValue(bgtemp,temp);
                                switch (k as string)
                                {
                                    case "url":
                                        {
                                            bgtemp.url = (string)temp;
                                        }
                                        break;
                                    case "title":
                                        {
                                            bgtemp.title = (string)temp;
                                        }
                                        break;
                                    case "file":
                                        {
                                            bgtemp.file = (int)temp;
                                        }
                                        break;
                                    case "idx":
                                        {
                                            bgtemp.idx = (int)temp;
                                        }
                                        break;
                                    case "size_w":
                                        {
                                            bgtemp.size_w = (int)temp;
                                        }
                                        break;
                                    case "size_h":
                                        {
                                            bgtemp.size_h = (int)temp;
                                        }
                                        break;
                                    case "drag":
                                        {
                                            bgtemp.drag = (int)temp;
                                        }
                                        break;
                                    case "center":
                                        {
                                            bgtemp.center = (int)temp;
                                        }
                                        break;
                                    case "offset_x":
                                        {
                                            bgtemp.offset_x = (int)temp;
                                        }
                                        break;
                                    case "offset_y":
                                        {
                                            bgtemp.offset_y = (int)temp;
                                        }
                                        break;
                                    case "close":
                                        {
                                            bgtemp.close = (int)temp;
                                        }
                                        break;
                                    case "close_offset_x":
                                        {
                                            bgtemp.close_offset_x = (int)temp;
                                        }
                                        break;
                                    case "close_offset_y":
                                        {
                                            bgtemp.close_offset_y = (int)temp;
                                        }
                                        break;
                                    default: break;
                                }

                            }
                        }
                    }
                    page.bg = bgtemp;

                    NPCCustomFont fonttemp = new NPCCustomFont();
                    if (Dic.TryGetValue("font", out temp))
                    {
                        IronPython.Runtime.PythonDictionary font = temp as IronPython.Runtime.PythonDictionary;
                        if (null != font)
                        {
                            foreach (var k in font.Keys)
                            {
                                temp = font.TryGetValue(k, out temp) ? temp : null;
                                if (null != temp)
                                {
                                    switch (k as string)
                                    {
                                        case "color":
                                            {
                                                fonttemp.color = (string)temp;
                                            }
                                            break;
                                        case "size":
                                            {
                                                fonttemp.size = (int)temp;
                                            }
                                            break;
                                        case "offset_x":
                                            {
                                                fonttemp.offset_x = (int)temp;
                                            }
                                            break;
                                        case "offset_y":
                                            {
                                                fonttemp.offset_y = (int)temp;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    page.font = fonttemp;
                    if (Dic.TryGetValue("needitems", out temp))
                    {
                        IronPython.Runtime.PythonDictionary Needitems = temp as IronPython.Runtime.PythonDictionary;
                        if (null != Needitems)
                        {
                            foreach (var k in Needitems.Keys)
                            {
                                NPCCustomNeedItems NeedItemstemp = new NPCCustomNeedItems();
                                Needitems.TryGetValue(k, out temp);
                                IronPython.Runtime.PythonDictionary target = temp as IronPython.Runtime.PythonDictionary;
                                foreach (var u in target.Keys)
                                {
                                    temp = target.TryGetValue(u, out temp) ? temp : null;
                                    if (null != temp)
                                    {
                                        switch (u as string)
                                        {
                                            case "pos_x":
                                                {
                                                    NeedItemstemp.pos_x = (int)temp;
                                                }
                                                break;
                                            case "pos_y":
                                                {
                                                    NeedItemstemp.pos_y = (int)temp;
                                                }
                                                break;
                                            case "file":
                                                {
                                                    NeedItemstemp.file = (int)temp;
                                                }
                                                break;
                                            case "idx":
                                                {
                                                    NeedItemstemp.idx = (int)temp;
                                                }
                                                break;
                                            case "dragidx":
                                                {
                                                    NeedItemstemp.dragidx = (int)temp;
                                                }
                                                break;
                                        }
                                    }
                                }
                                page.needItems.Add(NeedItemstemp);
                            }
                        }
                    }
                    packet.NpcPage = page;
                    ob.NPCPage = page;
                    ob.Enqueue(packet);
                }
                else
                {
                    ob.NPC = null;
                    ob.NPCPage = null;
                    ob.Enqueue(new S.NPCClose());
                }
            }
            catch (SyntaxErrorException e)
            {
                ob.NPC = null;
                ob.NPCPage = null;
                ob.Enqueue(new S.NPCClose());
                string msg = "NpcEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                ob.NPC = null;
                ob.NPCPage = null;
                ob.Enqueue(new S.NPCClose());
                string msg = "NpcEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                ob.NPC = null;
                ob.NPCPage = null;
                ob.Enqueue(new S.NPCClose());
                string msg = "NpcEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }
        }

        /*/// <summary>
        /// NPC呼出
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="page"></param>
        public virtual void NPCCall(PlayerObject ob, NPCPage page)
        {
            while (true)
            {
                if (page == null) return;

                NPCPage failPage;
                if (!CheckPage(ob, page, out failPage))
                {
                    page = failPage;
                    continue;
                }

                DoActions(ob, page);

                if (page.SuccessPage != null)
                {
                    page = page.SuccessPage;
                    continue;
                }

                if (string.IsNullOrEmpty(page.Say))
                {
                    ob.NPC = null;
                    ob.NPCPage = null;
                    ob.Enqueue(new S.NPCClose());
                    return;
                }

                ob.NPC = this;
                //ob.NPCPage = page;

                S.NPCResponse packet;
                packet = new S.NPCResponse { ObjectID = ObjectID, Index = page.Index };
                ob.Enqueue(packet);

                break;
            }
        }
        /// <summary>
        /// NPC行为过程
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="page"></param>
        private void DoActions(PlayerObject ob, NPCPage page)
        {
            foreach (NPCAction action in page.Actions)
            {
                switch (action.ActionType)
                {
                    case NPCActionType.Teleport:
                        if (action.MapParameter1 == null) return;

                        Map map = SEnvir.GetMap(action.MapParameter1);

                        if (action.IntParameter1 == 0 && action.IntParameter2 == 0)
                            ob.Teleport(map, map.GetRandomLocation());
                        else
                            ob.Teleport(map, new Point(action.IntParameter1, action.IntParameter2));
                        break;
                    case NPCActionType.TakeGold:
                        ob.Gold -= action.IntParameter1;
                        ob.GoldChanged();
                        break;
                    case NPCActionType.ChangeElement:
                        UserItem weapon = ob.Equipment[(int)EquipmentSlot.Weapon];

                        S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                        result.NewStats[Stat.WeaponElement] = action.IntParameter1 - weapon.Stats[Stat.WeaponElement];

                        weapon.AddStat(Stat.WeaponElement, action.IntParameter1 - weapon.Stats[Stat.WeaponElement], StatSource.Refine);
                        weapon.StatsChanged();
                        ob.RefreshStats();
                        //刷新完整属性列表
                        result.FullItemStats = weapon.ToClientInfo().FullItemStats;
                        ob.Enqueue(result);
                        break;
                    case NPCActionType.ChangeHorse:
                        ob.Character.Account.Horse = (HorseType)action.IntParameter1;

                        ob.RemoveMount();

                        ob.RefreshStats();

                        if (ob.Character.Account.Horse != HorseType.None) ob.Mount();
                        break;
                    case NPCActionType.GiveGold:

                        long gold = ob.Gold + action.IntParameter1;

                        ob.Gold = (long)gold;
                        ob.GoldChanged();

                        break;
                    case NPCActionType.Marriage:
                        ob.MarriageRequest();
                        break;
                    case NPCActionType.Divorce:
                        ob.MarriageLeave();
                        break;
                    case NPCActionType.RemoveWeddingRing:
                        ob.MarriageRemoveRing();
                        break;
                    case NPCActionType.GiveItem:
                        if (action.ItemParameter1 == null) continue;

                        ItemCheck check = new ItemCheck(action.ItemParameter1, action.IntParameter1, UserItemFlags.None, TimeSpan.Zero);

                        if (!ob.CanGainItems(false, check)) continue;

                        while (check.Count > 0)
                        {
                            UserItem newItem = SEnvir.CreateFreshItem(check);
                            //记录物品来源
                            SEnvir.RecordTrackingInfo(newItem, NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPCInfo?.NPCName, ob?.Name);
                            ob.GainItem(newItem);
                        }

                        break;
                    case NPCActionType.TakeItem:
                        if (action.ItemParameter1 == null) continue;

                        ob.TakeItem(action.ItemParameter1, action.IntParameter1);
                        break;
                    case NPCActionType.ResetWeapon:
                        ob.NPCResetWeapon();
                        break;
                    case NPCActionType.GiveItemExperience:
                        if (action.ItemParameter1 == null) continue;

                        check = new ItemCheck(action.ItemParameter1, action.IntParameter1, UserItemFlags.None, TimeSpan.Zero);

                        if (!ob.CanGainItems(false, check)) continue;

                        while (check.Count > 0)
                        {
                            UserItem item = SEnvir.CreateFreshItem(check);
                            //记录物品来源
                            SEnvir.RecordTrackingInfo(item, NPCInfo?.Region?.Map?.Description, ObjectType.NPC, NPCInfo?.NPCName, ob?.Name);

                            item.Experience = action.IntParameter2;

                            if (item.Experience >= Globals.GameAccessoryEXPInfoList[item.Level].Exp)
                            {
                                item.Experience -= Globals.GameAccessoryEXPInfoList[item.Level].Exp;
                                item.Level++;

                                item.Flags |= UserItemFlags.Refinable;
                            }

                            ob.GainItem(item);
                        }

                        break;
                    case NPCActionType.SpecialRefine:
                        ob.NPCSpecialRefine(action.StatParameter1, action.IntParameter1);
                        break;
                    case NPCActionType.Rebirth:    //转生
                        if (ob.Level >= 86 + ob.Character.Rebirth)
                            ob.NPCRebirth();
                        break;
                }
            }
        }
        /// <summary>
        /// 检查NPC页面
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="page"></param>
        /// <param name="failPage"></param>
        /// <returns></returns>
        private bool CheckPage(PlayerObject ob, NPCPage page, out NPCPage failPage)
        {
            failPage = null;
            foreach (NPCCheck check in page.Checks)
            {
                failPage = check.FailPage;
                UserItem weap;
                switch (check.CheckType)
                {
                    case NPCCheckType.Level:
                        if (!Compare(check.Operator, ob.Level, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Class:
                        if (!Compare(check.Operator, (int)ob.Class, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Gold:
                        if (!Compare(check.Operator, ob.Gold, check.IntParameter1)) return false;
                        break;

                    case NPCCheckType.HasWeapon:
                        if (ob.Equipment[(int)EquipmentSlot.Weapon] != null != (check.Operator == Operator.Equal)) return false;
                        break;

                    case NPCCheckType.WeaponLevel:
                        if (!Compare(check.Operator, ob.Equipment[(int)EquipmentSlot.Weapon].Level, check.IntParameter1)) return false;
                        break;

                    case NPCCheckType.WeaponCanRefine:
                        if ((ob.Equipment[(int)EquipmentSlot.Weapon].Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable != (check.Operator == Operator.Equal)) return false;
                        break;
                    case NPCCheckType.WeaponAddedStats:
                        if (!Compare(check.Operator, ob.Equipment[(int)EquipmentSlot.Weapon].Stats[check.StatParameter1], check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.WeaponElement:
                        weap = ob.Equipment[(int)EquipmentSlot.Weapon];

                        Stat element;
                        int value = 0;

                        switch ((Element)check.IntParameter1)
                        {
                            case Element.None:
                                value += weap.Stats.GetWeaponElementValue();
                                value += weap.Info.Stats.GetWeaponElementValue();
                                break;
                            case Element.Fire:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.FireAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Ice:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.IceAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Lightning:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.LightningAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Wind:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.WindAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Holy:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.HolyAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Dark:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.DarkAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }

                                break;
                            case Element.Phantom:
                                element = weap.Stats.GetWeaponElement();

                                if (element == Stat.None)
                                    element = weap.Info.Stats.GetWeaponElement();

                                if (element == Stat.PhantomAttack)
                                {
                                    value += weap.Stats.GetWeaponElementValue();
                                    value += weap.Info.Stats.GetWeaponElementValue();
                                }
                                break;
                        }


                        if (!Compare(check.Operator, value, check.IntParameter2)) return false;
                        break;
                    case NPCCheckType.PKPoints:
                        if (!Compare(check.Operator, ob.Stats[Stat.PKPoint], check.IntParameter1 == 0 ? Config.RedPoint : check.IntParameter1) && ob.Stats[Stat.Redemption] == 0)
                            return false;
                        break;
                    case NPCCheckType.Horse:
                        if (!Compare(check.Operator, (int)ob.Character.Account.Horse, check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.Marriage:
                        if (check.Operator == Operator.Equal)
                        {
                            if (ob.Character.Partner == null) return false;
                        }
                        else
                        {
                            if (ob.Character.Partner != null) return false;
                        }
                        break;
                    case NPCCheckType.WeddingRing:
                        if (check.Operator == Operator.Equal)
                        {
                            if (ob.Equipment[(int)EquipmentSlot.RingL] == null || (ob.Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage) return false;
                        }
                        else
                        {
                            if (ob.Equipment[(int)EquipmentSlot.RingL] != null && (ob.Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return false;
                        }
                        break;
                    case NPCCheckType.HasItem:
                        if (check.ItemParameter1 == null) continue;
                        if (!Compare(check.Operator, ob.GetItemCount(check.ItemParameter1), check.IntParameter1)) return false;
                        break;
                    case NPCCheckType.CanGainItem:
                        if (check.ItemParameter1 == null) continue;

                        ItemCheck itemCheck = new ItemCheck(check.ItemParameter1, check.IntParameter1, UserItemFlags.None, TimeSpan.Zero);

                        if (!ob.CanGainItems(false, itemCheck)) return false;
                        break;
                    case NPCCheckType.CanResetWeapon:
                        if (check.Operator == Operator.Equal)
                        {
                            if (SEnvir.Now < ob.Equipment[(int)EquipmentSlot.Weapon].ResetCoolDown) return false;
                        }
                        else
                        {
                            if (SEnvir.Now >= ob.Equipment[(int)EquipmentSlot.Weapon].ResetCoolDown) return false;
                        }
                        break;
                    case NPCCheckType.Random:
                        if (!Compare(check.Operator, SEnvir.Random.Next(check.IntParameter1), check.IntParameter2)) return false;
                        break;
                }
            }
            return true;
        }
        */

        /// <summary>
        /// 对比比较
        /// </summary>
        /// <param name="op"></param>
        /// <param name="pValue"></param>
        /// <param name="cValue"></param>
        /// <returns></returns>
        private bool Compare(Operator op, long pValue, long cValue)
        {
            switch (op)
            {
                case Operator.Equal:                //等于
                    return pValue == cValue;
                case Operator.NotEqual:             //不等于
                    return pValue != cValue;
                case Operator.LessThan:             //小于
                    return pValue < cValue;
                case Operator.LessThanOrEqual:      //小于或等于
                    return pValue <= cValue;
                case Operator.GreaterThan:          //大于
                    return pValue > cValue;
                case Operator.GreaterThanOrEqual:   //大于或等于
                    return pValue >= cValue;
                default: return false;
            }
        }
        /// <summary>
        /// 获取信息数据包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectNPC
            {
                ObjectID = ObjectID,

                NPCIndex = NPCInfo.Index,

                CurrentLocation = CurrentLocation,

                Direction = Direction,
            };
        }
        /// <summary>
        /// 可见
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanBeSeenBy(PlayerObject ob)
        {
            return Visible && base.CanBeSeenBy(ob);
        }
        /// <summary>
        /// 数据可见
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanDataBeSeenBy(PlayerObject ob)
        {
            return false;
        }
        /// <summary>
        /// 获取数据包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override Packet GetDataPacket(PlayerObject ob)   //获取数据包
        {
            return null;
        }
    }
}
