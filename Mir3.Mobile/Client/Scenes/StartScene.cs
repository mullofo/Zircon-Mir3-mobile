using Client.Controls;
using Client.Envir;
using Library;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;

namespace Client.Scenes
{
    /// <summary>
    /// 启动游戏
    /// </summary>
    public class StartScene : DXScene
    {
        /// <summary>
        /// LOGO动画
        /// </summary>
        DXAnimatedControl LogoVideo;
        /// <summary>
        /// 官方开场背景图
        /// </summary>
        public DXImageControl StartLogo;
        /// <summary>
        /// 渐变延迟时间
        /// </summary>
        public DateTime LoopTime;
        /// <summary>
        /// 启动游戏窗口
        /// </summary>
        /// <param name="size">视窗大小</param>
        public StartScene(Size size) : base(size)
        {
            //每个场景实例化之前都要设置下偏移
            //Location = Functions.ScalePointXOffset(new Point(0, 0), CEnvir.UIScale, (int)Math.Round(CEnvir.UI_Offset_X / ZoomRate));

            LogoVideo = new DXAnimatedControl   //LOGO动画
            {
                BaseIndex = 200,
                LibraryFile = LibraryFile.Wemade,
                Animated = true,
                AnimationDelay = TimeSpan.FromSeconds(5),
                FrameCount = 150,
                Parent = this,
                Loop = false,
                UseOffSet = true,
            };
            Size s = LogoVideo.Library.GetSize(200);
            LogoVideo.Location = new Point((Config.IntroSceneSize.Width - s.Width) / 2, (Config.IntroSceneSize.Height - s.Height) / 2);
            LogoVideo.AfterAnimation += (s, e) =>  //绘图后
            {
                LogoVideo.Visible = false;
                StartLogo.Visible = true;
                LoopTime = Time.Now;
            };

            StartLogo = new DXImageControl        //官方开场背景图
            {
                Index = 7,
                LibraryFile = LibraryFile.Interface1c,
                Parent = this,
                IsControl = true,
                Visible = false,
            };

            KeyPress += (s, e) =>   //回车ESC跳过动画
            {
                if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Escape)   //回车执行事件
                {
                    NextSecene();
                }
            };

            DXSoundManager.Play(SoundIndex.Wemade145);
        }

        #region Methods
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            TimeSpan loopTime = Time.Now - LoopTime;

            //官方开场背景图渐变
            if (StartLogo.IsVisible)
            {
                if (loopTime.TotalMilliseconds < 100)
                {
                    //小于1500，一直显示
                    return;
                }
                else if (loopTime.TotalMilliseconds >= 1500 && loopTime.TotalMilliseconds < 3500)
                {
                    //大于1500，开始渐变
                    int fRate = 255 - ((int)(loopTime.TotalMilliseconds - 1500) * 255) / 2000;
                    StartLogo.ForeColour = Color.FromArgb(255, fRate, fRate, fRate);
                    return;
                }
                else
                {
                    //结束开场
                    StartLogo.Visible = false;

                    NextSecene();
                }
            }
        }

        /// <summary>
        /// 下一个场景选区场景
        /// </summary>
        void NextSecene()
        {
            CEnvir.UpdateScale(true);
            ActiveScene = new LoginScene(CEnvir.GameSize);

            Dispose();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            LogoVideo.TryDispose();
            StartLogo.TryDispose();
        }
    }
}
