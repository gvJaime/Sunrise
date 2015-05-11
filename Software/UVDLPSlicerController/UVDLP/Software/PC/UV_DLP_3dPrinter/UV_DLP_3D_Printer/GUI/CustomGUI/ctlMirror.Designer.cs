namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlMirror
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
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.ctlTitle1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.lblX);
            this.flowLayoutPanel1.Controls.Add(this.lblY);
            this.flowLayoutPanel1.Controls.Add(this.lblZ);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 54);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(224, 59);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // lblX
            // 
            this.lblX.BackColor = System.Drawing.Color.Navy;
            this.lblX.Font = new System.Drawing.Font("Courier New", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblX.ForeColor = System.Drawing.Color.White;
            this.lblX.Location = new System.Drawing.Point(3, 3);
            this.lblX.Margin = new System.Windows.Forms.Padding(3);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(50, 50);
            this.lblX.TabIndex = 9;
            this.lblX.Text = "X";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblX.Click += new System.EventHandler(this.lblX_Click);
            // 
            // lblY
            // 
            this.lblY.BackColor = System.Drawing.Color.Navy;
            this.lblY.Font = new System.Drawing.Font("Courier New", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblY.ForeColor = System.Drawing.Color.White;
            this.lblY.Location = new System.Drawing.Point(59, 3);
            this.lblY.Margin = new System.Windows.Forms.Padding(3);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(50, 50);
            this.lblY.TabIndex = 10;
            this.lblY.Text = "Y";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblY.Click += new System.EventHandler(this.lblY_Click);
            // 
            // lblZ
            // 
            this.lblZ.BackColor = System.Drawing.Color.Navy;
            this.lblZ.Font = new System.Drawing.Font("Courier New", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZ.ForeColor = System.Drawing.Color.White;
            this.lblZ.Location = new System.Drawing.Point(115, 3);
            this.lblZ.Margin = new System.Windows.Forms.Padding(3);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(50, 50);
            this.lblZ.TabIndex = 11;
            this.lblZ.Text = "Z";
            this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblZ.Click += new System.EventHandler(this.lblZ_Click);
            // 
            // ctlTitle1
            // 
            this.ctlTitle1.BackColor = System.Drawing.Color.DarkBlue;
            this.ctlTitle1.Checked = false;
            this.ctlTitle1.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttStateTrig;
            this.ctlTitle1.Gapx = 0;
            this.ctlTitle1.Gapy = 0;
            this.ctlTitle1.GLBackgroundImage = null;
            this.ctlTitle1.GLImage = null;
            this.ctlTitle1.GLVisible = false;
            this.ctlTitle1.GuiAnchor = null;
            this.ctlTitle1.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlTitle1.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttMirror;
            this.ctlTitle1.Location = new System.Drawing.Point(3, 3);
            this.ctlTitle1.Name = "ctlTitle1";
            this.ctlTitle1.Size = new System.Drawing.Size(235, 45);
            this.ctlTitle1.StyleName = null;
            this.ctlTitle1.TabIndex = 7;
            this.ctlTitle1.Text = "Mirror";
            this.ctlTitle1.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlTitle1.Click += new System.EventHandler(this.ctlTitle1_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.Navy;
            this.flowLayoutPanel2.Controls.Add(this.ctlTitle1);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(242, 126);
            this.flowLayoutPanel2.TabIndex = 9;
            // 
            // ctlMirror
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "ctlMirror";
            this.Size = new System.Drawing.Size(242, 126);
            this.Resize += new System.EventHandler(this.ctlMirror_Resize);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ctlTitle ctlTitle1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}
