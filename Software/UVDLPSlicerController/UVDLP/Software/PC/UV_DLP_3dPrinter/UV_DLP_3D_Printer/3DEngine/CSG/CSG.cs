using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Engine3D;
using System.Threading;
// Original CSG.JS library by Evan Wallace (http://madebyevan.com), under the MIT license.
// GitHub: https://github.com/evanw/csg.js/
// 
// C++ port by Tomasz Dabrowski (http://28byteslater.com), under the MIT license.
// GitHub: https://github.com/dabroz/csgjs-cpp/
//
// C# port by Steve Hernandez (http://www.envisionlabs.net/), under the MIT license.
// GitHub: https://github.com/Pacmanfan/UVDLPSlicerController
// 
// Constructive Solid Geometry (CSG) is a modeling technique that uses Boolean
// operations like union and intersection to combine 3D solids. This library
// implements CSG operations on meshes elegantly and concisely using BSP trees,
// and is meant to serve as an easily understandable implementation of the
// algorithm. All edge cases involving overlapping coplanar polygons in both
// solids are correctly handled.

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    /// <summary>
    /// The CSG class is the main entry point into boolean operations for objects
    /// </summary>
    public class CSG
    {
        public enum eCSGEvent 
        {
            eStarted,
            eProgress,
            eCompleted,
            eError
        }
        public delegate void CSGEventDel(eCSGEvent ev, string msg, Object3d dat);

        public const float csgjs_EPSILON = 0.00001f;
        public Thread m_runthread;
        public bool m_running;
        private eCSGOp m_op;
        private Object3d m_obj1, m_obj2;
        private static CSG m_instance = null;
        private static int STACKSIZE = 256 * 1024 * 1024; // 16mb stack size
        //private bool m_cancel;
        public CSGEventDel CSGEvent;

        public enum eCSGOp 
        {
            eUnion,
            eIntersection,
            eSubtraction
        }
        private CSG() 
        {
            m_runthread = null;
            m_running = false;
        }
        public static CSG Instance() 
        {
            if (m_instance == null) 
            {
                m_instance = new CSG();
            }
            return m_instance;
        }
        private void RaiseEvent(eCSGEvent ev, string msg, Object3d dat) 
        {
            if (CSGEvent != null) 
            {
                CSGEvent(ev, msg, dat);
            }
        }
        public void StartOp(eCSGOp op, Object3d obj1, Object3d obj2) 
        {
            m_op = op;
            m_obj1 = obj1;
            m_obj2 = obj2;
            //m_cancel = false;
            m_runthread = new Thread(new ThreadStart(RunThread), STACKSIZE);
            m_running = true;
            m_runthread.Start();
        }
        public void Cancel() 
        {
           // m_cancel = true;
            m_running = false;
        }
        private void RunThread() 
        {
            RaiseEvent(eCSGEvent.eStarted, "", null);
            try
            {
                List<csgjs_polygon> a = ConvertTo(m_obj1);
                List<csgjs_polygon> b = ConvertTo(m_obj2);
                csgjs_csgnode A = new csgjs_csgnode(a);
                csgjs_csgnode B = new csgjs_csgnode(b);
                csgjs_csgnode AB = null;
                switch (m_op)
                {
                    case eCSGOp.eIntersection:
                        AB = csgjs_csgnode.csg_intersect(A, B);
                        break;
                    case eCSGOp.eSubtraction:
                        AB = csgjs_csgnode.csg_subtract(A, B);
                        break;
                    case eCSGOp.eUnion:
                        AB = csgjs_csgnode.csg_union(A, B);
                        break;
                }
                //raise an event
                Object3d res = ConvertFrom(AB.allPolygons());
                RaiseEvent(eCSGEvent.eCompleted, "", res);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
                RaiseEvent(eCSGEvent.eError, "", null);
            }
            m_running = false;
        }
        private Object3d Union(Object3d obj1, Object3d obj2) 
        {            
            List<csgjs_polygon> a = ConvertTo(obj1);
            List<csgjs_polygon> b = ConvertTo(obj2);
            csgjs_csgnode A = new csgjs_csgnode(a);
            csgjs_csgnode B = new csgjs_csgnode(b);
            csgjs_csgnode AB = csgjs_csgnode.csg_union(A,B);
            return ConvertFrom(AB.allPolygons());
        }
        private Object3d Subtract(Object3d obj1, Object3d obj2)
        {
            List<csgjs_polygon> a = ConvertTo(obj1);
            List<csgjs_polygon> b = ConvertTo(obj2);
            csgjs_csgnode A = new csgjs_csgnode(a);
            csgjs_csgnode B = new csgjs_csgnode(b);
            csgjs_csgnode AB = csgjs_csgnode.csg_subtract(A, B);
            return ConvertFrom(AB.allPolygons());
        }
        private Object3d Intersect(Object3d obj1, Object3d obj2)
        {
            List<csgjs_polygon> a = ConvertTo(obj1);
            List<csgjs_polygon> b = ConvertTo(obj2);
            csgjs_csgnode A = new csgjs_csgnode(a);
            csgjs_csgnode B = new csgjs_csgnode(b);
            csgjs_csgnode AB = csgjs_csgnode.csg_intersect(A, B);
            return ConvertFrom(AB.allPolygons());
        }
        private List<csgjs_polygon> ConvertTo(Object3d obj) 
        {
            List<csgjs_polygon> polys = new List<csgjs_polygon>();
            foreach (Polygon p in obj.m_lstpolys) 
            {
                List<csgjs_vertex> list = new List<csgjs_vertex>();
                list.Add(new csgjs_vertex(p.m_points[0]));
                list.Add(new csgjs_vertex(p.m_points[1]));
                list.Add(new csgjs_vertex(p.m_points[2]));
                csgjs_polygon newpoly = new csgjs_polygon(list); // create  from a list of vertexes to initialize plane
                polys.Add(newpoly);
            }
            return polys;
        }
        private Object3d ConvertFrom(List<csgjs_polygon> lstply) 
        {
            Object3d obj = new Object3d();
            for (int i = 0; i < lstply.Count; i++)
            {
                csgjs_polygon poly = lstply[i];

                for (int j = 2; j < poly.vertices.Count; j++)
                {
                    Polygon ply = new Polygon(); // create a new polygon
                    ply.m_points = new Point3d[3];
                    obj.m_lstpolys.Add(ply); //add it to the list

                    Point3d p0 = new Point3d();
                    Point3d p1 = new Point3d();
                    Point3d p2 = new Point3d();

                    p0.Set(poly.vertices[0].pos.x, poly.vertices[0].pos.y, poly.vertices[0].pos.z);
                    p1.Set(poly.vertices[j - 1].pos.x, poly.vertices[j - 1].pos.y, poly.vertices[j - 1].pos.z);
                    p2.Set(poly.vertices[j].pos.x, poly.vertices[j].pos.y, poly.vertices[j].pos.z);

                    ply.m_points[0] = p0;
                    ply.m_points[1] = p1;
                    ply.m_points[2] = p2;

                    obj.m_lstpoints.Add(p0);
                    obj.m_lstpoints.Add(p1);
                    obj.m_lstpoints.Add(p2);
                }
            }
            obj.Update();
            return obj;
        }
    }
}
