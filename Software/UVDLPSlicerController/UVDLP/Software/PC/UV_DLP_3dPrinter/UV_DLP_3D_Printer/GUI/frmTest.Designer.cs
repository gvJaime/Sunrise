namespace UV_DLP_3D_Printer.GUI
{
    partial class frmTest
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
            this.ctlManualControl1 = new UV_DLP_3D_Printer.GUI.Controls.ctlManualControl();
            this.SuspendLayout();
            // 
            // ctlManualControl1
            // 
            this.ctlManualControl1.Gapx = 0;
            this.ctlManualControl1.Gapy = 0;
            this.ctlManualControl1.GLBackgroundImage = null;
            this.ctlManualControl1.GLVisible = false;
            this.ctlManualControl1.GuiAnchor = null;
            this.ctlManualControl1.Location = new System.Drawing.Point(0, 0);
            this.ctlManualControl1.Name = "ctlManualControl1";
            this.ctlManualControl1.Size = new System.Drawing.Size(213, 375);
            this.ctlManualControl1.StyleName = null;
            this.ctlManualControl1.TabIndex = 0;
            this.ctlManualControl1.SizeChanged += new System.EventHandler(this.ctlManualControl1_SizeChanged);
            // 
            // frmTest
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(667, 466);
            this.Controls.Add(this.ctlManualControl1);
            this.Name = "frmTest";
            this.Text = "frmTest";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ctlManualControl ctlManualControl1;

    }
}