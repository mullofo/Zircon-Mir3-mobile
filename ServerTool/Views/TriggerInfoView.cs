using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class TriggerInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public TriggerInfoView()
        {
            InitializeComponent();

            TriggerInfoGridControl.DataSource = SMain.Session.GetCollection<TriggerInfo>().Binding;

            ItemInfoLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MagicInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MagicInfo>().Binding;
            ConditionTypeImageComboBox.Items.AddEnum<TriggerConditionType>();
            EfffectTypeImageComboBox.Items.AddEnum<TriggerEffectType>();
            StatImageComboBox.Items.AddEnum<Stat>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(TriggerInfoGridView);
            SMain.SetUpView(TriggerConditionView);
            SMain.SetUpView(TriggerEffectView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<TriggerInfo>.ExportExcel(true);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                Helpers.HelperExcel<TriggerInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}