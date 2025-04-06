using Library;

namespace Server.Models.Monsters
{
    public class Changer : MonsterObject   //可移动 不反击 去血1点
    {
        public override bool CanAttack => false;

        public override bool CanAttackTarget(MapObject ob)
        {
            return false;
        }

        public override int Attacked(MapObject ob, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return base.Attacked(ob, 1, element, ignoreShield, canCrit);
        }

        public override bool ApplyPoison(Poison p)
        {
            return false;
        }
    }
}
