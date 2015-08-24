using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Engine3D;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlView : ctlUserPanel
    {
        SplitContainer mTreeViewHolder;
        SplitContainer mMessagePanelHolder;
        ctlNumber mLayerNumberScroll;
        ctlObjectInfo mObjectInfoPanel;
        ctlScene mctlScene;

        public ctlView()
        {
            InitializeComponent();
            mTreeViewHolder = null;
            //set some initial states
            buttBoundingBox.Checked = UVDLPApp.Instance().m_appconfig.m_showBoundingBox;
            buttSelOutline.Checked = UVDLPApp.Instance().m_appconfig.m_showOutline;
            buttSelColor.Checked = UVDLPApp.Instance().m_appconfig.m_showShaded;
            buttShowSlice.Checked = UVDLPApp.Instance().m_appconfig.m_viewslice3d;
            mLayerNumberScroll = null;
            mMessagePanelHolder = null;
            mTreeViewHolder = null;
            mObjectInfoPanel = null;
            #if DEBUG
            buttShowConsole.Checked = true;    
            #else
            buttShowConsole.Checked = false;
            #endif
        }

        public void ConsoleVisible(bool val) 
        {
            buttShowConsole.Visible = val;
            Refresh();
        }
        public SplitContainer MessagePanelHolder
        {
            get { return mMessagePanelHolder; }
            set { mMessagePanelHolder = value; }
        }

        public SplitContainer TreeViewHolder
        {
            get { return mTreeViewHolder; }
            set { mTreeViewHolder = value; }
        }

        public ctlNumber LayerNumberScroll
        {
            get { return mLayerNumberScroll; }
            set { mLayerNumberScroll = value; }
        }

        public ctlScene SceneControl 
        {
            get { return mctlScene; }
            set { mctlScene = value; }
        }
        public ctlObjectInfo ObjectInfoPanel
        {
            get { return mObjectInfoPanel; }
            set { mObjectInfoPanel = value; }
        }

        private void buttEnableTransparency_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_engine3d.m_alpha = buttEnableTransparency.Checked;
            UVDLPApp.Instance().m_engine3d.UpdateLists();
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");            
        }

        private void buttShowSlice_Click(object sender, EventArgs e)
        {
            //buttSliceHeight.Enabled = buttShowSlice.Checked;
            if (mLayerNumberScroll != null)
                mLayerNumberScroll.Visible = buttShowSlice.Checked;
            UVDLPApp.Instance().m_appconfig.m_viewslice3d = buttShowSlice.Checked;
            // now save it
            UVDLPApp.Instance().m_appconfig.Save(UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + UVDLPApp.m_appconfigname);            
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
        }

        private void buttTreeView_Click(object sender, EventArgs e)
        {
         //   SceneControl.Visible = buttTreeView.Checked;
        }

        private void ctlImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void buttShowConsole_Click(object sender, EventArgs e)
        {
            try
            {
                bool tmp = buttShowConsole.Checked;
                string vis = tmp.ToString();
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eShowLogWindow, vis);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        /*private void buttShowSliceHeight_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().m_appconfig.m_viewslice3dheight = buttSliceHeight.Checked;
            UVDLPApp.Instance().m_appconfig.Save(UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + UVDLPApp.m_appconfigname);
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
        }*/

        private void buttObjectProperties_Click(object sender, EventArgs e)
        {
            if (mObjectInfoPanel != null)
            {
             //   mObjectInfoPanel.Visible = buttObjectProperties.Checked;
            }
        }

        private void buttBoundingBox_Click(object sender, EventArgs e)
        {
            setSelectedObjectViewMode(true, false, false);
        }

        private void buttSelOutline_Click(object sender, EventArgs e)
        {
            setSelectedObjectViewMode(false, true, false);
        }

        private void buttSelColor_Click(object sender, EventArgs e)
        {
            setSelectedObjectViewMode(false, false, true);
        }

        private void setSelectedObjectViewMode(bool isBoundingBox, bool isOutline, bool isShaded)
        {
            try
            {
                UVDLPApp.Instance().m_appconfig.m_showBoundingBox = buttBoundingBox.Checked = isBoundingBox;
                UVDLPApp.Instance().m_appconfig.m_showOutline = buttSelOutline.Checked = isOutline;
                UVDLPApp.Instance().m_appconfig.m_showShaded = buttSelColor.Checked = isShaded;
                UVDLPApp.Instance().m_appconfig.Save(UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + UVDLPApp.m_appconfigname);
                if (UVDLPApp.Instance().SelectedObjectList != null)
                {
                    foreach (Object3d obj in UVDLPApp.Instance().SelectedObjectList)
                        obj.InvalidateList();
                }

                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        private void buttSliceView_Click(object sender, EventArgs e)
        {
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "redraw");            
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                flowLayoutPanel2.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
            {
                flowLayoutPanel1.BackColor = ct.FrameColor;
                flowLayoutPanel3.BackColor = ct.FrameColor;
              //  flowLayoutPanel4.BackColor = ct.FrameColor;
              //  flowLayoutPanel5.BackColor = ct.FrameColor;
            }
        }

        private void ctlTitle1_Click(object sender, EventArgs e)
        {
            if (ctlTitle1.Checked)
            {
                //expand
                
                int h = ctlTitle1.Height;
                h += flowLayoutPanel1.Height;
                h += flowLayoutPanel3.Height;
                h += 3 * 5;
                this.Height = h;
            }
            else
            {
                // 
                this.Height = ctlTitle1.Height + 5;
            }
        }

        private void ctlView_Resize(object sender, EventArgs e)
        {
            ctlTitle1.Width = ctlTitle1.Parent.Width - 6;
            flowLayoutPanel1.Width = flowLayoutPanel1.Parent.Width - 6;
            flowLayoutPanel3.Width = flowLayoutPanel3.Parent.Width - 6;
        }

        public override void RegisterSubControls(string parentName)
        {
            UVDLPApp.Instance().m_gui_config.AddButton(parentName + ".title", ctlTitle1);
        }

    }
}
