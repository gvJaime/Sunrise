using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.GUI.Controls.ManualControls
{
    public partial class ctlMainManual : UserControl
    {
        public ctlMainManual()
        {
            InitializeComponent();
            UVDLPApp.Instance().m_gui_config.AddControl("ctlManualControl", ctlManualControl1);
        }
    }
}
