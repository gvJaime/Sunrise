using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using UV_DLP_3D_Printer;
using UV_DLP_3D_Printer.Slicing;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlSliceView : UserControl
    {
       // frmDLP m_frmdlp = null; // now that we support multiple monitors, we can't use this approach anymore

        public ctlSliceView()
        {
            try
            {
                InitializeComponent();
                buttPreviewOnDisplay.Checked = UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay; // set the initial check state
                UVDLPApp.Instance().m_slicer.Slice_Event += new Slicer.SliceEvent(SliceEv);
                UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
                UVDLPApp.Instance().m_buildmgr.BuildStatus += new delBuildStatus(BuildStat);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void BuildStat(eBuildStatus status, string message, int code) 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { BuildStat(status,message,code); }));
            }
            else
            {
                switch (status)
                {
                    case eBuildStatus.eBuildStarted:
                        buttPreviewOnDisplay.Enabled = false;
                        numLayer.Enabled = false;
                        break;
                    case eBuildStatus.eBuildCancelled:
                    case eBuildStatus.eBuildCompleted:
                        buttPreviewOnDisplay.Enabled = true;
                        numLayer.Enabled = true;
                        break;
                    case eBuildStatus.eBuildPaused:
                    case eBuildStatus.eBuildResumed:
                        break;
                }
            }
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
                        case eAppEvent.eSlicedLoaded:
                            if (UVDLPApp.Instance().m_slicefile != null) 
                            {
                                SetNumLayers(UVDLPApp.Instance().m_slicefile.NumSlices);
                            }
                           // DebugLogger.Instance().LogRecord(Message);
                            break;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void SliceEv(Slicer.eSliceEvent ev, int layer, int totallayers, SliceFile sf)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { SliceEv(ev, layer, totallayers, sf); }));
                }
                else
                {
                    switch (ev)
                    {
                        case Slicer.eSliceEvent.eSliceStarted:
                           
                            break;
                        case Slicer.eSliceEvent.eLayerSliced:
                            break;
                        case Slicer.eSliceEvent.eSliceCompleted:                            
                            SetNumLayers(totallayers);
                            break;
                        case Slicer.eSliceEvent.eSliceCancelled:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        void UpdateChecked()
        {
            /*
            if ((m_frmdlp == null) || m_frmdlp.IsDisposed)
            {
                buttPreviewOnDisplay.Checked = false;
                return;
            }
             * */
            //buttPreviewOnDisplay.Checked = m_frmdlp.Visible && UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay;
            buttPreviewOnDisplay.Checked = UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay;
        }
        public void SetNumLayers(int val)
        {
            try
            {
                if (val < 0)
                    val = 0;
                if (val > 0)
                {
                    numLayer.MaxInt = val;
                    numLayer.IntVal = 1;
                    numLayer.Visible = true;
                }
                else
                {
                    numLayer.Visible = false;
                }
                itemNumLayers.DataText = val.ToString();
                ViewLayer(0);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

        }

        
        public void ViewLayer(int layer)
        {
            
            try
            {
                
                // try an invoke to get past the crash
                this.Invoke((MethodInvoker)delegate()
                {                                    
                    //render the 2d slice
                    Bitmap bmp = null;
                    if (UVDLPApp.Instance().m_slicefile != null)
                    {
                        bmp = UVDLPApp.Instance().m_slicefile.GetSliceImage(layer);                      
                        if (bmp == null)
                        {
                            return;
                        }
                        bmp.Tag = BuildManager.SLICE_NORMAL;  
                        // change the picture on this control
                        if (picSlice.Image != null)
                        {
                            //get rid of the old one
                            picSlice.Image.Dispose();
                            picSlice.Image = null;
                        }
                         
                        picSlice.Image = bmp;//now show the 2d slice
                        picSlice.Refresh();

                        //if we're printing, DO NOT show the preview slices on the frmDLP's
                        if (UVDLPApp.Instance().m_buildmgr.IsPrinting != true && UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay == true)
                        {
                            //DisplayManager.Instance().PreviewOnDisplays(bmp);
                            // make a copy because it can be disposed
                            Bitmap preview = new Bitmap(bmp);
                            preview.Tag = BuildManager.SLICE_NORMAL; // mark it as normal
                            DisplayManager.Instance().PreviewOnDisplays(preview);
                        }
                    }
                });
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            
        }
        
        private void numLayer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int vscrollval = numLayer.IntVal - 1;
                ViewLayer(vscrollval);
                numLayer.Refresh();
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                // probably past the max.
            }
        }

        private void ctlSliceView_SizeChanged(object sender, EventArgs e)
        {
            // update inner control positions
            foreach (Control ctl in Controls)
            {
                if (ctl.GetType().IsSubclassOf(typeof(ctlAnchorable)))
                    ((ctlAnchorable)ctl).UpdatePosition();
            }
        }

        private void buttPreviewOnDisplay_Click(object sender, EventArgs e)
        {
            if (buttPreviewOnDisplay.Checked)
            {
                UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay = true;
                ViewLayer(numLayer.IntVal - 1);
            }
            else
            {
                // if the user unchecks the preview on dlp button, blank the dlp display.
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eShowBlank, "");
                UVDLPApp.Instance().m_appconfig.m_previewslicesbuilddisplay = false;
            }
            //save the check value
            UVDLPApp.Instance().m_appconfig.Save(UVDLPApp.Instance().m_apppath + UVDLPApp.m_pathsep + UVDLPApp.m_appconfigname);
        }
    }
}
