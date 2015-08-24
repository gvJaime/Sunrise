using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using System.Drawing;
using UV_DLP_3D_Printer._3DEngine;

namespace Engine3D
{
    public class AxisIndicator
    {
        List<Object3d> axisInd;
        List<Quad3d> axisCent;

        public AxisIndicator()
        {
        }

        public void Create(float spokeLen, float spokeRad, float centerRad, float arrowLen, float arrowRad)
        {
            float ang90 = (float)(Math.PI / 2.0);
            Cylinder3d cn;
            axisInd = new List<Object3d>();

            // x arrow
            cn = new Cylinder3d();
            cn.Create(arrowRad, 0.0f, arrowLen, 8, 2);
            cn.SetColor(Color.Red);
            cn.Rotate(0, ang90, 0);
            cn.Translate(spokeLen + arrowLen / 2 - 0.1f, 0, -arrowLen / 2);
            axisInd.Add(cn);
            cn = new Cylinder3d();
            cn.Create(spokeRad, spokeRad, spokeLen, 8, 2);
            cn.Rotate(0, ang90, 0);
            cn.Translate(spokeLen / 2, 0, -spokeLen / 2);
            cn.SetColor(Color.Red);
            axisInd.Add(cn);

            // y arrow
            cn = new Cylinder3d();
            cn.Create(arrowRad, 0.0f, arrowLen, 8, 2);
            cn.SetColor(Color.Green);
            cn.Rotate(-ang90, 0, 0);
            cn.Translate(0, spokeLen + arrowLen / 2 - 0.1f, -arrowLen / 2);
            axisInd.Add(cn);
            cn = new Cylinder3d();
            cn.Create(spokeRad, spokeRad, spokeLen, 8, 2);
            cn.Rotate(-ang90, 0, 0);
            cn.Translate(0, spokeLen / 2, -spokeLen / 2);
            cn.SetColor(Color.Green);
            axisInd.Add(cn);

            // z arrow
            cn = new Cylinder3d();
            cn.Create(arrowRad, 0.0f, arrowLen, 8, 2);
            cn.SetColor(Color.Blue);
            cn.Translate(0, 0, spokeLen);
            axisInd.Add(cn);
            cn = new Cylinder3d();
            cn.Create(spokeRad, spokeRad, spokeLen, 8, 2);
            cn.SetColor(Color.Blue);
            axisInd.Add(cn);

            // center
            //Object3d sp = Primitives.Sphere(centerRad, 7, 8);
            //sp.SetColor(Color.Yellow);
            //axisInd.Add(sp);
            float r = centerRad;
            axisCent = Quad3d.CreateQuadBox(-r, r, -r, r, -r, r);
            axisCent[0].imgname = "axisx";
            axisCent[1].imgname = "axisx";
            axisCent[2].imgname = "axisy";
            axisCent[3].imgname = "axisy";
            axisCent[4].imgname = "axisz";
            axisCent[5].imgname = "axisz";

        }

        public void RenderGL()
        {
            foreach (Object3d obj in axisInd)
            {
                obj.RenderGL(false, false, false, Color.Gray);
            }
            foreach (Quad3d quad in axisCent)
            {
                quad.RenderGL();
            }
        }
    }
}
