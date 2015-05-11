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
    public partial class frmBuildSizeCalib : Form
    {
        float platoformsizeX, platoformsizeY;
        public float calcplatoformsizeX, calcplatoformsizeY;
        float modelsizeX, modelsizeY;
        float measuredmodelsizeX, measuredmodelsizeY;
        public void setPlatformSize(float x, float y)
        {
            platoformsizeX = x;
            platoformsizeY = y;
            SetData();
            CalcNewSize();
        }
        public void setModelSize(float x, float y)
        {
            modelsizeX = x;
            modelsizeY = y;
            SetData();
            CalcNewSize();
        }
        public frmBuildSizeCalib()
        {
            InitializeComponent();
            CalcNewSize();
            SetData();
        }
       
        private void CalcNewSize() 
        {
            try
            {
                GetData(); // get the current data
                //make some calculations
                //calcplatoformsizeX = measuredmodelsizeX / platoformsizeX;
                // scale is measuredsize / modelsize
                // scale is modelsize / measuredsize
                //                float scaleX = modelsizeX / measuredmodelsizeX;
                //                float scaleY = modelsizeY / measuredmodelsizeY;
                
                float scaleX = measuredmodelsizeX / modelsizeX;
                float scaleY = measuredmodelsizeY / modelsizeY;
                calcplatoformsizeX = scaleX * platoformsizeX;
                calcplatoformsizeY = scaleY * platoformsizeY;
                SetData();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void SetData()
        {
            lblBuildSizeX.Text = platoformsizeX.ToString();
            lblBuildSizeY.Text = platoformsizeY.ToString();
            lblNewBuildSizeX.Text = calcplatoformsizeX.ToString();
            lblNewBuildSizeY.Text = calcplatoformsizeY.ToString();
        }
        private void GetData() 
        {
            try
            {
                measuredmodelsizeX = float.Parse(txtmeasuredx.Text);
                measuredmodelsizeY = float.Parse(txtmeasuredy.Text);
                modelsizeX = float.Parse(txtmodelx.Text);
                modelsizeY = float.Parse(txtmodely.Text);
            }
            catch (Exception ) 
            {
                //DebugLogger.Instance().LogError(ex);
            }
        }
        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            GetData();
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void txtmeasuredx_TextChanged(object sender, EventArgs e)
        {
            CalcNewSize();
        }

        private void txtmeasuredy_TextChanged(object sender, EventArgs e)
        {
            CalcNewSize();

        }

        private void txtmodelx_TextChanged(object sender, EventArgs e)
        {
            CalcNewSize();
        }

        private void txtmodely_TextChanged(object sender, EventArgs e)
        {
            CalcNewSize();
        }
    }
}
