

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using UV_DLP_3D_Printer.GUI;
using SoftwareLocker;
using UV_DLP_3D_Printer.Plugin;
using System.Threading;

namespace UV_DLP_3D_Printer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            SetDefaultCulture(System.Globalization.CultureInfo.InvariantCulture); 
            Application.SetCompatibleTextRenderingDefault(false);
            //init the app object
            UVDLPApp.Instance().DoAppStartup(); // start the app and load the plug-ins
            //iterate through the plugins
            bool showlicensedialog = false;
            bool nosplash = false;

#if (REQUIREPLUGIN)
            if (UVDLPApp.Instance().m_plugins.Count == 0) 
            {
                Application.Exit();
                return;
            }
#endif
            //iterate through all plugins,
            // if they are un-licensed and enabled - show licensing dialog.
            foreach (PluginEntry pe in UVDLPApp.Instance().m_plugins) 
            {
                if (pe.m_licensed == false && pe.m_enabled == true) // if we don't have a license, and it's marked enabled
                {
                    showlicensedialog = true;
                }
                int options = pe.m_plugin.GetInt("Options");
                if (options != -1 && pe.m_enabled == true) // check to see if this plugin has options
                {
                    if ((options & PluginOptions.OPTION_TIMED_TRIAL) != 0) // check for timed trial bit
                    {
                        // check for trial version
                        //start trial version 
                        if (!CheckTrial(pe)) 
                        {
                            //disable the plug-in
                            pe.m_enabled = false;
                            //exit?
                            Application.Exit();
                            return;
                            
                        }
                    }
                    if ((options & PluginOptions.OPTION_NOSPLASH) != 0) 
                    {
                        nosplash = true;
                    }
                }
                
            }
            if (showlicensedialog == true) 
            {
                frmPluginManager pim = new frmPluginManager();
                pim.ShowDialog();
            }
            try
            {
#if !DEBUG  // no splash screen under debug release
                if (!nosplash)
                {
                    frmSplash splash = new frmSplash(); // should pull from a licensed plug-in if need-be
                    splash.Show();
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(5);
                        Application.DoEvents();
                    }
                }
#endif
                Application.Run(new frmMain2());
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

        }
        static bool CheckTrial(PluginEntry pe) 
        {
            
            TrialMaker t = new TrialMaker("TT6", Application.StartupPath + "\\RegFile.reg",
                //Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\TMSetp.dbf",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TMSetp.dbf",
                "",
                60, 1000, "745");
            //Environment.SpecialFolder.ApplicationData
            byte[] MyOwnKey = { 97, 250, 1, 5, 84, 21, 7, 63,
            4, 54, 87, 56, 123, 10, 3, 62,
            7, 9, 20, 36, 37, 21, 101, 57};
            t.TripleDESKey = MyOwnKey;

            TrialMaker.RunTypes RT = t.ShowDialog();
            //bool is_trial;
            if (RT != TrialMaker.RunTypes.Expired)
            {
                /*
                if (RT == TrialMaker.RunTypes.Full)
                    is_trial = false;
                else
                    is_trial = true;
                 * */
                return true;
                //Application.Run(new Form1(is_trial));
            }
            return false;
            //return true;
        }
        /*Set up a methoid to use reflection to set the culture information*/
        static void SetDefaultCulture(CultureInfo culture)
        {
            Type type = typeof(CultureInfo);

            try
            {
                type.InvokeMember("s_userDefaultCulture",
                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                                    null,
                                    culture,
                                    new object[] { culture });

                type.InvokeMember("s_userDefaultUICulture",
                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                                    null,
                                    culture,
                                    new object[] { culture });
            }
            catch { }

            try
            {
                type.InvokeMember("m_userDefaultCulture",
                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                                    null,
                                    culture,
                                    new object[] { culture });

                type.InvokeMember("m_userDefaultUICulture",
                                    BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                                    null,
                                    culture,
                                    new object[] { culture });
            }
            catch { }
        }

    }
}


