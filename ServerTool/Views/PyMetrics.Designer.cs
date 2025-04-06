namespace Server.Views
{
    partial class PyMetricsView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PyMetricsView));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.ClearLogsButton = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.PyMetricsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.functionNameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.executionCountColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.averageExecutionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.totalExecutionTimeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.maxExecutionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.maxMapObjName = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PyMetricsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.ClearLogsButton});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.MdiMergeStyle = DevExpress.XtraBars.Ribbon.RibbonMdiMergeStyle.Always;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(830, 147);
            // 
            // ClearLogsButton
            // 
            this.ClearLogsButton.Caption = "重置";
            this.ClearLogsButton.Id = 1;
            this.ClearLogsButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ClearLogsButton.ImageOptions.Image")));
            this.ClearLogsButton.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("ClearLogsButton.ImageOptions.LargeImage")));
            this.ClearLogsButton.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("ClearLogsButton.ImageOptions.SvgImage")));
            this.ClearLogsButton.LargeWidth = 50;
            this.ClearLogsButton.Name = "ClearLogsButton";
            this.ClearLogsButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ClearLogsButton_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "主页";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.AllowTextClipping = false;
            this.ribbonPageGroup1.ItemLinks.Add(this.ClearLogsButton);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "操作";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 147);
            this.gridControl1.MainView = this.PyMetricsGridView;
            this.gridControl1.MenuManager = this.ribbon;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(830, 370);
            this.gridControl1.TabIndex = 3;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.PyMetricsGridView});
            // 
            // PyMetricsGridView
            // 
            this.PyMetricsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.functionNameColumn,
            this.executionCountColumn,
            this.averageExecutionTime,
            this.totalExecutionTimeColumn,
            this.maxExecutionTime,
            this.maxMapObjName});
            this.PyMetricsGridView.GridControl = this.gridControl1;
            this.PyMetricsGridView.Name = "PyMetricsGridView";
            this.PyMetricsGridView.OptionsView.ShowGroupPanel = false;
            // 
            // functionNameColumn
            // 
            this.functionNameColumn.Caption = "函数名";
            this.functionNameColumn.FieldName = "FunctionName";
            this.functionNameColumn.Name = "functionNameColumn";
            this.functionNameColumn.OptionsColumn.AllowEdit = false;
            this.functionNameColumn.Visible = true;
            this.functionNameColumn.VisibleIndex = 0;
            // 
            // executionCountColumn
            // 
            this.executionCountColumn.Caption = "执行次数";
            this.executionCountColumn.FieldName = "ExecutionCount";
            this.executionCountColumn.Name = "executionCountColumn";
            this.executionCountColumn.Visible = true;
            this.executionCountColumn.VisibleIndex = 1;
            // 
            // averageExecutionTime
            // 
            this.averageExecutionTime.Caption = "平均耗时(毫秒)";
            this.averageExecutionTime.DisplayFormat.FormatString = "N2";
            this.averageExecutionTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.averageExecutionTime.FieldName = "averageExecutionTime";
            this.averageExecutionTime.Name = "averageExecutionTime";
            this.averageExecutionTime.UnboundExpression = "[TotalExecutionTime] / [ExecutionCount]";
            this.averageExecutionTime.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.averageExecutionTime.Visible = true;
            this.averageExecutionTime.VisibleIndex = 2;
            // 
            // totalExecutionTimeColumn
            // 
            this.totalExecutionTimeColumn.Caption = "总耗时";
            this.totalExecutionTimeColumn.FieldName = "TotalExecutionTime";
            this.totalExecutionTimeColumn.Name = "totalExecutionTimeColumn";
            // 
            // maxExecutionTime
            // 
            this.maxExecutionTime.Caption = "最大耗时(毫秒)";
            this.maxExecutionTime.DisplayFormat.FormatString = "N2";
            this.maxExecutionTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.maxExecutionTime.FieldName = "MaxExecutionTime";
            this.maxExecutionTime.Name = "maxExecutionTime";
            this.maxExecutionTime.Visible = true;
            this.maxExecutionTime.VisibleIndex = 3;
            // 
            // maxMapObjName
            // 
            this.maxMapObjName.Caption = "最大耗时地图对象";
            this.maxMapObjName.FieldName = "MaxExecutionTimeMapObjectName";
            this.maxMapObjName.Name = "maxMapObjName";
            this.maxMapObjName.ToolTip = "出现最大耗时的玩家/怪物/NPC的名字";
            this.maxMapObjName.Visible = true;
            this.maxMapObjName.VisibleIndex = 4;
            // 
            // PyMetricsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 517);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.ribbon);
            this.Name = "PyMetricsView";
            this.Ribbon = this.ribbon;
            this.Text = "Py脚本执行统计";
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PyMetricsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem ClearLogsButton;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView PyMetricsGridView;
        private DevExpress.XtraGrid.Columns.GridColumn functionNameColumn;
        private DevExpress.XtraGrid.Columns.GridColumn executionCountColumn;
        private DevExpress.XtraGrid.Columns.GridColumn averageExecutionTime;
        private DevExpress.XtraGrid.Columns.GridColumn maxExecutionTime;
        private DevExpress.XtraGrid.Columns.GridColumn maxMapObjName;
        private DevExpress.XtraGrid.Columns.GridColumn totalExecutionTimeColumn;
    }
}