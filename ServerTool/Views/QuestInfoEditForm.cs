using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class QuestInfoEditForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public QuestInfoEditForm()
        {
            InitializeComponent();

            //任务列表
            QuestInfogridControl.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            NPCLookUpEdit.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;
            QuestTypeImageComboBox.Items.AddEnum<QuestType>();
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            //要求页面
            //RequestgridControl.DataSource = null;
            QuestLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            RequestTypeComboBox.Items.AddEnum<QuestRequirementType>();
            ClassTypeComboBox.Items.AddEnum<RequiredClass>();

            //任务奖励页面  
            RewardClassComboBox.Items.AddEnum<RequiredClass>();
            RewardItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            //具体执行任务
            TaskItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            TaskImageComboBox.Items.AddEnum<QuestTaskType>();

            //任务怪物说明
            uestTaskMonLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            TaskMapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;

            SelectEditItem.Tag = 0;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(QuestInfogridView);
            SMain.SetUpView(RequestPagegridView);
            SMain.SetUpView(TasksgridView);
            SMain.SetUpView(MonsterDetailsgridView);
            SMain.SetUpView(RewardsgridView);

            TasksgridView.RowClick += TasksgridView_RowClick;
        }

        private void TasksgridView_RowClick(object sender, RowClickEventArgs e)
        {
            var list = TasksgridControl.DataSource as MirDB.DBBindingList<QuestTask>;
            if (list == null || list.Count() < e.RowHandle + 1 || e.RowHandle < 0)
            {
                return;
            }
            var task = list[e.RowHandle];
            MonsterDetailsgridControl.DataSource = task.MonsterDetails;
        }

        public QuestInfo SelectPage//选中的QuestInfo
        {
            get { return _SelectPage; }
            set
            {
                if (_SelectPage == value)
                {
                    return;
                }

                var oldValue = _SelectPage;
                _SelectPage = value;

                if (SelectPage != null)
                {
                    RequestgridControl.DataSource = SelectPage.Requirements;
                    RewardsgridControl.DataSource = SelectPage.Rewards;
                    TasksgridControl.DataSource = SelectPage.Tasks;
                    if (SelectPage.Tasks.Count > 0)
                    {
                        SelectQuestTask = SelectPage.Tasks[0];
                    }
                    else SelectQuestTask = null;

                    AcceptText.Text = SelectPage.AcceptText;
                    ProgressText.Text = SelectPage.ProgressText;
                    CompletedText.Text = SelectPage.CompletedText;
                    ArchiveText.Text = SelectPage.ArchiveText;
                }
                else
                {
                    AcceptText.Text = "";
                    ProgressText.Text = "";
                    CompletedText.Text = "";
                    ArchiveText.Text = "";
                    barEditItem_RequestQuestPage.EditValue = "";
                    SelectEditItem.Tag = -1;
                    SelectEditItem.EditValue = "";

                    SelectRequestQuestPage = null;
                }
            }
        }
        public QuestInfo _SelectPage;

        public QuestInfo MainPage;

        public QuestInfo SelectRequestQuestPage;
        public QuestTask SelectQuestTask
        {
            get { return _SelectQuestTask; }
            set
            {
                if (_SelectQuestTask == value)
                {
                    return;
                }

                var oldValue = _SelectQuestTask;
                _SelectQuestTask = value;

                if (SelectQuestTask != null)
                {
                    MonsterDetailsgridControl.DataSource = SelectQuestTask.MonsterDetails;
                }
                else
                {
                    MonsterDetailsgridControl.DataSource = null;
                }
            }
        }
        public QuestTask _SelectQuestTask;

        private void QuestInfoView_SelectionChanged(object sender, EventArgs e)
        {
            GridView view = QuestInfogridControl.FocusedView as GridView;

            if (view == null) return;
            SelectPage = view.GetFocusedRow() as QuestInfo;
            MainPage = SelectPage;
        }

        private void QuestInfoEditForm_SizeChanged(object sender, EventArgs e)
        {
            QuestInfogridControl.Location = new Point(10, 10);
            QuestInfogridControl.Size = new Size(ClientSize.Width / 3, ClientSize.Height / 2);

            AcceptText.Location = new Point(QuestInfogridControl.Location.X + QuestInfogridControl.Size.Width + 6, 10);
            AcceptText.Size = new Size(ClientSize.Width / 7 + 16, QuestInfogridControl.Size.Height);

            ProgressText.Location = new Point(AcceptText.Location.X + AcceptText.Size.Width + 6, 10);
            ProgressText.Size = new Size(ClientSize.Width / 7 + 16, QuestInfogridControl.Size.Height);

            CompletedText.Location = new Point(ProgressText.Location.X + ProgressText.Size.Width + 6, 10);
            CompletedText.Size = new Size(ClientSize.Width / 7 + 16, QuestInfogridControl.Size.Height);

            ArchiveText.Location = new Point(CompletedText.Location.X + CompletedText.Size.Width + 6, 10);
            ArchiveText.Size = new Size(ClientSize.Width / 7 + 16, QuestInfogridControl.Size.Height);

            RequestgridControl.Location = new Point(5, QuestInfogridControl.Location.Y + QuestInfogridControl.Size.Height + 10);
            RequestgridControl.Size = new Size(ClientSize.Width / 4 - 10, ClientSize.Height / 2 - 50);

            RewardsgridControl.Location = new Point(5 + RequestgridControl.Location.X + RequestgridControl.Size.Width, QuestInfogridControl.Location.Y + QuestInfogridControl.Size.Height + 10);
            RewardsgridControl.Size = new Size(ClientSize.Width / 4 - 10, ClientSize.Height / 2 - 50);

            TasksgridControl.Location = new Point(5 + RewardsgridControl.Location.X + RewardsgridControl.Size.Width, QuestInfogridControl.Location.Y + QuestInfogridControl.Size.Height + 10);
            TasksgridControl.Size = new Size(ClientSize.Width / 4 - 10, ClientSize.Height / 2 - 50);

            MonsterDetailsgridControl.Location = new Point(5 + TasksgridControl.Location.X + TasksgridControl.Size.Width, QuestInfogridControl.Location.Y + QuestInfogridControl.Size.Height + 10);
            MonsterDetailsgridControl.Size = new Size(ClientSize.Width / 4 - 10, ClientSize.Height / 2 - 50);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SelectPage != null)
            {
                SelectPage.AcceptText = AcceptText.Text;
                SelectPage.ProgressText = ProgressText.Text;
                SelectPage.CompletedText = CompletedText.Text;
                SelectPage.ArchiveText = ArchiveText.Text;
            }
        }

        private void RequestPagegridView_MouseUp(object sender, MouseEventArgs e)
        {
            SelectEditItem.EditValue = "检测要求页面";
            SelectEditItem.Tag = 1;
        }

        private void RewardsgridView_MouseUp(object sender, MouseEventArgs e)
        {
            SelectEditItem.EditValue = "任务报酬页面";
            SelectEditItem.Tag = 2;
        }

        private void TasksgridView_MouseUp(object sender, MouseEventArgs e)
        {
            SelectEditItem.EditValue = "任务信息页面";
            SelectEditItem.Tag = 3;
        }

        private void MonsterDetailsgridView_MouseUp(object sender, MouseEventArgs e)
        {
            SelectEditItem.EditValue = "任务怪物信息页面";
            SelectEditItem.Tag = 4;
        }

        private void QuestInfogridView_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = QuestInfogridControl.FocusedView as GridView;

            if (view == null) return;
            QuestInfo SelectInfo = view.GetFocusedRow() as QuestInfo;

            SelectPage = SelectInfo;
            MainPage = SelectPage;
        }
    }
}