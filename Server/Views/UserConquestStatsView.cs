using DevExpress.XtraGrid.Views.Grid;
using Server.Envir;

namespace Server.Views
{
    public partial class UserConquestStatsView : DevExpress.XtraEditors.XtraForm
    {
        public UserConquestStatsView()        //角色攻城统计视图
        {
            InitializeComponent();

            UserDropGridControl.DataSource = SEnvir.UserConquestStatsList?.Binding;


            UserDropGridView.OptionsSelection.MultiSelect = true;
            UserDropGridView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
    }
}