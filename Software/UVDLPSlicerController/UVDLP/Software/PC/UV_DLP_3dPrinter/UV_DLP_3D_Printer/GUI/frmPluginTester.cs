using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Plugin;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmPluginTester : Form
    {
        PluginEntry m_ip;
        public frmPluginTester()
        {
            InitializeComponent();
            SetupPlugins();
        }
        public void SetupPlugins() 
        {
            lvplugins.Clear();
            foreach (PluginEntry ip in UVDLPApp.Instance().m_plugins)
            {
                lvplugins.Items.Add(ip.m_plugin.Name);
            }
        }
        private void ListContents() 
        {
            if (m_ip == null) return;
            lvcontents.Items.Clear();
            foreach (PluginItem pi in m_ip.m_plugin.GetPluginItems) 
            {
                lvcontents.Items.Add(pi.m_name + " / " + pi.m_type.ToString());
            }
        }
        private void lvplugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int idx = lvplugins.SelectedIndices[0];
                m_ip = UVDLPApp.Instance().m_plugins[idx];
                ListContents();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void SetControl(UserControl ctl) 
        {
            if (ctl == null) return;
            panel1.Controls.Clear(); // remove all previous controls
            panel1.Controls.Add(ctl); // add our user control
        }
        private void SetString(string dat) 
        {
            lblData.Text = dat;
        }
        private void SetInt(int dat)
        {
            lblData.Text = ""+ dat;
        }
        private void SetImage(Bitmap bmp) 
        {
            pic1.Image = bmp;
        }
        private void lvcontents_SelectedIndexChanged(object sender, EventArgs e)
        {
            // the user clicked on an item here, get the plug in item, then retrieve the actual
            // data from the plugin
            try
            {
                int idx = lvcontents.SelectedIndices[0]; // get the index
                PluginItem pi = m_ip.m_plugin.GetPluginItems[idx]; // get the single manifest item
                switch (pi.m_type)
                {
                    case ePlItemType.eString:
                        SetString(m_ip.m_plugin.GetString(pi.m_name));
                        break;
                    case ePlItemType.eInt:
                        SetInt(m_ip.m_plugin.GetInt(pi.m_name));
                        break;
                    case ePlItemType.eImage:
                        SetImage(m_ip.m_plugin.GetImage(pi.m_name));
                        break;
                    case ePlItemType.eControl:
                        SetControl(m_ip.m_plugin.GetControl(pi.m_name));
                        break;
                    case ePlItemType.eBin:
                        SetString(Utility.ByteArrayToString(m_ip.m_plugin.GetBinary(pi.m_name)));
                        break;
                    case ePlItemType.eFunction:
                        m_ip.m_plugin.ExecuteFunction(pi.m_name);
                        break;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

    }
}
