using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class GiantLizard : MonsterObject   //诺玛骑兵 石像火焰狮子 巨蜥 炎魔 黄铜黑耀武士 烈火神徒 法术神徒 火焰攻击
    {
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange = 7;
        /// <summary>
        /// 射程时间
        /// </summary>
        public DateTime RangeTime;
        /// <summary>
        /// 射程冷却时间
        /// </summary>
        public TimeSpan RangeCooldown;
        /// <summary>
        /// PVP射程开关
        /// </summary>
        public bool CanPvPRange = true;
        /// <summary>
        /// 在攻击范围内
        /// </summary>
        /// <returns></returns>
        protected override bool InAttackRange()
        {
            //目标当前地图不对
            if (Target.CurrentMap != CurrentMap) return false;
            //目标当前坐标等当前坐标
            if (Target.CurrentLocation == CurrentLocation) return false;

            if (SEnvir.Now > RangeTime && (CanPvPRange || Target.Race != ObjectType.Player))
                return Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
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
                Attack(ob, GetDC(), AttackElement);
                return;
            }

            Attack(ob, GetDC(), AttackElement);
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack)
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

            if (Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
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
                RangeTime = SEnvir.Now + RangeCooldown;

                Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   Target,
                                   false));
            }

        }
    }
}
