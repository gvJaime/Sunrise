using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using UV_DLP_3D_Printer;
using UV_DLP_3D_Printer._3DEngine;

namespace Engine3D
{
    public class Quad3d
    {
        public Point3d [] points;
        protected C2DImage img;
        public String imgname;
        public Color col;
        public bool visible;
        public Quad3d()
        {
            points = new Point3d[4];
            for (int i = 0; i < 4; i++)
                points[i] = new Point3d(0, 0, 0);
            img = null;
            imgname = null;
        }

        public Quad3d(string imname) : this()
        {
            imgname = imname;
        }

        public void SetPoint(int idx, float x, float y, float z)
        {
            points[idx].x = x;
            points[idx].y = y;
            points[idx].z = z;
        }

        public void RenderGL()
        {
            if ((img == null) && (imgname != null))
            {
                img = UVDLPApp.Instance().m_2d_graphics.GetImage(imgname);
            }
            if (img != null)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, img.tex);
                //GL.Begin(BeginMode.Quads);
                GL.Begin(PrimitiveType.Quads);

                GL.Color3(Color.White);
                GL.TexCoord2(img.x2, img.y1);
                GL.Vertex3(points[0].x, points[0].y, points[0].z);
                GL.TexCoord2(img.x1, img.y1);
                GL.Vertex3(points[1].x, points[1].y, points[1].z);
                GL.TexCoord2(img.x1, img.y2);
                GL.Vertex3(points[2].x, points[2].y, points[2].z);
                GL.TexCoord2(img.x2, img.y2);
                GL.Vertex3(points[3].x, points[3].y, points[3].z);
                GL.End();

                GL.Disable(EnableCap.Texture2D);
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(col);
                for (int i = 0; i < 4; i++)
                    GL.Vertex3(points[i].x, points[i].y, points[i].z);
                GL.End();
            }
        }

        public static List<Quad3d> CreateQuadBox(float x1, float x2, float y1, float y2, float z1, float z2, string image = null)
        {
            Quad3d q;
            List<Quad3d> qlist = new List<Quad3d>();

            // right wall
            q = new Quad3d(image);
            q.SetPoint(0, x2, y2, z2);
            q.SetPoint(1, x2, y1, z2);
            q.SetPoint(2, x2, y1, z1);
            q.SetPoint(3, x2, y2, z1);
            qlist.Add(q);

            // left wall
            q = new Quad3d(image);
            q.SetPoint(0, x1, y1, z2);
            q.SetPoint(1, x1, y2, z2);
            q.SetPoint(2, x1, y2, z1);
            q.SetPoint(3, x1, y1, z1);
            qlist.Add(q);

            // front wall
            q = new Quad3d(image);
            q.SetPoint(0, x2, y1, z2);
            q.SetPoint(1, x1, y1, z2);
            q.SetPoint(2, x1, y1, z1);
            q.SetPoint(3, x2, y1, z1);
            qlist.Add(q);

            // back wall
            q = new Quad3d(image);
            q.SetPoint(0, x1, y2, z2);
            q.SetPoint(1, x2, y2, z2);
            q.SetPoint(2, x2, y2, z1);
            q.SetPoint(3, x1, y2, z1);
            qlist.Add(q);

            // top wall
            q = new Quad3d(image);
            q.SetPoint(0, x2, y2, z2);
            q.SetPoint(1, x1, y2, z2);
            q.SetPoint(2, x1, y1, z2);
            q.SetPoint(3, x2, y1, z2);
            qlist.Add(q);

            // bottom wall
            q = new Quad3d(image);
            q.SetPoint(0, x2, y1, z1);
            q.SetPoint(1, x1, y1, z1);
            q.SetPoint(2, x1, y2, z1);
            q.SetPoint(3, x2, y2, z1);
            qlist.Add(q);

            return qlist;
        }
    }    
}
