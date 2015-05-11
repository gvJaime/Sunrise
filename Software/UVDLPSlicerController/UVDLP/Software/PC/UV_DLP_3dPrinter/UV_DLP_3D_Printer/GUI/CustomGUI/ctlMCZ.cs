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
    public partial class ctlMCZ : ctlMCBase
    {
        Image[] mSteps;
        Image[] mStepsSel;
        Image mCentImg, mCentImgSel;
        Image mTopImg, mTopImgSel;
        Image mHomeImg, mhomeImgCent;
        Image mArrowU, mArrowD;
        Rectangle[] mOffsets;
        int mSelLevel, mLastSelLevel;
        int mCenterX, mCenterY;
        int mButHomeCent, mButHomeX, mButHomeY;
        int mArrowPos;
        int mStepStart;
        int mStepHeight;
        int mCtlWidth;
        int mCtlHeight;
        String mAxisSign;
        bool mHasHome;
        MachineControlAxis mAxis;

        public event MotorMoveDelegate MotorMove;
        public event MotorHomeDelegate MotorHome;

        public ctlMCZ()
        {
            InitializeComponent();
            mSteps = new Image[4];
            mStepsSel = new Image[4];
            mOffsets = new Rectangle[8];
            mLevelVals = new float[] { 0.1f, 1, 10, 50 };
            DoubleBuffered = true;
            mSelLevel = 0;
            mCtlHeight = 256;
            mCtlWidth = 70;
            Height = mCtlHeight;
            Width = mCtlWidth;
            mStepStart = 10;
            mStepHeight = 20;
            mCenterY = (mCtlHeight + mStepHeight) / 2;
            mCenterX = mCtlWidth / 2;
            mArrowPos = mStepStart + (mStepHeight * 7) / 2;
            Font = new Font("Arial", (float)(mStepHeight * 0.75), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            FrameColor = Color.RoyalBlue;
            UpdateOffsets();
            mAxisSign = "Z";
            mUnit = "mm";
            mHasHome = true;
            mAxis = MachineControlAxis.Z;
            mLastSelLevel = 0;
        }

        [DefaultValue(MachineControlAxis.Z)]
        [Description("Axis enum"), Category("Data")]
        public MachineControlAxis AxisValue 
        {
            get { return mAxis; }
            set { mAxis = value; }
        }

        [DefaultValue("Z")]
        [Description("Axis sign"), Category("Data")]
        public String AxisSign
        {
            get { return mAxisSign; }
            set { mAxisSign = value; Invalidate(); }
        }

        [DefaultValue(true)]
        [Description("Does this axis have home button"), Category("Data")]
        public bool HasHome
        {
            get { return mHasHome; }
            set { mHasHome = value; Invalidate(); }
        }

        // d = height of control in pixels
        void UpdateOffsets()
        {
            int w = global::UV_DLP_3D_Printer.Properties.Resources.mc_z0.Width;
            mButHomeCent = mCenterY - mStepStart - 4 * mStepHeight - mTopImg.Height / 2;
            mButHomeX = mCenterX - mTopImg.Width / 2;
            mButHomeY = mButHomeCent - mTopImg.Height / 2;
            int offs = mStepStart - 5;
            for (int i = 0; i < 4; i++)
            {
                mOffsets[i] = new Rectangle((mCtlWidth - w) / 2, mCenterY - offs - mSteps[i].Height, mSteps[i].Width, mSteps[i].Height);
                mOffsets[i + 4] = new Rectangle((mCtlWidth - w) / 2, mCenterY + offs, mSteps[i].Width, mSteps[i].Height);
            }

        }

        protected override void UpdateBitmaps()
        {
            mSteps[0] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z3, mLevelColors[0]);
            mSteps[1] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z2, mLevelColors[1]);
            mSteps[2] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z1, mLevelColors[2]);
            mSteps[3] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z0, mLevelColors[3]);

            mStepsSel[0] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z3, mSelColor);
            mStepsSel[1] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z2, mSelColor);
            mStepsSel[2] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z1, mSelColor);
            mStepsSel[3] = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_z0, mSelColor);
            // center
            mCentImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_centn, mLevelColors[3]);
            mCentImgSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_centn, mSelColor);
            // home
            mTopImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_ztop, mFrameColor);
            mTopImgSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_ztop, mSelColor);
            mHomeImg = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_home, mLevelColors[3]);
            mhomeImgCent = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_home, mFrameColor);
            // arrows
            mArrowU = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_uparrown, mArrowCol);
            mArrowD = C2DGraphics.RotateBmp90deg(mArrowU, 2);
        }

        // return correct arched image based on level:
        // level: 1 to 4 and -1 to -4: selected level where 1 and -1 are closer to the center
        Image GetStepImage(int level)
        {
            if (mSelLevel == level)
                return mStepsSel[4 - Math.Abs(level)];
            return mSteps[4 - Math.Abs(level)];
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics gr = e.Graphics;
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            for (int i = 0; i < 4; i++)
            {
                gr.DrawImage(GetStepImage(4 - i), mOffsets[i]);
                gr.DrawImage(GetStepImage(-4 + i), mOffsets[i + 4]);
            }

            // text
            int txtStart = mCenterY - mStepStart - mStepHeight / 2;
            for (int i = 0; i < 3; i++)
            {
                DrawTextCentered(gr, mLevelVals[i].ToString(), mCenterX, txtStart - i * mStepHeight, mLevelColors[i], true);
            }

            // center
            int lvl = Math.Abs(mSelLevel) - 1;
            if (lvl >= 0)
            {
                bool isPressed = (mSelLevel == mLastSelLevel);
                DrawImageCentered(gr, isPressed ? mCentImgSel : mCentImg, mCenterX, mCenterY);
                if ((lvl >= 0) && (lvl <= 3))
                {
                    String txt = (mSelLevel < 0) ? "-" : "";
                    txt += mLevelVals[lvl].ToString();
                    DrawTextCentered(gr, txt, mCenterX, mCenterY, mFrameColor, true);
                }
                if (lvl == 4)
                {
                    DrawImageCentered(gr, mhomeImgCent, mCenterX, mCenterY);
                    DrawTextCentered(gr, mAxisSign, mCenterX, mCenterY + 4, mLevelColors[3]);
                }
            }
            else
            {
                DrawTextCentered(gr, mUnit, mCenterX, mCenterY, mInvBackColor);
            }

            // top
            bool issel = (mSelLevel == 5);
            Image topImg = issel ? mTopImgSel : mTopImg;
            DrawImageCentered(gr, topImg, mCenterX, mButHomeCent);
            if (mHasHome)
            {
                DrawImageCentered(gr, mHomeImg, mCenterX, mButHomeCent);
                DrawTextCentered(gr, mAxisSign, mCenterX, mButHomeCent + 4, issel ? mSelColor : mFrameColor);
            }

            // arrows
            Color atcol = Color.FromArgb(120, 0, 0, 0);
            DrawImageCentered(gr, mArrowU, mCenterX, mCenterY - mArrowPos);
            DrawTextCentered(gr, "+" + mAxisSign, mCenterX, mCenterY - mArrowPos, atcol);
            DrawImageCentered(gr, mArrowD, mCenterX, mCenterY + mArrowPos);
            DrawTextCentered(gr, "-" + mAxisSign, mCenterX, mCenterY + mArrowPos, atcol);

            // title
            if (mTitle != null)
            {
                DrawTextCentered(gr, mTitle, mCenterX, mCtlHeight - Font.Height / 2, mInvBackColor, true);
            }
        }

        protected int GetSelection(int x, int y)
        {
            int level;

            // test for home button
            if (mHasHome && HitBitmap(x, y, mTopImg, mButHomeX, mButHomeY))
                return 5;

            x -= mCenterX;
            y = mCenterY - y;
            int rad = (Math.Abs(y) - mStepStart) / mStepHeight;
            if (rad < 0)
                return 0;
            level = (int)rad + 1;
            //if (level > 3)
            //    return 0;
            if (level > 4) // allow the topmost level to be selected - SMH
                return 0;

            int hw = mSteps[0].Width / 2;
            if ((x < -hw) || (x > hw))
                return 0;

            if (y < 0)
                level = -level;
            return level;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int level = GetSelection(e.X, e.Y);
            if (level != mSelLevel)
            {
                mSelLevel = level;
                Invalidate();
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
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
            if (mLastSelLevel == mSelLevel)
            {
                if ((lvl >= 0) && (lvl <= 3) && (MotorMove != null))
                {
                    float res = mLevelVals[lvl];
                    if (mSelLevel < 0)
                        res = -res;
                    MotorMove(this, mAxis, res);
                }
                if ((lvl == 4) && (MotorHome != null))
                {
                    MotorHome(this, mAxis);
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
