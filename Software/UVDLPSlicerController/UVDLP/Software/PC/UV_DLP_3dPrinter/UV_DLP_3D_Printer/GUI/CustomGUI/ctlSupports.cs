using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Configs;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlSupports : ctlUserPanel
    {
        private SupportConfig m_sc;
        private bool m_settingData;
        private bool m_changingData;
        private bool m_numFBSelected;
        private const int opensize = 622;
        public ctlSupports()
        {
            InitializeComponent();
            UVDLPApp.Instance().m_supportgenerator.SupportEvent += new SupportGeneratorEvent(SupEvent);
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEvent);
            m_sc = UVDLPApp.Instance().m_supportconfig; // should copy really
            SetData();
            m_changingData = false;
            m_numFBSelected = true;
            ListSupports();
        }
        public void FixForeColor(Color clr) 
        {
            label1.ForeColor = clr;
            label2.ForeColor = clr;
            label3.ForeColor = clr;
            label4.ForeColor = clr;
            label6.ForeColor = clr;
            label7.ForeColor = clr;
            labelAutoSup.ForeColor = clr;
            label9.ForeColor = clr;
            lbSupports.ForeColor = clr;
        }
        private void AppEvent(eAppEvent ev,string message)
        {
            switch (ev) 
            {
                case eAppEvent.eModelRemoved:
                    ListSupports();
                    break;
                case eAppEvent.eModelAdded:
                    ListSupports();
                    break;
                case eAppEvent.eObjectSelected:
                    Object3d sel = UVDLPApp.Instance().SelectedObject;
                    if (sel != null) 
                    {
                        //highlist the selected
                        HighLightSelected(sel);
                    }
                    break;
            }
        }

        private void HighLightSelected(Object3d sel) 
        {
            int idx = 0;
            foreach (ListViewItem lvi in lbSupports.Items) 
            {
                Object3d obj = (Object3d)lvi.Tag;
                if (obj == sel) 
                {
                    //lbSupports.sel
                    lbSupports.Items[idx].Selected = true;
                    break;
                }
                idx++;
            }
        }

        private void SetData()
        {
            m_settingData = true;
            try
            {
                numFB.FloatVal = (float)m_sc.fbrad;
                numFT.FloatVal = (float)m_sc.ftrad;
                numHB.FloatVal = (float)m_sc.hbrad;
                numHT.FloatVal = (float)m_sc.htrad;
                numFB1.FloatVal = (float)m_sc.fbrad2;
                numY.FloatVal = (float)m_sc.yspace;
                numX.FloatVal = (float)m_sc.xspace;
                numGap.FloatVal = (float)m_sc.mingap;
                chkOnlyDownward.Checked = m_sc.m_onlydownward;
                chkSupportAllScene.Checked = !m_sc.m_onlyselected;
                numDownAngle.FloatVal = (float)m_sc.downwardAngle;
                switch (m_sc.eSupType)
                {
                    case SupportConfig.eAUTOSUPPORTTYPE.eBON:
                        cmbSupType.SelectedIndex = 0;
                        break;
                    case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE:
                        cmbSupType.SelectedIndex = 1;
                        break;
//                    case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE2:
  //                      cmbSupType.SelectedIndex = 2;
    //                    break;
                }
                UpdateForSupportType();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            m_settingData = false;
        }
        private bool GetData()
        {
            try
            {
                m_sc.fbrad = (double)numFB.FloatVal;
                m_sc.ftrad = (double)numFT.FloatVal;
                m_sc.hbrad = (double)numHB.FloatVal;
                m_sc.htrad = (double)numHT.FloatVal;
                m_sc.fbrad2 = (double)numFB1.FloatVal;
                m_sc.yspace = (double)numY.FloatVal;
                m_sc.xspace = (double)numX.FloatVal;
                m_sc.mingap = (double)numGap.FloatVal;
                m_sc.m_onlydownward = chkOnlyDownward.Checked;
                m_sc.m_onlyselected = !chkSupportAllScene.Checked;
                m_sc.downwardAngle = numDownAngle.FloatVal;
                pictureSupport.Invalidate();
                switch (cmbSupType.SelectedIndex) 
                {
                    case 0:
                        m_sc.eSupType = SupportConfig.eAUTOSUPPORTTYPE.eBON;
                        break;
                    case 1:
                        m_sc.eSupType = SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE;
                        break;
//                    case 2:
  //                      m_sc.eSupType = SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE2;
    //                    break;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!m_changingData)
                    DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }
        
        public void SupEvent(SupportEvent ev, string message, Object obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SupEvent(ev, message, obj); }));
            }
            else
            {
                try
                {
                    switch (ev)
                    {
                        case SupportEvent.eCompleted:
                            //close this dialog
                            progressTitle.Value = 0;
                            progressTitle.Text = "Supports";
                            //DialogResult = System.Windows.Forms.DialogResult.OK;
                            //Close();
                            ListSupports();
                            break;
                        case SupportEvent.eCancel:
                            break;
                        case SupportEvent.eProgress:
                            // P
                            string[] toks = message.Split('/');
                            int cs = int.Parse(toks[0]);
                            int ts = int.Parse(toks[1]);
                            if (cs == 0) // set up the prog bar on the first step
                            {
                                progressTitle.Maximum = ts;
                                progressTitle.Text = "Generating...";
                            }
                            else
                            {
                                progressTitle.Value = cs;
                            }
                            break;
                        case SupportEvent.eStarted:
                            break;
                        case SupportEvent.eSupportGenerated:
                            ListSupports();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex.Message);
                }
            }
        }

        private void buttAddSupport_Click(object sender, EventArgs e)
        {
            if (buttAddSupport.Checked)
            {
                UVDLPApp.Instance().SupportEditMode = UVDLPApp.eSupportEditMode.eAddSupport;
            }
            else 
            {
                UVDLPApp.Instance().SupportEditMode = UVDLPApp.eSupportEditMode.eNone;
            }
        }

        private void buttGenBase_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().AddSupportBase(!chkSupportAllScene.Checked);
        }
        
        private void buttAutoSupport_Click(object sender, EventArgs e)
        {
            try
            {
                GetData();
                UVDLPApp.Instance().m_supportconfig = m_sc; // should copy really
                UVDLPApp.Instance().StartAddSupports(); // start the support generation
                UVDLPApp.Instance().SaveSupportConfig(UVDLPApp.Instance().m_appconfig.SupportConfigName);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }

        }

        private void chkDownPolys_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null) return;
            // tell the 3d engine to only show polygons from objects that are facing downward at the specified angle
            if (chkDownPolys.Checked == true)
            {
                double angle = (double)numDownAngle.FloatVal;
                // I think this should affect all objects in the scene
                // and it should be more of a global setting 
                UVDLPApp.Instance().SelectedObject.MarkPolysDown(angle);
            }
            else
            {
                // restore the object
                UVDLPApp.Instance().SelectedObject.ClearPolyTags();
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
        }
        private PointF[] GetLinePoints(float[] coords)
        {
            int npts = coords.Length / 2;
            PointF[] pts = new PointF[npts];
            for (int i = 0; i < npts; i++)
            {
                pts[i] = new PointF(coords[i * 2], coords[i * 2 + 1]);
            }
            return pts;
        }

        private void DrawGuide(PaintEventArgs e, float x, float y, float edge, Control c)
        {
            float yend = c.Location.Y + c.Height / 2 - pictureSupport.Location.Y;
            float xend = pictureSupport.Width - edge;
            if (x > xend)
                return;
            Pen pen = new Pen(Color.Black, 1);
            PointF[] pts = GetLinePoints(new float[] { x, y, xend, y, xend, yend, pictureSupport.Width, yend });
            e.Graphics.DrawLines(pen, pts);
        }

        private void pictureSupport_Paint(object sender, PaintEventArgs e)
        {
            float fullHeight = pictureSupport.Height;
            float yfb = 0.95f * fullHeight;
            float yft = 0.75f * fullHeight;
            float yhb = 0.25f * fullHeight;
            float yht = 0.05f * fullHeight;

            float halfPixPermm = (float)pictureSupport.Width / 10.0f;
            float xc = (float)pictureSupport.Width * 0.4f;

            float xfb = (float)(halfPixPermm * m_sc.fbrad);
            float xfb2 = (float)(halfPixPermm * m_sc.fbrad2);
            float xft = (float)(halfPixPermm * m_sc.ftrad);
            float xhb = (float)(halfPixPermm * m_sc.hbrad);
            float xht = (float)(halfPixPermm * m_sc.htrad);

            SolidBrush greenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
            SolidBrush redBrush = new SolidBrush(Color.Pink);

            PointF[] supportHead = GetLinePoints(new float[] {
                xc - xht, yht, xc + xht, yht, xc + xhb, yhb, xc - xhb, yhb});
            PointF[] supportBody = GetLinePoints(new float[] {
                xc - xhb, yhb, xc + xhb, yhb, xc + xft, yft, xc - xft, yft});
            PointF[] supportFoot = GetLinePoints(new float[] {
                xc - xft, yft, xc + xft, yft, xc + xfb, yfb, xc - xfb, yfb});
            PointF[] supportFoot2 = GetLinePoints(new float[] {
                xc - xft, yft, xc + xft, yft, xc + xfb2, yfb, xc - xfb2, yfb});

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Pen blackPen = new Pen(Color.Black, 2);
            e.Graphics.FillPolygon(greenBrush, supportHead);
            e.Graphics.FillPolygon(yellowBrush, supportBody);
            if (m_numFBSelected)
                e.Graphics.FillPolygon(redBrush, supportFoot);
            else
                e.Graphics.FillPolygon(redBrush, supportFoot2);
            e.Graphics.DrawPolygon(blackPen, supportHead);
            e.Graphics.DrawPolygon(blackPen, supportBody);
            if (m_numFBSelected)
                e.Graphics.DrawPolygon(blackPen, supportFoot);
            else
                e.Graphics.DrawPolygon(blackPen, supportFoot2);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            DrawGuide(e, xc + xht + 3, yht, 4, numHT);
            DrawGuide(e, xc + xhb + 3, yhb, 8, numHB);
            DrawGuide(e, xc + xft + 3, yft, 8, numFT);
            if (m_numFBSelected)
                DrawGuide(e, xc + xfb + 3, yfb, 4, numFB);
            else
                DrawGuide(e, xc + xfb2 + 3, yfb, 4, numFB1);
        }
 
        private void num_ValueChanged(object sender, EventArgs e)
        {
            if (m_settingData)
                return;
            m_changingData = true;
            GetData();
            m_changingData = false;
            pictureSupport.Invalidate();
        }

        private void numFB_Enter(object sender, EventArgs e)
        {
            m_numFBSelected = true;
            pictureSupport.Invalidate();
        }

        private void numFB1_Enter(object sender, EventArgs e)
        {
            m_numFBSelected = false;
            pictureSupport.Invalidate();
        }

        private void numDownAngle_ValueChanged(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            //UVDLPApp.Instance().SelectedObject.
            if (chkDownPolys.Checked == true)
            {
                double angle = (double)numDownAngle.FloatVal;
                UVDLPApp.Instance().SelectedObject.MarkPolysDown(angle);
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
            }
        }
        private void UpdateForSupportType() 
        {
            switch (m_sc.eSupType)
            {
                case SupportConfig.eAUTOSUPPORTTYPE.eBON:
                    chkOnlyDownward.Visible = true;
                    //labelAutoSup.Visible = true;
                    labelAutoSup.Text = "Automatic Support";
                    label6.Visible = true;
                    label7.Visible = true;
                    numX.Visible = true;
                    numGap.Visible = false;
                    numY.Visible = true;
                    
                    break;
                case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE:
                //case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE2:
                    chkOnlyDownward.Visible = true;
                    //labelAutoSup.Visible = false;
                    labelAutoSup.Text = "Minimum Gap (mm)";
                    label6.Visible = false;
                    label7.Visible = false;
                    numX.Visible = false;
                    numGap.Visible = true;
                    numY.Visible = false;

                    break;
            }        
        }
        private void cmbSupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetData();
            UpdateForSupportType();
        }
        public void ListSupports() 
        {
            if (UVDLPApp.Instance().m_supportgenerator.Generating)
                return; // avoid cross-thread using of support list
            lbSupports.Items.Clear();
            foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects) 
            {
                if ((obj.tag == Object3d.OBJ_SUPPORT) || (obj.tag == Object3d.OBJ_SUPPORT_BASE)) 
                {
                    ListViewItem lvi = new ListViewItem(obj.Name);
                    lvi.Tag = obj;
                    lbSupports.Items.Add(lvi);
                }
            }
        
        }
        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                ctlTitle1.ForeColor = ct.ForeColor;
                label1.ForeColor = ct.ForeColor;
                label2.ForeColor = ct.ForeColor;
                label3.ForeColor = ct.ForeColor;
                label4.ForeColor = ct.ForeColor;
                labelAutoSup.ForeColor = ct.ForeColor;
                label6.ForeColor = ct.ForeColor;
                label7.ForeColor = ct.ForeColor;
                label9.ForeColor = ct.ForeColor;
                cmbSupType.ForeColor = ct.ForeColor;
                lbSupports.ForeColor = ct.ForeColor;
                cmdRemoveSupports.ForeColor = ct.ForeColor;
                progressTitle.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                ctlTitle1.BackColor = ct.BackColor;
                BackColor = ct.BackColor;
                cmbSupType.BackColor = ct.BackColor;
                //flowLayoutPanel2.BackColor = ct.BackColor;
                progressTitle.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
            {
                
                flowLayoutPanel1.BackColor = ct.FrameColor;
                panel1.BackColor = ct.FrameColor;
                panel2.BackColor = ct.FrameColor;
                panelSuppotShape.BackColor = ct.FrameColor;
                panel3.BackColor = ct.FrameColor;
                lbSupports.BackColor = ct.FrameColor;
                cmdRemoveSupports.BackColor = ct.BackColor;
            }

        }

        private void lbSupports_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the selected item
            try
            {
                ListView.SelectedListViewItemCollection slvi =  lbSupports.SelectedItems;
                if (slvi.Count > 0) 
                {
                    ListViewItem lvi = slvi[0];
                    Object3d obj = (Object3d)lvi.Tag;
                    UVDLPApp.Instance().SelectedObject = obj;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void cmdRemoveSupports_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().RemoveAllSupports();
        }

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                this.Height = opensize + 5;
            }
            else
            {
                this.Height = ctlTitle1.Height + 5;
            }
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }
    }
}
