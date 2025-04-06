﻿using Library;
using Server.Envir;
using System;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class SpittingSpider : MonsterObject   //喷毒攻击  毒蜘蛛 腐蚀人鬼 骷髅士兵 紫红女神等
    {

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 2 || y > 2) return false;


            return x == 0 || x == y || y == 0;
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation }); //Animation ?

            UpdateAttackTime();

            LineAttack(2);
        }

        protected void LineAttack(int distance)
        {
            LineAttack(distance, Direction);
        }
        protected void LineAttack(int distance, MirDirection direction)
        {
            for (int i = 1; i <= distance; i++)
            {
                Point target = Functions.Move(CurrentLocation, direction, i);

                if (target == Target.CurrentLocation)
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        Target,
                        GetDC(),
                        AttackElement));
                }
                else
                {
                    Cell cell = CurrentMap.GetCell(target);
                    if (cell?.Objects == null) continue;

                    foreach (MapObject ob in cell.Objects)
                    {
                        if (!CanAttackTarget(ob)) continue;

                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(400),
                            ActionType.DelayAttack,
                            ob,
                            GetDC(),
                            AttackElement));

                        break;
                    }
                }
            }
        }
    }
}
