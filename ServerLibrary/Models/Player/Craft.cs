using Library;
using Library.SystemModels;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Utils.Logging;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //制作
    {
        public void CheckCrafting()
        {
            if (CraftingItem == null) return;
            if (CraftFinishTime <= SEnvir.Now)
            {
                //制作完成
                FinalizeCrafting();
            }
        }
        public void InitializeCrating(int targetIndex)
        {
            if (CraftingItem != null)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = "上一个制作物品尚未结束".Lang(Connection.Language) });
                return;
            }
            CraftItemInfo targetItem = SEnvir.CraftItemInfoList.Binding.FirstOrDefault(x => x.Index == targetIndex);
            if (targetItem == null)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = "制作目标未定义".Lang(Connection.Language) });
                return;
            }

            if (CraftLevel < targetItem.LevelNeeded)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = "制作等级不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.GoldCost > Gold)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = "金币不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.Item1 != null && GetItemCount(targetItem.Item1) < targetItem.Amount1)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = targetItem.Item1.ItemName + "  " + "数量不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.Item2 != null && GetItemCount(targetItem.Item2) < targetItem.Amount2)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = targetItem.Item2.ItemName + "  " + "数量不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.Item3 != null && GetItemCount(targetItem.Item3) < targetItem.Amount3)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = targetItem.Item3.ItemName + "  " + "数量不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.Item4 != null && GetItemCount(targetItem.Item4) < targetItem.Amount4)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = targetItem.Item4.ItemName + "  " + "数量不足".Lang(Connection.Language) });
                return;
            }

            if (targetItem.Item5 != null && GetItemCount(targetItem.Item5) < targetItem.Amount5)
            {
                Enqueue(new S.CraftStartFailed { TargetItemIndex = targetIndex, Reason = targetItem.Item5.ItemName + "  " + "数量不足".Lang(Connection.Language) });
                return;
            }

            //扣除物品
            ChangeGold(-targetItem.GoldCost);
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "制作系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = targetItem.GoldCost,
                ExtraInfo = $"制作系统扣除制作费用"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            if (targetItem.Item1 != null)
            {
                TakeItem(targetItem.Item1, targetItem.Amount1);
            }
            if (targetItem.Item2 != null)
            {
                TakeItem(targetItem.Item2, targetItem.Amount2);
            }
            if (targetItem.Item3 != null)
            {
                TakeItem(targetItem.Item3, targetItem.Amount3);
            }
            if (targetItem.Item4 != null)
            {
                TakeItem(targetItem.Item4, targetItem.Amount4);
            }
            if (targetItem.Item5 != null)
            {
                TakeItem(targetItem.Item5, targetItem.Amount5);
            }

            CraftFinishTime = SEnvir.Now + new TimeSpan(0, 0, targetItem.TimeCost);
            CraftingItem = targetItem;
            //发包
            Enqueue(new S.CraftAcknowledged { TargetItemIndex = targetItem.Index, CompleteTime = CraftFinishTime });
        }
        public void FinalizeCrafting()
        {
            if (CraftingItem == null)
            {
                Enqueue(new S.CraftResult { Succeed = false, Reason = "制作目标未定义".Lang(Connection.Language) });
                return;
            }
            if (DateTime.Compare(SEnvir.Now, CraftFinishTime) < 0)
            {
                Enqueue(new S.CraftResult { Succeed = false, Reason = CraftingItem.Item.ItemName + "  " + "仍在制造中".Lang(Connection.Language) });
                return;
            }

            if (SEnvir.Random.Next(100) < CraftingItem.SuccessRate) //成功
            {
                //发信息
                Enqueue(new S.CraftResult { Succeed = true, Reason = CraftingItem.Item.ItemName + "  " + "制造成功".Lang(Connection.Language) + "！" });
                //给物品
                ItemCheck check = new ItemCheck(CraftingItem.Item, CraftingItem.TargetAmount, UserItemFlags.None,
                    TimeSpan.Zero);

                //todo 极品几率
                UserItem item = SEnvir.CreateDropItem(check, 2);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "制造系统".Lang(Connection.Language), Character?.CharacterName);

                //给不了物品 改用信件发送 
                if (RemainingSlots(GridType.Inventory) < (CraftingItem.TargetAmount / CraftingItem.Item.StackSize) + 1)
                {
                    //信件发送
                    MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                    mail.Account = Character.Account;
                    mail.Subject = "制造成功".Lang(Connection.Language);
                    mail.Sender = "制造系统".Lang(Connection.Language);
                    mail.Message = $"PlayerObject.CraftSuccess".Lang(Connection.Language, item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count);

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
                //加经验
                CraftExp += CraftingItem.GainExp;
            }
            else
            {
                //失败 发信息
                Enqueue(new S.CraftResult { Succeed = false, Reason = "PlayerObject.CraftFailure".Lang(Connection.Language, CraftingItem.Item.ItemName) });
            }
            //重置
            CraftingItem = null;
            CraftFinishTime = DateTime.MaxValue;
        }

        public void CancelCrafting()
        {
            //重置
            CraftingItem = null;
            CraftFinishTime = DateTime.MaxValue;
            Enqueue(new S.CraftResult { Succeed = false, Reason = "制作已取消".Lang(Connection.Language) });
        }
    }
}
