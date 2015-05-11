using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    public class csgjs_vertex
    {
        public csgjs_vector pos;
        public csgjs_vector normal;
        public csgjs_vector uv;

        public csgjs_vertex()
        {
            pos = new csgjs_vector();
            normal = new csgjs_vector();
            uv = new csgjs_vector();
        }
        public csgjs_vertex(Point3d pnt) 
        {
            pos = new csgjs_vector((float)pnt.x,(float)pnt.y,(float)pnt.z);
            normal = new csgjs_vector();
            uv = new csgjs_vector();
        }
        public csgjs_vertex clone() 
        {
            csgjs_vertex c = new csgjs_vertex();
            c.pos = pos.clone();
            c.normal = normal.clone();
            c.uv = uv.clone();
            return c;
        }
        // Invert all orientation-specific data (e.g. vertex normal). Called when the
        // orientation of a polygon is flipped.
        public static csgjs_vertex flip(csgjs_vertex v) 
        {
            v.normal = csgjs_vector.negate(v.normal); 
	        return v; 
        }
        // Create a new vertex between this vertex and `other` by linearly
        // interpolating all properties using a parameter of `t`. Subclasses should
        // override this to interpolate additional properties.
        public static csgjs_vertex interpolate(csgjs_vertex  a,  csgjs_vertex b, float t)
        {
	        csgjs_vertex ret = new csgjs_vertex();
            ret.pos = csgjs_vector.lerp(a.pos, b.pos, t);
            ret.normal = csgjs_vector.lerp(a.normal, b.normal, t);
            ret.uv = csgjs_vector.lerp(a.uv, b.uv, t);
	        return ret;
        }
    }
}
