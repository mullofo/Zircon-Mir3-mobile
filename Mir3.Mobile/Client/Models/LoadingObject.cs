using Client.Controls;
using Client.Extentions;
using Library;
using System;
using System.Drawing;

namespace Client.Models
{
    /// <summary>
    /// 加载时的动画
    /// </summary>
    public class LoadingObject
    {
        public TimeSpan Duration;

        public bool Shift;

        public DXAnimatedControl animation;

        /// <summary>
        /// 加载时的动画
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="par"></param>
        public LoadingObject(int duration, DXControl par)
        {
            Duration = TimeSpan.FromSeconds(duration);
            CreateAnimation(par);
        }

        /// <summary>
        /// 创建动画
        /// </summary>
        /// <param name="par"></param>
        public void CreateAnimation(DXControl par)
        {
            if (animation == null)
            {
                animation = new DXAnimatedControl  //角色动画
                {
                    LibraryFile = LibraryFile.GameInter,   //动画调用的素材库
                    Animated = true,   //动画开启
                    FrameCount = 100,  //动画帧数 100帧
                    Parent = par,
                    Visible = false,
                    Location = new Point(0, 0)  //显示坐标
                };
            }
            animation.AnimationDelay = TimeSpan.FromMilliseconds(1000);
            animation.Loop = true;
            animation.AnimationStart = DateTime.MinValue;
            animation.Animated = true;
            animation.BaseIndex = 2230;
            animation.FrameCount = 10;
            animation.Hint = "Models.Animation".Lang();
            animation.BringToFront();
        }
    }
}
