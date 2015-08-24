using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;
using UV_DLP_3D_Printer.GUI.CustomGUI;

// Xml Helper class for new configuration system -SHS
namespace UV_DLP_3D_Printer.Configs
{
    public class XmlHelper
    {
        XmlDocument m_xdoc;
        String m_filename;
        String m_name;
        XmlAttribute m_verattr;
        
        public XmlNode m_toplevel;
        public int m_version;


        protected void NewDocument(String docName)
        {
            m_xdoc.AppendChild(m_xdoc.CreateXmlDeclaration("1.0", "utf-8", ""));
            m_toplevel = m_xdoc.CreateElement(docName);
            m_verattr = m_xdoc.CreateAttribute("FileVersion");
            m_version = 0;
            m_verattr.Value = m_version.ToString();
            m_toplevel.Attributes.Append(m_verattr);
            m_xdoc.AppendChild(m_toplevel);
        }

        // Start new empty XML document.
        public void StartNew(String filename, String docName)
        {
            m_xdoc = new XmlDocument();
            m_filename = filename;
            NewDocument(docName);
        }
        // Read XML document from stream
        public bool LoadFromStream(Stream stream, String docName)
        {
           // m_filename = filename;
           // m_name = System.IO.Path.GetFileName(filename);
            m_xdoc = new XmlDocument();
            m_toplevel = null;
            //bool fileExist = System.IO.File.Exists(m_filename);
            try
            {
                m_xdoc.Load(stream);
                m_toplevel = m_xdoc.ChildNodes[1];
                //m_version = GetInt(m_xdoc, "FileVersion", 0);
                if (m_toplevel.Name != docName)
                {
                    m_toplevel = null;
                }
                else
                {
                    m_verattr = m_toplevel.Attributes["FileVersion"];
                    m_version = int.Parse(m_verattr.Value);
                }
                return true;
            }
            catch (Exception ex)
            {
               
                DebugLogger.Instance().LogError(m_name + ": " + ex.Message);
              
                m_toplevel = null;
            }
            return false;
        }
        // Read XML document from file, if not exist, start new XML document. 
        public bool Start(String filename, String docName)
        {
            m_filename = filename;
            m_name = System.IO.Path.GetFileName(filename);
            m_xdoc = new XmlDocument();
            m_toplevel = null;
            bool fileExist = System.IO.File.Exists(m_filename);
            try
            {
                m_xdoc.Load(m_filename);
                m_toplevel = m_xdoc.ChildNodes[1];
                //m_version = GetInt(m_xdoc, "FileVersion", 0);
                if (m_toplevel.Name != docName)
                    m_toplevel = null;
                else
                {
                    m_verattr = m_toplevel.Attributes["FileVersion"];
                    m_version = int.Parse(m_verattr.Value);
                }
            }
            catch (Exception ex)
            {
                if (fileExist)
                {
                    DebugLogger.Instance().LogError(m_name + ": " + ex.Message);
                    m_xdoc.RemoveAll();
                }
                m_toplevel = null;
            }
            if (m_toplevel == null)
            {
                NewDocument(docName);
            }
            return fileExist;
        }

        /*public bool Start(MemoryStream ms, String docName)
        {
            m_filename = null;
            m_name = docName;
            m_xdoc = new XmlDocument();
            m_toplevel = null;
            try
            {
                m_xdoc.Load(ms);
                m_toplevel = m_xdoc.ChildNodes[1];
                //m_version = GetInt(m_xdoc, "FileVersion", 0);
                if (m_toplevel.Name != docName)
                    m_toplevel = null;
                else
                {
                    m_verattr = m_toplevel.Attributes["FileVersion"];
                    m_version = int.Parse(m_verattr.Value);
                }
            }
            catch (Exception ex)
            {
                m_toplevel = null;
            }
            return true;
        }*/
        
        public XmlNode FindChildElement(XmlNode parentNode, String elemName)
        {
            foreach (XmlNode nd in parentNode.ChildNodes)
            {
                if (nd.Name == elemName)
                {
                    return nd;
                }
            }
            return null;
        }
        /// <summary>
        /// Iterate through all nodes at this level and add all
        /// nodes that match the name to the list
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="elemName"></param>
        /// <returns></returns>
        public List<XmlNode> FindAllChildElement(XmlNode parentNode, String elemName)
        {
            List<XmlNode> m_nodes = new List<XmlNode>();
            foreach (XmlNode nd in parentNode.ChildNodes)
            {
                if (nd.Name == elemName)
                {
                    m_nodes.Add(nd); ;
                }
            }
            return m_nodes;
        }

