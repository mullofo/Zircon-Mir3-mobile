using DevExpress.XtraGrid.Views.Grid;
using Library.SystemModels;
using Server.DBModels;

namespace Server.Views
{
    public partial class UserDropView : DevExpress.XtraEditors.XtraForm
    {
        public UserDropView()       //角色爆率视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SMain.Session.GetCollection<UserDrop>().Binding;

            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}