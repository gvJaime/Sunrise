using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Plugin
{
    public class PluginOptions
    {
        public static ushort OPTION_TIMED_TRIAL = 0x01; // if no license is present, give the user a 30-day timed trial with a license dialog
        public static ushort OPTION_NOLICENSE   = 0x02; // no license key required to run this plugin
        public static ushort OPTION_NOSPLASH = 0X04; // no splash screen to be shown (suppress splash altogether)
    }
}
