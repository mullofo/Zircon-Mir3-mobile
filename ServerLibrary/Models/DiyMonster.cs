using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 自定义怪物
    /// </summary>
    public class DiyMonster : MonsterObject
    {
        /// <summary>
        /// 攻击反倍数率
        /// </summary>
        private int AttackPowerRate = 10;
        public DateTime TeleportTime = SEnvir.Now;
        /// <summary>
        /// 自定义怪物信息
        /// </summary>
        /// <param name="monsterInfo"></param>
        public DiyMonster(MonsterInfo monsterInfo)
        {
            MonsterInfo = monsterInfo;
        }
        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="power"></param>
        /// <param name="element"></param>
        /// <param name="canReflect"></param>
        /// <param name="ignoreShield"></param>
        /// <param name="canCrit"></param>
        /// <param name="canStruck"></param>
        /// <returns></returns>
        /*public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (MonsterInfo.AI == 423)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 155)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 151)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 152)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 153)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 407)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 474)
            {
                return base.Attacked(ob, 1, element, ignoreShield, canCrit);
            }
            else if (MonsterInfo.AI == 355)
            {
                return base.Attacked(ob, 1, element, canReflect, ignoreShield, false, canStruck);
            }
            else if (MonsterInfo.AI == 356)
            {
                return base.Attacked(ob, 1, element, canReflect, ignoreShield, false, canStruck);
            }
            else
                return base.Attacked(ob, power, element, ignoreShield, canCrit);
        }*/

        protected override void OnSpawned()
        {
            base.OnSpawned();

            ActionTime = SEnvir.Now.AddSeconds(2);

            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }
        /// <summary>
        /// 进程目标
        /// </summary>
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (CanAttack)
                OnAiRun();

            if (MonsterInfo.DiyMoveMode == 1) return;//自定义怪物移动方式 1 原地不懂

            if (CurrentLocation == Target.CurrentLocation)//人物重叠了移开
            {
                MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) break;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
            }

            if (MonsterInfo.DiyMoveMode == -1) //自定义怪物移动方式 -1 躲避模式，向反方向跑
            {
                MirDirection direction;
                int rotation;
                if (Functions.InRange(Target.CurrentLocation, CurrentLocation, 6))
                {
                    direction = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                    rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                return;
            }

            else if (MonsterInfo.DiyMoveMode == 2)//自定义怪物移动方式 2 瞬移模式
            {
                if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 3) && SEnvir.Now > TeleportTime)
                {
                    MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                    Cell cell = null;
                    for (int i = 0; i < 8; i++)
                    {
                        cell = CurrentMap.GetCell(Functions.Move(Target.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                        if (cell == null || cell.Movements != null)
                        {
                            cell = null;
                            continue;
                        }
                        break;
                    }

                    if (cell != null)
                    {
                        Direction = Functions.DirectionFromPoint(cell.Location, Target.CurrentLocation);
                        Teleport(CurrentMap, cell.Location);

                        TeleportTime = SEnvir.Now.AddSeconds(5);
                    }
                }
                return;
            }

            else if ((MonsterInfo.DiyMoveMode == 0) && (!InAttackRange()))//修正自定义怪物会与玩家重叠的问题
                MoveTo(Target.CurrentLocation);
        }
        /// <summary>
        /// AI执行
        /// </summary>
        public void OnAiRun()
        {
            if (MonsterInfo.MonDiyAiActions.Count == 0)
            {
                Attack();
                return;
            }

            int AttackPower = 0;
            Element AttackElement = Element.None;
            List<MapObject> targets = null;
            List<Point> locations = null;
            int DelayTime = 300;
            int ActId = -1;

            //处理动作
            for (int i = 0; i < MonsterInfo.MonDiyAiActions.Count; i++)
            {
                MonDiyAiAction CurAction = MonsterInfo.MonDiyAiActions[i];
                if (CheckAction(CurAction))
                {
                    ActId = i;
                    switch (CurAction.PowerType)
                    {
                        case MonDiyPowerType.NONE:
                            break;
                        case MonDiyPowerType.DC:
                            AttackPower += GetDC();
                            break;
                        case MonDiyPowerType.MC:
                            AttackPower += GetMC();
                            break;
                        case MonDiyPowerType.SC:
                            AttackPower += GetSC();
                            break;
                        default:
                            break;
                    }
                    AttackElement = CurAction.ElementType;
                    targets = GetAttackTargets(CurAction.Target, out locations);
                    DelayTime = CurAction.nDelay;
                    break;
                }
            }
            if (ActId >= 0)
            {
                Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                MonDiyAiAction CurAction = MonsterInfo.MonDiyAiActions[ActId];
                List<uint> targetIDs = new List<uint>();

                switch (CurAction.ActType)
                {
                    case MonDiyActType.Attack:
                        Broadcast(new S.DiyObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, SpellMagicID = CurAction.SystemMagic == MagicType.None ? CurAction.MagicID : (int)CurAction.SystemMagic, ActID = CurAction.ActionID });
                        break;
                    case MonDiyActType.RangeAttack:
                    case MonDiyActType.RemoteAttack:
                        Broadcast(new S.DiyObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, SpellMagicID = CurAction.SystemMagic == MagicType.None ? CurAction.MagicID : (int)CurAction.SystemMagic, Targets = targetIDs, Locations = locations, ActID = CurAction.ActionID/*, RomoteMagicID = CurAction.MagicID*/ });
                        DelayTime += 200;
                        break;
                    case MonDiyActType.ShowEffect:
                        Broadcast(new S.DiyObjectEffect { ObjectID = ObjectID, Direction = Direction, EffectID = CurAction.SystemMagic == MagicType.None ? CurAction.MagicID : (int)CurAction.SystemMagic });
                        break;
                    default:
                        break;
                }
                UpdateAttackTime();

                if (locations.Count == 0)
                {
                    foreach (MapObject ob in targets)
                    {
                        targetIDs.Add(ob.ObjectID);
                        locations.Add(ob.CurrentLocation);
                        ApplyAttackEffect(CurAction, ob, AttackPower, AttackElement, DelayTime, CurAction.ActType == MonDiyActType.RangeAttack);
                    }
                }
                else
                {
                    foreach (MapObject ob in targets)
                    {
                        targetIDs.Add(ob.ObjectID);
                        ApplyAttackEffect(CurAction, ob, AttackPower, AttackElement, DelayTime, CurAction.ActType == MonDiyActType.RangeAttack);
                    }
                }
            }
        }
        /// <summary>
        /// 应用攻击效果
        /// </summary>
        /// <param name="CurAction">自定义怪物AI</param>
        /// <param name="ob">地图对象</param>
        /// <param name="AttackPower">攻击威力</param>
        /// <param name="AttackElement">攻击元素</param>
        /// <param name="DelayTime">延迟时间</param>
        /// <param name="MagicAttack">魔法攻击</param>
        public void ApplyAttackEffect(MonDiyAiAction CurAction, MapObject ob, int AttackPower, Element AttackElement, int DelayTime, bool MagicAttack = false)
        {
            if (ob?.Node == null) return;
            int damage = AttackPower * AttackPowerRate / 10;
            if (CurAction.ActType == MonDiyActType.RemoteAttack)
            {
                DelayTime += Functions.Distance(CurrentLocation, ob.CurrentLocation) * 48;
            }
            foreach (MonAttackEffect AttackEffect in CurAction.MonAttackEffects)
            {
                switch (AttackEffect.AtkEffect)
                {
                    case Library.AttackEffect.Damage:
                        damage += AttackEffect.nParameter;
                        ActionList.Add(new DelayedAction(
                          SEnvir.Now.AddMilliseconds(DelayTime),
                          ActionType.DelayAttack,
                          ob,
                          damage,
                          AttackElement));
                        break;
                    case Library.AttackEffect.HAPosion:
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Green,
                            Owner = this,
                            TickCount = damage + Stats[Stat.DarkAttack] * 2,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Red,
                            Owner = this,
                            TickCount = damage + Stats[Stat.DarkAttack] * 2,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        break;
                    case Library.AttackEffect.HpPosion:
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Green,
                            Owner = this,
                            TickCount = damage + Stats[Stat.DarkAttack] * 2,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        break;
                    case Library.AttackEffect.AcPosion:
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Red,
                            Owner = this,
                            TickCount = damage + Stats[Stat.DarkAttack] * 2,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        break;
                    case Library.AttackEffect.StonePosion:
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Paralysis,
                            Owner = this,
                            TickCount = 3,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        break;
                    case Library.AttackEffect.FaintPosion:
                        ob.ApplyPoison(new Poison
                        {
                            Value = AttackEffect.nParameter + Level / 14,
                            Type = PoisonType.Slow,
                            Owner = this,
                            TickCount = 3,
                            TickFrequency = TimeSpan.FromSeconds(2),
                        });
                        break;
                    case Library.AttackEffect.Push:
                        MirDirection mirDirection = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        ob.Pushed(mirDirection, AttackEffect.nParameter);
                        break;
                    case Library.AttackEffect.Pullover:
                        ob.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction));
                        break;
                    case Library.AttackEffect.Heal:
                        if (ob.CurrentHP >= ob.Stats[Stat.Health] || ob.Buffs.Any(x => x.Type == BuffType.Heal)) continue;
                        Stats buffStats = new Stats
                        {
                            [Stat.Healing] = damage + Stats[Stat.HolyAttack] * 2,
                            [Stat.HealingCap] = 30 + AttackEffect.nParameter,
                        };
                        ob.BuffAdd(BuffType.Heal, TimeSpan.FromSeconds(buffStats[Stat.Healing] / buffStats[Stat.HealingCap]), buffStats, false, false, TimeSpan.FromSeconds(1));

                        break;
                    case Library.AttackEffect.Invisib:
                        if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility)) continue;

                        Stats bufStats = new Stats
                        {
                            [Stat.Invisibility] = 1
                        };
                        ob.BuffAdd(BuffType.Invisibility, TimeSpan.FromSeconds(damage + Stats[Stat.PhantomAttack] * 2), bufStats, true, false, TimeSpan.Zero);

                        break;
                    case Library.AttackEffect.Tranport:
                        if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility)) continue;
                        ob.Teleport(CurrentMap, ob.CurrentLocation, false);

                        Stats bufftStats = new Stats
                        {
                            [Stat.Transparency] = 1
                        };

                        ob.BuffAdd(BuffType.Transparency, TimeSpan.FromSeconds(damage + Stats[Stat.PhantomAttack] * 2), bufftStats, true, false, TimeSpan.Zero);

                        break;
                    case Library.AttackEffect.Jumpto:
                        Cell cell = ob.CurrentMap.GetCell(Functions.Move(ob.CurrentLocation, Direction, 1));
                        if (cell == null || cell.Movements != null)
                            cell = ob.CurrentCell;
                        Teleport(ob.CurrentMap, cell.Location);
                        break;
                    case Library.AttackEffect.Recall:
                        cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, 1));

                        if (cell == null || cell.Movements != null)
                            cell = CurrentCell;

                        ob.Teleport(CurrentMap, cell.Location);
                        break;
                    case Library.AttackEffect.LifeSteal:
                        Stats buffsStats = new Stats
                        {
                            [Stat.LifeSteal] = 4 + AttackEffect.nParameter
                        };

                        ob.BuffAdd(BuffType.LifeSteal, TimeSpan.FromSeconds(damage + Stats[Stat.DarkAttack] * 2), buffsStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.Clearstatus:
                        Purify(ob);
                        break;
                    case Library.AttackEffect.AddDc:
                        Stats buffDcStats = new Stats
                        {
                            [Stat.MaxDC] = 5 + AttackEffect.nParameter
                        };
                        ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(damage), buffDcStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.AddMc:
                        Stats buffMcStats = new Stats
                        {
                            [Stat.MaxMC] = 5 + AttackEffect.nParameter
                        };
                        ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(damage), buffMcStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.AddSc:
                        Stats buffScStats = new Stats
                        {
                            [Stat.MaxSC] = 5 + AttackEffect.nParameter
                        };
                        ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(damage), buffScStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.AddAc:
                        Stats buffAcStats = new Stats
                        {
                            [Stat.MaxAC] = 5 + AttackEffect.nParameter
                        };
                        ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(damage), buffAcStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.AddMac:
                        Stats buffMrStats = new Stats
                        {
                            [Stat.MaxMR] = 5 + AttackEffect.nParameter
                        };
                        ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(damage), buffMrStats, true, false, TimeSpan.Zero);
                        break;
                    case Library.AttackEffect.Power:
                        if (ob == this)
                            AttackPowerRate = AttackEffect.nParameter * 10;
                        break;
                    case Library.AttackEffect.Say:
                        Say(AttackEffect.sParameter);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 检查动作
        /// </summary>
        /// <param name="DiyAction"></param>
        /// <returns></returns>
        private bool CheckAction(MonDiyAiAction DiyAction)
        {
            foreach (MonActCheck ActCheck in DiyAction.MonActChecks)
            {
                switch (ActCheck.CheckType)
                {
                    case MonCheckType.NEAR:
                        if (!Compare(ActCheck.Operators, Functions.Distance(CurrentLocation, Target.CurrentLocation), ActCheck.IntParameter)) return false;
                        break;
                    case MonCheckType.RANDOM:
                        if (!Compare(ActCheck.Operators, SEnvir.Random.Next(100), ActCheck.IntParameter)) return false;
                        break;
                    case MonCheckType.SURROUNDED:
                        if (!Compare(ActCheck.Operators, GetTargets(CurrentMap, CurrentLocation, 1).Count, ActCheck.IntParameter)) return false;
                        break;
                    case MonCheckType.LOWHP:
                        if (!Compare(ActCheck.Operators, CurrentHP, Stats[Stat.Health] * ActCheck.IntParameter / 100)) return false;
                        break;
                    case MonCheckType.POWER:
                        if (!Compare(ActCheck.Operators, AttackPowerRate, ActCheck.IntParameter * 10)) return false;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
        /// <summary>
        /// 对比
        /// </summary>
        /// <param name="op"></param>
        /// <param name="pValue"></param>
        /// <param name="cValue"></param>
        /// <returns></returns>
        private bool Compare(Operators op, long pValue, long cValue)
        {
            switch (op)
            {
                case Operators.Equal:
                    return pValue == cValue;
                case Operators.NotEqual:
                    return pValue != cValue;
                case Operators.LessThan:
                    return pValue < cValue;
                case Operators.LessThanOrEqual:
                    return pValue <= cValue;
                case Operators.GreaterThan:
                    return pValue > cValue;
                case Operators.GreaterThanOrEqual:
                    return pValue >= cValue;
                default: return false;
            }
        }
        /// <summary>
        /// 获取攻击目标
        /// </summary>
        /// <param name="hitType"></param>
        /// <param name="locations"></param>
        /// <returns></returns>
        public List<MapObject> GetAttackTargets(TargetType hitType, out List<Point> locations)
        {
            Point location;
            locations = new List<Point>();
            List<MapObject> targets = new List<MapObject>();
            switch (hitType)
            {
                case TargetType.SELF:
                    targets.Add(this);
                    break;
                case TargetType.TARGET:
                    targets.Add(Target);
                    break;
                case TargetType.LINE://需完善TODO
                    for (int i = 1; i <= 8; i++)
                    {
                        location = Functions.Move(CurrentLocation, Direction, i);
                        Cell cell = CurrentMap.GetCell(location);

                        if (cell == null) continue;
                        locations.Add(cell.Location);

                        if (cell?.Objects == null) continue;

                        foreach (MapObject ob in cell.Objects)
                        {
                            if (!CanAttackTarget(ob)) continue;
                            targets.Add(ob);
                        }
                    }
                    break;
                case TargetType.HALFMOON:
                    targets = GetTargets(CurrentMap, Functions.Move(CurrentLocation, Direction), 0);
                    targets.AddRange(GetTargets(CurrentMap, Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, -1)), 0));
                    targets.AddRange(GetTargets(CurrentMap, Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 1)), 0));
                    targets.AddRange(GetTargets(CurrentMap, Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, 2)), 0));
                    break;
                case TargetType.FULLMOON:
                    targets = GetTargets(CurrentMap, CurrentLocation, 1);
                    break;
                case TargetType.ENEMY:
                    targets = GetTargets(CurrentMap, CurrentLocation, ViewRange);
                    break;
                case TargetType.FRIEND:
                    targets = GetAllObjects(CurrentLocation, ViewRange);
                    for (int i = targets.Count - 1; i >= 0; i--)
                    {
                        if (!CanHelpTarget(targets[i]))
                        {
                            targets.RemoveAt(i);
                        }
                    }

                    break;
                default:
                    break;
            }

            return targets;
        }
        /// <summary>
        /// 怪物说话
        /// </summary>
        /// <param name="SayStr"></param>
        public void Say(string SayStr)
        {
            if (SayStr == "") return;

            string _temname;
            // 只过滤结尾的数字
            _temname = MonsterInfo.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

            string text = string.Format("{0}: {1}", _temname, SayStr);

            foreach (PlayerObject eplayer in SeenByPlayers)
            {
                if (!Functions.InRange(CurrentLocation, eplayer.CurrentLocation, Config.MaxViewRange)) continue;

                eplayer.Connection.ReceiveChat(text, MessageType.Normal, ObjectID);

                foreach (SConnection observer in eplayer.Connection.Observers)
                {
                    observer.ReceiveChat(text, MessageType.Normal, ObjectID);
                }
            }
        }
    }
}
