using CSScriptLib;
using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using S = Library.Network.ServerPackets;

namespace Server.Scripts.Npc
{
    public class NPCScript
    {
        public static string DefalutScript { get; } = @"Npcs/welcome.txt";
        public const string
           MainKey = "[@MAIN]",
           BuyKey = "[@BUY]",
           SellKey = "[@SELL]",
           BuySellKey = "[@BUYSELL]",
           RepairKey = "[@REPAIR]",
           SRepairKey = "[@SREPAIR]",
           RefineKey = "[@REFINE]",
           RefineCheckKey = "[@REFINECHECK]",
           RefineCollectKey = "[@REFINECOLLECT]",
           ReplaceWedRingKey = "[@REPLACEWEDDINGRING]",
           BuyBackKey = "[@BUYBACK]",
           StorageKey = "[@STORAGE]",
           ConsignKey = "[@CONSIGN]",
           MarketKey = "[@MARKET]",
           CraftKey = "[@CRAFT]",
           CompanionKey = "[@COMPANION]",
           WeddingRingKey = "[@WDDINGRRING]",
           RefinementStoneKey = "[@REFINEMENT]",
           MasterRefineKey = "[@MASTERREFINE]",
           WeaponResetKey = "[@WEAPONREST]",
           ItemFragmentKey = "[@ITEMFRAGMENT]",
           AccessoryRefineUpgradeKey = "[@ARUPGRADE]",
           AccessoryRefineLevelKey = "[@ARLEVEL]",
           AccessoryResetKey = "[@ARREST]",
           WeaponCraftResetKey = "[@WEAPONCRAFTREST]",
           AdditionalKey = "[@ADDITIONAL]",
           TombstoneKey = "[@TOMBSTONEKEY]",
           GuildCreateKey = "[@CREATEGUILD]",
           RequestWarKey = "[@REQUESTWAR]",
           SendParcelKey = "[@SENDPARCEL]",
           CollectParcelKey = "[@COLLECTPARCEL]",
           AwakeningKey = "[@AWAKENING]",
           DisassembleKey = "[@DISASSEMBLE]",
           DowngradeKey = "[@DOWNGRADE]",
           ResetKey = "[@RESET]",
           PearlBuyKey = "[@PEARLBUY]",
           BuyUsedKey = "[@BUYUSED]",

           GoodsKey = "[GOODS]",
           RecipeKey = "[RECIPE]",
           UsedTypeKey = "[USEDTYPES]",
           QuestKey = "[QUESTS]",
           SpeechKey = "[SPEECH]",

           InsertKey = "#INSERT",
           IncludeKey = "#INCLUDE",
           CallKey = "#CALL",

            IfKey = "IF",
            SayKey = "SAY",
            ActKey = "ACT",
            ElseSayKey = "ELSESAY",
            ElseActkey = "ELSEACT",

            GotoKey = "GOTO",
            DelayGotoKey = "DELAYGOTO",
            GroupGotoKey = "GROUPGOTO",
            TimeReCallKey = "TIMERECALL",
            TimeReCallGroupKey = "TIMERECALLGROUP";

        public string FileName { get; private set; }
        /// <summary>
        /// 物品列表
        /// </summary>
        public List<ClientNPCGood> Goods = new List<ClientNPCGood>();

        public List<NPCPage> NPCPages = new List<NPCPage>();
        /// <summary>
        /// 物品价格倍率
        /// </summary>
        public decimal Rate { get; set; } = 1;
        /// <summary>
        /// 物品类型
        /// </summary>
        public List<ItemType> ItemTypes { get; set; } = new List<ItemType>();

