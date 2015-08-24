using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class sMiiCtrl : UserControl
    {
        public class miicom 
        {
            public char command;
            public string description;
            public miicom(char com, string desc) 
            {
                command = com;
                description = desc;
            }
        }

        private static miicom[] commands = 
        {
            new miicom('a',"Z Table Up"),
            new miicom('b',"Z Table Down"),
            new miicom('u',"Z Table Check"),

            new miicom('v',"micro step normal 30 micro"),
            new miicom('x',"micro step normal 50 micro"),
            new miicom('y',"micro step normal 70 micro"),
            new miicom('z',"micro step normal 100 micro"),
            new miicom('t',"Stop current action (break)"),

            //new miicom('d',"micro step normal 100 micro"),
           // new miicom('e',"micro step slow 50 micro"),
           // new miicom('f',"micro step slow 100 micro"),
            new miicom('g',"Pump resin"),
            new miicom('h',"Stop Pumping Resin"),
            new miicom('i',"Redraw Resin"),
            new miicom('k',"UV Motor On"),
            new miicom('l',"UV Motor Off"),
            new miicom('m',"Pico Projector On"),
            new miicom('n',"Pico Projector Off"),
            new miicom('p',"Fluorescent UV On"),
            new miicom('q',"Fluorescent UV Off"),
            new miicom('r',"LED UV On"),
            new miicom('s',"LED UV Off")
        };

        /*
     case 'm'://pico projector on (for test)
     case 'n'://pico projector on (for test)                              
     case 'p':                             //fluorescent UV on      
     case 'q':                             //fluorescent UV off
     case 'r':                             //LED UV on
     case 's':                             //LED UV off
         
         */
        public sMiiCtrl()
        {
            InitializeComponent();
            SetupCommands();
        }
        private void SetupCommands()
        {
            lbMiiCommands.Items.Clear();
            foreach (miicom mc in commands) 
            {
                lbMiiCommands.Items.Add(mc.command.ToString() + " = " + mc.description);
            }
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            //get the selected item index
            int idx = lbMiiCommands.SelectedIndex;
            if (idx == -1) return;
            miicom mc = commands[idx];
            //send to the printer
            string txt = "M800 T0 " + mc.command.ToString() + "\r\n";
            UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(txt);
        }

    }
}
