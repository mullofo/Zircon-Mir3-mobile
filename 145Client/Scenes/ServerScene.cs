using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.Scenes
{
    /// <summary>
    /// 选区界面
    /// </summary>
    public sealed class ServerScene : DXScene
    {
        public DXImageControl BackGround;
        public DXControl Content;
        public ServersDialog ServersBox;
        /// <summary>
        /// 选区界面
        /// </summary>
        /// <param name="size">视窗大小</param>
        public ServerScene(Size size) : base(size)
        {
            Content = new DXControl()   //内容
            {
                Size = size,
                Parent = this,
            };

            BackGround = new DXImageControl        //选区场景背景图
            {
                Index = 7,
                LibraryFile = LibraryFile.Interface1c145,
                Parent = Content,
                Location = new Point(0, 60),
            };

            ServersBox = new ServersDialog                //服务器内容
            {
                Parent = Content
            };
            //选区窗体坐标
            ServersBox.Location = new Point(25, 60);

        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Content.TryDispose();
                ServersBox.TryDispose();
                BackGround.TryDispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// 服务器列表界面
    /// </summary>
    public sealed class ServersDialog : DXWindow
    {
        public List<SelectInfo> CharacterList { get; set; }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;

        public DXLabel Server1, Explain;
        public DXControl Panel;
        /// <summary>
        /// 服务器列表界面
        /// </summary>
        public ServersDialog()
        {
            Size = new Size(640, 480);
            Opacity = 0F;
            TitleLabel.Visible = false;
            AllowResize = false;
            CloseButton.Visible = false;
            Movable = false;
            Panel = new DXControl
            {
                Parent = this,
                Border = true,
                DrawTexture = true,
                Opacity = 0.8F,
                Location = new Point(5, 5),
                BackColour = Color.FromArgb(147, 154, 150),  //背景色
                BorderColour = Color.FromArgb(140, 140, 140),  //边框颜色
                Visible = true
            };

            Server1 = new DXLabel
            {
                Parent = this,
                Location = new Point(5, 12),
                Text = string.IsNullOrEmpty(NetworkConfig.Server1Name) ? NetworkConfig.ClientName : NetworkConfig.Server1Name,
                ForeColour = Color.FromArgb(0, 0, 255),
                Outline = false,
                //DrawTexture = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
            Server1.MouseClick += Login;
            Server1.MouseEnter += (o, e) =>  //鼠标进入
            {
                Server1.BackColour = Color.FromArgb(150, 175, 175);  //背景色
            };
            Server1.MouseLeave += (o, e) =>  //鼠标离开
            {
                Server1.BackColour = Color.Empty;
            };

            Panel.Size = new Size(10 + Server1.Size.Width, 50);
            Server1.Size = new Size(10 + Server1.Size.Width, 35);

            Explain = new DXLabel
            {
                Parent = this,
                Location = new Point(35, 380),
                Text = "ServerScene.Explain".Lang(),
                ForeColour = Color.Yellow,
                Outline = true,
                Size = new Size(550, 50),
            };

            KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    DXSoundManager.Play(SoundIndex.ButtonA);
                    Login(s, null);
                    e.Handled = true;
                }
            };
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, MouseEventArgs e)  //回车键直接进入
        {
            DXSoundManager.StopAllSounds();
            DXSoundManager.Play(SoundIndex.SelChrBgm145);
            //TODO,服务器列表值的正确处理
            SelectScene scene;
            ActiveScene = scene = new SelectScene(Config.IntroSceneSize)
            {
                SelectBox =
                {
                    CharacterList = CharacterList
                }
            };
            scene.SelectBox.UpdateCharacters();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Server1.TryDispose();
                Explain.TryDispose();
                Panel.TryDispose();
            }
        }
    }
}
