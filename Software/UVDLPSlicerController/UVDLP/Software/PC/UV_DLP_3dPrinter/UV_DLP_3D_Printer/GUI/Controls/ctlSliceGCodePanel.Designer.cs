namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlSliceGCodePanel
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
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.ctlTitleViewSlice = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.ctlTitleViewGCode = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.ctlGcodeView1 = new UV_DLP_3D_Printer.GUI.Controls.ctlGcodeView();
            this.ctlSliceView1 = new UV_DLP_3D_Printer.GUI.Controls.ctlSliceView();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(967, 50);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(10, 445);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel2.Controls.Add(this.ctlTitleViewSlice);
            this.flowLayoutPanel2.Controls.Add(this.ctlTitleViewGCode);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(977, 50);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // ctlTitleViewSlice
            // 
            this.ctlTitleViewSlice.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlTitleViewSlice.Checked = false;
            this.ctlTitleViewSlice.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.ctlTitleViewSlice.Gapx = 0;
            this.ctlTitleViewSlice.Gapy = 0;
            this.ctlTitleViewSlice.GLBackgroundImage = null;
            this.ctlTitleViewSlice.GLVisible = false;
            this.ctlTitleViewSlice.GuiAnchor = null;
            this.ctlTitleViewSlice.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttViewSlice;
            this.ctlTitleViewSlice.Location = new System.Drawing.Point(3, 3);
            this.ctlTitleViewSlice.Name = "ctlTitleViewSlice";
            this.ctlTitleViewSlice.OnClickCallback = "ShowSliceView";
            this.ctlTitleViewSlice.Size = new System.Drawing.Size(283, 40);
            this.ctlTitleViewSlice.StyleName = null;
            this.ctlTitleViewSlice.TabIndex = 0;
            this.ctlTitleViewSlice.Text = "Slice View";
            // 
            // ctlTitleViewGCode
            // 
            this.ctlTitleViewGCode.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlTitleViewGCode.Checked = false;
            this.ctlTitleViewGCode.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.ctlTitleViewGCode.Gapx = 0;
            this.ctlTitleViewGCode.Gapy = 0;
            this.ctlTitleViewGCode.GLBackgroundImage = null;
            this.ctlTitleViewGCode.GLVisible = false;
            this.ctlTitleViewGCode.GuiAnchor = null;
            this.ctlTitleViewGCode.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttViewGcode;
            this.ctlTitleViewGCode.Location = new System.Drawing.Point(292, 3);
            this.ctlTitleViewGCode.Name = "ctlTitleViewGCode";
            this.ctlTitleViewGCode.OnClickCallback = "ShowGCodeView";
            this.ctlTitleViewGCode.Size = new System.Drawing.Size(283, 40);
            this.ctlTitleViewGCode.StyleName = null;
            this.ctlTitleViewGCode.TabIndex = 1;
            this.ctlTitleViewGCode.Text = "GCode View";
            // 
            // ctlGcodeView1
            // 
            this.ctlGcodeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlGcodeView1.Location = new System.Drawing.Point(0, 50);
            this.ctlGcodeView1.Name = "ctlGcodeView1";
            this.ctlGcodeView1.Size = new System.Drawing.Size(967, 445);
            this.ctlGcodeView1.TabIndex = 2;
            // 
            // ctlSliceView1
            // 
            this.ctlSliceView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctlSliceView1.Location = new System.Drawing.Point(0, 0);
            this.ctlSliceView1.Name = "ctlSliceView1";
            this.ctlSliceView1.Size = new System.Drawing.Size(977, 495);
            this.ctlSliceView1.TabIndex = 3;
            // 
            // ctlSliceGCodePanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.ctlGcodeView1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.ctlSliceView1);
            this.Name = "ctlSliceGCodePanel";
            this.Size = new System.Drawing.Size(977, 495);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private CustomGUI.ctlTitle ctlTitleViewSlice;
        private CustomGUI.ctlTitle ctlTitleViewGCode;
        private ctlGcodeView ctlGcodeView1;
        private ctlSliceView ctlSliceView1;
    }
}
