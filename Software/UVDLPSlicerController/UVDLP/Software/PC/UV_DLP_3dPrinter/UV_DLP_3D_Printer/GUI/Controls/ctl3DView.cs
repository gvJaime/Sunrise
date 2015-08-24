using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer.GUI.CustomGUI;
using Engine3D;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using UV_DLP_3D_Printer._3DEngine;
using UV_DLP_3D_Printer.Plugin;
using UV_DLP_3D_Printer.Device_Interface;
using UV_DLP_3D_Printer.Slicing;

namespace UV_DLP_3D_Printer.GUI.Controls
{
    public partial class ctl3DView : UserControl
    {

        bool m_movingobjectmode = false; // for moving objects while the shift key is held down
        ctlImageButton m_pressedButt = null;
        Control m_selectedControl = null;
        Timer m_modelAnimTmr;
        public GLCamera m_camera;
        public GLCamera m_axisCam;
        bool loaded = false;
        //float m_ix = 1.0f, m_iy = 1.0f, m_iz = 2.0f;
        Engine3D.Vector3d m_isectnormal; // the normal at the intersection
        Slice m_curslice = null; // for previewing only

        private bool lmdown, rmdown, mmdown; // flags for mouse buttons down
        private int mdx, mdy; // delta mouse movements

        IGraphicsContext m_context = null;
        OpenTK.Matrix4 m_projection;
        OpenTK.Matrix4 m_axisProjection;
        OpenTK.Matrix4 m_modelView;

        private bool firstTime = true;
        float m_savex, m_savey, m_saveh; // m_savez
        C2DGraphics gr2d;
        GuiConfigManager guiconf;
        public List<ctlBgnd> ctlBgndList;
        int m_sliceTex;
        int m_sliceViewW, m_sliceViewH;
        int m_sliceW, m_sliceH;
        bool ctrldown;// is the control key held down?
        bool Render3dSpace = true;
        uint mColorBuffer = 0;
        SliceFile m_sf = null;

        //ctlImageButton imbtn;
        

        public delegate void On3dViewRedraw();
        public event On3dViewRedraw Event3DViewRedraw;

        public Form m_splash = null;

        public ctl3DView()
        {
            InitializeComponent();
            //Visible = false;
            //SetupSceneTree();
            m_modelAnimTmr = null;
            m_camera = new GLCamera();
            m_axisCam = new GLCamera();
            ResetCameraView();
            m_isectnormal = new Engine3D.Vector3d();

            mainViewSplitContainer.Panel1Collapsed = true;

            UVDLPApp.Instance().m_undoer.parentControl = this;
            UVDLPApp.Instance().m_undoer.AsociateUndoButton(buttUndo);
            UVDLPApp.Instance().m_undoer.AsociateRedoButton(buttRedo);

            //glControl1. = new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, 8);
            gr2d = UVDLPApp.Instance().m_2d_graphics;
            ctlBgndList = new List<ctlBgnd>();
            guiconf = UVDLPApp.Instance().m_gui_config; // set from the main program GUIConfig
            guiconf.TopLevelControl = mainViewSplitContainer.Panel2;
            UpdateButtonList();
            RearrangeGui(); // once the GUIConfig is loaded from the plugins and from the main GUIConfig, the screen is re-arranged

            // toplevel controls must point to this

            glControl1.PaintCallback += new ctlGL.delPaint(DisplayFunc);            

            m_sliceTex = -1;
            RegisterCallbacks();

            UVDLPApp.Instance().m_slicer.Slice_Event += new Slicer.SliceEvent(SliceEv);
            UVDLPApp.Instance().AppEvent += new AppEventDelegate(AppEventDel);
        }


