using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer.GUI.CustomGUI;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlUserParamEdit : UserControl
    {
        UserParameterList m_parms;
        public ctlUserParamEdit()
        {
            InitializeComponent();
        }
        public void Setup(UserParameterList parms) 
        {
            try
            {
                m_parms = parms;
                lvParams.Items.Clear();
                //parms.paramDict.
                foreach (KeyValuePair<string, CWParameter> parm in parms.paramDict)
                {
                    CWParameter cwp = parm.Value;
                    ListViewItem lvi = lvParams.Items.Add(parm.Key);
                    if(cwp.ParamType == typeof(string))
                    {
                        GuiParam<string> dat = (GuiParam<string>)cwp;
                        lvi.SubItems.Add(dat.GetVal());
                    }
                    //GuiParam prm
                                     
                }
                lvParams.Refresh();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void cmdDelParam_Click(object sender, EventArgs e)
        {
            try
            {
                //get selection out of lvParams
                ListViewItem lvi = lvParams.SelectedItems[0];
                string key = lvi.SubItems[0].Text;
                m_parms.paramDict[key] = null;
                Setup(m_parms);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void cmdNewParam_Click(object sender, EventArgs e)
        {
            CWParameter cwp = m_parms.GetParameter(txtName.Text, txtValue.Text);
            if (cwp.ParamType == typeof(string))
            {
                GuiParam<string> dat = (GuiParam<string>)cwp;
                dat.SetValue(txtValue.Text);
                dat.SetDefault(txtValue.Text);
            }
            Setup(m_parms);
        }
        /*
        private void cmdApply_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi = lvParams.SelectedItems[0];
                string key = lvi.SubItems[0].Text;
                CWParameter parm = m_parms.paramDict[key];
                if (parm != null)
                {
                    parm.paramName = txtValue.Text;
                }
                Setup(m_parms);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        */
        private void lvParams_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi = lvParams.SelectedItems[0];
                string key = lvi.SubItems[0].Text;
                CWParameter parm = m_parms.paramDict[key];
                if (parm != null)
                {
                    txtName.Text = key;
                    //txtValue.Text = parm.paramName;
                    if (parm.ParamType == typeof(string))
                    {
                        GuiParam<string> dat = (GuiParam<string>)parm;
                        txtValue.Text = dat.GetVal();
                    }
                }
            }
            catch (Exception ex) 
            {
                txtValue.Text = "";
                txtName.Text = "";
                //DebugLogger.Instance().LogError(ex);
            }
        }
    }
}
