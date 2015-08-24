using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace plugTest
{
    // this is a test class to show that we can load a control from another dll
    // we can even use a class derived from ctlUserPanel defined in Creation Workshop
    public partial class ctlTestControl : ctlUserPanel
    {
        public ctlTestControl()
        {
            InitializeComponent();
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            lbl1.Text = "Button Clicked";
        }

        private void ctlTestControl_Load(object sender, EventArgs e)
        {

        }
    }
}
