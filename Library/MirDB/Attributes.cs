using System;

namespace MirDB
{
    /// <summary>
    /// 会话模式
    /// </summary>
    [Flags]
    public enum SessionMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 客户端
        /// </summary>
        Client = 1,
        /// <summary>
        /// 服务端
        /// </summary>
        Server = 2,
        /// <summary>
        /// ST工具
        /// </summary>
        ServerTool = 4,
    }
    [Flags]
    public enum SessionScope
    {
        None = 0x0,
        EntryAssembly = 0x1,
        ExecutingAssembly = 0x2,
        CallingAssembly = 0x4,
        All = 0x7
    }
    /// <summary>
    /// 用户对象属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UserObject : Attribute { }
    /// <summary>
    /// 客户端对象属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientObject : Attribute { }
    /// <summary>
    /// 服务端对象属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ServerOnly : Attribute { }
    /// <summary>
    /// 忽略属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreProperty : Attribute { }
    /// <summary>
    /// 管理属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Association : Attribute
    {
        /// <summary>
        /// 特征
        /// </summary>
        public string Identity { get; }
        /// <summary>
        /// 主表删除时，关联的子表属性是否一起删除
        /// </summary>
        public bool Aggregate { get; }
        /// <summary>
        /// 关联特征
        /// </summary>
        /// <param name="identity"></param>
        public Association(string identity)
        {
            Identity = identity;
        }
        /// <summary>
        /// 关联综合数据
        /// </summary>
        /// <param name="aggregate"></param>
        public Association(bool aggregate)
        {
            Aggregate = aggregate;
        }
        /// <summary>
        /// 关联特征和综合数据
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="aggregate"></param>
        public Association(string identity, bool aggregate)
        {
            Identity = identity;
            Aggregate = aggregate;
        }

        /// <summary>
        /// 导出excel显示名
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class ExportAttribute : Attribute
        {

        }

        /// <summary>
        /// 国际化
        /// </summary>
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum)]
        public class LangAttribute : Attribute
        {
            public string Descrption { get; private set; }
            public LangAttribute(string discription = null)
            {
                Descrption = discription;
            }
        }
    }
}
