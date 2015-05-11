namespace UV_DLP_3D_Printer.GUI
{
    partial class frmSlice
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
            this.cmdSliceOptions = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.prgSlice = new System.Windows.Forms.ProgressBar();
            this.cmdSlice = new System.Windows.Forms.Button();
            this.cmbSliceEngine = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmbSliceProfiles = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdSliceOptions
            // 
            this.cmdSliceOptions.Location = new System.Drawing.Point(353, 12);
            this.cmdSliceOptions.Name = "cmdSliceOptions";
            this.cmdSliceOptions.Size = new System.Drawing.Size(122, 36);
            this.cmdSliceOptions.TabIndex = 27;
            this.cmdSliceOptions.Text = "Options";
            this.cmdSliceOptions.UseVisualStyleBackColor = true;
            this.cmdSliceOptions.Visible = false;
            this.cmdSliceOptions.Click += new System.EventHandler(this.cmdSliceOptions_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(150, 22);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(0, 17);
            this.lblMessage.TabIndex = 26;
            // 
            // prgSlice
            // 
            this.prgSlice.Location = new System.Drawing.Point(12, 107);
            this.prgSlice.Name = "prgSlice";
            this.prgSlice.Size = new System.Drawing.Size(453, 23);
            this.prgSlice.TabIndex = 25;
            // 
            // cmdSlice
            // 
            this.cmdSlice.Location = new System.Drawing.Point(166, 200);
            this.cmdSlice.Name = "cmdSlice";
            this.cmdSlice.Size = new System.Drawing.Size(122, 36);
            this.cmdSlice.TabIndex = 24;
            this.cmdSlice.Text = "Slice";
            this.cmdSlice.UseVisualStyleBackColor = true;
            this.cmdSlice.Click += new System.EventHandler(this.cmdSlice_Click);
            // 
            // cmbSliceEngine
            // 
            this.cmbSliceEngine.FormattingEnabled = true;
            this.cmbSliceEngine.Items.AddRange(new object[] {
            "UV DLP Slicer",
            "Slic3r"});
            this.cmbSliceEngine.Location = new System.Drawing.Point(237, 146);
            this.cmbSliceEngine.Name = "cmbSliceEngine";
            this.cmbSliceEngine.Size = new System.Drawing.Size(194, 24);
            this.cmbSliceEngine.TabIndex = 29;
            this.cmbSliceEngine.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 17);
            this.label1.TabIndex = 30;
            this.label1.Text = "Slicing Engine";
            this.label1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::UV_DLP_3D_Printer.Properties.Resources.animatedTurningHelix;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.TabIndex = 31;
            this.pictureBox1.TabStop = false;
            // 
            // cmbSliceProfiles
            // 
            this.cmbSliceProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSliceProfiles.FormattingEnabled = true;
            this.cmbSliceProfiles.Location = new System.Drawing.Point(204, 57);
            this.cmbSliceProfiles.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbSliceProfiles.Name = "cmbSliceProfiles";
            this.cmbSliceProfiles.Size = new System.Drawing.Size(193, 24);
            this.cmbSliceProfiles.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 17);
            this.label2.TabIndex = 33;
            this.label2.Text = "Slicing Profile:";
            // 
            // frmSlice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 248);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbSliceProfiles);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSliceEngine);
            this.Controls.Add(this.cmdSliceOptions);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.prgSlice);
            this.Controls.Add(this.cmdSlice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSlice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Slice!";
            this.Activated += new System.EventHandler(this.frmSlice_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSlice_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdSliceOptions;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ProgressBar prgSlice;
        private System.Windows.Forms.Button cmdSlice;
        private System.Windows.Forms.ComboBox cmbSliceEngine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cmbSliceProfiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}