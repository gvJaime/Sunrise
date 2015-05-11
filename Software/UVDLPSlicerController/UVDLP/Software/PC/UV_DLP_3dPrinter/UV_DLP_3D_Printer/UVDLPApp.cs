using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Engine3D;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using UV_DLP_3D_Printer.Drivers;
using UV_DLP_3D_Printer.Slicing;
using UV_DLP_3D_Printer;
using System.Drawing;
using UV_DLP_3D_Printer.Configs;
using System.Collections;
using Ionic.Zip;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using UV_DLP_3D_Printer.Plugin;
using System.Windows;
using System.Reflection;
using UV_DLP_3D_Printer._3DEngine;
using UV_DLP_3D_Printer._3DEngine.CSG;
using UV_DLP_3D_Printer.Licensing;
using CreationWorkshop.Licensing;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using UV_DLP_3D_Printer.GUI;
using UV_DLP_3D_Printer.Building;
using UV_DLP_3D_Printer.Util.Sequence;

namespace UV_DLP_3D_Printer
{
    public enum eAppEvent
    {
        eModelAdded, // we just loaded / created a model and added it to the scene
        eModelNotLoaded, // we tried to load a model and it failed to load
        eModelRemoved, // we removed a model from the scene
        eGCodeLoaded, 
        eGCodeSaved,
        eSupportGenerated,
        eSlicedLoaded,
        eMachineTypeChanged,
        eMachineConfigChanged,
        eSliceProfileChanged,
        eShowDLP,  // this event is raised when someone wants to show the DLP screen
        eShowCalib, // request to show calibration screen
        eShowBlank, // request to show blank screen 
        eHideDLP,
        eMachineConnected, // the main serial port for the machine was opened - aka - we connected
        eMachineDisconnected, // the machine disconnected
        eDisplayConnected,
        eDisplayDisconnected,
        eObjectSelected, // object selection has changed
        eReDraw, // this is used when an application action needs to re-draw the 3d display
        eReDraw2D, // update only the top 2D layer of the 3D display
        eUpdateSelectedObject, // this will update object information and will perform a redraw.
        eShowLogWindow,  
        eSceneFileNameChanged,
        eNewVersionAvailable, // the app contacted the server and saw there was a new version available
        eSceneSaved, // scene was saved to disk. message contains the file type (cws or stl)
        eMachineConfigLoaded,  // Machine config was just loaded
        eLicenseUpdate, // server sent a response after we contacted them with license info
        eCustom, // plugins can use this for very general purpose low-bandwidth signaling 
    }
    public delegate void AppEventDelegate(eAppEvent ev, String Message);
    /*
     This represents the main application object
     */
    public class UVDLPApp : IPluginHost
    {
        public event AppEventDelegate AppEvent;
        private static UVDLPApp m_instance = null;
        public String m_PathMachines;
        public String m_PathProfiles;
        public String m_apppath;
        private String m_scenefilename;
        public string appconfigname; // the full filename
        // the current application configuration object
        public AppConfig m_appconfig;
        // the simple 3d graphic engine we're using along with OpenGL
        public Engine3d m_engine3d = new Engine3d();
        // the current model we're working with
        //private Object3d m_selectedobject = null;
        private List<Object3d> m_selectedobjects = null;
        // the scene object used for slicing
        private Object3d m_sceneobject = null; // need to get rid of this...
        // the current machine configuration
        public MachineConfig m_printerinfo = new MachineConfig();
        // the current building / slicing profile
        public SliceBuildConfig m_buildparms;
        public DisplayManager m_dispmgr;// = DisplayManager.Instance(); // initialize the singleton for early event app binding
        // handle program wide callbacks by name
        public CallbackHandler m_callbackhandler;

        public SupportConfig m_supportconfig;
        public SupportGenerator m_supportgenerator;
        public SequenceManager m_sequencemanager;
        // the interface to the printer
        public DeviceInterface m_deviceinterface;// = new PrinterInterface();
        // the generated or loaded GCode File;
        public GCodeFile m_gcode;
        // the slicer we're using 
        public Slicer m_slicer;//
        //current slice file
        public SliceFile m_slicefile;
        public BuildManager m_buildmgr;
        public prjcmdlst m_proj_cmd_lst; // projector command list
        public GuiConfigManager m_gui_config;
        public C2DGraphics m_2d_graphics;

        public static String m_appconfigname = "CreationConfig.xml";
        public static String m_pathsep = "\\";

