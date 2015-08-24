namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlMCTemp
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
            this.textSetTemp = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox();
            this.SuspendLayout();
            // 
            // textSetTemp
            // 
            this.textSetTemp.BackColor = System.Drawing.Color.RoyalBlue;
            this.textSetTemp.ErrorColor = System.Drawing.Color.Red;
            this.textSetTemp.FloatVal = 0F;
            this.textSetTemp.ForeColor = System.Drawing.Color.White;
            this.textSetTemp.IntVal = 0;
            this.textSetTemp.Location = new System.Drawing.Point(10, 196);
            this.textSetTemp.Name = "textSetTemp";
            this.textSetTemp.Size = new System.Drawing.Size(50, 24);
            this.textSetTemp.TabIndex = 0;
            this.textSetTemp.Text = "0";
            this.textSetTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textSetTemp.ValidColor = System.Drawing.Color.White;
            this.textSetTemp.ValueType = UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox.EValueType.Int;
            this.textSetTemp.TextChanged += new System.EventHandler(this.textSetTemp_TextChanged);
            // 
            // ctlMCTemp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.textSetTemp);
            this.Name = "ctlMCTemp";
            this.Size = new System.Drawing.Size(70, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private ctlTextBox textSetTemp;

    }
}
