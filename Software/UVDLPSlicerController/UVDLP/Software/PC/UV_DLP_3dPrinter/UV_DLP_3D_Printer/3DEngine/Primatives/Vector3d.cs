using System;
using System.Collections.Generic;
using System.Text;
using UV_DLP_3D_Printer;

namespace Engine3D
{
    public class Vector3d : Point3d
    {
        public Vector3d() 
        {
            x = y = z = 0.0f;
        }
        public Vector3d(float xp, float yp, float zp)
        {
            x = xp;
            y = yp;
            z = zp;
        }

        public Vector3d clone() 
        {
            return new Vector3d(x, y, z);
        }
        public static Vector3d operator + (Vector3d  a,  Vector3d  b) 
        { 
            return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z); 
        }
        public static Vector3d operator - ( Vector3d  a,  Vector3d  b) 
        { 
            return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z); 
        }
        public static Vector3d operator * ( Vector3d  a, float b) 
        { 
            return new Vector3d(a.x * b, a.y * b, a.z * b); 
        }
        public static Vector3d operator *(float b, Vector3d a)
        {
            return new Vector3d(a.x * b, a.y * b, a.z * b);
        }
        public static Vector3d operator /(Vector3d a, float b) 
        { 
            return a * (1.0f / b); 
        }
        public static double dot( Vector3d  a,  Vector3d  b) 
        { 
            return a.x * b.x + a.y * b.y + a.z * b.z; 
        }
        public static Vector3d lerp( Vector3d  a,  Vector3d  b, float v) 
        { 
            return a + (b - a) * v; 
        }
        public static Vector3d negate( Vector3d  a) 
        { 
            return a * -1.0f; 
        }
        public static float length( Vector3d  a) 
        { 
            return (float)Math.Sqrt(dot(a, a)); 
        }
        public static Vector3d unit( Vector3d  a) 
        { 
            return a / length(a); 
        }
        public static Vector3d cross( Vector3d  a,  Vector3d  b) 
        { 
            return new Vector3d(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x); 
        }

        public float Mag()
        {
            return (float)Math.Sqrt((x * x) + (y * y) + (z * z));
        }
        public void Scale(float scale) 
        {
            x *= scale;
            y *= scale;
            z *= scale;
        }
        public Vector3d Cross(Vector3d v) 
        {
            Vector3d cr = new Vector3d();
            cr.x = y * v.z - z * v.y;
            cr.y = z * v.x - x * v.z;
            cr.z = x * v.y - y * v.x;
            return cr;
        }
        public double Dot(Vector3d v) //dot product 
        {
	        double dp = ( x * v.x ) +
		               ( y * v.y ) +
		     	       ( z * v.z );
	        return dp;
        }

       public void Normalize()  
       {
    	    float oneoverdist  = 1.0f / Mag();
            x *= oneoverdist;
            y *= oneoverdist;
            z *= oneoverdist;
       }
    }
}
