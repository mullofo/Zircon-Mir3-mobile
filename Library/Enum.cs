using System;
using System.ComponentModel;
using static MirDB.Association;

namespace Library
{
    public enum Language
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [Description("中")]
        SimplifiedChinese,
        /// <summary>
        /// 英语
        /// </summary>
        [Description("英")]
        English,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [Description("繁")]
        TraditionalChinese,
        /// <summary>
        /// 韩语
        /// </summary>
        [Description("韩")]
        Korean,
    }
    /// <summary>
    /// Stat属性状态
    /// </summary>
    [Lang("Stat属性状态")]
    public enum Stat
    {
        /// <summary>
        /// 基础生命值
        /// </summary>
        [StatDescription(Title = "基础生命值", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        [Description("BaseHealth 基础生命值 0")]
        BaseHealth,
        /// <summary>
        /// 基础魔法值
        /// </summary>
        [StatDescription(Title = "基础魔法值", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        [Description("BaseMana 基础魔法值 1")]
        BaseMana,
        /// <summary>
        /// 生命值
        /// </summary>
        [StatDescription(Title = "生命HP", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Health 生命值 2")]
        Health,
        /// <summary>
        /// 魔法值
        /// </summary>
        [StatDescription(Title = "魔法MP", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Mana 魔法值 3")]
        Mana,
        /// <summary>
        /// 最小物防
        /// </summary>
        [StatDescription(Title = "物理防御", Format = "{0}-0", Mode = StatType.Min, MinStat = MinAC, MaxStat = MaxAC)]
        [Description("MinAC 最小物防 4")]
        MinAC,
        /// <summary>
        /// 最大物防
        /// </summary>
        [StatDescription(Title = "物理防御", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinAC, MaxStat = MaxAC)]
        [Description("MaxAC 最大物防 5")]
        MaxAC,
        /// <summary>
        /// 最小魔防
        /// </summary>
        [StatDescription(Title = "魔法防御", Format = "{0}-0", Mode = StatType.Min, MinStat = MinMR, MaxStat = MaxMR)]
        [Description("MinMR 最小魔防 6")]
        MinMR,
        /// <summary>
        /// 最大魔防
        /// </summary>
        [StatDescription(Title = "魔法防御", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinMR, MaxStat = MaxMR)]
        [Description("MaxMR 最大魔防 7")]
        MaxMR,
        /// <summary>
        /// 最小破坏
        /// </summary>
        [StatDescription(Title = "破坏", Format = "{0}-0", Mode = StatType.Min, MinStat = MinDC, MaxStat = MaxDC)]
        [Description("MinDC 最小破坏 8")]
        MinDC,
        /// <summary>
        /// 最大破坏
        /// </summary>
        [StatDescription(Title = "破坏", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinDC, MaxStat = MaxDC)]
        [Description("MaxDC 最大破坏 9")]
        MaxDC,
        /// <summary>
        /// 最小自然
        /// </summary>
        [StatDescription(Title = "自然系魔法", Format = "{0}-0", Mode = StatType.SpellPower, MinStat = MinMC, MaxStat = MaxMC)]
        [Description("MinMC 最小自然 10")]
        MinMC,
        /// <summary>
        /// 最大自然
        /// </summary>
        [StatDescription(Title = "自然系魔法", Format = "{0}-{1}", Mode = StatType.SpellPower, MinStat = MinMC, MaxStat = MaxMC)]
        [Description("MaxMC 最大自然 11")]
        MaxMC,
        /// <summary>
        /// 最小灵魂
        /// </summary>
        [StatDescription(Title = "灵魂系魔法", Format = "{0}-0", Mode = StatType.SpellPower, MinStat = MinSC, MaxStat = MaxSC)]
        [Description("MinSC 最小灵魂 12")]
        MinSC,
        /// <summary>
        /// 最大灵魂
        /// </summary>
        [StatDescription(Title = "灵魂系魔法", Format = "{0}-{1}", Mode = StatType.SpellPower, MinStat = MinSC, MaxStat = MaxSC)]
        [Description("MaxSC 最大灵魂 13")]
        MaxSC,
        /// <summary>
        /// 准确
        /// </summary>
        [StatDescription(Title = "准确", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Accuracy 准确 14")]
        Accuracy,
        /// <summary>
        /// 敏捷
        /// </summary>
        [StatDescription(Title = "敏捷", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Agility 敏捷 15")]
        Agility,
        /// <summary>
        /// 攻击速度
        /// </summary>
        [StatDescription(Title = "攻击速度", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackSpeed)]
        [Description("AttackSpeed 攻击速度 16")]
        AttackSpeed,
        /// <summary>
        /// 光照范围
        /// </summary>
        [StatDescription(Title = "光照范围", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Light 光照范围 17")]
        Light,
        /// <summary>
        /// 强度
        /// </summary>
        [StatDescription(Title = "强度", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Strength 强度 18")]
        Strength,
        /// <summary>
        /// 幸运
        /// </summary>
        [StatDescription(Title = "幸运", Format = "{0:+#0;-#0;#0}", Mode = StatType.Luck)]
        [Description("Luck 幸运 19")]
        Luck,
        /// <summary>
        /// 火攻
        /// </summary>
        [StatDescription(Title = "火（火焰）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("FireAttack 火攻 20")]
        FireAttack,
        /// <summary>
        /// 火防
        /// </summary>
        [StatDescription(Title = "火（火焰）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("FireResistance 火防 21")]
        FireResistance,
        /// <summary>
        /// 冰攻
        /// </summary>
        [StatDescription(Title = "冰（冰寒）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("IceAttack 冰攻 22")]
        IceAttack,
        /// <summary>
        /// 冰防
        /// </summary>
        [StatDescription(Title = "冰（冰寒）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("IceResistance 冰防 23")]
        IceResistance,
        /// <summary>
        /// 雷攻
        /// </summary>
        [StatDescription(Title = "雷（电击）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("LightningAttack 雷攻 24")]
        LightningAttack,
        /// <summary>
        /// 雷防
        /// </summary>
        [StatDescription(Title = "雷（电击）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("LightningResistance 雷防 25")]
        LightningResistance,
        /// <summary>
        /// 风攻
        /// </summary>
        [StatDescription(Title = "风（风）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("WindAttack 风攻 26")]
        WindAttack,
        /// <summary>
        /// 风防
        /// </summary>
        [StatDescription(Title = "风（风）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("WindResistance 风防 27")]
        WindResistance,
        /// <summary>
        /// 神圣攻
        /// </summary>
        [StatDescription(Title = "神圣（神圣）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("HolyAttack 神圣攻 28")]
        HolyAttack,
        /// <summary>
        /// 神圣防
        /// </summary>
        [StatDescription(Title = "神圣（神圣）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("HolyResistance 神圣防 29")]
        HolyResistance,
        /// <summary>
        /// 暗黑攻
        /// </summary>
        [StatDescription(Title = "暗黑（暗黑）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("DarkAttack 暗黑攻 30")]
        DarkAttack,
        /// <summary>
        /// 暗黑防
        /// </summary>
        [StatDescription(Title = "暗黑（暗黑）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("DarkResistance 暗黑防 31")]
        DarkResistance,
        /// <summary>
        /// 幻影攻
        /// </summary>
        [StatDescription(Title = "幻影（幻影）", Format = "{0:+#0;-#0;#0}", Mode = StatType.AttackElement)]
        [Description("PhantomAttack 幻影攻 32")]
        PhantomAttack,
        /// <summary>
        /// 幻影防
        /// </summary>
        [StatDescription(Title = "幻影（幻影）", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("PhantomResistance 幻影防 33")]
        PhantomResistance,
        /// <summary>
        /// 舒适 回复时间设定
        /// </summary>
        [StatDescription(Title = "舒适", Format = "{0:+#0;-#0;#0}", Mode = StatType.Comfort)]
        [Description("Comfort 舒适 34")]
        Comfort,
        /// <summary>
        /// 吸血几率%
        /// </summary>
        [StatDescription(Title = "吸血几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("LifeSteal 吸血几率% 35")]
        LifeSteal,
        /// <summary>
        /// 经验加成%
        /// </summary>
        [StatDescription(Title = "经验", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ExperienceRate 经验加成% 36")]
        ExperienceRate,
        /// <summary>
        /// 爆率加成%
        /// </summary>
        [StatDescription(Title = "爆率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DropRate 爆率加成% 37")]
        DropRate,
        /// <summary>
        /// 空置状态
        /// </summary>
        [StatDescription(Title = "空置状态", Mode = StatType.None)]
        [Description("None 空置 38")]
        None,
        /// <summary>
        /// 技能熟练度
        /// </summary>
        [StatDescription(Title = "技能熟练度", Format = "x{0}", Mode = StatType.Default)]
        [Description("SkillRate 技能熟练度 39")]
        SkillRate,
        /// <summary>
        /// 捡取范围
        /// </summary>
        [StatDescription(Title = "拾取范围", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("PickUpRadius 拾取范围 40")]
        PickUpRadius,
        /// <summary>
        /// 生命恢复
        /// </summary>
        [StatDescription(Title = "生命恢复", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Healing 生命恢复(怪物每秒回血值) 41")]
        Healing,
        /// <summary>
        /// 最大生命恢复
        /// </summary>
        [StatDescription(Title = "最大生命恢复值", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("HealingCap 最大生命恢复值(怪物回血总血量) 42")]
        HealingCap,
        /// <summary>
        /// 隐身
        /// </summary>
        [StatDescription(Title = "隐身", Mode = StatType.Text)]
        [Description("Invisibility 隐身 技能引用不作为道具属性 43")]
        Invisibility,
        /// <summary>
        /// 强火
        /// </summary>
        [StatDescription(Title = "强元素: 火", Mode = StatType.Text)]
        [Description("FireAffinity 强火 可设置为怪物攻击属性 44")]
        FireAffinity,
        /// <summary>
        /// 强冰
        /// </summary>
        [StatDescription(Title = "强元素: 冰", Mode = StatType.Text)]
        [Description("IceAffinity 强冰 可设置为怪物攻击属性 45")]
        IceAffinity,
        /// <summary>
        /// 强雷
        /// </summary>
        [StatDescription(Title = "强元素: 雷", Mode = StatType.Text)]
        [Description("LightningAffinity 强雷 可设置为怪物攻击属性 46")]
        LightningAffinity,
        /// <summary>
        /// 强风
        /// </summary>
        [StatDescription(Title = "强元素: 风", Mode = StatType.Text)]
        [Description("WindAffinity 强风 可设置为怪物攻击属性 47")]
        WindAffinity,
        /// <summary>
        /// 强神圣
        /// </summary>
        [StatDescription(Title = "强元素: 神圣", Mode = StatType.Text)]
        [Description("HolyAffinity 强神圣 可设置为怪物攻击属性 48")]
        HolyAffinity,
        /// <summary>
        /// 强暗黑
        /// </summary>
        [StatDescription(Title = "强元素: 暗黑", Mode = StatType.Text)]
        [Description("DarkAffinity 强暗黑 可设置为怪物攻击属性 49")]
        DarkAffinity,
        /// <summary>
        /// 强幻影
        /// </summary>
        [StatDescription(Title = "强元素: 幻影", Mode = StatType.Text)]
        [Description("PhantomAffinity 强幻影 可设置为怪物攻击属性 50")]
        PhantomAffinity,
        /// <summary>
        /// 反弹伤害%
        /// </summary>
        [StatDescription(Title = "反弹伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ReflectDamage 反弹伤害% 51")]
        ReflectDamage,
        /// <summary>
        /// 武器元素
        /// </summary>
        [StatDescription(Mode = StatType.None)]
        [Description("WeaponElement 武器元素 不作为道具属性 52")]
        WeaponElement,
        /// <summary>
        /// 救赎
        /// </summary>
        [StatDescription(Title = "救赎", Mode = StatType.Text)]
        [Description("Redemption 救赎 技能引用不作为道具属性 53")]
        Redemption,
        /// <summary>
        /// 生命值%
        /// </summary>
        [StatDescription(Title = "体力增加", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("HealthPercent 生命值% 54")]
        HealthPercent,
        /// <summary>
        /// 暴击几率%
        /// </summary>
        [StatDescription(Title = "暴击几率", Format = "{0:+#0;-#0;#0}%", Mode = StatType.Default)]
        [Description("CriticalChance 暴击几率% 55")]
        CriticalChance,
        /// <summary>
        /// 5%收益
        /// </summary>
        [StatDescription(Title = "5% 收益增加", Format = "{0} 或更多", Mode = StatType.Default)]
        [Description("SaleBonus5 叠加材料类道具增加收益5% 56")]
        SaleBonus5,
        /// <summary>
        /// 10%收益
        /// </summary>
        [StatDescription(Title = "10% 收益增加", Format = "{0} 或更多", Mode = StatType.Default)]
        [Description("SaleBonus10 叠加材料类道具增加收益10% 57")]
        SaleBonus10,
        /// <summary>
        /// 15%收益
        /// </summary>
        [StatDescription(Title = "15% 收益增加", Format = "{0} 或更多", Mode = StatType.Default)]
        [Description("SaleBonus15 叠加材料类道具增加收益15% 58")]
        SaleBonus15,
        /// <summary>
        /// 20%收益
        /// </summary>
        [StatDescription(Title = "20% 收益增加", Format = "{0} 或更多", Mode = StatType.Default)]
        [Description("SaleBonus20 叠加材料类道具增加收益20% 59")]
        SaleBonus20,
        /// <summary>
        /// 魔法盾
        /// </summary>
        [StatDescription(Title = "魔法盾", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MagicShield 魔法盾 技能引用不作为道具属性 60")]
        MagicShield,
        /// <summary>
        /// 隐身
        /// </summary>
        [StatDescription(Title = "隐身", Mode = StatType.Text)]
        [Description("Cloak 隐身 技能引用不作为道具属性 61")]
        Cloak,
        /// <summary>
        /// 潜行
        /// </summary>
        [StatDescription(Title = "潜行", Format = "{0} 持续", Mode = StatType.Default)]
        [Description("CloakDamage 潜行 技能引用不作为道具属性 62")]
        CloakDamage,
        /// <summary>
        /// 心机一转
        /// </summary>
        [StatDescription(Title = "心机一转", Format = "{0}", Mode = StatType.Default)]
        [Description("TheNewBeginning 心机一转 技能引用不作为道具属性 63")]
        TheNewBeginning,
        /// <summary>
        /// 灰名
        /// </summary>
        [StatDescription(Title = "灰名, 其他人可以合法攻击你", Mode = StatType.Text)]
        [Description("Brown 灰名 不作为道具属性 64")]
        Brown,
        /// <summary>
        /// PK值
        /// </summary>
        [StatDescription(Title = "PK值", Format = "{0}", Mode = StatType.Default)]
        [Description("PKPoint PK值 不作为道具属性 65")]
        PKPoint,
        /// <summary>
        /// 全服喊话无等级限制
        /// </summary>
        [StatDescription(Title = "全服喊话无等级限制", Mode = StatType.Text)]
        [Description("GlobalShout 全服喊话无等级限制 不作为道具属性 66")]
        GlobalShout,
        /// <summary>
        /// 自然系魔法%
        /// </summary>
        [StatDescription(Title = "自然系魔法", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MCPercent 自然系魔法% 67")]
        MCPercent,
        /// <summary>
        /// 天打雷劈几率%
        /// </summary>
        [StatDescription(Title = "天打雷劈几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("JudgementOfHeaven 天打雷劈几率% 68")]
        JudgementOfHeaven,
        /// <summary>
        /// 妙影无踪
        /// </summary>
        [StatDescription(Title = "妙影无踪", Mode = StatType.Text)]
        [Description("Transparency 妙影无踪 技能引用不作为道具属性 69")]
        Transparency,
        /// <summary>
        /// 阴阳法环
        /// </summary>
        [StatDescription(Title = "阴阳法环", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("CelestialLight 阴阳法环 技能引用不作为道具属性 70")]
        CelestialLight,
        /// <summary>
        /// 黄泉旅者
        /// </summary>
        [StatDescription(Title = "黄泉旅者", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("DarkConversion 黄泉旅者 技能引用不作为道具属性 71")]
        DarkConversion,
        /// <summary>
        /// HP恢复
        /// </summary>
        [StatDescription(Title = "HP恢复", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("RenounceHPLost HP恢复 72")]
        RenounceHPLost,
        /// <summary>
        /// 背包重量
        /// </summary>
        [StatDescription(Title = "背包重量", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("BagWeight 背包重量 73")]
        BagWeight,
        /// <summary>
        /// 负重能力
        /// </summary>
        [StatDescription(Title = "负重能力", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("WearWeight 负重能力 74")]
        WearWeight,
        /// <summary>
        /// 手负重能力
        /// </summary>
        [StatDescription(Title = "手负重能力", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("HandWeight 手负重能力 75")]
        HandWeight,
        /// <summary>
        /// 金币爆率
        /// </summary>
        [StatDescription(Title = "金币爆率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("GoldRate 金币爆率% 76")]
        GoldRate,
        /// <summary>
        /// 剩余时间
        /// </summary>
        [StatDescription(Title = "剩余时间", Mode = StatType.Time)]
        [Description("OldDuration 剩余时间(秒) 77")]
        OldDuration,
        /// <summary>
        /// 获得赏金
        /// </summary>
        [StatDescription(Title = "获得赏金", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("AvailableHuntGold 获得赏金 不属于道具属性 78")]
        AvailableHuntGold,
        /// <summary>
        /// 获得赏金最大值
        /// </summary>
        [StatDescription(Title = "获得赏金最大值", Format = "{0:#0}", Mode = StatType.Default)]
        [Description("AvailableHuntGoldCap 获得赏金最大值 不属于道具属性 79")]
        AvailableHuntGoldCap,
        /// <summary>
        /// 复活冷却
        /// </summary>
        [StatDescription(Title = "复活冷却", Mode = StatType.Time)]
        [Description("ItemReviveTime 复活冷却时间 不属于道具属性 80")]
        ItemReviveTime,
        /// <summary>
        /// 提高精炼成功几率%
        /// </summary>
        [StatDescription(Title = "提高精炼成功几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxRefineChance 提高精炼成功几率% 81")]
        MaxRefineChance,
        /// <summary>
        /// 宠物背包空间
        /// </summary>
        [StatDescription(Title = "宠物背包空间", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("CompanionInventory 宠物背包空间 82")]
        CompanionInventory,
        /// <summary>
        /// 宠物背包负重
        /// </summary>
        [StatDescription(Title = "宠物背包负重", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("CompanionBagWeight 宠物背包负重 83")]
        CompanionBagWeight,
        /// <summary>
        /// 破坏%
        /// </summary>
        [StatDescription(Title = "物理攻击提升", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DCPercent 破坏% 84")]
        DCPercent,
        /// <summary>
        /// 灵魂系魔法%
        /// </summary>
        [StatDescription(Title = "灵魂系魔法提升", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SCPercent 灵魂系魔法% 85")]
        SCPercent,
        /// <summary>
        /// 宠物饥饿度
        /// </summary>
        [StatDescription(Title = "宠物饥饿度", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("CompanionHunger 宠物饥饿度 不属于道具属性 86")]
        CompanionHunger,
        /// <summary>
        /// 宠物破坏值%
        /// </summary>
        [StatDescription(Title = "宠物攻击加成", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("PetDCPercent 宠物破坏值加成% （移花接玉属性） 87")]
        PetDCPercent,
        /// <summary>
        /// 在地图上定位领主
        /// </summary>
        [StatDescription(Title = "在地图上定位领主", Mode = StatType.Text)]
        [Description("BossTracker 在地图上定位领主 88")]
        BossTracker,
        /// <summary>
        /// 在地图上定位玩家
        /// </summary>
        [StatDescription(Title = "在地图上定位玩家", Mode = StatType.Text)]
        [Description("PlayerTracker 在地图上定位玩家 89")]
        PlayerTracker,
        /// <summary>
        /// 宠物经验
        /// </summary>
        [StatDescription(Title = "宠物经验", Format = "x{0}", Mode = StatType.Default)]
        [Description("CompanionRate 宠物经验 不属于道具属性 90")]
        CompanionRate,
        /// <summary>
        /// 负重
        /// </summary>
        [StatDescription(Title = "负重", Format = "x{0}", Mode = StatType.Default)]
        [Description("WeightRate 负重 不属于道具属性 91")]
        WeightRate,
        /// <summary>
        /// 最大物理防御和最大魔法防御已经提高
        /// </summary>
        [StatDescription(Title = "最大物理防御和最大魔法防御已经提高", Mode = StatType.Text)]
        [Description("Defiance 最大物防魔防增加 不属于道具属性 92")]
        Defiance,
        /// <summary>
        /// 减少你的防御来提高破坏力
        /// </summary>
        [StatDescription(Title = "减少你的防御来提高破坏力", Mode = StatType.Text)]
        [Description("Might 减少防御提高破坏力 不属于道具属性 93")]
        Might,
        /// <summary>
        /// 魔法值%
        /// </summary>
        [StatDescription(Title = "魔法值增加", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ManaPercent 魔法值% 94")]
        ManaPercent,
        /// <summary>
        /// 传送命令: @天地合一
        /// </summary>
        [StatDescription(Title = "传送命令: @天地合一", Mode = StatType.Text)]
        [Description("RecallSet 天地合一命令 不属于道具属性 95")]
        RecallSet,
        /// <summary>
        /// 怪物基础经验%
        /// </summary>
        [StatDescription(Title = "怪物基础经验", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MonsterExperience 怪物基础经验% 96")]
        MonsterExperience,
        /// <summary>
        /// 怪物基础金币%
        /// </summary>
        [StatDescription(Title = "怪物基础金币", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MonsterGold 怪物基础金币% 97")]
        MonsterGold,
        /// <summary>
        /// 怪物基础爆率%
        /// </summary>
        [StatDescription(Title = "怪物基础爆率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MonsterDrop 怪物基础爆率% 98")]
        MonsterDrop,
        /// <summary>
        /// 怪物基础伤害%
        /// </summary>
        [StatDescription(Title = "怪物基础伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MonsterDamage 怪物基础伤害% 99")]
        MonsterDamage,
        /// <summary>
        /// 怪物基础生命%
        /// </summary>
        [StatDescription(Title = "怪物基础生命", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MonsterHealth 怪物基础生命% 100")]
        MonsterHealth,
        [StatDescription(Mode = StatType.None)]
        [Description("ItemIndex 道具序号索引 不属于道具属性 101")]
        ItemIndex,
        /// <summary>
        /// 提高宠物拾取速度功效
        /// </summary>
        [StatDescription(Title = "提高宠物拾取速度", Mode = StatType.Text)]
        [Description("CompanionCollection 提高宠物拾取速度功效 102")]
        CompanionCollection,
        /// <summary>
        /// 护身功效
        /// </summary>
        [StatDescription(Title = "护身", Mode = StatType.None)]
        [Description("ProtectionRing 护身功效 隐藏属性 103")]
        ProtectionRing,
        /// <summary>
        /// 隐身功效
        /// </summary>
        [StatDescription(Mode = StatType.None)]
        [Description("ClearRing 隐身功效 隐藏属性 104")]
        ClearRing,
        /// <summary>
        /// 传送功效
        /// </summary>
        [StatDescription(Mode = StatType.None)]
        [Description("TeleportRing 传送功效 隐藏属性 105")]
        TeleportRing,
        /// <summary>
        /// 基础经验加成%
        /// </summary>
        [StatDescription(Title = "基础经验加成", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BaseExperienceRate 基础经验加成% 106")]
        BaseExperienceRate,
        /// <summary>
        /// 基础金币加成%
        /// </summary>
        [StatDescription(Title = "基础金币加成", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BaseGoldRate 基础金币加成% 107")]
        BaseGoldRate,
        /// <summary>
        /// 基础爆率加成%
        /// </summary>
        [StatDescription(Title = "基础爆率加成", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BaseDropRate 基础爆率加成% 108")]
        BaseDropRate,
        /// <summary>
        /// 最小冰冻伤害
        /// </summary>
        [StatDescription(Title = "冰冻伤害", Format = "{0}", Mode = StatType.Default)]
        [Description("FrostBiteDamage 最小冰冻伤害 109")]
        FrostBiteDamage,
        /// <summary>
        /// 怪物最高经验%
        /// </summary>
        [StatDescription(Title = "怪物最高经验", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxMonsterExperience 怪物最高经验% 110")]
        MaxMonsterExperience,
        /// <summary>
        /// 怪物最高金币%
        /// </summary>
        [StatDescription(Title = "怪物最高金币", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxMonsterGold 怪物最高金币% 111")]
        MaxMonsterGold,
        /// <summary>
        /// 怪物最高爆率%
        /// </summary>
        [StatDescription(Title = "怪物最高爆率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxMonsterDrop 怪物最高爆率% 112")]
        MaxMonsterDrop,
        /// <summary>
        /// 怪物最高伤害%
        /// </summary>
        [StatDescription(Title = "怪物最高伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxMonsterDamage 怪物最高伤害% 113")]
        MaxMonsterDamage,
        /// <summary>
        /// 怪物最高生命%
        /// </summary>
        [StatDescription(Title = "怪物最高生命", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MaxMonsterHealth 怪物最高生命% 114")]
        MaxMonsterHealth,
        /// <summary>
        /// 暴击伤害%
        /// </summary>
        [StatDescription(Title = "暴击伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("CriticalDamage 暴击伤害% 115")]
        CriticalDamage,
        /// <summary>
        /// 经验
        /// </summary>
        [StatDescription(Title = "经验", Format = "{0}", Mode = StatType.Default)]
        [Description("Experience 道具经验 不属于道具属性 116")]
        Experience,
        /// <summary>
        /// 激活死亡掉落道具
        /// </summary>
        [StatDescription(Title = "激活死亡掉落道具", Mode = StatType.Text)]
        [Description("DeathDrops 该道具属性激活死亡掉落道具 117")]
        DeathDrops,

        /// <summary>
        /// 体质
        /// </summary>
        [StatDescription(Title = "体质", Format = "{0:+#0;-#0;#0}", Mode = StatType.ElementResistance)]
        [Description("PhysicalResistance 体质 118")]
        PhysicalResistance,

        /// <summary>
        /// 碎片分解成功几率%
        /// </summary>
        [StatDescription(Title = "碎片分解成功几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FragmentRate 碎片分解成功几率% 119")]
        FragmentRate,
        /// <summary>
        /// 获得召唤随机领主的机会
        /// </summary>
        [StatDescription(Title = "获得召唤随机领主的机会", Mode = StatType.Text)]
        [Description("MapSummoning 召唤BOSS 120")]
        MapSummoning,
        /// <summary>
        /// 最大冰冻伤害
        /// </summary>
        [StatDescription(Title = "最大冰冻伤害", Format = "{0}", Mode = StatType.Default)]
        [Description("FrostBiteMaxDamage 最大冰冻伤害 121")]
        FrostBiteMaxDamage,
        /// <summary>
        /// 麻痹几率%
        /// </summary>
        [StatDescription(Title = "麻痹几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ParalysisChance 麻痹几率% 122")]
        ParalysisChance,
        /// <summary>
        /// 减速几率%
        /// </summary>
        [StatDescription(Title = "减速几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SlowChance 减速几率% 123")]
        SlowChance,
        /// <summary>
        /// 沉默几率%
        /// </summary>
        [StatDescription(Title = "沉默几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SilenceChance 沉默几率% 124")]
        SilenceChance,
        /// <summary>
        /// 格挡几率%
        /// </summary>
        [StatDescription(Title = "格挡几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BlockChance 格挡几率% 125")]
        BlockChance,
        /// <summary>
        /// 闪避几率%
        /// </summary>
        [StatDescription(Title = "闪避几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("EvasionChance 闪避几率% 126")]
        EvasionChance,
        /// <summary>
        /// 忽视隐身藏匿状态
        /// </summary>
        [StatDescription(Mode = StatType.None)]
        [Description("IgnoreStealth 忽视隐身藏匿状态 127")]
        IgnoreStealth,
        [StatDescription(Mode = StatType.None)]
        [Description("FootballArmourAction 口哨道具专用定义 128")]
        FootballArmourAction,
        /// <summary>
        /// 毒性抵抗%
        /// </summary>
        [StatDescription(Title = "毒性抵抗", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("PoisonResistance 毒性抵抗% 129")]
        PoisonResistance,
        /// <summary>
        /// 转生
        /// </summary>
        [StatDescription(Title = "转生", Format = "{0}", Mode = StatType.Default)]
        [Description("Rebirth 转生道具判断属性 130")]
        Rebirth,
        /// <summary>
        /// 请求联盟公会
        /// </summary>
        [StatDescription(Title = "请求联盟公会: ", Mode = StatType.Text)]
        [Description("Guild1 请求联盟的公会 不属于道具属性 131")]
        Guild1,
        /// <summary>
        /// 接受联盟公会
        /// </summary>
        [StatDescription(Title = "接受联盟公会: ", Mode = StatType.Text)]
        [Description("Guild2 接受联盟的公会 不属于道具属性 132")]
        Guild2,
        /// <summary>
        /// 魔法躲避%
        /// </summary>
        [StatDescription(Title = "魔法躲避", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MagicEvade 魔法躲避% 133")]
        MagicEvade,
        /// <summary>
        /// 额外伤害增加
        /// </summary>
        [StatDescription(Title = "额外伤害", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("ExtraDamage 额外伤害增加 134")]
        ExtraDamage,
        /// <summary>
        /// 会心一击%
        /// </summary>
        [StatDescription(Title = "会心一击", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("CriticalHit 会心一击% 135")]
        CriticalHit,
        /// <summary>
        /// 附加技能绿毒%
        /// </summary>
        [StatDescription(Title = "附加技能:绿毒", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.None)]
        [Description("GreenPoison 附加技能绿毒% 隐藏属性 136")]
        GreenPoison,
        /// <summary>
        /// 附加技能红毒%
        /// </summary>
        [StatDescription(Title = "附加技能:红毒", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.None)]
        [Description("RedPoison 附加技能红毒% 隐藏属性 137")]
        RedPoison,
        /// <summary>
        /// 等级倍数转HPMP
        /// </summary>
        [StatDescription(Title = "等级倍数转HPMP", Format = "x{0}", Mode = StatType.None)]
        [Description("LvConvertHPMP 等级倍数转HPMP 隐藏属性 138")]
        LvConvertHPMP,
        /// <summary>
        /// 最小黑炎(月河)防御
        /// </summary>
        [StatDescription(Title = "黑炎防御", Format = "{0}-0", Mode = StatType.Min, MinStat = MinBAC, MaxStat = MaxBAC)]
        [Description("MinBAC 最小黑炎(月河)防御 139")]
        MinBAC,
        /// <summary>
        /// 最大黑炎(月河)防御
        /// </summary>
        [StatDescription(Title = "黑炎防御", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinBAC, MaxStat = MaxBAC)]
        [Description("MaxBAC 最大黑炎(月河)防御 140")]
        MaxBAC,
        /// <summary>
        /// 最小黑炎(月河)攻击
        /// </summary>
        [StatDescription(Title = "黑炎攻击", Format = "{0}-0", Mode = StatType.Min, MinStat = MinBC, MaxStat = MaxBC)]
        [Description("MinBC 最小黑炎(月河)攻击 141")]
        MinBC,
        /// <summary>
        /// 最大黑炎(月河)攻击
        /// </summary>
        [StatDescription(Title = "黑炎攻击", Format = "{0}-{1}", Mode = StatType.Max, MinStat = MinBC, MaxStat = MaxBC)]
        [Description("MaxBC 最大黑炎(月河)攻击 142")]
        MaxBC,
        /// <summary>
        /// 地图属性角色每秒生命值+ -
        /// </summary>
        [StatDescription(Title = "黑炎引力", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("BHealth 地图属性角色每秒生命值+ - 143")]
        BHealth,
        /// <summary>
        /// 物理防御提升%
        /// </summary>
        [StatDescription(Title = "物理防御提升", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ACPercent 物理防御提升% 144")]
        ACPercent,
        /// <summary>
        /// 魔法防御提升%
        /// </summary>
        [StatDescription(Title = "魔法防御提升", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MRPercent 魔法防御提升% 145")]
        MRPercent,
        /// <summary>
        /// 任务道具爆率加成%
        /// </summary>
        [StatDescription(Title = "任务成功率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("QuestDropRate 任务道具爆率加成% 146")]
        QuestDropRate,
        /// <summary>
        /// 死系怪物追加伤害增加
        /// </summary>
        [StatDescription(Title = "死系怪物伤害", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("DieExtraDamage 死系怪物追加伤害增加 147")]
        DieExtraDamage,
        /// <summary>
        /// 生系怪物追加伤害增加
        /// </summary>
        [StatDescription(Title = "生系怪物伤害", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("LifeExtraDamage 生系怪物追加伤害增加 148")]
        LifeExtraDamage,
        /// <summary>
        /// 最终伤害减少
        /// </summary>
        [StatDescription(Title = "最终伤害减少", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("FinalDamageReduce 最终伤害减少 149")]
        FinalDamageReduce,
        /// <summary>
        /// 最终伤害减少%
        /// </summary>
        [StatDescription(Title = "最终伤害减少", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FinalDamageReduceRate 最终伤害减少% 150")]
        FinalDamageReduceRate,
        /// <summary>
        /// 无敌状态
        /// </summary>
        [StatDescription(Title = "你不会受到任何伤害", Mode = StatType.Text)]
        [Description("Invincibility 无敌状态 技能引用不作为道具属性 151")]
        Invincibility,
        /// <summary>
        /// 护身法盾
        /// </summary>
        [StatDescription(Title = "护身法盾", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("SuperiorMagicShield 护身法盾 技能引用不作为道具属性 152")]
        SuperiorMagicShield,
        /// <summary>
        /// 采矿成功几率
        /// </summary>
        [StatDescription(Title = "采矿成功几率", Format = "{0:#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MiningSuccessRate 采矿成功几率% 153")]
        MiningSuccessRate,
        /// <summary>
        /// %体力恢复速度
        /// </summary>
        [StatDescription(Title = "体力恢复速度", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("HPRegenRate 体力恢复速度% 154")]
        HPRegenRate,
        /// <summary>
        /// %魔力恢复速度
        /// </summary>
        [StatDescription(Title = "魔力恢复速度", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MPRegenRate 魔力恢复速度% 155")]
        MPRegenRate,
        /// <summary>
        /// %无视物防
        /// </summary>
        [StatDescription(Title = "无视物防%", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ACIgnoreRate 无视物防% 156")]
        ACIgnoreRate,
        /// <summary>
        /// %无视魔防
        /// </summary>
        [StatDescription(Title = "无视魔防%", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MRIgnoreRate 无视魔防% 157")]
        MRIgnoreRate,

        /// <summary>
        /// 抗暴击
        /// </summary>      
        [StatDescription(Title = "抗暴击", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("CriticalChanceResistance 抗暴击值(人物) 158")]
        CriticalChanceResistance,
        /// <summary>
        /// %抗麻痹
        /// </summary>
        [StatDescription(Title = "抗麻痹", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ParalysisChanceResistance 抗麻痹%(人物) 159")]
        ParalysisChanceResistance,
        /// <summary>
        /// %抗沉默
        /// </summary>
        [StatDescription(Title = "抗沉默", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SilenceChanceResistance 抗沉默%(人物) 160")]
        SilenceChanceResistance,
        /// <summary>
        /// %抗格挡
        /// </summary>
        [StatDescription(Title = "抗格挡", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BlockChanceResistance 抗格挡%(人物) 161")]
        BlockChanceResistance,
        /// <summary>
        /// 深渊几率
        /// </summary>
        [StatDescription(Title = "深渊几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AbyssChance 深渊几率% 162")]
        AbyssChance,
        /// <summary>
        /// %抗深渊
        /// </summary>
        [StatDescription(Title = "抗深渊", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AbyssChanceResistance 抗深渊%(人物) 163")]
        AbyssChanceResistance,
        /// <summary>
        /// %防圣言
        /// </summary>
        [StatDescription(Title = "防圣言", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ExpelUndeadResistance 防圣言% 164")]
        ExpelUndeadResistance,
        /// <summary>
        /// 无敌几率
        /// </summary>
        [StatDescription(Title = "无敌几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("InvincibilityChance 无敌几率% 165")]
        InvincibilityChance,
        /// <summary>
        /// 暴毒几率
        /// </summary>
        [StatDescription(Title = "暴毒", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("GreenPosionPro 暴毒几率% 166")]
        GreenPosionPro,
        /// <summary>
        /// 出笼几率
        /// </summary>
        [StatDescription(Title = "出笼", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("PetACPower 出笼几率% 167")]
        PetACPower,
        /// <summary>
        /// 运气几率
        /// </summary>
        [StatDescription(Title = "运气", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("PotionLuck 运气几率% 168")]
        PotionLuck,
        /// <summary>
        /// 狂飙几率
        /// </summary>
        [StatDescription(Title = "狂飙", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AttackSpeedAdd 狂飙几率% 169")]
        AttackSpeedAdd,
        /// <summary>
        /// 暴锤几率
        /// </summary>
        [StatDescription(Title = "暴锤", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DamageAdd 暴锤几率% 170")]
        DamageAdd,
        /// <summary>
        /// 反弹几率%
        /// </summary>
        [StatDescription(Title = "反弹几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ReflectChance 反弹几率% 171")]
        ReflectChance,
        /// <summary>
        /// 宠物防御值%
        /// </summary>
        [StatDescription(Title = "宠物防魔加成", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("PetMCPercent 宠物防魔值加成% 172")]
        PetMCPercent,
        /// <summary>
        /// 经验加成%
        /// </summary>
        [StatDescription(Title = "经验", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("TeamExperienceRate 经验加成% (组队不加成) 173")]
        TeamExperienceRate,
        /// <summary>
        /// 最终伤害增加%
        /// </summary>
        [StatDescription(Title = "最终伤害增加", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FinalDamageAddRate 最终伤害增加% 174")]
        FinalDamageAddRate,
        /// <summary>
        /// 晕击几率%
        /// </summary>
        [StatDescription(Title = "晕击几率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("StunnedStrikeChance 晕击几率% 175")]
        StunnedStrikeChance,
        /// <summary>
        /// %抗晕击
        /// </summary>
        [StatDescription(Title = "抗晕击", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("StunnedStrikeChanceResistance 抗晕击%(人物) 176")]
        StunnedStrikeChanceResistance,
        /// <summary>
        /// 抗拒
        /// </summary>      
        [StatDescription(Title = "抗拒", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("Repulsion 抗拒 177")]
        Repulsion,
        /// <summary>
        /// 抽蓝%
        /// </summary>
        [StatDescription(Title = "抽蓝", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SmokingMP 抽蓝%(人物) 178")]
        SmokingMP,
        /// <summary>
        /// 护血%
        /// </summary>
        [StatDescription(Title = "护血", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ProtectBlood 护血%(人物) 蓝转成血量 179")]
        ProtectBlood,
        /// <summary>
        /// 复血%
        /// </summary>
        [StatDescription(Title = "复血", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("Rehydrations 复血%(道士) 盾破复活血量加成 180")]
        Rehydrations,
        /// <summary>
        /// 增效%
        /// </summary>
        [StatDescription(Title = "增效", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("Synerg 增效%(人物) 喝药效果提升 181")]
        Synerg,
        /// <summary>
        /// 神速%
        /// </summary>
        [StatDescription(Title = "神速", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("Marvellously 神速%(人物) 喝药CD减少 182")]
        Marvellously,
        /// <summary>
        /// 持续时间
        /// </summary>
        [StatDescription(Title = "持续时间", Mode = StatType.Time)]
        [Description("Duration 消耗品BUFF持续时间(不能用于装备限时设置) 10000")]
        Duration = 10000,

        //镶嵌道具独有的属性
        /// <summary>
        /// 镶嵌成功几率%
        /// </summary>
        [StatDescription(Title = "镶嵌成功几率", Format = "{0:#0%;-#0%;#0%}", Mode = StatType.None)]
        [Description("GemOrbSuccess 镶嵌成功几率% 隐藏属性 10001")]
        GemOrbSuccess,
        /// <summary>
        /// 装备破碎几率%
        /// </summary>
        [StatDescription(Title = "装备破碎几率", Format = "{0:#0%;-#0%;#0%}", Mode = StatType.None)]
        [Description("GemOrbBrake 装备破碎几率% 隐藏属性 10002")]
        GemOrbBrake,
        /// <summary>
        /// 镶嵌宝石
        /// </summary>
        [StatDescription(Title = "镶嵌宝石", Format = "{0:+#0;-#0;#0}", Mode = StatType.Default)]
        [Description("UsedGemSlot 镶嵌宝石 10003")]
        UsedGemSlot,
        /// <summary>
        /// 空闲宝石插槽
        /// </summary>
        [StatDescription(Title = "空闲宝石插槽", Format = "{0}", Mode = StatType.Default)]
        [Description("EmptyGemSlot 空闲宝石插槽 10004")]
        EmptyGemSlot,
        //钓鱼道具独有属性
        /// <summary>
        /// 寻鱼成功率
        /// </summary>
        [StatDescription(Title = "钓鱼成功率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FishFinding 钓鱼成功率% 10005")]
        FishFindingChance,
        /// <summary>
        /// 寻鱼成功率递增
        /// </summary>
        [StatDescription(Title = "寻鱼成功率递增", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FishFindingFailedAdd 寻鱼失败时成功率递增% 10006")]
        FishFindingFailedAdd,
        /// <summary>
        /// 咬钩几率
        /// </summary>
        [StatDescription(Title = "咬钩成功率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FishNibbleChance 咬钩钓鱼成功率% 10007")]
        FishNibbleChance,
        /// <summary>
        /// 摇轮自动收杆几率
        /// </summary>
        [StatDescription(Title = "摇轮成功率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AutoReelChance 摇轮自动收杆几率% 10008")]
        AutoReelChance,
        /// <summary>
        /// 钓鱼产出次物品概率
        /// </summary>
        [StatDescription(Title = "钓出该道具概率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.None)]
        [Description("FishObtainChance 钓出该道具概率% 10009")]
        FishObtainChance,
        /// <summary>
        /// 提高钓鱼获得物品概率
        /// </summary>
        [StatDescription(Title = "物品成功率", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FishObtainItemChance 增加钓出道具成功概率% 10010")]
        FishObtainItemChance,

        /// <summary>
        /// 装备幻化
        /// </summary>
        [StatDescription(Title = "幻化", Mode = StatType.Shape)]
        [Description("Illusion 装备幻化 15000")]
        Illusion = 15000,
        /// <summary>
        /// 持续时间
        /// </summary>
        [StatDescription(Title = "幻化限时", Mode = StatType.None)]
        [Description("IllusionDuration 幻化持续时间 15001")]
        IllusionDuration,

        //宝石技能提升属性
        /// <summary>
        /// 技能(烈火剑法)伤害%
        /// </summary>
        [StatDescription(Title = "技能(烈火剑法)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FlamingSwordHoist 技能(烈火剑法)伤害% 20001")]
        FlamingSwordHoist = 20001,
        /// <summary>
        /// 技能(翔空剑法)伤害%
        /// </summary>
        [StatDescription(Title = "技能(翔空剑法)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DragonRiseHoist 技能(翔空剑法)伤害% 20002")]
        DragonRiseHoist,
        /// <summary>
        /// 技能(莲月剑法)伤害%
        /// </summary>
        [StatDescription(Title = "技能(莲月剑法)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BladeStormHoist 技能(莲月剑法)伤害% 20003")]
        BladeStormHoist,
        /// <summary>
        /// 技能(十方斩)伤害%
        /// </summary>
        [StatDescription(Title = "技能(十方斩)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DestructiveSurgeHoist 技能(十方斩)伤害% 20004")]
        DestructiveSurgeHoist,
        /// <summary>
        /// 技能(快刀斩马)伤害%
        /// </summary>
        [StatDescription(Title = "技能(快刀斩马)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SwiftBladeHoist 技能(快刀斩马)伤害% 20005")]
        SwiftBladeHoist,
        /// <summary>
        /// 技能(火球术)伤害%
        /// </summary>
        [StatDescription(Title = "技能(火球术)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FireBallHoist 技能(火球术)伤害% 20006")]
        FireBallHoist,
        /// <summary>
        /// 技能(大火球)伤害%
        /// </summary>
        [StatDescription(Title = "技能(大火球)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AdamantineFireBallHoist 技能(大火球)伤害% 20007")]
        AdamantineFireBallHoist,
        /// <summary>
        /// 技能(地狱火)伤害%
        /// </summary>
        [StatDescription(Title = "技能(地狱火)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ScortchedEarthHoist 技能(地狱火)伤害% 20008")]
        ScortchedEarthHoist,
        /// <summary>
        /// 技能(火墙)伤害%
        /// </summary>
        [StatDescription(Title = "技能(火墙)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FireWallHoist 技能(火墙)伤害% 20009")]
        FireWallHoist,
        /// <summary>
        /// 技能(爆裂火焰)伤害%
        /// </summary>
        [StatDescription(Title = "技能(爆裂火焰)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FireStormHoist 技能(爆裂火焰)伤害% 20010")]
        FireStormHoist,
        /// <summary>
        /// 技能(焰天火雨)伤害%
        /// </summary>
        [StatDescription(Title = "技能(焰天火雨)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("MeteorShowerHoist 技能(焰天火雨)伤害% 20011")]
        MeteorShowerHoist,
        /// <summary>
        /// 技能(冰月神掌)伤害%
        /// </summary>
        [StatDescription(Title = "技能(冰月神掌)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("IceBoltHoist 技能(冰月神掌)伤害% 20012")]
        IceBoltHoist,
        /// <summary>
        /// 技能(冰月震天)伤害%
        /// </summary>
        [StatDescription(Title = "技能(冰月震天)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("IceBladesHoist 技能(冰月震天)伤害% 20013")]
        IceBladesHoist,
        /// <summary>
        /// 技能(冰咆哮)伤害%
        /// </summary>
        [StatDescription(Title = "技能(冰咆哮)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("IceStormHoist 技能(冰咆哮)伤害% 20014")]
        IceStormHoist,
        /// <summary>
        /// 技能(魄冰刺)伤害%
        /// </summary>
        [StatDescription(Title = "技能(魄冰刺)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("GreaterFrozenEarthHoist 技能(魄冰刺)伤害% 20015")]
        GreaterFrozenEarthHoist,
        /// <summary>
        /// 技能(霹雳掌)伤害%
        /// </summary>
        [StatDescription(Title = "技能(霹雳掌)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("LightningBallHoist 技能(霹雳掌)伤害% 20016")]
        LightningBallHoist,
        /// <summary>
        /// 技能(雷电术)伤害%
        /// </summary>
        [StatDescription(Title = "技能(雷电术)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ThunderBoltHoist 技能(雷电术)伤害% 20017")]
        ThunderBoltHoist,
        /// <summary>
        /// 技能(疾光电影)伤害%
        /// </summary>
        [StatDescription(Title = "技能(疾光电影)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("LightningBeamHoist 技能(疾光电影)伤害% 20018")]
        LightningBeamHoist,
        /// <summary>
        /// 技能(地狱雷光)伤害%
        /// </summary>
        [StatDescription(Title = "技能(地狱雷光)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("LightningWaveHoist 技能(地狱雷光)伤害% 20019")]
        LightningWaveHoist,
        /// <summary>
        /// 技能(怒神霹雳)伤害%
        /// </summary>
        [StatDescription(Title = "技能(怒神霹雳)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ChainLightningHoist 技能(怒神霹雳)伤害% 20020")]
        ChainLightningHoist,
        /// <summary>
        /// 技能(电闪雷鸣)伤害%
        /// </summary>
        [StatDescription(Title = "技能(电闪雷鸣)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ThunderStrikeHoist 技能(电闪雷鸣)伤害% 20021")]
        ThunderStrikeHoist,
        /// <summary>
        /// 技能(风掌)伤害%
        /// </summary>
        [StatDescription(Title = "技能(风掌)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("GustBlastHoist 技能(风掌)伤害% 20022")]
        GustBlastHoist,
        /// <summary>
        /// 技能(击风)伤害%
        /// </summary>
        [StatDescription(Title = "技能(击风)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("CycloneHoist 技能(击风)伤害% 20023")]
        CycloneHoist,
        /// <summary>
        /// 技能(风震天)伤害%
        /// </summary>
        [StatDescription(Title = "技能(风震天)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("BlowEarthHoist 技能(风震天)伤害% 20024")]
        BlowEarthHoist,
        /// <summary>
        /// 技能(龙卷风)伤害%
        /// </summary>
        [StatDescription(Title = "技能(龙卷风)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DragonTornadoHoist 技能(龙卷风)伤害% 20025")]
        DragonTornadoHoist,
        /// <summary>
        /// 技能(灵魂火符)伤害%
        /// </summary>
        [StatDescription(Title = "技能(灵魂火符)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ExplosiveTalismanHoist 技能(灵魂火符)伤害% 20026")]
        ExplosiveTalismanHoist,
        /// <summary>
        /// 技能(月魂断玉)伤害%
        /// </summary>
        [StatDescription(Title = "技能(月魂断玉)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("EvilSlayerHoist 技能(月魂断玉)伤害% 20027")]
        EvilSlayerHoist,
        /// <summary>
        /// 技能(月魂灵波)伤害%
        /// </summary>
        [StatDescription(Title = "技能(月魂灵波)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("GreaterEvilSlayerHoist 技能(月魂灵波)伤害% 20028")]
        GreaterEvilSlayerHoist,
        /// <summary>
        /// 技能(灭魂火符)伤害%
        /// </summary>
        [StatDescription(Title = "技能(灭魂火符)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ImprovedExplosiveTalismanHoist 技能(灭魂火符)伤害% 20029")]
        ImprovedExplosiveTalismanHoist,
        /// <summary>
        /// 技能(盛开)伤害%
        /// </summary>
        [StatDescription(Title = "技能(盛开)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FullBloomHoist 技能(盛开)伤害% 20030")]
        FullBloomHoist,
        /// <summary>
        /// 技能(白莲)伤害%
        /// </summary>
        [StatDescription(Title = "技能(白莲)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("WhiteLotusHoist 技能(白莲)伤害% 20031")]
        WhiteLotusHoist,
        /// <summary>
        /// 技能(红莲)伤害%
        /// </summary>
        [StatDescription(Title = "技能(红莲)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("RedLotusHoist 技能(红莲)伤害% 20032")]
        RedLotusHoist,
        /// <summary>
        /// 技能(月季)伤害%
        /// </summary>
        [StatDescription(Title = "技能(月季)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("SweetBrierHoist 技能(月季)伤害% 20033")]
        SweetBrierHoist,
        /// <summary>
        /// 技能(狂涛涌泉)伤害%
        /// </summary>
        [StatDescription(Title = "技能(狂涛涌泉)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("DragonRepulseHoist 技能(狂涛涌泉)伤害% 20034")]
        DragonRepulseHoist,
        /// <summary>
        /// 技能(日闪)伤害%
        /// </summary>
        [StatDescription(Title = "技能(日闪)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("FlashOfLightHoist 技能(日闪)伤害% 20035")]
        FlashOfLightHoist,

        /*  未完成
        /// <summary>
        /// 技能(天之怒火)伤害%
        /// </summary>
        [StatDescription(Title = "技能(天之怒火)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("AsteroidHoist 技能(天之怒火)伤害%")]
        AsteroidHoist,
        /// <summary>
        /// 技能(天打雷劈)伤害%
        /// </summary>
        [StatDescription(Title = "技能(天打雷劈)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("JudgementOfHeavenHoist 技能(天打雷劈)伤害%")]
        JudgementOfHeavenHoist,
        /// <summary>
        /// 技能(抗拒火环)伤害%
        /// </summary>
        [StatDescription(Title = "技能(抗拒火环)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("RepulsionHoist 技能(抗拒火环)伤害%")]
        RepulsionHoist,
        /// <summary>
        /// 技能(空拳刀法)伤害%
        /// </summary>
        [StatDescription(Title = "技能(空拳刀法)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("TaoistCombatKickHoist 技能(空拳刀法)伤害%")]
        TaoistCombatKickHoist,
        /// <summary>
        /// 技能(吸星大法)伤害%
        /// </summary>
        [StatDescription(Title = "技能(吸星大法)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("LifeStealHoist 技能(吸星大法)伤害%")]
        LifeStealHoist,
        /// <summary>
        /// 技能(横扫千军)伤害%
        /// </summary>
        [StatDescription(Title = "技能(横扫千军)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("ThunderKickHoist 技能(横扫千军)伤害%")]
        ThunderKickHoist,
        /// <summary>
        /// 技能(孽报)伤害%
        /// </summary>
        [StatDescription(Title = "技能(孽报)伤害", Format = "{0:+#0%;-#0%;#0%}", Mode = StatType.Percent)]
        [Description("KarmaHoist 技能(孽报)伤害%")]
        KarmaHoist,*/

        /// <summary>
        /// 武器附加暴击几率%
        /// </summary>
        [StatDescription(Title = "暴击几率", Format = "{0:+#0;-#0;#0}%", Mode = StatType.Default)]
        [Description("WeponCriticalChance 武器附加暴击几率% 60001")]
        WeponCriticalChance = 60001,

        /// <summary>
        /// 生命值
        /// </summary>
        [StatDescription(Title = "自动恢复HP上限", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        [Description("MedicamentsHealth 自动回升HP上限 70001")]
        MedicamentsHealth = 70001,
        /// <summary>
        /// 魔法值
        /// </summary>
        [StatDescription(Title = "自动恢复MP上限", Format = "{0:+#0;-#0;#0}", Mode = StatType.None)]
        [Description("MedicamentsMana 自动回升MP上限 70002")]
        MedicamentsMana,
    }

    /// <summary>
    /// 快捷键绑定操作
    /// </summary>
    [Lang("快捷键绑定操作")]
    public enum KeyBindAction
    {
        /// <summary>
        /// 置空
        /// </summary>
        [Description("置空")]
        None,
        /// <summary>
        /// 设置
        /// </summary>
        [Description("设置")]
        ConfigWindow,
        /// <summary>
        /// 角色
        /// </summary>
        [Description("角色")]
        CharacterWindow,
        /// <summary>
        /// 背包
        /// </summary>
        [Description("背包")]
        InventoryWindow,
        /// <summary>
        /// 技能
        /// </summary>
        [Description("技能")]
        MagicWindow,
        /// <summary>
        /// 技能快捷栏
        /// </summary>
        [Description("技能快捷栏")]
        MagicBarWindow,
        /// <summary>
        /// 排行榜
        /// </summary>
        [Description("排行榜")]
        RankingWindow,
        /// <summary>
        /// 商城
        /// </summary>
        [Description("商城")]
        GameStoreWindow,
        /// <summary>
        /// 宠物
        /// </summary>
        [Description("宠物")]
        CompanionWindow,
        /// <summary>
        /// 组队
        /// </summary>
        [Description("组队")]
        GroupWindow,
        /// <summary>
        /// 仓库
        /// </summary>
        //[Description("仓库")]
        //StorageWindow,
        /// <summary>
        /// 行会
        /// </summary>
        [Description("行会")]
        GuildWindow,
        /// <summary>
        /// 任务日志
        /// </summary>
        [Description("任务日志")]
        QuestLogWindow,
        ///// <summary>
        ///// 任务跟踪
        ///// </summary>
        [Description("任务跟踪")]
        QuestTrackerWindow,
        /// <summary>
        /// 物品快捷栏
        /// </summary>
        [Description("物品快捷栏")]
        BeltWindow,
        /// <summary>
        /// 寄售
        /// </summary>
        [Description("寄售")]
        MarketPlaceWindow,
        /// <summary>
        /// 小地图
        /// </summary>
        [Description("小地图")]
        MapMiniWindow,
        /// <summary>
        /// 切换小地图大小
        /// </summary>
        [Description("切换小地图大小")]
        MapMiniWindowSize,
        /// <summary>
        /// 大地图
        /// </summary>
        [Description("大地图")]
        MapBigWindow,
        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("交流")]
        CommunicationBoxWindow,

        /// <summary>
        /// 退出游戏
        /// </summary>
        [Description("退出游戏")]
        ExitGameWindow,
        /// <summary>
        /// 切换攻击模式
        /// </summary>
        [Description("切换攻击模式")]
        ChangeAttackMode,
        /// <summary>
        /// 切换宠物模式
        /// </summary>
        [Description("切换宠物模式")]
        ChangePetMode,
        /// <summary>
        /// 切换允许组队
        /// </summary>
        [Description("切换允许组队")]
        GroupAllowSwitch,
        /// <summary>
        /// 将目标添加到组队
        /// </summary>
        [Description("将目标添加到组队")]
        GroupTarget,
        /// <summary>
        /// 请求交易
        /// </summary>
        [Description("请求交易")]
        TradeRequest,
        /// <summary>
        /// 切换允许交易
        /// </summary>
        [Description("切换允许交易")]
        TradeAllowSwitch,
        /// <summary>
        /// 拾取物品
        /// </summary>
        [Description("拾取物品")]
        ItemPickUp,
        /// <summary>
        /// 夫妻传送
        /// </summary>
        [Description("夫妻传送")]
        PartnerTeleport,
        /// <summary>
        /// 切换骑乘
        /// </summary>
        [Description("切换骑乘")]
        MountToggle,
        /// <summary>
        /// 切换自动跑步
        /// </summary>
        [Description("切换自动跑步")]
        AutoRunToggle,
        /// <summary>
        /// 切换聊天模式
        /// </summary>
        [Description("切换聊天模式")]
        ChangeChatMode,
        /// <summary>
        /// 物品快捷栏1
        /// </summary>
        [Description("物品快捷栏 1")]
        UseBelt01,
        /// <summary>
        /// 物品快捷栏2
        /// </summary>
        [Description("物品快捷栏 2")]
        UseBelt02,
        /// <summary>
        /// 物品快捷栏3
        /// </summary>
        [Description("物品快捷栏 3")]
        UseBelt03,
        /// <summary>
        /// 物品快捷栏4
        /// </summary>
        [Description("物品快捷栏 4")]
        UseBelt04,
        /// <summary>
        /// 物品快捷栏5
        /// </summary>
        [Description("物品快捷栏 5")]
        UseBelt05,
        /// <summary>
        /// 物品快捷栏6
        /// </summary>
        [Description("物品快捷栏 6")]
        UseBelt06,
        /// <summary>
        /// 技能快捷栏1
        /// </summary>
        [Description("技能快捷栏 1")]
        SpellSet01,
        /// <summary>
        /// 技能快捷栏2
        /// </summary>
        [Description("技能快捷栏 2")]
        SpellSet02,
        /// <summary>
        /// 技能快捷栏3
        /// </summary>
        [Description("技能快捷栏 3")]
        SpellSet03,
        /// <summary>
        /// 技能快捷栏4
        /// </summary>
        [Description("技能快捷栏 4")]
        SpellSet04,
        /// <summary>
        /// 使用技能1
        /// </summary>
        [Description("使用技能 1")]
        SpellUse01,
        /// <summary>
        /// 使用技能2
        /// </summary>
        [Description("使用技能 2")]
        SpellUse02,
        /// <summary>
        /// 使用技能3
        /// </summary>
        [Description("使用技能 3")]
        SpellUse03,
        /// <summary>
        /// 使用技能4
        /// </summary>
        [Description("使用技能 4")]
        SpellUse04,
        /// <summary>
        /// 使用技能5
        /// </summary>
        [Description("使用技能 5")]
        SpellUse05,
        /// <summary>
        /// 使用技能6
        /// </summary>
        [Description("使用技能 6")]
        SpellUse06,
        /// <summary>
        /// 使用技能7
        /// </summary>
        [Description("使用技能 7")]
        SpellUse07,
        /// <summary>
        /// 使用技能8
        /// </summary>
        [Description("使用技能 8")]
        SpellUse08,
        /// <summary>
        /// 使用技能9
        /// </summary>
        [Description("使用技能 9")]
        SpellUse09,
        /// <summary>
        /// 使用技能10
        /// </summary>
        [Description("使用技能 10")]
        SpellUse10,
        /// <summary>
        /// 使用技能11
        /// </summary>
        [Description("使用技能 11")]
        SpellUse11,
        /// <summary>
        /// 使用技能12
        /// </summary>
        [Description("使用技能 12")]
        SpellUse12,
        /// <summary>
        /// 锁定/解锁物品
        /// </summary>
        [Description("锁定/解锁物品")]
        ToggleItemLock,
        /// <summary>
        /// 爆率查询
        /// </summary>
        //[Description("爆率查询")]
        //FortuneWindow,
        /// <summary>
        /// 主菜单
        /// </summary>
        //[Description("主菜单")]
        //MenuBoxWindow,
        /// <summary>
        /// 大补帖
        /// </summary>
        [Description("大补帖")]
        BigPatchBoxWindow,
        /// <summary>
        /// D键功能
        /// </summary>
        [Description("D键功能")]
        NPCDKeyWindow,
        /// <summary>
        /// 聊天输入窗口
        /// </summary>
        //[Description("聊天输入窗口")]
        //ChatInputWindow,
        /// <summary>
        /// 聊天窗口
        /// </summary>
        //[Description("聊天窗口")]
        ChatWindow,
        /// <summary>
        /// 制作列表
        /// </summary>
        //[Description("制作列表")]
        //CraftWindow,
        /// <summary>
        /// 记忆传送
        /// </summary>
        //[Description("记忆传送")]
        //FixedPointWindow,
        /// <summary>
        /// BUFF框
        /// </summary>
        [Description("BUFF框")]
        BuffWindow,
        /// <summary>
        /// 死亡复活
        /// </summary>
        [Description("死亡复活")]
        TownReviveWindow,
        /// <summary>
        /// 拍卖行
        /// </summary>
        [Description("拍卖行")]
        AuctionsWindow,
        /// <summary>
        /// 忠诚度排行版
        /// </summary>
        [Description("忠诚度排行版")]
        BonusPoolWindow,
        /// <summary>
        /// 攻城兵器
        /// </summary>
        [Description("攻城兵器")]
        WarWeaponWindow,
        /// <summary>
        /// 队友框
        /// </summary>
        [Description("队友框")]
        GroupFrameWindow,
        /// <summary>
        /// 使用技能13
        /// </summary>
        [Description("使用技能 13")]
        SpellUse13,
        /// <summary>
        /// 使用技能14
        /// </summary>
        [Description("使用技能 14")]
        SpellUse14,
        /// <summary>
        /// 使用技能15
        /// </summary>
        [Description("使用技能 15")]
        SpellUse15,
        /// <summary>
        /// 使用技能16
        /// </summary>
        [Description("使用技能 16")]
        SpellUse16,
        /// <summary>
        /// 使用技能17
        /// </summary>
        [Description("使用技能 17")]
        SpellUse17,
        /// <summary>
        /// 使用技能18
        /// </summary>
        [Description("使用技能 18")]
        SpellUse18,
        /// <summary>
        /// 使用技能19
        /// </summary>
        [Description("使用技能 19")]
        SpellUse19,
        /// <summary>
        /// 使用技能20
        /// </summary>
        [Description("使用技能 20")]
        SpellUse20,
        /// <summary>
        /// 使用技能21
        /// </summary>
        [Description("使用技能 21")]
        SpellUse21,
        /// <summary>
        /// 使用技能22
        /// </summary>
        [Description("使用技能 22")]
        SpellUse22,
        /// <summary>
        /// 使用技能23
        /// </summary>
        [Description("使用技能 23")]
        SpellUse23,
        /// <summary>
        /// 使用技能24
        /// </summary>
        [Description("使用技能 24")]
        SpellUse24,
    }

    /// <summary>
    /// 角色性别
    /// </summary>
    [Lang("角色性别")]
    public enum MirGender : byte
    {
        /// <summary>
        /// 性别男
        /// </summary>
	    [Description("男")]
        Male,
        /// <summary>
        /// 性别女
        /// </summary>
		[Description("女")]
        Female
    }

    /// <summary>
    /// 角色职业
    /// </summary>
    [Lang("角色职业")]
    public enum MirClass : byte
    {
        /// <summary>
        /// 职业战士
        /// </summary>
	    [Description("战士")]
        Warrior,
        /// <summary>
        /// 职业法师
        /// </summary>
		[Description("法师")]
        Wizard,
        /// <summary>
        /// 职业道士
        /// </summary>
		[Description("道士")]
        Taoist,
        /// <summary>
        /// 职业刺客
        /// </summary>
		[Description("刺客")]
        Assassin,
    }

    /// <summary>
    /// 攻击模式
    /// </summary>
    [Lang("攻击模式")]
    public enum AttackMode : byte
    {
        /// <summary>
        /// 和平模式
        /// </summary>
        [Description("[和平攻击]")]
        Peace,
        /// <summary>
        /// 组队模式
        /// </summary>
        [Description("[组队攻击]")]
        Group,
        /// <summary>
        /// 行会模式
        /// </summary>
        [Description("[行会攻击]")]
        Guild,
        /// <summary>
        /// 善恶模式
        /// </summary>
        [Description("[善恶攻击]")]
        WarRedBrown,
        /// <summary>
        /// 全体模式
        /// </summary>
        [Description("[全体攻击]")]
        All
    }

    /// <summary>
    /// 宠物攻击模式
    /// </summary>
    [Lang("宠物攻击模式")]
    public enum PetMode : byte
    {
        /// <summary>
        /// 宠物跟随攻击模式
        /// </summary>
        [Description("[跟随,攻击]")]
        Both,
        /// <summary>
        /// 宠物跟随模式
        /// </summary>
        [Description("[跟随]")]
        Move,
        /// <summary>
        /// 宠物攻击模式
        /// </summary>
        [Description("[攻击]")]
        Attack,
        /// <summary>
        /// 宠物竞技PVP模式
        /// </summary>
        [Description("[竞技]")]
        PvP,
        /// <summary>
        /// 宠物停止模式
        /// </summary>
        [Description("[停止]")]
        None,
    }

    public enum CharacterState : byte
    {
        Normal = 0,
        Block = 1,
        Sell = 2
    }

    /// <summary>
    /// 方位朝向
    /// </summary>
    public enum MirDirection : byte
    {
        /// <summary>
        /// 方向向上
        /// </summary>
        [Description("向上")]
        Up = 0,
        /// <summary>
        /// 方向右上
        /// </summary>
        [Description("右上")]
        UpRight = 1,
        /// <summary>
        /// 方向向右
        /// </summary>
		[Description("向右")]
        Right = 2,
        /// <summary>
        /// 方向右下
        /// </summary>
        [Description("右下")]
        DownRight = 3,
        /// <summary>
        /// 方向向下
        /// </summary>
		[Description("向下")]
        Down = 4,
        /// <summary>
        /// 方向左下
        /// </summary>
        [Description("左下")]
        DownLeft = 5,
        /// <summary>
        /// 方向向左
        /// </summary>
		[Description("向左")]
        Left = 6,
        /// <summary>
        /// 方向左上
        /// </summary>
        [Description("左上")]
        UpLeft = 7
    }

    [Flags]
    /// <summary>
    /// 职业要求
    /// </summary>
    [Lang("职业要求")]
    public enum RequiredClass : byte
    {
        /// <summary>
        /// 职业要求无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 职业要求战士
        /// </summary>
		[Description("战士")]
        Warrior = 1,
        /// <summary>
        /// 职业要求法师
        /// </summary>
		[Description("法师")]
        Wizard = 2,
        /// <summary>
        /// 职业要求道士
        /// </summary>
		[Description("道士")]
        Taoist = 4,
        /// <summary>
        /// 职业要求刺客
        /// </summary>
		[Description("刺客")]
        Assassin = 8,
        /// <summary>
        /// 职业要求战法道
        /// </summary>
        [Description("战士/法师/道士")]
        WarWizTao = Warrior | Wizard | Taoist,
        /// <summary>
        /// 职业要求法道
        /// </summary>
        [Description("法师/道士")]
        WizTao = Wizard | Taoist,
        /// <summary>
        /// 职业要求战刺
        /// </summary>
        [Description("战士/刺客")]
        AssWar = Warrior | Assassin,
        /// <summary>
        /// 职业要求全职业
        /// </summary>
		[Description("全职业通用")]
        All = WarWizTao | Assassin
    }

    [Flags]
    /// <summary>
    /// 性别要求
    /// </summary>
    [Lang("性别要求")]
    public enum RequiredGender : byte
    {
        /// <summary>
        /// 性别要求男
        /// </summary>
        [Description("男")]
        Male = 1,
        /// <summary>
        /// 性别要求女
        /// </summary>
        [Description("女")]
        Female = 2,
        /// <summary>
        /// 性别要求男女通用
        /// </summary>
        [Description("男女通用")]
        None = Male | Female
    }

    /// <summary>
    /// 装备框架位置 
    /// </summary>
    public enum EquipmentSlot
    {
        /// <summary>
        /// 武器框架位置
        /// </summary>
        Weapon = 0,
        /// <summary>
        /// 衣服框架位置
        /// </summary>
        Armour = 1,
        /// <summary>
        /// 头盔框架位置
        /// </summary>
        Helmet = 2,
        /// <summary>
        /// 火把框架位置
        /// </summary>
        Torch = 3,
        /// <summary>
        /// 项链框架位置
        /// </summary>
        Necklace = 4,
        /// <summary>
        /// 左手镯框架位置
        /// </summary>
        BraceletL = 5,
        /// <summary>
        /// 右手镯框架位置
        /// </summary>
        BraceletR = 6,
        /// <summary>
        /// 左戒指框架位置
        /// </summary>
        RingL = 7,
        /// <summary>
        /// 右戒指框架位置
        /// </summary>
        RingR = 8,
        /// <summary>
        /// 鞋子框架位置
        /// </summary>
        Shoes = 9,
        /// <summary>
        /// 毒药框架位置
        /// </summary>
        Poison = 10,
        /// <summary>
        /// 护身符框架位置
        /// </summary>
        Amulet = 11,
        /// <summary>
        /// 鲜花框架位置
        /// </summary>
        Flower = 12,
        /// <summary>
        /// 马甲框架位置
        /// </summary>
        HorseArmour = 13,
        /// <summary>
        /// 徽章框架位置
        /// </summary>
        Emblem = 14,
        /// <summary>
        /// 盾牌框架位置
        /// </summary>
        Shield = 15,
        /// <summary>
        /// 声望称号框架位置
        /// </summary>
        FameTitle = 16,
        /// <summary>
        /// 时装框架位置
        /// </summary>
        Fashion = 17,
        /// <summary>
        /// 自动药剂框架位置
        /// </summary>
        [Description("自动药剂")]
        Medicament = 18,
    }
    /// <summary>
    /// 宠物装备框架位置
    /// </summary>
    public enum CompanionSlot
    {
        /// <summary>
        /// 宠物装备格包裹
        /// </summary>
        Bag = 0,
        /// <summary>
        /// 宠物装备格头饰
        /// </summary>
        Head = 1,
        /// <summary>
        /// 宠物装备格背饰
        /// </summary>
        Back = 2,
        /// <summary>
        /// 宠物装备格粮食
        /// </summary>
        Food = 3,
    }
    /// <summary>
    /// 钓鱼装备框架位置
    /// </summary>
    public enum FishingSlot
    {
        /// <summary>
        /// 鱼钩框架位置
        /// </summary>
        Hook = 0,
        /// <summary>
        /// 鱼漂框架位置
        /// </summary>
        Float = 1,
        /// <summary>
        /// 鱼饵框架位置
        /// </summary>
        Bait = 2,
        /// <summary>
        /// 探鱼器框架位置
        /// </summary>
        Finder = 3,
        /// <summary>
        /// 摇轮框架位置
        /// </summary>
        Reel = 4,
    }
    /// <summary>
    /// 格子类型
    /// </summary>
    public enum GridType
    {
        /// <summary>
        /// 格子类型为空
        /// </summary>
        None,
        /// <summary>
        /// 格子类型包裹
        /// </summary>
        Inventory,
        /// <summary>
        /// 格子类型人物装备格子
        /// </summary>
        Equipment,
        /// <summary>
        /// 格子类型物品快捷栏
        /// </summary>
        Belt,
        /// <summary>
        /// 格子类型卖
        /// </summary>
        Sell,
        /// <summary>
        /// 格子类型修理
        /// </summary>
        Repair,
        /// <summary>
        /// 格子类型仓库
        /// </summary>
        Storage,
        /// <summary>
        /// 格子类型自动喝药
        /// </summary>
        AutoPotion,
        /// <summary>
        /// 格子类型精炼部分黑铁
        /// </summary>
        RefineBlackIronOre,
        /// <summary>
        /// 格子类型精炼部分配件
        /// </summary>
        RefineAccessory,
        /// <summary>
        /// 格子类型精炼部分特殊道具
        /// </summary>
        RefineSpecial,
        /// <summary>
        /// 格子类型观察者
        /// </summary>
        Inspect,
        /// <summary>
        /// 格子类型寄售
        /// </summary>
        Consign,
        /// <summary>
        /// 格子类型邮件
        /// </summary>
        SendMail,
        /// <summary>
        /// 格子类型交易用户
        /// </summary>
        TradeUser,
        /// <summary>
        /// 格子类型交易玩家
        /// </summary>
        TradePlayer,
        /// <summary>
        /// 行会仓库
        /// </summary>
        GuildStorage,
        /// <summary>
        /// 格子类型宠物包裹
        /// </summary>
        CompanionInventory,
        /// <summary>
        /// 格子类型宠物装备格子
        /// </summary>
        CompanionEquipment,
        /// <summary>
        /// 格子类型婚戒
        /// </summary>
        WeddingRing,
        /// <summary>
        /// 格子类型精炼部分铁矿
        /// </summary>
        RefinementStoneIronOre,
        /// <summary>
        /// 格子类型精炼部分银矿
        /// </summary>
        RefinementStoneSilverOre,
        /// <summary>
        /// 格子类型精炼部分金刚石
        /// </summary>
        RefinementStoneDiamond,
        /// <summary>
        /// 格子类型精炼部分金矿
        /// </summary>
        RefinementStoneGoldOre,
        /// <summary>
        /// 格子类型精炼部分晶石
        /// </summary>
        RefinementStoneCrystal,
        /// <summary>
        /// 格子类型道具碎片
        /// </summary>
        ItemFragment,
        /// <summary>
        /// 格子类型升级首饰提升属性
        /// </summary>
        AccessoryRefineUpgradeTarget,
        /// <summary>
        /// 格子类型首饰升级目标
        /// </summary>
        AccessoryRefineLevelTarget,
        /// <summary>
        /// 格子类型首饰升级物品
        /// </summary>
        AccessoryRefineLevelItems,
        /// <summary>
        /// 格子类型大师精炼碎片1
        /// </summary>
        MasterRefineFragment1,
        /// <summary>
        /// 格子类型大师精炼碎片2
        /// </summary>
        MasterRefineFragment2,
        /// <summary>
        /// 格子类型大师精炼碎片3
        /// </summary>
        MasterRefineFragment3,
        /// <summary>
        /// 格子类型大师精炼石
        /// </summary>
        MasterRefineStone,
        /// <summary>
        /// 格子类型大师精炼特殊
        /// </summary>
        MasterRefineSpecial,
        /// <summary>
        /// 格子类型首饰重置
        /// </summary>
        AccessoryReset,
        /// <summary>
        /// 格子类型武器工艺模板
        /// </summary>
        WeaponCraftTemplate,
        /// <summary>
        /// 格子类型武器工艺黄色
        /// </summary>
        WeaponCraftYellow,
        /// <summary>
        /// 格子类型武器工艺蓝色
        /// </summary>
        WeaponCraftBlue,
        /// <summary>
        /// 格子类型武器工艺红色
        /// </summary>
        WeaponCraftRed,
        /// <summary>
        /// 格子类型武器工艺紫色
        /// </summary>
        WeaponCraftPurple,
        /// <summary>
        /// 格子类型武器工艺绿色
        /// </summary>
        WeaponCraftGreen,
        /// <summary>
        /// 格子类型武器工艺灰色
        /// </summary>
        WeaponCraftGrey,
        /// <summary>
        /// 碎片包裹
        /// </summary>
        PatchGrid,
        /// <summary>
        /// 格子类型钓鱼装备
        /// </summary>
        FishingEquipment,
        /// <summary>
        /// 格子类型目标书籍
        /// </summary>
        BookTarget,
        /// <summary>
        /// 格子类型材料书籍
        /// </summary>
        BookMaterial,
        /// <summary>
        /// 格子类型被镶嵌的装备
        /// </summary>
        GemTargetItem,
        /// <summary>
        /// 格子类型开孔/取下宝石需要的材料
        /// </summary>
        GemMaterial,
        /// <summary>
        /// 格子类型提高成功率材料
        /// </summary>
        ChanceMaterial,
        /// <summary>
        /// 格子类型特殊修理
        /// </summary>
        SpecialRepair,
        /// <summary>
        /// 格子类型一键出售
        /// </summary>
        RootSell,
        /// <summary>
        /// 格子类型自定义对话框需求物品
        /// </summary>
        CustomDialog,
        /// <summary>
        /// 格子类型待升级的武器
        /// </summary>
        UpgradeWeapon,
        /// <summary>
        /// 格子类型首饰合成目标
        /// </summary>
        AccessoryComposeLevelTarget,
        /// <summary>
        /// 格子类型首饰合成物品
        /// </summary>
        AccessoryComposeLevelItems,
        /// <summary>
        /// 格子类型首饰合成钢玉石
        /// </summary>
        RefinementStoneCorundum,
        /// <summary>
        /// 格子类型精炼部分魔晶石
        /// </summary>
        RefinementStoneMateria,
        /// <summary>
        /// NPC选择的物品
        /// </summary>
        NPCSelect,
    }
    /// <summary>
    /// BUFF类型
    /// </summary>
    [Lang("BUFF类型")]
    public enum BuffType
    {
        /// <summary>
        /// 置空
        /// </summary>
        [Description("置空")]
        None,
        /// <summary>
        /// 服务器BUFF
        /// </summary>
        [Description("服务器BUFF")]
        Server = 1,
        /// <summary>
        /// 赏金BUFF
        /// </summary>
        [Description("赏金BUFF")]
        HuntGold = 2,
        /// <summary>
        /// 观察者BUFF
        /// </summary>
        [Description("观察者BUFF")]
        Observable = 3,
        /// <summary>
        /// 灰名BUFF
        /// </summary>
        [Description("灰名BUFF")]
        Brown = 4,
        /// <summary>
        /// PK值BUFF
        /// </summary>
        [Description("PKBUFF")]
        PKPoint = 5,
        /// <summary>
        /// PVP诅咒BUFF
        /// </summary>
        [Description("PVPBUFF")]
        PvPCurse = 6,
        /// <summary>
        /// 救赎BUFF
        /// </summary>
        [Description("救赎BUFF")]
        Redemption = 7,
        /// <summary>
        /// 宠物BUFF
        /// </summary>
        [Description("宠物BUFF")]
        Companion = 8,
        /// <summary>
        /// 沙巴克BUFF
        /// </summary>
        [Description("沙巴克BUFF")]
        Castle = 9,
        /// <summary>
        /// 时效道具属性BUFF
        /// </summary>
        [Description("时效道具属性BUFF")]
        ItemBuff = 10,
        /// <summary>
        /// 永久道具属性BUFF
        /// </summary>
        [Description("永久道具属性BUFF")]
        ItemBuffPermanent = 11,
        /// <summary>
        /// 排行BUFF
        /// </summary>
        [Description("排行版BUFF")]
        Ranking = 12,
        /// <summary>
        /// 管理员BUFF
        /// </summary>
        [Description("管理员BUFF")]
        Developer = 13,
        /// <summary>
        /// 回归者BUFF
        /// </summary>
        [Description("回归者BUFF")]
        Veteran = 14,
        /// <summary>
        /// 地图附加属性BUFF
        /// </summary>
        [Description("地图附加属性BUFF")]
        MapEffect = 15,
        /// <summary>
        /// 行会BUFF
        /// </summary>
        [Description("行会BUFF")]
        Guild = 16,
        /// <summary>
        /// 死亡药水BUFF
        /// </summary>
        [Description("死亡药水BUFF")]
        DeathDrops = 17,
        /// <summary>
        /// 组队BUFF
        /// </summary>
        [Description("组队BUFF")]
        Group = 18,
        /// <summary>
        /// 奖金池BUFF
        /// </summary>
        [Description("奖金池BUFF")]
        RewardPool = 19,
        /// <summary>
        /// 铁布衫BUFF
        /// </summary>
        [Description("铁布衫BUFF")]
        Defiance = 100,
        /// <summary>
        /// 破血狂杀BUFF
        /// </summary>
        [Description("破血狂杀BUFF")]
        Might = 101,
        /// <summary>
        /// 金刚之躯BUFF
        /// </summary>
        [Description("金刚之躯BUFF")]
        Endurance = 102,
        /// <summary>
        /// 移花接木BUFF
        /// </summary>
        [Description("移花接木BUFF")]
        ReflectDamage = 103,
        /// <summary>
        /// 无敌BUFF
        /// </summary>
        [Description("无敌BUFF")]
        Invincibility = 104,
        /// <summary>
        /// 凝血离魂BUFF
        /// </summary>
        [Description("凝血离魂BUFF")]
        Renounce = 200,
        /// <summary>
        /// 魔法盾BUFF
        /// </summary>
        [Description("魔法盾BUFF")]
        MagicShield = 201,
        /// <summary>
        /// 天打雷劈BUFF
        /// </summary>
        [Description("天打雷劈BUFF")]
        JudgementOfHeaven = 202,
        /// <summary>
        /// 护身法盾BUFF
        /// </summary>
        [Description("护身法盾BUFF")]
        SuperiorMagicShield = 203,
        /// <summary>
        /// 治愈术BUFF
        /// </summary>
        [Description("治愈术BUFF")]
        Heal = 300,
        /// <summary>
        /// 隐身术BUFF
        /// </summary>
        [Description("隐身术BUFF")]
        Invisibility = 301,
        /// <summary>
        /// 幽灵盾BUFF
        /// </summary>
        [Description("幽灵盾BUFF")]
        MagicResistance = 302,
        /// <summary>
        /// 神圣战甲术BUFF
        /// </summary>
        [Description("神圣战甲术BUFF")]
        Resilience = 303,
        /// <summary>
        /// 强震魔法BUFF
        /// </summary>
        [Description("强震魔法BUFF")]
        ElementalSuperiority = 304,
        /// <summary>
        /// 猛虎强势BUFF
        /// </summary>
        [Description("猛虎强势BUFF")]
        BloodLust = 305,
        /// <summary>
        /// 移花接玉BUFF
        /// </summary>
        [Description("移花接玉BUFF")]
        StrengthOfFaith = 306,
        /// <summary>
        /// 阴阳法环BUFF
        /// </summary>
        [Description("阴阳法环BUFF")]
        CelestialLight = 307,
        /// <summary>
        /// 妙影无踪BUFF
        /// </summary>
        [Description("妙影无踪BUFF")]
        Transparency = 308,
        /// <summary>
        /// 吸星大法BUFF
        /// </summary>
        [Description("吸血BUFF")]
        LifeSteal = 309,
        /// <summary>
        /// 毒云BUFF
        /// </summary>
        [Description("毒云BUFF")]
        PoisonousCloud = 400,
        /// <summary>
        /// 盛开BUFF
        /// </summary>
        [Description("盛开BUFF")]
        FullBloom = 401,
        /// <summary>
        /// 白莲BUFF
        /// </summary>
        [Description("白莲BUFF")]
        WhiteLotus = 402,
        /// <summary>
        /// 红莲BUFF
        /// </summary>
        [Description("红莲BUFF")]
        RedLotus = 403,
        /// <summary>
        /// 潜行BUFF
        /// </summary>
        [Description("潜行BUFF")]
        Cloak = 404,
        /// <summary>
        /// 鬼灵步BUFF
        /// </summary>
        [Description("鬼灵步BUFF")]
        GhostWalk = 405,
        /// <summary>
        /// 心击一转BUFF
        /// </summary>
        [Description("心机一转BUFF")]
        TheNewBeginning = 406,
        /// <summary>
        /// 黄泉旅者BUFF
        /// </summary>
        [Description("黄泉旅者BUFF")]
        DarkConversion = 407,
        /// <summary>
        /// 狂涛涌泉BUFF
        /// </summary>
        [Description("狂涛涌泉BUFF")]
        DragonRepulse = 408,
        /// <summary>
        /// 风之闪避BUFF
        /// </summary>
        [Description("风之闪避BUFF")]
        Evasion = 409,
        /// <summary>
        /// 风之守护BUFF
        /// </summary>
        [Description("风之守护BUFF")]
        RagingWind = 410,
        /// <summary>
        /// 护身冰环BUFF
        /// </summary>
        [Description("护身冰环BUFF")]
        FrostBite = 411,
        /// <summary>
        /// 离魂邪风BUFF
        /// </summary>
        [Description("离魂邪风BUFF")]
        ElementalHurricane = 412,
        /// <summary>
        /// 集中BUFF
        /// </summary>
        [Description("集中BUFF")]
        Concentration = 413,
        /// <summary>
        /// 千刃杀风BUFF
        /// </summary>
        [Description("千刃杀风BUFF")]
        SuperTransparency = 414,
        /// <summary>
        /// 魔防衰弱BUFF
        /// </summary>
        [Description("魔防衰弱BUFF")]
        MagicWeakness = 500,
        /// <summary>
        /// 姜太公BUFF
        /// </summary>
        [Description("姜太公BUFF")]
        FishingMaster = 600,
        /// <summary>
        /// 自定义BUFF
        /// </summary>
        [Description("自定义BUFF")]
        CustomBuff = 900,
        /// <summary>
        /// 泰山投币BUFF
        /// </summary>
        [Description("泰山投币BUFF")]
        TarzanBuff = 901,
        /// <summary>
        /// 活动BUFF
        /// </summary>
        [Description("活动BUFF")]
        EventBuff = 902,
        /// <summary>
        /// 宝宝幻影BUFF
        /// </summary>
        [Description("宝宝幻影BUFF")]
        PhantomBuff = 903,
        /// <summary>
        /// 转生BUFF
        /// </summary>
        [Description("转生BUFF")]
        AfterImages = 950,
        /// <summary>
        /// 怪物BUFF
        /// </summary>
        [Description("怪物BUFF")]
        MonsterBuff = 1000,
        /// <summary>
        /// 穿透BUFF
        /// </summary>
        [Description("穿透BUFF")]
        PierceBuff = 1101,
        /// <summary>
        /// 灼伤BUFF
        /// </summary>
        [Description("灼伤BUFF")]
        BurnBuff = 1102,
    }

    /// <summary>
    /// 道具要求类型
    /// </summary>
    [Lang("道具要求类型")]
    public enum RequiredType : byte
    {
        /// <summary>
        /// 要求类型等级
        /// </summary>
        [Description("等级")]
        Level,
        /// <summary>
        /// 要求类型最高等级
        /// </summary>
        [Description("最高等级")]
        MaxLevel,
        /// <summary>
        /// 要求类型物防
        /// </summary>
        [Description("物理防御")]
        AC,
        /// <summary>
        /// 要求类型魔防
        /// </summary>
        [Description("魔法防御")]
        MR,
        /// <summary>
        /// 要求类型破坏
        /// </summary>
        [Description("破坏")]
        DC,
        /// <summary>
        /// 要求类型自然
        /// </summary>
        [Description("自然")]
        MC,
        /// <summary>
        /// 要求类型灵魂
        /// </summary>
        [Description("灵魂")]
        SC,
        /// <summary>
        /// 要求类型生命值
        /// </summary>
        [Description("生命值")]
        Health,
        /// <summary>
        /// 要求类型魔法值
        /// </summary>
        [Description("魔法值")]
        Mana,
        /// <summary>
        /// 要求类型准确
        /// </summary>
        [Description("准确")]
        Accuracy,
        /// <summary>
        /// 要求类型敏捷
        /// </summary>
        [Description("敏捷")]
        Agility,
        /// <summary>
        /// 要求类型宠物等级
        /// </summary>
        [Description("宠物等级")]
        CompanionLevel,
        /// <summary>
        /// 要求类型最高宠物等级
        /// </summary>
        [Description("最高宠物等级")]
        MaxCompanionLevel,
        /// <summary>
        /// 要求类型转生等级
        /// </summary>
        [Description("转生等级")]
        RebirthLevel,
        /// <summary>
        /// 要求类型最高转生等级
        /// </summary>
        [Description("最高转生等级")]
        MaxRebirthLevel,
        /// <summary>
        /// 要求类型内功等级
        /// </summary>
        [Description("内功等级")]
        InternalSkill,
    }
    /// <summary>
    /// 物品分类
    /// </summary>
    [Lang("道具极品分类")]
    public enum Rarity : byte
    {
        /// <summary>
        /// 物品分类普通物品
        /// </summary>
        [Description("普通物品")]
        Common,
        /// <summary>
        /// 物品分类高级物品
        /// </summary>
        [Description("高级物品")]
        Superior,
        /// <summary>
        /// 物品分类稀世物品
        /// </summary>
		[Description("稀世物品")]
        Elite,
    }
    /// <summary>
    /// 地图光效
    /// </summary>
    public enum LightSetting : byte
    {
        /// <summary>
        /// 地图光效默认
        /// </summary>
        [Description("默认")]
        Default,
        /// <summary>
        /// 地图光效白天
        /// </summary>
        [Description("白天")]
        Light,
        /// <summary>
        /// 地图光效夜晚
        /// </summary>
        [Description("夜晚")]
        Night,
        /// <summary>
        /// 地图光效全亮
        /// </summary>
        [Description("全亮")]
        FullBright,
    }
    /// <summary>
    /// 天气效果
    /// </summary>
    public enum WeatherSetting : byte
    {
        /// <summary>
        /// 天气效果无
        /// </summary>
        [Description("空")]
        None,
        /// <summary>
        /// 天气效果默认
        /// </summary>
        [Description("默认")]
        Default,
        /// <summary>
        /// 天气效果雾
        /// </summary>
        [Description("雾")]
        Fog,
        /// <summary>
        /// 天气效果燃烧的雾
        /// </summary>
        //[Description("燃烧的雾")]
        //BurningFog,
        /// <summary>
        /// 天气效果雪
        /// </summary>
        [Description("雪")]
        Snow,
        /// <summary>
        /// 天气效果花瓣雨
        /// </summary>
        //[Description("花瓣雨")]
        //Everfall,
        /// <summary>
        /// 天气效果雨
        /// </summary>
        [Description("雨")]
        Rain,
    }
    /// <summary>
    /// 地图战斗属性
    /// </summary>
    public enum FightSetting : byte
    {
        /// <summary>
        /// 地图属性普通
        /// </summary>
        [Description("普通")]
        None,
        /// <summary>
        /// 地图属性安全
        /// </summary>
        [Description("安全")]
        Safe,
        /// <summary>
        /// 地图属性战斗
        /// </summary>
        [Description("战斗")]
        Fight,
        /// <summary>
        /// 地图属性战争
        /// </summary>
        [Description("战争")]
        War,
    }
    /// <summary>
    /// 对象类型
    /// </summary>
    public enum ObjectType : byte
    {
        /// <summary>
        /// 无对象类型
        /// </summary>
        [Description("无")]
        None,
        /// <summary>
        /// 玩家对象类型
        /// </summary>
        [Description("玩家")]
        Player,
        /// <summary>
        /// 道具对象类型
        /// </summary>
        [Description("道具")]
        Item,
        /// <summary>
        /// NPC对象类型
        /// </summary>
        [Description("NPC")]
        NPC,
        /// <summary>
        /// 物件施法对象类型
        /// </summary>
        [Description("物体施法对象")]
        Spell,
        /// <summary>
        /// 怪物对象类型
        /// </summary>
        [Description("怪物")]
        Monster
    }
    /// <summary>
    /// 道具分类
    /// </summary>
    [Lang("道具分类")]
    public enum ItemType : byte
    {
        /// <summary>
        /// 道具分类无
        /// </summary>
        [Description("无")]
        Nothing,
        /// <summary>
        /// 道具分类消耗品
        /// </summary>
        [Description("消耗品")]
        Consumable,
        /// <summary>
        /// 道具分类武器
        /// </summary>
		[Description("武器")]
        Weapon,
        /// <summary>
        /// 道具分类衣服
        /// </summary>
		[Description("衣服")]
        Armour,
        /// <summary>
        /// 道具分类火把
        /// </summary>
        [Description("火把")]
        Torch,
        /// <summary>
        /// 道具分类头盔
        /// </summary>
		[Description("头盔")]
        Helmet,
        /// <summary>
        /// 道具分类项链
        /// </summary>
		[Description("项链")]
        Necklace,
        /// <summary>
        /// 道具分类手镯
        /// </summary>
		[Description("手镯")]
        Bracelet,
        /// <summary>
        /// 道具分类戒指
        /// </summary>
		[Description("戒指")]
        Ring,
        /// <summary>
        /// 道具分类鞋子
        /// </summary>
		[Description("鞋子")]
        Shoes,
        /// <summary>
        /// 道具分类毒药
        /// </summary>
		[Description("毒药")]
        Poison,
        /// <summary>
        /// 道具分类护身符
        /// </summary>
		[Description("护身符")]
        Amulet,
        /// <summary>
        /// 道具分类肉类
        /// </summary>
		[Description("肉")]
        Meat,
        /// <summary>
        /// 道具分类矿石
        /// </summary>
		[Description("矿石")]
        Ore,
        /// <summary>
        /// 道具分类书
        /// </summary>
		[Description("书籍")]
        Book,
        /// <summary>
        /// 道具分类卷轴
        /// </summary>
		[Description("卷轴")]
        Scroll,
        /// <summary>
        /// 道具分类宝石
        /// </summary>
        [Description("宝石")]
        DarkStone,
        /// <summary>
        /// 道具分类特殊精炼材料
        /// </summary>
        [Description("特殊精炼")]
        RefineSpecial,
        /// <summary>
        /// 道具分类马甲
        /// </summary>
        [Description("马甲")]
        HorseArmour,
        /// <summary>
        /// 道具分类鲜花
        /// </summary>
		[Description("鲜花")]
        Flower,
        /// <summary>
        /// 道具分类宠物粮食
        /// </summary>
        [Description("宠物粮食")]
        CompanionFood,
        /// <summary>
        /// 道具分类宠物背包
        /// </summary>
        [Description("宠物背包")]
        CompanionBag,
        /// <summary>
        /// 道具分类宠物头盔
        /// </summary>
        [Description("宠物头盔")]
        CompanionHead,
        /// <summary>
        /// 道具分类宠物背饰
        /// </summary>
        [Description("宠物背带")]
        CompanionBack,
        /// <summary>
        /// 道具分类系统道具
        /// </summary>
        [Description("系统")]
        System,
        /// <summary>
        /// 道具分类物品碎片
        /// </summary>
        [Description("物品碎片")]
        ItemPart,
        /// <summary>
        /// 道具分类勋章
        /// </summary>
		[Description("徽章")]
        Emblem,
        /// <summary>
        /// 道具分类盾牌
        /// </summary>
		[Description("盾牌")]
        Shield,
        /// <summary>
        /// 道具分类鱼钩
        /// </summary>
        [Description("鱼钩")]
        Hook,
        /// <summary>
        /// 道具分类鱼漂
        /// </summary>
        [Description("鱼漂")]
        Float,
        /// <summary>
        /// 道具分类鱼饵
        /// </summary>
        [Description("鱼饵")]
        Bait,
        /// <summary>
        /// 道具分类探鱼器
        /// </summary>
        [Description("探鱼器")]
        Finder,
        /// <summary>
        /// 道具分类摇轮
        /// </summary>
        [Description("摇轮")]
        Reel,
        /// <summary>
        /// 道具分类鱼类
        /// </summary>
        [Description("鱼")]
        Fish,
        /// <summary>
        /// 以物换物标记
        /// </summary>
        [Description("以物换物标记")]
        Barter,
        /// <summary>
        /// 道具分类声望称号类
        /// </summary>
        [Description("声望称号")]
        FameTitle,
        /// <summary>
        /// 道具分类翅膀
        /// </summary>
        [Description("翅膀")]
        Wing,
        /// <summary>
        /// 道具分类附魔石
        /// </summary>
        [Description("附魔石")]
        Gem,
        /// <summary>
        /// 道具分类宝珠
        /// </summary>
        [Description("宝珠")]
        Orb,
        /// <summary>
        /// 道具分类符文
        /// </summary>
        [Description("符文")]
        Rune,
        /// <summary>
        /// 道具分类腰带
        /// </summary>
        [Description("腰带")]
        Belt,
        /// <summary>
        /// 道具分类开孔材料
        /// </summary>
        [Description("穿孔材料")]
        Drill,
        /// <summary>
        /// 道具分类时装
        /// </summary>
        [Description("时装")]
        Fashion,
        /// <summary>
        /// 道具分类材料
        /// </summary>
        [Description("材料")]
        Material,
        /// <summary>
        /// 道具分类飞镖武器
        /// </summary>
        [Description("飞镖武器")]
        DartWeapon,
        /// <summary>
        /// 道具分类自动药剂
        /// </summary>
        [Description("自动药剂")]
        Medicament,

    }
    /// <summary>
    /// 武器分类
    /// </summary>
    [Lang("武器分类")]
    public enum WeaponType : byte
    {
        [Description("默认")]
        None,
        [Description("单手")]
        OneHands,
        [Description("双手")]
        BothHands,
    }
    /// <summary>
    /// 角色行为
    /// </summary>
    public enum MirAction : byte
    {
        /// <summary>
        /// 站立
        /// </summary>
        [Description("站立")]
        Standing,
        /// <summary>
        /// 移动
        /// </summary>
        [Description("移动")]
        Moving,
        /// <summary>
        /// 推
        /// </summary>
        [Description("推")]
        Pushed,
        /// <summary>
        /// 攻击
        /// </summary>
        [Description("攻击")]
        Attack,
        /// <summary>
        /// 范围攻击
        /// </summary>
        [Description("范围攻击")]
        RangeAttack,
        /// <summary>
        /// 技能
        /// </summary>
        [Description("技能")]
        Spell,
        /// <summary>
        /// 收割
        /// </summary>
        [Description("收割")]
        Harvest,
        /// <summary>
        /// 敲击 被打中
        /// </summary>
        [Description("被攻击后仰")]
        Struck,
        /// <summary>
        /// 死亡消失过程
        /// </summary>
        [Description("死亡消失过程")]
        Die,
        /// <summary>
        /// 死亡地面尸体
        /// </summary>
        [Description("死亡地面尸体")]
        Dead,
        /// <summary>
        /// 出现
        /// </summary>
        [Description("出现")]
        Show,
        /// <summary>
        /// 消失
        /// </summary>
        [Description("消失")]
        Hide,
        /// <summary>
        /// 坐骑
        /// </summary>
        [Description("坐骑")]
        Mount,
        /// <summary>
        /// 挖矿
        /// </summary>
        [Description("挖矿")]
        Mining,
        /// <summary>
        /// 钓鱼抛杆
        /// </summary>
        [Description("钓鱼抛杆")]
        FishingCast,
        /// <summary>
        /// 钓鱼等待
        /// </summary>
        [Description("钓鱼等待")]
        FishingWait,
        /// <summary>
        /// 钓鱼收杆
        /// </summary>
        [Description("钓鱼收杆")]
        FishingReel,
        /// <summary>
        /// 自定义攻击
        /// </summary>
        [Description("自定义攻击")]
        DiyAttack,
        /// <summary>
        /// 自定义技能
        /// </summary>
        [Description("自定义技能")]
        DiySpell,
    }
    /// <summary>
    /// 动作行为动画
    /// </summary>
    public enum MirAnimation : byte
    {
        /// <summary>
        /// 站立
        /// </summary>
        [Description("站立")]
        Standing,
        /// <summary>
        /// 行走
        /// </summary>
        [Description("行走")]
        Walking,
        /// <summary>
        /// 缓慢站立
        /// </summary>
        [Description("缓慢站立")]
        CreepStanding,
        /// <summary>
        /// 缓慢走
        /// </summary>
        [Description("缓慢走")]
        CreepWalkSlow,
        /// <summary>
        /// 缓慢走变快
        /// </summary>
        [Description("缓慢走变快")]
        CreepWalkFast,
        /// <summary>
        /// 跑
        /// </summary>
        [Description("跑")]
        Running,
        /// <summary>
        /// 推
        /// </summary>
        [Description("推")]
        Pushed,
        /// <summary>
        /// 人物动作1 双手攻击
        /// </summary>
        [Description("双手攻击")]
        Combat1,
        /// <summary>
        /// 人物动作2 举手魔法
        /// </summary>
        [Description("人物动作2")]
        Combat2,
        /// <summary>
        /// 攻击魔法3 平A
        /// </summary>
        [Description("人物动作3")]
        Combat3,
        /// <summary>
        /// 人物动作4 十方斩
        /// </summary>
        [Description("人物动作4")]
        Combat4,
        /// <summary>
        /// 人物动作5 翔空剑法
        /// </summary>
        [Description("人物动作5")]
        Combat5,
        /// <summary>
        /// 人物动作6 莲月剑法
        /// </summary>
        [Description("人物动作6")]
        Combat6,
        /// <summary>
        /// 人物动作7 空拳刀法
        /// </summary>
        [Description("人物动作7")]
        Combat7,
        /// <summary>
        /// 人物动作8 野蛮冲撞
        /// </summary>
        [Description("人物动作8")]
        Combat8,
        /// <summary>
        /// 人物动作9 朝拜 作揖
        /// </summary>
        [Description("人物动作9")]
        Combat9,
        /// <summary>
        /// 人物动作10 投掷 打水漂
        /// </summary>
        [Description("人物动作10")]
        Combat10,
        /// <summary>
        /// 人物动作11 
        /// </summary>
        [Description("人物动作11")]
        Combat11,
        [Description("人物动作12")]
        Combat12,
        [Description("人物动作13")]
        Combat13,
        [Description("人物动作14")]
        Combat14,
        [Description("人物动作15")]
        Combat15,
        /// <summary>
        /// 收割 割肉
        /// </summary>
        [Description("收割")]
        Harvest,
        /// <summary>
        /// 站立姿势
        /// </summary>
        [Description("站立姿势")]
        Stance,
        /// <summary>
        /// 被击中 后仰
        /// </summary>
        [Description("被击中")]
        Struck,
        /// <summary>
        /// 死亡消失过程
        /// </summary>
        [Description("死亡消失过程")]
        Die,
        /// <summary>
        /// 死亡地面尸体
        /// </summary>
        [Description("死亡地面尸体")]
        Dead,
        /// <summary>
        /// 其他类死亡
        /// </summary>
        [Description("其他类死亡")]
        Skeleton,
        /// <summary>
        /// 出现
        /// </summary>
        [Description("出现")]
        Show,
        /// <summary>
        /// 消失
        /// </summary>
        [Description("消失")]
        Hide,
        /// <summary>
        /// 骑马站立
        /// </summary>
        [Description("骑马站立")]
        HorseStanding,
        /// <summary>
        /// 骑马行走
        /// </summary>
        [Description("骑马行走")]
        HorseWalking,
        /// <summary>
        /// 骑马跑
        /// </summary>
        [Description("骑马跑")]
        HorseRunning,
        /// <summary>
        /// 骑马撞
        /// </summary>
        [Description("骑马撞")]
        HorseStruck,
        /// <summary>
        /// 石化
        /// </summary>
        [Description("石化")]
        StoneStanding,
        /// <summary>
        /// 狂涛泉涌开始
        /// </summary>
        [Description("狂涛泉涌开始")]
        DragonRepulseStart,
        /// <summary>
        /// 狂涛泉涌过程
        /// </summary>
        [Description("狂涛泉涌过程")]
        DragonRepulseMiddle,
        /// <summary>
        /// 狂涛泉涌结束
        /// </summary>
        [Description("狂涛泉涌结束")]
        DragonRepulseEnd,
        /// <summary>
        /// 钓鱼 抛竿
        /// </summary>
        [Description("钓鱼 抛竿")]
        FishingCast,
        /// <summary>
        /// 钓鱼 等待
        /// </summary>
        [Description("钓鱼 等待")]
        FishingWait,
        /// <summary>
        /// 钓鱼 收线
        /// </summary>
        [Description("钓鱼 收线")]
        FishingReel,
        /// <summary>
        /// 离魂邪风开始
        /// </summary>
        [Description("离魂邪风开始")]
        ChannellingStart,
        /// <summary>
        /// 离魂邪风过程
        /// </summary>
        [Description("离魂邪风过程")]
        ChannellingMiddle,
        /// <summary>
        /// 离魂邪风结束
        /// </summary>
        [Description("离魂邪风结束")]
        ChannellingEnd,
        /// <summary>
        /// 沙巴克开门
        /// </summary>
        [Description("沙巴克开门")]
        OpenDoor,
        /// <summary>
        /// 沙巴克关门
        /// </summary>
        [Description("沙巴克关门")]
        CloseDoor,
    }
    /// <summary>
    /// 聊天信息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 普通聊天
        /// </summary>
        Normal,
        /// <summary>
        /// 区域聊天  !
        /// </summary>
        Shout,
        /// <summary>
        /// 私聊 自己的文字显示 /
        /// </summary>
        WhisperIn,
        /// <summary>
        /// GM私聊  
        /// </summary>
        GMWhisperIn,
        /// <summary>
        /// 私聊 收到方文字显示
        /// </summary>
        WhisperOut,
        /// <summary>
        /// 组队聊天   !!
        /// </summary>
        Group,
        /// <summary>
        /// 世界喊话   !@
        /// </summary>
        Global,
        /// <summary>
        /// 提示信息 
        /// </summary>
        Hint,
        /// <summary>
        /// 系统信息
        /// </summary>
        System,
        /// <summary>
        /// 公告信息
        /// </summary>
        Announcement,
        /// <summary>
        /// 战斗信息提示
        /// </summary>
        Combat,
        /// <summary>
        /// 观察者聊天
        /// </summary>
        ObserverChat,
        /// <summary>
        /// 行会聊天    !~
        /// </summary>
        Guild,
        /// <summary>
        /// 告示显示
        /// </summary>
        Notice,
        /// <summary>
        /// 极品物品提示
        /// </summary>
		ItemTips,
        /// <summary>
        /// boss提示
        /// </summary>
		BossTips,
        /// <summary>
        /// 所有,仅按钮判断用
        /// </summary>
        All,
        /// <summary>
        /// 联盟 TODO
        /// </summary>
        Union,
        /// <summary>
        /// 师徒 TODO
        /// </summary>
        Mentor,
        /// <summary>
        /// 中央滚动告示
        /// </summary>
        RollNotice,
        /// <summary>
        /// 持久提示
        /// </summary>
        DurabilityTips,
        /// <summary>
        /// 道具使用提示
        /// </summary>
        UseItem,
        /// <summary>
        /// 消息动作 复活
        /// </summary>
        Revive,
    }
    /// <summary>
    /// NPC对话类型
    /// </summary>
    public enum NPCDialogType
    {
        /// <summary>
        /// NPC对话类型空置
        /// </summary>
        None,
        /// <summary>
        /// NPC对话类型买
        /// </summary>
        BuySell,
        /// <summary>
        /// NPC对话类型修理
        /// </summary>
        Repair,
        /// <summary>
        /// NPC对话类型精炼
        /// </summary>
        Refine,
        /// <summary>
        /// NPC对话类型高级精炼
        /// </summary>
        RefineRetrieve,
        /// <summary>
        /// NPC对话类型宠物管理
        /// </summary>
        CompanionManage,
        /// <summary>
        /// NPC对话类型结婚
        /// </summary>
        WeddingRing,
        /// <summary>
        /// NPC对话类型精炼石合成
        /// </summary>
        RefinementStone,
        /// <summary>
        /// NPC对话类型大师精炼
        /// </summary>
        MasterRefine,
        /// <summary>
        /// NPC对话类型武器重置
        /// </summary>
        WeaponReset,
        /// <summary>
        /// NPC对话类型道具分解碎片
        /// </summary>
        ItemFragment,
        /// <summary>
        /// NPC对话类型附件定义升级
        /// </summary>
        AccessoryRefineUpgrade,
        /// <summary>
        /// NPC对话类型附件设置等级
        /// </summary>
        AccessoryRefineLevel,
        /// <summary>
        /// NPC对话类型附件设置
        /// </summary>
        AccessoryReset,
        /// <summary>
        /// NPC对话类型武器制作
        /// </summary>
        WeaponCraft,
        /// <summary>
        /// NPC对话类型仓库
        /// </summary>
        Storage,
        /// <summary>
        /// NPC对话类型寄售
        /// </summary>
        MarketSearch,
        /// <summary>
        /// NPC对话类型卖
        /// </summary>
        Sell,
        /// <summary>
        /// NPC对话类型D键菜单功能
        /// </summary>
        DKey,
        /// <summary>
        /// NPC对话类型额外属性加点
        /// </summary>
        Additional,
        /// <summary>
        /// NPC对话类型技能书合成
        /// </summary>
        BookCombine,
        /// <summary>
        /// NPC对话类型镶嵌打卡
        /// </summary>
        Perforation,
        /// <summary>
        /// NPC对话类型镶嵌附魔
        /// </summary>
        Enchanting,
        /// <summary>
        /// NPC对话类型附魔合成
        /// </summary>
        EnchantmentSynthesis,
        /// <summary>
        /// NPC对话类型特殊修理
        /// </summary>
        SpecialRepair,
        /// <summary>
        /// NPC对话类型一键出售
        /// </summary>
        RootSell,
        /// <summary>
        /// 新版武器升级
        /// </summary>
        WeaponUpgrade,
        /// <summary>
        /// 古代墓碑
        /// </summary>
        AncientTombstone,
        /// <summary>
        /// 新版首饰合成
        /// </summary>
        AccessoryCombine,
        /// <summary>
        /// NPC回购道具
        /// </summary>
        BuyBack,
        /// <summary>
        /// NPC元宝寄售
        /// </summary>
        GameGoldMarketSearch,
        /// <summary>
        /// NPC金币交易行
        /// </summary>
        GoldTradingBusiness,
        /// <summary>
        /// NPC拍卖行
        /// </summary>
        Auctions,
        /// <summary>
        /// 账号寄售行
        /// </summary>
        AccountConsignment,
        /// <summary>
        /// 我的寄售
        /// </summary>
        MyMarket,
    }
    /// <summary>
    /// 技能显示属性树定义
    /// </summary>
    [Lang("技能树定义")]
    public enum MagicSchool
    {
        /// <summary>
        /// 技能显示属性定义
        /// 空置
        /// </summary>
        [Description("空置")]
        None,
        /// <summary>
        /// 技能显示属性定义
        /// 武技
        /// </summary>
        [Description("武技")]
        WeaponSkills,
        /// <summary>
        /// 技能显示属性定义
        /// 被动
        /// </summary>
        [Description("被动")]
        Passive,
        /// <summary>
        /// 技能显示属性定义
        /// 转换
        /// </summary>
        [Description("转换")]
        Neutral,
        /// <summary>
        /// 技能显示属性定义
        /// 火
        /// </summary>
        [Description("火（火焰）")]
        Fire,
        /// <summary>
        /// 技能显示属性定义
        /// 冰
        /// </summary>
        [Description("冰（冰寒）")]
        Ice,
        /// <summary>
        /// 技能显示属性定义
        /// 雷
        /// </summary>
        [Description("雷（电击）")]
        Lightning,
        /// <summary>
        /// 技能显示属性定义
        /// 风
        /// </summary>
        [Description("风（风）")]
        Wind,
        /// <summary>
        /// 技能显示属性定义
        /// 神圣
        /// </summary>
        [Description("神圣（神圣）")]
        Holy,
        /// <summary>
        /// 技能显示属性定义
        /// 暗黑
        /// </summary>
        [Description("暗黑（暗黑）")]
        Dark,
        /// <summary>
        /// 技能显示属性定义
        /// 幻影
        /// </summary>
        [Description("幻影（幻影）")]
        Phantom,
        /// <summary>
        /// 技能显示属性定义
        /// 无条件
        /// </summary>
        [Description("无条件")]
        Unconditional,
        /// <summary>
        /// 技能显示属性定义
        /// 格斗
        /// </summary>
        [Description("格斗")]
        Combat,
        /// <summary>
        /// 技能显示属性定义
        /// 刺杀
        /// </summary>
        [Description("刺杀")]
        Assassination,
        /// <summary>
        /// 技能显示属性定义
        /// 暗杀
        /// </summary>
        [Description("暗杀")]
        Assassinatie,
        /// <summary>
        /// 技能显示属性定义
        /// 内功专用
        /// </summary>
        [Description("内功")]
        InternalSkill,
    }
    /// <summary>
    /// 技能说明显示主动被动
    /// </summary>
    [Lang("技能类型说明")]
    public enum MagicAction
    {
        /// <summary>
        /// 技能行为空
        /// </summary>
        [Description("无")]
        None,
        /// <summary>
        /// 主动技能
        /// </summary>
        [Description("主动型")]
        Active,
        /// <summary>
        /// 被动技能
        /// </summary>
        [Description("被动型")]
        Passive,
        /// <summary>
        /// 转换技能
        /// </summary>
        [Description("转换型")]
        Conversion
    }
    /// <summary>
    /// 元素属性
    /// </summary>
    [Lang("元素属性")]
    public enum Element : byte
    {
        /// <summary>
        /// 没有元素属性
        /// </summary>
        [Description("无")]
        None,
        /// <summary>
        /// 火元素
        /// </summary>
        [Description("火（火焰）")]
        Fire,
        /// <summary>
        /// 冰元素
        /// </summary>
        [Description("冰（冰寒）")]
        Ice,
        /// <summary>
        /// 雷元素
        /// </summary>
        [Description("雷（电击）")]
        Lightning,
        /// <summary>
        /// 风元素
        /// </summary>
        [Description("风（风）")]
        Wind,
        /// <summary>
        /// 神圣元素
        /// </summary>
        [Description("神圣（神圣）")]
        Holy,
        /// <summary>
        /// 暗黑元素
        /// </summary>
        [Description("暗黑（暗黑）")]
        Dark,
        /// <summary>
        /// 幻影元素
        /// </summary>
        [Description("幻影（幻影）")]
        Phantom,
    }
    /// <summary>
    /// 魔法技能类型
    /// </summary>
    public enum MagicType
    {
        /// <summary>
        /// 魔法技能类型空
        /// </summary>
        [Description("置空")]
        None,

        #region 战士
        /// <summary>
        /// 基本剑术
        /// </summary>
        [Description("基本剑术")]
        Swordsmanship = 100,

        /// <summary>
        /// 运气术
        /// </summary>
        [Description("运气术")]
        PotionMastery = 101,

        /// <summary>
        /// 攻杀剑术
        /// </summary>
        [Description("攻杀剑术")]
        Slaying = 102,

        /// <summary>
        /// 刺杀剑术
        /// </summary>
        [Description("刺杀剑术")]
        Thrusting = 103,

        /// <summary>
        /// 半月弯刀
        /// </summary>
        [Description("半月弯刀")]
        HalfMoon = 104,

        /// <summary>
        /// 野蛮冲撞
        /// </summary>
        [Description("野蛮冲撞")]
        ShoulderDash = 105,

        /// <summary>
        /// 烈火剑法
        /// </summary>
        [Description("烈火剑法")]
        FlamingSword = 106,

        /// <summary>
        /// 翔空剑法
        /// </summary>
        [Description("翔空剑法")]
        DragonRise = 107,

        /// <summary>
        /// 莲月剑法
        /// </summary>
        [Description("莲月剑法")]
        BladeStorm = 108,

        /// <summary>
        /// 十方斩
        /// </summary>
        [Description("十方斩")]
        DestructiveSurge = 109,

        /// <summary>
        /// 乾坤大挪移
        /// </summary>
        [Description("乾坤大挪移")]
        Interchange = 110,

        /// <summary>
        /// 铁布衫
        /// </summary>
        [Description("铁布衫")]
        Defiance = 111,

        /// <summary>
        /// 斗转星移
        /// </summary>
        [Description("斗转星移")]
        Beckon = 112,

        /// <summary>
        /// 破血狂杀
        /// </summary>
        [Description("破血狂杀")]
        Might = 113,

        /// <summary>
        /// 快刀斩马
        /// </summary>
        [Description("快刀斩马")]
        SwiftBlade = 114,

        /// <summary>
        /// 狂暴冲撞
        /// </summary>
        [Description("狂暴冲撞(无效)")]
        Assault = 115,

        /// <summary>
        /// 金刚之躯
        /// </summary>
        [Description("金刚之躯")]
        Endurance = 116,

        /// <summary>
        /// 移花接木
        /// </summary>
        [Description("移花接木")]
        ReflectDamage = 117,

        /// <summary>
        /// 泰山压顶
        /// </summary>
        [Description("泰山压顶")]
        Fetter = 118,

        /// <summary>
        /// 旋风斩
        /// </summary>
        [Description("旋风斩(无效)")]
        SwirlingBlade = 119,

        /// <summary>
        /// 君临步
        /// </summary>
        [Description("君临步")]
        ReigningStep = 120,

        /// <summary>
        /// 屠龙斩
        /// </summary>
        [Description("屠龙斩(Z版技能)")]
        MaelstromBlade = 121,

        /// <summary>
        /// 高级运气术
        /// </summary>
        [Description("高级运气术(Z版技能)")]
        AdvancedPotionMastery = 122,

        /// <summary>
        /// 挑衅
        /// </summary>
        [Description("挑衅")]
        MassBeckon = 123,

        /// <summary>
        /// 天雷锤
        /// </summary>
        [Description("天雷锤")]
        SeismicSlam = 124,

        /// <summary>
        /// 空破斩
        /// </summary>
        [Description("空破斩")]
        CrushingWave = 125,

        /// <summary>
        /// 无敌
        /// </summary>
        [Description("无敌")]
        Invincibility = 126,

        /// <summary>
        /// 百步神拳
        /// </summary>
        [Description("百步神拳(内功技能未完成)")]
        HundredStepFist = 180,

        /// <summary>
        /// 一击
        /// </summary>
        [Description("一击(内功技能未完成)")]
        ABlow = 181,

        /// <summary>
        /// 泰拳剑
        /// </summary>
        [Description("泰拳剑(内功技能未完成)")]
        ThaiBoxingSword = 182,

        /// <summary>
        /// 火神剑
        /// </summary>
        [Description("火神剑(内功技能未完成)")]
        FireSword = 183,

        #endregion

        #region 法师
        /// <summary>
        /// 火球术
        /// </summary>
        [Description("火球术")]
        FireBall = 201,

        /// <summary>
        /// 霹雳掌
        /// </summary>
        [Description("霹雳掌")]
        LightningBall = 202,

        /// <summary>
        /// 冰月神掌
        /// </summary>
        [Description("冰月神掌")]
        IceBolt = 203,

        /// <summary>
        /// 风掌
        /// </summary>
        [Description("风掌")]
        GustBlast = 204,

        /// <summary>
        /// 抗拒火环
        /// </summary>
        [Description("抗拒火环")]
        Repulsion = 205,

        /// <summary>
        /// 诱惑之光
        /// </summary>
        [Description("诱惑之光")]
        ElectricShock = 206,

        /// <summary>
        /// 瞬息移动
        /// </summary>
        [Description("瞬息移动")]
        Teleportation = 207,

        /// <summary>
        /// 大火球
        /// </summary>
        [Description("大火球")]
        AdamantineFireBall = 208,

        /// <summary>
        /// 雷电术
        /// </summary>
        [Description("雷电术")]
        ThunderBolt = 209,

        /// <summary>
        /// 冰月震天
        /// </summary>
        [Description("冰月震天")]
        IceBlades = 210,

        /// <summary>
        /// 击风
        /// </summary>
        [Description("击风")]
        Cyclone = 211,

        /// <summary>
        /// 地狱火
        /// </summary>
        [Description("地狱火")]
        ScortchedEarth = 212,

        /// <summary>
        /// 疾光电影
        /// </summary>
        [Description("疾光电影")]
        LightningBeam = 213,

        /// <summary>
        /// 冰沙掌
        /// </summary>
        [Description("冰沙掌")]
        FrozenEarth = 214,

        /// <summary>
        /// 风震天
        /// </summary>
        [Description("风震天")]
        BlowEarth = 215,

        /// <summary>
        /// 火墙
        /// </summary>
        [Description("火墙")]
        FireWall = 216,

        /// <summary>
        /// 圣言术
        /// </summary>
        [Description("圣言术")]
        ExpelUndead = 217,

        /// <summary>
        /// 移形换位
        /// </summary>
        [Description("移形换位")]
        GeoManipulation = 218,

        /// <summary>
        /// 魔法盾
        /// </summary>
        [Description("魔法盾")]
        MagicShield = 219,

        /// <summary>
        /// 爆裂火焰
        /// </summary>
        [Description("爆裂火焰")]
        FireStorm = 220,

        /// <summary>
        /// 地狱雷光
        /// </summary>
        [Description("地狱雷光")]
        LightningWave = 221,

        /// <summary>
        /// 冰咆哮
        /// </summary>
        [Description("冰咆哮")]
        IceStorm = 222,

        /// <summary>
        /// 龙卷风
        /// </summary>
        [Description("龙卷风")]
        DragonTornado = 223,

        /// <summary>
        /// 魄冰刺
        /// </summary>
        [Description("魄冰刺")]
        GreaterFrozenEarth = 224,

        /// <summary>
        /// 怒神霹雳
        /// </summary>
        [Description("怒神霹雳")]
        ChainLightning = 225,

        /// <summary>
        /// 焰天火雨
        /// </summary>
        [Description("焰天火雨")]
        MeteorShower = 226,

        /// <summary>
        /// 凝血离魂
        /// </summary>
        [Description("凝血离魂")]
        Renounce = 227,

        /// <summary>
        /// 旋风墙
        /// </summary>
        [Description("旋风墙")]
        Tempest = 228,

        /// <summary>
        /// 天打雷劈
        /// </summary>
        [Description("天打雷劈")]
        JudgementOfHeaven = 229,

        /// <summary>
        /// 电闪雷鸣
        /// </summary>
        [Description("电闪雷鸣")]
        ThunderStrike = 230,

        /// <summary>
        /// 透心链
        /// </summary>
        [Description("透心链(无效)")]
        RayOfLight = 231,

        /// <summary>
        /// 混元掌
        /// </summary>
        [Description("混元掌(无效)")]
        BurstOfEnergy = 232,

        /// <summary>
        /// 魔光盾
        /// </summary>
        [Description("魔光盾(无效)")]
        ShieldOfPreservation = 233,

        /// <summary>
        /// 焚魂魔功
        /// </summary>
        [Description("焚魂魔功(无效)")]
        RetrogressionOfEnergy = 234,

        /// <summary>
        /// 魔爆术
        /// </summary>
        [Description("魔爆术(无效)")]
        FuryBlast = 235,

        /// <summary>
        /// 地狱魔焰
        /// </summary>
        [Description("地狱魔焰(无效)")]
        TempestOfUnstableEnergy = 236,

        /// <summary>
        /// 分身术
        /// </summary>
        [Description("分身术")]
        MirrorImage = 237,

        /// <summary>
        /// 高级凝血离魂
        /// </summary>
        [Description("高级凝血离魂(Z版技能)")]
        AdvancedRenounce = 238,

        /// <summary>
        /// 护身冰环
        /// </summary>
        [Description("护身冰环")]
        FrostBite = 239,

        /// <summary>
        /// 天之怒火
        /// </summary>
        [Description("天之怒火")]
        Asteroid = 240,

        /// <summary>
        /// 离魂邪风
        /// </summary>
        [Description("离魂邪风")]
        ElementalHurricane = 241,

        /// <summary>
        /// 护身法盾
        /// </summary>
        [Description("护身法盾")]
        SuperiorMagicShield = 242,

        /// <summary>
        /// 冰雨
        /// </summary>
        [Description("冰雨")]
        IceRain = 243,

        /// <summary>
        /// 冰魂神掌
        /// </summary>
        [Description("冰魂神掌(内功技能未完成)")]
        IceSoulPalm = 280,

        /// <summary>
        /// 雪龙波
        /// </summary>
        [Description("雪龙波(内功技能未完成)")]
        Chevron = 281,

        /// <summary>
        /// 冰旋波
        /// </summary>
        [Description("冰旋波(内功技能未完成)")]
        IceSwirlingWave = 282,

        /// <summary>
        /// 龙花旋
        /// </summary>
        [Description("龙花旋(内功技能未完成)")]
        DragonFlowerSpin = 283,

        #endregion

        #region 道士
        /// <summary>
        /// 治愈术
        /// </summary>
        [Description("治愈术")]
        Heal = 300,

        /// <summary>
        /// 精神力战法
        /// </summary>
        [Description("精神力战法")]
        SpiritSword = 301,

        /// <summary>
        /// 施毒术
        /// </summary>
        [Description("施毒术")]
        PoisonDust = 302,

        /// <summary>
        /// 灵魂火符
        /// </summary>
        [Description("灵魂火符")]
        ExplosiveTalisman = 303,

        /// <summary>
        /// 月魂断玉
        /// </summary>
        [Description("月魂断玉")]
        EvilSlayer = 304,

        /// <summary>
        /// 隐身术
        /// </summary>
        [Description("隐身术")]
        Invisibility = 305,

        /// <summary>
        /// 幽灵盾
        /// </summary>
        [Description("幽灵盾")]
        MagicResistance = 306,

        /// <summary>
        /// 集体隐身术
        /// </summary>
        [Description("集体隐身术")]
        MassInvisibility = 307,

        /// <summary>
        /// 月魂灵波
        /// </summary>
        [Description("月魂灵波")]
        GreaterEvilSlayer = 308,

        /// <summary>
        /// 神圣战甲术
        /// </summary>
        [Description("神圣战甲术")]
        Resilience = 309,

        /// <summary>
        /// 困魔咒
        /// </summary>
        [Description("困魔咒")]
        TrapOctagon = 310,

        /// <summary>
        /// 空拳刀法
        /// </summary>
        [Description("空拳刀法")]
        TaoistCombatKick = 311,

        /// <summary>
        /// 强魔震法
        /// </summary>
        [Description("强魔震法")]
        ElementalSuperiority = 312,

        /// <summary>
        /// 群体治愈术
        /// </summary>
        [Description("群体治愈术")]
        MassHeal = 313,

        /// <summary>
        /// 猛虎强势
        /// </summary>
        [Description("猛虎强势")]
        BloodLust = 314,

        /// <summary>
        /// 回生术
        /// </summary>
        [Description("回生术")]
        Resurrection = 315,

        /// <summary>
        /// 云寂术
        /// </summary>
        [Description("云寂术")]
        Purification = 316,

        /// <summary>
        /// 妙影无踪
        /// </summary>
        [Description("妙影无踪")]
        Transparency = 317,

        /// <summary>
        /// 阴阳法环
        /// </summary>
        [Description("阴阳法环")]
        CelestialLight = 318,

        /// <summary>
        /// 养生术
        /// </summary>
        [Description("养生术")]
        EmpoweredHealing = 319,

        /// <summary>
        /// 吸星大法
        /// </summary>
        [Description("吸星大法")]
        LifeSteal = 320,

        /// <summary>
        /// 灭魂火符
        /// </summary>
        [Description("灭魂火符")]
        ImprovedExplosiveTalisman = 321,

        /// <summary>
        /// 施毒大法
        /// </summary>
        [Description("施毒大法")]
        GreaterPoisonDust = 322,

        /// <summary>
        /// 迷魂大法
        /// </summary>
        [Description("迷魂大法(未完成)")]
        Scarecrow = 323,

        /// <summary>
        /// 横扫千军
        /// </summary>
        [Description("横扫千军(未完成)")]
        ThunderKick = 324,

        /// <summary>
        /// 神灵守护
        /// </summary>
        [Description("神灵守护(未完成)")]
        DragonBreath = 325,

        /// <summary>
        /// 隐魂术
        /// </summary>
        [Description("隐魂术(Z版技能)")]
        MassTransparency = 326,

        /// <summary>
        /// 月明波
        /// </summary>
        [Description("月明波(Z版技能)")]
        GreaterHolyStrike = 327,

        /// <summary>
        /// 群体灵魂火符
        /// </summary>
        [Description("群体灵魂火符(Z版技能)")]
        AugmentExplosiveTalisman = 328,

        /// <summary>
        /// 群体月魂灵波
        /// </summary>
        [Description("群体月魂灵波(Z版技能)")]
        AugmentEvilSlayer = 329,

        /// <summary>
        /// 强化云寂术
        /// </summary>
        [Description("强化云寂术(Z版技能)")]
        AugmentPurification = 330,

        /// <summary>
        /// 强化回生术
        /// </summary>
        [Description("强化回生术(Z版技能)")]
        OathOfThePerished = 331,

        /// <summary>
        /// 召唤骷髅
        /// </summary>
        [Description("召唤骷髅")]
        SummonSkeleton = 332,

        /// <summary>
        /// 召唤神兽
        /// </summary>
        [Description("召唤神兽")]
        SummonShinsu = 333,

        /// <summary>
        /// 超强召唤骷髅
        /// </summary>
        [Description("超强召唤骷髅")]
        SummonJinSkeleton = 334,

        /// <summary>
        /// 移花接玉
        /// </summary>
        [Description("移花接玉")]
        StrengthOfFaith = 335,

        /// <summary>
        /// 焰魔召唤术
        /// </summary>
        [Description("焰魔召唤术")]
        SummonDemonicCreature = 336,

        /// <summary>
        /// 魔焰强解术
        /// </summary>
        [Description("魔焰强解术")]
        DemonExplosion = 337,

        /// <summary>
        /// 传染
        /// </summary>
        [Description("传染")]
        Infection = 338,

        /// <summary>
        /// 地狱回疗
        /// </summary>
        [Description("地狱回疗(Z版技能)")]
        DemonicRecovery = 339,

        /// <summary>
        /// 虚弱化
        /// </summary>
        [Description("虚弱化")]
        Neutralize = 340,

        /// <summary>
        /// 强化虚弱化
        /// </summary>
        [Description("强化虚弱化(Z版技能)")]
        AugmentNeutralize = 341,

        /// <summary>
        /// 暗鬼阵
        /// </summary>
        [Description("暗鬼阵")]
        DarkSoulPrison = 342,

        /// <summary>
        /// 缚敌诀符
        /// </summary>
        [Description("缚敌诀符(内功技能未完成)")]
        BindTheEnemy = 380,

        /// <summary>
        /// 雷电
        /// </summary>
        [Description("雷电(内功技能未完成)")]
        LightningTheEnemy = 381,

        /// <summary>
        /// 灭天雷
        /// </summary>
        [Description("灭天雷(内功技能未完成)")]
        Mietianlei = 382,

        /// <summary>
        /// 毒荫云
        /// </summary>
        [Description("毒荫云(内功技能未完成)")]
        PoisonousShadowCloud = 383,

        #endregion

        #region 刺客
        /// <summary>
        /// 垂柳舞
        /// </summary>
        [Description("垂柳舞")]
        WillowDance = 401,

        /// <summary>
        /// 蔓藤舞
        /// </summary>
        [Description("蔓藤舞")]
        VineTreeDance = 402,

        /// <summary>
        /// 磨炼
        /// </summary>
        [Description("磨炼")]
        Discipline = 403,

        /// <summary>
        /// 毒云
        /// </summary>
        [Description("毒云")]
        PoisonousCloud = 404,

        /// <summary>
        /// 盛开
        /// </summary>
        [Description("盛开")]
        FullBloom = 405,

        /// <summary>
        /// 潜行
        /// </summary>
        [Description("潜行")]
        Cloak = 406,

        /// <summary>
        /// 白莲
        /// </summary>
        [Description("白莲")]
        WhiteLotus = 407,

        /// <summary>
        /// 满月恶狼
        /// </summary>
        [Description("满月恶狼")]
        CalamityOfFullMoon = 408,

        /// <summary>
        /// 亡灵束缚
        /// </summary>
        [Description("亡灵束缚")]
        WraithGrip = 409,

        /// <summary>
        /// 红莲
        /// </summary>
        [Description("红莲")]
        RedLotus = 410,

        /// <summary>
        /// 烈焰
        /// </summary>
        [Description("烈焰")]
        HellFire = 411,

        /// <summary>
        /// 血禅
        /// </summary>
        [Description("血禅")]
        PledgeOfBlood = 412,

        /// <summary>
        /// 血之盟约
        /// </summary>
        [Description("血之盟约")]
        Rake = 413,

        /// <summary>
        /// 月季
        /// </summary>
        [Description("月季")]
        SweetBrier = 414,

        /// <summary>
        /// 亡灵替身
        /// </summary>
        [Description("亡灵替身")]
        SummonPuppet = 415,

        /// <summary>
        /// 孽报
        /// </summary>
        [Description("孽报")]
        Karma = 416,

        /// <summary>
        /// 亡灵之手
        /// </summary>
        [Description("亡灵之手")]
        TouchOfTheDeparted = 417,

        /// <summary>
        /// 残月之乱
        /// </summary>
        [Description("残月之乱")]
        WaningMoon = 418,

        /// <summary>
        /// 鬼灵步
        /// </summary>
        [Description("鬼灵步")]
        GhostWalk = 419,

        /// <summary>
        /// 神机妙算
        /// </summary>
        [Description("神机妙算")]
        ElementalPuppet = 420,

        /// <summary>
        /// 深渊
        /// </summary>
        [Description("深渊")]
        Rejuvenation = 421,

        /// <summary>
        /// 决意
        /// </summary>
        [Description("决意(未完成)")]
        Resolution = 422,

        /// <summary>
        /// 切换
        /// </summary>
        [Description("切换(未完成)")]
        ChangeOfSeasons = 423,

        /// <summary>
        /// 解放
        /// </summary>
        [Description("解放")]
        Release = 424,

        /// <summary>
        /// 新月爆炎龙
        /// </summary>
        [Description("新月爆炎龙")]
        FlameSplash = 425,

        /// <summary>
        /// 百花盛开
        /// </summary>
        [Description("百花盛开(Z版技能)")]
        BloodyFlower = 426,

        /// <summary>
        /// 心机一转
        /// </summary>
        [Description("心机一转")]
        TheNewBeginning = 427,

        /// <summary>
        /// 鹰击
        /// </summary>
        [Description("鹰击")]
        DanceOfSwallow = 428,

        /// <summary>
        /// 黄泉旅者
        /// </summary>
        [Description("黄泉旅者")]
        DarkConversion = 429,

        /// <summary>
        /// 狂涛涌泉
        /// </summary>
        [Description("狂涛涌泉")]
        DragonRepulse = 430,

        /// <summary>
        /// 修罗降临
        /// </summary>
        [Description("修罗降临")]
        AdventOfDemon = 431,

        /// <summary>
        /// 罗刹降临
        /// </summary>
        [Description("罗刹降临")]
        AdventOfDevil = 432,

        /// <summary>
        /// 深渊苦海
        /// </summary>
        [Description("深渊苦海")]
        Abyss = 433,

        /// <summary>
        /// 日闪
        /// </summary>
        [Description("日闪")]
        FlashOfLight = 434,

        /// <summary>
        /// 隐沦
        /// </summary>
        [Description("隐沦(未完成)")]
        Stealth = 435,

        /// <summary>
        /// 风之闪避
        /// </summary>
        [Description("风之闪避")]
        Evasion = 436,

        /// <summary>
        /// 风之守护
        /// </summary>
        [Description("风之守护")]
        RagingWind = 437,

        /// <summary>
        /// 高级百花盛开
        /// </summary>
        [Description("高级百花盛开(Z版技能)")]
        AdvancedBloodyFlower = 438,

        /// <summary>
        /// 最后抵抗
        /// </summary>
        [Description("最后抵抗")]
        Massacre = 439,

        /// <summary>
        /// 暗影艺术
        /// </summary>
        [Description("暗影艺术")]
        ArtOfShadows = 440,
        /// <summary>
        /// 集中
        /// </summary>
        [Description("集中")]
        Concentration = 441,
        /// <summary>
        /// 业火
        /// </summary>
        [Description("业火")]
        SwordOfVengeance = 442,
        /// <summary>
        /// 千刃杀风
        /// </summary>
        [Description("千刃杀风")]
        ThousandBlades = 443,

        /// <summary>
        /// 奥幻杀
        /// </summary>
        [Description("奥幻杀(内功技能未完成)")]
        ArcaneKill = 480,

        /// <summary>
        /// 破碎
        /// </summary>
        [Description("破碎(内功技能未完成)")]
        Fracture = 481,

        /// <summary>
        /// 四华轮
        /// </summary>
        [Description("四华轮(内功技能未完成)")]
        SihuaWheel = 482,

        /// <summary>
        /// 残月轮
        /// </summary>
        [Description("残月轮(内功技能未完成)")]
        WaningMoonWheel = 483,

        /// <summary>
        /// 血火
        /// </summary>
        [Description("血火")]
        BloodFire = 484,

        #endregion

        #region  怪物
        /// <summary>
        /// 怪物烧焦地面
        /// </summary>
        [Description("怪物烧焦地面")]
        MonsterScortchedEarth = 501,
        /// <summary>
        /// 怪物冰咆哮
        /// </summary>
        [Description("怪物冰咆哮")]
        MonsterIceStorm = 502,
        /// <summary>
        /// 怪物死亡毒云
        /// </summary>
        [Description("怪物死亡毒云")]
        MonsterDeathCloud = 503,
        /// <summary>
        /// 怪物闪电风暴
        /// </summary>
        [Description("怪物闪电风暴")]
        MonsterThunderStorm = 504,
        /// <summary>
        /// 火系攻击特效
        /// </summary>
        [Description("火系攻击特效")]
        SamaGuardianFire = 505,
        /// <summary>
        /// 冰系攻击特效
        /// </summary>
        [Description("冰系攻击特效")]
        SamaGuardianIce = 506,
        /// <summary>
        /// 雷系攻击特效
        /// </summary>
        [Description("雷系攻击特效")]
        SamaGuardianLightning = 507,
        /// <summary>
        /// 风系攻击特效
        /// </summary>
        [Description("风系攻击特效")]
        SamaGuardianWind = 508,
        /// <summary>
        /// 朱雀天王火攻击特效
        /// </summary>
        [Description("朱雀天王火攻击特效")]
        SamaPhoenixFire = 509,
        /// <summary>
        /// 玄武天王冰攻击特效
        /// </summary>
        [Description("玄武天王冰攻击特效")]
        SamaBlackIce = 510,
        /// <summary>
        /// 青龙天王雷攻击特效
        /// </summary>
        [Description("青龙天王雷攻击特效")]
        SamaBlueLightning = 511,
        /// <summary>
        /// 白虎天王风攻击特效
        /// </summary>
        [Description("白虎天王风攻击特效")]
        SamaWhiteWind = 512,
        /// <summary>
        /// 魔灵神主火攻击特效
        /// </summary>
        [Description("魔灵神主火攻击特效")]
        SamaProphetFire = 513,
        /// <summary>
        /// 魔灵神主雷攻击特效
        /// </summary>
        [Description("魔灵神主雷攻击特效")]
        SamaProphetLightning = 514,
        /// <summary>
        /// 魔灵神主风攻击特效
        /// </summary>
        [Description("魔灵神主风攻击特效")]
        SamaProphetWind = 515,
        /// <summary>
        /// 大龙虾左夹攻击特效
        /// </summary>
        [Description("大龙虾左夹攻击特效")]
        DoomClawLeftPinch = 520,
        /// <summary>
        /// 大龙虾左扫攻击特效
        /// </summary>
        [Description("大龙虾左扫攻击特效")]
        DoomClawLeftSwipe = 521,
        /// <summary>
        /// 大龙虾右夹攻击特效
        /// </summary>
        [Description("大龙虾右夹攻击特效")]
        DoomClawRightPinch = 522,
        /// <summary>
        /// 大龙虾右扫攻击特效
        /// </summary>
        [Description("大龙虾右扫攻击特效")]
        DoomClawRightSwipe = 523,
        /// <summary>
        /// 大龙虾波浪攻击特效
        /// </summary>
        [Description("大龙虾波浪攻击特效")]
        DoomClawWave = 524,
        /// <summary>
        /// 大龙虾吐泡攻击特效
        /// </summary>
        [Description("大龙虾吐泡攻击特效")]
        DoomClawSpit = 525,
        /// <summary>
        /// 水晶火虫红色火球攻击特效
        /// </summary>
        [Description("水晶火虫红色火球攻击特效")]
        PinkFireBall = 530,
        /// <summary>
        /// 魔道道士绿毒球攻击特效
        /// </summary>
        [Description("魔道道士绿毒球攻击特效")]
        GreenSludgeBall = 540,

        /// <summary>
        /// 六角魔兽地狱使者蝙蝠
        /// </summary>
        [Description("六角魔兽地狱使者蝙蝠")]
        HellBringerBats = 550,
        /// <summary>
        /// 斩决鬼喷毒
        /// </summary>
        [Description("斩决鬼喷毒")]
        PoisonousGolemLineAoE = 551,
        /// <summary>
        /// 火冥鸢焦土
        /// </summary>
        [Description("火冥鸢焦土")]
        IgyuScorchedEarth = 552,
        /// <summary>
        /// 火冥鸢旋转火旋风攻击
        /// </summary>
        [Description("火冥鸢旋转火旋风攻击")]
        IgyuCyclone = 553,
        /// <summary>
        /// 投石车抛石攻击地面效果 用于粒子
        /// </summary>
        [Description("投石车抛石攻击地面效果")]
        WarWeaponShell = 554,

        #endregion

    }

    /// <summary>
    /// 副本地图类型
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// 地图类型通用
        /// </summary>
        System,
        /// <summary>
        /// 地图类型单人
        /// </summary>
        Single,
        /// <summary>
        /// 地图类型组队
        /// </summary>
        Group,
    }
    /// <summary>
    /// 加载地图类型
    /// </summary>
    public enum LoadMapType : byte
    {
        /// <summary>
        /// 黑龙特定地图类型
        /// </summary>
        BlackDragonMir3Map,
        /// <summary>
        /// 其他地图类型
        /// </summary>
        Others
    }

    /// <summary>
    /// 怪物图像定义
    /// </summary>
    public enum MonsterImage
    {
        /// <summary>
        /// 怪物图像为空
        /// </summary>
        [Description("置空")]
        None,

        /// <summary>
        /// 卫士
        /// </summary>
        [Description("Mon3-6 卫士")]
        Guard,

        /// <summary>
        /// 鸡
        /// </summary>
        [Description("Mon3-0 鸡")]
        Chicken,
        /// <summary>
        /// 猪
        /// </summary>
        [Description("Mon12-9 猪")]
        Pig,
        /// <summary>
        /// 鹿
        /// </summary>
        [Description("Mon3-1 鹿")]
        Deer,
        /// <summary>
        /// 牛
        /// </summary>
        [Description("Mon13-1 牛")]
        Cow,
        /// <summary>
        /// 羊
        /// </summary>
        [Description("Mon6-8 羊")]
        Sheep,
        /// <summary>
        /// 多钩猫
        /// </summary>
        [Description("Mon4-8 多钩猫")]
        ClawCat,
        /// <summary>
        /// 狼
        /// </summary>
        [Description("Mon7-5 狼")]
        Wolf,
        /// <summary>
        /// 森林雪人
        /// </summary>
        [Description("Mon4-0 森林雪人")]
        ForestYeti,
        /// <summary>
        /// 栗子树
        /// </summary>
        [Description("Mon13-7 栗子树")]
        ChestnutTree,
        /// <summary>
        /// 食人花
        /// </summary>
        [Description("Mon4-1 食人花")]
        CarnivorousPlant,
        /// <summary>
        /// 半兽战士
        /// </summary>
        [Description("Mon3-3 半兽战士")]
        Oma,
        /// <summary>
        /// 虎蛇
        /// </summary>
        [Description("Mon6-7 虎蛇")]
        TigerSnake,
        /// <summary>
        /// 毒蜘蛛
        /// </summary>
        [Description("Mon3-5 毒蜘蛛")]
        SpittingSpider,
        /// <summary>
        /// 稻草人
        /// </summary>
        [Description("Mon5-0 稻草人")]
        Scarecrow,
        /// <summary>
        /// 半兽勇士
        /// </summary>
        [Description("Mon3-4 半兽勇士")]
        OmaHero,
        /// <summary>
        /// 山洞蝙蝠
        /// </summary>
        [Description("Mon3-9 山洞蝙蝠")]
        CaveBat,
        /// <summary>
        /// 蝎子
        /// </summary>
        [Description("Mon3-8 蝎子")]
        Scorpion,
        /// <summary>
        /// 骷髅
        /// </summary>
        [Description("Mon4-2 骷髅")]
        Skeleton,
        /// <summary>
        /// 骷髅战士
        /// </summary>
        [Description("Mon4-4 骷髅战士")]
        SkeletonAxeMan,
        /// <summary>
        /// 掷斧骷髅
        /// </summary>
        [Description("Mon4-3 掷斧骷髅")]
        SkeletonAxeThrower,
        /// <summary>
        /// 骷髅战将
        /// </summary>
        [Description("Mon4-5 骷髅战将")]
        SkeletonWarrior,
        /// <summary>
        /// 骷髅精灵
        /// </summary>
        [Description("Mon4-6 骷髅精灵")]
        SkeletonLord,
        /// <summary>
        /// 洞蛆
        /// </summary>
        [Description("Mon4-7 洞蛆")]
        CaveMaggot,
        /// <summary>
        /// 老道僵尸
        /// </summary>
        [Description("Mon5-8 老道僵尸")]
        GhostSorcerer,
        /// <summary>
        /// 法鬼
        /// </summary>
        [Description("Mon5-9 法鬼")]
        GhostMage,
        /// <summary>
        /// 牛鬼
        /// </summary>
        [Description("Mon6-0 牛鬼")]
        VoraciousGhost,
        /// <summary>
        /// 食鬼
        /// </summary>
        [Description("Mon6-1 食鬼")]
        DevouringGhost,
        /// <summary>
        /// 尸鬼
        /// </summary>
        [Description("Mon6-2 尸鬼")]
        CorpseRaisingGhost,
        /// <summary>
        /// 尸王
        /// </summary>
        [Description("Mon6-3 尸王")]
        GhoulChampion,
        /// <summary>
        /// 盔甲蚂蚁
        /// </summary>
        [Description("Mon1-8 盔甲蚂蚁")]
        ArmoredAnt,
        /// <summary>
        /// 蚂蚁战士
        /// </summary>
        [Description("Mon2-4 蚂蚁战士")]
        AntSoldier,
        /// <summary>
        /// 蚂蚁道士
        /// </summary>
        [Description("Mon1-7 蚂蚁道士")]
        AntHealer,
        /// <summary>
        /// 爆毒蚂蚁
        /// </summary>
        [Description("Mon10-6 爆毒蚂蚁")]
        AntNeedler,
        /// <summary>
        /// 盔甲虫
        /// </summary>
        [Description("Mon7-0 盔甲虫")]
        ShellNipper,
        /// <summary>
        /// 多角虫
        /// </summary>
        [Description("Mon7-3 多角虫")]
        Beetle,
        /// <summary>
        /// 威思尔小虫
        /// </summary>
        [Description("Mon7-1 威思尔小虫")]
        VisceralWorm,
        /// <summary>
        /// 多脚虫
        /// </summary>
        [Description("Mon15-5 多脚虫")]
        MutantFlea,
        /// <summary>
        /// 蜘蛛蛙
        /// </summary>
        [Description("Mon15-9 蜘蛛蛙")]
        PoisonousMutantFlea,
        /// <summary>
        /// 胞眼虫
        /// </summary>
        [Description("Mon15-7 胞眼虫")]
        BlasterMutantFlea,
        /// <summary>
        /// 跳跳蜂
        /// </summary>
        [Description("Mon8-1 跳跳蜂")]
        WasHatchling,
        /// <summary>
        /// 蜈蚣
        /// </summary>
        [Description("Mon7-6 蜈蚣")]
        Centipede,
        /// <summary>
        /// 蝴蝶虫
        /// </summary>
        [Description("Mon8-2 蝴蝶虫")]
        ButterflyWorm,
        /// <summary>
        /// 黑色恶蛆
        /// </summary>
        [Description("Mon7-8 黑色恶蛆")]
        MutantMaggot,
        /// <summary>
        /// 钳虫
        /// </summary>
        [Description("Mon7-9 钳虫")]
        Earwig,
        /// <summary>
        /// 邪恶钳虫
        /// </summary>
        [Description("Mon8-0 邪恶钳虫")]
        IronLance,
        /// <summary>
        /// 触龙神
        /// </summary>
        [Description("Mon7-7 触龙神")]
        LordNiJae,
        /// <summary>
        /// 浪子人鬼
        /// </summary>
        [Description("Mon14-8 浪子人鬼")]
        RottingGhoul,
        /// <summary>
        /// 腐蚀人鬼
        /// </summary>
        [Description("Mon14-2 腐蚀人鬼")]
        DecayingGhoul,
        /// <summary>
        /// 吸血鬼
        /// </summary>
        [Description("Mon5-2 吸血鬼")]
        BloodThirstyGhoul,
        /// <summary>
        /// 暗黑战士
        /// </summary>
        [Description("Mon5-6 暗黑战士")]
        SpinedDarkLizard,
        /// <summary>
        /// 沃玛战士
        /// </summary>
        [Description("Mon5-1 沃玛战士")]
        UmaInfidel,
        /// <summary>
        /// 火焰沃玛
        /// </summary>
        [Description("Mon5-3 火焰沃玛")]
        UmaFlameThrower,
        /// <summary>
        /// 沃玛卫士
        /// </summary>
        [Description("Mon5-4 沃玛卫士")]
        UmaAnguisher,
        /// <summary>
        /// 沃玛教主
        /// </summary>
        [Description("Mon5-5 沃玛教主")]
        UmaKing,
        /// <summary>
        /// 月魔蜘蛛
        /// </summary>
        [Description("Mon11-1 月魔蜘蛛")]
        SpiderBat,
        /// <summary>
        /// 幻影蜘蛛
        /// </summary>
        [Description("Mon11-6 幻影蜘蛛")]
        ArachnidGazer,
        /// <summary>
        /// 爆裂蜘蛛
        /// </summary>
        [Description("Mon11-5 爆裂蜘蛛")]
        Larva,
        /// <summary>
        /// 血巨人
        /// </summary>
        [Description("Mon11-7 血巨人")]
        RedMoonGuardian,
        /// <summary>
        /// 血金刚
        /// </summary>
        [Description("Mon11-8 血金刚")]
        RedMoonProtector,
        /// <summary>
        /// 花色蜘蛛
        /// </summary>
        [Description("Mon12-1 花色蜘蛛")]
        VenomousArachnid,
        /// <summary>
        /// 黑角蜘蛛
        /// </summary>
        [Description("Mon12-2黑角蜘蛛")]
        DarkArachnid,
        /// <summary>
        /// 赤月恶魔
        /// </summary>
        [Description("Mon11-4 赤月恶魔")]
        RedMoonTheFallen,
        /// <summary>
        /// 祖玛弓箭手
        /// </summary>
        [Description("Mon9-2 祖玛弓箭手")]
        ZumaSharpShooter,
        /// <summary>
        /// 祖玛雕像
        /// </summary>
        [Description("Mon9-3 祖玛雕像")]
        ZumaFanatic,
        /// <summary>
        /// 祖玛卫士
        /// </summary>
        [Description("Mon9-4 祖玛卫士")]
        ZumaGuardian,
        /// <summary>
        /// 大老鼠
        /// </summary>
        [Description("Mon9-1 大老鼠")]
        ViciousRat,
        /// <summary>
        /// 祖玛教主
        /// </summary>
        [Description("Mon9-5 祖玛教主")]
        ZumaKing,
        /// <summary>
        /// 东魔神怪
        /// </summary>
        [Description("Mon16-7 东魔神怪")]
        EvilFanatic,
        /// <summary>
        /// 猿猴战士
        /// </summary>
        [Description("Mon16-4 猿猴战士")]
        Monkey,
        /// <summary>
        /// 巨象兽
        /// </summary>
        [Description("Mon16-8 巨象兽")]
        EvilElephant,
        /// <summary>
        /// 西魔神怪
        /// </summary>
        [Description("Mon16-6 西魔神怪")]
        CannibalFanatic,
        /// <summary>
        /// 巨型多角虫
        /// </summary>
        [Description("Mon7-4 巨型多角虫")]
        SpikedBeetle,
        /// <summary>
        /// 诺玛
        /// </summary>
        [Description("Mon13-8 诺玛")]
        NumaGrunt,
        /// <summary>
        /// 诺玛法老
        /// </summary>
        [Description("Mon2-3 诺玛法老")]
        NumaMage,
        /// <summary>
        /// 诺玛将士
        /// </summary>
        [Description("Mon2-7 诺玛将士")]
        NumaElite,
        /// <summary>
        /// 沙漠鱼魔
        /// </summary>
        [Description("Mon10-4 沙漠鱼魔")]
        SandShark,
        /// <summary>
        /// 沙漠石人
        /// </summary>
        [Description("Mon1-4 沙漠石人")]
        StoneGolem,
        /// <summary>
        /// 沙漠风魔
        /// </summary>
        [Description("Mon10-7 沙漠风魔")]
        WindfurySorceress,
        /// <summary>
        /// 沙漠树魔
        /// </summary>
        [Description("Mon10-7 沙漠树魔")]
        CursedCactus,
        /// <summary>
        /// 异界之门
        /// </summary>
        [Description("Mon1-5 异界之门")]
        NetherWorldGate,
        /// <summary>
        /// 变异迅猛蜥
        /// </summary>
        [Description("Mon20-1 变异迅猛蜥")]
        RagingLizard,
        /// <summary>
        /// 变异刺骨蜥
        /// </summary>
        [Description("Mon20-2 变异刺骨蜥")]
        SawToothLizard,
        /// <summary>
        /// 变异丑蜥
        /// </summary>
        [Description("Mon20-3 变异丑蜥")]
        MutantLizard,
        /// <summary>
        /// 变异毒蜥
        /// </summary>
        [Description("Mon20-4 变异毒蜥")]
        VenomSpitter,
        /// <summary>
        /// 魔石咆哮者
        /// </summary>
        [Description("Mon20-5 魔石咆哮者")]
        SonicLizard,
        /// <summary>
        /// 魔石狂热者
        /// </summary>
        [Description("Mon20-6 魔石狂热者")]
        GiantLizard,
        /// <summary>
        /// 变异利爪蜥
        /// </summary>
        [Description("Mon20-9 变异利爪蜥")]
        CrazedLizard,
        /// <summary>
        /// 魔石守护神
        /// </summary>
        [Description("Mon20-7 魔石守护神")]
        TaintedTerror,
        /// <summary>
        /// 地天灭王
        /// </summary>
        [Description("Mon20-8 地天灭王")]
        DeathLordJichon,
        /// <summary>
        /// 潘夜战士
        /// </summary>
        [Description("Mon14-7 潘夜战士")]
        Minotaur,
        /// <summary>
        /// 潘夜冰魔
        /// </summary>
        [Description("Mon14-3 潘夜冰魔")]
        FrostMinotaur,
        /// <summary>
        /// 潘夜云魔
        /// </summary>
        [Description("Mon14-4 潘夜云魔")]
        ShockMinotaur,
        /// <summary>
        /// 潘夜火魔
        /// </summary>
        [Description("Mon14-6 潘夜火魔")]
        FlameMinotaur,
        /// <summary>
        /// 潘夜风魔
        /// </summary>
        [Description("Mon14-5 潘夜风魔")]
        FuryMinotaur,
        /// <summary>
        /// 潘夜左护卫
        /// </summary>
        [Description("Mon14-1 潘夜左护卫")]
        BanyaLeftGuard,
        /// <summary>
        /// 潘夜右护卫
        /// </summary>
        [Description("Mon14-0 潘夜右护卫")]
        BanyaRightGuard,
        /// <summary>
        /// 潘夜牛魔王
        /// </summary>
        [Description("Mon14-9 潘夜牛魔王")]
        EmperorSaWoo,
        /// <summary>
        /// 骷髅弓箭手
        /// </summary>
        [Description("Mon15-4 骷髅弓箭手")]
        BoneArcher,
        /// <summary>
        /// 骷髅武士
        /// </summary>
        [Description("Mon15-3 骷髅武士")]
        BoneBladesman,
        /// <summary>
        /// 骷髅武将
        /// </summary>
        [Description("Mon15-0 骷髅武将")]
        BoneCaptain,
        /// <summary>
        /// 骷髅士兵
        /// </summary>
        [Description("Mon15-2 骷髅士兵")]
        BoneSoldier,
        /// <summary>
        /// 骷髅教主
        /// </summary>
        [Description("Mon15-1 骷髅教主")]
        ArchLichTaedu,
        /// <summary>
        /// 角蝇
        /// </summary>
        [Description("Mon8-3 角蝇")]
        WedgeMothLarva,
        /// <summary>
        /// 蝙蝠
        /// </summary>
        [Description("Mon8-4 蝙蝠")]
        LesserWedgeMoth,
        /// <summary>
        /// 楔蛾
        /// </summary>
        [Description("Mon8-5 楔蛾")]
        WedgeMoth,
        /// <summary>
        /// 红野猪
        /// </summary>
        [Description("Mon8-6 红野猪")]
        RedBoar,
        /// <summary>
        /// 蝎蛇
        /// </summary>
        [Description("Mon8-9 蝎蛇")]
        ClawSerpent,
        /// <summary>
        /// 黑野猪
        /// </summary>
        [Description("Mon8-7 黑野猪")]
        BlackBoar,
        /// <summary>
        /// 白野猪
        /// </summary>
        [Description("Mon8-8 白野猪")]
        TuskLord,
        /// <summary>
        /// 超级黑猪王
        /// </summary>
        [Description("Mon16-0 超级黑猪王")]
        RazorTusk,
        /// <summary>
        /// 黑度紫红女神
        /// </summary>
        [Description("Mon17-2 黑度紫红女神")]
        PinkGoddess,
        /// <summary>
        /// 黑度绿荫女神
        /// </summary>
        [Description("Mon17-3 黑度绿荫女神")]
        GreenGoddess,
        /// <summary>
        /// 武力魔神将
        /// </summary>
        [Description("Mon17-1 武力魔神将")]
        MutantCaptain,
        /// <summary>
        /// 石像狮子
        /// </summary>
        [Description("Mon17-0 石像狮子")]
        StoneGriffin,
        /// <summary>
        /// 火焰狮子
        /// </summary>
        [Description("Mon16-9 火焰狮子")]
        FlameGriffin,
        /// <summary>
        /// 变异骷髅
        /// </summary>
        [Description("Mon6-6 变异骷髅")]
        WhiteBone,
        /// <summary>
        /// 神兽
        /// </summary>
        [Description("Mon10-0 神兽")]
        Shinsu,
        /// <summary>
        /// 炎魔
        /// </summary>
        [Description("Mon26-2 炎魔")]
        InfernalSoldier,
        /// <summary>
        /// 镜像
        /// </summary>
        [Description("镜像")]
        InfernalGuardian,
        /// <summary>
        /// 地狱战士
        /// </summary>
        [Description("地狱战士(弃用)")]
        InfernalWarrior,
        /// <summary>
        /// 犬猴魔
        /// </summary>
        [Description("Mon2-2 犬猴魔")]
        CorpseStalker,
        /// <summary>
        /// 轻甲守卫
        /// </summary>
        [Description("Mon1-6 轻甲守卫")]
        LightArmedSoldier,
        /// <summary>
        /// 爆毒神魔
        /// </summary>
        [Description("Mon10-3 爆毒神魔")]
        CorrosivePoisonSpitter,
        /// <summary>
        /// 神舰守卫
        /// </summary>
        [Description("Mon10-9 神舰守卫")]
        PhantomSoldier,
        /// <summary>
        /// 触角神魔
        /// </summary>
        [Description("Mon1-2 触角神魔")]
        MutatedOctopus,
        /// <summary>
        /// 恶形鬼
        /// </summary>
        [Description("Mon10-2 恶形鬼")]
        AquaLizard,
        /// <summary>
        /// 海神将领
        /// </summary>
        [Description("Mon1-9 海神将领")]
        Stomper,
        /// <summary>
        /// 红衣法师
        /// </summary>
        [Description("Mon2-9 红衣法师")]
        CrimsonNecromancer,
        /// <summary>
        /// 霸王守卫
        /// </summary>
        [Description("Mon2-0 霸王守卫")]
        ChaosKnight,
        /// <summary>
        /// 霸王教主
        /// </summary>
        [Description("Mon13-0 霸王教主")]
        PachonTheChaosBringer,
        /// <summary>
        /// 诺玛骑兵
        /// </summary>
        [Description("Mon19-0 诺玛骑兵")]
        NumaCavalry,
        /// <summary>
        /// 诺玛司令
        /// </summary>
        [Description("Mon19-4 诺玛司令")]
        NumaHighMage,
        /// <summary>
        /// 诺玛抛石兵
        /// </summary>
        [Description("Mon19-3 诺玛抛石兵")]
        NumaStoneThrower,
        /// <summary>
        /// 诺玛斧兵
        /// </summary>
        [Description("Mon19-5 诺玛斧兵")]
        NumaRoyalGuard,
        /// <summary>
        /// 诺玛装甲兵
        /// </summary>
        [Description("Mon19-1 诺玛装甲兵")]
        NumaArmoredSoldier,
        /// <summary>
        /// 冰魂弓箭手
        /// </summary>
        [Description("Mon21-0 冰魂弓箭手")]
        IcyRanger,
        /// <summary>
        /// 魄冰女神
        /// </summary>
        [Description("Mon18-0 魄冰女神")]
        IcyGoddess,
        /// <summary>
        /// 冰魂鬼武士
        /// </summary>
        [Description("Mon21-2 冰魂鬼武士")]
        IcySpiritWarrior,
        /// <summary>
        /// 冰魂鬼武将
        /// </summary>
        [Description("Mon21-3 冰魂鬼武将")]
        IcySpiritGeneral,
        /// <summary>
        /// 幽灵骑士
        /// </summary>
        [Description("Mon21-4 幽灵骑士")]
        GhostKnight,
        /// <summary>
        /// 冰魂鬼卒
        /// </summary>
        [Description("Mon21-6 冰魂鬼卒")]
        IcySpiritSpearman,
        /// <summary>
        /// 狼人
        /// </summary>
        [Description("Mon21-7 狼人")]
        Werewolf,
        /// <summary>
        /// 雪狼
        /// </summary>
        [Description("Mon21-8 雪狼")]
        Whitefang,
        /// <summary>
        /// 冰魂卫士
        /// </summary>
        [Description("Mon21-9 冰魂卫士")]
        IcySpiritSolider,
        /// <summary>
        /// 野猪
        /// </summary>
        [Description("Mon18-1 野猪")]
        WildBoar,
        /// <summary>
        /// 赤龙石门
        /// </summary>
        [Description("Mon23-9 赤龙石门")]
        JinamStoneGate,
        /// <summary>
        /// 火影
        /// </summary>
        [Description("Mon21-5 火影")]
        FrostLordHwa,
        /// <summary>
        /// 宠物小花猪
        /// </summary>
        [Description("Mon34-0 宠物小花猪")]
        Companion_Pig,
        /// <summary>
        /// 宠物可爱的白猪
        /// </summary>
        [Description("Mon34-1 宠物可爱的白猪")]
        Companion_TuskLord,
        /// <summary>
        /// 宠物小骷髅战士
        /// </summary>
        [Description("Mon34-2 宠物小骷髅战士")]
        Companion_SkeletonLord,
        /// <summary>
        /// 宠物幼狮
        /// </summary>
        [Description("Mon34-3 宠物幼狮")]
        Companion_Griffin,
        /// <summary>
        /// 宠物小银龙
        /// </summary>
        [Description("Mon34-4 宠物小银龙")]
        Companion_Dragon,
        /// <summary>
        /// 宠物毛驴
        /// </summary>
        [Description("Mon34-5 宠物毛驴")]
        Companion_Donkey,
        /// <summary>
        /// 宠物山羊
        /// </summary>
        [Description("Mon34-6 宠物山羊")]
        Companion_Sheep,
        /// <summary>
        /// 宠物霸主
        /// </summary>
        [Description("Mon34-7 宠物霸主")]
        Companion_BanyoLordGuzak,
        /// <summary>
        /// 宠物熊猫酒仙
        /// </summary>
        [Description("Mon34-8 宠物熊猫酒仙")]
        Companion_Panda,
        /// <summary>
        /// 宠物韩版兔子
        /// </summary>
        [Description("Mon34-9 宠物韩版兔子")]
        Companion_Rabbit,
        /// <summary>
        /// 震天魔神
        /// </summary>
        [Description("Mon17-4 震天魔神")]
        JinchonDevil,
        /// <summary>
        /// 半兽首将
        /// </summary>
        [Description("Mon3-7 半兽首将")]
        OmaWarlord,
        /// <summary>
        /// 卫护将首
        /// </summary>
        [Description("Mon22-0 卫护将首")]
        EscortCommander,
        /// <summary>
        /// 红衣舞姬
        /// </summary>
        [Description("Mon22-2 红衣舞姬")]
        FieryDancer,
        /// <summary>
        /// 绿衣舞姬
        /// </summary>
        [Description("Mon22-3 绿衣舞姬")]
        EmeraldDancer,
        /// <summary>
        /// 黎明女王
        /// </summary>
        [Description("Mon22-1 黎明女王")]
        QueenOfDawn,
        /// <summary>
        /// 雾影魔卒
        /// </summary>
        [Description("Mon23-3 雾影魔卒")]
        OYoungBeast,
        /// <summary>
        /// 阎昆魔女
        /// </summary>
        [Description("Mon23-6 阎昆魔女")]
        YumgonWitch,
        /// <summary>
        /// 魔大将
        /// </summary>
        [Description("Mon23-4 魔大将")]
        MaWarlord,
        /// <summary>
        /// 真幻鬼
        /// </summary>
        [Description("Mon23-7 真幻鬼")]
        JinhwanSpirit,
        /// <summary>
        /// 真幻鬼婢
        /// </summary>
        [Description("Mon23-8 真幻鬼婢")]
        JinhwanGuardian,
        /// <summary>
        /// 阎昆魔军
        /// </summary>
        [Description("Mon23-5 阎昆魔军")]
        YumgonGeneral,
        /// <summary>
        /// 蚩尤将军
        /// </summary>
        [Description("Mon23-0 蚩尤将军")]
        ChiwooGeneral,
        /// <summary>
        /// 赤龙女王
        /// </summary>
        [Description("Mon23-2 赤龙女王")]
        DragonQueen,
        /// <summary>
        /// 赤龙魔王
        /// </summary>
        [Description("Mon23-1 赤龙魔王")]
        DragonLord,
        /// <summary>
        /// 冰湖白魔兽
        /// </summary>
        [Description("Mon21-1 冰湖白魔兽")]
        FerociousIceTiger,
        /// <summary>
        /// 火系士兵
        /// </summary>
        [Description("Mon25-0 火系士兵")]
        SamaFireGuardian,
        /// <summary>
        /// 冰系士兵
        /// </summary>
        [Description("Mon25-1 冰系士兵")]
        SamaIceGuardian,
        /// <summary>
        /// 雷系士兵
        /// </summary>
        [Description("Mon25-2 雷系士兵")]
        SamaLightningGuardian,
        /// <summary>
        /// 风系士兵
        /// </summary>
        [Description("Mon25-3 风系士兵")]
        SamaWindGuardian,
        /// <summary>
        /// 朱雀天王
        /// </summary>
        [Description("Mon25-4 朱雀天王")]
        Phoenix,
        /// <summary>
        /// 玄武天王
        /// </summary>
        [Description("Mon25-5 玄武天王")]
        BlackTortoise,
        /// <summary>
        /// 青龙天王
        /// </summary>
        [Description("Mon25-6 青龙天王")]
        BlueDragon,
        /// <summary>
        /// 白虎天王
        /// </summary>
        [Description("Mon25-7 白虎天王")]
        WhiteTiger,
        /// <summary>
        /// 剑客神徒
        /// </summary>
        [Description("Mon27-0 剑客神徒")]
        SamaCursedBladesman,
        /// <summary>
        /// 法术神徒
        /// </summary>
        [Description("Mon27-1 法术神徒")]
        SamaCursedSlave,
        /// <summary>
        /// 烈火神徒
        /// </summary>
        [Description("Mon27-2 烈火神徒")]
        SamaCursedFlameMage,
        /// <summary>
        /// 魔灵神主
        /// </summary>
        [Description("Mon27-3 魔灵神主")]
        SamaProphet,
        /// <summary>
        /// 魔法师
        /// </summary>
        [Description("Mon27-4 魔法师")]
        SamaSorcerer,
        /// <summary>
        /// 封印盒
        /// </summary>
        [Description("Mon27-5 封印盒")]
        EnshrinementBox,
        /// <summary>
        /// 灵石
        /// </summary>
        [Description("Mon19-7 灵石")]
        BloodStone,
        /// <summary>
        /// 纯虎
        /// </summary>
        [Description("Mon35-0 纯虎")]
        OrangeTiger,
        /// <summary>
        /// 黄虎
        /// </summary>
        [Description("Mon35-1 黄虎")]
        RegularTiger,
        /// <summary>
        /// 褐虎
        /// </summary>
        [Description("Mon35-2 褐虎")]
        RedTiger,
        /// <summary>
        /// 雪虎
        /// </summary>
        [Description("Mon35-3 雪虎")]
        SnowTiger,
        /// <summary>
        /// 黑虎
        /// </summary>
        [Description("Mon35-4 黑虎")]
        BlackTiger,
        /// <summary>
        /// 黑翼虎
        /// </summary>
        [Description("Mon35-5 黑翼虎")]
        BigBlackTiger,
        /// <summary>
        /// 白翼虎
        /// </summary>
        [Description("Mon35-6 白翼虎")]
        BigWhiteTiger,
        /// <summary>
        /// 虎将军
        /// </summary>
        [Description("Mon35-7 虎将军")]
        OrangeBossTiger,
        /// <summary>
        /// 虎老板
        /// </summary>
        [Description("Mon35-8 虎老板")]
        BigBossTiger,
        /// <summary>
        /// 暴戾猿猴战士
        /// </summary>
        [Description("Mon30-0 暴戾猿猴战士")]
        WildMonkey,
        /// <summary>
        /// 霜冻雪人
        /// </summary>
        [Description("Mon30-1 霜冻雪人")]
        FrostYeti,
        /// <summary>
        /// 邪恶毒蛇
        /// </summary>
        [Description("Mon9-0 邪恶毒蛇")]
        EvilSnake,
        /// <summary>
        /// 沙漠蜥蜴
        /// </summary>
        [Description("Mon28-0沙漠蜥蜴")]
        Salamander,
        /// <summary>
        /// 沙鬼
        /// </summary>
        [Description("Mon28-1 沙鬼")]
        SandGolem,

        [Description("Mon29-0 诺玛攻城士兵")]
        SDMob4,
        [Description("Mon29-1 诺玛攻城战士")]
        SDMob5,
        [Description("Mon29-2 诺玛攻城法师")]
        SDMob6,

        /// <summary>
        /// 半兽武士
        /// </summary>
        [Description("Mon29-8 半兽武士")]
        SDMob7,
        /// <summary>
        /// 半兽法师
        /// </summary>
        [Description("Mon29-9半兽法师")]
        OmaMage,

        [Description("Mon32-1 月河怪物")]
        SDMob9,
        [Description("Mon32-5 月河怪物")]
        SDMob10,
        [Description("Mon32-6 月河怪物")]
        SDMob11,
        [Description("Mon32-7 月河怪物")]
        SDMob12,
        [Description("Mon32-8 月河怪物")]
        SDMob13,
        [Description("Mon32-9 月河怪物")]
        SDMob14,

        /// <summary>
        /// 水晶傀儡
        /// </summary>
        [Description("Mon40-0 水晶傀儡")]
        CrystalGolem,
        /// <summary>
        /// 尘土恶魔
        /// </summary>
        [Description("Mon41-1 尘土恶魔")]
        DustDevil,
        /// <summary>
        /// 双尾蝎子
        /// </summary>
        [Description("Mon41-2 双尾蝎子")]
        TwinTailScorpion,
        /// <summary>
        /// 嗜血鼹
        /// </summary>
        [Description("Mon41-3 嗜血鼹")]
        BloodyMole,
        /// <summary>
        /// 异魔族战士
        /// </summary>
        [Description("Mon44-3 异魔族战士")]
        SDMob19,
        /// <summary>
        /// 异魔族兵卒
        /// </summary>
        [Description("Mon44-4 异魔族兵卒")]
        SDMob20,
        /// <summary>
        /// 异魔族弓手
        /// </summary>
        [Description("Mon44-5 异魔族弓手")]
        SDMob21,
        /// <summary>
        /// 异魔族骤术师
        /// </summary>
        [Description("Mon44-6 异魔族骤术师")]
        SDMob22,
        /// <summary>
        /// 异魔族百夫长
        /// </summary>
        [Description("Mon44-7 异魔族百夫长")]
        SDMob23,
        /// <summary>
        /// 灰饕餮
        /// </summary>
        [Description("Mon44-8 灰饕餮")]
        SDMob24,
        /// <summary>
        /// 绿饕餮
        /// </summary>
        [Description("Mon44-9 绿饕餮")]
        SDMob25,
        /// <summary>
        /// 独眼蜘蛛
        /// </summary>
        [Description("Mon28-8 独眼蜘蛛")]
        GangSpider,
        /// <summary>
        /// 天狼蜘蛛
        /// </summary>
        [Description("Mon28-9 天狼蜘蛛")]
        VenomSpider,
        /// <summary>
        /// 异魔族族长
        /// </summary>
        [Description("Mon45-0 异魔族族长")]
        SDMob26,
        /// <summary>
        /// 龙虾王
        /// </summary>
        [Description("Mon45-3 龙虾王")]
        LobsterLord,
        /// <summary>
        /// 龙虾脚
        /// </summary>
        [Description("龙虾脚(弃用)")]
        LobsterSpawn,
        /// <summary>
        /// 水晶火虫
        /// </summary>
        [Description("Mon47-0 水晶火虫")]
        NewMob1,
        /// <summary>
        /// 水晶蠕虫
        /// </summary>
        [Description("Mon47-1 水晶蠕虫")]
        NewMob2,
        /// <summary>
        /// 水晶蝙蝠
        /// </summary>
        [Description("Mon47-2 水晶蝙蝠")]
        NewMob3,
        /// <summary>
        /// 水晶魔像
        /// </summary>
        [Description("Mon47-3 水晶魔像")]
        NewMob4,
        /// <summary>
        /// 水晶金魔像
        /// </summary>
        [Description("Mon47-4 水晶金魔像")]
        NewMob5,
        /// <summary>
        /// 水晶长枪狂徒
        /// </summary>
        [Description("Mon47-5 水晶长枪狂徒")]
        NewMob6,
        /// <summary>
        /// 水晶魔法狂徒
        /// </summary>
        [Description("Mon47-6 水晶魔法狂徒")]
        NewMob7,
        /// <summary>
        /// 水晶玄武
        /// </summary>
        [Description("Mon47-7 水晶玄武")]
        NewMob8,
        /// <summary>
        /// 水晶小玄武
        /// </summary>
        [Description("Mon47-8 水晶小玄武")]
        NewMob9,
        /// <summary>
        /// 水晶守护树
        /// </summary>
        [Description("Mon47-9 水晶守护树")]
        NewMob10,
        /// <summary>
        /// 小僵尸
        /// </summary>
        [Description("Mon49-0 小僵尸")]
        MonasteryMon0,
        /// <summary>
        /// 僵尸
        /// </summary>
        [Description("Mon49-1 僵尸")]
        MonasteryMon1,
        /// <summary>
        /// 怨魂僵尸
        /// </summary>
        [Description("Mon49-2 怨魂僵尸")]
        MonasteryMon2,
        /// <summary>
        /// 血灵僵尸
        /// </summary>
        [Description("Mon49-3 血灵僵尸")]
        MonasteryMon3,
        /// <summary>
        /// 魔道道士
        /// </summary>
        [Description("Mon49-4 魔道道士")]
        MonasteryMon4,
        /// <summary>
        /// 魔气僵尸魔道
        /// </summary>
        [Description("Mon49-5 魔气僵尸魔道")]
        MonasteryMon5,
        /// <summary>
        /// 魔气大僵尸
        /// </summary>
        [Description("Mon49-6 魔气大僵尸")]
        MonasteryMon6,

        /// <summary>
        /// 自定义怪物
        /// </summary>
        [Description("自定义")]
        DiyMonsMon,

        /// <summary>
        /// 密林浣候
        /// </summary>
        [Description("Mon36-2 密林浣候")]
        CrazedPrimate,
        /// <summary>
        /// 密林BOSS六角魔兽
        /// </summary>
        [Description("Mon36-3 密林BOSS六角魔兽")]
        HellBringer,
        /// <summary>
        /// 密林白鬼狼
        /// </summary>
        [Description("Mon37-0 密林白鬼狼")]
        YurinMon0,
        /// <summary>
        /// 密林赤鬼狼
        /// </summary>
        [Description("Mon37-1 密林赤鬼狼")]
        YurinMon1,
        /// <summary>
        /// 密林巨角白虎
        /// </summary>
        [Description("Mon37-2 密林巨角白虎")]
        WhiteBeardedTiger,
        /// <summary>
        /// 密林巨角黑虎
        /// </summary>
        [Description("Mon37-3 密林巨角黑虎")]
        BlackBeardedTiger,
        /// <summary>
        /// 密林铁甲角牛
        /// </summary>
        [Description("Mon37-4 密林铁甲角牛")]
        HardenedRhino,
        /// <summary>
        /// 密林大角象
        /// </summary>
        [Description("Mon37-5 密林大角象")]
        Mammoth,
        /// <summary>
        /// 密林斩决鬼
        /// </summary>
        [Description("Mon37-6 密林斩决鬼")]
        CursedSlave1,
        /// <summary>
        /// 密林影软鬼
        /// </summary>
        [Description("Mon37-7 密林影软鬼")]
        CursedSlave2,
        /// <summary>
        /// 密林盲鬼
        /// </summary>
        [Description("Mon37-8 密林虎型鬼")]
        CursedSlave3,
        /// <summary>
        /// 密林虎型鬼
        /// </summary>
        [Description("Mon37-9 密林盲鬼")]
        PoisonousGolem,

        /// <summary>
        /// 花园战士
        /// </summary>
        [Description("Mon38-0 花园战士")]
        GardenSoldier,
        /// <summary>
        /// 花园剑士
        /// </summary>
        [Description("Mon38-1 花园剑士")]
        GardenDefender,
        /// <summary>
        /// 花园红芭蕉
        /// </summary>
        [Description("Mon38-2 花园红芭蕉")]
        RedBlossom,
        /// <summary>
        /// 花园青芭蕉
        /// </summary>
        [Description("Mon38-3 花园青芭蕉")]
        BlueBlossom,
        /// <summary>
        /// 花园火冥鸢
        /// </summary>
        [Description("Mon38-4 花园火冥鸢")]
        FireBird,

        /// <summary>
        /// 古墓士兵
        /// </summary>
        [Description("Mon42-4 古墓士兵")]
        Terracotta1,
        /// <summary>
        /// 古墓矛兵
        /// </summary>
        [Description("Mon42-5 古墓矛兵")]
        Terracotta2,
        /// <summary>
        /// 古墓骑兵
        /// </summary>
        [Description("Mon42-6 古墓骑兵")]
        Terracotta3,
        /// <summary>
        /// 古墓长矛骑兵
        /// </summary>
        [Description("Mon42-7 古墓长矛骑兵")]
        Terracotta4,
        /// <summary>
        /// 古墓护卫武士
        /// </summary>
        [Description("Mon42-8 古墓护卫武士")]
        TerracottaSub,
        /// <summary>
        /// 古墓BOSS古墓主人
        /// </summary>
        [Description("Mon42-9 古墓BOSS古墓主人")]
        TerracottaBoss,
        /// <summary>
        /// 练功木桩
        /// </summary>
        [Description("Mon39-7 练功木桩")]
        Practitioner,
        /// <summary>
        /// 弓箭手
        /// </summary>
        [Description("Mon9-6 弓箭手")]
        Archer,
        /// <summary>
        /// 沙巴克主门
        /// </summary>
        [Description("Mon54-0 沙巴克主门")]
        SabukPrimeGate,
        /// <summary>
        /// 沙巴克左门
        /// </summary>
        [Description("Mon54-3 沙巴克左门")]
        SabukLeftDoor,
        /// <summary>
        /// 沙巴克右门
        /// </summary>
        [Description("Mon54-3 沙巴克右门")]
        SabukRightDoor,
        /// <summary>
        /// 半兽人
        /// </summary>
        [Description("Mon28-4 半兽人")]
        OmaA,
        /// <summary>
        /// 红蛇
        /// </summary>
        [Description("Mon6-4 红蛇")]
        RedSnake,
        /// <summary>
        /// 七点白蛇
        /// </summary>
        [Description("Mon13-6 七点白蛇")]
        SevenPointWhiteSnake,
        /// <summary>
        /// 黑影鬼
        /// </summary>
        [Description("Mon39-4 黑影鬼")]
        ShadowGhost,
        /// <summary>
        /// 九幽灵
        /// </summary>
        [Description("Mon46-5 九幽灵")]
        NineGhosts,
        /// <summary>
        /// 投石车
        /// </summary>
		[Description("Mon17-7 投石车")]
        Catapult,
        /// <summary>
        /// 弩车
        /// </summary>
        [Description("Mon17-9 弩车")]
        Ballista,
        /// <summary>
        /// 棕马
        /// </summary>
        [Description("Mon52-0 棕马")]
        BrownHorse,
        /// <summary>
        /// 白马
        /// </summary>
        [Description("Mon52-1 白马")]
        WhiteHorse,
        /// <summary>
        /// 黑马
        /// </summary>
        [Description("Mon52-2 黑马")]
        BlackHorse,
        /// <summary>
        /// 红马
        /// </summary>
        [Description("Mon52-3 红马")]
        RedHorse,
        /// <summary>
        /// 诺玛教主
        /// </summary>
        [Description("Mon19-6 诺玛教主")]
        MasterNorma,
        /// <summary>
        /// 幽灵
        /// </summary>
        [Description("Mon2-8 幽灵")]
        Ghost,
        /// <summary>
        /// 沙城守护者
        /// </summary>
        [Description("Mon45-0 沙城守护者")]
        SabakGuardian,
        /// <summary>
        /// 左栏栅
        /// </summary>
        [Description("Mon54-4 左栏栅")]
        LeftFence,
        /// <summary>
        /// 右栏栅
        /// </summary>
        [Description("Mon54-5 右栏栅")]
        RightFence,
        /// <summary>
        /// 前栏栅
        /// </summary>
        [Description("Mon54-7 前栏栅")]
        FrontFence,
        /// <summary>
        /// 后栏栅
        /// </summary>
        [Description("Mon54-6 后栏栅")]
        AfterFence,
        /// <summary>
        /// 诺玛圣骑士
        /// </summary>
        [Description("Mon102-1 诺玛圣骑士")]
        NormaPaladin,
        /// <summary>
        /// 诺玛大司令
        /// </summary>
        [Description("Mon102-4 诺玛大司令")]
        NomaCommander,
        /// <summary>
        /// 诺玛抛石王
        /// </summary>
        [Description("Mon102-3 诺玛抛石王")]
        NormaRiprapKing,
        /// <summary>
        /// 诺玛斧兵王
        /// </summary>
        [Description("Mon102-5 诺玛斧兵王")]
        NomaAxeWarriorKing,
        /// <summary>
        /// 诺玛装甲王
        /// </summary>
        [Description("Mon102-2 诺玛装甲王")]
        NormaArmorKing,
        /// <summary>
        /// 诺玛统领
        /// </summary>
        [Description("Mon102-6 诺玛统领")]
        CommanderNoma,
        /// <summary>
        /// 诺玛大法老
        /// </summary>
        [Description("Mon102-0 诺玛大法老")]
        PharaohNorma,
        /// <summary>
        /// 劳动蚂蚁
        /// </summary>
        [Description("Mon28-3 劳动蚂蚁")]
        LaborAnts,
    }

    /// <summary>
    /// 怪物自定义攻击类型
    /// </summary>
    public enum MonDiyActType
    {
        /// <summary>
        /// 怪物自定义普通攻击
        /// </summary>
        [Description("普通攻击")]
        Attack,
        /// <summary>
        /// 怪物自定义范围攻击
        /// </summary>
        [Description("范围攻击")]
        RangeAttack,
        /// <summary>
        /// 怪物自定义远程攻击
        /// </summary>
        [Description("远程攻击")]
        RemoteAttack,
        /// <summary>
        /// 怪物自定义显示效果
        /// </summary>
        [Description("显示效果")]
        ShowEffect
    }
    /// <summary>
    /// 怪物自定义攻击力类型
    /// </summary>
    public enum MonDiyPowerType
    {
        /// <summary>
        /// 置空
        /// </summary>
        [Description("置空")]
        NONE,
        /// <summary>
        /// 怪物自定义物理攻击
        /// </summary>
        [Description("破坏物理攻击")]
        DC,
        /// <summary>
        /// 怪物自定义自然攻击
        /// </summary>
        [Description("自然魔法攻击")]
        MC,
        /// <summary>
        /// 怪物自定义灵魂攻击
        /// </summary>
        [Description("灵魂道术攻击")]
        SC
    }
    /// <summary>
    /// 怪物自定义目标类型
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// 目标类型自己
        /// </summary>
        [Description("目标类型自己")]
        SELF,
        /// <summary>
        /// 目标类型攻击目标
        /// </summary>
        [Description("目标类型攻击目标")]
        TARGET,
        /// <summary>
        /// 目标类型直线攻击
        /// </summary>
        [Description("目标类型直线攻击")]
        LINE,
        /// <summary>
        /// 目标类型半月范围攻击
        /// </summary>
        [Description("目标类型半月范围攻击")]
        HALFMOON,
        /// <summary>
        /// 目标类型圆形攻击
        /// </summary>
        [Description("目标类型圆形攻击")]
        FULLMOON,
        /// <summary>
        /// 目标类型敌对
        /// </summary>
        [Description("目标类型敌对")]
        ENEMY,
        /// <summary>
        /// 目标类型朋友
        /// </summary>
        [Description("目标类型朋友")]
        FRIEND
    }
    /// <summary>
    /// 怪物自定义检测类型
    /// </summary>
    public enum MonCheckType
    {
        /// <summary>
        /// 检测类型距离
        /// </summary>
        [Description("检测距离NEAR")]
        NEAR,
        /// <summary>
        /// 检测类型数值
        /// </summary>
        [Description("检测数值RANDOM")]
        RANDOM,
        /// <summary>
        /// 检测类型范围
        /// </summary>
        [Description("检测范围SURROUNDED")]
        SURROUNDED,
        /// <summary>
        /// 检测类型血量
        /// </summary>
        [Description("检测血量LOWHP")]
        LOWHP,
        /// <summary>
        /// 检测类型攻击值
        /// </summary>
        [Description("检测攻击值POWER")]
        POWER
    }
    /// <summary>
    /// 怪物自定义逻辑符号
    /// </summary>
    public enum Operators
    {
        /// <summary>
        /// 逻辑判断等于
        /// </summary>
        [Description("等于")]
        Equal,
        /// <summary>
        /// 逻辑判断不等于
        /// </summary>
        [Description("不等于")]
        NotEqual,
        /// <summary>
        /// 逻辑判断小于
        /// </summary>
        [Description("小于")]
        LessThan,
        /// <summary>
        /// 逻辑判断小于或者等于
        /// </summary>
        [Description("小于等于")]
        LessThanOrEqual,
        /// <summary>
        /// 逻辑判断大于
        /// </summary>
        [Description("大于")]
        GreaterThan,
        /// <summary>
        /// 逻辑判断大于或等于
        /// </summary>
        [Description("大于等于")]
        GreaterThanOrEqual,
    }
    /// <summary>
    /// 怪物自定义攻击效果
    /// </summary>
    public enum AttackEffect
    {
        /// <summary>
        /// 延迟伤害效果
        /// </summary>
        [Description("延迟伤害效果 Damage")]
        Damage,
        /// <summary>
        /// 红毒加绿毒效果
        /// </summary>
        [Description("红毒加绿毒效果 HAPosion")]
        HAPosion,
        /// <summary>
        /// 绿毒减血效果
        /// </summary>
        [Description("绿毒减血效果 HpPosion")]
        HpPosion,
        /// <summary>
        /// 红毒降防效果
        /// </summary>
        [Description("红毒降防效果 AcPosion")]
        AcPosion,
        /// <summary>
        /// 麻痹效果
        /// </summary>
        [Description("麻痹效果 StonePosion")]
        StonePosion,
        /// <summary>
        /// 减速效果
        /// </summary>
        [Description("减速效果 FaintPosion")]
        FaintPosion,
        /// <summary>
        /// 推动抗拒
        /// </summary>
        [Description("推抗拒效果 Push")]
        Push,
        /// <summary>
        /// 吸人效果
        /// </summary>
        [Description("吸人效果 Pullover")]
        Pullover,
        /// <summary>
        /// 治愈效果
        /// </summary>
        [Description("治愈效果 Heal")]
        Heal,
        /// <summary>
        /// 隐身效果
        /// </summary>
        [Description("隐身效果 Invisib")]
        Invisib,
        /// <summary>
        /// 秒影并传送效果
        /// </summary>
        [Description("秒影并传送效果 Tranport")]
        Tranport,
        /// <summary>
        /// 吸取生命值效果
        /// </summary>
        [Description("吸取生命值效果 LifeSteal")]
        LifeSteal,
        /// <summary>
        /// 清除负面状态效果
        /// </summary>
        [Description("清除负面状态效果 Clearstatus")]
        Clearstatus,
        /// <summary>
        /// 增加破坏攻击值
        /// </summary>
        [Description("增加破坏攻击值 AddDc")]
        AddDc,
        /// <summary>
        /// 增加自然魔法值
        /// </summary>
        [Description("增加自然魔法值 AddMc")]
        AddMc,
        /// <summary>
        /// 增加灵魂道术值
        /// </summary>
        [Description("增加灵魂道术值 AddSc")]
        AddSc,
        /// <summary>
        /// 增加防御值
        /// </summary>
        [Description("增加防御值 AddAc")]
        AddAc,
        /// <summary>
        /// 增加魔防值
        /// </summary>
        [Description("增加魔防值 AddMac")]
        AddMac,
        /// <summary>
        /// 增加攻击翻倍
        /// </summary>
        [Description("增加攻击翻倍 Power")]
        Power,
        /// <summary>
        /// 说话效果
        /// </summary>
        [Description("说话效果 Say")]
        Say,
        /// <summary>
        /// 跳转效果
        /// </summary>
        [Description("跳转效果 Jumpto")]
        Jumpto,
        /// <summary>
        /// 召回效果
        /// </summary>
        [Description("召回效果 Recall")]
        Recall
    }
    /// <summary>
    /// 大地图图标
    /// </summary>
    public enum MapIcon
    {
        /// <summary>
        /// 地图图标为空
        /// </summary>
        [Description("空置")]
        None,
        /// <summary>
        /// 地图图标洞穴
        /// </summary>
        [Description("洞穴")]
        Cave,
        /// <summary>
        /// 地图图标出口
        /// </summary>
        [Description("出口")]
        Exit,
        /// <summary>
        /// 地图图标进下层
        /// </summary>
        [Description("进下层")]
        Down,
        /// <summary>
        /// 地图图标回上层
        /// </summary>
        [Description("回上层")]
        Up,
        /// <summary>
        /// 地图图标省份
        /// </summary>
        [Description("省份")]
        Province,
        /// <summary>
        /// 地图图标建筑
        /// </summary>
        [Description("建筑")]
        Building,

        [Description("入口550小骷髅")]
        Entrance550,
        [Description("入口551大骷髅")]
        Entrance551,
        [Description("入口552矿")]
        Entrance552,
        [Description("入口553蚂蚁")]
        Entrance553,
        [Description("入口554牛头")]
        Entrance554,
        [Description("入口555大牛头")]
        Entrance555,
        [Description("入口556迷宫")]
        Entrance556,
        [Description("入口557小三角")]
        Entrance557,
        [Description("入口558大三角")]
        Entrance558,
        [Description("入口559佛头")]
        Entrance559,
        [Description("入口560四角头")]
        Entrance560,
        [Description("入口561船")]
        Entrance561,
        [Description("入口562花")]
        Entrance562,
        [Description("入口563大密林")]
        Entrance563,

        [Description("连接100图标小城")]
        Connect100,
        [Description("连接102图标树")]
        Connect102,
        [Description("连接103图标迷宫")]
        Connect103,
        [Description("连接104图标蛇")]
        Connect104,
        [Description("连接120图标金字塔")]
        Connect120,
        [Description("连接121图标弓塔")]
        Connect121,
        [Description("连接122图标π")]
        Connect122,
        [Description("连接123图标迷雾")]
        Connect123,
        [Description("连接140图标沙漠1")]
        Connect140,
        [Description("连接141图标沙漠2")]
        Connect141,
        [Description("连接142图标沙漠3")]
        Connect142,
        [Description("连接143图标沙漠4")]
        Connect143,
        [Description("连接160图标密林1")]
        Connect160,
        [Description("连接161图标密林2")]
        Connect161,
        [Description("连接162图标密林3")]
        Connect162,
        [Description("连接300图标六角形")]
        Connect300,
        [Description("连接301图标山谷")]
        Connect301,
        [Description("连接302图标解锁")]
        Connect302,
        [Description("连接510图标小山洞")]
        Connect510,
        [Description("连接511图标四方门")]
        Connect511,
        [Description("连接570图标蜈蚣地宫")]
        Connect570,
        [Description("连接571图标猪洞地宫")]
        Connect571,
        [Description("连接572图标祖玛地宫")]
        Connect572,
    }

    /// <summary>
    /// 特效
    /// </summary>
    public enum Effect
    {
        /// <summary>
        /// 传送出去
        /// </summary>
        TeleportOut,
        /// <summary>
        /// 传送进来
        /// </summary>
        TeleportIn,
        /// <summary>
        /// 盛开
        /// </summary>
        FullBloom,
        /// <summary>
        /// 白莲
        /// </summary>
        WhiteLotus,
        /// <summary>
        /// 红莲
        /// </summary>
        RedLotus,
        /// <summary>
        /// 月季
        /// </summary>
        SweetBrier,
        /// <summary>
        /// 孽报
        /// </summary>
        Karma,
        /// <summary>
        /// 木偶
        /// </summary>
        Puppet,
        /// <summary>
        /// 木偶火
        /// </summary>
        PuppetFire,
        /// <summary>
        /// 木偶冰
        /// </summary>
        PuppetIce,
        /// <summary>
        /// 木偶闪电
        /// </summary>
        PuppetLightning,
        /// <summary>
        /// 木偶风
        /// </summary>
        PuppetWind,
        /// <summary>
        /// 召唤骷髅
        /// </summary>
        SummonSkeleton,
        /// <summary>
        /// 召唤神兽
        /// </summary>
        SummonShinsu,
        /// <summary>
        /// 天打雷劈
        /// </summary>
        ThunderBolt,
        /// <summary>
        /// 鹰击
        /// </summary>
        DanceOfSwallow,
        /// <summary>
        /// 日闪
        /// </summary>
        FlashOfLight,
        /// <summary>
        /// 魔焰强解术
        /// </summary>
        DemonExplosion,
        /// <summary>
        /// 护身冰环
        /// </summary>
        FrostBiteEnd,
        /// <summary>
        /// 抗拒火环
        /// </summary>
        Repulsion,
        /// <summary>
        /// 分身出现时
        /// </summary>
        MirrorImage,
        /// <summary>
        /// 分身死亡时
        /// </summary>
        MirrorImageDie,
    }

    [Flags]
    /// <summary>
    /// 状态类型
    /// </summary>
    public enum PoisonType
    {
        /// <summary>
        /// 为空
        /// </summary>
        None = 0,
        /// <summary>
        /// 绿毒状态
        /// </summary>
        Green = 1,
        /// <summary>
        /// 红毒状态
        /// </summary>
        Red = 2,
        /// <summary>
        /// 减速冰冻状态
        /// </summary>
        Slow = 4,
        /// <summary>
        /// 麻痹状态
        /// </summary>
        Paralysis = 8,
        /// <summary>
        /// 幽灵控制或石化状态
        /// </summary>
        WraithGrip = 16,
        /// <summary>
        /// 火烧状态
        /// </summary>
        HellFire = 32,
        /// <summary>
        /// 沉默状态
        /// </summary>
        Silenced = 64,
        /// <summary>
        /// 深渊状态
        /// </summary>
        Abyss = 128,
        /// <summary>
        /// 传染状态
        /// </summary>
        Infection = 256,
        /// <summary>
        /// 虚弱化状态
        /// </summary>
        Neutralize = 512,
        /// <summary>
        /// 千刃杀风状态
        /// </summary>
        ThousandBlades = 1024,
        /// <summary>
        /// 晕击状态
        /// </summary>
        StunnedStrike = 2048,
        /// <summary>
        /// 电击状态
        /// </summary>
        ElectricShock = 4096,
    }
    /// <summary>
    /// 地面魔法效果
    /// </summary>
    public enum SpellEffect
    {
        /// <summary>
        /// 地面魔法效果空置
        /// </summary>
        None,
        /// <summary>
        /// 地面魔法效果安全区光效
        /// </summary>
        SafeZone,
        /// <summary>
        /// 地面魔法效果火墙
        /// </summary>
        FireWall,
        /// <summary>
        /// 地面魔法效果怪物喷火
        /// </summary>
        MonsterFireWall,
        /// <summary>
        /// 地面魔法效果暴风
        /// </summary>
        Tempest,
        /// <summary>
        /// 地面魔法效果陷阱
        /// </summary>
        TrapOctagon,
        /// <summary>
        /// 地面魔法效果毒云
        /// </summary>
        PoisonousCloud,
        /// <summary>
        /// 地面魔法效果碎石
        /// </summary>
        Rubble,
        /// <summary>
        /// 地面魔法效果怪物死亡云雾
        /// </summary>
        MonsterDeathCloud,
        /// <summary>
        /// 地面魔法效果暗鬼阵
        /// </summary>
        DarkSoulPrison,
        /// <summary>
        /// 地面魔法效果业火
        /// </summary>
        SwordOfVengeance,
        /// <summary>
        /// 地面魔法效果冰雨
        /// </summary>
        IceRain,
        /// <summary>
        /// 地面魔法炮击效果
        /// </summary>
        WarWeaponShell,
    }
    /// <summary>
    /// 寄售索引
    /// </summary>
    [Lang("寄售索引")]
    public enum MarketPlaceSort
    {
        /// <summary>
        /// 最新上架
        /// </summary>
	    [Description("最新上架")]
        Newest,
        /// <summary>
        /// 旧的商品
        /// </summary>
		[Description("旧的商品")]
        Oldest,
        /// <summary>
        /// 最高价格
        /// </summary>
        [Description("最高价格")]
        HighestPrice,
        /// <summary>
        /// 最低价格
        /// </summary>
        [Description("最低价格")]
        LowestPrice,
    }

    /// <summary>
    /// 寄售索引
    /// </summary>
    [Lang("寄售商店索引")]
    public enum MarketPlaceStoreSort
    {
        /// <summary>
        /// 按字母顺序
        /// </summary>
        [Description("按字母顺序")]
        Alphabetical,
        /// <summary>
        /// 最高价格
        /// </summary>
        [Description("最高价格")]
        HighestPrice,
        /// <summary>
        /// 最低价格
        /// </summary>
        [Description("最低价格")]
        LowestPrice,
        /// <summary>
        /// 最受欢迎
        /// </summary>
		[Description("最受欢迎")]
        Favourite
    }

    /// <summary>
    /// 精炼可增加属性类型
    /// </summary>
    public enum RefineType : byte
    {
        /// <summary>
        /// 精炼可增加属性 无
        /// </summary>
        None,
        /// <summary>
        /// 精炼可增加属性 持久
        /// </summary>
        Durability,
        /// <summary>
        /// 精炼可增加属性 破坏
        /// </summary>
        DC,
        /// <summary>
        /// 精炼可增加属性 全系列魔法
        /// </summary>
        SpellPower,
        /// <summary>
        /// 精炼可增加属性 火
        /// </summary>
        Fire,
        /// <summary>
        /// 精炼可增加属性 冰
        /// </summary>
        Ice,
        /// <summary>
        /// 精炼可增加属性 雷
        /// </summary>
        Lightning,
        /// <summary>
        /// 精炼可增加属性 风
        /// </summary>
        Wind,
        /// <summary>
        /// 精炼可增加属性 神圣
        /// </summary>
        Holy,
        /// <summary>
        /// 精炼可增加属性 暗黑
        /// </summary>
        Dark,
        /// <summary>
        /// 精炼可增加属性 幻影
        /// </summary>
        Phantom,
        /// <summary>
        /// 精炼可增加属性 重置
        /// </summary>
        Reset,
        /// <summary>
        /// 精炼可增加属性 血量
        /// </summary>
        Health,
        /// <summary>
        /// 精炼可增加属性 蓝值
        /// </summary>
        Mana,
        /// <summary>
        /// 精炼可增加属性 防御
        /// </summary>
        AC,
        /// <summary>
        /// 精炼可增加属性 魔御
        /// </summary>
        MR,
        /// <summary>
        /// 精炼可增加属性 准确
        /// </summary>
        Accuracy,
        /// <summary>
        /// 精炼可增加属性 敏捷
        /// </summary>
        Agility,
        /// <summary>
        /// 精炼可增加属性 破坏百分比
        /// </summary>
        DCPercent,
        /// <summary>
        /// 精炼可增加属性 全系列魔法百分比
        /// </summary>
        SPPercent,
        /// <summary>
        /// 精炼可增加属性 血量百分比
        /// </summary>
        HealthPercent,
        /// <summary>
        /// 精炼可增加属性 蓝值百分比
        /// </summary>
        ManaPercent,
    }

    /// <summary>
    /// 武器锻造时间
    /// </summary>
    public enum RefineQuality : byte  //武器锻造时间
    {
        /// <summary>
        /// 马上完成
        /// </summary>
        Rush,
        /// <summary>
        /// 快色
        /// </summary>
        Quick,
        /// <summary>
        /// 标准
        /// </summary>
        Standard,
        /// <summary>
        /// 谨慎
        /// </summary>
        Careful,
        /// <summary>
        /// 精确
        /// </summary>
        Precise,
    }

    /// <summary>
    /// 首饰精炼可增加属性类型
    /// </summary>
    public enum JewelryRefineType : byte
    {
        /// <summary>
        /// 精炼可增加属性 破坏
        /// </summary>
        DC,
        /// <summary>
        /// 精炼可增加属性 破坏
        /// </summary>
        MC,
        /// <summary>
        /// 精炼可增加属性 破坏
        /// </summary>
        SC,
        /// <summary>
        /// 精炼可增加属性 火攻
        /// </summary>
        FireAttack,
        /// <summary>
        /// 精炼可增加属性 冰攻
        /// </summary>
        IceAttack,
        /// <summary>
        /// 精炼可增加属性 雷攻
        /// </summary>
        LightningAttack,
        /// <summary>
        /// 精炼可增加属性 风攻
        /// </summary>
        WindAttack,
        /// <summary>
        /// 精炼可增加属性 神圣攻
        /// </summary>
        HolyAttack,
        /// <summary>
        /// 精炼可增加属性 暗黑攻
        /// </summary>
        DarkAttack,
        /// <summary>
        /// 精炼可增加属性 幻影攻
        /// </summary>
        PhantomAttack,
    }

    /// <summary>
    /// 货币通用类型
    /// </summary>
    [Lang("货币通用类型")]
    public enum CurrencyType : byte
    {
        /// <summary>
        /// 货币类型为空
        /// </summary>
        None,
        /// <summary>
        /// 货币类型金币
        /// </summary>
        Gold,
        /// <summary>
        /// 货币类型元宝
        /// </summary>
        [Description("赞助币")]
        GameGold,
        /// <summary>
        /// 货币类型声望FP
        /// </summary>
        Prestige,
        /// <summary>
        /// 货币类型贡献
        /// </summary>
        Contribute,
        /// <summary>
        /// 奖金池的货币
        /// </summary>
        [Description("奖池币")]
        RewardPoolCurrency,
    }

    /// <summary>
    /// 货币变化方向
    /// </summary>
    public enum CurrencyAction
    {
        [Description("未定义")]
        Undefined = 0,

        [Description("增加")]
        Add = 1,

        [Description("减少")]
        Deduct = 2,
    }

    /// <summary>
    /// 货币变化原因
    /// </summary>
    public enum CurrencySource
    {
        [Description("未定义")]
        Undefined = 0,


        [Description("充值+")]
        TopUp = 100,

        [Description("GM添加+")]
        GMAdd = 101,

        [Description("使用道具+")]
        ItemAdd = 102,

        [Description("拾取+")]
        PickUpAdd = 103,

        [Description("脚本获得+")]
        ScriptAdd = 104,

        [Description("交易获得+")]
        TradeAdd = 105,

        [Description("活动获得+")]
        EventAdd = 106,

        [Description("其他货币兑换获得+")]
        RedeemAdd = 107,

        [Description("打怪获得+")]
        KillMobAdd = 108,

        [Description("红包获得+")]
        RedPacketAdd = 109,

        [Description("寄售商城获得+")]
        MarketplaceAdd = 110,

        [Description("其他途径获得+")]
        OtherAdd = 150,



        [Description("寄售商城购物-")]
        MarketplaceDeduct = 200,

        [Description("GM扣除-")]
        GMDeduct = 201,

        [Description("使用道具-")]
        ItemDeduct = 202,

        [Description("丢弃-")]
        DropDeduct = 203,

        [Description("脚本扣除-")]
        ScriptDeduct = 204,

        [Description("交易扣除-")]
        TradeDeduct = 205,

        [Description("活动扣除-")]
        EventDeduct = 206,

        [Description("兑换扣除-")]
        RedeemDeduct = 207,

        [Description("打怪扣除-")]
        KillMobDeduct = 208,

        [Description("红包扣除-")]
        RedPacketDeduct = 209,

        [Description("提现扣除-")]
        CashOutDeduct = 210,

        [Description("系统功能扣除-")]
        SystemDeduct = 211,

        [Description("元宝商城购物-")]
        StoreDeduct = 212,

        [Description("其他途径扣除-")]
        OtherDeduct = 250,
    }

    /// <summary>
    /// 道具作用类型
    /// </summary>
    public enum ItemEffect : byte
    {
        /// <summary>
        /// 空置
        /// </summary>
        [Description("置空")]
        None,
        /// <summary>
        /// 金币
        /// </summary>
        [Description("金币 Gold")]
        Gold = 1,
        /// <summary>
        ///  经验值
        /// </summary>
        [Description("经验值 Experience")]
        Experience = 2,
        /// <summary>
        /// 宠物解锁道具
        /// </summary>
        [Description("宠物解锁 ompanionTicket")]
        CompanionTicket = 3,
        /// <summary>
        /// 宠物包裹
        /// </summary>
        [Description("宠物包裹 BasicCompanionBag")]
        BasicCompanionBag = 4,
        /// <summary>
        /// 挖矿工具
        /// </summary>
        [Description("挖矿工具 PickAxe")]
        PickAxe = 5,
        /// <summary>
        /// 沃玛号角 创建行会工具
        /// </summary>
        [Description("沃玛号角创建行会工具 UmaKingHorn")]
        UmaKingHorn = 6,
        /// <summary>
        /// 道具碎片
        /// </summary>
        [Description("道具碎片 ItemPart")]
        ItemPart = 7,
        /// <summary>
        /// 胡萝卜道具
        /// </summary>
        [Description("胡萝卜道具 Carrot")]
        Carrot = 8,
        /// <summary>
        /// 攻击强效水
        /// </summary>
        [Description("攻击强效水 DestructionElixir")]
        DestructionElixir = 10,
        /// <summary>
        /// 疾风强效水
        /// </summary>
        [Description("疾风强效水 HasteElixir")]
        HasteElixir = 11,
        /// <summary>
        /// 体力强效水
        /// </summary>
        [Description("体力强效水 LifeElixir")]
        LifeElixir = 12,
        /// <summary>
        /// 魔力强效水
        /// </summary>
        [Description("魔力强效水 ManaElixir")]
        ManaElixir = 13,
        /// <summary>
        /// 自然强效水
        /// </summary>
        [Description("自然强效水 NatureElixir")]
        NatureElixir = 14,
        /// <summary>
        /// 灵魂强效水
        /// </summary>
        [Description("灵魂强效水 SpiritElixir")]
        SpiritElixir = 15,
        /// <summary>
        /// 黑铁矿
        /// </summary>
        [Description("黑铁矿 BlackIronOre")]
        BlackIronOre = 20,
        /// <summary>
        /// 金矿
        /// </summary>
        [Description("金矿 GoldOre")]
        GoldOre = 21,
        /// <summary>
        /// 金刚石
        /// </summary>
        [Description("金刚石 Diamond")]
        Diamond = 22,
        /// <summary>
        /// 银矿
        /// </summary>
        [Description("银矿 SilverOre")]
        SilverOre = 23,
        /// <summary>
        /// 铁矿
        /// </summary>
        [Description("铁矿 IronOre")]
        IronOre = 24,
        /// <summary>
        /// 刚玉石
        /// </summary>
        [Description("刚玉石 Corundum")]
        Corundum = 25,
        /// <summary>
        /// 解毒药剂
        /// </summary>
        [Description("解毒药剂 ElixirOfPurification")]
        ElixirOfPurification = 30,
        /// <summary>
        /// 回生丸
        /// </summary>
        [Description("回生丸 PillOfReincarnation")]
        PillOfReincarnation = 31,
        /// <summary>
        /// 结晶
        /// </summary>
        [Description("结晶 Crystal")]
        Crystal = 40,
        /// <summary>
        /// 制炼石
        /// </summary>
        [Description("制炼石 RefinementStone")]
        RefinementStone = 41,
        /// <summary>
        /// 初级碎片
        /// </summary>
        [Description("初级碎片 Fragment1")]
        Fragment1 = 42,
        /// <summary>
        /// 中级碎片
        /// </summary>
        [Description("中级碎片 Fragment2")]
        Fragment2 = 43,
        /// <summary>
        /// 高级碎片
        /// </summary>
        [Description("高级碎片 Fragment3")]
        Fragment3 = 44,
        /// <summary>
        /// 性别改变
        /// </summary>
        [Description("性别改变 GenderChange")]
        GenderChange = 50,
        /// <summary>
        /// 发型改变
        /// </summary>
        [Description("发型改变 HairChange")]
        HairChange = 51,
        /// <summary>
        /// 衣服染色
        /// </summary>
        [Description("衣服染色 ArmourDye")]
        ArmourDye = 52,
        /// <summary>
        /// 名字改变
        /// </summary>
        [Description("名字改变 NameChange")]
        NameChange = 53,
        /// <summary>
        /// 财富检查
        /// </summary>
        [Description("财富检查 FortuneChecker")]
        FortuneChecker = 54,
        /// <summary>
        /// 通用武器模板
        /// </summary>
        [Description("通用武器模板 WeaponTemplate")]
        WeaponTemplate = 60,
        /// <summary>
        /// 战士武器模板
        /// </summary>
        [Description("战士武器模板 WarriorWeapon")]
        WarriorWeapon = 61,
        /// <summary>
        /// 法师武器模板
        /// </summary>
        [Description("法师武器模板 WizardWeapon")]
        WizardWeapon = 63,
        /// <summary>
        /// 道士武器模板
        /// </summary>
        [Description("道士武器模板 TaoistWeapon")]
        TaoistWeapon = 64,
        /// <summary>
        /// 刺客武器模板
        /// </summary>
        [Description("刺客武器模板 AssassinWeapon")]
        AssassinWeapon = 65,
        /// <summary>
        /// 黄色饰品
        /// </summary>
        [Description("黄色饰品 YellowSlot")]
        YellowSlot = 70,
        /// <summary>
        /// 蓝色饰品
        /// </summary>
        [Description("蓝色饰品 BlueSlot")]
        BlueSlot = 71,
        /// <summary>
        /// 红色饰品
        /// </summary>
        [Description("红色饰品 RedSlot")]
        RedSlot = 72,
        /// <summary>
        /// 紫色饰品
        /// </summary>
        [Description("紫色饰品 PurpleSlot")]
        PurpleSlot = 73,
        /// <summary>
        /// 绿色饰品
        /// </summary>
        [Description("绿色饰品 GreenSlot")]
        GreenSlot = 74,
        /// <summary>
        /// 灰色饰品
        /// </summary>
        [Description("灰色饰品 GreySlot")]
        GreySlot = 75,
        /// <summary>
        /// 足球圣诞衣服
        /// </summary>
        [Description("足球圣诞衣服 FootballArmour")]
        FootballArmour = 80,
        /// <summary>
        /// 口哨
        /// </summary>
        [Description("口哨 FootBallWhistle")]
        FootBallWhistle = 81,
        /// <summary>
        /// 武器属性提取
        /// </summary>
        [Description("武器属性提取 StatExtractor")]
        StatExtractor = 90,
        /// <summary>
        /// 无法提取定义
        /// </summary>
        [Description("无法提取定义 SpiritBlade")]
        SpiritBlade = 91,
        /// <summary>
        /// 精炼属性提取
        /// </summary>
        [Description("精炼属性提取 RefineExtractor")]
        RefineExtractor = 92,
        /// <summary>
        /// 元宝
        /// </summary>
        [Description("元宝 GameGold")]
        GameGold = 100,
        /// <summary>
        /// 声望FP
        /// </summary>
        [Description("声望FP Prestige")]
        Prestige = 101,
        /// <summary>
        /// 贡献CP
        /// </summary>
        [Description("贡献CP Contribute")]
        Contribute = 102,
        /// <summary>
        /// 行会联盟条约
        /// </summary>
        [Description("行会联盟条约 GuildAllianceTreaty")]
        GuildAllianceTreaty = 103,
        /// <summary>
        /// 宠物自动粮仓解锁
        /// </summary>
        [Description("宠物自动粮仓解锁 CompanionAutoBarn")]
        CompanionAutoBarn = 104,
        /// <summary>
        /// 开孔类
        /// </summary>
        [Description("开孔类 DrillAddHole")]
        DrillAddHole = 105,
        /// <summary>
        /// 取下镶嵌宝石
        /// </summary>
        [Description("取下镶嵌宝石 DrillRemoveGem")]
        DrillRemoveGem = 106,
        /// <summary>
        /// 成功几率材料效果
        /// </summary>
        [Description("成功几率材料效果 ChanceMaterialEffect")]
        ChanceMaterialEffect = 107,
        /// <summary>
        /// 禁止镶嵌
        /// </summary>
        [Description("禁止镶嵌 NoGem")]
        NoGem = 108,
        /// <summary>
        /// 传送符
        /// </summary>
        [Description("传送符 DeliveryTally")]
        DeliveryTally = 109,
        /// <summary>
        /// 接受任务
        /// </summary>
        [Description("获得任务 QuestStarter")]
        QuestStarter = 110,
        /// <summary>
        /// 泰山BUFF幸运币
        /// </summary>
        [Description("泰山BUFF幸运币 LuckyCoins")]
        LuckyCoins = 111,
        /// <summary>
        /// 召唤宠物
        /// </summary>
        [Description("召唤宠物 CallPet")]
        CallPet = 112,
        /// <summary>
        /// 衣服属性提取器
        /// </summary>
        [Description("衣服属性提取器 ArmourExtractor")]
        ArmourExtractor = 113,
        /// <summary>
        /// 项链属性提取器
        /// </summary>
        [Description("项链属性提取器 NecklaceExtractor")]
        NecklaceExtractor = 114,
        /// <summary>
        /// 右手镯属性提取器
        /// </summary>
        [Description("右手镯属性提取器 BraceletRExtractor")]
        BraceletRExtractor = 115,
        /// <summary>
        /// 右戒指属性提取器
        /// </summary>
        [Description("右戒指属性提取器 RingRExtractor")]
        RingRExtractor = 116,
        /// <summary>
        /// 魔晶石
        /// </summary>
        [Description("魔晶石 Materia")]
        Materia = 117,
        /// <summary>
        /// 鞋子属性提取器
        /// </summary>
        [Description("鞋子属性提取器 ShoesExtractor")]
        ShoesExtractor = 118,
        /// <summary>
        /// 头盔属性提取器
        /// </summary>
        [Description("头盔属性提取器 HelmetExtractor")]
        HelmetExtractor = 119,
        /// <summary>
        /// 武器极品属性提取
        /// </summary>
        [Description("武器极品属性提取 StatExcellentExtractor")]
        StatExcellentExtractor = 120,
    }

    [Flags]
    /// <summary>
    /// 用户道具标签
    /// </summary>
    public enum UserItemFlags
    {
        /// <summary>
        /// 道具标签无
        /// </summary>
        [Description("空")]
        None = 0,
        /// <summary>
        /// 道具标签锁定
        /// </summary>
        [Description("锁定")]
        Locked = 1,
        /// <summary>
        /// 道具标签绑定
        /// </summary>
        [Description("绑定")]
        Bound = 2,
        /// <summary>
        /// 道具标签不能出售的
        /// </summary>
        [Description("不能出售")]
        Worthless = 4,
        /// <summary>
        /// 道具标签可以精炼
        /// </summary>
        [Description("可精炼")]
        Refinable = 8,
        /// <summary>
        /// 道具标签时间限制
        /// </summary>
        [Description("时间限制")]
        Expirable = 16,
        /// <summary>
        /// 道具标签任务道具
        /// </summary>
        [Description("任务道具")]
        QuestItem = 32,
        /// <summary>
        /// 道具标签GM制作
        /// </summary>
        [Description("GM制作")]
        GameMaster = 64,
        /// <summary>
        /// 道具标签结婚戒指
        /// </summary>
        [Description("结婚戒指")]
        Marriage = 128,
        /// <summary>
        /// 道具标签不可精炼
        /// </summary>
        [Description("不可精炼")]
        NonRefinable = 256,
        /// <summary>
        /// 道具标签拍卖行道具
        /// </summary>
        [Description("拍卖行道具")]
        AutionItem = 512,
    }

    /// <summary>
    /// 坐骑类型
    /// </summary>
    public enum HorseType : byte
    {
        /// <summary>
        /// 没有坐骑
        /// </summary>
        [Description("无")]
        None,
        /// <summary>
        /// 棕马
        /// </summary>
        [Description("棕马")]
        Brown,
        /// <summary>
        /// 白马
        /// </summary>
        [Description("白马")]
        White,
        /// <summary>
        /// 红马
        /// </summary>
        [Description("红马")]
        Red,
        /// <summary>
        /// 黑马
        /// </summary>
        [Description("黑马")]
        Black,
        /// <summary>
        /// 自定义马1
        /// </summary>
        [Description("自定义马1")]
        DiyHorse1,
        /// <summary>
        /// 自定义马2
        /// </summary>
        [Description("赤兔(幻化)")]
        DiyHorse2,
        /// <summary>
        /// 自定义马3
        /// </summary>
        [Description("自定义马3")]
        DiyHorse3,
        /// <summary>
        /// 自定义马4
        /// </summary>
        [Description("自定义马4")]
        DiyHorse4,
        /// <summary>
        /// 自定义马5
        /// </summary>
        [Description("自定义马5")]
        DiyHorse5,
        /// <summary>
        /// 自定义马6
        /// </summary>
        [Description("自定义马6")]
        DiyHorse6,
        /// <summary>
        /// 自定义马7
        /// </summary>
        [Description("自定义马7")]
        DiyHorse7,
        /// <summary>
        /// 自定义马8
        /// </summary>
        [Description("自定义马8")]
        DiyHorse8,
        /// <summary>
        /// 自定义马9
        /// </summary>
        [Description("自定义马9")]
        DiyHorse9,
        /// <summary>
        /// 自定义马10
        /// </summary>
        [Description("自定义马10")]
        DiyHorse10,
        /// <summary>
        /// 自定义马11
        /// </summary>
        [Description("自定义马11")]
        DiyHorse11,
        /// <summary>
        /// 自定义马12
        /// </summary>
        [Description("自定义马12")]
        DiyHorse12,
        /// <summary>
        /// 自定义马13
        /// </summary>
        [Description("自定义马13")]
        DiyHorse13,
        /// <summary>
        /// 自定义马14
        /// </summary>
        [Description("烈焰(幻化)")]
        DiyHorse14,
        /// <summary>
        /// 自定义马15
        /// </summary>
        [Description("自定义马15")]
        DiyHorse15,
        /// <summary>
        /// 自定义马16
        /// </summary>
        [Description("自定义马16")]
        DiyHorse16,
        /// <summary>
        /// 自定义马17
        /// </summary>
        [Description("自定义马17")]
        DiyHorse17,
        /// <summary>
        /// 自定义马18
        /// </summary>
        [Description("自定义马18")]
        DiyHorse18,
        /// <summary>
        /// 自定义马19
        /// </summary>
        [Description("自定义马19")]
        DiyHorse19,
        /// <summary>
        /// 自定义马20
        /// </summary>
        [Description("自定义马20")]
        DiyHorse20,
        /// <summary>
        /// 自定义马21
        /// </summary>
        [Description("自定义马21")]
        DiyHorse21,
        /// <summary>
        /// 自定义马22
        /// </summary>
        [Description("自定义马22")]
        DiyHorse22,
        /// <summary>
        /// 自定义马23
        /// </summary>
        [Description("追风(幻化)")]
        DiyHorse23,
        /// <summary>
        /// 自定义马24
        /// </summary>
        [Description("自定义马24")]
        DiyHorse24,
        /// <summary>
        /// 自定义马25
        /// </summary>
        [Description("自定义马25")]
        DiyHorse25,
        /// <summary>
        /// 自定义马26
        /// </summary>
        [Description("自定义马26")]
        DiyHorse26,
        /// <summary>
        /// 自定义马27
        /// </summary>
        [Description("自定义马27")]
        DiyHorse27,
        /// <summary>
        /// 自定义马28
        /// </summary>
        [Description("雷霆(幻化)")]
        DiyHorse28,
        /// <summary>
        /// 自定义马29
        /// </summary>
        [Description("自定义马29")]
        DiyHorse29,
        /// <summary>
        /// 自定义马30
        /// </summary>
        [Description("自定义马30")]
        DiyHorse30,
        /// <summary>
        /// 自定义马31
        /// </summary>
        [Description("自定义马31")]
        DiyHorse31,
        /// <summary>
        /// 自定义马32
        /// </summary>
        [Description("自定义马32")]
        DiyHorse32,
        /// <summary>
        /// 自定义马33
        /// </summary>
        [Description("自定义马33")]
        DiyHorse33,
        /// <summary>
        /// 自定义马34
        /// </summary>
        [Description("自定义马34")]
        DiyHorse34,
        /// <summary>
        /// 自定义马35
        /// </summary>
        [Description("自定义马35")]
        DiyHorse35,
        /// <summary>
        /// 自定义马36
        /// </summary>
        [Description("自定义马36")]
        DiyHorse36,
        /// <summary>
        /// 自定义马37
        /// </summary>
        [Description("自定义马37")]
        DiyHorse37,
        /// <summary>
        /// 自定义马38
        /// </summary>
        [Description("自定义马38")]
        DiyHorse38,
        /// <summary>
        /// 自定义马39
        /// </summary>
        [Description("自定义马39")]
        DiyHorse39,
        /// <summary>
        /// 自定义马40
        /// </summary>
        [Description("自定义马40")]
        DiyHorse40,
    }

    [Flags]
    /// <summary>
    /// 行会设置许可
    /// </summary>
    public enum GuildPermission
    {
        /// <summary>
        /// 无权限
        /// </summary>
        None = 0,
        /// <summary>
        /// 会长权限
        /// </summary>
        Leader = -1,
        /// <summary>
        /// 编辑通知权限
        /// </summary>
        EditNotice = 1,
        /// <summary>
        /// 添加成员权限
        /// </summary>
        AddMember = 2,
        /// <summary>
        /// 删除成员权限
        /// </summary>
        RemoveMember = 4,
        /// <summary>
        /// 行会仓库使用权限
        /// </summary>
        Storage = 8,
        /// <summary>
        /// 行会基金权限
        /// </summary>
        FundsRepair = 16,
        /// <summary>
        /// 基金用户权限
        /// </summary>
        FundsMerchant = 32,
        /// <summary>
        /// 基金市场使用权限
        /// </summary>
        FundsMarket = 64,
        /// <summary>
        /// 行会战权限
        /// </summary>
        StartWar = 128,
        /// <summary>
        /// 行会联盟权限
        /// </summary>
        Alliance = 256,
    }

    [Flags]
    /// <summary>
    /// 任务图标
    /// </summary>
    public enum QuestIcon
    {
        /// <summary>
        /// 空置
        /// </summary>
        None = 0,
        /// <summary>
        /// 新任务
        /// </summary>
        NewQuest = 1,
        /// <summary>
        /// 任务进行
        /// </summary>
        QuestIncomplete = 2,
        /// <summary>
        /// 任务完成
        /// </summary>
        QuestComplete = 4,
        /// <summary>
        /// 新的可重复
        /// </summary>
        NewRepeatable = 8,
        /// <summary>
        /// 可重复完成
        /// </summary>
        RepeatableComplete = 16,
    }

    /// <summary>
    /// 任务类型
    /// </summary>
    [Lang("任务类型")]
    public enum QuestType
    {
        /// <summary>
        /// 空置
        /// </summary>
        [Description("普通")]
        None = 0,
        /// <summary>
        /// 主线
        /// </summary>
        [Description("主线")]
        Main = 1,
        /// <summary>
        /// 剧情
        /// </summary>
        [Description("剧情")]
        Story = 2,
        /// <summary>
        /// 万事通任务，服务端设置每日可接任务次数，从所有每日任务里随机获取任务
        /// </summary>
        [Description("万事通")]
        Daily = 3,
        /// <summary>
        /// 每日指定NPC或者道具双击获得任务，可重复完成
        /// </summary>
        [Description("重复")]
        Repeatable = 4,
        /// <summary>
        /// 系统定时开启，同时显示活动NPC
        /// </summary>
        [Description("活动(未完成)")]
        Event = 5,
        /// <summary>
        /// 奇遇, 满足特定条件开启
        /// </summary>
        [Description("奇遇")]
        Hidden = 6,
    }

    /// <summary>
    /// 地图链接特殊条件
    /// </summary>
    public enum MovementEffect
    {
        /// <summary>
        /// 地图链接无条件
        /// </summary>
        None = 0,
        /// <summary>
        /// 地图链接特殊修理
        /// </summary>
        [Description("特殊修理")]
        SpecialRepair = 1,

    }

    /// <summary>
    /// 自动辅助设置定义
    /// </summary>
    public enum AutoSetConf
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 免助跑
        /// </summary>
        SetRunCheckBox,
        /// <summary>
        /// 免SHIFT
        /// </summary>
        SetShiftBox,
        /// <summary>
        /// 综合数显
        /// </summary>
        SetDisplayBox,
        /// <summary>
        /// 免蜡
        /// </summary>
        SetBrightBox,
        /// <summary>
        /// 清理尸体
        /// </summary>
        SetCorpseBox,
        /// <summary>
        /// 自动烈火
        /// </summary>
        SetFlamingSwordBox,
        /// <summary>
        /// 自动翔空
        /// </summary>
        SetDragobRiseBox,
        /// <summary>
        /// 自动莲月
        /// </summary>
        SetBladeStormBox,
        /// <summary>
        /// 自动屠龙斩
        /// </summary>
        SetMaelstromBlade,
        /// <summary>
        /// 自动魔法盾
        /// </summary>
        SetMagicShieldBox,
        /// <summary>
        /// 自动凝血
        /// </summary>
        SetRenounceBox,
        /// <summary>
        /// 自动换毒符
        /// </summary>
        SetPoisonDustBox,
        /// <summary>
        /// 自动阴阳盾
        /// </summary>
        SetCelestialBox,
        /// <summary>
        /// 自动四花
        /// </summary>
        SetFourFlowersBox,
        /// <summary>
        /// 自动技能1
        /// </summary>
        SetMagicskillsBox,
        /// <summary>
        /// 自动技能2
        /// </summary>
        SetMagicskills1Box,
        /// <summary>
        /// 自动挂机
        /// </summary>
        SetAutoOnHookBox,
        /// <summary>
        /// 自动捡取
        /// </summary>
        SetPickUpBox,
        /// <summary>
        /// 自动上毒
        /// </summary>
        SetAutoPoisonBox,
        /// <summary>
        /// 自动躲避
        /// </summary>
        SetAutoAvoidBox,
        /// <summary>
        /// 死亡复活
        /// </summary>
        SetDeathResurrectionBox,
        /// <summary>
        /// 单技能挂机
        /// </summary>
        SetSingleHookSkillsBox,
        /// <summary>
        /// 群体技能挂机
        /// </summary>
        SetGroupHookSkillsBox,
        /// <summary>
        /// 召唤技能
        /// </summary>
        SetSummoningSkillsBox,
        /// <summary>
        /// 随机保护
        /// </summary>
        SetRandomItemBox,
        /// <summary>
        /// 回城保护
        /// </summary>
        SetHomeItemBox,
        /// <summary>
        /// 自动铁布衫
        /// </summary>
        SetDefianceBox,
        /// <summary>
        /// 自动破血狂杀
        /// </summary>
        SetMightBox,
        /// <summary>
        /// 显示血量
        /// </summary>
        SetShowHealth,
        /// <summary>
        /// 自动风之闪避
        /// </summary>
        SetEvasionBox,
        /// <summary>
        /// 自动风之守护
        /// </summary>
        SerRagingWindBox,
        /// <summary>
        /// 雷达显示
        /// </summary>
        SetDisplayObject,
        /// <summary>
        /// 设置最大范围,必须写在最后一位，追加枚举写前面
        /// </summary>
        SetMaxConf
    }

    /// <summary>
    /// 快捷键设置
    /// </summary>
    public enum SpellKey : byte
    {
        None,

        [Description("F1")]
        Spell01,
        [Description("F2")]
        Spell02,
        [Description("F3")]
        Spell03,
        [Description("F4")]
        Spell04,
        [Description("F5")]
        Spell05,
        [Description("F6")]
        Spell06,
        [Description("F7")]
        Spell07,
        [Description("F8")]
        Spell08,
        [Description("F9")]
        Spell09,
        [Description("F10")]
        Spell10,
        [Description("F11")]
        Spell11,
        [Description("F12")]
        Spell12,

        [Description("F1")]
        Spell13,
        [Description("F2")]
        Spell14,
        [Description("F3")]
        Spell15,
        [Description("F4")]
        Spell16,
        [Description("F5")]
        Spell17,
        [Description("F6")]
        Spell18,
        [Description("F7")]
        Spell19,
        [Description("F8")]
        Spell20,
        [Description("F9")]
        Spell21,
        [Description("F10")]
        Spell22,
        [Description("F11")]
        Spell23,
        [Description("F12")]
        Spell24,
    }

    /// <summary>
    /// 怪物标志
    /// </summary>
    public enum MonsterFlag
    {
        /// <summary>
        /// 怪物标识空
        /// </summary>
        None = 0,
        /// <summary>
        /// 变异骷髅
        /// </summary>
        [Description("变异骷髅")]
        Skeleton = 1,
        /// <summary>
        /// 超强骷髅
        /// </summary>
        [Description("超强骷髅")]
        JinSkeleton = 2,
        /// <summary>
        /// 神兽
        /// </summary>
        [Description("神兽")]
        Shinsu = 3,
        /// <summary>
        /// 炎魔
        /// </summary>
        [Description("炎魔")]
        InfernalSoldier = 4,
        /// <summary>
        /// 稻草人
        /// </summary>
        [Description("稻草人")]
        Scarecrow = 5,
        /// <summary>
        /// 替身木偶
        /// </summary>
        [Description("替身木偶")]
        SummonPuppet = 6,
        /// <summary>
        /// 镜像
        /// </summary>
        [Description("镜像")]
        MirrorImage = 7,
        /// <summary>
        /// 爆裂蜘蛛
        /// </summary>
        [Description("爆裂蜘蛛")]
        Larva = 100,
        /// <summary>
        /// 蝙蝠
        /// </summary>
        [Description("蝙蝠")]
        LesserWedgeMoth = 110,
        /// <summary>
        /// 祖玛弓箭手
        /// </summary>
        [Description("祖玛弓箭手")]
        ZumaArcherMonster = 120,
        /// <summary>
        /// 祖玛卫士
        /// </summary>
        [Description("祖玛卫士")]
        ZumaGuardianMonster = 121,
        /// <summary>
        /// 祖玛雕像
        /// </summary>
        [Description("祖玛雕像")]
        ZumaFanaticMonster = 122,
        /// <summary>
        /// 护法天
        /// </summary>
        [Description("护法天")]
        ZumaKeeperMonster = 123,
        /// <summary>
        /// 骷髅弓箭手
        /// </summary>
        [Description("骷髅弓箭手")]
        BoneArcher = 130,
        /// <summary>
        /// 骷髅武将
        /// </summary>
        [Description("骷髅武将")]
        BoneCaptain = 131,
        /// <summary>
        /// 骷髅武士
        /// </summary>
        [Description("骷髅武士")]
        BoneBladesman = 132,
        /// <summary>
        /// 骷髅士兵
        /// </summary>
        [Description("骷髅士兵")]
        BoneSoldier = 133,
        /// <summary>
        /// 骨鬼将
        /// </summary>
        [Description("骨鬼将")]
        SkeletonEnforcer = 134,
        /// <summary>
        /// 幼铗虫
        /// </summary>
        [Description("幼铗虫")]
        MatureEarwig = 140,
        /// <summary>
        /// 黄甲虫
        /// </summary>
        [Description("黄甲虫")]
        GoldenArmouredBeetle = 141,
        /// <summary>
        /// 百足虫
        /// </summary>
        [Description("百足虫")]
        Millipede = 142,
        /// <summary>
        /// 凶恶火灵牛鬼
        /// </summary>
        [Description("凶恶火灵牛鬼")]
        FerociousFlameDemon = 150,
        /// <summary>
        /// 火灵牛鬼
        /// </summary>
        [Description("火灵牛鬼")]
        FlameDemon = 151,
        /// <summary>
        /// 骷髅魔卒
        /// </summary>
        [Description("骷髅魔卒")]
        GoruSpearman = 160,
        /// <summary>
        /// 超强骷髅弓箭手
        /// </summary>
        [Description("超强骷髅弓箭手")]
        GoruArcher = 161,
        /// <summary>
        /// 骷髅鬼将
        /// </summary>
        [Description("骷髅鬼将")]
        GoruGeneral = 162,
        /// <summary>
        /// 赤龙魔王
        /// </summary>
        [Description("赤龙魔王")]
        DragonLord = 170,
        /// <summary>
        /// 雾影魔卒
        /// </summary>
        [Description("雾影魔卒")]
        OYoungBeast = 171,
        /// <summary>
        /// 阎昆魔女
        /// </summary>
        [Description("阎昆魔女")]
        YumgonWitch = 172,
        /// <summary>
        /// 魔小将
        /// </summary>
        [Description("魔小将")]
        MaWarden = 173,
        /// <summary>
        /// 魔大将
        /// </summary>
        [Description("魔大将")]
        MaWarlord = 174,
        /// <summary>
        /// 真幻鬼
        /// </summary>
        [Description("真幻鬼")]
        JinhwanSpirit = 175,
        /// <summary>
        /// 真幻鬼婢
        /// </summary>
        [Description("真幻鬼婢")]
        JinhwanGuardian = 176,
        /// <summary>
        /// 雾影魔将
        /// </summary>
        [Description("雾影魔将")]
        OyoungGeneral = 177,
        /// <summary>
        /// 阎昆魔君
        /// </summary>
        [Description("阎昆魔君")]
        YumgonGeneral = 178,
        /// <summary>
        /// 海盗武将
        /// </summary>
        [Description("海盗武将")]
        BanyoCaptain = 180,
        /// <summary>
        /// 魔法师
        /// </summary>
        [Description("魔法师")]
        SamaSorcerer = 190,
        /// <summary>
        /// 灵石类
        /// </summary>
        [Description("灵石类")]
        BloodStone = 191,
        /// <summary>
        /// 水晶火虫
        /// </summary>
        [Description("水晶火虫")]
        QuartzPinkBat = 200,
        /// <summary>
        /// 水晶蝙蝠
        /// </summary>
        [Description("水晶蝙蝠")]
        QuartzBlueBat = 201,
        /// <summary>
        /// 水晶魔像
        /// </summary>
        [Description("水晶魔像")]
        QuartzBlueCrystal = 202,
        /// <summary>
        /// 水晶魔法狂徒
        /// </summary>
        [Description("水晶魔法狂徒")]
        QuartzRedHood = 203,
        /// <summary>
        /// 水晶小玄武
        /// </summary>
        [Description("水晶小玄武")]
        QuartzMiniTurtle = 204,
        /// <summary>
        /// 水晶玄武
        /// </summary>
        [Description("水晶玄武")]
        QuartzTurtleSub = 205,
        /// <summary>
        /// 魔气僵尸-魔道
        /// </summary>
        [Description("魔气僵尸-魔道")]
        Sacrafice = 210,
        /// <summary>
        /// 地狱蝙蝠
        /// </summary>
        [Description("地狱蝙蝠")]
        HellishBat = 211,
        /// <summary>
        /// 棕马
        /// </summary>
        [Description("棕马")]
        BrownHorse = 301,
        /// <summary>
        /// 白马
        /// </summary>
        [Description("白马")]
        WhiteHorse = 302,
        /// <summary>
        /// 红马
        /// </summary>
        [Description("红马")]
        RedHorse = 303,
        /// <summary>
        /// 黑马
        /// </summary>
        [Description("黑马")]
        BlackHorse = 304,
        /// <summary>
        /// 自定义马1
        /// </summary>
        [Description("自定义马1")]
        DiyHorse1 = 305,
        /// <summary>
        /// 自定义马2
        /// </summary>
        [Description("赤兔(幻化)")]
        DiyHorse2 = 306,
        /// <summary>
        /// 自定义马3
        /// </summary>
        [Description("自定义马3")]
        DiyHorse3 = 307,
        /// <summary>
        /// 自定义马4
        /// </summary>
        [Description("自定义马4")]
        DiyHorse4 = 308,
        /// <summary>
        /// 自定义马5
        /// </summary>
        [Description("自定义马5")]
        DiyHorse5 = 309,
        /// <summary>
        /// 自定义马6
        /// </summary>
        [Description("自定义马6")]
        DiyHorse6 = 310,
        /// <summary>
        /// 自定义马7
        /// </summary>
        [Description("自定义马7")]
        DiyHorse7 = 311,
        /// <summary>
        /// 自定义马8
        /// </summary>
        [Description("自定义马8")]
        DiyHorse8 = 312,
        /// <summary>
        /// 自定义马9
        /// </summary>
        [Description("自定义马9")]
        DiyHorse9 = 313,
        /// <summary>
        /// 自定义马10
        /// </summary>
        [Description("自定义马10")]
        DiyHorse10 = 314,
        /// <summary>
        /// 自定义马11
        /// </summary>
        [Description("自定义马11")]
        DiyHorse11 = 315,
        /// <summary>
        /// 自定义马12
        /// </summary>
        [Description("自定义马12")]
        DiyHorse12 = 316,
        /// <summary>
        /// 自定义马13
        /// </summary>
        [Description("自定义马13")]
        DiyHorse13 = 317,
        /// <summary>
        /// 自定义马14
        /// </summary>
        [Description("烈焰(幻化)")]
        DiyHorse14 = 318,
        /// <summary>
        /// 自定义马15
        /// </summary>
        [Description("自定义马15")]
        DiyHorse15 = 319,
        /// <summary>
        /// 自定义马16
        /// </summary>
        [Description("自定义马16")]
        DiyHorse16 = 320,
        /// <summary>
        /// 自定义马17
        /// </summary>
        [Description("自定义马17")]
        DiyHorse17 = 321,
        /// <summary>
        /// 自定义马18
        /// </summary>
        [Description("自定义马18")]
        DiyHorse18 = 322,
        /// <summary>
        /// 自定义马19
        /// </summary>
        [Description("自定义马19")]
        DiyHorse19 = 323,
        /// <summary>
        /// 自定义马20
        /// </summary>
        [Description("自定义马20")]
        DiyHorse20 = 324,
        /// <summary>
        /// 自定义马21
        /// </summary>
        [Description("自定义马21")]
        DiyHorse21 = 325,
        /// <summary>
        /// 自定义马22
        /// </summary>
        [Description("自定义马22")]
        DiyHorse22 = 326,
        /// <summary>
        /// 自定义马23
        /// </summary>
        [Description("追风(幻化)")]
        DiyHorse23 = 327,
        /// <summary>
        /// 自定义马24
        /// </summary>
        [Description("自定义马24")]
        DiyHorse24 = 328,
        /// <summary>
        /// 自定义马25
        /// </summary>
        [Description("自定义马25")]
        DiyHorse25 = 329,
        /// <summary>
        /// 自定义马26
        /// </summary>
        [Description("自定义马26")]
        DiyHorse26 = 330,
        /// <summary>
        /// 自定义马27
        /// </summary>
        [Description("自定义马27")]
        DiyHorse27 = 331,
        /// <summary>
        /// 自定义马28
        /// </summary>
        [Description("雷霆(幻化)")]
        DiyHorse28 = 332,
        /// <summary>
        /// 自定义马29
        /// </summary>
        [Description("自定义马29")]
        DiyHorse29 = 333,
        /// <summary>
        /// 自定义马30
        /// </summary>
        [Description("自定义马30")]
        DiyHorse30 = 334,
        /// <summary>
        /// 自定义马31
        /// </summary>
        [Description("自定义马31")]
        DiyHorse31 = 335,
        /// <summary>
        /// 自定义马32
        /// </summary>
        [Description("自定义马32")]
        DiyHorse32 = 336,
        /// <summary>
        /// 自定义马33
        /// </summary>
        [Description("自定义马33")]
        DiyHorse33 = 337,
        /// <summary>
        /// 自定义马34
        /// </summary>
        [Description("自定义马34")]
        DiyHorse34 = 338,
        /// <summary>
        /// 自定义马35
        /// </summary>
        [Description("自定义马35")]
        DiyHorse35 = 339,
        /// <summary>
        /// 自定义马36
        /// </summary>
        [Description("自定义马36")]
        DiyHorse36 = 340,
        /// <summary>
        /// 自定义马37
        /// </summary>
        [Description("自定义马37")]
        DiyHorse37 = 341,
        /// <summary>
        /// 自定义马38
        /// </summary>
        [Description("自定义马38")]
        DiyHorse38 = 342,
        /// <summary>
        /// 自定义马39
        /// </summary>
        [Description("自定义马39")]
        DiyHorse39 = 343,
        /// <summary>
        /// 自定义马40
        /// </summary>
        [Description("自定义马40")]
        DiyHorse40 = 344,
    }
    /// <summary>
    /// 自定义魔法方向
    /// </summary>
    public enum MagicDir
    {
        /// <summary>
        /// 无方向
        /// </summary>
        [Description("空置")]
        None,
        /// <summary>
        /// 八个方向线性魔法
        /// </summary>
        [Description("线性魔法8方向")]
        Dir8,
        /// <summary>
        /// 全方位魔法效果
        /// </summary>
        [Description("全方位魔法方向")]
        Dir16,
    }

    #region Packet Enums

    /// <summary>
    /// 新建账号
    /// </summary>
    public enum NewAccountResult : byte
    {
        /// <summary>
        /// 系统禁止新建账号
        /// </summary>
        Disabled,
        /// <summary>
        /// 邮箱格式错误
        /// </summary>
        BadEMail,
        /// <summary>
        /// 密码格式错误
        /// </summary>
        BadPassword,
        /// <summary>
        /// 姓名格式错误
        /// </summary>
        BadRealName,
        /// <summary>
        /// 邮箱已经被使用
        /// </summary>
        AlreadyExists,
        /// <summary>
        /// 推荐人错误
        /// </summary>
        BadReferral,
        /// <summary>
        /// 找不到该推荐人
        /// </summary>
        ReferralNotFound,
        /// <summary>
        /// 推荐人未激活
        /// </summary>
        ReferralNotActivated,
        /// <summary>
        /// 无效的邀请码
        /// </summary>
        BadInviteCode,
        /// <summary>
        /// 新建账号成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 修改密码
    /// </summary>
    public enum ChangePasswordResult : byte
    {
        /// <summary>
        /// 系统禁止修改密码
        /// </summary>
        Disabled,
        /// <summary>
        /// 邮箱格式错误
        /// </summary>
        BadEMail,
        /// <summary>
        /// 当前密码格式错误
        /// </summary>
        BadCurrentPassword,
        /// <summary>
        /// 新密码格式错误
        /// </summary>
        BadNewPassword,
        /// <summary>
        /// 账号不存在
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// 账号未激活
        /// </summary>
        AccountNotActivated,
        /// <summary>
        /// 密码错误
        /// </summary>
        WrongPassword,
        /// <summary>
        /// 被禁止登陆的账号
        /// </summary>
        Banned,
        /// <summary>
        /// 修改密码成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 请求重置密码
    /// </summary>
    public enum RequestPasswordResetResult : byte
    {
        /// <summary>
        /// 系统禁止重置密码
        /// </summary>
        Disabled,
        /// <summary>
        /// 邮箱地址错误
        /// </summary>
        BadEMail,
        /// <summary>
        /// 账号不存在
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// 账号未激活
        /// </summary>
        AccountNotActivated,
        /// <summary>
        /// 请求延迟
        /// </summary>
        ResetDelay,
        /// <summary>
        /// 账号被禁止
        /// </summary>
        Banned,
        /// <summary>
        /// 请求重置密码成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    public enum ResetPasswordResult : byte
    {
        /// <summary>
        /// 系统禁止手动重置密码
        /// </summary>
        Disabled,
        /// <summary>
        /// 无法找到账号
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// 新密码格式错误
        /// </summary>
        BadNewPassword,
        /// <summary>
        /// 密匙过期
        /// </summary>
        KeyExpired,
        /// <summary>
        /// 重置密码成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 激活
    /// </summary>
    public enum ActivationResult : byte
    {
        /// <summary>
        /// 系统禁止手动激活
        /// </summary>
        Disabled,
        /// <summary>
        /// 无法找到账号
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// 成功激活
        /// </summary>
        Success,
    }
    /// <summary>
    /// 重置激活码
    /// </summary>
    public enum RequestActivationKeyResult : byte
    {
        /// <summary>
        /// 当前禁止申请激活码
        /// </summary>
        Disabled,
        /// <summary>
        /// 邮件错误
        /// </summary>
        BadEMail,
        /// <summary>
        /// 账号不存在
        /// </summary>
        AccountNotFound,
        /// <summary>
        /// 账号已激活
        /// </summary>
        AlreadyActivated,
        /// <summary>
        /// 申请延迟
        /// </summary>
        RequestDelay,
        /// <summary>
        /// 成功申请激活码
        /// </summary>
        Success,
    }
    /// <summary>
    /// 登录结果
    /// </summary>
    public enum LoginResult : byte
    {
        /// <summary>
        /// 当前禁止登陆
        /// </summary>
        Disabled,
        /// <summary>
        /// 邮箱名不对
        /// </summary>
        BadEMail,
        /// <summary>
        /// 密码格式不对
        /// </summary>
        BadPassword,
        /// <summary>
        /// 账号不存在
        /// </summary>
        AccountNotExists,
        /// <summary>
        /// 账号未激活
        /// </summary>
        AccountNotActivated,
        /// <summary>
        /// 密码错误
        /// </summary>
        WrongPassword,
        /// <summary>
        /// 账号被封
        /// </summary>
        Banned,
        /// <summary>
        /// 账号正在使用
        /// </summary>
        AlreadyLoggedIn,
        /// <summary>
        /// 账号正在使用并重新发送密码
        /// </summary>
        AlreadyLoggedInPassword,
        /// <summary>
        /// 账号管理员登陆中
        /// </summary>
        AlreadyLoggedInAdmin,
        /// <summary>
        /// 账号成功登陆
        /// </summary>
        Success,
        /// <summary>
        /// 锁IP
        /// </summary>
        LockIp,
        /// <summary>
        /// 达到多开上限
        /// </summary>
        MaxConnectionExceeded,
    }
    /// <summary>
    /// 新建角色
    /// </summary>
    public enum NewCharacterResult : byte
    {
        /// <summary>
        /// 禁止新建角色
        /// </summary>
        Disabled,
        /// <summary>
        /// 角色名不符合要求
        /// </summary>
        BadCharacterName,
        /// <summary>
        /// 性别无效
        /// </summary>
        BadGender,
        /// <summary>
        /// 职业无效
        /// </summary>
        BadClass,
        /// <summary>
        /// 头发类型不对
        /// </summary>
        BadHairType,
        /// <summary>
        /// 头发颜色不对
        /// </summary>
        BadHairColour,
        /// <summary>
        /// 衣服颜色不对
        /// </summary>
        BadArmourColour,
        /// <summary>
        /// 职业无法使用
        /// </summary>
        ClassDisabled,
        /// <summary>
        /// 创建人数限制
        /// </summary>
        MaxCharacters,
        /// <summary>
        /// 角色名存在
        /// </summary>
        AlreadyExists,
        /// <summary>
        /// 角色创建成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 删除角色
    /// </summary>
    public enum DeleteCharacterResult : byte
    {
        /// <summary>
        /// 禁止删除角色
        /// </summary>
        Disabled,
        /// <summary>
        /// 角色已删除
        /// </summary>
        AlreadyDeleted,
        /// <summary>
        /// 找不到角色
        /// </summary>
        NotFound,
        /// <summary>
        /// 删除成功
        /// </summary>
        Success
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    public enum StartGameResult : byte
    {
        /// <summary>
        /// 禁止开始游戏
        /// </summary>
        Disabled,
        /// <summary>
        /// 删除角色无法开始游戏
        /// </summary>
        Deleted,
        /// <summary>
        /// 刚退出游戏角色延迟
        /// </summary>
        Delayed,
        /// <summary>
        /// 无法生成角色
        /// </summary>
        UnableToSpawn,
        /// <summary>
        /// 找不到角色
        /// </summary>
        NotFound,
        /// <summary>
        /// 可以开始游戏
        /// </summary>
        Success,
        /// <summary>
        /// 出售中角色
        /// </summary>
        Sell
    }
    /// <summary>
    /// 断开游戏
    /// </summary>
    public enum DisconnectReason : byte
    {
        /// <summary>
        /// 未知原因
        /// </summary>
        Unknown,
        /// <summary>
        /// 连接超时
        /// </summary>
        TimedOut,
        /// <summary>
        /// 错误的版本
        /// </summary>
        WrongVersion,
        /// <summary>
        /// 服务器关闭
        /// </summary>
        ServerClosing,
        /// <summary>
        /// 其他用户登录
        /// </summary>
        AnotherUser,
        /// <summary>
        /// 其他用户修改密码
        /// </summary>
        AnotherUserPassword,
        /// <summary>
        /// 管理员登录
        /// </summary>
        AnotherUserAdmin,
        /// <summary>
        /// 禁止登录
        /// </summary>
        Banned,
        /// <summary>
        /// 服务器奔溃
        /// </summary>
        Crashed,
        /// <summary>
        /// 辅助监测
        /// </summary>
        PlugInDetection,
        /// <summary>
        /// ClientSystemDB错误的版本
        /// </summary>
        WrongClientSystemDBVersion,
    }
    public enum Platform : byte
    {
        /// <summary>
        /// 桌面
        /// </summary>
        Desktop,
        /// <summary>
        /// 手机
        /// </summary>
        Mobile,
    }
    #endregion

    #region Sound
    /// <summary>
    /// 声音索引序号
    /// </summary>
    public enum SoundIndex
    {
        None,
        LoginScene,
        SelectScene,

        // 地区音乐,
        B000,
        B2,
        B8,
        B009D,
        B009N,
        B0014D,
        B0014N,
        B100,
        B122,
        B300,
        B400,
        B14001,
        BD00,
        BD01,
        BD02,
        BD041,
        BD042,
        BD50,
        BD60,
        BD70,
        BD99,
        BD100,
        BD101,
        BD210,
        BD211,
        BDUnderseaCave,
        BDUnderseaCaveBoss,
        D3101,
        D3102,
        D3400,
        Dungeon_1,
        Dungeon_2,
        ID1_001,
        ID1_002,
        ID1_003,
        TS001,
        TS002,
        TS003,

        ButtonA,
        ButtonB,
        ButtonC,

        SelectWarriorMale,
        SelectWarriorFemale,
        SelectWizardMale,
        SelectWizardFemale,
        SelectTaoistMale,
        SelectTaoistFemale,
        SelectAssassinMale,
        SelectAssassinFemale,

        TeleportOut,
        TeleportIn,

        ItemPotion,
        ItemWeapon,
        ItemArmour,
        ItemRing,
        ItemBracelet,
        ItemNecklace,
        ItemHelmet,
        ItemShoes,
        ItemDefault,

        GoldPickUp,
        GoldGained,

        DaggerSwing,
        WoodSwing,
        IronSwordSwing,
        ShortSwordSwing,
        AxeSwing,
        ClubSwing,
        WandSwing,
        FistSwing,
        GlaiveAttack,
        ClawAttack,

        GenericStruckPlayer,
        GenericStruckMonster,

        Foot1,
        Foot2,
        Foot3,
        Foot4,
        HorseWalk1,
        HorseWalk2,
        HorseRun,

        MaleStruck,
        FemaleStruck,

        MaleDie,
        FemaleDie,

        #region Magics 技能声效

        SlayingMale,
        SlayingFemale,

        EnergyBlast,

        HalfMoon,

        FlamingSword,

        DragonRise,

        BladeStorm,

        DestructiveBlow,

        DefianceStart,

        AssaultStart,

        SwiftBladeEnd,

        FireBallStart,
        FireBallTravel,
        FireBallEnd,

        ThunderBoltStart,
        ThunderBoltTravel,
        ThunderBoltEnd,

        IceBoltStart,
        IceBoltTravel,
        IceBoltEnd,

        GustBlastStart,
        GustBlastTravel,
        GustBlastEnd,

        RepulsionEnd,

        ElectricShockStart,
        ElectricShockEnd,

        GreaterFireBallStart,
        GreaterFireBallTravel,
        GreaterFireBallEnd,

        LightningStrikeStart,
        LightningStrikeEnd,

        GreaterIceBoltStart,
        GreaterIceBoltTravel,
        GreaterIceBoltEnd,

        CycloneStart,
        CycloneEnd,

        TeleportationStart,

        LavaStrikeStart,
        // LavaStrikeEnd,

        LightningBeamEnd,

        FrozenEarthStart,
        FrozenEarthEnd,

        BlowEarthStart,
        BlowEarthEnd,
        BlowEarthTravel,

        FireWallStart,
        FireWallEnd,

        ExpelUndeadStart,
        ExpelUndeadEnd,

        MagicShieldStart,

        FireStormStart,
        FireStormEnd,

        LightningWaveStart,
        LightningWaveEnd,

        IceStormStart,
        IceStormEnd,

        DragonTornadoStart,
        DragonTornadoEnd,

        GreaterFrozenEarthStart,
        GreaterFrozenEarthEnd,

        ChainLightningStart,
        ChainLightningEnd,

        FrostBiteStart,

        HealStart,
        HealEnd,

        PoisonDustStart,
        PoisonDustEnd,

        ExplosiveTalismanStart,
        ExplosiveTalismanTravel,
        ExplosiveTalismanEnd,

        HolyStrikeStart,
        HolyStrikeTravel,
        HolyStrikeEnd,

        ImprovedHolyStrikeStart,
        ImprovedHolyStrikeTravel,
        ImprovedHolyStrikeEnd,

        MagicResistanceTravel,
        MagicResistanceEnd,

        ResilienceTravel,
        ResilienceEnd,

        ShacklingTalismanStart,
        ShacklingTalismanEnd,

        SummonSkeletonStart,
        SummonSkeletonEnd,

        InvisibilityEnd,

        MassInvisibilityTravel,
        MassInvisibilityEnd,

        TaoistCombatKickStart,

        MassHealStart,
        MassHealEnd,

        BloodLustTravel,
        BloodLustEnd,

        ResurrectionStart,

        PurificationStart,
        PurificationEnd,

        SummonShinsuStart,
        SummonShinsuEnd,

        StrengthOfFaithStart,
        StrengthOfFaithEnd,

        PoisonousCloudStart,

        CloakStart,

        WraithGripStart,
        WraithGripEnd,

        HellFireStart,

        FullBloom,
        WhiteLotus,
        RedLotus,
        SweetBrier,
        SweetBrierMale,
        SweetBrierFemale,

        Karma,

        TheNewBeginning,

        SummonPuppet,

        DanceOfSwallowsEnd,
        DragonRepulseStart,   //狂涛泉涌
        AbyssStart,
        FlashOfLightEnd,
        EvasionStart,
        RagingWindStart,

        #endregion

        #region Monsters  怪物声效

        ChickenAttack,
        ChickenStruck,
        ChickenDie,

        PigAttack,
        PigStruck,
        PigDie,

        DeerAttack,
        DeerStruck,
        DeerDie,

        CowAttack,
        CowStruck,
        CowDie,

        SheepAttack,
        SheepStruck,
        SheepDie,

        ClawCatAttack,
        ClawCatStruck,
        ClawCatDie,

        WolfAttack,
        WolfStruck,
        WolfDie,

        ForestYetiAttack,
        ForestYetiStruck,
        ForestYetiDie,

        CarnivorousPlantAttack,
        CarnivorousPlantStruck,
        CarnivorousPlantDie,

        OmaAttack,
        OmaStruck,
        OmaDie,

        TigerSnakeAttack,
        TigerSnakeStruck,
        TigerSnakeDie,

        SpittingSpiderAttack,
        SpittingSpiderStruck,
        SpittingSpiderDie,

        ScarecrowAttack,
        ScarecrowStruck,
        ScarecrowDie,

        OmaHeroAttack,
        OmaHeroStruck,
        OmaHeroDie,

        CaveBatAttack,
        CaveBatStruck,
        CaveBatDie,

        ScorpionAttack,
        ScorpionStruck,
        ScorpionDie,

        SkeletonAttack,
        SkeletonStruck,
        SkeletonDie,

        SkeletonAxeManAttack,
        SkeletonAxeManStruck,
        SkeletonAxeManDie,

        SkeletonAxeThrowerAttack,
        SkeletonAxeThrowerStruck,
        SkeletonAxeThrowerDie,

        SkeletonWarriorAttack,
        SkeletonWarriorStruck,
        SkeletonWarriorDie,

        SkeletonLordAttack,
        SkeletonLordStruck,
        SkeletonLordDie,

        CaveMaggotAttack,
        CaveMaggotStruck,
        CaveMaggotDie,

        GhostSorcererAttack,
        GhostSorcererStruck,
        GhostSorcererDie,

        GhostMageAppear,
        GhostMageAttack,
        GhostMageStruck,
        GhostMageDie,

        VoraciousGhostAttack,
        VoraciousGhostStruck,
        VoraciousGhostDie,

        GhoulChampionAttack,
        GhoulChampionStruck,
        GhoulChampionDie,

        ArmoredAntAttack,
        ArmoredAntStruck,
        ArmoredAntDie,

        AntNeedlerAttack,
        AntNeedlerStruck,
        AntNeedlerDie,

        KeratoidAttack,
        KeratoidStruck,
        KeratoidDie,

        ShellNipperAttack,
        ShellNipperStruck,
        ShellNipperDie,

        VisceralWormAttack,
        VisceralWormStruck,
        VisceralWormDie,

        MutantFleaAttack,
        MutantFleaStruck,
        MutantFleaDie,

        PoisonousMutantFleaAttack,
        PoisonousMutantFleaStruck,
        PoisonousMutantFleaDie,

        BlasterMutantFleaAttack,
        BlasterMutantFleaStruck,
        BlasterMutantFleaDie,

        WasHatchlingAttack,
        WasHatchlingStruck,
        WasHatchlingDie,

        CentipedeAttack,
        CentipedeStruck,
        CentipedeDie,

        ButterflyWormAttack,
        ButterflyWormStruck,
        ButterflyWormDie,

        MutantMaggotAttack,
        MutantMaggotStruck,
        MutantMaggotDie,

        EarwigAttack,
        EarwigStruck,
        EarwigDie,

        IronLanceAttack,
        IronLanceStruck,
        IronLanceDie,

        LordNiJaeAttack,
        LordNiJaeStruck,
        LordNiJaeDie,

        RottingGhoulAttack,
        RottingGhoulStruck,
        RottingGhoulDie,

        DecayingGhoulAttack,
        DecayingGhoulStruck,
        DecayingGhoulDie,

        BloodThirstyGhoulAttack,
        BloodThirstyGhoulStruck,
        BloodThirstyGhoulDie,

        SpinedDarkLizardAttack,
        SpinedDarkLizardStruck,
        SpinedDarkLizardDie,

        UmaInfidelAttack,
        UmaInfidelStruck,
        UmaInfidelDie,

        UmaFlameThrowerAttack,
        UmaFlameThrowerStruck,
        UmaFlameThrowerDie,

        UmaAnguisherAttack,
        UmaAnguisherStruck,
        UmaAnguisherDie,

        UmaKingAttack,
        UmaKingStruck,
        UmaKingDie,

        SpiderBatAttack,
        SpiderBatStruck,
        SpiderBatDie,

        ArachnidGazerStruck,
        ArachnidGazerDie,

        LarvaAttack,
        LarvaStruck,

        RedMoonGuardianAttack,
        RedMoonGuardianStruck,
        RedMoonGuardianDie,

        RedMoonProtectorAttack,
        RedMoonProtectorStruck,
        RedMoonProtectorDie,

        VenomousArachnidAttack,
        VenomousArachnidStruck,
        VenomousArachnidDie,

        DarkArachnidAttack,
        DarkArachnidStruck,
        DarkArachnidDie,

        RedMoonTheFallenAttack,
        RedMoonTheFallenStruck,
        RedMoonTheFallenDie,

        ViciousRatAttack,
        ViciousRatStruck,
        ViciousRatDie,

        ZumaSharpShooterAttack,
        ZumaSharpShooterStruck,
        ZumaSharpShooterDie,

        ZumaFanaticAttack,
        ZumaFanaticStruck,
        ZumaFanaticDie,

        ZumaGuardianAttack,
        ZumaGuardianStruck,
        ZumaGuardianDie,

        ZumaKingAppear,
        ZumaKingAttack,
        ZumaKingStruck,
        ZumaKingDie,

        EvilFanaticAttack,
        EvilFanaticStruck,
        EvilFanaticDie,

        MonkeyAttack,
        MonkeyStruck,
        MonkeyDie,

        EvilElephantAttack,
        EvilElephantStruck,
        EvilElephantDie,

        CannibalFanaticAttack,
        CannibalFanaticStruck,
        CannibalFanaticDie,

        SpikedBeetleAttack,
        SpikedBeetleStruck,
        SpikedBeetleDie,

        NumaGruntAttack,
        NumaGruntStruck,
        NumaGruntDie,

        NumaMageAttack,
        NumaMageStruck,
        NumaMageDie,

        NumaEliteAttack,
        NumaEliteStruck,
        NumaEliteDie,

        SandSharkAttack,
        SandSharkStruck,
        SandSharkDie,

        StoneGolemAppear,
        StoneGolemAttack,
        StoneGolemStruck,
        StoneGolemDie,

        WindfurySorceressAttack,
        WindfurySorceressStruck,
        WindfurySorceressDie,

        CursedCactusAttack,
        CursedCactusStruck,
        CursedCactusDie,

        RagingLizardAttack,
        RagingLizardStruck,
        RagingLizardDie,

        SawToothLizardAttack,
        SawToothLizardStruck,
        SawToothLizardDie,

        MutantLizardAttack,
        MutantLizardStruck,
        MutantLizardDie,

        VenomSpitterAttack,
        VenomSpitterStruck,
        VenomSpitterDie,

        SonicLizardAttack,
        SonicLizardStruck,
        SonicLizardDie,

        GiantLizardAttack,
        GiantLizardStruck,
        GiantLizardDie,

        CrazedLizardAttack,
        CrazedLizardStruck,
        CrazedLizardDie,

        TaintedTerrorAttack,
        TaintedTerrorStruck,
        TaintedTerrorDie,
        TaintedTerrorAttack2,

        DeathLordJichonAttack,
        DeathLordJichonStruck,
        DeathLordJichonDie,
        DeathLordJichonAttack2,
        DeathLordJichonAttack3,

        MinotaurAttack,
        MinotaurStruck,
        MinotaurDie,

        FrostMinotaurAttack,
        FrostMinotaurStruck,
        FrostMinotaurDie,

        BanyaLeftGuardAttack,
        BanyaLeftGuardStruck,
        BanyaLeftGuardDie,

        EmperorSaWooAttack,
        EmperorSaWooStruck,
        EmperorSaWooDie,

        BoneArcherAttack,
        BoneArcherStruck,
        BoneArcherDie,

        BoneCaptainAttack,
        BoneCaptainStruck,
        BoneCaptainDie,

        ArchLichTaeduAttack,
        ArchLichTaeduStruck,
        ArchLichTaeduDie,

        WedgeMothLarvaAttack,
        WedgeMothLarvaStruck,
        WedgeMothLarvaDie,

        LesserWedgeMothAttack,
        LesserWedgeMothStruck,
        LesserWedgeMothDie,

        WedgeMothAttack,
        WedgeMothStruck,
        WedgeMothDie,

        RedBoarAttack,
        RedBoarStruck,
        RedBoarDie,

        ClawSerpentAttack,
        ClawSerpentStruck,
        ClawSerpentDie,

        BlackBoarAttack,
        BlackBoarStruck,
        BlackBoarDie,

        TuskLordAttack,
        TuskLordStruck,
        TuskLordDie,

        RazorTuskAttack,
        RazorTuskStruck,
        RazorTuskDie,

        PinkGoddessAttack,
        PinkGoddessStruck,
        PinkGoddessDie,

        GreenGoddessAttack,
        GreenGoddessStruck,
        GreenGoddessDie,

        MutantCaptainAttack,
        MutantCaptainStruck,
        MutantCaptainDie,

        StoneGriffinAttack,
        StoneGriffinStruck,
        StoneGriffinDie,

        FlameGriffinAttack,
        FlameGriffinStruck,
        FlameGriffinDie,

        WhiteBoneAttack,
        WhiteBoneStruck,
        WhiteBoneDie,

        ShinsuSmallStruck,
        ShinsuSmallDie,

        ShinsuBigAttack,
        ShinsuBigStruck,
        ShinsuBigDie,

        ShinsuShow,

        CorpseStalkerAttack,
        CorpseStalkerStruck,
        CorpseStalkerDie,

        LightArmedSoldierAttack,
        LightArmedSoldierStruck,
        LightArmedSoldierDie,

        CorrosivePoisonSpitterAttack,
        CorrosivePoisonSpitterStruck,
        CorrosivePoisonSpitterDie,

        PhantomSoldierAttack,
        PhantomSoldierStruck,
        PhantomSoldierDie,

        MutatedOctopusAttack,
        MutatedOctopusStruck,
        MutatedOctopusDie,

        AquaLizardAttack,
        AquaLizardStruck,
        AquaLizardDie,

        CrimsonNecromancerAttack,
        CrimsonNecromancerStruck,
        CrimsonNecromancerDie,

        ChaosKnightAttack,
        ChaosKnightStruck,
        ChaosKnightDie,

        PachontheChaosbringerAttack,
        PachontheChaosbringerStruck,
        PachontheChaosbringerDie,

        NumaCavalryAttack,
        NumaCavalryStruck,
        NumaCavalryDie,

        NumaHighMageAttack,
        NumaHighMageStruck,
        NumaHighMageDie,

        NumaStoneThrowerAttack,
        NumaStoneThrowerStruck,
        NumaStoneThrowerDie,

        NumaRoyalGuardAttack,
        NumaRoyalGuardStruck,
        NumaRoyalGuardDie,

        NumaArmoredSoldierAttack,
        NumaArmoredSoldierStruck,
        NumaArmoredSoldierDie,

        IcyRangerAttack,
        IcyRangerStruck,
        IcyRangerDie,

        IcyGoddessAttack,
        IcyGoddessStruck,
        IcyGoddessDie,

        IcySpiritWarriorAttack,
        IcySpiritWarriorStruck,
        IcySpiritWarriorDie,

        IcySpiritGeneralAttack,
        IcySpiritGeneralStruck,
        IcySpiritGeneralDie,

        GhostKnightAttack,
        GhostKnightStruck,
        GhostKnightDie,

        IcySpiritSpearmanAttack,
        IcySpiritSpearmanStruck,
        IcySpiritSpearmanDie,

        WerewolfAttack,
        WerewolfStruck,
        WerewolfDie,

        WhitefangAttack,
        WhitefangStruck,
        WhitefangDie,

        IcySpiritSoliderAttack,
        IcySpiritSoliderStruck,
        IcySpiritSoliderDie,

        WildBoarAttack,
        WildBoarStruck,
        WildBoarDie,

        FrostLordHwaAttack,
        FrostLordHwaStruck,
        FrostLordHwaDie,

        JinchonDevilAttack,
        JinchonDevilAttack2,
        JinchonDevilAttack3,
        JinchonDevilStruck,
        JinchonDevilDie,

        EscortCommanderAttack,
        EscortCommanderStruck,
        EscortCommanderDie,

        FieryDancerAttack,
        FieryDancerStruck,
        FieryDancerDie,

        EmeraldDancerAttack,
        EmeraldDancerStruck,
        EmeraldDancerDie,

        QueenOfDawnAttack,
        QueenOfDawnStruck,
        QueenOfDawnDie,

        OYoungBeastAttack,
        OYoungBeastStruck,
        OYoungBeastDie,

        YumgonWitchAttack,
        YumgonWitchStruck,
        YumgonWitchDie,

        MaWarlordAttack,
        MaWarlordStruck,
        MaWarlordDie,

        JinhwanSpiritAttack,
        JinhwanSpiritStruck,
        JinhwanSpiritDie,

        JinhwanGuardianAttack,
        JinhwanGuardianStruck,
        JinhwanGuardianDie,

        YumgonGeneralAttack,
        YumgonGeneralStruck,
        YumgonGeneralDie,

        ChiwooGeneralAttack,
        ChiwooGeneralStruck,
        ChiwooGeneralDie,

        DragonQueenAttack,
        DragonQueenStruck,
        DragonQueenDie,

        DragonLordAttack,
        DragonLordStruck,
        DragonLordDie,

        FerociousIceTigerAttack,
        FerociousIceTigerStruck,
        FerociousIceTigerDie,

        SamaFireGuardianAttack,
        SamaFireGuardianStruck,
        SamaFireGuardianDie,

        SamaIceGuardianAttack,
        SamaIceGuardianStruck,
        SamaIceGuardianDie,

        SamaLightningGuardianAttack,
        SamaLightningGuardianStruck,
        SamaLightningGuardianDie,

        SamaWindGuardianAttack,
        SamaWindGuardianStruck,
        SamaWindGuardianDie,

        PhoenixAttack,
        PhoenixStruck,
        PhoenixDie,

        BlackTortoiseAttack,
        BlackTortoiseStruck,
        BlackTortoiseDie,

        BlueDragonAttack,
        BlueDragonStruck,
        BlueDragonDie,

        WhiteTigerAttack,
        WhiteTigerStruck,
        WhiteTigerDie,
        #endregion

        ThunderKickEnd,

        ThunderKickStart,
        RakeStart,

        Main,  //韩版Login场景背景音乐
        ItemGold,

        #region 新增音效

        FlashOfLightStart,
        Wemade,
        Wemade2,
        StartGame,
        ReigningStepStart,
        NeutralizeEnd,
        DarkSoulPrison,
        ElementalHurricane,
        IceRainStart,
        Concentration,

        #endregion

        #region 新增怪物音效

        CrazedPrimateAttack,
        CrazedPrimateStruck,
        CrazedPrimateDie,

        HellBringerAttack,
        HellBringerAttack2,
        HellBringerAttack3,
        HellBringerStruck,
        HellBringerDie,

        YurinHoundAttack,
        YurinHoundStruck,
        YurinHoundDie,

        YurinTigerAttack,
        YurinTigerStruck,
        YurinTigerDie,

        HardenedRhinoAttack,
        HardenedRhinoStruck,
        HardenedRhinoDie,

        MammothAttack,
        MammothStruck,
        MammothDie,

        CursedSlave1Attack,
        CursedSlave1Attack2,
        CursedSlave1Struck,
        CursedSlave1Die,

        CursedSlave2Attack,
        CursedSlave2Struck,
        CursedSlave2Die,

        CursedSlave3Attack,
        CursedSlave3Attack2,
        CursedSlave3Struck,
        CursedSlave3Die,

        PoisonousGolemAttack,
        PoisonousGolemAttack2,
        PoisonousGolemStruck,
        PoisonousGolemDie,

        GardenSoldierAttack,
        GardenSoldierAttack2,
        GardenSoldierStruck,
        GardenSoldierDie,

        GardenDefenderAttack,
        GardenDefenderAttack2,
        GardenDefenderStruck,
        GardenDefenderDie,

        RedBlossomAttack,
        RedBlossomAttack2,
        RedBlossomStruck,
        RedBlossomDie,

        BlueBlossomAttack,
        BlueBlossomStruck,
        BlueBlossomDie,

        FireBirdAttack,
        FireBirdAttack2,
        FireBirdAttack3,
        FireBirdStruck,
        FireBirdDie,

        Terracotta1Attack,
        Terracotta1Struck,
        Terracotta1Die,

        Terracotta2Attack,
        Terracotta2Struck,
        Terracotta2Die,

        Terracotta3Attack,
        Terracotta3Struck,
        Terracotta3Die,

        Terracotta4Attack,
        Terracotta4Struck,
        Terracotta4Die,

        TerracottaSubAttack,
        TerracottaSubAttack2,
        TerracottaSubStruck,
        TerracottaSubDie,

        TerracottaBossAttack,
        TerracottaBossAttack2,
        TerracottaBossStruck,
        TerracottaBossDie,

        #endregion

        news,  //私聊提醒

        NSelectWarriorMale,
        NSelectWarriorFemale,
        NSelectWizardMale,
        NSelectWizardFemale,
        NSelectTaoistMale,
        NSelectTaoistFemale,

        NWarriorMale,
        NWarriorFemale,
        NWizardMale,
        NWizardFemale,
        NTaoistMale,
        NTaoistFemale,

        #region 145相关音效

        //145动画音效
        Wemade145,
        StartGame145,
        CreateChr145,
        SelChrRe145,

        //145背景音乐
        LoginSceneBgm145,
        SelChrBgm145,
        CreateChrBgm145,

        //145选人场景音效
        SelectWarriorMale145,
        SelectWarriorFemale145,
        SelectWizardMale145,
        SelectWizardFemale145,
        SelectTaoistMale145,
        SelectTaoistFemale145,

        //145创建角色场景音效
        CreateWarriorMale145,
        CreateWarriorFemale145,
        CreateWizardMale145,
        CreateWizardFemale145,
        CreateTaoistMale145,
        CreateTaoistFemale145,

        #endregion

        gxfc,

#if __MOBILE__
        /// <summary>
        /// 启动音乐
        /// </summary>
        Mobile
#endif
        //手机
    }
    #endregion

    /// <summary>
    /// 自定义动画声音
    /// </summary>
    public enum MirAnimationSound
    {
        /// <summary>
        /// 攻击声音
        /// </summary>
        [Description("攻击声音")]
        AttackSound,
        /// <summary>
        /// 被攻击声音
        /// </summary>
        [Description("被攻击声音")]
        StruckSound,
        /// <summary>
        /// 死亡声音
        /// </summary>
        [Description("死亡声音")]
        DieSound
    }

    #region 被事件管理器支持的事件
    /// <summary>
    /// 成就事件类型
    /// </summary>
    public enum EventTypes
    {
        #region 人物
        PlayerLogin = 100,
        PlayerLogout,
        PlayerAttack,
        PlayerLifeSteal,
        PlayerUseMagic,
        PlayerMove,
        PlayerHarvest,
        PlayerOnline,
        PlayerMine,
        PlayerGainItem,
        PlayerDealDamageToPlayer,
        PlayerDealDamageToMonster,
        PlayerDodge,
        PlayerLevelUp,
        PlayerRebirth,

        PlayerCraft,

        PlayerKillMonster,

        PlayerRankingChange,

        PlayerWeaponRefine,

        PlayerWeaponReset,

        PlayerAccessoryRefineLevel,

        PlayerMarry,

        PlayerDivorce,

        PlayerDie,
        #endregion

        #region 物品

        ItemDrop = 200,
        ItemBreak,
        ItemMove,

        #endregion

        #region 怪物

        MonsterSpawn,
        MonsterKilled,

        #endregion

        #region 行会

        GuildCreate,

        #endregion

        #region 系统

        MinuteChange,
        HourChange,
        DayChange,
        WeekChange,
        MonthChange,

        #endregion
    }

    #endregion

    /// <summary>
    /// 攻城玩法类型
    /// </summary>
    public enum ConquestType
    {
        /// <summary>
        /// 击杀BOSS积分模式
        /// </summary>
        [Description("击杀BOSS积分模式")]
        Boss,
        /// <summary>
        /// 夺旗模式
        /// </summary>
        [Description("夺旗模式")]
        Flag,
        /// <summary>
        /// 占领皇宫模式
        /// </summary>
        [Description("争夺皇宫模式(开发中)")]
        Capture,
        /// <summary>
        /// 掠夺道具模式
        /// </summary>
        [Description("抢夺道具模式(开发中)")]
        Prop,
    }

    /// <summary>
    /// 自定义技能动画类型
    /// </summary>
    public enum DiyMagicType
    {
        /// <summary>
        /// 点攻击动画
        /// </summary>
        [Description("点攻击动画")]
        Point,
        /// <summary>
        /// 直线攻击动画
        /// </summary>
        [Description("直线攻击动画")]
        Line,
        /// <summary>
        /// 飞向目的地爆炸攻击动画
        /// </summary>
        [Description("飞向目的地爆炸攻击动画")]
        FlyDestinationExplosion,
        /// <summary>
        /// 飞行命中目标爆炸攻击动画
        /// </summary>
        [Description("飞行命中目标爆炸攻击动画")]
        FiyHitTargetExplosion,
        /// <summary>
        /// 爆炸魔法动画
        /// </summary>
        [Description("爆炸魔法动画")]
        ExplosionMagic,
        /// <summary>
        /// 行动魔法动画
        /// </summary>
        [Description("行动魔法动画")]
        ActMagic,
        /// <summary>
        /// 身体特效动画
        /// </summary>
        [Description("身体特效动画")]
        BodyEffect,
        /// <summary>
        /// 魔法攻击玩家动画
        /// </summary>
        [Description("魔法攻击玩家动画")]
        MagicAttackPlayer,
        /// <summary>
        /// 出现动画
        /// </summary>
        [Description("出现动画")]
        MonOnSpawned,
        DressLightInner_Man_Down,
        DressLightInner_WoMan_Down,
        DressLightInner_Man,
        DressLightInner_WoMan,
        DressLightOut_Man,
        DressLightOut_WoMan,
        WeaponLightInner,
        WeaponLightOut_Man,
        WeaponLightOut_WoMan,
        /// <summary>
        /// 攻击动画
        /// </summary>
        [Description("攻击动画")]
        MonAttackMon,
        /// <summary>
        /// 战斗动画1
        /// </summary>
        [Description("战斗动画1")]
        MonCombat1,
        /// <summary>
        /// 战斗动画2
        /// </summary>
        [Description("战斗动画2")]
        MonCombat2,
        /// <summary>
        /// 战斗动画3
        /// </summary>
        [Description("战斗动画3")]
        MonCombat3,
        /// <summary>
        /// 战斗动画4
        /// </summary>
        [Description("战斗动画4")]
        MonCombat4,
        /// <summary>
        /// 战斗动画5
        /// </summary>
        [Description("战斗动画5")]
        MonCombat5,
        /// <summary>
        /// 战斗动画6
        /// </summary>
        [Description("战斗动画6")]
        MonCombat6,
        /// <summary>
        /// 死亡动画
        /// </summary>
        [Description("死亡动画")]
        MonDie,
        /// <summary>
        /// 空置
        /// </summary>
        [Description("空置")]
        None,
    }

    /// <summary>
    /// 投币选项
    /// </summary>
    public enum TossCoinOption : byte
    {
        /// <summary>
        /// 一次
        /// </summary>
        Once,
        /// <summary>
        /// 十次
        /// </summary>
        TenTimes,
        /// <summary>
        /// 百次
        /// </summary>
        HundredTimes,
        /// <summary>
        /// 十次高级
        /// </summary>
        TenTimesAdvanced,
        /// <summary>
        /// 二十次高级
        /// </summary>
        HundredTimesAdvanced,
    }

    /// <summary>
    /// 无主物品的类型
    /// </summary>
    public enum OwnerlessItemType
    {
        None,
        UserSold,
        SystemDropped,
        UserDropped,
    }

    /// <summary>
    /// 套装触发类型
    /// </summary>
    public enum ItemSetRequirementType
    {
        [Description("本搭配内的物品[不能]触发其他搭配。选了这个选项，在计算其他套装以及其他搭配的时候，本搭配内的装备穿了也视为没穿")]
        WithoutReplacement,
        [Description("本搭配内的物品[可以]触发其他搭配。选了这个选项，在计算其他套装以及其他搭配的时候，可以用到本搭配内的任何装备")]
        WithReplacement,
    }
    /// <summary>
    /// 红包的类型
    /// </summary>
    public enum RedPacketType
    {
        [Description("无-本项占位无实际作用")]
        None,
        [Description("拼手气")]
        Randomly,
        [Description("平均分")]
        Evenly,
    }
    /// <summary>
    /// 红包发送范围
    /// </summary>
    public enum RedPacketScope
    {
        [Description("无-本项占位无实际作用")]
        None,
        [Description("全服")]
        Server,
        [Description("同行会")]
        Guild,
        [Description("同小队")]
        Group,
    }
    public enum TradeType
    {
        None,
        Buy,
        Sell,
    }
    public enum StockOrderType
    {
        None,
        Normal,
        Completed,
        Part,
        Cannel,
    }
}