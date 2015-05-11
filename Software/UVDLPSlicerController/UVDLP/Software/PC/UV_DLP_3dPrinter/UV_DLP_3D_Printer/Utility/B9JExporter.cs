using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UV_DLP_3D_Printer.Slicing;
using UV_DLP_3D_Printer._3DEngine;
using Engine3D;
using UV_DLP_3D_Printer.Configs;
using UV_DLP_3D_Printer.GUI.ExportControls;

namespace UV_DLP_3D_Printer
{
    public class BitArray
    {
        public byte[] bytes;
        public int bitcount;
        public BitArray()
        {
            bytes = new byte[100];
            bitcount = 0;
        }

        public void PushBits(int value, int nbits)
        {
            int nbytes = (bitcount + nbits) / 8;
            if (nbytes >= bytes.Length)
                Array.Resize(ref bytes, bytes.Length * 2);
            for (int i = nbits - 1; i >= 0; i--)
            {
                if ((value & (1 << i)) != 0)
                {
                    bytes[bitcount / 8] |= (byte)(1 << (bitcount & 7)); 
                }
                bitcount++;
            }
        }
    }

    public class B9JExporter : CWExporter
    {
        Stream m_stream = null;
        ctlExportB9Creator m_ctlSettings = null;
        string m_name = null;
        string m_description = null;

        public override string FileExtension
        {
            get { return "b9j"; }
        }

        public override string Description
        {
            get { return "B9Creator job file"; } 
        }

        public override Control SettingPanel
        {
            get
            {
                if (m_ctlSettings == null)
                    m_ctlSettings = new ctlExportB9Creator();
                return m_ctlSettings;
            }
        }

        public override string Export(Stream stream, string filename)
        {
            m_stream = stream;
            if ((m_name == null) || (m_name.Length == 0))
                m_name = Path.GetFileNameWithoutExtension(filename);
            if ((m_description == null) || (m_description.Length == 0))
                m_description = m_name;
            try
            {
                ReportStart();
                SliceBuildConfig config = UVDLPApp.Instance().m_buildparms;
                config.UpdateFrom(UVDLPApp.Instance().m_printerinfo); // make sure we've got the correct display size and PixPerMM values   

                // write header
                WriteString("1");   // version
                WriteString(m_name);  // scene name
                WriteString(m_description);  // scene description
                WriteDouble(1 / config.dpmmX); // pixel size in mm
                WriteDouble(config.ZThick);    // slice thickness
                WriteInt32(0);      // base standoff layers (?)
                WriteInt32(0);      // Number of base offset layers where extents are filled (?)
                WriteString("Reserved3");
                WriteString("Reserved2");
                WriteString("Reserved1");

                // write slices
                int numslices = UVDLPApp.Instance().m_slicer.GetNumberOfSlices(config);
                WriteInt32(numslices);

                float zlev = (float)(config.ZThick * 0.5);
                int npix = config.xres * config.yres;
                int[] lbm = new int[npix];  // current slice
                int p;

                //Bitmap bm = new Bitmap(config.xres, config.yres, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // working bitmap
                //Color savecol = UVDLPApp.Instance().m_appconfig.m_foregroundcolor;
                
                if (UVDLPApp.Instance().m_slicer.SliceFile == null)
                {
                    SliceFile sf = new SliceFile(config);
                    sf.m_mode = SliceFile.SFMode.eImmediate;
                    UVDLPApp.Instance().m_slicer.SliceFile = sf; // wasn't set
                }

                for (int c = 0; c < numslices; c++)
                {
                    //bool layerneedssupport = false;
                    if (CancelExport)
                    {
                        return "Info|Export operation canceled";
                    }
                    ReportProgress(c * 100 / numslices);

                    /*Slice sl = UVDLPApp.Instance().m_slicer.GetSliceImmediate(zlev);
                    zlev += (float)config.ZThick;

                    if ((sl == null) || (sl.m_segments == null) || (sl.m_segments.Count == 0))
                        continue;
                    sl.Optimize();// find loops
                    using (Graphics gfx = Graphics.FromImage(bm))
                        gfx.Clear(Color.Transparent);

                    //render current slice
                    UVDLPApp.Instance().m_appconfig.m_foregroundcolor = Color.White;
                    sl.RenderSlice(config, ref bm);*/

                    Bitmap bm = UVDLPApp.Instance().m_slicer.SliceImmediate(zlev);
                    zlev += (float)config.ZThick;
                    BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                            ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    Marshal.Copy(data.Scan0, lbm, 0, lbm.Length);
                    for (p = 0; p < npix; p++)
                        lbm[p] &= 0xFFFFFF;
                    bm.UnlockBits(data);
                    CrushSlice(lbm, config.xres, config.yres);
                }

                // write supports (currently not handled)
                WriteInt32(0); // zero supports
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                return "Error|Export terminated unexpectedly";
            }

            return "Export|Export completed successfully";
        }

