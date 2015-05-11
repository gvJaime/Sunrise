namespace UV_DLP_3D_Printer.GUI
{
    partial class frmPluginManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPluginManager));
            this.label1 = new System.Windows.Forms.Label();
            this.lvplugins = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmLicensed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdLicense = new System.Windows.Forms.Button();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.cmdEnable = new System.Windows.Forms.Button();
            this.clmEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Installed Plug-Ins";
            // 
            // lvplugins
            // 
            this.lvplugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmLicensed,
            this.clmEnabled,
            this.clmVersion,
            this.clmDescription});
            this.lvplugins.FullRowSelect = true;
            this.lvplugins.GridLines = true;
            this.lvplugins.HideSelection = false;
            this.lvplugins.Location = new System.Drawing.Point(12, 30);
            this.lvplugins.MultiSelect = false;
            this.lvplugins.Name = "lvplugins";
            this.lvplugins.Size = new System.Drawing.Size(627, 186);
            this.lvplugins.TabIndex = 3;
            this.lvplugins.UseCompatibleStateImageBehavior = false;
            this.lvplugins.View = System.Windows.Forms.View.Details;
            this.lvplugins.SelectedIndexChanged += new System.EventHandler(this.lvplugins_SelectedIndexChanged);
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            this.clmName.Width = 117;
            // 
            // clmLicensed
            // 
            this.clmLicensed.Text = "Licensed";
            this.clmLicensed.Width = 71;
            // 
            // clmVersion
            // 
            this.clmVersion.Text = "Version";
            this.clmVersion.Width = 80;
            // 
            // clmDescription
            // 
            this.clmDescription.Text = "Description";
            this.clmDescription.Width = 266;
            // 
            // cmdLicense
            // 
            this.cmdLicense.Enabled = false;
            this.cmdLicense.Location = new System.Drawing.Point(16, 271);
            this.cmdLicense.Name = "cmdLicense";
            this.cmdLicense.Size = new System.Drawing.Size(130, 30);
            this.cmdLicense.TabIndex = 4;
            this.cmdLicense.Text = "Enter License";
            this.cmdLicense.UseVisualStyleBackColor = true;
            this.cmdLicense.Click += new System.EventHandler(this.cmdLicense_Click);
            // 
            // txtLicense
            // 
            this.txtLicense.Location = new System.Drawing.Point(166, 275);
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.Size = new System.Drawing.Size(277, 22);
            this.txtLicense.TabIndex = 5;
            // 
            // cmdEnable
            // 
            this.cmdEnable.Location = new System.Drawing.Point(16, 230);
            this.cmdEnable.Name = "cmdEnable";
            this.cmdEnable.Size = new System.Drawing.Size(130, 29);
            this.cmdEnable.TabIndex = 6;
            this.cmdEnable.Text = "Enable";
            this.cmdEnable.UseVisualStyleBackColor = true;
            this.cmdEnable.Click += new System.EventHandler(this.cmdEnable_Click);
            // 
            // clmEnabled
            // 
            this.clmEnabled.Text = "Enabled";
            this.clmEnabled.Width = 74;
            // 
            // frmPluginManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 313);
            this.Controls.Add(this.cmdEnable);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.cmdLicense);
            this.Controls.Add(this.lvplugins);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPluginManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plugin and Licensing Management";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvplugins;
        private System.Windows.Forms.ColumnHeader clmName;
        private System.Windows.Forms.ColumnHeader clmLicensed;
        private System.Windows.Forms.ColumnHeader clmVersion;
        private System.Windows.Forms.ColumnHeader clmDescription;
        private System.Windows.Forms.Button cmdLicense;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.Button cmdEnable;
        private System.Windows.Forms.ColumnHeader clmEnabled;
    }
}