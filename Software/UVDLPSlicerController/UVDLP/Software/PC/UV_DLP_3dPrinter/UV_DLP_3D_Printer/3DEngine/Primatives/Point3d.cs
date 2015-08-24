using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Engine3D
{
    public class Point3d
    {
        // private static float epsilon = .0001f; // define an epsilon value for matching
        private static float epsilon = Single.Epsilon; // define an epsilon value for matching
        public float x, y, z;
        public Point3d() 
        {
            x = y = z = 0.0f;
        }

        public Point3d(Point3d pnt)
        {
            x = pnt.x;
            y = pnt.y;
            z = pnt.z;
            
        }

        public static Vector3d operator -(Point3d c1, Point3d c2) 
        {
            Vector3d ret = new Vector3d();
            ret.Set(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
            return ret;
        }

        public static Point3d operator +(Point3d c1, Vector3d c2)
        {
            Point3d ret = new Point3d();
            ret.Set(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
            return ret;
        }
        

        /// <summary>
        /// This is an epsilon-based match
        /// </summary>
        /// <param name="pnt"></param>
        /// <returns></returns>
        public bool Matches(Point3d pnt) 
        {
            if (pnt.x >= (x - epsilon) && pnt.x <= (x + epsilon))
            {
                if (pnt.y >= (y - epsilon) && pnt.y <= (y + epsilon))
                {
                    if (pnt.z >= (z - epsilon) && pnt.z <= (z + epsilon))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// this function tests for an exact match of points
        /// </summary>
        /// <param name="pnt"></param>
        /// <returns></returns>
        public bool IsEqual(Point3d pnt) 
        {
            if (x == pnt.x && y == pnt.y && z == pnt.z)
                return true;
            return false;
        }
        public Point3d(float xp, float yp, float zp)
        {
            Set(xp, yp, zp);
        }
        public void Set(float xp, float yp, float zp)
        {
            x = xp;
            y = yp;
            z = zp;     
        }
        public void Translate(float xp, float yp, float zp)
        {
            x += xp;
            y += yp;
            z += zp;     
        }
        public void Set(Point3d pnt) 
        {
            Set(pnt.x, pnt.y, pnt.z);
        }

        public void Load(BinaryReader br) 
        {
            x = br.ReadSingle();
            y = br.ReadSingle();
            z = br.ReadSingle();
        }

        public void Load(StreamReader sr) 
        {
            x = float.Parse(sr.ReadLine());
            y = float.Parse(sr.ReadLine());
            z = float.Parse(sr.ReadLine());
        }
        public void Save(StreamWriter sw) 
        {
            sw.WriteLine(x);
            sw.WriteLine(y);
            sw.WriteLine(z);
        }
    }    
}
