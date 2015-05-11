using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine3D;
using UV_DLP_3D_Printer;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using UV_DLP_3D_Printer._3DEngine;
using System.Drawing;
namespace UV_DLP_3D_Printer._3DEngine
{
    /// <summary>
    /// This class can take an Object3d, and render it into an off-screen buffer
    /// This class should be able to automatically set up the camera as well to a relative distance away
    /// these previews are then used in the Load file dialog, and exported into the /preview directory
    /// </summary>
    public class PreviewGenerator
    {
        public Color BackColor;
        public Color SceneColor;
        public ePreview ViewAngle;
        public float Scale;
        OpenTK.Matrix4 projection;

        public enum ePreview 
        {
            None,
            Top,
            Bottom,
            Front,
            Back,
            Right,
            Left,
            Isometric
        }

        public PreviewGenerator() 
        {
            BackColor = Color.White;
            SceneColor = Color.FromArgb(129,129,129);
            ViewAngle = ePreview.Front;
            Scale = 0.2f;
        }
        /// <summary>
        /// This function will take an object, move the camera a distance away and generate the preview
        /// </summary>
        /// <param name="xsize"></param>
        /// <param name="ysize"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Bitmap GeneratePreview(int xsize, int ysize, List<Object3d> objs) 
        {
            if (ViewAngle == ePreview.None)
                return null;

            try
            {
                // taken from http://www.opentk.com/doc/graphics/frame-buffer-objects
                // more good examples here: http://www.opentk.com/node/1642?page=1

                // find scene extents
                Point3d minext = new Point3d(999999,999999,999999);
                Point3d maxext = new Point3d(-999999,-999999,-999999);
                foreach (Object3d obj in objs)
                {
                    minext.x = Math.Min(obj.m_min.x, minext.x);
                    minext.y = Math.Min(obj.m_min.y, minext.y);
                    minext.z = Math.Min(obj.m_min.z, minext.z);
                    maxext.x = Math.Max(obj.m_max.x, maxext.x);
                    maxext.y = Math.Max(obj.m_max.y, maxext.y);
                    maxext.z = Math.Max(obj.m_max.z, maxext.z);
                }

                int FboWidth = xsize;
                int FboHeight = ysize;

                uint FboHandle;
                uint ColorTexture;
                uint DepthRenderbuffer;
                GLCamera previewcamera = new GLCamera();

                
                //OpenTK.Matrix4 projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(0.55f, (float)xsize / (float)ysize, 1, 2000);


                // Create Color Texture
                GL.GenTextures(1, out ColorTexture);
                GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, FboWidth, FboHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

                // test for GL Error here (might be unsupported format)

                GL.BindTexture(TextureTarget.Texture2D, 0); // prevent feedback, reading and writing to the same image is a bad idea

                // Create Depth Renderbuffer
                GL.Ext.GenRenderbuffers(1, out DepthRenderbuffer);
                GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);
                GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent32, FboWidth, FboHeight);

                // test for GL Error here (might be unsupported format)

                // Create a FBO and attach the textures
                GL.Ext.GenFramebuffers(1, out FboHandle);
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
                GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
                GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);

                // now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

                // since there's only 1 Color buffer attached this is not explicitly required
                GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

                GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
                GL.Viewport(0, 0, FboWidth, FboHeight);

