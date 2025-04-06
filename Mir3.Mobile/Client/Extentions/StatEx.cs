using Library;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Client.Extentions
{
    public static class StatEx
    {
        public static string GetDisplay(this Stats stats, Stat stat)
        {
            Type type = stat.GetType();

            MemberInfo[] infos = type.GetMember(stat.ToString());

            StatDescription description = infos[0].GetCustomAttribute<StatDescription>();

            if (description == null) return null;

            List<Stat> list;
            string value;
            bool neecComma;
            switch (description.Mode)
            {
                case StatType.None:
                    return null;
                case StatType.Default:
                    return stat.Lang() + ": " + string.Format(description.Format, stats[stat]);
                case StatType.Min:
                    if (stats[description.MaxStat] != 0) return null;

                    return stat.Lang() + ": " + string.Format(description.Format, stats[stat]);
                case StatType.Max:
                    return stat.Lang() + ": " + string.Format(description.Format, stats[description.MinStat], stats[stat]);
                case StatType.Percent:
                    return stat.Lang() + ": " + string.Format(description.Format, stats[stat] / 100D);
                case StatType.Text:
                    return stat.Lang();
                case StatType.AttackSpeed:
                    //if (Config.AttackSpeedValue)
                    return stat.Lang() + ": " + string.Format(description.Format, stats[stat] / 10D);

                //return stat.Lang() + ": " + string.Format(description.Format, stats[stat]);
                case StatType.Comfort:
                    //if (Config.ComfortValue)
                    return stat.Lang() + ": " + string.Format(description.Format, stats[stat] / 10D);

                //return stat.Lang() + ": " + string.Format(description.Format, stats[stat]);
                case StatType.Luck:
                    if (stats[stat] == 0) return null;

                    if (stats[stat] < 0)
                        return "诅咒".Lang() + ": " + string.Format(description.Format, stats[stat]);

                    return "幸运".Lang() + ": " + string.Format(description.Format, stats[stat]);
                case StatType.Time:
                    if (stats[stat] < 0)
                        return stat.Lang() + ": " + "永久".Lang();

                    return stat.Lang() + ": " + TimeSpan.FromSeconds(stats[stat]).Lang(true);
                case StatType.SpellPower:
                    if (description.MinStat == stat && stats[description.MaxStat] != 0) return null;

                    //0-xx
                    if (stats[Stat.MinMC] != stats[Stat.MinSC] || stats[Stat.MaxMC] != stats[Stat.MaxSC])
                        return stat.Lang() + ": " + string.Format(description.Format, stats[description.MinStat], stats[stat]);

                    if (stat != Stat.MaxSC)
                    {
                        //xx-0
                        if (stat == Stat.MinSC && stats[Stat.MinSC] != 0 && stats[Stat.MinMC] == stats[Stat.MinSC] && stats[Stat.MaxMC] == stats[Stat.MaxSC])
                            return "全系列魔法".Lang() + ": " + string.Format(description.Format, stats[description.MinStat], stats[description.MaxStat]);
                        else
                            return null;
                    }
                    //xx-yy
                    return "全系列魔法".Lang() + ": " + string.Format(description.Format, stats[description.MinStat], stats[stat]);
                case StatType.AttackElement:
                    list = new List<Stat>();
                    foreach (KeyValuePair<Stat, int> pair in stats.Values)
                        if (type.GetMember(pair.Key.ToString())[0].GetCustomAttribute<StatDescription>().Mode == StatType.AttackElement) list.Add(pair.Key);

                    if (list.Count == 0 || list[0] != stat)
                        return null;

                    value = $"攻击元素".Lang() + ": ";

                    neecComma = false;
                    foreach (Stat s in list)
                    {
                        description = type.GetMember(s.ToString())[0].GetCustomAttribute<StatDescription>();

                        if (neecComma)
                            value += $", ";

                        value += $"{s.Lang()}+" + stats[s];
                        neecComma = true;
                    }
                    return value;
                case StatType.ElementResistance:
                    list = new List<Stat>();
                    foreach (KeyValuePair<Stat, int> pair in stats.Values)
                    {
                        if (type.GetMember(pair.Key.ToString())[0].GetCustomAttribute<StatDescription>().Mode == StatType.ElementResistance) list.Add(pair.Key);
                    }

                    if (list.Count == 0)
                        return null;

                    bool ei;
                    bool hasAdv = false, hasDis = false;

                    foreach (Stat s in list)
                    {
                        if (stats[s] > 0)
                            hasAdv = true;

                        if (stats[s] < 0)
                            hasDis = true;
                    }

                    if (!hasAdv) // EV Online
                    {
                        ei = false;

                        if (list[0] != stat) return null;
                    }
                    else
                    {
                        if (!hasDis && list[0] != stat) return null;

                        ei = list[0] == stat;

                        if (!ei && list[1] != stat) return null; //Impossible to be false and have less than 2 stats.
                    }

                    value = ei ? $"强元素".Lang() + ": " : $"弱元素".Lang() + ": ";

                    neecComma = false;

                    foreach (Stat s in list)
                    {
                        description = type.GetMember(s.ToString())[0].GetCustomAttribute<StatDescription>();

                        if ((stats[s] > 0) != ei) continue;

                        if (neecComma)
                            value += $", ";

                        value += $"{s.Lang()}x" + Math.Abs(stats[s]);
                        neecComma = true;
                    }

                    return value;
                default: return null;
            }
        }
        /// <summary>
        /// 属性可以是负值，负值时显示不同颜色
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static string GetFormat(this Stats stats, Stat stat, bool isAdded = false)  //获取格式
        {
            Type type = stat.GetType();

            MemberInfo[] infos = type.GetMember(stat.ToString());

            StatDescription description = infos[0].GetCustomAttribute<StatDescription>();

            if (description == null) return null;

            switch (description.Mode)
            {
                case StatType.Default:
                case StatType.Luck:
                    return string.Format(description.Format, stats[stat]);
                case StatType.Min:
                    return stats[description.MaxStat] == 0 ? (string.Format(description.Format, stats[stat])) : null;
                case StatType.Max:
                case StatType.SpellPower:
                    return isAdded ? $"+{stats[stat]}" : string.Format(description.Format, stats[description.MinStat], stats[stat]);
                case StatType.Percent:
                    return isAdded ? $"+{stats[stat]}%" : string.Format(description.Format, stats[stat] / 100D);
                case StatType.Text:
                    return description.Format;
                case StatType.AttackSpeed:
                    //if (Config.AttackSpeedValue)
                    return string.Format(description.Format, stats[stat] / 10D);

                //return string.Format(description.Format, stats[stat]);
                case StatType.Comfort:
                    //if (Config.ComfortValue)
                    return string.Format(description.Format, stats[stat] / 10D);

                //return string.Format(description.Format, stats[stat]);
                case StatType.Time:
                    if (stats[stat] < 0)
                        return "永久".Lang();

                    return TimeSpan.FromSeconds(stats[stat]).Lang(true);
                default: return null;
            }
        }

    }
}
