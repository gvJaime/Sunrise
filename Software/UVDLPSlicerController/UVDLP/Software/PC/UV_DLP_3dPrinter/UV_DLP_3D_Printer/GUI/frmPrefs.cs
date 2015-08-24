using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmPrefs : Form
    {
        public frmPrefs()
        {
            InitializeComponent();
            SetData();
        }
        /*
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                lblSlic3rlocation.Text = openFileDialog1.FileName;
            }
        }
         * */
        private void SetData() 
        {
            //lblSlic3rlocation.Text = UVDLPApp.Instance().m_appconfig.m_slic3rloc;
            panelback.BackColor = UVDLPApp.Instance().m_appconfig.m_backgroundcolor;
            panelfore.BackColor = UVDLPApp.Instance().m_appconfig.m_foregroundcolor;
            chkDriverLog.Checked = UVDLPApp.Instance().m_appconfig.m_driverdebuglog;
            chkIgnoreGCRsp.Checked = UVDLPApp.Instance().m_appconfig.m_ignore_response;
           // txtSlic3rParams.Text = UVDLPApp.Instance().m_appconfig.m_slic3rparameters;
        }
        private void GetData() 
        {
            //UVDLPApp.Instance().m_appconfig.m_slic3rloc = lblSlic3rlocation.Text;
            UVDLPApp.Instance().m_appconfig.m_backgroundcolor = panelback.BackColor;
            UVDLPApp.Instance().m_appconfig.m_foregroundcolor = panelfore.BackColor;
            UVDLPApp.Instance().m_appconfig.m_driverdebuglog = chkDriverLog.Checked;
            UVDLPApp.Instance().m_appconfig.m_ignore_response = chkIgnoreGCRsp.Checked;
            //UVDLPApp.Instance().m_appconfig.m_slic3rparameters = txtSlic3rParams.Text;
            UVDLPApp.Instance().SaveAppConfig();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            GetData();
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmdselectfore_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = panelfore.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                panelfore.BackColor = colorDialog1.Color;
            }
        }

        private void cmdselectback_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = panelback.BackColor;
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                panelback.BackColor = colorDialog1.Color;
            }
        }
    }
}
