using Library;
using Server.Envir;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class SabakGuardian : MonMove   //沙巴克守护者
    {
        public SabakGuardian()
        {
            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(ob, SEnvir.Random.Next(10, 50), element, ignoreShield, canCrit);
        }

        public void RangeAttack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            UpdateAttackTime();

            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400),
                               ActionType.DelayAttack,
                               Target,
                               GetDC(),
                               AttackElement));
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
