using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Ionic.Zip;
using UV_DLP_3D_Printer.GUI;

namespace Engine3D
{
    public class ModelLoaderAmf : ModelLoaderType
    {
        class EdgeAmf
        {
            public int v1;
            public int v2;
            public int v12 = -1;
            public Vector3d t1;
            public Vector3d t2;
            public Vector3d t12 = null;
            public bool initialEdge = false;
        }

        class PointAmf
        {
            public Point3d pt;
            public Vector3d normal = null;
            public List<EdgeAmf> edgeList = null;
            public void AddEdge(EdgeAmf edge)
            {
                if (edgeList == null)
                    edgeList = new List<EdgeAmf>();
                edgeList.Add(edge);
            }

            public EdgeAmf FindEdge(int v2)
            {
                if (edgeList == null)
                    return null;
                foreach (EdgeAmf edge in edgeList)
                {
                    if (edge.v2 == v2)
                        return edge;
                }
                return null;
            }
        }

        class TriangleAmf
        {
            public TriangleAmf(int vr1, int vr2, int vr3)
            {
                v1 = vr1;
                v2 = vr2;
                v3 = vr3;
            }
            public int v1;
            public int v2;
            public int v3;
        }

        public enum eUnit
        {
            Millimeter = 0,
            Inch,
            Meter,
            Micron,
            Feet
        }
        XmlDocument m_xdoc;
        Object3d m_curObject;
        eUnit m_unit;
        float m_scaleFactor;
        List<Object3d> m_objList;
        List<PointAmf> m_pointList;
        List<EdgeAmf> m_edgeList;
        List<TriangleAmf> m_flatTrigs;
        String m_filepath;
        int m_nobjects;
        bool m_smoothObj;
        const float Epsilon = 0.0000001f;
        frmAmfSmoothing m_formSmooth;
        int m_smoothLevel;
        
        public XmlNode m_toplevel;
        public int m_version;
        public ModelLoaderAmf()
        {
            m_fileExt = ".amf";
            m_fileDesc = "Additive Manufacturing File Format";
            m_formSmooth = new frmAmfSmoothing();
        }

        public eUnit Unit
        {
            get { return m_unit; }
        }

