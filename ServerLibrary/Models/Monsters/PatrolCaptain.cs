using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class PatrolCaptain : MonsterObject  //巡逻队长
    {
        public int Range = 3;
        public DateTime TingTime;
        public DateTime CastTime;
        public TimeSpan CastDelay = TimeSpan.FromSeconds(15);
        public int RangeChance = 5;

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 2);
        }

        public override void ProcessAI()
        {
            base.ProcessAI();

            if (Dead) return;

            if (SEnvir.Now > TingTime)
            {
                TingTime = SEnvir.Now.AddMinutes(2);

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange))
                {
                    ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, Config.MaxViewRange));

                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.WraithGrip,
                        TickFrequency = TimeSpan.FromSeconds(5),
                        TickCount = 1,
                    });
                }
            }
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(ob, SEnvir.Random.Next(10, 60), element, ignoreShield, canCrit);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (CanAttack && SEnvir.Now > CastTime)
            {
                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, ViewRange);

                if (targets.Count > 0)
                {
                    foreach (MapObject ob in targets)
                    {
                        if (CurrentHP > Stats[Stat.Health] / 2 && SEnvir.Random.Next(2) > 0) continue;

                        DeathCloud(ob.CurrentLocation);
                    }

                    UpdateAttackTime();
                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.None, Targets = new List<uint> { Target.ObjectID } });
                    CastTime = SEnvir.Now + CastDelay;
                }
            }

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(RangeChance) == 0)
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

            if (SEnvir.Random.Next(RangeChance) > 0)
            {
                if (InAttackRange() && CanAttack)
                    Attack();
            }
            else RangeAttack();
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
                        GetDC(),
                        AttackElement,
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
                            GetDC(),
                            AttackElement,
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
                                   GetDC(),
                                   AttackElement,
                                   true));
            }
            else
            {
                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                LineAttack(Range);
            }
        }

        public virtual void RangeAttack()
        {
            switch (SEnvir.Random.Next(5))
            {
                case 0:
                    AttackAoE(2, MagicType.FireStorm, Element.Fire);
                    break;
                case 1:
                    AttackAoE(3, MagicType.SamaGuardianFire, Element.Fire);
                    break;
            }
        }

        public override bool ApplyPoison(Poison p)
        {
            return false;
        }

        public override void ProcessRegen()
        {
        }
    }
}
