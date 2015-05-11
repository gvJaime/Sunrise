using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using UV_DLP_3D_Printer.Slicing;
using UV_DLP_3D_Printer.Plugin;
namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmSlice : Form
    {
        Process exeProcess = null;
        Thread m_thread = null;
        public frmSlice()
        {
            InitializeComponent();
            SetProgressSpinner(false);
            UVDLPApp.Instance().m_slicer.Slice_Event += new Slicer.SliceEvent(SliceEv);
            SetTitle();

            //default to the appropriate slicer
            if (UVDLPApp.Instance().m_printerinfo.m_machinetype == MachineConfig.eMachineType.UV_DLP)
            {
                cmbSliceEngine.Visible = false;
                cmbSliceEngine.SelectedIndex = 0;
            }
            else 
            {
                cmbSliceEngine.Visible = true; // could be FDM based machine, allow user t ochoose slicing engine
                cmbSliceEngine.SelectedIndex = 1;
            }
        }

        private void SliceEv(Slicer.eSliceEvent ev, int layer, int totallayers, SliceFile sf)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SliceEv(ev, layer, totallayers,sf); }));
            }
            else
            {
                switch (ev)
                {
                    case Slicer.eSliceEvent.eSliceStarted:
                        cmdSlice.Text = "Cancel";
                        prgSlice.Maximum = totallayers - 1;
                        break;
                    case Slicer.eSliceEvent.eLayerSliced:
                        prgSlice.Maximum = totallayers - 1;
                        prgSlice.Value = layer;
                        lblMessage.Text = "Slicing Layer " + (layer + 1).ToString() + " of " + totallayers.ToString();
                        
                        break;
                    case Slicer.eSliceEvent.eSliceCompleted:
                        lblMessage.Text = "Slicing Completed";
                        cmdSlice.Text = "Slice!";
                        Close();
                        break;
                    case Slicer.eSliceEvent.eSliceCancelled:
                        cmdSlice.Text = "Slice!";
                        lblMessage.Text = "Slicing Cancelled";
                        prgSlice.Value = 0;
                        break;
                }
            }
        }

        private void SetTitle()
        {
            this.Text = "Slice! " + "  ( Slice Profile : ";
            this.Text += Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_buildparms.m_filename);
            this.Text += ", Machine : " + Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_printerinfo.m_filename) + ")";
        }
        private void cmdSliceOptions_Click(object sender, EventArgs e)
        {
            //frmSliceOptions m_frmsliceopt = new frmSliceOptions();
            //m_frmsliceopt.Show();
            //frmSliceOptions m_frmsliceopt = new frmSliceOptions(ref UVDLPApp.Instance().m_buildparms);
            //m_frmsliceopt.ShowDialog(); // will modal work here?
        }
        private void frmSlice_FormClosed(object sender, FormClosedEventArgs e)
        {
            UVDLPApp.Instance().m_slicer.Slice_Event -=new Slicer.SliceEvent(SliceEv);
        }

        private void cmdSlice_Click(object sender, EventArgs e)
        {
            LoadSelectedProfile();
            try
            {
                if (cmbSliceEngine.SelectedIndex == -1) 
                    return;
                if (cmbSliceEngine.SelectedIndex == 0)
                {
                    if (UVDLPApp.Instance().m_slicer.IsSlicing)
                    {
                        UVDLPApp.Instance().m_slicer.CancelSlicing();
                    }
                    else
                    {
                        SliceBuildConfig sp = UVDLPApp.Instance().m_buildparms;
                        sp.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we've got the correct display size and PixPerMM values                        
                        int numslices = UVDLPApp.Instance().m_slicer.GetNumberOfSlices(sp); // determine the number of slices\
                        UVDLPApp.Instance().m_slicefile = UVDLPApp.Instance().m_slicer.Slice(sp); // start slicing the scene
                    }
                }
                else 
                {
                    if (cmdSlice.Text.Contains("Cancel")) // cancel Slic3r
                    {
                        if (exeProcess != null) 
                        {
                            exeProcess.Kill();
                        }
                    }
                    else // start slicing using Slic3r
                    {
                        //save the scene object to a temp name
                        UVDLPApp.Instance().CalcScene(); // calc the scene object
                        if (UVDLPApp.Instance().m_engine3d.m_objects.Count > 1) 
                        {
                            UVDLPApp.Instance().Scene.SaveSTL_Binary(UVDLPApp.Instance().Scene.m_fullname);
                        } // else already saved

                        StartThread();
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        private void SetProgressSpinner(bool val) 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SetProgressSpinner(val); }));
            }
            else
            {
                if (val == true)
                {
                    pictureBox1.Image = global::UV_DLP_3D_Printer.Properties.Resources.animatedTurningHelix;
                }
                else 
                {
                    pictureBox1.Image = null;//
                }
            }        
        }
        private void CloseForm() 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { CloseForm(); }));
            }
            else
            {
                Close();
            }        
        }

        private string RunGCodePostProcess(string gcodefilename) 
        {
            //iterate throguh all loaded/enabled plugins
            //check to see if they support post-processing of gcode

            object[] parms = new object[1];
            //set the first parameter to be the image
            parms[0] = (object)gcodefilename;
            //call the plugin command
            UVDLPApp.Instance().PerformPluginCommand("GCodePostProcess", parms, true);
            return gcodefilename;
        }
        private void TriggerLoadGCode(string gcodefilename) 
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { TriggerLoadGCode(gcodefilename); }));
            }
            else
            {                
                // if this is the powder-based printer, or
                // any machine profile that requires post-processing of the gcode,
                // then we need to run the post-process command here...
                UVDLPApp.Instance().LoadGCode(RunGCodePostProcess(gcodefilename));

            }        
        }
        private void SetCmdSliceText(string txt)         
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { SetCmdSliceText(txt); }));
            }
            else
            {
                cmdSlice.Text = txt; // set the button to be 'cancel'
            }
        }

        private void StartThread() 
        {
            m_thread = new Thread(new ThreadStart(StartSlic3r));
            m_thread.Start();
        }
        private void StartSlic3r() 
        {
            string slicerexe = UVDLPApp.Instance().m_appconfig.m_slic3rloc;
            string parms = UVDLPApp.Instance().Scene.m_fullname;
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = slicerexe;
            startInfo.Arguments = parms + " " + UVDLPApp.Instance().m_appconfig.m_slic3rparameters;
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;//.Hidden;
            //startInfo.Arguments += parms;
            SetProgressSpinner(true);
            DebugLogger.Instance().LogInfo("Slic3r exe: " + startInfo.FileName);
            DebugLogger.Instance().LogInfo("Arguements: " + startInfo.Arguments);

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (exeProcess = Process.Start(startInfo))
                {
                    SetCmdSliceText("Cancel");
                    exeProcess.WaitForExit();
                    int exitcode = exeProcess.ExitCode;
                    // exit code 2 for 1 type of failure
                    // 255 is another exit failure
                    if (exitcode >= 0)
                    {
                        string gcodename = Path.GetDirectoryName(parms) + UVDLPApp.m_pathsep + Path.GetFileNameWithoutExtension(parms) + ".gcode";
                        TriggerLoadGCode(gcodename);
                        CloseForm();
                    }
                    else 
                    {
                        DebugLogger.Instance().LogError("Slic3r failed with exit code " + exitcode);
                        MessageBox.Show("Slicing Failed");
                    }
                    SetCmdSliceText("Slice");
                    SetProgressSpinner(false);
                }
            }
            catch(Exception ex)
            {
                SetCmdSliceText("Slice");
                SetProgressSpinner(false);
                DebugLogger.Instance().LogError("Slic3r failed " + ex.Message);
                MessageBox.Show("Slicing Failed");
            }        
        }        

        private void frmSlice_Activated(object sender, EventArgs e)
        {
            SetTitle();
            if (UVDLPApp.Instance().m_engine3d.m_objects.Count == 0)
            {
                lblMessage.Text = "Scene is empty";
                cmdSlice.Enabled = false;
            }
        }

        private void PopulateProfiles()
        {
            try
            {
                cmbSliceProfiles.Items.Clear();
                foreach (string prof in UVDLPApp.Instance().SliceProfiles())
                {
                    cmbSliceProfiles.Items.Add(prof);
                }
                //get the current profile name
                string curprof = UVDLPApp.Instance().GetCurrentSliceProfileName();
                cmbSliceProfiles.SelectedItem = curprof;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            PopulateProfiles();
            base.OnShown(e);
        }

        private void LoadSelectedProfile()
        {
            if (cmbSliceProfiles.SelectedIndex == -1)
            {
                //blank items
                return;
            }
            else
            {
                //set this profile to be the active one for the program                
                string shortname = cmbSliceProfiles.SelectedItem.ToString();
                string fname = UVDLPApp.Instance().m_PathProfiles + UVDLPApp.m_pathsep + shortname + ".slicing";
                UVDLPApp.Instance().LoadBuildSliceProfile(fname);
            }

        }

    }
}
