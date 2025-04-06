namespace Server.Views
{
    partial class MagicInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagicInfoView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.MagicImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.SchoolImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.ClassImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.ActionImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.MagicInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn17 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn18 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn19 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn20 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn21 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MagicInfoGridControl = new DevExpress.XtraGrid.GridControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClassImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicInfoGridControl)).BeginInit();
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
            this.ribbon.Size = new System.Drawing.Size(1045, 147);
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
            // MagicImageComboBox
            // 
            this.MagicImageComboBox.AutoHeight = false;
            this.MagicImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MagicImageComboBox.Name = "MagicImageComboBox";
            // 
            // SchoolImageComboBox
            // 
            this.SchoolImageComboBox.AutoHeight = false;
            this.SchoolImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.SchoolImageComboBox.Name = "SchoolImageComboBox";
            // 
            // ClassImageComboBox
            // 
            this.ClassImageComboBox.AutoHeight = false;
            this.ClassImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ClassImageComboBox.Name = "ClassImageComboBox";
            // 
            // ActionImageComboBox
            // 
            this.ActionImageComboBox.AutoHeight = false;
            this.ActionImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ActionImageComboBox.Name = "ActionImageComboBox";
            // 
            // MagicInfoGridView
            // 
            this.MagicInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn15,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn16,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn13,
            this.gridColumn8,
            this.gridColumn17,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn18,
            this.gridColumn19,
            this.gridColumn14,
            this.gridColumn20,
            this.gridColumn21});
            this.MagicInfoGridView.DetailHeight = 377;
            this.MagicInfoGridView.GridControl = this.MagicInfoGridControl;
            this.MagicInfoGridView.Name = "MagicInfoGridView";
            this.MagicInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.MagicInfoGridView.OptionsView.ColumnAutoWidth = false;
            this.MagicInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.MagicInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.MagicInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.MagicInfoGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "技能序号";
            this.gridColumn15.FieldName = "Index";
            this.gridColumn15.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn15.MinWidth = 23;
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.OptionsColumn.AllowEdit = false;
            this.gridColumn15.OptionsColumn.ReadOnly = true;
            this.gridColumn15.ToolTip = "技能唯一序号";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 0;
            this.gridColumn15.Width = 87;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "技能名字";
            this.gridColumn1.FieldName = "Name";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.ToolTip = "技能名称";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 87;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "对应技能代码";
            this.gridColumn2.ColumnEdit = this.MagicImageComboBox;
            this.gridColumn2.FieldName = "Magic";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.ToolTip = "对应技能";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 87;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "技能树";
            this.gridColumn3.ColumnEdit = this.SchoolImageComboBox;
            this.gridColumn3.FieldName = "School";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ToolTip = "技能树对应位置";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            this.gridColumn3.Width = 87;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "职业";
            this.gridColumn4.ColumnEdit = this.ClassImageComboBox;
            this.gridColumn4.FieldName = "Class";
            this.gridColumn4.MinWidth = 23;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.ToolTip = "对应职业";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 87;
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "技能图标";
            this.gridColumn16.FieldName = "Icon";
            this.gridColumn16.MinWidth = 23;
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.ToolTip = "技能图标，对应客户端Data文件夹MIcon.ZL文件";
            this.gridColumn16.Visible = true;
            this.gridColumn16.VisibleIndex = 6;
            this.gridColumn16.Width = 87;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "基础施法MP值";
            this.gridColumn5.FieldName = "BaseCost";
            this.gridColumn5.MinWidth = 23;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.ToolTip = "基础每次施法需要魔法值";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 7;
            this.gridColumn5.Width = 87;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "升级施法MP值";
            this.gridColumn6.FieldName = "LevelCost";
            this.gridColumn6.MinWidth = 23;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.ToolTip = "升级每次施法需要魔法值";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 8;
            this.gridColumn6.Width = 87;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "最低基础攻击值";
            this.gridColumn7.FieldName = "MinBasePower";
            this.gridColumn7.MinWidth = 23;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.ToolTip = "最低基础攻击值";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 9;
            this.gridColumn7.Width = 87;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "最大基础攻击值";
            this.gridColumn13.FieldName = "MaxBasePower";
            this.gridColumn13.MinWidth = 23;
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.ToolTip = "最高基础攻击值";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 10;
            this.gridColumn13.Width = 87;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "最低升级攻击值";
            this.gridColumn8.FieldName = "MinLevelPower";
            this.gridColumn8.MinWidth = 23;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.ToolTip = "最低升级攻击值";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 11;
            this.gridColumn8.Width = 87;
            // 
            // gridColumn17
            // 
            this.gridColumn17.Caption = "最大升级攻击值";
            this.gridColumn17.FieldName = "MaxLevelPower";
            this.gridColumn17.MinWidth = 23;
            this.gridColumn17.Name = "gridColumn17";
            this.gridColumn17.ToolTip = "最高提升攻击值";
            this.gridColumn17.Visible = true;
            this.gridColumn17.VisibleIndex = 12;
            this.gridColumn17.Width = 87;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "1级修炼等级";
            this.gridColumn9.FieldName = "NeedLevel1";
            this.gridColumn9.MinWidth = 23;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.ToolTip = "1级修炼等级";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 13;
            this.gridColumn9.Width = 87;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "2级修炼等级";
            this.gridColumn10.FieldName = "NeedLevel2";
            this.gridColumn10.MinWidth = 23;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.ToolTip = "2级修炼等级";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 14;
            this.gridColumn10.Width = 87;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "3级修炼等级";
            this.gridColumn11.FieldName = "NeedLevel3";
            this.gridColumn11.MinWidth = 23;
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.ToolTip = "3级修炼等级";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 15;
            this.gridColumn11.Width = 87;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "1级修炼值";
            this.gridColumn12.FieldName = "Experience1";
            this.gridColumn12.MinWidth = 23;
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.ToolTip = "1级修炼值";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 16;
            this.gridColumn12.Width = 87;
            // 
            // gridColumn18
            // 
            this.gridColumn18.Caption = "2级修炼值";
            this.gridColumn18.FieldName = "Experience2";
            this.gridColumn18.MinWidth = 23;
            this.gridColumn18.Name = "gridColumn18";
            this.gridColumn18.ToolTip = "2级修炼值";
            this.gridColumn18.Visible = true;
            this.gridColumn18.VisibleIndex = 17;
            this.gridColumn18.Width = 87;
            // 
            // gridColumn19
            // 
            this.gridColumn19.Caption = "3级修炼值";
            this.gridColumn19.FieldName = "Experience3";
            this.gridColumn19.MinWidth = 23;
            this.gridColumn19.Name = "gridColumn19";
            this.gridColumn19.ToolTip = "3级修炼值";
            this.gridColumn19.Visible = true;
            this.gridColumn19.VisibleIndex = 18;
            this.gridColumn19.Width = 87;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "技能释放延迟";
            this.gridColumn14.FieldName = "Delay";
            this.gridColumn14.MinWidth = 23;
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.ToolTip = "技能施法延迟";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 19;
            this.gridColumn14.Width = 87;
            // 
            // gridColumn20
            // 
            this.gridColumn20.Caption = "技能说明";
            this.gridColumn20.FieldName = "Description";
            this.gridColumn20.MinWidth = 23;
            this.gridColumn20.Name = "gridColumn20";
            this.gridColumn20.ToolTip = "技能文字说明";
            this.gridColumn20.Visible = true;
            this.gridColumn20.VisibleIndex = 20;
            this.gridColumn20.Width = 87;
            // 
            // gridColumn21
            // 
            this.gridColumn21.Caption = "攻击类型";
            this.gridColumn21.ColumnEdit = this.ActionImageComboBox;
            this.gridColumn21.FieldName = "Action";
            this.gridColumn21.MinWidth = 23;
            this.gridColumn21.Name = "gridColumn21";
            this.gridColumn21.ToolTip = "技能说明主动或被动";
            this.gridColumn21.Visible = true;
            this.gridColumn21.VisibleIndex = 5;
            this.gridColumn21.Width = 87;
            // 
            // MagicInfoGridControl
            // 
            this.MagicInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MagicInfoGridControl.Location = new System.Drawing.Point(0, 147);
            this.MagicInfoGridControl.MainView = this.MagicInfoGridView;
            this.MagicInfoGridControl.MenuManager = this.ribbon;
            this.MagicInfoGridControl.Name = "MagicInfoGridControl";
            this.MagicInfoGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MagicImageComboBox,
            this.SchoolImageComboBox,
            this.ClassImageComboBox,
            this.ActionImageComboBox});
            this.MagicInfoGridControl.Size = new System.Drawing.Size(1045, 433);
            this.MagicInfoGridControl.TabIndex = 2;
            this.MagicInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.MagicInfoGridView});
            // 
            // MagicInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 580);
            this.Controls.Add(this.MagicInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "MagicInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "魔法技能设置";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SchoolImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ClassImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActionImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicInfoGridControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox MagicImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox SchoolImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ClassImageComboBox;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ActionImageComboBox;
        private DevExpress.XtraGrid.Views.Grid.GridView MagicInfoGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.GridControl MagicInfoGridControl;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn17;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn18;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn19;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn20;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn21;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
    }
}