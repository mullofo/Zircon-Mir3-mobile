using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Font = MonoGame.Extended.Font;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 聊天信息左边透明显示框
    /// </summary>
    public class ChatLDialog : DXWindow
    {
        #region Properties
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        /// <summary>
        /// 信息文字
        /// </summary>
        private Font ChatFont;
        public List<DXLabel> History = new List<DXLabel>();
        #endregion

        /// <summary>
        /// 聊天信息左边透明显示框
        /// </summary>
        public ChatLDialog()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            PassThrough = true;

            Size = new Size(270, 80);
            ChatFont = new Font(Config.FontName, CEnvir.FontSize(9F));
            History = new List<DXLabel>();
        }

        #region method
        private readonly object obj = 1;
        /// <summary>
        /// 显示信息内容
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ReceiveChat(string message, MessageType type)
        {
            switch (type)
            {
                case MessageType.Normal://一般信息
                case MessageType.Shout://区域聊天
                case MessageType.Global://世界喊话
                case MessageType.Group://组队聊天
                case MessageType.GMWhisperIn:     //GM私聊  
                case MessageType.WhisperIn://私聊 收到私聊
                case MessageType.WhisperOut:
                case MessageType.ObserverChat://观察者聊天
                case MessageType.Guild://行会聊天
                case MessageType.System:
                case MessageType.Notice:
                    return;
            }

            //超过宽度分割文本
            int lineWidth = Size.Width;
            List<string> list = new List<string>();
            int width = 0;
            for (int i = 1; i < message.Length; i++)
            {
                Size si = TextRenderer.MeasureText(DXManager.Graphics, message.Substring(width, i - width), ChatFont);
                if (si.Width > lineWidth)
                {
                    list.Add(message.Substring(width, i - width - 1));
                    width = i - 1;
                }
            }
            list.Add(message.Substring(width, message.Length - width));

            for (int j = 0; j < list.Count; j++)
            {
                message = list[j];
                var label = new DXLabel
                {
                    Tag = type,
                    Tag_0 = CEnvir.Now,
                    Parent = this,
                    PassThrough = true,
                    IsControl = false,
                    ForeColour = Color.White,
                    Font = ChatFont,
                };
                label.Text = message ?? "";
                lock (obj)
                {
                    UpdateItems(label.Size.Height);
                    label.Location = new Point(0, Size.Height - label.Size.Height - 2);

                    History.Add(label);
                    Visible = true;
                }
            }

        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            Visible = !GameScene.Game.ChatBox.Visible && History.Count(p => p.Visible) != 0;

            if (History.Count > 0)
            {
                lock (obj)
                {
                    var old = History.FirstOrDefault(p => p.Visible);
                    if (old != null && !old.IsDisposed && (CEnvir.Now - (DateTime)old.Tag_0).Seconds > 15)  //信息停留时间
                    {
                        old.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="height"></param>
        private void UpdateItems(int height)
        {
            while (History.Count > 3)
            {
                var oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }
            foreach (var item in History)
            {
                item.Location = new Point(item.Location.X, item.Location.Y - height - 2);
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (History != null)
                {
                    for (int i = 0; i < History.Count; i++)
                    {
                        if (History[i] != null)
                        {
                            if (!History[i].IsDisposed)
                                History[i].Dispose();

                            History[i] = null;
                        }
                    }

                    History.Clear();
                    History = null;
                }
            }
        }
        #endregion
    }
}
