using Library;
using System.Collections.Generic;

namespace Server.Models.Temp
{
    /// <summary>
    /// 道具组
    /// </summary>
    public class ItemGroup
    {
        /// <summary>
        /// 带标签的道具索引
        /// </summary>
        public KeyValuePair<int, UserItemFlags> IndexWithFlags { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public long Count { get; set; }
    }
}
