using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlImage : ctlAnchorable
    { 
        Image mImage;
        String mGLImage;
        C2DImage mGLImageCach;

        [DefaultValue(null)]
        [Description("Image composesed of all 4 button states"), Category("Data")]
        public Image Image
        {
            get { return mImage; }
            set { mImage = value;}
        }

        [DefaultValue(null)]
        [Description("GL image name"), Category("Data")]
        public String GLImage
        {
            get { return mGLImage; }
            set
            {
                mGLImage = value;
                mGLImageCach = null;
            }
        }

        public ctlImage()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (GLDisplay)
                return;
            BackColor = Style.BackColor;
            base.OnPaintBackground(e);
        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (GLDisplay)
            {
                base.OnPaint(pevent);
                return;
            }
            Image img = mImage;
            if (img == null)
                img = UVDLPApp.Instance().m_2d_graphics.GetBitmap(mGLImage);
            if (img == null)
                return;
            Graphics gr = pevent.Graphics;
            img = C2DGraphics.ColorizeBitmap((Bitmap)img, Style.ForeColor);
            Rectangle dstrc = new Rectangle(0, 0, Width, Height);
            gr.DrawImage(img, dstrc);
            //base.OnPaint(pevent);
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.ForeColor.IsValid())
                ForeColor = ct.ForeColor;
            if (ct.BackColor.IsValid())
                BackColor = ct.BackColor;
        }

        private void InitializeComponent()
        {
        }

        public override void OnGLPaint(C2DGraphics gr)
        {
            base.OnGLPaint(gr);
            if (mGLImageCach == null)
            {
                mGLImageCach = gr.GetImage(mGLImage);
                if (mGLImageCach == null)
                    return;
                gr.SetColor(Style.ForeColor);
                gr.Image(mGLImageCach, 0, 0, mGLImageCach.w, mGLImageCach.h, 0, 0, Width, Height);
            }
        }
    }
}
