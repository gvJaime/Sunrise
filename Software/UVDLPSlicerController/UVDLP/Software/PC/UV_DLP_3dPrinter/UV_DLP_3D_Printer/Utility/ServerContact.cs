using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using UV_DLP_3D_Printer.Licensing;
using CreationWorkshop.Licensing;
using UV_DLP_3D_Printer.Plugin;
using System.Collections.Specialized;
using System.Windows.Forms;
namespace UV_DLP_3D_Printer
{
    /// <summary>
    /// This class will attempt to phone home to the server to check for updates
    /// and relay licensing information
    /// it will be run once at application startup
    /// </summary>
    public class ServerContact
    {
        public delegate void Servercontacted(string id); // should raise an event that data was sent from the server to us..
        public event Servercontacted ServerContactEvenet;
        public class VersionInfo 
        {
            string mLink; // download link
            DateTime mTimestamp; // when was this version created?
            string mVersion; // in the form of '1.0.0.X' or similar so we can compare version numbers to this one.
            public VersionInfo() 
            {
                mLink = "";
                mTimestamp = DateTime.Now;
                mVersion = "1.0.0.1";
            }
            // This class is created from the server response
        }
        private Thread m_thread; // this will be user

        /// <summary>
        /// This function will contact the server and get the latest version number of the app
        /// CW can then use this to 
        /// </summary>
        /// <returns></returns>
        public static VersionInfo LatestVersion() 
        {
            VersionInfo vi = new VersionInfo();

            return vi;
        }

        public ServerContact() 
        {
            ContactServer();// this will kick off contacting the server
        }
        

        public void CheckForUpdate()
        {
            //post a query to a php page on my site
            // get the response
            //parse it, check version numbers
            //if we can update,
            // prompt the user
            // if the user agrees,
            //launch the upgrade helper program
            // with params
        }

        public void UpdateRegInfo() 
        {
            ContactServer();
        }

        private void ContactServer() 
        {
            m_thread = new Thread(new ThreadStart(ContactServerThread));
            m_thread.Start();
            //try an http post            
        }
        private string FindLicenseKey(int vendorID) 
        {
            foreach (LicenseKey lk in KeyRing.Instance().m_keys)
            {
                if (vendorID == lk.VendorID) 
                {
                    return lk.m_key;
                }
            }
            return "no key";
        }
        /// <summary>
        /// I think I need to change this to make multiple posts
        /// one post for each licensed plug-in
        /// </summary>
        private void ContactServerThread() 
        {
            // try the HTTP Post
            try
            {                
                WebClient client = new WebClient();
                string postform = UVDLPApp.Instance().m_appconfig.m_contactform;
                string posturl = UVDLPApp.Instance().m_appconfig.m_serveraddress;
                string postaddr = posturl + "/" + postform;
                if (UVDLPApp.Instance().m_plugins.Count > 0)
                {
                    foreach (PluginEntry pe in UVDLPApp.Instance().m_plugins)//foreach licensed plugin, generate the info...
                    {
                        try
                        {
                            NameValueCollection nvc = new NameValueCollection(); ;
                            nvc["CWVersion"] = Application.ProductVersion; // include the product version
                            nvc["Machine_ID"] = FingerPrint.Value(); //get the unique identifier of this machine
                            nvc["PluginLicenseKey"] = FindLicenseKey(pe.m_plugin.GetInt("VendorID")); // find the license key for this plugin
                            nvc["PluginName"] = pe.m_plugin.GetString("PluginName"); //
                            nvc["PluginVendorID"] = pe.m_plugin.GetInt("VendorID").ToString();
                            nvc["PluginVendorName"] = pe.m_plugin.GetString("VendorName");
                            nvc["PluginVersion"] = pe.m_plugin.GetString("Version");// version of the plugin

                            var response = client.UploadValues(postaddr, "POST", nvc);
                            string resp = Encoding.Default.GetString(response);
                            //DebugLogger.Instance().LogInfo(resp); // log the response as a test
                            ParseResponse(resp);//parse the response
                        }
                        catch (Exception) { }
                    }
                }
                else 
                {
                    //just do it once with defaults
                    try
                    {
                        NameValueCollection nvc = new NameValueCollection(); ;
                        nvc["CWVersion"] = Application.ProductVersion; // include the product version
                        nvc["Machine_ID"] = FingerPrint.Value(); //get the unique identifier of this machine
                        nvc["PluginLicenseKey"] = ""; // find the license key for this plugin
                        nvc["PluginName"] = "none";
                        nvc["PluginVendorID"] = "4096";//
                        nvc["PluginVendorName"] = "none";
                        nvc["PluginVersion"] = "1.0.0.0";// version of the plugin

                        var response = client.UploadValues(postaddr, "POST", nvc);
                        string resp = Encoding.Default.GetString(response);
                        //DebugLogger.Instance().LogInfo(resp); // log the response as a test
                        ParseResponse(resp);//parse the response
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ) 
            {
                //DebugLogger.Instance().LogError(ex);
                // may want to silently fail here
            }
        }

        private void ParseResponse(string response) 
        {
            string resp = response;
            if (ServerContactEvenet != null) 
            {
                ServerContactEvenet("reginfo");
            }
        }
    }
}
