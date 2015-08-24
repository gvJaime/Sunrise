using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UV_DLP_3D_Printer;

namespace Engine3D
{
    public class Point2D
    {
        public double x, y;
        public double key; // for sorting

        public Point2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Matches(double x1, double y1)
        {
            if (x1 >= (x - Path2D.Epsilon) && x1 <= (x + Path2D.Epsilon))
            {
                if (y1 >= (y - Path2D.Epsilon) && y1 <= (y + Path2D.Epsilon))
                {
                    return true;
                }
            }
            return false;
        }

        public int CompareTo(Point2D other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (x < other.x)
                return -1;

            if (x > other.x)
                return 1;

            return y.CompareTo(other.y);
        }
    }
    // this class represent a single sigment of a 2d path. the sigment goes from point p1
    // to point p2 such that the right side is inside the object and the left is outside.
    public class Segment2D
    {
        public Point2D p1;
        public Point2D p2;
        public Point2D pmin;
        public Point2D pmax;
        public bool isValid;

        public Segment2D(double x1, double y1, double x2, double y2)
        {
            p1 = new Point2D(x1, y1);
            p2 = new Point2D(x2, y2);
            CalcMinMax();
        }
 
        public Segment2D(Point2D p1, Point2D p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            CalcMinMax();
        }

        // generate a sigment from single line poly, using the normal to determine direction
        public Segment2D(Point2D p1, Point2D p2, double normx, double normy)
        {
            // the current segment is not longer matching
            // match curve direction based on normal such that the right side is inside the object.
            double curveDir = Math.Atan2(p2.y - p1.y, p2.x - p1.x);
            double normalDir = Math.Atan2(normy, normx);
            double dirDiff = curveDir - normalDir;
            if (dirDiff < 0)
                dirDiff += 2 * Math.PI; // handle negative result
            if ((dirDiff > 0) && (dirDiff < Math.PI))
            {
                // reverse direction to match normal
                this.p1 = p2;
                this.p2 = p1;
           }
            else
            {
                this.p1 = p1;
                this.p2 = p2;
            }
            CalcMinMax();
        }

        void CalcMinMax()
        {
            pmin = new Point2D(Math.Min(p1.x, p2.x) - Path2D.Epsilon, Math.Min(p1.y, p2.y) - Path2D.Epsilon);
            pmax = new Point2D(Math.Max(p1.x, p2.x) + Path2D.Epsilon, Math.Max(p1.y, p2.y) + Path2D.Epsilon);

            isValid = false;
        }

        // split a sigment at a given point, preserving direction
        public List<Segment2D> SplitAt(Point2D pt)
        {
            List<Segment2D> segments = new List<Segment2D>();
            segments.Add(new Segment2D(p1, pt));
            segments.Add(new Segment2D(pt, p2));
            return segments;
        }

        // find intersection between 2 segments. if they do intersect return new segments after 
        // splitting originals on the intersection point.
        public List<Segment2D> Intersect(Segment2D seg, Path2D path)
        {
            // quickly eliminate non adjacent lines
            if ((seg.pmax.y <= pmin.y) || (seg.pmin.y >= pmax.y) || (seg.pmax.x <= pmin.x) || (seg.pmin.x >= pmax.x))
                return null;

            // special case 1: sigments have same endpoints, nothing to do
            if ((Object.ReferenceEquals(p1, seg.p1) && Object.ReferenceEquals(p2, seg.p2)) ||
                (Object.ReferenceEquals(p2, seg.p1) && Object.ReferenceEquals(p1, seg.p2)))
                return null;

            double x21 = p2.x - p1.x;
            double x43 = seg.p2.x - seg.p1.x;
            double y21 = p2.y - p1.y;
            double y43 = seg.p2.y - seg.p1.y;

            double c = x21 * y43 - y21 * x43;

            List<Segment2D> segments;
            // if lines are parrallel, nothing to do
            if (Math.Abs(c) <= Path2D.DEpsilon)
                return null;

            // lines are not parallel. if both lines share any edge, nothing to do
            if (Object.ReferenceEquals(p1, seg.p1) || Object.ReferenceEquals(p2, seg.p2) ||
                Object.ReferenceEquals(p2, seg.p1) || Object.ReferenceEquals(p1, seg.p2))
                return null;

            double x31 = seg.p1.x - p1.x;
            double y31 = seg.p1.y - p1.y;

            double u = (y43 * x31 - x43 * y31) / c;
            double v = (y21 * x31 - x21 * y31) / c;

            // if u or v are outside the 0-1 range, no intersection
            if ((u < -Path2D.Epsilon) || (u > 1 + Path2D.Epsilon) || (v < -Path2D.Epsilon) || (v > 1 + Path2D.Epsilon))
                return null;

            // get intersection point
            Point2D ip = path.GetCreatePoint(p1.x + u * x21, p1.y + u * y21);

            segments = new List<Segment2D>();
            // if ip is not on edge of lines, split them
            if (!Object.ReferenceEquals(p1, ip) && !Object.ReferenceEquals(p2, ip))
                segments.AddRange(SplitAt(ip));
            else
                segments.Add(this);
            if (!Object.ReferenceEquals(seg.p1, ip) && !Object.ReferenceEquals(seg.p2, ip))
                segments.AddRange(seg.SplitAt(ip));
            else
                segments.Add(seg);
            return segments;
        }

    }

