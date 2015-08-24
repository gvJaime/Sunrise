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
    public partial class ctlMCXY : ctlMCBase
    {
        Image[] mArches;
        Image[] mArchesSel;
        Rectangle[] mOffsets;
        PointF[] mArchTxtPos;
        Image mCentImg, mCentImgSel;
        Image mYHomeImg, mYHomeImgSel;
        Image mXHomeImg, mXHomeImgSel;
        Image mAllHomeImg, mAllHomeImgSel;
        Image mHomeImg, mhomeImgCent;
        Image mArrowU, mArrowD, mArrowL, mArrowR;
        MachineControlAxis mSelAxis, mLastSelAxis;
        int mSelLevel, mLastSelLevel;
        int mCenter;
        int mButMin, mButMax;
        int mButHomePos;
        int mArrowPos;
        double mInRad;
        double mButtRad;
        double mArchWidth;
        int mCircWidth;

        public event MotorMoveDelegate MotorMove;
        public event MotorHomeDelegate MotorHome;

        //int mCircsize;
        public ctlMCXY()
        {
            InitializeComponent();
            mArches = new Image[4];
            mArchesSel = new Image[4];
            mOffsets = new Rectangle[4];
            mArchTxtPos = new PointF[4];
            mLevelVals = new float[] { 0.1f, 1, 10, 100 };
            DoubleBuffered = true;
            mSelAxis = 0;
            mSelLevel = 0;
            mCircWidth = 256;
            Height = mCircWidth;
            Width = mCircWidth;
            mInRad = 35;
            mButtRad = mCircWidth / 2;
            mArchWidth = 20;
            mCenter = mCircWidth / 2;
            mButMin = mCenter - 60;
            mButMax = mCenter - 10;
            mButHomePos = 25;
            mArrowPos = (int)(mCenter + mInRad) / 2;

            Font = new Font("Arial", (float)(mArchWidth * 0.75), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            UpdateOffsets(mCircWidth);
            FrameColor = Color.RoyalBlue;
            mUnit = "mm";
            //UpdateBitmaps();
        }

        // d = diameter of big circle in pixels
        void UpdateOffsets(int d)
        {
            int w = global::UV_DLP_3D_Printer.Properties.Resources.mc_px0.Width;
            int h = global::UV_DLP_3D_Printer.Properties.Resources.mc_px0.Height;
            mOffsets[0] = new Rectangle((d - w) / 2, 0, w, h);
            mOffsets[1] = new Rectangle((d - w) / 2, -d, w, h);
            mOffsets[2] = new Rectangle((d - w) / 2 - d, -d, w, h);
            mOffsets[3] = new Rectangle((d - w) / 2 - d, 0, w, h);
        }

        protected override void UpdateBitmaps()
        {
            float vang = (float)(45.0 * Math.PI / 180.0);
            for (int i = 0; i < 4; i++)
            {
                float vlen = (float)(mInRad + mArchWidth / 2 + i * mArchWidth);
                mArchTxtPos[i] = new PointF((float)(mCenter - 16 + Math.Cos(vang) * vlen), (float)(mCenter - 10 - Math.Sin(vang) * vlen));
            }
            mArches[0] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px3, mLevelColors[0]);
            mArches[1] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px2, mLevelColors[1]);
            mArches[2] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px1, mLevelColors[2]);
            mArches[3] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px0, mLevelColors[3]);

            mArchesSel[0] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px3, mSelColor);
            mArchesSel[1] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px2, mSelColor);
            mArchesSel[2] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px1, mSelColor);
            mArchesSel[3] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_px0, mSelColor);
            // center
            mCentImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_cent, mLevelColors[3]);
            mCentImgSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_cent, mSelColor);
            // corners
            mYHomeImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_trcorner, mFrameColor);
            mYHomeImgSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_trcorner, mSelColor);
            mXHomeImg = C2DGraphics.RotateBmp90deg(mYHomeImg, 3);
            mXHomeImgSel = C2DGraphics.RotateBmp90deg(mYHomeImgSel, 3);
            Bitmap brCorner = C2DGraphics.RotateBmp90deg(global::UV_DLP_3D_Printer.Properties.Resources.mc_trcorner, 1);
            mAllHomeImg = C2DGraphics.ColorizeBitmapHQ(brCorner, mLevelColors[1]);
            mAllHomeImgSel = C2DGraphics.ColorizeBitmapHQ(brCorner, mSelColor);

            mHomeImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_home, mLevelColors[3]);
            mhomeImgCent = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_home, mFrameColor);
            // arrows
            mArrowU = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_uparrow, mArrowCol);
            mArrowR = C2DGraphics.RotateBmp90deg(mArrowU, 1);
            mArrowD = C2DGraphics.RotateBmp90deg(mArrowU, 2);
            mArrowL = C2DGraphics.RotateBmp90deg(mArrowU, 3);
        }

        // return correct arched image based on axis and level:
        // axis: 0=X 1=Y
        // level: 1 to 4 and -1 to -4: selected arch level where 1 and -1 are closer to the center
        Image GetArchImage(MachineControlAxis axis, int level)
        {
            if ((mSelAxis == axis) && (mSelLevel == level))
                return mArchesSel[4-Math.Abs(level)];
            return mArches[4-Math.Abs(level)];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics gr = e.Graphics;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicsState gs = gr.Save();

            gr.DrawImage(GetArchImage(MachineControlAxis.Y, 4), mOffsets[0]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, 3), mOffsets[0]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, 2), mOffsets[0]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, 1), mOffsets[0]);

            gr.RotateTransform(90);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, 4), mOffsets[1]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, 3), mOffsets[1]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, 2), mOffsets[1]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, 1), mOffsets[1]);

            gr.RotateTransform(90);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, -4), mOffsets[2]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, -3), mOffsets[2]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, -2), mOffsets[2]);
            gr.DrawImage(GetArchImage(MachineControlAxis.Y, -1), mOffsets[2]);

            gr.RotateTransform(90);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, -4), mOffsets[3]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, -3), mOffsets[3]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, -2), mOffsets[3]);
            gr.DrawImage(GetArchImage(MachineControlAxis.X, -1), mOffsets[3]);

            gr.Restore(gs);

            // text
            for (int i = 0; i < 4; i++)
            {
                DrawTextCentered(gr, mLevelVals[i].ToString(), mArchTxtPos[i].X, mArchTxtPos[i].Y, mLevelColors[i], true);
            }

            // center
            int lvl = Math.Abs(mSelLevel) - 1;
            if (lvl >= 0)
            {
                bool isPressed = (mSelAxis == mLastSelAxis) && (mSelLevel == mLastSelLevel);
                DrawImageCentered(gr, isPressed ? mCentImgSel : mCentImg, mCenter, mCenter);
                if ((lvl >= 0) && (lvl <= 3))
                {
                    DrawTextCentered(gr, mSelAxis.ToString(), mCenter, mCenter - Font.Height / 2, mFrameColor, true);
                    String txt = (mSelLevel < 0) ? "-" : "";
                    txt += mLevelVals[lvl].ToString();
                    DrawTextCentered(gr, txt, mCenter, mCenter + Font.Height / 2, mFrameColor, true);
                }
                if (lvl == 4)
                {
                    DrawImageCentered(gr, mhomeImgCent, mCenter, mCenter);
                    DrawTextCentered(gr, mSelAxis.ToString(), mCenter + 2, mCenter + 4, mLevelColors[3]);
                }
            }
            else
            {
                DrawTextCentered(gr, mUnit, mCenter, mCenter, mInvBackColor);
            }

            // corners
            bool issel = ((mSelAxis == MachineControlAxis.Y) && (mSelLevel == 5));
            Image cornImg = issel ? mYHomeImgSel : mYHomeImg;
            gr.DrawImage(cornImg, mCircWidth - mYHomeImg.Width, 0, mYHomeImg.Width, mYHomeImg.Height);
            DrawImageCentered(gr, mHomeImg, mCircWidth - mButHomePos, mButHomePos);
            DrawTextCentered(gr, "Y", mCircWidth - mButHomePos, mButHomePos + 4, issel ? mSelColor : mFrameColor);

            issel = ((mSelAxis == MachineControlAxis.X) && (mSelLevel == 5));
            cornImg = issel ? mXHomeImgSel : mXHomeImg;
            gr.DrawImage(cornImg, 0, 0, mYHomeImg.Width, mYHomeImg.Height);
            DrawImageCentered(gr, mHomeImg, mButHomePos, mButHomePos);
            DrawTextCentered(gr, "X", mButHomePos, mButHomePos + 4, issel ? mSelColor : mFrameColor);

            issel = ((mSelAxis == MachineControlAxis.All) && (mSelLevel == 5));
            cornImg = issel ? mAllHomeImgSel : mAllHomeImg;
            gr.DrawImage(cornImg, mCircWidth - mAllHomeImg.Width, mCircWidth - mAllHomeImg.Height, mAllHomeImg.Width, mAllHomeImg.Height);
            DrawImageCentered(gr, mHomeImg, mCircWidth - mButHomePos, mCircWidth - mButHomePos);
            DrawTextCentered(gr, "All", mCircWidth - mButHomePos + 2, mCircWidth - mButHomePos + 4, issel ? mSelColor : mFrameColor);

            // arrows
            Color atcol = Color.FromArgb(120, 0, 0, 0);
            DrawImageCentered(gr, mArrowU, mCenter, mCenter - mArrowPos);
            DrawTextCentered(gr, "+Y", mCenter, mCenter - mArrowPos, atcol);
            DrawImageCentered(gr, mArrowR, mCenter + mArrowPos, mCenter);
            DrawTextCentered(gr, "+X", mCenter + mArrowPos, mCenter, atcol);
            DrawImageCentered(gr, mArrowD, mCenter, mCenter + mArrowPos);
            DrawTextCentered(gr, "-Y", mCenter, mCenter + mArrowPos, atcol);
            DrawImageCentered(gr, mArrowL, mCenter - mArrowPos, mCenter);
            DrawTextCentered(gr, "-X", mCenter - mArrowPos, mCenter, atcol);

            // title
            if (mTitle != null)
            {
                DrawText(gr, mTitle, 5, mCircWidth - Font.Height, mInvBackColor, true); 
            }
        }

        protected int  GetSelection(int x, int y, out MachineControlAxis axis)
        {
            // first test corner buttons
            axis = MachineControlAxis.X;
            if (HitBitmap(x, y, mXHomeImg, 0, 0))
                return 5;
            axis = MachineControlAxis.Y;
            if (HitBitmap(x, y, mYHomeImg, mCircWidth - mYHomeImg.Width, 0))
                return 5;
            axis = MachineControlAxis.All;
            if (HitBitmap(x, y, mAllHomeImg, mCircWidth - mAllHomeImg.Width, mCircWidth - mAllHomeImg.Height))
                return 5;

            // test for arches
            int level;
            axis = mSelAxis;
            x = x - mCenter;
            y = mCenter - y;
            double rad = Math.Sqrt(x * x + y * y);
            rad = (rad - mInRad) / mArchWidth;
            if (rad < 0)
                return 0;
            level = (int)rad + 1;
            if (level > 4)
                return 0;

            // test angle to find quad
            double angle = Math.Atan2(y, x) * 180.0 / Math.PI + 45.0;
            if (angle < 0)
                angle += 360.0;
            int quad = (int)(angle / 90);
            angle = angle - quad * 90;
            if ((angle < 5) || (angle > 85))
                return 0;
            switch (quad)
            {
                case 0: axis = MachineControlAxis.X; break;
                case 1: axis = MachineControlAxis.Y; break;
                case 2: axis = MachineControlAxis.X; level = -level; break;
                case 3: axis = MachineControlAxis.Y; level = -level; break;
            }
            return level;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            MachineControlAxis axis;
            int level = GetSelection(e.X, e.Y, out axis);
            if ((level != mSelLevel) || (axis != mSelAxis))
            {
                mSelAxis = axis;
                mSelLevel = level;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            mLastSelAxis = mSelAxis;
            mLastSelLevel = mSelLevel;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            int lvl = Math.Abs(mSelLevel) - 1;
            if (lvl < 0)
                return;
            if ((mLastSelLevel == mSelLevel) && (mLastSelAxis == mSelAxis))
            {
                if ((lvl >=0) && (lvl <= 3) && (MotorMove != null))
                {
                    float res = mLevelVals[lvl];
                    if (mSelLevel < 0)
                        res = -res;
                    MotorMove(this, mSelAxis, res);
                }
                if ((lvl == 4) && (MotorHome != null))
                {
                    MotorHome(this, mSelAxis);
                }
            }
            mLastSelLevel = 0;
            Invalidate();
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            ForeColor = ct.ForeColor;
            FrameColor = ct.FrameColor;
        }
    }
}
