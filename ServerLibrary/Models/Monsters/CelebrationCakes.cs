using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class CelebrationCakes : MonsterObject   //庆典蛋糕
    {
        public override bool CanMove => false;
        public override bool CanAttack => false;

        public CelebrationCakes()
        {
            Direction = MirDirection.Up;
            Passive = true;
        }

        public override void ProcessRoam()
        {
        }
        public override void ProcessSearch()
        {
        }
        public override bool CanAttackTarget(MapObject ob)
        {
            return false;
        }
        public override void ProcessTarget()
        {
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(ob, SEnvir.Random.Next(1, 10), element, ignoreShield, canCrit);
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
