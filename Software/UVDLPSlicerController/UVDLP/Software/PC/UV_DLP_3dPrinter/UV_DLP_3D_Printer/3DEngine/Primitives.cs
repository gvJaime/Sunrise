using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using UV_DLP_3D_Printer;

namespace UV_DLP_3D_Printer._3DEngine
{
    /// <summary>
    /// This is a utility class with static functions that will
    /// return a variety of object primitives, This deviates from the 
    /// 'cylinder' class which is needed as a base class to dynamically 
    /// scale supports
    /// </summary>
    public class Primitives
    {
        public static Object3d Sphere(float radius, int rings, int sectors)
        {
            try
            {
                Object3d sp = new Object3d();
                sp.Name = "Sphere";
                float R = 1f/(float)(rings-1);
                float S = 1f/(float)(sectors-1);

                float M_PI = (float)Math.PI;// *0.0174532925f; // with deg2rad
                float M_PI_2 = M_PI / 2;
                for(int r = 0; r < rings; r++) 
                {
                    for(int s = 0; s < sectors; s++) 
                    {
                        float y =(float) Math.Sin( - M_PI_2 + M_PI * r * R );
                        float x = (float)Math.Cos(2 * M_PI * s * S) * (float)Math.Sin(M_PI * r * R);
                        float z = (float)Math.Sin(2 * M_PI * s * S) * (float)Math.Sin(M_PI * r * R);

                        Point3d pnt = new Point3d(x * radius, y * radius, z * radius);
                        sp.m_lstpoints.Add(pnt);
                    }
                }

                //indices.resize(rings * sectors * 4);
                //std::vector<GLushort>::iterator i = indices.begin();
                for(int r = 0; r < rings - 1; r++) 
                {
                    for (int s = 0; s < sectors - 1; s++)
                    {
                        Polygon p1 = new Polygon();
                        Polygon p2 = new Polygon();
                        sp.m_lstpolys.Add(p1);
                        sp.m_lstpolys.Add(p2);
                        p1.m_points = new Point3d[3];
                        p2.m_points = new Point3d[3];
                        p1.m_points[2] = sp.m_lstpoints[r * sectors + s];
                        p1.m_points[1] = sp.m_lstpoints[r * sectors + (s + 1)];
                        p1.m_points[0] = sp.m_lstpoints[(r + 1) * sectors + (s + 1)];

                        p2.m_points[2] = sp.m_lstpoints[(r + 1) * sectors + (s + 1)];
                        p2.m_points[1] = sp.m_lstpoints[(r + 1) * sectors + s];
                        p2.m_points[0] = sp.m_lstpoints[r * sectors + s];                        
                    }
                }
                sp.Update();
                sp.Rotate(90 * 0.0174532925f, 0, 0);
                sp.Update();
                return sp;            
            }
            catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                return null;
            }
        }
    }
}