                // render whatever your heart desires, when done ...
                // clear buffer
                //GL.ClearColor(Color.White);
                // clear the screen, to make it very obvious what the clear affected. only the FBO, not the real framebuffer
                GL.ClearColor(BackColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //set up camera for the specified view
                float bmpaspect = (float)xsize / (float)ysize;
                switch (ViewAngle)
                {
                    case ePreview.Front:
                        SetProjection(bmpaspect, minext.x, maxext.x, minext.z, maxext.z);
                        previewcamera.ResetView(0, -50, 0, 0, 0);
                        break;
                    case ePreview.Back:
                        SetProjection(bmpaspect, -maxext.x, -minext.x, minext.z, maxext.z);
                        previewcamera.ResetView(0, 50, 0, 0, 0);
                        break;
                    case ePreview.Top:
                        SetProjection(bmpaspect, minext.x, maxext.x, minext.y, maxext.y);
                        previewcamera.ResetView(0, -50, 0, 90, 0);
                        break;
                    case ePreview.Bottom:
                        SetProjection(bmpaspect, minext.x, maxext.x, -maxext.y, -minext.y);
                        previewcamera.ResetView(0, -50, 0, -90, 0);
                        break;
                    case ePreview.Right:
                        SetProjection(bmpaspect, minext.y, maxext.y, minext.z, maxext.z);
                        previewcamera.ResetView(50, 0, 0, 0, 0);
                        break;
                    case ePreview.Left:
                        SetProjection(bmpaspect, -maxext.y, -minext.y, minext.z, maxext.z);
                        previewcamera.ResetView(-50,0 , 0, 0, 0);
                       break;
                    case ePreview.Isometric:
                        float oldscale = Scale;
                        Scale = ((Scale +1f) * 1.4f) - 1f;
                        SetProjection(bmpaspect, minext.x, maxext.x, minext.z, maxext.z);
                        Scale = oldscale;
                        previewcamera.ResetView(50, -50, 0, 30, 0);
                        break;
                }


                //previewcamera.ResetView(0, -200, 0, 20, 20);
                
                //previewcamera.ResetView(0, -200, obj.m_radius/2, 0, 00);
                // setup projection matrix
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);
                GL.MatrixMode(MatrixMode.Modelview);
                previewcamera.SetViewGL();
                //render scene
                foreach (Object3d obj in objs)
                {
                    obj.InvalidateList();
                    obj.RenderGL(false, false, false, SceneColor);
                    obj.InvalidateList();
                }
                //copy the framebuffer to a bitmap
                Bitmap bmppreview = GetBitmap(xsize, ysize);
                GL.PopAttrib(); // restores GL.Viewport() parameters
                GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
                GL.DrawBuffer(DrawBufferMode.Back);
                return bmppreview;
            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex);
                return null;
            }
        }

        public Bitmap GeneratePreview(int xsize, int ysize, Object3d obj)
        {
            List<Object3d> objs = new List<Object3d>();
            objs.Add(obj);
            return GeneratePreview(xsize, ysize, objs);
        }

        // generate preview from scene excluding supports
        public Bitmap GeneratePreview(int xsize, int ysize)
        {
            List<Object3d> objs = new List<Object3d>();
            foreach (Object3d obj in UVDLPApp.Instance().m_engine3d.m_objects)
            {
                if (obj.tag == Object3d.OBJ_NORMAL)
                    objs.Add(obj);
            }
            return GeneratePreview(xsize, ysize, objs);
        }
       

        void SetProjection(float bmpaspect, float minx, float maxx, float miny, float maxy)
        {
            float width = maxx - minx;
            float height = maxy - miny;
            maxx += width * Scale;
            minx -= width * Scale;
            maxy += height * Scale;
            miny -= height * Scale;
            float sceneaspect = width / height;
            if (sceneaspect < bmpaspect)
            {
                // tall image
                width = ((maxy - miny) * bmpaspect - (maxx - minx)) / 2;
                minx -= width;
                maxx += width;
                
                /*float scale2 = bmpaspect / sceneaspect;
                minx *= scale2;
                maxx *= scale2;*/
            }
            else
            {
                // wide image
                height = ((maxx - minx) / bmpaspect - (maxy - miny)) / 2;
                miny -= height;
                maxy += height;
                /*float scale2 = sceneaspect / bmpaspect;
                miny *= scale2;
                maxy *= scale2;*/
            }
            projection = OpenTK.Matrix4.CreateOrthographicOffCenter(minx, maxx, miny, maxy, 1, 2000);
       }

        private Bitmap GetBitmap(int width, int height)
        {
            System.Drawing.Bitmap bitmap = null;

            bitmap = new System.Drawing.Bitmap(width, height);
            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            
            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);
            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }
    }
}
