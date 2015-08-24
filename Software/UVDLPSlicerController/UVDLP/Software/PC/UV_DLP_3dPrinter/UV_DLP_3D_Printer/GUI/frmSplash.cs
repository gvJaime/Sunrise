using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmSplash : Form
    {
        Timer m_timer;
        private int m_total = 0;
        int max = 200;
        public frmSplash()
        {
            InitializeComponent();
            label1.Parent = pictureBox1;
            label1.BackColor = Color.Transparent;
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
            label3.Parent = pictureBox1;
            label3.BackColor = Color.Transparent;
            label4.Parent = pictureBox1;
            label4.BackColor = Color.Transparent;
            label5.Parent = pictureBox1;
            label5.BackColor = Color.Transparent;

            version.Parent = pictureBox1;
            version.BackColor = Color.Transparent;
            lblbuilddate.Parent = pictureBox1;
            lblbuilddate.BackColor = Color.Transparent;
            
            version.Text = "Version " + Application.ProductVersion;
            lblbuilddate.Text = "Build Date: " + Utility.RetrieveLinkerTimestamp().ToString();

            LoadPluginSplash();
            RemoveMessages();
            m_timer = new Timer();
            m_timer.Interval = 20;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            
            Opacity = 0.0;
        }
        private void RemoveMessages() 
        {
            label5.Visible = false;
            label2.Visible = false;
            Refresh();
        
        }
        private void LoadPluginSplash() 
        {
            Bitmap bmp = UVDLPApp.Instance().GetPluginImage("Splash");
            if (bmp != null) 
            {
                //set the width and height of the form
                this.Width = bmp.Width;
                this.Height = bmp.Height;
                pictureBox1.Image = bmp;
                //hide the control labels for the default version
                label1.Visible = false;
                label2.Visible = false;
                version.Visible = false;
                lblbuilddate.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                Refresh();
            }
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (UVDLPApp.Instance().m_splashStop && (m_total < (max - 50)))
                {
                    m_total = max - 50;
                    Visible = false;
                    Update();
                    Visible = true;
                    //this.Opacity = 1;
                    //Update();
                }
                if (m_total >= max)// check for closing
                {
                    m_timer.Stop();
                    Close();
                    UVDLPApp.Instance().RaiseAppEvent(eAppEvent.eReDraw, "splash end");
                    return;
                }
                
                if (m_total > (max - 10)) // fade out
                {
                    this.Opacity -= .1;
                }

                if (m_total < 10) // fade in 
                {
                    this.Opacity += .1;
                }
                m_total++;
                
            }
            catch (Exception ex) 
            {
                m_timer.Stop();
                DebugLogger.Instance().LogError(ex.Message);
                Close();
            }
        }
    }
}
