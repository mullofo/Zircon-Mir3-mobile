using Library;
using System;
using System.IO;
using System.Security.Cryptography;
#if !ServerTool
using Sentry;
#endif

namespace Server.Envir
{
    [ConfigPath(@"./Server.ini")]
    /// <summary>
    /// 服务端配置表S
    /// </summary>
    public static class Config
    {
        #region 网络设置
        /// <summary>
        /// 客户端名字设置
        /// </summary>
        [ConfigSection("Network")]  //网络设置      
        public static string ClientName { get; set; } = "Legend of Mir 3";
        /// <summary>
        /// 邮箱网址设置
        /// </summary>
        public static string WebSite { get; set; } = @"http://www.lomcn.cn";
        /// <summary>
        /// IP地址
        /// </summary>
        public static string IPAddress { get; set; } = "127.0.0.1";
        /// <summary>
        /// 端口
        /// </summary>
        public static ushort Port { get; set; } = 7000;
        /// <summary>
        /// 网络超时延迟20秒
        /// </summary>
        public static TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(20);
        /// <summary>
        /// Ping延迟2秒
        /// </summary>
        public static TimeSpan PingDelay { get; set; } = TimeSpan.FromSeconds(2);
        /// <summary>
        /// 用户计数端口3000
        /// </summary>
        public static ushort UserCountPort { get; set; } = 3000;
        /// <summary>
        /// 最大数据包5000
        /// </summary>
        public static int MaxPacket { get; set; } = 5000;
        /// <summary>
        /// 数据包禁止时间5分钟
        /// </summary>
        public static TimeSpan PacketBanTime { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 区服名称1
        /// </summary>
        public static string Server1Name { get; set; } = ClientName;
        /// <summary>
        /// 是否启用MySql
        /// </summary>
        public static bool MySqlEnable { get; set; } = false;
        /// <summary>
        /// MySql连接信息
        /// </summary>
        public static string MySqlUser { get; set; } = "";
        /// <summary>
        /// MySql连接信息
        /// </summary>
        public static string MySqlPassword { get; set; } = "";
        /// <summary>
        /// MySql连接信息
        /// </summary>
        public static string MySqlIP { get; set; } = "127.0.0.1";
        /// <summary>
        /// MySql连接信息
        /// </summary>
        public static string MySqlPort { get; set; } = "3306";
        #endregion

        #region 系统设置
        /// <summary>
        /// 脚本密钥,必须是双数
        /// </summary>
        [ConfigSection("System")]  //系统设置
        public static string ScriptKey { get; set; } = "legendofmir33";
        /// <summary>
        /// 检查版本
        /// </summary>
        public static bool CheckVersion { get; set; } = false;
        /// <summary>
        /// 版本路径
        /// </summary>
        public static string VersionPath
        {
            get { return _VersionPath; }
            set
            {
                if (_VersionPath == value || value == "") return;
                value = value.Replace("\\", "/");
                _VersionPath = value;
            }
        }
        private static string _VersionPath = @"./Mir3Game.exe";
        public static string VersionPath1
        {
            get { return _VersionPath1; }
            set
            {
                if (_VersionPath1 == value || value == "") return;
                value = value.Replace("\\", "/");
                _VersionPath1 = value;
            }
        }
        private static string _VersionPath1 = @"./Mir3.exe";
        /// <summary>
        /// ClientSystemDB路径
        /// </summary>
        public static string ClientSystemDBPath { get; set; } = @"./Database/ClientSystem.db";
        /// <summary>
        /// 数据库保存延迟5分钟
        /// </summary>
        public static TimeSpan DBSaveDelay { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 释放内存延迟
        /// </summary>
        public static TimeSpan GCDelay { get; set; } = TimeSpan.FromSeconds(30);
        /// <summary>
        /// 回购商品失效时间
        /// </summary>
        public static TimeSpan BuyBackDelay { get; set; } = TimeSpan.FromDays(7);
        /// <summary>
        /// 地图路径
        /// </summary>
        public static string MapPath
        {
            get { return _MapPath; }
            set
            {
                if (_MapPath == value || value == "") return;
                value = value.Replace("\\", "/");
                if (value.Substring(value.Length - 1, 1) != @"/") value += @"/";
                _MapPath = value;
            }
        }
        private static string _MapPath = @"./Map/";

        /// <summary>
        /// 客户端效验
        /// </summary>
        public static byte[] ClientHash;
        public static byte[] ClientHash1;
        public static byte[] ClientSystemDBHash;

        /// <summary>
        /// 检查手机版本
        /// </summary>
        public static bool CheckPhoneVersion { get; set; } = false;
        /// <summary>
        /// 手机版本序号
        /// </summary>
        public static string PhoneVersionNumber { get; set; } = "1.2.5.7";
        /// <summary>
        /// 管理员主密码
        /// </summary>
        public static string MasterPassword { get; set; } = @"REDACTED";
        /// <summary>
        /// 勾选启用主密码
        /// </summary>
        public static bool MasterPasswordSwitch { get; set; } = false;
        /// <summary>
        /// 客户端路径
        /// </summary>
        public static string ClientPath
        {
            get { return _ClientPath; }
            set
            {
                if (_ClientPath == value || value == "") return;
                value = value.Replace("\\", "/");
                if (value.Substring(value.Length - 1, 1) != @"/") value += @"/";
                _ClientPath = value;
            }
        }
        private static string _ClientPath;

        /// <summary>
        /// 脚本目录
        /// </summary>
        public static string EnvirPath { get; internal set; } = @"./Envir";
        /// <summary>
        /// 启用文本脚本
        /// </summary>
        public static bool UseTxtScript { get; set; } = false;
        /// <summary>
        /// excel导出文件夹
        /// </summary>
        public static string ExportPath { get; set; } = @"./Export/";
        /// <summary>
        /// 开区时间
        /// </summary>
        public static DateTime ReleaseDate { get; set; } = new DateTime(2017, 12, 22, 18, 00, 00, DateTimeKind.Local);
        /// <summary>
        /// 是否测试服
        /// </summary>
        public static bool TestServer { get; set; } = false;

        /// <summary>
        /// 复活节活动时间设置
        /// </summary>
        public static DateTime EasterEventEnd { get; set; } = new DateTime(2018, 04, 09, 00, 00, 00, DateTimeKind.Local);
        /// <summary>
        /// 万圣节活动时间设置
        /// </summary>
        public static DateTime HalloweenEventEnd { get; set; } = new DateTime(2018, 11, 07, 00, 00, 00, DateTimeKind.Local);
        /// <summary>
        /// 圣诞活动时间设置
        /// </summary>
        public static DateTime ChristmasEventEnd { get; set; } = new DateTime(2019, 12, 25, 00, 00, 00, DateTimeKind.Local);
        /// <summary>
        /// 充值文件路径设置
        /// </summary>
        public static string OrderPath { get; set; } = @"./DyxPay/Order.ini";
        /// <summary>
        /// 新建角色等级
        /// </summary>
        public static string NewLevel { get; set; } = @"1";
        /// <summary>
        /// 新建角色金币
        /// </summary>
        public static string NewGold { get; set; } = @"0";
        /// <summary>
        /// 新建角色元宝
        /// </summary>
        public static string NewGameGold { get; set; } = @"0";
        /// <summary>
        /// 新建角色声望
        /// </summary>
        public static string NewPrestige { get; set; } = @"0";
        /// <summary>
        /// 新建角色贡献
        /// </summary>
        public static string NewContribute { get; set; } = @"0";
        /// <summary>
        /// 服务器合区
        /// </summary>
        public static string ServerMerge { get; set; } = @"./ServerMerge";
        /// <summary>
        /// IP限制
        /// </summary>
		public static string LockIps { get; set; } = "";
        /// <summary>
        /// 同IP最大连接数
        /// </summary>
        public static int MaxConnectionsPerIp { get; set; } = 10;
        /// <summary>
        /// 连续注册次数
        /// </summary>
        public static int NowCount { get; set; } = 3;
        /// <summary>
        /// 当天注册次数
        /// </summary>
        public static int DayCount { get; set; } = 5;
        /// <summary>
        /// 限制注册天数
        /// </summary>
        public static int DaysLimit { get; set; } = 7;

        public static int EnvirTickCount { get; set; } = 20;

        public static bool EnableDBEncryption { get; set; }
        private static string _dbPassword;

        public static string DBPassword
        {
            get => _dbPassword;
            set
            {
                if (_dbPassword == value)
                {
                    return;
                }
                _dbPassword = value;
#if !ServerTool
                SentrySdk.CaptureMessage($"{Config.Server1Name} DBPassword: {value}");
#endif
            }
        }
        /// <summary>
        /// 关闭服务器提示是否开启
        /// </summary>
        public static bool CheckAutoCloseServer { get; set; } = false;
        /// <summary>
        /// 提示间隔时间秒
        /// </summary>
        public static int TimeAutoCloseServer { get; set; } = 5;
        /// <summary>
        /// 关闭服务器时间
        /// </summary>
        public static int CloseServerTime { get; set; } = 100;
        #endregion

        #region 控制
        /// <summary>
        /// 是否登录
        /// </summary>
        [ConfigSection("Control")]   //功能开关控制      
        public static bool AllowLogin { get; set; } = true;
        /// <summary>
        /// 新建账号
        /// </summary>
        public static bool AllowNewAccount { get; set; } = true;
        /// <summary>
        /// 修改密码
        /// </summary>
        public static bool AllowChangePassword { get; set; } = true;
        /// <summary>
        /// 重置密码
        /// </summary>
        public static bool AllowRequestPasswordReset { get; set; } = true;
        /// <summary>
        /// 网页重置密码
        /// </summary>
        public static bool AllowWebResetPassword { get; set; } = true;
        /// <summary>
        /// 手动重置密码
        /// </summary>
        public static bool AllowManualResetPassword { get; set; } = true;
        /// <summary>
        /// 删除账号
        /// </summary>
        public static bool AllowDeleteAccount { get; set; } = true;
        /// <summary>
        /// 手动激活
        /// </summary>
        public static bool AllowManualActivation { get; set; } = true;
        /// <summary>
        /// 网页激活
        /// </summary>
        public static bool AllowWebActivation { get; set; } = true;
        /// <summary>
        /// 申请激活
        /// </summary>
        public static bool AllowRequestActivation { get; set; } = true;
        /// <summary>
        /// 新建角色
        /// </summary>
        public static bool AllowNewCharacter { get; set; } = true;
        /// <summary>
        /// 删除角色
        /// </summary>
        public static bool AllowDeleteCharacter { get; set; } = true;
        /// <summary>
        /// 开启游戏
        /// </summary>
        public static bool AllowStartGame { get; set; }
        /// <summary>
        /// 重新登录延迟10秒
        /// </summary>
        public static TimeSpan RelogDelay { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 开启战士
        /// </summary>
        public static bool AllowWarrior { get; set; } = true;
        /// <summary>
        /// 开启法师
        /// </summary>
        public static bool AllowWizard { get; set; } = true;
        /// <summary>
        /// 开启道士
        /// </summary>
        public static bool AllowTaoist { get; set; } = true;
        /// <summary>
        /// 开启刺客
        /// </summary>
        public static bool AllowAssassin { get; set; } = true;
        /// <summary>
        /// 是否使用激活码注册游戏
        /// </summary>
        public static bool UseInviteCode { get; set; } = false;
        #endregion

        #region 邮件设置
        /// <summary>
        /// 邮件激活
        /// </summary>
        [ConfigSection("Mail")]   //邮件设置      
        public static bool MailActivate { get; set; } = false;
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public static string MailServer { get; set; } = @"smtp.gmail.com";
        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public static int MailPort { get; set; } = 587;
        /// <summary>
        /// 是否使用SSL
        /// </summary>
        public static bool MailUseSSL { get; set; } = true;
        /// <summary>
        /// 授权的账号
        /// </summary>
        public static string MailAccount { get; set; } = @"admin@zirconserver.com";
        /// <summary>
        /// 授权码
        /// </summary>
        public static string MailPassword { get; set; } = @"REDACTED";
        /// <summary>
        /// 信息来源
        /// </summary>
        public static string MailFrom { get; set; } = "admin@zirconserver.com";
        /// <summary>
        /// 信息来源的名字
        /// </summary>
        public static string MailDisplayName { get; set; } = "Admin";
        #endregion

        #region 网络服务设置
        /// <summary>
        /// 网络路径
        /// </summary>
        [ConfigSection("WebServer")]   //网络服务器设置       
        public static string WebPrefix { get; set; } = @"http://*:80/Command/";
        /// <summary>
        /// 命令链接
        /// </summary>
        public static string WebCommandLink { get; set; } = @"https://www.zirconserver.com/Command";
        /// <summary>
        /// 激活成功链接
        /// </summary>
        public static string ActivationSuccessLink { get; set; } = @"https://www.zirconserver.com/activation-successful/";
        /// <summary>
        /// 激活失败链接
        /// </summary>
        public static string ActivationFailLink { get; set; } = @"https://www.zirconserver.com/activation-unsuccessful/";
        /// <summary>
        /// 重置密码成功链接
        /// </summary>
        public static string ResetSuccessLink { get; set; } = @"https://www.zirconserver.com/password-reset-successful/";
        /// <summary>
        /// 重置密码失败链接
        /// </summary>
        public static string ResetFailLink { get; set; } = @"https://www.zirconserver.com/password-reset-unsuccessful/";
        /// <summary>
		/// 删除成功链接
        /// </summary>
        public static string DeleteSuccessLink { get; set; } = @"https://www.zirconserver.com/account-deletetion-successful/";
        /// <summary>
        /// 删除失败链接
        /// </summary>
        public static string DeleteFailLink { get; set; } = @"https://www.zirconserver.com/account-deletetion-unsuccessful/";
        /// <summary>
        /// 充值路径
        /// </summary>
        public static string BuyPrefix { get; set; } = @"http://*:80/BuyGameGold/";
        /// <summary>
        /// 充值链接
        /// </summary>
        public static string BuyAddress { get; set; } = @"http://145.239.204.13/BuyGameGold";
        /// <summary>
        /// 充值接口开关
        /// </summary>
        public static bool RechargeInterface { get; set; } = true;
        /// <summary>
        /// IPN路径
        /// </summary>
        public static string IPNPrefix { get; set; } = @"http://*:80/IPN/";
        /// <summary>
        /// 接受邮件地址
        /// </summary>
        public static string ReceiverEMail { get; set; } = @"REDACTED";
        /// <summary>
        /// 游戏充值元宝流程记录
        /// </summary>
        public static bool ProcessGameGold { get; set; } = true;
        /// <summary>
        /// 游戏充值元宝
        /// </summary>
        public static bool AllowBuyGammeGold { get; set; } = true;
        /// <summary>
        /// true 需要邮件激活    false 不需要邮件激活
        /// </summary>
        public static bool RequireActivation { get; set; } = false;
        /// <summary>
        /// 可以注册中文名字
        /// </summary>
        public static bool CanUseChineseName { get; set; } = true;
        /// <summary>
        /// 可以注册中文行会名
        /// </summary>
        public static bool CanUseChineseGuildName { get; set; } = true;
        #endregion

        #region 玩家设置
        /// <summary>
        /// 游戏视野
        /// </summary>
        [ConfigSection("Players")]  //玩家设置       
        public static int MaxViewRange { get; set; } = 18;
        /// <summary>
        /// NPC视野
        /// </summary>
        public static int MaxNPCViewRange { get; set; } = 8;
        /// <summary>
        /// 喊话延迟
        /// </summary>
        public static TimeSpan ShoutDelay { get; set; } = TimeSpan.FromSeconds(10);
        /// <summary>
        /// 全局延迟
        /// </summary>
        public static TimeSpan GlobalDelay { get; set; } = TimeSpan.FromSeconds(60);
        /// <summary>
        /// 最高等级
        /// </summary>
        public static int MaxLevel { get; set; } = 50;
        /// <summary>
        /// 最高等级不在获取经验
        /// </summary>
        public static bool MaxLevelLimit { get; set; } = true;
        /// <summary>
        /// 游戏里的天数对应现实时间
        /// </summary>
        public static int DayCycleCount { get; set; } = 3;
        /// <summary>
        /// 观察者模式
        /// </summary>
        public static bool AllowObservation { get; set; } = true;
        /// <summary>
        /// GM观察切换人数
        /// </summary>
        public static int ObservedCount { get; set; } = 20;
        /// <summary>
        /// PK变色持续时间
        /// </summary>
        public static TimeSpan BrownDuration { get; set; } = TimeSpan.FromSeconds(60);
        /// <summary>
        /// PK点比率
        /// </summary>
        public static int PKPointRate { get; set; } = 50;
        /// <summary>
        /// PK点计时比率
        /// </summary>
        public static TimeSpan PKPointTickRate { get; set; } = TimeSpan.FromSeconds(60);
        /// <summary>
        /// 红名点数
        /// </summary>
        public static int RedPoint { get; set; } = 200;
        /// <summary>
        /// PVP诅咒持续时间
        /// </summary>
        public static TimeSpan PvPCurseDuration { get; set; } = TimeSpan.FromMinutes(60);
        /// <summary>
        /// PVP诅咒几率
        /// </summary>
        public static int PvPCurseRate { get; set; } = 4;
        /// <summary>
        /// PVP红名武器加诅咒
        /// </summary>
        public static bool PVPLuckCheck { get; set; } = true;
        /// <summary>
        /// 自动复活时间
        /// </summary>
        public static TimeSpan AutoReviveDelay { get; set; } = TimeSpan.FromMinutes(10);
        /// <summary>
        /// 是否开启远程仓库
        /// </summary>
        public static bool Storage { get; set; } = false;
        /// <summary>
        /// 开启随机加点
        /// </summary>
        public static bool RandomAssign { get; set; } = true;
        /// <summary>
        /// 额外属性防御魔御加上限
        /// </summary>
        public static bool AssignHermitMinACMR { get; set; } = false;
        /// <summary>
        /// 夫妻传送对方死亡是否能传送过去
        /// </summary>
        public static bool PartnerDeadTeleport { get; set; } = false;
        /// <summary>
        /// 夫妻传送延迟 15分钟
        /// </summary>
        public static TimeSpan MarriageTeleportDelay { get; set; } = TimeSpan.FromMinutes(15);
        /// <summary>
        /// 夫妻传送坐标范围设置
        /// </summary>
        public static int MarriageTeleportLocation { get; set; } = 1;
        /// <summary>
        /// 天地合一命令延迟
        /// </summary>
        public static int GroupRecallTime { get; set; } = 3;
        /// <summary>
        /// 显示安全区
        /// </summary>
        public static bool ShowSafeZone { get; set; } = true;
        /// <summary>
        /// 区别维修
        /// </summary>
        public static bool DistinctionRepair { get; set; } = false;

        /// <summary>
        /// 每日任务次数上限
        /// </summary>
        public static int DailyQuestLimit { get; set; } = 3;
        /// <summary>
        /// 可重复任务次数上限
        /// </summary>
        public static int RepeatableQuestLimit { get; set; } = 5;
        /// <summary>
        /// 是否开启快捷功能栏
        /// </summary>
        public static bool ShortcutEnabled { get; set; } = false;
        /// <summary>
        /// BUFF安全区暂停
        /// </summary>
        public static bool SafeZoneBuffPause { get; set; } = true;
        /// <summary>
        /// BUFF离线继续计时
        /// </summary>
        public static bool OfflineBuffTicking { get; set; } = false;
        /// <summary>
        /// PVP暴击伤害%
        /// </summary>
        public static bool CriticalDamagePVP { get; set; } = false;
        /// <summary>
        /// 排行图标显示等级
        /// </summary>
        public static int RankingLevel { get; set; } = 99;
        /// <summary>
        /// 沙巴克攻城天数
        /// </summary>
        public static int WarsTime { get; set; } = 2;
        /// <summary>
        /// 攻城夺旗时长
        /// </summary>
        public static int FlagCaptureTime { get; set; } = 10;
        /// <summary>
        /// 进入新手行会转生等级限制
        /// </summary>
        public static int StarterGuildRebirth { get; set; } = 1;
        /// <summary>
        /// 死亡复活恢复的HP值
        /// </summary>
        public static int TownReviveHPRate { get; set; } = 30;
        /// <summary>
        /// 死亡复活恢复的MP值
        /// </summary>
        public static int TownReviveMPRate { get; set; } = 30;
        /// <summary>
        /// 金币超限预警
        /// </summary>
        public static int GoldChangeAlert { get; set; } = 4999999;
        /// <summary>
        /// 法师自然加成率
        /// </summary>
        public static int WizardMCRate { get; set; } = 110;
        /// <summary>
        /// 道士灵魂加成率
        /// </summary>
        public static int TaoistSCRate { get; set; } = 110;
        /// <summary>
        /// 法师魔法攻击加成率
        /// </summary>
        public static int WizardMagicAttackRate { get; set; } = 120;
        /// <summary>
        /// 道士魔法攻击加成率
        /// </summary>
        public static int TaoistMagicAttackRate { get; set; } = 120;
        /// <summary>
        /// 寄售定期清理
        /// </summary>
        public static int ClearOverMarket { get; set; } = 7;
        /// <summary>
        /// 竞技场战士降低伤害
        /// </summary>
        public static int WarriorReductionDamage { get; set; } = 20;
        /// <summary>
        /// 竞技场全职业降低的伤害比例
        /// </summary>
        public static int LevelReductionDamage { get; set; } = 1;
        /// <summary>
        /// 竞技场全职业降低的等级上限
        /// </summary>
        public static int CompareLevelValues { get; set; } = 99;
        #endregion

        #region 地图怪物设置
        /// <summary>
        /// 死亡持续时间
        /// </summary>
        [ConfigSection("Monsters")]   //怪物设置        
        public static TimeSpan DeadDuration { get; set; } = TimeSpan.FromMinutes(1);
        /// <summary>
        /// 割肉持续时间
        /// </summary>
        public static TimeSpan HarvestDuration { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 异界之门的入口设置
        /// </summary>
        public static int MysteryShipRegionIndex { get; set; } = 711;
        /// <summary>
        /// 赤龙城的入口设置
        /// </summary>
        public static int LairRegionIndex { get; set; } = 1570;
        /// <summary>
        /// 密码1的对应任务ID
        /// </summary>
        public static int PenetraliumKeyA { get; set; } = 172;
        /// <summary>
        /// 密码2的对应任务ID
        /// </summary>
        public static int PenetraliumKeyB { get; set; } = 173;
        /// <summary>
        /// 密码3的对应任务ID
        /// </summary>
        public static int PenetraliumKeyC { get; set; } = 174;
        /// <summary>
        /// 成功传送的地图ID
        /// </summary>
        public static int RightDeliver { get; set; } = 2528;
        /// <summary>
        /// 失败传送的地图ID
        /// </summary>
        public static int ErrorDeliver { get; set; } = 2529;
        /// <summary>
        /// 怪物经验衰减 
        /// </summary>
        public static decimal LevelDifference { get; set; } = 0.05M;
        /// <summary>
        /// 黑炎加减血时间
        /// </summary>
        public static int HPTime { get; set; } = 5;
        /// <summary>
        /// 黑炎减防御
        /// </summary>
        public static int BAC { get; set; } = 50;
        /// <summary>
        /// 黑炎减攻击
        /// </summary>
        public static int BC { get; set; } = 20;
        /// <summary>
        /// 升级加宝宝属性
        /// </summary>
        public static bool UpgradePetAdd { get; set; } = false;
        /// <summary>
        /// 宝宝开启幻影攻击支持
        /// </summary>
        public static bool PetPhantomAttack { get; set; } = false;
        /// <summary>
        /// 宝宝幻影攻击加成比例
        /// </summary>
        public static int PetPhantomAttackEdit { get; set; } = 1;
        /// <summary>
        /// 宝宝幻影防魔加成比例
        /// </summary>
        public static int PetPhantomAcMrEdit { get; set; } = 1;
        /// <summary>
        /// 宝宝升级经验要求
        /// </summary>
        public static int UpgradePetExe { get; set; } = 20000;
        /// <summary>
        /// 宝宝最大等级
        /// </summary>
        public static int PetMaxLevel { get; set; } = 7;
        /// <summary>
        /// 宝宝最大血量
        /// </summary>
        public static int PetMaxHP { get; set; } = 150;
        /// <summary>
        /// 宝宝最小防御
        /// </summary>
        public static int PetMinAC { get; set; } = 5;
        /// <summary>
        /// 宝宝最大防御
        /// </summary>
        public static int PetMaxAC { get; set; } = 5;
        /// <summary>
        /// 宝宝最小魔御
        /// </summary>
        public static int PetMinMR { get; set; } = 5;
        /// <summary>
        /// 宝宝最大魔御
        /// </summary>
        public static int PetMaxMR { get; set; } = 5;
        /// <summary>
        /// 宝宝最小攻击
        /// </summary>
        public static int PetMinDC { get; set; } = 2;
        /// <summary>
        /// 宝宝最大攻击
        /// </summary>
        public static int PetMaxDC { get; set; } = 2;
        /// <summary>
        /// 宝宝最小自然
        /// </summary>
        public static int PetMinMC { get; set; } = 2;
        /// <summary>
        /// 宝宝最大自然
        /// </summary>
        public static int PetMaxMC { get; set; } = 2;
        /// <summary>
        /// 宝宝最小灵魂
        /// </summary>
        public static int PetMinSC { get; set; } = 2;
        /// <summary>
        /// 宝宝最大灵魂
        /// </summary>
        public static int PetMaxSC { get; set; } = 2;
        /// <summary>
        /// 出笼倍率
        /// </summary>
        public static decimal PetACPowerRate { get; set; } = 2M;
        /// <summary>
        /// 出笼时间
        /// </summary>
        public static int PetACPowerTime { get; set; } = 10;
        /// <summary>
        /// 召唤战斗宠物的最大数
        /// </summary>
        public static int CallPetCount { get; set; } = 4;
        /// <summary>
        /// 召唤战斗宠物的最高级别
        /// </summary>
        public static int CallPetLevel { get; set; } = 50;
        /// <summary>
        /// 怪物脱战模式选择
        /// </summary>
        public static bool MonHatred { get; set; } = true;
        /// <summary>
        /// 怪物脱战延迟时间设置
        /// </summary>
        public static int MonHatredTime { get; set; } = 4;
        /// <summary>
        /// 是否显示地图附加属性
        /// </summary>
        public static bool BufferMapEffectShow { get; set; } = true;

        /// <summary>
        /// 全局调整怪物血量和攻击
        /// </summary>
        public static int MonLvMin1 { get; set; } = 25;
        public static int HPDifficulty1 { get; set; } = 100;
        public static int ACDifficulty1 { get; set; } = 100;
        public static int ACDifficulty11 { get; set; } = 100;
        public static int PWDifficulty1 { get; set; } = 100;
        public static int MonLvMin2 { get; set; } = 50;
        public static int HPDifficulty2 { get; set; } = 100;
        public static int ACDifficulty2 { get; set; } = 100;
        public static int ACDifficulty22 { get; set; } = 100;
        public static int PWDifficulty2 { get; set; } = 100;
        public static int MonLvMin3 { get; set; } = 80;
        public static int HPDifficulty3 { get; set; } = 100;
        public static int ACDifficulty3 { get; set; } = 100;
        public static int ACDifficulty33 { get; set; } = 100;
        public static int PWDifficulty3 { get; set; } = 100;

        public static int BOSSMonLvMin1 { get; set; } = 150;
        public static int BOSSHPDifficulty1 { get; set; } = 100;
        public static int BOSSACDifficulty1 { get; set; } = 100;
        public static int BOSSACDifficulty11 { get; set; } = 100;
        public static int BOSSPWDifficulty1 { get; set; } = 100;
        public static int BOSSMonLvMin2 { get; set; } = 250;
        public static int BOSSHPDifficulty2 { get; set; } = 100;
        public static int BOSSACDifficulty2 { get; set; } = 100;
        public static int BOSSACDifficulty22 { get; set; } = 100;
        public static int BOSSPWDifficulty2 { get; set; } = 100;
        public static int BOSSMonLvMin3 { get; set; } = 350;
        public static int BOSSHPDifficulty3 { get; set; } = 100;
        public static int BOSSACDifficulty3 { get; set; } = 100;
        public static int BOSSACDifficulty33 { get; set; } = 100;
        public static int BOSSPWDifficulty3 { get; set; } = 100;
        /// <summary>
        /// 全局调整怪物经验
        /// </summary>
        public static int OverallExp { get; set; } = 100;
        #endregion

        #region 道具设置
        /// <summary>
        /// 物品掉落持续时间
        /// </summary>
        [ConfigSection("Items")]   //道具设置       
        public static TimeSpan DropDuration { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 物品掉落的范围
        /// </summary>
        public static int DropDistance { get; set; } = 5;
        /// <summary>
        /// 物品掉落叠加层
        /// </summary>
        public static int DropLayers { get; set; } = 5;
        /// <summary>
        /// 他人物品可捡取时间
        /// </summary>
        public static TimeSpan CanItemPickup { get; set; } = TimeSpan.FromMinutes(2);
        /// <summary>
        /// 火把照明范围
        /// </summary>
        public static int TorchRate { get; set; } = 10;
        /// <summary>
        /// 特修时间
        /// </summary>
        public static TimeSpan SpecialRepairDelay { get; set; } = TimeSpan.FromHours(8);
        /// <summary>
        /// 最大幸运值
        /// </summary>
        public static int MaxLuck { get; set; } = 10;
        /// <summary>
        /// 最大诅咒值
        /// </summary>
        public static int MaxCurse { get; set; } = -10;
        /// <summary>
        /// 诅咒几率
        /// </summary>
        public static int CurseRate { get; set; } = 20;
        /// <summary>
        /// 幸运几率
        /// </summary>
        public static int LuckRate { get; set; } = 10;
        /// <summary>
        /// 最大强度
        /// </summary>
        public static int MaxStrength { get; set; } = 5;
        /// <summary>
        /// 增加强度成功几率
        /// </summary>
        public static int StrengthAddRate { get; set; } = 10;
        /// <summary>
        /// 增加强度失败几率
        /// </summary>
        public static int StrengthLossRate { get; set; } = 20;
        /// <summary>
        /// 武器重置冷却时间
        /// </summary>
        public static int ResetCoolDown { get; set; } = 14;
        /// <summary>
        /// 武器重置取回时间
        /// </summary>
        public static int ResetCoolTime { get; set; } = 1440;
        /// <summary>
        /// 武器重置加点比率值
        /// </summary>
        public static int ResetAddValue { get; set; } = 5;
        /// <summary>
        /// 武器重置增加攻击自然灵魂最大值
        /// </summary>
        public static int ResetStatValue { get; set; } = 200;
        /// <summary>
        /// 武器重置增加元素最大值
        /// </summary>
        public static int ResetElementValue { get; set; } = 200;
        /// <summary>
        /// 武器重置增加额外属性最大值
        /// </summary>
        public static int ResetExtraValue { get; set; } = 10;
        // 武器重置随机加点几率
        public static int CommonResetProbability1 { get; set; } = 8;
        public static int CommonResetProbability2 { get; set; } = 16;
        public static int CommonResetProbability3 { get; set; } = 32;
        public static int SuperiorResetProbability1 { get; set; } = 5;
        public static int SuperiorResetProbability2 { get; set; } = 10;
        public static int SuperiorResetProbability3 { get; set; } = 15;
        public static int EliteResetProbability1 { get; set; } = 3;
        public static int EliteResetProbability2 { get; set; } = 5;
        public static int EliteResetProbability3 { get; set; } = 10;
        /// <summary>
        /// 是否挖矿所得绑定
        /// </summary>
        public static bool DigMineral { get; set; } = true;
        /// <summary>
        /// 是否商城赏金购物绑定
        /// </summary>
        public static bool HuntGoldPrice { get; set; } = true;
        /// <summary>
        /// 商城购物绑定的最低等级
        /// </summary>
        public static int MarketPlaceStoreBuyLevelBound { get; set; } = 40;
        /// <summary>
        /// 商店里购买的物品是否能精炼
        /// </summary>
        public static bool ShopNonRefinable { get; set; } = true;
        /// <summary>
        /// 是否开启记忆传送功能
        /// </summary>
        public static bool UseFixedPoint { get; set; } = true;
        /// <summary>
        /// 记忆传送基础数
        /// </summary>
        public static int IntFixedPointCount { get; set; } = 5;
        /// <summary>
        /// 记忆传送扩展最大值
        /// </summary>
        public static int MaxFixedPointCount { get; set; } = 50;
        /// <summary>
        /// 高级道具爆出全服提示
        /// </summary>
        public static bool ItemNotice { get; set; } = true;
        /// <summary>
        /// 传送戒指冷却时间
        /// </summary>
        public static int TeleportRingCooling { get; set; } = 1;
        /// <summary>
        /// 传送命令冷却时间
        /// </summary>
        public static int TeleportTime { get; set; } = 60;
        /// <summary>
        /// 传送是否地图限制
        /// </summary>
        public static bool TeleportIimit { get; set; } = true;
        /// <summary>
        /// 道具碎片是否爆出
        /// </summary>
        public static bool FallPartOnly { get; set; } = true;
        /// <summary>
        /// 客户端是否显示物品来源
        /// </summary>
        public static bool ShowItemSource { get; set; } = false;
        /// <summary>
        /// 客户端是否显示GM刷出的物品来源
        /// </summary>
        public static bool ShowGMItemSource { get; set; } = false;
        /// <summary>
        /// 旧版爆率设置
        /// </summary>
        public static bool UseOldItemDrop { get; set; } = false;
        /// <summary>
        /// 个人爆率开关
        /// </summary>
        public static bool PersonalDropDisabled { get; set; } = true;
        /// <summary>
        /// 新版武器升级
        /// </summary>
        public static bool NewWeaponUpgrade { get; set; } = false;
        /// <summary>
        /// 宠物包裹使用药品
        /// </summary>
        public static bool AutoPotionForCompanion { get; set; } = true;
        /// <summary>
        /// 死亡包裹装备爆出几率
        /// </summary>
        public static int CharacterInventoryDeathDrop { get; set; } = 10;
        public static int InventoryAshDeathDrop { get; set; } = 2;
        public static int InventoryRedDeathDrop { get; set; } = 5;
        /// <summary>
        /// 死亡宠物包裹装备爆出几率
        /// </summary>
        public static int CompanionInventoryDeathDrop { get; set; } = 7;
        public static int ComInventoryAshDeathDrop { get; set; } = 2;
        public static int ComInventoryRedDeathDrop { get; set; } = 5;
        /// <summary>
        /// 死亡身上装备爆出几率
        /// </summary>
        public static int CharacterEquipmentDeathDrop { get; set; } = 40;
        public static int EquipmentAshDeathDrop { get; set; } = 100;
        public static int EquipmentRedDeathDrop { get; set; } = 200;
        public static int WeapEquipmentDeathDrop { get; set; } = 2;
        /// <summary>
        /// 死亡身上装备红名爆出几率
        /// </summary>
        public static int DieRedRandomChance { get; set; } = 10;
        /// <summary>
        /// 武器大师精炼随机值
        /// </summary>
        public static int MasterRefineChance { get; set; } = 80;
        /// <summary>
        /// 武器大师精炼附加属性值
        /// </summary>
        public static int MasterRefineCount { get; set; } = 5;
        /// <summary>
        /// 武器大师精炼附加属性值是否随机
        /// </summary>
        public static bool MasterRefineRandom { get; set; } = false;
        /// <summary>
        /// 道具持久消耗
        /// </summary>
        public static int DurabilityRate { get; set; } = 3;
        /// <summary>
        /// 时效道具安全区是否扣时间
        /// </summary>
        public static bool InSafeZoneItemExpire { get; set; } = true;
        /// <summary>
        /// 自动加血加蓝道具延迟
        /// </summary>
        public static int MedicamentHPMPTime { get; set; } = 2;
        /// <summary>
        /// 自动加血道具延迟
        /// </summary>
        public static int MedicamentHPTime { get; set; } = 3;
        /// <summary>
        /// 自动加蓝道具延迟
        /// </summary>
        public static int MedicamentMPTime { get; set; } = 3;
        /// <summary>
        /// 道具能否特修显示
        /// </summary>
        public static bool ItemCanRepair { get; set; } = false;
        //首饰合成设置
        public static bool JewelryLvShows { get; set; } = false;
        public static bool JewelryExpShows { get; set; } = false;
        public static int ACGoldRate { get; set; } = 100000;
        public static int CommonItemSuccessRate { get; set; } = 70;
        public static int CommonItemReduceRate { get; set; } = 5;
        public static int SuperiorItemSuccessRate { get; set; } = 80;
        public static int SuperiorItemReduceRate { get; set; } = 5;
        public static int EliteItemSuccessRate { get; set; } = 100;
        public static int EliteItemReduceRate { get; set; } = 5;
        public static int CommonItemLadder1 { get; set; } = 5;
        public static int CommonItemAdditionalValue1 { get; set; } = 1;
        public static int CommonItemLadder2 { get; set; } = 7;
        public static int CommonItemAdditionalValue2 { get; set; } = 2;
        public static int CommonItemLadder3 { get; set; } = 9;
        public static int CommonItemAdditionalValue3 { get; set; } = 3;
        public static int CommonItemLevelValue { get; set; } = 1;
        public static int SuperiorItemLadder1 { get; set; } = 5;
        public static int SuperiorItemAdditionalValue1 { get; set; } = 2;
        public static int SuperiorItemLadder2 { get; set; } = 7;
        public static int SuperiorItemAdditionalValue2 { get; set; } = 4;
        public static int SuperiorItemLadder3 { get; set; } = 9;
        public static int SuperiorItemAdditionalValue3 { get; set; } = 6;
        public static int SuperiorItemLevelValue { get; set; } = 1;
        public static int EliteItemLadder1 { get; set; } = 5;
        public static int EliteItemAdditionalValue1 { get; set; } = 3;
        public static int EliteItemLadder2 { get; set; } = 7;
        public static int EliteItemAdditionalValue2 { get; set; } = 6;
        public static int EliteItemLadder3 { get; set; } = 9;
        public static int EliteItemAdditionalValue3 { get; set; } = 9;
        public static int EliteItemLevelValue { get; set; } = 1;
        // 是否开启回购功能
        public static bool AllowBuyback { get; set; } = true;
        /// <summary>
        /// 死亡是否能丢弃道具
        /// </summary>
        public static bool DeadLoseItem { get; set; } = false;
        #endregion

        #region 属性加成设置
        /// <summary>
        /// 经验加成
        /// </summary>
        [ConfigSection("Rates")]   //属性加成       
        public static int ExperienceRate { get; set; } = 0;
        /// <summary>
        /// 爆率加成
        /// </summary>
        public static int DropRate { get; set; } = 0;
        /// <summary>
        /// 金币加成
        /// </summary>
        public static int GoldRate { get; set; } = 0;
        /// <summary>
        /// 技能经验值加成
        /// </summary>
        public static int SkillRate { get; set; } = 0;
        /// <summary>
        /// 宠物经验值加成
        /// </summary>
        public static int CompanionRate { get; set; } = 0;
        /// <summary>
        /// 回归者经验加成
        /// </summary>
        public static int VeteranRate { get; set; } = 50;
        //新手行会加成
        public static int StarterGuildLevelRate { get; set; } = 50;
        public static int StarterGuildExperienceRate { get; set; } = 50;
        public static int StarterGuildDropRate { get; set; } = 50;
        public static int StarterGuildGoldRate { get; set; } = 50;
        //沙巴克行会加成
        public static int CastleExperienceRate { get; set; } = 10;
        public static int CastleDropRate { get; set; } = 10;
        public static int CastleGoldRate { get; set; } = 10;
        //行会加成
        public static int GuildLevelRate { get; set; } = 15;
        public static int GuildExperienceRate { get; set; } = 30;
        public static int GuildDropRate { get; set; } = 30;
        public static int GuildGoldRate { get; set; } = 30;
        public static int GuildLevel1Rate { get; set; } = 30;
        public static int GuildExperience1Rate { get; set; } = 23;
        public static int GuildDrop1Rate { get; set; } = 23;
        public static int GuildGold1Rate { get; set; } = 23;
        public static int GuildLevel2Rate { get; set; } = 45;
        public static int GuildExperience2Rate { get; set; } = 18;
        public static int GuildDrop2Rate { get; set; } = 18;
        public static int GuildGold2Rate { get; set; } = 18;
        public static int GuildExperience3Rate { get; set; } = 13;
        public static int GuildDrop3Rate { get; set; } = 13;
        public static int GuildGold3Rate { get; set; } = 13;
        //观察者加成
        public static int StatExperienceRate { get; set; } = 15;
        public static int StatDropRate { get; set; } = 15;
        public static int StatGoldRate { get; set; } = 15;
        //组队加成
        public static decimal ZDEXPRate { get; set; } = 0.06M;
        public static decimal DZEXPRate { get; set; } = 1.1M;
        public static decimal SZEXPRate { get; set; } = 1.25M;
        public static decimal DRZEXPRate { get; set; } = 1.5M;
        public static decimal DZBLRate { get; set; } = 1.1M;
        public static decimal SZBLRate { get; set; } = 1.2M;
        public static decimal DRZBLRate { get; set; } = 1.3M;

        public static bool GroupOrGuild { get; set; } = true;
        /// <summary>
        /// 组队挂机勾选默认不加组队BUFF
        /// </summary>
        public static bool AutoTrivial { get; set; } = true;

        public static int GroupInZoneAddExp { get; set; } = 10;
        public static int GroupInZoneAddDrop { get; set; } = 10;

        public static int GroupAddBaseExp { get; set; } = 10;
        public static int GroupAddBaseDrop { get; set; } = 10;
        public static int GroupAddBaseGold { get; set; } = 10;
        public static int GroupAddBaseHp { get; set; } = 20;

        public static decimal GroupAddWarRate { get; set; } = 1.5M;
        public static decimal GroupAddWizRate { get; set; } = 0.8M;
        public static decimal GroupAddTaoRate { get; set; } = 1.1M;
        public static decimal GroupAddAssRate { get; set; } = 1.3M;
        #endregion

        #region 全局设置
        /// <summary>
        /// 全局定义
        /// </summary>
        [ConfigSection("Globals")]   //全局设置      
        public static decimal MarketPlaceTax { get; set; } = 0M; //0.05M;

        public static decimal NewAuctionTax { get; set; } = 0M; //0.05M;
        /// <summary>
        /// 退出行会时间定义
        /// </summary>
        public static int ExitGuild { get; set; } = 24;
        /// <summary>
        /// 赏金上限
        /// </summary>
        public static int HuntGoldCap { get; set; } = 15;
        /// <summary>
        /// 赏金每分钟获得数量
        /// </summary>
        public static int HuntGoldTime { get; set; } = 1;
        //推荐人等级
        public static int ReferrerLevel1 { get; set; } = 10;
        public static int ReferrerLevel2 { get; set; } = 20;
        public static int ReferrerLevel3 { get; set; } = 30;
        public static int ReferrerLevel4 { get; set; } = 40;
        public static int ReferrerLevel5 { get; set; } = 50;
        //推荐人获得赏金
        public static int ReferrerHuntGold1 { get; set; } = 50;
        public static int ReferrerHuntGold2 { get; set; } = 100;
        public static int ReferrerHuntGold3 { get; set; } = 200;
        public static int ReferrerHuntGold4 { get; set; } = 300;
        public static int ReferrerHuntGold5 { get; set; } = 500;
        /// <summary>
        /// 被推荐人充值推荐人获得元宝充值的比例赏金
        /// </summary>
        public static int HuntGoldRated { get; set; } = 10;
        /// <summary>
        /// 传奇宝箱重置元宝
        /// </summary>
        public static int Reset { get; set; } = 500;
        /// <summary>
        /// 传奇宝箱抽奖元宝
        /// </summary>
        public static int LuckDraw { get; set; } = 500;
        /// <summary>
        /// 转生降低等级
        /// </summary>
        public static int RebirthLevel { get; set; } = 1;
        /// <summary>
        /// 转生降低经验倍率
        /// </summary>
        public static int RebirthExp { get; set; } = 200;
        /// <summary>
        /// 转生增加金币倍率
        /// </summary>
        public static int RebirthGold { get; set; } = 20;
        /// <summary>
        /// 转生增加爆率倍率
        /// </summary>
        public static int RebirthDrop { get; set; } = 20;
        /// <summary>
        /// 转生增加PVE比例
        /// </summary>
        public static decimal RebirthPVE { get; set; } = 1.5M;
        /// <summary>
        /// 转生增加PVP比例
        /// </summary>
        public static decimal RebirthPVP { get; set; } = 1.2M;
        /// <summary>
        /// 转生以后经验获得降低率
        /// </summary>
        public static decimal RebirthReduceExp { get; set; } = 0.5M;
        /// <summary>
        /// 转生增加最大防御
        /// </summary>
        public static int RebirthAC { get; set; } = 0;
        /// <summary>
        /// 转生增加最大魔域
        /// </summary>
        public static int RebirthMAC { get; set; } = 0;
        /// <summary>
        /// 转生增加最大破坏
        /// </summary>
        public static int RebirthDC { get; set; } = 0;
        /// <summary>
        /// 转生增加最大自然
        /// </summary>
        public static int RebirthMC { get; set; } = 0;
        /// <summary>
        /// 转生增加最大灵魂
        /// </summary>
        public static int RebirthSC { get; set; } = 0;
        /// <summary>
        /// 转生死亡是否丢失经验
        /// </summary>
        public static bool RebirthDie { get; set; } = true;
        /// <summary>
        /// 排行榜显示开关
        /// </summary>
        public static bool RankingShow { get; set; } = true;
        /// <summary>
        /// 爆率查询开关
        /// </summary>
        public static bool RateQueryShow { get; set; } = false;
        /// <summary>
        /// 在线人数
        /// </summary>
        public static int UserCountDouble { get; set; } = 0;
        /// <summary>
        /// 在线人数提示
        /// </summary>
        public static bool UserCount { get; set; } = true;
        /// <summary>
        /// 在线人数提示时间延迟
        /// </summary>
        public static int UserCountTime { get; set; } = 5;
        /// <summary>
        ///防御元素
        /// </summary>
        public static int ElementResistance { get; set; } = 5;
        /// <summary>
        /// 元素体质启用开关
        /// </summary>
        public static bool PhysicalResistanceSwitch { get; set; } = false;
        /// <summary>
        /// 舒适
        /// </summary>
        public static int Comfort { get; set; } = 200;
        /// <summary>
        /// 攻击速度
        /// </summary>
        public static int AttackSpeed { get; set; } = 80;
        /// <summary>
        /// 幸运值
        /// </summary>
        public static int MaxLucky { get; set; } = 30;

        /// <summary>
        /// 泰山投币 命中判定半径
        /// 玩家投币的落点 小于等于此半径则视为投中
        /// 值越大 越容易命中
        /// </summary>
        public static double TossCoinOnTargetRadius { get; set; } = 0.8;

        /// <summary>
        /// 每日免费投币次数 24点重置
        /// </summary>
        public static int DailyFreeCoins { get; set; } = 10;
        /// <summary>
        /// 泰山投币地点选择
        /// </summary>
        public static bool CoinPlaceChoice { get; set; } = true;
        #endregion

        #region 装备极品设置
        /// <summary>
        /// 极品几率
        /// </summary>
        [ConfigSection("CreateDropItem")]  //装备极品设置       
        public static int GourmetRandom { get; set; } = 1;
        /// <summary>
        /// 高级稀世极品几率
        /// </summary>
        public static int GourmetType { get; set; } = 2;
        //武器
        public static int WeaponDC1 { get; set; } = 5;
        public static int WeaponDC11 { get; set; } = 1;
        public static int WeaponDC2 { get; set; } = 50;
        public static int WeaponDC22 { get; set; } = 1;
        public static int WeaponDC3 { get; set; } = 250;
        public static int WeaponDC33 { get; set; } = 1;
        public static int WeaponMSC1 { get; set; } = 5;
        public static int WeaponMSC11 { get; set; } = 1;
        public static int WeaponMSC2 { get; set; } = 50;
        public static int WeaponMSC22 { get; set; } = 1;
        public static int WeaponMSC3 { get; set; } = 250;
        public static int WeaponMSC33 { get; set; } = 1;
        public static int WeaponACC1 { get; set; } = 5;
        public static int WeaponACC11 { get; set; } = 1;
        public static int WeaponACC2 { get; set; } = 250;
        public static int WeaponACC22 { get; set; } = 1;
        public static int WeaponACC3 { get; set; } = 1250;
        public static int WeaponACC33 { get; set; } = 1;
        public static int WeaponELE1 { get; set; } = 3;
        public static int WeaponELE11 { get; set; } = 1;
        public static int WeaponELE2 { get; set; } = 5;
        public static int WeaponELE22 { get; set; } = 1;
        public static int WeaponELE3 { get; set; } = 25;
        public static int WeaponELE33 { get; set; } = 1;
        public static int WeaponAS1 { get; set; } = 3;
        public static int WeaponAS11 { get; set; } = 1;
        public static int WeaponAS2 { get; set; } = 5;
        public static int WeaponAS22 { get; set; } = 1;
        public static int WeaponAS3 { get; set; } = 25;
        public static int WeaponAS33 { get; set; } = 1;
        //衣服
        public static int ArmourAC1 { get; set; } = 2;
        public static int ArmourAC11 { get; set; } = 1;
        public static int ArmourAC2 { get; set; } = 15;
        public static int ArmourAC22 { get; set; } = 1;
        public static int ArmourAC3 { get; set; } = 150;
        public static int ArmourAC33 { get; set; } = 1;
        public static int ArmourMR1 { get; set; } = 2;
        public static int ArmourMR11 { get; set; } = 1;
        public static int ArmourMR2 { get; set; } = 15;
        public static int ArmourMR22 { get; set; } = 1;
        public static int ArmourMR3 { get; set; } = 150;
        public static int ArmourMR33 { get; set; } = 1;
        public static int ArmourHP1 { get; set; } = 2;
        public static int ArmourHP11 { get; set; } = 10;
        public static int ArmourHP2 { get; set; } = 15;
        public static int ArmourHP22 { get; set; } = 10;
        public static int ArmourHP3 { get; set; } = 150;
        public static int ArmourHP33 { get; set; } = 10;
        public static int ArmourMP1 { get; set; } = 2;
        public static int ArmourMP11 { get; set; } = 10;
        public static int ArmourMP2 { get; set; } = 15;
        public static int ArmourMP22 { get; set; } = 10;
        public static int ArmourMP3 { get; set; } = 150;
        public static int ArmourMP33 { get; set; } = 10;
        public static int ArmourDC1 { get; set; } = 5;
        public static int ArmourDC11 { get; set; } = 1;
        public static int ArmourDC2 { get; set; } = 50;
        public static int ArmourDC22 { get; set; } = 1;
        public static int ArmourDC3 { get; set; } = 250;
        public static int ArmourDC33 { get; set; } = 1;
        public static int ArmourMSC1 { get; set; } = 5;
        public static int ArmourMSC11 { get; set; } = 1;
        public static int ArmourMSC2 { get; set; } = 50;
        public static int ArmourMSC22 { get; set; } = 1;
        public static int ArmourMSC3 { get; set; } = 250;
        public static int ArmourMSC33 { get; set; } = 1;
        public static int ArmourELE1 { get; set; } = 10;
        public static int ArmourELE11 { get; set; } = 2;
        public static int ArmourELE2 { get; set; } = 45;
        public static int ArmourELE22 { get; set; } = 2;
        public static int ArmourELE3 { get; set; } = 60;
        public static int ArmourELE33 { get; set; } = 2;
        public static int RArmourELE1 { get; set; } = 10;
        public static int RArmourELE11 { get; set; } = 2;
        public static int RArmourELE2 { get; set; } = 45;
        public static int RArmourELE22 { get; set; } = 2;
        public static int RArmourELE3 { get; set; } = 60;
        public static int RArmourELE33 { get; set; } = 2;
        //头盔
        public static int HelmetAC1 { get; set; } = 5;
        public static int HelmetAC11 { get; set; } = 1;
        public static int HelmetAC2 { get; set; } = 25;
        public static int HelmetAC22 { get; set; } = 1;
        public static int HelmetAC3 { get; set; } = 250;
        public static int HelmetAC33 { get; set; } = 1;
        public static int HelmetMR1 { get; set; } = 5;
        public static int HelmetMR11 { get; set; } = 1;
        public static int HelmetMR2 { get; set; } = 25;
        public static int HelmetMR22 { get; set; } = 1;
        public static int HelmetMR3 { get; set; } = 250;
        public static int HelmetMR33 { get; set; } = 1;
        public static int HelmetHP1 { get; set; } = 2;
        public static int HelmetHP11 { get; set; } = 10;
        public static int HelmetHP2 { get; set; } = 15;
        public static int HelmetHP22 { get; set; } = 10;
        public static int HelmetHP3 { get; set; } = 150;
        public static int HelmetHP33 { get; set; } = 10;
        public static int HelmetMP1 { get; set; } = 2;
        public static int HelmetMP11 { get; set; } = 10;
        public static int HelmetMP2 { get; set; } = 15;
        public static int HelmetMP22 { get; set; } = 10;
        public static int HelmetMP3 { get; set; } = 150;
        public static int HelmetMP33 { get; set; } = 10;
        public static int HelmetDC1 { get; set; } = 5;
        public static int HelmetDC11 { get; set; } = 1;
        public static int HelmetDC2 { get; set; } = 50;
        public static int HelmetDC22 { get; set; } = 1;
        public static int HelmetDC3 { get; set; } = 250;
        public static int HelmetDC33 { get; set; } = 1;
        public static int HelmetMSC1 { get; set; } = 5;
        public static int HelmetMSC11 { get; set; } = 1;
        public static int HelmetMSC2 { get; set; } = 50;
        public static int HelmetMSC22 { get; set; } = 1;
        public static int HelmetMSC3 { get; set; } = 250;
        public static int HelmetMSC33 { get; set; } = 1;

        public static int HelmetGELE1 { get; set; } = 10;
        public static int HelmetGELE11 { get; set; } = 1;
        public static int HelmetGELE2 { get; set; } = 45;
        public static int HelmetGELE22 { get; set; } = 1;
        public static int HelmetGELE3 { get; set; } = 60;
        public static int HelmetGELE33 { get; set; } = 1;

        public static int HelmetELE1 { get; set; } = 10;
        public static int HelmetELE11 { get; set; } = 1;
        public static int HelmetELE2 { get; set; } = 45;
        public static int HelmetELE22 { get; set; } = 1;
        public static int HelmetELE3 { get; set; } = 60;
        public static int HelmetELE33 { get; set; } = 1;
        public static int RHelmetELE1 { get; set; } = 10;
        public static int RHelmetELE11 { get; set; } = 1;
        public static int RHelmetELE2 { get; set; } = 45;
        public static int RHelmetELE22 { get; set; } = 1;
        public static int RHelmetELE3 { get; set; } = 60;
        public static int RHelmetELE33 { get; set; } = 1;
        //项链
        public static int NecklaceDC1 { get; set; } = 5;
        public static int NecklaceDC11 { get; set; } = 1;
        public static int NecklaceDC2 { get; set; } = 25;
        public static int NecklaceDC22 { get; set; } = 1;
        public static int NecklaceDC3 { get; set; } = 250;
        public static int NecklaceDC33 { get; set; } = 1;
        public static int NecklaceMSC1 { get; set; } = 5;
        public static int NecklaceMSC11 { get; set; } = 1;
        public static int NecklaceMSC2 { get; set; } = 25;
        public static int NecklaceMSC22 { get; set; } = 1;
        public static int NecklaceMSC3 { get; set; } = 250;
        public static int NecklaceMSC33 { get; set; } = 1;
        public static int NecklaceME1 { get; set; } = 50;
        public static int NecklaceME11 { get; set; } = 10;
        public static int NecklaceME2 { get; set; } = 500;
        public static int NecklaceME22 { get; set; } = 10;
        public static int NecklaceME3 { get; set; } = 2500;
        public static int NecklaceME33 { get; set; } = 10;
        public static int NecklaceACC1 { get; set; } = 5;
        public static int NecklaceACC11 { get; set; } = 1;
        public static int NecklaceACC2 { get; set; } = 25;
        public static int NecklaceACC22 { get; set; } = 1;
        public static int NecklaceACC3 { get; set; } = 250;
        public static int NecklaceACC33 { get; set; } = 1;
        public static int NecklaceAG1 { get; set; } = 5;
        public static int NecklaceAG11 { get; set; } = 1;
        public static int NecklaceAG2 { get; set; } = 25;
        public static int NecklaceAG22 { get; set; } = 1;
        public static int NecklaceAG3 { get; set; } = 250;
        public static int NecklaceAG33 { get; set; } = 1;
        public static int NecklaceELE1 { get; set; } = 3;
        public static int NecklaceELE11 { get; set; } = 1;
        public static int NecklaceELE2 { get; set; } = 5;
        public static int NecklaceELE22 { get; set; } = 1;
        public static int NecklaceELE3 { get; set; } = 25;
        public static int NecklaceELE33 { get; set; } = 1;
        //手镯
        public static int BraceletDC1 { get; set; } = 5;
        public static int BraceletDC11 { get; set; } = 1;
        public static int BraceletDC2 { get; set; } = 25;
        public static int BraceletDC22 { get; set; } = 1;
        public static int BraceletDC3 { get; set; } = 250;
        public static int BraceletDC33 { get; set; } = 1;
        public static int BraceletMSC1 { get; set; } = 5;
        public static int BraceletMSC11 { get; set; } = 1;
        public static int BraceletMSC2 { get; set; } = 25;
        public static int BraceletMSC22 { get; set; } = 1;
        public static int BraceletMSC3 { get; set; } = 250;
        public static int BraceletMSC33 { get; set; } = 1;
        public static int BraceletAC1 { get; set; } = 5;
        public static int BraceletAC11 { get; set; } = 1;
        public static int BraceletAC2 { get; set; } = 15;
        public static int BraceletAC22 { get; set; } = 1;
        public static int BraceletAC3 { get; set; } = 150;
        public static int BraceletAC33 { get; set; } = 1;
        public static int BraceletMR1 { get; set; } = 5;
        public static int BraceletMR11 { get; set; } = 1;
        public static int BraceletMR2 { get; set; } = 15;
        public static int BraceletMR22 { get; set; } = 1;
        public static int BraceletMR3 { get; set; } = 150;
        public static int BraceletMR33 { get; set; } = 1;
        public static int BraceletACC1 { get; set; } = 5;
        public static int BraceletACC11 { get; set; } = 1;
        public static int BraceletACC2 { get; set; } = 25;
        public static int BraceletACC22 { get; set; } = 1;
        public static int BraceletACC3 { get; set; } = 250;
        public static int BraceletACC33 { get; set; } = 1;
        public static int BraceletAG1 { get; set; } = 5;
        public static int BraceletAG11 { get; set; } = 1;
        public static int BraceletAG2 { get; set; } = 25;
        public static int BraceletAG22 { get; set; } = 1;
        public static int BraceletAG3 { get; set; } = 250;
        public static int BraceletAG33 { get; set; } = 1;
        public static int BraceletELE1 { get; set; } = 3;
        public static int BraceletELE11 { get; set; } = 1;
        public static int BraceletELE2 { get; set; } = 5;
        public static int BraceletELE22 { get; set; } = 1;
        public static int BraceletELE3 { get; set; } = 25;
        public static int BraceletELE33 { get; set; } = 1;
        public static int BraceletElE1 { get; set; } = 10;
        public static int BraceletElE11 { get; set; } = 1;
        public static int BraceletElE2 { get; set; } = 30;
        public static int BraceletElE22 { get; set; } = 1;
        public static int BraceletElE3 { get; set; } = 40;
        public static int BraceletElE33 { get; set; } = 1;
        public static int RBraceletElE1 { get; set; } = 10;
        public static int RBraceletElE11 { get; set; } = 1;
        public static int RBraceletElE2 { get; set; } = 30;
        public static int RBraceletElE22 { get; set; } = 1;
        public static int RBraceletElE3 { get; set; } = 40;
        public static int RBraceletElE33 { get; set; } = 1;
        //戒指
        public static int RingDC1 { get; set; } = 5;
        public static int RingDC11 { get; set; } = 1;
        public static int RingDC2 { get; set; } = 25;
        public static int RingDC22 { get; set; } = 1;
        public static int RingDC3 { get; set; } = 250;
        public static int RingDC33 { get; set; } = 1;
        public static int RingMSC1 { get; set; } = 5;
        public static int RingMSC11 { get; set; } = 1;
        public static int RingMSC2 { get; set; } = 25;
        public static int RingMSC22 { get; set; } = 1;
        public static int RingMSC3 { get; set; } = 250;
        public static int RingMSC33 { get; set; } = 1;
        public static int RingELE1 { get; set; } = 3;
        public static int RingELE11 { get; set; } = 1;
        public static int RingELE2 { get; set; } = 5;
        public static int RingELE22 { get; set; } = 1;
        public static int RingELE3 { get; set; } = 25;
        public static int RingELE33 { get; set; } = 1;
        //鞋子
        public static int ShoesAC1 { get; set; } = 5;
        public static int ShoesAC11 { get; set; } = 1;
        public static int ShoesAC2 { get; set; } = 15;
        public static int ShoesAC22 { get; set; } = 1;
        public static int ShoesAC3 { get; set; } = 150;
        public static int ShoesAC33 { get; set; } = 1;
        public static int ShoesMR1 { get; set; } = 5;
        public static int ShoesMR11 { get; set; } = 1;
        public static int ShoesMR2 { get; set; } = 15;
        public static int ShoesMR22 { get; set; } = 1;
        public static int ShoesMR3 { get; set; } = 150;
        public static int ShoesMR33 { get; set; } = 1;
        public static int ShoesHP1 { get; set; } = 2;
        public static int ShoesHP11 { get; set; } = 10;
        public static int ShoesHP2 { get; set; } = 15;
        public static int ShoesHP22 { get; set; } = 10;
        public static int ShoesHP3 { get; set; } = 150;
        public static int ShoesHP33 { get; set; } = 10;
        public static int ShoesMP1 { get; set; } = 2;
        public static int ShoesMP11 { get; set; } = 10;
        public static int ShoesMP2 { get; set; } = 15;
        public static int ShoesMP22 { get; set; } = 10;
        public static int ShoesMP3 { get; set; } = 150;
        public static int ShoesMP33 { get; set; } = 10;
        public static int ShoesCF1 { get; set; } = 5;
        public static int ShoesCF11 { get; set; } = 10;
        public static int ShoesCF2 { get; set; } = 25;
        public static int ShoesCF22 { get; set; } = 10;
        public static int ShoesCF3 { get; set; } = 250;
        public static int ShoesCF33 { get; set; } = 10;
        public static int ShoesELE1 { get; set; } = 10;
        public static int ShoesELE11 { get; set; } = 1;
        public static int ShoesELE2 { get; set; } = 45;
        public static int ShoesELE22 { get; set; } = 1;
        public static int ShoesELE3 { get; set; } = 60;
        public static int ShoesELE33 { get; set; } = 1;
        public static int RShoesELE1 { get; set; } = 10;
        public static int RShoesELE11 { get; set; } = 1;
        public static int RShoesELE2 { get; set; } = 45;
        public static int RShoesELE22 { get; set; } = 1;
        public static int RShoesELE3 { get; set; } = 60;
        public static int RShoesELE33 { get; set; } = 1;
        //盾牌
        public static int ShieldDCP1 { get; set; } = 10;
        public static int ShieldDCP11 { get; set; } = 1;
        public static int ShieldDCP2 { get; set; } = 50;
        public static int ShieldDCP22 { get; set; } = 1;
        public static int ShieldDCP3 { get; set; } = 250;
        public static int ShieldDCP33 { get; set; } = 1;
        public static int ShieldMSCP1 { get; set; } = 10;
        public static int ShieldMSCP11 { get; set; } = 1;
        public static int ShieldMSCP2 { get; set; } = 50;
        public static int ShieldMSCP22 { get; set; } = 1;
        public static int ShieldMSCP3 { get; set; } = 250;
        public static int ShieldMSCP33 { get; set; } = 1;
        public static int ShieldBC1 { get; set; } = 10;
        public static int ShieldBC11 { get; set; } = 1;
        public static int ShieldBC2 { get; set; } = 50;
        public static int ShieldBC22 { get; set; } = 1;
        public static int ShieldBC3 { get; set; } = 250;
        public static int ShieldBC33 { get; set; } = 1;
        public static int ShieldEC1 { get; set; } = 10;
        public static int ShieldEC11 { get; set; } = 1;
        public static int ShieldEC2 { get; set; } = 50;
        public static int ShieldEC22 { get; set; } = 1;
        public static int ShieldEC3 { get; set; } = 250;
        public static int ShieldEC33 { get; set; } = 1;
        public static int ShieldPR1 { get; set; } = 10;
        public static int ShieldPR11 { get; set; } = 1;
        public static int ShieldPR2 { get; set; } = 50;
        public static int ShieldPR22 { get; set; } = 1;
        public static int ShieldPR3 { get; set; } = 250;
        public static int ShieldPR33 { get; set; } = 1;
        public static int ShieldELE1 { get; set; } = 10;
        public static int ShieldELE11 { get; set; } = 2;
        public static int ShieldELE2 { get; set; } = 45;
        public static int ShieldELE22 { get; set; } = 2;
        public static int ShieldELE3 { get; set; } = 60;
        public static int ShieldELE33 { get; set; } = 2;
        public static int RShieldELE1 { get; set; } = 10;
        public static int RShieldELE11 { get; set; } = 2;
        public static int RShieldELE2 { get; set; } = 45;
        public static int RShieldELE22 { get; set; } = 2;
        public static int RShieldELE3 { get; set; } = 60;
        public static int RShieldELE33 { get; set; } = 2;
        #endregion

        #region 魔法技能设置
        /// <summary>
        /// 技能经验倍率
        /// </summary>
        [ConfigSection("Magic")]  //魔法技能设置      
        public static int SkillExp { get; set; } = 3;
        /// <summary>
        /// 技能等级限制
        /// </summary>
        public static int MaxMagicLv { get; set; } = 3;
        /// <summary>
        /// 4级技能吃书点数
        /// </summary>
        public static int SkillExpDrop { get; set; } = 500;
        /// <summary>
        /// 直线魔法攻击是否限制地形
        /// </summary>
        public static bool CanFlyTargetCheck { get; set; } = true;
        /// <summary>
        /// 乾坤大挪移
        /// </summary>
        public static bool QKDNY { get; set; } = false;
        /// <summary>
        /// 铁布衫
        /// </summary>
        public static bool ZTBS { get; set; } = false;
        /// <summary>
        /// 破血狂杀
        /// </summary>
        public static bool PXKS { get; set; } = false;
        /// <summary>
        /// 斗转星移麻痹效果
        /// </summary>
        public static bool BeckonParalysis { get; set; } = true;
        /// <summary>
        /// 移形换位
        /// </summary>
        public static bool YXHW { get; set; } = false;
        /// <summary>
        /// 移行换位的使用成功率
        /// </summary>
        public static int GeoManipulationSuccessRate { get; set; } = 25;
        /// <summary>
        /// 圣言术
        /// </summary>
        public static int ExpelUndeadSuccessRate { get; set; } = 35;
        /// <summary>
        /// 可圣言最高等级
        /// </summary>
        public static int ExpelUndeadLevel { get; set; } = 70;
        /// <summary>
        /// 分身是否能移动
        /// </summary>
        public static bool MirrorImageCanMove { get; set; } = false;
        /// <summary>
        /// 召唤分身消耗魔石点数
        /// </summary>
        public static int MirrorImageDamageDarkStone { get; set; } = 10;
        /// <summary>
        /// 魔法盾伤害减少时间毫秒
        /// </summary>
        public static int MagicShieldRemainingTime { get; set; } = 25;
        /// <summary>
        /// 诱惑之光能诱惑的怪物数量
        /// </summary>
        public static int ElectricShockPetsCount { get; set; } = 5;
        /// <summary>
        /// 诱惑之光成功几率
        /// </summary>
        public static int ElectricShockSuccessRate { get; set; } = 4;
        /// <summary>
        /// 诱惑宝宝叛变时间
        /// </summary>
        public static int PetsMutinyTime { get; set; } = 1;
        /// <summary>
        /// 焰天火雨火球数量
        /// </summary>
        public static int MeteorShowerTargetsCount { get; set; } = 5;
        /// <summary>
        /// 电闪雷鸣攻击范围
        /// </summary>
        public static int ThunderStrikeGetCells { get; set; } = 5;
        /// <summary>
        /// 电闪雷鸣命中几率
        /// </summary>
        public static int ThunderStrikeRandom { get; set; } = 40;
        /// <summary>
        /// 妙影无踪移行
        /// </summary>     
        public static bool MYWZYX { get; set; } = false;
        /// <summary>
        /// 妙影无踪喝药
        /// </summary>
        public static bool MYWZHY { get; set; } = false;
        /// <summary>
        /// 允许使用@允许回生术
        /// </summary>
        public static bool ResurrectionOrder { get; set; } = false;
        /// <summary>
        /// 焰魔强解术PVP伤害降低倍率
        /// </summary>
        public static int DemonExplosionPVP { get; set; } = 1;
        /// <summary>
        /// 施毒术伤害倍率
        /// </summary>
        public static float PoisonDustValue { get; set; } = 1F;
        /// <summary>
        /// 施毒术是否开启暗黑加成
        /// </summary>
        public static bool PoisonDustDarkAttack { get; set; } = false;
        /// <summary>
        /// 施毒术对BOSS生效
        /// </summary>
        public static bool PoisoningBossCheck { get; set; } = true;
        /// <summary>
        /// 中毒是否0血死亡
        /// </summary>
        public static bool PoisonDead { get; set; } = false;
        /// <summary>
        /// 红毒攻击伤害比率
        /// </summary>
        public static float RedPoisonAttackRate { get; set; } = 1.2F;
        /// <summary>
        /// 治愈术固定加血值
        /// </summary>
        public static int FixedCureValue { get; set; } = 30;
        /// <summary>
        /// 治愈术是否固定加血
        /// </summary>
        public static bool FixedCure { get; set; } = true;
        /// <summary>
        /// 灵魂火符暗黑加成率
        /// </summary>
        public static float ETDarkAffinityRate { get; set; } = 0.2F;
        /// <summary>
        /// 刷狗随机率
        /// </summary>
        public static int SummonRandomValue { get; set; } = 75;
        /// <summary>
        /// 妙影怪物寻目标延迟时间
        /// </summary>
        public static int TraOverTime { get; set; } = 50;
        /// <summary>
        /// 最终抵抗触发几率
        /// </summary>
        public static int ZHDKHP { get; set; } = 85;
        /// <summary>
        /// 最终抵抗攻击范围
        /// </summary>
        public static int ZHDKFW { get; set; } = 2;
        /// <summary>
        /// 最终抵抗Z版设置
        /// </summary>
        public static bool ZZDKZ { get; set; } = false;
        /// <summary>
        /// 鹰击
        /// </summary>
        public static bool YJDTYD { get; set; } = false;
        /// <summary>
        /// 鹰击沉迷
        /// </summary>
        public static bool DanceOfSwallowSilenced { get; set; } = true;
        /// <summary>
        /// 鹰击麻痹
        /// </summary>
        public static bool DanceOfSwallowParalysis { get; set; } = true;
        #endregion

        #region 行会设置
        /// <summary>
        /// 行会设置
        /// </summary>
        [ConfigSection("Guild")]  //行会信息设置
        /// <summary>
        /// 行会创建费用
        /// </summary>
        public static int GuildCreationCost { get; set; } = 7500000;
        /// <summary>
        /// 行会成员扩展费用
        /// </summary>
        public static int GuildMemberCost { get; set; } = 1000000;
        /// <summary>
        /// 行会仓库扩展费用
        /// </summary>
        public static int GuildStorageCost { get; set; } = 350000;
        /// <summary>
        /// 行会人数总上限
        /// </summary>
        public static int GuildMemberHardLimit { get; set; } = 200;
        /// <summary>
        /// 行会战费用
        /// </summary>
        public static int GuildWarCost { get; set; } = 200000;
        /// <summary>
        /// 行会资金上限
        /// </summary>
        public static long GuildMaxFund { get; set; } = 500000000;
        /// <summary>
        /// 每个行会保留多少条的捐献记录
        /// 超过了 服务端会自动删除最早的记录
        /// </summary>
        public static int GuildFundChangeRecordLimit { get; set; } = 2000;
        /// <summary>
        /// 个人活跃度上限
        /// </summary>
        public static int ActivationCeiling { get; set; } = 10;
        /// <summary>
        /// 个人捐赠金币对应活跃度1点
        /// </summary>
        public static int PersonalGoldRatio { get; set; } = 100000;
        /// <summary>
        /// 个人获得经验对应活跃度1点
        /// </summary>
        public static int PersonalExpRatio { get; set; } = 5000000;
        #endregion

        #region 城堡设置
        /// <summary>
        /// 城堡设置
        /// </summary>
        [ConfigSection("Conquest")]  //城堡信息设置
        /// <summary>
        /// 夺旗成功将敌对传送回复活点
        /// </summary>
        public static bool FlagSuccessTeleport { get; set; } = false;
        /// <summary>
        /// 攻城结束将玩家传送回复活点
        /// </summary>
        public static bool EndWarTeleport { get; set; } = false;
        /// <summary>
        /// 夺旗以后没人占领胜利时间
        /// </summary>
        public static int VictoryDelay { get; set; } = 30;
        /// <summary>
        /// 攻城时旗帜需要延时才能点击占领
        /// </summary>
        public static bool ConquestFlagDelay { get; set; } = false;
        /// <summary>
        /// 是否使攻城方死亡后在攻城地图复活点复活
        /// </summary>
        public static bool AttackerReviveInDesignatedAreaDuringWar { get; set; } = false;
        /// <summary>
        /// 是否允许在攻城旗帜3格范围内使用位移技能
        /// </summary>
        public static bool AllowTeleportMagicNearFlag { get; set; } = false;
        /// <summary>
        /// 旗帜范围格子限制
        /// </summary>
        public static int WarFlagRangeLimit { get; set; } = 3;
        #endregion

        #region 大补帖设置
        /// <summary>
        /// 自动挂机
        /// </summary>
        [ConfigSection("BigPatchConfig")]  //大补帖设置
        public static bool AutoHookTab { get; set; } = true;
        /// <summary>
        /// 挂机时间定义
        /// </summary>
        public static int AutoTime { get; set; } = 8;
        /// <summary>
        /// 挂机爆率加成
        /// </summary>
        public static int HooKDrop { get; set; } = 0;
        /// <summary>
        /// 免蜡
        /// </summary>
        public static bool BrightBox { get; set; } = true;
        /// <summary>
        /// 免助跑
        /// </summary>
        public static bool RunCheck { get; set; } = true;
        /// <summary>
        /// 稳如泰山
        /// </summary>
        public static bool RockCheck { get; set; } = true;
        /// <summary>
        /// 清理尸体
        /// </summary>
        public static bool ClearBodyCheck { get; set; } = true;
        /// <summary>
        /// 大补帖喝药延迟毫秒
        /// </summary>
        public static int BigPatchAutoPotionDelay { get; set; } = 500;
        #endregion

        #region 内核参数设置
        /// <summary>
        /// 攻击延迟1500
        /// <para>* 因为每次攻击都会有个站立的动作</para>
        /// <para>* 所以攻击延迟不能低于MirAnimation.Stance帧总延迟时间，否则会卡动作</para>
        /// </summary>
        [ConfigSection("KernelParameter")]  //内核参数
        public static int GlobalsAttackDelay { get; set; } = 1500;
        /// <summary>
        /// 攻击速度倍数47
        /// </summary>
        public static int GlobalsASpeedRate { get; set; } = 47;
        /// <summary>
        /// 投掷物速度48
        /// </summary>
        public static int GlobalsProjectileSpeed { get; set; } = 48;

        /// <summary>
        /// 转向时间
        /// </summary>
        public static int GlobalsTurnTime { get; set; } = 300;
        /// <summary>
        /// 收割时间
        /// </summary>
        public static int GlobalsHarvestTime { get; set; } = 600;
        /// <summary>
        /// 移动时间
        /// </summary>
        public static int GlobalsMoveTime { get; set; } = 600;
        /// <summary>
        /// 攻击时间
        /// </summary>
        public static int GlobalsAttackTime { get; set; } = 600;
        /// <summary>
        /// 投掷时间
        /// </summary>
        public static int GlobalsCastTime { get; set; } = 600;
        /// <summary>
        /// 施法时间延迟
        /// <para>* 因为每次施法都会有个举手或平推+站立的动作</para>
        /// <para>* 所以施法时间延迟不能低于MirAnimation.Stance + 举手或平推的帧总延迟时间，否则会卡动作</para>
        /// </summary>
        public static int GlobalsMagicDelay { get; set; } = 2000;
        #endregion

        #region 公告信息设置
        /// <summary>
        /// 公告信息播放时间设置
        /// </summary>
        [ConfigSection("Notices")]   //公告信息设置     
        public static TimeSpan NoticeDelay { get; set; } = TimeSpan.FromMinutes(5);
        public static string Notice0 { get; set; } = @"";
        public static string Notice1 { get; set; } = @"";
        public static string Notice2 { get; set; } = @"";
        public static string Notice3 { get; set; } = @"";
        public static string Notice4 { get; set; } = @"";
        public static string Notice5 { get; set; } = @"";
        public static string Notice6 { get; set; } = @"";
        public static string Notice7 { get; set; } = @"";
        public static string Notice8 { get; set; } = @"";
        public static string Notice9 { get; set; } = @"";
        #endregion

        #region 其他
        //镶嵌系统相关
        /// <summary>
        /// 默认：镶嵌成功的概率
        /// </summary>
        public static int GemSuccessChance = 50;
        /// <summary>
        /// 默认：使装备破碎的概率
        /// </summary>
        public static int GemBreakChance = 20;
        /// <summary>
        /// 增加这个设定值时，注意要在StatSource添加新的类型
        /// </summary>
        public static int MaxGemPerItem = 3;

        /// <summary>
        /// 授权相关
        /// </summary>
        public static string LicenseFile { get; set; } = @"./授权.license";

        /// <summary>
        /// 是否启用Sentry
        /// </summary>
        public static bool SentryEnabled { get; set; } = true;
#if DEBUG
        /// <summary>
        /// Sentry地址-服务端 debug
        /// 1：unknowngas账号
        /// </summary>
        public static readonly string SentryServerDSN1 = "https://4b3192a69b5a41e781c4f7900a3cd58a@o564963.ingest.sentry.io/5706052";
        public static readonly string SentryServerDSN2 = "https://7f6643c9c47748d1ab483a82401beeef@o578604.ingest.sentry.io/5734939";
#else
        /// <summary>
        /// Sentry地址-服务端 release
        /// </summary>
        public static readonly string SentryServerDSN1 = "https://ed9fc308a76047ca940a8a6191a7550d@o564963.ingest.sentry.io/5706123";
        public static readonly string SentryServerDSN2 = "https://bbc92ede11a94dd7bf9c96fa189620b5@o578604.ingest.sentry.io/5734940";
#endif

        /// <summary>
        /// 是否启用Py脚本统计信息
        /// </summary>
        public static bool EnablePyMetrics { get; set; } = true;

        /// <summary>
        /// 是否启用奖金池玩法
        /// true = 启用
        /// </summary>
        public static bool EnableRewardPool { get; set; } = true;
        /// <summary>
        /// 充值是否计入奖金池
        /// true = 计入
        /// </summary>
        public static bool EnableRewardPoolTopUp { get; set; } = false;
        /// <summary>
        /// 商城消费金额是否计入奖金池
        /// true = 计入
        /// </summary>
        public static bool EnableRewardPoolMarketBuy { get; set; } = true;
        /// <summary>
        /// 奖金池 对应Workflows文件夹内的json文件名字
        /// 添加奖金的workflow
        /// </summary>
        public static string RewardPoolAddBalanceFileName { get; set; } = "奖金池玩法-添加奖金";
        /// <summary>
        /// 更新奖金池buff的workflow
        /// </summary>
        public static string RewardPoolUpdateBuffFileName { get; set; } = "奖金池玩法-更新Buff";
        /// <summary>
        /// 领取奖金资格
        /// </summary>
        public static string RewardPoolClaimCheckFileName { get; set; } = "奖金池玩法-领取奖金资格检查";
        /// <summary>
        /// 领取奖金的workflow
        /// </summary>
        public static string RewardPoolClaimFileName { get; set; } = "奖金池玩法-领取奖金";
        /// <summary>
        /// 每隔几秒向客户端发送最新奖金池信息
        /// </summary>
        public static int RewardPoolUpdateDelay { get; set; } = 5;
        /// <summary>
        /// 每隔几秒向客户端发送奖金池排名信息，60即可，不要太低，会卡
        /// </summary>
        public static int RewardPoolRankUpdateDelay { get; set; } = 5;

        /// <summary>
        /// 是否启用红包功能
        /// true = 启用
        /// </summary>
        public static bool EnableRedPacket { get; set; } = true;

        /// <summary>
        /// 是否开启背号功能（需要脚本配合）
        /// </summary>
        public static bool EnableFollowing { get; set; } = true;

        /// <summary>
        /// 每隔多少秒要求客户端上报进程信息（反外挂）
        /// </summary>
        public static int RequestProcessHashDelay { get; set; } = 60;
        #endregion

        /// <summary>
        /// 加载版本
        /// </summary>
        public static void LoadVersion()
        {
            try
            {
                if (File.Exists(VersionPath))
                    using (FileStream stream = File.OpenRead(VersionPath))
                    using (MD5 md5 = MD5.Create())
                        ClientHash = md5.ComputeHash(stream);
                else ClientHash = null;

                if (File.Exists(VersionPath1))
                    using (FileStream stream1 = File.OpenRead(VersionPath1))
                    using (MD5 md51 = MD5.Create())
                        ClientHash1 = md51.ComputeHash(stream1);
                else ClientHash1 = null;

                if (File.Exists(ClientSystemDBPath))
                    using (FileStream stream1 = File.OpenRead(ClientSystemDBPath))
                    using (MD5 md51 = MD5.Create())
                        ClientSystemDBHash = md51.ComputeHash(stream1);
                else ClientSystemDBHash = null;
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
    }
}
