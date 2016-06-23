﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            txtColumnName.Text = Properties.Settings.Default.LastColumnName;
            txtFieldName.Text = Properties.Settings.Default.LastFieldName;
        }

        private void FrmPdfInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastPdfPath = txtPdfPath.Text;
            Properties.Settings.Default.LastColumnName = txtColumnName.Text;
            Properties.Settings.Default.LastFieldName = txtFieldName.Text;
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
            int nRootObject = doc.Objects.Where(obj => obj.UsageCount == 0).Count();
            List<PdfStream> streams = doc.Objects
                .Where(obj => obj.Data.Type == PdfElementTypes.Stream)
                .Select(obj => (PdfStream)obj.Data)
                .ToList();
            int nStreams = streams.Count;
            int nPages = doc.Pages.Count;

            List<string> lines = new List<string>();
            lines.Add(string.Format("Filename : {0}", System.IO.Path.GetFileNameWithoutExtension(txtPdfPath.Text)));
            lines.Add(string.Format("Number of Objects : {0}", nObjects));
            lines.Add(string.Format("Number of Roots   : {0}", nRootObject));
            lines.Add(string.Format("Number of Streams : {0}", nStreams));
            lines.Add(string.Format("Number of Pages   : {0}", nPages));

            int pageNumber = 1;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                lines.Add("-----------------------------------------------------------------------------------------");
                if (page.BaseData.Values.ContainsKey("CropBox"))
                {
                    PdfArray cropBox = page.BaseData.Values["CropBox"] as PdfArray;
                    lines.Add(string.Format("Page({0} of {1}): {2} {3} {4} {5}", pageNumber, doc.Pages.Count,
                            PdfElementUtils.GetReal(cropBox.Values[0], 0),
                            PdfElementUtils.GetReal(cropBox.Values[1], 0),
                            PdfElementUtils.GetReal(cropBox.Values[2], 0),
                            PdfElementUtils.GetReal(cropBox.Values[3], 0)));
                }
                else
                {
                    lines.Add(string.Format("Page({0} of {1}): ", pageNumber, doc.Pages.Count));
                }
                pageNumber++;

                PdfTextExtractor extractor = new PdfTextExtractor(page);
                foreach (PdfTextElement textElement in extractor.Elements)
                {
                    lines.Add(string.Format("Text({0}, {1})({2}, {3}): \"{4}\"", 
                        textElement.Matrix.Matrix[0, 2], textElement.Matrix.Matrix[1, 2], textElement.VisibleWidth, textElement.VisibleHeight, 
                        textElement.VisibleText));
                }
            }

            txtOutput.Lines = lines.ToArray();
        }

        private void btnGetColumn_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);
            
            var columnData = new List<string>();
            foreach (PdfDocumentPage page in doc.Pages)
            {
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                columnData.AddRange(extractor.GetColumn(txtColumnName.Text));
            }
            txtOutput.Lines = columnData.ToArray();
        }

        private void btnGetField_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);
            
            var fieldData = new List<string>();
            foreach (PdfDocumentPage page in doc.Pages)
            {
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                fieldData.Add(extractor.GetField(txtFieldName.Text));
            }
            txtOutput.Lines = fieldData.ToArray();
        }
    }
}
