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
            this.components = new System.ComponentModel.Container();
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
            this.lblThreadNum = new System.Windows.Forms.Label();
            this.txtThreadNum = new System.Windows.Forms.TextBox();
            this.chkGather2 = new System.Windows.Forms.CheckBox();
            this.chkGather = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFileExt = new System.Windows.Forms.TextBox();
            this.lbFileExt = new System.Windows.Forms.Label();
            this.chkRealExecute = new System.Windows.Forms.CheckBox();
            this.chkOverWrite = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
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
            this.txtFolder.Size = new System.Drawing.Size(333, 21);
            this.txtFolder.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(413, 39);
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
            this.lbRegEx.Text = "第一层正则：";
            // 
            // txtRegEx
            // 
            this.txtRegEx.Location = new System.Drawing.Point(95, 78);
            this.txtRegEx.Name = "txtRegEx";
            this.txtRegEx.Size = new System.Drawing.Size(312, 21);
            this.txtRegEx.TabIndex = 16;
            this.txtRegEx.TextChanged += new System.EventHandler(this.txtRegEx_TextChanged);
            // 
            // txtRegEx2
            // 
            this.txtRegEx2.Location = new System.Drawing.Point(95, 112);
            this.txtRegEx2.Name = "txtRegEx2";
            this.txtRegEx2.Size = new System.Drawing.Size(312, 21);
            this.txtRegEx2.TabIndex = 18;
            this.txtRegEx2.TextChanged += new System.EventHandler(this.txtRegEx2_TextChanged);
            // 
            // lbRegEx2
            // 
            this.lbRegEx2.AutoSize = true;
            this.lbRegEx2.Location = new System.Drawing.Point(12, 119);
            this.lbRegEx2.Name = "lbRegEx2";
            this.lbRegEx2.Size = new System.Drawing.Size(77, 12);
            this.lbRegEx2.TabIndex = 17;
            this.lbRegEx2.Text = "第二层正则：";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(5, 100);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 32);
            this.btnCopy.TabIndex = 20;
            this.btnCopy.Text = "扁平化拷贝";
            this.toolTip1.SetToolTip(this.btnCopy, "将逐层目录作为文件名的一部分，重命名并拷贝文件到单层目录下");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lbGrp1
            // 
            this.lbGrp1.AutoSize = true;
            this.lbGrp1.Location = new System.Drawing.Point(413, 87);
            this.lbGrp1.Name = "lbGrp1";
            this.lbGrp1.Size = new System.Drawing.Size(53, 12);
            this.lbGrp1.TabIndex = 6;
            this.lbGrp1.Text = "匹配组：";
            // 
            // txtGrp1
            // 
            this.txtGrp1.Location = new System.Drawing.Point(464, 82);
            this.txtGrp1.Name = "txtGrp1";
            this.txtGrp1.Size = new System.Drawing.Size(24, 21);
            this.txtGrp1.TabIndex = 12;
            this.txtGrp1.Text = "0";
            // 
            // lbGrp2
            // 
            this.lbGrp2.AutoSize = true;
            this.lbGrp2.Location = new System.Drawing.Point(413, 115);
            this.lbGrp2.Name = "lbGrp2";
            this.lbGrp2.Size = new System.Drawing.Size(53, 12);
            this.lbGrp2.TabIndex = 6;
            this.lbGrp2.Text = "匹配组：";
            // 
            // txtGrp2
            // 
            this.txtGrp2.Location = new System.Drawing.Point(464, 110);
            this.txtGrp2.Name = "txtGrp2";
            this.txtGrp2.Size = new System.Drawing.Size(24, 21);
            this.txtGrp2.TabIndex = 12;
            this.txtGrp2.Text = "0";
            // 
            // lblThreadNum
            // 
            this.lblThreadNum.AutoSize = true;
            this.lblThreadNum.Location = new System.Drawing.Point(500, 44);
            this.lblThreadNum.Name = "lblThreadNum";
            this.lblThreadNum.Size = new System.Drawing.Size(53, 12);
            this.lblThreadNum.TabIndex = 14;
            this.lblThreadNum.Text = "线程数：";
            // 
            // txtThreadNum
            // 
            this.txtThreadNum.Location = new System.Drawing.Point(554, 41);
            this.txtThreadNum.Name = "txtThreadNum";
            this.txtThreadNum.Size = new System.Drawing.Size(24, 21);
            this.txtThreadNum.TabIndex = 12;
            this.txtThreadNum.Text = "1";
            // 
            // chkGather2
            // 
            this.chkGather2.AutoSize = true;
            this.chkGather2.Location = new System.Drawing.Point(494, 112);
            this.chkGather2.Name = "chkGather2";
            this.chkGather2.Size = new System.Drawing.Size(72, 16);
            this.chkGather2.TabIndex = 22;
            this.chkGather2.Text = "抓取数据";
            this.toolTip1.SetToolTip(this.chkGather2, "抓取正则2匹配结果对应的文件，否则只打印匹配结果");
            this.chkGather2.UseVisualStyleBackColor = true;
            this.chkGather2.CheckedChanged += new System.EventHandler(this.chkGather2_CheckedChanged);
            // 
            // chkGather
            // 
            this.chkGather.AutoSize = true;
            this.chkGather.Location = new System.Drawing.Point(494, 86);
            this.chkGather.Name = "chkGather";
            this.chkGather.Size = new System.Drawing.Size(72, 16);
            this.chkGather.TabIndex = 23;
            this.chkGather.Text = "抓取数据";
            this.toolTip1.SetToolTip(this.chkGather, "抓取正则1匹配结果对应的文件，否则只打印匹配结果");
            this.chkGather.UseVisualStyleBackColor = true;
            this.chkGather.CheckedChanged += new System.EventHandler(this.chkGather_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkOverWrite);
            this.groupBox1.Controls.Add(this.chkRealExecute);
            this.groupBox1.Controls.Add(this.lbFileExt);
            this.groupBox1.Controls.Add(this.txtFileExt);
            this.groupBox1.Controls.Add(this.btnCopy);
            this.groupBox1.Location = new System.Drawing.Point(502, 258);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 138);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            // 
            // txtFileExt
            // 
            this.txtFileExt.Location = new System.Drawing.Point(3, 35);
            this.txtFileExt.Name = "txtFileExt";
            this.txtFileExt.Size = new System.Drawing.Size(86, 21);
            this.txtFileExt.TabIndex = 21;
            this.txtFileExt.Text = "*.jpg|*.bmp";
            this.toolTip1.SetToolTip(this.txtFileExt, "多个文件后缀名用｜线隔开");
            // 
            // lbFileExt
            // 
            this.lbFileExt.AutoSize = true;
            this.lbFileExt.Location = new System.Drawing.Point(3, 17);
            this.lbFileExt.Name = "lbFileExt";
            this.lbFileExt.Size = new System.Drawing.Size(65, 12);
            this.lbFileExt.TabIndex = 22;
            this.lbFileExt.Text = "过滤文件：";
            // 
            // chkRealExecute
            // 
            this.chkRealExecute.AutoSize = true;
            this.chkRealExecute.Location = new System.Drawing.Point(9, 82);
            this.chkRealExecute.Name = "chkRealExecute";
            this.chkRealExecute.Size = new System.Drawing.Size(72, 16);
            this.chkRealExecute.TabIndex = 23;
            this.chkRealExecute.Text = "真实操作";
            this.toolTip1.SetToolTip(this.chkRealExecute, "不勾选则模拟输出操作结果");
            this.chkRealExecute.UseVisualStyleBackColor = true;
            // 
            // chkOverWrite
            // 
            this.chkOverWrite.AutoSize = true;
            this.chkOverWrite.Location = new System.Drawing.Point(9, 60);
            this.chkOverWrite.Name = "chkOverWrite";
            this.chkOverWrite.Size = new System.Drawing.Size(72, 16);
            this.chkOverWrite.TabIndex = 23;
            this.chkOverWrite.Text = "重名覆盖";
            this.chkOverWrite.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 413);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkGather2);
            this.Controls.Add(this.chkGather);
            this.Controls.Add(this.txtRegEx2);
            this.Controls.Add(this.lbRegEx2);
            this.Controls.Add(this.txtRegEx);
            this.Controls.Add(this.lbRegEx);
            this.Controls.Add(this.lblThreadNum);
            this.Controls.Add(this.lbLen);
            this.Controls.Add(this.lbTo);
            this.Controls.Add(this.txtGrp2);
            this.Controls.Add(this.txtGrp1);
            this.Controls.Add(this.txtThreadNum);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Label lblThreadNum;
        private System.Windows.Forms.TextBox txtThreadNum;
        private System.Windows.Forms.CheckBox chkGather2;
        private System.Windows.Forms.CheckBox chkGather;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkRealExecute;
        private System.Windows.Forms.Label lbFileExt;
        private System.Windows.Forms.TextBox txtFileExt;
        private System.Windows.Forms.CheckBox chkOverWrite;
    }
}

