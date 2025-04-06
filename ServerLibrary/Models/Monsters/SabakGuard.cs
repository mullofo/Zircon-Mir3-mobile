using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class SabakGuard : MonMove  //沙巴克守卫 
    {
        public DateTime FearTime;
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange = 5;
        /// <summary>
        /// 退缩几率
        /// </summary>
        public int FearRate = 6;
        /// <summary>
        /// 退缩持续时间
        /// </summary>
        public int FearDuration = 2;

        public SabakGuard()
        {
            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!CanAttack || SEnvir.Now < FearTime) return;

            if (SEnvir.Now > RoamTime) return;
            Attack();
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(ob, SEnvir.Random.Next(50, 60), element, ignoreShield, canCrit);
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            if (SEnvir.Random.Next(FearRate) == 0)
                FearTime = SEnvir.Now.AddSeconds(FearDuration + SEnvir.Random.Next(4));

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Config.GlobalsProjectileSpeed),
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
