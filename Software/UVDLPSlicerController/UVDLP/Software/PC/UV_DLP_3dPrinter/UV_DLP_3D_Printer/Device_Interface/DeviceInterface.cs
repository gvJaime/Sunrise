using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Timers;
using System.IO;
using UV_DLP_3D_Printer.Drivers;
using UV_DLP_3D_Printer.Configs;


namespace UV_DLP_3D_Printer
{
    /*
     This class is the main interface to communicate with the printer
     * it controls the serial connection to the machine and provides 
     * data back from the machine (such as temperature readout)
     * This printer interface will support a *very limited subset of g-code commands
     * that it will use to control the printer
     * I'm not sure if choosing GCodes/MCodes is the *right choice,
     * but hey, whatever works for now...
     * 
     * I'm including a few manual commands in here because i intend on implementing a printer control panel
     * that can manually jog and set temperatures
     * 
     * the DeviceInterface can only handle one command at a time (by design)
     * the previous command must either time out or respond
     * 
     * GCode listing 
     * 
     * G1 - Coordinated Motion
     * G28 - Home given Axes to maximum
     * G92 - Define current position on axes
     * 
     * 
     * MCode listing
     * M0 Unconditional Halt
     * M17 enable motor(s)
     * M18 disable motor(s)     
     * M109 Snnn set build platform temperature in degrees Celsuis
     * M128 get position
     * 
     * 
     * 
     */

    public enum ePIStatus 
    {
       // eReady, // ready for next command
        eError, // something went wrong
        eConnected, // device is now connected
        eDisconnected // device disconnected
    }

    public class DeviceInterface
    {
        //declare a delegate for the outside world to listen in
        public delegate void DeviceInterfaceStatus(ePIStatus status, String Command);
        public delegate void DeviceDataReceived(DeviceDriver device, byte[] data, int length); // raw data
        public delegate void DeviceLineReceived(DeviceDriver device, string line); // line of data terminated by a \r\n

        public DeviceInterfaceStatus StatusEvent;
        public DeviceDataReceived DataEvent; // event for indicating data received from the serial port
        public DeviceLineReceived LineDataEvent; // event for indicating a 'line' of data was received - termniated by '\r\n'
        private DeviceDriver m_driver; //support for a single device driver 
        private List<DeviceDriver> m_lstprojectors; // serial interface for multiple monitors

        private bool m_ready; // ready to send a command
        // this is an override for the ready, some drivers like the EliteImageWorks,
        //don't have GCode responses, so we shoudn't wait for an "OK"
        private bool m_alwaysready;

        private byte[] m_databufA;
        private byte[] m_databufB;
        private const int BUFF_SIZE = 4096;
        private const char TERMCHAR = '\n';

        public DeviceInterface() 
        {
            m_lstprojectors = new List<DeviceDriver>();
            m_driver = null;
            m_ready = true;
            m_databufA = null;// new byte[BUFF_SIZE];
            m_databufB = null;// new byte[BUFF_SIZE];
            m_alwaysready = false; // assume it's a generic driver that requires gcode response to be ready
        }

        public bool AlwaysReady 
        {
            get { return m_alwaysready; }
            set { m_alwaysready = value; }
        }

        // get and set the printdriver
        public DeviceDriver Driver
        {
            get { return m_driver; }
            set 
            {
                if (m_driver != null)
                {
                    DeviceDriver olddriver = m_driver;
                    if(olddriver.Connected == true)
                        olddriver.Disconnect(); // disconnect the old driver
                    //remove the old device driver delegates
                    olddriver.DataReceived -= new DeviceDriver.DataReceivedEvent(DriverDataReceivedEvent);
                    olddriver.DeviceStatus -= new DeviceDriver.DeviceStatusEvent(DriverDeviceStatusEvent);
                }
                //set the new driver
                m_driver = value; 
                //and bind the delegates to listen to events
                m_driver.DataReceived += new DeviceDriver.DataReceivedEvent(DriverDataReceivedEvent);
                m_driver.DeviceStatus += new DeviceDriver.DeviceStatusEvent(DriverDeviceStatusEvent);
            }
        }