        public int ScriptID { get; set; }
        public NPCScriptType ScriptType;
        public NPCScript(string fileName, NPCScriptType type)
        {
            ScriptID = ++SEnvir.ScriptIndex;
            FileName = fileName;
            ScriptType = type;
            Load();
            SEnvir.NPCScripts.Add(ScriptID, this);
        }
        public static NPCScript Get(string fileName)
        {
            return SEnvir.NPCScripts.SingleOrDefault(x => x.Value.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).Value;
        }
        public static NPCScript Get(int index)
        {
            return SEnvir.NPCScripts[index];
        }
        public static NPCScript GetOrAdd(string fileName, NPCScriptType type)
        {

            var script = SEnvir.NPCScripts.SingleOrDefault(x => x.Value.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).Value;

            if (script != null)
            {
                return script;
            }
            return new NPCScript(fileName, type);
        }

        private void Load()
        {
            ClearInfo();

            if (!Directory.Exists(Config.EnvirPath)) return;

            string path = Path.Combine(Config.EnvirPath, FileName);

            if (File.Exists(path))
            {
                List<string> lines = GetPageLines(path);
                ParseScript(lines);
            }
            else
                SEnvir.Log(string.Format("未找到脚本: {0}", path));
        }
        public void ClearInfo()
        {
            Goods = new List<ClientNPCGood>();
            NPCPages = new List<NPCPage>();
        }

        private void ParseScript(IList<string> lines)
        {
            NPCPages.AddRange(ParsePages(lines));
            ParseItemTypes(lines);
            ParseGoods(lines);
            //ParseQuests(lines);
        }

        private List<string> ParseInsert(List<string> lines, string basePath)
        {
            List<string> newLines = new List<string>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].ToUpper().StartsWith(InsertKey)) continue;

                string[] split = lines[i].Split(' ');

                if (split.Length < 2) continue;

                string path = Path.Combine(basePath, split[1].Substring(1, split[1].Length - 2));

                if (!File.Exists(path))
                    SEnvir.Log(string.Format("未找到插入的脚本: {0}", path));
                else
                    newLines = CryptionHelper.DecryptFilelines(path).ToList();

