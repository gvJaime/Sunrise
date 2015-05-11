using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Engine3D;
using UV_DLP_3D_Printer;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;

namespace Engine3D
{
    public class Object3d
    {
        //public delegate void ObjectEvent(); // this indicates something happened to this object

        public List<Point3d> m_lstpoints; // list of 3d points in object
        public List<Polygon> m_lstpolys;// list of polygons
        public List<Object3d> m_supports; // a list of support objects attached to this one
        public Object3d m_parent = null;

        public string m_name; // just the filename
        public string m_fullname; // full path with filename
        private bool m_visible;
        public Point3d m_min, m_max;
        public Point3d m_center;
        public int m_wireframe = 0; // wireframe thickness 0 = no wireframe
        public float m_radius;
        public Material material;// = new Material();
        public int tag = -1; // acting as an object ID
        public const int OBJ_NORMAL        =0; // a regular old object
        public const int OBJ_SUPPORT = 1; // a generated support
        public const int OBJ_GROUND = 2; // ground plane usewd for hit-testing
        public const int OBJ_SUPPORT_BASE = 3; // base support plate
        public const int OBJ_SEL_PLANE = 4; // camera-facing selection plane for selected object


        public bool m_inSelectedList = false;
        protected int m_listid; // gl call list id 
        private double m_volume = -1;


        public double Volume
        {
            get
            {
                if (m_volume < 0)
                    m_volume = CalculateVolume();
                return m_volume;
            }
        }

        public Object3d() 
        {
            Init();
        }
        /// <summary>
        /// Copy copies the supports as well
        /// </summary>
        /// <returns></returns>
        public Object3d Copy() 
        {
            //copy this object
            Object3d cpy = Clone();
            //copy the support structures
            foreach (Object3d obj in this.m_supports) 
            {
                if (obj is Support)
                {
                    Support newsup = ((Support)obj).MakeCopy();
                    cpy.AddSupport(newsup);
                }
            }
            return cpy;
        }
        public virtual Object3d Clone() 
        {
            Object3d obj = new Object3d();
            try
            {
                obj.m_name = UVDLPApp.Instance().Engine3D.GetUniqueName( this.m_name); // need to find unique name
                obj.m_fullname = this.m_fullname;
                obj.tag = this.tag;

                foreach (Polygon ply in m_lstpolys)
                {
                    Polygon pl2 = new Polygon();
                    pl2.m_color = ply.m_color;
                    pl2.m_points = new Point3d[3];
                    obj.m_lstpolys.Add(pl2);
                    pl2.m_points[0] = new Point3d(ply.m_points[0]);
                    pl2.m_points[1] = new Point3d(ply.m_points[1]);
                    pl2.m_points[2] = new Point3d(ply.m_points[2]);
                }
                foreach (Polygon ply in obj.m_lstpolys) 
                {
                    foreach (Point3d pnt in ply.m_points) 
                    {
                        obj.m_lstpoints.Add(pnt); // a fair bit of overlap, but whatever...
                    }
                }
                obj.Update();
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
            }
            return obj;
        }

        public void AddSupport(Object3d s)
        {
            if (s == null)
                return;
            if (s.m_parent != null) 
                s.m_parent.RemoveSupport(s);
            s.m_parent = this;
            if (m_supports.IndexOf(s) < 0)
                m_supports.Add(s);
        }

        public void RemoveSupport(Object3d s)
        {
            if (s == null)
                return;
            while (m_supports.IndexOf(s) >= 0)
                m_supports.Remove(s);
            s.m_parent = null;
        }

        public SupportBase GetSupportBase()
        {
            foreach (Object3d obj in m_supports)
            {
                if (obj is SupportBase)
                    return (SupportBase)obj;
            }
            return null;
        }


        public void InvalidateList() 
        {
            m_listid = -1;
        }
        public void Init() 
        {
            m_lstpolys = new List<Polygon>();
            m_lstpoints = new List<Point3d>();
            m_supports = new List<Object3d>();
            m_center = new Point3d();
            m_name = "Model";
            m_fullname = "Model";
            m_min = new Point3d();
            m_max = new Point3d();
            m_visible = true;
            m_radius = 0.0f;
            material = new Material();
            tag = Object3d.OBJ_NORMAL;
            m_listid = -1;
            m_volume = -1;
        }
        public string Name 
        { 
            get { return m_name; }
            set { m_name = value; }
        }

        public int NumPolys { get { return m_lstpolys.Count; } }
        public int NumPoints { get { return m_lstpoints.Count; } }
        public bool Visible 
        {
            get { return m_visible; }
            set {  m_visible = value; }
        }

