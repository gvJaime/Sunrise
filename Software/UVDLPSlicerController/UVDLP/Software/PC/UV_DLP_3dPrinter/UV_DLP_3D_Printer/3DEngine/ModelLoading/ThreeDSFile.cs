using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using UV_DLP_3D_Printer;
namespace Engine3D
{
    public struct Triangle
    {
        public int vertex1;
        public int vertex2;
        public int vertex3;

        public Triangle(int v1, int v2, int v3)
        {
            vertex1 = v1;
            vertex2 = v2;
            vertex3 = v3;
        }

        public override string ToString()
        {
            return String.Format("v1: {0} v2: {1} v3: {2}", vertex1, vertex2, vertex3);
        }
    }
    public class ThreeDSFile
    {
        enum Groups
        {
            C_PRIMARY = 0x4D4D,
            C_OBJECTINFO = 0x3D3D,
            C_VERSION = 0x0002,
            C_EDITKEYFRAME = 0xB000,
            C_MATERIAL = 0xAFFF,
            C_MATNAME = 0xA000,
            C_MATAMBIENT = 0xA010,
            C_MATDIFFUSE = 0xA020,
            C_MATSPECULAR = 0xA030,
            C_MATSHININESS = 0xA040,
            C_MATMAP = 0xA200,
            C_MATMAPFILE = 0xA300,
            C_OBJECT = 0x4000,
            C_OBJECT_MESH = 0x4100,
            C_OBJECT_VERTICES = 0x4110,
            C_OBJECT_FACES = 0x4120,
            C_OBJECT_MATERIAL = 0x4130,
            C_OBJECT_UV = 0x4140
        }

        class ThreeDSChunk
        {
            public ushort ID;
            public uint Length;
            public int BytesRead;

            public ThreeDSChunk(BinaryReader reader)
            {
                // 2 byte ID
                ID = reader.ReadUInt16();
                //Console.WriteLine ("ID: {0}", ID.ToString("x"));

                // 4 byte length
                Length = reader.ReadUInt32();
                //Console.WriteLine ("Length: {0}", Length);

                // = 6
                BytesRead = 6;
            }
        }

        BinaryReader reader;

        //Object3d model = new Object3d();
        ArrayList models = new ArrayList();
        public ArrayList ThreeDSModel
        {
            get
            {
                return models;
            }
        }

        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        string base_dir;

        public ThreeDSFile(string file_name)
        {
            base_dir = new FileInfo(file_name).DirectoryName + "/";

            FileStream file;
            file = new FileStream(file_name, FileMode.Open, FileAccess.Read);

            reader = new BinaryReader(file);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            ThreeDSChunk chunk = new ThreeDSChunk(reader);
            if (chunk.ID != (short)Groups.C_PRIMARY)
                throw new ApplicationException("Not a proper 3DS file.");

            ProcessChunk(chunk);

            reader.Close();
            file.Close();
        }

        void ProcessChunk(ThreeDSChunk chunk)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_VERSION:

                        int version = reader.ReadInt32();
                        child.BytesRead += 4;

                        Console.WriteLine("3DS File Version: {0}", version);
                        break;

                    case Groups.C_OBJECTINFO:

                        ThreeDSChunk obj_chunk = new ThreeDSChunk(reader);

                        // not sure whats up with this chunk
                        SkipChunk(obj_chunk);
                        child.BytesRead += obj_chunk.BytesRead;

                        ProcessChunk(child);

                        break;

                    case Groups.C_MATERIAL:

                        ProcessMaterialChunk(child);
                        //SkipChunk ( child );
                        break;

                    case Groups.C_OBJECT:

                        //SkipChunk ( child );
                        string name = ProcessString(child);
                        Console.WriteLine("OBJECT NAME: {0}", name);

                        Object3d e = ProcessObjectChunk(child);
                        //e.CalculateNormals();
                        e.Update();
                        models.Add(e);

                        break;

                    default:

                        SkipChunk(child);
                        break;

                }

