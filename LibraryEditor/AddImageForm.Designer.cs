namespace LibraryEditor
{
    partial class AddImageForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.ShadowTextBox = new System.Windows.Forms.TextBox();
            this.ShadowYTextBox = new System.Windows.Forms.TextBox();
            this.ShadowXTextBox = new System.Windows.Forms.TextBox();
            this.OffSetYTextBox = new System.Windows.Forms.TextBox();
            this.OffSetXTextBox = new System.Windows.Forms.TextBox();
            this.ShadowLabel = new System.Windows.Forms.Label();
            this.ShadowYLabel = new System.Windows.Forms.Label();
            this.ShadowXLabel = new System.Windows.Forms.Label();
            this.OffSetYLabel = new System.Windows.Forms.Label();
            this.OffSetXLabel = new System.Windows.Forms.Label();
            this.shadowGroupBox = new System.Windows.Forms.GroupBox();
            this.shadowRadioButtonABGR32 = new System.Windows.Forms.RadioButton();
            this.shadowPictureBox = new System.Windows.Forms.PictureBox();
            this.shadowRadioButtonDXT5 = new System.Windows.Forms.RadioButton();
            this.shadowRadioButtonDXT1 = new System.Windows.Forms.RadioButton();
            this.overlayGroupBox = new System.Windows.Forms.GroupBox();
            this.overlayRadioButtonABGR32 = new System.Windows.Forms.RadioButton();
            this.overlayPictureBox = new System.Windows.Forms.PictureBox();
            this.overlayRadioButtonDXT5 = new System.Windows.Forms.RadioButton();
            this.overlayRadioButtonDXT1 = new System.Windows.Forms.RadioButton();
            this.imageGroupBox = new System.Windows.Forms.GroupBox();
            this.imageRadioButtonARGB32 = new System.Windows.Forms.RadioButton();
            this.imageRadioButtonABGR32 = new System.Windows.Forms.RadioButton();
            this.imagePictureBox = new System.Windows.Forms.PictureBox();
            this.imageRadioButtonDXT5 = new System.Windows.Forms.RadioButton();
            this.imageRadioButtonDXT1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonAdd = new System.Windows.Forms.RadioButton();
            this.radioButtonInsert = new System.Windows.Forms.RadioButton();
            this.radioButtonReplace = new System.Windows.Forms.RadioButton();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.overlayRadioButtonARGB32 = new System.Windows.Forms.RadioButton();
            this.shadowRadioButtonARGB32 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.shadowGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowPictureBox)).BeginInit();
            this.overlayGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayPictureBox)).BeginInit();
            this.imageGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imagePictureBox)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.HeightLabel);
            this.groupBox1.Controls.Add(this.WidthLabel);
            this.groupBox1.Controls.Add(this.ShadowTextBox);
            this.groupBox1.Controls.Add(this.ShadowYTextBox);
            this.groupBox1.Controls.Add(this.ShadowXTextBox);
            this.groupBox1.Controls.Add(this.OffSetYTextBox);
            this.groupBox1.Controls.Add(this.OffSetXTextBox);
            this.groupBox1.Controls.Add(this.ShadowLabel);
            this.groupBox1.Controls.Add(this.ShadowYLabel);
            this.groupBox1.Controls.Add(this.ShadowXLabel);
            this.groupBox1.Controls.Add(this.OffSetYLabel);
            this.groupBox1.Controls.Add(this.OffSetXLabel);
            this.groupBox1.Location = new System.Drawing.Point(14, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(160, 309);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图像层：坐标";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(91, 35);
            this.HeightLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 15);
            this.HeightLabel.TabIndex = 11;
            this.HeightLabel.Text = "高: ";
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(5, 35);
            this.WidthLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(38, 15);
            this.WidthLabel.TabIndex = 10;
            this.WidthLabel.Text = "宽: ";
            // 
            // ShadowTextBox
            // 
            this.ShadowTextBox.Location = new System.Drawing.Point(94, 178);
            this.ShadowTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ShadowTextBox.Name = "ShadowTextBox";
            this.ShadowTextBox.ReadOnly = true;
            this.ShadowTextBox.Size = new System.Drawing.Size(55, 25);
            this.ShadowTextBox.TabIndex = 9;
            this.ShadowTextBox.Text = "0";
            this.ShadowTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ShadowYTextBox
            // 
            this.ShadowYTextBox.Location = new System.Drawing.Point(94, 148);
            this.ShadowYTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ShadowYTextBox.Name = "ShadowYTextBox";
            this.ShadowYTextBox.ReadOnly = true;
            this.ShadowYTextBox.Size = new System.Drawing.Size(55, 25);
            this.ShadowYTextBox.TabIndex = 8;
            this.ShadowYTextBox.Text = "0";
            this.ShadowYTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ShadowXTextBox
            // 
            this.ShadowXTextBox.Location = new System.Drawing.Point(94, 118);
            this.ShadowXTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ShadowXTextBox.Name = "ShadowXTextBox";
            this.ShadowXTextBox.ReadOnly = true;
            this.ShadowXTextBox.Size = new System.Drawing.Size(55, 25);
            this.ShadowXTextBox.TabIndex = 7;
            this.ShadowXTextBox.Text = "0";
            this.ShadowXTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // OffSetYTextBox
            // 
            this.OffSetYTextBox.Location = new System.Drawing.Point(94, 88);
            this.OffSetYTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.OffSetYTextBox.Name = "OffSetYTextBox";
            this.OffSetYTextBox.ReadOnly = true;
            this.OffSetYTextBox.Size = new System.Drawing.Size(55, 25);
            this.OffSetYTextBox.TabIndex = 6;
            this.OffSetYTextBox.Text = "0";
            this.OffSetYTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // OffSetXTextBox
            // 
            this.OffSetXTextBox.Location = new System.Drawing.Point(94, 58);
            this.OffSetXTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.OffSetXTextBox.Name = "OffSetXTextBox";
            this.OffSetXTextBox.ReadOnly = true;
            this.OffSetXTextBox.Size = new System.Drawing.Size(55, 25);
            this.OffSetXTextBox.TabIndex = 5;
            this.OffSetXTextBox.Text = "0";
            this.OffSetXTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ShadowLabel
            // 
            this.ShadowLabel.AutoSize = true;
            this.ShadowLabel.Location = new System.Drawing.Point(5, 181);
            this.ShadowLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShadowLabel.Name = "ShadowLabel";
            this.ShadowLabel.Size = new System.Drawing.Size(37, 15);
            this.ShadowLabel.TabIndex = 4;
            this.ShadowLabel.Text = "影子";
            // 
            // ShadowYLabel
            // 
            this.ShadowYLabel.AutoSize = true;
            this.ShadowYLabel.Location = new System.Drawing.Point(5, 151);
            this.ShadowYLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShadowYLabel.Name = "ShadowYLabel";
            this.ShadowYLabel.Size = new System.Drawing.Size(83, 15);
            this.ShadowYLabel.TabIndex = 3;
            this.ShadowYLabel.Text = "影子偏移 Y";
            // 
            // ShadowXLabel
            // 
            this.ShadowXLabel.AutoSize = true;
            this.ShadowXLabel.Location = new System.Drawing.Point(5, 121);
            this.ShadowXLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ShadowXLabel.Name = "ShadowXLabel";
            this.ShadowXLabel.Size = new System.Drawing.Size(83, 15);
            this.ShadowXLabel.TabIndex = 2;
            this.ShadowXLabel.Text = "影子偏移 X";
            // 
            // OffSetYLabel
            // 
            this.OffSetYLabel.AutoSize = true;
            this.OffSetYLabel.Location = new System.Drawing.Point(5, 91);
            this.OffSetYLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.OffSetYLabel.Name = "OffSetYLabel";
            this.OffSetYLabel.Size = new System.Drawing.Size(53, 15);
            this.OffSetYLabel.TabIndex = 1;
            this.OffSetYLabel.Text = "偏移 Y";
            // 
            // OffSetXLabel
            // 
            this.OffSetXLabel.AutoSize = true;
            this.OffSetXLabel.Location = new System.Drawing.Point(5, 61);
            this.OffSetXLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.OffSetXLabel.Name = "OffSetXLabel";
            this.OffSetXLabel.Size = new System.Drawing.Size(53, 15);
            this.OffSetXLabel.TabIndex = 0;
            this.OffSetXLabel.Text = "偏移 X";
            // 
            // shadowGroupBox
            // 
            this.shadowGroupBox.Controls.Add(this.shadowRadioButtonARGB32);
            this.shadowGroupBox.Controls.Add(this.shadowRadioButtonABGR32);
            this.shadowGroupBox.Controls.Add(this.shadowPictureBox);
            this.shadowGroupBox.Controls.Add(this.shadowRadioButtonDXT5);
            this.shadowGroupBox.Controls.Add(this.shadowRadioButtonDXT1);
            this.shadowGroupBox.Location = new System.Drawing.Point(721, 15);
            this.shadowGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.shadowGroupBox.Name = "shadowGroupBox";
            this.shadowGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.shadowGroupBox.Size = new System.Drawing.Size(262, 307);
            this.shadowGroupBox.TabIndex = 14;
            this.shadowGroupBox.TabStop = false;
            this.shadowGroupBox.Text = "影子层";
            this.shadowGroupBox.Visible = false;
            // 
            // shadowRadioButtonABGR32
            // 
            this.shadowRadioButtonABGR32.AutoSize = true;
            this.shadowRadioButtonABGR32.Location = new System.Drawing.Point(6, 275);
            this.shadowRadioButtonABGR32.Margin = new System.Windows.Forms.Padding(2);
            this.shadowRadioButtonABGR32.Name = "shadowRadioButtonABGR32";
            this.shadowRadioButtonABGR32.Size = new System.Drawing.Size(122, 19);
            this.shadowRadioButtonABGR32.TabIndex = 5;
            this.shadowRadioButtonABGR32.TabStop = true;
            this.shadowRadioButtonABGR32.Text = "ABGR32(手游)";
            this.shadowRadioButtonABGR32.UseVisualStyleBackColor = true;
            this.shadowRadioButtonABGR32.CheckedChanged += new System.EventHandler(this.shadowRadioButtonABGR32_CheckedChanged);
            // 
            // shadowPictureBox
            // 
            this.shadowPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shadowPictureBox.Location = new System.Drawing.Point(6, 22);
            this.shadowPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.shadowPictureBox.Name = "shadowPictureBox";
            this.shadowPictureBox.Size = new System.Drawing.Size(250, 224);
            this.shadowPictureBox.TabIndex = 12;
            this.shadowPictureBox.TabStop = false;
            // 
            // shadowRadioButtonDXT5
            // 
            this.shadowRadioButtonDXT5.AutoSize = true;
            this.shadowRadioButtonDXT5.Location = new System.Drawing.Point(70, 252);
            this.shadowRadioButtonDXT5.Margin = new System.Windows.Forms.Padding(2);
            this.shadowRadioButtonDXT5.Name = "shadowRadioButtonDXT5";
            this.shadowRadioButtonDXT5.Size = new System.Drawing.Size(60, 19);
            this.shadowRadioButtonDXT5.TabIndex = 4;
            this.shadowRadioButtonDXT5.TabStop = true;
            this.shadowRadioButtonDXT5.Text = "DXT5";
            this.shadowRadioButtonDXT5.UseVisualStyleBackColor = true;
            this.shadowRadioButtonDXT5.CheckedChanged += new System.EventHandler(this.shadowRadioButtonDXT5_CheckedChanged);
            // 
            // shadowRadioButtonDXT1
            // 
            this.shadowRadioButtonDXT1.AutoSize = true;
            this.shadowRadioButtonDXT1.Location = new System.Drawing.Point(6, 252);
            this.shadowRadioButtonDXT1.Margin = new System.Windows.Forms.Padding(2);
            this.shadowRadioButtonDXT1.Name = "shadowRadioButtonDXT1";
            this.shadowRadioButtonDXT1.Size = new System.Drawing.Size(60, 19);
            this.shadowRadioButtonDXT1.TabIndex = 3;
            this.shadowRadioButtonDXT1.TabStop = true;
            this.shadowRadioButtonDXT1.Text = "DXT1";
            this.shadowRadioButtonDXT1.UseVisualStyleBackColor = true;
            this.shadowRadioButtonDXT1.CheckedChanged += new System.EventHandler(this.shadowRadioButtonDXT1_CheckedChanged);
            // 
            // overlayGroupBox
            // 
            this.overlayGroupBox.Controls.Add(this.overlayRadioButtonARGB32);
            this.overlayGroupBox.Controls.Add(this.overlayRadioButtonABGR32);
            this.overlayGroupBox.Controls.Add(this.overlayPictureBox);
            this.overlayGroupBox.Controls.Add(this.overlayRadioButtonDXT5);
            this.overlayGroupBox.Controls.Add(this.overlayRadioButtonDXT1);
            this.overlayGroupBox.Location = new System.Drawing.Point(451, 15);
            this.overlayGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.overlayGroupBox.Name = "overlayGroupBox";
            this.overlayGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.overlayGroupBox.Size = new System.Drawing.Size(262, 307);
            this.overlayGroupBox.TabIndex = 14;
            this.overlayGroupBox.TabStop = false;
            this.overlayGroupBox.Text = "覆盖层";
            this.overlayGroupBox.Visible = false;
            // 
            // overlayRadioButtonABGR32
            // 
            this.overlayRadioButtonABGR32.AutoSize = true;
            this.overlayRadioButtonABGR32.Location = new System.Drawing.Point(6, 275);
            this.overlayRadioButtonABGR32.Margin = new System.Windows.Forms.Padding(2);
            this.overlayRadioButtonABGR32.Name = "overlayRadioButtonABGR32";
            this.overlayRadioButtonABGR32.Size = new System.Drawing.Size(122, 19);
            this.overlayRadioButtonABGR32.TabIndex = 15;
            this.overlayRadioButtonABGR32.TabStop = true;
            this.overlayRadioButtonABGR32.Text = "ABGR32(手游)";
            this.overlayRadioButtonABGR32.UseVisualStyleBackColor = true;
            this.overlayRadioButtonABGR32.CheckedChanged += new System.EventHandler(this.overlayRadioButtonABGR32_CheckedChanged);
            // 
            // overlayPictureBox
            // 
            this.overlayPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.overlayPictureBox.Location = new System.Drawing.Point(6, 22);
            this.overlayPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.overlayPictureBox.Name = "overlayPictureBox";
            this.overlayPictureBox.Size = new System.Drawing.Size(250, 224);
            this.overlayPictureBox.TabIndex = 0;
            this.overlayPictureBox.TabStop = false;
            // 
            // overlayRadioButtonDXT5
            // 
            this.overlayRadioButtonDXT5.AutoSize = true;
            this.overlayRadioButtonDXT5.Location = new System.Drawing.Point(70, 252);
            this.overlayRadioButtonDXT5.Margin = new System.Windows.Forms.Padding(2);
            this.overlayRadioButtonDXT5.Name = "overlayRadioButtonDXT5";
            this.overlayRadioButtonDXT5.Size = new System.Drawing.Size(60, 19);
            this.overlayRadioButtonDXT5.TabIndex = 14;
            this.overlayRadioButtonDXT5.TabStop = true;
            this.overlayRadioButtonDXT5.Text = "DXT5";
            this.overlayRadioButtonDXT5.UseVisualStyleBackColor = true;
            this.overlayRadioButtonDXT5.CheckedChanged += new System.EventHandler(this.overlayRadioButtonDXT5_CheckedChanged);
            // 
            // overlayRadioButtonDXT1
            // 
            this.overlayRadioButtonDXT1.AutoSize = true;
            this.overlayRadioButtonDXT1.Location = new System.Drawing.Point(6, 252);
            this.overlayRadioButtonDXT1.Margin = new System.Windows.Forms.Padding(2);
            this.overlayRadioButtonDXT1.Name = "overlayRadioButtonDXT1";
            this.overlayRadioButtonDXT1.Size = new System.Drawing.Size(60, 19);
            this.overlayRadioButtonDXT1.TabIndex = 13;
            this.overlayRadioButtonDXT1.TabStop = true;
            this.overlayRadioButtonDXT1.Text = "DXT1";
            this.overlayRadioButtonDXT1.UseVisualStyleBackColor = true;
            this.overlayRadioButtonDXT1.CheckedChanged += new System.EventHandler(this.overlayRadioButtonDXT1_CheckedChanged);
            // 
            // imageGroupBox
            // 
            this.imageGroupBox.Controls.Add(this.imageRadioButtonARGB32);
            this.imageGroupBox.Controls.Add(this.imageRadioButtonABGR32);
            this.imageGroupBox.Controls.Add(this.imagePictureBox);
            this.imageGroupBox.Controls.Add(this.imageRadioButtonDXT5);
            this.imageGroupBox.Controls.Add(this.imageRadioButtonDXT1);
            this.imageGroupBox.Location = new System.Drawing.Point(181, 15);
            this.imageGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.imageGroupBox.Name = "imageGroupBox";
            this.imageGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.imageGroupBox.Size = new System.Drawing.Size(262, 307);
            this.imageGroupBox.TabIndex = 13;
            this.imageGroupBox.TabStop = false;
            this.imageGroupBox.Text = "图像层";
            // 
            // imageRadioButtonARGB32
            // 
            this.imageRadioButtonARGB32.AutoSize = true;
            this.imageRadioButtonARGB32.Location = new System.Drawing.Point(134, 252);
            this.imageRadioButtonARGB32.Margin = new System.Windows.Forms.Padding(2);
            this.imageRadioButtonARGB32.Name = "imageRadioButtonARGB32";
            this.imageRadioButtonARGB32.Size = new System.Drawing.Size(76, 19);
            this.imageRadioButtonARGB32.TabIndex = 3;
            this.imageRadioButtonARGB32.TabStop = true;
            this.imageRadioButtonARGB32.Text = "ARGB32";
            this.imageRadioButtonARGB32.UseVisualStyleBackColor = true;
            this.imageRadioButtonARGB32.CheckedChanged += new System.EventHandler(this.imageRadioButtonARGB32_CheckedChanged);
            // 
            // imageRadioButtonABGR32
            // 
            this.imageRadioButtonABGR32.AutoSize = true;
            this.imageRadioButtonABGR32.Location = new System.Drawing.Point(6, 275);
            this.imageRadioButtonABGR32.Margin = new System.Windows.Forms.Padding(2);
            this.imageRadioButtonABGR32.Name = "imageRadioButtonABGR32";
            this.imageRadioButtonABGR32.Size = new System.Drawing.Size(122, 19);
            this.imageRadioButtonABGR32.TabIndex = 2;
            this.imageRadioButtonABGR32.TabStop = true;
            this.imageRadioButtonABGR32.Text = "ABGR32(手游)";
            this.imageRadioButtonABGR32.UseVisualStyleBackColor = true;
            this.imageRadioButtonABGR32.CheckedChanged += new System.EventHandler(this.imageRadioButtonABGR32_CheckedChanged);
            // 
            // imagePictureBox
            // 
            this.imagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imagePictureBox.Location = new System.Drawing.Point(6, 22);
            this.imagePictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.imagePictureBox.Name = "imagePictureBox";
            this.imagePictureBox.Size = new System.Drawing.Size(250, 224);
            this.imagePictureBox.TabIndex = 0;
            this.imagePictureBox.TabStop = false;
            // 
            // imageRadioButtonDXT5
            // 
            this.imageRadioButtonDXT5.AutoSize = true;
            this.imageRadioButtonDXT5.Location = new System.Drawing.Point(70, 252);
            this.imageRadioButtonDXT5.Margin = new System.Windows.Forms.Padding(2);
            this.imageRadioButtonDXT5.Name = "imageRadioButtonDXT5";
            this.imageRadioButtonDXT5.Size = new System.Drawing.Size(60, 19);
            this.imageRadioButtonDXT5.TabIndex = 1;
            this.imageRadioButtonDXT5.TabStop = true;
            this.imageRadioButtonDXT5.Text = "DXT5";
            this.imageRadioButtonDXT5.UseVisualStyleBackColor = true;
            this.imageRadioButtonDXT5.CheckedChanged += new System.EventHandler(this.imageRadioButtonDXT5_CheckedChanged);
            // 
            // imageRadioButtonDXT1
            // 
            this.imageRadioButtonDXT1.AutoSize = true;
            this.imageRadioButtonDXT1.Location = new System.Drawing.Point(6, 252);
            this.imageRadioButtonDXT1.Margin = new System.Windows.Forms.Padding(2);
            this.imageRadioButtonDXT1.Name = "imageRadioButtonDXT1";
            this.imageRadioButtonDXT1.Size = new System.Drawing.Size(60, 19);
            this.imageRadioButtonDXT1.TabIndex = 0;
            this.imageRadioButtonDXT1.TabStop = true;
            this.imageRadioButtonDXT1.Text = "DXT1";
            this.imageRadioButtonDXT1.UseVisualStyleBackColor = true;
            this.imageRadioButtonDXT1.CheckedChanged += new System.EventHandler(this.imageRadioButtonDXT1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonAdd);
            this.groupBox2.Controls.Add(this.radioButtonInsert);
            this.groupBox2.Controls.Add(this.radioButtonReplace);
            this.groupBox2.Location = new System.Drawing.Point(15, 328);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(285, 54);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "导入方式";
            // 
            // radioButtonAdd
            // 
            this.radioButtonAdd.AutoSize = true;
            this.radioButtonAdd.Location = new System.Drawing.Point(192, 24);
            this.radioButtonAdd.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonAdd.Name = "radioButtonAdd";
            this.radioButtonAdd.Size = new System.Drawing.Size(88, 19);
            this.radioButtonAdd.TabIndex = 2;
            this.radioButtonAdd.TabStop = true;
            this.radioButtonAdd.Text = "尾部追加";
            this.radioButtonAdd.UseVisualStyleBackColor = true;
            // 
            // radioButtonInsert
            // 
            this.radioButtonInsert.AutoSize = true;
            this.radioButtonInsert.Location = new System.Drawing.Point(99, 24);
            this.radioButtonInsert.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonInsert.Name = "radioButtonInsert";
            this.radioButtonInsert.Size = new System.Drawing.Size(88, 19);
            this.radioButtonInsert.TabIndex = 1;
            this.radioButtonInsert.TabStop = true;
            this.radioButtonInsert.Text = "插入图片";
            this.radioButtonInsert.UseVisualStyleBackColor = true;
            // 
            // radioButtonReplace
            // 
            this.radioButtonReplace.AutoSize = true;
            this.radioButtonReplace.Location = new System.Drawing.Point(5, 24);
            this.radioButtonReplace.Margin = new System.Windows.Forms.Padding(2);
            this.radioButtonReplace.Name = "radioButtonReplace";
            this.radioButtonReplace.Size = new System.Drawing.Size(88, 19);
            this.radioButtonReplace.TabIndex = 0;
            this.radioButtonReplace.TabStop = true;
            this.radioButtonReplace.Text = "替换图片";
            this.radioButtonReplace.UseVisualStyleBackColor = true;
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(78, 550);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(2);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(95, 38);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "确定";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // btnCancle
            // 
            this.btnCancle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancle.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancle.Location = new System.Drawing.Point(260, 550);
            this.btnCancle.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(95, 38);
            this.btnCancle.TabIndex = 4;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 498);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 29);
            this.button1.TabIndex = 7;
            this.button1.Text = "《《";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(182, 498);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(59, 29);
            this.button2.TabIndex = 8;
            this.button2.Text = "》》";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(89, 499);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(85, 25);
            this.textBox1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.OrangeRed;
            this.label1.Location = new System.Drawing.Point(16, 398);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(266, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "DXT1：有损压缩，透明度只有0跟255。";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.OrangeRed;
            this.label2.Location = new System.Drawing.Point(16, 421);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(264, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "DXT5：有损压缩，支持复杂的透明度。";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.OrangeRed;
            this.label3.Location = new System.Drawing.Point(16, 444);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(325, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "ARGB32：无压缩，无损，也就是原始位图数据。";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.OrangeRed;
            this.label4.Location = new System.Drawing.Point(16, 466);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(242, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "图像质量：ARGB32 > DXT5 > DXT1";
            // 
            // overlayRadioButtonARGB32
            // 
            this.overlayRadioButtonARGB32.AutoSize = true;
            this.overlayRadioButtonARGB32.Location = new System.Drawing.Point(134, 252);
            this.overlayRadioButtonARGB32.Margin = new System.Windows.Forms.Padding(2);
            this.overlayRadioButtonARGB32.Name = "overlayRadioButtonARGB32";
            this.overlayRadioButtonARGB32.Size = new System.Drawing.Size(76, 19);
            this.overlayRadioButtonARGB32.TabIndex = 16;
            this.overlayRadioButtonARGB32.TabStop = true;
            this.overlayRadioButtonARGB32.Text = "ARGB32";
            this.overlayRadioButtonARGB32.UseVisualStyleBackColor = true;
            this.overlayRadioButtonARGB32.CheckedChanged += new System.EventHandler(this.overlayRadioButtonARGB32_CheckedChanged);
            // 
            // shadowRadioButtonARGB32
            // 
            this.shadowRadioButtonARGB32.AutoSize = true;
            this.shadowRadioButtonARGB32.Location = new System.Drawing.Point(134, 252);
            this.shadowRadioButtonARGB32.Margin = new System.Windows.Forms.Padding(2);
            this.shadowRadioButtonARGB32.Name = "shadowRadioButtonARGB32";
            this.shadowRadioButtonARGB32.Size = new System.Drawing.Size(76, 19);
            this.shadowRadioButtonARGB32.TabIndex = 13;
            this.shadowRadioButtonARGB32.TabStop = true;
            this.shadowRadioButtonARGB32.Text = "ARGB32";
            this.shadowRadioButtonARGB32.UseVisualStyleBackColor = true;
            this.shadowRadioButtonARGB32.CheckedChanged += new System.EventHandler(this.shadowRadioButtonARGB32_CheckedChanged);
            // 
            // AddImageForm
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.CancelButton = this.btnCancle;
            this.ClientSize = new System.Drawing.Size(451, 594);
            this.Controls.Add(this.imageGroupBox);
            this.Controls.Add(this.overlayGroupBox);
            this.Controls.Add(this.shadowGroupBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "图像导入";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.shadowGroupBox.ResumeLayout(false);
            this.shadowGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shadowPictureBox)).EndInit();
            this.overlayGroupBox.ResumeLayout(false);
            this.overlayGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayPictureBox)).EndInit();
            this.imageGroupBox.ResumeLayout(false);
            this.imageGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imagePictureBox)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label ShadowLabel;
        private System.Windows.Forms.Label ShadowYLabel;
        private System.Windows.Forms.Label ShadowXLabel;
        private System.Windows.Forms.Label OffSetYLabel;
        private System.Windows.Forms.Label OffSetXLabel;
        public System.Windows.Forms.TextBox OffSetXTextBox;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancle;
        public System.Windows.Forms.TextBox ShadowTextBox;
        public System.Windows.Forms.TextBox ShadowYTextBox;
        public System.Windows.Forms.TextBox ShadowXTextBox;
        public System.Windows.Forms.TextBox OffSetYTextBox;
        public System.Windows.Forms.RadioButton radioButtonAdd;
        public System.Windows.Forms.RadioButton radioButtonInsert;
        public System.Windows.Forms.RadioButton radioButtonReplace;
        public System.Windows.Forms.RadioButton imageRadioButtonABGR32;
        public System.Windows.Forms.RadioButton imageRadioButtonDXT5;
        public System.Windows.Forms.RadioButton imageRadioButtonDXT1;
        public System.Windows.Forms.Label HeightLabel;
        public System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.PictureBox imagePictureBox;
        private System.Windows.Forms.PictureBox overlayPictureBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox shadowGroupBox;
        public System.Windows.Forms.RadioButton shadowRadioButtonABGR32;
        private System.Windows.Forms.PictureBox shadowPictureBox;
        public System.Windows.Forms.RadioButton shadowRadioButtonDXT5;
        public System.Windows.Forms.RadioButton shadowRadioButtonDXT1;
        private System.Windows.Forms.GroupBox overlayGroupBox;
        public System.Windows.Forms.RadioButton overlayRadioButtonABGR32;
        public System.Windows.Forms.RadioButton overlayRadioButtonDXT5;
        public System.Windows.Forms.RadioButton overlayRadioButtonDXT1;
        private System.Windows.Forms.GroupBox imageGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.RadioButton imageRadioButtonARGB32;
        public System.Windows.Forms.RadioButton shadowRadioButtonARGB32;
        public System.Windows.Forms.RadioButton overlayRadioButtonARGB32;
    }
}