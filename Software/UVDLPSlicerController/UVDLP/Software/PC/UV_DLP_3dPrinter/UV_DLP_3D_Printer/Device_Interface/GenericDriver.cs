using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Drivers
{
    /// <summary>
    /// This is a generic serial port class it's primary purpose is to send gcode data
    /// but it can also generally send general ascii or hex data
    /// </summary>
    public class GenericDriver : DeviceDriver
    {
        public static string PASSTHROUGHMCODE = "M800";
        public GenericDriver() 
        {
            m_drivertype = eDriverType.eGENERIC;
            UVDLPApp.Instance().m_deviceinterface.AlwaysReady = false;
           
        }
        public override bool Connect() 
        {
            try
            {
                m_serialport.Open();
                if (m_serialport.IsOpen)
                {
                    m_connected = true;
                    RaiseDeviceStatus(this, eDeviceStatus.eConnect);
                    Logging = UVDLPApp.Instance().m_appconfig.m_driverdebuglog; // enable / disable logging
                    return true;
                }
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
            return false;
        }
        public override bool Disconnect() 
        {
            try
            {
                m_serialport.Close();
                m_connected = false;
                RaiseDeviceStatus(this, eDeviceStatus.eDisconnect);
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
            
        }
        protected readonly object _locker = new object();
        
        public override int Write(byte[] data, int len) 
        {
            lock (_locker)
            {
                m_serialport.Write(data, 0, len);
                return len;
            }
        }

        protected string RemoveComment(string line) 
        {
           
            string newln = "";
            // this function removes the comments from the line
            line = line.Trim(); // trim off any whitespace
            if (line.StartsWith(";"))
            {
                return "";
            }
            if (line.Contains(';'))
            {
                // this line does not start with a comment, but contains a comment,
                // split the line and give only the first portion
                String[] Lines = line.Split(';');
                if (Lines.Length > 0)
                {
                    newln = Lines[0];
                    newln += "\r\n"; // make sure to cap it off               
                }
                else
                {
                    //wtf here?
                    DebugLogger.Instance().LogError("Should be a line here....");
                }
            }
            else 
            {
                //line contains no comments
                newln = line + "\r\n";
            }
            return newln;
        }

        /// <summary>
        /// The idea of the passthrough here is that some devices require things OTHER than straight gcode/mode
        /// some are binary drivers like the , some use another ascii protocol like the Uncia or the MiiCraft printers
        /// normally, I'd take the time to write another device driver specifically for these printers, but I'm kinda lazy, 
        /// and I think there has to be a better more generic approach, and this IS the eGeneric driver, so:
        /// 
        /// This function looks for the passthrough command PASSTHROUGHMCODE. This is a GCode/MCode that
        /// is executed on the HOST side and not the microcontroller/motor driver side.
        /// It is not passed through to the client side. This command will take a ascii-encoded 
        /// hex string and convert it to a sequence of bytes to be sent raw to the serial port
        /// Returns:
        /// if this function identifies the passthrough command and successfully sends the raw byte stream, it will return 
        /// true, it will return false otherwise.
        /// </summary>
        public bool DoPassthrough(string line)
        {
            try
            {
                // the PASSTHROUGHMODE command is setup like so:
                // M800 T0 [ASCII]
                // M800 T1 [ASCII-Encoded HEX]
                // for example: M800 T0 qrs ; this would pass through the ascii codes for 'qrs' and nothing more
                // M800 T1 0f3ed52be3 ;// this would take the ascii-encoded hex string and send the bytes
                if (line.StartsWith(PASSTHROUGHMCODE))
                {
                    string hexstr = line;
                    hexstr = hexstr.Replace(PASSTHROUGHMCODE, string.Empty); // get rid of the command
                    hexstr = hexstr.Trim(); // trim off the proceeding space
                    if (hexstr.StartsWith("T0")) 
                    {
                        hexstr = hexstr.Replace("T0", string.Empty);
                        hexstr = hexstr.Trim();
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(hexstr);
                        m_serialport.Write(data,0, data.Length);
                        return true;                         
                    }
                    else if (hexstr.StartsWith("T1"))
                    {
                        hexstr = hexstr.Replace("T1", string.Empty);
                        hexstr = hexstr.Trim();
                        byte[] data = Utility.HexStringToByteArray(hexstr); // convert to a byte array
                       // Write(data, data.Length); // send the raw bytes of data
                        m_serialport.Write(data, 0, data.Length);
                        return true;
                    }
                    else 
                    {
                        //error
                        DebugLogger.Instance().LogError(PASSTHROUGHMCODE + " Parse error:" + line);
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public override int Write(String line) 
        {
            lock (_locker) // ensure synchronization
            {
                line = RemoveComment(line);
                if (line.Trim().Length > 0)
                {
                    Log(line);
                    if (!DoPassthrough(line))
                    {
                        m_serialport.Write(line);
                    }
                }
                return line.Trim().Length;
            }
        }
    }
}
