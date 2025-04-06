using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 玩家动作
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 角色当前的坐标方位
        /// </summary>
        public override MirDirection Direction
        {
            get { return Character.Direction; }
            set { Character.Direction = value; }
        }
        /// <summary>
        /// 攻击行为
        /// </summary>
        /// <param name="action"></param>
        public override void ProcessAction(DelayedAction action)
        {
            MapObject ob;
            switch (action.Type)  //攻击类型
            {
                case ActionType.Turn:
                    PacketWaiting = false;
                    Turn((MirDirection)action.Data[0]);
                    return;
                case ActionType.Harvest:
                    PacketWaiting = false;
                    Harvest((MirDirection)action.Data[0]);
                    return;
                case ActionType.Move:
                    PacketWaiting = false;
                    Move((MirDirection)action.Data[0], (int)action.Data[1]);
                    return;
                case ActionType.Magic:
                    PacketWaiting = false;
                    Magic((C.Magic)action.Data[0]);
                    return;
                case ActionType.Mining:
                    PacketWaiting = false;
                    Mining((MirDirection)action.Data[0]);
                    return;
                case ActionType.Attack:
                    PacketWaiting = false;
                    Attack((MirDirection)action.Data[0], (MagicType)action.Data[1]);
                    return;
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0], (List<UserMagic>)action.Data[1], (bool)action.Data[2], (int)action.Data[3]);
                    return;
                case ActionType.AttackDelay:
                    AttackDelay((Point)action.Data[0], (List<UserMagic>)action.Data[1], (bool)action.Data[2], (int)action.Data[3]);
                    return;
                case ActionType.DelayMagic:
                    CompleteMagic(action.Data);
                    return;
                case ActionType.DelayedAttackDamage:
                    ob = (MapObject)action.Data[0];

                    if (!CanAttackTarget(ob)) return;

                    ob.Attacked(this, (int)action.Data[1], (Element)action.Data[2], (bool)action.Data[3], (bool)action.Data[4], (bool)action.Data[5], (bool)action.Data[6]);
                    return;
                case ActionType.DelayedMagicDamage:
                    ob = (MapObject)action.Data[1];

                    if (!CanAttackTarget(ob)) return;

                    MagicAttack((List<UserMagic>)action.Data[0], ob, (bool)action.Data[2], (Stats)action.Data[3], (int)action.Data[4]);
                    return;
                case ActionType.Mount:
                    PacketWaiting = false;
                    Mount();
                    break;
            }
            base.ProcessAction(action);
        }
        /// <summary>
        /// 下马
        /// </summary>
        public void RemoveMount()
        {
            if (Horse == HorseType.None) return;

            Horse = HorseType.None;
            Broadcast(new S.ObjectMount { ObjectID = ObjectID, Horse = Horse, HorseType = Character.Horse });
        }

        #region Packet Actions 数据包操作

        /// <summary>
        /// 旋转方向
        /// </summary>
        /// <param name="direction"></param>
        public void Turn(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Turn, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (direction != Direction)
                TradeClose();

            Direction = direction;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsTurnTime);

            if (PoisonList.Any(x => x.Type == PoisonType.Neutralize))
                ActionTime += TimeSpan.FromMilliseconds(Config.GlobalsTurnTime);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow });
        }
        /// <summary>
        /// 收割 割肉 方向
        /// </summary>
        /// <param name="direction"></param>
        public void Harvest(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Harvest, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove || Horse != HorseType.None)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            Direction = direction;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsHarvestTime);

            if (PoisonList.Any(x => x.Type == PoisonType.Neutralize))
                ActionTime += TimeSpan.FromMilliseconds(Config.GlobalsTurnTime);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectHarvest { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow });

            Point front = Functions.Move(CurrentLocation, Direction, 1);
            int range = Stats[Stat.PickUpRadius];
            bool send = false;
            bool success = false;

            for (int d = 0; d <= range; d++)
            {
                for (int y = front.Y - d; y <= front.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = front.X - d; x <= front.X + d; x += Math.Abs(y - front.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y];

                        if (cell?.Objects == null) continue;

                        foreach (MapObject cellObject in cell.Objects)
                        {
                            if (cellObject.Race != ObjectType.Monster) continue;

                            MonsterObject ob = (MonsterObject)cellObject;

                            if (ob.Drops == null) continue;

                            List<UserItem> items;

                            if (!ob.Drops.TryGetValue(Character.Account, out items))
                            {
                                send = true;
                                continue;
                            }

                            if (ob.HarvestCount > 0)
                            {
                                ob.HarvestCount--;
                                continue;
                            }

                            if (items != null)
                            {
                                success = true;
                                for (int i = items.Count - 1; i >= 0; i--)
                                {
                                    UserItem item = items[i];
                                    if (item.UserTask == null) continue;

                                    if (item.UserTask.Quest.Character == Character && !item.UserTask.Completed) continue;

                                    items.Remove(item);
                                    item.Delete();
                                }

                                if (items.Count == 0) items = null;
                            }

                            if (items == null)
                            {
                                ob.Drops.Remove(Character.Account);

                                if (ob.Drops.Count == 0) ob.Drops = null;
                                ob.HarvestChanged();
                                Connection.ReceiveChat("Monster.HarvestNothing".Lang(Connection.Language), MessageType.System);

                                foreach (SConnection con in Connection.Observers)
                                    con.ReceiveChat("Monster.HarvestNothing".Lang(con.Language), MessageType.System);
                                continue;
                            }

                            for (int i = items.Count - 1; i >= 0; i--)
                            {
                                UserItem item = items[i];

                                ItemCheck check = new ItemCheck(item, item.Count, item.Flags, item.ExpireTime);

                                if (!CanGainItems(false, check)) continue;

                                GainItem(item);
                                items.Remove(item);
                            }

                            if (items.Count == 0)
                            {
                                ob.Drops.Remove(Character.Account);

                                if (ob.Drops.Count == 0) ob.Drops = null;
                                ob.HarvestChanged();

                                continue;
                            }

                            Connection.ReceiveChat("Monster.HarvestCarry".Lang(Connection.Language), MessageType.System);

                            foreach (SConnection con in Connection.Observers)
                                con.ReceiveChat("Monster.HarvestCarry".Lang(con.Language), MessageType.System);
                            continue;
                        }
                    }
                }
            }

            if (send)
            {
                Connection.ReceiveChat("Monster.HarvestOwner".Lang(Connection.Language), MessageType.System);


                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Monster.HarvestOwner".Lang(con.Language), MessageType.System);
            }

            #region 人物挖肉事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerHarvest(EventTypes.PlayerHarvest,
                    new PlayerHarvestEventArgs { success = success }));
            #endregion

        }

        /// <summary>
        /// 骑马
        /// </summary>
        public void Mount()
        {
            if (SEnvir.Now < ActionTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Mount));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (Dead)  //死亡
            {
                Connection.ReceiveChat("Skills.HorseDead".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.HorseDead".Lang(con.Language), MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            if (Character.Horse == HorseType.None)
            {
                Connection.ReceiveChat("Skills.HorseOwner".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.HorseOwner".Lang(con.Language), MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            if (!CurrentMap.Info.CanHorse)
            {
                Connection.ReceiveChat("Skills.HorseMap".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.HorseMap".Lang(con.Language), MessageType.System);

                Enqueue(new S.MountFailed { Horse = Horse });
                return;
            }

            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsTurnTime);

            if (Horse == HorseType.None)
                Horse = Character.Horse;
            else
                Horse = HorseType.None;

            Broadcast(new S.ObjectMount { ObjectID = ObjectID, Horse = Horse, HorseType = Character.Horse });
        }
        /// <summary>
        /// 移动方向
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        public void Move(MirDirection direction, int distance)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < MoveTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Move, direction, distance));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanMove)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (distance <= 0 || distance > 3)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (distance == 3 && Horse == HorseType.None)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            Cell cell = null;

            for (int i = 1; i <= distance; i++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction, i));
                if (cell == null)
                {
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
                }

                if (cell.IsBlocking(this, false))   //玩家穿人
                {
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
                }
            }

            BuffRemove(BuffType.Invisibility);
            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.SuperTransparency);
            //移动则暂停钓鱼和姜太公buff
            FishingInterrupted();

            if (distance > 1)
            {
                if (Stats[Stat.Comfort] < 120)
                    RegenTime = SEnvir.Now + RegenDelay;
                BuffRemove(BuffType.Cloak);
            }

            Direction = direction;

            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsMoveTime);
            MoveTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsMoveTime);

            //如果移动距离等2  并且 开启跑动扣血
            if (distance == 2)
            {
                //跑动时间
                RuningTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsMoveTime);
                //移动距离
                RuningSetps += distance;
                //如果移动距离到了20 并且 跑动时间等于移动时间(说明持续再跑)
                if (RuningSetps == 20 && RuningTime == MoveTime)
                {
                    if (Dead) return;  //如果死亡跳过

                    if (CurrentHP > 1)  //如果角色的血量大于1
                    {
                        CurrentHP -= 1;           //减血1点
                        RuningSetps = 0;          //复位       
                    }
                }
            }

            PreventSpellCheck = true;
            CurrentCell = cell.GetMovement(this);
            PreventSpellCheck = false;

            RemoveAllObjects();
            AddAllObjects();

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            Broadcast(new S.ObjectMove { ObjectID = ObjectID, Direction = direction, Location = CurrentLocation, Slow = slow, Distance = distance });
            CheckSpellObjects();

            #region 人物移动事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerMove(EventTypes.PlayerMove,
                new PlayerMoveEventArgs { numSteps = distance, direction = direction }));
            #endregion
        }

        /// <summary>
        /// 普通攻击
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="attackMagic"></param>
        public void Attack(MirDirection direction, MagicType attackMagic)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < AttackTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Attack, direction, attackMagic));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanAttack)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }
            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];

            if (weapon != null && weapon.IsRefine)
            {
                Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction, 1));

                if (cell != null)
                {
                    bool flag = false;
                    if (cell.Objects != null)
                    {
                        foreach (MapObject ob in cell.Objects)
                        {
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                case ObjectType.NPC:
                                    flag = true;
                                    break;
                            }
                            if (flag) break;
                        }
                    }
                    if (flag)
                    {
                        weapon.IsRefine = false;
                        S.ItemStatsChanged mainResult = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                        if (SEnvir.Random.Next(100) < weapon.Chance)
                        {

                            switch (weapon.RefineType)
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

                                    weapon.MergeRefineElements(out Stat element1);

                                    for (int i = weapon.AddedStats.Count - 1; i >= 0; i--)
                                    {
                                        UserItemStat stat = weapon.AddedStats[i];
                                        if (stat.Stat == Stat.WeaponElement) continue;

                                        switch (stat.Stat)           //武器重置增加的属性删除，其他属性保留
                                        {
                                            case Stat.MaxDC:
                                            case Stat.MaxMC:
                                            case Stat.MaxSC:
                                            case Stat.FireAttack:
                                            case Stat.LightningAttack:
                                            case Stat.IceAttack:
                                            case Stat.WindAttack:
                                            case Stat.DarkAttack:
                                            case Stat.HolyAttack:
                                            case Stat.PhantomAttack:
                                            case Stat.EvasionChance:
                                            case Stat.BlockChance:
                                                break;
                                            default:
                                                continue;
                                        }

                                        int value = 0;

                                        switch (weapon.Info.Rarity)  //按武器的类型走
                                        {
                                            case Rarity.Common:
                                                if (SEnvir.Random.Next(Config.CommonResetProbability1) == 0)
                                                {
                                                    value = 1;

                                                    if (SEnvir.Random.Next(Config.CommonResetProbability2) == 0)
                                                        value += 1;

                                                    if (SEnvir.Random.Next(Config.CommonResetProbability3) == 0)
                                                        value += 1;
                                                }
                                                break;
                                            case Rarity.Superior:
                                                if (SEnvir.Random.Next(Config.SuperiorResetProbability1) == 0)
                                                {
                                                    value = 1;

                                                    if (SEnvir.Random.Next(Config.SuperiorResetProbability2) == 0)
                                                        value += 1;

                                                    if (SEnvir.Random.Next(Config.SuperiorResetProbability3) == 0)
                                                        value += 1;
                                                }
                                                break;
                                            case Rarity.Elite:
                                                if (SEnvir.Random.Next(Config.EliteResetProbability1) == 0)
                                                {
                                                    value = 1;

                                                    if (SEnvir.Random.Next(Config.EliteResetProbability2) == 0)
                                                        value += 1;

                                                    if (SEnvir.Random.Next(Config.EliteResetProbability3) == 0)
                                                        value += 1;
                                                }
                                                break;
                                        }

                                        stat.Delete();
                                        weapon.AddStat(stat.Stat, value, StatSource.Enhancement);
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
                        S.ItemRefineChange result = new S.ItemRefineChange { ErrorID = 0, Refine = false, Item = weapon.ToClientInfo() };

                        Enqueue(result);

                        Broadcast(new S.ObjectWeaponLeveled { ObjectID = ObjectID });
                        RefreshStats();
                    }
                }
            }

            CombatTime = SEnvir.Now;

            //舒适小于150 攻击中断回血
            if (Stats[Stat.Comfort] < 150)
                RegenTime = SEnvir.Now + RegenDelay;
            Direction = direction;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsAttackTime);

            int aspeed = Stats[Stat.AttackSpeed];
            if (SEnvir.Now > SpeedDelay)//判断是否是触发时间
            {
                //非触发时间
                if (SEnvir.Random.Next(100) < Stats[Stat.AttackSpeedAdd])
                {
                    SpeedDelay = SEnvir.Now.AddSeconds(10);
                    aspeed = aspeed * 200 / 100;
                }
            }
            else//触发时间
            {
                aspeed = aspeed * 200 / 100;
            }
            int attackDelay = (int)(Config.GlobalsAttackDelay - aspeed / 10.0 * Config.GlobalsASpeedRate);
            attackDelay = Math.Max(100, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || PoisonList.Any(x => x.Type == PoisonType.Neutralize))
                AttackTime += TimeSpan.FromMilliseconds(attackDelay);

            MagicType validMagic = MagicType.None;
            List<UserMagic> magics = new List<UserMagic>();

            UserMagic magic;

            #region Warrior 战士  

            if (Magics.TryGetValue(MagicType.Swordsmanship, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);

            if (Magics.TryGetValue(MagicType.Slaying, out magic) && Level >= magic.Info.NeedLevel1)
            {
                if (CanPowerAttack && attackMagic == MagicType.Slaying)
                {
                    magics.Add(magic);
                    validMagic = MagicType.Slaying;
                    Enqueue(new S.MagicToggle { Magic = MagicType.Slaying, CanUse = CanPowerAttack = false });
                }

                if (!CanPowerAttack && SEnvir.Random.Next(5) == 0)
                    Enqueue(new S.MagicToggle { Magic = MagicType.Slaying, CanUse = CanPowerAttack = true });
            }

            if (attackMagic == MagicType.Thrusting && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = MagicType.Thrusting;
                    magics.Add(magic);
                    ChangeMP(-cost);
                }
            }

            if (attackMagic == MagicType.HalfMoon && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = MagicType.HalfMoon;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (attackMagic == MagicType.DestructiveSurge && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    DestructiveSurgeLifeSteal = 0;
                    validMagic = MagicType.DestructiveSurge;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (CanFlamingSword && attackMagic == MagicType.FlamingSword && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)  //烈火剑法
            {
                validMagic = MagicType.FlamingSword;
                magics.Add(magic);
                CanFlamingSword = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.FlamingSword, CanUse = false });
            }
            //大补帖 自动烈火
            else if (setConfArr[(int)AutoSetConf.SetFlamingSwordBox] && Magics.TryGetValue(MagicType.FlamingSword, out magic) && Level >= magic.Info.NeedLevel1) //烈火剑法
            {
                MagicToggle(new C.MagicToggle { Magic = MagicType.FlamingSword, CanUse = true });
            }

            if (CanDragonRise && attackMagic == MagicType.DragonRise && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)   //翔空剑法
            {
                validMagic = MagicType.DragonRise;
                magics.Add(magic);
                CanDragonRise = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.DragonRise, CanUse = false });
            }
            //大补帖 自动翔空剑法
            else if (setConfArr[(int)AutoSetConf.SetDragobRiseBox] && Magics.TryGetValue(MagicType.DragonRise, out magic) && Level >= magic.Info.NeedLevel1)  //翔空剑法
            {
                MagicToggle(new C.MagicToggle { Magic = MagicType.DragonRise, CanUse = true });
            }

            if (CanBladeStorm && attackMagic == MagicType.BladeStorm && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1) //莲月剑法
            {
                validMagic = MagicType.BladeStorm;
                magics.Add(magic);
                CanBladeStorm = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.BladeStorm, CanUse = false });
            }
            //大补帖 自动莲月剑法
            else if (setConfArr[(int)AutoSetConf.SetBladeStormBox] && Magics.TryGetValue(MagicType.BladeStorm, out magic) && Level >= magic.Info.NeedLevel1)  //莲月剑法
            {
                MagicToggle(new C.MagicToggle { Magic = MagicType.BladeStorm, CanUse = true });
            }

            if (CanMaelstromBlade && attackMagic == MagicType.MaelstromBlade && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1) //屠龙斩
            {
                validMagic = MagicType.MaelstromBlade;
                magics.Add(magic);
                CanMaelstromBlade = false;
                Enqueue(new S.MagicToggle { Magic = MagicType.MaelstromBlade, CanUse = false });
            }
            //大补帖 自动屠龙斩
            else if (setConfArr[(int)AutoSetConf.SetMaelstromBlade] && Magics.TryGetValue(MagicType.MaelstromBlade, out magic) && Level >= magic.Info.NeedLevel1)  //屠龙斩
            {
                MagicToggle(new C.MagicToggle { Magic = MagicType.MaelstromBlade, CanUse = true });
            }

            #endregion

            #region Taoist 道士

            if (Magics.TryGetValue(MagicType.SpiritSword, out magic) && Level >= magic.Info.NeedLevel1)
            {
                magics.Add(magic);
                LevelMagic(magic);
            }

            #endregion

            #region Assassin 刺客

            if (Magics.TryGetValue(MagicType.VineTreeDance, out magic) && Level >= magic.Info.NeedLevel1)
            {
                LevelMagic(magic);
                magics.Add(magic);
            }

            if (Magics.TryGetValue(MagicType.Discipline, out magic) && Level >= magic.Info.NeedLevel1)
            {
                LevelMagic(magic);
                magics.Add(magic);
            }

            if (Magics.TryGetValue(MagicType.BloodyFlower, out magic) && Level >= magic.Info.NeedLevel1)
            {
                LevelMagic(magic);
                magics.Add(magic);
            }

            if (Magics.TryGetValue(MagicType.AdvancedBloodyFlower, out magic) && Level >= magic.Info.NeedLevel1)
            {
                LevelMagic(magic);
                magics.Add(magic);
            }

            if (SEnvir.Random.Next(2) == 0 && Magics.TryGetValue(MagicType.CalamityOfFullMoon, out magic) && Level >= magic.Info.NeedLevel1)
            {
                LevelMagic(magic);
                magics.Add(magic);
            }

            if (attackMagic == MagicType.FullBloom && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (attackMagic == MagicType.WhiteLotus && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (attackMagic == MagicType.RedLotus && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (attackMagic == MagicType.SweetBrier && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            if (setConfArr[(int)AutoSetConf.SetFourFlowersBox])
            {

            }

            if (attackMagic == MagicType.Karma && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Now >= magic.Cooldown && Buffs.Any(x => x.Type == BuffType.Cloak))
            {
                int cost = Stats[Stat.Health] * magic.Cost / 100;

                UserMagic augMagic;
                if (Magics.TryGetValue(MagicType.Release, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                {
                    cost -= cost * augMagic.GetPower() / 100;
                    magics.Add(augMagic);
                    LevelMagic(magic);
                }

                if (cost < CurrentHP)
                {
                    validMagic = attackMagic;
                    magics.Add(magic);
                    ChangeHP(-cost);
                }
            }

            if (validMagic == MagicType.None && SEnvir.Random.Next(2) == 0 && Magics.TryGetValue(MagicType.WaningMoon, out magic) && Level >= magic.Info.NeedLevel1)
                magics.Add(magic);
            if (Magics.TryGetValue(MagicType.WaningMoon, out magic))
            {
                LevelMagic(magic);
            }

            if (attackMagic == MagicType.FlameSplash && Magics.TryGetValue(attackMagic, out magic) && Level >= magic.Info.NeedLevel1)
            {
                int cost = magic.Cost;

                if (cost <= CurrentMP)
                {
                    FlameSplashLifeSteal = 0;
                    validMagic = MagicType.FlameSplash;
                    magics.Add(magic);
                    ChangeMP(-cost);
                    LevelMagic(magic);
                }
            }

            #endregion

            Element element = Functions.GetElement(Stats);

            if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
            {
                foreach (KeyValuePair<Stat, int> stats in Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                {
                    switch (stats.Key)
                    {
                        case Stat.FireAffinity:
                            element = Element.Fire;
                            break;
                        case Stat.IceAffinity:
                            element = Element.Ice;
                            break;
                        case Stat.LightningAffinity:
                            element = Element.Lightning;
                            break;
                        case Stat.WindAffinity:
                            element = Element.Wind;
                            break;
                        case Stat.HolyAffinity:
                            element = Element.Holy;
                            break;
                        case Stat.DarkAffinity:
                            element = Element.Dark;
                            break;
                        case Stat.PhantomAffinity:
                            element = Element.Phantom;
                            break;
                    }
                }
            }

            //如果（攻击位置(功能.移动（当前位置 方向） 魔法 真实））
            if (AttackLocation(Functions.Move(CurrentLocation, Direction), magics, true))
            {
                switch (attackMagic)
                {
                    case MagicType.FullBloom:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.WhiteLotus:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.RedLotus:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        break;
                    case MagicType.SweetBrier:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        if (Magics.TryGetValue(MagicType.FullBloom, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }

                        if (Magics.TryGetValue(MagicType.WhiteLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.RedLotus, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(attackDelay + attackDelay / 2);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = attackDelay + attackDelay / 2 });
                        }
                        if (Magics.TryGetValue(MagicType.SweetBrier, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        break;
                    case MagicType.Karma:
                        Enqueue(new S.MagicToggle { Magic = attackMagic, CanUse = false });

                        UseItemTime = SEnvir.Now.AddSeconds(10);

                        if (Magics.TryGetValue(MagicType.Karma, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }

                        if (Magics.TryGetValue(MagicType.SummonPuppet, out magic))
                        {
                            magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                        }
                        break;
                    default:

                        break;
                }
            }

            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);
            BuffRemove(BuffType.SuperTransparency);

            //攻击则暂停钓鱼和姜太公buff
            FishingInterrupted();

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow, AttackMagic = validMagic, AttackElement = element });

            switch (validMagic)
            {
                case MagicType.Thrusting:
                    AttackLocation(Functions.Move(CurrentLocation, Direction, 2), magics, false);
                    break;
                case MagicType.HalfMoon:
                case MagicType.DragonRise: //半月 翔空 攻击面前3个方向
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, -1)), magics, false);
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 1)), magics, false);
                    AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 2)), magics, false);
                    break;
                case MagicType.DestructiveSurge:
                    for (int i = 1; i < 8; i++)
                        AttackLocation(Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i)), magics, false);
                    break;
                case MagicType.FlameSplash:
                    int count = 0;
                    List<MirDirection> directions = new List<MirDirection>();

                    for (int i = 0; i < 8; i++)
                        directions.Add((MirDirection)i);

                    directions.Remove(Direction);

                    while (count < 4)
                    {
                        MirDirection dir = directions[SEnvir.Random.Next(directions.Count)];

                        if (AttackLocation(Functions.Move(CurrentLocation, dir), magics, false))
                            count++;

                        directions.Remove(dir);
                        if (directions.Count == 0) break;
                    }
                    break;
            }

            CurrentMagic = validMagic;
        }
        /// <summary>
        /// 设置魔法技能攻击
        /// </summary>
        /// <param name="p"></param>
        public void Magic(C.Magic p)
        {
            UserMagic magic;

            if (!Magics.TryGetValue(p.Type, out magic) || Level < magic.Info.NeedLevel1)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }
            CurrentMagic = magic.Info.Magic;

            if (SEnvir.Now < ActionTime || SEnvir.Now < MagicTime || SEnvir.Now < magic.Cooldown)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Magic, p));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });

                return;
            }

            if (!CanCast)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            switch (p.Type) //魔法技能类型
            {
                case MagicType.ShoulderDash:
                case MagicType.Interchange:
                case MagicType.Defiance:
                case MagicType.Beckon:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Fetter:
                case MagicType.SwiftBlade:
                case MagicType.Endurance:
                case MagicType.Assault:
                case MagicType.SeismicSlam:
                case MagicType.ReigningStep:
                case MagicType.CrushingWave:
                case MagicType.Invincibility:

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.AdamantineFireBall:
                case MagicType.ThunderBolt:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.FrozenEarth:
                case MagicType.BlowEarth:
                case MagicType.FireWall:
                case MagicType.FireStorm:
                case MagicType.LightningWave:
                case MagicType.ExpelUndead:
                case MagicType.GeoManipulation:
                case MagicType.Transparency:
                case MagicType.MagicShield:
                case MagicType.FrostBite:
                case MagicType.IceStorm:
                case MagicType.IceRain:
                case MagicType.DragonTornado:
                case MagicType.GreaterFrozenEarth:
                case MagicType.ChainLightning:
                case MagicType.MeteorShower:
                case MagicType.Renounce:
                case MagicType.Tempest:
                case MagicType.JudgementOfHeaven:
                case MagicType.ThunderStrike:
                case MagicType.Teleportation:
                case MagicType.Asteroid:
                case MagicType.SuperiorMagicShield:

                case MagicType.Heal:
                case MagicType.PoisonDust:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.GreaterHolyStrike:
                case MagicType.MagicResistance:
                case MagicType.Resilience:
                //case MagicType.ShacklingTalisman:
                case MagicType.Invisibility:
                case MagicType.MassInvisibility:
                case MagicType.MassTransparency:
                case MagicType.ThunderKick:
                case MagicType.StrengthOfFaith:
                case MagicType.CelestialLight:
                case MagicType.GreaterPoisonDust:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.Scarecrow:
                case MagicType.LifeSteal:
                case MagicType.ImprovedExplosiveTalisman:


                case MagicType.TrapOctagon:
                case MagicType.TaoistCombatKick:
                case MagicType.ElementalSuperiority:
                case MagicType.MassHeal:
                case MagicType.BloodLust:
                case MagicType.Resurrection:
                case MagicType.Purification:
                case MagicType.MirrorImage:
                case MagicType.SummonSkeleton:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonShinsu:
                //case MagicType.Transparency:

                case MagicType.PoisonousCloud:
                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.TheNewBeginning:
                case MagicType.SummonPuppet:
                case MagicType.Abyss:
                case MagicType.FlashOfLight:
                case MagicType.DanceOfSwallow:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.MassBeckon:
                case MagicType.Infection:
                case MagicType.Neutralize:
                case MagicType.DarkSoulPrison:
                case MagicType.SwordOfVengeance:
                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.ThousandBlades:
                    if (Buffs.Any(x => x.Type == BuffType.SuperTransparency)) break;

                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.DarkConversion:
                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion)) break;

                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.DragonRepulse:
                    if (Stats[Stat.Health] * magic.Cost / 1000 >= CurrentHP || CurrentHP < Stats[Stat.Health] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    if (Stats[Stat.Mana] * magic.Cost / 1000 >= CurrentMP || CurrentMP < Stats[Stat.Mana] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.Cloak:
                    if (Buffs.Any(x => x.Type == BuffType.Cloak)) break;

                    if (SEnvir.Now < CombatTime.AddSeconds(10))
                    {
                        Connection.ReceiveChat("Skills.CloakCombat".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.CloakCombat".Lang(con.Language), MessageType.System);
                        break;
                    }

                    if (Stats[Stat.Health] * magic.Cost / 1000 >= CurrentHP || CurrentHP < Stats[Stat.Health] / 10)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.ElementalHurricane:
                    int cost = magic.Cost;
                    if (Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
                        cost = 0;

                    if (cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                case MagicType.Concentration:
                    if (Buffs.Any(x => x.Type == BuffType.Concentration)) break;

                    if (magic.Cost > CurrentMP)
                    {
                        Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                        return;
                    }
                    break;
                default:
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                    return;
            }

            //完成成本
            //战斗时间
            MapObject ob = VisibleObjects.FirstOrDefault(x => x.ObjectID == p.Target);

            if (ob != null && !Functions.InRange(CurrentLocation, ob.CurrentLocation, Globals.MagicRange))
                ob = null;

            bool cast = true;
            bool flag = false;

            List<uint> targets = new List<uint>();
            List<Point> locations = new List<Point>();

            List<Cell> cells;
            Stats stats;
            int power;
            UserMagic augMagic;
            HashSet<MapObject> realTargets;
            List<UserMagic> magics;
            int _shape;

            int count;
            List<MapObject> possibleTargets;
            Point location;
            BuffInfo buff;
            switch (p.Type)
            {
                #region Warrior 战士

                case MagicType.ShoulderDash:
                    if ((Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) break;  //如果是石化状态 跳过

                    Direction = p.Direction;          //方向
                    count = ShoulderDashEnd(magic);   //计数=野蛮冲撞结束

                    if (count == 0)  //如果计数等0 出提示
                    {
                        Connection.ReceiveChat("Skills.DashFailed".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.DashFailed".Lang(con.Language), MessageType.System);
                    }
                    //发包 技能冷却
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    ChangeMP(-magic.Cost);
                    return;
                case MagicType.ReigningStep:
                    if ((Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip) break;

                    Direction = p.Direction;
                    count = ReigningStepEnd(magic);

                    if (count == 0)
                    {
                        Connection.ReceiveChat("Skills.DashFailed".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.DashFailed".Lang(con.Language), MessageType.System);
                    }

                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    ChangeMP(-magic.Cost);
                    return;
                case MagicType.Interchange:
                case MagicType.Beckon:
                    if (ob == null) break;

                    if (!CanAttackTarget(ob))
                    {
                        ob = null;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(300),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Defiance:
                    if (Buffs.Any(x => x.Type == BuffType.Might)) return;
                    ob = null;
                    p.Direction = MirDirection.Down;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Might:
                    if (Buffs.Any(x => x.Type == BuffType.Defiance)) return;
                    ob = null;
                    p.Direction = MirDirection.Down;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.ReflectDamage:
                case MagicType.Endurance:
                case MagicType.Invincibility:
                    ob = null;
                    p.Direction = MirDirection.Down;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Fetter:
                    ob = null;
                    p.Direction = MirDirection.Down;

                    cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.SwiftBlade:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);

                    cells = CurrentMap.GetCells(p.Location, 0, 3);
                    SwiftBladeLifeSteal = 0;

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(900),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.SeismicSlam:
                    ob = null;

                    cells = CurrentMap.GetCells(Functions.Move(CurrentLocation, p.Direction, 3), 0, 3);
                    SwiftBladeLifeSteal = 0;

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(600),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.ThousandBlades:
                    if (ob == null) break;
                    if (Buffs.Any(x => x.Type == BuffType.SuperTransparency)) break;
                    if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, 1)) break;

                    if (!CanAttackTarget(ob))
                    {
                        ob = null;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(300),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.CrushingWave:
                    ob = null;

                    for (int i = 1; i <= 12; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;
                        locations.Add(cell.Location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(400 + i * 60),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));

                        switch (p.Direction)
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(200 + i * 60),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(200 + i * 60),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2))),
                                    false));
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(200 + i * 60),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1))),
                                    false));
                                ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(200 + i * 60),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1))),
                                    false));
                                break;
                        }
                    }
                    break;
                case MagicType.MassBeckon:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;

                #endregion

                #region Wizard 法师

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.AdamantineFireBall:
                case MagicType.IceBlades:
                    if (!CanFlyTarget(ob) || !CanAttackTarget(ob))                //如果可攻击目标为空 或者 中间有阻碍
                    {
                        locations.Add(p.Location);           //坐标增加
                        ob = null;                           //对象等空
                        break;                               //跳出
                    }

                    targets.Add(ob.ObjectID);                //攻击对象增加
                    ActionList.Add(new DelayedAction(        //延迟执行动作
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 48),  //延迟时间加上当前坐标距离
                        ActionType.DelayMagic,               //动作类型延迟施法
                        new List<UserMagic> { magic },       //新的技能列表
                        ob));                                //对象
                    break;
                case MagicType.ThunderBolt:
                case MagicType.Cyclone:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }
                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.ElectricShock:
                    if (!CanAttackTarget(ob) || ob.Race != ObjectType.Monster)  //可以攻击目标为空 不是怪物
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }
                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.ExpelUndead:
                    if (!CanAttackTarget(ob) || ob.Race != ObjectType.Monster || !((MonsterObject)ob).MonsterInfo.Undead)  //可以攻击目标为空 不是怪物 不是生系
                    {
                        ob = null;
                        break;
                    }
                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Repulsion:
                    ob = null;

                    for (MirDirection d = MirDirection.Up; d <= MirDirection.UpLeft; d++) //方向朝向
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            CurrentMap.GetCell(Functions.Move(CurrentLocation, d)), //将目标按不同的反向移动
                            d));
                    }
                    break;
                case MagicType.Teleportation:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.ScortchedEarth:
                case MagicType.FrozenEarth:
                    ob = null;

                    for (int i = 1; i <= 8; i++)   //设置8格
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i); //坐标朝向
                        Cell cell = CurrentMap.GetCell(location);   //对应坐标单元格

                        if (cell == null) continue;

                        if (!flag && cell?.Objects != null)
                            for (int j = cell.Objects.Count - 1; j >= 0; j--)
                            {
                                if (j >= cell.Objects.Count) continue;
                                MapObject obtem = cell.Objects[j];
                                if (!CanAttackTarget(obtem)) continue;
                                flag = true; break;
                            }

                        locations.Add(cell.Location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(800),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));

                        switch (p.Direction)  //对应八个方向
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                    SEnvir.Now.AddMilliseconds(800),
                                    ActionType.DelayMagic,
                                    new List<UserMagic> { magic },
                                    celltem,
                                    false));

                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                }
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));

                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                    break;
                                }
                        }
                    }
                    if (flag)
                        LevelMagic(magic);
                    break;
                case MagicType.LightningBeam:
                    ob = null;

                    locations.Add(Functions.Move(CurrentLocation, p.Direction));   //增加坐标方向

                    for (int i = 1; i <= 8; i++)   //设置8格距离
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (!flag && cell?.Objects != null)
                            for (int j = cell.Objects.Count - 1; j >= 0; j--)
                            {
                                if (j >= cell.Objects.Count) continue;
                                MapObject obtem = cell.Objects[j];
                                if (!CanAttackTarget(obtem)) continue;
                                flag = true; break;
                            }

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));

                        switch (p.Direction)    //八个方向
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));

                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                }
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                }
                                break;
                        }
                    }
                    if (flag)
                        LevelMagic(magic);
                    break;
                case MagicType.BlowEarth:
                    ob = null;

                    Point lastLocation = CurrentLocation;   //坐标等角色当前坐标

                    for (int i = 1; i <= 8; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);   //当前朝向
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;

                        if (!flag && cell?.Objects != null)
                            for (int j = cell.Objects.Count - 1; j >= 0; j--)
                            {
                                if (j >= cell.Objects.Count) continue;
                                MapObject obtem = cell.Objects[j];
                                if (!CanAttackTarget(obtem)) continue;
                                flag = true; break;
                            }

                        lastLocation = location;

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(800),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            true));

                        switch (p.Direction)   //八个方向
                        {
                            case MirDirection.Up:
                            case MirDirection.Right:
                            case MirDirection.Down:
                            case MirDirection.Left:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 2)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                }
                                break;
                            case MirDirection.UpRight:
                            case MirDirection.DownRight:
                            case MirDirection.DownLeft:
                            case MirDirection.UpLeft:
                                {
                                    Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, 1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                    celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(p.Direction, -1)));
                                    if (!flag && celltem != null && celltem?.Objects != null)
                                        for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                        {
                                            if (j >= celltem.Objects.Count) continue;
                                            MapObject obtem = celltem.Objects[j];
                                            if (!CanAttackTarget(obtem)) continue;
                                            flag = true; break;
                                        }
                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(800),
                                        ActionType.DelayMagic,
                                        new List<UserMagic> { magic },
                                        celltem,
                                        false));
                                }
                                break;
                        }
                    }

                    locations.Add(lastLocation);

                    if (lastLocation == CurrentLocation)
                        cast = false;

                    if (flag)
                        LevelMagic(magic);
                    break;
                case MagicType.FireWall:
                    ob = null;

                    //if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))  //如果不是指定范围
                    //{
                    //    cast = false;
                    //    break;
                    //}

                    foreach (ConquestWar war in SEnvir.ConquestWars)   //攻城对象
                    {
                        if (war.Map != CurrentMap) continue;   //不是攻城地图

                        for (int i = SpellList.Count - 1; i >= 0; i--)
                        {
                            if (SpellList[i].Effect != SpellEffect.FireWall) continue;

                            SpellList[i].Despawn();
                        }
                        break;
                    }
                    //power = Math.Min(8, (magic.Level * 2) + 3);   //伤害次数

                    power = Math.Min(12, (magic.Level * 2) + 1 + this.Stats[Stat.MaxMC] / 20);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Up)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Down)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(p.Location),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Left)),
                        power));

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(p.Location, MirDirection.Right)),
                        power));
                    LevelMagic(magic);
                    break;
                case MagicType.FireStorm://爆裂火焰 
                case MagicType.LightningWave://地狱雷光
                case MagicType.IceStorm://冰咆哮               
                case MagicType.DragonTornado://龙卷风
                    //ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))  //不是指定范围
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    //cells = CurrentMap.GetCells(p.Location, 0, 1);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob, p.Location));

                    //foreach (Cell cell in cells)
                    //{
                    //	ActionList.Add(new DelayedAction(
                    //		SEnvir.Now.AddMilliseconds(500),
                    //		ActionType.DelayMagic,
                    //		new List<UserMagic> { magic },
                    //		cell));
                    //}
                    LevelMagic(magic);
                    break;
                case MagicType.IceRain:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    power = (magic.Level + 1) * 2;

                    foreach (ConquestWar war in SEnvir.ConquestWars)
                    {
                        if (war.Map != CurrentMap) continue;

                        for (int i = SpellList.Count - 1; i >= 0; i--)
                        {
                            if (SpellList[i].Effect != SpellEffect.IceRain) continue;

                            SpellList[i].Despawn();
                        }
                        break;
                    }

                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            power));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.Asteroid:  //天之怒火
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))   //不是指定范围
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 4);   //当前地图4格范围

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(1000),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }

                    if (Magics.TryGetValue(MagicType.FireWall, out augMagic) && augMagic.Info.NeedLevel1 > Level)   //增加火墙
                        augMagic = null;

                    if (augMagic != null)   //如果火墙不为空
                    {
                        foreach (ConquestWar war in SEnvir.ConquestWars)   //攻城时处理
                        {
                            if (war.Map != CurrentMap) continue;

                            for (int i = SpellList.Count - 1; i >= 0; i--)
                            {
                                if (SpellList[i].Effect != SpellEffect.FireWall) continue;

                                SpellList[i].Despawn();
                            }
                            break;
                        }

                        power = (magic.Level + 2) * 5;   //伤害次数

                        foreach (Cell cell in cells)    //对应范围对象伤害
                        {
                            if (Math.Abs(cell.Location.X - p.Location.X) + Math.Abs(cell.Location.Y - p.Location.Y) >= 3) continue;

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(1550),
                                ActionType.DelayMagic,
                                new List<UserMagic> { augMagic },
                                cell,
                                power));
                        }
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.MagicShield:
                case MagicType.SuperiorMagicShield:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1100),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Renounce:
                case MagicType.JudgementOfHeaven:
                case MagicType.FrostBite:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.GeoManipulation:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))   //不是指定范围
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location));                          //对应坐标
                    break;
                case MagicType.GreaterFrozenEarth:
                    ob = null;

                    for (int d = -1; d <= 1; d++)
                        for (int i = 1; i <= 8; i++)         //8格范围
                        {
                            MirDirection direction = Functions.ShiftDirection(p.Direction, d);   //朝向

                            location = Functions.Move(CurrentLocation, direction, i);
                            Cell cell = CurrentMap.GetCell(location);

                            if (cell == null) continue;

                            if (!flag && cell?.Objects != null)
                                for (int j = cell.Objects.Count - 1; j >= 0; j--)
                                {
                                    if (j >= cell.Objects.Count) continue;
                                    MapObject obtem = cell.Objects[j];
                                    if (!CanAttackTarget(obtem)) continue;
                                    flag = true; break;
                                }

                            locations.Add(cell.Location);

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(800),
                                ActionType.DelayMagic,
                                new List<UserMagic> { magic },
                                cell,
                                true));

                            switch (direction)           //斜刺三个方向
                            {
                                case MirDirection.Up:
                                case MirDirection.Right:
                                case MirDirection.Down:
                                case MirDirection.Left:
                                    {
                                        Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -2)));
                                        if (!flag && celltem != null && celltem?.Objects != null)
                                            for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                            {
                                                if (j >= celltem.Objects.Count) continue;
                                                MapObject obtem = celltem.Objects[j];
                                                if (!CanAttackTarget(obtem)) continue;
                                                flag = true; break;
                                            }
                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(800),
                                            ActionType.DelayMagic,
                                            new List<UserMagic> { magic },
                                            celltem,
                                            false));

                                        celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 2)));
                                        if (!flag && celltem != null && celltem?.Objects != null)
                                            for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                            {
                                                if (j >= celltem.Objects.Count) continue;
                                                MapObject obtem = celltem.Objects[j];
                                                if (!CanAttackTarget(obtem)) continue;
                                                flag = true; break;
                                            }
                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(800),
                                            ActionType.DelayMagic,
                                            new List<UserMagic> { magic },
                                            celltem,
                                            false));
                                    }
                                    break;
                                case MirDirection.UpRight:
                                case MirDirection.DownRight:
                                case MirDirection.DownLeft:
                                case MirDirection.UpLeft:
                                    {
                                        Cell celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 1)));
                                        if (!flag && celltem != null && celltem?.Objects != null)
                                            for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                            {
                                                if (j >= celltem.Objects.Count) continue;
                                                MapObject obtem = celltem.Objects[j];
                                                if (!CanAttackTarget(obtem)) continue;
                                                flag = true; break;
                                            }
                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(800),
                                            ActionType.DelayMagic,
                                            new List<UserMagic> { magic },
                                            celltem,
                                            false));

                                        celltem = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -1)));
                                        if (!flag && celltem != null && celltem?.Objects != null)
                                            for (int j = celltem.Objects.Count - 1; j >= 0; j--)
                                            {
                                                if (j >= celltem.Objects.Count) continue;
                                                MapObject obtem = celltem.Objects[j];
                                                if (!CanAttackTarget(obtem)) continue;
                                                flag = true; break;
                                            }
                                        ActionList.Add(new DelayedAction(
                                            SEnvir.Now.AddMilliseconds(800),
                                            ActionType.DelayMagic,
                                            new List<UserMagic> { magic },
                                            celltem,
                                            false));
                                    }
                                    break;
                            }
                        }
                    if (flag)
                        LevelMagic(magic);
                    break;
                case MagicType.ChainLightning:

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))   //不是对应范围
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    //cells = CurrentMap.GetCells(p.Location, 0, 1);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob, p.Location));
                    //locations.Add(p.Location);
                    //cells = CurrentMap.GetCells(p.Location, 0, 4);

                    //foreach (Cell cell in cells)
                    //{
                    //	ActionList.Add(new DelayedAction(
                    //		SEnvir.Now.AddMilliseconds(500),
                    //		ActionType.DelayMagic,
                    //		new List<UserMagic> { magic },
                    //		cell,
                    //		Functions.Distance(cell.Location, p.Location))); //Central Point
                    //}
                    LevelMagic(magic);
                    break;
                case MagicType.MeteorShower:
                    ob = null;

                    magics = new List<UserMagic> { magic };

                    realTargets = new HashSet<MapObject>();

                    possibleTargets = GetTargets(CurrentMap, p.Location, 3);

                    while (realTargets.Count < (Config.MeteorShowerTargetsCount + 1) + magic.Level)   //按技能等级增加数量
                    {
                        if (possibleTargets.Count == 0) break;

                        MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                        possibleTargets.Remove(target);

                        if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                        realTargets.Add(target);
                    }

                    foreach (MapObject target in realTargets)
                    {
                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.Tempest:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))  //不是对应范围
                    {
                        cast = false;
                        break;
                    }

                    power = (magic.Level + 2) * 5;   //技能威力

                    foreach (ConquestWar war in SEnvir.ConquestWars)   //攻城处理
                    {
                        if (war.Map != CurrentMap) continue;

                        for (int i = SpellList.Count - 1; i >= 0; i--)
                        {
                            if (SpellList[i].Effect != SpellEffect.Tempest) continue;

                            SpellList[i].Despawn();
                        }

                        break;
                    }

                    cells = CurrentMap.GetCells(p.Location, 0, 1);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            power));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.ThunderStrike:
                    ob = null;

                    // 增加电闪雷鸣攻击范围
                    cells = CurrentMap.GetCells(CurrentLocation, 0, Config.ThunderStrikeGetCells);
                    foreach (Cell cell in cells)
                    {
                        if (cell.Objects == null)
                        {
                            if (SEnvir.Random.Next(Config.ThunderStrikeRandom) == 0)   //命中几率
                                locations.Add(cell.Location);

                            continue;
                        }

                        foreach (MapObject target in cell.Objects)
                        {
                            //转生挂钩雷劈的几率
                            if (SEnvir.Random.Next(4 + this.Stats[Stat.Rebirth]) == 0) continue;

                            if (!CanAttackTarget(target)) continue;

                            if (target.Race == ObjectType.Player && SEnvir.Random.Next(8 + Stats[Stat.Rebirth]) == 0) continue;

                            targets.Add(target.ObjectID);

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(500),
                                ActionType.DelayMagic,
                                new List<UserMagic> { magic },
                                target));
                        }
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.ElementalHurricane:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
                    {
                        BuffRemove(BuffType.ElementalHurricane);
                    }
                    else
                    {
                        buff = BuffAdd(BuffType.ElementalHurricane, TimeSpan.MaxValue, null, true, false, TimeSpan.FromSeconds(1));
                        buff.TickTime = TimeSpan.FromMilliseconds(500);
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.MirrorImage:
                    ob = null;   //目标=空

                    ActionList.Add(new DelayedAction(             //操作列表 添加   新的动作延迟
                        SEnvir.Now.AddMilliseconds(500),          //延迟500毫秒
                        ActionType.DelayMagic,                    //行动类型 延迟魔法
                        new List<UserMagic> { magic }));          //角色技能
                    break;

                #endregion

                #region Taoist 道士

                case MagicType.Heal:
                    if (!CanHelpTarget(ob))
                        ob = this;

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.PoisonDust:

                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.GreaterPoisonDust, out augMagic);
                    if (Magics.TryGetValue(MagicType.GreaterPoisonDust, out augMagic))
                    {
                        LevelMagic(augMagic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;
                        possibleTargets = GetTargets(CurrentMap, p.Location, 4);  //可能的目标 = 获取目标 当前角色地图 坐标 4格范围

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        int shape;

                        if (!UsePoison(1, out shape))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            shape == 0 ? PoisonType.Green : PoisonType.Red));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }
                    if (ob == null)
                        locations.Add(p.Location);

                    break;
                case MagicType.ExplosiveTalisman:
                case MagicType.ImprovedExplosiveTalisman:
                    if (!CanFlyTarget(ob))                //中间有阻碍
                    {
                        locations.Add(p.Location);           //坐标增加
                        ob = null;                           //对象等空
                        break;                               //跳出
                    }

                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.AugmentExplosiveTalisman, out augMagic);
                    if (Magics.TryGetValue(MagicType.AugmentExplosiveTalisman, out augMagic))
                    {
                        LevelMagic(augMagic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;
                        possibleTargets = GetTargets(CurrentMap, p.Location, 2);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                        if (_shape <= 0 || _shape >= 9) break;
                        if (!UseAmulet(1, _shape, out stats))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            target == ob,
                            stats));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    if (ob == null)
                        locations.Add(p.Location);

                    break;
                case MagicType.EvilSlayer:
                case MagicType.GreaterEvilSlayer:
                case MagicType.GreaterHolyStrike:
                    if (!CanFlyTarget(ob))                //中间有阻碍
                    {
                        locations.Add(p.Location);           //坐标增加
                        ob = null;                           //对象等空
                        break;                               //跳出
                    }

                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.AugmentEvilSlayer, out augMagic);
                    if (Magics.TryGetValue(MagicType.AugmentEvilSlayer, out augMagic))
                    {
                        LevelMagic(augMagic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);


                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetTargets(CurrentMap, p.Location, 2);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        if (Equipment[(int)EquipmentSlot.Amulet]?.Info.Stats[Stat.HolyAffinity] > 0)
                        {
                            _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                            if (_shape <= 0 || _shape >= 9) break;
                            UseAmulet(1, _shape, out stats);
                        }
                        else
                            stats = null;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            target == ob,
                            stats));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    if (ob == null)
                        locations.Add(p.Location);

                    break;

                case MagicType.MagicResistance:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(1, _shape, out stats))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            stats));
                    }
                    break;
                case MagicType.ElementalSuperiority:

                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(1, _shape, out stats))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell,
                            stats));
                    }
                    break;
                case MagicType.Resilience:
                case MagicType.BloodLust:
                case MagicType.LifeSteal:

                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, _shape))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 3);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.TrapOctagon:

                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        p.Location));
                    break;
                case MagicType.SummonSkeleton:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(1, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Skeleton)));
                    break;
                case MagicType.SummonJinSkeleton:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(2, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinSkeleton)));
                    break;
                case MagicType.SummonShinsu:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(5, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Shinsu)));
                    break;
                case MagicType.SummonDemonicCreature:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(25, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap,
                        Functions.Move(CurrentLocation, p.Direction),
                        SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.InfernalSoldier)));
                    break;
                case MagicType.Invisibility:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(2, _shape))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.StrengthOfFaith:
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(5, _shape))
                    {
                        cast = false;
                        break;
                    }
                    targets.Add(ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Transparency:
                    if (Buffs.Any(x => x.Type == BuffType.Transparency))
                        BuffRemove(BuffType.Transparency);
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(10, _shape))
                    {
                        cast = false;
                        break;
                    }

                    //if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    //{
                    //cast = false;
                    //break;
                    //}

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location));
                    break;
                case MagicType.CelestialLight:
                    if (Buffs.Any(x => x.Type == BuffType.CelestialLight))
                        BuffRemove(BuffType.CelestialLight);  //删除 阴阳法环BUFF
                    ob = null;
                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!UseAmulet(10, _shape))    //使用符 10张
                    {
                        cast = false;
                        break;
                    }

                    targets.Add(ObjectID);  //添加目标

                    ActionList.Add(new DelayedAction(            //添加操作列表     新的动作延迟
                        SEnvir.Now.AddMilliseconds(1500),        //添加毫秒 1500
                        ActionType.DelayMagic,                   //动作类型，延迟魔法
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.MassTransparency:
                case MagicType.MassInvisibility:
                    ob = null;

                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange) || !UseAmulet(2, _shape))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.MassHeal:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);
                    cells = CurrentMap.GetCells(p.Location, 0, 2);

                    foreach (Cell cell in cells)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, p.Location) * 48),
                            ActionType.DelayMagic,
                            new List<UserMagic> { magic },
                            cell));
                    }
                    break;
                case MagicType.TaoistCombatKick:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(CurrentLocation, p.Direction)),
                        p.Direction));
                    break;
                case MagicType.Purification:

                    magics = new List<UserMagic> { magic };

                    Magics.TryGetValue(MagicType.AugmentPurification, out augMagic);
                    if (Magics.TryGetValue(MagicType.AugmentPurification, out augMagic))
                    {
                        LevelMagic(augMagic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (ob != null && (CanAttackTarget(ob) || CanHelpTarget(ob)))
                        realTargets.Add(ob);
                    else
                    {
                        realTargets.Add(this);
                        ob = null;
                    }

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetAllObjects(p.Location, 3);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            if (!CanAttackTarget(target) && CanHelpTarget(target))
                                realTargets.Add(target);
                        }
                    }

                    count = -1;

                    foreach (MapObject target in realTargets)
                    {
                        _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                        if (_shape <= 0 || _shape >= 9)
                        {
                            break;
                        }
                        if (!UseAmulet(2, _shape))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    break;
                case MagicType.Resurrection:
                    magics = new List<UserMagic> { magic };

                    Magics.TryGetValue(MagicType.OathOfThePerished, out augMagic);
                    if (Magics.TryGetValue(MagicType.OathOfThePerished, out augMagic))
                    {
                        LevelMagic(augMagic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (!Config.ResurrectionOrder)  //如果服务端设置为不使用回生术命令
                    {
                        if ((InGroup(ob as PlayerObject) || InGuild(ob as PlayerObject)) && ob.Dead)   //判断是否组队  或   是否行会   并死亡
                            realTargets.Add(ob);
                        else
                            ob = null;
                    }
                    else
                    {
                        if (ob.Race == ObjectType.Player)
                        {
                            PlayerObject player = (PlayerObject)ob;

                            if (player.Character.Account.AllowResurrectionOrder)
                            {
                                if (ob.Dead)
                                    realTargets.Add(ob);
                                else
                                    ob = null;
                            }
                            else
                            {
                                Connection.ReceiveChat("System.RefuseResurrectionOrder".Lang(Connection.Language), MessageType.System);
                            }
                        }
                    }

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;

                        possibleTargets = GetAllObjects(p.Location, 3);//回生范围 3为格子数

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            //修复回生术引起系统奔溃
                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            if (target is PlayerObject)
                            {
                                if (!Config.ResurrectionOrder)
                                {
                                    if ((InGroup(target as PlayerObject) || InGuild(target as PlayerObject)) && target.Dead)
                                        realTargets.Add(target);
                                }
                                else
                                {
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = (PlayerObject)ob;

                                        if (player.Character.Account.AllowResurrectionOrder)
                                        {
                                            if (target.Dead)
                                                realTargets.Add(target);
                                        }
                                        else
                                        {
                                            Connection.ReceiveChat("System.RefuseResurrectionOrder".Lang(Connection.Language), MessageType.System);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    count = -1;

                    foreach (MapObject target in realTargets)
                    {
                        if (!UseAmulet(1, 0))//回生术 修改 灵魂护身符 sharp=0
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    break;
                case MagicType.DemonExplosion:
                    ob = null;

                    _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                    if (_shape <= 0 || _shape >= 9)
                    {
                        cast = false;
                        break;
                    }
                    if (Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead) || !UseAmulet(20, _shape, out stats))
                    {
                        cast = false;
                        break;
                    }

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        stats));
                    break;
                case MagicType.Infection:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500 + Functions.Distance(CurrentLocation, ob.CurrentLocation) * 48),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Neutralize:
                    magics = new List<UserMagic> { magic };
                    Magics.TryGetValue(MagicType.AugmentNeutralize, out augMagic);
                    if (Magics.TryGetValue(MagicType.AugmentNeutralize, out augMagic))
                    {
                        LevelMagic(magic);
                    }

                    realTargets = new HashSet<MapObject>();

                    if (CanAttackTarget(ob))
                        realTargets.Add(ob);

                    if (augMagic != null && SEnvir.Now > augMagic.Cooldown && Level >= augMagic.Info.NeedLevel1)
                    {
                        magics.Add(augMagic);
                        power = augMagic.GetPower() + 1;
                        possibleTargets = GetTargets(CurrentMap, p.Location, 2);

                        while (power >= realTargets.Count)
                        {
                            if (possibleTargets.Count == 0) break;

                            MapObject target = possibleTargets[SEnvir.Random.Next(possibleTargets.Count)];

                            possibleTargets.Remove(target);

                            if (!Functions.InRange(CurrentLocation, target.CurrentLocation, Globals.MagicRange)) continue;

                            realTargets.Add(target);
                        }
                    }

                    count = -1;
                    foreach (MapObject target in realTargets)
                    {
                        _shape = Equipment[(int)EquipmentSlot.Amulet]?.Info.Shape ?? -1;
                        if (_shape <= 0 || _shape >= 9)
                        {
                            break;
                        }

                        if (!UseAmulet(1, _shape, out stats))
                            break;

                        if (augMagic != null)
                            count++;

                        targets.Add(target.ObjectID);
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(1400 + Functions.Distance(CurrentLocation, target.CurrentLocation) * 48),
                            ActionType.DelayMagic,
                            magics,
                            target,
                            target == ob,
                            stats));
                    }

                    if (count > 0)
                    {
                        augMagic.Cooldown = SEnvir.Now.AddMilliseconds(augMagic.Info.Delay);
                        Enqueue(new S.MagicCooldown { InfoIndex = augMagic.Info.Index, Delay = augMagic.Info.Delay });
                    }

                    if (ob == null)
                        locations.Add(p.Location);
                    break;
                case MagicType.DarkSoulPrison:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }
                    power = magic.Level + 5;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location,
                        power));
                    break;

                #endregion

                #region Assassin 刺客

                case MagicType.PoisonousCloud:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.Cloak:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.Cloak)) break;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.WraithGrip:

                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }

                    if (ob.Race == ObjectType.Player ? ob.Level >= Level : ob.Level > Level + 15)
                    {
                        Connection.ReceiveChat("Skills.WraithLevel".Lang(Connection.Language, ob.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.WraithLevel".Lang(con.Language, ob.Name), MessageType.System);

                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Abyss:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }


                    if ((ob.Race == ObjectType.Player && ob.Level >= Level) || (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.IsBoss))
                    {
                        Connection.ReceiveChat("Skills.AbyssLevel".Lang(Connection.Language, ob.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.AbyssLevel".Lang(con.Language, ob.Name), MessageType.System);

                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;
                case MagicType.Rake:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(600),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction))));

                    break;
                case MagicType.HellFire:
                    if (!CanAttackTarget(ob))
                    {
                        locations.Add(p.Location);
                        ob = null;
                        cast = false;
                        break;
                    }

                    targets.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1200),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        ob));
                    break;

                case MagicType.TheNewBeginning:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.SummonPuppet:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                    ob = null;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));
                    break;
                case MagicType.DanceOfSwallow:

                    DanceOfSwallowEnd(magic, ob);

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    return;

                case MagicType.DarkConversion:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion)) break;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));

                    break;
                case MagicType.DragonRepulse:
                    ob = null;

                    buff = BuffAdd(BuffType.DragonRepulse, TimeSpan.FromSeconds(6), null, true, false, TimeSpan.FromSeconds(1));
                    buff.TickTime = TimeSpan.FromMilliseconds(500);
                    break;
                case MagicType.FlashOfLight:
                    ob = null;

                    magics = new List<UserMagic> { magic };
                    /*   buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);

                        if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                        {
                            BuffRemove(buff);
                            magics.Add(augMagic);
                            if (buff.Stats[Stat.TheNewBeginning] > 1)
                                BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                        }*/

                    for (int i = 1; i <= 2; i++)
                    {
                        location = Functions.Move(CurrentLocation, p.Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;
                        locations.Add(cell.Location);

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(400),
                            ActionType.DelayMagic,
                            magics,
                            cell));
                    }
                    LevelMagic(magic);
                    break;
                case MagicType.Concentration:
                    ob = null;

                    if (Buffs.Any(x => x.Type == BuffType.Concentration)) break;

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic }));

                    break;
                case MagicType.SwordOfVengeance:
                    ob = null;

                    if (!Functions.InRange(CurrentLocation, p.Location, Globals.MagicRange))
                    {
                        cast = false;
                        break;
                    }

                    locations.Add(p.Location);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(1800),
                        ActionType.DelayMagic,
                        new List<UserMagic> { magic },
                        p.Location,
                        10));
                    break;

                #endregion

                default:
                    Connection.ReceiveChat("没有实现".Lang(Connection.Language), MessageType.System);
                    break;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                    if (Buffs.Any(x => x.Type == BuffType.Cloak))
                    {
                        BuffRemove(BuffType.Cloak);
                        break;
                    }
                    ChangeHP(-(Stats[Stat.Health] * magic.Cost / 1000));
                    break;
                case MagicType.DragonRepulse:
                    ChangeHP(-(Stats[Stat.Health] * magic.Cost / 1000));
                    ChangeMP(-(Stats[Stat.Mana] * magic.Cost / 1000));
                    break;
                case MagicType.DarkConversion:
                    if (Buffs.Any(x => x.Type == BuffType.DarkConversion))
                    {
                        BuffRemove(BuffType.DarkConversion);
                        break;
                    }
                    ChangeMP(-magic.Cost);
                    break;
                case MagicType.ElementalHurricane:
                    if (Buffs.Any(x => x.Type == BuffType.ElementalHurricane))
                        break;
                    ChangeMP(-(Stats[Stat.Mana] * magic.Cost / 1000));
                    break;
                default:
                    ChangeMP(-magic.Cost);
                    break;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.DarkConversion:
                case MagicType.ChangeOfSeasons:
                case MagicType.TheNewBeginning:
                case MagicType.Transparency:
                case MagicType.Concentration:
                    break;
                default:
                    BuffRemove(BuffType.Cloak);
                    BuffRemove(BuffType.Transparency);
                    BuffRemove(BuffType.SuperTransparency);
                    break;
            }

            switch (magic.Info.Magic)
            {
                case MagicType.Cloak:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.ChangeOfSeasons:
                case MagicType.TheNewBeginning:
                case MagicType.MirrorImage:
                case MagicType.SummonPuppet:
                case MagicType.SummonSkeleton:
                case MagicType.Scarecrow:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonDemonicCreature:
                    break;
                case MagicType.Defiance:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Invincibility:

                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.Teleportation:
                case MagicType.GeoManipulation:
                case MagicType.MagicShield:
                case MagicType.FrostBite:
                case MagicType.Renounce:
                case MagicType.JudgementOfHeaven:
                case MagicType.SuperiorMagicShield:

                case MagicType.Heal:
                case MagicType.Invisibility:
                case MagicType.MagicResistance:
                case MagicType.MassInvisibility:
                case MagicType.MassTransparency:
                case MagicType.Resilience:
                case MagicType.ElementalSuperiority:
                case MagicType.MassHeal:
                case MagicType.BloodLust:
                case MagicType.Resurrection:
                case MagicType.Transparency:
                case MagicType.CelestialLight:
                case MagicType.LifeSteal:
                case MagicType.SummonShinsu:
                case MagicType.StrengthOfFaith:
                case MagicType.DarkSoulPrison:

                case MagicType.PoisonousCloud:
                case MagicType.DarkConversion:
                case MagicType.Concentration:
                    break;
                default:
                    CombatTime = SEnvir.Now;
                    break;
            }

            if (cast)
            {
                Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
            }

            Direction = ob == null || ob == this ? p.Direction : Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);

            if (Stats[Stat.Comfort] < 150)
                RegenTime = SEnvir.Now + RegenDelay;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsCastTime);
            MagicTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsMagicDelay);

            if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || PoisonList.Any(x => x.Type == PoisonType.Neutralize))
                MagicTime += TimeSpan.FromMilliseconds(Config.GlobalsMagicDelay);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            //放技能则暂停钓鱼和姜太公buff
            FishingInterrupted();

            //魔法攻击元素
            Element element = Element.None;
            switch (magic.Info.Magic)
            {
                case MagicType.ElementalSuperiority:
                    if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.Amulet)
                    {
                        foreach (KeyValuePair<Stat, int> stat in Equipment[(int)EquipmentSlot.Amulet].Info.Stats.Values)
                        {
                            switch (stat.Key)
                            {
                                case Stat.FireAffinity:
                                    element = Element.Fire;
                                    break;
                                case Stat.IceAffinity:
                                    element = Element.Ice;
                                    break;
                                case Stat.LightningAffinity:
                                    element = Element.Lightning;
                                    break;
                                case Stat.WindAffinity:
                                    element = Element.Wind;
                                    break;
                                case Stat.HolyAffinity:
                                    element = Element.Holy;
                                    break;
                                case Stat.DarkAffinity:
                                    element = Element.Dark;
                                    break;
                                case Stat.PhantomAffinity:
                                    element = Element.Phantom;
                                    break;
                            }
                        }
                    }
                    break;
            }

            Broadcast(new S.ObjectMagic
            {
                ObjectID = ObjectID,
                Direction = Direction,
                CurrentLocation = CurrentLocation,
                Type = p.Type,
                Targets = targets,
                Locations = locations,
                Cast = cast,
                Slow = slow,
                AttackElement = element,

            });
        }
        /// <summary>
        /// 魔法技能切换
        /// </summary>
        /// <param name="p"></param>
        public void MagicToggle(C.MagicToggle p)
        {
            UserMagic magic;

            if (!Magics.TryGetValue(p.Magic, out magic) || Level < magic.Info.NeedLevel1 || Horse != HorseType.None) return;

            switch (p.Magic)
            {
                case MagicType.Thrusting:
                    Character.CanThrusting = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.HalfMoon:
                    Character.CanHalfMoon = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.DestructiveSurge:
                    Character.CanDestructiveSurge = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.FlameSplash:
                    Character.CanFlameSplash = p.CanUse;
                    Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = p.CanUse });
                    break;
                case MagicType.DemonicRecovery:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    if (Pets.All(x => x.MonsterInfo.Flag != MonsterFlag.InfernalSoldier || x.Dead))
                        return;

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    DemonicRecoveryEnd(magic);
                    break;
                case MagicType.FlamingSword:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanFlamingSword)
                    {
                        Connection.ReceiveChat("Skills.ChargeFail".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.ChargeFail".Lang(con.Language, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        FlamingSwordTime = SEnvir.Now.AddSeconds(26);
                        CanFlamingSword = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanFlamingSword });
                    }

                    if (Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.MaelstromBlade, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }
                    break;
                case MagicType.DragonRise:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanDragonRise)
                    {
                        Connection.ReceiveChat("Skills.ChargeFail".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.ChargeFail".Lang(con.Language, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        DragonRiseTime = SEnvir.Now.AddSeconds(26);
                        CanDragonRise = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanDragonRise });
                    }

                    if (Magics.TryGetValue(MagicType.FlamingSword, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.MaelstromBlade, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }
                    break;
                case MagicType.BladeStorm:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanBladeStorm)
                    {
                        Connection.ReceiveChat("Skills.ChargeFail".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.ChargeFail".Lang(con.Language, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        BladeStormTime = SEnvir.Now.AddSeconds(26);
                        CanBladeStorm = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanBladeStorm });
                    }

                    if (Magics.TryGetValue(MagicType.FlamingSword, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.MaelstromBlade, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }
                    break;
                case MagicType.MaelstromBlade:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    LevelMagic(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });

                    if (CanMaelstromBlade)
                    {
                        Connection.ReceiveChat("Skills.ChargeFail".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("Skills.ChargeFail".Lang(con.Language, magic.Info.Name), MessageType.System);
                    }
                    else
                    {
                        MaelstromBladeTime = SEnvir.Now.AddSeconds(26);
                        CanMaelstromBlade = true;
                        Enqueue(new S.MagicToggle { Magic = p.Magic, CanUse = CanMaelstromBlade });
                    }

                    if (Magics.TryGetValue(MagicType.FlamingSword, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.DragonRise, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }

                    if (Magics.TryGetValue(MagicType.BladeStorm, out magic) && SEnvir.Now.AddSeconds(10) > magic.Cooldown)
                    {
                        magic.Cooldown = SEnvir.Now.AddSeconds(10);
                        Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 10000 });
                    }
                    break;
                case MagicType.Endurance:
                    if (magic.Cost > CurrentMP || SEnvir.Now < magic.Cooldown || Dead || (Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

                    ChangeMP(-magic.Cost);
                    EnduranceEnd(magic);
                    magic.Cooldown = SEnvir.Now.AddMilliseconds(magic.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = magic.Info.Delay });
                    break;
            }
        }
        /// <summary>
        /// 挖矿方向
        /// </summary>
        /// <param name="direction"></param>
        public void Mining(MirDirection direction)
        {
            if (SEnvir.Now < ActionTime || SEnvir.Now < AttackTime)
            {
                if (!PacketWaiting)
                {
                    ActionList.Add(new DelayedAction(ActionTime, ActionType.Mining, direction));
                    PacketWaiting = true;
                }
                else
                    Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            if (!CanAttack)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            CombatTime = SEnvir.Now;

            if (Stats[Stat.Comfort] < 150)
                RegenTime = SEnvir.Now + RegenDelay;
            Direction = direction;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsAttackTime);

            int aspeed = Stats[Stat.AttackSpeed];
            int attackDelay = (int)(Config.GlobalsAttackDelay - aspeed / 10.0 * Config.GlobalsASpeedRate);
            attackDelay = Math.Max(100, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            TimeSpan slow = TimeSpan.Zero;
            if (poison != null)
            {
                slow = TimeSpan.FromMilliseconds(poison.Value * 100);
                ActionTime += slow;
            }

            if (BagWeight > Stats[Stat.BagWeight] || HandWeight > Stats[Stat.HandWeight] || WearWeight > Stats[Stat.WearWeight] || PoisonList.Any(x => x.Type == PoisonType.Neutralize))
                AttackTime += TimeSpan.FromMilliseconds(attackDelay);

            bool result = false;
            if (CurrentMap.Info.CanMine && CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction)) == null)
            {
                UserItem weap = Equipment[(int)EquipmentSlot.Weapon];

                if (weap != null && weap.Info.Effect == ItemEffect.PickAxe && (weap.CurrentDurability > 0 || weap.Info.Durability > 0))
                {
                    DamageItem(GridType.Equipment, (int)EquipmentSlot.Weapon, 4);

                    //挖矿
                    foreach (MineInfo info in CurrentMap.Info.Mining)
                    {
                        UserItemFlags bound = Config.DigMineral ? UserItemFlags.Bound : UserItemFlags.None;

                        //挖矿几率判断   如果随机值小于等于0    随机值*采矿成功几率 不超百分百
                        if (SEnvir.Random.Next(info.Chance - info.Chance * Math.Min(1, Stats[Stat.MiningSuccessRate] / 100)) <= 0)
                        {
                            ItemCheck check = new ItemCheck(info.Item, 1, bound, TimeSpan.Zero);

                            if (!CanGainItems(false, check)) continue;

                            UserItem item = SEnvir.CreateDropItem(check);
                            //记录物品来源
                            SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "挖矿".Lang(Connection.Language), Character?.CharacterName);

                            GainItem(item);

                            #region 人物挖矿事件
                            //队列一个事件, 不要忘记添加listener
                            SEnvir.EventManager.QueueEvent(
                                new PlayerMine(EventTypes.PlayerMine,
                                    new PlayerMineEventArgs { item = item }));
                            #endregion
                        }
                        else
                        {
                            #region 人物挖矿事件
                            //队列一个事件, 不要忘记添加listener
                            SEnvir.EventManager.QueueEvent(
                                new PlayerMine(EventTypes.PlayerMine,
                                    new PlayerMineEventArgs { item = null }));
                            #endregion
                        }
                    }

                    bool hasRubble = false;

                    foreach (MapObject ob in CurrentCell.Objects)
                    {
                        if (ob.Race != ObjectType.Spell) continue;

                        SpellObject rubble = (SpellObject)ob;
                        if (rubble.Effect != SpellEffect.Rubble) continue;

                        hasRubble = true;

                        rubble.Power++;
                        rubble.Broadcast(new S.ObjectSpellChanged { ObjectID = ob.ObjectID, Power = rubble.Power });
                        rubble.TickTime = SEnvir.Now.AddMinutes(1);
                        break;
                    }

                    if (!hasRubble)
                    {
                        SpellObject ob = new SpellObject
                        {
                            DisplayLocation = CurrentLocation,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromMinutes(1),
                            TickTime = SEnvir.Now.AddMinutes(1),
                            Owner = this,
                            Effect = SpellEffect.Rubble,
                        };

                        ob.Spawn(CurrentMap.Info, CurrentLocation);

                        PauseBuffs();  //挖矿的时候BUFF判断
                    }
                    result = true;
                }
            }

            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);
            BuffRemove(BuffType.SuperTransparency);
            Broadcast(new S.ObjectMining { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Slow = slow, Effect = result });
        }
        #endregion
    }
}
