using Library.Network;
using Library.SystemModels;
using MirDB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace Library
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class Globals
    {
#if WEB
        /// <summary>
        /// 资源文件根路径
        /// </summary>
        public static string ResoucePath { get; set; }
#endif
        /// <summary>
        /// 初始货币单位，ItemInfo 必须存在这个物品名
        /// </summary>
        public static readonly string Currency = "金币";
        /// <summary>
        /// 货币信息
        /// </summary>
        public static ItemInfo GoldInfo;
        /// <summary>
        /// 道具信息列表
        /// </summary>
        public static DBCollection<ItemInfo> ItemInfoList;
        /// <summary>
        /// 国际化
        /// </summary>
        public static DBCollection<LangInfo> LangInfoList;

        /// <summary>
        /// 魔法技能信息列表
        /// </summary>
        public static DBCollection<MagicInfo> MagicInfoList;
        /// <summary>
        /// 地图信息列表
        /// </summary>
        public static DBCollection<MapInfo> MapInfoList;
        /// <summary>
        /// NPC页列表
        /// </summary>
        //public static DBCollection<NPCPage> NPCPageList;
        /// <summary>
        /// 怪物信息列表
        /// </summary>
        public static DBCollection<MonsterInfo> MonsterInfoList;
        /// <summary>
        /// 商城信息列表
        /// </summary>
        public static DBCollection<StoreInfo> StoreInfoList;
        /// <summary>
        /// NPC信息列表
        /// </summary>
        public static DBCollection<NPCInfo> NPCInfoList;
        /// <summary>
        /// 地图链接信息列表
        /// </summary>
        public static DBCollection<MovementInfo> MovementInfoList;
        /// <summary>
        /// 任务信息列表
        /// </summary>
        public static DBCollection<QuestInfo> QuestInfoList;
        /// <summary>
        /// 任务交付信息列表
        /// </summary>
        public static DBCollection<QuestTask> QuestTaskList;
        /// <summary>
        /// 宠物信息列表
        /// </summary>
        public static DBCollection<CompanionInfo> CompanionInfoList;
        /// <summary>
        /// 宠物等级信息列表
        /// </summary>
        public static DBCollection<CompanionLevelInfo> CompanionLevelInfoList;
        /// <summary>
        /// 自定义道具特效列表
        /// </summary>
        public static DBCollection<ItemDisplayEffect> ItemDisplayEffectList;
        /// <summary>
        /// 制作道具信息列表
        /// </summary>
        public static DBCollection<CraftItemInfo> CraftItemInfoList;
        /// <summary>
        /// 经验设置
        /// </summary>
        public static DBCollection<PlayerExpInfo> GamePlayEXPInfoList;
        /// <summary>
        /// 武器经验设置
        /// </summary>
        public static DBCollection<WeaponExpInfo> GameWeaponEXPInfoList;
        /// <summary>
        /// 首饰经验设置
        /// </summary>
        public static DBCollection<AccessoryExpInfo> GameAccessoryEXPInfoList;
        /// <summary>
        /// 制作经验设置
        /// </summary>
        public static DBCollection<CraftExpInfo> GameCraftExpInfoList;
        /// <summary>
        /// 成就信息
        /// </summary>
        public static DBCollection<AchievementInfo> AchievementInfoList;
        /// <summary>
        /// 成就要求
        /// </summary>
        public static DBCollection<AchievementRequirement> AchievementRequirementList;
        /// <summary>
        /// 自定义buff列表
        /// </summary>
        public static DBCollection<CustomBuffInfo> CustomBuffInfoList;
        /// <summary>
        /// 新版武器升级设置
        /// </summary>
        public static DBCollection<WeaponUpgradeNew> WeaponUpgradeInfoList;
        /// <summary>
        /// 自定义怪物信息列表
        /// </summary>
        public static DBCollection<MonAnimationFrame> MonAnimationFrameList;
        /// <summary>
        /// 自定义技能动画
        /// </summary>
        public static DBCollection<DiyMagicEffect> DiyMagicEffectList;
        /// <summary>
        /// 刷怪区域列表
        /// </summary>
        public static DBCollection<RespawnInfo> RespawnInfoList;
        /// <summary>
        /// 套装列表
        /// </summary>
        public static DBCollection<SetInfo> SetInfoList;
        /// <summary>
        /// 套装搭配列表
        /// </summary>
        public static DBCollection<SetGroup> SetGroupList;
        /// <summary>
        /// 套装组件列表
        /// </summary>
        public static DBCollection<SetGroupItem> SetGroupItemList;
        /// <summary>
        /// 套装属性列表
        /// </summary>
        public static DBCollection<SetInfoStat> SetInfoStatList;
        /// <summary>
        /// 行会升级经验列表
        /// </summary>
        public static DBCollection<GuildLevelExp> GuildLevelExpList;

        /// <summary>
        /// 自定义BUFF分组信息
        /// </summary>
        public static Dictionary<string, HashSet<int>> CustomBuffGroupsDict = new Dictionary<string, HashSet<int>>();

        /// <summary>
        /// 商城分类
        /// </summary>
        public static Dictionary<string, string[]> StoreDic = new Dictionary<string, string[]>();

        /// <summary>
        /// 随机数
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// 邮箱账号正则表达式
        /// </summary>
        public static readonly Regex EMailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.Compiled);
        /// <summary>
        /// 密码正则表达式
        /// </summary>
        public static readonly Regex PasswordRegex = new Regex(@"^[\S]{" + MinPasswordLength + "," + MaxPasswordLength + "}$", RegexOptions.Compiled);
        /// <summary>
        /// 角色名字正则表达式
        /// </summary>
        public static readonly Regex CharacterReg = new Regex(@"^[\u4e00-\u9fa5_A-Za-z0-9]{" + MinCharacterNameLength + "," + MaxCharacterNameLength + @"}$", RegexOptions.Compiled);
        /// <summary>
        /// 行会名字正则表达式
        /// </summary>
        public static readonly Regex GuildNameRegex = new Regex(@"^[\u4e00-\u9fa5_A-Za-z0-9]{" + MinGuildNameLength + "," + MaxGuildNameLength + "}$", RegexOptions.Compiled);
        /// <summary>
        /// 英文名字正则表达式
        /// </summary>
        public static readonly Regex EnCharacterReg = new Regex(@"^[A-Za-z0-9]{" + MinCharacterNameLength + "," + MaxCharacterNameLength + @"}$", RegexOptions.Compiled);
        /// <summary>
        /// 英文行会名字正则表达式
        /// </summary>
        public static readonly Regex EnGuildNameRegex = new Regex(@"^[A-Za-z0-9]{" + MinGuildNameLength + "," + MaxGuildNameLength + "}$", RegexOptions.Compiled);
        /// <summary>
        /// 去除非法符号(只允许字母 数字 标点 . @ -)
        /// </summary>
        public static readonly Regex RemoveIllegalRegex = new Regex(@"[^\w\.@-]", RegexOptions.Compiled);

        /// <summary>
        /// 自定义颜色
        /// </summary>
        public static readonly Color NoneColour = Color.White,
                                     FireColour = Color.OrangeRed,
                                     IceColour = Color.PaleTurquoise,
                                     LightningColour = Color.LightSkyBlue,
                                     WindColour = Color.LightSeaGreen,
                                     HolyColour = Color.DarkKhaki,
                                     DarkColour = Color.SaddleBrown,
                                     PhantomColour = Color.Purple,
                                     BrownNameColour = Color.Brown,
                                     RedNameColour = Color.Red;

        // 激活中的全服自定义buff
        // 会被应用在每一个玩家身上
        // buff的index: 开始时间
        public static IDictionary<int, DateTime> ActiveEventCustomBuffs = new Dictionary<int, DateTime>();

        //允许打孔的装备
        public static readonly IList<ItemType> AllowedGemItemTypes = new List<ItemType>(){
            ItemType.Armour,
            ItemType.Belt,
            ItemType.Bracelet,
            ItemType.Helmet,
            ItemType.Ring,
            ItemType.Shoes,
            ItemType.Necklace,
            ItemType.Weapon,
            ItemType.Shield,
            ItemType.Wing,
            ItemType.Fashion,
        };

        //允许镶嵌的属性
        public static readonly IList<Stat> AllowedAttachementStats = new List<Stat>() {
            Stat.BaseHealth,
            Stat.BaseMana,
            Stat.Health,
            Stat.Mana,
            Stat.MinAC,
            Stat.MaxAC,
            Stat.MinMR,
            Stat.MaxMR,
            Stat.MinDC,
            Stat.MaxDC,
            Stat.MinMC,
            Stat.MaxMC,
            Stat.MinSC,
            Stat.MaxSC,
            Stat.Accuracy,
            Stat.Agility,
            Stat.AttackSpeed,
            Stat.Light,
            Stat.Strength,
            Stat.Luck,
            Stat.FireAttack,
            Stat.FireResistance,
            Stat.IceAttack,
            Stat.IceResistance,
            Stat.LightningAttack,
            Stat.LightningResistance,
            Stat.WindAttack,
            Stat.WindResistance,
            Stat.HolyAttack,
            Stat.HolyResistance,
            Stat.DarkAttack,
            Stat.DarkResistance,
            Stat.PhantomAttack,
            Stat.PhantomResistance,
            Stat.Comfort,
            Stat.LifeSteal,
            Stat.ExperienceRate,
            Stat.DropRate,
            Stat.SkillRate,
            Stat.PickUpRadius,
            Stat.Healing,
            Stat.HealingCap,
            Stat.ReflectDamage,
            Stat.HealthPercent,
            Stat.CriticalChance,
            Stat.MCPercent,
            //Stat.JudgementOfHeaven,
            Stat.BagWeight,
            Stat.WearWeight,
            Stat.HandWeight,
            Stat.GoldRate,
            Stat.DCPercent,
            Stat.SCPercent,
            Stat.PetDCPercent,
            //Stat.BossTracker,
            //Stat.PlayerTracker,
            Stat.CompanionRate,
            Stat.WeightRate,
            Stat.ManaPercent,
            Stat.MonsterExperience,
            Stat.MonsterGold,
            Stat.MonsterDrop,
            Stat.BaseExperienceRate,
            Stat.BaseGoldRate,
            Stat.BaseDropRate,
            Stat.CriticalDamage,
            Stat.PhysicalResistance,
            //Stat.FragmentRate,
            Stat.FrostBiteMaxDamage,
            Stat.ParalysisChance,
            Stat.SlowChance,
            Stat.SilenceChance,
            Stat.BlockChance,
            Stat.EvasionChance,
            Stat.PoisonResistance,
            //Stat.Rebirth,
            Stat.MagicEvade,
            Stat.ExtraDamage,
            Stat.CriticalHit,
            Stat.GreenPoison,
            Stat.RedPoison,
            Stat.LvConvertHPMP,
            Stat.MinBAC,
            Stat.MaxBAC,
            Stat.MinBC,
            Stat.MaxBC,
            //Stat.BHealth,
            //Stat.Blessing,
            Stat.ACPercent,
            Stat.MRPercent,
            Stat.FinalDamageReduce,
            Stat.FinalDamageReduceRate,
            Stat.BHealth,
            Stat.ACPercent,
            Stat.MRPercent,
            Stat.QuestDropRate,
            Stat.DieExtraDamage,
            Stat.LifeExtraDamage,
            Stat.FinalDamageReduce,
            Stat.FinalDamageReduceRate,
            Stat.Invincibility,
            Stat.SuperiorMagicShield,
            Stat.MiningSuccessRate,
            Stat.HPRegenRate,
            Stat.MPRegenRate,
            Stat.ACIgnoreRate,
            Stat.MRIgnoreRate,
            Stat.CriticalChanceResistance,
            Stat.ParalysisChanceResistance,
            Stat.SilenceChanceResistance,
            Stat.BlockChanceResistance,
            Stat.AbyssChance,
            Stat.AbyssChanceResistance,
            Stat.ExpelUndeadResistance,
            Stat.InvincibilityChance,
            Stat.WeponCriticalChance,

            //技能石头
            Stat.FlamingSwordHoist,
            Stat.DragonRiseHoist,
            Stat.BladeStormHoist,
            Stat.DestructiveSurgeHoist,
            Stat.SwiftBladeHoist,
            Stat.FireBallHoist,
            Stat.AdamantineFireBallHoist,
            Stat.ScortchedEarthHoist,
            Stat.FireWallHoist,
            Stat.FireStormHoist,
            Stat.MeteorShowerHoist,
            Stat.IceBoltHoist,
            Stat.IceBladesHoist,
            Stat.IceStormHoist,
            Stat.GreaterFrozenEarthHoist,
            Stat.LightningBallHoist,
            Stat.ThunderBoltHoist,
            Stat.LightningBeamHoist,
            Stat.LightningWaveHoist,
            Stat.ChainLightningHoist,
            Stat.ThunderStrikeHoist,
            Stat.GustBlastHoist,
            Stat.CycloneHoist,
            Stat.BlowEarthHoist,
            Stat.DragonTornadoHoist,
            Stat.ExplosiveTalismanHoist,
            Stat.EvilSlayerHoist,
            Stat.GreaterEvilSlayerHoist,
            Stat.ImprovedExplosiveTalismanHoist,
            Stat.FullBloomHoist,
            Stat.WhiteLotusHoist,
            Stat.RedLotusHoist,
            Stat.SweetBrierHoist,
            Stat.DragonRepulseHoist,
            Stat.FlashOfLightHoist,
        };

        //开放的宝石位置
        public static readonly IList<StatSource> GemList = new List<StatSource>()
        { StatSource.Gem1, StatSource.Gem2, StatSource.Gem3};

        /// <summary>
        ///自带头盔的衣服列表
        ///元素为image值
        /// </summary>
        public static readonly IList<int> ArmourWithHelmetList = new List<int>()
        {
            947,957,2810,2820,2872,2882,5340,5341,5342,5343,5344,5345,5346,5347,5350,5351,5352,5353,
        };

        /// <summary>
        ///不显示裸体的衣服列表
        ///元素为image值
        /// </summary>
        public static readonly IList<int> ArmourWithBodyList = new List<int>()
        {
            5344,5345,5346,5347,
        };

        /// <summary>
        ///自带武器的衣服列表
        ///元素为image值
        /// </summary>
        public static readonly IList<int> ArmourWithWeaponList = new List<int>()
        {
            5344,5345,5346,5347,
        };


        //状态类成就需求
        public static readonly IList<AchievementRequirementType> StatusAchievementRequirementTypeList =
            new List<AchievementRequirementType>()
            {
                AchievementRequirementType.None,
                AchievementRequirementType.UseMagic,
                AchievementRequirementType.InMap,
                AchievementRequirementType.WearingItem,
                AchievementRequirementType.CarryingItem,
                AchievementRequirementType.LevelLessThan,
                AchievementRequirementType.LevelGreaterOrEqualThan,
            };
        /// <summary>
        /// 新手行会名称
        /// </summary>
        public const string StarterGuildName = "新手行会";

        /// <summary>
        /// 新手行会名称
        /// </summary>
        public const string StarterGuildMember = "新成员";

        /// <summary>
        /// 最小密码长度
        /// </summary>
        public const int MinPasswordLength = 5;
        /// <summary>
        /// 最大密码长度
        /// </summary>
        public const int MaxPasswordLength = 15;
        /// <summary>
        /// 最小注册输入真实名字长度
        /// </summary>
        public const int MinRealNameLength = 2;
        /// <summary>
        /// 最大注册输入真实名字名字长度
        /// </summary>
        public const int MaxRealNameLength = 15;
        /// <summary>
        /// 最大邮箱账号长度
        /// </summary>
        public const int MaxEMailLength = 50;
        /// <summary>
        /// 最小角色名称长度
        /// </summary>
        public const int MinCharacterNameLength = 2;
        /// <summary>
        /// 最大角色名称长度
        /// </summary>
        public const int MaxCharacterNameLength = 5;
        /// <summary>
        /// 创建角色最大数
        /// </summary>
        public const int MaxCharacterCount = 2;
        /// <summary>
        /// 最小行会名字长度
        /// </summary>
        public const int MinGuildNameLength = 2;
        /// <summary>
        /// 最大行会名字长度
        /// </summary>
        public const int MaxGuildNameLength = 5;
        /// <summary>
        /// 最大聊天文字长度
        /// </summary>
        public const int MaxChatLength = 120;
        /// <summary>
        /// 最大行会公告长度
        /// </summary>
        public const int MaxGuildNoticeLength = 4000;
        /// <summary>
        /// 最大快捷栏计数格子数
        /// </summary>
        public const int MaxBeltCount = 6;
        /// <summary>
        /// 最大自动喝药计数行数
        /// </summary>
        public const int MaxAutoPotionCount = 14;
        /// <summary>
        /// 魔法范围格数
        /// </summary>
        public const int MagicRange = 10;
        /// <summary>
        /// 普通修理掉持久上限的比例
        /// </summary>
        public const int DurabilityLossRate = 15;
        /// <summary>
        /// 组队人数限制
        /// </summary>
        public const int GroupLimit = 15;
        /// <summary>
        /// 隐身范围
        /// </summary>
        public const int CloakRange = 3;
        /// <summary>
        /// 摆摊地点费用
        /// </summary>
        public const int MarketPlaceFee = 0;
        /// <summary>
        /// 首饰升级成本费用
        /// </summary>
        public const int AccessoryLevelCost = 0;
        /// <summary>
        /// 首饰重置成本费用
        /// </summary>
        public const int AccessoryResetCost = 1000000;
        /// <summary>
        /// 工艺武器升级成本费用
        /// </summary>
        public const int CraftWeaponPercentCost = 1000000;
        /// <summary>
        /// 通用工艺武器升级成本费用
        /// </summary>
        public const int CommonCraftWeaponPercentCost = 30000000;
        /// <summary>
        /// 高级工艺武器升级成本费用
        /// </summary>
        public const int SuperiorCraftWeaponPercentCost = 60000000;
        /// <summary>
        /// 大师工艺武器升级成本费用
        /// </summary>
        public const int EliteCraftWeaponPercentCost = 80000000;
        /// <summary>
        /// 合成技能书时每一点额外持久的费用
        /// </summary>
        public const int BookCombineFeePerDurability = 1000;
        /// <summary>
        /// 寄售税率
        /// </summary>
        public static decimal MarketPlaceTax = 0.07M;

        /// <summary>
        /// 大师精炼成本
        /// </summary>
        public static long MasterRefineCost = 50000;
        /// <summary>
        /// 大师评估成本
        /// </summary>
        public static long MasterRefineEvaluateCost = 250000;

        /// <summary>
        /// 制造等级 累计经验
        /// </summary>
        public static readonly IDictionary<int, int> CraftExpDict = new Dictionary<int, int>()
        {
            {0,0}
        };
        /// <summary>
        /// 游戏分辨率设置
        /// </summary>
        public static List<Size> ValidResolutions = new List<Size>
        {
            //new Size(320, 240),
            //new Size(400, 300),
            //new Size(512, 384),
            //new Size(640, 400),
            //new Size(640, 480),
            //new Size(720, 480),
            //new Size(720, 576),
            //new Size(800, 600),
            new Size(1024, 600),
            new Size(1024, 768),
            new Size(1280, 720),
            new Size(1280, 768),
            new Size(1280, 800),
            new Size(1280, 960),
            new Size(1280, 1024),
            new Size(1360, 768),
            new Size(1360, 1024),
            new Size(1366, 768),
            new Size(1440, 900),
            new Size(1600, 900),
            new Size(1680, 1050),
            //new Size(1920, 1080)
        };

        /// <summary>
        /// 背包格子初始值
        /// </summary>
        public const int InventorySize = 48;
        /// <summary>
        /// 碎片包裹格子初始值
        /// </summary>
        public const int PatchGridSize = 24;
        /// <summary>
        /// 人物装备格子初始值
        /// </summary>
        public const int EquipmentSize = 19;
        /// <summary>
        /// 宠物背包格子初始值
        /// </summary>
        public const int CompanionInventorySize = 1000;
        /// <summary>
        /// 宠物装备格子初始值
        /// </summary>
        public const int CompanionEquipmentSize = 4;
        /// <summary>
        /// 道具格子偏移值
        /// </summary>
        public const int EquipmentOffSet = 1000;
        /// <summary>
        /// 碎片格子偏移值
        /// </summary>
        public const int PatchOffSet = 2000;
        /// <summary>
        /// 钓鱼装备格子偏移值
        /// </summary>
        public const int FishingOffSet = 3000;
        /// <summary>
        /// 仓库格子初始值
        /// </summary>
        public const int StorageSize = 100;
        /// <summary>
        /// 钓鱼装备格子初始值
        /// </summary>
        public const int FishingEquipmentSize = 5;

        /// <summary>
        /// 是否需要填实名
        /// </summary>
        public static bool RealNameRequired = false;
        /// <summary>
        /// 是否需要填生日
        /// </summary>
        public static bool BirthDateRequired = false;

        /// <summary>
        /// 精炼时间设置
        /// </summary>
        public static Dictionary<RefineQuality, TimeSpan> RefineTimes = new Dictionary<RefineQuality, TimeSpan>
        {
            [RefineQuality.Rush] = TimeSpan.FromMinutes(1),
            [RefineQuality.Quick] = TimeSpan.FromMinutes(30),
            [RefineQuality.Standard] = TimeSpan.FromHours(1),
            [RefineQuality.Careful] = TimeSpan.FromHours(6),
            [RefineQuality.Precise] = TimeSpan.FromDays(1),
        };

        #region 钓鱼

        /// <summary>
        /// 最短钓鱼等待时间
        /// </summary>
        public static readonly TimeSpan MinFishingWaitingTime = TimeSpan.FromMilliseconds(5000);
        /// <summary>
        /// 最长钓鱼等待时间
        /// </summary>
        public static readonly TimeSpan MaxFishingWaitingTime = TimeSpan.FromMilliseconds(50000);
        /// <summary>
        /// 收杆窗口时间
        /// </summary>
        public static readonly TimeSpan FishingWindowTime = TimeSpan.FromMilliseconds(10000);
        /// <summary>
        /// 基础找到鱼的概率
        /// </summary>
        public static readonly int FishingBaseFindingChance = 10;
        /// <summary>
        /// 基础鱼咬钩的概率
        /// </summary>
        public static readonly int FishingBaseNibbleChance = 10;
        /// <summary>
        /// 寻鱼失败时成功率增加
        /// </summary>
        public static readonly int FishingBaseFindingFailedAdd = 10;


        #endregion

        /// <summary>
        /// 聊天道具信息正则表达式
        /// </summary>
        public static Regex RegexChatItemName = new Regex(@"<(.*?/.*?)>");
    }

    /// <summary>
    /// 选择角色信息
    /// </summary>
    public sealed class SelectInfo
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int CharacterIndex { get; set; }
        /// <summary>
        /// 角色名字
        /// </summary>
        public string CharacterName { get; set; }
        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 角色性别
        /// </summary>
        public MirGender Gender { get; set; }
        /// <summary>
        /// 角色职业
        /// </summary>
        public MirClass Class { get; set; }
        /// <summary>
        /// 角色目前位置
        /// </summary>
        public int Location { get; set; }
        /// <summary>
        /// 角色最后登录时间
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// 账号的状态
        /// </summary>
        public int CharacterState { get; set; }
    }

    /// <summary>
    /// 客户端相关控制
    /// </summary>
    public sealed class ClientControl
    {
        /// <summary>
        /// 是否远程仓库
        /// </summary>
        public bool OnRemoteStorage { get; set; }
        /// <summary>
        /// 是否显示挂机页
        /// </summary>
        public bool OnAutoHookTab { get; set; }
        /// <summary>
        /// 是否免蜡
        /// </summary>
        public bool OnBrightBox { get; set; }
        /// <summary>
        /// 是否免助跑
        /// </summary>
        public bool OnRunCheck { get; set; }
        /// <summary>
        /// 是否稳如泰山
        /// </summary>
        public bool OnRockCheck { get; set; }
        /// <summary>
        /// 是否清理尸体
        /// </summary>
        public bool OnClearBodyCheck { get; set; }
        /// <summary>
        /// 喝药延迟毫秒
        /// </summary>
        public int BigPatchAutoPotionDelayEdit { get; set; }
        /// <summary>
        /// 是否开启排行版显示
        /// </summary>
        public bool RankingShowCheck { get; set; }
        /// <summary>
        /// 是否开启观察者开关
        /// </summary>
        public bool ObserverSwitchCheck { get; set; }
        /// <summary>
        /// 是否开启爆率查询
        /// </summary>
        public bool RateQueryShowCheck { get; set; }
        /// <summary>
        /// 新版武器升级开关
        /// </summary>
        public bool NewWeaponUpgradeCheck { get; set; }
        /// <summary>
        /// 宠物包裹辅助喝药
        /// </summary>
        public bool AutoPotionForCompanion { get; set; }
        /// <summary>
        /// 充值开关
        /// </summary>
        public bool RechargeInterfaceCheck { get; set; }
        /// <summary>
        /// 行会创建费用
        /// </summary>
        public int GuildCreationCostEdit { get; set; }
        /// <summary>
        /// 行会成员扩展费用
        /// </summary>
        public int GuildMemberCostEdit { get; set; }
        /// <summary>
        /// 行会仓库扩展费用
        /// </summary>
        public int GuildStorageCostEdit { get; set; }
        /// <summary>
        /// 行会人数总上限
        /// </summary>
        public int GuildMemberHardLimitEdit { get; set; }
        /// <summary>
        /// 行会战费用
        /// </summary>
        public int GuildWarCostEdit { get; set; }
        /// <summary>
        /// 行会活跃度上限
        /// </summary>
        public int ActivationCeiling { get; set; }
        /// <summary>
        /// 个人捐赠金币对应活跃度1点
        /// </summary>
        public int PersonalGoldRatio { get; set; }
        /// <summary>
        /// 个人获得经验对应活跃度1点
        /// </summary>
        public int PersonalExpRatio { get; set; }
        /// <summary>
        /// 安全区是否消耗时间限制道具
        /// </summary>
        public bool InSafeZoneItemExpireCheck { get; set; }
        /// <summary>
        /// 道具是否能特修显示开关
        /// </summary>
        public bool ItemCanRepairCheck { get; set; }
        /// <summary>
        /// 新版首饰升级等级是否显示开关
        /// </summary>
        public bool JewelryLvShowsCheck { get; set; }
        /// <summary>
        /// 新版首饰升级经验是否显示开关
        /// </summary>
        public bool JewelryExpShowsCheck { get; set; }
        /// <summary>
        /// 新版首饰升级费用
        /// </summary>
        public int ACGoldRateCostEdit { get; set; }

        /// <summary>
        /// 攻击延迟1500
        /// </summary>
        public int GlobalsAttackDelay { get; set; }
        /// <summary>
        /// 攻击速度倍数47
        /// </summary>
        public int GlobalsASpeedRate { get; set; }
        /// <summary>
        /// 投掷物速度48
        /// </summary>
        public int GlobalsProjectileSpeed { get; set; }

        /// <summary>
        /// 转向时间
        /// </summary>
        public int GlobalsTurnTime { get; set; }
        /// <summary>
        /// 收割时间
        /// </summary>
        public int GlobalsHarvestTime { get; set; }
        /// <summary>
        /// 移动时间
        /// </summary>
        public int GlobalsMoveTime { get; set; }
        /// <summary>
        /// 攻击延迟1500
        /// <para>* 因为每次攻击都会有个站立的动作</para>
        /// <para>* 所以攻击延迟不能低于MirAnimation.Stance帧总延迟时间，否则会卡动作</para>
        /// </summary>
        public int GlobalsAttackTime { get; set; }
        /// <summary>
        /// 投掷时间
        /// </summary>
        public int GlobalsCastTime { get; set; }
        /// <summary>
        /// 施法时间延迟
        /// <para>* 因为每次施法都会有个举手或平推+站立的动作</para>
        /// <para>* 所以施法时间延迟不能低于MirAnimation.Stance + 举手或平推的帧总延迟时间，否则会卡动作</para>
        /// </summary>
        public int GlobalsMagicDelay { get; set; }
        /// <summary>
        /// 玩家视野
        /// </summary>
        public int MaxViewRange { get; set; }
        /// <summary>
        /// 是否显示地图附加属性
        /// </summary>
        public bool BufferMapEffectShow { get; set; }
        /// <summary>
        /// 泰山投币地图选择
        /// </summary>
        public bool CoinPlaceChoiceCheck { get; set; }
        /// <summary>
        /// 普通首饰成功几率值
        /// </summary>
        public int CommonItemSuccess { get; set; }
        /// <summary>
        /// 普通首饰成功几率降低值
        /// </summary>
        public int CommonItemReduce { get; set; }
        /// <summary>
        /// 高级首饰成功几率值
        /// </summary>
        public int SuperiorItemSuccess { get; set; }
        /// <summary>
        /// 高级首饰成功几率降低值
        /// </summary>
        public int SuperiorItemReduce { get; set; }
        /// <summary>
        /// 稀世首饰成功几率值
        /// </summary>
        public int EliteItemSuccess { get; set; }
        /// <summary>
        /// 稀世首饰成功几率降低值
        /// </summary>
        public int EliteItemReduce { get; set; }
        /// <summary>
        /// 商店出售道具是否能精炼
        /// </summary>
        public bool ShopNonRefinable { get; set; }
        /// <summary>
        /// 是否使用记忆传送功能
        /// </summary>
        public bool UseFixedPoint { get; set; }
    }
    /// <summary>
    /// 开始信息
    /// </summary>
    public sealed class StartInformation
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 对象ID
        /// </summary>
        public uint ObjectID { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 成就名称
        /// </summary>
        public string AchievementTitle { get; set; }
        /// <summary>
        /// 名字颜色
        /// </summary>
        public Color NameColour { get; set; }
        /// <summary>
        /// 行会名字
        /// </summary>
        public string GuildName { get; set; }
        /// <summary>
        /// 行会等级
        /// </summary>
        public string GuildRank { get; set; }
        /// <summary>
        /// 职业
        /// </summary>
        public MirClass Class { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public MirGender Gender { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public Point Location { get; set; }
        /// <summary>
        /// 朝向
        /// </summary>
        public MirDirection Direction { get; set; }
        /// <summary>
        /// 所在地图序号
        /// </summary>
        public int MapIndex { get; set; }

        /// <summary>
        /// 金币
        /// </summary>
        public long Gold { get; set; }
        /// <summary>
        /// 元宝
        /// </summary>
        public int GameGold { get; set; }
        /// <summary>
        /// 声望
        /// </summary>
        public int Prestige { get; set; }
        /// <summary>
        /// 贡献
        /// </summary>
        public int Contribute { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 头发类型
        /// </summary>
        public int HairType { get; set; }
        /// <summary>
        /// 头发颜色
        /// </summary>
        public Color HairColour { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public int Weapon { get; set; }
        /// <summary>
        /// 衣服
        /// </summary>
        public int Armour { get; set; }
        /// <summary>
        /// 盾牌
        /// </summary>
        public int Shield { get; set; }
        /// <summary>
        /// 徽章效果
        /// </summary>
        public int Emblem { get; set; }
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour { get; set; }
        /// <summary>
        /// 衣服图片
        /// </summary>
        public int ArmourImage { get; set; }
        /// <summary>
        /// 衣服序号
        /// </summary>
        public int ArmourIndex { get; set; }
        /// <summary>
        /// 武器序号
        /// </summary>
        public int WeaponIndex { get; set; }
        /// <summary>
        /// 武器图片
        /// </summary>
        public int WeaponImage { get; set; }

        /// <summary>
        /// 制造等级
        /// </summary>
        public int CraftLevel { get; set; }
        /// <summary>
        /// 制造熟练度
        /// </summary>
        public int CraftExp { get; set; }
        /// <summary>
        /// 制造完成时间
        /// </summary>
        public DateTime CraftFinishTime { get; set; }
        /// <summary>
        /// 制造快捷列表物品
        /// </summary>
        public int BookmarkedCraftItemInfoIndex { get; set; }
        /// <summary>
        /// 制作中的物品
        /// </summary>
        public int CraftingItemIndex { get; set; }

        /// <summary>
        /// 已做每日任务次数
        /// </summary>
        public int DailyQuestRemains { get; set; }
        /// <summary>
        /// 已做每日重复任务次数
        /// </summary>
        public int RepeatableQuestRemains { get; set; }

        /// <summary>
        /// 经验值
        /// </summary>
        public decimal Experience { get; set; }
        /// <summary>
        /// 当前HP
        /// </summary>
        public int CurrentHP { get; set; }
        /// <summary>
        /// 当前MP
        /// </summary>
        public int CurrentMP { get; set; }

        /// <summary>
        /// 攻击状态
        /// </summary>
        public AttackMode AttackMode { get; set; }
        /// <summary>
        /// 宠物攻击状态
        /// </summary>
        public PetMode PetMode { get; set; }

        /// <summary>
        /// 额外属性加点
        /// </summary>
        public int HermitPoints { get; set; }

        /// <summary>
        /// 日期时间
        /// </summary>
        public float DayTime { get; set; }
        /// <summary>
        /// 允许组队
        /// </summary>
        public bool AllowGroup { get; set; }
        /// <summary>
        /// 允许加好友
        /// </summary>
        public bool AllowFriend { get; set; }
        /// <summary>
        /// 允许复活
        /// </summary>
        public bool AllowResurrectionOrder { get; set; }

        /// <summary>
        /// 角色道具
        /// </summary>
        public List<ClientUserItem> Items { get; set; }
        /// <summary>
        /// 角色药品快捷栏
        /// </summary>
        public List<ClientBeltLink> BeltLinks { get; set; }
        /// <summary>
        /// 角色自动喝药栏
        /// </summary>
        public List<ClientAutoPotionLink> AutoPotionLinks { get; set; }
        /// <summary>
        /// 角色是否开启挂机
        /// </summary>
        public List<ClientAutoFightLink> AutoFightLinks { get; set; }
        /// <summary>
        /// 角色魔法技能
        /// </summary>
        public List<ClientUserMagic> Magics { get; set; }
        /// <summary>
        /// 角色BUFF状态
        /// </summary>
        public List<ClientBuffInfo> Buffs { get; set; }
        /// <summary>
        /// 角色中毒类别效果
        /// </summary>
        public PoisonType Poison { get; set; }

        /// <summary>
        /// 安全区
        /// </summary>
        public bool InSafeZone { get; set; }
        /// <summary>
        /// 观察者
        /// </summary>
        public bool Observable { get; set; }
        /// <summary>
        /// 死亡
        /// </summary>
        public bool Dead { get; set; }

        /// <summary>
        /// 坐骑类型
        /// </summary>
        public HorseType Horse { get; set; }

        /// <summary>
        /// 头盔形状
        /// </summary>
        public int HelmetShape { get; set; }
        /// <summary>
        /// 时装形状
        /// </summary>
        public int FashionShape { get; set; }
        /// <summary>
        /// 时装图像
        /// </summary>
        public int FashionImage { get; set; }
        /// <summary>
        /// 坐骑形状
        /// </summary>
        public int HorseShape { get; set; }
        /// <summary>
        /// 盾牌形状
        /// </summary>
        public int ShieldShape { get; set; }
        /// <summary>
        /// 徽章形状
        /// </summary>
        public int EmblemShape { get; set; }

        /// <summary>
        /// 角色任务
        /// </summary>
        public List<ClientUserQuest> Quests { get; set; }
        /// <summary>
        /// 角色成就任务
        /// </summary>
        public List<ClientUserAchievement> Achievements { get; set; }

        /// <summary>
        /// 宠物解锁列
        /// </summary>
        public List<int> CompanionUnlocks { get; set; }
        /// <summary>
        /// 可用宠物信息
        /// </summary>
        public List<CompanionInfo> AvailableCompanions = new List<CompanionInfo>();
        /// <summary>
        /// 角色宠物列表
        /// </summary>
        public List<ClientUserCompanion> Companions { get; set; }
        /// <summary>
        /// 宠物
        /// </summary>
        public int Companion { get; set; }
        /// <summary>
        /// 仓库容量
        /// </summary>
        public int StorageSize { get; set; }
        /// <summary>
        /// 碎片包裹容量
        /// </summary>
        public int PatchGridSize { get; set; }
        /// <summary>
        /// 技能经验设置
        /// </summary>
        public int SkillExpDrop { get; set; }
        /// <summary>
        /// 免费投币次数
        /// </summary>
        public int FreeTossCount { get; set; }

        public HorseType HorseType { get; set; }

        [CompleteObject]   //完整对象
        public void OnComplete()  //打开完成
        {
            foreach (int index in CompanionUnlocks)   //遍历数组 （宠物解锁中的ID索引）
                //可用宠物 添加（Globals里的宠物信息列表 结合 第一）
                AvailableCompanions.Add(Globals.CompanionInfoList.Binding.First(x => x.Index == index));
        }
    }

    /// <summary>
    /// 客户端角色道具
    /// </summary>
    public sealed class ClientUserItem
    {
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo Info;
        /// <summary>
        /// 完整的物品属性
        /// </summary>
        public List<FullItemStat> FullItemStats { get; set; }
        /// <summary>
        /// 道具索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 道具信息索引
        /// </summary>
        public int InfoIndex { get; set; }
        /// <summary>
        /// 当前持久
        /// </summary>
        public int CurrentDurability { get; set; }
        /// <summary>
        /// 最大持久
        /// </summary>
        public int MaxDurability { get; set; }
        /// <summary>
        /// 数值
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 格子链接
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 道具等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 道具经验
        /// </summary>
        public decimal Experience { get; set; }
        /// <summary>
        /// 道具颜色
        /// </summary>
        public Color Colour { get; set; }
        /// <summary>
        /// 道具特修冷却时间
        /// </summary>
        public TimeSpan SpecialRepairCoolDown { get; set; }
        /// <summary>
        /// 道具重置冷却时间
        /// </summary>
        public TimeSpan ResetCoolDown { get; set; }
        /// <summary>
        /// 是否新获得
        /// </summary>
        public bool New;
        /// <summary>
        /// 下一次特修时间
        /// </summary>
        public DateTime NextSpecialRepair;
        /// <summary>
        /// 下一次重置时间
        /// </summary>
        public DateTime NextReset;
        /// <summary>
        /// 增加的属性信息
        /// </summary>
        public Stats AddedStats { get; set; }
        /// <summary>
        /// 角色道具标识
        /// </summary>
        public UserItemFlags Flags { get; set; }
        /// <summary>
        /// 道具过期时间
        /// </summary>
        public TimeSpan ExpireTime { get; set; }
        /// <summary>
        /// 道具过期日期
        /// </summary>
        public DateTime ExpireDateTime { get; set; }

        public TimeSpan IllusionExpireTime { get; set; }

        public DateTime IllusionExpireDateTime { get; set; }
        /// <summary>
        /// 联盟行会名1
        /// </summary>
        public string Guild1Name { get; set; }
        /// <summary>
        /// 联盟行会名2
        /// </summary>
        public string Guild2Name { get; set; }
        /// <summary>
        /// 获得物品的时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 获得物品的地图
        /// </summary>
        public string SourceMap { get; set; }
        /// <summary>
        /// 获得物品的玩家
        /// </summary>
        public string OriginalOwner { get; set; }
        /// <summary>
        /// 获得物品的来源名称
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// 道具自定义前缀
        /// </summary>
        public string CustomPrefixText { get; set; }
        /// <summary>
        /// 道具自定义前缀颜色
        /// </summary>
        public Color CustomPrefixColor { get; set; }

        /// <summary>
        /// 回购类型
        /// </summary>
        public OwnerlessItemType OwnerlessType { get; set; }
        /// <summary>
        /// 是否精炼
        /// </summary>
        public bool Refine { get; set; }
        [IgnorePropertyPacket]      //忽略属性包
        public bool ExInfoOnly { get; set; }
        [IgnorePropertyPacket]      //忽略属性包
        public string ExInfo { get; set; }
        [IgnorePropertyPacket]      //忽略属性包
        public string LastName { get; set; }                            //
        [IgnorePropertyPacket]      //忽略属性包   
        public long LastPrice { get; set; }
        /// <summary>
        /// 负重
        /// </summary>
        [IgnorePropertyPacket]      //忽略属性包      
        public int Weight
        {
            get
            {
                switch (Info.ItemType)
                {
                    case ItemType.Poison:
                    case ItemType.Amulet:
                        return Info.Weight;
                    default:
                        return (int)Math.Min(int.MaxValue, Info.Weight * Count);
                }
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        [CompleteObject]
        public void Complete()
        {
            Info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);

            NextSpecialRepair = Time.Now + SpecialRepairCoolDown;
            NextReset = Time.Now + ResetCoolDown;
        }

        /// <summary>
        /// 客户端角色道具
        /// </summary>
        public ClientUserItem() { }

        /// <summary>
        /// 客户端角色道具信息数值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="count"></param>
        public ClientUserItem(ItemInfo info, long count)
        {
            Info = info;
            Count = info.Effect == ItemEffect.GameGold ? count / 100 : count;
            MaxDurability = info.Durability;
            CurrentDurability = info.Durability;
            Level = 1;
            AddedStats = new Stats();
        }

        /// <summary>
        /// 客户端角色道具数值 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public ClientUserItem(ClientUserItem item, long count)
        {
            Info = item.Info;

            Index = item.Index;
            InfoIndex = item.InfoIndex;

            CurrentDurability = item.CurrentDurability;
            MaxDurability = item.MaxDurability;

            Count = item.Info.Effect == ItemEffect.GameGold ? count / 100 : count;

            Slot = item.Slot;

            Level = item.Level;
            Experience = item.Experience;

            Colour = item.Colour;

            SpecialRepairCoolDown = item.SpecialRepairCoolDown;

            Flags = item.Flags;
            ExpireTime = item.ExpireTime;

            New = item.New;
            NextSpecialRepair = item.NextSpecialRepair;

            AddedStats = new Stats(item.AddedStats);

            //分割物品 继承物品来源
            CreationTime = item.CreationTime;
            SourceMap = item.SourceMap;
            SourceName = item.SourceName;
            OriginalOwner = item.OriginalOwner;

            CustomPrefixText = CustomPrefixText;
            CustomPrefixColor = CustomPrefixColor;
        }

        /// <summary>
        /// 出售价格
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public long Price(long count)
        {
            if ((Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return 0;

            decimal p = Info.Price;

            if (Info.Durability > 0)
            {
                decimal r = Info.Price / 2M / Info.Durability;

                p = MaxDurability * r;

                r = MaxDurability > 0 ? CurrentDurability / (decimal)MaxDurability : 0;

                p = Math.Floor(p / 2M + p / 2M * r + Info.Price / 2M);
            }

            p = p * (AddedStats.Count * 0.1M + 1M);

            //if (Info.Stats[Stat.SaleBonus20] > 0 && Info.Stats[Stat.SaleBonus20] <= count)
            //    p *= 1.2M;
            //else if (Info.Stats[Stat.SaleBonus15] > 0 && Info.Stats[Stat.SaleBonus15] <= count)
            //    p *= 1.15M;
            //else if (Info.Stats[Stat.SaleBonus10] > 0 && Info.Stats[Stat.SaleBonus10] <= count)
            //    p *= 1.1M;
            //else if (Info.Stats[Stat.SaleBonus5] > 0 && Info.Stats[Stat.SaleBonus5] <= count)
            //    p *= 1.05M;

            return (long)(p * count * Info.SellRate); // * 0.6M);
        }

        /// <summary>
        /// 道具修理费用
        /// </summary>
        /// <param name="special"></param>
        /// <returns></returns>
        public long RepairCost(bool special)
        {
            //if (Info.Durability == 0 || CurrentDurability >= MaxDurability) return 0;

            //int rate = special ? 2 : 1;

            //decimal p = Math.Floor(MaxDurability * (Info.Price / 2M / Info.Durability) + Info.Price / 2M);
            //p = p * (AddedStats.Count * 0.1M + 1M);

            //return (int)(p * Count - Price(Count)) * rate;

            if (Info.Durability == 0 || CurrentDurability >= MaxDurability) return 0;   //如果 道具持久等0  或者当前持久 大于等于 最大持久  不需要修理

            decimal rate = special ? 1M : 0.5M;  //倍率如果是特修就 1.5倍 否则就是正常价格

            decimal p = Math.Floor((MaxDurability - CurrentDurability) * (Info.Price / 1M / MaxDurability)); //+Info.Price / 2M
            //p = p * (AddedStats.Count * 0.1M + 1M);

            return (long)(p * rate); //*Count - Price(Count)
        }

        /// <summary>
        /// 可以升级的首饰道具
        /// </summary>
        /// <returns></returns>
        public bool CanAccessoryUpgrade()
        {
            switch (Info.ItemType)
            {
                case ItemType.Ring:
                case ItemType.Bracelet:
                case ItemType.Necklace:
                    break;
                default: return false;
            }
            return (Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && (Flags & UserItemFlags.Refinable) == UserItemFlags.Refinable;
        }

        /// <summary>
        /// 可以精炼的道具
        /// </summary>
        /// <returns></returns>
        public bool CanFragment()
        {
            if ((Flags & UserItemFlags.NonRefinable) == UserItemFlags.NonRefinable || (Flags & UserItemFlags.Worthless) == UserItemFlags.Worthless) return false;

            switch (Info.Rarity)
            {
                case Rarity.Common:
                    if (Info.RequiredAmount <= 15) return false;
                    break;
                case Rarity.Superior:
                    break;
                case Rarity.Elite:
                    break;
            }

            switch (Info.ItemType)
            {
                case ItemType.Weapon:
                case ItemType.Armour:
                case ItemType.Helmet:
                case ItemType.Necklace:
                case ItemType.Bracelet:
                case ItemType.Ring:
                case ItemType.Shoes:
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 可以精炼的成本
        /// </summary>
        /// <returns></returns>
        public int FragmentCost()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 9;
                        /* case ItemType.Helmet:
                         case ItemType.Necklace:
                         case ItemType.Bracelet:
                         case ItemType.Ring:
                         case ItemType.Shoes:
                             return Info.RequiredAmount * 7000 / 9;*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Info.RequiredAmount * 10000 / 2;
                        /*  case ItemType.Helmet:
                          case ItemType.Necklace:
                          case ItemType.Bracelet:
                          case ItemType.Ring:
                          case ItemType.Shoes:
                              return Info.RequiredAmount * 10000 / 10;*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armour:
                            return 250000;
                        case ItemType.Helmet:
                            return 50000;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 150000;
                        case ItemType.Shoes:
                            return 30000;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 精炼计数
        /// </summary>
        /// <returns></returns>
        public int FragmentCount()
        {
            switch (Info.Rarity)
            {
                case Rarity.Common:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*  case ItemType.Helmet:
                              return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                          case ItemType.Necklace:
                              return Math.Max(1, Info.RequiredAmount / 8);
                          case ItemType.Bracelet:
                              return Math.Max(1, Info.RequiredAmount / 15);
                          case ItemType.Ring:
                              return Math.Max(1, Info.RequiredAmount / 9);
                          case ItemType.Shoes:
                              return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Superior:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                        case ItemType.Helmet:
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                        case ItemType.Shoes:
                            return Math.Max(1, Info.RequiredAmount / 2 + 5);
                        /*  case ItemType.Helmet:
                              return Math.Max(1, (Info.RequiredAmount - 30) / 6);
                          case ItemType.Necklace:
                              return Math.Max(1, Info.RequiredAmount / 10);
                          case ItemType.Bracelet:
                              return Math.Max(1, Info.RequiredAmount / 15);
                          case ItemType.Ring:
                              return Math.Max(1, Info.RequiredAmount / 10);
                          case ItemType.Shoes:
                              return Math.Max(1, (Info.RequiredAmount - 35) / 6);*/
                        default:
                            return 0;
                    }
                case Rarity.Elite:
                    switch (Info.ItemType)
                    {
                        case ItemType.Armour:
                        case ItemType.Weapon:
                            return 50;
                        case ItemType.Helmet:
                            return 5;
                        case ItemType.Necklace:
                        case ItemType.Bracelet:
                        case ItemType.Ring:
                            return 10;
                        case ItemType.Shoes:
                            return 3;
                        default:
                            return 0;
                    }
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// 客户端药品快捷栏链接
    /// </summary>
    public sealed class ClientBeltLink
    {
        /// <summary>
        /// 物品快捷栏槽
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 链接信息索引
        /// </summary>
        public int LinkInfoIndex { get; set; }
        /// <summary>
        /// 链接道具索引
        /// </summary>
        public int LinkItemIndex { get; set; }
    }

    /// <summary>
    /// 客户端自动喝药栏链接
    /// </summary>
    public sealed class ClientAutoPotionLink
    {
        /// <summary>
        /// 自动喝药栏槽
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 链接信息索引
        /// </summary>
        public int LinkInfoIndex { get; set; }
        /// <summary>
        /// 加红
        /// </summary>
        public int Health { get; set; }
        /// <summary>
        /// 加蓝
        /// </summary>
        public int Mana { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 客户端自动挂机链接
    /// </summary>
    public sealed class ClientAutoFightLink
    {
        /// <summary>
        /// 自动辅助设置槽
        /// </summary>
        public AutoSetConf Slot { get; set; }
        /// <summary>
        /// 自动辅助魔法技能索引
        /// </summary>
        public MagicType MagicIndex { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 时间是否有效
        /// </summary>
        public int TimeCount { get; set; }
    }

    /// <summary>
    /// 客户端用户魔法技能
    /// </summary>
    public class ClientUserMagic
    {
        /// <summary>
        /// 技能索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 技能信息索引
        /// </summary>
        public int InfoIndex { get; set; }
        /// <summary>
        /// 魔法技能信息
        /// </summary>
        public MagicInfo Info;

        /// <summary>
        /// 快捷栏1
        /// </summary>
        public SpellKey Set1Key { get; set; }
        /// <summary>
        /// 快捷栏2
        /// </summary>
        public SpellKey Set2Key { get; set; }
        /// <summary>
        /// 快捷栏3
        /// </summary>
        public SpellKey Set3Key { get; set; }
        /// <summary>
        /// 快捷栏4
        /// </summary>
        public SpellKey Set4Key { get; set; }
        /// <summary>
        /// 魔法技能等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 魔法技能经验值
        /// </summary>
        public long Experience { get; set; }
        /// <summary>
        /// 技能主动被动标签
        /// </summary>
        public MagicAction Action { get; set; }
        /// <summary>
        /// 魔法技能冷却时间
        /// </summary>
        public TimeSpan Cooldown { get; set; }
        /// <summary>
        /// 魔法技能下一次释放时间
        /// </summary>
        public DateTime NextCast;


        /// <summary>
        /// 魔法技能释放计数
        /// </summary>
        [IgnorePropertyPacket]
        public int Cost => Info.BaseCost + Level * Info.LevelCost / 3;
        /// <summary>
        /// 魔法技能下一次释放时间计数
        /// </summary>
        [CompleteObject]
        public void Complete()
        {
            NextCast = Time.Now + Cooldown;
            Info = Globals.MagicInfoList.Binding.FirstOrDefault(x => x.Index == InfoIndex);
        }
    }


    public sealed class NPCCustomBG
    {
        public string url { get; set; } = "";
        public string title { get; set; } = "";
        public int file { get; set; } = -1;
        public int idx { get; set; } = -1;
        public int size_w { get; set; } = 0;
        public int size_h { get; set; } = 0;
        public int drag { get; set; } = 0;
        public int center { get; set; } = 0;
        public int offset_x { get; set; } = 0;
        public int offset_y { get; set; } = 0;
        public int close { get; set; } = 1;
        public int close_offset_x { get; set; } = 0;
        public int close_offset_y { get; set; } = 0;
    }
    public sealed class NPCCustomFont
    {
        public string color { get; set; } = "";
        public int size { get; set; } = 9;
        public int offset_x { get; set; } = 0;
        public int offset_y { get; set; } = 0;

    }
    public sealed class NPCCustomNeedItems
    {
        public int pos_x { get; set; } = 0;
        public int pos_y { get; set; } = 0;
        public int file { get; set; } = -1;
        public int idx { get; set; } = -1;
        public int dragidx { get; set; } = 0;
    }
    /// <summary>
    /// NPC商品
    /// </summary>
    public sealed class ClientNPCGood
    {
        /// <summary>
        /// NPC商品索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// NPC道具名字
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// NPC道具费率
        /// </summary>
        public decimal Rate { get; set; }

        public decimal RealRate;
        /// <summary>
        /// NPC道具信息
        /// </summary>
        public ItemInfo Item;
        /// <summary>
        /// NPC货币=金币
        /// </summary>
        public string Currency { get; set; } = Globals.Currency;
        /// <summary>
        /// NPC货币成本
        /// </summary>
        public int CurrencyCost { get; set; } = 0;
        /// <summary>
        /// NPC商品成本
        /// </summary>
        public int Cost;

        public int RealCost;
        /// <summary>
        /// 回购道具需要显示全部属性
        /// </summary>
        public ClientUserItem UserItem { get; set; }
    }

    public interface INPCPage
    {

        NPCDialogType DialogType { get; set; }
        List<ClientNPCGood> Goods { get; set; }
        List<ItemType> Types { get; set; }
        /// <summary>
        /// txt脚本逻辑,PY脚本不使用
        /// </summary>
        List<INPCSegment> SegmentList { get; set; }
    }

    public interface INPCSegment
    {

    }
    /// <summary>
    /// 客户端NPC页面
    /// </summary>
    public sealed class ClientNPCPage : INPCPage
    {
        /// <summary>
        /// 客户端NPC说话
        /// </summary>
        public string Say { get; set; }
        /// <summary>
        /// NPC类型
        /// </summary>
        public NPCDialogType DialogType { get; set; }
        /// <summary>
        /// 默认不开启以物换物
        /// </summary>
        public int BarterFlag { get; set; } = 0;
        /// <summary>
        /// NPC商品
        /// </summary>
        public List<ClientNPCGood> Goods { get; set; }
        /// <summary>
        /// NPC道具类型
        /// </summary>
        public List<ItemType> Types { get; set; }

        public string CastleName { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal TaxRate { get; set; }

        /// <summary>
        /// 用户自定义对话框背景效果
        /// </summary>
        public NPCCustomBG bg { get; set; } = null;
        /// <summary>
        /// 用户自定义字体效果
        /// </summary>
        public NPCCustomFont font { get; set; } = null;
        /// <summary>
        /// 用户自定义需求物品框
        /// /// </summary>
        public List<NPCCustomNeedItems> needItems { get; set; } = new List<NPCCustomNeedItems>();

        [Obsolete]
        public List<INPCSegment> SegmentList { get; set; }

        [CompleteObject]
        public void Complete()
        {
            for (var i = 0; i < Goods.Count; i++)
            {
                Goods[i].Item = Globals.ItemInfoList.Binding.First(x => x.ItemName == Goods[i].ItemName);
                Goods[i].Cost = (int)Math.Round(Goods[i].Item.Price * Goods[i].Rate);
            }
        }
    }

    /// <summary>
    /// D键菜单页
    /// </summary>
    public sealed class DKeyPage
    {
        /// <summary>
        /// D键文本信息
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// D键文本X坐标
        /// </summary>
        public int labelLocationX { get; set; }
        /// <summary>
        /// D键文本Y坐标
        /// </summary>
        public int labelLocationY { get; set; }
        /// <summary>
        /// D键目标NPC索引
        /// </summary>
        public uint targetNPCIndex { get; set; }
        /// <summary>
        /// D键文本颜色
        /// </summary>
        public Color textColor { get; set; }
    }

    /// <summary>
    /// 单元格链接信息
    /// </summary>
    public class CellLinkInfo
    {
        /// <summary>
        /// 单元格格子类型
        /// </summary>
        public GridType GridType { get; set; }
        /// <summary>
        /// 单元格
        /// </summary>
        public int Slot { get; set; }
        /// <summary>
        /// 单元格计数
        /// </summary>
        public long Count { get; set; }
    }

    /// <summary>
    /// 客户端BUFF信息
    /// </summary>
    public class ClientBuffInfo
    {
        /// <summary>
        /// BUFF索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// BUFF类型
        /// </summary>
        public BuffType Type { get; set; }
        /// <summary>
        /// BUFF剩余时间
        /// </summary>
        public TimeSpan RemainingTime { get; set; }
        /// <summary>
        /// BUFF滴答频率时间
        /// </summary>
        public TimeSpan TickFrequency { get; set; }
        /// <summary>
        /// BUFF增加属性状态
        /// </summary>
        public Stats Stats { get; set; }
        /// <summary>
        /// BUFF暂停
        /// </summary>
        public bool Pause { get; set; }
        /// <summary>
        /// BUFF道具索引
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// 自定义BUFF的索引
        /// </summary>
        public int FromCustomBuff { get; set; }
    }

    /// <summary>
    /// 客户端精炼信息
    /// </summary>
    public class ClientRefineInfo
    {
        /// <summary>
        /// 精炼信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 精炼武器
        /// </summary>
        public ClientUserItem Weapon { get; set; }
        /// <summary>
        /// 精炼类型
        /// </summary>
        public RefineType Type { get; set; }
        /// <summary>
        /// 精炼时间
        /// </summary>
        public RefineQuality Quality { get; set; }
        /// <summary>
        /// 精炼几率
        /// </summary>
        public int Chance { get; set; }
        /// <summary>
        /// 最大精炼几率
        /// </summary>
        public int MaxChance { get; set; }
        /// <summary>
        /// 精炼持续时间
        /// </summary>
        public TimeSpan ReadyDuration { get; set; }
        /// <summary>
        /// 精炼取回时间
        /// </summary>
        public DateTime RetrieveTime;

        [CompleteObject]
        public void Complete()
        {
            RetrieveTime = Time.Now + ReadyDuration;
        }
    }

    /// <summary>
    /// 排行榜信息
    /// </summary>
    public sealed class RankInfo
    {
        /// <summary>
        /// 排行
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// 排行索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 排行名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排行职业
        /// </summary>
        public MirClass Class { get; set; }
        /// <summary>
        /// 排行等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 排行经验
        /// </summary>
        public decimal Experience { get; set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool Online { get; set; }
        /// <summary>
        /// 是否开启观察开关
        /// </summary>
        public bool Observable { get; set; }
        /// <summary>
        /// 排行转生
        /// </summary>
        public int Rebirth { get; set; }
    }

    /// <summary>
    /// 客户端商城信息
    /// </summary>
    public class ClientMarketPlaceInfo
    {
        /// <summary>
        /// 商城信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 商城道具
        /// </summary>
        public ClientUserItem Item { get; set; }
        /// <summary>
        /// 商城销售价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 销售价格类型
        /// </summary>
        public CurrencyType PriceType { get; set; }
        /// <summary>
        /// 商城卖方
        /// </summary>
        public string Seller { get; set; }
        /// <summary>
        /// 商城信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 商品所有者
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// 附加费用
        /// </summary>
        public bool Loading;
        /// <summary>
        /// 上架时间
        /// </summary>
        public DateTime CreatTime { get; set; }
    }

    /// <summary>
    /// 客户端邮件信息
    /// </summary>
    public class ClientMailInfo
    {
        /// <summary>
        /// 邮件信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 邮件是否阅读
        /// </summary>
        public bool Opened { get; set; }
        /// <summary>
        /// 邮件附带道具
        /// </summary>
        public bool HasItem { get; set; }
        /// <summary>
        /// 邮件时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 邮件发件人
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 邮件附带金额
        /// </summary>
        public int Gold { get; set; }
        /// <summary>
        /// 邮件道具
        /// </summary>
        public List<ClientUserItem> Items { get; set; }
    }
    public class ClientGoldMarketInfo
    {
        public long GoldPrice { get; set; }
        public long Count { get; set; }

    }
    public class ClientGoldMarketMyOrderInfo
    {
        public int Index { get; set; }
        public long GoldPrice { get; set; }
        public long Count { get; set; }
        public DateTime Date { get; set; }
        public StockOrderType TradeState { get; set; }
        public long CompletedCount { get; set; }
        public TradeType TradeType { get; set; }
    }
    /// <summary>
    /// 客户端好友信息
    /// </summary>
    public class ClientFriendInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 增加时间
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Character { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string LinkID { get; set; }
        /// <summary>
        /// 添加在线标识符
        /// </summary>
        public bool Online { get; set; }
    }

    /// <summary>
    /// 客户端行会信息
    /// </summary>
    public class ClientGuildInfo
    {
        /// <summary>
        /// 行会名字
        /// </summary>
        public string GuildName { get; set; }
        /// <summary>
        /// 行会旗帜
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 行会旗帜颜色
        /// </summary>
        public Color FlagColor { get; set; }
        /// <summary>
        /// 行会公共
        /// </summary>
        public string Notice { get; set; }
        /// <summary>
        /// 行会成员列表
        /// </summary>
        public int MemberLimit { get; set; }
        /// <summary>
        /// 行会资金
        /// </summary>
        public long GuildFunds { get; set; }
        /// <summary>
        /// 行会每日资金增长
        /// </summary>
        public long DailyGrowth { get; set; }
        /// <summary>
        /// 行会捐款总数
        /// </summary>
        public long TotalContribution { get; set; }
        /// <summary>
        /// 行会每日捐款数额
        /// </summary>
        public long DailyContribution { get; set; }
        /// <summary>
        /// 行会成员索引
        /// </summary>
        public int UserIndex { get; set; }
        /// <summary>
        /// 行会仓库列表
        /// </summary>
        public int StorageLimit { get; set; }
        /// <summary>
        /// 行会税率
        /// </summary>
        public int Tax { get; set; }
        /// <summary>
        /// 行会默认等级
        /// </summary>
        public string DefaultRank { get; set; }
        /// <summary>
        /// 行会默认权限
        /// </summary>
        public GuildPermission DefaultPermission { get; set; }
        /// <summary>
        /// 是否允许申请加入
        /// </summary>
        public bool AllowApply { get; set; }
        /// <summary>
        /// 行会成员信息
        /// </summary>
        public List<ClientGuildMemberInfo> Members { get; set; }
        /// <summary>
        /// 行会仓库道具
        /// </summary>
        public List<ClientUserItem> Storage { get; set; }
        /// <summary>
        /// 行会联盟信息
        /// </summary>
        public List<ClientGuildAllianceInfo> Alliances { get; set; }
        /// <summary>
        /// 金库公告
        /// </summary>
        public string VaultNotice { get; set; }
        /// <summary>
        /// 金库上限金额
        /// </summary>
        public long MaxFund { get; set; }
        /// <summary>
        /// 捐赠信息
        /// </summary>
        public List<ClientGuildFundChangeInfo> FundChanges { get; set; }
        /// <summary>
        /// 捐献排名
        /// </summary>
        public List<ClientGuildFundRankInfo> FundRanks { get; set; }
        /// <summary>
        /// 行会等级
        /// </summary>
        public int GuildLevel { get; set; }

        public long GuildExp { get; set; }

        public long ActiveCount { get; set; }

        public long DailyActiveCount { get; set; }

        /// <summary>
        /// 自己今日贡献
        /// </summary>
        public long OwnDailyActiveCount { get; set; }

        /// <summary>
        /// 自己总贡献度
        /// </summary>
        public long OwnTotalActiveCount { get; set; }

        [IgnorePropertyPacket]
        public GuildPermission Permission => Members.FirstOrDefault(x => x.Index == UserIndex)?.Permission ?? GuildPermission.None;
    }

    /// <summary>
    /// 客户端行会成员信息
    /// </summary>
    public class ClientGuildMemberInfo
    {
        /// <summary>
        /// 行会成员信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 角色Index
        /// </summary>
        public int PlayerIndex { get; set; }
        /// <summary>
        /// 行会成员名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 行会成员分级
        /// </summary>
        public string Rank { get; set; }
        /// <summary>
        /// 行会成员捐款总额
        /// </summary>
        public long TotalContribution { get; set; }
        /// <summary>
        /// 行会成员每日捐款数额
        /// </summary>
        public long DailyContribution { get; set; }

        public long TotalActiveCount { get; set; }

        public long DailyActiveCount { get; set; }
        /// <summary>
        /// 行会成员在线时间
        /// </summary>
        public TimeSpan Online { get; set; }
        /// <summary>
        /// 行会成员权限许可
        /// </summary>
        public GuildPermission Permission { get; set; }
        /// <summary>
        /// 行会成员离线时间
        /// </summary>
        public DateTime LastOnline;
        /// <summary>
        /// 对象ID
        /// </summary>
        public uint ObjectID { get; set; }

        [CompleteObject]
        public void Complete()
        {
            if (Online == TimeSpan.MinValue)
                LastOnline = DateTime.MaxValue;
            else
                LastOnline = Time.Now - Online;
        }
    }

    /// <summary>
    /// 客户端行会联盟信息
    /// </summary>
    public class ClientGuildAllianceInfo
    {
        /// <summary>
        /// 行会联盟信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 联盟名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联盟在线计数
        /// </summary>
        public int OnlineCount { get; set; }
    }
    /// <summary>
    /// 客户端行会捐赠信息
    /// </summary>
    public class ClientGuildFundChangeInfo
    {
        public string GuildName { get; set; }
        public string Name { get; set; }
        public long Amount { get; set; }
        public DateTime OperationTime { get; set; }
    }

    public class ClientGuildFundRankInfo
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public long TotalAmount { get; set; }
    }

    /// <summary>
    /// 客户端角色任务
    /// </summary>
    public class ClientUserQuest
    {
        /// <summary>
        /// 任务索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 任务信息
        /// </summary>
        [IgnorePropertyPacket]
        public QuestInfo Quest { get; set; }
        /// <summary>
        /// 任务道具索引
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// 任务开始道具
        /// </summary>
        [IgnorePropertyPacket]
        public ItemInfo StartItem { get; set; }
        /// <summary>
        /// 任务索引值
        /// </summary>
        public int QuestIndex { get; set; }
        /// <summary>
        /// 是否跟踪任务进度
        /// </summary>
        public bool Track { get; set; }
        /// <summary>
        /// 任务完成
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// 选择奖励
        /// </summary>
        public int SelectedReward { get; set; }
        /// <summary>
        /// 任务额外信息
        /// </summary>
        public string ExtraInfo { get; set; }
        /// <summary>
        /// 已完成的任务
        /// </summary>
        [IgnorePropertyPacket]
        public bool IsComplete => Tasks.Count == Quest.Tasks.Count && Tasks.All(x => x.Completed);

        public List<ClientUserQuestTask> Tasks { get; set; }

        [CompleteObject]
        public void Complete()
        {
            Quest = Globals.QuestInfoList.Binding.First(x => x.Index == QuestIndex);
            StartItem = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == ItemIndex);
        }
    }

    /// <summary>
    /// 客户端角色完成任务
    /// </summary>
    public class ClientUserQuestTask
    {
        /// <summary>
        /// 完成任务索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 任务完成
        /// </summary>
        [IgnorePropertyPacket]
        public QuestTask Task { get; set; }
        /// <summary>
        /// 任务完成索引
        /// </summary>
        public int TaskIndex { get; set; }
        /// <summary>
        /// 任务完成数量
        /// </summary>
        public long Amount { get; set; }

        [IgnorePropertyPacket]
        public bool Completed => Amount >= Task.Amount;

        [CompleteObject]
        public void Complete()
        {
            Task = Globals.QuestTaskList.Binding.First(x => x.Index == TaskIndex);
        }
    }

    /// <summary>
    /// 客户端角色成就
    /// </summary>
    public class ClientUserAchievement
    {
        /// <summary>
        /// 成就索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 成就信息
        /// </summary>
        [IgnorePropertyPacket]
        public AchievementInfo Achievement { get; set; }
        /// <summary>
        /// 成就信息索引
        /// </summary>
        public int AchievementIndex { get; set; }
        /// <summary>
        /// 成就完成
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// 成就要求
        /// </summary>
        public List<ClientUserAchievementRequirement> Requirements { get; set; }
        /// <summary>
        /// 成就完成数据
        /// </summary>
        public string CompleteDate { get; set; }

        [IgnorePropertyPacket]
        public bool IsComplete => Requirements.Count == Achievement.AchievementRequirements.Count && Requirements.All(x => x.Completed);

        [CompleteObject]
        public void Complete()
        {
            Achievement = Globals.AchievementInfoList.Binding.First(x => x.Index == AchievementIndex);
        }
    }

    /// <summary>
    /// 成就要求
    /// </summary>
    public class ClientUserAchievementRequirement
    {
        /// <summary>
        /// 成就需求索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 成就需求
        /// </summary>
        [IgnorePropertyPacket]
        public AchievementRequirement Requirement { get; set; }
        /// <summary>
        /// 成就需求索引
        /// </summary>
        public int RequirementIndex { get; set; }
        /// <summary>
        /// 成就当前值
        /// </summary>
        public decimal CurrentValue { get; set; }

        [IgnorePropertyPacket]
        public bool Completed => Globals.StatusAchievementRequirementTypeList.Contains(Requirement.RequirementType) ||
                                 (!Requirement.Reverse && CurrentValue >= Requirement.RequiredAmount) ||
                                 (Requirement.Reverse && CurrentValue <= Requirement.RequiredAmount);

        [CompleteObject]
        public void Complete()
        {
            Requirement = Globals.AchievementRequirementList.Binding.First(x => x.Index == RequirementIndex);
        }
    }

    /// <summary>
    /// 客户端宠物对象
    /// </summary>
    public class ClientCompanionObject
    {
        /// <summary>
        /// 宠物名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 宠物头部形状
        /// </summary>
        public int HeadShape { get; set; }
        /// <summary>
        /// 宠物背部形状
        /// </summary>
        public int BackShape { get; set; }
    }
    /// <summary>
    /// 客户端宠物捡取设置
    /// </summary>
    public class ClientCompanionSortSet
    {
        /// <summary>
        /// 宠物捡取道具类型
        /// </summary>
        public ItemType SetType { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 客户端宠物捡取类别设置
    /// </summary>
    public class ClientCompanionSortLevSet
    {
        /// <summary>
        /// 宠物捡取道具类别
        /// </summary>
        public Rarity SetType { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// 客户端角色宠物
    /// </summary>
    public class ClientUserCompanion
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 宠物名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 宠物索引
        /// </summary>
        public int CompanionIndex { get; set; }
        /// <summary>
        /// 宠物信息
        /// </summary>
        public CompanionInfo CompanionInfo;
        /// <summary>
        /// 宠物等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 宠物饥饿度
        /// </summary>
        public int Hunger { get; set; }
        /// <summary>
        /// 宠物经验
        /// </summary>
        public int Experience { get; set; }
        /// <summary>
        /// 宠物等级属性3
        /// </summary>
        public Stats Level3 { get; set; }
        /// <summary>
        /// 宠物等级属性5
        /// </summary>
        public Stats Level5 { get; set; }
        /// <summary>
        /// 宠物等级属性7
        /// </summary>
        public Stats Level7 { get; set; }
        /// <summary>
        /// 宠物等级属性10
        /// </summary>
        public Stats Level10 { get; set; }
        /// <summary>
        /// 宠物等级属性11
        /// </summary>
        public Stats Level11 { get; set; }
        /// <summary>
        /// 宠物等级属性13
        /// </summary>
        public Stats Level13 { get; set; }
        /// <summary>
        /// 宠物等级属性15
        /// </summary>
        public Stats Level15 { get; set; }
        /// <summary>
        /// 宠物自动粮仓  默认关闭
        /// </summary>
        public bool AutoFeed { get; set; } = false;
        /// <summary>
        /// 宠物主人名字
        /// </summary>
        public string CharacterName { get; set; }

        /// <summary>
        /// 角色道具
        /// </summary>
        public List<ClientUserItem> Items { get; set; }
        /// <summary>
        /// 宠物捡取道具分类设置
        /// </summary>
        public List<ClientCompanionSortSet> Sorts { get; set; }
        /// <summary>
        /// 宠物捡取道具类别设置
        /// </summary>
        public List<ClientCompanionSortLevSet> SortsLev { get; set; }
        /// <summary>
        /// 宠物捡取道具分类设定
        /// </summary>
        public bool[] SortingArry = new bool[(int)ItemType.Medicament + 1];
        /// <summary>
        /// 宠物捡取道具类别设定
        /// </summary>
        public bool[] SortingLevArry = new bool[(int)Rarity.Elite + 1];
        /// <summary>
        /// 宠物装备背包格子设定
        /// </summary>
        public ClientUserItem[] EquipmentArray = new ClientUserItem[Globals.CompanionEquipmentSize], InventoryArray = new ClientUserItem[Globals.CompanionInventorySize];


        [CompleteObject]
        public void OnComplete()
        {
            CompanionInfo = Globals.CompanionInfoList.Binding.First(x => x.Index == CompanionIndex);

            foreach (ClientUserItem item in Items)
            {
                if (item.Slot < Globals.EquipmentOffSet)
                    InventoryArray[item.Slot] = item;
                else
                    EquipmentArray[item.Slot - Globals.EquipmentOffSet] = item;
            }
            foreach (ClientCompanionSortSet sort in Sorts)
            {
                SortingArry[(int)sort.SetType] = sort.Enabled;
            }

            foreach (ClientCompanionSortLevSet sortlev in SortsLev)
            {
                SortingLevArry[(int)sortlev.SetType] = sortlev.Enabled;
            }
        }

    }

    /// <summary>
    /// 客户端玩家信息
    /// </summary>
    public class ClientPlayerInfo
    {
        /// <summary>
        /// 对象索引
        /// </summary>
        public uint ObjectID { get; set; }
        /// <summary>
        /// 玩家名字
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 玩家对象数据
    /// </summary>
    public class ClientObjectData
    {
        /// <summary>
        /// 对象索引
        /// </summary>
        public uint ObjectID;
        /// <summary>
        /// 地图索引
        /// </summary>
        public int MapIndex;
        /// <summary>
        /// 坐标
        /// </summary>
        public Point Location;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo MonsterInfo;
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo ItemInfo;
        /// <summary>
        /// 宠物主人
        /// </summary>
        public string PetOwner;
        /// <summary>
        /// 血量
        /// </summary>
        public int Health;
        /// <summary>
        /// 最大血值
        /// </summary>
        public int MaxHealth;
        /// <summary>
        /// 蓝量
        /// </summary>
        public int Mana;
        /// <summary>
        /// 最大蓝值
        /// </summary>
        public int MaxMana;
        /// <summary>
        /// 属性状态
        /// </summary>
        public Stats Stats { get; set; }
        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool Dead;
    }

    /// <summary>
    /// 客户端黑名单信息
    /// </summary>
    public class ClientBlockInfo
    {
        /// <summary>
        /// 黑名单信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 黑名单名字
        /// </summary>
        public string Name { get; set; }
    }

    public class FixeUnit
    {
        public FixeUnit() { }

        public FixeUnit(int Index, Point Postion)
        {
            QuestIndex = Index;
            Pos = Postion;
        }
        public int QuestIndex { get; set; }
        public Point Pos { get; set; }

        public static bool operator !=(FixeUnit a1, FixeUnit a2)
        {
            if ((a1 as object) != null)
                return !a1.Equals(a2);
            else
                return (a2 as object) != null;
        }
        public static bool operator ==(FixeUnit a1, FixeUnit a2)
        {
            if ((a1 as object) != null)
                return a1.Equals(a2);
            else
                return (a2 as object) == null;
        }

        public override int GetHashCode()
        {
            return QuestIndex;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            FixeUnit a2 = obj as FixeUnit;
            if ((System.Object)a2 == null)
            {
                return false;
            }
            if (QuestIndex != a2.QuestIndex || Pos != a2.Pos)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 客户端记录坐标信息
    /// </summary>
    public class ClientFixedPointInfo
    {
        /// <summary>
        /// 地点
        /// </summary>
        public FixeUnit Uind { get; set; }
        /// <summary>
        /// 地点名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地点名字颜色
        /// </summary>
        public Color NameColour { get; set; }
    }

    /// <summary>
    /// 客户端财富信息
    /// </summary>
    public class ClientFortuneInfo
    {
        /// <summary>
        /// 道具索引
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo ItemInfo;
        /// <summary>
        /// 检查时间
        /// </summary>
        public TimeSpan CheckTime { get; set; }
        /// <summary>
        /// 爆率记录
        /// </summary>
        public long DropCount { get; set; }
        /// <summary>
        /// 进展
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 检查数据
        /// </summary>
        public DateTime CheckDate;

        [CompleteObject]
        public void OnComplete()
        {
            ItemInfo = Globals.ItemInfoList.Binding.First(x => x.Index == ItemIndex);

            CheckDate = Time.Now - CheckTime;
        }
    }

    /// <summary>
    /// 地图门点标记
    /// </summary>
    public class Door
    {
        /// <summary>
        /// 门点标记序号
        /// </summary>
        public byte index;
        /// <summary>
        /// 0: closed, 1: opening, 2: open, 3: closing  门的状态 0:关闭，1:打开，2:打开，3:关闭
        /// </summary>
        public byte DoorState;
        /// <summary>
        /// 门点图片序号
        /// </summary>
        public byte ImageIndex;
        /// <summary>
        /// 门点最后标记
        /// </summary>
        public long LastTick;
        /// <summary>
        /// 门点位置
        /// </summary>
        public Point Location;
    }

    /// <summary>
    /// 客户端传给服务端的聊天框道具信息
    /// </summary>
    public class ChatItemInfo
    {
        /// <summary>
        /// 用户道具索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 道具格子类别
        /// </summary>
        public GridType GridType { get; set; }
        /// <summary>
        /// 需要匹配的正则道具名
        /// </summary>
        public string RegexInternalName { get; set; }
        /// <summary>
        /// 替换后的道具名
        /// </summary>
        public string InternalName { get; set; }

    }

    /// <summary>
    /// 客户端用的 奖金池信息
    /// </summary>
    public class ClientRewardPoolInfo
    {
        public string CurrencyName { get; set; }
        public decimal Balance { get; set; }
        public int PoolMax { get; set; }
        public int CurrentTier { get; set; }
        public int CurrentLowerLimit { get; set; }
        public int CurrentUpperLimit { get; set; }
    }
    /// <summary>
    /// 奖金池排行榜信息
    /// </summary>
    public class ClientRewardPoolRanks
    {
        public string Name { get; set; }
        public decimal TotalEarned { get; set; }
        public int TotalEarnedRank { get; set; }
        public decimal TotalCashedOut { get; set; }
        //public int TotalCashedOutRank { get; set; }
    }
    /// <summary>
    /// 客户端用的 红包信息
    /// </summary>
    public class ClientRedPacketInfo
    {
        public int Index { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal FaceValue { get; set; }
        public decimal RemainingValue { get; set; }
        public int TotalCount { get; set; }
        public RedPacketType Type { get; set; }
        public RedPacketScope Scope { get; set; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public int RemainingCount { get; set; }
        public List<ClientRedPacketClaimInfo> ClaimInfoList { get; set; }

        public bool HasExpired;

        [CompleteObject]
        public void OnComplete()
        {
            HasExpired = ExpireTime < Time.Now;
        }
    }
    /// <summary>
    /// 客户端用的 红包领取记录信息
    /// </summary>
    public class ClientRedPacketClaimInfo
    {
        public int RedPacketIndex { get; set; }
        public decimal Amount { get; set; }
        public string ClaimerName { get; set; }
        public DateTime ClaimTime { get; set; }
        public CurrencyType Currency { get; set; }
    }
    public sealed class ClientNewAuction
    {
        public int Index { get; set; }
        public bool Closed { get; set; }
        public long PriceAdd { get; set; }
        public long Price { get; set; }
        public long BuyItNowPrice { get; set; }
        public long LastPrice { get; set; }
        public string OwnName { get; set; }
        public string LastName { get; set; }
        public ClientUserItem Item { get; set; }
        public bool Loading;
    }


    /// <summary>
    /// 客户端商城信息
    /// </summary>
    public class ClientAccountConsignmentInfo
    {
        /// <summary>
        /// 商城信息索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 角色名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商城销售价格
        /// </summary>
        public long Price { get; set; }
        public string GuildName { get; set; }   //行会名字
        public int GuildFlag { get; set; }  //行会旗帜
        public Color GuildFlagColor { get; set; } //行会旗帜颜色
        public string GuildRank { get; set; }   //行会等级
        public string Partner { get; set; }  //配偶
        public MirClass Class { get; set; }   //职业
        public int Level { get; set; }   //等级
        public MirGender Gender { get; set; }   //性别
        public Stats Stats { get; set; }   //状态
        public Stats HermitStats { get; set; }   //额外加点状态
        public int HermitPoints { get; set; }   //额外点数
        public bool ObserverPacket { get; set; }
        public List<ClientUserItem> Items { get; set; }  //道具
        public int Hair { get; set; }  //发型
        public Color HairColour { get; set; }   //发型颜色
        public int WearWeight { get; set; }  //穿戴负重
        public int HandWeight { get; set; }  //手负重
        public bool HideHelmet { get; set; } //隐藏头盔
        public bool HideFashion { get; set; } //隐藏时装
        public bool HideShield { get; set; } //隐藏盾牌
        /// <summary>
        /// 坐骑
        /// </summary>
        public HorseType Horse { get; set; }
        /// <summary>
        /// 忠诚度
        /// </summary>
        public decimal Myself { get; set; }
    }
}


