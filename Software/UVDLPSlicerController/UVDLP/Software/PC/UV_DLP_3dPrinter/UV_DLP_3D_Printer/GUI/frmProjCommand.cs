using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmProjCommand : Form
    {
        private ProjectorCommand m_pc;
        public frmProjCommand()
        {
            InitializeComponent();
            m_pc = null;
            SetData();
            DisplayCommandList();
        }

        private void SetData() 
        {
            if (m_pc == null) return;
            chkHex.Checked = m_pc.hex;
            txtCommand.Text = m_pc.command;
            txtName.Text = m_pc.name;
        }
        private bool GetData() 
        {
            if (m_pc == null) return false;
            try
            {
                string ts = txtCommand.Text;
                ts = ts.Replace(" ", string.Empty);
                byte[] tmp = Utility.HexStringToByteArray(ts);
                if (tmp == null) return false;
                m_pc.hex = chkHex.Checked;                
                m_pc.command = txtCommand.Text;
                m_pc.name = txtName.Text; 
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (GetData()) 
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void lbCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the name of the 
            if (lbCommands.SelectedIndex == -1)
            {
                cmdDelete.Enabled = false;
                return;
            }
            else 
            {
                cmdDelete.Enabled = true;
            }
            string name = lbCommands.SelectedItem.ToString();
            foreach (ProjectorCommand cmd in UVDLPApp.Instance().m_proj_cmd_lst.m_commands) 
            {
                if (cmd.name.Equals(name)) 
                {
                    m_pc = cmd;
                    SetData();
                }
            }
        }
        private void DisplayCommandList() 
        {
            lbCommands.Items.Clear();
            foreach (ProjectorCommand cmd in UVDLPApp.Instance().m_proj_cmd_lst.m_commands) 
            {
                lbCommands.Items.Add(cmd.name);
            }
        }
        private void cmdAdd_Click(object sender, EventArgs e)
        {
            //prjcmdlst 
            ProjectorCommand cmd = new ProjectorCommand();
            UVDLPApp.Instance().m_proj_cmd_lst.m_commands.Add(cmd);
            DisplayCommandList();
            UVDLPApp.Instance().SaveProjectorCommands(UVDLPApp.Instance().m_appconfig.ProjectorCommandsFile);
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (!GetData())
            {
                MessageBox.Show("Please Check Input");
            }
            else
            {
                DisplayCommandList();
                UVDLPApp.Instance().SaveProjectorCommands(UVDLPApp.Instance().m_appconfig.ProjectorCommandsFile);
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (lbCommands.SelectedIndex == -1) return;
            ProjectorCommand cmd =  UVDLPApp.Instance().m_proj_cmd_lst.m_commands[lbCommands.SelectedIndex];
            UVDLPApp.Instance().m_proj_cmd_lst.m_commands.Remove(cmd);
            DisplayCommandList();
            UVDLPApp.Instance().SaveProjectorCommands(UVDLPApp.Instance().m_appconfig.ProjectorCommandsFile);
        }

    }
}
