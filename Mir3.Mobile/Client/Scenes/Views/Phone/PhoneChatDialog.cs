using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 主聊天框
    /// </summary>
    public class PhoneChatDialog : DXWindow
    {
        #region Properties

        /// <summary>
        /// 顶部标签
        /// </summary>
        public DXLabel Title1Label;
        public DXImageControl BackGround;
        private DXControl TextPanel;
        private DXButton SendMessageButton;
        /// <summary>
        /// 聊天模式开关
        /// </summary>
        public bool IsAll, IsGroup, IsNormal, IsSystem, IsGuild, IsMentor, IsUnion, IsOther;
        /// <summary>
        /// 黑名单
        /// </summary>
        public DXButton AllButton;
        /// <summary>
        /// 所有人聊天
        /// </summary>
        public DXButton NormalButton;
        /// <summary>
        /// 小组聊天
        /// </summary>
        public DXButton GroupButton;
        /// <summary>
        /// 行会聊天
        /// </summary>
        public DXButton GuildButton;
        /// <summary>
        /// 拒绝悄悄话
        /// </summary>
        public DXButton SystemButton;
        /// <summary>
        /// 拒绝行会聊天
        /// </summary>
        public DXButton UnionButton;
        /// <summary>
        /// 拒绝世界聊天
        /// </summary>
        public DXButton MentorButton;
        /// <summary>
        ///聊天框发送道具信息label
        /// </summary>
        public List<DXLabel> ChatItemLines;
        /// <summary>
        /// 复活标签
        /// </summary>
        public DXLabel ReviveLabel;
        /// <summary>
        /// 消息历史记录
        /// </summary>
        public List<Message> History;
        /// <summary>
        /// 聊天内容线
        /// </summary>
        public List<DXLabel> ChatLines;
        /// <summary>
        /// 滚动条计数
        /// </summary>
        public int StartIndex;
        /// <summary>
        /// 信息计数
        /// </summary>
        public int LineCount;
        /// <summary>
        /// 信息文字
        /// </summary>
        private Font ChatFont;
        private DXButton Close1Button;
        #endregion
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        /// <summary>
        /// 聊天框主界面
        /// </summary>
        public PhoneChatDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;
            Opacity = 0F;
            BackColour = Color.Black;
            AllowResize = false;
            CanResizeWidth = false;
            CanResizeHeightBottom = false;

            History = new List<Message>();
            ChatLines = new List<DXLabel>();
            ChatItemLines = new List<DXLabel>();
            ChatFont = new Font(Config.FontName, CEnvir.FontSize(9F));
            //KeyDown += ChatPanel_KeyDown;
            //PassThrough = true;

            BackGround = new DXImageControl
            {
                ImageOpacity = 0.7F,
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 1115,
                PassThrough = true,
            };
            Size = BackGround.Size;

            Title1Label = new DXLabel
            {
                Text = "聊天",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            Title1Label.Location = new Point((Size.Width - Title1Label.Size.Width) / 2, 15);

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = BackGround,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(465, 302),
                ImageOpacity = 0.7F,
            };
            Close1Button.TouchUp += (o, e) => Visible = false;

            TextPanel = new DXControl  //文本面板
            {
                DrawTexture = true,
                Parent = BackGround,
                Opacity = 0.7F,
                Size = new Size(490, 200),
                Location = new Point(8, 44),
                BackColour = Color.Black,
                //PassThrough = true,
            };
            TextPanel.FreeDrag += ChatPanel_MouseWheel;

            SendMessageButton = new DXButton
            {
                Parent = BackGround,
                Size = new Size(50, 24),
                ButtonType = ButtonType.Default,
                Label = { Text = "发送" },
                Location = new Point(380, 268),
                ImageOpacity = 0.7F,
            };
            SendMessageButton.TouchUp += (o, e) =>
            {
                GameScene.Game.ChatTextBox.SendMessage();
            };

            #region Status 按钮状态
            IsAll = true;      //全部
            IsNormal = true;   //一般
            IsGroup = true;    //队伍
            IsSystem = true;   //系统
            IsGuild = true;    //行会
            IsMentor = true;   //师徒
            IsUnion = true;    //联盟
            #endregion

            #region Buttons

            AllButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1070,
                Tag = MessageType.All,
                Hint = "[拒绝所有信息]".Lang(),
                Parent = BackGround,
            };
            AllButton.Location = new Point(20, BackGround.Size.Height - AllButton.Size.Height - 15);
            AllButton.MouseClick += ChatMouseClick;

            NormalButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1071,
                Tag = MessageType.Normal,
                Hint = "[拒绝本地聊天]".Lang(),
                Parent = BackGround,
                Location = new Point(20, 277)
            };
            NormalButton.Location = new Point(AllButton.Location.X + NormalButton.Size.Width + 12, AllButton.Location.Y);
            NormalButton.MouseClick += ChatMouseClick;

            GroupButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1072,
                Tag = MessageType.Group,
                Hint = "[拒绝组队聊天]".Lang(),
                Parent = BackGround,
                Location = new Point(126, 277)
            };
            GroupButton.Location = new Point(NormalButton.Location.X + GroupButton.Size.Width + 12, AllButton.Location.Y);
            GroupButton.MouseClick += ChatMouseClick;

            GuildButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1073,
                Tag = MessageType.Guild,
                Hint = "[拒绝行会聊天]".Lang(),
                Parent = BackGround,
                Location = new Point(166, 277)
            };
            GuildButton.Location = new Point(GroupButton.Location.X + GuildButton.Size.Width + 12, AllButton.Location.Y);
            GuildButton.MouseClick += ChatMouseClick;

            SystemButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1074,
                Tag = MessageType.System,
                Hint = "[拒绝系统信息]".Lang(),
                Parent = BackGround,
                Location = new Point(206, 277)
            };
            SystemButton.Location = new Point(GuildButton.Location.X + SystemButton.Size.Width + 12, AllButton.Location.Y);
            SystemButton.MouseClick += ChatMouseClick;

            UnionButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1075,
                Tag = MessageType.Union,
                Hint = "[拒绝联盟聊天]".Lang(),
                Parent = BackGround,
                Location = new Point(246, 277)
            };
            UnionButton.Location = new Point(SystemButton.Location.X + UnionButton.Size.Width + 12, AllButton.Location.Y);
            UnionButton.MouseClick += ChatMouseClick;

            MentorButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1076,
                Tag = MessageType.Mentor,
                Hint = "[拒绝世界喊话]".Lang(),
                Parent = BackGround,
                Location = new Point(287, 277)
            };
            MentorButton.Location = new Point(UnionButton.Location.X + MentorButton.Size.Width + 12, AllButton.Location.Y);
            MentorButton.MouseClick += ChatMouseClick;

            #endregion

            LineCount = TextPanel.Size.Height / (TextRenderer.MeasureText(DXManager.Graphics, "字", ChatFont).Height + 2);
        }

        #region methods

        /// <summary>
        /// 显示聊天内容
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ReceiveChat(string message, MessageType type)
        {
            switch (type)
            {
                case MessageType.Normal:
                    if (!(IsNormal || IsAll)) return;  //一般信息 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Shout:
                    if (!(IsNormal || IsAll)) return;  //区域聊天 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Global:
                    if (!IsMentor) return;    //世界喊话 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Group:
                    if (!(IsGroup || IsNormal || IsAll)) return;    //组队聊天 如果不是组队聊天或者一般信息或者全部显示 跳出
                    break;
                case MessageType.Hint:
                    if (!(IsNormal || IsAll)) return;    //提示信息 跳出
                    break;
                case MessageType.System:
                    if (!(IsSystem || IsAll)) return;    //系统信息 如果不是系统信息或者不是全部显示 跳出
                    break;
                case MessageType.WhisperIn:
                case MessageType.WhisperOut:
                    if (BigPatchConfig.ChkMsgNotify)
                    {
                        DXSoundManager.Play(SoundIndex.news);
                    }
                    if (!(IsNormal || IsAll)) return;      //私聊 收到私聊 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Combat:
                    //if (!Config.ChkCloseCombatTips) return;
                    if (!(IsNormal || IsAll)) return;    //战斗信息 跳出
                    break;
                case MessageType.ObserverChat:
                    if (!(IsNormal || IsAll)) return;     //观察者聊天 如果不是一般信息或者全部显示 跳出
                    break;
                case MessageType.Guild:
                    if (!(IsGuild || IsNormal || IsAll)) return;    //行会聊天 如果不是行会聊天或者一般信息或者不是全部显示 跳出
                    break;
                case MessageType.UseItem:
                    if (!(IsNormal || IsAll)) return;   //道具使用提示 跳出
                    break;
            }

            //超过宽度分割文本
            int lineWidth = TextPanel.Size.Width;
            List<string> list = new List<string>();
            int width = 0;
            int height = 0;
            for (int i = 1; i < message.Length; i++)
            {
                Size si = TextRenderer.MeasureText(DXManager.Graphics, message.Substring(width, i - width), ChatFont);
                if (si.Width > lineWidth)
                {
                    list.Add(message.Substring(width, i - width - 1));
                    width = i - 1;
                }
                height = si.Height;
            }
            list.Add(message.Substring(width, message.Length - width));

            if (StartIndex == History.Count - LineCount)
            {
                StartIndex += list.Count;
            }
            for (int j = 0; j < list.Count; j++)
            {
                History.Add(new Message
                {
                    Text = list[j],
                    Type = type
                });
            }
            while (History.Count > 300)
            {
                Message item = History[0];
                History.Remove(item);
            }

            Update();
        }

        private int tempStartIndex;
        public void Update()
        {
            for (int x = 0; x < ChatLines.Count; x++)
            {
                ChatLines[x].Dispose();
            }
            ChatLines.Clear();

            for (int x = 0; x < ChatItemLines.Count; x++)
            {
                ChatItemLines[x].Dispose();
            }
            ChatItemLines.Clear();

            if (StartIndex >= History.Count)
            {
                StartIndex = History.Count - 1;
            }
            if (StartIndex < 0)
            {
                StartIndex = 0;
            }

            if (History.Count > 1)
            {
                if (tempStartIndex != StartIndex)
                {
                    tempStartIndex = StartIndex;
                    Update();
                }
            }

            int height = 0;
            for (int x = StartIndex; x < History.Count; x++)
            {
                DXLabel label = new DXLabel
                {
                    AutoSize = true,
                    Text = History[x].Text,
                    Location = new Point(0, height),
                    Outline = false,
                    DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,   //文本格式   分词 | 单词省略号
                    Parent = TextPanel,
                    Font = ChatFont,
                    Tag = History[x].Type,
                };
                //label.MouseWheel += ChatPanel_MouseWheel;
                label.FreeDrag += ChatPanel_MouseWheel;

                switch (History[x].Type)
                {
                    case MessageType.Normal:  //普通聊天
                    case MessageType.Shout:   //区域聊天
                    case MessageType.Global:  //世界喊话
                    case MessageType.WhisperIn:  //私聊
                    case MessageType.WhisperOut:  //收到私聊
                    case MessageType.Group:    //组队聊天
                    case MessageType.ObserverChat:  //观察者聊天
                    case MessageType.Guild:   //行会聊天
                        label.Tap += (o, e) =>
                        {
                            string[] parts = label.Text.Split(':', ' ');
                            if (parts.Length == 0) return;

                            //选择聊天框 支持中文
                            GameScene.Game.ChatTextBox.StartPM(Regex.Replace(parts[0], "[^A-Za-z0-9\u4e00-\u9fa5]", ""));
                        };
                        //label.MouseDoubleClick += (o, e) =>
                        //{
                        //    //双击 支持中文
                        //    GameScene.Game.ChatTextBox.SetInputText(label.Text);
                        //};
                        break;
                    case MessageType.GMWhisperIn:     //GM私聊  
                    case MessageType.Hint:          //提示信息 
                    case MessageType.System:        //系统信息
                    case MessageType.Announcement:  //公告信息
                    case MessageType.Combat:        //战斗信息提示
                    case MessageType.Notice:        //告示中央显示
                    case MessageType.RollNotice:    //中央滚动告示
                    case MessageType.ItemTips:      //极品物品提示
                    case MessageType.BossTips:      //boss提示 
                    case MessageType.UseItem:       //道具使用提示
                    case MessageType.DurabilityTips: //持久提示
                    case MessageType.Revive:
                        break;
                }

                //解析聊天框接收的道具信息（客户端解析服务端返回的 <ItemName/Index> 的组合）
                string currentLine = label.Text;
                int oldLength = currentLine.Length;
                Capture capture = null;
                foreach (Match match in Globals.RegexChatItemName.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList())
                {
                    try
                    {
                        int offSet = oldLength - currentLine.Length;

                        //拆分 <ItemName/Index> 的组合
                        capture = match.Groups[1].Captures[0];
                        string[] values = capture.Value.Split('/');
                        currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, values[0]);
                        string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                        Size size1 = TextRenderer.MeasureText(DXManager.Graphics, text, label.Font, label.Size, TextFormatFlags.TextBoxControl);

                        //通过拆分后Index来创建道具信息label
                        ClientUserItem item = GameScene.Game.ChatItemList.FirstOrDefault(x => x.Index == int.Parse(values[1]));
                        if (item != null)
                        {
                            DXLabel itemLabel = new DXLabel   //道具信息
                            {
                                AutoSize = true,
                                Visible = true,
                                Location = new Point(size1.Width - 11, 0),
                                Text = values[0],
                                BackColour = Color.Orange,
                                Outline = false,
                                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,   //文本格式   分词 | 单词省略号
                                Parent = label,
                                Font = ChatFont,
                                ForeColour = Color.Black,
                            };

                            itemLabel.MouseEnter += (o, e) =>  //鼠标进入指向道具显示道具内容
                            {
                                itemLabel.BackColour = Color.Red;
                                GameScene.Game.MouseItem = item;
                            };
                            itemLabel.MouseLeave += (o, e) =>  //鼠标离开道具内容置空
                            {
                                itemLabel.BackColour = Color.Orange;
                                GameScene.Game.MouseItem = null;
                            };

                            ChatItemLines.Add(itemLabel);
                        }
                    }
                    catch (Exception ex)
                    {
                        CEnvir.SaveError(ex.ToString());
                        CEnvir.SaveError(currentLine);
                        CEnvir.SaveError(capture.Value);
                    }
                }
                label.Text = currentLine;

                UpdateColours(label);
                ChatLines.Add(label);

                height += label.Size.Height + 2;
                if (x - StartIndex == LineCount - 1) break;
            }

        }
        /// <summary>
        /// 更新聊天文字标签颜色
        /// </summary>
        /// <param name="label"></param>
        public void UpdateColours(DXLabel label)
        {
            Color empty = false ? Color.FromArgb(100, 0, 0, 0) : Color.Empty;  //透明标签
            switch ((MessageType)label.Tag)
            {
                case MessageType.Normal:      //普通文字
                    label.BackColour = empty;  //背景色 空值
                    label.ForeColour = Config.LocalTextColour;
                    break;
                case MessageType.Shout:      //区域聊天
                    label.BackColour = Color.Yellow;
                    label.ForeColour = Config.ShoutTextColour;   //文字色 黑色
                    break;
                case MessageType.Group:    //组队聊天
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.GroupTextColour;
                    break;
                case MessageType.Global:  //世界聊天
                    label.BackColour = Color.Lime;
                    label.ForeColour = Config.GlobalTextColour;
                    break;
                case MessageType.Hint:   //提示信息
                    label.BackColour = Color.Red;
                    label.ForeColour = Config.HintTextColour;
                    break;
                case MessageType.System:   //系统信息
                    label.BackColour = Color.Red;
                    label.ForeColour = Color.White;
                    break;
                case MessageType.Announcement:  //公告信息
                    label.BackColour = Color.FromArgb(255, 255, 128, 0);  //背景色 橙色
                    label.ForeColour = Config.AnnouncementTextColour;  //文字色 白色
                    break;
                case MessageType.WhisperIn:   //私聊信息
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.WhisperInTextColour;
                    break;
                case MessageType.GMWhisperIn:  //GM私聊信息
                    label.BackColour = empty;
                    label.ForeColour = Color.Lime;
                    break;
                case MessageType.WhisperOut:   //收到私聊信息
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.WhisperOutTextColour;
                    break;
                case MessageType.Combat:   //战斗信息提示
                    label.BackColour = Color.Red;
                    label.ForeColour = Color.White;
                    break;
                case MessageType.ObserverChat:  //观察者聊天
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.ObserverTextColour;
                    break;
                case MessageType.Guild:      //行会聊天
                    label.BackColour = empty;    //背景色 空值
                    label.ForeColour = Config.GuildTextColour;
                    break;
                case MessageType.Notice:          //告示
                    //label.BackColour = Color.FromArgb(255, 150, 145, 255);   //背景色 蓝底
                    //label.ForeColour = Color.Black;     //前景色
                    label.BackColour = Color.Red;   //背景色 蓝底
                    label.ForeColour = Color.White;     //前景色
                    break;
                case MessageType.ItemTips:          //极品物品提示
                    label.BackColour = Color.Blue;    //背景色 白色
                    label.ForeColour = Color.White;    //前景色
                    break;
                case MessageType.BossTips:          //boss提示
                    label.BackColour = Color.Magenta;   //背景色 白色
                    label.ForeColour = Color.White;        //前景色
                    break;
                case MessageType.RollNotice:          //走马灯中央滚动告示
                    label.BackColour = Color.Red;            //背景色 红底
                    label.ForeColour = Color.White;          //文字颜色 白色
                    break;
                case MessageType.DurabilityTips: //持久提示
                    label.BackColour = Color.Yellow;            //背景色 黄色
                    label.ForeColour = Color.Black;          //文字颜色 黑色
                    break;
                case MessageType.UseItem:       //道具使用提示
                    label.BackColour = Color.FromArgb(255, 255, 128, 0);  //背景色 橙色
                    label.ForeColour = Color.White;          //文字颜色 白色
                    label.Outline = false;
                    break;
            }
        }

        /// <summary>
        /// 聊天按钮鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatMouseClick(object sender, MouseEventArgs e)
        {
            var btn = (DXButton)sender;
            MessageType type = (MessageType)btn.Tag;
            switch (type)
            {
                case MessageType.All:
                    IsAll = !IsAll;
                    IsNormal = IsAll;
                    IsGuild = IsAll;
                    IsGroup = IsAll;
                    IsSystem = IsAll;
                    IsUnion = IsAll;
                    IsMentor = IsAll;

                    AllButton.Index = IsAll ? 1070 : 1080;
                    AllButton.Hint = IsAll ? "[拒绝所有信息]" : "[激活所有信息]";
                    NormalButton.Index = IsNormal ? 1071 : 1081;
                    GuildButton.Index = IsGuild ? 1073 : 1083;
                    GroupButton.Index = IsGroup ? 1072 : 1082;
                    SystemButton.Index = IsSystem ? 1074 : 1084;
                    UnionButton.Index = IsUnion ? 1075 : 1085;
                    MentorButton.Index = IsMentor ? 1076 : 1086;
                    break;
                case MessageType.Normal:
                    IsNormal = !IsNormal;
                    //ChangeAllButtonStatue(IsNormal);
                    NormalButton.Hint = IsNormal ? "[拒绝本地聊天]" : "[激活本地聊天]";
                    NormalButton.Index = IsNormal ? 1071 : 1081;
                    break;
                case MessageType.Guild:
                    IsGuild = !IsGuild;
                    //ChangeAllButtonStatue(IsGuild);
                    GuildButton.Hint = IsGuild ? "[拒绝行会聊天]" : "[激活行会聊天]";
                    GuildButton.Index = IsGuild ? 1073 : 1083;
                    break;
                case MessageType.Group:
                    IsGroup = !IsGroup;
                    //ChangeAllButtonStatue(IsGroup);
                    GroupButton.Hint = IsGroup ? "[拒绝组队聊天]" : "[激活组队聊天]";
                    GroupButton.Index = IsGroup ? 1072 : 1082;
                    break;
                case MessageType.System:
                    IsSystem = !IsSystem;
                    //ChangeAllButtonStatue(IsSystem);
                    SystemButton.Hint = IsSystem ? "[拒绝系统信息]" : "[激活系统信息]";
                    SystemButton.Index = IsSystem ? 1074 : 1084;
                    break;
                case MessageType.Union:
                    IsUnion = !IsUnion;
                    //ChangeAllButtonStatue(IsUnion);
                    UnionButton.Hint = IsUnion ? "[拒绝联盟聊天]" : "[激活联盟聊天]";
                    UnionButton.Index = IsUnion ? 1075 : 1085;
                    break;
                case MessageType.Mentor:
                    IsMentor = !IsMentor;
                    //ChangeAllButtonStatue(IsMentor);
                    MentorButton.Hint = IsMentor ? "[拒绝世界喊话]" : "[激活世界喊话]";
                    MentorButton.Index = IsMentor ? 1076 : 1086;
                    break;
            }
        }
        /// <summary>
        /// 信息框鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChatPanel_MouseWheel(object sender, TouchEventArgs e)
        {
            int value = (int)e.Delta.Y;
            if ((StartIndex != 0 || value < 0) && (StartIndex != History.Count - 1 || value > 0))
            {
                StartIndex -= value / 10;
                Update();
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Title1Label != null)
                {
                    if (!Title1Label.IsDisposed)
                    {
                        Title1Label.Dispose();
                    }
                    Title1Label = null;
                }

                if (BackGround != null)
                {
                    if (!BackGround.IsDisposed)
                    {
                        BackGround.Dispose();
                    }
                    BackGround = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                    {
                        Close1Button.Dispose();
                    }
                    Close1Button = null;
                }

                if (SendMessageButton != null)
                {
                    if (!SendMessageButton.IsDisposed)
                    {
                        SendMessageButton.Dispose();
                    }
                    SendMessageButton = null;
                }

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                    {
                        TextPanel.Dispose();
                    }
                    TextPanel = null;
                }

                if (ChatLines != null)
                {
                    for (int i = 0; i < ChatLines.Count; i++)
                    {
                        if (ChatLines[i] != null)
                        {
                            if (!ChatLines[i].IsDisposed)
                                ChatLines[i].Dispose();

                            ChatLines[i] = null;
                        }
                    }

                    ChatLines.Clear();
                    ChatLines = null;
                }

                if (ChatItemLines != null)
                {
                    for (int i = 0; i < ChatItemLines.Count; i++)
                    {
                        if (ChatItemLines[i] != null)
                        {
                            if (!ChatItemLines[i].IsDisposed)
                                ChatItemLines[i].Dispose();

                            ChatItemLines[i] = null;
                        }
                    }

                    ChatItemLines.Clear();
                    ChatItemLines = null;
                }

                if (History != null)
                {
                    History.Clear();
                    History = null;
                }

                if (ReviveLabel != null)
                {
                    if (!ReviveLabel.IsDisposed)
                        ReviveLabel.Dispose();

                    ReviveLabel = null;
                }

                if (AllButton != null)
                {
                    if (!AllButton.IsDisposed)
                    {
                        AllButton.Dispose();
                    }
                    AllButton = null;
                }

                if (NormalButton != null)
                {
                    if (!NormalButton.IsDisposed)
                    {
                        NormalButton.Dispose();
                    }
                    NormalButton = null;
                }

                if (GroupButton != null)
                {
                    if (!GroupButton.IsDisposed)
                    {
                        GroupButton.Dispose();
                    }
                    GroupButton = null;
                }

                if (SystemButton != null)
                {
                    if (!SystemButton.IsDisposed)
                    {
                        SystemButton.Dispose();
                    }
                    SystemButton = null;
                }

                if (GuildButton != null)
                {
                    if (!GuildButton.IsDisposed)
                    {
                        GuildButton.Dispose();
                    }
                    GuildButton = null;
                }

                if (MentorButton != null)
                {
                    if (!MentorButton.IsDisposed)
                    {
                        MentorButton.Dispose();
                    }
                    MentorButton = null;
                }

                if (UnionButton != null)
                {
                    if (!UnionButton.IsDisposed)
                    {
                        UnionButton.Dispose();
                    }
                    UnionButton = null;
                }

                StartIndex = 0;
                LineCount = 0;
                ChatFont = null;
            }
        }
        #endregion
    }

}