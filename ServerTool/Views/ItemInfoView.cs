using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class ItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemInfoView()         //道具信息视图
        {
            InitializeComponent();

            ItemInfoGridControl.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            SetLookUpEdit.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            ItemTypeImageComboBox.Items.AddEnum<ItemType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredGenderImageComboBox.Items.AddEnum<RequiredGender>();
            StatImageComboBox.Items.AddEnum<Stat>();
            RequiredTypeImageComboBox.Items.AddEnum<RequiredType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoGridView);
            SMain.SetUpView(ItemStatsGridView);
            SMain.SetUpView(DropsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)    //保存
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<ItemInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<ItemInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)    //导出
        {
            ItemInfoGridView.OptionsBehavior.AutoPopulateColumns = false;

            ExportImportHelp.ExportExcel(this.Text, ItemInfoGridView);
        }

        private void barButtonItem2_ItemClick_1(object sender, ItemClickEventArgs e)   //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(ItemInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ItemInfo ItemInfo = ItemInfoGridView.GetRow(i) as ItemInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.ItemType = ExportImportHelp.GetEnumName<ItemType>(Convert.ToString(DataRow["ItemType"]));
                        ItemInfo.RequiredClass = ExportImportHelp.GetEnumName<RequiredClass>(Convert.ToString(DataRow["RequiredClass"]));
                        ItemInfo.RequiredGender = ExportImportHelp.GetEnumName<RequiredGender>(Convert.ToString(DataRow["RequiredGender"]));
                        ItemInfo.RequiredType = ExportImportHelp.GetEnumName<RequiredType>(Convert.ToString(DataRow["RequiredType"]));
                        ItemInfo.Effect = (ItemEffect)Enum.Parse(typeof(ItemEffect), Convert.ToString(DataRow["Effect"]));
                        ItemInfo.Rarity = ExportImportHelp.GetEnumName<Rarity>(Convert.ToString(DataRow["Rarity"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");

            }
            return;
        }*/
    }

}