        private void AppEventDel(eAppEvent ev, String Message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { AppEventDel(ev, Message); }));
            }
            else
            {
                switch (ev)
                {
                    case eAppEvent.eSlicedLoaded: // update the gui to view
                        if (UVDLPApp.Instance().m_slicefile != null)
                        {
                            int totallayers = UVDLPApp.Instance().m_slicefile.NumSlices;
                            SetNumLayers(totallayers);
                        }
                        break;
                    case eAppEvent.eObjectSelected:
                        // set the current sel plane object
                        if (UVDLPApp.Instance().SelectedObject != null)
                        {
                            RTUtils.UpdateObjectSelectionPlane(UVDLPApp.Instance().SelectedObject.m_center,m_camera.m_right, m_camera.m_up);
                        }

                        break;
                }
            }

        }
        private void SliceEv(Slicer.eSliceEvent ev, int layer, int totallayers, SliceFile sf)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate() { SliceEv(ev, layer, totallayers, sf); }));
                }
                else
                {
                    switch (ev)
                    {
                        case Slicer.eSliceEvent.eSliceCompleted:
                            SetNumLayers(totallayers);
                            m_sf = sf;
                            this.Update();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        protected void RegisterCallbacks() 
        {
            CallbackHandler cb = UVDLPApp.Instance().m_callbackhandler;              
        }

        public GuiConfigManager GuiConfig
        {
            get { return guiconf; }
        }

        public C2DGraphics Graphics2D
        {
            get { return gr2d; }
        }

        public void RearrangeGui()
        {
            if (guiconf != null)
                guiconf.LayoutGui(Width, Height);
            UpdateView(true);
        }


        public void Enable3dView(bool isEnable)
        {
            glControl1.Enabled = isEnable;
            glControl1.Visible = isEnable;
        }

        public void SetNumLayers(int val)
        {
            if (val < 0)
                val = 0;
            if (UVDLPApp.Instance().m_appconfig.m_viewslice3d && (val > 0))
            {
                numLayer.MaxInt = scrollLayer.Maximum = val;
                numLayer.IntVal = 1;
                numLayer.Visible = true;
                ctlInfoItemZLev.Visible = true;
                scrollLayer.Visible = true;
            }
            else
            {
                numLayer.Visible = false;
                ctlInfoItemZLev.Visible = false;                
                scrollLayer.Visible = false;
            }
            ViewLayer(0);
        }

        
        #region GL Rendering
        // handle 3d view rendering 
        public void ResetCameraView()
        {
            m_camera.ResetView(0, -200, 0, 20, 20);
            m_axisCam.ResetView(0, -100, 0, 20, 0);
            UpdateView();
        }

        public void UpdateView(bool update3D)
        {
            if (update3D)
                Render3dSpace = true;
            glControl1.Invalidate();
            if (UVDLPApp.Instance().SelectedObject != null)
            {
                
                if (UVDLPApp.Instance().SelectedObject.tag == Object3d.OBJ_SUPPORT)
                {
                    Support sup = (Support)UVDLPApp.Instance().SelectedObject;

                    RTUtils.UpdateObjectSelectionPlane(sup.Centroid(), m_camera.m_right, m_camera.m_up);
                }
                else
                {
                  
                    RTUtils.UpdateObjectSelectionPlane(UVDLPApp.Instance().SelectedObject.m_center,  m_camera.m_right, m_camera.m_up);
                }
            }
            //DisplayFunc();
        }

        public void UpdateView()
        {
            UpdateView(true);
        }


        private void SetupForMachineType()
        {
            MachineConfig mc = UVDLPApp.Instance().m_printerinfo;
            m_camera.UpdateBuildVolume((float)mc.m_PlatXSize, (float)mc.m_PlatYSize, (float)mc.m_PlatZSize);
        }

        protected void Set2DView()
        {
            gr2d.SetupViewport(glControl1.Width, glControl1.Height);
        }

        protected void Set3DView()
        {
            GL.CullFace(CullFaceMode.Back); // specify culling backfaces               
            if (!UVDLPApp.Instance().m_engine3d.m_alpha)
                GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            GL.LoadMatrix(ref m_projection);
            GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
            //GL.LoadMatrix(ref m_modelView);
            m_camera.SetViewGL();
        }

        private void SetupViewport()
        {
            if (!loaded)
                return;
            float aspect = 1.0f;
            try
            {
                int w = glControl1.Width;
                int h = glControl1.Height;

                GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
                aspect = ((float)glControl1.Width) / ((float)glControl1.Height);

                SetAlpha(false); // start off with alpha off

                GL.Enable(EnableCap.CullFace); // enable culling of faces
                GL.CullFace(CullFaceMode.Back); // specify culling backfaces               

                m_projection = OpenTK.Matrix4.CreatePerspectiveFieldOfView(0.55f, aspect, 1, 2000);
                m_axisProjection = OpenTK.Matrix4.CreatePerspectiveOffCenter(-2 * aspect + 0.3f, 0.3f, -0.3f, 1.7f, 1, 2000);
                m_modelView = OpenTK.Matrix4.LookAt(new OpenTK.Vector3(5, 0, -5), new OpenTK.Vector3(0, 0, 0), new OpenTK.Vector3(0, 0, 1));
                
                GL.ShadeModel(ShadingModel.Smooth); // tell it to shade smoothly

                // properties of materials
                GL.Enable(EnableCap.ColorMaterial); // allow polys to have color
                float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
                float[] mat_shininess = { 50.0f };
                GL.Material(MaterialFace.Front, MaterialParameter.Specular, mat_specular);
                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, mat_shininess);

                //set a color to clear the background
                GL.ClearColor(Color.LightBlue);

                // antialising lines
                GL.Enable(EnableCap.LineSmooth);
                //GL.Enable(EnableCap.PolygonSmooth);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
                //GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
                float[] res = new float[2];
                GL.GetFloat(GetPName.SmoothLineWidthRange, res);
               // DebugLogger.Instance().LogInfo("Stencil depth: " + glControl1.GraphicsMode.Stencil.ToString());

                // prepare texture buffer to save 3d rendring
                if (mColorBuffer == 0)
                    GL.GenTextures(1, out mColorBuffer);
                GL.BindTexture(TextureTarget.Texture2D, mColorBuffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);


                // lighting
                if (firstTime)
                {
                    GL.Enable(EnableCap.Lighting);
                    GL.Enable(EnableCap.Light0);
                    float[] light_position = { 1.0f, 1.0f, 1.0f, 0.0f };
                    GL.Light(LightName.Light0, LightParameter.Position, light_position);
                    GL.Light(LightName.Light0, LightParameter.Diffuse, Color.LightGray);
                }

                // load gui elements
                if (firstTime)
                {
                    gr2d.LoadTexture(global::UV_DLP_3D_Printer.Properties.Resources.cwtexture1_index);
                    gr2d.LoadFont("Arial18", global::UV_DLP_3D_Printer.Properties.Resources.Arial18_metrics);
                    LoadPluginGui();
                    //Visible = true;
                }

                Set3DView();
                UVDLPApp.Instance().PerformPluginCommand("AfterThemingDone", true);
                if (firstTime)
                    UVDLPApp.Instance().m_splashStop = true;
                firstTime = false;
            }
            catch (Exception ex)
            {
                String s = ex.Message;
                // the create perspective function blows up on certain ratios
            }
        }
 
        private void SetAlpha(bool val)
        {
            if (!loaded)
                return;

            if (val == true)
            {
                GL.Disable(EnableCap.DepthTest); // need to disable z buffering for proper display
                GL.Enable(EnableCap.AlphaTest);
                //GL.Disable(EnableCap.CullFace); // enable culling of faces
            }
            else
            {
                //GL.Enable(EnableCap.CullFace); // enable culling of faces
                GL.Disable(EnableCap.AlphaTest);
                GL.Enable(EnableCap.DepthTest); // for z buffer        
            }
        }

        void DrawBackground()
        {
            Set2DView();
            guiconf.DrawBackground(gr2d, glControl1.Width, glControl1.Height);

            //SetAlpha(m_showalpha);
            Set3DView();
        }

        void DrawForeground()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            Set2DView();
            /*gr2d.Rectangle(0,0,w,70,Color.RoyalBlue);
            GL.Color3(Color.White);
            gr2d.Image("cwlogo_round", w / 2 - 50, 0);*/
            if (!Render3dSpace)
            {
                gr2d.SetColor(Color.White);
                gr2d.Image((int)mColorBuffer, 0, 1, 1, 0, 0, 0, Width, Height);
            }
            guiconf.DrawForeground(gr2d, glControl1.Width, glControl1.Height);

            foreach (ctlBgnd cb in ctlBgndList)
            {
                gr2d.SetColor(cb.col);
                gr2d.Panel9(cb.imageName, cb.x, cb.y, cb.w, cb.h);
            }
            /*
            if (ctlViewOptions.SliceVisible && (m_sliceTex != -1))
            {
                int px = w - m_sliceViewW - 20;
                gr2d.SetColor(Color.FromArgb(50, 255, 255, 255));
                gr2d.Image(m_sliceTex, 0, 1, 0, 1, px, 90, m_sliceViewW, m_sliceViewH);
            }
            */
            foreach (Control subctl in mainViewSplitContainer.Panel2.Controls)
            {
                if (subctl.GetType().IsSubclassOf(typeof(ctlUserPanel)))
                {
                    ((ctlUserPanel)subctl).GLRedraw(gr2d, subctl.Location.X, subctl.Location.Y);
                }
            }
            
            gr2d.ResetDrawingRegion();
            //SetAlpha(m_showalpha);
            Set3DView();
        }

        private void DisplayFunc()
        {
            if (!loaded)
                return;

            glControl1.MakeCurrent();

            //glControl1.SelectBackPainting();
            /* Clear the buffer, clear the matrix */
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (Render3dSpace)
            {
                //GL.LoadIdentity(); // assuming we're in the model matrix still
                SetAlpha(UVDLPApp.Instance().m_engine3d.m_alpha);
                DrawBackground();

                UVDLPApp.Instance().Engine3D.RenderGL();
                //glControl1.SelectForePainting();
                //DrawISect();
                Render3dSlice();
                RenderAxisIndicator();

                GL.BindTexture(TextureTarget.Texture2D, mColorBuffer);
                GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 0, 0, Width, Height, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
                //GL.Flush();
            DrawForeground();
            GL.Flush();
            glControl1.SwapBuffers();
            if (Event3DViewRedraw != null)
                Event3DViewRedraw();
            Render3dSpace = false;
        }

        private void Render3dSlice()
        {
            if (m_curslice == null)
                return;
            if (UVDLPApp.Instance().m_appconfig.m_viewslice3d == false)
                return;
            /*
            if (m_curslice.m_opsegs == null)
            {
                m_curslice.Optimize();
                m_curslice.DetermineInteriorExterior(UVDLPApp.Instance().m_buildparms);
                m_curslice.ColorLines();
            }
             * */
            foreach (PolyLine3d ply in m_curslice.m_segments)
            {
                ply.RenderGL();
            }
        }

        private void RenderAxisIndicator()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref m_axisProjection);
            GL.MatrixMode(MatrixMode.Modelview);
            m_axisCam.SetViewGL();
            UVDLPApp.Instance().Engine3D.RenderAxisIndicator();
        }

        private List<ISectData> TestHitTest(int X, int Y)
        {
            if (!loaded)
                return null;
           // String mess = "";
           // mess = "Screen X,Y = (" + X.ToString() + "," + Y.ToString() + ")\r\n";

            /*
            (Note that most window systems place the mouse coordinate origin in the upper left of the window instead of the lower left. 
            That's why window_y is calculated the way it is in the above code. When using a glViewport() that doesn't match the window height,
            the viewport height and viewport Y are used to determine the values for window_y and norm_y.)

            The variables norm_x and norm_y are scaled between -1.0 and 1.0. Use them to find the mouse location on your zNear clipping plane like so:

            float y = near_height * norm_y;
            float x = near_height * aspect * norm_x;
            Now your pick ray vector is (x, y, -zNear).             
             */
            int w = glControl1.Width;
            int h = glControl1.Height;
          //  mess += "Screen Width/Height = " + w.ToString() + "," + h.ToString() + "\r\n";
            float aspect = ((float)glControl1.Width) / ((float)glControl1.Height);
            //mess += "Screen Aspect = " + aspect.ToString() + "\r\n";

            int window_y = (h - Y) - h / 2;
            double norm_y = (double)(window_y) / (double)(h / 2);
            int window_x = X - w / 2;
            double norm_x = (double)(window_x) / (double)(w / 2);
            float near_height = .2825f; // no detectable error

            float y = (float)(near_height * norm_y);
            float x = (float)(near_height * aspect * norm_x);

            /*
            To transform this eye coordinate pick ray into object coordinates, multiply it by the inverse of the ModelView matrix in use 
            when the scene was rendered. When performing this multiplication, remember that the pick ray is made up of a vector and a point, 
            and that vectors and points transform differently. You can translate and rotate points, but vectors only rotate. 
            The way to guarantee that this is working correctly is to define your point and vector as four-element arrays, 
            as the following pseudo-code shows:

            float ray_pnt[4] = {0.f, 0.f, 0.f, 1.f};
            float ray_vec[4] = {x, y, -near_distance, 0.f};
            The one and zero in the last element determines whether an array transforms as a point or a vector when multiplied by the 
            inverse of the ModelView matrix.*/
            Vector4 ray_pnt = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            //Vector4 ray_vec = new Vector4((float)norm_x, (float)norm_y, -1.0f, 0);
            Vector4 ray_vec = new Vector4((float)x, (float)y, -1f, 0);
            ray_vec.Normalize();

            //mess += "Eye Pick Vec =  (" + String.Format("{0:0.00}", ray_vec.X) + ", " + String.Format("{0:0.00}", ray_vec.Y) + "," + String.Format("{0:0.00}", ray_vec.Z) + ")\r\n";

            Matrix4 modelViewMatrix;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
            Matrix4 viewInv = Matrix4.Invert(modelViewMatrix);

            Vector4 t_ray_pnt = new Vector4();
            Vector4 t_ray_vec = new Vector4();

            Vector4.Transform(ref ray_vec, ref viewInv, out t_ray_vec);
            Vector4.Transform(ref ray_pnt, ref viewInv, out t_ray_pnt);
            //mess += "World Pick Vec =  (" + String.Format("{0:0.00}", t_ray_vec.X) + ", " + String.Format("{0:0.00}", t_ray_vec.Y) + "," + String.Format("{0:0.00}", t_ray_vec.Z) + ")\r\n";
            //mess += "World Pick Pnt =  (" + String.Format("{0:0.00}", t_ray_pnt.X) + ", " + String.Format("{0:0.00}", t_ray_pnt.Y) + "," + String.Format("{0:0.00}", t_ray_pnt.Z) + ")\r\n";

            Point3d origin = new Point3d();
            Point3d intersect = new Point3d();
            Engine3D.Vector3d dir = new Engine3D.Vector3d();

            origin.Set(t_ray_pnt.X, t_ray_pnt.Y, t_ray_pnt.Z);
            dir.Set(t_ray_vec.X, t_ray_vec.Y, t_ray_vec.Z); // should this be scaled?

            List<ISectData> isects = RTUtils.IntersectObjects(dir, origin, UVDLPApp.Instance().Engine3D.m_objects, true);
            if (isects.Count > 0)
            {

                foreach (ISectData isect in isects)
                {
                    if (!float.IsNaN(isect.intersect.x)) // check for NaN
                    {
                        /*
                        m_ix = (float)isect.intersect.x; // show the closest
                        m_iy = (float)isect.intersect.y;
                        m_iz = (float)isect.intersect.z;
                        */
                        isect.poly.CalcNormal();
                        m_isectnormal.x = isect.poly.m_normal.x;
                        m_isectnormal.y = isect.poly.m_normal.y;
                        m_isectnormal.z = isect.poly.m_normal.z;

                        break;
                    }
                }

                //ISectData isect = (ISectData)isects[0]; // get the first
                // check for NaN

            }

            return isects;
        }

        #endregion GL Rendering


        #region GL control events
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control) 
            {
                ctrldown = true;
            }
            // if the delete key is pressed, deleted the currently selected object 
            if (e.KeyCode == Keys.Delete)
            {
                UVDLPApp.Instance().RemoveCurrentModel();
            }
            if ((e.KeyCode == Keys.Z) && (e.Modifiers == Keys.Control))
            {
                UVDLPApp.Instance().m_undoer.Undo();
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                if (m_movingobjectmode == false)
                {
                    m_movingobjectmode = true;
                    Object3d obj = UVDLPApp.Instance().SelectedObject;
                    if (obj != null)
                    {
                        m_savex = obj.m_center.x;
                        m_savey = obj.m_center.y;
                        //m_savez = obj.m_center.z;
                        if (obj.tag == Object3d.OBJ_SUPPORT)
                        {
                            //obj.CalcMinMaxes();
                            obj.FindMinMax();
                            m_saveh = obj.m_max.z - obj.m_min.z;
                        }
                    }
                }
            }
        }

        private void glControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                ctrldown = false;
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                m_movingobjectmode = false;
                // update object info
                Object3d obj = UVDLPApp.Instance().SelectedObject;
                if (obj != null)
                {
                    m_savex = obj.m_center.x - m_savex;
                    m_savey = obj.m_center.y - m_savey;
                    //m_savez = obj.m_center.z - m_savez;
                    UVDLPApp.Instance().m_undoer.SaveTranslation(obj, m_savex, m_savey, 0);
                    if (obj.tag == Object3d.OBJ_SUPPORT)
                    {
                        //obj.CalcMinMaxes();
                        obj.FindMinMax();
                        m_saveh = (obj.m_max.z - obj.m_min.z) / m_saveh;
                        UVDLPApp.Instance().m_undoer.SaveScale(obj, 1, 1, m_saveh);
                        UVDLPApp.Instance().m_undoer.LinkToPrev();
                    }
                }

            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            m_context = glControl1.Context;
            glControl1.MouseWheel += new MouseEventHandler(glControl1_MouseWheel);
            SetupViewport();
            UVDLPApp.Instance().PerformPluginCommand("3dViewLoadedCommand", true);

        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            mdx = e.X;
            mdy = e.Y;
            try
            {
                if (e.Button == MouseButtons.Middle)
                {
                    mmdown = true;
                }

                if (e.Button == MouseButtons.Left)
                {
                    lmdown = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    rmdown = true;
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }

        private void glControl1_MouseLeave(object sender, EventArgs e)
        {
            //should cancel any move commands
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            
            double dx = 0, dy = 0;
            if (lmdown || rmdown || mmdown)
            {
                dx = e.X - mdx;
                dy = e.Y - mdy;
                mdx = e.X;
                mdy = e.Y;

            }
            dx /= 2;
            dy /= 2;

            if (lmdown)
            {
                m_camera.RotateRightFlat((float)dx);
                m_camera.RotateUp((float)dy);
                m_axisCam.RotateRightFlat((float)dx);
                m_axisCam.RotateUp((float)dy);

                UpdateView();
            }
            else if (mmdown)
            {
                m_camera.MoveForward((float)dy);
                UpdateView();
            }
            else if (rmdown)
            {
                m_camera.Move((float)dx, (float)dy);
                UpdateView();
            }

            // if no object selected, bail
            if (UVDLPApp.Instance().SelectedObject == null)
                return;
            
                if (m_movingobjectmode) // if we're moving an object - shift key down
                {
                    List<ISectData> hits = TestHitTest(e.X, e.Y); // hit-test all
                    // examine the last isect data
                    foreach (ISectData dat in hits)
                    {
                        //remember to break out of this foreach loop after executing a movement.

                        // either we're moving a support
                        if (UVDLPApp.Instance().SelectedObject.tag == Object3d.OBJ_SUPPORT)
                        {
                            // if it's the base we're moving,
                            //allow the base to change types
                            // see if it intersects with the ground, or an object
                            //cast as a support object
                            Support sup = (Support)UVDLPApp.Instance().SelectedObject;
                            if (sup.SelectionType == Support.eSelType.eWhole) 
                            {
                                // we're in modify mode, but we're still moving the whole support
                                if (dat.obj.tag == Object3d.OBJ_SEL_PLANE)
                                {                               
                                    //we should really try a top/ bottom intersection / scale to hieg
                                    // move the support                                     
                                    sup.Translate(
                                        (float)(dat.intersect.x - UVDLPApp.Instance().SelectedObject.m_center.x),
                                        (float)(dat.intersect.y - UVDLPApp.Instance().SelectedObject.m_center.y),
                                        0.0f);
                                    // now we've moved the object approximately to where it needs to be
                                    //turn it back into a base 
                                    sup.SubType = Support.eSubType.eBase;
                                    //get the center location
                                    Point3d centroid = sup.Centroid();
                                    Engine3D.Vector3d upvec = new Engine3D.Vector3d();
                                    upvec.Set(0, 0, 1);
                                    Point3d origin = new Point3d();
                                    origin.Set(centroid.x, centroid.y, .001f);// above the ground plane
                                    List<ISectData> isects = RTUtils.IntersectObjects(upvec, origin, UVDLPApp.Instance().Engine3D.m_objects, false);
                                    foreach (ISectData isd in isects) 
                                    {
                                        if (isd.obj.tag == Object3d.OBJ_NORMAL)// if we've intersected a normal object upwards
                                        {
                                            sup.SelectionType = Support.eSelType.eTip;
                                            sup.MoveFromTip(isd);
                                            sup.SelectionType = Support.eSelType.eWhole;
                                            break;      
                                        }
                                    }
                                    //starting at the x/y ground plane, hittest upward
                                    break;
                                }
                            }
                            else if (sup.SelectionType == Support.eSelType.eBase) 
                            {
                                //going to change this to test for intersection with object, or ground plane
                                // if intersected with an object, change to intra type
                                // and set the base on the object
                                // if intersected with ground, change to base type and put on ground
                                if (dat.obj.tag == Object3d.OBJ_GROUND) 
                                {
                                    // make sure we're a base tip
                                    sup.SubType = Support.eSubType.eBase;
                                    // position the bottom to the intersection point
                                    sup.PositionBottom(dat);
                                    break;
                                }
                                else if (dat.obj.tag == Object3d.OBJ_NORMAL) // intersected with an object 
                                {
                                    //should check with the normal of the object to see if it's facing upwards
                                    sup.SubType = Support.eSubType.eIntra;
                                    // position the bottom to the intersection point
                                    sup.PositionBottom(dat);
                                    break;
                                }
                            }
                            else if (sup.SelectionType == Support.eSelType.eTip) 
                            {
                                if (dat.obj.tag == Object3d.OBJ_NORMAL) // intersected with an object 
                                {

                                    sup.MoveFromTip(dat);                                        
                                    UpdateView();
                                    break;
                                }                                    
                            }
                        }
                        else // or a normal object based on object selection plane
                        {
                            if (dat.obj.tag == Object3d.OBJ_SEL_PLANE) 
                            {
                                UVDLPApp.Instance().SelectedObject.Translate(
                                    (float)(dat.intersect.x - UVDLPApp.Instance().SelectedObject.m_center.x),
                                    (float)(dat.intersect.y - UVDLPApp.Instance().SelectedObject.m_center.y),
                                    0.0f);                            
                            }
                            break;
                        }
                    }
                    UpdateView();
                }                
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                mmdown = false;
            }
            if (e.Button == MouseButtons.Left)
            {
                lmdown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rmdown = false;
            }

        }

        void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            m_camera.MoveForward(e.Delta / 10);
            UpdateView();
        }
        
        /*private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (m_splash != null)
                return;
            DisplayFunc();
        }*/

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            SetupViewport();
            CalcSliceLocation();
            UpdateView();
        }

        private void glControl1_DoubleClick(object sender, EventArgs e)
        {
            // the screen was double clicked
            // do object selection
            MouseEventArgs me = e as MouseEventArgs;
            MouseButtons buttonPushed = me.Button;
            int xPos = me.X;
            int yPos = me.Y;
            List<ISectData> isects = TestHitTest(xPos, yPos);
            // find the first object that's not the ground
            // this will allow us to select an object from the bottom.
            foreach (ISectData i in isects)
            {
                if (i.obj.tag != Object3d.OBJ_GROUND && i.obj.tag != Object3d.OBJ_SEL_PLANE)
                {
                    if (ModifierKeys == Keys.Control) // double click and control
                    {
                        UVDLPApp.Instance().AddToSelectionList(i.obj);
                        UVDLPApp.Instance().m_engine3d.UpdateLists();
                    }
                    else
                    {
                        if (i.obj.tag == Object3d.OBJ_SUPPORT) 
                        {
                            Support sup = (Support)i.obj;
                            sup.Select(i.poly);
                        }
                        UVDLPApp.Instance().SelectedObject = i.obj;
                        UVDLPApp.Instance().m_engine3d.UpdateLists();
                        UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eUpdateSelectedObject, "");
                    }
                    UpdateView();
                    break;
                }
            }
        }

        private void ctl3DView_Load(object sender, EventArgs e)
        {
            SetupForMachineType();
            Invalidate(true);
        }

        private void mainViewSplitContainer_Panel2_SizeChanged(object sender, EventArgs e)
        {
            // update inner control positions
            /*foreach (Control ctl in mainViewSplitContainer.Panel2.Controls)
            {
                if (ctl.GetType() == typeof(ctlImageButton))
                    continue;
                if (ctl.GetType().IsSubclassOf(typeof(ctlAnchorable)))
                    ((ctlAnchorable)ctl).UpdatePosition();
            }*/

            RearrangeGui();
        }

        #endregion GL control events

        #region 3d View buttons
        private void buttGlHome_Click(object sender, EventArgs e)
        {
            if (m_modelAnimTmr != null)
                return;
            m_camera.ResetViewAnim(0, -200, 0, 20, 20);
            m_axisCam.ResetViewAnim(0, -100, 0, 20, 0);
            m_modelAnimTmr = new Timer();
            m_modelAnimTmr.Interval = 25;
            m_modelAnimTmr.Tick += new EventHandler(m_modelAnimTmr_Tick);
            m_modelAnimTmr.Start();
        }

        private void buttGLTop_Click(object sender, EventArgs e)
        {
            if (m_modelAnimTmr != null)
                return;
            m_camera.ResetViewAnim(0, -200, 0, 90, 20);
            m_axisCam.ResetViewAnim(0, -100, 0, 90, 0);
            m_modelAnimTmr = new Timer();
            m_modelAnimTmr.Interval = 25;
            m_modelAnimTmr.Tick += new EventHandler(m_modelAnimTmr_Tick);
            m_modelAnimTmr.Start();
        }

        void m_modelAnimTmr_Tick(object sender, EventArgs e)
        {
            bool cameraon = m_camera.AnimTick();
            bool axison = m_axisCam.AnimTick();
            if ((cameraon == false) && (axison == false))
            {
                m_modelAnimTmr.Stop();
                m_modelAnimTmr = null;
            }
            UpdateView();
        }

        private void ShowPanel(ctlImageButton butt, string ctlname)
        {
            //ctlUserPanel ctl = guiconf.GetControl(ctlname);
            Control ctl = guiconf.GetControl(ctlname);
            if (ctl == null)
                return;

            if (ctl == m_selectedControl)
            {
                //butt.Gapx -= 5;
                m_pressedButt = null;
                ctl.Visible = false;
                m_selectedControl = null;
            }
            else
            {
                if (m_selectedControl != null)
                {
                    //m_pressedButt.Gapx -= 5;
                    m_pressedButt.Checked = false;
                    m_selectedControl.Visible = false;
                }
                m_pressedButt = butt;
                m_selectedControl = ctl;
                ctl.Visible = true;
            }
        }
        


        #endregion 3d View buttons


        #region 3d View controls


        void CalcSliceLocation()
        {
            if ((m_sliceH <= 0) || (m_sliceW <= 0))
                return;
            m_sliceViewH = Height / 2;
            m_sliceViewW = m_sliceViewH * m_sliceW / m_sliceH;
            if (m_sliceViewW > (Width / 2))
            {
                m_sliceViewW = Width / 2;
                m_sliceViewH = m_sliceViewW * m_sliceH / m_sliceW;
            }
        }

        void LoadSlice(int layer)
        {
            Bitmap bmp = null;
            bmp = UVDLPApp.Instance().m_slicefile.GetSliceImage(layer);
            if (bmp != null)
            {
                bmp.Tag = BuildManager.SLICE_NORMAL; // set the tag for later
                m_sliceW = bmp.Width;
                m_sliceH = bmp.Height;
                CalcSliceLocation();

                if (m_sliceTex != -1)
                    gr2d.DeleteTexture(m_sliceTex);
                m_sliceTex = gr2d.LoadTextureImage(bmp);
            }
        }

        public void ViewLayer(int layer)
        {
            try
            {
                if (UVDLPApp.Instance().m_appconfig.m_viewslice3d == true)
                {
                    if (UVDLPApp.Instance().m_slicefile != null) // check to make sure we have a slicefile
                    {
                        m_curslice = UVDLPApp.Instance().m_slicefile.GetSlice(layer);
                        //if (m_curslice != null) // vector slice can be null if we're loading this from zip/subdir
                        {
                            //UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "");
                            LoadSlice(layer);
                            UpdateView();
                        }
                    }
                }
                else
                {
                    m_curslice = null;
                }

            }
            catch (Exception ex) 
            {
                DebugLogger.Instance().LogError(ex.Message);
                DebugLogger.Instance().LogError(ex.StackTrace);
                m_curslice = null;
            }

        }
        
        private void numLayer_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int vscrollval = numLayer.IntVal - 1;
                scrollLayer.Value = scrollLayer.Maximum - vscrollval;
                numLayer.Refresh(); // redraw this
                double zlev = ((double)vscrollval * m_sf.m_config.ZThick);
                zlev = Math.Round(zlev, 3, MidpointRounding.ToEven);
                ctlInfoItemZLev.DataText = zlev.ToString();
                ctlInfoItemZLev.Refresh();
                ViewLayer(vscrollval);
            }
            catch (Exception)
            {
                // probably past the max.
            }
        }

        private void scrollLayer_ValueChanged(object sender, EventArgs e)
        {
            numLayer.IntVal = scrollLayer.Maximum - scrollLayer.Value + 1;
        }


        void UpdateButtonList()
        {
            // buttons
            guiconf.AddButton("home", buttGlHome);
            guiconf.AddButton("top", buttGLTop);
            guiconf.AddButton("undo", buttUndo); 
            guiconf.AddButton("redo", buttRedo);
            guiconf.AddControl("clayernum", numLayer);
            guiconf.AddControl("clayerZ", ctlInfoItemZLev);            
            guiconf.AddControl("clayerscroll", scrollLayer);
            guiconf.AddControl("glControl1", glControl1);
        }

        #endregion 3d View controls
        /// <summary>
        /// This function iterates through all the plugins
        /// and loads the configuration GUIConfig from each
        /// there is no reason why this needs to be in the ctl3DView
        /// this should go in the main UVDLPApp class
        /// </summary>
        public void LoadPluginGui()
        {
            IPlugin plugin = null;
            foreach (PluginEntry ip in UVDLPApp.Instance().m_plugins) 
            {
                try
                {
                    if (ip.m_licensed != true || ip.m_enabled == false)
                        continue;

                    plugin = ip.m_plugin;
                    if (plugin == null)
                        continue;

                    if (!plugin.SupportFunctionality(PluginFuctionality.CustomGUI))
                        continue;

                    string guiconfname = null;
                    foreach (PluginItem pi in plugin.GetPluginItems)
                    {
                        try
                        {
                            switch (pi.m_type)
                            {
                                case ePlItemType.eTexture:
                                    gr2d.LoadTexture(plugin.GetString(pi.m_name + "_index"), plugin);
                                    break;

                                case ePlItemType.eGuiConfig:
                                    guiconfname = plugin.GetString(pi.m_name);
                                    break;
                                    
                                case ePlItemType.eControl:
                                    UserControl ctl = plugin.GetControl(pi.m_name);
                                    if ((ctl.GetType() == typeof(ctlImageButton)) || ctl.GetType().IsSubclassOf(typeof(ctlImageButton)))
                                    {
                                        guiconf.AddButton(pi.m_name, (ctlImageButton)ctl);
                                    }
                                    else  //(ctl.GetType().IsSubclassOf(typeof(ctlUserPanel)))
                                    {
                                        guiconf.AddControl(pi.m_name, ctl);
                                    }
                                    break;
                                    
                            }
                        }
                        catch (Exception ex) 
                        {
                            DebugLogger.Instance().LogError(ex);
                        }
                    }
                    //after all the resource have been loaded from the GUI
                    // the 'script' xml layout portion of the GUI is loaded to do something with those resources
                    if ((guiconf != null) && (guiconfname != null))
                    {
                        //gc.ClearLayout();
                        // test new gui config system
                        GuiConfigDB gconfdb = new GuiConfigDB();
                        gconfdb.LoadConfiguration(guiconfname, plugin);
                        guiconf.ApplyConfiguration(gconfdb);
                        gconfdb.SaveConfiguration("GuiConfigPlugin.xml");
                        RearrangeGui();
                    }
                }
                catch (Exception ex) 
                {
                    DebugLogger.Instance().LogError(ex);
                }
            }
        }

        Support AddNewSupport(float x, float y, float lz, Object3d parent)
        {
            Support s = new Support();
            Configs.SupportConfig sc = UVDLPApp.Instance().m_supportconfig;
            //s.Create(sc,parent, (float)sc.fbrad, (float)sc.ftrad, (float)sc.hbrad, (float)sc.htrad, lz * .2f, lz * .6f, lz * .2f, 11);
            s.Create(sc, parent,  lz * .2f, lz * .6f, lz * .2f);
            s.Translate(x, y, 0);
            s.SetColor(Color.Yellow);
            if (parent != null)
                parent.AddSupport(s);
            UVDLPApp.Instance().m_engine3d.AddObject(s);
            UVDLPApp.Instance().SelectedObject = s;
            //RaiseSupportEvent(UV_DLP_3D_Printer.SupportEvent.eSupportGenerated, s.Name, s);
            UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eModelAdded, "");
            return s;
            
        }
        

        /// <summary>
        /// For now this is the editing mode for the currently selected support
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl1_Click(object sender, EventArgs e)
        {
            // single click on GL Control
            if (UVDLPApp.Instance().SupportEditMode == UVDLPApp.eSupportEditMode.eAddSupport) 
            {
                // if we're adding supports
                MouseEventArgs me = e as MouseEventArgs;
                // MouseButtons buttonPushed = me.Button;
                int xPos = me.X;
                int yPos = me.Y;
                List<ISectData> isects = TestHitTest(xPos, yPos);
                if (isects.Count == 0) return; // no intersections
                ISectData isd1 = null;
                foreach (ISectData isd in isects)
                {
                    // find the closest object we clicked
                    if (isd.obj.tag == Object3d.OBJ_NORMAL)
                    {
                        isd1 = isd; //  save it
                        break;
                    }
                }
                if (isd1 == null) return; // no object intersection
                //add a support 
                Support sup = AddNewSupport(isd1.intersect.x, isd1.intersect.y, isd1.intersect.z, isd1.obj);
                sup.SelectionType = Support.eSelType.eTip;
                sup.MoveFromTip(isd1);
                sup.SelectionType = Support.eSelType.eWhole;

                UpdateView();
                return;
            }            
        }

    }

    public class ctlBgnd
    {
        public int x, y;
        public int w, h;
        public Color col;
        public string imageName;
    }


}
