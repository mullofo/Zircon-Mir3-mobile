using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class CraftItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CraftItemInfoView()
        {
            InitializeComponent();

            CraftItemInfoGridControl.DataSource = SMain.Session.GetCollection<CraftItemInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CraftItemInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)  //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, CraftItemInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(CraftItemInfoGridView, ref dt);
                IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CraftItemInfo ItemInfo = CraftItemInfoGridView.GetRow(i) as CraftItemInfo;
                        DataRow DataRow = dt.Rows[i];

                        ItemInfo.Item = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item"]));
                        ItemInfo.Item1 = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item1"]));
                        ItemInfo.Item2 = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item2"]));
                        ItemInfo.Item3 = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item3"]));
                        ItemInfo.Item4 = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item4"]));
                        ItemInfo.Item5 = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item5"]));
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
                Helpers.HelperExcel<CraftItemInfo>.ExportExcel();

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
                Helpers.HelperExcel<CraftItemInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}
