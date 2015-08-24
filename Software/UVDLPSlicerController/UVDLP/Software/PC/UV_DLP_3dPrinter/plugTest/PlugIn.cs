using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer.Plugin;
using System.Drawing;
using UV_DLP_3D_Printer;
using System.Resources;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace plugTest
{
    // this class must be named 'Plugin'
    // it serves as the main entry point  
    public class PlugIn : IPlugin
    {
        /// <summary>
        /// This is the manifest contents of this plugin
        /// all data, functions, controls, strings, xml files,
        /// binary resources, etc are defined in this manifest
        /// and served by the get functions below
        /// This interface also needs to have a method to call functions defined in this plugin
        /// </summary>
        private static PluginItem[] manifest =
        {
            new  PluginItem ("VendorName",ePlItemType.eString,0),
            new  PluginItem ("PluginName",ePlItemType.eString,0),
            new  PluginItem ("Version",ePlItemType.eString,0),
            new  PluginItem ("Description",ePlItemType.eString,0),
            new  PluginItem("Icon",ePlItemType.eImage,0),
            new  PluginItem("Splash",ePlItemType.eImage,0),
            new PluginItem("VendorID",ePlItemType.eInt,0),
            new PluginItem("Options",ePlItemType.eInt,0),
            new PluginItem("TestControl",ePlItemType.eControl,0),
            new PluginItem("TestXML",ePlItemType.eString,PluginItem.TAG_XML), // string resource - xml file
            new PluginItem("TestBinary",ePlItemType.eBin,PluginItem.TAG_ZIP), // binary zip file
            new PluginItem("TestFunction",ePlItemType.eFunction,PluginItem.TAG_GENERIC), // exposed function interface
        };
        private IPluginHost m_host;
        private bool inited;
        private static string m_Vendorname =    "TestVendor";
        private static int m_VendorID =         1234;
        private static string m_PluginName =    "TestPlugin";
        private static string version = "1.0.0.1";
        private static ushort m_options = PluginOptions.OPTION_NOLICENSE;

        private byte []m_hash; // simple SHA1 hash for validating against license keys
        public String Name { get { return m_PluginName; } }
        /// <summary>
        /// This function returns a manifest list of everything in the plugin
        /// </summary>
        public List<PluginItem> GetPluginItems 
        { 
            get 
            {
                List<PluginItem> items = new List<PluginItem>();
                items.AddRange(manifest);
                return items;
            } 
        }
        /// <summary>
        /// This function will retrieve a binary resource from the dll
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public byte[] GetBinary(string name)
        {
            if (name.Equals("TestBinary"))
                return Properties.Resources.binarytest;
            return null;
        }
        /// <summary>
        /// public interface funcxtion to return bitmaps based on name from this plugin
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bitmap GetImage(string name) 
        {
            try
            {
                if (name.Equals("Icon"))
                    return Properties.Resources.index;
                if (name.Equals("Splash"))
                    return Properties.Resources.doge;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return null; 
        }
        public UserControl GetControl(string name) 
        {
            if (name.Equals("TestControl")) 
            {
                return new ctlTestControl();
            }
            return null;
        }
        public int GetInt(string name) 
        {
            if (name.Equals("VendorID"))
                return m_VendorID;
            if (name.Equals("Options"))
                return m_options;

            return -1;
            
        }
        public String GetString(string name) 
        {
            if (name.Equals("VendorName"))
                return m_Vendorname;
            if (name.Equals("PluginName"))
                return m_PluginName;
            if (name.Equals("TestXML"))
                return Properties.Resources.text;
            if (name.Equals("Description"))
                return "Test Example Plugin";
            if (name.Equals("Version"))
            {
                return version;// Assembly.GetCallingAssembly().GetName().Version.ToString();
            }

            return "Unknown Name";
        }

        /// <summary>
        /// This maps string names to a function to be called
        /// </summary>
        /// <param name="name"></param>
        public void ExecuteFunction(string name) 
        {
            if (name.Equals("TestFunction"))
                MyTestFunc();        
        }
        public void ExecuteFunction(string name, object[] parms)
        {

        }

        /// <summary>
        /// This is an example of a test function that can
        /// be called from the plug in host.
        /// Anything could really be in here
        /// </summary>
        private void MyTestFunc() 
        {
            DebugLogger.Instance().LogInfo("Plugin Test - Test function executed");
        }
        public PlugIn() 
        {
            inited = false;
        }

        private void Initialize() 
        {
            if (inited) // no re-initialization
                return;
            inited = true;
            //copy the hash 
            m_hash = new byte[20];
        }

        public bool SupportFunctionality(PluginFuctionality func)
        {
            switch (func)
            {
                case PluginFuctionality.CustomGUI: return true;
            }
            return false;
        }

        public IPluginHost Host 
        {
            get { return m_host; }
            set 
            { 
                m_host = value;  // set the host
                Initialize(); // initialize any local resources
            }
        }
    }
}
