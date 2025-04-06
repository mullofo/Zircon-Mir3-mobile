using DevExpress.XtraBars;
using Library.SystemModels;
using Server.Envir;
using System;
using System.IO;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class NPCInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public NPCInfoView()        //NPC信息视图
        {
            InitializeComponent();

            this.txtSelect.AutoComplete = true;

            LoadTxtSource();
            this.txtSelect.Validating += TxtSelect_Validating;
            this.RefreshTxt.ItemClick += RefreshTxt_ItemClick;

            NPCInfoGridControl.DataSource = SMain.Session.GetCollection<NPCInfo>().Binding;
            RegionLookUpEdit.DataSource = SMain.Session.GetCollection<MapRegion>().Binding;
            //PageLookUpEdit.DataSource = SMain.Session.GetCollection<NPCPage>().Binding;


        }

        private void RefreshTxt_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.txtSelect.Items.Clear();
            LoadTxtSource();
        }

        private void LoadTxtSource()
        {
            GetScripts(new DirectoryInfo(Config.EnvirPath + @"\Npcs"));
        }

        private void TxtSelect_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var box = sender as DevExpress.XtraEditors.ComboBoxEdit;
            if (box == null) return;
            var v = box.EditValue as string;
            if (!string.IsNullOrEmpty(v))
            {
                if (this.txtSelect.Items.IndexOf(v) == -1)
                {
                    box.EditValue = string.Empty;
                }
                else if (!File.Exists(Path.Combine(Config.EnvirPath, v)))
                {
                    box.EditValue = string.Empty;
                }
            }

        }

        private void GetScripts(DirectoryInfo dirInfo)
        {
            foreach (var item in dirInfo.GetFiles())
            {
                this.txtSelect.Items.Add((item.FullName.Replace(new DirectoryInfo(Config.EnvirPath).FullName + @"\", "")).Replace(@"\", @"/"));
            }
            foreach (System.IO.DirectoryInfo subdir in dirInfo.GetDirectories())
            {
                GetScripts(subdir);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(NPCInfoGridView);
        }
        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<NPCInfo>.ExportExcel();

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
                Helpers.HelperExcel<NPCInfo>.ImportExcel();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }

        /*private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ExportImportHelp.ExportExcel(this.Text, NPCInfoGridView);
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DataTable dt = null;
                ExportImportHelp.ImportExcel(NPCInfoGridView, ref dt);
                IList<NPCPage> NPCPageList = SMain.Session.GetCollection<NPCPage>().Binding;
                IList<MapRegion> MapRegionList = SMain.Session.GetCollection<MapRegion>().Binding;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NPCInfo ItemInfo = NPCInfoGridView.GetRow(i) as NPCInfo;
                        DataRow DataRow = dt.Rows[i];
                         
                        ItemInfo.Region = MapRegionList.FirstOrDefault<MapRegion>(o => o.Map.Description + " - " + o.Description == Convert.ToString(DataRow["Region"]));
                        if(dt.Columns.Contains("EntryPage"))
                        ItemInfo.EntryPage = NPCPageList.FirstOrDefault<NPCPage>(o => o.Description == Convert.ToString(DataRow["EntryPage"]));
                        if (dt.Columns.Contains("CurrentIcon"))
                         ItemInfo.CurrentIcon = (QuestIcon)Enum.Parse(typeof(QuestIcon), Convert.ToString(DataRow["CurrentIcon"]));
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