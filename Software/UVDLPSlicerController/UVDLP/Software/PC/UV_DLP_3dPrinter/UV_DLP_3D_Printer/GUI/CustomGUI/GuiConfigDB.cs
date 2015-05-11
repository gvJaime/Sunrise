using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using UV_DLP_3D_Printer._3DEngine;
using UV_DLP_3D_Printer.Plugin;
using UV_DLP_3D_Printer.Util.Sequence;
using System.Diagnostics;
using Ionic.Zip;
using UV_DLP_3D_Printer.Configs;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{

    public enum GuiParamState
    {
        Unset = 0,
        Default,
        Inherited,
        Explicit
    }

    public abstract class CWParameter
    {
        public string paramName;
        public GuiParamState state;
        public bool defaultSet;
        public abstract Type ParamType { get; }
        public abstract void SaveUser(XmlDocument xd, XmlNode xnode);
        public static CWParameter LoadUser(XmlNode xnode)
        {
            CWParameter newPar = null;
            try
            {
                string defVal = null;
                try { defVal = xnode.Attributes["default"].Value; }
                catch { }
                string value = xnode.InnerText;
                newPar = CreateParameter(xnode.Name, xnode.Attributes["type"].Value, defVal, value);
            }
            catch
            {
                return null;
            }

            return newPar;
        }

        public static CWParameter CreateParameter(string name, string type, string defVal, string value)
        {
            CWParameter newPar = null;
            switch (type)
            {
                case "Int32": newPar = CreateIntParam(defVal, value); break;
                case "Color": newPar = CreateColorParam(defVal, value); break;
                case "Boolean": newPar = CreateBoolParam(defVal, value); break;
                case "String": newPar = CreateStringParam(defVal, value); break;
            }
            newPar.paramName = name;
            return newPar;
        }

        public static GuiParam<string> CreateStringParam(string defValue, string value)
        {
            GuiParam<string> par = new GuiParam<string>();
            if ((defValue != null) && (defValue.Length > 0))
                par.SetDefault(defValue);
            if ((value != null) && (value.Length > 0))
                par.SetValue(value);
            return par;
        }

        public static GuiParam<int> CreateIntParam(string defValue, string value)
        {
            GuiParam<int> par = new GuiParam<int>();
            if ((defValue != null) && (defValue.Length > 0))
                par.SetDefault(int.Parse(defValue));
            if ((value != null) && (value.Length > 0))
                par.SetValue(int.Parse(value));
            return par;
        }

        public static GuiParam<bool> CreateBoolParam(string defValue, string value)
        {
            GuiParam<bool> par = new GuiParam<bool>();
            if ((defValue != null) && (defValue.Length > 0))
                par.SetDefault(bool.Parse(defValue));
            if ((value != null) && (value.Length > 0))
                par.SetValue(bool.Parse(value));
            return par;
        }

        public static GuiParam<Color> CreateColorParam(string defValue, string value)
        {
            GuiParam<Color> par = new GuiParam<Color>();
            if ((defValue != null) && (defValue.Length > 0))
                par.SetDefault(GuiConfigDB.ParseColor(defValue));
            if ((value != null) && (value.Length > 0))
                par.SetValue(GuiConfigDB.ParseColor(value));
            return par;
        }
    }

    public class GuiParam<T> : CWParameter
    {
        T var;
        T defVal;
        public GuiParam<T> parrent;

        public GuiParam()
        {
            state = GuiParamState.Unset;
            parrent = null;
            paramName = null;
            defaultSet = false;
        }

        public override Type ParamType
        {
            get { return typeof(T); }
        }

        public void SetDefault(T defval)
        {
            state = GuiParamState.Default;
            defVal = defval;
            defaultSet = true;
            parrent = null;
        }

        public void InheritFrom(GuiParam<T> t)
        {
            parrent = t;
            state = GuiParamState.Inherited;
        }

        public GuiParam<T> GetCopy()
        {
            GuiParam<T> newPar = new GuiParam<T>();
            newPar.state = state;
            newPar.var = var;
            newPar.defVal = defVal;
            newPar.defaultSet = defaultSet;
            newPar.parrent = parrent;
            newPar.paramName = paramName;
            return newPar;
        }

        public GuiParam(T defval)
        {
            SetDefault(defval);
        }

        public T GetVal()
        {
            if (state == GuiParamState.Inherited && parrent != null)
                return parrent.GetVal();
            if (state == GuiParamState.Default)
                return defVal;
            return var;
        }

        public bool IsValid()
        {
            return state != GuiParamState.Unset;
        }

        public bool IsExplicit()
        {
            return state == GuiParamState.Explicit;
        }

        public T GetIfValid(T defvar)
        {
            if (IsValid())
                return GetVal();
            return defvar;
        }

        public T GetIfExplicit(T defvar)
        {
            if (IsExplicit())
                return var;
            return defvar;
        }

        public void Save(XmlDocument xd, XmlNode parent, string name)
        {
            if (state == GuiParamState.Explicit)
                GuiConfigDB.AddParameter(xd, parent, name, var);
        }

        // save general purpose user parameters
        public override void SaveUser(XmlDocument xd, XmlNode xnode)
        {
            XmlAttribute xa = xd.CreateAttribute("type");
            xa.Value = ParamType.Name;
            xnode.Attributes.Append(xa);

            if (defaultSet)
            {
                xa = xd.CreateAttribute("default");
                if (defVal is Color)
                    xa.Value = GuiConfigDB.ColorToString((Color)(Object)defVal);
                else
                    xa.Value = defVal.ToString();
                xnode.Attributes.Append(xa);
            }

            if (IsExplicit())
            {
                if (var is Color)
                    xnode.InnerText = GuiConfigDB.ColorToString((Color)(Object)var);
                else
                    xnode.InnerText = var.ToString();
            }
        }

        public void SetValue(T t)
        {
            var = t;
            state = GuiParamState.Explicit;
        }

        public static implicit operator GuiParam<T>(T t)
        {
            GuiParam<T> temp = new GuiParam<T>();
            temp.SetValue(t);
            return temp;
        }
        public static implicit operator T(GuiParam<T> t)
        {
            return t.GetVal();
        }

    }

    public abstract class GuiDecorItem
    {
        public abstract void Show(C2DGraphics g2d, int w, int h);
        public GuiParam<bool> visible = new GuiParam<bool>(true);
        public GuiParam<string> level = new GuiParam<string>("foreground");
        public GuiParam<string> name = new GuiParam<string>();
    }

    public class GuiDecorImage : GuiDecorItem
    {
        public GuiDecorImage()
        {
            this.imgname = new GuiParam<string>();
            this.docking = new GuiParam<string>("cc");
            this.x = new GuiParam<int>(0);
            this.y = new GuiParam<int>(0);
            this.col = new GuiParam<Color>(Color.White);
            this.opacity = new GuiParam<int>(-1);
        }

        public GuiDecorImage(string imgname, GuiParam<string> docking, GuiParam<int> x, GuiParam<int> y, GuiParam<Color> col, GuiParam<int> opacity)
        {
            this.imgname = imgname;
            this.docking = docking;
            this.x = x;
            this.y = y;
            this.col = col;
            this.opacity = opacity;
        }
        public GuiParam<string> imgname;
        public GuiParam<string> docking; // tl = top left, rc = right center, nn = no dock, etc
        public GuiParam<int> x, y;       // gap to edge when docked, absolute if not
        public GuiParam<int> opacity;
        public GuiParam<Color> col;

        Color color
        {
            get
            {
                Color res = col;
                if (opacity.IsExplicit())
                    return Color.FromArgb(opacity * 255 / 100, res.R, res.G, res.B);
                return res;
            }
        }

        public override void Show(C2DGraphics g2d, int w, int h)
        {
            if (!visible)
                return;
            int iw = 0, ih = 0;
            g2d.GetImageDim(imgname, ref iw, ref ih);
            int px = GuiConfigManager.GetPosition(0, w, iw, x, ((string)docking)[1]);
            int py = GuiConfigManager.GetPosition(0, h, ih, y, ((string)docking)[0]);
            g2d.SetColor(color);
            g2d.Image(imgname, px, py);
        }
    }

    public class GuiDecorBar : GuiDecorItem
    {
        public GuiDecorBar(bool isgrad)
        {
            this.docking = new GuiParam<string>("n");
            this.bw = new GuiParam<int>(100);
            this.coltl = new GuiParam<Color>(Color.White);
            this.coltr = new GuiParam<Color>(Color.White);
            this.colbl = new GuiParam<Color>(Color.White);
            this.colbr = new GuiParam<Color>(Color.White);
            this.isgrad = isgrad;
        }

        public GuiDecorBar(GuiParam<string> docking, GuiParam<int> w, GuiParam<Color> col) // solid bar
        {
            this.docking = docking;
            this.bw = w;
            coltl = col;
            isgrad = false;
        }

        public GuiDecorBar(GuiParam<string> docking, GuiParam<int> w, GuiParam<Color> coltl, GuiParam<Color> coltr, 
            GuiParam<Color> colbl, GuiParam<Color> colbr) // gradient bar
        {
            this.docking = docking;
            this.bw = w;
            this.coltl = coltl;
            this.coltr = coltr;
            this.colbl = colbl;
            this.colbr = colbr;
            isgrad = true;
        }

        public GuiParam<string> docking; // t = top, b = bottom, l = left, r = right, n = no dock (fullscreen)
        public GuiParam<int> bw;         // bar width
        public GuiParam<Color> coltl, coltr, colbl, colbr;
        public bool isgrad;
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
            if (isgrad)
                g2d.GradientRect(px, py, pw, ph, coltl, coltr, colbl, colbr);
            else
                g2d.GradientRect(px, py, pw, ph, coltl, coltl, coltl, coltl);
        }
    }

    public class GuiControlPad
    {
        public GuiParam<int> Left;
        public GuiParam<int> Right;
        public GuiParam<int> Top;
        public GuiParam<int> Bottom;

        public GuiControlPad()
        {
            Left = new GuiParam<int>();
            Right = new GuiParam<int>();
            Top = new GuiParam<int>();
            Bottom = new GuiParam<int>();
        }

        public GuiControlPad(int left, int right, int top, int bottom)
        {
            Left = new GuiParam<int>(left);
            Right = new GuiParam<int>(right);
            Top = new GuiParam<int>(top);
            Bottom = new GuiParam<int>(bottom);
        }

    }

    // NOTE!! any member added to this class should be added to SetDefault(), CopyFrom(), and InheritFrom() member functions.
    public class GuiControlStyle
    {
        public static GuiParam<Color> NullColor = new GuiParam<Color>();
        //public static Color DefaultColor = Color.FromArgb(2);

        public GuiControlStyle(string name, Color forecol, Color backcol)
        {
            Name = name;
            ForeColor = forecol;
            BackColor = backcol;
            FrameColor = NullColor;
            mBackImage = null;
        }

        public GuiControlStyle(string name)
        {
            Name = name;
            ForeColor = NullColor;
            BackColor = NullColor;
            FrameColor = NullColor;
            mBackImage = null;
        }

        public GuiConfigDB confDb = null;

        // general purpose control style
        public String Name;
        public GuiParam<Color> ForeColor;
        public GuiParam<Color> BackColor;
        public GuiParam<Color> FrameColor;
        public GuiParam<String> mBackImage;
        Image mBackImageCache = null;
        C9Patch mBackImage9PCache = null;
        public GuiParam<bool> glMode;

        // button styles
        public GuiParam<int> SubImgCount;
        public GuiParam<Color> PressedColor;
        public GuiParam<Color> CheckedColor;
        public GuiParam<Color> DisabledColor;
        public GuiParam<Color> HoverColor;
        public GuiParam<int> PressedSize;
        public GuiParam<int> HoverSize;
        //public GuiParam<string> BgndImageName;
        public GuiParam<String> mCheckedImage;
        public GuiParam<String> BorderShape;
        C2DImage mCheckedImageCacheGl;
        Image mCheckedImageCache = null;
        public GuiControlPad PanelPad;

        // misc
        public GuiParam<bool> applySubControls;
        public GuiParam<bool> applyWindowsControls;
        public GuiParam<string> inheritFrom;
        public GuiControlStyle parent;



        public virtual void SetDefault()
        {
            ForeColor = new GuiParam<Color>(Color.White);
            BackColor = new GuiParam<Color>(Color.RoyalBlue);
            FrameColor = new GuiParam<Color>(Color.Navy);
            CheckedColor = new GuiParam<Color>(Color.Orange);
            PressedColor = new GuiParam<Color>(Color.White);
            HoverColor = new GuiParam<Color>(Color.White);
            PressedSize = new GuiParam<int>(100);
            HoverSize = new GuiParam<int>(100);
            DisabledColor = new GuiParam<Color>(Color.FromArgb(60, 255, 255, 255));
            SubImgCount = new GuiParam<int>(1);
            glMode = new GuiParam<bool>(false);
            mBackImage = new GuiParam<string>();
            CheckedImage = new GuiParam<string>();
            BorderShape = new GuiParam<string>();
            inheritFrom = new GuiParam<string>();
            PanelPad = new GuiControlPad(10,10,10,10);
            applySubControls = new GuiParam<bool>(true);
            applyWindowsControls = new GuiParam<bool>(false);
            parent = null;
        }

        public void CopyFrom(GuiControlStyle sctl)
        {
            if (sctl == null)
                return;

            ForeColor = sctl.ForeColor.GetCopy();
            BackColor = sctl.BackColor.GetCopy();
            FrameColor = sctl.FrameColor.GetCopy();
            mBackImage = sctl.mBackImage.GetCopy();
            glMode = sctl.glMode.GetCopy();

            SubImgCount = sctl.SubImgCount.GetCopy();
            PressedColor = sctl.PressedColor.GetCopy();
            CheckedColor = sctl.CheckedColor.GetCopy();
            DisabledColor = sctl.DisabledColor.GetCopy();
            HoverColor = sctl.HoverColor.GetCopy();
            PressedSize = sctl.PressedSize.GetCopy();
            HoverSize = sctl.HoverSize.GetCopy();
            PanelPad = new GuiControlPad();
            PanelPad.Left = sctl.PanelPad.Left.GetCopy();
            PanelPad.Right = sctl.PanelPad.Right.GetCopy();
            PanelPad.Top = sctl.PanelPad.Top.GetCopy();
            PanelPad.Bottom = sctl.PanelPad.Bottom.GetCopy();
            applySubControls = sctl.applySubControls.GetCopy();
            applyWindowsControls = sctl.applyWindowsControls.GetCopy();
            CheckedImage = sctl.CheckedImage.GetCopy();
            BorderShape = sctl.BorderShape.GetCopy();
            inheritFrom = sctl.inheritFrom.GetCopy();
        }
 
        public void InheritFrom(GuiControlStyle sctl)
        {
            if (sctl == null)
                return;

            parent = sctl;
            inheritFrom = sctl.Name;
            ForeColor.InheritFrom(sctl.ForeColor);
            BackColor.InheritFrom(sctl.BackColor);
            FrameColor.InheritFrom(sctl.FrameColor);
            mBackImage.InheritFrom(sctl.mBackImage);
            glMode.InheritFrom(sctl.glMode);

            SubImgCount.InheritFrom(sctl.SubImgCount);
            PressedColor.InheritFrom(sctl.PressedColor);
            CheckedColor.InheritFrom(sctl.CheckedColor);
            DisabledColor.InheritFrom(sctl.DisabledColor);
            HoverColor.InheritFrom(sctl.HoverColor);
            PressedSize.InheritFrom(sctl.PressedSize);
            HoverSize.InheritFrom(sctl.HoverSize);
            PanelPad = new GuiControlPad();
            PanelPad.Left.InheritFrom(sctl.PanelPad.Left);
            PanelPad.Right.InheritFrom(sctl.PanelPad.Right);
            PanelPad.Top.InheritFrom(sctl.PanelPad.Top);
            PanelPad.Bottom.InheritFrom(sctl.PanelPad.Bottom);
            applySubControls.InheritFrom(sctl.applySubControls);
            applyWindowsControls.InheritFrom(sctl.applyWindowsControls);
            CheckedImage.InheritFrom(sctl.CheckedImage);
            BorderShape.InheritFrom(sctl.BorderShape);
        }

        public GuiParam<String> CheckedImage
        {
            get { return mCheckedImage; }
            set
            {
                mCheckedImage = value;
                mCheckedImageCacheGl = null;
                mCheckedImageCache = null;
            }
        }

        public C2DImage CheckedImageCacheGl
        {
            get
            {
                if (mCheckedImageCacheGl == null)
                    mCheckedImageCacheGl = UVDLPApp.Instance().m_2d_graphics.GetImage(mCheckedImage);
                return mCheckedImageCacheGl;
            }
        }

        public Image CheckedImageCache
        {
            get
            {
                if (((string)mCheckedImage != null) && (mCheckedImageCache == null) && (confDb != null))
                    mCheckedImageCache = confDb.GetImage(mCheckedImage);
                return mCheckedImageCache;
            }
        }

        public GuiParam<String> BackImage
        {
            get { return mBackImage; }
            set
            {
                mBackImage = value;
                mBackImageCache = null;
            }
        }

        public Image BackImageCache
        {
            get
            {
                if (((string)mBackImage != null) && (mBackImageCache == null) && (confDb != null))
                    mBackImageCache = confDb.GetImage(mBackImage);
                return mBackImageCache;
            }
        }

        public C9Patch BackImage9Patch
        {
            get
            {
                if ((mBackImage9PCache == null) && (BackImageCache != null) && ((string)mBackImage).StartsWith("9p"))
                    mBackImage9PCache = new C9Patch((Bitmap)BackImageCache);
                return mBackImage9PCache;
            }
        }
    }

    public class GuiControl
    {
        public string name;
        public GuiParam<string> style;
        public GuiParam<bool> visible;
        public GuiParam<int> x, y, w, h;
        public GuiParam<int> px, py;
        public GuiParam<string> dock;
        public GuiParam<string> action;
        public GuiParam<string> parent;
        public GuiParam<String> BorderShape;

        public GuiControl(string name)
        {
            this.name = name;
            parent = new GuiParam<string>();
            style = new GuiParam<string>();
            visible = new GuiParam<bool>(true);
            x = new GuiParam<int>();
            y = new GuiParam<int>();
            w = new GuiParam<int>();
            h = new GuiParam<int>();
            dock = new GuiParam<string>();
            action = new GuiParam<string>();
            BorderShape = new GuiParam<string>();
        }
    }

    public class GuiButton : GuiControl
    {
        //public string name;
        public GuiParam<string> image;
        public GuiParam<string> checkImage;
        public GuiParam<string> onClickCmd;

        public GuiButton(string name) : base(name)
        {
            image = new GuiParam<string>();
            checkImage = new GuiParam<string>();
            onClickCmd = new GuiParam<string>();
        }
    }
    /*
    public class CommandSequence
    {
        public enum CSType
        {
            gcode,          // G Code
            comstr,         // Serial text command
            comhex          // Serial hex string
        }
        public CSType type;
        public string name;
        public string sequence;
        public CommandSequence(string name, CSType type, string sequence)
        {
            this.name = name;
            this.type = type;
            this.sequence = sequence;
        }
    }
    */
    public class GuiLayout
    {
        public enum LayoutType
        {
            Layout,
            Menu,
            MenuItem,
            Panel,
            FlowPanel,
            SplitPanel,
            SplitPanel1,
            SplitPanel2,
            TabPanel,
            TabItem,
            Control
        }
        public string name;
        public LayoutType type;
        public List<GuiLayout> subLayouts;
        public GuiParam<string> text;
        public GuiParam<string> action;
        public GuiParam<int> px;
        public GuiParam<int> py;
        public GuiParam<int> w;
        public GuiParam<int> h;
        public GuiParam<string> dock;
        public GuiParam<string> control;
        public GuiParam<bool> isCollapsed;
        public GuiParam<bool> collapsible;
        public GuiParam<bool> isSelected;
        public GuiParam<string> image;
        public GuiParam<string> orientation;
        public GuiParam<string> direction;
        public GuiParam<int> splitPos;
        public GuiParam<string> style;

        public GuiLayout(LayoutType type, string name)
        {
            this.type = type;
            this.name = name;
            subLayouts = new List<GuiLayout>();
            text = new GuiParam<string>();
            action = new GuiParam<string>();
            px = new GuiParam<int>();
            py = new GuiParam<int>();
            w = new GuiParam<int>();
            h = new GuiParam<int>();
            dock = new GuiParam<string>();
            control = new GuiParam<string>();
            isCollapsed = new GuiParam<bool>();
            collapsible = new GuiParam<bool>();
            isSelected = new GuiParam<bool>();
            image = new GuiParam<string>();
            orientation = new GuiParam<string>();
            splitPos = new GuiParam<int>();
            direction = new GuiParam<string>();
        }
   }

    public class GuiConfigDB
    {
        const int FILE_VERSION = 1;
       // public enum EntityType { Buttons, Panels, Decals } // not used

        //Dictionary<String, ctlUserPanel> Controls;
        /* move to manager
        Dictionary<String, Control> Controls;
        Dictionary<String, ctlImageButton> Buttons;
         */
        public Dictionary<String, GuiControlStyle> GuiControlStylesDict;
        public Dictionary<String, GuiDecorItem> GuiDecorItemsDict;
        public List<GuiControlStyle> GuiControlStyles;
        public List<GuiControlStyle> GuiButtonStyles;
        public List<GuiLayout> GuiLayouts;
        public Dictionary<String, GuiControl> GuiControlsDict;
        public Dictionary<String, GuiButton> GuiButtonsDict;
        public Dictionary<String, Image> GuiUserImages;
        //List<GuiControl> GuiControls;
        //List<GuiButton> GuiButtons;
        public List<CommandSequence> CmdSequenceList;
        ResourceManager Res; // the resource manager for the main CW application
        IPlugin Plugin;
        Control mTopLevelControl = null;
        public GuiControlStyle DefaultControlStyle;
        public GuiParam<bool> HideAllButtons;
        public GuiParam<bool> HideAllControls;
        public GuiParam<bool> HideAllDecals;
        Dictionary<string, int> NameGenerator;
        public UserParameterList m_userparms;
        public bool PreviewMode;


        public GuiConfigDB()
        {
            //Controls = new Dictionary<string, ctlUserPanel>();
            /* move to manager
            Controls = new Dictionary<string, Control>();
            Buttons = new Dictionary<string, ctlImageButton>();
            */
            
            GuiControlStylesDict = new Dictionary<string, GuiControlStyle>();
            GuiDecorItemsDict = new Dictionary<string,GuiDecorItem>();

            GuiControlStyles = new List<GuiControlStyle>();
            GuiButtonStyles = new List<GuiControlStyle>();

            GuiControlsDict = new Dictionary<string, GuiControl>();
            GuiButtonsDict = new Dictionary<string, GuiButton>();
            GuiUserImages = null;
            //GuiControls = new List<GuiControl>();
            //GuiButtons = new List<GuiButton>();
            //BgndDecorList = new List<GuiDecorItem>();
            //FgndDecorList = new List<GuiDecorItem>();
            CmdSequenceList = new List<CommandSequence>();
            
            GuiLayouts = new List<GuiLayout>();

            NameGenerator = new Dictionary<string, int>();

            Res = global::UV_DLP_3D_Printer.Properties.Resources.ResourceManager;
            Plugin = null;
            HideAllButtons = new GuiParam<bool>();
            HideAllControls = new GuiParam<bool>();
            HideAllDecals = new GuiParam<bool>();

            PreviewMode = false;
            m_userparms = new UserParameterList();
        }

        public List<GuiDecorItem> BgndDecorList
        {
            get
            {
                return GetDecorListByLevel("background");
            }
        }
        public List<GuiDecorItem> FgndDecorList
        {
            get
            {
                return GetDecorListByLevel("foreground");
            }
        }

        public List<GuiDecorItem> GetDecorListByLevel(string level)
        {
            List<GuiDecorItem> lst = new List<GuiDecorItem>();
            foreach (KeyValuePair<string, GuiDecorItem> pair in GuiDecorItemsDict)
            {
                if ((string)pair.Value.level == level)
                    lst.Add(pair.Value);
            }
            return lst;
        }

        public Control TopLevelControl
        {
            get { return mTopLevelControl; }
            set { mTopLevelControl = value; }
        }

        public GuiControlStyle GetControlStyle(string name)
        {
            if ((name == null) || !GuiControlStylesDict.ContainsKey(name))
                return null;
            return GuiControlStylesDict[name];
        }


        public GuiDecorItem GetDecorItem(string name)
        {
            if (!GuiDecorItemsDict.ContainsKey(name))
                return null;
            return GuiDecorItemsDict[name];
        }

        public string GetUniqueName(string prefix)
        {
            if (NameGenerator.ContainsKey(prefix))
            {
                NameGenerator[prefix]++;
                return prefix + NameGenerator[prefix].ToString();
            }
            NameGenerator[prefix] = 1;
            return prefix + "1";
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
                        case "layouts": HandleLayouts(xnode); break;
                        case "userparams": HandleUserParams(xnode); break;
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

        public void LoadConfigFromFile(string fname)
        {
            StreamReader streamReader = new StreamReader(fname);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            LoadConfiguration(text);
        }

        void LoadUserParms(XmlNode usrprmnode) 
        {
            //

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
            try
            {
                //get name
                GuiParam<string> name = GetStrParam(seqnode, "name", "");
                if (!name.IsExplicit())
                {
                    DebugLogger.Instance().LogWarning("Sequence must have a name in GUIConfig");
                    return;
                }
                //get sequence
                string seq = GetStrParam(seqnode, "seqdata", "");
                //get type
                string seqtype = GetStrParam(seqnode, "seqtype", "");
                if (seqtype.ToLower().Equals("gcode"))
                {                    
                    CommandSequence gcs = new GCodeSequence(name, seq);
                    CmdSequenceList.Add(gcs);
                }
                if (seqtype.ToLower().Equals("process"))
                {
                    // no sequence string - this is handled by individual sequence entris for processes
                    ProcessSequence psc = new ProcessSequence(name);
                    CmdSequenceList.Add(psc);
                    //read other variables from xml...
                    //read processentry nodes
                    foreach (XmlNode nd in seqnode)
                    {
                        switch (nd.Name.ToLower())
                        {
                            case "processentry":
                                ProcessEntry pe = new ProcessEntry();
                                pe.args = GetStrParam(nd, "args", "");
                                pe.processname = GetStrParam(nd, "processname", "");
                                pe.wait = GetBoolParam(nd, "waitfor", false);
                                pe.windowstyle = GetStrParam(nd, "windowstyle", "normal");
                                psc.m_entries.Add(pe);
                                break;
                        }
                    }
                }
                else
                {
                    DebugLogger.Instance().LogWarning("Unknown sequence type " + seqtype + " in GUIConfig");
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public List<string> GetCmdSequenceNames()
        {
            List<string> res = new List<string>();
            foreach (CommandSequence gcs in CmdSequenceList)
                res.Add(gcs.m_name);
            return res;
        }

        #endregion

        #region Decals

        string GenerateDecalName(string basename)
        {
            int i = 1;
            while (GuiDecorItemsDict.ContainsKey(basename + i.ToString()))
                i++;
            return basename + i.ToString();
        }

        void HandleDecals(XmlNode decalnode)
        {
            HideAllDecals = GetBoolParam(decalnode, "HideAll", false);
            foreach (XmlNode xnode in decalnode.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "bar": HandleBars(xnode); break;
                    case "image": HandleImages(xnode); break;
                }
            }
        }

        /*List<GuiDecorItem> GetListFromLevel(XmlNode xnode)
        {
            List<GuiDecorItem> dlist = FgndDecorList;
            if (GetStrParam(xnode, "level", "") == "background")
            {
                dlist = BgndDecorList;
            }
            return dlist;
        }*/


        void HandleBars(XmlNode barnode)
        {
            GuiParam<string> docking = GetStrParam(barnode, "dock", "n");
            GuiParam<int> w = GetIntParam(barnode, "width", 100);
            string name = GetStrParam(barnode, "name", null);
            GuiDecorBar dcr = null;
            if (!GetStrParam(barnode, "color", null).IsExplicit())
            {
                Color coltl = GetColorParam(barnode, "tlcolor", Color.White);
                Color coltr = GetColorParam(barnode, "trcolor", Color.White);
                Color colbl = GetColorParam(barnode, "blcolor", Color.White);
                Color colbr = GetColorParam(barnode, "brcolor", Color.White);
                dcr = new GuiDecorBar(docking, w, coltl, coltr, colbl, colbr);
            }
            else
            {
                Color col = GetColorParam(barnode, "color", Color.White);
                dcr = new GuiDecorBar(docking, w, col);
            }
            dcr.level = GetStrParam(barnode, "level", "foreground");
            if (name == null)
                name = GenerateDecalName(dcr.isgrad ? "Gradient" : "Bar");
            dcr.name = name;
            GuiDecorItemsDict[name] = dcr;
        }

        void HandleImages(XmlNode imgnode)
        {
            string imgname = GetStrParam(imgnode, "image", null);
            if (imgname == null)
                return;
            string name = GetStrParam(imgnode, "name", null);
            if (name == null)
                name = GenerateDecalName("Image");
            GuiParam<string> docking = GetDockingParam(imgnode, "dock", "cc");
            GuiParam<int> x = GetIntParam(imgnode, "x", 0);
            GuiParam<int> y = GetIntParam(imgnode, "y", 0);
            GuiParam<Color> col = GetColorParam(imgnode, "color", Color.White);
            GuiParam<int> opacity = GetIntParam(imgnode, "opacity", -1);
            GuiDecorItem dcr = new GuiDecorImage(imgname, docking, x, y, col, opacity);
            dcr.level = GetStrParam(imgnode, "level", "foreground");
            dcr.name = name;
            GuiDecorItemsDict[name] = dcr;
        }
        #endregion

        #region Buttons
        void HandleButtons(XmlNode buttnode)
        {
            /* move to manager
            if (GetBoolParam(buttnode, "HideAll", false))
                HideAllButtons();
             */
            HideAllButtons = GetBoolParam(buttnode, "HideAll", false);
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
            GuiControlStyle bt = GetControlStyle(name);
            if (bt == null)
            {
                bt = new GuiControlStyle(name);
                GuiControlStylesDict[name] = bt;
                GuiButtonStyles.Add(bt);
                bt.SetDefault();
            }
            bt.confDb = this;
            UpdateStyle(xnode, bt);

            /* move to manager
            if (name == "DefaultButton")
            {
                foreach (KeyValuePair<String, ctlImageButton> pair in Buttons)
                {
                    ctlImageButton butt = pair.Value;
                    butt.ApplyStyle(bt);
                }
            }
             */
        }

        void HandleButton(XmlNode buttnode)
        {
            string name = GetStrParam(buttnode, "name", null);
            if (name == null)
                return;
            /* move to manager
            if (!Buttons.ContainsKey(name))
            {
                // create a new empty button
                AddButton(name, new ctlImageButton());
                Buttons[name].BringToFront();
            }
             */
            GuiButton butt = null;
            if (!GuiButtonsDict.ContainsKey(name))
            {
                butt = new GuiButton(name);
                //butt.name = name;
                GuiButtonsDict[name] = butt;
            }
            else
                butt = GuiButtonsDict[name];

            //ctlImageButton butt = Buttons[name];
//            butt.Visible = true;
            butt.visible = GetBoolParam(buttnode, "visible", true);
            butt.dock = GetDockingParam(buttnode, "dock", "cc");
            butt.x = GetIntParam(buttnode, "x", 0);
            butt.y = GetIntParam(buttnode, "y", 0);
            butt.w = GetIntParam(buttnode, "w", 0);
            butt.h = GetIntParam(buttnode, "h", 0);
            butt.style = GetStrParam(buttnode, "style", null);
            butt.onClickCmd = GetStrParam(buttnode, "click", null);
            /* move to manager
            GuiControlStyle bstl = GetControlStyle(butt.StyleName);
            if (bstl != null)
            {
                butt.GLVisible = bstl.glMode;
            }
            //butt.GLVisible = GetBoolParam(buttnode, "gl", butt.GLVisible);
             */

            butt.image = GetStrParam(buttnode, "image", null);
            /* move to manager
            if (imgname != null)
            {
                butt.GLImage = imgname;
                butt.Image = GetImageParam(buttnode, "image", null);
            }
             
            butt.CheckImage = GetImageParam(buttnode, "check", butt.CheckImage);
            */
            butt.checkImage = GetStrParam(buttnode, "check", null);
            butt.action = GetStrParam(buttnode, "action", null);
            butt.parent = GetStrParam(buttnode, "parent", null);

            /* move to manager
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
             */
        }

        public List<string> GetButtonNames()
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, GuiButton> pair in GuiButtonsDict)
                res.Add(pair.Key);
            return res;
        }

        public List<string> GetButtonStyleNames()
        {
            List<string> res = new List<string>();
            foreach (GuiControlStyle gcs in GuiButtonStyles)
                res.Add(gcs.Name);
            return res;
        }

        public List<string> GetGuiDecorImageNames()
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, GuiDecorItem> pair in GuiDecorItemsDict)
            {
                if (pair.Value is GuiDecorImage)
                    res.Add((string)pair.Value.name);
            }
            return res;
        }

        public List<string> GetGuiDecorBarNames(bool isgrad)
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, GuiDecorItem> pair in GuiDecorItemsDict)
            {
                if ((pair.Value is GuiDecorBar) && (((GuiDecorBar)pair.Value).isgrad == isgrad))
                    res.Add((string)pair.Value.name);
            }
            return res;
        }

        #endregion

        #region Controls
        void HandleControls(XmlNode ctlnode)
        {
            /* move to manager
            if (GetBoolParam(ctlnode, "HideAll", false))
                HideAllControls();
             **/
            HideAllControls = GetBoolParam(ctlnode, "HideAll", false);
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
            GuiControlStyle ct = GetControlStyle(name);
            if (ct == null)
            {
                ct = new GuiControlStyle(name);
                GuiControlStylesDict[name] = ct;
                GuiControlStyles.Add(ct);
                ct.SetDefault();
            }
            ct.confDb = this;
            UpdateStyle(xnode, ct);

            /* move to manager
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
             */
        }
        #region style applying for non-ctlUsercontrol controls
        /* move to manager
        public void ApplyStyleRecurse(Control ctl, GuiControlStyle ct)
        {

            if ((ctl is ctlUserPanel) || ct.applyWindowsControls)
            {
                if (ct.BackColor != GuiControlStyle.NullColor)
                    ctl.BackColor = ct.BackColor;

                if (ct.ForeColor != GuiControlStyle.NullColor)
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
        */

        #endregion

        void HandleControl(XmlNode ctlnode)
        {
            string name = GetStrParam(ctlnode, "name", null);
            if (name == null)
                return;
            GuiControl ct = null;
            if (!GuiControlsDict.ContainsKey(name))
            {
                ct = new GuiControl(name);
                ct.name = name;
                GuiControlsDict[name] = ct;
            }
            else
                ct = GuiControlsDict[name];

            //ctlUserPanel ctl = Controls[name];
            /* move to manager 
             Control ct = Controls[name];
            if (ctlnode.Attributes.GetNamedItem("visible") != null)
                ct.Visible = GetBoolParam(ctlnode, "visible", ct.Visible);
             * */
            ct.visible = GetBoolParam(ctlnode, "visible", true);
            ct.x = GetIntParam(ctlnode, "x", 0);
            ct.y = GetIntParam(ctlnode, "y", 0);
            ct.w = GetIntParam(ctlnode, "w", 0);
            ct.h = GetIntParam(ctlnode, "h", 0);
            //load some control locations as well,            
            ct.px = GetIntParam(ctlnode, "px", 0);
            ct.py = GetIntParam(ctlnode, "py", 0);
            /* move to manager
            Point pt = new Point(px,py);
            ct.Location = pt;
            */
            // load docking style
            ct.dock = GetDockingParam(ctlnode, "dock", "cc");
            
            ct.action = GetStrParam(ctlnode, "action", null);
            ct.parent = GetStrParam(ctlnode, "parent", null);
            /* move to manager
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
             */

            ct.style = GetStrParam(ctlnode, "style", null);
            ct.BorderShape = GetStrParam(ctlnode, "shape", null);
            /* move to manager
            String styleName = GetStrParam(ctlnode, "style", null);
            GuiControlStyle style = GetControlStyle(styleName);
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
             * */
        }

        public List<string> GetControlNames()
        {
            List<string> res = new List<string>();
            foreach (KeyValuePair<string, GuiControl> pair in GuiControlsDict)
                res.Add(pair.Key);
            return res;
        }

        public List<string> GetControlStyleNames()
        {
            List<string> res = new List<string>();
            foreach (GuiControlStyle gcs in GuiControlStyles)
                res.Add(gcs.Name);
            return res;
        }
        #endregion

        #region Layouts
        void HandleLayouts(XmlNode lnode)
        {
            foreach (XmlNode xnode in lnode.ChildNodes)
            {
                switch (xnode.Name.ToLower())
                {
                    case "layout": HandleLayout(xnode); break;
                }
            }

        }

        void HandleLayout(XmlNode lnode)
        {
            GuiLayout gl = HandleLayoutRecurse(lnode);
            if (gl != null)
            {
                for (int i=0; i<GuiLayouts.Count; i++)
                {
                    if (GuiLayouts[i].name == gl.name)
                    {
                        GuiLayouts[i] = gl;
                        return;
                    }
                }
                GuiLayouts.Add(gl);
            }
        }

        GuiLayout HandleLayoutRecurse(XmlNode lnode)
        {
            GuiLayout.LayoutType ltype;
            try
            {
                ltype = (GuiLayout.LayoutType)Enum.Parse(typeof(GuiLayout.LayoutType), lnode.Name, true);
            }
            catch (Exception)
            {
                return null;
            }
            string name;
            GuiParam<string> gpname = GetStrParam(lnode, "name", null);
            if (gpname.IsExplicit())
                name = gpname;
            else
                name = GetUniqueName(ltype.ToString());
            GuiLayout gl = new GuiLayout(ltype, name);
            gl.text = GetStrParam(lnode, "text", null);
            gl.action = GetStrParam(lnode, "action", null);
            gl.dock = GetStrParam(lnode, "dock", null);
            gl.control = GetStrParam(lnode, "control", null);
            gl.image = GetStrParam(lnode, "image", null);
            gl.orientation = GetStrParam(lnode, "orientation", null);
            gl.direction = GetStrParam(lnode, "direction", null);
            gl.px = GetIntParam(lnode, "px", 0);
            gl.py = GetIntParam(lnode, "py", 0);
            gl.w = GetIntParam(lnode, "w", 0);
            gl.h = GetIntParam(lnode, "h", 0);
            gl.splitPos = GetIntParam(lnode, "splitpos", 0);
            gl.isCollapsed = GetBoolParam(lnode, "iscollapsed", false);
            gl.collapsible = GetBoolParam(lnode, "collapsible", false);
            gl.isSelected = GetBoolParam(lnode, "isselected", false);
            gl.style = GetStrParam(lnode, "style", null);
            foreach (XmlNode subnode in lnode.ChildNodes)
            {
                GuiLayout subgl = HandleLayoutRecurse(subnode);
                if (subgl != null)
                    gl.subLayouts.Add(subgl);
            }

            return gl;
        }

        #endregion


        #region UserParameters
        void HandleUserParams(XmlNode upnode)
        {
            foreach (XmlNode xnode in upnode.ChildNodes)
            {
                CWParameter param = CWParameter.LoadUser(xnode);
                m_userparms.paramDict[param.paramName] = param;
            }
        }

        #endregion

        #region Attribute parsing
        public static GuiParam<string> GetStrParam(XmlNode xnode, string paramName, object defVal)
        {
            GuiParam<string> res;
            if (defVal is GuiParam<string>)
                res = (GuiParam<string>)defVal;
            else
                res = new GuiParam<string>((string)defVal);
            try
            {
                res = xnode.Attributes[paramName].Value;
            }
            catch (Exception) { }
            return res;
        }

        public static GuiParam<int> GetIntParam(XmlNode xnode, string paramName, object defVal)
        {
            GuiParam<int> res;
            if (defVal is GuiParam<int>)
                res = (GuiParam<int>)defVal;
            else
                res = new GuiParam<int>((int)defVal);
            try
            {
                res = int.Parse(xnode.Attributes[paramName].Value);
            }
            catch (Exception) { }
            return res;
        }

        public static int[] GetIntArrayParam(XmlNode xnode, string paramName)
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

        public static GuiParam<bool> GetBoolParam(XmlNode xnode, string paramName, object defVal)
        {
            GuiParam<bool> res;
            if (defVal is GuiParam<bool>)
                res = (GuiParam<bool>)defVal;
            else
                res = new GuiParam<bool>((bool)defVal);
            try
            {
                res = bool.Parse(xnode.Attributes[paramName].Value);
            }
            catch (Exception) { }
            return res;
        }

        public static GuiParam<Color> GetColorParam(XmlNode xnode, string paramName, object defVal)
        {
            GuiParam<Color> res;
            if (defVal is GuiParam<Color>)
                res = (GuiParam<Color>)defVal;
            else
                res = new GuiParam<Color>((Color)defVal);
            try
            {
                string sres = xnode.Attributes[paramName].Value;
                if (sres == "null")
                {
                    res.state = GuiParamState.Unset;
                }
                else if (sres == "default")
                {
                    res.state = GuiParamState.Default;
                }
                else
                {
                    res = ParseColor(sres);
                }
            }
            catch (Exception) { }
            return res;
        }

        public static Color ParseColor(string cname)
        {
            Color res;
            if (cname[0] == '#')
            {
                cname = cname.Substring(1);
                if (cname.Length > 7)
                    res = Color.FromArgb(int.Parse(cname, System.Globalization.NumberStyles.HexNumber));
                else
                    res = Color.FromArgb((int)(long.Parse(cname, System.Globalization.NumberStyles.HexNumber) | 0xFF000000));
            }
            else
            {
                res = Color.FromName(cname);
                if (res.ToArgb() == 0)
                    throw new Exception("Bad color name");
            }
            return res;
        }

        public List<string> GetAllImageNames()
        {
            List<string> imnames = UVDLPApp.Instance().m_2d_graphics.GetImageNames();
            ResourceSet resourceSet = Res.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                if (entry.Value.GetType() == typeof(Bitmap))
                    imnames.Add(resourceKey);
            }            
            return imnames;
        }

        public void AddUserImage(string name, Image img)
        {
            if (GuiUserImages == null)
                GuiUserImages = new Dictionary<string, Image>();
            GuiUserImages[name] = img;
        }

        public Image GetImage(string imageName)
        {
            Image img = null;
            if (Plugin != null)
                img = Plugin.GetImage(imageName);
            if ((GuiUserImages != null) && (GuiUserImages.ContainsKey(imageName)))
                img = GuiUserImages[imageName];
            if (img == null) // try to get from the 2d graphics first
                img = UVDLPApp.Instance().m_2d_graphics.GetBitmap(imageName);
            if (img == null)
                img = (Image)Res.GetObject(imageName);
            return img;
        }

        public Image GetImage(GuiParam<string> imageName, Image defVal)
        {
            if (!imageName.IsExplicit())
                return defVal;
            Image img = GetImage(imageName);
            if (img == null)
                return defVal;
            return img;
        }

        public string FixDockingVal(string origdock)
        {
            if (origdock == null)
                return "cc";
            string dock = origdock.ToLower();
            while (dock.Length < 2)
                dock += "c";
            return dock;
        }

        GuiParam<string> GetDockingParam(XmlNode xnode, string paramName, object defVal)
        {
            GuiParam<string> res;
            if (defVal is GuiParam<string>)
                res = (GuiParam<string>)defVal;
            else
                res = new GuiParam<string>((string)defVal);
            try
            {
                string dock = xnode.Attributes[paramName].Value;
                res = FixDockingVal(dock);
            }
            catch (Exception) { }
            return res;
        }

        void UpdateStyle(XmlNode xnode, GuiControlStyle ct)
        {
            ct.inheritFrom = GetStrParam(xnode, "copyfrom", null);
            if (ct.inheritFrom.IsExplicit())
            {
                GuiControlStyle inheritFromStyle = GetControlStyle(ct.inheritFrom);
                if (inheritFromStyle != null)
                    ct.InheritFrom(inheritFromStyle);
            }
        
            ct.ForeColor = GetColorParam(xnode, "forecolor", ct.ForeColor);
            ct.BackColor = GetColorParam(xnode, "backcolor", ct.BackColor);
            ct.FrameColor = GetColorParam(xnode, "framecolor", ct.FrameColor);
            ct.glMode = GetBoolParam(xnode, "gl", ct.glMode);
            ct.CheckedColor = GetColorParam(xnode, "checkcolor", ct.CheckedColor);
            ct.HoverColor = GetColorParam(xnode, "hovercolor", ct.HoverColor);
            ct.PressedColor = GetColorParam(xnode, "presscolor", ct.PressedColor);
            ct.SubImgCount = GetIntParam(xnode, "nimages", ct.SubImgCount);
            ct.BackImage = GetStrParam(xnode, "bgndimage", ct.BackImage);
            ct.CheckedImage = GetStrParam(xnode, "checkimage", ct.CheckedImage);
            ct.DisabledColor = GetColorParam(xnode, "disablecolor", ct.DisabledColor);
            ct.HoverSize = GetIntParam(xnode, "hoverscale", ct.HoverSize);
            ct.PressedSize = GetIntParam(xnode, "pressscale", ct.PressedSize);
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

        #region Save configuration
        public void SaveConfiguration(string fileName)
        {
            XmlDocument xd = CreateConfigXml();
            /*if (Plugin != null)
                fileName += "_" + Plugin.Name + ".xml";
            else
                fileName += ".xml";*/
            try
            {
                xd.Save(fileName);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError("Unable to save GUI configuration: " + ex.Message);
            }
        }

        public void SaveConfiguration(MemoryStream ms)
        {
            XmlDocument xd = CreateConfigXml();
            /*if (Plugin != null)
                fileName += "_" + Plugin.Name + ".xml";
            else
                fileName += ".xml";*/
            try
            {
                xd.Save(ms);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError("Unable to save GUI configuration: " + ex.Message);
            }
        }

        XmlDocument CreateConfigXml()
        {
            XmlDocument xd = new XmlDocument();
            xd.AppendChild(xd.CreateXmlDeclaration("1.0", "utf-8", ""));
            XmlNode toplevel = xd.CreateElement("GuiConfig");
            XmlAttribute verattr = xd.CreateAttribute("FileVersion");
            verattr.Value = FILE_VERSION.ToString();
            toplevel.Attributes.Append(verattr);
            xd.AppendChild(toplevel);
            SaveButtons(xd, toplevel);
            SaveControls(xd, toplevel);
            SaveDecals(xd, toplevel);
            SaveLayouts(xd, toplevel);
            SaveSequences(xd, toplevel);
            SaveUserParams(xd, toplevel);
            return xd;
        }

        void SaveStyle(XmlDocument xd, XmlNode xnode, GuiControlStyle stl)
        {
            stl.inheritFrom.Save(xd, xnode, "copyfrom");
            stl.ForeColor.Save(xd, xnode, "forecolor");
            stl.BackColor.Save(xd, xnode, "backcolor");
            stl.FrameColor.Save(xd, xnode, "framecolor");
            stl.glMode.Save(xd, xnode, "gl");
            stl.CheckedColor.Save(xd, xnode, "checkcolor");
            stl.HoverColor.Save(xd, xnode, "hovercolor");
            stl.PressedColor.Save(xd, xnode, "presscolor");
            stl.SubImgCount.Save(xd, xnode, "nimages");
            stl.BackImage.Save(xd, xnode, "bgndimage");
            stl.CheckedImage.Save(xd, xnode, "checkimage");
            stl.DisabledColor.Save(xd, xnode, "disablecolor");
            stl.HoverSize.Save(xd, xnode, "hoverscale");
            stl.PressedSize.Save(xd, xnode, "pressscale");
            stl.applySubControls.Save(xd, xnode, "applysubcontrols");
            stl.applyWindowsControls.Save(xd, xnode, "applywincontrols");
            if (stl.PanelPad.Left.IsExplicit() && stl.PanelPad.Right.IsExplicit()
                && stl.PanelPad.Top.IsExplicit() && stl.PanelPad.Bottom.IsExplicit())
            {
                if ((int)stl.PanelPad.Left == (int)stl.PanelPad.Right && (int)stl.PanelPad.Left == (int)stl.PanelPad.Top
                    && (int)stl.PanelPad.Left == (int)stl.PanelPad.Bottom)
                {
                    AddParameter(xd, xnode, "panelpad", (int)stl.PanelPad.Left);
                }
                else
                {
                    AddParameter(xd, xnode, "panelpad", string.Format("{0},{1},{2},{3}", (int)stl.PanelPad.Left, (int)stl.PanelPad.Right,
                        (int)stl.PanelPad.Top, (int)stl.PanelPad.Bottom));
                }
            }
        }

        void SaveControlStyles(XmlDocument xd, XmlNode parent)
        {
            foreach (GuiControlStyle stl in GuiControlStyles)
            {
                XmlNode cstyle = xd.CreateElement("style");
                parent.AppendChild(cstyle);
                if (stl.Name != "DefaultControl")
                    AddParameter(xd, cstyle, "name", stl.Name);
                SaveStyle(xd, cstyle, stl);
            }
        }

        void SaveControlDefs(XmlDocument xd, XmlNode parent)
        {
            foreach (KeyValuePair<string, GuiControl> pair in GuiControlsDict)
            {
                GuiControl ct = pair.Value;
                XmlNode cnode = xd.CreateElement("control");
                parent.AppendChild(cnode);
                AddParameter(xd, cnode, "name", ct.name);
                ct.visible.Save(xd, cnode, "visible");
                ct.x.Save(xd, cnode, "x");
                ct.y.Save(xd, cnode, "y");
                ct.w.Save(xd, cnode, "w");
                ct.h.Save(xd, cnode, "h");
                ct.px.Save(xd, cnode, "px");
                ct.py.Save(xd, cnode, "py");
                ct.action.Save(xd, cnode, "action");
                ct.parent.Save(xd, cnode, "parent");
                ct.style.Save(xd, cnode, "style");
            }
        }

        void SaveControls(XmlDocument xd, XmlNode parent)
        {
            XmlNode controlssNode = xd.CreateElement("controls");
            parent.AppendChild(controlssNode);
            HideAllControls.Save(xd, controlssNode, "HideAll");
            SaveControlStyles(xd, controlssNode);
            SaveControlDefs(xd, controlssNode);
        }

        void SaveButtonStyles(XmlDocument xd, XmlNode parent)
        {
            foreach (GuiControlStyle stl in GuiButtonStyles)
            {
                XmlNode bstyle = xd.CreateElement("style");
                parent.AppendChild(bstyle);
                if (stl.Name != "DefaultButton")
                    AddParameter(xd, bstyle, "name", stl.Name);
                SaveStyle(xd, bstyle, stl);
            }
        }

        void SaveButtonDefs(XmlDocument xd, XmlNode parent)
        {
            foreach (KeyValuePair<string, GuiButton> pair in GuiButtonsDict)
            {
                GuiButton butt = pair.Value;
                XmlNode bnode = xd.CreateElement("button");
                parent.AppendChild(bnode);
                AddParameter(xd, bnode, "name", butt.name);
                butt.visible.Save(xd, bnode, "visible");
                butt.dock.Save(xd, bnode, "dock");
                butt.x.Save(xd, bnode, "x");
                butt.y.Save(xd, bnode, "y");
                butt.w.Save(xd, bnode, "w");
                butt.h.Save(xd, bnode, "h");
                butt.style.Save(xd, bnode, "style");
                butt.onClickCmd.Save(xd, bnode, "click");
                butt.image.Save(xd, bnode, "image");
                butt.checkImage.Save(xd, bnode, "check");
                butt.action.Save(xd, bnode, "action");
                butt.parent.Save(xd, bnode, "parent");
            }
        }

        void SaveButtons(XmlDocument xd, XmlNode parent)
        {
            XmlNode buttunsNode = xd.CreateElement("buttons");
            parent.AppendChild(buttunsNode);
            HideAllButtons.Save(xd, buttunsNode, "HideAll");
            SaveButtonStyles(xd, buttunsNode);
            SaveButtonDefs(xd, buttunsNode);
        }

        void SaveBar(XmlDocument xd, XmlNode parent, GuiDecorBar db)
        {
            XmlNode dbnode = xd.CreateElement("bar");
            parent.AppendChild(dbnode);
            db.docking.Save(xd, dbnode, "dock");
            db.bw.Save(xd, dbnode, "width");
            db.name.Save(xd, dbnode, "name");
            if (db.isgrad)
            {
                db.coltl.Save(xd, dbnode, "tlcolor");
                db.coltr.Save(xd, dbnode, "trcolor");
                db.colbl.Save(xd, dbnode, "blcolor");
                db.colbr.Save(xd, dbnode, "brcolor");
            }
            else
            {
                db.coltl.Save(xd, dbnode, "color");
            }
            db.level.Save(xd, dbnode, "level");
        }

        void SaveImage(XmlDocument xd, XmlNode parent, GuiDecorImage di)
        {
            XmlNode dinode = xd.CreateElement("image");
            parent.AppendChild(dinode);
            di.imgname.Save(xd, dinode, "image");
            di.name.Save(xd, dinode, "name");
            di.docking.Save(xd, dinode, "dock");
            di.x.Save(xd, dinode, "x");
            di.y.Save(xd, dinode, "y");
            di.col.Save(xd, dinode, "color");
            di.opacity.Save(xd, dinode, "opacity");
            di.level.Save(xd, dinode, "level");
       }

        void SaveDecals(XmlDocument xd, XmlNode parent)
        {
            XmlNode decNode = xd.CreateElement("decals");
            parent.AppendChild(decNode);
            HideAllDecals.Save(xd, decNode, "HideAll");
            foreach (KeyValuePair<string, GuiDecorItem> pair in GuiDecorItemsDict)
            {
                GuiDecorItem dec = pair.Value;
                if (dec is GuiDecorBar)
                    SaveBar(xd, decNode, (GuiDecorBar)dec);
                else if (dec is GuiDecorImage)
                    SaveImage(xd, decNode, (GuiDecorImage)dec);
            }
        }
        
        void SaveSequences(XmlDocument xd, XmlNode parent)
        {
            XmlNode decNode = xd.CreateElement("sequences");            
            parent.AppendChild(decNode);
            foreach (CommandSequence cs in CmdSequenceList)
            {
                
                XmlNode csnode = xd.CreateElement("sequence");
                parent.AppendChild(csnode);
                AddParameter(xd, csnode, "name", cs.m_name);
                AddParameter(xd, csnode, "seqdata", cs.m_seq);
                AddParameter(xd, csnode, "seqtype", cs.m_seqtype);
            }
        }

        void SaveLayouts(XmlDocument xd, XmlNode parent)
        {
            XmlNode lNode = xd.CreateElement("layouts");
            parent.AppendChild(lNode);
            foreach (GuiLayout gl in GuiLayouts)
            {
                SaveLayoutRecurse(xd, lNode, gl);
            }
        }

        void SaveLayoutRecurse(XmlDocument xd, XmlNode parent, GuiLayout gl)
        {
            XmlNode glNode = xd.CreateElement(gl.type.ToString().ToLower());
            parent.AppendChild(glNode);
            AddParameter(xd, glNode, "name", gl.name);
            gl.text.Save(xd, glNode, "text");
            gl.action.Save(xd, glNode, "action");
            gl.dock.Save(xd, glNode, "dock");
            gl.control.Save(xd, glNode, "control");
            gl.image.Save(xd, glNode, "image");
            gl.orientation.Save(xd, glNode, "orientation");
            gl.direction.Save(xd, glNode, "direction");
            gl.px.Save(xd, glNode, "px");
            gl.py.Save(xd, glNode, "py");
            gl.w.Save(xd, glNode, "w");
            gl.h.Save(xd, glNode, "h");
            gl.splitPos.Save(xd, glNode, "splitpos");
            gl.isCollapsed.Save(xd, glNode, "iscollapsed");
            gl.collapsible.Save(xd, glNode, "collapsible");
            gl.isSelected.Save(xd, glNode, "isselected");
            gl.style.Save(xd, glNode, "style");
            foreach (GuiLayout subgl in gl.subLayouts)
            {
                SaveLayoutRecurse(xd, glNode, subgl);
            }
        }

        void SaveUserParams(XmlDocument xd, XmlNode parent)
        {
            XmlNode upNode = xd.CreateElement("userparams");
            parent.AppendChild(upNode);
            foreach (KeyValuePair<string, CWParameter> pair in m_userparms.paramDict)
            {
                XmlNode parnode = xd.CreateElement(pair.Value.paramName);
                upNode.AppendChild(parnode);
                pair.Value.SaveUser(xd, parnode);
            }
        }

        public static void AddParameter(XmlDocument xd, XmlNode parent, string name, object data)
        {
            if (data == null)
                return;
            XmlAttribute xa = xd.CreateAttribute(name);
            xa.Value = data.ToString();
            if (data is Color)
                xa.Value = ColorToString((Color)data);
            parent.Attributes.Append(xa);
        }

        public static string ColorToString(Color col)
        {
            string colstr = col.ToString();
            string colval = colstr.Substring(7, colstr.IndexOf(']') - 7);
            if (colval[1] == '=')
                colval = "#" + col.ToArgb().ToString("x");
            return colval;
        }
        #endregion
    }
}
