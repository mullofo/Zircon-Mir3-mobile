using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MonDiyAiActionView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonDiyAiActionView()
        {
            InitializeComponent();

            MonDiyAiActionGridControl.DataSource = SMain.Session.GetCollection<MonDiyAiAction>().Binding;

            MonLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            ActTypeComboBox.Items.AddEnum<MonDiyActType>();
            ActAniComboBox.Items.AddEnum<MirAnimation>();
            PowerTypeComboBox.Items.AddEnum<MonDiyPowerType>();
            ElementTypeComboBox.Items.AddEnum<Element>();
            TargetComboBox.Items.AddEnum<TargetType>();
            CheckTypeComboBox.Items.AddEnum<MonCheckType>();
            OperatorComboBox.Items.AddEnum<Operators>();
            AtkEffectComboBox.Items.AddEnum<AttackEffect>();
            SystemMagicComboBox.Items.AddEnum<MagicType>();

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonDiyAiActionGridView);
            SMain.SetUpView(MonActChecksGridView);
            SMain.SetUpView(MonAttackEffectsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<MonDiyAiAction>.ExportExcel(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<MonDiyAiAction>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView view = MonDiyAiActionGridControl.FocusedView as GridView;
            if (view == null) return;
            MonDiyAiAction SelectInfo = view.GetFocusedRow() as MonDiyAiAction;

            if (SelectInfo.Monster.Image != MonsterImage.DiyMonsMon)
            {
                MessageBox.Show("该类型怪物不支持自定义！");
                return;
            }
            if (SelectInfo.SystemMagic != MagicType.None)
            {
                MessageBox.Show("该怪物使用系统魔法，不支持查看！");
                return;
            }

            if (DiyMonShowViewer.CurrentDiyMonViewer == null)
            {
                DiyMonShowViewer.CurrentDiyMonViewer = new DiyMonShowViewer();
                DiyMonShowViewer.CurrentDiyMonViewer.Show();
            }

            DiyMonShowViewer.CurrentDiyMonViewer.BringToFront();
            DiyMonShowViewer.CurrentDiyMonViewer.Text = string.Format("怪物自定义效果实时显示--[{0}]", SelectInfo.Monster.MonsterName);
            DiyMonShowViewer.CurrentDiyMonViewer.DiymagicEffect = null;

            DiyMonShowViewer.CurrentDiyMonViewer.monsterAIInfo = SelectInfo;
        }
    }
}