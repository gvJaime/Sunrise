using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer.Util.Sequence;
using System.Diagnostics;

namespace UV_DLP_3D_Printer.Util.Sequence
{
    /// <summary>
    /// a process sequence is a list of 1 or more processes that can run external applications
    /// we can generate parameters at run time based on variables from the config files, state vars, etc...
    /// </summary>
    class ProcessEntry 
    {
        public string processname;
        public string args;
        public bool wait; // wait for process to complete?
        public string windowstyle;
        public ProcessEntry Clone() 
        {
            ProcessEntry pe = new ProcessEntry();
            pe.processname = processname;
            pe.args = args;
            pe.wait = wait;
            return pe;
        }
        private string EvaluateArgs() 
        {
            return args;
        }
        private ProcessWindowStyle GetWindowStyle() 
        {
            switch (windowstyle.ToLower()) 
            {
                case "hidden":
                    return ProcessWindowStyle.Hidden;
                case "maximized":
                    return ProcessWindowStyle.Maximized;
                case "minimized":
                    return ProcessWindowStyle.Minimized;
                case "normal":
                    return ProcessWindowStyle.Normal;
            }
            return ProcessWindowStyle.Normal;
        }
        public void RunProcess() 
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = processname;
            startInfo.Arguments = EvaluateArgs();
            startInfo.WindowStyle = GetWindowStyle();
            

            DebugLogger.Instance().LogInfo("Process Name: " + startInfo.FileName);
            DebugLogger.Instance().LogInfo("Arguements: " + startInfo.Arguments);

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    if (wait)
                    {
                        exeProcess.WaitForExit();
                        int exitcode = exeProcess.ExitCode;
                        DebugLogger.Instance().LogInfo("Process returned : " + exitcode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError("Process execution failed " + ex.Message);
            }   
        }
    }
    class ProcessSequence : CommandSequence
    {
        public List<ProcessEntry> m_entries;   
        private void Init()
        {
            m_entries = new List<ProcessEntry>();
            base.m_seqtype = COMMAND_TYPE_SPAWN_PROCESS;
        }
        public ProcessSequence() 
        {
            Init();
        
        }
        public ProcessSequence(string name)
        {
            //create empty process, make new list
            m_entries = new List<ProcessEntry>();
            m_name = name;
            Init();
        }

        public ProcessSequence Clone() 
        {
            ProcessSequence newseq = new ProcessSequence();
            newseq.m_name = m_name;
            newseq.m_seqtype = m_seqtype;
            foreach (ProcessEntry pe in m_entries) 
            {
                ProcessEntry pec = pe.Clone();
                newseq.m_entries.Add(pec);
            }
            return newseq;
        }
    }
}
