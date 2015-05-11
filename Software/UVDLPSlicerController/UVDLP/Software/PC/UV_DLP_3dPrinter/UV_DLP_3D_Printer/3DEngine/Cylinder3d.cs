using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer;
using System.Collections;

namespace Engine3D
{
    public class Cylinder3d : Object3d
    {
        public Cylinder3d() 
        {
            
        }

        /*
            Given a radius length r and an angle t in radians and a circle's center (h,k),
         *  you can calculate the coordinates of a point on the circumference as follows
         *  (this is pseudo-code, you'll have to adapt it to your language):
            float x = r*cos(t) + h;
            float y = r*sin(t) + k;         
         */

        public void Create(float bottomradius, float topradius, float height, int numdivscirc, int numdivsheight) 
        {
            //generate sets of points that describe a circle vertically
           // int idx = 0; // this points to the first point in the circle
            float zlev = 0.0f; // start at the bottom of the cylinder
            //Name = "Cylinder";
           // for (int cnt = 0; cnt < numdivsheight; cnt++)
           // {
            GenerateCirclePoints(bottomradius, numdivscirc, zlev,true); // bottom
            zlev += height;
            GenerateCirclePoints(topradius, numdivscirc, zlev,true); // top
            // now generate side polygons
            for (int cnt = 0; cnt < numdivscirc; cnt++) 
            {
                /* left
                 3
                 |\
                 | \
                 |__\
                 2   1
                 
                 right
                 
                 2   3
                 _____
                 \   |
                  \  |
                   \ |
                    \|                 
                     1
                 * 
                 * 
                 */
                

                // the left looks correct
                int topidx = numdivscirc + 1; // index to the first point in the top circle
                Polygon plyl = new Polygon();
                m_lstpolys.Add(plyl);
                plyl.m_points = new Point3d[3]; // create some point storage
                plyl.m_points[0] = (Point3d)m_lstpoints[cnt];
                plyl.m_points[1] = (Point3d)m_lstpoints[(cnt + 1) % numdivscirc];
                plyl.m_points[2] = (Point3d)m_lstpoints[cnt + topidx];
                plyl.CalcCenter();
                plyl.CalcNormal();

                // bottom faces
                int centeridx = numdivscirc;
                Polygon plb = new Polygon();
                m_lstpolys.Add(plb);
                plb.m_points = new Point3d[3]; // create some point storage
                plb.m_points[0] = (Point3d)m_lstpoints[centeridx]; // the first point is always the center point
                plb.m_points[1] = (Point3d)m_lstpoints[(cnt + 1) % numdivscirc];
                plb.m_points[2] = (Point3d)m_lstpoints[cnt];
                plb.CalcCenter();
                plb.CalcNormal();

                
                
                Polygon plyr = new Polygon();
                m_lstpolys.Add(plyr);
                plyr.m_points = new Point3d[3]; // create some point storage
                plyr.m_points[0] = (Point3d)m_lstpoints[(cnt + 1) % numdivscirc];
                plyr.m_points[1] = (Point3d)m_lstpoints[((cnt + 1) % numdivscirc) + topidx]; // 
                plyr.m_points[2] = (Point3d)m_lstpoints[cnt + topidx]; // the point directly above it

                plyr.CalcCenter();
                plyr.CalcNormal();
                  
                //int topidx = numdivscirc + 1; // index to the first point in the top circle
                // top faces
                centeridx = topidx + numdivscirc;
                Polygon plt = new Polygon();
                m_lstpolys.Add(plt);
                plt.m_points = new Point3d[3]; // create some point storage
                plt.m_points[0] = (Point3d)m_lstpoints[centeridx]; // the first point is always the center pointt
                plt.m_points[2] = (Point3d)m_lstpoints[(cnt + 1) % numdivscirc + topidx];
                plt.m_points[1] = (Point3d)m_lstpoints[topidx + cnt];
                plt.CalcCenter();
                plt.CalcNormal();

            }
            //idx += numdivscirc;
           // }        
            CalcCenter();
            //CalcMinMaxes();
            FindMinMax();

        }

        /*
         This function generates a list of points for a cirlce add a certain level,
         * and adds them to the object point list
         */
        protected void GenerateCirclePoints(double radius, int numdivscirc,float zlev,bool addcenter)
        {            
            float step = (float)(Math.PI * 2) / numdivscirc;
            float t = 0.0f;
            for (int cnt = 0; cnt < numdivscirc; cnt++)
            {
                Point3d pnt;               
                //create a new point and add it
                pnt = new Point3d(); // bottom points
                m_lstpoints.Add(pnt);               

                //Point3d pnt = new Point3d(); // bottom points
                pnt.x = (float)(radius * Math.Cos(t));
                pnt.y = (float)(radius * Math.Sin(t));
                pnt.z = (float)zlev;                
                t += step;
            }
            if (addcenter)
            {
                // add another point right in the center for the triangulating the face
                Point3d centerpnt = new Point3d(); // bottom points
                centerpnt.x = 0;
                centerpnt.y = 0;
                centerpnt.z = zlev;

                m_lstpoints.Add(centerpnt);
            }
        }


    }
}
