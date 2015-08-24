using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
namespace UV_DLP_3D_Printer
{
    /* This class holds a Z min/max value for an object*/
    public class MinMax
    {
        public double m_min, m_max;
        public bool InRange(double z) 
        {
            if (z >= m_min && z <= m_max)
                return true;

            return false;
        }
        public void Translate(Point3d pnt) 
        {
            m_min += pnt.z;
            m_max += pnt.z;
        }

    }
}
