using Library;
using Server.Envir;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 卫士AI
    /// </summary>
    public sealed class Guard : MonsterObject
    {
        public override bool Blocking => true;
        public override bool CanMove => false;

        public Guard()
        {
            NameColour = Color.SkyBlue;
        }

        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0]);
                    return;
            }

            base.ProcessAction(action);
        }

        public override void ProcessSearch()
        {
            ProperSearch();
        }

        protected override bool InAttackRange()
        {
            return Target.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange);
        }
        public override void ProcessNameColour()
        {
            NameColour = Color.SkyBlue;
        }
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {

            if (!Functions.InRange(CurrentLocation, attacker.CurrentLocation, 10)) return 0;

            return base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit);
        }

        public override bool ShouldAttackTarget(MapObject ob)
        {
            return CanAttackTarget(ob);
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob">地图对象</param>
        /// <returns></returns>
        public override bool CanAttackTarget(MapObject ob)
        {
            //对象节点为空    对象死亡     对象不显示   对象是卫士   对象是沙巴克BOSS  对象是镜像  对象是宠物  不攻击
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord || ob is MirrorImage || ob is Companion) return false;

            switch (ob.Race)
            {
                //玩家红名攻击
                case ObjectType.Player:
                    return ob.Stats[Stat.PKPoint] >= Config.RedPoint && ob.Stats[Stat.Redemption] == 0 || (Target != null && ob == Target);

                //怪物攻击
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == null)
                        return !mob.Passive;

                    if (mob.PetOwner.Stats[Stat.PKPoint] >= Config.RedPoint && mob.PetOwner.Stats[Stat.Redemption] == 0)
                        return true;

                    return false;

                default:
                    return false;
            }
        }

        protected override void Attack()
        {
            if (!CanAttackTarget(Target))
            {
                Target = null;
                return;
            }

            Point targetBack = Functions.Move(Target.CurrentLocation, Target.Direction, -1);

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Target.Direction, Location = targetBack });
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(300),
                               ActionType.DelayAttack,
                               Target));
        }

        private void Attack(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead) return;

            int power;

            if (ob.Race == ObjectType.Monster)
            {
                MonsterObject mob = (MonsterObject)ob;
                mob.EXPOwner = null;
                power = ob.CurrentHP;
            }
            else
                power = GetDC();

            ob.Attacked(this, power, Element.None);
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()
        {
        }

    }
}
