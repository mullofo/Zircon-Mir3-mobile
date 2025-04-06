using Client.Controls;
using Client.Envir;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 任务跟踪进度框
    /// </summary>
    public sealed class QuestTrackerDialog : DXControl
    {
        #region Properties
        /// <summary>
        /// 任务行
        /// </summary>
        public List<DXLabel> Lines = new List<DXLabel>();
        /// <summary>
        /// 任务内容面板
        /// </summary>
        public DXLabel TextPanel;
        #endregion

        /// <summary>
        /// 最大显示任务数
        /// </summary>
        const int MAX = 5;
        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);
            if (TextPanel != null)
            {
                TextPanel.Size = nValue;
            }
        }
        /// <summary>
        /// 任务进度框界面
        /// </summary>
        public QuestTrackerDialog()
        {
            PassThrough = true;
            AllowResize = false;
            Size = new Size(210, 0);

            TextPanel = new DXLabel     //内容面板
            {
                Parent = this,
                PassThrough = true,
                Size = Size,
                BackColour = Color.Black,
                Opacity = 0.3F
            };
        }

        #region Methods
        /// <summary>
        /// 填充任务
        /// </summary>
        public void PopulateQuests()
        {
            foreach (DXLabel line in Lines)
                line.Dispose();

            Lines.Clear();

            if (!Config.QuestTrackerVisible)
            {
                Visible = false;
                return;
            }

            var height = 0;
            var count = 0;
            foreach (QuestInfo quest in GameScene.Game.QuestBox.CurrentTab.Quests)
            {
                if (count == MAX)
                {
                    break;
                }

                ClientUserQuest userQuest = GameScene.Game.QuestLog.First(x => x.Quest == quest);

                if (!userQuest.Track) continue;
                Type type = quest.QuestType.GetType();
                MemberInfo[] infos = type.GetMember(quest.QuestType.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                DXLabel label = new DXLabel   //任务信息
                {
                    Text = $"<{description?.Description ?? quest.QuestType.ToString()}> {quest.QuestName}",
                    ForeColour = Color.Yellow,
                    Parent = TextPanel,
                    Outline = true,
                    OutlineColour = Color.Black,  //轮廓线颜色 黑色
                    IsControl = false,
                    Location = new Point(15, height)
                };

                DXAnimatedControl QuestIcon = new DXAnimatedControl   //任务图标
                {
                    Parent = TextPanel,
                    Location = new Point(2, height),
                    Loop = true,
                    LibraryFile = LibraryFile.Interface,
                    BaseIndex = 83,
                    FrameCount = 2,
                    AnimationDelay = TimeSpan.FromSeconds(1),
                    IsControl = false,
                };
                label.Disposing += (o, e) =>
                {
                    QuestIcon.Dispose();
                };

                label.LocationChanged += (o, e) =>
                {
                    QuestIcon.Location = new Point(QuestIcon.Location.X, label.Location.Y);
                };

                QuestIcon.BaseIndex = !userQuest.IsComplete ? 85 : 93;

                //if (userQuest.IsComplete)
                //    label.Text += " (完成)";

                Lines.Add(label);

                foreach (QuestTask task in quest.Tasks)
                {
                    height += 15;
                    ClientUserQuestTask userTask = userQuest.Tasks.FirstOrDefault(x => x.Task == task);

                    //if (userTask != null && userTask.Completed) continue;
                    var quests = GameScene.Game.GetTrackerText(task, userQuest);
                    DXLabel label1 = new DXLabel
                    {
                        Text = quests[0],
                        Parent = TextPanel,
                        ForeColour = Color.White,
                        Outline = true,
                        OutlineColour = Color.Black,
                        IsControl = false,
                        Location = new Point(15, height)
                    };

                    var label2 = new DXLabel
                    {
                        Text = quests[1],
                        Parent = TextPanel,
                        ForeColour = Color.White,
                        Outline = true,
                        OutlineColour = Color.Black,
                        IsControl = false,

                    };
                    label2.Location = new Point(TextPanel.Size.Width - label2.Size.Width - 15, height);

                    if (userTask != null && userTask.Completed)
                    {
                        label1.ForeColour = Color.FromArgb(0, 130, 250);
                        label2.ForeColour = Color.FromArgb(0, 130, 250);
                    }

                    Lines.Add(label1);
                    Lines.Add(label2);
                }

                height += 15;
                count++;
            }

            Visible = Lines.Count > 0;

            Size = new Size(Size.Width, height + 4);
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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

                if (TextPanel != null)
                {
                    if (!TextPanel.IsDisposed)
                        TextPanel.Dispose();

                    TextPanel = null;
                }
            }
        }
        /// <summary>
        /// 更换位置
        /// </summary>
        public void ChangeLocation()
        {
            Location = new Point(Location.X, GameScene.Game.MiniMapBox.Size.Height + 10);
        }
        #endregion
    }
}
