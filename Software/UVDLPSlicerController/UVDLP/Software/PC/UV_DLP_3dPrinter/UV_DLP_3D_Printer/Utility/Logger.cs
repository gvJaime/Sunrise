using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace UV_DLP_3D_Printer
{
    //public delegate void LoggerMessageHandler(Logger o, string message);
    public delegate void LoggerStatusHandler(Logger o, eLogStatus status, string message);
    public enum eLogStatus
    {
        eLogOpened,
        eLogClosed,
        eLogOpenError,
        eLogCloseError,
        eLogLowDiskSpace,
        eLogWriteError,
        eLogWroteRecord,
        eLogEnabled,
        eLogDisabled
    }
    public enum eLoggerType
    {
        eDataLogger,
        eDebugLogger,
        eLogger,
        eHexLogger,
    }
    public class Logger
    {
        public eLoggerType m_loggertype;
        // public event LoggerMessageHandler LoggerMessageEvent;
        public event LoggerStatusHandler LoggerStatusEvent;
        private StreamWriter sw = null;                 //Stream Writer
        private string m_logpath;
        private bool m_enablelogging = true;
        /// <summary>
        /// Logs a message to a Log File
        /// </summary> 
        public Logger()
        {
            m_loggertype = eLoggerType.eLogger;
        }

        /// <summary>
        /// Destructor:: Clear out the instance
        /// </summary>
        ~Logger()
        {
            CloseLogFile();
        }
        public string CreateTimeStampFileName(string basename)
        {
            string fn = basename;
            DateTime dt = DateTime.Now;
            // MM_DD_YYYY_HH_MM_SS.LOG
            fn += "_" + dt.Month.ToString();
            fn += "_" + dt.Day.ToString();
            fn += "_" + dt.Year.ToString();
            fn += "_" + dt.Hour.ToString();
            fn += "_" + dt.Minute.ToString();
            fn += "_" + dt.Second.ToString();
            fn += ".log";
            return fn;
        }
        public void RaiseLogStatusEvent(Logger log, eLogStatus status, string message)
        {
            if (LoggerStatusEvent != null)
            {
                LoggerStatusEvent(log, status, message);
            }
        }
        public bool EnableLogging
        {
            get { return m_enablelogging; }
            set
            {
                m_enablelogging = value;
                if (value)
                {
                    RaiseLogStatusEvent(this, eLogStatus.eLogEnabled, "Logging Enabled");
                }
                else
                {
                    RaiseLogStatusEvent(this, eLogStatus.eLogDisabled, "Logging Disabled");
                }
            }
        }

        /// <summary>
        /// Outputs a record to the log File
        /// </summary>
        /// <param name="OutStr">String to write</param>
        public virtual void LogRecord(string OutStr)
        {
            RaiseLogStatusEvent(this, eLogStatus.eLogWroteRecord, OutStr);
            if (m_enablelogging == false) return;
            if (IsLogFileOpen() == false) return;
            try
            {
                sw.WriteLine(OutStr);
                sw.Flush();
                
            }
            catch (Exception)
            {
                RaiseLogStatusEvent(this, eLogStatus.eLogWriteError, "Error Writing to Log file");
            }
        }
        /// <summary>
        /// this function will take an array of bytes and log the bytes as
        /// a hex string with spaces between each byte (2 digit hex #).
        /// It logs 16 bytes per row 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        public void LogHexRecord(byte[] data, int offset, int len)
        {
            //
            if (m_enablelogging == false) return;
            if (IsLogFileOpen() == false) return;
            try
            {

                int position = offset;
                bool done = false;
                string outstr = "";
                while (!done)
                {
                    for (int c = 0; c < 16; c++)
                    {
                        if (position >= offset + len) { done = true; break; }
                        byte d = data[position];
                        outstr += string.Format("{0:x2} ", d);
                        position++;
                    }
                    outstr += "\r\n";
                }
                sw.WriteLine(outstr);
                sw.Flush();
                RaiseLogStatusEvent(this, eLogStatus.eLogWroteRecord, outstr);
            }
            catch (Exception)
            {
                RaiseLogStatusEvent(this, eLogStatus.eLogWriteError, "Error Writing to Log file");
            }
        }
        private void OpenLogFile()
        {
            try
            {
                if (IsLogFileOpen()) // if it's already open, close it
                {
                    CloseLogFile();
                }
                sw = new StreamWriter(m_logpath, false); // open it
                RaiseLogStatusEvent(this, eLogStatus.eLogOpened, "Log Opened");
            }
            catch (Exception)
            {
                RaiseLogStatusEvent(this, eLogStatus.eLogOpenError, "Error Opening log file");
            }
        }
        public void CloseLogFile()
        {
            try
            {
                if (IsLogFileOpen())
                {
                    sw.Close();
                    sw = null;
                    RaiseLogStatusEvent(this, eLogStatus.eLogClosed, "Log file closed");
                }
            }
            catch (Exception)
            {
                RaiseLogStatusEvent(this, eLogStatus.eLogCloseError, "Error closing log file");
            }
        }
        private bool IsLogFileOpen()
        {
            if (sw == null) return false;
            return true;
        }
        public static string TimeStamp()
        {
                DateTime CurTime = DateTime.Now;
                return CurTime.ToString("HH:mm:ss.fff");
        }
        public void SetLogFile(string fname)
        {
            try
            {
                m_logpath = fname;
                if (m_enablelogging)
                {
                    if (IsLogFileOpen())
                    {
                        CloseLogFile(); // Close and re-open to reset the name                        
                    }
                    OpenLogFile();
                }
            }
            catch (Exception)
            {
                // couldn't open log file
            }
        }
    }
}
