using DevExpress.XtraEditors;
using Server.DBModels;

namespace Server.Views
{
    public partial class CharacterView : XtraForm
    {
        public CharacterView()        //角色视图
        {
            InitializeComponent();

            CharacterGridControl.DataSource = SMain.Session.GetCollection<CharacterInfo>().Binding;
            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;

            CharacterGridView.OptionsSelection.MultiSelect = true;
            CharacterGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
        }
    }
}