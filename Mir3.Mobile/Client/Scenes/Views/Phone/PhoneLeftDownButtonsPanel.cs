using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Point = System.Drawing.Point;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 手机UI左边下方按钮区域
    /// </summary>
    public sealed class PhoneLeftDownButtonsPanel : DXControl
    {
        #region Properties

        /// <summary>
        /// 骑马按钮
        /// </summary>
        private DXButton HorseButton;
        /// <summary>
        /// 聊天按钮
        /// </summary>
        private DXButton ChatButton;

        #endregion

        public PhoneLeftDownButtonsPanel()
        {
            DrawTexture = false;
            PassThrough = true;  //穿透开启

            HorseButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 154,
            };
            HorseButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (CEnvir.Now < GameScene.Game.User.NextActionTime || GameScene.Game.User.ActionQueue.Count > 0) return;
                if (CEnvir.Now < GameScene.Game.User.ServerTime) return;
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && GameScene.Game.User.Horse == HorseType.None)
                {
                    GameScene.Game.ReceiveChat("战斗中无法骑马".Lang(), MessageType.System);
                    return;
                }

                GameScene.Game.User.ServerTime = CEnvir.Now.AddSeconds(5);
                CEnvir.Enqueue(new C.Mount());
            };

            ChatButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 149,
            };
            ChatButton.Location = new Point(HorseButton.Location.X, HorseButton.Location.Y + HorseButton.Size.Height + 5);
            ChatButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.ChatBox.Visible = !GameScene.Game.ChatBox.Visible;
            };

            Size = new Size(ChatButton.Size.Width, ChatButton.Location.Y + ChatButton.Size.Height);
        }


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (HorseButton != null)
                {
                    if (!HorseButton.IsDisposed)
                        HorseButton.Dispose();

                    HorseButton = null;
                }

                if (ChatButton != null)
                {
                    if (!ChatButton.IsDisposed)
                        ChatButton.Dispose();

                    ChatButton = null;
                }
            }
        }
        #endregion

    }
}
