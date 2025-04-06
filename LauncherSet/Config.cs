namespace LauncherSet
{
    /// <summary>
    /// 登录器配置
    /// </summary>
    [ConfigPath(@".\LauncherSet.ini")]
    public static class Config
    {
        /// <summary>
        /// 登录器更新地址
        /// </summary>
        [ConfigSection("Patcher")]
        public static string Host { get; set; } = $"http://IP地址/Client/Patch";
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
        /// <summary>
        /// 标题
        /// </summary>
        [ConfigSection("LauncherSet")]
        public static string Title { get; set; } = $"传奇3";
        /// <summary>
        /// 背景图片
        /// </summary>
        public static string BackImage { get; set; }
        /// <summary>
        /// 背景链接地址
        /// </summary>
        public static string WebAddress { get; set; } = $"";
        /// <summary>
        /// 网址
        /// </summary>
        public static string PatchRemark { get; set; } = $"http://www.lomcn.cn";
        /// <summary>
        /// 备注
        /// </summary>
        public static string PatchAdress { get; set; } = $"网址";
    }
}
