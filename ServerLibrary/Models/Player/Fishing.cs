using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //钓鱼
    {
        public void FishingCast()
        {
            UserItem rod = Equipment[(int)EquipmentSlot.Weapon];
            UserItem armour = Equipment[(int)EquipmentSlot.Armour];
            UserItem hook = FishingEquipment[(int)FishingSlot.Hook];
            UserItem fishingFloat = FishingEquipment[(int)FishingSlot.Float];
            UserItem bait = FishingEquipment[(int)FishingSlot.Bait];
            UserItem finder = FishingEquipment[(int)FishingSlot.Finder];
            UserItem reel = FishingEquipment[(int)FishingSlot.Reel];

            //要有鱼竿
            if (rod == null || (rod.Info.Shape != 125 && rod.Info.Shape != 126))
            {
                IsFishing = false;
                Connection.ReceiveChat("你没有装备钓竿".Lang(Connection.Language), MessageType.System);
                return;
            }
            //要穿蓑衣 否则没有钓鱼动作
            if (armour == null || armour.Info.Shape != 16)
            {
                IsFishing = false;
                Connection.ReceiveChat("你没有穿蓑衣".Lang(Connection.Language), MessageType.System);
                return;
            }
            //要装备鱼钩
            if (hook == null)
            {
                IsFishing = false;
                Connection.ReceiveChat("你没有装备鱼钩".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (IsFishing)
            {
                Connection.ReceiveChat("你已经开始钓鱼".Lang(Connection.Language), MessageType.System);
                return;
            }

            //检查钓鱼地块,检查人物正前方第2个坐标是不是钓鱼块
            Point fishingPoint = Functions.Move(CurrentLocation, Direction, 2);
            if (fishingPoint.X < 0 || fishingPoint.Y < 0 || CurrentMap.Width < fishingPoint.X || CurrentMap.Height < fishingPoint.Y)
            {
                IsFishing = false;
                return;
            }

            bool fishingCell = CurrentMap.FishingCells[fishingPoint.X, fishingPoint.Y];

            if (!fishingCell)
            {
                IsFishing = false;
                return;
            }

            DamageItem(GridType.Equipment, (int)EquipmentSlot.Weapon, 1);
            DamageItem(GridType.FishingEquipment, (int)FishingSlot.Hook, 1);

            if (fishingFloat != null)
            {
                DamageItem(GridType.FishingEquipment, (int)FishingSlot.Float, 1);
            }

            if (bait != null)
            {
                //DamageItem(GridType.FishingEquipment, (int)FishingSlot.Bait, 1);
                //鱼饵是消耗品 每次消耗一个
                FishingEquipment[(int)FishingSlot.Bait].Count--;
                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.FishingEquipment, Slot = (int)FishingSlot.Bait, Count = FishingEquipment[(int)FishingSlot.Bait].Count }, Success = true });

                if (FishingEquipment[(int)FishingSlot.Bait].Count <= 0)
                {
                    RemoveItem(FishingEquipment[(int)FishingSlot.Bait]); //移除项目（道具）
                    FishingEquipment[(int)FishingSlot.Bait].Delete();  //道具 删除
                    FishingEquipment[(int)FishingSlot.Bait] = null; //背包值[i] 为空
                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.FishingEquipment, Slot = (int)FishingSlot.Bait }, Success = true });

                }
            }

            if (finder != null)
            {
                DamageItem(GridType.FishingEquipment, (int)FishingSlot.Finder, 1);
            }

            FishingStartTime = SEnvir.Now;
            FishingPerfectTime = DateTime.MaxValue;

            double totalMs = (Globals.MaxFishingWaitingTime - Globals.MinFishingWaitingTime).TotalMilliseconds;
            double endingMs = SEnvir.Random.NextDouble() * totalMs + Globals.MinFishingWaitingTime.TotalMilliseconds; //结束距离开始的时间

            FishingFinishTime = SEnvir.Now + TimeSpan.FromMilliseconds(endingMs);

            IsFishing = true;

            if (SEnvir.Random.Next(100) < TotalFindingChance)
            {
                //找到鱼
                double perfectMs = SEnvir.Random.NextDouble() * (FishingFinishTime - FishingStartTime).TotalMilliseconds;
                FishingPerfectTime = FishingStartTime + TimeSpan.FromMilliseconds(perfectMs);
                FoundFish = true;
            }
            else
            {
                //没找到鱼
                FishingFindingFailedTimes += 1;
            }

            Enqueue(new S.FishingStarted { StartTime = FishingStartTime, EndTime = FishingFinishTime, PerfectTime = FishingPerfectTime, FindingChance = TotalFindingChance });
            Broadcast(new S.ObjectFishing
            {
                ObjectID = ObjectID,
                Direction = Direction,
                Location = CurrentLocation,
                Action = MirAction.FishingCast
            });
        }

        public void FishingReel(DateTime reelTime)
        {
            if (!IsFishing || reelTime < FishingStartTime || reelTime > FishingFinishTime)
            {
                Connection.ReceiveChat("你没有在钓鱼".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (reelTime <= FishingStartTime + Globals.MinFishingWaitingTime)
            {
                //Connection.ReceiveChat("现在收杆太早了".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!FoundFish)
            {
                //Connection.ReceiveChat($"这次你没有找到鱼，但是下一次找到鱼成功率会增加{FishingFindingFailedTimes * Globals.FishingBaseFindingFailedAdd}%", MessageType.System);
                Connection.ReceiveChat($"鱼脱离进食".Lang(Connection.Language), MessageType.System);
                ResetFishing(false);
                Enqueue(new S.FishingEnded());
                Broadcast(new S.ObjectFishing
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Action = MirAction.FishingReel
                });
                return;
            }

            if (FoundFish && reelTime > FishingPerfectTime - Globals.FishingWindowTime && reelTime < FishingPerfectTime + Globals.FishingWindowTime)
            {
                //完美收杆 一定有鱼
                FishingSuccess = true;
                GiveFish();

                ResetFishing(true);
                Enqueue(new S.FishingEnded());
                Broadcast(new S.ObjectFishing
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Action = MirAction.FishingReel
                });
                return;
            }

            if (IsFishing && FoundFish && reelTime > FishingStartTime && reelTime < FishingFinishTime)
            {
                //一定几率有鱼
                bool reelSuccess = SEnvir.Random.Next(100) < TotalNibbleChance;
                if (reelSuccess)
                {
                    FishingSuccess = true;
                    GiveFish();
                    ResetFishing(true);
                }
                else
                {
                    Connection.ReceiveChat("很遗憾鱼儿跑掉了".Lang(Connection.Language), MessageType.System);
                    ResetFishing(true);
                }
                Enqueue(new S.FishingEnded());
                Broadcast(new S.ObjectFishing
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Action = MirAction.FishingReel
                });
            }

        }

        public void GiveFish()
        {
            if (SEnvir.FishList == null || SEnvir.FishList.Count < 1) return;
            //每15秒打乱一次奖品列表
            if (SEnvir.Now > FishListShuffleTime)
            {
                SEnvir.FishList.Shuffle(SEnvir.Random);
                FishListShuffleTime = SEnvir.Now + TimeSpan.FromSeconds(15);
            }

            //随机选一个鱼
            ItemInfo fishInfo = SEnvir.FishList[SEnvir.Random.Next(SEnvir.FishList.Count)];

            //对每种鱼计算概率 
            foreach (ItemInfo fish in SEnvir.FishList)
            {
                if (SEnvir.Random.Next(100) < fish.Stats[Stat.FishObtainChance] + Stats[Stat.FishObtainItemChance])
                {
                    fishInfo = fish;
                    break;
                }
            }

            ItemCheck fishCheck = new ItemCheck(fishInfo, 1, UserItemFlags.None, TimeSpan.Zero);

            if (CanGainItems(false, fishCheck))
            {
                UserItem fish = SEnvir.CreateFreshItem(fishCheck);
                //记录物品来源
                SEnvir.RecordTrackingInfo(fish, CurrentMap?.Info?.Description, ObjectType.None, "钓鱼系统".Lang(Connection.Language), Character?.CharacterName);
                GainItem(fish);
                Connection.ReceiveChat($"PlayerObject.FishingSuccess".Lang(Connection.Language, fishInfo.ItemName), MessageType.System);
            }
            else
            {
                Connection.ReceiveChat($"PlayerObject.FishingFailure".Lang(Connection.Language, fishInfo.ItemName), MessageType.System);
            }
        }

        public void ResetFishing(bool resetFailedTimes)
        {
            IsFishing = false;
            FishingStartTime = DateTime.MaxValue;
            FishingFinishTime = DateTime.MaxValue;
            FishingPerfectTime = DateTime.MaxValue;
            FoundFish = false;
            if (resetFailedTimes)
                FishingFindingFailedTimes = 0;
            FishingSuccess = false;

        }

        public void CheckFishing()
        {
            NextFishingCheckTime += TimeSpan.FromMilliseconds(1000);

            if (IsFishing && SEnvir.Now > FishingFinishTime)
            {
                ResetFishing(FishingFindingFailedTimes < 1);
                Enqueue(new S.FishingEnded());
                Broadcast(new S.ObjectFishing
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Action = MirAction.FishingReel
                });

                if (CanAutoReel)
                {
                    FishingCast();
                }
            }

            if (IsFishing && SEnvir.Now > FishingPerfectTime && CanAutoReel)
            {
                FishingReel(SEnvir.Now);
                FishingCast();
            }
        }

        public void FishingInterrupted()
        {
            if (IsFishing)
            {
                Enqueue(new S.FishingEnded());
                Broadcast(new S.ObjectFishing
                {
                    ObjectID = ObjectID,
                    Direction = Direction,
                    Location = CurrentLocation,
                    Action = MirAction.FishingReel
                });
            }
            PauseBuff(BuffType.FishingMaster);
            ResetFishing(true);
        }

        public void AddFishingMasterBuff(int duration)
        {
            BuffInfo fishingBuff = Buffs.FirstOrDefault(x => x.Type == BuffType.FishingMaster);
            if (fishingBuff == null)
            {
                BuffAdd(BuffType.FishingMaster, TimeSpan.FromSeconds(duration),
                    new Stats { [Stat.ClearRing] = 1, [Stat.Invincibility] = 1 }, true, false, TimeSpan.FromSeconds(1));
            }
            else
            {
                fishingBuff.RemainingTime += TimeSpan.FromSeconds(duration);
                // 告知客户端
                Enqueue(new S.BuffChanged { Index = fishingBuff.Index, Stats = fishingBuff.Stats, RemainingTime = fishingBuff.RemainingTime });
            }
        }
    }
}
