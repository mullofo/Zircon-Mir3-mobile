using DevExpress.XtraBars;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class EventInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public EventInfoView()         //事件信息视图  BOSS开门
        {
            InitializeComponent();


            EventInfoGridControl.DataSource = SMain.Session.GetCollection<MonsterEventInfo>().Binding;

            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            RespawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(EventInfoGridView);
            SMain.SetUpView(TargetsGridView);
            SMain.SetUpView(ActionsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MonsterEventInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<MonsterEventInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}