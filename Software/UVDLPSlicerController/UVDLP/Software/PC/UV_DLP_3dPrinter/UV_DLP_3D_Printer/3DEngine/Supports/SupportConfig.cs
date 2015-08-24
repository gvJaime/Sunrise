using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;


namespace UV_DLP_3D_Printer.Configs
{
    /// <summary>
    /// This is a class for holding configuartion to generate
    /// Automatic or manual support structures.
    /// It also holds the parameters about the individual supports that are created
    /// In the future, this needs to be split into 2 classes:
    /// 1 - The automatic support generation configuration
    /// 2 - the individual support configuration
    /// 
    /// </summary>
[Serializable]
    public class SupportConfig
    {
        public enum eAUTOSUPPORTTYPE  // this is for the automatic support generation 
        {
            eBON, // bed of nails
            eADAPTIVE, // a tree-like structure
            //eADAPTIVE2, // a tree-like structure
        }

        public enum eCrossSectionShape
        {
            eCircle, // generate points around a circle
            ePlus // use the list of points to define an 'X' or '+' shape for each cross-section
        }

        public const int FILE_VERSION = 1; // this should change every time the format changes
        public double xspace, yspace;
        public double mingap; // minimum gap between adaptively generated supports
        public double htrad; // head top radius 
        public double hbrad; // head bottom radius
        public double ftrad; // foot top radius
        public double fbrad; // foot bottom radius
        public double fbrad2; // foot bottom radius 2
        public double downwardAngle;
        public int vdivs; // vertical divisions, not used
        public int cdivs; // circular divisions - used for cylinderical supports
        public bool m_onlydownward; // generate supports only on the downward facing polygons in the scene / object
        public bool m_onlyselected;
        public eAUTOSUPPORTTYPE eSupType;
        public eCrossSectionShape eSectionShape;

        public SupportConfig() 
        {
            eSupType = eAUTOSUPPORTTYPE.eBON;
            xspace = 5.0; // 5 mm spacing
            yspace = 5.0; // 5 mm spacing
            mingap = 5.0; // 5 mm spacing
            htrad = .2;//
            hbrad = .5; //
            ftrad = .5;
            fbrad = 2; // for support on the platform
            fbrad2 = .2; // for intra-object support
            //vdivs = 1; // divisions vertically
            m_onlydownward = false;
            m_onlyselected = false;
            downwardAngle = 45;
            cdivs = 11; // a prime number
            eSectionShape = eCrossSectionShape.eCircle;
        }

        public SupportConfig Clone() 
        {
            SupportConfig sc = new SupportConfig();
            sc.eSupType = eSupType;
            sc.fbrad = fbrad;
            sc.fbrad2 = fbrad2;
            sc.ftrad = ftrad;
            sc.hbrad = hbrad;
            sc.htrad = htrad;
            sc.m_onlydownward = m_onlydownward;
            sc.m_onlyselected = m_onlyselected;
            sc.downwardAngle = downwardAngle;
            sc.mingap = mingap;
            sc.vdivs = vdivs;
            sc.xspace = xspace;
            sc.yspace = yspace;
            sc.eSectionShape = eSectionShape;
            return sc;
        }
        public void Load(String filename)
        {
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(filename, "SupportConfig");
            XmlNode sc = xh.m_toplevel;

            xspace = xh.GetDouble(sc, "XSpace", 5.0);
            yspace = xh.GetDouble(sc, "YSpace", 5.0);
            mingap = xh.GetDouble(sc, "MinAdaptiveGap", 5.0);
            htrad = xh.GetDouble(sc, "HeadTopRadiusMM", 0.2);
            hbrad = xh.GetDouble(sc, "HeadBottomRadiusMM", 0.5);
            ftrad = xh.GetDouble(sc, "FootTopRadiusMM", 0.5);
            fbrad = xh.GetDouble(sc, "FootBottomRadiusMM", 2.0);
            fbrad2 = xh.GetDouble(sc, "FootBottomIntraRadiusMM", 0.2);
            downwardAngle = xh.GetDouble(sc, "DownwardAngle", 45.0);
            m_onlydownward = xh.GetBool(sc, "GenerateOnDownward", false);

            if (!fileExist)
            {
                xh.Save(FILE_VERSION);
            }
        }

        public void Save(String filename)
        {
            XmlHelper xh = new XmlHelper();
            xh.Start(filename, "SupportConfig");
            XmlNode sc = xh.m_toplevel;
            xh.SetParameter(sc, "XSpace", xspace);
            xh.SetParameter(sc, "YSpace", yspace);
            xh.SetParameter(sc, "MinAdaptiveGap", mingap);
            xh.SetParameter(sc, "HeadTopRadiusMM", htrad);
            xh.SetParameter(sc, "HeadBottomRadiusMM", hbrad);
            xh.SetParameter(sc, "FootTopRadiusMM", ftrad);
            xh.SetParameter(sc, "FootBottomRadiusMM", fbrad);
            xh.SetParameter(sc, "FootBottomIntraRadiusMM", fbrad2);
            xh.SetParameter(sc, "DownwardAngle", downwardAngle);
            xh.SetParameter(sc, "GenerateOnDownward", m_onlydownward);
            xh.Save(FILE_VERSION);
        }

    }
}
