namespace GetWebSiteTool
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lbFile = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtClear = new System.Windows.Forms.Button();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lbURL = new System.Windows.Forms.Label();
            this.openFolderDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.txtNum1 = new System.Windows.Forms.TextBox();
            this.txtNum2 = new System.Windows.Forms.TextBox();
            this.txtLen = new System.Windows.Forms.TextBox();
            this.lbTo = new System.Windows.Forms.Label();
            this.lbLen = new System.Windows.Forms.Label();
            this.lbRegEx = new System.Windows.Forms.Label();
            this.txtRegEx = new System.Windows.Forms.TextBox();
            this.txtRegEx2 = new System.Windows.Forms.TextBox();
            this.lbRegEx2 = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.lbGrp1 = new System.Windows.Forms.Label();
            this.txtGrp1 = new System.Windows.Forms.TextBox();
            this.lbGrp2 = new System.Windows.Forms.Label();
            this.txtGrp2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbFile
            // 
            this.lbFile.AutoSize = true;
            this.lbFile.Location = new System.Drawing.Point(12, 50);
            this.lbFile.Name = "lbFile";
            this.lbFile.Size = new System.Drawing.Size(65, 12);
            this.lbFile.TabIndex = 0;
            this.lbFile.Text = "保存目录：";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(74, 41);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(423, 21);
            this.txtFolder.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(503, 41);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "浏览..";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(508, 147);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 48);
            this.btnGet.TabIndex = 3;
            this.btnGet.Text = "分 析";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(14, 147);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatus.Size = new System.Drawing.Size(488, 249);
            this.txtStatus.TabIndex = 4;
            // 
            // txtClear
            // 
            this.txtClear.Location = new System.Drawing.Point(508, 204);
            this.txtClear.Name = "txtClear";
            this.txtClear.Size = new System.Drawing.Size(75, 48);
            this.txtClear.TabIndex = 5;
            this.txtClear.Text = "清 空";
            this.txtClear.UseVisualStyleBackColor = true;
            this.txtClear.Click += new System.EventHandler(this.txtClear_Click);
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(74, 12);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(333, 21);
            this.txtURL.TabIndex = 7;
            // 
            // lbURL
            // 
            this.lbURL.AutoSize = true;
            this.lbURL.Location = new System.Drawing.Point(12, 21);
            this.lbURL.Name = "lbURL";
            this.lbURL.Size = new System.Drawing.Size(59, 12);
            this.lbURL.TabIndex = 6;
            this.lbURL.Text = "URL地址：";
            // 
            // txtNum1
            // 
            this.txtNum1.Location = new System.Drawing.Point(413, 12);
            this.txtNum1.Name = "txtNum1";
            this.txtNum1.Size = new System.Drawing.Size(24, 21);
            this.txtNum1.TabIndex = 10;
            this.txtNum1.Text = "0";
            // 
            // txtNum2
            // 
            this.txtNum2.Location = new System.Drawing.Point(458, 12);
            this.txtNum2.Name = "txtNum2";
            this.txtNum2.Size = new System.Drawing.Size(24, 21);
            this.txtNum2.TabIndex = 11;
            this.txtNum2.Text = "0";
            // 
            // txtLen
            // 
            this.txtLen.Location = new System.Drawing.Point(554, 14);
            this.txtLen.Name = "txtLen";
            this.txtLen.Size = new System.Drawing.Size(24, 21);
            this.txtLen.TabIndex = 12;
            this.txtLen.Text = "1";
            // 
            // lbTo
            // 
            this.lbTo.AutoSize = true;
            this.lbTo.Location = new System.Drawing.Point(441, 20);
            this.lbTo.Name = "lbTo";
            this.lbTo.Size = new System.Drawing.Size(11, 12);
            this.lbTo.TabIndex = 13;
            this.lbTo.Text = "~";
            // 
            // lbLen
            // 
            this.lbLen.AutoSize = true;
            this.lbLen.Location = new System.Drawing.Point(488, 20);
            this.lbLen.Name = "lbLen";
            this.lbLen.Size = new System.Drawing.Size(65, 12);
            this.lbLen.TabIndex = 14;
            this.lbLen.Text = "通配符长：";
            // 
            // lbRegEx
            // 
            this.lbRegEx.AutoSize = true;
            this.lbRegEx.Location = new System.Drawing.Point(12, 85);
            this.lbRegEx.Name = "lbRegEx";
            this.lbRegEx.Size = new System.Drawing.Size(77, 12);
            this.lbRegEx.TabIndex = 15;
            this.lbRegEx.Text = "正则表达式：";
            // 
            // txtRegEx
            // 
            this.txtRegEx.Location = new System.Drawing.Point(95, 78);
            this.txtRegEx.Name = "txtRegEx";
            this.txtRegEx.Size = new System.Drawing.Size(402, 21);
            this.txtRegEx.TabIndex = 16;
            // 
            // txtRegEx2
            // 
            this.txtRegEx2.Location = new System.Drawing.Point(95, 112);
            this.txtRegEx2.Name = "txtRegEx2";
            this.txtRegEx2.Size = new System.Drawing.Size(402, 21);
            this.txtRegEx2.TabIndex = 18;
            // 
            // lbRegEx2
            // 
            this.lbRegEx2.AutoSize = true;
            this.lbRegEx2.Location = new System.Drawing.Point(12, 119);
            this.lbRegEx2.Name = "lbRegEx2";
            this.lbRegEx2.Size = new System.Drawing.Size(77, 12);
            this.lbRegEx2.TabIndex = 17;
            this.lbRegEx2.Text = "正则表达式：";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(508, 270);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 44);
            this.btnCopy.TabIndex = 20;
            this.btnCopy.Text = "遍历拷贝";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lbGrp1
            // 
            this.lbGrp1.AutoSize = true;
            this.lbGrp1.Location = new System.Drawing.Point(503, 87);
            this.lbGrp1.Name = "lbGrp1";
            this.lbGrp1.Size = new System.Drawing.Size(53, 12);
            this.lbGrp1.TabIndex = 6;
            this.lbGrp1.Text = "匹配组：";
            // 
            // txtGrp1
            // 
            this.txtGrp1.Location = new System.Drawing.Point(554, 82);
            this.txtGrp1.Name = "txtGrp1";
            this.txtGrp1.Size = new System.Drawing.Size(24, 21);
            this.txtGrp1.TabIndex = 12;
            this.txtGrp1.Text = "1";
            // 
            // lbGrp2
            // 
            this.lbGrp2.AutoSize = true;
            this.lbGrp2.Location = new System.Drawing.Point(503, 115);
            this.lbGrp2.Name = "lbGrp2";
            this.lbGrp2.Size = new System.Drawing.Size(53, 12);
            this.lbGrp2.TabIndex = 6;
            this.lbGrp2.Text = "匹配组：";
            // 
            // txtGrp2
            // 
            this.txtGrp2.Location = new System.Drawing.Point(554, 110);
            this.txtGrp2.Name = "txtGrp2";
            this.txtGrp2.Size = new System.Drawing.Size(24, 21);
            this.txtGrp2.TabIndex = 12;
            this.txtGrp2.Text = "1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 413);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.txtRegEx2);
            this.Controls.Add(this.lbRegEx2);
            this.Controls.Add(this.txtRegEx);
            this.Controls.Add(this.lbRegEx);
            this.Controls.Add(this.lbLen);
            this.Controls.Add(this.lbTo);
            this.Controls.Add(this.txtGrp2);
            this.Controls.Add(this.txtGrp1);
            this.Controls.Add(this.txtLen);
            this.Controls.Add(this.txtNum2);
            this.Controls.Add(this.txtNum1);
            this.Controls.Add(this.lbGrp2);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.lbGrp1);
            this.Controls.Add(this.lbURL);
            this.Controls.Add(this.txtClear);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.lbFile);
            this.Name = "Form1";
            this.Text = "网站抓取工具（大圣）";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbFile;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button txtClear;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lbURL;
        private System.Windows.Forms.FolderBrowserDialog openFolderDlg;
        private System.Windows.Forms.TextBox txtNum1;
        private System.Windows.Forms.TextBox txtNum2;
        private System.Windows.Forms.TextBox txtLen;
        private System.Windows.Forms.Label lbTo;
        private System.Windows.Forms.Label lbLen;
        private System.Windows.Forms.Label lbRegEx;
        private System.Windows.Forms.TextBox txtRegEx;
        private System.Windows.Forms.TextBox txtRegEx2;
        private System.Windows.Forms.Label lbRegEx2;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label lbGrp1;
        private System.Windows.Forms.TextBox txtGrp1;
        private System.Windows.Forms.Label lbGrp2;
        private System.Windows.Forms.TextBox txtGrp2;
    }
}

