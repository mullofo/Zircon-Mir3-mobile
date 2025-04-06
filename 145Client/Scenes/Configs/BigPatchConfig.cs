using Library;
using System.Collections.Generic;
using System.Drawing;

namespace Client.Scenes.Configs
{
    [CConfigPath(@".\Data\Saved\{0}_{1}_BigPatch.cfg")]
    public static class BigPatchConfig
    {
        /// <summary>
        /// 自动烈火
        /// </summary>
        [ConfigSection("BigPatch")]  //辅助配置
        public static bool AutoFlamingSword { get; set; } = false;
        /// <summary>
        /// 自动翔空
        /// </summary>
        public static bool AutoDragobRise { get; set; } = false;
        /// <summary>
        /// 自动莲月
        /// </summary>
        public static bool AutoBladeStorm { get; set; } = false;
        /// <summary>
        /// 自动铁布衫
        /// </summary>
        public static bool AutoDefiance { get; set; } = false;
        /// <summary>
        /// 自动破血狂杀
        /// </summary>
        public static bool AutoMight { get; set; } = false;
        /// <summary>
        /// 自动屠龙斩
        /// </summary>
        public static bool AutoMaelstromBlade { get; set; } = false;
        /// <summary>
        /// 连招
        /// </summary>
        public static bool AutoCombo { get; set; } = false;
        /// <summary>
        /// 连招类型
        /// </summary>
        public static int ComboType { get; set; } = 0;
        /// <summary>
        /// 第一招
        /// </summary>
        public static MagicType Combo1 { get; set; } = MagicType.None;
        /// <summary>
        /// 第二招
        /// </summary>
        public static MagicType Combo2 { get; set; } = MagicType.None;
        /// <summary>
        /// 第三招
        /// </summary>
        public static MagicType Combo3 { get; set; } = MagicType.None;
        /// <summary>
        /// 第四招
        /// </summary>
        public static MagicType Combo4 { get; set; } = MagicType.None;
        /// <summary>
        /// 第五招
        /// </summary>
        public static MagicType Combo5 { get; set; } = MagicType.None;

        /// <summary>
        /// 自动魔法盾
        /// </summary>
        public static bool AutoMagicShield { get; set; } = false;
        /// <summary>
        /// 自动凝血
        /// </summary>
        public static bool AutoRenounce { get; set; } = false;
        /// <summary>
        /// 自动天打雷劈
        /// </summary>
        public static bool AutoThunder { get; set; } = false;

        /// <summary>
        /// 自动换毒
        /// </summary>
        public static bool AutoPoisonDust { get; set; } = false;
        /// <summary>
        /// 自动换符
        /// </summary>
        public static bool AutoAmulet { get; set; } = false;
        /// <summary>
        /// 自动阴阳盾
        /// </summary>
        public static bool AutoCelestial { get; set; } = false;

        /// <summary>
        /// 自动四花
        /// </summary>
        public static bool AutoFourFlowers { get; set; } = false;
        /// <summary>
        /// 自动风之闪避
        /// </summary>
        public static bool AutoEvasion { get; set; } = false;
        /// <summary>
        /// 自动风之守护
        /// </summary>
        public static bool AutoRagingWind { get; set; } = false;

        /// <summary>
        /// 自动技能1时间延迟设置
        /// </summary>
        public static long NumbSkill1 { get; set; } = 10;
        /// <summary>
        /// 自动技能1
        /// </summary>
        public static MagicType AutoSkillMagic_1 { get; set; } = MagicType.None;
        /// <summary>
        /// 自动技能1开关
        /// </summary>
        public static bool AutoMagicSkill_1 { get; set; } = false;
        /// <summary>
        /// 自动技能2时间延迟设置
        /// </summary>
        public static long NumbSkill2 { get; set; } = 10;
        /// <summary>
        /// 自动技能2
        /// </summary>
        public static MagicType AutoSkillMagic_2 { get; set; } = MagicType.None;
        /// <summary>
        /// 自动技能2开关
        /// </summary>
        public static bool AutoMagicSkill_2 { get; set; } = false;
        /// <summary>
        /// BUFF时间倒计时
        /// </summary>
        public static bool ChkBufTimer { get; set; } = false;
        /// <summary>
        /// 自动添加仇敌
        /// </summary>
        public static bool ChkAutoAddEnemy { get; set; } = false;
        /// <summary>
        /// PK设置攻击模式
        /// </summary>
        public static bool ChkPkMode { get; set; } = false;
        /// <summary>
        /// PK设置喝药模式
        /// </summary>
        public static bool ChkPkDrink { get; set; } = false;

