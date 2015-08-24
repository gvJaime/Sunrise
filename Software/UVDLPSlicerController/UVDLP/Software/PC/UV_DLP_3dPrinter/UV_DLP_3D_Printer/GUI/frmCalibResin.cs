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
    public partial class frmCalibResin : Form
    {
        SliceBuildConfig m_config;
        TextBox[] m_modelNumbers;
        public frmCalibResin()
        {
            InitializeComponent();
            m_modelNumbers = new TextBox[] { txtThk00, txtThk01, txtThk02, txtThk03, txtThk04, txtThk05, txtThk06, txtThk07, txtThk08, txtThk09 };
            foreach (TextBox tb in m_modelNumbers)
            {
                tb.ReadOnly = true;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            /*string shortname = UVDLPApp.Instance().GetCurrentSliceProfileName();
            string fname = UVDLPApp.Instance().m_PathProfiles + UVDLPApp.m_pathsep + shortname + ".slicing";
            UVDLPApp.Instance().LoadBuildSliceProfile(fname);*/
            m_config = UVDLPApp.Instance().m_buildparms;
            m_config.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we've got the correct display size and PixPerMM values
            txtMinExpose.Text = m_config.minExposure.ToString();
            txtStepExpose.Text = m_config.exposureStep.ToString();
            FillPattern();
        }

        void UpdateIntField(TextBox tb, ref int field)
        {
            try
            {
                field = int.Parse(tb.Text);
            }
            catch
            {
                tb.Text = field.ToString();
            }
        }

        void FillPattern()
        {
            int et = m_config.minExposure;
            for (int i = 0; i < m_modelNumbers.Length; i++)
            {
                m_modelNumbers[i].Text = et.ToString();
                et += m_config.exposureStep;
            }
        }

        private void buttClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttPrint_Click(object sender, EventArgs e)
        {
            UpdateIntField(txtMinExpose, ref m_config.minExposure);
            UpdateIntField(txtStepExpose, ref m_config.exposureStep);
            FillPattern();
            buttPrint.Enabled = false;
            Cursor = Cursors.WaitCursor;
            Update();
            UVDLPApp.Instance().m_slicefile = UVDLPApp.Instance().m_slicer.Slice(m_config, Slicing.SliceFile.ModelType.eResinTest1); // start slicing the test model
            DialogResult dr = MessageBox.Show("Test model generation completed.\nClose the calibration form and press PLAY button to print.", "Test model generation");
            buttPrint.Enabled = true;
            Cursor = Cursors.Default;
        }
    }
}
