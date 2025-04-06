using Library;
using System;
using System.Drawing;

namespace Client.Envir
{
    [ConfigPath(@"Mir3.ini")]
    /// <summary>
    /// 客户端配置
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 登录 选人 场景分辨率
        /// </summary>
        public static readonly Size IntroSceneSize = new Size(640, 480);
        /// <summary>
        /// IP地址设置
        /// </summary>
        public const string DefaultIPAddress = "127.0.0.1";
        /// <summary>
        /// 端口设置
        /// </summary>
        public const int DefaultPort = 7000;

#if Mobile
        /// <summary>
        /// 微端IP地址
        /// </summary>
        public static string MicroClientIP = "47.104.67.238";
        /// <summary>
        /// 微端端口
        /// </summary>
        public static int MicroClientPort = 8000;

        /// <summary>
        /// FTP是否需要登录
        /// </summary>
        public static bool UseLogin = true;

        /// <summary>
        /// FTP更新地址
        /// </summary>
        public static string Host = "http://47.104.67.238:7090/";
        /// <summary>
        /// FTP更新用户名
        /// </summary>

        public static string UserName = "mobuser";
        /// <summary>
        /// FTP密码
        /// </summary>
        public static string Password = "Mob_user@321!";
#endif
        /// <summary>
        /// 使用网络设置
        /// </summary>
        public static bool UseNetworkConfig = true;

#if DEBUG
        /// <summary>
        /// 储存一区IP地址
        /// </summary>
        public static string IPAddress = "120.26.142.179";
        /// <summary>
        /// 储存一区端口
        /// </summary>
        public static int Port = 7000;
#else
        public static string IPAddress = "120.26.142.179";
        public static int Port = 7000;
#endif

        [ConfigSection("Network")]  //网络配置
        /// <summary>
        /// 客户端区服名字
        /// </summary>
        public static string ClientName { get; set; } = "Legend of Mir 3";
        /// <summary>
        /// 储存一区服名字
        /// </summary>
        public static string Server1Name { get; set; } = "";
        /// <summary>
        /// 网络超时延迟时间
        /// </summary>
        public static TimeSpan TimeOutDuration { get; set; } = TimeSpan.FromSeconds(15);

        /// <summary>
        /// 是否使用德迅云防护
        /// </summary>
        public static bool UseCloudShield { get; set; } = false;

        /// <summary>
        /// 德迅云防护IP 127开头
        /// </summary>
        public static string ShieldIP { get; set; }

        /// <summary>
        /// 德迅云防护sdk密钥。可从单实例控制面板的sdk密钥列表中获取
        /// </summary>
        public static string ShieldKey { get; set; }

        /// <summary>
        /// 道具地面颜色其他类
        /// </summary>
        [ConfigSection("Client")]
        public static Color ItemNameGroundColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色装备类
        /// </summary>
        public static Color ItemNameGroundEquipColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色材料类
        /// </summary>
        public static Color ItemNameGroundNothingColour { get; set; } = Color.FromArgb(0, 255, 50);
        /// <summary>
        /// 道具地面颜色消耗品类
        /// </summary>
        public static Color ItemNameGroundConsumableColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色普通极品类
        /// </summary>
        public static Color ItemNameGroundCommonColour { get; set; } = Color.MediumSeaGreen;
        /// <summary>
        /// 道具地面颜色高级类
        /// </summary>
        public static Color ItemNameGroundSuperiorColour { get; set; } = Color.Orange;
        /// <summary>
        /// 道具地面颜色稀世类
        /// </summary>
        public static Color ItemNameGroundEliteColour { get; set; } = Color.MediumPurple;
        /// <summary>
        /// NPC名字颜色
        /// </summary>
        public static Color NPCNameColour { get; set; } = Color.White;
        /// <summary>
        /// 怪物血条颜色
        /// </summary>
        public static Color MonHealthColour { get; set; } = Color.FromArgb(248, 33, 32);
        /// <summary>
        /// 装备等级特性标签显示
        /// </summary>
        public static bool ItemLevelLabel { get; set; } = false;
        /// <summary>
        /// 攻击速度数值显示
        /// </summary>
        public static bool AttackSpeedValue { get; set; } = false;
        /// <summary>
        /// 舒适数值显示
        /// </summary>
        public static bool ComfortValue { get; set; } = false;

