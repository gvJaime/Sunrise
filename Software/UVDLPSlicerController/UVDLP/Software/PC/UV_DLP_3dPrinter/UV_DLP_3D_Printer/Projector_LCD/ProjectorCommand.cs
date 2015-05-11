using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UV_DLP_3D_Printer
{
    [Serializable]
    public class ProjectorCommand
    {
        public string command; // the hex-encoded string
        public bool hex; // hex or ascii
        public string name; // the name of this command
        
        public byte[] GetBytes() 
        {
            try
            {
                string cmd = command.Replace(" ", string.Empty);
                return Utility.HexStringToByteArray(cmd);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return null;
            }
        }
        public ProjectorCommand() 
        {
            command = "";
            hex = false;
            name = "none";
        }
    }
}
