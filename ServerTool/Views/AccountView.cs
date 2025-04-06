using DevExpress.XtraGrid.Views.Grid;
using Server.DBModels;

namespace Server.Views
{
    public partial class AccountView : DevExpress.XtraEditors.XtraForm
    {
        public AccountView()    //账户视图
        {
            InitializeComponent();   //初始化组件

            AccountGridControl.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;
            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;

            AccountGridView.OptionsSelection.MultiSelect = true;
            AccountGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}