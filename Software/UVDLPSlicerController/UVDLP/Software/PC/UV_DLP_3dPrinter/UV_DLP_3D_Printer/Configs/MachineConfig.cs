using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UV_DLP_3D_Printer.Configs;
using System.IO;

namespace UV_DLP_3D_Printer
{
    /*
     This class holds some basic information about the printer's machine configuration
     * such as:
     * DLP resolution
     * Build Platform Width/Height/Length(Z)
     * calculated x/y dpi (or dpmm)
     * 
     */
    public class MachineConfig
    {
        public enum eMachineType          // indicates the major machine type
        {
            FDM,
            UV_DLP
        }
        public enum eMultiMonType 
        {
            eVertical,
            eHorizontal
        }

        public const int FILE_VERSION = 1; // this should change every time the format changes
        public double m_PlatXSize; // the X size of the build platform in mm
        public double m_PlatYSize; // the Y size of the build platform in mm
        public double m_PlatZSize; // the Z size of the Z axis length in mm
        private double m_XMaxFeedrate;// in mm/min 
        private double m_YMaxFeedrate;// in mm/min 
        private double m_ZMaxFeedrate;// in mm/min 
        public eMachineType m_machinetype;
        // for systems that have more than 2 configured monitors, a description of the orientation is needed
        public eMultiMonType m_multimontype; 
        public String m_description; // a description
        public String m_name; // the profile name
        public String m_filename;// the filename of this profile. (not saved)
        public DeviceDriverConfig m_driverconfig;
        //public MonitorConfig m_monitorconfig;
        public List<MonitorConfig> m_lstMonitorconfigs; // starting to add support for multiple monitors
        public UserParameterList userParams;
        // this is held here in the mahcine profile to copy to the slicing profile
        // this rendering size can be different than the monitor size, it can be the size 
        //of multiple monitors stiched together
        public int XRenderSize;
        public int YRenderSize;
        public bool m_OverrideRenderSize;

        public string MachineControls;

