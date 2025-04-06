using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CrimsonNecromancer : MonsterObject   //红衣法师
    {
        public DateTime DebuffTime;

        public override void ProcessTarget()
        {
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

                        target.BuffAdd(BuffType.MagicWeakness, TimeSpan.FromSeconds(10), null, false, false, TimeSpan.Zero);
                        if (target.Race == ObjectType.Player)
                            (target as PlayerObject).Connection.ReceiveChat($"你的魔御降低为0，持续时间10秒", MessageType.System);
                    }
                }
            }

            base.ProcessTarget();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new Library.Network.ServerPackets.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();

            foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, 3))  //遍历3格范围目标攻击
            {
                ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   target,
                                   GetDC(),
                                   AttackElement));
            }
        }
    }
}
