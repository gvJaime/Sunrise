namespace UV_DLP_3D_Printer.GUI.Controls
{
    partial class ctlSliceView
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
            this.picSlice = new System.Windows.Forms.PictureBox();
            this.buttPreviewOnDisplay = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.numLayer = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.itemNumLayers = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlInfoItem();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            ((System.ComponentModel.ISupportInitialize)(this.picSlice)).BeginInit();
            this.SuspendLayout();
            // 
            // picSlice
            // 
            this.picSlice.BackColor = System.Drawing.Color.Black;
            this.picSlice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSlice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picSlice.Location = new System.Drawing.Point(0, 0);
            this.picSlice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.picSlice.Name = "picSlice";
            this.picSlice.Size = new System.Drawing.Size(711, 452);
            this.picSlice.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picSlice.TabIndex = 18;
            this.picSlice.TabStop = false;
            // 
            // buttPreviewOnDisplay
            // 
            this.buttPreviewOnDisplay.BackColor = System.Drawing.Color.Navy;
            this.buttPreviewOnDisplay.Checked = false;
            this.buttPreviewOnDisplay.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttPreviewOnDisplay.Gapx = 10;
            this.buttPreviewOnDisplay.Gapy = 10;
            this.buttPreviewOnDisplay.GLBackgroundImage = null;
            this.buttPreviewOnDisplay.GLImage = null;
            this.buttPreviewOnDisplay.GLVisible = false;
            this.buttPreviewOnDisplay.GuiAnchor = null;
            this.buttPreviewOnDisplay.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Left;
            this.buttPreviewOnDisplay.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPrevDisp;
            this.buttPreviewOnDisplay.Location = new System.Drawing.Point(10, 385);
            this.buttPreviewOnDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 10, 4);
            this.buttPreviewOnDisplay.Name = "buttPreviewOnDisplay";
            this.buttPreviewOnDisplay.Size = new System.Drawing.Size(48, 48);
            this.buttPreviewOnDisplay.StyleName = null;
            this.buttPreviewOnDisplay.TabIndex = 29;
            this.ctlToolTip1.SetToolTip(this.buttPreviewOnDisplay, "View slice on device display");
            this.buttPreviewOnDisplay.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Bottom;
            this.buttPreviewOnDisplay.Click += new System.EventHandler(this.buttPreviewOnDisplay_Click);
            // 
            // numLayer
            // 
            this.numLayer.BackColor = System.Drawing.Color.RoyalBlue;
            this.numLayer.ButtonsColor = System.Drawing.Color.Navy;
            this.numLayer.Checked = false;
            this.numLayer.EnableScroll = true;
            this.numLayer.ErrorColor = System.Drawing.Color.Red;
            this.numLayer.FloatVal = 10F;
            this.numLayer.Gapx = 0;
            this.numLayer.Gapy = 10;
            this.numLayer.GLBackgroundImage = null;
            this.numLayer.GLVisible = false;
            this.numLayer.GuiAnchor = null;
            this.numLayer.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Center;
            this.numLayer.Increment = 1F;
            this.numLayer.IntVal = 1000;
            this.numLayer.Location = new System.Drawing.Point(157, 409);
            this.numLayer.MaxFloat = 500F;
            this.numLayer.MaxInt = 1000;
            this.numLayer.MinFloat = -500F;
            this.numLayer.MinimumSize = new System.Drawing.Size(20, 5);
            this.numLayer.MinInt = 1;
            this.numLayer.Name = "numLayer";
            this.numLayer.Size = new System.Drawing.Size(425, 24);
            this.numLayer.StyleName = null;
            this.numLayer.TabIndex = 28;
            this.numLayer.TextFont = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.numLayer.ValidColor = System.Drawing.Color.White;
            this.numLayer.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.Bottom;
            this.numLayer.Visible = false;
            this.numLayer.ValueChanged += new System.EventHandler(this.numLayer_ValueChanged);
            // 
            // itemNumLayers
            // 
            this.itemNumLayers.BackColor = System.Drawing.Color.Navy;
            this.itemNumLayers.BorderWidth = 3;
            this.itemNumLayers.DataBackColor = System.Drawing.Color.RoyalBlue;
            this.itemNumLayers.DataColor = System.Drawing.Color.White;
            this.itemNumLayers.DataText = "";
            this.itemNumLayers.Font = new System.Drawing.Font("Arial", 17.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.itemNumLayers.Gapx = 0;
            this.itemNumLayers.Gapy = 0;
            this.itemNumLayers.GLBackgroundImage = null;
            this.itemNumLayers.GLVisible = false;
            this.itemNumLayers.GuiAnchor = null;
            this.itemNumLayers.Location = new System.Drawing.Point(10, 10);
            this.itemNumLayers.Name = "itemNumLayers";
            this.itemNumLayers.Size = new System.Drawing.Size(207, 30);
            this.itemNumLayers.StyleName = null;
            this.itemNumLayers.TabIndex = 19;
            this.itemNumLayers.TitleBackColor = System.Drawing.Color.Navy;
            this.itemNumLayers.TitleColor = System.Drawing.Color.White;
            this.itemNumLayers.TitleText = "# Layers:";
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 1500;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // ctlSliceView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.buttPreviewOnDisplay);
            this.Controls.Add(this.numLayer);
            this.Controls.Add(this.itemNumLayers);
            this.Controls.Add(this.picSlice);
            this.Name = "ctlSliceView";
            this.Size = new System.Drawing.Size(711, 452);
            this.SizeChanged += new System.EventHandler(this.ctlSliceView_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picSlice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picSlice;
        private CustomGUI.ctlInfoItem itemNumLayers;
        private CustomGUI.ctlNumber numLayer;
        private CustomGUI.ctlImageButton buttPreviewOnDisplay;
        private CustomGUI.ctlToolTip ctlToolTip1;
    }
}
