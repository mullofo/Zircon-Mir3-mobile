namespace Server.Views
{
    partial class NPCInfoView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NPCInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.RefreshTxt = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.NPCInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.NPCInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colNPCName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEntryPage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.PageLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RegionLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FileName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtSelect = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.barButtonItem1,
            this.barButtonItem2,
            this.RefreshTxt});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1087, 147);
            // 
            // SaveButton
            // 
            this.SaveButton.Caption = "保存";
            this.SaveButton.Id = 1;
            this.SaveButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.Image")));
            this.SaveButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("SaveButton.ImageOptions.LargeImage")));
            this.SaveButton.LargeWidth = 60;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.SaveButton_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "导出";
            this.barButtonItem1.Id = 2;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "导入";
            this.barButtonItem2.Id = 3;
            this.barButtonItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.Image")));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // RefreshTxt
            // 
            this.RefreshTxt.Caption = "TXT";
            this.RefreshTxt.Id = 4;
            this.RefreshTxt.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("RefreshTxt.ImageOptions.Image")));
            this.RefreshTxt.Name = "RefreshTxt";
            this.RefreshTxt.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "主页";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "存档";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup2.ItemLinks.Add(this.RefreshTxt);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "数据";
            // 
            // NPCInfoGridControl
            // 
            this.NPCInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NPCInfoGridControl.Location = new System.Drawing.Point(0, 147);
            this.NPCInfoGridControl.MainView = this.NPCInfoGridView;
            this.NPCInfoGridControl.MenuManager = this.ribbon;
            this.NPCInfoGridControl.Name = "NPCInfoGridControl";
            this.NPCInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.RegionLookUpEdit,
            this.PageLookUpEdit,
            this.txtSelect});
            this.NPCInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.NPCInfoGridControl.Size = new System.Drawing.Size(1087, 468);
            this.NPCInfoGridControl.TabIndex = 2;
            this.NPCInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.NPCInfoGridView});
            // 
            // NPCInfoGridView
            // 
            this.NPCInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colNPCName,
            this.colImage,
            this.colEntryPage,
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4,
            this.FileName,
            this.gridColumn2});
            this.NPCInfoGridView.DetailHeight = 377;
            this.NPCInfoGridView.GridControl = this.NPCInfoGridControl;
            this.NPCInfoGridView.Name = "NPCInfoGridView";
            this.NPCInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.NPCInfoGridView.OptionsView.ColumnAutoWidth = false;
            this.NPCInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.NPCInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.NPCInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colNPCName
            // 
            this.colNPCName.Caption = "NPC名字";
            this.colNPCName.FieldName = "NPCName";
            this.colNPCName.MinWidth = 150;
            this.colNPCName.Name = "colNPCName";
            this.colNPCName.ToolTip = "NPC名字";
            this.colNPCName.Visible = true;
            this.colNPCName.VisibleIndex = 2;
            this.colNPCName.Width = 150;
            // 
            // colImage
            // 
            this.colImage.Caption = "素材图片序号";
            this.colImage.FieldName = "Image";
            this.colImage.MinWidth = 80;
            this.colImage.Name = "colImage";
            this.colImage.ToolTip = "NPC显示的图库，对应客户端Data里的NPC.ZL文件";
            this.colImage.Visible = true;
            this.colImage.VisibleIndex = 3;
            this.colImage.Width = 80;
            // 
            // colEntryPage
            // 
            this.colEntryPage.ColumnEdit = this.PageLookUpEdit;
            this.colEntryPage.FieldName = "EntryPage";
            this.colEntryPage.MinWidth = 23;
            this.colEntryPage.Name = "colEntryPage";
            this.colEntryPage.Width = 87;
            // 
            // PageLookUpEdit
            // 
            this.PageLookUpEdit.AutoHeight = false;
            this.PageLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.PageLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.PageLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Description", "Description"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DialogType", "DialogType"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Say", "Say")});
            this.PageLookUpEdit.DisplayMember = "Description";
            this.PageLookUpEdit.Name = "PageLookUpEdit";
            this.PageLookUpEdit.NullText = "[Page is null]";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "NPC设定区域";
            this.gridColumn1.ColumnEdit = this.RegionLookUpEdit;
            this.gridColumn1.FieldName = "Region";
            this.gridColumn1.MinWidth = 250;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn1.ToolTip = "NPC设置区域，对应地图区域设置里写好的位置范围";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 250;
            // 
            // RegionLookUpEdit
            // 
            this.RegionLookUpEdit.AutoHeight = false;
            this.RegionLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.RegionLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.RegionLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ServerDescription", "Server Description"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Size", "Size")});
            this.RegionLookUpEdit.DisplayMember = "ServerDescription";
            this.RegionLookUpEdit.Name = "RegionLookUpEdit";
            this.RegionLookUpEdit.NullText = "[Region is null]";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "序号";
            this.gridColumn3.FieldName = "Index";
            this.gridColumn3.MinWidth = 50;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ToolTip = "NPC序号";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 50;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "勾选隐藏NPC";
            this.gridColumn4.FieldName = "Display";
            this.gridColumn4.MinWidth = 80;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 80;
            // 
            // FileName
            // 
            this.FileName.Caption = "TXT脚本路径";
            this.FileName.ColumnEdit = this.txtSelect;
            this.FileName.FieldName = "NPCFile";
            this.FileName.MinWidth = 250;
            this.FileName.Name = "FileName";
            this.FileName.Visible = true;
            this.FileName.VisibleIndex = 6;
            this.FileName.Width = 250;
            // 
            // txtSelect
            // 
            this.txtSelect.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtSelect.AutoHeight = false;
            this.txtSelect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtSelect.Name = "txtSelect";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "攻城区域不隐藏勾选NPC";
            this.gridColumn2.FieldName = "WarDisplay";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 5;
            // 
            // NPCInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 615);
            this.Controls.Add(this.NPCInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "NPCInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "NPC设置";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NPCInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PageLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegionLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl NPCInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView NPCInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colNPCName;
        private DevExpress.XtraGrid.Columns.GridColumn colImage;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit RegionLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colEntryPage;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit PageLookUpEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
		private DevExpress.XtraGrid.Columns.GridColumn FileName;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox txtSelect;
        private DevExpress.XtraBars.BarButtonItem RefreshTxt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
    }
}