        public void SetColor(Color color)
        {
            foreach (Polygon p in m_lstpolys)
            {
                p.m_color = color;
            }
               
        }
        private void CalcMinMaxes() 
        {
            foreach (Polygon p in m_lstpolys) 
            {
                p.CalcMinMax();
            }
        }

        void Rotate(Matrix3D rotmat)
        {
            for (int c = 0; c < m_lstpoints.Count; c++)
            {
                Point3d p = (Point3d)m_lstpoints[c];
                Point3d p1 = rotmat.Transform(p);
                p.x = p1.x;
                p.y = p1.y;
                p.z = p1.z;
            }
        }

        public void Rotate(float x, float y, float z) 
        {
            Point3d center = CalcCenter();
            float cz = center.z;
            if ((x == 0) && (y == 0))
                cz = 0;
            // else - i think we need to delete all supports -SHS
            Translate((float)-center.x, (float)-center.y, (float)-cz);

            Matrix3D rotmat = new Matrix3D();
            rotmat.Identity();
            rotmat.Rotate(x, y, z);
            Rotate(rotmat);
            if ((x == 0) && (y == 0))
            {
                foreach (Object3d sup in m_supports)
                    sup.Rotate(rotmat);
            }
            Translate((float)center.x, (float)center.y, (float)cz);
        }
        /// <summary>
        /// This scales from the center of the object
        /// </summary>
        /// <param name="sfx"></param>
        /// <param name="sfy"></param>
        /// <param name="sfz"></param>
        public void Scale(float sfx,float sfy, float sfz)
        {
            Point3d center = CalcCenter();
            Translate((float)-center.x, (float)-center.y, 0);
            foreach (Point3d p in m_lstpoints)
            {
                p.x *= sfx;
                p.y *= sfy;
                p.z *= sfz;
            }
            Translate((float)center.x, (float)center.y, 0);
            m_volume = -1;
        }
        public void Scale(float sf) 
        {
            Point3d center = CalcCenter();
            Translate((float)-center.x, (float)-center.y, 0);
            foreach (Point3d p in m_lstpoints) 
            {
                p.x *= sf;
                p.y *= sf;
                p.z *= sf;
            }
            Translate((float)center.x, (float)center.y, 0);
        }
        /// <summary>
        /// this function is called after an object is loaded to center the object
        /// at x/y 0,0 and to place the bottom of the object touching the platform at z position 0
        /// </summary>
        public void CenterOnPlatform() 
        {
            Point3d center = CalcCenter();
            FindMinMax();
            float zlev = (float)m_min.z;
            float epsilon = 0.0f;// .05f; // add in a the level of 1 slice
            float zmove = -zlev - epsilon;
            Translate((float)-center.x, (float)-center.y, (float)zmove);                   
        }
        public void FlipWinding() 
        {
            foreach (Polygon p in m_lstpolys) 
            {
                p.FlipWinding();
            }
        }
        protected static int GetListID() 
        {
            return GL.GenLists(1); 
        }

