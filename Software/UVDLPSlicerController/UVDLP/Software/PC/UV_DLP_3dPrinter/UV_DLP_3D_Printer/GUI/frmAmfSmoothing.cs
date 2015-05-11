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
    public partial class frmAmfSmoothing : Form
    {
        
        public frmAmfSmoothing()
        {
            InitializeComponent();
            comboSmooth.SelectedIndex = 2;
        }

        public int SmoothLevel
        {
            get { return comboSmooth.SelectedIndex; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
