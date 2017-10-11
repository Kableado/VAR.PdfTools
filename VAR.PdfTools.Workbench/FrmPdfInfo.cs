using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            txtPdfPath.Text = Properties.Settings.Default.LastPdfPath;
            txtColumnName.Text = Properties.Settings.Default.LastColumnName;
            txtFieldName.Text = Properties.Settings.Default.LastFieldName;
            txtText.Text = Properties.Settings.Default.LastText;
        }

        private void FrmPdfInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LastPdfPath = txtPdfPath.Text;
            Properties.Settings.Default.LastColumnName = txtColumnName.Text;
            Properties.Settings.Default.LastFieldName = txtFieldName.Text;
            Properties.Settings.Default.LastText = txtText.Text;
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
                    lines.Add(string.Format("Text({0}, {1})({2}, {3})[{4}]: \"{5}\"",
                        textElement.Matrix.Matrix[0, 2], textElement.Matrix.Matrix[1, 2], textElement.VisibleWidth, textElement.VisibleHeight,
                        textElement.Font == null ? "#NULL#" : textElement.Font.Name,
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

        private void btnHasText_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);

            List<string> lines = new List<string>();
            int pageNum = 1;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                lines.Add(string.Format("Page({0}) : {1}", pageNum, Convert.ToString(extractor.HasText(txtText.Text))));
            }
            txtOutput.Lines = lines.ToArray();
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            if (File.Exists(txtPdfPath.Text) == false)
            {
                MessageBox.Show("File does not exist");
                return;
            }

            int MaxSize = 10000;

            PdfDocument doc = PdfDocument.Load(txtPdfPath.Text);
            string baseDocumentPath = Path.GetDirectoryName(txtPdfPath.Text);
            string baseDocumentFilename = Path.GetFileNameWithoutExtension(txtPdfPath.Text);

            List<string> lines = new List<string>();
            lines.Add(string.Format("Filename : {0}", baseDocumentFilename));
            lines.Add(string.Format("Number of Pages : {0}", doc.Pages.Count));

            int pageNumber = 1;
            foreach (PdfDocumentPage page in doc.Pages)
            {
                double pageXMin = double.MaxValue;
                double pageYMin = double.MaxValue;
                double pageXMax = double.MinValue;
                double pageYMax = double.MinValue;

                // Preprocess page to get max size
                PdfTextExtractor extractor = new PdfTextExtractor(page);
                foreach (PdfTextElement textElement in extractor.Elements)
                {
                    double textElementXMin = textElement.GetX();
                    double textElementYMax = textElement.GetY();
                    double textElementXMax = textElementXMin + textElement.VisibleWidth;
                    double textElementYMin = textElementYMax - textElement.VisibleHeight;

                    if (textElementXMax > pageXMax) { pageXMax = textElementXMax; }
                    if (textElementYMax > pageYMax) { pageYMax = textElementYMax; }
                    if (textElementXMin < pageXMin) { pageXMin = textElementXMin; }
                    if (textElementYMin < pageYMin) { pageYMin = textElementYMin; }
                }
                lines.Add(string.Format("Page {0:0000} TextElements : {1}", pageNumber, extractor.Elements.Count));

                // Prepare page image
                int pageWidth = (int)Math.Ceiling(pageXMax - pageXMin);
                int pageHeight = (int)Math.Ceiling(pageYMax - pageYMin);
                int Scale = 10;
                while ((pageWidth * Scale) > MaxSize) { Scale--; }
                while ((pageHeight * Scale) > MaxSize) { Scale--; }
                if (Scale <= 0) { Scale = 1; }
                using (Bitmap bmp = new Bitmap(pageWidth * Scale, pageHeight * Scale, PixelFormat.Format32bppArgb))
                using (Graphics gc = Graphics.FromImage(bmp))
                using (Pen penTextElem = new Pen(Color.Blue))
                {
                    gc.Clear(Color.White);

                    // Draw text elements
                    foreach (PdfTextElement textElement in extractor.Elements)
                    {
                        DrawTextElement(textElement, gc, penTextElem, Scale, pageHeight, pageXMin, pageYMin);
                    }

                    // Save image to disk
                    string fileName = Path.Combine(baseDocumentPath, string.Format("{0}_{1:0000}.png", baseDocumentFilename, pageNumber));
                    bmp.Save(fileName, ImageFormat.Png);
                }
                pageNumber++;
            }

            txtOutput.Lines = lines.ToArray();
        }
        
        private static void DrawTextElement(PdfTextElement textElement, Graphics gc, Pen penTextElem, int Scale, int pageHeight, double pageXMin, double pageYMin)
        {
            double textElementX = textElement.GetX() - pageXMin;
            double textElementY = textElement.GetY() - pageYMin;
            double textElementWidth = textElement.VisibleWidth;
            double textElementHeight = textElement.VisibleHeight;
            string textElementText = textElement.VisibleText;
            string textElementFontName = (textElement.Font == null ? string.Empty : textElement.Font.Name);

            if (textElementHeight < 0.0001) { return; }

            double textElementPageX = textElementX;
            double textElementPageY = pageHeight - textElementY;
            
            DrawRoundedRectangle(gc, penTextElem,
                (int)(textElementPageX * Scale),
                (int)(textElementPageY * Scale),
                (int)(textElementWidth * Scale),
                (int)(textElementHeight * Scale),
                Scale);


            using (Font font = new Font("Arial", (int)(textElementHeight * Scale), GraphicsUnit.Pixel))
            {
                foreach (PdfCharElement c in textElement.Characters)
                {
                    gc.DrawString(c.Char,
                        font,
                        Brushes.Black,
                        (int)((textElementPageX + c.Displacement) * Scale),
                        (int)(textElementPageY * Scale));
                    gc.FillRectangle(Brushes.Red,
                        (int)((textElementPageX + c.Displacement) * Scale),
                        (int)(textElementPageY * Scale),
                        2, 2);
                }
            }
        }
        
        public static GraphicsPath RoundedRect(int x, int y, int width, int height, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(x, y, diameter, diameter);
            GraphicsPath path = new GraphicsPath();
            
            // top left arc 
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = (x + width) - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = (y + height) - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = x;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void DrawRoundedRectangle(Graphics graphics, Pen pen, int x, int y, int width, int height, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(x, y, width, height, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }
        
    }
}
