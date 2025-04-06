using Server.Envir;

namespace Server.Views
{
    public partial class UserQuestView : DevExpress.XtraEditors.XtraForm
    {
        public UserQuestView()
        {
            InitializeComponent();

            UserQuestGridControl.DataSource = SEnvir.UserQuestList?.Binding;

            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;

            UserQuestGridView.OptionsSelection.MultiSelect = true;
            UserQuestGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            SMain.SetUpView(UserQuestTaskView);
        }

    }
}