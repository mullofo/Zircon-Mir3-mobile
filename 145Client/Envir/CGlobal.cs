using System.Drawing;

namespace Client.Envir
{
    /// <summary>
    /// 特定字体设置
    /// </summary>
    public class CGlobal
    {
        /// <summary>
        /// 字体选择中文
        /// </summary>
        public static Font BlodFont = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold);
        /// <summary>
        /// 字体选择英文
        /// </summary>
        public static Font BloadFontEng = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold);
        /// <summary>
        /// 字体默认颜色
        /// </summary>
        public static Color SysWhite = Color.FromArgb(223, 255, 205);
    }
}
