using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UV_DLP_3D_Printer.Slicing.Modules
{
    // 

    /*
     * this is a general parameter that can be loaded or saved to an XML file
     Each module has a set of 0 or more defined slicing parameters 
     * These parameters are used by each module stage to perform 
     * module-specific actions
     * 
     * The application can use these as user-defined variables whereever a value is needed
     */
    public enum eParm 
    {
        eInt,
        eDouble,
        eString,
        eBool
    }

    public class Parm
    {
        public string m_name; // the name of the parameter (No Spaces)
        public int      m_ival; // the integer value (if used)
        public double   m_dval; // the double value (if used)
        public string m_sval;   // the string value (if used)
        public bool m_bval; // boolean value (if used)
        public string m_help; // description of this variable
        public eParm m_parmtype; // the type of paramter this is

        /*define some constructors here so the individual modules can define arrays of these */
        public Parm() // for the load
        {

        }
        public Parm(string name, int ival, string help) 
        {
            m_name = name;
            m_ival = ival;
            m_help = help;
            m_parmtype = eParm.eInt;
        }
        public Parm(string name, double dval, string help)
        {
            m_name = name;
            m_dval = dval;
            m_help = help;
            m_parmtype = eParm.eDouble;
        }
        public Parm(string name, string sval, string help)
        {
            m_name = name;
            m_sval = sval;
            m_help = help;
            m_parmtype = eParm.eString;
        }
        public Parm(string name, bool bval, string help)
        {
            m_name = name;
            m_bval = bval;
            m_help = help;
            m_parmtype = eParm.eBool;
        }
        public bool SetFromString(string value) 
        {
            try
            {
                switch (m_parmtype)
                {
                    case eParm.eBool:
                        m_bval = bool.Parse(value);
                        break;
                    case eParm.eDouble:
                        m_dval = double.Parse(value);
                        break;
                    case eParm.eInt:
                        m_ival = int.Parse(value);
                        break;
                    case eParm.eString:
                        m_sval = value;
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
            
        }
        public bool Load(XmlReader reader) 
        {
            try
            {
                m_name = reader.ReadElementString();
                m_parmtype = (eParm)Enum.Parse(typeof(eParm), reader.ReadElementString());
                SetFromString(reader.ReadElementString());
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }        
        }
        public bool Save(XmlWriter writer)
        {
            try
            {
                writer.WriteElementString("Name", m_name);
                writer.WriteElementString("Type", m_parmtype.ToString());
                writer.WriteElementString("Value", ToString());
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }
        public override string ToString() 
        {
            switch (m_parmtype) 
            {
                case eParm.eDouble:
                    return m_dval.ToString();
                case eParm.eInt:
                    return m_ival.ToString();
                case eParm.eString:
                    return m_sval;
                case eParm.eBool:
                    return m_bval.ToString();
            }
            return "";
        }
    }
}
