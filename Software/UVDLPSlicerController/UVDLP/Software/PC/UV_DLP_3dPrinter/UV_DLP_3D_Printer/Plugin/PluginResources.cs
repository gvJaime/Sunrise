/************************************************************************************
 * handle external resources for lite plugins
 ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using UV_DLP_3D_Printer.GUI.CustomGUI;



namespace UV_DLP_3D_Printer.Plugin
{
    public class PluginInfo
    {
        public GuiParam<int> vendorID;
        public GuiParam<int> productID;
        public GuiParam<string> vendorName;
        public GuiParam<string> productName;
        public GuiParam<string> description;
        public GuiParam<string> version;
        public GuiParam<string> icon;
        public GuiParam<string> splashScreenImage;
        public GuiParam<string> aboutImage;
        public GuiParam<string> guiConfig;
        public GuiParam<string> guiLayout;

        public PluginInfo()
        {
            vendorID = new GuiParam<int>(0x1000);
            productID = new GuiParam<int>();
            vendorName = new GuiParam<string>("Envision Labs");
            productName = new GuiParam<string>("Envision Labs");
            description = new GuiParam<string>("Envision Labs Plugin");
            version = new GuiParam<string>("1.0.0.1");
            guiConfig = new GuiParam<string>("GuiConfig.xml");
            guiLayout = new GuiParam<string>();
        }

        public void Load(XmlNode xnode)
        {
            vendorID = GuiConfigDB.GetIntParam(xnode, "vendorid", 0x1000);
            productID = GuiConfigDB.GetIntParam(xnode, "productid", new GuiParam<int>());
            vendorName = GuiConfigDB.GetStrParam(xnode, "vendorname", "Envision Labs");
            productName = GuiConfigDB.GetStrParam(xnode, "productname", "Envision Labs");
            description = GuiConfigDB.GetStrParam(xnode, "description", "Envision Labs Plugin");
            version = GuiConfigDB.GetStrParam(xnode, "version", "1.0.0.1");
            guiConfig = GuiConfigDB.GetStrParam(xnode, "guiconfig", "GuiConfig.xml");
            guiLayout = GuiConfigDB.GetStrParam(xnode, "guilayout", new GuiParam<string>());
        }

        public void Save(XmlDocument xd, XmlNode xnode)
        {
            vendorID.Save(xd, xnode, "vendorid");
            productID.Save(xd, xnode, "productid");
            vendorName.Save(xd, xnode, "vendorname");
            productName.Save(xd, xnode, "productname");
            description.Save(xd, xnode, "description");
            version.Save(xd, xnode, "version");
            guiConfig.Save(xd, xnode, "guiconfig");
            guiLayout.Save(xd, xnode, "guilayout");
        }
    }

    public class PluginResources
    {
        const int FILE_VERSION = 1;

        List<string> mSliceConfigFiles;
        List<string> mMachineConfigFiles;
        List<string> mImageFiles;
        List<string> mPluginGeneral;
        public PluginInfo PluginInfo;

        public PluginResources()
        {
            mSliceConfigFiles = new List<string>();
            mMachineConfigFiles = new List<string>();
            mImageFiles = new List<string>();
            mPluginGeneral = new List<string>();
            mPluginGeneral.Add("PluginInfo");
            PluginInfo = new PluginInfo();
            
        }

        public List<string> PluginInfoItems
        {
            get { return mPluginGeneral; }
        }

        public List<string> SliceConfigFiles
        {
            get { return mSliceConfigFiles; }
            set { mSliceConfigFiles = value; }
        }

        public List<string> MachineConfigFiles
        {
            get { return mMachineConfigFiles; }
            set { mMachineConfigFiles = value; }
        }

        public List<string> ImageFiles
        {
            get { return mImageFiles; }
            set { mImageFiles = value; }
        }

        #region manifest parsing
        public void ParseManifest(MemoryStream stream)
        {
            XmlDocument xd = new XmlDocument();
            stream.Seek(0, SeekOrigin.Begin);
            xd.Load(stream);
            XmlNode rootnode = xd.ChildNodes[1];
            if (rootnode.Name != "CWPluginManifest")
                return;

            foreach (XmlNode xnode in rootnode.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "PluginInfo": PluginInfo.Load(xnode); break;
                    case "UserImages":
                        LoadFileList(xnode, "Image", mImageFiles);
                        break;

                    case "MachineConfigs":
                        LoadFileList(xnode, "MachineConfig", mMachineConfigFiles);
                        break;

                    case "SliceProfile":
                        LoadFileList(xnode, "SliceProfile", mSliceConfigFiles);
                        break;

                }
            }
        }

        void LoadFileList(XmlNode parentnode, string name, List<string> fileList)
        {
            foreach (XmlNode xnode in parentnode.ChildNodes)
            {
                if (xnode.Name == name)
                {
                    try
                    {
                        fileList.Add(xnode.Attributes["filename"].Value);
                    }
                    catch {}
                }
            }
        }

        #endregion



        #region manifest generation
        public void SaveManifest(MemoryStream stream)
        {
            XmlDocument xd = new XmlDocument();
            xd.AppendChild(xd.CreateXmlDeclaration("1.0", "utf-8", ""));
            XmlNode toplevel = xd.CreateElement("CWPluginManifest");
            XmlAttribute verattr = xd.CreateAttribute("FileVersion");
            verattr.Value = FILE_VERSION.ToString();
            toplevel.Attributes.Append(verattr);
            xd.AppendChild(toplevel);
            SaveInfo(xd, toplevel);
            SaveFileList(xd, toplevel, "UserImages", "Image", mImageFiles);
            SaveFileList(xd, toplevel, "MachineConfigs", "MachineConfig", mMachineConfigFiles);
            SaveFileList(xd, toplevel, "SliceProfile", "SliceProfile", mSliceConfigFiles);
            xd.Save(stream);
        }

        void SaveInfo(XmlDocument xd, XmlNode parentnode)
        {
            XmlNode xnode = xd.CreateElement("PluginInfo");
            PluginInfo.Save(xd, xnode);
            parentnode.AppendChild(xnode);
        }

        void SaveFileList(XmlDocument xd, XmlNode parentnode, string title, string item, List<string> fileList)
        {
            XmlNode xnode = xd.CreateElement(title);
            foreach (string imgfile in fileList)
            {
                XmlNode imgnode = xd.CreateElement(item);
                XmlAttribute fileattr = xd.CreateAttribute("filename");
                fileattr.Value = imgfile;
                imgnode.Attributes.Append(fileattr);
                xnode.AppendChild(imgnode);
            }
            parentnode.AppendChild(xnode);
        }

        #endregion
    }
}
