namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlUserParamEdit
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
            this.lvParams = new System.Windows.Forms.ListView();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdDelParam = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.cmdNewParam = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.SuspendLayout();
            // 
            // lvParams
            // 
            this.lvParams.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvParams.Location = new System.Drawing.Point(8, 28);
            this.lvParams.Name = "lvParams";
            this.lvParams.Size = new System.Drawing.Size(391, 97);
            this.lvParams.TabIndex = 0;
            this.lvParams.UseCompatibleStateImageBehavior = false;
            this.lvParams.View = System.Windows.Forms.View.Details;
            this.lvParams.SelectedIndexChanged += new System.EventHandler(this.lvParams_SelectedIndexChanged);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(8, 150);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(180, 22);
            this.txtName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "User Parameters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 69;
            this.label3.Text = "Value";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(8, 196);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtValue.Size = new System.Drawing.Size(391, 113);
            this.txtValue.TabIndex = 68;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 169;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 217;
            // 
            // cmdDelParam
            // 
            this.cmdDelParam.BackColor = System.Drawing.Color.CornflowerBlue;
            this.cmdDelParam.Checked = false;
            this.cmdDelParam.CheckImage = null;
            this.cmdDelParam.Gapx = 5;
            this.cmdDelParam.Gapy = 5;
            this.cmdDelParam.GLBackgroundImage = null;
            this.cmdDelParam.GLImage = null;
            this.cmdDelParam.GLVisible = false;
            this.cmdDelParam.GuiAnchor = null;
            this.cmdDelParam.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.cmdDelParam.Image = global::UV_DLP_3D_Printer.Properties.Resources.butMinus;
            this.cmdDelParam.Location = new System.Drawing.Point(406, 28);
            this.cmdDelParam.Margin = new System.Windows.Forms.Padding(4);
            this.cmdDelParam.Name = "cmdDelParam";
            this.cmdDelParam.Size = new System.Drawing.Size(35, 32);
            this.cmdDelParam.StyleName = null;
            this.cmdDelParam.TabIndex = 67;
            this.cmdDelParam.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.cmdDelParam.Click += new System.EventHandler(this.cmdDelParam_Click);
            // 
            // cmdNewParam
            // 
            this.cmdNewParam.BackColor = System.Drawing.Color.CornflowerBlue;
            this.cmdNewParam.Checked = false;
            this.cmdNewParam.CheckImage = null;
            this.cmdNewParam.Gapx = 5;
            this.cmdNewParam.Gapy = 5;
            this.cmdNewParam.GLBackgroundImage = null;
            this.cmdNewParam.GLImage = null;
            this.cmdNewParam.GLVisible = false;
            this.cmdNewParam.GuiAnchor = null;
            this.cmdNewParam.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.cmdNewParam.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPlus;
            this.cmdNewParam.Location = new System.Drawing.Point(195, 150);
            this.cmdNewParam.Margin = new System.Windows.Forms.Padding(4);
            this.cmdNewParam.Name = "cmdNewParam";
            this.cmdNewParam.Size = new System.Drawing.Size(35, 32);
            this.cmdNewParam.StyleName = null;
            this.cmdNewParam.TabIndex = 66;
            this.cmdNewParam.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.cmdNewParam.Click += new System.EventHandler(this.cmdNewParam_Click);
            // 
            // ctlUserParamEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.cmdDelParam);
            this.Controls.Add(this.cmdNewParam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lvParams);
            this.Name = "ctlUserParamEdit";
            this.Size = new System.Drawing.Size(494, 332);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvParams;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private CustomGUI.ctlImageButton cmdDelParam;
        private CustomGUI.ctlImageButton cmdNewParam;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;

    }
}
