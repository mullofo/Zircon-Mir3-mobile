using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class MovementInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MovementInfoView()       //地图链接视图
        {
            InitializeComponent();

            MovementGridControl.DataSource = SMain.Session.GetCollection<MovementInfo>().Binding;

            MapLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            SpawnLookUpEdit.DataSource = SMain.Session.GetCollection<RespawnInfo>().Binding;

            MapIconImageComboBox.Items.AddEnum<MapIcon>();
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.Server);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(MovementGridView);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<MovementInfo>.ExportExcel();

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
                Helpers.HelperExcel<MovementInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ExportImportHelp.ExportExcel(this.Text, MovementGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(MovementGridView, ref dt);
                IList<MapRegion> MonsterInfoList = SMain.Session.GetCollection<MapRegion>().Binding;
                IList<ItemInfo> ItemInfoList = SMain.Session.GetCollection<ItemInfo>().Binding;
                IList<RespawnInfo> RespawnInfoList = SMain.Session.GetCollection<RespawnInfo>().Binding;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MovementInfo ItemInfo = MovementGridView.GetRow(i) as MovementInfo;
                        DataRow DataRow = dt.Rows[i];
                        ItemInfo.SourceRegion = MonsterInfoList.FirstOrDefault<MapRegion>(o => o.Map.Description+" - "+o.Description == Convert.ToString(DataRow["SourceRegion"]));
                        ItemInfo.DestinationRegion = MonsterInfoList.FirstOrDefault<MapRegion>(o => o.Map.Description + " - " + o.Description == Convert.ToString(DataRow["DestinationRegion"]));
                                                
                        ItemInfo.Icon = ExportImportHelp.GetEnumName<MapIcon>(Convert.ToString(DataRow["Icon"]));
                        ItemInfo.NeedItem = ItemInfoList.FirstOrDefault<ItemInfo>(o => o.ItemName == Convert.ToString(DataRow["NeedItem"]));
                        
                        ItemInfo.NeedSpawn = RespawnInfoList.FirstOrDefault<RespawnInfo>(o => o.MonsterName == Convert.ToString(DataRow["NeedSpawn"]));
                        ItemInfo.Effect = (MovementEffect)Enum.Parse(typeof(MovementEffect), Convert.ToString(DataRow["Effect"]));
                        ItemInfo.RequiredClass = (RequiredClass)Enum.Parse(typeof(RequiredClass), Convert.ToString(DataRow["RequiredClass"]));
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