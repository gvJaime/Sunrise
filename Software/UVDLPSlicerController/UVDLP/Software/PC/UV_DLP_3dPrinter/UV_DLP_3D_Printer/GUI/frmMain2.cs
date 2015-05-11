using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using System.IO;
using UV_DLP_3D_Printer.Slicing;
using UV_DLP_3D_Printer._3DEngine;
using UV_DLP_3D_Printer.Device_Interface;
using UV_DLP_3D_Printer.Device_Interface.AutoDetect;


namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmMain2 : Form
    {
        // this section is to hook into the global mouse events for the application
        // we're going to do hit testing when the right mouse button is clicked. for 
        //'Pro'feature skinning
        public delegate void MouseMovedEvent();
        public delegate void MouseRDownEvent();
        public class GlobalMouseHandler : IMessageFilter
        {
            private const int WM_MOUSEMOVE = 0x0200;
            private const int WM_RBUTTONDOWN = 0x0204;
            public event MouseMovedEvent TheMouseMoved;
            public event MouseRDownEvent TheMouseRDown;
            #region IMessageFilter Members
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEMOVE)
                {
                    if (TheMouseMoved != null)
                    {
                        TheMouseMoved();
                    }
                }
                else if (m.Msg == WM_RBUTTONDOWN) 
                {
                    if (TheMouseRDown != null) 
                    {
                        TheMouseRDown();
                    }
                }
                // Always allow message to continue to the next filter control
                return false;
            }
            #endregion
        }        
        // the tabview class is used to display a ctlTitle with a panel object on the pnlTopTabs
        // I'm adding this small tracker class so we can add new tabs / controls loaded
        // from plugins or from the guiconfig files.
        private class tabview 
        {
            static int idxgen = 0;
            public int m_tabidx;
            public ctlImageButton m_title;
            public Control m_panel;
            public string m_name;
            public tabview(string name, ctlImageButton title, Control pcontrol) 
            {
                m_name = name;
                m_title = title;
                m_panel = pcontrol;
                m_tabidx = idxgen++;
            }
        }
        /*
        private enum eViewTypes
        {
            eV3d,
            eVSlice,
            eVControl,
            eVConfig,
            eNone
        }
         */ 
        private int m_viewtype;
        public event delBuildStatus BuildStatusInvoked; // rund the build delegate in Form thread
        public string m_appname = "Creation Workshop";
        frmSlice m_frmSlice = new frmSlice();
        public ManualControl m_manctl;
        int rightToolsWidth = 0;
        StringBuilder m_logSB;
        List<tabview> m_lsttabs;
        private string SceneFileExt;
        public GlobalMouseHandler app_mouse_handler = new GlobalMouseHandler();
        
        public frmMain2()
        {
            SceneFileExt = "cws";// default to creation workshop CWS files
            m_lsttabs = new List<tabview>();
            m_logSB = new StringBuilder();
            InitializeComponent();
            
            UVDLPApp.Instance().m_mainform = this;
            m_manctl = ManualControl.Instance(); // late intialization happens here after the UVDLP app Singleton is initiated.          

            RegisterCallbacks();
            RegisterGUI();
            SetButtonStatuses();
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
            DebugLogger.Instance().LoggerStatusEvent += new LoggerStatusHandler(LoggerStatusEvent);
            UVDLPApp.Instance().m_slicer.Slice_Event += new Slicer.SliceEvent(SliceEv);
            UVDLPApp.Instance().m_buildmgr.BuildStatus += new delBuildStatus(BuildStatus);
            UVDLPApp.Instance().m_deviceinterface.StatusEvent += new DeviceInterface.DeviceInterfaceStatus(DeviceStatusEvent);
            UVDLPApp.Instance().m_buildmgr.PrintLayer += new delPrinterLayer(PrintLayer);
            UVDLPApp.Instance().Engine3D.UpdateGrid();
            ctl3DView1.UpdateView(); // initial update
            // set up initial log data in form
            foreach (string lg in DebugLogger.Instance().GetLog())
            {
                //txtLog.Text = lg + "\r\n" + txtLog.Text;
                AddtoLog(lg);
            }
            //RearrangeGui
            AddButtons();
            AddControls();
            SetupTabs();
            //set the default tab
            m_viewtype = -1;// start with the first tab added
            ShowView(0);// 3d tab

            
            ctl3DView1.RearrangeGui();
            ctl3DView1.Enable3dView(true);
            // test new gui config system
            GuiConfigDB gconfdb = new GuiConfigDB();
            //try to load the GUI
            gconfdb.LoadConfiguration(global::UV_DLP_3D_Printer.Properties.Resources.GuiConfig);
            UVDLPApp.Instance().m_gui_config.ApplyConfiguration(gconfdb);

            //ctlSliceGCodePanel1.ctlSliceViewctl.DlpForm = m_frmdlp; // set the dlp form for direct control
            SetMainMessage("");
            SetTimeMessage("");
            #if (DEBUG)
                ShowLogPanel(true);
            #else
                ShowLogPanel(false);
                pluginTesterToolStripMenuItem.Visible = false;
                testToolStripMenuItem.Visible = false;
                testMachineControlToolStripMenuItem.Visible = false;
                loadGUIConfigToolStripMenuItem.Visible = false;
                checkForUpdatesToolStripMenuItem.Visible = false;
                //miiManualControlToolStripMenuItem.Visible = false;
                //miiManualControlToolStripMenuItem.Visible = false;
            #endif
                SetTitle();
            UVDLPApp.Instance().PerformPluginCommand("MainFormLoadedCommand", true);
            AddGlobalMouseHandler();
        }
        private void AddGlobalMouseHandler() 
        {
            
            //app_mouse_handler = new GlobalMouseHandler();
            app_mouse_handler.TheMouseMoved += new MouseMovedEvent(gmh_TheMouseMoved);
           // app_mouse_handler.TheMouseRDown += new MouseRDownEvent(gmh_TheMouseRDown);
            Application.AddMessageFilter(app_mouse_handler);
        
        }
        /*
        void gmh_TheMouseRDown()
        {
            Point cur_pos = System.Windows.Forms.Cursor.Position;
            //recursive hit test controls here 
            DebugLogger.Instance().LogInfo("pos x,y = " + cur_pos.X + "," + cur_pos.Y);
            // change the control coordiantes to screen coordinates
            Point cur_pos_screen = this.PointToScreen(cur_pos);
            // pass this main window, along with the click position in screen coordinates
            List<Control> lstctl = HTLoc(this,cur_pos);
            DebugLogger.Instance().LogInfo("count :" + lstctl.Count);
        }

        List<Control> HTLoc(Control tctrl, Point loc_screen) 
        {
            List<Control> lst = new List<Control>();
            // first, convert the screen coords into this controls local coords
            Point loc_control = tctrl.PointToClient(loc_screen);
            //test to see if these screen coordniates fall within the bounding box of tctrl bounds
            if (tctrl.ClientRectangle.Contains(loc_control)) // bounds
            {
                // if it does, add tctrl
                lst.Add(tctrl);
                DebugLogger.Instance().LogInfo(tctrl.Name);
                // and now, iterate through all child controls
                foreach (Control c in tctrl.Controls)
                {
                    lst.AddRange(HTLoc(c, loc_screen));
                }
            }
            return lst;
        }
        */

        void gmh_TheMouseMoved()
        {
            Point cur_pos = System.Windows.Forms.Cursor.Position;
           
        }
        public void SetAboutVisible(bool val) 
        {
            aboutToolStripMenuItem.Visible = val;
        }

        public void SetFileExt(string ext) 
        {
            SceneFileExt = ext;
        }
        /// <summary>
        /// This adds buttons to the GUI config for later skinning
        /// </summary>
        private void AddButtons() 
        {
            UVDLPApp.Instance().m_gui_config.AddButton("openfile", buttOpenFile);
            UVDLPApp.Instance().m_gui_config.AddButton("play", buttPlay);
            UVDLPApp.Instance().m_gui_config.AddButton("pause", buttPause);
            UVDLPApp.Instance().m_gui_config.AddButton("save", buttSaveScene);

            UVDLPApp.Instance().m_gui_config.AddButton("stop", buttStop);
            UVDLPApp.Instance().m_gui_config.AddButton("connect", buttConnect);
            UVDLPApp.Instance().m_gui_config.AddButton("disconnect", buttDisconnect);
            UVDLPApp.Instance().m_gui_config.AddButton("buttExpandLeft", buttExpandLeft);
            
        }
        public void AddTabView(string name, ctlImageButton title, Control view) 
        {
            m_lsttabs.Add(new tabview(name, title, view));
            UVDLPApp.Instance().m_gui_config.AddButton(name + ".title", title);
        }
        private void SetupTabs() 
        {
            AddTabView("3dView",ctlTitle3dView, pnl3dview);
            AddTabView("SliceView",ctlTitleViewSlice, pnlSliceView);
            AddTabView("ManualControlView",ctlTitleViewControls, ctlMainManual1);
            AddTabView("ConfigureView",ctlTitleConfigure, ctlMainConfig1);
        }
        private void AddControls() 
        {
            // the main title buttons

            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitle3dView", ctlTitle3dView); //ctlTitle3dView
            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitleViewSlice", ctlTitleViewSlice);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitleViewControls", ctlTitleViewControls);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitleConfigure", ctlTitleConfigure);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlSliceGCodePanel1", ctlSliceGCodePanel1);

            UVDLPApp.Instance().m_gui_config.AddControl("ctlMainConfig1", ctlMainConfig1);
            UVDLPApp.Instance().m_gui_config.AddControl(pnlSliceView);
            UVDLPApp.Instance().m_gui_config.AddControl(ctl3DView1);
            UVDLPApp.Instance().m_gui_config.AddControl(pnl3dview);
            UVDLPApp.Instance().m_gui_config.AddControl(txtLog);

            //left side controls
            UVDLPApp.Instance().m_gui_config.AddControl("ctlSupports1", ctlSupports1);
            //right side controls
            UVDLPApp.Instance().m_gui_config.AddControl("ctlScene1", ctlScene1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlMoveExpand1", ctlMoveExpand1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlScale1", ctlScale1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlRotate1", ctlRotate1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlMirror1", ctlMirror1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlView1", ctlView1);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlObjectInfo1", ctlObjectInfo1);

            // main form panel
            UVDLPApp.Instance().m_gui_config.AddControl("pnlMain", splitContainer1.Panel1);
            // menu bar
            UVDLPApp.Instance().m_gui_config.AddControl("MainMenu", menuStrip1);
            
            // panels on the main form
            UVDLPApp.Instance().m_gui_config.AddControl("pnlTopIcons", pnlTopIcons);
            UVDLPApp.Instance().m_gui_config.AddControl("pnlTopTabs", pnlTopTabs);
            UVDLPApp.Instance().m_gui_config.AddControl("pnlTopIconsMain", pnlTopIconsMain);

            UVDLPApp.Instance().m_gui_config.AddControl("pnlRightBar", flowLayoutPanel1);
            UVDLPApp.Instance().m_gui_config.AddControl("pnlLeftBar", flowLayoutPanel2);

            //
            UVDLPApp.Instance().m_gui_config.AddControl("mainmsg", lblMainMessage);
            UVDLPApp.Instance().m_gui_config.AddControl("timemsg", lblTime);

            UVDLPApp.Instance().m_gui_config.AddControl("ctlMainManual", ctlMainManual1);
            //UVDLPApp.Instance().m_gui_config.AddControl("preferencesToolStripMenuItem", preferencesToolStripMenuItem);

            

        }

        private void SetTimeMessage(String message)
        {
            lblTime.Text = message;
        }
        private void SetMainMessage(String message)
        {
            try
            {
                lblMainMessage.Text = message;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }

        #region DLP Screen Controls


        private void showCalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //make sure all the dlp screens are showing
            DisplayManager.Instance().ShowDLPScreens();
            UVDLPApp.Instance().m_buildparms.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we get the right screen size 
            UVDLPApp.Instance().m_buildmgr.ShowCalibration(UVDLPApp.Instance().m_buildparms.xres, UVDLPApp.Instance().m_buildparms.yres, UVDLPApp.Instance().m_buildparms);
        }

        #endregion DLP screen controls

        #region Delegate Event Handlers
        private void SliceEv(Slicer.eSliceEvent ev, int layer, int totallayers, SliceFile sf)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { SliceEv(ev, layer, totallayers, sf); }));
                }
                else
                {
                    switch (ev)
                    {
                        case Slicer.eSliceEvent.eSliceStarted:
                            SetMainMessage("Slicing Started");
                            break;
                        case Slicer.eSliceEvent.eLayerSliced:
                            break;
                        case Slicer.eSliceEvent.eSliceCompleted:
                            //ctl3DView1.SetNumLayers(totallayers);
                            SetMainMessage("Slicing Completed");
                            String timeest = BuildManager.EstimateBuildTime(UVDLPApp.Instance().m_gcode);
                            SetTimeMessage("Estimated Build Time: " + timeest);
                            //show the slice in the slice view
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
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
                    //ctl3DView1.ViewLayer(layer); // set the 3d layer
                    // display info only if it's a normal layer
                    if (layertype == BuildManager.SLICE_NORMAL)
                    {

                        String txt = "Printing layer " + (layer + 1) + " of " + UVDLPApp.Instance().m_slicefile.NumSlices;
                        DebugLogger.Instance().LogRecord(txt);
                    }

                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }


        /*
         This function is called when the device status changes
         * most of this is for display purposes only,
         * the real business logic should be held in the app class
         */
        void DeviceStatusEvent(ePIStatus status, String Command)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { DeviceStatusEvent(status, Command); }));
            }
            else
            {
                switch (status)
                {
                    case ePIStatus.eConnected:
                        SetButtonStatuses();
                        DebugLogger.Instance().LogRecord("Device Connected");
                        break;
                    case ePIStatus.eDisconnected:
                        SetButtonStatuses();
                        DebugLogger.Instance().LogRecord("Device Disconnected");
                        break;
                    case ePIStatus.eError:
                        break;
                    //case ePIStatus.eReady:
                    //   break;
                }
            }
        }

        void BuildStatus(eBuildStatus printstat, string mess, int data)
        {
            // displays the print status
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { BuildStatus(printstat, mess, data); }));
            }
            else
            {
                if (BuildStatusInvoked != null)
                    BuildStatusInvoked(printstat, mess, data);
                String message = "";
                switch (printstat)
                {
                    case eBuildStatus.eBuildPaused:
                        message = "Print Paused";
                        SetButtonStatuses();
                        SetMainMessage(message);
                        DebugLogger.Instance().LogRecord(message);

                        break;
                    case eBuildStatus.eBuildResumed:
                        message = "Print Resumed";
                        SetButtonStatuses();
                        SetMainMessage(message);
                        DebugLogger.Instance().LogRecord(message);

                        break;
                    case eBuildStatus.eBuildCancelled:
                        message = "Print Cancelled";
                        SetButtonStatuses();
                        SetMainMessage(message);
                        DebugLogger.Instance().LogRecord(message);

                        break;
                    case eBuildStatus.eLayerCompleted:
                        message = "Layer Completed";
                        break;
                    case eBuildStatus.eBuildCompleted:
                        try
                        {
                            message = "Print Completed";
                            SetButtonStatuses();
                            //MessageBox.Show("Build Completed");
                            SetMainMessage(message);
                            DebugLogger.Instance().LogRecord(message);
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.Instance().LogError(ex.Message);
                        }
                        break;
                    case eBuildStatus.eBuildStarted:
                        message = "Print Started";
                        SetButtonStatuses();
                        // if the current machine type is a UVDLP printer, make sure we can show the screen
                        if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
                        {
                            if (!DisplayManager.Instance().ShowDLPScreens())
                            {
                                MessageBox.Show("Monitor not found, cancelling build", "Error");
                                UVDLPApp.Instance().m_buildmgr.CancelPrint();
                            }
                        }
                        SetMainMessage(message);
                        DebugLogger.Instance().LogRecord(message);
                        break;
                    case eBuildStatus.eBuildStatusUpdate:
                        // a message from the build manager has arrived
                        this.SetTimeMessage(mess);
                        break;
                }
            }
        }
        void LoggerStatusEvent(Logger o, eLogStatus status, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { LoggerStatusEvent(o, status, message); }));
            }
            else
            {
                switch (status)
                {
                    case eLogStatus.eLogWroteRecord:
                        AddtoLog(message);
                        break;
                }
            }
        }
        private void AddtoLog(string message) 
        {
            m_logSB.Append(message);
            m_logSB.Append("\r\n");
            txtLog.Text = m_logSB.ToString();//message + "\r\n" + txtLog.Text;         
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            txtLog.Refresh();
        }
        /*
          This handles specific events triggered by the app
          */
        private void AppEventDel(eAppEvent ev, String Message)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { AppEventDel(ev, Message); }));
                }
                else
                {
                    switch (ev)
                    {
                        case eAppEvent.eModelNotLoaded:
                            DebugLogger.Instance().LogRecord(Message);
                            break;

                        case eAppEvent.eModelRemoved:
                            //the current model was removed
                            DebugLogger.Instance().LogRecord(Message);
                            //UpdateSceneInfo();
                            UVDLPApp.Instance().m_engine3d.UpdateLists();
                            ctl3DView1.UpdateView();
                            break;
                        case eAppEvent.eModelAdded:
                            //UpdateSceneInfo();
                            UVDLPApp.Instance().m_engine3d.UpdateLists();
                            //DisplayFunc();
                            ctl3DView1.UpdateView();
                            DebugLogger.Instance().LogRecord(Message);
                            break;
                        case eAppEvent.eUpdateSelectedObject:
                            UpdateSceneInfo();
                            //ctl3DView1.UpdateView();
                            break;
                        case eAppEvent.eShowLogWindow:
                            bool vis = bool.Parse(Message);
                            ShowLogPanel(vis);
                            break;
                        case eAppEvent.eReDraw: // redraw the 3d display
                            //DisplayFunc();
                            ctl3DView1.UpdateView();
                            break;
                        case eAppEvent.eReDraw2D: // redraw the 2d layer of the 3d display
                            ctl3DView1.UpdateView(false);
                            break;
                        case eAppEvent.eShowBlank:
                            //showBlankDLP();
                            DisplayManager.Instance().showBlankDLPs();
                            break;
                        case eAppEvent.eShowCalib:
                            showCalibrationToolStripMenuItem_Click(null, null);
                            break;
                        case eAppEvent.eShowDLP:
                            DisplayManager.Instance().ShowDLPScreens();
                            break;
                        case eAppEvent.eHideDLP:
                            DisplayManager.Instance().HideDLPScreens();
                            break;
                        case eAppEvent.eMachineConnected:
                            DisplayManager.Instance().showBlankDLPs();
                            break;
                        case eAppEvent.eMachineDisconnected:
                            break;
                        case eAppEvent.eSceneFileNameChanged:
                            SetTitle();
                            break;
                        
                  case eAppEvent.eSlicedLoaded: // update the gui to view
                            try // this is also called when the slice PROFILE is loaded
                            {
                                DebugLogger.Instance().LogRecord(Message);
                                if (UVDLPApp.Instance().m_slicefile != null)
                                {
                                    int totallayers = UVDLPApp.Instance().m_slicefile.NumSlices;
                                    ctl3DView1.SetNumLayers(totallayers);
                                }
                            }
                            catch (Exception ex) { }
                      break;
                  case eAppEvent.eSliceProfileChanged:
                      SetTitle();
                      break;
                  case eAppEvent.eMachineTypeChanged:
                      // FIXFIX : activate SetupForMachineType on 3dview control
                      SetupForMachineType();
                      SetTitle();
                      break;
                        /*
              case eAppEvent.eGCodeLoaded:
                  DebugLogger.Instance().LogRecord(Message);
                  m_frmGCode.GcodeView.Text = UVDLPApp.Instance().m_gcode.RawGCode;
                  break;
              case eAppEvent.eGCodeSaved:
                  DebugLogger.Instance().LogRecord(Message);
                  break;



                   * */
                    }
                    //Refresh();
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

        }
        #endregion

        private void SetupForMachineType()
        {
            MachineConfig mc = UVDLPApp.Instance().m_printerinfo;            
            ctl3DView1.m_camera.UpdateBuildVolume((float)mc.m_PlatXSize, (float)mc.m_PlatYSize, (float)mc.m_PlatZSize);
        }

        public void SetTitle()
        {
            
            this.Text = m_appname + " - " + "  ( Slice Profile : ";
            this.Text += Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_buildparms.m_filename);
            this.Text += ", Machine : " + Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_printerinfo.m_filename);// +")";
            this.Text += ", Scene : " + Path.GetFileNameWithoutExtension(UVDLPApp.Instance().SceneFileName) + ")";
             
        }
        private void UpdateSceneInfo()
        {
            try
            {
                //ctl3DView1.UpdateObjectInfo();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");
                ctl3DView1.UpdateView();
            }
            catch (Exception) { }

        }
        private void RegisterGUI() 
        {
            UVDLPApp.Instance().m_gui_config.AddButton("play", buttPlay);
            UVDLPApp.Instance().m_gui_config.AddButton("pause", buttPause);
            UVDLPApp.Instance().m_gui_config.AddButton("stop", buttStop);
            UVDLPApp.Instance().m_gui_config.AddButton("connect", buttConnect);
            UVDLPApp.Instance().m_gui_config.AddButton("disconnect", buttDisconnect);
            UVDLPApp.Instance().m_gui_config.AddButton("openfile", buttOpenFile);
            UVDLPApp.Instance().m_gui_config.AddButton("slice", buttSlice);
//            UVDLPApp.Instance().m_gui_config.AddButton("stop", buttStop);

        }
        private void RegisterCallbacks() 
        {
            // the main tab buttons
           // This one callback handler replaces the 4 previous
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickSwitchTabView", ClickSwitchTabView_Click, null, "Switch Tab Display");

            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickView3d", ClickView3d, null, "Switch Tab Display");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickViewSlice", ClickViewSlice, null, "Switch Tab Display");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickViewConfig", ClickViewConfig, null, "Switch Tab Display");


            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickExpandLeft", ClickExpandLeft_Click, null, "Expand / retract left panel");
            //load model click
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("LoadSTLModel_Click", LoadSTLModel_Click, null, "Load Model");

            // Connecting / disconnecting printer
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ConnectPrinter", cmdConnect1_Click, null, "Connect to the printer");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("DisconnectPrinter", cmdDisconnect_Click, null, "Disconnect from the printer");

            //start/stop/pause printing
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("StartPrint", cmdStartPrint_Click, null, "Begin printing scene");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("PausePrint", cmdPause_Click_1, null, "Pause printing");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("StopPrint", cmdStop_Click, null, "Stop printing process");

            //Slicing

            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("StartSlice", cmdSlice1_Click, null, "Start Slicing");

            //ctlMainManual1
        }

        private void HideAllViews() 
        {
            
            foreach (tabview tv in m_lsttabs) 
            {
                tv.m_panel.Visible = false;
                tv.m_panel.Dock = DockStyle.None;
                tv.m_title.Checked = false;
            }
             
        }


        private void ShowView(int vt) 
        {
            try
            {

                if (vt == m_viewtype) 
                    return; // already there
                HideAllViews();
                foreach (tabview tv in m_lsttabs) 
                {
                    if (vt == tv.m_tabidx) 
                    {
                        tv.m_panel.Visible = true;
                        tv.m_panel.Dock = DockStyle.Fill;
                        m_viewtype = tv.m_tabidx;
                        tv.m_title.Checked = true;
                        break;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        #region button statuses
        private void SetButtonStatuses()
        {
            try
            {
                if (UVDLPApp.Instance().m_deviceinterface.Connected)
                {
                    buttConnect.Enabled = false;
                    buttDisconnect.Enabled = true;

                    if (UVDLPApp.Instance().m_buildmgr.IsPrinting)
                    {
                        if (UVDLPApp.Instance().m_buildmgr.IsPaused())
                        {
                            buttPlay.Enabled = true;
                            buttStop.Enabled = true;
                            buttPause.Enabled = false;
                        }
                        else
                        {
                            buttPlay.Enabled = false;
                            buttStop.Enabled = true;
                            buttPause.Enabled = true;
                        }
                    }
                    else
                    {
                        buttPlay.Enabled = true;
                        buttStop.Enabled = false;
                        buttPause.Enabled = false;
                    }
                }
                else
                {
                    buttConnect.Enabled = true;
                    buttDisconnect.Enabled = false;
                    buttPlay.Enabled = false;
                    buttStop.Enabled = false;
                    buttPause.Enabled = false;
                }
               // ctl3DView1.SetButtonStatus(buttConnect.Enabled, buttDisconnect.Enabled, buttPlay.Enabled, buttStop.Enabled, buttPause.Enabled);
                Refresh();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }
        #endregion


        #region Machine Connect / Disconnect
        public void cmdConnect1_Click(object sender, object e)
        {
            try
            {
                if (!UVDLPApp.Instance().m_deviceinterface.Connected) // 
                {
                    // configure the main device interface
                    UVDLPApp.Instance().m_deviceinterface.Configure(UVDLPApp.Instance().m_printerinfo.m_driverconfig.m_connection);
                    //get the name of the main serial interface
                    String com = UVDLPApp.Instance().m_printerinfo.m_driverconfig.m_connection.comname;
                    if (com.ToUpper().Equals("AUTODETECT")) 
                    {
                        com = SerialAutodetect.Instance().DeterminePort(UVDLPApp.Instance().m_printerinfo.m_driverconfig.m_connection.speed);
                        if (!com.Equals("invalid"))
                        {
                            UVDLPApp.Instance().m_printerinfo.m_driverconfig.m_connection.comname = com;
                        }
                        else 
                        {
                            DebugLogger.Instance().LogError("Serial port not auto-detected");
                            return;
                        }
                    }
                    DebugLogger.Instance().LogRecord("Connecting to Printer on " + com + " using " + UVDLPApp.Instance().m_printerinfo.m_driverconfig.m_drivertype.ToString());
                    if (!UVDLPApp.Instance().m_deviceinterface.Connect())
                    {
                        DebugLogger.Instance().LogRecord("Cannot connect printer driver on " + com);
                        //don't try to connect the monitor serial port unless the main machine connection is made first
                        // this prevents the problem of having a connected serial port and no way to disconnect it
                    }
                    else
                    {                        
                        UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eMachineConnected, "Printer connected");
                        // check to see if we're uv dlp
                        // configure the projector
                        if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
                        {
                            DisplayManager.Instance().ConnectMonitorSerials();
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }

        public void cmdDisconnect_Click(object sender, object e)
        {
            if (UVDLPApp.Instance().m_deviceinterface.Connected) // disconnect
            {
                DebugLogger.Instance().LogRecord("Disconnecting from Printer");
                UVDLPApp.Instance().m_deviceinterface.Disconnect();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eMachineDisconnected, "Printer connection closed");
            }
            //terminate connections to projector Drivers
            DisplayManager.Instance().DisconnectAllMonitorSerial();
        }
        #endregion

        #region click handlers

        
        public void cmdSlice1_Click(object sender, object e)
        {
            if (m_frmSlice.IsDisposed)
            {
                m_frmSlice = new frmSlice();
            }
            m_frmSlice.Show();
            m_frmSlice.BringToFront();
        } 
        private void hardwareGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String hardwareGuide = UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + "HardwareGuide.pdf";
                System.Diagnostics.Process.Start(hardwareGuide);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void plugInsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPluginManager pim = new frmPluginManager();
            pim.ShowDialog();

        }
        private void userManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                String usermanualpath = UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + "UserManual.pdf";
                System.Diagnostics.Process.Start(usermanualpath);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
        }
        /*
        private void saveSceneSTLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog1.FileName = "";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //save the scene object
                    UVDLPApp.Instance().CalcScene();
                    UVDLPApp.Instance().Scene.SaveSTL_Binary(saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
        */
        public void ShowLogPanel(bool visible)
        {
            splitContainer1.Panel2Collapsed = !visible;
        }


        private void pluginTesterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPluginTester pit = new frmPluginTester();
            pit.Show();

        }
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_callbackhandler.DumpCommands("CWCommands.txt");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPrefs prefs = new frmPrefs();
            prefs.ShowDialog();
        }
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_exporter.ShowDialog();
        }
        public void cmdStop_Click(object sender, object e)
        {
            UVDLPApp.Instance().m_buildmgr.CancelPrint();
        }

        public void cmdPause_Click_1(object sender, object e)
        {
            UVDLPApp.Instance().m_buildmgr.PausePrint();
        }

        public void cmdStartPrint_Click(object sender, object e)
        {
            if (UVDLPApp.Instance().m_buildmgr.IsPaused())
            {
                UVDLPApp.Instance().m_buildmgr.ResumePrint();
            }
            else
            {
                //check the machine type here
                if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
                {
                    //check to see if there is a slice file
                    if (UVDLPApp.Instance().m_slicefile == null)
                    {
                        MessageBox.Show("No Slicing file, cannot begin build");
                        return;
                    }
                    // check for gcode
                    if (UVDLPApp.Instance().m_gcode == null)
                    {
                        MessageBox.Show("No GCode file, cannot begin build");
                        return;
                    }
                    /* remove non-uvdlp gcode check for now - dean piper special..
                    // not a UV DLP GCode file
                    if (UVDLPApp.Instance().m_gcode.IsUVDLPGCode() == false)
                    {
                        MessageBox.Show("Not a UV DLP GCode file\r\nCannot begin build\r\nPossibly wrong slicer used");
                        return;
                    }
                     */ 
                    UVDLPApp.Instance().m_buildmgr.StartPrint(UVDLPApp.Instance().m_slicefile, UVDLPApp.Instance().m_gcode);

                }
                else  // assume FDM or similar
                {
                    if (UVDLPApp.Instance().m_gcode == null)
                    {
                        MessageBox.Show("No GCode file, cannot begin build");
                        return;
                    }
                    //  a UV DLP GCode file is being used for some reason
                    if (UVDLPApp.Instance().m_gcode.IsUVDLPGCode() == true)
                    {
                        MessageBox.Show("UV DLP GCode file commands detected\r\nCannot begin build\r\nPossibly wrong slicer used");
                        return;
                    }
                    UVDLPApp.Instance().m_buildmgr.StartPrint(null, UVDLPApp.Instance().m_gcode);
                }

            }
        }
        public void LoadSTLModel_Click(object sender, object e)
        {
            openFileDialog1.FileName = "";
            //openFileDialog1.Filter = "3D Model Files (*.stl;*.obj;*.3ds;*.amf)|*.stl;*.obj;*.3ds;*.amf|Scene files (*.cws)|*.cws";
            openFileDialog1.Filter = "3D Model Files (*.stl;*.obj;*.3ds;*.amf)|*.stl;*.obj;*.3ds;*.amf|Scene files (*." + SceneFileExt + ")|*." + SceneFileExt;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in openFileDialog1.FileNames)
                {
                    if (filename.Contains("." + SceneFileExt ))
                    {
                        //scene file
                        if (SceneFile.Instance().LoadSceneFile(filename))
                        {
                            //set up for newly loaded scene
                            //load gcode
                            UVDLPApp.Instance().PostLoadScene();
                            //raise events
                            //load slicing info?
                            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
                        }
                        else
                        {
                            DebugLogger.Instance().LogError("Error loading scene file : " + filename);
                        }
                    }
                    else
                    {
                        if (UVDLPApp.Instance().LoadModel(filename) == false)
                        {
                            DebugLogger.Instance().LogError("Error loading object file : " + filename);
                        }
                        else
                        {
                            //chkWireframe.Checked = false;
                            //ctl3DView1.UpdateObjectInfo();
                        }
                    }
                }
            }
        }
        private void ClickExpandLeft_Click(object sender, object vars)
        {
            try
            {
                int opened = Convert.ToInt32(flowLayoutPanel2.Tag);


                if (opened == 0)
                {
                    flowLayoutPanel2.Tag = 1; // set opened
                    //expand
                    //should look into the controls and get the wideest control in there
                    int maxwidth = 0;
                    foreach (Control c in flowLayoutPanel2.Controls) 
                    {
                        if (c.Width > maxwidth) 
                        {
                            maxwidth = c.Width;
                        }
                    }

                    flowLayoutPanel2.Width = maxwidth + 8;
                    ctlSupports1.Visible = true;
                }
                else
                {
                    //retract
                    flowLayoutPanel2.Tag = 0;
                    flowLayoutPanel2.Width = 50;
                    ctlSupports1.Visible = false;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        // move the tab to the 3d view
        private void ClickView3d(object sender, object e) 
        {
            ShowView(0);
        }
        // move the tab to the 3d view
        private void ClickViewSlice(object sender, object e)
        {
            ShowView(1);
        }
        // move the tab to the 3d view
        private void ClickViewConfig(object sender, object e)
        {
            ShowView(3);
        }

        private void ClickSwitchTabView_Click(object sender, object e) 
        {
            try
            {
                //cast sender to be the ctlImageButton of the ctlTitle
                ctlImageButton button = (ctlImageButton)sender;
                //find the tab view
                tabview tabv = null;
                foreach (tabview tv in m_lsttabs) 
                {
                    if (tv.m_title == button) 
                    {
                        tabv = tv;
                        break;
                    }
                }
                if (tabv != null) 
                {
                    //UncheckTabs(tabv.m_title);
                    ShowView(tabv.m_tabidx);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        /*
        private void ClickMainConfigView_Click(object sender, object e)
        {
            UncheckTabs(ctlTitleConfigure);
            ShowView(eViewTypes.eVConfig);
        }

        private void ClickManualCtlView_Click(object sender, object e)
        {
            
            UncheckTabs(ctlTitleViewControls);

            ShowView(eViewTypes.eVControl);
        }
        private void ctlTitle3dView_Click(object sender, object e)
        {
            
            UncheckTabs(ctlTitle3dView);

            ShowView(eViewTypes.eV3d);
        }
        private void ctlTitleViewSlice_Click(object sender, object e)
        {
            
            UncheckTabs(ctlTitleViewSlice);

            ShowView(eViewTypes.eVSlice);
        }
         * */
        #endregion

        public MenuStrip GetMenuStrip() 
        {
            return this.menuStrip1;
        }
        private void testMachineControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frm = new frmTest();
            frm.ShowDialog();
        }

        public void SaveScene(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SaveScene(sender, e); }));
            }
            else
            {
                //saveFileDialog1.Filter = "Scene files (*.cws)|*.cws|STL File (*.stl)|*.stl";
                saveFileDialog1.Filter = "Scene files (*." + SceneFileExt + ")|*." + SceneFileExt + "|STL File (*.stl)|*.stl";
                saveFileDialog1.FilterIndex = 0;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //check the filter index
                    switch (saveFileDialog1.FilterIndex) // index starts at 1 instead of 0
                    {
                        case 1:
                            SceneFile.Instance().SaveModelsIntoScene(saveFileDialog1.FileName);
                            if (UVDLPApp.Instance().m_buildparms.exportpreview != PreviewGenerator.ePreview.None)
                            {
                                PreviewGenerator pg = new PreviewGenerator();
                                pg.ViewAngle = UVDLPApp.Instance().m_buildparms.exportpreview;
                                Bitmap preview = pg.GeneratePreview(512, 512);
                                    SceneFile.Instance().AddPreviewImage(UVDLPApp.Instance().SceneFileName, preview, "Default", "ScenePreview.png");
                            }
                            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSceneSaved, "cws");
                            break;
                        case 2:
                            //stl file
                            UVDLPApp.Instance().CalcScene(); // calc the scene object
                            UVDLPApp.Instance().Scene.SaveSTL_Binary(saveFileDialog1.FileName);
                            UVDLPApp.Instance().Scene.m_fullname = saveFileDialog1.FileName;
                            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSceneSaved, "stl");
                            break;
                    }
                }
            }
        }

        private void testLoadSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (SceneFile.Instance().LoadSceneFile(openFileDialog1.FileName)) 
                {
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
                }
            }
        }

        private void flowLayoutPanel1_ClientSizeChanged(object sender, EventArgs e)
        {
            if (rightToolsWidth == 0)
                rightToolsWidth = flowLayoutPanel1.Width;
            int newwidth = rightToolsWidth;
            if (flowLayoutPanel1.VerticalScroll.Visible)
                newwidth = rightToolsWidth + flowLayoutPanel1.Width - flowLayoutPanel1.ClientSize.Width;
            if (flowLayoutPanel1.Width != newwidth)
                flowLayoutPanel1.Width = newwidth;
        }

        private void testNewSlicerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (testNewSlicerToolStripMenuItem.Checked)
                UVDLPApp.Instance().m_slicer.m_slicemethod = Slicer.eSliceMethod.eNormalCount;
            else
                UVDLPApp.Instance().m_slicer.m_slicemethod = Slicer.eSliceMethod.eEvenOdd;
        }

        private void loadGUIConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // load a new GUIConfig file from disk
                // this is a debug only function for now.
                openFileDialog1.Filter = "Gui Config Files (*.xml;*.txt)|*.xml;*.txt|All files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
                {
                    // test new gui config system
                    GuiConfigDB gconfdb = new GuiConfigDB();
                    gconfdb.LoadConfigFromFile(openFileDialog1.FileName);
                    //gconfdb.PreviewMode = true;
                    UVDLPApp.Instance().m_gui_config.ApplyConfiguration(gconfdb);
                    Control winLayout = UVDLPApp.Instance().m_gui_config.GetLayout("MainLayout");
                    if (winLayout != null)
                    {
                        Form frm = new Form();
                        frm.Size = new Size(winLayout.Width, winLayout.Height);
                        frm.Controls.Add(winLayout);
                        frm.ShowDialog();
                    }
                    UVDLPApp.Instance().m_gui_config.DumpDatabase("GuiManagerDatabase.txt");
                    //UVDLPApp.Instance().m_gui_config.LayoutGui(
                }
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //check for an update, contact the server
            UVDLPApp.Instance().m_sc.CheckForUpdate();
        }

        private void miiManualControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMiiTest mii = new frmMiiTest();
            mii.Show();
        }

        private void ctl3DView1_Load(object sender, EventArgs e)
        {

        }

        private void frmMain2_MouseClick(object sender, MouseEventArgs e)
        {
            // can I hit - test every control in the form from here?
            // the goal here is to find out what control was clicked on so we can start skinning that way.
            int x = e.Location.X;
            int y = e.Location.Y;
            //this.Controls

        }

        private void frmMain2_MouseMove(object sender, MouseEventArgs e)
        {
            e = e;
        }
    }
}
