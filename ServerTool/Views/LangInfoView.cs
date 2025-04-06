using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class LangInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public LangInfoView()
        {
            InitializeComponent();
            LangGridControl.DataSource = SMain.Session.GetCollection<LangInfo>().Binding;
            LangTypeComboBox.Items.AddEnum<Language>();
            TypeLookUpEdit.DataSource = LangInfo.GetLangs();
            LangInfoGridView.CustomRowCellEditForEditing += LangInfoGridView_CustomRowCellEditForEditing;

        }

        private void LangInfoGridView_CustomRowCellEditForEditing(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var list = LangGridControl.DataSource as IList<LangInfo>;
            var row = list[e.RowHandle];
            if (row == null) return;
            var type = LangInfo.GetType(row.Type);

            if (e.Column.FieldName == nameof(LangInfo.Type))
            {
                if (type?.IsEnum == true)
                {
                    row.Key = string.Empty;
                    row.Value = string.Empty;
                }
                return;
            }
            if (e.Column.FieldName != nameof(LangInfo.Key)) return;

            if (type?.IsEnum != true) return;

            var repositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            repositoryItem.Items.AddEnum(type, false);
            e.RepositoryItem = repositoryItem;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(LangInfoGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Helpers.HelperExcel<LangInfo>.ExportExcel();

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
                Helpers.HelperExcel<LangInfo>.ImportExcel();

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败" + ex.Message, "提示");
            }
        }
    }
}