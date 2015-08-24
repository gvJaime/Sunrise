using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlScale : ctlUserPanel
    {
        public ctlScale()
        {
            InitializeComponent();
            //UVDLPApp.Instance().m_gui_config.AddButton("scale", ctlTitle1.Button); // get its style from control
        }
        public void FixForeColor(Color clr)
        {
            label5.ForeColor = clr;
            label6.ForeColor = clr;
            label7.ForeColor = clr;
            label12.ForeColor = clr;
        }
        protected void ScaleObject(ctlTextBox var, float x, float y, float z)
        {
            try
            {
                if (UVDLPApp.Instance().SelectedObject == null)
                    return;
                float val = var.FloatVal / 100f;
                x = (x == 0) ? 1 : x * val;
                y = (y == 0) ? 1 : y * val;
                z = (z == 0) ? 1 : z * val; 
                UVDLPApp.Instance().SelectedObject.Scale(x, y, z);
                UVDLPApp.Instance().m_undoer.SaveScale(UVDLPApp.Instance().SelectedObject, x, y, z);
                UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
                //ShowObjectInfo();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");

            }
            catch (Exception)
            {

            }
        }

        private void buttScaleAll_Click(object sender, EventArgs e)
        {
            ScaleObject(textScaleAll, 1, 1, 1);
        }

        private void buttScaleX_Click(object sender, EventArgs e)
        {
            ScaleObject(textScaleX, 1, 0, 0);
        }

        private void buttScaleY_Click(object sender, EventArgs e)
        {
            ScaleObject(textScaleY, 0, 1, 0);
        }

        private void buttScaleZ_Click(object sender, EventArgs e)
        {
            ScaleObject(textScaleZ, 0, 0, 1);
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                //labelManipType.ForeColor = ct.ForeColor;
                ctlTitle1.ForeColor = ct.ForeColor;
                label5.ForeColor = ct.ForeColor;
                label6.ForeColor = ct.ForeColor;
                label7.ForeColor = ct.ForeColor;
                label12.ForeColor = ct.ForeColor;
                textScaleX.ForeColor = ct.ForeColor;
                textScaleY.ForeColor = ct.ForeColor;
                textScaleZ.ForeColor = ct.ForeColor;
                textScaleAll.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                ctlTitle1.BackColor = ct.BackColor;
                manipObject.BackColor = ct.BackColor;
                textScaleX.BackColor = ct.BackColor;
                textScaleY.BackColor = ct.BackColor;
                textScaleZ.BackColor = ct.BackColor;
                textScaleAll.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel2.BackColor = ct.FrameColor;
                flowLayoutPanel3.BackColor = ct.FrameColor;
                flowLayoutPanel4.BackColor = ct.FrameColor;                
                flowLayoutPanel5.BackColor = ct.FrameColor;
            }

        }

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                //this.Height = 225 + 5;
                int h = ctlTitle1.Height + flowLayoutPanel1.Height;
                h += flowLayoutPanel2.Height;
                h += flowLayoutPanel3.Height;
                h += flowLayoutPanel4.Height;
                h += flowLayoutPanel5.Height;
                h += 3 * 7; // vertical margins
                this.Height = h;
            }
            else
            {
                this.Height = ctlTitle1.Height + 5;
            }
        }

        private void ctlScale_Resize(object sender, EventArgs e)
        {
            try
            {
                ctlTitle1.Width = ctlTitle1.Parent.Width - 6;
                flowLayoutPanel1.Width = flowLayoutPanel1.Parent.Width - 6;
                flowLayoutPanel2.Width = flowLayoutPanel2.Parent.Width - 6;
                flowLayoutPanel3.Width = flowLayoutPanel3.Parent.Width - 6;
                flowLayoutPanel4.Width = flowLayoutPanel4.Parent.Width - 6;
                flowLayoutPanel5.Width = flowLayoutPanel5.Parent.Width - 6;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttmm2inch_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            float scale = 25.4f;
            UVDLPApp.Instance().SelectedObject.Scale(scale, scale, scale);
            UVDLPApp.Instance().m_undoer.SaveScale(UVDLPApp.Instance().SelectedObject, scale, scale, scale);
            UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            //ShowObjectInfo();
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
        }

        private void buttinch2mm_Click(object sender, EventArgs e)
        {

            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            float scale = 1.0f/25.4f;
            UVDLPApp.Instance().SelectedObject.Scale(scale, scale, scale);
            UVDLPApp.Instance().m_undoer.SaveScale(UVDLPApp.Instance().SelectedObject, scale, scale, scale);
            UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            //ShowObjectInfo();
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");

        }

        private void buttAllMinus_Click(object sender, EventArgs e)
        {
            //get value from txtScaleAll
            try
            {
                float fval = float.Parse(textScaleAll.Text);
                fval -= 10.0f;
                textScaleAll.Text = fval.ToString();
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex);            
            }
        }

        private void buttAllPlus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleAll.Text);
                fval += 10.0f;
                textScaleAll.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }

        }

        private void buttXMinus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleX.Text);
                fval -= 10.0f;
                textScaleX.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }


        }

        private void buttXPlus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleX.Text);
                fval += 10.0f;
                textScaleX.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttYMinus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleY.Text);
                fval -= 10.0f;
                textScaleY.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttYPlus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleY.Text);
                fval += 10.0f;
                textScaleY.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttZMinus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleZ.Text);
                fval -= 10.0f;
                textScaleZ.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttZPlus_Click(object sender, EventArgs e)
        {
            try
            {
                float fval = float.Parse(textScaleZ.Text);
                fval += 10.0f;
                textScaleZ.Text = fval.ToString();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }
    }
}
