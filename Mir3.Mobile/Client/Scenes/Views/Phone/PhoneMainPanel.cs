using Client.Controls;
using Library;
using System.Drawing;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// UI底部主面板
    /// </summary>
    public sealed class PhoneMainPanel : DXControl
    {
        #region Properties

        public DXImageControl ChatBackImag;
        //private DXButton SystemButtion, SpeakButtion;
        private DXButton MagicButton;
        private DXButton MarketPlace;
        private DXButton ChatButton;
        #endregion

        /// <summary>
        /// 底部UI界面左边框
        /// </summary>
        public PhoneMainPanel()
        {
            Size = new Size(350, 100);
            PassThrough = true;  //穿透开启

            //ChatBackImag = new DXImageControl
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 10,
            //    Visible = true,
            //};
            //ChatBackImag.Location = new Point(Size.Width - ChatBackImag.Size.Width, 0);

            //SystemButtion = new DXButton
            //{
            //    Parent = ChatBackImag,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 11,
            //    Location = new Point(5, 8),
            //};
            //SystemButtion.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.ChatTextBox.ChatModeButton.InvokeMouseClick();
            //};

            //SpeakButtion = new DXButton
            //{
            //    Parent = ChatBackImag,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 12,
            //    Location = new Point(5, 65),
            //};
            //SpeakButtion.TouchUp += (o, e) =>
            //{
            //    if (!GameScene.Game.ChatTextBox.Visible)
            //    {
            //        GameScene.Game.ChatTextBox.Visible = true;
            //        GameScene.Game.ChatTextBox.OpenChat();
            //        FocusControl = GameScene.Game.ChatTextBox.TextBox;
            //    }
            //    else
            //    {
            //        GameScene.Game.ChatTextBox.SendMessage();
            //        GameScene.Game.ChatTextBox.Visible = false;
            //    }
            //};

            MarketPlace = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 151,
            };
            MarketPlace.Location = new Point(0, 0);
            MarketPlace.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.MarketPlaceBox.Visible = !GameScene.Game.MarketPlaceBox.Visible;
            };

            MagicButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 150,
            };
            MagicButton.Location = new Point(MarketPlace.Location.X, MarketPlace.Location.Y + MagicButton.Size.Height + 5);
            MagicButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;
            };

            ChatButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 150,
            };
            ChatButton.Location = new Point(Size.Width - ChatButton.Size.Width, MagicButton.Location.Y);
            ChatButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.ChatBox.Visible = !GameScene.Game.ChatBox.Visible;
            };

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                //if (SystemButtion != null)
                //{
                //    if (!SystemButtion.IsDisposed)
                //        SystemButtion.Dispose();

                //    SystemButtion = null;
                //}

                //if (SpeakButtion != null)
                //{
                //    if (!SpeakButtion.IsDisposed)
                //        SpeakButtion.Dispose();

                //    SpeakButtion = null;
                //}

                if (MagicButton != null)
                {
                    if (!MagicButton.IsDisposed)
                        MagicButton.Dispose();

                    MagicButton = null;
                }

                if (MarketPlace != null)
                {
                    if (!MarketPlace.IsDisposed)
                        MarketPlace.Dispose();

                    MarketPlace = null;
                }

                if (ChatBackImag != null)
                {
                    if (!ChatBackImag.IsDisposed)
                        ChatBackImag.Dispose();

                    ChatBackImag = null;
                }
            }
        }
        #endregion

    }
}
