using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Library;
using System;
using System.Drawing;
using System.Threading.Tasks;
using C = Library.Network.ClientPackets;

namespace Client.Scenes
{
    public class LoadScene : DXScene
    {
        public int CharacterIndex { get; set; }
        public bool StartGameAttempted { get; set; }
        public DXConServerBox ServerBox { get; private set; }

        public DXStartGameBox StartGameBox { get; private set; }
        public LoadScene(Size size) : base(size)
        {
            ServerBox = new DXConServerBox("LoadScene.ServerBox".Lang())
            {
                Parent = this,
                Visible = true,
            };
            ServerBox.Location = new Point((Size.Width - ServerBox.Size.Width) / 2, (Size.Height - ServerBox.Size.Height) / 2);
            //ServerBox显示时间1秒
            DateTime expiry = CEnvir.Now + TimeSpan.FromMilliseconds(500);
            ServerBox.ProcessAction = () =>
            {
                if (CEnvir.Now > expiry)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(100);
                        CEnvir.Target.BeginInvoke(new Action(CreateGame));
                    });

                    ServerBox.ProcessAction = null;
                }
            };

            StartGameBox = new DXStartGameBox("LoadScene.StartGameBox".Lang())
            {
                Parent = this,
                Visible = false,
            };
            StartGameBox.Location = new Point((Size.Width - StartGameBox.Size.Width) / 2, (Size.Height - StartGameBox.Size.Height) / 2);
            StartGameBox.Button.MouseClick += (o, e) =>   //鼠标点击
            {
                StartGame();
            };
            StartGameBox.Button.KeyPress += (s, e) =>   //回车
            {
                StartGameBox.Button.Index = 1252;
                StartGame();
            };
        }

        void CreateGame()
        {
            if (GameScene.Game != null)
            {
                return;
            }

            GameScene scene = new GameScene(Config.GameSize);
            scene.IsVisible = false;

            scene.Loaded = true;

            ServerBox.Visible = false;
            StartGameBox.Visible = true;
        }

        void StartGame()
        {
            if (StartGameAttempted) return;
            if (GameScene.Game == null || !GameScene.Game.Loaded) return;

            StartGameAttempted = true;

            CEnvir.Enqueue(new C.StartGame
            {
                Platform = Platform.Desktop,
                CharacterIndex = CharacterIndex,
                ClientMACInfo = CEnvir.MacInfo,
                ClientCPUInfo = CEnvir.CpuInfo,
                ClientHDDInfo = CEnvir.HDDnfo,
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                ServerBox.TryDispose();
                StartGameBox.TryDispose();
            }
        }
    }
}
