using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 战士技能
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 野蛮冲撞结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <returns></returns>
        public int ShoulderDashEnd(UserMagic magic)
        {
            int distance = magic.GetPower();

            int travelled = 0;
            Cell cell;
            MapObject target = null;

            for (int d = 1; d <= distance; d++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d));

                if (cell == null) break;

                if (cell.Objects == null)
                {
                    travelled++;
                    continue;
                }
                if (target == null && d != 1) break;

                bool blocked = false;
                bool stacked = false;
                MapObject stackedMob = null;

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= (Level + Stats[Stat.Repulsion]) || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)//怪物推走一格 如果不为1 下一格有怪物
                    {
                        if (target == null) target = ob;

                        //LevelMagic(magic);
                        CheckBrown(ob);
                        continue;
                    }
                    //下一格有怪物则标记有重叠 并记录当前的怪物
                    stacked = true;
                    stackedMob = ob;
                }
                //如果被阻止则不推
                if (blocked) break;
                //没有重叠 则玩家可以多推一格
                if (!stacked)
                {
                    travelled++;
                    continue;
                }

                if (magic.Level < 3) break; //小于3级 不能推两个

                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d + 1));

                if (cell == null) break; //因为有墙或推不动，所以不能再推了

                //由于堆叠，第一个推不动，而且不是墙，所以这个里边一定有叠加
                if (cell.Objects != null) //可能是有人撞进去的
                    for (int c = cell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= (Level + Stats[Stat.Repulsion]) || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                        {
                            blocked = true;
                            break;
                        }

                        if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                        {
                            blocked = true;
                            continue;
                        }

                        if (ob.Pushed(Direction, 1) == 1)
                        {
                            //LevelMagic(magic);
                            CheckBrown(ob);
                            continue;
                        }

                        blocked = true;
                        break;
                    }

                if (blocked) break; //无法推动两个目标（水平或墙壁）

                //推到第二个空间，现在需要推第一个暴徒
                //推送stackedMob应该是100%成功的，因为它不是水平的，也没有墙或暴徒阻挡。
                stackedMob.Pushed(Direction, 1); //把这个放在这里以避免水平/机会检查
                //LevelMagic(magic);
                //需要再次检查第一个
                Point location = Functions.Move(CurrentLocation, Direction, d);
                cell = CurrentMap.Cells[location.X, location.Y];

                if (cell.Objects == null) //移动后可能不会有更多的暴徒出现在初始空间
                {
                    travelled++;
                    continue;
                }

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= (Level + Stats[Stat.Repulsion]) || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        //LevelMagic(magic);
                        CheckBrown(ob);
                        continue;
                    }

                    blocked = true;
                    break;
                }

                if (blocked) break;

                travelled++;
            }

            MagicType type = magic.Info.Magic;
            if (travelled > 0 && target != null)
            {
                UserMagic assault;

                if (Magics.TryGetValue(MagicType.Assault, out assault) && Level >= assault.Info.NeedLevel1 && SEnvir.Now >= assault.Cooldown)
                {
                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Paralysis,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower()),
                        Owner = this,
                    });

                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Silenced,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower() * 2),
                        Owner = this,
                    });

                    assault.Cooldown = SEnvir.Now.AddMilliseconds(assault.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = assault.Info.Index, Delay = assault.Info.Delay });
                    type = assault.Info.Magic;
                    LevelMagic(assault);
                }
            }

            cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, travelled));

            //野蛮过图？？
            //CurrentCell = cell.GetMovement(this);
            CurrentCell = cell;
            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectDash { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Distance = travelled, Magic = type });

            ActionTime = SEnvir.Now.AddMilliseconds(300 * travelled);

            LevelMagic(magic);

            return travelled;
        }
        /// <summary>
        /// 君临步结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <returns></returns>
        public int ReigningStepEnd(UserMagic magic)
        {
            int distance = magic.GetPower();

            int travelled = 0;
            Cell cell;
            MapObject target = null;

            for (int d = 1; d <= distance; d++)
            {
                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d));

                if (cell == null) break;

                if (cell.Objects == null)
                {
                    travelled++;
                    continue;
                }

                bool blocked = false;
                bool stacked = false;
                MapObject stackedMob = null;

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        if (target == null) target = ob;

                        LevelMagic(magic);
                        continue;
                    }

                    stacked = true;
                    stackedMob = ob;
                }

                if (blocked) break;

                if (!stacked)
                {
                    travelled++;
                    continue;
                }

                if (magic.Level < 3) break; // Cannot push 2 mobs

                cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, d + 1));

                if (cell == null) break; // Cannot push anymore as there is a wall or couldn't push

                //Failed to push first mob because of stacking AND its not a wall so must be mob in this cell
                if (cell.Objects != null) // Could have dashed someone through door.
                    for (int c = cell.Objects.Count - 1; c >= 0; c--)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;

                        if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                        {
                            blocked = true;
                            break;
                        }

                        if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                        {
                            blocked = true;
                            continue;
                        }

                        if (ob.Pushed(Direction, 1) == 1)
                        {
                            LevelMagic(magic);
                            continue;
                        }

                        blocked = true;
                        break;
                    }

                if (blocked) break; // Cannot push the two targets (either by level or wall)

                //pushed 2nd space, Now need to push the first mob
                //Should be 100% success to push stackedMob as it wasn't level nor is there a wall or mob in the way.
                stackedMob.Pushed(Direction, 1); //put this here to avoid the level / chance check
                LevelMagic(magic);
                //need to check first cell again
                Point location = Functions.Move(CurrentLocation, Direction, d);
                cell = CurrentMap.Cells[location.X, location.Y];

                if (cell.Objects == null) //Might not be any more mobs on initial space after moving it
                {
                    travelled++;
                    continue;
                }

                for (int c = cell.Objects.Count - 1; c >= 0; c--)
                {
                    MapObject ob = cell.Objects[c];
                    if (!ob.Blocking) continue;

                    if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= Level || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level || ob.Buffs.Any(x => x.Type == BuffType.Endurance))
                    {
                        blocked = true;
                        break;
                    }

                    if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.CanPush)
                    {
                        blocked = true;
                        continue;
                    }

                    if (ob.Pushed(Direction, 1) == 1)
                    {
                        LevelMagic(magic);
                        continue;
                    }

                    blocked = true;
                    break;
                }

                if (blocked) break;

                travelled++;
            }

            MagicType type = magic.Info.Magic;
            if (travelled > 0 && target != null)
            {
                UserMagic assault;

                if (Magics.TryGetValue(MagicType.ReigningStep, out assault) && Level >= assault.Info.NeedLevel1 && SEnvir.Now >= assault.Cooldown)
                {
                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Paralysis,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower()),
                        Owner = this,
                    });

                    target.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Silenced,
                        TickCount = 1,
                        TickFrequency = TimeSpan.FromMilliseconds(travelled * 300 + assault.GetPower() * 2),
                        Owner = this,
                    });

                    assault.Cooldown = SEnvir.Now.AddMilliseconds(assault.Info.Delay);
                    Enqueue(new S.MagicCooldown { InfoIndex = assault.Info.Index, Delay = assault.Info.Delay });
                    type = assault.Info.Magic;
                    LevelMagic(assault);
                }
            }

            cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction, travelled));

            CurrentCell = cell.GetMovement(this);

            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectDash { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Distance = travelled, Magic = type });

            ActionTime = SEnvir.Now.AddMilliseconds(300 * travelled);

            return travelled;
        }
        /// <summary>
        /// 乾坤大挪移
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void InterchangeEnd(UserMagic magic, MapObject ob)
        {
            if (!Config.QKDNY)
            {
                if (CurrentMap.Info.SkillDelay > 0)
                {
                    Connection.ReceiveChat("Skills.SkillBadMap".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Skills.SkillBadMap".Lang(con.Language, magic.Info.Name), MessageType.System);
                    return;
                }
            }

            if (ob == null || ob.CurrentMap != CurrentMap) return;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= Config.MaxLevel) return;
                    if (ob.Level >= (Level + Stats[Stat.Repulsion]) || ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return;
                    break;
                case ObjectType.Monster:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= (Level + Stats[Stat.Repulsion]) || !((MonsterObject)ob).MonsterInfo.CanPush) return;
                    break;
                case ObjectType.Item:
                    break;
                default:
                    return;
            }

            if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;

            Point current = CurrentLocation;

            /*  if (CurrentMap.Info.SkillDelay > 0) return;
              {
                  TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                  Connection.ReceiveChat("".Lang(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat("".Lang(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                  UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                  Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
              }*/

            Teleport(CurrentMap, ob.CurrentLocation);
            ob.Teleport(CurrentMap, current);

            if (ob.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)ob).PvPTime = SEnvir.Now;
            }

            int delay = magic.Info.Delay;
            //if (SEnvir.Now <= PvPTime.AddSeconds(30))
            //delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            CheckBrown(ob);

            LevelMagic(magic);
        }
        /// <summary>
        /// 斗转星移
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象/param>
        public void BeckonEnd(UserMagic magic, MapObject ob)
        {
            if (ob == null || ob.CurrentMap != CurrentMap) return;

            if (CurrentMap.Info.SkillDelay > 0)
            {
                Connection.ReceiveChat("Skills.SkillBadMap".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.SkillBadMap".Lang(con.Language, magic.Info.Name), MessageType.System);
                return;
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (!CanAttackTarget(ob)) return;
                    if (ob.Level >= Config.MaxLevel) return;
                    if (ob.Level >= (Level + Stats[Stat.Repulsion]) || ob.Buffs.Any(x => x.Type == BuffType.Endurance)) return;

                    /* if (CurrentMap.Info.SkillDelay > 0)
                     {
                         Connection.ReceiveChat("".Lang(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                         foreach (SConnection con in Connection.Observers)
                             con.ReceiveChat("".Lang(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                         return;
                     }*/

                    if (SEnvir.Random.Next(10) > 4 + magic.Level) return;
                    break;
                case ObjectType.Monster:
                    if (!CanAttackTarget(ob) || ob.Level >= (Level + Stats[Stat.Repulsion])) return;

                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss || !mob.MonsterInfo.CanPush) return;

                    if (SEnvir.Random.Next(10) > 3 + magic.Level * 2) return;
                    break;
                case ObjectType.Item:
                    if (SEnvir.Random.Next(10) > 3 + magic.Level * 2) return;
                    break;
                default:
                    return;
            }

            if (!ob.Teleport(CurrentMap, Functions.Move(CurrentLocation, Direction))) return;

            /*   if (CurrentMap.Info.SkillDelay > 0)
               {
                   TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                   Connection.ReceiveChat("".Lang(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                   foreach (SConnection con in Connection.Observers)
                       con.ReceiveChat("".Lang(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                   UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                   Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
               }*/

            if (ob.Race != ObjectType.Item && Config.BeckonParalysis)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(ob.Race == ObjectType.Monster ? (1 + magic.Level) : 1),
                    TickCount = 1,
                });
            }

            if (ob.Race != ObjectType.Item)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Slow,
                    Value = 4,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 1,
                });
            }

            int delay = magic.Info.Delay;
            //if (SEnvir.Now <= PvPTime.AddSeconds(30))
            //delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            LevelMagic(magic);
        }
        /// <summary>
        /// 挑衅结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void MassBeckonEnd(UserMagic magic)
        {
            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 9);

            foreach (MapObject ob in targets)
            {
                if (ob.Race != ObjectType.Monster) continue;

                if (!CanAttackTarget(ob)) continue;
                if (ob.Level - 10 > Level || !((MonsterObject)ob).MonsterInfo.CanPush) continue;

                if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) continue;

                if (!ob.Teleport(CurrentMap, CurrentMap.GetRandomLocation(CurrentLocation, 3))) continue;

                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(1 + magic.Level),
                    TickCount = 1,
                });

                LevelMagic(magic);
            }
        }
        /// <summary>
        /// 铁布衫结束时
        /// </summary>
        /// <param name="magic"></param>
        public void DefianceEnd(UserMagic magic)
        {
            if (!Config.ZTBS)
            {
                if (Buffs.Any(x => x.Type == BuffType.Might))
                {
                    BuffRemove(BuffType.Might);
                }

                int valueAC = 4 + magic.Level * 5;     //增加防御
                int valueMR = 4 + magic.Level * 5;     //增加魔防
                int valueDC = -(2 + magic.Level * 3);  //减少攻击

                Stats buffStats = new Stats
                {
                    [Stat.MinAC] = valueAC,
                    [Stat.MaxAC] = valueAC,
                    [Stat.MinMR] = valueMR,
                    [Stat.MaxMR] = valueMR,
                    [Stat.MinDC] = valueDC,
                    [Stat.MaxDC] = valueDC,
                };

                BuffAdd(BuffType.Defiance, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);
            }
            else
            {
                if (Buffs.Any(x => x.Type == BuffType.Might))
                {
                    BuffRemove(BuffType.Might);
                    ChangeHP(-(CurrentHP / 2));
                }

                Stats buffStats = new Stats
                {
                    [Stat.Defiance] = 1,
                };

                BuffAdd(BuffType.Defiance, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);
            }

            LevelMagic(magic);
        }
        /// <summary>
        /// 破血狂杀结束时
        /// </summary>
        /// <param name="magic"></param>
        public void MightEnd(UserMagic magic)
        {
            if (!Config.PXKS)
            {
                if (Buffs.Any(x => x.Type == BuffType.Defiance))
                {
                    BuffRemove(BuffType.Defiance);
                }

                int valueAC = -(2 + magic.Level * 3);   //减少防御
                int valueMR = -(3 + magic.Level * 4);   //减少魔防
                int valueDC = 4 + magic.Level * 5;      //增加攻击

                Stats buffStats = new Stats
                {
                    [Stat.MinDC] = valueDC,
                    [Stat.MaxDC] = valueDC,
                    [Stat.MinAC] = valueAC,
                    [Stat.MaxAC] = valueAC,
                    [Stat.MinMR] = valueMR,
                    [Stat.MaxMR] = valueMR,
                };

                BuffAdd(BuffType.Might, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);
            }
            else
            {
                if (Buffs.Any(x => x.Type == BuffType.Defiance))
                {
                    BuffRemove(BuffType.Defiance);
                    ChangeHP(-(CurrentHP / 2));
                }
                int value = 4 + magic.Level * 6;

                Stats buffStats = new Stats
                {
                    [Stat.MinDC] = value,
                    [Stat.MaxDC] = value,
                };

                BuffAdd(BuffType.Might, TimeSpan.FromSeconds(60 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);
            }

            LevelMagic(magic);
        }
        /// <summary>
        /// 金刚之躯结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void EnduranceEnd(UserMagic magic)
        {
            BuffAdd(BuffType.Endurance, TimeSpan.FromSeconds(10 + magic.Level * 5), null, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }

        /// <summary>
        /// 无敌结束
        /// </summary>
        /// <param name="magic"></param>
        public void InvincibilityEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.Invincibility] = 1,
            };

            BuffAdd(BuffType.Invincibility, TimeSpan.FromSeconds(5 + magic.Level), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 移花接木结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void ReflectDamageEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.ReflectDamage] = 5 + magic.Level * 3,
            };

            BuffAdd(BuffType.ReflectDamage, TimeSpan.FromSeconds(15 + magic.Level * 10), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 泰山压顶结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        public void FetterEnd(UserMagic magic, Cell cell)
        {
            if (cell == null || cell.Map != CurrentMap) return;

            if (cell.Objects == null) return;

            foreach (MapObject ob in cell.Objects)
            {
                if (!CanAttackTarget(ob)) continue;

                switch (ob.Race)
                {
                    case ObjectType.Monster:
                        if (ob.Level > Level + 15) continue;

                        ob.ApplyPoison(new Poison
                        {
                            Owner = this,
                            Value = (3 + magic.Level) * 2,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromSeconds(5 + magic.Level * 3),
                            Type = PoisonType.Slow,
                        });
                        break;
                }

                LevelMagic(magic);
            }
        }
    }
}
