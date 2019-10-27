using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using VAR.PdfTools.Maths;

namespace VAR.PdfTools
{
    public class PdfPageRenderer
    {
        private PdfDocumentPage _page;
        private PdfTextExtractor _pdfTextExtractor;
        private Rect _pageRect;
        private int _pageWidth;
        private int _pageHeight;
        private int _scale = 10;

        private const int MaxSize = 10000;


        public PdfTextExtractor Extractor { get { return _pdfTextExtractor; } }

        public PdfPageRenderer(PdfDocumentPage page)
        {
            _page = page;
            _pdfTextExtractor = new PdfTextExtractor(_page);
        }

        public PdfPageRenderer(PdfTextExtractor pdfTextExtractor)
        {
            _pdfTextExtractor = pdfTextExtractor;
            _page = pdfTextExtractor.Page;


            // Calculate page size and scale
            _pageRect = _pdfTextExtractor.GetRect();
            _pageWidth = (int)Math.Ceiling(_pageRect.XMax - _pageRect.XMin);
            _pageHeight = (int)Math.Ceiling(_pageRect.YMax - _pageRect.YMin);
            while ((_pageWidth * _scale) > MaxSize) { _scale--; }
            while ((_pageHeight * _scale) > MaxSize) { _scale--; }
            if (_scale <= 0) { _scale = 1; }
        }

        public Bitmap Render()
        {
            if (_pdfTextExtractor.Elements.Count == 0)
            {
                // Nothing to render
                Bitmap emptyBmp = new Bitmap(100, 200, PixelFormat.Format32bppArgb);
                using (Graphics gcEmpty = Graphics.FromImage(emptyBmp))
                    gcEmpty.Clear(Color.White);
                return emptyBmp;
            }

            // Prepare image
            Bitmap bmp = new Bitmap(_pageWidth * _scale, _pageHeight * _scale, PixelFormat.Format32bppArgb);
            Graphics gc = Graphics.FromImage(bmp);
            gc.Clear(Color.White);

            // Draw text elements of the page
            using (Pen penTextElem = new Pen(Color.Blue))
            using (Pen penCharElem = new Pen(Color.Navy))
            {
                foreach (PdfTextElement textElement in _pdfTextExtractor.Elements)
                {
                    DrawTextElement(textElement, gc, penTextElem, penCharElem, _scale, _pageHeight, _pageRect.XMin, _pageRect.YMin, Brushes.Black);
                }
            }

            gc.Dispose();
            return bmp;
        }

        public Bitmap RenderColumn(PdfTextElementColumn columnData, Bitmap bmp = null)
        {
            Graphics gc;
            if (bmp == null)
            {
                bmp = new Bitmap(_pageWidth * _scale, _pageHeight * _scale, PixelFormat.Format32bppArgb);
                gc = Graphics.FromImage(bmp);
                gc.Clear(Color.White);
            }
            else
            {
                gc = Graphics.FromImage(bmp);
            }

            // Draw text elements of the column
            using (Pen penTextElem = new Pen(Color.Red))
            using (Pen penCharElem = new Pen(Color.DarkRed))
            {
                foreach (PdfTextElement textElement in columnData.Elements)
                {
                    DrawTextElement(textElement, gc, penTextElem, penCharElem, _scale, _pageHeight, _pageRect.XMin, _pageRect.YMin, Brushes.OrangeRed);
                }
            }

            // Draw column extents
            using (Pen penColumn = new Pen(Color.Red))
            {
                float y = (float)(_pageRect.YMax - columnData.Y);
                float x1 = (float)(columnData.X1 - _pageRect.XMin);
                float x2 = (float)(columnData.X2 - _pageRect.XMin);

                gc.DrawLine(penColumn, x1 * _scale, y * _scale, x2 * _scale, y * _scale);
                gc.DrawLine(penColumn, x1 * _scale, y * _scale, x1 * _scale, _pageHeight * _scale);
                gc.DrawLine(penColumn, x2 * _scale, y * _scale, x2 * _scale, _pageHeight * _scale);
            }

            gc.Dispose();
            return bmp;
        }

        private static void DrawTextElement(PdfTextElement textElement, Graphics gc, Pen penTextElem, Pen penCharElem, int scale, int pageHeight, double pageXMin, double pageYMin, Brush brushText)
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
                    (int)(textElementPageX * scale),
                    (int)(textElementPageY * scale),
                    (int)(textElementWidth * scale),
                    (int)(textElementHeight * scale),
                    5);
            }

            using (Font font = new Font("Arial", (int)(textElementHeight * scale), GraphicsUnit.Pixel))
            {
                foreach (PdfCharElement c in textElement.Characters)
                {
                    gc.DrawString(c.Char,
                        font,
                        brushText,
                        (int)((textElementPageX + c.Displacement) * scale),
                        (int)(textElementPageY * scale));
                    if (penCharElem != null)
                    {
                        DrawRoundedRectangle(gc, penCharElem,
                            (int)((textElementPageX + c.Displacement) * scale),
                            (int)(textElementPageY * scale),
                            (int)(c.Width * scale),
                            (int)(textElementHeight * scale),
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
