using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO.Ports;

namespace UV_DLP_3D_Printer.Configs
{
    public class ConnectionConfig
    {
        public String comname;
        public int speed;
        public int databits; // 8
        public Parity parity;
        public StopBits stopbits;
        public Handshake handshake;

        public ConnectionConfig()
        {

        }

        public void CreateDefault()
        {
            comname = "Com1";
            speed = 115200;
            databits = 8;
            parity = Parity.None;
            stopbits = StopBits.One;

        }
        public bool Load(XmlHelper xh, XmlNode parent)  // use new xml system -SHS
        {
            XmlNode cps = xh.FindSection(parent, "ComPortSettings");
            comname = xh.GetString(cps, "PortName", "Com1");
            speed = xh.GetInt(cps, "Speed", 115200);
            databits = xh.GetInt(cps, "Databits", 8);
            parity = (Parity)xh.GetEnum(cps, "Parity", typeof(Parity), Parity.None);
            stopbits = (StopBits)xh.GetEnum(cps, "Stopbits", typeof(StopBits), StopBits.One);
            handshake = (Handshake)xh.GetEnum(cps, "Handshake", typeof(Handshake), Handshake.None);
            return true;

        }

        public bool Save(XmlHelper xh, XmlNode parent) // use new xml system -SHS
        {
            XmlNode cps = xh.FindSection(parent, "ComPortSettings");
            xh.SetParameter(cps, "PortName", comname);
            xh.SetParameter(cps, "Speed", speed);
            xh.SetParameter(cps, "Databits", databits);
            xh.SetParameter(cps, "Parity", parity);
            xh.SetParameter(cps, "Stopbits", stopbits);
            xh.SetParameter(cps, "Handshake", handshake);
            return true;
        }
    }
}
