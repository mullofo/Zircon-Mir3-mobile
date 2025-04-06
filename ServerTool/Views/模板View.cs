using DevExpress.XtraBars;
using System;

namespace Server.Views
{
    public partial class 模板View : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public 模板View()
        {
            InitializeComponent();



        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(模板_GridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO 导出
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            //TODO 导入
        }
    }
}