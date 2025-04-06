using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class BaseStatView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public BaseStatView()    //基础属性状态视图
        {
            InitializeComponent();

            BaseStatGridControl.DataSource = SMain.Session.GetCollection<BaseStat>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(BaseStatGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<BaseStat>.ExportExcel(true);

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
                Helpers.HelperExcel<BaseStat>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}