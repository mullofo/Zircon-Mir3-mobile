using Server.DBModels;

namespace Server.Views
{
    public partial class UserAchievementView : DevExpress.XtraEditors.XtraForm
    {
        public UserAchievementView()
        {
            InitializeComponent();

            UserAchievementGridControl.DataSource = SMain.Session.GetCollection<UserAchievement>().Binding;

            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;

            UserAchievementGridView.OptionsSelection.MultiSelect = true;
            UserAchievementGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            SMain.SetUpView(RequirementView);
        }

    }
}