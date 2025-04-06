namespace LauncherSet
{
    partial class LMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtUpdateAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBackImage = new System.Windows.Forms.PictureBox();
            this.txtPatchRemark = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPatchAdress = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSelectPicture = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtWebAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.clientStyle = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBackImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(111, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "登录器名称";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(197, 12);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(232, 21);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Text = "Z3登录器（技术群：361852683）";
            // 
            // txtUpdateAddress
            // 
            this.txtUpdateAddress.Location = new System.Drawing.Point(197, 69);
            this.txtUpdateAddress.Name = "txtUpdateAddress";
            this.txtUpdateAddress.Size = new System.Drawing.Size(232, 21);
            this.txtUpdateAddress.TabIndex = 3;
            this.txtUpdateAddress.Text = "http://";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(111, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "更新地址";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(111, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "背景图片";
            // 
            // pictureBackImage
            // 
            this.pictureBackImage.Image = global::LauncherSet.Properties.Resources.test;
            this.pictureBackImage.Location = new System.Drawing.Point(197, 162);
            this.pictureBackImage.Name = "pictureBackImage";
            this.pictureBackImage.Size = new System.Drawing.Size(232, 136);
            this.pictureBackImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBackImage.TabIndex = 5;
            this.pictureBackImage.TabStop = false;
            // 
            // txtPatchRemark
            // 
            this.txtPatchRemark.Location = new System.Drawing.Point(197, 407);
            this.txtPatchRemark.Name = "txtPatchRemark";
            this.txtPatchRemark.Size = new System.Drawing.Size(232, 21);
            this.txtPatchRemark.TabIndex = 7;
            this.txtPatchRemark.Text = "http://www.lomcn.cn";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 364);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "补丁说明文字";
            // 
            // txtPatchAdress
            // 
            this.txtPatchAdress.Location = new System.Drawing.Point(197, 361);
            this.txtPatchAdress.Name = "txtPatchAdress";
            this.txtPatchAdress.Size = new System.Drawing.Size(232, 21);
            this.txtPatchAdress.TabIndex = 9;
            this.txtPatchAdress.Text = "最新补丁说明";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 410);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "补丁链接地址";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(221, 457);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "生成登录器(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSelectPicture
            // 
            this.btnSelectPicture.Location = new System.Drawing.Point(101, 189);
            this.btnSelectPicture.Name = "btnSelectPicture";
            this.btnSelectPicture.Size = new System.Drawing.Size(75, 23);
            this.btnSelectPicture.TabIndex = 11;
            this.btnSelectPicture.Text = "选择图片";
            this.btnSelectPicture.UseVisualStyleBackColor = true;
            this.btnSelectPicture.Click += new System.EventHandler(this.btnSelectPicture_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(111, 327);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "背景链接网址";
            // 
            // txtWebAddress
            // 
            this.txtWebAddress.Location = new System.Drawing.Point(197, 324);
            this.txtWebAddress.Name = "txtWebAddress";
            this.txtWebAddress.Size = new System.Drawing.Size(232, 21);
            this.txtWebAddress.TabIndex = 13;
            this.txtWebAddress.Text = "http://www.lomcn.cn";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(111, 119);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "客户端风格";
            // 
            // clientStyle
            // 
            this.clientStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientStyle.FormattingEnabled = true;
            this.clientStyle.Items.AddRange(new object[] {
            "全部",
            "韩版",
            "1.45"});
            this.clientStyle.Location = new System.Drawing.Point(197, 116);
            this.clientStyle.Name = "clientStyle";
            this.clientStyle.Size = new System.Drawing.Size(121, 20);
            this.clientStyle.TabIndex = 14;
            // 
            // LMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 487);
            this.Controls.Add(this.clientStyle);
            this.Controls.Add(this.txtWebAddress);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnSelectPicture);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPatchAdress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPatchRemark);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBackImage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUpdateAddress);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label1);
            this.Name = "LMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Z3登录配置工具8.9版(技术群：361852683)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBackImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtUpdateAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBackImage;
        private System.Windows.Forms.TextBox txtPatchRemark;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPatchAdress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSelectPicture;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtWebAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox clientStyle;
    }
}

