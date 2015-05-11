using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Engine3D;
using UV_DLP_3D_Printer;
using System.Collections;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using UV_DLP_3D_Printer._3DEngine;

namespace Engine3D
{
    public class Polygon 
    {
        public Vector3d m_normal; // the plane normal
        public Point3d m_center; // the calculated center of the polygon
        public float m_radius; // the radius of the poly for sphere intersection testing
        public Color m_color,m_colorsource;
        public Point3d[] m_points; // points in poly, also contained in parent objects points list 
        public MinMax m_minmax; // cached for slicing
        public bool m_hidden; // for hiding polygons during the manual support genetation step
        //move vars cached for slicing
        public int tag; // special markers for this polygon
        public static int TAG_REGULAR        = 0;
        public static int TAG_MARKDOWN      = 1;
            
        public Polygon() 
        {
            m_normal = new Vector3d();
            m_radius = 0.0f;
            m_colorsource = Color.Gray;
            m_color = m_colorsource;// Color.Gray;
            m_center = new Point3d();
            m_minmax = new MinMax(); // really should be bounding box
            m_hidden = false;
            tag = TAG_REGULAR;
        }
      
        public void CalcNormal() 
        {
            float Ax, Ay, Az;
            float Bx, By, Bz;
            Ax = m_points[1].x - m_points[0].x;
            Ay = m_points[1].y - m_points[0].y;
            Az = m_points[1].z - m_points[0].z;
            Bx = m_points[2].x - m_points[0].x;
            By = m_points[2].y - m_points[0].y;
            Bz = m_points[2].z - m_points[0].z;        
   
            float Nx = (Ay * Bz) - (Az * By);
            float Ny = (Az * Bx) - (Ax * Bz);
            float Nz = (Ax * By) - (Ay * Bx);
            m_normal.x = Nx;
            m_normal.y = Ny;
            m_normal.z = Nz;
            float length = (float)Math.Sqrt((m_normal.x * m_normal.x) + (m_normal.y * m_normal.y) + (m_normal.z * m_normal.z));
            m_normal.x /= length;
            m_normal.y /= length;
            m_normal.z /= length;
        }
        public PolyLine3d IntersectZPlane(float zcur)
        {
            try
            {
                PolyLine3d segment = new PolyLine3d();
                //Intersect the polygon with the specified Z-Plane 
                // this will return 0,1,2 intersections.
                // using the returns, impose several rules
                    //use a polyline to do the intersections

                Point3d p1, p2, p3; // intersection points for the 3 3d line segments
                int count = 0;
                Point3d[] lst = new Point3d[3];
                PolyLine3d lineseg1 = null;
                PolyLine3d lineseg2 = null;
                PolyLine3d lineseg3 = null;

                lineseg1 = new PolyLine3d();
                lineseg1.AddPoint(m_points[0]); // 0-1
                lineseg1.AddPoint(m_points[1]);
                p1 = lineseg1.IntersectZ(zcur);
                if (p1 != null)
                {
                    count++;
                    segment.AddPoint(p1);
                }


                lineseg2 = new PolyLine3d();
                lineseg2.AddPoint(m_points[1]); // 1-2
                lineseg2.AddPoint(m_points[2]);

                p2 = lineseg2.IntersectZ(zcur);
                if (p2 != null)
                {
                    count++;
                    segment.AddPoint(p2);
                }

                if (count == 0)
                    return null;

                // there is no sense in doing the 3rd intersection if we don't have 
                // at least 1 point at this stage

                lineseg3 = new PolyLine3d();
                lineseg3.AddPoint(m_points[2]); // 2-0
                lineseg3.AddPoint(m_points[0]);
                p3 = lineseg3.IntersectZ(zcur);
                if (p3 != null)
                {
                    count++;
                    segment.AddPoint(p3);
                }
                if (count != 2) // might be 0,1 or 3
                    return null;

                segment.m_color = Color.Red;
                segment.m_derived = this; // set the parent
                return segment;
            }
            catch (Exception) 
            {
                return null;
            }
        }


