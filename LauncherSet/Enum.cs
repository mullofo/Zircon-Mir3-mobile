using System.ComponentModel;

namespace LauncherSet
{
    public enum MagicType   //魔法技能类型
    {
        None,
        /// <summary>
        /// 基本剑术
        /// </summary>
        Swordsmanship = 100,

        /// <summary>
        /// 运气术
        /// </summary>
        PotionMastery = 101,

        /// <summary>
        /// 攻杀剑术
        /// </summary>
        Slaying = 102,

        /// <summary>
        /// 刺杀剑术
        /// </summary>
        Thrusting = 103,

        /// <summary>
        /// 半月弯刀
        /// </summary>
        HalfMoon = 104,

        /// <summary>
        /// 野蛮冲撞
        /// </summary>
        ShoulderDash = 105,

        /// <summary>
        /// 烈火剑法
        /// </summary>
        FlamingSword = 106,

        /// <summary>
        /// 翔空剑法
        /// </summary>
        DragonRise = 107,

        /// <summary>
        /// 莲月剑法
        /// </summary>
        BladeStorm = 108,

        /// <summary>
        /// 十方斩
        /// </summary>
        DestructiveSurge = 109,

        /// <summary>
        /// 乾坤大挪移
        /// </summary>
        Interchange = 110,

        /// <summary>
        /// 铁布衫
        /// </summary>
        Defiance = 111,

        /// <summary>
        /// 斗转星移
        /// </summary>
        Beckon = 112,

        /// <summary>
        /// 破血狂杀
        /// </summary>
        Might = 113,

        /// <summary>
        /// 快刀斩马
        /// </summary>
        SwiftBlade = 114,

        /// <summary>
        /// 狂暴冲撞
        /// </summary>
        Assault = 115,

        /// <summary>
        /// 金刚之躯
        /// </summary>
        Endurance = 116,

        /// <summary>
        /// 移花接木
        /// </summary>
        ReflectDamage = 117,

        /// <summary>
        /// 泰山压顶
        /// </summary>
        Fetter = 118,

        /// <summary>
        /// 旋风斩
        /// </summary>
        SwirlingBlade = 119,

        /// <summary>
        /// 君临步
        /// </summary>
        ReigningStep = 120,

        /// <summary>
        /// 屠龙斩
        /// </summary>
        MaelstromBlade = 121,

        /// <summary>
        /// 火球术
        /// </summary>
        FireBall = 201,

        /// <summary>
        /// 霹雳掌
        /// </summary>
        LightningBall = 202,

        /// <summary>
        /// 冰月神掌
        /// </summary>
        IceBolt = 203,

        /// <summary>
        /// 风掌
        /// </summary>
        GustBlast = 204,

        /// <summary>
        /// 抗拒火环
        /// </summary>
        Repulsion = 205,

        /// <summary>
        /// 诱惑之光
        /// </summary>
        ElectricShock = 206,

        /// <summary>
        /// 瞬息移动
        /// </summary>
        Teleportation = 207,

        /// <summary>
        /// 大火球
        /// </summary>
        AdamantineFireBall = 208,

        /// <summary>
        /// 雷电术
        /// </summary>
        ThunderBolt = 209,

        /// <summary>
        /// 冰月震天
        /// </summary>
        IceBlades = 210,

        /// <summary>
        /// 击风
        /// </summary>
        Cyclone = 211,

        /// <summary>
        /// 地狱火
        /// </summary>
        ScortchedEarth = 212,

        /// <summary>
        /// 疾光电影
        /// </summary>
        LightningBeam = 213,

        /// <summary>
        /// 冰沙掌
        /// </summary>
        FrozenEarth = 214,

        /// <summary>
        /// 风震天
        /// </summary>
        BlowEarth = 215,

        /// <summary>
        /// 火墙
        /// </summary>
        FireWall = 216,

        /// <summary>
        /// 圣言术
        /// </summary>
        ExpelUndead = 217,

        /// <summary>
        /// 移形换位
        /// </summary>
        GeoManipulation = 218,

        /// <summary>
        /// 魔法盾
        /// </summary>
        MagicShield = 219,

        /// <summary>
        /// 爆裂火焰
        /// </summary>
        FireStorm = 220,

        /// <summary>
        /// 地狱雷光
        /// </summary>
        LightningWave = 221,

        /// <summary>
        /// 冰咆哮
        /// </summary>
        IceStorm = 222,

        /// <summary>
        /// 龙卷风
        /// </summary>
        DragonTornado = 223,

        /// <summary>
        /// 魄冰刺
        /// </summary>
        GreaterFrozenEarth = 224,

        /// <summary>
        /// 怒神霹雳
        /// </summary>
        ChainLightning = 225,

        /// <summary>
        /// 焰天火雨
        /// </summary>
        MeteorShower = 226,

        /// <summary>
        /// 凝血离魂
        /// </summary>
        Renounce = 227,

        /// <summary>
        /// 旋风墙
        /// </summary>
        Tempest = 228,

        /// <summary>
        /// 天打雷劈
        /// </summary>
        JudgementOfHeaven = 229,

