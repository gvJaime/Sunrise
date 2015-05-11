using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using System.Collections;

namespace UV_DLP_3D_Printer._3DEngine
{
    /*
     Ray tracing utilites for the 3d engine
     */
    public class RTUtils
    {
        static Object3d m_gp = null; // artificial ground plane
        static public Object3d m_selplane = null; // artificial camera front-facing selection plane

        static bool vecinit = false;
        // keep these vectors around so we don't have to re-allocate them for every intersection test
        static Vector3d u, v, n;              // triangle vectors
        static Vector3d dir, w0, w;           // ray vectors
        static Vector3d V, temp;
        static Point3d IOendp = new Point3d();
        static Point3d IOintersect = new Point3d();
        
        static Point3d GPendp;
        static Point3d GPintersect;

        static Point3d ObSelendp;
        static Point3d ObSelintersect;

        static List<ISectData> m_isectlst;

        static void Initvecs() 
        {
            vecinit = true;
            u = new Vector3d();
            v = new Vector3d();
            n = new Vector3d();
            dir = new Vector3d();
            w0 = new Vector3d();
            w = new Vector3d();
            V = new Vector3d();
            temp = new Vector3d();
            IOendp = new Point3d();
            IOintersect = new Point3d();

            GPendp = new Point3d();
            GPintersect = new Point3d();

            ObSelendp = new Point3d();
            ObSelintersect = new Point3d();

            m_isectlst = new List<ISectData>();
        }
        // intersect3D_RayTriangle(): find the 3D intersection of a ray with a triangle
        //    Input:  a ray R, and a triangle T
        //    Output: *I = intersection point (when it exists)
        //    Return: -1 = triangle is degenerate (a segment or point)
        //             0 =  disjoint (no intersect)
        //             1 =  intersect in unique point I1
        //             2 =  are in the same plane
        static int intersect3D_RayTriangle(Point3d startp, Point3d endp, Polygon T,ref Point3d I)
        {
            float r, a, b;              // params to calc ray-plane intersect

            // get triangle edge vectors and plane normal
            
            u.Set(T.m_points[1].x - T.m_points[0].x, T.m_points[1].y - T.m_points[0].y, T.m_points[1].z - T.m_points[0].z);
            v.Set(T.m_points[2].x - T.m_points[0].x, T.m_points[2].y - T.m_points[0].y, T.m_points[2].z - T.m_points[0].z);
            n = Vector3d.cross(u , v);              // cross product
            dir.Set(endp.x - startp.x, endp.y - startp.y, endp.z - startp.z);

            w0.Set(startp.x - T.m_points[0].x, startp.y - T.m_points[0].y, startp.z - T.m_points[0].z);
            a = (float)-Vector3d.dot(n, w0); //a = -dot(n, w0);
            b = (float)Vector3d.dot(n, dir);//b = dot(n, dir);
            if(Math.Abs(b) < .0001)
            {     // ray is  parallel to triangle plane
                if (a == 0)                 // ray lies in triangle plane
                    return 2;
                else return 0;              // ray disjoint from plane
            }

            // get intersect point of ray with triangle plane
            r = a / b;
            if (r < 0.0)                    // ray goes away from triangle
                return 0;                   // => no intersect
            // for a segment, also test if (r > 1.0) => no intersect

            //*I = R.P0 + r * dir;            // intersect point of ray and plane
            I.x = startp.x + r * dir.x;
            I.y = startp.y + r * dir.y;
            I.z = startp.z + r * dir.z;
            if (float.IsNaN(I.x)) 
            {
                // what's going on here?
                I.x = -1.0f;
            }
            // is I inside T?
            double uu, uv, vv, wu, wv, D;
            uu = Vector3d.dot(u, u);
            uv = Vector3d.dot(u, v);
            vv = Vector3d.dot(v, v);
            //w = I - T.m_points[0];// V0;
            w.Set(I.x - T.m_points[0].x,I.y - T.m_points[0].y,I.z - T.m_points[0].z);// V0;
            wu = Vector3d.dot(w, u);
            wv = Vector3d.dot(w, v);
            D = uv * uv - uu * vv;

            // get and test parametric coords
            double s, t;
            s = (uv * wv - vv * wu) / D;
            if (s < 0.0 || s > 1.0)         // I is outside T
                return 0;
            t = (uv * wu - uu * wv) / D;
            if (t < 0.0 || (s + t) > 1.0)  // I is outside T
                return 0;

            return 1;                       // I is in T
        }

        
        public static bool IntersectPoly(Polygon poly, Point3d start, Point3d end,ref  Point3d intersection)
        {
            if (intersect3D_RayTriangle(start, end, poly, ref intersection) == 1)
                return true;
            return false;
        }
        
