using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 青芭蕉AI
    /// </summary>
    public class BlueBlossom : MonsterObject
    {
        /// <summary>
        /// 在攻击范围内
        /// </summary>
        /// <returns></returns>
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, 8);  //目标在8格范围内
        }
        /// <summary>
        /// 应该攻击的目标
        /// </summary>
        /// <param name="ob">对象</param>
        /// <returns></returns>
        public override bool ShouldAttackTarget(MapObject ob)
        {
            return CanAttackTarget(ob);
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob">对象</param>
        /// <returns></returns>
        public override bool CanAttackTarget(MapObject ob)
        {
            return CanHelpTarget(ob);
        }
        /// <summary>
        /// 可以治疗的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanHelpTarget(MapObject ob)
        {
            return base.CanHelpTarget(ob) && ob.CurrentHP < ob.Stats[Stat.Health] && ob.Buffs.All(x => x.Type != BuffType.Heal);
        }
        /// <summary>
        /// 动作过程
        /// </summary>
        /// <param name="action">延迟动作</param>
        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Heal((MapObject)action.Data[0]);
                    return;
            }

            base.ProcessAction(action);
        }

        public override void ProcessSearch()
        {
            ProperSearch();
        }
        /// <summary>
        /// 治疗加血
        /// </summary>
        /// <param name="ob">对象</param>
        public void Heal(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead) return;

            ob.BuffAdd(BuffType.Heal, TimeSpan.MaxValue, new Stats { [Stat.Healing] = Stats[Stat.Healing], [Stat.HealingCap] = Stats[Stat.HealingCap] }, false, false, TimeSpan.FromSeconds(1));
        }
        /// <summary>
        /// 攻击
        /// </summary>
        protected override void Attack()
        {
            //从点开始的方向 目标当前坐标
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400 + Functions.Distance(CurrentLocation, Target.CurrentLocation) * Config.GlobalsProjectileSpeed),
                               ActionType.DelayAttack,
                               Target));
        }
    }
}
