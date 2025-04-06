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
    /// 法师技能
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        //public Element AttackElement => Stats.GetAffinityElement();

        /// <summary>
        /// 抗拒火环结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        /// <param name="direction">方位朝向</param>
        private void RepulsionEnd(UserMagic magic, Cell cell, MirDirection direction)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob) || ob.Level >= Config.MaxLevel || ob.Level >= (Level + Stats[Stat.Repulsion]) || SEnvir.Random.Next(16) >= 6 + magic.Level * 3 + Level - ob.Level) continue;

                CheckBrown(ob);
                //CanPush check ?

                if (ob.Pushed(direction, magic.GetPower()) <= 0) continue;

                LevelMagic(magic);
                break;
            }
        }
        /// <summary>
        /// 诱惑之光结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        private void ElectricShockEnd(UserMagic magic, MonsterObject ob)
        {
            if (ob?.Node == null || !CanAttackTarget(ob)) return;  //目标空  不可攻击的 跳出

            if (ob.MonsterInfo.IsBoss) return;  //是BOSS 跳出

            if (SEnvir.Random.Next(Config.ElectricShockSuccessRate - magic.Level) > 0)  //成功几率
            {
                if (SEnvir.Random.Next(2) == 0) LevelMagic(magic);  //随机2里出0 加技能经验
                return;
            }

            LevelMagic(magic);

            if (ob.PetOwner == this)  //怪物主人是自己
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(magic.Level * 5 + 10);  //诱惑晕住的时间
                ob.Target = null;
                return;
            }

            if (SEnvir.Random.Next(2) > 0)  //随机大于0
            {
                ob.ShockTime = SEnvir.Now.AddSeconds(magic.Level * 5 + 10);  //诱惑晕住的时间
                ob.Target = null;
                return;
            }

            if (ob.Level > Level || !ob.MonsterInfo.CanTame) return;   //对象等级大于角色等级+2  或  不能诱惑的怪  跳出

            if (SEnvir.Random.Next(Level + 20 + magic.Level * 5) <= ob.Level + 10)
            {
                if (SEnvir.Random.Next(5) > 0 && ob.PetOwner == null)   //一定的几率怪物会发狂
                {
                    ob.RageTime = SEnvir.Now.AddSeconds(SEnvir.Random.Next(20) + 10);
                    ob.Target = null;
                }
                return;
            }

            if (Pets.Count >= Config.ElectricShockPetsCount && (ob != null && (ob.MonsterInfo.Image != MonsterImage.Catapult || ob.MonsterInfo.Image != MonsterImage.Ballista))) return;   //诱惑之光诱惑宠物数量

            if (SEnvir.Random.Next(4) > 0) return;

            if (SEnvir.Random.Next(20) == 0)   //一定几率会将怪物诱惑死亡
            {
                if (ob.EXPOwner == null && ob.PetOwner == null)
                    ob.EXPOwner = this;

                ob.Die();
                return;
            }

            if (ob.PetOwner != null)
            {
                int hp = Math.Max(1, ob.Stats[Stat.Health] / 10);

                if (hp < ob.CurrentHP) ob.SetHP(hp);

                ob.PetOwner.Pets.Remove(ob);
                ob.PetOwner = null;
                ob.Magics.Clear();
            }
            else if (ob.SpawnInfo != null)
            {
                ob.SpawnInfo.AliveCount--;
                ob.SpawnInfo = null;
            }

            ob.PetOwner = this;
            Pets.Add(ob);

            ob.Master?.MinionList.Remove(ob);
            ob.Master = null;

            ob.TameTime = SEnvir.Now.AddHours(magic.Level + Config.PetsMutinyTime);
            ob.Target = null;
            ob.RageTime = DateTime.MinValue;
            ob.ShockTime = DateTime.MinValue;
            ob.Magics.Add(magic);
            ob.SummonLevel = 0;
            ob.RefreshStats();

            ob.Broadcast(new S.ObjectPetOwnerChanged { ObjectID = ob.ObjectID, PetOwner = Name });
        }
        /// <summary>
        /// 圣言术结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        private void ExpelUndeadEnd(UserMagic magic, MonsterObject ob)
        {
            //如果对象空 不是可攻击目标 是BOSS 等级大于或等于70级 跳过
            if (ob?.Node == null || !CanAttackTarget(ob) || ob.MonsterInfo.IsBoss || ob.Level >= Config.ExpelUndeadLevel) return;

            if (ob.Target == null && ob.CanAttackTarget(this))
                ob.Target = this;

            if (ob.Level >= Level - 1 + SEnvir.Random.Next(4)) return;  //对象等级大于自己的级别 跳过


            if (ob.PetOwner != null && SEnvir.Random.Next(100) < ob.PetOwner.Stats[Stat.ExpelUndeadResistance])  //防圣言
            {
                ob.PetOwner.Connection.ReceiveChat("抵抗圣言成功".Lang(Connection.Language), MessageType.Hint);
                return;
            }

            //如果随机值100 大于等于 35 + 技能等级*9 + (角色等级-怪物等级）*5 + 幻影属性/2   跳出
            if (SEnvir.Random.Next(100) >= Config.ExpelUndeadSuccessRate + magic.Level * 9 + (Level - ob.Level) * 1 + Stats[Stat.PhantomAttack] / 2) return;  //随机值

            if (ob.EXPOwner == null && ob.Master == null)
                ob.EXPOwner = this;

            CheckBrown(ob);

            ob.SetHP(0);

            LevelMagic(magic);
        }
        /// <summary>
        /// 瞬息移动结束
        /// </summary>
        /// <param name="magic">技能</param>
        private void TeleportationEnd(UserMagic magic)
        {
            if (CurrentMap.Info.SkillDelay > 0)
            {
                Connection.ReceiveChat("Skills.SkillBadMap".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.SkillBadMap".Lang(con.Language, magic.Info.Name), MessageType.System);
                return;
            }

            if (SEnvir.Random.Next(9) > 2 + magic.Level * 2) return;

            /*
            if (CurrentMap.Info.SkillDelay > 0)
            {
                TimeSpan delay = TimeSpan.FromMilliseconds(CurrentMap.Info.SkillDelay * 3);

                Connection.ReceiveChat("".Lang(Connection.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("".Lang(con.Language.SkillEffort, magic.Info.Name, Functions.ToString(delay, true)), MessageType.System);

                UseItemTime = (UseItemTime < SEnvir.Now ? SEnvir.Now : UseItemTime) + delay;
                Enqueue(new S.ItemUseDelay { Delay = SEnvir.Now - UseItemTime });
            }*/

            Teleport(CurrentMap, CurrentMap.GetRandomLocation());
            LevelMagic(magic);
        }
        /// <summary>
        /// 火墙结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        /// <param name="power">伤害次数</param>
        private void FireWallEnd(UserMagic magic, Cell cell, int power)
        {
            if (cell == null) return;
            //如果这个地图格内有火墙，则把之前的火墙去掉
            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect == SpellEffect.FireWall) return;

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }
            //生成新的火墙对象
            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(3),
                Owner = this,
                Effect = SpellEffect.FireWall,
                Magic = magic,
            };

            ob.Spawn(cell.Map.Info, cell.Location);
        }
        /// <summary>
        /// 移形换位结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="location">坐标</param>
        public void GeoManipulationEnd(UserMagic magic, Point location)
        {
            Cell targetCell = CurrentMap.GetCell(location);

            if (!Config.YXHW)
            {
                if (CurrentMap.Info.SkillDelay > 0)
                {
                    Connection.ReceiveChat("Skills.SkillBadMap".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                    foreach (SConnection con in Connection.Observers)
                        con.ReceiveChat("Skills.SkillBadMap".Lang(con.Language, magic.Info.Name), MessageType.System);
                    return;
                }
            }

            if (location == CurrentLocation) return;

            if (targetCell == null) return;

            if (targetCell.Objects != null)
            {
                foreach (MapObject ob in targetCell.Objects)
                {
                    switch (ob.Race)
                    {
                        case ObjectType.Monster:
                        case ObjectType.Player:
                            if (!ob.Dead) return;
                            break;
                        case ObjectType.NPC:
                            return;
                    }
                }
            }

            if (SEnvir.Random.Next(100) > Config.GeoManipulationSuccessRate + magic.Level * 15) return;

            if (!Teleport(CurrentMap, location, false)) return;

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

            LevelMagic(magic);

            int delay = magic.Info.Delay;
            //if (SEnvir.Now <= PvPTime.AddSeconds(30))
            //delay *= 10;

            magic.Cooldown = SEnvir.Now.AddMilliseconds(delay);
            Enqueue(new S.MagicCooldown { InfoIndex = magic.Info.Index, Delay = delay });
        }
        /// <summary>
        /// 魔法盾结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void MagicShieldEnd(UserMagic magic)
        {
            //魔法盾和护身法盾不同时出现
            //if (Buffs.Any(x => x.Type == BuffType.MagicShield || x.Type == BuffType.SuperiorMagicShield)) return;

            Stats buffStats = new Stats
            {
                [Stat.MagicShield] = magic.Info.MaxBasePower + magic.Level * magic.Info.MaxLevelPower     //修改魔法盾自定义抵消参数
            };

            BuffAdd(BuffType.MagicShield, TimeSpan.FromSeconds(30 + magic.Level * 20 + GetMC() / 2 + Stats[Stat.PhantomAttack] * 2), buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 护身法盾结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void SuperiorMagicShieldEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield)) return;

            BuffRemove(BuffType.MagicShield);

            Stats buffStats = new Stats
            {
                [Stat.SuperiorMagicShield] = (int)(Stats[Stat.Mana] * (0.25F + magic.Level * 0.05F))
            };

            BuffAdd(BuffType.SuperiorMagicShield, TimeSpan.MaxValue, buffStats, true, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 护身冰环
        /// </summary>
        /// <param name="magic">技能</param>
        public void FrostBiteEnd(UserMagic magic)
        {
            if (Buffs.Any(x => x.Type == BuffType.FrostBite)) return;

            Stats buffStats = new Stats
            {
                [Stat.FrostBiteDamage] = GetMC() + Stats[Stat.IceAttack] * 2 + magic.GetPower(),
                [Stat.FrostBiteMaxDamage] = Stats[Stat.MaxMC] * 50 + Stats[Stat.IceAttack] * 70,
            };

            BuffAdd(BuffType.FrostBite, TimeSpan.FromSeconds(3 + magic.Level * 3), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 凝血离魂结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void RenounceEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.HealthPercent] = -(magic.Info.MinBasePower + (magic.Level + 1) * 5 + magic.Info.MinLevelPower),   //修改凝血离魂自定义参数

                [Stat.MCPercent] = magic.Info.MaxBasePower + (magic.Level * 2 + 2) * 5 + magic.Info.MaxLevelPower,
            };

            int health = CurrentHP;

            BuffInfo buff = BuffAdd(BuffType.Renounce, TimeSpan.FromSeconds(30 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            buff.Stats[Stat.RenounceHPLost] = health - CurrentHP;
            Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });

            LevelMagic(magic);
        }
        /// <summary>
        /// 天打雷劈结束
        /// </summary>
        /// <param name="magic">技能</param>
        public void JudgementOfHeavenEnd(UserMagic magic)
        {
            Stats buffStats = new Stats
            {
                [Stat.JudgementOfHeaven] = (2 + magic.Level) * 20,
            };

            BuffAdd(BuffType.JudgementOfHeaven, TimeSpan.FromSeconds(30 + magic.Level * 30), buffStats, false, false, TimeSpan.Zero);

            LevelMagic(magic);
        }
        /// <summary>
        /// 怒神霹雳结束
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="cell">单元</param>
        /// <param name="extra">额外</param>
        private void ChainLightningEnd(List<UserMagic> magics, Cell cell, int extra)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob)) continue;

                MagicAttack(magics, ob, true, null, extra);
            }
        }
        /// <summary>
        /// 怒神霹雳结束2
        /// </summary>
        /// <param name="magics">技能</param>
        /// <param name="ob">对象</param>
        /// <param name="location">坐标</param>
        private void ChainLightningEnd2(List<UserMagic> magics, MapObject ob, Point location)
        {
            List<Cell> cells = CurrentMap.GetCells(ob?.CurrentLocation ?? location, 0, 4);
            if (cells == null) return;

            foreach (Cell cell in cells)
            {
                if (cell.Objects == null) continue;
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (i >= cell.Objects.Count) continue;
                    MapObject o = cell.Objects[i];
                    if (o == null) continue;
                    if (!CanAttackTarget(o)) continue;
                    int extra = Functions.Distance(cell.Location, location);
                    MagicAttack(magics, o, true, null, extra);
                }
            }
        }
        /// <summary>
        /// 旋风墙结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="cell">单元</param>
        /// <param name="power">威力</param>
        private void TempestEnd(UserMagic magic, Cell cell, int power)
        {
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = this,
                Effect = SpellEffect.Tempest,
                Magic = magic,
            };

            ob.Spawn(cell.Map.Info, cell.Location);
        }

        /// <summary>
        /// 分身术结束
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="ob">对象</param>
        private void MirrorImageEnd(UserMagic magic, MapObject ob)
        {
            if (ob?.Node == null || !CanHelpTarget(ob)) return;   //节点为空 或 可帮助目标 为空 跳出

            int count = 1;   //召唤分身的数量 = 1 

            if (Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone)   //判断装备格子附身符位置的元素石
            {
                //优先判断是否有分身，以免消耗元素石。
                MonsterObject mb = Pets.FirstOrDefault(x => x.MonsterInfo.Flag == MonsterFlag.MirrorImage);
                if (mb != null)  //如果分身不为空
                {
                    mb.PetRecall();  //召唤到身边
                    return;
                }

                //获取元素石属性
                Stats darkstoneStats = new Stats(Equipment[(int)EquipmentSlot.Amulet].Info.Stats);
                Element element = darkstoneStats.GetAffinityElement();
                if (element == Element.None) return;

                MagicType magicType = MagicType.None;
                switch (element)
                {
                    case Element.Fire:
                        magicType = MagicType.FireStorm;
                        break;
                    case Element.Ice:
                        magicType = MagicType.IceStorm;
                        break;
                    case Element.Lightning:
                        magicType = MagicType.LightningWave;
                        break;
                    case Element.Wind:
                        magicType = MagicType.DragonTornado;
                        break;
                }
                if (magicType == MagicType.None) return;
                //如果角色没有该魔法或者等级小于魔法所需等级
                UserMagic augMagic;
                if (!Magics.TryGetValue(magicType, out augMagic) || Level < augMagic.Info.NeedLevel1) return;

                int range = 2;   //默认范围2格

                for (int i = 0; i < count; i++)
                {
                    if (i >= count) return;  //如果分身数量大于1 就跳出

                    MirrorImage mob = new MirrorImage    //召唤的镜像分身
                    {
                        MonsterInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MirrorImage),   //怪物信息=镜像信息
                        Player = this,                                                                                //玩家=自己
                        DarkStoneStats = darkstoneStats,                                                              //元素石=定义的元素石
                        Skill = augMagic,
                        Direction = Direction,                                                                        //坐标=当前坐标
                        TameTime = SEnvir.Now.AddDays(365),                                                            //傀儡时间=365天
                    };

                    if (mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, range)))  //傀儡刷出 获取当前角色地图的随机坐标(当前角色坐标范围 格数)
                    {
                        Pets.Add(mob);                  //宠物列表增加当前傀儡
                        mob.PetOwner = this;            //当前傀儡的主人指定为自己
                    }
                }

                //最终完成魔法后再消耗元素石
                DamageDarkStone(Config.MirrorImageDamageDarkStone);    //减少该元素石的10点消耗

                LevelMagic(magic);
            }
            else
            {
                Connection.ReceiveChat("Skills.MirrorImageElement".Lang(Connection.Language, magic.Info.Name), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Skills.MirrorImageElement".Lang(con.Language, magic.Info.Name), MessageType.System);
                return;
            }
        }
        /// <summary>
        /// 冰雨
        /// </summary>
        /// <param name="magic"></param>
        /// <param name="cell"></param>
        /// <param name="power"></param>
        private void IceRainEnd(UserMagic magic, Cell cell, int power)
        {
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.IceRain) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = power,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = this,
                Effect = SpellEffect.IceRain,
                Magic = magic,
            };

            ob.Spawn(cell.Map.Info, cell.Location);
        }
    }
}
