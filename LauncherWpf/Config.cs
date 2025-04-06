namespace Launcher
{
    /// <summary>
    /// 登录器配置
    /// </summary>
    //[ConfigPath(@".\LauncherSet.ini")]
    public static class Config
    {
        /// <summary>
        /// 登录器更新地址
        /// </summary>
        [ConfigSection("Patcher")]
        public static string Host { get; set; }
        /// <summary>
        /// 是否保存登录记录
        /// </summary>
        public static bool UseLogin { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public static string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public static string Password { get; set; }

    }
}
