namespace UV_DLP_3D_Printer.GUI
{
    partial class frmBuildSizeCalib
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
            this.modelgroup = new System.Windows.Forms.GroupBox();
            this.txtmodely = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtmodelx = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtmeasuredy = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtmeasuredx = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBuildSizeY = new System.Windows.Forms.Label();
            this.lblBuildSizeX = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblNewBuildSizeY = new System.Windows.Forms.Label();
            this.lblNewBuildSizeX = new System.Windows.Forms.Label();
            this.modelgroup.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Location = new System.Drawing.Point(95, 247);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(120, 37);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // modelgroup
            // 
            this.modelgroup.Controls.Add(this.txtmodely);
            this.modelgroup.Controls.Add(this.label2);
            this.modelgroup.Controls.Add(this.txtmodelx);
            this.modelgroup.Controls.Add(this.label1);
            this.modelgroup.Location = new System.Drawing.Point(12, 12);
            this.modelgroup.Name = "modelgroup";
            this.modelgroup.Size = new System.Drawing.Size(203, 104);
            this.modelgroup.TabIndex = 1;
            this.modelgroup.TabStop = false;
            this.modelgroup.Text = "Model Size (mm)";
            // 
            // txtmodely
            // 
            this.txtmodely.Location = new System.Drawing.Point(72, 61);
            this.txtmodely.Name = "txtmodely";
            this.txtmodely.Size = new System.Drawing.Size(100, 22);
            this.txtmodely.TabIndex = 3;
            this.txtmodely.Text = "2.0";
            this.txtmodely.TextChanged += new System.EventHandler(this.txtmodely_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y Size";
            // 
            // txtmodelx
            // 
            this.txtmodelx.Location = new System.Drawing.Point(72, 33);
            this.txtmodelx.Name = "txtmodelx";
            this.txtmodelx.Size = new System.Drawing.Size(100, 22);
            this.txtmodelx.TabIndex = 1;
            this.txtmodelx.Text = "2.0";
            this.txtmodelx.TextChanged += new System.EventHandler(this.txtmodelx_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "X Size";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtmeasuredy);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtmeasuredx);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(221, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 104);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Measured Size (mm)";
            // 
            // txtmeasuredy
            // 
            this.txtmeasuredy.Location = new System.Drawing.Point(72, 61);
            this.txtmeasuredy.Name = "txtmeasuredy";
            this.txtmeasuredy.Size = new System.Drawing.Size(100, 22);
            this.txtmeasuredy.TabIndex = 3;
            this.txtmeasuredy.Text = "2.0";
            this.txtmeasuredy.TextChanged += new System.EventHandler(this.txtmeasuredy_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y Size";
            // 
            // txtmeasuredx
            // 
            this.txtmeasuredx.Location = new System.Drawing.Point(72, 33);
            this.txtmeasuredx.Name = "txtmeasuredx";
            this.txtmeasuredx.Size = new System.Drawing.Size(100, 22);
            this.txtmeasuredx.TabIndex = 1;
            this.txtmeasuredx.Text = "2.0";
            this.txtmeasuredx.TextChanged += new System.EventHandler(this.txtmeasuredx_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "X Size";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(221, 247);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(120, 37);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblBuildSizeY);
            this.groupBox1.Controls.Add(this.lblBuildSizeX);
            this.groupBox1.Location = new System.Drawing.Point(12, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 104);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current Build Platform Size";
            // 
            // lblBuildSizeY
            // 
            this.lblBuildSizeY.AutoSize = true;
            this.lblBuildSizeY.Location = new System.Drawing.Point(6, 61);
            this.lblBuildSizeY.Name = "lblBuildSizeY";
            this.lblBuildSizeY.Size = new System.Drawing.Size(48, 17);
            this.lblBuildSizeY.TabIndex = 2;
            this.lblBuildSizeY.Text = "Y Size";
            // 
            // lblBuildSizeX
            // 
            this.lblBuildSizeX.AutoSize = true;
            this.lblBuildSizeX.Location = new System.Drawing.Point(6, 33);
            this.lblBuildSizeX.Name = "lblBuildSizeX";
            this.lblBuildSizeX.Size = new System.Drawing.Size(48, 17);
            this.lblBuildSizeX.TabIndex = 0;
            this.lblBuildSizeX.Text = "X Size";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblNewBuildSizeY);
            this.groupBox3.Controls.Add(this.lblNewBuildSizeX);
            this.groupBox3.Location = new System.Drawing.Point(221, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(203, 104);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "New Build Platform Size";
            // 
            // lblNewBuildSizeY
            // 
            this.lblNewBuildSizeY.AutoSize = true;
            this.lblNewBuildSizeY.Location = new System.Drawing.Point(6, 61);
            this.lblNewBuildSizeY.Name = "lblNewBuildSizeY";
            this.lblNewBuildSizeY.Size = new System.Drawing.Size(48, 17);
            this.lblNewBuildSizeY.TabIndex = 2;
            this.lblNewBuildSizeY.Text = "Y Size";
            // 
            // lblNewBuildSizeX
            // 
            this.lblNewBuildSizeX.AutoSize = true;
            this.lblNewBuildSizeX.Location = new System.Drawing.Point(6, 33);
            this.lblNewBuildSizeX.Name = "lblNewBuildSizeX";
            this.lblNewBuildSizeX.Size = new System.Drawing.Size(48, 17);
            this.lblNewBuildSizeX.TabIndex = 0;
            this.lblNewBuildSizeX.Text = "X Size";
            // 
            // frmBuildSizeCalib
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 298);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.modelgroup);
            this.Controls.Add(this.cmdOK);
            this.Name = "frmBuildSizeCalib";
            this.Text = "Build Size Calibration";
            this.modelgroup.ResumeLayout(false);
            this.modelgroup.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox modelgroup;
        private System.Windows.Forms.TextBox txtmodely;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtmodelx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtmeasuredy;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtmeasuredx;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblBuildSizeY;
        private System.Windows.Forms.Label lblBuildSizeX;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblNewBuildSizeY;
        private System.Windows.Forms.Label lblNewBuildSizeX;
    }
}