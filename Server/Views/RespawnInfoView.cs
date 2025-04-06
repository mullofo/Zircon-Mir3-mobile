using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class RespawnInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public RespawnInfoView()       //怪物刷新视图
        {
            InitializeComponent();

            RespawnInfoGridControl.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(RespawnInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<RespawnInfo>.ExportExcel();

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
                Helpers.HelperExcel<RespawnInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, RespawnInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(RespawnInfoGridView, ref dt);
                IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                IList<MapRegion> MapRegionList = SMain.Session.GetCollection<MapRegion>().Binding;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RespawnInfo ItemInfo = RespawnInfoGridView.GetRow(i) as RespawnInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Monster = MonsterInfoList.FirstOrDefault<MonsterInfo>(o => o.MonsterName == Convert.ToString(DataRow["Monster"]));
                        ItemInfo.Region = MapRegionList.FirstOrDefault<MapRegion>(o => o.Map.Description + " - " + o.Description == Convert.ToString(DataRow["Region"]));
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