using Library;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 栏栅AI
    /// </summary>
    public class Fence : MonsterObject
    {
        /// <summary>
        /// 不能移动
        /// </summary>
        public override bool CanMove => false;
        public override bool CanAttack => false;
        public MirDirection CurrentDir = MirDirection.Up;
        /// <summary>
        /// 怪物刷新朝向
        /// </summary>
        public Fence()
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
        public override bool ApplyPoison(Poison p)
        {
            return false;
        }
    }
}