        /// <summary>
        /// Return a list of polygon next to a hole
        /// operates in n^2 time
        /// </summary>
        /// <returns></returns>
        public List<Polygon> FindHoles() 
        {
            Update();
            List<Polygon> lstholes = new List<Polygon>();
            Dictionary<Polygon, int> map = new Dictionary<Polygon, int>();
            try
            {
                foreach (Polygon ply in m_lstpolys)
                {
                    foreach (Polygon ply2 in m_lstpolys)
                    {
                        if (ply != ply2) // if it's not the same poly
                        {
                            if (ply.SpheresIntersect(ply2)) // and the 2 bounding spheres of the polygon intersect
                            {
                                if (ply.SharesEdge(ply2)) // and it shares an edge
                                {
                                    if (map.ContainsKey(ply)) // check to see if it's in the map already
                                    {
                                        map[ply]++; // increment it
                                    }
                                    else
                                    {
                                        map.Add(ply, 1); // or add it
                                    }
                                }
                            }
                        }
                    }
                }
                // generate some results to pass back
                foreach (Polygon ply in m_lstpolys)
                {
                    if (map.ContainsKey(ply))
                    {
                        if (map[ply] < 3)
                        {
                            lstholes.Add(ply);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return lstholes;
        }
        
        public virtual void RenderGL(bool showalpha, bool selected, bool renderOutline, Color renderColor)
        {
            if (m_listid == -1)
            {
                m_listid = GetListID();
                GL.NewList(m_listid, ListMode.CompileAndExecute);
                if (m_inSelectedList && renderOutline)
                {
                    GL.Enable(EnableCap.StencilTest);
                    GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
                    GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
                    GL.StencilMask(0xFF);
                    GL.Clear(ClearBufferMask.StencilBufferBit);
                }
                foreach (Polygon poly in m_lstpolys)
                {
                    poly.RenderGL(this.m_wireframe, showalpha, renderColor != Color.Gray ? renderColor : poly.m_color);
                }
                if (m_inSelectedList && renderOutline)
                {
                    GL.Disable(EnableCap.Lighting);
                    GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
                    GL.StencilMask(0);
                    GL.DepthMask(true);
                    Color selcol = Color.Orange;
                    if (selected)
                        selcol = Color.Red;
                    foreach (Polygon poly in m_lstpolys)
                    {
                        poly.RenderGL(4, showalpha, selcol);
                    }
                    GL.Disable(EnableCap.StencilTest);
                    GL.Enable(EnableCap.Lighting);
                }
                GL.EndList();
            }
            else
            {
                GL.CallList(m_listid);
            }
        }
        
        public double CalculateVolume() 
        {
            double vol = 0.0;
            
            try
            {
                //save the center                
                Point3d center = CalcCenter();
                //move to origin
                //Translate((float)-center.x, (float)-center.y, (float)-center.z);                
                STranslate((float)-center.x, (float)-center.y, (float)-center.z);
                foreach (Polygon poly in m_lstpolys) 
                {
                    Point3d p1 = poly.m_points[0];
                    Point3d p2 = poly.m_points[1];
                    Point3d p3 = poly.m_points[2];
                    double v321 = p3.x * p2.y * p1.z;
                    double v231 = p2.x * p3.y * p1.z;
                    double v312 = p3.x * p1.y * p2.z;
                    double v132 = p1.x * p3.y * p2.z;
                    double v213 = p2.x * p1.y * p3.z;
                    double v123 = p1.x * p2.y * p3.z;
                    vol += (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
                }
                //move it back
                STranslate((float)center.x, (float)center.y, (float)center.z);
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
            return vol;
        }


        private Point3d AddUniqueVert(Point3d pnt) 
        {
            foreach (Point3d p in m_lstpoints) 
            {
                if (pnt.Equals(p)) // if it's already in the list, return it
                    return p;
            }
            m_lstpoints.Add(pnt); // otherwise add it to the list
            return pnt;
        }
        private void LoadDXFPolyPoints(out Point3d[] pnts, StreamReader sr) 
        {
            ArrayList lst = new ArrayList();
            bool done = false;
            Point3d pnt = null;
            while (!done) 
            {
                string line = sr.ReadLine();
                line = line.Trim();
                
                if (line == "10" || line == "11" || line == "12" || line == "13")
                {
                    pnt = new Point3d();
                    lst.Add(pnt);
                    pnt.x = float.Parse(sr.ReadLine());
                }
                if (line == "20" || line == "21" || line == "22" || line == "23") 
                {
                    pnt.y = float.Parse(sr.ReadLine());
                }
                if (line == "30" || line == "31" || line == "32" || line == "33") 
                {
                    pnt.z = float.Parse(sr.ReadLine());
                }
                if (line == "62") done = true;
            }
            pnts = new Point3d[lst.Count];
            int idx = 0;
            foreach (Point3d p in lst) 
            {
                pnts[idx++] = p;
            }
        
        }
        public bool GenerateFromBitmap(string file, Vector3d f) 
        {
            try
            {
                m_name = Path.GetFileNameWithoutExtension(file);
                Bitmap bm = new Bitmap(file);
                // add 3d points
                for (int y = 0; y < bm.Height; y++) 
                {
                    for (int x = 0; x < bm.Width; x++) 
                    {
                        Color clr = bm.GetPixel(x, y);
                        Point3d pnt = new Point3d();
                        pnt.x = f.x * ((float)x);
                        pnt.y = f.y * ((float)y);
                        pnt.z = f.z * ((float)clr.R);
                        m_lstpoints.Add(pnt);
                    }
                }
                // now generate polys
                for (int y = 0; y < bm.Height  ; y++)
                {
                    for (int x = 0; x < bm.Width ; x++)
                    {
                        if (y == (bm.Height - 1)) continue;
                        if (x == (bm.Width - 1)) continue;
                        Polygon ply = new Polygon();
                        ply.m_points = new Point3d[3];
                        int idx1 = (y * bm.Width) + x;
                        int idx2 = (y * bm.Width) + x + 1;
                        int idx3 = (y * bm.Width) + x + bm.Width ;
                        ply.m_points[0] = (Point3d)m_lstpoints[idx1];
                        ply.m_points[1] = (Point3d)m_lstpoints[idx2];
                        ply.m_points[2] = (Point3d)m_lstpoints[idx3];
                        ply.CalcCenter();
                        ply.CalcNormal();
                        m_lstpolys.Add(ply);
                        
                       
                        Polygon ply2 = new Polygon();
                        ply2.m_points = new Point3d[3];
                        idx1 = (y * bm.Width) + x + 1;
                        idx2 = (y * bm.Width) + x + bm.Width + 1;
                        idx3 = (y * bm.Width) + x + bm.Width;
                        ply2.m_points[0] = (Point3d)m_lstpoints[idx1];
                        ply2.m_points[1] = (Point3d)m_lstpoints[idx2];
                        ply2.m_points[2] = (Point3d)m_lstpoints[idx3];

                        ply2.CalcCenter();
                        ply2.CalcNormal();
                        m_lstpolys.Add(ply2);
                         
                    }
                }
                Update();
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }
        /// <summary>
        /// Iterates through all the points, looking for the min / max points of the models
        /// </summary>
        public void FindMinMax()         
        {
            Point3d first = (Point3d)this.m_lstpoints[0];
            m_min.Set(first.x, first.y, first.z);
            m_max.Set(first.x, first.y, first.z);
            foreach (Point3d p in this.m_lstpoints)             
            {
                if (p.x < m_min.x)
                    m_min.x = p.x;
                if (p.y < m_min.y)
                    m_min.y = p.y;
                if (p.z < m_min.z)
                    m_min.z = p.z;

                if (p.x > m_max.x)
                    m_max.x = p.x;
                if (p.y > m_max.y)
                    m_max.y = p.y;
                if (p.z > m_max.z)
                    m_max.z = p.z;
            }
            CalcMinMaxes();
        }
        /*
         This is called after calccenter
         * it iterates through all points and finds the one that is farthest away from the center point
         */
        public void CalcRadius() 
        {
            float maxdist = 0.0f;
            float td = 0.0f;
            foreach (Point3d p in m_lstpoints)
            {
                td = (p.x - m_center.x) * (p.x - m_center.x);
                td += (p.y - m_center.y) * (p.y - m_center.y);
                td += (p.z - m_center.z) * (p.z - m_center.z);
                if (td >= maxdist)
                    maxdist = td;
            }
			m_radius = (float)Math.Sqrt(maxdist);
        }

        public Point3d CalcCenter() 
        {
            Point3d center = new Point3d();
            center.Set(0, 0, 0);
            foreach (Point3d p in m_lstpoints) 
            {
                center.x += p.x;
                center.y += p.y;
                center.z += p.z;
            }

            center.x /= m_lstpoints.Count;
            center.y /= m_lstpoints.Count;
            center.z /= m_lstpoints.Count;

            m_center.Set(center.x, center.y, center.z);
            return center;
        }

        /*
         Test function to mark polygons facing doward a different color         
         */
        public void MarkPolysDown(double angle) 
        {
            Vector3d upvec = new Vector3d();
            double inc = 1.0 / 90.0;
            angle = -(1 - (angle * inc));
            upvec.Set(new Point3d(0,0,1));
            foreach (Polygon p in this.m_lstpolys) 
            {
                p.CalcNormal();
                double d = p.m_normal.Dot(upvec);
                if (d <= angle)  // facing down
                {
                    p.tag = Polygon.TAG_MARKDOWN;
                }
                else 
                {
                    p.tag = Polygon.TAG_REGULAR;
                }
            }
            InvalidateList();
        }
        public void ClearPolyTags() 
        {
            foreach (Polygon p in this.m_lstpolys)
            {
                p.tag = Polygon.TAG_REGULAR;
                p.m_color = p.m_colorsource;// Color.Gray;
            }
            InvalidateList();
        }

        /*
         This function initializes initial positions 
         * and calculates the center, radius
         */
        public void Update() 
        {
            try
            {
                CalcCenter();
                CalcRadius();
                FindMinMax();
                foreach (Polygon p in m_lstpolys)
                {
                    p.Update();
                }
                m_listid = -1; // invalidate the list id
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }
        /*
        public void UpdateMove(float x, float y , float z)
        {
            try
            {
                m_center.Translate(x, y, z);
                m_min.Translate(x, y, z);
                m_max.Translate(x, y, z);

                //UpdateBoundingBox();
                foreach (Polygon p in m_lstpolys)
                {
                    p.UpdateMove( x,  y ,  z);
                }
                m_listid = -1; // invalidate the list id
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                DebugLogger.Instance().LogError(ex.StackTrace);
            }
        }
        */
        /*This function adds the objects points and polygons to this one*/
        public void Add(Object3d obj) 
        {
            foreach (Point3d p in obj.m_lstpoints)
            {
                m_lstpoints.Add(p);
            }
            foreach (Polygon ply in obj.m_lstpolys) 
            {
                m_lstpolys.Add(ply);
            }
            Update();
        }
        public void RenderBoundingBox(Color color) 
        {
            GL.Begin(PrimitiveType.LineStrip);//.Lines);
            GL.Color3(color);            
            GL.Vertex3(m_min.x, m_min.y, m_min.z);
            GL.Vertex3(m_max.x, m_min.y, m_min.z);
            GL.Vertex3(m_max.x, m_max.y, m_min.z);
            GL.Vertex3(m_min.x, m_max.y, m_min.z);
            GL.Vertex3(m_min.x, m_min.y, m_min.z);
            GL.Vertex3(m_min.x, m_min.y, m_max.z);
            GL.Vertex3(m_max.x, m_min.y, m_max.z);
            GL.Vertex3(m_max.x, m_max.y, m_max.z);
            GL.Vertex3(m_min.x, m_max.y, m_max.z);
            GL.Vertex3(m_min.x, m_min.y, m_max.z);
            GL.Vertex3(m_min.x, m_min.y, m_min.z);
            GL.Vertex3(m_min.x, m_min.y, m_max.z);
            GL.Vertex3(m_max.x, m_min.y, m_min.z);
            GL.Vertex3(m_max.x, m_min.y, m_max.z);
            GL.Vertex3(m_max.x, m_max.y, m_min.z);
            GL.Vertex3(m_max.x, m_max.y, m_max.z);
            GL.Vertex3(m_min.x, m_max.y, m_min.z);
            GL.Vertex3(m_min.x, m_max.y, m_max.z);
            GL.End();            
        }

        public void STranslate(float x, float y, float z)
        {
            foreach (Point3d p in m_lstpoints)
            {
                p.x += x;
                p.y += y;
                p.z += z;
            }
        }
        /*Move the model in object space */
        public virtual void Translate(float x, float y, float z, bool updateUndo = false) 
        {
            foreach (Point3d p in m_lstpoints) 
            {
                p.x += x;
                p.y += y;
                p.z += z;
            }
            // translate connected supports as well
            if ((x != 0) || (y != 0))
            {
                foreach (Object3d sup in m_supports)
                    sup.Translate(x, y, 0);
            }
            if (z != 0)
            {
                foreach (Object3d sup in m_supports)
                    if (sup is Support)
                        ((Support)sup).AddToHeight(z);
            }
            m_center.Translate(x, y, z);
            m_min.Translate(x, y, z);
            m_max.Translate(x, y, z);

            foreach (Polygon p in m_lstpolys)
            {
                p.UpdateMove(x, y, z);
            }
            m_listid = -1; // invalidate the list id
            if (updateUndo)
                UVDLPApp.Instance().m_undoer.SaveTranslation(this, x, y, z);
        }
        public bool LoadSTL(string filename) 
        {
            // only binary stl for now
            bool val = LoadSTL_Binary(filename);

            if (!val)
            {
                Init();// re-init the object
                return LoadSTL_ASCII(filename);
            }
            
            return val;
        }


        /*
         * LoadSTL_Binary
         * This function loads a binary STL file
         * File Format:
            UINT8[80] – Header
            UINT32 – Number of triangles

            foreach triangle
            REAL32[3] – Normal vector
            REAL32[3] – Vertex 1
            REAL32[3] – Vertex 2
            REAL32[3] – Vertex 3
            UINT16 – Attribute byte count
            end         
             */


        public bool SaveSTL_Binary(string filename) 
        {
            try
            {
                BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create));
                byte[] header = new byte[80];
                //fill in the header
                bw.Write(header, 0, 80);
                bw.Write((uint)m_lstpolys.Count);
                foreach (Polygon p in m_lstpolys) 
                {
                    //write the normal
                    bw.Write((float)p.m_normal.x);
                    bw.Write((float)p.m_normal.y);
                    bw.Write((float)p.m_normal.z);
                    foreach (Point3d pnt in p.m_points) 
                    {
                        bw.Write((float)pnt.x);
                        bw.Write((float)pnt.y);
                        bw.Write((float)pnt.z);
                    }
                    bw.Write((ushort)0); // 16 bit attribute
                }
                bw.Close();
                return true;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }

        public bool SaveSTL_Binary(ref MemoryStream ms)
        {
            try
            {
                BinaryWriter bw = new BinaryWriter(ms);
                byte[] header = new byte[80];
                //fill in the header
                bw.Write(header, 0, 80);
                bw.Write((uint)m_lstpolys.Count);
                foreach (Polygon p in m_lstpolys)
                {
                    //write the normal
                    bw.Write((float)p.m_normal.x);
                    bw.Write((float)p.m_normal.y);
                    bw.Write((float)p.m_normal.z);
                    foreach (Point3d pnt in p.m_points)
                    {
                        bw.Write((float)pnt.x);
                        bw.Write((float)pnt.y);
                        bw.Write((float)pnt.z);
                    }
                    bw.Write((ushort)0); // 16 bit attribute
                }
                //bw.Close();
                return true;
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
        }

        public bool LoadSTL_Binary(MemoryStream stream,string name)
        {
            BinaryReader br = null;
            try
            {
                br = new BinaryReader(stream);
                m_fullname = name;
                m_name = Path.GetFileNameWithoutExtension(name);
                byte[] data = new byte[80];
                data = br.ReadBytes(80); // read the header
                uint numtri = br.ReadUInt32();
                for (uint c = 0; c < numtri; c++)
                {
                    Polygon p = new Polygon();
                    m_lstpolys.Add(p); // add this polygon to the object
                    p.m_normal.Load(br); // load the normal
                    p.m_points = new Point3d[3]; // create storage
                    for (int pc = 0; pc < 3; pc++) //iterate through the points
                    {
                        Point3d pnt = new Point3d();
                        pnt.Load(br);
                        m_lstpoints.Add(pnt);
                        p.m_points[pc] = pnt;
                    }
                    uint attr = br.ReadUInt16(); // not used attribute
                }

                Update(); // initial positions please...
                br.Close();
                return true;
            }
            catch (Exception)
            {
                if (br != null)
                    br.Close();
                return false;
            }

        }
        
        public bool LoadSTL_Binary(string filename) 
        {
            BinaryReader br = null;
            try
            {
                br = new BinaryReader(File.Open(filename, FileMode.Open));
                m_fullname = filename;
                m_name = Path.GetFileNameWithoutExtension(filename);
                byte[] data = new byte[80];
                data = br.ReadBytes(80); // read the header
                uint numtri = br.ReadUInt32();
                for (uint c = 0; c < numtri; c++) 
                {
                    Polygon p = new Polygon();
                    m_lstpolys.Add(p); // add this polygon to the object
                    p.m_normal.Load(br); // load the normal
                    p.m_points = new Point3d[3]; // create storage
                    for (int pc = 0; pc < 3; pc++) //iterate through the points
                    {                        
                        Point3d pnt = new Point3d();
                        pnt.Load(br);
                        m_lstpoints.Add(pnt);
                        p.m_points[pc] = pnt;
                    }
                    uint attr = br.ReadUInt16(); // attribute COULD be used for color 
                    /*
                    The VisCAM and SolidView software packages use the two "attribute byte count" bytes at the end of every triangle to store a 15-bit RGB color:

                        bit 0 to 4 are the intensity level for blue (0 to 31),
                        bits 5 to 9 are the intensity level for green (0 to 31),
                        bits 10 to 14 are the intensity level for red (0 to 31),
                        bit 15 is 1 if the color is valid, or 0 if the color is not valid (as with normal STL files).
                     
                     */
                    //BBBBBGGGGGRRRRRV
                    //VRRRRRGGGGGBBBBB
                    byte R, G, B, used;
                    B = (byte)((attr & 0x001f)<<3);
                    G = (byte)((attr>>5 & 0x001f)<<3);
                    R = (byte)((attr>>10 & 0x001f)<<3);

                    used = (byte)(attr >> 15 & 0x0001);
                    if (used != 0) 
                    {
                        p.m_colorsource = Color.FromArgb(255, R, G, B);
                        p.m_color = p.m_colorsource;
                    }

                }
                
                Update(); // initial positions please...
                br.Close();
                return true;
            }
            catch (Exception) 
            {
                if(br!=null)
                    br.Close();
                return false;
            }
            
        }
        /// <summary>
        /// This function loads an ascii STL file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool LoadSTL_ASCII(string filename) 
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                m_fullname = filename;
                m_name = Path.GetFileNameWithoutExtension(filename);
                //first line should be "solid <name> " 
                string line = sr.ReadLine();
                string []toks = line.Split(' ');
                if (!toks[0].ToLower().StartsWith("solid"))
                    return false; // does not start with "solid"
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();//facet
                    if (line.ToLower().StartsWith("facet"))
                    {
                        line = sr.ReadLine().Trim();//outerloop
                        if (!line.ToLower().StartsWith("outer loop")) 
                        {
                            return false;
                        }
                        Polygon poly = new Polygon();//create a new polygon                        
                        m_lstpolys.Add(poly); // add it to the object's polygon list
                        poly.m_points = new Point3d[3]; // create the storage for 3 points 
                        
                        for (int idx = 0; idx < 3; idx++)//read the point, will break somehow on bad 4 vertext faces.
                        {
                            poly.m_points[idx] = new Point3d(); // create a new point at the poly's indexed point list
                            m_lstpoints.Add(poly.m_points[idx]); // add this point to the object's list of point

                            char[] delimiters = new char[] { ' ' };
                            line = sr.ReadLine().Trim(); // vertex
                            toks = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                            if (!toks[0].ToLower().Equals("vertex")) 
                            {
                                return false;
                            }
                            poly.m_points[idx].x = (float)float.Parse(toks[1].Trim(), System.Globalization.CultureInfo.InvariantCulture); 
                            poly.m_points[idx].y = (float)float.Parse(toks[2].Trim(), System.Globalization.CultureInfo.InvariantCulture); 
                            poly.m_points[idx].z = (float)float.Parse(toks[3].Trim(), System.Globalization.CultureInfo.InvariantCulture); 
                            //poly.m_points[idx].x = (float)float.Parse(toks[1].Trim(), System.Globalization.NumberStyles.Any);
                            //poly.m_points[idx].y = (float)float.Parse(toks[2].Trim(), System.Globalization.NumberStyles.Any);
                            //poly.m_points[idx].z = (float)float.Parse(toks[3].Trim(), System.Globalization.NumberStyles.Any);                           
                        }

                        line = sr.ReadLine().Trim();//endloop
                        if (!line.Equals("endloop")) 
                        {
                            return false;
                        }
                        line = sr.ReadLine().Trim().ToLower(); // endfacet
                        if (!line.Equals("endfacet"))
                        {
                            return false;
                        }

                    } // endfacet
                    else if (line.ToLower().StartsWith("endsolid"))
                    {

                        Update(); // initial positions please...
                    }
                    else 
                    {
                        DebugLogger.Instance().LogError("Error in LoadSTL ASCII, facet expected");
                    }
                } // end of input stream
                
                
                sr.Close();
            }
            catch (Exception ex ) 
            {
                DebugLogger.Instance().LogError(ex.StackTrace);
                DebugLogger.Instance().LogError(ex.Message);
                return false;
            }
            FindMinMax();
            return true;
        }
        #region 3DSLoader
        // Most of the code for the 3ds loader is in the ThreeDSFile.cs
        // This function is just the entry point for it all
        public bool Load3ds(string filename) 
        {
           
            try
            {
                ThreeDSFile tds = new ThreeDSFile(filename);
                foreach (Object3d obj in tds.ThreeDSModel) 
                {
                    this.Add(obj);
                }
                Update();
                m_fullname = filename;
                m_name = Path.GetFileNameWithoutExtension(filename);
                return true;
            }   
            catch (Exception e) 
            {
                DebugLogger.Instance().LogError(e.Message);
                return false;
            }
        }
        #endregion

        #region OBJ FILE LOADER
        public bool LoadObjFile(string fileName)
﻿      ﻿  {
          try
          {
        ﻿  ﻿  ﻿  if (string.IsNullOrEmpty(fileName))
        ﻿  ﻿  ﻿  {
                     return false;           
        ﻿  ﻿  ﻿  }
        ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  if (!File.Exists(fileName))
        ﻿  ﻿  ﻿  {
                     DebugLogger.Instance().LogError("3ds file could not be found " + fileName);
                     return false;
                     //﻿  ﻿  ﻿  ﻿  throw new ArgumentException("3ds file could not be found", "fileName");
        ﻿  ﻿  ﻿  }
                 this.m_fullname = fileName;
                 this.m_name = Path.GetFileNameWithoutExtension(fileName);
        ﻿  ﻿  ﻿  using (StreamReader sr = File.OpenText(fileName))
        ﻿  ﻿  ﻿  {
        ﻿  ﻿  ﻿  ﻿  
              ﻿  ﻿  ﻿  int curLineNo = 0;﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
                   ﻿  ﻿  ﻿  ﻿  
              ﻿  ﻿  ﻿  string line = null;
              ﻿  ﻿  ﻿  bool done = false;
                    //ArrayList lclpoints = new ArrayList();
                    ArrayList lclply = new ArrayList();
                    ArrayList lclnrm = new ArrayList();

        ﻿  ﻿  ﻿  ﻿  while ((line = sr.ReadLine()) != null)
        ﻿  ﻿  ﻿  ﻿  {
        ﻿  ﻿  ﻿  ﻿  ﻿  curLineNo++;
        ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  if (done || line.Trim() == string.Empty || line.StartsWith("#"))
        ﻿  ﻿  ﻿  ﻿  ﻿  {
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  continue;
        ﻿  ﻿  ﻿  ﻿  ﻿  }
        ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  switch (parts[0])
        ﻿  ﻿  ﻿  ﻿  ﻿  {
        ﻿  ﻿  ﻿  ﻿  ﻿  case "v": // vertex         ﻿  ﻿  ﻿  ﻿
                        Point3d pnt = new Point3d();
                        float[] v = ParseVector(parts);
                        pnt.x = v[0];
                        pnt.y = v[1];
                        pnt.z = v[2];
              ﻿  ﻿  ﻿  ﻿  ﻿  m_lstpoints.Add(pnt);
                        break;
        ﻿  ﻿  ﻿  ﻿  ﻿  case "vn": // vertex normal
            ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  lclnrm.Add(ParseVector(parts));
            ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  break;
        ﻿  ﻿  ﻿  ﻿  ﻿  //case "g":
        ﻿  ﻿  ﻿  ﻿  //﻿  ﻿  done = true;
        ﻿  ﻿  ﻿  ﻿  //﻿  ﻿  break;
        ﻿  ﻿  ﻿  ﻿  ﻿  case "f":﻿  ﻿  ﻿  ﻿  ﻿  ﻿  // a face              ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  if (parts.Length < 3)
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  {
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  throw new FormatException(string.Format("Face found with less three indices (line {0})", curLineNo));﻿  ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  }
 ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  Polygon ply;
                    int fp1, fp2, fp3;
                    // 4 pointed poly becomes 2 tris
                    // 5 pointed poly becomes 3 tris
                    // 6 pointed poly becomes 4 tris
                    int numpnts = parts.Length - 1; // take off one 
                    int numpoly = numpnts - 2;
                    int partidx = 1;
                    fp1 = ParseFacePart(parts[partidx]) - 1; // point 0 is common to all polygons
                    partidx++;
                    for (int c = 0; c < numpoly; c++) 
                    {
                        ply = new Polygon(); // create a new poly
                        m_lstpolys.Add(ply); // add it to this object
                        ply.m_points = new Point3d[3]; // create point storage
                        //set the points
                        fp2 = ParseFacePart(parts[partidx]) - 1;
                        fp3 = ParseFacePart(parts[partidx+1]) - 1;

                        ply.m_points[0] = (Point3d)m_lstpoints[fp1];
                        ply.m_points[1] = (Point3d)m_lstpoints[fp2];
                        ply.m_points[2] = (Point3d)m_lstpoints[fp3];

                        partidx++;
                        ply.Update(); // update all the info
                    }
                    
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  break;
        ﻿  ﻿  ﻿  ﻿  ﻿  }
        ﻿  ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  ﻿  }
        ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  ﻿  ﻿ // Console.WriteLine("v: {0} n: {1} q:{2}", vectors.Count,normals.Count, quads.Count);
        ﻿  ﻿  ﻿  ﻿  
        ﻿  ﻿  ﻿  }
    ﻿     ﻿  ﻿  ﻿ return true;
          }
          catch (Exception ex) 
          {
              DebugLogger.Instance().LogError(ex.Message);
              return false;
          }
﻿  ﻿  }
﻿  ﻿  
﻿  ﻿  static float[] ParseVector(string[] parts)
﻿  ﻿  {﻿  ﻿  ﻿  
          float []dv = new float[3];
          dv[0] = Single.Parse(parts[1]);
          dv[1] = Single.Parse(parts[2]);
          dv[2] = Single.Parse(parts[3]);

    ﻿  ﻿  ﻿  return dv;
﻿  ﻿  }                     
﻿  ﻿  
﻿  ﻿  ﻿  ﻿  
﻿  ﻿  static int ParseFacePart(string part)
﻿  ﻿  {
          try
          {
        ﻿  ﻿  ﻿  string[] pieces = part.Split('/');
        ﻿  ﻿  ﻿  return int.Parse(pieces[0]); // piece 0 is the vertex index
          }
          catch (Exception ex) 
          {
              DebugLogger.Instance().LogError(ex.Message);
              return -1;
          }
﻿  ﻿  }
#endregion OBJ FILE LOADER﻿
      #region DXFLoader
      public bool LoadDXF(string filename) 
        {
            try
            {
                StreamReader sr = new StreamReader(filename);
                m_fullname = filename;
                m_name = Path.GetFileNameWithoutExtension(filename);
                while (!sr.EndOfStream) 
                {
                    string line = sr.ReadLine();
                    line = line.Trim();
                    if (line.ToUpper() == "3DFACE") 
                    {
                        Polygon poly = new Polygon();//create a new polygon
                        m_lstpolys.Add(poly); // add it to the polygon list
                        Point3d []pnts;
                        LoadDXFPolyPoints(out pnts, sr);
                        poly.m_points = new Point3d[pnts.Length]; // create the storage
                        int idx = 0;
                        foreach(Point3d p in pnts)
                        {
                            poly.m_points[idx++] = AddUniqueVert(p);
                        }
                    }
                }
                sr.Close();
                if (NumPolys > 0)
                {
                    Update();
                    return true;
                }
                else 
                {
                    return false;
                }
                
            }catch( Exception)
            {
                return false;            
            }
        }
      #endregion dxfloader
    }

}
