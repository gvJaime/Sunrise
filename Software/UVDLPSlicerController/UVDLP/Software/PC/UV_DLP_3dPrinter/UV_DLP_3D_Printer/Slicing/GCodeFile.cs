using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UV_DLP_3D_Printer
{
    public class GCodeFile
    {
        String m_gcode; // the entire file
        String[] m_lines; // the file, one string per line

        public GCodeFile(String gc) 
        {
            m_gcode = gc;
            m_lines = null;
        }
        public GCodeFile(Stream gcs)
        {
            Load(gcs);
        }

        private void GenerateLines()         
        {
            m_lines = m_gcode.Split('\n'); // split on the newline
        }

        public bool Load(Stream instream) 
        {
            try
            {
                TextReader tw = new StreamReader(instream);
                m_lines = null;
                m_gcode = tw.ReadToEnd();
              //  tw.Close();
                m_lines = null;
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }            
        
        }
        public bool Load(String filename) 
        {
            try
            {
                TextReader tw = File.OpenText(filename);
                m_lines = null;
                m_gcode = tw.ReadToEnd();
                tw.Close();
                m_lines = null;
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }            
        }

        public bool Save(String filename)
        {
            try
            {
                TextWriter tw = File.CreateText(filename);
                tw.Write(m_gcode);
                tw.Close();
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }            
        }
        public bool Save(Stream outstream)
        {
            try
            {
                TextWriter tw = new StreamWriter(outstream);
                tw.Write(m_gcode);
               // tw.Close();
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// this is a cheat to find, parse and return the number of slices in a gcode file for UV DLP GCode
        /// </summary>
        /// <returns></returns>
        public int GetVar(string var) 
        {
            try
            {
                foreach (string s in Lines)
                {
                    if (s.Contains(var))
                    {
                        String str = s;
                        str = str.Replace('(', ' ');
                        str = str.Replace(')', ' ');
                        str = str.Replace(';', ' ');
                        string[] tmp = str.Split('=');
                        return int.Parse(tmp[1]);

                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
            return -1;
        }

        public double GetDoubleVar(string var)
        {
            try
            {
                foreach (string s in Lines)
                {
                    if (s.Contains(var))
                    {
                        String str = s;
                        str = str.Replace('(', ' ');
                        str = str.Replace(')', ' ');
                        str = str.Replace(';', ' ');
                        string[] tmp = str.Split('=');
                        string[] t2 = tmp[1].Split(' ');
                        return double.Parse(t2[0]);

                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
            return -1;
        }
        
        public String[] Lines 
        {
            get 
            {
                if (m_lines == null)
                    GenerateLines();
                return m_lines; 
            }
        }
        public String RawGCode 
        {
            get 
            {
                return m_gcode; 
            }
            set 
            {
                m_gcode = value;
                m_lines = null; // clear it so it will be re-generated when needed
            }
        }
        /// <summary>
        /// returns true if this file 'looks' like it contains markers for UV DLP slicing
        /// </summary>
        /// <returns></returns>
        public bool IsUVDLPGCode() 
        {
            foreach (string ln in Lines) 
            {
                if (ln.Contains("<Slice> ")) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
