namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlMainConfig
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ctlMachineConfigView = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.ctlSliceProfileConfig = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.ctlMachineConfig1 = new UV_DLP_3D_Printer.GUI.Controls.ctlMachineConfig();
            this.ctlToolpathGenConfig1 = new UV_DLP_3D_Printer.GUI.Controls.ctlToolpathGenConfig();
            this.pnlMachineConfig = new System.Windows.Forms.Panel();
            this.pnlToolpathGenConfig = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlMachineConfig.SuspendLayout();
            this.pnlToolpathGenConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.ctlMachineConfigView);
            this.flowLayoutPanel1.Controls.Add(this.ctlSliceProfileConfig);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(991, 50);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // ctlMachineConfigView
            // 
            this.ctlMachineConfigView.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlMachineConfigView.Checked = false;
            this.ctlMachineConfigView.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.ctlMachineConfigView.Gapx = 0;
            this.ctlMachineConfigView.Gapy = 0;
            this.ctlMachineConfigView.GLBackgroundImage = null;
            this.ctlMachineConfigView.GLVisible = false;
            this.ctlMachineConfigView.GuiAnchor = null;
            this.ctlMachineConfigView.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttMachineConfig;
            this.ctlMachineConfigView.Location = new System.Drawing.Point(3, 3);
            this.ctlMachineConfigView.Name = "ctlMachineConfigView";
            this.ctlMachineConfigView.OnClickCallback = "ClickViewConfMachine";
            this.ctlMachineConfigView.Size = new System.Drawing.Size(290, 40);
            this.ctlMachineConfigView.StyleName = null;
            this.ctlMachineConfigView.TabIndex = 1;
            this.ctlMachineConfigView.Text = "Configure Machine";
            // 
            // ctlSliceProfileConfig
            // 
            this.ctlSliceProfileConfig.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlSliceProfileConfig.Checked = false;
            this.ctlSliceProfileConfig.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.ctlSliceProfileConfig.Gapx = 0;
            this.ctlSliceProfileConfig.Gapy = 0;
            this.ctlSliceProfileConfig.GLBackgroundImage = null;
            this.ctlSliceProfileConfig.GLVisible = false;
            this.ctlSliceProfileConfig.GuiAnchor = null;
            this.ctlSliceProfileConfig.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSliceConf;
            this.ctlSliceProfileConfig.Location = new System.Drawing.Point(299, 3);
            this.ctlSliceProfileConfig.Name = "ctlSliceProfileConfig";
            this.ctlSliceProfileConfig.OnClickCallback = "ClickViewSliceConfig";
            this.ctlSliceProfileConfig.Size = new System.Drawing.Size(290, 40);
            this.ctlSliceProfileConfig.StyleName = null;
            this.ctlSliceProfileConfig.TabIndex = 2;
            this.ctlSliceProfileConfig.Text = "Configure Slicing Profile";
            // 
            // ctlMachineConfig1
            // 
            this.ctlMachineConfig1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlMachineConfig1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctlMachineConfig1.Location = new System.Drawing.Point(0, 0);
            this.ctlMachineConfig1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctlMachineConfig1.Name = "ctlMachineConfig1";
            this.ctlMachineConfig1.Size = new System.Drawing.Size(256, 197);
            this.ctlMachineConfig1.TabIndex = 2;
            // 
            // ctlToolpathGenConfig1
            // 
            this.ctlToolpathGenConfig1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlToolpathGenConfig1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctlToolpathGenConfig1.Location = new System.Drawing.Point(0, 0);
            this.ctlToolpathGenConfig1.Name = "ctlToolpathGenConfig1";
            this.ctlToolpathGenConfig1.Size = new System.Drawing.Size(226, 218);
            this.ctlToolpathGenConfig1.TabIndex = 1;
            // 
            // pnlMachineConfig
            // 
            this.pnlMachineConfig.Controls.Add(this.ctlMachineConfig1);
            this.pnlMachineConfig.Location = new System.Drawing.Point(528, 162);
            this.pnlMachineConfig.Name = "pnlMachineConfig";
            this.pnlMachineConfig.Size = new System.Drawing.Size(256, 197);
            this.pnlMachineConfig.TabIndex = 3;
            // 
            // pnlToolpathGenConfig
            // 
            this.pnlToolpathGenConfig.Controls.Add(this.ctlToolpathGenConfig1);
            this.pnlToolpathGenConfig.Location = new System.Drawing.Point(40, 109);
            this.pnlToolpathGenConfig.Name = "pnlToolpathGenConfig";
            this.pnlToolpathGenConfig.Size = new System.Drawing.Size(226, 218);
            this.pnlToolpathGenConfig.TabIndex = 4;
            // 
            // ctlMainConfig
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlToolpathGenConfig);
            this.Controls.Add(this.pnlMachineConfig);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "ctlMainConfig";
            this.Size = new System.Drawing.Size(991, 482);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlMachineConfig.ResumeLayout(false);
            this.pnlToolpathGenConfig.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private CustomGUI.ctlTitle ctlMachineConfigView;
        private CustomGUI.ctlTitle ctlSliceProfileConfig;
        private ctlToolpathGenConfig ctlToolpathGenConfig1;
        private ctlMachineConfig ctlMachineConfig1;
        private System.Windows.Forms.Panel pnlMachineConfig;
        private System.Windows.Forms.Panel pnlToolpathGenConfig;
    }
}
