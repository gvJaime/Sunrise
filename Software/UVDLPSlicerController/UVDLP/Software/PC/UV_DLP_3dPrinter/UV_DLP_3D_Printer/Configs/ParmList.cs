using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using UV_DLP_3D_Printer.Slicing.Modules;

namespace UV_DLP_3D_Printer.Configs
{
    /*
     What I'm trying to do with this parm list is to create a storage for 
     * program parameters, there can be multiple parmlists, each acts as it's own
     * config file, my plan is to replace the slicing and machine profiles with this.
     
     */
    public class ParmList
    {
        protected ArrayList m_parms = null;
        public Parm GetParm(string name)
        {
            foreach (Parm p in m_parms)
            {
                if (p.m_name == name)
                {
                    return p;
                }
            }
            return null;
        }

        //define an indexer to get access to the vars
        public Parm this[string name]
        {
            get
            {
                return null;
            }
            set 
            {
                
            }
        }
        public bool Save(string filename) 
        {
            return false;
        }

        public bool Load(string filename) 
        {
            return false;
        }
        public double GetDouble(string name)
        {
            Parm p = GetParm(name);
            if (p != null)
            {
                return p.m_dval;
            }
            return 0.0;
        }
        public void AddParms(Parm[] parms)
        {
            if (m_parms == null)
            {
                m_parms = new ArrayList();
            }
            foreach (Parm p in parms)
            {
                m_parms.Add(p);
            }
        }

        public void ClearParms()
        {
            m_parms = new ArrayList();
        }
        public ArrayList Parms
        {
            get
            {
                return m_parms;
            }
        }
    }
}