        /// <summary>
        /// 自动挂机
        /// </summary>
        public static bool AndroidPlayer { get; set; } = false;
        /// <summary>
        /// 挂机自动捡取
        /// </summary>
        public static bool AndroidPickUp { get; set; } = false;
        /// <summary>
        /// 挂机自动上毒
        /// </summary>
        public static bool AndroidPoisonDust { get; set; } = false;
        /// <summary>
        /// 挂机自动躲避
        /// </summary>
        public static bool AndroidEluded { get; set; } = false;
        /// <summary>
        /// 挂机自动回城
        /// </summary>
        public static bool AndroidBackCastle { get; set; } = false;
        /// <summary>
        /// 挂机Boss随机
        /// </summary>
        public static bool AndroidBossRandom { get; set; } = false;
        /// <summary>
        /// 挂机遇人随机
        /// </summary>
        public static bool AndroidPlayerRandom { get; set; } = false;
        /// <summary>
        /// 挂机单体技能开关
        /// </summary>
        public static bool AndroidSingleSkill { get; set; } = false;
        /// <summary>
        /// 挂机单体技能设置
        /// </summary>
        public static MagicType AndroidSkills { get; set; } = MagicType.None;//单体技能技能
        /// <summary>
        /// 挂机位置
        /// </summary>
        public static Point AndroidCoord { get; set; } = Point.Empty;
        /// <summary>
        /// 挂机范围
        /// </summary>
        public static long AndroidCoordRange { get; set; } = 10;
        /// <summary>
        /// 锁定挂机范围
        /// </summary>
        public static bool AndroidLockRange { get; set; } = false;
        /// <summary>
        /// 少于多少HP回城
        /// </summary>
        public static long AndroidBackCastleMinPHValue { get; set; } = 10;
        /// <summary>
        /// 少于多少血回城开关
        /// </summary>
        public static bool AndroidMinPHBackCastle { get; set; } = false;
        /// <summary>
        /// 少于多少HP随机
        /// </summary>
        public static long AndroidRandomMinPHValue { get; set; } = 20;
        /// <summary>
        /// 少于开始血随机开关
        /// </summary>
        public static bool AndroidMinPHRandom { get; set; } = false;
        /// <summary>
        /// 间隔时间
        /// </summary>
        public static long TimeAutoRandom { get; set; } = 5;
        /// <summary>
        /// 自动随机开关
        /// </summary>
        public static bool ChkAutoRandom { get; set; } = false;
        /// <summary>
        /// 切怪间隔时间
        /// </summary>
        public static long TargetTimeRandom { get; set; } = 10;
        /// <summary>
        /// 切换目标
        /// </summary>
        public static bool ChkChangeTarget { get; set; } = false;
        /// <summary>
        /// 自动消耗品特修时间
        /// </summary>
        public static long ConsumeRepairTime { get; set; } = 10;
        /// <summary>
        /// 自动消耗品特修开关
        /// </summary>
        public static bool ChkConsumeRepair { get; set; } = false;

