using Library;
using System.Drawing;

namespace Client.Envir
{
    [ConfigPath(@".\Data\Diy\Mir3.dat")]
    /// <summary>
    /// 客户端配置
    /// </summary>
    public static class ClientDetails
    {
        /// <summary>
        /// 道具地面颜色其他类
        /// </summary>
        [ConfigSection("Client")]
        public static Color ItemNameGroundColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色装备类
        /// </summary>
        public static Color ItemNameGroundEquipColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色材料类
        /// </summary>
        public static Color ItemNameGroundNothingColour { get; set; } = Color.FromArgb(0, 255, 50);
        /// <summary>
        /// 道具地面颜色消耗品类
        /// </summary>
        public static Color ItemNameGroundConsumableColour { get; set; } = Color.FromArgb(255, 255, 125);
        /// <summary>
        /// 道具地面颜色普通极品类
        /// </summary>
        public static Color ItemNameGroundCommonColour { get; set; } = Color.MediumSeaGreen;
        /// <summary>
        /// 道具地面颜色高级类
        /// </summary>
        public static Color ItemNameGroundSuperiorColour { get; set; } = Color.Orange;
        /// <summary>
        /// 道具地面颜色稀世类
        /// </summary>
        public static Color ItemNameGroundEliteColour { get; set; } = Color.MediumPurple;
        /// <summary>
        /// NPC名字颜色
        /// </summary>
        public static Color NPCNameColour { get; set; } = Color.White;
        /// <summary>
        /// 怪物血条颜色
        /// </summary>
        public static Color MonHealthColour { get; set; } = Color.FromArgb(248, 33, 32);
        /// <summary>
        /// 装备等级特性标签显示
        /// </summary>
        public static bool ItemLevelLabel { get; set; } = false;
        /// <summary>
        /// 攻击速度数值显示
        /// </summary>
        public static bool AttackSpeedValue { get; set; } = false;
        /// <summary>
        /// 舒适数值显示
        /// </summary>
        public static bool ComfortValue { get; set; } = false;
    }
}