        public override List<Object3d> LoadModel(string filename)
        {
            m_filepath = filename;
            try
            {
                m_xdoc = new XmlDocument();
                MemoryStream mstream = TryReadZip(filename);
                if (mstream != null) {
                    m_xdoc.Load(mstream);
                }
                else
                {
                    m_xdoc.Load(filename);
                }
                m_toplevel = m_xdoc["amf"];
                m_objList = new List<Object3d>();
                ParseBody(m_toplevel);
                return m_objList;
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
            return null;
        }

        private MemoryStream TryReadZip(string filename)
        {
            try
            {
                ZipFile zpf = ZipFile.Read(filename);
                ZipEntry ze = zpf[Path.GetFileName(filename)];
                MemoryStream mstream = new MemoryStream();
                ze.Extract(mstream);
                mstream.Seek(0, SeekOrigin.Begin);
                return mstream;
            }
            catch (Exception ex) 
            {
                m_lastError = ex.Message;
            }
            return null;
        }

        String GetAttr(XmlNode node, String attr, String defval)
        {
            try
            {
                String res = node.Attributes[attr].Value;
                return res;
            }
            catch (Exception) { }
            return defval;
        }

        void GetUnit(XmlNode node)
        {
            String unitStr = GetAttr(node, "unit", "millimeter");
            switch (unitStr)
            {
                default:
                case "millimeter":
                    m_unit = eUnit.Millimeter;
                    m_scaleFactor = 1.0f;
                    break;

                case "inch":
                    m_unit = eUnit.Inch;
                    m_scaleFactor = 25.4f;
                    break;

                case "meter":
                    m_unit = eUnit.Meter;
                    m_scaleFactor = 1000.0f;
                    break;

                case "micron":
                    m_unit = eUnit.Micron;
                    m_scaleFactor = 1.0f / 1000.0f;
                    break;

                case "feet":
                    m_unit = eUnit.Feet;
                    m_scaleFactor = 12.0f * 2.54f;
                    break;
            }
        }

        void ParseBody(XmlNode xBody)
        {
            GetUnit(xBody);
            m_nobjects = 0;
            foreach (XmlNode xnode in xBody.ChildNodes)
            {
                switch (xnode.Name)
                {
                    case "object":
                        ParseObject(xnode);
                        break;
                }
            }
            if (m_nobjects > 1)
            {
                for (int i = 1; i <= m_nobjects; i++)
                {
                    m_objList[i].Name += "_" + i.ToString();
                }
            }
        }

        void ParseObject(XmlNode xObject)
        {
            m_curObject = new Object3d();
            m_smoothObj = false;
            m_smoothLevel = -1;
            ParseMesh(xObject["mesh"]);
            m_curObject.m_fullname = m_filepath;
            m_curObject.Name = Path.GetFileName(m_filepath);
            m_curObject.Update();
            //m_curObject.m_wireframe = 4;
            m_objList.Add(m_curObject);
            m_nobjects++;
        }

        void ParseMesh(XmlNode xMesh)
        {
            ReadVertices(xMesh["vertices"]);
            if (m_smoothObj && (m_smoothLevel == -1))
            {
                if (m_formSmooth.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_smoothLevel = m_formSmooth.SmoothLevel;
                }
            }
            m_curObject = new Object3d();
            m_curObject.m_lstpoints = new List<Point3d>();
            m_flatTrigs = new List<TriangleAmf>();
            foreach (XmlNode node in xMesh.ChildNodes)
            {
                if (node.Name == "volume")
                    ReadVolume(node);
            }
            foreach (PointAmf pamf in m_pointList)
                m_curObject.m_lstpoints.Add(pamf.pt);
            foreach (TriangleAmf t in m_flatTrigs)
            {
                AddFlatTriangle(t.v1, t.v2, t.v3);
            }
        }

        void ReadVertices(XmlNode xVerts)
        {
            m_pointList = new List<PointAmf>();
            m_edgeList = new List<EdgeAmf>();
            foreach (XmlNode xnode in xVerts.ChildNodes)
            {
                if (xnode.Name == "vertex")
                    ParseVertex(xnode);
                if (xnode.Name == "edge")
                    ParseEdge(xnode);
            }
            // link edges to points
            foreach (EdgeAmf edge in m_edgeList)
            {
                m_pointList[edge.v1].AddEdge(edge);
            }
            m_edgeList = null; // we no longer need it
        }

        void ParseVertex(XmlNode xVert)
        {
            XmlNode xcoor = xVert["coordinates"];
            float x = float.Parse(xcoor["x"].InnerText) * m_scaleFactor;
            float y = float.Parse(xcoor["y"].InnerText) * m_scaleFactor;
            float z = float.Parse(xcoor["z"].InnerText) * m_scaleFactor;
            Point3d pt = new Point3d(x, y, z);
            PointAmf pamf= new PointAmf();
            pamf.pt = pt;

            // add normal if exists
            XmlNode xnorm = xVert["normal"];
            if (xnorm != null)
            {
                m_smoothObj = true;
                x = float.Parse(xnorm["nx"].InnerText);
                y = float.Parse(xnorm["ny"].InnerText);
                z = float.Parse(xnorm["nz"].InnerText);
                pamf.normal = new Vector3d(x, y, z);
            }

            m_pointList.Add(pamf);
        }

        void ParseEdge(XmlNode xEdge)
        {
            EdgeAmf edge = new EdgeAmf();
            edge.v1 = int.Parse(xEdge["v1"].InnerText);
            float x = float.Parse(xEdge["dx1"].InnerText); // *m_scaleFactor;
            float y = float.Parse(xEdge["dy1"].InnerText); // *m_scaleFactor;
            float z = float.Parse(xEdge["dz1"].InnerText); // *m_scaleFactor;
            edge.t1 = new Vector3d(x, y, z);
            edge.t1.Normalize();
            edge.v2 = int.Parse(xEdge["v2"].InnerText);
            x = float.Parse(xEdge["dx2"].InnerText); // *m_scaleFactor;
            y = float.Parse(xEdge["dy2"].InnerText); // *m_scaleFactor;
            z = float.Parse(xEdge["dz2"].InnerText); // *m_scaleFactor;
            edge.t2 = new Vector3d(x, y, z);
            edge.t2.Normalize();
            edge.initialEdge = true;
            m_edgeList.Add(edge);
            m_smoothObj = true;
        }

        void ReadVolume(XmlNode xVolume)
        {
            foreach (XmlNode xnode in xVolume.ChildNodes)
            {
                if (xnode.Name == "triangle")
                    ParseTriangle(xnode);
            }
        }

        // split flat triangles so they match joining smooth triangles
        void AddFlatTriangle(int v1, int v2, int v3)
        {
            int v12 = FindCenterPoint(v1, v2);
            int v23 = FindCenterPoint(v2, v3);
            int v31 = FindCenterPoint(v3, v1);

            // lots of cases:
            if ((v12 < 0) && (v23 < 0) && (v31 < 0))
            {
                AddTriangle(v1, v2, v3);
                return;
            }

            // 1 valid point
            if (Test1ValidPoint(v1, v2, v3, v12, v23, v31))
                return;
            if (Test1ValidPoint(v2, v3, v1, v23, v31, v12))
                return;
            if (Test1ValidPoint(v3, v1, v2, v31, v12, v23))
                return;

            // 2 valid points
            if (Test2ValidPoints(v1, v2, v3, v12, v23, v31))
                return;
            if (Test2ValidPoints(v2, v3, v1, v23, v31, v12))
                return;
            if (Test2ValidPoints(v3, v1, v2, v31, v12, v23))
                return;

            // 3 points are valid, full split
            AddFlatTriangle(v1, v12, v31);
            AddFlatTriangle(v2, v23, v12);
            AddFlatTriangle(v3, v31, v23);
            AddTriangle(v12, v23, v31); // last one is always full.
        }

        bool Test1ValidPoint(int v1, int v2, int v3, int v12, int v23, int v31)
        {
            // we rely on that that 0 valid points already tested
            if ((v12 < 0) && (v23 < 0))
            {
                AddFlatTriangle(v1, v2, v31);
                AddFlatTriangle(v31, v2, v3);
                return true;
            }
            return false;
        }

        bool Test2ValidPoints(int v1, int v2, int v3, int v12, int v23, int v31)
        {
            // we rely on that that 1 valid point already tested
            if (v12 < 0)
            {
                AddFlatTriangle(v1, v2, v23);
                AddFlatTriangle(v23, v31, v1);
                AddFlatTriangle(v3, v31, v23);
                return true;
            }
            return false;
        }


        int FindCenterPoint(int v1, int v2)
        {
            EdgeAmf edge = m_pointList[v1].FindEdge(v2);
            if (edge == null)
                edge = m_pointList[v2].FindEdge(v1);
            if (edge != null)
                return edge.v12;
            return -1;
        }


        bool HaveSmoothInfo(int v1, int v2)
        {
            EdgeAmf edge;
            if (m_pointList[v1].normal != null)
                return true;
            edge = m_pointList[v1].FindEdge(v2);
            if ((edge != null) && edge.initialEdge)
                return true;
            edge = m_pointList[v2].FindEdge(v1);
            if ((edge != null) && edge.initialEdge)
                return true;
            return false;
        }

        void ParseTriangle(XmlNode xTri)
        {
            int v1 = int.Parse(xTri["v1"].InnerText);
            int v2 = int.Parse(xTri["v2"].InnerText);
            int v3 = int.Parse(xTri["v3"].InnerText);
            if (!m_smoothObj || (!HaveSmoothInfo(v1, v2) && !HaveSmoothInfo(v2, v3) && !HaveSmoothInfo(v3, v1)))
                m_flatTrigs.Add(new TriangleAmf(v1,v2,v3));
            else
                SmoothTriangle(v1, v2, v3, null, null, null, m_smoothLevel);
        }
        
        void SmoothTriangle(int v1, int v2, int v3, Vector3d n1, Vector3d n2, Vector3d n3, int level)
        {
            if (level == 0)
            {
                // end smooth level, return the resulting triangle
                AddTriangle(v1, v2, v3);
                return;
            }

            // do smooth
            level--;
            // calculate normals
            if (n1 == null)
                n1 = CalcNormal(v1, v2, v3);
            if (n2 == null)
                n2 = CalcNormal(v2, v3, v1);
            if (n3 == null)
                n3 = CalcNormal(v3, v1, v2);
            Vector3d n12, n23, n31;
            int v12 = SplitEdge(v1, v2, n1, n2, out n12);
            int v23 = SplitEdge(v2, v3, n2, n3, out n23);
            int v31 = SplitEdge(v3, v1, n3, n1, out n31);
            SmoothTriangle(v1, v12, v31, n1, n12, n31, level);
            SmoothTriangle(v12, v2, v23, n12, n2, n23, level);
            SmoothTriangle(v23, v3, v31, n23, n3, n31, level);
            SmoothTriangle(v12, v23, v31, n12, n23, n31, level);
        }

        void AddTriangle(int v1, int v2, int v3)
        {
            Point3d pt1 = m_pointList[v1].pt;
            Point3d pt2 = m_pointList[v2].pt;
            Point3d pt3 = m_pointList[v3].pt;
            Polygon p = new Polygon();
            p.m_points = new Point3d[] { pt1, pt2, pt3 };
            // calculate normal
            Vector3d edge1 = new Vector3d(pt1.x - pt2.x, pt1.y - pt2.y, pt1.z - pt2.z);
            Vector3d edge2 = new Vector3d(pt3.x - pt2.x, pt3.y - pt2.y, pt3.z - pt2.z);
            p.m_normal = Vector3d.cross(edge1, edge2);
            p.m_normal.Normalize();
            m_curObject.m_lstpolys.Add(p);
        }

        // split edge v1-v2. v3 is the last corner in the triangle and is used for computing normals
        int SplitEdge(int v1, int v2, Vector3d norm1, Vector3d norm2, out Vector3d norm12)
        {
            EdgeAmf edge = m_pointList[v1].FindEdge(v2);
            if (edge == null)
            {
                edge = m_pointList[v2].FindEdge(v1);
                if (edge != null)
                {
                    // swap verteces to match edge
                    int tv = v1;
                    v1 = v2;
                    v2 = tv;
                    Vector3d tnorm = norm1;
                    norm1 = norm2;
                    norm2 = tnorm;
                }
            }

            Vector3d t1, t2;
            PointAmf pamf1 = m_pointList[v1];
            PointAmf pamf2 = m_pointList[v2];
            Point3d pt1 = pamf1.pt;
            Point3d pt2 = pamf2.pt;
            PointAmf pamf;
            float x, y, z;

            // calculate edge vector
            x = pt2.x - pt1.x;
            y = pt2.y - pt1.y;
            z = pt2.z - pt1.z;
            Vector3d edgeDir = new Vector3d(x, y, z);

            // first see if we have an edge for this segment
            if (edge != null)
            {
                // if this edge was already split, return result
                if (edge.v12 >= 0)
                {
                    norm12 = CalcCenterNormal(norm1, norm2, edge.t12);
                    return edge.v12;
                }
                t1 = edge.t1;
                t2 = edge.t2;
            }
             else
            {
                t1 = GetTangetFromNormal(norm1, edgeDir);
                t2 = GetTangetFromNormal(norm2, edgeDir);
            }

            float d = edgeDir.Mag();

            // calculate mid point using Hermite interpolation
            x = 0.5f * pt1.x + 0.125f * t1.x * d + 0.5f * pt2.x - 0.125f * t2.x * d;
            y = 0.5f * pt1.y + 0.125f * t1.y * d + 0.5f * pt2.y - 0.125f * t2.y * d;
            z = 0.5f * pt1.z + 0.125f * t1.z * d + 0.5f * pt2.z - 0.125f * t2.z * d;

            pamf = new PointAmf();
            pamf.pt = new Point3d(x, y, z);
            int v = m_pointList.Count;
            m_pointList.Add(pamf);

            // calculate new tanget and new normal
            x = -1.5f * pt1.x - 0.25f * t1.x * d + 1.5f * pt2.x - 0.25f * t2.x * d;
            y = -1.5f * pt1.y - 0.25f * t1.y * d + 1.5f * pt2.y - 0.25f * t2.y * d;
            z = -1.5f * pt1.z - 0.25f * t1.z * d + 1.5f * pt2.z - 0.25f * t2.z * d;
            Vector3d tanget = new Vector3d(x, y, z);
            tanget.Normalize();

            norm12 = CalcCenterNormal(norm1, norm2, tanget);
            
            if (edge == null)
            {
                //pamf.normal = GetNormalFromTanget(norm1, tanget);
                // create an edge for this segment
                edge = new EdgeAmf();
                edge.v1 = v1;
                edge.v2 = v2;
                edge.t1 = t1;
                edge.t2 = t2;
                pamf1.AddEdge(edge);
            }
            edge.t12 = tanget;
            edge.v12 = m_pointList.Count - 1; // saves double computation 

            //tanget.Normalize();
            // save 2 split edges
            EdgeAmf edge1 = new EdgeAmf();
            edge1.v1 = v1;
            edge1.v2 = v;
            edge1.t1 = t1;
            edge1.t2 = tanget;
            pamf1.AddEdge(edge1);

            EdgeAmf edge2 = new EdgeAmf();
            edge2.v1 = v;
            edge2.v2 = v2;
            edge2.t1 = tanget;
            edge2.t2 = t2;
            pamf.AddEdge(edge2);

            return v;
        }

        // split edge v1-v2. v3 is the last corner in the triangle and is used for computing normals
/*        int SplitEdge(int v1, int v2, int v3)
        {
            Vector3d norm1 = CalcNormal(v1, v2, v3);
            Vector3d norm2 = CalcNormal(v2, v3, v1);
            EdgeAmf edge = m_pointList[v1].FindEdge(v2);
            if (edge == null)
            {
                edge = m_pointList[v2].FindEdge(v1);
                if (edge != null)
                {
                    // swap verteces tomatch edge
                    int tv = v1;
                    v1 = v2;
                    v2 = tv;
                    Vector3d tnorm = norm1;
                    norm1 = norm2;
                    norm2 = tnorm;
                }
            }

            Vector3d t1, t2;
            PointAmf pamf1 = m_pointList[v1];
            PointAmf pamf2 = m_pointList[v2];
            Point3d pt1 = pamf1.pt;
            Point3d pt2 = pamf2.pt;
            PointAmf pamf;
            float x, y, z;

            // calculate edge vector
            x = pt2.x - pt1.x;
            y = pt2.y - pt1.y;
            z = pt2.z - pt1.z;
            Vector3d edgeDir = new Vector3d(x, y, z);

            // first see if we have an edge for this segment
            if (edge != null)
            {
                // if this edge was already split, return result
                if (edge.v12 >= 0)
                    return edge.v12;
                t1 = edge.t1;
                t2 = edge.t2;
            }
            /*else if ((pamf1.normal == null) && (pamf2.normal == null))
            {
                // its a linear line, return the center
                x = (pamf1.pt.x + pamf2.pt.x) / 2.0f;
                y = (pamf1.pt.y + pamf2.pt.y) / 2.0f;
                z = (pamf1.pt.z + pamf2.pt.z) / 2.0f;
                pamf = new PointAmf();
                pamf.pt = new Point3d(x, y, z);
                m_pointList.Add(pamf);
                return m_pointList.Count - 1;
            }
            else
            {
                // calculate tangets from normals.
                //edgeDir.Normalize();
                t1 = GetTangetFromNormal(norm1, edgeDir);
                t2 = GetTangetFromNormal(norm2, edgeDir);
                /*if (pamf1.normal == null)
                    pamf1.normal = norm1;
                if (pamf2.normal == null)
                    pamf2.normal = norm2;
            }

            float d = edgeDir.Mag();

            // calculate mid point using Hermite interpolation
            x = 0.5f * pt1.x + 0.125f * t1.x * d + 0.5f * pt2.x - 0.125f * t2.x * d;
            y = 0.5f * pt1.y + 0.125f * t1.y * d + 0.5f * pt2.y - 0.125f * t2.y * d;
            z = 0.5f * pt1.z + 0.125f * t1.z * d + 0.5f * pt2.z - 0.125f * t2.z * d;

            pamf = new PointAmf();
            pamf.pt = new Point3d(x, y, z);
            int v = m_pointList.Count;
            m_pointList.Add(pamf);

            // calculate new tanget and new normal
            x = -1.5f * pt1.x - 0.25f * t1.x * d + 1.5f * pt2.x - 0.25f * t2.x * d;
            y = -1.5f * pt1.y - 0.25f * t1.y * d + 1.5f * pt2.y - 0.25f * t2.y * d;
            z = -1.5f * pt1.z - 0.25f * t1.z * d + 1.5f * pt2.z - 0.25f * t2.z * d;
            Vector3d tanget = new Vector3d(x, y, z);
            tanget.Normalize();

            Vector3d tvec = (norm1 * 0.5f) + (norm2 * 0.5f);
            tvec = Vector3d.cross(tvec, tanget);
            pamf.normal = Vector3d.cross(tanget, tvec);

            if (edge == null)
            {
                //pamf.normal = GetNormalFromTanget(norm1, tanget);
                // create an edge for this segment
                edge = new EdgeAmf();
                edge.v1 = v1;
                edge.v2 = v2;
                edge.t1 = t1;
                edge.t2 = t2;
                pamf1.AddEdge(edge);
            }
            edge.v12 = m_pointList.Count - 1; // saves double computation 

            //tanget.Normalize();
            // save 2 split edges
            EdgeAmf edge1 = new EdgeAmf();
            edge1.v1 = v1;
            edge1.v2 = v;
            edge1.t1 = t1;
            edge1.t2 = tanget;
            pamf1.AddEdge(edge1);

            EdgeAmf edge2 = new EdgeAmf();
            edge2.v1 = v;
            edge2.v2 = v2;
            edge2.t1 = tanget;
            edge2.t2 = t2;
            pamf.AddEdge(edge2);

            return v;
        }*/

        // calc normal at corner v (looking at the triangle when corner v is at the bottom, v1 at top right, and v2 is at to left
        Vector3d CalcNormal(int v, int v1, int v2)
        {
            PointAmf pamf = m_pointList[v];
            if (pamf.normal != null)
                return pamf.normal;
            Vector3d t1 = GetTanget(v, v1);
            Vector3d t2 = GetTanget(v, v2);
            Vector3d normal = Vector3d.cross(t1, t2);
            normal.Normalize();
            return normal;
        }

        // calc a split point normal from 2 edge normal and center tanget
        Vector3d CalcCenterNormal(Vector3d norm1, Vector3d norm2, Vector3d tanget)
        {
            Vector3d tvec = (norm1 * 0.5f) + (norm2 * 0.5f);
            tvec = Vector3d.cross(tvec, tanget);
            Vector3d res = Vector3d.cross(tanget, tvec);
            res.Normalize();
            return res;
        }

        // get a tanget of an egde, at point v
        Vector3d GetTanget(int v, int v1)
        {
            EdgeAmf edge = m_pointList[v].FindEdge(v1);
            if (edge != null)
                return edge.t1;
            edge = m_pointList[v1].FindEdge(v);
            if (edge != null)
                return Vector3d.negate(edge.t2);
            Point3d pt1 = m_pointList[v].pt;
            Point3d pt2 = m_pointList[v1].pt;
            return new Vector3d(pt2.x - pt1.x, pt2.y - pt1.y, pt2.z - pt1.z);
        }

        Vector3d GetTangetFromNormal(Vector3d norm, Vector3d dir)
        {
            Vector3d normxdir = Vector3d.cross(norm, dir);
            Vector3d res = Vector3d.cross(normxdir, norm);
            if ((Math.Abs(res.x) < Epsilon) && (Math.Abs(res.y) < Epsilon) && (Math.Abs(res.z) < Epsilon))
                res = dir.clone();
            res.Normalize();
            //res = res * dir.Mag();
            return res;
        }

        Vector3d GetNormalFromTanget(Vector3d norm, Vector3d tanget)
        {
            Vector3d normtang = Vector3d.cross(norm, tanget);
            Vector3d res = Vector3d.cross(tanget, normtang);
            if ((Math.Abs(res.x) < Epsilon) && (Math.Abs(res.y) < Epsilon) && (Math.Abs(res.z) < Epsilon))
                return null;
            res.Normalize();
            //res = res * dir.Mag();
            return res;
        }

     }
}
