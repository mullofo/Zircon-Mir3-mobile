using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Library;
using Library.Network.ClientPackets;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 成就功能
    /// </summary>
    public sealed class AchievementTab : DXTab
    {
        #region Properties

        public DXButton AllTab, RegularTab, DailyTab, QuestTab, HuntTab, PKTab, MapTab, EventTab;
        public DXButton ActiveTab;
        private DXButton TitleQuery, TitleCancel;
        public DXButton QuestButton, AchievementButton, CanAcceptButton, CompletedButton;

        public DXLabel CurrentTitle;

        private DXLabel AllLabel, RegularLabel, DailyLabel, QuestLabel, HuntLabel, PKLabel, MapLabel, EventLabel;

        private DXMirScrollBar ScrollBar;

        public List<AchievementRow> Rows;
        public List<AchievementRow> AllRows;
        private List<ClientUserAchievement> SelectedAchievements = GameScene.Game.AchievementLog;

        public AchievementRow SelectedAchievementRow; //勾选的row

        #endregion

        /// <summary>
        /// 成就界面
        /// </summary>
        public AchievementTab()
        {
            Size = new Size(717, 465);
            Rows = new List<AchievementRow>();
            AllRows = new List<AchievementRow>();

            CurrentTitle = new DXLabel
            {
                Parent = this,
                Location = new Point(0, 10),//-10 -42
                ForeColour = Color.White,
                IsControl = false,
                Text = "无".Lang(),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.TextBoxControl,
            };
            CurrentTitle.Size = new Size(172, 20);

            AllTab = new DXButton
            {
                Index = 522,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 48),
                Parent = this,
                Opacity = 1,
                Text = "所有".Lang(),
            };
            AllTab.MouseClick += SelectedTabChanged;

            AllLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 50),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "所有".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            RegularTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 78),
                Parent = this,
                Opacity = 1,
                Text = "普通".Lang(),
            };
            RegularTab.MouseClick += SelectedTabChanged;

            RegularLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 80),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "普通".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            DailyTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 108),
                Parent = this,
                Opacity = 1,
                Text = "生活".Lang(),
            };
            DailyTab.MouseClick += SelectedTabChanged;

            DailyLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 110),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "生活".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            QuestTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 138),
                Parent = this,
                Opacity = 1,
                Text = "任务".Lang(),
            };
            QuestTab.MouseClick += SelectedTabChanged;

            QuestLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 140),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "任务".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            HuntTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 168),
                Parent = this,
                Opacity = 1,
                Text = "杀怪".Lang(),
            };
            HuntTab.MouseClick += SelectedTabChanged;

            HuntLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 170),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "杀怪".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            PKTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 198),
                Parent = this,
                Opacity = 1,
                Text = "战斗".Lang(),
            };
            PKTab.MouseClick += SelectedTabChanged;

            PKLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 200),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "战斗".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            MapTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 228),
                Parent = this,
                Opacity = 1,
                Text = "地区".Lang(),
            };
            MapTab.MouseClick += SelectedTabChanged;

            MapLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 230),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "地区".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            EventTab = new DXButton
            {
                Index = 521,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(0, 258),
                Parent = this,
                Opacity = 1,
                Text = "活动".Lang(),
            };
            EventTab.MouseClick += SelectedTabChanged;

            EventLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(62, 260),
                ForeColour = Color.Gold,
                IsControl = false,
                Text = "活动".Lang(),
                Font = new Font("Microsoft YaHei UI", CEnvir.FontSize(11F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            ScrollBar = new DXMirScrollBar
            {
                Parent = this,
                Size = new Size(15, 387),
                Location = new Point(Size.Width - 23, 6),
                Change = 90,
                MaxValue = 95,
            };
            ScrollBar.VisibleSize = ScrollBar.Size.Height;
            ScrollBar.ValueChanged += ScrollBar_ValueChanged;
            MouseWheel += ScrollBar.DoMouseWheel;

            TitleQuery = new DXButton
            {
                Index = 537,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(387, 400),
                Parent = this,
                Enabled = false,
            };

            TitleCancel = new DXButton
            {
                Index = 532,
                LibraryFile = LibraryFile.GameInter2,
                Location = new Point(487, 400),
                Parent = this,
                Enabled = true,
            };
            TitleCancel.MouseClick += (sender, args) =>
            {
                CEnvir.Enqueue(new TakeOffAchievementTitle());
            };
        }

        #region Methods
        public void Init()
        {
            if (ActiveTab == null)
            {
                SelectedTabChanged(AllTab, null);
            }
        }
        /// <summary>
        /// 填充所有行
        /// </summary>
        public void PopulateAllRows()
        {
            if (AllRows.Count != GameScene.Game.AchievementLog.Count)
            {
                for (int i = 0; i < GameScene.Game.AchievementLog.Count; i++)
                {
                    AchievementRow newRow = new AchievementRow(GameScene.Game.AchievementLog[i])
                    {
                        Parent = this,
                        Location = new Point(175, 12 + (i % 4) * 95),
                        Visible = false,
                    };
                    newRow.MouseWheel += ScrollBar.DoMouseWheel;
                    AllRows.Add(newRow);

                    if (GameScene.Game.AchievementLog[i].IsComplete) newRow.ToggleCompleted();
                }
            }
        }
        /// <summary>
        /// 刷新行
        /// </summary>
        public void RefreshRows()
        {
            PopulateAllRows();

            Rows.Clear();

            foreach (AchievementRow row in AllRows)
            {
                if (SelectedAchievements.Contains(row.Achievement))
                {
                    row.Parent = this;
                    row.Location = new Point(175, 12 + (Rows.Count % 4) * 95);
                    row.Visible = Rows.Count < 4;
                    row.PassThrough = true;
                    row.MouseWheel += ScrollBar.DoMouseWheel;
                    Rows.Add(row);
                }
                else
                {
                    row.Visible = false;
                }
            }

            ScrollBar.MaxValue = 90 * Rows.Count;
            ScrollBar.VisibleSize = Rows.Count;
            ScrollBar.Value = 1;
        }

        /// <summary>
        /// 更改成就称号
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="sendPacket"></param>
        public void ChangeTitle(string newTitle, bool sendPacket = true)
        {
            if (string.IsNullOrEmpty(newTitle))
            {
                CurrentTitle.Text = "";
                if (SelectedAchievementRow?.UseTitleBox != null) SelectedAchievementRow.UseTitleBox.Checked = false;

                if (sendPacket)
                    CEnvir.Enqueue(new TakeOffAchievementTitle());
            }
            else
            {
                AchievementRow newRow = AllRows.FirstOrDefault(x => x.Achievement.Achievement.Title == newTitle);

                if (SelectedAchievementRow != null && newRow != SelectedAchievementRow)
                {
                    SelectedAchievementRow.UseTitleBox.Checked = false;
                }

                SelectedAchievementRow = newRow;

                if (SelectedAchievementRow != null)
                {
                    SelectedAchievementRow.UseTitleBox.Checked = true;
                    CurrentTitle.Text = newTitle;
                    CurrentTitle.Size = new Size(172, 20);

                    if (sendPacket)
                        CEnvir.Enqueue(new WearAchievementTitle { AchievementIndex = SelectedAchievementRow.Achievement.AchievementIndex });
                }
            }
        }
        /// <summary>
        /// 滚动条改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            int y = ScrollBar.Value / 90;
            int index = 0;

            for (int i = 0; i < Rows.Count; i++)
            {
                if (i < y || i > y + 3)
                {
                    Rows[i].Visible = false;
                    continue;
                }

                Rows[i].Location = new Point(175, 12 + index * 95);
                Rows[i].Visible = true;
                index++;
            }
        }
        /// <summary>
        /// 所选选项卡改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectedTabChanged(object sender, EventArgs e)
        {
            if (!(sender is DXButton clickedTab)) return;

            ActiveTab = clickedTab;
            AllTab.Index = 521;
            RegularTab.Index = 521;
            DailyTab.Index = 521;
            QuestTab.Index = 521;
            HuntTab.Index = 521;
            PKTab.Index = 521;
            MapTab.Index = 521;
            EventTab.Index = 521;

            ActiveTab.Index = 522;

            switch (ActiveTab.Text)
            {
                case "所有":
                case "All":
                    SelectedAchievements = GameScene.Game.AchievementLog;
                    break;
                case "普通":
                case "Ordinary":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Regular).ToList();
                    break;
                case "生活":
                case "Life":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Daily).ToList();
                    break;
                case "任务":
                case "Task":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Quest).ToList();
                    break;
                case "杀怪":
                case "KillingMon":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Hunt).ToList();
                    break;
                case "战斗":
                case "Fight":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.PK).ToList();
                    break;
                case "地区":
                case "Area":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Map).ToList();
                    break;
                case "活动":
                case "Activity":
                    SelectedAchievements = GameScene.Game.AchievementLog
                        .Where(x => x.Achievement.Category == AchievementCategory.Event).ToList();
                    break;
                default:
                    SelectedAchievements = GameScene.Game.AchievementLog;
                    break;
            }

            RefreshRows();
        }

        #endregion
    }
    /// <summary>
    /// 成就任务行
    /// </summary>
    public sealed class AchievementRow : DXControl
    {
        private DXImageControl FinishBackground;

        private DXLabel AchievementNameLabel, UseTitleLabel, DescriptionLabel, ProgressLabel, AchievementGradeLabel, FinishDateLabel;
        public DXCheckBox UseTitleBox;
        private DXImageControl EmptyReward, NonEmptyReward;

        public ClientUserAchievement Achievement;

        /// <summary>
        /// 成就任务行
        /// </summary>
        /// <param name="achievement"></param>
        public AchievementRow(ClientUserAchievement achievement)
        {
            IsControl = true;
            Size = new Size(516, 90);

            Achievement = achievement;

            FinishBackground = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 510,
                Opacity = 1F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false,
            };

            UseTitleBox = new DXCheckBox
            {
                Parent = this,
                ReadOnly = true,
                Location = new Point(22, 8),
                Visible = false,
                Checked = GameScene.Game?.User?.AchievementTitle == achievement.Achievement?.Title,
            };
            UseTitleBox.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;  //如果是观察者 返回

                if (achievement.IsComplete)
                {
                    UseTitleBox.Checked = !UseTitleBox.Checked;
                    if (UseTitleBox.Checked)
                    {
                        GameScene.Game.User.AchievementTitle = achievement.Achievement.Title;
                        GameScene.Game.QuestBox?.TitleTab.ChangeTitle(achievement.Achievement.Title);
                    }
                    else
                    {
                        GameScene.Game.User.AchievementTitle = string.Empty;
                        GameScene.Game.QuestBox?.TitleTab.ChangeTitle(string.Empty);
                    }
                }
            };

            UseTitleLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(38, 4),
                ForeColour = Color.DarkGray,
                IsControl = false,
                Text = $"[{"使用称号".Lang()}]",
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            };

            AchievementNameLabel = new DXLabel
            {
                Parent = this,
                //Size = new Size(300, 25),
                ForeColour = Color.DarkGray,
                IsControl = false,
                Text = achievement.Achievement.Title,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
            AchievementNameLabel.Location = new Point(260 - AchievementNameLabel.DisplayArea.Width / 2, 11);

            DescriptionLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.DarkGray,
                IsControl = false,
                Text = achievement.Achievement.Description,
                //Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                //AutoSize = true,
                Location = new Point(80, 35),
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.TextBoxControl |
                             TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis,
            };
            DescriptionLabel.Size = new Size(360, 30);

            AchievementGradeLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(25, 75),
                ForeColour = Color.DarkGray,
                IsControl = false,
                AutoSize = true,
                Visible = true,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            switch (achievement.Achievement.Grade)
            {
                case 0:
                    AchievementGradeLabel.Text = $"[{"初级".Lang()}]";
                    break;
                case 1:
                    AchievementGradeLabel.Text = $"[{"中级".Lang()}]";
                    break;
                case 2:
                    AchievementGradeLabel.Text = $"[{"高级".Lang()}]";
                    break;
            }

            ProgressLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(80, 70),
                ForeColour = Color.DarkGray,
                IsControl = false,
                Text = GetProgressText(),
                AutoSize = true,
                Visible = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
            ProgressLabel.Size = new Size(360, 20);

            FinishDateLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(435, 70),
                ForeColour = Color.FromArgb(255, 200, 0),
                IsControl = false,
                Text = achievement.CompleteDate,
                AutoSize = true,
                Visible = false,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Regular),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };

            List<AchievementReward> rewards = null;
            if (GameScene.Game != null && GameScene.Game.User != null)
            {
                switch (GameScene.Game.User.Class)
                {
                    case MirClass.Warrior:
                        rewards = achievement.Achievement.AchievementRewards.Where(x =>
                            x.Class.HasFlag(RequiredClass.Warrior)).ToList();
                        break;
                    case MirClass.Wizard:
                        rewards = achievement.Achievement.AchievementRewards.Where(x =>
                            x.Class.HasFlag(RequiredClass.Wizard)).ToList();
                        break;
                    case MirClass.Taoist:
                        rewards = achievement.Achievement.AchievementRewards.Where(x =>
                            x.Class.HasFlag(RequiredClass.Taoist)).ToList();
                        break;
                    case MirClass.Assassin:
                        rewards = achievement.Achievement.AchievementRewards.Where(x =>
                            x.Class.HasFlag(RequiredClass.Assassin)).ToList();
                        break;
                }
            }

            if (rewards != null && rewards.Count > 0)
            {
                NonEmptyReward = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.GameInter2,
                    Index = 2930,
                    Opacity = 1F,
                    Location = new Point(26, 35),
                    IsControl = true,
                    //PassThrough = true,
                    Visible = true,
                    Hint = "",
                };

                foreach (AchievementReward reward in rewards)
                {
                    ItemInfo item = reward.Item;
                    int amount = reward.Amount;
                    NonEmptyReward.Hint += item.Lang(p => p.ItemName) + " x " + amount + "\n";
                }
                NonEmptyReward.Hint = NonEmptyReward.Hint.TrimEnd('\r', '\n');
            }
            else
            {
                EmptyReward = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.GameInter2,
                    Index = 520,
                    Opacity = 1F,
                    Location = new Point(26, 32),
                    IsControl = true,
                    PassThrough = true,
                    Visible = true,
                };
            }
        }
        /// <summary>
        /// 获取成就状态要求文本
        /// </summary>
        /// <param name="requirement"></param>
        /// <returns></returns>
        public string GetStatusRequirementText(AchievementRequirement requirement)
        {
            switch (requirement.RequirementType)
            {
                case AchievementRequirementType.UseMagic:
                    return "使用".Lang() + "[" + requirement.MagicParameter?.Name + "], ";
                case AchievementRequirementType.InMap:
                    return "在".Lang() + "[" + requirement.MapParameter?.Description + "], ";
                case AchievementRequirementType.WearingItem:
                    return "佩戴".Lang() + "[" + requirement.ItemParameter?.Lang(p => p.ItemName) + "], ";
                case AchievementRequirementType.CarryingItem:
                    return "携带".Lang() + "[" + requirement.ItemParameter?.Lang(p => p.ItemName) + "], ";
                case AchievementRequirementType.LevelLessThan:
                    return "等级小于".Lang() + "[" + requirement.RequiredAmount + "级".Lang() + "]" + "时".Lang() + ", ";
                case AchievementRequirementType.LevelGreaterOrEqualThan:
                    return "等级大于等于".Lang() + "[" + requirement.RequiredAmount + "级".Lang() + "]" + "时".Lang() + ", ";
                case AchievementRequirementType.DigTimes:
                    if (requirement.ItemParameter != null)
                    {
                        return "挖出".Lang() + "[" + requirement.ItemParameter.Lang(p => p.ItemName) + "], ";
                    }
                    break;
            }

            return string.Empty;
        }
        /// <summary>
        /// 获取成就进度文本
        /// </summary>
        /// <returns></returns>
        public string GetProgressText()
        {
            if (Achievement.Achievement.IsHidden) return "[" + "隐藏成就".Lang() + "]";

            string prefix = "";
            string res = "";

            foreach (var temp in Achievement.Requirements)
            {
                AchievementRequirement requirement = temp.Requirement;
                prefix += GetStatusRequirementText(requirement);
                if (Globals.StatusAchievementRequirementTypeList.Contains(requirement.RequirementType))
                {
                    continue;
                }

                switch (requirement.RequirementType)
                {
                    case AchievementRequirementType.KillMonster:
                        res += requirement.MonsterParameter.MonsterName + ": " + temp.CurrentValue + "/" +
                               requirement.RequiredAmount + " ";
                        break;
                    case AchievementRequirementType.RankAll:
                    case AchievementRequirementType.RankWarrior:
                    case AchievementRequirementType.RankWizard:
                    case AchievementRequirementType.RankTaoist:
                    case AchievementRequirementType.RankAssassin:
                        res += $"rank.normal".Lang(temp.CurrentValue + 1);
                        break;

                    default:
                        res += EnumHelp.GetDescription(requirement.RequirementType) + " " + temp.CurrentValue + "/" +
                              requirement.RequiredAmount + " ";
                        break;
                }

            }
            return string.IsNullOrEmpty(res) ? "无".Lang() : prefix + res;
        }
        /// <summary>
        /// 切换完成
        /// </summary>
        public void ToggleCompleted()
        {
            FinishBackground.Visible = true;
            UseTitleBox.Visible = true;
            UseTitleLabel.ForeColour = Color.FromArgb(255, 200, 0);
            AchievementNameLabel.ForeColour = Color.Gold;
            DescriptionLabel.ForeColour = Color.White;
            AchievementGradeLabel.ForeColour = Color.White;
            ProgressLabel.ForeColour = Color.LawnGreen;
            FinishDateLabel.Visible = true;

            if (NonEmptyReward != null)
            {
                NonEmptyReward.Enabled = true;
            }
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Achievement = null;

                if (FinishBackground != null)
                {
                    if (!FinishBackground.IsDisposed)
                        FinishBackground.Dispose();

                    FinishBackground = null;
                }

                if (AchievementNameLabel != null)
                {
                    if (!AchievementNameLabel.IsDisposed)
                        AchievementNameLabel.Dispose();

                    AchievementNameLabel = null;
                }

                if (UseTitleLabel != null)
                {
                    if (!UseTitleLabel.IsDisposed)
                        UseTitleLabel.Dispose();

                    UseTitleLabel = null;
                }

                if (DescriptionLabel != null)
                {
                    if (!DescriptionLabel.IsDisposed)
                        DescriptionLabel.Dispose();

                    DescriptionLabel = null;
                }

                if (ProgressLabel != null)
                {
                    if (!ProgressLabel.IsDisposed)
                        ProgressLabel.Dispose();

                    ProgressLabel = null;
                }

                if (AchievementGradeLabel != null)
                {
                    if (!AchievementGradeLabel.IsDisposed)
                        AchievementGradeLabel.Dispose();

                    AchievementGradeLabel = null;
                }

                if (FinishDateLabel != null)
                {
                    if (!FinishDateLabel.IsDisposed)
                        FinishDateLabel.Dispose();

                    FinishDateLabel = null;
                }

                if (UseTitleBox != null)
                {
                    if (!UseTitleBox.IsDisposed)
                        UseTitleBox.Dispose();

                    UseTitleBox = null;
                }

                if (EmptyReward != null)
                {
                    if (!EmptyReward.IsDisposed)
                        EmptyReward.Dispose();

                    EmptyReward = null;
                }

                if (NonEmptyReward != null)
                {
                    if (!NonEmptyReward.IsDisposed)
                        NonEmptyReward.Dispose();

                    NonEmptyReward = null;
                }
            }
        }
        #endregion

    }
}
