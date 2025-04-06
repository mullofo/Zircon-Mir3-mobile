using Library;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 刺客技能
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 毒云结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void PoisonousCloudEnd(UserMagic magic)
        {
            if (CurrentCell.Objects.FirstOrDefault(x => x.Race == ObjectType.Spell && ((SpellObject)x).Effect == SpellEffect.PoisonousCloud) != null) return;

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

            int duration = magic.GetPower();
            foreach (Cell cell in cells)
            {
                SpellObject ob = new SpellObject
                {
                    Visible = cell == CurrentCell,
                    DisplayLocation = CurrentLocation,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(duration),
                    Owner = this,
                    Effect = SpellEffect.PoisonousCloud,
                    Power = 5,
                };

                ob.Spawn(CurrentMap.Info, cell.Location);
            }
            LevelMagic(magic);
        }
        /// <summary>
        /// 潜行结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="forceGhost">鬼灵步</param>
        public void CloakEnd(UserMagic magic, MapObject ob, bool forceGhost)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Cloak)) return;

            UserMagic pledgeofBlood = Magics.ContainsKey(MagicType.PledgeOfBlood) ? Magics[MagicType.PledgeOfBlood] : null;

            int value = 0;
            if (pledgeofBlood != null && Level >= pledgeofBlood.Info.NeedLevel1)
                value = pledgeofBlood.GetPower();

            Stats buffStats = new Stats
            {
                [Stat.Cloak] = 1,
                [Stat.CloakDamage] = Stats[Stat.Health] * (20 - magic.Level - value) / 1000,
            };

            ob.BuffAdd(BuffType.Cloak, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.FromSeconds(2));

            LevelMagic(magic);
            LevelMagic(pledgeofBlood);
            if (!forceGhost)
            {
                UserMagic ghostWalk = Magics.ContainsKey(MagicType.GhostWalk) ? Magics[MagicType.GhostWalk] : null;
                if (ghostWalk == null || Level < ghostWalk.Info.NeedLevel1) return;

                int rate = (ghostWalk.Level + 1) * 3;

                if (SEnvir.Random.Next(2 + rate) >= rate) return;

                LevelMagic(ghostWalk);
            }
            ob.BuffAdd(BuffType.GhostWalk, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 血之盟约结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        public void RakeEnd(UserMagic magic, Cell cell)
        {
            if (cell?.Objects == null) return;

            foreach (MapObject ob in cell.Objects)
                if (MagicAttack(new List<UserMagic> { magic }, ob, true) > 0) break;
        }
        /// <summary>
        /// 亡灵束缚结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void WraithGripEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            int power = GetSP();

            int duration = magic.GetPower();

            UserMagic touch = null;

            if (Magics.TryGetValue(MagicType.TouchOfTheDeparted, out touch) && Level < magic.Info.NeedLevel1)
                touch = null;

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.WraithGrip,
                Owner = this,
                TickCount = ob.Race == ObjectType.Player ? duration * 7 / 10 : duration,
                TickFrequency = TimeSpan.FromSeconds(1),
                Extra = touch,
            });

            if (touch != null)
                ob.ApplyPoison(new Poison
                {
                    Value = power,
                    Type = PoisonType.Paralysis,

                    Owner = this,
                    TickCount = ob.Race == ObjectType.Player ? duration * 3 / 10 : duration,
                    TickFrequency = TimeSpan.FromSeconds(1),
                });

            LevelMagic(magic);
            LevelMagic(touch);
        }
        /// <summary>
        /// 深渊苦海结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void AbyssEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || (ob.Poison & PoisonType.Abyss) == PoisonType.Abyss) return;

            int power = GetSP();

            int duration = (magic.Level + 3) * 2;

            if (ob.Race == ObjectType.Monster)
                duration *= 2;

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.Abyss,
                Owner = this,
                TickCount = duration,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            if (ob.Race == ObjectType.Monster)
                ((MonsterObject)ob).Target = null;

            LevelMagic(magic);
        }
        /// <summary>
        /// 烈焰结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void HellFireEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            if (MagicAttack(new List<UserMagic> { magic }, ob, true) <= 0) return;

            int power = Math.Min(GetSC(), GetMC()) / 2;

            int duration = magic.GetPower();

            ob.ApplyPoison(new Poison
            {
                Value = power,
                Type = PoisonType.HellFire,
                Owner = this,
                TickCount = duration / 2,
                TickFrequency = TimeSpan.FromSeconds(2),
            });

            LevelMagic(magic);
        }
        /// <summary>
        /// 心机一转结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void TheNewBeginningEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.TheNewBeginning] = Math.Min(4, Stats[Stat.TheNewBeginning] + 1)
            };

            ob.BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), buffStats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 亡灵替身结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void SummonPuppetEnd(UserMagic magic, MapObject ob)
        {
            /*  if (CurrentMap.Info.SkillDelay > 0)
              {
                  Connection.ReceiveChat("".Lang(Connection.Language.SkillBadMap, magic.Info.Name), MessageType.System);

                  foreach (SConnection con in Connection.Observers)
                      con.ReceiveChat("".Lang(con.Language.SkillBadMap, magic.Info.Name), MessageType.System);
                  return;
              }*/

            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            List<UserMagic> magics = new List<UserMagic> { magic };

            UserMagic augMagic;

            //Summon Puppets.

            int count = magic.Level + 1;

            if (Magics.TryGetValue(MagicType.ElementalPuppet, out augMagic) && Level < augMagic.Info.NeedLevel1)
                augMagic = null;

            Stats darkstoneStats = new Stats();
            if (augMagic != null)
            {
                if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)
                    darkstoneStats = Equipment[(int)EquipmentSlot.Amulet].Info.Stats;

                DamageDarkStone(10);

                magics.Add(augMagic);
            }

            if (Magics.TryGetValue(MagicType.ArtOfShadows, out augMagic) && Level < augMagic.Info.NeedLevel1)
                augMagic = null;

            int range = 1;
            if (augMagic != null)
            {
                count += augMagic.GetPower();
                range = 3;

                magics.Add(augMagic);
            }

            for (int i = 0; i < count; i++)
            {
                Puppet mob = new Puppet
                {
                    MonsterInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.SummonPuppet),
                    Player = this,
                    DarkStoneStats = darkstoneStats,
                    Direction = Direction,
                    TameTime = SEnvir.Now.AddDays(365)
                };

                foreach (UserMagic m in magics)
                    mob.Magics.Add(m);

                if (mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, range)))
                {
                    Pets.Add(mob);
                    mob.PetOwner = this;
                }
            }

            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                Connection.ReceiveChat("".Lang(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("".Lang(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Cell cell = CurrentMap.GetCell(CurrentMap.GetRandomLocation(CurrentLocation, 4));  //随机变换位置

            if (cell != null) CurrentCell = cell;  //如果位置等空，就保持当前位置

            CloakEnd(magic, ob, true);   //进行隐身

            //发包移动到随机位置
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            LevelMagic(magic);
        }
        /// <summary>
        /// 鹰击结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void DanceOfSwallowEnd(UserMagic magic, MapObject ob)
        {
            if (!Config.YJDTYD)
            {
                if (CurrentMap.Info.SkillDelay > 0)
                {
                    Connection.ReceiveChat("Skills.SkillBadMap".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Skills.SkillBadMap".Lang(con.Language, magic.Info.Name), MessageType.System);
                    return;
                }
            }

            if (!CanAttackTarget(ob))
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
            Cell cell = null;
            for (int i = 0; i < 8; i++)
            {
                cell = CurrentMap.GetCell(Functions.Move(ob.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                if (cell == null || cell.IsBlocking(this, false) || cell.Movements != null)
                {
                    cell = null;
                    continue;
                }
                break;
            }

            if (cell == null)
            {
                Enqueue(new S.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            /* if (CurrentMap.Info.SkillDelay > 0)
             {
                 TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay);

                 Connection.ReceiveChat("".Lang(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                 foreach (SConnection con in Connection.Observers)
                     con.ReceiveChat("".Lang(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                 UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                 Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
             }*/

            PreventSpellCheck = true;
            CurrentCell = cell;
            PreventSpellCheck = false;

            Direction = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            BuffRemove(BuffType.Transparency);
            BuffRemove(BuffType.Cloak);

            CombatTime = SEnvir.Now;

            if (Stats[Stat.Comfort] < 150)
                RegenTime = SEnvir.Now + RegenDelay;
            ActionTime = SEnvir.Now + TimeSpan.FromMilliseconds(Config.GlobalsAttackTime);

            int aspeed = Stats[Stat.AttackSpeed];
            int attackDelay = (int)(Config.GlobalsAttackDelay - aspeed / 10.0 * Config.GlobalsASpeedRate);
            attackDelay = Math.Max(100, attackDelay);
            AttackTime = SEnvir.Now.AddMilliseconds(attackDelay);

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, AttackMagic = magic.Info.Magic });

            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(400), ActionType.DelayAttack,
                ob,
                new List<UserMagic> { magic },
                true,
                0));

            int delay = magic.Info.Delay;
            //if (SEnvir.Now <= PvPTime.AddSeconds(30))
            //delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });
        }
        /// <summary>
        /// 黄泉旅者结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void DarkConversionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.DarkConversion)) return;

            Stats buffStats = new Stats
            {
                [Stat.DarkConversion] = magic.GetPower(),
            };

            ob.BuffAdd(BuffType.DarkConversion, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.FromSeconds(2));
        }
        /// <summary>
        /// 风之闪避结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void EvasionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.EvasionChance] = 4 + magic.Level * 2,
            };

            ob.BuffAdd(BuffType.Evasion, TimeSpan.FromSeconds(magic.GetPower()), buffStats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 风之守护结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void RagingWindEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            ob.BuffAdd(BuffType.RagingWind, TimeSpan.FromSeconds(magic.GetPower()), null, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 集中结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void ConcentrationEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Concentration)) return;

            int power = 5 + magic.Level;

            Stats buffStats = new Stats
            {
                [Stat.CriticalChance] = power,
            };

            ob.BuffAdd(BuffType.Concentration, TimeSpan.FromSeconds(magic.GetPower()), buffStats, false, false, TimeSpan.FromSeconds(1));

            LevelMagic(magic);
        }
        /// <summary>
        /// 业火结束时
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="location">坐标</param>
        /// <param name="power">伤害</param>
        private void SwordOfVengeanceEnd(UserMagic magic, Point location, int power)
        {
            Cell cell = CurrentMap.GetCell(location);
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.SwordOfVengeance) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                Visible = true,
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(1),
                Owner = this,
                Effect = SpellEffect.SwordOfVengeance,
                Magic = magic,
            };

            ob.Spawn(cell.Map.Info, cell.Location);

            LevelMagic(magic);
        }
    }
}
