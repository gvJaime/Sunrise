using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// This control shows text on the left, and a number control on the right.
namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public partial class ctlParameter : ctlMCBase
    {
        int mTextX, mTextY;
        int mParamWidth;
        public delegate void ValueChangedDelegate(Object sender, decimal newval);
        public event ValueChangedDelegate ValueChanged;
        public ctlParameter()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Font = new Font("Arial", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            mTextX = 5;
            mParamWidth = 80;
            FrameColor = Color.RoyalBlue;
        }

        [DefaultValue(0)]
        [Description("Parameter value"), Category("Data")]
        public decimal Value
        {
            get { return ctlParam.Value; }
            set { ctlParam.Value = value; }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            mTextY = (Height - Font.Height) / 2;
            ctlParam.Width = mParamWidth;
            //ctlParam.Height = Height - 8;
            ctlParam.Font = new Font("Arial", (float)Height / 2.5f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            ctlParam.Location = new Point(Width - mParamWidth - 4, (Height - ctlParam.Height) / 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics gr = e.Graphics;
            if (mTitle != null)
                DrawText(gr, mTitle, mTextX, mTextY, mInvBackColor, true);
        }

        public NumericUpDown Parameter
        {
            get { return ctlParam; }
        }

        private void ctlParam_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(sender, ctlParam.Value);
            }
        }
    }
}
