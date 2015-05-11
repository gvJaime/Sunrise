using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using UV_DLP_3D_Printer._3DEngine;

namespace UV_DLP_3D_Printer.GUI.CustomGUI
{
    public class C9Patch
    {
        Bitmap image;
        int x1, x2;
        int y1, y2;
        int minx, miny;
        Rectangle[,] srcrc = new Rectangle[3, 3];
        Rectangle[,] dstrc = new Rectangle[3, 3];
        public C9Patch(Bitmap img)
        {
            // try detecting green guide
            int i;
            bool haveguide = true;
            x1 = -1; 
            x2 = -1;
            for (i = 0; i < img.Width; i++)
            {
                Color pix = img.GetPixel(i, 0);
                if (pix.A == 0)
                {
                    if ((x1 >= 0) && (x2 < 0))
                        x2 = img.Width - i;
                    continue;
                }
                if (pix == Color.Green)
                {
                    if (x1 < 0)
                        x1 = i - 1;
                    continue;
                }
                haveguide = false;
                break;
            }
            if (haveguide)
            {
                y1 = -1;
                y2 = -1;
                for (i = 0; i < img.Height; i++)
                {
                    Color pix = img.GetPixel(0, i);
                    if (pix.A == 0)
                    {
                        if ((y1 >= 0) && (y2 < 0))
                            y2 = img.Height - i;
                        continue;
                    }
                    if (pix == Color.Green)
                    {
                        if (y1 < 0)
                            y1 = i - 1;
                        continue;
                    }
                    haveguide = false;
                    break;
                }
            }
            if (haveguide && (x1 > 0) && (x2 > 0) && (y1 > 0) && (y2 > 0))
            {
                image = img.Clone(new Rectangle(1, 1, img.Width - 1, img.Height - 1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                image = img;
                x1 = x2 = img.Width / 3;
                y1 = y2 = img.Height / 3;
             }
            CalcParameters();
        }

        void CalcParameters()
        {
            minx = x1 + x2 + 2;
            miny = y1 + y2 + 2;
            int px2 = image.Width - x2;
            int py2 = image.Height - y2;
            int cw = px2 - x1;
            int ch = py2 - y1;
            srcrc[0, 0] = new Rectangle(0, 0, x1, y1);
            srcrc[1, 0] = new Rectangle(x1, 0, cw, y1);
            srcrc[2, 0] = new Rectangle(px2, 0, x2, y1);
            srcrc[0, 1] = new Rectangle(0, y1, x1, ch);
            srcrc[1, 1] = new Rectangle(x1, y1, cw, ch);
            srcrc[2, 1] = new Rectangle(px2, y1, x2, ch);
            srcrc[0, 2] = new Rectangle(0, py2, x1, y2);
            srcrc[1, 2] = new Rectangle(x1, py2, cw, y2);
            srcrc[2, 2] = new Rectangle(px2, py2, x2, y2);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    dstrc[i, j] = new Rectangle(srcrc[i,j].Location, srcrc[i,j].Size);
        }

        void AdjustDest(Rectangle destrc)
        {
            for (int i = 0; i < 3; i++)
            {
                dstrc[0, i].X = srcrc[0, i].X + destrc.X;
                dstrc[1, i].X = srcrc[1, i].X + destrc.X;
                dstrc[2, i].X = destrc.Width - x2 + destrc.X;
                dstrc[1, i].Width = destrc.Width - x2 - x1;

                dstrc[i, 0].Y = srcrc[i, 0].Y + destrc.Y;
                dstrc[i, 1].Y = srcrc[i, 1].Y + destrc.Y;
                dstrc[i, 2].Y = destrc.Height - y2 + destrc.Y;
                dstrc[i, 1].Height = destrc.Height - y2 - y1;
            }
        }

        public void Draw(Graphics gr, Rectangle destrc, Color col)
        {
            Bitmap img;
            if (col == Color.White)
                img = image;
            else
                img = C2DGraphics.ColorizeBitmap(image, col);
            if ((destrc.Width < minx) || (destrc.Height < miny))
                gr.DrawImage(img, destrc);
            else
            {
                // do the actual 9 patch.
                AdjustDest(destrc);
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        gr.DrawImage(img, dstrc[i,j], srcrc[i,j], GraphicsUnit.Pixel);
            }
        }
    }
}
