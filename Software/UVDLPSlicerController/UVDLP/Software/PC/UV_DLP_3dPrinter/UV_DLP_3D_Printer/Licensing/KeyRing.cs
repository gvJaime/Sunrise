using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CreationWorkshop.Licensing;
namespace UV_DLP_3D_Printer.Licensing
{
    /// <summary>
    /// This class stores and retrieves all the keys
    /// </summary>
    public class KeyRing
    {
        public static KeyRing m_instance = null;
        public List<LicenseKey> m_keys;
        private KeyRing() 
        {
            m_keys = new List<LicenseKey>();
        }
        public LicenseKey Find(int vid) 
        {
            try
            {
                foreach (LicenseKey lk in m_keys)
                {
                    if (lk.valid)
                    {
                        if (lk.VendorID == (ushort)vid)
                        {
                            return lk;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return null;
        }
        public static KeyRing Instance()
        {
            if (m_instance == null) 
            {
                m_instance = new KeyRing();
            }
            return m_instance;
        }

        public bool Load(string file) 
        {
            try
            {
                m_keys.Clear();
                TextReader tr = new StreamReader(file);
                while (true) 
                {
                    string line = tr.ReadLine();
                    if (line != null)
                    {
                        line = line.Trim();
                        if (line.Length > 0)
                        {
                            LicenseKey lk = new LicenseKey();
                            lk.Init(line);
                            m_keys.Add(lk);
                        }
                    }
                    else 
                    {
                        break;
                    }
                }
                tr.Close();
                tr = null;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                return false;
            }
            return true;
        }

        public bool Save(string file) 
        {
            try
            {
                TextWriter tw = new StreamWriter(file);
                foreach (LicenseKey lk in m_keys) 
                {
                    tw.WriteLine(lk.m_key);
                }
                tw.Close();
                tw = null;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
                return false;
            }
            return true;
        }
    }
}
