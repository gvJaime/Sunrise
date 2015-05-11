using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Engine3D;
using System.IO;
using System.Xml;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer.Slicing;
using System.Drawing;

namespace UV_DLP_3D_Printer._3DEngine
{
    /// <summary>
    /// This class is a singleton. It's purpose is to load and save an entire scene into a zip file
    /// This allows the user to later load/save the scene that they were previously working on
    /// it will save the scene into a zip file along with an XML file that stores a manifest as well as additional
    /// metadata about each object such as tag info, current selected onject, etc...
    /// this file can also store the png slices (if sliced) or an SVG file
    /// </summary>
    public class SceneFile
    {
        private static SceneFile m_instance = null;
      //  private ZipFile mZip; // this handle is used for when we're slicing and adding entries
        private XmlHelper mManifest; // this handle is used for when we're slicing and adding slice entries
        
        private SceneFile() 
        {
          //  mZip = null;
            mManifest = null;
        }

        private bool FilePatternMatch(string filename,string filepatterns) 
        {
            try
            {
                filepatterns = filepatterns.ToLower();
                filename = filename.ToLower();
                string[] exts = filepatterns.Split('|');
                foreach (string ext in exts)
                {
                    if (filename.EndsWith(ext))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }

            return false;
        }
        /// <summary>
        /// This will remove the specified file resources from the manifest and files from the 
        /// zip file. This is a completely self-contained function that will ensure the disposal 
        /// of the zip file object, we were previously having issues with the file not being completely closed
        /// filepattern can be a list of file extensions patternslike so:
        /// .png|.svg
        /// </summary>
        /// <param name="scenefilename"></param>
        /// <returns></returns>
        public bool RemoveResourcesFromFile(string scenefilename, string section, string filepattern)
        {
            bool ret = true;
            try
            {
                LoadManifest(scenefilename); // load the latest version of the manifest from the zip file
                // open the zip file for read/write
                // use the 'using' block to ensure resource disposal
                using (ZipFile zip1 = ZipFile.Read(scenefilename))
                {
                    XmlNode rmnodes = mManifest.FindSection(mManifest.m_toplevel, section);
                    if (rmnodes != null)
                    {
                        rmnodes.RemoveAll(); // remove all child nodes for this manifest entry
                        List<ZipEntry> etr = new List<ZipEntry>(); // entries to remove
                        foreach (ZipEntry ze in zip1) // create a list of entries to remove
                        {
                            if (FilePatternMatch(ze.FileName, filepattern))
                            {
                                etr.Add(ze);
                            }
                        }
                        //and remove them
                        zip1.RemoveEntries(etr);
                    }
                    zip1.Save(); // save it
                }
                //now update the changed manifest entries
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                ret = false;
            }
            return ret;
        }

        public bool RemoveSingleFile(string scenefilename, string section, string filename)
        {
            bool ret = true;
            try
            {
                LoadManifest(scenefilename); // load the latest version of the manifest from the zip file
                // open the zip file for read/write
                // use the 'using' block to ensure resource disposal
                using (ZipFile zip1 = ZipFile.Read(scenefilename))
                {
                    XmlNode rmnodes = mManifest.FindSection(mManifest.m_toplevel, section);
                    if (rmnodes != null)
                    {
                        List<ZipEntry> etr = new List<ZipEntry>(); // entries to remove
                        foreach (XmlNode xnode in rmnodes.ChildNodes)
                        {
                            if (xnode.InnerText.Equals(filename))
                            {
                                rmnodes.RemoveChild(xnode);
                                if (zip1.ContainsEntry(filename))
                                    etr.Add(zip1[filename]);
                            }
                        }
                        //and remove them
                        zip1.RemoveEntries(etr);
                    }
                    zip1.Save(); // save it
                }
                //now update the changed manifest entries
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                ret = false;
            }
            return ret;
        }

        public bool RemoveResourcesBySection(string scenefilename, string section)
        {
            bool ret = true;
            try
            {
                LoadManifest(scenefilename); // load the latest version of the manifest from the zip file
                // open the zip file for read/write
                // use the 'using' block to ensure resource disposal
                using (ZipFile zip1 = ZipFile.Read(scenefilename))
                {
                    XmlNode rmnodes = mManifest.FindSection(mManifest.m_toplevel, section);
                    if (rmnodes != null)
                    {
                        List<ZipEntry> etr = new List<ZipEntry>(); // entries to remove
                        foreach (XmlNode xnode in rmnodes.ChildNodes)
                        {
                            string filename = xnode.InnerText;
                            if (zip1.ContainsEntry(filename))
                                etr.Add(zip1[filename]);
                            // try inner sections
                            foreach (XmlNode xnodeinner in xnode.ChildNodes)
                            {
                                string filenameinner = xnode.InnerText;
                                if (zip1.ContainsEntry(filenameinner))
                                    etr.Add(zip1[filename]);
                                // try inner sections
                            }
                            
                        }
                        rmnodes.RemoveAll(); // remove all child nodes for this manifest entry
                        //and remove them
                        zip1.RemoveEntries(etr);
                    }
                    zip1.Save(); // save it
                }
                //now update the changed manifest entries
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// This will open the zip file
        /// load the manifest file into this object
        /// and close the zip file
        /// </summary>
        /// <param name="scenefilename"></param>
        /// <returns></returns>
        public bool LoadManifest(string scenefilename) 
        {
            try
            {
                using (ZipFile tmpZip = ZipFile.Read(scenefilename))
                {
                    //open the manifest file                
                    string xmlname = "manifest.xml";
                    ZipEntry manifestentry = tmpZip[xmlname];
                    //get memory stream
                    MemoryStream manistream = new MemoryStream();
                    //extract the stream
                    manifestentry.Extract(manistream);
                    //read from stream
                    manistream.Seek(0, SeekOrigin.Begin); // rewind the stream for reading
                    //create a new XMLHelper to load the stream into
                    mManifest = new XmlHelper();
                    //load the stream
                    mManifest.LoadFromStream(manistream, "manifest");
                } // the end of this using block should close the zip file
                return true;
            }
            catch (Exception ex) 
            {
                mManifest = new XmlHelper();
                mManifest.StartNew("", "manifest"); 
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }
        /// <summary>
        /// this function will tkae the manifest that is in this object and write it to the zip file
        /// this is an atomic operation the zip file will not be held open
        /// </summary>
        /// <returns></returns>
        public bool UpdateManifest(string scenefilename) 
        {
            try
            {
                using (ZipFile tmpzip = ZipFile.Read(scenefilename))
                {
                    string xmlname = "manifest.xml";
                    //remove the old manifest entry
                    try
                    {
                        tmpzip.RemoveEntry(xmlname);
                    }
                    catch (Exception) 
                    {
                        DebugLogger.Instance().LogInfo("Creating new manifest for scene file");
                        // might not be in the file
                    }
                    //create a new memory stream to store the manifest file
                    MemoryStream manifeststream = new MemoryStream();
                    //store the modified manifest stream
                    ZipEntry manifestentry = new ZipEntry();
                    //save the XML document to memorystream
                    if (mManifest != null)
                    {
                        //save the manifest file to a memory stream
                        mManifest.Save(1, ref manifeststream);
                        //rewind to the beginning
                        manifeststream.Seek(0, SeekOrigin.Begin);
                        //save the memorystream for the xml metadata manifest into the zip file
                        tmpzip.AddEntry(xmlname, manifeststream);
                        //save the file
                        tmpzip.Save();
                    }
                    else 
                    {
                        //no existing manifest, start a new one
                        mManifest = new XmlHelper();
                        mManifest.StartNew("", "manifest");
                        tmpzip.Save();
                    }
                } // end of using statement ensures closure of file
                return true;
            }
            catch (Exception ex) 
            {
               // m
                DebugLogger.Instance().LogError(ex);
                return false;
            }
        }
        /// <summary>
        /// Add a single image slice to the entry in the manifest and store in the zip
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddSlice(string scenefilename, MemoryStream ms, string imname)
        {
            try
            {
                LoadManifest(scenefilename);
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {
                    // store the slice file into the zip
                    mZip.AddEntry(imname, ms);
                    mZip.Save();
                } // file should be closed here
                //update the manifest
                // find the slices node in the top level
                XmlNode slicesnode = mManifest.FindSection(mManifest.m_toplevel, "Slices");
                if (slicesnode == null)  // no slice node
                {
                    //create one
                    slicesnode = mManifest.AddSection(mManifest.m_toplevel, "Slices");
                }
                //add the slice file name into the manifest
                XmlNode curslice = mManifest.AddSection(slicesnode, "Slice");
                mManifest.SetParameter(curslice, "name", imname);
                UpdateManifest(scenefilename); // save the new manifest file out
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public bool AddVectorSlice(string scenefilename, MemoryStream ms, string imname)
        {
            try
            {
                LoadManifest(scenefilename);
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {
                    // store the slice file into the zip
                    mZip.AddEntry(imname, ms);
                    mZip.Save();
                }
                //add to the manifest
                // find the slices node in the top level
                XmlNode slicesnode = mManifest.FindSection(mManifest.m_toplevel, "VectorSlices");
                if (slicesnode == null)  // no slice node
                {
                    //create one
                    slicesnode = mManifest.AddSection(mManifest.m_toplevel, "VectorSlices");
                }
                //add the slice file name into the manifest
                XmlNode curslice = mManifest.AddSection(slicesnode, "Slice");
                mManifest.SetParameter(curslice, "name", imname);
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public bool AddPreviewImage(string scenefilename, Image img, string prevtype, string imname)
        {
            try
            {
                LoadManifest(scenefilename);
                RemoveSingleFile(scenefilename, "ScenePreview", imname);
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    // store the slice file into the zip
                    mZip.AddEntry(imname, ms);
                    mZip.Save();
                }
                //add to the manifest
                // find the slices node in the top level
                XmlNode sprev = mManifest.FindSection(mManifest.m_toplevel, "ScenePreview");
                if (sprev == null)  // no gcode node
                {
                    //create one
                    sprev = mManifest.AddSection(mManifest.m_toplevel, "ScenePreview");
                }
                //add the gcode file name into the manifest
                mManifest.SetParameter(sprev, prevtype, imname);
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public void ClearPreviewImages(string scenefilename)
        {
            RemoveResourcesBySection(scenefilename, "ScenePreview");
        }
        
        public bool AddUserFile(string scenefilename, MemoryStream ms, string fileid, string filename)
        {
            try
            {
                LoadManifest(scenefilename);
                RemoveSingleFile(scenefilename, "ScenePreview", filename);
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    // store the slice file into the zip
                    mZip.AddEntry(filename, ms);
                    mZip.Save();
                }
                //add to the manifest
                // find the slices node in the top level
                XmlNode sprev = mManifest.FindSection(mManifest.m_toplevel, "UserFiles");
                if (sprev == null)  // no gcode node
                {
                    //create one
                    sprev = mManifest.AddSection(mManifest.m_toplevel, "UserFiles");
                }
                //add the file name into the manifest
                mManifest.SetParameter(sprev, fileid, filename);
                UpdateManifest(scenefilename);
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public void ClearUserFiles(string scenefilename)
        {
            RemoveResourcesBySection(scenefilename, "UserFiles");
        }

        public static SceneFile Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new SceneFile();
            }
            return m_instance;
        }
        // add a slice to the cws / manifest file
        //add/replace gcode in a cws / manifest file
        public bool AddGCodeToFile(string scenefilename, MemoryStream ms, string gcname) 
        {
            try
            {
                LoadManifest(scenefilename);
                using (ZipFile mZip = ZipFile.Read(scenefilename))                
                {
                    // store the slice file into the zip
                    //add the entry
                    mZip.AddEntry(gcname, ms);
                    //save the file
                    mZip.Save();
                } // the end of the using block should ensure it's closed

                // now update the manifest
                // find the slices node in the top level
                XmlNode gcodenode = mManifest.FindSection(mManifest.m_toplevel, "GCode");
                if (gcodenode == null)  // no gcode node
                {
                    //create one
                    gcodenode = mManifest.AddSection(mManifest.m_toplevel, "GCode");
                }
                //add the gcode file name into the manifest
                mManifest.SetParameter(gcodenode, "name", gcname);                                     
                UpdateManifest(scenefilename);
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

        public bool AddSliceProfileToFile(string scenefilename, MemoryStream ms, string sliceprofilename)
        {
            try
            {
                //get the latest version of the manifest
                LoadManifest(scenefilename); 
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {
                    // store the slice file into the zip
                    mZip.AddEntry(sliceprofilename, ms);
                    mZip.Save();
                }
                // find the slices node in the top level
                XmlNode sliceprofilenode = mManifest.FindSection(mManifest.m_toplevel, "SliceProfile");
                if (sliceprofilenode == null)  // no gcode node
                {
                    //create one
                    sliceprofilenode = mManifest.AddSection(mManifest.m_toplevel, "SliceProfile");
                }
                //add the slice profile file name into the manifest
                mManifest.SetParameter(sliceprofilenode, "name", sliceprofilename);
                UpdateManifest(scenefilename);
                return true;                
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }
        
        public GCodeFile LoadGCodeFromScene(string scenefilename) 
        {
            GCodeFile gcf = null;
            try
            {
                //load the latest version of the manifest
                LoadManifest(scenefilename);
                //find the gcode section
                XmlNode gcn = mManifest.FindSection(mManifest.m_toplevel, "GCode");
                //get the name of the gcode file
                string gcodename = mManifest.GetString(gcn, "name", "none");
                // if there is a gcode file, open the zip and load it
                if (!gcodename.Equals("none"))
                {
                    //open the zip
                    using (ZipFile mZip = ZipFile.Read(scenefilename))
                    {
                        ZipEntry gcodeentry = mZip[gcodename];
                        MemoryStream gcstr = new MemoryStream();
                        gcodeentry.Extract(gcstr);
                        //rewind to beginning
                        gcstr.Seek(0, SeekOrigin.Begin);
                        gcf = new GCodeFile(gcstr);
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return gcf;
        }

        public bool LoadSliceProfileFromScene(string scenefilename)
        {
            try
            {
                LoadManifest(scenefilename);
                XmlNode gcn = mManifest.FindSection(mManifest.m_toplevel, "SliceProfile");
                string sliceprofilename = mManifest.GetString(gcn, "name", "none");
                if (!sliceprofilename.Equals("none"))
                {
                    using (ZipFile mZip = ZipFile.Read(scenefilename))
                    {
                        ZipEntry gcodeentry = mZip[sliceprofilename];
                        MemoryStream gcstr = new MemoryStream();
                        gcodeentry.Extract(gcstr);
                        //rewind to beginning
                        gcstr.Seek(0, SeekOrigin.Begin);
                        //GCodeFile gcf = new GCodeFile(gcstr);
                        UVDLPApp.Instance().m_buildparms = new SliceBuildConfig();
                        UVDLPApp.Instance().m_buildparms.Load(gcstr, sliceprofilename);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }

            return false;
        }
        
        public bool LoadSceneFile(string scenefilename) 
        {
            try
            {
                UVDLPApp.Instance().SceneFileName = scenefilename;
                LoadManifest(scenefilename);
                using (ZipFile mZip = ZipFile.Read(scenefilename))
                {

                    //examine manifest
                    //find the node with models
                    XmlNode topnode = mManifest.m_toplevel;

                    //load gcode if present
                    XmlNode gcn = mManifest.FindSection(mManifest.m_toplevel, "GCode");
                    //get the name of the gcode file
                    string gcodename = mManifest.GetString(gcn, "name", "none");
                    if (!gcodename.Equals("none"))
                    {
                        //open the zip
                        ZipEntry gcodeentry = mZip[gcodename];
                        MemoryStream gcstr = new MemoryStream();
                        gcodeentry.Extract(gcstr);
                        //rewind to beginning
                        gcstr.Seek(0, SeekOrigin.Begin);
                        GCodeFile gcf = new GCodeFile(gcstr);
                        UVDLPApp.Instance().m_gcode = gcf;
                        UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eGCodeLoaded, "GCode Loaded ");
                    }
                    else 
                    {
                        UVDLPApp.Instance().m_gcode = new GCodeFile(""); // empty
                    }

                    // load slice profile if present
                    XmlNode spn = mManifest.FindSection(mManifest.m_toplevel, "SliceProfile");
                    string sliceprofilename = mManifest.GetString(spn, "name", "none");
                    if (!sliceprofilename.Equals("none"))
                    {
                        ZipEntry gcodeentry = mZip[sliceprofilename];
                        MemoryStream gcstr = new MemoryStream();
                        gcodeentry.Extract(gcstr);
                        //rewind to beginning
                        gcstr.Seek(0, SeekOrigin.Begin);
                        //GCodeFile gcf = new GCodeFile(gcstr);
                        UVDLPApp.Instance().m_buildparms = new SliceBuildConfig();
                        UVDLPApp.Instance().m_buildparms.Load(gcstr, sliceprofilename);
                        //create a new slice file based off of the build and slicing parameters
                        UVDLPApp.Instance().m_slicefile = new SliceFile(UVDLPApp.Instance().m_buildparms);
                        UVDLPApp.Instance().m_slicefile.m_mode = SliceFile.SFMode.eLoaded;
                        UVDLPApp.Instance().m_slicer.SliceFile = UVDLPApp.Instance().m_slicefile;
                        UVDLPApp.Instance().m_slicefile.NumSlices = UVDLPApp.Instance().m_slicer.GetNumberOfSlices(UVDLPApp.Instance().m_buildparms);
                        //raise the event to indicate it's loaded
                        UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSliceProfileChanged, "Slice Profile loaded");
                        UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eSlicedLoaded, "Slice Profile loaded");

                    }

                    // now load the models
                    XmlNode models = mManifest.FindSection(topnode, "Models");
                    List<XmlNode> modelnodes = mManifest.FindAllChildElement(models, "model");
                    // bool supportLoaded = false;
                    foreach (XmlNode nd in modelnodes)
                    {
                        string name = mManifest.GetString(nd, "name", "noname");
                        string modstlname = name + ".stl";
                        int tag = mManifest.GetInt(nd, "tag", 0);
                        ZipEntry modelentry = mZip[modstlname]; // the model name will have the _XXXX on the end with the stl extension
                        MemoryStream modstr = new MemoryStream();
                        modelentry.Extract(modstr);
                        //rewind to beginning
                        modstr.Seek(0, SeekOrigin.Begin);
                        //fix the name
                        name = name.Substring(0, name.Length - 5);// get rid of the _XXXX at the end
                        string parentName = mManifest.GetString(nd, "parent", "noname");
                        Object3d obj, tmpObj;
                        switch (tag)
                        {
                            case Object3d.OBJ_SUPPORT:
                            case Object3d.OBJ_SUPPORT_BASE:
                                if (tag == Object3d.OBJ_SUPPORT)
                                    obj = (Object3d)(new Support());
                                else
                                    obj = (Object3d)(new SupportBase());
                                //load the model
                                obj.LoadSTL_Binary(modstr, name);
                                //add to the 3d engine
                                UVDLPApp.Instance().m_engine3d.AddObject(obj);
                                //set the tag
                                obj.tag = tag;
                                obj.SetColor(System.Drawing.Color.Yellow);
                                //find and set the parent
                                tmpObj = UVDLPApp.Instance().m_engine3d.Find(parentName);
                                if (tmpObj != null)
                                {
                                    tmpObj.AddSupport(obj);
                                }
                                //supportLoaded = true;
                                break;

                            default:
                                //load as normal object
                                obj = new Object3d();
                                //load the model
                                obj.LoadSTL_Binary((MemoryStream)modstr, name);
                                //add to the 3d engine
                                UVDLPApp.Instance().m_engine3d.AddObject(obj);
                                //set the tag
                                obj.tag = tag;
                                break;
                        }
                    }
                }
                
                UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelAdded, "Scene loaded");
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
                return false;
            }
        }


        /// <summary>
        /// Save the entire scene into a zip file with a manifest
        /// This file will later be re-used to store png slicee, gcode & svg
        /// </summary>
        /// <param name="scenename"></param>
        /// <returns></returns>
        public bool SaveModelsIntoScene(string scenefilename)
        {
            try
            {
                // get the scene name
                UVDLPApp.Instance().SceneFileName = scenefilename;
               // MemoryStream manifeststream = new MemoryStream(); ;
                //string xmlname = "manifest.xml";
                ZipFile mZip = null;
                bool newfile = false;
                //check to see if the file already exists
                if (File.Exists(scenefilename)) 
                {

                    RemoveResourcesFromFile(scenefilename, "Models", ".stl|.obj|.amf|.dxf");
                    LoadManifest(scenefilename); // reload the manifest
                    mZip = ZipFile.Read(scenefilename);
                }
                else 
                {                    
                    mManifest = new XmlHelper();
                    mManifest.StartNew("", "manifest");                    
                    mZip = new ZipFile();
                    newfile = true;
                    
                    
                }

                XmlNode mc = null;
                //find or create
                mc = mManifest.FindSection(mManifest.m_toplevel, "Models");
                if(mc == null)
                    mc = mManifest.AddSection(mManifest.m_toplevel, "Models");

                using (mZip)
                {
                    //we need to make sure that only unique names are put in the zipentry
                    // cloned objects yield the same name
                    List<string> m_uniquenames = new List<string>();
                    // we can adda 4-5 digit code to the end here to make sure names are unique
                    int id = 0;
                    string idstr;
                    foreach (Object3d obj in UVDLPApp.Instance().m_engine3d.m_objects)
                    {
                        //create a unique id to post-fix item names
                        id++;
                        idstr = string.Format("{0:0000}", id);
                        idstr = "_" + idstr;
                        //create a new memory stream
                        MemoryStream ms = new MemoryStream();
                        //save the object to the memory stream
                        obj.SaveSTL_Binary(ref ms);
                        //rewind the stream to the beginning
                        ms.Seek(0, SeekOrigin.Begin);
                        //get the file name with no extension
                        string objname = Path.GetFileNameWithoutExtension(obj.Name);
                        //spaces cause this to blow up too
                        objname = objname.Replace(' ', '_');
                        // add a value to the end of the string to make sure it's a unique name
                        objname = objname + idstr;
                        string objnameNE = objname;
                        objname += ".stl";  // stl file

                        mZip.AddEntry(objname, ms);
                        //create an entry for this object, using the object name with no extension
                        //save anything special about it

                        //XmlNode objnode = manifest.AddSection(mc, objnameNE);
                        XmlNode objnode = mManifest.AddSection(mc, "model");
                        mManifest.SetParameter(objnode, "name", objnameNE);
                        mManifest.SetParameter(objnode, "tag", obj.tag);
                        if (obj.tag != Object3d.OBJ_NORMAL && obj.m_parent != null)
                        {
                            // note it's parent name in the entry
                            mManifest.SetParameter(objnode, "parent", Path.GetFileNameWithoutExtension(obj.m_parent.Name));
                        }
                    }
                    if (newfile)
                    {
                        mZip.Save(scenefilename);
                    }
                    else
                    {
                        mZip.Save();
                    }
                } // end using block. mZip is still in scope, but should be closed here...

                UpdateManifest(scenefilename);
                DebugLogger.Instance().LogInfo("Saved scene into " + scenefilename);
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
            return false;
        }

    }
}
