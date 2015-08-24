using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// This class is used to return items available in a plugin to the main app
    /// </summary>
    /// 
    //This is a list of the item types returned
    public enum ePlItemType 
    {
        eImage, // graphical resource
        eControl, // control listed
        eBin, // binary resource - generic, zip, xml document
        eFunction, // a function this plugin exposes
        eString, // a string resource from the plugin, could be an xml document 
        eInt,    // a simple integer resource
        eTexture, // GL texture resource
        eGuiConfig,  // an XML string containing gui configuration
    }
    public class PluginItem
    {
        public static int TAG_GENERIC = 0;
        public static int TAG_XML = 1; // xml is encoded as a string 
        public static int TAG_ZIP = 2;
        public static int TAG_STARTUP = 3; // this fuction is a startup function

        public string m_name;
        public ePlItemType m_type;
        public int m_data; // extra tag data  - this can help identify a sub-type of things like binary data
        public PluginItem(string name, ePlItemType ty, int tag) 
        {
            m_name = name;
            m_type = ty;
            m_data = tag;
        }
    }
}
