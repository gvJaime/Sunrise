using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    public class csgjs_plane
    {
	    public  const int COPLANAR = 0;
	    public  const int FRONT = 1;
	    public  const int BACK = 2;
        public  const int SPANNING = 3;

	    public csgjs_vector normal;
	    public float w;

        public csgjs_plane clone() 
        {
            csgjs_plane c = new csgjs_plane();
            c.normal = normal.clone();
            c.w = w;
            return c;
        }

        public csgjs_plane() 
        {
            normal = new csgjs_vector();
            w = 0.0f;
        }

        public csgjs_plane( csgjs_vector  a,  csgjs_vector  b,  csgjs_vector  c)
        {
	        normal = csgjs_vector.unit(csgjs_vector.cross(b - a, c - a));
	        w = csgjs_vector.dot(normal, a);
        }

        public bool ok()  
        {
	        return csgjs_vector.length(normal) > 0.0f; 
        }

        public void flip()
        {
            normal = csgjs_vector.negate(normal); 
            w *= -1.0f;
        }

        // Split `polygon` by this plane if needed, then put the polygon or polygon
        // fragments in the appropriate lists. Coplanar polygons go into either
        // `coplanarFront` or `coplanarBack` depending on their orientation with
        // respect to this plane. Polygons in front or in back of this plane go into
        // either `front` or `back`.
        public void splitPolygon(csgjs_polygon  polygon, List<csgjs_polygon> coplanarFront, List<csgjs_polygon> coplanarBack,ref List<csgjs_polygon> front,ref List<csgjs_polygon>  back) 
        {
	        // Classify each point as well as the entire polygon into one of the above
	        // four classes.
	        int polygonType = 0;
            List<int> types = new List<int>();

	        for (int i = 0; i < polygon.vertices.Count; i++) 
	        {
                float t = csgjs_vector.dot(normal, polygon.vertices[i].pos) - w;
                int type = (t < -CSG.csgjs_EPSILON) ? BACK : ((t > CSG.csgjs_EPSILON) ? FRONT : COPLANAR);
		        polygonType |= type;
		        types.Add(type);
	        }

	        // Put the polygon in the correct list, splitting it when necessary.
	        switch (polygonType) 
	        {
	        case COPLANAR:
		        {
                    if (csgjs_vector.dot(normal, polygon.plane.normal) > 0)
				        coplanarFront.Add(polygon);
			        else 
				        coplanarBack.Add(polygon);
			        break;
		        }
	        case FRONT:
		        {
			        front.Add(polygon);
			        break;
		        }
	        case BACK:
		        {
			        back.Add(polygon);
			        break;
		        }
	        case SPANNING:
		        {
			        List<csgjs_vertex> f = new List<csgjs_vertex>();
                    List<csgjs_vertex> b = new List<csgjs_vertex>();
			        for (int i = 0; i < polygon.vertices.Count; i++) 
			        {
				        int j = (i + 1) % polygon.vertices.Count;
				        int ti = types[i], tj = types[j];
				        csgjs_vertex vi = polygon.vertices[i], vj = polygon.vertices[j];
				        if (ti != BACK) 
                            f.Add(vi);
				        if (ti != FRONT) 
                            b.Add(vi);
				        if ((ti | tj) == SPANNING) 
				        {
                            float t = (w - csgjs_vector.dot(normal, vi.pos)) / csgjs_vector.dot(normal, vj.pos - vi.pos);
                            csgjs_vertex v = csgjs_vertex.interpolate(vi, vj, t);
					        f.Add(v);
					        b.Add(v);
				        }
			        }
			        if (f.Count >= 3) 
                        front.Add(new csgjs_polygon(f));

			        if (b.Count >= 3) 
                        back.Add(new csgjs_polygon(b));
			        break;
		        }
	        }
        }
    }
}
