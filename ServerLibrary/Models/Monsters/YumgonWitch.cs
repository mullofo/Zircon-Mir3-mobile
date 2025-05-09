﻿using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class YumgonWitch : MonsterObject  //蚩尤将军 魔女
    {
        public int AttackRange = 10;
        public virtual Element AoEElement { get; set; }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
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

            if (SEnvir.Random.Next(5) > 0 || !Functions.InRange(Target.CurrentLocation, CurrentLocation, 3))
            {
                Broadcast(new Library.Network.ServerPackets.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(400),
                    ActionType.DelayAttack,
                    Target,
                    GetDC(),
                    AttackElement));
            }
            else
            {
                Broadcast(new Library.Network.ServerPackets.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, 3))
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        target,
                        GetDC(),
                        AoEElement));
                }
            }

        }
    }
}
