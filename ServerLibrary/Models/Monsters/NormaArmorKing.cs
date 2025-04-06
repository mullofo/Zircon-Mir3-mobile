using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NormaArmorKing : MonsterObject      //诺玛装甲王
    {
        public Stat Affinity;

        public NormaArmorKing()
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
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 2);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack && SEnvir.Random.Next(2) == 0)
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

            if (SEnvir.Random.Next(10) == 0)
            {
                Target.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.StunnedStrike,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 1,
                });
            }
            else
            {
                if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
                {
                    Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       Target,
                                       GetDC(),
                                       AttackElement));
                }
                else
                {
                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                    ActionList.Add(new DelayedAction(
                                       SEnvir.Now.AddMilliseconds(400),
                                       ActionType.DelayAttack,
                                       Target,
                                       GetDC(),
                                       AttackElement));
                }
            }
        }
    }
}
