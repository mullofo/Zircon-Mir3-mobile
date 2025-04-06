using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Library;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 聊天选项卡
    /// </summary>
    public sealed class ChatTab : DXTab
    {
        #region Properties
        public static List<ChatTab> Tabs = new List<ChatTab>();
        /// <summary>
        /// 左边的宽边
        /// </summary>
        public DXImageControl Left;
        /// <summary>
        /// 右边的缩放按钮
        /// </summary>
        public DXImageControl Right;
        /// <summary>
        /// 顶部横杆
        /// </summary>
        public DXImageControl Top;
        /// <summary>
        /// 信息图标
        /// </summary>
        public DXImageControl AlertIcon;
        /// <summary>
        /// 聊天选项面板
        /// </summary>
        public ChatOptionsPanel Panel;
        /// <summary>
        /// 聊天框里的文本面板
        /// </summary>
        public DXControl TextPanel;
        /// <summary>
        /// 聊天内容滚动条
        /// </summary>
        public DXVScrollBar ScrollBar;
        /// <summary>
        /// 聊天历史记录
        /// </summary>
        public List<DXLabel> History = new List<DXLabel>();
        /// <summary>
        /// 是否调整大小
        /// </summary>
        public bool IsResized = false;
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool Locked
        {
            get => _Locked;
            set
            {
                if (_Locked == value) return;
                bool oldValue = _Locked;
                _Locked = value;
                OnLockedChanged(oldValue, value);
            }
        }
        private bool _Locked;
        public event EventHandler<EventArgs> LockedChanged;
        public void OnLockedChanged(bool oValue, bool nValue)
        {
            CanResizeUp = !Locked;
            if (Right != null) Right.IsVisible = !Locked;
            LockedChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void OnSizeChanged(Size oValue, Size nValue)  //更改大小
        {
            base.OnSizeChanged(oValue, nValue);

            // DrawTexture = true;
            if (ScrollBar == null || TextPanel == null) return;

            TextPanel.Location = new Point(ResizeBuffer + 5, ResizeBuffer + 5);
            TextPanel.Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer * 2, Size.Height - ResizeBuffer * 2);

            if (Locked)
            {
                ScrollBar.Size = new Size(ScrollBar.Size.Width, Size.Height);
            }
            else
            {
                ScrollBar.Size = new Size(ScrollBar.Size.Width, Size.Height);
                ScrollBar.Location = new Point(ScrollBar.Location.X, ResizeBuffer * 2);
            }
            ScrollBar.VisibleSize = Size.Height;

            Top.Location = new Point(0, 2);
            Left.Location = new Point(0, 0);
            Right.Location = new Point(Size.Width - Right.Size.Width, 0);
            Right.Visible = !Locked;
            if (IsResized)
            {
                //拖动大小的时候不能让他通过
                ResizeChat();
            }
        }

        public override void OnIsResizingChanged(bool oValue, bool nValue)
        {
            if (!IsResizing)
            {
                ResizeChat();
                IsResized = true;
            }
            IsResized = IsResizing;
            base.OnIsResizingChanged(oValue, nValue);
        }

        public override void OnSelectedChanged(bool oValue, bool nValue)  //在选定更改时
        {
            base.OnSelectedChanged(oValue, nValue);

            if (Panel == null || CurrentTabControl == null || CurrentTabControl.SelectedTab != this) return;

            float opacity = Panel.TransparentCheckBox.Checked ? 0.5F : 1F;   //透明标签

            foreach (DXButton button in CurrentTabControl.TabButtons)
                button.Opacity = opacity;
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible && AlertIcon != null)
                AlertIcon.Visible = false;
        }

        #endregion

        /// <summary>
        /// 聊天选项卡
        /// </summary>
        public ChatTab()
        {
            Opacity = 0.5F;
            AllowResize = false;
            Locked = true;
            DrawOtherBorder = true;  //绘制其他边框

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Size = new Size(20, Size.Height - ResizeBuffer * 2),
                VisibleSize = Size.Height,
                Border = true,
                BorderColour = Color.Magenta,
            };
            ScrollBar.Location = new Point(Size.Width - ScrollBar.Size.Width - ResizeBuffer, 0);
            ScrollBar.ValueChanged += (o, e) => UpdateItems();

            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.UI1, 1149, -1, -1, 1207);

            TextPanel = new DXControl  //文本面板
            {
                Parent = this,
                PassThrough = true,
                Location = new Point(ResizeBuffer, ResizeBuffer),
                Size = new Size(Size.Width - ScrollBar.Size.Width - 1 - ResizeBuffer * 2, Size.Height),
            };

            TextPanel.MouseWheel += ScrollBar.DoMouseWheel;

            AlertIcon = new DXImageControl
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = TabButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            Top = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 0,
            };
            Left = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1215,
            };
            Right = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1218,
            };

            MouseWheel += ScrollBar.DoMouseWheel;
            Tabs.Add(this);
        }

        #region Methods

        public void ResizeChat() //调整聊天
        {
            if (!IsResizing)
            {
                foreach (DXLabel label in History)
                {
                    if (label.Size.Width == TextPanel.Size.Width) continue;

                    Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
                    label.Size = new Size(size.Width, size.Height);

                    //label.Size = new Size(TextPanel.Size.Width, DXLabel.GetHeight(label, TextPanel.Size.Width).Height);
                }

                UpdateItems();
                UpdateScrollBar();
            }
        }
        public void UpdateItems() //更新项目
        {
            int y = -ScrollBar.Value;

            foreach (DXLabel control in History)
            {
                control.Location = new Point(0, y);
                y += control.Size.Height + 2;
            }

        }

        public void UpdateScrollBar()  //更新滚动条
        {
            ScrollBar.VisibleSize = TextPanel.Size.Height;

            int height = 0;

            foreach (DXLabel control in History)
                height += control.Size.Height + 2;

            ScrollBar.MaxValue = height;
            if (Locked) ScrollBar.Size = new Size(ScrollBar.Size.Width, Size.Height);
            else ScrollBar.Size = new Size(ScrollBar.Size.Width, Size.Height);
        }

        public void ReceiveChat(string message, MessageType type)  //接收聊天
        {
            if (Panel == null) return;

            switch (type)
            {
                case MessageType.Normal: //普通文字
                    if (!Panel.LocalCheckBox.Checked) return;
                    break;
                case MessageType.Shout:  //区域聊天
                    if (!Panel.ShoutCheckBox.Checked) return;
                    break;
                case MessageType.Global: //世界喊话
                    if (!Panel.GlobalCheckBox.Checked) return;
                    break;
                case MessageType.Group:  //组队聊天
                    if (!Panel.GroupCheckBox.Checked) return;
                    break;
                case MessageType.Hint:  //提示信息
                    if (!Panel.HintCheckBox.Checked) return;
                    break;
                case MessageType.System:  //系统信息
                    if (!Panel.SystemCheckBox.Checked) return;
                    break;
                case MessageType.WhisperIn:   //私聊
                case MessageType.WhisperOut:  //收到私聊
                    if (!Panel.WhisperCheckBox.Checked) return;//不接受私聊
                    break;
                case MessageType.Combat:  //战斗信息
                    if (!Panel.GainsCheckBox.Checked) return;
                    break;
                case MessageType.ObserverChat:  //观察者信息
                    if (!Panel.ObserverCheckBox.Checked) return;
                    break;
                case MessageType.Guild:   //行会聊天
                    if (!Panel.GuildCheckBox.Checked) return;
                    break;
            }

            DXLabel label = new DXLabel
            {
                AutoSize = false,
                Text = message,   //文本=消息设置
                Outline = false,  //轮廓
                DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,   //文本格式   分词 | 单词省略号
                Parent = TextPanel,
            };
            label.MouseWheel += ScrollBar.DoMouseWheel;
            label.Tag = type;

            switch (type)
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
                        ChatWindow.InputBox?.StartPM(Regex.Replace(parts[0], "[^A-Za-z0-9\u4e00-\u9fa5]", ""));
                    };
                    label.MouseDoubleClick += (o, e) =>
                    {
                        //双击 支持中文
                        ChatWindow.InputBox?.SetInputText(label.Text);
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
                case MessageType.BossTips:		//boss提示 
                    label.MouseDoubleClick += (o, e) =>
                    {
                        //双击 支持中文
                        ChatWindow.InputBox?.SetInputText(label.Text);
                    };
                    break;
            }

            UpdateColours(label);  //更新标签颜色

            Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
            label.Size = new Size(size.Width, size.Height);

            History.Add(label);

            while (History.Count > 250)
            {
                DXLabel oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();

            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }
            else UpdateItems();

            if (Config.ChkAutoReplay)
            {
                GameScene.Game.BigPatchBox?.ReceiveChat(message, type);
            }
        }
        public void ReceiveChat(MessageAction action, params object[] args)  //接收聊天
        {
            DXLabel label;
            switch (action)
            {
                case MessageAction.Revive:    //复活 文本提示点击改为聊天框提示

                    DXMessageBox box = new DXMessageBox($"你死了, 点击这里在城里复活。", "复活", DXMessageBoxButtons.YesNo);

                    label = new DXLabel
                    {
                        AutoSize = false,
                        Text = "你死了, 点击这里在城里复活。",
                        Outline = false,
                        DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis,
                        Parent = TextPanel,
                    };
                    box.YesButton.MouseClick += (o, e) =>
                    {
                        CEnvir.Enqueue(new C.TownRevive());
                        label.Dispose();
                    };

                    label.MouseClick += (o, e) =>
                    {
                        CEnvir.Enqueue(new C.TownRevive());
                        label.Dispose();
                    };
                    label.MouseWheel += ScrollBar.DoMouseWheel;
                    label.Disposing += (o, e) =>
                    {
                        if (IsDisposed) return;

                        History.Remove(label);
                        UpdateScrollBar();
                        ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
                    };
                    label.Tag = MessageType.Announcement;
                    break;
                default:
                    return;
            }

            UpdateColours(label);

            Size size = DXLabel.GetHeight(label, TextPanel.Size.Width);
            label.Size = new Size(size.Width, size.Height);

            History.Add(label);

            while (History.Count > 250)
            {
                DXLabel oldLabel = History[0];
                History.Remove(oldLabel);
                oldLabel.Dispose();
            }

            AlertIcon.Visible = !IsVisible && Panel.AlertCheckBox.Checked;

            bool update = ScrollBar.Value >= ScrollBar.MaxValue - ScrollBar.VisibleSize;

            UpdateScrollBar();

            if (update)
            {
                ScrollBar.Value = ScrollBar.MaxValue - label.Size.Height;
            }
            else UpdateItems();
        }
        public void UpdateColours()
        {
            foreach (DXLabel label in History)
                UpdateColours(label);
        }
        private void UpdateColours(DXLabel label)  //更新聊天文字标签颜色
        {
            Color empty = Panel?.TransparentCheckBox.Checked == true ? Color.FromArgb(100, 0, 0, 0) : Color.Empty;  //透明标签

            switch ((MessageType)label.Tag)
            {
                case MessageType.Normal:      //普通文字
                    label.BackColour = empty;     //背景色 空值
                    label.ForeColour = Config.LocalTextColour;
                    break;
                case MessageType.Shout:      //区域聊天
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.ShoutTextColour;
                    break;
                case MessageType.Group:    //组队聊天
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.GroupTextColour;
                    break;
                case MessageType.Global:  //世界聊天
                    label.BackColour = empty;    //背景色 空值
                    label.ForeColour = Config.GlobalTextColour;  //文字颜色
                    break;
                case MessageType.Hint:   //提示信息
                    label.BackColour = Color.Red;   //背景色 红色
                    label.ForeColour = Config.HintTextColour;
                    break;
                case MessageType.System:   //系统信息
                    label.BackColour = Color.Red;//Color.FromArgb(200, 255, 255, 255);  //背景色 白色
                    label.ForeColour = Color.White;   //文字颜色 白色
                    break;
                case MessageType.Announcement:  //公告信息
                    label.BackColour = Color.FromArgb(255, 255, 130, 5);   //背景色 橙黄色
                    label.ForeColour = Color.White;   //文字颜色 白色
                    break;
                case MessageType.WhisperIn:   //私聊信息
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.WhisperInTextColour;
                    break;
                case MessageType.GMWhisperIn:  //GM私聊信息
                    label.BackColour = empty;//Color.FromArgb(200, 255, 255, 255);  //背景色 白色
                    label.ForeColour = Color.Lime;//Config.GMWhisperInTextColour;
                    break;
                case MessageType.WhisperOut:   //收到私聊信息
                    label.BackColour = empty;//背景色 空值
                    label.ForeColour = Config.WhisperOutTextColour;
                    break;
                case MessageType.Combat:   //战斗信息提示
                    label.BackColour = Color.Red;    //背景色 红色
                    label.ForeColour = Color.White;   //文字色 白色
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
					label.BackColour = Color.FromArgb(255, 150, 145, 255);   //背景色 蓝底
					label.ForeColour = Color.Black;          //文字颜色 黑色
                    break;
				case MessageType.ItemTips:          //极品物品提示
					label.BackColour = Color.Blue;     //背景色 蓝色
					label.ForeColour = Color.White;    //文字色 白色
					break;
				case MessageType.BossTips:          //boss提示
					label.BackColour = Color.Magenta;    //背景色 紫红色
					label.ForeColour = Color.White;      //文字色 白色
					break;
                case MessageType.RollNotice:          //走马灯中央滚动告示
                    label.BackColour = Color.Red;            //背景色 红底
                    label.ForeColour = Color.White;          //文字颜色 白色
                    break;
            }
        }
        public void TransparencyChanged()  //透明度更改
        {
            if (Panel.TransparentCheckBox.Checked)
            {
                ScrollBar.Visible = false;
                DrawTexture = false;
                DrawOtherBorder = false;
                AllowResize = false;

                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 0.5f;


                foreach (DXLabel label in History)
                    UpdateColours(label);
            }
            else
            {
                ScrollBar.Visible = true;
                DrawTexture = true;
                AllowResize = true;
                DrawOtherBorder = true;

                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 1f;
                
                foreach (DXLabel label in History)
                    UpdateColours(label);
            }
        }

        public void NameTagChanged()  //名字标签更改
        {
            if (Panel.NameTagCheckBox.Checked)
            {
                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 0f;

                foreach (DXLabel label in History)
                    UpdateColours(label);

                //不显示分页标签文本
                //TabButton.Text = null;
                TabButton.Visible = false;
            }
            else
            {
                if (CurrentTabControl.SelectedTab == this)
                    foreach (DXButton button in CurrentTabControl.TabButtons)
                        button.Opacity = 1f;

                foreach (DXLabel label in History)
                    UpdateColours(label);

                //不显示分页标签文本
                TabButton.Text = Panel.Text;
                TabButton.Visible = true;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Tabs.Remove(this);

                if (Panel != null)
                {
                    if (!Panel.IsDisposed)
                        Panel.Dispose();

                    Panel = null;
                }

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                        TextPanel.Dispose();

                    TextPanel = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

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

                if (AlertIcon != null)
                {
                    if (!AlertIcon.IsDisposed)
                        AlertIcon.Dispose();

                    AlertIcon = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 聊天窗口
    /// </summary>
    public sealed class ChatWindow : DXTabControl
    {
        #region Properties
        public Size DefaultSize;
        public DXImageControl Left, Right, Level, Upgrade;
        public DXLabel LevelLabel;
        public DXButton ExpendButton, PlugIn;
        public static DXVScrollBar ScrollBar;  //滚动条

        #region Tabs 
        public ChatTab DefaultsTab, LocalsTab, GroupsTab;
        public ChatTab GlobalsTab, GuildsTab, WhisperTab;
        #endregion

        #region ActivedTab
        public ChatTab ActivedTab
        {
            get => _ActivedTab;
            private set
            {
                if (_ActivedTab == value) return;

                ChatTab oldValue = _ActivedTab;
                _ActivedTab = value;
                OnActivedTabChanged(oldValue, value);
            }
        }
        private ChatTab _ActivedTab = null;
        public event EventHandler<EventArgs> ActivedTabChanged;
        public void OnActivedTabChanged(ChatTab oValue, ChatTab nValue)
        {
            if (ScrollBar != null && oValue != null)
            {
                ScrollBar.Parent = oValue;
                ScrollBar.Location = new Point(oValue.Size.Width - ScrollBar.Size.Width + 4, ResizeBuffer * 2);
                ScrollBar.Size = new Size(ScrollBar.Size.Width, ActivedTab.Size.Height);
                ScrollBar.SetSkin(LibraryFile.UI1, 1149, -1, -1, 1207);
            }

            ScrollBar = nValue.ScrollBar;
            if (ActivedTab.Locked)
            {
                ScrollBar.Parent = AvatarControl;
                ScrollBar.Size = new Size(ScrollBar.Size.Width, ActivedTab.Size.Height);
                ScrollBar.Location = new Point(AvatarControl.Size.Width - ScrollBar.Size.Width + 4, ResizeBuffer * 2);
                ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);
                if (InputBox != null)
                {
                    InputBox.Location = new Point(0, InputBox.Parent.Size.Height - InputBox.Size.Height);
                    InputBox.Size = new Size(InputBox.Parent.Size.Width - AvatarControl.Size.Width, InputBox.Size.Height);
                }
            }
            else
            {
                ScrollBar.Parent = ActivedTab;
                ScrollBar.Location = new Point(ActivedTab.Size.Width - ScrollBar.Size.Width + 4, ResizeBuffer * 2);
                ScrollBar.Size = new Size(ScrollBar.Size.Width, ActivedTab.Size.Height);
                ScrollBar.SetSkin(LibraryFile.UI1, 1149, -1, -1, 1207);
                if (InputBox != null)
                {
                    InputBox.Location = new Point(0, InputBox.Parent.Size.Height - InputBox.Size.Height);
                    InputBox.Size = new Size(InputBox.Parent.Size.Width, InputBox.Size.Height);
                }
            }
            ActivedTab.UpdateScrollBar();
            ActivedTabChanged?.Invoke(nValue, EventArgs.Empty);
        }
        #endregion

        #region AvatarControl
        /// <summary>
        /// 照片框
        /// </summary>
        public DXImageControl AvatarControl
        {
            get => _AvatarControl;
            set
            {
                if (_AvatarControl == value) return;
                DXImageControl oldValue = _AvatarControl;
                _AvatarControl = value;
            }
        }
        private DXImageControl _AvatarControl;
        #endregion

        #region InputBox
        /// <summary>
        /// 输入框
        /// </summary>
        public static ChatTextBox InputBox;
        /// <summary>
        /// 照片
        /// </summary>
        public DXImageControl Photo;
        #endregion        

        public override void OnSizeChanged(Size oValue, Size nValue)  //更改大小
        {
            base.OnSizeChanged(oValue, nValue);
            if (InputBox == null) return;
            if (ActivedTab == null) return;
            if (AvatarControl == null) return;

            int InputHeight = InputBox.Size.Height - 10;
            foreach (DXControl control in Controls)
            {
                DXTab tab = control as DXTab;

                if (tab == null || tab.Updating) continue;
                if (ActivedTab.Locked)
                {
                    control.Size = new Size(Size.Width - control.Location.X - AvatarControl.Size.Width, Size.Height - control.Location.Y - InputHeight);
                }
                else
                {
                    control.Size = new Size(Size.Width - control.Location.X, Size.Height - control.Location.Y - InputHeight);
                }
            }

            if (Config.GameSize.Width <= 1024)
            {
                GlobalsTab.TabButton.RightAligned = true;
                GuildsTab.TabButton.RightAligned = true;
                WhisperTab.TabButton.RightAligned = true;
            }
            else
            {
                GlobalsTab.TabButton.RightAligned = false;
                GuildsTab.TabButton.RightAligned = false;
                WhisperTab.TabButton.RightAligned = false;
            }
            if (ActivedTab.Locked)
            {
                InputBox.Size = new Size(Size.Width - AvatarControl.Size.Width, InputBox.Size.Height);
                AvatarControl.Location = new Point(Size.Width - AvatarControl.Size.Width, TabHeight);
            }
            else
            {
                InputBox.Size = new Size(Size.Width, InputBox.Size.Height);
            }
            InputBox.Location = new Point(Size.Width, Size.Height - InputBox.Size.Height);
            Left.Location = new Point(0, TabHeight);
            Right.Location = new Point(Size.Width - Right.Size.Width, TabHeight);
            Level.Location = new Point(Size.Width / 2 - Level.Size.Width / 2, -2);
            if (!IsResizing) TabsChanged();
        }
        public override void OnDisplayAreaChanged(Rectangle oValue, Rectangle nValue)
        {
            base.OnDisplayAreaChanged(oValue, nValue);

            if (Parent == null || InputBox == null) return;

            int InputHeight = InputBox.Size.Height - 10;
            foreach (DXControl control in Controls)
            {
                DXTab tab = control as DXTab;

                if (tab == null || tab.Updating) continue;

                if (ActivedTab.Locked)
                {
                    control.Size = new Size(Size.Width - control.Location.X - AvatarControl.Size.Width, Size.Height - control.Location.Y - InputHeight);
                }
                else
                {
                    control.Size = new Size(Size.Width - control.Location.X, Size.Height - control.Location.Y - InputHeight);
                }
            }
            InputBox.Location = new Point(0, Size.Height - InputBox.Size.Height);
            if (ActivedTab.Locked)
            {
                InputBox.Size = new Size(Size.Width - AvatarControl.Size.Width, InputBox.Size.Height);
                AvatarControl.Location = new Point(Size.Width - AvatarControl.Size.Width, TabHeight);
            }
            else
            {
                InputBox.Size = new Size(Size.Width, InputBox.Size.Height);
            }
            TabsChanged();
        }
        public override void OnSelectedTabChanged(DXTab oValue, DXTab nValue)
        {
            base.OnSelectedTabChanged(oValue, nValue);

            ActivedTab = nValue as ChatTab;
        }
        #endregion Properties

        public ChatWindow()
        {
            AllowResize = false;

            if (InputBox == null)
            {
                InputBox = new ChatTextBox
                {
                    Parent = this,
                    AllowResize = false,
                    Movable = false,
                };
                InputBox.Size = new Size(DefaultSize.Width, InputBox.Size.Height);
            }

            if (null == AvatarControl)
            {
                AvatarControl = new DXImageControl
                {
                    LibraryFile = LibraryFile.UI1,
                    Index = 1122,
                    Parent = this,
                };

                PlugIn = new DXButton   //大补帖按钮
                {
                    LibraryFile = LibraryFile.UI1,
                    Index = 1130,
                    Parent = AvatarControl,
                    Location = new Point(8, 113)
                };
                PlugIn.MouseClick += (o, e) => GameScene.Game.BigPatchBox.Visible = !GameScene.Game.BigPatchBox.Visible;

                Photo = new DXImageControl  //绘制各职业角色图片
                {
                    LibraryFile = LibraryFile.ProgUse,
                    Index = 650,
                    Parent = AvatarControl,
                    Location = new Point(5, 18),
                };
                Photo.AfterDraw += delegate
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
                                ((MapObject.User.Class != MirClass.Assassin || MapObject.User.Gender != 0) ? 657 : 656))))));
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
                                ((@class != MirClass.Assassin || gender != 0) ? 657 : 656))))));
                    }
                    MirImage mirImage = Photo.Library.CreateImage(index, ImageType.Image);
                    DXControl.PresentTexture(mirImage.Image, this, new Rectangle(Photo.DisplayArea.X, Photo.DisplayArea.Y, mirImage.Width, mirImage.Height), Color.White, Photo);
                };
            }

            DefaultsTab = AddChatTab("全部".Lang());
            DefaultsTab.Panel.AlertCheckBox.Checked = true;

            DefaultsTab.Panel.LocalCheckBox.Checked = true;
            DefaultsTab.Panel.WhisperCheckBox.Checked = true;
            DefaultsTab.Panel.GroupCheckBox.Checked = true;
            DefaultsTab.Panel.GuildCheckBox.Checked = true;
            DefaultsTab.Panel.ShoutCheckBox.Checked = true;//区域聊天
            DefaultsTab.Panel.GlobalCheckBox.Checked = true;
            DefaultsTab.Panel.ObserverCheckBox.Checked = true;
            DefaultsTab.Panel.SystemCheckBox.Checked = true;
            DefaultsTab.Panel.GainsCheckBox.Checked = true;//战斗聊天
            DefaultsTab.Panel.HintCheckBox.Checked = true;//信息提示

            LocalsTab = AddChatTab("本地"Lang());
            LocalsTab.Panel.LocalCheckBox.Checked = true;
            LocalsTab.Panel.ShoutCheckBox.Checked = true;//区域聊天
            LocalsTab.Panel.GainsCheckBox.Checked = true;//战斗聊天
            LocalsTab.Panel.HintCheckBox.Checked = true;//信息提示

            GroupsTab = AddChatTab("组队"Lang());
            GroupsTab.Panel.GroupCheckBox.Checked = true;
            GroupsTab.Panel.GainsCheckBox.Checked = true;
            GroupsTab.Panel.HintCheckBox.Checked = true;

            GlobalsTab = AddChatTab("公频"Lang());
            GlobalsTab.Panel.GlobalCheckBox.Checked = true;
            GlobalsTab.Panel.SystemCheckBox.Checked = true;

            GuildsTab = AddChatTab("行会"Lang());
            GuildsTab.Panel.GuildCheckBox.Checked = true;
            GuildsTab.Panel.HintCheckBox.Checked = true;
            GuildsTab.Panel.GainsCheckBox.Checked = true;//战斗聊天

            WhisperTab = AddChatTab("私聊"Lang());
            WhisperTab.Panel.WhisperCheckBox.Checked = true;
            WhisperTab.Panel.SystemCheckBox.Checked = true;

            ActivedTab = DefaultsTab;

            Left = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1215,
            };
            Right = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1218,
            };

            //等级
            Level = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1217,
            };

            Upgrade = new DXImageControl  //时间变化
            {
                Parent = Level,
                Location = new Point(46, 6),
                LibraryFile = LibraryFile.UI1,
                Index = 1200,
            };
            Upgrade.AfterDraw += delegate
            {
                int n = (GameScene.Game.MapControl.MapInfo.Light == LightSetting.Default) ? 1200 : ((GameScene.Game.MapControl.MapInfo.Light == LightSetting.Light) ? 1201 : ((GameScene.Game.MapControl.MapInfo.Light != LightSetting.Night) ? 1203 : 1202));
                MirImage mirImage2 = Upgrade.Library.CreateImage(n, ImageType.Image);
                DXControl.PresentTexture(mirImage2.Image, this, new Rectangle(Upgrade.DisplayArea.X, Upgrade.DisplayArea.Y, mirImage2.Width, mirImage2.Height), Color.White, Upgrade);
            };

            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Parent = Level,
                Font = new Font("Symbol", CEnvir.FontSize(8F), FontStyle.Bold),
                ForeColour = Color.Yellow,
                Location = new Point(48, 15),
                Size = new Size(30, 16),
                Hint = "等级"Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            ExpendButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Location = new Point(21, 1),
                Parent = Right,
                Index = 1168,
                Hint = "聊天记录窗口".Lang() + "(Ctrl+R, R)",
            };
            ExpendButton.MouseClick += (o, e) =>
            {
                LockPanel(!ActivedTab.Locked);
            };

            LockPanel(true);
        }

        #region Methods

        public void LockPanel(bool bLock)
        {
            DefaultsTab.Locked = bLock;
            LocalsTab.Locked = bLock;
            GroupsTab.Locked = bLock;
            GlobalsTab.Locked = bLock;
            GuildsTab.Locked = bLock;
            WhisperTab.Locked = bLock;
            AvatarControl.IsVisible = bLock;

            if (bLock)
            {
                ExpendButton.Hint = "聊天记录窗口".Lang();
                Size = new Size(Size.Width, DefaultSize.Height);
                Location = new Point(Location.X, GameScene.Game.Size.Height - DefaultSize.Height);

                ScrollBar.Parent = AvatarControl;
                ScrollBar.Size = new Size(ScrollBar.Size.Width, ActivedTab.Size.Height);
                ScrollBar.Location = new Point(AvatarControl.Size.Width - ScrollBar.Size.Width + 4, ResizeBuffer * 2);
                ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);
            }
            else
            {
                ExpendButton.Hint = "聊天记录窗口".Lang();
                Size = new Size(Size.Width, Size.Height + 200);
                Location = new Point(Location.X, Location.Y - 200);

                ScrollBar.Parent = ActivedTab;
                ScrollBar.Size = new Size(ScrollBar.Size.Width, ActivedTab.Size.Height);
                ScrollBar.Location = new Point(ActivedTab.Size.Width - ActivedTab.ScrollBar.Size.Width + 4, ResizeBuffer * 2);
                ScrollBar.SetSkin(LibraryFile.UI1, 1149, -1, -1, 1207);
            }
            ActivedTab.UpdateScrollBar();
        }
        private ChatTab AddChatTab(string TabName)
        {
            //聊天设置的设置面板
            ChatOptionsDialog Options = GameScene.Game.ChatOptionsBox;
            ChatOptionsPanel Pannel = Options.AddOptionsPanel(TabName);
            ChatTab Tab = new ChatTab  //聊天框
            {
                Parent = this,
                Locked = true,
                Opacity = 1F,
                Panel = Pannel,
                TabButton =
                {
                    Movable = true, 
                    AllowDragOut = false,
                    Label = { Text = Pannel?.Text },
                    Visible = false,        //是否显示页面分页            
                },
                AllowResize = true,
                CanResizeHeight = true,
                CanResizeWidth = false,
                CanResizeUp = false,
                CanResizeLeft = false,
                CanResizeRight = false,
                CanResizeDown = false,
            };

            if (Pannel != null)
            {
                Pannel.AlertCheckBox.Checked = true;
                Pannel.LocalCheckBox.Checked = false;
                Pannel.WhisperCheckBox.Checked = false;
                Pannel.GroupCheckBox.Checked = false;
                Pannel.GuildCheckBox.Checked = false;
                Pannel.ShoutCheckBox.Checked = false;
                Pannel.GlobalCheckBox.Checked = false;
                Pannel.ObserverCheckBox.Checked = false;
                Pannel.SystemCheckBox.Checked = false;
                Pannel.GainsCheckBox.Checked = false;
                Pannel.HintCheckBox.Checked = false;
            }
            return Tab;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (InputBox != null) InputBox.Dispose();
                InputBox = null;

                if (AvatarControl != null) AvatarControl.Dispose();
                AvatarControl = null;


                if (ExpendButton != null)
                {
                    if (!ExpendButton.IsDisposed)
                        ExpendButton.Dispose();

                    ExpendButton = null;
                }
                _ActivedTab = null;
            }
        }
        #endregion IDisposable
    }
}
