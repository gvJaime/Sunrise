using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public enum MachineControlAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
        Tilt = 3,
        Extruder = 4,
        All
    }

    public class ctlMCBase : ctlUserPanel
    {
        protected Color mFrameColor;
        protected Color[] mLevelColors;
        protected Color mSelColor;
        protected Color mArrowCol;
        protected Color mInvBackColor;
        protected String mTitle;
        protected String mUnit;
        protected float[] mLevelVals;

        public ctlMCBase()
        {
            mLevelColors = new Color[4];
            mArrowCol = Color.FromArgb(120, 200, 255, 200);
            mTitle = "";
            mUnit = "";
            mLevelVals = new float[] { 0, 0, 0, 0 };
            UpdateInvBackColor();
        }

        public delegate void MotorMoveDelegate(Object sender, MachineControlAxis axis, float val);
        public delegate void MotorHomeDelegate(Object sender, MachineControlAxis axis);
        
        [Description("Base color of all graphics"), Category("Data")]
        public Color FrameColor
        {
            get { return mFrameColor; }
            set { mFrameColor = value; UpdateColors();  UpdateBitmaps(); }
        }

        [DefaultValue("")]
        [Description("Title of the control"), Category("Data")]
        public String Title
        {
            get { return mTitle; }
            set { mTitle = value; Invalidate(); }
        }

        [DefaultValue("")]
        [Description("Unit of the control"), Category("Data")]
        public String Unit
        {
            get { return mUnit; }
            set { mUnit = value; Invalidate(); }
        }

        [Description("Return value array"), Category("Data")]
        public float [] ReturnValues
        {
            get { return mLevelVals; }
            set {
                for (int i = 0; i < Math.Min(mLevelVals.Length, value.Length); i++)
                    mLevelVals[i] = value[i];
                Invalidate(); 
            }
        }

        protected void UpdateInvBackColor()
        {
            if (BackColor == null)
                return;
            int r = BackColor.R;
            int g = BackColor.G;
            int b = BackColor.B;
            int avr = (r + g + b) / 3;
            if (avr > 128)
                mInvBackColor = Color.FromArgb(r / 4, g / 4, b / 4);
            else
                mInvBackColor = Color.FromArgb(191 + r / 4, 191 + g / 4, 192 + b / 4);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            UpdateInvBackColor();
        }

        protected void DrawImageCentered(Graphics gr, Image img, int x, int y)
        {
            int w = img.Width;
            int h = img.Height;
            gr.DrawImage(img, x - w / 2, y - h / 2, w, h);
        }

        protected void DrawText(Graphics gr, string str, float x, float y, Color col, bool outline = false)
        {
            if (outline)
            {
                Brush bkbr = new SolidBrush(Color.FromArgb((col.R + col.G + col.B) / 6, Color.Black));
                gr.DrawString(str, Font, bkbr, x + 1, y + 1);
            }
            Brush br = new SolidBrush(col);
            gr.DrawString(str, Font, br, x, y);
        }

        protected void DrawTextCentered(Graphics gr, string str, float x, float y, Color col, bool outline = false)
        {
            SizeF sf = gr.MeasureString(str, Font);
            x -= sf.Width / 2;
            y -= sf.Height / 2;
            DrawText(gr, str, x, y, col, outline);
        }

        protected virtual Color GetLevelColor(int anum)
        {
            if (anum == 0)
                return mFrameColor;
            return Color.FromArgb(
                mFrameColor.A,
                mFrameColor.R + (anum * (255 - mFrameColor.R)) / 4,
                mFrameColor.G + (anum * (255 - mFrameColor.G)) / 4,
                mFrameColor.B + (anum * (255 - mFrameColor.B)) / 4
            );
        }

        protected void UpdateColors()
        {
            for (int i = 0; i < 4; i++)
            {
                mLevelColors[i] = GetLevelColor(i);
            }
            mSelColor = Color.FromArgb(mFrameColor.A, 255 - mFrameColor.R, 255 - mFrameColor.G, 255 - mFrameColor.B);

        }

        protected bool HitBitmap(int x, int y, Image img, int imgx, int imgy, int transLevel = 250)
        {
            int tx = x - imgx;
            int ty = y - imgy;
            if ((tx > 0) && (ty>0) && (tx < img.Width) && (ty < img.Height))
            {
                Color pix = ((Bitmap)img).GetPixel(tx, ty);
                if (pix.A > transLevel)
                    return true;
            }
            return false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseEventArgs me = new MouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, -1, -1, 0);
            OnMouseMove(me);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor == null)
            {
                base.OnPaintBackground(e);
                return;
            }
            Brush br = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(br, 0, 0, Width, Height);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Color fcol = Color.FromArgb(90, mInvBackColor);
            Pen pen = new Pen(fcol, 2);
            C2DGraphics.DrawRoundRectangle(e.Graphics, pen, 0, 0, Width - 1, Height - 1, 5);
        }

        protected virtual void UpdateBitmaps()
        {
        }
    }
}
