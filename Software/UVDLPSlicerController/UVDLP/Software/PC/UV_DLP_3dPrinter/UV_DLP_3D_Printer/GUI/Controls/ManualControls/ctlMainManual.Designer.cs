namespace UV_DLP_3D_Printer.GUI.Controls.ManualControls
{
    partial class ctlMainManual
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
            this.ctlManualControl1 = new UV_DLP_3D_Printer.GUI.Controls.ctlManualControl();
            this.SuspendLayout();
            // 
            // ctlManualControl1
            // 
            this.ctlManualControl1.ComponentSupport = "XYZPG";
            this.ctlManualControl1.Gapx = 0;
            this.ctlManualControl1.Gapy = 0;
            this.ctlManualControl1.GLBackgroundImage = null;
            this.ctlManualControl1.GLVisible = false;
            this.ctlManualControl1.GuiAnchor = null;
            this.ctlManualControl1.Location = new System.Drawing.Point(3, 3);
            this.ctlManualControl1.Name = "ctlManualControl1";
            this.ctlManualControl1.Size = new System.Drawing.Size(771, 355);
            this.ctlManualControl1.StyleName = "DefaultControl";
            this.ctlManualControl1.TabIndex = 0;
            // 
            // ctlMainManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.ctlManualControl1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ctlMainManual";
            this.Size = new System.Drawing.Size(945, 627);
            this.ResumeLayout(false);

        }

        #endregion

        private ctlManualControl ctlManualControl1;

    }
}
