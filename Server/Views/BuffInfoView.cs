using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class BuffInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public BuffInfoView()
        {
            InitializeComponent();

            BuffInfoGridControl.DataSource = SMain.Session.GetCollection<CustomBuffInfo>().Binding;

            StatImageComboBox.Items.AddEnum<Stat>();
            BuffTypeImageComboBox.Items.AddEnum<BuffType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(BuffInfoGridView);
            SMain.SetUpView(BuffStatsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<CustomBuffInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<CustomBuffInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}