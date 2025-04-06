using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MagicInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MagicInfoView()          //魔法技能视图
        {
            InitializeComponent();

            MagicInfoGridControl.DataSource = SMain.Session.GetCollection<MagicInfo>().Binding;

            MagicImageComboBox.Items.AddEnum<MagicType>();
            SchoolImageComboBox.Items.AddEnum<MagicSchool>();
            ClassImageComboBox.Items.AddEnum<MirClass>();
            ActionImageComboBox.Items.AddEnum<MagicAction>();  //技能说明主动或者被动
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MagicInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MagicInfo>.ExportExcel();

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
                Helpers.HelperExcel<MagicInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MagicInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MagicInfoGridView, ref dt);
             
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MagicInfo ItemInfo = MagicInfoGridView.GetRow(i) as MagicInfo;
                        DataRow DataRow = dt.Rows[i];
                      
                        ItemInfo.Magic = (MagicType)Enum.Parse(typeof(MagicType), Convert.ToString(DataRow["Magic"]));
                        ItemInfo.Class = ExportImportHelp.GetEnumName<MirClass>(Convert.ToString(DataRow["Class"]));
                        ItemInfo.School = ExportImportHelp.GetEnumName<MagicSchool>(Convert.ToString(DataRow["School"]));
                        ItemInfo.Action = ExportImportHelp.GetEnumName<MagicAction>(Convert.ToString(DataRow["Action"]));
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