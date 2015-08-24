using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UV_DLP_3D_Printer._3DEngine.CSG
{
    // a vector class to hold either a vector or 3d point
    public class csgjs_vector
    {
        public float x, y, z;
        public csgjs_vector()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        public csgjs_vector(float xp, float yp, float zp)
        {
            x = xp;
            y = yp;
            z = zp;
        }
        public csgjs_vector clone() 
        {
            return new csgjs_vector(x, y, z);
        }
        public static csgjs_vector operator + (csgjs_vector  a,  csgjs_vector  b) 
        { 
            return new csgjs_vector(a.x + b.x, a.y + b.y, a.z + b.z); 
        }
        public static csgjs_vector operator - ( csgjs_vector  a,  csgjs_vector  b) 
        { 
            return new csgjs_vector(a.x - b.x, a.y - b.y, a.z - b.z); 
        }
        public static csgjs_vector operator * ( csgjs_vector  a, float b) 
        { 
            return new csgjs_vector(a.x * b, a.y * b, a.z * b); 
        }
        public static csgjs_vector operator / ( csgjs_vector  a, float b) 
        { 
            return a * (1.0f / b); 
        }
        public static float dot( csgjs_vector  a,  csgjs_vector  b) 
        { 
            return a.x * b.x + a.y * b.y + a.z * b.z; 
        }
        public static csgjs_vector lerp( csgjs_vector  a,  csgjs_vector  b, float v) 
        { 
            return a + (b - a) * v; 
        }
        public static csgjs_vector negate( csgjs_vector  a) 
        { 
            return a * -1.0f; 
        }
        public static float length( csgjs_vector  a) 
        { 
            return (float)Math.Sqrt(dot(a, a)); 
        }
        public static csgjs_vector unit( csgjs_vector  a) 
        { 
            return a / length(a); 
        }
        public static csgjs_vector cross( csgjs_vector  a,  csgjs_vector  b) 
        { 
            return new csgjs_vector(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x); 
        }
    }
}
