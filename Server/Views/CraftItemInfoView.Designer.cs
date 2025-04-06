namespace Server.Views
{
    partial class CraftItemInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CraftItemInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.CraftItemInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.CraftItemInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Item = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.Blueprint = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SortNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TargetAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SuccessRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GainExp = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TimeCost = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GoldCost = new DevExpress.XtraGrid.Columns.GridColumn();
            this.LevelNeeded = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Item1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Amount1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Item2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Amount2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Item3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Amount3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Item4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Amount4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Item5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Amount5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CraftItemInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CraftItemInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).BeginInit();
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
            this.ribbon.Size = new System.Drawing.Size(925, 147);
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
            // CraftItemInfoGridControl
            // 
            this.CraftItemInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CraftItemInfoGridControl.Location = new System.Drawing.Point(0, 147);
            this.CraftItemInfoGridControl.MainView = this.CraftItemInfoGridView;
            this.CraftItemInfoGridControl.MenuManager = this.ribbon;
            this.CraftItemInfoGridControl.Name = "CraftItemInfoGridControl";
            this.CraftItemInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ItemLookUpEdit,
            this.StatImageComboBox});
            this.CraftItemInfoGridControl.Size = new System.Drawing.Size(925, 497);
            this.CraftItemInfoGridControl.TabIndex = 3;
            this.CraftItemInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CraftItemInfoGridView});
            // 
            // CraftItemInfoGridView
            // 
            this.CraftItemInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Item,
            this.Blueprint,
            this.SortNumber,
            this.TargetAmount,
            this.SuccessRate,
            this.GainExp,
            this.TimeCost,
            this.GoldCost,
            this.LevelNeeded,
            this.Item1,
            this.Amount1,
            this.Item2,
            this.Amount2,
            this.Item3,
            this.Amount3,
            this.Item4,
            this.Amount4,
            this.Item5,
            this.Amount5});
            this.CraftItemInfoGridView.CustomizationFormBounds = new System.Drawing.Rectangle(2046, 515, 260, 242);
            this.CraftItemInfoGridView.DetailHeight = 377;
            this.CraftItemInfoGridView.GridControl = this.CraftItemInfoGridControl;
            this.CraftItemInfoGridView.Name = "CraftItemInfoGridView";
            this.CraftItemInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CraftItemInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CraftItemInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CraftItemInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CraftItemInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Item
            // 
            this.Item.Caption = "道具名";
            this.Item.ColumnEdit = this.ItemLookUpEdit;
            this.Item.FieldName = "Item";
            this.Item.Name = "Item";
            this.Item.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item.ToolTip = "目标道具(制作出来的物品)";
            this.Item.Visible = true;
            this.Item.VisibleIndex = 0;
            // 
            // ItemLookUpEdit
            // 
            this.ItemLookUpEdit.AutoHeight = false;
            this.ItemLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.ItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItemName", "Item Name")});
            this.ItemLookUpEdit.DisplayMember = "ItemName";
            this.ItemLookUpEdit.Name = "ItemLookUpEdit";
            this.ItemLookUpEdit.NullText = "[Item is null]";
            // 
            // Blueprint
            // 
            this.Blueprint.Caption = "图纸编号";
            this.Blueprint.FieldName = "Blueprint";
            this.Blueprint.Name = "Blueprint";
            this.Blueprint.ToolTip = "合成方案编号(图纸编号)";
            this.Blueprint.Visible = true;
            this.Blueprint.VisibleIndex = 1;
            // 
            // SortNumber
            // 
            this.SortNumber.Caption = "列表排序号";
            this.SortNumber.FieldName = "SortNumber";
            this.SortNumber.Name = "SortNumber";
            this.SortNumber.ToolTip = "排序号(显示在列表里的排序)";
            this.SortNumber.Visible = true;
            this.SortNumber.VisibleIndex = 2;
            // 
            // TargetAmount
            // 
            this.TargetAmount.Caption = "制作获得数量";
            this.TargetAmount.FieldName = "TargetAmount";
            this.TargetAmount.Name = "TargetAmount";
            this.TargetAmount.ToolTip = "制作数量(制作获得的道具数量)";
            this.TargetAmount.Visible = true;
            this.TargetAmount.VisibleIndex = 3;
            // 
            // SuccessRate
            // 
            this.SuccessRate.Caption = "制作成功率";
            this.SuccessRate.FieldName = "SuccessRate";
            this.SuccessRate.Name = "SuccessRate";
            this.SuccessRate.ToolTip = "制作成功率";
            this.SuccessRate.Visible = true;
            this.SuccessRate.VisibleIndex = 4;
            // 
            // GainExp
            // 
            this.GainExp.Caption = "获得熟练度";
            this.GainExp.FieldName = "GainExp";
            this.GainExp.Name = "GainExp";
            this.GainExp.ToolTip = "获得熟练度";
            this.GainExp.Visible = true;
            this.GainExp.VisibleIndex = 5;
            // 
            // TimeCost
            // 
            this.TimeCost.Caption = "消耗时间";
            this.TimeCost.FieldName = "TimeCost";
            this.TimeCost.Name = "TimeCost";
            this.TimeCost.ToolTip = "消耗时间";
            this.TimeCost.Visible = true;
            this.TimeCost.VisibleIndex = 6;
            // 
            // GoldCost
            // 
            this.GoldCost.Caption = "消耗金币";
            this.GoldCost.FieldName = "GoldCost";
            this.GoldCost.Name = "GoldCost";
            this.GoldCost.ToolTip = "消耗金币";
            this.GoldCost.Visible = true;
            this.GoldCost.VisibleIndex = 7;
            // 
            // LevelNeeded
            // 
            this.LevelNeeded.Caption = "制作技能要求等级";
            this.LevelNeeded.FieldName = "LevelNeeded";
            this.LevelNeeded.Name = "LevelNeeded";
            this.LevelNeeded.ToolTip = "打造技能等级要求";
            this.LevelNeeded.Visible = true;
            this.LevelNeeded.VisibleIndex = 8;
            // 
            // Item1
            // 
            this.Item1.Caption = "材料一";
            this.Item1.ColumnEdit = this.ItemLookUpEdit;
            this.Item1.FieldName = "Item1";
            this.Item1.Name = "Item1";
            this.Item1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item1.ToolTip = "原料1";
            this.Item1.Visible = true;
            this.Item1.VisibleIndex = 9;
            // 
            // Amount1
            // 
            this.Amount1.Caption = "材料一数量";
            this.Amount1.FieldName = "Amount1";
            this.Amount1.Name = "Amount1";
            this.Amount1.ToolTip = "原料1数量";
            this.Amount1.Visible = true;
            this.Amount1.VisibleIndex = 10;
            // 
            // Item2
            // 
            this.Item2.Caption = "材料二";
            this.Item2.ColumnEdit = this.ItemLookUpEdit;
            this.Item2.FieldName = "Item2";
            this.Item2.Name = "Item2";
            this.Item2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item2.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item2.ToolTip = "原料2";
            this.Item2.Visible = true;
            this.Item2.VisibleIndex = 11;
            // 
            // Amount2
            // 
            this.Amount2.Caption = "材料二数量";
            this.Amount2.FieldName = "Amount2";
            this.Amount2.Name = "Amount2";
            this.Amount2.ToolTip = "原料2数量";
            this.Amount2.Visible = true;
            this.Amount2.VisibleIndex = 12;
            // 
            // Item3
            // 
            this.Item3.Caption = "材料三";
            this.Item3.ColumnEdit = this.ItemLookUpEdit;
            this.Item3.FieldName = "Item3";
            this.Item3.Name = "Item3";
            this.Item3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item3.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item3.ToolTip = "原料3";
            this.Item3.Visible = true;
            this.Item3.VisibleIndex = 13;
            // 
            // Amount3
            // 
            this.Amount3.Caption = "材料三数量";
            this.Amount3.FieldName = "Amount3";
            this.Amount3.Name = "Amount3";
            this.Amount3.ToolTip = "原料3数量";
            this.Amount3.Visible = true;
            this.Amount3.VisibleIndex = 14;
            // 
            // Item4
            // 
            this.Item4.Caption = "材料四";
            this.Item4.ColumnEdit = this.ItemLookUpEdit;
            this.Item4.FieldName = "Item4";
            this.Item4.Name = "Item4";
            this.Item4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item4.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item4.ToolTip = "原料4";
            this.Item4.Visible = true;
            this.Item4.VisibleIndex = 15;
            // 
            // Amount4
            // 
            this.Amount4.Caption = "材料四数量";
            this.Amount4.FieldName = "Amount4";
            this.Amount4.Name = "Amount4";
            this.Amount4.ToolTip = "原料4数量";
            this.Amount4.Visible = true;
            this.Amount4.VisibleIndex = 16;
            // 
            // Item5
            // 
            this.Item5.Caption = "材料五";
            this.Item5.ColumnEdit = this.ItemLookUpEdit;
            this.Item5.FieldName = "Item5";
            this.Item5.Name = "Item5";
            this.Item5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.Item5.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Item5.ToolTip = "原料5";
            this.Item5.Visible = true;
            this.Item5.VisibleIndex = 17;
            // 
            // Amount5
            // 
            this.Amount5.Caption = "材料五数量";
            this.Amount5.FieldName = "Amount5";
            this.Amount5.Name = "Amount5";
            this.Amount5.ToolTip = "原料5数量";
            this.Amount5.Visible = true;
            this.Amount5.VisibleIndex = 18;
            // 
            // StatImageComboBox
            // 
            this.StatImageComboBox.AutoHeight = false;
            this.StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatImageComboBox.Name = "StatImageComboBox";
            // 
            // CraftItemInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 644);
            this.Controls.Add(this.CraftItemInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "CraftItemInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "道具制作设置";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CraftItemInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CraftItemInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).EndInit();
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
        private DevExpress.XtraGrid.GridControl CraftItemInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CraftItemInfoGridView;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit ItemLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn Item;
        private DevExpress.XtraGrid.Columns.GridColumn Blueprint;
        private DevExpress.XtraGrid.Columns.GridColumn SortNumber;
        private DevExpress.XtraGrid.Columns.GridColumn TargetAmount;
        private DevExpress.XtraGrid.Columns.GridColumn SuccessRate;
        private DevExpress.XtraGrid.Columns.GridColumn GainExp;
        private DevExpress.XtraGrid.Columns.GridColumn TimeCost;
        private DevExpress.XtraGrid.Columns.GridColumn GoldCost;
        private DevExpress.XtraGrid.Columns.GridColumn LevelNeeded;
        private DevExpress.XtraGrid.Columns.GridColumn Item1;
        private DevExpress.XtraGrid.Columns.GridColumn Amount1;
        private DevExpress.XtraGrid.Columns.GridColumn Item2;
        private DevExpress.XtraGrid.Columns.GridColumn Amount2;
        private DevExpress.XtraGrid.Columns.GridColumn Item3;
        private DevExpress.XtraGrid.Columns.GridColumn Amount3;
        private DevExpress.XtraGrid.Columns.GridColumn Item4;
        private DevExpress.XtraGrid.Columns.GridColumn Amount4;
        private DevExpress.XtraGrid.Columns.GridColumn Item5;
        private DevExpress.XtraGrid.Columns.GridColumn Amount5;
    }
}