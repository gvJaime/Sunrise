using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer._3DEngine;
using Engine3D;
using UV_DLP_3D_Printer._3DEngine.CSG;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlMeshTools : ctlUserPanel
    {
        private enum ePrimType 
        {
            eSphere,
            eCube,
            eCone,
            eTorus,
            eCylinder
        }
        private FlowLayoutPanel flowLayoutPanel1;
        //private IContainer components;
        private ctlImageButton buttUnion;
        private ctlImageButton buttIntersect;
        private ctlImageButton buttSubtract;
        private FlowLayoutPanel flowLayoutPanel2;
        private ctlProgress progressTitle;
        private FlowLayoutPanel flowLayoutPanel4;
        private ctlImageButton buttAddCube;
        private ctlImageButton buttAddSphere;
        private ctlImageButton buttAddCone;
        private FlowLayoutPanel flowLayoutPanel5;
        private ctlImageButton ctlImageButton7;
        private ctlImageButton ctlImageButton8;
        private ctlImageButton ctlImageButton9;
        private FlowLayoutPanel flowLayoutPanel3;
        private ctlImageButton buttAddTorus;
        private ctlImageButton buttAddCylinder;
        private ctlNumber nbrSPHdivs;
        private ctlNumber nbrSPVdivs;
        private Button cmdCreatePrim;
        private Label label1;
        private ctlToolTip ctlToolTip1;
        private ctlNumber nbrSPRad;
        private Label label3;
        private Label label2;
        private FlowLayoutPanel pnlSphere;
        private FlowLayoutPanel pnlCone;
        private Label label4;
        private ctlNumber nbrCNVdivs;
        private Label label6;
        private ctlNumber nbrCNRad;
        private Label label8;
        private ctlNumber nbrCNHeight;
        private FlowLayoutPanel pnlCube;
        private Label label5;
        private ctlNumber ctlNumber1;
        private FlowLayoutPanel pnlTorus;
        private Label label7;
        private ctlNumber ctlNumber2;
        private ctlNumber ctlNumber3;
        private Label label9;
        private ctlNumber ctlNumber4;
        private ctlNumber ctlNumber5;
        private ePrimType m_primtype;
    
        public ctlMeshTools() 
        {
            InitializeComponent();
            buttAddSphere_Click(null, null);
            nbrSPVdivs.IntVal = 15;
            nbrSPHdivs.IntVal = 17;
            nbrSPRad.FloatVal = 5.0f;
            nbrCNVdivs.IntVal = 33;
            nbrCNRad.FloatVal = 5.0f;
            nbrCNHeight.FloatVal = 10.0f;
        }
        private void SetupForPrimType() 
        {
            //hide all panels
            //show only prim type panel
            HideAllPanels();
            switch (m_primtype) 
            {
                case ePrimType.eCone:
                case ePrimType.eCylinder:
                    pnlCone.Show();
                    break;
                case ePrimType.eSphere:
                    pnlSphere.Show();
                    break;
                case ePrimType.eCube:
                    pnlCube.Show();
                    break;
                case ePrimType.eTorus:
                    pnlTorus.Show();
                    break;
            }
        }
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttUnion = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttIntersect = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttSubtract = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.progressTitle = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlProgress();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttAddCube = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttAddSphere = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttAddCone = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.ctlImageButton7 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlImageButton8 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.ctlImageButton9 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttAddTorus = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.buttAddCylinder = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlImageButton();
            this.cmdCreatePrim = new System.Windows.Forms.Button();
            this.pnlSphere = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.nbrSPVdivs = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label2 = new System.Windows.Forms.Label();
            this.nbrSPHdivs = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label3 = new System.Windows.Forms.Label();
            this.nbrSPRad = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.pnlCone = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.nbrCNVdivs = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label6 = new System.Windows.Forms.Label();
            this.nbrCNRad = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label8 = new System.Windows.Forms.Label();
            this.nbrCNHeight = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.pnlCube = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.ctlNumber1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.pnlTorus = new System.Windows.Forms.FlowLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.ctlNumber2 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.ctlNumber3 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.label9 = new System.Windows.Forms.Label();
            this.ctlNumber4 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.ctlNumber5 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlNumber();
            this.ctlToolTip1 = new UV_DLP_3D_Printer.GUI.CustomGUI.ctlToolTip();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.pnlSphere.SuspendLayout();
            this.pnlCone.SuspendLayout();
            this.pnlCube.SuspendLayout();
            this.pnlTorus.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel1.Controls.Add(this.buttUnion);
            this.flowLayoutPanel1.Controls.Add(this.buttIntersect);
            this.flowLayoutPanel1.Controls.Add(this.buttSubtract);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 36);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel1.TabIndex = 25;
            // 
            // buttUnion
            // 
            this.buttUnion.BackColor = System.Drawing.Color.Navy;
            this.buttUnion.Checked = false;
            this.buttUnion.CheckImage = null;
            this.buttUnion.Gapx = 5;
            this.buttUnion.Gapy = 5;
            this.buttUnion.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttUnion.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttUnion;
            this.buttUnion.Location = new System.Drawing.Point(5, 5);
            this.buttUnion.Margin = new System.Windows.Forms.Padding(5);
            this.buttUnion.Name = "buttUnion";
            this.buttUnion.Padding = new System.Windows.Forms.Padding(5);
            this.buttUnion.Size = new System.Drawing.Size(48, 48);
            this.buttUnion.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttUnion, "Union Selected");
            this.buttUnion.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttUnion.Click += new System.EventHandler(this.buttUnion_Click);
            // 
            // buttIntersect
            // 
            this.buttIntersect.BackColor = System.Drawing.Color.Navy;
            this.buttIntersect.Checked = false;
            this.buttIntersect.CheckImage = null;
            this.buttIntersect.Enabled = false;
            this.buttIntersect.Gapx = 5;
            this.buttIntersect.Gapy = 5;
            this.buttIntersect.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttIntersect.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttIntersect;
            this.buttIntersect.Location = new System.Drawing.Point(58, 5);
            this.buttIntersect.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttIntersect.Name = "buttIntersect";
            this.buttIntersect.Padding = new System.Windows.Forms.Padding(5);
            this.buttIntersect.Size = new System.Drawing.Size(48, 48);
            this.buttIntersect.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.buttIntersect, "Intersect Selected");
            this.buttIntersect.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttIntersect.Click += new System.EventHandler(this.buttIntersect_Click);
            // 
            // buttSubtract
            // 
            this.buttSubtract.BackColor = System.Drawing.Color.Navy;
            this.buttSubtract.Checked = false;
            this.buttSubtract.CheckImage = null;
            this.buttSubtract.Enabled = false;
            this.buttSubtract.Gapx = 5;
            this.buttSubtract.Gapy = 5;
            this.buttSubtract.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSubtract.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSubtract;
            this.buttSubtract.Location = new System.Drawing.Point(111, 5);
            this.buttSubtract.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttSubtract.Name = "buttSubtract";
            this.buttSubtract.Padding = new System.Windows.Forms.Padding(5);
            this.buttSubtract.Size = new System.Drawing.Size(48, 48);
            this.buttSubtract.TabIndex = 27;
            this.ctlToolTip1.SetToolTip(this.buttSubtract, "Subtract Selected");
            this.buttSubtract.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttSubtract.Click += new System.EventHandler(this.buttSubtract_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.progressTitle);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel4);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel3);
            this.flowLayoutPanel2.Controls.Add(this.cmdCreatePrim);
            this.flowLayoutPanel2.Controls.Add(this.pnlSphere);
            this.flowLayoutPanel2.Controls.Add(this.pnlCone);
            this.flowLayoutPanel2.Controls.Add(this.pnlCube);
            this.flowLayoutPanel2.Controls.Add(this.pnlTorus);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1009, 274);
            this.flowLayoutPanel2.TabIndex = 26;
            // 
            // progressTitle
            // 
            this.progressTitle.BarColor = System.Drawing.Color.RoyalBlue;
            this.progressTitle.BorderThickness = 2;
            this.progressTitle.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.progressTitle.ForeColor = System.Drawing.Color.White;
            this.progressTitle.Location = new System.Drawing.Point(5, 4);
            this.progressTitle.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.progressTitle.Maximum = 100;
            this.progressTitle.Minimum = 0;
            this.progressTitle.Name = "progressTitle";
            this.progressTitle.Size = new System.Drawing.Size(160, 25);
            this.progressTitle.TabIndex = 1;
            this.progressTitle.Text = "Mesh Tools";
            this.progressTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.progressTitle.Value = 0;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel4.Controls.Add(this.buttAddCube);
            this.flowLayoutPanel4.Controls.Add(this.buttAddSphere);
            this.flowLayoutPanel4.Controls.Add(this.buttAddCone);
            this.flowLayoutPanel4.Controls.Add(this.flowLayoutPanel5);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 100);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(164, 53);
            this.flowLayoutPanel4.TabIndex = 29;
            // 
            // buttAddCube
            // 
            this.buttAddCube.BackColor = System.Drawing.Color.Navy;
            this.buttAddCube.Checked = false;
            this.buttAddCube.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddCube.Enabled = false;
            this.buttAddCube.Gapx = 5;
            this.buttAddCube.Gapy = 5;
            this.buttAddCube.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCube.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPCube;
            this.buttAddCube.Location = new System.Drawing.Point(5, 5);
            this.buttAddCube.Margin = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.buttAddCube.Name = "buttAddCube";
            this.buttAddCube.Padding = new System.Windows.Forms.Padding(5);
            this.buttAddCube.Size = new System.Drawing.Size(48, 48);
            this.buttAddCube.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttAddCube, "Add Box");
            this.buttAddCube.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCube.Click += new System.EventHandler(this.buttAddCube_Click);
            // 
            // buttAddSphere
            // 
            this.buttAddSphere.BackColor = System.Drawing.Color.Navy;
            this.buttAddSphere.Checked = false;
            this.buttAddSphere.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddSphere.Gapx = 5;
            this.buttAddSphere.Gapy = 5;
            this.buttAddSphere.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddSphere.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPSphere;
            this.buttAddSphere.Location = new System.Drawing.Point(58, 5);
            this.buttAddSphere.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.buttAddSphere.Name = "buttAddSphere";
            this.buttAddSphere.Padding = new System.Windows.Forms.Padding(5);
            this.buttAddSphere.Size = new System.Drawing.Size(48, 48);
            this.buttAddSphere.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.buttAddSphere, "Add Sphere");
            this.buttAddSphere.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddSphere.Click += new System.EventHandler(this.buttAddSphere_Click);
            // 
            // buttAddCone
            // 
            this.buttAddCone.BackColor = System.Drawing.Color.Navy;
            this.buttAddCone.Checked = false;
            this.buttAddCone.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddCone.Gapx = 5;
            this.buttAddCone.Gapy = 5;
            this.buttAddCone.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCone.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPCone;
            this.buttAddCone.Location = new System.Drawing.Point(111, 5);
            this.buttAddCone.Margin = new System.Windows.Forms.Padding(0, 5, 5, 0);
            this.buttAddCone.Name = "buttAddCone";
            this.buttAddCone.Padding = new System.Windows.Forms.Padding(5);
            this.buttAddCone.Size = new System.Drawing.Size(48, 48);
            this.buttAddCone.TabIndex = 27;
            this.ctlToolTip1.SetToolTip(this.buttAddCone, "Add Cone");
            this.buttAddCone.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCone.Click += new System.EventHandler(this.buttAddCone_Click);
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel5.Controls.Add(this.ctlImageButton7);
            this.flowLayoutPanel5.Controls.Add(this.ctlImageButton8);
            this.flowLayoutPanel5.Controls.Add(this.ctlImageButton9);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 56);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel5.TabIndex = 28;
            // 
            // ctlImageButton7
            // 
            this.ctlImageButton7.BackColor = System.Drawing.Color.Navy;
            this.ctlImageButton7.Checked = false;
            this.ctlImageButton7.CheckImage = null;
            this.ctlImageButton7.Gapx = 5;
            this.ctlImageButton7.Gapy = 5;
            this.ctlImageButton7.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlImageButton7.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttUnion;
            this.ctlImageButton7.Location = new System.Drawing.Point(5, 5);
            this.ctlImageButton7.Margin = new System.Windows.Forms.Padding(5);
            this.ctlImageButton7.Name = "ctlImageButton7";
            this.ctlImageButton7.Padding = new System.Windows.Forms.Padding(5);
            this.ctlImageButton7.Size = new System.Drawing.Size(48, 48);
            this.ctlImageButton7.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.ctlImageButton7, "Union Selected");
            this.ctlImageButton7.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlImageButton8
            // 
            this.ctlImageButton8.BackColor = System.Drawing.Color.Navy;
            this.ctlImageButton8.Checked = false;
            this.ctlImageButton8.CheckImage = null;
            this.ctlImageButton8.Gapx = 5;
            this.ctlImageButton8.Gapy = 5;
            this.ctlImageButton8.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlImageButton8.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttIntersect;
            this.ctlImageButton8.Location = new System.Drawing.Point(58, 5);
            this.ctlImageButton8.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.ctlImageButton8.Name = "ctlImageButton8";
            this.ctlImageButton8.Padding = new System.Windows.Forms.Padding(5);
            this.ctlImageButton8.Size = new System.Drawing.Size(48, 48);
            this.ctlImageButton8.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.ctlImageButton8, "Intersect Selected");
            this.ctlImageButton8.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlImageButton9
            // 
            this.ctlImageButton9.BackColor = System.Drawing.Color.Navy;
            this.ctlImageButton9.Checked = false;
            this.ctlImageButton9.CheckImage = null;
            this.ctlImageButton9.Gapx = 5;
            this.ctlImageButton9.Gapy = 5;
            this.ctlImageButton9.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlImageButton9.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttSubtract;
            this.ctlImageButton9.Location = new System.Drawing.Point(111, 5);
            this.ctlImageButton9.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.ctlImageButton9.Name = "ctlImageButton9";
            this.ctlImageButton9.Padding = new System.Windows.Forms.Padding(5);
            this.ctlImageButton9.Size = new System.Drawing.Size(48, 48);
            this.ctlImageButton9.TabIndex = 27;
            this.ctlToolTip1.SetToolTip(this.ctlImageButton9, "Subtract Selected");
            this.ctlImageButton9.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowLayoutPanel3.Controls.Add(this.buttAddTorus);
            this.flowLayoutPanel3.Controls.Add(this.buttAddCylinder);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 153);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(164, 58);
            this.flowLayoutPanel3.TabIndex = 28;
            // 
            // buttAddTorus
            // 
            this.buttAddTorus.BackColor = System.Drawing.Color.Navy;
            this.buttAddTorus.Checked = false;
            this.buttAddTorus.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddTorus.Enabled = false;
            this.buttAddTorus.Gapx = 5;
            this.buttAddTorus.Gapy = 5;
            this.buttAddTorus.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddTorus.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPTorus;
            this.buttAddTorus.Location = new System.Drawing.Point(5, 5);
            this.buttAddTorus.Margin = new System.Windows.Forms.Padding(5);
            this.buttAddTorus.Name = "buttAddTorus";
            this.buttAddTorus.Padding = new System.Windows.Forms.Padding(5);
            this.buttAddTorus.Size = new System.Drawing.Size(48, 48);
            this.buttAddTorus.TabIndex = 25;
            this.ctlToolTip1.SetToolTip(this.buttAddTorus, "Add Torus");
            this.buttAddTorus.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddTorus.Click += new System.EventHandler(this.buttAddTorus_Click);
            // 
            // buttAddCylinder
            // 
            this.buttAddCylinder.BackColor = System.Drawing.Color.Navy;
            this.buttAddCylinder.Checked = false;
            this.buttAddCylinder.CheckImage = global::UV_DLP_3D_Printer.Properties.Resources.buttChecked;
            this.buttAddCylinder.Gapx = 5;
            this.buttAddCylinder.Gapy = 5;
            this.buttAddCylinder.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCylinder.Image = global::UV_DLP_3D_Printer.Properties.Resources.buttPCylinder;
            this.buttAddCylinder.Location = new System.Drawing.Point(58, 5);
            this.buttAddCylinder.Margin = new System.Windows.Forms.Padding(0, 5, 5, 5);
            this.buttAddCylinder.Name = "buttAddCylinder";
            this.buttAddCylinder.Padding = new System.Windows.Forms.Padding(5);
            this.buttAddCylinder.Size = new System.Drawing.Size(48, 48);
            this.buttAddCylinder.TabIndex = 26;
            this.ctlToolTip1.SetToolTip(this.buttAddCylinder, "Add Cylinder");
            this.buttAddCylinder.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.buttAddCylinder.Click += new System.EventHandler(this.buttAddCylinder_Click);
            // 
            // cmdCreatePrim
            // 
            this.cmdCreatePrim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCreatePrim.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCreatePrim.ForeColor = System.Drawing.Color.White;
            this.cmdCreatePrim.Location = new System.Drawing.Point(3, 217);
            this.cmdCreatePrim.Name = "cmdCreatePrim";
            this.cmdCreatePrim.Size = new System.Drawing.Size(159, 32);
            this.cmdCreatePrim.TabIndex = 31;
            this.cmdCreatePrim.Text = "Create";
            this.cmdCreatePrim.UseVisualStyleBackColor = true;
            this.cmdCreatePrim.Click += new System.EventHandler(this.cmdCreatePrim_Click);
            // 
            // pnlSphere
            // 
            this.pnlSphere.BackColor = System.Drawing.Color.RoyalBlue;
            this.pnlSphere.Controls.Add(this.label1);
            this.pnlSphere.Controls.Add(this.nbrSPVdivs);
            this.pnlSphere.Controls.Add(this.label2);
            this.pnlSphere.Controls.Add(this.nbrSPHdivs);
            this.pnlSphere.Controls.Add(this.label3);
            this.pnlSphere.Controls.Add(this.nbrSPRad);
            this.pnlSphere.Location = new System.Drawing.Point(173, 3);
            this.pnlSphere.Name = "pnlSphere";
            this.pnlSphere.Size = new System.Drawing.Size(164, 161);
            this.pnlSphere.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Vertical Divisions";
            // 
            // nbrSPVdivs
            // 
            this.nbrSPVdivs.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrSPVdivs.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrSPVdivs.Checked = false;
            this.nbrSPVdivs.ErrorColor = System.Drawing.Color.Red;
            this.nbrSPVdivs.FloatVal = 10F;
            this.nbrSPVdivs.Gapx = 5;
            this.nbrSPVdivs.Gapy = 5;
            this.nbrSPVdivs.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrSPVdivs.Increment = 1F;
            this.nbrSPVdivs.IntVal = 1000;
            this.nbrSPVdivs.Location = new System.Drawing.Point(3, 20);
            this.nbrSPVdivs.MaxFloat = 500F;
            this.nbrSPVdivs.MaxInt = 1000;
            this.nbrSPVdivs.MinFloat = -500F;
            this.nbrSPVdivs.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrSPVdivs.MinInt = 5;
            this.nbrSPVdivs.Name = "nbrSPVdivs";
            this.nbrSPVdivs.Size = new System.Drawing.Size(144, 27);
            this.nbrSPVdivs.TabIndex = 0;
            this.nbrSPVdivs.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrSPVdivs.ValidColor = System.Drawing.Color.White;
            this.nbrSPVdivs.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Horizontal Divisions";
            // 
            // nbrSPHdivs
            // 
            this.nbrSPHdivs.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrSPHdivs.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrSPHdivs.Checked = false;
            this.nbrSPHdivs.ErrorColor = System.Drawing.Color.Red;
            this.nbrSPHdivs.FloatVal = 17F;
            this.nbrSPHdivs.Gapx = 5;
            this.nbrSPHdivs.Gapy = 5;
            this.nbrSPHdivs.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrSPHdivs.Increment = 1F;
            this.nbrSPHdivs.IntVal = 1000;
            this.nbrSPHdivs.Location = new System.Drawing.Point(3, 70);
            this.nbrSPHdivs.MaxFloat = 500F;
            this.nbrSPHdivs.MaxInt = 1000;
            this.nbrSPHdivs.MinFloat = -500F;
            this.nbrSPHdivs.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrSPHdivs.MinInt = 5;
            this.nbrSPHdivs.Name = "nbrSPHdivs";
            this.nbrSPHdivs.Size = new System.Drawing.Size(144, 26);
            this.nbrSPHdivs.TabIndex = 1;
            this.nbrSPHdivs.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrSPHdivs.ValidColor = System.Drawing.Color.White;
            this.nbrSPHdivs.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Radius";
            // 
            // nbrSPRad
            // 
            this.nbrSPRad.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrSPRad.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrSPRad.Checked = false;
            this.nbrSPRad.ErrorColor = System.Drawing.Color.Red;
            this.nbrSPRad.FloatVal = 519F;
            this.nbrSPRad.Gapx = 5;
            this.nbrSPRad.Gapy = 5;
            this.nbrSPRad.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrSPRad.Increment = 1F;
            this.nbrSPRad.IntVal = 5;
            this.nbrSPRad.IsFloat = true;
            this.nbrSPRad.Location = new System.Drawing.Point(3, 119);
            this.nbrSPRad.MaxFloat = 500F;
            this.nbrSPRad.MaxInt = 1000;
            this.nbrSPRad.MinFloat = 1F;
            this.nbrSPRad.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrSPRad.MinInt = 5;
            this.nbrSPRad.Name = "nbrSPRad";
            this.nbrSPRad.Size = new System.Drawing.Size(144, 26);
            this.nbrSPRad.TabIndex = 5;
            this.nbrSPRad.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrSPRad.ValidColor = System.Drawing.Color.White;
            this.nbrSPRad.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // pnlCone
            // 
            this.pnlCone.BackColor = System.Drawing.Color.RoyalBlue;
            this.pnlCone.Controls.Add(this.label4);
            this.pnlCone.Controls.Add(this.nbrCNVdivs);
            this.pnlCone.Controls.Add(this.label6);
            this.pnlCone.Controls.Add(this.nbrCNRad);
            this.pnlCone.Controls.Add(this.label8);
            this.pnlCone.Controls.Add(this.nbrCNHeight);
            this.pnlCone.Location = new System.Drawing.Point(343, 3);
            this.pnlCone.Name = "pnlCone";
            this.pnlCone.Size = new System.Drawing.Size(164, 161);
            this.pnlCone.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Divisions";
            // 
            // nbrCNVdivs
            // 
            this.nbrCNVdivs.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrCNVdivs.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrCNVdivs.Checked = false;
            this.nbrCNVdivs.ErrorColor = System.Drawing.Color.Red;
            this.nbrCNVdivs.FloatVal = 10F;
            this.nbrCNVdivs.Gapx = 5;
            this.nbrCNVdivs.Gapy = 5;
            this.nbrCNVdivs.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrCNVdivs.Increment = 1F;
            this.nbrCNVdivs.IntVal = 1000;
            this.nbrCNVdivs.Location = new System.Drawing.Point(3, 20);
            this.nbrCNVdivs.MaxFloat = 500F;
            this.nbrCNVdivs.MaxInt = 1000;
            this.nbrCNVdivs.MinFloat = -500F;
            this.nbrCNVdivs.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrCNVdivs.MinInt = 5;
            this.nbrCNVdivs.Name = "nbrCNVdivs";
            this.nbrCNVdivs.Size = new System.Drawing.Size(144, 27);
            this.nbrCNVdivs.TabIndex = 0;
            this.nbrCNVdivs.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrCNVdivs.ValidColor = System.Drawing.Color.White;
            this.nbrCNVdivs.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(3, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Radius";
            // 
            // nbrCNRad
            // 
            this.nbrCNRad.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrCNRad.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrCNRad.Checked = false;
            this.nbrCNRad.ErrorColor = System.Drawing.Color.Red;
            this.nbrCNRad.FloatVal = 519F;
            this.nbrCNRad.Gapx = 5;
            this.nbrCNRad.Gapy = 5;
            this.nbrCNRad.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrCNRad.Increment = 1F;
            this.nbrCNRad.IntVal = 5;
            this.nbrCNRad.IsFloat = true;
            this.nbrCNRad.Location = new System.Drawing.Point(3, 70);
            this.nbrCNRad.MaxFloat = 500F;
            this.nbrCNRad.MaxInt = 1000;
            this.nbrCNRad.MinFloat = 1F;
            this.nbrCNRad.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrCNRad.MinInt = 5;
            this.nbrCNRad.Name = "nbrCNRad";
            this.nbrCNRad.Size = new System.Drawing.Size(144, 26);
            this.nbrCNRad.TabIndex = 5;
            this.nbrCNRad.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrCNRad.ValidColor = System.Drawing.Color.White;
            this.nbrCNRad.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(3, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "Height";
            // 
            // nbrCNHeight
            // 
            this.nbrCNHeight.BackColor = System.Drawing.Color.RoyalBlue;
            this.nbrCNHeight.ButtonsColor = System.Drawing.Color.Navy;
            this.nbrCNHeight.Checked = false;
            this.nbrCNHeight.ErrorColor = System.Drawing.Color.Red;
            this.nbrCNHeight.FloatVal = 1000F;
            this.nbrCNHeight.Gapx = 5;
            this.nbrCNHeight.Gapy = 5;
            this.nbrCNHeight.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.nbrCNHeight.Increment = 1F;
            this.nbrCNHeight.IntVal = 10;
            this.nbrCNHeight.IsFloat = true;
            this.nbrCNHeight.Location = new System.Drawing.Point(3, 119);
            this.nbrCNHeight.MaxFloat = 500F;
            this.nbrCNHeight.MaxInt = 1000;
            this.nbrCNHeight.MinFloat = 0.1F;
            this.nbrCNHeight.MinimumSize = new System.Drawing.Size(20, 5);
            this.nbrCNHeight.MinInt = 5;
            this.nbrCNHeight.Name = "nbrCNHeight";
            this.nbrCNHeight.Size = new System.Drawing.Size(144, 26);
            this.nbrCNHeight.TabIndex = 7;
            this.nbrCNHeight.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.nbrCNHeight.ValidColor = System.Drawing.Color.White;
            this.nbrCNHeight.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // pnlCube
            // 
            this.pnlCube.BackColor = System.Drawing.Color.RoyalBlue;
            this.pnlCube.Controls.Add(this.label5);
            this.pnlCube.Controls.Add(this.ctlNumber1);
            this.pnlCube.Location = new System.Drawing.Point(513, 3);
            this.pnlCube.Name = "pnlCube";
            this.pnlCube.Size = new System.Drawing.Size(164, 161);
            this.pnlCube.TabIndex = 34;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Size";
            // 
            // ctlNumber1
            // 
            this.ctlNumber1.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlNumber1.ButtonsColor = System.Drawing.Color.Navy;
            this.ctlNumber1.Checked = false;
            this.ctlNumber1.ErrorColor = System.Drawing.Color.Red;
            this.ctlNumber1.FloatVal = 10F;
            this.ctlNumber1.Gapx = 5;
            this.ctlNumber1.Gapy = 5;
            this.ctlNumber1.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlNumber1.Increment = 1F;
            this.ctlNumber1.IntVal = 1000;
            this.ctlNumber1.Location = new System.Drawing.Point(3, 20);
            this.ctlNumber1.MaxFloat = 500F;
            this.ctlNumber1.MaxInt = 1000;
            this.ctlNumber1.MinFloat = -500F;
            this.ctlNumber1.MinimumSize = new System.Drawing.Size(20, 5);
            this.ctlNumber1.MinInt = 5;
            this.ctlNumber1.Name = "ctlNumber1";
            this.ctlNumber1.Size = new System.Drawing.Size(144, 27);
            this.ctlNumber1.TabIndex = 0;
            this.ctlNumber1.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlNumber1.ValidColor = System.Drawing.Color.White;
            this.ctlNumber1.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // pnlTorus
            // 
            this.pnlTorus.BackColor = System.Drawing.Color.RoyalBlue;
            this.pnlTorus.Controls.Add(this.label7);
            this.pnlTorus.Controls.Add(this.ctlNumber2);
            this.pnlTorus.Controls.Add(this.ctlNumber3);
            this.pnlTorus.Controls.Add(this.label9);
            this.pnlTorus.Controls.Add(this.ctlNumber4);
            this.pnlTorus.Controls.Add(this.ctlNumber5);
            this.pnlTorus.Location = new System.Drawing.Point(683, 3);
            this.pnlTorus.Name = "pnlTorus";
            this.pnlTorus.Size = new System.Drawing.Size(164, 161);
            this.pnlTorus.TabIndex = 35;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Radii (Inner/outer)";
            // 
            // ctlNumber2
            // 
            this.ctlNumber2.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlNumber2.ButtonsColor = System.Drawing.Color.Navy;
            this.ctlNumber2.Checked = false;
            this.ctlNumber2.ErrorColor = System.Drawing.Color.Red;
            this.ctlNumber2.FloatVal = 422F;
            this.ctlNumber2.Gapx = 5;
            this.ctlNumber2.Gapy = 5;
            this.ctlNumber2.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlNumber2.Increment = 1F;
            this.ctlNumber2.IntVal = 2;
            this.ctlNumber2.IsFloat = true;
            this.ctlNumber2.Location = new System.Drawing.Point(3, 20);
            this.ctlNumber2.MaxFloat = 50F;
            this.ctlNumber2.MaxInt = 1000;
            this.ctlNumber2.MinFloat = 0.1F;
            this.ctlNumber2.MinimumSize = new System.Drawing.Size(20, 5);
            this.ctlNumber2.MinInt = 1;
            this.ctlNumber2.Name = "ctlNumber2";
            this.ctlNumber2.Size = new System.Drawing.Size(143, 27);
            this.ctlNumber2.TabIndex = 0;
            this.ctlNumber2.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlNumber2.ValidColor = System.Drawing.Color.White;
            this.ctlNumber2.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlNumber3
            // 
            this.ctlNumber3.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlNumber3.ButtonsColor = System.Drawing.Color.Navy;
            this.ctlNumber3.Checked = false;
            this.ctlNumber3.ErrorColor = System.Drawing.Color.Red;
            this.ctlNumber3.FloatVal = 823F;
            this.ctlNumber3.Gapx = 5;
            this.ctlNumber3.Gapy = 5;
            this.ctlNumber3.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlNumber3.Increment = 1F;
            this.ctlNumber3.IntVal = 2;
            this.ctlNumber3.IsFloat = true;
            this.ctlNumber3.Location = new System.Drawing.Point(3, 53);
            this.ctlNumber3.MaxFloat = 50F;
            this.ctlNumber3.MaxInt = 1000;
            this.ctlNumber3.MinFloat = 0.1F;
            this.ctlNumber3.MinimumSize = new System.Drawing.Size(20, 5);
            this.ctlNumber3.MinInt = 1;
            this.ctlNumber3.Name = "ctlNumber3";
            this.ctlNumber3.Size = new System.Drawing.Size(143, 27);
            this.ctlNumber3.TabIndex = 4;
            this.ctlNumber3.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlNumber3.ValidColor = System.Drawing.Color.White;
            this.ctlNumber3.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(3, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(151, 17);
            this.label9.TabIndex = 5;
            this.label9.Text = "Divisions (Radial/Axial)";
            // 
            // ctlNumber4
            // 
            this.ctlNumber4.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlNumber4.ButtonsColor = System.Drawing.Color.Navy;
            this.ctlNumber4.Checked = false;
            this.ctlNumber4.ErrorColor = System.Drawing.Color.Red;
            this.ctlNumber4.FloatVal = 15F;
            this.ctlNumber4.Gapx = 5;
            this.ctlNumber4.Gapy = 5;
            this.ctlNumber4.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlNumber4.Increment = 1F;
            this.ctlNumber4.IntVal = 1000;
            this.ctlNumber4.Location = new System.Drawing.Point(3, 103);
            this.ctlNumber4.MaxFloat = 500F;
            this.ctlNumber4.MaxInt = 1000;
            this.ctlNumber4.MinFloat = 0.1F;
            this.ctlNumber4.MinimumSize = new System.Drawing.Size(20, 5);
            this.ctlNumber4.MinInt = 3;
            this.ctlNumber4.Name = "ctlNumber4";
            this.ctlNumber4.Size = new System.Drawing.Size(144, 26);
            this.ctlNumber4.TabIndex = 8;
            this.ctlNumber4.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlNumber4.ValidColor = System.Drawing.Color.White;
            this.ctlNumber4.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlNumber5
            // 
            this.ctlNumber5.BackColor = System.Drawing.Color.RoyalBlue;
            this.ctlNumber5.ButtonsColor = System.Drawing.Color.Navy;
            this.ctlNumber5.Checked = false;
            this.ctlNumber5.ErrorColor = System.Drawing.Color.Red;
            this.ctlNumber5.FloatVal = 15F;
            this.ctlNumber5.Gapx = 5;
            this.ctlNumber5.Gapy = 5;
            this.ctlNumber5.HorizontalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            this.ctlNumber5.Increment = 1F;
            this.ctlNumber5.IntVal = 1000;
            this.ctlNumber5.Location = new System.Drawing.Point(3, 135);
            this.ctlNumber5.MaxFloat = 500F;
            this.ctlNumber5.MaxInt = 1000;
            this.ctlNumber5.MinFloat = 0.1F;
            this.ctlNumber5.MinimumSize = new System.Drawing.Size(20, 5);
            this.ctlNumber5.MinInt = 3;
            this.ctlNumber5.Name = "ctlNumber5";
            this.ctlNumber5.Size = new System.Drawing.Size(144, 26);
            this.ctlNumber5.TabIndex = 9;
            this.ctlNumber5.TextFont = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.ctlNumber5.ValidColor = System.Drawing.Color.White;
            this.ctlNumber5.VerticalAnchor = UV_DLP_3D_Printer.GUI.CustomGUI.ctlAnchorable.AnchorTypes.None;
            // 
            // ctlToolTip1
            // 
            this.ctlToolTip1.AutoPopDelay = 5000;
            this.ctlToolTip1.BackColor = System.Drawing.Color.Turquoise;
            this.ctlToolTip1.ForeColor = System.Drawing.Color.Navy;
            this.ctlToolTip1.InitialDelay = 100;
            this.ctlToolTip1.ReshowDelay = 100;
            // 
            // ctlMeshTools
            // 
            this.BackColor = System.Drawing.Color.Navy;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "ctlMeshTools";
            this.Size = new System.Drawing.Size(1009, 274);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.pnlSphere.ResumeLayout(false);
            this.pnlSphere.PerformLayout();
            this.pnlCone.ResumeLayout(false);
            this.pnlCone.PerformLayout();
            this.pnlCube.ResumeLayout(false);
            this.pnlCube.PerformLayout();
            this.pnlTorus.ResumeLayout(false);
            this.pnlTorus.PerformLayout();
            this.ResumeLayout(false);

        }
        /// <summary>
        /// This function unchecks all the buttons for primitives
        /// </summary>
        private void UnCheckAll() 
        {
            buttAddCone.Checked = false;
            buttAddCube.Checked = false;
            buttAddCylinder.Checked = false;
            buttAddSphere.Checked = false;
            buttAddTorus.Checked = false;
        }

        private void HideAllPanels() 
        {
            pnlCone.Hide();
            pnlSphere.Hide();
            pnlCube.Hide();
        }
        private void buttAddSphere_Click(object sender, EventArgs e)
        {
            UnCheckAll();
            buttAddSphere.Checked = true;
            m_primtype = ePrimType.eSphere;
            SetupForPrimType();         
        }
        private void buttAddTorus_Click(object sender, EventArgs e)
        {
            UnCheckAll();
            buttAddTorus.Checked = true;
            m_primtype = ePrimType.eTorus;
            SetupForPrimType();  
        }
        private void buttAddCube_Click(object sender, EventArgs e)
        {
            UnCheckAll();
            buttAddCube.Checked = true;
            m_primtype = ePrimType.eCube;
            SetupForPrimType();
        }
        private void buttAddCylinder_Click(object sender, EventArgs e)
        {
            UnCheckAll();
            buttAddCylinder.Checked = true;
            m_primtype = ePrimType.eCylinder;
            SetupForPrimType();
        }

        private void buttAddCone_Click(object sender, EventArgs e)
        {
            UnCheckAll();
            buttAddCone.Checked = true;
            m_primtype = ePrimType.eCone;
            SetupForPrimType();
            /*
            Cylinder3d cy = new Cylinder3d();
            cy.Create(10, 0, 10, 17, 2);
            if (cy != null)
            {
                UVDLPApp.Instance().Engine3D.AddObject(cy);
                UVDLPApp.Instance().SelectedObject = cy;
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelAdded, "");
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
            }
             */ 
        }

        private void cmdCreatePrim_Click(object sender, EventArgs e)
        {
//            Object3d newobj = null;
            float h, rad; 
            int ring, sector;
            switch (m_primtype) 
            {
                case ePrimType.eCone:
                    rad = nbrCNRad.FloatVal;
                    h = nbrCNHeight.FloatVal;
                    ring = nbrCNVdivs.IntVal;
                    Cylinder3d cn = new Cylinder3d();
                    cn.Create(rad, 0.0f, h, ring, 2);
                    if (cn != null)
                    {
                        UVDLPApp.Instance().Engine3D.AddObject(cn);
                        UVDLPApp.Instance().SelectedObject = cn;
                    }

                    break;
                case ePrimType.eCube:
                    break;
                case ePrimType.eCylinder:
                    rad = nbrCNRad.FloatVal;
                    h = nbrCNHeight.FloatVal;
                    ring = nbrCNVdivs.IntVal;
                    Cylinder3d cy = new Cylinder3d();
                    cy.Create(rad, rad, h, ring, 2);
                    if (cy != null)
                    {
                        UVDLPApp.Instance().Engine3D.AddObject(cy);
                        UVDLPApp.Instance().SelectedObject = cy;
                    }

                    break;
                case ePrimType.eSphere:
                    rad = nbrSPRad.FloatVal;
                    ring = nbrSPHdivs.IntVal;
                    sector = nbrSPVdivs.IntVal;
                    Object3d newobj = Primitives.Sphere(rad, ring, sector);
                    if (newobj != null) 
                    {
                        UVDLPApp.Instance().Engine3D.AddObject(newobj);
                        UVDLPApp.Instance().SelectedObject = newobj;
                    }
                    break;
                case ePrimType.eTorus:
                    break;
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelAdded, "");
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");

        }

        public void CSGEvent(CSG.eCSGEvent ev, string msg, Object3d dat)
        {
            try
            {
                switch (ev)
                {
                    case CSG.eCSGEvent.eCompleted:
                        break;
                    case CSG.eCSGEvent.eError:
                        break;
                    case CSG.eCSGEvent.eProgress:
                        //tick the progress bar
                        break;
                    case CSG.eCSGEvent.eStarted:
                        break;
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttUnion_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObjectList.Count < 2) return; // must have morre than 2 objects
            CSG.Instance().StartOp(CSG.eCSGOp.eUnion, UVDLPApp.Instance().SelectedObjectList[0], UVDLPApp.Instance().SelectedObjectList[1]);
        }

        private void buttIntersect_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObjectList.Count < 2) return; // must have morre than 2 objects
            CSG.Instance().StartOp(CSG.eCSGOp.eIntersection, UVDLPApp.Instance().SelectedObjectList[0], UVDLPApp.Instance().SelectedObjectList[1]);
        }

        private void buttSubtract_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObjectList.Count < 2) return; // must have morre than 2 objects
            CSG.Instance().StartOp(CSG.eCSGOp.eSubtraction, UVDLPApp.Instance().SelectedObjectList[0], UVDLPApp.Instance().SelectedObjectList[1]);
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                cmdCreatePrim.ForeColor = ct.ForeColor;
                label1.ForeColor = ct.ForeColor;
                label2.ForeColor = ct.ForeColor;
                label3.ForeColor = ct.ForeColor;
                label4.ForeColor = ct.ForeColor;
                label5.ForeColor = ct.ForeColor;
                label6.ForeColor = ct.ForeColor;
                label7.ForeColor = ct.ForeColor;
                label8.ForeColor = ct.ForeColor;
                label9.ForeColor = ct.ForeColor;
                progressTitle.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                cmdCreatePrim.BackColor = ct.BackColor;
                progressTitle.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel3.BackColor = ct.FrameColor;
                flowLayoutPanel4.BackColor = ct.FrameColor;
                pnlSphere.BackColor = ct.FrameColor;
                pnlCone.BackColor = ct.FrameColor;
                pnlCube.BackColor = ct.FrameColor;
                pnlTorus.BackColor = ct.FrameColor;
            }

        }
    }
}
