using Client.Controls;
using Client.Envir;
using System;
using System.Drawing;
using Font = MonoGame.Extended.Font;

namespace Client.Models
{
    /// <summary>
    /// 屏幕中间位置显示公告信息框
    /// </summary>
    public class WarningObject
    {
        /// <summary>
        /// 文本信息
        /// </summary>
        public string Text;
        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color ForeColour;
        /// <summary>
        /// 轮廓颜色
        /// </summary>
        public Color OutlineColour;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public TimeSpan Duration;
        /// <summary>
        /// X偏移量
        /// </summary>
        public int OffsetX = 25;
        /// <summary>
        /// Y偏移量
        /// </summary>
        public int OffsetY = 50;
        /// <summary>
        /// 转移
        /// </summary>
        public bool Shift;
        /// <summary>
        /// 标签
        /// </summary>
        public DXLabel Label;

        /// <summary>
        /// 屏幕中间位置显示公告信息框
        /// </summary>
        public WarningObject(string text, Color textColour, DXControl par)
        {
            Text = text;
            ForeColour = textColour;
            StartTime = CEnvir.Now;
            Duration = TimeSpan.FromSeconds(3.0);
            OutlineColour = Color.Black;
            CreateLabel(par);
        }

        /// <summary>
        /// 创建信息标签
        /// </summary>
        /// <param name="par"></param>
        public void CreateLabel(DXControl par)
        {
            if (Label == null)
            {
                Label = new DXLabel
                {
                    Text = Text,
                    Parent = par,
                    ForeColour = ForeColour,
                    Outline = true,
                    OutlineColour = OutlineColour,
                    IsVisible = true,
                    Font = new Font(Config.FontName, 20f),
                    PassThrough = true,
                };
            }
        }
    }
}
