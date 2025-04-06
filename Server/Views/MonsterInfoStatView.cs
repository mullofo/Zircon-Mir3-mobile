using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MonsterInfoStatView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonsterInfoStatView()       //怪物属性信息视图
        {
            InitializeComponent();

            MonsterInfoStatGridControl.DataSource = SMain.Session.GetCollection<MonsterInfoStat>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            StatImageComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonsterInfoStatGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MonsterInfoStat>.ExportExcel();

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
                Helpers.HelperExcel<MonsterInfoStat>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MonsterInfoStatGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MonsterInfoStatGridView, ref dt);
                IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MonsterInfoStat ItemInfo = MonsterInfoStatGridView.GetRow(i) as MonsterInfoStat;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Monster = MonsterInfoList.FirstOrDefault<MonsterInfo>(o => o.MonsterName == Convert.ToString(DataRow["Monster"]));
                        ItemInfo.Stat = ExportImportHelp.GetEnumName<Stat>(Convert.ToString(DataRow["Stat"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }*/
    }
}