using Library;
using Library.SystemModels;
using System.Collections.Generic;

namespace Server.Models
{
    /// <summary>
    /// 副本地图
    /// </summary>
    public sealed class FubenMap : Map
    {
        /// <summary>
        /// 当前玩家
        /// </summary>
        public PlayerObject CurrentPlayer { get; }
        /// <summary>
        /// 组队成员
        /// </summary>
        public List<PlayerObject> GroupMembers;
        /// <summary>
        /// 地图类型
        /// </summary>
        public MapType mapType { get; }
        /// <summary>
        /// 副本地图
        /// </summary>
        /// <param name="info">地图信息</param>
        public FubenMap(MapInfo info) : base(info)
        {

        }

    }
}