        /// <summary>
        /// 免助跑
        /// </summary>
        public static bool ChkAvertVerb { get; set; } = true;
        /// <summary>
        /// 数字显血
        /// </summary>
        public static bool ChkShowHPBar { get; set; } = true;
        /// <summary>
        /// 免蜡烛
        /// </summary>
        public static bool ChkAvertBright { get; set; } = false;
        /// <summary>
        /// 显示玩家名字
        /// </summary>
        public static bool ShowPlayerNames { get; set; } = true;
        /// <summary>
        /// 综合数显
        /// </summary>
        public static bool ChkDisplayOthers { get; set; } = true;
        /// <summary>
        /// 快速小退
        /// </summary>
        public static bool ChkQuickSelect { get; set; } = false;
        /// <summary>
        /// 雷达显示
        /// </summary>
        public static bool ChkShowObjects { get; set; } = false;
        /// <summary>
        /// BOSS提醒
        /// </summary>
        public static bool ChkBossWarrning { get; set; } = true;
        /// <summary>
        /// 跑步砍
        /// </summary>
        public static bool ChkRunningHit { get; set; } = false;
        /// <summary>
        /// 跑不停
        /// </summary>
        public static bool ChkKeepRunning { get; set; } = false;
        /// <summary>
        /// 持久警告
        /// </summary>
        public static bool ChkDurableWarning { get; set; } = true;
        /// <summary>
        /// 怪物信息
        /// </summary>
        public static bool ChkMonsterInfo { get; set; } = true;
        /// <summary>
        /// 自动关组队
        /// </summary>
        public static bool ChkAutoUnAcceptGroup { get; set; } = false;
        /// <summary>
        /// 清理尸体
        /// </summary>
        public static bool ChkCleanCorpse { get; set; } = false;
        /// <summary>
        /// 怪物名字显示
        /// </summary>
        public static bool ChkMonsterNameTips { get; set; } = false;
        /// <summary>
        /// 怪物等级提示
        /// </summary>
        public static bool ChkMonsterLevelTips { get; set; } = false;
        /// <summary>
        /// 显示血量
        /// </summary>
        public static bool ShowHealth { get; set; } = true;
        /// <summary>
        /// 怪物显示血量
        /// </summary>
        public static bool ShowMonHealth { get; set; } = true;
        /// <summary>
        /// 免SHIFT
        /// </summary>
        public static bool ChkAvertShift { get; set; } = true;
        /// <summary>
        /// 攻击锁定
        /// </summary>
        public static bool ChkLockTarget { get; set; } = true;
        /// <summary>
        /// 显示数字飘血
        /// </summary>
        public static bool ShowDamageNumbers { get; set; } = true;
        /// <summary>
        /// 显示攻击目标
        /// </summary>
        public static bool ChkShowTargetInfo { get; set; } = false;
        /// <summary>
        /// 右键单击取消目标锁定
        /// </summary>
        public static bool RightClickDeTarget { get; set; } = true;
        /// <summary>
        /// 战斗退出
        /// </summary>
        public static bool ChkQuitWar { get; set; } = false;
        /// <summary>
        /// 免石化
        /// </summary>
        public static bool ChkAvertPetrifaction { get; set; } = false;
        /// <summary>
        /// 稳如泰山
        /// </summary>
        public static bool ChkAvertShake { get; set; } = true;
        /// <summary>
        /// 攻击着色
        /// </summary>
        public static bool ChkColourTarget { get; set; } = false;
        /// <summary>
        /// 队友着色
        /// </summary>
        public static bool ChkColourFriend { get; set; } = false;
        /// <summary>
        /// 放魔法时自动下马
        /// </summary>
        public static bool ChkDismountToFireMagic { get; set; } = false;
        /// <summary>
        /// 防范暗杀
        /// </summary>
        public static bool ChkDefendAssassination { get; set; } = false;
        /// <summary>
        /// 关闭经验提示
        /// </summary>
        public static bool ChkCloseExpTips { get; set; } = false;
        /// <summary>
        /// 关闭战斗信息提示
        /// </summary>
        public static bool ChkCloseCombatTips { get; set; } = false;
        /// <summary>
        /// 转生残影
        /// </summary>
        public static bool ChkShowRebirthShow { get; set; } = true;
        /// <summary>
        /// 死亡红屏
        /// </summary>
        public static bool ChkDeathRedScreen { get; set; } = true;
        /// <summary>
        /// 物品闪烁
        /// </summary>
        public static bool ChkItemObjShining { get; set; } = false;
        /// <summary>
        /// 物品极品光柱
        /// </summary>
        public static bool ChkItemObjBeam { get; set; } = true;
        /// <summary>
        /// 物品显示
        /// </summary>
        public static bool ChkItemObjShow { get; set; } = true;
        /// <summary>
        /// 自动捡取
        /// </summary>
        public static bool ChkAutoPick { get; set; } = true;
        /// <summary>
        /// TAB捡取
        /// </summary>
        public static bool ChkTabPick { get; set; } = false;
        /// <summary>
        /// 天气效果
        /// </summary>
        public static int Weather { get; set; } = 0;
        /// <summary>
        /// 自动练技能开关
        /// </summary>
        public static bool ChkAutoFire { get; set; } = false;
        /// <summary>
        /// 设置自动练技能按键
        /// </summary>
        public static int AutoFire { get; set; } = 0;
        /// <summary>
        /// 设置自动练技能间隔时间
        /// </summary>
        public static long AutoFireInterval { get; set; } = 10;
        /// <summary>
        /// 鼠标滚轮中键骑马
        /// </summary>
        public static bool ChkCallMounts { get; set; } = false;
        /// <summary>
        /// 鼠标中键放魔法
        /// </summary>
        public static bool ChkCastingMagic { get; set; } = false;
        /// <summary>
        /// 鼠标中键设置的魔法技能快捷
        /// </summary>
        public static int MiddleMouse { get; set; } = 0;

