using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class CompanionInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CompanionInfoView()   //宠物信息视图
        {
            InitializeComponent();

            CompanionInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionInfo>().Binding;
            CompanionLevelInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionLevelInfo>().Binding;
            CompanionSkillInfoGridControl.DataSource = SMain.Session.GetCollection<CompanionSkillInfo>().Binding;

            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            MonsterInfoLevelLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            PetSkillMonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CompanionInfoGridView);
            SMain.SetUpView(CompanionLevelInfoGridView);
            SMain.SetUpView(CompanionSkillInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel.ExportExcel(typeof(CompanionInfo), typeof(CompanionLevelInfo), typeof(CompanionSkillInfo));
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
                Helpers.HelperExcel<CompanionInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<CompanionLevelInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<CompanionSkillInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}