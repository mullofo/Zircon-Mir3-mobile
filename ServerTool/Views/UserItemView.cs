using DevExpress.XtraGrid.Views.Grid;
using Server.DBModels;

namespace Server.Views
{
    public partial class UserItemView : DevExpress.XtraEditors.XtraForm
    {
        public UserItemView()        //角色道具视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SMain.Session.GetCollection<UserItem>().Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}