        [ConfigSection("Graphics")]  //图像配置
        /// <summary>
        /// 默认窗口化 True为全屏
        /// </summary>
        public static bool FullScreen { get; set; } = false;
        /// <summary>
        /// 垂直同步
        /// </summary>
        public static bool VSync { get; set; } = false;
        /// <summary>
        /// 帧数限制
        /// </summary>
        public static bool LimitFPS { get; set; } = true;
        /// <summary>
        /// 游戏场景分辨率
        /// </summary>
        public static Size GameSize = new Size(960, 540);
        /// <summary>
        /// 缓存时间 默认30分钟
        /// </summary>
        public static TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(3);
        /// <summary>
        /// 字体设置
        /// </summary>
        public static string FontName { get; set; } = "宋体";
        /// <summary>
        /// 地图路径
        /// </summary>
        public static string MapPath { get; set; } = @"Map/";
        /// <summary>
        /// 鼠标样式
        /// </summary>
        public static bool ClipMouse { get; set; } = false;
        /// <summary>
        /// BUG顶部调试信息
        /// </summary>
        public static bool DebugLabel = false;
        /// <summary>
        /// 字体文字大小
        /// </summary>
        public static float FontSizeMod { get; set; } = 0.0F;
        /// <summary>
        /// 国际化
        /// </summary>
        public static string Language { get; set; } = "SimplifiedChinese";

        /// <summary>
        /// 窗体边框式样
        /// </summary>
        public static bool Borderless { get; set; } = false;
        /// <summary>
        /// 绘图效果
        /// </summary>
        public static bool DrawEffects { get; set; } = true;
        /// <summary>
        /// 平滑效果
        /// </summary>
        public static bool SmoothRendering { get; set; } = false;
        /// <summary>
        /// 粒子效果开关
        /// </summary>
        public static bool EnableParticle { get; set; } = true;

        [ConfigSection("Sound")]  //声音配置
        /// <summary>
        /// 声音开关
        /// </summary>
        public static bool SoundInBackground { get; set; } = true;
        /// <summary>
        /// 背景声音
        /// </summary>
        public static int SoundOverLap { get; set; } = 5;
        /// <summary>
        /// 音乐开关
        /// </summary>
        public static bool BGM { get; set; } = true;
        /// <summary>
        /// 音乐音量
        /// </summary>
        public static int BGMVolume { get; set; } = 70;
        /// <summary>
        /// 音效开关
        /// </summary>
        public static bool OtherMusic { get; set; } = true;
        /// <summary>
        /// 音效音量
        /// </summary>
        public static int OtherMusicVolume { get; set; } = 70;
        /// <summary>
        /// 系统音量
        /// </summary>
        public static int SystemVolume { get; set; } = 70;
        /// <summary>
        /// 音量音量
        /// </summary>
        public static int MusicVolume { get; set; } = 70;
        /// <summary>
        /// 玩家音量
        /// </summary>
        public static int PlayerVolume { get; set; } = 70;
        /// <summary>
        /// 怪物音量
        /// </summary>
        public static int MonsterVolume { get; set; } = 70;
        /// <summary>
        /// 技能音量
        /// </summary>
        public static int MagicVolume { get; set; } = 70;

        [ConfigSection("Login")]  //登录
        /// <summary>
        /// 是否记住登录信息
        /// </summary>
        public static bool RememberDetails { get; set; } = true;
        /// <summary>
        /// 保留登录的账号
        /// </summary>
        public static string RememberedEMail { get; set; } = "";
        /// <summary>
        /// 保留登录的密码
        /// </summary>
        public static string RememberedPassword { get; set; } = "";

        [ConfigSection("Game")]  //游戏
#if Mobile
        public static long VersionCode { get; set; } = 1;
        public static string VersionName { get; set; } = "1.0.0";
#endif
        /// <summary>
        /// 按ESC退出所有窗口
        /// </summary>
        public static bool EscapeCloseAll { get; set; } = false;
        /// <summary>
        /// 按SHIFT打开聊天窗口
        /// </summary>
        public static bool ShiftOpenChat { get; set; } = true;
        /// <summary>
        /// 特修
        /// </summary>
        public static bool SpecialRepair { get; set; } = true;

