using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlPreview : ctlUserPanel
    {
        Image mImage;
        Rectangle mDstrc;
        public ctlPreview()
        {
            InitializeComponent();
        }

        [Description("Image composesed of all 4 button states"), Category("Data")]
        public Image Image
        {
            get { return mImage; }
            set { 
                mImage = value;
                ScaleImage();
                Invalidate();
            }
        }

        void ScaleImage()
        {
            if (mImage == null)
                return;
            if ((Height == 0) || (Width == 0))
                return;
            float iratio = (float)Image.Width / (float)Image.Height;
            float cratio = (float)Width / (float)Height;
            if (iratio > cratio)
            {
                int h = (int)((float)Width / iratio);
                mDstrc = new Rectangle(0, (Height - h) / 2, Width, h);
            }
            else
            {
                int w = (int)((float)Height * iratio);
                mDstrc = new Rectangle((Width - w) / 2, 0, w, Height);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ScaleImage();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen p = new Pen(Style.ForeColor, 4);
            g.DrawRectangle(p, 0, 0, Width, Height);
            if (Image != null)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.DrawImage(mImage, mDstrc);
            }
            else if (Text != null)
            {
                SizeF texts = g.MeasureString(Text, Font);
                float posx = ((float)Width - texts.Width) / 2;
                float posy = ((float)Height - texts.Height) / 2;
                g.DrawString(Text, Font, new SolidBrush(Style.ForeColor), posx, posy);
            }
        }
    }
}
