using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer._3DEngine
{
    /// <summary>
    /// This class is a helper class to deal with creating / loading and saving of the scene manifest
    /// </summary>
    public class SceneManifest
    {

        public SceneManifest() 
        {
        
        }

        public void CreateFromStream(Stream stream) 
        {
            XmlHelper manifest = new XmlHelper();
        }

    }
}