        /// <summary>
        /// 怪物信息显示全部
        /// </summary>
        public static bool MonsterBoxExpanded { get; set; } = true;
        /// <summary>
        /// 显示任务跟踪框
        /// </summary>
        public static bool QuestTrackerVisible { get; set; } = true;
        /// <summary>
        /// 组队界面显示
        /// </summary>
        //public static bool PartyListVisible { get; set; } = false;
        /// <summary>
        /// 聊天记录
        /// </summary>
        public static bool LogChat { get; set; } = true;
        /// <summary>
        /// 排行版职业选择
        /// </summary>
        public static int RankingClass { get; set; } = (int)RequiredClass.All;
        /// <summary>
        /// 排行版显示
        /// </summary>
        public static bool RankingOnline { get; set; } = true;
        /// <summary>
        /// 泰山投币次数选择
        /// </summary>
        public static int ThrowSetting { get; set; } = 0;

        [ConfigSection("Colours")]  //文字颜色配置
        /// <summary>
        /// 普通聊天
        /// </summary>
        public static Color LocalTextColour { get; set; } = Color.White;
        /// <summary>
        /// GM私聊
        /// </summary>
        public static Color GMWhisperInTextColour { get; set; } = Color.Red;
        /// <summary>
        /// 收到私聊信息
        /// </summary>
        public static Color WhisperInTextColour { get; set; } = Color.FromArgb(230, 120, 5);
        /// <summary>
        /// 发送私聊信息
        /// </summary>
        public static Color WhisperOutTextColour { get; set; } = Color.FromArgb(138, 138, 225);
        /// <summary>
        /// 组队聊天
        /// </summary>
        public static Color GroupTextColour { get; set; } = Color.Yellow;
        /// <summary>
        /// 行会聊天
        /// </summary>
        public static Color GuildTextColour { get; set; } = Color.Lime;
        /// <summary>
        /// 区域聊天
        /// </summary>
        public static Color ShoutTextColour { get; set; } = Color.Black;
        /// <summary>
        /// 世界喊话
        /// </summary>
        public static Color GlobalTextColour { get; set; } = Color.Black;
        /// <summary>
        /// 观察者聊天
        /// </summary>
        public static Color ObserverTextColour { get; set; } = Color.Silver;
        /// <summary>
        /// 提示信息
        /// </summary>
        public static Color HintTextColour { get; set; } = Color.White;
        /// <summary>
        /// 系统信息
        /// </summary>
        public static Color SystemTextColour { get; set; } = Color.Red;
        /// <summary>
        /// 战斗信息
        /// </summary>
        public static Color GainsTextColour { get; set; } = Color.White;
        /// <summary>
        /// 公告信息
        /// </summary>
        public static Color AnnouncementTextColour { get; set; } = Color.White;
        /// <summary>
        /// 中央公告信息
        /// </summary>
        public static Color NoticeTextColour { get; set; } = Color.DarkBlue;

        /// <summary>
        /// 自动登录延迟
        /// </summary>
        [ConfigSection("Other")]  //其他
        public static int AutoLoginDelay { get; set; } = 30;
        /// <summary>
        /// 是否开启快捷功能栏
        /// </summary>
        public static bool ShortcutEnabled { get; set; } = false;
        /// <summary>
        /// 4级技能经验设置
        /// </summary>
        public static int SkillExpDrop { get; set; }
        /// <summary>
        /// 平滑倍数
        /// </summary>
        //public static int SmoothRenderingRate { get; set; } = 5;

        /// <summary>
        /// 是否启用Sentry
        /// </summary>
        public static bool SentryEnabled { get; set; } = true;
#if DEBUG
        /// <summary>
        /// Sentry地址-客户端 debug
        /// </summary>
        public static string SentryClientDSN1 { get; set; } = "https://e29477df1a8b4ec78c62125f58415d26@o564963.ingest.sentry.io/5706016";
        public static string SentryClientDSN2 { get; set; } = "https://2ec0c15bb2a644f7b521e4a96f3319e3@o578604.ingest.sentry.io/5734937";
#else
        /// <summary>
        /// Sentry地址-客户端 debug
        /// </summary>
        public static string SentryClientDSN1 { get; set; } = "https://b97b7d30d02649f297d1cb1adb9de276@o564963.ingest.sentry.io/5706121";
        public static string SentryClientDSN2 { get; set; } = "https://2a6a4bb38e794851b0971dff8cded6e7@o578604.ingest.sentry.io/5734938";
#endif
    }
}
