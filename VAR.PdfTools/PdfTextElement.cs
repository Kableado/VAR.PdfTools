using System.Collections.Generic;
using System.Linq;
using VAR.PdfTools.Maths;

namespace VAR.PdfTools
{
    public struct PdfCharElement
    {
        public string Char;
        public double Displacement;
        public double Width;
    }

    public class PdfTextElement
    {
        #region Properties

        public PdfFont Font { get; set; }

        public double FontSize { get; set; }

        public Matrix3x3 Matrix { get; set; }

        public string RawText { get; set; }

        public string VisibleText { get; set; }

        public double VisibleWidth { get; set; }

        public double VisibleHeight { get; set; }

        public List<PdfCharElement> Characters { get; set; }

        public List<PdfTextElement> Childs { get; set; }

        #endregion

        #region Public methods

        public double GetX()
        {
            return Matrix.Matrix[0, 2];
        }

        public double GetY()
        {
            return Matrix.Matrix[1, 2];
        }

        public PdfTextElement SubPart(int startIndex, int endIndex)
        {
            PdfTextElement blockElem = new PdfTextElement
            {
                Font = null,
                FontSize = FontSize,
                Matrix = Matrix.Copy(),
                RawText = RawText.Substring(startIndex, endIndex - startIndex),
                VisibleText = VisibleText.Substring(startIndex, endIndex - startIndex),
                VisibleWidth = 0,
                VisibleHeight = VisibleHeight,
                Characters = new List<PdfCharElement>(),
                Childs = new List<PdfTextElement>(),
            };
            double displacement = Characters[startIndex].Displacement;
            blockElem.Matrix.Matrix[0, 2] += displacement;
            for (int j = startIndex; j < endIndex; j++)
            {
                blockElem.Characters.Add(new PdfCharElement
                {
                    Char = Characters[j].Char,
                    Displacement = Characters[j].Displacement - displacement,
                    Width = Characters[j].Width,
                });
            }
            PdfCharElement lastChar = blockElem.Characters[blockElem.Characters.Count - 1];
            blockElem.VisibleWidth = lastChar.Displacement + lastChar.Width;
            foreach (PdfTextElement elem in Childs)
            {
                blockElem.Childs.Add(elem);
            }

            return blockElem;
        }

        public double MaxWidth()
        {
            return Characters.Average(c => c.Width);
        }

        #endregion
    }

}
