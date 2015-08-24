using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Collections;
using System.Threading;

namespace UV_DLP_3D_Printer.Device_Interface.AutoDetect
{
    public class ConnectionTester
    {
        public string m_portname;
        private SerialPort m_serialport;
        public int m_baud;
        public eConnTestStatus m_result; // results are store here
        private Thread m_thread;
        private bool m_running;
        private TestingState m_state;
        private const string QUERYSTRING = "M114\r\n";
        private const int TIMEOUTVAL = 3000;
        private string m_strb;
        private enum TestingState 
        {
            eSendMessage,
            eWaitForReply,
            eExiting
        }
        public event ConnectionTesterStatus ConnectionTesterStatusEvent;
        public enum eConnTestStatus 
        {            
            eOpenFailure, // port could not be opened
            eWriteError, // we opend the port, but couldn't write
            eDeviceResponded, // device responded to the message we sent correctly
            eNoResponse // no response or response not expected
        }
        public delegate void ConnectionTesterStatus(ConnectionTester obj,eConnTestStatus status); // report a final status

        public ConnectionTester(string port) 
        {
            m_portname = port;
        }

        public void Start() 
        {
            m_strb = "";
            bool erroropen = false;
            m_thread = new Thread(new ThreadStart(run));
            m_serialport = new SerialPort();
            m_serialport.BaudRate = m_baud;
            m_serialport.DataBits = 8;
            m_serialport.Parity = Parity.None;
            m_serialport.Handshake = Handshake.None;
            m_serialport.PortName = m_portname;
            //set up a listener to receive data
            m_serialport.DataReceived += new SerialDataReceivedEventHandler(m_serialport_DataReceived);
            // try to open the port now.
            try
            {
                m_serialport.Open();
            }
            catch (Exception) 
            {
                erroropen = true;
            }
            // check to see if there was an error, or the port is not open
            if (erroropen || m_serialport.IsOpen == false) 
            {
                DebugLogger.Instance().LogInfo("Error opening serial port " + m_portname);
                // couldn't open port for some reason, set return result
                m_result = eConnTestStatus.eOpenFailure;
                if (ConnectionTesterStatusEvent != null)
                {
                    ConnectionTesterStatusEvent(this, m_result);
                    return; // exit and don't start the thread
                }                
            }
            //if we're here, the port is open, let's try to send some data
            m_state = TestingState.eSendMessage; // go to the send message state
            m_thread.Start();
            m_running = true;
        }
        /// <summary>
        /// Function to recieve the data, this won't be triggered in Mono
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void m_serialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte []buffer = new byte[m_serialport.BytesToRead]; // allocate storage for the return
            int read = m_serialport.Read(buffer, 0, m_serialport.BytesToRead); // read the buffer
            // convert it to a string
            //try to parse it...

            string str = System.Text.Encoding.UTF8.GetString(buffer); // need to put all the data into a buffer and wait for newline
            m_strb += str;
            if (m_strb.Contains("\r\n"))
                ParseResults(m_strb);
        }

        void ParseResults(string data) 
        {
            if (data.Contains("X:") && data.Contains("Y:") && data.Contains("Z:")) 
            {
                // we received the position, we're done here
                if (ConnectionTesterStatusEvent != null)
                {
                    m_result = eConnTestStatus.eDeviceResponded;
                    ConnectionTesterStatusEvent(this, m_result);
                    m_state = TestingState.eExiting; // we're done here..
                }  
            }
        }

        int GetTimerValue()
        {
            return Environment.TickCount;
        }
        public void run() 
        {
            int sendstarttime = 0;
            while (m_running) 
            {
                try
                {
                    Thread.Sleep(0);
                    // wait for response
                    switch (m_state)
                    {
                        case TestingState.eSendMessage:
                            sendstarttime = GetTimerValue(); // mark the time we sent the message
                            try
                            {                                
                                m_serialport.Write(QUERYSTRING); // send the query string
                                m_state = TestingState.eWaitForReply; // go to the waiting state
                            }
                            catch (Exception) // check to see if the write failed
                            {
                                m_result = eConnTestStatus.eWriteError;
                                if (ConnectionTesterStatusEvent != null)
                                {
                                    ConnectionTesterStatusEvent(this, m_result);
                                    m_state = TestingState.eExiting; // we're done here..
                                }
                            }
                            break;
                        case TestingState.eWaitForReply:
                            //check for timeout
                            if (GetTimerValue() > (sendstarttime + TIMEOUTVAL))
                            {
                                m_result = eConnTestStatus.eNoResponse; // we timed out
                                if (ConnectionTesterStatusEvent != null) // raise the result
                                {
                                    ConnectionTesterStatusEvent(this, m_result);
                                    m_state = TestingState.eExiting; // we're done here..
                                }
                            }
                            break;
                        case TestingState.eExiting:
                            m_running = false; // signal to exit this thread
                            if (m_serialport.IsOpen)
                                m_serialport.Close();
                            break;
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
        }
    }
}
