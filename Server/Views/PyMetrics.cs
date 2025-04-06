using DevExpress.XtraBars;
using Server.Envir;
using System;

namespace Server.Views
{
    public partial class PyMetricsView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public static BindingList<string> Logs = new BindingList<string>();

        public PyMetricsView()       //系统日志视图
        {
            InitializeComponent();
            gridControl1.DataSource = SEnvir.PyMetricsDict.Values;

        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            gridControl1.DataSource = SEnvir.PyMetricsDict.Values;
            this.gridControl1.RefreshDataSource();
        }

        private void ClearLogsButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SEnvir.PyMetricsDict?.Clear();
        }
    }
}