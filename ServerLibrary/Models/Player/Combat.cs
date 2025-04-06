using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.SystemModels;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 玩家战斗过程
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        public DateTime TraOverTime;
        /// <summary>
        /// 角色攻击模式
        /// </summary>
        public AttackMode AttackMode
        {
            get { return Character.AttackMode; }
            set { Character.AttackMode = value; }
        }

        /// <summary>
        /// 角色闪避
        /// </summary>
        public override void Dodged()
        {
            base.Dodged();

            UserMagic magic;

            if (Magics.TryGetValue(MagicType.WillowDance, out magic) && Level >= magic.Info.NeedLevel1)
                LevelMagic(magic);

            #region 人物闪避事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerDodge(EventTypes.PlayerDodge,
                    new PlayerDodgeEventArgs { }));
            #endregion

            //Todo Poison Cloud
        }

        /// <summary>
        /// 施法技能过程
        /// </summary>
        public void ProcessSkill()
        {
            //以下状态不触发技能
            if (Horse != HorseType.None || Dead || Buffs.Any(x => x.Type == BuffType.DragonRepulse || x.Type == BuffType.FrostBite) ||
                (Poison & PoisonType.Paralysis) == PoisonType.Paralysis ||
                (Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike ||
                (Poison & PoisonType.Silenced) == PoisonType.Silenced) return;

            /*UserMagic magic;
            if (setConfArr[(int)AutoSetConf.SetMagicShieldBox] && Magics.TryGetValue(MagicType.MagicShield, out magic)  && !Buffs.Any(x => x.Type == BuffType.MagicShield) && magic.Cost <= CurrentMP)
            {
                MagicShieldEnd(magic);
                ChangeMP(-magic.Cost);
            }

            if (setConfArr[(int)AutoSetConf.SetRenounceBox] && Magics.TryGetValue(MagicType.Renounce, out magic)  && !Buffs.Any(x => x.Type == BuffType.Renounce) && magic.Cost <= CurrentMP)
            {
                RenounceEnd(magic);
                ChangeMP(-magic.Cost);
            }*/

            if (setConfArr[(int)AutoSetConf.SetAutoOnHookBox])
            {
                if (AutoTime > SEnvir.Now)
                {
                    long oldValue = Character.Account.AutoTime;
                    Character.Account.AutoTime = (int)(AutoTime - SEnvir.Now).TotalSeconds;
                    if (Character.Account.AutoTime != oldValue)
                        Enqueue(new S.AutoTimeChanged { AutoTime = Character.Account.AutoTime });
                }
                else
                {
                    Character.Account.AutoTime = 0;
                    setConfArr[(int)AutoSetConf.SetAutoOnHookBox] = false;
                    Enqueue(new S.AutoTimeChanged { AutoTime = Character.Account.AutoTime });
                }
            }
        }

        #region Combat
        /// <summary>
        /// 攻击位置2
        /// </summary>
        /// <param name="location">坐标</param>
        /// <param name="magics">技能</param>
        /// <param name="primary">限制</param>
        /// <returns></returns>
        public bool AttackLocation2(Point location, List<UserMagic> magics, bool primary)
        {
            Cell cell = CurrentMap.GetCell(location);

            if (cell?.Objects == null) return false;

            bool result = false;

            foreach (MapObject ob in cell.Objects)
            {
                if (!CanAttackTarget(ob)) continue;

                int delay = 300;
                foreach (UserMagic magic in magics)
                {
                    if (magic.Info.Magic == MagicType.DragonRise)
                        delay = 600;
                }

                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(delay), ActionType.AttackDelay,
                    location,
                    magics,
                    primary,
                    0));

                result = true;
                break;
            }
            return result;
        }
        /// <summary>
        /// 攻击位置
        /// </summary>
        /// <param name="location">坐标</param>
        /// <param name="magics">技能</param>
        /// <param name="primary">限制</param>
        /// <returns></returns>
        public bool AttackLocation(Point location, List<UserMagic> magics, bool primary)
        {
            Cell cell = CurrentMap.GetCell(location);

            if (cell?.Objects == null) return false;

            bool result = false;

            foreach (MapObject ob in cell.Objects)
            {

                if (!CanAttackTarget(ob) && !(ob is Guard)) continue;

                int delay = 300;
                foreach (UserMagic magic in magics)
                {
                    if (magic.Info.Magic == MagicType.DragonRise)
                        delay = 600;
                }

                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(delay), ActionType.DelayAttack,
                    ob,
                    magics,
                    primary,
                    0));

                result = true;
            }
            return result;
        }
        /// <summary>
        /// 攻击延迟
        /// </summary>
        /// <param name="location">坐标</param>
        /// <param name="magics">技能</param>
        /// <param name="primary">限制</param>
        /// <param name="extra">附加值</param>
        public void AttackDelay(Point location, List<UserMagic> magics, bool primary, int extra)
        {
            Cell cell = CurrentMap.GetCell(location);

            if (cell?.Objects == null) return;

            foreach (MapObject ob in cell.Objects)
            {
                if (!CanAttackTarget(ob)) continue;

                Attack(ob, magics, primary, 0);
            }
        }
        /// <summary>
        /// 攻击他人
        /// </summary>
        /// <param name="ob">对象</param>
        /// <param name="magics">技能</param>
        /// <param name="primary">主攻击</param>
        /// <param name="extra">附加值</param>
        public void Attack(MapObject ob, List<UserMagic> magics, bool primary, int extra)
        {
            if (ob?.Node == null || ob.Dead) return;

            for (int i = Pets.Count - 1; i >= 0; i--)
                if (Pets[i].Target == null)
                    Pets[i].Target = ob;

            int power = GetDC();          //攻击力
            int karmaDamage = 0;          //伤害值
            bool ignoreAccuracy = false, hasFlameSplash = false, hasLotus = false, hasDestructiveSurge = false;   //忽略准确  新月炎龙 莲花 十方斩 

            bool hasBladeStorm = false, hasDanceOfSallows = false, hasMaelstromBlade = false; //莲月  鹰击  屠龙斩          
            bool hasMassacre = false;                             //最后抵抗
            bool hasSwiftBlade = false, hasSeismicSlam = false;   //旋风斩 天雷锤

            bool hasThousandBlades = false;  //千刃杀风

            bool BladeStormhas = false, HalfMoonhas = false, DestructiveSurgehas = false;

            UserMagic magic;
            foreach (UserMagic mag in magics)
            {
                switch (mag.Info.Magic)
                {
                    case MagicType.BladeStorm:  //莲月
                        BladeStormhas = true;
                        break;
                    case MagicType.FullBloom:         //盛开
                    case MagicType.WhiteLotus:        //白莲
                    case MagicType.RedLotus:          //红莲
                    case MagicType.SweetBrier:        //月季
                        ignoreAccuracy = true;        //忽略准确
                        hasLotus = true;              //莲花
                        break;
                    case MagicType.SwiftBlade:       //快刀斩马
                    case MagicType.SeismicSlam:      //天雷锤
                        ignoreAccuracy = true;       //忽略准确
                        hasSwiftBlade = true;        //快刀斩马
                        break;
                    case MagicType.ThousandBlades:
                        ignoreAccuracy = true;
                        break;
                    case MagicType.FlameSplash:      //新月炎龙
                        hasFlameSplash = !primary;
                        break;
                    case MagicType.DanceOfSwallow:   //鹰击
                        hasDanceOfSallows = true;
                        break;
                    case MagicType.HalfMoon:            //半月
                        HalfMoonhas = true;
                        break;
                    case MagicType.DestructiveSurge:   //十方斩
                        hasDestructiveSurge = !primary;
                        DestructiveSurgehas = true;
                        break;
                    case MagicType.CrushingWave:
                        hasSwiftBlade = !primary;
                        break;
                }
            }

            int accuracy = Stats[Stat.Accuracy];   //准确

            int res;

            if (!ignoreAccuracy && SEnvir.Random.Next(ob.Stats[Stat.Agility]) > accuracy)   //忽视  随机准确 > 准确
            {
                ob.Dodged();    //躲避
                return;
            }

            bool hasStone = Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;   //护身符 或 暗石

            for (int i = magics.Count - 1; i >= 0; i--)
            {
                magic = magics[i];
                int bonus;
                switch (magic.Info.Magic)
                {
                    case MagicType.Slaying:                 //攻杀
                    case MagicType.CalamityOfFullMoon:     //满月恶狼
                        power += magic.GetPower() + power * ((magic.Level + 1) * 5 / 100);
                        break;
                    case MagicType.FlamingSword:            //烈火剑法
                        power = power * magic.GetPower() / 100 + power * Stats[Stat.FlamingSwordHoist] / 100;
                        break;
                    case MagicType.DragonRise:            //翔空剑法
                        power = power * magic.GetPower() / 100 + power * Stats[Stat.DragonRiseHoist] / 100;
                        break;
                    case MagicType.BladeStorm:            //莲月剑法
                        power = power * magic.GetPower() / 100 + power * Stats[Stat.BladeStormHoist] / 100;
                        hasBladeStorm = true;
                        break;
                    case MagicType.MaelstromBlade:      //屠龙斩
                        power = power * magic.GetPower() / 100;
                        hasMaelstromBlade = true;
                        break;
                    case MagicType.Thrusting:           //刺杀
                        if (primary)
                            power = power * magic.GetPower() / 100;
                        break;
                    case MagicType.HalfMoon:            //半月
                        if (!primary)
                            power = power * magic.GetPower() / 100;
                        break;
                    case MagicType.DestructiveSurge:    //十方斩
                        if (!primary)
                            power = power * magic.GetPower() / 100 + power * Stats[Stat.DestructiveSurgeHoist] / 100;
                        break;
                    case MagicType.SwiftBlade:          //快刀斩马
                        power = power * magic.GetPower() / 100 + power * Stats[Stat.SwiftBladeHoist] / 100;

                        //if (ob.Race == ObjectType.Player)
                        //    power /= 2;
                        break;
                    case MagicType.SeismicSlam:         //天雷锤
                        power = power * magic.GetPower() / 100;

                        //if (ob.Race == ObjectType.Player)
                        //    power /= 2;

                        hasSeismicSlam = true;

                        break;
                    case MagicType.ThousandBlades:
                        power = power * magic.GetPower() / 100;

                        if (ob.Race == ObjectType.Player)
                            power /= 2;

                        hasThousandBlades = true;

                        break;
                    case MagicType.CrushingWave:
                        if (!primary)
                            power = power * magic.GetPower() / 100;

                        //if (ob.Race == ObjectType.Player)
                        //    power /= 2;
                        break;
                    case MagicType.FullBloom:          //盛开
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        if (SEnvir.Random.Next(100) >= Stats[Stat.ACIgnoreRate])
                            power = Math.Max(0, power - ob.GetAC() + GetDC());
                        else
                            power += GetDC();

                        if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
                            power += Math.Max(0, bonus - ob.GetMR());
                        else
                            power += bonus;

                        power += power * Stats[Stat.FullBloomHoist] / 100;

                        if (Magics.TryGetValue(MagicType.BloodFire, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Random.Next(Config.MaxMagicLv + 3 - magic.Level) == 0)
                        {
                            power += power * (magic.Level + 1) / 10;
                            LevelMagic(magic);
                        }

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / (Config.ElementResistance * 2);
                        else if (res < 0)
                            power -= power * res / Config.ElementResistance;

                        BuffAdd(BuffType.FullBloom, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FullBloom });
                        break;
                    case MagicType.WhiteLotus:   //白莲
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        if (SEnvir.Random.Next(100) >= Stats[Stat.ACIgnoreRate])
                            power = Math.Max(0, power - ob.GetAC() + GetDC());
                        else
                            power += GetDC();

                        power += power * Stats[Stat.WhiteLotusHoist] / 100;

                        if (Buffs.Any(x => x.Type == BuffType.FullBloom))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }

                        if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
                            power += Math.Max(0, bonus - ob.GetMR());
                        else
                            power += bonus;

                        if (Magics.TryGetValue(MagicType.BloodFire, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Random.Next(Config.MaxMagicLv + 3 - magic.Level) == 0)
                        {
                            power += power * (magic.Level + 1) / 10;
                            LevelMagic(magic);
                        }

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / (Config.ElementResistance * 2);
                        else if (res < 0)
                            power -= power * res / Config.ElementResistance;

                        BuffRemove(BuffType.FullBloom);
                        BuffAdd(BuffType.WhiteLotus, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.WhiteLotus });
                        break;
                    case MagicType.RedLotus:   //红莲
                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        if (SEnvir.Random.Next(100) >= Stats[Stat.ACIgnoreRate])
                            power = Math.Max(0, power - ob.GetAC() + GetDC());
                        else
                            power += GetDC();

                        power += power * Stats[Stat.RedLotusHoist] / 100;

                        if (Buffs.Any(x => x.Type == BuffType.WhiteLotus))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }

                        if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
                            power += Math.Max(0, bonus - ob.GetMR());
                        else
                            power += bonus;

                        if (Magics.TryGetValue(MagicType.BloodFire, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Random.Next(Config.MaxMagicLv + 3 - magic.Level) == 0)
                        {
                            power += power * (magic.Level + 1) / 10;
                            LevelMagic(magic);
                        }

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / (Config.ElementResistance * 2);
                        else if (res < 0)
                            power -= power * res / Config.ElementResistance;

                        BuffRemove(BuffType.WhiteLotus);
                        BuffAdd(BuffType.RedLotus, TimeSpan.FromSeconds(15), null, false, false, TimeSpan.Zero);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.RedLotus });
                        break;
                    case MagicType.SweetBrier:   //月季

                        bonus = GetLotusMana(ob.Race) * magic.GetPower() / 1000;

                        if (SEnvir.Random.Next(100) >= Stats[Stat.ACIgnoreRate])
                            power = Math.Max(0, power - ob.GetAC() + GetDC());
                        else
                            power += GetDC();

                        power += power * Stats[Stat.SweetBrierHoist] / 100;

                        if (Buffs.Any(x => x.Type == BuffType.RedLotus))
                        {
                            bonus *= 3;
                            power += Math.Max(0, Stats[Stat.MaxDC] - 100);
                        }

                        if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
                            power += Math.Max(0, bonus - ob.GetMR());
                        else
                            power += bonus;

                        if (Magics.TryGetValue(MagicType.BloodFire, out magic) && Level >= magic.Info.NeedLevel1 && SEnvir.Random.Next(Config.MaxMagicLv + 3 - magic.Level) == 0)
                        {
                            power += power * (magic.Level + 1) / 10;
                            LevelMagic(magic);
                        }

                        if (ob.Race == ObjectType.Player)
                            res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                        else
                            res = ob.Stats.GetResistanceValue(Element.None);

                        if (res > 0)
                            power -= power * res / (Config.ElementResistance * 2);
                        else if (res < 0)
                            power -= power * res / Config.ElementResistance;

                        BuffRemove(BuffType.RedLotus);
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.SweetBrier });
                        break;
                    case MagicType.Karma:    //孽报
                        power += GetDC();

                        karmaDamage = ob.CurrentHP * magic.GetPower() / 100;

                        if (ob.Race == ObjectType.Monster)
                        {
                            if (((MonsterObject)ob).MonsterInfo.IsBoss)
                                karmaDamage = magic.GetPower() * 20;
                            else
                                karmaDamage /= 4;
                        }

                        /* BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);
                         if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out magic) && Level >= magic.Info.NeedLevel1)
                         {
                             power += power * magic.GetPower() / 100;
                             magics.Add(magic);
                             BuffRemove(buff);
                             if (buff.Stats[Stat.TheNewBeginning] > 1)
                                 BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                         }*/

                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.Karma });
                        break;
                    case MagicType.FlameSplash:  //新月炎龙
                        if (!primary)
                            power = power * magic.GetPower() / 100;

                        break;
                    case MagicType.DanceOfSwallow:  //鹰击
                        power += GetDC();
                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.DanceOfSwallow });
                        break;
                    case MagicType.Massacre:
                        hasMassacre = true;
                        break;
                }
            }

            CurrentMagic = magics.Count > 0 ? magics[0].Info.Magic : MagicType.None;

            Element element = Element.None;  //元素为空
            int maxElementAttack = 0;

            if (!hasMassacre)    //没最后抵抗
            {
                if (!hasLotus)   //没莲花
                {
                    if (SEnvir.Random.Next(100) >= Stats[Stat.ACIgnoreRate])
                    {
                        if (ob.Race == ObjectType.Player && !Config.CriticalDamagePVP)      //PVP降低伤害
                        {
                            power = (power * 100 - ob.GetAC() * 130) / 100;
                        }
                        else
                            power -= ob.GetAC();  //伤害值 -= 防御值
                    }

                    if (ob.Race == ObjectType.Player)  //如果 （对象是玩家）
                        //res = 统计数据.获取抵抗值（检查石头 ？ 护身符 元素值为空）
                        res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                    else //否则
                        //res = 统计数据.获取抵抗值（元素为空）
                        res = ob.Stats.GetResistanceValue(Element.None);

                    if (res > 0)  //如果 值大于0
                        power -= power * res / (Config.ElementResistance * 2);  //伤害值 -= 伤害值 * 定义值/10
                    else if (res < 0)  //否则 如果 （值小于0）
                        power -= power * res / Config.ElementResistance;  //伤害值 -= 伤害值 * 定义值/5
                }

                if (power < 0) power = 0; //如果伤害值小于0 那么伤害值等0

                //元素伤害
                for (Element ele = Element.Fire; ele <= Element.Phantom; ele++)
                {
                    if (hasFlameSplash && ele > Element.Fire) break;

                    int value = Stats.GetElementValue(ele);
                    if (value == 0) continue;
                    if (hasStone)
                    {
                        value += Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityValue(ele);
                        //element = ele;
                    }
                    int value1 = ob.Stats.GetResistanceValue(ele);

                    if (maxElementAttack < value - value1)
                    {
                        maxElementAttack = value - value1;
                        element = ele;
                    }
                    //element = value > maxElementAttack ? ele : element;
                    //maxElementAttack = Math.Max(maxElementAttack, value);
                }

                power += maxElementAttack;

                res = ob.Stats.GetResistanceValue(element);

                if (res <= 0)
                    power -= maxElementAttack * res * 3 / (Config.ElementResistance * 2);
                else
                    power -= maxElementAttack * res * 2 / (Config.ElementResistance * 2);

                if (hasStone && (!hasFlameSplash || element == Element.Fire))
                    DamageDarkStone();

                if (hasFlameSplash)
                    element = Element.Fire;
            }
            else
            {
                power = extra;  //伤害值=额外伤害

                if (ob.Race == ObjectType.Player)
                    res = ob.Stats.GetResistanceValue(hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None);
                else
                    res = ob.Stats.GetResistanceValue(Element.None);

                if (res > 0)
                    power -= power * res / (Config.ElementResistance * 2);
                else if (res < 0)
                    power -= power * res / Config.ElementResistance;
            }

            if (Stats[Stat.FinalDamageAddRate] > 0)  //最终伤害增加百分比
            {
                power += power * Stats[Stat.FinalDamageAddRate] / 100;
            }

            //当前所在地图是竞技场
            if (CurrentMap.Info.CanPlayName == true)
            {
                //战士职业降低伤害设置
                if (Class == MirClass.Warrior)
                {
                    power -= power * Config.WarriorReductionDamage / 100;
                }

                //等级差压制降低对应的伤害
                if (Level > ob.Level && ob.Race == ObjectType.Player)
                {
                    power -= power * Config.LevelReductionDamage / 100 * Math.Min(Config.CompareLevelValues, Level - ob.Level);
                }
            }

            if (power <= 0)
            {
                ob.Blocked();  //ob被阻止
                return;
            }

            int damage = 0;
            if (hasBladeStorm)
            {
                power /= 2;
                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayedAttackDamage, ob, power, element, true, false, ob.Stats[Stat.MagicShield] == 0, true));
            }

            if (hasMaelstromBlade)
            {
                power /= 2;
                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayedAttackDamage, ob, power, element, true, false, ob.Stats[Stat.MagicShield] == 0, true));
            }

            if (karmaDamage > 0)
                damage += ob.Attacked(this, karmaDamage, Element.None, false, true, false);

            damage += ob.Attacked(this, power, element, true, false, !hasMassacre);

            if (damage <= 0) return;

            CheckBrown(ob);

            DamageItem(GridType.Equipment, (int)EquipmentSlot.Weapon, SEnvir.Random.Next(2) + 1);
            if (hasDanceOfSallows && ob.Level < Level)
            {
                magic = magics.FirstOrDefault(x => x.Info.Magic == MagicType.DanceOfSwallow);

                if (Config.DanceOfSwallowSilenced)
                {
                    ob.ApplyPoison(new Poison
                    {
                        Type = PoisonType.Silenced,
                        TickCount = 1,
                        Owner = this,
                        TickFrequency = TimeSpan.FromSeconds(magic.GetPower() + 1),
                    });
                }

                if (Config.DanceOfSwallowParalysis)
                {
                    ob.ApplyPoison(new Poison
                    {
                        Owner = this,
                        Type = PoisonType.Paralysis,
                        TickFrequency = TimeSpan.FromSeconds(1),
                        TickCount = 1,
                    });
                }
            }

            //if (Buffs.Any(x => x.Type == BuffType.Might) && Magics.TryGetValue(MagicType.Might, out magic))
            //    LevelMagic(magic);

            decimal lifestealAmount = damage * Stats[Stat.LifeSteal] / 100M;

            if (hasSwiftBlade)        //快刀斩马
            {
                lifestealAmount = Math.Min(lifestealAmount, 2000 - SwiftBladeLifeSteal);
                SwiftBladeLifeSteal = Math.Min(2000, SwiftBladeLifeSteal + lifestealAmount);
            }

            if (hasFlameSplash)     //新月炎龙爆
            {
                lifestealAmount = Math.Min(lifestealAmount, 750 - FlameSplashLifeSteal);
                FlameSplashLifeSteal = Math.Min(750, FlameSplashLifeSteal + lifestealAmount);
            }
            if (hasDestructiveSurge)   //十方斩
            {
                lifestealAmount = Math.Min(lifestealAmount, 750 - DestructiveSurgeLifeSteal);
                DestructiveSurgeLifeSteal = Math.Min(750, DestructiveSurgeLifeSteal + lifestealAmount);
            }

            if ((primary || Class == MirClass.Warrior || hasFlameSplash) && lifestealAmount > 1)   //技能限制  或 战士
                LifeSteal += lifestealAmount;

            if (LifeSteal > 1)      //吸血
            {
                int heal = (int)Math.Floor(LifeSteal);
                LifeSteal -= heal;

                if (HalfMoonhas) heal = Math.Min(30, heal);   //如果是半月 那么最大值30点

                if (DestructiveSurgehas) heal = Math.Min(20, heal);   //如果是十方 那么最大值15点

                if (BladeStormhas) heal = heal * 2;  //如果是莲月 那么加2次

                ChangeHP(Math.Min((hasLotus ? 1500 : 750), heal));   //如果是定义 最大1500  否则最大值750
                                                                     //DisplayLifeSteal = true;
            }

            //  if (primary)

            int psnRate = 200;

            if (ob.Level >= 250)
                psnRate = 2000;

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.ParalysisChance] - ob.Stats[Stat.ParalysisChanceResistance]) || hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    TickCount = 1,
                });
            }

            if (hasThousandBlades)
            {
                ob.ApplyPoison(new Poison
                {
                    Type = PoisonType.ThousandBlades,
                    Owner = this,
                    TickCount = 3,
                    TickFrequency = TimeSpan.FromSeconds(2),//这个时间可以修改
                    Value = GetDC(),
                });
            }

            if (hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Type = PoisonType.WraithGrip,
                    Owner = this,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromMilliseconds(1500),
                });
            }

            if (ob.Race != ObjectType.Player && SEnvir.Random.Next(psnRate) < Stats[Stat.SlowChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Slow,
                    Value = 20,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.SilenceChance] - ob.Stats[Stat.SilenceChanceResistance]) || hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Silenced,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.AbyssChance] - ob.Stats[Stat.AbyssChanceResistance]) || hasSeismicSlam)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Abyss,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.StunnedStrikeChance] - ob.Stats[Stat.StunnedStrikeChanceResistance]))
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.StunnedStrike,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    TickCount = 1,
                });
            }

            foreach (UserMagic mag in magics)  //遍历 （用户技能 技能）
            {
                switch (mag.Info.Magic)
                {
                    case MagicType.Swordsmanship:
                    case MagicType.Slaying:
                    case MagicType.Thrusting:
                    case MagicType.PledgeOfBlood:
                    case MagicType.Rake:
                    case MagicType.Karma:
                        LevelMagic(mag);
                        break;
                }
            }

            if (Config.ZZDKZ)  //最终抵抗技能设置
            {
                if (ob.Dead && ob.Race == ObjectType.Monster && ob.CurrentHP < 0 && !hasThousandBlades) //如果 （ob死亡 并且 ob对象是怪物 并且 当前血量小于0 并且 不是千刃杀风）
                {
                    if (Magics.TryGetValue(MagicType.Massacre, out magic) && Level >= magic.Info.NeedLevel1) //如果 （技能是刺客最终抵抗 并且 技能等级 >= 技能的等级1）
                    {
                        magics.Add(magic); //增加技能
                        LevelMagic(magic);
                    }

                    if (magic != null) //如果 技能 不为 空
                    {
                        power = Math.Abs(ob.CurrentHP) * magic.GetPower() / 100; //伤害 = 绝对值（当前血量）* 技能的攻击值 /100

                        foreach (MapObject target in GetTargets(CurrentMap, ob.CurrentLocation, Config.ZHDKFW)) //遍历 （在获取目标中映射对象目标（当前地图，当前位置，半径2））
                        {
                            if (target.Race != ObjectType.Monster) continue; //如果 （攻击对象 不等于 怪物） 继续

                            MonsterObject mob = (MonsterObject)target; //怪物对象定义 为目标

                            if (mob.MonsterInfo.IsBoss) continue; //如果定义目标是BOSS 跳过

                            //操作列表添加（新的延迟动作（延迟600毫秒），动作类型.延迟攻击）
                            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(600), ActionType.DelayAttack,
                                target,
                                magics,
                                false,
                                power));
                        }
                    }
                }
            }
            else
            {
                if (Dead) return;
                if (CurrentHP < (long)(Stats[Stat.Health] * ((100 - Config.ZHDKHP - SEnvir.Random.Next(6)) / 100D)))  //如果 （当前血量 小于 （人物最大血量的10%-15%））
                {
                    if (Magics.TryGetValue(MagicType.Massacre, out magic) && Level >= magic.Info.NeedLevel1)  //如果 （技能是刺客最终抵抗 并且 技能等级 >= 技能的等级1）
                        magics.Add(magic);  //增加技能

                    if (magic != null)   //如果 技能 不为 空
                    {
                        power = Math.Abs(CurrentHP) * magic.GetPower() / 100;  //伤害 = 绝对值（当前血量）* 技能的攻击值 /100

                        foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, Config.ZHDKFW))  //遍历 （在获取目标中映射对象目标（当前地图，当前位置，半径2））
                        {
                            if (target.Race != ObjectType.Monster) continue;  //如果 （攻击对象 不等于 怪物） 继续

                            MonsterObject mob = (MonsterObject)target;  //怪物对象定义 为目标

                            if (mob.MonsterInfo.IsBoss) continue;  //如果定义目标是BOSS 跳过

                            //操作列表添加（新的延迟动作（延迟600毫秒），动作类型.延迟攻击）
                            ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(600), ActionType.DelayAttack,
                                target,  //目标
                                magics,  //技能
                                false,   //假
                                power)); //伤害
                        }
                    }
                }
            }

            // 攻击带毒
            // 这里假设GreenPoison的值是上毒的概率, 上限100, 即刀刀带毒, 下限0, 必然不带毒
            if (SEnvir.Random.Next(100) < Stats[Stat.GreenPoison])//绿毒
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Value = 4,//每次飘毒掉血的值
                    Type = PoisonType.Green,
                    TickFrequency = TimeSpan.FromSeconds(1),//每1秒飘毒1次
                    TickCount = 5,//飘多少次毒
                });
            }
            // 这里假设RedPoison的值是上毒的概率, 上限100, 即刀刀带毒, 下限0, 必然不带毒
            if (SEnvir.Random.Next(100) < Stats[Stat.RedPoison]) //红毒
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Value = 4,//这个值对于红毒没有实际作用
                    Type = PoisonType.Red,
                    TickFrequency = TimeSpan.FromSeconds(1),//这个值对于红毒没有实际作用
                    TickCount = 5,//飘多少次毒
                });
            }

            //todo 何时计算damage？

            #region 人物平A或者使用技能平A

            if (magics.Count > 0)
            {
                //队列一个事件, 不要忘记添加listener
                SEnvir.EventManager.QueueEvent(
                        new PlayerMagic(EventTypes.PlayerUseMagic,
                            new PlayerMagicEventArgs { target = ob, magic = CurrentMagic, element = element, damage = damage }));
            }
            else
            {
                //队列一个事件, 不要忘记添加listener
                SEnvir.EventManager.QueueEvent(
                    new PlayerAttack(EventTypes.PlayerAttack,
                        new PlayerAttackEventArgs { target = ob, element = element, damage = damage }));
            }

            //python 触发
            try
            {
                string magicName = magics.Count > 0 ? magics[0].Info.Name : "普通攻击";
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, magicName, ob, damage });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnPlayerAttack", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            #endregion

            #region 人物吸血

            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerLifeSteal(EventTypes.PlayerLifeSteal,
                    new PlayerLifeStealEventArgs { target = ob, amount = lifestealAmount }));

            #endregion
        }

        /// <summary>
        /// 魔法攻击
        /// </summary>
        /// <param name="magics">魔法技能</param>
        /// <param name="ob">攻击对象</param>
        /// <param name="primary">基础</param>
        /// <param name="stats">元素属性</param>
        /// <param name="extra">额外</param>
        /// <returns></returns>
        public int MagicAttack(List<UserMagic> magics, MapObject ob, bool primary, Stats stats = null, int extra = 0)
        {
            if (ob?.Node == null || ob.Dead || !CanAttackTarget(ob)) return 0;

            if (PetMode == PetMode.PvP)
            {
                for (int i = Pets.Count - 1; i >= 0; i--)
                    if (Pets[i].CanAttackTarget(ob))
                        Pets[i].Target = ob;
            }
            else
                for (int i = Pets.Count - 1; i >= 0; i--)
                    if (Pets[i].Target == null)
                        Pets[i].Target = ob;

            Element element = Element.None;
            int slow = 0, slowLevel = 0, repel = 0, silence = 0;

            bool canStuck = true;

            int power = 0;
            UserMagic asteroid = null;

            foreach (UserMagic magic in magics)
            {
                CurrentMagic = magic.Info.Magic;
                switch (magic.Info.Magic)
                {
                    case MagicType.FireBall:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.FireBallHoist] / 100;
                        break;
                    case MagicType.ScortchedEarth:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.ScortchedEarthHoist] / 100;
                        break;
                    case MagicType.FireStorm:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.FireStormHoist] / 100;
                        break;
                    case MagicType.AdamantineFireBall:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.AdamantineFireBallHoist] / 100;
                        break;
                    case MagicType.MeteorShower:
                        element = Element.Fire;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.MeteorShowerHoist] / 100;
                        break;
                    case MagicType.Asteroid:
                        element = Element.Fire;
                        asteroid = magic;
                        canStuck = false;
                        break;
                    case MagicType.FireWall:
                        element = Element.Fire;
                        power += (magic.GetPower() + GetMC() + power * Stats[Stat.FireWallHoist] / 100) * 120 / 100;
                        canStuck = false;
                        break;
                    case MagicType.HellFire:
                        element = Element.Fire;
                        power += magic.GetPower() + GetDC();
                        break;
                    case MagicType.IceBolt:
                        slowLevel = 3;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.IceBoltHoist] / 100;
                        slow = 10;
                        break;
                    case MagicType.FrozenEarth:
                        slowLevel = 3;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC();
                        slow = 10;
                        break;
                    case MagicType.IceBlades:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.IceBladesHoist] / 100;
                        slow = 5;
                        break;
                    case MagicType.GreaterFrozenEarth:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.GreaterFrozenEarthHoist] / 100;
                        slow = 5;
                        break;
                    case MagicType.IceStorm:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.IceStormHoist] / 100;
                        slow = 5;
                        break;
                    case MagicType.IceRain:
                        element = Element.Ice;
                        power += magic.GetPower() + GetMC();
                        silence = 4;
                        break;
                    case MagicType.FrostBite:
                        slowLevel = 5;
                        element = Element.Ice;
                        power += Math.Min(stats[Stat.FrostBiteDamage], stats[Stat.FrostBiteMaxDamage]) - Stats[Stat.IceAttack] * 2;
                        slow = 5;
                        break;
                    case MagicType.LightningBall:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.LightningBallHoist] / 100;
                        break;
                    case MagicType.ThunderBolt:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.ThunderBoltHoist] / 100;
                        break;
                    case MagicType.LightningWave:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.LightningWaveHoist] / 100;
                        break;
                    case MagicType.LightningBeam:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.LightningBeamHoist] / 100;
                        break;
                    case MagicType.ThunderStrike:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.ThunderStrikeHoist] / 100;
                        power += power / 2;
                        break;
                    case MagicType.ChainLightning:
                        element = Element.Lightning;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.ChainLightningHoist] / 100;

                        power = power * 5 / (extra + 5);
                        break;
                    case MagicType.GustBlast:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.GustBlastHoist] / 100;
                        repel = 8;
                        break;
                    case MagicType.BlowEarth:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.BlowEarthHoist] / 100;
                        repel = 8;
                        break;
                    case MagicType.Cyclone:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.CycloneHoist] / 100;
                        repel = 3;
                        break;
                    case MagicType.DragonTornado:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC() + power * Stats[Stat.DragonTornadoHoist] / 100;
                        repel = 6;
                        break;
                    case MagicType.Tempest:
                        element = Element.Wind;
                        power += magic.GetPower() + GetMC();
                        repel = 3;
                        canStuck = false;
                        break;
                    case MagicType.ElementalHurricane:
                        bool hasStone = Equipment[(int)EquipmentSlot.Amulet]?.Info.ItemType == ItemType.DarkStone;
                        element = hasStone ? Equipment[(int)EquipmentSlot.Amulet].Info.Stats.GetAffinityElement() : Element.None;
                        power += magic.GetPower() + GetMC();
                        break;

                    case MagicType.ExplosiveTalisman:
                        element = Element.Dark;
                        power += magic.GetPower() + GetSC() + power * Stats[Stat.ExplosiveTalismanHoist] / 100;
                        break;
                    case MagicType.ImprovedExplosiveTalisman:
                        element = Element.Dark;
                        power += magic.GetPower() + GetSC() + power * Stats[Stat.ImprovedExplosiveTalismanHoist] / 100;
                        //power += power;
                        break;
                    case MagicType.EvilSlayer:
                        element = Element.Holy;
                        power += magic.GetPower() + GetSC() + power * Stats[Stat.EvilSlayerHoist] / 100;
                        break;
                    case MagicType.GreaterEvilSlayer:
                        element = Element.Holy;
                        power += magic.GetPower() + GetSC();
                        break;
                    case MagicType.GreaterHolyStrike:
                        element = Element.Holy;
                        power += magic.GetPower() + GetSC() + power * Stats[Stat.GreaterEvilSlayerHoist] / 100;
                        break;
                    case MagicType.DarkSoulPrison:
                        element = Element.Dark;
                        power += magic.GetPower() + GetSC();
                        break;
                    case MagicType.SummonPuppet:
                        element = Element.Fire;
                        power += GetDC() * magic.GetPower() / 100;
                        break;
                    case MagicType.Rake:
                        element = Element.Ice;
                        power += GetDC() * magic.GetPower() / 100;
                        slow = 1;
                        slowLevel = 10;
                        break;
                    case MagicType.DragonRepulse:
                        element = Element.Lightning;
                        power = GetDC() * magic.GetPower() / 100 + Level + power * Stats[Stat.DragonRepulseHoist] / 100;

                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }
                        break;
                    case MagicType.FlashOfLight:
                        element = Element.None;
                        power = GetDC() * magic.GetPower() / 100 + power * Stats[Stat.FlashOfLightHoist] / 100;

                        BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.TheNewBeginning);
                        UserMagic augMagic;

                        if (buff != null && Magics.TryGetValue(MagicType.TheNewBeginning, out augMagic) && Level >= augMagic.Info.NeedLevel1)
                        {
                            power *= 2;
                            LevelMagic(augMagic);
                            BuffRemove(buff);
                            if (buff.Stats[Stat.TheNewBeginning] > 1)
                                BuffAdd(BuffType.TheNewBeginning, TimeSpan.FromMinutes(1), new Stats { [Stat.TheNewBeginning] = buff.Stats[Stat.TheNewBeginning] - 1 }, false, false, TimeSpan.Zero);
                        }

                        ob.Broadcast(new S.ObjectEffect { ObjectID = ob.ObjectID, Effect = Effect.FlashOfLight });
                        break;
                    /*case MagicType.TheNewBeginning:
                        power += 2;
                        break;*/
                    case MagicType.ElementalPuppet:
                        if (stats.Count == 0) break;

                        foreach (KeyValuePair<Stat, int> s in stats.Values)
                        {
                            switch (s.Key)
                            {
                                case Stat.FireAffinity:
                                    element = Element.Fire;
                                    power += s.Value;
                                    break;
                                case Stat.IceAffinity:
                                    element = Element.Ice;
                                    power += s.Value;
                                    slow = 2;
                                    slowLevel = 3;
                                    break;
                                case Stat.LightningAffinity:
                                    element = Element.Lightning;
                                    power += s.Value;
                                    break;
                                case Stat.WindAffinity:
                                    element = Element.Wind;
                                    power += s.Value;
                                    repel = 2;
                                    silence = 4;
                                    break;
                                case Stat.HolyAffinity:
                                    element = Element.Holy;
                                    power += s.Value;
                                    break;
                                case Stat.DarkAffinity:
                                    element = Element.Dark;
                                    power += s.Value;
                                    break;
                                case Stat.PhantomAffinity:
                                    element = Element.Phantom;
                                    power += s.Value;
                                    break;
                            }
                        }
                        LevelMagic(magic);
                        break;
                    case MagicType.DemonExplosion:
                        power = extra;
                        element = Element.Phantom;
                        break;
                    case MagicType.MirrorImage:
                        element = Element.Phantom;
                        power += GetDC() * magic.GetPower() / 100;
                        break;
                    case MagicType.SwordOfVengeance:
                        element = Element.Lightning;
                        power = GetDC() * magic.GetPower() / 100;
                        break;
                }
            }

            foreach (UserMagic magic in magics)
            {
                switch (magic.Info.Magic)
                {
                    //技能释放时，周边伤害设置
                    case MagicType.ScortchedEarth:
                    case MagicType.LightningBeam:
                    case MagicType.BlowEarth:
                    case MagicType.FrozenEarth:
                    case MagicType.ElementalHurricane:
                        if (!primary)
                            power = (int)(power * 0.3F);
                        break;
                    case MagicType.GreaterFrozenEarth:
                        if (!primary)
                            power = (int)(power * 0.6F);
                        break;
                    case MagicType.FireWall:
                        power = (int)(power * 0.60F);
                        break;
                    case MagicType.Tempest:
                        power = (int)(power * 0.80F);
                        break;
                    case MagicType.IceRain:
                        power = (int)(power * 0.80F);
                        break;
                    case MagicType.ExplosiveTalisman:
                        if (stats != null && stats[Stat.DarkAffinity] >= 1)
                        {
                            if ((ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.Undead) || ob.Race == ObjectType.Player)
                                power += (int)(power * Config.ETDarkAffinityRate);
                        }

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }
                        break;
                    case MagicType.ImprovedExplosiveTalisman:
                        if (stats != null && stats[Stat.DarkAffinity] >= 1)
                        {
                            if (ob.Race == ObjectType.Monster && !((MonsterObject)ob).MonsterInfo.Undead)
                                power += (int)(power * 0.6F);
                        }

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }
                        break;
                    case MagicType.DarkSoulPrison:
                        power = (int)(power * 0.40F);
                        break;
                    case MagicType.EvilSlayer:
                        if (stats != null && stats[Stat.HolyAffinity] >= 1)
                        {
                            if (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.Undead)
                                power += (int)(power * 0.3F);
                        }

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }
                        break;
                    case MagicType.GreaterEvilSlayer:
                        if (stats != null && stats[Stat.HolyAffinity] >= 1)
                        {
                            if (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.Undead)
                                power += (int)(power * 0.4F);
                        }

                        if (!primary)
                        {
                            power = (int)(power * 0.65F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.5F);
                        }
                        break;
                    case MagicType.GreaterHolyStrike:
                        if (stats != null && stats[Stat.HolyAffinity] >= 1)
                        {
                            if (ob.Race == ObjectType.Monster && ((MonsterObject)ob).MonsterInfo.Undead)
                                power += (int)(power * 1.4F);
                        }

                        if (!primary)
                        {
                            power = (int)(power * 1.45F);
                            //  if (ob.Race == ObjectType.Player)
                            //      power = (int)(power * 0.6F);
                        }

                        break;
                }
            }

            if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
            {
                if (ob.Race == ObjectType.Player && !Config.CriticalDamagePVP)      //PVP降低伤害
                {
                    power = (power * 100 - ob.GetMR() * 130) / 100;
                }
                else
                    power -= ob.GetMR();  //伤害值 -= 魔御值
            }

            /* if (Buffs.Any(x => x.Type == BuffType.Renounce))
             {
                 if (ob.Race == ObjectType.Player)
                     power += ob.Stats[Stat.Health] * (1 + (Math.Min(4000, ob.Stats[Stat.Health]) / 2000)) / 100;
             }*/

            switch (element)
            {
                case Element.None:
                    if (Config.PhysicalResistanceSwitch)
                    {
                        power -= power * ob.Stats[Stat.PhysicalResistance] / (Config.ElementResistance * 2);
                    }
                    break;
                case Element.Fire:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.FireAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的火元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.FireAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的火元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.FireAttack)) * 100 / 100;  //伤害+= 获取的火元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.FireAttack) * 3 / 2;  //伤害+= 获取的火元素值 * 2
                    }
                    power -= power * ob.Stats[Stat.FireResistance] / (Config.ElementResistance * 2);     //伤害-= 伤害*攻击对象的强元素/10                    
                    break;
                case Element.Ice:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.IceAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的冰元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.IceAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的冰元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.IceAttack)) * 100 / 100;  //伤害+= 获取的冰元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.IceAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.IceResistance] / (Config.ElementResistance * 2);
                    break;
                case Element.Lightning:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.LightningAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的雷元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.LightningAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的雷元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.LightningAttack)) * 100 / 100;  //伤害+= 获取的雷元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.LightningAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.LightningResistance] / (Config.ElementResistance * 2);
                    break;
                case Element.Wind:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.WindAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的风元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.WindAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的风元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.WindAttack)) * 100 / 100;  //伤害+= 获取的风元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.WindAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.WindResistance] / (Config.ElementResistance * 2);
                    break;
                case Element.Holy:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.HolyAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的神圣元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.HolyAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的神圣元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.HolyAttack)) * 100 / 100;  //伤害+= 获取的神圣元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.HolyAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.HolyResistance] / (Config.ElementResistance * 2);
                    break;
                case Element.Dark:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.DarkAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的暗黑元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.DarkAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的暗黑元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.DarkAttack)) * 100 / 100;  //伤害+= 获取的暗黑元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.DarkAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.DarkResistance] / (Config.ElementResistance * 2);
                    break;
                case Element.Phantom:
                    if (ob.Race == ObjectType.Player)
                    {
                        if (Class == MirClass.Wizard)  //法师职业
                            power = (power + GetElementPower(ob.Race, Stat.PhantomAttack)) * Config.WizardMagicAttackRate / 100;  //伤害+= 获取的幻影元素值 * 2
                        else if (Class == MirClass.Taoist)  //道士职业
                            power = (power + GetElementPower(ob.Race, Stat.PhantomAttack)) * Config.TaoistMagicAttackRate / 100;  //伤害+= 获取的幻影元素值 * 2
                        else  //其他职业
                            power = (power + GetElementPower(ob.Race, Stat.PhantomAttack)) * 100 / 100;  //伤害+= 获取的幻影元素值 * 2
                    }
                    else
                    {
                        power += GetElementPower(ob.Race, Stat.PhantomAttack) * 3 / 2;
                    }
                    power -= power * ob.Stats[Stat.PhantomResistance] / (Config.ElementResistance * 2);
                    break;
            }

            if (asteroid != null)
            {
                power += asteroid.GetPower() + GetMC();
            }

            if (Stats[Stat.FinalDamageAddRate] > 0)  //最终伤害增加百分比
            {
                power += power * Stats[Stat.FinalDamageAddRate] / 100;
            }

            //当前所在地图是竞技场
            if (CurrentMap.Info.CanPlayName == true)
            {
                //战士职业降低伤害设置
                if (Class == MirClass.Warrior)
                {
                    power -= power * Config.WarriorReductionDamage / 100;
                }

                //等级差压制降低对应的伤害
                if (Level > ob.Level && ob.Race == ObjectType.Player)
                {
                    power -= power * Config.LevelReductionDamage / 100 * Math.Min(Config.CompareLevelValues, Level - ob.Level);
                }
            }

            if (power <= 0)
            {
                ob.Blocked();
                return 0;
            }

            if (SEnvir.Random.Next(ob.Race == ObjectType.Player ? 300 : 200) < ob.Stats[Stat.MagicEvade])  // 魔法躲避
            {
                ob.Dodged();
                return 0;
            }

            int damage = ob.Attacked(this, power, element, false, false, true, canStuck);

            if (damage <= 0) return damage;

            int psnRate = 100;

            if (ob.Level >= 250)
                psnRate = 1000;
            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.ParalysisChance] - ob.Stats[Stat.ParalysisChanceResistance]))
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Paralysis,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 1,
                });
            }

            if (ob.Race != ObjectType.Player && SEnvir.Random.Next(psnRate) < Stats[Stat.SlowChance])
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Slow,
                    Value = 20,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.SilenceChance] - ob.Stats[Stat.SilenceChanceResistance]))
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Silenced,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.AbyssChance] - ob.Stats[Stat.AbyssChanceResistance]))
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Abyss,
                    TickFrequency = TimeSpan.FromSeconds(5),
                    TickCount = 1,
                });
            }

            if (SEnvir.Random.Next(psnRate) < Math.Max(0, Stats[Stat.StunnedStrikeChance] - ob.Stats[Stat.StunnedStrikeChanceResistance]))
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.StunnedStrike,
                    TickFrequency = TimeSpan.FromSeconds(3),
                    TickCount = 1,
                });
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    /*   if (slow > 0 && SEnvir.Random.Next(slow) == 0 && Level > ob.Level)
                       {
                           TimeSpan duration = TimeSpan.FromSeconds(3 + SEnvir.Random.Next(3));
                           if (ob.Race == ObjectType.Monster)
                           {
                               slowLevel *= 2;
                               duration += duration;
                           }


                           ob.ApplyPoison(new Poison
                           {
                               Type = PoisonType.Slow,
                               Value = slowLevel,
                               TickCount = 1,
                               TickFrequency = duration,
                               Owner = this,
                           });*/
                    /*

                    if (repel > 0 && ob.CurrentMap == CurrentMap && Level  > ob.Level && SEnvir.Random.Next(repel) == 0)
                {
                    MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                    if (ob.Pushed(dir, 1) == 0)
                    {
                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }
                    }*/
                    break;
                case ObjectType.Monster:
                    //技能冰冻设置大于0      随机冰冻阈值等0    不是BOSS    可以被冰冻
                    if (slow > 0 && SEnvir.Random.Next(slow) == 0 && !((MonsterObject)ob).MonsterInfo.IsBoss && !((MonsterObject)ob).MonsterInfo.IsSlow)
                    {
                        TimeSpan duration = TimeSpan.FromSeconds(3 + SEnvir.Random.Next(3));

                        slowLevel *= 2;
                        duration += duration;

                        ob.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Slow,
                            Value = slowLevel,
                            TickCount = 1,
                            TickFrequency = duration,
                            Owner = this,
                        });
                    }

                    if (repel > 0 && ob.CurrentMap == CurrentMap && Level > ob.Level && SEnvir.Random.Next(repel) == 0 && ob.Race != ObjectType.Player)
                    {
                        MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                        if (ob.Pushed(dir, 1) == 0)
                        {
                            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                            for (int i = 1; i < 2; i++)
                            {
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                                if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                            }
                        }
                    }

                    if (silence > 0 && !((MonsterObject)ob).MonsterInfo.IsBoss)
                    {
                        ob.ApplyPoison(new Poison
                        {
                            Type = PoisonType.Silenced,
                            Value = slowLevel,
                            TickCount = 1,
                            TickFrequency = TimeSpan.FromSeconds(silence),
                            Owner = this,
                        });
                    }
                    break;
            }

            CheckBrown(ob);

            foreach (UserMagic magic in magics)
            {
                switch (magic.Info.Magic)
                {
                    case MagicType.FireBall:
                        if (Class == MirClass.Wizard)
                            LevelMagic(magic);
                        break;
                    case MagicType.LightningBall:
                    case MagicType.IceBolt:
                    case MagicType.GustBlast:
                    case MagicType.AdamantineFireBall:
                    case MagicType.ThunderBolt:
                    case MagicType.IceBlades:
                    case MagicType.Cyclone:
                    case MagicType.ExplosiveTalisman:
                    case MagicType.EvilSlayer:
                    case MagicType.GreaterEvilSlayer:
                    case MagicType.GreaterHolyStrike:
                    case MagicType.ImprovedExplosiveTalisman:
                        LevelMagic(magic);  //魔法攻击就加技能经验
                        break;
                }
            }

            UserMagic temp;
            //if (Buffs.Any(x => x.Type == BuffType.Renounce) && Magics.TryGetValue(MagicType.Renounce, out temp))
            //{
            //    LevelMagic(temp);
            //}

            if (Magics.TryGetValue(MagicType.AdvancedRenounce, out temp))
                LevelMagic(temp);

            //python 触发
            try
            {
                string magicName = magics.Count > 0 ? magics[0].Info.Name : "普通攻击";
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, magicName, ob, damage });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnPlayerAttack", args);
                    //trig_player(this, "OnPlayerAttack", args);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            return damage;
        }

        /// <summary>
        /// 角色被攻击时
        /// </summary>
        /// <param name="attacker">攻击对象</param>
        /// <param name="power">攻击值</param>
        /// <param name="element">元素</param>
        /// <param name="canReflect">反射</param>
        /// <param name="ignoreShield">忽略盾</param>
        /// <param name="canCrit">可以暴击</param>
        /// <param name="canStruck">可以攻击</param>
        /// <returns></returns>
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker?.Node == null || power == 0 || Dead || attacker.CurrentMap != CurrentMap ||
                (!Functions.InRange(attacker.CurrentLocation, CurrentLocation, Config.MaxViewRange) && (attacker.Race == ObjectType.Monster && (attacker as MonsterObject).MonsterInfo.AI != 146))
                || Stats[Stat.Invincibility] > 0) return 0;

            UserMagic magic;
            if (element != Element.None)    //如果元素不为空
            {
                if (SEnvir.Random.Next(attacker.Race == ObjectType.Player ? 200 : 100) <= Stats[Stat.EvasionChance])// 4 + magic.Level * 2)   闪避
                {
                    if (Buffs.Any(x => x.Type == BuffType.Evasion) && Magics.TryGetValue(MagicType.Evasion, out magic))   //技能  风之闪避
                        LevelMagic(magic);

                    DisplayMiss = true;
                    return 0;
                }
            }
            else
            {
                if (SEnvir.Random.Next(attacker.Race == ObjectType.Player ? 200 : 100) <= Stats[Stat.BlockChance] - attacker.Stats[Stat.BlockChanceResistance])  //格挡
                {
                    DisplayBlock = true;
                    return 0;
                }
            }

            if (SEnvir.Random.Next(100) < Stats[Stat.InvincibilityChance])
            {
                DisplayBlock = true;
                return 0;
            }

            CombatTime = SEnvir.Now;

            if (attacker.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)attacker).PvPTime = SEnvir.Now;
            }

            if (Stats[Stat.Comfort] < 200)
                RegenTime = SEnvir.Now + RegenDelay;

            if ((Poison & PoisonType.Red) == PoisonType.Red || Stats[Stat.RedPoison] > 0)  //红毒
                power = (int)(power * Config.RedPoisonAttackRate);

            for (int i = 0; i < attacker.Stats[Stat.Rebirth]; i++)    //转生PVP伤害
                power = (int)(power * Config.RebirthPVP);

            if (SEnvir.Random.Next(100) < Math.Max(0, attacker.Stats[Stat.CriticalChance] + attacker.Stats[Stat.WeponCriticalChance] - Stats[Stat.CriticalChanceResistance]) && canCrit)  //暴击几率
            {
                if (!canReflect)
                    power = (int)(power * 1.2F);
                else if (attacker.Race == ObjectType.Player)
                {
                    if (Config.CriticalDamagePVP)      //暴击几率降低伤害
                    {
                        power = (int)(power * 1.3F / 2);
                    }
                    else
                    {
                        power = (int)(power * 1.3F);
                    }
                }
                else
                    power += power;

                Critical();
            }
            else if (SEnvir.Random.Next(100) < 30 && attacker.Stats[Stat.CriticalHit] > 0)  //会心一击
            {
                power += power * attacker.Stats[Stat.CriticalHit] / 100;
                CriticalHit();  //显示会心一击效果
            }
            else if (SEnvir.Random.Next(100) < attacker.Stats[Stat.DamageAdd])
            {
                power = power * 130 / 100;
                DamageAdd();   //显示暴捶效果
            }

            if (Config.CriticalDamagePVP)
            {
                if (SEnvir.Random.Next(100) < (attacker.Stats[Stat.CriticalChance] + attacker.Stats[Stat.WeponCriticalChance]) && canCrit)  //暴击几率%
                {
                    power += power * 30 / 100 + power * (attacker.Stats[Stat.CriticalDamage] + attacker.Stats[Stat.WeponCriticalChance]) / 100;
                    FatalAttack();  //显示致命一击效果
                }
            }

            power += attacker.Stats[Stat.ExtraDamage];  //额外伤害

            if (SEnvir.Random.Next(100) < attacker.Stats[Stat.SmokingMP])
            {
                ChangeMP(-CurrentMP);
                SmokingMP();
            }

            BuffInfo buff;

            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.FrostBite);

            if (buff != null)
            {
                buff.Stats[Stat.FrostBiteDamage] += power;
                Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });
                return 0;
            }

            if (attacker.Race == ObjectType.Monster && SEnvir.Now < FrostBiteImmunity) return 0;

            if (!ignoreShield)
            {
                if (Buffs.Any(x => x.Type == BuffType.Cloak))
                    power -= power / 2;

                buff = Buffs.FirstOrDefault(x => x.Type == BuffType.MagicShield);

                if (buff != null)
                {
                    buff.RemainingTime -= TimeSpan.FromMilliseconds(power * Config.MagicShieldRemainingTime);
                    Enqueue(new S.BuffTime { Index = buff.Index, Time = buff.RemainingTime });
                }

                power -= power * Stats[Stat.MagicShield] / 100;
            }

            if (Stats[Stat.FinalDamageReduce] > 0)   //最终伤害减少
            {
                power -= Stats[Stat.FinalDamageReduce];
                if (power < 0) return 0;
            }

            if (Stats[Stat.FinalDamageReduceRate] > 0)  //最终伤害减少百分比
            {
                power -= power * Stats[Stat.FinalDamageReduceRate] / 100;
                if (power < 0) return 0;
            }

            //STRUCKDONE
            if (StruckTime != DateTime.MaxValue && SEnvir.Now > StruckTime.AddMilliseconds(500) && canStruck) //&&!Buffs.Any(x => x.Type == BuffType.DragonRepulse)) 
            {
                StruckTime = SEnvir.Now;

                //if (StruckTime.AddMilliseconds(300) > ActionTime) ActionTime = StruckTime.AddMilliseconds(300);
                Broadcast(new S.ObjectStruck { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, AttackerID = attacker.ObjectID, Element = element });

                bool update = false;
                for (int i = 0; i < Equipment.Length; i++)
                {
                    switch ((EquipmentSlot)i)
                    {
                        case EquipmentSlot.Amulet:
                        case EquipmentSlot.Poison:
                        case EquipmentSlot.Torch:
                            continue;
                    }

                    update = DamageItem(GridType.Equipment, i, SEnvir.Random.Next(2) + 1, true) || update;
                }

                if (update)
                {
                    SendShapeUpdate();
                    RefreshStats();
                }
            }

            #region Conquest Stats

            UserConquestStats conquest = SEnvir.GetConquestStats(this);

            if (conquest != null)
            {
                switch (attacker.Race)
                {
                    case ObjectType.Player:
                        conquest.PvPDamageTaken += power;

                        conquest = SEnvir.GetConquestStats((PlayerObject)attacker);

                        if (conquest != null)
                            conquest.PvPDamageDealt += power;

                        break;
                    case ObjectType.Monster:
                        MonsterObject mob = (MonsterObject)attacker;

                        if (mob is CastleLord)
                            conquest.BossDamageTaken += power;
                        else if (mob.PetOwner != null)
                        {
                            conquest.PvPDamageTaken += power;

                            conquest = SEnvir.GetConquestStats(mob.PetOwner);

                            if (conquest != null)
                                conquest.PvPDamageDealt += power;
                        }
                        break;
                }
            }

            #endregion

            LastHitter = attacker;
            if (!ignoreShield && Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield))
            {
                buff = Buffs.FirstOrDefault(x => x.Type == BuffType.SuperiorMagicShield);

                if (buff != null)
                {
                    buff.Stats[Stat.SuperiorMagicShield] -= (int)Math.Min(int.MaxValue, power);
                    if (buff.Stats[Stat.SuperiorMagicShield] <= 0)
                        BuffRemove(buff);
                    else
                        Enqueue(new S.BuffChanged() { Index = buff.Index, Stats = new Stats(buff.Stats) });
                }
            }
            else
                ChangeHP(-power);
            LastHitter = null;

            if (CanAttackTarget(attacker) && (SEnvir.Random.Next(100) < Stats[Stat.ReflectChance]))    // canReflect &&   && attacker.Race != ObjectType.Player
            {
                attacker.Attacked(this, power * Stats[Stat.ReflectDamage] / 100, Element.None, false);

                //if (Buffs.Any(x => x.Type == BuffType.ReflectDamage) && Magics.TryGetValue(MagicType.ReflectDamage, out magic))
                //    LevelMagic(magic);
            }

            if (canReflect && CanAttackTarget(attacker) && SEnvir.Random.Next(100) < Stats[Stat.JudgementOfHeaven] && !(attacker is CastleLord))
            {
                int damagePvE = GetMC() / 5 + GetElementPower(ObjectType.Monster, Stat.LightningAttack) * 2;
                int damagePvP = Math.Min(50, GetMC() / 5 + GetElementPower(ObjectType.Monster, Stat.LightningAttack) / 2);

                Broadcast(new S.ObjectEffect { ObjectID = attacker.ObjectID, Effect = Effect.ThunderBolt });
                ActionList.Add(new DelayedAction(SEnvir.Now.AddMilliseconds(300), ActionType.DelayedAttackDamage, attacker, attacker.Race == ObjectType.Player ? damagePvP : damagePvE, Element.Lightning, false, false, true, true));

                //if (Buffs.Any(x => x.Type == BuffType.JudgementOfHeaven) && Magics.TryGetValue(MagicType.JudgementOfHeaven, out magic))
                //    LevelMagic(magic);
            }

            //if (Buffs.Any(x => x.Type == BuffType.Defiance) && Magics.TryGetValue(MagicType.Defiance, out magic))
            //LevelMagic(magic);

            if (Buffs.Any(x => x.Type == BuffType.RagingWind) && Magics.TryGetValue(MagicType.RagingWind, out magic))
                LevelMagic(magic);

            if (Magics.TryGetValue(MagicType.AdventOfDemon, out magic) && element == Element.None)
                LevelMagic(magic);

            if (Magics.TryGetValue(MagicType.AdventOfDevil, out magic) && element != Element.None)
                LevelMagic(magic);

            if (Buffs.Any(x => x.Type == BuffType.Invincibility) && Magics.TryGetValue(MagicType.Invincibility, out magic))
                LevelMagic(magic);

            return power;
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob">目标对象</param>
        /// <returns></returns>
        public override bool CanAttackTarget(MapObject ob)
        {
            if (ob?.Node == null || ob == this || ob.Dead || !ob.Visible || ob is Guard) return false;  //ob is Guard

            switch (ob.Race)
            {
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == null) return true; //Wild Monster

                    // Player vs Pet
                    if (mob.PetOwner == this)
                        return AttackMode == AttackMode.All || mob is Puppet;

                    if (mob is Puppet) return false; //Don't hit other person's puppet

                    if (mob.InSafeZone || InSafeZone) return false;

                    switch (CurrentMap.Info.Fight)
                    {
                        case FightSetting.Safe:
                            return false;
                    }

                    switch (mob.CurrentMap.Info.Fight)
                    {
                        case FightSetting.Safe:
                            return false;
                    }

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (InGroup(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (InGuild(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(mob.PetOwner))
                                return false;
                            else if (mob.PetOwner.Stats[Stat.PKPoint] >= Config.RedPoint && (InGuild(mob.PetOwner) || InGroup(mob.PetOwner)))
                                return false;
                            break;
                    }

                    return true;
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;
                    //if (player.GameMaster) return false;
                    if (player.Observer) return false;

                    if (InSafeZone || player.InSafeZone) return false; //Login Time?

                    switch (CurrentMap.Info.Fight)
                    {
                        case FightSetting.Safe:
                            return false;
                    }

                    switch (player.CurrentMap.Info.Fight)
                    {
                        case FightSetting.Safe:
                            return false;
                    }

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (InGroup(player))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (InGuild(player))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(player))
                                return false;
                            else if (player.Stats[Stat.PKPoint] >= Config.RedPoint && (InGuild(player) || InGroup(player)))
                                return false;
                            break;
                    }
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 直线魔法攻击目标
        /// </summary>
        /// <param name="ob">目标对象</param>
        /// <returns></returns>
        public override bool CanFlyTarget(MapObject ob)
        {
            if (Config.CanFlyTargetCheck) return true;

            if (ob?.Node == null || ob == this || ob is Guard || ob.Dead || !ob.Visible) return false;

            Point location = CurrentLocation;
            Point target = ob.CurrentLocation;

            while (location != target)
            {
                MirDirection dir = Functions.DirectionFromPoint(location, target);

                location = Functions.Move(location, dir, 1);

                if (location.X < 0 || location.Y < 0 || location.X >= CurrentMap.Width || location.Y >= CurrentMap.Height) return false;

                if (CurrentMap.GetCell(location) == null) return false;
            }

            return true;
        }
        /// <summary>
        /// 可以帮助的目标
        /// </summary>
        /// <param name="ob">目标对象</param>
        /// <returns></returns>
        public override bool CanHelpTarget(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;
            if (ob == this) return true;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (InGroup(player))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (InGuild(player))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(player))
                                return true;
                            break;
                    }

                    return false;

                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (mob.PetOwner == this) return true;
                    if (mob.PetOwner == null) return false;

                    switch (AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (InGroup(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (InGuild(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !AtWar(mob.PetOwner))
                                return true;
                            break;
                    }

                    return false;

                default:
                    return false;
            }
        }
        /// <summary>
        /// 完成魔法技能
        /// </summary>
        /// <param name="data"></param>
        public void CompleteMagic(params object[] data)
        {
            List<UserMagic> magics = (List<UserMagic>)data[0];
            foreach (UserMagic magic in magics)
            {
                CurrentMagic = magic.Info.Magic;
                switch (magic.Info.Magic)
                {
                    #region Warrior 战士

                    case MagicType.Interchange:
                        InterchangeEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.Beckon:
                        BeckonEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.MassBeckon:
                        MassBeckonEnd(magic);
                        break;
                    case MagicType.Defiance:
                        DefianceEnd(magic);
                        break;
                    case MagicType.Might:
                        MightEnd(magic);
                        break;
                    case MagicType.ReflectDamage:
                        ReflectDamageEnd(magic);
                        break;
                    case MagicType.Fetter:
                        FetterEnd(magic, (Cell)data[1]);
                        break;
                    case MagicType.SwiftBlade:
                    case MagicType.SeismicSlam:
                        Cell cell = (Cell)data[1];
                        if (cell == null || cell.Objects == null) continue;

                        for (int i = cell.Objects.Count - 1; i >= 0; i--)
                        {
                            if (!CanAttackTarget(cell.Objects[i])) continue;
                            Attack(cell.Objects[i], magics, true, 0);
                        }
                        break;
                    case MagicType.ThousandBlades:
                        MapObject mapObject = (MapObject)data[1];
                        if (mapObject == null || mapObject.CurrentMap != CurrentMap) break;

                        Attack(mapObject, magics, true, 0);

                        if (!Buffs.Any(x => x.Type == BuffType.SuperTransparency))
                        {
                            Stats buffStats = new Stats
                            {
                                [Stat.Transparency] = 1
                            };

                            BuffAdd(BuffType.SuperTransparency, TimeSpan.FromSeconds(6), buffStats, true, false, TimeSpan.Zero);

                            ApplyPoison(new Poison
                            {
                                Owner = this,
                                Type = PoisonType.Paralysis,
                                TickFrequency = TimeSpan.FromSeconds(6),
                                TickCount = 1,
                            });
                        }
                        break;
                    case MagicType.Invincibility:
                        InvincibilityEnd(magic);
                        break;
                    case MagicType.CrushingWave:
                        cell = (Cell)data[1];
                        if (cell == null || cell.Objects == null) continue;

                        for (int i = cell.Objects.Count - 1; i >= 0; i--)
                        {
                            if (!CanAttackTarget(cell.Objects[i])) continue;
                            Attack(cell.Objects[i], magics, (bool)data[2], 0);
                        }
                        LevelMagic(magic);
                        break;

                    #endregion

                    #region Wizard 法师

                    case MagicType.FireBall:
                    case MagicType.IceBolt:
                    case MagicType.LightningBall:
                    case MagicType.ThunderBolt:
                    case MagicType.GustBlast:
                    case MagicType.AdamantineFireBall:
                    case MagicType.IceBlades:
                    case MagicType.Cyclone:
                    case MagicType.MeteorShower:
                    case MagicType.ThunderStrike:
                    case MagicType.DragonRepulse:
                        MagicAttack(magics, (MapObject)data[1], true);
                        break;
                    case MagicType.Repulsion:
                        RepulsionEnd(magic, (Cell)data[1], (MirDirection)data[2]);
                        break;
                    case MagicType.ScortchedEarth:
                    case MagicType.LightningBeam:
                    case MagicType.FrozenEarth:
                    case MagicType.BlowEarth:
                    case MagicType.GreaterFrozenEarth:
                    case MagicType.ElementalHurricane:
                        AttackCell(magics, (Cell)data[1], (bool)data[2]);
                        break;
                    case MagicType.FireStorm:
                    case MagicType.LightningWave:
                    case MagicType.IceStorm:
                    case MagicType.DragonTornado:
                        AttackCell2(magics, (MapObject)data[1], (Point)data[2], true);
                        break;
                    case MagicType.Asteroid:
                        AttackCell(magics, (Cell)data[1], true);
                        break;
                    case MagicType.ChainLightning:
                        ChainLightningEnd2(magics, (MapObject)data[1], (Point)data[2]);
                        break;
                    case MagicType.Teleportation:
                        TeleportationEnd(magic);
                        break;
                    case MagicType.ElectricShock:
                        ElectricShockEnd(magic, (MonsterObject)data[1]);
                        break;
                    case MagicType.ExpelUndead:
                        ExpelUndeadEnd(magic, (MonsterObject)data[1]);
                        break;
                    case MagicType.FireWall:
                        FireWallEnd(magic, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.IceRain:
                        IceRainEnd(magic, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.MagicShield:
                        MagicShieldEnd(magic);
                        break;
                    case MagicType.FrostBite:
                        FrostBiteEnd(magic);
                        break;
                    case MagicType.GeoManipulation:
                        GeoManipulationEnd(magic, (Point)data[1]);
                        break;
                    case MagicType.Renounce:
                        RenounceEnd(magic);
                        break;
                    case MagicType.Tempest:
                        TempestEnd(magic, (Cell)data[1], (int)data[2]);
                        break;
                    case MagicType.JudgementOfHeaven:
                        JudgementOfHeavenEnd(magic);
                        break;
                    case MagicType.MirrorImage:
                        MirrorImageEnd(magic, this);
                        break;
                    case MagicType.SuperiorMagicShield:
                        SuperiorMagicShieldEnd(magic);
                        break;

                    #endregion

                    #region Taoist 道士

                    case MagicType.Heal:
                        HealEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.PoisonDust:
                        PoisonDustEnd(magics, (MapObject)data[1], (PoisonType)data[2]);
                        break;
                    case MagicType.ExplosiveTalisman:
                    case MagicType.EvilSlayer:
                    case MagicType.GreaterEvilSlayer:
                    case MagicType.GreaterHolyStrike:
                    case MagicType.ImprovedExplosiveTalisman:
                        MagicAttack(magics, (MapObject)data[1], (bool)data[2], (Stats)data[3]);
                        break;
                    case MagicType.Invisibility:
                        InvisibilityEnd(magic, this);
                        break;
                    case MagicType.StrengthOfFaith:
                        StrengthOfFaithEnd(magic);
                        break;
                    case MagicType.Transparency:
                        TransparencyEnd(magic, this, (Point)data[1]);
                        break;
                    case MagicType.CelestialLight:
                        CelestialLightEnd(magic);
                        break;
                    case MagicType.DemonExplosion:
                        DemonExplosionEnd(magic, (Stats)data[1]);
                        break;
                    case MagicType.MagicResistance:
                    case MagicType.ElementalSuperiority:
                        BuffCell(magic, (Cell)data[1], (Stats)data[2]);
                        break;
                    case MagicType.SummonSkeleton:
                    case MagicType.SummonJinSkeleton:
                    case MagicType.SummonShinsu:
                    case MagicType.SummonDemonicCreature:
                        SummonEnd(magic, (Map)data[1], (Point)data[2], (MonsterInfo)data[3]);
                        break;
                    case MagicType.TrapOctagon:
                        TrapOctagonEnd(magic, (Map)data[1], (Point)data[2]);
                        break;
                    case MagicType.Resilience:
                    case MagicType.MassInvisibility:
                    case MagicType.MassTransparency:
                    case MagicType.BloodLust:
                    case MagicType.MassHeal:
                    case MagicType.LifeSteal:
                        BuffCell(magic, (Cell)data[1], null);
                        break;
                    case MagicType.TaoistCombatKick:
                        TaoistCombatKick(magic, (Cell)data[1], (MirDirection)data[2]);
                        break;
                    case MagicType.Purification:
                        PurificationEnd(magics, (MapObject)data[1]);
                        break;
                    case MagicType.Resurrection:
                        ResurrectionEnd(magic, (PlayerObject)data[1]);
                        break;
                    case MagicType.Infection:
                        InfectionEnd(magics, (MapObject)data[1]);
                        break;
                    case MagicType.Neutralize:
                        NeutralizeEnd(magics, (MapObject)data[1]);
                        break;
                    case MagicType.DarkSoulPrison:
                        DarkSoulPrisonEnd(magic, (Point)data[1], (int)data[2]);
                        break;

                    #endregion

                    #region Assassin 刺客

                    case MagicType.PoisonousCloud:
                        PoisonousCloudEnd(magic);
                        break;
                    case MagicType.Cloak:
                        CloakEnd(magic, this, false);
                        break;
                    case MagicType.WraithGrip:
                        WraithGripEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.Abyss:
                        AbyssEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.HellFire:
                        HellFireEnd(magic, (MapObject)data[1]);
                        break;
                    case MagicType.TheNewBeginning:
                        TheNewBeginningEnd(magic, this);
                        break;
                    case MagicType.SummonPuppet:
                        SummonPuppetEnd(magic, this);
                        break;
                    case MagicType.DarkConversion:
                        DarkConversionEnd(magic, this);
                        break;
                    case MagicType.Evasion:
                        EvasionEnd(magic, this);
                        break;
                    case MagicType.RagingWind:
                        RagingWindEnd(magic, this);
                        break;
                    case MagicType.Rake:
                        RakeEnd(magic, (Cell)data[1]);
                        break;
                    case MagicType.FlashOfLight:
                        AttackCell(magics, (Cell)data[1], true);
                        break;
                    case MagicType.Concentration:
                        ConcentrationEnd(magic, this);
                        break;
                    case MagicType.SwordOfVengeance:
                        SwordOfVengeanceEnd(magic, (Point)data[1], (int)data[2]);
                        break;

                        #endregion
                }
            }
        }
        /// <summary>
        /// 魔法技能等级经验
        /// </summary>
        /// <param name="magic">魔法技能</param>
        public void LevelMagic(UserMagic magic)
        {
            if (magic == null) return;

            int experience = SEnvir.Random.Next(Config.SkillExp) + 1;

            experience *= Stats[Stat.SkillRate];

            int maxExperience;
            switch (magic.Level)
            {
                case 0:
                    if (Level < magic.Info.NeedLevel1) return;

                    maxExperience = magic.Info.Experience1;
                    break;
                case 1:
                    if (Level < magic.Info.NeedLevel2) return;

                    maxExperience = magic.Info.Experience2;
                    break;
                case 2:
                    if (Level < magic.Info.NeedLevel3) return;

                    maxExperience = magic.Info.Experience3;
                    break;
                default:
                    return;
            }

            magic.Experience += experience;

            if (magic.Experience >= maxExperience)
            {
                magic.Experience -= maxExperience;
                magic.Level++;
                RefreshStats();

                for (int i = Pets.Count - 1; i >= 0; i--)
                    Pets[i].RefreshStats();
            }

            Enqueue(new S.MagicLeveled { InfoIndex = magic.Info.Index, Level = magic.Level, Experience = magic.Experience });
        }
        /// <summary>
        /// 推的动作
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public override int Pushed(MirDirection direction, int distance)
        {
            UserMagic magic;
            if (Buffs.Any(x => x.Type == BuffType.Endurance) && Magics.TryGetValue(MagicType.Endurance, out magic))
                LevelMagic(magic);

            RemoveMount();

            return base.Pushed(direction, distance);
        }
        /// <summary>
        /// 施毒
        /// </summary>
        /// <param name="p">毒</param>
        /// <returns></returns>
        public override bool ApplyPoison(Poison p)
        {
            if (p.Owner != null && p.Owner.Race == ObjectType.Player)
            {
                PvPTime = SEnvir.Now;
                ((PlayerObject)p.Owner).PvPTime = SEnvir.Now;
            }

            if (Buffs.Any(x => x.Type == BuffType.Endurance))
            {
                UserMagic magic;
                if (Magics.TryGetValue(MagicType.Endurance, out magic))
                    LevelMagic(magic);

                return false;
            }

            bool res = base.ApplyPoison(p);

            //中毒提示
            if (res)
            {
                switch (p.Type)
                {
                    case PoisonType.Paralysis:
                        Connection.ReceiveChat("System.PoisonedParalysis".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedParalysis".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.StunnedStrike:
                        Connection.ReceiveChat("System.PoisonedStunnedStrike".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedStunnedStrike".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.Green:
                    case PoisonType.Red:
                        Connection.ReceiveChat("System.Poisoned".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.Poisoned".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.Silenced:
                        Connection.ReceiveChat("System.PoisonedSilenced".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedSilenced".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.Slow:
                        Connection.ReceiveChat("System.PoisonedSlow".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedSlow".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.WraithGrip:
                        Connection.ReceiveChat("System.PoisonedWraithGrip".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedWraithGrip".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.Abyss:
                        Connection.ReceiveChat("System.PoisonedAbyss".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.PoisonedAbyss".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.HellFire:
                        Connection.ReceiveChat("你被焚烧了".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("你被焚烧了".Lang(con.Language), MessageType.System);
                        break;
                    case PoisonType.ElectricShock:
                        Connection.ReceiveChat("你被电击了".Lang(Connection.Language), MessageType.System);

                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("你被电击了".Lang(con.Language), MessageType.System);
                        break;
                }

                if (p.Owner != null && p.Owner.Race == ObjectType.Player)
                    ((PlayerObject)p.Owner).CheckBrown(this);
            }

            return res;
        }
        /// <summary>
        /// 攻击单元,传入的单个cell为单位受到攻击
        /// </summary>
        /// <param name="magics">魔法技能</param>
        /// <param name="cell">单元</param>
        /// <param name="primary">基础</param>
        private void AttackCell(List<UserMagic> magics, Cell cell, bool primary)  //修复服务器奔溃错误
        {
            if (cell?.Objects == null) return;
            if (cell.Objects.Count == 0) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                if (i >= cell.Objects.Count) continue;
                MapObject ob = cell.Objects[i];
                if (!CanAttackTarget(ob)) continue;

                MagicAttack(magics, ob, primary);
            }
        }
        /// <summary>
        /// 攻击单元2,以传入的MapObject.CurrentLocation为单位向周围扩展一个坐标受到攻击，否则以传入的location为单位向周围扩展一个坐标受到攻击
        /// </summary>
        /// <param name="magics">魔法技能</param>
        /// <param name="ob">目标对象</param>
        /// <param name="location">坐标</param>
        /// <param name="primary">基础</param>
        private void AttackCell2(List<UserMagic> magics, MapObject ob, Point location, bool primary)
        {
            List<Cell> cells = CurrentMap.GetCells(ob?.CurrentLocation ?? location, 0, 1);
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

                    MagicAttack(magics, o, primary);
                }
            }
        }

        /// <summary>
        /// BUFF单元
        /// </summary>
        /// <param name="magic">魔法技能</param>
        /// <param name="cell">单元</param>
        /// <param name="stats">属性</param>
        private void BuffCell(UserMagic magic, Cell cell, Stats stats)
        {
            if (cell?.Objects == null) return;

            for (int i = cell.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = cell.Objects[i];
                if (!CanHelpTarget(ob)) continue;

                switch (magic.Info.Magic)
                {
                    case MagicType.MagicResistance:
                        MagicResistanceEnd(magic, ob, stats);
                        break;
                    case MagicType.Resilience:
                        ResilienceEnd(magic, ob);
                        break;
                    case MagicType.MassInvisibility:
                        InvisibilityEnd(magic, ob);
                        break;
                    case MagicType.MassTransparency:
                        MassTransparencyEnd(magic, ob);
                        break;
                    case MagicType.ElementalSuperiority:
                        ElementalSuperiorityEnd(magic, ob, stats);
                        break;
                    case MagicType.BloodLust:
                        BloodLustEnd(magic, ob);
                        break;
                    case MagicType.MassHeal:
                        HealEnd(magic, ob);
                        break;
                    case MagicType.LifeSteal:
                        LifeStealEnd(magic, ob);
                        break;
                }
            }
        }
        /// <summary>
        /// 人物死亡时
        /// </summary>
        public override void Die()
        {
            RevivalTime = SEnvir.Now + Config.AutoReviveDelay;  //复活时间

            RemoveMount();  //死亡下马

            TradeClose();   //交易失败

            HashSet<MonsterObject> clearList = new HashSet<MonsterObject>(TaggedMonsters);  //清除目标对象列表

            foreach (MonsterObject ob in clearList)
                ob.EXPOwner = null;   //对象经验置空

            TaggedMonsters.Clear();  //目标对象清除

            base.Die();

            for (int i = SpellList.Count - 1; i >= 0; i--)
                SpellList[i].Despawn();  //移出施法的对象

            for (int i = Pets.Count - 1; i >= 0; i--)
            {
                MonsterObject pet = Pets[i];
                if (pet == null || pet.MonsterInfo.AI == 146) continue;
                Pets[i].Die();   //宠物死亡
                Pets.Remove(pet);
            }
            //Pets.Clear();    //宠物清除

            #region Conquest Stats  攻城战争的统计

            UserConquestStats conquest = SEnvir.GetConquestStats(this);

            if (conquest != null && LastHitter != null)
            {
                switch (LastHitter.Race)
                {
                    case ObjectType.Player:
                        conquest.PvPDeathCount++;

                        conquest = SEnvir.GetConquestStats((PlayerObject)LastHitter);

                        if (conquest != null)
                            conquest.PvPKillCount++;
                        break;
                    case ObjectType.Monster:
                        MonsterObject mob = (MonsterObject)LastHitter;

                        if (mob is CastleLord)
                            conquest.BossDeathCount++;
                        else if (mob.PetOwner != null)
                        {
                            conquest.PvPDeathCount++;

                            conquest = SEnvir.GetConquestStats(mob.PetOwner);
                            if (conquest != null)
                                conquest.PvPKillCount++;
                        }
                        break;
                }
            }

            #endregion

            PlayerObject attacker = null;  //攻击者等空

            if (LastHitter != null)
            {
                switch (LastHitter.Race)
                {
                    case ObjectType.Player:
                        attacker = (PlayerObject)LastHitter;
                        break;
                    case ObjectType.Monster:
                        attacker = ((MonsterObject)LastHitter).PetOwner;
                        break;
                }
            }

            #region 人物被人物杀 包括被人物的宝宝击杀

            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerKilled(EventTypes.PlayerDie,
                    new PlayerKilledEventArgs { Killer = attacker }));

            //python 触发
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    //击杀者
                    PythonTuple args = PythonOps.MakeTuple(new object[] { attacker, this });
                    SEnvir.ExecutePyWithTimer(trig_player, attacker, "OnKillPlayer", args);
                    //trig_player(attacker, "OnKillPlayer", args);

                    //被击杀者
                    PythonTuple args2 = PythonOps.MakeTuple(new object[] { this, attacker });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnDie", args2);
                    //trig_player(this, "OnDie", args2);
                }
            }
            catch (System.Data.SyntaxErrorException e)
            {
                string msg = "PlayerEvent Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "PlayerEvent SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }

            #endregion

            switch (CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                    //case FightSetting.War:
                    return;
            }

            if (InSafeZone) return;

            if (Config.RebirthDie)  //如果转生死亡丢失经验勾选
            {
                if (Stats[Stat.Rebirth] > 0 && (LastHitter == null || LastHitter.Race != ObjectType.Player) && Experience > 0)      //转生死亡失去经验
                {
                    //Level = Math.Max(Level - Stats[Stat.Rebirth] * 3, 1);
                    decimal expbonus = Experience;
                    Enqueue(new S.GainedExperience { Amount = -expbonus, WeapEx = 0M, BonusEx = 0M, });
                    Experience = 0;

                    if (expbonus > 0)
                    {
                        List<PlayerObject> targets = new List<PlayerObject>();

                        foreach (PlayerObject player in SEnvir.Players)
                        {
                            if (player.Character.Rebirth > 0 || player.Character.Level >= 86) continue;

                            targets.Add(player);
                        }

                        PlayerObject target = null;
                        if (targets.Count > 0)
                        {
                            target = targets[SEnvir.Random.Next(targets.Count)];

                            target.GainExperience(expbonus, false, int.MaxValue, false, false);
                        }

                        SEnvir.Broadcast(new S.Chat { Text = $"PlayerObject.RebirthDie".Lang(Connection.Language, (Name, expbonus.ToString("##,##0"), target?.Name ?? "没有人".Lang(Connection.Language))), Type = MessageType.System });
                    }

                    // Enqueue(new S.LevelChanged { Level = Level, Experience = Experience });
                    // Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });
                }
            }

            BuffInfo buff;
            int rate;
            TimeSpan time;

            if (attacker != null)  //如果攻击者不等空
            {
                if (AtWar(attacker))  //如果是在（战争中的攻击者）  || CurrentMap.Info.Fight == FightSetting.War
                {
                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)  //行会成员
                    {
                        if (member.Account.Connection == null) continue;

                        //member.Account.Connection.ReceiveChat("Guild.GuildWarDeath".Lang(member.Account.Connection.Language, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);

                        //foreach (SConnection con in SEnvir.Connections)
                        //con.ReceiveChat("Guild.GuildWarDeath".Lang(con.Language, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                    }
                    foreach (GuildMemberInfo member in attacker.Character.Account.GuildMember.Guild.Members)  //攻击方行会成员
                    {
                        if (member.Account.Connection == null) continue;

                        //member.Account.Connection.ReceiveChat("Guild.GuildWarDeath".Lang(member.Account.Connection.Language, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);                
                    }
                    foreach (SConnection con in SEnvir.Connections)
                        con.ReceiveChat("Guild.GuildWarDeath".Lang(con.Language, Name, Character.Account.GuildMember.Guild.GuildName, attacker.Name, attacker.Character.Account.GuildMember.Guild.GuildName), MessageType.System);
                }
                else
                {
                    if (Stats[Stat.PKPoint] < Config.RedPoint && Stats[Stat.Brown] == 0)  //如果 PK值小于200 或者 不是灰名
                    {
                        Connection.ReceiveChat("System.MurderedBy".Lang(Connection.Language, attacker.Name), MessageType.System); //出提示被杀死了
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.MurderedBy".Lang(con.Language, attacker.Name), MessageType.System);

                        //PvP击杀对方
                        if (attacker.Stats[Stat.PKPoint] >= Config.RedPoint && SEnvir.Random.Next(Config.PvPCurseRate) == 0)  //如果PK值大于200 或者 诅咒随机值等0
                        {
                            if (Config.PVPLuckCheck)
                            {
                                int luck = Config.MaxCurse;  //定义 幸运值等最大诅咒值
                                //判断攻击者武器装备格子没有武器    判断攻击者武器的幸运等于最大诅咒值
                                if (attacker.Equipment[(int)EquipmentSlot.Weapon] != null && (attacker.Equipment[(int)EquipmentSlot.Weapon].Stats[Stat.Luck] != luck))
                                {

                                    //发包更新攻击者武器属性值变化
                                    S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                                    attacker.Enqueue(result);

                                    attacker.Equipment[(int)EquipmentSlot.Weapon].AddStat(Stat.Luck, -1, StatSource.Enhancement);  //攻击者武器增加幸运值-1
                                    attacker.Equipment[(int)EquipmentSlot.Weapon].StatsChanged();                                  //攻击者武器属性值变化
                                    result.NewStats[Stat.Luck]--;                                    //结果 新属性信息[幸运]--

                                    attacker.Stats[Stat.Luck]--;                                     //攻击者属性值运行--

                                    //刷新完整属性列表
                                    result.FullItemStats = attacker.Equipment[(int)EquipmentSlot.Weapon].ToClientInfo().FullItemStats;
                                }
                            }
                            else
                            {
                                //增加诅咒BUFF -1点 一个小时
                                rate = -1;
                                time = Config.PvPCurseDuration;
                                buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PvPCurse);

                                if (buff != null)
                                {
                                    rate += buff.Stats[Stat.Luck];
                                    time += buff.RemainingTime;
                                }

                                attacker.BuffAdd(BuffType.PvPCurse, time, new Stats { [Stat.Luck] = rate }, false, false, TimeSpan.Zero);
                            }

                            attacker.Connection.ReceiveChat("System.Curse".Lang(attacker.Connection.Language, Name), MessageType.System);  //出提示厄运伴随着你
                            foreach (SConnection con in attacker.Connection.Observers)
                                con.ReceiveChat("System.Murdered".Lang(con.Language, Name), MessageType.System);    //出提示你杀死了谁
                        }
                        else
                        {
                            if (SEnvir.Random.Next(5) == 0)
                            {
                                int luck = Config.MaxCurse;  //定义 幸运值等最大诅咒值
                                //判断攻击者武器装备格子没有武器       判断攻击者武器的幸运等于最大诅咒值
                                if (attacker.Equipment[(int)EquipmentSlot.Weapon] != null && (attacker.Equipment[(int)EquipmentSlot.Weapon].Stats[Stat.Luck] != luck))
                                {

                                    //发包更新攻击者武器属性值变化
                                    S.ItemStatsChanged result = new S.ItemStatsChanged { GridType = GridType.Equipment, Slot = (int)EquipmentSlot.Weapon, NewStats = new Stats() };
                                    attacker.Enqueue(result);

                                    attacker.Equipment[(int)EquipmentSlot.Weapon].AddStat(Stat.Luck, -1, StatSource.Enhancement);  //攻击者武器增加幸运值-1
                                    attacker.Equipment[(int)EquipmentSlot.Weapon].StatsChanged();                                  //攻击者武器属性值变化
                                    result.NewStats[Stat.Luck]--;                                    //结果 新属性信息[幸运]--

                                    attacker.Stats[Stat.Luck]--;                                     //攻击者属性值运行--

                                    //刷新完整属性列表
                                    result.FullItemStats = attacker.Equipment[(int)EquipmentSlot.Weapon].ToClientInfo().FullItemStats;
                                }
                            }
                            attacker.Connection.ReceiveChat("System.Murdered".Lang(attacker.Connection.Language, Name), MessageType.System);  //出提示你杀死了谁
                            foreach (SConnection con in attacker.Connection.Observers)
                                con.ReceiveChat("System.Murdered".Lang(con.Language, Name), MessageType.System);
                        }

                        attacker.IncreasePKPoints(Config.PKPointRate);
                    }
                    else
                    {
                        attacker.Connection.ReceiveChat("System.Protected".Lang(attacker.Connection.Language), MessageType.System);
                        foreach (SConnection con in attacker.Connection.Observers)
                            con.ReceiveChat("System.Protected".Lang(con.Language), MessageType.System);

                        Connection.ReceiveChat("System.Killed".Lang(Connection.Language, attacker.Name), MessageType.System);
                        foreach (SConnection con in Connection.Observers)
                            con.ReceiveChat("System.Killed".Lang(con.Language, attacker.Name), MessageType.System);
                    }
                }
            }
            else
            {
                if (Stats[Stat.PKPoint] >= Config.RedPoint)
                {
                    bool update = false;
                    for (int i = 0; i < Equipment.Length; i++)
                    {
                        UserItem item = Equipment[i];
                        if (item == null) continue;

                        update = DamageItem(GridType.Equipment, i, item.Info.Durability / 10) || update;
                    }

                    if (update)
                    {
                        SendShapeUpdate();
                        RefreshStats();
                    }
                }

                Connection.ReceiveChat("System.Died".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("System.Died".Lang(con.Language), MessageType.System);
                /*
                #region 人物被杀

                //队列一个事件, 不要忘记添加listener
                SEnvir.EventManager.QueueEvent(
                    new PlayerKilled(EventTypes.PlayerDie,
                        new PlayerKilledEventArgs { Killer = LastHitter }));

                //python 触发
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this, LastHitter });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnDie", args);
                        //trig_player(this, "OnDie", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
                {
                    string msg = "PlayerEvent Syntax error : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "PlayerEvent SystemExit : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }

                #endregion
                */
            }

            if (CurrentMap.Info.DeathDrop || Stats[Stat.DeathDrops] > 0)  //如果 地图死亡掉落开启  使用了贪婪的药水，那么开启死亡掉落物品
            {
                DeathDrop();
                SendShapeUpdate();
            }
        }
        /// <summary>
        /// 死亡掉落道具
        /// </summary>
        public void DeathDrop()
        {
            #region 死亡包裹掉落装备
            for (int i = 0; i < Inventory.Length; i++)   //包裹里掉落装备
            {
                UserItem item = Inventory[i];

                if (item == null) continue;   //如果道具为空 继续
                if (!item.Info.CanDeathDrop) continue;  //如果道具信息为死亡无法爆出 继续
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;   //如果道具标签为绑定 继续
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;  //如果道具标签为结婚戒指 继续
                //if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;  //如果道具标签为无法出售的道具 继续
                int subnum = 0;

                if (Stats[Stat.PKPoint] >= Config.RedPoint) subnum = Config.InventoryRedDeathDrop;   //红名死亡爆率

                else if (Stats[Stat.Brown] != 0) subnum = Config.InventoryAshDeathDrop;  //灰名死亡爆率

                if (SEnvir.Random.Next(Config.CharacterInventoryDeathDrop - subnum) > 0) continue;  //随机值10大于0 继续

                Cell cell = GetDropLocation(4, null);

                if (cell == null) break;

                long count;

                count = 1 + SEnvir.Random.Next((int)item.Count);

                UserItem dropItem;
                if (count == item.Count)
                {
                    dropItem = item;
                    RemoveItem(item);
                    Inventory[i] = null;
                    count = 0;
                }
                else
                {
                    dropItem = SEnvir.CreateFreshItem(item);
                    dropItem.Count = count;
                    item.Count -= count;

                    count = item.Count;
                }

                //记录物品来源
                SEnvir.RecordTrackingInfo(dropItem, CurrentMap?.Info?.Description, ObjectType.Player, Name, item?.OriginalOwner);

                Companion?.RefreshWeight();
                dropItem.IsTemporary = true;

                ItemObject ob = new ItemObject
                {
                    Item = dropItem,
                };

                ob.Spawn(CurrentMap.Info, cell.Location);

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = i, Count = count }, Success = true });
            }
            #endregion

            #region 死亡宠物包裹掉落物品
            if (Companion != null)  //宠物掉落物品
            {
                for (int i = 0; i < Companion.Inventory.Length; i++)
                {
                    UserItem item = Companion.Inventory[i];

                    if (item == null) continue;
                    if (!item.Info.CanDeathDrop) continue;
                    if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                    if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;

                    int subnum1 = 0;

                    if (Stats[Stat.PKPoint] >= Config.RedPoint) subnum1 = Config.ComInventoryRedDeathDrop;   //红名死亡爆率

                    else if (Stats[Stat.Brown] != 0) subnum1 = Config.ComInventoryAshDeathDrop;  //灰名死亡爆率

                    if (SEnvir.Random.Next(Config.CompanionInventoryDeathDrop - subnum1) > 0) continue;  //白名死亡爆率

                    Cell cell = GetDropLocation(4, null);

                    if (cell == null) break;

                    long count;

                    count = 1 + SEnvir.Random.Next((int)item.Count);

                    UserItem dropItem;
                    if (count == item.Count)
                    {
                        dropItem = item;
                        RemoveItem(item);
                        Companion.Inventory[i] = null;
                        count = 0;
                    }
                    else
                    {
                        dropItem = SEnvir.CreateFreshItem(item);
                        dropItem.Count = count;
                        item.Count -= count;

                        count = item.Count;
                    }

                    Companion?.RefreshWeight();
                    dropItem.IsTemporary = true;

                    //记录物品来源
                    SEnvir.RecordTrackingInfo(dropItem, CurrentMap?.Info?.Description, ObjectType.Player, Name, item?.OriginalOwner);

                    ItemObject ob = new ItemObject
                    {
                        Item = dropItem,
                    };

                    ob.Spawn(CurrentMap.Info, cell.Location);

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.CompanionInventory, Slot = i, Count = count }, Success = true });
                }
            }
            #endregion

            #region 死亡身上装备掉落
            bool botter = Character.Account.ItemBot || Character.Account.GoldBot;

            //int subnum2 = 0;

            //if (Stats[Stat.PKPoint] >= Config.RedPoint) subnum2 = Config.EquipmentRedDeathDrop;   //红名死亡爆率

            //else if (Stats[Stat.Brown] != 0) subnum2 = Config.EquipmentAshDeathDrop;  //灰名死亡爆率

            List<int> dropList = new List<int>();

            if ((Stats[Stat.PKPoint] >= Config.RedPoint) && SEnvir.Random.Next(Config.DieRedRandomChance) == 0) //如果是红名 随机值10出0
            {
                for (int i = 0; i < Equipment.Length; i++)
                {
                    UserItem item = Equipment[i];

                    if (item == null) continue;
                    if (!item.Info.CanDeathDrop) continue;
                    if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                    if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                    if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;
                    if (i == (int)EquipmentSlot.Weapon) continue;  //武器不爆
                    if (CurrentMap.Info.Index == 25 && SEnvir.ConquestWars.Count > 0) continue;  //如果当前地图是沙巴克，且沙巴克攻城进行中 不爆身上装备
                    if (CurrentMap.Info.Fight == FightSetting.War) continue;  //如果是战争地图属性，不爆身上装备

                    dropList.Add(i);

                    if (botter && dropList.Count > 0) break;
                }

                if (dropList.Count > 0)
                {
                    int index = dropList[SEnvir.Random.Next(dropList.Count)];

                    UserItem item = Equipment[index];

                    Cell cell = GetDropLocation(4, null);

                    if (cell != null)
                    {
                        UserItem dropItem;

                        dropItem = item;
                        RemoveItem(item);
                        Equipment[index] = null;

                        dropItem.IsTemporary = true;

                        ItemObject ob = new ItemObject
                        {
                            Item = dropItem,
                        };

                        ob.Spawn(CurrentMap.Info, cell.Location);

                        Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = index, Count = 0 }, Success = true });
                    }
                }
            }

            dropList.Clear();

            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem item = Equipment[i];

                if (item == null) continue;
                if (!item.Info.CanDeathDrop) continue;
                if ((item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) continue;
                if ((item.Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) continue;
                if ((item.Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) continue;
                if (CurrentMap.Info.Index == 25 && SEnvir.ConquestWars.Count > 0) continue;  //如果当前地图是沙巴克，且沙巴克攻城进行中 不爆身上装备
                if (CurrentMap.Info.Fight == FightSetting.War) continue;  //如果是战争地图属性，不爆身上装备

                if (item.Info.Rarity == Rarity.Common && SEnvir.Random.Next(Config.CharacterEquipmentDeathDrop) != 0)
                {
                    continue; //如果是普通装备 几率40出0 
                }
                else
                {
                    if (item.Info.Rarity == Rarity.Common && i == (int)EquipmentSlot.Weapon) //武器爆率在按几率降低二分之一
                    {
                        if (SEnvir.Random.Next(Config.CharacterEquipmentDeathDrop * Config.WeapEquipmentDeathDrop) != 0) continue;
                    }
                }

                if (item.Info.Rarity == Rarity.Superior && SEnvir.Random.Next(Config.EquipmentAshDeathDrop) != 0)
                {
                    continue; //如果是高级装备 几率100出0
                }
                else
                {
                    if (item.Info.Rarity == Rarity.Superior && i == (int)EquipmentSlot.Weapon) //武器爆率在按几率降低二分之一
                    {
                        if (SEnvir.Random.Next(Config.EquipmentAshDeathDrop * Config.WeapEquipmentDeathDrop) != 0) continue;
                    }
                }

                if (item.Info.Rarity == Rarity.Elite && SEnvir.Random.Next(Config.EquipmentRedDeathDrop) != 0)
                {
                    continue; //如果是稀释装备 几率200出0
                }
                else
                {
                    if (item.Info.Rarity == Rarity.Elite && i == (int)EquipmentSlot.Weapon) //武器爆率在按几率降低二分之一
                    {
                        if (SEnvir.Random.Next(Config.EquipmentRedDeathDrop * Config.WeapEquipmentDeathDrop) != 0) continue;
                    }
                }

                dropList.Add(i);

                if (botter && dropList.Count > 0) break;
            }

            if (dropList.Count > 0)
            {
                int index = dropList[SEnvir.Random.Next(dropList.Count)];

                UserItem item = Equipment[index];

                Cell cell = GetDropLocation(4, null);

                if (cell != null)
                {
                    UserItem dropItem;

                    dropItem = item;
                    RemoveItem(item);
                    Equipment[index] = null;

                    dropItem.IsTemporary = true;

                    ItemObject ob = new ItemObject
                    {
                        Item = dropItem,
                    };

                    ob.Spawn(CurrentMap.Info, cell.Location);

                    Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Equipment, Slot = index, Count = 0 }, Success = true });
                }
            }

            #endregion

            RefreshWeight();  //刷新负重
            RefreshStats();   //刷新属性
        }
        /// <summary>
        /// 是否在行会里
        /// </summary>
        /// <param name="player">玩家对象</param>
        /// <returns></returns>
        public bool InGuild(PlayerObject player)
        {
            if (player == null) return false;

            if (Character.Account.GuildMember == null || player.Character.Account.GuildMember == null) return false;

            return (Character.Account.GuildMember.Guild == player.Character.Account.GuildMember.Guild) || (SEnvir.GuildAllianceInfoList.Binding.Any(x => (x.Guild1 == Character.Account.GuildMember.Guild && x.Guild2 == player.Character.Account.GuildMember.Guild) || (x.Guild2 == Character.Account.GuildMember.Guild && x.Guild1 == player.Character.Account.GuildMember.Guild)));
        }
        /// <summary>
        /// 判断是否灰名
        /// </summary>
        /// <param name="ob">角色对象</param>
        public void CheckBrown(MapObject ob)
        {
            //if in fight map
            PlayerObject player;
            switch (ob.Race)
            {
                case ObjectType.Player:
                    player = (PlayerObject)ob;
                    break;
                case ObjectType.Monster:
                    player = ((MonsterObject)ob).PetOwner;
                    break;
                default:
                    return;
            }

            if (player == null || player == this) return;

            switch (CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                case FightSetting.War:
                    return;
            }

            if (InSafeZone || player.InSafeZone) return;

            switch (player.CurrentMap.Info.Fight)
            {
                case FightSetting.Safe:
                case FightSetting.Fight:
                case FightSetting.War:
                    return;
            }

            if (player.Stats[Stat.Brown] > 0 || player.Stats[Stat.PKPoint] >= Config.RedPoint) return;

            if (AtWar(player)) return;

            BuffAdd(BuffType.Brown, Config.BrownDuration, new Stats { [Stat.Brown] = 1 }, false, false, TimeSpan.Zero);
        }

        /// <summary>
        /// 增加PK点
        /// </summary>
        /// <param name="count">点数</param>
        public void IncreasePKPoints(int count)
        {
            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.PKPoint);

            if (buff != null)
                count += buff.Stats[Stat.PKPoint];

            if (count >= Config.RedPoint && !Character.BindPoint.RedZone)
            {
                SafeZoneInfo info = SEnvir.SafeZoneInfoList.Binding.FirstOrDefault(x => x.RedZone && x.ValidBindPoints.Count > 0);

                if (info != null)
                    Character.BindPoint = info;
            }

            BuffAdd(BuffType.PKPoint, TimeSpan.MaxValue, new Stats { [Stat.PKPoint] = count }, false, false, Config.PKPointTickRate);
        }
        /// <summary>
        /// 获取四花蓝值和幸运值
        /// </summary>
        /// <param name="race">对象</param>
        /// <returns></returns>
        public int GetLotusMana(ObjectType race)
        {
            if (race != ObjectType.Player) return Stats[Stat.Mana];

            int min = 0;
            int max = Stats[Stat.Mana];

            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 获得元素攻击和幸运值
        /// </summary>
        /// <param name="race">对象</param>
        /// <param name="element">元素属性</param>
        /// <returns></returns>
        public int GetElementPower(ObjectType race, Stat element)
        {
            if (race != ObjectType.Player) return Stats[element];

            int min = 0;
            int max = Stats[element];

            int luck = Stats[Stat.Luck];

            if (min < 0) min = 0;
            if (min >= max) return max;

            if (luck > 0)
            {
                if (luck >= 10) return max;

                if (SEnvir.Random.Next(10) < luck) return max;
            }
            else if (luck < 0)
            {
                if (luck < -SEnvir.Random.Next(10)) return min;
            }

            return SEnvir.Random.Next(min, max + 1);
        }
        /// <summary>
        /// 赋值毒状态
        /// </summary>
        /// <param name="type">毒的类型</param>
        /// <param name="tickcount">滴答数</param>
        /// <param name="value">总时长</param>
        public void SetPoison(PoisonType type, int value, int tickcount, double time)
        {
            ApplyPoison(new Poison
            {
                Type = type,
                Value = value,
                TickCount = tickcount,
                TickFrequency = TimeSpan.FromSeconds(time),
                Owner = this,
            });
        }
        #endregion
    }
}