        /// <summary>
        /// 自动回复
        /// </summary>
        public static bool ChkAutoReplay { get; set; } = false;
        /// <summary>
        /// 自动喊话间隔
        /// </summary>
        public static long AutoSayInterval { get; set; } = 60000;
        /// <summary>
        /// 自动回复内容
        /// </summary>
        public static int AutoReplayItem { get; set; } = 0;
        /// <summary>
        /// 屏蔽NPC说话
        /// </summary>
        public static bool ChkShieldNpcWords { get; set; } = false;
        /// <summary>
        /// 保存喊话内容
        /// </summary>
        public static bool ChkSaveSayRecord { get; set; } = false;
        /// <summary>
        /// 屏蔽怪物说话
        /// </summary>
        public static bool ChkShieldMonsterWords { get; set; } = false;
        /// <summary>
        /// 自动说话记录信息
        /// </summary>
        public static List<string> AutoSayLines { get; set; } = new List<string>();
        /// <summary>
        /// 来消息声音提醒
        /// </summary>
        public static bool ChkMsgNotify { get; set; } = true;

        /// <summary>
        /// 魔法列表
        /// </summary>
        public static List<MagicHelper> magics { get; set; } = new List<MagicHelper>();

        /// <summary>
        /// 智能喝红
        /// </summary>
        [ConfigSection("HP")]
        public static bool HPAuto { get; set; } = false;
        /// <summary>
        /// 智能喝红默认值
        /// </summary>
        public static long HP { get; set; } = 0;
        /// <summary>
        /// 与角色HP保持同步
        /// </summary>
        public static bool HPSync { get; set; } = false;
        /// <summary>
        /// 自定义喝药HP
        /// </summary>
        public static string HP1 { get; set; } = "False|0|金创药（小）";
        public static string HP2 { get; set; } = "";
        public static string HP3 { get; set; } = "";
        public static string HP4 { get; set; } = "";
        public static string HP5 { get; set; } = "";
        public static string HP6 { get; set; } = "";
        public static string HP7 { get; set; } = "";
        public static string HP8 { get; set; } = "";

        [ConfigSection("MP")]
        /// <summary>
        /// 智能喝蓝
        /// </summary>
        public static bool MPAuto { get; set; } = false;
        /// <summary>
        /// 智能喝蓝默认值
        /// </summary>
        public static long MP { get; set; } = 0;
        /// <summary>
        /// 与角色MP保持同步
        /// </summary>
        public static bool MPSync { get; set; } = false;
        /// <summary>
        /// 自定义喝药MP
        /// </summary>
        public static string MP1 { get; set; } = "False|0|魔法药（小）";
        public static string MP2 { get; set; } = "";
        public static string MP3 { get; set; } = "";
        public static string MP4 { get; set; } = "";
        /// <summary>
        /// 随机
        /// </summary>

        [ConfigSection("Other")]
        public static string Roll1 { get; set; } = "False|0|随机传送卷";
        /// <summary>
        /// 回城
        /// </summary>
        public static string Roll2 { get; set; } = "False|0|回城卷";
        /// <summary>
        /// 小退
        /// </summary>
        public static string OffLine { get; set; } = "False|0";
        /// <summary>
        /// 治愈术
        /// </summary>
        public static string AutoHeal { get; set; } = "False|0";
        /// <summary>
        /// 最后下线生命
        /// </summary>
        public static int LastHealth { get; set; } = 0;

