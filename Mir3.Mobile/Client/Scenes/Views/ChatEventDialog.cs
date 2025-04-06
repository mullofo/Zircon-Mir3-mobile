using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    public class ChatEventDialog : DXWindow
    {
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);
            if (!string.IsNullOrEmpty(nValue))
            {
                Content.Text = nValue;
                if (!Visible)
                {
                    Visible = true;
                }

#if Mobile
#else
                if (GameScene.Game?.TopInfoBox != null)
                {
                    GameScene.Game.TopInfoBox.Visible = false;
                }
#endif
            }
        }

        public void Show(S.Chat chat)
        {
            Text = chat.Text;
            Icon.Index = chat.NpcFace;
            Visible = true;
            _showTime = CEnvir.Now.AddSeconds(5);
        }
        DateTime? _showTime;
        DXImageControl Icon;
        DXImageControl ContentBg;
        DXLabel Content;

        public ChatEventDialog()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            PassThrough = true;

            ContentBg = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Parent = this,
                Index = 2000,
            };
            Icon = new DXImageControl
            {
                LibraryFile = LibraryFile.NPCface,
                Parent = ContentBg,
                Index = 4,
            };
            Size = ContentBg.Size;
            var w = Icon.Size.Width + 2;
            Content = new DXLabel()
            {
                AutoSize = false,
                Size = ContentBg.Size - new Size(w, 0),
                ForeColour = Color.White,
                Parent = ContentBg,
                Location = new Point(w, 0)
            };
        }

        public override void Process()
        {
            base.Process();
            if (Visible && _showTime != null && CEnvir.Now > _showTime)
            {
                Visible = false;
                _showTime = null;
#if Mobile
#else
                if (GameScene.Game?.TopInfoBox != null)
                {
                    GameScene.Game.TopInfoBox.Visible = true;
                }
#endif
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Icon.TryDispose();
                ContentBg.TryDispose();
                Content.TryDispose();
            }
        }
    }
}
