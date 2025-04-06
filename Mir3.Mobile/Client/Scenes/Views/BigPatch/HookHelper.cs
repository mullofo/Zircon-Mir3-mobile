using Client.Envir;
using Client.Models;
using Client.Scenes.Configs;
using Library;
using Library.SystemModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using C = Library.Network.ClientPackets;
namespace Client.Scenes.Views
{
    public partial class HookHelper
    {
        public static ClientAutoPotionLink[] Links = new ClientAutoPotionLink[16];

        public static ClientAutoPotionLink[] AutoLinks = new ClientAutoPotionLink[16];

        public static bool UseCompanionInvent { get; private set; }
        public static DateTime UseHpTime = CEnvir.Now;
        public static DateTime UseMpTime = CEnvir.Now;


        public static void LoadAutoPotionConfig()
        {
            Hping = false;
            Mping = false;
            UseCompanionInvent = CEnvir.ClientControl.AutoPotionForCompanion;
            UpdateOrAdd(0, BigPatchConfig.HP1);
            UpdateOrAdd(1, BigPatchConfig.HP2);
            UpdateOrAdd(2, BigPatchConfig.HP3);
            UpdateOrAdd(3, BigPatchConfig.HP4);
            UpdateOrAdd(4, BigPatchConfig.HP5);
            UpdateOrAdd(5, BigPatchConfig.HP6);
            UpdateOrAdd(6, BigPatchConfig.HP7);
            UpdateOrAdd(7, BigPatchConfig.HP8);
            UpdateOrAdd(8, BigPatchConfig.MP1);
            UpdateOrAdd(9, BigPatchConfig.MP2);
            UpdateOrAdd(10, BigPatchConfig.MP3);
            UpdateOrAdd(11, BigPatchConfig.MP4);
            UpdateOrAdd(12, BigPatchConfig.Roll1);
            UpdateOrAdd(13, BigPatchConfig.Roll2);
            UpdateOrAdd(14, BigPatchConfig.OffLine);
            UpdateOrAdd(15, BigPatchConfig.AutoHeal);
        }

        public static void AutoPotionInitialize()
        {
            #region 加载UI
            GameScene.Game.BigPatchBox.AutoPotionPage.LoadConfig();
            #endregion
        }

        public static void UpdateOrAdd(int index, string vStr)
        {
            var arr = vStr.Split('|');
            if (index < 14)
            {
                if (arr.Length < 3) return;
                var itemInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == arr[2]);
                if (itemInfo == null) return;
                if (string.IsNullOrEmpty(arr[1]))
                {
                    arr[1] = "0";
                }
                var enable = Convert.ToBoolean(arr[0]);
                var value = Convert.ToInt32(arr[1]);
                var link = Links[index];
                var hp = index >= 8 && index <= 11 ? 0 : value;
                var mp = index >= 8 && index <= 11 ? value : 0;
                if (link == null)
                {
                    Links[index] = new ClientAutoPotionLink();
                }
                Links[index].Enabled = enable;
                Links[index].Slot = index;
                Links[index].Health = hp;
                Links[index].Mana = mp;
                Links[index].LinkInfoIndex = itemInfo.Index;
            }
            else
            {
                if (arr.Length < 2) return;
                if (string.IsNullOrEmpty(arr[1]))
                {
                    arr[1] = "0";
                }
                var enable = Convert.ToBoolean(arr[0]);
                var value = Convert.ToInt32(arr[1]);
                var link = Links[index];
                if (link == null)
                {
                    Links[index] = new ClientAutoPotionLink();
                }
                Links[index].Enabled = enable;
                Links[index].Slot = index;
                Links[index].Health = value;
                Links[index].Mana = 0;
                Links[index].LinkInfoIndex = -1;
            }
        }

        #region 喝红

        static bool Hping = false;
        static DateTime _warningHpTime = CEnvir.Now;

