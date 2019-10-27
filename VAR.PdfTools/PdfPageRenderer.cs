using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VAR.PdfTools
{
    public class PdfPageRenderer
    {
        private PdfDocumentPage _page;
        private PdfTextExtractor _pdfTextExtractor;

        private const int MaxSize = 10000;

        public PdfTextExtractor Extractor { get { return _pdfTextExtractor; } }

        public PdfPageRenderer(PdfDocumentPage page)
        {
            _page = page;
            _pdfTextExtractor = new PdfTextExtractor(_page);
        }

        public Bitmap Render()
        {
            if (_pdfTextExtractor.Elements.Count == 0)
            {
                Bitmap emptyBmp = new Bitmap(100, 200, PixelFormat.Format32bppArgb);
                return emptyBmp;
            }

            double pageXMin = double.MaxValue;
            double pageYMin = double.MaxValue;
            double pageXMax = double.MinValue;
            double pageYMax = double.MinValue;

            // Preprocess page to get max size
            foreach (PdfTextElement textElement in _pdfTextExtractor.Elements)
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

            // Prepare page image
            int pageWidth = (int)Math.Ceiling(pageXMax - pageXMin);
            int pageHeight = (int)Math.Ceiling(pageYMax - pageYMin);
            int Scale = 10;
            while ((pageWidth * Scale) > MaxSize) { Scale--; }
            while ((pageHeight * Scale) > MaxSize) { Scale--; }
            if (Scale <= 0) { Scale = 1; }
            Bitmap bmp = new Bitmap(pageWidth * Scale, pageHeight * Scale, PixelFormat.Format32bppArgb);
            using (Graphics gc = Graphics.FromImage(bmp))
            using (Pen penTextElem = new Pen(Color.Blue))
            using (Pen penCharElem = new Pen(Color.Navy))
            {
                gc.Clear(Color.White);

                // Draw text elements
                foreach (PdfTextElement textElement in _pdfTextExtractor.Elements)
                {
                    DrawTextElement(textElement, gc, penTextElem, penCharElem, Scale, pageHeight, pageXMin, pageYMin, Brushes.Black);
                }
            }
            return bmp;
        }

        private static void DrawTextElement(PdfTextElement textElement, Graphics gc, Pen penTextElem, Pen penCharElem, int Scale, int pageHeight, double pageXMin, double pageYMin, Brush brushText)
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

            if (penTextElem != null)
            {
                DrawRoundedRectangle(gc, penTextElem,
                    (int)(textElementPageX * Scale),
                    (int)(textElementPageY * Scale),
                    (int)(textElementWidth * Scale),
                    (int)(textElementHeight * Scale),
                    5);
            }

            using (Font font = new Font("Arial", (int)(textElementHeight * Scale), GraphicsUnit.Pixel))
            {
                foreach (PdfCharElement c in textElement.Characters)
                {
                    gc.DrawString(c.Char,
                        font,
                        brushText,
                        (int)((textElementPageX + c.Displacement) * Scale),
                        (int)(textElementPageY * Scale));
                    if (penCharElem != null)
                    {
                        DrawRoundedRectangle(gc, penCharElem,
                            (int)((textElementPageX + c.Displacement) * Scale),
                            (int)(textElementPageY * Scale),
                            (int)(c.Width * Scale),
                            (int)(textElementHeight * Scale),
                            5);
                    }
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
