using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Threading;
using UV_DLP_3D_Printer.Slicing;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer.Drivers;
using UV_DLP_3D_Printer.Building;

namespace UV_DLP_3D_Printer
{
    /*
     * This class controls print jobs from start to finish. It feeds the generated sliced images
     * one at a time, along with control information and GCode over the PrinterInterface
     * it can now also control FDM builds
     * 
     * A new function of this class is to run gcode 'snippets', or short sequences
     * triggered by GUIConfig defined sequences and onclick button events
     */
    /*
     This class raises an event when the printing starts,
     * when it stops
     * when it's cancelled 
     * and each time the layer changes
     */
    public enum eBuildStatus
    {
        eBuildStarted,
        eBuildCancelled,
        eBuildPaused,
        eBuildResumed,
        eLayerCompleted,
        eBuildCompleted,
        eBuildStatusUpdate
    }
    

    public delegate void delBuildStatus(eBuildStatus printstat, string message, int data);
    public delegate void delPrinterLayer(Bitmap bmplayer, int layernum, int layertype); // this is raised to display the next layer, mainly for UV DLP

    public class BuildManager
    {
        private  const int STATE_START                = 0;
        private  const int STATE_DO_NEXT_COMMAND      = 1;
        private  const int STATE_WAITING_FOR_DELAY    = 2;
        private  const int STATE_CANCELLED            = 3;
        private  const int STATE_IDLE                 = 4;
        private  const int STATE_DONE                 = 5;
        private  const int STATE_WAIT_DISPLAY         = 6; // waiting for the display to finish

        public const int SLICE_NORMAL                  =  0;
        public const int SLICE_BLANK                   = -1;
        public const int SLICE_CALIBRATION             = -2;
        public const int SLICE_SPECIAL                 = -3; // pulled from a plugin by name
        public const int SLICE_OUTLINE                 = -4; // this is a outline slice

        

        public delBuildStatus BuildStatus; // the delegate to let the rest of the world know
        public delPrinterLayer PrintLayer; // the delegate to show a new layer (UV DLP Printers)
        private bool m_printing = false;
        private bool m_paused = false;
        private bool m_runningsnippet; // are we executing a short gcode snippet?

        private int m_curlayer = 0; // the current visible slice layer index #
        SliceFile m_sf = null; // current file we're building
        GCodeFile m_gcode = null; // a reference from the active gcode file
        int m_gcodeline = 0; // which line of GCode are we currently on.
        int m_state = STATE_IDLE; // the state machine variable
        private Thread m_runthread; // a thread to run all this..
        private bool m_running; // a var to control thread life
        private DateTime m_printstarttime;
        private System.Timers.Timer m_buildtimer;
        private const int BUILD_TIMER_INTERVAL = 1000; // 2 second updates
        Bitmap m_blankimage = null; // a blank image to display
        Bitmap m_calibimage = null; // a calibration image to display
        private DateTime m_buildstarttime;
        private string estimatedbuildtime = "";
        bool callbackinited = false;
        private static int outlinelayer = 0;
        // the pause request and cancel request are used to ensure that
        // the build stops on a blank image, and not on exposing a slice
        private bool m_pause_request;
      //  private bool m_cancel_request;

