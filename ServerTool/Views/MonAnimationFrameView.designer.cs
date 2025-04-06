namespace Server.Views
{
    partial class MonAnimationFrameView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonAnimationFrameView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.MonAnimationFrameGridControl = new DevExpress.XtraGrid.GridControl();
            this.MonAnimationFrameGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MonLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MirActImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.effectfileComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ActSoundComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.MagicDir = new DevExpress.XtraGrid.Columns.GridColumn();
            this.EffectdirComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.gridColumn16 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonAnimationFrameGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonAnimationFrameGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonLookUpEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MirActImageComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.effectfileComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActSoundComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectdirComboBox)).BeginInit();
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
            this.barButtonItem3});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1356, 147);
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
            this.ribbonPageGroup2,
            this.ribbonPageGroup3});
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
            // MonAnimationFrameGridControl
            // 
            this.MonAnimationFrameGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MonAnimationFrameGridControl.Location = new System.Drawing.Point(0, 147);
            this.MonAnimationFrameGridControl.MainView = this.MonAnimationFrameGridView;
            this.MonAnimationFrameGridControl.MenuManager = this.ribbon;
            this.MonAnimationFrameGridControl.Name = "MonAnimationFrameGridControl";
            this.MonAnimationFrameGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MonLookUpEdit,
            this.MirActImageComboBox,
            this.effectfileComboBox,
            this.EffectdirComboBox,
            this.ActSoundComboBox});
            this.MonAnimationFrameGridControl.Size = new System.Drawing.Size(1356, 362);
            this.MonAnimationFrameGridControl.TabIndex = 2;
            this.MonAnimationFrameGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.MonAnimationFrameGridView});
            // 
            // MonAnimationFrameGridView
            // 
            this.MonAnimationFrameGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.MagicDir,
            this.gridColumn16,
            this.gridColumn14});
            this.MonAnimationFrameGridView.DetailHeight = 377;
            this.MonAnimationFrameGridView.GridControl = this.MonAnimationFrameGridControl;
            this.MonAnimationFrameGridView.Name = "MonAnimationFrameGridView";
            this.MonAnimationFrameGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.MonAnimationFrameGridView.OptionsView.EnableAppearanceOddRow = true;
            this.MonAnimationFrameGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.MonAnimationFrameGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.MonAnimationFrameGridView.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "怪物信息";
            this.gridColumn1.ColumnEdit = this.MonLookUpEdit;
            this.gridColumn1.FieldName = "Monster";
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColumn1.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.gridColumn1.ToolTip = "怪物的名字";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 56;
            // 
            // MonLookUpEdit
            // 
            this.MonLookUpEdit.AutoHeight = false;
            this.MonLookUpEdit.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            this.MonLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MonLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Index", "Index"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MonsterName", "Monster Name"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AI", "AI"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Level", "Level"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Experience", "Experience"),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("IsBoss", "IsBoss")});
            this.MonLookUpEdit.DisplayMember = "MonsterName";
            this.MonLookUpEdit.Name = "MonLookUpEdit";
            this.MonLookUpEdit.NullText = "[Monster is null]";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "怪物行为动作";
            this.gridColumn2.ColumnEdit = this.MirActImageComboBox;
            this.gridColumn2.FieldName = "MonAnimation";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.ToolTip = "具体行为动画，比方站立，行走，攻击等等，每一个一行";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 56;
            // 
            // MirActImageComboBox
            // 
            this.MirActImageComboBox.AutoHeight = false;
            this.MirActImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.MirActImageComboBox.Name = "MirActImageComboBox";
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "动画起始序号";
            this.gridColumn3.FieldName = "startIndex";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ToolTip = "起始图片动画序号，数据库里动画的起始值";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            this.gridColumn3.Width = 56;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "动画帧数";
            this.gridColumn4.FieldName = "frameCount";
            this.gridColumn4.MinWidth = 23;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.ToolTip = "图片动画的帧数，有多少张图片";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 56;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "动画偏移";
            this.gridColumn5.FieldName = "offSet";
            this.gridColumn5.MinWidth = 23;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.ToolTip = "图片动画的偏移量";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            this.gridColumn5.Width = 56;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "动画循环速度(毫秒)";
            this.gridColumn6.FieldName = "frameDelay";
            this.gridColumn6.MinWidth = 23;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.ToolTip = "图片动画循环，单位为毫秒";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 6;
            this.gridColumn6.Width = 56;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "勾选方向播放";
            this.gridColumn7.FieldName = "Reversed";
            this.gridColumn7.MinWidth = 23;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.ToolTip = "勾选反转播放图片";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 7;
            this.gridColumn7.Width = 56;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "勾选固定速度播放";
            this.gridColumn8.FieldName = "StaticSpeed";
            this.gridColumn8.MinWidth = 23;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.ToolTip = "勾选固定速度播放图片";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 8;
            this.gridColumn8.Width = 56;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "技能魔法库选择";
            this.gridColumn9.ColumnEdit = this.effectfileComboBox;
            this.gridColumn9.FieldName = "effectfile";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.ToolTip = "技能魔法特效库选择";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 11;
            this.gridColumn9.Width = 48;
            // 
            // effectfileComboBox
            // 
            this.effectfileComboBox.AutoHeight = false;
            this.effectfileComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.effectfileComboBox.Name = "effectfileComboBox";
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "技能动画起始序号";
            this.gridColumn10.FieldName = "effectfrom";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.ToolTip = "技能特效起始图片序号";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 12;
            this.gridColumn10.Width = 90;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "技能动画帧数";
            this.gridColumn11.FieldName = "effectframe";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.ToolTip = "技能特效图片张数";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 13;
            this.gridColumn11.Width = 39;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "技能动画循环速度(毫秒)";
            this.gridColumn12.FieldName = "effectdelay";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.ToolTip = "技能特效图片循环时间";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 14;
            this.gridColumn12.Width = 39;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "动作声音序号";
            this.gridColumn13.ColumnEdit = this.ActSoundComboBox;
            this.gridColumn13.FieldName = "ActSound";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 9;
            // 
            // ActSoundComboBox
            // 
            this.ActSoundComboBox.AutoHeight = false;
            this.ActSoundComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.ActSoundComboBox.Name = "ActSoundComboBox";
            // 
            // MagicDir
            // 
            this.MagicDir.Caption = "魔法技能方向";
            this.MagicDir.ColumnEdit = this.EffectdirComboBox;
            this.MagicDir.FieldName = "effectdir";
            this.MagicDir.Name = "MagicDir";
            this.MagicDir.ToolTip = "怪物技能魔法方向设置";
            this.MagicDir.Visible = true;
            this.MagicDir.VisibleIndex = 15;
            // 
            // EffectdirComboBox
            // 
            this.EffectdirComboBox.AutoHeight = false;
            this.EffectdirComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.EffectdirComboBox.Name = "EffectdirComboBox";
            // 
            // gridColumn16
            // 
            this.gridColumn16.Caption = "序号";
            this.gridColumn16.FieldName = "MonsterIdx";
            this.gridColumn16.Name = "gridColumn16";
            this.gridColumn16.Visible = true;
            this.gridColumn16.VisibleIndex = 1;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "自定动作声音";
            this.gridColumn14.FieldName = "ActSoundStr";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 10;
            // 
            // ribbonPageGroup3
            // 
            this.ribbonPageGroup3.ItemLinks.Add(this.barButtonItem3);
            this.ribbonPageGroup3.Name = "ribbonPageGroup3";
            this.ribbonPageGroup3.Text = "刷怪新怪";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "刷新";
            this.barButtonItem3.Id = 4;
            this.barButtonItem3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.ImageOptions.Image")));
            this.barButtonItem3.Name = "barButtonItem3";
            this.barButtonItem3.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
            this.barButtonItem3.ItemDoubleClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemDoubleClick);
            // 
            // MonAnimationFrameView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1356, 509);
            this.Controls.Add(this.MonAnimationFrameGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "MonAnimationFrameView";
            this.Ribbon = this.ribbon;
            this.Text = "自定义怪物动作";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonAnimationFrameGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonAnimationFrameGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MonLookUpEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MirActImageComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.effectfileComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ActSoundComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EffectdirComboBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl MonAnimationFrameGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView MonAnimationFrameGridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit MonLookUpEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox MirActImageComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox effectfileComboBox;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.Columns.GridColumn MagicDir;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox EffectdirComboBox;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn16;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox ActSoundComboBox;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
    }
}