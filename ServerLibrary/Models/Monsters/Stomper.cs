using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class Stomper : MonsterObject         //海神将领
    {
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
