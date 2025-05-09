﻿using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class EscortCommander : MonsterObject   //卫护首将
    {
        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(2) == 0)
                        RangeAttack();
                }


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
            }

            if (!CanAttack) return;

            if (SEnvir.Random.Next(2) == 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public void RangeAttack()
        {
            if (Functions.InRange(Target.CurrentLocation, CurrentLocation, 2) && SEnvir.Random.Next(2) == 0)
                MonsterThunderStorm(GetDC());
            else if (Functions.InRange(Target.CurrentLocation, CurrentLocation, Globals.MagicRange))
                AttackMagic(MagicType.ThunderBolt, Element.Lightning, false);
        }
    }
}
