
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Engine3D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;


namespace UV_DLP_3D_Printer
{
    public class Point2d : IComparable
    {
        public int x, y;
        public PolyLine3d m_parent;
        public bool m_backfacing;

        int IComparable.CompareTo(object obj)
        {
            Point2d p = (Point2d)obj;
            if (p.x > x) 
            {
                return -1;
            }
            else if (p.x < x)
            {
                return 1;
            }
            else 
            {
                return 0;
            }
        }

    }
    public class Line2d 
    {
        public Point2d p1, p2;
        public PolyLine3d m_parent;
        public Line2d() 
        {
            p1 = new Point2d();
            p2 = new Point2d();
        }
        public void SetParent(PolyLine3d parent) 
        {
            m_parent = parent;
            p1.m_parent = parent;
            p2.m_parent = parent;
        }
        public Point2d IntersectY(int ypos) 
        {
            Point2d pnt = new Point2d();
            double minx, maxx;
            double miny, maxy;
            double yp = ypos;
            minx = (double)Math.Min(p1.x, p2.x);
            maxx = (double)Math.Max(p1.x, p2.x);
            miny = (double)Math.Min(p1.y, p2.y);
            maxy = (double)Math.Max(p1.y, p2.y);
            double yrange = maxy - miny;// the range of the x coord
            double scale = (double)((yp - miny) / yrange);
            //pnt.x = (int)LERP(minx, maxx, scale);
            Point2d pmin, pmax;
            if (p1.y < p2.y) // find the point with the min y
            {
                pmin = p1;
                pmax = p2;
            }
            else
            {
                pmin = p2;
                pmax = p1;

            }
            //pnt.x = (int)LERP(p2.x, p1.x, scale); 
            pnt.x = (int)LERP(pmin.x, pmax.x, scale); 
            pnt.y = ypos;
            return pnt;
     
        }
        private static double LERP(double a, double b, double c) { return (double)(((b) - (a)) * (c) + (a)); }
    }
    public class MinMax_XY 
    {
        public int xmin, xmax, ymin, ymax; 
    }
    /*
     The idea in this class is to provide a single slice of a model
     * This is represented 
     */

    public class Slice 
    {
        public List<PolyLine3d> m_segments; // list of polyline segments
        public List<PolyLine3d> m_opsegs; // list of optimized polyline segments
        
        public Slice()
        {
            m_segments = new List<PolyLine3d>();
            m_opsegs = null;
        }
        public bool Load(StreamReader sr) 
        {
            try
            {
                int numplys = int.Parse(sr.ReadLine());
                for (int c = 0; c < numplys; c++) 
                {
                    PolyLine3d pl = new PolyLine3d();
                    pl.Load(sr);
                    m_segments.Add(pl);
                }
                return true;
            }
            catch (Exception ) 
            {
                return false;
            }

        }
        public bool Save(StreamWriter sw) 
        {
            try
            {
                //save number of points
                sw.WriteLine(m_segments.Count);
                foreach (PolyLine3d pl in m_segments) 
                {
                    pl.Save(sw);
                }
                return true;
            }
            catch (Exception ) 
            {
                return false;
            }
        }

        /// <summary>
        /// This function trys to join together the short line segments into 
        /// a set of longer 3d polyines
        /// The goal here is to determine the interior and exterior lines
        /// This is going to help us do several things:
        /// 1) import/export SVG/CLI files
        /// 2) determine overlapping boundaries for better self-intersecting models
        /// 3) determine overhangs - by detemining if polylines from other layers intersect vertically or are encapsulated.
        /// 
        /// The first pass of this algorithm may have to be N^2 search, I'll try
        /// to change it to log(N) or N as I go along
        /// </summary>
        /// 

