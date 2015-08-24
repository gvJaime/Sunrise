using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class ctlRFrame : UserControl
    {
        Color mInvBackColor;

        public ctlRFrame()
        {
            mInvBackColor = Color.Black;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics gr = e.Graphics;
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Color fcol = Color.FromArgb(90, mInvBackColor);
            Pen pen = new Pen(fcol, 2);
            C2DGraphics.DrawRoundRectangle(gr, pen, 0, 0, Width - 1, Height - 1, 5);
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
    }
}
