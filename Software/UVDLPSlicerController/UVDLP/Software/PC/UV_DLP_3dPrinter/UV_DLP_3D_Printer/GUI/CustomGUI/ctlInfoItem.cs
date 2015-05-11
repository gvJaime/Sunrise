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
    public partial class ctlInfoItem : ctlUserPanel
    {
        protected int mBorderWidth;
        protected int mTitleWidth;
        protected int mGap;

        public ctlInfoItem()
        {
            InitializeComponent();
            mBorderWidth = 1;
            mTitleWidth = 50;
            mGap = 1;
            PlaceElements();
        }
        public void FixForeColor(Color clr)
        {
            labelData.ForeColor = clr;
        }
        // get/set properties
        [DefaultValue(1)]
        [Description("Width of border arround text in pixels"), Category("Data")]
        public int BorderWidth
        {
            get { return mBorderWidth; }
            set { mBorderWidth = value; PlaceElements(); }
        }

        [DefaultValue(50)]
        [Description("Title text width as a precentage of the full width"), Category("Data")]
        public int TitleWidth
        {
            get { return mTitleWidth; }
            set { mTitleWidth = value; PlaceElements(); }
        }

        [DefaultValue(1)]
        [Description("Gap between title and data in pixels"), Category("Data")]
        public int Gap
        {
            get { return mGap; }
            set { mGap = value; PlaceElements(); }
        }

        [Description("Color of title text"), Category("Data")]
        public Color TitleColor
        {
            get { return labelTitle.ForeColor; }
            set { labelTitle.ForeColor = value; }
        }

        [Description("Color of title background"), Category("Data")]
        public Color TitleBackColor
        {
            get { return labelTitle.BackColor; }
            set { labelTitle.BackColor = value; }
        }

        [Description("Color of data text"), Category("Data")]
        public Color DataColor
        {
            get { return labelData.ForeColor; }
            set { labelData.ForeColor = value; }
        }

        [Description("Color of data background"), Category("Data")]
        public Color DataBackColor
        {
            get { return labelData.BackColor; }
            set { labelData.BackColor = value; }
        }

        [Description("Text of title"), Category("Data")]
        public String TitleText
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }

        [Description("Text of data"), Category("Data")]
        public String DataText
        {
            get { return labelData.Text; }
            set { labelData.Text = value; }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                labelTitle.Font = value;
                labelData.Font = value;
            }
        }


        protected void PlaceElements()
        {
            int w = Width;
            int h = Height;
            if ((w / h) < 3)
            {
                h = w / 3;
            }
            int bs = h - 2 * mBorderWidth;
            labelTitle.Location = new Point(mBorderWidth, mBorderWidth);
            int fullwidth = w - 2 * mBorderWidth - mGap;
            int tw = fullwidth * mTitleWidth / 100;
            labelTitle.Width = tw;
            labelTitle.Height = bs;
            labelData.Location = new Point(mBorderWidth + tw + mGap, mBorderWidth);
            labelData.Width = fullwidth - tw;
            labelData.Height = bs;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PlaceElements();
        }

        public override void ApplyStyle(GuiControlStyle ct)
        {
            base.ApplyStyle(ct);
            if (ct.BackColor.IsValid())
            {
                BackColor = ct.BackColor;
                labelTitle.BackColor = ct.BackColor;
            }
            if (ct.FrameColor.IsValid())
                labelData.BackColor = ct.FrameColor;
            if (ct.ForeColor.IsValid())
            {
                labelData.ForeColor = ct.ForeColor;
                labelTitle.ForeColor = ct.ForeColor;
            }
        }
    }
}
