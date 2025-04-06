using Library;
using Library.SystemModels;
using Server.DBModels;
using System;

namespace Server.Models
{
    /// <summary>
    /// 道具检查
    /// </summary>
    public class ItemCheck
    {
        /// <summary>
        /// 角色道具
        /// </summary>
        public UserItem Item { get; set; }
        /// <summary>
        /// 道具信息
        /// </summary>
        public ItemInfo Info { get; set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// 道具标签
        /// </summary>
        public UserItemFlags Flags { get; set; }
        /// <summary>
        /// 道具使用时间
        /// </summary>
        public TimeSpan ExpireTime { get; set; }
        /// <summary>
        /// 道具属性
        /// </summary>
        public Stats Stats { get; set; }

        /// <summary>
        /// 道具检查
        /// </summary>
        /// <param name="item">角色道具</param>
        /// <param name="count">数量</param>
        /// <param name="flags">标签</param>
        /// <param name="time">使用时间</param>
        public ItemCheck(UserItem item, long count, UserItemFlags flags, TimeSpan time)
        {
            Item = item;
            Info = item.Info;
            Count = count;
            Flags = flags;

            ExpireTime = time;
            Stats = new Stats(item.Stats);
        }

        /// <summary>
        /// 道具检查
        /// </summary>
        /// <param name="info">道具信息</param>
        /// <param name="count">数量</param>
        /// <param name="flags">标签</param>
        /// <param name="time">使用时间</param>
        public ItemCheck(ItemInfo info, long count, UserItemFlags flags, TimeSpan time)
        {
            Info = info;
            Count = count;
            Flags = flags;

            ExpireTime = time;

            Stats = new Stats();
        }
    }
}