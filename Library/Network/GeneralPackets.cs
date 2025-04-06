namespace Library.Network.GeneralPackets
{
    //通用数据包
    /// <summary>
    /// 关联的数据包
    /// </summary>
    public sealed class Connected : Packet { }
    /// <summary>
    /// PING
    /// </summary>
    public sealed class Ping : Packet { }
    /// <summary>
    /// 检查版本
    /// </summary>
    public sealed class CheckVersion : Packet { }
    /// <summary>
    /// 版本
    /// </summary>
    public sealed class Version : Packet
    {
        public Platform Platform { get; set; }
        public byte[] ClientHash { get; set; }
        public byte[] ClientSystemDBHash { get; set; }
        public string ClientMACInfo { get; set; }
        public string ClientCPUInfo { get; set; }
        public string ClientHDDInfo { get; set; }
    }
    /// <summary>
    /// 完美版本
    /// </summary>
    public sealed class GoodVersion : Packet
    {
        //数据库的密码发给客户端
        public bool DBEncrypted { get; set; }
        public string DBPassword { get; set; }
        //区服列表
        public string Server1Name { get; set; }
        public int PlayCount { get; set; }
    }
    /// <summary>
    /// PING响应
    /// </summary>
    public sealed class PingResponse : Packet
    {
        public int Ping { get; set; }
    }
    /// <summary>
    /// 断开数据包
    /// </summary>
    public sealed class Disconnect : Packet
    {
        public DisconnectReason Reason { get; set; }
    }
}