        /// <summary>
        /// This allows for retrieve arbitrary variables from the machine XML configuration
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public string GetStringVar(string varname) 
        {
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(m_filename, "MachineConfig");
            XmlNode mc = xh.m_toplevel;
            string retstr = xh.GetString(mc, varname,"");
            return retstr;        
        }
        public MonitorConfig FindMonitorByName(string monid) 
        {
            try
            {
                foreach (MonitorConfig mc in m_lstMonitorconfigs) 
                {
                    if (mc.Monitorid.Equals(monid)) 
                    {
                        return mc;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return null;
        }

        /// <summary>
        /// The reason this is here is to make sure that the 
        /// monitor X/Y size matches with what is detected with the system
        /// saving the monitor x/y res and reloading it has caused confusion, causing users to 
        /// have to remove, then re-add the indicated monitor
        /// </summary>
        private void CorrectMonitorConfig() 
        {
            foreach (MonitorConfig mc in m_lstMonitorconfigs)
            {
                int xres =0, yres =0;
                if (DisplayManager.Instance().GetMonitorResoultion(mc.Monitorid, ref xres, ref yres)) 
                {
                    mc.m_XDLPRes = xres;
                    mc.m_YDLPRes = yres;
                }
            }
        }

        public bool Load(string filename)
        {
            m_filename = filename;
            m_lstMonitorconfigs.Clear(); // clear any previously loaded monitors
            m_name = Path.GetFileNameWithoutExtension(filename);
            bool retval = false;
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(m_filename, "MachineConfig");
            retval = Load(xh);
            if (!fileExist)
            {
                xh.Save(FILE_VERSION);
            }
            return retval;
        }

        public bool Load(MemoryStream ms, string name)
        {
            m_filename = null;
            m_lstMonitorconfigs.Clear(); // clear any previously loaded monitors
            m_name = name;
            bool retval = false;
            XmlHelper xh = new XmlHelper();
            xh.LoadFromStream(ms, "MachineConfig");
            retval = Load(xh);
            return retval;
        }

        bool Load(XmlHelper xh)
        {
            bool retval = false;

            XmlNode mc = xh.m_toplevel;

            m_PlatXSize = xh.GetDouble(mc, "PlatformXSize", 102.0);
            m_PlatYSize = xh.GetDouble(mc, "PlatformYSize", 77.0);
            m_PlatZSize = xh.GetDouble(mc, "PlatformZSize", 100.0);
            m_XMaxFeedrate = xh.GetDouble(mc, "MaxXFeedRate", 100.0);
            m_YMaxFeedrate = xh.GetDouble(mc, "MaxYFeedRate", 100.0);
            m_ZMaxFeedrate = xh.GetDouble(mc, "MaxZFeedRate", 100.0);
            XRenderSize = xh.GetInt(mc, "XRenderSize", 1024);
            YRenderSize = xh.GetInt(mc, "YRenderSize", 768);
            m_OverrideRenderSize = xh.GetBool(mc, "OverrideRenderSize", false);

            MachineControls = xh.GetString(mc, "DisplayedControls", "XYZPG");
            m_machinetype = (eMachineType)xh.GetEnum(mc, "MachineType", typeof(eMachineType), eMachineType.UV_DLP);
            m_multimontype = (eMultiMonType)xh.GetEnum(mc, "MultiMonType", typeof(eMultiMonType), eMultiMonType.eHorizontal);
            
            if (m_driverconfig.Load(xh, mc))
            {
                retval = true;
            }

            //m_monitorconfig.Load(xh, mc);
            List<XmlNode> monitornodes = xh.FindAllChildElement(mc, "MonitorDriverConfig");
            m_lstMonitorconfigs = new List<MonitorConfig>();
            foreach (XmlNode node in monitornodes) 
            {
                MonitorConfig monc = new MonitorConfig();
                monc.Load(xh, node);
                m_lstMonitorconfigs.Add(monc);
            }
            if (m_lstMonitorconfigs.Count > 0)
            {
                // we need at least 1 monitor
                //m_monitorconfig = m_lstMonitorconfigs[0];
            }
            else 
            {
                DebugLogger.Instance().LogError("No monitor configurations found!");
            }

            CalcPixPerMM();
            //if we have a special render size, don't wipe it out...
            if (m_OverrideRenderSize == false)
            {
                CorrectMonitorConfig();
            }
            userParams = new UserParameterList();
            xh.LoadUserParamList(userParams);
            return retval;
        }

        private void LoadSequences(XmlHelper xh) 
        {
            XmlNode mc = xh.m_toplevel;
            List<XmlNode> sequencesnodes = xh.FindAllChildElement(mc, "Sequences"); // should only be one of these
            List<XmlNode> sequencenodes = xh.FindAllChildElement(mc, "Sequence"); //can be multiple sequence definitions

            foreach (XmlNode node in sequencenodes)
            {
               // MonitorConfig monc = new MonitorConfig();
               // monc.Load(xh, node);
               // m_lstMonitorconfigs.Add(monc);
            }
        }
        public bool Save(string filename)
        {
            bool retval = false;
            m_filename = filename;
            m_name = Path.GetFileNameWithoutExtension(filename);
            XmlHelper xh = new XmlHelper();
            // bool fileExist = xh.Start(m_filename, "MachineConfig");
            //bool fileExist = false;
            xh.StartNew(m_filename, "MachineConfig");
            XmlNode mc = xh.m_toplevel;
            xh.SetParameter(mc, "PlatformXSize", m_PlatXSize);
            xh.SetParameter(mc, "PlatformYSize", m_PlatYSize);
            xh.SetParameter(mc, "PlatformZSize", m_PlatZSize);
            xh.SetParameter(mc, "MaxXFeedRate", m_XMaxFeedrate);
            xh.SetParameter(mc, "MaxYFeedRate", m_YMaxFeedrate);
            xh.SetParameter(mc, "MaxZFeedRate", m_ZMaxFeedrate);
            xh.SetParameter(mc, "XRenderSize", XRenderSize);
            xh.SetParameter(mc, "YRenderSize", YRenderSize);
            xh.SetParameter(mc, "OverrideRenderSize", m_OverrideRenderSize);
            xh.SetParameter(mc, "DisplayedControls", MachineControls);

            xh.SetParameter(mc, "MachineType", m_machinetype);
            xh.SetParameter(mc, "MultiMonType", m_multimontype);

            if (m_driverconfig.Save(xh, mc))
            {
                retval = true;
            }
            // save all the monitor configurations
            foreach (MonitorConfig monc in m_lstMonitorconfigs) 
            {
                monc.Save(xh, mc);
            }
            //m_monitorconfig.Save(xh, mc);
            xh.SaveUserParamList(userParams);
            xh.Save(FILE_VERSION);
            return retval;
        }
        public MachineConfig()
        {
            m_PlatXSize = 102.0;
            m_PlatYSize = 77.0;
            m_PlatZSize = 100; // 100 mm default, we have to load this
            m_XMaxFeedrate = 100;
            m_YMaxFeedrate = 100;
            m_ZMaxFeedrate = 100;
            XRenderSize = 1024;
            YRenderSize = 768;
            m_driverconfig = new DeviceDriverConfig();
            //m_monitorconfig = new MonitorConfig();
            m_lstMonitorconfigs = new List<MonitorConfig>(); // create a list of monitors attached to the system
            m_machinetype = eMachineType.UV_DLP;
            m_multimontype = eMultiMonType.eVertical;
            MachineControls = "";
            userParams = new UserParameterList();
            CalcPixPerMM();
        }
        // create a null loop-back machine for test
        public void CreateNullMachine() 
        {
            m_PlatXSize = 102.0;
            m_PlatYSize = 77.0;
            m_PlatZSize = 100; // 100 mm default, we have to load this
            m_XMaxFeedrate = 100;
            m_YMaxFeedrate = 100;
            m_ZMaxFeedrate = 100;
            XRenderSize = 1024;
            YRenderSize = 768;
            m_driverconfig = new DeviceDriverConfig();
            m_driverconfig.m_drivertype = Drivers.eDriverType.eNULL_DRIVER;
            //m_monitorconfig = new MonitorConfig();
            m_lstMonitorconfigs = new List<MonitorConfig>(); // for now, m_monitorconfig is assiugned to the first loaded
            m_machinetype = eMachineType.UV_DLP;
            m_driverconfig.m_connection.comname = "LoopBack";
            CalcPixPerMM();
        
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(";(****Machine Configuration ******)\r\n");
            sb.Append(";(Platform X Size         = " + m_PlatXSize + "mm )\r\n");
            sb.Append(";(Platform Y Size         = " + m_PlatYSize + "mm )\r\n");
            sb.Append(";(Platform Z Size         = " + m_PlatZSize + "mm )\r\n");

            sb.Append(";(Max X Feedrate          = " + m_XMaxFeedrate + "mm/s )\r\n");
            sb.Append(";(Max Y Feedrate          = " + m_YMaxFeedrate + "mm/s )\r\n");
            sb.Append(";(Max Z Feedrate          = " + m_ZMaxFeedrate + "mm/s )\r\n");
            sb.Append(";(Machine Type            = " + m_machinetype.ToString() + ")\r\n");
            return sb.ToString();
        }

        public void CalcPixPerMM()
        {
            //m_monitorconfig.CalcPixPerMM(m_PlatXSize, m_PlatYSize); 
        }


        public void SetPlatSize(double xsz, double ysz)
        {
            m_PlatXSize = xsz;
            m_PlatYSize = ysz;
            CalcPixPerMM();
        }

        /// <summary>
        /// Feed rates for X/Y/Z - Not currently used...
        /// </summary>
        public double XMaxFeedrate
        {
            get { return m_XMaxFeedrate; }
            set { m_XMaxFeedrate = value; }
        }
        public double YMaxFeedrate
        {
            get { return m_YMaxFeedrate; }
            set { m_YMaxFeedrate = value; }
        }
        public double ZMaxFeedrate
        {
            get { return m_ZMaxFeedrate; }
            set { m_ZMaxFeedrate = value; }
        }


    }
}