        public static bool IntersectSphere(Point3d start,Point3d end,ref Point3d intersect, Point3d center,float radius)
        {
	        bool retval = false;
	        float EO;//EO is distance from start of ray to center of sphere
	        float d,disc,raylen;//

	        temp.Set(center.x - start.x,center.y - start.y,	center.z - start.z);

	        EO = temp.Mag(); // unnormalized length
	        V.Set(end.x - start.x,end.y - start.y,end.z - start.z);
	        raylen = V.Mag();// magnitude of direction vector
	        V.Normalize();// normalize the direction vector
	        disc = (radius*radius) - ((EO*EO) - (raylen*raylen));
	        if(disc < 0.0f)
            {
                retval = false;// no intersection
	        }
            else
            { // compute the intersection point
		        retval = true;
		        d = (float)Math.Sqrt(disc);
		        intersect.x = start.x + ((raylen-d)*V.x);
		        intersect.y = start.y + ((raylen-d)*V.y);
		        intersect.z = start.z + ((raylen-d)*V.z);
	        }
	        return retval;
        }
        /*
         The object selection plane is used to help move objects around
         * This is a plane is centered at the object center and faces the center of the view
         * This is used to determine an intersection along the plane to help move the object
         * This is created by picking a center point, and using the camera's forward and right vectors to generate
         * 2 polygons that form a forward-facing plane at a distance of the camera to the point along the forward vector
         * parameters:
         * center is the  center of the currently selected object
         * fromcamera is the vector from the camera to the center of the selected object
         * cameraup is the up vector of the camera
         */
        public static void UpdateObjectSelectionPlane(Point3d center, Vector3d cameraup, Vector3d cameraright) 
        {

            //get the currently selected object
            //create 2 polygons 
            //forward facing towards the camera
            /*
             p0   p1
             *------
             |\    |
             | \   |  
             |  *  | object center
             |   \ |
             |    \|
             ------|
             p3   p2             
             */
            Point3d p0;
            Point3d p1;
            Point3d p2;
            Point3d p3;

            if (m_selplane == null)
            {
                m_selplane = new Object3d();
                m_selplane.Name = "Selection Plane";
                p0 = new Point3d();
                p1 = new Point3d();
                p2 = new Point3d();
                p3 = new Point3d();
                // add the points
                m_selplane.m_lstpoints.Add(p0);
                m_selplane.m_lstpoints.Add(p1);
                m_selplane.m_lstpoints.Add(p2);
                m_selplane.m_lstpoints.Add(p3);
                //new polygon
                Polygon ply0 = new Polygon();
                ply0.m_points = new Point3d[3];
                //set poly points
                ply0.m_points[0] = p0;
                ply0.m_points[1] = p1;
                ply0.m_points[2] = p2;
                //add the polygon to the model
                m_selplane.m_lstpolys.Add(ply0);

                Polygon ply1 = new Polygon();
                ply1.m_points = new Point3d[3];
                ply1.m_points[0] = p0;
                ply1.m_points[1] = p2;
                ply1.m_points[2] = p3;
                m_selplane.m_lstpolys.Add(ply1);
                m_selplane.tag = Object3d.OBJ_SEL_PLANE; // groundplane tag
            }
            else 
            {
                //references to already created points
                p0 = m_selplane.m_lstpoints[0];
                p1 = m_selplane.m_lstpoints[1];
                p2 = m_selplane.m_lstpoints[2];
                p3 = m_selplane.m_lstpoints[3];
            }
            float scaler = 100.0f;
                        
            p0.Set((cameraup.x / 2) - (cameraright.x / 2), (cameraup.y / 2) - (cameraright.y / 2), (cameraup.z/2) - (cameraright.z/2));
            p1.Set((cameraup.x / 2) + (cameraright.x / 2), (cameraup.y / 2) + (cameraright.y / 2), (cameraup.z / 2) + (cameraright.z / 2));
            p2.Set((-cameraup.x / 2) + (cameraright.x / 2), (-cameraup.y / 2) + (cameraright.y / 2), (-cameraup.z / 2) + (cameraright.z / 2));
            p3.Set((-cameraup.x / 2) - (cameraright.x / 2), (-cameraup.y / 2) - (cameraright.y / 2), (-cameraup.z / 2) - (cameraright.z / 2));
                       
            p0.x *= scaler;
            p0.y *= scaler;
            p0.z *= scaler;

            p1.x *= scaler;
            p1.y *= scaler;
            p1.z *= scaler;

            p2.x *= scaler;
            p2.y *= scaler;
            p2.z *= scaler;

            p3.x *= scaler;
            p3.y *= scaler;
            p3.z *= scaler;

            // initial positions
            m_selplane.Update();
            // center it on the object           
            m_selplane.Translate(center.x, center.y, center.z); 

        }

