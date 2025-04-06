using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Font = MonoGame.Extended.Font;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 右边聊天信息显示框
    /// </summary>
    public class ChatRDialog : DXWindow
    {
        #region Properties
        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => false;
        /// <summary>
        /// 信息文字
        /// </summary>
        private Font ChatFont;
        public List<DXControl> History = new List<DXControl>();
        #endregion

        /// <summary>
        /// 右边聊天信息显示框
        /// </summary>
        public ChatRDialog()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            PassThrough = true;
            Size = new Size(320, 80);
            ChatFont = new Font(Config.FontName, CEnvir.FontSize(9F));
            History = new List<DXControl>();

            headsize = TextRenderer.MeasureText(DXManager.Graphics, "【GM私聊】", ChatFont);
        }

        #region method
        private readonly object obj = 1;
        private Size headsize;
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
                case MessageType.GMWhisperIn:     //GM私聊  
                case MessageType.WhisperIn://私聊
                case MessageType.WhisperOut://私聊
                case MessageType.ObserverChat://观察者聊天
                case MessageType.Shout://区域聊天
                case MessageType.System:
                case MessageType.Notice:
                    if (!(GameScene.Game.ChatBox.IsNormal || GameScene.Game.ChatBox.IsAll)) return;  //区域聊天 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Global://世界喊话
                    if (!GameScene.Game.ChatBox.IsMentor) return;    //世界喊话 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Group://组队聊天
                    if (!(GameScene.Game.ChatBox.IsGroup || GameScene.Game.ChatBox.IsNormal || GameScene.Game.ChatBox.IsAll)) return;    //组队聊天 如果不是组队聊天或者一般信息或者全部显示 跳出
                    break;
                case MessageType.Guild://行会聊天
                    if (!(GameScene.Game.ChatBox.IsGuild || GameScene.Game.ChatBox.IsNormal || GameScene.Game.ChatBox.IsAll)) return;    //行会聊天 如果不是行会聊天或者一般信息或者不是全部显示 跳出
                    break;
                default:
                    return;
            }
            //超过宽度分割文本
            int lineWidth = Size.Width - headsize.Width;
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
                var background = new DXControl
                {
                    //Tag = type,
                    Tag = CEnvir.Now,
                    Parent = this,
                    PassThrough = true,
                    IsControl = false,
                };
                var typelabel = new DXLabel
                {
                    Tag = type,
                    Parent = background,
                    PassThrough = true,
                    IsControl = false,
                    Font = ChatFont,
                };
                if (j == 0)
                    UpdateColours(typelabel);
                typelabel.Location = new Point(headsize.Width - typelabel.Size.Width, 0);

                var label = new DXLabel
                {
                    Parent = background,
                    PassThrough = true,
                    IsControl = false,
                    Location = new Point(typelabel.Location.X + typelabel.Size.Width, typelabel.Location.Y),
                    ForeColour = Color.White,
                    Font = ChatFont,
                };
                label.Text = message ?? "";

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
                background.Size = new Size(label.Location.X + label.Size.Width, label.Size.Height);

                lock (obj)
                {
                    UpdateItems(background.Size.Height);
                    background.Location = new Point(0, Size.Height - background.Size.Height - 2);

                    History.Add(background);
                    Visible = true;
                }
            }
        }

        /// <summary>
        /// 更新聊天文字标签颜色
        /// </summary>
        /// <param name="label"></param>
        public void UpdateColours(DXLabel label)
        {
            switch ((MessageType)label.Tag)
            {
                case MessageType.Normal:      //普通文字
                    label.Text = "【普通】";
                    label.ForeColour = Color.White;
                    break;
                case MessageType.Shout:      //区域聊天
                    label.Text = "【喊话】";
                    label.ForeColour = Color.FromArgb(255, 192, 0);
                    break;
                case MessageType.Group:    //组队聊天
                    label.Text = "【组队】";
                    label.ForeColour = Color.FromArgb(112, 48, 160);
                    break;
                case MessageType.Global:  //世界聊天
                    label.Text = "【世界】";
                    label.ForeColour = Color.FromArgb(0, 176, 80);
                    break;
                case MessageType.WhisperIn:   //私聊信息
                    label.Text = "【私聊】";
                    label.ForeColour = Color.FromArgb(0, 112, 192);
                    break;
                case MessageType.GMWhisperIn:  //GM私聊信息
                    label.Text = "【GM私聊】";
                    label.ForeColour = Color.Red;
                    break;
                case MessageType.WhisperOut:   //收到私聊信息
                    label.Text = "【私聊】";
                    label.ForeColour = Color.FromArgb(0, 112, 192);
                    break;
                case MessageType.ObserverChat:  //观察者聊天
                    label.Text = "【观察者】";
                    label.ForeColour = Config.ObserverTextColour;
                    break;
                case MessageType.Guild:      //行会聊天
                    label.Text = "【行会】";
                    label.ForeColour = Color.FromArgb(0, 176, 240);
                    break;
                case MessageType.System:
                    label.Text = "【注意】";
                    label.ForeColour = Color.Red;
                    break;
                case MessageType.Notice:
                    label.Text = "【官方】";
                    label.ForeColour = Color.Red;
                    break;
            }
        }

        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            Visible = History.Count(p => p.Visible) != 0;

            if (History.Count > 0)
            {
                lock (obj)
                {
                    var old = History.FirstOrDefault(p => p.Visible);
                    if (old != null && !old.IsDisposed && (CEnvir.Now - (DateTime)old.Tag).Seconds > 15)  //信息停留时间
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
                oldLabel.TryDispose();
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
