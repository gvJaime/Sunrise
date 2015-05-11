using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
namespace UV_DLP_3D_Printer._3DEngine.Supports
{
    public static class SupportData
    {
        static Point3d[] crosspoints = 
        {
            new Point3d(-.25f,-.25f,0f),
            new Point3d(-.25f,-.5f,0f),
            new Point3d(.25f,-.5f,0f),
            new Point3d(.25f,-.25f,0f),
            new Point3d(.5f,-.25f,0f),
            new Point3d(.5f,.25f,0f),
            new Point3d(.25f,.25f,0f),
            new Point3d(.25f,.5f,0f),
            new Point3d(-.25f,.5f,0f),
            new Point3d(-.25f,.25f,0f),
            new Point3d(-.5f,.25f,0f),
            new Point3d(-.5f,-.25f,0f)
        };
        //     new Point3d(-.25f,-.25f,0),new Point3d(-.25f,-.5,0),new Point3d(.25f,-.5,0),new Point3d(.25f,-.25f,0),new Point3d(.5,-.25f,0),new Point3d(.5,.25f,0),new Point3d(.25f,.25f,0),new Point3d(.25f,.5,0),new Point3d(-.25f,.5,0),new Point3d(-.25f,.25f,0),new Point3d(-.5,.25f,0),new Point3d(-.5,-.25f,0)

        public static Point3d []GetCrossPoints() 
        {
            return crosspoints;
        }
    }
}