                chunk.BytesRead += child.BytesRead;
                //Console.WriteLine ( "ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
            }
        }

        void ProcessMaterialChunk(ThreeDSChunk chunk)
        {
            string name = string.Empty;
            Material m = new Material();

            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_MATNAME:

                        name = ProcessString(child);
                        Console.WriteLine("Material: {0}", name);
                        break;

                    case Groups.C_MATAMBIENT:

                        m.Ambient = ProcessColorChunk(child);
                        break;

                    case Groups.C_MATDIFFUSE:

                        m.Diffuse = ProcessColorChunk(child);
                        break;

                    case Groups.C_MATSPECULAR:

                        m.Specular = ProcessColorChunk(child);
                        break;

                    case Groups.C_MATSHININESS:

                        m.Shininess = ProcessPercentageChunk(child);
                        //Console.WriteLine ( "SHININESS: {0}", m.Shininess );
                        break;

                    case Groups.C_MATMAP:

                        ProcessPercentageChunk(child);

                        //SkipChunk ( child );
                        ProcessTexMapChunk(child, m);

                        break;

                    default:

                        SkipChunk(child);
                        break;
                }
                chunk.BytesRead += child.BytesRead;
            }
            materials.Add(name, m);
        }

        void ProcessTexMapChunk(ThreeDSChunk chunk, Material m)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);
                switch ((Groups)child.ID)
                {
                    case Groups.C_MATMAPFILE:

                        string name = ProcessString(child);
                        Console.WriteLine("	Texture File: {0}", name);
                        /*
                        Bitmap bmp;
                        try
                        {                        
                            bmp = new Bitmap(base_dir + name);
                        }
                        catch (Exception e)
                        {
                            // couldn't find the file
                            Console.WriteLine("	ERROR: could not load file '{0}'", base_dir + name);
                            break;
                        }

                        // Flip image (needed so texture are the correct way around!)
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        System.Drawing.Imaging.BitmapData imgData = bmp.LockBits(new Rectangle(new Point(0, 0), bmp.Size),
                                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        //								System.Drawing.Imaging.PixelFormat.Format24bppRgb ); 

                        m.BindTexture(imgData.Width, imgData.Height, imgData.Scan0);

                        bmp.UnlockBits(imgData);
                        bmp.Dispose();
                        */

                        break;

                    default:

                        SkipChunk(child);
                        break;

                }
                chunk.BytesRead += child.BytesRead;
            }
        }

        float[] ProcessColorChunk(ThreeDSChunk chunk)
        {
            ThreeDSChunk child = new ThreeDSChunk(reader);
            float[] c = new float[] { (float)reader.ReadByte() / 256, (float)reader.ReadByte() / 256, (float)reader.ReadByte() / 256 };
            //Console.WriteLine ( "R {0} G {1} B {2}", c.R, c.B, c.G );
            chunk.BytesRead += (int)child.Length;
            return c;
        }

        int ProcessPercentageChunk(ThreeDSChunk chunk)
        {
            ThreeDSChunk child = new ThreeDSChunk(reader);
            int per = reader.ReadUInt16();
            child.BytesRead += 2;
            chunk.BytesRead += child.BytesRead;
            return per;
        }

        Object3d ProcessObjectChunk(ThreeDSChunk chunk)
        {
            return ProcessObjectChunk(chunk, new Object3d());
        }

        Object3d ProcessObjectChunk(ThreeDSChunk chunk, Object3d e)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_OBJECT_MESH:

                        ProcessObjectChunk(child, e);
                        break;

                    case Groups.C_OBJECT_VERTICES:

                        //e.vertices = ReadVertices(child);
                        e.m_lstpoints = ReadVertices(child);
                        break;

                    case Groups.C_OBJECT_FACES:

                        //e.indices = 
                        Triangle []tris = ReadIndices(child);
                        foreach (Triangle t in tris) 
                        {
                            Polygon p = new Polygon();
                            p.m_points = new Point3d[3];
                            p.m_points[0] = (Point3d)e.m_lstpoints[t.vertex1];
                            p.m_points[1] = (Point3d)e.m_lstpoints[t.vertex2];
                            p.m_points[2] = (Point3d)e.m_lstpoints[t.vertex3];
                            e.m_lstpolys.Add(p);
                        }
                        e.Update();
                        if (child.BytesRead < child.Length)
                            ProcessObjectChunk(child, e);
                        break;

                    case Groups.C_OBJECT_MATERIAL:

                        string name2 = ProcessString(child);
                        Console.WriteLine("	Uses Material: {0}", name2);

                        Material mat;
                        if (materials.TryGetValue(name2, out mat))
                            e.material = mat;
                        else
                            Console.WriteLine(" Warning: Material '{0}' not found. ", name2);

                        SkipChunk(child);
                        break;

                    case Groups.C_OBJECT_UV:

                        int cnt = reader.ReadUInt16();
                        child.BytesRead += 2;

                        Console.WriteLine("	TexCoords: {0}", cnt);
                        //e.texcoords = new TexCoord[cnt];
                        //TexCoord tc = new TexCoord();
                        for (int ii = 0; ii < cnt; ii++)
                        {
                            //should add this to a list somewhere
                            TexCoord tc = new TexCoord(reader.ReadSingle(), reader.ReadSingle());
                        }

                        child.BytesRead += (cnt * (4 * 2));

                        break;

                    default:

                        SkipChunk(child);
                        break;

                }
                chunk.BytesRead += child.BytesRead;
                //Console.WriteLine ( "	ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
            }
            return e;
        }

        void SkipChunk(ThreeDSChunk chunk)
        {
            int length = (int)chunk.Length - chunk.BytesRead;
            reader.ReadBytes(length);
            chunk.BytesRead += length;
        }

        string ProcessString(ThreeDSChunk chunk)
        {
            StringBuilder sb = new StringBuilder();

            byte b = reader.ReadByte();
            int idx = 0;
            while (b != 0)
            {
                sb.Append((char)b);
                b = reader.ReadByte();
                idx++;
            }
            chunk.BytesRead += idx + 1;

            return sb.ToString();
        }

        List<Point3d> ReadVertices(ThreeDSChunk chunk)
        {
            ushort numVerts = reader.ReadUInt16();
            chunk.BytesRead += 2;
            Console.WriteLine("	Vertices: {0}", numVerts);
            List<Point3d> lst = new List<Point3d>();
            for (int ii = 0; ii < numVerts; ii++)
            {
                
                float f1 = reader.ReadSingle();
                float f2 = reader.ReadSingle();
                float f3 = reader.ReadSingle();
                Point3d pnt = new Point3d(f1,f2,f3);
                lst.Add(pnt);
            }

            chunk.BytesRead += lst.Count * (3 * 4);

            return lst;
        }

        Triangle[] ReadIndices(ThreeDSChunk chunk)
        {
            ushort numIdcs = reader.ReadUInt16();
            chunk.BytesRead += 2;
            Console.WriteLine("	Indices: {0}", numIdcs);
            Triangle[] idcs = new Triangle[numIdcs];

            for (int ii = 0; ii < idcs.Length; ii++)
            {
                idcs[ii] = new Triangle(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                // flags
                reader.ReadUInt16();
            }
            chunk.BytesRead += (2 * 4) * idcs.Length;
            return idcs;
        }

    }
}