    // the following class is used for efficient search for intersections
    // this class holds either the leftmost or the rightsmost point in a segment, and a pointer to the segment
    // for left points, this class holds other data needed for list sorting
    public class SegPoint : IComparable<SegPoint>
    {
        public Point2D pt;       // a point from a segment
        public Segment2D seg;    // the segment this point belongs to
        public int segix;        // the segment position in the source list
        public int segpix;       // position in the segPoint list

        // the following is needed only if pt is the left point of a segment
        //public Point2D pt2;      // the other point
        public SegPoint otherPoint;// the other point of this segment
        public bool isLeft;      // is it the leftmost point of the segments (downmost if segment is vertical)
        public double a,b;       // for y = a * x + b  

        public SegPoint(Segment2D seg, int segix, bool isleft)
        {
            isLeft = isleft;
            if (isleft)
            {
                pt = seg.p1.CompareTo(seg.p2) < 0 ? seg.p1 : seg.p2;
                if (seg.p1.x == seg.p2.x)
                {
                    a = 0;
                    b = pt.y;
                }
                else
                {
                    a = (seg.p2.y - seg.p1.y) / (seg.p2.x - seg.p1.x);
                    b = seg.p2.y - a * seg.p2.x;
                }
            }
            else
                pt = seg.p1.CompareTo(seg.p2) < 0 ? seg.p2 : seg.p1;
            this.seg = seg;
            this.segix = segix;
        }

        public int CompareTo(SegPoint other)
        {
            int res = pt.CompareTo(other.pt);
            if (res != 0) 
                return res;
            if (!isLeft)
                return other.isLeft ? -1 : 0;
            if (!other.isLeft)
                return 1;
            return otherPoint.pt.CompareTo(other.otherPoint.pt);
        }

        public double GetYPos(double x)
        {
            return a * x + b;
        }

        public static bool operator <(SegPoint pt1, SegPoint pt2)
        {
            return pt1.CompareTo(pt2) < 0;
        }

        public static bool operator >(SegPoint pt1, SegPoint pt2)
        {
            return pt1.CompareTo(pt2) > 0;
        }
    }

    public class SegPointYComparer : IComparer<SegPoint>
    {
        public double x;

        public int Compare(SegPoint sp1, SegPoint sp2)
        {
            if (Object.ReferenceEquals(sp1, sp2))
                return 0;
            int res = sp1.GetYPos(x).CompareTo(sp2.GetYPos(x));
            if (res != 0)
                return res;
            return sp1.a.CompareTo(sp2.a);
        }
    }

    public class SegPointList
    {
        public List<SegPoint> items;
        private SegPointYComparer spYComparer = new SegPointYComparer();

        public SegPointList()
        {
            items = new List<SegPoint>();
        }

        // add a point sorted by x position
        public int Add(SegPoint sp)
        {
            int pos = items.BinarySearch(sp);
            if (pos < 0)
            {
                pos = ~pos;
                items.Insert(pos, sp);
                sp.segpix = pos;
            }
            return pos;
        }
 