        public List<PluginEntry> m_plugins; // list of plug-ins
        public PluginStates m_pluginstates;

        public Undoer m_undoer;
        public frmMain2 m_mainform; // reference to the main application form       

        public ServerContact m_sc;
        public AuxBuildCmds m_AuxBuildCmds;

        public frmExport m_exporter;

        public bool m_splashStop = false;

        #region SupportModes
        public enum eSupportEditMode 
        {
            eNone,
            eAddSupport
            //eModifySupport
        }
        public eSupportEditMode SupportEditMode 
        {
            get { return m_supportmode;}
            set { m_supportmode = value; }
        }

        private eSupportEditMode m_supportmode;
        #endregion

        public static UVDLPApp Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new UVDLPApp();
            }
            return m_instance;
        }

        private UVDLPApp() 
        {
            m_supportmode = eSupportEditMode.eNone;
            SceneFileName = "";
            m_callbackhandler = new CallbackHandler();
            m_appconfig = new AppConfig();
            m_printerinfo = new MachineConfig();
            m_buildparms = new SliceBuildConfig();
            m_deviceinterface = new DeviceInterface();
            m_buildmgr = new BuildManager();
            m_slicer = new Slicer();
            m_slicer.m_slicemethod = Slicer.eSliceMethod.eNormalCount;// configure the slicer to user the new normal count - Thanks Shai!!!
            m_slicer.Slice_Event += new Slicer.SliceEvent(SliceEv);
            //m_dispmgr = DisplayManager.Instance(); // initialize the singleton for early event app binding
            //m_flexslice = new FlexSlice();
            m_gcode = new GCodeFile(""); // create a blank gcode to start with
            m_supportconfig = new SupportConfig();
            m_supportgenerator = new SupportGenerator();
            m_supportgenerator.SupportEvent+= new SupportGeneratorEvent(SupEvent);
            CSG.Instance().CSGEvent += new CSG.CSGEventDel(CSGEvent);
            m_proj_cmd_lst = new prjcmdlst();
            m_plugins = new List<PluginEntry>(); // list of user plug-ins
            m_pluginstates =  PluginStates.Instance(); // initialize the plugin state singleton           
            m_undoer = new Undoer();
            m_2d_graphics = new C2DGraphics();
            m_gui_config = new GuiConfigManager();
            m_AuxBuildCmds = AuxBuildCmds.Instance(); // make sure the singleton doesn't go away...
            m_sequencemanager = SequenceManager.Instance();
            m_exporter = new frmExport();
            m_exporter.RegisterExporter(new B9JExporter());
        }

        public enum Platform
        {
            Windows,
            Linux,
            Mac
        }
        public void CSGEvent(CSG.eCSGEvent ev, string msg, Object3d dat) 
        {
            try
            {
                switch (ev)
                {
                    case CSG.eCSGEvent.eCompleted:
                        m_engine3d.AddObject(dat);
                        RaiseAppEvent(eAppEvent.eReDraw, "");
                        break;
                    case CSG.eCSGEvent.eError:
                        break;
                    case CSG.eCSGEvent.eProgress:
                        break;
                    case CSG.eCSGEvent.eStarted:
                        break;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        public void SupEvent(SupportEvent ev, string message, Object obj) 
        {
            //if()
            
            try
            {
                switch (ev)
                {
                    case SupportEvent.eCompleted:
                        List<Object3d> lstobjs = (List<Object3d>)obj;
                        bool firstAdded = true;
                        if (lstobjs != null) 
                        {
                            foreach (Object3d o in lstobjs) 
                            {
                                m_engine3d.AddObject((Object3d)o);
                                UVDLPApp.Instance().m_undoer.SaveAddition((Object3d)o);
                                if (!firstAdded)
                                    UVDLPApp.Instance().m_undoer.LinkToPrev();
                                firstAdded = false;
                            }
                            RaiseAppEvent(eAppEvent.eModelAdded, message);
                        }
                        break;
                    case SupportEvent.eCancel:
                        break;
                    case SupportEvent.eProgress:
                        break;
                    case SupportEvent.eStarted:
                        
                        break;
                    case SupportEvent.eSupportGenerated:
                        //m_engine3d.AddObject((Object3d)obj);
                      //  RaiseAppEvent(eAppEvent.eModelAdded, message);
                            //add the model to the scene
                        break;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);       // look more into the error being raised here on slicing completed
            }
        }
        public void RaiseAppEvent(eAppEvent ev, String message) 
        {
            if (AppEvent != null) 
            {
                AppEvent(ev, message);
            }
        }
        public bool SaveProjectorCommands(string filename) 
        {
            return m_proj_cmd_lst.Save(m_apppath + m_pathsep + filename);
        }
        public bool LoadProjectorCommands(string filename) 
        {
            return m_proj_cmd_lst.Load(m_apppath + m_pathsep + filename);
        }

        public bool LoadSupportConfig(string filename)
        {
            m_supportconfig.Load(m_apppath + m_pathsep + filename);
            return true;
        }
        public bool SaveSupportConfig(string filename)
        {
            m_supportconfig.Save(m_apppath + m_pathsep + filename);
            return true;
        }

        public String SceneFileName
        {
            get 
            { 
                return m_scenefilename; 
            }
            set 
            {
                m_scenefilename = value;
                RaiseAppEvent(eAppEvent.eSceneFileNameChanged, m_scenefilename);
            }
          
        }

        /// <summary>
        /// I want to get rid of this function, need to replace the 
        /// Slic3r model and the supportgenerator model
        /// </summary>
        public void CalcScene() 
        {
            m_sceneobject = new Object3d();
            int idx = 0;
            foreach(Object3d obj in m_engine3d.m_objects)
            {
                m_sceneobject.Add(obj);
                if (idx == 0) // if this is the first object
                {
                    if (m_engine3d.m_objects.Count > 1)// if there is more than one object in the scene, generate a unique name                      
                    {
                        string scenename = obj.m_fullname;
                        scenename = Path.GetDirectoryName(obj.m_fullname) + m_pathsep + Path.GetFileNameWithoutExtension(obj.m_fullname) + "_scene.stl";
                        m_sceneobject.m_fullname = scenename;
                    }
                    else 
                    {
                        m_sceneobject.m_fullname = obj.m_fullname;
                    }
                }
                idx++;
            }
        }
        public Object3d Scene 
        {
            get 
            {
                return m_sceneobject;
            }
        }

        //private Object3d m_selectedobject = null;

        
        public void StartAddSupports()
        {

            CalcScene(); // do it for the scene
            m_supportgenerator.Start(m_supportconfig, m_sceneobject);

        }

        public void RemoveAllSupports() 
        {
            List<Object3d> lst = new List<Object3d>();

            foreach (Object3d obj in m_engine3d.m_objects) 
            {
                if ((obj.tag == Object3d.OBJ_SUPPORT) || (obj.tag == Object3d.OBJ_SUPPORT_BASE))
                {
                    lst.Add(obj);
                }
                if (obj.m_supports.Count > 0) 
                {
                    obj.m_supports.Clear();
                }
            }
            foreach (Object3d obj in lst) 
            {
                m_undoer.SaveDelition(obj);
                m_engine3d.RemoveObject(obj);                
            }
            RaiseAppEvent(eAppEvent.eModelRemoved, "model removed");
        }
        /*
        /// <summary>
        /// Adds a new dummy support
        /// </summary>
        public void AddSupport()
        {
            // Cylinder3d cyl = new Cylinder3d();
            // cyl.Create(2.5, 1.5, 10, 15, 2);
            Support s = new Support();
            //s.Create((float)m_supportconfig.fbrad, 1.5f, 1.5f, .75f, 2f, 5f, 2f, 20);
            s.Create(null, (float)m_supportconfig.fbrad, (float)m_supportconfig.ftrad, (float)m_supportconfig.hbrad,
                (float)m_supportconfig.htrad, 2f, 5f, 2f, 11);
            m_engine3d.AddObject(s);
            UVDLPApp.Instance().m_undoer.SaveAddition(s);
            RaiseAppEvent(eAppEvent.eModelAdded, "Model Created");
        }
        */
        /// <summary>
        /// Adds a support base plate under objects 
        /// </summary>
        public void AddSupportBase(bool selectedObjectsOnly)
        {
            // add support base - SHS
            List<Object3d> stdObs = new List<Object3d>();
            foreach (Object3d obj in UVDLPApp.Instance().m_engine3d.m_objects)
            {
                if (selectedObjectsOnly && !obj.m_inSelectedList)
                    continue;

                if ((obj != null) && (obj.tag == Object3d.OBJ_NORMAL))
                    stdObs.Add(obj);
            }

            foreach (Object3d obj in stdObs)
            {
                // remove old support base if exists
                SupportBase sb = obj.GetSupportBase();
                if (sb != null)
                {
                    obj.RemoveSupport(sb);
                    UVDLPApp.Instance().m_undoer.SaveDelition(sb);
                    UVDLPApp.Instance().m_engine3d.RemoveObject(sb);
                }
                sb = new SupportBase();
                sb.Generate(obj, 5);
                //lstsupports.Add(sb);
                obj.AddSupport(sb);
                m_engine3d.AddObject(sb);
                UVDLPApp.Instance().m_undoer.SaveAddition(sb);
            }
            RaiseAppEvent(eAppEvent.eModelAdded, "Support bases Created");
        }


        /// <summary>
        /// Removes the currently selected object
        /// </summary>
        public void RemoveCurrentModel() 
        {
            try
            {
                if (SelectedObject != null)
                {
                    UVDLPApp.Instance().m_undoer.SaveDelition(SelectedObject);
                    foreach (Object3d sup in SelectedObject.m_supports)
                    {
                        UVDLPApp.Instance().m_undoer.SaveDelition(sup);
                        UVDLPApp.Instance().m_undoer.LinkToPrev();
                        m_engine3d.RemoveObject(sup, false); // remove all the supports of this object, hold out on sending events
                    }
                    m_engine3d.RemoveObject(SelectedObject); // now remove the object
                    SelectedObject = null;
                    RaiseAppEvent(eAppEvent.eModelRemoved, "model removed");
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public Object3d SelectedObject
        {
            get 
            {
                if (m_selectedobjects == null)
                    return null;
                return m_selectedobjects[0];
            }
            set 
            {
                if (m_selectedobjects != null)
                {
                    foreach (Object3d obj in m_selectedobjects)
                    {
                        obj.m_inSelectedList = false;
                    }
                }
                if (value == null)
                    m_selectedobjects = null;
                else
                {
                    m_selectedobjects = new List<Object3d>();
                    m_selectedobjects.Add(value);
                    value.m_inSelectedList = true;
                }
                m_engine3d.UpdateLists(); // need to re-update the selected object lists
                RaiseAppEvent(eAppEvent.eObjectSelected, "Object Selection Changed");
            }
        }

        public List<Object3d> SelectedObjectList
        {
            get { return m_selectedobjects; }
        }


        public void AddToSelectionList(Object3d obj)
        {
            if (obj == null)
                return;
            if (m_selectedobjects == null)
                SelectedObject = obj;
            else
            {
                m_selectedobjects.Add(obj);
                obj.m_inSelectedList = true;
            }
        }

        /// <summary>
        /// Loads a model, adds it to the 3d engine to be shown, and raises an app event
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadModel(String filename) 
        {
            try
            {
                ModelLoader ml = new ModelLoader();
                List<Object3d> objs = ml.Load(filename);
                if (objs != null)
                {
                    foreach (Object3d obj in objs)
                    {
                        obj.CenterOnPlatform();
                        m_engine3d.AddObject(obj);
                        m_undoer.SaveAddition(obj);
                        SelectedObject = obj;
                        //test code to create a preview, this should definitely go somewhere else
                        /*PreviewGenerator pg = new PreviewGenerator();
                        Bitmap preview = pg.GeneratePreview(512, 512, obj);
                        if(preview !=null)
                            preview.Save(UVDLPApp.Instance().m_apppath + "\\testpreview.png");*/

                    }
                    UVDLPApp.Instance().m_engine3d.UpdateLists();
                    m_slicefile = null; // the slice file is not longer current
                    RaiseAppEvent(eAppEvent.eModelAdded, "Model Loaded " + filename);
                }
                else 
                {
                    RaiseAppEvent(eAppEvent.eModelNotLoaded, "Model " + filename + " Failed to load");
                }

                return (objs != null);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
                return false;
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        void SliceEv(Slicer.eSliceEvent ev, int layer, int totallayers,SliceFile sf) 
        {
          //  String path = "";
           // String fileName = "";
            switch (ev) 
            {
                case Slicer.eSliceEvent.eSliceStarted:

                    break;
                case Slicer.eSliceEvent.eLayerSliced:
                  
                    break;
                case Slicer.eSliceEvent.eSliceCompleted: // this all needs to be changed....
                    m_slicefile = sf;
                    //generate the GCode
                    m_gcode = GCodeGenerator.Generate(m_slicefile, m_printerinfo);
                    //we only need the file name of the gcode if we're saving it somewhere...
                    //see if we're exporting this to a zip file 
                    //if (sf.m_config.m_exportopt.Contains("ZIP") && sf.m_config.export)
                    if (sf.m_config.export)
                    {
                        // open the existing scene file
                        //store the gcode
                        MemoryStream stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(m_gcode.RawGCode));
                        String gcn = Path.GetFileNameWithoutExtension(UVDLPApp.Instance().SceneFileName) + ".gcode";
                        //SceneFile.Instance().RemoveExistingGCode(UVDLPApp.Instance().SceneFileName);
                        SceneFile.Instance().RemoveResourcesFromFile(UVDLPApp.Instance().SceneFileName, "GCode", ".gcode");
                        SceneFile.Instance().AddGCodeToFile(UVDLPApp.Instance().SceneFileName, stream, gcn);
                    }
                    //save the slicer object for later too                    
                    //save the slice file

                   // UVDLPApp.Instance().m_slicefile.Save(path + UVDLPApp.m_pathsep + fn + ".sliced");
                    break;
                case Slicer.eSliceEvent.eSliceCancelled:
                    DebugLogger.Instance().LogRecord("Slicing Cancelled");
                    break;


            }
        }
        /// <summary>
        /// This is called after the scene file is loaded
        /// It will also load the gcode file and slicing profile / vector slices
        /// </summary>
        public void PostLoadScene() 
        {
            
            
        }
        public void LoadGCode(String filename)
        {
            try
            {
                if (!m_gcode.Load(filename))
                {
                    DebugLogger.Instance().LogRecord("Cannot load GCode File " + filename);
                }
                RaiseAppEvent(eAppEvent.eGCodeLoaded, "GCode Loaded " + filename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }
        public void SaveGCode(String filename) 
        {
            try
            {
                if (!UVDLPApp.Instance().m_gcode.Save(filename))
                {
                    DebugLogger.Instance().LogRecord("Cannot save GCode File " + filename);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }
        
        // a public property to get the 3d engine
        public Engine3d Engine3D { get { return m_engine3d; } }

        public static Platform RunningPlatform()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return Platform.Mac;
                    else
                        return Platform.Linux;

                case PlatformID.MacOSX:
                    return Platform.Mac;

                default:
                    return Platform.Windows;
            }
        }

        /*
         This function returns the path to the current profile terminated by pathsep
         */
        public String ProfilePathString()
        {
            String profilepath = Path.GetDirectoryName(m_appconfig.m_cursliceprofilename);
            profilepath += m_pathsep;
            profilepath += Path.GetFileNameWithoutExtension(m_appconfig.m_cursliceprofilename);
            profilepath += m_pathsep;
            return profilepath;
        }
        public bool LoadBuildSliceProfile(string filename) 
        {
            m_buildparms = new SliceBuildConfig();
            bool ret = m_buildparms.Load(filename);
            if (ret) 
            {
                m_appconfig.m_cursliceprofilename = filename;
                m_appconfig.Save(m_apppath + m_pathsep + m_appconfigname);// this name doesn't change
                RaiseAppEvent(eAppEvent.eSliceProfileChanged, "");
            }
            return ret;
        }


        /*
         This function loads the machine profile and makes it current
         */
        public bool LoadMachineConfig(string filename) 
        {
            bool ret = m_printerinfo.Load(filename);
            if (ret) 
            {
                m_appconfig.m_curmachineeprofilename = filename; // set the app config name
                //save the app config
                m_appconfig.Save(m_apppath + m_pathsep + m_appconfigname);// this name doesn't change
            }
            RaiseAppEvent(eAppEvent.eMachineTypeChanged, "");
            RaiseAppEvent(eAppEvent.eMachineConfigChanged, "");
            m_engine3d.UpdateGrid();
            return ret;
        }

        public void SaveCurrentMachineConfig() 
        {
            try
            {
                m_printerinfo.Save(m_appconfig.m_curmachineeprofilename);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }
        public void SaveCurrentSliceBuildConfig()
        {
            try
            {
                m_buildparms.Save(m_appconfig.m_cursliceprofilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogRecord(ex.Message);
            }
        }

        public void SetupDriver() 
        {
            DebugLogger.Instance().LogRecord("Changing driver type to " + m_printerinfo.m_driverconfig.m_drivertype.ToString());
            if (m_deviceinterface.Driver != null) 
            {
                if (m_deviceinterface.Driver.Connected == true) 
                {
                    // be sure to close the old driver to play nice
                    m_deviceinterface.Driver.Disconnect();
                }
            }
            m_deviceinterface.Driver = DriverFactory.Create(m_printerinfo.m_driverconfig.m_drivertype);            
        }
        public void SaveAppConfig() 
        {
            m_appconfig.Save(m_apppath + m_pathsep + m_appconfigname);  // use full path - SHS
        }
        /// <summary>
        /// This function returns a list of Slice/Build Profiles
        /// </summary>
        /// <returns></returns>
        public List<string>  SliceProfiles() 
        {
            List<string> profiles = new List<string>();
            try
            {

                string[] filePaths = Directory.GetFiles(UVDLPApp.Instance().m_PathProfiles, "*.slicing");
                string curprof = Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_buildparms.m_filename);
                //create a new menu item for all build/slice profiles
                foreach (String profile in filePaths)
                {
                    String pn = Path.GetFileNameWithoutExtension(profile);
                    profiles.Add(pn);
                }
                
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return profiles;
        }

        private void CheckAndCreateDefaultMachines() 
        {
            //Check to see if the null machine exists
            if (!File.Exists(UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + "NullMachine.machine"))
            {
                DebugLogger.Instance().LogWarning("Null Machine not found, creating...");
                // if it doesn't, create it 
                MachineConfig mc = new MachineConfig();
                mc.CreateNullMachine();
                if (!mc.Save(UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + "NullMachine.machine"))
                {
                    DebugLogger.Instance().LogError("Could not save Null Machine");
                }
            }

            //Check to see if the default SLA machine exists
            if (!File.Exists(UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + "Default_SLA.machine"))
            {
                DebugLogger.Instance().LogWarning("Default_SLA Machine not found, creating...");
                // if it doesn't, create it 
                MachineConfig mc = new MachineConfig();
                if (!mc.Save(UVDLPApp.Instance().m_PathMachines + UVDLPApp.m_pathsep + "Default_SLA.machine"))
                {
                    DebugLogger.Instance().LogError("Could not save Default_SLA Machine");
                }
            }

        
        }
        public void DoAppStartup() 
        {
            m_dispmgr = DisplayManager.Instance();// initialze the displays
            m_apppath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //get the path separater 
            if (RunningPlatform() == Platform.Windows)
            {
                m_pathsep = "\\";
            }
            else
            {
                m_pathsep = "/";
            }
            // define some default paths
            m_PathMachines = m_apppath + m_pathsep + "Machines";  // use full paths - SHS
            m_PathProfiles = m_apppath + m_pathsep + "Profiles";

            // set up directories if they don't exist
            if (!Directory.Exists(m_PathMachines)) 
            {
                Utility.CreateDirectory(m_PathMachines);
            }
            if (!Directory.Exists(m_PathProfiles))
            {
                Utility.CreateDirectory(m_PathProfiles);
            }

            if (!m_appconfig.Load(m_apppath + m_pathsep + m_appconfigname))  // use full path - SHS
            {
                m_appconfig.CreateDefault();
                m_appconfig.Save(m_apppath + m_pathsep + m_appconfigname);  // use full path - SHS
            }
            // this will check and make sure the default machines have been created
            CheckAndCreateDefaultMachines();
            //load the current machine configuration file
            if (!m_printerinfo.Load(m_appconfig.m_curmachineeprofilename)) 
            {
               // m_printerinfo.Save(m_appconfig.m_curmachineeprofilename);
                DebugLogger.Instance().LogError("Cannot Load Machine Profile " + m_appconfig.m_curmachineeprofilename);
            }
            // machine configuration was just loaded here.
            RaiseAppEvent(eAppEvent.eMachineTypeChanged, ""); // notify the gui to set up correctly
            //load the projector command list

            if (!LoadProjectorCommands(m_appconfig.ProjectorCommandsFile))
            {
                SaveProjectorCommands(m_appconfig.ProjectorCommandsFile);
            }
            //load the current slicing profile
            if (!m_buildparms.Load(m_appconfig.m_cursliceprofilename)) 
            {
                m_buildparms.CreateDefault();
                m_buildparms.Save(m_appconfig.m_cursliceprofilename);
            }
            // set up the drivers
            SetupDriver();
            //SetupDriverProjector();
            // load the support configuration
            if (!LoadSupportConfig(m_appconfig.SupportConfigName))
            {
                SaveSupportConfig(m_appconfig.SupportConfigName);
            }
            //load the license keys
            LoadLicenseKeys();
            //look for plug-ins and validate licensing
            ScanForPlugins();
            //look for 'lite' plugins
            ScanForPluginsLite();
            // validate those loaded plugins against license keys
            CheckLicensing();
            // initialize the plugins, the main form will send a secondary init after the main app gui is created
            PerformPluginCommand("InitCommand", true);
            m_sc = new ServerContact();
            m_sc.ServerContactEvenet += new ServerContact.Servercontacted(m_sc_ServerContactEvent);
            m_sc.UpdateRegInfo();
            m_undoer.RegisterCallback();
            // pack all 2d gl images
            //m_2d_graphics.GenereteUserTexture();
        }

        void m_sc_ServerContactEvent(string id)
        {
            //throw new NotImplementedException();
        }
        #region Plug-in management and licensing
        private void LoadLicenseKeys() 
        {
            try
            {
                string licensefile = m_apppath + m_pathsep + "licenses.key";
                if (File.Exists(licensefile))
                {
                    KeyRing.Instance().Load(licensefile);
                }
                else 
                {
                    DebugLogger.Instance().LogInfo("No Key Ring found");
                }
            }
            catch (Exception) 
            {
                DebugLogger.Instance().LogInfo("No License File");
            }
        }

        private void CheckLicensing() 
        {
            foreach (PluginEntry pe in m_plugins)
            {
                try
                {
                    // iterate through all loaded plugins
                    // get the vendor id
                    int vid = pe.m_plugin.GetInt("VendorID");
                    LicenseKey lk = KeyRing.Instance().Find(vid);
                    if (lk != null /*|| vid == 0x1000*/) 
                    {
                        pe.m_licensed = true;
                        
                    }
                    int options = pe.m_plugin.GetInt("Options");
                    if (options != -1) 
                    {
                        if ((options & PluginOptions.OPTION_NOLICENSE) != 0) 
                        {
                            pe.m_licensed = true;
                        }
                    }
                    
                }
                catch (Exception ex)             
                {
                    DebugLogger.Instance().LogError(ex);
                }

            }

        }
        

        public void PerformPluginCommand(string cmd,object[] parms, bool verifyLicense)
        {
            foreach (PluginEntry pe in m_plugins)
            {
                try
                {
                    // iterate through all loaded plugins
                    if (!verifyLicense || (pe.m_licensed )) 
                    {
                        if (pe.m_enabled)
                        {
                            pe.m_plugin.ExecuteFunction(cmd, parms);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
        }
        public void PerformPluginCommand(string cmd, bool verifyLicense)
        {
            foreach (PluginEntry pe in m_plugins)
            {
                try
                {
                    // iterate through all loaded plugins
                    if (!verifyLicense || (pe.m_licensed == true))
                    {
                        if (pe.m_enabled == true)
                        {
                            pe.m_plugin.ExecuteFunction(cmd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
        }

        /// <summary>
        /// This function will iterate through all loaded resources,
        /// check and see if they are properly licensed, and return the 
        /// requested image resource
        /// the first matching will be returned.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bitmap GetPluginImage(string name) 
        {
            Bitmap bmp = null;
            foreach (PluginEntry pe in m_plugins)
            {
                try
                {
                    // iterate through all loaded plugins
                    if (pe.m_licensed == true && pe.m_enabled == true) // only check the licensed, enabled plugins
                    {
                        bmp = pe.m_plugin.GetImage(name);
                        if (bmp != null)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
            return bmp;
        }
        /// <summary>
        /// returns the name of the current build / slice profile
        /// </summary>
        /// <returns></returns>
        public string GetCurrentSliceProfileName() 
        {
            return Path.GetFileNameWithoutExtension(m_appconfig.m_cursliceprofilename);            
        }
        //licenses can be in a few states:
        // licensed and enabled - load the plugin and use it
        // licensed and disabled - do not use the plugin, do not initialize
        // un-licensed and enabled - prompt the user for license key, do not initialize it
        // un-licensed and disabled - ignore this plugin, keep entry for it, do not initialize
        public void ScanForPlugins() 
        {

            // load the list of plugin states
            string picn = m_apppath + m_pathsep + "pluginconfig.cfg"; // plugin configuration name
            m_pluginstates.Load(picn);

            // get a list of dll's in this current directory
            // try to register them as a plug-in
            string[] filePaths = Directory.GetFiles(m_apppath, "*.dll");
            foreach (String pluginname in filePaths)
            {
                string args = Path.GetFileNameWithoutExtension(pluginname);
                if (args.ToLower().StartsWith("pl")) 
                {
                    // located a dll that is a potential plugin
                    Type ObjType = null;
                    try
                    {
                        // load it
                        Assembly ass = null;
                        //string args = Path.GetFileNameWithoutExtension(pluginname);
                        ass = Assembly.Load(args);
                        if (ass != null)
                        {
                            ObjType = ass.GetType(args + ".PlugIn"); // look for the plugin interface
                            // OK Lets create the object as we have the Report Type
                            if (ObjType != null)
                            {
                                // create an instance of the plugin
                                IPlugin plug = (IPlugin)Activator.CreateInstance(ObjType); 
                                // create an entry for the plugin
                                PluginEntry pe = new PluginEntry(plug,args);
                                //add the entry to the list of plugins
                                m_plugins.Add(pe);
                                //mark the plugin as enabled by default
                                pe.m_enabled = true; 
                                if (m_pluginstates.InList(args))
                                {
                                    // this plugin is listed in the disabled list.
                                    DebugLogger.Instance().LogInfo("Plugin " + args + " marked disabled");
                                    pe.m_enabled = false; 
                                }

                                //get the vendor id of the newly loaded plugin
                                int vid = plug.GetInt("VendorID");
                                //look for the license key for this plugin
                                LicenseKey lk = KeyRing.Instance().Find(vid);
                                // if we found it, mark it as licensed
                                if (lk != null)
                                {
                                    //initialize the plugin by setting the host.
                                    if(pe.m_enabled)
                                    {
                                        plug.Host = this; // this will initialize the plugin - the plugin's init function will be called
                                        DebugLogger.Instance().LogInfo("Loaded licensed plugin " + args);
                                    }
                                }
                                DebugLogger.Instance().LogInfo("Loaded plugin " + args);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.Instance().LogError(ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// Loading 'lite' plugins from zip file .plg files
        /// </summary>
        public void ScanForPluginsLite()
        {

            // load the list of plugin states
            string picn = m_apppath + m_pathsep + "pluginconfig.cfg"; // plugin configuration name
            m_pluginstates.Load(picn);

            // get a list of dll's in this current directory
            // try to register them as a plug-in
            string[] filePaths = Directory.GetFiles(m_apppath, "*.plg");
            foreach (String pluginname in filePaths)
            {
                try
                {
                    
                        // create an instance of the plugin
                    PluginLite pll = new PluginLite();
                    pll.m_filename = pluginname;
                    pll.LoadManifest();
                    IPlugin plug = (IPlugin)pll;
                    string args = Path.GetFileNameWithoutExtension(pluginname);
                    
                    // create an entry for the plugin
                    PluginEntry pe = new PluginEntry(plug, args);
                    //add the entry to the list of plugins
                    m_plugins.Add(pe);
                    //mark the plugin as enabled by default
                    pe.m_enabled = true;
                    if (m_pluginstates.InList(args))
                    {
                        // this plugin is listed in the disabled list.
                        DebugLogger.Instance().LogInfo("Plugin " + args + " marked disabled");
                        pe.m_enabled = false;
                    }
                    //get the vendor id of the newly loaded plugin
                    int vid = plug.GetInt("VendorID");
                    //look for the license key for this plugin
                    LicenseKey lk = KeyRing.Instance().Find(vid);
                    // if we found it, mark it as licensed
                    if (lk != null)
                    {
                        //initialize the plugin by setting the host.
                        if (pe.m_enabled)
                        {
                            plug.Host = this; // this will initialize the plugin - the plugin's init function will be called
                            DebugLogger.Instance().LogInfo("Loaded licensed plugin " + args);
                        }
                    }
                    DebugLogger.Instance().LogInfo("Loaded plugin " + args);                                       
                }
                catch (Exception ex)
                {
                    DebugLogger.Instance().LogError(ex.Message);
                }
            }
        }


        /// <summary>
        /// This is a plugin host implementation
        /// </summary>
        /// <param name="ipi"></param>
        /// <returns></returns>
        public bool Register(IPlugin ipi)
        {
            //DebugLogger.Instance().LogInfo("Registered: " + ipi.Name);
            return true;
        }
        #endregion
    }
}
