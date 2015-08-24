using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer
{
    /// <summary>
    /// Projector command list
    /// </summary>
    /// 
    [Serializable]
    public class prjcmdlst
    {
        public static int FILE_VERSION = 1;
        public List<ProjectorCommand> m_commands;
        public prjcmdlst() 
        {
            m_commands = new List<ProjectorCommand>();
        }
        public ProjectorCommand FindByName(string name) 
        {
            try
            {
                foreach (ProjectorCommand pc in m_commands)
                {
                    if (pc.name.Equals(name))
                    {
                        return pc;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return null;
        }
        // load from xml file -SHS
        public bool Load(String filename)
        {
            m_commands.Clear();
            XmlHelper xh = new XmlHelper();
            xh.Start(filename, "ProjectorCmdList");
            List<XmlNode> ndlist = xh.GetAllSections(null, "Command");
            foreach (XmlNode nd in ndlist)
            {
                ProjectorCommand pc = new ProjectorCommand();
                pc.name = xh.GetString(nd, "Name", "none");
                pc.hex = xh.GetBool(nd, "IsHex", false);
                pc.command = xh.GetString(nd, "Cmd", "");
                m_commands.Add(pc);
            }
            return true;
        }

        // save to xml file -SHS
        public bool Save(String filename)
        {
            XmlHelper xh = new XmlHelper();
            xh.StartNew(filename, "ProjectorCmdList");
            foreach (ProjectorCommand pc in m_commands)
            {
                XmlNode nd = xh.AddSection(null, "Command");
                xh.SetParameter(nd, "Name", pc.name);
                xh.SetParameter(nd, "IsHex", pc.hex);
                xh.SetParameter(nd, "Cmd", pc.command);
            }
            return xh.Save(FILE_VERSION);
        }
    }
}
