using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public class ctlGL : GLControl
    {
        public delegate void delPaint();
        public event delPaint PaintCallback;
        public ctlGL() : base (new GraphicsMode(OpenTK.Graphics.GraphicsMode.Default.ColorFormat,
                OpenTK.Graphics.GraphicsMode.Default.Depth, 8))
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (PaintCallback != null)
                PaintCallback();
        }
    }
}
