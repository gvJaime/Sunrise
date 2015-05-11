using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Collections;
using UV_DLP_3D_Printer.Drivers;

namespace UV_DLP_3D_Printer.Device_Interface.AutoDetect
{
    /// <summary>
    /// This is the Serial Auto Detect Class
    /// It's purpose is to identify which port(s) the printer/machine is connected with.
    /// It will list all the serial ports
    ///     - create a serial port detecter for each port
    ///     - run the serial detector on each port
    ///     - send one or more commands that will query the machine
    ///     - recieve a response (or not)
    ///     - report results
    /// </summary>
    public class SerialAutodetect
    {
        public class SerialAutodetectConfig 
        {
            public int m_baud; // we need to specify the baud at which we're auto-detecting...
        }
        private static SerialAutodetect m_instance = null;
        private List<ConnectionTester> m_list; // list of connectionTester objects we're spawning
        private List<ConnectionTester> m_lstresults; // the results of the autodetect - 
       // private SerialAutodetectConfig m_config;
        private const long TIMEOUTTIME = 5000; // time out value
        private long m_starttime; // for timeout
        private object m_lock = new object(); // for safe reading of the list

        private SerialAutodetect() 
        {
                    
        }
        private void Initialize() 
        {
            m_list = new List<ConnectionTester>();
            m_lstresults = new List<ConnectionTester>();
        }

        public static SerialAutodetect Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new SerialAutodetect();
            }
            return m_instance;
        }
        /// <summary>
        /// this function starts the serial port detection, waits for the results to return, and 
        /// reports back the detect serial port name
        /// </summary>
        /// <returns></returns>
        public string DeterminePort(int baud) 
        {
            Initialize();
            string comport = "invalid";
            bool done = false;
            SerialAutodetectConfig config = new SerialAutodetectConfig();            
            config.m_baud = baud;
            //open and test all serial ports at once
            foreach (String s in DeviceDriver.GetPortNames())
            {
                if (s.Equals("COM113"))
                {
                    // create a new tester
                    ConnectionTester tester = new ConnectionTester(s); // specify the name of the port we're trying to detect
                    //set the baud
                    tester.m_baud = config.m_baud;
                    //set up to listen to events
                    tester.ConnectionTesterStatusEvent += new ConnectionTester.ConnectionTesterStatus(ConnectionTesterStatusDel);
                    //start it off
                    tester.Start();
                }
            }
            m_starttime = Environment.TickCount;
            while (!done) 
            {
                try
                {
                    //check for timeout
                    if (Environment.TickCount >= (m_starttime + TIMEOUTTIME))
                        done = true;

                    Thread.Sleep(0); // yield
                    lock (m_lock)
                    {
                        // check the m_lstresults
                        foreach (ConnectionTester con in m_lstresults)
                        {
                            if (con.m_result == ConnectionTester.eConnTestStatus.eDeviceResponded)
                            {
                                comport = con.m_portname;
                                done = true;
                                Initialize();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
            return comport;
        }

        void ConnectionTesterStatusDel(ConnectionTester obj,ConnectionTester.eConnTestStatus status) 
        {
            m_lstresults.Add(obj);
        }
    }
}
