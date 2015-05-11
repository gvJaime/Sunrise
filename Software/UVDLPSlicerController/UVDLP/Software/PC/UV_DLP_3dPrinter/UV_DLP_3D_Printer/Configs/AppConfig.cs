using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer
{
    /*
     * The current application configuration
     */
    public class AppConfig
    {
        public const int FILE_VERSION = 1; // this should change every time the format changes
        private String m_LastModelFilename; // the last model loaded
        public string m_cursliceprofilename; // slicing / building profile
        public string m_curmachineeprofilename; // machine profile
        bool m_autoconnect; // autoconnect to the machine
        bool m_loadlastmodel; // load and display the last model
        public string m_slic3rloc; // location of slicer exetutable- shouldn't be here?
        public string m_slic3rparameters; // parameters to send to the command line invocation of slic3r
        public Color m_foregroundcolor;
        public Color m_backgroundcolor;
        
        public string SupportConfigName = "supportconfig.xml";
        public string ProjectorCommandsFile = "projectorcommands.xml";
        // some view options
        public bool m_viewslice3d; // view the 3d polyline of the slice while previewing with the scrollbar
        public bool m_viewslice3dheight; // view the height of the slice in 3d
        public bool m_previewslicesbuilddisplay; // show the 2d slice on the DLP display while previewing with the scrollbar
        public bool m_driverdebuglog;  // enable driver level debug logging of communication
        public bool m_ignore_response; // ignore the gcode responses, and go by timing
        public bool m_showBoundingBox; // selected objects will be marked with a bounding box
        public bool m_showShaded;      // selected objects will be marked with a different shade
        public bool m_showOutline;     // selected objects will be marked with an outline

        public string m_serveraddress = "http://www.buildyourownsla.com"; // for the http post
        public string m_contactform = "cwphp/cwupdate.php"; // the form to contact
        public string m_licensekey;

        public void CreateDefault() 
        {
            m_cursliceprofilename = UVDLPApp.Instance().m_PathProfiles + UVDLPApp.m_pathsep + "default.slicing";
            m_curmachineeprofilename = UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + "NullMachine.machine";
            SupportConfigName = "supportconfig.xml";
            m_LastModelFilename = "";
            m_loadlastmodel = true;
            m_autoconnect = false;
            m_previewslicesbuilddisplay = false;
            m_foregroundcolor = Color.White;
            m_backgroundcolor = Color.Black;
            m_viewslice3d = false;
            m_viewslice3dheight = false;
            m_driverdebuglog = false;
            m_ignore_response = false;
            m_showBoundingBox = true;
            m_showOutline = false;
            m_showShaded = false;
            m_licensekey = "00000000000000000000";// default to 20 0's
            // m_drivertype = eDriverType.eNULL_DRIVER;
        }

        public bool Load(String filename) 
        {
            try
            {
                XmlHelper xh = new XmlHelper();
                bool fileExist = xh.Start(filename, "ApplicationConfig");
                XmlNode ac = xh.m_toplevel;
                m_LastModelFilename = xh.GetString(ac, "LastModelName","");
                m_cursliceprofilename = UVDLPApp.Instance().m_PathProfiles + UVDLPApp.m_pathsep + xh.GetString(ac, "SliceProfileName", "default.slicing");
                m_curmachineeprofilename = UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + xh.GetString(ac, "MachineProfileName", "NullMachine.machine");
                m_autoconnect = xh.GetBool(ac, "AutoConnect", false);
                m_loadlastmodel = xh.GetBool(ac, "LoadLastModel", true);
                m_slic3rloc = xh.GetString(ac, "Slic3rLocation", "");
                m_slic3rparameters = xh.GetString(ac,"Slic3rParams","");
                m_foregroundcolor =  xh.GetColor(ac, "ForegroundColor", Color.White);
                m_backgroundcolor = xh.GetColor(ac, "BackgroundColor", Color.Black);
                m_previewslicesbuilddisplay = xh.GetBool(ac, "PreviewSlices", false);
                m_viewslice3d = xh.GetBool(ac, "Preview3dSlice", false);
                m_viewslice3dheight = xh.GetBool(ac, "Preview3dSliceHeight", false);
                m_driverdebuglog = xh.GetBool(ac, "DriverLogging", false);
                m_ignore_response = xh.GetBool(ac, "IgnoreGCRsp", false);
                m_showBoundingBox = xh.GetBool(ac, "ShowBoundingBox", true);
                m_showShaded = xh.GetBool(ac, "ShowShaded", false);
                m_showOutline = xh.GetBool(ac, "ShowOutline", false);
                m_licensekey = xh.GetString(ac, "LicenseKey", "00000000000000000000");
                m_serveraddress = xh.GetString(ac,"ServerAddress","www.buildyourownsla.com");
                m_contactform = xh.GetString(ac,"ContactForm","cwupdate.php");
                if (!fileExist)
                {
                    xh.Save(FILE_VERSION);
                }
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
        }
        public bool Save(String filename) 
        {
            try
            {
                XmlHelper xh = new XmlHelper();
                bool fileExist = xh.Start(filename, "ApplicationConfig");
                XmlNode ac = xh.m_toplevel;
                xh.SetParameter(ac, "LastModelName", m_LastModelFilename);
                xh.SetParameter(ac, "SliceProfileName", Path.GetFileName(m_cursliceprofilename));
                xh.SetParameter(ac, "MachineProfileName", Path.GetFileName(m_curmachineeprofilename));
                xh.SetParameter(ac, "AutoConnect", m_autoconnect ? "True" : "False");
                xh.SetParameter(ac, "LoadLastModel", m_loadlastmodel ? "True" : "False");
                xh.SetParameter(ac, "Slic3rLocation", m_slic3rloc);
                xh.SetParameter(ac, "Slic3rParams", m_slic3rparameters);
                xh.SetParameter(ac, "ForegroundColor", m_foregroundcolor);
                xh.SetParameter(ac, "BackgroundColor", m_backgroundcolor);
                xh.SetParameter(ac, "PreviewSlices", m_previewslicesbuilddisplay);
                xh.SetParameter(ac, "Preview3dSlice", m_viewslice3d);
                xh.SetParameter(ac, "Preview3dSliceHeight", m_viewslice3dheight);
                xh.SetParameter(ac, "DriverLogging", m_driverdebuglog);
                xh.SetParameter(ac, "IgnoreGCRsp", m_ignore_response);
                xh.SetParameter(ac, "ShowBoundingBox", m_showBoundingBox);
                xh.SetParameter(ac, "ShowOutline", m_showOutline);
                xh.SetParameter(ac, "ShowShaded", m_showShaded);
                xh.SetParameter(ac, "LicenseKey", m_licensekey);
                xh.SetParameter(ac, "ServerAddress", m_serveraddress);
                xh.SetParameter(ac, "ContactForm", m_contactform);
                xh.Save(FILE_VERSION);

                return true;
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false; 
            }
            
        }
        public String LastModelFilename 
        {
            get { return m_LastModelFilename; }
            set { m_LastModelFilename = value; }
        }

        public AppConfig() 
        {
            //CreateDefault();
        }
    }
}
