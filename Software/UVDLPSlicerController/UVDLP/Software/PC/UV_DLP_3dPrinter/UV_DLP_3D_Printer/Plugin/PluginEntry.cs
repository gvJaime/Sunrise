using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// this class hold the entry for a plugin after it's been loaded
    /// it hold status about the plugin such as enable/disable status
    /// and registration status
    /// </summary>
    public class PluginEntry
    {
        public bool m_enabled;
        public bool m_licensed;
        public IPlugin m_plugin;
        public string m_filename;

        public PluginEntry(IPlugin plugin,string filename) 
        {
            m_filename = filename;
            m_plugin = plugin;
            m_enabled = false;
            m_licensed = false;
        }

    }
}
