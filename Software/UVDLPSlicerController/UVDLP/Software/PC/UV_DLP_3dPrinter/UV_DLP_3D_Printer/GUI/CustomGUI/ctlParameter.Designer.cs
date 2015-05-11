namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlParameter
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
            this.ctlParam = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ctlParam)).BeginInit();
            this.SuspendLayout();
            // 
            // ctlParam
            // 
            this.ctlParam.Location = new System.Drawing.Point(76, 13);
            this.ctlParam.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ctlParam.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.ctlParam.Name = "ctlParam";
            this.ctlParam.Size = new System.Drawing.Size(71, 20);
            this.ctlParam.TabIndex = 0;
            this.ctlParam.ValueChanged += new System.EventHandler(this.ctlParam_ValueChanged);
            // 
            // ctlParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ctlParam);
            this.FrameColor = System.Drawing.Color.RoyalBlue;
            this.Name = "ctlParameter";
            this.Size = new System.Drawing.Size(150, 47);
            ((System.ComponentModel.ISupportInitialize)(this.ctlParam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown ctlParam;


    }
}