        /*/ remove a point sorted by x position
        public void Remove(SegPoint sp)
        {
            int pos = items.BinarySearch(sp);
            if (pos >= 0)
                items.RemoveAt(pos);
        }*/

        // add a point sorted by intersection of all existing lines in the list with a vertical line positioned at x 
        public int AddSweep(SegPoint sp, double x)
        {
            spYComparer.x = x;
            int pos = items.BinarySearch(sp, spYComparer);
            if (pos < 0)
            {
                pos = ~pos;
                items.Insert(pos, sp);
            }
            return pos;
        }

        public int AddSweep(SegPoint sp)
        {
            return AddSweep(sp, sp.pt.x);
        }

        // remove a point from sorted sweep line 
        public int RemoveSweep(SegPoint sp, double x)
        {
            spYComparer.x = x;
            int pos = items.BinarySearch(sp, spYComparer);
            if (pos >= 0)
                items.RemoveAt(pos);
            return pos;
        }

        public int RemoveSweep(SegPoint sp)
        {
            return RemoveSweep(sp, sp.pt.x);
        }

        public int Count
        {
            get { return items.Count; }
        }
    }

    // this class represent a single line polygon by list of points. It is ordered from points[0]
    // to points [n-1] such that the right side is inside the object and the left is outside.
    public class Polyline2D
    {
        public List<Point2D> points;
        public bool isClosed;
        public int level; // determine display order when rendering paths
        public int nPoints
        {
            get { return points.Count; }
        }

        public Polyline2D()
        {
            isClosed = false;
            level = 0;
            points = new List<Point2D>();
        }

        public void Add(Point2D p)
        {
            points.Add(p);
        }

        public bool PointInPoly(Point2D pt)
        {
            int i, j, nvert = nPoints;
            bool c = false;

            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (((points[i].y >= pt.y) != (points[j].y >= pt.y)) &&
                    (pt.x <= (points[j].x - points[i].x) * (pt.y - points[i].y) / (points[j].y - points[i].y) + points[i].x)
                  )
                    c = !c;
            }