        public void Optimize()
        {
            try
            {
                // copy all the polylines in segments into a list
                List<PolyLine3d> allseg = new List<PolyLine3d>();
                //create the final optimize segment list
                m_opsegs = new List<PolyLine3d>();

                // copy the list and clone the polyline segments 
                foreach (PolyLine3d pl in m_segments)
                {
                    if (!pl.m_points[0].Matches(pl.m_points[pl.m_points.Count - 1])) // discard sements with same beginning and end
                        allseg.Add(new PolyLine3d(pl)); 
                }
                // gotta keep track of lines to remove
                List<PolyLine3d> removelist = new List<PolyLine3d>();
                bool done = false;
                // matchcount is a counter that tracks how many endpoints we've matched this trip around
                int matchcount = 0;
                while (!done)
                {
                    // set the current line to be the first polyline segement
                    PolyLine3d curline = allseg[0];
                    //iterate through all the line segments in allsegs
                    for (int cnt = 0; cnt < allseg.Count; cnt++)
                    {
                        PolyLine3d pl = allseg[cnt];
                        if (cnt != 0) // the first polyline is the one we're always trying to match
                        {
                            // if the last point in the current polyline matches the first point
                            // in the line we're testing, add the second point of the line we're testing 
                            // to the end of the current line
                            if (curline.m_points[curline.m_points.Count - 1].Matches(pl.m_points[0])) // case 2
                            {
                                //if ((curline.m_derived.SharesEdge(pl.m_derived)))
                                {
                                    for (int i = 1; i < pl.m_points.Count; i++)
                                        curline.m_points.Add(pl.m_points[i]);
                                    removelist.Add(pl); // add the test line to the list of lines to remove, now that we've used it
                                    matchcount++;
                                }
                            }
                            else if (curline.m_points[curline.m_points.Count - 1].Matches(pl.m_points[pl.m_points.Count - 1])) // case 4 last point matches last
                            {
                                // && (curline.m_derived.SharesEdge(pl.m_derived))
                                pl.m_points.Reverse();
                                for (int i = 1; i < pl.m_points.Count; i++)
                                    curline.m_points.Add(pl.m_points[i]);
                                removelist.Add(pl);
                                matchcount++;
                            }
                        }
                    }
                    // now remove all the matched segments from all segment list
                    foreach (PolyLine3d seg in removelist)
                    {
                        allseg.Remove(seg);
                    }
                    removelist.Clear();

                    if (matchcount > 0)
                    {
                        matchcount = 0; // reset the match counter
                    }
                    else
                    {
                        // the current segment is not longer matching
                        // match curve direction based on normal such that the right side is inside the object.
                        double curveDir = Math.Atan2(curline.m_points[1].y - curline.m_points[0].y, curline.m_points[1].x - curline.m_points[0].x);
                        double normalDir = Math.Atan2(curline.m_derived.m_normal.y, curline.m_derived.m_normal.x);
                        double dirDiff = curveDir - normalDir;
                        if (dirDiff < 0)
                            dirDiff += 2 * Math.PI; // handle negative result
                        if ((dirDiff > 0) && (dirDiff < Math.PI))
                            curline.m_points.Reverse(); // reverse curve direction to match normal

                        // add it to the final list of optimized segments
                        m_opsegs.Add(curline);
                        //and remove it from the list of all segments
                        allseg.Remove(curline);
                        //check for end condition
                        if (allseg.Count == 0)
                            done = true;
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            RemoveDoublePoints(); // make really clean
        }
        private void RemoveDoublePoints() 
        {
            try
            {
                List<Point3d> lstRpnts = new List<Point3d>(); // points to remove
                foreach (PolyLine3d pl in m_opsegs)
                {
                    for (int c = 0; c < pl.m_points.Count - 1; c++)
                    {
                        if (pl.m_points[c].Matches(pl.m_points[c + 1]))
                        {
                            lstRpnts.Add(pl.m_points[c + 1]);

                        }
                    }
                    foreach (Point3d pnt in lstRpnts)
                        pl.m_points.Remove(pnt);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        /*
         This function calculates the min and max x/y coordinates of this slice
         */
        public static MinMax_XY CalcMinMax_XY(List<Line2d> lines2d) 
        {
            Line2d l1 = (Line2d)lines2d[0];
            MinMax_XY mm = new MinMax_XY();
            //start the min / max off with some valid values
            mm.xmin = mm.xmax = l1.p1.x;
            mm.ymin = mm.ymax = l1.p1.y;

            foreach (Line2d ln in lines2d) 
            {
                if (ln.p1.x < mm.xmin) mm.xmin = ln.p1.x;
                if (ln.p2.x < mm.xmin) mm.xmin = ln.p2.x;
                if (ln.p1.x > mm.xmax) mm.xmax = ln.p1.x;
                if (ln.p2.x > mm.xmax) mm.xmax = ln.p2.x;

                if (ln.p1.y < mm.ymin) mm.ymin = ln.p1.y;
                if (ln.p2.y < mm.ymin) mm.ymin = ln.p2.y;
                if (ln.p1.y > mm.ymax) mm.ymax = ln.p1.y;
                if (ln.p2.y > mm.ymax) mm.ymax = ln.p2.y;            
            }
            return mm;
        }
        private void RenderOutlines(Graphics g, SliceBuildConfig sp) 
        {
            //optimize the polylines to join short segements into longer segments in correct winding order
            Optimize();
            Pen pen_inset = null;
            Pen pen_outset = null;
            //create the appropriate pen
            pen_inset = new Pen(UVDLPApp.Instance().m_appconfig.m_foregroundcolor, (float)sp.m_outlinewidth_inset);
            pen_inset.Alignment = PenAlignment.Inset;
            pen_outset = new Pen(UVDLPApp.Instance().m_appconfig.m_foregroundcolor, (float)sp.m_outlinewidth_outset);
            pen_outset.Alignment = PenAlignment.Outset;
            //outset lines by definition will not meet at the corners, try to add some rounding to account for this.
            pen_outset.EndCap = LineCap.Round;
            pen_outset.StartCap = LineCap.Round;

            //get the half resolution for offsetting
            int hxres = sp.xres / 2;
            int hyres = sp.yres / 2;
            // iterate through all the polylines in m_opsegs
            //m_opsegs now contains 0 or more 3d polylines
            foreach(PolyLine3d pln in m_opsegs)
            {
                Point[] pntlst = new Point[pln.m_points.Count];
                //iterate through all the 3d points in this polyline segment
                for (int c = 0; c < pln.m_points.Count ; c++)
                {
                    // we must project these to 2d similar to Get2dLines
                    //3d to 2d conversion
                    Point3d p3d1 = (Point3d)pln.m_points[c];
                    Point pnt2d = new Point();
                    pnt2d.X = (int)(p3d1.x * sp.dpmmX);
                    pnt2d.Y = (int)(p3d1.y * sp.dpmmY);
                    // 2d offseting
                    pnt2d.X = (int)(pnt2d.X) + sp.XOffset + hxres;
                    pnt2d.Y = (int)(pnt2d.Y) + sp.YOffset + hyres;
                    pntlst[c] = pnt2d; // set the item in the array                    
                }
                //now render it
                g.DrawPolygon(pen_inset, pntlst);
                if (sp.m_outlinewidth_outset > 0)
                {
                    g.DrawPolygon(pen_outset, pntlst);
                }
            }
            // dispose of the pen
            pen_inset.Dispose();
            pen_outset.Dispose();
        }
        private void Render2dlines(Graphics g, List<Line2d> lines, SliceBuildConfig sp) 
        {
            try
            {
                Point pnt1 = new Point(); // create some points for drawing
                Point pnt2 = new Point();

                Pen pen;//= new Pen(UVDLPApp.Instance().m_appconfig.m_foregroundcolor, 1);                
                pen = new Pen(UVDLPApp.Instance().m_appconfig.m_foregroundcolor, 1);
                

                int hxres = sp.xres / 2;
                int hyres = sp.yres / 2;

                foreach (Line2d ln in lines)
                {
                    Point2d p1 = (Point2d)ln.p1;
                    Point2d p2 = (Point2d)ln.p2;
                    pnt1.X = (int)(p1.x) + sp.XOffset + hxres;
                    pnt1.Y = (int)(p1.y) + sp.YOffset + hyres;
                    pnt2.X = (int)(p2.x) + sp.XOffset + hxres;
                    pnt2.Y = (int)(p2.y) + sp.YOffset + hyres;
                    //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(pen, pnt1, pnt2);
                }
                pen.Dispose();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);

            }
        }
        //alright, it looks like I'm going to have to code up a 2d scanline fill algorithm
        // I suppose the first step is to convert all the 3d-ish polyline points in this slice into
        // 2d polylines, then use those to implement the fill
        /// <summary>
        /// This new slicing function renders into a pre-allocated bitmap
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="bmp"></param>
        public void RenderSlice(SliceBuildConfig sp, ref Bitmap bmp, bool outlineonly = false)
        {
            try
            {
                Graphics graph = Graphics.FromImage(bmp);
                Point pnt1 = new Point(); // create some points for drawing
                Point pnt2 = new Point();
                // in the future, the forecolor and backcolor should be pulled from the slice profile, not the app config
                Pen pen = new Pen(UVDLPApp.Instance().m_appconfig.m_foregroundcolor, 1);
                //convert all to 2d lines
                int hxres = sp.xres / 2;
                int hyres = sp.yres / 2;

                List<Line2d> lines2d = Get2dLines(sp, m_segments);
                if (lines2d.Count == 0) 
                    return;

                if (sp.m_createoutlines && outlineonly)
                {
                    RenderOutlines(graph, sp);
                }
                else
                {
                    // this renders the outline of the object for a filled slice
                    Render2dlines(graph, lines2d, sp);
                }
                // find the x/y min/max
                MinMax_XY mm = CalcMinMax_XY(lines2d);
                if (outlineonly)
                {
                    pen.Dispose();
                    return;
                }
                // iterate from the ymin to the ymax
                for (int y = mm.ymin; y < mm.ymax; y++) // this needs to be in scaled value 
                {
                    //      get a line of lines that intersect this 2d line
                    List<Line2d> intersecting = GetIntersecting2dYLines(y, lines2d);
                    //      get the list of point intersections
                    List<Point2d> points = GetIntersectingPoints(y, intersecting);
                    // sort the points in increasing x order
                    points.Sort();
                    SortBackfaces(points);

                    if (UVDLPApp.Instance().m_slicer.m_slicemethod == Slicer.eSliceMethod.eEvenOdd)
                    {
                        //      draw the X-Spans (should be even number)    
                        //    For a given pair of intersectin points
                        //    (Xi, Y), (Xj, Y)
                        //  −> Fill ceiling(Xi) to floor(Xj)

                        if (points.Count % 2 == 0)  // is even
                        {
                            for (int cnt = 0; cnt < points.Count; cnt += 2)  // increment by 2
                            {
                                Point2d p1 = (Point2d)points[cnt];
                                Point2d p2 = (Point2d)points[cnt + 1];
                                pnt1.X = (int)(p1.x + sp.XOffset + hxres);
                                pnt1.Y = (int)(p1.y + sp.YOffset + hyres);
                                pnt2.X = (int)(p2.x + sp.XOffset + hxres);
                                pnt2.Y = (int)(p2.y + sp.YOffset + hyres);
                                //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                graph.DrawLine(pen, pnt1.X, pnt1.Y, pnt2.X, pnt2.Y);
                            }
                        }
                        else  // flag error
                        {
                            DebugLogger.Instance().LogRecord("Row y=" + y + " odd # of points = " + points.Count + " - Model may have holes");
                        }
                    }

                    else if (UVDLPApp.Instance().m_slicer.m_slicemethod == Slicer.eSliceMethod.eNormalCount)
                    {
                        Point2d p1 = null;
                        Point2d p2 = null;
                        int turncount = 0;
                        if (points.Count != 0)
                        {
                            for (int cnt = 0; cnt < points.Count; cnt++)
                            {
                                if (points[cnt].m_backfacing)
                                {
                                    if (turncount == 0) //p1 == null)
                                        p1 = (Point2d)points[cnt];
                                    turncount++;
                                }
                                else
                                {
                                    //if (turncount > 0)
                                        turncount--;
                                    if ((turncount == 0) && (p1 != null))
                                    {
                                        p2 = (Point2d)points[cnt];
                                        pnt1.X = (int)(p1.x + sp.XOffset + hxres);
                                        pnt1.Y = (int)(p1.y + sp.YOffset + hyres);
                                        pnt2.X = (int)(p2.x + sp.XOffset + hxres);
                                        pnt2.Y = (int)(p2.y + sp.YOffset + hyres);
                                        //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                        graph.DrawLine(pen, pnt1.X, pnt1.Y, pnt2.X, pnt2.Y);
                                        p1 = p2;
                                        p2 = null;
                                    }
                                }
                            }
                            if (turncount != 0)// flag error
                            {
                                DebugLogger.Instance().LogRecord("Row y=" + y + " non equal in/outs = " + turncount + " - Model may have holes");
                            }
                        }
                    }
                }
                //clean up
                pen.Dispose();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void SortBackfaces(List<Point2d> points)
        {
            bool lastBackface = false;
            int cnt = 0;
            while (cnt < points.Count)
            {
                int cnt1 = cnt + 1;
                while ((cnt1 < points.Count) && (points[cnt1].x == points[cnt].x))
                    cnt1++;
                cnt1--;
                if (cnt1 > cnt)
                {
                    for (int i = cnt; i < cnt1; i++)
                    {
                        if (points[i].m_backfacing == lastBackface)
                        {
                            for (int k = i + 1; k <= cnt1; k++)
                            {
                                if (points[k].m_backfacing != lastBackface)
                                {
                                    Point2d pt = points[k];
                                    points[k] = points[i];
                                    points[i] = pt;
                                }
                            }
                        }
                        lastBackface = points[i].m_backfacing;
                    }
                }
                lastBackface = points[cnt1].m_backfacing;
                cnt = cnt1 + 1;
            }
        }

        // this function will return a list of 2d point intersections on the specified y line
        public static List<Point2d> GetIntersectingPoints(int ypos, List<Line2d> lines) 
        {
            List<Point2d> points = new List<Point2d>();
            //if the ypos intersects with an endpoint, add it twice, because it must be used twice to make an even number
            foreach (Line2d ln in lines) 
            {
                int ymin = Math.Min(ln.p1.y, ln.p2.y);
                int ymax = Math.Max(ln.p1.y, ln.p2.y);

                if (ln.p1.y == ypos && ln.p2.y == ypos)  
                {
                    if (ln.p1.x < ln.p1.y)
                        ln.p1.m_backfacing = true;
                    else 
                        ln.p1.m_backfacing = false;
                    ln.p2.m_backfacing = !ln.p1.m_backfacing;

                    points.Add(ln.p1);
                    points.Add(ln.p2);
                }                    
                else if (ln.p1.y == ypos) // point 1 endpoint is on the line,
                {
                    //If the intersection is the ymin of the edge’s
                    //endpoint, count it. Otherwise, don’t
                    if (ln.p1.y == ymin) // if the 
                    {
                        ln.p1.m_backfacing = (ln.m_parent.m_derived.m_normal.x < 0);
                        points.Add(ln.p1);
                    }
                    
                }
                else if (ln.p2.y == ypos) // point 2 endpoint is on the line,
                {
                    //If the intersection is the ymin of the edge’s
                    //endpoint, count it. Otherwise, don’t

                    if (ln.p2.y == ymin)
                    {
                        ln.p2.m_backfacing = (ln.m_parent.m_derived.m_normal.x < 0);
                        points.Add(ln.p2);
                    }
                }                     
                else // intersect  
                { 
                 
                    Point2d isect = ln.IntersectY(ypos); // singled point of intersection
                    isect.m_parent = ln.m_parent;
                    isect.m_backfacing = (ln.m_parent.m_derived.m_normal.x < 0);
                    points.Add(isect);
                }
            }
            return points;
        }
        
        /// <summary>
        /// this function converts all the 3d polyines to 2d lines so we can process everything 
        /// This is the equivanlant of a 3d->2d projection function
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private List<Line2d> Get2dLines(SliceBuildConfig sp, List<PolyLine3d> segments) 
        {
            List<Line2d> lst = new List<Line2d>();
            foreach (PolyLine3d ply in segments)  
            {
                for (int c = 0; c < ply.m_points.Count - 1; c++ )
                {
                    Line2d ln = new Line2d();
                    ln.SetParent(ply);
                    //get the 3d points of the line
                    Point3d p3d1 = (Point3d)ply.m_points[c];
                    Point3d p3d2 = (Point3d)ply.m_points[c + 1];
                    //convert them to 2d (probably should add an offset to center them)
                    ln.p1.x = (int)(p3d1.x * sp.dpmmX);
                    ln.p1.y = (int)(p3d1.y * sp.dpmmY);
                    ln.p2.x = (int)(p3d2.x * sp.dpmmX);
                    ln.p2.y = (int)(p3d2.y * sp.dpmmY);
                    lst.Add(ln);
                }
            }
            return lst; // return the list
        }

        private List<Line2d> Get2dLines_Save(SliceBuildConfig sp, List<PolyLine3d> segments)
        {
            List<Line2d> lst = new List<Line2d>();
            // this can be changed at some point to assume that the 3d polyline has more than 2 points
            // I'll need to do this when I want to properly generate inside / outside countours
            foreach (PolyLine3d ply in segments)
            {
                Line2d ln = new Line2d();
                ln.SetParent(ply);
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

        /*
         This function will return a list of lines that intersect with the specified Y scanline
         */
        public static List<Line2d> GetIntersecting2dYLines(int ypos, List<Line2d> all2dlines) 
        {
            List<Line2d> intersecting = new List<Line2d>();
            foreach (Line2d ln in all2dlines) 
            {
                if ((ln.p1.y >= ypos && ln.p2.y <= ypos) || 
                    (ln.p2.y >= ypos && ln.p1.y <= ypos) ||
                    (ln.p2.y == ypos && ln.p1.y == ypos)) 
                {
                    intersecting.Add(ln);
                }
            }
            return intersecting;
        }
    }
}
