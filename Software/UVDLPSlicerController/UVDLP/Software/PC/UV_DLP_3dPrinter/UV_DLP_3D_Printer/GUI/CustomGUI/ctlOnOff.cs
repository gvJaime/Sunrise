using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class ctlOnOff : ctlMCBase
    {
        Image mOnOffBgnd;
        Image mOnOffMain, mOnOffMainSel;
        Image mOnOffSlide, mOnOffSlideSel;
        int mCtlHeight;
        int mTextX, mTextY;
        bool mOverSwitch;
        bool mIsOn;
        bool mIsDown;
        public delegate void StateChangeDelegate(object obj, bool state);
        public event StateChangeDelegate StateChange;

        public ctlOnOff()
        {
            DoubleBuffered = true;
            Font = new Font("Arial", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            mOnOffBgnd = global::UV_DLP_3D_Printer.Properties.Resources.mc_onoffbgnd;
            mCtlHeight = mOnOffBgnd.Height;
            Height = mCtlHeight;
            mTextX = 5;
            mTextY = (mCtlHeight - Font.Height) / 2;
            mIsOn = false;
            mOverSwitch = false;
            mIsDown = false;
            FrameColor = Color.RoyalBlue;
        }

        [DefaultValue(false)]
        [Description("Switch state"), Category("Data")]
        public bool IsOn
        {
            get { return mIsOn; }
            set { mIsOn = value; Invalidate(); }
        }

        protected override void UpdateBitmaps()
        {
            mOnOffMain = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_onoff, mLevelColors[2]);
            mOnOffMainSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_onoff, mSelColor);
            mOnOffSlide = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_onoffslide, mLevelColors[2]);
            mOnOffSlideSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_onoffslide, mSelColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gr = e.Graphics;
            if (mTitle != null)
                DrawText(gr, mTitle, mTextX, mTextY, mInvBackColor, true);
            int xpos = Width - mOnOffBgnd.Width;
            gr.DrawImage(mOnOffBgnd, xpos, 0, mOnOffBgnd.Width, mOnOffBgnd.Height);
            Image img = mOverSwitch ? mOnOffMainSel : mOnOffMain;
            gr.DrawImage(img, xpos, 0, img.Width, img.Height);
            img = mIsDown ? mOnOffSlideSel : mOnOffSlide;
            if (mIsOn) xpos = Width - img.Width;
            gr.DrawImage(img, xpos, 0, img.Width, img.Height);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int xpos = Width - mOnOffBgnd.Width;
            bool overPtr = HitBitmap(e.X, e.Y, mOnOffBgnd, xpos, 0, 100);
            if (overPtr != mOverSwitch)
            {
                mOverSwitch = overPtr;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (mOverSwitch)
            {
                mIsDown = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            if (mIsDown)
            {
                mIsDown = false;
                mIsOn = !mIsOn;
                if (StateChange != null)
                    StateChange(this, mIsOn);
                Invalidate();
            }
                
        }
    }
}
