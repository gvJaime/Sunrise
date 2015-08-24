using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Configs;
using ImageArithmetic;
using System.Collections;

namespace UV_DLP_3D_Printer
{
    /*
     This is the form that is used to display the Image that is sent to the DLP
     * it is sized to match the resolution of the screen that it displays on
     * and it contains 1 image / picture control
     * It listens to the build manager events to show images on the screen.
     */
    public partial class frmDLP : Form 
    {
        private class bmi 
        {
            public bmi(Bitmap b, int l) 
            {
                bm = b;
                layer = l;
            }
            public Bitmap bm;
            public int layer;
        }
        Screen m_dlpscreen = null;
       // static int slcnt = 0; // slice / blank image counter
        private string m_screenid;
        Timer m_tmr;
        //float m_l, m_r, m_t, m_b;
        MonitorConfig m_monitorconfig;
        MonitorConfig.MRect m_rect;
        static int mfcount;
        List<bmi> m_lstbmps;

        public frmDLP()
        {
            mfcount = 0;
            InitializeComponent();
            m_screenid = "";
            m_rect = new MonitorConfig.MRect();
            //if we're not set correctly, default to full screen
            m_rect.top = 0.0f;
            m_rect.left = 0.0f;
            m_rect.bottom = 1.0f;
            m_rect.right = 1.0f;

            m_tmr = new Timer();
            m_tmr.Interval = 1000;
            m_tmr.Tick += new EventHandler(m_tmr_Tick);
            m_tmr.Start();
            UVDLPApp.Instance().m_buildmgr.PrintLayer += new delPrinterLayer(PrintLayer);
            m_lstbmps = new List<bmi>();
        }
        
        ~frmDLP()
        {
            // remember to remove the delegates
            UVDLPApp.Instance().m_buildmgr.PrintLayer -= PrintLayer;
            m_tmr.Tick -= m_tmr_Tick;
        }

        /// <summary>
        /// We're keeping track of all images that get sent to the dlp
        /// once we've moved on to the next layer, we can safely release 
        /// images that come from other layers.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="layer"></param>
        private void AddtoRemoveList(Bitmap bmp, int layer) 
        {
            if (bmp.Tag == null)  // tag not set
                return; 
            // only add layer slices
            if ((int)bmp.Tag != BuildManager.SLICE_NORMAL)
                return;

            foreach (bmi b in m_lstbmps) 
            {
                if (b.bm == bmp)// && layer == b.layer) 
                {
                    return; // already in list
                }
            }
            m_lstbmps.Add(new bmi(bmp,layer));
        }

        private void UpdateRemoveList(int curlayer) 
        {
            List<bmi> mrmlst = new List<bmi>();
            //iterate through all saved bitmaps
            //compare to see if they come from this layer or not
            foreach (bmi b in m_lstbmps)
            {
                if (curlayer != b.layer)
                {
                    mrmlst.Add(b);
                }
            }
            //dispose and remove entries for bitmap that are not the current layer.
            foreach (bmi b in mrmlst) 
            {
                b.bm.Dispose();
                b.bm = null;
                m_lstbmps.Remove(b);
            }            
        }
        private bool fullscreen() 
        {
            if (m_rect.top == 0.0f &&
                m_rect.left == 0.0f &&
                m_rect.right == 1.0f &&
                m_rect.bottom == 1.0f)
                return true;
            return false;
        }
        /// <summary>
        /// This sets the screen identifier and display portion so this form knows which screen to display into
        /// </summary>
        /// 
        public void Setup(string screenid, MonitorConfig monitorconfig) 
        {
            m_screenid = screenid;
            m_monitorconfig = monitorconfig;
            m_rect = monitorconfig.m_monitorrect;
        }
        
