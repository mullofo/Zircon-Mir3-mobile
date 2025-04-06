using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MapInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MapInfoView()       //地图信息视图
        {
            InitializeComponent();

            MapInfoGridControl.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            MapInfoLookUpEdit.DataSource = SMain.Session.GetCollection<MapInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            LightComboBox.Items.AddEnum<LightSetting>();
            WeatherComboBox.Items.AddEnum<WeatherSetting>();
            DirectionImageComboBox.Items.AddEnum<MirDirection>();
            MapIconImageComboBox.Items.AddEnum<MapIcon>();
            StartClassImageComboBox.Items.AddEnum<RequiredClass>();
            AttackModeComboBox.Items.AddEnum<AttackMode>();
        }

        private void MapInfoView_Load(object sender, EventArgs e)
        {
            SMain.SetUpView(MapInfoGridView);
            SMain.SetUpView(GuardsGridView);
            SMain.SetUpView(RegionGridView);
            SMain.SetUpView(MiningGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void EditButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (MapViewer.CurrentViewer == null)
            {
                MapViewer.CurrentViewer = new MapViewer();
                MapViewer.CurrentViewer.Show();
            }

            MapViewer.CurrentViewer.BringToFront();

            GridView view = MapInfoGridControl.FocusedView as GridView;

            if (view == null) return;

            MapViewer.CurrentViewer.Save();

            MapViewer.CurrentViewer.MapRegion = view.GetFocusedRow() as MapRegion;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MapInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<MapInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            ExportImportHelp.ExportExcel(this.Text, MapInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)  //导入
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MapInfoGridView, ref dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MapInfo ItemInfo = MapInfoGridView.GetRow(i) as MapInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.Light = ExportImportHelp.GetEnumName<LightSetting>(Convert.ToString(DataRow["Light"]));
                        ItemInfo.Weather = ExportImportHelp.GetEnumName<WeatherSetting>(Convert.ToString(DataRow["Weather"]));
                        ItemInfo.Fight = ExportImportHelp.GetEnumName<FightSetting>(Convert.ToString(DataRow["Fight"]));
                        ItemInfo.Music = (SoundIndex)Enum.Parse(typeof(SoundIndex), Convert.ToString(DataRow["Music"]));
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