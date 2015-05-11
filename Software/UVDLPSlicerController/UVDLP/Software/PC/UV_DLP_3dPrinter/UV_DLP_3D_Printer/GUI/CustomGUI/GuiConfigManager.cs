using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using UV_DLP_3D_Printer._3DEngine;
using UV_DLP_3D_Printer.Plugin;
using UV_DLP_3D_Printer.Util.Sequence;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class GuiConfigManager
    {
       // public enum EntityType { Buttons, Panels, Decals } // not used

        //Dictionary<String, ctlUserPanel> Controls;
        Dictionary<String, Control> Controls;
        Dictionary<String, ctlImageButton> Buttons;
        Dictionary<String, GuiControlStyle> GuiControlStylesDict;
        Dictionary<String, Control> GuiLayouts;
        List<GuiDecorItem> BgndDecorList;
        List<GuiDecorItem> FgndDecorList;
        ResourceManager Res; // the resource manager for the main CW application
        IPlugin Plugin; // not used quite yet...
        Control mTopLevelControl = null;
        public GuiControlStyle DefaultControlStyle;
        public GuiControlStyle DefaultButtonStyle;
        public GuiControlStyle DefaultTitleStyle;
        GuiConfigDB guiConf;
        bool firstTime;
        


        public GuiConfigManager()
        {
            Controls = new Dictionary<string, Control>();
            Buttons = new Dictionary<string, ctlImageButton>();
            GuiControlStylesDict = new Dictionary<string, GuiControlStyle>();
            BgndDecorList = new List<GuiDecorItem>();
            FgndDecorList = new List<GuiDecorItem>();
            GuiLayouts = new Dictionary<string, Control>();
            Res = global::UV_DLP_3D_Printer.Properties.Resources.ResourceManager;
            Plugin = null;
            DefaultControlStyle = new GuiControlStyle("DefaultControl");
            DefaultControlStyle.SetDefault();
            DefaultButtonStyle = new GuiControlStyle("DefaultButton");
            DefaultButtonStyle.SetDefault();
            DefaultTitleStyle = new GuiControlStyle("DefaultTitle");
            DefaultTitleStyle.SetDefault();
            firstTime = true;
        }

        public Control TopLevelControl
        {
            get { return mTopLevelControl; }
            set { mTopLevelControl = value; }
        }

        public void AddControl(string name, Control ctl)
        {
            Controls[name] = ctl;
            if (ctl is ctlUserPanel)
                ((ctlUserPanel)ctl).RegisterSubControls(name);
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }
        public void AddControl(Control ctl)
        {
            AddControl(ctl.Name, ctl);
        }

        public void AddButton(string name, ctlImageButton ctl)
        {
            Buttons[name] = ctl;
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }

        public void AddButton(ctlImageButton ctl)
        {
            AddButton(ctl.Name, ctl);
        }

        // in preview mode we use a copy of the controls with '!' at the beginning of the name.
        /*private string AdjustControlName(string name)
        {
            if ((name[0] != '!') && guiConf.PreviewMode)
                name = "!" + name;
            return name;
        }*/

        public Control GetControl(string name, bool previewMode)
        {
            if (name == null)
                return null;
            if (previewMode && Controls.ContainsKey("!" + name))
                return Controls["!" + name];
            if (!Controls.ContainsKey(name))
                return null;
            return Controls[name];
        }

        public Control GetControl(string name)
        {
            return GetControl(name, guiConf.PreviewMode);
        }

        public ctlImageButton GetButton(string name)
        {
            if ((name == null) || !Buttons.ContainsKey(name))
                return null;
            return Buttons[name];
        }

        public Control GetControlOrButton(string name, bool previewMode)
        {
            if (name == null)
                return null;
            Control ctl = GetButton(name);
            if (ctl == null)
                return GetControl(name, previewMode);
            return ctl;
        }

        public bool RemoveButton(string name)
        {
            if ((name == null) || !Buttons.ContainsKey(name))
                return false;
            Buttons.Remove(name);
            return true;
        }

        public Control GetControlOrButton(string name)
        {
            return GetControlOrButton(name, guiConf.PreviewMode);
        }

        private Control CreatePreviewControl(string name, Control ctl)
        {
            ctlPreview prevCtl = new ctlPreview();
            if (ctl != null)
            {
                if (ctl.Width >  20)
                    prevCtl.Width = ctl.Width;
                if (ctl.Height > 10)
                    prevCtl.Height = ctl.Height;
                prevCtl.Dock = ctl.Dock;
                if (ctl is ctlImageButton)
                    prevCtl.Image = ((ctlImageButton)ctl).Image;
            }
            prevCtl.Text = name;
            prevCtl.Name = name;
            Controls["!" + name] = prevCtl;
            if (guiConf.PreviewMode)
                ctl.MouseEnter += new EventHandler(ctl_MouseEnter);
            return prevCtl;
        }

        private void AddLayoutControl(string name, Control ctl)
        {
            if ((ctl.Name == null) || (ctl.Name == ""))
                ctl.Name = name;
            if (!(ctl is ctlPreview))
                Controls[ctl.Name] = ctl;
            if (guiConf.PreviewMode)
                ctl.MouseEnter += new EventHandler(ctl_MouseEnter);
        }

        /*private void AddLayoutControl(GuiLayout gl, Control ctl)
        {
            if ((ctl.Name == null) || (ctl.Name == ""))
                ctl.Name = gl.name;
            if (guiConf.PreviewMode || (gl.type != GuiLayout.LayoutType.Control))
                Controls[gl.name] = ctl;
            if (guiConf.PreviewMode)
                ctl.MouseEnter += new EventHandler(ctl_MouseEnter);
        }*/

        public GuiControlStyle GetControlStyle(string name)
        {
            if ((name == null) || !GuiControlStylesDict.ContainsKey(name))
                return null;
            return GuiControlStylesDict[name];
        }
        
        public static int GetPosition(int refpos, int refwidth, int width, int gap, Char anchor)
        {
            int retval = 0;
            switch (anchor)
            {
                case 't':
                case 'l':
                    retval = refpos + gap;
                    break;

                case 'c':
                    retval = refpos + (refwidth - width) / 2 + gap;
                    break;

                case 'r':
                case 'b':
                    retval = refpos + refwidth - width - gap;
                    break;
                default:
                    retval = gap;
                    break;
            }
            return retval;
        }

        public void ApplyConfiguration(GuiConfigDB conf)
        {
            guiConf = conf;
            if (firstTime)
            {
                UVDLPApp.Instance().m_callbackhandler.RegisterCallback("GMActivateTab", ActivateTab, "GUI Tab Activation <ctlTabItem> <ctlTabContent>");
                firstTime = false;
            }
            try
            {
                // read and store all styles
                foreach (KeyValuePair<string, GuiControlStyle> pair in conf.GuiControlStylesDict)
                    GuiControlStylesDict[pair.Key] = pair.Value;

                HandleDecals(conf);
                HandleButtonCreation(conf);
                HandleLayouts(conf);
                HandleControls(conf);
                HandleButtons(conf);
                HandleSequences(conf);

            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }


        public void DumpDatabase(string fname)
        {
            StreamWriter sw = new StreamWriter(fname);
            sw.WriteLine("Controls:");
            foreach (KeyValuePair<String, Control> pair in Controls)
            {
                sw.WriteLine("  {0,-30}{1}", pair.Key, pair.Value.GetType().ToString());
            }
            sw.WriteLine("");
            sw.WriteLine("Buttons:");
            foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
            {
                sw.WriteLine("  {0,-30}{1}", pair.Key, pair.Value.GetType().ToString());
            }
            sw.Close();
        }


        #region Sequences
        
        void HandleSequences(GuiConfigDB conf) 
        {
            foreach (CommandSequence cmdseq in conf.CmdSequenceList)
            {
                if (cmdseq.m_seqtype  == CommandSequence.COMMAND_TYPE_GCODE)
                {
                    GCodeSequence gcseq = new GCodeSequence(cmdseq.m_name, cmdseq.m_seq);
                    SequenceManager.Instance().Add(gcseq);
                }
                if (cmdseq.m_seqtype == CommandSequence.COMMAND_TYPE_SPAWN_PROCESS) 
                {
                    ProcessSequence cast = (ProcessSequence)cmdseq;
                    SequenceManager.Instance().Add(cast.Clone()); // insert a cloned copy into the sequencemanager
                }
            }

        }
        #endregion

        #region Decals

        void HandleDecals(GuiConfigDB conf)
        {
            if (conf.HideAllDecals)
            {
                BgndDecorList.Clear();
                FgndDecorList.Clear();
            }
            BgndDecorList.AddRange(conf.BgndDecorList);
            FgndDecorList.AddRange(conf.FgndDecorList);
        }

        #endregion

        #region Buttons
        void HandleButtons(GuiConfigDB conf)
        {
            if (conf.HideAllButtons.IsExplicit() && conf.HideAllButtons)
                HideAllButtons();

            // apply default style if exists
            GuiControlStyle defbutstl = conf.GetControlStyle("DefaultButton");
            if (defbutstl != null)
                DefaultButtonStyle = defbutstl;
            GuiControlStyle deftitstl = conf.GetControlStyle("DefaultTitle");
            foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
            {
                ctlImageButton butt = pair.Value;
                if (butt is ctlTitle)
                {
                    if (deftitstl != null)
                        butt.ApplyStyle(deftitstl);
                }
                else
                {
                    if (defbutstl != null)
                        butt.ApplyStyle(defbutstl);
                }
            }
            foreach (KeyValuePair<string, GuiButton> pair in conf.GuiButtonsDict)
                HandleButton(conf, pair.Value);
        }

        void HandleButtonCreation(GuiConfigDB conf)
        {
            foreach (KeyValuePair<string, GuiButton> pair in conf.GuiButtonsDict)
            {
                GuiButton gbtn = pair.Value;
                if (!Buttons.ContainsKey(gbtn.name))
                {
                    // create a new empty button
                    ctlImageButton btn = new ctlImageButton();
                    btn.Name = gbtn.name;
                    AddButton(btn);
                    btn.BringToFront();
                }
            }
        }

        void HandleButton(GuiConfigDB conf, GuiButton gbtn)
        {
            if (!Buttons.ContainsKey(gbtn.name))
                return; // should not happen!
            ctlImageButton butt = Buttons[gbtn.name];
//            butt.Visible = true;
            butt.Visible = gbtn.visible.GetIfExplicit(true);
            butt.GuiAnchor = gbtn.dock.GetIfExplicit(butt.GuiAnchor);
            butt.Gapx = gbtn.x.GetIfExplicit(butt.Gapx);
            butt.Gapy = gbtn.y.GetIfExplicit(butt.Gapy);
            butt.Width = gbtn.w.GetIfExplicit(butt.Width);
            butt.Height = gbtn.h.GetIfExplicit(butt.Height);
            butt.StyleName = gbtn.style.GetIfExplicit(butt.StyleName);
            butt.OnClickCallback = gbtn.onClickCmd.GetIfExplicit(butt.OnClickCallback);
            GuiControlStyle bstl = conf.GetControlStyle(butt.StyleName);
            if (bstl != null)
            {
                butt.GLVisible = bstl.glMode;
            }
            if (gbtn.image.IsExplicit())
            {
                butt.GLImage = gbtn.image;
                butt.Image = conf.GetImage(gbtn.image, null);
            }
            butt.CheckImage = conf.GetImage(gbtn.checkImage, butt.CheckImage);


            // add the ability to add buttons in various named parents
            // this will allow adding buttons to toolbar from plugins
            if (gbtn.action.IsExplicit())
            {
                string action = gbtn.action;
                if (action.Contains("remove")) // this handles removing a control from it's parent
                {
                    // remove this control from it's parent
                    if (butt.Parent != null)
                    {
                        butt.Parent.Controls.Remove(butt);
                        butt.Parent = null;
                    }
                }
                else if (action.Contains("addto")) // this handles adding a new control to a parent control
                {
                    // Get the name of the parent
                    string parentname = gbtn.parent;
                    if (gbtn.parent.IsExplicit() && (parentname != null) && (parentname.Length != 0))
                    {
                        //find the parent
                        if (Controls.ContainsKey(parentname))
                        {
                            Control ctlParent = Controls[parentname];
                            AddControlToParent(butt, ctlParent);
                        }
                        else
                        {
                            DebugLogger.Instance().LogWarning("Button parent not found: " + parentname);
                        }
                    }
                }
            }
        }

        public List<string> GetButtonNames()
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, ctlImageButton> pair in Buttons)
                res.Add(pair.Key);
            return res;
        }

        #endregion

        #region Controls
        void HandleControls(GuiConfigDB conf)
        {
            if (conf.HideAllControls.IsExplicit() && conf.HideAllControls)
                HideAllControls();
            // apply default style if exists
            GuiControlStyle stl = conf.GetControlStyle("DefaultControl");
            
            if (stl != null)
            {
                DefaultControlStyle = stl;
                foreach (KeyValuePair<String, Control> pair in Controls)
                {
                    if (pair.Value is ctlUserPanel)
                    {
                        ctlUserPanel ctl = (ctlUserPanel)pair.Value;
                        ctl.ApplyStyle(stl);
                    }
                    else
                    {
                        // apply the style by recursing through the object
                        ApplyStyleRecurse(pair.Value, stl);
                    }
                }
            }
            
            foreach (KeyValuePair<string, GuiControl> pair in conf.GuiControlsDict)
                HandleControl(conf, pair.Value);
        }

        #region style applying for non-ctlUsercontrol controls
        public void ApplyStyleRecurse(Control ctl, GuiControlStyle ct)
        {

            if ((ctl is ctlUserPanel) || ct.applyWindowsControls)
            {
                if (ct.BackColor.IsValid())
                    ctl.BackColor = ct.BackColor;

                if (ct.ForeColor.IsValid())
                    ctl.ForeColor = ct.ForeColor;
            }
            if (!ct.applySubControls)
                return;

            foreach (Control subctl in ctl.Controls)
            {
                if (subctl is ctlUserPanel)
                {
                    ((ctlUserPanel)subctl).ApplyStyle(ct);
                }
                else
                {
                    ApplyStyleRecurse(subctl, ct);
                }
            }
        }

        #endregion

        void HandleControl(GuiConfigDB conf, GuiControl gctl)
        {
            //if (!Controls.ContainsKey(gctl.name))
            //    return;

            //Control ct = Controls[gctl.name]; // find the existing control
            Control ct = GetControl(gctl.name);
            if (ct == null)
                return;
            if (gctl.visible.IsExplicit())
                ct.Visible = gctl.visible.GetVal();
            ct.Width = gctl.w.GetIfExplicit(ct.Width);
            ct.Height = gctl.h.GetIfExplicit(ct.Height);
            //load some control locations as well,

            if (gctl.px.IsExplicit() || gctl.py.IsExplicit())
            {
                int px, py;
                px = gctl.px.GetIfExplicit(ct.Location.X);
                py = gctl.py.GetIfExplicit(ct.Location.Y);
                Point pt = new Point(px, py);
                ct.Location = pt;
            }

            // load docking style
            if (gctl.action.IsExplicit())
            {
                string action = gctl.action;  // telling something to happen to this control
                if (action.Contains("remove")) // this handles removing a control from it's parent
                {
                    // remove this control from it's parent
                    if (ct.Parent != null)
                    {
                        try
                        {
                            if (ct.Parent.Controls.Contains(ct))
                            {
                                ct.Parent.Controls.Remove(ct);
                                ct.Parent = null;
                            }
                        }
                        catch (Exception ex) 
                        {
                            DebugLogger.Instance().LogError(ex);
                        }
                    }
                }
                else if (action.Contains("hide")) // this handles hiding
                {
                    // hide this control, do not remove it from the parent
                    ct.Hide();
                }
                else if (action.Contains("show")) // this handles showing
                {
                    // show this control
                    ct.Show();
                }
                else if (action.Contains("addto")) // this handles adding a new/existing control to a parent control
                {
                    // Get the name of the parent
                    // Get the name of the parent
                    string parentname = gctl.parent;
                    if (gctl.parent.IsExplicit() && (parentname != null) && (parentname.Length != 0))
                    {
                        //find the parent
                        //find the parent
                        if (Controls.ContainsKey(parentname))
                        {
                            Control ctlParent = Controls[parentname];
                            AddControlToParent(ct, ctlParent);
                        }
                        else
                        {
                            DebugLogger.Instance().LogWarning("Control parent not found: " + parentname);
                        }
                    }
                }
            }

            String styleName = gctl.style.GetIfValid(null);
            GuiControlStyle style = conf.GetControlStyle(styleName);
            if (ct is ctlUserPanel)
            {
                ctlUserPanel ctl = (ctlUserPanel)ct;
                ctl.GuiAnchor = gctl.dock.GetIfExplicit(ctl.GuiAnchor);
                ctl.Gapx = gctl.x.GetIfExplicit(ctl.Gapx);
                ctl.Gapy = gctl.y.GetIfExplicit(ctl.Gapy);
                if (styleName != null)
                {
                    ctl.StyleName = styleName;
                    if (style != null)
                    {
                        ctl.GLVisible = style.glMode;
                        ctl.ApplyStyle(style);
                    }
                }
                //ctl.GLVisible = GetBoolParam(ctlnode, "gl", false);
                if (ctl.GLVisible)
                    ctl.GLBackgroundImage = gctl.BorderShape.GetIfExplicit(ctl.GLBackgroundImage);
                else
                    ctl.bgndPanel.imageName = gctl.BorderShape.GetIfExplicit(ctl.bgndPanel.imageName);
            }
            else
            {
                if (style != null)
                {
                    ApplyStyleRecurse(ct, style);
                }
            }
        }

        public List<string> GetControlNames()
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, Control> pair in Controls)
                res.Add(pair.Key);
            return res;
        }

        #endregion

        #region Handle layout

        public Control GetLayout(string name)
        {
            if (!GuiLayouts.ContainsKey(name))
                return null;
            return GuiLayouts[name];
        }

        void HandleLayouts(GuiConfigDB conf)
        {
            foreach (GuiLayout gl in conf.GuiLayouts)
            {
                Control ctl = CreateLayoutRecurse(gl);
                GuiLayouts[gl.name] = ctl;
            }

        }

        void AddControlToParent(Control ctl, Control parent)
        {
            if (parent is ctlCollapse)
                ((ctlCollapse)parent).AddControl(ctl);
            else
                parent.Controls.Add(ctl);
        }

        Control CreateLayoutRecurse(GuiLayout gl)
        {
            Control ctl = null;
            switch (gl.type)
            {
                case GuiLayout.LayoutType.Control:
                    ctl = GetControlOrButton(gl.name, false);
                    if (guiConf.PreviewMode)
                        ctl = CreatePreviewControl(gl.name, ctl);
                    FillCommonLayoutParameters(gl, ctl);
                    break;

                case GuiLayout.LayoutType.FlowPanel:
                    ctl = CreateFlowPanel(gl);
                    break;

                case GuiLayout.LayoutType.Layout:
                case GuiLayout.LayoutType.Panel:
                    ctl = new Panel();
                    ctl.SuspendLayout();
                    if (gl.type == GuiLayout.LayoutType.Layout)
                        ctl.Dock = DockStyle.Fill;
                    FillCommonLayoutParameters(gl, ctl);
                    break;

                case GuiLayout.LayoutType.SplitPanel:
                    ctl = CreateSplitPanel(gl);
                    break;

                case GuiLayout.LayoutType.TabPanel:
                    ctl = CreateTabPanel(gl);
                    break;
            }

            if (ctl == null)
                return null;

            if (guiConf.PreviewMode || (gl.type != GuiLayout.LayoutType.Control))
                AddLayoutControl(gl.name, ctl);
            CreateSublayouts(gl, ctl);
            if (ctl is ctlCollapse)
                ((ctlCollapse)ctl).InnerControl.ResumeLayout();
            ctl.ResumeLayout();
            if ((gl.type != GuiLayout.LayoutType.Control) && gl.style.IsValid())
            {
                GuiControlStyle stl = GetControlStyle(gl.style);
                if (stl != null)
                    ApplyStyleRecurse(ctl, stl);
            }
            return ctl;
        }

        void ctl_MouseEnter(object sender, EventArgs e)
        {
            Control ctl = (Control)sender;
            UVDLPApp.Instance().m_callbackhandler.Activate("MouseEnteredControl", ctl, ctl.Name);
        }

        void CreateSublayouts(GuiLayout gl, Control parent)
        {
            foreach (GuiLayout subgl in gl.subLayouts)
            {
                Control subctl = CreateLayoutRecurse(subgl);
                if (subctl != null)
                {
                    AddControlToParent(subctl, parent);
                }
            }
        }

        Control WrapInCollapsible(GuiLayout gl, Control ctl)
        {
            Control retCtl = ctl;
            if (gl.collapsible.IsExplicit() && gl.collapsible)
            {
                ctlCollapse colCtl = new ctlCollapse();
                colCtl.SuspendLayout();
                colCtl.Name = ctl.Name + "Parent";
                colCtl.Dock = ctl.Dock;
                colCtl.InnerControl = ctl;
                AddLayoutControl(gl.name, ctl);
                if (gl.isCollapsed.IsExplicit())
                    colCtl.Collapsed = gl.isCollapsed;
                retCtl = colCtl;
            }
            return retCtl;
        }

        Control CreateFlowPanel(GuiLayout gl)
        {
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.SuspendLayout();
            flp.AutoScroll = true;
            flp.AutoScrollMargin = new System.Drawing.Size(100, 1000);
            flp.WrapContents = false;
            flp.FlowDirection = FlowDirection.LeftToRight;
            if (gl.direction.IsExplicit())
            {
                switch (((string)gl.direction).ToUpper())
                {
                    case "L2R": flp.FlowDirection = FlowDirection.LeftToRight; break;
                    case "T2B": flp.FlowDirection = FlowDirection.TopDown; break;
                    case "R2L": flp.FlowDirection = FlowDirection.RightToLeft; break;
                    case "B2T": flp.FlowDirection = FlowDirection.BottomUp; break;
                }
            }
            FillCommonLayoutParameters(gl, flp);
            return WrapInCollapsible(gl,flp);
        }

        SplitContainer CreateSplitPanel(GuiLayout gl)
        {
            SplitContainer sc = new SplitContainer();
            sc.SuspendLayout();
            sc.Panel1.SuspendLayout();
            sc.Panel2.SuspendLayout();
            sc.Orientation = Orientation.Horizontal;
            if (gl.orientation.IsExplicit() && (((string)gl.orientation).ToLower()[0] == 'v'))
                sc.Orientation = Orientation.Vertical;
            if (gl.splitPos.IsExplicit())
                sc.SplitterDistance = gl.splitPos;
            foreach (GuiLayout subgl in gl.subLayouts)
            {
                SplitterPanel sp;
                if (subgl.type == GuiLayout.LayoutType.SplitPanel1)
                    sp = sc.Panel1;
                else if (subgl.type == GuiLayout.LayoutType.SplitPanel2)
                    sp = sc.Panel2;
                else
                    continue;
                CreateSublayouts(subgl, sp);
            }
            FillCommonLayoutParameters(gl, sc);
            sc.Panel1.ResumeLayout();
            sc.Panel2.ResumeLayout();
            return sc;
        }

        Panel CreateTabPanel(GuiLayout gl)
        {
            Panel pnl = new Panel();
            pnl.SuspendLayout();
            pnl.Dock = DockStyle.Fill;
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.SuspendLayout();
            flp.Dock = DockStyle.Top;
            flp.FlowDirection = FlowDirection.LeftToRight;
            flp.Size = new Size(50, 50);
            flp.BackColor = Color.Aqua;
            foreach (GuiLayout subgl in gl.subLayouts)
            {
                Control ctl = null;
                if (subgl.type == GuiLayout.LayoutType.TabItem)
                    ctl = CreateTabItem(subgl, flp);
                if (ctl != null)
                    pnl.Controls.Add(ctl);
            }
            FillCommonLayoutParameters(gl, pnl);
            pnl.Controls.Add(flp);
            AddLayoutControl(gl.name + "-Title", flp);
            flp.ResumeLayout();
            pnl.BackColor = Color.Bisque;
            return pnl;
        }

        Control CreateTabItem(GuiLayout gl, FlowLayoutPanel flp)
        {
            Control shownControl = null;
            if (gl.subLayouts.Count == 1)
            {
                shownControl = CreateLayoutRecurse(gl.subLayouts[0]);
            }
            else
            {
                shownControl = new Panel();
                //shownControl.Name = gl.name + "-Content";
                //AddControl(shownControl);
                //Controls[shownControl.Name] = shownControl;
                CreateSublayouts(gl, shownControl);
                AddLayoutControl(gl.name + "-Content", shownControl);
            }
            if (shownControl == null)
                return null;

            shownControl.Dock = DockStyle.Fill;
            shownControl.Visible = gl.isSelected.IsExplicit() && gl.isSelected;

            Control tabctl = null;
            if (gl.control.IsExplicit() && !guiConf.PreviewMode)
                tabctl = GetControlOrButton(gl.control);
            if (tabctl == null)
            {
                string cmd = String.Format("GMActivateTab {0} {1}", gl.name, shownControl.Name);
                if (!gl.text.IsExplicit())
                {
                    // if no text, create image button
                    AddButton(gl.name, new ctlImageButton());
                    ctlImageButton butt = Buttons[gl.name];
                    butt.BringToFront();
                    if (gl.image.IsExplicit())
                    {
                        butt.GLImage = gl.image;
                        butt.Image = guiConf.GetImage(gl.image, null);
                    }
                    butt.OnClickCallback = cmd;
                    tabctl = butt;
                }
                else
                {
                    // text valid, create title control
                    RemoveButton(gl.name); // in case such a button exists with the same name
                    ctlTitle ttl = new ctlTitle();
                    ttl.Image = guiConf.GetImage(gl.image, null);
                    ttl.Text = gl.text;
                    ttl.Name = gl.name;
                    ttl.OnClickCallback = cmd;
                    ttl.CheckImage = guiConf.GetImage("buttChecked", null);
                    ttl.Size = new Size(180, 40);
                    ttl.FitWidth();
                    Controls[gl.name] = ttl;
                    tabctl = ttl;

                }
                CheckTab(tabctl, gl.isSelected.IsExplicit() && gl.isSelected);

            }
            AddLayoutControl(gl.name, tabctl);
            flp.Controls.Add(tabctl);
            return shownControl;
        }

        void FillCommonLayoutParameters(GuiLayout gl, Control ctl)
        {
            if (ctl == null)
                return;
            ctl.Name = gl.name;
            ctl.Visible = true;
            if (gl.w.IsExplicit()) ctl.Width = gl.w;
            if (gl.h.IsExplicit()) ctl.Height = gl.h;
            if (gl.dock.IsExplicit())
            {
                switch (((string)gl.dock).ToLower())
                {
                    case "top": ctl.Dock = DockStyle.Top; break;
                    case "bottom": ctl.Dock = DockStyle.Bottom; break;
                    case "left": ctl.Dock = DockStyle.Left; break;
                    case "right": ctl.Dock = DockStyle.Right; break;
                    case "fill": ctl.Dock = DockStyle.Fill; break;
                }
            }
            int px = ctl.Location.X;
            int py = ctl.Location.Y;
            if (gl.px.IsExplicit()) px = gl.px;
            if (gl.py.IsExplicit()) py = gl.py;
            if ((px != ctl.Location.X) || (py != ctl.Location.Y))
            {
                ctl.Location = new Point(px,py);
            }
        }

        #endregion

        #region Perform 3Dlayout

        void Draw(List<GuiDecorItem> dlist, C2DGraphics g2d, int w, int h)
        {
            foreach (GuiDecorItem di in dlist)
            {
                di.Show(g2d, w, h);
            }
        }

        public void DrawForeground(C2DGraphics g2d, int w, int h)
        {
            Draw(FgndDecorList, g2d, w, h);
        }

        public void DrawBackground(C2DGraphics g2d, int w, int h)
        {
            Draw(BgndDecorList, g2d, w, h);
        }

        public void LayoutGui(int w, int h)
        {
            LayoutButtons(w, h);
            LayoutControls(w, h);
        }

        void LayoutButtons(int w, int h)
        {
            foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
            {
                ctlImageButton butt = pair.Value;
                if (butt.GuiAnchor == null)
                    continue;
                int px = GetPosition(0, w, butt.Width, butt.Gapx, butt.GuiAnchor[1]);
                int py = GetPosition(0, h, butt.Height, butt.Gapy, butt.GuiAnchor[0]);
                butt.Location = new System.Drawing.Point(px, py);
            }
        }
        
        void LayoutControls(int w, int h)
        {
            foreach (KeyValuePair<String, Control> pair in Controls)
            {
                if (pair.Value is ctlUserPanel)
                {
                    ctlUserPanel ctl = (ctlUserPanel)pair.Value;
                    if (ctl.GuiAnchor == null)
                        continue;
                    int px = GetPosition(0, w, ctl.Width, ctl.Gapx, ctl.GuiAnchor[1]);
                    int py = GetPosition(0, h, ctl.Height, ctl.Gapy, ctl.GuiAnchor[0]);
                    ctl.Location = new System.Drawing.Point(px, py);
                }
                else 
                {
                    
                }
            }
        }

        public void ClearLayout()
        {
            BgndDecorList = new List<GuiDecorItem>();
            FgndDecorList = new List<GuiDecorItem>();
        }

        public void HideAllButtons()
        {
            foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
            {
                ctlImageButton butt = pair.Value;
                butt.Visible = false;
            }
        }

        void HideAllControls()
        {
            foreach (KeyValuePair<String, Control> pair in Controls)
            {
                Control ctl = pair.Value;
                ctl.Visible = false;
            }
        }

        #endregion
        #region Gui Layout text commands handling

        string[]  VerifyArgs(Object varsobj, int count, string funcname)
        {
             string[] vars = (string[])varsobj;
            if (vars.Length != count)
            {
                DebugLogger.Instance().LogError("Wrong number of arguments calling" + funcname);
                return null;
            }
            return vars;
       }

        #region GuiActivateTab command

        // GuiActivateTab <ctlTabItem> <ctlTabContent>
        void ActivateTab(Object sender, Object varsobj)
        {
            string[] vars = VerifyArgs(varsobj, 2, "ActivateTab");
            if (vars == null)
                return;
            Control tabCtl = GetControlOrButton(vars[0]);
            Control contentCtl = GetControl(vars[1]);
            if ((tabCtl == null) || !(tabCtl is ctlUserPanel) || (contentCtl == null))
            {
                DebugLogger.Instance().LogError("Unknown/wrong controls calling GuiActivateTab");
                return;
            }
            ActivateTab(tabCtl.Parent, tabCtl, contentCtl);
        }

        void CheckTab(Control tabItem, bool isCheck)
        {
            if (tabItem is ctlImageButton)
                ((ctlImageButton)tabItem).Checked = isCheck;
            else if (tabItem is ctlTitle)
                ((ctlTitle)tabItem).Checked = isCheck;
        }

        public void ActivateTab(Control tabParent, Control tabItem, Control tabContent)
        {
            foreach (Control ctl in tabParent.Controls)
            {
                CheckTab(ctl, Object.ReferenceEquals(ctl, tabItem));
            }
            foreach (Control ctl in tabParent.Parent.Controls)
            {
                if (Object.ReferenceEquals(ctl, tabParent))
                    continue;
                if (Object.ReferenceEquals(ctl, tabContent))
                {
                    ctl.Dock = DockStyle.Fill;
                    ctl.Visible = true;
                }
                else
                {
                    ctl.Visible = false;
                    ctl.Dock = DockStyle.None;
                }
            }
        }

        #endregion

        bool GetToggleState(string type, Control button, bool curstate)
        {
            if (type.ToLower() == "false")
                return false;
            if (type.ToLower() != "toggle")
                return true;
            if (button is ctlImageButton)
                return ((ctlImageButton)button).Checked;
            return !curstate;
        }

        // GuiShowControl <control> <true/false/toggle>
        public void ShowControl(Object sender, Object varsobj)
        {
            string[] vars = VerifyArgs(varsobj, 2, "GuiShowControl");
            if (vars == null)
                return;

            Control ctl = GetControlOrButton(vars[0]);
            if (ctl == null)
            {
                DebugLogger.Instance().LogError("Unknown/wrong control calling GuiShowControl");
                return;
            }
            ctl.Visible = GetToggleState(vars[2], (Control)sender, ctl.Visible);
        }

        // GuiCollapse <ctlCollapseButt> <true/false>
        // will collapse a control to the size of ctlCollapseButt that should be inside it.
        // the control must be either docked to one of the sides, and must be parent of ctlCollapseButt
        public void CollapseControl(Object sender, Object varsobj)
        {
            string[] vars = VerifyArgs(varsobj, 2, "GuiCollapse");
            if (vars == null)
                return;

        }

        #endregion
    }
}
