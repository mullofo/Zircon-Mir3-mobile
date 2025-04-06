using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

            Size = new Size(GameScene.Game.ChatBox.Size.Width - 10, 70);
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
            var label = new DXLabel
            {
                Tag = CEnvir.Now,
                Parent = this,
                PassThrough = true,
            };
            var prefix = "";
            switch (type)     //只显示 普通 区域 组队 私聊 收到私聊 行会  观察者聊天
            {
                case MessageType.Normal:
                    if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Config.LocalTextColour;
                    break;
                case MessageType.Shout:
                    if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Color.FromArgb(255, 255, 255, 80);
                    break;
                case MessageType.Group:
                    if (!(GameScene.Game.ChatBox.IsGroup || GameScene.Game.ChatBox.IsAll)) return;
                    label.ForeColour = Config.GroupTextColour;
                    break;
                case MessageType.WhisperIn:
                    if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Config.WhisperInTextColour;
                    break;
                case MessageType.WhisperOut:
                    if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Config.WhisperOutTextColour;
                    break;
                case MessageType.Guild:
                    if (!(GameScene.Game.ChatBox.IsGuild || GameScene.Game.ChatBox.IsAll)) return;
                    label.ForeColour = Config.GuildTextColour;
                    break;
                case MessageType.ObserverChat:
                    if (!GameScene.Game.ChatBox.IsAll) return;
                    label.ForeColour = Config.ObserverTextColour;
                    break;
                default:
                    return;
            }
            label.Tag = CEnvir.Now;
            label.Text = prefix + (message ?? "");
            Size size = DXLabel.GetHeight(label, Size.Width - 40);
            label.Size = new Size(size.Width, size.Height);
            lock (obj)
            {
                UpdateItems(label.Size.Height);
                label.Location = new Point(0, Size.Height - label.Size.Height - 2);

                History.Add(label);
                Visible = true;
            }

            //解析聊天框接收的道具信息（客户端解析服务端返回的 <ItemName/Index> 的组合）
            string currentLine = label.Text;
            Capture capture = null;
            foreach (Match match in Globals.RegexChatItemName.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList())
            {
                try
                {
                    //拆分 <ItemName/Index> 的组合
                    capture = match.Groups[1].Captures[0];
                    string[] values = capture.Value.Split('/');
                    currentLine = currentLine.Remove(capture.Index - 1, capture.Length + 2).Insert(capture.Index - 1, values[0]);
                    string text = currentLine.Substring(0, capture.Index - 1) + " ";
                    Size size1 = TextRenderer.MeasureText(DXManager.Graphics, text, label.Font, label.Size, TextFormatFlags.TextBoxControl);
                }
                catch
                {
                }
            }
            label.Text = currentLine;
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
                    if (old != null && !old.IsDisposed && (CEnvir.Now - (DateTime)old.Tag).Seconds > 7)  //信息停留时间
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
