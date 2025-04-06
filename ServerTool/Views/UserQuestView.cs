using Server.DBModels;

namespace Server.Views
{
    public partial class UserQuestView : DevExpress.XtraEditors.XtraForm
    {
        public UserQuestView()
        {
            InitializeComponent();

            UserQuestGridControl.DataSource = SMain.Session.GetCollection<UserQuest>().Binding;

            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;

            UserQuestGridView.OptionsSelection.MultiSelect = true;
            UserQuestGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            SMain.SetUpView(UserQuestTaskView);
        }

    }
}