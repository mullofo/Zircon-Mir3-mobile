﻿using Server.Envir;
using System.Collections.Generic;

namespace Server.Models.Monsters
{
    public class Affliction : MonsterObject  //折磨
    {
        protected override void Attack()
        {
            SetHP(0);
        }

        public override void Die()
        {
            base.Die();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 1);
            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(800),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   AttackElement));
            }
        }
    }
}
