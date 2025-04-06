namespace LibraryEditor
{
    partial class LMain
    {
        /// <summary>
        /// 必需的设计器变量
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，则为true；否则为false。</param>
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
        /// 设计器支持所需的方法-不要修改
        /// 此方法的内容和代码编辑器。
        /// </summary>
        private void InitializeComponent()  //初始化组件
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LMain));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.countBlanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeBlanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.safeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDXT1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemARGB32 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWTL_1to1_ZL = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemABGR32 = new System.Windows.Forms.ToolStripMenuItem();
            this.skinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cryptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ShadowTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ShadowOffSetYTextBox = new System.Windows.Forms.TextBox();
            this.ShadowOffSetXTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.nudJump = new System.Windows.Forms.NumericUpDown();
            this.checkBoxPreventAntiAliasing = new System.Windows.Forms.CheckBox();
            this.checkBoxQuality = new System.Windows.Forms.CheckBox();
            this.buttonSkipPrevious = new System.Windows.Forms.Button();
            this.buttonSkipNext = new System.Windows.Forms.Button();
            this.ZoomTrackBar = new System.Windows.Forms.TrackBar();
            this.ExportButton = new System.Windows.Forms.Button();
            this.OffSetYTextBox = new System.Windows.Forms.TextBox();
            this.OffSetXTextBox = new System.Windows.Forms.TextBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ImportButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.PreviewListView = new CustomFormControl.FixedListView();
            this.ImageList = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.OpenLibraryDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveLibraryDialog = new System.Windows.Forms.SaveFileDialog();
            this.ImportImageDialog = new System.Windows.Forms.OpenFileDialog();
            this.OpenWeMadeDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contrayCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButtonOverlay = new System.Windows.Forms.RadioButton();
            this.radioButtonShadow = new System.Windows.Forms.RadioButton();
            this.radioButtonImage = new System.Windows.Forms.RadioButton();
            this.MemUsagetimer = new System.Windows.Forms.Timer(this.components);
            this.MainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudJump)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomTrackBar)).BeginInit();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionsToolStripMenuItem,
            this.skinToolStripMenuItem,
            this.cryptToolStripMenuItem});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(1260, 28);
            this.MainMenu.TabIndex = 0;
            this.MainMenu.Text = "主菜单";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileToolStripMenuItem.Image")));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.fileToolStripMenuItem.Text = "文件";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.newToolStripMenuItem.Text = "新建";
            this.newToolStripMenuItem.ToolTipText = "新建 .ZL";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.openToolStripMenuItem.Text = "打开";
            this.openToolStripMenuItem.ToolTipText = "打开盛大传奇3或者韩国传奇3的文件";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(134, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.saveToolStripMenuItem.Text = "存档";
            this.saveToolStripMenuItem.ToolTipText = "保存当前打开的 .ZL";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.saveAsToolStripMenuItem.Text = "另存为";
            this.saveAsToolStripMenuItem.ToolTipText = "新建 .ZL";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(134, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.closeToolStripMenuItem.Text = "关闭";
            this.closeToolStripMenuItem.ToolTipText = "退出应用程序";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // functionsToolStripMenuItem
            // 
            this.functionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToToolStripMenuItem,
            this.countBlanksToolStripMenuItem,
            this.removeBlanksToolStripMenuItem,
            this.convertToolStripMenuItem});
            this.functionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("functionsToolStripMenuItem.Image")));
            this.functionsToolStripMenuItem.Name = "functionsToolStripMenuItem";
            this.functionsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.functionsToolStripMenuItem.Text = "功能";
            // 
            // copyToToolStripMenuItem
            // 
            this.copyToToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToToolStripMenuItem.Image")));
            this.copyToToolStripMenuItem.Name = "copyToToolStripMenuItem";
            this.copyToToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.copyToToolStripMenuItem.Text = "复制到..";
            this.copyToToolStripMenuItem.ToolTipText = "复制到新的.ZL或现有.ZL的结尾";
            this.copyToToolStripMenuItem.Visible = false;
            this.copyToToolStripMenuItem.Click += new System.EventHandler(this.copyToToolStripMenuItem_Click);
            // 
            // countBlanksToolStripMenuItem
            // 
            this.countBlanksToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("countBlanksToolStripMenuItem.Image")));
            this.countBlanksToolStripMenuItem.Name = "countBlanksToolStripMenuItem";
            this.countBlanksToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.countBlanksToolStripMenuItem.Text = "插入空图";
            this.countBlanksToolStripMenuItem.ToolTipText = "插入空白图像到 .ZL";
            this.countBlanksToolStripMenuItem.Click += new System.EventHandler(this.countBlanksToolStripMenuItem_Click);
            // 
            // removeBlanksToolStripMenuItem
            // 
            this.removeBlanksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.safeToolStripMenuItem});
            this.removeBlanksToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeBlanksToolStripMenuItem.Image")));
            this.removeBlanksToolStripMenuItem.Name = "removeBlanksToolStripMenuItem";
            this.removeBlanksToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.removeBlanksToolStripMenuItem.Text = "删除空图";
            this.removeBlanksToolStripMenuItem.ToolTipText = "快速删除空图";
            this.removeBlanksToolStripMenuItem.Click += new System.EventHandler(this.removeBlanksToolStripMenuItem_Click);
            // 
            // safeToolStripMenuItem
            // 
            this.safeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("safeToolStripMenuItem.Image")));
            this.safeToolStripMenuItem.Name = "safeToolStripMenuItem";
            this.safeToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.safeToolStripMenuItem.Text = "保护";
            this.safeToolStripMenuItem.ToolTipText = "使用安全的方法清除空图";
            this.safeToolStripMenuItem.Click += new System.EventHandler(this.safeToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemDXT1,
            this.toolStripMenuItemARGB32,
            this.toolStripMenuItemWTL_1to1_ZL,
            this.toolStripMenuItemABGR32});
            this.convertToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("convertToolStripMenuItem.Image")));
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.convertToolStripMenuItem.Text = "转换素材";
            this.convertToolStripMenuItem.ToolTipText = "支持传奇2，传奇3，Zircon，水晶素材文件";
            // 
            // toolStripMenuItemDXT1
            // 
            this.toolStripMenuItemDXT1.Name = "toolStripMenuItemDXT1";
            this.toolStripMenuItemDXT1.Size = new System.Drawing.Size(261, 26);
            this.toolStripMenuItemDXT1.Text = "To Dxt1";
            this.toolStripMenuItemDXT1.ToolTipText = "转换素材到.Zl文件，图像格式DXT1";
            this.toolStripMenuItemDXT1.Click += new System.EventHandler(this.toolStripMenuItemDXT1_Click);
            // 
            // toolStripMenuItemARGB32
            // 
            this.toolStripMenuItemARGB32.Name = "toolStripMenuItemARGB32";
            this.toolStripMenuItemARGB32.Size = new System.Drawing.Size(261, 26);
            this.toolStripMenuItemARGB32.Text = "To ARGB32";
            this.toolStripMenuItemARGB32.ToolTipText = "转换素材到.Zl文件，图像格式ARGB32";
            this.toolStripMenuItemARGB32.Click += new System.EventHandler(this.toolStripMenuItemARGB32_Click);
            // 
            // toolStripMenuItemWTL_1to1_ZL
            // 
            this.toolStripMenuItemWTL_1to1_ZL.Name = "toolStripMenuItemWTL_1to1_ZL";
            this.toolStripMenuItemWTL_1to1_ZL.Size = new System.Drawing.Size(261, 26);
            this.toolStripMenuItemWTL_1to1_ZL.Text = "官方WTL 1:1 黑龙Zl";
            this.toolStripMenuItemWTL_1to1_ZL.Click += new System.EventHandler(this.toolStripMenuItemWTL_1to1_ZL_Click);
            // 
            // toolStripMenuItemABGR32
            // 
            this.toolStripMenuItemABGR32.Name = "toolStripMenuItemABGR32";
            this.toolStripMenuItemABGR32.Size = new System.Drawing.Size(261, 26);
            this.toolStripMenuItemABGR32.Text = "黑龙Zl 转换 黑龙手游端Zl";
            this.toolStripMenuItemABGR32.ToolTipText = "黑龙Zl 转换 黑龙手游端Zl";
            this.toolStripMenuItemABGR32.Click += new System.EventHandler(this.toolStripMenuItemABGR32_Click);
            // 
            // skinToolStripMenuItem
            // 
            this.skinToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.skinToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("skinToolStripMenuItem.Image")));
            this.skinToolStripMenuItem.Name = "skinToolStripMenuItem";
            this.skinToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.skinToolStripMenuItem.Text = "皮肤";
            this.skinToolStripMenuItem.Visible = false;
            // 
            // cryptToolStripMenuItem
            // 
            this.cryptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encodeToolStripMenuItem,
            this.decodeToolStripMenuItem});
            this.cryptToolStripMenuItem.Name = "cryptToolStripMenuItem";
            this.cryptToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.cryptToolStripMenuItem.Text = "加密";
            // 
            // encodeToolStripMenuItem
            // 
            this.encodeToolStripMenuItem.Name = "encodeToolStripMenuItem";
            this.encodeToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.encodeToolStripMenuItem.Text = "加密";
            this.encodeToolStripMenuItem.Click += new System.EventHandler(this.encodeToolStripMenuItem_Click);
            // 
            // decodeToolStripMenuItem
            // 
            this.decodeToolStripMenuItem.Name = "decodeToolStripMenuItem";
            this.decodeToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.decodeToolStripMenuItem.Text = "取消加密";
            this.decodeToolStripMenuItem.Click += new System.EventHandler(this.decodeToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(0, 62);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1MinSize = 300;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.PreviewListView);
            this.splitContainer1.Panel2MinSize = 150;
            this.splitContainer1.Size = new System.Drawing.Size(1260, 785);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ShadowTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.ShadowOffSetYTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.ShadowOffSetXTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.button1);
            this.splitContainer2.Panel1.Controls.Add(this.nudJump);
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxPreventAntiAliasing);
            this.splitContainer2.Panel1.Controls.Add(this.checkBoxQuality);
            this.splitContainer2.Panel1.Controls.Add(this.buttonSkipPrevious);
            this.splitContainer2.Panel1.Controls.Add(this.buttonSkipNext);
            this.splitContainer2.Panel1.Controls.Add(this.ZoomTrackBar);
            this.splitContainer2.Panel1.Controls.Add(this.ExportButton);
            this.splitContainer2.Panel1.Controls.Add(this.OffSetYTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.OffSetXTextBox);
            this.splitContainer2.Panel1.Controls.Add(this.DeleteButton);
            this.splitContainer2.Panel1.Controls.Add(this.ImportButton);
            this.splitContainer2.Panel1.Controls.Add(this.label10);
            this.splitContainer2.Panel1.Controls.Add(this.label8);
            this.splitContainer2.Panel1.Controls.Add(this.HeightLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.WidthLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.ForeColor = System.Drawing.Color.Black;
            this.splitContainer2.Panel1MinSize = 200;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel);
            this.splitContainer2.Size = new System.Drawing.Size(1260, 400);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // ShadowTextBox
            // 
            this.ShadowTextBox.Location = new System.Drawing.Point(119, 154);
            this.ShadowTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ShadowTextBox.Name = "ShadowTextBox";
            this.ShadowTextBox.Size = new System.Drawing.Size(80, 25);
            this.ShadowTextBox.TabIndex = 27;
            this.ShadowTextBox.TextChanged += new System.EventHandler(this.ShadowTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(12, 158);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 15);
            this.label4.TabIndex = 28;
            this.label4.Text = "影子:";
            // 
            // ShadowOffSetYTextBox
            // 
            this.ShadowOffSetYTextBox.Location = new System.Drawing.Point(119, 124);
            this.ShadowOffSetYTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ShadowOffSetYTextBox.Name = "ShadowOffSetYTextBox";
            this.ShadowOffSetYTextBox.Size = new System.Drawing.Size(80, 25);
            this.ShadowOffSetYTextBox.TabIndex = 24;
            this.ShadowOffSetYTextBox.TextChanged += new System.EventHandler(this.ShadowOffSetYTextBox_TextChanged);
            // 
            // ShadowOffSetXTextBox
            // 
            this.ShadowOffSetXTextBox.Location = new System.Drawing.Point(119, 94);
            this.ShadowOffSetXTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.ShadowOffSetXTextBox.Name = "ShadowOffSetXTextBox";
            this.ShadowOffSetXTextBox.Size = new System.Drawing.Size(80, 25);
            this.ShadowOffSetXTextBox.TabIndex = 23;
            this.ShadowOffSetXTextBox.TextChanged += new System.EventHandler(this.ShadowOffSetXTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(12, 128);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 15);
            this.label2.TabIndex = 26;
            this.label2.Text = "影子偏移 Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(12, 98);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 15);
            this.label3.TabIndex = 25;
            this.label3.Text = "影子偏移 X:";
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.Location = new System.Drawing.Point(4, 225);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 30);
            this.button1.TabIndex = 22;
            this.button1.Tag = "";
            this.button1.Text = "导出BMP";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ExportBMPButton_Click);
            // 
            // nudJump
            // 
            this.nudJump.Location = new System.Drawing.Point(60, 271);
            this.nudJump.Margin = new System.Windows.Forms.Padding(4);
            this.nudJump.Maximum = new decimal(new int[] {
            650000,
            0,
            0,
            0});
            this.nudJump.Name = "nudJump";
            this.nudJump.Size = new System.Drawing.Size(96, 25);
            this.nudJump.TabIndex = 21;
            this.nudJump.ValueChanged += new System.EventHandler(this.nudJump_ValueChanged);
            this.nudJump.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudJump_KeyDown);
            // 
            // checkBoxPreventAntiAliasing
            // 
            this.checkBoxPreventAntiAliasing.AutoSize = true;
            this.checkBoxPreventAntiAliasing.Location = new System.Drawing.Point(115, 365);
            this.checkBoxPreventAntiAliasing.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxPreventAntiAliasing.Name = "checkBoxPreventAntiAliasing";
            this.checkBoxPreventAntiAliasing.Size = new System.Drawing.Size(104, 19);
            this.checkBoxPreventAntiAliasing.TabIndex = 20;
            this.checkBoxPreventAntiAliasing.Text = "不消除锯齿";
            this.checkBoxPreventAntiAliasing.UseVisualStyleBackColor = true;
            this.checkBoxPreventAntiAliasing.CheckedChanged += new System.EventHandler(this.checkBoxPreventAntiAliasing_CheckedChanged);
            // 
            // checkBoxQuality
            // 
            this.checkBoxQuality.AutoSize = true;
            this.checkBoxQuality.Location = new System.Drawing.Point(10, 365);
            this.checkBoxQuality.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxQuality.Name = "checkBoxQuality";
            this.checkBoxQuality.Size = new System.Drawing.Size(74, 19);
            this.checkBoxQuality.TabIndex = 19;
            this.checkBoxQuality.Text = "不模糊";
            this.checkBoxQuality.UseVisualStyleBackColor = true;
            this.checkBoxQuality.CheckedChanged += new System.EventHandler(this.checkBoxQuality_CheckedChanged);
            // 
            // buttonSkipPrevious
            // 
            this.buttonSkipPrevious.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonSkipPrevious.Image = ((System.Drawing.Image)(resources.GetObject("buttonSkipPrevious.Image")));
            this.buttonSkipPrevious.Location = new System.Drawing.Point(16, 268);
            this.buttonSkipPrevious.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSkipPrevious.Name = "buttonSkipPrevious";
            this.buttonSkipPrevious.Size = new System.Drawing.Size(38, 30);
            this.buttonSkipPrevious.TabIndex = 17;
            this.buttonSkipPrevious.Tag = "";
            this.buttonSkipPrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonSkipPrevious.UseVisualStyleBackColor = true;
            this.buttonSkipPrevious.Click += new System.EventHandler(this.buttonSkipPrevious_Click);
            // 
            // buttonSkipNext
            // 
            this.buttonSkipNext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonSkipNext.Image = ((System.Drawing.Image)(resources.GetObject("buttonSkipNext.Image")));
            this.buttonSkipNext.Location = new System.Drawing.Point(162, 268);
            this.buttonSkipNext.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSkipNext.Name = "buttonSkipNext";
            this.buttonSkipNext.Size = new System.Drawing.Size(38, 30);
            this.buttonSkipNext.TabIndex = 16;
            this.buttonSkipNext.Tag = "";
            this.buttonSkipNext.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.buttonSkipNext.UseVisualStyleBackColor = true;
            this.buttonSkipNext.Click += new System.EventHandler(this.buttonSkipNext_Click);
            // 
            // ZoomTrackBar
            // 
            this.ZoomTrackBar.LargeChange = 1;
            this.ZoomTrackBar.Location = new System.Drawing.Point(16, 305);
            this.ZoomTrackBar.Margin = new System.Windows.Forms.Padding(4);
            this.ZoomTrackBar.Minimum = 1;
            this.ZoomTrackBar.Name = "ZoomTrackBar";
            this.ZoomTrackBar.Size = new System.Drawing.Size(184, 56);
            this.ZoomTrackBar.TabIndex = 4;
            this.ZoomTrackBar.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.ZoomTrackBar.Value = 1;
            this.ZoomTrackBar.Scroll += new System.EventHandler(this.ZoomTrackBar_Scroll);
            // 
            // ExportButton
            // 
            this.ExportButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ExportButton.Image = ((System.Drawing.Image)(resources.GetObject("ExportButton.Image")));
            this.ExportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ExportButton.Location = new System.Drawing.Point(119, 225);
            this.ExportButton.Margin = new System.Windows.Forms.Padding(4);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(98, 30);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Tag = "";
            this.ExportButton.Text = "导出PNG";
            this.ExportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportPNGButton_Click);
            // 
            // OffSetYTextBox
            // 
            this.OffSetYTextBox.Location = new System.Drawing.Point(119, 64);
            this.OffSetYTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.OffSetYTextBox.Name = "OffSetYTextBox";
            this.OffSetYTextBox.Size = new System.Drawing.Size(80, 25);
            this.OffSetYTextBox.TabIndex = 6;
            this.OffSetYTextBox.TextChanged += new System.EventHandler(this.OffSetYTextBox_TextChanged);
            // 
            // OffSetXTextBox
            // 
            this.OffSetXTextBox.Location = new System.Drawing.Point(119, 34);
            this.OffSetXTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.OffSetXTextBox.Name = "OffSetXTextBox";
            this.OffSetXTextBox.Size = new System.Drawing.Size(80, 25);
            this.OffSetXTextBox.TabIndex = 5;
            this.OffSetXTextBox.TextChanged += new System.EventHandler(this.OffSetXTextBox_TextChanged);
            // 
            // DeleteButton
            // 
            this.DeleteButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DeleteButton.Image = ((System.Drawing.Image)(resources.GetObject("DeleteButton.Image")));
            this.DeleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.DeleteButton.Location = new System.Drawing.Point(119, 188);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(98, 30);
            this.DeleteButton.TabIndex = 2;
            this.DeleteButton.Tag = "";
            this.DeleteButton.Text = "删除图像";
            this.DeleteButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ImportButton
            // 
            this.ImportButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ImportButton.Image = ((System.Drawing.Image)(resources.GetObject("ImportButton.Image")));
            this.ImportButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ImportButton.Location = new System.Drawing.Point(4, 188);
            this.ImportButton.Margin = new System.Windows.Forms.Padding(4);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(98, 30);
            this.ImportButton.TabIndex = 0;
            this.ImportButton.Tag = "";
            this.ImportButton.Text = "导入图像";
            this.ImportButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportReplace_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(12, 68);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 15);
            this.label10.TabIndex = 12;
            this.label10.Text = "偏移 Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(12, 38);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 15);
            this.label8.TabIndex = 11;
            this.label8.Text = "偏移 X:";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.HeightLabel.Location = new System.Drawing.Point(156, 9);
            this.HeightLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 15);
            this.HeightLabel.TabIndex = 10;
            this.HeightLabel.Text = "<空>";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(116, 9);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "高度:";
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.WidthLabel.Location = new System.Drawing.Point(58, 9);
            this.WidthLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(38, 15);
            this.WidthLabel.TabIndex = 8;
            this.WidthLabel.Text = "<空>";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "宽度:";
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.BackColor = System.Drawing.Color.Black;
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel.Controls.Add(this.ImageBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(4);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(1053, 398);
            this.panel.TabIndex = 1;
            // 
            // ImageBox
            // 
            this.ImageBox.BackColor = System.Drawing.Color.Transparent;
            this.ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBox.Location = new System.Drawing.Point(0, 0);
            this.ImageBox.Margin = new System.Windows.Forms.Padding(4);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(64, 64);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            // 
            // PreviewListView
            // 
            this.PreviewListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.PreviewListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PreviewListView.BackColor = System.Drawing.Color.GhostWhite;
            this.PreviewListView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(152)))), ((int)(((byte)(156)))));
            this.PreviewListView.HideSelection = false;
            this.PreviewListView.LargeImageList = this.ImageList;
            this.PreviewListView.Location = new System.Drawing.Point(0, 0);
            this.PreviewListView.Margin = new System.Windows.Forms.Padding(4);
            this.PreviewListView.Name = "PreviewListView";
            this.PreviewListView.Size = new System.Drawing.Size(1256, 375);
            this.PreviewListView.TabIndex = 0;
            this.PreviewListView.UseCompatibleStateImageBehavior = false;
            this.PreviewListView.VirtualMode = true;
            this.PreviewListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.PreviewListView_RetrieveVirtualItem);
            this.PreviewListView.SelectedIndexChanged += new System.EventHandler(this.PreviewListView_SelectedIndexChanged);
            this.PreviewListView.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.PreviewListView_VirtualItemsSelectionRangeChanged);
            // 
            // ImageList
            // 
            this.ImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ImageList.ImageSize = new System.Drawing.Size(64, 64);
            this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(12, 6);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(16, 16);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 14;
            this.pictureBox.TabStop = false;
            this.toolTip.SetToolTip(this.pictureBox, "从黑色背景切换到白色背景");
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // OpenLibraryDialog
            // 
            this.OpenLibraryDialog.Filter = "BlackDragon Library|*.Zl";
            // 
            // SaveLibraryDialog
            // 
            this.SaveLibraryDialog.Filter = "BlackDragon Library|*.Zl";
            // 
            // ImportImageDialog
            // 
            this.ImportImageDialog.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            this.ImportImageDialog.Multiselect = true;
            // 
            // OpenWeMadeDialog
            // 
            this.OpenWeMadeDialog.Filter = "WeMade|*.Wil;*.Wtl|Shanda|*.Wzl;*.Miz|Lib|*.Lib|Zircon|*.Zl|传奇正传|*.dat;*.idx";
            this.OpenWeMadeDialog.Multiselect = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1,
            this.toolStripProgressBar});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip.Location = new System.Drawing.Point(0, 850);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            this.statusStrip.Size = new System.Drawing.Size(1260, 26);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "状态栏";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Margin = new System.Windows.Forms.Padding(0, 2, 10, 2);
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(73, 22);
            this.toolStripStatusLabel.Text = "选择图像:";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Margin = new System.Windows.Forms.Padding(0, 2, 10, 2);
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(43, 22);
            this.toolStripStatusLabel2.Text = "格式:";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(4, 20);
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(250, 18);
            this.toolStripProgressBar.Step = 1;
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.contrayCheckBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.radioButtonOverlay);
            this.panel1.Controls.Add(this.radioButtonShadow);
            this.panel1.Controls.Add(this.radioButtonImage);
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 32);
            this.panel1.TabIndex = 3;
            // 
            // contrayCheckBox
            // 
            this.contrayCheckBox.AutoSize = true;
            this.contrayCheckBox.Location = new System.Drawing.Point(138, 6);
            this.contrayCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.contrayCheckBox.Name = "contrayCheckBox";
            this.contrayCheckBox.Size = new System.Drawing.Size(59, 19);
            this.contrayCheckBox.TabIndex = 19;
            this.contrayCheckBox.Text = "反色";
            this.contrayCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 8);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 15);
            this.label5.TabIndex = 15;
            this.label5.Text = "背景颜色切换";
            // 
            // radioButtonOverlay
            // 
            this.radioButtonOverlay.AutoSize = true;
            this.radioButtonOverlay.Enabled = false;
            this.radioButtonOverlay.Location = new System.Drawing.Point(425, 6);
            this.radioButtonOverlay.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonOverlay.Name = "radioButtonOverlay";
            this.radioButtonOverlay.Size = new System.Drawing.Size(58, 19);
            this.radioButtonOverlay.TabIndex = 2;
            this.radioButtonOverlay.Text = "覆盖";
            this.radioButtonOverlay.UseVisualStyleBackColor = true;
            this.radioButtonOverlay.CheckedChanged += new System.EventHandler(this.radioButtonOverlay_CheckedChanged);
            // 
            // radioButtonShadow
            // 
            this.radioButtonShadow.AutoSize = true;
            this.radioButtonShadow.Enabled = false;
            this.radioButtonShadow.Location = new System.Drawing.Point(344, 6);
            this.radioButtonShadow.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonShadow.Name = "radioButtonShadow";
            this.radioButtonShadow.Size = new System.Drawing.Size(58, 19);
            this.radioButtonShadow.TabIndex = 1;
            this.radioButtonShadow.Text = "影子";
            this.radioButtonShadow.UseVisualStyleBackColor = true;
            this.radioButtonShadow.CheckedChanged += new System.EventHandler(this.radioButtonShadow_CheckedChanged);
            // 
            // radioButtonImage
            // 
            this.radioButtonImage.AutoSize = true;
            this.radioButtonImage.Checked = true;
            this.radioButtonImage.Enabled = false;
            this.radioButtonImage.Location = new System.Drawing.Point(264, 6);
            this.radioButtonImage.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonImage.Name = "radioButtonImage";
            this.radioButtonImage.Size = new System.Drawing.Size(58, 19);
            this.radioButtonImage.TabIndex = 0;
            this.radioButtonImage.TabStop = true;
            this.radioButtonImage.Text = "图片";
            this.radioButtonImage.UseVisualStyleBackColor = true;
            this.radioButtonImage.CheckedChanged += new System.EventHandler(this.radioButtonImage_CheckedChanged);
            // 
            // MemUsagetimer
            // 
            this.MemUsagetimer.Enabled = true;
            this.MemUsagetimer.Interval = 1000;
            this.MemUsagetimer.Tick += new System.EventHandler(this.MemUsagetimer_Tick);
            // 
            // LMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1260, 876);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.MainMenu);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainMenu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(808, 509);
            this.Name = "LMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Z3专用客户端素材编辑器（技术群：361852683）";
            this.Resize += new System.EventHandler(this.LMain_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudJump)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ZoomTrackBar)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private CustomFormControl.FixedListView PreviewListView;
        private System.Windows.Forms.ImageList ImageList;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog OpenLibraryDialog;
        private System.Windows.Forms.SaveFileDialog SaveLibraryDialog;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.OpenFileDialog OpenWeMadeDialog;
        private System.Windows.Forms.ToolStripMenuItem functionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeBlanksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem countBlanksToolStripMenuItem;
        private System.Windows.Forms.TextBox OffSetYTextBox;
        private System.Windows.Forms.TextBox OffSetXTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripMenuItem safeToolStripMenuItem;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TrackBar ZoomTrackBar;
        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripMenuItem skinToolStripMenuItem;
        private System.Windows.Forms.Button buttonSkipPrevious;
        private System.Windows.Forms.Button buttonSkipNext;
        private System.Windows.Forms.CheckBox checkBoxQuality;
        private System.Windows.Forms.CheckBox checkBoxPreventAntiAliasing;
        private System.Windows.Forms.NumericUpDown nudJump;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonOverlay;
        private System.Windows.Forms.RadioButton radioButtonShadow;
        private System.Windows.Forms.RadioButton radioButtonImage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        public System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDXT1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemABGR32;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemARGB32;
        private System.Windows.Forms.Timer MemUsagetimer;
        public System.Windows.Forms.OpenFileDialog ImportImageDialog;
        private System.Windows.Forms.TextBox ShadowTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ShadowOffSetYTextBox;
        private System.Windows.Forms.TextBox ShadowOffSetXTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox contrayCheckBox;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWTL_1to1_ZL;
        private System.Windows.Forms.ToolStripMenuItem cryptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decodeToolStripMenuItem;
    }
}

