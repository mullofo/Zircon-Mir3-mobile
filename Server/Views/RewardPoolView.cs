using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;
using System;

namespace Server.Views
{
    public partial class RewardPoolView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public RewardPoolView()
        {
            InitializeComponent();
            RewardPoolViewGridControl.DataSource = SEnvir.RewardPoolInfoList?.Binding;

            RewardPoolViewGridView.OptionsSelection.MultiSelect = true;
            RewardPoolViewGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(RewardPoolViewGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SEnvir.Session.Save(true, MirDB.SessionMode.Server);
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