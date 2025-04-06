using Server.Envir;
using System.Collections.Generic;

namespace Server.Models.Monsters
{
    public class Larva : MonsterObject   //爆裂蜘蛛
    {

        public int Range = 1;  //范围设置 1
        public override void Process()   //过程
        {
            base.Process();  //基本过程

            if (Dead) return;        //如果死亡 返回

            if (Target == null)      //如果 目标 是 空
            {
                SetHP(0);   //设置  HP 为 0
                return;    //返回
            }
        }

        protected override void Attack()   //攻击
        {
            SetHP(0);  //设置  HP 为 0
        }

        public override void Die()   //死亡
        {
            base.Die();   //基本死亡

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, Range);  //列出目标 = 获取目标（当前地图 当前位置 范围）
            foreach (MapObject target in targets)         //在目标中映射对象目标
            {
                ActionList.Add(new DelayedAction(                           //动作列表添加（新的延迟动作
                                   SEnvir.Now.AddMilliseconds(800),         //800毫秒
                                   ActionType.DelayAttack,                  //动作攻击类型  延迟攻击
                                   target,                                  //目标
                                   GetDC(),                                 //获取DC值
                                   AttackElement));                         //攻击单元
            }
        }
    }
}
