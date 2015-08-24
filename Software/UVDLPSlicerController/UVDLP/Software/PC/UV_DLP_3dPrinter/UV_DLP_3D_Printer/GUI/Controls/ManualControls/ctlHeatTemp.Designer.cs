namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlHeatTemp
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdSet1 = new System.Windows.Forms.Button();
            this.txtVal1 = new System.Windows.Forms.TextBox();
            this.cmdOff1 = new System.Windows.Forms.Button();
            this.cmdOff2 = new System.Windows.Forms.Button();
            this.cmdSet2 = new System.Windows.Forms.Button();
            this.txtVal2 = new System.Windows.Forms.TextBox();
            this.chkMonitorTemps = new System.Windows.Forms.CheckBox();
            this.lblHBP = new System.Windows.Forms.Label();
            this.lblEXT0 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmdSet1
            // 
            this.cmdSet1.Location = new System.Drawing.Point(68, 28);
            this.cmdSet1.Name = "cmdSet1";
            this.cmdSet1.Size = new System.Drawing.Size(53, 23);
            this.cmdSet1.TabIndex = 0;
            this.cmdSet1.Text = "Set";
            this.cmdSet1.UseVisualStyleBackColor = true;
            this.cmdSet1.Click += new System.EventHandler(this.cmdSet1_Click);
            // 
            // txtVal1
            // 
            this.txtVal1.Location = new System.Drawing.Point(127, 29);
            this.txtVal1.Name = "txtVal1";
            this.txtVal1.Size = new System.Drawing.Size(46, 22);
            this.txtVal1.TabIndex = 2;
            this.txtVal1.Text = "225";
            // 
            // cmdOff1
            // 
            this.cmdOff1.Location = new System.Drawing.Point(9, 28);
            this.cmdOff1.Name = "cmdOff1";
            this.cmdOff1.Size = new System.Drawing.Size(53, 23);
            this.cmdOff1.TabIndex = 3;
            this.cmdOff1.Text = "Off";
            this.cmdOff1.UseVisualStyleBackColor = true;
            this.cmdOff1.Click += new System.EventHandler(this.cmdOff1_Click);
            // 
            // cmdOff2
            // 
            this.cmdOff2.Location = new System.Drawing.Point(9, 85);
            this.cmdOff2.Name = "cmdOff2";
            this.cmdOff2.Size = new System.Drawing.Size(53, 23);
            this.cmdOff2.TabIndex = 8;
            this.cmdOff2.Text = "Off";
            this.cmdOff2.UseVisualStyleBackColor = true;
            this.cmdOff2.Click += new System.EventHandler(this.cmdOff2_Click);
            // 
            // cmdSet2
            // 
            this.cmdSet2.Location = new System.Drawing.Point(68, 85);
            this.cmdSet2.Name = "cmdSet2";
            this.cmdSet2.Size = new System.Drawing.Size(53, 23);
            this.cmdSet2.TabIndex = 5;
            this.cmdSet2.Text = "Set";
            this.cmdSet2.UseVisualStyleBackColor = true;
            this.cmdSet2.Click += new System.EventHandler(this.cmdSet2_Click);
            // 
            // txtVal2
            // 
            this.txtVal2.Location = new System.Drawing.Point(126, 85);
            this.txtVal2.Name = "txtVal2";
            this.txtVal2.Size = new System.Drawing.Size(46, 22);
            this.txtVal2.TabIndex = 7;
            this.txtVal2.Text = "125";
            // 
            // chkMonitorTemps
            // 
            this.chkMonitorTemps.AutoSize = true;
            this.chkMonitorTemps.Location = new System.Drawing.Point(9, 114);
            this.chkMonitorTemps.Name = "chkMonitorTemps";
            this.chkMonitorTemps.Size = new System.Drawing.Size(163, 21);
            this.chkMonitorTemps.TabIndex = 11;
            this.chkMonitorTemps.Text = "Monitor Temperature";
            this.chkMonitorTemps.UseVisualStyleBackColor = true;
            this.chkMonitorTemps.CheckedChanged += new System.EventHandler(this.chkMonitorTemps_CheckedChanged);
            // 
            // lblHBP
            // 
            this.lblHBP.AutoSize = true;
            this.lblHBP.Location = new System.Drawing.Point(178, 88);
            this.lblHBP.Name = "lblHBP";
            this.lblHBP.Size = new System.Drawing.Size(17, 17);
            this.lblHBP.TabIndex = 12;
            this.lblHBP.Text = "C";
            // 
            // lblEXT0
            // 
            this.lblEXT0.AutoSize = true;
            this.lblEXT0.Location = new System.Drawing.Point(179, 34);
            this.lblEXT0.Name = "lblEXT0";
            this.lblEXT0.Size = new System.Drawing.Size(17, 17);
            this.lblEXT0.TabIndex = 13;
            this.lblEXT0.Text = "C";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(5, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 22);
            this.label2.TabIndex = 30;
            this.label2.Text = "Heater 1: (Ext0)";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 22);
            this.label1.TabIndex = 31;
            this.label1.Text = "Heater 2: (HBP)";
            // 
            // ctlHeatTemp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblEXT0);
            this.Controls.Add(this.lblHBP);
            this.Controls.Add(this.chkMonitorTemps);
            this.Controls.Add(this.cmdOff2);
            this.Controls.Add(this.cmdSet2);
            this.Controls.Add(this.txtVal2);
            this.Controls.Add(this.cmdOff1);
            this.Controls.Add(this.cmdSet1);
            this.Controls.Add(this.txtVal1);
            this.Name = "ctlHeatTemp";
            this.Size = new System.Drawing.Size(237, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdSet1;
        private System.Windows.Forms.TextBox txtVal1;
        private System.Windows.Forms.Button cmdOff1;
        private System.Windows.Forms.Button cmdOff2;
        private System.Windows.Forms.Button cmdSet2;
        private System.Windows.Forms.TextBox txtVal2;
        private System.Windows.Forms.CheckBox chkMonitorTemps;
        private System.Windows.Forms.Label lblHBP;
        private System.Windows.Forms.Label lblEXT0;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