        protected void LogParamErr(XmlNode parent, String id, String type)
        {
            DebugLogger.Instance().LogRecord(m_name + ": Invalid parameter for " + parent.Name + "." + id + " should be " + type);
        }

        // if parent is null, find top level section
        public XmlNode FindSection(XmlNode parentNode, String elemName)
        {
            XmlNode nd;
            if (parentNode == null)
            {
                parentNode = m_toplevel;
            }
            nd = FindChildElement(parentNode, elemName);
            if (nd == null)
            {
                nd = m_xdoc.CreateElement(elemName);
                parentNode.AppendChild(nd);
            }
            return nd;
        }

        // this will add a section even if a section with the same name exists
        // it is used for arrays of records
        public XmlNode AddSection(XmlNode parentNode, String elemName)
        {
            XmlNode nd;
            if (parentNode == null)
            {
                parentNode = m_toplevel;
            }
            nd = m_xdoc.CreateElement(elemName);
            parentNode.AppendChild(nd);
            return nd;
        }

        public List<XmlNode> GetAllSections(XmlNode parentNode, String sectName)
        {
            if (parentNode == null)
            {
                parentNode = m_toplevel;
            }
            List<XmlNode> ndlist = new List<XmlNode>(); 
            foreach (XmlNode nd in parentNode.ChildNodes)
            {
                if (nd.Name == sectName)
                {
                    ndlist.Add(nd);
                }
            }
            return ndlist;
        }

        protected void AddParameter(XmlNode parentNode, String id, String val)
        {
            XmlNode nd = m_xdoc.CreateElement(id);
            nd.InnerText = val;
            parentNode.AppendChild(nd);
        }

        // get commands

        public String GetString(XmlNode parentNode, String id, String default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            if (nd != null)
            {
                return nd.InnerText;
            }
            AddParameter(parentNode, id, default_val);
            return default_val;
        }

        public double GetDouble(XmlNode parentNode, String id, double default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            double result;
            if (nd == null)
            {
                AddParameter(parentNode, id, default_val.ToString());
                return default_val;
            }
            try
            {
                result = double.Parse(nd.InnerText);
            }
            catch (Exception)
            {
                LogParamErr(parentNode, id, "floating point number");
                result = default_val;
            }
            return result;
        }
        /// <summary>
        
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="id"></param>
        /// <param name="default_val"></param>
        /// <returns></returns>
        public int GetInt(XmlNode parentNode, String id, int default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            int result;
            if (nd == null)
            {
                AddParameter(parentNode, id, default_val.ToString());
                return default_val;
            }
            try
            {
                // read from old file..
                bool tmp;
                bool success = bool.TryParse(nd.InnerText,out tmp);
                if (success) // this was a boolean var (old bool)
                {
                    result = 0;
                }
                else // this came from the new style (new int)
                {
                    result = int.Parse(nd.InnerText);
                }
                
            }
            catch (Exception)
            {
                LogParamErr(parentNode, id, "integer number");
                result = default_val;
            }
            return result;
        }

        private int GetColorComponent(String str, String compid, Char endchar)
        {
            int start = str.IndexOf(compid) + 2;
            if (start < 1)
                return - 1;
            int len = str.IndexOf(endchar, start) - start;
            if (len < 1)
                return -1;
            return int.Parse(str.Substring(start, len));
        }

        public Color ParseColor(String colstr)
        {
            Color result;
            if (colstr.StartsWith("Color ["))
            {
                int a = GetColorComponent(colstr, "A=", ',');
                if (a >= 0)
                {
                    int r = GetColorComponent(colstr, "R=", ',');
                    int g = GetColorComponent(colstr, "G=", ',');
                    int b = GetColorComponent(colstr, "B=", ']');
                    result = Color.FromArgb(a, r, g, b);
                }
                else
                {
                    int len = colstr.IndexOf(']') - 7;
                    result = Color.FromName(colstr.Substring(7, len));
                }
            }
            else
            {
                result = Color.FromArgb(int.Parse(colstr));
            }
            return result;
        }

