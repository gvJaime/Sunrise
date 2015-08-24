using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Plugin
{
    /// <summary>
    /// This is the host interface for our plug-in architecture
    /// </summary>
    public interface IPluginHost
    {
        bool Register(IPlugin ipi); // we must be able to register the plug-in
    }
}
