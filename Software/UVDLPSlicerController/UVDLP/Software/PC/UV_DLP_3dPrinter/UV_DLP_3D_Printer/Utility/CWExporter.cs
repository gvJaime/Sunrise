using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace UV_DLP_3D_Printer
{
    public abstract class CWExporter
    {
        public enum eExportState
        {
            Start,
            Progress,
            End,
            Close,
            Error
        }

        public abstract string FileExtension { get; }
        public abstract string Description { get; }
        public abstract string Export(Stream stream, string filename); // returns result string i.e: "Success|Export successfuly completed"
        public delegate void ExportProgressDeledgate(eExportState state, string data);
        public event ExportProgressDeledgate ExportProgress;
        public virtual Control SettingPanel
        {
            get { return null; }
        }
        public virtual void ReadSettings() { }
        public bool CancelExport;
        public virtual void ReportStart()
        {
            if (ExportProgress != null)
                ExportProgress(eExportState.Start, null);
        }
        public virtual void ReportProgress(int percent)
        {
            if (ExportProgress != null)
                ExportProgress(eExportState.Progress, percent.ToString());
        }
        public virtual string ValidateExport()
        {
            return null; // returns null if export is valid or result string if not
        }
    }
}
