using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UV_DLP_3D_Printer._3DEngine;
using Engine3D;
using UV_DLP_3D_Printer.Configs;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using UV_DLP_3D_Printer.Slicing;

namespace UV_DLP_3D_Printer
{
    /*
     This class generates support structures for the scene.
     * I'm going to start off by doing the following:
     * walk from the min/max x/y in 1mm resolution in 3d
     * generate a ray from the z=0 to the zMax build size and test the scene for intersection,
     * at intersection points, we can generate cylinders
     * 
     * 
     * On thing that I need to fix/add with the support generator is the ability to generate supports for
     * overhangs, one method that I could potentially use to detect overhangs or islands is to examine the generated 
     * 2d slices.
     * I can iterate through the vertical z slices, and identify regions where new material appears. from comparing previous frames,
     * I can determine the slope, and where new supports may be needed.
     * I think I can do this in 2d, I can then generated supports either in 2d or 3d
     * // the model can be treated as volumetric data
     * foreach slice z
     *      foreach pixel y
     *          foreach pixel x
     *              if(pixel color is black)
     *              {
     *                  
     *              }
     * 
     * This first implementation is a simple x/y scan,
     * further implementations can use more than a cylinder object, or 
     * modify the class to generate new sements on demand
     * We could use the concept of a slice to represent a plane of points
     * this plane of points can then be extruded along a path,
     * able to add or remove new segments.
     * We can use this for many porposes
     * for now, it has to be fairly convex, or generating a center point for
     * surface triangulation may not always work.
     * we can use circle/shape funtions to generate segments/slices
     */
    public enum SupportEvent 
    {
        eStarted, // the support generatation just started
        eCompleted, // the support generation completed
        eCancel, // the suport generator is in a cancelled state
        eProgress, // we've move 1 across the x plane 
        eSupportGenerated, // used to add a model
    }
    public delegate void SupportGeneratorEvent(SupportEvent evnt, string message, Object obj);

    
    public class AdaptiveSupportParams
    {
        public int[] lbm; // current slice bitmap. all pixel values are either 0 or num of current slice
        public int[] lbmz; // z buffer of slices. all non zero pixel values show the closest slice seen from top.
        public int[] lbms; // support map. each added support fiils a rectangle of pixels in this map. pixel value is the height of the support base
        public int[] searchmap; // bitmap of pixels already processed. (no need to check them)
        public int xres, yres; // actually the size of the slice bitmap
        public int mingap;
        public int noverlaped;
        public int nsupported;
        public int minHeight; // minimum support height in number of layers
    }

    public class SupportGenerator
    {
        private SupportConfig m_sc;
        private Object3d m_model;
        private bool m_cancel;
        private bool m_generating; // true while this is running
        public SupportGeneratorEvent SupportEvent;
        private int m_supportgap;
        private AdaptiveSupportParams m_asp;

        public void RaiseSupportEvent(SupportEvent evnt, string message, Object obj) 
        {
            if (SupportEvent != null) 
            {
                SupportEvent(evnt, message, obj);
            }
        }

        public bool Generating 
        {
            get { return m_generating; }
        }

        public SupportGenerator() 
        {
            m_cancel = false;
            m_asp = new AdaptiveSupportParams();
        }
        /// <summary>
        /// Start the support generation
        /// </summary>
        public void Start(SupportConfig sc, Object3d model)
        {
            Thread m_thread = new Thread(new ThreadStart(StartGenerating));
            m_sc = sc;
            m_model = model;
            m_cancel = false;
            m_generating = true;
            m_thread.Start();
        }
        /// <summary>
        /// Cancel the support generation
        /// </summary>
        public void Cancel() 
        {
            m_cancel = true;
            m_generating = false;
        }
        private void StartGenerating() 
        {
            RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eStarted, "Support Generation Started", null);
            switch (m_sc.eSupType) 
            {
                case SupportConfig.eAUTOSUPPORTTYPE.eBON:
                    GenerateSupportObjects();
                    break;
                case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE:
                //case SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE2:
                    GenerateAdaptive2();
                    break;
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "Support generation ended");
        }
        /// <summary>
        /// This is a helper function that converts 3d polylines to 2d
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private List<Line2d> Get2dLines(SliceBuildConfig sp, List<PolyLine3d> segments)
        {
            List<Line2d> lst = new List<Line2d>();
            // this can be changed at some point to assume that the 3d polyline has more than 2 points
            // I'll need to do this when I want to properly generate inside / outside countours
            foreach (PolyLine3d ply in segments)
            {
                Line2d ln = new Line2d();
                //get the 3d points of the line
                Point3d p3d1 = (Point3d)ply.m_points[0];
                Point3d p3d2 = (Point3d)ply.m_points[1];
                //convert them to 2d (probably should add an offset to center them)
                ln.p1.x = (int)(p3d1.x * sp.dpmmX);// +hxres;
                ln.p1.y = (int)(p3d1.y * sp.dpmmY);// +hyres;
                ln.p2.x = (int)(p3d2.x * sp.dpmmX);// +hxres;
                ln.p2.y = (int)(p3d2.y * sp.dpmmY);// +hyres;
                lst.Add(ln);
            }
            return lst; // return the list
        }

