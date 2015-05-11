using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using System.Collections;
using System.Drawing;
using System.Threading;
using UV_DLP_3D_Printer.Slicing;
using Ionic.Zip;
using System.Drawing.Imaging;
using System.IO;
using UV_DLP_3D_Printer._3DEngine;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer
{
    public class Slicer
    {
        public enum eSliceEvent 
        {
            eSliceStarted,
            eLayerSliced,
            eSliceCompleted,
            eSliceCancelled
        }
        public enum eSliceMethod
        {
            eEvenOdd,
            eNormalCount
        }
        public delegate void SliceEvent(eSliceEvent ev, int layer, int totallayers,SliceFile sf);

        private SliceFile m_sf; // the current file being sliced
        public SliceEvent Slice_Event;
        private Thread m_slicethread;
        private bool m_cancel = false;
        private bool isslicing = false;
        public eSliceMethod m_slicemethod = eSliceMethod.eEvenOdd;

        public Slicer() 
        {
        
        }
        public SliceFile SliceFile 
        {
            get { return m_sf; }
            set { m_sf = value; }
        }
        public bool IsSlicing { get { return isslicing; } }
        public void CancelSlicing() 
        {
            m_cancel = true;
            isslicing = false;
        }

        public void RaiseSliceEvent(eSliceEvent ev, int curlayer, int totallayers)
        {
            if (Slice_Event != null) 
            {
                Slice_Event(ev, curlayer, totallayers, m_sf);
            }
        }
        /// <summary>
        /// This will get the number of slices in the scene from the specified slice config
        /// This uses the Scene object from the app, we slice with individual objects though
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public int GetNumberOfSlices(SliceBuildConfig sp)
        {
            try
            {
                //UVDLPApp.Instance().CalcScene();
                MinMax mm = UVDLPApp.Instance().Engine3D.CalcSceneExtents(); // get the scene min/max
                //int numslices = (int)((UVDLPApp.Instance().Scene.m_max.z - UVDLPApp.Instance().Scene.m_min.z) / sp.ZThick);
                //int numslices = (int)((mm.m_max - mm.m_min) / sp.ZThick);
                int numslices = CalcNumSlices(mm.m_max, sp.ZThick); // height of whole scene
                return numslices;
            }
            catch (Exception) 
            {
                m_cancel = true;
                return 0;
            }
        }

        // standard scene slicing
        // this function takes the object, the slicing parameters,
        // and the output directory. it generates the object slices
        // and saves them in the directory
        public SliceFile Slice(SliceBuildConfig sp)//, Object3d obj) 
        {
            // create new slice file
            m_sf = new SliceFile(sp);
            m_sf.m_modeltype = Slicing.SliceFile.ModelType.eScene;
            if (sp.export == false)
            {
                m_sf.m_mode = SliceFile.SFMode.eImmediate;
            }
            m_slicethread = new Thread(new ThreadStart(slicefunc));
            m_slicethread.Start();
            isslicing = true;
            return m_sf;
        }

        // slicing of special objects. this is done in immediate mode only. no thread needed
        public SliceFile Slice(SliceBuildConfig sp, SliceFile.ModelType modeltype)//, Object3d obj)
        {
            int numslices = 0;
            string scenename = "";
            switch (modeltype)
            {
                case SliceFile.ModelType.eScene:
                    return Slice(sp);
                    //break;

                case SliceFile.ModelType.eResinTest1:
                    numslices = (int)(7.0 / sp.ZThick);
                    scenename = "Test Model V1";
                    break;
            }

            m_sf = new SliceFile(sp);
            m_sf.m_modeltype = modeltype;
            m_sf.m_mode = SliceFile.SFMode.eImmediate;
            m_sf.NumSlices = numslices;
            SliceStarted(scenename, numslices);
            DebugLogger.Instance().LogRecord("Test model slicing started");
            SliceCompleted(scenename, 0, numslices);
            return m_sf;
        }

        private static Bitmap ReflectX(Bitmap source)
        {
            try
            {
                source.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Bitmap b = new Bitmap(source.Width, source.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.DrawImage(source, 0, 0, source.Width, source.Height);
                }
                b.Tag = BuildManager.SLICE_NORMAL; // added to dispose of old images
                return b;
            }
            catch { return null; }

        }
        private static Bitmap ReflectY(Bitmap source)
        {
            try
            {
                source.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Bitmap b = new Bitmap(source.Width, source.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.DrawImage(source, 0, 0, source.Width, source.Height);
                }
                b.Tag = BuildManager.SLICE_NORMAL;
                return b;
            }
            catch { return null; }

        }
        private static Bitmap ResizeImage(Bitmap imgToResize, Size size)
        {
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)b))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(imgToResize, 0, 0, size.Width, size.Height);
                }
                b.Tag = BuildManager.SLICE_NORMAL;
                return b;
            }
            catch { return null; }
        }

        private static SliceBuildConfig m_saved = new SliceBuildConfig();
        /// <summary>
        /// This get slice immediate is currently for previewing only
        /// </summary>
        /// <param name="curz"></param>
        /// <returns></returns>
        public Slice GetSliceImmediate(float curz, bool SliceSelectedOnly)
        {
            try
            {
                SliceBuildConfig sbf = new SliceBuildConfig(m_sf.m_config); // save it
                Slice sl = new Slice();//create a new slice
                sl.m_segments = new List<PolyLine3d>();
                foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
                {
                    if (SliceSelectedOnly && !obj.m_inSelectedList)
                        continue;

                    if (curz >= obj.m_min.z && curz <= obj.m_max.z) // only slice from the bottom to the top of the objects
                    {
                        List<Polygon> lstply = GetZPolys(obj, curz);//get a list of polygons at this slice z height that potentially intersect
                        List<PolyLine3d> lstintersections = GetZIntersections(lstply, curz);//iterate through all the polygons and generate x/y line segments at this 3d z level                        
                        sl.m_segments.AddRange(lstintersections);// Set the list of intersections                         
                    }
                }

                return sl;
            }
            catch (Exception ex)
            {
                string s = ex.StackTrace;
                DebugLogger.Instance().LogRecord(ex.Message);
                return null;
            }        
        
        }

        public Slice GetSliceImmediate(float curz)
        {
            return GetSliceImmediate(curz, false);
        }

        
        /// <summary>
        /// This function will immediately return a bitmap slice at the specified Z-Level
        /// If lstPoly is not null, a list of vector polylines representing the slice is returned
        /// </summary>
        /// <param name="zlev"></param>
        /// <param name="lstPoly"></param>
        /// <returns></returns>
        //public Bitmap SliceImmediate(float curz, List<PolyLine3d> lstPoly = null)
        public Bitmap SliceImmediate(float curz, List<PolyLine3d> allIntersections, bool outline = false)
        {
            try
            {
                //first take care of scaling up the output bitmap paramters size, so we can re-sample later
                double scaler = 1.5; // specify the scale factor
                m_saved.CopyFrom(m_sf.m_config);// save the orginal
                if (m_sf.m_config.antialiasing == true)
                {
                    scaler = m_sf.m_config.aaval;
                    m_sf.m_config.dpmmX *= scaler;
                    m_sf.m_config.dpmmY *= scaler;
                    m_sf.m_config.xres = (int)(scaler * m_sf.m_config.xres);
                    m_sf.m_config.yres = (int)(scaler * m_sf.m_config.yres);
                }
                SliceBuildConfig sbf = new SliceBuildConfig(m_sf.m_config); // save it

                Bitmap bmp = new Bitmap(m_sf.m_config.xres, m_sf.m_config.yres); // create a new bitmap on a per-slice basis                         
                Graphics graph = Graphics.FromImage(bmp);
                graph.Clear(UVDLPApp.Instance().m_appconfig.m_backgroundcolor);//clear the image for rendering               

                //convert all to 2d lines
                Bitmap savebm = null;
                // check for cancelation

                foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
                {
                    savebm = bmp; // need to set this here in case it's not rendered
                    if (curz >= obj.m_min.z && curz <= obj.m_max.z) // only slice from the bottom to the top of the objects
                    {
                        List<Polygon> lstply = GetZPolys(obj, curz);//get a list of polygons at this slice z height that potentially intersect
                        List<PolyLine3d> lstintersections = GetZIntersections(lstply, curz);//iterate through all the polygons and generate x/y line segments at this 3d z level
                        Slice sl = new Slice();//create a new slice
                        sl.m_segments = lstintersections;// Set the list of intersections 
                        sl.RenderSlice(m_sf.m_config, ref bmp,outline);
                        if (allIntersections != null)
                            allIntersections.AddRange(lstintersections);
                        savebm = bmp;
                    }
                }

                if (m_sf.m_config.antialiasing == true) // we're using anti-aliasing here, so resize the image
                {
                    savebm = ResizeImage(bmp, new Size(m_saved.xres, m_saved.yres));
                }
                if (m_sf.m_config.m_flipX == true)
                {
                    savebm = ReflectX(savebm);
                }
                if (m_sf.m_config.m_flipY == true)
                {
                    savebm = ReflectY(savebm);
                }
                //restore the original size
                m_sf.m_config.CopyFrom(m_saved);
                return savebm;
            }
            catch (Exception ex)
            {
                string s = ex.StackTrace;
                DebugLogger.Instance().LogError(ex);
                return null;
            }

        }

        public Bitmap SliceImmediate(float curz, bool outline = false)
        {
            return SliceImmediate(curz, null,outline);
        }


        int CalcNumSlices(double height, double zThick)
        {
            int numslices = (int)(height / zThick);
            //if ((((double)numslices + 0.5) * zThick) <= height)
            if ((((double)numslices + (zThick/2)) * zThick) <= height)
                    numslices++;
            return numslices;
        }

         private void slicefunc()
        {
            try
            {
                MinMax mm = UVDLPApp.Instance().Engine3D.CalcSceneExtents();
                int numslices = CalcNumSlices(mm.m_max, m_sf.m_config.ZThick);
                float curz = (float)(m_sf.m_config.ZThick / 2.0); // start at half slice thickness             
                int c = 0;
                string scenename = UVDLPApp.Instance().SceneFileName;
                // a little housework here...
                foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
                {
                    obj.FindMinMax();
                }

                m_sf.NumSlices = numslices;
                SliceStarted(scenename, numslices);
                DebugLogger.Instance().LogRecord("Slicing started");

                if (m_sf.m_config.export == false)
                {
                    // if we're not actually exporting slices right now, then 
                    // raise the completed event and exit
                    SliceCompleted(scenename, 0, numslices);
                    //m_sf.m_config.CopyFrom(m_saved);
                    isslicing = false;
                    return; // exit slicing, nothing more to do...
                }

                // if we're actually exporting something here, iterate through slices
                for (c = 0; c < numslices; c++)
                {
                    List<PolyLine3d> lstintersections = null;
                    if (m_sf.m_config.exportsvg != 0)
                        lstintersections = new List<PolyLine3d>();
                    Bitmap savebm = SliceImmediate(curz, lstintersections);
                    Bitmap outlinebm = null;
                    //check to see if we're rendering the outline BM too
                    if (m_sf.m_config.m_createoutlines == true)
                    {
                        outlinebm = SliceImmediate(curz, lstintersections, true);
                    }
                    if (m_cancel || (savebm == null))
                    {
                        isslicing = false;
                        m_cancel = false;
                        //restore the original sizes 
                        //m_sf.m_config.CopyFrom(m_saved);
                        RaiseSliceEvent(eSliceEvent.eSliceCancelled, c, numslices);
                        return;
                    }
                    curz += (float)m_sf.m_config.ZThick;// move the slice for the next layer
                    //raise an event to say we've finished a slice
                    LayerSliced(scenename, c, numslices, savebm, lstintersections);
                    if (m_sf.m_config.m_createoutlines == true) // make sure we export the slice too
                    {
                        LayerSliced(scenename, c, numslices, outlinebm, lstintersections, true);
                    }
                }
                // restore the original
                m_sf.m_config.CopyFrom(m_saved);
                SliceCompleted(scenename, c, numslices);
                DebugLogger.Instance().LogRecord("Slicing Completed");
                isslicing = false;

            }
            catch (Exception ex)
            {
                string s = ex.StackTrace;
                DebugLogger.Instance().LogRecord(ex.Message);
                //RaiseSliceEvent(eSliceEvent.eSliceCancelled,0,0);
                m_cancel = true;
            }
        }
        
        private void SliceStarted(string scenename, int numslices) 
        {
            if ( m_sf.m_config.export == true) // if we're exporting
            {
                //exporting to cws file
                //get the name oif the scene file
                if (UVDLPApp.Instance().SceneFileName.Length == 0) 
                {
                    MessageBox.Show("Please Save the Scene First Before Exporting Slices");
                    CancelSlicing();
                    return;
                }
                if (m_sf.m_config.exportpng == true) 
                {
                    // if we're exporting png slices to disk as well, then make sure we have a directory to export them into
                    try
                    {
                        string exportdirname = SliceFile.GetSliceFilePath(UVDLPApp.Instance().SceneFileName);
                        if (!Directory.Exists(exportdirname))  // if the directory does not exist
                        {
                            //create the directory to export images into
                            Directory.CreateDirectory(exportdirname);
                            //create the /preview directory here?
                        }

                    }
                    catch (Exception ex) 
                    {
                        DebugLogger.Instance().LogError(ex);
                    }
                }
                if (UVDLPApp.Instance().SceneFileName.Length != 0) // check again to make sure we've really got a name
                {
                    //remove all the previous images first
                    //remove the png slices
                    //SceneFile.Instance().RemoveResourcesFromFile(UVDLPApp.Instance().SceneFileName, "Slices", ".png");
                    SceneFile.Instance().RemoveResourcesBySection(UVDLPApp.Instance().SceneFileName, "Slices");
                    //remove the vector slices
                    SceneFile.Instance().RemoveResourcesFromFile(UVDLPApp.Instance().SceneFileName, "VectorSlices", ".svg");
                    //remove any slice profile in the scene file
                    SceneFile.Instance().RemoveResourcesFromFile(UVDLPApp.Instance().SceneFileName, "SliceProfile", ".slicing");
                    //create a memory stream to hold the slicing profile in memory
                    MemoryStream ms = new MemoryStream();
                    //serialize the slciing profile into the memory stream
                    string sliceprofilename = Path.GetFileNameWithoutExtension(UVDLPApp.Instance().m_buildparms.m_filename) + ".slicing";
                    UVDLPApp.Instance().m_buildparms.Save(ms, sliceprofilename);
                    ms.Seek(0, SeekOrigin.Begin); // rewind
                    //save the stream to the scene cws zip file
                    SceneFile.Instance().AddSliceProfileToFile(UVDLPApp.Instance().SceneFileName, ms, sliceprofilename);
                    // if we've saved this scene before, then we can save the images into it. Open it up for add
                }
                else 
                {
                    //no name? cancel slicing
                    CancelSlicing();
                }
            }
            RaiseSliceEvent(eSliceEvent.eSliceStarted, 0, numslices);
        }


        StreamWriter GenerateSVG(List<PolyLine3d> lstPoly, bool isFillPoly)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            double width = UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            double height = UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
            sw.WriteLine("<!-- Created with CreationWorkshop (http://www.envisionlabs.net/) -->");
            sw.WriteLine();
            sw.WriteLine("<svg width=\"{0}mm\" height=\"{1}mm\" viewBox=\"{2} {3} {0} {1}\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">", width, height, -width/2, -height/2);
            if (isFillPoly)
            {
                // sort polygons into display layers
                int[] dispLevel = new int[lstPoly.Count];
                int i, j, k;
                int maxLevel = 0;
                for (i = 0; i < lstPoly.Count; i++)
                {
                    dispLevel[i] = 0;
                    for (j = 0; j < lstPoly.Count; j++)
                    {
                        if (j == i)
                            continue;
                        if (lstPoly[j].PointInPoly2D(lstPoly[i].m_points[0].x, lstPoly[i].m_points[0].y))
                            dispLevel[i]++;
                    }
                    if (dispLevel[i] > maxLevel)
                        maxLevel = dispLevel[i];
                }

                // draw polygons layer by layer
                for (k = 0; k <= maxLevel; k++)
                {
                    for (j = 0; j < lstPoly.Count; j++)
                    {
                        if (dispLevel[j] != k)
                            continue;
                        PolyLine3d pl = lstPoly[j];
                        int plen = pl.m_points.Count;
                        if (pl.m_points[0].Matches(pl.m_points[plen - 1]))
                            plen--; // no need for last point if it matches the firat
                        // determine polygon direction
                   
                        float dir = 0;
                        for (i = 1; i < plen; i++)
                        {
                            dir += (pl.m_points[i].x - pl.m_points[i - 1].x) * (pl.m_points[i].y + pl.m_points[i - 1].y);
                        }
                        dir += (pl.m_points[0].x - pl.m_points[plen - 1].x) * (pl.m_points[0].y + pl.m_points[plen - 1].y);

                        // draw polygon
                        sw.Write("<polygon points=\"");
                        for (i = 0; i < plen; i++)
                        {
                            sw.Write("{0},{1}", pl.m_points[i].x, -pl.m_points[i].y);
                            if (i < (plen - 1))
                                sw.Write(" ");
                        }
                        sw.WriteLine("\" style=\"fill:{0}\" />", dir < 0 ? "black" : "white");
                        // - for some resaon it seems 
                        //     that polygon direction does not work properly so i use layer level instead.
                        //sw.WriteLine("\" style=\"fill:{0}\" />", (k & 1) == 1 ? "black" : "white");
                    }
                }
            }
            else
            {
                sw.Write("<path d=\"");
                foreach (PolyLine3d pl in lstPoly)
                {
                    int plen = pl.m_points.Count;
                    if (pl.m_points[0].Matches(pl.m_points[plen - 1]))
                        plen--; // no need for last point if it matches the firat
                    for (int i = 0; i < plen; i++)
                    {
                        if (i == 0)
                            sw.Write("M{0} {1} ", pl.m_points[i].x, -pl.m_points[i].y);
                        else
                            sw.Write("L{0} {1} ", pl.m_points[i].x, -pl.m_points[i].y);
                    }
                    sw.WriteLine("Z ");
                }
                sw.WriteLine("\" />");
            }

             //<path d="M 15 2 L9.5 18.0 L25.5 22.0 Z M 15.0 0 L7.5 20.0 L22.5 20.0 Z" fill-rule="evenodd"/>
            sw.WriteLine("</svg>");
            sw.Flush();
            sw.BaseStream.Seek(0,SeekOrigin.Begin);
            
            return sw;
        }

        /// <summary>
        /// This will be called when we're exporting 
        /// </summary>
        /// <param name="scenename"></param>
        /// <param name="layer"></param>
        /// <param name="numslices"></param>
        /// <param name="bmp"></param>
        /// <param name="lstPoly"></param>
        private void LayerSliced(string scenename, int layer, int numslices, Bitmap bmp, List<PolyLine3d> lstintersections, bool outline =false) 
        {
            string path;
            try
            {
                // if (m_buildparms.exportimages)
                {
                    // get the model name
                    String modelname = scenename;
                    String outlinename = "";
                    // strip off the file extension
                    path = SliceFile.GetSliceFilePath(modelname);
                    if (outline) 
                    {
                        outlinename = "_outline";
                    }
                    String imname = Path.GetFileNameWithoutExtension(modelname) + outlinename + String.Format("{0:0000}", layer) + ".png";
                    String imagename = path + UVDLPApp.m_pathsep + imname;
                    // create a memory stream for this to save into
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin); // seek back to beginning
                    if (!m_cancel) // if we're not in the process of cancelling
                    {
                        SceneFile.Instance().AddSlice(UVDLPApp.Instance().SceneFileName,ms, imname);
                    }
                    
                    if (m_sf.m_config.exportpng) 
                    {
                        //imagename
                        bmp.Save(imagename);
                    }
                    if (lstintersections != null)
                    {
                        StreamWriter sw;
                        imname = Path.GetFileNameWithoutExtension(modelname) + String.Format("{0:0000}", layer) + ".svg";
                        imagename = path + UVDLPApp.m_pathsep + imname;
                        if (m_sf.m_config.exportsvg < 3)
                        {
                            Path2D vectorPath = new Path2D(lstintersections);
                            sw = vectorPath.GenerateSVG(UVDLPApp.Instance().m_printerinfo.m_PlatXSize,
                                UVDLPApp.Instance().m_printerinfo.m_PlatYSize, m_sf.m_config.exportsvg == 2);
                        }
                        else
                        {
                            Slice sl = new Slice();
                            sl.m_segments = lstintersections;
                            sl.Optimize();
                            sw = GenerateSVG(sl.m_opsegs, m_sf.m_config.exportsvg == 4);
                        }
                        if (!m_cancel)
                        {
                            SceneFile.Instance().AddVectorSlice(UVDLPApp.Instance().SceneFileName,(MemoryStream)sw.BaseStream, imname);
                        }
                    }

                    RaiseSliceEvent(eSliceEvent.eLayerSliced, layer, numslices);
                }
            }
            catch (Exception ex) 
            {
                string s = ex.StackTrace;
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
        /*
        private void LayerSliced(string scenename, int layer, int numslices, Bitmap bmp)
        {
            LayerSliced(scenename, layer, numslices, bmp, null);
        }
        */
        private void SliceCompleted(string scenename, int layer, int numslices) 
        {
            RaiseSliceEvent(eSliceEvent.eSliceCompleted, layer, numslices);
        }

        /*
         This function takes in a list of polygons along with a z height.
         * What is returns is an ArrayList of 3d line segments. These line segments correspond
         * to the intersection of a plane through the polygons. Each polygon may return 0 or 1 line intersections 
         * on the 2d XY plane
         * I beleive I can determine the winding order (inside or outside facing), based off of the polygon normal
         */
        public List<PolyLine3d> GetZIntersections(List<Polygon> polys, float zcur) 
        {
            try
            {
                List<PolyLine3d> lstlines = new List<PolyLine3d>();
                foreach (Polygon poly in polys) 
                {
                    PolyLine3d s3d = poly.IntersectZPlane(zcur);
                    if (s3d != null) 
                    {
                        lstlines.Add(s3d);
                    }
                }
                return lstlines;
            }
            catch (Exception) 
            {
                return null;
            }
        }

        /*
         Return a list of polygons that intersect at this zlevel
         */
        public List<Polygon> GetZPolys(Object3d obj, double zlev) 
        {
            List<Polygon> lst = new List<Polygon>();
            try
            {
                if (zlev >= obj.m_min.z && zlev <= obj.m_max.z)
                {
                    foreach (Polygon p in obj.m_lstpolys)
                    {
                        //check and see if current z level is between any of the polygons z coords
                        //MinMax mm = p.CalcMinMax();
                        if (p.m_minmax.InRange(zlev))
                        {
                            lst.Add(p);
                        }
                    }
                }                
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return lst;
        }

        // this function generates slices for a test object needed for layer calibration
        public Bitmap GetTestModelV1Slice(int layer)
        {
            float pw = 50; // test pattern width (user parameter?)
            float ph = 20; // test pattern height
            float xdp = (float)m_sf.m_config.dpmmX;
            float ydp = (float)m_sf.m_config.dpmmY;
            int bw = m_sf.m_config.xres;
            int bh = m_sf.m_config.yres;
            // make sure the test pattern is not too wide
            if ((pw * xdp) > (0.9f * bw))
            {
                pw = (0.9f * bw) / xdp;
                ph = (0.9f * bh) / ydp;
            }
            Bitmap bmp = new Bitmap(bw, bh); // create a new bitmap on a per-slice basis                    
            Graphics graph = Graphics.FromImage(bmp);
            graph.Clear(UVDLPApp.Instance().m_appconfig.m_backgroundcolor); //clear the image for rendering
            Brush br = new SolidBrush(UVDLPApp.Instance().m_appconfig.m_foregroundcolor);
            int testlayer = m_sf.NumSlices - 10;
            if (layer < 10)
            {
                // generate a 54 * 24 mm base
                float w = xdp * (pw + 4);
                float h = ydp * (ph + 4);
                float x = (bw - w) / 2;
                float y = (bh - h) / 2;
                graph.FillRectangle(br, x, y, w, h);
            }
            else 
            {
                // generate support walls
                float w = xdp * pw;
                float h = ydp * ph;
                float x = (bw - w) / 2;
                float y = (bh - h) / 2;
                float step = w / 5;
                // vertical bars
                for (int i = 0; i < 6; i++)
                {
                    float x1 = x + i * step - xdp;
                    graph.FillRectangle(br, x1, y, 2 * xdp, h);
                }
                // center horizontal bar
                float y1 = y + h / 2 - ydp;
                graph.FillRectangle(br, x, y1, w, 2 * xdp);
            }
            if ((layer >= testlayer) && (layer < m_sf.NumSlices))
            {
                float w = xdp * pw;
                float h = ydp * ph;
                float x = (bw - w) / 2;
                float y = (bh - h) / 2;
                float stepx = w / 5;
                float stepy = h / 2;
                for (int i = 0; i <= layer - testlayer; i++)
                {
                    float x1 = x + stepx * (i % 5);
                    float y1 = y + stepy * (i / 5);
                    graph.FillRectangle(br, x1, y1, stepx, stepy);
                }
            }
            return bmp;
        }

    }
}
