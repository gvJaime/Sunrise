using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
// Holds a node in a BSP tree. A BSP tree is built from a collection of polygons
// by picking a polygon to split along. That polygon (and all other coplanar
// polygons) are added directly to that node and the other polygons are added to
// the front and/or back subtrees. This is not a leafy BSP tree since there is
// no distinction between internal and leaf nodes.
    public class csgjs_csgnode
    {
	    public List<csgjs_polygon> polygons;
	    public csgjs_csgnode front;
	    public csgjs_csgnode back;
	    public csgjs_plane plane;

        public csgjs_csgnode()
        {
            front = null;
            back = null;
            plane = new csgjs_plane();
            polygons = new List<csgjs_polygon>();
        }

        public csgjs_csgnode( List<csgjs_polygon>  list)
        {
            front = null;
            back = null;
            plane = new csgjs_plane();
            polygons = new List<csgjs_polygon>();
	        build(list);
        }
        public csgjs_csgnode clone() 
        {
	        csgjs_csgnode ret = new csgjs_csgnode();
            foreach (csgjs_polygon p in polygons) 
            {
                ret.polygons.Add(p.clone());
            }
	        ret.plane = plane.clone();

	        if (front != null)
                ret.front = front.clone();

	        if (back!= null) 
                ret.back = back.clone();

	        return ret;
        }
        // Remove all polygons in this BSP tree that are inside the other BSP tree
        // `bsp`.
        void clipTo(csgjs_csgnode  other)
        {
	        polygons = other.clipPolygons(polygons);
            if (front != null)
            {
                front.clipTo(other);
            }
            if (back != null)
            {
                back.clipTo(other);
            }
        }
        // Convert solid space to empty space and empty space to solid space.
        void invert()
        {	
	        for (int i = 0; i < polygons.Count; i++)
            {
		        polygons[i].flip();
            }
	        plane.flip();
	        if (front !=null) 
            {
                front.invert();
            }
	        if (back != null) 
            {
                back.invert();
            }

	        //std::swap(front, back);
            //hmm - need to swap front and back...

            csgjs_csgnode f = null;
            csgjs_csgnode b = null;
            if(front != null)
            {
                f = front.clone();
            }
            if(back !=null)
            {
                b = back.clone(); 
            }
            back = f;
            front = b;
        }

        // Recursively remove all polygons in `polygons` that are inside this BSP
        // tree.
        List<csgjs_polygon> clipPolygons( List<csgjs_polygon>  list) 
        {
            if (!plane.ok())
            {
                return list;
            }
            List<csgjs_polygon> list_front = new List<csgjs_polygon>();
            List<csgjs_polygon> list_back = new List<csgjs_polygon>();

            for (int i = 0; i < list.Count; i++)
            {
                plane.splitPolygon(list[i], list_front, list_back,ref list_front,ref list_back);
            }

            if (front != null)
            {
                list_front = front.clipPolygons(list_front);
            }
            if (back != null)
            {
                list_back = back.clipPolygons(list_back);
            }
            else
            {
                //list_back.clear();
                list_back.RemoveAll(delegate(csgjs_polygon v)
                {
                    return true;
                });
                //list_back = new List<csgjs_polygon>(); // clear the list by creating a new one
            }
	        //need to make sure to get this insert right...
            // if I'm reading this correctly, it's placing all the items in list_back
            // onto the end of list_front
	        //list_front.insert(list_front.end(), list_back.begin(), list_back.end());
            foreach (csgjs_polygon pb in list_back) 
            {
                list_front.Add(pb.clone());
            }
           // list_front.Insert(
	        return list_front;
        }

        // Return a list of all polygons in this BSP tree.
        public List<csgjs_polygon> allPolygons() 
        {
            List<csgjs_polygon> list = new List<csgjs_polygon>();
            foreach(csgjs_polygon p in polygons)
            {
                list.Add(p.clone());
            }

	        List<csgjs_polygon> list_front = new List<csgjs_polygon>();
            List<csgjs_polygon> list_back = new List<csgjs_polygon>();


	        if (front != null) 
                list_front = front.allPolygons();

	        if (back != null) 
                list_back = back.allPolygons();

	        //list.insert(list.end(), list_front.begin(), list_front.end());
	        //list.insert(list.end(), list_back.begin(), list_back.end());
            foreach (csgjs_polygon pf in list_front) 
            {
                list.Add(pf.clone());
            }

            foreach (csgjs_polygon pb in list_back)
            {
                list.Add(pb.clone());
            }

	        return list;
        }

        // Build a BSP tree out of `polygons`. When called on an existing tree, the
        // new polygons are filtered down to the bottom of the tree and become new
        // nodes there. Each set of polygons is partitioned using the first polygon
        // (no heuristic is used to pick a good split).
        void build( List<csgjs_polygon> list)
        {
	        if (list.Count == 0) 
                return;

	        if (!plane.ok()) 
                plane = list[0].plane;

	        List<csgjs_polygon> list_front = new List<csgjs_polygon>();
            List<csgjs_polygon> list_back = new List<csgjs_polygon>();

	        for (int i = 0; i < list.Count; i++) 
	        {
		        plane.splitPolygon(list[i], polygons, polygons,ref list_front,ref list_back);
	        }
	        if (list_front.Count > 0) 
	        {
		        if (front == null) 
                    front = new csgjs_csgnode();

		        front.build(list_front);
	        }
	        if (list_back.Count > 0) 
	        {
		        if (back == null) 
                    back = new csgjs_csgnode();

		        back.build(list_back);
	        }
        }

        // Return a new CSG solid representing space in either this solid or in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        public static csgjs_csgnode csg_union(csgjs_csgnode  a1, csgjs_csgnode b1)
        {
	        csgjs_csgnode  a = a1.clone();
	        csgjs_csgnode  b = b1.clone();
	        a.clipTo(b);
	        b.clipTo(a);
	        b.invert();
	        b.clipTo(a);
	        b.invert();
	        a.build(b.allPolygons());
	        csgjs_csgnode ret = new csgjs_csgnode(a.allPolygons());
	        a = null;
            b = null;
	        return ret;
        }

        // Return a new CSG solid representing space in this solid but not in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        public static csgjs_csgnode csg_subtract(csgjs_csgnode a1, csgjs_csgnode  b1)
        {
	        csgjs_csgnode  a = a1.clone();
	        csgjs_csgnode b = b1.clone();
	        a.invert();
	        a.clipTo(b);
	        b.clipTo(a);
	        b.invert();
	        b.clipTo(a);
	        b.invert();
	        a.build(b.allPolygons());
	        a.invert();
	        csgjs_csgnode  ret = new csgjs_csgnode(a.allPolygons());
	        a = null;
	        b = null;
	        return ret;
        }

        // Return a new CSG solid representing space both this solid and in the
        // solid `csg`. Neither this solid nor the solid `csg` are modified.
        public static csgjs_csgnode  csg_intersect( csgjs_csgnode  a1,  csgjs_csgnode  b1)
        {
	        csgjs_csgnode  a = a1.clone();
	        csgjs_csgnode  b = b1.clone();
	        a.invert();
	        b.clipTo(a);
	        b.invert();
	        a.clipTo(b);
	        b.clipTo(a);
	        a.build(b.allPolygons());
	        a.invert();
	        csgjs_csgnode ret = new csgjs_csgnode(a.allPolygons());
            a = null;
            b = null;
	        return ret;
        }
    }

}
