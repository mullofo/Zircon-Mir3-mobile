using Library;
using System.Drawing;

namespace Client.Models
{
    /// <summary>
    /// 目标动作
    /// </summary>
    public sealed class ObjectAction
    {
        /// <summary>
        /// 动作
        /// </summary>
        public MirAction Action;
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction;
        /// <summary>
        /// 位置
        /// </summary>
        public Point Location;
        /// <summary>
        /// 额外的
        /// </summary>
        public object[] Extra;

        /// <summary>
        /// 目标动作
        /// </summary>
        public ObjectAction(MirAction action, MirDirection direction, Point location, params object[] extra)
        {
            Action = action;
            Direction = direction;
            Location = location;
            Extra = extra;
        }
    }
}
