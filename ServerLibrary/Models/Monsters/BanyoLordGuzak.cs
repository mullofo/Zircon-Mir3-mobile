using Library;
using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class BanyoLordGuzak : MonsterObject   //海盗天马
    {
        public DateTime SlaveTime, PuriTime, CastTime;

        public override void Process()
        {
            base.Process();

            if (Dead) return;

            if (SEnvir.Now > PuriTime)
            {
                PuriTime = SEnvir.Now.AddSeconds(20);

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange))
                    Purify(ob);
            }

            if (SEnvir.Now >= SlaveTime)
            {
                SlaveTime = SEnvir.Now.AddSeconds(45);

                for (int i = MinionList.Count - 1; i >= 0; i--)
                {
                    MonsterObject mob = MinionList[i];
                    if (mob.CurrentMap == CurrentMap && Functions.InRange(CurrentLocation, mob.CurrentLocation, 10)) continue;

                    mob.EXPOwner = null;
                    mob.Die();
                    mob.Despawn();
                }

                SpawnMinions(5 - MinionList.Count, 0, Target);
            }
        }
        public override bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 10));
        }

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

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 8) && SEnvir.Now > CastTime)
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
            else if (InAttackRange() && SEnvir.Now > CastTime)
            {
                CastTime = SEnvir.Now.AddSeconds(20);

                DragonRepulse();
            }

            base.ProcessTarget();
        }
    }
}
