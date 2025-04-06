using Library;
using System;
using System.Drawing;

namespace Client.Envir
{
    [ConfigPath(@".\Mir3.ini")]
    /// <summary>
    /// 客户端配置
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// 视窗设置
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
        /// 视窗大小
        /// </summary>
        public static Size GameSize { get; set; } = new Size(1024, 768);
        /// <summary>
        /// 缓存时间 默认30分钟
        /// </summary>
        public static TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
        /// <summary>
        /// 字体设置
        /// </summary>
        public static string FontName { get; set; } = "宋体";
        /// <summary>
        /// 地图路径
        /// </summary>
        public static string MapPath { get; set; } = @".\Map\";
        /// <summary>
        /// 鼠标样式
        /// </summary>
        public static bool ClipMouse { get; set; } = false;
        /// <summary>
        /// BUG顶部调试信息
        /// </summary>
        public static bool DebugLabel { get; set; } = false;
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
        public static int BGMVolume { get; set; } = 25;
        /// <summary>
        /// 音效开关
        /// </summary>
        public static bool OtherMusic { get; set; } = true;
        /// <summary>
        /// 音效音量
        /// </summary>
        public static int OtherMusicVolume { get; set; } = 25;
        /// <summary>
        /// 系统音量
        /// </summary>
        public static int SystemVolume { get; set; } = 25;
        /// <summary>
        /// 音量音量
        /// </summary>
        public static int MusicVolume { get; set; } = 25;
        /// <summary>
        /// 玩家音量
        /// </summary>
        public static int PlayerVolume { get; set; } = 25;
        /// <summary>
        /// 怪物音量
        /// </summary>
        public static int MonsterVolume { get; set; } = 25;
        /// <summary>
        /// 技能音量
        /// </summary>
        public static int MagicVolume { get; set; } = 25;

        [ConfigSection("Login")]  //登录
        /// <summary>
        /// 是否记住登录信息
        /// </summary>
        public static bool RememberDetails { get; set; } = true;
        /// <summary>
        /// 保留登录的账号
        /// </summary>
        public static string RememberedEMail { get; set; } = string.Empty;
        /// <summary>
        /// 保留登录的密码
        /// </summary>
        public static string RememberedPassword { get; set; } = string.Empty;

        [ConfigSection("Game")]  //游戏   
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
