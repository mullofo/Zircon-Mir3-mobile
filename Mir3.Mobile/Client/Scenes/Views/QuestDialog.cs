using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 任务框架 TODO 任务最多接20个，拒绝任务按钮
    /// </summary>
    public sealed class QuestDialog : DXWindow
    {
        #region Properties
        /// <summary>
        /// 任务背景图
        /// </summary>
        public DXImageControl Background;
        /// <summary>
        /// 任务栏标题
        /// </summary>
        public DXLabel Title;
        /// <summary>
        /// 任务条数
        /// </summary>
        public DXLabel DoingQuest;
        /// <summary>
        /// 任务条数状态显示
        /// </summary>
        public DXLabel DoingText;
        /// <summary>
        /// 放弃任务按钮
        /// </summary>
        public DXButton DeleteQuest;
        /// <summary>
        /// 结束按钮
        /// </summary>
        public DXButton Close1Button;
        /// <summary>
        /// 领取奖励按钮
        /// </summary>
        public DXButton ClaimRewardButton;
        /// <summary>
        /// 任务成就内容切换选项卡
        /// </summary>
        public DXTabControl TabControl;
        /// <summary>
        /// 可接任务项
        /// </summary>
        public QuestTab AvailableTab;
        /// <summary>
        /// 当前任务项
        /// </summary>
        public QuestTab CurrentTab;
        /// <summary>
        /// 完成任务项
        /// </summary>
        public QuestTab CompletedTab;
        /// <summary>
        /// 成就任务项
        /// </summary>
        public AchievementTab TitleTab;
        /// <summary>
        /// 所选的任务项(默认显示的)
        /// </summary>
        public QuestTab SelectedTab;

        public override WindowType Type => WindowType.QuestBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 任务界面
        /// </summary>
        public QuestDialog()
        {
            //TitleLabel.Text = "任务日志";
            HasFooter = false;
            HasTitle = false;
            IgnoreMoveBounds = true;

            //SetClientSize(new Size(558, 430));

            Background = new DXImageControl            //背景容器
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 1000,
                FixedSize = false,
                Parent = this,
                Location = new Point(0, 0),
            };
            Background.MouseDown += Background_MouseDown;  //鼠标事件
            Size = Background.Size;   //大小等于图片的大小

            Title = new DXLabel       //标题
            {
                Parent = this,
                ForeColour = Color.White,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                Text = "任务日志".Lang(),
                Location = new Point(Size.Width / 2 - 20, 17),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            DeleteQuest = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 982,
                Parent = this,
                Location = new Point(400, 440)
            };
            DeleteQuest.MouseClick += (sender, args) =>
            {
                if (SelectedTab != null)
                {
                    QuestInfo quest = SelectedTab.SelectedQuest?.QuestInfo;
                    if (quest == null) return;

                    new DXConfirmWindow("任务次数不会返还，确认放弃此任务?", () =>
                    {
                        CEnvir.Enqueue(new C.GiveUpQuest { QuestIndex = quest.Index });
                    });
                }
            };

            Close1Button = new DXButton   //关闭按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 972,
                Parent = this,
                Location = new Point(600, 440),
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            ClaimRewardButton = new DXButton   //领取奖励按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 997,
                Parent = this,
                Location = new Point(500, 440),
                ButtonType = ButtonType.SmallButton,
                Visible = false,
            };
            ClaimRewardButton.MouseEnter += (o, e) => ClaimRewardButton.Index = 995;
            ClaimRewardButton.MouseLeave += (o, e) => ClaimRewardButton.Index = 997;
            ClaimRewardButton.MouseClick += (o, e) =>
            {
                if (SelectedTab != null) //TODO 测试这个 随身领奖励
                {
                    if (SelectedTab.SelectedQuest?.QuestInfo == null) return;

                    if (SelectedTab.HasChoice && !SelectedTab.RandomChoice && SelectedTab.SelectedCell == null)
                    {
                        GameScene.Game.ReceiveChat("请选择一个奖励".Lang(), MessageType.System);
                    }
                    else if (SelectedTab.SelectedQuest.QuestInfo.FinishNPC != null)
                    {
                        GameScene.Game.ReceiveChat("请前往相应NPC处完成任务".Lang(), MessageType.System);
                    }
                    else
                    {
                        CEnvir.Enqueue(new C.QuestComplete { Index = SelectedTab.SelectedQuest.QuestInfo.Index, ChoiceIndex = ((QuestReward)(SelectedTab.SelectedCell)?.Tag)?.Index ?? 0 });
                    }
                }
            };

            DoingQuest = new DXLabel
            {
                Parent = this,
                Text = "0",
                ForeColour = Color.White,
                Location = new Point(11, 445),
                AutoSize = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Size = new Size(50, 15)
            };


            DoingText = new DXLabel
            {
                Parent = this,
                Text = "进行中的任务".Lang(),
                ForeColour = Color.White,
                Location = new Point(100, 445),
            };

            TabControl = new DXTabControl   //选项卡
            {
                Parent = Background,
                Opacity = 0F,
                Location = new Point(13, 21),
                Size = new Size(Size.Width - 21, Size.Height - 19),
            };

            CurrentTab = new QuestTab    //当前任务选项
            {
                TabButton = {
                    Opacity=1F,
                    LibraryFile=LibraryFile.GameInter,
                    Index=1005},
                Parent = TabControl,
                //Border = true,
                Opacity = 0F,
                Tag = 1,
                ChoiceGrid = { ReadOnly = true }
            };
            SelectedTab = CurrentTab;
            CurrentTab.TabButton.MouseClick += (s, e) =>
            {
                Background.Index = 1000;
                CurrentTab.TabButton.Opacity = 1F;
                TitleTab.TabButton.Opacity = 0F;
                AvailableTab.TabButton.Opacity = 0F;
                CompletedTab.TabButton.Opacity = 0F;
                //DoingQuest.Visible = true;
                //DoingText.Visible = true;
                //ClaimRewardButton.Visible = true;
                //CurrentTab.SelectedQuest = null;
            };

            TitleTab = new AchievementTab    //成就任务选项
            {
                TabButton = {
                    Opacity=0F,
                    LibraryFile=LibraryFile.GameInter,
                    Index=1006,
                    Visible = false},
                Parent = TabControl,
                //Border = true,
                Opacity = 0F,
                //ShowTrackerBox = { Visible = false }
            };
            TitleTab.TabButton.MouseClick += (s, e) =>
            {
                Background.Index = 1001;
                CurrentTab.TabButton.Opacity = 0F;
                TitleTab.TabButton.Opacity = 1F;
                AvailableTab.TabButton.Opacity = 0F;
                CompletedTab.TabButton.Opacity = 0F;
                DoingQuest.Visible = false;
                DoingText.Visible = false;
                ClaimRewardButton.Visible = false;
                TitleTab.Init();
            };

            AvailableTab = new QuestTab   //可接任务选项
            {
                TabButton = {
                    Opacity=0F,
                    LibraryFile=LibraryFile.GameInter,
                    Index=1007},
                Parent = TabControl,
                //Border = true,
                Opacity = 0F,
                Tag = 2,
                ShowTrackerBox = { Visible = false }
            };
            AvailableTab.TabButton.MouseClick += (s, e) =>
            {
                Background.Index = 1000;
                CurrentTab.TabButton.Opacity = 0F;
                TitleTab.TabButton.Opacity = 0F;
                AvailableTab.TabButton.Opacity = 1F;
                CompletedTab.TabButton.Opacity = 0F;
                //DoingQuest.Visible = true;
                //DoingText.Visible = true;
                //ClaimRewardButton.Visible = true;
                //AvailableTab.SelectedQuest = null;

            };

            CompletedTab = new QuestTab    //已完成任务选项
            {
                TabButton = {
                    Opacity=0F,
                    LibraryFile=LibraryFile.GameInter,
                    Index=1008},
                Parent = TabControl,
                //Border = true,
                Opacity = 0F,
                Tag = 3,
                ShowTrackerBox = { Visible = false }
            };
            CompletedTab.TabButton.MouseClick += (s, e) =>
            {
                Background.Index = 1000;
                CurrentTab.TabButton.Opacity = 0F;
                TitleTab.TabButton.Opacity = 0F;
                AvailableTab.TabButton.Opacity = 0F;
                CompletedTab.TabButton.Opacity = 1F;
                //DoingQuest.Visible = true;
                //DoingText.Visible = true;
                //ClaimRewardButton.Visible = true;
                //CompletedTab.SelectedQuest = null;
            };
        }

        #region Methods
        /// <summary>
        /// 背景容器鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Background_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);  //鼠标按下时
        }
        /// <summary>
        /// 任务改变时
        /// </summary>
        /// <param name="quest"></param>
        public void QuestChanged(ClientUserQuest quest)
        {
            if (AvailableTab.SelectedQuest?.QuestInfo == quest.Quest)
                AvailableTab.UpdateQuestDisplay();

            //if (CurrentTab.SelectedQuest?.QuestInfo == quest.Quest)
            CurrentTab.UpdateQuestDisplay();

            if (CompletedTab.SelectedQuest?.QuestInfo == quest.Quest)
                CompletedTab.UpdateQuestDisplay();

            //if (TitleTab.SelectedQuest?.QuestInfo == quest.Quest)
            //    TitleTab.UpdateQuestDisplay();

        }
        /// <summary>
        /// 填充任务
        /// </summary>
        public void PopulateQuests()
        {
            bool available = false, current = false, completed = false;
            foreach (QuestInfo quest in Globals.QuestInfoList.Binding)
            {
                ClientUserQuest userQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == quest);

                if (userQuest == null)
                {
                    if (!GameScene.Game.CanAccept(quest)) continue;

                    if (AvailableTab.Quests.Contains(quest)) continue;

                    //除非正在进行 或者已完成
                    if (quest.QuestType == QuestType.Daily)
                    {
                        continue;
                    }

                    AvailableTab.Quests.Add(quest);
                    available = true;
                    continue;
                }

                if (AvailableTab.Quests.Contains(quest))
                {
                    AvailableTab.Quests.Remove(quest);
                    available = true;
                }

                if (userQuest.Completed) //生成已完成任务列表
                {
                    if (CompletedTab.Quests.Contains(quest)) continue;

                    CompletedTab.Quests.Add(quest);
                    completed = true;

                    if (!CurrentTab.Quests.Contains(quest)) continue;

                    CurrentTab.Quests.Remove(quest);
                    current = true;

                    continue;
                }

                if (CurrentTab.Quests.Contains(quest)) continue;

                CurrentTab.Quests.Add(quest);
                current = true;
            }

            if (available)
            {
                AvailableTab.NeedUpdate = true;
                AvailableTab.UpdateQuestDisplay();
            }
            if (current)
            {
                CurrentTab.NeedUpdate = true;
                CurrentTab.UpdateQuestDisplay();
            }
            if (completed)
            {
                CompletedTab.NeedUpdate = true;
                CompletedTab.UpdateQuestDisplay();
            }
            //if (title)
            //{
            //    TitleTab.NeedUpdate = true;
            //    TitleTab.UpdateQuestDisplay();
            //}

            DoingQuest.Text = CurrentTab.Quests.Count == 0 && CurrentTab.Quests.Count == 0 ? "0" : $"{CurrentTab.Quests.Count} / {AvailableTab.Quests.Count + CurrentTab.Quests.Count}";
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (AvailableTab != null)
                {
                    if (!AvailableTab.IsDisposed)
                        AvailableTab.Dispose();

                    AvailableTab = null;
                }

                if (CurrentTab != null)
                {
                    if (!CurrentTab.IsDisposed)
                        CurrentTab.Dispose();

                    CurrentTab = null;
                }

                if (CompletedTab != null)
                {
                    if (!CompletedTab.IsDisposed)
                        CompletedTab.Dispose();

                    CompletedTab = null;
                }
                DoingQuest.TryDispose();
                DoingText.TryDispose();
            }
        }
        #endregion
    }
    /// <summary>
    /// 当前任务选项
    /// </summary>
    public sealed class QuestTab : DXTab
    {
        #region Properties

        #region NeedUpdate
        /// <summary>
        /// 是否需要更新
        /// </summary>
        public bool NeedUpdate
        {
            get => _NeedUpdate;
            set
            {
                if (_NeedUpdate == value) return;

                bool oldValue = _NeedUpdate;
                _NeedUpdate = value;

                OnNeedUpdateChanged(oldValue, value);
            }
        }
        private bool _NeedUpdate;
        public event EventHandler<EventArgs> NeedUpdateChanged;
        public void OnNeedUpdateChanged(bool oValue, bool nValue)
        {
            NeedUpdateChanged?.Invoke(this, EventArgs.Empty);

            if (!NeedUpdate) return;

            if (!IsVisible) return;

            UpdateQuestTree();
        }

        #endregion

        #region SelectedQuest
        /// <summary>
        /// 选定的任务
        /// </summary>
        public QuestTreeEntry SelectedQuest
        {
            get => _SelectedQuest;
            set
            {

                QuestTreeEntry oldValue = _SelectedQuest;
                _SelectedQuest = value;

                OnSelectedQuestChanged(oldValue, value);
            }
        }
        private QuestTreeEntry _SelectedQuest;
        public event EventHandler<EventArgs> SelectedQuestChanged;

        public DXItemCell SelectedCell
        {
            get => _SelectedCell;
            set
            {
                DXItemCell oldValue = _SelectedCell;
                _SelectedCell = value;

                OnSelectedCellChanged(oldValue, value);
            }
        }
        private DXItemCell _SelectedCell;
        public event EventHandler<EventArgs> SelectedCellChanged;
        public void OnSelectedCellChanged(DXItemCell oValue, DXItemCell nValue)
        {
            if (oValue != null)
            {
                oValue.FixedBorder = false;
                oValue.Border = false;
                oValue.FixedBorderColour = false;
                oValue.BorderColour = Color.Lime;
            }

            if (nValue != null)
            {
                nValue.Border = true;
                nValue.FixedBorder = true;
                nValue.FixedBorderColour = true;
                nValue.BorderColour = Color.Lime;
            }

            SelectedCellChanged?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 选定的任务改变
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public void OnSelectedQuestChanged(QuestTreeEntry oValue, QuestTreeEntry nValue)
        {
            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);

            foreach (DXItemCell cell in RewardGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
            }

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.Item = null;
                cell.Tag = null;
                cell.FixedBorder = false;
                cell.Border = false;
                cell.FixedBorderColour = false;
                cell.BorderColour = Color.Lime;
            }

            if (SelectedQuest?.QuestInfo == null)
            {
                TaskView.RemoveAll();
                DescriptionLabel.Text = string.Empty;

                QuestNameLabel.Text = string.Empty;
                EndLabel.Text = string.Empty;
                //StartLabel.Text = string.Empty;
                return;
            }

            int standard = 0, choice = 0;
            HasChoice = false;
            RandomChoice = false;

            foreach (QuestReward reward in SelectedQuest.QuestInfo.Rewards)
            {
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ClientUserItem item = new ClientUserItem(reward.Item, reward.Amount)
                {
                    Flags = flags,
                    ExpireTime = duration
                };

                ChooseReward.Visible = false;
                ChoiceGrid.Visible = false;

                if (reward.Choice)  //任务里选择奖励
                {
                    if (choice >= ChoiceGrid.Grid.Length) continue;

                    HasChoice = true;

                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;

                    if (SelectedQuest.UserQuest?.SelectedReward == reward.Index)
                    {
                        ChoiceGrid.Grid[choice].Border = true;
                        ChoiceGrid.Grid[choice].FixedBorder = true;
                        ChoiceGrid.Grid[choice].FixedBorderColour = true;
                        ChoiceGrid.Grid[choice].BorderColour = Color.Lime;
                    }
                    choice++;
                }
                else if (reward.Random)
                {
                    RandomChoice = true;
                    ChoiceGrid.Grid[choice].Item = item;
                    ChoiceGrid.Grid[choice].Tag = reward;
                    choice++;
                }
                else
                {
                    if (standard >= RewardGrid.Grid.Length) continue;

                    RewardGrid.Grid[standard].Item = item;
                    RewardGrid.Grid[standard].Tag = reward;

                    standard++;
                }
            }

            if (HasChoice || RandomChoice)
            {
                ChooseReward.Visible = true;
                ChoiceGrid.Visible = true;
                ChooseReward.Index = RandomChoice ? 924 : HasChoice ? 923 : -1;//随机奖励
            }

            QuestNameLabel.Text = SelectedQuest?.QuestInfo?.QuestName ?? "无".Lang();
            DescriptionLabel.Text = GameScene.Game.GetQuestText(SelectedQuest.QuestInfo, SelectedQuest.UserQuest, true);
            GameScene.Game.GetTaskText(TaskView, SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            var info = SelectedQuest.QuestInfo;
            if ((int)Tag == 2)
                EndLabel.Text = string.Format("({0}) {1}", info?.StartNPC?.Region?.Map?.Description ?? "无".Lang(), info?.StartNPC?.NPCName ?? "无".Lang());
            else
                EndLabel.Text = string.Format("({0}) {1}", info?.FinishNPC?.Region?.Map?.Description ?? "无".Lang(), info?.FinishNPC?.NPCName ?? "无".Lang());
            //StartLabel.Text = SelectedQuest.QuestInfo?.StartNPC?.RegionName ?? " 无".Lang();
            EndLabel.Location = new Point(EndLabelButton.Location.X - EndLabel.Size.Width - 2, EndLabelButton.Location.Y + 2);

            if (GameScene.Game.QuestBox != null)
                GameScene.Game.QuestBox.ClaimRewardButton.Visible = SelectedQuest.UserQuest != null && SelectedQuest.UserQuest.IsComplete && !SelectedQuest.UserQuest.Completed;
            SelectedQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public List<QuestInfo> Quests = new List<QuestInfo>();

        public DXVScrollBar ScrollBar;

        public DXLabel QuestNameLabel;
        //public DXLabel StartLabel;
        public DXLabel DescriptionLabel, EndLabel;
        public DXButton EndLabelButton;
        public DXListView TaskView;

        public DXItemGrid RewardGrid, ChoiceGrid;

        public ClientUserItem[] RewardArray, ChoiceArray;   //任务奖励数组  任务选择奖励数组

        public DXCheckBox ShowTrackerBox;

        public DXImageControl Reward, ChooseReward;

        public bool HasChoice;
        public bool RandomChoice;
        /// <summary>
        /// 任务树
        /// </summary>
        public QuestTree Tree;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (!IsVisible || !NeedUpdate) return;

            UpdateQuestTree();

        }
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            if (Tree == null) return;

            Tree.Size = new Size(348, Size.Height - 70);

        }
        #endregion

        /// <summary>
        /// 当前任务选项
        /// </summary>
        public QuestTab()
        {
            int width = 360;

            Tree = new QuestTree   //任务树
            {
                Parent = this,
                Location = new Point(5, 15)
            };
            Tree.SelectedEntryChanged += (o, e) => SelectedQuest = Tree.SelectedEntry;


            DXLabel label = new DXLabel
            {
                Text = "任务内容".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Visible = false,
                Location = new Point(width, 4)
            };

            ShowTrackerBox = new DXCheckBox
            {
                Label = { Text = "显示任务追踪".Lang() }, //显示任务追踪窗口
                Parent = this,
                Checked = Config.QuestTrackerVisible,
                Visible = false,
            };
            ShowTrackerBox.Location = new Point(width + 303 - ShowTrackerBox.Size.Width + 30, 12);
            ShowTrackerBox.CheckedChanged += (o, e) =>
            {
                Config.QuestTrackerVisible = ShowTrackerBox.Checked;
                GameScene.Game.QuestTrackerBox.PopulateQuests();
            };

            QuestNameLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(width, 12)
            };

            DescriptionLabel = new DXLabel   //任务内容文本框
            {
                AutoSize = false,
                Size = new Size(315, 90),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                ForeColour = Color.White,
                Location = new Point(width + 10, label.Location.Y + label.Size.Height + 25),
                Parent = this,
            };

            label = new DXLabel
            {
                Text = "完成条件".Lang(),//Tasks
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Visible = false,
                Location = new Point(width + 0, DescriptionLabel.Location.Y + DescriptionLabel.Size.Height + 5),
            };

            //TasksLabel = new DXLabel   //完成条件文本框
            //{
            //    AutoSize = false,
            //    Size = new Size(330, 65),
            //    //Border = true,
            //    //BorderColour = Color.FromArgb(198, 166, 99),
            //    ForeColour = Color.White,
            //    Location = new Point(370, 170),
            //    Parent = this,
            //};
            TaskView = new DXListView
            {
                HasHeader = false,
                HasVScrollBar = false,
                ItemBorder = false,
                SelectedBorder = false,
                ItemSelectedBackColour = Color.Empty,
                ItemSelectedBorderColour = Color.Empty,
                ItemForeColour = Color.White,
                Parent = this,
                Size = new Size(330, 105),
                Location = new Point(width + 13, 140),
            };
            TaskView.InsertColumn(0, "序号", 36, 16, "序号");
            TaskView.InsertColumn(1, "条件", 190, 16, "完成条件");
            TaskView.InsertColumn(2, "数量", 128, 16, "完成数量");

            Reward = new DXImageControl  //奖励
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 922,
                Parent = this,
                IsControl = false,
                Location = new Point(width + 2, TaskView.Location.Y + TaskView.Size.Height + 5),
            };

            RewardArray = new ClientUserItem[5];  //任务奖励 数组
            RewardGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(width + 12, Reward.Location.Y + Reward.Size.Height + 2),
                GridSize = new Size(RewardArray.Length, 1),
                ItemGrid = RewardArray,
                ReadOnly = true,
            };

            ChooseReward = new DXImageControl  //奖励图标
            {
                LibraryFile = LibraryFile.GameInter,
                Parent = this,
                Index = 923,
                IsControl = false,
                Location = new Point(width + 4, RewardGrid.Location.Y + RewardGrid.Size.Height + 4),
                Visible = false,
            };

            ChoiceArray = new ClientUserItem[4];
            ChoiceGrid = new DXItemGrid
            {
                Parent = this,
                Location = new Point(width + 12, ChooseReward.Location.Y + ChooseReward.Size.Height + 2),
                GridSize = new Size(ChoiceArray.Length, 1),
                ItemGrid = ChoiceArray,
                ReadOnly = true,
                Visible = false,
            };

            foreach (DXItemCell cell in ChoiceGrid.Grid)
            {
                cell.MouseClick += (o, e) =>
                {
                    if (RandomChoice) return;
                    if (((DXItemCell)o).Item == null) return;

                    SelectedCell = (DXItemCell)o;
                };
            }

            //label = new DXLabel
            //{
            //    Text = "开始".Lang() + ": ",
            //    Parent = this,
            //    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            //    //ForeColour = Color.FromArgb(198, 166, 99),
            //    Outline = true,
            //    OutlineColour = Color.Black,
            //    IsControl = false,
            //};
            //label.Location = new Point(width + 50 - label.Size.Width + 10, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 5);

            //StartLabel = new DXLabel  //开始标签
            //{
            //    Parent = this,
            //    ForeColour = Color.White,
            //    Location = new Point(label.Location.X + label.Size.Width - 8, label.Location.Y + (label.Size.Height - 12) / 2),
            //};
            //StartLabel.MouseClick += (o, e) =>  //鼠标点击时
            //{
            //    if (SelectedQuest?.QuestInfo?.StartNPC?.Region?.Map == null) return;

            //    GameScene.Game.BigMapBox.Visible = true;
            //    GameScene.Game.BigMapBox.Opacity = 1F;
            //    GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo?.StartNPC?.Region?.Map;
            //};

            //label = new DXLabel
            //{
            //    Text = "结束".Lang() + ": ",
            //    Parent = this,
            //    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
            //    //ForeColour = Color.FromArgb(198, 166, 99),
            //    Outline = true,
            //    OutlineColour = Color.Black,
            //    IsControl = false,
            //    Location = new Point(width + 0, label.Location.Y + label.Size.Height),
            //};
            //label.Location = new Point(width + 50 - label.Size.Width + 175, ChoiceGrid.Location.Y + ChoiceGrid.Size.Height + 5);
            EndLabelButton = new DXButton   //npc大地图按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 137,
                Parent = this,
                Location = new Point(666, 146),
            };
            EndLabelButton.MouseClick += (o, e) =>  //鼠标点击时
            {
                if ((int)Tag == 2)
                {
                    if (SelectedQuest?.QuestInfo?.StartNPC?.Region?.Map == null) return;

                    GameScene.Game.BigMapBox.Visible = true;
                    GameScene.Game.BigMapBox.Opacity = 1F;
                    GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo?.StartNPC?.Region?.Map;
                }
                else
                {
                    if (SelectedQuest?.QuestInfo?.FinishNPC?.Region?.Map == null) return;

                    GameScene.Game.BigMapBox.Visible = true;
                    GameScene.Game.BigMapBox.Opacity = 1F;
                    GameScene.Game.BigMapBox.SelectedInfo = SelectedQuest.QuestInfo?.FinishNPC?.Region?.Map;
                }

            };

            EndLabel = new DXLabel   //结束标签
            {
                Parent = this,
                ForeColour = Color.White,
                Location = new Point(EndLabelButton.Location.X - 50, EndLabelButton.Location.Y),
            };

        }

        #region Methods
        /// <summary>
        /// 更新任务树
        /// </summary>
        public void UpdateQuestTree()
        {
            NeedUpdate = false;

            Tree.TreeList.Clear();

            Quests.Sort((x1, x2) =>
            {
                int res = string.Compare(x1?.StartNPC?.Region?.Map?.Description, x2?.StartNPC?.Region?.Map?.Description, StringComparison.Ordinal);
                if (res == 0)
                    return string.Compare(x1.QuestName, x2.QuestName, StringComparison.Ordinal);

                return res;
            });
            foreach (QuestInfo quest in Quests)
            {
                MapInfo map = quest?.StartNPC?.Region?.Map;

                List<QuestInfo> quests;

                //如果是道具任务或者重复任务  或者  任务是奇遇任务  跳过 不写入任务树
                if (quest.QuestType == QuestType.Repeatable || quest.QuestType == QuestType.Hidden)
                    continue;

                if (map == null)
                {
                    //不需要startNPC的任务 注意数据库要新建一个类似GM地图的空地图，Description写“其他”
                    MapInfo emptyMap = Globals.MapInfoList.Binding.FirstOrDefault(x => x.Description == "其他".Lang());
                    if (emptyMap == null) continue;

                    if (!Tree.TreeList.TryGetValue(emptyMap, out quests))
                        Tree.TreeList[emptyMap] = quests = new List<QuestInfo>();

                    quests.Add(quest);
                }
                else
                {
                    if (!Tree.TreeList.TryGetValue(map, out quests))
                        Tree.TreeList[map] = quests = new List<QuestInfo>();

                    quests.Add(quest);
                }
            }

            Tree.ListChanged();
        }
        /// <summary>
        /// 更新请求显示
        /// </summary>
        public void UpdateQuestDisplay()
        {
            if (SelectedQuest == null)
            {
                TaskView.RemoveAll();
                return;
            }

            if (SelectedQuest?.QuestInfo == null)
                TaskView.RemoveAll();
            else
                GameScene.Game.GetTaskText(TaskView, SelectedQuest.QuestInfo, SelectedQuest.UserQuest);

            if (SelectedQuest != null)
                SelectedQuest.QuestInfo = SelectedQuest.QuestInfo; // Refresh icons
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Quests.Clear();
                Quests = null;

                RewardArray = null;
                ChoiceArray = null;

                HasChoice = false;
                RandomChoice = false;

                _SelectedQuest = null;
                SelectedQuestChanged = null;

                _NeedUpdate = false;
                NeedUpdateChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (TaskView != null)
                {
                    if (!TaskView.IsDisposed)
                        TaskView.Dispose();

                    TaskView = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (EndLabel != null)
                {
                    if (!EndLabel.IsDisposed)
                        EndLabel.Dispose();

                    EndLabel = null;
                }

                //if (StartLabel != null)
                //{
                //    if (!StartLabel.IsDisposed)
                //        StartLabel.Dispose();

                //    StartLabel = null;
                //}

                if (RewardGrid != null)
                {
                    if (!RewardGrid.IsDisposed)
                        RewardGrid.Dispose();

                    RewardGrid = null;
                }

                if (ChoiceGrid != null)
                {
                    if (!ChoiceGrid.IsDisposed)
                        ChoiceGrid.Dispose();

                    ChoiceGrid = null;
                }

                if (ShowTrackerBox != null)
                {
                    if (!ShowTrackerBox.IsDisposed)
                        ShowTrackerBox.Dispose();

                    ShowTrackerBox = null;
                }

                if (Tree != null)
                {
                    if (!Tree.IsDisposed)
                        Tree.Dispose();

                    Tree = null;
                }

                if (Reward != null)
                {
                    if (!Reward.IsDisposed)
                        Reward.Dispose();

                    Reward = null;
                }

                if (ChooseReward != null)
                {
                    if (!ChooseReward.IsDisposed)
                        ChooseReward.Dispose();

                    ChooseReward = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }

                if (EndLabelButton != null)
                {
                    if (!EndLabelButton.IsDisposed)
                        EndLabelButton.Dispose();

                    EndLabelButton = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 任务树
    /// </summary>
    public class QuestTree : DXControl
    {
        #region Properties

        #region SelectedEntry
        /// <summary>
        /// 选定任务树目录
        /// </summary>
        public QuestTreeEntry SelectedEntry
        {
            get => _SelectedEntry;
            set
            {
                QuestTreeEntry oldValue = _SelectedEntry;
                _SelectedEntry = value;

                OnSelectedEntryChanged(oldValue, value);
            }
        }
        private QuestTreeEntry _SelectedEntry;
        public event EventHandler<EventArgs> SelectedEntryChanged;
        public virtual void OnSelectedEntryChanged(QuestTreeEntry oValue, QuestTreeEntry nValue)
        {
            SelectedEntryChanged?.Invoke(this, EventArgs.Empty);

            if (oValue != null)
                oValue.Selected = false;

            if (nValue != null)
                nValue.Selected = true;
        }

        #endregion

        public Dictionary<MapInfo, List<QuestInfo>> TreeList = new Dictionary<MapInfo, List<QuestInfo>>();

        private DXVScrollBar ScrollBar;

        public List<DXControl> Lines = new List<DXControl>();

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(15, Size.Height);
            ScrollBar.Location = new Point(Size.Width - 17, 0);
            ScrollBar.VisibleSize = Size.Height;
        }
        #endregion

        /// <summary>
        /// 任务树
        /// </summary>
        public QuestTree()
        {
            //Border = true;
            //BorderColour = Color.FromArgb(198, 166, 99);

            ScrollBar = new DXVScrollBar   //滚动条
            {
                Parent = this,
                Change = 22,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateScrollBar();

            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.GameInter, -1, -1, -1, 1225);

            MouseWheel += ScrollBar.DoMouseWheel;
        }

        #region Methods
        /// <summary>
        /// 更新滚动条
        /// </summary>
        public void UpdateScrollBar()
        {
            ScrollBar.MaxValue = Lines.Count * 22;

            for (int i = 0; i < Lines.Count; i++)
                Lines[i].Location = new Point(Lines[i].Location.X, i * 22 - ScrollBar.Value);
        }
        /// <summary>
        /// 列改变
        /// </summary>
        public void ListChanged()
        {
            QuestInfo selectedQuest = SelectedEntry?.QuestInfo;

            foreach (DXControl control in Lines)
                control.Dispose();

            Lines.Clear();

            _SelectedEntry = null;
            QuestTreeEntry firstEntry = null;

            foreach (KeyValuePair<MapInfo, List<QuestInfo>> pair in TreeList)
            {
                QuestTreeHeader header = new QuestTreeHeader //任务树标题
                {
                    Parent = this,
                    Location = new Point(1, Lines.Count * 22),
                    Size = new Size(Size.Width - 30, 20),
                    Map = pair.Key
                };
                header.ExpandButton.MouseClick += (o, e) => ListChanged();
                header.MouseWheel += ScrollBar.DoMouseWheel;

                Lines.Add(header);

                if (!pair.Key.Expanded) continue;

                foreach (QuestInfo quest in pair.Value)
                {
                    QuestTreeEntry entry = new QuestTreeEntry    //任务树条目
                    {
                        Parent = this,
                        Location = new Point(1, Lines.Count * 22),
                        Size = new Size(Size.Width - 30, 20),
                        QuestInfo = quest,
                        Selected = quest == selectedQuest,
                    };
                    entry.MouseWheel += ScrollBar.DoMouseWheel;
                    entry.MouseClick += (o, e) =>
                    {
                        SelectedEntry = entry;
                    };

                    if (firstEntry == null)
                        firstEntry = entry;

                    if (entry.Selected)
                        SelectedEntry = entry;

                    entry.TrackBox.CheckedChanged += (o, e) =>
                    {
                        if (entry.UserQuest.Track == entry.TrackBox.Checked) return;

                        entry.UserQuest.Track = entry.TrackBox.Checked;

                        CEnvir.Enqueue(new C.QuestTrack { Index = entry.UserQuest.Index, Track = entry.UserQuest.Track });

                        GameScene.Game.QuestTrackerBox.PopulateQuests();
                    };

                    Lines.Add(entry);
                }
            }

            if (SelectedEntry == null)
                SelectedEntry = firstEntry;

            UpdateScrollBar();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TreeList.Clear();
                TreeList = null;

                _SelectedEntry = null;
                SelectedEntryChanged = null;

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }

                    }

                    Lines.Clear();
                    Lines = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 任务树标题
    /// </summary>
    public sealed class QuestTreeHeader : DXControl
    {
        #region Properties

        #region Expanded
        /// <summary>
        /// 扩展
        /// </summary>
        public bool Expanded
        {
            get => _Expanded;
            set
            {
                if (_Expanded == value) return;

                bool oldValue = _Expanded;
                _Expanded = value;

                OnExpandedChanged(oldValue, value);
            }
        }
        private bool _Expanded;
        public event EventHandler<EventArgs> ExpandedChanged;
        public void OnExpandedChanged(bool oValue, bool nValue)
        {
            ExpandedChanged?.Invoke(this, EventArgs.Empty);


            ExpandButton.Index = Expanded ? 4871 : 4870;

            Map.Expanded = Expanded;
        }

        #endregion

        #region Map
        /// <summary>
        /// 地图信息
        /// </summary>
        public MapInfo Map
        {
            get => _Map;
            set
            {
                if (_Map == value) return;

                MapInfo oldValue = _Map;
                _Map = value;

                OnMapChanged(oldValue, value);
            }
        }
        private MapInfo _Map;
        public event EventHandler<EventArgs> MapChanged;
        public void OnMapChanged(MapInfo oValue, MapInfo nValue)
        {
            Expanded = Map.Expanded;
            MapLabel.Text = Map.Description;

            MapChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXButton ExpandButton;
        public DXLabel MapLabel;
        #endregion

        /// <summary>
        /// 任务树标题
        /// </summary>
        public QuestTreeHeader()
        {
            ExpandButton = new DXButton   //展开按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4870,
                Location = new Point(2, 2)
            };
            ExpandButton.MouseClick += (o, e) => Expanded = !Expanded;

            MapLabel = new DXLabel   //地图标签
            {
                Parent = this,
                ForeColour = Color.White,
                IsControl = false,
                Location = new Point(25, 2)
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Expanded = false;
                ExpandedChanged = null;

                _Map = null;
                MapChanged = null;


                if (ExpandButton != null)
                {
                    if (!ExpandButton.IsDisposed)
                        ExpandButton.Dispose();

                    ExpandButton = null;
                }

                if (MapLabel != null)
                {
                    if (!MapLabel.IsDisposed)
                        MapLabel.Dispose();

                    MapLabel = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 任务树条目
    /// </summary>
    public sealed class QuestTreeEntry : DXControl
    {
        #region Properties

        #region Selected
        /// <summary>
        /// 选定的内容
        /// </summary>
        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            SelectedChanged?.Invoke(this, EventArgs.Empty);
            Border = Selected;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

        }

        #endregion

        #region QuestInfo
        /// <summary>
        /// 任务信息
        /// </summary>
        public QuestInfo QuestInfo
        {
            get => _QuestInfo;
            set
            {
                QuestInfo oldValue = _QuestInfo;
                _QuestInfo = value;

                OnQuestInfoChanged(oldValue, value);
            }
        }
        private QuestInfo _QuestInfo;
        public event EventHandler<EventArgs> QuestInfoChanged;
        public void OnQuestInfoChanged(QuestInfo oValue, QuestInfo nValue)
        {
            UserQuest = GameScene.Game.QuestLog.FirstOrDefault(x => x.Quest == QuestInfo);

            Type type = QuestInfo.QuestType.GetType();
            MemberInfo[] infos = type.GetMember(QuestInfo.QuestType.ToString());

            DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

            TrackBox.Visible = false;
            QuestNameLabel.Text = "(" + $"{description?.Description ?? QuestInfo.QuestType.ToString()}" + ") " + "<" + QuestInfo.QuestName + ">";

            if (UserQuest == null)
            {
                QuestIcon.BaseIndex = 83; //可接任务图标
                QuestNameLabel.Location = new Point(40, 2);
            }
            else if (UserQuest.Completed)
            {
                QuestIcon.BaseIndex = 91; //当前任务图标
                QuestNameLabel.Location = new Point(40, 2);
            }
            else if (!UserQuest.IsComplete)
            {
                QuestIcon.BaseIndex = 85; //完成任务图标
                TrackBox.Visible = true;
                QuestNameLabel.Location = new Point(65, 2);
            }
            else
            {
                QuestIcon.BaseIndex = 93; //当前任务图标
                TrackBox.Visible = true;
                QuestNameLabel.Location = new Point(65, 2);
            }

            TrackBox.Checked = UserQuest != null && UserQuest.Track;

            QuestInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UserQuest
        /// <summary>
        /// 角色任务
        /// </summary>
        public ClientUserQuest UserQuest
        {
            get => _UserQuest;
            set
            {
                ClientUserQuest oldValue = _UserQuest;
                _UserQuest = value;

                OnUserQuestChanged(oldValue, value);
            }
        }
        private ClientUserQuest _UserQuest;
        public event EventHandler<EventArgs> UserQuestChanged;
        public void OnUserQuestChanged(ClientUserQuest oValue, ClientUserQuest nValue)
        {
            UserQuestChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXCheckBox TrackBox;

        public DXAnimatedControl QuestIcon;
        public DXLabel QuestNameLabel;

        #endregion

        /// <summary>
        /// 任务树条目
        /// </summary>
        public QuestTreeEntry()
        {
            DrawTexture = true;
            BackColour = Color.FromArgb(25, 20, 0);

            BorderColour = Color.FromArgb(198, 166, 99);

            QuestIcon = new DXAnimatedControl    //任务图标
            {
                Parent = this,
                Location = new Point(20, 2),
                Loop = true,
                LibraryFile = LibraryFile.Interface,
                BaseIndex = 83,
                FrameCount = 2,
                AnimationDelay = TimeSpan.FromSeconds(1),
                IsControl = false,
            };

            TrackBox = new DXCheckBox   //跟踪开关
            {
                Parent = this,
                Location = new Point(45, 3),
            };

            QuestNameLabel = new DXLabel   //任务名字标签
            {
                Parent = this,
                Location = new Point(65, 2),
                IsControl = false,
                ForeColour = Color.FromArgb(80, 255, 30),
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _QuestInfo = null;
                QuestInfoChanged = null;

                _UserQuest = null;
                UserQuestChanged = null;

                if (TrackBox != null)
                {
                    if (!TrackBox.IsDisposed)
                        TrackBox.Dispose();

                    TrackBox = null;
                }

                if (QuestIcon != null)
                {
                    if (!QuestIcon.IsDisposed)
                        QuestIcon.Dispose();

                    QuestIcon = null;
                }

                if (QuestNameLabel != null)
                {
                    if (!QuestNameLabel.IsDisposed)
                        QuestNameLabel.Dispose();

                    QuestNameLabel = null;
                }
            }
        }
        #endregion
    }
}