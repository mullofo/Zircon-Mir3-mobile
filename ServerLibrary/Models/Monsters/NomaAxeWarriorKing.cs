using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class NomaAxeWarriorKing : MonsterObject  //诺玛斧兵王
    {
        public int Range = 3;
        public DateTime DebuffTime;

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > Range || y > Range) return false;


            return x == 0 || x == y || y == 0;
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
                base.Attack(ob, GetDC(), AttackElement);
                return;
            }


            base.Attack(ob, GetDC(), AttackElement);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack) //random 4 > 0
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

            if (CanAttack && SEnvir.Now > DebuffTime)
            {
                DebuffTime = SEnvir.Now.AddSeconds(10);

                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 3);

                if (targets.Count > 0)
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, targets[0].CurrentLocation);

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    foreach (MapObject target in targets)
                    {
                        if (target.Race == ObjectType.Player && target.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;

                        target.BuffAdd(BuffType.BurnBuff, TimeSpan.FromSeconds(5), null, false, false, TimeSpan.Zero);
                        if (target.Race == ObjectType.Player)
                            (target as PlayerObject).Connection.ReceiveChat($"你的攻击降低一半，持续时间5秒", MessageType.System);
                    }
                }
            }
        }

        private void LineAttack(int distance)
        {
            for (int i = 1; i <= distance; i++)
            {
                Point target = Functions.Move(CurrentLocation, Direction, i);

                if (target == Target.CurrentLocation)
                {
                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(400),
                        ActionType.DelayAttack,
                        Target,
                        false));
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
                            false));

                        break;
                    }
                }


            }
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1) && SEnvir.Random.Next(4) > 0)
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
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                LineAttack(Range);
            }

        }
    }
}
