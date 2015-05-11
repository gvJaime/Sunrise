namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlGcodeView
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.ctlImageButton1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttConfig = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            this.txtGCode = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.ctlImageButton1);
            this.panel1.Controls.Add(this.buttConfig);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 279);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(402, 56);
            this.panel1.TabIndex = 2;
            // 
            // ctlImageButton1
            // 
            this.ctlImageButton1.BackColor = System.Drawing.SystemColors.Control;
            this.ctlImageButton1.Checked = false;
            this.ctlImageButton1.CheckImage = null;
            this.ctlImageButton1.Gapx = 10;
            this.ctlImageButton1.Gapy = 10;
            this.ctlImageButton1.GLBackgroundImage = null;
            this.ctlImageButton1.GLImage = null;
            this.ctlImageButton1.GLVisible = false;
            this.ctlImageButton1.GuiAnchor = null;
            this.ctlImageButton1.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Right;
            this.ctlImageButton1.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSave;
            this.ctlImageButton1.Location = new System.Drawing.Point(57, 4);
            this.ctlImageButton1.Margin = new System.Windows.Forms.Padding(4, 4, 10, 4);
            this.ctlImageButton1.Name = "ctlImageButton1";
            this.ctlImageButton1.Size = new System.Drawing.Size(48, 48);
            this.ctlImageButton1.StyleName = null;
            this.ctlImageButton1.TabIndex = 19;
            this.ctlToolTip1.SetToolTip(this.ctlImageButton1, "Save G-Code file");
            this.ctlImageButton1.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Top;
            this.ctlImageButton1.Click += new System.EventHandler(this.ctlImageButton1_Click);
            // 
            // buttConfig
            // 
            this.buttConfig.BackColor = System.Drawing.SystemColors.Control;
            this.buttConfig.Checked = false;
            this.buttConfig.CheckImage = null;
            this.buttConfig.Gapx = 10;
            this.buttConfig.Gapy = 10;
            this.buttConfig.GLBackgroundImage = null;
            this.buttConfig.GLImage = null;
            this.buttConfig.GLVisible = false;
            this.buttConfig.GuiAnchor = null;
            this.buttConfig.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Right;
            this.buttConfig.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttOpenFile;
            this.buttConfig.Location = new System.Drawing.Point(4, 4);
            this.buttConfig.Margin = new System.Windows.Forms.Padding(4, 4, 10, 4);
            this.buttConfig.Name = "buttConfig";
            this.buttConfig.Size = new System.Drawing.Size(48, 48);
            this.buttConfig.StyleName = null;
            this.buttConfig.TabIndex = 19;
            this.ctlToolTip1.SetToolTip(this.buttConfig, "Load G-Code file");
            this.buttConfig.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Top;
            this.buttConfig.Click += new System.EventHandler(this.buttConfig_Click);
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 1500;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // txtGCode
            // 
            this.txtGCode.AcceptsReturn = true;
            this.txtGCode.BackColor = System.Drawing.Color.White;
            this.txtGCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGCode.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGCode.Location = new System.Drawing.Point(0, 0);
            this.txtGCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGCode.Multiline = true;
            this.txtGCode.Name = "txtGCode";
            this.txtGCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGCode.Size = new System.Drawing.Size(402, 279);
            this.txtGCode.TabIndex = 3;
            this.txtGCode.WordWrap = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ctlGcodeView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.txtGCode);
            this.Controls.Add(this.panel1);
            this.Name = "ctlGcodeView";
            this.Size = new System.Drawing.Size(402, 335);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomGUI.ctlImageButton buttConfig;
        private CustomGUI.ctlImageButton ctlImageButton1;
        private CustomGUI.ctlToolTip ctlToolTip1;
        private System.Windows.Forms.TextBox txtGCode;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
