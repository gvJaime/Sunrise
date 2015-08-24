using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlObjectInfo : ctlAnchorable
    {
        const float TitleHeight = 23;
        const float NameHeight = 16;
        const float ItemHeihft = 17;

        public ctlObjectInfo()
        {
            InitializeComponent();
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
        }
        public void FixForeColor(Color clr)
        {
            //treeScene.ForeColor = clr;
            tVolume.FixForeColor(clr);
            tCost.FixForeColor(clr);
            tPoints.FixForeColor(clr);
            tPolys.FixForeColor(clr);
            tMin.FixForeColor(clr);
            tMax.FixForeColor(clr);
            tSize.FixForeColor(clr);
        }
        private void AppEventDel(eAppEvent ev, String Message)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { AppEventDel(ev, Message); }));
                }
                else
                {
                    switch (ev)
                    {
                        case eAppEvent.eModelAdded:
                            FillObjectInfo(UVDLPApp.Instance().SelectedObject);
                            break;
                        case eAppEvent.eUpdateSelectedObject:
                            FillObjectInfo(UVDLPApp.Instance().SelectedObject);
                            break;
                    }
                    //Refresh();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }

        }

        private void AdjustHeight(Control ctl, float newHeight)
        {
            ctl.Height = (int)newHeight;
            ctl.Font = new Font("Arial", newHeight / 1.5f, GraphicsUnit.Pixel);
        }

        private void layoutPanel_Resize(object sender, EventArgs e)
        {
            float totalHeight = NameHeight + 7 * ItemHeihft;
            float totalMargin = 0;
            foreach (Control ctl in layoutPanel.Controls)
            {
                totalMargin += ctl.Margin.Top + ctl.Margin.Bottom;
                ctl.Width = Width - ctl.Margin.Left - ctl.Margin.Right;
            }
            buttScene.Location = new Point(panel1.Width - buttScene.Width, 2);
            tName.Width = panel1.Width - buttScene.Width - 6;
            totalMargin += ctlTitle1.Height;
            if (Height <= totalMargin)
                return;
            float hScale = ((float)Height - totalMargin) / totalHeight;
            //AdjustHeight(tTitle, TitleHeight * hScale);
            AdjustHeight(tName, NameHeight * hScale);
            foreach (Control ctl in layoutPanel.Controls)
            {
                if (ctl.GetType() == typeof(ctlInfoItem))
                    AdjustHeight(ctl, ItemHeihft * hScale);
            }
        }

        public void FillObjectInfo(Object3d selobj)
        {
            List<Object3d> objects;
            if (buttScene.Checked)
            {
                objects = UVDLPApp.Instance().Engine3D.m_objects;
            }
            else
            {
                objects = new List<Object3d>();
                if (selobj != null)
                    objects.Add(selobj);
            }
            if (objects.Count == 0)
            {
                foreach (Control ctl in layoutPanel.Controls)
                {
                    if (ctl.GetType() == typeof(ctlInfoItem))
                        ((ctlInfoItem)ctl).DataText = "";
                }
                tName.Text = "";
                return;
            }
            tName.Text = buttScene.Checked ? "Scene" : selobj.Name;
            Point3d min = new Point3d(99999999,99999999,99999999);
            Point3d max = new Point3d(-99999999,-99999999,-99999999);
            int points = 0;
            int polys = 0;
            double vol = 0;
            foreach (Object3d obj in objects)
            {
                if (obj.tag != Object3d.OBJ_NORMAL && obj.tag != Object3d.OBJ_SUPPORT && obj.tag != Object3d.OBJ_SUPPORT_BASE)
                    continue;
                obj.FindMinMax();
                points += obj.NumPoints;
                polys += obj.NumPolys;
                if (obj.m_min.x < min.x) min.x = obj.m_min.x;
                if (obj.m_min.y < min.y) min.y = obj.m_min.y;
                if (obj.m_min.z < min.z) min.z = obj.m_min.z;
                if (obj.m_max.x > max.x) max.x = obj.m_max.x;
                if (obj.m_max.y > max.y) max.y = obj.m_max.y;
                if (obj.m_max.z > max.z) max.z = obj.m_max.z;
                vol += obj.Volume;
            }
            tPoints.DataText = points.ToString();
            tPolys.DataText = polys.ToString();
            tMin.DataText = String.Format("{0:0.00}, {1:0.00}, {2:0.00}", min.x, min.y, min.z);
            tMax.DataText = String.Format("{0:0.00}, {1:0.00}, {2:0.00}", max.x, max.y, max.z);
            double xs, ys, zs;
            xs = max.x - min.x;
            ys = max.y - min.y;
            zs = max.z - min.z;
            tSize.DataText = String.Format("{0:0.00}, {1:0.00}, {2:0.00}", xs, ys, zs);
            vol /= 1000.0; // convert to cm^3
            tVolume.DataText = string.Format("{0:0.000} cm^3", vol);
            double cost = vol * (UVDLPApp.Instance().m_buildparms.m_resinprice / 1000.0);
            tCost.DataText = string.Format("{0:0.000}", cost);
       }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
            {
                ctlTitle1.ForeColor = ct.ForeColor;
                tName.ForeColor = ct.ForeColor;
            }
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                layoutPanel.BackColor = ct.BackColor;
                ctlTitle1.BackColor = ct.BackColor;
                tName.BackColor = ct.BackColor;
            }
        }

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                this.Height = 250;
                /*
                int h = 0;
                h += ctlTitle1.Height;
                h += tName.Height;
                h += tVolume.Height;
                h += tCost.Height;
                h += tPoints.Height;
                h += tPolys.Height;
                h += tMin.Height;
                h += tMax.Height;
                h += tSize.Height;
                this.Height = h + 3*4;
                */
            }
            else
            {
                this.Height = ctlTitle1.Height + 5;
            }
        }

        private void ctlObjectInfo_Resize(object sender, EventArgs e)
        {
            /*
            layoutPanel.Width = layoutPanel.Parent.Width;
            ctlTitle1.Width = ctlTitle1.Parent.Width -6;
            tName.Width = tName.Parent.Width - 6;
            tVolume.Width = tVolume.Parent.Width - 6;
            tCost.Width = tCost.Parent.Width - 6;
            tPoints.Width = tPoints.Parent.Width - 6;
            tPolys.Width = tPolys.Parent.Width - 6;
            tMin.Width = tMin.Parent.Width - 6;
            tMax.Width = tMax.Parent.Width - 6;
            tSize.Width = tSize.Parent.Width - 6;
            */
        }

        private void buttScene_Click(object sender, EventArgs e)
        {
            FillObjectInfo(UVDLPApp.Instance().SelectedObject);
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }
    }
}
