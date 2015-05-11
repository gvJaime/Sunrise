using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlSliceGCodePanel : ctlUserPanel
    {
        public ctlSliceGCodePanel()
        {
            InitializeComponent();
            RegisterCallbacks();
            ctlTitleViewSlice.Checked = true;
            ShowSliceView_Click(null,null);
        }

        public ctlSliceView ctlSliceViewctl
        {
            get {return ctlSliceView1;}
        }
            
        private void RegisterCallbacks()
        {
            // the main tab buttons
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ShowSliceView", ShowSliceView_Click, null, "View Slice display");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ShowGCodeView", ShowGCodeView_Click, null, "View GCode display");
        }
        private void ShowSliceView_Click(object sender, object vars) 
        {
            ctlSliceView1.BringToFront();
            ctlTitleViewGCode.Checked = false;
            ctlTitleViewSlice.Checked = true;
        }
        private void ShowGCodeView_Click(object sender, object vars)
        {
            ctlGcodeView1.BringToFront();
            ctlTitleViewSlice.Checked = false; // uncheck the other
            ctlTitleViewGCode.Checked = true;
        }
        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.BackColor.IsValid())
                flowLayoutPanel2.BackColor = ct.BackColor;
            if (ct.ForeColor.IsValid())
                flowLayoutPanel2.ForeColor = ct.ForeColor;
        }
    }
}
