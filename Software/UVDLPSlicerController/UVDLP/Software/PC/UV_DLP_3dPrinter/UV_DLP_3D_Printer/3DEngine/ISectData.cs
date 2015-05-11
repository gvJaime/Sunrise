using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using System.Collections;

namespace UV_DLP_3D_Printer
{
    public class ISectData : IComparable
    {
        public ISectData(Object3d o, Polygon p, Point3d isect, Point3d orgin, Vector3d dir) 
        {
            intersect = new Point3d();
            intersect.Set(isect);
            origin = new Point3d();
            direction = new Vector3d();
            origin.Set(orgin);
            direction.Set(dir);
            obj = o;
            poly = p;
        }

        public int CompareTo(object c)
        {
            ISectData c1 = this;
            ISectData c2 = (ISectData)c;
            double d1 = (c1.origin.x - c1.intersect.x) * (c1.origin.x - c1.intersect.x) +
                        (c1.origin.y - c1.intersect.y) * (c1.origin.y - c1.intersect.y) +
                        (c1.origin.z - c1.intersect.z) * (c1.origin.z - c1.intersect.z);

            double d2 = (c2.origin.x - c2.intersect.x) * (c2.origin.x - c2.intersect.x) +
                        (c2.origin.y - c2.intersect.y) * (c2.origin.y - c2.intersect.y) +
                        (c2.origin.z - c2.intersect.z) * (c2.origin.z - c2.intersect.z);

            if (d1 > d2)
                return 1;
            if (d1 < d2)
                return -1;
            else
                return 0;
        }
        
        public Point3d origin;
        public Vector3d direction;

        public Object3d obj; // the object that was intersected
        public Polygon poly; // the polygon that was intersected - the surface normal can be obtained here
        public Point3d intersect; // the intersection point
    }
}
