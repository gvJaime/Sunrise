namespace UV_DLP_3D_Printer.GUI.Controls.ManualControls
{
    partial class ctlGCodeManual
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
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtGCode = new System.Windows.Forms.TextBox();
            this.cmdSendGCode = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSent = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtReceived = new System.Windows.Forms.TextBox();
            this.cmdClear = new System.Windows.Forms.Button();
            this.checkLogAll = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel5.Controls.Add(this.lblTitle);
            this.flowLayoutPanel5.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Padding = new System.Windows.Forms.Padding(3);
            this.flowLayoutPanel5.Size = new System.Drawing.Size(270, 334);
            this.flowLayoutPanel5.TabIndex = 28;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(6, 3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(257, 20);
            this.lblTitle.TabIndex = 66;
            this.lblTitle.Text = "Manual GCode Commands";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.flowLayoutPanel1.Controls.Add(this.txtGCode);
            this.flowLayoutPanel1.Controls.Add(this.cmdSendGCode);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.txtSent);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.txtReceived);
            this.flowLayoutPanel1.Controls.Add(this.cmdClear);
            this.flowLayoutPanel1.Controls.Add(this.checkLogAll);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 23);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(259, 308);
            this.flowLayoutPanel1.TabIndex = 67;
            // 
            // txtGCode
            // 
            this.txtGCode.Location = new System.Drawing.Point(3, 2);
            this.txtGCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGCode.Name = "txtGCode";
            this.txtGCode.Size = new System.Drawing.Size(171, 20);
            this.txtGCode.TabIndex = 52;
            // 
            // cmdSendGCode
            // 
            this.cmdSendGCode.Location = new System.Drawing.Point(180, 2);
            this.cmdSendGCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdSendGCode.Name = "cmdSendGCode";
            this.cmdSendGCode.Size = new System.Drawing.Size(75, 23);
            this.cmdSendGCode.TabIndex = 53;
            this.cmdSendGCode.Text = "Send!";
            this.cmdSendGCode.UseVisualStyleBackColor = true;
            this.cmdSendGCode.Click += new System.EventHandler(this.cmdSendGCode_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 18);
            this.label1.TabIndex = 67;
            this.label1.Text = "Sent";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSent
            // 
            this.txtSent.BackColor = System.Drawing.Color.White;
            this.txtSent.Location = new System.Drawing.Point(3, 47);
            this.txtSent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSent.Multiline = true;
            this.txtSent.Name = "txtSent";
            this.txtSent.ReadOnly = true;
            this.txtSent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSent.Size = new System.Drawing.Size(252, 85);
            this.txtSent.TabIndex = 54;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(3, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(252, 20);
            this.label2.TabIndex = 68;
            this.label2.Text = "Received";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtReceived
            // 
            this.txtReceived.BackColor = System.Drawing.Color.White;
            this.txtReceived.Location = new System.Drawing.Point(3, 156);
            this.txtReceived.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReceived.Multiline = true;
            this.txtReceived.Name = "txtReceived";
            this.txtReceived.ReadOnly = true;
            this.txtReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceived.Size = new System.Drawing.Size(252, 125);
            this.txtReceived.TabIndex = 69;
            // 
            // cmdClear
            // 
            this.cmdClear.Location = new System.Drawing.Point(3, 285);
            this.cmdClear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(75, 22);
            this.cmdClear.TabIndex = 70;
            this.cmdClear.Text = "Clear";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // checkLogAll
            // 
            this.checkLogAll.AutoSize = true;
            this.checkLogAll.Location = new System.Drawing.Point(84, 286);
            this.checkLogAll.Name = "checkLogAll";
            this.checkLogAll.Size = new System.Drawing.Size(116, 17);
            this.checkLogAll.TabIndex = 71;
            this.checkLogAll.Text = "Log all GCode sent";
            this.checkLogAll.UseVisualStyleBackColor = true;
            // 
            // ctlGCodeManual
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel5);
            this.Name = "ctlGCodeManual";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(276, 340);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox txtGCode;
        private System.Windows.Forms.Button cmdSendGCode;
        private System.Windows.Forms.TextBox txtSent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtReceived;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.CheckBox checkLogAll;
    }
}
