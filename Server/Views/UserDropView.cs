using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class UserDropView : DevExpress.XtraEditors.XtraForm
    {
        public UserDropView()       //角色爆率视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.UserDropList?.Binding;

            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;
            ItemLookUpEdit.DataSource = SEnvir.ItemInfoList?.Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}