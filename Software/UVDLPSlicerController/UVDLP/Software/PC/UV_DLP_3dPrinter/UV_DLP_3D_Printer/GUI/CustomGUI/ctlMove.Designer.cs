namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    partial class ctlMove
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
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttOnPlatform = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttCenter = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttArrange = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.buttXMinus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.textMoveX = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox();
            this.buttXPlus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.buttYMinus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.textMoveY = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox();
            this.buttYPlus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel10 = new System.Windows.Forms.FlowLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.buttZMinus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.textMoveZ = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox();
            this.buttZPlus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.flowLayoutPanel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.Navy;
            this.flowLayoutPanel2.Controls.Add(this.label8);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel7);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel8);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel10);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(214, 238);
            this.flowLayoutPanel2.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 31);
            this.label8.TabIndex = 0;
            this.label8.Text = "Move";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.buttOnPlatform);
            this.flowLayoutPanel1.Controls.Add(this.buttCenter);
            this.flowLayoutPanel1.Controls.Add(this.buttArrange);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 34);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // buttOnPlatform
            // 
            this.buttOnPlatform.BackColor = System.Drawing.Color.Navy;
            this.buttOnPlatform.Checked = false;
            this.buttOnPlatform.CheckImage = null;
            this.buttOnPlatform.Gapx = 5;
            this.buttOnPlatform.Gapy = 5;
            this.buttOnPlatform.GLBackgroundImage = null;
            this.buttOnPlatform.GLImage = null;
            this.buttOnPlatform.GLVisible = false;
            this.buttOnPlatform.GuiAnchor = null;
            this.buttOnPlatform.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttOnPlatform.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttOnPlatform;
            this.buttOnPlatform.Location = new System.Drawing.Point(5, 5);
            this.buttOnPlatform.Margin = new System.Windows.Forms.Padding(5);
            this.buttOnPlatform.Name = "buttOnPlatform";
            this.buttOnPlatform.Size = new System.Drawing.Size(48, 48);
            this.buttOnPlatform.StyleName = null;
            this.buttOnPlatform.TabIndex = 23;
            this.ctlToolTip1.SetToolTip(this.buttOnPlatform, "Put object\r\non platform");
            this.buttOnPlatform.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttOnPlatform.Click += new System.EventHandler(this.buttOnPlatform_Click);
            // 
            // buttCenter
            // 
            this.buttCenter.BackColor = System.Drawing.Color.Navy;
            this.buttCenter.Checked = false;
            this.buttCenter.CheckImage = null;
            this.buttCenter.Gapx = 5;
            this.buttCenter.Gapy = 5;
            this.buttCenter.GLBackgroundImage = null;
            this.buttCenter.GLImage = null;
            this.buttCenter.GLVisible = false;
            this.buttCenter.GuiAnchor = null;
            this.buttCenter.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttCenter.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttCenter;
            this.buttCenter.Location = new System.Drawing.Point(58, 5);
            this.buttCenter.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttCenter.Name = "buttCenter";
            this.buttCenter.Size = new System.Drawing.Size(48, 48);
            this.buttCenter.StyleName = null;
            this.buttCenter.TabIndex = 24;
            this.ctlToolTip1.SetToolTip(this.buttCenter, "Center object");
            this.buttCenter.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttCenter.Click += new System.EventHandler(this.buttCenter_Click);
            // 
            // buttArrange
            // 
            this.buttArrange.BackColor = System.Drawing.Color.Navy;
            this.buttArrange.Checked = false;
            this.buttArrange.CheckImage = null;
            this.buttArrange.Gapx = 5;
            this.buttArrange.Gapy = 5;
            this.buttArrange.GLBackgroundImage = null;
            this.buttArrange.GLImage = null;
            this.buttArrange.GLVisible = false;
            this.buttArrange.GuiAnchor = null;
            this.buttArrange.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttArrange.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttArrange;
            this.buttArrange.Location = new System.Drawing.Point(111, 5);
            this.buttArrange.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttArrange.Name = "buttArrange";
            this.buttArrange.Size = new System.Drawing.Size(48, 48);
            this.buttArrange.StyleName = null;
            this.buttArrange.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttArrange, "Arrange objects\r\non platform");
            this.buttArrange.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttArrange.Click += new System.EventHandler(this.buttArrange_Click);
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel7.Controls.Add(this.label9);
            this.flowLayoutPanel7.Controls.Add(this.buttXMinus);
            this.flowLayoutPanel7.Controls.Add(this.textMoveX);
            this.flowLayoutPanel7.Controls.Add(this.buttXPlus);
            this.flowLayoutPanel7.Location = new System.Drawing.Point(3, 95);
            this.flowLayoutPanel7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(164, 38);
            this.flowLayoutPanel7.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(3, 3);
            this.label9.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 32);
            this.label9.TabIndex = 1;
            this.label9.Text = "X";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttXMinus
            // 
            this.buttXMinus.BackColor = System.Drawing.Color.Navy;
            this.buttXMinus.Checked = false;
            this.buttXMinus.CheckImage = null;
            this.buttXMinus.Gapx = 5;
            this.buttXMinus.Gapy = 5;
            this.buttXMinus.GLBackgroundImage = null;
            this.buttXMinus.GLImage = null;
            this.buttXMinus.GLVisible = false;
            this.buttXMinus.GuiAnchor = null;
            this.buttXMinus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttXMinus.Image = global::UV_DLP_3D_Printer.Properties.Resources.butMinus;
            this.buttXMinus.Location = new System.Drawing.Point(32, 5);
            this.buttXMinus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttXMinus.Name = "buttXMinus";
            this.buttXMinus.Size = new System.Drawing.Size(28, 28);
            this.buttXMinus.StyleName = null;
            this.buttXMinus.TabIndex = 2;
            this.buttXMinus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttXMinus.Click += new System.EventHandler(this.buttXMinus_Click);
            this.buttXMinus.DoubleClick += new System.EventHandler(this.buttXMinus_Click);
            // 
            // textMoveX
            // 
            this.textMoveX.BackColor = System.Drawing.Color.Navy;
            this.textMoveX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMoveX.ErrorColor = System.Drawing.Color.Red;
            this.textMoveX.FloatVal = 10F;
            this.textMoveX.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textMoveX.ForeColor = System.Drawing.Color.White;
            this.textMoveX.IntVal = 1;
            this.textMoveX.Location = new System.Drawing.Point(66, 5);
            this.textMoveX.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textMoveX.MaxFloat = 500F;
            this.textMoveX.MaxInt = 1000;
            this.textMoveX.MinFloat = -500F;
            this.textMoveX.MinInt = 1;
            this.textMoveX.Name = "textMoveX";
            this.textMoveX.Size = new System.Drawing.Size(60, 28);
            this.textMoveX.TabIndex = 4;
            this.textMoveX.Text = "10.0";
            this.textMoveX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textMoveX.ValidColor = System.Drawing.Color.White;
            this.textMoveX.ValueType = UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox.EValueType.Float;
            // 
            // buttXPlus
            // 
            this.buttXPlus.BackColor = System.Drawing.Color.Navy;
            this.buttXPlus.Checked = false;
            this.buttXPlus.CheckImage = null;
            this.buttXPlus.Gapx = 5;
            this.buttXPlus.Gapy = 5;
            this.buttXPlus.GLBackgroundImage = null;
            this.buttXPlus.GLImage = null;
            this.buttXPlus.GLVisible = false;
            this.buttXPlus.GuiAnchor = null;
            this.buttXPlus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttXPlus.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPlus;
            this.buttXPlus.Location = new System.Drawing.Point(132, 5);
            this.buttXPlus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttXPlus.Name = "buttXPlus";
            this.buttXPlus.Size = new System.Drawing.Size(28, 28);
            this.buttXPlus.StyleName = null;
            this.buttXPlus.TabIndex = 3;
            this.buttXPlus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttXPlus.Click += new System.EventHandler(this.buttXPlus_Click);
            this.buttXPlus.DoubleClick += new System.EventHandler(this.buttXPlus_Click);
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel8.Controls.Add(this.label10);
            this.flowLayoutPanel8.Controls.Add(this.buttYMinus);
            this.flowLayoutPanel8.Controls.Add(this.textMoveY);
            this.flowLayoutPanel8.Controls.Add(this.buttYPlus);
            this.flowLayoutPanel8.Location = new System.Drawing.Point(3, 136);
            this.flowLayoutPanel8.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(164, 38);
            this.flowLayoutPanel8.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(3, 3);
            this.label10.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 32);
            this.label10.TabIndex = 1;
            this.label10.Text = "Y";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttYMinus
            // 
            this.buttYMinus.BackColor = System.Drawing.Color.Navy;
            this.buttYMinus.Checked = false;
            this.buttYMinus.CheckImage = null;
            this.buttYMinus.Gapx = 5;
            this.buttYMinus.Gapy = 5;
            this.buttYMinus.GLBackgroundImage = null;
            this.buttYMinus.GLImage = null;
            this.buttYMinus.GLVisible = false;
            this.buttYMinus.GuiAnchor = null;
            this.buttYMinus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttYMinus.Image = global::UV_DLP_3D_Printer.Properties.Resources.butMinus;
            this.buttYMinus.Location = new System.Drawing.Point(32, 5);
            this.buttYMinus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttYMinus.Name = "buttYMinus";
            this.buttYMinus.Size = new System.Drawing.Size(28, 28);
            this.buttYMinus.StyleName = null;
            this.buttYMinus.TabIndex = 2;
            this.buttYMinus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttYMinus.Click += new System.EventHandler(this.buttYMinus_Click);
            this.buttYMinus.DoubleClick += new System.EventHandler(this.buttYMinus_Click);
            // 
            // textMoveY
            // 
            this.textMoveY.BackColor = System.Drawing.Color.Navy;
            this.textMoveY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMoveY.ErrorColor = System.Drawing.Color.Red;
            this.textMoveY.FloatVal = 10F;
            this.textMoveY.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textMoveY.ForeColor = System.Drawing.Color.White;
            this.textMoveY.IntVal = 1;
            this.textMoveY.Location = new System.Drawing.Point(66, 5);
            this.textMoveY.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textMoveY.MaxFloat = 500F;
            this.textMoveY.MaxInt = 1000;
            this.textMoveY.MinFloat = -500F;
            this.textMoveY.MinInt = 1;
            this.textMoveY.Name = "textMoveY";
            this.textMoveY.Size = new System.Drawing.Size(60, 28);
            this.textMoveY.TabIndex = 4;
            this.textMoveY.Text = "10.0";
            this.textMoveY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textMoveY.ValidColor = System.Drawing.Color.White;
            this.textMoveY.ValueType = UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox.EValueType.Float;
            // 
            // buttYPlus
            // 
            this.buttYPlus.BackColor = System.Drawing.Color.Navy;
            this.buttYPlus.Checked = false;
            this.buttYPlus.CheckImage = null;
            this.buttYPlus.Gapx = 5;
            this.buttYPlus.Gapy = 5;
            this.buttYPlus.GLBackgroundImage = null;
            this.buttYPlus.GLImage = null;
            this.buttYPlus.GLVisible = false;
            this.buttYPlus.GuiAnchor = null;
            this.buttYPlus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttYPlus.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPlus;
            this.buttYPlus.Location = new System.Drawing.Point(132, 5);
            this.buttYPlus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttYPlus.Name = "buttYPlus";
            this.buttYPlus.Size = new System.Drawing.Size(28, 28);
            this.buttYPlus.StyleName = null;
            this.buttYPlus.TabIndex = 3;
            this.buttYPlus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttYPlus.Click += new System.EventHandler(this.buttYPlus_Click);
            this.buttYPlus.DoubleClick += new System.EventHandler(this.buttYPlus_Click);
            // 
            // flowLayoutPanel10
            // 
            this.flowLayoutPanel10.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel10.Controls.Add(this.label11);
            this.flowLayoutPanel10.Controls.Add(this.buttZMinus);
            this.flowLayoutPanel10.Controls.Add(this.textMoveZ);
            this.flowLayoutPanel10.Controls.Add(this.buttZPlus);
            this.flowLayoutPanel10.Location = new System.Drawing.Point(3, 177);
            this.flowLayoutPanel10.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel10.Name = "flowLayoutPanel10";
            this.flowLayoutPanel10.Size = new System.Drawing.Size(164, 38);
            this.flowLayoutPanel10.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(3, 3);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(26, 32);
            this.label11.TabIndex = 1;
            this.label11.Text = "Z";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttZMinus
            // 
            this.buttZMinus.BackColor = System.Drawing.Color.Navy;
            this.buttZMinus.Checked = false;
            this.buttZMinus.CheckImage = null;
            this.buttZMinus.Gapx = 5;
            this.buttZMinus.Gapy = 5;
            this.buttZMinus.GLBackgroundImage = null;
            this.buttZMinus.GLImage = null;
            this.buttZMinus.GLVisible = false;
            this.buttZMinus.GuiAnchor = null;
            this.buttZMinus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttZMinus.Image = global::UV_DLP_3D_Printer.Properties.Resources.butMinus;
            this.buttZMinus.Location = new System.Drawing.Point(32, 5);
            this.buttZMinus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttZMinus.Name = "buttZMinus";
            this.buttZMinus.Size = new System.Drawing.Size(28, 28);
            this.buttZMinus.StyleName = null;
            this.buttZMinus.TabIndex = 2;
            this.buttZMinus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttZMinus.Click += new System.EventHandler(this.buttZMinus_Click);
            this.buttZMinus.DoubleClick += new System.EventHandler(this.buttZMinus_Click);
            // 
            // textMoveZ
            // 
            this.textMoveZ.BackColor = System.Drawing.Color.Navy;
            this.textMoveZ.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMoveZ.ErrorColor = System.Drawing.Color.Red;
            this.textMoveZ.FloatVal = 10F;
            this.textMoveZ.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.textMoveZ.ForeColor = System.Drawing.Color.White;
            this.textMoveZ.IntVal = 1;
            this.textMoveZ.Location = new System.Drawing.Point(66, 5);
            this.textMoveZ.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textMoveZ.MaxFloat = 500F;
            this.textMoveZ.MaxInt = 1000;
            this.textMoveZ.MinFloat = -500F;
            this.textMoveZ.MinInt = 1;
            this.textMoveZ.Name = "textMoveZ";
            this.textMoveZ.Size = new System.Drawing.Size(60, 28);
            this.textMoveZ.TabIndex = 4;
            this.textMoveZ.Text = "10.0";
            this.textMoveZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textMoveZ.ValidColor = System.Drawing.Color.White;
            this.textMoveZ.ValueType = UV_DLP_3D_Printer.GUI.CustomGUI.ctlTextBox.EValueType.Float;
            // 
            // buttZPlus
            // 
            this.buttZPlus.BackColor = System.Drawing.Color.Navy;
            this.buttZPlus.Checked = false;
            this.buttZPlus.CheckImage = null;
            this.buttZPlus.Gapx = 5;
            this.buttZPlus.Gapy = 5;
            this.buttZPlus.GLBackgroundImage = null;
            this.buttZPlus.GLImage = null;
            this.buttZPlus.GLVisible = false;
            this.buttZPlus.GuiAnchor = null;
            this.buttZPlus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttZPlus.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPlus;
            this.buttZPlus.Location = new System.Drawing.Point(132, 5);
            this.buttZPlus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.buttZPlus.Name = "buttZPlus";
            this.buttZPlus.Size = new System.Drawing.Size(28, 28);
            this.buttZPlus.StyleName = null;
            this.buttZPlus.TabIndex = 3;
            this.buttZPlus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttZPlus.Click += new System.EventHandler(this.buttZPlus_Click);
            this.buttZPlus.DoubleClick += new System.EventHandler(this.buttZPlus_Click);
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 1500;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // ctlMove
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "ctlMove";
            this.Size = new System.Drawing.Size(214, 238);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Label label9;
        private ctlImageButton buttXMinus;
        private ctlTextBox textMoveX;
        private ctlImageButton buttXPlus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label label10;
        private ctlImageButton buttYMinus;
        private ctlImageButton buttYPlus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel10;
        private System.Windows.Forms.Label label11;
        private ctlImageButton buttZMinus;
        private ctlImageButton buttZPlus;
        private ctlImageButton buttOnPlatform;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ctlImageButton buttCenter;
        private ctlTextBox textMoveY;
        private ctlTextBox textMoveZ;
        private ctlToolTip ctlToolTip1;
        private ctlImageButton buttArrange;
    }
}