        private static void CreateGroundPlane()
        {
            m_gp = new Object3d();
            m_gp.Name = "GroundPlane";
            Point3d p0=new Point3d(-500,-500,0);
            Point3d p1=new Point3d(500,-500,0);
            Point3d p2=new Point3d(500,500,0);
            Point3d p3=new Point3d(-500,500,0);
            m_gp.m_lstpoints.Add(p0);
            m_gp.m_lstpoints.Add(p1);
            m_gp.m_lstpoints.Add(p2);
            m_gp.m_lstpoints.Add(p3);

            Polygon ply0 = new Polygon();
            ply0.m_points = new Point3d[3];
            ply0.m_points[0] = p0;
            ply0.m_points[1] = p1;
            ply0.m_points[2] = p2;

            Polygon ply1 = new Polygon();
            ply1.m_points = new Point3d[3];
            ply1.m_points[0] = p0;
            ply1.m_points[1] = p2;
            ply1.m_points[2] = p3;
            m_gp.m_lstpolys.Add(ply0);
            m_gp.m_lstpolys.Add(ply1);
            m_gp.tag = Object3d.OBJ_GROUND; // groundplane tag
            m_gp.Update();
           // p1.m

        }
        private static ISectData ISectObjSelPlane(Vector3d direction, Point3d origin) 
        {
            ISectData isect = null;
            if (m_selplane == null)
                return null;
            direction.Normalize();
            direction.Scale(10000.0f);

            ObSelendp.Set(origin);
            ObSelendp.x += direction.x;
            ObSelendp.y += direction.y;
            ObSelendp.z += direction.z;
            // intersect with the imaginary object selection plane
            if (IntersectSphere(origin, ObSelendp, ref ObSelintersect, m_selplane.m_center, m_selplane.m_radius))
            {
                foreach (Polygon p in m_selplane.m_lstpolys)
                {
                    // try a less- costly sphere intersect here   
                    if (IntersectSphere(origin, ObSelendp, ref ObSelintersect, p.m_center, p.m_radius))
                    {
                        // if it intersects,
                        if (RTUtils.IntersectPoly(p, origin, ObSelendp, ref ObSelintersect))
                        {
                            isect = new ISectData(m_selplane, p, ObSelendp, origin, direction);
                        }
                    }
                }
            }
            return isect;
        
        }
        private static ISectData ISectGroundPlane(Vector3d direction, Point3d origin)
        {
            ISectData isect = null;
            direction.Normalize();
            direction.Scale(10000.0f);

            GPendp.Set(origin);
            GPendp.x += direction.x;
            GPendp.y += direction.y;
            GPendp.z += direction.z;
            // intersect with the imaginary groundplane object;
            if (m_gp == null) 
            {
                CreateGroundPlane();
            }
            if (IntersectSphere(origin, GPendp, ref GPintersect, m_gp.m_center, m_gp.m_radius))
            {
                foreach (Polygon p in m_gp.m_lstpolys)
                {
                    //GPintersect = new Point3d();
                    // try a less- costly sphere intersect here   
                    if (IntersectSphere(origin, GPendp, ref GPintersect, p.m_center, p.m_radius))
                    {
                        // if it intersects,
                        if (RTUtils.IntersectPoly(p, origin, GPendp, ref GPintersect))
                        {
                           isect = new ISectData(m_gp, p, GPintersect, origin, direction);
                        }
                    }
                }
            }
            return isect;
        }

        /// <summary>
        /// This function takes a list of objects and a ray and starting point.
        /// It will return an ArrayList of ISectData,
        /// if no intersections occur, the list will be empty,
        /// the suports variable indicates whether to intersect supports
        /// Each Object should be updated before being added to the list here.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="origin"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        /// 
        static object lck = new object();
        public static List<ISectData> IntersectObjects(Vector3d direction, Point3d origin, List<Object3d> objectsin, bool supports) 
        {
            List<Object3d> objects = new List<Object3d>();
            objects.AddRange(objectsin);
            objects.Add(RTUtils.m_selplane);
            //List<ISectData> m_isectlst = new List<ISectData>();
            
            try
            {
                if (!vecinit) 
                {
                    Initvecs();
                }
                m_isectlst.Clear();
                direction.Normalize();
                direction.Scale(10000.0f);


                IOendp.Set(origin);
                IOendp.x += direction.x;
                IOendp.y += direction.y;
                IOendp.z += direction.z;
                lock (lck)
                {
                    foreach (Object3d obj in objects)
                    {
                        if (obj.tag == Object3d.OBJ_SUPPORT && !supports)
                            continue;
                        // try a less- costly sphere intersect here   
                        if (IntersectSphere(origin, IOendp, ref IOintersect, obj.m_center, obj.m_radius))
                        {
                            foreach (Polygon p in obj.m_lstpolys)
                            {
                                //IOintersect = new Point3d();
                                // try a less- costly sphere intersect here   
                                if (IntersectSphere(origin, IOendp, ref IOintersect, p.m_center, p.m_radius))
                                {
                                    // if it intersects,
                                    if (RTUtils.IntersectPoly(p, origin, IOendp, ref IOintersect))
                                    {
                                        m_isectlst.Add(new ISectData(obj, p, IOintersect, origin, direction));
                                    }
                                }
                            }
                        }
                    }
                }
                //intersect the ground plane
                ISectData gp = ISectGroundPlane(direction, origin);
                if (gp != null)
                {
                    m_isectlst.Add(gp);
                }
                /*
                //intersect the object selection plane
                ISectData obsel = ISectObjSelPlane(direction, origin);
                if (obsel != null) 
                {
                    m_isectlst.Add(obsel);
                }
                 */ 
                m_isectlst.Sort();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return m_isectlst;
        }
    }
}
