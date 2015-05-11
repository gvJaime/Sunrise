namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlView
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
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.ctlTitle1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttEnableTransparency = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttShowSlice = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttShowConsole = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttBoundingBox = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttSelOutline = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttSelColor = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.Navy;
            this.flowLayoutPanel2.Controls.Add(this.ctlTitle1);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel3);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(235, 206);
            this.flowLayoutPanel2.TabIndex = 24;
            // 
            // ctlTitle1
            // 
            this.ctlTitle1.Checked = false;
            this.ctlTitle1.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttStateTrig;
            this.ctlTitle1.Gapx = 0;
            this.ctlTitle1.Gapy = 0;
            this.ctlTitle1.GLBackgroundImage = null;
            this.ctlTitle1.GLVisible = false;
            this.ctlTitle1.GuiAnchor = null;
            this.ctlTitle1.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttView;
            this.ctlTitle1.Location = new System.Drawing.Point(3, 3);
            this.ctlTitle1.Name = "ctlTitle1";
            this.ctlTitle1.Size = new System.Drawing.Size(235, 45);
            this.ctlTitle1.StyleName = null;
            this.ctlTitle1.TabIndex = 6;
            this.ctlTitle1.Text = "View Options";
            this.ctlTitle1.Click += new System.EventHandler(this.ctlTitle1_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.buttEnableTransparency);
            this.flowLayoutPanel1.Controls.Add(this.buttShowSlice);
            this.flowLayoutPanel1.Controls.Add(this.buttShowConsole);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 54);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // buttEnableTransparency
            // 
            this.buttEnableTransparency.BackColor = System.Drawing.Color.Navy;
            this.buttEnableTransparency.Checked = false;
            this.buttEnableTransparency.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttEnableTransparency.Gapx = 5;
            this.buttEnableTransparency.Gapy = 5;
            this.buttEnableTransparency.GLBackgroundImage = null;
            this.buttEnableTransparency.GLImage = null;
            this.buttEnableTransparency.GLVisible = false;
            this.buttEnableTransparency.GuiAnchor = null;
            this.buttEnableTransparency.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttEnableTransparency.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttTransparent;
            this.buttEnableTransparency.Location = new System.Drawing.Point(5, 5);
            this.buttEnableTransparency.Margin = new System.Windows.Forms.Padding(5);
            this.buttEnableTransparency.Name = "buttEnableTransparency";
            this.buttEnableTransparency.Size = new System.Drawing.Size(48, 48);
            this.buttEnableTransparency.StyleName = null;
            this.buttEnableTransparency.TabIndex = 23;
            this.ctlToolTip1.SetToolTip(this.buttEnableTransparency, "Make objects 50%\r\ntransparent");
            this.buttEnableTransparency.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttEnableTransparency.Click += new System.EventHandler(this.buttEnableTransparency_Click);
            // 
            // buttShowSlice
            // 
            this.buttShowSlice.BackColor = System.Drawing.Color.Navy;
            this.buttShowSlice.Checked = false;
            this.buttShowSlice.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttShowSlice.Gapx = 5;
            this.buttShowSlice.Gapy = 5;
            this.buttShowSlice.GLBackgroundImage = null;
            this.buttShowSlice.GLImage = null;
            this.buttShowSlice.GLVisible = false;
            this.buttShowSlice.GuiAnchor = null;
            this.buttShowSlice.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttShowSlice.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttShowslice;
            this.buttShowSlice.Location = new System.Drawing.Point(58, 5);
            this.buttShowSlice.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttShowSlice.Name = "buttShowSlice";
            this.buttShowSlice.Size = new System.Drawing.Size(48, 48);
            this.buttShowSlice.StyleName = null;
            this.buttShowSlice.TabIndex = 24;
            this.ctlToolTip1.SetToolTip(this.buttShowSlice, "Show slice preview \r\non scene");
            this.buttShowSlice.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttShowSlice.Click += new System.EventHandler(this.buttShowSlice_Click);
            // 
            // buttShowConsole
            // 
            this.buttShowConsole.BackColor = System.Drawing.Color.Navy;
            this.buttShowConsole.Checked = true;
            this.buttShowConsole.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttShowConsole.Gapx = 5;
            this.buttShowConsole.Gapy = 5;
            this.buttShowConsole.GLBackgroundImage = null;
            this.buttShowConsole.GLImage = null;
            this.buttShowConsole.GLVisible = false;
            this.buttShowConsole.GuiAnchor = null;
            this.buttShowConsole.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttShowConsole.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttConsole;
            this.buttShowConsole.Location = new System.Drawing.Point(111, 5);
            this.buttShowConsole.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttShowConsole.Name = "buttShowConsole";
            this.buttShowConsole.Size = new System.Drawing.Size(48, 48);
            this.buttShowConsole.StyleName = null;
            this.buttShowConsole.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttShowConsole, "Show Console");
            this.buttShowConsole.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttShowConsole.Click += new System.EventHandler(this.buttShowConsole_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel3.Controls.Add(this.buttBoundingBox);
            this.flowLayoutPanel3.Controls.Add(this.buttSelOutline);
            this.flowLayoutPanel3.Controls.Add(this.buttSelColor);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 115);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel3.TabIndex = 5;
            // 
            // buttBoundingBox
            // 
            this.buttBoundingBox.BackColor = System.Drawing.Color.Navy;
            this.buttBoundingBox.Checked = false;
            this.buttBoundingBox.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttBoundingBox.Gapx = 5;
            this.buttBoundingBox.Gapy = 5;
            this.buttBoundingBox.GLBackgroundImage = null;
            this.buttBoundingBox.GLImage = null;
            this.buttBoundingBox.GLVisible = false;
            this.buttBoundingBox.GuiAnchor = null;
            this.buttBoundingBox.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttBoundingBox.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttBoundingBox;
            this.buttBoundingBox.Location = new System.Drawing.Point(5, 5);
            this.buttBoundingBox.Margin = new System.Windows.Forms.Padding(5);
            this.buttBoundingBox.Name = "buttBoundingBox";
            this.buttBoundingBox.Size = new System.Drawing.Size(48, 48);
            this.buttBoundingBox.StyleName = null;
            this.buttBoundingBox.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttBoundingBox, "Mark selected objects\r\nwith a bounding box");
            this.buttBoundingBox.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttBoundingBox.Click += new System.EventHandler(this.buttBoundingBox_Click);
            // 
            // buttSelOutline
            // 
            this.buttSelOutline.BackColor = System.Drawing.Color.Navy;
            this.buttSelOutline.Checked = false;
            this.buttSelOutline.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttSelOutline.Gapx = 5;
            this.buttSelOutline.Gapy = 5;
            this.buttSelOutline.GLBackgroundImage = null;
            this.buttSelOutline.GLImage = null;
            this.buttSelOutline.GLVisible = false;
            this.buttSelOutline.GuiAnchor = null;
            this.buttSelOutline.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSelOutline.Image = global::UV_DLP_3D_Printer.Properties.Resources.SelOutline;
            this.buttSelOutline.Location = new System.Drawing.Point(58, 5);
            this.buttSelOutline.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttSelOutline.Name = "buttSelOutline";
            this.buttSelOutline.Size = new System.Drawing.Size(48, 48);
            this.buttSelOutline.StyleName = null;
            this.buttSelOutline.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.buttSelOutline, "Mark selected objects\r\nwith an outline");
            this.buttSelOutline.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSelOutline.Click += new System.EventHandler(this.buttSelOutline_Click);
            // 
            // buttSelColor
            // 
            this.buttSelColor.BackColor = System.Drawing.Color.Navy;
            this.buttSelColor.Checked = false;
            this.buttSelColor.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttSelColor.Gapx = 5;
            this.buttSelColor.Gapy = 5;
            this.buttSelColor.GLBackgroundImage = null;
            this.buttSelColor.GLImage = null;
            this.buttSelColor.GLVisible = false;
            this.buttSelColor.GuiAnchor = null;
            this.buttSelColor.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSelColor.Image = global::UV_DLP_3D_Printer.Properties.Resources.SelShaded;
            this.buttSelColor.Location = new System.Drawing.Point(111, 5);
            this.buttSelColor.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttSelColor.Name = "buttSelColor";
            this.buttSelColor.Size = new System.Drawing.Size(48, 48);
            this.buttSelColor.StyleName = null;
            this.buttSelColor.TabIndex = 27;
            this.ctlToolTip1.SetToolTip(this.buttSelColor, "Mark selected objects\r\nwith a different shade");
            this.buttSelColor.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSelColor.Click += new System.EventHandler(this.buttSelColor_Click);
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 500;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // ctlView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "ctlView";
            this.Size = new System.Drawing.Size(235, 206);
            this.Resize += new System.EventHandler(this.ctlView_Resize);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ctlImageButton buttEnableTransparency;
        private ctlImageButton buttShowSlice;
        private ctlImageButton buttBoundingBox;
        private ctlToolTip ctlToolTip1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private ctlImageButton buttShowConsole;
        private ctlTitle ctlTitle1;
        private ctlImageButton buttSelOutline;
        private ctlImageButton buttSelColor;
    }
}
