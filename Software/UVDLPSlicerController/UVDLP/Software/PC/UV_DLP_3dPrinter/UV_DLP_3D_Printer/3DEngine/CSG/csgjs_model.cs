using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    public class csgjs_model
    {
        public List<csgjs_vertex> vertices;
        public List<int> indices;
        public csgjs_model()
        {
            vertices = new List<csgjs_vertex>();
            indices = new List<int>();
        }

        public static List<csgjs_polygon> csgjs_modelToPolygons(csgjs_model  model)
        {
	        List<csgjs_polygon> list = new List<csgjs_polygon>();
	        for (int i = 0; i < model.indices.Count; i+= 3)
	        {
		        List<csgjs_vertex> triangle = new List<csgjs_vertex>();
		        for (int j = 0; j < 3; j++)
		        {
			        csgjs_vertex v = model.vertices[model.indices[i + j]];
			        triangle.Add(v);//.push_back(v);
		        }
		        //list.push_back(csgjs_polygon(triangle));
                list.Add(new csgjs_polygon(triangle));
	        }
	        return list;
        }

        public static csgjs_model csgjs_modelFromPolygons(List<csgjs_polygon> polygons)
        {
	        csgjs_model model = new csgjs_model();
	        int p = 0;
	        for (int i = 0; i < polygons.Count; i++)
	        {
		        csgjs_polygon poly = polygons[i];

		        for (int j = 2; j < poly.vertices.Count; j++)
		        {
			        model.vertices.Add(poly.vertices[0]);
                    model.indices.Add(p++);
			        model.vertices.Add(poly.vertices[j - 1]);
                    model.indices.Add(p++);
			        model.vertices.Add(poly.vertices[j]);
                    model.indices.Add(p++);			
		        }
	        }
	        return model;
        }
    }
}
