using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.IO;

namespace UV_DLP_3D_Printer.Configs
{
    public class MonitorConfig
    {
        /// <summary>
        /// Monitor rect, this is used to define what
        /// portion of the image this monitor will display
        /// This is a scaler value from 0-1
        /// This will allow multiple display devices to configure a much larger projected image
        /// </summary>
        public class MRect 
        {
            public float left, right, top, bottom;
            public MRect() 
            {
                left = 0.0f;
                right = 1.0f;
                top = 0.0f;
                bottom = 1.0f;
            }
            public float Width()
            {
                return right - left;
            }
            public float Height()
            {
                return bottom - top;
            }
        }

        public double m_XDLPRes; // the X resolution of the DLP projector in pixels
        public double m_YDLPRes; // the Y resolution of the DLP projector in pixels
        private double m_Xpixpermm; // the calculated pixels per mm
        private double m_Ypixpermm; // the calculated pixels per mm
        public Boolean m_displayconnectionenabled;   // projector comm enabled / disabled -SHS
        public ConnectionConfig m_displayconnection; // to the projector or similar
        private string m_monitorid; // which monitor we're using
        public MRect m_monitorrect; // the display portions of the larger image
        //brightness / color correction mask
        public string m_brightmask_filename; // a brightness / color correction mask
        public bool m_usemask;//
        public Bitmap m_mask; // the loaded mask (if any)        

        public MonitorConfig()
        {
            m_XDLPRes = 1024;
            m_YDLPRes = 768;
            m_Xpixpermm = 1.0;
            m_Ypixpermm = 1.0;
            m_displayconnectionenabled = false;  // -SHS
            m_displayconnection = new ConnectionConfig();
            m_monitorrect = new MRect();
            m_displayconnection.CreateDefault();
            m_monitorid = "";
            m_mask = null;
            m_usemask = false;
            m_brightmask_filename = "";
        }

        public bool Load(XmlHelper xh, XmlNode thisnode)
        {
            XmlNode mdc = thisnode;// xh.FindSection(parent, "MonitorDriverConfig");
            m_XDLPRes = xh.GetDouble(mdc, "DLP_X_Res", 1024.0);
            m_YDLPRes = xh.GetDouble(mdc, "DLP_Y_Res", 768.0);
            m_monitorid = xh.GetString(mdc, "MonitorID", "");
            m_displayconnectionenabled = xh.GetBool(mdc, "DisplayCommEnabled", false);
            m_displayconnection.Load(xh, mdc);
            m_monitorrect.top = (float)xh.GetDouble(mdc, "MonitorTop", 0.0);
            m_monitorrect.left = (float)xh.GetDouble(mdc, "MonitorLeft", 0.0);
            m_monitorrect.right = (float)xh.GetDouble(mdc, "MonitorRight", 1.0);
            m_monitorrect.bottom = (float)xh.GetDouble(mdc, "MonitorBottom", 1.0);
            m_brightmask_filename = xh.GetString(mdc, "CorrectionMask", "");
            m_usemask = xh.GetBool(mdc, "UseMask", false);
            if (m_usemask == true) 
            {
                try
                {
                    if (File.Exists(m_brightmask_filename))
                    {
                        m_mask = new Bitmap(m_brightmask_filename);
                    }
                    else 
                    {
                        DebugLogger.Instance().LogWarning("Mask Image " + m_brightmask_filename + " not found, disabling");
                        m_usemask = false;
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
            return true;
        }        

        public bool Save(XmlHelper xh, XmlNode parent) // use new xml system -SHS
        {
           // XmlNode mdc = xh.FindSection(parent, "MonitorDriverConfig");
            XmlNode mdc = xh.AddSection(parent, "MonitorDriverConfig");
            xh.SetParameter(mdc, "DLP_X_Res", m_XDLPRes); // gotta make this auto..
            xh.SetParameter(mdc, "DLP_Y_Res", m_YDLPRes);
            xh.SetParameter(mdc, "MonitorID", m_monitorid);
            xh.SetParameter(mdc, "DisplayCommEnabled", m_displayconnectionenabled);
            m_displayconnection.Save(xh, mdc);

            xh.SetParameter(mdc, "MonitorTop", m_monitorrect.top);
            xh.SetParameter(mdc, "MonitorLeft", m_monitorrect.left);
            xh.SetParameter(mdc, "MonitorRight", m_monitorrect.right);
            xh.SetParameter(mdc, "MonitorBottom", m_monitorrect.bottom);

            xh.SetParameter(mdc, "CorrectionMask", m_brightmask_filename);
            xh.SetParameter(mdc, "UseMask", m_usemask);
            return true;
        }

        public void CalcPixPerMM(double platX, double platY)
        {
            try
            {
                m_Xpixpermm = m_XDLPRes / platX;
                m_Ypixpermm = m_YDLPRes / platY;
            }
            catch (Exception)
            {
                DebugLogger.Instance().LogError("Invalid machine platform size");
                m_Xpixpermm = 1.0;
                m_Ypixpermm = 1.0;
            }
        }
        /*
        public void SetDLPRes(double xres, double yres)
        {
            m_XDLPRes = xres;
            m_YDLPRes = yres;
        }
        */
        public double PixPerMMX { get { return m_Xpixpermm; } }
        public double PixPerMMY { get { return m_Ypixpermm; } }
       // public int XRes { get { return (int)m_XDLPRes; } }
       // public int YRes { get { return (int)m_YDLPRes; } }

        public string Monitorid
        {
            get { return m_monitorid; }
            set { m_monitorid = value; }
        }

    }
}
