using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 刷新人物属性
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 刷新负重
        /// </summary>
        public void RefreshWeight()
        {
            BagWeight = 0;

            foreach (UserItem item in Inventory)
            {
                if (item == null) continue;

                BagWeight += item.Weight;
            }

            WearWeight = 0;
            HandWeight = 0;

            foreach (UserItem item in Equipment)
            {
                if (item?.Info == null) continue;

                switch (item.Info.ItemType)
                {
                    case ItemType.Weapon:
                    case ItemType.Torch:
                        HandWeight += item.Weight;
                        break;
                    default:
                        WearWeight += item.Weight;
                        break;
                }
            }

            Enqueue(new S.WeightUpdate { BagWeight = BagWeight, WearWeight = WearWeight, HandWeight = HandWeight });
        }
        /// <summary>
        /// 刷新属性
        /// </summary>
        public override void RefreshStats()
        {
            int tracking = Stats[Stat.BossTracker] + Stats[Stat.PlayerTracker];

            Stats.Clear();

            AddBaseStats();

            if (Character.Horse != HorseType.None)
            {
                switch (Character.Horse)   //坐骑属性
                {
                    case HorseType.Brown:
                        MonsterInfo brownHorseInfo = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.BrownHorse);
                        if (brownHorseInfo != null)
                        {
                            foreach (var kvp in brownHorseInfo.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.White:
                        MonsterInfo whiteHorseInfo = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.WhiteHorse);
                        if (whiteHorseInfo != null)
                        {
                            foreach (var kvp in whiteHorseInfo.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.Red:
                        MonsterInfo redHorseInfo = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.RedHorse);
                        if (redHorseInfo != null)
                        {
                            foreach (var kvp in redHorseInfo.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.Black:
                        MonsterInfo blackHorseInfo = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.BlackHorse);
                        if (blackHorseInfo != null)
                        {
                            foreach (var kvp in blackHorseInfo.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse1:
                        MonsterInfo diyHorse1Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse1);
                        if (diyHorse1Info != null)
                        {
                            foreach (var kvp in diyHorse1Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse2:
                        MonsterInfo diyHorse2Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse2);
                        if (diyHorse2Info != null)
                        {
                            foreach (var kvp in diyHorse2Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse3:
                        MonsterInfo diyHorse3Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse3);
                        if (diyHorse3Info != null)
                        {
                            foreach (var kvp in diyHorse3Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse4:
                        MonsterInfo diyHorse4Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse4);
                        if (diyHorse4Info != null)
                        {
                            foreach (var kvp in diyHorse4Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse5:
                        MonsterInfo diyHorse5Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse5);
                        if (diyHorse5Info != null)
                        {
                            foreach (var kvp in diyHorse5Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse6:
                        MonsterInfo diyHorse6Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse6);
                        if (diyHorse6Info != null)
                        {
                            foreach (var kvp in diyHorse6Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse7:
                        MonsterInfo diyHorse7Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse7);
                        if (diyHorse7Info != null)
                        {
                            foreach (var kvp in diyHorse7Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse8:
                        MonsterInfo diyHorse8Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse8);
                        if (diyHorse8Info != null)
                        {
                            foreach (var kvp in diyHorse8Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse9:
                        MonsterInfo diyHorse9Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse9);
                        if (diyHorse9Info != null)
                        {
                            foreach (var kvp in diyHorse9Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse10:
                        MonsterInfo diyHorse10Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse10);
                        if (diyHorse10Info != null)
                        {
                            foreach (var kvp in diyHorse10Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse11:
                        MonsterInfo diyHorse11Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse11);
                        if (diyHorse11Info != null)
                        {
                            foreach (var kvp in diyHorse11Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse12:
                        MonsterInfo diyHorse12Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse12);
                        if (diyHorse12Info != null)
                        {
                            foreach (var kvp in diyHorse12Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse13:
                        MonsterInfo diyHorse13Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse13);
                        if (diyHorse13Info != null)
                        {
                            foreach (var kvp in diyHorse13Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse14:
                        MonsterInfo diyHorse14Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse14);
                        if (diyHorse14Info != null)
                        {
                            foreach (var kvp in diyHorse14Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse15:
                        MonsterInfo diyHorse15Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse15);
                        if (diyHorse15Info != null)
                        {
                            foreach (var kvp in diyHorse15Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse16:
                        MonsterInfo diyHorse16Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse16);
                        if (diyHorse16Info != null)
                        {
                            foreach (var kvp in diyHorse16Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse17:
                        MonsterInfo diyHorse17Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse17);
                        if (diyHorse17Info != null)
                        {
                            foreach (var kvp in diyHorse17Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse18:
                        MonsterInfo diyHorse18Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse18);
                        if (diyHorse18Info != null)
                        {
                            foreach (var kvp in diyHorse18Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse19:
                        MonsterInfo diyHorse19Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse19);
                        if (diyHorse19Info != null)
                        {
                            foreach (var kvp in diyHorse19Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse20:
                        MonsterInfo diyHorse20Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse20);
                        if (diyHorse20Info != null)
                        {
                            foreach (var kvp in diyHorse20Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse21:
                        MonsterInfo diyHorse21Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse21);
                        if (diyHorse21Info != null)
                        {
                            foreach (var kvp in diyHorse21Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse22:
                        MonsterInfo diyHorse22Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse22);
                        if (diyHorse22Info != null)
                        {
                            foreach (var kvp in diyHorse22Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse23:
                        MonsterInfo diyHorse23Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse23);
                        if (diyHorse23Info != null)
                        {
                            foreach (var kvp in diyHorse23Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse24:
                        MonsterInfo diyHorse24Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse24);
                        if (diyHorse24Info != null)
                        {
                            foreach (var kvp in diyHorse24Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse25:
                        MonsterInfo diyHorse25Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse25);
                        if (diyHorse25Info != null)
                        {
                            foreach (var kvp in diyHorse25Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse26:
                        MonsterInfo diyHorse26Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse26);
                        if (diyHorse26Info != null)
                        {
                            foreach (var kvp in diyHorse26Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse27:
                        MonsterInfo diyHorse27Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse27);
                        if (diyHorse27Info != null)
                        {
                            foreach (var kvp in diyHorse27Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse28:
                        MonsterInfo diyHorse28Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse28);
                        if (diyHorse28Info != null)
                        {
                            foreach (var kvp in diyHorse28Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse29:
                        MonsterInfo diyHorse29Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse29);
                        if (diyHorse29Info != null)
                        {
                            foreach (var kvp in diyHorse29Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse30:
                        MonsterInfo diyHorse30Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse30);
                        if (diyHorse30Info != null)
                        {
                            foreach (var kvp in diyHorse30Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse31:
                        MonsterInfo diyHorse31Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse31);
                        if (diyHorse31Info != null)
                        {
                            foreach (var kvp in diyHorse31Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse32:
                        MonsterInfo diyHorse32Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse32);
                        if (diyHorse32Info != null)
                        {
                            foreach (var kvp in diyHorse32Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse33:
                        MonsterInfo diyHorse33Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse33);
                        if (diyHorse33Info != null)
                        {
                            foreach (var kvp in diyHorse33Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse34:
                        MonsterInfo diyHorse34Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse34);
                        if (diyHorse34Info != null)
                        {
                            foreach (var kvp in diyHorse34Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse35:
                        MonsterInfo diyHorse35Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse35);
                        if (diyHorse35Info != null)
                        {
                            foreach (var kvp in diyHorse35Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse36:
                        MonsterInfo diyHorse36Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse36);
                        if (diyHorse36Info != null)
                        {
                            foreach (var kvp in diyHorse36Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse37:
                        MonsterInfo diyHorse37Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse37);
                        if (diyHorse37Info != null)
                        {
                            foreach (var kvp in diyHorse37Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse38:
                        MonsterInfo diyHorse38Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse38);
                        if (diyHorse38Info != null)
                        {
                            foreach (var kvp in diyHorse38Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse39:
                        MonsterInfo diyHorse39Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse39);
                        if (diyHorse39Info != null)
                        {
                            foreach (var kvp in diyHorse39Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    case HorseType.DiyHorse40:
                        MonsterInfo diyHorse40Info = SEnvir.MonsterInfoList.Binding.FirstOrDefault(x => x.Flag == MonsterFlag.DiyHorse40);
                        if (diyHorse40Info != null)
                        {
                            foreach (var kvp in diyHorse40Info.Stats.Values)
                            {
                                Stats[kvp.Key] += kvp.Value;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 算上钓鱼装备
            var allEquipments = Equipment.Concat(FishingEquipment).Where(x => x != null).ToList();
            // 全身装备生效的套装属性
            // 0持久不计算在内
            var validItemGroups = Functions.GetAllActiveItemSetGroups(allEquipments.Where(x => x.CurrentDurability > 0).Select(y => y.Info).ToList());

            foreach (UserItem item in allEquipments) //计算全身+钓鱼装备带来的属性
            {
                if (item == null || (item.CurrentDurability == 0 && item.Info.Durability > 0) || item.Info == null) continue;

                if (item.Info.ItemType == ItemType.HorseArmour && Character.Horse == HorseType.None) continue;

                Stats.Add(item.Info.Stats, item.Info.ItemType != ItemType.Weapon);
                Stats.Add(item.Stats, item.Info.ItemType != ItemType.Weapon);

                if (item.Info.ItemType == ItemType.Weapon)
                {
                    Stat ele = item.Stats.GetWeaponElement();

                    if (ele == Stat.None)
                        ele = item.Info.Stats.GetWeaponElement();

                    if (ele != Stat.None)
                        Stats[ele] += item.Stats.GetWeaponElementValue() + item.Info.Stats.GetWeaponElementValue();
                }
            }
            //组队不为空 且 组队成员大于8人
            if (GroupMembers != null && GroupMembers.Count >= 8)
            {
                int warrior = 0, wizard = 0, taoist = 0, assassin = 0;

                foreach (PlayerObject ob in GroupMembers)
                {
                    switch (ob.Class)
                    {
                        case MirClass.Warrior:
                            warrior++;
                            break;
                        case MirClass.Wizard:
                            wizard++;
                            break;
                        case MirClass.Taoist:
                            taoist++;
                            break;
                        case MirClass.Assassin:
                            assassin++;
                            break;
                    }
                }
                // 血蓝组队加成
                if (warrior >= 2 && wizard >= 2 && taoist >= 2 && assassin >= 2)
                {
                    Stats[Stat.Health] += Stats[Stat.BaseHealth] / 10;
                    Stats[Stat.Mana] += Stats[Stat.BaseMana] / 10;
                }
            }
            //技能对应的属性加成
            foreach (KeyValuePair<MagicType, UserMagic> pair in Magics)
            {
                if (Level < pair.Value.Info.NeedLevel1) continue;

                switch (pair.Key)
                {
                    case MagicType.Swordsmanship:
                        Stats[Stat.Accuracy] += pair.Value.Level * 3;
                        break;
                    case MagicType.SpiritSword:
                        Stats[Stat.Accuracy] += pair.Value.Level * 3;
                        break;
                    case MagicType.Slaying:
                        Stats[Stat.Accuracy] += pair.Value.Level * 1;
                        //Stats[Stat.MinDC] += pair.Value.Level * 2;
                        //Stats[Stat.MaxDC] += pair.Value.Level * 2;
                        break;
                    case MagicType.WillowDance:
                        Stats[Stat.Agility] += pair.Value.Level * 3;
                        break;
                    case MagicType.VineTreeDance:
                        Stats[Stat.Accuracy] += pair.Value.Level * 3;
                        break;
                    case MagicType.Discipline:
                        Stats[Stat.Accuracy] += pair.Value.GetPower() / 3;
                        Stats[Stat.MinDC] += pair.Value.GetPower();
                        break;
                    case MagicType.AdventOfDemon:
                        Stats[Stat.MaxAC] += pair.Value.GetPower();
                        break;
                    case MagicType.AdventOfDevil:
                        Stats[Stat.MaxMR] += pair.Value.GetPower();
                        break;
                    case MagicType.BloodyFlower:
                    case MagicType.AdvancedBloodyFlower:
                        Stats[Stat.LifeSteal] += pair.Value.GetPower();
                        break;
                    case MagicType.AdvancedRenounce:
                        Stats[Stat.MCPercent] += (1 + pair.Value.Level) * 10;
                        break;
                }
            }
            //BUFF加成
            foreach (BuffInfo buff in Buffs)
            {
                if (buff.Pause) continue;

                if (buff.Type == BuffType.ItemBuff)
                {
                    Stats.Add(SEnvir.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).Stats);
                    continue;
                }

                if (buff.Stats == null) continue;

                Stats.Add(buff.Stats);
            }
            //套装加成
            foreach (SetGroup group in validItemGroups)
            {
                foreach (SetInfoStat groupStat in group.GroupStats)
                {
                    if (Level < groupStat.Level) continue;

                    switch (Class)
                    {
                        case MirClass.Warrior:
                            if ((groupStat.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                            break;
                        case MirClass.Wizard:
                            if ((groupStat.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                            break;
                        case MirClass.Taoist:
                            if ((groupStat.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                            break;
                        case MirClass.Assassin:
                            if ((groupStat.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                            break;
                    }

                    Stats[groupStat.Stat] += groupStat.Amount;
                }
            }

            UserMagic magic;
            if (Buffs.Any(x => x.Type == BuffType.RagingWind) && Magics.TryGetValue(MagicType.RagingWind, out magic))
            {

                int power = Stats[Stat.MinAC] + Stats[Stat.MaxAC] + 4 + magic.Level * 6;

                Stats[Stat.MinAC] = power * 3 / 10;
                Stats[Stat.MaxAC] = power - Stats[Stat.MinAC];

                power = Stats[Stat.MinMR] + Stats[Stat.MaxMR] + 4 + magic.Level * 6;

                Stats[Stat.MinMR] = power * 3 / 10;
                Stats[Stat.MaxMR] = power - Stats[Stat.MinMR];
            }

            //攻击速度 每15级加1点，最高加3点攻击速度
            Stats[Stat.AttackSpeed] += Math.Min(3, Level / 15);         //攻击速度 += 数字最小值（3，等级/15）

            Stats[Stat.FireResistance] = Math.Min(Config.ElementResistance, Stats[Stat.FireResistance]);
            Stats[Stat.IceResistance] = Math.Min(Config.ElementResistance, Stats[Stat.IceResistance]);
            Stats[Stat.LightningResistance] = Math.Min(Config.ElementResistance, Stats[Stat.LightningResistance]);
            Stats[Stat.WindResistance] = Math.Min(Config.ElementResistance, Stats[Stat.WindResistance]);
            Stats[Stat.HolyResistance] = Math.Min(Config.ElementResistance, Stats[Stat.HolyResistance]);
            Stats[Stat.DarkResistance] = Math.Min(Config.ElementResistance, Stats[Stat.DarkResistance]);
            Stats[Stat.PhantomResistance] = Math.Min(Config.ElementResistance, Stats[Stat.PhantomResistance]);
            if (Config.PhysicalResistanceSwitch)
            {
                Stats[Stat.PhysicalResistance] = Math.Min(Config.ElementResistance, Stats[Stat.PhysicalResistance]);
            }

            Stats[Stat.Luck] = Math.Min(Config.MaxLucky, Stats[Stat.Luck]);  //幸运最高值设置

            Stats[Stat.Comfort] = Math.Min(Config.Comfort, Stats[Stat.Comfort]);  //舒适最高值设置
            Stats[Stat.AttackSpeed] = Math.Min(Config.AttackSpeed, Stats[Stat.AttackSpeed]);  //攻击设置最高值设置

            //回血时间=毫秒（1.5秒-舒适值*65）
            int HPRegenDelayMs = Math.Max(0, 15000 - Stats[Stat.Comfort] * 65);
            //回血速度属性
            //不可以超过100
            Stats[Stat.HPRegenRate] = Math.Min(100, Stats[Stat.HPRegenRate]);
            //不可以小于-100
            Stats[Stat.HPRegenRate] = Math.Max(-100, Stats[Stat.HPRegenRate]);
            //最终回血延迟
            HPRegenDelayMs = HPRegenDelayMs * (100 - Stats[Stat.HPRegenRate]) / 100;
            RegenDelay = TimeSpan.FromMilliseconds(HPRegenDelayMs);

            //回蓝时间=毫秒（1.5秒-舒适值*65）
            int MPRegenDelayMs = Math.Max(0, 15000 - Stats[Stat.Comfort] * 65);
            //回蓝速度属性
            //不可以超过100
            Stats[Stat.MPRegenRate] = Math.Min(100, Stats[Stat.MPRegenRate]);
            //不可以小于-100
            Stats[Stat.MPRegenRate] = Math.Max(-100, Stats[Stat.MPRegenRate]);
            //最终回蓝延迟
            MPRegenDelayMs = MPRegenDelayMs * (100 - Stats[Stat.MPRegenRate]) / 100;
            MPRegenDelay = TimeSpan.FromMilliseconds(MPRegenDelayMs);

            //无视物防属性 不可超过100
            Stats[Stat.ACIgnoreRate] = Math.Min(100, Stats[Stat.ACIgnoreRate]);
            //无视魔防属性 不可超过100
            Stats[Stat.MRIgnoreRate] = Math.Min(100, Stats[Stat.MRIgnoreRate]);

            Stats[Stat.Health] += (Stats[Stat.Health] * Stats[Stat.HealthPercent]) / 100;   //血量%
            Stats[Stat.Mana] += (Stats[Stat.Mana] * Stats[Stat.ManaPercent]) / 100;    //蓝量%

            Stats[Stat.MinAC] += (Stats[Stat.MinAC] * Stats[Stat.ACPercent]) / 100;    //物防%
            Stats[Stat.MaxAC] += (Stats[Stat.MaxAC] * Stats[Stat.ACPercent]) / 100;

            Stats[Stat.MinMR] += (Stats[Stat.MinMR] * Stats[Stat.MRPercent]) / 100;    //魔防%
            Stats[Stat.MaxMR] += (Stats[Stat.MaxMR] * Stats[Stat.MRPercent]) / 100;

            Stats[Stat.MinDC] += (Stats[Stat.MinDC] * Stats[Stat.DCPercent]) / 100;    //攻击%
            Stats[Stat.MaxDC] += (Stats[Stat.MaxDC] * Stats[Stat.DCPercent]) / 100;

            Stats[Stat.MinMC] += (Stats[Stat.MinMC] * Stats[Stat.MCPercent]) / 100;    //自然%
            Stats[Stat.MaxMC] += (Stats[Stat.MaxMC] * Stats[Stat.MCPercent]) / 100;

            Stats[Stat.MinSC] += (Stats[Stat.MinSC] * Stats[Stat.SCPercent]) / 100;    //灵魂%
            Stats[Stat.MaxSC] += (Stats[Stat.MaxSC] * Stats[Stat.SCPercent]) / 100;

            if (CurrentMap.Info.BlackScorching == true)          //如果月河属性开启
            {
                Stats[Stat.MinDC] = (Stats[Stat.MinDC] * Config.BC) / 100 + (Stats[Stat.MinBC]);              //攻击降低% +月河攻
                Stats[Stat.MaxDC] = (Stats[Stat.MaxDC] * Config.BC) / 100 + (Stats[Stat.MaxBC]);

                Stats[Stat.MinMC] = (Stats[Stat.MinMC] * Config.BC) / 100 + (Stats[Stat.MinBC]) / 2;         //魔法降低% +月河攻
                Stats[Stat.MaxMC] = (Stats[Stat.MaxMC] * Config.BC) / 100 + (Stats[Stat.MaxBC]) / 2;

                Stats[Stat.MinSC] = (Stats[Stat.MinSC] * Config.BC) / 100 + (Stats[Stat.MinBC]) / 2;        //道术减低% +月河攻
                Stats[Stat.MaxSC] = (Stats[Stat.MaxSC] * Config.BC) / 100 + (Stats[Stat.MaxBC]) / 2;

                Stats[Stat.MinAC] = (Stats[Stat.MinAC] * Config.BAC) / 100 + (Stats[Stat.MinBAC]);          //防御降低% + 月河防
                Stats[Stat.MaxAC] = (Stats[Stat.MaxAC] * Config.BAC) / 100 + (Stats[Stat.MaxBAC]);
                Stats[Stat.MinMR] = (Stats[Stat.MinMR] * Config.BAC) / 100 + (Stats[Stat.MinBAC]);
                Stats[Stat.MaxMR] = (Stats[Stat.MaxMR] * Config.BAC) / 100 + (Stats[Stat.MaxBAC]);
            }

            Stats[Stat.Health] += (Level * Stats[Stat.LvConvertHPMP]);   //破血魔镜隐藏属性
            Stats[Stat.Mana] += (Level * Stats[Stat.LvConvertHPMP]);  //破血魔镜隐藏属性

            Stats[Stat.Health] = Math.Max(10, Stats[Stat.Health]);
            Stats[Stat.Mana] = Math.Max(10, Stats[Stat.Mana]);

            if (Stats[Stat.Defiance] > 0)
            {
                Stats[Stat.MinAC] = Stats[Stat.MaxAC];
                Stats[Stat.MinMR] = Stats[Stat.MaxMR];
            }

            if (Buffs.Any(x => x.Type == BuffType.MagicWeakness))
            {
                Stats[Stat.MinMR] = 0;
                Stats[Stat.MaxMR] = 0;
            }

            if (Buffs.Any(x => x.Type == BuffType.PierceBuff))
            {
                Stats[Stat.FireResistance] = 0;
                Stats[Stat.IceResistance] = 0;
                Stats[Stat.LightningResistance] = 0;
                Stats[Stat.WindResistance] = 0;
                Stats[Stat.HolyResistance] = 0;
                Stats[Stat.DarkResistance] = 0;
                Stats[Stat.PhantomResistance] = 0;
            }

            if (Buffs.Any(x => x.Type == BuffType.BurnBuff))
            {
                Stats[Stat.MinDC] = Stats[Stat.MinDC] / 2;
                Stats[Stat.MaxDC] = Stats[Stat.MaxDC] / 2;
                Stats[Stat.MinMC] = Stats[Stat.MinMC] / 2;
                Stats[Stat.MaxMC] = Stats[Stat.MaxMC] / 2;
                Stats[Stat.MinSC] = Stats[Stat.MinSC] / 2;
                Stats[Stat.MaxSC] = Stats[Stat.MaxSC] / 2;
            }

            Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC]);
            Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC]);
            Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR]);
            Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR]);
            Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC]);
            Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC]);
            Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC]);
            Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC]);

            Stats[Stat.MinDC] = Math.Min(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Min(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Min(Stats[Stat.MinSC], Stats[Stat.MaxSC]);

            Stats[Stat.HandWeight] += Stats[Stat.HandWeight] * Stats[Stat.WeightRate];
            Stats[Stat.WearWeight] += Stats[Stat.WearWeight] * Stats[Stat.WeightRate];
            Stats[Stat.BagWeight] += Stats[Stat.BagWeight] * Stats[Stat.WeightRate];

            Stats[Stat.Rebirth] = Character.Rebirth;

            Stats[Stat.DropRate] += Math.Max(Config.RebirthDrop, 0) * Stats[Stat.Rebirth];  //转生爆率加成
            Stats[Stat.GoldRate] += Math.Max(Config.RebirthGold, 0) * Stats[Stat.Rebirth];  //转生金币加成

            //转生加属性
            Stats[Stat.MaxAC] += Math.Max(Config.RebirthAC, 0) * Stats[Stat.Rebirth];
            Stats[Stat.MaxMR] += Math.Max(Config.RebirthMAC, 0) * Stats[Stat.Rebirth];

            Stats[Stat.MaxDC] += Math.Max(Config.RebirthDC, 0) * Stats[Stat.Rebirth];
            Stats[Stat.MaxMC] += Math.Max(Config.RebirthMC, 0) * Stats[Stat.Rebirth];
            Stats[Stat.MaxSC] += Math.Max(Config.RebirthSC, 0) * Stats[Stat.Rebirth];

            if (Stats[Stat.ProtectBlood] > 0)
            {
                Stats[Stat.Health] = Stats[Stat.Health] + Stats[Stat.Mana] * Stats[Stat.ProtectBlood] / 100;
                Stats[Stat.Mana] = Stats[Stat.Mana] - Stats[Stat.Mana] * Stats[Stat.ProtectBlood] / 100;
            }

            Enqueue(new S.StatsUpdate { Stats = Stats, HermitStats = Character.HermitStats, HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints) });

            S.DataObjectMaxHealthMana p = new S.DataObjectMaxHealthMana { ObjectID = ObjectID, MaxHealth = Stats[Stat.Health], MaxMana = Stats[Stat.Mana] };

            foreach (PlayerObject player in DataSeenByPlayers)
                player.Enqueue(p);

            RefreshWeight();

            if (CurrentHP > Stats[Stat.Health]) SetHP(Stats[Stat.Health]);
            if (CurrentMP > Stats[Stat.Mana]) SetMP(Stats[Stat.Mana]);

            if (Spawned && tracking != Stats[Stat.PlayerTracker] + Stats[Stat.BossTracker])
            {
                RemoveAllObjects();
                AddAllObjects();
            }
        }
    }
}
