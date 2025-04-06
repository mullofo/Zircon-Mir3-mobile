using DevExpress.XtraBars;
using Server.Envir;
using Server.Utils.Logging;
using System;
using System.ComponentModel;

namespace Server.Views
{
    public partial class CurrencyLogView : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public static BindingList<CurrencyLogEntry> CurrencyLogs = new BindingList<CurrencyLogEntry>();

        public CurrencyLogView()
        {
            InitializeComponent();

            CurrencyLogViewGridControl.DataSource = CurrencyLogs;

        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            while (!SEnvir.CurrencyLogs.IsEmpty)
            {
                if (!SEnvir.CurrencyLogs.TryDequeue(out var log)) continue;

                CurrencyLogs.Add(log);
            }

            //if (Logs.Count > 0)
            //    ClearLogsButton.Enabled = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CurrencyLogViewGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
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