        public static void Init()
        {
            AutoFlamingSword = false;
            AutoDragobRise = false;
            AutoBladeStorm = false;
            AutoDefiance = false;
            AutoMaelstromBlade = false;
            AutoCombo = false;
            ComboType = 0;
            Combo1 = MagicType.None;
            Combo2 = MagicType.None;
            Combo3 = MagicType.None;
            Combo4 = MagicType.None;
            Combo5 = MagicType.None;
            AutoMagicShield = false;
            AutoRenounce = false;
            AutoThunder = false;
            AutoPoisonDust = false;
            AutoAmulet = false;
            AutoCelestial = false;
            AutoFourFlowers = false;
            AutoEvasion = false;
            AutoRagingWind = false;
            NumbSkill1 = 10;
            AutoSkillMagic_1 = MagicType.None;
            AutoMagicSkill_1 = false;
            NumbSkill2 = 10;
            AutoSkillMagic_2 = MagicType.None;
            AutoMagicSkill_2 = false;
            ChkBufTimer = false;
            ChkAutoAddEnemy = false;
            ChkPkMode = false;
            ChkPkDrink = false;
            AndroidPlayer = false;
            AndroidPickUp = false;
            AndroidPoisonDust = false;
            AndroidEluded = false;
            AndroidBackCastle = false;
            AndroidBossRandom = false;
            AndroidPlayerRandom = false;
            AndroidSingleSkill = false;
            AndroidSkills = MagicType.None;//单体技能技能
            AndroidCoord = Point.Empty;
            AndroidCoordRange = 10;
            AndroidLockRange = false;
            AndroidBackCastleMinPHValue = 10;
            AndroidMinPHBackCastle = false;
            AndroidRandomMinPHValue = 20;
            AndroidMinPHRandom = false;
            TimeAutoRandom = 5;
            ChkAutoRandom = false;
            TargetTimeRandom = 10;
            ChkChangeTarget = false;
            ConsumeRepairTime = 10;
            ChkConsumeRepair = false;
            ChkAvertVerb = true;
            ChkShowHPBar = true;
            ChkAvertBright = false;
            ShowPlayerNames = true;
            ChkDisplayOthers = true;
            ChkQuickSelect = false;
            ChkShowObjects = false;
            ChkBossWarrning = true;
            ChkRunningHit = false;
            ChkKeepRunning = false;
            ChkDurableWarning = true;
            ChkMonsterInfo = true;
            ChkAutoUnAcceptGroup = false;
            ChkCleanCorpse = false;
            ChkMonsterNameTips = false;
            ChkMonsterLevelTips = false;
            ShowHealth = true;
            ShowMonHealth = true;
            ChkAvertShift = true;
            ChkLockTarget = true;
            ShowDamageNumbers = true;
            ChkShowTargetInfo = false;
            RightClickDeTarget = true;
            ChkQuitWar = false;
            ChkAvertPetrifaction = false;
            ChkAvertShake = true;
            ChkColourTarget = false;
            ChkColourFriend = false;
            ChkDismountToFireMagic = false;
            ChkDefendAssassination = false;
            ChkCloseExpTips = false;
            ChkCloseCombatTips = false;
            ChkShowRebirthShow = true;
            ChkDeathRedScreen = true;
            ChkItemObjShining = false;
            ChkItemObjBeam = true;
            ChkItemObjShow = true;
            ChkAutoPick = true;
            ChkTabPick = false;
            Weather = 0;
            ChkAutoFire = false;
            AutoFire = 0;
            AutoFireInterval = 10;
            ChkCallMounts = false;
            ChkCastingMagic = false;
            MiddleMouse = 0;
            ChkAutoReplay = false;
            AutoSayInterval = 60000;
            AutoReplayItem = 0;
            ChkShieldNpcWords = false;
            ChkSaveSayRecord = false;
            ChkShieldMonsterWords = false;
            AutoSayLines = new List<string>();
            ChkMsgNotify = true;
            magics = new List<MagicHelper>();
            HPAuto = false;
            HP = 0;
            HPSync = false;
            HP1 = "False|0|金创药（小）";
            HP2 = "";
            HP3 = "";
            HP4 = "";
            HP5 = "";
            HP6 = "";
            HP7 = "";
            HP8 = "";
            MPAuto = false;
            MP = 0;
            MPSync = false;
            MP1 = "False|0|魔法药（小）";
            MP2 = "";
            MP3 = "";
            MP4 = "";
            Roll1 = "False|0|随机传送卷";
            Roll2 = "False|0|回城卷";
            OffLine = "False|0";
            AutoHeal = "False|0";
            LastHealth = 0;
        }
    }
}
