using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine3D;
namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmMeshHoles : Form
    {
        public frmMeshHoles()
        {
            InitializeComponent();
        }

        private void cmdCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (UVDLPApp.Instance().SelectedObject == null)
                    return;
                List<Polygon> holes = UVDLPApp.Instance().SelectedObject.FindHoles();
                if (holes.Count > 0)
                {
                    lblReport.Text = "Mesh has holes: # holes = " + holes.Count;
                }
                else
                {
                    lblReport.Text = "No holes detected";
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
    }
}