        public BuildManager() 
        {
            m_buildtimer = new System.Timers.Timer();
            m_buildtimer.Elapsed += new ElapsedEventHandler(m_buildtimer_Elapsed);
            m_buildtimer.Interval = BUILD_TIMER_INTERVAL;
            m_pause_request = false;
            callbackinited = false;
          //  m_cancel_request = false;
            //install the callback handler for the displaydone
           // UVDLPApp.Instance().m_callbackhandler.RegisterCallback("DisplayDone", DisplayDone, null, "Indicates when the display device is done with the current slice");
        }
        /// <summary>
        /// This is a display callback handler
        /// it is used in circumstances where the display time may vary (like in the LaserSLA/SLS)
        /// the display device can signal completion by calling this function.
        /// This will cause the buildmanager to move onto the next build step 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="vars"></param>
        void DisplayDone(Object sender, Object vars) 
        {
            //move to the next build state
            m_state = BuildManager.STATE_DO_NEXT_COMMAND; // move onto next layer

        }
        private void StartBuildTimer() 
        {
            m_buildtimer.Start();
        }
        private void StopBuildTimer() 
        {
            m_buildtimer.Stop();
        }
        void m_buildtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                double percentdone = 0.0;
                if (m_gcode != null)
                {
                    double totallines = m_gcode.Lines.Length;
                    double curline = m_gcodeline;
                    percentdone = (curline / totallines) * 100.0;
                    TimeSpan span = DateTime.Now.Subtract(m_buildstarttime);

                    string tm = //"Elapsed " + span.Hours + ":" + span.Minutes + ":" + span.Seconds + " of " + EstimateBuildTime(m_gcode);
                    tm = String.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    tm = "Elapsed " + tm;
                    tm += " of " + estimatedbuildtime;
                    string mess = tm +  " - " + string.Format("{0:0.00}",percentdone) + "% Completed";
                    RaiseStatusEvent(eBuildStatus.eBuildStatusUpdate, mess, (int)percentdone);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
        /// <summary>
        /// This function will return the estimated build time for UV DLP print Jobs
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static String EstimateBuildTime(GCodeFile file) 
        {
            int bt = 0; // in milliseconds
            bool done = false;
            int gidx = 0;
            while (!done) 
            {
                if (gidx >= file.Lines.Length)
                {
                    done = true;
                    break;
                }

                String line = file.Lines[gidx++];
                if (line.Length > 0)
                {
                    // if the line is a comment, parse it to see if we need to take action
                    if (line.ToLower().Contains("<delay> "))// get the delay
                    {
                        int delay = getvarfromline(line);
                        bt += delay;
                    }
                }
            }
            TimeSpan ts = new TimeSpan();
            ts = TimeSpan.FromMilliseconds(bt);
            return String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }
        
        /// <summary>
        /// This GetBlank screen is trickier than it sounds
        /// some machines (like the Deep Imager 5) require a bitmap that is not 
        /// truly blank, but has red lines, so the projector doesn't go into standy
        /// we need to look for the image provided in the plugin if it exists and use that
        /// if it doesn't exist (no plugin - not the right plugin) , we'll just create  blank one
        /// at the display resolution
        /// </summary>
        /// <param name="xres"></param>
        /// <param name="yres"></param>
        /// <returns></returns>
        public void MakeBlank(int xres, int yres) 
        {
            if (m_blankimage == null )  // blank image is null, create it
            {
                // try to load it from the plug-in
                m_blankimage = UVDLPApp.Instance().GetPluginImage("Blank"); 
                //otherwise, create a new one
                if (m_blankimage == null)
                {
                    m_blankimage = new Bitmap(xres, yres);
                    using (Graphics gfx = Graphics.FromImage(m_blankimage))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        gfx.FillRectangle(brush, 0, 0, xres, yres);
                    }
                    m_blankimage.Tag = BuildManager.SLICE_BLANK;
                }
            }            
        }

        /// <summary>
        /// Make and show a new calibration image
        /// </summary>
        /// <param name="xres"></param>
        /// <param name="yres"></param>
        /// <param name="sc"></param>
        public void ShowCalibration(int xres, int yres, SliceBuildConfig sc) 
        {
           // if (m_calibimage == null)  // blank image is null, create it
            {
                m_calibimage = new Bitmap(xres,yres);
                m_calibimage.Tag = BuildManager.SLICE_CALIBRATION;
                // fill it with black
                using (Graphics gfx = Graphics.FromImage(m_calibimage))
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    gfx.FillRectangle(brush, 0, 0, xres, yres);
                    int xpos = 0, ypos = 0;
                    Pen pen = new Pen(new SolidBrush(Color.DarkRed));
                    for(xpos = 0; xpos < xres; xpos += (int)(sc.dpmmX*10.0))
                    {
                        Point p1 = new Point(xpos,0);
                        Point p2 = new Point(xpos,yres);
                        gfx.DrawLine(pen, p1, p2);
                    }
                    for (ypos = 0; ypos < yres; ypos += (int)(sc.dpmmY*10.0))
                    {
                        Point p1 = new Point(0, ypos);
                        Point p2 = new Point(xres, ypos);
                        gfx.DrawLine(pen, p1, p2);
                    }

                }
            }
            PrintLayer(m_calibimage, SLICE_CALIBRATION, SLICE_CALIBRATION);                    
        }
        public void ShowBlank(int xres, int yres) 
        {
            MakeBlank(xres, yres);
            PrintLayer(m_blankimage, SLICE_BLANK, SLICE_BLANK);            
        }
        /// <summary>
        /// This will return true while we are printing, even if paused
        /// </summary>
        public bool IsPrinting { get { return m_printing; } }

