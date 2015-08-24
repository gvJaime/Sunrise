using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    public class csgjs_polygon
    {
	    public List<csgjs_vertex> vertices;
	    public csgjs_plane plane;


        public csgjs_polygon clone() 
        {
            csgjs_polygon c = new csgjs_polygon();
            foreach (csgjs_vertex v in vertices) 
            {
                c.vertices.Add(v.clone());
            }
            c.plane = plane.clone();
            return c;
        }
        public void flip()
        {
            vertices.Reverse();
	        for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].normal = csgjs_vector.negate(vertices[i].normal);
            }
	        plane.flip();
        }

        public csgjs_polygon()
        {
            vertices = new List<csgjs_vertex>();
            plane = new csgjs_plane();
        }

        public csgjs_polygon( List<csgjs_vertex>list) 
        {
            vertices = new List<csgjs_vertex>(list);
            plane = new csgjs_plane(vertices[0].pos, vertices[1].pos, vertices[2].pos);
        }
    };
}
