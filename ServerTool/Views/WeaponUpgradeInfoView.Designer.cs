namespace Server.Views
{
    partial class WeaponUpgradeInfoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeaponUpgradeInfoView));
            this.WeaponUpgradeInfoGridControl = new DevExpress.XtraGrid.GridControl();
            this.WeaponUpgradeInfoGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.Increment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BlackIronOre = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SpendGold = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BasicFragment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.AdvanceFragment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SeniorFragment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RefinementStone = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SuccessRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.FailureRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TimeCost = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponUpgradeInfoGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponUpgradeInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // WeaponUpgradeInfoGridControl
            // 
            this.WeaponUpgradeInfoGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WeaponUpgradeInfoGridControl.Location = new System.Drawing.Point(0, 147);
            this.WeaponUpgradeInfoGridControl.MainView = this.WeaponUpgradeInfoGridView;
            this.WeaponUpgradeInfoGridControl.MenuManager = this.ribbon;
            this.WeaponUpgradeInfoGridControl.Name = "WeaponUpgradeInfoGridControl";
            this.WeaponUpgradeInfoGridControl.Size = new System.Drawing.Size(1009, 391);
            this.WeaponUpgradeInfoGridControl.TabIndex = 0;
            this.WeaponUpgradeInfoGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.WeaponUpgradeInfoGridView});
            // 
            // WeaponUpgradeInfoGridView
            // 
            this.WeaponUpgradeInfoGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Increment,
            this.BlackIronOre,
            this.SpendGold,
            this.BasicFragment,
            this.AdvanceFragment,
            this.SeniorFragment,
            this.RefinementStone,
            this.SuccessRate,
            this.FailureRate,
            this.TimeCost});
            this.WeaponUpgradeInfoGridView.DetailHeight = 377;
            this.WeaponUpgradeInfoGridView.GridControl = this.WeaponUpgradeInfoGridControl;
            this.WeaponUpgradeInfoGridView.Name = "WeaponUpgradeInfoGridView";
            this.WeaponUpgradeInfoGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.WeaponUpgradeInfoGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.WeaponUpgradeInfoGridView.OptionsView.EnableAppearanceOddRow = true;
            this.WeaponUpgradeInfoGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.WeaponUpgradeInfoGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.WeaponUpgradeInfoGridView.OptionsView.ShowGroupPanel = false;
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
            this.ribbon.Size = new System.Drawing.Size(1009, 147);
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
            // Increment
            // 
            this.Increment.Caption = "增加数值";
            this.Increment.FieldName = "Increment";
            this.Increment.Name = "Increment";
            this.Increment.Visible = true;
            this.Increment.VisibleIndex = 0;
            // 
            // BlackIronOre
            // 
            this.BlackIronOre.Caption = "黑铁数量";
            this.BlackIronOre.FieldName = "BlackIronOre";
            this.BlackIronOre.Name = "BlackIronOre";
            this.BlackIronOre.Visible = true;
            this.BlackIronOre.VisibleIndex = 1;
            // 
            // SpendGold
            // 
            this.SpendGold.Caption = "花费金币";
            this.SpendGold.FieldName = "SpendGold";
            this.SpendGold.Name = "SpendGold";
            this.SpendGold.Visible = true;
            this.SpendGold.VisibleIndex = 2;
            // 
            // BasicFragment
            // 
            this.BasicFragment.Caption = "初级碎片";
            this.BasicFragment.FieldName = "BasicFragment";
            this.BasicFragment.Name = "BasicFragment";
            this.BasicFragment.Visible = true;
            this.BasicFragment.VisibleIndex = 3;
            // 
            // AdvanceFragment
            // 
            this.AdvanceFragment.Caption = "中级碎片";
            this.AdvanceFragment.FieldName = "AdvanceFragment";
            this.AdvanceFragment.Name = "AdvanceFragment";
            this.AdvanceFragment.Visible = true;
            this.AdvanceFragment.VisibleIndex = 4;
            // 
            // SeniorFragment
            // 
            this.SeniorFragment.Caption = "高级碎片";
            this.SeniorFragment.FieldName = "SeniorFragment";
            this.SeniorFragment.Name = "SeniorFragment";
            this.SeniorFragment.Visible = true;
            this.SeniorFragment.VisibleIndex = 5;
            // 
            // RefinementStone
            // 
            this.RefinementStone.Caption = "制练石";
            this.RefinementStone.FieldName = "RefinementStone";
            this.RefinementStone.Name = "RefinementStone";
            this.RefinementStone.Visible = true;
            this.RefinementStone.VisibleIndex = 6;
            // 
            // SuccessRate
            // 
            this.SuccessRate.Caption = "成功率";
            this.SuccessRate.FieldName = "SuccessRate";
            this.SuccessRate.Name = "SuccessRate";
            this.SuccessRate.Visible = true;
            this.SuccessRate.VisibleIndex = 7;
            // 
            // FailureRate
            // 
            this.FailureRate.Caption = "失败率";
            this.FailureRate.FieldName = "FailureRate";
            this.FailureRate.Name = "FailureRate";
            this.FailureRate.Visible = true;
            this.FailureRate.VisibleIndex = 8;
            // 
            // TimeCost
            // 
            this.TimeCost.Caption = "所需时间";
            this.TimeCost.FieldName = "TimeCost";
            this.TimeCost.Name = "TimeCost";
            this.TimeCost.Visible = true;
            this.TimeCost.VisibleIndex = 9;
            // 
            // WeaponUpgradeInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 538);
            this.Controls.Add(this.WeaponUpgradeInfoGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "WeaponUpgradeInfoView";
            this.Ribbon = this.ribbon;
            this.Text = "新版武器升级";
            ((System.ComponentModel.ISupportInitialize)(this.WeaponUpgradeInfoGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WeaponUpgradeInfoGridView)).EndInit();
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
        private DevExpress.XtraGrid.GridControl WeaponUpgradeInfoGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView WeaponUpgradeInfoGridView;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn Increment;
        private DevExpress.XtraGrid.Columns.GridColumn BlackIronOre;
        private DevExpress.XtraGrid.Columns.GridColumn SpendGold;
        private DevExpress.XtraGrid.Columns.GridColumn BasicFragment;
        private DevExpress.XtraGrid.Columns.GridColumn AdvanceFragment;
        private DevExpress.XtraGrid.Columns.GridColumn SeniorFragment;
        private DevExpress.XtraGrid.Columns.GridColumn RefinementStone;
        private DevExpress.XtraGrid.Columns.GridColumn SuccessRate;
        private DevExpress.XtraGrid.Columns.GridColumn FailureRate;
        private DevExpress.XtraGrid.Columns.GridColumn TimeCost;
    }
}