using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer
{
    /*
     * This class holds some information about the 
     * slicing and building parameters
     */
    [Serializable()]
    public class SliceBuildConfig
    {
        public static int FILE_VERSION = 2;
        public string m_filename; // for housekeeping
        public enum eBuildDirection 
        {
            Top_Down,
            Bottom_Up
        }
        public double dpmmX; // dots per mm x
        public double dpmmY; // dots per mm y
        public int xres, yres; // the resolution of the output image in pixels - set by the machine configuration
        public double ZThick; // thickness of the z layer - slicing height
        public int layertime_ms; // time to project image per layer in milliseconds
        public int firstlayertime_ms; // first layer exposure time 
        public int numfirstlayers;
        public int blanktime_ms; // blanking time between layers
        public int plat_temp; // desired platform temperature in celsius 
        public int exportsvg; // export the svg slices when building 0-none, 1-compound path, 2-filled polygons
        public bool export; // export image slices when building into cws file
        public bool exportpng;// export png slices to disk
        public eBuildDirection direction;
        public double liftdistance; // distance to lift and retract
        public double slidetiltval; // a value used for slide / tilt 
        public bool antialiasing; // should we use anti-aliasing
        public bool usemainliftgcode; // should we use mainliftgcode-tab instead of generating the gcode
        public double aaval; // anti-aliasing scaler value - How much to upsample the image values between 1.0 - 3.0 should be fine
        public double liftfeedrate; // initial lift may cause a lot of suction. To maximize lift power, we slow the steppers down to maximize stepper motor torque.
        public double bottomliftfeedrate; // the bottom layers require a lower lift rate because of the additional resin exposure
        public double liftretractrate; // the feedrate that this lowers(for bottom-up) or raises(top-down) the build platform, this is the retraction rate of the lift.
        private String m_headercode; // inserted at beginning of file
        private String m_footercode; // inserted at end of file
        private String m_preslicecode; // inserted before each slice
        private String m_liftcode; // inserted before each slice
        private String m_layercode; // inserted before each slice
        public int XOffset, YOffset; // the X/Y pixel offset used 
        public String m_exportopt; // export sliced images in ZIP or SUBDIR
        public bool m_flipX; // mirror the x axis
        public bool m_flipY; // mirror the y axis
        public string m_notes;
        public double m_resinprice; // per liter
        public Dictionary<string, InkConfig> inks;
        public String selectedInk;
        public int minExposure; // for resin test model
        public int exposureStep; // for resin test model
        public bool m_createoutlines; // render the slices additionallyt as outlines in separate bitmaps
        public double m_outlinewidth_inset;
        public double m_outlinewidth_outset;
        public PreviewGenerator.ePreview exportpreview; // generate a preview file and image
        //need some parms here for auto support

        public UserParameterList userParams;

        private String[] m_deflayer = 
        {
            ";********** Layer Start ********\r\n", //
            ";Here you can set any G or M-Code which should be executed per-layer during the build process\r\n",
            "<slice> $CURSLICE\r\n", 
            "G91 ;Relative Positioning\r\n",
            "M17 ;Enable motors\r\n",
            ";********** Layer End **********\r\n", // 
        };

        private String[] m_defheader = 
        {
            ";********** Header Start ********\r\n", //
            ";Here you can set any G or M-Code which should be executed BEFORE the build process\r\n",
            "G21 ;Set units to be mm\r\n", 
            "G91 ;Relative Positioning\r\n",
            "M17 ;Enable motors\r\n",
            ";********** Header End **********\r\n", // 
            //";()\r\n"
        };
        private String[] m_deffooter = 
        {
            ";********** Footer Start ********\r\n", //
            ";Here you can set any G or M-Code which should be executed after the last Layer is Printed\r\n",
            "M18 ;Disable Motors\r\n",
            ";<Completed>\r\n", // a marker for completed            
            ";********** Footer End ********\r\n", // 
        };

        
        private String[] m_defpreslice = 
        {
            ";********** Pre-Slice Start ********\r\n", //
            ";Set up any GCode here to be executed before a lift\r\n",
            ";********** Pre-Slice End **********\r\n",
        };

        private String[] m_deflift = 
        {
            ";********** Lift Sequence ********\r\n",// 
            "G1{$SlideTiltVal != 0? X$SlideTiltVal:} Z($ZLiftDist * $ZDir) F{$CURSLICE < $NumFirstLayers?$ZBottomLiftRate:$ZLiftRate}\r\n", 
            "G1{$SlideTiltVal != 0? X($SlideTiltVal * -1):} Z(($LayerThickness-$ZLiftDist) * $ZDir) F$ZRetractRate\r\n",
            ";<Delay> %d$BlankTime\r\n",
            ";********** Lift Sequence **********\r\n", // 
        };

        private String[] m_defshutteropen = 
        {
            ";********** Shutter open Sequence ********\r\n",// 
        };

        private String[] m_defshutterclose = 
        {
            ";********** Shutter close Sequence ********\r\n",// 
        };

        private string DefGCodeHeader() 
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in m_defheader)
                sb.Append(s);
            return sb.ToString();        
        }

        private string DefGCodeFooter()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in m_deffooter)
                sb.Append(s);
            return sb.ToString();
        }
        private string DefGCodePreslice()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in m_defpreslice)
                sb.Append(s);
            return sb.ToString();
        }
        private string DefGCodeLift()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in m_deflift)
                sb.Append(s);
            return sb.ToString();
        }
        private string DefGCodeLayer()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in m_deflayer)
                sb.Append(s);
            return sb.ToString();
        }

        
        public String HeaderCode
        {
            get { return m_headercode; }
            set { m_headercode = value; }
        }
        public String FooterCode
        {
            get { return m_footercode; }
            set { m_footercode = value; }
        }
        public string LayerCode 
        {
            get { return m_layercode; }
            set { m_layercode = value; }
        
        }
        public String LiftCode
        {
            get { return m_liftcode; }
            set { m_liftcode = value; }
        }

        public String PreSliceCode
        {
            get { return m_preslicecode; }
            set { m_preslicecode = value; }
        }


        /*
         Copy constructor
         */
        public SliceBuildConfig(SliceBuildConfig source) 
        {
            CopyFrom(source);
        }
        public void CopyFrom(SliceBuildConfig source) 
        {
            dpmmX = source.dpmmX; // dots per mm x
            dpmmY = source.dpmmY; // dots per mm y
            xres = source.xres;
            yres = source.yres; // the resolution of the output image
            ZThick = source.ZThick; // thickness of the z layer - slicing height
            layertime_ms = source.layertime_ms; // time to project image per layer in milliseconds
            firstlayertime_ms = source.firstlayertime_ms;
            blanktime_ms = source.blanktime_ms;
            plat_temp = source.plat_temp; // desired platform temperature in celsius 
            // exportgcode = source.exportgcode; // export the gcode file when slicing
            exportsvg = source.exportsvg; // export the svg slices when building
            export = source.export; // export image slices when building
            exportpng = source.exportpng;
            m_headercode = source.m_headercode; // inserted at beginning of file
            m_footercode = source.m_footercode; // inserted at end of file
            m_preslicecode = source.m_preslicecode; // inserted before each slice
            m_liftcode = source.m_liftcode; // its the main lift code
            m_layercode = source.m_layercode; // its the main lift code
            
            liftdistance = source.liftdistance;
            direction = source.direction;
            numfirstlayers = source.numfirstlayers;
            XOffset = source.XOffset;
            YOffset = source.YOffset;
            slidetiltval = source.slidetiltval;
            antialiasing = source.antialiasing;
            usemainliftgcode = source.usemainliftgcode;
            liftfeedrate = source.liftfeedrate;
            bottomliftfeedrate = source.bottomliftfeedrate;
            liftretractrate = source.liftretractrate;
            aaval = source.aaval;//
            //m_generateautosupports = source.m_generateautosupports;
            m_exportopt = source.m_exportopt;
            m_flipX = source.m_flipX;
            m_flipY = source.m_flipY;
            m_notes = source.m_notes;
            m_resinprice = source.m_resinprice;
            selectedInk = source.selectedInk;
            m_createoutlines = source.m_createoutlines;
            m_outlinewidth_inset = source.m_outlinewidth_inset;
            m_outlinewidth_outset = source.m_outlinewidth_outset;
            if (source.inks != null)
            {
                inks = new Dictionary<string, InkConfig>();
                foreach (KeyValuePair<string, InkConfig> entry in source.inks)
                {
                    inks[entry.Key] = entry.Value;
                }
            }
            minExposure = source.minExposure;
            exposureStep = source.exposureStep;
            exportpreview = source.exportpreview;

            userParams = source.userParams;
        }
        public SliceBuildConfig() 
        {           
            // every new config will be set to default settings,
            // if the load fails for some reason (such as a previous version)
            // then whatever information that has already been loaded will stay
            // the rest will have been set up to the default values
            CreateDefault(); 
        }

        public void UpdateFrom(MachineConfig mf)
        {
            try
            {
                //update the slice / build profile here with the 
                // x/y resolution for the current display(s)
                xres = mf.XRenderSize;
                yres = mf.YRenderSize;
                //get the first monitor configuration
                MonitorConfig mc = mf.m_lstMonitorconfigs[0];

                dpmmX = (xres) / mf.m_PlatXSize;
                dpmmY = (yres) / mf.m_PlatYSize;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        public void CreateDefault() 
        {
            numfirstlayers = 3;
            layertime_ms = 1000;// 1 second default
            firstlayertime_ms = 5000;
            blanktime_ms = 2000; // 2 seconds blank
            xres = 1024;
            yres = 768;
            ZThick = .05;
            plat_temp = 75;
            dpmmX = 102.4;
            dpmmY = 76.8;
            XOffset = 0;
            YOffset = 0;
            numfirstlayers = 3;
            //exportgcode = true;
            exportsvg = 0;
            export = false;
            exportpng = false;
            direction = eBuildDirection.Bottom_Up;
            liftdistance = 5.0;
            //raise_time_ms = 750;
            slidetiltval = 0.0;
            antialiasing = false;
            usemainliftgcode = false;
            aaval = 1.5;
            liftfeedrate = 50.0;// 50mm/s
            bottomliftfeedrate = 25.0;
            liftretractrate = 100.0;// 100mm/s
            m_exportopt = "SUBDIR"; // default to saving in subdirectory
            m_flipX = false;
            m_flipY = false;
            m_notes = "";
            m_resinprice = 0.0;//
            //set the default gcode segments
            m_headercode = DefGCodeHeader();
            m_footercode = DefGCodeFooter();
            m_liftcode = DefGCodeLift();
            m_layercode = DefGCodeLayer();
            m_preslicecode = DefGCodePreslice();
            inks = new Dictionary<string, InkConfig>();
            selectedInk = "Default";
            inks[selectedInk] = new InkConfig(selectedInk);
            minExposure = 500;
            m_createoutlines = false;
            m_outlinewidth_inset = 2.0;
            m_outlinewidth_outset = 0.0;
            exposureStep = 200;
            exportpreview = PreviewGenerator.ePreview.None;
            userParams = new UserParameterList();
        }

        public bool SetCurrentInk(string inkname)
        {
            if (inks.ContainsKey(inkname))
            {
                selectedInk = inkname;
                InkConfig ic = inks[inkname];
                ZThick = ic.ZThick;
                layertime_ms = ic.layertime_ms;
                firstlayertime_ms = ic.firstlayertime_ms;
                numfirstlayers = ic.numfirstlayers;
                m_resinprice = ic.resinprice;
                return true;
            }
            return false;
        }

        public bool UpdateCurrentInk()
        {
            if (inks.ContainsKey(selectedInk))
            {
                InkConfig ic = inks[selectedInk];
                ic.ZThick = ZThick;
                ic.layertime_ms = layertime_ms;
                ic.firstlayertime_ms = firstlayertime_ms;
                ic.numfirstlayers = numfirstlayers;
                ic.resinprice = m_resinprice;
                return true;
            }
            return false;
        }

        private void LoadInternal(ref XmlHelper xh) 
        {
            XmlNode sbc = xh.m_toplevel;
            dpmmX = xh.GetDouble(sbc, "DotsPermmX", 102.4);
            dpmmY = xh.GetDouble(sbc, "DotsPermmY", 76.8);
            xres = xh.GetInt(sbc, "XResolution", 1024);
            yres = xh.GetInt(sbc, "YResolution", 768);
            //ZThick = xh.GetDouble(sbc, "SliceHeight", 0.05);
            //layertime_ms = xh.GetInt(sbc, "LayerTime", 1000); // 1 second default
            //firstlayertime_ms = xh.GetInt(sbc, "FirstLayerTime", 5000);
            blanktime_ms = xh.GetInt(sbc, "BlankTime", 2000); // 2 seconds blank
            plat_temp = xh.GetInt(sbc, "PlatformTemp", 75);
            //exportgcode = xh.GetBool(sbc, "ExportGCode"));

            exportsvg = xh.GetInt(sbc, "ExportSVG", 0); // the problem is this was previously a boolean variable

            if ((exportsvg < 0) || (exportsvg > 4))
                exportsvg = 0;
            export = xh.GetBool(sbc, "Export", false); ;
            exportpng = xh.GetBool(sbc, "ExportPNG", false); ;

            XOffset = xh.GetInt(sbc, "XOffset", 0);
            YOffset = xh.GetInt(sbc, "YOffset", 0);
            //numfirstlayers = xh.GetInt(sbc, "NumberofBottomLayers", 3);
            direction = (eBuildDirection)xh.GetEnum(sbc, "Direction", typeof(eBuildDirection), eBuildDirection.Bottom_Up);
            liftdistance = xh.GetDouble(sbc, "LiftDistance", 5.0);
            slidetiltval = xh.GetDouble(sbc, "SlideTiltValue", 0.0);
            antialiasing = xh.GetBool(sbc, "AntiAliasing", false);
            usemainliftgcode = xh.GetBool(sbc, "UseMainLiftGCode", false);
            aaval = xh.GetDouble(sbc, "AntiAliasingValue", 1.5);
            liftfeedrate = xh.GetDouble(sbc, "LiftFeedRate", 50.0); // 50mm/s
            bottomliftfeedrate = xh.GetDouble(sbc, "BottomLiftFeedRate", 25.0); // 50mm/s
            liftretractrate = xh.GetDouble(sbc, "LiftRetractRate", 100.0); // 100mm/s
            m_exportopt = xh.GetString(sbc, "ExportOption", "SUBDIR"); // default to saving in subdirectory
            m_flipX = xh.GetBool(sbc, "FlipX", false);
            m_flipY = xh.GetBool(sbc, "FlipY", false);
            m_notes = xh.GetString(sbc, "Notes", "");
            m_createoutlines = xh.GetBool(sbc, "RenderOutlines", false);
            m_outlinewidth_inset = xh.GetDouble(sbc, "OutlineWidth_Inset", 2.0);
            m_outlinewidth_outset = xh.GetDouble(sbc, "OutlineWidth_Outset", 0.0);
            
            //m_resinprice = xh.GetDouble(sbc, "ResinPriceL", 0.0);

            m_headercode = xh.GetString(sbc, "GCodeHeader", DefGCodeHeader());
            m_footercode = xh.GetString(sbc, "GCodeFooter", DefGCodeFooter());
            m_preslicecode = xh.GetString(sbc, "GCodePreslice", DefGCodePreslice());
            m_liftcode = xh.GetString(sbc, "GCodeLift", DefGCodeLift());
            m_layercode = xh.GetString(sbc, "GCodeLayer", DefGCodeLayer());
            selectedInk = xh.GetString(sbc, "SelectedInk", "Default");
            inks = new Dictionary<string, InkConfig>();
            List<XmlNode> inkNodes = xh.FindAllChildElement(sbc, "InkConfig");
            foreach (XmlNode xnode in inkNodes)
            {
                string name = xh.GetString(xnode, "Name", "Default");
                InkConfig ic = new InkConfig(name);
                ic.Load(xh, xnode);
                inks[name] = ic;
            }
            if (!inks.ContainsKey(selectedInk))
            {
                InkConfig ic = new InkConfig(selectedInk);
                ic.Load(xh, sbc); // try loading legacy settings from parent
                inks[selectedInk] = ic;
            }
            SetCurrentInk(selectedInk);
            minExposure = xh.GetInt(sbc, "MinTestExposure", 500);
            exposureStep = xh.GetInt(sbc, "TestExposureStep", 200);
            exportpreview = (PreviewGenerator.ePreview)xh.GetEnum(sbc, "ExportPreview", typeof(PreviewGenerator.ePreview), PreviewGenerator.ePreview.None);
            xh.LoadUserParamList(userParams);
        }
        /// <summary>
        /// This allows for retrieve arbitrary variables from the slice XML configuration
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public string GetStringVar(string varname)
        {
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(m_filename, "SliceBuildConfig");
            XmlNode mc = xh.m_toplevel;
            string retstr = xh.GetString(mc, varname, "");
            return retstr;
        }
        /// <summary>
        /// This function assumes that the file has already been saved
        /// after setting the variable, it will re-save the file
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="value"></param>
        public void SetStringVar(string varname, string value) 
        {
            //m_filename = filename;
            XmlHelper xh = new XmlHelper();
            xh.StartNew(m_filename, "SliceBuildConfig");
            SaveInternal(ref xh);
            //save the var
            XmlNode sbc = xh.m_toplevel;
            xh.SetParameter(sbc, varname, value);
            try
            {
                xh.Save(FILE_VERSION);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }

        /// <summary>
        /// Load the slice and build profile from a Stream
        /// This is used when we're serializing / deserializing from the 
        /// Memory stream pulled from a zip file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool Load(Stream stream, String filename) 
        {
            m_filename = filename;
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.LoadFromStream(stream, "SliceBuildConfig");
            LoadInternal(ref xh);
            return true;            
        }

        public bool Load(String filename) 
        {
            m_filename = filename;
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(filename, "SliceBuildConfig");

            LoadInternal(ref xh);
            if (!fileExist)
            {
                return xh.Save(FILE_VERSION);
            }

            return true;
         
        }

        private void SaveInternal(ref XmlHelper xh) 
        {
            XmlNode sbc = xh.m_toplevel;
            xh.SetParameter(sbc, "DotsPermmX", dpmmX);
            xh.SetParameter(sbc, "DotsPermmY", dpmmY);
            xh.SetParameter(sbc, "XResolution", xres);
            xh.SetParameter(sbc, "YResolution", yres);
            //xh.SetParameter(sbc, "SliceHeight", ZThick);
            //xh.SetParameter(sbc, "LayerTime", layertime_ms);
            //xh.SetParameter(sbc, "FirstLayerTime", firstlayertime_ms);
            xh.SetParameter(sbc, "BlankTime", blanktime_ms);
            xh.SetParameter(sbc, "PlatformTemp", plat_temp);
            // xh.SetParameter(sbc, "ExportGCode", exportgcode);
            xh.SetParameter(sbc, "ExportSVG", exportsvg);
            xh.SetParameter(sbc, "Export", export);
            xh.SetParameter(sbc, "ExportPNG", exportpng);
            
            xh.SetParameter(sbc, "XOffset", XOffset);
            xh.SetParameter(sbc, "YOffset", YOffset);
            //xh.SetParameter(sbc, "NumberofBottomLayers", numfirstlayers);
            xh.SetParameter(sbc, "Direction", direction);
            xh.SetParameter(sbc, "LiftDistance", liftdistance);
            xh.SetParameter(sbc, "SlideTiltValue", slidetiltval);
            xh.SetParameter(sbc, "AntiAliasing", antialiasing);
            xh.SetParameter(sbc, "UseMainLiftGCode", usemainliftgcode);
            xh.SetParameter(sbc, "AntiAliasingValue", aaval);
            xh.SetParameter(sbc, "LiftFeedRate", liftfeedrate);
            xh.SetParameter(sbc, "BottomLiftFeedRate", bottomliftfeedrate);            
            xh.SetParameter(sbc, "LiftRetractRate", liftretractrate);
            xh.SetParameter(sbc, "ExportOption", m_exportopt);
            xh.SetParameter(sbc, "RenderOutlines", m_createoutlines);
            xh.SetParameter(sbc, "OutlineWidth_Inset", m_outlinewidth_inset);
            xh.SetParameter(sbc, "OutlineWidth_Outset", m_outlinewidth_outset);
            
            xh.SetParameter(sbc, "FlipX", m_flipX);
            xh.SetParameter(sbc, "FlipY", m_flipY);
            xh.SetParameter(sbc, "Notes", m_notes);
            //xh.SetParameter(sbc, "ResinPriceL", m_resinprice);
            xh.SetParameter(sbc, "GCodeHeader", m_headercode);
            xh.SetParameter(sbc, "GCodeFooter", m_footercode);
            xh.SetParameter(sbc, "GCodePreslice", m_preslicecode);
            xh.SetParameter(sbc, "GCodeLift", m_liftcode);
            xh.SetParameter(sbc, "GCodeLayer", m_layercode);

            xh.SetParameter(sbc, "SelectedInk", selectedInk);
            foreach (KeyValuePair<string, InkConfig> entry in inks)
            {
                inks[entry.Key].Save(xh, sbc);
            }
            xh.SetParameter(sbc, "MinTestExposure", minExposure);
            xh.SetParameter(sbc, "TestExposureStep", exposureStep);
            xh.SetParameter(sbc, "ExportPreview", exportpreview);

            xh.SaveUserParamList(userParams);
        }

        public bool Save(String filename) 
        {
            m_filename = filename;
            XmlHelper xh = new XmlHelper();
            xh.StartNew(filename, "SliceBuildConfig");
            SaveInternal(ref xh);
            try
            {
                xh.Save(FILE_VERSION);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
            return true;
        }

        public bool Save(MemoryStream stream, string filename) 
        {
            m_filename = filename;
            XmlHelper xh = new XmlHelper();
            bool fileExist = xh.Start(filename, "SliceBuildConfig");
            SaveInternal(ref xh);
            xh.Save(FILE_VERSION, ref stream);
            return true;            
        }
        // these get stored to the gcode file as a reference
        public override String ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(";(****Build and Slicing Parameters****)\r\n");
            sb.Append(";(Pix per mm X            = " + String.Format("{0:0.00000}", dpmmX) + " px/mm )\r\n");
            sb.Append(";(Pix per mm Y            = " + String.Format("{0:0.00000}", dpmmY) + " px/mm )\r\n");
            sb.Append(";(X Resolution            = " + xres + " )\r\n");
            sb.Append(";(Y Resolution            = " + yres + " )\r\n");
           // sb.Append(";(X Pixel Offset          = " + XOffset + " px )\r\n");
           // sb.Append(";(Y Pixel Offset          = " + YOffset + " px )\r\n");
            sb.Append(";(Layer Thickness         = " + String.Format("{0:0.00000}", ZThick) + " mm )\r\n");
            sb.Append(";(Layer Time              = " + layertime_ms + " ms )\r\n");
            sb.Append(";(Render Outlines         = " + m_createoutlines.ToString() + "\r\n");
            sb.Append(";(Outline Width Inset     = " + m_outlinewidth_inset.ToString() + "\r\n");
            sb.Append(";(Outline Width Outset    = " + m_outlinewidth_outset.ToString() + "\r\n");
            sb.Append(";(Bottom Layers Time      = " + firstlayertime_ms + " ms )\r\n");
            sb.Append(";(Number of Bottom Layers = " + numfirstlayers + " )\r\n");
            sb.Append(";(Blanking Layer Time     = " + blanktime_ms + " ms )\r\n");
            sb.Append(";(Build Direction         = " + direction.ToString() + ")\r\n");
            sb.Append(";(Lift Distance           = " + liftdistance.ToString() + " mm )\r\n");
            sb.Append(";(Slide/Tilt Value        = " + slidetiltval.ToString() + ")\r\n");
            sb.Append(";(Anti Aliasing           = " + antialiasing.ToString() + ")\r\n");
            sb.Append(";(Use Mainlift GCode Tab  = " + usemainliftgcode.ToString() + ")\r\n");
            sb.Append(";(Anti Aliasing Value     = " + aaval.ToString() + " )\r\n");
            sb.Append(";(Z Lift Feed Rate        = " + String.Format("{0:0.00000}", liftfeedrate) + " mm/s )\r\n");
            sb.Append(";(Z Bottom Lift Feed Rate = " + String.Format("{0:0.00000}", bottomliftfeedrate) + " mm/s )\r\n");
            sb.Append(";(Z Lift Retract Rate     = " + String.Format("{0:0.00000}", liftretractrate) + " mm/s )\r\n");
            sb.Append(";(Flip X                  = " + m_flipX.ToString() + ")\r\n");
            sb.Append(";(Flip Y                  = " + m_flipY.ToString() + ")\r\n");
            return sb.ToString();
        }

        
        public bool SaveFile(String filename, String contents) 
        {
            try
            {
                File.WriteAllText(filename, contents);
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
        }

        public string AddNewResin(string resinName)
        {
            if (inks.ContainsKey(resinName))
                return "Resin profile name already exists";
            InkConfig ic = new InkConfig(resinName);
            if ((selectedInk != null) && inks.ContainsKey(selectedInk))
                ic.CopyFrom(inks[selectedInk]);
            inks[resinName] = ic;
            SetCurrentInk(resinName);
            return "OK";
        }

        public string RemoveSelectedInk()
        {
            if (inks.ContainsKey(selectedInk))
            {
                inks.Remove(selectedInk);
                if (inks.Count == 0)
                {
                    selectedInk = null;
                    AddNewResin("Default");
                    return "Resin profile reset to default|Attention";
                }
                else
                {
                    SetCurrentInk(inks.First().Key);
                }
            }
            return "OK";
        }
    }
}