            return c;
        }
    }

    public class SegmentBin
    {
        double minx, maxx, miny, maxy;
        double hgap, vgap, thgap, tvgap;
        int nbins;
        List<Segment2D>[] hbins;
        List<Segment2D>[] vbins;

        public SegmentBin(double minx, double miny, double maxx, double maxy, int nbins)
        {
            this.minx = minx;
            this.miny = miny;
            this.maxx = maxx;
            this.maxy = maxy;
            this.nbins = nbins;
            hgap = (maxx - minx) / nbins;
            vgap = (maxy - miny) / nbins;
            thgap = hgap / 10.0;
            tvgap = vgap / 10.0;
            hbins = new List<Segment2D>[nbins+1];
            vbins = new List<Segment2D> [nbins+1];
            for (int i = 0; i <= nbins; i++)
            {
                hbins[i] = new List<Segment2D>();
                vbins[i] = new List<Segment2D>();
            }
        }

        public void Add(Segment2D seg)
        {
            // horizontal
            if (Math.Abs(seg.p1.x - seg.p2.x) > Path2D.Epsilon)
            {
                int from = (int)((seg.pmin.x - minx - thgap) / hgap);
                if (from < 0)
                    from = 0;
                int to = (int)((seg.pmax.x - minx + thgap) / hgap);
                for (int i = from; i <= to; i++)
                    hbins[i].Add(seg);
            }

            // vertical
            if (Math.Abs(seg.p1.y - seg.p2.y) > Path2D.Epsilon)
            {
                int from = (int)((seg.pmin.y - miny - tvgap) / vgap);
                if (from < 0)
                    from = 0;
                int  to = (int)((seg.pmax.y - miny + tvgap) / vgap);
                for (int i = from; i <= to; i++)
                    vbins[i].Add(seg);
            }
        }

        public void Add(List<Segment2D> segs)
        {
            foreach (Segment2D seg in segs)
                Add(seg);
        }

        public List<Segment2D> GetHbin(double x)
        {
            int pos = (int)((x - minx) / hgap);
            return hbins[pos];
        }

        public List<Segment2D> GetVbin(double y)
        {
            int pos = (int)((y - miny) / vgap);
            return vbins[pos];
        }
    }

    public class Path2D
    {
        List<Point2D> points;
        List<Segment2D> segments;
        public List<Polyline2D> polys;
        public static double Epsilon = 0.000001;
        public static double DEpsilon = 0.000000001;
        public double minx, miny, maxx, maxy;

        public Point2D GetCreatePoint(double x, double y)
        {
            double key = x * x + y * y;
            double keyfrom = key * 0.99 - 2 * Epsilon;
            double keyto = key * 1.01 + 2 * Epsilon;
            int from = 0;
            int to = points.Count;
            int pos = 0;
            while ((to - from) > 1)
            {
                pos = (to + from) / 2;
                if (points[pos].key < keyfrom)
                    from = pos;
                else
                    to = pos;
            }
            while ((from < points.Count) && (points[from].key < keyto))
            {
                if (points[from].Matches(x, y))
                    return points[from];
                from++;
            }

            while ((pos < points.Count) && (points[pos].key < key))
                pos++;
            
            /*for (int i = 0; i < points.Count; i++ )
                if (points[i].Matches(x, y))
                    return points[i];*/

            Point2D npt = new Point2D(x, y);
            npt.key = key;
            if (pos < points.Count)
                points.Insert(pos, npt);
            else
                points.Add(npt);
            if (minx > x) minx = x;
            if (maxx < x) maxx = x;
            if (miny > y) miny = y;
            if (maxy < y) maxy = y;
            return npt;
        }

        public Path2D(List<PolyLine3d> pls3d)
        {
            try
            {
                points = new List<Point2D>();
                segments = new List<Segment2D>();
                polys = new List<Polyline2D>();
                minx = miny = Double.MaxValue;
                maxx = maxy = Double.MinValue;
                // create dummy points to help with sorting
                GetCreatePoint(0, 0);
                //GetCreatePoint(1000000, 1000000);
                foreach (PolyLine3d p3d in pls3d)
                {
                    Point2D pt1 = GetCreatePoint(p3d.m_points[0].x, p3d.m_points[0].y);
                    Point2D pt2 = GetCreatePoint(p3d.m_points[1].x, p3d.m_points[1].y);
                    if (Object.ReferenceEquals(pt1, pt2)) // invalid segment
                        continue;
                    Segment2D seg = new Segment2D(pt1, pt2, p3d.m_derived.m_normal.x, p3d.m_derived.m_normal.y);
                    segments.Add(seg);
                }
                FindIntersections();
                //FindIntersectionsFast();
                RemoveRedundant();
                ConstructPolyLines();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        // find all intersections and split segments at intersection points
        public void FindIntersections()
        {
            try
            {
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    for (int j = i + 1; j < segments.Count; j++)
                    {
                        List<Segment2D> splits = segments[i].Intersect(segments[j], this);
                        if (splits != null)
                        {
                            // splits must be >= 2
                            segments[i] = splits[0];
                            segments[j] = splits[1];
                            for (int k = 2; k < splits.Count; k++)
                                segments.Add(splits[k]);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        // a (hopefully) faster version of FindIntersections()
        // based on article from: http://geomalgorithms.com/a09-_intersect-3.html
        public void FindIntersectionsFast()
        {
            // 1. add endpoints sorted leftmost to rightmost
            SegPointList segPoints = new SegPointList();
            for (int i = 0; i < segments.Count; i++)
            {
                SegPoint sp1 = new SegPoint(segments[i], i, true);
                SegPoint sp2 = new SegPoint(segments[i], i, false);
                sp1.otherPoint = sp2;
                sp2.otherPoint = sp1;
                segPoints.Add(sp1);
                segPoints.Add(sp2);
            }
            SegPointList sweepPoints = new SegPointList();
            int segix = 0;
            while (segix < segPoints.Count)
            {
                SegPoint sp = segPoints.items[segix];
                if (sp.isLeft)
                {
                    // add to sweep line and test intersections with upper and lower segments
                    int sp_pos = sweepPoints.AddSweep(sp);
                    if (sp_pos > 0)
                        TestIntersections(segPoints, sweepPoints, sp_pos - 1, sp_pos);
                    if (sp_pos < (sweepPoints.Count - 1))
                        TestIntersections(segPoints, sweepPoints, sp_pos, sp_pos + 1);
                }
                else
                {
                    // remove segment from sweep file and test intersection of neighboring segments
                    int sp_pos = sweepPoints.RemoveSweep(sp.otherPoint);
                    if ((sp_pos > 0) && (sp_pos < sweepPoints.Count))
                        TestIntersections(segPoints, sweepPoints, sp_pos - 1, sp_pos);
                }
                segix++;
            }
        }

        private void TestIntersections(SegPointList segPoints, SegPointList sweepPoints, int sp_pos1, int sp_pos2)
        {
            SegPoint sp1 = sweepPoints.items[sp_pos1];
            SegPoint sp2 = sweepPoints.items[sp_pos2];
            List<Segment2D> splits = sp1.seg.Intersect(sp2.seg, this);
            if (splits != null)
            {
                // splits must be >= 2
                foreach (Segment2D seg in splits)
                {
                    SegPoint nspL = new SegPoint(seg, 0, true);
                    SegPoint nspR = new SegPoint(seg, 0, false);
                    nspR.otherPoint = nspL;
                    nspL.otherPoint = nspR;
                    if (Object.ReferenceEquals(nspL.pt, sp1.pt))
                    {
                        // matches begining of sp1, replace sp1 with new segment in all places
                        nspL.segix = sp1.segix;
                        nspL.segpix = sp1.segpix;
                        segPoints.items[nspL.segpix] = nspL;
                        segments[nspL.segix] = seg;
                        sweepPoints.items[sp_pos1] = nspL;
                    }
                    else if (Object.ReferenceEquals(nspL.pt, sp2.pt))
                    {
                        // matches begining of sp2, replace sp2 with new segment in all places
                        nspL.segix = sp2.segix;
                        nspL.segpix = sp2.segpix;
                        segPoints.items[nspL.segpix] = nspL;
                        segments[nspL.segix] = seg;
                        sweepPoints.items[sp_pos2] = nspL;
                    }
                    else
                    {
                        // new segment, add everywhere
                        nspL.segix = segments.Count;
                        segments.Add(seg);
                        segPoints.Add(nspL);
                    }
                    nspR.segix = nspL.segix;
                    if (Object.ReferenceEquals(nspR.pt, sp1.otherPoint.pt))
                    {
                        // matches ending of sp1, replace sp1 endings in all places
                        nspR.segpix = sp1.otherPoint.segpix;
                        segPoints.items[nspR.segpix] = nspR;
                        //sweepPoints.items[sp_pos1] = nspL;
                    }
                    else if (Object.ReferenceEquals(nspR.pt, sp2.otherPoint.pt))
                    {
                        // matches ending of sp2, replace sp2 endings in all places
                        nspR.segpix = sp2.otherPoint.segpix;
                        segPoints.items[nspR.segpix] = nspR;
                        //sweepPoints.items[sp_pos2] = nspL;
                    }
                    else
                    {
                        // new segment point, add to segment points
                        segPoints.Add(nspR);
                    }
                }
            }
        }

        // this function finds intersection of seg with y line
        // if no intersection or intersecton too close to x, do nothing
        // find position of intersection, if it is to the right of x, update rightCount with segment direction
        // otherwise update leftCount with segment direction
        public void UpdateYcrossing(double y, double x, Segment2D seg, ref int leftCount, ref int rightCount)
        {
            int dir = 0;
            if ((y >= seg.p1.y) && (y < seg.p2.y))
                dir = -1;
            else if ((y <= seg.p1.y) && (y > seg.p2.y))
                dir = 1;
            if (dir != 0)
            {
                double u = (y - seg.p1.y) / (seg.p2.y - seg.p1.y);
                double xpos = seg.p1.x + u * (seg.p2.x - seg.p1.x);
                if (Math.Abs(xpos - x) > Path2D.Epsilon)
                {
                    if (xpos > x)
                        rightCount += dir;
                    else
                        leftCount -= dir;
                }
            }
        }

        // same as UpdateYcrossing but in vertical direction
        public void UpdateXcrossing(double x, double y, Segment2D seg, ref int botCount, ref int topCount)
        {
            int dir = 0;
            if ((x >= seg.p1.x) && (x < seg.p2.x))
                dir = 1;
            else if ((x <= seg.p1.x) && (x > seg.p2.x))
                dir = -1;
            if (dir != 0)
            {
                double u = (x - seg.p1.x) / (seg.p2.x - seg.p1.x);
                double ypos = seg.p1.y + u * (seg.p2.y - seg.p1.y);
                if (Math.Abs(ypos - y) > Path2D.Epsilon)
                {
                    if (ypos > y)
                        topCount += dir;
                    else
                        botCount -= dir;
                }
            }
        }

        // segments represent an edge where the right side is inside the object and the left side is
        // outside. when objects intersect some segments fall totaly inside the object. (left and right
        // side is in the object). These segments are now redundant so we remove them.
        public void RemoveRedundant()
        {
            int i, j, n_nonhoriz; 
            List<Segment2D> lineSegs;
            try
            {
                // collect all horizontal segments and push them to the end, they will be processed in the vertical sweep
                n_nonhoriz = segments.Count;
                for (i = 0; i < n_nonhoriz; i++)
                {
                    while ((i < n_nonhoriz) && (Math.Abs(segments[i].p1.y - segments[i].p2.y) <= Path2D.Epsilon))
                    {
                        // possible performance enhancement: switch segments instead of push ???
                        Segment2D seg = segments[i];
                        segments.RemoveAt(i);
                        segments.Add(seg);
                        n_nonhoriz--;
                    }
                }
                SegmentBin sbin = new SegmentBin(minx, miny, maxx, maxy, 100);
                sbin.Add(segments);

                // horizontal sweep
                for (i = 0; i < n_nonhoriz; i++)
                {
                    Segment2D seg = segments[i];
                    double y = seg.p1.y + 0.61263546 * (seg.p2.y - seg.p1.y); // arbitrary point between y1 and y2.
                    double u = (y - seg.p1.y) / (seg.p2.y - seg.p1.y);
                    double xpos = seg.p1.x + u * (seg.p2.x - seg.p1.x);
                    int rightCount = 0;
                    int leftCount = 0;
                    /*for (j = 0; j < n_nonhoriz; j++)
                    {
                        if (j == i)
                            continue;
                        UpdateYcrossing(y, xpos, segments[j], ref leftCount, ref rightCount);
                    }*/
                    lineSegs = sbin.GetVbin(y);
                    foreach (Segment2D seg1 in lineSegs)
                    {
                        if (!Object.ReferenceEquals(seg, seg1))
                            UpdateYcrossing(y, xpos, seg1, ref leftCount, ref rightCount);
                    }
                    // mark valid only the segments that one side count is zero and the other non zero
                    if (((leftCount == 0) && (rightCount != 0)) || ((leftCount != 0) && (rightCount == 0)))
                        seg.isValid = true;
                }

                // vertical sweep
                for (i = n_nonhoriz; i < segments.Count; i++)
                {
                    Segment2D seg = segments[i];
                    double x = seg.p1.x + 0.61263546f * (seg.p2.x - seg.p1.x); // arbitrary point between y1 and y2.
                    double u = (x - seg.p1.x) / (seg.p2.x - seg.p1.x);
                    double ypos = seg.p1.y + u * (seg.p2.y - seg.p1.y);
                    int topCount = 0;
                    int botCount = 0;
                    /*for (j = 0; j < segments.Count; j++)
                    {
                        if ((j == i) || (Math.Abs(segments[j].p1.x - segments[j].p2.x) <= Path2D.Epsilon)) // skip vertical lines
                            continue;
                        UpdateXcrossing(x, ypos, segments[j], ref botCount, ref topCount);
                    }*/
                    lineSegs = sbin.GetHbin(x);
                    foreach (Segment2D seg1 in lineSegs)
                    {
                        if (!Object.ReferenceEquals(seg, seg1))
                            UpdateXcrossing(x, ypos, seg1, ref botCount, ref topCount);
                    }

                    // mark valid only the segments that one side count is zero and the other non zero
                    if (((botCount == 0) && (topCount != 0)) || ((botCount != 0) && (topCount == 0)))
                        seg.isValid = true;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void ConstructPolyLines()
        {
            try
            {
                while (segments.Count != 0)
                {
                    if (!segments[0].isValid)
                    {
                        segments.RemoveAt(0);
                        continue;
                    }
                    Polyline2D ply = new Polyline2D();
                    ply.Add(segments[0].p1);
                    Point2D curpt = segments[0].p2;
                    ply.Add(curpt);
                    polys.Add(ply);
                    segments.RemoveAt(0);
                    bool segmentFound = true;
                    while (segmentFound)
                    {
                        int i = 0;
                        segmentFound = false;
                        while (i < segments.Count)
                        {
                            Segment2D seg = segments[i];

                            if (seg.isValid && Object.ReferenceEquals(seg.p1, curpt))
                            {
                                curpt = seg.p2;
                                if (Object.ReferenceEquals(curpt, ply.points[0]))
                                {
                                    ply.isClosed = true;
                                    segments.RemoveAt(i);
                                    segmentFound = false;
                                    break;
                                }
                                ply.Add(curpt);
                                segments.RemoveAt(i);
                                segmentFound = true;
                            }
                            else if (seg.isValid && Object.ReferenceEquals(seg.p2, ply.points[0]))
                            {
                                ply.points.Insert(0, seg.p1);
                                segments.RemoveAt(i);
                                segmentFound = true;
                            }
                            else
                                i++;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public StreamWriter GenerateSVG(double width, double height, bool isFillPoly)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            //double width = UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            //double height = UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
            sw.WriteLine("<!-- Created with CreationWorkshop (http://www.envisionlabs.net/) -->");
            sw.WriteLine();
            sw.WriteLine("<svg width=\"{0}mm\" height=\"{1}mm\" viewBox=\"{2} {3} {0} {1}\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">", width, height, -width / 2, -height / 2);
            if (isFillPoly)
            {
                // sort polygons into display layers
                //int[] dispLevel = new int[lstPoly.Count];
                int i, j, k;
                int maxLevel = 0;
                for (i = 0; i < polys.Count; i++)
                {
                    if (!polys[i].isClosed)
                        continue;
                    polys[i].level = 0;
                    for (j = 0; j < polys.Count; j++)
                    {
                        if ((j == i) || !polys[j].isClosed)
                            continue;
                        if (polys[j].PointInPoly(polys[i].points[0]))
                            polys[i].level++;
                    }
                    if (polys[i].level > maxLevel)
                        maxLevel = polys[i].level;
                }

                // draw polygons layer by layer
                for (k = 0; k <= maxLevel; k++)
                {
                    for (j = 0; j < polys.Count; j++)
                    {
                        if (polys[j].level != k)
                            continue;
                        Polyline2D pl = polys[j];
                        int plen = pl.nPoints;

                        // determine polygon direction
                        double dir = 0;
                        for (i = 1; i < plen; i++)
                        {
                            dir += (pl.points[i].x - pl.points[i - 1].x) * (pl.points[i].y + pl.points[i - 1].y);
                        }
                        dir += (pl.points[0].x - pl.points[plen - 1].x) * (pl.points[0].y + pl.points[plen - 1].y);

                        // draw polygon
                        sw.Write("<polygon points=\"");
                        for (i = 0; i < plen; i++)
                        {
                            sw.Write("{0},{1}", pl.points[i].x, -pl.points[i].y);
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
                foreach (Polyline2D pl in polys)
                {
                    int plen = pl.nPoints;
                    for (int i = 0; i < plen; i++)
                    {
                        if (i == 0)
                            sw.Write("M{0} {1} ", pl.points[i].x, -pl.points[i].y);
                        else
                            sw.Write("L{0} {1} ", pl.points[i].x, -pl.points[i].y);
                    }
                    sw.WriteLine("Z ");
                }
                sw.WriteLine("\" />");
            }

            //<path d="M 15 2 L9.5 18.0 L25.5 22.0 Z M 15.0 0 L7.5 20.0 L22.5 20.0 Z" fill-rule="evenodd"/>
            sw.WriteLine("</svg>");
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Begin);

            return sw;
        }

    }
}
