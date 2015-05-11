using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctlToolpathGenConfig : UserControl //ctlUserPanel// 
    {
        System.Windows.Forms.TabPage m_gcodetab;
        public ctlToolpathGenConfig()
        {
            try
            {
                InitializeComponent();            
                m_gcodetab = tabOptions.TabPages["tbGCode"];
                PopulateProfiles();
               // lbGCodeSection.SelectedIndex = 0;
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        /// <summary>
        /// This lock/unlock pair is for plugin that wish to restrict user's ability
        /// to change too much on the slicing profile screen
        /// </summary>
        public void LockAllButExposure() 
        {
            try
            {
                grpLift.Hide();
                groupBox3.Hide(); // image reflection
                groupBox6.Hide();                
                tabOptions.TabPages.Remove(m_gcodetab);
                //TabControl
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void UnlockAll() 
        {
            try
            {
                grpLift.Show();
                groupBox3.Show(); // image reflection
                groupBox6.Show();
                //tabOptions.TabPages["tbGCode"].Show();
                tabOptions.TabPages.Add(m_gcodetab);
            }
            catch (Exception ex)                       
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
       private SliceBuildConfig m_config = null;
        // this populates the profile in use and the combo 
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
       private string CurPrefGcodePath() 
       {
           try
           {
               string shortname = cmbSliceProfiles.SelectedItem.ToString();
               string fname = UVDLPApp.Instance().m_PathProfiles;
               fname += UVDLPApp.m_pathsep + shortname + UVDLPApp.m_pathsep;
               return fname;
           }
           catch (Exception ex) 
           {
               DebugLogger.Instance().LogError(ex.Message);
               return "";
           }
       }

       private String GetSlicingFilename(string shortname)
       {
           string fname = UVDLPApp.Instance().m_PathProfiles;
           fname += UVDLPApp.m_pathsep + shortname + ".slicing";
           return fname;
       }

        private SliceBuildConfig LoadProfile(string shortname) 
        {
            SliceBuildConfig profile = new SliceBuildConfig();
            try
            {
                string fname = GetSlicingFilename(shortname);
                if (!profile.Load(fname))
                {
                    DebugLogger.Instance().LogError("Could not load " + fname);
                    return null;
                }
                else 
                {
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSlicedLoaded, "Slice Profile loaded");
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSliceProfileChanged, "Slice Profile loaded");
                    return profile;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return null;
        }
        private void SetValues() 
        {            
            chkExport.Checked = m_config.export;
            comboExportSvg.SelectedIndex = m_config.exportsvg;
            comboExportPreview.SelectedItem = m_config.exportpreview.ToString();
            chkExportPNG.Checked = m_config.exportpng;
            chkExport_CheckedChanged(null, null);
            txtAAVal.Text = "" + m_config.aaval.ToString();
            txtBlankTime.Text = m_config.blanktime_ms.ToString();
            txtXOffset.Text = m_config.XOffset.ToString();
            txtYOffset.Text = m_config.YOffset.ToString();
            txtLiftDistance.Text = m_config.liftdistance.ToString();
            txtSlideTilt.Text = m_config.slidetiltval.ToString();
            chkantialiasing.Checked = m_config.antialiasing;
            txtliftfeed.Text = m_config.liftfeedrate.ToString();
            txtbottomliftfeed.Text = m_config.bottomliftfeedrate.ToString();

            txtretractfeed.Text = m_config.liftretractrate.ToString();
            chkReflectX.Checked = m_config.m_flipX;
            chkReflectY.Checked = m_config.m_flipY;
            txtNotes.Text = m_config.m_notes;
            chkOutlines.Checked = m_config.m_createoutlines;
            txtOutlineWidthInset.Text = m_config.m_outlinewidth_inset.ToString();
            txtOutlineWidthInset.Enabled = chkOutlines.Checked;
            txtOutlineWidthOutset.Text = m_config.m_outlinewidth_outset.ToString();
            txtOutlineWidthOutset.Enabled = chkOutlines.Checked;
            // resin
            UpdateResinList();
            SetResinValues();

            cmbBuildDirection.Items.Clear();
            foreach(String name in Enum.GetNames(typeof(SliceBuildConfig.eBuildDirection)))
            {
                cmbBuildDirection.Items.Add(name);
            }
            cmbBuildDirection.SelectedItem = m_config.direction.ToString();
        }

        private void SetResinValues()
        {
            txtZThick.Text = "" + String.Format("{0:0.000}", m_config.ZThick);
            txtLayerTime.Text = "" + m_config.layertime_ms;
            txtFirstLayerTime.Text = m_config.firstlayertime_ms.ToString();
            txtResinPriceL.Text = m_config.m_resinprice.ToString();
            txtnumbottom.Text = m_config.numfirstlayers.ToString();
        }

        private void UpdateResinList()
        {
            comboResin.Items.Clear();
            int i = 0;
            foreach (KeyValuePair<string, InkConfig> entry in m_config.inks)
            {
                comboResin.Items.Add(entry.Key);
                if (entry.Key == m_config.selectedInk)
                    comboResin.SelectedIndex = i;
                i++;
            }
        }

        private bool GetValues() 
        {
            try
            {
                
                m_config.m_exportopt = "ZIP";
                m_config.blanktime_ms = int.Parse(txtBlankTime.Text);
                m_config.XOffset = int.Parse(txtXOffset.Text);
                m_config.YOffset = int.Parse(txtYOffset.Text);
                m_config.liftdistance = double.Parse(txtLiftDistance.Text);
                m_config.slidetiltval = double.Parse(txtSlideTilt.Text);
                m_config.antialiasing = chkantialiasing.Checked;
                m_config.liftfeedrate = double.Parse(txtliftfeed.Text);
                m_config.bottomliftfeedrate = double.Parse(txtbottomliftfeed.Text);
                m_config.liftretractrate = double.Parse(txtretractfeed.Text);
                m_config.m_flipX = chkReflectX.Checked;
                m_config.m_flipY = chkReflectY.Checked;
                m_config.m_notes = txtNotes.Text;
                m_config.aaval = double.Parse(txtAAVal.Text);
                m_config.direction = (SliceBuildConfig.eBuildDirection)Enum.Parse(typeof(SliceBuildConfig.eBuildDirection), cmbBuildDirection.SelectedItem.ToString());
                m_config.export = chkExport.Checked;
                m_config.exportsvg = comboExportSvg.SelectedIndex;
                m_config.exportpng = chkExportPNG.Checked;
                m_config.exportpreview = (PreviewGenerator.ePreview)Enum.Parse(typeof(PreviewGenerator.ePreview), comboExportPreview.SelectedItem.ToString());
                // resin
                m_config.ZThick = Single.Parse(txtZThick.Text);
                m_config.layertime_ms = int.Parse(txtLayerTime.Text);
                m_config.firstlayertime_ms = int.Parse(txtFirstLayerTime.Text);
                m_config.numfirstlayers = int.Parse(txtnumbottom.Text);
                m_config.m_resinprice = double.Parse(txtResinPriceL.Text);
                m_config.m_createoutlines = chkOutlines.Checked;
                m_config.m_outlinewidth_inset = double.Parse(txtOutlineWidthInset.Text);
                m_config.m_outlinewidth_outset = double.Parse(txtOutlineWidthOutset.Text);
                m_config.UpdateCurrentInk();
                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Please check input parameters\r\n" + ex.Message,"Input Error");
                return false;
            }
        }

        private void cmdAutoCalc_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetValues())
                {
                    double zlift = m_config.liftdistance; // in mm
                    double zliftrate = m_config.liftfeedrate; // in mm/m
                    double zliftretract = m_config.liftretractrate; // in mm/m
                    double tilt = m_config.slidetiltval; // in mm
                    double totalpath = Math.Sqrt(tilt * tilt + zlift * zlift);
                    zliftrate /= 60.0d;     // to convert to mm/s
                    zliftretract /= 60.0d;  // to convert to mm/s


                    double tval = 0;
                    double settlingtime = 1500.0d; // 500 ms
                    tval = (totalpath / zliftrate);
                    tval += (totalpath / zliftretract);
                    tval *= 1000.0d; // convert to ms
                    tval += settlingtime;
                    int itval = (int)tval;
                    itval = (itval / 100 + 1) * 100;  // round to the nearest 0.1 second
                    String stime = itval.ToString();
                    txtBlankTime.Text = stime;

                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if(m_config == null) return;
            if(cmbSliceProfiles.SelectedIndex == -1) return;
            try
            {
                if (GetValues())
                {
                    string shortname = cmbSliceProfiles.SelectedItem.ToString();
                    string fname = GetSlicingFilename(shortname);
                    m_config.Save(fname);
                    // make sure main build params are updated if needed
                    if (cmbSliceProfiles.SelectedItem.ToString() == shortname)
                    {
                        UVDLPApp.Instance().LoadBuildSliceProfile(fname);
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmbSliceProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the item
            if (cmbSliceProfiles.SelectedIndex == -1)
            {
                //blank items
                return;
            }
            else 
            {
                //set this profile to be the active one for the program                
                string shortname = cmbSliceProfiles.SelectedItem.ToString();
                string fname = GetSlicingFilename(shortname);
                UVDLPApp.Instance().LoadBuildSliceProfile(fname); // load it globally
                try
                {
                    m_config = LoadProfile(shortname); // and again here for this control
                    if (m_config != null)
                    {
                        SetValues();
                        lbGCodeSection_SelectedIndexChanged(this, null);
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }

            }
        }
        private void cmdNew_Click(object sender, EventArgs e)
        {
            // prompt for a new name
            frmProfileName frm = new frmProfileName();
            if (frm.ShowDialog() == DialogResult.OK) 
            {
                //create a new profile
                SliceBuildConfig bf = new SliceBuildConfig();
                //save it
                string shortname = frm.ProfileName;
                string fname = GetSlicingFilename(shortname);
                if (!bf.Save(fname)) 
                {
                    MessageBox.Show("Error saving new profile " + fname);
                }
                //re-display the new list
                PopulateProfiles();
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string shortname = cmbSliceProfiles.SelectedItem.ToString();
                if (shortname.ToLower().Contains("default"))
                {
                    MessageBox.Show("Cannot delete default profile");
                }
                else
                {

                    string fname = GetSlicingFilename(shortname);
                    File.Delete(fname); // delete the file
                    //string pname = UVDLPApp.Instance().m_PathProfiles + UVDLPApp.m_pathsep + shortname;
                   // Directory.Delete(pname, true); // no longer creating specific directories for profiles - gcode is now embedded
                    //choose another profile to select as main profile
                    cmbSliceProfiles.SelectedIndex = 0; //set to the first profile...
                    PopulateProfiles();
                    //cmbSliceProfiles
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        /// <summary>
        /// this index changes when the user selects an item from the list of GCode file segements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbGCodeSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtGCode.Text = GCodeSection2GCode();
        }

        private string GCodeSection2GCode()
        {
            if (lbGCodeSection.SelectedIndex == -1) return "";
            switch (lbGCodeSection.SelectedItem.ToString())
            {
                case "Start":           return m_config.HeaderCode;
                case "Pre-Slice":       return m_config.PreSliceCode;
                case "Lift":            return m_config.LiftCode;
                case "End": return m_config.FooterCode;
                case "Layer": return m_config.LayerCode;
            }
            return "";
        }
        /*
        private string GCodeSection2FName() 
        {
            if (lbGCodeSection.SelectedIndex == -1) return "";
            switch (lbGCodeSection.SelectedItem.ToString()) 
            {
                case "Start":           return "start.gcode";
                case "Pre-Slice":       return "preslice.gcode";
                case "Lift":            return "lift.gcode";
                case "End":             return "end.gcode";
                case "Open-Shutter":    return "openshutter.gcode";
                case "Close-Shutter":   return "closeshutter.gcode";
            }
            return "";
        }
        */
        private void cmdSaveGCode_Click(object sender, EventArgs e)
        {
            try
            {
                // save the gcode to the right section
                string gcode = txtGCode.Text;
                if (lbGCodeSection.SelectedIndex == -1) return;
                switch (lbGCodeSection.SelectedItem.ToString())
                {
                    case "Start": m_config.HeaderCode = gcode; break;
                    case "Pre-Slice": m_config.PreSliceCode = gcode; break;
                    case "Lift": m_config.LiftCode = gcode; break;
                    case "End": m_config.FooterCode = gcode; break;
                }
               // m_config.SaveFile(CurPrefGcodePath() + GCodeSection2FName(), gcode);
                //really just need to save the profile name here.
                // make sure main build params are updated if needed
                string shortname = cmbSliceProfiles.SelectedItem.ToString();
                string fname = GetSlicingFilename(shortname);
                m_config.Save(fname);

                shortname = cmbSliceProfiles.SelectedItem.ToString();
                if (cmbSliceProfiles.SelectedItem.ToString() == shortname)
                {
                    UVDLPApp.Instance().LoadBuildSliceProfile(GetSlicingFilename(shortname));
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void cmdReloadGCode_Click(object sender, EventArgs e)
        {
            txtGCode.Text = GCodeSection2GCode();
        }

        private void chkExport_CheckedChanged(object sender, EventArgs e)
        {
            //groupBox2.Enabled = chkExport.Checked;
            comboExportSvg.Enabled = chkExport.Checked;
            chkExportPNG.Enabled = chkExport.Checked;
            //comboExportPreview.Enabled = chkExport.Checked;
            labelExportSvg.Enabled = chkExport.Checked;
            //labelExportPreview.Enabled = chkExport.Checked;
        }


        private void chkantialiasing_CheckedChanged(object sender, EventArgs e)
        {
            txtAAVal.Enabled = chkantialiasing.Checked;
        }

        private void comboResin_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_config.SetCurrentInk(comboResin.SelectedItem.ToString());
            SetResinValues();
        }

        private void cmdNewResin_Click(object sender, EventArgs e)
        {
            frmProfileName frm = new frmProfileName();
            frm.Text = "New Resin Profile";
            if (frm.ShowDialog() == DialogResult.OK)
            {
                //create a new resin profile
                string res = m_config.AddNewResin(frm.ProfileName);
                if (res != "OK")
                {
                    MessageBox.Show(res, "Error");
                    return;
                }
                UpdateResinList();
                SetResinValues();
                //comboResin.Items.Add(frm.ProfileName);
                //comboResin.SelectedIndex = comboResin.Items.Count - 1;
            }
        }

        private void cmdDelResin_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to delete this Resin Profile?", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string res = m_config.RemoveSelectedInk();
            if (res != "OK")
            {
                string[] strs = res.Split('|');
                MessageBox.Show(strs[0], strs.Length > 1 ? strs[1] : "Error");
            }
            UpdateResinList();
            SetResinValues();
        }

        private void buttResinCalib_Click(object sender, EventArgs e)
        {
            frmCalibResin frmCalib = new frmCalibResin();
            frmCalib.ShowDialog();
        }

        private void txtAAVal_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdHelp_Click(object sender, EventArgs e)
        {
            frmSliceProfileHelp frm = new frmSliceProfileHelp();
            frm.ShowDialog();

        }

        private void chkOutlines_CheckedChanged(object sender, EventArgs e)
        {
            txtOutlineWidthInset.Enabled = chkOutlines.Checked;
            txtOutlineWidthOutset.Enabled = chkOutlines.Checked;
        }

    }
}