        public void CalcMinMax() 
        {
            m_minmax.m_min = m_points[0].z;
            m_minmax.m_max = m_points[0].z;

            foreach (Point3d pnt in m_points)
            {
                if (pnt.z > m_minmax.m_max)
                    m_minmax.m_max = pnt.z;

                if (pnt.z < m_minmax.m_min)
                    m_minmax.m_min = pnt.z;

            }
            //return mm;
        }
        /// <summary>
        /// Flip the winding order
        /// </summary>
        public void FlipWinding() 
        {
            Point3d pnt;
            pnt = m_points[0];
            m_points[0] = m_points[2];
            m_points[2] = pnt;
        }
        public void CalcCenter() 
        {
            try
            {
                m_center.Set(0, 0, 0);

                foreach (Point3d pnt in m_points)
                {
                    m_center.x += pnt.x;
                    m_center.y += pnt.y;
                    m_center.z += pnt.z;
                }
                m_center.x /= m_points.Length; // number of points
                m_center.y /= m_points.Length; // number of points
                m_center.z /= m_points.Length; // number of points
            }catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
        public bool SpheresIntersect(Polygon ply) 
        {
            bool retval = false;
            Vector3d vec = m_center - ply.m_center;
            float dist = (vec.x * vec.x) + (vec.y * vec.y) + (vec.z * vec.z);
            float mindist = m_radius * ply.m_radius;
            if (dist <= (mindist * mindist))
                retval = true;
            return retval;
        }
        /// <summary>
        /// This will return true if this poly and the specified share and edge (2 points)
        /// use this sparingly, it's a n^2 time routine 
        /// </summary>
        /// <param name="ply"></param>
        /// <returns></returns>
        public bool SharesEdge(Polygon ply) 
        {
            bool ret = false;
            int cnt = 0;
            foreach(Point3d pnt in m_points)
            {
                foreach (Point3d pnt2 in ply.m_points) 
                {
                    if (pnt.Matches(pnt2)) 
                    {
                        cnt++;
                    }
                }
            }
            if (cnt == 2) 
            {
                ret = true;
            }
            return ret;
        }

        // this will skip over re-calculateing the radius and normal, because we just moved
        public void UpdateMove(float x, float y, float z) 
        {
            //CalcCenter();
            m_center.Translate(x, y, z);
            //CalcMinMax();
            m_minmax.m_min += z;
            m_minmax.m_max += z;
            
        }
        /*
         The update function should be called after the containing object
         * moves, scales or rotates to update the polygon information
         */
        public void Update() 
        {
            CalcCenter();
            CalcMinMax();
            CalcRadius();
            CalcNormal();
        }

        static Vector3d newlen = new Vector3d(); // for calculating the radius of this poly
        public void CalcRadius()
        {	        
            newlen.Set(0, 0, 0);
            for (int c = 0; c < m_points.Length; c++)
	        {
                newlen.x = m_center.x - m_points[c].x;
                newlen.y = m_center.y - m_points[c].y;
                newlen.z = m_center.z - m_points[c].z;
                if(newlen.Mag() >= m_radius)
                {
                    m_radius = newlen.Mag();
                }
	        }
        }
        
        public void RenderGL(int wireframe,bool alpha, Color clr) 
        {
            // clip test before rendering 
            // use center point and radius to determine visibility (3d test)
            // 
            // test dot product of the transformed normal
            //Color clr = m_color;
            if (m_hidden == true)
                return; // not displaying this poly...
            float oldLineWidth = 1;
            if (wireframe > 0)
            {
                GL.GetFloat(GetPName.LineWidth, out oldLineWidth);
                GL.LineWidth(wireframe);
                GL.Begin(PrimitiveType.LineLoop);//.LineStrip);
            }else
            {
                GL.Begin(PrimitiveType.Triangles);
                if (tag == TAG_MARKDOWN)
                {
                    clr = Color.Red;
                }
            }

            if (alpha)
            {
                if (tag == TAG_MARKDOWN)
                {
                    GL.Color3(clr);
                }
                else
                {
                    GL.Color4((byte)clr.R, (byte)clr.G, (byte)clr.B, (byte)128);
                }
            }
            else
            {
                GL.Color3(clr);
            }
            
            GL.Normal3(m_normal.x, m_normal.y, m_normal.z);
            foreach (Point3d p in this.m_points)
            {               
                GL.Vertex3(p.x, p.y, p.z);
            }
            GL.End();
            if (wireframe > 0)
                GL.LineWidth(oldLineWidth);
        }
        

    }
}
