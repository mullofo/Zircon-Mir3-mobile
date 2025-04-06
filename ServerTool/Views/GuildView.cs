using DevExpress.XtraBars;
using Server.DBModels;
using System;

namespace Server.Views
{
    public partial class GuildInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public GuildInfoView()
        {
            InitializeComponent();
            GuildGridControl.DataSource = SMain.Session.GetCollection<GuildInfo>().Binding;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(GuildGridView);
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