namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlSupports
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
            this.progressTitle = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlProgress();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttGenBase = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttAddSupport = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttAutoSupport = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.chkSupportAllScene = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.panelSuppotShape = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numFB1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.numFB = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.numFT = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.numHB = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.numHT = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.pictureSupport = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkDownPolys = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.numDownAngle = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.numGap = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.cmbSupType = new System.Windows.Forms.ComboBox();
            this.chkOnlyDownward = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.numY = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.numX = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelAutoSup = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmdRemoveSupports = new System.Windows.Forms.Button();
            this.lbSupports = new System.Windows.Forms.ListView();
            this.Supports = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label9 = new System.Windows.Forms.Label();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            this.ctlTitle1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTitle();
            this.flowLayoutPanel1.SuspendLayout();
            this.panelSuppotShape.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSupport)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressTitle
            // 
            this.progressTitle.BackColor = System.Drawing.Color.MidnightBlue;
            this.progressTitle.BarColor = System.Drawing.Color.RoyalBlue;
            this.progressTitle.BorderThickness = 2;
            this.progressTitle.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.progressTitle.ForeColor = System.Drawing.Color.White;
            this.progressTitle.Location = new System.Drawing.Point(7, 45);
            this.progressTitle.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.progressTitle.Maximum = 100;
            this.progressTitle.Minimum = 0;
            this.progressTitle.Name = "progressTitle";
            this.progressTitle.Size = new System.Drawing.Size(298, 25);
            this.progressTitle.TabIndex = 0;
            this.progressTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.progressTitle.Value = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.buttGenBase);
            this.flowLayoutPanel1.Controls.Add(this.buttAddSupport);
            this.flowLayoutPanel1.Controls.Add(this.buttAutoSupport);
            this.flowLayoutPanel1.Controls.Add(this.chkSupportAllScene);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(8, 71);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(297, 44);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // buttGenBase
            // 
            this.buttGenBase.BackColor = System.Drawing.Color.Navy;
            this.buttGenBase.Checked = false;
            this.buttGenBase.CheckImage = null;
            this.buttGenBase.Gapx = 5;
            this.buttGenBase.Gapy = 5;
            this.buttGenBase.GLBackgroundImage = null;
            this.buttGenBase.GLImage = null;
            this.buttGenBase.GLVisible = false;
            this.buttGenBase.GuiAnchor = null;
            this.buttGenBase.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttGenBase.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSupBase;
            this.buttGenBase.Location = new System.Drawing.Point(5, 5);
            this.buttGenBase.Margin = new System.Windows.Forms.Padding(5);
            this.buttGenBase.Name = "buttGenBase";
            this.buttGenBase.Size = new System.Drawing.Size(40, 40);
            this.buttGenBase.StyleName = null;
            this.buttGenBase.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttGenBase, "Add Support Base Plate");
            this.buttGenBase.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttGenBase.Click += new System.EventHandler(this.buttGenBase_Click);
            // 
            // buttAddSupport
            // 
            this.buttAddSupport.BackColor = System.Drawing.Color.Navy;
            this.buttAddSupport.Checked = false;
            this.buttAddSupport.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddSupport.Gapx = 5;
            this.buttAddSupport.Gapy = 5;
            this.buttAddSupport.GLBackgroundImage = null;
            this.buttAddSupport.GLImage = null;
            this.buttAddSupport.GLVisible = false;
            this.buttAddSupport.GuiAnchor = null;
            this.buttAddSupport.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddSupport.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttAddSupport;
            this.buttAddSupport.Location = new System.Drawing.Point(50, 5);
            this.buttAddSupport.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttAddSupport.Name = "buttAddSupport";
            this.buttAddSupport.Size = new System.Drawing.Size(40, 40);
            this.buttAddSupport.StyleName = null;
            this.buttAddSupport.TabIndex = 23;
            this.ctlToolTip1.SetToolTip(this.buttAddSupport, "Add Support");
            this.buttAddSupport.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddSupport.Click += new System.EventHandler(this.buttAddSupport_Click);
            // 
            // buttAutoSupport
            // 
            this.buttAutoSupport.BackColor = System.Drawing.Color.Navy;
            this.buttAutoSupport.Checked = false;
            this.buttAutoSupport.CheckImage = null;
            this.buttAutoSupport.Gapx = 5;
            this.buttAutoSupport.Gapy = 5;
            this.buttAutoSupport.GLBackgroundImage = null;
            this.buttAutoSupport.GLImage = null;
            this.buttAutoSupport.GLVisible = false;
            this.buttAutoSupport.GuiAnchor = null;
            this.buttAutoSupport.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAutoSupport.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttAutoSupport;
            this.buttAutoSupport.Location = new System.Drawing.Point(95, 5);
            this.buttAutoSupport.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttAutoSupport.Name = "buttAutoSupport";
            this.buttAutoSupport.Size = new System.Drawing.Size(40, 40);
            this.buttAutoSupport.StyleName = null;
            this.buttAutoSupport.TabIndex = 24;
            this.ctlToolTip1.SetToolTip(this.buttAutoSupport, "Generate Automatic Supports");
            this.buttAutoSupport.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAutoSupport.Click += new System.EventHandler(this.buttAutoSupport_Click);
            // 
            // chkSupportAllScene
            // 
            this.chkSupportAllScene.BackColor = System.Drawing.Color.Navy;
            this.chkSupportAllScene.Checked = true;
            this.chkSupportAllScene.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.chkSupportAllScene.Gapx = 5;
            this.chkSupportAllScene.Gapy = 5;
            this.chkSupportAllScene.GLBackgroundImage = null;
            this.chkSupportAllScene.GLImage = null;
            this.chkSupportAllScene.GLVisible = false;
            this.chkSupportAllScene.GuiAnchor = null;
            this.chkSupportAllScene.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.chkSupportAllScene.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttScene;
            this.chkSupportAllScene.Location = new System.Drawing.Point(140, 5);
            this.chkSupportAllScene.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.chkSupportAllScene.Name = "chkSupportAllScene";
            this.chkSupportAllScene.Size = new System.Drawing.Size(40, 40);
            this.chkSupportAllScene.StyleName = null;
            this.chkSupportAllScene.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.chkSupportAllScene, "Checked: Support entire scene\r\nUnchecked: Support selected objects only");
            this.chkSupportAllScene.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // panelSuppotShape
            // 
            this.panelSuppotShape.BackColor = System.Drawing.Color.RoyalBlue;
            this.panelSuppotShape.Controls.Add(this.label1);
            this.panelSuppotShape.Controls.Add(this.numFB1);
            this.panelSuppotShape.Controls.Add(this.numFB);
            this.panelSuppotShape.Controls.Add(this.numFT);
            this.panelSuppotShape.Controls.Add(this.numHB);
            this.panelSuppotShape.Controls.Add(this.numHT);
            this.panelSuppotShape.Controls.Add(this.pictureSupport);
            this.panelSuppotShape.ForeColor = System.Drawing.Color.White;
            this.panelSuppotShape.Location = new System.Drawing.Point(9, 115);
            this.panelSuppotShape.Margin = new System.Windows.Forms.Padding(0);
            this.panelSuppotShape.Name = "panelSuppotShape";
            this.panelSuppotShape.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelSuppotShape.Size = new System.Drawing.Size(200, 188);
            this.panelSuppotShape.TabIndex = 25;
            this.panelSuppotShape.Text = "Support Parameters";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label1.Location = new System.Drawing.Point(3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 21);
            this.label1.TabIndex = 28;
            this.label1.Text = "Support shape";
            // 
            // numFB1
            // 
            this.numFB1.BackColor = System.Drawing.Color.RoyalBlue;
            this.numFB1.ButtonsColor = System.Drawing.Color.Navy;
            this.numFB1.Checked = false;
            this.numFB1.ErrorColor = System.Drawing.Color.Red;
            this.numFB1.FloatVal = 1F;
            this.numFB1.Gapx = 5;
            this.numFB1.Gapy = 5;
            this.numFB1.GLBackgroundImage = null;
            this.numFB1.GLVisible = false;
            this.numFB1.GuiAnchor = null;
            this.numFB1.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFB1.Increment = 0.1F;
            this.numFB1.IntVal = 1;
            this.numFB1.IsFloat = true;
            this.numFB1.Location = new System.Drawing.Point(115, 160);
            this.numFB1.Margin = new System.Windows.Forms.Padding(4);
            this.numFB1.MaxFloat = 20F;
            this.numFB1.MaxInt = 1000;
            this.numFB1.MinFloat = 0.1F;
            this.numFB1.MinimumSize = new System.Drawing.Size(20, 5);
            this.numFB1.MinInt = 1;
            this.numFB1.Name = "numFB1";
            this.numFB1.Size = new System.Drawing.Size(79, 21);
            this.numFB1.StyleName = null;
            this.numFB1.TabIndex = 27;
            this.numFB1.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numFB1, "Foot Bottom intra (mm)");
            this.numFB1.ValidColor = System.Drawing.Color.White;
            this.numFB1.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFB1.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            this.numFB1.Enter += new System.EventHandler(this.numFB1_Enter);
            // 
            // numFB
            // 
            this.numFB.BackColor = System.Drawing.Color.RoyalBlue;
            this.numFB.ButtonsColor = System.Drawing.Color.Navy;
            this.numFB.Checked = false;
            this.numFB.ErrorColor = System.Drawing.Color.Red;
            this.numFB.FloatVal = 1F;
            this.numFB.Gapx = 5;
            this.numFB.Gapy = 5;
            this.numFB.GLBackgroundImage = null;
            this.numFB.GLVisible = false;
            this.numFB.GuiAnchor = null;
            this.numFB.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFB.Increment = 0.1F;
            this.numFB.IntVal = 1;
            this.numFB.IsFloat = true;
            this.numFB.Location = new System.Drawing.Point(115, 133);
            this.numFB.Margin = new System.Windows.Forms.Padding(4);
            this.numFB.MaxFloat = 20F;
            this.numFB.MaxInt = 1000;
            this.numFB.MinFloat = 0.1F;
            this.numFB.MinimumSize = new System.Drawing.Size(20, 5);
            this.numFB.MinInt = 1;
            this.numFB.Name = "numFB";
            this.numFB.Size = new System.Drawing.Size(79, 21);
            this.numFB.StyleName = null;
            this.numFB.TabIndex = 26;
            this.numFB.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numFB, "Foot Bottom (mm)");
            this.numFB.ValidColor = System.Drawing.Color.White;
            this.numFB.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFB.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            this.numFB.Enter += new System.EventHandler(this.numFB_Enter);
            // 
            // numFT
            // 
            this.numFT.BackColor = System.Drawing.Color.RoyalBlue;
            this.numFT.ButtonsColor = System.Drawing.Color.Navy;
            this.numFT.Checked = false;
            this.numFT.ErrorColor = System.Drawing.Color.Red;
            this.numFT.FloatVal = 1F;
            this.numFT.Gapx = 5;
            this.numFT.Gapy = 5;
            this.numFT.GLBackgroundImage = null;
            this.numFT.GLVisible = false;
            this.numFT.GuiAnchor = null;
            this.numFT.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFT.Increment = 0.1F;
            this.numFT.IntVal = 1;
            this.numFT.IsFloat = true;
            this.numFT.Location = new System.Drawing.Point(115, 106);
            this.numFT.Margin = new System.Windows.Forms.Padding(4);
            this.numFT.MaxFloat = 20F;
            this.numFT.MaxInt = 1000;
            this.numFT.MinFloat = 0.1F;
            this.numFT.MinimumSize = new System.Drawing.Size(20, 5);
            this.numFT.MinInt = 1;
            this.numFT.Name = "numFT";
            this.numFT.Size = new System.Drawing.Size(79, 21);
            this.numFT.StyleName = null;
            this.numFT.TabIndex = 25;
            this.numFT.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numFT, "Foot Top (mm)");
            this.numFT.ValidColor = System.Drawing.Color.White;
            this.numFT.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numFT.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numHB
            // 
            this.numHB.BackColor = System.Drawing.Color.RoyalBlue;
            this.numHB.ButtonsColor = System.Drawing.Color.Navy;
            this.numHB.Checked = false;
            this.numHB.ErrorColor = System.Drawing.Color.Red;
            this.numHB.FloatVal = 1F;
            this.numHB.Gapx = 5;
            this.numHB.Gapy = 5;
            this.numHB.GLBackgroundImage = null;
            this.numHB.GLVisible = false;
            this.numHB.GuiAnchor = null;
            this.numHB.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numHB.Increment = 0.1F;
            this.numHB.IntVal = 1;
            this.numHB.IsFloat = true;
            this.numHB.Location = new System.Drawing.Point(115, 58);
            this.numHB.Margin = new System.Windows.Forms.Padding(4);
            this.numHB.MaxFloat = 20F;
            this.numHB.MaxInt = 1000;
            this.numHB.MinFloat = 0.1F;
            this.numHB.MinimumSize = new System.Drawing.Size(20, 5);
            this.numHB.MinInt = 1;
            this.numHB.Name = "numHB";
            this.numHB.Size = new System.Drawing.Size(79, 21);
            this.numHB.StyleName = null;
            this.numHB.TabIndex = 24;
            this.numHB.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numHB, "Head Bottom (mm)");
            this.numHB.ValidColor = System.Drawing.Color.White;
            this.numHB.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numHB.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numHT
            // 
            this.numHT.BackColor = System.Drawing.Color.RoyalBlue;
            this.numHT.ButtonsColor = System.Drawing.Color.Navy;
            this.numHT.Checked = false;
            this.numHT.ErrorColor = System.Drawing.Color.Red;
            this.numHT.FloatVal = 1F;
            this.numHT.Gapx = 5;
            this.numHT.Gapy = 5;
            this.numHT.GLBackgroundImage = null;
            this.numHT.GLVisible = false;
            this.numHT.GuiAnchor = null;
            this.numHT.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numHT.Increment = 0.1F;
            this.numHT.IntVal = 1;
            this.numHT.IsFloat = true;
            this.numHT.Location = new System.Drawing.Point(115, 31);
            this.numHT.Margin = new System.Windows.Forms.Padding(4);
            this.numHT.MaxFloat = 20F;
            this.numHT.MaxInt = 1000;
            this.numHT.MinFloat = 0.1F;
            this.numHT.MinimumSize = new System.Drawing.Size(20, 5);
            this.numHT.MinInt = 1;
            this.numHT.Name = "numHT";
            this.numHT.Size = new System.Drawing.Size(79, 21);
            this.numHT.StyleName = null;
            this.numHT.TabIndex = 23;
            this.numHT.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numHT, "Head Top (mm)");
            this.numHT.ValidColor = System.Drawing.Color.White;
            this.numHT.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numHT.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // pictureSupport
            // 
            this.pictureSupport.Location = new System.Drawing.Point(3, 31);
            this.pictureSupport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureSupport.Name = "pictureSupport";
            this.pictureSupport.Size = new System.Drawing.Size(108, 150);
            this.pictureSupport.TabIndex = 12;
            this.pictureSupport.TabStop = false;
            this.pictureSupport.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureSupport_Paint);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.panel1.Controls.Add(this.chkDownPolys);
            this.panel1.Controls.Add(this.numDownAngle);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(210, 115);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(95, 188);
            this.panel1.TabIndex = 0;
            // 
            // chkDownPolys
            // 
            this.chkDownPolys.BackColor = System.Drawing.Color.Navy;
            this.chkDownPolys.Checked = false;
            this.chkDownPolys.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.chkDownPolys.Gapx = 5;
            this.chkDownPolys.Gapy = 5;
            this.chkDownPolys.GLBackgroundImage = null;
            this.chkDownPolys.GLImage = null;
            this.chkDownPolys.GLVisible = false;
            this.chkDownPolys.GuiAnchor = null;
            this.chkDownPolys.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.chkDownPolys.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttViewDown;
            this.chkDownPolys.Location = new System.Drawing.Point(7, 32);
            this.chkDownPolys.Margin = new System.Windows.Forms.Padding(5);
            this.chkDownPolys.Name = "chkDownPolys";
            this.chkDownPolys.Size = new System.Drawing.Size(32, 32);
            this.chkDownPolys.StyleName = null;
            this.chkDownPolys.TabIndex = 31;
            this.ctlToolTip1.SetToolTip(this.chkDownPolys, "Show downward facing \r\nsurfaces by angle");
            this.chkDownPolys.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.chkDownPolys.Click += new System.EventHandler(this.chkDownPolys_Click);
            // 
            // numDownAngle
            // 
            this.numDownAngle.BackColor = System.Drawing.Color.RoyalBlue;
            this.numDownAngle.ButtonsColor = System.Drawing.Color.Navy;
            this.numDownAngle.Checked = false;
            this.numDownAngle.ErrorColor = System.Drawing.Color.Red;
            this.numDownAngle.FloatVal = 1000F;
            this.numDownAngle.Gapx = 5;
            this.numDownAngle.Gapy = 5;
            this.numDownAngle.GLBackgroundImage = null;
            this.numDownAngle.GLVisible = false;
            this.numDownAngle.GuiAnchor = null;
            this.numDownAngle.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numDownAngle.Increment = 1F;
            this.numDownAngle.IntVal = 1;
            this.numDownAngle.IsFloat = true;
            this.numDownAngle.Location = new System.Drawing.Point(6, 64);
            this.numDownAngle.Margin = new System.Windows.Forms.Padding(4);
            this.numDownAngle.MaxFloat = 90F;
            this.numDownAngle.MaxInt = 1000;
            this.numDownAngle.MinFloat = 0F;
            this.numDownAngle.MinimumSize = new System.Drawing.Size(20, 5);
            this.numDownAngle.MinInt = 1;
            this.numDownAngle.Name = "numDownAngle";
            this.numDownAngle.Size = new System.Drawing.Size(79, 21);
            this.numDownAngle.StyleName = null;
            this.numDownAngle.TabIndex = 30;
            this.numDownAngle.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numDownAngle, "Downward facing degree");
            this.numDownAngle.ValidColor = System.Drawing.Color.White;
            this.numDownAngle.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numDownAngle.ValueChanged += new System.EventHandler(this.numDownAngle_ValueChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 22);
            this.label2.TabIndex = 29;
            this.label2.Text = "View";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.RoyalBlue;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.numGap);
            this.panel2.Controls.Add(this.cmbSupType);
            this.panel2.Controls.Add(this.chkOnlyDownward);
            this.panel2.Controls.Add(this.numY);
            this.panel2.Controls.Add(this.numX);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.labelAutoSup);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(8, 303);
            this.panel2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(297, 154);
            this.panel2.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(47, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(194, 23);
            this.label4.TabIndex = 37;
            this.label4.Text = "Generate on Downward Facing";
            // 
            // numGap
            // 
            this.numGap.BackColor = System.Drawing.Color.RoyalBlue;
            this.numGap.ButtonsColor = System.Drawing.Color.Navy;
            this.numGap.Checked = false;
            this.numGap.ErrorColor = System.Drawing.Color.Red;
            this.numGap.FloatVal = 1F;
            this.numGap.Gapx = 5;
            this.numGap.Gapy = 5;
            this.numGap.GLBackgroundImage = null;
            this.numGap.GLVisible = false;
            this.numGap.GuiAnchor = null;
            this.numGap.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numGap.Increment = 0.1F;
            this.numGap.IntVal = 1;
            this.numGap.IsFloat = true;
            this.numGap.Location = new System.Drawing.Point(49, 116);
            this.numGap.Margin = new System.Windows.Forms.Padding(4);
            this.numGap.MaxFloat = 20F;
            this.numGap.MaxInt = 1000;
            this.numGap.MinFloat = -20F;
            this.numGap.MinimumSize = new System.Drawing.Size(20, 5);
            this.numGap.MinInt = 1;
            this.numGap.Name = "numGap";
            this.numGap.Size = new System.Drawing.Size(79, 21);
            this.numGap.StyleName = null;
            this.numGap.TabIndex = 36;
            this.numGap.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numGap, "Horizontal grid spacing");
            this.numGap.ValidColor = System.Drawing.Color.White;
            this.numGap.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // cmbSupType
            // 
            this.cmbSupType.BackColor = System.Drawing.Color.RoyalBlue;
            this.cmbSupType.ForeColor = System.Drawing.SystemColors.Info;
            this.cmbSupType.FormattingEnabled = true;
            this.cmbSupType.Items.AddRange(new object[] {
            "Bed of Nails",
            "Adaptive"});
            this.cmbSupType.Location = new System.Drawing.Point(7, 33);
            this.cmbSupType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbSupType.Name = "cmbSupType";
            this.cmbSupType.Size = new System.Drawing.Size(223, 24);
            this.cmbSupType.TabIndex = 35;
            this.cmbSupType.SelectedIndexChanged += new System.EventHandler(this.cmbSupType_SelectedIndexChanged);
            // 
            // chkOnlyDownward
            // 
            this.chkOnlyDownward.BackColor = System.Drawing.Color.Navy;
            this.chkOnlyDownward.Checked = false;
            this.chkOnlyDownward.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.chkOnlyDownward.Gapx = 5;
            this.chkOnlyDownward.Gapy = 5;
            this.chkOnlyDownward.GLBackgroundImage = null;
            this.chkOnlyDownward.GLImage = null;
            this.chkOnlyDownward.GLVisible = false;
            this.chkOnlyDownward.GuiAnchor = null;
            this.chkOnlyDownward.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.chkOnlyDownward.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttAutoDown;
            this.chkOnlyDownward.Location = new System.Drawing.Point(7, 57);
            this.chkOnlyDownward.Margin = new System.Windows.Forms.Padding(5);
            this.chkOnlyDownward.Name = "chkOnlyDownward";
            this.chkOnlyDownward.Size = new System.Drawing.Size(32, 32);
            this.chkOnlyDownward.StyleName = null;
            this.chkOnlyDownward.TabIndex = 32;
            this.ctlToolTip1.SetToolTip(this.chkOnlyDownward, "Generate support only on\r\ndownward facing surfaces");
            this.chkOnlyDownward.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // numY
            // 
            this.numY.BackColor = System.Drawing.Color.RoyalBlue;
            this.numY.ButtonsColor = System.Drawing.Color.Navy;
            this.numY.Checked = false;
            this.numY.ErrorColor = System.Drawing.Color.Red;
            this.numY.FloatVal = 1F;
            this.numY.Gapx = 5;
            this.numY.Gapy = 5;
            this.numY.GLBackgroundImage = null;
            this.numY.GLVisible = false;
            this.numY.GuiAnchor = null;
            this.numY.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numY.Increment = 0.1F;
            this.numY.IntVal = 1;
            this.numY.IsFloat = true;
            this.numY.Location = new System.Drawing.Point(49, 129);
            this.numY.Margin = new System.Windows.Forms.Padding(4);
            this.numY.MaxFloat = 20F;
            this.numY.MaxInt = 1000;
            this.numY.MinFloat = -20F;
            this.numY.MinimumSize = new System.Drawing.Size(20, 5);
            this.numY.MinInt = 1;
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(79, 21);
            this.numY.StyleName = null;
            this.numY.TabIndex = 34;
            this.numY.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numY, "Vertical grid spacing");
            this.numY.ValidColor = System.Drawing.Color.White;
            this.numY.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // numX
            // 
            this.numX.BackColor = System.Drawing.Color.RoyalBlue;
            this.numX.ButtonsColor = System.Drawing.Color.Navy;
            this.numX.Checked = false;
            this.numX.ErrorColor = System.Drawing.Color.Red;
            this.numX.FloatVal = 1F;
            this.numX.Gapx = 5;
            this.numX.Gapy = 5;
            this.numX.GLBackgroundImage = null;
            this.numX.GLVisible = false;
            this.numX.GuiAnchor = null;
            this.numX.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.numX.Increment = 0.1F;
            this.numX.IntVal = 1;
            this.numX.IsFloat = true;
            this.numX.Location = new System.Drawing.Point(49, 105);
            this.numX.Margin = new System.Windows.Forms.Padding(4);
            this.numX.MaxFloat = 20F;
            this.numX.MaxInt = 1000;
            this.numX.MinFloat = -20F;
            this.numX.MinimumSize = new System.Drawing.Size(20, 5);
            this.numX.MinInt = 1;
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(79, 21);
            this.numX.StyleName = null;
            this.numX.TabIndex = 33;
            this.numX.TextFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlToolTip1.SetToolTip(this.numX, "Horizontal grid spacing");
            this.numX.ValidColor = System.Drawing.Color.White;
            this.numX.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(3, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 14);
            this.label7.TabIndex = 32;
            this.label7.Text = "Y (mm):";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 10.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(4, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 15);
            this.label6.TabIndex = 32;
            this.label6.Text = "X (mm):";
            // 
            // labelAutoSup
            // 
            this.labelAutoSup.Font = new System.Drawing.Font("Arial", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.labelAutoSup.ForeColor = System.Drawing.Color.White;
            this.labelAutoSup.Location = new System.Drawing.Point(0, 91);
            this.labelAutoSup.Name = "labelAutoSup";
            this.labelAutoSup.Size = new System.Drawing.Size(136, 15);
            this.labelAutoSup.TabIndex = 32;
            this.labelAutoSup.Text = "Automatic Support Grid:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(291, 22);
            this.label3.TabIndex = 29;
            this.label3.Text = "Automatic Supports";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.RoyalBlue;
            this.panel3.Controls.Add(this.cmdRemoveSupports);
            this.panel3.Controls.Add(this.lbSupports);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Location = new System.Drawing.Point(9, 456);
            this.panel3.Margin = new System.Windows.Forms.Padding(0, 0, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(296, 140);
            this.panel3.TabIndex = 27;
            // 
            // cmdRemoveSupports
            // 
            this.cmdRemoveSupports.Location = new System.Drawing.Point(115, 4);
            this.cmdRemoveSupports.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdRemoveSupports.Name = "cmdRemoveSupports";
            this.cmdRemoveSupports.Size = new System.Drawing.Size(171, 32);
            this.cmdRemoveSupports.TabIndex = 32;
            this.cmdRemoveSupports.Text = "Remove all Supports";
            this.cmdRemoveSupports.UseVisualStyleBackColor = true;
            this.cmdRemoveSupports.Click += new System.EventHandler(this.cmdRemoveSupports_Click);
            // 
            // lbSupports
            // 
            this.lbSupports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Supports});
            this.lbSupports.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSupports.FullRowSelect = true;
            this.lbSupports.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lbSupports.HideSelection = false;
            this.lbSupports.Location = new System.Drawing.Point(3, 39);
            this.lbSupports.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbSupports.MultiSelect = false;
            this.lbSupports.Name = "lbSupports";
            this.lbSupports.Size = new System.Drawing.Size(283, 95);
            this.lbSupports.TabIndex = 31;
            this.lbSupports.UseCompatibleStateImageBehavior = false;
            this.lbSupports.View = System.Windows.Forms.View.Details;
            this.lbSupports.SelectedIndexChanged += new System.EventHandler(this.lbSupports_SelectedIndexChanged);
            // 
            // Supports
            // 
            this.Supports.Text = "Supports";
            this.Supports.Width = 158;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Arial", 17.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(5, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(104, 22);
            this.label9.TabIndex = 29;
            this.label9.Text = "Supports";
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 1500;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // ctlTitle1
            // 
            this.ctlTitle1.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlTitle1.Checked = true;
            this.ctlTitle1.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttStateTrig;
            this.ctlTitle1.Gapx = 0;
            this.ctlTitle1.Gapy = 0;
            this.ctlTitle1.GLBackgroundImage = null;
            this.ctlTitle1.GLVisible = false;
            this.ctlTitle1.GuiAnchor = null;
            this.ctlTitle1.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSupport;
            this.ctlTitle1.Location = new System.Drawing.Point(3, 2);
            this.ctlTitle1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ctlTitle1.Name = "ctlTitle1";
            this.ctlTitle1.OnClickCallback = "";
            this.ctlTitle1.Size = new System.Drawing.Size(302, 43);
            this.ctlTitle1.StyleName = null;
            this.ctlTitle1.TabIndex = 28;
            this.ctlTitle1.Text = "Support Generation";
            this.ctlTitle1.Click += new System.EventHandler(this.ctlTitle1_Click);
            // 
            // ctlSupports
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.ctlTitle1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.progressTitle);
            this.Controls.Add(this.panelSuppotShape);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ctlSupports";
            this.Size = new System.Drawing.Size(310, 602);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panelSuppotShape.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSupport)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public ctlImageButton buttAddSupport;
        public ctlImageButton buttAutoSupport;
        public ctlProgress progressTitle;
        public ctlToolTip ctlToolTip1;
        public System.Windows.Forms.Panel panelSuppotShape;
        public ctlNumber numHT;
        public System.Windows.Forms.PictureBox pictureSupport;
        public ctlNumber numFB;
        public ctlNumber numFT;
        public ctlNumber numHB;
        public ctlNumber numFB1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Panel panel1;
        public ctlNumber numDownAngle;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label label3;
        public ctlImageButton chkDownPolys;
        public ctlNumber numY;
        public ctlNumber numX;
        public ctlImageButton chkOnlyDownward;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label labelAutoSup;
        public System.Windows.Forms.ComboBox cmbSupType;
        public System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.ListView lbSupports;
        public System.Windows.Forms.ColumnHeader Supports;
        public System.Windows.Forms.Button cmdRemoveSupports;
        public ctlTitle ctlTitle1;
        public ctlImageButton buttGenBase;
        public ctlNumber numGap;
        public System.Windows.Forms.Label label4;
        public ctlImageButton chkSupportAllScene;
    }
}
