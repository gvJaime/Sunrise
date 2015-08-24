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
    public partial class frmAbout : Form
    {
        public frmAbout()
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
            version.Parent = pictureBox1;
            version.BackColor = Color.Transparent;
            version.Text = "Version " + Application.ProductVersion;
            LoadAbout();

        }
        private void LoadAbout() 
        {
             Bitmap bmp = UVDLPApp.Instance().GetPluginImage("About");
             if (bmp != null)
             {
                 label1.Hide();
                 label2.Hide();
                 label3.Hide();
                 label4.Hide();
                 version.Hide();
                 cmdDonate.Hide();
                 pictureBox1.Image = bmp;
             }
        }
        private void cmdDonate_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "";

                string business = "pacmanfan321@gmail.com";  // your paypal email
                string description = "Donation";            // '%20' represents a space. remember HTML!
                string country = "US";                  // AU, US, etc.
                string currency = "USD";                 // AUD, USD, etc.

                url += "https://www.paypal.com/cgi-bin/webscr" +
                    "?cmd=" + "_donations" +
                    "&business=" + business +
                    "&lc=" + country +
                    "&item_name=" + description +
                    "&currency_code=" + currency +
                    "&bn=" + "PP%2dDonationsBF";

                System.Diagnostics.Process.Start(url);
                //System.Diagnostics.Process.Start(target);
            }
            catch(Exception ex)
            {
                DebugLogger.Instance().LogError(ex.Message);
            }
        }
    }
}