        public static void AutoHP()
        {
            if (Hping) return;
            if (GameScene.Game.BigPatchBox?.AutoPotionPage == null) return;
            if (GameScene.Game.Observer) return;
            var hp = GameScene.Game.User.CurrentHP;
            var top = GameScene.Game.User.Stats[Stat.Health];
            if (hp < 0 || GameScene.Game.User.Dead || top == 0 || hp == top) return;

            Task.Run(() =>
            {
                try
                {
                    Hping = true;
                    UseRoll(12);
                    UseRoll(13);

                    if (!BigPatchConfig.HPAuto)
                    {
                        HpConfig();
                    }
                    else
                    {
                        HpAuto();
                    }

                    Hping = false;
                }
                catch
                {
                    Hping = false;
                }
            });
        }

        private static void HpConfig()
        {
            if (CEnvir.Now <= UseHpTime) return;

            if (GameScene.Game.User.CurrentHP < GameScene.Game.User.Stats[Stat.Health])
            {
                TryExiteGame();
                if (_isExiting || GameScene.Game?.User == null) return;
                UseHeal();

                if (BigPatchConfig.HPAuto)
                {
                    HpAuto();
                    return;
                }

                var links = Links.Where(p => p != null && p.Slot < 8 && p.Enabled && p.Health > 0).OrderBy(p => p.Health);
                if (links == null || !links.Any())
                {
                    return;
                }

                if (GameScene.Game.User.CurrentHP > links.Last().Health) return;
                //相关包裹是否有相关药品
                var indexs = links.Select(p => p.LinkInfoIndex);
                var items = GameScene.Game.Inventory.Where(p => p != null && indexs.Contains(p.InfoIndex));
                if (UseCompanionInvent)
                {
                    var compItems = GameScene.Game.Companion?.InventoryArray?.Where(p => p != null && indexs.Contains(p.InfoIndex));
                    if (compItems != null)
                    {
                        items = items == null ? compItems : items.Union(compItems);
                    }
                }

                if ((items == null || !items.Any()) && CEnvir.Now > _warningHpTime)
                {
                    //CEnvir.Target.Invoke(new Action(() => GameScene.Game.ReceiveChat($">>  警告没有金创药相关物品".Lang(), MessageType.Combat)));
                    _warningHpTime = CEnvir.Now.AddSeconds(10);
                    return;
                }

                foreach (var item in links)
                {
                    var info = GameScene.Game.Inventory.FirstOrDefault(p => p != null && p.InfoIndex == item.LinkInfoIndex);
                    var isInventory = true;
                    if (info == null && UseCompanionInvent)
                    {
                        info = GameScene.Game.Companion?.InventoryArray?.FirstOrDefault(p => p != null && p.InfoIndex == item.LinkInfoIndex);
                        if (info != null)
                            isInventory = false;
                    }
                    if (info?.Info != null && GameScene.Game.User.CurrentHP <= item.Health && info.Count > 0)
                    {
                        if (GameScene.Game.CanUseItem(info))
                        {
                            var cell = isInventory ?
                                GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index) :
                                GameScene.Game.CompanionBox.InventoryGrid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index);
                            if (cell == null) continue;
                            //if (cell.GridType != GridType.Inventory || cell.GridType != GridType.CompanionInventory) break;
                            cell?.UseItem();
                            UseHpTime = CEnvir.Now.AddMilliseconds(info.Info.Durability + CEnvir.ClientControl.BigPatchAutoPotionDelayEdit);
                            break;
                        }
                    }
                }
            }
        }

        private static void HpAuto()
        {
            if (CEnvir.Now <= UseHpTime) return;

            if (GameScene.Game.User.CurrentHP < BigPatchConfig.HP)
            {
                TryExiteGame();
                if (_isExiting || GameScene.Game?.User == null) return;
                UseHeal();

                if (!BigPatchConfig.HPAuto)
                {
                    HpConfig();
                    return;
                }
                Func<ClientUserItem, bool> where = p => p != null && p.Info.CanAutoPot && p.Info.Stats[Stat.Health] > 0;
                var items = GameScene.Game.Inventory.Where(where);
                if (UseCompanionInvent)
                {
                    var compItems = GameScene.Game.Companion?.InventoryArray?.Where(where);
                    if (compItems != null)
                    {
                        items = items == null ? compItems : items.Union(compItems);
                    }
                }
                var list = items.Select(p => p.Info).OrderByDescending(p => p.Stats[Stat.Health]).OrderByDescending(p => p.Stats[Stat.Mana]);
                if (list == null || !list.Any())
                {
                    if (CEnvir.Now > _warningHpTime)
                    {
                        //CEnvir.Target.Invoke(new Action(() => GameScene.Game.ReceiveChat($">>  警告没有金创药相关物品".Lang(), MessageType.Combat)));
                        _warningHpTime = CEnvir.Now.AddSeconds(10);
                    }
                    return;
                }

                foreach (var item in list)
                {
                    var info = GameScene.Game.Inventory.FirstOrDefault(p => p != null && p.InfoIndex == item.Index);
                    var isInventory = true;
                    if (info == null && UseCompanionInvent)
                    {
                        info = GameScene.Game.Companion?.InventoryArray?.FirstOrDefault(p => p != null && p.InfoIndex == item.Index);
                        if (info != null)
                            isInventory = false;
                    }
                    if (info?.Info != null && GameScene.Game.User.CurrentHP + info.Info.Stats[Stat.Health] <= BigPatchConfig.HP && info?.Count > 0)
                    {
                        if (GameScene.Game.CanUseItem(info))
                        {
                            var cell = isInventory ?
                                GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index) :
                                GameScene.Game.CompanionBox.InventoryGrid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index);
                            if (cell == null) return;
                            //if (cell.GridType != GridType.Inventory || cell.GridType != GridType.CompanionInventory) break;
                            cell?.UseItem();
                            UseHpTime = CEnvir.Now.AddMilliseconds(info.Info.Durability + CEnvir.ClientControl.BigPatchAutoPotionDelayEdit);
                            break;
                        }

                    }
                }
            }
        }
        #endregion

        #region 喝蓝
        static DateTime _warningMpTime = CEnvir.Now;
        static bool Mping = false;
        public static void AutoMP()
        {
            if (Mping || Hping) return;
            if (GameScene.Game.BigPatchBox?.AutoPotionPage == null) return;
            if (GameScene.Game.Observer) return;

            var mp = GameScene.Game.User.CurrentMP;
            var top = GameScene.Game.User.Stats[Stat.Mana];
            if (GameScene.Game.User.Dead || top == 0 || mp == top) return;

            Task.Run(() =>
            {
                try
                {
                    Mping = true;

                    if (!BigPatchConfig.MPAuto)
                    {
                        MpConfig();
                    }
                    else
                    {
                        MpAuto();
                    }

                    Mping = false;
                }
                catch
                {
                    Mping = false;
                }
            });
        }

        private static void MpConfig()
        {
            if (Hping || CEnvir.Now <= UseHpTime || CEnvir.Now <= UseMpTime) return;

            if (GameScene.Game.User.CurrentMP < GameScene.Game.User.Stats[Stat.Mana])
            {
                if (BigPatchConfig.MPAuto)
                {
                    MpAuto();
                    return;
                }

                var links = Links.Where(p => p != null && p.Slot >= 8 && p.Slot <= 11 && p.Enabled && p.Mana > 0).OrderBy(p => p.Mana);
                if (links == null || !links.Any())
                {
                    return;
                }

                if (GameScene.Game.User.CurrentMP > links.Last().Mana) return;

                //相关包裹是否有相关药品
                var indexs = links.Select(p => p.LinkInfoIndex);
                var items = GameScene.Game.Inventory.Where(p => p != null && indexs.Contains(p.InfoIndex));
                if (UseCompanionInvent)
                {
                    var comItems = GameScene.Game.Companion?.InventoryArray?.Where(p => p != null && indexs.Contains(p.InfoIndex));
                    if (comItems != null)
                    {
                        items = items == null ? comItems : items.Union(comItems);
                    }
                }
                if (items == null || !items.Any())
                {
                    if (CEnvir.Now > _warningMpTime)
                    {
                        //CEnvir.Target.Invoke(new Action(() => GameScene.Game.ReceiveChat($">>  警告: 没有蓝药了".Lang(), MessageType.Combat)));
                        _warningMpTime = CEnvir.Now.AddSeconds(10);
                    }
                    return;
                }

                foreach (var item in links)
                {
                    var info = GameScene.Game.Inventory.FirstOrDefault(p => p != null && p.InfoIndex == item.LinkInfoIndex);
                    var isInvent = true;
                    if (info == null && UseCompanionInvent)
                    {
                        info = GameScene.Game.Companion?.InventoryArray?.FirstOrDefault(p => p != null && p.InfoIndex == item.LinkInfoIndex);
                        if (info != null)
                            isInvent = false;
                    }
                    if (item.Mana >= GameScene.Game.User.CurrentMP && info?.Count > 0)
                    {
                        if (GameScene.Game.CanUseItem(info))
                        {
                            var cell = isInvent ?
                                GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index) :
                                 GameScene.Game.CompanionBox.InventoryGrid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index);
                            if (cell == null) continue;
                            cell?.UseItem();
                            UseMpTime = CEnvir.Now.AddMilliseconds(2000 + CEnvir.ClientControl.BigPatchAutoPotionDelayEdit);
                            break;
                        }

                    }
                }
            }
        }

        private static void MpAuto()
        {
            if (Hping || CEnvir.Now <= UseHpTime || CEnvir.Now <= UseMpTime) return;

            if (GameScene.Game.User.CurrentMP < BigPatchConfig.MP)
            {
                if (!BigPatchConfig.MPAuto)
                {
                    MpConfig();
                    return;
                }

                Func<ClientUserItem, bool> where = p => p != null && p.Info.CanAutoPot && p.Info.Stats[Stat.Mana] > 0;
                var items = GameScene.Game.Inventory.Where(where);
                if (UseCompanionInvent && GameScene.Game.Companion != null)
                {
                    var compItems = GameScene.Game.Companion?.InventoryArray?.Where(where);
                    if (compItems != null)
                    {
                        items = items == null ? compItems : items.Union(compItems);
                    }
                }
                var list = items.Select(p => p.Info).OrderByDescending(p => p.Stats[Stat.Mana]);
                if (list == null || !list.Any())
                {
                    if (CEnvir.Now > _warningMpTime)
                    {
                        //CEnvir.Target.Invoke(new Action(() => GameScene.Game.ReceiveChat($">>  警告: 没有蓝药了".Lang(), MessageType.Combat)));
                        _warningMpTime = CEnvir.Now.AddSeconds(10);
                    }
                    return;
                }
                foreach (var item in list)
                {
                    var info = GameScene.Game.Inventory.FirstOrDefault(p => p != null && p.InfoIndex == item.Index);
                    var isInvent = true;
                    if (info == null && UseCompanionInvent)
                    {
                        info = GameScene.Game.Companion?.InventoryArray?.FirstOrDefault(p => p != null && p.InfoIndex == item.Index);
                        if (info != null)
                            isInvent = false;
                    }
                    if (info?.Info != null && GameScene.Game.User.CurrentMP + info.Info.Stats[Stat.Mana] <= BigPatchConfig.MP && info?.Count > 0)
                    {
                        if (GameScene.Game.CanUseItem(info))
                        {
                            var cell = isInvent ?
                                GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index) :
                                GameScene.Game.CompanionBox.InventoryGrid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index);
                            if (cell == null) continue;
                            cell?.UseItem();
                            UseMpTime = CEnvir.Now.AddMilliseconds(2000 + CEnvir.ClientControl.BigPatchAutoPotionDelayEdit);
                            break;
                        }

                    }
                }
            }
        }
        #endregion

        #region 随机或回城
        static DateTime _rollTime = CEnvir.Now;
        static DateTime _warnRoll = CEnvir.Now;
        private static void UseRoll(int slot)
        {
            if (Links[slot] == null) return;
            var roll = Globals.ItemInfoList.Binding.FirstOrDefault(p => p.Index == Links[slot].LinkInfoIndex);
            if (CEnvir.Now < _rollTime && roll?.Shape == 3) return;

            if (!Links[slot].Enabled || Links[slot].Health <= 0) return;
            if (GameScene.Game?.User?.InSafeZone == true) return;

            var link = Links.FirstOrDefault(p => p != null && p.Slot == slot && p.Enabled && p.Health > 0);
            if (link == null)
            {
                return;
            }
            if (GameScene.Game.User.CurrentHP > Links[slot].Health) return;

            var info = GameScene.Game.Inventory.FirstOrDefault(p => p != null && p.InfoIndex == link.LinkInfoIndex);
            var isInvent = true;
            if (info == null && UseCompanionInvent)
            {
                info = GameScene.Game.Companion?.InventoryArray?.FirstOrDefault(p => p != null && p.InfoIndex == link.LinkInfoIndex);
                if (info != null)
                    isInvent = false;
            }
            if (info == null)
            {
                if (CEnvir.Now > _warnRoll)
                {
                    var item = CEnvir.Session.GetCollection<ItemInfo>().Binding.FirstOrDefault(p => p.Index == link.LinkInfoIndex);
                    //CEnvir.Target.Invoke(new Action(() => GameScene.Game.ReceiveChat($">>  警告没有{item?.ItemName ?? "随机传送卷或回城卷"}物品", MessageType.Combat)));
                    _warnRoll = CEnvir.Now.AddSeconds(10);
                }
                return;
            }
            if (link.Health >= GameScene.Game.User.CurrentHP && info.Count > 0)
            {
                var tryCount = 1;
                while (tryCount < 10)
                {
                    if (GameScene.Game?.User?.InSafeZone == true) break;
                    if (GameScene.Game.CanUseItem(info))
                    {
                        var cell = isInvent ?
                            GameScene.Game.InventoryBox.Grid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index) :
                            GameScene.Game.CompanionBox.InventoryGrid.Grid.FirstOrDefault(p => p.Item != null && p.Item?.Info?.Index == info.Info.Index);
                        if (cell == null) break;
                        if (cell.UseItem())
                        {
                            _rollTime = info.Info?.Shape == 3 ? CEnvir.Now.AddSeconds(6) : CEnvir.Now.AddSeconds(3);
                            break;
                        }
                    }
                    tryCount++;
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion

        #region 小退
        static bool _isExiting = false;

        static DateTime _tryTime = CEnvir.Now;
        /// <summary>
        /// 小退
        /// </summary>
        private static void TryExiteGame()
        {
            if (GameScene.Game?.User?.InSafeZone == true) return;

            if (BigPatchConfig.LastHealth != 0 && GameScene.Game.User.CurrentHP > Links[14].Health)
            {
                BigPatchConfig.LastHealth = 0;
                SaveAutoPotionConfig();
            }
            if (!_isExiting && Links[14]?.Enabled == true)
            {
                try
                {
                    _isExiting = true;
                    if (Links[14].Health > GameScene.Game.User.CurrentHP && (BigPatchConfig.LastHealth <= 0 || GameScene.Game.User.CurrentHP < BigPatchConfig.LastHealth) && GameScene.Game?.ExitBox != null)
                    {
                        while (MapObject.User != null)
                        {
                            if (CEnvir.Now < _tryTime) continue;
                            if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && !BigPatchConfig.ChkQuickSelect)
                            {
                                //CEnvir.Target.Invoke(new Action(() =>
                                //{
                                //    _tryTime = CEnvir.Now.AddSeconds(1);
                                //    GameScene.Game.ReceiveChat("战斗中无法退出游戏。".Lang(), MessageType.System);
                                //}));

                                continue;
                            }
                            BigPatchConfig.LastHealth = GameScene.Game.User.CurrentHP;
                            SaveAutoPotionConfig();
                            CEnvir.Enqueue(new C.Logout());
                            break;
                        }

                    }
                    _isExiting = false;
                }
                catch
                {
                    _isExiting = false;
                }

            }
        }
        #endregion

        #region 治愈术
        static DateTime _tryHeal = CEnvir.Now;
        /// <summary>
        /// 治愈术
        /// </summary>
        private static void UseHeal()
        {
            if (Links[15] == null || GameScene.Game?.User?.Class != MirClass.Taoist) return;

            if (!Links[15].Enabled || Links[15].Health == 0) return;
            while (GameScene.Game?.User?.CurrentHP < Links[15].Health)
            {
                if (CEnvir.Now < _tryHeal) continue;
                //CEnvir.Target.Invoke(new Action(() =>
                //{
                //    _tryHeal = CEnvir.Now.AddSeconds(1);
                //    GameScene.Game.UseMagic(MagicType.Heal);
                //}));
            }

        }
        #endregion

        public static void SaveAutoPotionConfig()
        {
            CConfigReader.Save(GameScene.Game.User.Name, GameScene.Game.User.CharacterIndex);
        }
    }
}