        /// <summary>
        /// 电闪雷鸣
        /// </summary>
        ThunderStrike = 230,

        /// <summary>
        /// 透心链
        /// </summary>
        RayOfLight = 231,

        /// <summary>
        /// 混元掌
        /// </summary>
        BurstOfEnergy = 232,

        /// <summary>
        /// 魔光盾
        /// </summary>
        ShieldOfPreservation = 233,

        /// <summary>
        /// 焚魂魔功
        /// </summary>
        RetrogressionOfEnergy = 234,

        /// <summary>
        /// 魔爆术
        /// </summary>
        FuryBlast = 235,

        /// <summary>
        /// 地狱魔焰
        /// </summary>
        TempestOfUnstableEnergy = 236,

        /// <summary>
        /// 治愈术
        /// </summary>
        Heal = 300,

        /// <summary>
        /// 精神力战法
        /// </summary>
        SpiritSword = 301,

        /// <summary>
        /// 施毒术
        /// </summary>
        PoisonDust = 302,

        /// <summary>
        /// 灵魂火符
        /// </summary>
        ExplosiveTalisman = 303,

        /// <summary>
        /// 月魂断玉
        /// </summary>
        EvilSlayer = 304,

        /// <summary>
        /// 隐身术
        /// </summary>
        Invisibility = 305,

        /// <summary>
        /// 幽灵盾
        /// </summary>
        MagicResistance = 306,

        /// <summary>
        /// 集体隐身术
        /// </summary>
        MassInvisibility = 307,

        /// <summary>
        /// 月魂灵波
        /// </summary>
        GreaterEvilSlayer = 308,

        /// <summary>
        /// 神圣战甲术
        /// </summary>
        Resilience = 309,

        /// <summary>
        /// 困魔咒
        /// </summary>
        TrapOctagon = 310,

        /// <summary>
        /// 空拳刀法
        /// </summary>
        TaoistCombatKick = 311,

        /// <summary>
        /// 强魔震法
        /// </summary>
        ElementalSuperiority = 312,

        /// <summary>
        /// 群体治愈术
        /// </summary>
        MassHeal = 313,

        /// <summary>
        /// 猛虎强势
        /// </summary>
        BloodLust = 314,

        /// <summary>
        /// 回生术
        /// </summary>
        Resurrection = 315,

        /// <summary>
        /// 云寂术
        /// </summary>
        Purification = 316,

        /// <summary>
        /// 妙影无踪
        /// </summary>
        Transparency = 317,

        /// <summary>
        /// 阴阳法环
        /// </summary>
        CelestialLight = 318,

        /// <summary>
        /// 养生术
        /// </summary>
        EmpoweredHealing = 319,

        /// <summary>
        /// 吸星大法
        /// </summary>
        LifeSteal = 320,

        /// <summary>
        /// 灭魂火符
        /// </summary>
        ImprovedExplosiveTalisman = 321,

        /// <summary>
        /// 施毒大法
        /// </summary>
        GreaterPoisonDust = 322,

        /// <summary>
        /// 迷魂大法
        /// </summary>
        Scarecrow = 323,

        /// <summary>
        /// 横扫千军
        /// </summary>
        ThunderKick = 324,

        /// <summary>
        /// 神灵守护
        /// </summary>
        DragonBreath = 325,

        /// <summary>
        /// 隐魂术
        /// </summary>
        MassTransparency = 326,

        /// <summary>
        /// 月明波
        /// </summary>
        GreaterHolyStrike = 327,

        /// <summary>
        /// 垂柳舞
        /// </summary>
        WillowDance = 401,

        /// <summary>
        /// 蔓藤舞
        /// </summary>
        VineTreeDance = 402,

        /// <summary>
        /// 磨炼
        /// </summary>
        Discipline = 403,

        /// <summary>
        /// 毒云
        /// </summary>
        PoisonousCloud = 404,

        /// <summary>
        /// 盛开
        /// </summary>
        FullBloom = 405,

        /// <summary>
        /// 潜行
        /// </summary>
        Cloak = 406,

        /// <summary>
        /// 白莲
        /// </summary>
        WhiteLotus = 407,

        /// <summary>
        /// 满月恶狼
        /// </summary>
        CalamityOfFullMoon = 408,

        /// <summary>
        /// 亡灵束缚
        /// </summary>
        WraithGrip = 409,

        /// <summary>
        /// 红莲
        /// </summary>
        RedLotus = 410,

        /// <summary>
        /// 烈焰
        /// </summary>
        HellFire = 411,

        /// <summary>
        /// 血禅
        /// </summary>
        PledgeOfBlood = 412,

        /// <summary>
        /// 血之盟约
        /// </summary>
        Rake = 413,

        /// <summary>
        /// 月季
        /// </summary>
        SweetBrier = 414,

        /// <summary>
        /// 亡灵替身
        /// </summary>
        SummonPuppet = 415,

        /// <summary>
        /// 孽报
        /// </summary>
        Karma = 416,

        /// <summary>
        /// 亡灵之手
        /// </summary>
        TouchOfTheDeparted = 417,

