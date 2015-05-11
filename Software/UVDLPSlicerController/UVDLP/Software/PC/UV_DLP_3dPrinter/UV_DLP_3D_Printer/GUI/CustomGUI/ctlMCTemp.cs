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
    public partial class ctlMCTemp : ctlMCBase
    {
        Image mTempBgnd;
        Image mTempPoint, mTempPointSel;
        bool mOverPointer;
        int mCenterX, mCenterY;
        int mTMinPos, mTMaxPos;
        int mTCurrent, mTCurrentPos;
        int mTSet, mTSetPos, mTSetLast;
        int mCtlWidth;
        int mCtlHeight;
        int mBgndPos;
        int mBottomPos;
        int mTMin, mTMax;
        int mPointOffs;
        String mTempSign;

        public delegate void SetTempChangedDelegate(Object obj, int newTemp);
        public event SetTempChangedDelegate SetTempChanged;

        public ctlMCTemp()
        {
            InitializeComponent();
            DoubleBuffered = true;
            mCtlHeight = 256;
            mCtlWidth = 70;
            Height = mCtlHeight;
            Width = mCtlWidth;
            mCenterY = mCtlHeight / 2;
            mCenterX = mCtlWidth / 2;
            mTCurrent = 25;
            mTSet = 225;
            mTMin = 0;
            mTMax = 300;
            Font = new Font("Arial", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            mTempBgnd = global::UV_DLP_3D_Printer.Properties.Resources.mc_temper;
            mBgndPos = 20;
            mTMinPos = mBgndPos + mTempBgnd.Height - 36;
            mTMaxPos = mBgndPos + 20;
            mBottomPos = mBgndPos + mTempBgnd.Height - 16;
            UpdatePositions();
            FrameColor = Color.RoyalBlue;
            textSetTemp.Text = mTSet.ToString();
            mTempSign = "°";
            mUnit = "C";
            mOverPointer = false;
            mPointOffs = 9999;
        }

        [DefaultValue(300)]
        [Description("Maximum setable temperature"), Category("Data")]
        public int MaxTemp
        {
            get { return mTMax; }
            set { mTMax = value; UpdatePositions(); }
        }
        [DefaultValue(0)]
        [Description("Minimum setable temperature"), Category("Data")]
        public int MinTemp
        {
            get { return mTMin; }
            set { mTMin = value; UpdatePositions(); }
        }
        [DefaultValue(25)]
        [Description("Current measured temperature"), Category("Data")]
        public int CurrentTemp
        {
            get { return mTCurrent; }
            set { mTCurrent = value; UpdatePositions(); }
        }
        
        void UpdatePositions()
        {
            textSetTemp.MinInt = mTMin;
            textSetTemp.MaxInt = mTMax;
            mTSetPos = TempToPos(mTSet);
            mTCurrentPos = TempToPos(mTCurrent);
            Invalidate();
        }

        int TempToPos(int t)
        {
            return mTMinPos - ((mTMinPos - mTMaxPos) * (t - mTMin)) / (mTMax - mTMin);
        }

        int PosToTemp(int p)
        {
            return  mTMin + ((mTMinPos - p) * (mTMax - mTMin)) / (mTMinPos - mTMaxPos);
        }

        protected override void UpdateBitmaps()
        {
            mTempPoint = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_tpoint, mFrameColor);
            mTempPointSel = C2DGraphics.ColorizeBitmapHQ(global::UV_DLP_3D_Printer.Properties.Resources.mc_tpoint, mSelColor);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gr = e.Graphics;
            Brush br = new SolidBrush(Color.Red);
            gr.FillRectangle(br, mCenterX - 10, mTCurrentPos, 20, mBottomPos - mTCurrentPos);
            gr.DrawImage(mTempBgnd, mCenterX - mTempBgnd.Width / 2, mBgndPos, mTempBgnd.Width, mTempBgnd.Height);
            Image ptrImg = mOverPointer ? mTempPointSel : mTempPoint;
            DrawImageCentered(gr, ptrImg, mCenterX, mTSetPos);
            String temp = mTCurrent.ToString() + mTempSign + mUnit;
            DrawTextCentered(gr, temp, mCenterX, 16, mInvBackColor, true);

            // title
            if (mTitle != null)
            {
                DrawTextCentered(gr, mTitle, mCenterX, mCtlHeight - Font.Height / 2, mInvBackColor, true);
            }
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            ForeColor = ct.ForeColor;
            FrameColor = ct.FrameColor;
            textSetTemp.BackColor = ct.FrameColor;
            textSetTemp.ForeColor = ct.ForeColor;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mPointOffs != 9999)
            {
                int newpos = e.Y + mPointOffs;
                if (newpos > mTMinPos)
                    newpos = mTMinPos;
                if (newpos < mTMaxPos)
                    newpos = mTMaxPos;
                if (newpos != mTSetPos)
                {
                    mTSetPos = newpos;
                    mTSet = PosToTemp(mTSetPos);
                    textSetTemp.Text = mTSet.ToString();
                    Invalidate();
                }
            }
            else
            {
                bool overPtr = HitBitmap(e.X, e.Y, mTempPoint, mCenterX - mTempPoint.Width / 2, mTSetPos - mTempPoint.Height / 2);
                if (overPtr != mOverPointer)
                {
                    mOverPointer = overPtr;
                    mTSetLast = mTSet;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (mOverPointer)
            {
                mPointOffs = mTSetPos - e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            mPointOffs = 9999;
            if ((mTSetLast != mTSet) && (SetTempChanged != null))
            {
                SetTempChanged(this, mTSet);
            }
        }

        private void textSetTemp_TextChanged(object sender, EventArgs e)
        {
            if (textSetTemp.Valid)
            {
                mTSet = textSetTemp.IntVal;
                mTSetPos = TempToPos(mTSet);
                if (SetTempChanged != null)
                    SetTempChanged(this, mTSet);
                Invalidate();
            }
        }

    }
}
