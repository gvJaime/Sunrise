using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer;
using UV_DLP_3D_Printer.Drivers;
using System.Drawing;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer
{
    /// <summary>
    /// This class keeps track of the multiple frmDLP's used in a multi-monitor setup and display
    /// </summary>
    public class DisplayManager
    {
        private static DisplayManager m_instance = null;
        private List<frmDLP> m_displays;
        // this should listen for a machine configuration change event
        // so it can automatically set up for the correct screens
        public static DisplayManager Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new DisplayManager();
            }
            return m_instance;
        }

        public bool GetMonitorResoultion(string monitorid, ref int xres, ref int yres) 
        {
            bool retval = false;
            // iterate through screens
            foreach (Screen s in Screen.AllScreens)
            {
                string mn = Utility.CleanMonitorString(s.DeviceName);
                string mid = Utility.CleanMonitorString(monitorid);
                if (mn.Contains(mid))
                {
                    xres = s.Bounds.Width;
                    yres = s.Bounds.Height;
                    retval = true;
                    break;
                }
            }
            return retval;
        }

        public bool SendProjCommand(string displayname, string commandname) 
        {
            try
            {
                // get the projector command for 'on'
                // ACER_ON
                ProjectorCommand pcmd = UVDLPApp.Instance().m_proj_cmd_lst.FindByName(commandname);
                if (pcmd != null)
                {
                    DeviceDriver dd = DisplayManager.Instance().FindDisplaySerialPortDriverByName(displayname);
                    if (dd != null)
                    {
                        if (dd.Connected)
                        {
                            byte[] data = pcmd.GetBytes();
                            dd.Write(data, data.Length);
                            return true;
                        }
                        else 
                        {
                            DebugLogger.Instance().LogError("Projector Driver not connected");    
                        }
                    }
                    else
                    {
                        DebugLogger.Instance().LogError("Projector Driver not found");
                    }
                }
                else
                {
                    DebugLogger.Instance().LogError("Projector command not found");
                }

            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }
        public DeviceDriver FindDisplaySerialPortDriverByName(string name) 
        {
            //find the serial port for display 2
            foreach (MonitorConfig mc in UVDLPApp.Instance().m_printerinfo.m_lstMonitorconfigs)
            {
                if (mc.Monitorid.Contains(name)) // we found our monitor
                {
                    if (mc.m_displayconnectionenabled)
                    {
                        DeviceDriver driver = UVDLPApp.Instance().m_deviceinterface.FindProjDriverByComName(mc.m_displayconnection.comname);
                        return driver;
                    }
                    break;
                }
                // find the display by name, we should have a function for this already somewhere
            }
            return null;
        }
        /**/
        private DisplayManager() 
        {            
            m_displays = new List<frmDLP>();
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEv);
        }
        /// <summary>
        /// This function is different from the PrintLayer delegate in the frmDLP
        /// This function is used to preview a slice on the display
        /// </summary>
        public void PreviewOnDisplays( Image image) 
        {
            foreach (frmDLP frm in m_displays) 
            {
                frm.ShowImage(image,BuildManager.SLICE_NORMAL,-1); // use layer -1 to indicate this is a preview image
            }
        }
        /// <summary>
        /// This function removes all previous device drivers for the projector
        /// serial ports, it will then re-create the drivers, configure them, and 
        /// add them to the device interface for later use.
        /// </summary>
        private void RegenProjectorSerialPorts() 
        {
            // this will remove all projector device drivers.
            UVDLPApp.Instance().m_deviceinterface.RemoveAllProjDrivers();
            foreach (MonitorConfig mc in UVDLPApp.Instance().m_printerinfo.m_lstMonitorconfigs) 
            {
                //check to see if we're recreatiung the port
                if (mc.m_displayconnectionenabled == true)
                {
                    DeviceDriver dev = DriverFactory.Create(eDriverType.eGENERIC);
                    dev.Configure(mc.m_displayconnection);
                    UVDLPApp.Instance().m_deviceinterface.AddDriver(dev);
                }
            }
        }

        private void GenerateForms() 
        {
            if (m_displays != null) 
            {
                //close all previous forms and get rid of them
                foreach (frmDLP frm in m_displays) 
                {
                    frm.Hide();
                    frm.Dispose();
                    //frm = null;
                }
                m_displays.Clear();
            }
            foreach (MonitorConfig mc in UVDLPApp.Instance().m_printerinfo.m_lstMonitorconfigs) 
            {
                frmDLP frm = new frmDLP();
                frm.Setup(mc.Monitorid, mc);
                m_displays.Add(frm);
            }

        }
        public void AppEv(eAppEvent ev, string s) 
        {
            switch (ev)
            {
                case eAppEvent.eMachineConfigChanged:
                    //regenerate all frmDLP's from the configuration monitor settings
                    GenerateForms();
                    //regenerate all device drivers fro mthe monitor configurations
                    RegenProjectorSerialPorts();
                    break;
            }
        }
        /// <summary>
        /// Based on the current machine configuration profile,
        /// return the number of configured screens
        /// </summary>
        /// <returns></returns>
        public int GetNumberofActiveScreens() 
        {
            try
            {
                return UVDLPApp.Instance().m_printerinfo.m_lstMonitorconfigs.Count;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return 0;
        }
        /// <summary>
        /// shows all dlp as blank
        /// </summary>
        public void showBlankDLPs() 
        {
            ShowDLPScreens();
            UVDLPApp.Instance().m_buildmgr.ShowBlank(UVDLPApp.Instance().m_buildparms.xres, UVDLPApp.Instance().m_buildparms.yres);

        }
        public bool ShowDLPScreens() 
        {
            bool ret = true;
            foreach (frmDLP frm in m_displays) 
            {
                if (!frm.ShowDLPScreen()) 
                {
                    ret = false;
                }
            }
            return ret;
        }
        public void HideDLPScreens() 
        {
            foreach (frmDLP frm in m_displays)
            {
                frm.HideDLPScreen();
            }        
        }
        /// <summary>
        /// This function will iterate through all monitor configurations,
        /// and attempt to open the serial control ports for each monitor.
        /// This function can be called multiple times to keep trying to open ports
        /// </summary>
        public void ConnectMonitorSerials() 
        {
            UVDLPApp.Instance().m_deviceinterface.ConnectAllProjDrivers();
        }
        /// <summary>
        /// This closes the serial control ports for all monitors.
        /// </summary>
        public void DisconnectAllMonitorSerial() 
        {
            UVDLPApp.Instance().m_deviceinterface.DisconnectAllProjDrivers();
        }
    }
}
