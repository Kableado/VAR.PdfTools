using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VAR.PdfTools.Workbench
{
    public partial class FrmPdfInfo : Form
    {
        public FrmPdfInfo()
        {
            InitializeComponent();
        }

        private void FrmPdfInfo_Load(object sender, EventArgs e)
        {
            txtPdfPath.Text = Properties.Settings.Default.LastPdfPath;
        }

        private void FrmPdfInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastPdfPath = txtPdfPath.Text;
            Properties.Settings.Default.Save();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dlgFile = new OpenFileDialog();
            DialogResult result = dlgFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtPdfPath.Text = dlgFile.FileName;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);

            int nObjects = doc.Objects.Count;
            List<PdfStream> streams = doc.Objects.Where(obj => obj.Data.Type == PdfElementTypes.Stream).Select(obj => (PdfStream)obj.Data).ToList();
            int nStreams = streams.Count;
            List<PdfStream> streamsWithFilters = streams
                .Where(stream => stream.Dictionary.Values.ContainsKey("Filter"))
                .ToList();
            var streamFilters = new List<string>();
            foreach(PdfStream stream in streamsWithFilters)
            {
                IPdfElement filter = stream.Dictionary.Values["Filter"];
                if (filter is PdfArray)
                {
                    filter = ((PdfArray)filter).Values[0];
                }
                if (filter is PdfName)
                {
                    streamFilters.Add(((PdfName)filter).Value);
                }
            }

            txtOutput.Lines = new string[]
            {
                string.Format("Number of Objects: {0}", nObjects),
                string.Format("Number of Streams: {0}", nStreams),
                string.Format("Unsuported Stream Filters: {0}", string.Join(", ", streamFilters.Distinct().ToArray())),
            };

        }

    }
}
