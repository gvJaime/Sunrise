using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.ExportControls
{
    public partial class ctlExportB9Creator : UserControl
    {
        public ctlExportB9Creator()
        {
            InitializeComponent();
        }

        public string SceneName
        {
            get { return textName.Text; }
            set { textName.Text = value; }
        }

        public string Description
        {
            get { return textDescription.Text; }
            set { textDescription.Text = value; }
        }
    }
}
