using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer
{
    public class DebugLogger : Logger
    {
        private static DebugLogger _instance = null;            //Make this class a singleton
        private List<string> m_logs = new List<string>(); // keep the log around for a while

        public DebugLogger()
        {
            m_loggertype = eLoggerType.eDebugLogger;
            SetLogFile("Creation.log");
        }
        
        public List<string> GetLog()
        {
            return m_logs;
        } 
        
        /// <summary>
        /// Destructor:: Clear out the instance
        /// </summary>
        ~DebugLogger()
        {
            CloseLogFile();
            _instance = null;
        }

        /// <summary>
        /// Gets the instance to the File Logger, creates if necessary
        /// </summary>
        /// <returns>Instance of this object</returns>
        public static DebugLogger Instance()
        {
            if (_instance == null)
                _instance = new DebugLogger();
            return _instance;
        }

        /// <summary>
        /// Outputs a record to the log File
        /// </summary>
        /// <param name="OutStr">String to write</param>
        public override void LogRecord(string OutStr)
        {
            string MsgOut;
            try
            {
                DateTime CurTime = DateTime.Now;
                MsgOut = CurTime.ToString("HH:mm:ss.fff") + "> " + OutStr;
                base.LogRecord(MsgOut);
                m_logs.Add(MsgOut);
            }
            catch (Exception) { }
        }
        public void LogInfo(string message) 
        {
            LogRecord("Info:"+message);
        }
        public void LogWarning(string message)
        {
            LogRecord("Warn:" + message);
        }

        public void LogError(string message)
        {
            LogRecord("Err :" + message);
        }
        public void LogError(Exception ex)
        {
            LogRecord("Err :" + ex.Message);
            LogRecord("Err :" + ex.StackTrace);
        }

    }
}
