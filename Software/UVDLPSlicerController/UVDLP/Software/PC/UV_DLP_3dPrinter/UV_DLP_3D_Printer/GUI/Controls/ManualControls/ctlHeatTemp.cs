using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using UV_DLP_3D_Printer.Drivers;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.GUI.Controls
{   
     

    /// <summary>
    /// This is the event that fires when the controls' set/off buttons are clicked
    /// </summary>

    public partial class ctlHeatTemp : ctlUserPanel
    {
   
        protected System.Timers.Timer m_tempchecktimer;
        private const int DEF_TEMPCHECK = 3000;// 3 seconds
        public ctlHeatTemp()
        {
            InitializeComponent();
            m_tempchecktimer = new System.Timers.Timer();
            m_tempchecktimer.Elapsed += new ElapsedEventHandler(m_tempchecktimer_Elapsed);
            m_tempchecktimer.Interval = DEF_TEMPCHECK;
            UVDLPApp.Instance().m_deviceinterface.LineDataEvent += new DeviceInterface.DeviceLineReceived(LineReceived);
        }
   
        //we're going to need historical values of these as well to graph
        private double HBP_Temp;
        private double EXT0_Temp;
        public bool MonitorTemps
        {
            get
            {
                return false;

            }
            set
            {
                if (value == true)
                {
                    m_tempchecktimer.Start();
                }
                else
                {
                    m_tempchecktimer.Stop();
                }
            }
        }

        public void LineReceived(DeviceDriver device, string line) 
        {
            // parse for temperature data
            //convert to string
            string ln = line.Trim().ToUpper();

            //ok T:201 B:117 
            String[] parts = line.Split(' '); // split on spaces
            bool temp = false;
            if (parts.Length > 1)  // if this is more than just an 'ok'
            {
                foreach (string part in parts)
                {
                    try
                    {
                        //part = part.ToUpper(); 

                        if (part.StartsWith("T:"))
                        {
                            string[] tmp = part.Split(':');
                            EXT0_Temp = double.Parse(tmp[1]);
                            temp = true;
                        }
                        if (part.StartsWith("B:"))
                        {
                            string[] tmp = part.Split(':');
                            HBP_Temp = double.Parse(tmp[1]);
                            temp = true;
                        }
                        if (temp) 
                        {
                            //update the gui
                            UpdateForm();
                        }
                    }
                    catch (Exception ex) 
                    {
                        DebugLogger.Instance().LogError(ex.Message);
                    }
                }
            }

        }

        void m_tempchecktimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //throw new NotImplementedException();
            //send a gcode to query temps
            UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice("M105\r\n");
        }

        /*
        public void SetHBPTemp(double degC) 
        {
            HBP_Temp = degC;
            UpdateForm();
        }
        public void SetEXT0Temp(double degC)
        {
            EXT0_Temp = degC;
            UpdateForm();
        }
         * */
        private void UpdateForm() 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { UpdateForm(); }));
            }
            else
            {
                // update the visual controls
                lblHBP.Text = "" + HBP_Temp + " C";
                lblEXT0.Text = "" + EXT0_Temp + " C";
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmdOff1_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice("M104 S0\r\n");
        }

        private void cmdOff2_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice("M140 S0\r\n");
        }
        /// <summary>
        /// HBP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSet2_Click(object sender, EventArgs e)
        {
            try
            {
                int celsius = int.Parse(txtVal2.Text);

                string cmd = "M140 S" + celsius + "\r\n";
                UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(cmd);
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Ext 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSet1_Click(object sender, EventArgs e)
        {
            try
            {
                int temp = int.Parse(txtVal1.Text);
                string cmd = "M104 S" + temp + "\r\n";
                UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(cmd);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkMonitorTemps_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMonitorTemps.Checked)
            {
                MonitorTemps = true;
            }
            else 
            {
                MonitorTemps = false;
            }
        }
    }
}
