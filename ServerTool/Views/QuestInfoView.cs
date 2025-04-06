using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class QuestInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public QuestInfoView()           //任务信息视图
        {
            InitializeComponent();

            RequirementImageComboBox.Items.AddEnum<QuestRequirementType>();
            TaskImageComboBox.Items.AddEnum<QuestTaskType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            QuestTypeImageComboBox.Items.AddEnum<QuestType>();

            QuestInfoGridControl.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;

            QuestInfoLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            NPCLookUpEdit.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(QuestInfoGridView);
            SMain.SetUpView(RequirementsGridView);
            SMain.SetUpView(TaskGridView);
            SMain.SetUpView(MonsterDetailsGridView);
            SMain.SetUpView(RewardsGridView);

            RewardsGridView.CellValueChanged += RewardsGridView_CellValueChanged;
        }

        private void RewardsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e) //随机获取和自由选择 只能勾选其中之一
        {

            RewardsGridView.CellValueChanged -= RewardsGridView_CellValueChanged;

            GridView view = sender as GridView;
            if (view == null) return;

            bool thisValue = false;

            if (e.Column.Caption == "勾选的道具可以选择领取")
            {
                thisValue = (bool)e.Value;
                if (thisValue)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["Random"], false);
                }
            }
            else if (e.Column.Caption == "勾选的道具随机获得")
            {
                thisValue = (bool)e.Value;
                if (thisValue)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["Choice"], false);
                }
            }

            RewardsGridView.CellValueChanged += RewardsGridView_CellValueChanged;
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<QuestInfo>.ExportExcel(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                Helpers.HelperExcel<QuestInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*[Obsolete]
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string strPath = Convert.ToString(ConfigurationManager.AppSettings["ExportExcelPath"]);
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                string strFullPath = strPath + this.Text + ".xls";

                IList<QuestInfo> QuestInfoList = SMain.Session.GetCollection<QuestInfo>().Binding;
                foreach (QuestInfo QuestInfo in QuestInfoList)
                {
                    QuestInfo.IndexCopy = QuestInfo.Index;
                    QuestInfo.StartNPCName = QuestInfo.StartNPC == null ? "" : QuestInfo.StartNPC.NPCName;
                    QuestInfo.FinishNPCName = QuestInfo.FinishNPC == null ? "" : QuestInfo.FinishNPC.NPCName;
                    QuestInfo.StartItemName = QuestInfo.StartItem == null ? "[Item is null]" : QuestInfo.StartItem.ItemName;
                    QuestInfo.QuestTypeName = EnumHelp.GetDescription(QuestInfo.QuestType);
                }

                #region QuestInfo
                DataTable dtQuestInfo = NPOIHelper.ToDataTable<QuestInfo>(QuestInfoList);
                Dictionary<string, DataTable> dic = new Dictionary<string, DataTable>();
                dic.Add("QuestInfo", dtQuestInfo);
                #endregion

                #region Requirements
                List<QuestRequirement> QuestRequirementist = new List<QuestRequirement>();
                foreach (QuestInfo QuestInfo in QuestInfoList)
                {
                    List<QuestRequirement> QuestRequirementListTemp = new List<QuestRequirement>(QuestInfo.Requirements.ToList());
                    QuestRequirementListTemp.ForEach(o => o.QuertInfoIndex = QuestInfo.Index);
                    QuestRequirementListTemp.ForEach(o => o.RequirementName = Convert.ToString(o.Requirement));
                    QuestRequirementListTemp.ForEach(o => o.ClassName = EnumHelp.GetDescription(o.Class));
                    QuestRequirementListTemp.ForEach(o => o.QuestParameterName = o.QuestParameter == null ? "" : Convert.ToString(o.QuestParameter.QuestName));
                    QuestRequirementist.AddRange(QuestRequirementListTemp);
                }
                DataTable dtQuestRequirement = NPOIHelper.ToDataTable<QuestRequirement>(QuestRequirementist);
                dic.Add("Requirements", dtQuestRequirement);
                #endregion

                #region Rewards
                List<QuestReward> QuestRewardList = new List<QuestReward>();
                foreach (QuestInfo QuestInfo in QuestInfoList)
                {
                    List<QuestReward> QuestRewardListTemp = new List<QuestReward>(QuestInfo.Rewards.ToList());
                    QuestRewardListTemp.ForEach(o => o.QuertInfoIndex = QuestInfo.Index);
                    QuestRewardListTemp.ForEach(o => o.ItemName = o.Item.ItemName);
                    QuestRewardListTemp.ForEach(o => o.ClassName = EnumHelp.GetDescription(o.Class));
                    QuestRewardList.AddRange(QuestRewardListTemp);
                }
                DataTable dtRewards = NPOIHelper.ToDataTable<QuestReward>(QuestRewardList);
                dic.Add("Rewards", dtRewards);
                #endregion

                #region Tasks
                List<QuestTask> QuestTaskList = new List<QuestTask>();
                foreach (QuestInfo QuestInfo in QuestInfoList)
                {
                    List<QuestTask> QuestTaskListTemp = new List<QuestTask>(QuestInfo.Tasks.ToList());
                    QuestTaskListTemp.ForEach(o => o.IndexCopy = o.Index);
                    QuestTaskListTemp.ForEach(o => o.QuertInfoIndex = QuestInfo.Index);
                    QuestTaskListTemp.ForEach(o => o.TaskName = Convert.ToString(o.Task));
                    QuestTaskListTemp.ForEach(o => o.ItemParameterName = o.ItemParameter == null ? "" : o.ItemParameter.ItemName);
                    QuestTaskList.AddRange(QuestTaskListTemp);
                }
                DataTable dtTasks = NPOIHelper.ToDataTable<QuestTask>(QuestTaskList);
                dic.Add("Tasks", dtTasks);
                #endregion

                #region Task_MonsterDetails
                List<QuestTaskMonsterDetails> QuestTaskMonsterDetailsList = new List<QuestTaskMonsterDetails>();
                foreach (QuestInfo QuestInfo in QuestInfoList)
                {
                    List<QuestTask> QuestTaskListNew = QuestInfo.Tasks.ToList();
                    foreach (QuestTask QuestTask in QuestTaskListNew)
                    {
                        List<QuestTaskMonsterDetails> QuestTaskMonsterDetailsListTemp = new List<QuestTaskMonsterDetails>(QuestTask.MonsterDetails.ToList());
                        QuestTaskMonsterDetailsListTemp.ForEach(o => o.QuerTaskIndex = QuestTask.Index);
                        QuestTaskMonsterDetailsListTemp.ForEach(o => o.MonsterName = o.Monster == null ? "" : o.Monster.MonsterName);
                        QuestTaskMonsterDetailsListTemp.ForEach(o => o.MapName = o.Map == null ? "" : o.Map.Description);
                        QuestTaskMonsterDetailsList.AddRange(QuestTaskMonsterDetailsListTemp);
                    }

                }
                DataTable dtMonsterDetails = NPOIHelper.ToDataTable<QuestTaskMonsterDetails>(QuestTaskMonsterDetailsList);
                dic.Add("MonsterDetails", dtMonsterDetails);
                #endregion

                NPOIHelper.Export(dic, strFullPath);
                System.Diagnostics.Process.Start(strFullPath);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // ExportImportHelp.ExportExcel(this.Text, QuestInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Excel文件(*.xlsx;*.xls)()|*.xlsx;*.xls";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string strPath = dialog.FileName;
                    List<DataTable> DatatList = NPOIHelper.Import(strPath);

                    DataTable dtQuestInfo = DatatList.FirstOrDefault(o => o.TableName == "QuestInfo");
                    if (dtQuestInfo == null)
                    {
                        MessageBox.Show("未找到QuestInfo表，请确认是不是修改过Excel");
                        return;
                    }

                    DataTable dtRequirements = DatatList.FirstOrDefault(o => o.TableName == "Requirements");
                    DataTable dtRewards = DatatList.FirstOrDefault(o => o.TableName == "Rewards");
                    DataTable dtTasks = DatatList.FirstOrDefault(o => o.TableName == "Tasks");
                    DataTable dtMonsterDetails = DatatList.FirstOrDefault(o => o.TableName == "MonsterDetails");

                    //QuestName不能有重复，否则可能会导出数据不正确 
                    List<QuestInfo> QuestInfoList = NPOIHelper.ToList<QuestInfo>(dtQuestInfo);

                    bool blnExists = QuestInfoList.GroupBy(l => l.QuestName).Where(g => g.Count() > 1).Count() > 0;
                    if (blnExists)
                    {
                        MessageBox.Show("QuestName不能有重复！");
                        return;
                    }
                    blnExists = QuestInfoList.GroupBy(l => l.Index).Where(g => g.Count() > 1).Count() > 0;
                    if (blnExists)
                    {
                        MessageBox.Show("Index不能有重复！");
                        return;
                    }

                    List<QuestRequirement> QuestRequirementList = NPOIHelper.ToList<QuestRequirement>(dtRequirements);
                    List<QuestReward> QuestRewardList = NPOIHelper.ToList<QuestReward>(dtRewards);
                    List<QuestTask> QuestTaskList = NPOIHelper.ToList<QuestTask>(dtTasks);
                    List<QuestTaskMonsterDetails> QuestTaskMonsterDetailsList = NPOIHelper.ToList<QuestTaskMonsterDetails>(dtMonsterDetails);

                    IList<NPCInfo> NPCInfoList = SMain.Session.GetCollection<NPCInfo>().Binding;
                    IList<ItemInfo> StartItemList = SMain.Session.GetCollection<ItemInfo>().Binding;

                    foreach (QuestInfo QuestInfo in QuestInfoList)
                    {
                        #region QuestInfo
                        QuestInfo QuestInfoTemp = SMain.Session.GetCollection<QuestInfo>().Binding.ToList().FirstOrDefault(o => o.QuestName == QuestInfo.QuestName);

                        if (QuestInfoTemp == null || QuestInfoTemp.Index == 0)
                        {//Add
                            QuestInfoTemp = SMain.Session.GetCollection<QuestInfo>().CreateNewObject();

                        }

                        QuestInfoTemp.IndexCopy = QuestInfo.IndexCopy;
                        QuestInfoTemp.QuestName = QuestInfo.QuestName;    
                        QuestInfoTemp.IsTemporary = QuestInfo.IsTemporary;
                        QuestInfoTemp.AcceptText = QuestInfo.AcceptText;
                        QuestInfoTemp.ProgressText = QuestInfo.ProgressText;
                        QuestInfoTemp.CompletedText = QuestInfo.CompletedText;
                        QuestInfoTemp.ArchiveText = QuestInfo.ArchiveText;
                        QuestInfoTemp.StartNPC = NPCInfoList.FirstOrDefault<NPCInfo>(o => o.NPCName == QuestInfo.StartNPCName);
                        QuestInfoTemp.FinishNPC = NPCInfoList.FirstOrDefault<NPCInfo>(o => o.NPCName == QuestInfo.FinishNPCName);
                        QuestInfoTemp.StartItem = StartItemList.FirstOrDefault<ItemInfo>(o => o.ItemName == QuestInfo.StartItemName);
                        QuestInfoTemp.QuestType = ExportImportHelp.GetEnumName<QuestType>(QuestInfo.QuestTypeName);  //任务类型

                        #endregion

                        for (int k = 0; k < QuestInfoTemp.Requirements.Count; k++)
                        {
                            QuestInfoTemp.Requirements.RemoveAt(k);
                            k--;
                        }
                        for (int k = 0; k < QuestInfoTemp.Rewards.Count; k++)
                        {
                            QuestInfoTemp.Rewards.RemoveAt(k);
                            k--;
                        }
                        for (int k = 0; k < QuestInfoTemp.Tasks.Count; k++)
                        {
                            QuestInfoTemp.Tasks.RemoveAt(k);
                            k--;
                        }

                        #region Requirements
                        List<QuestRequirement> QuestRequirementListTemp = QuestRequirementList.Where(o => o.QuertInfoIndex == QuestInfoTemp.IndexCopy).ToList();
                        foreach (QuestRequirement QuestRequirementTemp in QuestRequirementListTemp)
                        {
                            QuestRequirement QuestRequirementAdd = QuestInfoTemp.Requirements.AddNew();
                            QuestRequirementAdd.Requirement = (QuestRequirementType)Enum.Parse(typeof(QuestRequirementType), QuestRequirementTemp.RequirementName);
                            QuestRequirementAdd.IntParameter1 = QuestRequirementTemp.IntParameter1;
                            QuestRequirementAdd.QuestParameter = QuestInfoTemp;
                            QuestRequirementAdd.Class = ExportImportHelp.GetEnumName<RequiredClass>(QuestRequirementTemp.ClassName);
                            QuestRequirementAdd.Quest = QuestInfoTemp;
                        }
                        #endregion

                        #region Rewards
                        IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;

                        List<QuestReward> QuestRewardListTemp = QuestRewardList.Where(o => o.QuertInfoIndex == QuestInfoTemp.IndexCopy).ToList();
                        foreach (QuestReward QuestRewardTemp in QuestRewardListTemp)
                        {
                            QuestReward QuestRewardAdd = QuestInfoTemp.Rewards.AddNew();
                            QuestRewardAdd.Item = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == QuestRewardTemp.ItemName);
                            QuestRewardAdd.Amount = QuestRewardTemp.Amount;
                            QuestRewardAdd.Bound = QuestRewardTemp.Bound;
                            QuestRewardAdd.Duration = QuestRewardTemp.Duration;
                            QuestRewardAdd.Class = ExportImportHelp.GetEnumName<RequiredClass>(QuestRewardTemp.ClassName);
                            QuestRewardAdd.Choice = QuestRewardTemp.Choice;
                            QuestRewardAdd.Random = QuestRewardTemp.Random;
                            QuestRewardAdd.Quest = QuestInfoTemp;

                        }
                        #endregion

                        #region Tasks


                        List<QuestTask> QuestTaskListTemp = QuestTaskList.Where(o => o.QuertInfoIndex == QuestInfoTemp.IndexCopy).ToList();


                        foreach (QuestTask QuestTaskTemp in QuestTaskListTemp)
                        {
                            QuestTask QuestTaskAdd = QuestInfoTemp.Tasks.AddNew();
                            QuestTaskAdd.Task = (QuestTaskType)Enum.Parse(typeof(QuestTaskType), QuestTaskTemp.TaskName);
                            QuestTaskAdd.ItemParameter = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == QuestTaskTemp.ItemParameterName);
                            QuestTaskAdd.Amount = QuestTaskTemp.Amount;
                            QuestTaskAdd.MobDescription = QuestTaskTemp.MobDescription;

                            #region MonsterDetails
                            IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                            IList<MapInfo> MapInfoList = SMain.Session.GetCollection<MapInfo>().Binding;

                            List<QuestTaskMonsterDetails> QuestTaskMonsterDetailListTemp = QuestTaskMonsterDetailsList.Where(o => o.QuerTaskIndex == QuestTaskTemp.IndexCopy).ToList();
                            foreach (QuestTaskMonsterDetails QuestTaskMonsterDetailsTemp in QuestTaskMonsterDetailListTemp)
                            {
                                QuestTaskMonsterDetails QuestTaskMonsterDetailsAdd = QuestTaskAdd.MonsterDetails.AddNew();
                                QuestTaskMonsterDetailsAdd.Monster = MonsterInfoList.FirstOrDefault<MonsterInfo>(o => o.MonsterName == QuestTaskMonsterDetailsTemp.MonsterName);
                                QuestTaskMonsterDetailsAdd.Map = MapInfoList.FirstOrDefault<MapInfo>(o => o.Description == QuestTaskMonsterDetailsTemp.MapName);
                                QuestTaskMonsterDetailsAdd.Chance = QuestTaskMonsterDetailsTemp.Chance;
                                QuestTaskMonsterDetailsAdd.Amount = QuestTaskMonsterDetailsTemp.Amount;
                                QuestTaskMonsterDetailsAdd.DropSet = QuestTaskMonsterDetailsTemp.DropSet;
                                QuestTaskMonsterDetailsAdd.Task = QuestTaskAdd;
                            }

                            #endregion

                        }
                        #endregion

                    }

                    MessageBox.Show("导入成功","提示");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }*/
    }
}