        private class UnsupportedRegions 
        {
            public UnsupportedRegions(PolyLine3d p) { ply = p; }
            public Point3d Center() 
            {
                Point3d center = new Point3d();
                //center.Set(ply.m_points[0]);
                foreach(Point3d p in ply.m_points)
                {
                    center.x += p.x;
                    center.y += p.y;
                    center.z += p.z;
                }
                center.x /= (float)(ply.m_points.Count);
                center.y /= (float)(ply.m_points.Count);
                center.z /= (float)(ply.m_points.Count);
                return center;
            }
            public PolyLine3d ply;
        };
        /// <summary>
        /// This is the auto support generation routine for
        /// the app that can do the following:
        /// gridded x/y supports
        /// handle all un-supported regions
        /// generate intra-object supports
        /// </summary>
        public void GenerateAutoIntra() 
        {
            //iterate through all models
            // add all points into a master list - allpoly
            // add all polygons into a master list - allpnt
            //iterate through allpoly and generate a list of polys facing downward - downpoly
            // now, here comes the tricky part - 
            //foreach downward poly dply in downpoly
            // foreach point dpnt in dply
            // intersect the scene from dpnt downward
            // now - either it's going to intersect the ground, or another polygon 
            // create a support (or support entry) sup for the dpnt and intersection (ground or poly)
            // add sup to potentials list

            // the above algorithm is completely sufficient. the problem with it is that it will
            // generate far too many supports
            // I think at this point, we do a secondary pass to smartly remove supports

            // we still need to identify 'islands' of unsupported regions / polys
            // and make sure to add a support to them
            // I need to come up with some sort of function to iterate through all polys
            // and mark them as supported or not.
        }
        /// <summary>
        /// NOT CURRENTLY USED
        /// This is the adaptive support generation, it should automatically 
        /// detect overhangs,
        /// The way that it does this is by generating a closed polyline loop for each layer
        /// and checking the 2d projection of the current layer with the previous layer
        /// </summary>
        public void GenerateAdaptive()
        {
            //iterate through all the layers starting from z=0            
            // check every polyline in the current layer to make sure it is encased or overlaps polylines in the previous layer
            // generate a list of unsupported polylines
            // 'check' to see if the polyline can be dropped straight down
            // this has to do slicing of the scene
            try
            {

                SliceBuildConfig config = UVDLPApp.Instance().m_buildparms;
                config.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we've got the correct display size and PixPerMM values   

                if (UVDLPApp.Instance().m_slicer.SliceFile == null)
                {
                    SliceFile sf = new SliceFile(config);
                    sf.m_mode = SliceFile.SFMode.eImmediate;
                    UVDLPApp.Instance().m_slicer.SliceFile = sf; // wasn't set
                }
                //create new list for each layer to hold unsupported regions
                List<UnsupportedRegions> lstunsup = new List<UnsupportedRegions>();
                List<Object3d> lstsupports = new List<Object3d>(); // final list of supports

                int numslices = UVDLPApp.Instance().m_slicer.GetNumberOfSlices(config);
                float zlev = (float)(config.ZThick / 2.0);
                Slice curslice = null;
                Slice prevslice = null;
                int hxres = config.xres / 2;
                int hyres = config.yres / 2;
                for (int c = 0; c < numslices; c++)
                {
                    //bool layerneedssupport = false;
                    if (m_cancel)
                    {
                        RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCancel, "Support Generation Cancelled", null);
                        return;
                    }
                    RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eProgress, "" + c + "/" + numslices, null);

                    Slice sl = UVDLPApp.Instance().m_slicer.GetSliceImmediate(zlev);
                    zlev += (float)config.ZThick;

                    if (sl == null)
                        continue;
                    if (sl.m_segments == null)
                        continue;
                    if (sl.m_segments.Count == 0)
                        continue;
                    sl.Optimize();// find loops
                    //sl.DetermineInteriorExterior(config); // mark the interior/exterior loops
                    prevslice = curslice;
                    curslice = sl;
                    Bitmap bm = new Bitmap(config.xres, config.yres);
                    using (Graphics gfx = Graphics.FromImage(bm))
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        gfx.FillRectangle(brush, 0, 0, bm.Width, bm.Height);
                    }

