using DevExpress.XtraGrid.Views.Grid;
using Server.DBModels;

namespace Server.Views
{
    public partial class UserMailView : DevExpress.XtraEditors.XtraForm
    {
        public UserMailView()         //角色邮件视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SMain.Session.GetCollection<MailInfo>().Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}