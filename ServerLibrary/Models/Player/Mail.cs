using IronPython.Runtime;
using Library;
using Library.SystemModels;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using Server.Utils.Logging;
using System;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject // 人物邮件
    {
        #region Mail
        //邮件
        public void MailGetItem(C.MailGetItem p)  //邮件获得道具
        {
            MailInfo mail = Character.Account.Mail.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null)
            {
                Enqueue(new S.MailDelete { Index = p.Index, ObserverPacket = true });
                return;
            }

            UserItem item = mail.Items.FirstOrDefault(x => x.Slot == p.Slot);

            if (item == null)
            {
                Enqueue(new S.MailItemDelete { Index = p.Index, Slot = p.Slot, ObserverPacket = true });
                return;
            }

            if (!InSafeZone && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat("Mail.MailSafeZone".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)))
            {
                Connection.ReceiveChat("Mail.MailNeedSpace".Lang(Connection.Language), MessageType.System);
                return;
            }

            item.Mail = null;
            GainItem(item);

            Enqueue(new S.MailItemDelete { Index = p.Index, Slot = p.Slot, ObserverPacket = true });
        }
        public void MailDelete(int index)  //邮件删除
        {
            MailInfo mail = Character.Account.Mail.FirstOrDefault(x => x.Index == index);

            if (mail == null)
            {
                Enqueue(new S.MailDelete { Index = index, ObserverPacket = true });
                return;
            };

            if (mail.Items.Count > 0)
            {
                Connection.ReceiveChat("Mail.MailHasItems".Lang(Connection.Language), MessageType.System);
                return;
            }

            mail.Delete();

            Enqueue(new S.MailDelete { Index = index, ObserverPacket = true });
        }
        public void MailSend(C.MailSend p)  //邮件发送
        {
            Enqueue(new S.MailSend { ObserverPacket = false });

            S.ItemsChanged result = new S.ItemsChanged { Links = p.Links };

            Enqueue(result);

            if (!ParseLinks(p.Links, 0, 7)) return;    //邮件发送物品数量

            AccountInfo account = SEnvir.GetCharacter(p.Recipient)?.Account;

            if (account == null || SEnvir.IsBlocking(Character.Account, account))
            {
                Connection.ReceiveChat("Mail.MailNotFound".Lang(Connection.Language, p.Recipient), MessageType.System);
                return;
            }

            if (account == Character.Account && !Character.Account.TempAdmin)
            {
                Connection.ReceiveChat("Mail.MailSelfMail".Lang(Connection.Language), MessageType.System);
                return;
            }
            if (p.Gold < 0 || p.Gold > Gold)
            {
                Connection.ReceiveChat("Mail.MailMailCost".Lang(Connection.Language), MessageType.System);
                return;
            }
            if (p.Gold + 2000 > Gold)
            {
                Connection.ReceiveChat("金币不足2000，无法发送邮件".Lang(Connection.Language), MessageType.System);
                return;
            }
            UserItem item;  //用户项目 道具
            foreach (CellLinkInfo link in p.Links)
            {
                UserItem[] fromArray;

                switch (link.GridType)
                {
                    case GridType.Inventory:            //背包
                        if (!InSafeZone && !Character.Account.TempAdmin)
                        {
                            Connection.ReceiveChat("Mail.MailSendSafeZone".Lang(Connection.Language), MessageType.System);   //邮件判断如果不是在安全区，那么提示语
                            return;
                        }
                        fromArray = Inventory;
                        break;
                    case GridType.PatchGrid:                    //碎片包裹
                        if (!InSafeZone && !Character.Account.TempAdmin)
                        {
                            Connection.ReceiveChat("Mail.MailSendSafeZone".Lang(Connection.Language), MessageType.System);   //邮件判断如果不是在安全区，那么提示语
                            return;
                        }
                        fromArray = PatchGrid;
                        break;
                    case GridType.Storage:       //仓库
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:
                        if (Companion == null) return;   //邮件判断如果没有宠物就返回

                        if (!InSafeZone && !Character.Account.TempAdmin)   //如果不在安全区，那么提示语
                        {
                            Connection.ReceiveChat("Mail.MailSendSafeZone".Lang(Connection.Language), MessageType.System);
                            return;
                        }

                        fromArray = Companion.Inventory;
                        break;
                    default:
                        return;
                }

                if (link.Slot < 0 || link.Slot >= fromArray.Length) return;

                item = fromArray[link.Slot];

                if (item == null || link.Count > item.Count) return;
                if (((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound || !item.Info.CanTrade) && !account.Admin && !Character.Account.Admin) return;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;
                //Success
            }

            Gold -= 2000;
            GoldChanged();

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            mail.Account = account;
            mail.Sender = Name;
            mail.Subject = p.Subject;
            mail.Message = p.Message;

            result.Success = true;

            if (p.Gold > 0)
            {
                Gold -= p.Gold;
                GoldChanged();

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "邮件系统",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.Gold,
                    Action = CurrencyAction.Deduct,
                    Source = CurrencySource.ItemAdd,
                    Amount = p.Gold,
                    ExtraInfo = $"邮件发送金币给{account}{p.Recipient}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                item = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                item.Count = p.Gold;
                item.Slot = mail.Items.Count;
                item.Mail = mail;
            }

            foreach (CellLinkInfo link in p.Links)
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
                    case GridType.Storage:   //仓库
                        fromArray = Storage;
                        break;
                    case GridType.CompanionInventory:  //宠物包裹
                        fromArray = Companion.Inventory;
                        break;
                }

                item = fromArray[link.Slot];

                if (link.Count == item.Count)
                {
                    RemoveItem(item);
                    fromArray[link.Slot] = null;
                }
                else
                {
                    item.Count -= link.Count;

                    UserItem newItem = SEnvir.CreateFreshItem(item);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(item, newItem);

                    item = newItem;
                    item.Count = link.Count;
                }

                item.Slot = mail.Items.Count;
                item.Mail = mail;

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "邮件",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.None,
                    Action = CurrencyAction.Undefined,
                    Source = CurrencySource.OtherAdd,
                    Amount = item.Count,
                    ExtraInfo = $"{Character}邮件发送给{p.Recipient}: {item.Info.ItemName}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);
            }

            if (p.Links.Count > 0)
            {
                Companion?.RefreshWeight();
                RefreshWeight();
            }

            mail.HasItem = mail.Items.Count > 0;

            if (account.Connection?.Player != null)
                account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });

            Connection.ReceiveChat("Mail.MailSentSuccessfully".Lang(Connection.Language), MessageType.System);
        }

        public void PYMailSend(string subject, string sender, string message, IronPython.Runtime.List rewards, string recipient = "") //PY信件发送
        {
            // rewards 的格式是[(物品1名称, 数量, 是否绑定)， (物品2名称, 数量, 是否绑定)...]
            // 一封邮件最多7个物品
            if (rewards == null || rewards.Count < 0 || rewards.Count > 7) return;

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            //收件人默认是自己
            AccountInfo account = Character.Account;
            if (!string.IsNullOrEmpty(recipient))
            {
                //收件人不是当前玩家
                account = SEnvir.GetCharacter(recipient)?.Account;
            }

            if (account == null)
                return;

            mail.Account = account;
            mail.Subject = subject;
            mail.Sender = sender;
            mail.Message = message;
            mail.HasItem = rewards.Count > 0;

            foreach (PythonTuple reward in rewards)
            {
                if (reward == null || reward.Count != 3) continue;

                string itemName = (string)reward[0];
                int amount = Convert.ToInt32(reward[1]);
                bool bound = (bool)reward[2];

                UserItemFlags flag = bound ? UserItemFlags.Bound : UserItemFlags.None;
                ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == itemName);
                if (info == null) continue;

                TimeSpan duration = TimeSpan.FromSeconds(info.Duration);

                if (duration != TimeSpan.Zero) flag |= UserItemFlags.Expirable;

                ItemCheck check = new ItemCheck(info, amount, flag, duration);
                UserItem item = SEnvir.CreateFreshItem(check);

                //记录物品来源
                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.NPC, "脚本系统".Lang(Connection.Language), Character?.CharacterName);

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "脚本邮件",
                    Time = SEnvir.Now,
                    Character = Character,
                    Currency = CurrencyType.None,
                    Action = CurrencyAction.Undefined,
                    Source = CurrencySource.OtherAdd,
                    Amount = mail.Items.Count,
                    ExtraInfo = $"{Character}邮件发送给{recipient}: {item.Info.ItemName}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                item.Slot = mail.Items.Count;
                item.Mail = mail;

            }

            if (account.Connection?.Player != null)
            {
                account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });
            }
        }
        #endregion
    }
}
