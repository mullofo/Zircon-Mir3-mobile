using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NormaPaladin : MonsterObject   //诺玛圣骑士
    {
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange = 7;
        /// <summary>
        /// 射程时间
        /// </summary>
        public DateTime RangeTime;
        /// <summary>
        /// 射程冷却时间
        /// </summary>
        public TimeSpan RangeCooldown;
        /// <summary>
        /// PVP射程开关
        /// </summary>
        public bool CanPvPRange = true;

        public Stat Affinity;

        public NormaPaladin()
        {
            switch (SEnvir.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Affinity = Stat.FireAffinity;
                    break;
                case DayOfWeek.Tuesday:
                    Affinity = Stat.IceAffinity;
                    break;
                case DayOfWeek.Wednesday:
                    Affinity = Stat.LightningAffinity;
                    break;
                case DayOfWeek.Thursday:
                    Affinity = Stat.WindAffinity;
                    break;
                case DayOfWeek.Friday:
                    Affinity = Stat.HolyAffinity;
                    break;
                case DayOfWeek.Saturday:
                    Affinity = Stat.DarkAffinity;
                    break;
                case DayOfWeek.Sunday:
                    Affinity = Stat.PhantomAffinity;
                    break;
            }
        }

        public override void ApplyBonusStats()
        {
            base.ApplyBonusStats();

            Stats[Affinity] = 1;
        }

        /// <summary>
        /// 在攻击范围内
        /// </summary>
        /// <returns></returns>
        protected override bool InAttackRange()
        {
            //目标当前地图不对
            if (Target.CurrentMap != CurrentMap) return false;
            //目标当前坐标等当前坐标
            if (Target.CurrentLocation == CurrentLocation) return false;

            if (SEnvir.Now > RangeTime && (CanPvPRange || Target.Race != ObjectType.Player))
                return Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0], (bool)action.Data[1]);
                    return;
            }

            base.ProcessAction(action);
        }

        public void Attack(MapObject ob, bool melee)
        {
            if (melee)
            {
                Attack(ob, GetDC(), AttackElement);
                return;
            }

            Attack(ob, GetDC(), AttackElement);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (CanAttack && SEnvir.Random.Next(5) == 0)
            {
                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 5);

                if (targets.Count > 0)
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, targets[0].CurrentLocation);

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    foreach (MapObject target in targets)
                    {
                        if (target.Race == ObjectType.Player && target.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;

                        target.BuffAdd(BuffType.PierceBuff, TimeSpan.FromSeconds(5), null, false, false, TimeSpan.Zero);
                        if (target.Race == ObjectType.Player)
                            (target as PlayerObject).Connection.ReceiveChat($"你的强元素降低为0，持续时间5秒", MessageType.System);
                    }
                }
            }

            if (InAttackRange() && CanAttack)
                Attack();

            if (CurrentLocation == Target.CurrentLocation)
            {
                MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) break;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
            }
            else
                MoveTo(Target.CurrentLocation);

            if (InAttackRange() && CanAttack)
                Attack();
        }


        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
            {
                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   true));
            }
            else
            {
                RangeTime = SEnvir.Now + RangeCooldown;

                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   false));
            }

        }
    }
}
