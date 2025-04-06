namespace Server.Views
{
    partial class ItemCustomInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemCustomInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ItemCustomInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.ItemCustomInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Item = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.DrawInnerEffect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InnerEffectLibrary = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InnerImageStartIndex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InnerImageCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InnerX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.InnerY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DrawOuterEffect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.IsUnisex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OuterEffectLibrary = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OuterImageStartIndex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OuterImageCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OuterX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.OuterY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.EffectBehindImage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.itemDisplayEffectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCustomInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCustomInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemDisplayEffectBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
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
            this.ribbon.Size = new System.Drawing.Size(881, 147);
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
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "导入";
            this.barButtonItem2.Id = 3;
            this.barButtonItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.Image")));
            this.barButtonItem2.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.LargeImage")));
            this.barButtonItem2.Name = "barButtonItem2";
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
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem2);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "数据";
            // 
            // ItemCustomInfoGridControl
            // 
            this.ItemCustomInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ItemCustomInfoGridControl.Location = new System.Drawing.Point(0, 147);
            this.ItemCustomInfoGridControl.MainView = this.ItemCustomInfoGridView;
            this.ItemCustomInfoGridControl.MenuManager = this.ribbon;
            this.ItemCustomInfoGridControl.Name = "ItemCustomInfoGridControl";
            this.ItemCustomInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ItemLookUpEdit,
            this.StatImageComboBox});
            this.ItemCustomInfoGridControl.Size = new System.Drawing.Size(881, 500);
            this.ItemCustomInfoGridControl.TabIndex = 3;
            this.ItemCustomInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.ItemCustomInfoGridView});
            // 
            // ItemCustomInfoGridView
            // 
            this.ItemCustomInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Item,
            this.DrawInnerEffect,
            this.InnerEffectLibrary,
            this.InnerImageStartIndex,
            this.InnerImageCount,
            this.InnerX,
            this.InnerY,
            this.DrawOuterEffect,
            this.IsUnisex,
            this.OuterEffectLibrary,
            this.OuterImageStartIndex,
            this.OuterImageCount,
            this.OuterX,
            this.OuterY,
            this.EffectBehindImage});
            this.ItemCustomInfoGridView.CustomizationFormBounds = new System.Drawing.Rectangle(2046, 515, 260, 242);
            this.ItemCustomInfoGridView.DetailHeight = 377;
            this.ItemCustomInfoGridView.GridControl = this.ItemCustomInfoGridControl;
            this.ItemCustomInfoGridView.Name = "ItemCustomInfoGridView";
            this.ItemCustomInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.ItemCustomInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.ItemCustomInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.ItemCustomInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.ItemCustomInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Item
            // 
            this.Item.Caption = "道具名";
            this.Item.ColumnEdit = this.ItemLookUpEdit;
            this.Item.FieldName = "Info";
            this.Item.MinWidth = 23;
            this.Item.Name = "Item";
            this.Item.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item.ToolTip = "道具名称";
            this.Item.Visible = true;
            this.Item.VisibleIndex = 0;
            this.Item.Width = 65;
            // 
            // ItemLookUpEdit
            // 
            this.ItemLookUpEdit.AutoHeight = false;
            this.ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Image", "Image")});
            this.ItemLookUpEdit.DisplayMember = "ItemName";
            this.ItemLookUpEdit.Name = "ItemLookUpEdit";
            this.ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // DrawInnerEffect
            // 
            this.DrawInnerEffect.Caption = "勾选开启内观";
            this.DrawInnerEffect.FieldName = "DrawInnerEffect";
            this.DrawInnerEffect.Name = "DrawInnerEffect";
            this.DrawInnerEffect.ToolTip = "勾选开启内观";
            this.DrawInnerEffect.Visible = true;
            this.DrawInnerEffect.VisibleIndex = 1;
            this.DrawInnerEffect.Width = 50;
            // 
            // InnerEffectLibrary
            // 
            this.InnerEffectLibrary.Caption = "内观素材库调用";
            this.InnerEffectLibrary.FieldName = "InnerEffectLibrary";
            this.InnerEffectLibrary.Name = "InnerEffectLibrary";
            this.InnerEffectLibrary.ToolTip = "内观库调用";
            this.InnerEffectLibrary.Visible = true;
            this.InnerEffectLibrary.VisibleIndex = 2;
            this.InnerEffectLibrary.Width = 58;
            // 
            // InnerImageStartIndex
            // 
            this.InnerImageStartIndex.Caption = "内观起始图片序号";
            this.InnerImageStartIndex.FieldName = "InnerImageStartIndex";
            this.InnerImageStartIndex.Name = "InnerImageStartIndex";
            this.InnerImageStartIndex.ToolTip = "内观起始图片序号";
            this.InnerImageStartIndex.Visible = true;
            this.InnerImageStartIndex.VisibleIndex = 3;
            this.InnerImageStartIndex.Width = 62;
            // 
            // InnerImageCount
            // 
            this.InnerImageCount.Caption = "内观图片张数";
            this.InnerImageCount.FieldName = "InnerImageCount";
            this.InnerImageCount.Name = "InnerImageCount";
            this.InnerImageCount.ToolTip = "内观图片张数";
            this.InnerImageCount.Visible = true;
            this.InnerImageCount.VisibleIndex = 4;
            this.InnerImageCount.Width = 58;
            // 
            // InnerX
            // 
            this.InnerX.Caption = "内观X坐标(默认120)";
            this.InnerX.FieldName = "InnerX";
            this.InnerX.Name = "InnerX";
            this.InnerX.ToolTip = "内观X坐标 默认120";
            this.InnerX.Visible = true;
            this.InnerX.VisibleIndex = 5;
            this.InnerX.Width = 48;
            // 
            // InnerY
            // 
            this.InnerY.Caption = "内观Y坐标(默认290)";
            this.InnerY.FieldName = "InnerY";
            this.InnerY.Name = "InnerY";
            this.InnerY.ToolTip = "内观Y坐标 默认290";
            this.InnerY.Visible = true;
            this.InnerY.VisibleIndex = 6;
            this.InnerY.Width = 45;
            // 
            // DrawOuterEffect
            // 
            this.DrawOuterEffect.Caption = "勾选开启特效外观";
            this.DrawOuterEffect.FieldName = "DrawOuterEffect";
            this.DrawOuterEffect.Name = "DrawOuterEffect";
            this.DrawOuterEffect.ToolTip = "勾选开启外观";
            this.DrawOuterEffect.Visible = true;
            this.DrawOuterEffect.VisibleIndex = 8;
            this.DrawOuterEffect.Width = 46;
            // 
            // IsUnisex
            // 
            this.IsUnisex.Caption = "勾选男女使用相同特效";
            this.IsUnisex.FieldName = "IsUnisex";
            this.IsUnisex.Name = "IsUnisex";
            this.IsUnisex.ToolTip = "勾选代表男女外观特效相同";
            this.IsUnisex.Visible = true;
            this.IsUnisex.VisibleIndex = 10;
            this.IsUnisex.Width = 57;
            // 
            // OuterEffectLibrary
            // 
            this.OuterEffectLibrary.Caption = "外观素材库调用";
            this.OuterEffectLibrary.FieldName = "OuterEffectLibrary";
            this.OuterEffectLibrary.Name = "OuterEffectLibrary";
            this.OuterEffectLibrary.ToolTip = "外观库调用";
            this.OuterEffectLibrary.Visible = true;
            this.OuterEffectLibrary.VisibleIndex = 9;
            this.OuterEffectLibrary.Width = 58;
            // 
            // OuterImageStartIndex
            // 
            this.OuterImageStartIndex.Caption = "外观起始图片序号";
            this.OuterImageStartIndex.FieldName = "OuterImageStartIndex";
            this.OuterImageStartIndex.Name = "OuterImageStartIndex";
            this.OuterImageStartIndex.ToolTip = "外观起始图片序号";
            this.OuterImageStartIndex.Visible = true;
            this.OuterImageStartIndex.VisibleIndex = 11;
            this.OuterImageStartIndex.Width = 61;
            // 
            // OuterImageCount
            // 
            this.OuterImageCount.Caption = "外观图片张数";
            this.OuterImageCount.FieldName = "OuterImageCount";
            this.OuterImageCount.Name = "OuterImageCount";
            this.OuterImageCount.ToolTip = "外观图片张数";
            this.OuterImageCount.Visible = true;
            this.OuterImageCount.VisibleIndex = 12;
            this.OuterImageCount.Width = 63;
            // 
            // OuterX
            // 
            this.OuterX.Caption = "外观X坐标";
            this.OuterX.FieldName = "OuterX";
            this.OuterX.Name = "OuterX";
            this.OuterX.ToolTip = "外观X坐标";
            this.OuterX.Visible = true;
            this.OuterX.VisibleIndex = 13;
            this.OuterX.Width = 39;
            // 
            // OuterY
            // 
            this.OuterY.Caption = "外观Y坐标";
            this.OuterY.FieldName = "OuterY";
            this.OuterY.Name = "OuterY";
            this.OuterY.ToolTip = "外观Y坐标";
            this.OuterY.Visible = true;
            this.OuterY.VisibleIndex = 14;
            this.OuterY.Width = 94;
            // 
            // EffectBehindImage
            // 
            this.EffectBehindImage.Caption = "勾选特效图绘制在底层";
            this.EffectBehindImage.FieldName = "EffectBehindImage";
            this.EffectBehindImage.Name = "EffectBehindImage";
            this.EffectBehindImage.ToolTip = "勾选特效绘制在底图前面";
            this.EffectBehindImage.Visible = true;
            this.EffectBehindImage.VisibleIndex = 7;
            this.EffectBehindImage.Width = 59;
            // 
            // StatImageComboBox
            // 
            this.StatImageComboBox.AutoHeight = false;
            this.StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatImageComboBox.Name = "StatImageComboBox";
            // 
            // itemDisplayEffectBindingSource
            // 
            this.itemDisplayEffectBindingSource.DataSource = typeof(Library.SystemModels.ItemDisplayEffect);
            // 
            // ItemCustomInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 647);
            this.Controls.Add(this.ItemCustomInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "ItemCustomInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "道具特效设置";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCustomInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCustomInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemDisplayEffectBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.GridControl ItemCustomInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView ItemCustomInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn Item;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn DrawInnerEffect;
        private DevExpress.XtraGrid.Columns.GridColumn InnerEffectLibrary;
        private DevExpress.XtraGrid.Columns.GridColumn InnerImageStartIndex;
        private DevExpress.XtraGrid.Columns.GridColumn InnerImageCount;
        private DevExpress.XtraGrid.Columns.GridColumn InnerX;
        private DevExpress.XtraGrid.Columns.GridColumn InnerY;
        private DevExpress.XtraGrid.Columns.GridColumn DrawOuterEffect;
        private DevExpress.XtraGrid.Columns.GridColumn IsUnisex;
        private DevExpress.XtraGrid.Columns.GridColumn OuterEffectLibrary;
        private DevExpress.XtraGrid.Columns.GridColumn OuterImageStartIndex;
        private DevExpress.XtraGrid.Columns.GridColumn OuterImageCount;
        private DevExpress.XtraGrid.Columns.GridColumn OuterX;
        private DevExpress.XtraGrid.Columns.GridColumn OuterY;
        private System.Windows.Forms.BindingSource itemDisplayEffectBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn EffectBehindImage;
    }
}