using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class DropInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DropInfoView()     //爆率信息视图
        {
            InitializeComponent();

            DropInfoGridControl.DataSource = SMain.Session.GetCollection<DropInfo>().Binding;

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(DropInfoGridView);
        }


        private void SavingButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, DropInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(DropInfoGridView, ref dt);
                IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;
              
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DropInfo ItemInfo = DropInfoGridView.GetRow(i) as DropInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Monster = MonsterInfoList.FirstOrDefault<MonsterInfo>(o => o.MonsterName == Convert.ToString(DataRow["Monster"]));
                        ItemInfo.Item = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item"]));

                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");

            }
        }*/

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<DropInfo>.ExportExcel();

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
                Helpers.HelperExcel<DropInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}