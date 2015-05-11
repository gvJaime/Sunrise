using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using Ionic.Zip;
using System.IO;
using System.Windows.Forms;
namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// This class loads and saves to zip file that are the .pcw file format - Plugin Creation Workshop
    /// this file can contain the following:
    /// 1 guiconfig.xml file
    /// 0 or more machine config files
    /// 0 or moe slicing profile files
    /// 
    /// The slicing profiles will copy into the main /Profiles directory ONLY if they don't exist there already
    /// The machine profiles will copy into the main /Machine directory ONLY if they don't exist there already
    /// 
    /// 
    /// This is a 'lite' plugin desing for the following:
    /// 
    /// change/remove the splash screen
    /// change the about screen
    /// change the toolbar icon pictures
    /// remove toolbar icons
    /// remove program controls
    /// theme / color scheme
    /// create new toolbar buttons and bind them to gcode sequences
    /// 
    /// </summary>
    class PluginLite : IPlugin
    {
        private static PluginItem[] manifest =
        {
            new  PluginItem ("VendorName",ePlItemType.eString,0),
            new  PluginItem ("PluginName",ePlItemType.eString,0),
            new  PluginItem ("Version",ePlItemType.eString,0),
            new  PluginItem ("Description",ePlItemType.eString,0),
            new  PluginItem("Icon",ePlItemType.eImage,0),
            new  PluginItem("Splash",ePlItemType.eImage,0),
            new  PluginItem("About",ePlItemType.eImage,0),            
            new  PluginItem("VendorID",ePlItemType.eInt,0),
            new  PluginItem("GuiConfig",ePlItemType.eGuiConfig,0),
        };
        private IPluginHost m_host; // the host of this program is UVDLPapp
        public string m_filename; // the name of this zip file
        private static string m_Vendorname = "Envision Labs";        
        private static int m_VendorID = 0x1000; //
        private static string m_PluginName = "Envision Labs";
        private static string m_description = "Envision Labs Plugin";
        private static string version = "1.0.0.1";
        private static int m_licensed = 0; // locked to a vendor key (1 or 0)
        private GuiConfigDB m_guiconfig;
        private bool inited = false;
        private PluginResources pluginRes;
        private Dictionary<string, Bitmap> m_images;

        public Bitmap GetImage(string name)
        {
            if ((name != null) && (m_images != null) && (m_images.ContainsKey(name)))
                return m_images[name];
            return null;
        }

        /*public Bitmap GetImage(string name)
        {
            Bitmap bmp = null;
            string fname = null;
            if (pluginRes != null)
            {
                foreach (string str in pluginRes.ImageFiles)
                {
                    if (Path.GetFileNameWithoutExtension(str) == name)
                        fname = @"Images/" + str;
                }
            }
            if (fname == null)
                fname = name + ".png"; // backward compatibility needed ??
            //try to load a bitmap from the zip file
            try
            {
                using (ZipFile m_zip = ZipFile.Read(m_filename))
                {
                     ZipEntry ze = m_zip[fname];
                    Stream stream = new MemoryStream();
                    ze.Extract(stream);
                    bmp = new Bitmap(stream);
                    if (bmp != null)
                        return bmp;
                }
            }
            catch (Exception)
            {
                DebugLogger.Instance().LogError("Image resource " + name + ".png error loading from plugin " + m_filename);
            }
            return bmp;
        }*/

        public void LoadManifest()
        {
            pluginRes = new PluginResources();
            try
            {
                using (ZipFile m_zip = ZipFile.Read(m_filename))
                {
                    ZipEntry ze = m_zip["PluginManifest.xml"];
                    MemoryStream stream = new MemoryStream();
                    ze.Extract(stream);
                    pluginRes.ParseManifest(stream);
                }
            }
            catch (Exception)
            {
                DebugLogger.Instance().LogError("Error loading PluginManifest.xml from plugin " + m_filename);
            }
        }

        private void CopyConfigFile(ZipFile zipFile, string fileName)
        {
            ZipEntry ze = zipFile[fileName];
            ze.Extract(ExtractExistingFileAction.DoNotOverwrite);
        }

        public void CopyConfigFiles()
        {
            try
            {
                using (ZipFile m_zip = ZipFile.Read(m_filename))
                {
                    foreach (string name in pluginRes.MachineConfigFiles)
                        CopyConfigFile(m_zip, @"Machines/" + name);
                    foreach (string name in pluginRes.SliceConfigFiles)
                        CopyConfigFile(m_zip, @"Profiles/" + name);
                }
            }
            catch (Exception)
            {
                DebugLogger.Instance().LogError("Error copying config file");
            }
        }

        public void LoadImages()
        {
            if (pluginRes == null)
                return;
            m_images = new Dictionary<string, Bitmap>();
            try
            {
                using (ZipFile m_zip = ZipFile.Read(m_filename))
                {
                    foreach (string name in pluginRes.ImageFiles)
                    {
                        string fname = @"Images/" + name;
                        ZipEntry ze = m_zip[fname];
                        Stream stream = new MemoryStream();
                        ze.Extract(stream);
                        Bitmap bmp = new Bitmap(stream);
                        if (bmp != null)
                        {
                            string imname = Path.GetFileNameWithoutExtension(name);
                            m_images[imname] = bmp;
                            if (name.StartsWith("gl"))
                                UVDLPApp.Instance().m_2d_graphics.AddImage(imname, bmp);
                       }
                    }
                }
            }
            catch (Exception)
            {
                DebugLogger.Instance().LogError("Error copying config file");
            }

        }

        public GuiConfigDB GUIDB 
        {
            get { return m_guiconfig; }
            set { m_guiconfig = value; }
        }
        // this will set the plugin host
        public IPluginHost Host 
        {
            get { return m_host; }
            set { 
                m_host = value;
                Initialize();
            }
        } 
        // this function will return a manifest of plugin items
        public List<PluginItem> GetPluginItems 
        {
            get
            {
                List<PluginItem> items = new List<PluginItem>();
                items.AddRange(manifest);
                return items;
            }
        }

        public int GetInt(string name)
        {
            if (name.Equals("VendorID"))
                return pluginRes == null ? m_VendorID : (int)pluginRes.PluginInfo.vendorID;
            if (name.Equals("HasLicense"))
                return m_licensed;
            return -1;

        }
        public String GetString(string name)
        {
            if (name.Equals("VendorName"))
                return pluginRes == null ? m_Vendorname : (string)pluginRes.PluginInfo.vendorName;

            if (name.Equals("PluginName"))
                return pluginRes == null ? m_PluginName : (string)pluginRes.PluginInfo.productName;

            if (name.Equals("Description"))
                return pluginRes == null ? m_description : (string)pluginRes.PluginInfo.description;

            if (name.Equals("Version"))
                return pluginRes == null ? version : (string)pluginRes.PluginInfo.version;// Assembly.GetCallingAssembly().GetName().Version.ToString();

            if (name.Equals("GuiConfig")) 
            {
                // load from zip
                Stream guiconf = Utility.ReadFromZip(m_filename, "GuiConfig.xml");
                if (guiconf == null) //bail if it's not there.
                {
                    DebugLogger.Instance().LogError("No GuiConfig.xml found in plugin");
                    return "";
                }
                StreamReader streamReader = new StreamReader(guiconf);
                //get the text of the stream
                string sguiconf = streamReader.ReadToEnd();
                streamReader.Close();
                return sguiconf;
            }
            return "Unknown Name";
        }

        public UserControl GetControl(string name) 
        {
            return null; // no actual gui controls in here for now
        }
        //don't think we actually used this before.
        public byte[] GetBinary(string name) 
        {
            return null;
        }

        public void ExecuteFunction(string name) 
        {
        
        }
        public void ExecuteFunction(string name, object[] parms) 
        {
        
        }
        public bool SupportFunctionality(PluginFuctionality func) 
        {
            switch (func) 
            {
                case PluginFuctionality.CustomGUI: return true;
            }
            return false;
        }
        private void Initialize()
        {
            if (inited) // no re-initialization
                return;
            CopyConfigFiles();
            LoadImages();
            //load the guiconfigdb locally here to search / parse items
            inited = true;
        }
        public String Name 
        {
            //get { return "Pro Plugin created"; }
            get { return Path.GetFileNameWithoutExtension(m_filename); }
        } // required NOT to be part of the string plugin items        
    }
}
