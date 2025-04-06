using DevExpress.XtraBars;

namespace Server.Views
{
    public partial class PyMetricsView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public static BindingList<string> Logs = new BindingList<string>();

        public PyMetricsView()       //系统日志视图
        {
            InitializeComponent();

        }

        private void ClearLogsButton_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}