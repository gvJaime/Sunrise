using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlSupport : UserControl
    {
        private SupportConfig m_sc;
        private bool m_settingData;
        private bool m_changingData;
        private bool m_numFBSelected;

        public ctlSupport()
        {
            InitializeComponent();
            UVDLPApp.Instance().m_supportgenerator.SupportEvent += new SupportGeneratorEvent(SupEvent);
            //m_sc = new SupportConfig();
            m_sc = UVDLPApp.Instance().m_supportconfig; // should copy really
            SetData();
            m_changingData = false;
            m_numFBSelected = true;
        }
        private void SetData()
        {
            m_settingData = true;
            try
            {
                numFB.Value = (decimal)m_sc.fbrad;
                numFT.Value = (decimal)m_sc.ftrad;
                numHB.Value = (decimal)m_sc.hbrad;
                numHT.Value = (decimal)m_sc.htrad;
                numFB1.Value = (decimal)m_sc.fbrad2;
                numX.Value = (decimal)m_sc.xspace;
                numY.Value = (decimal)m_sc.yspace;
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
                m_sc.fbrad = (double)numFB.Value;
                m_sc.ftrad = (double)numFT.Value;
                m_sc.hbrad = (double)numHB.Value;
                m_sc.htrad = (double)numHT.Value;
                m_sc.fbrad2 = (double)numFB1.Value;
                m_sc.xspace = (double)numX.Value;
                m_sc.yspace = (double)numY.Value;
                m_sc.m_onlydownward = chkOnlyDownward.Checked;
                pictureSupport.Invalidate();
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
                            progressBar1.Value = 0;
                            //DialogResult = System.Windows.Forms.DialogResult.OK;
                            //Close();
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
                                progressBar1.Maximum = ts;
                            }
                            else
                            {
                                progressBar1.Value = cs;
                            }
                            break;
                        case SupportEvent.eStarted:
                            break;
                        case SupportEvent.eSupportGenerated:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex.Message);
                }
            }
        }

        private void cmdAutoSupport_Click(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            GetData();
            UVDLPApp.Instance().AddSupport();
            UVDLPApp.Instance().SaveSupportConfig(UVDLPApp.Instance().m_appconfig.SupportConfigName);
        }

        private void chkDownPolys_CheckedChanged(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null) return;
            // tell the 3d engine to only show polygons from objects that are facing downward at the specified angle
            if (chkDownPolys.Checked == true)
            {
                double angle = (double)numdownangle.Value;
                UVDLPApp.Instance().SelectedObject.MarkPolysDown(angle);                
            }
            else 
            {
                // restore the object
                UVDLPApp.Instance().SelectedObject.ClearPolyTags();
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
        }

        private PointF[] GetLinePoints(float [] coords)
        {
            int npts = coords.Length / 2;
            PointF [] pts = new PointF[npts];
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
            PointF [] pts = GetLinePoints(new float [] {x, y, xend, y, xend, yend, pictureSupport.Width, yend} );
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

        private void ctlSupport_Load(object sender, EventArgs e)
        {
            pictureSupport.BackColor = groupSupportParam.BackColor;
        }

        private void numdownangle_ValueChanged(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null) return;
            if (chkDownPolys.Checked == true)
            {
                double angle = (double)numdownangle.Value;
                UVDLPApp.Instance().SelectedObject.MarkPolysDown(angle);
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
        }

    }
}