        public DeviceDriver FindProjDriverByComName(string comname) 
        {
            try
            {
                foreach (DeviceDriver dd in m_lstprojectors) 
                {
                    ConnectionConfig cc = dd.m_config;
                    if (cc.comname.Equals(comname)) 
                    {
                        return dd;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return null;
        }
        /*
         Adds a new projector driver serial port
         */
        public void AddDriver(DeviceDriver d)
        {
            m_lstprojectors.Add(d);
            //add some events here..
            d.DataReceived += new DeviceDriver.DataReceivedEvent(DriverDataReceivedEvent);
            d.DeviceStatus += new DeviceDriver.DeviceStatusEvent(DriverDeviceStatusEvent);
        }
        /*
         Remove a projector driver serial port
         */
        public void RemoveAllProjDrivers() 
        {
            foreach (DeviceDriver d in m_lstprojectors) 
            {
                d.DataReceived -= new DeviceDriver.DataReceivedEvent(DriverDataReceivedEvent);
                d.DeviceStatus -= new DeviceDriver.DeviceStatusEvent(DriverDeviceStatusEvent);
                d.Disconnect();
            }
            m_lstprojectors.Clear();
        }
        public void ConnectAllProjDrivers() 
        {            
            foreach (DeviceDriver d in m_lstprojectors)
            {
                if (d.Connect()) 
                {
                    DebugLogger.Instance().LogInfo("Projector serial driver connected");
                }
            }        
        }
        public void DisconnectAllProjDrivers()
        {
            foreach (DeviceDriver d in m_lstprojectors)
            {
                d.Disconnect();
            }
        }
       
        public DeviceDriver GetDriver(int i) 
        {
            return m_lstprojectors[i];
        }
        
        public void Configure(ConnectionConfig cc) 
        {
            Driver.Configure(cc);
        }
        public void DriverDeviceStatusEvent(DeviceDriver device, eDeviceStatus status) 
        {
            switch (status) 
            {
                case eDeviceStatus.eError:
                    if (StatusEvent != null)
                    {
                        StatusEvent(ePIStatus.eError, "I/O Error");
                    }                    
                    break;
                case eDeviceStatus.eConnect:                    
                    if (StatusEvent != null) 
                    {
                        StatusEvent(ePIStatus.eConnected, "Connected");
                    }
                    break;
                case eDeviceStatus.eDisconnect:
                    if (StatusEvent != null)
                    {
                        StatusEvent(ePIStatus.eDisconnected, "Disconnected");
                    }
                    break;

            }
        }
        private static object lockobj = new object();
        // this is called when we receive data from the device driver
        // one or more full lines can be received here
        void DriverDataReceivedEvent(DeviceDriver device, byte[] data, int length) 
        {
            lock (lockobj)
            {
                try
                {
                    // m_ready = true; // not sure I should do this here, I think it may be the cause of the flow control issue
                    // raise the data event
                    if (DataEvent != null)
                    {
                        DataEvent(device, data, length); // raise an event whenever data is received
                    }

                    if (m_alwaysready) 
                    {
                        // don't go any further to parse responses and generate line data events
                        // alwaysready is true for EliteImageWorks driver
                        return; 
                    }
                    // copy the data into the A buffer
                    int termpos = -1;
                    // copy the data into the 'A' buffer
                    // need to copy into the end of the A buffer
                    if (m_databufA == null)
                    {
                        m_databufA = CopyData(0, data, 0, length);

                    }
                    else
                    {
                        m_databufA = AddBuffers(m_databufA, data);
                    }
                    termpos = Term_Pos(m_databufA, m_databufA.Length);
                    while (termpos != -1)
                    {
                        m_databufB = CopyData(0, m_databufA, 0, termpos + 1);
                        string result = System.Text.Encoding.ASCII.GetString(m_databufB); 
                        m_ready = true;

                        if (LineDataEvent != null)
                        {
                            LineDataEvent(device, result); // raise an event for each complete line we receive
                        }
                        m_databufB = CopyData(0, m_databufA, termpos + 1, (m_databufA.Length - (termpos + 1)));
                        m_databufA = CopyData(0, m_databufB, 0, m_databufB.Length);
                        termpos = Term_Pos(m_databufA, m_databufA.Length); // check again
                    }
                }
                catch (Exception)
                {
                    // DebugLogger.Instance().LogError(ex.Message);  // this is erroring on the null driver for some reason
                }
            }
        }

        public static int Term_Pos(byte[] buffer, int len)
        {
            for (int c = 0; c < len; c++)
            {
                if (buffer[c] == TERMCHAR)
                {
                    return c;
                }
            }
            return -1;
        }  

        public static byte[] AddBuffers(byte[] Abuf, byte[] Bbuf)
        {
            byte []retbuf = new byte[Abuf.Length + Bbuf.Length ];
            int c=0;
            for(c=0;c<Abuf.Length;c++)
            {
                retbuf[c]=Abuf[c];     		
            }
            for(int i=0;i<Bbuf.Length;i++)
            {
                retbuf[c++] = Bbuf[i];     		
            }
            return retbuf;     	
        }
        public static byte[] CopyData(int dst_start, byte[] srcbuffer, int src_start, int datalen)
        {
            try
            {

                byte[] dstbuffer = new byte[datalen];
                for (int c = 0; c < datalen; c++) 
                {
                    dstbuffer[dst_start + c] = srcbuffer[src_start + c];
                }
                return dstbuffer;

            }
            catch (Exception )
            {
                //DebugLogger.Instance().LogError(e.Message); // error with null driver causing issues, look into it
                return null;
                //DebugLogger.Instance().LogRecord("DM_Utils:CopyData: Error copying byte data " + e.Message);
            }
        }

        public bool Connected { get { return m_driver.Connected; } }
        //public bool ConnectedProjector { get { return m_lstprojectors.Connected; } }


        /*
         This function moves the Z axis to by the distance in mm 
         * at the specified feed rate
         */
        public void Move(double zpos, double rate)
        {
            if (!Connected)
            {
                DebugLogger.Instance().LogInfo("Device not connected");
                return;
            }
            else
            {
                String command = "G1 Z" + zpos + " F" + rate + "\r\n";
                SendCommandToDevice("G91\r\n");
                SendCommandToDevice(command);
                //SendCommandToDevice("G90\r\n");
            }
        }

        public void MoveE(double edist, double rate)
        {
            if (!Connected)
            {
                DebugLogger.Instance().LogInfo("Device not connected");
                return;
            }
            else
            {
                SendCommandToDevice("M83\r\n");
                String command = "G1 E" + edist + " F" + rate + "\r\n";
                SendCommandToDevice(command);
                //SendCommandToDevice("G90\r\n");
            }
        }


        /*
         This function moves the X (Tilt/Slide) axis to by the distance in mm 
         * at the specified feed rate
         */
        public void MoveX(double xpos, double rate)
        {
            if (!Connected)
            {
                DebugLogger.Instance().LogInfo("Device not connected");
                return;
            }
            else
            {
                String command = "G1 X" + xpos + " F" + rate + "\r\n";
                SendCommandToDevice("G91\r\n");
                SendCommandToDevice(command);
                //SendCommandToDevice("G90\r\n");
            }
        }
        /*
         This function moves the Y (Tilt/Slide) axis to by the distance in mm 
         * at the specified feed rate
         */
        public void MoveY(double ypos, double rate)
        {
            String command = "G1 Y" + ypos + " F" + rate + "\r\n";
            SendCommandToDevice("G91\r\n");
            SendCommandToDevice(command);
            //SendCommandToDevice("G90\r\n");
        }
        public bool Disconnect()
        {
            lock (lockobj)
            {
                m_ready = false;
            }
            return m_driver.Disconnect();
        }
        public bool Connect()
        {
            try
            {
                lock (lockobj)
                {
                    m_ready = true;
                }
                return m_driver.Connect();
            }
            catch (Exception)
            {
                return false;
            }
        }
        //for drivers that need to be kicked every once in a while to set their states.
        public void SetReady(bool readyforcommand) 
        {
            m_ready = readyforcommand;
        }
        /// <summary>
        /// This will be true if the device is ready for another command
        /// </summary>
        /// <returns></returns>
        public bool ReadyForCommand() 
        {
            lock (lockobj)
            {
                return m_ready;
            }
        }

        public bool SendCommandToDevice(String command) 
        {
            UVDLPApp.Instance().m_callbackhandler.Activate("cmdLogGcode", this, command);
            try
            {
                lock (lockobj)
                {
                    // this could be the source of the lock
                    if (m_driver.Write(command) > 0)
                    {
                        if (AlwaysReady) // relys more on timing
                        {
                            m_ready = true; 
                        }
                        else 
                        {
                            m_ready = false;
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception) 
            {
                return false;
            }
        }
    }
}
