using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class AchievementInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public AchievementInfoView()
        {
            InitializeComponent();

            AchievementInfoGridControl.DataSource = SMain.Session.GetCollection<AchievementInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MagicLookUpEdit.DataSource = SMain.Session.GetCollection<MagicInfo>().Binding;
            QuestLookUpEdit.DataSource = SMain.Session.GetCollection<QuestInfo>().Binding;
            AchievementLookUpEdit.DataSource = SMain.Session.GetCollection<AchievementInfo>().Binding;
            NPCLookUpEdit.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;

            ClassImageComboBox.Items.AddEnum<RequiredClass>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(AchievementInfoGridView);
            SMain.SetUpView(AchievementRequirementsGridView);
            SMain.SetUpView(AchievementRewardsGridView);

            AchievementRequirementsGridView.CellValueChanged += AchievementRequirementsGridView_CellValueChanged;
        }

        private void AchievementRequirementsGridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            AchievementRequirementsGridView.CellValueChanged -= AchievementRequirementsGridView_CellValueChanged;

            GridView view = sender as GridView;
            if (view == null) return;

            if (e.Column.Caption == "成就要求类型")
            {
                AchievementRequirementType thisValue = (AchievementRequirementType)e.Value;

                if (thisValue != AchievementRequirementType.InMap &&
                    thisValue != AchievementRequirementType.WearingItem &&
                    thisValue != AchievementRequirementType.CarryingItem)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["Reverse"], false);
                    view.Columns["Reverse"].Visible = false;
                }
                else if (thisValue == AchievementRequirementType.RankAll ||
                         thisValue == AchievementRequirementType.RankWarrior ||
                         thisValue == AchievementRequirementType.RankWizard ||
                         thisValue == AchievementRequirementType.RankTaoist ||
                         thisValue == AchievementRequirementType.RankAssassin)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["Reverse"], true);
                    view.Columns["Reverse"].Visible = false;
                }
                else
                {
                    view.Columns["Reverse"].Visible = true;
                }
            }

            if (e.Column.Caption == "反转要求")
            {
                AchievementRequirementType thatValue = (AchievementRequirementType)view.GetRowCellValue(e.RowHandle, view.Columns["RequirementType"]);

                if (thatValue != AchievementRequirementType.InMap &&
                    thatValue != AchievementRequirementType.WearingItem &&
                    thatValue != AchievementRequirementType.CarryingItem &&
                    thatValue == AchievementRequirementType.RankAll &&
                    thatValue == AchievementRequirementType.RankWarrior &&
                    thatValue == AchievementRequirementType.RankWizard &&
                    thatValue == AchievementRequirementType.RankTaoist &&
                    thatValue == AchievementRequirementType.RankAssassin)
                {
                    view.SetRowCellValue(e.RowHandle, view.Columns["Reverse"], false);
                }
            }
            AchievementRequirementsGridView.CellValueChanged += AchievementRequirementsGridView_CellValueChanged;
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)  //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<AchievementInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<AchievementInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}
