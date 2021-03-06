﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VAR.PdfTools.PdfElements;

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
            var configuration = new Configuration();
            configuration.Load();
            txtPdfPath.Text = configuration.Get("LastPdfPath", string.Empty);
            txtField1.Text = configuration.Get("Field1", string.Empty);
            txtField2.Text = configuration.Get("Field2", string.Empty);
            txtField3.Text = configuration.Get("Field3", string.Empty);
            txtPages.Text = configuration.Get("Pages", string.Empty);
            chkRender.Checked = configuration.Get("Render", false);
        }

        private void FrmPdfInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            var configuration = new Configuration();
            var configItems = new Dictionary<string, string>();
            configuration.Set("LastPdfPath", txtPdfPath.Text);
            configuration.Set("Field1", txtField1.Text);
            configuration.Set("Field2", txtField2.Text);
            configuration.Set("Field3", txtField3.Text);
            configuration.Set("Pages", txtPages.Text);
            configuration.Set("Render", chkRender.Checked);
            configuration.Save();
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
                    string fontName = textElement.Font == null ? "#NULL#" : textElement.Font.Name;
                    if (fontName == "#NULL#" && textElement.Childs.Count > 0)
                    {
                        var fontNames = textElement.Childs.Select(c => c.Font == null ? "#NULL#" : c.Font.Name);
                        StringBuilder sbFontName = new StringBuilder();
                        foreach (string fontNameAux in fontNames)
                        {
                            if (sbFontName.Length > 0) { sbFontName.Append(";"); }
                            sbFontName.Append(fontNameAux);
                        }
                        fontName = sbFontName.ToString();
                    }

                    lines.Add(string.Format("Text({0}, {1})({2}, {3})[{4}]: \"{5}\"",
                        Math.Round(textElement.Matrix.Matrix[0, 2], 2),
                        Math.Round(textElement.Matrix.Matrix[1, 2], 2),
                        Math.Round(textElement.VisibleWidth, 2),
                        Math.Round(textElement.VisibleHeight, 2),
                        fontName,
                        textElement.VisibleText));
                }
            }

            txtOutput.Lines = lines.ToArray();
        }

        private void btnHasText1_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string text = txtField1.Text;

            Action_HasText(pdfPath, text);
        }

        private void btnGetField1_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string field = txtField1.Text;

            Action_GetField(pdfPath, field);
        }

        private void btnGetColumn1_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string column = txtField1.Text;

            Action_GetColumn(pdfPath, column);
        }

        private void btnHasText2_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string text = txtField2.Text;

            Action_HasText(pdfPath, text);
        }

        private void btnGetField2_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string field = txtField2.Text;

            Action_GetField(pdfPath, field);
        }

        private void btnGetColumn2_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string column = txtField2.Text;

            Action_GetColumn(pdfPath, column);
        }

        private void btnHasText3_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string text = txtField3.Text;

            Action_HasText(pdfPath, text);
        }

        private void btnGetField3_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string field = txtField3.Text;

            Action_GetField(pdfPath, field);
        }

        private void btnGetColumn3_Click(object sender, EventArgs e)
        {
            string pdfPath = txtPdfPath.Text;
            string column = txtField3.Text;

            Action_GetColumn(pdfPath, column);
        }

        private IEnumerable<int> GetSelectedPages(int maxPages)
        {
            string pages = txtPages.Text;
            if (string.IsNullOrEmpty(pages))
            {
                return Enumerable.Range(1, maxPages);
            }

            string[] pagesParts;
            if (pages.Contains(","))
            {
                pagesParts = pages.Split(',');
            }
            else
            {
                pagesParts = new string[] { pages };
            }
            List<int> listPages = new List<int>();
            foreach (string part in pagesParts)
            {
                if (part.Contains("-"))
                {
                    string[] range = part.Split('-');
                    if (range.Length == 2)
                    {
                        int pageStart;
                        int pageEnd;
                        if (int.TryParse(range[0], out pageStart) && int.TryParse(range[1], out pageEnd))
                        {
                            listPages.AddRange(Enumerable.Range(pageStart, (pageEnd - pageStart) + 1));
                        }
                    }
                }
                else
                {
                    int pageNum;
                    if (int.TryParse(part, out pageNum))
                    {
                        listPages.Add(pageNum);
                    }
                }
            }
            if (listPages.Count == 0)
            {
                listPages.AddRange(Enumerable.Range(1, maxPages));
            }
            return listPages;
        }

        private void Action_HasText(string pdfPath, string text)
        {
            if (System.IO.File.Exists(pdfPath) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(pdfPath);

            IEnumerable<int> selectedPages = GetSelectedPages(doc.Pages.Count);
            List<string> lines = new List<string>();
            int pageNum = 0;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                pageNum++;
                if (selectedPages.Contains(pageNum) == false) { continue; }
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                lines.Add(string.Format("Page({0}) : {1}", pageNum, Convert.ToString(extractor.HasText(text))));
            }
            txtOutput.Lines = lines.ToArray();
        }

        private void Action_GetField(string pdfPath, string field)
        {
            if (System.IO.File.Exists(pdfPath) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(pdfPath);

            IEnumerable<int> selectedPages = GetSelectedPages(doc.Pages.Count);
            var fieldData = new List<string>();
            int pageNum = 0;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                pageNum++;
                if (selectedPages.Contains(pageNum) == false) { continue; }
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                fieldData.Add(extractor.GetFieldAsString(field));
            }
            txtOutput.Lines = fieldData.ToArray();
        }

        private void Action_GetColumn(string pdfPath, string column)
        {
            if (System.IO.File.Exists(pdfPath) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(pdfPath);
            string baseDocumentPath = Path.GetDirectoryName(txtPdfPath.Text);
            string baseDocumentFilename = Path.GetFileNameWithoutExtension(txtPdfPath.Text);

            IEnumerable<int> selectedPages = GetSelectedPages(doc.Pages.Count);
            var columns = new List<string>();
            int pageNum = 0;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                pageNum++;
                if (selectedPages.Contains(pageNum) == false) { continue; }
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                PdfTextElementColumn columnData;
                if (column.StartsWith("#"))
                {
                    string[] columnParts = column.Substring(1).Split(';');
                    double y = Convert.ToDouble(columnParts[0]);
                    double x1 = Convert.ToDouble(columnParts[1]);
                    double x2 = Convert.ToDouble(columnParts[2]);
                    columnData = extractor.GetColumn(null, y, x1, x2, x1, x2);
                }
                else
                {
                    columnData = extractor.GetColumn(column);
                }
                if (chkRender.Checked)
                {
                    var pdfPageRenderer = new PdfPageRenderer(extractor);
                    Bitmap bmp = pdfPageRenderer.Render();
                    pdfPageRenderer.RenderColumn(columnData, bmp);
                    string fileName = Path.Combine(baseDocumentPath, string.Format("{0}_{1:0000}.png", baseDocumentFilename, pageNum));
                    bmp.Save(fileName, ImageFormat.Png);
                    bmp.Dispose();
                    GC.Collect();
                }
                columns.AddRange(columnData.Elements.Select(t => t.VisibleText));
            }
            txtOutput.Lines = columns.ToArray();
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            if (File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);
            string baseDocumentPath = Path.GetDirectoryName(txtPdfPath.Text);
            string baseDocumentFilename = Path.GetFileNameWithoutExtension(txtPdfPath.Text);

            List<string> lines = new List<string>();
            lines.Add(string.Format("Filename : {0}", baseDocumentFilename));
            lines.Add(string.Format("Number of Pages : {0}", doc.Pages.Count));

            IEnumerable<int> selectedPages = GetSelectedPages(doc.Pages.Count);
            int pageNum = 0;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                pageNum++;
                if (selectedPages.Contains(pageNum) == false) { continue; }

                PdfPageRenderer pdfPageRenderer = new PdfPageRenderer(page);
                Bitmap bmp = pdfPageRenderer.Render();

                lines.Add(string.Format("Page {0:0000} TextElements : {1}", pageNum, pdfPageRenderer.Extractor.Elements.Count));

                // Save image to disk
                string fileName = Path.Combine(baseDocumentPath, string.Format("{0}_{1:0000}.png", baseDocumentFilename, pageNum));
                bmp.Save(fileName, ImageFormat.Png);
                bmp.Dispose();
                GC.Collect();
            }

            txtOutput.Lines = lines.ToArray();
        }
    }
}
