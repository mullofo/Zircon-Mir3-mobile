using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{

    /// <summary>
    /// 聊天输入框
    /// </summary>
    public sealed class ChatTextBox : DXWindow
    {
        #region Properties

        #region Mode
        /// <summary>
        /// 聊天模式
        /// </summary>
        public ChatMode Mode
        {
            get { return _Mode; }
            set
            {
                if (_Mode == value) return;

                ChatMode oldValue = _Mode;
                _Mode = value;

                OnModeChanged(oldValue, value);
            }
        }
        private ChatMode _Mode;
        public event EventHandler<EventArgs> ModeChanged;
        public void OnModeChanged(ChatMode oValue, ChatMode nValue)
        {
            ModeChanged?.Invoke(this, EventArgs.Empty);

            switch (nValue)
            {
                case ChatMode.Local:
                    ChatModeButton.Label.Text = "一般".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 本地聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Group:
                    ChatModeButton.Label.Text = "组队".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 组队聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Guild:
                    ChatModeButton.Label.Text = "行会".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 行会聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Whisper:
                    ChatModeButton.Label.Text = "私聊".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 私聊聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Shout:
                    ChatModeButton.Label.Text = "喊话".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 喊话聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Global:
                    ChatModeButton.Label.Text = "世界".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 全服聊天模式".Lang(), MessageType.System);
                    break;
                case ChatMode.Observer:
                    ChatModeButton.Label.Text = "观察".Lang();
                    GameScene.Game.ReceiveChat("改变聊天模式 -> 观察者聊天模式".Lang(), MessageType.System);
                    break;
                default:
                    ChatModeButton.Label.Text = Mode.ToString();
                    break;
            }
        }

        #endregion

        public string LastPM;

        public DXTextBox TextBox;

        public DXButton ChatModeButton;

        /// <summary>
        /// 聊天框道具信息列表，用于封装到C.Chat封包中发送到服务端
        /// </summary>
        public List<ChatItemInfo> LinkedItems = new List<ChatItemInfo>();

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;

        #endregion

        /// <summary>
        /// 聊天输入框
        /// </summary>
        public ChatTextBox()
        {
            Size = new Size(360, 25);

            Opacity = 0F;
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            AllowResize = false;

            Movable = false;

            ChatModeButton = new DXButton   //聊天模式按钮切换
            {
                Parent = this,
                ButtonType = ButtonType.Default,
                Label = { Text = "一般".Lang() },
                Size = new Size(60, 24),
            };
            ChatModeButton.TouchUp += (o, e) => Mode = (ChatMode)(((int)(Mode) + 1) % 7);

            TextBox = new DXTextBox   //聊天输入框
            {
                Size = new Size(300, 22),
                Parent = this,
                MaxLength = Globals.MaxChatLength,
                //Opacity = 0.7F,
                BackColour = Color.FromArgb(0, 70, 52, 36),
                IsControl = true,
                BorderSize = 0,
                Border = false,
                Location = new Point(60, 3),
            };
            TextBox.TextBox.KeyPress += TextBox_KeyPress;

            TextBox.TextBox.KeyDown += TextBox_KeyDown;

            TextBox.MouseClick += TextBox_MouseClick;

            //SetClientSize(new Size(TextBox.Size.Width + 15, TextBox.Size.Height));
        }

        #region Methods
        /// <summary>
        /// 文本输入按键过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(TextBox.TextBox.Text))
                    {
                        CEnvir.Enqueue(new C.Chat
                        {
                            Text = TextBox.TextBox.Text,
                            Links = new List<ChatItemInfo>(LinkedItems)
                        });

                        if (TextBox.TextBox.Text[0] == '/')
                        {
                            string[] parts = TextBox.TextBox.Text.Split(' ');

                            if (parts.Length > 0)
                            {
                                LastPM = parts[0];
                                GameScene.Game.LatestWhispers.JoinWhisper(LastPM.Remove(0, 1));
                            }
                        }
                        TextBox.TextBox.Text = string.Empty;
                        LinkedItems.Clear();
                        TextBox.BackColour = Color.Black;
                    }
                    DXTextBox.ActiveTextBox = null;
                    if (!GameScene.Game.ChatBox.Visible)
                    {
                        Visible = false;
                    }
                    break;
                case (char)Keys.Escape:
                    e.Handled = true;
                    DXTextBox.ActiveTextBox = null;
                    TextBox.TextBox.Text = string.Empty;
                    LinkedItems.Clear();
                    TextBox.BackColour = Color.Black;
                    break;
            }
        }
        /// <summary>
        /// 聊天文本输入框鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            DXItemCell itemCell = GameScene.Game.SelectedCell;
            TextBox.BackColour = Color.FromArgb(0, 35, 26, 18);
            if (itemCell == null) return;

            string text = string.Format("<{0}> ", itemCell.Item.Info.Lang(p => p.ItemName));
            string newMsg = TextBox.TextBox.Text += text;
            if (newMsg.Length > Globals.MaxChatLength) return;

            LinkedItems.Add(new ChatItemInfo
            {
                Index = itemCell.Item.Index,
                GridType = itemCell.GridType,
                //正则，服务端需要通过以下正则生成 <ItemName/Index> 组合返回给客户端聊天文本信息
                RegexInternalName = $"<{itemCell.Item.Info.ItemName.Replace("(", "\\(").Replace(")", "\\)").Replace("（", "\\（").Replace("）", "\\）").Replace("[", "\\[").Replace("]", "\\]")}>",
                InternalName = $"<{itemCell.Item.Info.Lang(p => p.ItemName)}/{itemCell.Item.Index}>",
            });

            TextBox.TextBox.Text = newMsg;
            TextBox.TextBox.SelectionLength = 0;  //文本框选择长度为0
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length; //文本框启动=文本框文本长度

            if (GameScene.Game.SelectedCell != null) GameScene.Game.SelectedCell = null;
        }

        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
            {
                Keys keyCode = e.KeyCode;
                Keys keys = keyCode;
                if (keys != Keys.Up)
                {
                    if (keys == Keys.Down)
                    {
                        if (GameScene.Game.LatestWhispers.Visible)
                        {
                            GameScene.Game.LatestWhispers.MoveDown();
                            DXLabel selected = GameScene.Game.LatestWhispers.Selected;
                            string text = (selected != null) ? selected.Text : null;
                            if (text != null)
                            {
                                TextBox.TextBox.Text = "/" + text + " ";
                            }
                            TextBox.TextBox.SelectionLength = 0;
                            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                        }
                        e.Handled = true;
                    }
                }
                else
                {
                    if (GameScene.Game.LatestWhispers.Visible)
                    {
                        GameScene.Game.LatestWhispers.MoveUp();
                        DXLabel selected2 = GameScene.Game.LatestWhispers.Selected;
                        string text2 = (selected2 != null) ? selected2.Text : null;
                        if (text2 != null)
                        {
                            TextBox.TextBox.Text = "/" + text2 + " ";
                        }
                        TextBox.TextBox.SelectionLength = 0;
                        TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    }
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// 按键时-全局事件
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);  //基于按键

            switch (e.KeyChar)  //开关（键入字符）
            {
                case '@':
                    TextBox.SetFocus();
                    TextBox.TextBox.Text = @"@";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '!':
                    if (!Config.ShiftOpenChat) return;
                    TextBox.SetFocus();
                    TextBox.TextBox.Text = @"!";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
                case ' ':
                case (char)Keys.Enter:  //回车键进入
                    OpenChat();
                    e.Handled = true;
                    break;
                case '/':
                    TextBox.SetFocus();
                    if (string.IsNullOrEmpty(LastPM))
                        TextBox.TextBox.Text = "/";
                    else
                        TextBox.TextBox.Text = LastPM + " ";
                    TextBox.Visible = true;
                    TextBox.TextBox.SelectionLength = 0;
                    TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        /// <summary>
        /// 开始聊天
        /// </summary>
        public void OpenChat()
        {
            if (string.IsNullOrEmpty(TextBox.TextBox.Text))  //如果 （字符串为空（文本框 文本））
                switch (Mode)  //开关模式
                {
                    case ChatMode.Shout:    //聊天模式 区域喊话
                        TextBox.TextBox.Text = @"!";
                        break;
                    case ChatMode.Whisper:      //聊天模式 私聊
                        if (!string.IsNullOrWhiteSpace(LastPM))
                            TextBox.TextBox.Text = LastPM + " ";
                        break;
                    case ChatMode.Group:   //聊天模式 组队
                        TextBox.TextBox.Text = @"!!";
                        break;
                    case ChatMode.Guild:  //聊天模式 行会
                        TextBox.TextBox.Text = @"!~";
                        break;
                    case ChatMode.Global:  //聊天模式 世界喊话
                        TextBox.TextBox.Text = @"!@";
                        break;
                    case ChatMode.Observer:  //聊天模式 观察者聊天
                        TextBox.TextBox.Text = @"#";
                        break;
                }

            TextBox.SetFocus();      //设置文本框焦点
            TextBox.TextBox.SelectionLength = 0;  //文本框选择长度为0
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length; //文本框启动=文本框文本长度
            //TextBox.BackColour = Color.FromArgb(0, 35, 26, 18);
        }
        /// <summary>
        /// 开始焦点
        /// </summary>
        /// <param name="name"></param>
        public void StartPM(string name)
        {
            TextBox.TextBox.Text = $"/{name} ";  //文本框文本显示私聊/名字
            TextBox.SetFocus();
            Visible = true;
            TextBox.TextBox.SelectionLength = 0;
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
        }
        /// <summary>
        /// 设置输入文本
        /// </summary>
        /// <param name="str"></param>
        public void SetInputText(string str)
        {
            TextBox.TextBox.Text = str;  //文本框文本显示私聊/名字
            TextBox.SetFocus();
            TextBox.TextBox.SelectionLength = 0;
            TextBox.TextBox.SelectionStart = TextBox.TextBox.Text.Length;
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(TextBox.TextBox.Text))
            {
                CEnvir.Enqueue(new C.Chat
                {
                    Text = TextBox.TextBox.Text,
                    Links = new List<ChatItemInfo>(LinkedItems)
                });

                if (TextBox.TextBox.Text[0] == '/')
                {
                    string[] parts = TextBox.TextBox.Text.Split(' ');

                    if (parts.Length > 0)
                    {
                        LastPM = parts[0];
                        GameScene.Game.LatestWhispers.JoinWhisper(LastPM.Remove(0, 1));
                    }
                }
                TextBox.TextBox.Text = string.Empty;
                LinkedItems.Clear();
                //TextBox.BackColour = Color.Black;
            }
            DXTextBox.ActiveTextBox = null;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                LastPM = null;

                if (TextBox != null)
                {
                    if (!TextBox.IsDisposed)
                        TextBox.Dispose();

                    TextBox = null;
                }

                if (ChatModeButton != null)
                {
                    if (!ChatModeButton.IsDisposed)
                    {
                        ChatModeButton.Dispose();
                    }
                    ChatModeButton = null;
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// 聊天模式
    /// </summary>
    public enum ChatMode
    {
        /// <summary>
        /// 本地
        /// </summary>
        Local,
        /// <summary>
        /// 私聊
        /// </summary>
        Whisper,
        /// <summary>
        /// 组队
        /// </summary>
        Group,
        /// <summary>
        /// 行会
        /// </summary>
        Guild,
        /// <summary>
        /// 附近喊话
        /// </summary>
        Shout,
        /// <summary>
        /// 世界
        /// </summary>
        Global,
        /// <summary>
        /// 观察者
        /// </summary>
        Observer,
    }
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; }
    }
}
