﻿using Library;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class DepartedMonster : MonsterObject   //死去的怪物
    {
        public override void Die()
        {
            base.Die();

            List<Element> elements = new List<Element> { Element.None, Element.Fire, Element.Ice, Element.Lightning, Element.Wind };
            Effect effect = Effect.Puppet;
            Element element = elements[SEnvir.Random.Next(elements.Count)];

            switch (element)
            {
                case Element.Fire:
                    effect = Effect.PuppetFire;
                    break;
                case Element.Ice:
                    effect = Effect.PuppetIce;
                    break;
                case Element.Lightning:
                    effect = Effect.PuppetLightning;
                    break;
                case Element.Wind:
                    effect = Effect.PuppetWind;
                    break;
            }

            Broadcast(new S.ObjectEffect { Effect = effect, ObjectID = ObjectID });

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 2);

            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   element));
            }

            foreach (MapObject target in targets)
            {
                if (target.Dead || target.Race != ObjectType.Player) continue;

                SpawnMinions(4, 0, target);
            }
        }

        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap.Info, CurrentLocation);
        }
    }
}
