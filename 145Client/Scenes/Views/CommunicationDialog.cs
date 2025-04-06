using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 交流面板
    /// </summary>
    public sealed class CommunicationDialog : DXWindow
    {
        /// <summary>
        /// 背景框
        /// </summary>
        public DXImageControl BackGround;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton Close1Button;
        /// <summary>
        /// 好友按钮
        /// </summary>
        public DXButton GoodFriendButton;
        /// <summary>
        /// 收件按钮
        /// </summary>
        public DXButton ReceivingButton;
        /// <summary>
        /// 信件按钮
        /// </summary>
        public DXButton PostalMatterButton;
        /// <summary>
        /// 黑名单按钮
        /// </summary>
        public DXButton BlacklistButton;
        /// <summary>
        /// 好友面板按钮
        /// </summary>
        public DXImageControl FriendPanel;
        /// <summary>
        /// 收件面板按钮
        /// </summary>
        public DXImageControl ReceivingPanel;
        /// <summary>
        /// 信件面板按钮
        /// </summary>
        public DXImageControl PostalMatterPanel;
        /// <summary>
        /// 黑名单面板按钮
        /// </summary>
        public DXImageControl BlacklistPanel;

        #region 好友

        /// <summary>
        /// 好友列表框
        /// </summary>
        private FriendFunctionList FriendFunctionsList;
        /// <summary>
        /// 好友列表内容
        /// </summary>
        public List<ClientFriendInfo> FriendList = new List<ClientFriendInfo>();

        public FriendRow[] FriendRows;

        public FriendRow FriendHeader;

        public DXVScrollBar FriendScrollBar;

        private ClientFriendInfo SelectedFriend;

        private int selectedIndex = 0;

        public DXComboBox FriendStatusTypeBox, LookFriendTypeBox;


        public bool AllowFriend
        {
            get => _AllowFriend;
            set
            {
                if (_AllowFriend == value) return;

                bool oldValue = _AllowFriend;
                _AllowFriend = value;

                OnAllowGroupChanged(oldValue, value);
            }
        }
        private bool _AllowFriend;
        public event EventHandler<EventArgs> AllowFriendChanged;
        public void OnAllowGroupChanged(bool oValue, bool nValue)
        {
            AllowFriendChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region 黑名单
        /// <summary>
        /// 黑名单列表框
        /// </summary>
        private DXListBox ListBox;
        /// <summary>
        /// 黑名单列表内容
        /// </summary>
        public List<DXListBoxItem> ListBoxItems = new List<DXListBoxItem>();
        /// <summary>
        /// 黑名单增加移除按钮
        /// </summary>
        public DXButton addButton, removeButton;
        #endregion

        #region 发送邮件

        #region 收件人是否有效
        /// <summary>
        /// 收件人是否有效
        /// </summary>
        public bool RecipientValid
        {
            get => _RecipientValid;
            set
            {
                if (_RecipientValid == value) return;

                bool oldValue = _RecipientValid;
                _RecipientValid = value;

                OnRecipientValidChanged(oldValue, value);
            }
        }
        private bool _RecipientValid;
        public event EventHandler<EventArgs> RecipientValidChanged;
        public void OnRecipientValidChanged(bool oValue, bool nValue)
        {
            RecipientValidChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }

        #endregion

        #region 金币是否有效
        /// <summary>
        /// 金币是否有效
        /// </summary>
        public bool GoldValid
        {
            get => _GoldValid;
            set
            {
                if (_GoldValid == value) return;

                bool oldValue = _GoldValid;
                _GoldValid = value;

                OnGoldValidChanged(oldValue, value);
            }
        }
        private bool _GoldValid;
        public event EventHandler<EventArgs> GoldValidChanged;
        public void OnGoldValidChanged(bool oValue, bool nValue)
        {
            GoldValidChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }

        #endregion

        #region 尝试发送邮件
        /// <summary>
        /// 尝试发送邮件
        /// </summary>
        public bool SendAttempted
        {
            get => _SendAttempted;
            set
            {
                if (_SendAttempted == value) return;

                bool oldValue = _SendAttempted;
                _SendAttempted = value;

                OnSendAttemptedChanged(oldValue, value);
            }
        }
        private bool _SendAttempted;
        public event EventHandler<EventArgs> SendAttemptedChanged;
        public void OnSendAttemptedChanged(bool oValue, bool nValue)
        {
            SendAttemptedChanged?.Invoke(this, EventArgs.Empty);
            SendButton.Enabled = CanSend;
        }

        #endregion

        /// <summary>
        /// 发送判断
        /// </summary>
        public bool CanSend => !SendAttempted && RecipientValid && GoldValid;
        /// <summary>
        /// 收件人名字
        /// </summary>
        public DXTextBox RecipientBox;
        /// <summary>
        /// 发送信件主题
        /// </summary>
        public DXTextBox SubjectBox;
        /// <summary>
        /// 发送信件内容
        /// </summary>
        public DXTextBox MessageBox;
        /// <summary>
        /// 发送按钮
        /// </summary>
        public DXButton SendButton;
        /// <summary>
        /// 发送道具的格子
        /// </summary>
        public DXItemGrid Grid;
        /// <summary>
        /// 角色邮件道具
        /// </summary>
        public ClientUserItem[] MailItems;
        /// <summary>
        /// 金币输入(目前UI排版问题，暂时不用)
        /// </summary>
        public DXNumberBox GoldBox;

        #endregion

        #region 收件箱

        /// <summary>
        /// 签收邮件
        /// </summary>
        public DXButton CollectAllButton;
        /// <summary>
        /// 删除邮件
        /// </summary>
        public DXButton DeleteAll;
        /// <summary>
        /// 新建邮件
        /// </summary>
        public DXButton NewButton;
        /// <summary>
        /// 邮件行
        /// </summary>
        public MailRow Header;
        /// <summary>
        /// 邮件滚动条
        /// </summary>
        public DXVScrollBar ScrollBar;
        /// <summary>
        /// 邮件行信息
        /// </summary>
        public MailRow[] Rows;
        /// <summary>
        /// 客户端邮件列表信息
        /// </summary>
        public List<ClientMailInfo> MailList = new List<ClientMailInfo>();

        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!Visible && GameScene.Game.ReadMailBox != null)
                GameScene.Game.ReadMailBox.Visible = false;
        }
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (IsVisible)
                RefreshList();
            RefreshList_Friend();
        }

        #endregion

        #region Propetries
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public CommunicationDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0F;
            IgnoreMoveBounds = true;

            Size = UI1Library.GetSize(600);

            #region 界面

            BackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 600,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 1222;
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 1221;
            CloseButton.MouseClick += (o, e) => Visible = false;

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1422,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(108, 382),
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            FriendPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 601,
                Location = new Point(0, 60),
                Visible = true,
                ImageOpacity = 0.85F,
            };

            ReceivingPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 602,
                Location = new Point(0, 60),
                Visible = false,
                ImageOpacity = 0.85F,
            };

            PostalMatterPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 603,
                Location = new Point(0, 60),
                Visible = false,
                ImageOpacity = 0.85F,
            };

            BlacklistPanel = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 604,
                Location = new Point(0, 60),
                Visible = false,
                ImageOpacity = 0.85F,
            };

            GoodFriendButton = new DXButton
            {
                Index = 3610,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(9, 38),
                Parent = this,
                Opacity = 1F,
            };
            GoodFriendButton.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 1F;
                ReceivingButton.Opacity = 0F;
                PostalMatterButton.Opacity = 0F;
                BlacklistButton.Opacity = 0F;
                FriendPanel.Visible = true;
                ReceivingPanel.Visible = false;
                PostalMatterPanel.Visible = false;
                BlacklistPanel.Visible = false;
                Close1Button.Visible = true;
                SendButton.Visible = false;
                CollectAllButton.Visible = false;
                DeleteAll.Visible = false;
                NewButton.Visible = false;
            };

            ReceivingButton = new DXButton
            {
                Index = 3611,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(78, 38),
                Parent = this,
                Opacity = 0F,
            };
            ReceivingButton.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 0F;
                ReceivingButton.Opacity = 1F;
                PostalMatterButton.Opacity = 0F;
                BlacklistButton.Opacity = 0F;
                FriendPanel.Visible = false;
                ReceivingPanel.Visible = true;
                PostalMatterPanel.Visible = false;
                BlacklistPanel.Visible = false;
                Close1Button.Visible = false;
                SendButton.Visible = false;
                CollectAllButton.Visible = true;
                DeleteAll.Visible = true;
                NewButton.Visible = true;
            };

            PostalMatterButton = new DXButton
            {
                Index = 3612,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(147, 38),
                Parent = this,
                Opacity = 0F,
            };
            PostalMatterButton.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 0F;
                ReceivingButton.Opacity = 0F;
                PostalMatterButton.Opacity = 1F;
                BlacklistButton.Opacity = 0F;
                FriendPanel.Visible = false;
                ReceivingPanel.Visible = false;
                PostalMatterPanel.Visible = true;
                BlacklistPanel.Visible = false;
                Close1Button.Visible = false;
                SendButton.Visible = true;
                CollectAllButton.Visible = false;
                DeleteAll.Visible = false;
                NewButton.Visible = false;
            };

            BlacklistButton = new DXButton
            {
                Index = 3613,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(216, 38),
                Parent = this,
                Opacity = 0F,
            };
            BlacklistButton.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 0F;
                ReceivingButton.Opacity = 0F;
                PostalMatterButton.Opacity = 0F;
                BlacklistButton.Opacity = 1F;
                FriendPanel.Visible = false;
                ReceivingPanel.Visible = false;
                PostalMatterPanel.Visible = false;
                BlacklistPanel.Visible = true;
                Close1Button.Visible = true;
                SendButton.Visible = false;
                CollectAllButton.Visible = false;
                DeleteAll.Visible = false;
                NewButton.Visible = false;
            };

            #endregion

            #region 好友

            // 好友在线|离线排序
            FriendStatusTypeBox = new DXComboBox
            {
                Parent = FriendPanel,
                Location = new Point(150, 10),
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 50,
                Border = false,
            };
            new DXListBoxItem
            {
                Parent = FriendStatusTypeBox.ListBox,
                Label = { Text = "在线" },
                Item = 0
            };
            new DXListBoxItem
            {
                Parent = FriendStatusTypeBox.ListBox,
                Label = { Text = "离线" },
                Item = 1
            };
            FriendStatusTypeBox.ListBox.SelectItem(0);
            // 选择后的回调事件
            FriendStatusTypeBox.SelectedItemChanged += (o, e) =>
            {
                selectedIndex = (int)FriendStatusTypeBox.SelectedItem;
                RefreshList_Friend();
            };

            LookFriendTypeBox = new DXComboBox
            {
                Parent = FriendPanel,
                Location = new Point(150, 30),
                Size = new Size(122, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 50,
                Border = false,
            };
            new DXListBoxItem
            {
                Parent = LookFriendTypeBox.ListBox,
                Label = { Text = "查看全部好友" },
                Item = 0
            };
            LookFriendTypeBox.ListBox.SelectItem(0);

            DXControl Panel = new DXControl
            {
                Parent = FriendPanel,
                Size = new Size(285, 250),
                Location = new Point(12, 65),
            };

            // 好友rows
            FriendHeader = new FriendRow
            {
                Parent = Panel,
                IsHeader = true,
            };

            FriendRows = new FriendRow[12];
            FriendScrollBar = new DXVScrollBar
            {
                Parent = Panel,
                Size = new Size(20, 235),
                Location = new Point(ClientArea.Right - 32, ClientArea.Top - 15),
                VisibleSize = 12,
                Change = 3,
            };
            FriendScrollBar.ValueChanged += ScrollBar_ValueChanged_Friend;
            MouseWheel += FriendScrollBar.DoMouseWheel;
            //为滚动条自定义皮肤 -1为不设置
            FriendScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1207);

            //弹出菜单
            FriendFunctionsList = new FriendFunctionList
            {
                Parent = FriendPanel,
                Size = new Size(70, 60),
                Border = true,
                BorderColour = Color.FromArgb(65, 65, 65),
                BackColour = Color.FromArgb(65, 65, 65),
                Visible = false,
                PassThrough = false,
            };

            FriendFunctionListRow WhisperFunc = new FriendFunctionListRow
            {
                Parent = FriendFunctionsList,
                Label = { Text = "私聊" },
                Item = 0
            };
            WhisperFunc.MouseClick += (o, e) =>
            {
                GameScene.Game.ChatTextBox.TextBox.TextBox.Text = $"/{SelectedFriend.Name}";

                FriendFunctionsList.SelectedItem = null;
                SelectedFriend = null;

                FriendFunctionsList.Location = new Point(0, 0);
                FriendFunctionsList.Visible = false;
            };

            FriendFunctionListRow GroupFunc = new FriendFunctionListRow
            {
                Parent = FriendFunctionsList,
                Label = { Text = "组队" },
                Item = 1
            };
            GroupFunc.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.GroupInvite { Name = SelectedFriend.Name });
                FriendFunctionsList.SelectedItem = null;
                SelectedFriend = null;
                FriendFunctionsList.Location = new Point(0, 0);
                FriendFunctionsList.Visible = false;
            };

            FriendFunctionListRow WriteMailFunc = new FriendFunctionListRow
            {
                Parent = FriendFunctionsList,
                Label = { Text = "发送邮件" },
                Item = 2
            };
            WriteMailFunc.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 0F;
                ReceivingButton.Opacity = 0F;
                PostalMatterButton.Opacity = 1F;
                BlacklistButton.Opacity = 0F;
                FriendPanel.Visible = false;
                ReceivingPanel.Visible = false;
                PostalMatterPanel.Visible = true;
                BlacklistPanel.Visible = false;
                Close1Button.Visible = false;
                SendButton.Visible = true;
                CollectAllButton.Visible = false;
                DeleteAll.Visible = false;
                NewButton.Visible = false;
                RecipientBox.TextBox.Text = $"{SelectedFriend.Name}";

                FriendFunctionsList.SelectedItem = null;
                SelectedFriend = null;

                FriendFunctionsList.Location = new Point(0, 0);
                FriendFunctionsList.Visible = false;
            };

            FriendFunctionListRow DeleteFunc = new FriendFunctionListRow
            {
                Parent = FriendFunctionsList,
                Label = { Text = "删除好友" },
                Item = 2
            };
            DeleteFunc.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox($"确定要删除 {SelectedFriend.Name} 好友？", "删除好友", DXMessageBoxButtons.YesNo);
                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.FriendDeleteRequest { LinkID = SelectedFriend.LinkID });

                    FriendFunctionsList.SelectedItem = null;
                    SelectedFriend = null;
                };
                box.NoButton.MouseClick += (o1, e1) =>
                {
                    FriendFunctionsList.SelectedItem = null;
                    SelectedFriend = null;
                };

                FriendFunctionsList.Location = new Point(0, 0);
                FriendFunctionsList.Visible = false;
            };

            for (int i = 0; i < 12; i++)
            {
                int index = i;
                FriendRows[index] = new FriendRow
                {
                    Parent = Panel,
                    Location = new Point(5, 20 * i + 2),
                    Visible = true
                };
                FriendRows[index].MouseClick += (o, e) =>
                {
                    SelectedFriend = FriendList[index + FriendScrollBar.Value];
                    FriendFunctionsList.Location = new Point(CEnvir.MouseLocation.X - this.Location.X - 20, CEnvir.MouseLocation.Y - this.Location.Y - 125);
                    FriendFunctionsList.Visible = true;
                };
                FriendRows[index].MouseWheel += FriendScrollBar.DoMouseWheel;
            }

            #endregion

            #region 黑名单

            ListBox = new DXListBox
            {
                Parent = BlacklistPanel,
                Location = new Point(12, 65),
                Size = new Size(248, 238),
                Border = false,
            };

            addButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3642,
                Parent = BlacklistPanel,
                Location = new Point(40, 15),
            };
            addButton.MouseClick += (o, e) =>
            {
                new DXInputWindow((str) =>
                {
                    return Globals.CharacterReg.IsMatch(str);
                }, (str) =>
                {
                    CEnvir.Enqueue(new C.BlockAdd { Name = str });
                }, "输入你想添加进黑名单的玩家名字\n如果该玩家是好友，将自动解除好友关系".Lang());
            };

            removeButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3647,
                Parent = BlacklistPanel,
                Location = new Point(170, 15),
                Enabled = false,
            };
            removeButton.MouseClick += (o, e) =>
            {
                if (ListBox.SelectedItem == null) return;

                DXMessageBox box = new DXMessageBox($"character.removeBalck".Lang(ListBox.SelectedItem.Label.Text), "移除".Lang(), DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    CEnvir.Enqueue(new C.BlockRemove { Index = (int)ListBox.SelectedItem.Item });
                    RefreshBlockList();
                };
            };

            ListBox.selectedItemChanged += (o, e) =>
            {
                if (ListBox.SelectedItem == null) return;

                removeButton.Enabled = ListBox.SelectedItem != null;
            };

            RefreshBlockList();

            #endregion

            #region 发送邮件

            RecipientBox = new DXTextBox  //收件人输入框
            {
                Border = true,
                BackColour = Color.FromArgb(0, 16, 8, 8),
                BorderColour = Color.FromArgb(0, 16, 8, 8),
                Parent = PostalMatterPanel,
                Location = new Point(84, 12),
                Size = new Size(118, 15)
            };
            RecipientBox.TextBox.TextChanged += RecipientBox_TextChanged;

            SubjectBox = new DXTextBox   //主题输入框
            {
                Border = false,
                BackColour = Color.FromArgb(0, 16, 8, 8),
                Parent = PostalMatterPanel,
                Location = new Point(84, 31),
                Size = new Size(156, 15),
                MaxLength = 12,
            };

            MessageBox = new DXTextBox   //内容输入框
            {
                Border = false,
                BackColour = Color.FromArgb(0, 16, 8, 8),
                Parent = PostalMatterPanel,
                TextBox = { Multiline = true, AcceptsReturn = true, },
                Location = new Point(15, 55),
                Size = new Size(242, 195),
                MaxLength = 300,
                Opacity = 0.1F,
            };

            MailItems = new ClientUserItem[7];  //道具格子定义数量7个

            Grid = new DXItemGrid   //道具格子
            {
                GridSize = new Size(7, 1),
                Parent = PostalMatterPanel,
                Location = new Point(16, 263),
                Linked = true,
                GridType = GridType.SendMail,
            };

            GoldBox = new DXNumberBox   //金币输入框  暂时不用，所以默认不显示
            {
                Parent = PostalMatterPanel,
                Location = new Point(8, 315),
                UpButton = { Visible = false },
                DownButton = { Visible = false },
                Size = new Size(102, 18),
                ValueTextBox = { Location = new Point(1, 1), Size = new Size(100, 16) },
                MaxValue = 2000000000,
                Visible = false,
            };
            GoldBox.ValueTextBox.ValueChanged += GoldBox_ValueChanged;

            SendButton = new DXButton    //发送按钮
            {
                Index = 3667,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(108, 382),
                Parent = this,
                Enabled = false,
                Visible = false,
            };
            SendButton.MouseClick += (o, e) => Send();

            GoldValid = true;

            #endregion

            #region 邮件面板

            Header = new MailRow   //邮件行
            {
                Parent = ReceivingPanel,
                Location = new Point(ClientArea.Location.X + 5, ClientArea.Location.Y),
                IsHeader = true,
            };
            Rows = new MailRow[5];  //行设置15行

            ScrollBar = new DXVScrollBar        //滚动条
            {
                Parent = ReceivingPanel,
                Size = new Size(14, 315),
                Location = new Point(ClientArea.Right - 19, ClientArea.Top - 8),
                VisibleSize = 5,
                Change = 3,
            };
            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.GameInter, -1, -1, -1, 1225);
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            MouseWheel += ScrollBar.DoMouseWheel;

            DXControl panel = new DXControl
            {
                Parent = ReceivingPanel,
                Location = new Point(ClientArea.Location.X + 2, ClientArea.Location.Y + 33),
                Size = new Size(ClientArea.Width - 27, ClientArea.Size.Height - 50)
            };

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                Rows[index] = new MailRow
                {
                    Parent = panel,
                    Location = new Point(3, 2 + 49 * i),
                    Visible = false,
                };
                Rows[index].MouseClick += (o, e) =>
                {
                    GameScene.Game.ReadMailBox.Mail = Rows[index].Mail;

                    if (Rows[index].Mail.Opened) return;

                    Rows[index].Mail.Opened = true;
                    Rows[index].RefreshIcon();
                    UpdateIcon();

                    CEnvir.Enqueue(new C.MailOpened { Index = Rows[index].Mail.Index });
                };
                Rows[index].MouseWheel += ScrollBar.DoMouseWheel;
            }

            CollectAllButton = new DXButton
            {
                Location = new Point(ClientArea.Right - 180, Size.Height - 41),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "签收邮件".Lang() },
                Visible = false,
            };
            CollectAllButton.MouseClick += (o, e) =>
            {
                if (!MapObject.User.InSafeZone) return;
                int count = 5;
                foreach (ClientMailInfo mail in MailList)
                {
                    if (count <= 0) break;

                    if (mail.Items.Count == 0) continue;

                    if (!mail.Opened)
                    {
                        mail.Opened = true;
                        CEnvir.Enqueue(new C.MailOpened { Index = mail.Index });
                        count--;
                        foreach (MailRow row in Rows)
                        {
                            if (row.Mail != mail) continue;
                            row.RefreshIcon();
                            break;
                        }
                    }
                    foreach (ClientUserItem item in mail.Items)
                        CEnvir.Enqueue(new C.MailGetItem { Index = mail.Index, Slot = item.Slot });
                    count--;
                }
                UpdateIcon();
            };

            DeleteAll = new DXButton
            {
                Location = new Point(ClientArea.Right - 90, Size.Height - 41),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "删除邮件".Lang() },
                Visible = false,
            };
            DeleteAll.MouseClick += (o, e) =>
            {
                int count = 5;
                foreach (ClientMailInfo mail in MailList)
                {
                    if (count <= 0) break;
                    if (mail.Items.Count > 0) continue;

                    CEnvir.Enqueue(new C.MailDelete { Index = mail.Index });
                    count--;
                }
            };

            NewButton = new DXButton
            {
                Location = new Point(ClientArea.Right - 270, Size.Height - 41),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "新建邮件".Lang() },
                Visible = false,
            };
            NewButton.MouseClick += (o, e) =>
            {
                GoodFriendButton.Opacity = 0F;
                ReceivingButton.Opacity = 0F;
                PostalMatterButton.Opacity = 1F;
                BlacklistButton.Opacity = 0F;
                FriendPanel.Visible = false;
                ReceivingPanel.Visible = false;
                PostalMatterPanel.Visible = true;
                BlacklistPanel.Visible = false;
                Close1Button.Visible = false;
                SendButton.Visible = true;
                CollectAllButton.Visible = false;
                DeleteAll.Visible = false;
                NewButton.Visible = false;
            };

            #endregion

        }

        #region 刷新黑名单列表
        /// <summary>
        /// 刷新黑名单列表
        /// </summary>
        public void RefreshBlockList()
        {
            ListBox.SelectedItem = null;

            foreach (DXListBoxItem item in ListBoxItems)
                item.Dispose();

            ListBoxItems.Clear();

            foreach (ClientBlockInfo info in CEnvir.BlockList)
            {
                ListBoxItems.Add(new DXListBoxItem
                {
                    Parent = ListBox,
                    Label = { Text = info.Name },
                    Item = info.Index
                });
            }
        }
        #endregion

        #region 发送信件部分
        /// <summary>
        /// 金额改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoldBox_ValueChanged(object sender, EventArgs e)
        {
            GoldValid = GoldBox.Value >= 0 && GoldBox.Value <= MapObject.User.Gold;

            if (GoldBox.Value == 0)
                GoldBox.ValueTextBox.BorderColour = Color.FromArgb(198, 166, 99);
            else
                GoldBox.ValueTextBox.BorderColour = GoldValid ? Color.Green : Color.Red;
        }
        /// <summary>
        /// 收件人改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecipientBox_TextChanged(object sender, EventArgs e)
        {
            RecipientValid = Globals.CharacterReg.IsMatch(RecipientBox.TextBox.Text);

            if (string.IsNullOrEmpty(RecipientBox.TextBox.Text))
                RecipientBox.BorderColour = Color.FromArgb(0, 16, 8, 8);
            else
                RecipientBox.BorderColour = RecipientValid ? Color.FromArgb(0, 16, 8, 8) : Color.Red;
        }
        /// <summary>
        /// 发送
        /// </summary>
        public void Send()
        {
            SendAttempted = true;

            List<CellLinkInfo> links = new List<CellLinkInfo>();

            foreach (DXItemCell cell in Grid.Grid)
            {
                if (cell.Link == null) continue;

                links.Add(new CellLinkInfo { Count = cell.LinkedCount, GridType = cell.Link.GridType, Slot = cell.Link.Slot });

                cell.Link.Locked = true;
                cell.Link = null;
            }

            CEnvir.Enqueue(new C.MailSend { Links = links, Recipient = RecipientBox.TextBox.Text, Subject = SubjectBox.TextBox.Text, Message = MessageBox.TextBox.Text, Gold = GoldBox.Value });

            RecipientBox.TextBox.Text = string.Empty;
            MessageBox.TextBox.Text = string.Empty;
            SubjectBox.TextBox.Text = string.Empty;

            GoldBox.Value = 0;
        }

        #endregion

        #region 邮件面板
        /// <summary>
        /// 滚动条变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        public void RefreshList()
        {
            if (Rows == null) return;

            MailList.Sort((x1, x2) =>
            {
                int value = x2.Date.CompareTo(x1.Date);

                if (value == 0)
                    return x1.Index.CompareTo(x2.Index);
                return value;
            });

            ScrollBar.MaxValue = MailList.Count;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (i + ScrollBar.Value >= MailList.Count)
                {
                    Rows[i].Mail = null;
                    continue;
                }

                Rows[i].Mail = MailList[i + ScrollBar.Value];
            }
        }
        /// <summary>
        /// 更新图标
        /// </summary>
        public void UpdateIcon()
        {
            GameScene.Game.MainPanel.NewMailIcon.Visible = MailList.Any(x => !x.Opened);
            if (GameScene.Game.MainPanel.NewMailIcon.Visible == true)
                DXSoundManager.Play(SoundIndex.news);  //收到信件音乐提示
        }

        /// <summary>
        /// 滚动条更新好友列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBar_ValueChanged_Friend(object sender, EventArgs e)
        {
            RefreshList_Friend();
        }
        /// <summary>
        /// 刷新好友列表
        /// </summary>
        public void RefreshList_Friend()
        {
            if (FriendRows == null) return;

            FriendList.Sort((x1, x2) =>
            {
                int value = x2.AddDate.CompareTo(x1.AddDate);

                if (value == 0)
                    return x1.Index.CompareTo(x2.Index);
                return value;
            });

            // 根据选项做排序 0在线优先排序 1离线优先排序
            // 1 为在线 -1 为离线
            int sortType = selectedIndex == 0 ? 1 : -1;
            // 分拣指定排序模式
            FriendList.Sort((x1, x2) =>
            {
                int value = x2.Online.CompareTo(x1.Online);
                return value * sortType;
            });

            FriendScrollBar.MaxValue = FriendList.Count;

            for (int i = 0; i < FriendRows.Length; i++)
            {
                if (i + FriendScrollBar.Value >= FriendList.Count)
                {
                    FriendRows[i].Friend = null;
                    continue;
                }

                FriendRows[i].Friend = FriendList[i + FriendScrollBar.Value];
                FriendRows[i].NameLabel.ForeColour = FriendRows[i].Friend.Online ? Color.White : Color.DarkGray;
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BackGround != null)
                {
                    if (!BackGround.IsDisposed)
                        BackGround.Dispose();

                    BackGround = null;
                }

                if (Close1Button != null)
                {
                    if (!Close1Button.IsDisposed)
                        Close1Button.Dispose();

                    Close1Button = null;
                }

                if (GoodFriendButton != null)
                {
                    if (!GoodFriendButton.IsDisposed)
                        GoodFriendButton.Dispose();

                    GoodFriendButton = null;
                }

                if (ReceivingButton != null)
                {
                    if (!ReceivingButton.IsDisposed)
                        ReceivingButton.Dispose();

                    ReceivingButton = null;
                }

                if (PostalMatterButton != null)
                {
                    if (!PostalMatterButton.IsDisposed)
                        PostalMatterButton.Dispose();

                    PostalMatterButton = null;
                }

                if (BlacklistButton != null)
                {
                    if (!BlacklistButton.IsDisposed)
                        BlacklistButton.Dispose();

                    BlacklistButton = null;
                }

                if (FriendPanel != null)
                {
                    if (!FriendPanel.IsDisposed)
                        FriendPanel.Dispose();

                    FriendPanel = null;
                }

                if (FriendFunctionsList != null)
                {
                    if (!FriendFunctionsList.IsDisposed)
                        FriendFunctionsList.Dispose();

                    FriendFunctionsList = null;
                }

                if (ReceivingPanel != null)
                {
                    if (!ReceivingPanel.IsDisposed)
                        ReceivingPanel.Dispose();

                    ReceivingPanel = null;
                }

                if (PostalMatterPanel != null)
                {
                    if (!PostalMatterPanel.IsDisposed)
                        PostalMatterPanel.Dispose();

                    PostalMatterPanel = null;
                }

                if (BlacklistPanel != null)
                {
                    if (!BlacklistPanel.IsDisposed)
                        BlacklistPanel.Dispose();

                    BlacklistPanel = null;
                }

                if (ListBoxItems != null)
                {
                    for (int i = 0; i < ListBoxItems.Count; i++)
                    {
                        if (ListBoxItems[i] != null)
                        {
                            if (!ListBoxItems[i].IsDisposed)
                                ListBoxItems[i].Dispose();

                            ListBoxItems[i] = null;
                        }
                    }

                    ListBoxItems.Clear();
                    ListBoxItems = null;
                }

                if (ListBox != null)
                {
                    if (!ListBox.IsDisposed)
                        ListBox.Dispose();

                    ListBox = null;
                }

                if (addButton != null)
                {
                    if (!addButton.IsDisposed)
                        addButton.Dispose();

                    addButton = null;
                }

                if (removeButton != null)
                {
                    if (!removeButton.IsDisposed)
                        removeButton.Dispose();

                    removeButton = null;
                }

                _RecipientValid = false;
                RecipientValidChanged = null;

                _GoldValid = false;
                GoldValidChanged = null;

                _SendAttempted = false;
                SendAttemptedChanged = null;

                if (RecipientBox != null)
                {
                    if (!RecipientBox.IsDisposed)
                        RecipientBox.Dispose();

                    RecipientBox = null;
                }

                if (SubjectBox != null)
                {
                    if (!SubjectBox.IsDisposed)
                        SubjectBox.Dispose();

                    SubjectBox = null;
                }

                if (MessageBox != null)
                {
                    if (!MessageBox.IsDisposed)
                        MessageBox.Dispose();

                    MessageBox = null;
                }

                if (GoldBox != null)
                {
                    if (!GoldBox.IsDisposed)
                        GoldBox.Dispose();

                    GoldBox = null;
                }

                if (SendButton != null)
                {
                    if (!SendButton.IsDisposed)
                        SendButton.Dispose();

                    SendButton = null;
                }

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                MailItems = null;

                if (CollectAllButton != null)
                {
                    if (!CollectAllButton.IsDisposed)
                        CollectAllButton.Dispose();

                    CollectAllButton = null;
                }

                if (DeleteAll != null)
                {
                    if (!DeleteAll.IsDisposed)
                        DeleteAll.Dispose();

                    DeleteAll = null;
                }

                if (NewButton != null)
                {
                    if (!NewButton.IsDisposed)
                        NewButton.Dispose();

                    NewButton = null;
                }

                if (Header != null)
                {
                    if (!Header.IsDisposed)
                        Header.Dispose();

                    Header = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (NewButton != null)
                {
                    if (!NewButton.IsDisposed)
                        NewButton.Dispose();

                    NewButton = null;
                }

                if (FriendScrollBar != null)
                {
                    if (!FriendScrollBar.IsDisposed)
                        FriendScrollBar.Dispose();

                    FriendScrollBar = null;
                }

                if (FriendStatusTypeBox != null)
                {
                    if (!FriendStatusTypeBox.IsDisposed)
                        FriendStatusTypeBox.Dispose();

                    FriendStatusTypeBox = null;
                }

                if (LookFriendTypeBox != null)
                {
                    if (!LookFriendTypeBox.IsDisposed)
                        LookFriendTypeBox.Dispose();

                    LookFriendTypeBox = null;
                }

                if (Rows != null)
                {
                    for (int i = 0; i < Rows.Length; i++)
                    {
                        if (Rows[i] != null)
                        {
                            if (!Rows[i].IsDisposed)
                                Rows[i].Dispose();

                            Rows[i] = null;
                        }
                    }

                    Rows = null;
                }
                MailList.Clear();
                MailList = null;

                if (FriendRows != null)
                {
                    for (int i = 0; i < FriendRows.Length; i++)
                    {
                        if (FriendRows[i] != null)
                        {
                            if (!FriendRows[i].IsDisposed)
                                FriendRows[i].Dispose();

                            FriendRows[i] = null;
                        }
                    }

                    FriendRows = null;
                }
                FriendList.Clear();
                FriendList = null;
            }
        }
        #endregion
    }

    #region 邮件行内容
    /// <summary>
    /// 邮件行
    /// </summary>
    public sealed class MailRow : DXControl
    {
        #region Properties

        #region 行标题
        /// <summary>
        /// 行标题
        /// </summary>
        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            Icon.Visible = false;
            DrawTexture = false;
            IsControl = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region 邮件信息
        /// <summary>
        /// 邮件信息
        /// </summary>
        public ClientMailInfo Mail
        {
            get => _Mail;
            set
            {
                ClientMailInfo oldValue = _Mail;
                _Mail = value;

                OnMailChanged(oldValue, value);
            }
        }
        private ClientMailInfo _Mail;
        public event EventHandler<EventArgs> MailChanged;
        public void OnMailChanged(ClientMailInfo oValue, ClientMailInfo nValue)
        {
            Visible = nValue != null;
            if (nValue == null) return;

            SenderLabel.Text = Mail.Sender;
            DateLabel.Text = Mail.Date.ToShortDateString();

            RefreshIcon();

            MailChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// 图标显示
        /// </summary>
        public DXImageControl Icon;
        /// <summary>
        /// 发件人名字标签
        /// </summary>
        public DXLabel SenderLabel;
        /// <summary>
        /// 发件日期标签
        /// </summary>
        public DXLabel DateLabel;
        #endregion

        /// <summary>
        /// 邮件行
        /// </summary>
        public MailRow()
        {
            Size = new Size(245, 50);
            DrawTexture = true;
            Border = true;
            BorderColour = Color.FromArgb(0, 25, 20, 0);

            Icon = new DXImageControl    //图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3680,
                IsControl = false,
                Location = new Point(8, 6),
            };

            SenderLabel = new DXLabel   //发件人标签
            {
                AutoSize = false,
                Size = new Size(135, 50),
                Location = new Point(50, 0),
                ForeColour = Color.FromArgb(120, 120, 120),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                IsControl = false,
            };

            DateLabel = new DXLabel    //日期
            {
                AutoSize = false,
                Size = new Size(60, 50),
                Location = new Point(SenderLabel.Location.X + SenderLabel.Size.Width + 2, 0),
                ForeColour = Color.FromArgb(120, 120, 120),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (IsHeader) return;

            BorderColour = Color.Yellow;
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (IsHeader) return;

            BorderColour = Color.FromArgb(0, 80, 80, 125);
        }
        /// <summary>
        /// 刷新图标
        /// </summary>
        public void RefreshIcon()
        {
            if (Mail.HasItem)
                Icon.Index = Mail.Items.Count == 0 ? 3679 : 3681;
            else
                Icon.Index = Mail.Opened ? 3679 : 3680;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _IsHeader = false;
                IsHeaderChanged = null;

                _Mail = null;
                MailChanged = null;

                if (Icon != null)
                {
                    if (!Icon.IsDisposed)
                        Icon.Dispose();

                    Icon = null;
                }

                if (SenderLabel != null)
                {
                    if (!SenderLabel.IsDisposed)
                        SenderLabel.Dispose();

                    SenderLabel = null;
                }

                if (DateLabel != null)
                {
                    if (!DateLabel.IsDisposed)
                        DateLabel.Dispose();

                    DateLabel = null;
                }
            }
        }
        #endregion
    }
    #endregion

    #region 阅读邮件
    /// <summary>
    /// 阅读邮件功能
    /// </summary>
    public sealed class ReadMailDialog : DXWindow
    {
        #region Properties

        #region 邮件信息
        /// <summary>
        /// 邮件信息
        /// </summary>
        public ClientMailInfo Mail
        {
            get => _Mail;
            set
            {
                ClientMailInfo oldValue = _Mail;
                _Mail = value;

                OnMailChanged(oldValue, value);
            }
        }
        private ClientMailInfo _Mail;
        public event EventHandler<EventArgs> MailChanged;
        public void OnMailChanged(ClientMailInfo oValue, ClientMailInfo nValue)
        {
            Visible = Mail != null;

            if (Mail == null) return;

            SenderBox.TextBox.Text = Mail.Sender;
            SubjectBox.TextBox.Text = Mail.Subject;
            DateBox.TextBox.Text = Mail.Date.ToLongDateString() + " " + Mail.Date.ToLongTimeString();
            MessageLabel.Text = Mail.Message;

            foreach (DXItemCell cell in Grid.Grid)
                cell.Item = Mail.Items.FirstOrDefault(x => x.Slot == cell.Slot);

            MailChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// 收信界面背景
        /// </summary>
        public DXImageControl ReadMailBackGround;
        /// <summary>
        /// 发送者信息
        /// </summary>
        public DXTextBox SenderBox;
        /// <summary>
        /// 主题信息
        /// </summary>
        public DXTextBox SubjectBox;
        /// <summary>
        /// 日期信息
        /// </summary>
        public DXTextBox DateBox;
        /// <summary>
        /// 内容信息
        /// </summary>
        public DXLabel MessageLabel;
        /// <summary>
        /// 回复
        /// </summary>
        public DXButton ReplyButton;
        /// <summary>
        /// 删除
        /// </summary>
        public DXButton DeleteButton;
        /// <summary>
        /// 道具格子
        /// </summary>
        public DXItemGrid Grid;
        /// <summary>
        /// 邮件道具
        /// </summary>
        public ClientUserItem[] MailItems;

        public override WindowType Type => WindowType.ReadMailBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 阅读邮件界面
        /// </summary>
        public ReadMailDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 1F;
            IgnoreMoveBounds = true;

            Size = GameInterLibrary.GetSize(3606);

            ReadMailBackGround = new DXImageControl   //背景图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3606,
                Opacity = 1F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            SenderBox = new DXTextBox    //发件人信息栏
            {
                Border = false,
                BorderColour = Color.FromArgb(0, 198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(84, 53),
                Size = new Size(118, 15)
            };

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 113,
                LibraryFile = LibraryFile.Interface,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 112;
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 113;
            CloseButton.MouseClick += (o, e) => Visible = false;

            SubjectBox = new DXTextBox    //主题信息栏
            {
                Border = false,
                BorderColour = Color.FromArgb(0, 198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(84, 72),
                Size = new Size(156, 15),
            };

            DateBox = new DXTextBox     //时间信息栏
            {
                Border = false,
                BorderColour = Color.FromArgb(0, 198, 166, 99),
                ReadOnly = true,
                Editable = false,
                Parent = this,
                Location = new Point(84, 91),
                Size = new Size(156, 15),
            };

            MessageLabel = new DXLabel   //内容信息栏
            {
                Border = false,
                BorderColour = Color.FromArgb(0, 198, 166, 99),
                Parent = this,
                Location = new Point(15, 113),
                Size = new Size(242, 195),
                AutoSize = false,
                ForeColour = Color.White,
            };

            MailItems = new ClientUserItem[7];

            Grid = new DXItemGrid     //道具格子
            {
                GridSize = new Size(7, 1),
                Parent = this,
                Location = new Point(16, 323),
                ItemGrid = MailItems,
            };
            foreach (DXItemCell cell in Grid.Grid)
            {
                cell.ReadOnly = true;
                cell.MouseClick += (o, e) =>
                {
                    if (!MapObject.User.InSafeZone) return;
                    if (cell.Item == null) return;
                    CEnvir.Enqueue(new C.MailGetItem { Index = Mail.Index, Slot = cell.Slot });
                };
            }

            ReplyButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(70, SmallButtonHeight),
                Location = new Point(50, 385),
                Parent = this,
                Label = { Text = "回复" }
            };
            ReplyButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CommunicationBox.BringToFront();
                GameScene.Game.CommunicationBox.GoodFriendButton.Opacity = 0F;
                GameScene.Game.CommunicationBox.ReceivingButton.Opacity = 0F;
                GameScene.Game.CommunicationBox.PostalMatterButton.Opacity = 1F;
                GameScene.Game.CommunicationBox.BlacklistButton.Opacity = 0F;
                GameScene.Game.CommunicationBox.FriendPanel.Visible = false;
                GameScene.Game.CommunicationBox.ReceivingPanel.Visible = false;
                GameScene.Game.CommunicationBox.PostalMatterPanel.Visible = true;
                GameScene.Game.CommunicationBox.BlacklistPanel.Visible = false;
                GameScene.Game.CommunicationBox.Close1Button.Visible = false;
                GameScene.Game.CommunicationBox.SendButton.Visible = true;
                GameScene.Game.CommunicationBox.CollectAllButton.Visible = false;
                GameScene.Game.CommunicationBox.DeleteAll.Visible = false;
                GameScene.Game.CommunicationBox.NewButton.Visible = false;
                GameScene.Game.CommunicationBox.RecipientBox.TextBox.Text = Mail.Sender;
            };

            DeleteButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Size = new Size(70, SmallButtonHeight),
                Location = new Point(175, 385),
                Parent = this,
                Label = { Text = "删除".Lang() }
            };
            DeleteButton.MouseClick += (o, e) =>
            {
                if (Mail.Items.Count > 0)
                {
                    GameScene.Game.ReceiveChat("邮件内有物品无法删除".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.MailDelete { Index = Mail.Index });
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Mail = null;
                MailChanged = null;

                if (SenderBox != null)
                {
                    if (!SenderBox.IsDisposed)
                        SenderBox.Dispose();

                    SenderBox = null;
                }

                if (SubjectBox != null)
                {
                    if (!SubjectBox.IsDisposed)
                        SubjectBox.Dispose();

                    SubjectBox = null;
                }

                if (DateBox != null)
                {
                    if (!DateBox.IsDisposed)
                        DateBox.Dispose();

                    DateBox = null;
                }

                if (MessageLabel != null)
                {
                    if (!MessageLabel.IsDisposed)
                        MessageLabel.Dispose();

                    MessageLabel = null;
                }

                if (DeleteButton != null)
                {
                    if (!DeleteButton.IsDisposed)
                        DeleteButton.Dispose();

                    DeleteButton = null;
                }

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                MailItems = null;
            }
        }
        #endregion
    }
    #endregion

    #region 好友行内容

    public sealed class FriendFunctionList : DXListBox
    {
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            FriendFunctionListRow target = MouseControl as FriendFunctionListRow;

            if (target == null)
            {
                this.Visible = false;
            }
        }
    }

    public sealed class FriendFunctionListRow : DXListBoxItem
    {
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            DXListBox listBox = Parent as DXListBox;
            if (listBox == null) return;
            listBox.SelectedItem = null;
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            MouseControl = this;
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            MouseControl = Parent;
        }
    }

    public sealed class FriendRow : DXControl
    {
        #region Properties

        #region IsHeader

        public bool IsHeader
        {
            get => _IsHeader;
            set
            {
                if (_IsHeader == value) return;

                bool oldValue = _IsHeader;
                _IsHeader = value;

                OnIsHeaderChanged(oldValue, value);
            }
        }
        private bool _IsHeader;
        public event EventHandler<EventArgs> IsHeaderChanged;
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            DrawTexture = false;
            IsControl = false;

            IsHeaderChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Friend

        public ClientFriendInfo Friend
        {
            get => _Friend;
            set
            {
                ClientFriendInfo oldValue = _Friend;
                _Friend = value;

                OnFriendChanged(oldValue, value);
            }
        }
        private ClientFriendInfo _Friend;

        public void OnFriendChanged(ClientFriendInfo oValue, ClientFriendInfo nValue)
        {
            Visible = nValue != null;
            if (nValue == null) return;

            NameLabel.Text = Friend.Name;
            LinkID = Friend.LinkID;
        }

        #endregion

        private string LinkID;
        public DXLabel NameLabel;
        #endregion

        public FriendRow()
        {
            Size = new Size(240, 18);
            DrawTexture = true;
            Border = true;

            NameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(200, 18),
                Location = new Point(5, 2),
                ForeColour = Color.White,
                Parent = this,
                IsControl = false,
            };
        }

        #region Methods
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();

            if (IsHeader) return;
            Border = true;
            BorderColour = Color.Yellow;
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (IsHeader) return;
            Border = false;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _IsHeader = false;
                IsHeaderChanged = null;

                _Friend = null;

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }
            }
        }
        #endregion
    }
    #endregion

}
