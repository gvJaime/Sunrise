using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UV_DLP_3D_Printer;

namespace Engine3D
{
    public abstract class ModelLoaderType
    {
        protected String m_lastError = null;
        protected String m_fileExt = null;
        protected String m_fileDesc = null;
        public abstract List<Object3d> LoadModel(String filename);

        protected void LogError(String err)
        {
            m_lastError = err;
            DebugLogger.Instance().LogError(err); 
        }

        public String FileExtension
        {
            get { return m_fileExt; }
        }

        public String FileDescription
        {
            get { return m_fileExt; }
        }

        public String LastError
        {
            get { return m_lastError; }
        }

    }
}
