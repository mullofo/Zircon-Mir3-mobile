using Library.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Library
{
    public sealed class Stats   //属性状态
    {
        public SortedDictionary<Stat, int> Values { get; set; } = new SortedDictionary<Stat, int>();

        [IgnorePropertyPacket]
        public int Count => Values.Sum(pair => Math.Abs(pair.Value));

        [IgnorePropertyPacket]
        public int this[Stat stat]
        {
            get
            {
                int result;

                return !Values.TryGetValue(stat, out result) ? 0 : result;
            }
            set
            {
                if (value == 0)
                {
                    if (Values.ContainsKey(stat))
                        Values.Remove(stat);
                    return;
                }

                Values[stat] = value;
            }
        }

        public Stats()
        { }

        public Stats(Stats stats)
        {
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
                this[pair.Key] += pair.Value;
        }
        public Stats(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Values[(Stat)reader.ReadInt32()] = reader.ReadInt32();
        }
        public void Add(Stats stats, bool addElements = true)  //增加属性状态
        {
            foreach (KeyValuePair<Stat, int> pair in stats.Values)
                switch (pair.Key)
                {
                    case Stat.FireAttack:
                    case Stat.LightningAttack:
                    case Stat.IceAttack:
                    case Stat.WindAttack:
                    case Stat.HolyAttack:
                    case Stat.DarkAttack:
                    case Stat.PhantomAttack:
                        if (addElements)
                            this[pair.Key] += pair.Value;
                        break;
                    case Stat.ItemReviveTime:
                        if (pair.Value == 0) continue;

                        if (this[pair.Key] == 0)
                            this[pair.Key] = pair.Value;
                        else
                            this[pair.Key] = Math.Min(this[pair.Key], pair.Value);
                        break;
                    default:
                        //Stat附加功能
                        if (pair.Key.GetType().GetMember(pair.Key.ToString())[0].GetCustomAttribute<StatDescription>().Mode == StatType.Shape)
                        {
                            this[pair.Key] = pair.Value;
                        }
                        else
                            this[pair.Key] += pair.Value;
                        break;
                }
        }

        public void Write(BinaryWriter writer)  //写入(二进制写入程序)
        {
            writer.Write(Values.Count);

            foreach (KeyValuePair<Stat, int> pair in Values)
            {
                writer.Write((int)pair.Key);
                writer.Write(pair.Value);
            }

        }

        public bool Compare(Stats s2)
        {
            if (Values.Count != s2.Values.Count) return false;

            foreach (KeyValuePair<Stat, int> value in Values)
                if (s2[value.Key] != value.Value) return false;

            return true;
        }

        public void Clear()
        {
            Values.Clear();
        }

        public bool HasElementalWeakness()  //有基本弱点
        {
            return
                this[Stat.FireResistance] <= 0 && this[Stat.IceResistance] <= 0 && this[Stat.LightningResistance] <= 0 && this[Stat.WindResistance] <= 0 &&
                this[Stat.HolyResistance] <= 0 && this[Stat.DarkResistance] <= 0 &&
                this[Stat.PhantomResistance] <= 0 && this[Stat.PhysicalResistance] <= 0;
        }

        public Stat GetWeaponElement()  //获取武器元素
        {
            switch ((Element)this[Stat.WeaponElement])
            {
                case Element.Fire:
                    return Stat.FireAttack;
                case Element.Ice:
                    return Stat.IceAttack;
                case Element.Lightning:
                    return Stat.LightningAttack;
                case Element.Wind:
                    return Stat.WindAttack;
                case Element.Holy:
                    return Stat.HolyAttack;
                case Element.Dark:
                    return Stat.DarkAttack;
                case Element.Phantom:
                    return Stat.PhantomAttack;
            }

            foreach (KeyValuePair<Stat, int> pair in Values)
            {
                switch (pair.Key)
                {
                    case Stat.FireAttack:
                        return Stat.FireAttack;
                    case Stat.IceAttack:
                        return Stat.IceAttack;
                    case Stat.LightningAttack:
                        return Stat.LightningAttack;
                    case Stat.WindAttack:
                        return Stat.WindAttack;
                    case Stat.HolyAttack:
                        return Stat.HolyAttack;
                    case Stat.DarkAttack:
                        return Stat.DarkAttack;
                    case Stat.PhantomAttack:
                        return Stat.PhantomAttack;
                }
            }
            return Stat.None;
        }

        public Element GetWeaponElementAsElement()  //获取武器元素
        {
            switch ((Element)this[Stat.WeaponElement])
            {
                case Element.Fire:
                    return Element.Fire;
                case Element.Ice:
                    return Element.Ice;
                case Element.Lightning:
                    return Element.Lightning;
                case Element.Wind:
                    return Element.Wind;
                case Element.Holy:
                    return Element.Holy;
                case Element.Dark:
                    return Element.Dark;
                case Element.Phantom:
                    return Element.Phantom;
            }

            foreach (KeyValuePair<Stat, int> pair in Values)
            {
                switch (pair.Key)
                {
                    case Stat.FireAttack:
                        return Element.Fire;
                    case Stat.IceAttack:
                        return Element.Ice;
                    case Stat.LightningAttack:
                        return Element.Lightning;
                    case Stat.WindAttack:
                        return Element.Wind;
                    case Stat.HolyAttack:
                        return Element.Holy;
                    case Stat.DarkAttack:
                        return Element.Dark;
                    case Stat.PhantomAttack:
                        return Element.Phantom;
                }
            }
            return Element.None;
        }

        public int GetWeaponElementValue()  //获取武器元素值
        {
            return this[Stat.FireAttack] + this[Stat.IceAttack] + this[Stat.LightningAttack] + this[Stat.WindAttack] + this[Stat.HolyAttack] + this[Stat.DarkAttack] + this[Stat.PhantomAttack];
        }

        public int GetElementValue(Element element)  //获取元素值
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireAttack];
                case Element.Ice:
                    return this[Stat.IceAttack];
                case Element.Lightning:
                    return this[Stat.LightningAttack];
                case Element.Wind:
                    return this[Stat.WindAttack];
                case Element.Holy:
                    return this[Stat.HolyAttack];
                case Element.Dark:
                    return this[Stat.DarkAttack];
                case Element.Phantom:
                    return this[Stat.PhantomAttack];
                default:
                    return 0;
            }
        }

        public int GetAffinityValue(Element element)  //获取元素关联值
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireAffinity];
                case Element.Ice:
                    return this[Stat.IceAffinity];
                case Element.Lightning:
                    return this[Stat.LightningAffinity];
                case Element.Wind:
                    return this[Stat.WindAffinity];
                case Element.Holy:
                    return this[Stat.HolyAffinity];
                case Element.Dark:
                    return this[Stat.DarkAffinity];
                case Element.Phantom:
                    return this[Stat.PhantomAffinity];
                default:
                    return 0;
            }
        }
        public int GetResistanceValue(Element element)  //获取抵抗元素值
        {
            switch (element)
            {
                case Element.Fire:
                    return this[Stat.FireResistance];
                case Element.Ice:
                    return this[Stat.IceResistance];
                case Element.Lightning:
                    return this[Stat.LightningResistance];
                case Element.Wind:
                    return this[Stat.WindResistance];
                case Element.Holy:
                    return this[Stat.HolyResistance];
                case Element.Dark:
                    return this[Stat.DarkResistance];
                case Element.Phantom:
                    return this[Stat.PhantomResistance];
                case Element.None:
                    return this[Stat.PhysicalResistance];
                default:
                    return 0;
            }
        }
        public Element GetAffinityElement()
        {
            List<Element> elements = new List<Element>();

            if (this[Stat.FireAffinity] > 0)
                elements.Add(Element.Fire);

            if (this[Stat.IceAffinity] > 0)
                elements.Add(Element.Ice);

            if (this[Stat.LightningAffinity] > 0)
                elements.Add(Element.Lightning);

            if (this[Stat.WindAffinity] > 0)
                elements.Add(Element.Wind);

            if (this[Stat.HolyAffinity] > 0)
                elements.Add(Element.Holy);

            if (this[Stat.DarkAffinity] > 0)
                elements.Add(Element.Dark);

            if (this[Stat.PhantomAffinity] > 0)
                elements.Add(Element.Phantom);

            if (elements.Count == 0) return Element.None;

            return elements[Globals.Random.Next(elements.Count)];
        }
    }


    /// <summary>
    /// 属性来源
    /// </summary>
    public enum StatSource
    {
        /// <summary>
        /// 属性来源空置
        /// </summary>
        None,
        /// <summary>
        /// 属性来源额外增加
        /// </summary>
        Added,
        /// <summary>
        /// 属性来源精炼增加
        /// </summary>
        Refine,
        /// <summary>
        /// 属性来源临时增益BUFF
        /// </summary>
        Enhancement,
        /// <summary>
        /// 属性来源淬炼
        /// </summary>
        Other,
        /// <summary>
        /// 属性来源镶嵌宝石1
        /// </summary>
        Gem1,
        /// <summary>
        /// 属性来源镶嵌宝石2
        /// </summary>
        Gem2,
        /// <summary>
        /// 属性来源镶嵌宝石3
        /// </summary>
        Gem3,
        /// <summary>
        /// 属性来源符文
        /// </summary>
        Rune,
        /// <summary>
        /// 属性来源新版首饰合成
        /// </summary>
        Combine,
    }

    public enum StatType  //道具属性类型
    {
        /// <summary>
        /// 道具属性类型不显示
        /// </summary>
        None,
        /// <summary>
        /// 道具属性类型默认显示
        /// </summary>
        Default,
        /// <summary>
        /// 道具属性类型最小值显示
        /// </summary>
        Min,
        /// <summary>
        /// 道具属性类型最大值显示
        /// </summary>
        Max,
        /// <summary>
        /// 道具属性类型百分比显示
        /// </summary>
        Percent,
        /// <summary>
        /// 道具属性类型文本显示
        /// </summary>
        Text,
        /// <summary>
        /// 道具属性类型元素属性显示
        /// </summary>
        AttackElement,
        /// <summary>
        /// 道具属性类型防元素属性显示
        /// </summary>
        ElementResistance,
        /// <summary>
        /// 道具属性类型全系列魔法显示
        /// </summary>
        SpellPower,
        /// <summary>
        /// 道具属性类型时间显示
        /// </summary>
        Time,
        /// <summary>
        /// 道具属性类型舒适显示
        /// </summary>
        Comfort,
        /// <summary>
        /// 道具属性类型攻击速度显示
        /// </summary>
        AttackSpeed,
        /// <summary>
        /// 道具属性类型幸运显示
        /// </summary>
        Luck,
        /// <summary>
        /// 道具属性类型赋值叠加参数检查
        /// </summary>
        Shape,
    }

    /// <summary>
    /// 属性来源统计状态描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class StatDescription : Attribute
    {
        /// <summary>
        /// 属性来源统计状态标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 属性来源统计状态格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 属性来源统计状态样式
        /// </summary>
        public StatType Mode { get; set; }
        /// <summary>
        /// 属性来源统计状态最小值
        /// </summary>
        public Stat MinStat { get; set; }
        /// <summary>
        /// 属性来源统计状态最大值
        /// </summary>
        public Stat MaxStat { get; set; }
    }
}
