using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class CommanderNoma : MonsterObject     //诺玛统领
    {
        public Stat Affinity;
        public DateTime TeleportTime, TingTime;
        public DateTime DebuffTime;
        public DateTime QibuffTime;
        public DateTime CastTime;
        public TimeSpan CastDelay = TimeSpan.FromSeconds(15);

        public CommanderNoma()
        {
            switch (SEnvir.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    Affinity = Stat.FireAffinity;
                    break;
                case DayOfWeek.Tuesday:
                    Affinity = Stat.IceAffinity;
                    break;
                case DayOfWeek.Wednesday:
                    Affinity = Stat.LightningAffinity;
                    break;
                case DayOfWeek.Thursday:
                    Affinity = Stat.WindAffinity;
                    break;
                case DayOfWeek.Friday:
                    Affinity = Stat.HolyAffinity;
                    break;
                case DayOfWeek.Saturday:
                    Affinity = Stat.DarkAffinity;
                    break;
                case DayOfWeek.Sunday:
                    Affinity = Stat.PhantomAffinity;
                    break;
            }
        }

        public override void ApplyBonusStats()
        {
            base.ApplyBonusStats();

            Stats[Affinity] = 1;
        }

        public override void ProcessAI()
        {
            base.ProcessAI();

            if (Dead) return;

            if (SEnvir.Now > TingTime)
            {
                TingTime = SEnvir.Now.AddMinutes(1);

                foreach (MapObject ob in GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange))
                {
                    //ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, Config.MaxViewRange));

                    if (ob.Race == ObjectType.Player && ob.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;

                    if (SEnvir.Random.Next(5) == 0)
                    {
                        ob.ApplyPoison(new Poison
                        {
                            Owner = this,
                            Type = PoisonType.HellFire,
                            Value = GetMC() / 4,
                            TickFrequency = TimeSpan.FromSeconds(3),
                            TickCount = 2,
                        });
                    }

                    if (SEnvir.Random.Next(4) == 0)
                    {
                        ob.ApplyPoison(new Poison
                        {
                            Owner = this,
                            Type = PoisonType.StunnedStrike,
                            TickFrequency = TimeSpan.FromSeconds(2),
                            TickCount = 1,
                        });
                    }

                    if (SEnvir.Random.Next(3) == 0)
                    {
                        ob.ApplyPoison(new Poison
                        {
                            Owner = this,
                            Type = PoisonType.ElectricShock,
                            Value = GetMC() / 2,
                            TickFrequency = TimeSpan.FromSeconds(2),
                            TickCount = 1,
                        });
                    }
                }
            }
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 3) && SEnvir.Now > TeleportTime)
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

                    TeleportTime = SEnvir.Now.AddSeconds(5);
                }
            }

            if (Target.Race == ObjectType.Monster)
            {
                Target.SetHP(0);
                return;
            }

            if (CanAttack && SEnvir.Now > DebuffTime)
            {
                DebuffTime = SEnvir.Now.AddSeconds(5);

                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 3);

                if (targets.Count > 0 && SEnvir.Random.Next(3) == 0)
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, targets[0].CurrentLocation);

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    foreach (MapObject target in targets)
                    {
                        if (target.Race == ObjectType.Player && target.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;

                        target.BuffAdd(BuffType.BurnBuff, TimeSpan.FromSeconds(5), null, false, false, TimeSpan.Zero);
                        if (target.Race == ObjectType.Player)
                            (target as PlayerObject).Connection.ReceiveChat($"你的攻击降低一半，持续时间5秒", MessageType.System);
                    }
                }
            }

            if (CanAttack && SEnvir.Now > QibuffTime)
            {
                QibuffTime = SEnvir.Now.AddSeconds(5);

                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 3);

                if (targets.Count > 0 && SEnvir.Random.Next(3) == 0)
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, targets[0].CurrentLocation);

                    Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                    UpdateAttackTime();

                    foreach (MapObject target in targets)
                    {
                        if (target.Race == ObjectType.Player && target.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;

                        target.BuffAdd(BuffType.PierceBuff, TimeSpan.FromSeconds(5), null, false, false, TimeSpan.Zero);
                        if (target.Race == ObjectType.Player)
                            (target as PlayerObject).Connection.ReceiveChat($"你的强元素降低为0，持续时间5秒", MessageType.System);
                    }
                }
            }

            if (CanAttack && SEnvir.Now > CastTime)
            {
                List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, ViewRange);

                if (targets.Count > 0)
                {
                    foreach (MapObject ob in targets)
                    {
                        if (CurrentHP > Stats[Stat.Health] / 2 && SEnvir.Random.Next(2) > 0) continue;

                        SamaGuardianFire();
                    }

                    UpdateAttackTime();
                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.None, Targets = new List<uint> { Target.ObjectID } });
                    CastTime = SEnvir.Now + CastDelay;
                }
            }

            base.ProcessTarget();
        }
    }
}
