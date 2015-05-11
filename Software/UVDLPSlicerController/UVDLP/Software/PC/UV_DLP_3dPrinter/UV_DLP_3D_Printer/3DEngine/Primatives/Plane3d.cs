using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine3D
{
    /// <summary>
    /// This is a simple class that represents a plan in 3d 
    /// This is used for determining 3d intersections
    /// </summary>
    public class Plane3d
    {
        Vector3d _normal;
        Point3d _pnt;

        public Plane3d(Vector3d normal, Point3d pnt) 
        {
            _normal = new Vector3d(normal.x, normal.y, normal.z);
            _pnt = new Point3d(pnt);
        }
        public void Set(Vector3d normal, Point3d pnt) 
        {
            _normal.Set(normal.x, normal.y, normal.z);
            _pnt.Set(pnt);
        }

        public int Intersect(Point3d startp, Point3d endp, ref Point3d isect) 
        {
            float SMALL_NUM = 0.00000001f;
            Vector3d u = endp - startp;
            Vector3d w = startp - _pnt;
            float D = (float)_normal.Dot(u);
            float N = (float)-_normal.Dot(w);
            if (Math.Abs(D) < SMALL_NUM) 
            {
                if (N == 0)
                {
                    return 2; // segment lies on plane
                }
                else 
                {
                    return 0; // no intersection
                }
            }
            // they are not parallel
            // compute intersect param
            float sI = N / D;
            if (sI < 0 || sI > 1)
                return 0;                        // no intersection

            isect = startp +  sI* u;                  // compute segment intersect point
            return 1;
        
        }
        /*
        // intersect3D_SegmentPlane(): find the 3D intersection of a segment and a plane
        //    Input:  S = a segment, and Pn = a plane = {Point V0;  Vector n;}
        //    Output: *I0 = the intersect point (when it exists)
        //    Return: 0 = disjoint (no intersection)
        //            1 =  intersection in the unique point *I0
        //            2 = the  segment lies in the plane
        int intersect3D_SegmentPlane(Segment S, Plane Pn, Point* I)
        {
            Vector u = S.P1 - S.P0;
            Vector w = S.P0 - Pn.V0;

            float D = dot(Pn.n, u);
            float N = -dot(Pn.n, w);

            if (fabs(D) < SMALL_NUM)
            {           // segment is parallel to plane
                if (N == 0)                      // segment lies in plane
                    return 2;
                else
                    return 0;                    // no intersection
            }
            // they are not parallel
            // compute intersect param
            float sI = N / D;
            if (sI < 0 || sI > 1)
                return 0;                        // no intersection

            *I = S.P0 + sI * u;                  // compute segment intersect point
            return 1;
        }
         * */
    }
}
