using System;

namespace Server.Models
{
    /// <summary>
    /// 延迟动作
    /// </summary>
    public sealed class DelayedAction
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// 动作类型
        /// </summary>
        public ActionType Type;
        /// <summary>
        /// 对象数据
        /// </summary>
        public object[] Data;
        /// <summary>
        /// 延迟动作
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="type">动作类型</param>
        /// <param name="data">对象数据</param>
        public DelayedAction(DateTime time, ActionType type, params object[] data)
        {
            Time = time;
            Type = type;
            Data = data;
        }

    }
    /// <summary>
    /// 动作类型
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// 旋转
        /// </summary>
        Turn,
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 骑马
        /// </summary>
        Mount,
        /// <summary>
        /// 割肉
        /// </summary>
        Harvest,
        /// <summary>
        /// 攻击
        /// </summary>
        Attack,
        /// <summary>
        /// 魔法攻击
        /// </summary>
        Magic,
        /// <summary>
        /// 范围攻击
        /// </summary>
        RangeAttack,
        /// <summary>
        /// 延迟攻击
        /// </summary>
        DelayAttack,
        /// <summary>
        /// 延迟魔法攻击
        /// </summary>
        DelayMagic,
        /// <summary>
        /// 广播发包
        /// </summary>
        BroadCastPacket,
        /// <summary>
        /// 刷怪
        /// </summary>
        Spawn,
        /// <summary>
        /// 功能
        /// </summary>
        Function,
        /// <summary>
        /// 延迟攻击伤害
        /// </summary>
        DelayedAttackDamage,
        /// <summary>
        /// 延迟魔法攻击伤害
        /// </summary>
        DelayedMagicDamage,
        /// <summary>
        /// 采矿
        /// </summary>
        Mining,
        /// <summary>
        /// 延迟采矿
        /// </summary>
        DelayMining,
        /// <summary>
        /// 攻击延迟
        /// </summary>
        AttackDelay,
        /// <summary>
        /// NPC
        /// </summary>
        NPC
    }
}