        //This delegate is called when the print manager is printing a new layer
        void PrintLayer(Bitmap bmp, int layer, int layertype)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { PrintLayer(bmp, layer, layertype); }));
                }
                else
                {
                    ViewLayer(layer, bmp, layertype);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }
        private void ViewLayer(int layer, Bitmap image, int layertype)
        {
            try
            {
                Bitmap bmp = null;
                if (image == null) // we're here because of the scroll bar in the gui
                {
                    DebugLogger.Instance().LogError("Null BM in DLP form");
                    return;
                }
                else // the image was specified from the build manager
                {
                    bmp = image;
                }
                // if we're a UV DLP printer, show on the frmDLP
                if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
                {
                    // only show the image on the dlp if we're previewing
                    //need to make sure we show the layer if building
                    if (UVDLPApp.Instance().m_buildmgr.IsPrinting == true || UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay == true
                        || layertype == BuildManager.SLICE_BLANK || layertype == BuildManager.SLICE_CALIBRATION)
                    {
                        ShowImage(bmp,layertype,layer);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }

        }

        void m_tmr_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Visible == false)
                    return;
                Screen oldscr = m_dlpscreen;
                m_dlpscreen = GetDLPScreen(false);
                if (m_dlpscreen != oldscr)
                    ShowDLPScreen(false);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }


        /*
         Shows the specified image on the picture control
         */
        public void ShowImage(Image i, int layertype, int layer) 
        {
            mfcount++;
            try
            {
                int a = 0;
                if (layertype == BuildManager.SLICE_NORMAL) 
                {
                    a = layertype;
                }
                else if (layertype == BuildManager.SLICE_BLANK)
                {
                    a = layertype;
                }
                else 
                {
                    a = layertype;
                }
                
                // change this to show only the visible portion of the image based on the 
                // scaling / display portion
                int xp = (int)(m_rect.left * i.Width);
                int yp = (int)(m_rect.top * i.Height);
                int xh = (int)(m_rect.right * i.Width);
                int yh = (int)(m_rect.bottom * i.Height);
                //
                Rectangle srcRect = new Rectangle(xp, yp, xh - xp, yh - yp);
                Bitmap b = null;
                Bitmap cropped = null;
                if (fullscreen())
                {
                    // no need to copy, just set a reference
                    b = (Bitmap)i;
                    cropped = b;
                }
                else
                {
                    b = (Bitmap)i;
                    cropped = (Bitmap)b.Clone(srcRect, i.PixelFormat); // i think this clone bitmap function may be causing delays later down the line                 
                }
                //check screen ID for laser SLA
                if (m_screenid.Contains("LASERSHARK"))
                {
                    // create an object array to hold parameters
                    object[] parms = new object[2];
                    //set the first parameter to be the image
                    parms[0] = (object)cropped;
                    if (layertype == BuildManager.SLICE_BLANK)
                    {
                        parms[1] = (bool)true; // parameter 2 is used to indicate that this is a blank image
                    }
                    else
                    {
                        parms[1] = (bool)false;// not blank
                    }
                    //call the plugin command
                    UVDLPApp.Instance().PerformPluginCommand("SendToLaserDisplay", parms, false);
                }
                else
                {

                    AddtoRemoveList(cropped, layer); // add the image to the remove list
                    //Check to see if we're adjusting the brightness of the mask image here
                    if (m_monitorconfig.m_usemask == true && m_monitorconfig.m_mask != null && layertype != BuildManager.SLICE_BLANK)
                    {
                        //take the cropped bitmap
                        //multiply the mask image    
                        Bitmap result = ImageArithmetic.ExtBitmap.ArithmeticBlend(cropped, m_monitorconfig.m_mask, ColorCalculator.ColorCalculationType.Multiply);
                        result.Tag = BuildManager.SLICE_NORMAL;
                        picDLP.Image = result;
                        AddtoRemoveList(result, layer);                        
                    }
                    else
                    {
                        picDLP.Image = cropped;
                    }
                    picDLP.Refresh(); // show it now
                    UpdateRemoveList(layer);// free up old resources
                }
                
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
 


        /// <summary>
        /// This function will return the Screen specified in the machine configuration file
        /// If the screen cannot be found, it will return null. 
        /// This is a change in behaviour from before
        /// </summary>
        /// <returns></returns>
        public Screen GetDLPScreen(bool reportError)
        {
            Screen dlpscreen = null;
            foreach (Screen s in Screen.AllScreens)
            {
                string mn = Utility.CleanMonitorString(s.DeviceName);
                string mid = Utility.CleanMonitorString(m_screenid);
                if (mn.Contains(mid))
                {
                    dlpscreen = s;
                    break;
                }
            }
            if ((dlpscreen == null) && reportError)
            {
                //dlpscreen = Screen.AllScreens[0]; // default to the first if we can't find it
                DebugLogger.Instance().LogRecord("Can't find screen " + m_screenid);
            }
            return dlpscreen;
        }

        public Screen GetDLPScreen()
        {
            return GetDLPScreen(true);
        }

        public bool ShowDLPScreen()
        {
            return ShowDLPScreen(true);
        }

        protected bool ShowDLPScreen(bool rescan)
        {
            try
            {
                if (rescan)
                    m_dlpscreen = GetDLPScreen();
                Show();
                if (m_dlpscreen == null)
                {
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    if (picDLP.Image != null)
                        SetDesktopBounds(0, 0, picDLP.Image.Width / 2, picDLP.Image.Height / 2);
                    else
                        SetDesktopBounds(0, 0, 512, 384);
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                    SetDesktopBounds(m_dlpscreen.Bounds.X, m_dlpscreen.Bounds.Y, m_dlpscreen.Bounds.Width, m_dlpscreen.Bounds.Height);
                    WindowState = FormWindowState.Maximized;
                    FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    if (m_dlpscreen == Screen.PrimaryScreen)
                    {
                        BringToFront();
                        Focus();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
        }

        public bool HideDLPScreen()
        {
            Hide();
            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if ((e.KeyCode == Keys.Escape) && (m_dlpscreen == Screen.PrimaryScreen) 
                && (WindowState == FormWindowState.Maximized))
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == 0x0112) // WM_SYSCOMMAND
                {
                    if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                    {
                        FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    }
                }
                base.WndProc(ref m);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
    }
}
