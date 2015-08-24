using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class ctlProgress : Label
    {
        protected int mMinVal, mMaxVal;
        protected Color mBarColor;
        protected int mBorderThickness;
        protected int mValue;


        // will apear in properties panel
        [Description("Minimum progress value"), Category("Data")]
        public int Minimum
        {
            get { return mMinVal; }
            set { mMinVal = value; Invalidate(); }
        }

        [Description("Maximum progress value"), Category("Data")]
        public int Maximum
        {
            get { return mMaxVal; }
            set { mMaxVal = value; Invalidate(); }
        }

        [Description("Minimum progress value"), Category("Data")]
        public Color BarColor
        {
            get { return mBarColor; }
            set { mBarColor = value; Invalidate(); }
        }

        [Description("Progress value"), Category("Data")]
        public int Value
        {
            get { return mValue; }
            set { mValue = value; Invalidate(); }
        }

        [Description("Border thickness"), Category("Data")]
        public int BorderThickness
        {
            get { return mBorderThickness; }
            set { mBorderThickness = value; Invalidate(); }
        }

        public ctlProgress()
        {
            mMinVal = 0;
            mMaxVal = 100;
            mBorderThickness = 2;
            mValue = 0;
            mBarColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int val = mValue;
            if (val < mMinVal)
                val = mMinVal;
            else if (val > mMaxVal)
                val = mMaxVal;
            if (val > mMinVal)
            {
                int plen = (val - mMinVal) * (Width - 2 * mBorderThickness) / (mMaxVal - mMinVal);
                Rectangle rc = new Rectangle(mBorderThickness, mBorderThickness, plen, Height - 2 * mBorderThickness);
                Brush br = new SolidBrush(mBarColor);
                e.Graphics.FillRectangle(br, rc);
            }
            base.OnPaint(e);
        }
    }
}
