using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //泰山投币
    {
        //已经存在的泰山buff
        public List<CustomBuffInfo> GetAllExistingTaishanBuffs()
        {
            return Buffs.Where(x => x.Type == BuffType.TarzanBuff)
                .Select(y => y.FromCustomBuff)
                .Select(index => Globals.CustomBuffInfoList.Binding
                    .FirstOrDefault(z => z.Index == index))
                .Where(buff => buff != null).OrderBy(i => i.Index).ToList();
        }

        //同组泰山buff只可能存在一个
        public CustomBuffInfo GetExistingTaishanBuffsOfGroup(string groupName)
        {
            return GetAllExistingTaishanBuffs().FirstOrDefault(x => x.BuffGroup == groupName);
        }

        public void TaishanBuffChanged()
        {
            List<CustomBuffInfo> taishanBuffs = GetAllExistingTaishanBuffs();
            Enqueue(new S.TaishanBuffChanged
            {
                BuffIndex1 = taishanBuffs.Count > 0 ? taishanBuffs[0].Index : 0,
                BuffIndex2 = taishanBuffs.Count > 1 ? taishanBuffs[1].Index : 0,
                BuffIndex3 = taishanBuffs.Count > 2 ? taishanBuffs[2].Index : 0,
                BuffIndex4 = taishanBuffs.Count > 3 ? taishanBuffs[3].Index : 0,
                BuffIndex5 = taishanBuffs.Count > 4 ? taishanBuffs[4].Index : 0,
                BuffIndex6 = taishanBuffs.Count > 5 ? taishanBuffs[5].Index : 0,
            });
        }

        public bool LuckyCoinIsOnTarget(int angle, double initialDistance, double selectedDistance)
        {
            double height = Math.Tan(angle * (Math.PI / 180)) * (initialDistance + selectedDistance);

            return height < 5.7 + Config.TossCoinOnTargetRadius && height > 5.7 - Config.TossCoinOnTargetRadius;
        }

        public void LuckyCoinToss(int angle, double initialDistance, double selectedDistance, TossCoinOption option)
        {
            if (GetAllExistingTaishanBuffs().Count > 6)
            {
                //You cannot get new Taishan buff until an existing one is deleted
                Connection.ReceiveChat("Buff已满, 请删除一个后再投币", MessageType.System);
                return;
            }
            if (angle < 0 || angle > 55)
            {
                // Invalid lucky coin tossing angle
                Connection.ReceiveChat("投币角度无效", MessageType.System);
                return;
            }

            if (initialDistance < 3 || initialDistance > 5)
            {
                // Invalid lucky coin tossing distance
                Connection.ReceiveChat("投币距离无效", MessageType.System);
                return;
            }

            int coinsToUse = 1;
            bool isAdvanced = false;
            switch (option)
            {
                case TossCoinOption.Once:
                    coinsToUse = 1;
                    break;
                case TossCoinOption.TenTimes:
                    coinsToUse = 10;
                    break;
                case TossCoinOption.HundredTimes:
                    coinsToUse = 100;
                    break;
                case TossCoinOption.TenTimesAdvanced:
                    coinsToUse = 10;
                    isAdvanced = true;
                    break;
                case TossCoinOption.HundredTimesAdvanced:
                    coinsToUse = 100;
                    isAdvanced = true;
                    break;
                default:
                    // Invalid coin toss option
                    Connection.ReceiveChat("投币选项无效", MessageType.System);
                    return;
            }

            //todo 高级?
            if (isAdvanced)
            {
                Connection.ReceiveChat("高级未开放", MessageType.System);
                return;
            }

            ItemInfo coinInfo = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.LuckyCoins);
            if (coinInfo == null)
            {
                // Cannot find lucky coin item from server db 
                SEnvir.Log("泰山幸运币未设置。请设置相应道具的效果为LuckyCoins 泰山buff幸运币");
                Connection.ReceiveChat("服务器无法找到幸运币物品", MessageType.System);
                return;
            }

            int remainingFreeCoins = Config.DailyFreeCoins - Character.DailyFreeTossUsed;
            if (remainingFreeCoins < 0)
            {
                // 使用的免费次数大于服务器设置
                SEnvir.Log($"玩家 {Name} 的免费投币次数超过服务端设置, 如果0点前未修改过免费次数, 请将此情况报告给开发团队");
                remainingFreeCoins = 0;
            }

            if (remainingFreeCoins > 0)
            {
                //免费次数未耗尽
                if (option == TossCoinOption.Once || option == TossCoinOption.HundredTimes ||
                    option == TossCoinOption.TenTimes)
                {
                    if (remainingFreeCoins >= coinsToUse)
                    {
                        Character.DailyFreeTossUsed += coinsToUse;
                        coinsToUse = 0;
                    }
                    else
                    {
                        Character.DailyFreeTossUsed += remainingFreeCoins;
                        coinsToUse -= remainingFreeCoins;
                    }
                }
            }

            if (GetItemCountFromInventory(coinInfo) < coinsToUse)
            {
                // You don't have enough lucky coin
                Connection.ReceiveChat("你的幸运币数量不足", MessageType.System);
                return;
            }

            if (coinsToUse > 0) TakeItem(coinInfo, coinsToUse);
            Enqueue(new S.FreeCoinCountChanged { Count = Config.DailyFreeCoins - Character.DailyFreeTossUsed });

            //进行投币
            bool onTarget = false;
            for (int i = 0; i < coinsToUse; i++)
            {
                //是否命中？
                if (LuckyCoinIsOnTarget(angle, initialDistance, selectedDistance))
                {
                    onTarget = true;
                    //如果玩家已经有6个泰山buff 新的buff只能在6个组里面选
                    string groupName;
                    var existingBuffs = GetAllExistingTaishanBuffs();
                    groupName = existingBuffs.Count >= 6 ? existingBuffs[SEnvir.Random.Next(6)].BuffGroup : GetRandomTaishanBuffGroup();

                    CustomBuffInfo newBuff = LuckyCoinGetOneBuff(groupName);
                    CustomBuffInfo existingBuff = GetExistingTaishanBuffsOfGroup(newBuff.BuffGroup);
                    if (existingBuff == null)
                    {
                        //加上此BUFF
                        CustomBuffAdd(newBuff.Index);
                        //New buff {} has been added to your character
                        //Connection.ReceiveChat($"你获得了BUFF: {newBuff.BuffName}", MessageType.System);
                    }
                    else
                    {
                        //新buff等级更高 则替换旧buff
                        //否则不做操作 发客户端提示
                        if (newBuff.BuffLV > existingBuff.BuffLV)
                        {
                            CustomBuffRemove(existingBuff.Index);
                            CustomBuffAdd(newBuff.Index);
                            //Your buff {} has been removed
                            //Connection.ReceiveChat($"BUFF已移除: {existingBuff.BuffName}", MessageType.System);
                            //New buff {} has been added to your character
                            //Connection.ReceiveChat($"你获得了BUFF: {newBuff.BuffName}", MessageType.System);
                        }
                    }
                }
            }
            //发包通知客户端
            Enqueue(new S.CoinTossOnTarget { IsOnTarget = onTarget });
            TaishanBuffChanged();
        }

        public void RemoveTaishanBuff(int index)
        {
            List<CustomBuffInfo> buffs = GetAllExistingTaishanBuffs();
            CustomBuffInfo buff = buffs.FirstOrDefault(x => x.Index == index);
            if (buff == null)
            {
                //Cannot delete selected Taishan buff
                Connection.ReceiveChat("无法删除此Buff", MessageType.System);
                return;
            }

            CustomBuffRemove(index);
            TaishanBuffChanged();
        }

        public string GetRandomTaishanBuffGroup()
        {
            if (Globals.CustomBuffGroupsDict.Count < 1) return null;
            return Globals.CustomBuffGroupsDict.ElementAt(SEnvir.Random.Next(0, Globals.CustomBuffGroupsDict.Count))
                .Key;
        }

        public List<CustomBuffInfo> GetRandomTaishanBuffsOfGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return null;
            if (!Globals.CustomBuffGroupsDict.ContainsKey(groupName)) return null;

            //对应的buff
            return Globals.CustomBuffGroupsDict[groupName].Select(index => Globals.CustomBuffInfoList.Binding.First(x => x.Index == index)).ToList();
        }

        public CustomBuffInfo LuckyCoinGetOneBuff(string groupName = "")
        {
            //如果新随机的泰山buff分组已存在于玩家身上
            //已存在用Rate2 否则用Rate1
            bool useRate1 = GetExistingTaishanBuffsOfGroup(groupName) == null;

            List<KeyValuePair<CustomBuffInfo, int>> candidates = new List<KeyValuePair<CustomBuffInfo, int>>();
            if (string.IsNullOrEmpty(groupName))
            {
                //未指定分组 随机挑一个分组
                foreach (CustomBuffInfo info in GetRandomTaishanBuffsOfGroup(GetRandomTaishanBuffGroup()))
                {
                    candidates.Add(new KeyValuePair<CustomBuffInfo, int>(info, useRate1 ? info.GetRate : info.GetRate2));
                }
            }
            else
            {
                //指定分组
                foreach (CustomBuffInfo info in GetRandomTaishanBuffsOfGroup(groupName))
                {
                    candidates.Add(new KeyValuePair<CustomBuffInfo, int>(info, useRate1 ? info.GetRate : info.GetRate2));
                }
            }

            //从选定的分组随机buff
            CustomBuffInfo selectedBuff = Functions.WeightedRandom(SEnvir.Random, candidates);
            return selectedBuff;
        }
    }
}