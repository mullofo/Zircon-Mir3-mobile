using Library;
using Server.Envir;
using System;
using System.Collections.Generic;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 霸王教主AI
    /// </summary>
    public class PachontheChaosbringer : MonsterObject
    {
        public DateTime CastTime;
        /// <summary>
        /// 不可攻击的范围
        /// </summary>
        /// <returns></returns>
        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 2);
        }
        /// <summary>
        /// 攻击目标过程
        /// </summary>
        public override void ProcessTarget()
        {
            if (Target == null) return;

            /*if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 8) && SEnvir.Now > CastTime)
            {
                MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                Cell cell = null;
                for (int i = 0; i < 8; i++)
                {
                    cell = CurrentMap.GetCell(Functions.Move(Target.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                    if (cell == null || cell.Movements != null)
                    {
                        cell = null;
                        continue;
                    }
                    break;
                }

                if (cell != null)
                {
                    Direction = Functions.DirectionFromPoint(cell.Location, Target.CurrentLocation);
                    Teleport(CurrentMap, cell.Location);

                    CastTime = SEnvir.Now.AddSeconds(10);
                }
            }
            else */

            if (InAttackRange() && SEnvir.Now > CastTime)
            {
                CastTime = SEnvir.Now.AddSeconds(20);

                List<MapObject> allTargerts = GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange);

                DragonRepulse(allTargerts);
            }

            base.ProcessTarget();
        }
    }
}
