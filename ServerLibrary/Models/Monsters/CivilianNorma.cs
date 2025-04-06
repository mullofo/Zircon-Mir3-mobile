using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class CivilianNorma : MonsterObject   //平民诺玛
    {
        public override bool CanAttack => false;

        public override void ProcessTarget()
        {
            if (Target == null) return;

            MirDirection direction;
            int rotation;

            double hpLostPercent = (1d * Stats[Stat.Health] - CurrentHP) / Stats[Stat.Health] * 100d;

            if (hpLostPercent > 60)  //血量低于40%时 逃跑 往目标相反的方向随机移动
            {
                if (Functions.InRange(Target.CurrentLocation, CurrentLocation, 8))
                {
                    direction = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                    rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) break;

                        direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
            }
            else
            {
                if (!InAttackRange())
                {
                    if (SEnvir.Random.Next(5) == 0)
                    {
                        if (CurrentLocation == Target.CurrentLocation)
                        {
                            direction = (MirDirection)SEnvir.Random.Next(8);
                            rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int d = 0; d < 8; d++)
                            {
                                if (Walk(direction)) break;

                                direction = Functions.ShiftDirection(direction, rotation);
                            }
                        }
                        else
                            MoveTo(Target.CurrentLocation);
                    }
                    return;
                }
            }
        }
    }
}