        void CrushSlice(int[] lbm, int resx, int resy)
        {
            BitArray ba = new BitArray();
            int top = resy;
            int left = resx;
            int bottom = 0;
            int right = 0;
            int npix = resx * resy;
            int nWhitePixels = 0;
            bool curPixelWhite = lbm[0] != 0;
            int currentPos = 0;
            int dataLen;

            ba.PushBits(resx, 16);
            ba.PushBits(resy, 16);
            ba.PushBits(curPixelWhite ? 1 : 0, 1);

            do
            {
                dataLen = 0;
                curPixelWhite = lbm[currentPos] > 0; //returns true if pixel is white
              
                // count the pixels until the color changes
                while ((currentPos < npix) && (curPixelWhite == lbm[currentPos] > 0))
                {
                    currentPos++;
                    dataLen++;
                }
                if (curPixelWhite)
                {
                    nWhitePixels += dataLen;
                    // update extents
                    int origpos = currentPos - dataLen;
                    int endpos = currentPos - 1;
                    int xstart = origpos % resx;
                    int xend = endpos % resx;
                    if (xstart + dataLen - 1 >= resx)
                    {
                        xend = resx - 1;
                        if (xstart + dataLen - 1 > resx)
                            xstart = 0;
                    }
                    if (top == resy)
                        top = (origpos / resx);
                    if (bottom < (endpos / resx))
                        bottom = (endpos / resx);
                    if (left > xstart)
                        left = xstart;
                    if (right < xend)
                        right = xend;
                }

                // store the count
                int keyLen = computeKeySize(dataLen);
                if (keyLen < 0)
                {
                    // should not happen!
                    CancelExport = true;
                    return;
                }
                ba.PushBits(keyLen, 5);
                ba.PushBits(dataLen, keyLen + 1);
            } while (currentPos < npix);

            // write crushed slice to file
            WriteInt32(nWhitePixels);
            WriteInt32(left);
            WriteInt32(top);
            WriteInt32(right);
            WriteInt32(bottom);
            WriteBitArray(ba);
        }

        int computeKeySize(int dataLen)
        {
            for (int iKey = 0; iKey < 32; iKey++)
                if (dataLen <= (1 << iKey))
                    return iKey;
            return -1;
        }

        void WriteString(string str)
        {
            WriteInt32(str.Length * 2);
            foreach (char ch in str)
            {
                WriteInt16(ch);
            }
        }

        void WriteInt32(int value)
        {
            m_stream.WriteByte((byte)(value >> 24));
            m_stream.WriteByte((byte)(value >> 16));
            m_stream.WriteByte((byte)(value >> 8));
            m_stream.WriteByte((byte)value);
        }

        void WriteInt16(int value)
        {
            m_stream.WriteByte((byte)(value >> 8));
            m_stream.WriteByte((byte)value);
        }

        void WriteDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                for (int i = bytes.Length - 1; i >= 0; i--)
                    m_stream.WriteByte(bytes[i]);
            }
            else
            {
                for (int i = 0 - 1; i < bytes.Length; i++)
                    m_stream.WriteByte(bytes[i]);
            }
        }

        void WriteBitArray(BitArray ba)
        {
            WriteInt32(ba.bitcount);
            m_stream.Write(ba.bytes, 0, (ba.bitcount + 7) / 8);
        }

        public override string ValidateExport()
        {
            if (UVDLPApp.Instance().m_engine3d.m_objects.Count == 0)
                return "Attention|Scene is empty, export aborted";
            if (m_ctlSettings != null)
            {
                m_name = m_ctlSettings.SceneName;
                m_description = m_ctlSettings.Description;
            }
            return null;
        }
    }
}
