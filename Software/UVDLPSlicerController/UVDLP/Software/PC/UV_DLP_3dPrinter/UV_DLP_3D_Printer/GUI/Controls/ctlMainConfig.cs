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
    public partial class ctlMainConfig : ctlUserPanel
    {
        private enum eConfView 
        {
            eSlice,
            eMachine,
            eNone
        }
        private eConfView m_eView;

        public ctlMainConfig()
        {
            InitializeComponent();            
            RegisterCallbacks();
            HideControls();
            m_eView = eConfView.eNone;
            ClickViewConfMachine(null, null);
            UVDLPApp.Instance().m_gui_config.AddControl("guimachineconfig", ctlMachineConfig1);
            UVDLPApp.Instance().m_gui_config.AddControl("guimachineconfigparent", pnlMachineConfig);
            //UVDLPApp.Instance().m_gui_config.AddControl("ctlSliceProfileConfig", ctlSliceProfileConfig);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlToolpathGenConfig1", ctlToolpathGenConfig1); // the slice profile settings
            UVDLPApp.Instance().m_gui_config.AddControl("pnlToolpathGenConfig", pnlToolpathGenConfig); // the slice profile settings parent
            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitleMachineConfig", ctlMachineConfigView);
            UVDLPApp.Instance().m_gui_config.AddControl("ctlTitleSliceProfile", ctlSliceProfileConfig);
            
            //
            
        }
        private void HideControls() 
        {
            /*
            ctlMachineConfig1.Dock = DockStyle.None;
            ctlMachineConfig1.Visible = false;

            ctlToolpathGenConfig1.Dock = DockStyle.None;
            ctlToolpathGenConfig1.Visible = false;
            */
            pnlToolpathGenConfig.Dock = DockStyle.None;
            pnlToolpathGenConfig.Visible = false;

            pnlMachineConfig.Dock = DockStyle.None;
            pnlMachineConfig.Visible = false;
        }
        private void SetupView(eConfView view) 
        {
            if (m_eView == view) return;
            HideControls();
            m_eView = view;
            switch (m_eView)
            {
                case eConfView.eSlice:
                    pnlToolpathGenConfig.Dock = DockStyle.Fill;
                    pnlToolpathGenConfig.Visible = true;
                    break;
                case eConfView.eMachine:
                    pnlMachineConfig.Dock = DockStyle.Fill;
                    pnlMachineConfig.Visible = true;
                    break;
            }
        }
        private void RegisterCallbacks() 
        {
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickViewConfMachine", ClickViewConfMachine, null, "View Machine Configuration");
            UVDLPApp.Instance().m_callbackhandler.RegisterCallback("ClickViewSliceConfig", ClickViewSliceConfig, null, "View Slicing Configuration");
        
        }
        private void ClickViewConfMachine(object sender, object vars) 
        {
            SetupView(eConfView.eMachine);
            ctlMachineConfigView.Checked = true;
            ctlSliceProfileConfig.Checked = false;
            
        }
        private void ClickViewSliceConfig(object sender, object vars)
        {
            SetupView(eConfView.eSlice);
            ctlMachineConfigView.Checked = false;
            ctlSliceProfileConfig.Checked = true;
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.BackColor.IsValid())
                flowLayoutPanel1.BackColor = ct.BackColor;
            if (ct.ForeColor.IsValid())
                flowLayoutPanel1.ForeColor = ct.ForeColor;
            //this.BackColor = Control.DefaultBackColor;
            //this.ForeColor = Control.DefaultForeColor;
            ctlMachineConfigView.ApplyStyle(ct);
            ctlSliceProfileConfig.ApplyStyle(ct);
            pnlMachineConfig.BackColor = Control.DefaultBackColor;
            pnlMachineConfig.ForeColor = Control.DefaultForeColor;
        }
    }
}