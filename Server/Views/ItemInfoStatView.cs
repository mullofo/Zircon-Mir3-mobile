using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class ItemInfoStatView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemInfoStatView()          //道具属性信息视图
        {
            InitializeComponent();

            ItemInfoStatGridControl.DataSource = SMain.Session.GetCollection<ItemInfoStat>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            StatImageComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoStatGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)  //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, ItemInfoStatGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(ItemInfoStatGridView, ref dt);
                IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ItemInfoStat ItemInfo = ItemInfoStatGridView.GetRow(i) as ItemInfoStat;
                        DataRow DataRow = dt.Rows[i];

                        ItemInfo.Item = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["Item"]));
                        ItemInfo.Stat = ExportImportHelp.GetEnumName<Stat>(Convert.ToString(DataRow["Stat"]));
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
                Helpers.HelperExcel<ItemInfoStat>.ExportExcel();

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
                Helpers.HelperExcel<ItemInfoStat>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}