using DevExpress.XtraGrid.Views.Grid;
using Server.DBModels;

namespace Server.Views
{
    public partial class GameGoldPaymentView : DevExpress.XtraEditors.XtraForm
    {
        public GameGoldPaymentView()       //元宝充值视图
        {
            InitializeComponent();

            GameGoldPaymentGridControl.DataSource = SMain.Session.GetCollection<GameGoldPayment>().Binding;
            AccountLookUpEdit.DataSource = SMain.Session.GetCollection<AccountInfo>().Binding;

            GameGoldPaymentGridView.OptionsSelection.MultiSelect = true;
            GameGoldPaymentGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}