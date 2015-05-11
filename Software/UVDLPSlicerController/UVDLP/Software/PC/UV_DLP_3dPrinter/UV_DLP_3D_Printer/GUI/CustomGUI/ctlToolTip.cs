using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

// just so all tooltips will look the same
namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class ctlToolTip : ToolTip
    {
        public ctlToolTip()
        {
            this.ForeColor = Color.Navy;
            this.BackColor = Color.Turquoise;
            this.InitialDelay = 1500;
        }

        
    }
}
