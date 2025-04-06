using DevExpress.XtraGrid.Views.Grid;
using Library.SystemModels;
using Server.DBModels;

namespace Server.Views
{
    public partial class GameStoreSaleView : DevExpress.XtraEditors.XtraForm
    {
        public GameStoreSaleView()        //商城视图
        {
            InitializeComponent();

            GameStoreSaleGridControl.DataSource = SMain.Session.GetCollection<GameStoreSale>().Binding;
            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            GameStoreSaleGridView.OptionsSelection.MultiSelect = true;
            GameStoreSaleGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}