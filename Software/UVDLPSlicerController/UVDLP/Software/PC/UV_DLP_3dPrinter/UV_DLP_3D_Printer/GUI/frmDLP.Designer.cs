namespace UV_DLP_3D_Printer
{
    partial class frmDLP
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
            this.picDLP = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picDLP)).BeginInit();
            this.SuspendLayout();
            // 
            // picDLP
            // 
            this.picDLP.BackColor = System.Drawing.Color.Black;
            this.picDLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picDLP.Location = new System.Drawing.Point(0, 0);
            this.picDLP.Name = "picDLP";
            this.picDLP.Size = new System.Drawing.Size(642, 399);
            this.picDLP.TabIndex = 0;
            this.picDLP.TabStop = false;
            // 
            // frmDLP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 399);
            this.Controls.Add(this.picDLP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmDLP";
            this.Text = "frmDLP";
            ((System.ComponentModel.ISupportInitialize)(this.picDLP)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picDLP;
    }
}