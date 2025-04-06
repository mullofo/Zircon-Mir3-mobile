using Library;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CrawlerSlave : DuelHitMonster
    {

        protected override void Attack2()
        {
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.None, AttackElement = Element.None });

            foreach (MapObject ob in GetTargets(CurrentMap, Functions.Move(CurrentLocation, Direction), 1))
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(400),
                    ActionType.DelayAttack,
                    ob,
                    GetDC(),
                    AttackElement));

                ob.ApplyPoison(new Poison
                {
                    Value = GetMC(),
                    Type = PoisonType.Green,
                    Owner = this,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 10,
                });
            }
        }
    }
}
