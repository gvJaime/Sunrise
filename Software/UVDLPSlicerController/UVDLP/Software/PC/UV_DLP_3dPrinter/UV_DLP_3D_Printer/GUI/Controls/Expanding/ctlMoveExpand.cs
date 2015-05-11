using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI.Expanding
{
    public partial class ctlMoveExpand : ctlUserPanel
    {
        public ctlMoveExpand()
        {
            InitializeComponent();
            //register our button so it can be skinned
            //UVDLPApp.Instance().m_gui_config.AddButton("move", ctlTitle1.Button); gets style from control
        }
        public void FixForeColor(Color clr)
        {
            label9.ForeColor = clr;
            label10.ForeColor = clr;
            label11.ForeColor = clr;
        }

        protected void MoveObject(ctlTextBox var, float x, float y, float z)
        {
            try
            {
                if (UVDLPApp.Instance().SelectedObject == null)
                    return;
                float val = var.FloatVal;
                x *= val;
                y *= val;
                z *= val;
                UVDLPApp.Instance().SelectedObject.Translate(x, y, z, true);
                //UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, x, y, z);  // moved to translate function
                //UVDLPApp.Instance().SelectedObject.Update(); // make sure we update                         // moved to translate function
                //ShowObjectInfo();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void buttXMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveX, -1, 0, 0);
        }

        private void buttYMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveY, 0, -1, 0);
        }

        private void buttZMinus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveZ, 0, 0, -1);
        }

        private void buttXPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveX, 1, 0, 0);
        }

        private void buttYPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveY, 0, 1, 0);
        }

        private void buttZPlus_Click(object sender, EventArgs e)
        {
            MoveObject(textMoveZ, 0, 0, 1);
        }

        private void buttCenter_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null) return;
            Point3d center = UVDLPApp.Instance().SelectedObject.CalcCenter();
            UVDLPApp.Instance().SelectedObject.Translate((float)-center.x, (float)-center.y, (float)-center.z, true);
            //UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, (float)-center.x, (float)-center.y, (float)-center.z);
            //UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
        }

        private void buttOnPlatform_Click(object sender, EventArgs e)
        {
            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            Point3d center = UVDLPApp.Instance().SelectedObject.CalcCenter();
            UVDLPApp.Instance().SelectedObject.FindMinMax();
            float zlev = (float)UVDLPApp.Instance().SelectedObject.m_min.z;
            //float epsilon = .05f; // add in a the level of 1 slice
            //float zmove = -zlev - epsilon; // SHS - place object flat on platform, no epsilon
            float zmove = -zlev;
            //UVDLPApp.Instance().SelectedObject.Translate((float)0, (float)0, (float)-zlev);
            //UVDLPApp.Instance().SelectedObject.Translate((float)0, (float)0, (float)-epsilon);
            UVDLPApp.Instance().SelectedObject.Translate(0, 0, zmove, true);
            //UVDLPApp.Instance().m_undoer.SaveTranslation(UVDLPApp.Instance().SelectedObject, 0, 0, zmove);
            //UVDLPApp.Instance().SelectedObject.Update(); // make sure we update
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "updateobject");
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                //label8.ForeColor = ct.ForeColor;
                ctlTitle1.ForeColor = ct.ForeColor;
                label9.ForeColor = ct.ForeColor;
                label10.ForeColor = ct.ForeColor;
                label11.ForeColor = ct.ForeColor;
                textMoveX.ValidColor = ct.ForeColor;
                textMoveY.ValidColor = ct.ForeColor;
                textMoveZ.ValidColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                flowLayoutPanel2.BackColor = ct.BackColor;
                textMoveX.BackColor = ct.BackColor;
                textMoveY.BackColor = ct.BackColor;
                textMoveZ.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel7.BackColor = ct.FrameColor;
                flowLayoutPanel8.BackColor = ct.FrameColor;
                flowLayoutPanel10.BackColor = ct.FrameColor;
            }

        }

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                //expand
                //this.Height = 244 + 5;
                int h = ctlTitle1.Height;
                h += flowLayoutPanel1.Height;
                h += flowLayoutPanel7.Height;
                h += flowLayoutPanel8.Height;
                h += flowLayoutPanel10.Height;
                h += 3 * 8;
                Height = h;
            }
            else 
            {
                // 
                this.Height = ctlTitle1.Height + 5;
            }
        }

        private void ctlMoveExpand_Resize(object sender, EventArgs e)
        {
            //1,7,8,10,title
            ctlTitle1.Width = ctlTitle1.Parent.Width - 6;
            flowLayoutPanel1.Width = flowLayoutPanel1.Parent.Width - 6;
            flowLayoutPanel7.Width = flowLayoutPanel7.Parent.Width - 6;
            flowLayoutPanel8.Width = flowLayoutPanel8.Parent.Width - 6;
            flowLayoutPanel10.Width = flowLayoutPanel10.Parent.Width - 6;
        }

        private void buttArrange_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().Engine3D.RearrangeObjects();
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }
    }
}
