using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NormaRiprapKing : MonsterObject   //诺玛抛石王
    {
        public DateTime FearTime;
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange = 8;
        /// <summary>
        /// 退缩几率
        /// </summary>
        public int FearRate = 6;
        /// <summary>
        /// 退缩持续时间
        /// </summary>
        public int FearDuration = 2;

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            MirDirection direction;
            int rotation;
            if (!InAttackRange())
            {
                if (CurrentLocation == Target.CurrentLocation)
                {
                    direction = (MirDirection)SEnvir.Random.Next(8);
                    rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                    MoveTo(Target.CurrentLocation);

                return;
            }

            if (Functions.InRange(Target.CurrentLocation, CurrentLocation, AttackRange - 1))
            {
                direction = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) break;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
            }
            if (!CanAttack || SEnvir.Now < FearTime) return;

            Attack();
        }

        protected override void Attack()
        {
            if (SEnvir.Random.Next(10) == 0)
            {
                Target.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.HellFire,
                    Value = GetMC() / 4,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    TickCount = 2,
                });
            }
            else
            {
                //从点开始的方向 目标当前坐标
                Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                UpdateAttackTime();

                if (SEnvir.Random.Next(FearRate) == 0)
                    FearTime = SEnvir.Now.AddSeconds(FearDuration + SEnvir.Random.Next(4));

                foreach (MapObject target in GetTargets(CurrentMap, Target.CurrentLocation, 4))
                {
                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Config.GlobalsProjectileSpeed),
                                       ActionType.DelayAttack,
                                       target,
                                       GetDC(),
                                       AttackElement));
                }
            }
        }
    }
}
