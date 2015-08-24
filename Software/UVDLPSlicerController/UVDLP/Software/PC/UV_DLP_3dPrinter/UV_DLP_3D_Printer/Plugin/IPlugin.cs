using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// This is the interface that DLL's must conform to in order to be considered a plug-in for our program
    /*I'm thinking a plugin can host controls, XML resources, scripts, etc... 
     It should also have functions that it can run, maybe through a delegate 
     graphical resources
        Images, Icons, etc..
     We should be able to report everything listed in the manifest through
     some sort of enumeration function
     we should also be able to return 
 */
    public enum PluginFuctionality
    {
        CustomGUI,  // this plugin can GUI apearence, and optionally add GUI fucntions 
        HWDriver,   // this plugin can comunicate with a HW device. (Sending/Translating GCode to HW)
        Slicer,     // this plugin can convert objects to GCode,
        GCodePostProcess, // this plugin can post-process sliced GCode
        MachineProfile, // this plugin supports creating a new custom machine profile
        SliceBuildProfile, // This profile supports creating a new slicing / building profile

    }
    /// 
    /// </summary>
    public interface IPlugin
    {
        IPluginHost Host { get; set; } // this will set the plugin host
        // this function will return a manifest of plugin items
        List<PluginItem> GetPluginItems { get; }
        Bitmap GetImage(string name);
        String GetString(string name);
        UserControl GetControl(string name);
        byte[] GetBinary(string name);
        int GetInt(string name);
        void ExecuteFunction(string name);
        void ExecuteFunction(string name, object[] parms);
        bool SupportFunctionality(PluginFuctionality func);
        String Name { get; } // required NOT to be part of the string plugin items
    }
}
