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
   
    public abstract class DecorItem
    {
        public abstract void Show(C2DGraphics g2d, int w, int h);
        public bool visible = true;
        public string name = null;
    }

    public class DecorImage : DecorItem
    {
        public DecorImage(string imgname, string docking, int x, int y, Color col)
        {
            this.imgname = imgname;
            this.docking = docking;
            this.x = x;
            this.y = y;
            this.color = col;
        }
        string imgname;
        string docking; // tl = top left, rc = right center, nn = no dock, etc
        int x, y;       // gap to edge when docked, absolute if not
        Color color;

        public override void Show(C2DGraphics g2d, int w, int h)
        {
            if (!visible)
                return;
            int iw = 0, ih = 0;
            g2d.GetImageDim(imgname, ref iw, ref ih);
            int px = GuiConfig.GetPosition(0, w, iw, x, docking[1]);
            int py = GuiConfig.GetPosition(0, h, ih, y, docking[0]);
            g2d.SetColor(color);
            g2d.Image(imgname, px, py);
        }
    }

    public class DecorBar : DecorItem
    {
        public DecorBar(string docking, int w, Color col) // solid bar
        {
            this.docking = docking;
            this.bw = w;
            coltl = col;
            coltr = col;
            colbl = col;
            colbr = col;
        }

        public DecorBar(string docking, int w, Color coltl, Color coltr, Color colbl, Color colbr) // gradient bar
        {
            this.docking = docking;
            this.bw = w;
            this.coltl = coltl;
            this.coltr = coltr;
            this.colbl = colbl;
            this.colbr = colbr;
        }

        string docking; // t = top, b = bottom, l = left, r = right, n = no dock (fullscreen)
        int bw;         // bar width
        Color coltl, coltr, colbl, colbr;
        public override void Show(C2DGraphics g2d, int w, int h)
        {
            int px, py, pw, ph;
            switch (docking)
            {
                case "t": px = 0; py = 0; pw = w; ph = bw; break;
                case "b": px = 0; py = h - bw; pw = w; ph = bw; break;
                case "l": px = 0; py = 0; pw = bw; ph = h; break;
                case "r": px = w - bw; py = 0; pw = bw; ph = h; break;
                default: px = 0; py = 0; pw = w; ph = h; break;
            }
            g2d.GradientRect(px, py, pw, ph, coltl, coltr, colbl, colbr);
        }
    }

    public class ControlPad
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    // NOTE!! any member added to this class should be added to SetDefault() and CopyFrom() member functions.
    public class ControlStyle
    {
        public static Color NullColor = Color.FromArgb(1);
        public static Color DefaultColor = Color.FromArgb(2);

        public ControlStyle(Color forecol, Color backcol)
        {
            ForeColor = forecol;
            BackColor = backcol;
            FrameColor = NullColor;
            BackImage = null;
        }

        public ControlStyle()
        {
            ForeColor = NullColor;
            BackColor = NullColor;
            FrameColor = NullColor;
            BackImage = null;
        }

        // general purpose control style
        public String Name;
        public Color ForeColor;
        public Color BackColor;
        public Color FrameColor;
        public String BackImage;
        public bool glMode;

        // button styles
        public int SubImgCount;
        public Color PressedColor;
        public Color CheckedColor;
        public Color DisabledColor;
        public Color HoverColor;
        public float PressedSize;
        public float HoverSize;
        //public string BgndImageName;
        String mCheckedImage;
        C2DImage mCheckedImageCach;
        public ControlPad PanelPad;

        // misc
        public bool applySubControls;
        public bool applyWindowsControls;


        public virtual void SetDefault()
        {
            ForeColor = Color.White;
            BackColor = Color.RoyalBlue;
            FrameColor = Color.Navy;
            CheckedColor = Color.Orange;
            PressedColor = Color.White;
            HoverColor = Color.White;
            PressedSize = 100;
            HoverSize = 100;
            DisabledColor = Color.FromArgb(60, 255, 255, 255);
            SubImgCount = 1;
            glMode = false;
            //BgndImageName = null;
            PanelPad = new ControlPad();
            PanelPad.Left = PanelPad.Right = PanelPad.Top = PanelPad.Bottom = 10;
            applySubControls = true;
            applyWindowsControls = false;
        }

        public void CopyFrom(ControlStyle sctl)
        {
            if (sctl == null)
                return;
            ForeColor = sctl.ForeColor;
            BackColor = sctl.BackColor;
            FrameColor = sctl.FrameColor;
            BackImage = sctl.BackImage;
            glMode = sctl.glMode;

            SubImgCount = sctl.SubImgCount;
            PressedColor = sctl.PressedColor;
            CheckedColor = sctl.CheckedColor;
            DisabledColor = sctl.DisabledColor;
            HoverColor = sctl.HoverColor;
            PressedSize = sctl.PressedSize;
            HoverSize = sctl.HoverSize;
            //BgndImageName = sctl.BgndImageName;
            PanelPad = new ControlPad();
            PanelPad.Left = sctl.PanelPad.Left;
            PanelPad.Right = sctl.PanelPad.Right; 
            PanelPad.Top = sctl.PanelPad.Top;
            PanelPad.Bottom = sctl.PanelPad.Bottom;
            applySubControls = sctl.applySubControls;
            applyWindowsControls = sctl.applyWindowsControls;
            CheckedImage = sctl.CheckedImage;
        }

        public String CheckedImage
        {
            get { return mCheckedImage; }
            set
            {
                mCheckedImage = value;
                mCheckedImageCach = null;
            }
        }

        public C2DImage CheckedImageCach
        {
            get
            {
                if (mCheckedImageCach == null)
                    mCheckedImageCach = UVDLPApp.Instance().m_2d_graphics.GetImage(mCheckedImage);
                return mCheckedImageCach;
            }
        }

    }
     

    public class GuiConfig
    {
       // public enum EntityType { Buttons, Panels, Decals } // not used

        //Dictionary<String, ctlUserPanel> Controls;
        Dictionary<String, Control> Controls;
        Dictionary<String, ctlImageButton> Buttons;
        Dictionary<String, ControlStyle> ControlStyles;
        Dictionary<String, DecorItem> DecorItems;
        List<DecorItem> BgndDecorList;
        List<DecorItem> FgndDecorList;
        ResourceManager Res; // the resource manager for the main CW application
        IPlugin Plugin;
        Control mTopLevelControl = null;
        public ControlStyle DefaultControlStyle; 


        public GuiConfig()
        {
            BgndDecorList = new List<DecorItem>();
            FgndDecorList = new List<DecorItem>();
            //Controls = new Dictionary<string, ctlUserPanel>();
            Controls = new Dictionary<string, Control>();
            Buttons = new Dictionary<string, ctlImageButton>();
            ControlStyles = new Dictionary<string, ControlStyle>();
            DecorItems = new Dictionary<string,DecorItem>();
            Res = global::UV_DLP_3D_Printer.Properties.Resources.ResourceManager;
            Plugin = null;
            DefaultControlStyle = new ControlStyle();
            DefaultControlStyle.Name = "DefaultControl";
            DefaultControlStyle.SetDefault();
            ControlStyles[DefaultControlStyle.Name] = DefaultControlStyle;
        }

        public Control TopLevelControl
        {
            get { return mTopLevelControl; }
            set { mTopLevelControl = value; }
        }
        /*
        public void AddControl(string name, ctlUserPanel ctl)
        {
            Controls[name] = ctl;
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }
        */
        public void AddControl(string name, Control ctl)
        {
            Controls[name] = ctl;
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }

        public void AddControl(Control ctl)
        {
            if ((ctl == null) || (ctl.Name == null))
                return;
            Controls[ctl.Name] = ctl;
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }
        public void AddButton(string name, ctlImageButton ctl)
        {
            Buttons[name] = ctl;
            if ((ctl.Parent == null) && (mTopLevelControl != null))
                mTopLevelControl.Controls.Add(ctl);
        }

        public ControlStyle GetControlStyle(string name)
        {
            if ((name == null) || !ControlStyles.ContainsKey(name))
                return null;
            return ControlStyles[name];
        }

        /*
        public ctlUserPanel GetControl(string name)
        {
            if (!Controls.ContainsKey(name))
                return null;
            return Controls[name];
        }
        */
        public Control GetControl(string name)
        {
            if (!Controls.ContainsKey(name))
                return null;
            return Controls[name];
        }
        public ctlImageButton GetButton(string name)
        {
            if (!Buttons.ContainsKey(name))
                return null;
            return Buttons[name];
        }

        public DecorItem GetDecorItem(string name)
        {
            if (!DecorItems.ContainsKey(name))
                return null;
            return DecorItems[name];
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

        public void LoadConfiguration(String xmlConf, IPlugin plugin)
        {
            Plugin = plugin;
            try
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlConf));
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(ms);
                XmlNode rootnode = xdoc.ChildNodes[1];
                if (rootnode.Name != "GuiConfig")
                    return;

                foreach (XmlNode xnode in rootnode.ChildNodes)
                {
                    switch (xnode.Name)
                    {
                        case "decals": HandleDecals(xnode); break;
                        case "buttons": HandleButtons(xnode); break; 
                        case "controls": HandleControls(xnode); break;
                        case "sequences": LoadSequences(xnode); break;
                    }
                }

            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void LoadConfiguration(String xmlConf)
        {
            LoadConfiguration(xmlConf, null);
        }
        #region Sequences
        // sequences are command sequences that can be used
        // to send gcode (or other) commmands.
        // These sequences can be tied to a button onclick handler
        // this allows for creating a new button in the GUIConfig, and 
        //causing the click to send special sequences to the printer.
        
        void LoadSequences(XmlNode seqnode) 
        {
            foreach (XmlNode xnode in seqnode.ChildNodes)
            {
                switch (xnode.Name.ToLower())
                {
                    case "sequence": LoadSequence(xnode); break;                    
                }
            }

        }
        // sequences should be named with the prefix of where they came from, such as a namespace
        // for example, if a sequence is loaded from the guiconfig of
        // the plugin named plugPro, and the sequence name is goHome,
        // then the name should be: plugPro.goHome
        void LoadSequence(XmlNode seqnode) 
        {
            //get name
            string name = GetStrParam(seqnode, "name", "");
            //get sequence
            string seq = GetStrParam(seqnode, "seqdata", "");
            //get type
            string seqtype = GetStrParam(seqnode, "seqtype", "");
            if (seqtype.ToLower().Equals("gcode"))
            {
                GCodeSequence gcseq = new GCodeSequence(name, seq);
                SequenceManager.Instance().Add(gcseq);
            }
            else
            {
                DebugLogger.Instance().LogWarning("Unknown sequence type " + seqtype + " in GUIConfig");
            }
        }
        #endregion

        #region Decals

        void HandleDecals(XmlNode decalnode)
        {
            if (GetBoolParam(decalnode, "HideAll", false))
                ClearLayout();
            foreach (XmlNode xnode in decalnode.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "bar": HandleBars(xnode); break;
                    case "image": HandleImages(xnode); break;
                }
            }
        }

        List<DecorItem> GetListFromLevel(XmlNode xnode)
        {
            List<DecorItem> dlist = FgndDecorList;
            if (GetStrParam(xnode, "level", "") == "background")
            {
                dlist = BgndDecorList;
            }
            return dlist;
        }


        void HandleBars(XmlNode barnode)
        {
            string docking = GetStrParam(barnode, "dock", "n").ToLower();
            int w = GetIntParam(barnode, "width", 100);
            string name = GetStrParam(barnode, "name", null);
            List<DecorItem> dlist = GetListFromLevel(barnode);
            DecorItem dcr = null;
            if (GetStrParam(barnode, "color", null) == null)
            {
                Color coltl = GetColorParam(barnode, "tlcolor", Color.White);
                Color coltr = GetColorParam(barnode, "trcolor", Color.White);
                Color colbl = GetColorParam(barnode, "blcolor", Color.White);
                Color colbr = GetColorParam(barnode, "brcolor", Color.White);
                dcr = new DecorBar(docking, w, coltl, coltr, colbl, colbr);
            }
            else
            {
                Color col = GetColorParam(barnode, "color", Color.White);
                dcr = new DecorBar(docking, w, col);
            }
            dlist.Add(dcr);
            if (name != null)
            {
                dcr.name = name;
                DecorItems[name] = dcr;
            }

        }

        void HandleImages(XmlNode imgnode)
        {
            string imgname = GetStrParam(imgnode, "image", null);
            if (imgname == null)
                return;
            string name = GetStrParam(imgnode, "name", null);
            string docking = FixDockingVal(GetStrParam(imgnode, "dock", "cc"));
            int x = GetIntParam(imgnode, "x", 0);
            int y = GetIntParam(imgnode, "y", 0);
            Color col = GetColorParam(imgnode, "color", Color.White);
            int opacity = GetIntParam(imgnode, "opacity", -1) * 255 / 100;
            if ((opacity >= 0) && (opacity <= 255))
                col = Color.FromArgb(opacity, col.R, col.G, col.B);
            List<DecorItem> dlist = GetListFromLevel(imgnode);
            DecorItem dcr = new DecorImage(imgname, docking, x, y, col);
            dlist.Add(dcr);
            if (name != null)
            {
                dcr.name = name;
                DecorItems[name] = dcr;
            }
        }
        #endregion

        #region Buttons
        void HandleButtons(XmlNode buttnode)
        {
            if (GetBoolParam(buttnode, "HideAll", false))
                HideAllButtons();
            foreach (XmlNode xnode in buttnode.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "style": HandleButtonStyle(xnode); break;
                    case "button": HandleButton(xnode); break;
                }
            }
        }

        void HandleButtonStyle(XmlNode xnode)
        {
            string name = GetStrParam(xnode, "name", "DefaultButton");
            ControlStyle bt = GetControlStyle(name);
            if (bt == null)
            {
                bt = new ControlStyle();
                bt.Name = name;
                ControlStyles[name] = bt;
                bt.SetDefault();
            }
            UpdateStyle(xnode, bt);

            if (name == "DefaultButton")
            {
                foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
                {
                    ctlImageButton butt = pair.Value;
                    butt.ApplyStyle(bt);
                }
            }
        }

        void HandleButton(XmlNode buttnode)
        {
            string name = GetStrParam(buttnode, "name", null);
            if (name == null)
                return;
            if (!Buttons.ContainsKey(name))
            {
                // create a new empty button
                AddButton(name, new ctlImageButton());
                Buttons[name].BringToFront();
            }
            ctlImageButton butt = Buttons[name];
//            butt.Visible = true;
            butt.Visible = GetBoolParam(buttnode, "visible", true);
            butt.GuiAnchor = FixDockingVal(GetStrParam(buttnode, "dock", butt.GuiAnchor));
            butt.Gapx = GetIntParam(buttnode, "x", butt.Gapx);
            butt.Gapy = GetIntParam(buttnode, "y", butt.Gapy);
            butt.Width = GetIntParam(buttnode, "w", butt.Width);
            butt.Height = GetIntParam(buttnode, "h", butt.Height);
            butt.StyleName = GetStrParam(buttnode, "style", butt.StyleName);
            butt.OnClickCallback = GetStrParam(buttnode, "click", butt.OnClickCallback);
            ControlStyle bstl = GetControlStyle(butt.StyleName);
            if (bstl != null)
            {
                butt.GLVisible = bstl.glMode;
                butt.ApplyStyle(bstl);
            }
            //butt.GLVisible = GetBoolParam(buttnode, "gl", butt.GLVisible);
            string imgname = GetStrParam(buttnode, "image", null);
            if (imgname != null)
            {
                butt.GLImage = imgname;
                butt.Image = GetImageParam(buttnode, "image", null);
            }
            butt.CheckImage = GetImageParam(buttnode, "check", butt.CheckImage);


            // add the ability to add buttons in various named parents
            // this will allow adding buttons to toolbar from plugins
            string action = GetStrParam(buttnode, "action", "none");  // telling something to happen to this control
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
                string parentname = GetStrParam(buttnode, "parent", "");
                if (parentname == null) return;
                if (parentname.Length == 0) return;
                //find the parent
                Control ctlParent = Controls[parentname];
                if (ctlParent == null)
                {
                    DebugLogger.Instance().LogWarning("Button parent now found: " + parentname);
                    return;
                }
                {
                    ctlParent.Controls.Add(butt);
                }
            }
        }

        #endregion

        #region Controls
        void HandleControls(XmlNode ctlnode)
        {
            if (GetBoolParam(ctlnode, "HideAll", false))
                HideAllControls();
            foreach (XmlNode xnode in ctlnode.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "style": HandleControlStyle(xnode); break;
                    case "control": HandleControl(xnode); break;
                }
            }
        }
        
        void HandleControlStyle(XmlNode xnode)
        {
            string name = GetStrParam(xnode, "name", "DefaultControl");
            ControlStyle ct = GetControlStyle(name);
            if (ct == null)
            {
                ct = new ControlStyle();
                ct.Name = name;
                ControlStyles[name] = ct;
                ct.SetDefault();
            }
            UpdateStyle(xnode, ct);
            if (name == "DefaultControl")
            {
                //foreach (KeyValuePair<String, ctlUserPanel> pair in Controls)
                foreach (KeyValuePair<String, Control> pair in Controls)
                {
                    if (pair.Value is ctlUserPanel)
                    {
                        ctlUserPanel ctl = (ctlUserPanel)pair.Value;
                        ctl.ApplyStyle(DefaultControlStyle);
                    }
                    else 
                    {
                        // apply the style by recursing through the object
                        ApplyStyleRecurse(pair.Value, DefaultControlStyle);
                    }
                }
            }
        }
        #region style applying for non-ctlUsercontrol controls
        public void ApplyStyleRecurse(Control ctl, ControlStyle ct)
        {

            if ((ctl is ctlUserPanel) || ct.applyWindowsControls)
            {
                if (ct.BackColor != ControlStyle.NullColor)
                    ctl.BackColor = ct.BackColor;

                if (ct.ForeColor != ControlStyle.NullColor)
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

        void HandleControl(XmlNode ctlnode)
        {
            string name = GetStrParam(ctlnode, "name", null);
            if (name == null)
                return;
            if (!Controls.ContainsKey(name))
                return;

            //ctlUserPanel ctl = Controls[name];
            Control ct = Controls[name]; // find the existing control
            if (ctlnode.Attributes.GetNamedItem("visible") != null)
                ct.Visible = GetBoolParam(ctlnode, "visible", ct.Visible);
            ct.Width = GetIntParam(ctlnode, "w", ct.Width);
            ct.Height = GetIntParam(ctlnode, "h", ct.Height);
            //load some control locations as well,            
            int px, py;
            px = GetIntParam(ctlnode, "px", ct.Location.X);
            py = GetIntParam(ctlnode, "py", ct.Location.Y);
            Point pt = new Point(px,py);
            ct.Location = pt;
            // load docking style


            string action = GetStrParam(ctlnode, "action", "none");  // telling something to happen to this control
            if (action.Contains("remove")) // this handles removing a control from it's parent
            {
                // remove this control from it's parent
                if (ct.Parent != null)
                {
                    ct.Parent.Controls.Remove(ct);
                    ct.Parent = null;
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
                string parentname = GetStrParam(ctlnode, "parent", "");
                if (parentname == null) return;
                if (parentname.Length == 0) return;
                //find the parent
                Control ctlParent = Controls[parentname];
                if (ctlParent == null) 
                {
                    DebugLogger.Instance().LogWarning("Control parent not found: " + parentname);
                    return;
                }
                {
                    ctlParent.Controls.Add(ct);
                }
            }
            String styleName = GetStrParam(ctlnode, "style", null);
            ControlStyle style = GetControlStyle(styleName);
            if (ct is ctlUserPanel)
            {
                ctlUserPanel ctl = (ctlUserPanel)ct;
                ctl.GuiAnchor = FixDockingVal(GetStrParam(ctlnode, "dock", ctl.GuiAnchor));
                ctl.Gapx = GetIntParam(ctlnode, "x", ctl.Gapx);
                ctl.Gapy = GetIntParam(ctlnode, "y", ctl.Gapy);
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
                    ctl.GLBackgroundImage = GetStrParam(ctlnode, "shape", ctl.GLBackgroundImage);
                else
                    ctl.bgndPanel.imageName = GetStrParam(ctlnode, "shape", ctl.bgndPanel.imageName);
            }
            else
            {
                if (style != null)
                {
                    ApplyStyleRecurse(ct, style);
                }
            }
        }

        #endregion


        #region Attribute parsing
        string GetStrParam(XmlNode xnode, string paramName, string defVal)
        {
            try
            {
                string res = xnode.Attributes[paramName].Value;
                return res;
            }
            catch (Exception)
            {
                return defVal;
            }
        }

        int GetIntParam(XmlNode xnode, string paramName, int defVal)
        {
            try
            {
                int res = int.Parse(xnode.Attributes[paramName].Value);
                return res;
            }
            catch (Exception)
            {
                return defVal;
            }
        }

        int [] GetIntArrayParam(XmlNode xnode, string paramName)
        {
            List<int> num = new List<int>();
            string val = GetStrParam(xnode, paramName, null);
            if (val == null)
                return new int[0];
            foreach (string snum in val.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    int tnum = int.Parse(snum);
                    num.Add(tnum);
                }
                catch { }
            }
            int[] res = new int[num.Count];
            for (int i = 0; i < num.Count; i++)
                res[i] = num[i];
            return res;
        }

        bool GetBoolParam(XmlNode xnode, string paramName, bool defVal)
        {
            try
            {
                bool res = bool.Parse(xnode.Attributes[paramName].Value);
                return res;
            }
            catch (Exception)
            {
                return defVal;
            }
        }

        Color GetColorParam(XmlNode xnode, string paramName, Color defVal)
        {
            Color res;
            try
            {
                string sres = xnode.Attributes[paramName].Value;
                if (sres[0] == '#')
                {
                    sres = sres.Substring(1);
                    if (sres.Length > 7)
                        res = Color.FromArgb(int.Parse(sres, System.Globalization.NumberStyles.HexNumber));
                    else
                        res = Color.FromArgb((int)(long.Parse(sres, System.Globalization.NumberStyles.HexNumber) | 0xFF000000));
                }
                else if (sres == "null")
                {
                    res = ControlStyle.NullColor;
                }
                else if (sres == "default")
                {
                    res = ControlStyle.DefaultColor;
                }
                else
                {
                    res = Color.FromName(sres);
                }
                return res;
            }
            catch (Exception)
            {
                return defVal;
            }

        }

        Image GetImageParam(XmlNode xnode, string paramName, Image defVal)
        {
            string imgname = GetStrParam(xnode, paramName, null);
            if (imgname == null)
                return defVal;
            Image img = null;
            if (Plugin != null)
                img = Plugin.GetImage(imgname);
            if (img == null) // try to get from the 2d graphics first
                img = UVDLPApp.Instance().m_2d_graphics.GetBitmap(imgname);
            if (img == null)
                img = (Image)Res.GetObject(imgname);
            if (img == null)
                return defVal;
            return img;
        }

        string FixDockingVal(string origdock)
        {
            if (origdock == null)
                return "cc";
            string dock = origdock.ToLower();
            while (dock.Length < 2)
                dock += "c";
            return dock;
        }

        void UpdateStyle(XmlNode xnode, ControlStyle ct)
        {
            ControlStyle copyFromStyle = GetControlStyle(GetStrParam(xnode, "copyfrom", null));
            if (copyFromStyle != null)
                ct.CopyFrom(copyFromStyle);
            ct.ForeColor = GetColorParam(xnode, "forecolor", ct.ForeColor);
            ct.BackColor = GetColorParam(xnode, "backcolor", ct.BackColor);
            ct.FrameColor = GetColorParam(xnode, "framecolor", ct.BackColor);
            ct.glMode = GetBoolParam(xnode, "gl", ct.glMode);
            ct.CheckedColor = GetColorParam(xnode, "checkcolor", ct.CheckedColor);
            ct.HoverColor = GetColorParam(xnode, "hovercolor", ct.HoverColor);
            ct.PressedColor = GetColorParam(xnode, "presscolor", ct.PressedColor);
            ct.SubImgCount = GetIntParam(xnode, "nimages", ct.SubImgCount);
            ct.BackImage = GetStrParam(xnode, "bgndimage", ct.BackImage);
            ct.CheckedImage = GetStrParam(xnode, "checkimage", ct.CheckedImage);
            ct.DisabledColor = GetColorParam(xnode, "disablecolor", ct.DisabledColor);
            ct.HoverSize = GetIntParam(xnode, "hoverscale", (int)ct.HoverSize);
            ct.PressedSize = GetIntParam(xnode, "pressscale", (int)ct.PressedSize);
            //ct.BgndImageName = GetStrParam(xnode, "bgndimage", ct.BgndImageName);
            ct.applySubControls = GetBoolParam(xnode, "applysubcontrols", ct.applySubControls);
            ct.applyWindowsControls = GetBoolParam(xnode, "applywincontrols", ct.applyWindowsControls);
            int[] sizes = GetIntArrayParam(xnode, "panelpad");
            if (sizes.Length >= 4)
            {
                ct.PanelPad.Left = sizes[0];
                ct.PanelPad.Right = sizes[1];
                ct.PanelPad.Top = sizes[2];
                ct.PanelPad.Bottom = sizes[3];
            }
            else if (sizes.Length >= 1)
            {
                ct.PanelPad.Left = sizes[0];
                ct.PanelPad.Right = sizes[0];
                ct.PanelPad.Top = sizes[0];
                ct.PanelPad.Bottom = sizes[0];
            }
        }

        #endregion

        #region Perform layout

        void Draw(List<DecorItem> dlist, C2DGraphics g2d, int w, int h)
        {
            foreach (DecorItem di in dlist)
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
            BgndDecorList = new List<DecorItem>();
            FgndDecorList = new List<DecorItem>();
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
    }
}
