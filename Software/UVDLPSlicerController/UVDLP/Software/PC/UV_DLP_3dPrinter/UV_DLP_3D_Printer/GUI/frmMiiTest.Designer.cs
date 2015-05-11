namespace UV_DLP_3D_Printer.GUI
{
    partial class frmMiiTest
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
            this.sMiiCtrl1 = new UV_DLP_3D_Printer.GUI.CustomGUI.sMiiCtrl();
            this.SuspendLayout();
            // 
            // sMiiCtrl1
            // 
            this.sMiiCtrl1.Location = new System.Drawing.Point(12, 12);
            this.sMiiCtrl1.Name = "sMiiCtrl1";
            this.sMiiCtrl1.Size = new System.Drawing.Size(380, 255);
            this.sMiiCtrl1.TabIndex = 0;
            // 
            // frmMiiTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 279);
            this.Controls.Add(this.sMiiCtrl1);
            this.Name = "frmMiiTest";
            this.Text = "Mii Manual Control";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomGUI.sMiiCtrl sMiiCtrl1;
    }
}