        public Color GetColor(XmlNode parentNode, String id, Color default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            Color result;
            if (nd == null)
            {
                AddParameter(parentNode, id, default_val.ToString());
                return default_val;
            }
            try
            {
                result = ParseColor(nd.InnerText);
            }
            catch (Exception)
            {
                LogParamErr(parentNode, id, "color value");
                result = default_val;
            }
            return result;
        }

        public Boolean GetBool(XmlNode parentNode, String id, Boolean default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            Boolean result;
            if (nd == null)
            {
                AddParameter(parentNode, id, default_val.ToString());
                return default_val;
            }
            try
            {
                result = Boolean.Parse(nd.InnerText);
            }
            catch (Exception)
            {
                LogParamErr(parentNode, id, "True or False");
                result = default_val;
            }
            return result;
        }

        public Object GetEnum(XmlNode parentNode, String id, Type type, Object default_val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            Object result;
            if (nd == null)
            {
                AddParameter(parentNode, id, default_val.ToString());
                return default_val;
            }
            try
            {
                result = Enum.Parse(type, nd.InnerText);
            }
            catch (Exception)
            {
                StringBuilder sb = new StringBuilder();
                String [] names = Enum.GetNames(type);
                for (int i = 0; i < names.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(names[i]);
                }
                LogParamErr(parentNode, id, "one of: " + sb.ToString());
                result = default_val;
            }
            return result;
        }

        // set commands

        public void SetString(XmlNode parentNode, String id, String val)
        {
            XmlNode nd = FindChildElement(parentNode, id);
            if (nd != null)
            {
                nd.InnerText = val;
            }
            else
            {
                AddParameter(parentNode, id, val);
            }
        }

        public void SetParameter(XmlNode parentNode, String id, Object val)
        {
            SetString(parentNode, id, val.ToString());
        }

        // save general purpuse user parameter
        public void SetParameter(XmlNode parentNode, CWParameter param)
        {
            if (param.paramName == null)
                return; // parameter must have a name
            XmlNode nd = FindChildElement(parentNode, param.paramName);
            if (nd == null)
            {
                nd = m_xdoc.CreateElement(param.paramName);
                parentNode.AppendChild(nd);
            }
            param.SaveUser(m_xdoc, nd);
        }

        public void SaveUserParamList(XmlNode parentNode, UserParameterList parList)
        {
            foreach (KeyValuePair<string, CWParameter> pair in parList.paramDict)
            {
                SetParameter(parentNode, pair.Value);
            }
            parList.ConfigChanged = false;
        }

        public void SaveUserParamList(UserParameterList parList)
        {
            XmlNode nd = FindChildElement(m_toplevel, "UserParameters");
            if (nd == null)
            {
                nd = m_xdoc.CreateElement("UserParameters");
                m_toplevel.AppendChild(nd);
            }
            SaveUserParamList(nd, parList);
        }

        public void LoadUserParamList(XmlNode parentNode, UserParameterList parList)
        {
            foreach (XmlNode xnode in parentNode.ChildNodes)
            {
                CWParameter par = CWParameter.LoadUser(xnode);
                if (par != null)
                    parList.paramDict[par.paramName] = par;
            }
        }

        public void LoadUserParamList(UserParameterList parList)
        {
            XmlNode nd = FindChildElement(m_toplevel, "UserParameters");
            if (nd == null)
                return;
            LoadUserParamList(nd, parList);
        }

        public bool Save(int version)
        {

            try
            {
                m_version = version;
                m_verattr.Value = version.ToString();  // an exception was being thrown here first load - smh
                m_xdoc.Save(m_filename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(m_name + ": " + ex.Message);
                return false;
            }
            return true;
        }


        public bool Save(int version, ref MemoryStream stream)
        {

            try
            {
                m_version = version;
                m_verattr.Value = version.ToString();  // an exception was being thrown here first load - smh
                m_xdoc.Save(stream);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(m_name + ": " + ex.Message);
                return false;
            }
            return true;
        }
       

    }
}