        private void RaiseStatusEvent(eBuildStatus status,string message, int data) 
        {
            if (m_runningsnippet == false) // don't emit build events for gcode snippets
            {
                if (BuildStatus != null)
                {
                    BuildStatus(status, message, data);
                }
            }
        }

        private void RaiseStatusEvent(eBuildStatus status, string message)
        {
            RaiseStatusEvent(status, message, 0);
        }


        public bool IsPaused() 
        {
            return m_paused;
        }
        private void ImplementPause() 
        {
            m_paused = true;
            m_state = STATE_IDLE;
            StopBuildTimer();
            RaiseStatusEvent(eBuildStatus.eBuildPaused, "Print Paused");
            // special coding for Elite Image Works
            // in the future, this should be pulled from the machine config file
            // special commands or something...
            Drivers.DeviceDriver dr = UVDLPApp.Instance().m_deviceinterface.Driver;
            string pausecmd = UVDLPApp.Instance().m_printerinfo.GetStringVar("PauseCommand");
            if (pausecmd.Length > 0)
            {
                UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(pausecmd);
            }        
        }

        public void PausePrint() 
        {
            if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
            {
                // for UV DLP printers, we need to wait till a blank screen before pausing
                m_pause_request = true;
            }
            else  
            {
                // for FDM or other printer types, we can stop immendiately
                ImplementPause();
            }

        }
        public void ResumePrint() 
        {
            m_paused = false;
            m_state = BuildManager.STATE_DO_NEXT_COMMAND;
            StartBuildTimer();
            RaiseStatusEvent(eBuildStatus.eBuildResumed,"Next Layer");
            Drivers.DeviceDriver dr = UVDLPApp.Instance().m_deviceinterface.Driver;
            string resumecmd = UVDLPApp.Instance().m_printerinfo.GetStringVar("ResumeCommand");
            if (resumecmd.Length > 0)
            {
                UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(resumecmd);
            }
        }

