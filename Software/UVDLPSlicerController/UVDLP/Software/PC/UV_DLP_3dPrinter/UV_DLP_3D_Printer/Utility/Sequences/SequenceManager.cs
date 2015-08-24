using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.Util.Sequence
{
    /// <summary>
    /// The sequence manager is in charge of holding all the command sequences used
    /// these are loaded from the various GUIConfig xml's in the system
    /// each sequence has a name
    /// The sequence manager is in charge of executing the various sequence types
    /// to start off with, we're implementing gcode command sequences tied to GUI buttons
    /// </summary>
    public class SequenceManager
    {
        private List<CommandSequence> m_lstsequences; // bucket of sequences
        private static SequenceManager m_instance = null;
        public static SequenceManager Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new SequenceManager();
            }
            return m_instance;
        }
        public List<CommandSequence> Sequences 
        {
            get { return m_lstsequences; }
        }
        private SequenceManager() 
        {
            m_lstsequences = new List<CommandSequence>();
        }
        public void Remove(CommandSequence seq) 
        {
            try
            {
                m_lstsequences.Remove(seq);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        public void Add(CommandSequence seq) 
        {
            m_lstsequences.Add(seq);
        }
        private CommandSequence Find(string name) 
        {
            foreach (CommandSequence seq in m_lstsequences) 
            {
                if (name.Equals(seq.m_name)) 
                {
                    return seq;
                }
            }
            return null;
        }
        public bool ExecuteSequence(string name) 
        {
            try
            {
                CommandSequence seq = Find(name);
                if (seq == null)
                    return false;
                switch (seq.m_seqtype)
                {
                    case CommandSequence.COMMAND_TYPE_GCODE:
                        //a gcode command sequence - execute it with a pseudo-build manager/sequencer
                        GCodeFile gcf = new GCodeFile(seq.m_seq);
                        if (!UVDLPApp.Instance().m_buildmgr.IsPrinting) 
                        {
                            DebugLogger.Instance().LogInfo("Running GCode Sequence " + seq.m_name);
                            UVDLPApp.Instance().m_buildmgr.StartPrint(UVDLPApp.Instance().m_slicefile, gcf, true);
                        }
                        break;
                    case CommandSequence.COMMAND_TYPE_SPAWN_PROCESS:
                        //use the parameter variables to fill in the args, then spawn the process
                        // a flag can be to wait for the process to end , or simply start it.
                        ProcessSequence psc = (ProcessSequence)seq;
                        foreach (ProcessEntry pe in psc.m_entries) 
                        {
                            pe.RunProcess();
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }
    }
}
