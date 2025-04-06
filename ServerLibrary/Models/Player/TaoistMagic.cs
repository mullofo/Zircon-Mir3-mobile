using Library;
using Library.SystemModels;
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
    /// 道士技能
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 阴阳法环技能激活
        /// </summary>
        public override void CelestialLightActivate()
        {
            base.CelestialLightActivate();

            UserMagic magic;

            if (Magics.TryGetValue(MagicType.CelestialLight, out magic)) return;

            int delay = magic.Info.Delay;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });
        }

        /// <summary>
        /// 治愈术
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void HealEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.CurrentHP >= ob.Stats[Stat.Health] || ob.Buffs.Any(x => x.Type == BuffType.Heal)) return;

            UserMagic empowered;
            int bonus = 0;
            int cap = Config.FixedCureValue;  //固定加血点数

            if (Magics.TryGetValue(MagicType.EmpoweredHealing, out empowered) && Level >= empowered.Info.NeedLevel1)
            {
                bonus = empowered.GetPower();  //获取技能攻击值

                if (!Config.FixedCure)
                {
                    cap += (1 + empowered.Level) * Config.FixedCureValue;  //等级*固定加血点数
                }

                LevelMagic(empowered);
            }

            if (magic.Level >= 4)
            {
                cap = cap + 10;
            }

            Stats buffStats = new Stats
            {
                [Stat.Healing] = magic.GetPower() + GetSC() + bonus,   //生命恢复的总数
                [Stat.HealingCap] = cap,   //每秒加血点数
            };

            //BUFF持续时间等于  总恢复血量/每秒加血点数
            ob.BuffAdd(BuffType.Heal, TimeSpan.FromSeconds((buffStats[Stat.Healing] / buffStats[Stat.HealingCap]) / 2), buffStats, false, false, TimeSpan.FromSeconds(1));
            if (Class == MirClass.Taoist)
                LevelMagic(magic);
        }
        /// <summary>
        /// 施毒术结束
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="type">毒类型</param>
        public void PoisonDustEnd(List<UserMagic> magics, MapObject ob, PoisonType type)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;

            UserMagic magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.PoisonDust);
            if (magic == null) return;

            for (int i = Pets.Count - 1; i >= 0; i--)
                if (Pets[i].Target == null)
                    Pets[i].Target = ob;

            int duration = magic.GetPower() + GetSC();

            if (ob.Race == ObjectType.Player)
            {
                if (SEnvir.Random.Next(100) < Stats[Stat.GreenPosionPro])
                {
                    ob.ApplyPoison(new Poison
                    {
                        Value = magic.Level + 1 + Level / 10,
                        Type = type,
                        Owner = this,
                        TickCount = duration / 2,
                        TickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickTime = SEnvir.Now.AddSeconds(5),
                        ExtraValue = 10
                    });
                    ob.GreenPosionPro();
                }
                else
                {
                    ob.ApplyPoison(new Poison
                    {
                        Value = magic.Level + 1 + Level / 10,
                        Type = type,
                        Owner = this,
                        TickCount = duration / 2,
                        TickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickTime = SEnvir.Now.AddSeconds(5),
                        ExtraValue = 0
                    });
                }
            }
            else
            {
                if (SEnvir.Random.Next(100) < Stats[Stat.GreenPosionPro])
                {
                    ob.ApplyPoison(new Poison
                    {
                        Value = magic.Level + 1 + Level / 10,
                        Type = type,
                        Owner = this,
                        TickCount = duration / 2,
                        TickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickTime = SEnvir.Now.AddSeconds(5),
                        ExtraValue = 10
                    });
                    ob.GreenPosionPro();
                }
                else
                {
                    ob.ApplyPoison(new Poison
                    {
                        Value = magic.Level + 1 + Level / 10,
                        Type = type,
                        Owner = this,
                        TickCount = duration / 2,
                        TickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickFrequency = TimeSpan.FromSeconds(3),
                        ExtraTickTime = SEnvir.Now.AddSeconds(5),
                        ExtraValue = 0
                    });
                }
            }
            foreach (UserMagic mag in magics)
                LevelMagic(mag);
        }
        /// <summary>
        /// 隐身术结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void InvisibilityEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Invisibility)) return;

            Stats buffStats = new Stats
            {
                [Stat.Invisibility] = 1
            };

            ob.BuffAdd(BuffType.Invisibility, TimeSpan.FromSeconds(magic.GetPower() + GetSC()), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 吸星大法结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void LifeStealEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.LifeSteal] = 4 + (magic.Level) * 2
            };

            ob.BuffAdd(BuffType.LifeSteal, TimeSpan.FromSeconds(magic.GetPower() + GetSC() + Stats[Stat.DarkAttack] * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 移花接玉结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void StrengthOfFaithEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.DCPercent] = -(magic.Info.MinBasePower + magic.Level * magic.Info.MinLevelPower),   //移花自定义减属性
                [Stat.PetDCPercent] = magic.Info.MaxBasePower + magic.Level * magic.Info.MaxLevelPower,   //移花自定义加属性
            };

            BuffAdd(BuffType.StrengthOfFaith, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 妙影无踪结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="location">坐标</param>
        public void TransparencyEnd(UserMagic magic, MapObject ob, Point location)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;  //|| ob.Buffs.Any(x => x.Type == BuffType.Transparency)

            if (Config.MYWZYX)
            {
                Teleport(CurrentMap, location, false);
            }

            int delay = magic.Info.Delay;
            //if (SEnvir.Now <= PvPTime.AddSeconds(30))
            //delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            Stats buffStats = new Stats
            {
                [Stat.Transparency] = 1
            };

            ob.BuffAdd(BuffType.Transparency, TimeSpan.FromSeconds(Math.Min(SEnvir.Now <= PvPTime.AddSeconds(30) ? 20 : 3600, magic.GetPower() + GetSC() / 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 隐魂术结束
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="ob"></param>
        public void MassTransparencyEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob) || ob.Buffs.Any(x => x.Type == BuffType.Transparency)) return;

            int delay = magic.Info.Delay;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });

            Stats buffStats = new Stats
            {
                [Stat.Transparency] = 1
            };

            ob.BuffAdd(BuffType.Transparency, TimeSpan.FromSeconds(Math.Min(SEnvir.Now <= PvPTime.AddSeconds(30) ? 20 : 3600, magic.GetPower() + GetSC() / 2 + Stats[Stat.PhantomAttack] * 2)), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 阴阳法环结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void CelestialLightEnd(UserMagic magic)
        {
            //if (Buffs.Any(x => x.Type == BuffType.CelestialLight)) return;

            Stats buffStats = new Stats
            {
                [Stat.CelestialLight] = (magic.Level + 1) * 10,
            };

            BuffAdd(BuffType.CelestialLight, TimeSpan.FromSeconds(magic.GetPower() + GetSC()), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 幽灵盾结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="stats">属性</param>
        public void MagicResistanceEnd(UserMagic magic, MapObject ob, Stats stats)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.MaxMR] = 5 + magic.Level
            };

            if (stats[Stat.FireAffinity] > 0)
            {
                buffStats[Stat.FireResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.IceAffinity] > 0)
            {
                buffStats[Stat.IceResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.LightningAffinity] > 0)
            {
                buffStats[Stat.LightningResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.WindAffinity] > 0)
            {
                buffStats[Stat.WindResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.HolyAffinity] > 0)
            {
                buffStats[Stat.HolyResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.DarkAffinity] > 0)
            {
                buffStats[Stat.DarkResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            if (stats[Stat.PhantomAffinity] > 0)
            {
                buffStats[Stat.PhantomResistance] = 2;
                buffStats[Stat.MinMR] = 0;
                buffStats[Stat.MaxMR] = 0;
            }

            ob.BuffAdd(BuffType.MagicResistance, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 神圣战甲术结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void ResilienceEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            if (Config.PhysicalResistanceSwitch)
            {
                Stats buffStats = new Stats
                {
                    [Stat.MaxAC] = 5 + magic.Level,
                    [Stat.PhysicalResistance] = 1,
                };
                ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);
            }
            else
            {
                Stats buffStats = new Stats
                {
                    [Stat.MaxAC] = 5 + magic.Level,
                };
                ob.BuffAdd(BuffType.Resilience, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);
            }

            LevelMagic(magic);
        }
        /// <summary>
        /// 强震魔法
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="stats">属性</param>
        public void ElementalSuperiorityEnd(UserMagic magic, MapObject ob, Stats stats)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.MaxMC] = 3 + magic.Level,
                [Stat.MaxSC] = 3 + magic.Level
            };

            if (stats[Stat.FireAffinity] > 0)
            {
                buffStats[Stat.FireAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.IceAffinity] > 0)
            {
                buffStats[Stat.IceAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.LightningAffinity] > 0)
            {
                buffStats[Stat.LightningAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.WindAffinity] > 0)
            {
                buffStats[Stat.WindAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.HolyAffinity] > 0)
            {
                buffStats[Stat.HolyAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.DarkAffinity] > 0)
            {
                buffStats[Stat.DarkAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            if (stats[Stat.PhantomAffinity] > 0)
            {
                buffStats[Stat.PhantomAttack] = 3 + magic.Level;
                buffStats[Stat.MaxMC] = 0;
                buffStats[Stat.MaxSC] = 0;
            }

            ob.BuffAdd(BuffType.ElementalSuperiority, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 猛虎强势
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void BloodLustEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;

            Stats buffStats = new Stats
            {
                [Stat.MaxDC] = 5 + magic.Level
            };

            ob.BuffAdd(BuffType.BloodLust, TimeSpan.FromSeconds(magic.GetPower() + GetSC() * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 云寂术
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="ob">对象</param>
        public void PurificationEnd(List<UserMagic> magics, MapObject ob)
        {
            if (ob?.Node == null || magics.Count == 0) return;

            UserMagic magic = magics[0];
            if (SEnvir.Random.Next(100) > 40 + magic.Level * 20) return;

            if (ob.Level > (Level + Stats[Stat.Repulsion]) + 3) return;

            if (ob.Level == Level && SEnvir.Random.Next(100) > 90) return;

            if (ob.Level > Level && SEnvir.Random.Next(100) > Math.Max(90 - (ob.Level - Level) * 20, 30)) return;

            int result = Purify(ob);

            for (int i = 0; i < result; i++)
                foreach (UserMagic m in magics)
                    LevelMagic(m);
        }
        /// <summary>
        /// 魔焰强解术
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="stats">属性</param>
        public void DemonExplosionEnd(UserMagic magic, Stats stats)
        {
            MonsterObject pet = Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;

            int damage = pet.Stats[Stat.Health];
            pet.Broadcast(new S.ObjectEffect { Effect = Effect.DemonExplosion, ObjectID = pet.ObjectID });

            List<MapObject> targets = GetTargets(pet.CurrentMap, pet.CurrentLocation, 2);

            pet.ChangeHP(-damage * 75 / 100);

            int damagePvE = damage * magic.GetPower() / 100 + GetSC() * 3;
            int damagePvP = (damage * magic.GetPower() / 100 + GetSC() * 3) / Config.DemonExplosionPVP;

            if (stats != null && stats.GetAffinityValue(Element.Phantom) > 0)
            {
                damagePvE += GetElementPower(ObjectType.Monster, Stat.PhantomAttack) * 8;
                damagePvP += GetElementPower(ObjectType.Player, Stat.PhantomAttack) * 8 / Config.DemonExplosionPVP;
            }

            foreach (MapObject target in targets)
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(800),
                    ActionType.DelayedMagicDamage,
                    new List<UserMagic> { magic },
                    target,
                    true,
                    null,
                    target.Race == ObjectType.Player ? damagePvP : damagePvE));
            }

            LevelMagic(magic);
        }
        /// <summary>
        /// 地狱回疗
        /// </summary>
        /// <param name="magic">技能</param>
        public void DemonicRecoveryEnd(UserMagic magic)
        {
            MonsterObject pet = Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.InfernalSoldier && !x.Dead);

            if (pet == null) return;

            int health = pet.Stats[Stat.Health] * magic.GetPower() / 100;

            pet.ChangeHP(health);

            LevelMagic(magic);
        }
        /// <summary>
        /// 回生术结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        public void ResurrectionEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !ob.Dead) return;

            if (SEnvir.Random.Next(100) > 25 + magic.Level * 20) return;

            int power = magic.GetPower();

            ob.Dead = false;
            ob.SetHP(ob.Stats[Stat.Health] * power / 100);
            ob.SetMP(ob.Stats[Stat.Mana] * power / 100);

            Broadcast(new S.ObjectRevive { ObjectID = ob.ObjectID, Location = ob.CurrentLocation, Effect = false });
            PlayerObject player = (PlayerObject)ob;    //修复回生术 人物无法完整复活
            player.Enqueue(new S.ObjectRevive { ObjectID = ob.ObjectID, Location = ob.CurrentLocation, Effect = false });

            LevelMagic(magic);

            magic.Cooldown = SEnvir.Now.AddSeconds(1);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = 1000 });
        }
        /// <summary>
        /// 传染结束
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="ob">对象</param>
        public void InfectionEnd(List<UserMagic> magics, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || (ob.Poison & PoisonType.Infection) == PoisonType.Infection) return;

            UserMagic magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.Infection);
            if (magic == null) return;

            ob.ApplyPoison(new Poison
            {
                Value = GetSC() + Stats[Stat.CriticalChance] + Stats[Stat.CriticalDamage],
                Type = PoisonType.Infection,
                Owner = this,
                TickCount = 10 + magic.Level * 10,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            foreach (UserMagic mag in magics)
                LevelMagic(mag);
        }
        /// <summary>
        /// 虚弱化结束
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="ob">对象</param>
        public void NeutralizeEnd(List<UserMagic> magics, MapObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob) || ob.Level >= Level || (ob.Poison & PoisonType.Neutralize) == PoisonType.Neutralize) return;

            UserMagic magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.Neutralize);
            if (magic == null) return;

            int time = 5 + magic.Level * 2;

            ob.ApplyPoison(new Poison
            {
                Type = PoisonType.Neutralize,
                Owner = this,
                TickCount = time,
                TickFrequency = TimeSpan.FromSeconds(1),
            });

            foreach (UserMagic mag in magics)
                LevelMagic(mag);
        }
        /// <summary>
        /// 困魔咒结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="map">地图</param>
        /// <param name="location">坐标</param>
        public void TrapOctagonEnd(UserMagic magic, Map map, Point location)
        {
            if (map != CurrentMap) return;

            List<MapObject> targets = GetTargets(CurrentMap, location, 1);

            List<MapObject> trappedMonsters = new List<MapObject>();

            foreach (MapObject target in targets)
            {
                if (target.Race != ObjectType.Monster || target.Level >= Level + SEnvir.Random.Next(3)) continue;

                trappedMonsters.Add((MonsterObject)target);
            }

            if (trappedMonsters.Count == 0) return;

            int duration = GetSC() + magic.GetPower();

            List<Point> locationList = new List<Point>
            {
                new Point(location.X - 1, location.Y - 2),
                new Point(location.X - 1, location.Y + 2),
                new Point(location.X + 1, location.Y - 2),
                new Point(location.X + 1, location.Y + 2),

                new Point(location.X - 2, location.Y - 1),
                new Point(location.X - 2, location.Y + 1),
                new Point(location.X + 2, location.Y - 1),
                new Point(location.X + 2, location.Y + 1)
            };

            foreach (Point point in locationList)
            {
                SpellObject ob = new SpellObject
                {
                    DisplayLocation = point,
                    TickCount = duration * 4, //每隔1/4秒检查一次，看看是否所有的怪物都被困住了
                    TickFrequency = TimeSpan.FromMilliseconds(250),
                    Owner = this,
                    Effect = SpellEffect.TrapOctagon,
                    Magic = magic,
                    Targets = trappedMonsters,
                };

                ob.Spawn(map.Info, point);
            }

            DateTime shockTime = SEnvir.Now.AddSeconds(duration);
            foreach (MonsterObject monster in trappedMonsters)
            {
                if (shockTime <= monster.ShockTime) continue;

                monster.ShockTime = SEnvir.Now.AddSeconds(duration);
                LevelMagic(magic);
            }
        }
        /// <summary>
        /// 空拳刀法
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        /// <param name="direction">方位朝向</param>
        private void TaoistCombatKick(UserMagic magic, Cell cell, MirDirection direction)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];

                if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= (Level + Stats[Stat.Repulsion]) || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level) continue;

                CheckBrown(ob);

                if (ob.Pushed(direction, magic.GetPower()) <= 0) continue;

                Attack(ob, new List<UserMagic> { magic }, true, 0);
                LevelMagic(magic);
                break;
            }
        }
        /// <summary>
        /// 宝宝召唤结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="map">地图</param>
        /// <param name="location">坐标</param>
        /// <param name="info">怪物信息</param>
        public void SummonEnd(UserMagic magic, Map map, Point location, MonsterInfo info)
        {
            //如果怪物信息等空跳出
            if (info == null) return;
            //定义 ob等宝宝信息
            MonsterObject ob = Pets.FirstOrDefault(x => x.MonsterInfo == info);

            if (ob != null)  //如果宝宝信息不等空
            {
                ob.PetRecall();   //宝宝召唤
                return;
            }

            int count = 0;
            for (int i = 0; i < Pets.Count; i++)
            {
                MonsterObject pet = Pets[i];
                if (pet.MonsterInfo.AI == 146) continue;
                count++;
            }
            if (count >= 2) return;  //如果宝宝信息大于等于2跳出

            ob = MonsterObject.GetMonster(info);  //ob等怪物对象信息

            ob.PetOwner = this;   //宝宝主人等角色自己
            Pets.Add(ob);         //宝宝增加怪物对象

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;
            ob.Magics.Add(magic);

            // 宠物等级
            if (!Config.UpgradePetAdd)  //如果不是自定义宠物等级
            {
                ob.SummonLevel = magic.Level * 2;    //宠物等级等技能等级 * 2
            }
            else
            {
                ob.SummonLevel = magic.Level;   //宠物等级默认等技能等级
            }

            ob.TameTime = SEnvir.Now.AddDays(365);

            if (Buffs.Any(x => x.Type == BuffType.StrengthOfFaith))
                ob.Magics.Add(Magics[MagicType.StrengthOfFaith]);

            if (magic.Info.Magic == MagicType.SummonDemonicCreature && Magics.TryGetValue(MagicType.DemonicRecovery, out UserMagic demonRecovery))
            {
                ob.Magics.Add(demonRecovery);
            }

            //定义 cell 等于 角色坐标面前一格
            Cell cell = map.GetCell(Functions.Move(ob.PetOwner.CurrentLocation, ob.PetOwner.Direction));

            //  如果 cell等空  或  cell地图链接不等空  或者 对象刷新的坐标
            if (cell == null || cell.Movements != null || !ob.Spawn(map.Info, location))
            {
                //宝宝刷新在随机范围10格内的坐标里
                if (SEnvir.Random.Next(100) < Config.SummonRandomValue)
                    ob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 12));
                else
                    ob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 6));
            }

            // 召唤生物血量            
            ob.SetHP(ob.Stats[Stat.Health]);

            //如果宠物等级大于或者等于0 且 宠物主人幻影有加 且 服务端开启幻影加成
            if (ob.SummonLevel >= 0 && Stats[Stat.PhantomAttack] > 0 && Config.PetPhantomAttack)
            {
                Stats buffStats = new Stats
                {
                    [Stat.MinAC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAcMrEdit,
                    [Stat.MaxAC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAcMrEdit,

                    [Stat.MinMR] = Stats[Stat.PhantomAttack] * Config.PetPhantomAcMrEdit,
                    [Stat.MaxMR] = Stats[Stat.PhantomAttack] * Config.PetPhantomAcMrEdit,

                    [Stat.MinDC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,
                    [Stat.MaxDC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,

                    [Stat.MinMC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,
                    [Stat.MaxMC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,

                    [Stat.MinSC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,
                    [Stat.MaxSC] = Stats[Stat.PhantomAttack] * Config.PetPhantomAttackEdit,
                };

                ob.BuffAdd(BuffType.PhantomBuff, TimeSpan.FromDays(30), buffStats, true, false, TimeSpan.Zero);
            }

            LevelMagic(magic);
        }
        /// <summary>
        /// 暗鬼阵结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="location">坐标</param>
        /// <param name="power">威力</param>
        private void DarkSoulPrisonEnd(UserMagic magic, Point location, int power)
        {
            List<Cell> cells = CurrentMap.GetCells(location, 0, 3);

            foreach (Cell cell in cells)
            {
                if (cell.Objects != null)
                {
                    for (int i = cell.Objects.Count - 1; i >= 0; i--)
                    {
                        if (cell.Objects[i].Race != ObjectType.Spell) continue;

                        SpellObject spell = (SpellObject)cell.Objects[i];

                        if (spell.Effect != SpellEffect.DarkSoulPrison) continue;

                        spell.Despawn();
                    }
                }

                SpellObject ob = new SpellObject
                {
                    Visible = cell.Location == location,
                    DisplayLocation = cell.Location,
                    TickCount = power,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    Owner = this,
                    Effect = SpellEffect.DarkSoulPrison,
                    Magic = magic,
                };

                ob.Spawn(cell.Map.Info, cell.Location);
            }

            LevelMagic(magic);
        }
    }
}