        /// <summary>
        /// 残月之乱
        /// </summary>
        WaningMoon = 418,

        /// <summary>
        /// 鬼灵步
        /// </summary>
        GhostWalk = 419,

        /// <summary>
        /// 神机妙算
        /// </summary>
        ElementalPuppet = 420,

        /// <summary>
        /// 深渊
        /// </summary>
        Rejuvenation = 421,

        /// <summary>
        /// 决意
        /// </summary>
        Resolution = 422,

        /// <summary>
        /// 切换
        /// </summary>
        ChangeOfSeasons = 423,

        /// <summary>
        /// 解放
        /// </summary>
        Release = 424,

        /// <summary>
        /// 新月爆炎龙
        /// </summary>
        FlameSplash = 425,

        /// <summary>
        /// 百花盛开
        /// </summary>
        BloodyFlower = 426,

        /// <summary>
        /// 心机一转
        /// </summary>
        TheNewBeginning = 427,

        /// <summary>
        /// 鹰击
        /// </summary>
        DanceOfSwallow = 428,

        /// <summary>
        /// 黄泉旅者
        /// </summary>
        DarkConversion = 429,

        /// <summary>
        /// 狂涛涌泉
        /// </summary>
        DragonRepulse = 430,

        /// <summary>
        /// 修罗降临
        /// </summary>
        AdventOfDemon = 431,

        /// <summary>
        /// 罗刹降临
        /// </summary>
        AdventOfDevil = 432,

        /// <summary>
        /// 深渊苦海
        /// </summary>
        Abyss = 433,

        /// <summary>
        /// 日闪
        /// </summary>
        FlashOfLight = 434,

        /// <summary>
        /// 隐沦
        /// </summary>
        Stealth = 435,

        /// <summary>
        /// 风之闪避
        /// </summary>
        Evasion = 436,

        /// <summary>
        /// 风之守护
        /// </summary>
        RagingWind = 437,

        /// <summary>
        /// 群体灵魂火符
        /// </summary>
        AugmentExplosiveTalisman = 328,

        /// <summary>
        /// 群体月魂灵波
        /// </summary>
        AugmentEvilSlayer = 329,

        /// <summary>
        /// 强化云寂术
        /// </summary>
        AugmentPurification = 330,

        /// <summary>
        /// 强化回生术
        /// </summary>
        OathOfThePerished = 331,

        /// <summary>
        /// 魔焰强解术
        /// </summary>
        DemonExplosion = 337,

        /// <summary>
        /// 移花接玉
        /// </summary>
        StrengthOfFaith = 335,

        /// <summary>
        /// 召唤骷髅
        /// </summary>
        SummonSkeleton = 332,

        /// <summary>
        /// 分身术
        /// </summary>
        MirrorImage = 237,

        /// <summary>
        /// 超强召唤骷髅
        /// </summary>
        SummonJinSkeleton = 334,

        /// <summary>
        /// 召唤神兽
        /// </summary>
        SummonShinsu = 333,

        /// <summary>
        /// 焰魔召唤术
        /// </summary>
        SummonDemonicCreature = 336,

        /// <summary>
        /// 高级运气术
        /// </summary>
        AdvancedPotionMastery = 122,

        /// <summary>
        /// 高级百花盛开
        /// </summary>
        AdvancedBloodyFlower = 438,

        /// <summary>
        /// 高级凝血离魂
        /// </summary>
        AdvancedRenounce = 238,

        /// <summary>
        /// 挑衅
        /// </summary>
        MassBeckon = 123,

        /// <summary>
        /// 护身冰环
        /// </summary>
        FrostBite = 239,

        /// <summary>
        /// 传染
        /// </summary>
        Infection = 338,

        /// <summary>
        /// 最后抵抗
        /// </summary>
        Massacre = 439,

        /// <summary>
        /// 天雷锤
        /// </summary>
        SeismicSlam = 124,

        /// <summary>
        /// 地狱回疗
        /// </summary>
        DemonicRecovery = 339,

        /// <summary>
        /// 天之怒火
        /// </summary>
        Asteroid = 240,

        /// <summary>
        /// 集中
        /// </summary>
        ArtOfShadows = 440,

        MonsterScortchedEarth = 501,
        MonsterIceStorm = 502,
        MonsterDeathCloud = 503,
        MonsterThunderStorm = 504,

        SamaGuardianFire = 505,
        SamaGuardianIce = 506,
        SamaGuardianLightning = 507,
        SamaGuardianWind = 508,

        SamaPhoenixFire = 509,
        SamaBlackIce = 510,
        SamaBlueLightning = 511,
        SamaWhiteWind = 512,

        SamaProphetFire = 513,
        SamaProphetLightning = 514,
        SamaProphetWind = 515,

        DoomClawLeftPinch = 520,
        DoomClawLeftSwipe = 521,
        DoomClawRightPinch = 522,
        DoomClawRightSwipe = 523,
        DoomClawWave = 524,
        DoomClawSpit = 525,

        PinkFireBall = 530,
        GreenSludgeBall = 540,

    }

    public enum SpellKey : byte    //快捷键
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
    }
}
