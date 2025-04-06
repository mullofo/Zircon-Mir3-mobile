using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MonAnimationFrameView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonAnimationFrameView()
        {
            InitializeComponent();

            MonAnimationFrameGridControl.DataSource = SMain.Session.GetCollection<MonAnimationFrame>().Binding;
            MonLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            MirActImageComboBox.Items.AddEnum<MirAnimation>();
            ActSoundComboBox.Items.AddEnum<SoundIndex>();
            effectfileComboBox.Items.AddEnum<LibraryFile>();
            EffectdirComboBox.Items.AddEnum<MagicDir>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonAnimationFrameGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MonAnimationFrame>.ExportExcel();

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
                Helpers.HelperExcel<MonAnimationFrame>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        private void barButtonItem3_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            foreach (var MonAnimation in SMain.Session.GetCollection<MonAnimationFrame>().Binding)
            {
                MonsterInfo mon = SMain.Session.GetCollection<MonsterInfo>().Binding.FirstOrDefault(x => x.Index == MonAnimation.MonsterIdx);
                if (mon != null && mon.Image == MonsterImage.DiyMonsMon)
                {
                    MonAnimation.Monster = mon;
                }
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)   //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MonsterCustomInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MonsterCustomInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    IList<MonsterInfo> MonsterInfoList = SMain.Session.GetCollection<MonsterInfo>().Binding;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MonsterCustomInfo ItemInfo = MonsterCustomInfoGridView.GetRow(i) as MonsterCustomInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Monster = MonsterInfoList.FirstOrDefault<MonsterInfo>(o => o.MonsterName == Convert.ToString(DataRow["Monster"]));
                        ItemInfo.Animation = ExportImportHelp.GetEnumName<MirAnimation>(Convert.ToString(DataRow["Animation"]));
                        ItemInfo.Action = ExportImportHelp.GetEnumName<MirAction>(Convert.ToString(DataRow["Action"]));
                        ItemInfo.Effect = (LibraryFile)Enum.Parse(typeof(LibraryFile), Convert.ToString(DataRow["Effect"]));
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");

            }
        }*/
    }
}