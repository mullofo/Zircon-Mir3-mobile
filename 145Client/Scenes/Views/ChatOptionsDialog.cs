using System.Drawing;
using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class ChatOptionsDialog : DXWindow         //聊天选项对话框
    {
        #region Properties
        public DXListBox ListBox;

        public override WindowType Type => WindowType.ChatOptionsBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        #endregion

        public ChatOptionsDialog()
        {
            TitleLabel.Text = "聊天选项";
            HasFooter = true;

            SetClientSize(new Size(350, 200));

            ListBox = new DXListBox
            {
                Location = ClientArea.Location,
                Size = new Size(120, ClientArea.Height - SmallButtonHeight - 5),
                Parent = this,
            };

            DXButton AddButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "增加" },
                Parent = this,
                Size = new Size(50, SmallButtonHeight),
            };
            AddButton.Location = new Point((ListBox.DisplayArea.Right - AddButton.Size.Width), ListBox.DisplayArea.Bottom + 5);

            AddButton.MouseClick += (o, e) =>
            {
                ChatTab temp = AddNewTab($"聊天窗{ListBox.Controls.Count}");
            };
            DXButton ResetButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "全部重置" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.Right - 80 - 10, Size.Height - 43),
            };
            ResetButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox("确认重置所有聊天窗口", "重置聊天", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    for (int i = ChatTab.Tabs.Count - 1; i >= 0; i--)
                        ChatTab.Tabs[i].Panel.RemoveButton.InvokeMouseClick();
                };
            };

            DXButton SaveButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "全部保存" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.X, Size.Height - 43),
            };
            SaveButton.MouseClick += (o, e) =>
            {
                // DXMessageBox box = new DXMessageBox("Are you sure you want to reset ALL chat windows", "Chat Reset", DXMessageBoxButtons.YesNo);

                GameScene.Game.SaveChatTabs();
                GameScene.Game.ReceiveChat("聊天窗口已保存", MessageType.Announcement);
            };

            DXButton ReloadButton = new DXButton
            {
                ButtonType = ButtonType.Default,
                Label = { Text = "全部重新读取" },
                Parent = this,
                Size = new Size(80, DefaultHeight),
                Location = new Point(ClientArea.X + 85, Size.Height - 43),
            };
            ReloadButton.MouseClick += (o, e) =>
            {
                DXMessageBox box = new DXMessageBox("确认重新读取所有聊天窗口", "重读聊天", DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o1, e1) =>
                {
                    GameScene.Game.LoadChatTabs();
                };
            };
        }

        #region Methods

        public ChatOptionsPanel AddOptionsPanel(string tabName)
        {
            ChatOptionsPanel panel = new ChatOptionsPanel
            {
                Parent = this,
                Visible = false,
                Location = new Point(ListBox.Location.X + ListBox.Size.Width + 5, ListBox.Location.Y),
                LocalCheckBox = { Checked = true },
                WhisperCheckBox = { Checked = true },
                GroupCheckBox = { Checked = true },
                GuildCheckBox = { Checked = true },
                ShoutCheckBox = { Checked = true },
                GlobalCheckBox = { Checked = true },
                ObserverCheckBox = { Checked = true },
                HintCheckBox = { Checked = true },
                SystemCheckBox = { Checked = true },
                GainsCheckBox = { Checked = true },
                AlertCheckBox = { Checked = true },
                NameTagCheckBox = { Checked = Config.ChkHideChatTab },  //默认标签不显示
            };

            DXListBoxItem PanelLstItem = new DXListBoxItem
            {
                Parent = ListBox,
                Item = panel,
            };

            PanelLstItem.SelectedChanged += (o, e) =>
            {
                panel.Visible = PanelLstItem.Selected;
            };
            
            panel.Size = new Size(ClientArea.Width - panel.Location.X, ClientArea.Height);
            
            panel.Text = tabName;
            panel.TextChanged += (o1, e1) =>
            {
                PanelLstItem.Label.Text = panel.Text;
            };

            ListBox.SelectedItem = PanelLstItem;
            return panel;
        }

        public ChatTab AddNewTab(string tabName)
        {
            ChatOptionsPanel panel = AddOptionsPanel(tabName);
           
            DXTabControl tabControl = new DXTabControl
            {
                //PassThrough = false,
                Size = GameScene.Game.ChatBox.DefaultSize,//new Size(200, 200),
                Parent = GameScene.Game,
                Movable = true,
            };

            ChatTab tab = new ChatTab  //聊天框
            {
                Parent = tabControl,
                Panel =  panel,
                Opacity = 0.5F,
                TabButton =
                {
                    Movable = true, 
                    AllowDragOut = true,
                    Label = { Text = tabName },
                }        
            };

            tabControl.MouseWheel += (o, e1) => (tabControl.SelectedTab as ChatTab)?.ScrollBar.DoMouseWheel(o, e1);
            panel.TransparentCheckBox.CheckedChanged += (o, e1) => tab.TransparencyChanged();  //透明标签
            panel.NameTagCheckBox.CheckedChanged += (o, e1) =>
            {
                tab.NameTagChanged();   //名字标签变化
                Config.ChkHideChatTab = panel.NameTagCheckBox.Checked;
            };
            panel.AlertCheckBox.CheckedChanged += (o, e1) =>
            {
                tab.AlertIcon.Visible = panel.AlertCheckBox.Checked;
            };
            
            panel.TextChanged += (o1, e1) =>
            {
                tab.TabButton.Label.Text = panel.Text;
            };
            
            panel.RemoveButton.MouseClick += (o1, e1) =>
            {
                DXListBoxItem nextItem = null;
                DXListBoxItem found = null;
                
                foreach (DXControl control in ListBox.Controls)
                {
                    DXListBoxItem item = control as DXListBoxItem;
                    if (item == null) continue;

                    if (item.Item == panel)
                    {
                        found = item;
                        continue;
                    }

                    nextItem = control as DXListBoxItem;
                    
                    if (found != null) break;
                }
                ListBox.SelectedItem = nextItem;

                found.Dispose();
                panel.Dispose();
                ListBox.UpdateItems();
                
                tabControl = tab.Parent as DXTabControl;
                tab.Dispose();

                if (tabControl?.Controls.Count == 0)
                    tabControl.Dispose();
            };
            return tab;
        }

        //public void CreateDefaultWindows(string LabName= "全部")   //创建默认窗口  
        //{
        //    return; //这函数要废弃 不能用的
        //    GameScene.Game.ChatTextBox.Dispose();   //聊天文本框 释放

        //    GameScene.Game.ChatTextBox = new ChatTextBox
        //    {
        //        Parent = GameScene.Game,
        //    };

        //    ChatTab tab = AddNewTab(LabName);
            
        //    tab.CurrentTabControl.Size = new Size(GameScene.Game.ChatTextBox.Size.Width, GameScene.Game.MainPanel.Size.Height - GameScene.Game.ChatTextBox.Size.Height);
        //    tab.CurrentTabControl.Location = new Point(GameScene.Game.ChatTextBox.Location.X, GameScene.Game.MainPanel.Location.Y);//(GameScene.Game.ChatTextBox.Location.X, GameScene.Game.ChatTextBox.Location.Y - 150);

        //    tab.Panel.Text = LabName;   // {ListBox.Controls.Count - 1}
        //    tab.IsDefault = true;
        //    ChatTab.DefaultTab = tab;

        //    if(Config.ChkHideChatTab)
        //    {
        //     //   tab.TabButton.Visible = false;
        //    }
        //}
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ListBox != null)
                {
                    if (!ListBox.IsDisposed)
                        ListBox.Dispose();

                    ListBox = null;
                }
            }
        }

        #endregion
    }
    
    public sealed class ChatOptionsPanel : DXControl
    {
        #region Properties

        public DXTextBox NameTextBox;
        public DXButton RemoveButton;

        public DXCheckBox NameTagCheckBox;   //名字标签
        public DXCheckBox TransparentCheckBox, AlertCheckBox;
        public DXCheckBox LocalCheckBox, WhisperCheckBox;
        public DXCheckBox GroupCheckBox, GuildCheckBox;
        public DXCheckBox ShoutCheckBox, GlobalCheckBox;
        public DXCheckBox ObserverCheckBox;
        public DXCheckBox SystemCheckBox, GainsCheckBox;
        public DXCheckBox HintCheckBox;

        public override void OnTextChanged(string oValue, string nValue)
        {
            base.OnTextChanged(oValue, nValue);

            if (NameTextBox != null)
                NameTextBox.TextBox.Text = Text;
        }

        #endregion

        public ChatOptionsPanel()
        {
            DXLabel label = new DXLabel
            {
                Text = "窗口名称:",
                Outline = true,
                Parent = this,
            };
            label.Location = new Point(74 - label.Size.Width, 1);

            NameTextBox = new DXTextBox
            {
                Location = new Point(74, 1),
                Size = new Size(80, 20),
                Parent = this,
            };
            NameTextBox.TextBox.TextChanged += (o, e) => Text = NameTextBox.TextBox.Text;

            NameTagCheckBox = new DXCheckBox
            {
                Label = { Text = "窗口标签:" },
                Parent = this,
                Checked = false,
            };
            NameTagCheckBox.Location = new Point(100 - NameTagCheckBox.Size.Width, 30);

            TransparentCheckBox = new DXCheckBox
            {
                Label = { Text = "透明:" },
                Parent = this,
                Checked = false,
            };
            TransparentCheckBox.Location = new Point(100 - TransparentCheckBox.Size.Width, 55);

            AlertCheckBox = new DXCheckBox
            {
                Label = { Text = "显示提示框:" },
                Parent = this,
                Checked = false,
            };
            AlertCheckBox.Location = new Point(216 - AlertCheckBox.Size.Width, 55);


            LocalCheckBox = new DXCheckBox
            {
                Label = { Text = "本地聊天:" },
                Parent = this,
                Checked = false,
            };
            LocalCheckBox.Location = new Point(100 - LocalCheckBox.Size.Width, 80);

            WhisperCheckBox = new DXCheckBox
            {
                Label = { Text = "私聊:" },
                Parent = this,
                Checked = false,
            };
            WhisperCheckBox.Location = new Point(216 - WhisperCheckBox.Size.Width, 80);

            GroupCheckBox = new DXCheckBox
            {
                Label = { Text = "队伍聊天:" },
                Parent = this,
                Checked = false,
            };
            GroupCheckBox.Location = new Point(100 - GroupCheckBox.Size.Width, 105);

            GuildCheckBox = new DXCheckBox
            {
                Label = { Text = "行会聊天:" },
                Parent = this,
                Checked = false,
            };
            GuildCheckBox.Location = new Point(216 - GuildCheckBox.Size.Width, 105);

            ShoutCheckBox = new DXCheckBox
            {
                Label = { Text = "喊话:" },
                Parent = this,
                Checked = false,
            };
            ShoutCheckBox.Location = new Point(100 - ShoutCheckBox.Size.Width, 130);

            GlobalCheckBox = new DXCheckBox
            {
                Label = { Text = "全屏喊话:" },
                Parent = this,
                Checked = false,
            };
            GlobalCheckBox.Location = new Point(216 - GlobalCheckBox.Size.Width, 130);

            ObserverCheckBox = new DXCheckBox
            {
                Label = { Text = "观察者聊天:" },
                Parent = this,
                Checked = false,
            };
            ObserverCheckBox.Location = new Point(100 - ObserverCheckBox.Size.Width, 155);

            HintCheckBox = new DXCheckBox
            {
                Label = { Text = "提示消息:" },
                Parent = this,
                Checked = false,
            };
            HintCheckBox.Location = new Point(216 - HintCheckBox.Size.Width, 155);

            SystemCheckBox = new DXCheckBox
            {
                Label = { Text = "系统消息:" },
                Parent = this,
                Checked = false,
            };
            SystemCheckBox.Location = new Point(100 - SystemCheckBox.Size.Width, 180);

            GainsCheckBox = new DXCheckBox
            {
                Label = { Text = "获取消息:" },
                Parent = this,
                Checked = false,
            };
            GainsCheckBox.Location = new Point(216 - GainsCheckBox.Size.Width, 180);

            RemoveButton = new DXButton
            {
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "移除" },
                Parent = this,
                Size = new Size(50, SmallButtonHeight),
                Location = new Point(NameTextBox.DisplayArea.Right + 10, 0),
            };
        }
        
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (NameTextBox != null)
                {
                    if (!NameTextBox.IsDisposed)
                        NameTextBox.Dispose();

                    NameTextBox = null;
                }

                if (RemoveButton != null)
                {
                    if (!RemoveButton.IsDisposed)
                        RemoveButton.Dispose();

                    RemoveButton = null;
                }

                if (NameTagCheckBox != null)         //名字标签
                {
                    if (!NameTagCheckBox.IsDisposed)
                        NameTagCheckBox.Dispose();

                    NameTagCheckBox = null;
                }

                if (TransparentCheckBox != null)
                {
                    if (!TransparentCheckBox.IsDisposed)
                        TransparentCheckBox.Dispose();

                    TransparentCheckBox = null;
                }

                if (AlertCheckBox != null)
                {
                    if (!AlertCheckBox.IsDisposed)
                        AlertCheckBox.Dispose();

                    AlertCheckBox = null;
                }

                if (LocalCheckBox != null)
                {
                    if (!LocalCheckBox.IsDisposed)
                        LocalCheckBox.Dispose();

                    LocalCheckBox = null;
                }

                if (WhisperCheckBox != null)
                {
                    if (!WhisperCheckBox.IsDisposed)
                        WhisperCheckBox.Dispose();

                    WhisperCheckBox = null;
                }

                if (GroupCheckBox != null)
                {
                    if (!GroupCheckBox.IsDisposed)
                        GroupCheckBox.Dispose();

                    GroupCheckBox = null;
                }

                if (GuildCheckBox != null)
                {
                    if (!GuildCheckBox.IsDisposed)
                        GuildCheckBox.Dispose();

                    GuildCheckBox = null;
                }

                if (ShoutCheckBox != null)
                {
                    if (!ShoutCheckBox.IsDisposed)
                        ShoutCheckBox.Dispose();

                    ShoutCheckBox = null;
                }

                if (GlobalCheckBox != null)
                {
                    if (!GlobalCheckBox.IsDisposed)
                        GlobalCheckBox.Dispose();

                    GlobalCheckBox = null;
                }

                if (ObserverCheckBox != null)
                {
                    if (!ObserverCheckBox.IsDisposed)
                        ObserverCheckBox.Dispose();

                    ObserverCheckBox = null;
                }

                if (SystemCheckBox != null)
                {
                    if (!SystemCheckBox.IsDisposed)
                        SystemCheckBox.Dispose();

                    SystemCheckBox = null;
                }

                if (GainsCheckBox != null)
                {
                    if (!GainsCheckBox.IsDisposed)
                        GainsCheckBox.Dispose();

                    GainsCheckBox = null;
                }

                if (HintCheckBox != null)
                {
                    if (!HintCheckBox.IsDisposed)
                        HintCheckBox.Dispose();

                    HintCheckBox = null;
                }
            }

        }

        #endregion
    }
}
