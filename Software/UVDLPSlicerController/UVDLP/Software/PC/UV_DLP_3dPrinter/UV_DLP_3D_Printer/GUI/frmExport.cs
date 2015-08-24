using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UV_DLP_3D_Printer;
using System.Threading;
using System.IO;

namespace UV_DLP_3D_Printer.GUI
{
    public partial class frmExport : Form
    {
        List<CWExporter> Exporters;
        Thread m_thread;
        CWExporter m_exporter;
        Stream m_stream;
        string m_filename;

        public frmExport()
        {
            InitializeComponent();
            Exporters = new List<CWExporter>();
            comboExportType.Items.Clear();
            m_thread = null;
            m_stream = null;
        }

        public void RegisterExporter(CWExporter exporter)
        {
            Exporters.Add(exporter);
            comboExportType.Items.Add(exporter.Description);
            exporter.ExportProgress += new CWExporter.ExportProgressDeledgate(ExportProgressCallback);
        }

        private void ShowMessage(string msg)
        {
            if (msg == null)
                return;
            string[] message = msg.Split('|');
            string title = "Info";
            string text = "";
            if (message.Length >= 2)
            {
                title = message[0];
                text = message[1];
            }
            else
                text = message[0];
            MessageBox.Show(text, title);
        }

        public void ExportProgressCallback(CWExporter.eExportState state, string data)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { ExportProgressCallback(state, data); }));
            }
            else
            {
                switch (state)
                {
                    case CWExporter.eExportState.Start:
                        progressExport.Value = 0;
                        break;

                    case CWExporter.eExportState.Progress:
                        progressExport.Value = int.Parse(data);
                        break;

                    case CWExporter.eExportState.End:
                        progressExport.Value = 100;
                        break;

                    case CWExporter.eExportState.Close:
                        ShowMessage(data);
                        Hide();
                        break;
                }
            }
        }

        private void buttExport_Click(object sender, EventArgs e)
        {
            if (m_thread != null)
                return;

            if (comboExportType.SelectedIndex < 0)
            {
                MessageBox.Show("Select export type first", "Attention");
                return;
            }

            m_exporter = Exporters[comboExportType.SelectedIndex];

            string errMsg = m_exporter.ValidateExport();
            if (errMsg != null)
            {
                ShowMessage(errMsg);
                return;
            }

            try
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = string.Format("{0}(*.{1})|*.{1}|All files (*.*)|*.*", m_exporter.Description, m_exporter.FileExtension);
                if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_filename = sf.FileName;
                    m_stream = new FileStream(sf.FileName, FileMode.Create);
                    m_thread = new Thread(new ThreadStart(ExportingThread));
                    m_exporter.CancelExport = false;
                    m_thread.Start();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Instance().LogError(ex);
                if (m_stream != null)
                {
                    m_stream.Close();
                    File.Delete(m_filename);
                }
                m_stream = null;
            }
        }

        void ExportingThread()
        {
            string res = m_exporter.Export(m_stream, m_filename);
            ExportProgressCallback(CWExporter.eExportState.Close, res);
            m_stream.Close();
            if (m_exporter.CancelExport)
                File.Delete(m_filename);
            m_stream = null;
            m_thread = null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            buttCancel_Click(null, null);
            //Hide();
        }

        private void buttCancel_Click(object sender, EventArgs e)
        {
            if (m_thread != null && m_exporter != null)
                m_exporter.CancelExport = true;
            else
                Hide();
        }

        private void comboExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelSettings.Controls.Clear();
            CWExporter exporter = Exporters[comboExportType.SelectedIndex];
            Control ctlSettings = exporter.SettingPanel;
            if (ctlSettings != null)
            {
                ctlSettings.Location = new Point(0, 0);
                panelSettings.Controls.Add(ctlSettings);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            progressExport.Value = 0;
        }
    }
}
