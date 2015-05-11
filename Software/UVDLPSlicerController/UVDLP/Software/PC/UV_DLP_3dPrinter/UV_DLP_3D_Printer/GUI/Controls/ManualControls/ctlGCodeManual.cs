using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using UV_DLP_3D_Printer.Device_Interface;
using UV_DLP_3D_Printer.Drivers;

namespace UV_DLP_3D_Printer.GUI.Controls.ManualControls
{
    public partial class ctlGCodeManual : ctlRFrame
    {
        delegate void LogGcodeCallback(object sender, object vars);

        public ctlGCodeManual()
        {
            InitializeComponent();
            // the data received should be dis-abled during prints and re-enabled when not printing
            UVDLPApp.Instance().m_deviceinterface.LineDataEvent += new DeviceInterface.DeviceLineReceived(LineDataReceived);
            UVDLPApp.Instance().m_deviceinterface.DataEvent += new DeviceInterface.DeviceDataReceived(datareceivedev);
            UVDLPApp.Instance().m_buildmgr.BuildStatus += new delBuildStatus(BuildStatus);
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("cmdLogGcode", LogGcode, typeof(string), "Log all commands sent to machine");
        }
        void datareceivedev(DeviceDriver device, byte[] dat, int len) 
        {
            string str = Utility.ByteArrayToString(dat);
            DebugLogger.Instance().LogInfo(str);
        }

        private void cmdSendGCode_Click(object sender, EventArgs e)
        {
            try
            {
                if (UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(txtGCode.Text + "\r\n"))
                {
                    if (!checkLogAll.Checked)
                        txtSent.Text = txtGCode.Text + "\r\n" + txtSent.Text;
                }
                else
                {
                    DebugLogger.Instance().LogRecord("Could Not Send Raw GCode Command");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }

        void LogGcode(object sender, object vars)
        {
            if (this.txtSent.InvokeRequired)
            {
                LogGcodeCallback d = new LogGcodeCallback(LogGcode);
                this.Invoke(d, new object[] { sender, vars });
            }
            else
            {
                string txt = (String)vars;
                if (checkLogAll.Checked && (txt[0] != ';'))
                    txtSent.Text = txt + txtSent.Text;
            }
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            txtReceived.Text = "";
        }
        /// <summary>
        /// This will re-enable the ability to see sent GCode commands
        /// </summary>
        public void BuildStopped()
        {
            UVDLPApp.Instance().m_deviceinterface.LineDataEvent += new DeviceInterface.DeviceLineReceived(LineDataReceived);
        }
        /// <summary>
        /// When the build is started, we need to stop listening in on GCode commands,
        /// otherwise this control will be flooded with GCode messages
        /// it's trying to display, and will become un-responsive
        /// </summary>
        public void BuildStarted()
        {
            UVDLPApp.Instance().m_deviceinterface.LineDataEvent -= new DeviceInterface.DeviceLineReceived(LineDataReceived);
        }

        void BuildStatus(eBuildStatus printstat, string mess, int data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { BuildStatus(printstat, mess, data); }));
            }
            else
            {        
                switch (printstat)
                {
                    case eBuildStatus.eBuildCancelled:
                        BuildStopped();

                        break;
                    case eBuildStatus.eBuildCompleted:
                        try
                        {
                            BuildStopped();
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.Instance().LogError(ex.Message);
                        }
                        break;
                    case eBuildStatus.eBuildStarted:
                        BuildStarted();
                        break;
                }
            }
        }
        /*
        public override void ApplyStyle(ControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor != ControlStyle.NullColor)
            {
                label1.ForeColor = ct.ForeColor;
                label2.ForeColor = ct.ForeColor;
                lblTitle.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor != ControlStyle.NullColor)
            {
                BackColor = ct.BackColor;
            }
            if (ct.FrameColor != ControlStyle.NullColor)
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel5.BackColor = ct.FrameColor;
                lblTitle.BackColor = ct.FrameColor;
            }

        }
        */
        void LineDataReceived(DeviceDriver driver, string line)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { LineDataReceived(driver, line); }));
            }
            else
            {
                line = line.Trim();
                //sb.Insert(0, line);
                txtReceived.Text += line + "\r\n";
                //txtReceived.Refresh();
            }

        }

    }
}
