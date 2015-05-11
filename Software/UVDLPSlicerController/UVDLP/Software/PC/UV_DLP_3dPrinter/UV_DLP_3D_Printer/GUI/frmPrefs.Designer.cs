namespace UV_DLP_3D_Printer.GUI
{
    partial class frmPrefs
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
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.cmdselectfore = new System.Windows.Forms.Button();
            this.panelfore = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelback = new System.Windows.Forms.Panel();
            this.cmdselectback = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.grpDebug = new System.Windows.Forms.GroupBox();
            this.chkIgnoreGCRsp = new System.Windows.Forms.CheckBox();
            this.chkDriverLog = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.grpDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(232, 184);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(82, 36);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(320, 184);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 36);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // cmdselectfore
            // 
            this.cmdselectfore.Location = new System.Drawing.Point(41, 104);
            this.cmdselectfore.Name = "cmdselectfore";
            this.cmdselectfore.Size = new System.Drawing.Size(75, 39);
            this.cmdselectfore.TabIndex = 6;
            this.cmdselectfore.Text = "Select";
            this.cmdselectfore.UseVisualStyleBackColor = true;
            this.cmdselectfore.Click += new System.EventHandler(this.cmdselectfore_Click);
            // 
            // panelfore
            // 
            this.panelfore.Location = new System.Drawing.Point(41, 49);
            this.panelfore.Name = "panelfore";
            this.panelfore.Size = new System.Drawing.Size(75, 49);
            this.panelfore.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Foreground Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Background Color";
            // 
            // panelback
            // 
            this.panelback.Location = new System.Drawing.Point(178, 49);
            this.panelback.Name = "panelback";
            this.panelback.Size = new System.Drawing.Size(75, 49);
            this.panelback.TabIndex = 10;
            // 
            // cmdselectback
            // 
            this.cmdselectback.Location = new System.Drawing.Point(178, 104);
            this.cmdselectback.Name = "cmdselectback";
            this.cmdselectback.Size = new System.Drawing.Size(75, 39);
            this.cmdselectback.TabIndex = 9;
            this.cmdselectback.Text = "Select";
            this.cmdselectback.UseVisualStyleBackColor = true;
            this.cmdselectback.Click += new System.EventHandler(this.cmdselectback_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelback);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmdselectfore);
            this.groupBox2.Controls.Add(this.panelfore);
            this.groupBox2.Controls.Add(this.cmdselectback);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(315, 153);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Slice Color Selection";
            // 
            // grpDebug
            // 
            this.grpDebug.Controls.Add(this.chkIgnoreGCRsp);
            this.grpDebug.Controls.Add(this.chkDriverLog);
            this.grpDebug.Location = new System.Drawing.Point(333, 12);
            this.grpDebug.Name = "grpDebug";
            this.grpDebug.Size = new System.Drawing.Size(295, 153);
            this.grpDebug.TabIndex = 13;
            this.grpDebug.TabStop = false;
            this.grpDebug.Text = "Debugging";
            // 
            // chkIgnoreGCRsp
            // 
            this.chkIgnoreGCRsp.AutoSize = true;
            this.chkIgnoreGCRsp.Location = new System.Drawing.Point(7, 52);
            this.chkIgnoreGCRsp.Name = "chkIgnoreGCRsp";
            this.chkIgnoreGCRsp.Size = new System.Drawing.Size(193, 21);
            this.chkIgnoreGCRsp.TabIndex = 1;
            this.chkIgnoreGCRsp.Text = "Ignore GCode Responses";
            this.chkIgnoreGCRsp.UseVisualStyleBackColor = true;
            // 
            // chkDriverLog
            // 
            this.chkDriverLog.AutoSize = true;
            this.chkDriverLog.Location = new System.Drawing.Point(7, 24);
            this.chkDriverLog.Name = "chkDriverLog";
            this.chkDriverLog.Size = new System.Drawing.Size(247, 21);
            this.chkDriverLog.TabIndex = 0;
            this.chkDriverLog.Text = "Log Driver debugging to comm.log";
            this.chkDriverLog.UseVisualStyleBackColor = true;
            // 
            // frmPrefs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 232);
            this.Controls.Add(this.grpDebug);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrefs";
            this.Text = "Program Preferences";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grpDebug.ResumeLayout(false);
            this.grpDebug.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button cmdselectfore;
        private System.Windows.Forms.Panel panelfore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelback;
        private System.Windows.Forms.Button cmdselectback;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox grpDebug;
        private System.Windows.Forms.CheckBox chkIgnoreGCRsp;
        private System.Windows.Forms.CheckBox chkDriverLog;
    }
}