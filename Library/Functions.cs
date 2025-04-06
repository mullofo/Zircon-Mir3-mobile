using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Library
{
    /// <summary>
    /// 功能
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// 最大时间跨度
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static TimeSpan Max(TimeSpan value1, TimeSpan value2)
        {
            return value1 > value2 ? value1 : value2;
        }
        /// <summary>
        /// 最小时间跨度
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static TimeSpan Min(TimeSpan value1, TimeSpan value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        /// <summary>
        /// 获取元素属性
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static Element GetElement(Stats stats)
        {
            Element attackElement = Element.None;
            int value = 0;

            if (stats[Stat.FireAttack] > value)
            {
                attackElement = Element.Fire;
                value = stats[Stat.FireAttack];
            }

            if (stats[Stat.IceAttack] > value)
            {
                attackElement = Element.Ice;
                value = stats[Stat.IceAttack];
            }

            if (stats[Stat.LightningAttack] > value)
            {
                attackElement = Element.Lightning;
                value = stats[Stat.LightningAttack];
            }

            if (stats[Stat.WindAttack] > value)
            {
                attackElement = Element.Wind;
                value = stats[Stat.WindAttack];
            }

            if (stats[Stat.HolyAttack] > value)
            {
                attackElement = Element.Holy;
                value = stats[Stat.HolyAttack];
            }

            if (stats[Stat.DarkAttack] > value)
            {
                attackElement = Element.Dark;
                value = stats[Stat.DarkAttack];
            }

            if (stats[Stat.PhantomAttack] > value)
                attackElement = Element.Phantom;

            return attackElement;
        }
        /// <summary>
        /// 获得元素值
        /// </summary>
        /// <param name="stats"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetElement(Stats stats, out Element element)
        {
            element = Element.None;
            int value = 0;

            if (stats[Stat.FireAttack] > value)
            {
                element = Element.Fire;
                value = stats[Stat.FireAttack];
            }

            if (stats[Stat.IceAttack] > value)
            {
                element = Element.Ice;
                value = stats[Stat.IceAttack];
            }

            if (stats[Stat.LightningAttack] > value)
            {
                element = Element.Lightning;
                value = stats[Stat.LightningAttack];
            }

            if (stats[Stat.WindAttack] > value)
            {
                element = Element.Wind;
                value = stats[Stat.WindAttack];
            }

            if (stats[Stat.HolyAttack] > value)
            {
                element = Element.Holy;
                value = stats[Stat.HolyAttack];
            }

            if (stats[Stat.DarkAttack] > value)
            {
                element = Element.Dark;
                value = stats[Stat.DarkAttack];
            }

            if (stats[Stat.PhantomAttack] > value)
            {
                element = Element.Phantom;
                value = stats[Stat.PhantomAttack];
            }

            return value;
        }

        /// <summary>
        /// 获得攻击动画效果
        /// </summary>
        /// <param name="c"></param>
        /// <param name="w"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MirAnimation GetAttackAnimation(MirClass c, int w, MagicType m)
        {
            MirAnimation animation;

            switch (m)
            {
                case MagicType.Slaying:
                case MagicType.Thrusting:
                case MagicType.FlamingSword:
                    animation = MirAnimation.Combat3;
                    break;
                case MagicType.HalfMoon:
                case MagicType.DestructiveSurge:
                    animation = MirAnimation.Combat4;
                    break;
                case MagicType.DragonRise:
                    animation = MirAnimation.Combat5;
                    break;
                case MagicType.BladeStorm:
                case MagicType.MaelstromBlade:
                    animation = MirAnimation.Combat6;
                    break;
                case MagicType.ThousandBlades:
                    animation = MirAnimation.Combat10;
                    break;
                case MagicType.FullBloom:
                case MagicType.WhiteLotus:
                case MagicType.RedLotus:
                case MagicType.DanceOfSwallow:
                    if (w >= 1200)
                        animation = MirAnimation.Combat13;
                    else if (w >= 1100)
                        animation = MirAnimation.Combat5;
                    else
                        animation = MirAnimation.Combat3;
                    break;
                case MagicType.SweetBrier:
                case MagicType.Karma:
                    if (w >= 1200)
                        animation = MirAnimation.Combat12;
                    else if (w >= 1100)
                        animation = MirAnimation.Combat10;
                    else
                        animation = MirAnimation.Combat3;
                    break;
                default:
                    switch (c)
                    {
                        case MirClass.Assassin:
                            if (w >= 1200)
                                animation = MirAnimation.Combat11;
                            else if (w >= 1100)
                                animation = MirAnimation.Combat4;
                            else
                                animation = MirAnimation.Combat3;
                            break;
                        default:
                            animation = MirAnimation.Combat3;
                            //switch weapon shape
                            break;
                    }

                    break;
            }

            return animation;
        }

        /// <summary>
        /// 获得魔法攻击效果
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static MirAnimation GetMagicAnimation(MagicType m)
        {
            switch (m)
            {
                case MagicType.Beckon:
                case MagicType.MassBeckon:

                case MagicType.FireBall:
                case MagicType.IceBolt:
                case MagicType.LightningBall:
                case MagicType.GustBlast:
                case MagicType.ScortchedEarth:
                case MagicType.LightningBeam:
                case MagicType.AdamantineFireBall:
                case MagicType.IceBlades:
                case MagicType.FrozenEarth:
                case MagicType.MeteorShower:

                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                case MagicType.MagicResistance:
                case MagicType.Resilience:
                case MagicType.MassInvisibility:
                case MagicType.GreaterEvilSlayer:
                case MagicType.GreaterFrozenEarth:
                case MagicType.Infection:

                case MagicType.ElementalSuperiority:
                case MagicType.BloodLust:
                case MagicType.LifeSteal:
                case MagicType.ImprovedExplosiveTalisman:
                case MagicType.Neutralize:
                case MagicType.GreaterHolyStrike:
                    return MirAnimation.Combat1;

                case MagicType.Interchange:

                case MagicType.Repulsion:
                case MagicType.ElectricShock:
                case MagicType.LightningWave:
                case MagicType.Cyclone:
                case MagicType.Teleportation:
                case MagicType.FireWall:
                case MagicType.FireStorm:
                case MagicType.BlowEarth:
                case MagicType.ExpelUndead:
                case MagicType.MagicShield:
                case MagicType.IceStorm:
                case MagicType.DragonTornado:
                case MagicType.ChainLightning:
                case MagicType.GeoManipulation:
                case MagicType.Transparency:
                case MagicType.ThunderBolt:
                case MagicType.Renounce:
                case MagicType.FrostBite:
                case MagicType.Tempest:
                case MagicType.IceRain:
                case MagicType.JudgementOfHeaven:
                case MagicType.ThunderStrike:
                case MagicType.Asteroid:
                case MagicType.SuperiorMagicShield:
                    return MirAnimation.Combat2;

                case MagicType.ElementalHurricane:
                    return MirAnimation.ChannellingStart;

                case MagicType.ThousandBlades:
                    return MirAnimation.Combat10;

                case MagicType.Heal:
                case MagicType.PoisonDust:
                case MagicType.Invisibility:
                case MagicType.TrapOctagon:
                case MagicType.MassHeal:
                case MagicType.MassTransparency:
                case MagicType.Resurrection:
                case MagicType.Purification:
                case MagicType.SummonSkeleton:
                case MagicType.SummonJinSkeleton:
                case MagicType.SummonShinsu:
                case MagicType.StrengthOfFaith:
                case MagicType.CelestialLight:
                case MagicType.GreaterPoisonDust:
                case MagicType.SummonDemonicCreature:
                case MagicType.DemonExplosion:
                case MagicType.Scarecrow:
                case MagicType.DarkSoulPrison:
                case MagicType.MirrorImage:
                    return MirAnimation.Combat2;

                case MagicType.PoisonousCloud:
                case MagicType.SummonPuppet:
                    return MirAnimation.Combat14;
                case MagicType.DragonRepulse:
                    return MirAnimation.DragonRepulseStart;

                case MagicType.ThunderKick:
                case MagicType.TaoistCombatKick:
                    return MirAnimation.Combat7;

                case MagicType.Cloak:
                case MagicType.WraithGrip:
                case MagicType.HellFire:
                case MagicType.TheNewBeginning:
                case MagicType.DarkConversion:
                case MagicType.Abyss:
                case MagicType.Evasion:
                case MagicType.RagingWind:
                case MagicType.Concentration:
                case MagicType.SwordOfVengeance:
                    return MirAnimation.Combat9;

                case MagicType.Rake:
                    return MirAnimation.Combat5;
                case MagicType.FlashOfLight:
                    return MirAnimation.Combat10;

                case MagicType.Defiance:
                case MagicType.Might:
                case MagicType.ReflectDamage:
                case MagicType.Fetter:
                case MagicType.Invincibility:
                    return MirAnimation.Combat15;

                case MagicType.SwiftBlade:
                case MagicType.SeismicSlam:
                case MagicType.CrushingWave:
                    return MirAnimation.Combat3;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offSet"></param>
        /// <returns></returns>
        public static bool IsMatch(byte[] a, byte[] b, long offSet = 0)
        {
            if (b == null || a == null || b.Length + offSet > a.Length || offSet < 0) return false;

            for (int i = 0; i < b.Length; i++)
                if (a[offSet + i] != b[i])
                    return false;

            return true;
        }
        /// <summary>
        /// 是否匹配属性
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsMatch(Stats a, Stats b)
        {
            if (a == null || b == null || a.Values.Count != b.Values.Count) return false;

            foreach (KeyValuePair<Stat, int> pair in a.Values)
                if (pair.Value != b[pair.Key]) return false;

            return true;
        }

        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Color Lerp(Color source, Color destination, float rate)
        {
            if (rate >= 1)
                return destination;

            if (rate <= 0)
                return source;

            int a = destination.A - source.A, r = destination.R - source.R, g = destination.G - source.G, b = destination.B - source.B;

            return Color.FromArgb(Math.Min(byte.MaxValue, source.A + (int)(a * rate)), Math.Min(byte.MaxValue, source.R + (int)(r * rate)),
                                             Math.Min(byte.MaxValue, source.G + (int)(g * rate)), Math.Min(byte.MaxValue, source.B + (int)(b * rate)));
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static int Distance(Point source, Point dest)
        {
            return Math.Max(Math.Abs(source.X - dest.X), Math.Abs(source.Y - dest.Y));
        }

        /// <summary>
        /// 当你只能向上、向下、向左或向右移动时，两点距离有多远？（无对角线）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static int Distance4Directions(Point source, Point dest)
        {
            return Math.Abs(source.X - dest.X) + Math.Abs(source.Y - dest.Y);
        }
        public static bool InRange(Point a, Point b, int i)
        {
            return InRange(a.X, a.Y, b.X, b.Y, i);
        }
        public static bool InRange(int x1, int y1, int x2, int y2, int i)
        {
            return Math.Abs(x1 - x2) <= i && Math.Abs(y1 - y2) <= i;
        }
        //code by cxx 
        public static MirDirection DirectionFromPoint(Point src, Point dst)
        {
            //对角线
            int cx = 0;
            int cy = 0;
            if (src.X < dst.X)
            {
                cx = 1;
            }
            else if (src.X == dst.X)
            {
                cx = 0;
            }
            else
            {
                cx = -1;
            }

            if (Math.Abs(src.Y - dst.Y) > 2)
            {
                if ((src.X > dst.X - 1) && src.X <= dst.X + 1)
                {
                    cx = 0;
                }
            }

            if (src.Y < dst.Y)
            {
                cy = 1;
            }
            else if (src.Y == dst.Y)
            {
                cy = 0;
            }
            else
            {
                cy = -1;
            }

            if (Math.Abs(src.X - dst.X) > 2)
            {
                if ((src.Y >= dst.Y - 1) && (src.Y <= dst.Y + 1))
                {
                    cy = 0;
                }
            }

            if ((cx == 0) && (cy == -1))
            {
                return MirDirection.Up;
            }
            else if (cx == 1 && cy == -1)
            {
                return MirDirection.UpRight;
            }
            else if (cx == 1 && cy == 0)
            {
                return MirDirection.Right;
            }
            else if (cx == 1 && cy == 1)
            {
                return MirDirection.DownRight;
            }
            else if (cx == 0 && cy == 1)
            {
                return MirDirection.Down;
            }
            else if (cx == -1 && cy == 1)
            {
                return MirDirection.DownLeft;
            }
            else if (cx == -1 && cy == 0)
            {
                return MirDirection.Left;
            }
            else if (cx == -1 && cy == -1)
            {
                return MirDirection.UpLeft;
            }
            else
            {
                return MirDirection.Down;
            }
        }
        public static MirDirection DirectionFromPoint_(Point source, Point dest)  //从点开始的方向
        {
            if (source.X < dest.X)
            {
                if (source.Y < dest.Y)
                    return MirDirection.DownRight;
                if (source.Y > dest.Y)
                    return MirDirection.UpRight;
                return MirDirection.Right;
            }

            if (source.X > dest.X)
            {
                if (source.Y < dest.Y)
                    return MirDirection.DownLeft;
                if (source.Y > dest.Y)
                    return MirDirection.UpLeft;
                return MirDirection.Left;
            }

            return source.Y < dest.Y ? MirDirection.Down : MirDirection.Up;
        }

        public static Point PointNearTarget(Point start, Point end, int randomSteps = 1)
        {
            int x = end.X;
            int y = end.Y;
            if (start.X < end.X)
            {
                x -= randomSteps;
            }
            else if (start.X > end.X)
            {
                x += randomSteps;
            }

            if (start.Y < end.Y)
            {
                y -= randomSteps;
            }
            else if (start.Y > end.Y)
            {
                y += randomSteps;
            }

            return new Point(x, y);
        }

        public static double Distance(PointF p1, PointF p2)  //距离
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }
        /// <summary>
        /// 移动方向
        /// </summary>
        /// <param name="dir">方向</param>
        /// <param name="i">位置</param>
        /// <returns></returns>
        public static MirDirection ShiftDirection(MirDirection dir, int i)
        {
            return (MirDirection)(((int)dir + i + 8) % 8);
        }
        public static Point Move(Point location, MirDirection direction, int distance = 1)
        {
            switch (direction)
            {
                case MirDirection.Up:
                    location.Offset(0, -distance);
                    break;
                case MirDirection.UpRight:
                    location.Offset(distance, -distance);
                    break;
                case MirDirection.Right:
                    location.Offset(distance, 0);
                    break;
                case MirDirection.DownRight:
                    location.Offset(distance, distance);
                    break;
                case MirDirection.Down:
                    location.Offset(0, distance);
                    break;
                case MirDirection.DownLeft:
                    location.Offset(-distance, distance);
                    break;
                case MirDirection.Left:
                    location.Offset(-distance, 0);
                    break;
                case MirDirection.UpLeft:
                    location.Offset(-distance, -distance);
                    break;
            }
            return location;
        }
        /// <summary>
        /// 正确的装备格子
        /// </summary>
        /// <param name="type"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool CorrectSlot(ItemType type, EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return type == ItemType.Weapon;
                case EquipmentSlot.Armour:
                    return type == ItemType.Armour;
                case EquipmentSlot.Helmet:
                    return type == ItemType.Helmet;
                case EquipmentSlot.Torch:
                    return type == ItemType.Torch;
                case EquipmentSlot.Necklace:
                    return type == ItemType.Necklace;
                case EquipmentSlot.BraceletL:
                case EquipmentSlot.BraceletR:
                    return type == ItemType.Bracelet;
                case EquipmentSlot.RingL:
                case EquipmentSlot.RingR:
                    return type == ItemType.Ring;
                case EquipmentSlot.Shoes:
                    return type == ItemType.Shoes;
                case EquipmentSlot.Poison:
                    return type == ItemType.Poison;
                case EquipmentSlot.Amulet:
                    return type == ItemType.Amulet || type == ItemType.DarkStone;
                case EquipmentSlot.Flower:
                    return type == ItemType.Flower;
                case EquipmentSlot.Emblem:
                    return type == ItemType.Emblem;
                case EquipmentSlot.HorseArmour:
                    return type == ItemType.HorseArmour;
                case EquipmentSlot.Shield:
                    return type == ItemType.Shield;
                case EquipmentSlot.FameTitle:
                    return type == ItemType.FameTitle;
                case EquipmentSlot.Fashion:
                    return type == ItemType.Fashion;
                case EquipmentSlot.Medicament:
                    return type == ItemType.Medicament;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 正确的宠物装备格子
        /// </summary>
        /// <param name="type"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool CorrectSlot(ItemType type, CompanionSlot slot)
        {
            switch (slot)
            {
                case CompanionSlot.Bag:
                    return type == ItemType.CompanionBag;
                case CompanionSlot.Head:
                    return type == ItemType.CompanionHead;
                case CompanionSlot.Back:
                    return type == ItemType.CompanionBack;
                case CompanionSlot.Food:
                    return type == ItemType.CompanionFood;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 正确的钓鱼装备格子
        /// </summary>
        /// <param name="type"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool CorrectSlot(ItemType type, FishingSlot slot)
        {
            switch (slot)
            {
                case FishingSlot.Hook:
                    return type == ItemType.Hook;
                case FishingSlot.Float:
                    return type == ItemType.Float;
                case FishingSlot.Bait:
                    return type == ItemType.Bait;
                case FishingSlot.Finder:
                    return type == ItemType.Finder;
                case FishingSlot.Reel:
                    return type == ItemType.Reel;
                default:
                    return false;
            }
        }

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new PointF(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static int CalcDirection16(int nFireTileX, int nFireTileY, int nDestTileX, int nDestTileY)
        {
            int nWidth = nDestTileX - nFireTileX;
            int nHeight = nDestTileY - nFireTileY;

            float rLineLength, rBottomInTriangle;
            int nDimension;
            int bDir;
            rLineLength = (float)Math.Sqrt((float)nWidth * nWidth + nHeight * nHeight);

            // 扁夯.
            // 7  0  1          
            // 6     2
            // 5  4  3
            // 老窜篮 4俺狼 盒搁(90档扁霖)栏肺 唱穿绊 盒搁俊 措茄 cos蔼阑 利侩茄促.
            if (nWidth >= 0)
            {
                if (nHeight < 0)
                {
                    rBottomInTriangle = (float)-nHeight;
                    nDimension = 0;
                }
                else
                {
                    rBottomInTriangle = (float)nWidth;
                    nDimension = 4;
                }
            }
            else
            {
                if (nHeight >= 0)
                {
                    rBottomInTriangle = (float)nHeight;
                    nDimension = 8;
                }
                else
                {
                    rBottomInTriangle = (float)-nWidth;
                    nDimension = 12;
                }
            }
            // 6(cos45)  0(cos 0)  0(cos45)
            // 4(cos90)  2(cos 0)  2(cos 0)
            // 4(cos45)  2(cos90)  2(cos45)

            if (rLineLength == 0.0f)
            {
                return 0;
            }

            float rCosVal = rBottomInTriangle / rLineLength;

            float[] rCosVal16 = new float[8] { 1.0f, 0.980785f, 0.923880f, 0.831470f, 0.707107f, 0.555570f, 0.382683f, 0.195090f };

            // 阿盒搁阑 3俺狼 康开栏肺 唱穿绢辑 康开阑 犁炼沥茄促.
            bDir = 0;
            for (int nCnt = 0; nCnt < 8; nCnt++)
            {
                if (rCosVal <= rCosVal16[nCnt])
                {
                    bDir = (nDimension + (nCnt + 1) / 2);
                }
                else
                {
                    break;
                }
            }

            if (bDir >= 16) bDir = 0;

            return bDir;
        }

        public static string RandomString(Random Random, int length)  //随机字符串
        {
            StringBuilder str = new StringBuilder();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

            for (int i = 0; i < length; i++)
                str.Append(chars[Random.Next(chars.Length)]);

            return str.ToString();
        }

        public static void Shuffle<T>(this IList<T> list, Random random) //打乱一个列表
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        // 返回给定enum值的描述
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        /// <summary>
        /// 将精炼类型的元素细化
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Element RefineTypeToElement(RefineType t)
        {
            switch (t)
            {
                case RefineType.Fire:
                    return Element.Fire;
                case RefineType.Ice:
                    return Element.Ice;
                case RefineType.Lightning:
                    return Element.Lightning;
                case RefineType.Wind:
                    return Element.Wind;
                case RefineType.Holy:
                    return Element.Holy;
                case RefineType.Dark:
                    return Element.Dark;
                case RefineType.Phantom:
                    return Element.Phantom;
                default:
                    return Element.None;
            }
        }
        /// <summary>
        /// 将装备类型元素细化
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Stat ElementToStat(Element e)
        {
            switch (e)
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
                    return Stat.HolyAttack; ;
                case Element.Dark:
                    return Stat.DarkAttack;
                case Element.Phantom:
                    return Stat.PhantomAttack;
                default:
                    return Stat.None;
            }
        }
        /// <summary>
        /// 通过自定义技能动画获得自定义动画类型
        /// </summary>
        /// <param name="mirAnimation"></param>
        /// <returns></returns>
        public static DiyMagicType GetDiyMagicTypeByAction(MirAnimation mirAnimation)
        {
            DiyMagicType diyMagicType = DiyMagicType.None;
            switch (mirAnimation)
            {
                case MirAnimation.Standing:
                    break;
                case MirAnimation.Walking:
                    break;
                case MirAnimation.CreepStanding:
                    break;
                case MirAnimation.CreepWalkSlow:
                    break;
                case MirAnimation.CreepWalkFast:
                    break;
                case MirAnimation.Running:
                    break;
                case MirAnimation.Pushed:
                    break;
                case MirAnimation.Combat1:
                    diyMagicType = DiyMagicType.MonCombat1;
                    break;
                case MirAnimation.Combat2:
                    diyMagicType = DiyMagicType.MonCombat2;
                    break;
                case MirAnimation.Combat3:
                    diyMagicType = DiyMagicType.MonCombat3;
                    break;
                case MirAnimation.Combat4:
                    diyMagicType = DiyMagicType.MonCombat4;
                    break;
                case MirAnimation.Combat5:
                    diyMagicType = DiyMagicType.MonCombat5;
                    break;
                case MirAnimation.Combat6:
                    diyMagicType = DiyMagicType.MonCombat6;
                    break;
                case MirAnimation.Combat7:
                    break;
                case MirAnimation.Combat8:
                    break;
                case MirAnimation.Combat9:
                    break;
                case MirAnimation.Combat10:
                    break;
                case MirAnimation.Combat11:
                    break;
                case MirAnimation.Combat12:
                    break;
                case MirAnimation.Combat13:
                    break;
                case MirAnimation.Combat14:
                    break;
                case MirAnimation.Combat15:
                    break;
                case MirAnimation.Harvest:
                    break;
                case MirAnimation.Stance:
                    break;
                case MirAnimation.Struck:
                    break;
                case MirAnimation.Die:
                    diyMagicType = DiyMagicType.MonDie;
                    break;
                case MirAnimation.Dead:
                    break;
                case MirAnimation.Skeleton:
                    break;
                case MirAnimation.Show:
                    break;
                case MirAnimation.Hide:
                    break;
                case MirAnimation.HorseStanding:
                    break;
                case MirAnimation.HorseWalking:
                    break;
                case MirAnimation.HorseRunning:
                    break;
                case MirAnimation.HorseStruck:
                    break;
                case MirAnimation.StoneStanding:
                    break;
                case MirAnimation.DragonRepulseStart:
                    break;
                case MirAnimation.DragonRepulseMiddle:
                    break;
                case MirAnimation.DragonRepulseEnd:
                    break;
                case MirAnimation.ChannellingStart:
                    break;
                case MirAnimation.ChannellingMiddle:
                    break;
                case MirAnimation.ChannellingEnd:
                    break;
                default:
                    break;
            }
            return diyMagicType;
        }

        /* 等待某条件成立
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1, string timeoutMsg = "超时")
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                Task.Delay(timeout)))
            {
                MessageBox.Show(timeoutMsg);
            }
        }

        */

        // 带权重的随机
        // KeyValuePair<object, 权重>
        public static T WeightedRandom<T>(Random random, List<KeyValuePair<T, int>> list)
        {
            int total = list.Sum(x => x.Value);
            int rand = random.Next(total);
            int current = 0;
            foreach (var kvp in list)
            {
                current += kvp.Value;
                if (rand < current)
                {
                    return kvp.Key;
                }
            }

            return default;
        }

        // 是否是位移技能
        // 瞬息移动Teleportation /乾坤大挪移Interchange /斗转星移Beckon /移行换位GeoManipulation /鹰击DanceOfSwallow /妙影无踪Transparency
        public static bool IsTeleportMagic(MagicType magic)
        {
            switch (magic)
            {
                case MagicType.Teleportation:
                case MagicType.Interchange:
                case MagicType.Beckon:
                case MagicType.GeoManipulation:
                case MagicType.DanceOfSwallow:
                case MagicType.Transparency:
                    return true;
            }

            return false;
        }

        // 获取指定物品所处的所有Set
        public static List<SetInfo> GetAllSetInfo(ItemInfo item)
        {
            if (item != null)
            {
                var groups = GetAllSetGroup(item);
                if (groups.Count > 0)
                {
                    return groups.GroupBy(x => x.Set.SetName).Select(y => y.First().Set).ToList();
                }
            }

            return new List<SetInfo>();
        }

        // 获取指定物品所处的所有SetGroup
        public static List<SetGroup> GetAllSetGroup(ItemInfo item)
        {
            if (item != null)
            {
                return Globals.SetGroupList.Binding.Where(
                    x => x.SetGroupItems.Any(
                        y => y.SetGroupItemInfo.Index == item.Index)).ToList();
            }

            return new List<SetGroup>();
        }

        // 判断穿戴齐全的套装
        public static List<SetGroup> GetAllActiveItemSetGroups(List<ItemInfo> itemsWearing)
        {
            List<SetGroup> validGroups = new List<SetGroup>();

            if (itemsWearing != null)
            {
                // 遍历每一件装备
                List<ItemInfo> equipmentsCopy = itemsWearing.Select(x => x).ToList();

                foreach (var equipment in itemsWearing)
                {
                    // 找到所属的group
                    var setGroups = Globals.SetGroupList.Binding.Where(x => x.SetGroupItems.Any(y => y.SetGroupItemInfo.Index == equipment.Index)).ToList();

                    if (setGroups.Count < 1) continue;

                    // 判断其所属的每一个group是否穿戴齐全
                    foreach (SetGroup group in setGroups)
                    {
                        // 需要穿戴的件数
                        int requiredNum = group.GetRequiredNumItem();
                        // 判断身上穿的件数
                        int wearing = group.GetNumItemsWearing(equipmentsCopy);

                        /*
                        // 这里特殊处理一下2件相同装备组成套装的情况
                        if (requiredNum == 2)
                        {
                            var neededItems = group.GetSetGroupItemInfoList();
                            if (neededItems[0].Index == neededItems[1].Index)
                            {
                                var validItems = equipmentsCopy.Where(neededItems.Remove);
                                wearing = validItems.Count();
                            }
                        }

                        */

                        // 符合条件
                        if (wearing >= requiredNum)
                        {
                            if (group.SetRequirement == ItemSetRequirementType.WithoutReplacement)
                            {
                                // 不多次触发
                                foreach (var info in group.GetSetGroupItemInfoList())
                                {
                                    equipmentsCopy.Remove(info);
                                }
                                validGroups.Add(group);
                            }
                            else
                            {
                                if (!validGroups.Contains(group))
                                    validGroups.Add(group);
                            }

                        }

                    }
                }

            }


            return validGroups;
        }

        // 获取完整的道具套装信息
        // {套装名字 : {active: [groups], inactive: [groups]}}
        public static Dictionary<string, Dictionary<string, List<SetGroup>>> GetAllItemSetDetails(ItemInfo target,
            List<ItemInfo> itemsWearing, bool showHidden = true)
        {
            Dictionary<string, Dictionary<string, List<SetGroup>>> result =
                new Dictionary<string, Dictionary<string, List<SetGroup>>>();

            List<SetGroup> activeGroups = GetAllActiveItemSetGroups(itemsWearing);

            foreach (var setGroup in GetAllSetGroup(target))
            {
                if (setGroup.Set == null) continue;

                string setName = setGroup.Set.SetName;

                if (!showHidden && !setGroup.Set.SetInfoShow)
                {
                    // 不显示套装信息
                    continue;
                }

                if (!result.ContainsKey(setName))
                {
                    result[setName] = new Dictionary<string, List<SetGroup>>
                    {
                        ["active"] = new List<SetGroup>(),
                        ["inactive"] = new List<SetGroup>()
                    };
                }

                if (activeGroups.Contains(setGroup))
                {
                    result[setName]["active"].Add(setGroup);
                }
                else
                {
                    result[setName]["inactive"].Add(setGroup);
                }
            }

            return result;
        }

        /// <summary>
        /// 判断给定时间是否处于某时间段内
        /// </summary>
        /// <param name="target"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool DateTimeWithinRange(DateTime target, DateTime start, DateTime end)
        {
            return target >= start && target < end;
        }

        public static Point ScalePoint(Point p, float f)
        {
            return new Point((int)Math.Round((float)p.X * f), (int)Math.Round((float)p.Y * f));
        }

        public static Point ScalePointXOffset(Point p, float f, int x)
        {
            return new Point((int)Math.Round((float)p.X * f) + x, (int)Math.Round((float)p.Y * f));
        }

        public static Size ScaleSize(Size s, float f)
        {
            return new Size((int)Math.Round((float)s.Width * f), (int)Math.Round((float)s.Height * f));
        }

        public static Rectangle ScaleRectangle(Rectangle r, float f)
        {
            return new Rectangle(r.X, r.Y, (int)Math.Round((float)r.Width * f), (int)Math.Round((float)r.Height * f));
        }
    }
}
