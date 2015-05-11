using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Plugin;
using CreationWorkshop.Licensing;
using UV_DLP_3D_Printer.Licensing;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmPluginManager : Form
    {
        PluginEntry ipsel;
        public frmPluginManager()
        {
            InitializeComponent();
            SetupPlugins();
            ipsel = null;
            UpdateButtons();
        }
        public void SetupPlugins()
        {
            lvplugins.Items.Clear();
            foreach (PluginEntry ip in UVDLPApp.Instance().m_plugins)
            {
                ListViewItem lvi = new ListViewItem(ip.m_plugin.Name);
                lvi.SubItems.Add(ip.m_licensed.ToString());
                lvi.SubItems.Add(ip.m_enabled.ToString());
                lvi.SubItems.Add(ip.m_plugin.GetString("Version"));
                lvi.SubItems.Add(ip.m_plugin.GetString("Description"));
                lvplugins.Items.Add(lvi);
            }
        }

        private void UpdateButtons() 
        {
            if (ipsel == null)
            {
                cmdEnable.Enabled = false;
                return;
            }
            else 
            {
                cmdEnable.Enabled = true;
            }
            if (ipsel.m_licensed)
            {
                cmdLicense.Enabled = false;
            }
            else 
            {
                cmdLicense.Enabled = true;
            }
            if (ipsel.m_enabled)
            {
                cmdEnable.Text = "Disable";
            }
            else 
            {
                cmdEnable.Text = "Enable";
            }
        }

        private void lvplugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lvplugins.SelectedIndices.Count >0 )
            {
                int idx = lvplugins.SelectedIndices[0];
                ipsel = UVDLPApp.Instance().m_plugins[idx];
                UpdateButtons();
            }
        }


        private void cmdLicense_Click(object sender, EventArgs e)
        {
            // get the text string from 
            string license = txtLicense.Text;
            license = license.Trim();
            if (license.Length != 32) 
            {
                MessageBox.Show("Invalid license, please re-check");
                return;
            }
            LicenseKey lk = new LicenseKey();
            try
            {
                lk.Init(license);
                if (lk.valid)
                {
                    txtLicense.Text = "";
                    //should really check, vendor id against plugin vendor id
                    KeyRing.Instance().m_keys.Add(lk);
                    string licensefile = UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + "licenses.key";
                    if (KeyRing.Instance().Save(licensefile))
                    {
                        MessageBox.Show("License Added to Keyring, Restart to take effect");
                    }
                    else
                    {
                        MessageBox.Show("Error Saving Keyring");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid license, please re-check");
                }
            }
            catch (Exception) 
            {
                MessageBox.Show("Error validating license, please re-check");
            }
        }

        private void cmdEnable_Click(object sender, EventArgs e)
        {
            if (ipsel == null)
            {
                return;
            }
            if (cmdEnable.Text.Contains("Disable"))
            {
                //disable the current ipsel
                ipsel.m_enabled = false;
            }
            else 
            {
                ipsel.m_enabled = true;
            }
            // save the enabled status
            UVDLPApp.Instance().m_pluginstates.Save(); // save the state of the plugin enabled statuses
            //now refresh the buttons
            UpdateButtons();
            //refresh the plugin list as well
            SetupPlugins();
        }
    }
}
