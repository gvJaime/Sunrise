using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UV_DLP_3D_Printer.Drivers;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using UV_DLP_3D_Printer.GUI;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlMachineConfig : UserControl //ctlUserPanel //
    {
        private eDriverType m_saved;
        private MachineConfig m_config = new MachineConfig(); // just so it's not blank
        Configs.MonitorConfig curmc = null;
        System.Windows.Forms.TabPage m_userparmtab;
        bool settingup;

        public ctlMachineConfig()
        {
            InitializeComponent();
            settingup = false;
            m_userparmtab = tabControl1.TabPages["tabPage2"];
            UVDLPApp.Instance().m_gui_config.AddControl("groupMCControls", groupMCControls);

        }
        public void ShowMachineControls(bool val)
        {
            groupMCControls.Visible = val;
            if (val == false)
            {
                tabControl1.TabPages.Remove(m_userparmtab);
            }
            else 
            {
                tabControl1.TabPages.Add(m_userparmtab);
            }
        }
        private void FillMultiMon() 
        {
            cmbMultiSel.Items.Clear();
            cmbMultiSel.Items.Add(MachineConfig.eMultiMonType.eVertical.ToString());
            cmbMultiSel.Items.Add(MachineConfig.eMultiMonType.eHorizontal.ToString());
        }
        private void SetData() 
        {
            settingup = true;
            try
            {
                grpMachineConfig.Text = m_config.m_name;
                Monitors.Enabled = true;
                grpPrjSerial.Enabled = true;
                m_saved = m_config.m_driverconfig.m_drivertype;
                FillMultiMon();
                cmbMultiSel.SelectedItem = m_config.m_multimontype.ToString();
                chkOverride.Checked = m_config.m_OverrideRenderSize;
                //list the drivers
                txtPlatWidth.Text = "" + m_config.m_PlatXSize.ToString("0.00");
                txtPlatHeight.Text = "" + m_config.m_PlatYSize.ToString("0.00"); 
                txtPlatTall.Text = m_config.m_PlatZSize.ToString();
                //projwidth.Text = "" + m_config.m_monitorconfig.XRes;
                //projheight.Text = "" + m_config.m_monitorconfig.YRes;
                txtXRes.Text = "" + m_config.XRenderSize.ToString();
                txtYRes.Text = "" + m_config.YRenderSize.ToString();
                SetMachineControls(m_config.MachineControls);
                labelPressApply.Visible = false;
                FillConfiguredDisplays();
                ShowMicron();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            settingup = false;
        }

        void SetMachineControls(string mctl)
        {
            foreach (Control ctl in groupMCControls.Controls)
            {
                if (ctl is CheckBox)
                    ((CheckBox)ctl).Checked = false;
            }
            foreach (char ch in mctl)
            {
                switch (ch)
                {
                    case 'X':
                    case 'Y': checkMCXY.Checked = true; break;
                    case 'Z': checkMCZ.Checked = true; break;
                    case 'T': checkMCTilt.Checked = true; break;
                    case 'E': checkMCExtrude.Checked = true; break;
                    case 'H': checkMCHeater.Checked = true; break;
                    case 'B': checkMCBed.Checked = true; break;
                    case 'P': checkMCProjector.Checked = true; break;
                    case 'G': checkMCGCode.Checked = true; break;
                    case 'D': checkMCMotorDisable.Checked = true; break;
                    case 'S': checkMCSutter.Checked = true; break;
                }
            }
        }

        // clean bad characters from device name -SHS
        private string CleanScreenName(String name)
        {
            name = CleanMonitorString(name);
            int zero_place = name.IndexOf((char)0);
            if (zero_place >= 0)
                name = name.Substring(0,zero_place);
            return name;
        }

        private bool GetData(bool suppresspopup = false) 
        {
            try
            {
                if (cmbMultiSel.SelectedIndex != -1) 
                {
                    m_config.m_multimontype = (MachineConfig.eMultiMonType)Enum.Parse(typeof(MachineConfig.eMultiMonType), cmbMultiSel.SelectedItem.ToString());
                }
                if (m_saved != m_config.m_driverconfig.m_drivertype) 
                {
                    UVDLPApp.Instance().SetupDriver();
                }
                m_config.m_OverrideRenderSize = chkOverride.Checked;
                m_config.m_PlatXSize = double.Parse(txtPlatWidth.Text);
                m_config.m_PlatYSize = double.Parse(txtPlatHeight.Text);
                m_config.m_PlatZSize = double.Parse(txtPlatTall.Text);
                m_config.XRenderSize = int.Parse(txtXRes.Text);
                m_config.YRenderSize = int.Parse(txtYRes.Text);
                m_config.CalcPixPerMM();
                m_config.MachineControls = GetMachineControls();
                labelPressApply.Visible = false;

                if (lbConfigured.SelectedIndex != -1 && curmc !=null)
                {
                    //Configs.MonitorConfig mc = m_config.m_lstMonitorconfigs[lbConfigured.SelectedIndex];
                    curmc.m_usemask = chkEnableMask.Checked;
                }
                return true;
            }
            catch (Exception ex) 
            {
                if (!suppresspopup)
                {
                    DebugLogger.Instance().LogRecord(ex.Message);
                    MessageBox.Show("Please check input parameters\r\n" + ex.Message, "Input Error");
                }
                return false;
            }
        }

        string GetMachineControls()
        {
            String res = "";
            if (checkMCXY.Checked)
                res += "XY";
            if (checkMCZ.Checked)
                res += "Z";
            if (checkMCTilt.Checked)
                res += "T";
            if (checkMCExtrude.Checked)
                res += "E";
            if (checkMCHeater.Checked)
                res += "H";
            if (checkMCBed.Checked)
                res += "B";
            if (checkMCProjector.Checked)
                res += "P";
            if (checkMCGCode.Checked)
                res += "G";
            if (checkMCMotorDisable.Checked)
                res += "D";
            if (checkMCSutter.Checked)
                res += "S";
            return res;
        }

        /// <summary>
        /// On apply
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetData())
                {
                    //UVDLPApp.Instance().SaveCurrentMachineConfig();
                    m_config.Save(m_config.m_filename);
                    // if its the current used config, update the system
                    if (Path.GetFileNameWithoutExtension(m_config.m_filename) == cmbMachineProfiles.SelectedItem.ToString())
                    {
                        ConfigUpdated(m_config.m_filename);
                    }
                    // Close();
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eMachineConfigChanged, "");
                }
                if (m_config.m_lstMonitorconfigs.Count == 0) 
                {
                    MessageBox.Show("You Must Configure at least 1 Display Device");
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

        }
        private void FillMonitors()
        {
            try
            {
                lstMonitors.Items.Clear();
                foreach (Screen s in Screen.AllScreens)
                {
                    string sn = CleanScreenName(s.DeviceName);
                    sn = CleanScreenName(sn); // SHS CleanMonitorString(sn);
                    int xr = s.Bounds.Width;
                    int yr = s.Bounds.Height;
                    string wh = xr.ToString()+ "*"+yr.ToString();
                    lstMonitors.Items.Add(sn + " " + wh);  // -SHS
                }
                if (lstMonitors.Items.Count > 0)
                    lstMonitors.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }

        }
        private void FillConfiguredDisplays() 
        {
            try
            {
                lbConfigured.Items.Clear();
                if (m_config.m_lstMonitorconfigs.Count == 1)
                {
                    // if there is only 1 configured monitor, then we will set the res to that
                    lblMulti.Visible = false;
                    cmbMultiSel.Visible = false;
                    // if we're over-riding the monitor size, leave this alone...
                    if (m_config.m_OverrideRenderSize == false)
                    {
                        int xr = (int)m_config.m_lstMonitorconfigs[0].m_XDLPRes;
                        int yr = (int)m_config.m_lstMonitorconfigs[0].m_YDLPRes;
                        txtXRes.Text = xr.ToString();
                        txtYRes.Text = yr.ToString();
                    }
                }
                else
                {
                    if (m_config.m_lstMonitorconfigs.Count != 0)
                    {
                        lblMulti.Visible = true;
                        cmbMultiSel.Visible = true;
                    }

                }
                foreach (Configs.MonitorConfig mc in m_config.m_lstMonitorconfigs)
                {
                    //lbConfigured.Items.Add(mc.Monitorid);
                    string sn = CleanScreenName(mc.Monitorid);
                    sn = CleanScreenName(sn); // SHS CleanMonitorString(sn);
                    int xr = (int)mc.m_XDLPRes;
                    int yr = (int)mc.m_YDLPRes;
                    string wh = xr.ToString() + "*" + yr.ToString();
                    lbConfigured.Items.Add(sn + " " + wh);
                }
                cmbMultiSel_SelectedIndexChanged(null, null);
                //now select the first monitor in this list (if one is available)
                if (lbConfigured.Items.Count > 0)
                {
                    lbConfigured.SelectedIndex = 0;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

        }
        private void cmdRefreshMonitors_Click(object sender, EventArgs e)
        {
            FillMonitors();
        }

        private void lstMonitors_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the projector width and fill in the projwidth and projheight
            if (lstMonitors.SelectedIndex == -1) return;
            try
            {
                //projwidth.Text = "" + Screen.AllScreens[lstMonitors.SelectedIndex].Bounds.Width;
                //projheight.Text = "" + Screen.AllScreens[lstMonitors.SelectedIndex].Bounds.Height;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }

        }

        private void cmbMachineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                /*
                // enable/disable the group boxes based on this
                MachineConfig.eMachineType t = (MachineConfig.eMachineType)Enum.Parse(typeof(MachineConfig.eMachineType), cmbMachineType.SelectedItem.ToString());
                switch (t)
                {
                    case MachineConfig.eMachineType.FDM:
                        Monitors.Enabled = false;
                        //ProjectorRes.Enabled = false;
                        grpPrjSerial.Enabled = false;
                        break;
                    case MachineConfig.eMachineType.UV_DLP:
                        Monitors.Enabled = true;
                       // ProjectorRes.Enabled = true;
                        grpPrjSerial.Enabled = true;
                        break;
                }
                 * */
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }


        private void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbMachineProfiles.SelectedIndex != -1)
                {
                    if (MessageBox.Show(this, "Are you sure you want to delete this Machine Profile?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK)
                    {
                        //delete file    
                        File.Delete(FNFromIndex(cmbMachineProfiles.SelectedIndex));
                        cmbMachineProfiles.SelectedIndex = 0;
                        UpdateProfiles();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }

        private void UpdateButtons()
        {
            int idx = cmbMachineProfiles.SelectedIndex;
            if (idx == -1)
            {
                cmdRemove.Enabled = false;
            }
            else
            {
                cmdRemove.Enabled = true;
            }
        }
        private string FNFromIndex(int idx)
        {
            string[] filePaths = Directory.GetFiles(UVDLPApp.Instance().m_PathMachines, "*.machine");
            return filePaths[idx];
        }

        private void UpdateProfiles()
        {
            try
            {
                // get a list of profiles in the /machines directory
                string[] filePaths = Directory.GetFiles(UVDLPApp.Instance().m_PathMachines, "*.machine");
                //lstMachineProfiles.Items.Clear();
                cmbMachineProfiles.Items.Clear();
                foreach (String profile in filePaths)
                {
                    String pn = Path.GetFileNameWithoutExtension(profile);
                    //lstMachineProfiles.Items.Add(pn);
                    cmbMachineProfiles.Items.Add(pn);
                }
                MachineConfig cfg = UVDLPApp.Instance().m_printerinfo;
                cmbMachineProfiles.SelectedItem = cfg.m_name; // show the current profile in the combo box
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                            
            }
        }

        private void cmdNew_Click(object sender, EventArgs e)
        {
            // create a new profile, give it a name
            frmProfileName fpn = new frmProfileName();
            fpn.ShowDialog();
            String pf = fpn.ProfileName;
            if (pf.Length > 0)
            {
                //create a new profile with that name
                String fn = UVDLPApp.Instance().m_PathMachines;
                fn += UVDLPApp.m_pathsep;
                fn += pf;
                fn += ".machine";
                MachineConfig mc = new MachineConfig();
                mc.m_name = pf;
                if (!mc.Save(fn))
                {
                    DebugLogger.Instance().LogRecord("Error Saving new machine profile " + fn);
                    return;
                }
                UpdateProfiles();
            }
        }

        private void ConfigUpdated(String filename)
        {
            //load and make active
            if (UVDLPApp.Instance().LoadMachineConfig(filename) != true)
            {
                MessageBox.Show("Error loading machine configuration");
                //should try to load/create a valid one
                return;
            }
            UVDLPApp.Instance().SetupDriver();
            //load this profile
            //string filename = UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + lstMachineProfiles.SelectedItem.ToString() + ".machine";
            m_config = new MachineConfig();
            m_config.Load(filename);
            SetData(); // show the data
            UpdateMainConnection();
            UpdateDisplayConnection();
            //show the info on the GUI
        }

        /// <summary>
        /// The user is selecting a new machine for the current profile here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbMachineProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {           
            if(cmbMachineProfiles.SelectedIndex == -1)
                return;

            string filename = UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + cmbMachineProfiles.SelectedItem.ToString() + ".machine";
            ConfigUpdated(filename);
            //set the user parameters
            ctlUserParamEdit1.Setup(m_config.userParams);
        }

        private void ctlMachineConfig_Load(object sender, EventArgs e)
        {            
            FillMonitors(); // list out the system monitors
            UpdateProfiles();
            SetData();
            UpdateMainConnection();
            UpdateDisplayConnection();
        }
        private void UpdateMainConnection() 
        {
            lblConMachine.Text = m_config.m_driverconfig.m_connection.comname;
        }
        
        private void UpdateDisplayConnection()
        {
            try
            {
                int idx = lbConfigured.SelectedIndex;
                if (idx != -1)
                {
                    Configs.MonitorConfig mc = m_config.m_lstMonitorconfigs[idx];
                    grpPrjSerial.Enabled = true;
                    checkConDispEnable.Checked = mc.m_displayconnectionenabled;
                    lblConDisp.Text = mc.m_displayconnection.comname;
                }
                else 
                {
                    checkConDispEnable.Checked = false;
                    lblConDisp.Text = "";                
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
                checkConDispEnable.Checked = false;
                lblConDisp.Text = "";
                
            }
        }
        
        private void cmdCfgConMch_Click(object sender, EventArgs e)
        {
            frmConnection frmconnect = new frmConnection(ref m_config.m_driverconfig.m_connection,true);
            frmconnect.ShowDialog();
            UpdateMainConnection();
        }

        private void cmdCfgConDsp_Click(object sender, EventArgs e)
        {
            int idx = lbConfigured.SelectedIndex;
            if (idx == -1) return;
            Configs.MonitorConfig mc = m_config.m_lstMonitorconfigs[idx];
            frmConnection frmconnect = new frmConnection(ref mc.m_displayconnection);
            frmconnect.ShowDialog();
            UpdateDisplayConnection();
       
        }

        private void checkConDispEnable_CheckedChanged(object sender, EventArgs e)
        {
            //m_config.m_monitorconfig.m_displayconnectionenabled = checkConDispEnable.Checked;
            //get selected index of lbConfigured
            int idx = lbConfigured.SelectedIndex;
            if (idx == -1) return;
            Configs.MonitorConfig mc = m_config.m_lstMonitorconfigs[idx];
            mc.m_displayconnectionenabled = checkConDispEnable.Checked;

        }
        

        private void lbConfigured_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbConfigured.SelectedIndex == -1)
            {
                cmdRemoveConfigured.Enabled = false;
                grpPrjSerial.Enabled = false;
                curmc = null;
            }
            else 
            {
                cmdRemoveConfigured.Enabled = true;
                // show the resolution
                curmc = m_config.m_lstMonitorconfigs[lbConfigured.SelectedIndex];
                grpPrjSerial.Enabled = true;
                checkConDispEnable.Checked = curmc.m_displayconnectionenabled;
                lblConDisp.Text = curmc.m_displayconnection.comname;
                chkEnableMask.Checked = curmc.m_usemask;                

            }
        }

        private void cmdRemoveConfigured_Click(object sender, EventArgs e)
        {
            if (lbConfigured.SelectedIndex == -1) return;
//            m_config.m_lstMonitorconfigs.Remove(lbConfigured.SelectedIndex);
            try
            {
                m_config.m_lstMonitorconfigs.RemoveAt(lbConfigured.SelectedIndex);
                FillConfiguredDisplays();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void cmdNewMonConfig_Click(object sender, EventArgs e)
        {
            // get the currently selected monitor from lstMonitors
            // create a new Monitor Config
            //set the name
            // refresh the configure monitor list
            if (lstMonitors.SelectedIndex == -1) return;
            string monname = lstMonitors.SelectedItem.ToString();
            Configs.MonitorConfig mc = new Configs.MonitorConfig();
            // set the name 
            //mc.Monitorid = CleanMonitorString(Screen.AllScreens[lstMonitors.SelectedIndex].DeviceName); //CleanMonitorString(monname);
            mc.Monitorid = CleanScreenName(Screen.AllScreens[lstMonitors.SelectedIndex].DeviceName); // SHS - use other clean function
            //set the X/Y resolution
            mc.m_XDLPRes = Screen.AllScreens[lstMonitors.SelectedIndex].Bounds.Width;
            mc.m_YDLPRes = Screen.AllScreens[lstMonitors.SelectedIndex].Bounds.Height;
            m_config.m_lstMonitorconfigs.Add(mc);
            FillConfiguredDisplays();
        }

        private string CleanMonitorString(string str)
        {
            string tmp = str.Replace("\\", string.Empty);
            tmp = tmp.Replace(".", string.Empty);
            tmp = tmp.Trim();
            return tmp;
        }

        private void cmdRemoveConfigured_Load(object sender, EventArgs e)
        {

        }

        private void cmbMultiSel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // when the selected index of this changes, do the following
            //if one monitor is selected, (shouldn't be here)
            //  set the txtXRes to be the monitor width and txtYRes to be the height
            // set monitor 0 recto to be 0,0,1,1
            // if two monitors are configured,
            // chekc the orientation
            // eVertical
            // txtXRes = monitor 0 width
            // txtYRes = 2*monitor 0 height
            // set mon 0 rect to be 0,0,1,.5 (ltrb)
            // set mon 1 rect to be 0,.5,1,1
            //MonitorConfig mc 
            int xr, yr;
            if (m_config.m_lstMonitorconfigs.Count == 1) 
            {
                m_config.m_lstMonitorconfigs[0].m_monitorrect.top = 0;
                m_config.m_lstMonitorconfigs[0].m_monitorrect.left = 0;
                m_config.m_lstMonitorconfigs[0].m_monitorrect.right = 1;
                m_config.m_lstMonitorconfigs[0].m_monitorrect.bottom = 1;
                if (m_config.m_OverrideRenderSize == false)
                {
                    xr = (int)m_config.m_lstMonitorconfigs[0].m_XDLPRes;
                    yr = (int)m_config.m_lstMonitorconfigs[0].m_YDLPRes;
                    txtXRes.Text = xr.ToString();
                    txtYRes.Text = yr.ToString();
                }
                else 
                {
                    txtXRes.Text = m_config.XRenderSize.ToString();
                    txtYRes.Text = m_config.YRenderSize.ToString();
                
                }
            }else if( m_config.m_lstMonitorconfigs.Count == 2) 
            {
                if (cmbMultiSel.SelectedIndex != -1)
                {
                    MachineConfig.eMultiMonType orient = (MachineConfig.eMultiMonType)Enum.Parse(typeof(MachineConfig.eMultiMonType), cmbMultiSel.SelectedItem.ToString());
                    switch (orient) 
                    {
                        case MachineConfig.eMultiMonType.eHorizontal:
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.top = 0;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.left = 0;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.right = .5f;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.bottom = 1;

                            m_config.m_lstMonitorconfigs[1].m_monitorrect.top = 0;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.left = .5f;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.right = 1;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.bottom = 1;
                            
                            xr = (int)m_config.m_lstMonitorconfigs[0].m_XDLPRes;
                            yr = (int)m_config.m_lstMonitorconfigs[0].m_YDLPRes;
                            //double the width - assume mon0 and mon1 are same resolution
                            xr *= 2;
                            txtXRes.Text = xr.ToString();
                            txtYRes.Text = yr.ToString();

                            break;
                        case MachineConfig.eMultiMonType.eVertical:
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.top = 0;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.left = 0;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.right = 1;
                            m_config.m_lstMonitorconfigs[0].m_monitorrect.bottom = .5f;

                            m_config.m_lstMonitorconfigs[1].m_monitorrect.top = .5f;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.left = 0;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.right = 1;
                            m_config.m_lstMonitorconfigs[1].m_monitorrect.bottom = 1;
                            
                            xr = (int)m_config.m_lstMonitorconfigs[0].m_XDLPRes;
                            yr = (int)m_config.m_lstMonitorconfigs[0].m_YDLPRes;
                            //double the width - assume mon0 and mon1 are same resolution
                            yr *= 2;
                            txtXRes.Text = xr.ToString();
                            txtYRes.Text = yr.ToString();
                            break;
                    }
                }
                else 
                {
                    //error
                }                
            }
        }

        private void checkMCXXXX_CheckedChanged(object sender, EventArgs e)
        {
            labelPressApply.Visible = true;
        }

        private void chkEnableMask_CheckedChanged(object sender, EventArgs e)
        {
            // 
            cmdConfigMask.Enabled = chkEnableMask.Checked;
        }

        private void cmdConfigMask_Click(object sender, EventArgs e)
        {
            if (curmc != null)
            {
                openFileDialog1.FileName = curmc.m_brightmask_filename;
                if (openFileDialog1.ShowDialog() == DialogResult.OK) 
                {
                    string fname = openFileDialog1.FileName;
                    //string abspath;
                    //string relpath;
                    curmc.m_brightmask_filename = fname;
                    //now load the file
                    curmc.m_mask = new Bitmap(curmc.m_brightmask_filename);
                }
            }
        }

        private void cmdAdjust_Click(object sender, EventArgs e)
        {
            frmBuildSizeCalib bsc = new frmBuildSizeCalib();
            bsc.setModelSize(2.0f, 2.0f);
            bsc.setPlatformSize((float)m_config.m_PlatXSize, (float)m_config.m_PlatYSize);
            if (bsc.ShowDialog() == DialogResult.OK) 
            {
                try
                {
                    m_config.m_PlatXSize = bsc.calcplatoformsizeX;
                    m_config.m_PlatYSize = bsc.calcplatoformsizeY;
                }
                catch (Exception )
                {
                
                }
                SetData();
            }
        }
        private void ShowMicron() 
        {
            try
            {
                lblMicronX.Text = string.Format("{0:0} microns X",(( m_config.m_PlatXSize / m_config.XRenderSize)*1000));
                lblMicronY.Text = string.Format("{0:0} microns Y", ((m_config.m_PlatYSize / m_config.YRenderSize) * 1000));
            }
            catch (Exception ) 
            {
            
            }
        }

        private void txtPlatWidth_TextChanged(object sender, EventArgs e)
        {
            if (settingup) return;
            GetData(true);
            ShowMicron();
        }

        private void txtPlatHeight_TextChanged(object sender, EventArgs e)
        {
            if (settingup) return;
            GetData(true);
            ShowMicron();
        }
    }
}
