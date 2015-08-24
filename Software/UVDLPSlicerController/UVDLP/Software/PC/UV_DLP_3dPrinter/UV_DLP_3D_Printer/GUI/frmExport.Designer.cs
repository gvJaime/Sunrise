namespace UV_DLP_3D_Printer.GUI
{
    partial class frmExport
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
            this.progressExport = new System.Windows.Forms.ProgressBar();
            this.buttExport = new System.Windows.Forms.Button();
            this.buttCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboExportType = new System.Windows.Forms.ComboBox();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // progressExport
            // 
            this.progressExport.Location = new System.Drawing.Point(12, 198);
            this.progressExport.Name = "progressExport";
            this.progressExport.Size = new System.Drawing.Size(544, 19);
            this.progressExport.Step = 1;
            this.progressExport.TabIndex = 0;
            // 
            // buttExport
            // 
            this.buttExport.Location = new System.Drawing.Point(12, 230);
            this.buttExport.Name = "buttExport";
            this.buttExport.Size = new System.Drawing.Size(111, 24);
            this.buttExport.TabIndex = 1;
            this.buttExport.Text = "Export";
            this.buttExport.UseVisualStyleBackColor = true;
            this.buttExport.Click += new System.EventHandler(this.buttExport_Click);
            // 
            // buttCancel
            // 
            this.buttCancel.Location = new System.Drawing.Point(445, 230);
            this.buttCancel.Name = "buttCancel";
            this.buttCancel.Size = new System.Drawing.Size(111, 24);
            this.buttCancel.TabIndex = 2;
            this.buttCancel.Text = "Cancel";
            this.buttCancel.UseVisualStyleBackColor = true;
            this.buttCancel.Click += new System.EventHandler(this.buttCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Export type:";
            // 
            // comboExportType
            // 
            this.comboExportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboExportType.FormattingEnabled = true;
            this.comboExportType.Location = new System.Drawing.Point(78, 26);
            this.comboExportType.Name = "comboExportType";
            this.comboExportType.Size = new System.Drawing.Size(478, 21);
            this.comboExportType.TabIndex = 4;
            this.comboExportType.SelectedIndexChanged += new System.EventHandler(this.comboExportType_SelectedIndexChanged);
            // 
            // panelSettings
            // 
            this.panelSettings.Location = new System.Drawing.Point(12, 56);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(543, 132);
            this.panelSettings.TabIndex = 5;
            // 
            // frmExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 266);
            this.Controls.Add(this.panelSettings);
            this.Controls.Add(this.comboExportType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttCancel);
            this.Controls.Add(this.buttExport);
            this.Controls.Add(this.progressExport);
            this.Name = "frmExport";
            this.Text = "frmExport";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressExport;
        private System.Windows.Forms.Button buttExport;
        private System.Windows.Forms.Button buttCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboExportType;
        private System.Windows.Forms.Panel panelSettings;
    }
}