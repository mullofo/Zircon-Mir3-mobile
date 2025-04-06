using Server.Envir;

namespace Server.Views
{
    public partial class UserAchievementView : DevExpress.XtraEditors.XtraForm
    {
        public UserAchievementView()
        {
            InitializeComponent();

            UserAchievementGridControl.DataSource = SEnvir.UserAchievementList?.Binding;

            AccountLookUpEdit.DataSource = SEnvir.AccountInfoList?.Binding;

            UserAchievementGridView.OptionsSelection.MultiSelect = true;
            UserAchievementGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            SMain.SetUpView(RequirementView);
        }

    }
}