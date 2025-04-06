using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 主聊天框
    /// </summary>
    public class ChatDialog : DXWindow
    {
        #region Properties

        public DXControl TextPanel;
        public DXImageControl BigChatPanel, ChatPanel, Upgrade, BigChatPanelBackground;
        public PhotoControl Photo;
        public DXLabel LevelLabel;

        public DXVScrollBar ScrollBar;
        public DXButton ExpendButton, ShrinkButton, BigPatch;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => true;
        public override bool AutomaticVisibility => true;
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
        /// 聊天模式开关
        /// </summary>
        public bool IsAll, IsGroup, IsNormal, IsSystem, IsGuild, IsMentor, IsUnion, IsOther;
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
        //public DXControl LabelPanel;
        #endregion

        /// <summary>
        /// 聊天框主界面
        /// </summary>
        public ChatDialog()
        {
            Movable = false;
            Opacity = 0F;
            HasTitle = false;
            HasTopBorder = false;
            CloseButton.Visible = false;
            Border = false;
            BackColour = Color.Black;
            AllowResize = false;
            CanResizeWidth = false;
            CanResizeHeightBottom = false;
            Size = new Size(680, 320);
            History = new List<Message>();
            ChatLines = new List<DXLabel>();
            ChatItemLines = new List<DXLabel>();
            LineCount = 5;
            ChatFont = new Font(Config.FontName, CEnvir.FontSize(9F));
            KeyDown += ChatPanel_KeyDown;
            PassThrough = true;

            ChatPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Size = new Size(680, 152),
                Index = 1101,
                Visible = true,
                PassThrough = true,
            };
            ChatPanel.Location = new Point(0, 168);

            BigChatPanel = new DXImageControl
            {
                Parent = this,
                Size = new Size(680, 320),
                LibraryFile = LibraryFile.UI1,
                Index = 1102,
                Visible = false,
                PassThrough = true,
            };
            BigChatPanelBackground = new DXImageControl   //透明背景蒙版
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1103,
                Parent = BigChatPanel,
                ImageOpacity = 0.5F,
                Location = new Point(6, 37),
                PassThrough = true,
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
                Parent = BigChatPanel,
                Location = new Point(46, 277)
            };
            AllButton.MouseClick += ChatMouseClick;

            NormalButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1071,
                Tag = MessageType.Normal,
                Hint = "[拒绝本地聊天]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(86, 277)
            };
            NormalButton.MouseClick += ChatMouseClick;

            GroupButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1072,
                Tag = MessageType.Group,
                Hint = "[拒绝组队聊天]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(126, 277)
            };
            GroupButton.MouseClick += ChatMouseClick;

            GuildButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1073,
                Tag = MessageType.Guild,
                Hint = "[拒绝行会聊天]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(166, 277)
            };
            GuildButton.MouseClick += ChatMouseClick;

            SystemButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1074,
                Tag = MessageType.System,
                Hint = "[拒绝系统信息]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(206, 277)
            };
            SystemButton.MouseClick += ChatMouseClick;

            UnionButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1075,
                Tag = MessageType.Union,
                Hint = "[拒绝联盟聊天]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(246, 277)
            };
            UnionButton.MouseClick += ChatMouseClick;

            MentorButton = new DXButton
            {
                ImageOpacity = 0.7F,
                LibraryFile = LibraryFile.UI1,
                Index = 1076,
                Tag = MessageType.Mentor,
                Hint = "[拒绝世界喊话]".Lang(),
                Parent = BigChatPanel,
                Location = new Point(287, 277)
            };
            MentorButton.MouseClick += ChatMouseClick;

            #endregion

            TextPanel = new DXControl  //文本面板
            {
                Parent = ChatPanel,
                Opacity = 0,
                Size = new Size(565, 75),
                Location = new Point(10, 45),
                PassThrough = true,
            };
            TextPanel.MouseWheel += ChatPanel_MouseWheel;

            ScrollBar = new DXVScrollBar  //聊天信息滚动条
            {
                Parent = ChatPanel,
                Size = new Size(16, 100),
                VisibleSize = 1,
                Change = 1,
            };
            ScrollBar.Location = new Point(663, 40);
            ScrollBar.ValueChanged += (o, e) =>
            {
                StartIndex = ScrollBar.Value;
                Update();
            };
            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);

            Photo = new PhotoControl  //绘制各职业角色图片
            {
                Location = new Point(580, 209),
                Size = new Size(84, 94),
                Parent = this
            };

            BigPatch = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1130,
                Hint = "大补贴辅助(HOME)".Lang(),
                Visible = false,
            };
            BigPatch.Location = new Point(580, 304);
            BigPatch.MouseClick += (o, e) => GameScene.Game.BigPatchBox.Visible = !GameScene.Game.BigPatchBox.Visible;

            Upgrade = new DXImageControl  //时间变化
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1200,
            };
            Upgrade.Location = new Point(318, 177);
            Upgrade.AfterDraw += delegate
            {
                int n = (GameScene.Game.MapControl.MapInfo.Light == LightSetting.Default) ? 1200 : ((GameScene.Game.MapControl.MapInfo.Light == LightSetting.Light) ? 1201 : ((GameScene.Game.MapControl.MapInfo.Light != LightSetting.Night) ? 1203 : 1202));
                MirImage mirImage2 = Upgrade.Library.CreateImage(n, ImageType.Image);
                PresentTexture(mirImage2.Image, this, new Rectangle(Upgrade.DisplayArea.X, Upgrade.DisplayArea.Y, mirImage2.Width, mirImage2.Height), Color.White, Upgrade);
            };

            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.Yellow,
                Size = new Size(30, 16),
                Hint = "等级".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            LevelLabel.Location = new Point(320, 186);

            ExpendButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Parent = this,
                Index = 1068,
                Hint = "放大聊天窗(R)".Lang(),
                Visible = true,
            };
            ExpendButton.Location = new Point(665, 195);
            ExpendButton.MouseEnter += (o, e) =>     //鼠标移动上去
            {
                ExpendButton.Index = 1168;
                ExpendButton.Location = new Point(665, 193);
            };
            ExpendButton.MouseLeave += (o, e) =>     //鼠标离开
            {
                ExpendButton.Index = 1068;
                ExpendButton.Location = new Point(665, 195);
            };

            ExpendButton.MouseClick += (o, e) =>   //鼠标点击聊天框放大
            {
                ChatPanel.Visible = false;
                BigChatPanel.Visible = true;
                ShrinkButton.Location = new Point(665, 27);
                ExpendButton.Visible = false;
                ShrinkButton.Visible = true;
                TextPanel.Parent = BigChatPanelBackground;
                TextPanel.Location = new Point(4, 5);
                TextPanel.Size = new Size(653, 210);
                ScrollBar.Parent = BigChatPanel;
                ScrollBar.Location = new Point(663, 40);
                ScrollBar.Size = new Size(16, 268);
                LineCount = 14;
                Photo.Visible = false;
                BigPatch.Visible = false;
                Upgrade.Location = new Point(318, 9);
                LevelLabel.Location = new Point(320, 17);
                GameScene.Game.ChatTextBox.Location = new Point((GameScene.Game.Size.Width - GameScene.Game.ChatTextBox.Size.Width) / 2 - 28, GameScene.Game.Size.Height - GameScene.Game.ChatTextBox.Size.Height - 22);
            };

            ShrinkButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Parent = this,
                Index = 1068,
                Hint = "缩小聊天窗(R)".Lang(),
                Visible = false,
            };
            ShrinkButton.MouseEnter += (o, e) =>     //鼠标移动上去
            {
                ShrinkButton.Index = 1168;
                ShrinkButton.Location = new Point(665, 25);
            };
            ShrinkButton.MouseLeave += (o, e) =>     //鼠标离开
            {
                ShrinkButton.Index = 1068;
                ShrinkButton.Location = new Point(665, 27);
            };
            ShrinkButton.MouseClick += (o, e) =>   //鼠标点击聊天框缩小
            {
                ChatPanel.Visible = true;
                BigChatPanel.Visible = false;
                ExpendButton.Location = new Point(665, 193);
                ShrinkButton.Visible = false;
                ExpendButton.Visible = true;
                TextPanel.Parent = ChatPanel;
                TextPanel.Location = new Point(10, 45);
                TextPanel.Size = new Size(565, 75);
                ScrollBar.Parent = ChatPanel;
                ScrollBar.Location = new Point(663, 40);
                ScrollBar.Size = new Size(16, 100);
                LineCount = 5;
                Photo.Visible = true;
                BigPatch.Visible = true;
                Upgrade.Location = new Point(318, 177);
                LevelLabel.Location = new Point(320, 185);
                GameScene.Game.ChatTextBox.Location = new Point((GameScene.Game.Size.Width - GameScene.Game.ChatTextBox.Size.Width) / 2 - 28, GameScene.Game.Size.Height - GameScene.Game.ChatTextBox.Size.Height + 20);
            };
        }

        #region methods

        /// <summary>
        /// 聊天按钮鼠标点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatMouseClick(object sender, MouseEventArgs e)
        {
            var btn = (DXButton)sender;
            MessageType type;
            if (!Enum.TryParse<MessageType>(btn.Tag.ToString(), out type))
            {
                return;
            }
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
        /// 更改所有按钮状态
        /// </summary>
        /// <param name="show"></param>
        private void ChangeAllButtonStatue(bool show)
        {
            if (!show)
            {
                IsAll = false;
                AllButton.Index = IsAll ? 1070 : 1080;
            }
            else
            {
                if (IsGuild && IsGroup && IsNormal && IsSystem)
                {
                    IsAll = true;
                    AllButton.Index = IsAll ? 1070 : 1080;
                }
            }
        }

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
            for (int i = 1; i < message.Length; i++)
            {
                if (TextRenderer.MeasureText(DXManager.Graphics, message.Substring(width, i - width), ChatFont).Width > lineWidth)
                {
                    list.Add(message.Substring(width, i - width - 1));
                    width = i - 1;
                }
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
                ScrollBar.MaxValue = History.Count;
                ScrollBar.Value = StartIndex;
            }

            int height = 0;
            for (int x = StartIndex; x < History.Count; x++)
            {
                DXLabel label = new DXLabel   //信息内容
                {
                    AutoSize = true,
                    Text = History[x].Text,
                    Location = new Point(0, height),
                    Outline = false,
                    DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,   //文本格式   分词 | 单词省略号
                    Parent = TextPanel,
                    Tag = History[x].Type,
                };
                label.MouseWheel += ChatPanel_MouseWheel;

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
                        label.MouseUp += (o, e) =>
                        {
                            string[] parts = label.Text.Split(':', ' ');
                            if (parts.Length == 0) return;

                            //选择聊天框 支持中文
                            GameScene.Game.ChatTextBox.StartPM(Regex.Replace(parts[0], "[^A-Za-z0-9\u4e00-\u9fa5]", ""));
                        };
                        label.MouseDoubleClick += (o, e) =>
                        {
                            //双击 支持中文
                            GameScene.Game.ChatTextBox.SetInputText(label.Text);
                        };
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
                        //提示信息不需要鼠标事件
                        //label.MouseDoubleClick += (o, e) =>
                        //{
                        //    //双击 支持中文
                        //    GameScene.Game.ChatTextBox.SetInputText(label.Text);
                        //};
                        break;
                    case MessageType.Revive:
                        //DXMessageBox box = new DXMessageBox($"Chat.Revive".Lang(), "复活".Lang(), DXMessageBoxButtons.YesNo);

                        //box.YesButton.MouseClick += (o, e) =>   //YES按钮鼠标点击
                        //{
                        //    CEnvir.Enqueue(new C.TownRevive());
                        //    label.Dispose();
                        //};

                        //ReviveLabel = label;
                        //label.MouseClick += (o, e) =>   //鼠标点击
                        //{
                        //    CEnvir.Enqueue(new C.TownRevive());
                        //    History.Remove(History[(int)(o as DXLabel).Tag_0]);
                        //    Update();
                        //};
                        //label.Tag = MessageType.Announcement;
                        //label.Tag_0 = x;
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
                            //itemLabel.MouseDown += (o, e) => itemLabel.ForeColour = Color.Blue;
                            //itemLabel.MouseUp += (o, e) => itemLabel.ForeColour = Color.Red;

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

                height += 15;
                if (x - StartIndex == LineCount - 1) break;
            }

        }
        /// <summary>
        /// 更新聊天文字标签颜色
        /// </summary>
        /// <param name="label"></param>
        private void UpdateColours(DXLabel label)
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
        /// 信息框按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Prior:
                    if (StartIndex != 0)
                    {
                        StartIndex -= LineCount;
                        Update();
                        e.Handled = true;
                    }
                    break;
                case Keys.Next:
                    if (StartIndex != History.Count - 1)
                    {
                        StartIndex += LineCount;
                        Update();
                        e.Handled = true;
                    }
                    break;
                case Keys.End:
                    if (StartIndex != History.Count - 1)
                    {
                        StartIndex = History.Count - 1;
                        Update();
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    if (StartIndex != 0)
                    {
                        StartIndex--;
                        Update();
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (StartIndex != History.Count - 1)
                    {
                        StartIndex++;
                        Update();
                        e.Handled = true;
                    }
                    break;
                case Keys.Home:
                case Keys.Left:
                case Keys.Right:
                    break;
            }
        }
        /// <summary>
        /// 信息框鼠标滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int value = e.Delta / SystemInformation.MouseWheelScrollDelta;
            if ((StartIndex != 0 || value < 0) && (StartIndex != History.Count - 1 || value > 0))
            {
                StartIndex -= value;
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
                if (ChatPanel != null)
                {
                    if (!ChatPanel.IsDisposed)
                        ChatPanel.Dispose();

                    ChatPanel = null;
                }

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                        TextPanel.Dispose();

                    TextPanel = null;
                }

                if (BigChatPanel != null)
                {
                    if (!BigChatPanel.IsDisposed)
                        BigChatPanel.Dispose();

                    BigChatPanel = null;
                }

                if (BigChatPanelBackground != null)
                {
                    if (!BigChatPanelBackground.IsDisposed)
                        BigChatPanelBackground.Dispose();

                    BigChatPanelBackground = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
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

                if (Photo != null)
                {
                    if (!Photo.IsDisposed)
                    {
                        Photo.Dispose();
                    }
                    Photo = null;
                }

                if (BigPatch != null)
                {
                    if (!BigPatch.IsDisposed)
                    {
                        BigPatch.Dispose();
                    }
                    BigPatch = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (Upgrade != null)
                {
                    if (!Upgrade.IsDisposed)
                        Upgrade.Dispose();

                    Upgrade = null;
                }

                if (ExpendButton != null)
                {
                    if (!ExpendButton.IsDisposed)
                        ExpendButton.Dispose();

                    ExpendButton = null;
                }

                if (ShrinkButton != null)
                {
                    if (!ShrinkButton.IsDisposed)
                        ShrinkButton.Dispose();

                    ShrinkButton = null;
                }

                if (ReviveLabel != null)
                {
                    if (!ReviveLabel.IsDisposed)
                        ReviveLabel.Dispose();

                    ReviveLabel = null;
                }

                StartIndex = 0;
                LineCount = 0;
                ChatFont = null;
            }
        }
        #endregion
    }

    #region 照片框
    /// <summary>
    /// 照片框+小地图
    /// </summary>
    public sealed class PhotoControl : DXControl
    {
        public DXImageControl MapImage;
        public DXImageControl PhotoImage;

        public Dictionary<object, DXControl> MapInfoObjects = new Dictionary<object, DXControl>();
        public static float ScaleX, ScaleY;

        public PhotoControl()
        {
            MapImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MiniMap,
                ImageOpacity = 1.0F,
                Movable = false,  //可移动
                IgnoreMoveBounds = true,  //忽略移动边界
                Visible = false,
                ForeColour = Color.FromArgb(150, 150, 150)
            };
            MapImage.MouseClick += (o, e) =>
            {
                PhotoImage.Visible = true;
                MapImage.Visible = false;
            };

            PhotoImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.ProgUse,
                Index = 650,
                ImageOpacity = 1.0F,
                Movable = false,  //可移动
                IgnoreMoveBounds = true,  //忽略移动边界
                Visible = true,
            };
            PhotoImage.MouseClick += (o, e) =>
            {
                PhotoImage.Visible = false;
                MapImage.Visible = true;
            };
            PhotoImage.BeforeDraw += PhotoImage_BeforeDraw;

            //地图变更事件
            GameScene.Game.MapControl.MapInfoChanged += MapControl_MapInfoChanged;
        }

        #region methods
        public Point FixLocation(Point location)  //固定位置
        {
            if (location.Y > 0) location.Y = 0;
            if (location.X > 0) location.X = 0;
            if (location.X < -(MapImage.Size.Width - Size.Width)) location.X = -(MapImage.Size.Width - Size.Width);
            if (location.Y < -(MapImage.Size.Height - Size.Height)) location.Y = -(MapImage.Size.Height - Size.Height);
            return location;
        }

        /// <summary>
        /// 地图信息变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void MapControl_MapInfoChanged(object sender, EventArgs e)
        {
            foreach (DXControl temp in MapInfoObjects.Values)
                temp.Dispose();

            MapInfoObjects.Clear();

            if (GameScene.Game.MapControl.MapInfo == null) return;

            MapImage.Index = GameScene.Game.MapControl.MapInfo.MiniMap;
            MapImage.Size = new Size(MapImage.Size.Width, MapImage.Size.Height);

            //小地图窗口和地图全景的显示比例
            ScaleX = MapImage.Size.Width / (float)GameScene.Game.MapControl.Width;
            ScaleY = MapImage.Size.Height / (float)GameScene.Game.MapControl.Height;

            //地图背景最底层
            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Background, out MirLibrary library))
            {
                MirImage image = library.CreateImage(GameScene.Game.MapControl.MapInfo.Background, ImageType.Image);
                GameScene.Game.MapControl.BackgroundImage = image;
                if (image != null)
                {
                    GameScene.Game.MapControl.BackgroundScaleX = GameScene.Game.MapControl.Width * MapControl.CellWidth / (float)(image.Width - Config.GameSize.Width);
                    GameScene.Game.MapControl.BackgroundScaleY = GameScene.Game.MapControl.Height * MapControl.CellWidth / (float)(image.Height - Config.GameSize.Height);
                }
            }

            foreach (NPCInfo ob in Globals.NPCInfoList.Binding)
                Update(ob);

            foreach (MovementInfo ob in Globals.MovementInfoList.Binding)
                Update(ob);

            foreach (ClientObjectData ob in GameScene.Game.DataDictionary.Values)
                Update(ob);
        }

        /// <summary>
        /// 更新NPC信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(NPCInfo ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;

            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

                control = GameScene.Game.GetNPCControl(ob);
                control.Parent = MapImage;
                control.Opacity = MapImage.Opacity;

                MapInfoObjects[ob] = control;
            }
            else if ((QuestIcon)control.Tag == ob.CurrentIcon) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
            if (ob.Region?.Map != GameScene.Game.MapControl.MapInfo) return;

            control = GameScene.Game.GetNPCControl(ob);
            control.Parent = MapImage;
            control.Opacity = MapImage.Opacity;
            MapInfoObjects[ob] = control;

            if (ob.Region.PointList == null)
                ob.Region.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

            foreach (Point point in ob.Region.PointList)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int x = (minX + maxX) / 2;
            int y = (minY + maxY) / 2;

            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        /// <summary>
        /// 更新地图链接信息
        /// </summary>
        /// <param name="ob"></param>
        public void Update(MovementInfo ob)
        {
            if (ob.SourceRegion == null || ob.SourceRegion.Map != GameScene.Game.MapControl.MapInfo) return;
            if (ob.DestinationRegion?.Map == null || ob.Icon == MapIcon.None) return;

            if (ob.SourceRegion.PointList == null)
                ob.SourceRegion.CreatePoints(GameScene.Game.MapControl.Width);

            int minX = GameScene.Game.MapControl.Width, maxX = 0, minY = GameScene.Game.MapControl.Height, maxY = 0;

            foreach (Point point in ob.SourceRegion.PointList)
            {
                if (point.X < minX)
                    minX = point.X;
                if (point.X > maxX)
                    maxX = point.X;

                if (point.Y < minY)
                    minY = point.Y;
                if (point.Y > maxY)
                    maxY = point.Y;
            }

            int x = (minX + maxX) / 2;
            int y = (minY + maxY) / 2;

            DXImageControl control;
            MapInfoObjects[ob] = control = new DXImageControl
            {
                LibraryFile = LibraryFile.Interface,
                Parent = MapImage,
                Opacity = MapImage.Opacity,
                ImageOpacity = MapImage.Opacity,
                Hint = ob.DestinationRegion.Map.Description,
            };
            control.OpacityChanged += (o, e) => control.ImageOpacity = control.Opacity;

            switch (ob.Icon)  //地图各种标识图标
            {
                case MapIcon.Cave:
                    control.Index = 70;
                    control.ForeColour = Color.Red;
                    break;
                case MapIcon.Exit:
                    control.Index = 70;
                    control.ForeColour = Color.Green;
                    break;
                case MapIcon.Down:
                    control.Index = 500;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Up:
                    control.Index = 501;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Province:
                    control.Index = 101;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Building:
                    control.Index = 6124;
                    control.LibraryFile = LibraryFile.GameInter;
                    break;
                //各种入口图标
                case MapIcon.Entrance550:
                    control.Index = 550;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance551:
                    control.Index = 551;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance552:
                    control.Index = 552;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance553:
                    control.Index = 553;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance554:
                    control.Index = 554;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance555:
                    control.Index = 555;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance556:
                    control.Index = 556;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance557:
                    control.Index = 557;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance558:
                    control.Index = 558;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance559:
                    control.Index = 559;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance560:
                    control.Index = 560;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance561:
                    control.Index = 561;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance562:
                    control.Index = 562;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Entrance563:
                    control.Index = 563;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                //连接图标
                case MapIcon.Connect100:
                    control.Index = 100;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect102:
                    control.Index = 102;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect103:
                    control.Index = 103;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect104:
                    control.Index = 104;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect120:
                    control.Index = 120;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect121:
                    control.Index = 121;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect122:
                    control.Index = 122;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect123:
                    control.Index = 123;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect140:
                    control.Index = 140;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect141:
                    control.Index = 141;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect142:
                    control.Index = 142;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect143:
                    control.Index = 143;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect160:
                    control.Index = 160;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect161:
                    control.Index = 161;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect162:
                    control.Index = 162;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect300:
                    control.Index = 300;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect301:
                    control.Index = 301;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect302:
                    control.Index = 302;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect510:
                    control.Index = 510;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect511:
                    control.Index = 511;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect570:
                    control.Index = 570;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect571:
                    control.Index = 571;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
                case MapIcon.Connect572:
                    control.Index = 572;
                    control.LibraryFile = LibraryFile.WorldMap;
                    break;
            }
            //control.MouseClick += (o, e) => SelectedInfo = ob.DestinationRegion.Map;
            control.Location = new Point((int)(ScaleX * x) - control.Size.Width / 2, (int)(ScaleY * y) - control.Size.Height / 2);
        }
        /// <summary>
        /// 更新客户端数据
        /// </summary>
        /// <param name="ob"></param>
        public void Update(ClientObjectData ob)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control))
            {
                if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index) return;
                //if (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common) return;
                if (ob.ItemInfo != null) return;
                if (ob.MonsterInfo != null && ob.Dead) return;

                MapInfoObjects[ob] = control = new DXControl
                {
                    DrawTexture = true,
                    Parent = MapImage,
                    Opacity = MapImage.Opacity,
                    //MonsterInfo.AI < 0 ? Color.FromArgb(150, 200, 255) : Color.Red,
                };
            }
            else if (ob.MapIndex != GameScene.Game.MapControl.MapInfo.Index || (ob.MonsterInfo != null && ob.Dead) || (ob.ItemInfo != null && ob.ItemInfo.Rarity == Rarity.Common))
            {
                control.Dispose();
                MapInfoObjects.Remove(ob);
                return;
            }

            //小地图标记信息
            Size size = new Size(3, 3);
            Color colour = Color.White;
            string name = ob.Name;

            if (ob.MonsterInfo != null)
            {
                string _temname;
                // 只过滤结尾的数字
                _temname = ob.MonsterInfo.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                name = $"{_temname}";

                if (ob.MonsterInfo.AI < 0)
                {
                    colour = Color.LightBlue;
                }
                else
                {
                    colour = Color.Red;

                    if (GameScene.Game.HasQuest(ob.MonsterInfo, GameScene.Game.MapControl.MapInfo))
                        colour = Color.Orange;
                }

                if (ob.MonsterInfo.IsBoss)
                {
                    size = new Size(8, 8);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.Magenta,//colour,
                            DrawTexture = true,
                            Size = new Size(6, 6)
                        };
                    }
                    else
                        control.Controls[0].BackColour = Color.Magenta;

                    colour = Color.Pink;
                }

                if (!string.IsNullOrEmpty(ob.PetOwner))
                {
                    name += $" ({ob.PetOwner})";
                    control.DrawTexture = false;
                }
            }
            else if (ob.ItemInfo != null)
            {
                colour = Color.DarkBlue;
                //物品
            }
            else
            {
                if (MapObject.User.ObjectID == ob.ObjectID)
                {
                    //自己标记
                    size = new Size(5, 5);

                    if (control.Controls.Count == 0) // This is disgusting but its cheap
                    {
                        new DXControl
                        {
                            Parent = control,
                            Location = new Point(1, 1),
                            BackColour = Color.DarkOrange,
                            DrawTexture = true,
                            Size = new Size(3, 3)
                        };
                    }
                    //else
                    //	control.Controls[0].BackColour = Color.DarkOrange;
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Observer)
                {
                    control.Visible = false;
                }
                else if (GameScene.Game.GroupBox.Members.Any(p => p.ObjectID == ob.ObjectID))
                {
                    colour = Color.Lime;
                }
                else if (GameScene.Game.Partner != null && GameScene.Game.Partner.ObjectID == ob.ObjectID)
                {
                    colour = Color.DeepPink;
                }
                else if (GameScene.Game.GuildBox.GuildInfo != null && GameScene.Game.GuildBox.GuildInfo.Members.Any(p => p.ObjectID == ob.ObjectID))
                {
                    colour = Color.DeepSkyBlue;
                }
            }

            //小地图标记控件
            control.Hint = name;
            control.BackColour = colour;
            control.Size = size;
            control.Location = new Point((int)(ScaleX * ob.Location.X) - size.Width / 2, (int)(ScaleY * ob.Location.Y) - size.Height / 2);

            if (MapObject.User.ObjectID != ob.ObjectID) return;

            MapImage.Location = FixLocation(new Point(-control.Location.X + Size.Width / 2, -control.Location.Y + Size.Height / 2));
        }

        private void PhotoImage_BeforeDraw(object sender, EventArgs e)
        {
            MapObject mouseObject = GameScene.Game.MouseObject;
            int index;
            if (mouseObject == null || mouseObject.Race != ObjectType.Player)
            {
                index = (MapObject.User.Class == MirClass.Warrior && MapObject.User.Gender == MirGender.Male) ? 650 :
                        ((MapObject.User.Class == MirClass.Warrior && MapObject.User.Gender == MirGender.Female) ? 651 :
                        ((MapObject.User.Class == MirClass.Wizard && MapObject.User.Gender == MirGender.Male) ? 652 :
                        ((MapObject.User.Class == MirClass.Wizard && MapObject.User.Gender == MirGender.Female) ? 653 :
                        ((MapObject.User.Class == MirClass.Taoist && MapObject.User.Gender == MirGender.Male) ? 654 :
                        ((MapObject.User.Class == MirClass.Taoist && MapObject.User.Gender == MirGender.Female) ? 655 :
                        ((MapObject.User.Class == MirClass.Assassin && MapObject.User.Gender == MirGender.Male) ? 656 : 657))))));
            }
            else
            {
                MirClass @class = ((PlayerObject)GameScene.Game.MouseObject).Class;
                MirGender gender = ((PlayerObject)GameScene.Game.MouseObject).Gender;
                index = (@class == MirClass.Warrior && gender == MirGender.Male) ? 650 :
                        ((@class == MirClass.Warrior && gender == MirGender.Female) ? 651 :
                        ((@class == MirClass.Wizard && gender == MirGender.Male) ? 652 :
                        ((@class == MirClass.Wizard && gender == MirGender.Female) ? 653 :
                        ((@class == MirClass.Taoist && gender == MirGender.Male) ? 654 :
                        ((@class == MirClass.Taoist && gender == MirGender.Female) ? 655 :
                        ((@class == MirClass.Assassin && gender == MirGender.Male) ? 656 : 657))))));
            }
            PhotoImage.Index = index;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ob"></param>
        public void Remove(object ob)
        {
            DXControl control;

            if (!MapInfoObjects.TryGetValue(ob, out control)) return;

            control.Dispose();
            MapInfoObjects.Remove(ob);
        }

        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ScaleX = 0;
                ScaleY = 0;

                foreach (KeyValuePair<object, DXControl> pair in MapInfoObjects)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                MapInfoObjects.Clear();
                MapInfoObjects = null;


                if (PhotoImage != null)
                {
                    if (!PhotoImage.IsDisposed)
                        PhotoImage.Dispose();

                    PhotoImage = null;
                }

                if (MapImage != null)
                {
                    if (!MapImage.IsDisposed)
                        MapImage.Dispose();

                    MapImage = null;
                }
            }
        }
        #endregion
    }
    #endregion
}