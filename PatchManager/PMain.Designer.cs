namespace PatchManager
{
    partial class PMain
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
        private void InitializeComponent()
        {
            this.pcClientPathLabel = new System.Windows.Forms.Label();
            this.pcClientPathTextBox = new System.Windows.Forms.TextBox();
            this.pcFtpHostLabel = new System.Windows.Forms.Label();
            this.pcFtpAccountTextBox = new System.Windows.Forms.TextBox();
            this.pcFtpUseLoginCheckBox = new System.Windows.Forms.CheckBox();
            this.pcFtpUseLoginLabel = new System.Windows.Forms.Label();
            this.pcFtpAccountLabel = new System.Windows.Forms.Label();
            this.pcFtpPassLabel = new System.Windows.Forms.Label();
            this.pcFtpHostTextBox = new System.Windows.Forms.TextBox();
            this.pcFtpPassTextBox = new System.Windows.Forms.TextBox();
            this.pcUploadPatchButton = new System.Windows.Forms.Button();
            this.labelControl6 = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.labelControl8 = new System.Windows.Forms.Label();
            this.UploadSizeLabel = new System.Windows.Forms.Label();
            this.TotalProgressBar = new System.Windows.Forms.ProgressBar();
            this.UploadSpeedLabel = new System.Windows.Forms.Label();
            this.labelControl10 = new System.Windows.Forms.Label();
            this.FolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.mobileLabel = new System.Windows.Forms.Label();
            this.mobileCheckBox = new System.Windows.Forms.CheckBox();
            this.apkLabel = new System.Windows.Forms.Label();
            this.apkVersionLabel = new System.Windows.Forms.Label();
            this.apkVersionValueLabel = new System.Windows.Forms.Label();
            this.apkFIleTextBox = new System.Windows.Forms.TextBox();
            this.openAPKButton = new System.Windows.Forms.Button();
            this.mobFtpPassTextBox = new System.Windows.Forms.TextBox();
            this.mobFtpHostTextBox = new System.Windows.Forms.TextBox();
            this.mobFtpPassLabel = new System.Windows.Forms.Label();
            this.mobFtpAccountLabel = new System.Windows.Forms.Label();
            this.mobFtpUseLoginLabel = new System.Windows.Forms.Label();
            this.mobFtpUseLoginCheckBox = new System.Windows.Forms.CheckBox();
            this.mobFtpAccountTextBox = new System.Windows.Forms.TextBox();
            this.mobFtpHostLabel = new System.Windows.Forms.Label();
            this.mobClientPathTextBox = new System.Windows.Forms.TextBox();
            this.mobClientPathLabel = new System.Windows.Forms.Label();
            this.mobUploadPatchButton = new System.Windows.Forms.Button();
            this.openBaseZipButton = new System.Windows.Forms.Button();
            this.baseZipTextBox = new System.Windows.Forms.TextBox();
            this.baseZipLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pcClientPathLabel
            // 
            this.pcClientPathLabel.AutoEllipsis = true;
            this.pcClientPathLabel.AutoSize = true;
            this.pcClientPathLabel.Location = new System.Drawing.Point(10, 14);
            this.pcClientPathLabel.Name = "pcClientPathLabel";
            this.pcClientPathLabel.Size = new System.Drawing.Size(83, 12);
            this.pcClientPathLabel.TabIndex = 0;
            this.pcClientPathLabel.Text = "端游补丁目录:";
            // 
            // pcClientPathTextBox
            // 
            this.pcClientPathTextBox.Location = new System.Drawing.Point(99, 11);
            this.pcClientPathTextBox.Name = "pcClientPathTextBox";
            this.pcClientPathTextBox.Size = new System.Drawing.Size(234, 21);
            this.pcClientPathTextBox.TabIndex = 1;
            this.pcClientPathTextBox.TextChanged += new System.EventHandler(this.pcClientPathTextBox_TextChanged);
            // 
            // pcFtpHostLabel
            // 
            this.pcFtpHostLabel.AutoEllipsis = true;
            this.pcFtpHostLabel.AutoSize = true;
            this.pcFtpHostLabel.Location = new System.Drawing.Point(40, 41);
            this.pcFtpHostLabel.Name = "pcFtpHostLabel";
            this.pcFtpHostLabel.Size = new System.Drawing.Size(53, 12);
            this.pcFtpHostLabel.TabIndex = 2;
            this.pcFtpHostLabel.Text = "FTP主机:";
            // 
            // pcFtpAccountTextBox
            // 
            this.pcFtpAccountTextBox.Location = new System.Drawing.Point(99, 85);
            this.pcFtpAccountTextBox.Name = "pcFtpAccountTextBox";
            this.pcFtpAccountTextBox.Size = new System.Drawing.Size(234, 21);
            this.pcFtpAccountTextBox.TabIndex = 3;
            this.pcFtpAccountTextBox.TextChanged += new System.EventHandler(this.pcFtpAccountTextBox_TextChanged);
            // 
            // pcFtpUseLoginCheckBox
            // 
            this.pcFtpUseLoginCheckBox.AutoSize = true;
            this.pcFtpUseLoginCheckBox.Location = new System.Drawing.Point(99, 65);
            this.pcFtpUseLoginCheckBox.Name = "pcFtpUseLoginCheckBox";
            this.pcFtpUseLoginCheckBox.Size = new System.Drawing.Size(15, 14);
            this.pcFtpUseLoginCheckBox.TabIndex = 4;
            this.pcFtpUseLoginCheckBox.CheckedChanged += new System.EventHandler(this.pcFtpUseLoginCheckBox_CheckedChanged);
            // 
            // pcFtpUseLoginLabel
            // 
            this.pcFtpUseLoginLabel.AutoEllipsis = true;
            this.pcFtpUseLoginLabel.AutoSize = true;
            this.pcFtpUseLoginLabel.Location = new System.Drawing.Point(40, 64);
            this.pcFtpUseLoginLabel.Name = "pcFtpUseLoginLabel";
            this.pcFtpUseLoginLabel.Size = new System.Drawing.Size(53, 12);
            this.pcFtpUseLoginLabel.TabIndex = 5;
            this.pcFtpUseLoginLabel.Text = "FTP验证:";
            // 
            // pcFtpAccountLabel
            // 
            this.pcFtpAccountLabel.AutoEllipsis = true;
            this.pcFtpAccountLabel.AutoSize = true;
            this.pcFtpAccountLabel.Location = new System.Drawing.Point(40, 88);
            this.pcFtpAccountLabel.Name = "pcFtpAccountLabel";
            this.pcFtpAccountLabel.Size = new System.Drawing.Size(53, 12);
            this.pcFtpAccountLabel.TabIndex = 6;
            this.pcFtpAccountLabel.Text = "FTP账号:";
            // 
            // pcFtpPassLabel
            // 
            this.pcFtpPassLabel.AutoEllipsis = true;
            this.pcFtpPassLabel.AutoSize = true;
            this.pcFtpPassLabel.Location = new System.Drawing.Point(40, 115);
            this.pcFtpPassLabel.Name = "pcFtpPassLabel";
            this.pcFtpPassLabel.Size = new System.Drawing.Size(53, 12);
            this.pcFtpPassLabel.TabIndex = 7;
            this.pcFtpPassLabel.Text = "FTP密码:";
            // 
            // pcFtpHostTextBox
            // 
            this.pcFtpHostTextBox.Location = new System.Drawing.Point(99, 38);
            this.pcFtpHostTextBox.Name = "pcFtpHostTextBox";
            this.pcFtpHostTextBox.Size = new System.Drawing.Size(234, 21);
            this.pcFtpHostTextBox.TabIndex = 8;
            this.pcFtpHostTextBox.TextChanged += new System.EventHandler(this.pcFtpHostTextBox_TextChanged);
            // 
            // pcFtpPassTextBox
            // 
            this.pcFtpPassTextBox.Location = new System.Drawing.Point(99, 112);
            this.pcFtpPassTextBox.Name = "pcFtpPassTextBox";
            this.pcFtpPassTextBox.PasswordChar = '*';
            this.pcFtpPassTextBox.Size = new System.Drawing.Size(234, 21);
            this.pcFtpPassTextBox.TabIndex = 9;
            this.pcFtpPassTextBox.TextChanged += new System.EventHandler(this.pcFtpPassTextBox_TextChanged);
            // 
            // pcUploadPatchButton
            // 
            this.pcUploadPatchButton.Location = new System.Drawing.Point(100, 279);
            this.pcUploadPatchButton.Name = "pcUploadPatchButton";
            this.pcUploadPatchButton.Size = new System.Drawing.Size(233, 27);
            this.pcUploadPatchButton.TabIndex = 10;
            this.pcUploadPatchButton.Text = "上传端游补丁";
            this.pcUploadPatchButton.Click += new System.EventHandler(this.pcUploadPatchButton_Click);
            // 
            // labelControl6
            // 
            this.labelControl6.AutoEllipsis = true;
            this.labelControl6.AutoSize = true;
            this.labelControl6.Location = new System.Drawing.Point(193, 327);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(35, 12);
            this.labelControl6.TabIndex = 11;
            this.labelControl6.Text = "状态:";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoEllipsis = true;
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(234, 327);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(29, 12);
            this.StatusLabel.TabIndex = 12;
            this.StatusLabel.Text = "<空>";
            // 
            // labelControl8
            // 
            this.labelControl8.AutoEllipsis = true;
            this.labelControl8.AutoSize = true;
            this.labelControl8.Location = new System.Drawing.Point(191, 344);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(35, 12);
            this.labelControl8.TabIndex = 13;
            this.labelControl8.Text = "上传:";
            // 
            // UploadSizeLabel
            // 
            this.UploadSizeLabel.AutoEllipsis = true;
            this.UploadSizeLabel.AutoSize = true;
            this.UploadSizeLabel.Location = new System.Drawing.Point(234, 344);
            this.UploadSizeLabel.Name = "UploadSizeLabel";
            this.UploadSizeLabel.Size = new System.Drawing.Size(29, 12);
            this.UploadSizeLabel.TabIndex = 14;
            this.UploadSizeLabel.Text = "<空>";
            // 
            // TotalProgressBar
            // 
            this.TotalProgressBar.Location = new System.Drawing.Point(191, 362);
            this.TotalProgressBar.Name = "TotalProgressBar";
            this.TotalProgressBar.Size = new System.Drawing.Size(309, 16);
            this.TotalProgressBar.TabIndex = 15;
            // 
            // UploadSpeedLabel
            // 
            this.UploadSpeedLabel.AutoEllipsis = true;
            this.UploadSpeedLabel.AutoSize = true;
            this.UploadSpeedLabel.Location = new System.Drawing.Point(441, 344);
            this.UploadSpeedLabel.Name = "UploadSpeedLabel";
            this.UploadSpeedLabel.Size = new System.Drawing.Size(29, 12);
            this.UploadSpeedLabel.TabIndex = 17;
            this.UploadSpeedLabel.Text = "<空>";
            // 
            // labelControl10
            // 
            this.labelControl10.AutoEllipsis = true;
            this.labelControl10.AutoSize = true;
            this.labelControl10.Location = new System.Drawing.Point(401, 344);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(35, 12);
            this.labelControl10.TabIndex = 16;
            this.labelControl10.Text = "速度:";
            // 
            // mobileLabel
            // 
            this.mobileLabel.AutoSize = true;
            this.mobileLabel.Location = new System.Drawing.Point(387, 149);
            this.mobileLabel.Name = "mobileLabel";
            this.mobileLabel.Size = new System.Drawing.Size(35, 12);
            this.mobileLabel.TabIndex = 19;
            this.mobileLabel.Text = "手游:";
            // 
            // mobileCheckBox
            // 
            this.mobileCheckBox.AutoSize = true;
            this.mobileCheckBox.Location = new System.Drawing.Point(428, 147);
            this.mobileCheckBox.Name = "mobileCheckBox";
            this.mobileCheckBox.Size = new System.Drawing.Size(15, 14);
            this.mobileCheckBox.TabIndex = 18;
            this.mobileCheckBox.CheckedChanged += new System.EventHandler(this.mobileCheckBox_CheckedChanged);
            // 
            // apkLabel
            // 
            this.apkLabel.AutoEllipsis = true;
            this.apkLabel.AutoSize = true;
            this.apkLabel.Location = new System.Drawing.Point(393, 171);
            this.apkLabel.Name = "apkLabel";
            this.apkLabel.Size = new System.Drawing.Size(29, 12);
            this.apkLabel.TabIndex = 20;
            this.apkLabel.Text = "APK:";
            // 
            // apkVersionLabel
            // 
            this.apkVersionLabel.AutoEllipsis = true;
            this.apkVersionLabel.AutoSize = true;
            this.apkVersionLabel.Location = new System.Drawing.Point(369, 199);
            this.apkVersionLabel.Name = "apkVersionLabel";
            this.apkVersionLabel.Size = new System.Drawing.Size(53, 12);
            this.apkVersionLabel.TabIndex = 23;
            this.apkVersionLabel.Text = "APK版本:";
            // 
            // apkVersionValueLabel
            // 
            this.apkVersionValueLabel.AutoEllipsis = true;
            this.apkVersionValueLabel.AutoSize = true;
            this.apkVersionValueLabel.Location = new System.Drawing.Point(426, 199);
            this.apkVersionValueLabel.Name = "apkVersionValueLabel";
            this.apkVersionValueLabel.Size = new System.Drawing.Size(29, 12);
            this.apkVersionValueLabel.TabIndex = 24;
            this.apkVersionValueLabel.Text = "<空>";
            // 
            // apkFIleTextBox
            // 
            this.apkFIleTextBox.Enabled = false;
            this.apkFIleTextBox.Location = new System.Drawing.Point(428, 167);
            this.apkFIleTextBox.Name = "apkFIleTextBox";
            this.apkFIleTextBox.Size = new System.Drawing.Size(202, 21);
            this.apkFIleTextBox.TabIndex = 21;
            // 
            // openAPKButton
            // 
            this.openAPKButton.BackColor = System.Drawing.Color.LightGray;
            this.openAPKButton.Location = new System.Drawing.Point(631, 167);
            this.openAPKButton.Name = "openAPKButton";
            this.openAPKButton.Size = new System.Drawing.Size(31, 21);
            this.openAPKButton.TabIndex = 22;
            this.openAPKButton.Text = "...";
            this.openAPKButton.UseVisualStyleBackColor = false;
            this.openAPKButton.Click += new System.EventHandler(this.openAPKButton_Click);
            // 
            // mobFtpPassTextBox
            // 
            this.mobFtpPassTextBox.Location = new System.Drawing.Point(428, 111);
            this.mobFtpPassTextBox.Name = "mobFtpPassTextBox";
            this.mobFtpPassTextBox.PasswordChar = '*';
            this.mobFtpPassTextBox.Size = new System.Drawing.Size(234, 21);
            this.mobFtpPassTextBox.TabIndex = 34;
            this.mobFtpPassTextBox.TextChanged += new System.EventHandler(this.mobFtpPassTextBox_TextChanged);
            // 
            // mobFtpHostTextBox
            // 
            this.mobFtpHostTextBox.Location = new System.Drawing.Point(428, 37);
            this.mobFtpHostTextBox.Name = "mobFtpHostTextBox";
            this.mobFtpHostTextBox.Size = new System.Drawing.Size(234, 21);
            this.mobFtpHostTextBox.TabIndex = 33;
            this.mobFtpHostTextBox.TextChanged += new System.EventHandler(this.mobFtpHostTextBox_TextChanged);
            // 
            // mobFtpPassLabel
            // 
            this.mobFtpPassLabel.AutoEllipsis = true;
            this.mobFtpPassLabel.AutoSize = true;
            this.mobFtpPassLabel.Location = new System.Drawing.Point(369, 115);
            this.mobFtpPassLabel.Name = "mobFtpPassLabel";
            this.mobFtpPassLabel.Size = new System.Drawing.Size(53, 12);
            this.mobFtpPassLabel.TabIndex = 32;
            this.mobFtpPassLabel.Text = "FTP密码:";
            // 
            // mobFtpAccountLabel
            // 
            this.mobFtpAccountLabel.AutoEllipsis = true;
            this.mobFtpAccountLabel.AutoSize = true;
            this.mobFtpAccountLabel.Location = new System.Drawing.Point(369, 88);
            this.mobFtpAccountLabel.Name = "mobFtpAccountLabel";
            this.mobFtpAccountLabel.Size = new System.Drawing.Size(53, 12);
            this.mobFtpAccountLabel.TabIndex = 31;
            this.mobFtpAccountLabel.Text = "FTP账号:";
            // 
            // mobFtpUseLoginLabel
            // 
            this.mobFtpUseLoginLabel.AutoEllipsis = true;
            this.mobFtpUseLoginLabel.AutoSize = true;
            this.mobFtpUseLoginLabel.Location = new System.Drawing.Point(369, 64);
            this.mobFtpUseLoginLabel.Name = "mobFtpUseLoginLabel";
            this.mobFtpUseLoginLabel.Size = new System.Drawing.Size(53, 12);
            this.mobFtpUseLoginLabel.TabIndex = 30;
            this.mobFtpUseLoginLabel.Text = "FTP验证:";
            // 
            // mobFtpUseLoginCheckBox
            // 
            this.mobFtpUseLoginCheckBox.AutoSize = true;
            this.mobFtpUseLoginCheckBox.Location = new System.Drawing.Point(428, 64);
            this.mobFtpUseLoginCheckBox.Name = "mobFtpUseLoginCheckBox";
            this.mobFtpUseLoginCheckBox.Size = new System.Drawing.Size(15, 14);
            this.mobFtpUseLoginCheckBox.TabIndex = 29;
            this.mobFtpUseLoginCheckBox.CheckedChanged += new System.EventHandler(this.mobFtpUseLoginCheckBox_CheckedChanged);
            // 
            // mobFtpAccountTextBox
            // 
            this.mobFtpAccountTextBox.Location = new System.Drawing.Point(428, 84);
            this.mobFtpAccountTextBox.Name = "mobFtpAccountTextBox";
            this.mobFtpAccountTextBox.Size = new System.Drawing.Size(234, 21);
            this.mobFtpAccountTextBox.TabIndex = 28;
            this.mobFtpAccountTextBox.TextChanged += new System.EventHandler(this.mobFtpAccountTextBox_TextChanged);
            // 
            // mobFtpHostLabel
            // 
            this.mobFtpHostLabel.AutoEllipsis = true;
            this.mobFtpHostLabel.AutoSize = true;
            this.mobFtpHostLabel.Location = new System.Drawing.Point(369, 41);
            this.mobFtpHostLabel.Name = "mobFtpHostLabel";
            this.mobFtpHostLabel.Size = new System.Drawing.Size(53, 12);
            this.mobFtpHostLabel.TabIndex = 27;
            this.mobFtpHostLabel.Text = "FTP主机:";
            // 
            // mobClientPathTextBox
            // 
            this.mobClientPathTextBox.Location = new System.Drawing.Point(428, 10);
            this.mobClientPathTextBox.Name = "mobClientPathTextBox";
            this.mobClientPathTextBox.Size = new System.Drawing.Size(234, 21);
            this.mobClientPathTextBox.TabIndex = 26;
            this.mobClientPathTextBox.TextChanged += new System.EventHandler(this.mobClientPathTextBox_TextChanged);
            // 
            // mobClientPathLabel
            // 
            this.mobClientPathLabel.AutoEllipsis = true;
            this.mobClientPathLabel.AutoSize = true;
            this.mobClientPathLabel.Location = new System.Drawing.Point(339, 14);
            this.mobClientPathLabel.Name = "mobClientPathLabel";
            this.mobClientPathLabel.Size = new System.Drawing.Size(83, 12);
            this.mobClientPathLabel.TabIndex = 25;
            this.mobClientPathLabel.Text = "手游补丁目录:";
            // 
            // mobUploadPatchButton
            // 
            this.mobUploadPatchButton.Location = new System.Drawing.Point(429, 279);
            this.mobUploadPatchButton.Name = "mobUploadPatchButton";
            this.mobUploadPatchButton.Size = new System.Drawing.Size(233, 27);
            this.mobUploadPatchButton.TabIndex = 35;
            this.mobUploadPatchButton.Text = "上传手游补丁";
            this.mobUploadPatchButton.Click += new System.EventHandler(this.mobUploadPatchButton_Click);
            // 
            // openBaseZipButton
            // 
            this.openBaseZipButton.BackColor = System.Drawing.Color.LightGray;
            this.openBaseZipButton.Location = new System.Drawing.Point(631, 219);
            this.openBaseZipButton.Name = "openBaseZipButton";
            this.openBaseZipButton.Size = new System.Drawing.Size(31, 21);
            this.openBaseZipButton.TabIndex = 38;
            this.openBaseZipButton.Text = "...";
            this.openBaseZipButton.UseVisualStyleBackColor = false;
            this.openBaseZipButton.Click += new System.EventHandler(this.openBaseZipButton_Click);
            // 
            // baseZipTextBox
            // 
            this.baseZipTextBox.Enabled = false;
            this.baseZipTextBox.Location = new System.Drawing.Point(428, 219);
            this.baseZipTextBox.Name = "baseZipTextBox";
            this.baseZipTextBox.Size = new System.Drawing.Size(202, 21);
            this.baseZipTextBox.TabIndex = 37;
            // 
            // baseZipLabel
            // 
            this.baseZipLabel.AutoEllipsis = true;
            this.baseZipLabel.AutoSize = true;
            this.baseZipLabel.Location = new System.Drawing.Point(375, 223);
            this.baseZipLabel.Name = "baseZipLabel";
            this.baseZipLabel.Size = new System.Drawing.Size(47, 12);
            this.baseZipLabel.TabIndex = 36;
            this.baseZipLabel.Text = "基础包:";
            // 
            // PMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 391);
            this.Controls.Add(this.openBaseZipButton);
            this.Controls.Add(this.baseZipTextBox);
            this.Controls.Add(this.baseZipLabel);
            this.Controls.Add(this.mobUploadPatchButton);
            this.Controls.Add(this.mobFtpPassTextBox);
            this.Controls.Add(this.mobFtpHostTextBox);
            this.Controls.Add(this.mobFtpPassLabel);
            this.Controls.Add(this.mobFtpAccountLabel);
            this.Controls.Add(this.mobFtpUseLoginLabel);
            this.Controls.Add(this.mobFtpUseLoginCheckBox);
            this.Controls.Add(this.mobFtpAccountTextBox);
            this.Controls.Add(this.mobFtpHostLabel);
            this.Controls.Add(this.mobClientPathTextBox);
            this.Controls.Add(this.mobClientPathLabel);
            this.Controls.Add(this.apkVersionValueLabel);
            this.Controls.Add(this.apkVersionLabel);
            this.Controls.Add(this.openAPKButton);
            this.Controls.Add(this.apkFIleTextBox);
            this.Controls.Add(this.apkLabel);
            this.Controls.Add(this.mobileLabel);
            this.Controls.Add(this.mobileCheckBox);
            this.Controls.Add(this.UploadSpeedLabel);
            this.Controls.Add(this.labelControl10);
            this.Controls.Add(this.TotalProgressBar);
            this.Controls.Add(this.UploadSizeLabel);
            this.Controls.Add(this.labelControl8);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.labelControl6);
            this.Controls.Add(this.pcUploadPatchButton);
            this.Controls.Add(this.pcFtpPassTextBox);
            this.Controls.Add(this.pcFtpHostTextBox);
            this.Controls.Add(this.pcFtpPassLabel);
            this.Controls.Add(this.pcFtpAccountLabel);
            this.Controls.Add(this.pcFtpUseLoginLabel);
            this.Controls.Add(this.pcFtpUseLoginCheckBox);
            this.Controls.Add(this.pcFtpAccountTextBox);
            this.Controls.Add(this.pcFtpHostLabel);
            this.Controls.Add(this.pcClientPathTextBox);
            this.Controls.Add(this.pcClientPathLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "PMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "补丁管理器";
            this.Load += new System.EventHandler(this.PMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label pcClientPathLabel;
        private System.Windows.Forms.TextBox pcClientPathTextBox;
        private System.Windows.Forms.Label pcFtpHostLabel;
        private System.Windows.Forms.TextBox pcFtpAccountTextBox;
        private System.Windows.Forms.CheckBox pcFtpUseLoginCheckBox;
        private System.Windows.Forms.Label pcFtpUseLoginLabel;
        private System.Windows.Forms.Label pcFtpAccountLabel;
        private System.Windows.Forms.Label pcFtpPassLabel;
        private System.Windows.Forms.TextBox pcFtpHostTextBox;
        private System.Windows.Forms.TextBox pcFtpPassTextBox;
        private System.Windows.Forms.Button pcUploadPatchButton;
        private System.Windows.Forms.Label labelControl6;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label labelControl8;
        private System.Windows.Forms.Label UploadSizeLabel;
        private System.Windows.Forms.ProgressBar TotalProgressBar;
        private System.Windows.Forms.Label UploadSpeedLabel;
        private System.Windows.Forms.Label labelControl10;
        private System.Windows.Forms.FolderBrowserDialog FolderDialog;
        private System.Windows.Forms.Label mobileLabel;
        private System.Windows.Forms.CheckBox mobileCheckBox;
        private System.Windows.Forms.Label apkLabel;
        private System.Windows.Forms.Label apkVersionLabel;
        private System.Windows.Forms.Label apkVersionValueLabel;
        private System.Windows.Forms.TextBox apkFIleTextBox;
        private System.Windows.Forms.Button openAPKButton;
        private System.Windows.Forms.TextBox mobFtpPassTextBox;
        private System.Windows.Forms.TextBox mobFtpHostTextBox;
        private System.Windows.Forms.Label mobFtpPassLabel;
        private System.Windows.Forms.Label mobFtpAccountLabel;
        private System.Windows.Forms.Label mobFtpUseLoginLabel;
        private System.Windows.Forms.CheckBox mobFtpUseLoginCheckBox;
        private System.Windows.Forms.TextBox mobFtpAccountTextBox;
        private System.Windows.Forms.Label mobFtpHostLabel;
        private System.Windows.Forms.TextBox mobClientPathTextBox;
        private System.Windows.Forms.Label mobClientPathLabel;
        private System.Windows.Forms.Button mobUploadPatchButton;
        private System.Windows.Forms.Button openBaseZipButton;
        private System.Windows.Forms.TextBox baseZipTextBox;
        private System.Windows.Forms.Label baseZipLabel;
    }
}

