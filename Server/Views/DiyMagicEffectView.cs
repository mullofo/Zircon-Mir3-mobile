using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class DiyMagicEffectView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public DiyMagicEffectView()
        {
            InitializeComponent();

            DiyMagicEffectGridControl.DataSource = SMain.Session.GetCollection<DiyMagicEffect>().Binding;
            MagicTypeComboBox.Items.AddEnum<DiyMagicType>();
            magicDirComboBox.Items.AddEnum<MagicDir>();
            LibraryFileComboBox.Items.AddEnum<LibraryFile>();
            MagicSoundComboBox.Items.AddEnum<SoundIndex>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(DiyMagicEffectGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<DiyMagicEffect>.ExportExcel();

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
                Helpers.HelperExcel<DiyMagicEffect>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView view = DiyMagicEffectGridControl.FocusedView as GridView;
            if (view == null) return;
            DiyMagicEffect SelectInfo = view.GetFocusedRow() as DiyMagicEffect;

            if (DiyMonShowViewer.CurrentDiyMonViewer == null)
            {
                DiyMonShowViewer.CurrentDiyMonViewer = new DiyMonShowViewer();
                DiyMonShowViewer.CurrentDiyMonViewer.Show();
            }
            DiyMonShowViewer.CurrentDiyMonViewer.BringToFront();
            DiyMonShowViewer.CurrentDiyMonViewer.Text = string.Format("自定义魔法效果实时显示--序号[{0}]", SelectInfo.MagicID);
            DiyMonShowViewer.CurrentDiyMonViewer.monsterAIInfo = null;
            DiyMonShowViewer.CurrentDiyMonViewer.DiymagicEffect = SelectInfo;
        }
    }
}