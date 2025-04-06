using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MonsterInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MonsterInfoView()        //怪物信息视图
        {
            InitializeComponent();

            MonsterInfoGridControl.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;

            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            MonsterImageComboBox.Items.AddEnum<MonsterImage>();
            StatComboBox.Items.AddEnum<Stat>();
            StatImageComboBox.Items.AddEnum<MirAnimation>();
            EffectImageComboBox.Items.AddEnum<LibraryFile>();
            MagicDirImageComboBox.Items.AddEnum<MagicDir>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MonsterInfoGridView);
            SMain.SetUpView(MonsterInfoStatsGridView);
            SMain.SetUpView(DropsGridView);
            SMain.SetUpView(RespawnsGridView);
            //SMain.SetUpView(MonsterCustomInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MonsterInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<MonsterInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MonsterInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MonsterInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MonsterInfo ItemInfo = MonsterInfoGridView.GetRow(i) as MonsterInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Image = ExportImportHelp.GetEnumName<MonsterImage>(Convert.ToString(DataRow["Image"]));
                        //ItemInfo.Image = (MonsterImage)Enum.Parse(typeof(MonsterImage), Convert.ToString(DataRow["Image"]));
                        ItemInfo.File = (LibraryFile)Enum.Parse(typeof(LibraryFile), Convert.ToString(DataRow["File"]));
                        ItemInfo.AttackSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["AttackSound"]));
                        ItemInfo.StruckSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["StruckSound"]));
                        ItemInfo.DieSound = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["DieSound"]));
                        ItemInfo.Flag = (MonsterFlag)Enum.Parse(typeof(MonsterFlag), Convert.ToString(DataRow["Flag"]));
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