using Library;

namespace PatchManager
{
    /// <summary>
    /// 配置路径
    /// </summary>
    [ConfigPath(@".\PatchManager.ini")]
    public static class Config
    {
        /// <summary>
        /// 端游干净的补丁目录
        /// </summary>
        [ConfigSection("Patcher")]
        public static string PcCleanClientPath { get; set; } = @"C:\Server\Clients\Patch Files\";
        /// <summary>
        /// 端游FTP地址
        /// </summary>
        public static string PcFtpHost { get; set; } = @"ftp://ftp.zirconserver.com/";
        /// <summary>
        /// 端游FTP是否使用用户验证
        /// </summary>
        public static bool PcFtpUseLogin { get; set; } = true;
        /// <summary>
        /// 端游FTP用户名
        /// </summary>
        public static string PcUsername { get; set; } = @"REDACTED";
        /// <summary>
        /// 端游FTP密码
        /// </summary>
        public static string PcPassword { get; set; } = @"REDACTED";
        /// <summary>
        /// 手游干净的补丁目录
        /// </summary>
        [ConfigSection("MobPatcher")]
        public static string MobCleanClientPath { get; set; } = @"C:\Server\MobClients\Patch Files\";
        /// <summary>
        /// 手游FTP地址
        /// </summary>
        public static string MobFtpHost { get; set; } = @"ftp://ftp.zirconserver.com/";
        /// <summary>
        /// 手游FTP是否使用用户验证
        /// </summary>
        public static bool MobFtpUseLogin { get; set; } = true;
        /// <summary>
        /// 手游FTP用户名
        /// </summary>
        public static string MobUsername { get; set; } = @"REDACTED";
        /// <summary>
        /// 手游FTP密码
        /// </summary>
        public static string MobPassword { get; set; } = @"REDACTED";
    }
}
