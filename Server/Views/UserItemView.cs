using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class UserItemView : DevExpress.XtraEditors.XtraForm
    {
        public UserItemView()        //角色道具视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.UserItemList?.Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}