        // This function is called to start the print job
        public void StartPrint(SliceFile sf, GCodeFile gcode, bool snippet = false) 
        {

            //late init of callback handler 
            if (callbackinited == false)
            {
                UVDLPApp.Instance().m_callbackhandler.RegisterCallback("DisplayDone", DisplayDone, null, "Indicates when the display device is done with the current slice");
                callbackinited = true;
            }
            m_runningsnippet = snippet;

            if (m_printing)  // already printing
                return;
            //make sure to reset these
            m_pause_request = false;
            //m_cancel_request = false;

            m_printing = true;
            m_buildstarttime = new DateTime();
            m_buildstarttime = DateTime.Now;
            estimatedbuildtime = EstimateBuildTime(gcode);
            StartBuildTimer();
            
            m_sf = sf; // set the slicefile for rendering
            m_gcode = gcode; // set the file 
            m_state = STATE_START; // set the state machine as started
            m_runthread = new Thread(new ThreadStart(BuildThread));
            m_running = true;
            m_runthread.Start();
        }
        /// <summary>
        /// Get the name of the special image from the line
        /// so we can look for it in a plugin
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string GetSpecialName(string line) 
        {
            string sn = "";
            try
            {
                int idx = line.IndexOf("Special_");
                if (idx != -1)
                {
                    line = line.Replace("Special_", string.Empty);//remove the special part
                    sn = line.Substring(idx);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return sn;
        }
        private static int getvarfromline(String line) 
        {
            try
            {
                int val = 0;
                line = line.Replace(';', ' '); // remove comments
                line = line.Replace(')', ' ');
                String[] lines = line.Split('>');
                if (lines[1].ToLower().Contains("blank"))
                {
                    val = -1; // blank screen
                }    
                else if(lines[1].Contains("Special_"))
                {
                    val = -3; // special image
                }
                else if (lines[1].Contains("outline")) 
                { //;<slice> outline XXX
                    // 
                    val = SLICE_OUTLINE;
                    //still need to pull the number
                    String[] lns2 = lines[1].Trim().Split(' ');
                    outlinelayer = int.Parse(lns2[1].Trim()); // second should be variable
                }
                else
                {
                    String[] lns2 = lines[1].Trim().Split(' ');
                    val = int.Parse(lns2[0].Trim()); // first should be variable
                }
                
                return val;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(line);
                DebugLogger.Instance().LogError(ex);
                return 0;
            }            
        }

        private void SendProjCmd(MonitorConfig mc, ProjectorCommand pc)
        {
            try
            {
                //send the command
                if (mc.m_displayconnectionenabled == false)
                {
                    DebugLogger.Instance().LogWarning("Display connection not enabled");
                    return;
                }
                //Find the driver
                DeviceDriver driver = UVDLPApp.Instance().m_deviceinterface.FindProjDriverByComName(mc.m_displayconnection.comname);
                if (driver == null)
                {
                    DebugLogger.Instance().LogError("Driver not found");
                    return;
                }
                if (driver.Connected == true)
                {
                    //send the command.
                    driver.Write(pc.GetBytes(), pc.GetBytes().Length);
                }
                else
                {
                    DebugLogger.Instance().LogError("Driver not connected");
                    return;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
     
        }
        
        /// <summary>
        /// This function will perform an auxilary command as specified by the command name
        /// This can encompass a single extra gcode command defined somewhere else, 
        /// or an algorithm that blocks until complete
        /// This needs to be changed to allow for command parameters to be passed to aux command
        /// </summary>
        /// <param name="line"></param>
        public void PerformAuxCommand(string line) 
        {
            try
            {
                line = line.Replace(';', ' '); // remove comments
                line = line.Replace(')', ' '); // remove comments
                int bidx = line.IndexOf('>');
                if (bidx == -1)
                {
                    DebugLogger.Instance().LogError("Improperly formated auxerillery command");
                    return;
                }
                string ss1 = line.Substring(bidx + 1);
                //ss1 should now contain the command to perform
                ss1 = ss1.Trim();
                AuxBuildCmds.Instance().RunCmd(ss1);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        /// <summary>
        /// This function sends commands to the projector(s)
        /// The format is <DispCmd> MonitorID , cmdname
        /// Monitor ID can be any monitor on the system, or ALL
        /// </summary>
        /// <param name="line"></param>
        private void PerformDisplayCommand(string line)
        {
            try
            {
                line = line.Replace(';', ' '); // remove comments
                line = line.Replace(')', ' '); // remove comments
                int bidx = line.IndexOf('>');
                if (bidx == -1) 
                {
                    DebugLogger.Instance().LogError("Improperly formated display command");
                    return;
                }
                string ss1 = line.Substring(bidx+1);
                string[] lines = ss1.Split(',');
                if (lines.Length != 2) 
                {
                    DebugLogger.Instance().LogError("Improperly formated display command");
                    return;                
                }
                string monname = lines[0].Trim();
                string cmdname = lines[1].Trim();
                //get the command name
                ProjectorCommand pc = null;
                pc = UVDLPApp.Instance().m_proj_cmd_lst.FindByName(cmdname);
                if (pc == null) 
                {
                    DebugLogger.Instance().LogError("Could not find Display Command " + cmdname);
                    return;
                }
                // get the monitor ID
                if (monname.ToUpper().Equals("ALL"))
                {
                    //iterate through all configured monitors in machine monitor list
                    foreach (MonitorConfig mc in UVDLPApp.Instance().m_printerinfo.m_lstMonitorconfigs) 
                    {
                        SendProjCmd(mc, pc);
                    }
                }
                else 
                {
                    MonitorConfig mc = UVDLPApp.Instance().m_printerinfo.FindMonitorByName(monname);
                    if (mc != null)
                    {
                        SendProjCmd(mc, pc);
                    }
                    else 
                    {
                        DebugLogger.Instance().LogError("Monitor ID " + monname + " not found");
                        return;
                    }
                }
                
                // get the commands name
                //find the command in the projector command list
                // make sure the com port for the projector is open
                //send the command to the projector over serial
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        int GetTimerValue() 
        {
            return Environment.TickCount;
        }
        /*
         This is the thread that controls the build process
         * it needs to read the lines of gcode, one by one
         * send them to the printer interface,
         * wait for the printer to respond,
         * and also wait for the layer interval timer
         */
        void BuildThread() 
        {
            int now = GetTimerValue();
            int nextlayertime = 0;
            int sltime = -1, bltime = -1;
            while (m_running)
            {
                try
                {
                    //Thread.Sleep(1); //  sleep for 1 ms max
                    Thread.Sleep(0); //  sleep for 1 ms max
                    switch (m_state)
                    {
                        case BuildManager.STATE_START:
                            //start things off, reset some variables
                            RaiseStatusEvent(eBuildStatus.eBuildStarted, "Build Started");
                            m_state = BuildManager.STATE_DO_NEXT_COMMAND; // go to the first layer
                            UVDLPApp.Instance().m_deviceinterface.SetReady(true);// say we're ready
                            m_gcodeline = 0; // set the start line
                            m_curlayer = 0;
                            m_printstarttime = new DateTime();
                            break;
                        case BuildManager.STATE_WAITING_FOR_DELAY: // general delay statement
                            //check time var
                            if (GetTimerValue() >= nextlayertime)
                            {
                                //   DebugLogger.Instance().LogInfo("elapsed Layer time: " + GetTimerValue().ToString());
                                //   DebugLogger.Instance().LogInfo("Diff = " + (GetTimerValue() - nextlayertime).ToString());
                                m_state = BuildManager.STATE_DO_NEXT_COMMAND; // move onto next layer
                            }
                            else
                            {
                                Thread.Sleep(1); //  sleep for 1 ms to eliminate unnecessary cpu usage.
                            }
                            break;
                        case BuildManager.STATE_IDLE:
                            // do nothing
                            break;
                        case BuildManager.STATE_WAIT_DISPLAY: 
                            // we're waiting on the display to tell us we're done
                            // this is used in the LaserSLA plugin, the normal DLP mode uses just a simple <Delay> command
                            //do nothing
                            break;
                        case BuildManager.STATE_DO_NEXT_COMMAND:
                            //check for done
                            if (m_gcodeline >= m_gcode.Lines.Length)
                            {
                                //we're done..
                                m_state = BuildManager.STATE_DONE;
                                continue;
                            }
                            string line = "";
                            // if the driver reports we're ready for the next command, or
                            // if we choose to ignore the driver ready status
                            if (UVDLPApp.Instance().m_deviceinterface.ReadyForCommand() || (UVDLPApp.Instance().m_appconfig.m_ignore_response == true))
                            {
                                // go through the gcode, line by line
                                line = m_gcode.Lines[m_gcodeline++];
#if (DEBUG)
                                DebugLogger.Instance().LogInfo("Building : "+line);
#endif
                            }
                            else
                            {
                                continue; // device is not ready
                            }
                            line = line.Trim();
                            if (line.Length > 0) // if the line is not blank
                            {
                                // send  the line, whether or not it's a comment - this is for a reason....
                                // should check to see if the firmware is ready for another line

                                UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(line + "\r\n");
                                // if the line is a comment, parse it to see if we need to take action
                                if (line.ToLower().Contains("<delay> "))// get the delay
                                {
                                    nextlayertime = GetTimerValue() + getvarfromline(line);
                                    //DebugLogger.Instance().LogInfo("Next Layer time: " + nextlayertime.ToString());
                                    m_state = STATE_WAITING_FOR_DELAY;
                                    continue;
                                }
                                else if (line.ToLower().Contains("<dispcmd>"))  // display command
                                {
                                    PerformDisplayCommand(line);
                                }
                                else if (line.ToLower().Contains("<waitfordisplay>"))  // wait for display to be done
                                {                                   
                                    m_state = BuildManager.STATE_WAIT_DISPLAY;
                                }
                                else if (line.ToLower().Contains("<auxcmd>")) //auxillary command to run a pre-defined sequence
                                {
                                    PerformAuxCommand(line);
                                }
                                else if (line.ToLower().Contains("<slice> "))//get the slice number
                                {
                                    int layer = getvarfromline(line);
                                    int curtype = BuildManager.SLICE_NORMAL; // assume it's a normal image to begin with
                                    Bitmap bmp = null;

                                    if (layer == SLICE_BLANK)
                                    {
                                        MakeBlank(m_sf.XRes, m_sf.YRes);
                                        bmp = m_blankimage;
                                        curtype = BuildManager.SLICE_BLANK;
                                        bltime = GetTimerValue();
                                    }
                                    else if (layer == SLICE_SPECIAL) // plugins can override special images by named resource
                                    {
                                        // get the special image from the plugin (no caching for now..)
                                        string special = GetSpecialName(line);
                                        bmp = UVDLPApp.Instance().GetPluginImage(special);
                                        bmp.Tag = BuildManager.SLICE_SPECIAL;
                                        if (bmp == null) // no special image, even though it's specified..
                                        {
                                            MakeBlank(m_sf.XRes, m_sf.YRes);
                                            bmp = m_blankimage;
                                        }
                                        curtype = BuildManager.SLICE_BLANK;
                                    }
                                    else if (layer == SLICE_OUTLINE) 
                                    {
                                        // we're rendering the layer in an outline image
                                        // get the curlayer from the line
                                        m_curlayer = outlinelayer;
                                        if (m_sf != null)
                                        {
                                            bmp = m_sf.GetSliceImage(m_curlayer,true); // get the rendered image slice or load it if already rendered                                                                                
                                            if (bmp == null)
                                            {
                                                DebugLogger.Instance().LogError("Buildmanager bitmap is null layer = " + m_curlayer + " ");
                                            }
                                            else // not null
                                            {
                                                bmp.Tag = BuildManager.SLICE_NORMAL; // set the tag to normal so it is destroyed
                                            }
                                        }
                                        else
                                        {
                                            DebugLogger.Instance().LogWarning("Slice File is null during build and slice image is specified");
                                        }
                                        sltime = GetTimerValue();
                                    }
                                    else
                                    {
                                        m_curlayer = layer;
                                        if (m_sf != null)
                                        {
                                            bmp = m_sf.GetSliceImage(m_curlayer); // get the rendered image slice or load it if already rendered                                                                                
                                            if (bmp == null)
                                            {
                                                DebugLogger.Instance().LogError("Buildmanager bitmap is null layer = " + m_curlayer + " ");
                                            }
                                            else // not null
                                            {
                                                bmp.Tag = BuildManager.SLICE_NORMAL;
                                            }
                                        }
                                        else
                                        {
                                            DebugLogger.Instance().LogWarning("Slice File is null during build and slice image is specified");
                                        }
                                        sltime = GetTimerValue();
                                        //DebugLogger.Instance().LogInfo("Showing Slice image at :" + sltime.ToString());
                                        // if (sltime != -1 && bltime != -1)
                                        //   DebugLogger.Instance().LogInfo("Time between slice and blank :" + (sltime - bltime).ToString());


                                    }
                                    // if a pause is requested, stop here...
                                    if (m_pause_request == true)
                                    {
                                        m_pause_request = false;
                                        ImplementPause();
                                        // if the slice is blank, continue on to display it
                                        // if it's a new image slice (non-blank) continue the loop to go to the next state
                                        if (curtype != BuildManager.SLICE_BLANK)
                                            continue;
                                    }

                                    //raise a delegate so the main form can catch it and display layer information.
                                    if (PrintLayer != null)
                                    {
                                        PrintLayer(bmp, m_curlayer, curtype);
                                    }
                                    
                                    RaiseStatusEvent(eBuildStatus.eLayerCompleted,"Completed Layer");
                                }
                            }
                            break;
                        case BuildManager.STATE_DONE:
                            try
                            {
                                m_running = false;
                                m_state = BuildManager.STATE_IDLE;
                                StopBuildTimer();
                                DateTime endtime = new DateTime();
                                double totalminutes = (endtime - m_printstarttime).TotalMinutes;

                                m_printing = false; // mark printing doe
                                //raise done message
                                RaiseStatusEvent(eBuildStatus.eBuildStatusUpdate, "Build 100% Completed");
                                RaiseStatusEvent(eBuildStatus.eBuildCompleted, "Build Completed");
                            }
                            catch (Exception ex)
                            {
                                DebugLogger.Instance().LogError(ex.StackTrace);
                            }
                            break;
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex.StackTrace);
                }
            }
        }


        public int GenerateTimeEstimate() 
        {
            return -1;
        }

        // This function manually cancels the print job
        public void CancelPrint() 
        {
            if (m_printing) // only if we're already printing
            {
                m_printing = false;
                StopBuildTimer();
                m_curlayer = 0;
                m_state = BuildManager.STATE_IDLE;
                m_running = false;
                RaiseStatusEvent(eBuildStatus.eBuildCancelled, "Build Cancelled");
                string cancelcmd = UVDLPApp.Instance().m_printerinfo.GetStringVar("PrintCancelCommand");
                if (cancelcmd.Length > 0)
                {
                    UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(cancelcmd);
                }
            }
            m_paused = false;
        }
    }
}
