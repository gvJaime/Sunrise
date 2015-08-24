using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace UV_DLP_3D_Printer.Building
{
    /// <summary>
    /// This is the auxilery build command manager
    /// the Gcode allows for ';<auxcmd> cmdname ' statements
    /// This will cause gcode sequences or mini-algorithms to be run during the build sequence
    /// This class is the class that manages all those sequences, and will wait till completion
    /// </summary>
    public class AuxBuildCmds
    {
        private static AuxBuildCmds m_instance = null;
        private List<AuxCmd> m_lstCmds;
        private bool cmdcompleted;
        private AuxBuildCmds() 
        {
            m_lstCmds = new List<AuxCmd>();
            cmdcompleted = false;
        }
        public static AuxBuildCmds Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new AuxBuildCmds();
            }
            return m_instance;
        }
        public void AddCommand(AuxCmd cmd) 
        {
            m_lstCmds.Add(cmd);
            cmd.AuxCmdCompletedEvent += new AuxCmd.AuxCmdCompleted(cmd_AuxCmdCompletedEvent);
        }

        void cmd_AuxCmdCompletedEvent(AuxCmd cmd)
        {
            cmdcompleted = true;
        }

        /// <summary>
        /// This is a blocking call for now
        /// </summary>
        /// <param name="cmdname"></param>
        public void RunCmd(string cmdname, string parms = "") 
        {
            cmdcompleted = false;
            foreach (AuxCmd cmd in m_lstCmds) 
            {
                if(cmdname.Equals(cmd.m_cmdname))
                {
                    if (parms.Length != 0)
                    {
                        cmd.Execute(parms); // execute the command with parameters
                    }
                    else 
                    {
                        cmd.Execute(); // execute the command
                    }
                    
                    while (!cmdcompleted) //and wait for completion
                    {
                        Thread.Sleep(0);
                    }
                }
            }
            cmdcompleted = false;
        }
    }
}
