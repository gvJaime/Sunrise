namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlConfig
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
            this.buttShowMachineConfig = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlShowSliceConfig = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttShowMachineConfig
            // 
            this.buttShowMachineConfig.BackColor = System.Drawing.Color.Navy;
            this.buttShowMachineConfig.Checked = false;
            this.buttShowMachineConfig.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttShowMachineConfig.Gapx = 5;
            this.buttShowMachineConfig.Gapy = 5;
            this.buttShowMachineConfig.GLBackgroundImage = null;
            this.buttShowMachineConfig.GLImage = null;
            this.buttShowMachineConfig.GLVisible = false;
            this.buttShowMachineConfig.GuiAnchor = null;
            this.buttShowMachineConfig.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttShowMachineConfig.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttMachineConfig;
            this.buttShowMachineConfig.Location = new System.Drawing.Point(5, 5);
            this.buttShowMachineConfig.Margin = new System.Windows.Forms.Padding(5);
            this.buttShowMachineConfig.Name = "buttShowMachineConfig";
            this.buttShowMachineConfig.OnClickCallback = "ShowMachineConfig";
            this.buttShowMachineConfig.Size = new System.Drawing.Size(48, 48);
            this.buttShowMachineConfig.StyleName = null;
            this.buttShowMachineConfig.TabIndex = 32;
            this.buttShowMachineConfig.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlShowSliceConfig
            // 
            this.ctlShowSliceConfig.BackColor = System.Drawing.Color.Navy;
            this.ctlShowSliceConfig.Checked = false;
            this.ctlShowSliceConfig.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.ctlShowSliceConfig.Gapx = 5;
            this.ctlShowSliceConfig.Gapy = 5;
            this.ctlShowSliceConfig.GLBackgroundImage = null;
            this.ctlShowSliceConfig.GLImage = null;
            this.ctlShowSliceConfig.GLVisible = false;
            this.ctlShowSliceConfig.GuiAnchor = null;
            this.ctlShowSliceConfig.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlShowSliceConfig.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSliceConf;
            this.ctlShowSliceConfig.Location = new System.Drawing.Point(63, 5);
            this.ctlShowSliceConfig.Margin = new System.Windows.Forms.Padding(5);
            this.ctlShowSliceConfig.Name = "ctlShowSliceConfig";
            this.ctlShowSliceConfig.OnClickCallback = "ShowSliceConfig";
            this.ctlShowSliceConfig.Size = new System.Drawing.Size(48, 48);
            this.ctlShowSliceConfig.StyleName = null;
            this.ctlShowSliceConfig.TabIndex = 34;
            this.ctlShowSliceConfig.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(6, 2);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(182, 33);
            this.lblTitle.TabIndex = 35;
            this.lblTitle.Text = "Configuration";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel5.Controls.Add(this.buttShowMachineConfig);
            this.flowLayoutPanel5.Controls.Add(this.ctlShowSliceConfig);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(6, 39);
            this.flowLayoutPanel5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(182, 58);
            this.flowLayoutPanel5.TabIndex = 36;
            // 
            // ctlConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel5);
            this.Controls.Add(this.lblTitle);
            this.Name = "ctlConfig";
            this.Size = new System.Drawing.Size(197, 105);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ctlImageButton buttShowMachineConfig;
        private ctlImageButton ctlShowSliceConfig;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
    }
}
