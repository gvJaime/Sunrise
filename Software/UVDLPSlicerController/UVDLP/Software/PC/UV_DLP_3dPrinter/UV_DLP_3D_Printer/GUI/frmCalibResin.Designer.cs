namespace UV_DLP_3D_Printer.GUI
{
    partial class frmCalibResin
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
            this.buttPrint = new System.Windows.Forms.Button();
            this.txtStepExpose = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMinExpose = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtThk09 = new System.Windows.Forms.TextBox();
            this.txtThk08 = new System.Windows.Forms.TextBox();
            this.txtThk07 = new System.Windows.Forms.TextBox();
            this.txtThk06 = new System.Windows.Forms.TextBox();
            this.txtThk05 = new System.Windows.Forms.TextBox();
            this.txtThk04 = new System.Windows.Forms.TextBox();
            this.txtThk03 = new System.Windows.Forms.TextBox();
            this.txtThk02 = new System.Windows.Forms.TextBox();
            this.txtThk01 = new System.Windows.Forms.TextBox();
            this.txtThk00 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttPrint);
            this.groupBox1.Controls.Add(this.txtStepExpose);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtMinExpose);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(273, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Print Generation";
            // 
            // buttPrint
            // 
            this.buttPrint.Location = new System.Drawing.Point(6, 76);
            this.buttPrint.Name = "buttPrint";
            this.buttPrint.Size = new System.Drawing.Size(120, 30);
            this.buttPrint.TabIndex = 4;
            this.buttPrint.Text = "Print test model";
            this.buttPrint.UseVisualStyleBackColor = true;
            this.buttPrint.Click += new System.EventHandler(this.buttPrint_Click);
            // 
            // txtStepExpose
            // 
            this.txtStepExpose.Location = new System.Drawing.Point(132, 48);
            this.txtStepExpose.Name = "txtStepExpose";
            this.txtStepExpose.Size = new System.Drawing.Size(69, 20);
            this.txtStepExpose.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Exposure Step (ms):";
            // 
            // txtMinExpose
            // 
            this.txtMinExpose.Location = new System.Drawing.Point(132, 22);
            this.txtMinExpose.Name = "txtMinExpose";
            this.txtMinExpose.Size = new System.Drawing.Size(69, 20);
            this.txtMinExpose.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minimum Exposure (ms):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtThk09);
            this.groupBox2.Controls.Add(this.txtThk08);
            this.groupBox2.Controls.Add(this.txtThk07);
            this.groupBox2.Controls.Add(this.txtThk06);
            this.groupBox2.Controls.Add(this.txtThk05);
            this.groupBox2.Controls.Add(this.txtThk04);
            this.groupBox2.Controls.Add(this.txtThk03);
            this.groupBox2.Controls.Add(this.txtThk02);
            this.groupBox2.Controls.Add(this.txtThk01);
            this.groupBox2.Controls.Add(this.txtThk00);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 135);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(273, 134);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Exposure calculation";
            // 
            // txtThk09
            // 
            this.txtThk09.Location = new System.Drawing.Point(216, 105);
            this.txtThk09.Name = "txtThk09";
            this.txtThk09.Size = new System.Drawing.Size(46, 20);
            this.txtThk09.TabIndex = 10;
            // 
            // txtThk08
            // 
            this.txtThk08.Location = new System.Drawing.Point(165, 105);
            this.txtThk08.Name = "txtThk08";
            this.txtThk08.Size = new System.Drawing.Size(46, 20);
            this.txtThk08.TabIndex = 9;
            // 
            // txtThk07
            // 
            this.txtThk07.Location = new System.Drawing.Point(113, 105);
            this.txtThk07.Name = "txtThk07";
            this.txtThk07.Size = new System.Drawing.Size(46, 20);
            this.txtThk07.TabIndex = 8;
            // 
            // txtThk06
            // 
            this.txtThk06.Location = new System.Drawing.Point(61, 105);
            this.txtThk06.Name = "txtThk06";
            this.txtThk06.Size = new System.Drawing.Size(46, 20);
            this.txtThk06.TabIndex = 7;
            // 
            // txtThk05
            // 
            this.txtThk05.Location = new System.Drawing.Point(9, 105);
            this.txtThk05.Name = "txtThk05";
            this.txtThk05.Size = new System.Drawing.Size(46, 20);
            this.txtThk05.TabIndex = 6;
            // 
            // txtThk04
            // 
            this.txtThk04.Location = new System.Drawing.Point(216, 79);
            this.txtThk04.Name = "txtThk04";
            this.txtThk04.Size = new System.Drawing.Size(46, 20);
            this.txtThk04.TabIndex = 5;
            // 
            // txtThk03
            // 
            this.txtThk03.Location = new System.Drawing.Point(165, 79);
            this.txtThk03.Name = "txtThk03";
            this.txtThk03.Size = new System.Drawing.Size(46, 20);
            this.txtThk03.TabIndex = 4;
            // 
            // txtThk02
            // 
            this.txtThk02.Location = new System.Drawing.Point(113, 79);
            this.txtThk02.Name = "txtThk02";
            this.txtThk02.Size = new System.Drawing.Size(46, 20);
            this.txtThk02.TabIndex = 3;
            // 
            // txtThk01
            // 
            this.txtThk01.Location = new System.Drawing.Point(61, 79);
            this.txtThk01.Name = "txtThk01";
            this.txtThk01.Size = new System.Drawing.Size(46, 20);
            this.txtThk01.TabIndex = 2;
            // 
            // txtThk00
            // 
            this.txtThk00.Location = new System.Drawing.Point(9, 79);
            this.txtThk00.Name = "txtThk00";
            this.txtThk00.Size = new System.Drawing.Size(46, 20);
            this.txtThk00.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 39);
            this.label3.TabIndex = 0;
            this.label3.Text = "Use cell pattern exposure times bellow\r\nand actual model cell thickness to determ" +
                "ine\r\nbest exposure time.";
            // 
            // buttClose
            // 
            this.buttClose.Location = new System.Drawing.Point(83, 275);
            this.buttClose.Name = "buttClose";
            this.buttClose.Size = new System.Drawing.Size(120, 30);
            this.buttClose.TabIndex = 5;
            this.buttClose.Text = "Close";
            this.buttClose.UseVisualStyleBackColor = true;
            this.buttClose.Click += new System.EventHandler(this.buttClose_Click);
            // 
            // frmCalibResin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 317);
            this.Controls.Add(this.buttClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmCalibResin";
            this.Text = "Calibrate Resin";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttPrint;
        private System.Windows.Forms.TextBox txtStepExpose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinExpose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtThk09;
        private System.Windows.Forms.TextBox txtThk08;
        private System.Windows.Forms.TextBox txtThk07;
        private System.Windows.Forms.TextBox txtThk06;
        private System.Windows.Forms.TextBox txtThk05;
        private System.Windows.Forms.TextBox txtThk04;
        private System.Windows.Forms.TextBox txtThk03;
        private System.Windows.Forms.TextBox txtThk02;
        private System.Windows.Forms.TextBox txtThk01;
        private System.Windows.Forms.TextBox txtThk00;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttClose;
    }
}