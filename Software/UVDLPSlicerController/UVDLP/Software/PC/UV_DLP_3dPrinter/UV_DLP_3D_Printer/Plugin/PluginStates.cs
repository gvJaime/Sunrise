using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// This class manages the states of plugins
    /// this contains information about whehter or not the plugin 
    /// is loaded in the dis-abled state
    /// </summary>
    public class PluginStates
    {
        private static PluginStates m_instance = null;
        private List<PluginEntry> m_entries;
        private PluginStates() 
        {
            m_entries = new List<PluginEntry>();
        }
        public static PluginStates Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new PluginStates();
            }
            return m_instance;
        }
        public bool InList(string name) 
        {
            foreach (PluginEntry pe in m_entries)
            {
                if (pe.m_filename.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
        /*
        private PluginEntry Find(string fname) 
        {
            foreach (PluginEntry pe in UVDLPApp.Instance().m_plugins) 
            {
                if (pe.m_filename.Equals(fname)) 
                {
                    return pe; 
                }
            }
            return null;
        }
         * */
        public void Load(string fname) 
        {
            try
            {
                if (File.Exists(fname))
                {
                    //string fname = UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + "pluginconfig.cfg";
                    using (StreamReader reader = new StreamReader(fname))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            PluginEntry pe = new PluginEntry(null, line.Trim());//Find(line.Trim());
                            m_entries.Add(pe);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        public void Save() 
        {
            try
            {
                string fname = UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + "pluginconfig.cfg";
                using (StreamWriter writer = new StreamWriter(fname))
                {
                    foreach (PluginEntry pe in UVDLPApp.Instance().m_plugins) 
                    {
                        if (pe.m_enabled == false)  // only write out the disabled entries for now
                        {
                            writer.WriteLine(pe.m_filename);
                        }
                    }
                    writer.Close();
                }
                
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }        
        }
    }
}
