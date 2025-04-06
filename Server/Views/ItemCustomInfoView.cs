using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class ItemCustomInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemCustomInfoView()
        {
            InitializeComponent();

            ItemCustomInfoGridControl.DataSource = SMain.Session.GetCollection<ItemDisplayEffect>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemCustomInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)  //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, ItemCustomInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(ItemCustomInfoGridView, ref dt);
                IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ItemDisplayEffect ItemInfo = ItemCustomInfoGridView.GetRow(i) as ItemDisplayEffect;
                        DataRow DataRow = dt.Rows[i];

                        ItemInfo.Info = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Info"]));
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
                Helpers.HelperExcel<ItemDisplayEffect>.ExportExcel();

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
                Helpers.HelperExcel<ItemDisplayEffect>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}