                lines.AddRange(newLines);
            }

            lines.RemoveAll(str => str.ToUpper().StartsWith(InsertKey));

            return lines;
        }

        private List<string> ParseInclude(List<string> lines, string basePath)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].ToUpper().StartsWith(IncludeKey)) continue;

                string[] split = lines[i].Split(' ');

                string path = Path.Combine(basePath, split[1].Substring(1, split[1].Length - 2));
                string page = ("[" + split[2] + "]").ToUpper();

                bool start = false, finish = false;

                var parsedLines = new List<string>();

                if (!File.Exists(path))
                {
                    SEnvir.Log(string.Format("未找到包含的脚本: {0}", path));
                    return parsedLines;
                }

                IList<string> extLines = GetPageLines(path);

                for (int j = 0; j < extLines.Count; j++)
                {
                    if (!extLines[j].ToUpper().StartsWith(page)) continue;

                    for (int x = j + 1; x < extLines.Count; x++)
                    {
                        if (extLines[x].Trim() == ("{"))
                        {
                            start = true;
                            continue;
                        }

                        if (extLines[x].Trim() == ("}"))
                        {
                            finish = true;
                            break;
                        }

                        parsedLines.Add(extLines[x]);
                    }
                }

                if (start && finish)
                {
                    lines.InsertRange(i + 1, parsedLines);
                    parsedLines.Clear();
                }
            }

            lines.RemoveAll(str => str.ToUpper().StartsWith(IncludeKey));

            return lines;
        }

        /// <summary>
        /// CALL语法加工，交给NPCSegment处理
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private List<string> ParseCall(List<string> lines, string basePath)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                if (!lines[i].ToUpper().StartsWith(CallKey)) continue;
                var parsedLines = new List<string>();
                string[] split = lines[i].Split(' ');
                if (split.Length < 3)
                {
                    SEnvir.Log($@"{FileName}: {CallKey} 语法错误: {lines[i]}");
                    return parsedLines;
                }
                string path = Path.Combine(basePath, split[1].Substring(1, split[1].Length - 2));

                if (!File.Exists(path))
                {
                    SEnvir.Log($"{FileName}: {CallKey} 脚本不存在: {path}");
                    return parsedLines;
                }
                string page = ("[" + split[2] + "]").ToUpper();
                IList<string> extLines = GetPageLines(path);
                if (!extLines.Any(p => p.ToUpper().StartsWith(page)))
                {
                    SEnvir.Log($@"{FileName}: {CallKey} 文件{path}脚本不存在 {page}定义");
                    return parsedLines;
                }
                lines[i] = lines[i].Replace(CallKey, CallKey.Substring(1)).Replace("[", "").Replace("]", "");

            }
            return lines;
        }

        private List<string> GetPageLines(string path)
        {
            var basePath = Path.GetDirectoryName(path);
            var result = CryptionHelper.DecryptFilelines(path).Where(p => !p.StartsWith(";") && !string.IsNullOrEmpty(p.Trim())).ToList();
            result = ParseInsert(result, basePath);
            result = ParseInclude(result, basePath);
            result = ParseCall(result, basePath);
            return result;
        }

        private List<NPCPage> ParsePages(IList<string> lines, string key = MainKey)
        {
            List<NPCPage> pages = new List<NPCPage>();
            List<string> buttons = new List<string>();

            NPCPage page = ParsePage(lines, key);
            pages.Add(page);

            buttons.AddRange(page.Buttons);

            for (int i = 0; i < buttons.Count; i++)
            {
                string section = buttons[i];

                bool match = pages.Any(t => t.Key.ToUpper() == section.ToUpper());

                if (match) continue;

                page = ParsePage(lines, section);
                buttons.AddRange(page.Buttons);

                pages.Add(page);
            }

            return pages;
        }

        private NPCPage ParsePage(IList<string> scriptLines, string sectionName)
        {
            bool nextPage = false, nextSection = false;

            List<string> lines = scriptLines.Where(x => !string.IsNullOrEmpty(x)).ToList();

            NPCPage Page = new NPCPage(sectionName);
            //Cleans arguments out of search page name
            string tempSectionName = Page.ArgumentParse(sectionName);

            //parse all individual pages in a script, defined by sectionName
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (line.StartsWith(";")) continue;

                if (!lines[i].ToUpper().StartsWith(tempSectionName.ToUpper())) continue;

                List<string> segmentLines = new List<string>();

                nextPage = false;

                //Found a page, now process that page and split it into segments
                for (int j = i + 1; j < lines.Count; j++)
                {
                    string nextLine = lines[j];

                    if (j < lines.Count - 1)
                        nextLine = lines[j + 1];
                    else
                        nextLine = "";

                    if (nextLine.StartsWith("[") && nextLine.EndsWith("]"))
                    {
                        nextPage = true;
                    }

                    else if (nextLine.StartsWith($"#{IfKey}"))
                    {
                        nextSection = true;
                    }

                    if (nextSection || nextPage)
                    {
                        segmentLines.Add(lines[j]);

                        //end of segment, so need to parse it and put into the segment list within the page
                        if (segmentLines.Count > 0)
                        {
                            NPCSegment segment = ParseSegment(Page, segmentLines);

                            List<string> currentButtons = new List<string>();
                            currentButtons.AddRange(segment.Buttons);
                            currentButtons.AddRange(segment.ElseButtons);
                            currentButtons.AddRange(segment.GotoButtons);

                            Page.Buttons.AddRange(currentButtons);
                            Page.SegmentList.Add(segment);
                            segmentLines.Clear();

                            nextSection = false;
                        }

                        if (nextPage) break;

                        continue;
                    }

                    segmentLines.Add(lines[j]);
                }

                //bottom of script reached, add all lines found to new segment
                if (segmentLines.Count > 0)
                {
                    NPCSegment segment = ParseSegment(Page, segmentLines);

                    List<string> currentButtons = new List<string>();
                    currentButtons.AddRange(segment.Buttons);
                    currentButtons.AddRange(segment.ElseButtons);
                    currentButtons.AddRange(segment.GotoButtons);

                    Page.Buttons.AddRange(currentButtons);
                    Page.SegmentList.Add(segment);
                    segmentLines.Clear();
                }

                return Page;
            }

            return Page;
        }

        private NPCSegment ParseSegment(NPCPage page, IEnumerable<string> scriptLines)
        {
            List<string>
                checks = new List<string>(),
                acts = new List<string>(),
                say = new List<string>(),
                buttons = new List<string>(),
                elseSay = new List<string>(),
                elseActs = new List<string>(),
                elseButtons = new List<string>(),
                gotoButtons = new List<string>();

            List<string> lines = scriptLines.ToList();
            List<string> currentSay = say, currentButtons = buttons;

            Regex regex = new Regex(@"<.*?/(\@.*?)>");

            for (int i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;

                if (lines[i].StartsWith(";")) continue;

                if (lines[i].StartsWith("#"))
                {
                    string[] action = lines[i].Remove(0, 1).ToUpper().Trim().Split(' ');
                    switch (action[0])
                    {
                        case IfKey:
                            currentSay = checks;
                            currentButtons = null;
                            continue;
                        case SayKey:
                            currentSay = say;
                            currentButtons = buttons;
                            continue;
                        case ActKey:
                            currentSay = acts;
                            currentButtons = gotoButtons;
                            continue;
                        case ElseSayKey:
                            currentSay = elseSay;
                            currentButtons = elseButtons;
                            continue;
                        case ElseActkey:
                            currentSay = elseActs;
                            currentButtons = gotoButtons;
                            continue;
                        default:
                            throw new NotImplementedException();
                    }
                }

                if (lines[i].StartsWith("[") && lines[i].EndsWith("]")) break;

                if (currentButtons != null)
                {
                    Match match = regex.Match(lines[i]);

                    while (match.Success)
                    {
                        string argu = match.Groups[1].Captures[0].Value;

                        currentButtons.Add(string.Format("[{0}]", argu));
                        match = match.NextMatch();
                    }

                    //Check if line has a goto command
                    var parts = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Count() > 1)
                        switch (parts[0].ToUpper())
                        {
                            case GotoKey:
                            case GroupGotoKey:
                                gotoButtons.Add(string.Format("[{0}]", parts[1].ToUpper()));
                                break;
                            case TimeReCallKey:
                            case DelayGotoKey:
                            case TimeReCallGroupKey:
                                if (parts.Length > 2)
                                    gotoButtons.Add(string.Format("[{0}]", parts[2].ToUpper()));
                                break;
                        }
                }

                currentSay.Add(lines[i].TrimEnd());
            }

            NPCSegment segment = new NPCSegment(page, say, buttons, elseSay, elseButtons, gotoButtons);

            for (int i = 0; i < checks.Count; i++)
                segment.ParseCheck(checks[i]);

            for (int i = 0; i < acts.Count; i++)
                segment.ParseAct(segment.ActList, acts[i]);

            for (int i = 0; i < elseActs.Count; i++)
                segment.ParseAct(segment.ElseActList, elseActs[i]);

            currentButtons = new List<string>();
            currentButtons.AddRange(buttons);
            currentButtons.AddRange(elseButtons);
            currentButtons.AddRange(gotoButtons);

            return segment;
        }
        private void ParseItemTypes(IList<string> lines)
        {
            if (!lines.Any(p => p.StartsWith("%"))) return;
            for (int i = 0; i < lines.Count; i++)
            {
                while (++i < lines.Count)
                {
                    if (lines[i].StartsWith("[")) return;
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    if (lines[i].ToUpper().StartsWith("%"))
                    {
                        if (decimal.TryParse(lines[i].Substring(1), out decimal rate))
                        {
                            Rate = rate / 100;
                            continue;
                        }
                    }

                    if (!lines[i].StartsWith("+")) continue;
                    if (int.TryParse(lines[i].Substring(1), out int value))
                    {
                        switch (value)
                        {
                            case 0://药品
                            case 2:
                                ItemTypes.Add(ItemType.Consumable);
                                break;
                            case 1://肉
                            case 40:
                                ItemTypes.Add(ItemType.Meat);
                                break;
                            case 3://卷轴和药水
                                ItemTypes.Add(ItemType.Scroll);
                                ItemTypes.Add(ItemType.Consumable);
                                break;
                            case 4://书
                            case 51:
                            case 61:
                                ItemTypes.Add(ItemType.Book);
                                break;
                            case 5://武器，刀剑
                            case 6://武器，法杖
                                ItemTypes.Add(ItemType.Weapon);
                                break;
                            case 10://男衣
                            case 11://女衣
                                ItemTypes.Add(ItemType.Armour);
                                break;
                            case 12://时装
                                ItemTypes.Add(ItemType.Fashion);
                                break;
                            case 15://头盔
                                ItemTypes.Add(ItemType.Helmet);
                                break;
                            case 19://项链
                            case 20:
                            case 21:
                                ItemTypes.Add(ItemType.Necklace);
                                break;
                            case 22://戒指
                            case 23://戒指 （Ac=速度+）
                                ItemTypes.Add(ItemType.Ring);
                                break;
                            case 24://手镯
                            case 26://手镯祈祷、神秘、魔血、虹魔
                                ItemTypes.Add(ItemType.Bracelet);
                                break;
                            case 25://毒药、符、符石、鲜花
                                ItemTypes.Add(ItemType.Amulet);
                                ItemTypes.Add(ItemType.Poison);
                                ItemTypes.Add(ItemType.DarkStone);
                                ItemTypes.Add(ItemType.Flower);
                                break;
                            case 30://蜡烛
                                ItemTypes.Add(ItemType.Torch);
                                break;
                            case 36://卷轴
                            case 50://卷轴
                                ItemTypes.Add(ItemType.Scroll);
                                break;
                            case 41://任务物品
                                ItemTypes.Add(ItemType.Nothing);
                                ItemTypes.Add(ItemType.Book);
                                break;
                            case 42://材料
                                ItemTypes.Add(ItemType.Material);
                                break;
                            case 43://矿
                                ItemTypes.Add(ItemType.Ore);
                                break;
                            case 44://特殊物品
                            case 46://特殊物品
                                ItemTypes.Add(ItemType.RefineSpecial);
                                break;
                            case 45://骰子类//TODO
                                ItemTypes.Add(ItemType.Nothing);
                                break;
                            case 47://金条类 //TODO?
                                ItemTypes.Add(ItemType.Consumable);
                                break;
                            case 53://鞋子
                                ItemTypes.Add(ItemType.Shoes);
                                break;
                            case 54://投掷类武器
                                ItemTypes.Add(ItemType.DartWeapon);
                                break;
                            case 58://马匹
                                ItemTypes.Add(ItemType.HorseArmour);
                                break;
                            case 60://徽章 声望称号
                                ItemTypes.Add(ItemType.Emblem);
                                ItemTypes.Add(ItemType.FameTitle);
                                break;
                            case 65://镶嵌宝石类
                                ItemTypes.Add(ItemType.Gem);
                                ItemTypes.Add(ItemType.Orb);
                                ItemTypes.Add(ItemType.Rune);
                                ItemTypes.Add(ItemType.Drill);
                                break;
                            case 70://宠物
                                ItemTypes.Add(ItemType.CompanionFood);
                                ItemTypes.Add(ItemType.CompanionBag);
                                ItemTypes.Add(ItemType.CompanionHead);
                                ItemTypes.Add(ItemType.CompanionBack);
                                break;
                            case 80://盾牌
                                ItemTypes.Add(ItemType.Shield);
                                break;
                            case 90://渔具
                                ItemTypes.Add(ItemType.Hook);
                                ItemTypes.Add(ItemType.Float);
                                ItemTypes.Add(ItemType.Bait);
                                ItemTypes.Add(ItemType.Finder);
                                ItemTypes.Add(ItemType.Reel);
                                ItemTypes.Add(ItemType.Fish);
                                break;
                            default:
                                ItemTypes.Add(ItemType.Nothing);
                                break;
                        }
                    }
                }
            }
        }
        private void ParseGoods(IList<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].ToUpper().StartsWith(GoodsKey)) continue;

                while (++i < lines.Count)
                {
                    if (lines[i].StartsWith("[")) return;
                    if (string.IsNullOrEmpty(lines[i])) continue;

                    var data = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(p => p.ItemName == data[0]);
                    if (info == null)
                    {
                        SEnvir.Log($"{FileName} 物品 {data[0]} 不存在!");
                        continue;
                    }
                    if (ItemTypes.IndexOf(info.ItemType) == -1)
                    {
                        SEnvir.Log($"{FileName} 物品 {data[0]} 不在售卖类型范围内!");
                        continue;
                    }

                    ClientNPCGood good = new ClientNPCGood
                    {
                        ItemName = info.ItemName,
                        CurrencyCost = info.Price,
                        Rate = Rate,
                        Index = info.Index,
                        Item = info,
                        Cost = (int)Math.Round(info.Price * Rate)
                    };

                    Goods.Add(good);
                }
            }
        }

        public void Call(string key)
        {
            key = key.ToUpper();

            for (int i = 0; i < NPCPages.Count; i++)
            {
                NPCPage page = NPCPages[i];
                if (!String.Equals(page.Key, key, StringComparison.CurrentCultureIgnoreCase)) continue;

                foreach (NPCSegment segment in page.SegmentList)
                {
                    if (page.BreakFromSegments)
                    {
                        page.BreakFromSegments = false;
                        break;
                    }

                    ProcessSegment(page, segment);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="objectID">NPC objectid</param>
        /// <param name="key"></param>
        public void Call(PlayerObject player, uint objectID, string key)
        {
            key = key.ToUpper();

            if (!player.NPC.Dead)
            {
                if (key != MainKey)
                {
                    if (player.NPC.ObjectID != objectID) return;

                    bool found = false;

                    foreach (NPCSegment segment in player.NPCPage.SegmentList)
                    {
                        if (!player.NPCSuccess.TryGetValue(segment, out bool result)) break; //no result for segment ?

                        if ((result ? segment.Buttons : segment.ElseButtons).Any(s => s.ToUpper() == key))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        SEnvir.Log(string.Format("玩家: {0} 被阻止访问NPC密匙: '{1}' ", player.Name, key));
                        return;
                    }
                }
            }
            else
            {
                player.NPC.Dead = false;
            }

            if (key.StartsWith("[@@") && player.NPCInputStr == string.Empty)
            {
                //send off packet to request input
                //player.Enqueue(new S.NPCRequestInput { NPCID = player.NPC.ObjectID, PageName = key });
                return;
            }

            for (int i = 0; i < NPCPages.Count; i++)
            {
                NPCPage page = NPCPages[i];
                if (!String.Equals(page.Key, key, StringComparison.CurrentCultureIgnoreCase)) continue;

                player.NPCSpeech = new List<string>();
                player.NPCSuccess.Clear();

                foreach (NPCSegment segment in page.SegmentList)
                {
                    if (page.BreakFromSegments)
                    {
                        page.BreakFromSegments = false;
                        break;
                    }

                    ProcessSegment(player, page, segment, objectID);
                }

                Response(player, page);
            }
            player.NPCInputStr = string.Empty;
        }
        private void Response(PlayerObject player, NPCPage page)
        {
            ProcessSpecial(player, page);

            if (player.NPC == null)//人物被传送走
            {
                return;
            }
            if (!player.NPCSpeech.Any())
            {
                //player.NPC.Dead = true;
                return;
            }
            if (page.Key.ToUpper() == BuyKey || page.Key.ToUpper() == BuySellKey)
            {
                player.NPCPage.Goods = Goods;
                player.Enqueue(new S.NPCResponse { Page = player.NPCSpeech, ObjectID = player.NPC.ObjectID, NPCDialogType = page.DialogType, Goods = Goods });
            }
            else
            {
                player.Enqueue(new S.NPCResponse { Page = player.NPCSpeech, ObjectID = player.NPC.ObjectID, NPCDialogType = page.DialogType });
            }

        }
        private void ProcessSegment(PlayerObject player, NPCPage page, NPCSegment segment, uint objectID)
        {
            player.NPC = player.CurrentMap.NPCs.FirstOrDefault(p => p.ObjectID == objectID);
            player.NPCScriptID = ScriptID;
            player.NPCSuccess.Add(segment, segment.Check(player));
            player.NPCPage = page;
        }
        private void ProcessSegment(NPCPage page, NPCSegment segment)
        {
            segment.Check();
        }

        private void ProcessSpecial(PlayerObject player, NPCPage page)
        {

            var key = page.Key.ToUpper();

            switch (key)
            {
                case BuyKey:
                case BuySellKey:
                    page.DialogType = NPCDialogType.BuySell;
                    break;
                case RepairKey:
                    page.DialogType = NPCDialogType.Repair;
                    break;
                case RefineKey:
                    page.DialogType = NPCDialogType.Refine;
                    break;
                case CompanionKey:
                    page.DialogType = NPCDialogType.CompanionManage;
                    break;
                case WeddingRingKey:
                    page.DialogType = NPCDialogType.WeddingRing;
                    break;
                case RefinementStoneKey:
                    page.DialogType = NPCDialogType.RefinementStone;
                    break;
                case MasterRefineKey:
                    page.DialogType = NPCDialogType.MasterRefine;
                    break;
                case WeaponResetKey:
                    page.DialogType = NPCDialogType.WeaponReset;
                    break;
                case ItemFragmentKey:
                    page.DialogType = NPCDialogType.ItemFragment;
                    break;
                case AccessoryRefineUpgradeKey:
                    page.DialogType = NPCDialogType.AccessoryRefineUpgrade;
                    break;
                case AccessoryRefineLevelKey:
                    page.DialogType = NPCDialogType.AccessoryRefineLevel;
                    break;
                case AccessoryResetKey:
                    page.DialogType = NPCDialogType.AccessoryReset;
                    break;
                case WeaponCraftResetKey:
                    page.DialogType = NPCDialogType.WeaponCraft;
                    break;
                case AdditionalKey:
                    page.DialogType = NPCDialogType.Additional;
                    break;
                case MarketKey:
                    page.DialogType = NPCDialogType.MarketSearch;
                    break;
                case TombstoneKey:
                    page.DialogType = NPCDialogType.AncientTombstone;
                    break;
            }
        }
        static IEvaluator _evaluator;
        #region 自定义扩展函数
        /// <summary>
        /// 扩展函数
        /// </summary>
        public static void SetExtend()
        {
            SEnvir.CSScripts = new Dictionary<string, ExtendScript>();
            if (_evaluator == null)
            {
                _evaluator = CSScript.Evaluator.ReferenceDomainAssemblies(DomainAssemblies.AllStaticNonGAC);
            }
            GetScripts(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, Config.EnvirPath, "Extends")));
        }
        private static void GetScripts(DirectoryInfo dirInfo)
        {
            foreach (var item in dirInfo.GetFiles("*.cs"))
            {
                var combile = _evaluator.CompileCode(CryptionHelper.DecryptFileStr(item.FullName));

                var key = item.Name.Replace(".cs", "");
                var type = combile.GetType("css_root+" + key);
                var obj = combile.CreateInstance("css_root+" + key);

                SEnvir.CSScripts.Add(key.ToUpper(), new ExtendScript { Obj = obj, ScriptType = type });
            }
            foreach (DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                GetScripts(subdir);
            }
        }
        #endregion

    }
    public enum NPCScriptType
    {
        Normal,
        Called,
        AutoPlayer,
        AutoMonster,
        Robot
    }

}
