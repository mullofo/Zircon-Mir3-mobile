using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class SetInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SetInfoView()         //套装属性视图
        {
            InitializeComponent();

            SetInfoGridControl.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;
            ItemLookUpEdit.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;

            ClassImageComboBox.Items.AddEnum<RequiredClass>();
            SetRequirementComboBox.Items.AddEnum<ItemSetRequirementType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(SetInfoGridView);
            SMain.SetUpView(SetGroupsGridView);
            SMain.SetUpView(SetInfoStatGridView);
            SMain.SetUpView(SetGroupItemGridView);


            var setInfos = SMain.Session.GetCollection<SetInfo>().Binding;
            int count = 0;
            for (int i = setInfos.Count - 1; i >= 0; i--)
            {
                if (setInfos[i].SetGroups == null || setInfos[i].SetGroups.Count < 1)
                {
                    count++;
                    setInfos[i].Delete();
                }
            }
            if (count > 0)
            {
                SEnvir.Log($"删除了{count}组无效的SetInfo");
                count = 0;
            }

            var setGroups = SMain.Session.GetCollection<SetGroup>().Binding;
            for (int i = setGroups.Count - 1; i >= 0; i--)
            {
                if (setGroups[i].SetGroupItems == null || setGroups[i].SetGroupItems.Count < 1 ||
                    setGroups[i].GroupStats == null || setGroups[i].GroupStats.Count < 1 ||
                    setGroups[i].Set == null)
                {
                    count++;
                    setGroups[i].Delete();
                }
            }
            if (count > 0)
            {
                SEnvir.Log($"删除了{count}组无效的SetGroup");
                count = 0;
            }

            var setGroupItems = SMain.Session.GetCollection<SetGroupItem>().Binding;
            for (int i = setGroupItems.Count - 1; i >= 0; i--)
            {
                if (setGroupItems[i].SetGroupInfo == null || setGroupItems[i].SetGroupItemInfo == null)
                {
                    count++;
                    setGroupItems[i].Delete();
                }
            }
            if (count > 0)
            {
                SEnvir.Log($"删除了{count}组无效的SetGroupItem");
                count = 0;
            }

            var setInfoStats = SMain.Session.GetCollection<SetInfoStat>().Binding;
            for (int i = setInfoStats.Count - 1; i >= 0; i--)
            {
                if (setInfoStats[i].Group == null)
                {
                    count++;
                    setInfoStats[i].Delete();
                }
            }
            if (count > 0)
            {
                SEnvir.Log($"删除了{count}组无效的SetInfoStat");
                count = 0;
            }
        }

        private void SaveDatabaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)  //导出
        {
            try
            {
                Helpers.HelperExcel<SetInfo>.ExportExcel(true);

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
                Helpers.HelperExcel<SetInfo>.ImportExcel(true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}