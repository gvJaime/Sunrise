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
    public class ctlMCShutter : ctlMCBase
    {
        Bitmap mSOpen, mSClose;
        Image mSOpenStd, mSOpenSel, mSOpenPress;
        Image mSCloseStd, mSCloseSel, mSClosePress;
        int mCtlHeight;
        int mTextX, mTextY;
        bool mOverOpen, mOverClose;
        bool mPressOpen, mPressClose;
        public delegate void StateChangeDelegate(object obj, bool state);
        public event StateChangeDelegate StateChange;

        public ctlMCShutter()
        {
            DoubleBuffered = true;
            Font = new Font("Arial", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            mSOpen = global::UV_DLP_3D_Printer.Properties.Resources.mc_shuttopen;
            mSClose = global::UV_DLP_3D_Printer.Properties.Resources.mc_shuttclose;
            mCtlHeight = mSOpen.Height;
            Height = mCtlHeight;
            mTextX = 5;
            mTextY = (mCtlHeight - Font.Height) / 2;
            mOverOpen = mOverClose = false;
            mPressOpen = mPressClose = false;
            FrameColor = Color.RoyalBlue;
            mTitle = "Shutter:";
        }

        protected override void UpdateBitmaps()
        {
            mSOpenStd = C2DGraphics.ColorizeBitmapHQ(mSOpen, mLevelColors[2]);
            mSOpenSel = C2DGraphics.ColorizeBitmapHQ(mSOpen, mSelColor);
            mSOpenPress = C2DGraphics.ColorizeBitmapHQ(mSOpen, mLevelColors[0]);
            mSCloseStd = C2DGraphics.ColorizeBitmapHQ(mSClose, mLevelColors[2]);
            mSCloseSel = C2DGraphics.ColorizeBitmapHQ(mSClose, mSelColor);
            mSClosePress = C2DGraphics.ColorizeBitmapHQ(mSClose, mLevelColors[0]);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int padd = 5;
            Graphics gr = e.Graphics;
            if (mTitle != null)
                DrawText(gr, mTitle, mTextX, mTextY, mInvBackColor, true);
            int xpos = Width - mSOpen.Width - padd;
            if (mOverOpen)
            {
                if (mPressOpen)
                    gr.DrawImage(mSOpenPress, xpos, 0);
                else
                    gr.DrawImage(mSOpenSel, xpos, 0);
            }
            else
                gr.DrawImage(mSOpenStd, xpos, 0);
            xpos -= mSClose.Width + padd;
            if (mOverClose)
            {
                if (mPressClose)
                    gr.DrawImage(mSClosePress, xpos, 0);
                else
                    gr.DrawImage(mSCloseSel, xpos, 0);
            }
            else
                gr.DrawImage(mSCloseStd, xpos, 0);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int w = mSOpen.Width - 2;
            int h = mSOpen.Height - 2;
            int xpos = e.X - (Width - mSOpen.Width);
            bool overPtr = xpos > 2 && xpos < w && e.Y > 2 && e.Y < w && !mPressClose;
            if (overPtr != mOverOpen)
            {
                mOverOpen = overPtr;
                Invalidate();
            }
            xpos = e.X - (Width - 2 * mSOpen.Width);
            overPtr = xpos > 2 && xpos < w && e.Y > 2 && e.Y < w && !mPressOpen;
            if (overPtr != mOverClose)
            {
                mOverClose = overPtr;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (mOverOpen)
            {
                mPressOpen = true;
                Invalidate();
            }
            if (mOverClose)
            {
                mPressClose = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            if (mPressOpen)
            {
                mPressOpen = false;
                if (mOverOpen && StateChange != null)
                    StateChange(this, true);
                Invalidate();
            }
            if (mPressClose)
            {
                mPressClose = false;
                if (mOverClose && StateChange != null)
                    StateChange(this, false);
                Invalidate();
            }
                
        }
    }
}
