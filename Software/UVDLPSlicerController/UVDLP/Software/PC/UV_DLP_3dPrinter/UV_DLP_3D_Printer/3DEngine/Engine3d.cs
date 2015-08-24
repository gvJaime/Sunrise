using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using Engine3D;
using UV_DLP_3D_Printer;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using UV_DLP_3D_Printer._3DEngine;
namespace Engine3D
{
    public delegate void ModelAdded(Object3d model);
    public delegate void ModelRemoved(Object3d model);

    public class Engine3d
    {
        List<PolyLine3d> m_lines;
        public List<Object3d> m_objects;
        public List<Quad3d> m_walls;
        protected AxisIndicator m_axisInd;
        public event ModelAdded ModelAddedEvent;
        public event ModelRemoved ModelRemovedEvent;
        public bool m_alpha;

        public Engine3d() 
        {
            m_lines = new List<PolyLine3d>();
            m_objects = new List<Object3d>();
            m_alpha = false;
            m_axisInd = new AxisIndicator();
            m_axisInd.Create(14, 0.5f, 3, 5, 1.2f);
            //AddGrid(); // grid actually was created twice. -SHS
        }
        public void UpdateLists() 
        {
            foreach (Object3d obj in m_objects) 
            {
                obj.InvalidateList();
            }
        }
        /// <summary>
        /// Find a globally unique name for this 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetUniqueName(string name) 
        {
            string unname = name;
            int c = 0;
            while (IsUniqueName(unname) == false) 
            {
                unname = name + "_" + c.ToString();
                c++;
            }
            return unname;
        }
        public bool IsUniqueName(string name) 
        {
            bool ret = true;
            foreach (Object3d obj in m_objects)
            {
                if (obj.Name.Equals(name))
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }
        /// <summary>
        /// Iterate through all models in scene, along with all supports
        /// calculate the volume
        /// </summary>
        /// <returns></returns>
        public double CalcSceneVolume() 
        {
            double volume = 0.0d;
            foreach (Object3d obj in m_objects) 
            {
                volume += obj.CalculateVolume();
            }
            return volume;
        }
        /// <summary>
        /// This function calculates the Z scene extents.
        /// It will always incude z=0 to the max object z extent
        /// </summary>
        /// <returns></returns>
        public MinMax CalcSceneExtents() 
        {
            MinMax mm = new MinMax();
            mm.m_min = 0.0f;
            try
            {
                int c = 0;
                foreach (Object3d obj in m_objects)
                {
                    //obj.CalcMinMaxes();
                    obj.FindMinMax();
                    if (c == 0) //first one
                    {                        
                        mm.m_min = obj.m_min.z;
                        mm.m_max = obj.m_max.z;
                    }
                    if (obj.m_min.z < mm.m_min)
                        mm.m_min = obj.m_min.z;

                    if (obj.m_max.z > mm.m_max)
                        mm.m_max = obj.m_max.z;
                    c++;
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return mm;
        }
        public void UpdateGrid()
        {
            m_lines = new List<PolyLine3d>();
            AddGrid();
            AddPlatCube();
            AddPlatWalls();
        }
        public void AddGridLine(float x1, float y1, float x2, float y2, Color col)
        {
            AddLine(new PolyLine3d(new Point3d(x1, y1, 0), new Point3d(x2, y2, 0), col));
        }

        public void AddGrid() 
        {
            double xsz = UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            double ysz = UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            double hxsz = xsz / 2.0d;
            double hysz = ysz / 2.0d;

            for (double x = -hxsz; x < hxsz; x += 10)
            {
                AddLine(new PolyLine3d(new Point3d((float)x, (float)-hysz, 0), new Point3d((float)x, (float)hysz, 0), Color.Blue));
            }
            for (double y = -hysz; y < hysz; y += 10)
            {
                AddLine(new PolyLine3d(new Point3d((float)-hxsz, (float)y, 0), new Point3d((float)hxsz, (float)y, 0), Color.Blue));
            }
            /*
            for (float y = -50; y < 51; y += 10)
            {
                AddLine(new PolyLine3d(new Point3d(0, 0, -10), new Point3d(0, 0, 10), Color.Blue));
            }
            */
        }
        public void AddGridOld() 
        {
            for (float x = -50; x < 51; x += 10)
            {
                AddGridLine(x, -50, x, 50, Color.Blue);
            }
            for (float y = -50; y < 51; y += 10)
            {
                AddGridLine(-50, y, 50, y, Color.Blue);
            }
            AddLine(new PolyLine3d(new Point3d(0, 0, -10), new Point3d(0, 0, 10), Color.Blue));

            // add XY arrows
            AddGridLine(50, 0, 58, 0, Color.Blue);
            AddGridLine(58, 0, 55, 3, Color.Blue);
            AddGridLine(58, 0, 55, -3, Color.Blue);
            AddGridLine(0, 50, 0, 58, Color.Blue);
            AddGridLine(0, 58, 3, 55, Color.Blue);
            AddGridLine(0, 58, -3, 55, Color.Blue);
            AddGridLine(60, 2, 66, -2, Color.Red);
            AddGridLine(60, -2, 66, 2, Color.Red);
            AddGridLine(0, 60, 0, 63, Color.Red);
            AddGridLine(0, 63, 2, 66, Color.Red);
            AddGridLine(0, 63, -2, 66, Color.Red);
        }
        //This function draws a cube the size of the build platform
        // The X/Y is centered along the 0,0 center point. Z extends from 0 to Z

        public void AddPlatCube() 
        {
            float platX, platY, platZ;
            float X, Y, Z;
            Color cubecol = Color.Gray;
            platX = (float)UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            platY = (float)UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            platZ = (float)UVDLPApp.Instance().m_printerinfo.m_PlatZSize;
            X = platX / 2;
            Y = platY / 2;
            Z = platZ;

            // bottom
            AddLine(new PolyLine3d(new Point3d(-X, Y, 0), new Point3d(X, Y, 0), cubecol));
            AddLine(new PolyLine3d(new Point3d(-X, -Y, 0), new Point3d(X, -Y, 0), cubecol));

            AddLine(new PolyLine3d(new Point3d(-X, -Y, 0), new Point3d(-X, Y, 0), cubecol));
            AddLine(new PolyLine3d(new Point3d( X, -Y, 0), new Point3d( X, Y, 0), cubecol));

            // Top
            AddLine(new PolyLine3d(new Point3d(-X, Y, Z), new Point3d(X, Y, Z), cubecol));
            AddLine(new PolyLine3d(new Point3d(-X, -Y, Z), new Point3d(X, -Y, Z), cubecol));

            AddLine(new PolyLine3d(new Point3d(-X, -Y, Z), new Point3d(-X, Y, Z), cubecol));
            AddLine(new PolyLine3d(new Point3d(X, -Y, Z), new Point3d(X, Y, Z), cubecol));

            // side edges
            AddLine(new PolyLine3d(new Point3d(X, Y, 0), new Point3d(X, Y, Z), cubecol));
            AddLine(new PolyLine3d(new Point3d(X, -Y, 0), new Point3d(X, -Y, Z), cubecol));

            AddLine(new PolyLine3d(new Point3d(-X, Y, 0), new Point3d(-X, Y, Z), cubecol));
            AddLine(new PolyLine3d(new Point3d(-X, -Y, 0), new Point3d(-X, -Y, Z), cubecol));
        }

        void AddPlatWalls()
        {
            float platX, platY, platZ;
            platX = (float)UVDLPApp.Instance().m_printerinfo.m_PlatXSize / 2;
            platY = (float)UVDLPApp.Instance().m_printerinfo.m_PlatYSize / 2;
            platZ = (float)UVDLPApp.Instance().m_printerinfo.m_PlatZSize;
            m_walls = Quad3d.CreateQuadBox(-platX, platX, -platY, platY, 0, platZ, "WarnWall"); 
        }

        public void RemoveAllObjects() 
        {
            m_objects = new List<Object3d>();

        }
        public void AddObject(Object3d obj) 
        {
            m_objects.Add(obj); 
            if (ModelAddedEvent != null)
            {
                ModelAddedEvent(obj);
            }            
        }
        public void RemoveObject(Object3d obj) 
        {
            m_objects.Remove(obj);
            if (ModelRemovedEvent != null)
            {
                ModelRemovedEvent(obj);
            }                 
        }
        public void RemoveObject(Object3d obj, bool sendevents)
        {
            m_objects.Remove(obj);
            if (sendevents == true)
            {
                if (ModelRemovedEvent != null)
                {
                    ModelRemovedEvent(obj);
                }
            }
        }
        public void AddLine(PolyLine3d ply) { m_lines.Add(ply); }
        public void RemoveAllLines() 
        {
            m_lines = new List<PolyLine3d>();
        }

        void UpdateWallVisibility()
        {
            float platX, platY, platZ;
            float X, Y, Z;
            platX = (float)UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            platY = (float)UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            platZ = (float)UVDLPApp.Instance().m_printerinfo.m_PlatZSize;
            X = platX / 2;
            Y = platY / 2;
            Z = platZ;
            foreach (Quad3d q in m_walls)
                q.visible = false;
            foreach (Object3d obj in m_objects)
            {
                if (obj.m_max.x > X)
                    m_walls[0].visible = true;
                if (obj.m_min.x < -X)
                    m_walls[1].visible = true;
                if (obj.m_min.y < -Y)
                    m_walls[2].visible = true;
                if (obj.m_max.y > Y)
                    m_walls[3].visible = true;
                if (obj.m_max.z > Z)
                    m_walls[4].visible = true;
                if (obj.m_min.z < -0.1)
                    m_walls[5].visible = true;
            }
        }

        public void RenderGL(bool alpha) 
        {
            UpdateWallVisibility();
            try
            {
                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light0);
                GL.Disable(EnableCap.LineSmooth);
                foreach (Object3d obj in m_objects)
                {
                    Color objColor = Color.Gray;
                    if (UVDLPApp.Instance().m_appconfig.m_showShaded)
                    {
                        if (UVDLPApp.Instance().SelectedObject == obj)
                            objColor = Color.DarkGreen;
                        else if (obj.m_inSelectedList)
                            objColor = Color.DarkKhaki;
                    }
                    if (UVDLPApp.Instance().SelectedObject == obj) {
                        obj.RenderGL(alpha, true, UVDLPApp.Instance().m_appconfig.m_showOutline, objColor);
                    }
                    else 
                    {
                        obj.RenderGL(alpha, false, UVDLPApp.Instance().m_appconfig.m_showOutline, objColor);
                    }
                }

                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.Light0);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1);
                foreach (PolyLine3d ply in m_lines)
                {
                    ply.RenderGL();
                }
                // render selection bounding box
                if (UVDLPApp.Instance().m_appconfig.m_showBoundingBox && (UVDLPApp.Instance().SelectedObjectList != null))
                {
                    GL.LineWidth(2);
                    Color clr = Color.Red;
                    foreach (Object3d obj in UVDLPApp.Instance().SelectedObjectList)
                    {
                        obj.RenderBoundingBox(clr);
                        /*
                        foreach (PolyLine3d ply in obj.m_boundingBox)
                        {
                            ply.m_color = clr;
                            ply.RenderGL();
                        }
                         */ 
                        clr = Color.Orange;
                    }
                }

                // render walls if object is out of platform
                // GL.CullFace(CullFaceMode.Back); // specify culling backfaces               
                GL.CullFace(CullFaceMode.Front); // specify culling backfaces
                foreach (Quad3d q in m_walls)
                {
                    if (q.visible)
                        q.RenderGL();
                }
                GL.CullFace(CullFaceMode.Back); // specify culling backfaces
                foreach (Quad3d q in m_walls)
                {
                    if (q.visible)
                        q.RenderGL();
                }
                
                if (RTUtils.m_selplane != null) 
                {
                 //   RTUtils.m_selplane.RenderGL(true, false, false, Color.Cornsilk);
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
        }

        public void RenderGL()
        {
            RenderGL(m_alpha);
        }

        public void RenderAxisIndicator()
        {
            m_axisInd.RenderGL();
        }
        /*
         This function takes the specified vector and intersects all objects
         * in the scene, it will return the polygon? or point that intersects first
         * We can expand this to return list of all intersections, for the initial
         * purposes of support generation, this is used to go from z=0 to z=platmaxz
         */
        public void RayCast(Point3d pstart, Point3d pend) 
        {
        
        }
        public Object3d Find(string name) 
        {
            foreach (Object3d obj in m_objects) 
            {
                if (obj.Name.Equals(name)) 
                {
                    return obj;
                }
            }
            return null;
        }
        /*
         * This function try to rearranges all objects on the build platform so they 
         * do not touch each other
         */
        public void RearrangeObjects()
        {
            BinPacker bp = new BinPacker();
            List<BinPacker.BinRect> rects = new List<BinPacker.BinRect>();
            foreach (Object3d obj in m_objects)
            {
                if (obj.tag != Object3d.OBJ_NORMAL)
                    continue;
                float w = obj.m_max.x - obj.m_min.x;
                float h = obj.m_max.y - obj.m_min.y;
                BinPacker.BinRect rc = new BinPacker.BinRect(w, h, obj);
                rects.Add(rc);
            }
            
            float pw = (float)UVDLPApp.Instance().m_printerinfo.m_PlatXSize;
            float ph = (float)UVDLPApp.Instance().m_printerinfo.m_PlatYSize;
            bp.Pack(rects, (int)pw, (int)ph, true);

            // find pack size
            int maxw = 0;
            int maxh = 0;
            foreach (BinPacker.BinRect rc in rects)
            {
                if (rc.packed && ((rc.x + rc.w) > maxw))
                    maxw = rc.x + rc.w;
                if (rc.packed && ((rc.y + rc.h) > maxh))
                    maxh = rc.y + rc.h;
            }

            // find offsets to center all objects
            float offsx = -(float)maxw / 2f + 0.5f;
            float offsy = -(float)maxh / 2f + 0.5f;

            // move all objects to new positions
            foreach (BinPacker.BinRect rc in rects)
            {
                Object3d obj = (Object3d)rc.obj;
                if (rc.packed)
                {
                    if (rc.rotated)
                        obj.Rotate(0, 0, 1.570796326f);
                    float dx = rc.x - obj.m_min.x + offsx;
                    float dy = rc.y - obj.m_min.y + offsy;
                    obj.Translate(dx, dy, 0, true);
                }
                else
                {
                    // object could not fit, place outside platform
                    obj.Translate(pw / 2f - obj.m_min.x, 0, 0, true);
                }
            }
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw,"Redraw new arrangement");

        }
    }
}