                    if (prevslice != null && curslice != null)
                    {
                        //render current slice
                        curslice.RenderSlice(config, ref bm);
                        //now render the previous slice overtop the current slice in another color
                        Color savecol = UVDLPApp.Instance().m_appconfig.m_foregroundcolor;
                        UVDLPApp.Instance().m_appconfig.m_foregroundcolor = Color.HotPink;
                        //render previous slice over top
                        prevslice.RenderSlice(config, ref bm);
                        UVDLPApp.Instance().m_appconfig.m_foregroundcolor = savecol; // restore the color
                        // create a lock bitmap for faster pixel access
                        LockBitmap lbm = new LockBitmap(bm);
                        lbm.LockBits();
                        // now, iterate through all optimized polylines in current slice
                        // this approach isn't going to work, we need to iterate through all polyline
                        //segments in a slice at once, each individual segment needs to know 1 thing
                        // 1) the optimized segment it came from

                        //iterate through all optimized polygon segments

                        Dictionary<PolyLine3d, bool> supportmap = new Dictionary<PolyLine3d, bool>();
                        foreach (PolyLine3d pln in curslice.m_opsegs)
                        {
                            bool plysupported = false;
                            List<PolyLine3d> allsegments = new List<PolyLine3d>();
                            List<PolyLine3d> segments = pln.Split(); // split them, retaining the parent
                            allsegments.AddRange(segments);
                            //add all optimized polyline segments into the supported map
                            supportmap.Add(pln, true);
                            //split this optimized polyline back into 2-point segments for easier use

                            List<Line2d> lines2d = Get2dLines(config, allsegments);
                            if (lines2d.Count == 0) continue;

                            // find the x/y min/max
                            MinMax_XY mm = Slice.CalcMinMax_XY(lines2d);
                            // iterate from the ymin to the ymax
                            for (int y = mm.ymin; y < mm.ymax; y++) // this needs to be in scaled value 
                            {
                                //      get a line of lines that intersect this 2d line
                                List<Line2d> intersecting = Slice.GetIntersecting2dYLines(y, lines2d);
                                //      get the list of point intersections
                                List<Point2d> points = Slice.GetIntersectingPoints(y, intersecting);
                                // sort the points in increasing x order                           
                                points.Sort();
                                if (points.Count % 2 == 0)  // is even
                                {
                                    for (int cnt = 0; cnt < points.Count; cnt += 2)  // increment by 2
                                    {
                                        Point2d p1 = (Point2d)points[cnt];
                                        Point2d p2 = (Point2d)points[cnt + 1];
                                        Point pnt1 = new Point(); // create some points for drawing
                                        Point pnt2 = new Point();
                                        pnt1.X = (int)(p1.x + config.XOffset + hxres);
                                        pnt1.Y = (int)(p1.y + config.YOffset + hyres);
                                        pnt2.X = (int)(p2.x + config.XOffset + hxres);
                                        pnt2.Y = (int)(p2.y + config.YOffset + hyres);
                                        //iterate from pnt1.X to pnt2.x and check colors
                                        for (int xc = pnt1.X; xc < pnt2.X; xc++)
                                        {
                                            if (xc >= lbm.Width || xc <= 0) continue;
                                            if (pnt1.Y >= lbm.Height || pnt1.Y <= 0) continue;
                                            try
                                            {
                                                Color checkcol = lbm.GetPixel(xc, pnt1.Y);
                                                // need to check the locked BM here for the right color
                                                // if the pixel color is the hot pink, then this region has some support
                                                // we're going to need to beef this up and probably divide this all into regions on a grid
                                                if (checkcol.R == Color.HotPink.R && checkcol.G == Color.HotPink.G && checkcol.B == Color.HotPink.B)
                                                {
                                                    plysupported = true;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                DebugLogger.Instance().LogError(ex);
                                            }
                                        }
                                    }
                                }
                                else  // flag error
                                {
                                    DebugLogger.Instance().LogRecord("Row y=" + y + " odd # of points = " + points.Count + " - Model may have holes");
                                }
                            }// for y = startminY to endY
                            if (plysupported == false)
                            {

                                // layerneedssupport = true;
                                supportmap[pln] = false;
                                lstunsup.Add(new UnsupportedRegions(pln));
                            }
                        } // for each optimized polyline
                        lbm.UnlockBits(); // unlock the bitmap
                    } // prev and current slice are not null
                    //if (layerneedssupport)
                    //    SaveBM(bm, c); // uncomment this to see the layers that need support
                } // iterating through all slices
                // iterate through all unsupported regions
                // calculate the center
                // add a support from that region going down to the ground (or closest intersected)
                int scnt = 0;
                foreach (UnsupportedRegions region in lstunsup)
                {
                    Support s = new Support();
                    Point3d center = region.Center(); // taking the center of the region is a naive approach 
                    float lz = center.z;
                    lz += .65f;// why is this offset needed?
                    AddNewSupport(center.x, center.y, center.z, scnt++, null, lstsupports);
                }
                RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCompleted, "Support Generation Completed", lstsupports);
                m_generating = false;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        #region Adaptive Slicer ver 2

        // an Island is an area with contiguous white pixels in a slive image
        class SliceIsland
        {
            public int sliceid;  // z location of the slice
            public int islandid; // id of a single island in the slice
            public int minx, miny, maxx, maxy;
            public int pixCount;
            public int supportedCount;
            public int m_supportGap, supportRad;
            public int xres, yres;
            public SliceIsland(int iid, int sid, int xr, int yr)
            {
                minx = miny = 99999999;
                maxx = maxy = -99999999;
                sliceid = sid;
                islandid = iid;
                xres = xr;
                yres = yr;
                pixCount = supportedCount = 0;
                supportGap = 50;
            }

            public int supportGap
            {
                get { return m_supportGap; }
                set { m_supportGap = value; supportRad = supportGap / 2; }
            }

            // lbm = current slice bitmap. all pixel values are either 0 or num of current slice
            // lbmz = z buffer of slices. all non zero pixel values show the closest slice seen from top.
            // lbms = support map. each added support fiils a rectangle of pixels in this map
            // searchmap = bitmap of pixels already processed. (no need to check them)
            // pts = list of points arround which we want to continue flood algorithm.
            

            // check if a pixel is part of the current island. (true if its not zero, and not already tested)
            // update min/max boundaries of island.
            // using the z buffer see if this pixel position is supported by the layer below it.
            void CheckIslandPoint(int[] searchmap, int[] lbm, int[] lbmz, int x, int y, List<int> pts)
            {
                if ((x < 0) || (y < 0) || (x >= xres) || (y >= yres))
                    return;
                int p = y * xres + x;
                int lyr = lbm[p];
                if ((lyr == 0) || (searchmap[p] != 0))
                    return;
                searchmap[p] = islandid;
                pts.Add(p);
                if (x > maxx) maxx = x;
                if (x < minx) minx = x;
                if (y > maxy) maxy = y;
                if (y < miny) miny = y;
                pixCount++;
                if (lbmz[p] == (lyr - 1))
                    supportedCount++;
            }

            // find an island in a slice using flood fill
            // start from a given point and iterate on all neighbours untill no neighbors left.
            public void FloodIsland(int [] searchmap, int[] lbm, int[] lbmz, int xp, int yp)
            {
                List<int> fillPts = new List<int>();
                List<int> tpts;
                CheckIslandPoint(searchmap, lbm, lbmz, xp, yp, fillPts);
                while (fillPts.Count > 0)
                {
                    tpts = fillPts;
                    fillPts = new List<int>();
                    foreach (int p in tpts)
                    {
                        int x = p % xres;
                        int y = p / xres;
                        CheckIslandPoint(searchmap, lbm, lbmz, x + 1, y, fillPts);
                        CheckIslandPoint(searchmap, lbm, lbmz, x, y + 1, fillPts);
                        CheckIslandPoint(searchmap, lbm, lbmz, x - 1, y, fillPts);
                        CheckIslandPoint(searchmap, lbm, lbmz, x, y - 1, fillPts);
                    }
                }
            }

            // check if a pixel belongs to the currently flooded support area.
            // it belongs if it is near the support spot and in the same island as the support 
            void CheckSupportPoint(int[] searchmap, int x, int y, int supx, int supy, List<int> pts)
            {
                if ((x < 0) || (y < 0) || (x >= xres) || (y >= yres))
                    return;
                if (Math.Abs(supx - x) > m_supportGap)
                    return;
                if (Math.Abs(supy - y) > m_supportGap)
                    return;
                int p = y * xres + x;
                if (searchmap[p] != islandid)
                    return;
                searchmap[p] = 0;
                pts.Add(p);
            }

            // clear island surface near set support 
            // whenever we decide to place a support, clear all the pixels in the same island
            // that are close enough to this support, so no other supports will be added in this area.
            public void FloodSupport(int [] searchmap, int xp, int yp, int supx, int supy)
            {
                List<int> fillPts = new List<int>();
                List<int> tpts;
                CheckSupportPoint(searchmap, xp, yp, supx, supy, fillPts);
                while (fillPts.Count > 0)
                {
                    tpts = fillPts;
                    fillPts = new List<int>();
                    foreach (int p in tpts)
                    {
                        int x = p % xres;
                        int y = p / xres;
                        CheckSupportPoint(searchmap, x + 1, y, supx, supy, fillPts);
                        CheckSupportPoint(searchmap, x, y + 1, supx, supy, fillPts);
                        CheckSupportPoint(searchmap, x - 1, y, supx, supy, fillPts);
                        CheckSupportPoint(searchmap, x, y - 1, supx, supy, fillPts);
                    }
                }
            }
        }

        // holds a support location: x,y position, and height of base and tip. 
        class SupportLocation
        {
            public int x, y;
            public int ztop, zbottom;
            public SupportLocation(int sx, int sy, int stop, int sbot)
            {
                x = sx;
                y = sy;
                ztop = stop;// &0xFFFFFF;
                zbottom = sbot;// &0xFFFFFF;
            }
        }

        // fill a rectangle of "pixels" in the support bitmap arround a given support position
        void UpdateSupportMap(int[] lbms, int px, int py, int resx, int resy, int zlevel)
        {
            int x1 = px - m_supportgap;
            if (x1 < 0)
                x1 = 0;
            int y1 = py - m_supportgap;
            if (y1 < 0)
                y1 = 0;
            int x2 = px + m_supportgap;
            if (x2 > resx)
                x2 = resx;
            int y2 = py + m_supportgap;
            if (y2 > resy)
                y2 = resy;
            x2 -= x1;
            y2 -= y1;
            int p = y1 * resx + x1;
            for (y1 = 0; y1 < y2; y1++, p += resx - y2)
                for (x1 = 0; x1 < x2; x1++, p++)
                    lbms[p] = zlevel;
        }

        // calculate supports for islands that are completely unsupported by underneath layers. 
        void SupportLooseIsland(int[] searchmap, SliceIsland si, int[] lbm, int[] lbmz, int[] lbms, List<SupportLocation> sl)
        {
            int l = si.maxx - si.minx;
            int w = si.maxy - si.miny;
            int x, y, t, b, p;
            SupportLocation s = null;
            // /*P*/ = need to use parameters here
            if ((l < m_supportgap) && (w < m_supportgap))
            {
                // Island is small, in this case just put as Single support in the center
                x = (si.minx + si.maxx) / 2;
                y = (si.miny + si.maxy) / 2;
                p = y * si.xres + x;
                t = lbm[p];
                b = lbmz[p];
                if ((t != 0) && ((b == 0) || (si.sliceid - b >= m_asp.minHeight))) // disregard supports that are too low
                {
                    s = new SupportLocation(x, y, t, b);
                    sl.Add(s);
                    UpdateSupportMap(lbms, x, y, si.xres, si.yres, t);
                }
            }
            if (s != null)
                return;

            // island is big or irregular shape, iterate over the island surface and add supports.
            for (x = si.minx; x <= si.maxx; x++)
            {
                for (y = si.miny; y < si.maxy; y++)
                {
                    p = y * si.xres + x;
                    if (searchmap[p] != si.islandid)
                        continue;
                    b = lbmz[p]; // &0xFFFFFF;
                    t = lbm[p];
                    if ((b != 0) && (si.sliceid - b < m_asp.minHeight))
                        continue; // disregard supports that are too low
                    // add support to current location, and mark this area as supported
                    s = new SupportLocation(x, y, si.sliceid, b);
                    sl.Add(s);
                    UpdateSupportMap(lbms, x, y, si.xres, si.yres, t);
                    si.FloodSupport(searchmap, x, y, x, y);
                }
            }
        }

        // calculate supports for islands that are at least partially unsupported by underneath layers. 
        void SupportSupportedIsland(int[] searchmap, SliceIsland si, int[] lbm, int[] lbmz, int[] lbms, List<SupportLocation> sl)
        {
            int l = si.maxx - si.minx;
            int w = si.maxy - si.miny;
            int x, y, b, p, t;
            SupportLocation s = null;
            for (x = si.minx; x <= si.maxx; x++)
            {
                for (y = si.miny; y < si.maxy; y++)
                {
                    p = y * si.xres + x;
                    b = lbmz[p];
                    t = lbm[p];
                    if ((lbms[p] >= b) || (searchmap[p] != si.islandid)) // skip if area is already supported
                        continue;
                    if ((b != 0) && (si.sliceid - b < m_asp.minHeight))
                        continue; // disregard supports that are too low
                    // add support to current location, and mark this area as supported
                    s = new SupportLocation(x, y, si.sliceid, b);
                    sl.Add(s);
                    UpdateSupportMap(lbms, x, y, si.xres, si.yres, t);
                    si.FloodSupport(searchmap, x, y, x, y);
                }
            }
        }

        // analyze a single slice
        void ProcessSlice(int[] lbm, int[] lbmz, int[] lbms, int xres, int yres, List<SupportLocation> sl)
        {
            int npix = xres * yres;
            int[] searchmap = new int[npix];
            int x, y, p;
            for (p = 0; p < npix; p++)
                searchmap[p] = 0;

            int islandid = 1;
            List<SliceIsland> islands = new List<SliceIsland>();

            // find islands in the slice
            for (x = 0; x < xres; x++)
                for (y = 0; y < yres; y++)
                {
                    p = y * xres + x;
                    int sid = lbm[p];// &0xFFFFFF;
                    if ((searchmap[p] == 0) && (sid != 0))
                    {
                        SliceIsland si = new SliceIsland(islandid, sid, xres, yres);
                        si.supportGap = m_supportgap;
                        si.FloodIsland(searchmap, lbm, lbmz, x, y);
                        islands.Add(si);
                        islandid++;
                    }
                }

            // locate potential supports locations
            foreach (SliceIsland si in islands)
            {
                // case 1: island is not supported at all.
                if (si.supportedCount == 0)
                {
                    SupportLooseIsland(searchmap, si, lbm, lbmz, lbms, sl);
                }
                // case 2: island is partially supported
                else
                {
                    SupportSupportedIsland(searchmap, si, lbm, lbmz, lbms, sl);
                }
            }
        }

        void CheckSupportPoint(int x, int y, int supx, int supy, int ptid, List<int> pts)
        {
            if ((x < 0) || (y < 0) || (x >= m_asp.xres) || (y >= m_asp.yres))
                return;
            if (Math.Abs(supx - x) > m_asp.mingap)
                return;
            if (Math.Abs(supy - y) > m_asp.mingap)
                return;
            int p = y * m_asp.xres + x;
            if (m_asp.searchmap[p] == ptid)
                return;
            int lyr = m_asp.lbm[p];
            if ((lyr == 0) && (m_asp.lbmz[p] == 0))
                return;
            m_asp.searchmap[p] = ptid;
            if (m_asp.lbmz[p] == (lyr - 1))
                m_asp.noverlaped++;
            if (m_asp.lbms[p] >= m_asp.lbmz[p])
                m_asp.nsupported++;
            pts.Add(p);
        }

        bool UpdateSearchMap(int xp, int yp, int ptid)
        {
                List<int> fillPts = new List<int>();
                List<int> tpts;
                m_asp.nsupported = m_asp.noverlaped = 0;
                CheckSupportPoint(xp, yp, xp, yp, ptid, fillPts);
                while (fillPts.Count > 0)
                {
                    tpts = fillPts;
                    fillPts = new List<int>();
                    foreach (int p in tpts)
                    {
                        int x = p % m_asp.xres;
                        int y = p / m_asp.xres;
                        CheckSupportPoint(x + 1, y, xp, yp, ptid, fillPts);
                        CheckSupportPoint(x, y + 1, xp, yp, ptid, fillPts);
                        CheckSupportPoint(x - 1, y, xp, yp, ptid, fillPts);
                        CheckSupportPoint(x, y - 1, xp, yp, ptid, fillPts);
                    }
                }
                return ((m_asp.noverlaped == 0) || (m_asp.nsupported == 0));
            }
        

        // alternative method of support detection. not used at the moment
        void ProcessSlice2(List<SupportLocation> sl)
        {
            int npix = m_asp.xres * m_asp.yres;
            int b, x, y, p;
            for (p = 0; p < npix; p++)
                m_asp.searchmap[p] = 0;
            int ptid = 0;

            // find islands in the slice
            for (x = 0; x < m_asp.xres; x++)
            {
                for (y = 0; y < m_asp.yres; y++)
                {
                    p = y * m_asp.xres + x;
                    int sid = m_asp.lbm[p];
                    if ((sid != 0) && (m_asp.lbmz[p] != sid - 1) && (m_asp.searchmap[p] == 0))
                    {
                        b = m_asp.lbmz[p];
                        if ((b != 0) && (sid - b < m_asp.minHeight))
                            continue; // disregard supports that are too low
                        ptid++;
                        if (UpdateSearchMap(x, y, ptid))
                        {
                            SupportLocation s = new SupportLocation(x, y, sid, b);
                            sl.Add(s);
                            m_asp.lbms[p] = m_asp.lbmz[p];
                        }
                    }
                }
            }
        }

        Object3d GetSupportParrent(float x, float y, float z)
        {
            //Object3d obj;
            List<Object3d> matchingObjects = new List<Object3d>();
            foreach (Object3d obj in UVDLPApp.Instance().Engine3D.m_objects)
            {
                if (obj.tag != Object3d.OBJ_NORMAL)
                    continue;
                if ((x > obj.m_min.x) && (x < obj.m_max.x) && (y > obj.m_min.y) && (y < obj.m_max.y))
                    matchingObjects.Add(obj);
            }
            if (matchingObjects.Count == 0)
                return null; // Should not happen!
            if (matchingObjects.Count == 1)
                return matchingObjects[0]; // the easy case.

            Point3d origin;
            origin = new Point3d(); // bottom point
            origin.Set(x, y, 0.0f);
            //intersected = false; // reset the intersected flag to be false

            Vector3d up = new Vector3d(); // the up vector
            up.x = 0.0f;
            up.y = 0.0f;
            up.z = 1.0f;

            List<ISectData> lstISects = RTUtils.IntersectObjects(up, origin, matchingObjects, false);
            Object3d objFound = null;
            float minzdiff = 99999999f;
            // find the intersection closest to z.
            foreach (ISectData htd in lstISects)
            {
                float zdiff = Math.Abs(htd.intersect.z - z);
                if (zdiff < minzdiff)
                {
                    minzdiff = zdiff;
                    objFound = htd.obj;
                }
            }
            return objFound;
        }

        // shring the slice bitmap, to minimize edge efects. thick = thickness of border to shrink
        void ShrinkSlice(int[] lbm, int xres, int yres, int thick)
        {
            // process all bitmap but first and last lines to enhance performance by reducing edge checks
            int bsize = xres * (yres - 1) -1;
            int diag1 = xres + 1;
            int diag2 = xres - 1;
            List<int> delpoints = new List<int>();
            for (int j = 0; j < thick; j++)
            {
                delpoints.Clear();
                for (int i = xres + 1; i < bsize; i++)
                {
                    bool candidate = false;
                    if (lbm[i] == 0)
                        continue;
                    if ( ((lbm[i - 1] == 0) && (lbm[i + 1] != 0))
                        || ((lbm[i - xres] == 0) && (lbm[i + xres] != 0))
                        || ((lbm[i - diag1] == 0) && (lbm[i + diag1] != 0))
                        || ((lbm[i - diag2] == 0) && (lbm[i + diag2] != 0)) )
                       delpoints.Add(i);
                }
                foreach (int p in delpoints)
                    lbm[p] = 0;
            }
        }

        /// <summary>
        /// This is the adaptive support generation, it should automatically 
        /// detect overhangs and generate supports acordingly
        /// </summary>
        public void GenerateAdaptive2()
        {
            // iterate through all the layers starting from z=0
            // slice all layers
            // split each layer into non overlapping polygons
            // check each polygon to see if it is completely overhangs in the air, or partially
            //   supported by previous layers.
            // for complete overhangs always create supports
            // for partialy supported polygons, generate supports only if there is no other
            //   support in the vicinity
            try
            {

                SliceBuildConfig config = UVDLPApp.Instance().m_buildparms;
                config.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we've got the correct display size and PixPerMM values   

                if (UVDLPApp.Instance().m_slicer.SliceFile == null)
                {
                    SliceFile sf = new SliceFile(config);
                    sf.m_mode = SliceFile.SFMode.eImmediate;
                    UVDLPApp.Instance().m_slicer.SliceFile = sf; // wasn't set
                }
                //create new list for new supports
                List<Object3d> lstsupports = new List<Object3d>(); // final list of supports
                List<SupportLocation> supLocs = new List<SupportLocation>(); // temporary list of support locations.

                int numslices = UVDLPApp.Instance().m_slicer.GetNumberOfSlices(config);
                float zlev = (float)(config.ZThick * 0.5);
                int hxres = config.xres / 2;
                int hyres = config.yres / 2;
                int npix = config.xres * config.yres;
                int[] lbm = new int[npix];  // current slice
                int[] lbmz = new int[npix]; // z buffer
                int[] lbms = new int[npix]; // support map
                int p;
                for (p = 0; p < npix; p++)
                {
                    lbmz[p] = 0;
                    lbms[p] = -1;
                }

                Bitmap bm = new Bitmap(config.xres, config.yres, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // working bitmap
                Color savecol = UVDLPApp.Instance().m_appconfig.m_foregroundcolor;
                m_supportgap = (int)(m_sc.mingap * config.dpmmX);

                m_asp.lbm = lbm;
                m_asp.lbms = lbms;
                m_asp.lbmz = lbmz;
                m_asp.searchmap = new int[npix];
                m_asp.xres = config.xres;
                m_asp.yres = config.yres;
                m_asp.mingap = m_supportgap;
                m_asp.minHeight = (int)(5.0 / config.ZThick); // equvalent of 5mm in layers. should be a parameter?

                for (int c = 0; c < numslices; c++)
                {
                    //bool layerneedssupport = false;
                    if (m_cancel)
                    {
                        RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCancel, "Support Generation Cancelled", null);
                        return;
                    }
                    RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eProgress, "" + c + "/" + numslices, null);

                    Slice sl = UVDLPApp.Instance().m_slicer.GetSliceImmediate(zlev, m_sc.m_onlyselected);
                    zlev += (float)config.ZThick;

                    if ((sl == null) || (sl.m_segments == null) || (sl.m_segments.Count == 0))
                        continue;
                    sl.Optimize();// find loops
                    using (Graphics gfx = Graphics.FromImage(bm))
                        gfx.Clear(Color.Transparent);

                     //render current slice
                    UVDLPApp.Instance().m_appconfig.m_foregroundcolor = Color.FromArgb((0xFF << 24) | c);
                    sl.RenderSlice(config, ref bm);
                    BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    Marshal.Copy(data.Scan0, lbm, 0, lbm.Length);
                    for (p = 0; p < npix; p++)
                        lbm[p] &= 0xFFFFFF;
                    ShrinkSlice(lbm, bm.Width, bm.Height, 1);
                    bm.UnlockBits(data);
                    if (c > 0)
                    {
                        if (m_sc.eSupType == SupportConfig.eAUTOSUPPORTTYPE.eADAPTIVE)
                            ProcessSlice(lbm, lbmz, lbms, config.xres, config.yres, supLocs);
                        else
                            ProcessSlice2(supLocs);
                    }

                    // add slice to zbuffer bitmap
                    for (p = 0; p < npix; p++)
                        if (lbm[p] != 0)
                            lbmz[p] = lbm[p];
                }
                UVDLPApp.Instance().m_appconfig.m_foregroundcolor = savecol;

                int scnt = 0;
                foreach (SupportLocation spl in supLocs)
                {
                    RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eProgress, "" + scnt + "/" + supLocs.Count, null);
                    float px = (float)(spl.x - hxres) / (float)config.dpmmX;
                    float py = (float)(spl.y - hyres) / (float)config.dpmmY;
                    float pztop = ((float)spl.ztop + 0.51f) * (float)config.ZThick;
                    float pzbase = (float)spl.zbottom * (float)config.ZThick;
                    //pz += .650f; // wtf? indeed WTF!! found the bug... 
                    /*Object3d parent = null;
                    ISectData idat = GetIntersection(px, py, pztop);
                    if (idat != null)
                        parent = idat.obj;
                    Support sp = AddNewSupport(px, py, pztop, scnt++, GetSupportParrent(px, py, pztop), lstsupports);*/
                    /*if (spl.zbottom != 0)
                    {
                        sp.PositionBottom(
                    }
                    */
                    AddNewSupport(px, py, pzbase, pztop, scnt++, lstsupports);
                }
                RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCompleted, "Support Generation Completed", lstsupports);
                m_generating = false;

            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
            }
        }
        #endregion


        private void SaveBM(Bitmap bmp, int layer) 
        {
            String fn = UVDLPApp.Instance().SelectedObject.m_fullname;
            string tmp = Path.GetDirectoryName(fn);
            tmp += UVDLPApp.m_pathsep;
            tmp += Path.GetFileNameWithoutExtension(fn);
            tmp += "_" + layer + ".png";
            bmp.Save(tmp);
        }
        public List<Object3d> GenerateSupportObjects()
        {

            // iterate over the platform size by indicated mm step; // projected resolution in x,y
            // generate a 3d x/y point on z=0, 
            // generate another on the z=zmax
            // use this ray to intersect the scene
            // foreach intersection point, generate a support
            // we gott make sure supports don't collide
            // I also have to take into account the 
            // interface between the support and the model
            List<Object3d> lstsupports = new List<Object3d>();


            float ZVal = (float)UVDLPApp.Instance().m_printerinfo.m_PlatZSize;
            m_model.Update();
            float MinX = m_model.m_min.x;
            float MaxX = m_model.m_max.x;
            float MinY = m_model.m_min.y;
            float MaxY = m_model.m_max.y;

           // bool intersected = false;
            int scnt = 0; // support count
            // iterate from -HX to HX step xtep;
            double dts = (MaxX - MinX) / m_sc.xspace;
            int its = (int)dts;
            int curstep = 0;

            for (float x = (float)(MinX + (m_sc.xspace / 2.0f)); x < MaxX; x += (float)m_sc.xspace)
            {
                // say we're doing stuff
                RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eProgress, "" + curstep + "/" + its, null);
                curstep++;
                for (float y = (float)(MinY + (m_sc.yspace / 2)); y < MaxY; y += (float)m_sc.yspace)
                {
                    Point3d origin;                   
                    origin = new Point3d(); // bottom point
                    origin.Set(x, y, 0.0f);
                    //intersected = false; // reset the intersected flag to be false

                    Vector3d up = new Vector3d(); // the up vector
                    up.x = 0.0f;
                    up.y = 0.0f;
                    up.z = 1.0f;

                    List<ISectData> lstISects = RTUtils.IntersectObjects(up, origin, UVDLPApp.Instance().Engine3D.m_objects, false);
                    //check for cancelling
                    if (m_cancel)
                    {
                        RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCancel, "Support Generation Cancelled", null);
                        return lstsupports;
                    }

                    
                    Vector3d upvec = new Vector3d();
                    double inc = 1.0 / 90.0;
                    double angle = -(1 - (m_sc.downwardAngle * inc));
                    upvec.Set(new Point3d(0, 0, 1));

                    foreach (ISectData htd in lstISects)
                    {
                        if (m_sc.m_onlyselected && !htd.obj.m_inSelectedList)
                            continue;

                        if (htd.obj.tag == Object3d.OBJ_NORMAL)  // if this is not another support or the ground
                        {
                            htd.poly.CalcNormal();
                            double d = htd.poly.m_normal.Dot(upvec);
                            if (m_sc.m_onlydownward && d >= angle) // this makes sure downward works even if polygons are not tagged
                                    break; // not a downward facing and we're only doing downward
                            // this should be the closest intersected
                            Support sup = AddNewSupport(x, y, (float)htd.intersect.z, scnt++, htd.obj, lstsupports);
                            sup.SelectionType = Support.eSelType.eTip;
                            sup.MoveFromTip(htd);

                            break; // only need to make one support
                        }
                    }

                }
            }
            // return objects;
            RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eCompleted, "Support Generation Completed", lstsupports);
            m_generating = false;
            return lstsupports;
        }

        
        Support AddNewSupport(float x, float y, float lz, int scnt, Object3d parent, List<Object3d> lstsupports)
        {
            Support s = new Support();

            //s.Create(m_sc ,parent, (float)m_sc.fbrad, (float)m_sc.ftrad, (float)m_sc.hbrad, (float)m_sc.htrad, lz * .2f, lz * .6f, lz * .2f, 11);
            s.Create(m_sc, parent, lz * .2f, lz * .6f, lz * .2f);
            s.Translate(x, y, 0);
            s.Name = "Support " + scnt;
            s.SetColor(Color.Yellow);
            lstsupports.Add(s);
            if (parent != null)
                parent.AddSupport(s);
            
            RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eSupportGenerated, s.Name, s);
            return s;
        }
        

        ISectData GetIntersection(float x, float y, float z)
        {
            Vector3d dir = new Vector3d(0, 0, 1);
            Point3d origin = new Point3d(x, y, 0);
            List<ISectData> lstISects = RTUtils.IntersectObjects(dir, origin, UVDLPApp.Instance().Engine3D.m_objects, false);
            if (lstISects.Count == 0)
                return null;
            ISectData isectFound = null;
            float minzdiff = 99999999f;
            // find the intersection closest to z.
            foreach (ISectData htd in lstISects)
            {
                if (htd.obj.tag != Object3d.OBJ_NORMAL)
                    continue;
                if (m_sc.m_onlyselected && !htd.obj.m_inSelectedList)
                    continue;
                float zdiff = Math.Abs(htd.intersect.z - z);
                if ((zdiff < 1) && (zdiff < minzdiff))
                {
                    minzdiff = zdiff;
                    isectFound = htd;
                }
            }
            return isectFound;
        }

        Support AddNewSupport(float x, float y, float zbase, float ztop, int scnt, List<Object3d> lstsupports)
        {
            ISectData isectTop = GetIntersection(x, y, ztop);
            if (isectTop == null)
                return null;

            if (m_sc.m_onlydownward)
            {
                Vector3d upvec = new Vector3d();
                double inc = 1.0 / 90.0;
                double angle = -(1 - (m_sc.downwardAngle * inc));
                upvec.Set(new Point3d(0, 0, 1));
                isectTop.poly.CalcNormal();
                double d = isectTop.poly.m_normal.Dot(upvec);
                if (m_sc.m_onlydownward && d >= angle) // this makes sure downward works even if polygons are not tagged
                    return null;
            }

            Support s = new Support();
            s.Create(m_sc, isectTop.obj, ztop * .2f, ztop * .6f, ztop * .2f);
            s.Translate(x, y, 0);
            s.SelectionType = Support.eSelType.eTip;
            s.MoveFromTip(isectTop);

            if (zbase > 0)
            {
                ISectData isectBase = GetIntersection(x, y, zbase);
                if (isectBase != null)
                {
                    s.SelectionType = Support.eSelType.eBase;
                    s.SubType = Support.eSubType.eIntra;
                    s.PositionBottom(isectBase);
                }
            }

            s.Name = "Support " + scnt;
            s.SetColor(Color.Yellow);
            lstsupports.Add(s);
            if (isectTop.obj != null)
                isectTop.obj.AddSupport(s);

            RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eSupportGenerated, s.Name, s);
            return s;
        }

    }
}
 