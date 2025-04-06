namespace Server.Views
{
    partial class BuffInfoView
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuffInfoView));
            this.BuffStatsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.StatImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BuffInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.BuffInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Index = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BuffType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BuffTypeImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.BuffLV = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BuffName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BuffGroupColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SmallBuffIcon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BigBuffIcon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GetRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GetRate2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.PauseInSafeZoneColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DurationColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DurationTimeSpanEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
            this.CanDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.BuffStatsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffTypeImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DurationTimeSpanEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // BuffStatsGridView
            // 
            this.BuffStatsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStat,
            this.colAmount});
            this.BuffStatsGridView.GridControl = this.BuffInfoGridControl;
            this.BuffStatsGridView.Name = "BuffStatsGridView";
            this.BuffStatsGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.BuffStatsGridView.OptionsView.EnableAppearanceOddRow = true;
            this.BuffStatsGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.BuffStatsGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.BuffStatsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // colStat
            // 
            this.colStat.Caption = "道具属性值";
            this.colStat.ColumnEdit = this.StatImageComboBox;
            this.colStat.FieldName = "Stat";
            this.colStat.Name = "colStat";
            this.colStat.Visible = true;
            this.colStat.VisibleIndex = 0;
            // 
            // StatImageComboBox
            // 
            this.StatImageComboBox.AutoHeight = false;
            this.StatImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.StatImageComboBox.Name = "StatImageComboBox";
            // 
            // colAmount
            // 
            this.colAmount.Caption = "道具属性的数值";
            this.colAmount.FieldName = "Amount";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 1;
            // 
            // BuffInfoGridControl
            // 
            this.BuffInfoGridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.BuffInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.BuffStatsGridView;
            gridLevelNode1.RelationName = "BuffStats";
            this.BuffInfoGridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.BuffInfoGridControl.Location = new System.Drawing.Point(0, 160);
            this.BuffInfoGridControl.MainView = this.BuffInfoGridView;
            this.BuffInfoGridControl.MenuManager = this.ribbon;
            this.BuffInfoGridControl.Name = "BuffInfoGridControl";
            this.BuffInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.StatImageComboBox,
            this.DurationTimeSpanEdit,
            this.BuffTypeImageComboBox});
            this.BuffInfoGridControl.ShowOnlyPredefinedDetails = true;
            this.BuffInfoGridControl.Size = new System.Drawing.Size(1009, 378);
            this.BuffInfoGridControl.TabIndex = 0;
            this.BuffInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.BuffInfoGridView,
            this.BuffStatsGridView});
            // 
            // BuffInfoGridView
            // 
            this.BuffInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Index,
            this.BuffType,
            this.BuffLV,
            this.BuffName,
            this.BuffGroupColumn,
            this.SmallBuffIcon,
            this.BigBuffIcon,
            this.GetRate,
            this.GetRate2,
            this.PauseInSafeZoneColumn,
            this.DurationColumn,
            this.CanDelete,
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.BuffInfoGridView.DetailHeight = 377;
            this.BuffInfoGridView.GridControl = this.BuffInfoGridControl;
            this.BuffInfoGridView.Name = "BuffInfoGridView";
            this.BuffInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.BuffInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.BuffInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.BuffInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.BuffInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.BuffInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // Index
            // 
            this.Index.Caption = "序号";
            this.Index.FieldName = "Index";
            this.Index.Name = "Index";
            this.Index.OptionsColumn.AllowEdit = false;
            this.Index.Visible = true;
            this.Index.VisibleIndex = 0;
            // 
            // BuffType
            // 
            this.BuffType.Caption = "类型";
            this.BuffType.ColumnEdit = this.BuffTypeImageComboBox;
            this.BuffType.FieldName = "BuffType";
            this.BuffType.Name = "BuffType";
            this.BuffType.Visible = true;
            this.BuffType.VisibleIndex = 1;
            // 
            // BuffTypeImageComboBox
            // 
            this.BuffTypeImageComboBox.AutoHeight = false;
            this.BuffTypeImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.BuffTypeImageComboBox.Name = "BuffTypeImageComboBox";
            // 
            // BuffLV
            // 
            this.BuffLV.Caption = "等级";
            this.BuffLV.FieldName = "BuffLV";
            this.BuffLV.Name = "BuffLV";
            this.BuffLV.Visible = true;
            this.BuffLV.VisibleIndex = 2;
            // 
            // BuffName
            // 
            this.BuffName.Caption = "名称";
            this.BuffName.FieldName = "BuffName";
            this.BuffName.Name = "BuffName";
            this.BuffName.Visible = true;
            this.BuffName.VisibleIndex = 4;
            // 
            // BuffGroupColumn
            // 
            this.BuffGroupColumn.Caption = "buff分组";
            this.BuffGroupColumn.FieldName = "BuffGroup";
            this.BuffGroupColumn.Name = "BuffGroupColumn";
            this.BuffGroupColumn.Visible = true;
            this.BuffGroupColumn.VisibleIndex = 3;
            // 
            // SmallBuffIcon
            // 
            this.SmallBuffIcon.Caption = "小图标";
            this.SmallBuffIcon.FieldName = "SmallBuffIcon";
            this.SmallBuffIcon.Name = "SmallBuffIcon";
            this.SmallBuffIcon.ToolTip = "必填,24x24, 用于buff显示";
            this.SmallBuffIcon.Visible = true;
            this.SmallBuffIcon.VisibleIndex = 5;
            // 
            // BigBuffIcon
            // 
            this.BigBuffIcon.Caption = "大图标";
            this.BigBuffIcon.FieldName = "BigBuffIcon";
            this.BigBuffIcon.Name = "BigBuffIcon";
            this.BigBuffIcon.ToolTip = "非必填,36x36,用于泰山投币界面";
            this.BigBuffIcon.Visible = true;
            this.BigBuffIcon.VisibleIndex = 6;
            // 
            // GetRate
            // 
            this.GetRate.Caption = "获取几率1";
            this.GetRate.FieldName = "GetRate";
            this.GetRate.Name = "GetRate";
            this.GetRate.Visible = true;
            this.GetRate.VisibleIndex = 11;
            // 
            // GetRate2
            // 
            this.GetRate2.Caption = "获取几率2";
            this.GetRate2.FieldName = "GetRate2";
            this.GetRate2.Name = "GetRate2";
            this.GetRate2.Visible = true;
            this.GetRate2.VisibleIndex = 12;
            // 
            // PauseInSafeZoneColumn
            // 
            this.PauseInSafeZoneColumn.Caption = "安全区内是否暂停倒计时";
            this.PauseInSafeZoneColumn.FieldName = "PauseInSafeZone";
            this.PauseInSafeZoneColumn.Name = "PauseInSafeZoneColumn";
            this.PauseInSafeZoneColumn.Visible = true;
            this.PauseInSafeZoneColumn.VisibleIndex = 13;
            // 
            // DurationColumn
            // 
            this.DurationColumn.Caption = "持续时间";
            this.DurationColumn.ColumnEdit = this.DurationTimeSpanEdit;
            this.DurationColumn.FieldName = "Duration";
            this.DurationColumn.Name = "DurationColumn";
            this.DurationColumn.Visible = true;
            this.DurationColumn.VisibleIndex = 16;
            // 
            // DurationTimeSpanEdit
            // 
            this.DurationTimeSpanEdit.AutoHeight = false;
            this.DurationTimeSpanEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DurationTimeSpanEdit.Name = "DurationTimeSpanEdit";
            this.DurationTimeSpanEdit.NullText = "[永久(除非手动删除)]";
            // 
            // CanDelete
            // 
            this.CanDelete.Caption = "勾选0点删除BUFF";
            this.CanDelete.FieldName = "DeleteAtMidnight";
            this.CanDelete.Name = "CanDelete";
            this.CanDelete.Visible = true;
            this.CanDelete.VisibleIndex = 15;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "头顶称号图组序号";
            this.gridColumn1.FieldName = "OverheadTitle";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.ToolTip = "默认固定为20张图为一组";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 7;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "头顶称号图片帧数";
            this.gridColumn3.FieldName = "frameCount";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 8;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "称号图片偏移X坐标";
            this.gridColumn4.FieldName = "offSetX";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 9;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "称号图片偏移Y坐标";
            this.gridColumn5.FieldName = "offSetY";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 10;
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.SaveButton,
            this.barButtonItem1,
            this.barButtonItem2,
            this.ribbon.SearchEditItem});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 4;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1009, 160);
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
            this.ribbonPageGroup1.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
            this.ribbonPageGroup1.ItemLinks.Add(this.SaveButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "存档";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barButtonItem1);
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
            // gridColumn6
            // 
            this.gridColumn6.Caption = "下线是否消耗时间";
            this.gridColumn6.FieldName = "OfflineTicking";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 14;
            // 
            // BuffInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 538);
            this.Controls.Add(this.BuffInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "BuffInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "自定义BUFF设置";
            ((System.ComponentModel.ISupportInitialize)(this.BuffStatsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BuffTypeImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DurationTimeSpanEdit)).EndInit();
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
        private DevExpress.XtraGrid.GridControl BuffInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView BuffInfoGridView;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn Index;
        private DevExpress.XtraGrid.Columns.GridColumn BuffType;
        private DevExpress.XtraGrid.Columns.GridColumn BuffLV;
        private DevExpress.XtraGrid.Columns.GridColumn BuffName;
        private DevExpress.XtraGrid.Columns.GridColumn SmallBuffIcon;
        private DevExpress.XtraGrid.Columns.GridColumn GetRate;
        private DevExpress.XtraGrid.Columns.GridColumn GetRate2;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox StatImageComboBox;
        private DevExpress.XtraGrid.Views.Grid.GridView BuffStatsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn colStat;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn CanDelete;
        private DevExpress.XtraGrid.Columns.GridColumn DurationColumn;
        private DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit DurationTimeSpanEdit;
        private DevExpress.XtraGrid.Columns.GridColumn PauseInSafeZoneColumn;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox BuffTypeImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn BuffGroupColumn;
        private DevExpress.XtraGrid.Columns.GridColumn BigBuffIcon;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}