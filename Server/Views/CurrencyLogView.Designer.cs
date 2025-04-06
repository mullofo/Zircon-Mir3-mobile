namespace Server.Views
{
    partial class CurrencyLogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrencyLogView));
            this.CurrencyLogViewGridControl = new DevExpress.XtraGrid.GridControl();
            this.CurrencyLogViewGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.typeCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.componentCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.timeCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.playerCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.currencyCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.actionCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.amountCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sourceCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.otherInfoCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.SaveButton = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.InterfaceTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CurrencyLogViewGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrencyLogViewGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // CurrencyLogViewGridControl
            // 
            this.CurrencyLogViewGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CurrencyLogViewGridControl.Location = new System.Drawing.Point(0, 147);
            this.CurrencyLogViewGridControl.MainView = this.CurrencyLogViewGridView;
            this.CurrencyLogViewGridControl.MenuManager = this.ribbon;
            this.CurrencyLogViewGridControl.Name = "CurrencyLogViewGridControl";
            this.CurrencyLogViewGridControl.Size = new System.Drawing.Size(1009, 391);
            this.CurrencyLogViewGridControl.TabIndex = 0;
            this.CurrencyLogViewGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CurrencyLogViewGridView});
            // 
            // CurrencyLogViewGridView
            // 
            this.CurrencyLogViewGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.typeCol,
            this.componentCol,
            this.timeCol,
            this.playerCol,
            this.currencyCol,
            this.actionCol,
            this.amountCol,
            this.sourceCol,
            this.otherInfoCol});
            this.CurrencyLogViewGridView.DetailHeight = 377;
            this.CurrencyLogViewGridView.GridControl = this.CurrencyLogViewGridControl;
            this.CurrencyLogViewGridView.Name = "CurrencyLogViewGridView";
            this.CurrencyLogViewGridView.OptionsBehavior.Editable = false;
            this.CurrencyLogViewGridView.OptionsDetail.AllowExpandEmptyDetails = true;
            this.CurrencyLogViewGridView.OptionsView.EnableAppearanceEvenRow = true;
            this.CurrencyLogViewGridView.OptionsView.EnableAppearanceOddRow = true;
            this.CurrencyLogViewGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
            this.CurrencyLogViewGridView.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.CurrencyLogViewGridView.OptionsView.ShowGroupPanel = false;
            // 
            // typeCol
            // 
            this.typeCol.Caption = "类型";
            this.typeCol.FieldName = "LogLevel";
            this.typeCol.Name = "typeCol";
            this.typeCol.Visible = true;
            this.typeCol.VisibleIndex = 0;
            // 
            // componentCol
            // 
            this.componentCol.Caption = "模块";
            this.componentCol.FieldName = "Component";
            this.componentCol.Name = "componentCol";
            this.componentCol.Visible = true;
            this.componentCol.VisibleIndex = 1;
            // 
            // timeCol
            // 
            this.timeCol.Caption = "时间";
            this.timeCol.DisplayFormat.FormatString = "yyyy/MM/dd hh:mm:ss";
            this.timeCol.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.timeCol.FieldName = "Time";
            this.timeCol.Name = "timeCol";
            this.timeCol.Visible = true;
            this.timeCol.VisibleIndex = 2;
            // 
            // playerCol
            // 
            this.playerCol.Caption = "玩家";
            this.playerCol.FieldName = "Character.CharacterName";
            this.playerCol.Name = "playerCol";
            this.playerCol.Visible = true;
            this.playerCol.VisibleIndex = 3;
            // 
            // currencyCol
            // 
            this.currencyCol.Caption = "货币";
            this.currencyCol.FieldName = "Currency";
            this.currencyCol.Name = "currencyCol";
            this.currencyCol.Visible = true;
            this.currencyCol.VisibleIndex = 4;
            // 
            // actionCol
            // 
            this.actionCol.Caption = "动作";
            this.actionCol.FieldName = "Action";
            this.actionCol.Name = "actionCol";
            this.actionCol.Visible = true;
            this.actionCol.VisibleIndex = 5;
            // 
            // amountCol
            // 
            this.amountCol.Caption = "数额";
            this.amountCol.FieldName = "Amount";
            this.amountCol.Name = "amountCol";
            this.amountCol.Visible = true;
            this.amountCol.VisibleIndex = 6;
            // 
            // sourceCol
            // 
            this.sourceCol.Caption = "来源";
            this.sourceCol.FieldName = "Source";
            this.sourceCol.Name = "sourceCol";
            this.sourceCol.Visible = true;
            this.sourceCol.VisibleIndex = 7;
            // 
            // otherInfoCol
            // 
            this.otherInfoCol.Caption = "其它";
            this.otherInfoCol.FieldName = "ExtraInfo";
            this.otherInfoCol.Name = "otherInfoCol";
            this.otherInfoCol.Visible = true;
            this.otherInfoCol.VisibleIndex = 8;
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
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "主页";
            // 
            // gridColumn2
            // 
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            // 
            // InterfaceTimer
            // 
            this.InterfaceTimer.Enabled = true;
            this.InterfaceTimer.Interval = 1000;
            this.InterfaceTimer.Tick += new System.EventHandler(this.InterfaceTimer_Tick);
            // 
            // CurrencyLogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 538);
            this.Controls.Add(this.CurrencyLogViewGridControl);
            this.Controls.Add(this.ribbon);
            this.Name = "CurrencyLogView";
            this.Ribbon = this.ribbon;
            this.Text = "货币日志(实时更新重启清空, 永久存储位于Log文件夹)";
            ((System.ComponentModel.ISupportInitialize)(this.CurrencyLogViewGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrencyLogViewGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.BarButtonItem SaveButton;
        private DevExpress.XtraGrid.GridControl CurrencyLogViewGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CurrencyLogViewGridView;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn typeCol;
        private DevExpress.XtraGrid.Columns.GridColumn componentCol;
        private DevExpress.XtraGrid.Columns.GridColumn timeCol;
        private DevExpress.XtraGrid.Columns.GridColumn playerCol;
        private DevExpress.XtraGrid.Columns.GridColumn actionCol;
        private DevExpress.XtraGrid.Columns.GridColumn amountCol;
        private DevExpress.XtraGrid.Columns.GridColumn currencyCol;
        private DevExpress.XtraGrid.Columns.GridColumn sourceCol;
        private DevExpress.XtraGrid.Columns.GridColumn otherInfoCol;
        private System.Windows.Forms.Timer InterfaceTimer;
    }
}