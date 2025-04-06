namespace Server.Views
{
    partial class LangInfoView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LangInfoView));
            this.LangGridControl = new DevExpress.XtraGrid.GridControl();
            this.LangInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Type = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TypeLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.LangType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.LangTypeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.Key = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Value = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.LangGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LangInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TypeLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LangTypeComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // LangGridControl
            // 
            this.LangGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LangGridControl.Location = new System.Drawing.Point(0, 147);
            this.LangGridControl.MainView = this.LangInfoGridView;
            this.LangGridControl.MenuManager = this.ribbon;
            this.LangGridControl.Name = "LangGridControl";
            this.LangGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.TypeLookUpEdit,
            this.LangTypeComboBox});
            this.LangGridControl.Size = new System.Drawing.Size(1003, 388);
            this.LangGridControl.TabIndex = 0;
            this.LangGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.LangInfoGridView});
            // 
            // LangInfoGridView
            // 
            this.LangInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Type,
            this.LangType,
            this.Key,
            this.Value});
            this.LangInfoGridView.DetailHeight = 377;
            this.LangInfoGridView.GridControl = this.LangGridControl;
            this.LangInfoGridView.Name = "LangInfoGridView";
            this.LangInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.LangInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.LangInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.LangInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.LangInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.LangInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Type
            // 
            this.Type.Caption = "类型";
            this.Type.ColumnEdit = this.TypeLookUpEdit;
            this.Type.FieldName = "Type";
            this.Type.MaxWidth = 100;
            this.Type.MinWidth = 100;
            this.Type.Name = "Type";
            this.Type.Visible = true;
            this.Type.VisibleIndex = 0;
            this.Type.Width = 100;
            // 
            // TypeLookUpEdit
            // 
            this.TypeLookUpEdit.AutoHeight = false;
            this.TypeLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.TypeLookUpEdit.Name = "TypeLookUpEdit";
            // 
            // LangType
            // 
            this.LangType.Caption = "语言";
            this.LangType.ColumnEdit = this.LangTypeComboBox;
            this.LangType.FieldName = "LangType";
            this.LangType.MaxWidth = 100;
            this.LangType.MinWidth = 100;
            this.LangType.Name = "LangType";
            this.LangType.Visible = true;
            this.LangType.VisibleIndex = 1;
            this.LangType.Width = 100;
            // 
            // LangTypeComboBox
            // 
            this.LangTypeComboBox.AutoHeight = false;
            this.LangTypeComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.LangTypeComboBox.Name = "LangTypeComboBox";
            // 
            // Key
            // 
            this.Key.Caption = "Key";
            this.Key.FieldName = "Key";
            this.Key.MaxWidth = 350;
            this.Key.MinWidth = 350;
            this.Key.Name = "Key";
            this.Key.Visible = true;
            this.Key.VisibleIndex = 2;
            this.Key.Width = 350;
            // 
            // Value
            // 
            this.Value.Caption = "值";
            this.Value.FieldName = "Value";
            this.Value.MinWidth = 300;
            this.Value.Name = "Value";
            this.Value.Visible = true;
            this.Value.VisibleIndex = 3;
            this.Value.Width = 485;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.barButtonItem1,
            this.barButtonItem2});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1003, 147);
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
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem1, true);
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "数据";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // LangInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 535);
            this.Controls.Add(this.LangGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "LangInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "国际配置";
            ((System.ComponentModel.ISupportInitialize)(this.LangGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LangInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TypeLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LangTypeComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl LangGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView LangInfoGridView;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn Type;
        private DevExpress.XtraGrid.Columns.GridColumn LangType;
        private DevExpress.XtraGrid.Columns.GridColumn Value;
        private DevExpress.XtraGrid.Columns.GridColumn Key;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit TypeLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox LangTypeComboBox;
    }
}