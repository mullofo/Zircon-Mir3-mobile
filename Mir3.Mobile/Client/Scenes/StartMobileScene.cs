using Client.Controls;
using Client.Envir;
using Library;
using MonoGame.Extended;
using Patch;
using System;
using System.Drawing;

namespace Client.Scenes
{
    /// <summary>
    /// 启动游戏
    /// </summary>
    public class StartMobileScene : DXScene
    {
        DXLabel Title, PercentLabel;
        DXImageControl Background, PercentBack, Percent, TitleBack;

        //DXAnimatedControl DXAnimated;

        Update _update;
        /// <summary>
        /// 启动游戏窗口
        /// </summary>
        /// <param name="size">视窗大小</param>
        public StartMobileScene(Size size) : base(size)
        {
            IsControl = false;


            //var offset = Game1.Native.IsPad ? 1 : 0;
            var offset = CEnvir.Random.Next(0, 2);
            Background = new DXImageControl
            {
                Index = 47 + offset,
                LibraryFile = LibraryFile.StartMobileScene,
                IsControl = false,
                Parent = this
            };
            Background.Location = new Point((size.Width - Background.Size.Width) / 2, (size.Height - Background.Size.Height) / 2);
            PercentBack = new DXImageControl
            {
                Index = 52,
                LibraryFile = LibraryFile.StartMobileScene,
                Parent = this,
                TilingMode = TilingMode.Horizontally,
                FixedSize = true,
                Size = new Size(size.Width, 18),
                Location = new Point(0, size.Height - 18),
                IsControl = false,
            };

            Percent = new DXImageControl
            {
                Index = 54,
                LibraryFile = LibraryFile.StartMobileScene,
                Parent = this,
                TilingMode = TilingMode.Horizontally,
                FixedSize = true,
                Size = new Size(0, 18),
                Location = new Point(0, size.Height - 18),
                IsControl = false,
            };

            TitleBack = new DXImageControl
            {
                Index = 56,
                LibraryFile = LibraryFile.StartMobileScene,
                Parent = this,
                IsControl = false,
            };
            TitleBack.Location = new Point((size.Width - TitleBack.Size.Width) / 2, size.Height - Percent.Size.Height - TitleBack.Size.Height - 10);

            Title = new DXLabel
            {
                Text = "",
                ForeColour = Color.White,
                Font = new MonoGame.Extended.Font(Config.FontName, 12),
                Parent = this,
                AutoSize = false,
                Size = new Size(300, TitleBack.Size.Height),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
            };
            Title.Location = new Point((size.Width - Title.Size.Width) / 2, TitleBack.Location.Y - 2);

            PercentLabel = new DXLabel
            {
                Text = "进度 00.00%",
                ForeColour = Color.White,
                //Font = new MonoGame.Extended.Font(Config.FontName, 12),
                Parent = this,
                //AutoSize = false,
                Size = TitleBack.Size,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
            };
            PercentLabel.Location = new Point((Size.Width - PercentLabel.Size.Width) / 2, Size.Height - PercentLabel.Size.Height - 2);

            //DXAnimated = new DXAnimatedControl
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.StartMobileScene,
            //    BaseIndex = 1,
            //    Loop = false,
            //    FrameCount = 45,
            //    Animated = true,
            //    Location = Background.Location,
            //    AnimationDelay = TimeSpan.FromSeconds(8)
            //};
            //DXAnimated.AfterAnimation += DXAnimated_AfterAnimation;
            //DXAnimated.MouseClick += DXAnimated_MouseClick;
            //DXSoundManager.Play(SoundIndex.Mobile);
            DXAnimated_AfterAnimation();
        }

        //private void DXAnimated_MouseClick(object sender, MonoGame.Extended.Input.MouseEventArgs e)
        //{
        //    DXAnimated.Animated = false;
        //    DXAnimated_AfterAnimation(sender, e);
        //}


        #region Methods
        DateTime? _now;
        bool IsComplate;
        private void DXAnimated_AfterAnimation(/*object sender, EventArgs e*/)
        {
            //DXAnimated.Visible = false;
            //DXSoundManager.Stop(SoundIndex.Mobile);
            var vm = new PatchViewModel()
            {
                MaxWidth = Size.Width
            };
            vm.TotalUpdated = (per) =>
            {
                //if (vm.IsComplate && !IsComplate)
                //{
                //    IsComplate = true;
                //    _now = CEnvir.Now.AddSeconds(1);
                //}
                Percent.Size = new Size(per, Percent.Size.Height);
                Title.Text = vm.LoadText;
                PercentLabel.Text = $"进度 {vm.TotalPercent}%";
            };
            //vm.CurrentUpdated = (per) =>
            //{
            //    Debug.WriteLine("c:" + per);
            //};
            _update = new Update(vm);
            _ = _update.CheckPatchAsync(false);
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (_update == null) return;

            if (_update.AllCompleted && !IsComplate)
            {
                IsComplate = true;
                _now = CEnvir.Now.AddSeconds(1);
            }

            if (IsComplate && CEnvir.Now > _now)
            {
                _now = DateTime.MaxValue;

                foreach (var library in Libraries.LibraryList)
                {
                    if (library.Key == LibraryFile.StartMobileScene || library.Key == LibraryFile.Interface) continue;
                    CEnvir.LibraryList[library.Key] = new MirLibrary(library.Value);
                }

                NextSecene();
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
            Title.TryDispose();
            PercentBack.TryDispose();
            Percent.TryDispose();
            //DXAnimated.TryDispose();
        }
    }
}
