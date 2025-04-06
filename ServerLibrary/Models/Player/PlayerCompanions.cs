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
    public partial class PlayerObject : MapObject //宠物
    {
        /// <summary>
        /// 宠物
        /// </summary>
        public Companion Companion;
        /// <summary>
        /// 宠物等级锁定
        /// </summary>
        public bool CompanionLevelLock3, CompanionLevelLock5, CompanionLevelLock7, CompanionLevelLock10, CompanionLevelLock11, CompanionLevelLock13, CompanionLevelLock15;
        /// <summary>
        /// 宠物列表
        /// </summary>
        public List<MonsterObject> Pets = new List<MonsterObject>();
        /// <summary>
        /// 宠物模式
        /// </summary>
        public PetMode PetMode
        {
            get { return Character.PetMode; }
            set { Character.PetMode = value; }
        }

        public HashSet<int> CompanionPickUpSkips = new HashSet<int>();

        #region Companions

        /// <summary>
        /// 宠物解锁
        /// </summary>
        /// <param name="index"></param>
        public void CompanionUnlock(int index)
        {
            S.CompanionUnlock result = new S.CompanionUnlock();
            Enqueue(result);

            CompanionInfo info = SEnvir.CompanionInfoList.Binding.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (info.Available || Character.Account.CompanionUnlocks.Any(x => x.CompanionInfo == info))
            {
                Connection.ReceiveChat("Companion.CompanionAppearanceAlready".Lang(Connection.Language, info.MonsterInfo.MonsterName), MessageType.System);
                return;
            }

            UserItem item = null;
            int slot = 0;
            // 寻找宠物解锁券
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.CompanionTicket) continue;


                item = Inventory[i];
                slot = i;
                break;
            }
            // 找不到宠物解锁券
            if (item == null)
            {
                Connection.ReceiveChat("Companion.CompanionNeedTicket".Lang(Connection.Language), MessageType.System);
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
            // 解锁结果
            result.Index = info.Index;

            UserCompanionUnlock unlock = SEnvir.UserCompanionUnlockList.CreateNewObject();
            unlock.Account = Character.Account;
            unlock.CompanionInfo = info;
        }
        /// <summary>
        /// 宠物解锁自动粮仓
        /// </summary>
        /// <param name="index"></param>
        public void CompanionAutoFeedUnlock(int index)
        {
            UserCompanion info = Character.Account.Companions.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (info.AutoFeed) return; //已经开启过无需再开

            UserItem item = null;
            int slot = 0;
            // 寻找宠物自动粮仓券
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null || Inventory[i].Info.Effect != ItemEffect.CompanionAutoBarn) continue;


                item = Inventory[i];
                slot = i;
                break;
            }
            // 找不到宠物自动粮仓券
            if (item == null)
            {
                Connection.ReceiveChat("Companion.CompanionNeedGrainTicket".Lang(Connection.Language), MessageType.System);
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

            // 解锁成功
            info.AutoFeed = true;
            // 通知客户端刷新解锁图标
            Enqueue(new S.CompanionAutoFeedUnlocked { Index = info.Index, AutoFeedUnlocked = true });
        }
        /// <summary>
        /// 宠物购买
        /// </summary>
        /// <param name="p"></param>
        public void CompanionAdopt(C.CompanionAdopt p)
        {
            S.CompanionAdopt result = new S.CompanionAdopt();
            Enqueue(result);
            //如果 死亡 NPC为空 NPC页为空 跳出
            if (Dead || NPC == null || NPCPage == null) return;
            //如果NPC页不是宠物管理 跳出
            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;

            CompanionInfo info = SEnvir.CompanionInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            //如果宠物为空 跳出
            if (info == null) return;
            //如果宠物不能直接购买 或 没有解绑 提示信息并跳出
            if (!info.Available && Character.Account.CompanionUnlocks.All(x => x.CompanionInfo != info))
            {
                Connection.ReceiveChat("Companion.CompanionAppearanceAlready".Lang(Connection.Language), MessageType.System);
                return;
            }
            //如果购买金币不够 提示信息并跳出
            if (info.Price > Gold)
            {
                Connection.ReceiveChat("Companion.CompanionNeedGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (info.GameGoldPrice > GameGold)
            {
                Connection.ReceiveChat("Companion.CompanionNeedGameGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            //宠物取名规则不对 提示信息并跳出
            if (Config.CanUseChineseGuildName)
            {
                if (!Globals.GuildNameRegex.IsMatch(p.Name))
                {
                    Connection.ReceiveChat("Companion.CompanionBadName".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }
            else
            {
                if (!Globals.EnGuildNameRegex.IsMatch(p.Name))
                {
                    Connection.ReceiveChat("Companion.CompanionBadName".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            //金币大于0 金币减少
            if (info.Price > 0)
            {
                Gold -= info.Price;
                GoldChanged();

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "宠物购买系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = info.Price,
                    ExtraInfo = $"宠物购买扣除金币"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                //获得新宠物
                UserCompanion companion = SEnvir.UserCompanionList.CreateNewObject();

                companion.Account = Character.Account;
                companion.Info = info;
                companion.Level = 1;
                companion.Hunger = 100;
                companion.Name = p.Name;

                result.UserCompanion = companion.ToClientInfo();
            }
            else
            {
                GameGold -= info.GameGoldPrice;
                GameGoldChanged();

                //获得新宠物
                UserCompanion companion = SEnvir.UserCompanionList.CreateNewObject();

                companion.Account = Character.Account;
                companion.Info = info;
                companion.Level = 1;
                companion.Hunger = 100;
                companion.Name = p.Name;

                result.UserCompanion = companion.ToClientInfo();
            }
        }
        /// <summary>
        /// 宠物取回
        /// </summary>
        /// <param name="index"></param>
        public void CompanionRetrieve(int index)
        {
            if (Dead || NPC == null || NPCPage == null) return;

            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;


            UserCompanion info = Character.Account.Companions.FirstOrDefault(x => x.Index == index);

            if (info == null) return;

            if (info.Character != null)
            {
                if (info.Character != Character)
                    Connection.ReceiveChat("Companion.CompanionRetrieveFailed".Lang(Connection.Language, info.Name, info.Character.CharacterName), MessageType.System);
                return;
            }

            info.Character = Character;

            Enqueue(new S.CompanionStore());
            Enqueue(new S.CompanionRetrieve { Index = index });

            CompanionDespawn();
            CompanionSpawn();

        }
        /// <summary>
        /// 宠物寄存
        /// </summary>
        /// <param name="index"></param>
        public void CompanionStore(int index)
        {
            //如果 宠物死亡 或者 NPC为零 或者 NPC页为零 那么 返回
            if (Dead || NPC == null || NPCPage == null) return;
            //如果NPC页的类型不是宠物管理 那么 返回
            if (NPCPage.DialogType != NPCDialogType.CompanionManage) return;
            //如果角色的宠物为零 那么 返回
            if (Character.Companion == null) return;
            //角色宠物 为零
            Character.Companion = null;
            //发送 新的宠物商店信息
            Enqueue(new S.CompanionStore());
            //跳转到 宠物离开
            CompanionDespawn();
        }
        /// <summary>
        /// 宠物生成
        /// </summary>
        public void CompanionSpawn()
        {
            if (Companion != null) return;

            if (Character.Companion == null) return;

            Companion tempCompanion = new Companion(Character.Companion)
            {
                CompanionOwner = this,
            };


            if (tempCompanion.Spawn(CurrentMap.Info, CurrentLocation))
            {
                Companion = tempCompanion;
                CompanionApplyBuff();
            }
        }
        /// <summary>
        /// 宠物对应技能BUFF
        /// </summary>
        public void CompanionApplyBuff()
        {
            if (Companion.UserCompanion.Hunger <= 0) return;

            Stats buffStats = new Stats();

            if (Companion.UserCompanion.Level >= 3)
                buffStats.Add(Companion.UserCompanion.Level3);

            if (Companion.UserCompanion.Level >= 5)
                buffStats.Add(Companion.UserCompanion.Level5);

            if (Companion.UserCompanion.Level >= 7)
                buffStats.Add(Companion.UserCompanion.Level7);

            if (Companion.UserCompanion.Level >= 10)
                buffStats.Add(Companion.UserCompanion.Level10);

            if (Companion.UserCompanion.Level >= 11)
                buffStats.Add(Companion.UserCompanion.Level11);

            if (Companion.UserCompanion.Level >= 13)
                buffStats.Add(Companion.UserCompanion.Level13);

            if (Companion.UserCompanion.Level >= 15)
                buffStats.Add(Companion.UserCompanion.Level15);

            BuffInfo buff = BuffAdd(BuffType.Companion, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.FromMinutes(1));
            buff.TickTime = TimeSpan.FromMinutes(1); //设置为整分钟
        }
        /// <summary>
        /// 宠物离开
        /// </summary>
        public void CompanionDespawn()
        {
            if (Companion == null) return;  //如果宠物为零 返回

            BuffRemove(BuffType.Companion);  //BUFF移除  宠物BUFF

            Companion.CompanionOwner = null;  //宠物的所有者 为零
            Companion.Despawn();   //跳转到宠物的消失
            Companion = null;   //宠物为零
        }
        /// <summary>
        /// 刷新宠物BUFF属性状态
        /// </summary>
        public void CompanionRefreshBuff()
        {
            if (Companion.UserCompanion.Hunger <= 0) return;

            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.Companion);

            if (buff == null) return;

            Stats buffStats = new Stats();

            if (Companion.UserCompanion.Level >= 3)
                buffStats.Add(Companion.UserCompanion.Level3);

            if (Companion.UserCompanion.Level >= 5)
                buffStats.Add(Companion.UserCompanion.Level5);

            if (Companion.UserCompanion.Level >= 7)
                buffStats.Add(Companion.UserCompanion.Level7);

            if (Companion.UserCompanion.Level >= 10)
                buffStats.Add(Companion.UserCompanion.Level10);

            if (Companion.UserCompanion.Level >= 11)
                buffStats.Add(Companion.UserCompanion.Level11);

            if (Companion.UserCompanion.Level >= 13)
                buffStats.Add(Companion.UserCompanion.Level13);

            if (Companion.UserCompanion.Level >= 15)
                buffStats.Add(Companion.UserCompanion.Level15);


            buff.Stats = buffStats;
            RefreshStats();

            Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buffStats });
        }

        #endregion
    }
}
