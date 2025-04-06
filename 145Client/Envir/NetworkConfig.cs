using Library;
using System;

namespace Client.Envir
{
    [ConfigPath(@"Data/Network.ini")]
    /// <summary>
    /// 客户端配置
    /// </summary>
    public static class NetworkConfig
    {
        [ConfigSection("Network")]  //网络配置
        /// <summary>
        /// 使用网络设置
        /// </summary>
        public static bool UseNetworkConfig { get; set; } = true;
        /// <summary>
        /// 客户端区服名字
        /// </summary>
        public static string ClientName { get; set; } = "Legend of Mir 3";
        /// <summary>
        /// 储存一区服名字
        /// </summary>
        public static string Server1Name { get; set; } = "";
        /// <summary>
        /// 储存一区IP地址
        /// </summary>
        public static string IPAddress { get; set; } = "127.0.0.1";
        /// <summary>
        /// 储存一区端口
        /// </summary>
        public static int Port { get; set; } = 7000;
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
    }
}
