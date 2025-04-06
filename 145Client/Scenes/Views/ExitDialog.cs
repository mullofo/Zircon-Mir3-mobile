using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 退出对话框
    /// </summary>
    public sealed class ExitDialog : DXWindow
    {
        #region Properties
        public DXButton ToSelectButton, ExitButton;

        public override WindowType Type => WindowType.ExitBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 退出对话框
        /// </summary>
        public ExitDialog()
        {
            TitleLabel.Text = @"退出游戏".Lang();

            SetClientSize(new Size(200, 50 + DefaultHeight + DefaultHeight));
            ToSelectButton = new DXButton
            {
                Location = new Point(ClientArea.X + 35, ClientArea.Y + 20),
                Size = new Size(130, DefaultHeight),
                Parent = this,
                Label = { Text = "选择角色".Lang() },
            };
            ToSelectButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && !BigPatchConfig.ChkQuickSelect)
                {
                    GameScene.Game.ReceiveChat("战斗中无法退出游戏".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.Logout());
            };

            ExitButton = new DXButton
            {
                Location = new Point(ClientArea.X + 35, ClientArea.Y + 30 + DefaultHeight),
                Size = new Size(130, DefaultHeight),
                Parent = this,
                Label = { Text = "退出游戏".Lang() },
            };
            ExitButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && !BigPatchConfig.ChkQuickSelect)
                {
                    GameScene.Game.ReceiveChat("战斗中无法退出游戏".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Target.Close();
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ToSelectButton != null)
                {
                    if (!ToSelectButton.IsDisposed)
                        ToSelectButton.Dispose();

                    ToSelectButton = null;
                }

                if (ExitButton != null)
                {
                    if (!ExitButton.IsDisposed)
                        ExitButton.Dispose();

                    ExitButton = null;
                }
            }
        }
        #endregion
    }
}
