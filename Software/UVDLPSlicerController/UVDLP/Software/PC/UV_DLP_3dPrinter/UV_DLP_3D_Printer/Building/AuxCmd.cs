using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer.Building
{
    public class AuxCmd
    {
        public delegate void AuxCmdCompleted(AuxCmd cmd);
        // this should run a mini-build alg and signal when it's done
        public string m_cmdname;
        public event AuxCmdCompleted AuxCmdCompletedEvent; // this event should be raised when the command is done
        public virtual void Execute() 
        {
            
        }
        public virtual void Execute(string parms)
        {

        }
        protected void RaiseDone() 
        {
            if (AuxCmdCompletedEvent != null)
                AuxCmdCompletedEvent(this);
        }
    }
}
